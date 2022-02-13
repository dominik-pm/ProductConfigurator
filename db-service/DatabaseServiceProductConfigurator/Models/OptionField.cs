using System;
using System.Collections.Generic;

namespace DatabaseServiceProductConfigurator.Models
{
    public partial class OptionField
    {
        public OptionField()
        {
            ConfigurationHasOptionFields = new HashSet<ConfigurationHasOptionField>();
            OptionFieldHasLanguages = new HashSet<OptionFieldHasLanguage>();
            OptionFieldsHasOptionFieldBaseNavigations = new HashSet<OptionFieldsHasOptionField>();
            OptionFieldsHasOptionFieldOptionFieldNavigations = new HashSet<OptionFieldsHasOptionField>();
            ProductsHasOptionFields = new HashSet<ProductsHasOptionField>();
        }

        public string Id { get; set; } = null!;
        public string Type { get; set; } = null!;
        public bool Required { get; set; }

        public virtual EOptionType TypeNavigation { get; set; } = null!;
        public virtual ICollection<ConfigurationHasOptionField> ConfigurationHasOptionFields { get; set; }
        public virtual ICollection<OptionFieldHasLanguage> OptionFieldHasLanguages { get; set; }
        public virtual ICollection<OptionFieldsHasOptionField> OptionFieldsHasOptionFieldBaseNavigations { get; set; }
        public virtual ICollection<OptionFieldsHasOptionField> OptionFieldsHasOptionFieldOptionFieldNavigations { get; set; }
        public virtual ICollection<ProductsHasOptionField> ProductsHasOptionFields { get; set; }
    }
}
