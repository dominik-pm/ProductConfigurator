using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model
{
    public class Account : IIndexable<int>
    {
        public int ConfigId { get; set; }
        public string Name { get; set; }
        public string Email { get; set; }
    }
}
