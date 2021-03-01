using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using Core.Common.Extensions;
using DocumentDS.Business.Entities;
using EmailDownloader;
using EntryDataDS.Business.Entities;
using MoreLinq;
using org.apache.pdfbox.pdmodel;
using org.apache.pdfbox.util;
using OCR.Business.Entities;
using pdf_ocr;
using SimpleMvvmToolkit.ModelExtensions;
using Tesseract;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis.PageSegmenter;
using UglyToad.PdfPig.DocumentLayoutAnalysis.ReadingOrderDetector;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;
using WaterNut.DataSpace;
using FileTypes = CoreEntities.Business.Entities.FileTypes;
using Invoices = OCR.Business.Entities.Invoices;

namespace WaterNut.DataSpace
{
    public class InvoiceReader
    {
        public static bool Import(string file, int fileTypeId, int emailId, bool overWriteExisting,
            List<AsycudaDocumentSet> docSet, FileTypes fileType, Client client)
        {
            //Get Text
            try
            {

                
                StringBuilder pdftxt = new StringBuilder();
                

                //pdftxt = parseUsingPDFBox(file);
                var ripTask = Task.Run(() =>
                {
                    var txt = "------------------------------------------Ripped Text-------------------------\r\n";
                    txt += pdfPigText(file); //TODO: need to implement the layout logic
                    return txt; 
                });

               
                var singleColumnTask = Task.Run(() =>
                {

                    var txt =
                        "------------------------------------------Single Column-------------------------\r\n";
                    txt += new PdfOcr().Ocr(file, PageSegMode.SingleColumn);
                    return txt;
                });

                var sparseTextTask = Task.Run(() =>
                {
                   
                        var txt = "------------------------------------------SparseText-------------------------\r\n";
                        txt += new PdfOcr().Ocr(file, PageSegMode.SparseText);
                        return txt;
                
                });

                Task.WhenAll(ripTask, singleColumnTask, sparseTextTask);//, 

                pdftxt.AppendLine(ripTask.Result);
                pdftxt.AppendLine(singleColumnTask.Result);
                pdftxt.AppendLine(sparseTextTask.Result);
                //Get Template
                using (var ctx = new OCRContext())
                {
                    var templates = ctx.Invoices
                        .Include(x => x.Parts)
                        .Include("RegEx.RegEx")
                        .Include("RegEx.ReplacementRegEx")
                        .Include("Parts.RecuringPart")
                        .Include("Parts.Start.RegularExpressions")
                        .Include("Parts.End.RegularExpressions")
                        .Include("Parts.PartTypes")
                        .Include("Parts.ChildParts.ChildPart.Start.RegularExpressions")
                        .Include("Parts.ParentParts.ParentPart.Start.RegularExpressions")
                        .Include("Parts.Lines.RegularExpressions")
                        .Include("Parts.Lines.Fields.FieldValue")
                        .Include("Parts.Lines.Fields.FormatRegEx.RegEx")
                        .Include("Parts.Lines.Fields.FormatRegEx.ReplacementRegEx")
                        .Where(x => x.IsActive)
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                        //.Where(x => x.Id == 3) //BaseDataModel.Instance.CurrentApplicationSettings.TestMode != true ||
                        .ToList()
                        .Select(x => new Invoice(x));

                    foreach (var tmp in templates)
                    {
                        try
                        {
                            List<dynamic> csvLines = tmp.Read(tmp.Format(pdftxt.ToString()));
                            if (csvLines.Count < 1 || !tmp.Success)
                            {
                                var failedlines = tmp.Lines.DistinctBy(x => x.OCR_Lines.Id).Where(z => z.FailedFields.Any() || (z.OCR_Lines.Fields.Any(f => f.IsRequired) && !z.Values.Any())).ToList();
                                
                                if (failedlines.Any() && failedlines.Count < tmp.Lines.Count &&
                                    tmp.Parts.First().WasStarted && tmp.Lines.SelectMany(x => x.Values.Values).Any())
                                {
                                    ReportUnImportedFile(file, emailId, client, pdftxt.ToString(),"Following fields failed to import", failedlines);
                                    return false;
                                }
                                continue;
                            }
                            //
                            // -------------------Plan to only report import failures ---------- data failures will be reported after import
                            //

                            //var invoiceLines = tmp.Lines.Where(x => x.Values.Any(v => v.Value.Any(f => f.Key.Field == "SubTotal" || f.Key.Field == "InvoiceTotal" || f.Key.EntityType == "InvoiceDetails"))).ToList();
                            //var fields = invoiceLines.SelectMany(x => x.Values.ToList().SelectMany(v => v.Value).ToList()).ToList();
                            //var invoiceTotal = fields.Where(x => x.Key.EntityType == "Invoice" && x.Key.Field == "InvoiceTotal").DistinctBy(x => x.Key.Id).Sum(x => Convert.ToDouble(x.Value));
                            //var subTotal = fields.Where(x => x.Key.EntityType == "Invoice" && x.Key.Field == "SubTotal").DistinctBy(x => x.Key.Id).Sum(x => Convert.ToDouble(x.Value));
                            //var importedTotal = fields.Where(x => x.Key.EntityType == "InvoiceDetails" && x.Key.Field == "TotalCost" && !string.IsNullOrEmpty(x.Value) ).Sum(x => Convert.ToDouble(x.Value));
                            //if (Math.Abs(invoiceTotal - importedTotal) > 0.01)
                            //{
                                

                            //    ReportUnImportedFile(file, emailId, client, pdftxt.ToString(),"Invoice Totals don't Match Imported Total", invoiceLines);
                            //    return false;
                            //}

                            if (fileType.Id != tmp.OcrInvoices.FileTypeId)
                                fileType = BaseDataModel.GetFileType(tmp.OcrInvoices.FileTypeId);

                            SaveCsvEntryData.Instance.ProcessCsvSummaryData(fileType, docSet, overWriteExisting,
                                emailId,
                                 file, csvLines).Wait();
                            return true;
                        }
                        catch (Exception e)
                        {
                            var ex = new ApplicationException($"Problem importing file:{file} --- {e.Message}", e);
                            Console.WriteLine(ex);
                            throw ex;
                        }

                    }

                    ReportUnImportedFile(file, emailId, client, pdftxt.ToString(), "No template found for this File", new List<Line>());
                    //SeeWhatSticks(pdftxt);
                }

                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                var fileInfo = new FileInfo(file);
                var errTxt = $"Hey,\r\n\r\n The System Found a problem with this File-'{fileInfo.Name}'.\r\n" +
                             $"{e.Message}" +
                             "Check the file again or Check Joseph Bartholomew at Joseph@auto-brokerage.com to make the necessary changes.\r\n" +
                             "Thanks\r\n" +
                             $"AutoBot";
                EmailDownloader.EmailDownloader.SendBackMsg(emailId, client, errTxt);
                
                return false;
            }
        }

        private static string pdfPigText(string file)
        {
         
                var pdfText = "";
                var sb = new StringBuilder();
                using (var pdf = PdfDocument.Open(file))
                {
                    foreach (var page in pdf.GetPages())
                    {
                        // Either extract based on order in the underlying document with newlines and spaces.
                        var text = ContentOrderTextExtractor.GetText(page);
                        sb.AppendLine(text);
                        // // Or based on grouping letters into words.
                        // var otherText = string.Join(" ", page.GetWords());

                        // // Or the raw text of the page's content stream.
                        // var rawText = page.Text;

                        //// Console.WriteLine(text);
                    }

                }

                pdfText = sb.ToString();
                return pdfText;
           


        }

        private static string parseUsingPDFBox(string input)
        {
            PDDocument doc = null;

            try
            {
                doc = PDDocument.load(input);
                PDFTextStripper stripper = new PDFTextStripper();
               // stripper.
                return stripper.getText(doc);
            }
            finally
            {
                if (doc != null)
                {
                    doc.close();
                }
            }
        }

        private static void ReportUnImportedFile(string file, int emailId, Client client, string pdftxt, string error,
            List<Line> failedlst)
        {
            var fileInfo = new FileInfo(file);
            var body = $"Hey,\r\n\r\n {error}-'{fileInfo.Name}'.\r\n" +
                       $"{failedlst.Select(x => $"Line:{x.OCR_Lines.Name} - RegId: {x.OCR_Lines.RegularExpressions.Id} - Regex: '{x.OCR_Lines.RegularExpressions.RegEx}' - Fields: {x.FailedFields.SelectMany(z => z.ToList()).SelectMany(z => z.Value.ToList()).Select(z => $"{z.Key.Key} - '{z.Key.Field}'").DefaultIfEmpty(string.Empty).Aggregate((o, c) => o + "\r\n" + c)}").DefaultIfEmpty(string.Empty).Aggregate((o, c) => o + "\r\n" + c)}" +
                       "Thanks\r\n" +
                       $"AutoBot";
            File.WriteAllText(file + ".txt", pdftxt);
            EmailDownloader.EmailDownloader.ForwardMsg(emailId, client, "Invoice Template Not found!", body,
                new[] {"Joseph@auto-brokerage.com"}, new[] {file, file + ".txt"});
        }



        private static void SeeWhatSticks(string pdftext)
        {
            using (var ctx = new OCRContext())
            {
                var allLines = ctx.Lines.Include(x => x.RegularExpressions).ToList();

                var goodLines = new List<Lines>();
                    foreach (var regLine in allLines)
                    {
                      var match =  Regex.Match(pdftext, regLine.RegularExpressions.RegEx,
                            RegexOptions.Multiline | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                        if(match.Success) goodLines.Add(regLine);
                    }

                foreach (var line in goodLines)
                {

                }

            }
        }
    }

    public class Invoice
    {
        private EntryData EntryData { get; } = new EntryData();
        public Invoices OcrInvoices { get; }
        public List<Part> Parts { get; set; }
        public bool Success => Parts.All(x => x.Success);
        public List<Line> Lines => Parts.SelectMany(x => x.AllLines).DistinctBy(x => x.OCR_Lines.Id).ToList();

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

                if (!this.Lines.SelectMany(x => x.Values.Values).Any()) return new List<dynamic>();//!Success

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
            try
            {


                var lst = new List<IDictionary<string, object>>();
                var itm = new BetterExpando();
                var ditm = ((IDictionary<string, object>) itm);
                foreach (var line in x.Lines)
                {

                    foreach (var value in line.Values)
                    {
                        if (x.OCR_Part.RecuringPart != null && x.OCR_Part.RecuringPart.IsComposite == false)
                        {
                            itm = new BetterExpando();
                            ditm = ((IDictionary<string, object>) itm);
                        }

                        ditm["FileLineNumber"] = value.Key + 1;
                        foreach (var field in value.Value)
                        {
                            if (ditm.ContainsKey(field.Key.Field) && line.OCR_Lines.Fields.Select(z => z.Field)
                                    .Count(f => f == field.Key.Field) > 1)
                            {
                                switch (field.Key.DataType)
                                {
                                    case "String":
                                        ditm[field.Key.Field] =
                                            (ditm[field.Key.Field] + " " + GetValueByKey(value, field.Key.Key)).Trim();
                                        break;
                                    case "Number":
                                    case "Numeric":
                                        ditm[field.Key.Field] =
                                            Convert.ToDouble(ditm[field.Key.Field]??"0") +
                                            Convert.ToDouble(GetValueByKey(value, field.Key.Key));
                                        break;
                                    default:
                                        ditm[field.Key.Field] = GetValueByKey(value, field.Key.Key);
                                        break;
                                }

                            }
                            else
                            {
                                ditm[field.Key.Field] = GetValue(value, field.Key.Field);
                            }

                        }

                        if (x.OCR_Part.RecuringPart != null && x.OCR_Part.RecuringPart.IsComposite == false)
                            lst.Add(itm);
                    }



                }

                foreach (var childPart in x.ChildParts)
                {
                   // if (!childPart.Lines.Any()) continue;
                    var childItms = SetPartLineValues(childPart);
                    var fieldname = childPart.AllLines.First().OCR_Lines.Fields.First().EntityType;
                    if (childPart.OCR_Part.RecuringPart != null || !childPart.Lines.Any())
                    {
                        if (!x.Lines.Any())
                        {
                            lst.AddRange(childItms);
                        }
                        else
                        {
                            ditm[fieldname] = childItms;
                        }
                            
                    }
                    else
                    {
                        if (!x.Lines.Any())
                        {
                            lst.Add(childItms.FirstOrDefault());
                        }
                        else
                        {
                            ditm[fieldname] = childItms.FirstOrDefault();
                        }
                        
                    }

                }


                if ((x.OCR_Part.RecuringPart == null || x.OCR_Part.RecuringPart.IsComposite) && ditm.Any()) lst.Add(itm);
                return lst;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
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
        private dynamic GetValueByKey(KeyValuePair<int, Dictionary<Fields, string>> z, string key)
        {
            try
            {
                var f = z.Value.FirstOrDefault(q => q.Key.Key == key);
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
                    if (val == "") val = "0";
                    if (double.TryParse(val, out double num))
                        return num;
                    else
                        throw new ApplicationException(
                            $"{f.Key.Field} can not convert to {f.Key.DataType} for Value:{f.Value}");

                case "Date":
                    if (DateTime.TryParse(f.Value, out DateTime date))
                        return date;
                    else
                        //throw new ApplicationException(
                        //    $"{f.Key.Field} can not convert to {f.Key.DataType} for Value:{f.Value}");
                        return DateTime.MinValue;
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
                foreach (var reg in OcrInvoices.RegEx)
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

        public bool Success => Lines.All(x => !x.Values.SelectMany(z => z.Value).Any(z => z.Key.IsRequired && string.IsNullOrEmpty(z.Value.ToString()))) 
                               //&& this.Lines.Any()
                               && !this.FailedLines.Any()
                               && ChildParts.All(x => x.Success == true);
        public List<Line> FailedLines => Lines.Where(x =>  x.OCR_Lines.Fields.Any(z => z.IsRequired) && !x.Values.Any()).ToList()
                                         .Union(ChildParts.SelectMany(x =>x.FailedLines)).ToList();

        public List<Dictionary<string, List<KeyValuePair<Fields, string>>>> FailedFields =>
            Lines.SelectMany(x => x.FailedFields).ToList();
        //public List<Dictionary<string, List<KeyValuePair<Fields, string>>>> FailedFields => Lines
        //                                                  .Where(x => x.Values.SelectMany(z => z.Value).Any(z => z.Key.IsRequired && string.IsNullOrEmpty(z.Value.ToString())))
        //                                                  .Select(x => x.Values.SelectMany(z => z.Value.ToList())
        //                                                                        .GroupBy(g => g.Key.Field)
        //                                                                        .ToDictionary(k => k.Key, v => v.ToList())
        //).ToList();

        public List<Line> AllLines => Lines.Union(ChildParts.SelectMany(x => x.AllLines)).DistinctBy(x => x.OCR_Lines.Id).ToList();
        public bool WasStarted => this._startlines.Any();

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
                if(!WasStarted  || (WasStarted && OCR_Part.RecuringPart != null))
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
                var value = field.FieldValue?.Value ?? match.Groups[field.Key].Value.Trim();
                foreach (var reg in field.FormatRegEx)
                {
                    value = Regex.Replace(value, reg.RegEx.RegEx, reg.ReplacementRegEx.RegEx,
                        RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture);
                };
               
                values.Add(field, value.Trim());//$"\"{}\""
            }

            Values[lines.First().LineNumber] = values;
        }

        public Dictionary<int, Dictionary<Fields, string>> Values { get; } = new Dictionary<int, Dictionary<Fields, string>>();
        public bool MultiLine => OCR_Lines.MultiLine;

        public List<Dictionary<string, List<KeyValuePair<Fields, string>>>> FailedFields => this.Values
            .Where(x => x.Value.Any(z => z.Key.IsRequired && string.IsNullOrEmpty(z.Value.ToString())))
            .SelectMany(x => x.Value.ToList())
            .DistinctBy(x => x.Key.Id)
            .GroupBy(x => x.Key.Key)
            .Select(x => x.ToDictionary(k => x.Key, v => x.ToList()))
            .ToList();

    }

   
}
