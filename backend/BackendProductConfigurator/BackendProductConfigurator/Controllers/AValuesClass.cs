using BackendProductConfigurator.Validation.JWT.Managers;
using Model;
using System.Security.Claims;
using System.Text;

namespace BackendProductConfigurator.Controllers
{
    public abstract class AValuesClass
    {
        public static Dictionary<string, List<Configurator>> Configurators { get; set; } = new Dictionary<string, List<Configurator>>() { { "de", new List<Configurator>() }, { "en", new List<Configurator>() }, { "fr", new List<Configurator>() } };
        public static Dictionary<string, List<ConfiguratorSlim>> ConfiguratorsSlim { get; set; } = new Dictionary<string, List<ConfiguratorSlim>>() { { "de", new List<ConfiguratorSlim>() }, { "en", new List<ConfiguratorSlim>() }, { "fr", new List<ConfiguratorSlim>() } };
        public static Dictionary<string, List<ConfiguredProduct>> ConfiguredProducts { get; set; } = new Dictionary<string, List<ConfiguredProduct>>() { { "de", new List<ConfiguredProduct>() }, { "en", new List<ConfiguredProduct>() }, { "fr", new List<ConfiguredProduct>() } };
        public static Dictionary<string, List<ProductSaveExtended>> SavedProducts { get; set; } = new Dictionary<string, List<ProductSaveExtended>>() { { "de", new List<ProductSaveExtended>() }, { "en", new List<ProductSaveExtended>() }, { "fr", new List<ProductSaveExtended>() } };
        public static Dictionary<string, List<Account>> Accounts { get; set; } = new Dictionary<string, List<Account>>() { { "de", new List<Account>() }, { "en", new List<Account>() }, { "fr", new List<Account>() } };

        private static EValueMode ValueMode { get; set; } = EValueMode.DatabaseValues;
        private static string serverAddress = "http://andifined.ddns.net:5129";
        private static List<string> languages = new List<string>() { "de", "en", "fr" };

        private static Dictionary<Type, string> typeApis = new Dictionary<Type, string>
        {
            {typeof(ProductSaveExtended), "/db/configuration"},
            {typeof(Configurator), "/db/product" }
        };

        public static void SetValues()
        {
            switch(ValueMode)
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
            if(ValueMode == EValueMode.DatabaseValues)
                ADBAccess<T>.PostValue(language, serverAddress, typeApis[typeof(T)], value);
        }
        public static async void DeleteValue<T>(string language, T identifier) where T : class
        {
            if (ValueMode == EValueMode.DatabaseValues)
                await ADBAccess<T>.DeleteValue(language, serverAddress, typeApis[typeof(T)], identifier);
        }
        public static void SetDBValues()
        {
            foreach(string language in languages)
            {
                Configurators[language] = ADBAccess<Configurator>.GetValues(language, serverAddress, typeApis[typeof(Configurator)]).Result;

                ConfiguratorsSlim[language] = Configurators[language].Cast<ConfiguratorSlim>().ToList();

                SavedProducts[language] = ADBAccess<ProductSaveExtended>.GetValues(language, serverAddress, typeApis[typeof(ProductSaveExtended)]).Result;
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
                new OptionSection("Exterior", "EXTERIOR", new List<string> { "COLOR_GROUP" }),
                new OptionSection("Motor", "MOTOR_SECTION", new List<string> { "MOTORTYPE_GROUP", "MOTOR_GROUP" })
            };

            RulesExtended productDependencies = new RulesExtended()
            {
                BasePrice = 50000f,
                Models = new List<ModelType> { new ModelType { Name = "TI", Description = "Sportliche Version", Options = new List<string>() { "RED", "DIESEL", "D150" } } },
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

            ConfiguratorSlim ps1 = new ConfiguratorSlim()
            {
                ConfigId = "BENZ1",
                Name = "Fetter Benz",
                Description = "Laut und groß",
                Images = productImages
            };

            ConfiguratorSlim ps2 = new ConfiguratorSlim() {
                ConfigId = "ALFA1",
                Name = "Eleganter Alfa Romeo",
                Description = "Stylisch und erweckt",
                Images = productImages
            };

            ConfiguratorSlim ps3 = new ConfiguratorSlim()
            {
                ConfigId = "BENZ2",
                Name = "Fetterer Benz",
                Description = "Umso lauter und größer",
                Images = productImages
            };

            ConfiguratorsSlim["de"] = new List<ConfiguratorSlim> { ps1, ps2, ps3 };

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
            Account account = new Account();
            JWTService jWTService = new JWTService("c2plaDkzdWhBVWhpdW9zZGg5ODhob2lBdWgz");

            bearerToken = bearerToken.Replace("Bearer ", "");

            foreach(Claim claim in jWTService.GetTokenClaims(bearerToken))
            {
                switch(claim.Type)
                {
                    case ClaimTypes.Name:
                        account.UserName = claim.Value;
                        break;
                    case ClaimTypes.Email:
                        account.UserEmail = claim.Value;
                        break;
                }
            }

            return account;
        }
        public static Configurator GenerateConfigurator(ConfiguratorPost configuratorPost, string language)
        {
            Configurator configurator = configuratorPost as ConfiguratorBase as Configurator;

            foreach(OptionGroupExtended oge in configurator.OptionGroups)
            {
                if(oge.Replacement)
                {
                    configurator.Rules.ReplacementGroups.Add(oge.Id, oge.OptionIds);
                }
                configurator.OptionGroups.Add(oge as OptionGroup);
            }

            return configurator;
        }
        private static string GenerateConfigId(ConfiguratorPost configuratorPost, string postLanguage)
        {
            StringBuilder sb = new StringBuilder(configuratorPost.Name);
            List<string> configIds = new List<string>();
            foreach(string language in languages)
            {
                configIds.AddRange(AValuesClass.Configurators[language].Select(x => x.ConfigId).ToList());
            }

            sb.Replace(' ', '_');
            sb.Append("_").Append(postLanguage);

            int i = 1;
            while(configIds.Contains(sb.ToString()))
            {
                if (sb.ToString().Contains('#'))
                    sb.Remove(sb.ToString().IndexOf('#'), 5);
                sb.Append('#').Append(i++.ToString().PadLeft(4, '0'));
            }

            return sb.ToString();
        }
    }
    public enum EValueMode { TestValues, DatabaseValues }
}
