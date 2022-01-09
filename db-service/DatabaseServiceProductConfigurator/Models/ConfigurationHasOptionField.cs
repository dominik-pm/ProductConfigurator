using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

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
        [JsonIgnore]
        public int? ParentConfigId { get; set; }
        [JsonIgnore]
        public string? ParentOptionFieldId { get; set; }

        [JsonIgnore]
        public virtual Configuration Config { get; set; } = null!;
        public virtual OptionField OptionField { get; set; } = null!;
        [JsonIgnore]
        public virtual ConfigurationHasOptionField? Parent { get; set; }

        [JsonPropertyName("Children")]
        public virtual ICollection<ConfigurationHasOptionField> InverseParent { get; set; }

        [JsonPropertyName("Selected")]
        public virtual ICollection<Product> ProductNumbers { get; set; }
    }
}
