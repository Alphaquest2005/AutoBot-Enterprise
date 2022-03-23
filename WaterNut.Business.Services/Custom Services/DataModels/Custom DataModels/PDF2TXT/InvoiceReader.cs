using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Dynamic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Linq.Expressions;
using System.Net.Sockets;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Threading.Tasks.Schedulers;
using Core.Common;
using Core.Common.Extensions;
using Core.Common.Utils;
using CoreEntities.Business.Entities;
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
using TrackableEntities;
using UglyToad.PdfPig;
using UglyToad.PdfPig.Content;
using UglyToad.PdfPig.DocumentLayoutAnalysis.PageSegmenter;
using UglyToad.PdfPig.DocumentLayoutAnalysis.ReadingOrderDetector;
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;
using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;
using WaterNut.DataSpace;
using AsycudaDocumentSet_Attachments = CoreEntities.Business.Entities.AsycudaDocumentSet_Attachments;
using Attachments = CoreEntities.Business.Entities.Attachments;
using FileTypes = CoreEntities.Business.Entities.FileTypes;
using Invoices = OCR.Business.Entities.Invoices;

namespace WaterNut.DataSpace
{


    public class InvoiceReader
    {
        public const string CommandsTxt = "Commands:\r\n" +
                                          "UpdateRegex: RegId: 000, Regex: 'xyz', IsMultiline: True\r\n" +
                                          "AddFieldRegEx: RegId: 000,  Field: Name, Regex: '', ReplaceRegex: ''\r\n" +
                                          "RequestInvoice: Name:'xyz'\r\n" +
                                          "AddInvoice: Name:'', IDRegex:''\r\n" +
                                          "AddPart: Invoice:'', Name: '', StartRegex: '', ParentPart:'', IsRecurring: True, IsComposite: False, IsMultiLine: True \r\n" +
                                          "AddLine: Invoice:'',  Part: '', Name: '', Regex: ''\r\n" +
                                          "UpdateLine: Invoice:'',  Part: '', Name: '', Regex: ''\r\n" +
                                          "AddFieldFormatRegex: RegexId: 000, Keyword:'', Regex:'', ReplaceRegex:'', ReplacementRegexIsMultiLine: True, RegexIsMultiLine: True\r\n";

        public static bool Import(string file, int fileTypeId, string emailId, bool overWriteExisting,
            List<AsycudaDocumentSet> docSet, FileTypes fileType, Client client)
        {
            //Get Text
            try
            {

                
                var pdfTxt = GetPdftxt(file);

                //Get Template
                var templates = GetTemplates(x => true);

                foreach (var tmp in templates.OrderBy(x => x.OcrInvoices.Id))//.Where(x => x.OcrInvoices.Id == 99)
                    try
                    {
                        if(TryReadFile(file, emailId, fileType, pdfTxt, client, overWriteExisting, docSet, tmp, fileTypeId)) return true;
                    }
                    catch (Exception e)
                    {
                        Exception realerror = e;
                        while (realerror.InnerException != null)
                            realerror = realerror.InnerException;

                        var failedLines = tmp.Lines.Where(x => x.OCR_Lines.Fields.Any(z => realerror.Message.Contains(z.Field)) || realerror.Message.Contains(x.OCR_Lines.Name))
                            .ToList();
                        if (!failedLines.Any()) failedLines = tmp.Lines.Where(x => x.OCR_Lines.Fields.Any(z => z.DataType == "Number" )).ToList();
                        ReportUnImportedFile(docSet, file, emailId, fileTypeId, client, pdfTxt.ToString(), $"Problem importing file:{file} --- {realerror.Message}", failedLines);

                        var ex = new ApplicationException($"Problem importing file:{file} --- {realerror.Message}", e);
                        Console.WriteLine(ex);
                        return false;
                    }

                ReportUnImportedFile(docSet,file, emailId,fileTypeId, client, pdfTxt.ToString(), "No template found for this File", new List<Line>());

                   // SeeWhatSticks(pdfTxt.ToString());
                

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

        public static List<Invoice> GetTemplates(Expression<Func<Invoices, bool>> filter)
        {
            List<Invoice> templates;
            using (var ctx = new OCRContext())
            {
                templates = ctx.Invoices
                    .Include(x => x.Parts)
                    .Include("InvoiceIdentificatonRegEx.OCR_RegularExpressions")
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
                    .Include("Parts.Lines.Fields.ChildFields.FieldValue")
                    .Include("Parts.Lines.Fields.ChildFields.FormatRegEx.RegEx")
                    .Include("Parts.Lines.Fields.ChildFields.FormatRegEx.ReplacementRegEx")
                    .Where(x => x.IsActive)
                    .Where(x => x.ApplicationSettingsId ==
                                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                    .Where(filter) //BaseDataModel.Instance.CurrentApplicationSettings.TestMode != true ||
                    .ToList()
                    .Select(x => new Invoice(x)).ToList();
            }

            return templates;
        }

        public static bool TryReadFile(string file, string emailId, FileTypes fileType,StringBuilder pdftxt,Client client, bool overWriteExisting, List<AsycudaDocumentSet> docSet, 
             Invoice tmp, int fileTypeId)
        {
            
            if (!IsInvoiceDocument(tmp.OcrInvoices, pdftxt.ToString())) return false;

            var formattedPdfTxt = tmp.Format(pdftxt.ToString());
            var csvLines = tmp.Read(formattedPdfTxt.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList());
            if (csvLines.Count < 1 || !tmp.Success)
            {
                var failedlines = tmp.Lines.DistinctBy(x => x.OCR_Lines.Id).Where(z =>
                    z.FailedFields.Any() || (z.OCR_Lines.Fields.Any(f => f.IsRequired) && !z.Values.Any())).ToList();
                if (!tmp.Parts.Any(x => x.AllLines.Any(z =>
                    z.Values.Values.Any(v =>
                        v.Keys.Any(k => k.Field == "Name") && v.Values.Any(kv => kv == tmp.OcrInvoices.Name))))) return false;
                if (failedlines.Any() && failedlines.Count < tmp.Lines.Count &&
                    (tmp.Parts.First().WasStarted || !tmp.Parts.First().OCR_Part.Start.Any()) && tmp.Lines.SelectMany(x => x.Values.Values).Any())
                {
                    ReportUnImportedFile(docSet, file, emailId,fileTypeId, client, pdftxt.ToString(), "Following fields failed to import",
                        failedlines);
                    {
                        return true;
                    }
                }

                
                return false;
            }

            if (fileType.Id != tmp.OcrInvoices.FileTypeId)
                fileType = BaseDataModel.GetFileType(tmp.OcrInvoices.FileTypeId);



            SaveCsvEntryData.Instance.ProcessCsvSummaryData(fileType, docSet, overWriteExisting,
                emailId,
                file, csvLines).Wait();
           
            return true;
        }

        public static bool IsInvoiceDocument(Invoices invoice, string fileText)
        {
            return invoice.InvoiceIdentificatonRegEx.Any() && invoice.InvoiceIdentificatonRegEx.Any(x =>
                Regex.IsMatch(fileText,
                    x.OCR_RegularExpressions.RegEx,
                    RegexOptions.IgnoreCase | (x.OCR_RegularExpressions.MultiLine == true
                        ? RegexOptions.Multiline
                        : RegexOptions.Singleline) | RegexOptions.ExplicitCapture));
        }

        public static StringBuilder GetPdftxt(string file)
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

            //var sparsOsdTextTask = Task.Run(() =>
            //{
            //    var txt = "------------------------------------------SparseTextOsd-------------------------\r\n";
            //    txt += new PdfOcr().Ocr(file, PageSegMode.SparseTextOsd);
            //    return txt;
            //});

            //var SingleWordTextTask = Task.Run(() =>
            //{
            //    var txt = "------------------------------------------SingleWord-------------------------\r\n";
            //    txt += new PdfOcr().Ocr(file, PageSegMode.SingleWord);
            //    return txt;
            //});

            //var OsdOnlyTextTask = Task.Run(() =>
            //{
            //    var txt = "------------------------------------------OsdOnly-------------------------\r\n";
            //    txt += new PdfOcr().Ocr(file, PageSegMode.OsdOnly);
            //    return txt;
            //});

            //var RawLineTextTask = Task.Run(() =>
            //{
            //    var txt = "------------------------------------------RawLine-------------------------\r\n";
            //    txt += new PdfOcr().Ocr(file, PageSegMode.RawLine);
            //    return txt;
            //});

            Task.WaitAll(ripTask, singleColumnTask, sparseTextTask); //RawLineTextTask, OsdOnlyTextTask, SingleWordTextTask, sparsOsdTextTask

            pdftxt.AppendLine(singleColumnTask.Result);
            pdftxt.AppendLine(sparseTextTask.Result);
            pdftxt.AppendLine(ripTask.Result);
            return pdftxt;
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

   

        private static void ReportUnImportedFile(List<AsycudaDocumentSet> asycudaDocumentSets, string file, string emailId,
            int fileTypeId,
            Client client, string pdftxt, string error,
            List<Line> failedlst)
        {
            var fileInfo = new FileInfo(file);
            
            var txtFile = file + ".txt";
            //if (File.Exists(txtFile)) return;
            File.WriteAllText(txtFile, pdftxt);
            var body = CreateEmail(file, emailId, client, error, failedlst, fileInfo, txtFile);
            CreateTestCase(file, failedlst, txtFile, body);


            SaveImportError(asycudaDocumentSets, file, emailId, fileTypeId, pdftxt, error, failedlst, fileInfo);
        }

        private static void SaveImportError(List<AsycudaDocumentSet> asycudaDocumentSets, string file, string emailId, int fileTypeId, string pdftxt,
            string error, List<Line> failedlst, FileInfo fileInfo)
        {
            List<AsycudaDocumentSet_Attachments> existingAttachment = new List<AsycudaDocumentSet_Attachments>();
            using (var ctx = new CoreEntitiesContext())
            {
                foreach (var docSet in asycudaDocumentSets)
                {
                    existingAttachment.AddRange(ctx.AsycudaDocumentSet_Attachments
                        .Include(x => x.Attachments)
                        .Where(x =>
                            x.AsycudaDocumentSetId == docSet.AsycudaDocumentSetId && x.Attachments.FilePath == file)
                        .ToList());
                    if (!existingAttachment.Any())
                    {
                        var newAttachment = new CoreEntities.Business.Entities.AsycudaDocumentSet_Attachments(true)
                        {
                            TrackingState = TrackingState.Added,
                            AsycudaDocumentSetId = docSet.AsycudaDocumentSetId,
                            Attachments = new Attachments(true)
                            {
                                TrackingState = TrackingState.Added,
                                EmailId = emailId.ToString(),
                                FilePath = file
                            },
                            DocumentSpecific = true,
                            FileDate = fileInfo.CreationTime,
                            FileTypeId = fileTypeId,
                        };
                        ctx.AsycudaDocumentSet_Attachments.Add(newAttachment);
                        ctx.SaveChanges();
                        existingAttachment.Add(newAttachment);
                    }
                }
            }

            using (var ctx = new OCRContext())
            {
                foreach (var att in existingAttachment)
                {
                    var importErr = ctx.ImportErrors.FirstOrDefault(x => x.Id == att.Id);
                    if (importErr == null)
                    {
                        importErr = new ImportErrors(true)
                        {
                            Id = att.Id,
                            PdfText = pdftxt,
                            Error = error,
                            EntryDateTime = DateTime.Now,
                            OCR_FailedLines = failedlst.Select(x => new OCR_FailedLines(true)
                            {
                                TrackingState = TrackingState.Added,
                                DocSetAttachmentId = att.Id,
                                LineId = x.OCR_Lines.Id,
                                Resolved = false,
                                OCR_FailedFields = x.FailedFields.SelectMany(z =>
                                        z.SelectMany(q => q.Value.Select(w => w.Key)))
                                    .DistinctBy(z => z.Id)
                                    .Select(z => new OCR_FailedFields(true)
                                    {
                                        TrackingState = TrackingState.Added,
                                        FieldId = z.Id,
                                    })
                                    .ToList()
                            }).ToList()
                        };
                        ctx.ImportErrors.Add(importErr);
                    }
                    else
                    {
                        importErr.PdfText = pdftxt;
                        importErr.Error = error;
                        importErr.EntryDateTime = DateTime.Now;
                        importErr.OCR_FailedLines = failedlst.Select(x => new OCR_FailedLines(true)
                        {
                            TrackingState = TrackingState.Added,
                            DocSetAttachmentId = att.Id,
                            LineId = x.OCR_Lines.Id,
                            Resolved = false,
                            OCR_FailedFields = x.FailedFields.SelectMany(z =>
                                    z.SelectMany(q => q.Value.Select(w => w.Key)))
                                .DistinctBy(z => z.Id)
                                .Select(z => new OCR_FailedFields(true)
                                {
                                    TrackingState = TrackingState.Added,
                                    FieldId = z.Id,
                                })
                                .ToList()
                        }).ToList();
                    }

                    ctx.SaveChanges();
                }
            }
        }

        private static void CreateTestCase(string file, List<Line> failedlst, string txtFile, string body)
        {
            dynamic testCaseData = new BetterExpando();
            testCaseData.DateTime = DateTime.Now;
            testCaseData.Id = failedlst.FirstOrDefault()?.OCR_Lines.Parts.Invoices.Id;
            testCaseData.Supplier = failedlst.FirstOrDefault()?.OCR_Lines.Parts.Invoices.Name;
            testCaseData.PdfFile = file;
            testCaseData.TxtFile = txtFile;
            testCaseData.Message = body;
            //write to info
            UnitTestLogger.Log(
                new List<String>
                    {FunctionLibary.NameOfCallingClass(), failedlst.FirstOrDefault()?.OCR_Lines.Parts.Invoices.Name,},
                BaseDataModel.Instance.CurrentApplicationSettings.DataFolder, testCaseData);
        }

        private static string CreateEmail(string file, string emailId, Client client, string error, List<Line> failedlst,
            FileInfo fileInfo, string txtFile)
        {
            var body = $"Hey,\r\n\r\n {error}-'{fileInfo.Name}'.\r\n\r\n\r\n" +
                       $"{failedlst.Select(x => $"Line:{x.OCR_Lines.Name} - RegId: {x.OCR_Lines.RegularExpressions.Id} - Regex: {x.OCR_Lines.RegularExpressions.RegEx} - Fields: {x.FailedFields.SelectMany(z => z.ToList()).SelectMany(z => z.Value.ToList()).Select(z => $"{z.Key.Key} - '{z.Key.Field}'").DefaultIfEmpty(string.Empty).Aggregate((o, c) => o + "\r\n\r\n" + c)}").DefaultIfEmpty(string.Empty).Aggregate((o, c) => o + "\r\n" + c)}\r\n\r\n" +
                       "Thanks\r\n" +
                       "Thanks\r\n" +
                       $"AutoBot\r\n" +
                       $"\r\n" +
                       $"\r\n" +
                       CommandsTxt
                ;
            EmailDownloader.EmailDownloader.SendEmail( client, null, "Invoice Template Not found!", new[] {"Joseph@auto-brokerage.com"}, body, new[] {file, txtFile});
            return body;
        }




        private static void SeeWhatSticks(string pdftext)
        {
            using (var ctx = new OCRContext())
            {
                var allLines = ctx.Lines.Include(x => x.RegularExpressions).ToList();

                var goodLines = new List<Line>();
                foreach (var regLine in allLines)
                {
                    try
                    {
                        var match = Regex.Match(pdftext, regLine.RegularExpressions.RegEx,
                            (regLine.RegularExpressions.MultiLine == true
                                ? RegexOptions.Multiline
                                : RegexOptions.Singleline) | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture,
                            TimeSpan.FromSeconds(5));
                        if (match.Success)
                        {
                            var line = new Line(regLine);
                            line.Read(match.Value, 1, "SeeWhatSticks");
                            if (line.Values.Any(x => x.Value.Values.Any())) goodLines.Add(line);
                        }
                    }
                    catch (RegexMatchTimeoutException e)
                    {

                    }
                    catch (Exception e)
                    {

                        Console.WriteLine(e);
                        throw;

                    }
                }

                foreach (var line in goodLines)
                {

                }

            }
        }

        public static string GetImageTxt(string directoryName)
        {
            
               var str = PdfOcr.GetTextFromImage(PageSegMode.SingleColumn, directoryName, Path.Combine(directoryName, "AllImagetxt"), false);
            

            return str;
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
                    //if ((lineCount/text.Count) > MaxLinesCheckedToStart && Parts.First().WasStarted == false)
                    //    return new List<dynamic>() ;

                    lineCount += 1;
                    var iLine = new List<InvoiceLine>(){ new InvoiceLine(line, lineCount) };
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

        public double MaxLinesCheckedToStart { get; set; } = 0.5;
        private static Dictionary<string, List<BetterExpando>> table = new Dictionary<string, List<BetterExpando>>();
        private List<IDictionary<string, object>> SetPartLineValues(Part part)
        {
            try
            {


                var lst = new List<IDictionary<string, object>>();
                var itm = new BetterExpando();
                var ditm = ((IDictionary<string, object>) itm);

                

                foreach (var line in part.Lines)
                {
                    if (!line.OCR_Lines.Fields.Any()) continue;
                    var values = (line.OCR_Lines.DistinctValues == true 
                        ? DistinctValues(line.Values)
                        : line.Values).ToList();

                    if (!table.ContainsKey(line.OCR_Lines.Fields.First().EntityType) && line.OCR_Lines.IsColumn == true)
                        table.Add(line.OCR_Lines.Fields.First().EntityType, new List<BetterExpando>() { itm });
                   
                 

                        for (int i = 0; i <= values.Count - 1; i++)
                    {
                        var value = values[i];
                        if (part.OCR_Part.RecuringPart != null && part.OCR_Part.RecuringPart.IsComposite == false)
                        {
                            if (line.OCR_Lines.IsColumn != true || (line.OCR_Lines.IsColumn == true && i > table[line.OCR_Lines.Fields.First().EntityType].Count - 1))
                            {
                                itm = new BetterExpando();
                                if(line.OCR_Lines.IsColumn == true)
                                {
                                   table[line.OCR_Lines.Fields.First().EntityType].Add(itm);
                                    
                                }
                            }
                            else
                            {
                                itm = table[line.OCR_Lines.Fields.First().EntityType][i];
                            } 

                            ditm = ((IDictionary<string, object>)itm);
                        }



                        ditm["FileLineNumber"] = value.Key.lineNumber + 1;
                        ditm["Instance"] = i+1;
                        ditm["Section"] = value.Key.section;
                        foreach (var field in value.Value)
                        {
                            if (ditm.ContainsKey(field.Key.Field) && (field.Key.AppendValues == true ||line.OCR_Lines.Fields.Select(z => z.Field)
                                    .Count(f => f == field.Key.Field) > 1 ) )
                            {
                                ImportByDataType(field, ditm, value);
                            }
                            else
                            {
                                ditm[field.Key.Field] = GetValue(value, field.Key.Field);
                               //ImportByDataType(field, ditm, value);
                            }

                        }

                        if (ditm.Count == 1) continue;
                        if (part.OCR_Part.RecuringPart != null && part.OCR_Part.RecuringPart.IsComposite == false)
                            lst.Add(itm);
                    }
                }

                foreach (var childPart in part.ChildParts)
                {
                   // if (!childPart.Lines.Any()) continue;
                    var childItms = SetPartLineValues(childPart);
                    var fieldname = childPart.AllLines.First().OCR_Lines.Fields.First().EntityType;
                    if (childPart.OCR_Part.RecuringPart != null || !childPart.Lines.Any())
                    {
                        if (!part.Lines.Any())
                        {
                            lst.AddRange(childItms);
                        }
                        else
                        {
                            if (ditm.ContainsKey(fieldname))
                            {
                                ((List<IDictionary<string, object>>) ditm[fieldname]).AddRange(childItms);
                            }
                            else
                            {
                               ditm[fieldname] = childItms; 
                            }
                            
                        }
                            
                    }
                    else
                    {
                        if (!part.Lines.Any())
                        {
                            lst.Add(childItms.FirstOrDefault());
                        }
                        else
                        {
                            ditm[fieldname] = childItms.FirstOrDefault();
                        }
                        
                    }

                }


                if ((part.OCR_Part.RecuringPart == null || part.OCR_Part.RecuringPart.IsComposite) && ditm.Any()) lst.Add(itm);
                return lst;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private Dictionary<(int lineNumber, string section), Dictionary<Fields, string>> DistinctValues(Dictionary<(int lineNumber, string section), Dictionary<Fields, string>> lineValues)
        {
            var res = new Dictionary<(int lineNumber, string section), Dictionary<Fields, string>>();
            foreach (var val in lineValues.Where(val => !res.Values.Any(z => z.Values.Any(q => val.Value.ContainsValue(q)))))
            {
                res.Add((val.Key.lineNumber, val.Key.section), val.Value);
            }
            return res;
        }

        private void ImportByDataType(KeyValuePair<Fields, string> field, IDictionary<string, object> ditm, KeyValuePair<(int lineNumber, string section), Dictionary<Fields, string>> value)
        {
            switch (field.Key.DataType)
            {
                case "String":
                    ditm[field.Key.Field] =
                        (ditm[field.Key.Field] + " " + GetValueByKey(value, field.Key.Key)).Trim();
                    break;
                case "Number":
                case "Numeric":

                    if (field.Key.AppendValues == true)
                    {
                        var val = GetValueByKey(value, field.Key.Key);
                        if (ditm[field.Key.Field].ToString() !=  val.ToString())
                            ditm[field.Key.Field] =
                            Convert.ToDouble(ditm[field.Key.Field] ?? "0") +
                            Convert.ToDouble(GetValueByKey(value, field.Key.Key));
                    }
                    else
                    {
                        ditm[field.Key.Field] =
                        Convert.ToDouble(ditm[field.Key.Field] ?? "0") +
                        Convert.ToDouble(GetValueByKey(value, field.Key.Key));
                    }
                    
                    break;
                default:
                    ditm[field.Key.Field] = GetValueByKey(value, field.Key.Key);
                    break;
            }
        }

        private dynamic GetValue(KeyValuePair<(int lineNumber, string section), Dictionary<Fields, string>> z, string field)
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
        private dynamic GetValueByKey(KeyValuePair<(int lineNumber, string section), Dictionary<Fields, string>> z, string key)
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
                    var formatStrings = new List<string>() { "dd/MM/yyyy", "dd/M/yyyy", "d/MM/yyyy", "d/M/yyyy","M/yyyy", "MMMM d, yyyy", "dd.MM.yyyy" };
                    foreach (String formatString in formatStrings)
                    {
                        if (DateTime.TryParseExact(f.Value, formatString, CultureInfo.InvariantCulture, DateTimeStyles.None,
                            out DateTime edate))
                            return edate;
                    }
                   throw new ApplicationException(
                            $"{f.Key.Field} can not convert to {f.Key.DataType} for Value:{f.Value}");
                default:
                    return f.Value;
            }
        }

        public string Format(string pdftxt)
        {
            try
            {
                foreach (var reg in OcrInvoices.RegEx.OrderBy(x => x.Id))
                {
                    pdftxt = Regex.Replace(pdftxt, reg.RegEx.RegEx, reg.ReplacementRegEx.RegEx,
                        RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture);
                }

                return pdftxt;

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
        private readonly List<InvoiceLine> _lines = new List<InvoiceLine>();

        private StringBuilder _linesTxt = new StringBuilder();
        private StringBuilder _bodyTxt = new StringBuilder();

        public Parts OCR_Part;

        public Part(Parts part)
        {
            StartCount = part.Start.Select(x => x.RegularExpressions.RegEx).Count();
            EndCount = part.End.Select(x => x.RegularExpressions.RegEx).Count();
            OCR_Part = part;
            ChildParts = part.ParentParts.Select(x => new Part(x.ChildPart)).ToList();
            Lines = part.Lines.Select(x => new Line(x)).ToList();
            lastLineRead = 0;

        }

        public List<Part> ChildParts { get; }
        public List<Line> Lines { get; }
        private int EndCount { get; }

        private int StartCount { get; }

        public bool Success => Lines.All(x =>
                                   !x.Values.SelectMany(z => z.Value).Any(z =>
                                       z.Key.IsRequired && string.IsNullOrEmpty(z.Value.ToString())))
                               //&& this.Lines.Any()
                               && !this.FailedLines.Any()
                               && ChildParts.All(x => x.Success == true);

        public List<Line> FailedLines => Lines.Where(x => x.OCR_Lines.Fields.Any(z => z.IsRequired) && !x.Values.Any())
            .ToList()
            .Union(ChildParts.SelectMany(x => x.FailedLines)).ToList();

        public List<Dictionary<string, List<KeyValuePair<Fields, string>>>> FailedFields =>
            Lines.SelectMany(x => x.FailedFields).ToList();
        //public List<Dictionary<string, List<KeyValuePair<Fields, string>>>> FailedFields => Lines
        //                                                  .Where(x => x.Values.SelectMany(z => z.Value).Any(z => z.Key.IsRequired && string.IsNullOrEmpty(z.Value.ToString())))
        //                                                  .Select(x => x.Values.SelectMany(z => z.Value.ToList())
        //                                                                        .GroupBy(g => g.Key.Field)
        //                                                                        .ToDictionary(k => k.Key, v => v.ToList())
        //).ToList();

        public List<Line> AllLines =>
            Lines.Union(ChildParts.SelectMany(x => x.AllLines)).DistinctBy(x => x.OCR_Lines.Id).ToList();

        public bool WasStarted => this._startlines.Any();//{ get; set; } //;

        private static int lastLineRead = 0;

        private static Dictionary<string, string> Sections = new Dictionary<string, string>()
        {
            { "Single", "---Single Column---" },
            { "Sparse", "---SparseText---" },
            { "Ripped", "---Ripped Text---" }
        };

        public static string Section { get; set; }
        public void Read(List<InvoiceLine> newlines)
        {
            try
            {

                Sections.ForEach(s =>
                {
                    if(newlines.Any(x => x.Line.Contains(s.Value))) Section = s.Key;
                });

                if ((_endlines.Count == EndCount && EndCount > 0)
                    || (EndCount == 0 && OCR_Part.Start.Any(x => x.RegularExpressions?.MultiLine == true)
                        ? _startlines.Count >= StartCount
                        : _startlines.Count > StartCount && StartCount > 0)
                ) //attempting to do start to start
                {
                    if (OCR_Part.RecuringPart != null)
                    {
                        if (!OCR_Part.RecuringPart.IsComposite)
                        {
                            _startlines.Clear();
                            _endlines.Clear();
                            _bodylines.Clear();
                            _bodyTxt.Clear();
                            lastLineRead = _lines.LastOrDefault()?.LineNumber??0;
                            _lines.Clear();
                            _linesTxt.Clear();
                        }

                    }
                    else
                    {

                        return;
                    }


                }


                //if (_lines.FirstOrDefault(x => x.LineNumber == newlines.First().LineNumber) == null)
                //{
                //    _lines.AddRange(newlines);
                //    newlines.ForEach(x => _linesTxt.AppendLine(x.Line));
                //}

                var newInvoiceLines = newlines.Where(x => (_lines.LastOrDefault()?.LineNumber?? lastLineRead) < x.LineNumber).ToList();
                _lines.AddRange(newInvoiceLines);
                newInvoiceLines.ForEach(x => _linesTxt.AppendLine(x.Line));


                var startFound = FindStart();
                if (startFound)
                {
                   // WasStarted = true;
                    if (!WasStarted || (WasStarted && OCR_Part.RecuringPart != null))
                        if (_startlines.Count() < StartCount)
                            _startlines.Add(_lines.First());
                        else if (_startlines.Count() == StartCount) // treat as start tot start
                        {
                            _startlines.Clear();
                            _endlines.Clear();
                            _bodyTxt.Clear();
                            _bodylines.Clear();
                            if (StartCount != 0) _startlines.Add(_lines.First());
                        }
                }

                if (_startlines.Count() == StartCount &&
                    ((_endlines.Count < EndCount && EndCount > 0) || EndCount == 0))
                {


                    _bodylines.AddRange(_lines);
                    _lines.ForEach(x => _bodyTxt.AppendLine(x.Line));
                    ChildParts.ForEach(x => x.Read(_lines));
                    Lines.ForEach(x =>
                    {
                        if (x.OCR_Lines.RegularExpressions.MultiLine == true)
                            x.Read(_bodyTxt.ToString(), _bodylines.First().LineNumber, Section);
                        else x.Read(_bodylines.Last().Line, _bodylines.Last().LineNumber, Section);
                    });

                }

                if (_startlines.Count() == StartCount
                    && _startlines.All(x => _lines.All(l => l.LineNumber != x.LineNumber))
                    && _endlines.Count() < EndCount && OCR_Part.End.Any(z => Regex
                        .Match(_linesTxt.ToString(),
                            z.RegularExpressions.RegEx,
                            (z.RegularExpressions.MultiLine == true
                                ? RegexOptions.Multiline
                                : RegexOptions.Singleline) | RegexOptions.IgnoreCase).Success))
                {
                    _endlines.Add(_lines.LastOrDefault());
                }


                if (OCR_Part.Start.All(z => z.RegularExpressions?.MultiLine != true))
                {
                    _lines.Clear();
                    _linesTxt.Clear();
                }

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }

        }

        private bool FindStart()
        {
            try
            {


                if (!OCR_Part.Start.Any()) return true;

                if (WasStarted && OCR_Part.RecuringPart?.IsComposite == true) return false;

                foreach (var z in OCR_Part.Start)
                {
                    var match = Regex
                        .Match(_linesTxt.ToString().TrimEnd('\r'),
                            z.RegularExpressions.RegEx,
                            (z.RegularExpressions.MultiLine == true
                                ? RegexOptions.Multiline
                                : RegexOptions.Singleline) | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                    if (match.Success)
                    {

                        if (z.RegularExpressions?.MultiLine == true)
                        {
                            var sline = new List<InvoiceLine>();
                            var lines = _lines.OrderByDescending(x => x.LineNumber).ToList();
                            var matchLines = match.Value.Split(new char[] {'\r', '\n'}, StringSplitOptions.RemoveEmptyEntries);
                            for (var index = 0; index < lines.Count; index++)
                            {
                                if (sline.Count(x => !string.IsNullOrEmpty(x.Line)) > matchLines.Count(x => !string.IsNullOrEmpty(x)) + 1) return false;
                                var line = lines[index];
                                if(match.Value.Contains(line.Line) || matchLines.Any(x => line.Line.Contains(x))) sline.Add(line);
                                if (!sline.OrderBy(x => x.LineNumber).Select(x => x.Line).DefaultIfEmpty("")
                                    .Aggregate((c, n) => c + "\r\n" + n).Contains(match.Value.Trim())) continue;

                                _lines.Clear();
                                _lines.AddRange(sline.OrderBy(x => x.LineNumber));
                                return true;

                            }
                        }

                        return true;
                    }
                }

                return false;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
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

        private int Instance { get; set; } = 0;

        public bool Read(string line, int lineNumber, string section)
        {
            try
            {
                Instance += 1;
                var matches = Regex.Matches(line, OCR_Lines.RegularExpressions.RegEx,
                    (OCR_Lines.RegularExpressions.MultiLine == true
                        ? RegexOptions.Multiline
                        : RegexOptions.Singleline) | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                if (matches.Count == 0) return false;
                var values = new Dictionary<Fields, string>();
                foreach (Match match in matches)
                {


                    


                    foreach (var field in OCR_Lines.Fields.Where(x => x.ParentField == null))
                    {
                        var value = field.FieldValue?.Value ?? match.Groups[field.Key].Value.Trim();
                        foreach (var reg in field.FormatRegEx)
                        {
                            value = Regex.Replace(value, reg.RegEx.RegEx, reg.ReplacementRegEx.RegEx,
                                (OCR_Lines.RegularExpressions.MultiLine == true
                                    ? RegexOptions.Multiline
                                    : RegexOptions.Singleline) | RegexOptions.IgnoreCase |
                                RegexOptions.ExplicitCapture);
                        }


                        if (values.ContainsKey(field) )
                        {
                            if (/*OCR_Lines.Parts.RecuringPart != null && OCR_Lines.Parts.RecuringPart.IsComposite == true && 
                                 Took this out becasue marineco has two details which combining
                                 */ OCR_Lines.DistinctValues.GetValueOrDefault() != true) continue;
                            values[field] = values[field] + " " + value.Trim(); 
                        }
                        else
                        {
                           values.Add(field, value.Trim()); 
                        }
                        

                        foreach (var childField in field.ChildFields)
                        {
                            ReadChildField(childField, values,
                                values.Where(x => x.Key.Field == field.Field).Select(x => x.Value).DefaultIfEmpty("")
                                    .Aggregate((o, n) => o + " " + n));
                        }

                    }
                    

                }

                Values[(lineNumber, section)] = values;
                return true;
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }

        private void ReadChildField(Fields childField, Dictionary<Fields, string> values, string strValue)
        {
         
            var match = Regex.Match(strValue.Trim(), childField.Lines.RegularExpressions.RegEx,
                (childField.Lines.RegularExpressions.MultiLine == true
                    ? RegexOptions.Multiline
                    : RegexOptions.Singleline) | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
            if (!match.Success) return;
            foreach (var field in childField.Lines.Fields)
            {


                var value = field.FieldValue?.Value ?? match.Groups[field.Key].Value.Trim();
                foreach (var reg in field.FormatRegEx)
                {
                    value = Regex.Replace(value, reg.RegEx.RegEx, reg.ReplacementRegEx.RegEx,
                        (OCR_Lines.RegularExpressions.MultiLine == true
                            ? RegexOptions.Multiline
                            : RegexOptions.Singleline) | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture);
                }
                values.Add(field, value.Trim());
            }
        }

        public Dictionary<(int lineNumber, string section), Dictionary<Fields, string>> Values { get; } = new Dictionary<(int lineNumber, string section), Dictionary<Fields, string>>();
        //public bool MultiLine => OCR_Lines.MultiLine;

        public List<Dictionary<string, List<KeyValuePair<Fields, string>>>> FailedFields => this.Values
            .Where(x => x.Value.Any(z => z.Key.IsRequired && string.IsNullOrEmpty(z.Value.ToString())))
            .SelectMany(x => x.Value.ToList())
            .DistinctBy(x => x.Key.Id)
            .GroupBy(x => $"{x.Key.Field}-{x.Key.Key}")
            .DistinctBy(x => x.Key)
            .Select(x => x.ToDictionary(k => x.Key, v => x.ToList()))
            .ToList();

    }

   
}
