using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NecropolisQol.Models
{
    public class MonsterConfig
    {
        public string Name { get; set; } = "Invalid Name";
        public int Tier { get; set; } = 1;

        public static MonsterConfig FromConfig(string config)
        {
            if (config == null) return null;

            var splitConfig = config.Split(',');
            MonsterConfig monsterConfig = new MonsterConfig();
            monsterConfig.Name = splitConfig.ElementAtOrDefault(0);
            // For easier searching, store this as a lowercase string
            if (monsterConfig.Name != null)
                monsterConfig.Name = monsterConfig.Name.ToLower();
            var tier = splitConfig.ElementAtOrDefault(1);
            if (tier != null)
                monsterConfig.Tier = int.Parse(tier);

            return monsterConfig;
        }
    }
}
