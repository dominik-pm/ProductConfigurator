using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ConfiguratorPost : ConfiguratorBase
    {
        public Rules Rules { get; set; }
        public List<OptionGroupExtended> OptionGroups { get; set; }
    }
}
