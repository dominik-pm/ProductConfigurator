using DatabaseServiceProductConfigurator.Context;
using DatabaseServiceProductConfigurator.Models;
using Model;

namespace DatabaseServiceProductConfigurator.Services {

    public class RuleService : IRuleService {

        public RulesExtended ExtendProductDependencies( RulesExtended dependencies, string productNumber, List<Product> dbProducts, List<ProductsHasOptionField> dbProductHasOptionField, List<ProductsHasProduct> dbProductHasProduct ) {

            List<string> ReplacementGroups = (
                    from pof in dbProductHasOptionField
                    where pof.ProductNumber == productNumber && pof.DependencyType == "PARENT" && pof.OptionFieldsNavigation.Type == "SINGLE_SELECT"
                    select pof.OptionFields.ToString()
                ).ToList();
            if(ReplacementGroups.Count > 0)
                dependencies.ReplacementGroups.Add(productNumber, ReplacementGroups);

            List<string> Requirements = (
                    from pof in dbProductHasProduct
                    where pof.BaseProduct == productNumber && pof.DependencyType == "REQUIRED"
                    select pof.OptionProduct
                ).ToList();
            if(Requirements.Count > 0)
                dependencies.Requirements.Add(productNumber, Requirements);

            List<string> Incompabilities = (
                    from pof in dbProductHasProduct
                    where pof.BaseProduct == productNumber && pof.DependencyType == "EXCLUDING"
                    select pof.OptionProduct
                ).ToList();
            if( Incompabilities.Count > 0 )
                dependencies.Incompatibilities.Add(productNumber, Incompabilities);

            float? price = ( from pof in dbProducts where pof.ProductNumber == productNumber select pof.Price ).FirstOrDefault();
            if(price != null && price != 0)
                dependencies.PriceList.Add(productNumber, (float)price);

            return dependencies;
        }

        public RulesExtended ExtendProductDependenciesByOptionField( RulesExtended dependencies, string id, List<ProductsHasOptionField> dbProductHasOptionfield, List<OptionFieldsHasOptionField> dbOptionFieldHasOptionField ) {
            string toSave = id.ToString();

            List<string> ReplacementGroups = (
                      from pof in dbProductHasOptionfield
                      where pof.OptionFields == id && pof.DependencyType == "CHILD" && pof.OptionFieldsNavigation.Type == "SINGLE_SELECT"
                      select pof.ProductNumber
                ).ToList();
            if ( ReplacementGroups.Count > 0 )
                dependencies.ReplacementGroups.Add(toSave, ReplacementGroups);

            List<string> Requirements = (
                    from pof in dbOptionFieldHasOptionField
                    where pof.Base == id && pof.DependencyType == "REQUIRED"
                    select pof.OptionField.ToString()
                ).ToList();
            if ( Requirements.Count > 0 )
                dependencies.GroupRequirements.Add(toSave, Requirements);

            List<string> Incompabilities = (
                    from pof in dbOptionFieldHasOptionField
                    where pof.Base == id && pof.DependencyType == "EXCLUDING"
                    select pof.OptionField.ToString()
                ).ToList();
            if ( Incompabilities.Count > 0 )
                dependencies.Incompatibilities.Add(toSave, Incompabilities);

            return dependencies;
        }

    }
}
