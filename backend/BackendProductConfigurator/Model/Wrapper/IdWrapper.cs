using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Wrapper
{
    public class IdWrapper : IIndexable
    {
        public string Id { get; set; }
        public string ProductNumber { get; set; }
    }
}
