using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using ExileCore;
using ExileCore.PoEMemory;
using ExileCore.PoEMemory.Components;
using ExileCore.PoEMemory.Elements.Necropolis;
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
        return base.Initialise();
    }

    public override void Render()
    {
        RenderModStuff();
    }

    public void RenderModStuff()
    {
        NecropolisMonsterPanel necropolisMonsterPanel = GameController.IngameState.IngameUi.NecropolisMonsterPanel;


        if (necropolisMonsterPanel is
            {
                IsVisible: true
            })
        {
            // Pull the data for us to process
            var mods = GetModList(necropolisMonsterPanel);
            var monsters = GetMonsterList(necropolisMonsterPanel);

            monsters = monsters.OrderByDescending(x => CalculateMonsterValue(x)).ToList();

            // Loop through each mod so that we pre-calculate mod values.
            // No need to order this list as its reordered for each monster based on danger
            foreach(var mod in mods)
            {
                CalculateModValue(mod);
                var monster = monsters.FirstOrDefault(x => x.Order == mod.Order);
                CalculateModDanger(mod, monster);
            }
            mods.ForEach(x => CalculateModValue(x));

            if (Settings.MonsterValue.Value)
            {
                foreach(var monster in monsters)
                {
                    Graphics.DrawText(monster.CalculatedValue.ToString(), monster.MonsterAssociation.MonsterPortrait.GetClientRectCache.BottomLeft, Color.Green, ExileCore.Shared.Enums.FontAlign.Left);
                }
            }

            if (Settings.ModValue.Value)
            {
                foreach (var mod in mods)
                {
                    Graphics.DrawText(mod.CalculatedValue.ToString(), mod.MonsterAssociation.ModElement.GetClientRectCache.BottomLeft, Color.Green, ExileCore.Shared.Enums.FontAlign.Left);
                }
            }

            if (Settings.ModDanger.Value)
            {
                foreach (var mod in mods)
                {
                    Graphics.DrawText(mod.CalculatedDanger.ToString(), mod.MonsterAssociation.ModElement.GetClientRectCache.BottomRight, Color.Red, ExileCore.Shared.Enums.FontAlign.Left);
                }
            }


            if (Settings.GiveSuggestions.Value)
            {
                ModModel existingModel = null;
                ModModel desiredModel = null;

                List<int> alreadyHandledMods = new List<int>();

                for (int i = 0; i < monsters.Count; i++)
                {
                    MonsterModel monster = monsters.ElementAt(i);

                    // TODO: At this point, we could hyper-optimize and ensure that we choos the end "board" with the highest total net value.
                    // I think this is too complicated for a first draft though and potentially too resource intensive. Lets take the more instant
                    // easy calculation of only optimizing the top value monsters
                    // NOTE: If you make this even remotely more complicated... we probably need to take this off the render thread

                    // Reorder the mod list for us, ignoring all mods that are already claimed
                    var ourBestMods = mods.Where(x => !alreadyHandledMods.Contains(x.Order))
                        .OrderByDescending(mod => mod.CalculatedValue - CalculateModDanger(mod, monster))
                        .ToList();

                    ModModel mod = ourBestMods.FirstOrDefault();

                    // First... is this mod already in this slot? If so... skip it
                    if (monster.Order == mod.Order)
                    {
                        alreadyHandledMods.Add(mod.Order);
                        continue;
                    }

                    // Next... to prevent ever having a pointless switch... lets look at the mod value already in this slot
                    existingModel = ourBestMods.FirstOrDefault(x => x.Order == monster.Order);
                    if (existingModel != null && (mod.CalculatedValue - mod.CalculatedDanger) == (existingModel.CalculatedValue - mod.CalculatedDanger))
                    {
                        alreadyHandledMods.Add(existingModel.Order);
                        continue;
                    }

                    // If we've made it this far, we've found something that needs to switch. Render that.
                    desiredModel = mod;
                    break;
                }

                // If we have a desired model, we need to render a switch
                if (desiredModel != null)
                {
                    // TODO: Add configurable color and thickness
                    // TODO: Make an arrow and not just a line?
                    // TODO: Probably position better, maybe 25% through frame?
                    Graphics.DrawLine(existingModel.MonsterAssociation.ModElement.PositionNum, desiredModel.MonsterAssociation.ModElement.PositionNum, 5.0f, Color.Green);
                }
            }
        }
    }

    private List<ModModel> GetModList(NecropolisMonsterPanel necropolisTransitionWindow)
    {
        var modList = new List<ModModel>();
        var associations = necropolisTransitionWindow.Associations;
        for (int i = 0; i < associations.Count; i++)
        {
            ModModel model = ConvertElementToMod(associations.ElementAtOrDefault(i));
            if (model != null)
            {
                model.Order = i;
                modList.Add(model);
            }
        }

        return modList;
    }

    private List<MonsterModel> GetMonsterList(NecropolisMonsterPanel necropolisTransitionWindow)
    {
        var monsterList = new List<MonsterModel>();
        var associations = necropolisTransitionWindow.Associations;
        for (int i = 0; i < associations.Count; i++)
        {
            MonsterModel model = ConvertElementToMonster(associations.ElementAtOrDefault(i));
            if (model != null)
            {
                model.Order = i;
                monsterList.Add(model);
            }
        }

        return monsterList;
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
        
        model.Density = MonsterModel.MonsterDensityFromId(element.PackFrequency.Id);

        return model;
    }


    private ModModel ConvertElementToMod(NecropolisMonsterPanelMonsterAssociation element)
    {
        if (element == null) return null;

        ModModel model = new ModModel();
        model.MonsterAssociation = element;

        model.Name = element.ModElement.Text;

        // I don't think we can make much of a guess on the tier format
        model.Tier = 1;

        // TODO: Need to mark things as empty mod slots as well.
        // This may only impact campaign... guessing maps will have 6/6 every time?

        return model;
    }

    private int CalculateMonsterValue(MonsterModel model)
    {
        if (model == null) return -1;

        int monsterVal = 0;

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

        // Before we return... lets throw it on the model for future logic
        model.CalculatedValue = monsterVal;

        return monsterVal;
    }

    private float CalculateModValue(ModModel model)
    {
        if (model == null || model.Name == null) return 0;

        string lowerCaseModelDescription = model.Name.ToLower();

        float modVal = 0;

        // Mod is made more valuable (or less valuable if negative) by tier
        modVal +=  10 * model.Tier;

        // If this is a devotion mod, add our base devition bonus
        if (Settings.DevotionMods.Any(x => model.Name == x.Id))
            modVal += Settings.DevotedBonusValue.Value;

        // Now, lets make any adjustments to the danger based on the mod's danger level (and overrides for this monster)
        if (Settings.HotSwap.DevotionModMobWeightings.TryGetValue(model.Name, out Dictionary<string, float> overrides))
        {
            float overrideValue = 0;
            // TODO: Technically there are overrides per monster that we are just throwing away
            // Value is not based on monster. We should probably fix the UI, not this code?
            // Unless we truely want to modify value per monster... which would complicate things
            if (overrides.TryGetValue(NecropolisQolSettings.DEFAULT_MONSTER, out overrideValue))
            {
                modVal += overrideValue * 10;
            }
        }

        // Before we return... lets throw it on the model for future logic
        model.CalculatedValue = modVal;

        return modVal;
    }

    private float CalculateModDanger(ModModel mod, MonsterModel monster)
    {
        float danger = 0;

        // First, mods are more dangerous the higher the tier
        danger += mod.Tier * 10;

        // Now, lets make any adjustments to the danger based on the mod's danger level (and overrides for this monster)
        if (Settings.HotSwap.ModMobWeightings.TryGetValue(mod.Name, out Dictionary<string, float> overrides))
        {
            float overrideValue = 0;
            if (overrides.TryGetValue(monster.Name, out overrideValue) || overrides.TryGetValue(NecropolisQolSettings.DEFAULT_MONSTER, out overrideValue))
            {
                danger += overrideValue * 10;
            }
        }

        // Multiply the final danger value by our overall danger weight.
        // This is there to adjust how juiced we want the map. 0 = no care for danger, 1 = REALLY care about danger.
        danger *= Settings.DangerWeight;

        mod.CalculatedDanger = danger;

        return danger;
    }
}
