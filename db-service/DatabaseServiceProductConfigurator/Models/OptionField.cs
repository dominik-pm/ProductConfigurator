using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DatabaseServiceProductConfigurator.Models {
    public partial class OptionField {
        public OptionField() {
            ConfigurationHasOptionFields = new HashSet<ConfigurationHasOptionField>();
            OptionFieldHasLanguages = new HashSet<OptionFieldHasLanguage>();
            OptionFieldsHasOptionFieldBaseNavigations = new HashSet<OptionFieldsHasOptionField>();
            OptionFieldsHasOptionFieldOptionFieldNavigations = new HashSet<OptionFieldsHasOptionField>();
            ProductsHasOptionFields = new HashSet<ProductsHasOptionField>();
        }

        public string Id { get; set; } = null!;
        public string Type { get; set; } = null!;
        public bool Required { get; set; }

        [JsonIgnore]
        public virtual EOptionType TypeNavigation { get; set; } = null!;
        [JsonIgnore]
        public virtual ICollection<ConfigurationHasOptionField> ConfigurationHasOptionFields { get; set; }
        public virtual ICollection<OptionFieldHasLanguage> OptionFieldHasLanguages { get; set; }
        [JsonIgnore]
        public virtual ICollection<OptionFieldsHasOptionField> OptionFieldsHasOptionFieldBaseNavigations { get; set; }
        [JsonIgnore]
        public virtual ICollection<OptionFieldsHasOptionField> OptionFieldsHasOptionFieldOptionFieldNavigations { get; set; }
        [JsonIgnore]
        public virtual ICollection<ProductsHasOptionField> ProductsHasOptionFields { get; set; }
    }
}
