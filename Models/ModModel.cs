using ExileCore.PoEMemory;
using ExileCore.PoEMemory.Elements.Necropolis;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace NecropolisQol.Models
{
    internal class ModModel
    {
        public bool IsEmpty { get; set; }

        public bool IsDevoted { get; set; }
        public String Name { get; set; }
        public int Tier { get; set; }
        public NecropolisMonsterPanelMonsterAssociation MonsterAssociation { get; set; }
        public int Order { get; set; }
        public float CalculatedValue { get; set; }
        public float CalculatedDanger { get; set; }
    }
}
