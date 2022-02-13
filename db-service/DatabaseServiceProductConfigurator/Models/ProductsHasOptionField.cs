using System;
using System.Collections.Generic;

namespace DatabaseServiceProductConfigurator.Models
{
    public partial class ProductsHasOptionField
    {
        public string ProductNumber { get; set; } = null!;
        public string OptionFields { get; set; } = null!;
        public string DependencyType { get; set; } = null!;

        public virtual EDependencyType DependencyTypeNavigation { get; set; } = null!;
        public virtual OptionField OptionFieldsNavigation { get; set; } = null!;
        public virtual Product ProductNumberNavigation { get; set; } = null!;
    }
}
