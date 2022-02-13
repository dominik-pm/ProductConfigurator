using Model.Indexes;
using Model.Interfaces;
using Model.Languages;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class LanguageVariant : INameable, IDescribable
    {
        public string Name { get; set; }
        public string Description { get; set; }
        public List<Option> Options { get; set; }
        public List<NamedIndex> OptionSections { get; set; }
        public List<DescribedIndex> OptionGroups { get; set; }
        public List<DescribedIndex> Models { get; set; }
    }
}
