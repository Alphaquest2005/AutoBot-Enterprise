using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EntryDataDS.Business.Entities;
using MimeKit;
using MoreLinq;
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

            //Get Template
            using (var ctx = new OCRContext())
            {
                var templates = ctx.Invoices
                                .Include(x => x.Parts)
                                .Include("Parts.Start.RegExChain.RegularExpressions")
                                .Include("Parts.End.RegExChain.RegularExpressions")
                                .Include("Parts.PartTypes")
                                .Include("Parts.ChildParts.Part.Start.RegExChain.RegularExpressions")
                                .Include("Parts.ParentPart.Part.Start.RegExChain.RegularExpressions")
                                .Include("Parts.Lines.RegExChain.RegularExpressions")
                                .Include("Parts.Lines.Fields")
                                .ToList()
                                .Select(x => new Invoice(x));
                                
                foreach (var tmp in templates)
                {
                    tmp.Read(pdftxt.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList());
                }
            }
        }
    }

    public class Invoice
    {
        private EntryData EntryData { get; } = new EntryData();
        private Invoices OcrInvoices { get; }
        private List<Part> Parts { get; set; }

        public Invoice(Invoices ocrInvoices)
        {
            OcrInvoices = ocrInvoices;
            Parts = ocrInvoices.Parts.Select(z => new Part(z)).ToList();
        }

        public EntryData Read(List<string> text)
        {
            var lineCount = 0;
            foreach (var line in text)
            {
                lineCount += 1;
                var iLine = new InvoiceLine(line, lineCount);
                Parts.ForEach(x => x.Read(iLine));
            }

            return EntryData;
        }

        
    }

    public class InvoiceLine
    {
        public string Line { get; }
        public int LineNumber { get; }

        public InvoiceLine(string line, int lineNumber)
        {
            Line = line;
            LineNumber = lineNumber;
        }
    }

    public class Part
    {
        private readonly List<InvoiceLine> _startlines = new List<InvoiceLine>();
        private readonly List<InvoiceLine> _endlines = new List<InvoiceLine>();
        private readonly List<InvoiceLine> _bodylines = new List<InvoiceLine>();
        private Parts _part;

        public Part(Parts part)
        {
             StartCount = part.Start.Select(x => x.RegExChain.RegularExpressions.RegEx).Count();
             EndCount = part.End.Select(x => x.RegExChain.RegularExpressions.RegEx).Count();
            _part = part;
            ChildParts = part.ChildParts.Select(x => new Part(x.Part)).ToList();
            Lines = part.Lines.Select(x => new Line(x)).ToList();

        }

        private List<Part> ChildParts { get;  }
        public List<Line> Lines { get; }
        private int EndCount { get; }

        private int StartCount { get; }

        public void Read(InvoiceLine line)
        {
            if (_endlines.Count == EndCount)
            {
                if (_part.RecuringPart != null)
                {
                    _startlines.Clear();
                    _endlines.Clear();
                    _bodylines.Clear();
                }
                else
                {
                    return;
                }


            }
            if (_startlines.Count() < StartCount && _part.Start.Any(z => Regex.Match(line.Line,
                    z.RegExChain.RegularExpressions.RegEx, RegexOptions.Multiline | RegexOptions.IgnoreCase).Success))
            {
                _startlines.Add(line);
            }

            if (_startlines.Count() == StartCount && _endlines.Count < EndCount)
            {
                _bodylines.Add(line);
                ChildParts.ForEach(x => x.Read(line));
                Lines.Where(x => x.Values.Count < x.OCR_Lines.Fields.Count).ForEach(x => x.Read(_bodylines));

            }

            if (_endlines.Count() < EndCount && _part.End.Any(z => Regex.Match(line.Line,
                    z.RegExChain.RegularExpressions.RegEx, RegexOptions.Multiline | RegexOptions.IgnoreCase).Success))
            {
                _endlines.Add(line);
            }



        }
    }

    public class Line
    {
        public Lines OCR_Lines { get; }

        public Line(Lines lines)
        {
            OCR_Lines = lines;
        }

        public void Read(List<InvoiceLine> lines)
        {
            var line = lines.Select(x => x.Line).Aggregate((c, n) => c +"\r\n" + n);
            var match = Regex.Match(line, OCR_Lines.RegExChain.RegularExpressions.RegEx,
                RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
            if (!match.Success) return;
            foreach (var field in OCR_Lines.Fields)
            {
                Values.Add(field.Key, match.Groups[field.Key].Value.Trim());
            }
            
            
        }

        public Dictionary<string,string> Values { get; } = new Dictionary<string, string>();
    }

   
}
