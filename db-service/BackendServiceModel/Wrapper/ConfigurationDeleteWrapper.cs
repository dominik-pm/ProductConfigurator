using Model.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Model.Wrapper
{
    public class ConfigurationDeleteWrapper : IConfigId
    {
        public string ConfigId { get; set; }
    }
}
