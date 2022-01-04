using System;
using System.Collections.Generic;

namespace DatabaseServiceProductConfigurator.Models
{
    public partial class EOptionType
    {
        public EOptionType()
        {
            OptionFields = new HashSet<OptionField>();
        }

        public string Type { get; set; } = null!;

        public virtual ICollection<OptionField> OptionFields { get; set; }
    }
}
