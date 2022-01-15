using Model;

namespace BackendProductConfigurator.Controllers
{
    public abstract class AValuesClass
    {
        public static List<Configurator> Configurators { get; set; } = new List<Configurator>();
        public static List<ConfiguratorSlim> ConfiguratorsSlim { get; set; } = new List<ConfiguratorSlim>();
        public static List<ConfiguredProduct> ConfiguredProducts { get; set; } = new List<ConfiguredProduct>();
        public static List<ProductSaveExtended> SavedProducts { get; set; } = new List<ProductSaveExtended>();
        public static List<Account> Accounts { get; set; } = new List<Account>();

        private static EValueMode ValueMode { get; set; } = EValueMode.DatabaseValues;
        private static string serverAddress = "http://andifined.ddns.net:5129";

        private static Dictionary<Type, string> typeApis = new Dictionary<Type, string>
        {
            {typeof(ConfiguredProduct), "/db/configuration"},
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
        public static void PostValue<T>(T value) where T : class
        {
            if(ValueMode == EValueMode.DatabaseValues)
                ADBAccess<T>.PostValue(serverAddress, typeApis[typeof(ConfiguredProduct)], value);
        }
        public static void SetDBValues()
        {
            Configurators = ADBAccess<Configurator>.GetValues(serverAddress, typeApis[typeof(Configurator)]).Result;
            //SavedProducts = ADBAccess<ProductSaveExtended>.GetValues(serverAddress, "/db/account/configurations").Result;
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

            Rules productDependencies = new Rules()
            {
                BasePrice = 50000f,
                DefaultOptions = new List<string> { "RED", "DIESEL", "D150" },
                ReplacementGroups = new Dictionary<string, List<string>> { { "COLOR_GROUP", new List<string> { "ey", "wos" } } },
                Requirements = new Dictionary<string, List<string>> { { "D150", new List<string> { "DIESEL" } } },
                Incompatibilities = new Dictionary<string, List<string>> { { "D150", new List<string> { "PETROL" } } },
                GroupRequirements = new Dictionary<string, List<string>> { { "PANORAMATYPE_GROUP", new List<string> { "PANORAMAROOF" } } },
                PriceList = new Dictionary<string, float> { { "D150", 1500f },
                                                { "RED", 250f },
                                                { "DIESEL", 150f} }
            };

            Configurators.Add(new Configurator()
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
                Options = optionsList,
                Price=4.2f
            };

            ConfiguredProduct p2 = new ConfiguredProduct()
            {
                ConfigurationName = "Eleganter Alfa Romeo",
                Options = optionsList,
                Price = 9.65f
            };

            ConfiguredProduct p3 = new ConfiguredProduct()
            {
                ConfigurationName = "Fetterer Benz",
                Options = optionsList,
                Price = 0.8f
            };

            ConfiguredProducts = new List<ConfiguredProduct> { p1, p2, p3 };

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

            ConfiguratorsSlim = new List<ConfiguratorSlim> { ps1, ps2, ps3 };

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

            Accounts = new List<Account> { acc1, acc2, acc3 };

            ProductSaveExtended psave1 = new ProductSaveExtended()
            { 
                Description = "Saved product",
                Name = "Alfa 147",
                Options = options,
                SavedName = "Scherzermobil",
                Status = EStatus.Saved.ToString(),
                User = acc1,
                ConfigId = "ALFA1"
            };
            ProductSaveExtended psave2 = new ProductSaveExtended()
            {
                Description = "Saved product",
                Name = "Alfa 156",
                Options = options,
                SavedName = "Pömmermobil",
                Status = EStatus.Saved.ToString(),
                User = acc2,
                ConfigId = "ALFA"
            };
            ProductSaveExtended psave3 = new ProductSaveExtended()
            {
                Description = "Saved product",
                Name = "Alfa 166",
                Options = options,
                SavedName = "Leutgeb Toyota",
                Status = EStatus.Saved.ToString(),
                User = acc3,
                ConfigId = "BENZ1"
            };
            SavedProducts = new List<ProductSaveExtended> { psave1, psave2, psave3 };
        }
    }
    public enum EValueMode { TestValues, DatabaseValues }
}
