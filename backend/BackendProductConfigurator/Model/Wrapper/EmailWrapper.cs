using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Wrapper
{
    public class EmailWrapper
    {
        public ConfiguredProduct ConfiguredProduct { get; set; }
        public List<Option> Options { get; set; }
    }
}
