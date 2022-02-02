using System;
using System.Collections.Generic;

namespace DatabaseServiceProductConfigurator.Models
{
    public partial class ConfigurationHasOptionField
    {
        public ConfigurationHasOptionField()
        {
            InverseParent = new HashSet<ConfigurationHasOptionField>();
            ProductNumbers = new HashSet<Product>();
        }

        public int ConfigId { get; set; }
        public string OptionFieldId { get; set; } = null!;
        public int? ParentConfigId { get; set; }
        public string? ParentOptionFieldId { get; set; }

        public virtual Configuration Config { get; set; } = null!;
        public virtual OptionField OptionField { get; set; } = null!;
        public virtual ConfigurationHasOptionField? Parent { get; set; }
        public virtual ICollection<ConfigurationHasOptionField> InverseParent { get; set; }

        public virtual ICollection<Product> ProductNumbers { get; set; }
    }
}
