using System;
using System.IO;
using iText.IO.Font;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Kernel.Colors;

class Program
{
  public static readonly String FONT = "../data/NotoSerif-Regular.ttf";
  static void Main() {

    PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new FileStream("hello.pdf", FileMode.Create, FileAccess.Write)));
    Document document = new Document(pdfDocument);

    PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.IDENTITY_H);
    Paragraph par = new Paragraph("Hello world!\n(says C)");
    Color colorRGB = new DeviceRgb(255, 0, 255);

    par.SetFixedPosition((float)50.0, (float)676.0, 500);
    par.SetFontColor(colorRGB);
    par.SetFont(font);
    par.SetFontSize(24);
    par.SetFixedLeading(24);
    document.Add(par);
    document.Close();
    Console.WriteLine("Awesome PDF just got created.");
  }
}