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
            OptionId = options.Select(x => x.Id).ToList();
            Console.WriteLine(OptionId[0]);
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
                        Options = opts.OptionId
                    });
                }
            );
            return toReturn;
        }

        public List<ProductSaveExtended> GetConfigurations( string lang ) {
            List<Configuration> temp = _context.Configurations.Where(c => c.AccountId != null).ToList();

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

            List<Option> toReturn = new();
            float price = 0;

            foreach ( var item in fields ) {

                foreach ( var el in item.ProductNumbers ) {
                    price += el.Price;

                    InfoStruct infos = _languageService.GetProductWithLanguage(el.ProductNumber, lang);
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

        public void SaveConfiguration( ProductSaveExtended toSave, string lang ) {
            Models.Account? user = _context.Accounts.Where(a => a.Email.Equals(toSave.User.UserEmail)).FirstOrDefault();

            Configuration added = _context.Configurations.Add(
                new Configuration {
                    ProductNumber = toSave.ConfigId,
                    AccountId = user?.Id,
                    Date = toSave.Date
                }
            ).Entity;

            _context.ConfigurationsHasLanguages.Add(
                new ConfigurationsHasLanguage {
                    Configuration = added.Id,
                    Language = lang,
                    Name = toSave.SavedName,
                    Description = ""
                }
            );

            List<string> products = ( from p in _context.Products where toSave.Options.Select(c => c.Id).Contains(p.ProductNumber) select p.ProductNumber ).ToList();

            List<ConfigurationHasOptionField> fields = (
                from of in _context.OptionFields
                join pof in _context.ProductsHasOptionFields on of.Id equals pof.OptionFields
                where products.Contains(pof.ProductNumber)
                select new ConfigurationHasOptionField {
                    ConfigId = added.Id,
                    OptionFieldId = of.Id,
                    ProductNumbers = ( from pof1 in _context.ProductsHasOptionFields where pof1.OptionFields == of.Id && products.Contains(pof.ProductNumber) select pof1.ProductNumberNavigation ).ToList()
                }
            ).ToList();

            _context.ConfigurationHasOptionFields.AddRange(fields);
            _context.SaveChanges();

        }

        public void SaveModels( string productNumber, ModelType model, string lang ) {
            Configuration temp = _context.Configurations.Add(
                new Configuration {
                    Id = 0,
                    ProductNumber = productNumber,
                    AccountId = null
                }
            ).Entity;

            _context.ConfigurationsHasLanguages.Add(
                new ConfigurationsHasLanguage {
                    Configuration = temp.Id,
                    Language = lang,
                    Name = model.Name,
                    Description = model.Description
                }
            );

            foreach ( var item in model.Options ) {
                _context.ConfigurationHasOptionFields.Add(
                    new ConfigurationHasOptionField {
                        ConfigId = temp.Id,
                        OptionFieldId = GetOptionfieldByProductAndOption(productNumber, item)
                    }
                );
            }

            _context.SaveChanges();
        }

        private string GetOptionfieldByProductAndOption( string productNumber, string option ) {
            List<string> products = (
                from of in _context.ProductsHasOptionFields
                where of.ProductNumber == productNumber && of.DependencyType == "PARENT"
                select of.OptionFields
            ).ToList();

            List<string> options = (
                from of in _context.ProductsHasOptionFields
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