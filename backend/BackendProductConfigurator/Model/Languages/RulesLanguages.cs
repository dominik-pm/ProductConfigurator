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
        public RulesExtended ConvertToExtended()
        {
            RulesExtended rulesExtended = new RulesExtended()
            {
                BasePrice = base.BasePrice,
                DefaultModel = base.DefaultModel,
                GroupRequirements = base.GroupRequirements,
                Incompatibilities = base.Incompatibilities,
                PriceList = base.PriceList,
                Requirements = base.Requirements
            };
            return rulesExtended;
        }
    }
    
}
