using System;
using System.IO;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using System.Diagnostics;

    public class splitPDFUA {

        public const String DEST = "output.pdf";

        public static void Main(String[] args) {
            if (args.Length != 1)
            {
                Console.WriteLine("usage: dotnet run <filename>");
            } else {
                FileInfo file = new FileInfo(DEST);
                new splitPDFUA().CreatePdf(DEST, args[0]);
            }
        }

        public virtual void CreatePdf(String dest, String src) {
            Stopwatch watch = new Stopwatch();
            Console.WriteLine("open input: " + src);
            watch.Start();
            PdfDocument SourcePdf = new PdfDocument(new PdfReader(src));
            watch.Stop();
            Console.WriteLine(" => Time: " +watch.Elapsed);
            int n_pages = SourcePdf.GetNumberOfPages();
            int n_add_pages = 0;
            for (int i = 1; i<= n_pages; i=i+5000){
                 if (n_pages -i < 5000){
                    n_add_pages = n_pages - i;
                }
                else{
                    n_add_pages = 5000;
                }

                Console.WriteLine("  pages " + i + " - " +(i + n_add_pages));

                watch.Start();
                PdfDocument pdf = new PdfDocument(new PdfWriter((i + "_" + dest), new WriterProperties().AddUAXmpMetadata()));
                Document document = new Document(pdf);
                //Setting some required parameters
                pdf.SetTagged();
                pdf.GetCatalog().SetLang(new PdfString("en-US"));
                pdf.GetCatalog().SetViewerPreferences(new PdfViewerPreferences().SetDisplayDocTitle(true));
                PdfDocumentInfo info = pdf.GetDocumentInfo();
                info.SetTitle("split PDF/UA example pages " + i + "-" +(i+n_add_pages));

                SourcePdf.CopyPagesTo(i,i+n_add_pages, pdf, 1); 
                document.Close();
                watch.Stop();
                Console.WriteLine(" => Time: " +watch.Elapsed);
            }
            
        }
    }