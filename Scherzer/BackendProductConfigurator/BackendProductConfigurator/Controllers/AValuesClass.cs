using Model;

namespace BackendProductConfigurator.Controllers
{
    public abstract class AValuesClass
    {
        public static List<ProductConfig> ProductConfig { get; set; } = new List<ProductConfig>();
        public static List<Product> Products { get; set; } = new List<Product>();
        public static List<ProductSlim> ProductsSlim { get; set; } = new List<ProductSlim>();

        public static List<User> Users { get; set; } = new List<User>();

        public static void SetValues()
        {
            List<Option> options = new List<Option> { new Option("D150", "D150", "Fetter Diesel Motor", new List<string> { "youtube.com" }), new Option("D200", "D200", "Fetter Diesel Motor", new List<string> { "youtube.com" }), new Option("D250", "D250", "Fetter Diesel Motor", new List<string> { "youtube.com" }) };
            List<string> productImages = new List<string> { "google.com" };
            List<OptionGroup> optionGroups = new List<OptionGroup> { new OptionGroup("Color", "The exterior color of the product", "COLOR_GROUP", new List<string> { "RED", "WHITE", "GREEN" }), new OptionGroup("Motor Type", "The motor of your car", "MOTORTYPE_GROUP", new List<string> { "DIESEL", "PETROL", "ELECTRIC" }), new OptionGroup("Motor", "The selected Motor power", "MOTOR_GROUP", new List<string> { "D150", "D200", "D250" }) };
            List<OptionSection> optionSections = new List<OptionSection> { new OptionSection("Exterior", "EXTERIOR", new List<string> { "COLOR_GROUP" }), new OptionSection("Motor", "MOTOR_SECTION", new List<string> { "MOTORTYPE_GROUP", "MOTOR_GROUP" }) };
            ProductDependencies productDependencies = new ProductDependencies(50000, new List<string> { "RED", "DIESEL", "D150" }, new Dictionary<string, List<string>> { { "COLOR_GROUP", new List<string> { "ey", "wos" } } }, new Dictionary<string, List<string>> { { "D150", new List<string> { "DIESEL" } } }, new Dictionary<string, List<string>> { { "D150", new List<string> { "PETROL" } } }, new Dictionary<string, int> { { "D150", 1500 } });
            ProductConfig = new List<ProductConfig> { new ProductConfig(0, "Alfa Romeo 159 Configurator", "Configurable Car", productImages, productDependencies, options, optionGroups, optionSections) };

            List<Option> optionsList = new List<Option> { new Option("Option1", "Erste Option", "Ka Ahnung wos des duat", new List<string> { "Zehner.net", "Cool.com" }) };

            Product p1 = new Product() { Id = 0, Name = "Fetter Benz", Description = "Laut und groß", Images = new List<string> { "Zehner.net", "Cool.com" }, Options = optionsList, Price=4.2f };
            Product p2 = new Product() { Id = 1, Name = "Eleganter Alfa Romeo", Description = "Stylisch und erweckt", Images = new List<string> { "Zehner.net", "Cool.com" }, Options = optionsList, Price = 9.65f };
            Product p3 = new Product() { Id = 2, Name = "Fetterer Benz", Description = "Umso lauter und größer", Images = new List<string> { "Zehner.net", "Cool.com" }, Options = optionsList, Price = 0.8f };
            Products = new List<Product> { p1, p2, p3 };

            ProductSlim ps1 = new ProductSlim() { Id = 0, Name = "Fetter Benz", Description = "Laut und groß" };
            ProductSlim ps2 = new ProductSlim() { Id = 1, Name = "Eleganter Alfa Romeo", Description = "Stylisch und erweckt" };
            ProductSlim ps3 = new ProductSlim() { Id = 2, Name = "Fetterer Benz", Description = "Umso lauter und größer" };
            ProductsSlim = new List<ProductSlim> { ps1, ps2, ps3 };

            User u1 = new User() { Id = 1, Name = "TEST-FUCHS GmbH" };
            User u2 = new User() { Id = 2, Name = "HTL Krems" };
            User u3 = new User() { Id = 3, Name = "AVIA Station Gmünd" };
            Users = new List<User> { u1, u2, u3 };
        }
    }
}
