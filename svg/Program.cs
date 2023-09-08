using System;
using System.IO;
using System.Net;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.IO;
using iText.Svg.Converter;
using iText.Svg.Processors;
using iText.Svg.Processors.Impl;
using iText.Kernel.Pdf.Xobject;
using iText.Layout;
using iText.Layout.Element;
namespace ConsoleApp1
{
    class Program
    {
        private static String SVG_FILE = "input.svg";
        private static String OUTPUT_FILE = "result2.pdf";

        static void Main(string[] args)
        {
          PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new FileStream(OUTPUT_FILE, FileMode.Create, FileAccess.Write)));
          Document doc = new Document(pdfDocument);
          pdfDocument.AddNewPage(PageSize.A4);
          ISvgConverterProperties properties = new SvgConverterProperties().SetBaseUri(SVG_FILE);
          //SvgConverter.DrawOnDocument(new FileStream(SVG_FILE, FileMode.Open, FileAccess.Read, FileShare.Read), doc, 1, properties);
          FileStream svgPath = File.Open(SVG_FILE, FileMode.Open);
          Image image = SvgConverter.ConvertToImage(svgPath, pdfDocument);
          image.SetFixedPosition(0, 400);
          image.ScaleToFit(500, 360);
          doc.Add(image);
          doc.Close();
        }
    }
}
