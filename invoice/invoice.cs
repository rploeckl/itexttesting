using System;
using System.IO;
using System.Text;
using iText.IO.Font;
using iText.IO.Image;
using iText.Kernel.Colors;
using iText.Kernel.Font;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Properties;
using System.Globalization;

struct articledata
{


    public string name;
    public float price;
    public int quantity;

    // Constructor:
    public articledata(string name, float price, int quantity)
    {
        this.name = name;
        this.price = price;
        this.quantity = quantity;
    }
}

class Program
{
    public static readonly String FONT = "../data/NotoSerif-Regular.ttf";
    public static String senderfull =
                "17, Aviation Road\n" +
                "Paperfield\n" +
                "Phone 7079 4301\n" +
                "Fax 7079 4302\n" +
                "info@kraxi.com\n" +
                "www.kraxi.com\n";
    public static float x_table = 55;
    public static float tablewidth = 475;

    public static float y_address = 682;
    public static float x_salesrep = 455;
    public static float y_invoice = 542;
    public static float y_invoice_p2 = 800;
    public static float bottom = 40;
    public static float imagesize = 90;

    public static float fontsize = 11;
    public static float fontsizesmall = 9;

    public static String salesrepfilename = "../data/sales_rep4.jpg";
    public static String salesrepname = "Lucy Irwin";
    public static String salesrepcaption = "Local sales rep:";

    public static String invoiceheader = "INVOICE 2012-03";

    public static string closingtext =
        "Terms of payment: <fillcolor={rgb 1 0 0}>30 days net. " +
        "<fillcolor={gray 0}>90 days warranty starting at the day of sale. " +
        "This warranty covers defects in workmanship only. " +
        "Kraxi Systems, Inc. will, at its option, repair or replace the " +
        "product under the warranty. This warranty is not transferable. " +
        "No returns or exchanges will be accepted for wet products.";



    public static string[] headers = {
        "ITEM", "DESCRIPTION", "QUANTITY", "PRICE", "AMOUNT"
    };

    static void Main() {
        System.DateTime ltime;
        StringBuilder date = new StringBuilder();
        articledata[] dataset = {
            new articledata( "Super Kite",  20, 2),
            new articledata( "Turbo Flyer", 40, 5),
            new articledata( "Giga Trash", 180, 1),
            new articledata( "Bare Bone Kit", 50, 3),
            new articledata( "Nitty Gritty", 20, 10),
            new articledata( "Pretty Dark Flyer", 75, 1),
            new articledata( "Free Gift", 0, 1),
        };
        StringBuilder buf = new StringBuilder();
        CultureInfo culture = new CultureInfo("en-US");

        PdfDocument pdfDocument = new PdfDocument(new PdfWriter(new FileStream("invoice.pdf", FileMode.Create, FileAccess.Write)));
        Document document = new Document(pdfDocument);

        PdfFont font = PdfFontFactory.CreateFont(FONT, PdfEncodings.IDENTITY_H);
        /* ---------------------------------e
         * Print full company contact details
         * -----------------------------------
         */
        Paragraph par = new Paragraph(senderfull);
        Color colorCmyk = new DeviceCmyk(64,55,52,27);
        par.SetFontColor(colorCmyk)
            .SetFont(font)
            .SetFontSize(fontsize)
            .SetFixedLeading((float)(fontsize*1.2))
            .SetHeight(150)
            .SetWidth(imagesize);
        document.ShowTextAligned(par, x_salesrep, y_address, TextAlignment.LEFT, VerticalAlignment.BOTTOM);

        /* -----------------------------------
         * Place address and header text
         * -----------------------------------
         */
        String address =
            "John Q. Doe\n" +
            "255 Customer Lane\n" +
            "Suite B\n" +
            "12345 User Town\n" +
            "Everland";

        string[] months = { "January", "February", "March", "April",
                            "May", "June", "July", "August", "September",
                            "October", "November", "December" };


        /* -----------------------------------
         * Print address
         * -----------------------------------
         */
        Paragraph pAdd = new Paragraph(address);
        pAdd.SetFixedLeading((float)(fontsize*1.2))
            .SetFont(font)
            .SetFontSize(fontsize)
            .SetWidth(tablewidth/2)
            .SetHeight(100);

        /* -----------------------------------
         * Place name and image of local sales rep
         * -----------------------------------
         */
        Paragraph pSalesRepCaption = new Paragraph(salesrepcaption);
        pSalesRepCaption.SetFont(font)
                        .SetFontSize(fontsizesmall);
        document.ShowTextAligned(pSalesRepCaption, x_salesrep, y_address-fontsizesmall, TextAlignment.LEFT);

        Paragraph pSalesRepName = new Paragraph(salesrepname);
        pSalesRepName.SetFont(font)
                     .SetFontSize(fontsizesmall);
        document.ShowTextAligned(pSalesRepName, x_salesrep, y_address-2*fontsizesmall, TextAlignment.LEFT);

        document.ShowTextAligned(pAdd, x_table, y_address-100, TextAlignment.LEFT, VerticalAlignment.BOTTOM);
        Image image = new Image(ImageDataFactory.Create(salesrepfilename));
        image.SetFixedPosition(x_salesrep, y_address-3*fontsizesmall-imagesize)
             .ScaleToFit(imagesize, imagesize);
        document.Add(image);

        /* -----------------------------------
         * Print the header and date
         * -----------------------------------
         */
        /* Add a bookmark with the header text */

        ltime = System.DateTime.Now;
        date.Length = 0;
        date.AppendFormat("{0} {1}, {2}", months[ltime.Month-1], ltime.Day, ltime.Year);
        Paragraph pInvoiceDate = new Paragraph(date.ToString());
        pInvoiceDate.SetFont(font)
                     .SetFontSize(fontsize);
        document.ShowTextAligned(pInvoiceDate, x_table+ tablewidth, y_invoice, TextAlignment.RIGHT);

        Table table = new Table(5);
        table.SetWidth(UnitValue.CreatePercentValue(80));
        for (int col = 1; col <= headers.Length; col++)
        {
            table.AddHeaderCell(headers[col-1]);
        }
        /* ---------- Data rows: one for each article */
        float total = 0;
        int row = 1;

        for (int i = 0; i <  dataset.Length; i++) {
            float sum = dataset[i].quantity * dataset[i].price;
            /* column 1: ITEM */
            Cell cellitem= new Cell();
            cellitem.Add(new Paragraph(row.ToString()));
            cellitem.SetTextAlignment(TextAlignment.RIGHT);

            table.AddCell(cellitem);

            /* column 2: DESCRIPTION */
            Cell cellDesc= new Cell();
            cellDesc.Add(new Paragraph(dataset[i].name.ToString()));

            table.AddCell(cellDesc);

            /* column 3: QUANTITY */
            Cell cellQuantity= new Cell();
            cellQuantity.Add(new Paragraph(dataset[i].quantity.ToString()));
            cellQuantity.SetTextAlignment(TextAlignment.RIGHT); 

            table.AddCell(cellQuantity);

            /* column 4: PRICE */
            buf.Length = 0;
            buf.AppendFormat(culture, "{0:F2}", dataset[i].price);
            Cell cellPrice= new Cell();
            cellPrice.Add(new Paragraph(buf.ToString()));
            cellPrice.SetTextAlignment(TextAlignment.RIGHT);
            table.AddCell(cellPrice);

            /* column 5: AMOUNT */
            Cell cellAmount= new Cell();
            cellAmount.Add(new Paragraph(sum.ToString()));
            cellAmount.SetTextAlignment(TextAlignment.RIGHT);
            table.AddCell(cellAmount);
            row++;

        }
        table.SetWidth(tablewidth)
            .SetHeight(y_invoice-3*fontsize-20)
            .SetFont(font);

        document.Add(table);


        document.Close();
    }
}