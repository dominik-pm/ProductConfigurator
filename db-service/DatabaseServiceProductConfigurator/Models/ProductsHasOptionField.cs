using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DatabaseServiceProductConfigurator.Models
{
    public partial class ProductsHasOptionField
    {
        public string ProductNumber { get; set; } = null!;
        public string OptionFields { get; set; } = null!;
        public string DependencyType { get; set; } = null!;

        [JsonIgnore]
        public virtual EDependencyType DependencyTypeNavigation { get; set; } = null!;
        [JsonPropertyName("optionField")]
        public virtual OptionField OptionFieldsNavigation { get; set; } = null!;
        [JsonPropertyName("product")]
        public virtual Product ProductNumberNavigation { get; set; } = null!;
    }
}
