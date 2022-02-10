using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Indexes
{
    public class DescribedIndex : NamedIndex, IDescribable
    {
        public string Description { get; set; }
    }
}
