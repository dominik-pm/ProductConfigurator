using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace DatabaseServiceProductConfigurator.Models
{
    public partial class OptionFieldHasLanguage
    {
        public string OptionFieldId { get; set; } = null!;
        public string Language { get; set; } = null!;
        public string Name { get; set; } = null!;
        public string Description { get; set; } = null!;

        [JsonIgnore]
        public virtual ELanguage LanguageNavigation { get; set; } = null!;
        [JsonIgnore]
        public virtual OptionField OptionField { get; set; } = null!;
    }
}
