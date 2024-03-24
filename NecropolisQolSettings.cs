using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ExileCore.Shared.Interfaces;
using ExileCore.Shared.Nodes;
using ImGuiNET;
using Newtonsoft.Json;
using SharpDX;

namespace AncestorQol;

public class NecropolisQolSettings : ISettings
{
    public NecropolisQolSettings()
    {
        AvoidList = File.ReadLines("avoid.txt");
        DangerousList = File.ReadLines("dangerous.txt");
        PriorityList = File.ReadLines("priority.txt");
    }

    public ToggleNode Enable { get; set; } = new ToggleNode(false);

    public RangeNode<float> PriorityWeight { get; set; } = new RangeNode<float>(1.5f, 1.0f, 10.0f);
    public RangeNode<float> DangerousHighlight { get; set; } = new RangeNode<float>(-10, -50.0f, 0.0f);

    public ToggleNode MinimizeDanger { get; set; } = new ToggleNode(false);

    public IEnumerable<string> AvoidList { get; set; }
    public IEnumerable<string> DangerousList { get; set; }
    public IEnumerable<string> PriorityList { get; set; }

}
