using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using iTextSharp.text.pdf;
using iTextSharp.text.pdf.parser;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.util;

namespace Core.Common.PDF2TXT
{
    public class Pdf2Txt
    {
         private static readonly Pdf2Txt instance;
         static Pdf2Txt()
        {


            instance = new Pdf2Txt();
            
        }

         public static Pdf2Txt Instance
        {
            get { return instance; }
        }

        public string ExtractTextFromPdf(string path)
        {
            // ITextExtractionStrategy its = new LocationTextExtractionStrategy();

            //using (var reader = new PdfReader(path))
            //{
            //    var text = new StringBuilder();

            //    for (int i = 1; i <= reader.NumberOfPages; i++)
            //    {
            //        text.Append(PdfTextExtractor.GetTextFromPage(reader, i));
            //    }

            //    return  text.ToString();
            //}

            PDDocument doc = null;
            try
            {
                doc = PDDocument.load(path);
                var stripper = new PDFTextStripper();
                return stripper.getText(doc);
            }
            catch (Exception ex)
            {
                throw;
            }
            finally
            {
                if (doc != null)
                {
                    doc.close();
                }
            }

        }
    }
}
