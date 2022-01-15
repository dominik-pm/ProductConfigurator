using BackendTest.Models;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;
using FluentEmail.Smtp;
using System.Net.Mail;
using FluentEmail.Core;
using System.Text;
using FluentEmail.Razor;
using PdfSharp.Pdf;
using PdfSharp.Drawing;

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
        public string PDF()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

            PdfDocument document = new PdfDocument();

            PdfPage page = document.AddPage();

            XGraphics gfx = XGraphics.FromPdfPage(page); //Holt sich seitenspezifische Details für die Zeichenmethoden

            XFont font = new XFont("Century Gothic", 14);

            gfx.DrawString("Numero 1",
                           font,
                           XBrushes.Black,
                           new XRect(150, 200, 0.6 * page.Width,0.3 *  page.Height),
                           XStringFormats.Center);

            gfx.DrawString("Numero 2",
                           font,
                           XBrushes.Violet,
                           new XRect(0, 0, page.Width, page.Height),
                           XStringFormats.BottomLeft);

            gfx.DrawString("Numero 3",
                           font,
                           XBrushes.Red,
                           new XPoint(100, 300),
                           XStringFormats.Center);

            document.Save("./TestPDF.pdf");

            return "";
        }
        public string Product()
        {
            List<Option> options = new List<Option> { new Option("D150", "Fetter Diesel Motor", new List<string> { "youtube.com" }, "D150") };
            List<string> productImages = new List<string> { "boahnhub.tk" };
            List<OptionGroup> optionGroups = new List<OptionGroup> { new OptionGroup("Color", "The exterior color of the product", "COLOR_GROUP", new List<string> { "RED", "WHITE", "GREEN" }), new OptionGroup("Motor Type", "The motor of your car", "MOTORTYPE_GROUP", new List<string> { "DIESEL", "PETROL", "ELECTRIC" }), new OptionGroup("Motor", "The selected Motor power", "MOTOR_GROUP", new List<string> { "D150", "D200", "D250" }) };
            List<OptionSection> optionSections = new List<OptionSection> { new OptionSection("Exterior", "EXTERIOR", new List<string> { "COLOR_GROUP" }), new OptionSection("Motor", "MOTOR_SECTION", new List<string> { "MOTORTYPE_GROUP", "MOTOR_GROUP" }) };
            ProductDependencies productDependencies = new ProductDependencies(50000, new List<string> { "RED", "DIESEL", "D150" }, new List<OptionGroup> { new OptionGroup("Color", "The exterior color of the car", "COLOR_GROUP", new List<string> { "BLUE", "RED", "WHITE" }) }, new Dictionary<string, List<string>> { { "D150", new List<string> { "DIESEL" } } }, new Dictionary<string, List<string>> { { "D150", new List<string> { "PETROL" } } }, new Dictionary<string, int> { { "D150", 1500 } });
            Product product = new Product("0", "Alfa Romeo 159", "A really nice car", productImages, productDependencies, options, optionGroups, optionSections);

            var sender = new SmtpSender(() => new SmtpClient("localhost")
            {
                EnableSsl = false, //Zum Testen ausschalten
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Port = 25
            }
            ); //localhost ist der Empfangsserver --> Für Gmail die Adresse von Gmail einfügen

            StringBuilder template = new StringBuilder();
            template.AppendLine("Sehr geehrte/r Kunde/in,");
            template.AppendLine("<p>wir haben ihren Kauf des Produkts</p>");
            template.AppendLine("<h1>@Model.Name</h1>");
            template.AppendLine("mit folgenden Optionen:<li>");
            template.AppendLine("@foreach(var option in @Model.Options) { <ul>@option.Name</ul> } ");
            template.AppendLine("<p></li>erhalten.</p>");
            template.AppendLine("<h5>MfG, TEST-FUCHS GmbH</h5>");

            Email.DefaultSender = sender;
            Email.DefaultRenderer = new RazorRenderer();

            var email = Email
                .From("tobias.scherzer31@gmail.com")
                .To("tobias.scherzer31@gmail.com")
                .Subject("Supa")       
                .UsingTemplate(template.ToString(), product)                                      //Komplette Email
                //.Body("Supa host as gmocht bro")
                .Send();

            return "";
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}