using ExileCore.PoEMemory;
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
        public String Description { get; set; }
        public int Tier { get; set; }
        public Element Element { get; set; }
        public int Order { get; set; }
        public float CalculatedValue { get; set; }
    }
}
