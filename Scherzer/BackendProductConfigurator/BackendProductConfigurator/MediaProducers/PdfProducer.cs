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
        public static void GeneratePDF(ConfiguredProduct product)
        {
            InitiatePdfProducer();
            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page); //Holt sich seitenspezifische Details für die Zeichenmethoden
            XTextFormatter tf = new XTextFormatter(gfx); //Um Text besser zu formatieren
            
            XFont font = new XFont("Century Gothic", 14);
            XFont headerFont = new XFont("Century Gothic", 40);
            XFont smallDetailFont = new XFont("Century Gothic", 11);

            tf.Alignment = XParagraphAlignment.Center;
            tf.DrawString($"{product.ConfigurationName}",
                           headerFont,
                           XBrushes.Black,
                           new XRect(0, 20, page.Width, 40));

            tf.DrawString($"Produkt #{product.ConfiguratorId}",
                           font,
                           XBrushes.DarkGray,
                           new XRect(0, 70, page.Width, 20));

            gfx.DrawLine(new XPen(XColor.FromArgb(0,0,0)),
                         new XPoint(page.Width * 0.2, 100),
                         new XPoint(page.Width - page.Width * 0.2, 100));

            tf.Alignment = XParagraphAlignment.Left;
            tf.DrawString($"Ausgewählte Optionen:",
                           font,
                           XBrushes.Black,
                           new XRect(page.Width * 0.2, 110, page.Width * 0.6, 20));

            int yPosition = 140;
            foreach(Option option in product.Options)
            {
                tf.DrawString($"- {option.Name}",
                           font,
                           XBrushes.Black,
                           new XRect(page.Width * 0.24, yPosition, page.Width * 0.6, 20));
                yPosition += 30;
            }

            document.Save($"./product{product.ConfiguratorId}.pdf");
        }
    }
}
