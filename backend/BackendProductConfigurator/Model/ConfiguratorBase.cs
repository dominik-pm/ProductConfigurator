using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ConfiguratorBase : ConfiguratorSlim
    {
        public List<OptionSection> OptionSections { get; set; }
        public List<Option> Options { get; set; }
    }
}
