using DatabaseServiceProductConfigurator.Models;
using Microsoft.EntityFrameworkCore;
using System.Dynamic;

namespace DatabaseServiceProductConfigurator.Services {

    public struct OptionFieldStruct {
        public string id { get; set; }
        public string type { get; set; }
        public bool required { get; set; }
        public InfoStruct infos { get; set; }
        public List<OptionFieldStruct> children { get; set; } = new List<OptionFieldStruct>();
        public List<object> options { get; set; } = new List<object>();
        public object rules { get; private set; }

        public void SetRules(object rules) => this.rules = rules;
    }

    public static class OptionFieldService {

        private static product_configuratorContext context = new product_configuratorContext();

        public static List<OptionFieldStruct> GetByProductNumber( string productNumber, string lang ) {
            List<OptionFieldStruct> rawData = (
                from of in context.ProductsHasOptionFields
                where of.ProductNumber == productNumber && of.DependencyType == "PARENT"
                select new OptionFieldStruct {
                    id = of.OptionFieldsNavigation.Id,
                    type = of.OptionFieldsNavigation.Type,
                    required = of.OptionFieldsNavigation.Required,
                    infos = (
                        from ohl in context.OptionFieldHasLanguages
                        where ohl.Language == lang && ohl.OptionFieldId == of.OptionFieldsNavigation.Id
                        select new InfoStruct {
                            Name = ohl.Name,
                            Description = ohl.Description
                        }
                    ).FirstOrDefault()
                }
            ).ToList();

            foreach ( var item in rawData ) {
                item.children.AddRange(GetChildren(item.id, lang));
                item.options.AddRange(ProductService.GetByOptionField(item.id, lang));
                item.SetRules(RuleService.GetByOptionField(item.id));
            }

            return rawData;
        }

        public static List<OptionFieldStruct> GetChildren( string id, string lang ) {
            List<OptionFieldStruct> rawData = (
                from of in context.OptionFieldsHasOptionFields
                where of.BaseNavigation.Id == id && of.DependencyType == "CHILD"
                select new OptionFieldStruct {
                    id = of.OptionFieldNavigation.Id,
                    type = of.OptionFieldNavigation.Type,
                    required = of.OptionFieldNavigation.Required,
                    infos = (
                        from ohl in context.OptionFieldHasLanguages
                        where ohl.Language == lang && ohl.OptionFieldId == of.OptionFieldNavigation.Id
                        select new InfoStruct {
                            Name = ohl.Name,
                            Description = ohl.Description
                        }
                    ).FirstOrDefault()
                }
            ).ToList();

            foreach ( var item in rawData ) {
                item.children.AddRange(GetChildren(item.id, lang));
                item.options.AddRange(ProductService.GetByOptionField(item.id, lang));
                item.SetRules(RuleService.GetByOptionField(item.id));
            }

            return rawData;
        }

    }
}
