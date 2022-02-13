using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Rules
    {
        public float BasePrice { get; set; }
        public string DefaultModel { get; set; }
        public Dictionary<string, List<string>> Requirements { get; set; } = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> Incompatibilities { get; set; } = new Dictionary<string, List<string>>();
        public Dictionary<string, List<string>> GroupRequirements { get; set; } = new Dictionary<string, List<string>>();
        public Dictionary<string, float> PriceList { get; set; } = new Dictionary<string, float>();

    }
}
