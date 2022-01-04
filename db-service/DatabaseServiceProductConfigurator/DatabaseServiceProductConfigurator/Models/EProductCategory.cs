using System;
using System.Collections.Generic;

namespace DatabaseServiceProductConfigurator.Models
{
    public partial class EProductCategory
    {
        public EProductCategory()
        {
            Products = new HashSet<Product>();
        }

        public string Category { get; set; } = null!;

        public virtual ICollection<Product> Products { get; set; }
    }
}
