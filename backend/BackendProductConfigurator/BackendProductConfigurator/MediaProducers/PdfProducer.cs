using BackendProductConfigurator.Controllers;
using Model;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;

namespace BackendProductConfigurator.MediaProducers
{
    public static class PdfProducer
    {
        private static void InitiatePdfProducer()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }
        public static void GeneratePDF(ConfiguredProduct product, string configId, HttpRequest request)
        {
            InitiatePdfProducer();
            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page); //Holt sich seitenspezifische Details für die Zeichenmethoden
            XTextFormatter tf = new XTextFormatter(gfx); //Um Text besser zu formatieren
            Configurator configurator = AValuesClass.Configurators[AController<object, object>.GetAccLang(request)].Find(con => con.ConfigId == configId);
            Option tempOption;
            double smallSpacing = 24;
            double mediumSpacing = 30;
            double largeSpacing = 50;
            double yPosition = 20;

            XFont font = new XFont("Century Gothic", 14);
            XFont headerFont = new XFont("Century Gothic", 40);
            XFont smallDetailFont = new XFont("Century Gothic", 11);

            tf.Alignment = XParagraphAlignment.Center;
            tf.DrawString($"{product.ConfigurationName}",
                           headerFont,
                           XBrushes.Black,
                           new XRect(0, yPosition, page.Width, 40));

            yPosition += largeSpacing;

            tf.DrawString($"{configurator.Name} - Konfigurator",
                           font,
                           XBrushes.DarkGray,
                           new XRect(0, yPosition, page.Width, 20));

            yPosition += mediumSpacing;

            yPosition += DrawImage(gfx, configurator.Images[0], page.Width * 0.25, yPosition, page);

            DrawLine(yPosition, gfx, page);

            yPosition += smallSpacing;

            PrintOption(tf, font, page, page.Width * 0.2, page.Width * 0.56, yPosition, "Basispreis:", configurator.Rules.BasePrice);

            yPosition += smallSpacing;

            DrawLine(yPosition, gfx, page);

            yPosition += smallSpacing;

            tf.Alignment = XParagraphAlignment.Left;
            tf.DrawString($"Ausgewählte Optionen:",
                           font,
                           XBrushes.Black,
                           new XRect(page.Width * 0.2, yPosition, page.Width * 0.6, 20));

            yPosition += smallSpacing;
            foreach(string optionId in product.Options)
            {
                tempOption = configurator.Options.Where(o => o.Id == optionId).First();
                PrintOption(tf, font, page, page.Width * 0.24, page.Width * 0.56, yPosition, $"- {tempOption.Name}", configurator.Rules.PriceList[tempOption.Id]);
                yPosition += smallSpacing;
            }

            DrawLine(yPosition, gfx, page);

            yPosition += smallSpacing;

            PrintOption(tf, font, page, page.Width * 0.2, page.Width * 0.56, yPosition, "Summe:", product.Price);

            DateTime dateTime = DateTime.Now;
            document.Save($"./Product{dateTime.Year}{dateTime.Month}{dateTime.Day}_{dateTime.Hour}{dateTime.Minute}{dateTime.Second}{dateTime.Millisecond}.pdf");
        }

        private static double DrawImage(XGraphics gfx, string imgLoc, double x, double y, PdfPage page)
        {
            XImage image = XImage.FromFile(imgLoc);
            double width = page.Width * 0.5;
            double height = (image.Height / image.Width) * width; //Um Bildformat nicht zu zerstören
            gfx.DrawImage(image, x, y, width, height);
            return height + 15;
        }

        private static void PrintOption(XTextFormatter tf, XFont font, PdfPage page, double x1, double x2, double y, string leftText, float price)
        {
            tf.Alignment = XParagraphAlignment.Left;
            tf.DrawString(leftText,
                       font,
                       XBrushes.Black,
                       new XRect(x1, y, page.Width * 0.2, 20));

            tf.Alignment = XParagraphAlignment.Right;
            tf.DrawString($"{price}€",
                       font,
                       XBrushes.Black,
                       new XRect(x2, y, page.Width * 0.2, 20));
            tf.Alignment = XParagraphAlignment.Left;
        }
        private static void DrawLine(double yPosition, XGraphics gfx, PdfPage page)
        {
            gfx.DrawLine(new XPen(XColor.FromArgb(0, 0, 0)),
                         new XPoint(page.Width * 0.2, yPosition),
                         new XPoint(page.Width - page.Width * 0.2, yPosition));
        }
    }
}
