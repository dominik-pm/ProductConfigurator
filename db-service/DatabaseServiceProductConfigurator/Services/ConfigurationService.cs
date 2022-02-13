using DatabaseServiceProductConfigurator.Context;
using DatabaseServiceProductConfigurator.Models;
using Microsoft.EntityFrameworkCore;
using Model;
using Model.Enumerators;
using Model.Wrapper;

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

        public List<ModelType> GetVisibleModelsByProduct( string productNumber, string lang, List<ConfigurationsHasLanguage> langListConfiguration, List<Configuration> configs ) {
            List<Configuration> temp = configs.Where(conf => conf.ProductNumber == productNumber && conf.Visible == true && conf.AccountId == null).ToList();

            List<ModelType> toReturn = new();
            temp.ForEach(
                conf => {
                    var infos = _languageService.GetConfigurationWithLanguage(conf.Id, lang, langListConfiguration);
                    var opts = GetOptionsByConfigId(conf.Id, lang);
                    toReturn.Add(new ModelType {
                        Id = infos.Name,
                        Name = infos.Name,
                        Description = infos.Description,
                        OptionIds = opts.Options
                    });
                }
            );
            return toReturn;
        }

        public List<ProductSaveExtended> GetConfigurations( string lang ) {
            List<ProductHasLanguage> langListProduct = _context.ProductHasLanguages.ToList();
            List<ConfigurationsHasLanguage> langListConfiguration = _context.ConfigurationsHasLanguages.ToList();

            List<Configuration> temp = _context.Configurations.Where(c => c.AccountId == null).ToList();

            List<ProductSaveExtended> toReturn = new();
            temp.ForEach(
                conf => {
                    var opts = GetOptionsByConfigId(conf.Id, lang);
                    var infos = _languageService.GetConfigurationWithLanguage(conf.Id, lang, langListConfiguration);
                    var productInfo = _languageService.GetProductWithLanguage(conf.ProductNumber, lang, langListProduct);
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

        public ProductSaveExtended? GetConfiguredProductById( string lang, SavedConfigDeleteWrapper wrapper ) {
            List<ProductSaveExtended> temp = GetConfigurations(lang);
            return temp.Where(t => t.ConfigId == wrapper.ConfigId && t.SavedName == wrapper.SavedName && t.User.UserEmail == wrapper.UserEmail).FirstOrDefault();
        }

        private ConfiguredProductStruct GetOptionsByConfigId( int configId, string lang ) {
            List<ProductHasLanguage> langListProduct = _context.ProductHasLanguages.ToList();

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

                    InfoStruct infos = _languageService.GetProductWithLanguage(el.ProductNumber, lang, langListProduct);
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

            Configuration? config = GetConfigurationByProductSaveExtended(toSave, lang);
            Models.Account? user = _context.Accounts.Where(a => a.Email.Equals(toSave.User.UserEmail)).FirstOrDefault();

            if ( user == null )
                throw new NullReferenceException();

            if ( config != null && toSave.Status == EStatus.ordered.ToString() ) {
                _ = _context.Bookings.Add(
                    new Booking {
                        AccountId = user.Id,
                        Account = user,
                        Config = config,
                        ConfigId = config.Id
                    }
                );
            }

            Configuration added = _context.Configurations.Add(  // Adding the Configuration to the Database
                new Configuration {
                    ProductNumber = toSave.ConfigId,
                    ProductNumberNavigation = _context.Products.First(p => p.ProductNumber == toSave.ConfigId),
                    AccountId = user?.Id,
                    Account = user,
                    Date = toSave.Date
                }
            ).Entity;

            SaveConfigurationParameter(toSave, lang, added);

            _context.SaveChanges();

        }

        private void SaveConfigurationParameter( ProductSaveExtended toSave, string lang, Configuration added ) {

            _context.ConfigurationsHasLanguages.Add(    // Adding the Language for the Configuration
                new ConfigurationsHasLanguage {
                    Configuration = added.Id,
                    ConfigurationNavigation = added,
                    Language = lang,
                    LanguageNavigation = _context.ELanguages.First(l => l.Language == lang),
                    Name = toSave.SavedName,
                    Description = ""
                }
            );

            // Getting the selected Option from the Database
            List<string> products = ( from p in _context.Products where toSave.Options.Contains(p.ProductNumber) select p.ProductNumber ).ToList();

            // Saving the selected Options in the Database
            List<OptionField> fields = new();
            foreach ( var item in products ) {
                OptionField temp = GetOptionfieldByProductAndOption(added.ProductNumber, item);
                if ( !fields.Contains(temp) )
                    fields.Add(temp);
            }

            var productHasOptionFields = _context.ProductsHasOptionFields.Where(pof => pof.DependencyType == "CHILD");
            List<ConfigurationHasOptionField> toAdd = new();
            foreach ( var item in fields ) {
                List<Product> temp = (
                    from pof in productHasOptionFields
                    where toSave.Options.Contains(pof.ProductNumber)
                    && pof.OptionFields == item.Id
                    select pof.ProductNumberNavigation
                ).ToList();
                toAdd.Add(
                    new ConfigurationHasOptionField {
                        ConfigId = added.Id,
                        Config = added,
                        OptionFieldId = item.Id,
                        OptionField = item,
                        ProductNumbers = temp
                    }
                );
            }

            _context.ConfigurationHasOptionFields.AddRange(toAdd);

            // Check if the Configuration is booked or saved
            if ( toSave.Status == EStatus.ordered.ToString() ) {
                if ( added.Account == null )
                    throw new Exception("User not in Database!");
                _context.Bookings.Add(
                    new Booking {
                        ConfigId = added.Id,
                        Config = added,
                        AccountId = added.Account.Id,
                        Account = added.Account
                    }
                );
            }
        }

        public void SaveModels( Product product, ModelType model, string lang ) {
            Configuration temp = _context.Configurations.Add(
                new Configuration {
                    Id = 0,
                    ProductNumber = product.ProductNumber,
                    ProductNumberNavigation = product,
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
                    Name = model.Id,
                    Description = model.Description
                }
            );

            foreach ( var item in model.OptionIds ) {
                OptionField toInsert = GetOptionfieldByProductAndOption(product.ProductNumber, item);
                _context.ConfigurationHasOptionFields.Add(
                    new ConfigurationHasOptionField {
                        ConfigId = temp.Id,
                        Config = temp,
                        OptionFieldId = toInsert.Id,
                        OptionField = toInsert,
                        ProductNumbers = _context.Products.Where(p => model.OptionIds.Contains(p.ProductNumber)).ToList()
                    }
                );
            }

        }

        private OptionField GetOptionfieldByProductAndOption( string productNumber, string option ) {

            // Get direct Children
            List<string> fields = (
                from of in _context.ProductsHasOptionFields
                where of.ProductNumber == productNumber && of.DependencyType == "PARENT"
                select of.OptionFields
            ).ToList();

            // Get indirect Children
            List<string> temp = (
                from of in _context.OptionFieldsHasOptionFields
                where fields.Contains(of.Base) && of.DependencyType == "CHILD"
                select of.OptionField
            ).ToList();

            fields.AddRange(temp);

            List<OptionField> options = (
                from of in _context.ProductsHasOptionFields
                where of.ProductNumber == option && of.DependencyType == "CHILD"
                select of.OptionFieldsNavigation
            ).ToList();

            foreach ( var item in options ) {
                if ( fields.Contains(item.Id) )
                    return item;
            }

            throw new Exception("Option Product has no Dependency to Product or is not in Database!");
        }

        #endregion

        #region DELETE

        public void DeleteConfiguration( SavedConfigDeleteWrapper wrapper ) {

            Configuration? config = GetConfigurationByWrapper(wrapper);

            if ( config == null )
                throw new Exception("no Config");

            DeletePrivate(config);

        }

        public void DeleteConfiguration( int id ) {
            Configuration? config = _context.Configurations.Where(c => c.Id == id).FirstOrDefault();

            if ( config == null )
                throw new Exception("no Config");

            DeletePrivate(config);
        }

        private void DeletePrivate(Configuration config) {

            if ( _context.Bookings.Where(c => c.ConfigId == config.Id).Any() ) {
                config.Visible = false;
                _context.Update(config);
                _context.SaveChanges();
                return;
            }

            // LANGUAGE
            _context.ConfigurationsHasLanguages.RemoveRange(
                from chl in _context.ConfigurationsHasLanguages
                where chl.Configuration == config.Id
                select chl
            );

            // OPTIONS
            _context.ConfigurationHasOptionFields.RemoveRange(
                from cho in _context.ConfigurationHasOptionFields
                where cho.ConfigId == config.Id
                select cho
            );

            _context.Configurations.RemoveRange(
                _context.Configurations.Where(c => c.Id == config.Id)
            );

            _context.SaveChanges();
        }

        #endregion

        #region PUT

        public void UpdateConfiguration( ProductSaveExtended config, string lang, string oldSavedName ) {
            Configuration? toUpdate = GetConfigurationByProductSaveExtended(config, oldSavedName);

            if ( toUpdate == null )
                throw new NullReferenceException();

            toUpdate.Date = config.Date;

            List<ConfigurationHasOptionField> fields = _context.ConfigurationHasOptionFields.Where(c => c.ConfigId == toUpdate.Id).ToList();
            foreach ( var item in fields ) {
                item.ProductNumbers = new List<Product>();
            }
            _context.UpdateRange(fields); // Remove Dependencies
            _context.RemoveRange(fields); // Then Delete

            _context.RemoveRange(_context.ConfigurationsHasLanguages.Where(c => c.Configuration == toUpdate.Id && c.Language == lang));

            SaveConfigurationParameter(config, lang, toUpdate);

            _context.SaveChanges();
        }

        private Configuration? GetConfigurationByProductSaveExtended( ProductSaveExtended productSaveExtended, string oldSavedName ) {
            // get the User
            Models.Account? user = _context.Accounts.Where(a => a.Email.Equals(productSaveExtended.User.UserEmail)).FirstOrDefault();

            // get the Configuration with ProductNumber, Name and User
            Configuration? configuration = (
                from c in _context.Configurations
                where c.ProductNumber == productSaveExtended.ConfigId
                && c.ConfigurationsHasLanguages.Select(c => c.Name).Contains(oldSavedName)
                && c.Account == user
                select c
            ).FirstOrDefault();

            return configuration;
        }

        private Configuration? GetConfigurationByWrapper( SavedConfigDeleteWrapper wrapper ) {
            // get the User
            Models.Account? user = _context.Accounts.Where(a => a.Email.Equals(wrapper.UserEmail)).FirstOrDefault();

            // get the Configuration with ProductNumber, Name and User
            Configuration? configuration = (
                from c in _context.Configurations
                where c.ProductNumber == wrapper.ConfigId
                && c.ConfigurationsHasLanguages.Select(c => c.Name).Contains(wrapper.SavedName)
                && c.Account == user
                select c
            ).FirstOrDefault();

            return configuration;
        }

        #endregion

    }
}