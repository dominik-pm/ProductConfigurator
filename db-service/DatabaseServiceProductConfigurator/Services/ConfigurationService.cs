using DatabaseServiceProductConfigurator.Context;
using DatabaseServiceProductConfigurator.Models;
using Microsoft.EntityFrameworkCore;
using Model;

namespace DatabaseServiceProductConfigurator.Services {

    internal struct ConfiguredProductStruct {
        public float Price { get; init; }
        public List<string> Options { get; init; }

        public ConfiguredProductStruct( float price, List<string> options ) {
            this.Price = price;
            this.Options = options;
        }
    }

    public class ConfigurationService : IConfigurationService {

        private readonly ConfiguratorContext _context;
        private readonly ILanguageService _languageService;

        public ConfigurationService( ConfiguratorContext context, ILanguageService languageService ) {
            _context = context;
            _languageService = languageService;
        }

        #region GET

        public List<ModelType> GetModelsByProduct( string productNumber, string lang ) {
            List<Configuration> temp = _context.Configurations.Where(conf => conf.ProductNumber == productNumber).ToList();
            List<ModelType> toReturn = new();
            temp.ForEach(
                conf => {
                    var infos = _languageService.GetConfigurationWithLanguage(conf.Id, lang);
                    var opts = GetOptionsByConfigId(conf.Id, lang);
                    toReturn.Add(new ModelType {
                        Name = infos.Name,
                        Description = infos.Description,
                        Options = opts.Options
                    });
                }
            );
            return toReturn;
        }

        public List<ProductSaveExtended> GetConfigurations( string lang ) {
            List<Configuration> temp = _context.Configurations.Where(c => c.AccountId == null).ToList();

            List<ProductSaveExtended> toReturn = new();
            temp.ForEach(
                conf => {
                    var opts = GetOptionsByConfigId(conf.Id, lang);
                    var infos = _languageService.GetConfigurationWithLanguage(conf.Id, lang);
                    var productInfo = _languageService.GetProductWithLanguage(conf.ProductNumber, lang);
                    var ordered = _context.Bookings.Where(b => b.ConfigId == conf.Id && b.AccountId == conf.AccountId).Any();
                    var customer = _context.Accounts.Where(ac => ac.Id == conf.AccountId).FirstOrDefault();
                    toReturn.Add(new ProductSaveExtended {
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
                    });
                }
            );
            return toReturn;
        }

        public ProductSaveExtended? GetConfiguredProductById( string id ) => GetConfigurations(id).FirstOrDefault();

        private ConfiguredProductStruct GetOptionsByConfigId( int configId, string lang ) {

            List<ConfigurationHasOptionField> fields = new();
            foreach ( var item in (
                from op in _context.Configurations
                where op.Id == configId
                select op.ConfigurationHasOptionFields
            ).ToList() ) {
                fields.AddRange(item.ToList());
            }

            List<string> toReturn = new();
            float price = 0;

            foreach ( var item in fields ) {

                foreach ( var el in item.ProductNumbers ) {
                    price += el.Price;

                    InfoStruct infos = _languageService.GetProductWithLanguage(el.ProductNumber, lang);
                    toReturn.Add(
                        el.ProductNumber
                    );
                }
            }

            return new ConfiguredProductStruct(price, toReturn);
        }

        #endregion

        #region POST

        public void SaveConfiguration( ProductSaveExtended toSave, string lang ) {
            Models.Account? user = _context.Accounts.Where(a => a.Email.Equals(toSave.User.UserEmail)).FirstOrDefault();

            Configuration added = _context.Configurations.Add(
                new Configuration {
                    ProductNumber = toSave.ConfigId,
                    ProductNumberNavigation = _context.Products.First(p => p.ProductNumber == toSave.ConfigId),
                    AccountId = user?.Id,
                    Account = user,
                    Date = toSave.Date
                }
            ).Entity;

            _context.ConfigurationsHasLanguages.Add(
                new ConfigurationsHasLanguage {
                    Configuration = added.Id,
                    ConfigurationNavigation = added,
                    Language = lang,
                    LanguageNavigation = _context.ELanguages.First(l => l.Language == lang),
                    Name = toSave.SavedName,
                    Description = ""
                }
            );

            List<string> products = ( from p in _context.Products where toSave.Options.Contains(p.ProductNumber) select p.ProductNumber ).ToList();

            //List<ConfigurationHasOptionField> fields = (
            //    from of in _context.OptionFields
            //    join pof in _context.ProductsHasOptionFields on of.Id equals pof.OptionFields
            //    where products.Contains(pof.ProductNumber)
            //    select new ConfigurationHasOptionField {
            //        ConfigId = added.Id,
            //        Config = added,
            //        OptionFieldId = of.Id,
            //        OptionField = of,
            //        ProductNumbers = ( from pof1 in _context.ProductsHasOptionFields where pof1.OptionFields == of.Id && products.Contains(pof.ProductNumber) select pof1.ProductNumberNavigation ).ToList()
            //    }
            //).ToList();

            List<ConfigurationHasOptionField> fields = (
                from of in _context.OptionFields
                join pof in _context.ProductsHasOptionFields on of.Id equals pof.OptionFields
                where products.Contains(pof.ProductNumber)
                select new ConfigurationHasOptionField {
                    ConfigId = added.Id,
                    Config = added,
                    OptionFieldId = of.Id,
                    OptionField = of,
                    ProductNumbers = ( from pof1 in _context.ProductsHasOptionFields where pof1.OptionFields == of.Id && products.Contains(pof.ProductNumber) select pof1.ProductNumberNavigation ).ToList()
                }
            ).ToList();

            _context.ConfigurationHasOptionFields.AddRange(fields);

            if ( toSave.Status == EStatus.ordered.ToString() ) {
                if ( user == null )
                    throw new Exception("User not in Database!");
                _context.Bookings.Add(
                    new Booking {
                        ConfigId = added.Id,
                        Config = added,
                        AccountId = user.Id,
                        Account = user
                    }
                );
            }

            _context.SaveChanges();

        }

        public void SaveModels( string productNumber, ModelType model, string lang ) {
            Configuration temp = _context.Configurations.Add(
                new Configuration {
                    Id = 0,
                    ProductNumber = productNumber,
                    ProductNumberNavigation = _context.Products.First(p => p.ProductNumber == productNumber),
                    AccountId = null,
                    Account = null,
                    Date = DateTime.Now
                }
            ).Entity;

            _context.ConfigurationsHasLanguages.Add(
                new ConfigurationsHasLanguage {
                    Configuration = temp.Id,
                    ConfigurationNavigation = temp,
                    Language = lang,
                    LanguageNavigation = _context.ELanguages.First(l => l.Language == lang),
                    Name = model.Name,
                    Description = model.Description
                }
            );

            foreach ( var item in model.Options ) {
                OptionField toInsert = GetOptionfieldByProductAndOption(productNumber, item);
                _context.ConfigurationHasOptionFields.Add(
                    new ConfigurationHasOptionField {
                        ConfigId = temp.Id,
                        Config = temp,
                        OptionFieldId = toInsert.Id,
                        OptionField = toInsert,
                        ProductNumbers = _context.Products.Where(p => model.Options.Contains(p.ProductNumber)).ToList()
                    }
                );
            }

            _context.SaveChanges();
        }

        private OptionField GetOptionfieldByProductAndOption( string productNumber, string option ) {
            List<string> products = (
                from of in _context.ProductsHasOptionFields
                where of.ProductNumber == productNumber && of.DependencyType == "PARENT"
                select of.OptionFields
            ).ToList();

            List<OptionField> options = (
                from of in _context.ProductsHasOptionFields
                where of.ProductNumber == option && of.DependencyType == "CHILD"
                select of.OptionFieldsNavigation
            ).ToList();

            foreach ( var item in options ) {
                if ( products.Contains(item.Id) )
                    return item;
            }

            throw new Exception("Option Product has no Dependency to Product or is not in Database!");
        }

        #endregion

        #region DELETE

        public void DeleteConfiguration( int configID ) {

            Configuration? config = _context.Configurations.Where(c => c.Id.Equals(configID)).FirstOrDefault();
            if ( config == null )
                throw new Exception("no Config");

            if ( _context.Bookings.Where(c => c.ConfigId == configID).Any() )
                throw new Exception("there are Bookings");

            // LANGUAGE
            _context.ConfigurationsHasLanguages.RemoveRange(
                from chl in _context.ConfigurationsHasLanguages
                where chl.Configuration == configID
                select chl
            );

            // OPTIONS
            _context.ConfigurationHasOptionFields.RemoveRange(
                from cho in _context.ConfigurationHasOptionFields
                where cho.ConfigId == configID
                select cho
            );

            _context.Configurations.RemoveRange(
                _context.Configurations.Where(c => c.Id == configID)
            );

            _context.SaveChanges();

        }

        #endregion

        #region PUT

        public void UpdateConfiguration( ConfiguredProduct config ) {

        }

        #endregion

    }
}