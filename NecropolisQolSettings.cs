using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using ImGuiNET;
using NecropolisQol.Models;
using Newtonsoft.Json;
using SharpDX;

namespace AncestorQol;

public class NecropolisQolSettings : ISettings
{
    public NecropolisQolSettings()
    {
        AvoidList = File.ReadLines("avoid.txt") ?? new List<string>();
        DangerousList = File.ReadLines("dangerous.txt") ?? new List<string>();
        PriorityList = File.ReadLines("priority.txt") ?? new List<string>();
        var unparsedMonsterConfig = File.ReadLines("monsterConfig.txt") ?? new List<string>();
        MonsterConfigList = new List<MonsterConfig>();
        foreach (var unparsedMonster in unparsedMonsterConfig)
        {
            var monsterConfig = MonsterConfig.FromConfig(unparsedMonster);
            if (monsterConfig != null)
            {
                MonsterConfigList.Add(monsterConfig);
            }
        }
    }

    public ToggleNode Enable { get; set; } = new ToggleNode(false);

    public RangeNode<float> PriorityWeight { get; set; } = new RangeNode<float>(1.5f, 1.0f, 10.0f);
    public RangeNode<float> DangerousHighlight { get; set; } = new RangeNode<float>(-10, -50.0f, 0.0f);

    public ToggleNode MinimizeDanger { get; set; } = new ToggleNode(false);

    public IEnumerable<string> AvoidList { get; set; }
    public IEnumerable<string> DangerousList { get; set; }
    public IEnumerable<string> PriorityList { get; set; }

    public List<MonsterConfig> MonsterConfigList { get; set; }


}
