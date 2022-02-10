using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Indexes
{
    public class ConfiguratorIndex : IConfigId, IImageable
    {
        public string ConfigId { get; set; }
        public List<string> Images { get; set; }
    }
}
