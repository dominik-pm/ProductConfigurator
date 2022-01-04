using DatabaseServiceProductConfigurator.Models;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;

namespace DatabaseServiceProductConfigurator.Services {
    public static class OptionFieldService {

        private static product_configuratorContext context = new product_configuratorContext();

        public static List<object> GetByProductNumber( string productNumber, string lang ) {
            List<object> rawData = (
                from of in context.ProductsHasOptionFields
                where of.ProductNumber == productNumber && of.DependencyType == "PARENT"
                select new {
                    id = of.OptionFieldsNavigation.Id,
                    type = of.OptionFieldsNavigation.Type,
                    infos = (
                        from ohl in context.OptionFieldHasLanguages
                        where ohl.Language == lang && ohl.OptionFieldId == of.OptionFieldsNavigation.Id
                        select new {
                            ohl.Name,
                            ohl.Description
                        }
                    ).FirstOrDefault()
                }
            ).ToList<object>();

            List<object> cookedData = new List<object>();

            foreach ( var item in rawData ) {

                int id = (int) item.GetType().GetProperty("id").GetValue(item);

                dynamic newData = new ExpandoObject();
                foreach ( var prop in item.GetType().GetProperties() ) {
                    ( newData as IDictionary<string, Object> ).Add(prop.Name, prop.GetValue(item));
                }

                ( newData as IDictionary<string, object> ).Add("children", GetChildren(id, lang));
                ( newData as IDictionary<string, object> ).Add("options", ProductService.GetByOptionField(id, lang));
                ( newData as IDictionary<string, object> ).Add("rules", RuleService.GetByOptionField(id));

                cookedData.Add(newData);
            }

            return cookedData;
        }

        public static List<object> GetChildren( int id, string lang ) {
            List<object> rawData = (
                from of in context.OptionFieldsHasOptionFields
                where of.BaseNavigation.Id == id && of.DependencyType == "CHILD"
                select new {
                    id = of.OptionFieldNavigation.Id,
                    type = of.OptionFieldNavigation.Type,
                    infos = (
                        from ohl in context.OptionFieldHasLanguages
                        where ohl.Language == lang && ohl.OptionFieldId == of.OptionFieldNavigation.Id
                        select new {
                            ohl.Name,
                            ohl.Description
                        }
                    ).FirstOrDefault()
                }
            ).ToList<object>();

            List<object> cookedData = new List<object>();

            foreach ( var item in rawData ) {

                int localID = (int) item.GetType().GetProperty("id").GetValue(item);

                dynamic newData = new ExpandoObject();
                foreach ( var prop in item.GetType().GetProperties() ) {
                    ( newData as IDictionary<string, Object> ).Add(prop.Name, prop.GetValue(item));
                }

                (newData as IDictionary<string, object>).Add("children", GetChildren(localID, lang));
                ( newData as IDictionary<string, object> ).Add("options", ProductService.GetByOptionField(localID, lang));
                ( newData as IDictionary<string, object> ).Add("rules", RuleService.GetByOptionField(localID));

                cookedData.Add(newData);
            }

            return cookedData;
        }

    }
}
