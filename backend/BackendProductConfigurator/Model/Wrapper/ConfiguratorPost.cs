using Model.Languages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ConfiguratorPost : ConfiguratorBase
    {
        public RulesLanguages Rules { get; set; }
        public List<OptionGroupExtended> OptionGroups { get; set; }
        public Dictionary<string, LanguageVariant> Languages { get; set; }
    }
}
