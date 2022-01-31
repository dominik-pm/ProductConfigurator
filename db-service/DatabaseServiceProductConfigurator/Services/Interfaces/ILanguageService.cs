namespace DatabaseServiceProductConfigurator.Services {
    public interface ILanguageService {

        InfoStruct GetProductWithLanguage( string productNumber, string language );

        InfoStruct GetOptionsfieldWithLanguage( string id, string language );

        InfoStruct GetConfigurationWithLanguage( int id, string language );
        List<string> GetAllLanguages();
        string HandleLanguageInput( string input );

    }
}
