using DatabaseServiceProductConfigurator.Models;
using Model;

namespace DatabaseServiceProductConfigurator.Services {
    public interface IRuleService {

        RulesExtended ExtendProductDependencies( RulesExtended dependencies, string productNumber, List<Product> dbProducts, List<ProductsHasOptionField> dbProductHasOptionField, List<ProductsHasProduct> dbProductHasProduct );

        RulesExtended ExtendProductDependenciesByOptionField( RulesExtended dependencies, string id, List<ProductsHasOptionField> dbProductHasOptionfield, List<OptionFieldsHasOptionField> dbOptionFieldHasOptionField );

    }
}
