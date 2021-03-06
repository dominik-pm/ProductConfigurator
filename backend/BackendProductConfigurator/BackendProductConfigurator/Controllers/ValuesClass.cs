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
        public static Dictionary<string, List<Configurator>> Configurators { get; set; } = CreateLists<Configurator>(true);
        public static Dictionary<string, List<ConfiguredProduct>> ConfiguredProducts { get; set; } = CreateLists<ConfiguredProduct>(true);
        public static Dictionary<string, List<ProductSaveExtended>> SavedProducts { get; set; } = CreateLists<ProductSaveExtended>(false);
        public static Dictionary<string, List<Account>> Accounts { get; set; } = CreateLists<Account>(false);

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
                    task.Wait(GlobalValues.TimeOut);
                    break;
            }
        }
        public static void PostValue<T>(T value, string language) where T : class
        {
            if (GlobalValues.ValueMode == EValueMode.DatabaseValues)
            {
                try
                {
                    DBAccess<T>.PostValue(language, GlobalValues.ServerAddress, typeApis[typeof(T)], value).Wait();
                }
                catch (Exception e)
                {
                    throw;
                }
            }
        }
        public static void PutValue<T>(T value, string language) where T : class
        {
            if (GlobalValues.ValueMode == EValueMode.DatabaseValues)
            {
                try
                {
                    DBAccess<T>.PutValue(language, GlobalValues.ServerAddress, typeApis[typeof(T)], value).Wait();
                }
                catch (Exception e)
                {
                    throw;
                }
            }
        }
        public static void DeleteValue<T>(string language, T identifier) where T : class
        {
            if (GlobalValues.ValueMode == EValueMode.DatabaseValues)
            {
                try
                {
                    DBAccess<T>.DeleteValue(language, GlobalValues.ServerAddress, typeApis[typeof(T)], identifier).Wait();
                }
                catch (Exception e)
                {
                    throw;
                }
            }
        }
        public static void SetDBValues()
        {
            List<Task> tasks = new List<Task>();
            Task temp;
            foreach(string language in GlobalValues.Languages)
            {
                temp = Task.Factory.StartNew(new Action<object?>((str) =>
                {
                    try
                    {
                        string taskLanguage = str as string;
                        Task<List<Configurator>> t = DBAccess<Configurator>.GetValues(taskLanguage, GlobalValues.ServerAddress, typeApis[typeof(Configurator)]);
                        t.Wait();
                        Configurators[taskLanguage] = t.Result;
                    }
                    catch { }
                }), language);
                tasks.Add(temp);
                temp = Task.Factory.StartNew(new Action<object?>((str) =>
                {
                    try
                    {
                        string taskLanguage = str as string;
                        Task<List<ProductSaveExtended>> t = DBAccess<ProductSaveExtended>.GetValues(taskLanguage, GlobalValues.ServerAddress, typeApis[typeof(ProductSaveExtended)]);
                        SavedProducts[taskLanguage] = t.Wait(GlobalValues.TimeOut) ? t.Result : SavedProducts[taskLanguage];
                        t.Wait();
                        SavedProducts[taskLanguage] = t.Result;
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

            foreach(string language in GlobalValues.Languages)
            {
                Configurators[language].RemoveAll(x => true);

                Configurators[language].Add(new Configurator()
                {
                    ConfigId = "Alfa",
                    Name = "Alfa",
                    Description = "159",
                    Images = productImages,
                    Rules = productDependencies,
                    OptionGroups = optionGroups,
                    Options = options,
                    OptionSections = optionSections
                }
                );
            }
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
                    Options = GetConfiguratorValues<Option, IIndexable, OptionSlim>(configuratorPost.Options.Cast<IIndexable>().ToList(), languageDict.Value, languageDict.Value.Options),
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
                    elements.Add(GenerateValues(element as IdWrapper, currentElement as OptionSlim) as T);
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
        private static Option GenerateValues(IdWrapper loopElement, OptionSlim currentElement)
        {
            return new Option()
            {
                Id = currentElement.Id,
                Name = currentElement.Name,
                Description = currentElement.Description,
                ProductNumber = loopElement.ProductNumber
            };
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

            configIds.AddRange(ValuesClass.Configurators[GlobalValues.Languages.First()].Select(x => x.ConfigId).ToList());

            sb.Replace(' ', '_');

            int i = 1;
            while (configIds.Contains(sb.ToString()))
            {
                if (sb.ToString().Contains('*'))
                    sb.Remove(sb.ToString().IndexOf('*'), 5);
                sb.Append('*').Append(i++.ToString().PadLeft(4, '0'));
            }

            return sb.ToString();
        }
        public static Configurator AdaptConfiguratorsOptionIds(Configurator configurator, string oldConfigId)
        {
            try
            {
                if (!configurator.Options[0].Id.EndsWith($"+{oldConfigId}"))
                {
                    foreach (Option option in configurator.Options)
                    {
                        if(oldConfigId != "")
                            option.Id = option.Id.Replace($"_option+{oldConfigId}", string.Empty);
                        option.Id += $"_option+{configurator.ConfigId}";
                    }
                    foreach (LanguageIndex li in configurator.OptionGroups)
                    {
                        if (oldConfigId != "")
                            li.Id = li.Id.Replace($"_og+{oldConfigId}", string.Empty);
                        li.Id += $"_og+{configurator.ConfigId}";
                        li.OptionIds = li.OptionIds.Select(x => x = (oldConfigId != "") ? x.Replace($"_option+{oldConfigId}", string.Empty) : x).Select(x => x += $"_option+{configurator.ConfigId}").ToList();
                    }
                    foreach (OptionSection os in configurator.OptionSections)
                    {
                        if (oldConfigId != "")
                            os.Id = os.Id.Replace($"_os+{oldConfigId}", string.Empty);
                        os.Id += $"_os+{configurator.ConfigId}";
                        os.OptionGroupIds = os.OptionGroupIds.Select(x => x = (oldConfigId != "") ? x.Replace($"_og+{oldConfigId}", string.Empty) : x).Select(x => x += $"_og+{configurator.ConfigId}").ToList();
                    }
                    foreach (LanguageIndex li in configurator.Rules.Models)
                    {
                        if (oldConfigId != "")
                            li.Id = li.Id.Replace($"_model+{oldConfigId}", string.Empty);
                        li.Id += $"_model+{configurator.ConfigId}";
                        li.OptionIds = li.OptionIds.Select(x => x = (oldConfigId != "") ? x.Replace($"_option+{oldConfigId}", string.Empty) : x).Select(x => x += $"_option+{configurator.ConfigId}").ToList();
                    }

                    configurator.Rules.ReplacementGroups = AdaptIdsInDictionarys(configurator.Rules.ReplacementGroups, configurator.ConfigId, oldConfigId, "option");
                    configurator.Rules.Requirements = AdaptIdsInDictionarys(configurator.Rules.Requirements, configurator.ConfigId, oldConfigId, "option");
                    configurator.Rules.Incompatibilities = AdaptIdsInDictionarys(configurator.Rules.Incompatibilities, configurator.ConfigId, oldConfigId, "option");
                    configurator.Rules.GroupRequirements = AdaptIdsInDictionarys(configurator.Rules.GroupRequirements, configurator.ConfigId, oldConfigId, "og");
                    if (configurator.Rules.DefaultModel != "")
                    {
                        if (oldConfigId != "")
                            configurator.Rules.DefaultModel = configurator.Rules.DefaultModel.Replace($"_model+{oldConfigId}", "_model");
                        configurator.Rules.DefaultModel += $"_model+{configurator.ConfigId}";
                    }

                    configurator.Rules.PriceList = AdaptIdsInDictionarys(configurator.Rules.PriceList, configurator.ConfigId, oldConfigId, "option");
                }
            }
            catch (Exception ex)
            {
                throw ex;
            }
            
            return configurator;
        }
        private static Dictionary<string, List<string>> AdaptIdsInDictionarys(Dictionary<string, List<string>> dictionary, string configId, string oldConfigId, string appendage)
        {
            Dictionary<string, List<string>> temp = new Dictionary<string, List<string>>();
            foreach (KeyValuePair<string, List<string>> dic in dictionary)
            {
                temp.Add($"{((oldConfigId != "") ? dic.Key.Replace($"_{appendage}+{oldConfigId}", "") : dic.Key)}_{appendage}+{configId}", dic.Value.Select(x => x = (oldConfigId != "") ? x.Replace($"_{appendage}+{oldConfigId}", string.Empty) : x + $"_{appendage}+{configId}").ToList());
            }
            return temp;
        }
        private static Dictionary<string, float> AdaptIdsInDictionarys(Dictionary<string, float> dictionary, string configId, string oldConfigId, string appendage)
        {
            Dictionary<string, float> temp = new Dictionary<string, float>();
            foreach (KeyValuePair<string, float> dic in dictionary)
            {
                temp.Add($"{((oldConfigId != "") ? dic.Key.Replace($"_{appendage}+{oldConfigId}", "") : dic.Key)}_{appendage}+{configId}", dic.Value);
            }
            return temp;
        }
        private static Dictionary<string, List<T>> CreateLists<T>(bool isMultiLanguage)
        {
            Dictionary<string, List<T>> dict = new Dictionary<string, List<T>>();
            if (isMultiLanguage)
            {
                foreach (string language in GlobalValues.Languages)
                {
                    dict.Add(language, new List<T>());
                }
            }
            else
                dict.Add("NaL", new List<T>()); //NaL => Not a Language
            return dict;
        }
    }
}
