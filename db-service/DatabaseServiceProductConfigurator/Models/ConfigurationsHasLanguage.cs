using System;
using System.Collections.Generic;

namespace DatabaseServiceProductConfigurator.Models
{
    public partial class ConfigurationsHasLanguage
    {
        public int Configuration { get; set; }
        public string Language { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;

        public virtual Configuration ConfigurationNavigation { get; set; } = null!;
        public virtual ELanguage LanguageNavigation { get; set; } = null!;
    }
}
