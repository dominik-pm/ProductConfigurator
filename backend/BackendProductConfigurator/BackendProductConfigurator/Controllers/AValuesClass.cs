using Model;
using ADBAccess;

namespace BackendProductConfigurator.Controllers
{
    public abstract class AValuesClass
    {
        public static List<Configurator> Configurators { get; set; } = new List<Configurator>();
        public static List<ConfiguratorSlim> ConfiguratorsSlim { get; set; } = new List<ConfiguratorSlim>();
        public static List<ConfiguredProduct> ConfiguredProducts { get; set; } = new List<ConfiguredProduct>();
        public static List<ProductSaveExtended> SavedProducts { get; set; } = new List<ProductSaveExtended>();
        public static List<Account> Accounts { get; set; } = new List<Account>();

        public static void SetValues(EValueMode valueMode)
        {
            switch(valueMode)
            {
                case EValueMode.TestValues:
                    SetStaticValues();
                    break;
                case EValueMode.DatabaseValues:
                    SetDBValues();
                    break;
            }
        }
        public static void SetDBValues()
        {
            string serverAddress = "https://localhost:7109";
            Configurators = ADBAccess<Configurator>.GetValues(serverAddress, "/db/configurations").Result;
            SavedProducts = ADBAccess<ProductSaveExtended>.GetValues(serverAddress, "/db/account/configurations").Result;
        }

        public static void SetStaticValues()
        {
            List<Option> options = new List<Option> {
                                                      new Option("D150", "D150", "Fetter Diesel Motor"),
                                                      new Option("D200", "D200", "Fetter Diesel Motor"),
                                                      new Option("D250", "D250", "Fetter Diesel Motor"),
                                                      new Option("RED", "Alfa Rosso", "Red like a cherry"),
                                                      new Option("GREEN", "Green demon", "Green like the grinch"),
                                                      new Option("WHITE", "White cloud", "White as a sheet of paper"),
                                                      new Option("PROOF", "Panorama roof", "Very big panorama roof"),
                                                     };

            List<string> productImages = new List<string> { "google.com" };

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

            ProductDependencies productDependencies = new ProductDependencies
            (
                50000f,
                new List<string> { "RED", "DIESEL", "D150" },
                new Dictionary<string, List<string>> { { "COLOR_GROUP", new List<string> { "ey", "wos" } } },
                new Dictionary<string, List<string>> { { "D150", new List<string> { "DIESEL" } } },
                new Dictionary<string, List<string>> { { "D150", new List<string> { "PETROL" } } },
                new Dictionary<string, List<string>> { { "PANORAMATYPE_GROUP", new List<string> { "PANORAMAROOF" } } },
                new Dictionary<string, float> { { "D150", 1500f },
                                                { "RED", 250f },
                                                { "PROOF", 250f} }
            );

            Configurators.Add(new Configurator()
            {
                ConfigId = 0,
                Name = "Neuer Konfigurator",
                Description = "Sehr cool",
                Images = productImages,
                Dependencies = productDependencies,
                OptionGroups = optionGroups,
                Options = options,
                OptionSections = optionSections}
            );

            List<Option> optionsList = new List<Option>
            {
                new Option("Option1", "Erste Option", "Ka Ahnung wos des duat")
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
                ConfigId = 0,
                Name = "Fetter Benz",
                Description = "Laut und groß",
                Images = productImages
            };

            ConfiguratorSlim ps2 = new ConfiguratorSlim() {
                ConfigId = 1,
                Name = "Eleganter Alfa Romeo",
                Description = "Stylisch und erweckt",
                Images = productImages
            };

            ConfiguratorSlim ps3 = new ConfiguratorSlim()
            {
                ConfigId = 2,
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
                ConfigId = 0
            };
            ProductSaveExtended psave2 = new ProductSaveExtended()
            {
                Description = "Saved product",
                Name = "Alfa 156",
                Options = options,
                SavedName = "Pömmermobil",
                Status = EStatus.Saved.ToString(),
                User = acc2,
                ConfigId = 1
            };
            ProductSaveExtended psave3 = new ProductSaveExtended()
            {
                Description = "Saved product",
                Name = "Alfa 166",
                Options = options,
                SavedName = "Leutgeb Toyota",
                Status = EStatus.Saved.ToString(),
                User = acc3,
                ConfigId = 2
            };
            SavedProducts = new List<ProductSaveExtended> { psave1, psave2, psave3 };
        }
    }
    public enum EValueMode { TestValues, DatabaseValues}
}
