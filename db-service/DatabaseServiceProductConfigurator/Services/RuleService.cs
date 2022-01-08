using DatabaseServiceProductConfigurator.Models;
using Model;

namespace DatabaseServiceProductConfigurator.Services {

    public static class RuleService {

        static product_configuratorContext context = new product_configuratorContext();

        #region Backend

        public static ProductDependencies ExtendProductDependencies( this ProductDependencies dependencies, string productNumber ) {

            List<string> ReplacementGroups = (
                    from pof in context.ProductsHasOptionFields
                    where pof.ProductNumber == productNumber && pof.DependencyType == "PARENT" && pof.OptionFieldsNavigation.Type == "SINGLE_SELECT"
                    select pof.OptionFields.ToString()
                ).ToList();
            if(ReplacementGroups.Count > 0)
                dependencies.ReplacementGroups.Add(productNumber, ReplacementGroups);

            List<string> Requirements = (
                    from pof in context.ProductsHasProducts
                    where pof.BaseProduct == productNumber && pof.DependencyType == "REQUIRED"
                    select pof.OptionProduct
                ).ToList();
            if(Requirements.Count > 0)
                dependencies.Requirements.Add(productNumber, Requirements);

            List<string> Incompabilities = (
                    from pof in context.ProductsHasProducts
                    where pof.BaseProduct == productNumber && pof.DependencyType == "EXCLUDING"
                    select pof.OptionProduct
                ).ToList();
            if( Incompabilities.Count > 0 )
                dependencies.Incompabilities.Add(productNumber, Incompabilities);

            float? price = ( from pof in context.Products where pof.ProductNumber == productNumber select pof.Price ).FirstOrDefault();
            if(price != null && price != 0)
                dependencies.PriceList.Add(productNumber, (float)price);

            return dependencies;
        }

        public static ProductDependencies ExtendProductDependenciesByOptionField( this ProductDependencies dependencies, string id ) {
            string toSave = id.ToString();

            List<string> ReplacementGroups = (
                      from pof in context.OptionFieldsHasOptionFields
                      where pof.Base == id && pof.DependencyType == "CHILD" && pof.OptionFieldNavigation.Type == "SINGLE_SELECT"
                      select pof.OptionField.ToString()
                ).ToList();
            if ( ReplacementGroups.Count > 0 )
                dependencies.ReplacementGroups.Add(toSave, ReplacementGroups);

            List<string> Requirements = (
                    from pof in context.OptionFieldsHasOptionFields
                    where pof.Base == id && pof.DependencyType == "REQUIRED"
                    select pof.OptionField.ToString()
                ).ToList();
            if ( Requirements.Count > 0 )
                dependencies.GroupRequirements.Add(toSave, Requirements);

            List<string> Incompabilities = (
                    from pof in context.OptionFieldsHasOptionFields
                    where pof.Base == id && pof.DependencyType == "EXCLUDING"
                    select pof.OptionField.ToString()
                ).ToList();
            if ( Incompabilities.Count > 0 )
                dependencies.Incompabilities.Add(toSave, Incompabilities);

            return dependencies;
        }

        #endregion

        #region DB
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

        public static object GetByOptionField( string id ) {
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

        #endregion

    }
}
