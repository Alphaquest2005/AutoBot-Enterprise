using System;
using System.Collections.Generic;
using System.Data.Entity;
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
using Invoices = OCR.Business.Entities.Invoices;

namespace WaterNut.DataSpace
{
    public class InvoiceReader
    {
        public static void Import(string file, int? fileTypeId, int? emailId, bool overWriteExisting,
            List<AsycudaDocumentSet> docSet, string fileType)
        {
            //Get Text

            var pdftxt = PdfOcr.Ocr(file);

            //Get Template
            using (var ctx = new OCRContext())
            {
                var templates = ctx.Invoices
                    .Include(x => x.Parts)
                    .Include("Parts.RecuringPart")
                    .Include("Parts.Start.RegularExpressions")
                    .Include("Parts.End.RegularExpressions")
                    .Include("Parts.PartTypes")
                    .Include("Parts.ChildParts.ParentPart.Start.RegularExpressions")
                    .Include("Parts.ParentParts.ChildPart.Start.RegularExpressions")
                    .Include("Parts.Lines.RegularExpressions")
                    .Include("Parts.Lines.Fields.FieldValue")
                    .Where(x => BaseDataModel.Instance.CurrentApplicationSettings.TestMode != true || x.Id == 4)
                    .ToList()
                    .Select(x => new Invoice(x));

                foreach (var tmp in templates)
                {
                    try
                    {
                        var csvLines = tmp.Read(pdftxt.Split(new[] {"\r\n", "\r", "\n"}, StringSplitOptions.None)
                            .ToList());
                        if (csvLines.Count <= 1) continue;
                        SaveCsvEntryData.Instance.ProcessCsvSummaryData(fileType, docSet, overWriteExisting, emailId,
                            fileTypeId, file, csvLines).Wait();
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


    }

    public class Invoice
    {
        private EntryData EntryData { get; } = new EntryData();
        private Invoices OcrInvoices { get; }
        private List<Part> Parts { get; set; }
        public bool Success => Parts.All(x => x.Success);
        public List<Line> Lines => Parts.SelectMany(x => x.AllLines).ToList();

        public Invoice(Invoices ocrInvoices)
        {
            OcrInvoices = ocrInvoices;
            Parts = ocrInvoices.Parts
                .Where(x => (x.ParentParts.Any() && !x.ChildParts.Any()) || (!x.ParentParts.Any() && !x.ChildParts.Any()))
                //.Where(x => x.Id == 7)
                .Select(z => new Part(z)).ToList();
        }

        public List<SaveCsvEntryData.CSVDataSummary> Read(List<string> text)
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

            if (!Success) return new List<SaveCsvEntryData.CSVDataSummary>();

                var headerRow = new SaveCsvEntryData.CSVDataSummary()
                {
                    EntryDataId = GetValue(nameof(SaveCsvEntryData.CSVDataSummary.EntryDataId)),
                    TotalCost = GetValue(nameof(SaveCsvEntryData.CSVDataSummary.TotalCost)),
                    Cost = GetValue( nameof(SaveCsvEntryData.CSVDataSummary.Cost)),
                    Quantity = GetValue(nameof(SaveCsvEntryData.CSVDataSummary.Quantity)),
                    ItemDescription = GetValue(nameof(SaveCsvEntryData.CSVDataSummary.ItemDescription)),
                    TotalDeductions = GetValue(nameof(SaveCsvEntryData.CSVDataSummary.TotalDeductions)),
                    TotalFreight = GetValue(nameof(SaveCsvEntryData.CSVDataSummary.TotalFreight)),
                    TotalInsurance = GetValue(nameof(SaveCsvEntryData.CSVDataSummary.TotalInsurance)),
                    TotalInternalFreight = GetValue(nameof(SaveCsvEntryData.CSVDataSummary.TotalInternalFreight)),
                    TotalOtherCost = GetValue(nameof(SaveCsvEntryData.CSVDataSummary.TotalOtherCost)),
                    InvoiceTotal = GetValue(nameof(SaveCsvEntryData.CSVDataSummary.InvoiceTotal)),
                    Currency = GetValue(nameof(SaveCsvEntryData.CSVDataSummary.Currency)),
                    EntryDataDate = GetValue(nameof(SaveCsvEntryData.CSVDataSummary.EntryDataDate)),
                    ItemAlias = GetValue(nameof(SaveCsvEntryData.CSVDataSummary.ItemAlias)),
                    SupplierAddress = GetValue(nameof(SaveCsvEntryData.CSVDataSummary.SupplierAddress)),
                    SupplierCode = GetValue(nameof(SaveCsvEntryData.CSVDataSummary.SupplierCode)),
                    SupplierCountryCode = GetValue(nameof(SaveCsvEntryData.CSVDataSummary.SupplierCountryCode)),
                    SupplierInvoiceNo = GetValue(nameof(SaveCsvEntryData.CSVDataSummary.SupplierInvoiceNo)),
                    SupplierName = GetValue(nameof(SaveCsvEntryData.CSVDataSummary.SupplierName)),
                    Units = GetValue(nameof(SaveCsvEntryData.CSVDataSummary.Units)),
                    ItemNumber = GetValue(nameof(SaveCsvEntryData.CSVDataSummary.ItemNumber)),
                    
                    //Weight = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.Tax)),

                    //OtherCost = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.Tax)),
                    //PreviousInvoiceNumber = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.Tax)),
                    //ReceivedQuantity = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.Tax)),
                    //Freight = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.Tax)),
                    //Insurance = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.Tax)),
                    //InternalFreight = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.Tax)),
                    //InvoiceQuantity = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.Tax)),
                    //CustomerName = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.Tax)),
                    //Deductions = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.Tax)),
                    //EffectiveDate = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.Tax)),
                    //DocumentType = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.Tax)),
                    //CNumber = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.Tax)),
                    //Comment = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.Tax)),
                    //TotalWeight = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.Tax)),
                    //Tax = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.Tax)),
                    //TariffCode = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.TariffCode)),
                };

            var res = Lines.Where(x => x.OCR_Lines.Parts.RecuringPart != null)
                .SelectMany(x => x.Values.Select(z => new SaveCsvEntryData.CSVDataSummary()
                {
                    EntryDataId = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.EntryDataId))?? headerRow.EntryDataId,
                    TotalCost = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.TotalCost)),
                    Cost = /*GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.TotalCost)) < Convert.ToDouble(GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.Cost))) ? */
                          /*Math.Round(*/Convert.ToDouble(GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.TotalCost))) / Convert.ToDouble(GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.Quantity)))/*,2) */
                          /*: GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.Cost))*/,
                    Quantity = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.Quantity)),
                    ItemDescription = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.ItemDescription)),
                    TotalDeductions = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.TotalDeductions)),
                    TotalFreight = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.TotalFreight)),
                    TotalInsurance = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.TotalInsurance)),
                    TotalInternalFreight = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.TotalInternalFreight)),
                    TotalOtherCost = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.TotalOtherCost)),
                    InvoiceTotal = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.InvoiceTotal)),
                    Currency = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.Currency)) ?? headerRow.Currency,
                    EntryDataDate = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.EntryDataDate)) ?? headerRow.EntryDataDate,
                    ItemAlias = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.ItemAlias)),
                    SupplierAddress = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.SupplierAddress)),
                    SupplierCode = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.SupplierCode)),
                    SupplierCountryCode = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.SupplierCountryCode)),
                    SupplierInvoiceNo = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.SupplierInvoiceNo)),
                    SupplierName = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.SupplierName)),
                    Units = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.Units)),
                    ItemNumber = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.ItemNumber)),
                    LineNumber = z.Key,
                    //Weight = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.Tax)),

                    //OtherCost = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.Tax)),
                    //PreviousInvoiceNumber = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.Tax)),
                    //ReceivedQuantity = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.Tax)),
                    //Freight = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.Tax)),
                    //Insurance = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.Tax)),
                    //InternalFreight = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.Tax)),
                    //InvoiceQuantity = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.Tax)),
                    //CustomerName = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.Tax)),
                    //Deductions = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.Tax)),
                    //EffectiveDate = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.Tax)),
                    //DocumentType = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.Tax)),
                    //CNumber = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.Tax)),
                    //Comment = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.Tax)),
                    //TotalWeight = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.Tax)),
                    //Tax = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.Tax)),
                    //TariffCode = GetValue(z, nameof(SaveCsvEntryData.CSVDataSummary.TariffCode)),
                })).ToList();

            res.Add(headerRow);

            return res;
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

        private dynamic GetValue(string field)
        {
            var f = Lines.Where(x => x.OCR_Lines.Parts.RecuringPart == null).SelectMany(x => x.Values.Values).SelectMany(x => x).FirstOrDefault(x => x.Key.Field == field);
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
                    var val = f.Value.Replace("$", "");
                    if(double.TryParse(val, out double num))
                        return num;
                    else
                        throw new ApplicationException($"{f.Key.Field} can not convert to {f.Key.DataType} for Value:{f.Value}");
                    
                case "Date":
                    if (DateTime.TryParse(f.Value, out DateTime date))
                        return date;
                    else
                        throw new ApplicationException($"{f.Key.Field} can not convert to {f.Key.DataType} for Value:{f.Value}");
                default:
                    return f.Value;
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

        public bool Success => Lines.All(x => !x.Values.SelectMany(z => z.Value).Any(z => z.Key.IsRequired == true && string.IsNullOrEmpty(z.Value.ToString())) 
                               /*|| !x.Values.Any()*/) 
                               && ChildParts.All(x => x.Success == true);
        public List<Line> FailedLines => Lines.Where(x => !x.Values.SelectMany(z => z.Value).Any(z => z.Key.IsRequired == true && string.IsNullOrEmpty(z.Value.ToString()))).ToList()
                                         .Union(ChildParts.SelectMany(x =>x.FailedLines)).ToList();

        public List<Line> AllLines => Lines.Union(ChildParts.SelectMany(x => x.AllLines)).ToList();

        public void Read(InvoiceLine line)
        {
            if ((_endlines.Count == EndCount && EndCount > 0) 
                || (EndCount == 0 && _startlines.Count == StartCount && StartCount > 0))//attempting to do start to start
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
