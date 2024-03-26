using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NecropolisQol.Models
{
    public class ModConfiguration
    {
        public float Danger { get; set; }

        public Dictionary<string, float> MonsterOverride { get; set; } = new Dictionary<string, float>();

        public float GetDanger(string name=null)
        {
            if (name == null || !MonsterOverride.TryGetValue(name, out var danger))
                return Danger;

            return danger;
        }


    }
}
