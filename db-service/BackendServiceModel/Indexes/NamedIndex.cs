using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Indexes
{
    public class NamedIndex : IIndexable, INameable
    {
        public string Id { get; set; }
        public string Name { get; set; }
    }
}
