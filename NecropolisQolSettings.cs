using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using ImGuiNET;
using Newtonsoft.Json;
using SharpDX;
using ExileCore.Shared.Attributes;

namespace NecropolisQoL;

public class NecropolisQolSettings : ISettings
{
    [JsonIgnore]
    public static readonly string AllModsFileName = "AllModifiers.json";

    [JsonIgnore]
    public static readonly string ModMinTiersFileName = "ModifierMinimumTiers.json";

    [JsonIgnore]
    private const string OverwritePopup = "Overwrite Confirmation";

    [JsonIgnore]
    public List<string> AllMods { get; set; } = new List<string>();

    [JsonIgnore]
    public Dictionary<string, int> ModMinTiers { get; set; } = new Dictionary<string, int>();

    public ToggleNode Enable { get; set; } = new ToggleNode(false);

    public RangeNode<int> DevotedBonusValue { get; set; } = new RangeNode<int>(70, 0, 100);
    public RangeNode<float> HasPackLeaderValue { get; set; } = new RangeNode<float>(1.5f, -10f, 10f);

    public RangeNode<float> DangerWeight { get; set; } = new RangeNode<float>(0.5f, 0, 1);

    public ToggleNode MonsterValue { get; set; } = new ToggleNode(true);
    public ToggleNode ModValue { get; set; } = new ToggleNode(true);
    public ToggleNode ModDanger { get; set; } = new ToggleNode(true);

    public ToggleNode GiveSuggestions { get; set; } = new ToggleNode(true);
    public ColorNode Positive { get; set; } = new ColorNode(Color.Green);
    public ColorNode Negative { get; set; } = new ColorNode(Color.Red);

    [Menu("Suggestion Arrow Settings", 100, CollapsedByDefault = true)]
    [JsonIgnore]
    public EmptyNode SuggestionArrowSettings { get; set; }
    [Menu(null, parentIndex = 100)]
    public ColorNode SuggestionArrowColor { get; set; } = new ColorNode(Color.Green);
    [Menu(null, parentIndex = 100)]
    public RangeNode<float> SuggestionArrowThickness { get; set; } = new RangeNode<float>(5f, 1, 10);
    [Menu(null, parentIndex = 100)]
    public RangeNode<float> SuggestionArrowSize { get; set; } = new RangeNode<float>(10f, 10, 20);

    public string ModMobWeightingLastSaved { get; set; } = "";
    public string ModMobWeightingLastSelected { get; set; } = "";

    public Swappable HotSwap { get; set; } = new();

    [JsonIgnore]
    public CustomNode HotSwappableConfiguration { get; }

    [JsonIgnore]
    public CustomNode WeightsList { get; set; }

    public class Swappable
    {
        public Dictionary<string, (bool highlight, float weight)> Weights { get; set; } = [];
    }

    public NecropolisQolSettings()
    {
        List<string> files;

        HotSwappableConfiguration = new CustomNode
        {
            DrawDelegate = () =>
            {
                var _fileSaveName = ModMobWeightingLastSaved;
                var _selectedFileName = ModMobWeightingLastSelected;

                if (!ImGui.CollapsingHeader(
                        $"Load / Save Hot Swappable Configurations##{NecropolisQol.Main.Name}Load / Save",
                        ImGuiTreeNodeFlags.DefaultOpen
                    ))
                {
                    return;
                }

                ImGui.Indent();
                ImGui.InputTextWithHint("##SaveAs", "File Path...", ref _fileSaveName, 100);
                ImGui.SameLine();

                if (ImGui.Button("Save To File"))
                {
                    files = GetFiles();

                    _fileSaveName = Path.GetInvalidFileNameChars().Aggregate(
                        _fileSaveName,
                        (current, c) => current.Replace(c, '_')
                    );

                    if (_fileSaveName != string.Empty)
                    {
                        if (files.Contains(_fileSaveName))
                        {
                            ImGui.OpenPopup(OverwritePopup);
                        }
                        else
                        {
                            SaveHotSwapProfile(HotSwap, _fileSaveName);
                        }
                    }
                }

                ImGui.Separator();

                if (ImGui.BeginCombo("Load File##LoadHotSwapProfile", _selectedFileName))
                {
                    files = GetFiles();

                    foreach (var fileName in files)
                    {
                        var isSelected = _selectedFileName == fileName;

                        if (ImGui.Selectable(fileName, isSelected))
                        {
                            _selectedFileName = fileName;
                            _fileSaveName = fileName;
                            LoadHotSwapProfile(fileName);
                        }

                        if (isSelected)
                        {
                            ImGui.SetItemDefaultFocus();
                        }
                    }

                    ImGui.EndCombo();
                }

                ImGui.Separator();

                if (ImGui.Button("Open Template Folder"))
                {
                    var configDir = NecropolisQol.Main.ConfigDirectory;
                    var directoryToOpen = Directory.Exists(configDir);

                    if (!directoryToOpen)
                    {
                        // Log error when the config directory doesn't exist
                    }

                    if (configDir != null)
                    {
                        Process.Start("explorer.exe", configDir);
                    }
                }

                if (ShowButtonPopup(OverwritePopup, ["Are you sure?", "STOP"], out var saveSelectedIndex) &&
                    saveSelectedIndex == 0)
                {
                    SaveHotSwapProfile(HotSwap, _fileSaveName);
                }

                ModMobWeightingLastSaved = _fileSaveName;
                ModMobWeightingLastSelected = _selectedFileName;
                ImGui.Unindent();
            }
        };

        string modFilter = "", mobFilter = "";

        WeightsList = new CustomNode
        {
            DrawDelegate = () =>
            {

                if (!ImGui.CollapsingHeader(
                        $"Modifier Weighting",
                        ImGuiTreeNodeFlags.DefaultOpen
                    ))
                {
                    return;
                }
                ImGui.Indent();

                if (ImGui.Button("Reset All"))
                {
                    HotSwap.Weights = [];
                }
                ImGui.InputTextWithHint("Modifier Filter##ModFilter", "Filter Modifiers here", ref modFilter, 100);

                var filteredMods = string.IsNullOrEmpty(modFilter) ? AllMods : AllMods
                                        .Where(t => t.Contains(modFilter, StringComparison.InvariantCultureIgnoreCase))
                                        .ToList();

                filteredMods.Sort();

                if (!ImGui.BeginTable("WeightingTable", 3, ImGuiTableFlags.SizingFixedFit | ImGuiTableFlags.Borders))
                {
                    return;
                }

                ImGui.TableSetupColumn("Weighting", ImGuiTableColumnFlags.WidthFixed, 350);
                ImGui.TableSetupColumn("");
                ImGui.TableSetupColumn($"Modifier ({filteredMods.Count})", ImGuiTableColumnFlags.WidthStretch);
                ImGui.TableHeadersRow();

                // Weighting column
                ImGui.TableNextColumn();

                for (var i = 0; i < filteredMods.Count; i++)
                {
                    if (filteredMods[i] == "NoName")
                        continue;

                    var usableSpace = ImGui.GetContentRegionAvail();
                    ImGui.SetNextItemWidth(usableSpace.X);

                    if (!HotSwap.Weights.TryGetValue(filteredMods[i], out var tempWeight))
                    {
                        HotSwap.Weights.Add(filteredMods[i], (highlight: false, weight: 0.0f));
                    }

                    DisplayWeightSlider(filteredMods[i], ref tempWeight.weight);
                    HotSwap.Weights[filteredMods[i]] = tempWeight;
                    ImGui.TableNextColumn();
                    var highlighted = "Highlighted";
                    var normal = "Normal";
                    var label = tempWeight.highlight ? highlighted : normal.PadRight(highlighted.Length);

                    if (ImGui.Button($"{label}##Highlight_{filteredMods[i]}"))
                        tempWeight.highlight = !tempWeight.highlight;

                    HotSwap.Weights[filteredMods[i]] = tempWeight;
                    ImGui.TableNextColumn();
                    ImGui.TextUnformatted(filteredMods[i]);
                    if (i < filteredMods.Count - 1)
                    {
                        ImGui.TableNextColumn();
                    }
                }

                ImGui.EndTable();
                ImGui.Unindent();
            }
        };

    }

    private static void DisplayWeightSlider(string modId, ref float weight)
    {
        ImGui.SliderFloat($"##Slider_{modId}", ref weight, -10.0f, 10.0f);
    }

    #region Save / Load Section


    public void AddMinTier(string modId, int minTier)
    {
        ModMinTiers[modId] = minTier;
        var fullPath = Path.Combine(NecropolisQol.Main.ConfigDirectory, ModMinTiersFileName);
        var jsonString = JsonConvert.SerializeObject(ModMinTiers, Formatting.Indented);
        File.WriteAllText(fullPath, jsonString);
    }
    public void AddMods(List<string> newMods)
    {
        try
        {
            // purge NoName from newMods, im unaware how fragile the inner workings are.
            var newModsList = newMods.ToList();

            for (var index = 0; index < newModsList.Count; index++)
                if (newModsList[index] == "NoName")
                    newMods.RemoveAt(index);

            AllMods.AddRange(newMods);

            AllMods.Sort();
            var fullPath = Path.Combine(NecropolisQol.Main.ConfigDirectory, AllModsFileName);
            var jsonString = JsonConvert.SerializeObject(AllMods, Formatting.Indented);
            File.WriteAllText(fullPath, jsonString);
        }
        catch
        {
            // ignored
        }
    }

    public void LoadMinTiers()
    {
        try
        {
            var fullPath = Path.Combine(NecropolisQol.Main.ConfigDirectory, ModMinTiersFileName);
            var fileContent = File.ReadAllText(fullPath);
            ModMinTiers = JsonConvert.DeserializeObject<Dictionary<string, int>>(fileContent);
        }
        catch
        {
            // ignored
        }
    }

    public static bool ShowButtonPopup(string popupId, List<string> items, out int selectedIndex)
    {
        selectedIndex = -1;
        var isItemClicked = false;
        var showPopup = true;

        if (!ImGui.BeginPopupModal(
                popupId,
                ref showPopup,
                ImGuiWindowFlags.NoResize | ImGuiWindowFlags.AlwaysAutoResize
            ))
        {
            return false;
        }

        for (var i = 0; i < items.Count; i++)
        {
            if (ImGui.Button(items[i]))
            {
                selectedIndex = i;
                isItemClicked = true;
                ImGui.CloseCurrentPopup();
            }

            ImGui.SameLine();
        }

        ImGui.EndPopup();
        return isItemClicked;
    }

    public void LoadMods()
    {
        try
        {
            var fullPath = Path.Combine(NecropolisQol.Main.ConfigDirectory, AllModsFileName);
            var fileContent = File.ReadAllText(fullPath);
            AllMods = JsonConvert.DeserializeObject<List<string>>(fileContent);
        }
        catch
        {
            // ignored
        }
    }

    public void SaveHotSwapProfile(Swappable input, string fileName)
    {
        try
        {
            var folder = Path.Combine(NecropolisQol.Main.ConfigDirectory, "Profiles");
            var fullPath = Path.Combine(folder, $"{fileName}.json");
            var jsonString = JsonConvert.SerializeObject(input, Formatting.Indented);

            if (!Directory.Exists(folder))
            {
                Directory.CreateDirectory(folder);
            }
            File.WriteAllText(fullPath, jsonString);
        }
        catch
        {
            // ignored
        }
    }

    public void LoadHotSwapProfile(string fileName)
    {
        try
        {
            var fullPath = Path.Combine(NecropolisQol.Main.ConfigDirectory, "Profiles", $"{fileName}.json");
            var fileContent = File.ReadAllText(fullPath);
            HotSwap = JsonConvert.DeserializeObject<Swappable>(fileContent);
        }
        catch
        {
            // ignored
        }
    }

    public List<string> GetFiles()
    {
        var fileList = new List<string>();

        try
        {
            var dir = new DirectoryInfo(Path.Combine(NecropolisQol.Main.ConfigDirectory, "Profiles"));
            fileList = dir.GetFiles().Select(file => Path.GetFileNameWithoutExtension(file.Name)).ToList();
        }
        catch
        {
            // ignored
        }

        return fileList;
    }


    #endregion
}
