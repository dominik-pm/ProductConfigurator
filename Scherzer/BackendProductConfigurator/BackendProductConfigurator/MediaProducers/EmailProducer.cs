using FluentEmail.Core;
using FluentEmail.Razor;
using FluentEmail.Smtp;
using Model;
using System.Net.Mail;
using System.Text;

namespace BackendProductConfigurator.MediaProducers
{
    public static class EmailProducer
    {
        public static SmtpSender Sender { get; set; }
        public static StringBuilder Template { get; set; }
        private static void InitiateSender()
        {
            Sender = new SmtpSender(() => new SmtpClient("localhost")
            {
                EnableSsl = false, //Zum Testen ausschalten
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Port = 25
            }
            ); //localhost ist der Empfangsserver --> Für Gmail die Adresse von Gmail einfügen
        }

        private static void CreateRenderContent(StringBuilder template, EValidationResult validationResult)
        {
            switch(validationResult)
            {
                case EValidationResult.ValidationPassed:
                    template.AppendLine("<p>wir haben ihre Bestellung des Produkts</p>");
                    template.AppendLine("<h1>@Model.Name</h1>");
                    template.AppendLine("mit folgenden Optionen:<ul>");
                    template.AppendLine("@foreach(var option in @Model.Options) { <li>@option.Name</li> } ");
                    template.AppendLine("<p></ul>erhalten.</p>");
                    break;

                case EValidationResult.PriceInvalid:
                    template.AppendLine("<p>unglücklicherweise ist uns ein Fehler bei der Preisberechnung unterlaufen.</p>");
                    template.AppendLine("<p>Wir bitten um Verständnis. Versuchen Sie es noch einmal. Wenn der Fehler wieder vorkommt</p>");
                    template.AppendLine("<h5>Kontaktieren Sie den Kundensupport</h5>");
                    break;

                case EValidationResult.ConfigurationInvalid:
                    template.AppendLine("<p>unglücklicherweise ist Ihnen ein Fehler bei der Konfiguration unterlaufen.</p>");
                    template.AppendLine("<p>Bitte versuchen Sie noch einmal eine Konfiguration zu bestellen. Wenn der Fehler wieder vorkommt</p>");
                    template.AppendLine("<h5>Kontaktieren Sie den Kundensupport</h5>");
                    break;
            }
        }

        private static void InitiateRendering(EValidationResult validationResult)
        {
            Template = new StringBuilder();
            Template.AppendLine("Sehr geehrte/r Kunde/in,");
            CreateRenderContent(Template, validationResult);
            Template.AppendLine("<h5>MfG, TEST-FUCHS GmbH</h5>");

            Email.DefaultSender = Sender;
            Email.DefaultRenderer = new RazorRenderer();
        }

        public static void SendEmail(Product product, EValidationResult validationResult)
        {
            InitiateSender();
            InitiateRendering(validationResult);
            var email = Email
                .From("tobias.scherzer31@gmail.com")
                .To("tobias.scherzer31@gmail.com")
                .Subject("Supa")
                .UsingTemplate(Template.ToString(), product)
                .Send();
        }
    }
}
