using Model.Indexes;
using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace Model
{
    public class ConfiguratorSlim : ConfiguratorIndex, INameable, IDescribable
    {
        public string Name { get; set; }
        public string Description { get; set; }
    }
}
