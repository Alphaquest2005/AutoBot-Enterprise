using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using OCR.Business.Entities;
using pdf_ocr;

namespace AutoBotUtilities
{
    public class InvoiceReader
    {
        public static void Import(string file)
        {
            //Get Text
            var pdftxt = PdfOcr.Ocr(file);
            // Get Template
            using (var ctx = new OCRContext())
            {
                var templates = ctx.Templates
                                .Include(x => x.TemplateLines)
                                .Include(x => x.TemplateRegularExpressions)
                                .ToList();
                foreach (var tmp in templates)
                {
                    
                }
            }
        }
    }
}
