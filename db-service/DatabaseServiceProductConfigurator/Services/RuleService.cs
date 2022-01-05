using DatabaseServiceProductConfigurator.Models;

namespace DatabaseServiceProductConfigurator.Services {
    public static class RuleService {

        static product_configuratorContext context = new product_configuratorContext();

        public static object GetByProductNumber( string productNumber ) {
            return new {
                replacementGroups = (
                    from pof in context.ProductsHasOptionFields
                    where pof.ProductNumber == productNumber && pof.OptionFieldsNavigation.Type == "SINGLE_SELECT" && pof.DependencyType == "PARENT"
                    select pof.OptionFields
                ).ToList(),
                requirements = (
                    from pof in context.ProductsHasProducts
                    where pof.BaseProduct == productNumber && pof.DependencyType == "REQUIRED"
                    select pof.OptionProduct
                ).ToList(),
                incompatibilites = (
                    from pof in context.ProductsHasProducts
                    where pof.BaseProduct == productNumber && pof.DependencyType == "EXCLUDING"
                    select pof.OptionProduct
                ).ToList()
            };
        }

        public static object GetByOptionField( int id ) {
            return new {
                replacementGroups = (
                      from pof in context.OptionFieldsHasOptionFields
                      where pof.Base == id && pof.DependencyType == "CHILD" && pof.OptionFieldNavigation.Type == "SINGLE_SELECT"
                      select pof.OptionField
                ).ToList(),
                requirements = (
                    from pof in context.OptionFieldsHasOptionFields
                    where pof.Base == id && pof.DependencyType == "REQUIRED"
                    select pof.OptionField
                ).ToList(),
                incompatibilites = (
                    from pof in context.OptionFieldsHasOptionFields
                    where pof.Base == id && pof.DependencyType == "EXCLUDING"
                    select pof.OptionField
                ).ToList()
            };
        }

    }
}
