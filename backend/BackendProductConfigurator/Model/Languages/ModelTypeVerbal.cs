using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Languages
{
    public class ModelTypeVerbal : INameable, IDescribable
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
