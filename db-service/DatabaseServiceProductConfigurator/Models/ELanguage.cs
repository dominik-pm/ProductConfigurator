using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DatabaseServiceProductConfigurator.Models
{
    public partial class ELanguage
    {
        public ELanguage()
        {
            ConfigurationsHasLanguages = new HashSet<ConfigurationsHasLanguage>();
            OptionFieldHasLanguages = new HashSet<OptionFieldHasLanguage>();
            ProductHasLanguages = new HashSet<ProductHasLanguage>();
        }

        public string Language { get; set; } = null!;

        [JsonIgnore]
        public virtual ICollection<ConfigurationsHasLanguage> ConfigurationsHasLanguages { get; set; }
        [JsonIgnore]
        public virtual ICollection<OptionFieldHasLanguage> OptionFieldHasLanguages { get; set; }
        [JsonIgnore]
        public virtual ICollection<ProductHasLanguage> ProductHasLanguages { get; set; }
    }
}
