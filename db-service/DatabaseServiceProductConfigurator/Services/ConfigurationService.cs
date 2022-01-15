using DatabaseServiceProductConfigurator.Models;
using Microsoft.EntityFrameworkCore;
using Model;

namespace DatabaseServiceProductConfigurator.Services {

    public struct ConfigStruct {
        public int id { get; set; }
        public int? customer { get; set; }
        public ProductStruct product { get; set; }
        public List<ConfigOptionStruct> options { get; set; }
    }

    public struct ProductStruct {
        public string productNumber { get; set; }
        public float price { get; set; }
        public string category { get; set; }
        public object infos { get; set; }
    }

    public struct ConfigOptionStruct {
        public int configId { get; set; }
        public object optionField { get; set; }
        public List<ConfigOptionStruct> children { get; set; }
        public List<ProductStruct> selected { get; set; }
    }

    internal struct ConfiguredProductStruct {
        public float price { get; init; }
        public List<Option> options { get; init; }

        public ConfiguredProductStruct( float price, List<Option> options ) {
            this.price = price;
            this.options = options;
        }
    }

    public static class ConfigurationService {

        static product_configuratorContext context = new product_configuratorContext();

        #region Backend

        public static List<ConfiguredProduct> GetConfiguredProducts( string lang ) {
            return (
                from conf in context.Configurations
                let opts = GetOptionsByConfigId(conf.Id, lang)
                select new ConfiguredProduct {
                    ConfigurationName = conf.ProductNumber,
                    Options = opts.options,
                    Price = opts.price
                }
            ).ToList();
        }

        public static ConfiguredProduct? GetConfiguredProductById( string id ) => GetConfiguredProducts(id).FirstOrDefault();

        private static ConfiguredProductStruct GetOptionsByConfigId( int configId, string lang ) {
            product_configuratorContext localContext = new product_configuratorContext();

            List<ConfigurationHasOptionField> fields = new List<ConfigurationHasOptionField>();
            foreach ( var item in (
                from op in localContext.Configurations
                where op.Id == configId
                select op.ConfigurationHasOptionFields
            ).ToList() ) {
                fields.AddRange(item.ToList());
            }

            List<Option> toReturn = new List<Option>();
            float price = 0;

            foreach ( var item in fields ) {

                foreach ( var el in item.ProductNumbers ) {
                    price += el.Price;

                    ProductHasLanguage? infos = LanguageService.GetProductWithLanguage(el.ProductNumber, lang);
                    toReturn.Add(
                        new Option {
                            Id = el.ProductNumber,
                            Name = infos == null ? "" : infos.Name,
                            Description = infos == null ? "" : infos.Description
                        }
                    );
                }
            }

            return new ConfiguredProductStruct(price, toReturn);
        }

        public static bool SaveConfiguredProduct( ConfiguredProduct toSave ) {
            bool worked = true;

            try {
                Configuration added = context.Configurations.Add(
                    new Configuration {
                        ProductNumber = toSave.ConfigurationName,
                        Customer = null
                    }
                ).Entity;

                List<string> products = ( from p in context.Products where toSave.Options.Select(c => c.Id).Contains(p.ProductNumber) select p.ProductNumber ).ToList();

                List<ConfigurationHasOptionField> fields = (
                    from of in context.OptionFields
                    join pof in context.ProductsHasOptionFields on of.Id equals pof.OptionFields
                    where products.Contains(pof.ProductNumber)
                    select new ConfigurationHasOptionField {
                        ConfigId = added.Id,
                        OptionFieldId = of.Id,
                        ProductNumbers = ( from pof1 in context.ProductsHasOptionFields where pof1.OptionFields == of.Id && products.Contains(pof.ProductNumber) select pof1.ProductNumberNavigation ).ToList()
                    }
                ).ToList();

                context.ConfigurationHasOptionFields.AddRange(fields);
                context.SaveChanges();
            }
            catch ( Exception ex ) {
                Console.WriteLine(ex.Message);
                context.Dispose();
                worked = false;
            }

            return worked;

        }

        #endregion

        #region DB
        private static IQueryable<ConfigStruct> getConfigs( string lang ) {
            return (
                from c in context.Configurations
                let infos = LanguageService.GetProductWithLanguage(c.ProductNumber, lang)
                select new ConfigStruct {
                    id = c.Id,
                    customer = c.Customer,
                    product = new ProductStruct {
                        productNumber = c.ProductNumberNavigation.ProductNumber,
                        price = c.ProductNumberNavigation.Price,
                        category = c.ProductNumberNavigation.Category,
                        infos = new {
                            Name = infos.Name,
                            Description = infos.Description
                        }
                    },
                    options = getOptions(c.Id, lang)
                }
            );
        }

        private static List<ConfigOptionStruct> getOptions( int configId, string lang ) {
            product_configuratorContext localContext = new product_configuratorContext();

            return (
                from c in localContext.ConfigurationHasOptionFields
                where c.ConfigId == configId && ( c.ParentConfigId == null || c.ParentOptionFieldId == null )
                let infos = LanguageService.GetOptionsfieldWithLanguage(c.OptionFieldId, lang)
                select new ConfigOptionStruct {
                    configId = c.ConfigId,
                    optionField = new {
                        id = c.OptionFieldId,
                        type = c.OptionField.Type,
                        required = c.OptionField.Required,
                        infos = new {
                            name = infos.Name,
                            description = infos.Description
                        }
                    },
                    children = getChildren(c.ConfigId, c.OptionFieldId, lang),
                    selected = c.ProductNumbers.Select(i => new ProductStruct {
                        productNumber = i.ProductNumber,
                        price = i.Price,
                        category = i.Category,
                        infos = new {
                            Name = LanguageService.GetProductWithLanguage(i.ProductNumber, lang),
                            Description = LanguageService.GetProductWithLanguage(i.ProductNumber, lang)
                        }
                    }).ToList()
                }
            ).ToList();
        }

        private static List<ConfigOptionStruct> getChildren( int configId, string OptionFieldId, string lang ) {
            product_configuratorContext localContext = new product_configuratorContext();

            return (
                from c in localContext.ConfigurationHasOptionFields
                where c.ParentConfigId == configId && c.ParentOptionFieldId == OptionFieldId
                let infos = LanguageService.GetOptionsfieldWithLanguage(c.OptionFieldId, lang)
                select new ConfigOptionStruct {
                    configId = c.ConfigId,
                    optionField = new {
                        id = c.OptionFieldId,
                        type = c.OptionField.Type,
                        required = c.OptionField.Required,
                        infos = new {
                            name = infos.Name,
                            description = infos.Description
                        }
                    },
                    children = getChildren(c.ConfigId, c.OptionFieldId, lang),
                    selected = c.ProductNumbers.Select(i => new ProductStruct {
                        productNumber = i.ProductNumber,
                        price = i.Price,
                        category = i.Category,
                        infos = new {
                            Name = LanguageService.GetProductWithLanguage(i.ProductNumber, lang),
                            Description = LanguageService.GetProductWithLanguage(i.ProductNumber, lang)
                        }
                    }).ToList()
                }
            ).ToList();
        }

        public static ConfigStruct? GetById( int id, string lang ) {
            return getConfigs(lang).Where(c => c.id.Equals(id)).FirstOrDefault();
        }

        public static List<ConfigStruct> GetConfigurationsByCustomer( int? customerID, string lang ) =>
            getConfigs(lang).Where(c => c.customer == customerID).ToList();

        public static List<ConfigStruct> GetByProductNumber( string productNumber, string lang ) =>
            GetConfigurationsByCustomer(null, lang).Where(c => c.product.productNumber.Equals(productNumber)).ToList();

        #endregion

    }
}