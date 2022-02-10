using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Languages
{
    public class RulesLanguages : Rules
    {
        public List<ModelTypeIndex> Models { get; set; } = new List<ModelTypeIndex>();
    }
}
