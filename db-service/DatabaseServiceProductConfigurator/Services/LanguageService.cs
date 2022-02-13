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

        public static readonly string default_lang = "en";

        private readonly ConfiguratorContext _context;

        public LanguageService( ConfiguratorContext context ) {
            _context = context;
        }

        public InfoStruct GetProductWithLanguage( string productNumber, string language, List<ProductHasLanguage> dbList ) {
            List<ProductHasLanguage> infos = dbList.Where(c => c.ProductNumber == productNumber).ToList();

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

        public InfoStruct GetOptionsfieldWithLanguage( string id, string language, List<OptionFieldHasLanguage> dbList ) {
            List<OptionFieldHasLanguage> infos = dbList.Where(c => c.OptionFieldId == id).ToList();

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

        public InfoStruct GetConfigurationWithLanguage( int id, string language, List<ConfigurationsHasLanguage> dbList ) {
            List<ConfigurationsHasLanguage> infos = dbList.Where(c => c.Configuration == id).ToList();

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

            input = CleanLang(input);

            if ( dbLangs.Contains(input) )
                return input;

            return default_lang;
        }

        public string HandleLanguageInputCreate( string input ) {
            input = CleanLang(input);
            string lang = HandleLanguageInput(input);
            if ( lang != input) {
                _context.ELanguages.Add(new ELanguage { Language = input });
                _context.SaveChanges();
            }
            return input;
        }

        private static string CleanLang(string input) {
            if ( input.Contains('-') )
                input = input.Split('-')[0];
            return input;
        }

    }
}
