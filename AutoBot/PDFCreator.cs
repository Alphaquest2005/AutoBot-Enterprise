using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using ExcelDataReader;
using iText.Kernel.Geom;
using iText.Kernel.Pdf;
using iText.Layout;
using iText.Layout.Element;
using iText.Layout.Layout;
using iText.Layout.Properties;
using iText.Layout.Renderer;

namespace AutoBot;

public class PDFCreator<T>
{
    //public static void CreatePDF(List<T> list, string path)
    //{
    //    if (list == null || list.Count == 0) return;

    //    Document doc = new Document(CalculatePageSize(list, 12, 10));
    //    PdfWriter.GetInstance(doc, new FileStream(path, FileMode.Create));
    //    doc.Open();

    //    PdfPTable table = new PdfPTable(typeof(T).GetProperties().Length);
    //    foreach (var prop in typeof(T).GetProperties())
    //    {
    //        table.AddCell(prop.Name);
    //    }

    //    foreach (var item in list)
    //    {
    //        foreach (var prop in typeof(T).GetProperties())
    //        {
    //            table.AddCell(prop.GetValue(item)?.ToString() ?? "");
    //        }
    //    }

    //    doc.Add(table);
    //    doc.Close();
    //}

    public void CreatePDF(List<T> data, string fileName)
    {
        try
        {

            var fontSize = 12f;
            float footerSpace = 20;
            // Calculate the maximum width of the content in each column
            var columnWidths = new List<float>();
            foreach (var prop in typeof(T).GetProperties())
            {
                float maxWidth = 0;
                foreach (var item in data)
                {
                    var content = prop.GetValue(item)?.ToString() ?? "";
                    var width = content.Length * fontSize;  // Simplified width calculation
                    if (width > maxWidth)
                    {
                        maxWidth = width;
                    }
                }
                columnWidths.Add(maxWidth);
            }

            // Calculate the total width and height of the page
            float totalWidth = columnWidths.Sum() + 50;  // Add some margin
           
            float totalHeight = (data.Count + 3) * fontSize * 2 + footerSpace;  // Assume each row is twice the font size

            // Create a new PDF document with the calculated page size
            PdfWriter writer = new PdfWriter(fileName);
            PdfDocument pdf = new PdfDocument(writer);
            Document document = new Document(pdf, new PageSize(totalWidth, totalHeight));

            // Create a new Table with the calculated column widths
            Table table = new Table(UnitValue.CreatePointArray(columnWidths.ToArray()));

            // Set a custom renderer to control page breaks
           // table.SetNextRenderer(new CustomTableRenderer(table, new Table.RowRange(0, data.Count), fontSize));

            // Add headers to the table from the public properties of T
            foreach (var prop in typeof(T).GetProperties())
            {
                Cell cell = new Cell().Add(new Paragraph(prop.Name).SetFontSize(fontSize));
                cell.SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                table.AddHeaderCell(cell);
            }

            // Add data to the table from the list of T
            foreach (var item in data)
            {
                foreach (var prop in typeof(T).GetProperties())
                {
                    Cell cell = new Cell().Add(new Paragraph(prop.GetValue(item)?.ToString() ?? "").SetFontSize(fontSize));
                    cell.SetBorder(iText.Layout.Borders.Border.NO_BORDER);
                    table.AddCell(cell);
                }
            }

            // Add the table to the document
            document.Add(table);

            // Close the PDF document
            document.Close();





        }
        catch (Exception e)
        {
            Console.WriteLine(e);
            throw;
        }
    }

    public class CustomTableRenderer : TableRenderer
    {
        private float fontSize;

        public CustomTableRenderer(Table modelElement, Table.RowRange rowRange, float fontSize) : base(modelElement, rowRange)
        {
            this.fontSize = fontSize;
        }

        public override LayoutResult Layout(LayoutContext layoutContext)
        {
            LayoutResult result = base.Layout(layoutContext);

            if (result.GetStatus() == LayoutResult.PARTIAL && result.GetOverflowRenderer().GetOccupiedArea().GetBBox().GetHeight() <= 2 * fontSize)
            {
                return new LayoutResult(LayoutResult.NOTHING, null, null, this);
            }

            return result;
        }
    }

}