﻿using BackendProductConfigurator.App_Code;
using BackendProductConfigurator.Validation.JWT.Managers;
using Model;
using Model.Enumerators;
using Model.Indexes;
using Model.Interfaces;
using System.Security.Claims;
using System.Text;

namespace BackendProductConfigurator.Controllers
{
    public static class ValuesClass
    {
        public static Dictionary<string, List<Configurator>> Configurators { get; set; } = new Dictionary<string, List<Configurator>>() { { "de", new List<Configurator>() }, { "en", new List<Configurator>() }, { "fr", new List<Configurator>() } };
        public static Dictionary<string, List<ConfiguredProduct>> ConfiguredProducts { get; set; } = new Dictionary<string, List<ConfiguredProduct>>() { { "de", new List<ConfiguredProduct>() }, { "en", new List<ConfiguredProduct>() }, { "fr", new List<ConfiguredProduct>() } };
        public static Dictionary<string, List<ProductSaveExtended>> SavedProducts { get; set; } = new Dictionary<string, List<ProductSaveExtended>>() { { "de", new List<ProductSaveExtended>() }, { "en", new List<ProductSaveExtended>() }, { "fr", new List<ProductSaveExtended>() } };
        public static Dictionary<string, List<Account>> Accounts { get; set; } = new Dictionary<string, List<Account>>() { { "de", new List<Account>() }, { "en", new List<Account>() }, { "fr", new List<Account>() } };

        private static readonly List<string> languages = new List<string>() { "de", "en", "fr" };

        private static readonly Dictionary<Type, string> typeApis = new Dictionary<Type, string>
        {
            {typeof(ProductSaveExtended), "/db/configuration"},
            {typeof(Configurator), "/db/product" }
        };

        public static void SetValues()
        {
            switch(GlobalValues.ValueMode)
            {
                case EValueMode.TestValues:
                    SetStaticValues();
                    break;
                case EValueMode.DatabaseValues:
                    SetDBValues();
                    break;
            }
        }
        public static void PostValue<T>(T value, string language) where T : class
        {
            if(GlobalValues.ValueMode == EValueMode.DatabaseValues)
                ADBAccess<T>.PostValue(language, GlobalValues.ServerAddress, typeApis[typeof(T)], value).Wait();
        }
        public static void PutValue<T>(T value, string language) where T : class
        {
            if (GlobalValues.ValueMode == EValueMode.DatabaseValues)
                ADBAccess<T>.PutValue(language, GlobalValues.ServerAddress, typeApis[typeof(T)], value).Wait();
        }
        public static async void DeleteValue<T>(string language, T identifier) where T : class
        {
            if (GlobalValues.ValueMode == EValueMode.DatabaseValues)
                await ADBAccess<T>.DeleteValue(language, GlobalValues.ServerAddress, typeApis[typeof(T)], identifier);
        }
        public static void SetDBValues()
        {
            foreach(string language in languages)
            {
                try
                {
                    Configurators[language] = ADBAccess<Configurator>.GetValues(language, GlobalValues.ServerAddress, typeApis[typeof(Configurator)]).Result;

                    SavedProducts[language] = ADBAccess<ProductSaveExtended>.GetValues(language, GlobalValues.ServerAddress, typeApis[typeof(ProductSaveExtended)]).Result;
                }
                catch { }
            }
        }

        public static void SetStaticValues()
        {
            List<Option> options = new List<Option> {
                                                      new Option()
                                                      {
                                                          Id = "D150",
                                                          Name = "D150",
                                                          Description = "Fetter Diesel Motor"
                                                      },
                                                      new Option()
                                                      {
                                                          Id = "D200",
                                                          Name = "D200",
                                                          Description = "Fetter Diesel Motor"
                                                      },
                                                      new Option()
                                                      {
                                                          Id = "D250",
                                                          Name = "D250",
                                                          Description = "Fetter Diesel Motor"
                                                      },
                                                      new Option()
                                                      {
                                                          Id = "RED",
                                                          Name = "Alfa Rosso",
                                                          Description = "Red like a cherry"
                                                      },
                                                      new Option()
                                                      {
                                                          Id = "GREEN",
                                                          Name = "Green demon",
                                                          Description = "Green like the grinch"
                                                      }};

            List<string> productImages = new List<string> { "./Alfa_159_grey.jpg" };

            List<OptionGroup> optionGroups = new List<OptionGroup>
            {
                new OptionGroup() { Id = "COLOR_GROUP", Name = "Color", Description = "What color you want", OptionIds = new List<string>(){ "RED", "WHITE", "GREEN"}, Required = true },
                new OptionGroup() { Id = "MOTORTYPE_GROUP", Name = "A motor fuel type", Description = "What motor fuel", OptionIds = new List<string>(){ "DIESEL", "PETROL"}, Required = true },
                new OptionGroup() { Id = "MOTOR_GROUP", Name = "A motor power", Description = "The motor power", OptionIds = new List<string>(){ "D150", "D200", "D250"}, Required = true }
            };

            List<OptionSection> optionSections = new List<OptionSection>
            {
                new OptionSection() { Name = "Exterior", Id = "EXTERIOR", OptionGroupIds = new List<string> { "COLOR_GROUP" }},
                new OptionSection() { Name = "Motor", Id = "MOTOR_SECTION", OptionGroupIds = new List<string> { "MOTORTYPE_GROUP", "MOTOR_GROUP" }}
            };

            RulesExtended productDependencies = new RulesExtended()
            {
                BasePrice = 50000f,
                Models = new List<ModelType> { new ModelType { Id = "TI", Description = "Sportliche Version", OptionIds = new List<string>() { "RED", "DIESEL", "D150" } } },
                DefaultModel = "TI",
                ReplacementGroups = new Dictionary<string, List<string>> { { "COLOR_GROUP", new List<string> { "ey", "wos" } } },
                Requirements = new Dictionary<string, List<string>> { { "D150", new List<string> { "DIESEL" } } },
                Incompatibilities = new Dictionary<string, List<string>> { { "D150", new List<string> { "PETROL" } } },
                GroupRequirements = new Dictionary<string, List<string>> { { "PANORAMATYPE_GROUP", new List<string> { "PANORAMAROOF" } } },
                PriceList = new Dictionary<string, float> { { "D150", 1500f },
                                                { "RED", 250f },
                                                { "DIESEL", 150f} }
            };

            Configurators["de"].Add(new Configurator()
            {
                ConfigId = "Alfa",
                Name = "Neuer Konfigurator",
                Description = "Sehr cool",
                Images = productImages,
                Rules = productDependencies,
                OptionGroups = optionGroups,
                Options = options,
                OptionSections = optionSections}
            );

            List<Option> optionsList = new List<Option>
            {
                new Option()
                {
                    Id = "Option1",
                    Name = "Erste Option",
                    Description = "Ka Ahnung wos des duat"
                }
            };

            ConfiguredProduct p1 = new ConfiguredProduct()
            {
                ConfigurationName = "Fetter Benz",
                Options = optionsList.Select(x => x.Id).ToList(),
                Price=4.2f
            };

            ConfiguredProduct p2 = new ConfiguredProduct()
            {
                ConfigurationName = "Eleganter Alfa Romeo",
                Options = optionsList.Select(x => x.Id).ToList(),
                Price = 9.65f
            };

            ConfiguredProduct p3 = new ConfiguredProduct()
            {
                ConfigurationName = "Fetterer Benz",
                Options = optionsList.Select(x => x.Id).ToList(),
                Price = 0.8f
            };

            ConfiguredProducts["de"] = new List<ConfiguredProduct> { p1, p2, p3 };

            Account acc1 = new Account()
            {
                UserName = "TEST-FUCHS GmbH",
                UserEmail = "huh@what.com"
            };

            Account acc2 = new Account()
            {
                UserName = "HTL Krems",
                UserEmail = "huh@what.com"
            };

            Account acc3 = new Account()
            {
                UserName = "AVIA Station Gmünd",
                UserEmail = "huh@what.com"
            };

            Accounts["de"] = new List<Account> { acc1, acc2, acc3 };

            ProductSaveExtended psave1 = new ProductSaveExtended()
            { 
                Description = "Saved product",
                Name = "Alfa 147",
                Options = options.Select(x => x.Id).Cast<string>().ToList(),
                SavedName = "Scherzermobil",
                Status = EStatus.saved.ToString(),
                User = acc1,
                ConfigId = "ALFA1"
            };
            ProductSaveExtended psave2 = new ProductSaveExtended()
            {
                Description = "Saved product",
                Name = "Alfa 156",
                Options = options.Select(x => x.Id).Cast<string>().ToList(),
                SavedName = "Pömmermobil",
                Status = EStatus.saved.ToString(),
                User = acc2,
                ConfigId = "ALFA"
            };
            ProductSaveExtended psave3 = new ProductSaveExtended()
            {
                Description = "Saved product",
                Name = "Alfa 166",
                Options = options.Select(x => x.Id).Cast<string>().ToList(),
                SavedName = "Leutgeb Toyota",
                Status = EStatus.saved.ToString(),
                User = acc3,
                ConfigId = "BENZ1"
            };
            SavedProducts["de"] = new List<ProductSaveExtended> { psave1, psave2, psave3 };
        }

        public static Account FillAccountFromToken(string bearerToken)
        {
            Account account = new Account() { UserName = "testUser", UserEmail = "test@user.com", IsAdmin = true };
            //JWTService jWTService = new JWTService("c2plaDkzdWhBVWhpdW9zZGg5ODhob2lBdWgz");

            //bearerToken = bearerToken.Replace("Bearer ", "");

            //foreach(Claim claim in jWTService.GetTokenClaims(bearerToken))
            //{
            //    switch(claim.Type)
            //    {
            //        case ClaimTypes.Name:
            //            account.UserName = claim.Value;
            //            break;
            //        case ClaimTypes.Email:
            //            account.UserEmail = claim.Value;
            //            break;
            //        case "admin":
            //            account.IsAdmin = Convert.ToBoolean(claim.Value);
            //            break;
            //    }
            //}

            return account;
        }
        public static Dictionary<string, Configurator> GenerateConfigurator(ConfiguratorPost configuratorPost)
        {
            Dictionary<string, Configurator> configs = new Dictionary<string, Configurator>();

            foreach(KeyValuePair<string, LanguageVariant> languageDict in configuratorPost.Languages)
            {
                Configurator temp = new Configurator()
                {
                    ConfigId = GenerateConfigId(configuratorPost.Languages["en"]),
                    Images = configuratorPost.Images,
                    Rules = configuratorPost.Rules.ConvertToExtended(),
                    Name = languageDict.Value.Name,
                    Description = languageDict.Value.Description,
                    OptionGroups = GetConfiguratorValues<OptionGroup, OptionGroupIndex, DescribedIndex>(configuratorPost.OptionGroups, languageDict.Value, languageDict.Value.OptionGroups),
                    Options = GetConfiguratorValues<Option, IIndexable, Option>(configuratorPost.Options.Cast<IIndexable>().ToList(), languageDict.Value, languageDict.Value.Options),
                    OptionSections = GetConfiguratorValues<OptionSection, LanguageIndexGroup, NamedIndex>(configuratorPost.OptionSections, languageDict.Value, languageDict.Value.OptionSections)
                };
                temp.Rules.Models = GetConfiguratorValues<ModelType, LanguageIndexOption, DescribedIndex>(configuratorPost.Rules.Models, languageDict.Value, languageDict.Value.Models);
                temp.Rules.ReplacementGroups = FillReplacementGroups(configuratorPost.OptionGroups);

                configs.Add(languageDict.Key, temp);
            }

            return configs;
        }
        private static Dictionary<string, List<string>> FillReplacementGroups(List<OptionGroupIndex> optionGroups)
        {
            Dictionary<string, List<string>> dictionary = new Dictionary<string, List<string>>();
            foreach(OptionGroupIndex optionGroup in optionGroups)
            {
                if(optionGroup.Replacement)
                {
                    dictionary.Add(optionGroup.Id, optionGroup.OptionIds);
                }
            }
            return dictionary;
        }
        private static List<T> GetConfiguratorValues<T, K, L>(List<K> startElements, LanguageVariant languageVariant, List<L> languageList) where L : IIndexable where K : IIndexable where T : class, new()
        {
            List<T> elements = new List<T>();

            foreach(K element in startElements)
            {
                L currentElement = languageList.Where(x => x.Id == element.Id).First();
                if (typeof(T) == typeof(OptionSection))
                    elements.Add(GenerateValues(element as LanguageIndexGroup, currentElement as NamedIndex) as T);
                else if (typeof(T) == typeof(OptionGroup))
                    elements.Add(GenerateValues(element as OptionGroupIndex, currentElement as DescribedIndex) as T);
                else if (typeof(T) == typeof(Option))
                    elements.Add(GenerateValues(element as IIndexable, currentElement as Option) as T);
                else if (typeof(T) == typeof(ModelType))
                    elements.Add(GenerateValues(element as LanguageIndexOption, currentElement as DescribedIndex) as T);
            }

            return elements;
        }
        private static OptionSection GenerateValues(LanguageIndexGroup loopElement, NamedIndex currentElement)
        {
            return new OptionSection()
            {
                Id = loopElement.Id,
                Name = currentElement.Name,
                OptionGroupIds = loopElement.OptionGroupIds
            };
        }
        private static OptionGroup GenerateValues(OptionGroupIndex loopElement, DescribedIndex currentElement)
        {
            return new OptionGroup()
            {
                Id = loopElement.Id,
                Name = currentElement.Name,
                Description = currentElement.Description,
                OptionIds = loopElement.OptionIds,
                Required = loopElement.Required
            };
        }
        private static Option GenerateValues(IIndexable loopElement, Option currentElement)
        {
            return currentElement;
        }
        private static ModelType GenerateValues(LanguageIndexOption loopElement, DescribedIndex currentElement)
        {
            return new ModelType()
            {
                Id = loopElement.Id,
                OptionIds = loopElement.Options,
                Name = currentElement.Name,
                Description = currentElement.Description
            };
        }
        private static List<OptionGroup> AdaptOptionGroup(Configurator configurator)
        {
            foreach (OptionGroup og in configurator.OptionGroups)
            {
                og.OptionIds.ForEach(optionId => optionId = $"{optionId}_{configurator.ConfigId}");
            }

            return configurator.OptionGroups;
        }
        private static string GenerateConfigId(LanguageVariant languageVariant)
        {
            StringBuilder sb = new StringBuilder(languageVariant.Name);
            List<string> configIds = new List<string>();
            foreach (string language in languages)
            {
                configIds.AddRange(ValuesClass.Configurators[language].Select(x => x.ConfigId).ToList());
            }

            sb.Replace(' ', '_');

            int i = 1;
            while (configIds.Contains(sb.ToString()))
            {
                if (sb.ToString().Contains('#'))
                    sb.Remove(sb.ToString().IndexOf('#'), 5);
                sb.Append('#').Append(i++.ToString().PadLeft(4, '0'));
            }

            return sb.ToString();
        }
        public static Configurator AdaptConfiguratorsOptionIds(Configurator configurator)
        {
            foreach(Option option in configurator.Options)
            {
                option.Id += $"+{configurator.ConfigId}";
            }
            foreach(LanguageIndex li in configurator.OptionGroups)
            {
                li.Id += $"+{configurator.ConfigId}";
                li.OptionIds = li.OptionIds.Select(x => x += $"+{configurator.ConfigId}").ToList();
            }
            foreach(OptionSection os in configurator.OptionSections)
            {
                os.Id += $"+{configurator.ConfigId}";
                os.OptionGroupIds = os.OptionGroupIds.Select(x => x += $"+{configurator.ConfigId}").ToList();
            }
            foreach(LanguageIndex li in configurator.Rules.Models)
            {
                li.Id += $"+{configurator.ConfigId}";
                li.OptionIds = li.OptionIds.Select(x => x += $"+{configurator.ConfigId}").ToList();
            }
            
            configurator.Rules.ReplacementGroups = AdaptIdsInDictionarys(configurator.Rules.ReplacementGroups, configurator.ConfigId);
            configurator.Rules.Requirements = AdaptIdsInDictionarys(configurator.Rules.Requirements, configurator.ConfigId);
            configurator.Rules.Incompatibilities = AdaptIdsInDictionarys(configurator.Rules.Incompatibilities, configurator.ConfigId);
            configurator.Rules.GroupRequirements = AdaptIdsInDictionarys(configurator.Rules.GroupRequirements, configurator.ConfigId);

            configurator.Rules.PriceList = AdaptIdsInDictionarys(configurator.Rules.PriceList, configurator.ConfigId);
            
            return configurator;
        }
        private static Dictionary<string, List<string>> AdaptIdsInDictionarys(Dictionary<string, List<string>> dictionary, string configId)
        {
            Dictionary<string, List<string>> temp = new Dictionary<string, List<string>>();
            foreach (KeyValuePair<string, List<string>> dic in dictionary)
            {
                temp.Add($"{dic.Key}+{configId}", dic.Value.Select(x => x += $"+{configId}").ToList());
            }
            return temp;
        }
        private static Dictionary<string, float> AdaptIdsInDictionarys(Dictionary<string, float> dictionary, string configId)
        {
            Dictionary<string, float> temp = new Dictionary<string, float>();
            foreach (KeyValuePair<string, float> dic in dictionary)
            {
                temp.Add($"{dic.Key}+{configId}", dic.Value);
            }
            return temp;
        }
    }
}