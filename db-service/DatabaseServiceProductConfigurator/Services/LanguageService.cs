using DatabaseServiceProductConfigurator.Models;

namespace DatabaseServiceProductConfigurator.Services {

    public struct InfoStruct {
        public string Name { get; set; }
        public string Description { get; set; }
    }

    public static class LanguageService {

        static product_configuratorContext context = new product_configuratorContext();

        public static ProductHasLanguage? GetProductWithLanguage( string productNumber, string language ) => context.ProductHasLanguages
            .Where(phs => phs.ProductNumber.Equals(productNumber) && phs.Language.Equals(language))
            .FirstOrDefault();
        public static OptionFieldHasLanguage? GetOptionsfieldWithLanguage( int id, string language ) => context.OptionFieldHasLanguages
            .Where(ohs => ohs.OptionFieldId.Equals(id) && ohs.Language.Equals(language))
            .FirstOrDefault();

        public static List<string> GetAllLanguages () => context.ELanguages.Select(l => l.Language).ToList();

        public static string HandleLanguageInput(string input) {
            if ( input == null )
                return "en";

            string[] dbLangs = GetAllLanguages().ToArray();
            string country = input;

            if(input.Contains('-'))
                country = input.Split('-')[0];

            if ( dbLangs.Contains(country) )
                return country;

            return "en";
        }

    }
}
