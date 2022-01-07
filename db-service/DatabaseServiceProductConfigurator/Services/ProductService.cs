using DatabaseServiceProductConfigurator.Models;
using Microsoft.EntityFrameworkCore;
using Model;
using System.Text.Json.Serialization;

namespace DatabaseServiceProductConfigurator.Services {

    public static class ProductService {

        private static product_configuratorContext context = new product_configuratorContext();

        #region Backend

        private static List<Configurator> getConfigurators( string lang ) {
            List<Configurator> temp = (
                from p in context.Products
                let depen = new ProductDependencies(p.Price)
                let infos = LanguageService.GetProductWithLanguage(p.ProductNumber, lang)
                select new Configurator {
                    ConfigId = p.ProductNumber,
                    Name = infos == null ? "" : infos.Name,
                    Description = infos == null ? "" : infos.Description,
                    Images = ( from pic in context.Pictures where pic.ProductNumber == p.ProductNumber select pic.Url ).ToList(),
                    Options = GetOptionsByProductNumber(p.ProductNumber, lang),
                    OptionGroups = GetOptionGroupsByProductNumber(p.ProductNumber, lang),
                    OptionSections = GetOptionSectionByProductNumber(p.ProductNumber, lang),
                    Dependencies = depen,
                }
            ).ToList();

            foreach ( var item in temp ) {
                item.Options.ForEach(o => item.Dependencies = item.Dependencies.ExtendProductDependencies(o.Id));
                item.OptionGroups.ForEach(o => item.Dependencies = item.Dependencies.ExtendProductDependencies(o.Id));
                item.OptionSections.ForEach(o => item.Dependencies = item.Dependencies.ExtendProductDependencies(o.Id));
            }

            return temp;
        }

        public static List<Configurator> getAllConfigurators (string lang) => getConfigurators(lang).Where(c => (from p in context.Products where p.ProductNumber == c.ConfigId select p.Buyable).FirstOrDefault()).ToList();

        public static Configurator? GetConfiguratorByProductNumber (string productNumber, string lang) => getConfigurators(lang).Where(c => c.ConfigId == productNumber).FirstOrDefault();

        private static List<OptionSection> GetOptionSectionByProductNumber( string productNumber, string lang ) {
            product_configuratorContext localContext = new product_configuratorContext();

            List<OptionSection> sections = new List<OptionSection>();

            List<OptionField> rawFields = new List<OptionField>();
            List<OptionField> cookedFields = new List<OptionField>();

            rawFields.AddRange(
                (
                    from of in localContext.ProductsHasOptionFields
                    where of.ProductNumber == productNumber && of.DependencyType == "PARENT"
                    select of.OptionFieldsNavigation
                )
            );

            int cookedCount = 0;
            int rawCount = 0;
            do {
                cookedCount = cookedFields.Count();
                rawCount = rawFields.Count();

                List<OptionField> toAdd = new List<OptionField> ();
                foreach ( var field in rawFields ) {
                    if ( field.Type == "PARENT" ) {
                        toAdd.AddRange(
                            (
                                from ofo in localContext.OptionFieldsHasOptionFields
                                where field.Id == ofo.Base && !rawFields.Select(r => r.Id).Contains(ofo.OptionField)
                                select ofo.OptionFieldNavigation
                            ).ToList()
                        );
                        if ( !cookedFields.Select(cf => cf.Id).Contains(field.Id) ) {
                            cookedFields.Add(field);
                        }
                    }
                }
                rawFields.AddRange(toAdd);
            } while ( cookedCount != cookedFields.Count() || rawCount != rawFields.Count() );

            foreach ( var field in cookedFields ) {
                List<string> options = (
                    from ofo in localContext.OptionFieldsHasOptionFields
                    where ofo.Base == field.Id && ofo.DependencyType == "CHILD"
                    select ofo.OptionField.ToString()
                ).ToList();

                OptionFieldHasLanguage? fieldinfos = LanguageService.GetOptionsfieldWithLanguage(field.Id, lang);

                sections.Add(
                    new OptionSection (
                        (fieldinfos == null ? "" : fieldinfos.Name),
                        field.Id,
                        options
                    )
                );
            }

            return sections;
        }

        private static List<OptionGroup> GetOptionGroupsByProductNumber( string productNumber, string lang ) {
            product_configuratorContext localContext = new product_configuratorContext();

            List<OptionGroup> optionGroups = new List<OptionGroup>();

            List<OptionField> rawFields = new List<OptionField>();
            List<OptionField> cookedFields = new List<OptionField>();

            rawFields.AddRange(
                (
                    from of in localContext.ProductsHasOptionFields
                    where of.ProductNumber == productNumber && of.DependencyType == "PARENT"
                    select of.OptionFieldsNavigation
                )
            );

            int cookedCount = 0;
            int rawCount = 0;
            do {
                cookedCount = cookedFields.Count();
                rawCount = rawFields.Count();

                List<OptionField> toAdd = new List<OptionField> ();
                foreach ( var field in rawFields ) {
                    if ( field.Type == "PARENT" ) {
                        toAdd.AddRange(
                            (
                                from ofo in localContext.OptionFieldsHasOptionFields
                                where field.Id == ofo.Base && !rawFields.Select(r => r.Id).Contains(ofo.OptionField)
                                select ofo.OptionFieldNavigation
                            ).ToList()
                        );
                    }
                    else {
                        if ( !cookedFields.Select(cf => cf.Id).Contains(field.Id) ) {
                            cookedFields.Add(field);
                        }
                    }
                }
                rawFields.AddRange(toAdd);
            } while ( cookedCount != cookedFields.Count() || rawCount != rawFields.Count() );

            foreach ( var field in cookedFields ) {
                List<string> options = (
                    from pof in localContext.ProductsHasOptionFields
                    where pof.OptionFields == field.Id && pof.DependencyType == "CHILD"
                    let infos = LanguageService.GetProductWithLanguage(pof.ProductNumber, lang)
                    select pof.ProductNumber
                ).ToList();

                OptionFieldHasLanguage? fieldinfos = LanguageService.GetOptionsfieldWithLanguage(field.Id, lang);

                optionGroups.Add(
                    new OptionGroup {
                        Id = field.Id,
                        Name = fieldinfos == null ? "" : fieldinfos.Name,
                        Description = fieldinfos == null ? "" : fieldinfos.Description,
                        OptionIds = options,
                        Required = field.Required
                    }
                );
            }

            return optionGroups;
        }

        private static List<Option> GetOptionsByProductNumber( string productNumber, string lang ) {
            product_configuratorContext localContext = new product_configuratorContext();

            List<Option> options = new List<Option>();

            List<OptionField> fields = (
                from pof in localContext.ProductsHasOptionFields
                where pof.ProductNumber == productNumber && pof.DependencyType == "PARENT"
                select pof.OptionFieldsNavigation
            ).ToList();

            int currentCount = 0;
            do {
                currentCount = fields.Count();
                List<OptionField> toAdd = new List<OptionField>();
                foreach ( var item in fields ) {
                    toAdd.AddRange(
                        (
                            from of in localContext.OptionFieldsHasOptionFields
                            where of.Base == item.Id && !fields.Select(p => p.Id).Contains(of.OptionField)
                            select of.OptionFieldNavigation
                        ).ToList()
                    );
                }
                fields.AddRange(toAdd);
            } while ( currentCount != fields.Count() );




            foreach ( var item in fields ) {
                options.AddRange(
                    (
                        from opt in localContext.ProductsHasOptionFields
                        where opt.OptionFields == item.Id && opt.DependencyType == "CHILD"
                        let infos = LanguageService.GetProductWithLanguage(productNumber, lang)
                        select new Option(
                            opt.ProductNumber,
                            infos.Name,
                            infos.Description
                        )
                    )
                );
            }

            return options;
        }

        #endregion

        #region DB

        public static List<object> GetBuyableProducts( string lang ) {
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

        public static List<object> GetByOptionField( int id, string lang ) {

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

        #endregion
    }
}