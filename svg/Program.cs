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
using PDFlib_dotnet;

namespace ConsoleApp
{
    class Program
    {

        static void itextProcessSVG(string inputFile, string outputFile)
        {
            try
            {
                PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new FileStream(outputFile, FileMode.Create, FileAccess.Write)));
                Document doc = new Document(pdfDocument);
                pdfDocument.AddNewPage(PageSize.A4);
                try
                {
                    FileStream svgPath = File.Open(inputFile, FileMode.Open);
                    Image image = SvgConverter.ConvertToImage(svgPath, pdfDocument);
                    image.SetFixedPosition(0, 0);
                    image.ScaleToFit(PageSize.A4.GetWidth(), PageSize.A4.GetHeight());
                    //image.ScaleAbsolute(595, 842);
                    doc.Add(image);
                }
                catch(Exception e){
                    doc.Add( new Paragraph(e.Message));
                    Console.WriteLine(inputFile + " iText Exception: " + e.Message);
                }
                doc.Close();
            }
            catch(Exception e)
            {
                Console.WriteLine(" iText Exception: " + e.Message);
            }
        }
        static void pdflibProcessSVG(string inputFile, string itextoutput, string outputFile)
        {
            PDFlib p;
            int graphics;
            string message = "";
            byte[] buf = new byte[0];
            try
            {
                p = new PDFlib();

                /* This means we must check return values of load_graphics() etc. */
                p.set_option("errorpolicy=return");

                p.begin_document("", "");
                graphics = p.load_graphics("auto", inputFile, "");
                p.begin_page_ext(0, 0, "width=a4.width height=a4.height");
                p.fit_graphics(graphics, 0, 0, "boxsize {595 842} fitmethod {meet} position {left center}");
                p.end_page_ext("");
                p.end_document("");
                buf = p.get_buffer();
            }
            catch (PDFlibException e)
            {
                message = e.get_errmsg();
            } 


            int doc, page;
            try
            {
                p = new PDFlib();
                p.create_pvf("/pvf/input.pdf", buf, "");
                p.begin_document(outputFile, "");
                p.begin_page_ext(1190, 842, "");
                doc = p.open_pdi_document(itextoutput, "");
                page = p.open_pdi_page(doc, 1,"");
                p.fit_pdi_page(page, 0,0, "boxsize {595 842}");
                p.close_pdi_page(page);
                p.close_pdi_document(doc);
                p.fit_textline("iText 8.0", 297, 0, "fontname=Helvetica-Bold fontsize 24 noembedding fillcolor=red position {50 0}");
                if (message == ""){
                    doc = p.open_pdi_document("/pvf/input.pdf", "");
                    page = p.open_pdi_page(doc, 1,"");
                    p.fit_pdi_page(page, 595,0, "boxsize {595 842}");
                    p.close_pdi_page(page);
                    p.close_pdi_document(doc);
                }
                else
                {
                    p.fit_textline(message, 639, 700, "fontname=Helvetica-Bold fontsize 24 noembedding");

                }
                p.fit_textline("PDFlib 10.0", 892, 0, "fontname=Helvetica fontsize 24 noembedding fillcolor=red position {50 0}");
                p.end_page_ext("");
                p.end_document("");
            }
            catch (PDFlibException e)
            {
                message = e.get_errmsg();
            }
 


        }
        static void Main(string[] args)
        {
            String? line;
            try
            {
                if (args.Length != 1)
                {
                    Console.WriteLine("usage: dotnet run <listfile>");
                    return;
                }
                //Pass the file path and file name to the StreamReader constructor
                StreamReader sr = new StreamReader(args[0]);
                //Read the first line of text
                line = sr.ReadLine();
                //Continue to read until you reach end of file
                while (line != null)
                {
                    if (line.StartsWith("#"))
                    {
                        Console.WriteLine(line + " skipped");
                    }
                    else
                    {
                        itextProcessSVG(line, "output/" + System.IO.Path.GetFileName(line) + ".pdf");
                        pdflibProcessSVG(line, "output/" + System.IO.Path.GetFileName(line) + ".pdf", "output2/" + System.IO.Path.GetFileName(line) + ".pdf");
                    }
                    line = sr.ReadLine();
                }
            }
            catch(Exception e)
            {
                Console.WriteLine("Main Exception: " + e.Message);
            }


        }
    }
}
