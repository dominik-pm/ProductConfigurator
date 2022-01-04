using System;
using System.Collections.Generic;

namespace DatabaseServiceProductConfigurator.Models
{
    public partial class OptionFieldsHasOptionField
    {
        public int Base { get; set; }
        public int OptionField { get; set; }
        public string DependencyType { get; set; } = null!;

        public virtual OptionField BaseNavigation { get; set; } = null!;
        public virtual EDependencyType DependencyTypeNavigation { get; set; } = null!;
        public virtual OptionField OptionFieldNavigation { get; set; } = null!;
    }
}
