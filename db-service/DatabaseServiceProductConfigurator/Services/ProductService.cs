using DatabaseServiceProductConfigurator.Models;
using Microsoft.EntityFrameworkCore;
using Model;
using System.Text.Json.Serialization;

namespace DatabaseServiceProductConfigurator.Services {

    public static class ProductService {

        private static readonly Product_configuratorContext context = new();

        private static List<Configurator> GetConfigurators( string lang ) {
            List<Configurator> temp = (
                from p in context.Products
                let depen = new Rules { BasePrice = p.Price }
                let infos = LanguageService.GetProductWithLanguage(p.ProductNumber, lang)
                select new Configurator {
                    ConfigId = p.ProductNumber,
                    Name = infos.Name,
                    Description = infos.Description,
                    Images = ( from pic in context.Pictures where pic.ProductNumber == p.ProductNumber select pic.Url ).ToList(),
                    Options = GetOptionsByProductNumber(p.ProductNumber, lang),
                    OptionGroups = GetOptionGroupsByProductNumber(p.ProductNumber, lang),
                    OptionSections = GetOptionSectionByProductNumber(p.ProductNumber, lang),
                    Rules = depen,
                }
            ).ToList();

            foreach ( var item in temp ) {
                item.Options.ForEach(o => item.Rules = item.Rules.ExtendProductDependencies(o.Id));
                item.OptionGroups.ForEach(o => item.Rules = item.Rules.ExtendProductDependenciesByOptionField(o.Id));
                item.OptionSections.ForEach(o => item.Rules = item.Rules.ExtendProductDependenciesByOptionField(o.Id));
                item.Rules.Models.AddRange(ConfigurationService.GetModelsByProduct(item.ConfigId, lang));
            }

            return temp;
        }

        public static List<Configurator> GetAllConfigurators( string lang ) => GetConfigurators(lang).Where(c => ( from p in context.Products where p.ProductNumber == c.ConfigId select p.Buyable ).FirstOrDefault()).ToList();

        public static Configurator? GetConfiguratorByProductNumber( string productNumber, string lang ) => GetConfigurators(lang).Where(c => c.ConfigId == productNumber).FirstOrDefault();

        private static List<OptionSection> GetOptionSectionByProductNumber( string productNumber, string lang ) {
            Product_configuratorContext localContext = new();

            List<OptionSection> sections = new();

            List<OptionField> rawFields = new();
            List<OptionField> cookedFields = new();

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
                cookedCount = cookedFields.Count;
                rawCount = rawFields.Count;

                List<OptionField> toAdd = new();
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
            } while ( cookedCount != cookedFields.Count || rawCount != rawFields.Count );

            foreach ( var field in cookedFields ) {
                List<string> options = (
                    from ofo in localContext.OptionFieldsHasOptionFields
                    where ofo.Base == field.Id && ofo.DependencyType == "CHILD"
                    select ofo.OptionField.ToString()
                ).ToList();

                InfoStruct fieldinfos = LanguageService.GetOptionsfieldWithLanguage(field.Id, lang);

                sections.Add(
                    new OptionSection(
                        fieldinfos.Name,
                        field.Id,
                        options
                    )
                );
            }

            return sections;
        }

        private static List<OptionGroup> GetOptionGroupsByProductNumber( string productNumber, string lang ) {
            Product_configuratorContext localContext = new();

            List<OptionGroup> optionGroups = new();

            List<OptionField> rawFields = new();
            List<OptionField> cookedFields = new();

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
                cookedCount = cookedFields.Count;
                rawCount = rawFields.Count;

                List<OptionField> toAdd = new();
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
            } while ( cookedCount != cookedFields.Count || rawCount != rawFields.Count );

            foreach ( var field in cookedFields ) {
                List<string> options = (
                    from pof in localContext.ProductsHasOptionFields
                    where pof.OptionFields == field.Id && pof.DependencyType == "CHILD"
                    let infos = LanguageService.GetProductWithLanguage(pof.ProductNumber, lang)
                    select pof.ProductNumber
                ).ToList();

                InfoStruct fieldinfos = LanguageService.GetOptionsfieldWithLanguage(field.Id, lang);

                optionGroups.Add(
                    new OptionGroup {
                        Id = field.Id,
                        Name = fieldinfos.Name,
                        Description = fieldinfos.Description,
                        OptionIds = options,
                        Required = field.Required
                    }
                );
            }

            return optionGroups;
        }

        private static List<Option> GetOptionsByProductNumber( string productNumber, string lang ) {
            Product_configuratorContext localContext = new();

            List<Option> options = new();

            List<OptionField> fields = (
                from pof in localContext.ProductsHasOptionFields
                where pof.ProductNumber == productNumber && pof.DependencyType == "PARENT"
                select pof.OptionFieldsNavigation
            ).ToList();

            int currentCount = 0;
            do {
                currentCount = fields.Count;
                List<OptionField> toAdd = new();
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
            } while ( currentCount != fields.Count );




            foreach ( var item in fields ) {
                options.AddRange(
                    (
                        from opt in localContext.ProductsHasOptionFields
                        where opt.OptionFields == item.Id && opt.DependencyType == "CHILD"
                        let infos = LanguageService.GetProductWithLanguage(opt.ProductNumber, lang)
                        select new Option {
                            Id = opt.ProductNumber,
                            Name = infos.Name,
                            Description = infos.Description
                        }
                    )
                );
            }

            return options;
        }

        public static bool SaveConfigurator( Configurator config, string lang ) {
            bool worked = true;

            try {

                // MAIN PRODUCT
                context.Products.Add(
                    new Product {
                        ProductNumber = config.ConfigId,
                        Price = config.Rules.BasePrice,
                        Category = "",
                        Buyable = true
                    }
                );

                // OPTION PRODUCTS
                foreach ( var item in config.Options ) {
                    bool priceAvailable = config.Rules.PriceList.TryGetValue(item.Id, out float price);

                    context.Products.Add(
                        new Product {
                            ProductNumber = item.Id,
                            Price = priceAvailable ? price : 0,
                            Category = "",
                            Buyable = false
                        }
                    );
                }

                // REQUIREMENTS
                foreach ( var item in config.Rules.Requirements ) {
                    foreach ( var reqitem in item.Value ) {
                        context.ProductsHasProducts.Add(
                            new ProductsHasProduct {
                                BaseProduct = item.Key,
                                OptionProduct = reqitem,
                                DependencyType = "REQUIRED"
                            }
                        );
                    }
                }

                // INCOMPABILITIES
                foreach ( var item in config.Rules.Incompatibilities ) {
                    foreach ( var reqitem in item.Value ) {
                        context.ProductsHasProducts.Add(
                            new ProductsHasProduct {
                                BaseProduct = item.Key,
                                OptionProduct = reqitem,
                                DependencyType = "EXCLUDING"
                            }
                        );
                    }
                }

                // OPTION SECTIONS
                List<OptionField> temp = new();

                foreach ( var item in config.OptionSections ) {
                    OptionField tempSave = new() {
                        Id = item.Id,
                        Type = "PARENT",
                        Required = false
                    };
                    temp.Add(tempSave);
                    context.OptionFields.Add(tempSave);
                }

                // OPTION GROUPS
                List<string> SingleSelect = new();
                foreach ( var item in config.Rules.ReplacementGroups ) {
                    foreach ( var item2 in item.Value ) {
                        SingleSelect.Add(item2);
                    }
                }

                foreach ( var item in config.OptionGroups ) {
                    OptionField tempSave = new() {
                        Id = item.Id,
                        Type = SingleSelect.Contains(item.Id) ? "SINGLE_SELECT" : "MULTI_SELECT",
                        Required = item.Required
                    };
                    temp.Add(tempSave);
                    context.OptionFields.Add(tempSave);
                }

                // OPTIONFIELD HAS OPTIONFIELD
                foreach ( var item in config.OptionSections ) {
                    foreach ( var field in item.OptionGroupIds ) {
                        context.OptionFieldsHasOptionFields.Add(
                            new OptionFieldsHasOptionField {
                                Base = item.Id,
                                OptionField = field,
                                DependencyType = "CHILD"
                            }
                        );
                    }
                }

                // OPTIONFIELD HAS PRODUCT
                foreach ( var item in config.OptionGroups ) {
                    foreach ( var field in item.OptionIds ) {
                        context.ProductsHasOptionFields.Add(
                            new ProductsHasOptionField {
                                ProductNumber = field,
                                OptionFields = item.Id,
                                DependencyType = "CHILD"
                            }
                        );
                    }
                }

                // PRODUCT HAS OPTIONFIELD
                foreach ( var item in config.OptionSections ) {
                    context.ProductsHasOptionFields.Add(
                        new ProductsHasOptionField {
                            ProductNumber = config.ConfigId,
                            OptionFields = item.Id,
                            DependencyType = "PARENT"
                        }
                    );
                }

                //IMAGES
                foreach ( var item in config.Images ) {
                    context.Pictures.Add(
                        new Picture {
                            ProductNumber = config.ConfigId,
                            Url = item
                        }
                    );
                }

                //LANGUAGE FOR PRODUCTS
                foreach ( var item in config.Options ) {
                    context.ProductHasLanguages.Add(
                        new ProductHasLanguage {
                            ProductNumber = config.ConfigId,
                            Language = lang,
                            Name = item.Name,
                            Description = item.Description
                        }
                    );
                }

                //LANGUAGE FOR OPTIONFIELDS
                foreach ( var item in config.OptionGroups ) {
                    context.OptionFieldHasLanguages.Add(
                        new OptionFieldHasLanguage {
                            OptionFieldId = item.Id,
                            Language = lang,
                            Name = item.Name,
                            Description = item.Description
                        }
                    );
                }

                foreach ( var item in config.OptionSections ) {
                    context.OptionFieldHasLanguages.Add(
                        new OptionFieldHasLanguage {
                            OptionFieldId = item.Id,
                            Language = lang,
                            Name = item.Name,
                            Description = ""
                        }
                    );
                }

                // MODELS
                foreach ( var item in config.Rules.Models ) {
                    ConfigurationService.SaveModels(context, config.ConfigId, item, lang);
                }

                context.SaveChanges();

            }
            catch ( Exception ex ) {
                Console.WriteLine(ex.Message);
                context.Dispose();
                worked = false;
            }

            return worked;

        }

    }
}