using BackendProductConfigurator.App_Code;
using BackendProductConfigurator.Validation.JWT.Managers;
using BackendProductConfigurator.Validation.JWT.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Model;
using Model.Enumerators;
using Model.Indexes;
using Model.Interfaces;
using Model.Wrapper;
using System.Security.Claims;
using System.Text;

namespace BackendProductConfigurator.Controllers
{
    public static class ValuesClass
    {
        public static DateTime LastDBFetch;
        public static Dictionary<string, List<Configurator>> Configurators { get; set; } = new Dictionary<string, List<Configurator>>() { { "de", new List<Configurator>() }, { "en", new List<Configurator>() }, { "fr", new List<Configurator>() } };
        public static Dictionary<string, List<ConfiguredProduct>> ConfiguredProducts { get; set; } = new Dictionary<string, List<ConfiguredProduct>>() { { "de", new List<ConfiguredProduct>() }, { "en", new List<ConfiguredProduct>() }, { "fr", new List<ConfiguredProduct>() } };
        public static Dictionary<string, List<ProductSaveExtended>> SavedProducts { get; set; } = new Dictionary<string, List<ProductSaveExtended>>() { { "de", new List<ProductSaveExtended>() }, { "en", new List<ProductSaveExtended>() }, { "fr", new List<ProductSaveExtended>() } };
        public static Dictionary<string, List<Account>> Accounts { get; set; } = new Dictionary<string, List<Account>>() { { "de", new List<Account>() }, { "en", new List<Account>() }, { "fr", new List<Account>() } };

        private static readonly List<string> languages = new List<string>() { "de", "en", "fr" };

        private static readonly Dictionary<Type, string> typeApis = new Dictionary<Type, string>
        {
            {typeof(ProductSaveExtended), "/db/configuration"},
            {typeof(Configurator), "/db/product" },
            {typeof(ConfigurationDeleteWrapper), "/db/product" }
        };

        public static void SetValues()
        {
            switch(GlobalValues.ValueMode)
            {
                case EValueMode.TestValues:
                    SetStaticValues();
                    break;
                case EValueMode.DatabaseValues:
                    Task task = new Task(SetDBValues);
                    task.Start();
                    task.Wait();
                    break;
            }
        }
        public static void PostValue<T>(T value, string language) where T : class
        {
            try
            {
                if (GlobalValues.ValueMode == EValueMode.DatabaseValues)
                    ADBAccess<T>.PostValue(language, GlobalValues.ServerAddress, typeApis[typeof(T)], value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void PutValue<T>(T value, string language) where T : class
        {
            try
            {
                if (GlobalValues.ValueMode == EValueMode.DatabaseValues)
                    ADBAccess<T>.PutValue(language, GlobalValues.ServerAddress, typeApis[typeof(T)], value);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void DeleteValue<T>(string language, T identifier) where T : class
        {
            try
            {
                if (GlobalValues.ValueMode == EValueMode.DatabaseValues)
                    ADBAccess<T>.DeleteValue(language, GlobalValues.ServerAddress, typeApis[typeof(T)], identifier);
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }
        public static void SetDBValues()
        {
            List<Task> tasks = new List<Task>();
            Task temp;
            foreach(string language in languages)
            {
                temp = Task.Factory.StartNew(new Action<object?>((str) =>
                {
                    try
                    {
                        string taskLanguage = str as string;
                        Task<List<Configurator>> t = ADBAccess<Configurator>.GetValues(taskLanguage, GlobalValues.ServerAddress, typeApis[typeof(Configurator)]);
                        Configurators[taskLanguage] = t.Wait(GlobalValues.TimeOut) ? t.Result : new List<Configurator>();
                    }
                    catch { }
                }), language);
                tasks.Add(temp);
                temp = Task.Factory.StartNew(new Action<object?>((str) =>
                {
                    try
                    {
                        string taskLanguage = str as string;
                        Task<List<ProductSaveExtended>> t = ADBAccess<ProductSaveExtended>.GetValues(taskLanguage, GlobalValues.ServerAddress, typeApis[typeof(ProductSaveExtended)]);
                        SavedProducts[taskLanguage] = t.Wait(GlobalValues.TimeOut) ? t.Result : new List<ProductSaveExtended>();
                    }
                    catch { }
                }), language);
                tasks.Add(temp);
            }
            try
            {
                Task.WaitAll(tasks.ToArray());
            }
            catch (Exception ex)
            {
                throw ex;
            }
        }

        public static void SetStaticValues()
        {
            List<Option> options = new List<Option> {
                                                      new Option()
                                                      {
                                                          Id = "D150",
                                                          Name = "D150",
                                                          Description = "Mittlestarker Diesel Motor",
                                                          ProductNumber = "D150"
                                                      },
                                                      new Option()
                                                      {
                                                          Id = "DIESEL",
                                                          Name = "Diesel",
                                                          Description = "Diesel Treibstoff",
                                                          ProductNumber = "DIESEL"
                                                      },
                                                      new Option()
                                                      {
                                                          Id = "PETROL",
                                                          Name = "Benziner",
                                                          Description = "Benzin",
                                                          ProductNumber = "PETROL"
                                                      },
                                                      new Option()
                                                      {
                                                          Id = "D200",
                                                          Name = "D200",
                                                          Description = "Starker Diesel Motor",
                                                          ProductNumber = "D200"
                                                      },
                                                      new Option()
                                                      {
                                                          Id = "D250",
                                                          Name = "D250",
                                                          Description = "Sehr starker Diesel Motor",
                                                          ProductNumber = "D250"
                                                      },
                                                      new Option()
                                                      {
                                                          Id = "RED",
                                                          Name = "Alfa Rosso",
                                                          Description = "Intensives rot",
                                                          ProductNumber = "RED"
                                                      },
                                                      new Option()
                                                      {
                                                          Id = "P500",
                                                          Name = "P500",
                                                          Description = "Sehr starker Benzinmotor",
                                                          ProductNumber = "P500"
                                                      },
                                                      new Option()
                                                      {
                                                          Id = "GREEN",
                                                          Name = "Green demon",
                                                          Description = "Grün",
                                                          ProductNumber = "GREEN"
                                                      }};

            List<string> productImages = new List<string> { "Alfa_159_grey.jpg" };

            List<OptionGroup> optionGroups = new List<OptionGroup>
            {
                new OptionGroup() { Id = "COLOR_GROUP", Name = "Farbe", Description = "Farbe des Autos", OptionIds = new List<string>(){ "RED", "GREEN"}, Required = false },
                new OptionGroup() { Id = "MOTORTYPE_GROUP", Name = "Treibstoff", Description = "Motortreibstoff", OptionIds = new List<string>(){ "DIESEL", "PETROL"}, Required = false },
                new OptionGroup() { Id = "MOTOR_GROUP", Name = "Motorstärke", Description = "Die Kraft des Motors", OptionIds = new List<string>(){ "D150", "D200", "D250", "P500"}, Required = false }
            };

            List<OptionSection> optionSections = new List<OptionSection>
            {
                new OptionSection() { Name = "Exterior", Id = "EXTERIOR", OptionGroupIds = new List<string> { "COLOR_GROUP" }},
                new OptionSection() { Name = "Motor", Id = "MOTOR_SECTION", OptionGroupIds = new List<string> { "MOTORTYPE_GROUP", "MOTOR_GROUP" }}
            };

            RulesExtended productDependencies = new RulesExtended()
            {
                BasePrice = 50000f,
                Models = new List<ModelType> { new ModelType { Id = "Ti", Description = "Sportliche Version", OptionIds = new List<string>() { "RED", "DIESEL", "D150" }, Name = "Ti" } },
                DefaultModel = "TI",
                ReplacementGroups = new Dictionary<string, List<string>> { { "COLOR_GROUP", new List<string> { "RED", "GREEN" } }, { "MOTORTYPE_GROUP", new List<string> { "DIESEL", "PETROL" } }, { "MOTOR_GROUP", new List<string> { "D150", "D200", "D250" } } },
                Requirements = new Dictionary<string, List<string>> { { "D150", new List<string> { "DIESEL" } }, { "D200", new List<string> { "DIESEL" } }, { "D250", new List<string> { "DIESEL" } } },
                Incompatibilities = new Dictionary<string, List<string>> { { "P500", new List<string> { "DIESEL" } } },
                GroupRequirements = new Dictionary<string, List<string>> {  },
                PriceList = new Dictionary<string, float> { { "D150", 1500f },
                                                { "RED", 250f },
                                                { "DIESEL", 150f} }
            };

            Configurators["de"].RemoveAll(x => true);

            Configurators["de"].Add(new Configurator()
            {
                ConfigId = "Alfa",
                Name = "Alfa",
                Description = "159",
                Images = productImages,
                Rules = productDependencies,
                OptionGroups = optionGroups,
                Options = options,
                OptionSections = optionSections}
            );
        }

        public static Account FillAccountFromToken(string bearerToken)
        {
            Account account = new Account() { UserName = "admin", UserEmail = "configurator-admin@test-fuchs.com", IsAdmin = true };
            try
            {
                JWTContainerModel model = JWTContainerModel.GetJWTContainerModel(account.UserName, account.UserEmail, account.IsAdmin);

                JWTService jWTService = new JWTService(model.SecretKey);

                //bearerToken = jWTService.GenerateToken(model);
                //bearerToken = bearerToken.Replace("Bearer ", "");

                //foreach (Claim claim in jWTService.GetTokenClaims(bearerToken))
                //{
                //    switch (claim.Type)
                //    {
                //        case "userName":
                //            account.UserName = claim.Value;
                //            break;
                //        case "userEmail":
                //            account.UserEmail = claim.Value;
                //            break;
                //        case "admin":
                //            account.IsAdmin = Convert.ToBoolean(claim.Value);
                //            break;
                //    }
                //}
            }
            catch { }

            return account;
        }
        public static Dictionary<string, Configurator> GenerateConfigurator(ConfiguratorPost configuratorPost)
        {
            Dictionary<string, Configurator> configs = new Dictionary<string, Configurator>();
            LanguageVariant lv;
            try
            {
                lv = configuratorPost.Languages["en"];
            }
            catch (KeyNotFoundException)
            {
                lv = configuratorPost.Languages[configuratorPost.Languages.Keys.First()];
            }
            foreach (KeyValuePair<string, LanguageVariant> languageDict in configuratorPost.Languages)
            {
                Configurator temp = new Configurator()
                {
                    ConfigId = GenerateConfigId(lv),
                    Images = configuratorPost.Images,
                    Rules = configuratorPost.Rules.ConvertToExtended(),
                    Name = languageDict.Value.Name,
                    Description = languageDict.Value.Description,
                    OptionGroups = GetConfiguratorValues<OptionGroup, OptionGroupIndex, DescribedIndex>(configuratorPost.OptionGroups, languageDict.Value, languageDict.Value.OptionGroups),
                    Options = GetConfiguratorValues<Option, IIndexable, Option>(configuratorPost.Options.Cast<IIndexable>().ToList(), languageDict.Value, languageDict.Value.Options),
                    OptionSections = GetConfiguratorValues<OptionSection, LanguageIndexGroup, NamedIndex>(configuratorPost.OptionSections, languageDict.Value, languageDict.Value.OptionSections)
                };
                temp.Rules.Models = GetConfiguratorValues<ModelType, LanguageIndex, DescribedIndex>(configuratorPost.Rules.Models, languageDict.Value, languageDict.Value.Models);
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
                    elements.Add(GenerateValues(element as LanguageIndex, currentElement as DescribedIndex) as T);
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
        private static ModelType GenerateValues(LanguageIndex loopElement, DescribedIndex currentElement)
        {
            return new ModelType()
            {
                Id = loopElement.Id,
                OptionIds = loopElement.OptionIds,
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
            if(configurator.Rules.DefaultModel != "")
                configurator.Rules.DefaultModel += $"+{configurator.ConfigId}";

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
