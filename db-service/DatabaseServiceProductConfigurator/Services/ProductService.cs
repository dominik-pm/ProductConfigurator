using DatabaseServiceProductConfigurator.Context;
using DatabaseServiceProductConfigurator.Models;
using Microsoft.EntityFrameworkCore;
using Model;
using System.Text.Json.Serialization;

namespace DatabaseServiceProductConfigurator.Services {

    internal struct DBStruct {
        public List<ProductsHasOptionField> ProductHasOptionField { get; set; }
        public List<OptionFieldsHasOptionField> OptionFieldsHasOptionField { get; set; }
        public List<ProductHasLanguage> LangListProduct { get; set; }
        public List<OptionFieldHasLanguage> LangListOptionField { get; set; }
    }

    public class ProductService : IProductService {

        private readonly ConfiguratorContext _context;
        private readonly ILanguageService _languageService;
        private readonly IConfigurationService _configurationService;
        private readonly IRuleService _ruleService;

        public ProductService( ConfiguratorContext context, ILanguageService languageService, IConfigurationService configurationService, IRuleService ruleService ) {
            _context = context;
            _languageService = languageService;
            _configurationService = configurationService;
            _ruleService = ruleService;
        }

        #region GET
        private List<Configurator> GetConfigurators( string lang ) {
            DBStruct thisDbStruct = new() {
                ProductHasOptionField = _context.ProductsHasOptionFields.ToList(),
                OptionFieldsHasOptionField = _context.OptionFieldsHasOptionFields.ToList(),
                LangListProduct = _context.ProductHasLanguages.ToList(),
                LangListOptionField = _context.OptionFieldHasLanguages.ToList(),
            };
            List<ProductsHasProduct> dbProductHasProduct = _context.ProductsHasProducts.ToList();
            List<OptionFieldsHasOptionField> dbOptionFieldHasOptionField = _context.OptionFieldsHasOptionFields.ToList();
            List<Configuration> dbConfigurations = _context.Configurations.ToList();
            List<ConfigurationsHasLanguage> dbconfigurationsHasLanguages = _context.ConfigurationsHasLanguages.ToList();
            List<Product> products = _context.Products.ToList();

            List<Configurator> temp = new();
            products.ForEach(
                p => {
                    var depen = new RulesExtended { BasePrice = p.Price };
                    var infos = _languageService.GetProductWithLanguage(p.ProductNumber, lang, thisDbStruct.LangListProduct);
                    temp.Add(new Configurator {
                        ConfigId = p.ProductNumber,
                        Name = infos.Name,
                        Description = infos.Description,
                        Images = ( from pic in _context.Pictures where pic.ProductNumber == p.ProductNumber select pic.Url ).ToList(),
                        Options = GetOptionsByProductNumber(p.ProductNumber, lang, thisDbStruct),
                        OptionGroups = GetOptionGroupsByProductNumber(p.ProductNumber, lang, thisDbStruct),
                        OptionSections = GetOptionSectionByProductNumber(p.ProductNumber, lang, thisDbStruct),
                        Rules = depen,
                    });
                }
            );

            foreach ( var item in temp ) {
                item.Options.ForEach(o => item.Rules = _ruleService.ExtendProductDependencies(item.Rules, o.Id, products, thisDbStruct.ProductHasOptionField, dbProductHasProduct));
                item.OptionGroups.ForEach(o => item.Rules = _ruleService.ExtendProductDependenciesByOptionField(item.Rules, o.Id, thisDbStruct.ProductHasOptionField, dbOptionFieldHasOptionField));
                item.OptionSections.ForEach(o => item.Rules = _ruleService.ExtendProductDependenciesByOptionField(item.Rules, o.Id, thisDbStruct.ProductHasOptionField, dbOptionFieldHasOptionField));
                item.Rules.Models.AddRange(_configurationService.GetVisibleModelsByProduct(item.ConfigId, lang, dbconfigurationsHasLanguages, dbConfigurations));
            }

            return temp;
        }

        public List<Configurator> GetAllConfigurators( string lang ) {
            List<Product> products = _context.Products.ToList();
            Dictionary<string, bool> temp = new();
            products.ForEach(p => {
                temp[p.ProductNumber] = p.Buyable;
            });
            return GetConfigurators(lang).Where(c => temp[c.ConfigId]).ToList();
        }

        public Configurator? GetConfiguratorByProductNumber( string productNumber, string lang ) => GetConfigurators(lang).Where(c => c.ConfigId == productNumber).FirstOrDefault();

        private List<OptionSection> GetOptionSectionByProductNumber( string productNumber, string lang, DBStruct thisDb ) {

            List<OptionSection> sections = new();

            List<OptionField> rawFields = new();
            List<OptionField> cookedFields = new();

            rawFields.AddRange(
                (
                    from of in thisDb.ProductHasOptionField
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
                                from ofo in thisDb.OptionFieldsHasOptionField
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
                    from ofo in thisDb.OptionFieldsHasOptionField
                    where ofo.Base == field.Id && ofo.DependencyType == "CHILD"
                    select ofo.OptionField.ToString()
                ).ToList();

                InfoStruct fieldinfos = _languageService.GetOptionsfieldWithLanguage(field.Id, lang, thisDb.LangListOptionField);

                sections.Add(
                    new OptionSection {
                        Name = fieldinfos.Name,
                        Id = field.Id,
                        OptionGroupIds = options
                    }
                );
            }

            return sections;
        }

        private List<OptionGroup> GetOptionGroupsByProductNumber( string productNumber, string lang, DBStruct thisDb ) {

            List<OptionGroup> optionGroups = new();

            List<OptionField> rawFields = new();
            List<OptionField> cookedFields = new();

            rawFields.AddRange(
                (
                    from of in thisDb.ProductHasOptionField
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
                                from ofo in thisDb.OptionFieldsHasOptionField
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
                    from pof in thisDb.ProductHasOptionField
                    where pof.OptionFields == field.Id && pof.DependencyType == "CHILD"
                    let infos = _languageService.GetProductWithLanguage(pof.ProductNumber, lang, thisDb.LangListProduct)
                    select pof.ProductNumber
                ).ToList();

                InfoStruct fieldinfos = _languageService.GetOptionsfieldWithLanguage(field.Id, lang, thisDb.LangListOptionField);

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

        private List<Option> GetOptionsByProductNumber( string productNumber, string lang, DBStruct thisDb ) {

            List<Option> options = new();

            List<OptionField> fields = (
                from pof in thisDb.ProductHasOptionField
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
                            from of in thisDb.OptionFieldsHasOptionField
                            where of.Base == item.Id && !fields.Select(p => p.Id).Contains(of.OptionField)
                            select of.OptionFieldNavigation
                        ).ToList()
                    );
                }
                fields.AddRange(toAdd);
            } while ( currentCount != fields.Count );

            foreach ( var item in fields ) {
                List<ProductsHasOptionField> temp = (
                    from opt in thisDb.ProductHasOptionField
                    where opt.OptionFields == item.Id && opt.DependencyType == "CHILD"
                    select opt
                ).ToList();
                List<Option> toAdd = new();
                temp.ForEach(
                    opt => {
                        var infos = _languageService.GetProductWithLanguage(opt.ProductNumber, lang, thisDb.LangListProduct);
                        toAdd.Add(new Option {
                            Id = opt.ProductNumber,
                            Name = infos.Name,
                            Description = infos.Description
                        });
                    }
                );
                options.AddRange(toAdd);
            }

            return options;
        }

        #endregion

        #region POST

        public void SaveConfigurator( Configurator config, string lang ) {

            // MAIN PRODUCT
            Product MainProduct =
                new() {
                    ProductNumber = config.ConfigId,
                    Price = config.Rules.BasePrice,
                    Buyable = true
                };
            MainProduct = _context.Products.Add(MainProduct).Entity;

            // OPTION PRODUCTS
            List<Product> OptionProducts = new();
            foreach ( var item in config.Options ) {
                bool priceAvailable = config.Rules.PriceList.TryGetValue(item.Id, out float price);

                Product temp = new() {
                    ProductNumber = item.Id,
                    Price = priceAvailable ? price : 0,
                    Buyable = false
                };

                OptionProducts.Add(temp);
            }
            _context.Products.AddRange(OptionProducts);

            //var products = _context.Products.ToList();
            var dependencyTypes = _context.EDependencyTypes.ToList();

            // REQUIREMENTS
            foreach ( var item in config.Rules.Requirements ) {
                foreach ( var reqitem in item.Value ) {
                    _context.ProductsHasProducts.Add(
                        new ProductsHasProduct {
                            BaseProduct = item.Key,
                            BaseProductNavigation = OptionProducts.Where(p => p.ProductNumber == item.Key).First(),
                            OptionProduct = reqitem,
                            OptionProductNavigation = OptionProducts.Where(p => p.ProductNumber == reqitem).First(),
                            DependencyType = "REQUIRED",
                            DependencyTypeNavigation = dependencyTypes.Where(d => d.Type == "REQUIRED").First()
                        }
                    );
                }
            }

            // INCOMPABILITIES
            foreach ( var item in config.Rules.Incompatibilities ) {
                foreach ( var reqitem in item.Value ) {
                    _context.ProductsHasProducts.Add(
                        new ProductsHasProduct {
                            BaseProduct = item.Key,
                            BaseProductNavigation = OptionProducts.Where(p => p.ProductNumber == item.Key).First(),
                            OptionProduct = reqitem,
                            OptionProductNavigation = OptionProducts.Where(p => p.ProductNumber == reqitem).First(),
                            DependencyType = "EXCLUDING",
                            DependencyTypeNavigation = dependencyTypes.Where(d => d.Type == "EXCLUDING").First()
                        }
                    );
                }
            }

            // OPTION SECTIONS
            List<OptionField> optionFields = new();
            var optionTypes = _context.EOptionTypes.ToList();

            foreach ( var item in config.OptionSections ) {
                OptionField tempSave = new() {
                    Id = item.Id,
                    Type = "PARENT",
                    TypeNavigation = optionTypes.Where(o => o.Type == "PARENT").First(),
                    Required = false
                };
                optionFields.Add(tempSave);
            }

            // OPTION GROUPS
            List<string> SingleSelect = new();
            foreach ( var item in config.Rules.ReplacementGroups ) {
                foreach ( var item2 in item.Value ) {
                    SingleSelect.Add(item2);
                }
            }

            foreach ( var item in config.OptionGroups ) {
                string temp = SingleSelect.Contains(item.Id) ? "SINGLE_SELECT" : "MULTI_SELECT";
                OptionField tempSave = new() {
                    Id = item.Id,
                    Type = temp,
                    TypeNavigation = optionTypes.Where(o => o.Type == temp).First(),
                    Required = item.Required
                };
                optionFields.Add(tempSave);
            }

            _context.OptionFields.AddRange(optionFields);

            // OPTIONFIELD HAS OPTIONFIELD
            foreach ( var item in config.OptionSections ) {
                foreach ( var field in item.OptionGroupIds ) {
                    _context.OptionFieldsHasOptionFields.Add(
                        new OptionFieldsHasOptionField {
                            Base = item.Id,
                            BaseNavigation = optionFields.Where(of => of.Id == item.Id).First(),
                            OptionField = field,
                            OptionFieldNavigation = optionFields.Where(of => of.Id == field).First(),
                            DependencyType = "CHILD",
                            DependencyTypeNavigation = dependencyTypes.Where(d => d.Type == "CHILD").First()
                        }
                    );
                }
            }

            // OPTIONFIELD HAS PRODUCT
            foreach ( var item in config.OptionGroups ) {
                foreach ( var field in item.OptionIds ) {
                    _context.ProductsHasOptionFields.Add(
                        new ProductsHasOptionField {
                            ProductNumber = field,
                            ProductNumberNavigation = OptionProducts.Where(p => p.ProductNumber == field).First(),
                            OptionFields = item.Id,
                            OptionFieldsNavigation = optionFields.Where(o => o.Id == item.Id).First(),
                            DependencyType = "CHILD",
                            DependencyTypeNavigation = dependencyTypes.Where(d => d.Type == "CHILD").First()
                        }
                    );
                }
            }

            // PRODUCT HAS OPTIONFIELD
            foreach ( var item in config.OptionSections ) {
                _context.ProductsHasOptionFields.Add(
                    new ProductsHasOptionField {
                        ProductNumber = config.ConfigId,
                        ProductNumberNavigation = MainProduct,
                        OptionFields = item.Id,
                        OptionFieldsNavigation = optionFields.Where(o => o.Id == item.Id).First(),
                        DependencyType = "PARENT",
                        DependencyTypeNavigation = dependencyTypes.Where(d => d.Type == "PARENT").First()
                    }
                );
            }

            //IMAGES
            foreach ( var item in config.Images ) {
                _context.Pictures.Add(
                    new Picture {
                        ProductNumber = config.ConfigId,
                        ProductNumberNavigation = MainProduct,
                        Url = item
                    }
                );
            }

            var language = _context.ELanguages.Where(l => l.Language == lang).First();

            //LANGUAGE FOR PRODUCTS
            foreach ( var item in config.Options ) {
                _context.ProductHasLanguages.Add(
                    new ProductHasLanguage {
                        ProductNumber = item.Id,
                        ProductNumberNavigation = OptionProducts.Where(p => p.ProductNumber == item.Id).First(),
                        Language = lang,
                        LanguageNavigation = language,
                        Name = item.Name,
                        Description = item.Description
                    }
                );
            }

            //LANGUAGE FOR OPTIONFIELDS
            foreach ( var item in config.OptionGroups ) {
                _context.OptionFieldHasLanguages.Add(
                    new OptionFieldHasLanguage {
                        OptionFieldId = item.Id,
                        OptionField = optionFields.Where(o => o.Id == item.Id).First(),
                        Language = lang,
                        LanguageNavigation = language,
                        Name = item.Name,
                        Description = item.Description
                    }
                );
            }

            foreach ( var item in config.OptionSections ) {
                _context.OptionFieldHasLanguages.Add(
                    new OptionFieldHasLanguage {
                        OptionFieldId = item.Id,
                        OptionField = optionFields.Where(o => o.Id == item.Id).First(),
                        Language = lang,
                        LanguageNavigation = language,
                        Name = item.Name,
                        Description = ""
                    }
                );
            }

            _context.SaveChanges();

            // MODELS
            foreach ( var item in config.Rules.Models ) {
                _configurationService.SaveModels(MainProduct, item, lang);
            }

            _context.SaveChanges();

        }

        #endregion

        #region DELETE

        public void DeleteConfigurator( string productNumber ) {
            Configurator? configurator = GetConfiguratorByProductNumber(productNumber, "");

            if ( configurator == null )
                return;

            // PICTURES
            _context.Pictures.RemoveRange(
                from p in _context.Pictures
                where p.ProductNumber.Equals(configurator.ConfigId)
                select p
            );

            // LANGUAGE
            _context.ProductHasLanguages.RemoveRange(
                from pol in _context.ProductHasLanguages
                where pol.ProductNumber.Equals(configurator.ConfigId)
                select pol
            );

            // MODELS
            _context.Configurations.Where(c => c.ProductNumber.Equals(productNumber))
            .ToList().ForEach(item => _configurationService.DeleteConfiguration(item.Id));

            // OPTIONGROUPS/OPTIONSECTIONS
            List<string> removeOptionFieldID = new();
            foreach ( var item in configurator.OptionGroups ) {
                removeOptionFieldID.Add(item.Id);
            }
            foreach ( var item in configurator.OptionSections ) {
                removeOptionFieldID.Add(item.Id);
            }
            List<OptionField> removeOptionField = (
                from of in _context.OptionFields
                where removeOptionFieldID.Contains(of.Id)
                select of
            ).ToList();
            List<string> removeProductID = new();
            foreach ( var item in configurator.Options ) {
                removeProductID.Add(item.Id);
            }

            // OPTIONFIELDS
            foreach ( var item in removeOptionField ) {
                if ( !_context.ProductsHasOptionFields.Where(p => p.ProductNumber != configurator.ConfigId).Select(p => p.OptionFields).Contains(item.Id) ) {
                    List<OptionFieldsHasOptionField> temp = _context.OptionFieldsHasOptionFields.ToList();
                    foreach ( var ofo in temp ) {
                        if ( !removeOptionFieldID.Contains(ofo.Base) || !removeOptionFieldID.Contains(ofo.OptionField) )
                            removeOptionField.Remove(item);
                    }
                }
            }

            // OPTIONS
            foreach ( var item in removeProductID ) {
                DeleteConfigurator(item);
            }

            // RULES
            _context.ProductsHasProducts.RemoveRange(
                from php in _context.ProductsHasProducts
                where php.BaseProduct.Equals(configurator.ConfigId) || php.OptionProduct.Equals(configurator.ConfigId)
                select php
            );
            _context.ProductsHasOptionFields.RemoveRange(
                from pho in _context.ProductsHasOptionFields
                where pho.ProductNumber.Equals(configurator.ConfigId)
                select pho
            );
            _context.OptionFieldsHasOptionFields.RemoveRange(
                from ofho in _context.OptionFieldsHasOptionFields
                where removeOptionField.Select(rof => rof.Id).Contains(ofho.Base)
                    || removeOptionField.Select(rof => rof.Id).Contains(ofho.OptionField)
                select ofho
            );

            // REMOVE ALL OPTION FIELDS
            _context.OptionFields.RemoveRange(removeOptionField);

            // MAIN PRODUCT
            _context.Products.RemoveRange(
                _context.Products.Where(p => p.ProductNumber.Equals(configurator.ConfigId))
            );

            _context.SaveChanges();

        }

        #endregion

        #region PUT

        public void UpdateProduct( Configurator product, string lang ) {
            // Update Main Product
            Product MainProduct = _context.Products.Where(p => p.ProductNumber == product.ConfigId).First();
            MainProduct.Price = product.Rules.BasePrice;
            MainProduct = _context.Products.Update(MainProduct).Entity;

            //Updating Language
            var MainProductHasLanguage = (
                                            from p in _context.ProductHasLanguages
                                            where p.ProductNumber == product.ConfigId && p.Language == lang
                                            select p
                                        ).FirstOrDefault();
            if ( MainProductHasLanguage != null ) {
                MainProductHasLanguage.Name = product.Name;
                MainProductHasLanguage.Description = product.Description;
                _context.ProductHasLanguages.Update(MainProductHasLanguage);
            }
            else {
                _context.ProductHasLanguages.Add(new ProductHasLanguage {
                    ProductNumber = MainProduct.ProductNumber,
                    ProductNumberNavigation = MainProduct,
                    Language = lang,
                    LanguageNavigation = _context.ELanguages.Where(l => l.Language == lang).First(),
                    Name = product.Name,
                    Description = product.Description
                });
            }

            // Update Images
            _context.RemoveRange(_context.Pictures.Where(p => p.ProductNumber == product.ConfigId));
            foreach ( var item in product.Images ) {
                _context.Add(new Picture {
                    ProductNumber = MainProduct.ProductNumber,
                    ProductNumberNavigation = MainProduct,
                    Url = item
                }
                );
            }

            //------------------------------Deleting or Removing Dependencies of unused Options and Fields

            // Optionfields to Delete
            List<OptionField> fields = new();
            List<ProductsHasOptionField> pofDependency = (  // Option Sections
                from pof in _context.ProductsHasOptionFields
                where pof.ProductNumber == product.ConfigId
                && pof.DependencyType == "PARENT"
                && !product.OptionSections.Select(os => os.Id).Contains(pof.OptionFields)
                select pof
            ).ToList();
            fields.AddRange(pofDependency.Select(o => o.OptionFieldsNavigation).ToList());

            List<OptionFieldsHasOptionField> ofoDependency =    // Option Group
            (
                from ofo in _context.OptionFieldsHasOptionFields
                where fields.Select(f => f.Id).Contains(ofo.Base)
                select ofo
            ).ToList();
            fields.AddRange(ofoDependency.Select(o => o.OptionFieldNavigation).ToList());

            List<ProductsHasOptionField> temp = (   // Options
                from p in _context.ProductsHasOptionFields
                where p.DependencyType == "CHILD"
                && fields.Select(o => o.Id).Contains(p.OptionFields)
                select p
            ).ToList();

            List<Product> productToRemove = temp.Select(t => t.ProductNumberNavigation).Where(p => !product.Options.Select(o => o.Id).Contains(p.ProductNumber)).ToList();
            pofDependency.AddRange(temp);

            // Removing the Dependencies
            _context.ProductsHasOptionFields.RemoveRange(pofDependency);
            _context.OptionFieldsHasOptionFields.RemoveRange(ofoDependency);

            List<OptionField> toRemoveOptionField = new();
            foreach ( var item in fields ) {    // filter out the ones with dependencies
                if ( item.ConfigurationHasOptionFields.Count == 0
                    && item.ProductsHasOptionFields.Count == 0
                    && item.OptionFieldsHasOptionFieldOptionFieldNavigations.Count == 0
                    && item.OptionFieldsHasOptionFieldBaseNavigations.Count == 0 ) { toRemoveOptionField.Add(item); }
            }

            _context.OptionFieldHasLanguages.RemoveRange(_context.OptionFieldHasLanguages.Where(l => toRemoveOptionField.Select(o => o.Id).Contains(l.OptionFieldId)));

            List<Product> toRemoveProduct = new();
            foreach ( var item in productToRemove ) {    // filter out the ones with dependencies
                if ( item.Configurations.Count == 0
                     && item.ConfigurationHasOptionFields.Count == 0
                     && item.ProductsHasOptionFields.Count == 0 ) { toRemoveProduct.Add(item); }
            }

            // Remove Dependencies
            _context.ProductsHasProducts.RemoveRange(
                from p in _context.ProductsHasProducts
                where toRemoveProduct.Select(p => p.ProductNumber).Contains(p.BaseProduct)
                || toRemoveProduct.Select(p => p.ProductNumber).Contains(p.OptionProduct)
                select p
            );

            // remove images
            _context.Pictures.RemoveRange(
                from p in _context.Pictures
                where toRemoveProduct.Select(p => p.ProductNumber).Contains(p.ProductNumber)
                select p
            );

            // remove languages
            _context.ProductHasLanguages.RemoveRange(
                from p in _context.ProductHasLanguages
                where toRemoveProduct.Select(p => p.ProductNumber).Contains(p.ProductNumber)
                select p
            );

            // Remove Options
            _context.Products.RemoveRange(toRemoveProduct);
            // Remove OptionFields
            _context.OptionFields.RemoveRange(fields);


            // ------------------------------Inserting new Options and Fields and Updating older Options and Fields
            ELanguage language = _context.ELanguages.Where(l => l.Language == lang).First();
            List<EDependencyType> eDependencyTypes = _context.EDependencyTypes.ToList();

            // Updating existing Products
            List<ProductHasLanguage> dbProductHasLanguage = _context.ProductHasLanguages.ToList();

            List<Product> productsToUpdate = _context.Products.Where(p => product.Options.Select(o => o.Id).Contains(p.ProductNumber)).ToList();
            productsToUpdate.ForEach(p => {
                Option theOption = product.Options.Where(o => o.Id == p.ProductNumber).First();
                product.Rules.PriceList.TryGetValue(p.ProductNumber, out float price);
                p.Price = price;
                ProductHasLanguage? temp = dbProductHasLanguage.Where(l => p.ProductNumber == l.ProductNumber && l.Language == lang).FirstOrDefault();
                if ( temp != null ) {
                    temp.Name = theOption.Name;
                    temp.Description = theOption.Description;
                    _context.ProductHasLanguages.Update(temp);
                }
                else {
                    p.ProductHasLanguages.Add(
                        new ProductHasLanguage {
                            ProductNumber = p.ProductNumber,
                            ProductNumberNavigation = p,
                            Language = lang,
                            LanguageNavigation = language,
                            Name = theOption.Name,
                            Description = theOption.Description
                        }
                    );
                }
            });
            _context.Products.UpdateRange(productsToUpdate);

            // Inserting new Products
            List<Product> productsToInsert = new();
            foreach ( var item in product.Options.Where(p => !productsToUpdate.Select(t => t.ProductNumber).Contains(p.Id)) ) {
                product.Rules.PriceList.TryGetValue(item.Id, out float price);
                Product toAdd = new() {
                    ProductNumber = item.Id,
                    Price = price,
                    Buyable = false
                };
                toAdd.ProductHasLanguages.Add(
                    new ProductHasLanguage {
                        ProductNumber = toAdd.ProductNumber,
                        ProductNumberNavigation = toAdd,
                        Language = lang,
                        LanguageNavigation = language,
                        Name = item.Name,
                        Description = item.Description
                    }
                );
                productsToInsert.Add(toAdd);
            }
            _context.Products.AddRange(productsToInsert);

            List<Product> productOptions = new(); // One List for all Options
            productOptions.AddRange(productsToUpdate);
            productOptions.AddRange(productsToInsert);

            //---------------------Option Groups
            // Updating existing OptionFields
            List<OptionFieldHasLanguage> dbOFhasLanguage = _context.OptionFieldHasLanguages.Where(l => l.Language == lang).ToList();
            List<EOptionType> eOptionTypes = _context.EOptionTypes.ToList();

            List<OptionField> optionGroupToUpdate = _context.OptionFields.Where(o => product.OptionGroups.Select(o => o.Id).Contains(o.Id)).ToList();
            optionGroupToUpdate.ForEach(o => {
                OptionGroup temp = product.OptionGroups.Where(og => og.Id == o.Id).First();
                bool inDic = product.Rules.ReplacementGroups.ContainsKey(o.Id);
                o.Type = inDic ? "SINGLE_SELECT" : "MULTI_SELECT";
                o.TypeNavigation = eOptionTypes.Where(e => e.Type == o.Type).First();
                o.Required = temp.Required;
                OptionFieldHasLanguage? ofhl = dbOFhasLanguage.Where(dbofhl => dbofhl.OptionFieldId == o.Id).FirstOrDefault();
                if ( ofhl == null ) {
                    o.OptionFieldHasLanguages.Add(
                        new OptionFieldHasLanguage {
                            Language = lang,
                            LanguageNavigation = language,
                            OptionFieldId = o.Id,
                            OptionField = o,
                            Name = temp.Name,
                            Description = temp.Description
                        }
                    );
                }
                else {
                    ofhl.Name = temp.Name;
                    ofhl.Description = temp.Description;
                    _context.OptionFieldHasLanguages.Update(ofhl);
                }
                foreach ( var item in temp.OptionIds ) {
                    o.ProductsHasOptionFields.Add(new ProductsHasOptionField {
                        ProductNumber = item,
                        ProductNumberNavigation = productOptions.Where(p => p.ProductNumber == item).First(),
                        DependencyType = "CHILD",
                        DependencyTypeNavigation = eDependencyTypes.Where(d => d.Type == "CHILD").First(),
                        OptionFields = o.Id,
                        OptionFieldsNavigation = o
                    });
                }
            });
            _context.OptionFields.UpdateRange(optionGroupToUpdate);

            // Inserting new OptionFields
            List<OptionField> optionGroupToInsert = new();
            foreach ( var item in product.OptionGroups.Where(og => !optionGroupToUpdate.Select(of => of.Id).Contains(og.Id)) ) {
                string type = product.Rules.ReplacementGroups[item.Id].FirstOrDefault() == null ? "MULTI_SELECT" : "SINGLE_SELECT";
                OptionField toAdd = new() {
                    Id = item.Id,
                    Type = type,
                    TypeNavigation = eOptionTypes.Where(e => e.Type == type).First(),
                    Required = item.Required,
                };
                toAdd.OptionFieldHasLanguages.Add(new OptionFieldHasLanguage {
                    OptionFieldId = toAdd.Id,
                    OptionField = toAdd,
                    Language = lang,
                    LanguageNavigation = language,
                    Name = item.Name,
                    Description = item.Description
                });
                foreach ( var child in item.OptionIds ) {
                    toAdd.ProductsHasOptionFields.Add(new ProductsHasOptionField {
                        ProductNumber = child,
                        ProductNumberNavigation = productOptions.Where(p => p.ProductNumber == child).First(),
                        DependencyType = "CHILD",
                        DependencyTypeNavigation = eDependencyTypes.Where(d => d.Type == "CHILD").First(),
                        OptionFields = toAdd.Id,
                        OptionFieldsNavigation = toAdd
                    });
                }
                optionGroupToInsert.Add(toAdd);
            }
            _context.OptionFields.AddRange(optionGroupToInsert);

            List<OptionField> optionGroups = new();
            optionGroups.AddRange(optionGroupToUpdate);
            optionGroups.AddRange(optionGroupToInsert);

            //---------------------Option Sections
            // Updating existing OptionFields
            List<OptionField> optionSectionsToUpdate = _context.OptionFields.Where(o => product.OptionSections.Select(os => os.Id).Contains(o.Id)).ToList();
            optionSectionsToUpdate.ForEach(o => {
                OptionSection temp = product.OptionSections.Where(og => og.Id == o.Id).First();
                bool inDic = product.Rules.ReplacementGroups.ContainsKey(o.Id);
                o.Type = inDic ? "SINGLE_SELECT" : "MULTI_SELECT";
                o.TypeNavigation = eOptionTypes.Where(e => e.Type == o.Type).First();
                o.Required = false;
                OptionFieldHasLanguage? ofhl = dbOFhasLanguage.Where(dbofhl => dbofhl.OptionFieldId == o.Id).FirstOrDefault();
                if ( ofhl == null ) {
                    o.OptionFieldHasLanguages.Add(
                        new OptionFieldHasLanguage {
                            Language = lang,
                            LanguageNavigation = language,
                            OptionFieldId = o.Id,
                            OptionField = o,
                            Name = temp.Name,
                            Description = ""
                        }
                    );
                }
                else {
                    ofhl.Name = temp.Name;
                    ofhl.Description = "";
                    _context.OptionFieldHasLanguages.Update(ofhl);
                }
                foreach ( var item in temp.OptionGroupIds ) {
                    o.OptionFieldsHasOptionFieldBaseNavigations.Add(new OptionFieldsHasOptionField {
                        Base = o.Id,
                        BaseNavigation = o,
                        DependencyType = "CHILD",
                        DependencyTypeNavigation = eDependencyTypes.Where(d => d.Type == "CHILD").First(),
                        OptionField = item,
                        OptionFieldNavigation = optionGroups.Where(og => og.Id == item).First()
                    });
                }
            });
            _context.OptionFields.UpdateRange(optionSectionsToUpdate);

            // Inserting new OptionFields
            List<OptionField> optionSectionToInsert = new();
            foreach ( var item in product.OptionSections.Where(og => !optionSectionsToUpdate.Select(of => of.Id).Contains(og.Id)) ) {
                string type = product.Rules.ReplacementGroups[item.Id].FirstOrDefault() == null ? "MULTI_SELECT" : "SINGLE_SELECT";
                OptionField toAdd = new() {
                    Id = item.Id,
                    Type = type,
                    TypeNavigation = eOptionTypes.Where(e => e.Type == type).First(),
                    Required = false,
                };
                toAdd.OptionFieldHasLanguages.Add(new OptionFieldHasLanguage {
                    OptionFieldId = toAdd.Id,
                    OptionField = toAdd,
                    Language = lang,
                    LanguageNavigation = language,
                    Name = item.Name,
                    Description = ""
                });
                foreach ( var opt in item.OptionGroupIds ) {
                    toAdd.OptionFieldsHasOptionFieldBaseNavigations.Add(new OptionFieldsHasOptionField {
                        Base = toAdd.Id,
                        BaseNavigation = toAdd,
                        DependencyType = "CHILD",
                        DependencyTypeNavigation = eDependencyTypes.Where(d => d.Type == "CHILD").First(),
                        OptionField = opt,
                        OptionFieldNavigation = optionGroups.Where(og => og.Id == opt).First()
                    });
                }
                optionGroupToInsert.Add(toAdd);
            }
            _context.OptionFields.AddRange(optionSectionToInsert);

            List<OptionField> optionSections = new();
            optionSections.AddRange(optionSectionsToUpdate);
            optionSections.AddRange(optionSectionToInsert);

            List<OptionField> optionFields = new();
            optionFields.AddRange(optionGroups);
            optionFields.AddRange(optionSections);

            //Deleting Rules
            _context.RemoveRange(
                _context.ProductsHasProducts.Where(p =>
                    productOptions.Select(po => po.ProductNumber).Contains(p.BaseProduct)
                    || productOptions.Select(po => po.ProductNumber).Contains(p.OptionProduct)
                ).ToList()
            );
            _context.RemoveRange(
                _context.ProductsHasOptionFields.Where(p =>
                   productOptions.Select(po => po.ProductNumber).Contains(p.ProductNumber)
                   || optionFields.Select(o => o.Id).Contains(p.OptionFields)
                )
            );
            _context.RemoveRange(
                _context.OptionFieldsHasOptionFields.Where(p =>
                   optionFields.Select(o => o.Id).Contains(p.Base)
                   || optionFields.Select(o => o.Id).Contains(p.OptionField)
                )
            );

            // Creating dependencies from Main Product to OptionSections
            List<ProductsHasOptionField> ProductToOptionSection = new();
            optionSections.ForEach(os => {
                ProductToOptionSection.Add(
                    new ProductsHasOptionField {
                        ProductNumber = MainProduct.ProductNumber,
                        ProductNumberNavigation = MainProduct,
                        OptionFields = os.Id,
                        OptionFieldsNavigation = os,
                        DependencyType = "PARENT",
                        DependencyTypeNavigation = eDependencyTypes.Where(d => d.Type == "PARENT").First()
                    }
                );
            });
            _context.ProductsHasOptionFields.AddRange(ProductToOptionSection);

            //-------------------------RULES

            List<ProductsHasProduct> productDependencies = new();
            // Requirements
            foreach ( var baseP in product.Rules.Requirements.Keys ) {
                foreach ( var optionP in product.Rules.Requirements[baseP] ) {
                    productDependencies.Add(new ProductsHasProduct {
                        BaseProduct = baseP,
                        BaseProductNavigation = productOptions.Where(p => p.ProductNumber == baseP).First(),
                        OptionProduct = optionP,
                        OptionProductNavigation = productOptions.Where(p => p.ProductNumber == optionP).First(),
                        DependencyType = "REQUIRED",
                        DependencyTypeNavigation = eDependencyTypes.Where(d => d.Type == "REQUIRED").First()
                    });
                }
            }
            // Incompatibilities
            foreach ( var baseP in product.Rules.Incompatibilities.Keys ) {
                foreach ( var optionP in product.Rules.Incompatibilities[baseP] ) {
                    productDependencies.Add(new ProductsHasProduct {
                        BaseProduct = baseP,
                        BaseProductNavigation = productOptions.Where(p => p.ProductNumber == baseP).First(),
                        OptionProduct = optionP,
                        OptionProductNavigation = productOptions.Where(p => p.ProductNumber == optionP).First(),
                        DependencyType = "EXCLUDING",
                        DependencyTypeNavigation = eDependencyTypes.Where(d => d.Type == "EXCLUDING").First()
                    });
                }
            }
            _context.ProductsHasProducts.AddRange(productDependencies);

            List<OptionFieldsHasOptionField> optionFieldDependencies = new();
            // GroupRequirements
            foreach ( var baseOF in product.Rules.GroupRequirements.Keys ) {
                foreach ( var optionOF in product.Rules.GroupRequirements[baseOF] ) {
                    optionFieldDependencies.Add(new OptionFieldsHasOptionField {
                        Base = baseOF,
                        BaseNavigation = optionFields.Where(p => p.Id == baseOF).First(),
                        OptionField = optionOF,
                        OptionFieldNavigation = optionFields.Where(p => p.Id == optionOF).First(),
                        DependencyType = "REQUIRED",
                        DependencyTypeNavigation = eDependencyTypes.Where(d => d.Type == "REQUIRED").First()
                    });
                }
            }
            _context.OptionFieldsHasOptionFields.AddRange(optionFieldDependencies);

            _context.SaveChanges();

            // Update Models
            List<Configuration> configs = _context.Configurations.Where(c => c.ProductNumber == MainProduct.ProductNumber && c.AccountId == null).ToList();
            List<ProductSaveExtended> models = _configurationService.GetConfigurations(MainProduct.ProductNumber).Where(sp => sp.ConfigId == MainProduct.ProductNumber).ToList();
            models.ForEach(m => {
                if ( product.Rules.Models.Select(rm => rm.Id).Contains(m.Name) ) {
                    _configurationService.UpdateConfiguration(m, lang, m.Name);
                }
                else {
                    Configuration? temp = configs.Where(c => c.ConfigurationsHasLanguages.Where(l => l.Language == lang).FirstOrDefault() != null
                    && c.ConfigurationsHasLanguages.Where(l => l.Language == lang).First().Name == m.Name).FirstOrDefault();
                    if ( temp != null )
                        _configurationService.DeleteConfiguration(temp.Id);
                }

            });
            foreach ( var item in product.Rules.Models.Where(m => !models.Select(m => m.Name).Contains(m.Id)).ToArray() ) {
                _configurationService.SaveModels(MainProduct, item, lang);
            }

            _context.SaveChanges();
        }

        #endregion

    }
}