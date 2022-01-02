using Model;

namespace BackendProductConfigurator.Controllers
{
    public abstract class AValuesClass
    {
        public static List<Configurator> Configurators { get; set; } = new List<Configurator>();
        public static List<ConfiguredProduct> ConfiguredProducts { get; set; } = new List<ConfiguredProduct>();
        public static List<ProductSlim> ProductsSlim { get; set; } = new List<ProductSlim>();
        public static List<ProductSave> SavedProducts { get; set; } = new List<ProductSave>();
        public static List<Account> Accounts { get; set; } = new List<Account>();

        public static void SetValues()
        {
            List<Option> options = new List<Option> {
                                                      new Option("D150", "D150", "Fetter Diesel Motor", new List<string> { "youtube.com" }),
                                                      new Option("D200", "D200", "Fetter Diesel Motor", new List<string> { "youtube.com" }),
                                                      new Option("D250", "D250", "Fetter Diesel Motor", new List<string> { "youtube.com" }),
                                                      new Option("RED", "Alfa Rosso", "Red like a cherry", new List<string> { "" }),
                                                      new Option("GREEN", "Green demon", "Green like the grinch", new List<string> { "" }),
                                                      new Option("WHITE", "White cloud", "White as a sheet of paper", new List<string> { "" }),
                                                      new Option("PROOF", "Panorama roof", "Very big panorama roof", new List<string> { "" }),
                                                     };

            List<string> productImages = new List<string> { "google.com" };

            List<OptionGroup> optionGroups = new List<OptionGroup> { new OptionGroup() { Id = "COLOR_GROUP", Name = "Color", Description = "What color you want", OptionIds = new List<string>(){ "RED", "WHITE", "GREEN"}, Required = true },
                                                                     new OptionGroup() { Id = "MOTORTYPE_GROUP", Name = "A motor fuel type", Description = "What motor fuel", OptionIds = new List<string>(){ "DIESEL", "PETROL"}, Required = true },
                                                                     new OptionGroup() { Id = "MOTOR_GROUP", Name = "A motor power", Description = "The motor power", OptionIds = new List<string>(){ "D150", "D200", "D250"}, Required = true } };

            List<OptionSection> optionSections = new List<OptionSection> { new OptionSection("Exterior", "EXTERIOR", new List<string> { "COLOR_GROUP" }),
                                                                           new OptionSection("Motor", "MOTOR_SECTION", new List<string> { "MOTORTYPE_GROUP", "MOTOR_GROUP" }) };

            ProductDependencies productDependencies = new ProductDependencies(50000,
                                                                              new List<string> { "RED", "DIESEL", "D150" },
                                                                              new Dictionary<string, List<string>> { { "COLOR_GROUP", new List<string> { "ey", "wos" } } },
                                                                              new Dictionary<string, List<string>> { { "D150", new List<string> { "DIESEL" } } },
                                                                              new Dictionary<string, List<string>> { { "D150", new List<string> { "PETROL" } } },
                                                                              new Dictionary<string, float> { { "D150", 1500f },
                                                                                                              { "RED", 250f },
                                                                                                              { "PROOF", 250f} });

            Configurators.Add(new Configurator() { Id = 0, Name = "Neuer Konfigurator", Description = "Sehr cool", Images = productImages, Dependencies = productDependencies, OptionGroups = optionGroups, Options = options, OptionSections = optionSections});

            List<Option> optionsList = new List<Option> { new Option("Option1", "Erste Option", "Ka Ahnung wos des duat", new List<string> { "Zehner.net", "Cool.com" }) };

            ConfiguredProduct p1 = new ConfiguredProduct()
            { 
                ConfiguratorId = 0,
                ConfigurationName = "Fetter Benz",
                Options = optionsList,
                Price=4.2f };

            ConfiguredProduct p2 = new ConfiguredProduct()
            {
                ConfiguratorId = 0,
                ConfigurationName = "Eleganter Alfa Romeo",
                Options = optionsList,
                Price = 9.65f };

            ConfiguredProduct p3 = new ConfiguredProduct()
            {
                ConfiguratorId = 0,
                ConfigurationName = "Fetterer Benz",
                Options = optionsList,
                Price = 0.8f };

            ConfiguredProducts = new List<ConfiguredProduct> { p1, p2, p3 };

            ProductSlim ps1 = new ProductSlim() { //Hier Images hinzufügen, ProductSave anschauen, usw... CLEAN UP
                Id = 0,
                Name = "Fetter Benz",
                Description = "Laut und groß",
                Images = productImages};

            ProductSlim ps2 = new ProductSlim() {
                Id = 1,
                Name = "Eleganter Alfa Romeo",
                Description = "Stylisch und erweckt",
                Images = productImages
            };

            ProductSlim ps3 = new ProductSlim() {
                Id = 2,
                Name = "Fetterer Benz",
                Description = "Umso lauter und größer",
                Images = productImages
            };

            ProductsSlim = new List<ProductSlim> { ps1, ps2, ps3 };

            Account acc1 = new Account() {
                Id = 1,
                Name = "TEST-FUCHS GmbH",
                Email = "huh@what.com" };

            Account acc2 = new Account() {
                Id = 2,
                Name = "HTL Krems",
                Email = "huh@what.com" };

            Account acc3 = new Account() {
                Id = 3,
                Name = "AVIA Station Gmünd",
                Email = "huh@what.com" };

            Accounts = new List<Account> { acc1, acc2, acc3 };

            ProductSave psave1 = new ProductSave() { 
                Description = "Saved product",
                Id = 6,
                Name = "Alfa 147",
                Options = options,
                SavedName = "Scherzermobil",
                Status = EStatus.Saved.ToString() };
            ProductSave psave2 = new ProductSave()
            {
                Description = "Saved product",
                Id = 7,
                Name = "Alfa 156",
                Options = options,
                SavedName = "Pömmermobil",
                Status = EStatus.Saved.ToString()
            };
            ProductSave psave3 = new ProductSave()
            {
                Description = "Saved product",
                Id = 8,
                Name = "Alfa 166",
                Options = options,
                SavedName = "Leutgeb Toyota",
                Status = EStatus.Saved.ToString()
            };
            SavedProducts = new List<ProductSave> { psave1, psave2, psave3 };
        }
    }
}
