using DatabaseServiceProductConfigurator.Context;
using DatabaseServiceProductConfigurator.Models;

namespace DatabaseServiceProductConfigurator.Services {

    public struct InfoStruct {

        public InfoStruct( string Name, string Description ) {
            this.Name = Name;
            this.Description = Description;
        }

        public InfoStruct() { }

        public string Name { get; set; } = "";
        public string Description { get; set; } = "";
    }

    public class LanguageService : ILanguageService {

        static readonly string default_lang = "en";

        private readonly ConfiguratorContext _context;

        public LanguageService( ConfiguratorContext context ) {
            _context = context;
        }

        public InfoStruct GetProductWithLanguage( string productNumber, string language ) {
            List<ProductHasLanguage> infos = _context.ProductHasLanguages.Where(c => c.ProductNumber == productNumber).ToList();

            ProductHasLanguage temp;

            if ( infos.Select(c => c.Language).Contains(language) ) {
                temp = infos.Where(c => c.Language == language).First();
                return new InfoStruct(temp.Name, temp.Description);
            }
            else if ( infos.Select(c => c.Language).Contains(default_lang) ) {
                temp = infos.Where(c => c.Language == default_lang).First();
                return new InfoStruct(temp.Name, temp.Description);
            }
            else
                return new InfoStruct();
        }

        public InfoStruct GetOptionsfieldWithLanguage( string id, string language ) {
            List<OptionFieldHasLanguage> infos = _context.OptionFieldHasLanguages.Where(c => c.OptionFieldId == id).ToList();

            OptionFieldHasLanguage temp;

            if ( infos.Select(c => c.Language).Contains(language) ) {
                temp = infos.Where(c => c.Language == language).First();
                return new InfoStruct(temp.Name, temp.Description);
            }
            else if ( infos.Select(c => c.Language).Contains(default_lang) ) {
                temp = infos.Where(c => c.Language == default_lang).First();
                return new InfoStruct(temp.Name, temp.Description);
            }
            else
                return new InfoStruct();
        }

        public InfoStruct GetConfigurationWithLanguage( int id, string language ) {
            List<ConfigurationsHasLanguage> infos = _context.ConfigurationsHasLanguages.Where(c => c.Configuration == id).ToList();

            ConfigurationsHasLanguage temp;

            if ( infos.Select(c => c.Language).Contains(language) ) {
                temp = infos.Where(c => c.Language == language).First();
                return new InfoStruct(temp.Name, temp.Description);
            }
            else if ( infos.Select(c => c.Language).Contains(default_lang) ) {
                temp = infos.Where(c => c.Language == default_lang).First();
                return new InfoStruct(temp.Name, temp.Description);
            }
            else
                return new InfoStruct();

        }

        public List<string> GetAllLanguages() {
            return _context.ELanguages.Select(l => l.Language).ToList();
        }

        public string HandleLanguageInput( string input ) {
            if ( input == null )
                return default_lang;

            string[] dbLangs = GetAllLanguages().ToArray();
            string country = input;

            if ( input.Contains('-') )
                country = input.Split('-')[0];

            if ( dbLangs.Contains(country) )
                return country;

            return default_lang;
        }

    }
}
