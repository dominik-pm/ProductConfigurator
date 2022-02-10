using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Languages
{
    public class OptionSectionVerbal : IIndexable, INameable
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
