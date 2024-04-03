using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ExileCore.PoEMemory.Components;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using ImGuiNET;
using NecropolisQol.Models;
using Newtonsoft.Json;

namespace NecropolisQoL;

public class NecropolisQolSettings : ISettings
{
    [JsonIgnore]
    public static readonly string AllModsFileName = "allmods.txt";
    [JsonIgnore]
    public static readonly string ModMinTiersFileName = "modMinTiers.txt";

    [JsonIgnore]
    public List<string> AllMods { get; set; } = new List<string>();

    [JsonIgnore]
    public Dictionary<string,int> ModMinTiers { get; set; } = new Dictionary<string, int>();

    public ToggleNode Enable { get; set; } = new ToggleNode(false);

    public RangeNode<int> DevotedBonusValue { get; set; } = new RangeNode<int>(50, 0, 100);
    public RangeNode<float> HasPackLeaderValue { get; set; } = new RangeNode<float>(1.5f, -10f, 10f);

    public RangeNode<float> DangerWeight { get; set; } = new RangeNode<float>(0.5f, 0, 1);

    public ToggleNode MonsterValue { get; set; } = new ToggleNode(true);
    public ToggleNode ModValue { get; set; } = new ToggleNode(true);
    public ToggleNode ModDanger { get; set; } = new ToggleNode(true);

    public ToggleNode GiveSuggestions { get; set; } = new ToggleNode(true);

    public Dictionary<string, float> Weights { get; set; } = new Dictionary<string, float>();

    [JsonIgnore]
    public CustomNode WeightsList { get; set; } 

    public NecropolisQolSettings()
    {
        string modFilter = "", mobFilter = "";

        WeightsList = new CustomNode
        {
            DrawDelegate = () =>
            {
                ImGui.Indent();

                if (!ImGui.TreeNode("Modifier Weighting"))
                {
                    return;
                }

                ImGui.SameLine();
                ImGui.InputTextWithHint("Modifier Filter##ModFilter", "Filter Modifiers here", ref modFilter, 100);

                var filteredMods = string.IsNullOrEmpty(modFilter) ? AllMods : AllMods
                                        .Where(t => t.Contains(modFilter, StringComparison.InvariantCultureIgnoreCase))
                                        .ToList();


                for (var i = 0; i < filteredMods.Count; i++)
                {
                    if (!Weights.TryGetValue(filteredMods[i], out float tempWeight))
                    {
                        Weights.Add(filteredMods[i], 0.0f);
                    }
                    DisplayWeightSlider(filteredMods[i], ref tempWeight);

                    Weights[filteredMods[i]] = tempWeight;

                    // Add a separator for all but the last item
                    if (i < filteredMods.Count - 1)
                    {
                        ImGui.Separator();
                    }
                }

                ImGui.TreePop();
                ImGui.Unindent();
            }
        };
    }

    private static void DisplayWeightSlider(string modId, ref float weight)
    {
        ImGui.SliderFloat($"##{modId}", ref weight, -10.0f, 10.0f);

        ImGui.SameLine();
        ImGui.Text(modId);
    }

    #region Save / Load Section


    public void AddMinTier(string modId, int minTier)
    {
        ModMinTiers[modId] = minTier;
        var fullPath = Path.Combine(NecropolisQol.Main.ConfigDirectory, ModMinTiersFileName);
        File.AppendAllLines(fullPath, new List<string> { minTier.ToString() + "," + modId });
    }

    public void LoadMinTiers()
    {
        try
        {
            var fullPath = Path.Combine(NecropolisQol.Main.ConfigDirectory, ModMinTiersFileName);
            var fileContents = new List<string>(File.ReadAllLines(fullPath));
            foreach(var line in fileContents)
            {
                var modName = line.Substring(line.IndexOf(',') + 1);
                var minTier = line.Substring(0, line.IndexOf(",") + 1);

                ModMinTiers[modName] = int.Parse(minTier);
            }
        }
        catch
        {
            // ignored
        }
    }

    public void AddMods(List<string> newMods)
    {
        try
        {
            AllMods.AddRange(newMods);
            var fullPath = Path.Combine(NecropolisQol.Main.ConfigDirectory, AllModsFileName);
            File.AppendAllLines(fullPath, newMods);
        }
        catch
        {
            // ignored
        }
    }

    public void LoadMods()
    {
        try
        {
            var fullPath = Path.Combine(NecropolisQol.Main.ConfigDirectory, AllModsFileName);
            AllMods = new List<string>(File.ReadAllLines(fullPath));
        }
        catch
        {
            // ignored
        }
    }

    #endregion
}
