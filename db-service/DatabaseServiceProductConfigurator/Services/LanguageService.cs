using DatabaseServiceProductConfigurator.Models;

namespace DatabaseServiceProductConfigurator.Services {
    public static class LanguageService {

        static product_configuratorContext context = new product_configuratorContext();

        public static ProductHasLanguage? GetProductWithLanguage( string productNumber, string language ) => context.ProductHasLanguages
            .Where(phs => phs.ProductNumber.Equals(productNumber) && phs.Language.Equals(language))
            .FirstOrDefault();
        public static OptionFieldHasLanguage? GetOptionsfieldWithLanguage( int id, string language ) => context.OptionFieldHasLanguages
            .Where(ohs => ohs.OptionFieldId.Equals(id) && ohs.Language.Equals(language))
            .FirstOrDefault();

        public static List<string> GetAllLanguages () => context.ELanguages.Select(l => l.Language).ToList();

    }
}
