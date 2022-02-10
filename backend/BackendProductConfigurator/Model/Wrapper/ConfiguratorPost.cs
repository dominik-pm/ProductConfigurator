using Model.Interfaces;
using Model.Languages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ConfiguratorPost : ConfiguratorIndex
    {
        public List<IIndexable> Options { get; set; }
        public List<LanguageIndex> OptionSections { get; set; }
        public List<OptionGroupIndex> OptionGroups { get; set; }
        public RulesLanguages Rules { get; set; }
        public Dictionary<string, LanguageVariant> Languages { get; set; }
    }
}
