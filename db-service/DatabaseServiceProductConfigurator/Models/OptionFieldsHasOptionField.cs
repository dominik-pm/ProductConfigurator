using System;
using System.Collections.Generic;

namespace DatabaseServiceProductConfigurator.Models
{
    public partial class OptionFieldsHasOptionField
    {
        public string Base { get; set; } = null!;
        public string OptionField { get; set; } = null!;
        public string DependencyType { get; set; } = null!;

        public virtual OptionField BaseNavigation { get; set; } = null!;
        public virtual EDependencyType DependencyTypeNavigation { get; set; } = null!;
        public virtual OptionField OptionFieldNavigation { get; set; } = null!;
    }
}
