using BackendProductConfigurator.App_Code;
using BackendProductConfigurator.Controllers;
using Model;
using PdfSharp.Drawing;
using PdfSharp.Drawing.Layout;
using PdfSharp.Pdf;
using System.Text;

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
            XGraphics gfx = XGraphics.FromPdfPage(page);
            XTextFormatter tf = new XTextFormatter(gfx);
            Configurator configurator = ValuesClass.Configurators["de"].Where(con => con.ConfigId == configId).First();
            Option tempOption;
            double smallSpacing = 24;
            double mediumSpacing = 30;
            double largeSpacing = 50;
            double yPosition = 20;
            double leftBorder = 0.1;

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

            DrawLine(yPosition, leftBorder, gfx, page);

            yPosition += smallSpacing;

            PrintOption(tf, font, page, page.Width * leftBorder, page.Width * 0.66, yPosition, "Basispreis:", configurator.Rules.BasePrice);

            yPosition += smallSpacing;

            DrawLine(yPosition, leftBorder, gfx, page);

            yPosition += smallSpacing;

            tf.Alignment = XParagraphAlignment.Left;
            tf.DrawString($"Ausgewählte Optionen:",
                           font,
                           XBrushes.Black,
                           new XRect(page.Width * leftBorder, yPosition, page.Width * 0.6, 20));

            yPosition += smallSpacing;
            float price;
            foreach(string optionId in product.Options)
            {
                tempOption = configurator.Options.Where(o => o.Id == optionId).First();
                try
                {
                    price = configurator.Rules.PriceList[tempOption.Id];
                }
                catch
                {
                    price = 0f;
                }
                PrintOption(tf, font, page, page.Width * (leftBorder + 0.04), page.Width * 0.66, yPosition, $"- {tempOption.Name}", price);
                yPosition += smallSpacing;
            }

            DrawLine(yPosition, leftBorder, gfx, page);

            yPosition += smallSpacing;

            PrintOption(tf, font, page, page.Width * leftBorder, page.Width * 0.66, yPosition, "Summe:", product.Price);

            DateTime dateTime = DateTime.Now;

            StringBuilder saveName = new StringBuilder($"{GlobalValues.PDFOutput}/Product");
            saveName.Append('_').Append(configId).Append('_');
            saveName.Append(dateTime.Year);
            saveName.Append(dateTime.Month.ToString().PadLeft(2, '0'));
            saveName.Append(dateTime.Day.ToString().PadLeft(2, '0'));
            saveName.Append('_');
            saveName.Append(dateTime.Hour.ToString().PadLeft(2, '0'));
            saveName.Append(dateTime.Minute.ToString().PadLeft(2, '0'));
            saveName.Append(dateTime.Second.ToString().PadLeft(2, '0'));
            saveName.Append(dateTime.Millisecond.ToString().PadLeft(3, '0'));
            saveName.Append(".pdf");

            document.Save(saveName.ToString());
        }

        private static double DrawImage(XGraphics gfx, string imgLoc, double x, double y, PdfPage page)
        {
            XImage image;
            try
            {
                image = XImage.FromFile($"{GlobalValues.ImagesFolder}/{imgLoc.Replace('*', '/')}");
            }
            catch
            {
                throw new FileNotFoundException($"Image of file doesn't exist on path: {imgLoc.Replace('*', '/')}");
            }
            double width = page.Width * 0.5;
            double height = (image.Height / image.Width) * width;
            gfx.DrawImage(image, x, y, width, height);
            return height + 15;
        }

        private static void PrintOption(XTextFormatter tf, XFont font, PdfPage page, double x1, double x2, double y, string leftText, float price)
        {
            tf.Alignment = XParagraphAlignment.Left;
            tf.DrawString(leftText,
                       font,
                       XBrushes.Black,
                       new XRect(x1, y, page.Width * 0.7, 20));

            tf.Alignment = XParagraphAlignment.Right;
            tf.DrawString($"{price}€",
                       font,
                       XBrushes.Black,
                       new XRect(x2, y, page.Width * 0.2, 20));
            tf.Alignment = XParagraphAlignment.Left;
        }
        private static void DrawLine(double yPosition, double leftBorder, XGraphics gfx, PdfPage page)
        {
            gfx.DrawLine(new XPen(XColor.FromArgb(0, 0, 0)),
                         new XPoint(page.Width * leftBorder, yPosition),
                         new XPoint(page.Width - page.Width * leftBorder, yPosition));
        }
    }
}
