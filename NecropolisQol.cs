using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;
using ExileCore;
using ExileCore.PoEMemory;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.Elements.Necropolis;
using ExileCore.PoEMemory.FilesInMemory;
using ExileCore.Shared.Helpers;
using GameOffsets.Components;
using NecropolisQol.Models;
using SharpDX;
using Vector2 = System.Numerics.Vector2;

namespace NecropolisQoL;

public class NecropolisQol : BaseSettingsPlugin<NecropolisQolSettings>
{
    public static NecropolisQol Main;

    public override bool Initialise()
    {
        Main = this;
        Settings.LoadMods();
        Settings.LoadMinTiers();

        return base.Initialise();
    }

    public override void Render()
    {
        RenderModStuff();
    }

    private Dictionary<String, int> ListOfNormalTiers = new Dictionary<string, int>();

    public void RenderModStuff()
    {
        NecropolisMonsterPanel necropolisMonsterPanel = GameController.IngameState.IngameUi.NecropolisMonsterPanel;


        if (necropolisMonsterPanel is
            {
                IsVisible: true
            })
        {
            // Pull the data for us to process
            GetModelLists(necropolisMonsterPanel, out List<ModModel> mods, out List<MonsterModel> monsters);

            // Check if we found any new mods
            List<string> newMods = new List<string>();
            foreach(var mod in mods)
            {
                if(!Settings.AllMods.Contains(mod.Name))
                    newMods.Add(mod.Name);
            }

            // If we had new mods, add them to our mod list and file
            if (newMods.Count > 0) { Settings.AddMods(newMods); }

            monsters.ForEach(x => CalculateMonsterValue(x));

            // Loop through each mod so that we pre-calculate mod values.
            // No need to order this list as its reordered for each monster based on danger
            foreach(var mod in mods)
            {
                CalculateModValue(mod);
                CalculateModDanger(mod);
            }

            if (Settings.MonsterValue.Value)
            {
                foreach(var monster in monsters)
                {
                    Graphics.DrawText(((int)monster.CalculatedValue).ToString(), monster.MonsterAssociation.MonsterPortrait.GetClientRectCache.TopLeft, Color.Green, 20, ExileCore.Shared.Enums.FontAlign.Left);
                }
            }

            if (Settings.ModValue.Value)
            {
                foreach (var mod in mods)
                {
                    Graphics.DrawText(((int)mod.CalculatedValue).ToString(), mod.MonsterAssociation.ModElement.GetClientRectCache.TopLeft, Color.Green, 20, ExileCore.Shared.Enums.FontAlign.Left);
                }
            }

            if (Settings.ModDanger.Value)
            {
                foreach (var mod in mods)
                {
                    if (!mod.IsDevoted)
                        Graphics.DrawText(((int)mod.CalculatedDanger).ToString(), mod.MonsterAssociation.ModElement.GetClientRectCache.TopRight, Color.Red, 20, ExileCore.Shared.Enums.FontAlign.Right);
                }
            }


            if (Settings.GiveSuggestions.Value)
            {
                MonsterModel existingModel = null;
                MonsterModel desiredModel = null;

                List<int> alreadyHandledMonsters = new List<int>();

                mods = mods.OrderByDescending(x => x.CalculatedValue).ToList();
                for (int i = 0; i < mods.Count; i++)
                {
                    var mod = mods.ElementAt(i);

                    // Reorder the mod list for us, ignoring all mods that are already claimed
                    var ourBestMonsters = monsters.Where(x => !alreadyHandledMonsters.Contains(x.Order))
                        .OrderByDescending(monster => CalculateFinalWeight(mod, monster))
                        .ToList();

                    var bestMonster = ourBestMonsters.FirstOrDefault();

                    // First... is this mod already in this slot? If so... skip it
                    if (bestMonster.Order == mod.Order)
                    {
                        alreadyHandledMonsters.Add(bestMonster.Order);
                        continue;
                    }

                    existingModel = monsters.FirstOrDefault(x => x.Order == mod.Order);
                    // Next... to prevent ever having a pointless switch... lets look at the mod value already in this slot
                    if (existingModel != null && mod.CalculatedValue - (CalculateFinalWeight(mod, existingModel)) == (int)(CalculateFinalWeight(mod, bestMonster)))
                    {
                        alreadyHandledMonsters.Add(existingModel.Order);
                        continue;
                    }

                    var existingModelOnBestMonster = mods.FirstOrDefault(x => x.Order == bestMonster.Order);
                    // If these mods are effectively identical... no need to switch.
                    if (existingModel != null && (int)(mod.CalculatedValue - mod.CalculatedDanger) == (int)(existingModelOnBestMonster.CalculatedValue - existingModelOnBestMonster.CalculatedDanger))
                    {
                        alreadyHandledMonsters.Add(existingModel.Order);
                        continue;
                    }

                    // If we've made it this far, we've found something that needs to switch. Render that.
                    desiredModel = bestMonster;
                    break;
                }

                // If we have a desired model, we need to render a switch
                if (desiredModel != null)
                {
                    // TODO: Add configurable color and thickness
                    // TODO: Make an arrow and not just a line?
                    // TODO: Probably position better, maybe 25% through frame?
                    Graphics.DrawLine(existingModel.MonsterAssociation.ModElement.GetClientRectCache.Center.ToVector2Num(), desiredModel.MonsterAssociation.ModElement.GetClientRectCache.Center.ToVector2Num(), 5.0f, Color.Green);
                }
            }
        }
        else
        {
            // Cleanup since we no longer have the window visible
            ListOfNormalTiers.Clear();
        }
    }

    private float CalculateFinalWeight(ModModel mod, MonsterModel monster, bool excludeModTier=false)
    {
        return mod.CalculatedValue + monster.CalculatedValue - mod.CalculatedDanger;
    }

    private void GetModelLists(NecropolisMonsterPanel necropolisTransitionWindow, out List<ModModel> mods, out List<MonsterModel> monsters)
    {
        mods = new List<ModModel>();
        monsters = new List<MonsterModel>();
        var associations = necropolisTransitionWindow.Associations;
        for (int i = 0; i < associations.Count; i++)
        {
            MonsterModel monsterModel = ConvertElementToMonster(associations.ElementAtOrDefault(i));
            if (monsterModel != null)
            {
                monsterModel.Order = i;
                monsters.Add(monsterModel);
            }

            ModModel modModel = ConvertElementToMod(associations.ElementAtOrDefault(i));
            int minTier = 0;
            if (modModel != null)
            {
                modModel.Order = i;

                // Adjust our mod model if its already been adjusted by the monster
                var foundMinTier = Settings.ModMinTiers.TryGetValue(modModel.Name, out minTier);
                if (monsterModel != null && !modModel.IsDevoted && (!foundMinTier || monsterModel.ModTierModifier > 0 || modModel.Tier > minTier))
                    modModel.Tier -= monsterModel.ModTierModifier;

                mods.Add(modModel);
            }

            // Now we need to do some sanity checking on this association
            // Do we know of this one already? Is it in our list of tiers?
            if (minTier == 0)
            {
                if (ListOfNormalTiers.TryGetValue(modModel.Name, out int value))
                {
                    // If we're in the list of normal tiers and we now have a monster decreasing our mod
                    // We were moved from a normal mod to a minus mod. Did it actually decrease?
                    if (monsterModel.ModTierModifier < 0)
                    {
                        if (value < modModel.Tier)
                        {
                            // this is a special case where we are at the mod's min level. We need to save this off.
                            Settings.AddMinTier(modModel.Name, value);

                            // Might as well correct it while we're at it
                            modModel.Tier--;
                        }
                        else
                        {
                            // Clean it up from the list
                            ListOfNormalTiers.Remove(modModel.Name);
                        }
                    }
                }
                else if (monsterModel.ModTierModifier == 0)
                {
                    // we don't know of this one yet, lets throw it in the list if its on a normal monster
                    ListOfNormalTiers.Add(modModel.Name, modModel.Tier);
                }
            }
        }
    }

    private MonsterModel ConvertElementToMonster(NecropolisMonsterPanelMonsterAssociation element)
    {
        if (element == null) return null;

        MonsterModel model = new MonsterModel();
        model.MonsterAssociation = element;

        // Going to have to parse the description maybe?
        model.Name = element.Pack.Name;

        model.PackSizeLow = element.MinMonstersPerPack;
        model.PackSizeHigh = element.MaxMonstersPerPack;
        
        model.Density = MonsterModel.MonsterDensityFromId(element.PackFrequency?.Id ?? null);

        // We have to do some funky stuff to get the mod tier modifier
        Element monsterModifiers = element.MonsterPortrait.GetChildAtIndex(1);
        if (monsterModifiers != null && monsterModifiers.ChildCount > 0)
        {
            // If we have children... this monster has some special mods on it
            foreach (Element child in monsterModifiers.Children)
            {
                string tooltipText = child.Tooltip?.Text ?? null;

                if (tooltipText != null)
                {
                    if (tooltipText.Contains("+1"))
                    {
                        model.ModTierModifier = 1;
                        break;
                    }
                    else if (tooltipText.Contains("-1"))
                    {
                        model.ModTierModifier = -1;
                        break;
                    }
                    // Technically there is another with "50% more" or "50% less" but I don't think we need that
                }
            }
        }

        return model;
    }


    private ModModel ConvertElementToMod(NecropolisMonsterPanelMonsterAssociation element)
    {
        if (element == null) return null;

        ModModel model = new ModModel();
        model.MonsterAssociation = element;

        model.Name = element.ModElement.GetChildAtIndex(0)?.TextNoTags ?? "NoName";
        model.Name = Regex.Replace(model.Name, @"\d+", "#");
        model.Name = Regex.Replace(model.Name, @"(\r\n|\r|\n)", " ");

        // I don't think we can make much of a guess on the tier format
        var tierText = element.ModElement.GetChildFromIndices(new int[] { 3, 0 })?.Text ?? string.Empty;
        int tier = 1;
        if (tierText.StartsWith("Noble-Haunted"))
            tier = 6;
        else if (tierText.StartsWith("Thaumaturgist-Haunted"))
            tier = 5;
        else if (tierText.StartsWith("Gemling-Haunted"))
            tier = 4;
        else if (tierText.StartsWith("Soldier-Haunted"))
            tier = 3;
        else if (tierText.StartsWith("Peasant-Haunted"))
            tier = 2;
        else if (tierText.StartsWith("Servant-Haunted"))
            tier = 1;
        else if (tierText.StartsWith("Devoted"))
        {
            model.IsDevoted = true;
        }

        model.Tier = tier;

        // TODO: Need to mark things as empty mod slots as well.
        // This may only impact campaign... guessing maps will have 6/6 every time?

        return model;
    }

    private float CalculateMonsterValue(MonsterModel model)
    {
        if (model == null) return -1;

        float monsterVal = 0;

        switch (model.Density)
        {
            case MonsterModel.MonsterDensity.Low:
                monsterVal = 10;
                break;
            case MonsterModel.MonsterDensity.Normal:
                monsterVal = 20;
                break;
            case MonsterModel.MonsterDensity.High:
                monsterVal = 30;
                break;
        }

        // If average pack size is greater than 10... bump it up a tier.
        // It may be worth more than smol packs with high density
        int averagePackDensity = (model.PackSizeLow + model.PackSizeHigh) / 2;

        monsterVal += averagePackDensity;

        // Packs with a pack leader can definitely be more valuable.
        // Lets add a tier modifier for that.
        if (!String.IsNullOrEmpty(model.MonsterAssociation.Pack.LeaderDescription))
            monsterVal += 10 * Settings.HasPackLeaderValue.Value;

        // Adjust this monster's value up or down depending on if it increases mod tier
        monsterVal += model.ModTierModifier * 10;

        // Before we return... lets throw it on the model for future logic
        model.CalculatedValue = monsterVal;

        return monsterVal;
    }

    private float CalculateModValue(ModModel model)
    {
        if (model == null || model.Name == null) return 0;

        float modVal = 0;

        // Mod is made more valuable (or less valuable if negative) by tier
        modVal +=  10 * model.Tier;

        // If this is a devotion mod, add our base devition bonus
        if (model.IsDevoted)
            modVal += Settings.DevotedBonusValue.Value;

        // Modify the tier based on the config
        Settings.Weights.TryGetValue(model.Name ?? string.Empty, out float tierModifier);
        modVal += 10 * tierModifier;

        // Before we return... lets throw it on the model for future logic
        model.CalculatedValue = modVal;

        return modVal;
    }

    private float CalculateModDanger(ModModel mod)
    {
        float danger = 0;

        // First, mods are more dangerous the higher the tier
        if (!mod.IsDevoted)
            danger += mod.Tier * 10;

        // Multiply the final danger value by our overall danger weight.
        // This is there to adjust how juiced we want the map. 0 = no care for danger, 1 = REALLY care about danger.
        danger *= Settings.DangerWeight;

        mod.CalculatedDanger = danger;

        return danger;
    }
}
