using Model;

namespace DatabaseServiceProductConfigurator.Services {
    public interface IConfigurationService {

        List<ModelType> GetModelsByProduct( string productNumber, string lang );

        List<ProductSaveExtended> GetConfigurations( string lang );

        ProductSaveExtended? GetConfiguredProductById( string id );

        void SaveConfiguration( ProductSaveExtended toSave, string lang );

        void SaveModels( string productNumber, ModelType model, string lang );

        void DeleteConfiguration( int configID );

        void UpdateConfiguration( ConfiguredProduct config );

    }
}
