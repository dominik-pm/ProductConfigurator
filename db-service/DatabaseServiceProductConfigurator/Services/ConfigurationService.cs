using DatabaseServiceProductConfigurator.Models;
using Microsoft.EntityFrameworkCore;
using Model;

namespace DatabaseServiceProductConfigurator.Services {

    internal struct ConfiguredProductStruct {
        public float Price { get; init; }
        public List<Option> Options { get; init; }

        public ConfiguredProductStruct( float price, List<Option> options ) {
            this.Price = price;
            this.Options = options;
        }

        public List<string> GetOptionsId() => Options.Select(x => x.Id).ToList();
    }

    public static class ConfigurationService {

        static readonly Product_configuratorContext context = new();

        #region GET

        public static List<ModelType> GetModelsByProduct( string productNumber, string lang ) {
            return ( from conf in context.Configurations
                     where conf.ProductNumber == productNumber
                     let infos = LanguageService.GetConfigurationWithLanguage(conf.Id, lang)
                     let opts = GetOptionsByConfigId(conf.Id, lang)
                     select new ModelType {
                         Name = infos.Name,
                         Description = infos.Description,
                         Options = opts.GetOptionsId()
                     }
            ).ToList();
        }

        public static List<ConfiguredProduct> GetConfiguredProducts( string lang ) {
            return (
                from conf in context.Configurations
                let opts = GetOptionsByConfigId(conf.Id, lang)
                let infos = LanguageService.GetConfigurationWithLanguage(conf.Id, lang)
                select new ConfiguredProduct {
                    ConfigurationName = infos.Name,
                    Options = opts.Options,
                    Price = opts.Price
                }
            ).ToList();
        }

        public static ConfiguredProduct? GetConfiguredProductById( string id ) => GetConfiguredProducts(id).FirstOrDefault();

        private static ConfiguredProductStruct GetOptionsByConfigId( int configId, string lang ) {
            Product_configuratorContext localContext = new();

            List<ConfigurationHasOptionField> fields = new();
            foreach ( var item in (
                from op in localContext.Configurations
                where op.Id == configId
                select op.ConfigurationHasOptionFields
            ).ToList() ) {
                fields.AddRange(item.ToList());
            }

            List<Option> toReturn = new();
            float price = 0;

            foreach ( var item in fields ) {

                foreach ( var el in item.ProductNumbers ) {
                    price += el.Price;

                    InfoStruct infos = LanguageService.GetProductWithLanguage(el.ProductNumber, lang);
                    toReturn.Add(
                        new Option {
                            Id = el.ProductNumber,
                            Name = infos.Name,
                            Description = infos.Description
                        }
                    );
                }
            }

            return new ConfiguredProductStruct(price, toReturn);
        }

        #endregion

        #region POST

        public static bool SaveConfiguredProduct( ConfiguredProduct toSave, string productNumber, string lang, int? customerId = null ) {
            bool worked = true;

            try {
                Configuration added = context.Configurations.Add(
                    new Configuration {
                        ProductNumber = productNumber,
                        Customer = customerId
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

                context.ConfigurationsHasLanguages.Add(
                    new ConfigurationsHasLanguage {
                        Configuration = added.Id,
                        Language = lang,
                        Name = toSave.ConfigurationName,
                        Description = ""
                    }
                );

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

        public static void SaveModels( Product_configuratorContext local_Context, string productNumber, ModelType model, string lang ) {
            Configuration temp = local_Context.Configurations.Add(
                new Configuration {
                    Id = 0,
                    ProductNumber = productNumber,
                    Customer = null
                }
            ).Entity;

            local_Context.ConfigurationsHasLanguages.Add(
                new ConfigurationsHasLanguage {
                    Configuration = temp.Id,
                    Language = lang,
                    Name = model.Name,
                    Description = model.Description
                }
            );

            foreach(var item in model.Options ) { 
            local_Context.ConfigurationHasOptionFields.Add(
                new ConfigurationHasOptionField {
                    ConfigId = temp.Id,
                    OptionFieldId = GetOptionfieldByProductAndOption(productNumber, item)
                }
            );
            }
        }

        private static string GetOptionfieldByProductAndOption( string productNumber, string option ) {
            List<string> products = (
                from of in context.ProductsHasOptionFields
                where of.ProductNumber == productNumber && of.DependencyType == "PARENT"
                select of.OptionFields
            ).ToList();

            List<string> options = (
                from of in context.ProductsHasOptionFields
                where of.ProductNumber == option && of.DependencyType == "CHILD"
                select of.OptionFields
            ).ToList();

            foreach(var item in options ) {
                if(products.Contains(item))
                    return item;
            }

            throw new Exception("Option Product has no Dependency to Product or is not in Database!");
        }

        #endregion

        #region DELETE

        public static bool DeleteSavedProduct(int configID) {
            bool worked = true;

            try {



                context.SaveChanges();
            }
            catch ( Exception ex ) {
                Console.WriteLine(ex);
                context.Dispose();
                worked = false;
            }
            return worked;

        }

        #endregion

    }
}