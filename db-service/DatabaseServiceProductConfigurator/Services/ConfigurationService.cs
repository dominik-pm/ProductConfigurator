using DatabaseServiceProductConfigurator.Models;
using Microsoft.EntityFrameworkCore;
using Model;

namespace DatabaseServiceProductConfigurator.Services {

    internal struct ConfiguredProductStruct {
        public float Price { get; init; }
        public List<Option> Options { get; init; }
        public List<string> OptionId { get; init; }

        public ConfiguredProductStruct( float price, List<Option> options ) {
            this.Price = price;
            this.Options = options;
            OptionId = options.Select( x => x.Id ).ToList();
            Console.WriteLine(OptionId[0]);
        }
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
                         Options = opts.OptionId
                     }
            ).ToList();
        }

        public static List<ProductSaveExtended> GetConfigurations( string lang ) {
            return (
                from conf in context.Configurations
                where conf.AccountId != null
                let opts = GetOptionsByConfigId(conf.Id, lang)
                let infos = LanguageService.GetConfigurationWithLanguage(conf.Id, lang)
                let productInfo = LanguageService.GetProductWithLanguage(conf.ProductNumber, lang)
                let ordered = context.Bookings.Where(b => b.ConfigId == conf.Id && b.AccountId == conf.AccountId).Any()
                let customer = context.Accounts.Where(ac => ac.Id == conf.AccountId).FirstOrDefault()
                select new ProductSaveExtended {
                    SavedName = infos.Name,
                    Options = opts.Options,
                    ConfigId = conf.ProductNumber,
                    Name = productInfo.Name,
                    Description = productInfo.Description,
                    Status = ordered ? EStatus.ordered.ToString() : EStatus.saved.ToString(),
                    Date = conf.Date,
                    User = new Model.Account {
                        UserName = customer != null ? customer.Username : "",
                        UserEmail = customer != null ? customer.Email : ""
                    }
                }
            ).ToList();
        }

        public static ProductSaveExtended? GetConfiguredProductById( string id ) => GetConfigurations(id).FirstOrDefault();

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

        public static bool SaveConfiguration( ProductSaveExtended toSave, string lang ) {
            bool worked = true;

            try {
                Models.Account? user = context.Accounts.Where(a => a.Email.Equals(toSave.User.UserEmail)).FirstOrDefault(); 

                Configuration added = context.Configurations.Add(
                    new Configuration {
                        ProductNumber = toSave.ConfigId,
                        AccountId = user != null ? user.Id : null,
                        Date = toSave.Date
                    }
                ).Entity;

                context.ConfigurationsHasLanguages.Add(
                    new ConfigurationsHasLanguage {
                        Configuration = added.Id,
                        Language = lang,
                        Name = toSave.SavedName,
                        Description = ""
                    }
                );

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
                
        public static void SaveModels( Product_configuratorContext local_Context, string productNumber, ModelType model, string lang ) {
            Configuration temp = local_Context.Configurations.Add(
                new Configuration {
                    Id = 0,
                    ProductNumber = productNumber,
                    AccountId = null
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

            foreach ( var item in model.Options ) {
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

            foreach ( var item in options ) {
                if ( products.Contains(item) )
                    return item;
            }

            throw new Exception("Option Product has no Dependency to Product or is not in Database!");
        }

        #endregion

        #region DELETE

        public static bool DeleteConfiguration( int configID ) {
            bool worked = true;

            Configuration? config = context.Configurations.Where(c => c.Id.Equals(configID)).FirstOrDefault();
            if ( config == null )
                return false;

            try {

                if ( context.Bookings.Where(c => c.ConfigId == configID).Any() )
                    return false;

                // LANGUAGE
                context.ConfigurationsHasLanguages.RemoveRange(
                    from chl in context.ConfigurationsHasLanguages
                    where chl.Configuration == configID
                    select chl
                );

                // OPTIONS
                context.ConfigurationHasOptionFields.RemoveRange(
                    from cho in context.ConfigurationHasOptionFields
                    where cho.ConfigId == configID
                    select cho
                );

                context.Configurations.RemoveRange(
                    context.Configurations.Where(c => c.Id == configID)
                );

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

        #region PUT

        public static bool UpdateConfiguration( ConfiguredProduct config ) {
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