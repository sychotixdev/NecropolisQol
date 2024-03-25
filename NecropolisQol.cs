using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using ExileCore;
using ExileCore.PoEMemory;
using ExileCore.PoEMemory.Components;
using ExileCore.Shared.Helpers;
using NecropolisQol.Models;
using SharpDX;
using Vector2 = System.Numerics.Vector2;

namespace AncestorQol;

public class AncestorQol : BaseSettingsPlugin<NecropolisQolSettings>
{
    private static readonly Regex CleanDescriptionRegex = new Regex("<[^>]*>{(?<value>[^}]*)}", RegexOptions.Compiled);
    private static readonly Regex LineEndingRegex = new Regex("(\r?\n){3,}", RegexOptions.Compiled);
    private static readonly Regex SentenceSplitRegex = new Regex(@"\.( +)", RegexOptions.Compiled);

    /// These may be entirely replaced once the element is reversed
    private static readonly int[] MonsterChildrenIndicies = [0];

    // This corresponds to the hover window when viewing monster details
    private static readonly int[] MonsterDetailsWindowIndicies = [0];

    // Indicies for the monster's name
    private static readonly int[] MonsterDetailsWindowNameIndicies = [0];
    // Indicies for the description containing density and pack size
    private static readonly int[] MonsterDetailsWindowDescriptionIndicies = [0];

    private static readonly int[] ModChildrenIndicies = [0];

    private static readonly int[] ModDescriptionIndicies = [0];
    private static readonly int[] ModTierIndicies = [0];

    public override void Render()
    {
        RenderModStuff();
    }

    public void RenderModStuff()
    {
        Element necropolisTransitionWindow = GetNecropolisTransitionWindow();


        if (necropolisTransitionWindow is
            {
                IsVisible: true
            })
        {
            // Pull the data for us to process
            var mods = GetModList(necropolisTransitionWindow);
            var monsters = GetMonsterList(necropolisTransitionWindow);

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
                // TODO: Add rendering of monster value on UI element
            }

            if (Settings.ModValue.Value)
            {
                // TODO: Add rendering of mod value current monster on UI element
            }

            if (Settings.ModDanger.Value)
            {
                // TODO: Add rendering of mod danger for current monster on UI element
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
                    Graphics.DrawLine(existingModel.Element.PositionNum, desiredModel.Element.PositionNum, 5.0f, Color.Green);
                }
            }
        }
    }

    private List<ModModel> GetModList(Element necropolisTransitionWindow)
    {
        var modList = new List<ModModel>();
        var modElementList = GetModElementList(necropolisTransitionWindow);
        for (int i = 0; i < modElementList.Count; i++)
        {
            ModModel model = ConvertElementToMod(modElementList.ElementAtOrDefault(i));
            if (model != null)
            {
                model.Order = i;
                modList.Add(model);
            }
        }

        return modList;
    }

    private List<MonsterModel> GetMonsterList(Element necropolisTransitionWindow)
    {
        var monsterList = new List<MonsterModel>();
        var monsterElementList = GetMonsterElementList(necropolisTransitionWindow);
        for (int i = 0; i < monsterElementList.Count; i++)
        {
            MonsterModel model = ConvertElementToMonster(monsterElementList.ElementAtOrDefault(i));
            if (model != null)
            {
                model.Order = i;
                monsterList.Add(model);
            }
        }

        return monsterList;
    }

    private Element GetNecropolisTransitionWindow()
    {
        return null;
    }

    private IList<Element> GetMonsterElementList(Element necropolisTransitionWindow)
    {
        List<Element> result = new List<Element>();
        if (necropolisTransitionWindow == null) return new List<Element>();

        return necropolisTransitionWindow.GetChildFromIndices(MonsterChildrenIndicies).Children;
    }

    private IList<Element> GetModElementList(Element necropolisTransitionWindow)
    {
        List<Element> result = new List<Element>();
        if (necropolisTransitionWindow == null) return new List<Element>();

        return necropolisTransitionWindow.GetChildFromIndices(ModChildrenIndicies).Children;
    }

    private MonsterModel ConvertElementToMonster(Element element)
    {
        if (element == null) return null;

        MonsterModel model = new MonsterModel();

        // Going to have to parse the description maybe?
        model.Name = element.GetChildFromIndices(MonsterDetailsWindowNameIndicies)?.Text ?? "";
        String description = element.GetChildFromIndices(MonsterDetailsWindowDescriptionIndicies)?.Text ?? "";

        var rangeMatch = Regex.Match(description, @"(\d+)-(\d+)");
        if (rangeMatch.Success)
        {
            model.PackSizeLow = Int32.Parse(rangeMatch.Groups[1].Value);
            model.PackSizeLow = Int32.Parse(rangeMatch.Groups[2].Value);
        }

        var densityMatch = Regex.Match(description, @"(normal|low|high)", RegexOptions.IgnoreCase);
        if (densityMatch.Success)
        {
            model.Density = MonsterModel.MonsterDensityFromString(densityMatch.Value);
        }

        return model;
    }


    private ModModel ConvertElementToMod(Element element)
    {
        if (element == null) return null;

        ModModel model = new ModModel();

        model.Description = element.GetChildFromIndices(ModDescriptionIndicies).Text;

        // I don't think we can make much of a guess on the tier format
        Element tierElement = element.GetChildFromIndices(ModTierIndicies);
        model.Tier = 1;

        // TODO: Need to mark things as empty mod slots as well.

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
        if (model == null || model.Description == null) return 0;

        string lowerCaseModelDescription = model.Description.ToLower();

        float modVal = 0;

        // Mod is made more valuable (or less valuable if negative) by tier
        modVal +=  10 * model.Tier;

        // If this is a devotion mod, add our base devition bonus
        if (Settings.DevotionMods.Any(x => model.Description == x.Key))
            modVal += Settings.DevotedBonusValue.Value;

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
        if (Settings.ModDangers.TryGetValue(mod.Description, out ModConfiguration modConfiguration))
        {
            danger += 10 * modConfiguration.GetDanger(monster?.Name);
        }

        // Multiply the final danger value by our overall danger weight.
        // This is there to adjust how juiced we want the map. 0 = no care for danger, 1 = REALLY care about danger.
        danger *= Settings.DangerWeight;

        mod.CalculatedDanger = danger;

        return danger;
    }
}
