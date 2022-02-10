using Model.Indexes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Languages
{
    public class RulesLanguages : Rules
    {
        public List<LanguageIndex> Models { get; set; } = new List<LanguageIndex>();
    }
}
