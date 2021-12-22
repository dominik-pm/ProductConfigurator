using BackendTest.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using Newtonsoft.Json;

namespace BackendTest.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger)
        {
            _logger = logger;
        }

        public IActionResult Index()
        {
            return View();
        }

        public IActionResult Privacy()
        {
            return View();
        }
        public string Product()
        {
            //User user = new User() { Age = 18, Email = "jokers@mail.com", Name = "Tobias Scherzer" };
            List<Option> options = new List<Option> { new Option("D150", "Fetter Diesel Motor", new List<string> { "youtube.com" }, "D150") };
            List<string> productImages = new List<string> { "boahnhub.tk" };
            List<OptionGroup> optionGroups = new List<OptionGroup> { new OptionGroup("Color", "The exterior color of the product", "COLOR_GROUP", new List<string> { "RED", "WHITE", "GREEN" }), new OptionGroup("Motor Type", "The motor of your car", "MOTORTYPE_GROUP", new List<string> { "DIESEL", "PETROL", "ELECTRIC" }), new OptionGroup("Motor", "The selected Motor power", "MOTOR_GROUP", new List<string> { "D150", "D200", "D250" }) };
            List<OptionSection> optionSections = new List<OptionSection> { new OptionSection("Exterior", "EXTERIOR", new List<string> { "COLOR_GROUP" }), new OptionSection("Motor", "MOTOR_SECTION", new List<string> { "MOTORTYPE_GROUP", "MOTOR_GROUP" }) };
            ProductDependencies productDependencies = new ProductDependencies(50000, new List<string> { "RED", "DIESEL", "D150" }, new List<OptionGroup> { new OptionGroup("Color", "The exterior color of the car", "COLOR_GROUP", new List<string> { "BLUE", "RED", "WHITE" }) }, new Dictionary<string, List<string>> { { "D150", new List<string> { "DIESEL" } } }, new Dictionary<string, List<string>> { { "D150", new List<string> { "PETROL" } } }, new Dictionary<string, int> { { "D150", 1500 } });
            Product product = new Product("0", "Alfa Romeo 159", "A really nice car", productImages, productDependencies, options, optionGroups, optionSections);
            string stringjson = JsonConvert.SerializeObject(product);
            return stringjson;
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}