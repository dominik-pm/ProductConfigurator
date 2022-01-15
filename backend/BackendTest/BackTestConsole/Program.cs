using PdfSharp.Drawing;
using PdfSharp.Pdf;

System.Text.Encoding.RegisterProvider(System.Text.CodePagesEncodingProvider.Instance);

PdfDocument document = new PdfDocument();

PdfPage page = document.AddPage();

XGraphics gfx = XGraphics.FromPdfPage(page); //Holt sich seitenspezifische Details für die Zeichenmethoden

XFont font = new XFont("Century Gothic", 14);

gfx.DrawString("Produkt Id",
               new XFont("Arial", 40, XFontStyle.Bold),
               XBrushes.Black,
               new XPoint(200, 70));

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
