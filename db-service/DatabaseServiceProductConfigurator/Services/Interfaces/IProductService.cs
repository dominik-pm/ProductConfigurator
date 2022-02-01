using Model;

namespace DatabaseServiceProductConfigurator.Services {
    public interface IProductService {

        List<Configurator> GetAllConfigurators( string lang );
        Configurator? GetConfiguratorByProductNumber( string productNumber, string lang );
        void SaveConfigurator( Configurator config, string lang );
        void DeleteConfigurator( string productNumber );
        void UpdateProduct( Configurator product );

    }
}
