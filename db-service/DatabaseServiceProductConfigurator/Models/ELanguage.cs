using System;
using System.Collections.Generic;

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

        public virtual ICollection<ConfigurationsHasLanguage> ConfigurationsHasLanguages { get; set; }
        public virtual ICollection<OptionFieldHasLanguage> OptionFieldHasLanguages { get; set; }
        public virtual ICollection<ProductHasLanguage> ProductHasLanguages { get; set; }
    }
}
