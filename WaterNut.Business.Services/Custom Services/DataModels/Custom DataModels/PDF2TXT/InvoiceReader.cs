using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Globalization;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using DocumentDS.Business.Entities;
using EntryDataDS.Business.Entities;
using MoreLinq;
using OCR.Business.Entities;
using pdf_ocr;
using SimpleMvvmToolkit.ModelExtensions;
using WaterNut.DataSpace;
using FileTypes = CoreEntities.Business.Entities.FileTypes;
using Invoices = OCR.Business.Entities.Invoices;

namespace WaterNut.DataSpace
{
    public class InvoiceReader
    {
        public static void Import(string file, int fileTypeId, int emailId, bool overWriteExisting,
            List<AsycudaDocumentSet> docSet, FileTypes fileType)
        {
            //Get Text
            try
            {


                var pdftxt = PdfOcr.Ocr(file);

                //Get Template
                using (var ctx = new OCRContext())
                {
                    var templates = ctx.Invoices
                        .Include(x => x.Parts)
                        .Include("OCR_InvoiceRegEx.RegEx")
                        .Include("OCR_InvoiceRegEx.ReplacementRegEx")
                        .Include("Parts.RecuringPart")
                        .Include("Parts.Start.RegularExpressions")
                        .Include("Parts.End.RegularExpressions")
                        .Include("Parts.PartTypes")
                        .Include("Parts.ChildParts.ChildPart.Start.RegularExpressions")
                        .Include("Parts.ParentParts.ParentPart.Start.RegularExpressions")
                        .Include("Parts.Lines.RegularExpressions")
                        .Include("Parts.Lines.Fields.FieldValue")
                        .Where(x => x.Id == 10) //BaseDataModel.Instance.CurrentApplicationSettings.TestMode != true ||
                        .ToList()
                        .Select(x => new Invoice(x));

                    foreach (var tmp in templates)
                    {
                        try
                        {
                            List<dynamic> csvLines = tmp.Read(tmp.Format(pdftxt));
                            if (csvLines.Count < 1) continue;
                            if (fileType.Id != tmp.OcrInvoices.FileTypeId && tmp.OcrInvoices.FileTypeId.HasValue)
                                fileType = BaseDataModel.GetFileType(tmp.OcrInvoices.FileTypeId.Value);

                            SaveCsvEntryData.Instance.ProcessCsvSummaryData(fileType, docSet, overWriteExisting,
                                emailId,
                                fileType.Id, file, csvLines).Wait();
                            break;
                        }
                        catch (Exception e)
                        {
                            var ex = new ApplicationException($"Problem importing file:{file} --- {e.Message}", e);
                            Console.WriteLine(ex);
                            throw ex;
                        }

                    }
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }


    }

    public class Invoice
    {
        private EntryData EntryData { get; } = new EntryData();
        public Invoices OcrInvoices { get; }
        private List<Part> Parts { get; set; }
        public bool Success => Parts.All(x => x.Success);
        public List<Line> Lines => Parts.SelectMany(x => x.AllLines).ToList();

        public Invoice(Invoices ocrInvoices)
        {
            OcrInvoices = ocrInvoices;
            Parts = ocrInvoices.Parts
                .Where(x => (x.ParentParts.Any() && !x.ChildParts.Any()) ||
                            (!x.ParentParts.Any() && !x.ChildParts.Any()))
                //.Where(x => x.Id == 7)
                .Select(z => new Part(z)).ToList();
        }

        public List<dynamic> Read(List<string> text)
        {
            try
            {


                var lineCount = 0;
                foreach (var line in text)
                {
                    lineCount += 1;
                    var iLine = new InvoiceLine(line, lineCount);
                    Parts.ForEach(x => x.Read(iLine));
                }

                if (!Success) return new List<dynamic>();

                var ores = Parts.Select(x =>
                    {

                        var lst = SetPartLineValues(x);
                        return lst;
                    }
                ).ToList();

                return new List<dynamic> {ores.SelectMany(x => x.ToList()).ToList()};
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private List<IDictionary<string, object>> SetPartLineValues(Part x)
        {
            var lst = new List<IDictionary<string, object>>();
            var itm = new ExpandoObject();
            foreach (var line in x.Lines)
            {

                foreach (var value in line.Values)
                {
                    if (x.OCR_Part.RecuringPart != null && x.OCR_Part.RecuringPart.IsComposite == false)
                        itm = new ExpandoObject();
                    ((IDictionary<string, object>) itm)["FileLineNumber"] = value.Key + 1;
                    foreach (var field in value.Value)
                    {
                        ((IDictionary<string, object>) itm)[field.Key.Field] = GetValue(value, field.Key.Field);
                    }

                    if (x.OCR_Part.RecuringPart != null && x.OCR_Part.RecuringPart.IsComposite == false) lst.Add(itm);
                }

            }

            foreach (var childPart in x.ChildParts)
            {
                if (!childPart.Lines.Any()) continue;
                var childItms = SetPartLineValues(childPart);
                var fieldname = childPart.Lines.First().OCR_Lines.Fields.First().EntityType;
                if (childPart.OCR_Part.RecuringPart != null)
                {
                    ((IDictionary<string, object>) itm)[fieldname] = childItms;
                }
                else
                {
                    ((IDictionary<string, object>) itm)[fieldname] = childItms.FirstOrDefault();
                }

            }

            if (x.OCR_Part.RecuringPart == null || x.OCR_Part.RecuringPart.IsComposite) lst.Add(itm);
            return lst;
        }

        private dynamic GetValue(KeyValuePair<int, Dictionary<Fields, string>> z, string field)
        {
            try
            {
                var f = z.Value.FirstOrDefault(q => q.Key.Field == field);
                return f.Key == null ? null : GetValue(f);
            }
            catch (Exception e)
            {
                var ex = new ApplicationException($"Error Importing Line:{z.Key} --- {e.Message}", e);
                Console.WriteLine(ex);
                throw ex;
            }

        }

        private dynamic GetValue(string field)
        {
            var f = Lines.Where(x => x.OCR_Lines.Parts.RecuringPart == null).SelectMany(x => x.Values.Values)
                .SelectMany(x => x).FirstOrDefault(x => x.Key.Field == field);
            return f.Key == null ? null : GetValue(f);
        }

        private static dynamic GetValue(KeyValuePair<Fields, string> f)
        {
            if (f.Key == null) return null;
            switch (f.Key.DataType)
            {
                case "String":
                    return f.Value;
                case "Numeric":
                case "Number":
                    var val = f.Value.Replace("$", "");
                    if (double.TryParse(val, out double num))
                        return num;
                    else
                        throw new ApplicationException(
                            $"{f.Key.Field} can not convert to {f.Key.DataType} for Value:{f.Value}");

                case "Date":
                    if (DateTime.TryParse(f.Value, out DateTime date))
                        return date;
                    else
                        throw new ApplicationException(
                            $"{f.Key.Field} can not convert to {f.Key.DataType} for Value:{f.Value}");
                case "English Date":
                    if (DateTime.TryParseExact(f.Value, "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None,
                        out DateTime edate))
                        return edate;
                    else
                        throw new ApplicationException(
                            $"{f.Key.Field} can not convert to {f.Key.DataType} for Value:{f.Value}");
                default:
                    return f.Value;
            }
        }

        public List<string> Format(string pdftxt)
        {
            try
            {
                foreach (var reg in OcrInvoices.OCR_InvoiceRegEx)
                {
                    pdftxt = Regex.Replace(pdftxt, reg.RegEx.RegEx, reg.ReplacementRegEx.RegEx,
                        RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture);
                }

                return pdftxt.Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None).ToList();
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

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
        public Parts OCR_Part;

        public Part(Parts part)
        {
             StartCount = part.Start.Select(x => x.RegularExpressions.RegEx).Count();
             EndCount = part.End.Select(x => x.RegularExpressions.RegEx).Count();
            OCR_Part = part;
            ChildParts = part.ParentParts.Select(x => new Part(x.ChildPart)).ToList();
            Lines = part.Lines.Select(x => new Line(x)).ToList();

        }

        public List<Part> ChildParts { get;  }
        public List<Line> Lines { get; } 
        private int EndCount { get; }

        private int StartCount { get; }

        public bool Success => Lines.All(x => (x.OCR_Lines.Fields.Any(z => z.IsRequired) && !x.Values.SelectMany(z => z.Value).Any(z => z.Key.IsRequired && string.IsNullOrEmpty(z.Value.ToString()))) 
                               || !x.OCR_Lines.Fields.Any(z => z.IsRequired)) 
                               && ChildParts.All(x => x.Success == true);
        public List<Line> FailedLines => Lines.Where(x =>  x.OCR_Lines.Fields.Any(z => z.IsRequired) && !x.Values.Any()).ToList()
                                         .Union(ChildParts.SelectMany(x =>x.FailedLines)).ToList();

        public List<Line> AllLines => Lines.Union(ChildParts.SelectMany(x => x.AllLines)).ToList();

        public void Read(InvoiceLine line)
        {
            if ((_endlines.Count == EndCount && EndCount > 0) 
                || (EndCount == 0 && _startlines.Count > StartCount && StartCount > 0))//attempting to do start to start
            {
                if (OCR_Part.RecuringPart != null)
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
            if (OCR_Part.Start.Any(z => Regex.Match(line.Line,
                    z.RegularExpressions.RegEx, RegexOptions.Multiline | RegexOptions.IgnoreCase).Success))
            {
                if(_startlines.Count() < StartCount)
                    _startlines.Add(line);
                else if (_startlines.Count() == StartCount) // treat as start tot start
                {
                    _startlines.Clear();
                    _endlines.Clear();
                    _bodylines.Clear();
                    _startlines.Add(line);
                }
            }

            if (_startlines.Count() == StartCount && ((_endlines.Count < EndCount && EndCount > 0)  || EndCount == 0))
            {
                _bodylines.Add(line);
                ChildParts.ForEach(x => x.Read(line));
                Lines.ForEach(x => x.Read(x.MultiLine ? _bodylines : new List<InvoiceLine>() { line }));

            }

            if (_startlines.Count() == StartCount 
                && _startlines.All(x => x.LineNumber != line.LineNumber)
                && _endlines.Count() < EndCount && OCR_Part.End.Any(z => Regex.Match(line.Line,
                    z.RegularExpressions.RegEx, RegexOptions.Multiline | RegexOptions.IgnoreCase).Success))
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
            var match = Regex.Match(line, OCR_Lines.RegularExpressions.RegEx,
                RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
            if (!match.Success) return;

            var values = new Dictionary<Fields, string>();
            
            foreach (var field in OCR_Lines.Fields)
            {
                values.Add(field, field.FieldValue?.Value ?? match.Groups[field.Key].Value.Trim());//$"\"{}\""
            }

            Values[lines.First().LineNumber] = values;
        }

        public Dictionary<int, Dictionary<Fields, string>> Values { get; } = new Dictionary<int, Dictionary<Fields, string>>();
        public bool MultiLine => OCR_Lines.MultiLine;
    }

   
}
