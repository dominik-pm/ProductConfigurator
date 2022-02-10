using Model.Indexes;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class OptionGroupBase : LanguageIndex
    {
        public bool Required { get; set; }
    }
}
