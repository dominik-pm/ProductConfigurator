using Model.Indexes;
using Model.Interfaces;
using Model.Languages;
using Model.Wrapper;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class ConfiguratorPost : ConfiguratorIndex
    {
        public List<IdWrapper> Options { get; set; }
        public List<LanguageIndexGroup> OptionSections { get; set; }
        public List<OptionGroupIndex> OptionGroups { get; set; }
        public RulesLanguages Rules { get; set; }
        public Dictionary<string, LanguageVariant> Languages { get; set; }
    }
}
