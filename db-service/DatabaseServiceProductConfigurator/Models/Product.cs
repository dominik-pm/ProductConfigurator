using System;
using System.Collections.Generic;

namespace DatabaseServiceProductConfigurator.Models
{
    public partial class Product
    {
        public Product()
        {
            Configurations = new HashSet<Configuration>();
            Pictures = new HashSet<Picture>();
            ProductHasLanguages = new HashSet<ProductHasLanguage>();
            ProductsHasOptionFields = new HashSet<ProductsHasOptionField>();
            ProductsHasProductBaseProductNavigations = new HashSet<ProductsHasProduct>();
            ProductsHasProductOptionProductNavigations = new HashSet<ProductsHasProduct>();
            ConfigurationHasOptionFields = new HashSet<ConfigurationHasOptionField>();
        }

        public string ProductNumber { get; set; } = null!;
        public float Price { get; set; }
        public bool Buyable { get; set; }
        public int? BaseModel { get; set; }
        public string? ItemNumber { get; set; }

        public virtual Configuration? BaseModelNavigation { get; set; }
        public virtual ICollection<Configuration> Configurations { get; set; }
        public virtual ICollection<Picture> Pictures { get; set; }
        public virtual ICollection<ProductHasLanguage> ProductHasLanguages { get; set; }
        public virtual ICollection<ProductsHasOptionField> ProductsHasOptionFields { get; set; }
        public virtual ICollection<ProductsHasProduct> ProductsHasProductBaseProductNavigations { get; set; }
        public virtual ICollection<ProductsHasProduct> ProductsHasProductOptionProductNavigations { get; set; }

        public virtual ICollection<ConfigurationHasOptionField> ConfigurationHasOptionFields { get; set; }
    }
}
