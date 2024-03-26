using ExileCore.PoEMemory;
using ExileCore.PoEMemory.Elements.Necropolis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NecropolisQol.Models
{
    internal class MonsterModel
    {
        public enum MonsterDensity
        {
            None,
            Low,
            Normal,
            High
        }

        public string Name { get; set; } = "NoName";
        public int PackSizeLow { get; set; } = 1;
        public int PackSizeHigh { get; set; } = 1;
        public MonsterDensity Density { get; set; } = MonsterDensity.None;

        public NecropolisMonsterPanelMonsterAssociation MonsterAssociation { get; set; }
        public int Order { get; set; }
        public float CalculatedValue { get; set; }

        public static MonsterDensity MonsterDensityFromId(string str)
        {
            if (str != null)
            {
                str = str.ToLower();

                if ("low".Equals(str, StringComparison.OrdinalIgnoreCase))
                    return MonsterDensity.Low;
                if ("normal".Equals(str, StringComparison.OrdinalIgnoreCase))
                    return MonsterDensity.Normal;
                if ("high".Equals(str, StringComparison.OrdinalIgnoreCase))
                    return MonsterDensity.High;
            }

            return MonsterDensity.None;
        }

    }
}
