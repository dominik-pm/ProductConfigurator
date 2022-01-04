using System;
using System.Collections.Generic;

namespace DatabaseServiceProductConfigurator.Models
{
    public partial class ProductsHasProduct
    {
        public string BaseProduct { get; set; } = null!;
        public string OptionProduct { get; set; } = null!;
        public string DependencyType { get; set; } = null!;

        public virtual Product BaseProductNavigation { get; set; } = null!;
        public virtual EDependencyType DependencyTypeNavigation { get; set; } = null!;
        public virtual Product OptionProductNavigation { get; set; } = null!;
    }
}
