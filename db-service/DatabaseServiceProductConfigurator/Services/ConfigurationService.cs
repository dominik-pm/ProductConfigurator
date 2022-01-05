using DatabaseServiceProductConfigurator.Models;
using Microsoft.EntityFrameworkCore;

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

    public static class ConfigurationService {

        static product_configuratorContext context = new product_configuratorContext();

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

        private static List<ConfigOptionStruct> getChildren( int configId, int OptionFieldId, string lang ) {
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

        public static object? GetById( int id, string lang ) {
            return getConfigs(lang).Where(c => c.id.Equals(id)).FirstOrDefault();
        }

        public static List<ConfigStruct> GetConfigurationsByCustomer( int? customerID, string lang ) =>
            getConfigs(lang).Where(c => c.customer == customerID).ToList();

        public static List<ConfigStruct> GetByProductNumber( string productNumber, string lang ) =>
            GetConfigurationsByCustomer(null, lang).Where(c => c.product.productNumber.Equals(productNumber)).ToList();

    }
}