using DatabaseServiceProductConfigurator.Models;
using Model;
using Model.Wrapper;

namespace DatabaseServiceProductConfigurator.Services {
    public interface IConfigurationService {

        List<ModelType> GetVisibleModelsByProduct( string productNumber, string lang, List<ConfigurationsHasLanguage> langListConfiguration, List<Configuration> configs );

        List<ProductSaveExtended> GetConfigurations( string lang );

        ProductSaveExtended? GetConfiguredProductById( string lang, SavedConfigDeleteWrapper wrapper );

        void SaveConfiguration( ProductSaveExtended toSave, string lang );

        Configuration SaveModels( Product product, ModelType model, string lang );

        void DeleteConfiguration( SavedConfigDeleteWrapper wrapper );
        public void DeleteConfiguration( int id );

        void UpdateConfiguration( ProductSaveExtended config, string lang, string oldSavedName );

    }
}
