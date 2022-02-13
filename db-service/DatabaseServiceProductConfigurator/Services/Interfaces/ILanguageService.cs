using DatabaseServiceProductConfigurator.Models;

namespace DatabaseServiceProductConfigurator.Services {
    public interface ILanguageService {

        InfoStruct GetProductWithLanguage( string productNumber, string language, List<ProductHasLanguage> dbList );

        InfoStruct GetOptionsfieldWithLanguage( string id, string language, List<OptionFieldHasLanguage> dbList );

        InfoStruct GetConfigurationWithLanguage( int id, string language, List<ConfigurationsHasLanguage> dbList );
        List<string> GetAllLanguages();
        string HandleLanguageInput( string input );
        string HandleLanguageInputCreate( string input );

    }
}
