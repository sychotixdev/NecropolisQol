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

    private List<ModModel> CachedMods { get; set; }
    private List<MonsterModel> CachedMonsterModels { get; set; }

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

            mods = mods.OrderByDescending(x => CalculateModValue(x)).ToList();
            monsters = monsters.OrderByDescending(x => CalculateMonsterValue(x)).ToList();

            ModModel existingModel = null;
            ModModel desiredModel = null;

            for (int i = 0; i < mods.Count; i++)
            {
                MonsterModel monster = monsters.ElementAt(i);
                ModModel mod = mods.ElementAt(i);

                // First... is this mod already in this slot? If so... skip it
                if (monster.Order == mod.Order)
                    continue;
                // Next... to prevent ever having a pointless switch... lets look at the mod value already in this slot
                existingModel = mods.FirstOrDefault(x => x.Order == monster.Order);
                if (existingModel != null && mod.CalculatedValue == existingModel.CalculatedValue)
                {
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

            // We want to draw attention to anything particularly dangerous
            foreach (var mod in mods)
            {
                if (mod.CalculatedValue <= Settings.DangerousHighlight.Value)
                {
                    // TODO: Color configuration?
                    // TODO: Is this the right rectangle to use?
                    Graphics.DrawBox(mod.Element.GetClientRectCache, Color.Red);
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

        // Each monster has tiers. Now that we've calculated its base total... lets do some multiplication.
        MonsterConfig config = Settings.MonsterConfigList.FirstOrDefault(x => model.Name.Contains(x.Name));
        int monsterTier = 3;
        if (config == null)
            monsterTier = config.Tier;

        model.CalculatedValue *= monsterTier;

        // Before we return... lets throw it on the model for future logic
        model.CalculatedValue = monsterVal;

        return monsterVal;
    }

    private float CalculateModValue(ModModel model)
    {
        if (model == null || model.Description == null) return 0;

        string lowerCaseModelDescription = model.Description.ToLower();

        float modVal = 0;


        if (Settings.AvoidList.Any(x => lowerCaseModelDescription.Contains(x)))
        {
            modVal = -10;
        }
        // When minimizing danger... still treat priority items with priority
        else if (Settings.MinimizeDanger.Value && Settings.PriorityList.Any(x => lowerCaseModelDescription.Contains(x)))
        {
            modVal = Settings.PriorityWeight.Value;
        }
        // If we are minimizing danger... everything else is dangerous
        else if (Settings.MinimizeDanger.Value || Settings.DangerousList.Any(x => lowerCaseModelDescription.Contains(x)))
        {
            modVal = -1;
        }
        else if (Settings.PriorityList.Any(x => lowerCaseModelDescription.Contains(x)))
        {
            modVal = Settings.PriorityWeight.Value;
        }
        else
        {
            modVal = 1;
        }

        // Mod is made more valuable (or less valuable if negative) by tier
        modVal *= model.Tier;

        // Before we return... lets throw it on the model for future logic
        model.CalculatedValue = modVal;

        return modVal;
    }
}
