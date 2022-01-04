using System;
using System.Collections.Generic;

namespace DatabaseServiceProductConfigurator.Models
{
    public partial class EDependencyType
    {
        public EDependencyType()
        {
            OptionFieldsHasOptionFields = new HashSet<OptionFieldsHasOptionField>();
            ProductsHasOptionFields = new HashSet<ProductsHasOptionField>();
            ProductsHasProducts = new HashSet<ProductsHasProduct>();
        }

        public string Type { get; set; } = null!;

        public virtual ICollection<OptionFieldsHasOptionField> OptionFieldsHasOptionFields { get; set; }
        public virtual ICollection<ProductsHasOptionField> ProductsHasOptionFields { get; set; }
        public virtual ICollection<ProductsHasProduct> ProductsHasProducts { get; set; }
    }
}
