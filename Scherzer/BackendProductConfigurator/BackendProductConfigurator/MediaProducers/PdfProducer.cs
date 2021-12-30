using Model;
using PdfSharp.Drawing;
using PdfSharp.Pdf;

namespace BackendProductConfigurator.MediaProducers
{
    public static class PdfProducer
    {
        private static void InitiatePdfProducer()
        {
            System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);
        }
        public static void GeneratePDF(Product product)
        {
            InitiatePdfProducer();
            PdfDocument document = new PdfDocument();
            PdfPage page = document.AddPage();
            XGraphics gfx = XGraphics.FromPdfPage(page); //Holt sich seitenspezifische Details für die Zeichenmethoden

            XFont font = new XFont("Century Gothic", 14);
            XFont headerFont = new XFont("Century Gothic", 40);
            XFont smallDetailFont = new XFont("Century Gothic", 11);

            gfx.DrawString($"Produkt #{product.Id}",
                           headerFont,
                           XBrushes.Black,
                           new XPoint(200, 70));

            gfx.DrawString($"{product.Name}",
                           font,
                           XBrushes.DarkGray,
                           new XPoint(230, 90));

            gfx.DrawLine(new XPen(XColor.FromArgb(0,0,0)),
                         new XPoint(page.Width * 0.2, 120),
                         new XPoint(page.Width - page.Width * 0.2, 120));

            document.Save($"./product{product.Id}.pdf");
        }
    }
}
