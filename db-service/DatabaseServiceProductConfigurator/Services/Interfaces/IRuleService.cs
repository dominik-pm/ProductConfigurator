using Model;

namespace DatabaseServiceProductConfigurator.Services {
    public interface IRuleService {

        Rules ExtendProductDependencies( Rules dependencies, string productNumber );

        Rules ExtendProductDependenciesByOptionField( Rules dependencies, string id );

    }
}
