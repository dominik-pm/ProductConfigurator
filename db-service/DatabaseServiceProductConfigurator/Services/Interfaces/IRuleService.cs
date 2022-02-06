using Model;

namespace DatabaseServiceProductConfigurator.Services {
    public interface IRuleService {

        RulesExtended ExtendProductDependencies( RulesExtended dependencies, string productNumber );

        RulesExtended ExtendProductDependenciesByOptionField( RulesExtended dependencies, string id );

    }
}
