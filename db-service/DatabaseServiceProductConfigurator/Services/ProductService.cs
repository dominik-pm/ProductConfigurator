using DatabaseServiceProductConfigurator.Context;
using DatabaseServiceProductConfigurator.Models;
using Microsoft.EntityFrameworkCore;
using Model;
using System.Text.Json.Serialization;

namespace DatabaseServiceProductConfigurator.Services {

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

            List<Product> products = _context.Products.ToList();

            List<Configurator> temp = new();
            products.ForEach(
                p => {
                    var depen = new Rules { BasePrice = p.Price };
                    var infos = _languageService.GetProductWithLanguage(p.ProductNumber, lang);
                    temp.Add(new Configurator {
                        ConfigId = p.ProductNumber,
                        Name = infos.Name,
                        Description = infos.Description,
                        Images = ( from pic in _context.Pictures where pic.ProductNumber == p.ProductNumber select pic.Url ).ToList(),
                        Options = GetOptionsByProductNumber(p.ProductNumber, lang),
                        OptionGroups = GetOptionGroupsByProductNumber(p.ProductNumber, lang),
                        OptionSections = GetOptionSectionByProductNumber(p.ProductNumber, lang),
                        Rules = depen,
                    });
                }
            );

            foreach ( var item in temp ) {
                item.Options.ForEach(o => item.Rules = _ruleService.ExtendProductDependencies(item.Rules, o.Id));
                item.OptionGroups.ForEach(o => item.Rules = _ruleService.ExtendProductDependenciesByOptionField(item.Rules, o.Id));
                item.OptionSections.ForEach(o => item.Rules = _ruleService.ExtendProductDependenciesByOptionField(item.Rules, o.Id));
                item.Rules.Models.AddRange(_configurationService.GetModelsByProduct(item.ConfigId, lang));
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

        private List<OptionSection> GetOptionSectionByProductNumber( string productNumber, string lang ) {

            List<OptionSection> sections = new();

            List<OptionField> rawFields = new();
            List<OptionField> cookedFields = new();

            rawFields.AddRange(
                (
                    from of in _context.ProductsHasOptionFields
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
                                from ofo in _context.OptionFieldsHasOptionFields
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
                    from ofo in _context.OptionFieldsHasOptionFields
                    where ofo.Base == field.Id && ofo.DependencyType == "CHILD"
                    select ofo.OptionField.ToString()
                ).ToList();

                InfoStruct fieldinfos = _languageService.GetOptionsfieldWithLanguage(field.Id, lang);

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

        private List<OptionGroup> GetOptionGroupsByProductNumber( string productNumber, string lang ) {

            List<OptionGroup> optionGroups = new();

            List<OptionField> rawFields = new();
            List<OptionField> cookedFields = new();

            rawFields.AddRange(
                (
                    from of in _context.ProductsHasOptionFields
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
                                from ofo in _context.OptionFieldsHasOptionFields
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
                    from pof in _context.ProductsHasOptionFields
                    where pof.OptionFields == field.Id && pof.DependencyType == "CHILD"
                    let infos = _languageService.GetProductWithLanguage(pof.ProductNumber, lang)
                    select pof.ProductNumber
                ).ToList();

                InfoStruct fieldinfos = _languageService.GetOptionsfieldWithLanguage(field.Id, lang);

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

        private List<Option> GetOptionsByProductNumber( string productNumber, string lang ) {

            List<Option> options = new();

            List<OptionField> fields = (
                from pof in _context.ProductsHasOptionFields
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
                            from of in _context.OptionFieldsHasOptionFields
                            where of.Base == item.Id && !fields.Select(p => p.Id).Contains(of.OptionField)
                            select of.OptionFieldNavigation
                        ).ToList()
                    );
                }
                fields.AddRange(toAdd);
            } while ( currentCount != fields.Count );

            foreach ( var item in fields ) {
                List<ProductsHasOptionField> temp = (
                    from opt in _context.ProductsHasOptionFields
                    where opt.OptionFields == item.Id && opt.DependencyType == "CHILD"
                    select opt
                ).ToList();
                List<Option> toAdd = new();
                temp.ForEach(
                    opt => {
                        var infos = _languageService.GetProductWithLanguage(opt.ProductNumber, lang);
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
            _context.Products.Add(
                new Product {
                    ProductNumber = config.ConfigId,
                    Price = config.Rules.BasePrice,
                    Buyable = true
                }
            );

            // OPTION PRODUCTS
            foreach ( var item in config.Options ) {
                bool priceAvailable = config.Rules.PriceList.TryGetValue(item.Id, out float price);

                _context.Products.Add(
                    new Product {
                        ProductNumber = item.Id,
                        Price = priceAvailable ? price : 0,
                        Buyable = false
                    }
                );
            }

            // REQUIREMENTS
            foreach ( var item in config.Rules.Requirements ) {
                foreach ( var reqitem in item.Value ) {
                    _context.ProductsHasProducts.Add(
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
                    _context.ProductsHasProducts.Add(
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
                _context.OptionFields.Add(tempSave);
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
                _context.OptionFields.Add(tempSave);
            }

            // OPTIONFIELD HAS OPTIONFIELD
            foreach ( var item in config.OptionSections ) {
                foreach ( var field in item.OptionGroupIds ) {
                    _context.OptionFieldsHasOptionFields.Add(
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
                    _context.ProductsHasOptionFields.Add(
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
                _context.ProductsHasOptionFields.Add(
                    new ProductsHasOptionField {
                        ProductNumber = config.ConfigId,
                        OptionFields = item.Id,
                        DependencyType = "PARENT"
                    }
                );
            }

            //IMAGES
            foreach ( var item in config.Images ) {
                _context.Pictures.Add(
                    new Picture {
                        ProductNumber = config.ConfigId,
                        Url = item
                    }
                );
            }

            //LANGUAGE FOR PRODUCTS
            foreach ( var item in config.Options ) {
                _context.ProductHasLanguages.Add(
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
                _context.OptionFieldHasLanguages.Add(
                    new OptionFieldHasLanguage {
                        OptionFieldId = item.Id,
                        Language = lang,
                        Name = item.Name,
                        Description = item.Description
                    }
                );
            }

            foreach ( var item in config.OptionSections ) {
                _context.OptionFieldHasLanguages.Add(
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
                _configurationService.SaveModels(config.ConfigId, item, lang);
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

        public void UpdateProduct(Configurator product) {

        }

        #endregion

    }
}