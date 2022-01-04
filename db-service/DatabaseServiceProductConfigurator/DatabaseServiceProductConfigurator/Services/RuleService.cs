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
                    select new { pof.BaseProduct, pof.OptionProduct }
                ).ToList(),
                incompatibilites = (
                    from pof in context.ProductsHasProducts
                    where pof.BaseProduct == productNumber && pof.DependencyType == "EXCLUDING"
                    select new { pof.BaseProduct, pof.OptionProduct }
                ).ToList()
            };
        }

        public static object GetByOptionField( int id ) {
            return new {
                replacementGroups = (
                      from pof in context.ProductsHasOptionFields
                      where pof.OptionFields == id && pof.OptionFieldsNavigation.Type == "SINGLE_SELECT" && pof.DependencyType == "CHILD"
                      select pof.ProductNumber
                ).ToList(),
                requirements = (
                    from pof in context.OptionFieldsHasOptionFields
                    where pof.Base == id && pof.DependencyType == "REQUIRED"
                    select new { pof.Base, pof.OptionField }
                ).ToList(),
                incompatibilites = (
                    from pof in context.OptionFieldsHasOptionFields
                    where pof.Base == id && pof.DependencyType == "EXCLUDING"
                    select new { pof.Base, pof.OptionField }
                ).ToList()
            };
        }

    }
}
