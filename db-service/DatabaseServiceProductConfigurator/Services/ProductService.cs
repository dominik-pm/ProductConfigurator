using DatabaseServiceProductConfigurator.Models;
using Microsoft.EntityFrameworkCore;
using System.Text.Json.Serialization;

namespace DatabaseServiceProductConfigurator.Services {

    public static class ProductService {

        private static product_configuratorContext context = new product_configuratorContext();

        public static List<object> GetBuyableProducts(string lang) {
            return (
                from p in context.Products
                where p.Buyable == true
                let infos = LanguageService.GetProductWithLanguage(p.ProductNumber, lang)
                select new {
                    productNumber = p.ProductNumber,
                    price = p.Price,
                    Pictures = p.Pictures.Select(pic => pic.Url),
                    category = p.Category,
                    infos = new {
                        name = infos.Name,
                        description = infos.Description
                    }
                }
            ).ToList<object>();
        }

        //GetWithOptions and the right language
        public static object? GetWithOption( string productNumber, string lang ) {
            return
                ( from p in context.Products
                  where p.ProductNumber.Equals(productNumber)
                  select new {
                      p.ProductNumber,
                      p.Price,
                      p.Category,
                      p.Buyable,
                      Pictures = ( from pcs in context.Pictures where pcs.ProductNumber.Equals(p.ProductNumber) select pcs.Url ).ToList(),
                      infos = (
                        from pl in context.ProductHasLanguages
                        where pl.ProductNumber.Equals(p.ProductNumber) && pl.Language.Equals(lang)
                        select new {
                            pl.Name,
                            pl.Description
                        }
                      ).FirstOrDefault(),
                      fields = OptionFieldService.GetByProductNumber(productNumber, lang),
                      rules = RuleService.GetByProductNumber(productNumber)
                  }
                ).FirstOrDefault();
        }

        public static List<object> GetByOptionField (int id, string lang ) {

            return (
                from pof in context.ProductsHasOptionFields
                where pof.OptionFields == id && pof.DependencyType == "CHILD"
                select new {
                    pof.ProductNumber,
                    pof.ProductNumberNavigation.Price,
                    pof.ProductNumberNavigation.Category,
                    Pictures = ( from pcs in context.Pictures where pcs.ProductNumber.Equals(pof.ProductNumber) select pcs.Url ).ToList(),
                    info = (
                        from pl in context.ProductHasLanguages
                        where pl.ProductNumber.Equals(pof.ProductNumber) && pl.Language.Equals(lang)
                        select new {
                            pl.Name,
                            pl.Description
                        }
                    ).FirstOrDefault(),
                    rules = RuleService.GetByProductNumber(pof.ProductNumber)
                }
            ).ToList<object>();
        }
    }
}