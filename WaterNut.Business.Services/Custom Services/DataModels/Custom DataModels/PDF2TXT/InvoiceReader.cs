//﻿using System;
//using System.Collections.Generic;
//using System.Data.Entity;
//using System.Dynamic;
//using System.IO;
//using System.Linq;
//using System.Linq.Expressions;
//using System.Net.Sockets;
//using System.Text;
//using System.Text.RegularExpressions;
//using System.Threading;
//using System.Threading.Tasks;
//using System.Threading.Tasks.Schedulers;
//using Core.Common;
//using Core.Common.Extensions;
//using Core.Common.Utils;
//using CoreEntities.Business.Entities;
//using DocumentDS.Business.Entities;
//using EmailDownloader;
//using MoreLinq;
//using org.apache.pdfbox.pdmodel;
//using org.apache.pdfbox.util;
//using OCR.Business.Entities;
//using pdf_ocr;
//using SimpleMvvmToolkit.ModelExtensions;
//using Tesseract;
//using TrackableEntities;
//using UglyToad.PdfPig;
//using UglyToad.PdfPig.Content;
//using UglyToad.PdfPig.DocumentLayoutAnalysis.PageSegmenter;
//using UglyToad.PdfPig.DocumentLayoutAnalysis.ReadingOrderDetector;
//using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor;
//using UglyToad.PdfPig.DocumentLayoutAnalysis.WordExtractor;
//using WaterNut.Business.Services.Utils;
//using WaterNut.DataSpace;
//using AsycudaDocumentSet_Attachments = CoreEntities.Business.Entities.AsycudaDocumentSet_Attachments;
//using Attachments = CoreEntities.Business.Entities.Attachments;
//using FileTypes = CoreEntities.Business.Entities.FileTypes;
//using Invoices = OCR.Business.Entities.Invoices;
//using org.apache.pdfbox.pdmodel.font;
//using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;

//namespace WaterNut.DataSpace
//{


//    public class InvoiceReader
//    {
//        public const string CommandsTxt = "Commands:\r\n" +
//                                          "UpdateRegex: RegId: 000, Regex: 'xyz', IsMultiline: True\r\n" +
//                                          "AddFieldRegEx: RegId: 000,  Field: Name, Regex: '', ReplaceRegex: ''\r\n" +
//                                          "RequestInvoice: Name:'xyz'\r\n" +
//                                          "AddInvoice: Name:'', IDRegex:''\r\n" +
//                                          "AddPart: Template:'', Name: '', StartRegex: '', ParentPart:'', IsRecurring: True, IsComposite: False, IsMultiLine: True \r\n" +
//                                          "AddLine: Template:'',  Part: '', Name: '', Regex: ''\r\n" +
//                                          "UpdateLine: Template:'',  Part: '', Name: '', Regex: ''\r\n" +
//                                          "AddFieldFormatRegex: RegexId: 000, Keyword:'', Regex:'', ReplaceRegex:'', ReplacementRegexIsMultiLine: True, RegexIsMultiLine: True\r\n";

//        public static async Task<Dictionary<string, (string file, string, ImportStatus Success)>> Import(string file, int fileTypeId, string emailId, bool overWriteExisting,
//            List<AsycudaDocumentSet> docSet, FileTypes fileType, Client client)
//        {
//            Console.WriteLine($"[OCR DEBUG] InvoiceReader.Import: Starting import for file '{file}', FileTypeId: {fileTypeId}, EmailId: '{emailId}'");
//            var imports = new Dictionary<string, (string file, string, ImportStatus Success)>();
//            //Get Text
//            try
//            {

                
//                var pdfTxt = await GetPdftxt(file).ConfigureAwait(false);

                
//                //Get Template
//                var templates = GetTemplates(x => true);

//                var possibleInvoices = GetPossibleInvoices(templates, pdfTxt).ToList();
//                Console.WriteLine($"[OCR DEBUG] InvoiceReader.Import: Found {possibleInvoices.Count} possible invoice templates.");
//                foreach (var tmp in possibleInvoices)//.Where(x => x.OcrInvoices.Id == 117)
//                {
//                    Console.WriteLine($"[OCR DEBUG] InvoiceReader.Import: Attempting template '{tmp.OcrInvoices.Name}' (ID: {tmp.OcrInvoices.Id})");
//                    try
//                    {
//                        var importStatus = TryReadFile(file, emailId, fileType, pdfTxt, client, overWriteExisting, docSet, tmp,
//                            fileTypeId, tmp == possibleInvoices.Last());
//                        Console.WriteLine($"[OCR DEBUG] InvoiceReader.Import: TryReadFile result for template '{tmp.OcrInvoices.Name}': {importStatus}");
//                        var fileDescription = FileTypeManager.GetFileType(tmp.OcrInvoices.FileTypeId).Description;
//                        switch (importStatus)
//                        {
//                            case ImportStatus.Success:
                                
//                                imports.Add($"{file}-{tmp.OcrInvoices.Name}-{possibleInvoices.IndexOf(tmp)}", (file,  FileTypeManager.EntryTypes.GetEntryType(fileDescription), ImportStatus.Success));
//                                break;
//                            case ImportStatus.HasErrors:
//                                imports.Add($"{file}-{tmp.OcrInvoices.Name}-{possibleInvoices.IndexOf(tmp)}", (file, FileTypeManager.EntryTypes.GetEntryType(fileDescription), ImportStatus.HasErrors));
//                                break;
//                            case ImportStatus.Failed:
//                                await ReportUnImportedFile(docSet, file, emailId, fileTypeId, client, pdfTxt.ToString(), "No template found for this File", new List<Line>()).ConfigureAwait(false);
//                                imports.Add($"{file}-{tmp.OcrInvoices.Name}-{possibleInvoices.IndexOf(tmp)}", (file, FileTypeManager.EntryTypes.GetEntryType(fileDescription), ImportStatus.Failed));
//                                break;
//                        }
//                    }
//                    catch (Exception e)
//                    {
//                        Exception realerror = e;
//                        while (realerror.InnerException != null)
//                            realerror = realerror.InnerException;

//                        var failedLines = tmp.Lines.Where(x => x.OCR_Lines.Fields.Any(z => realerror.Message.Contains(z.Field)) || realerror.Message.Contains(x.OCR_Lines.Name))
//                            .ToList();
//                        if (!failedLines.Any()) failedLines = tmp.Lines.Where(x => x.OCR_Lines.Fields.Any(z => z.DataType == "Number" )).ToList();
//                        await ReportUnImportedFile(docSet, file, emailId, fileTypeId, client, pdfTxt.ToString(), $"Problem importing file:{file} --- {realerror.Message}", failedLines).ConfigureAwait(false);

//                        var ex = new ApplicationException($"Problem importing file:{file} --- {realerror.Message}", e);
//                        Console.WriteLine(ex);
//                        continue;
//                        // return false;
//                    }

//                }

//               // if(possibleInvoices.Where(x => x.OcrInvoices.))

//                   // SeeWhatSticks(pdfTxt.ToString());
                

//                Console.WriteLine($"[OCR DEBUG] InvoiceReader.Import: Finished import for file '{file}'. Results: {imports.Count} entries.");
//                return imports;
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine(e);
//                var fileInfo = new FileInfo(file);
//                var errTxt = $"Hey,\r\n\r\n The System Found a problem with this File-'{fileInfo.Name}'.\r\n" +
//                             $"{e.Message}" +
//                             "Check the file again or Check Joseph Bartholomew at Joseph@auto-brokerage.com to make the necessary changes.\r\n" +
//                             "Thanks\r\n" +
//                             $"AutoBot";
//                await EmailDownloader.EmailDownloader.SendBackMsgAsync(emailId, client, errTxt).ConfigureAwait(false);
                
//                return imports;
//            }
//        }

//        private static IEnumerable<Template> GetPossibleInvoices(List<Template> templates, StringBuilder pdfTxt)
//        {
//            return
//                templates
//                    .OrderBy(x => !x.OcrInvoices.Name.ToUpper().Contains("Tropical".ToUpper()))
//                    .ThenBy(x => x.OcrInvoices.Id)
//                    .Where(tmp => IsInvoiceDocument(tmp.OcrInvoices, pdfTxt.ToString()))
//                    .ToList(); //;
//        }

//        public static List<Template> GetTemplates(Expression<Func<Invoices, bool>> filter)
//        {
//            List<Template> templates;
//            using (var ctx = new OCRContext())
//            {
//                templates = ctx.Invoices
//                    .Include(x => x.Parts)
//                    .Include("TemplateIdentificatonRegEx.OCR_RegularExpressions")
//                    .Include("RegEx.RegEx")
//                    .Include("RegEx.ReplacementRegEx")
//                    .Include("Parts.RecuringPart")
//                    .Include("Parts.Start.RegularExpressions")
//                    .Include("Parts.End.RegularExpressions")
//                    .Include("Parts.PartTypes")
//                    .Include("Parts.ChildParts.ChildPart.Start.RegularExpressions")
//                    .Include("Parts.ParentParts.ParentPart.Start.RegularExpressions")
//                    .Include("Parts.Lines.RegularExpressions")
//                    .Include("Parts.Lines.Fields.FieldValue")
//                    .Include("Parts.Lines.Fields.FormatRegEx.RegEx")
//                    .Include("Parts.Lines.Fields.FormatRegEx.ReplacementRegEx")
//                    .Include("Parts.Lines.Fields.ChildFields.FieldValue")
//                    .Include("Parts.Lines.Fields.ChildFields.FormatRegEx.RegEx")
//                    .Include("Parts.Lines.Fields.ChildFields.FormatRegEx.ReplacementRegEx")
//                    .Where(x => x.IsActive)
//                    .Where(x => x.ApplicationSettingsId ==
//                                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
//                    .Where(filter) //BaseDataModel.Instance.CurrentApplicationSettings.TestMode != true ||
//                    .ToList()
//                    .Select(x => new Template(x)).ToList();
//            }

//            return templates;
//        }

//        public static ImportStatus TryReadFile(string file, string emailId, FileTypes fileType, StringBuilder pdftxt,
//            Client client, bool overWriteExisting, List<AsycudaDocumentSet> docSet,
//            Template tmp, int fileTypeId, bool isLastdoc)
//        {
//            try
//            {


//                Console.WriteLine(
//                    $"[OCR DEBUG] InvoiceReader.TryReadFile: Processing template '{tmp.OcrInvoices.Name}' (ID: {tmp.OcrInvoices.Id}) for file '{file}'");

//                var formattedPdfTxt = tmp.Format(pdftxt.ToString());
//                Console.WriteLine(
//                    $"[OCR DEBUG] InvoiceReader.TryReadFile: PDF text formatted using template {tmp.OcrInvoices.Id}.");

//                var csvLines = tmp.Read(formattedPdfTxt);
//                Console.WriteLine(
//                    $"[OCR DEBUG] InvoiceReader.TryReadFile: tmp.Read returned {csvLines?.Count ?? 0} data structures.");
//                if (csvLines != null && csvLines.Count > 0 && csvLines[0] is List<IDictionary<string, object>> list)
//                {
//                    Console.WriteLine(
//                        $"[OCR DEBUG] InvoiceReader.TryReadFile: First data structure contains {list.Count} items.");
//                }

//                AddNameSupplier(tmp, csvLines);

//                AddMissingRequiredFieldValues(tmp, csvLines);

//                WriteTextFile(file, formattedPdfTxt);


//                if (csvLines == null || csvLines.Count < 1 || !tmp.Success)
//                {
//                    Console.WriteLine(
//                        $"[OCR DEBUG] InvoiceReader.TryReadFile: Read failed or returned no lines. tmp.Success = {tmp.Success}. Entering ErrorState.");
//                    var errorResult = ErrorState(file, emailId, formattedPdfTxt, client, docSet, tmp, fileTypeId,
//                        isLastdoc);
//                    Console.WriteLine($"[OCR DEBUG] InvoiceReader.TryReadFile: ErrorState returned {errorResult}.");
//                    return errorResult ? ImportStatus.HasErrors : ImportStatus.Failed;
//                }
//                else
//                {
//                    Console.WriteLine(
//                        $"[OCR DEBUG] InvoiceReader.TryReadFile: Read successful. Entering ImportSuccessState.");
//                    var successResult = ImportSuccessState(file, emailId, fileType, overWriteExisting, docSet, tmp,
//                        csvLines);
//                    Console.WriteLine(
//                        $"[OCR DEBUG] InvoiceReader.TryReadFile: ImportSuccessState returned {successResult}.");
//                    return successResult == true ? ImportStatus.Success : ImportStatus.HasErrors;
//                }
//            }
//            catch (Exception e)
//            {
//                Console.WriteLine(e);
//                throw;
//            }

//        }

//        private static void AddNameSupplier(Template tmp, List<dynamic> csvLines)
//        {
//            if(csvLines.Count == 1 && !tmp.Lines.All(x => "Name, SupplierCode".Contains(x.OCR_Lines.Name)))
//                foreach (var doc in ((List<IDictionary<string, object>>) csvLines.First()))
//                {
//                    if (!doc.Keys.Contains("SupplierCode")) doc.Add("SupplierCode", tmp.OcrInvoices.Name);
//                    if (!doc.Keys.Contains("Name")) doc.Add("Name", tmp.OcrInvoices.Name);
//                }
//        }

//        private static void AddMissingRequiredFieldValues(Template tmp, List<dynamic> csvLines)
//        {
//            var requiredFieldsList = tmp.Lines.SelectMany(x => x.OCR_Lines.Fields)
//                .Where(z => z.IsRequired && !string.IsNullOrEmpty(z.FieldValue?.Value)).ToList();
//            foreach (var field in requiredFieldsList)
//            {
//                foreach (var doc in ((List<IDictionary<string, object>>)csvLines.First()).Where(doc => !doc.Keys.Contains(field.Field)))
//                {
//                    doc.Add(field.Field, field.FieldValue.Value);
//                }
//            }
//        }

//        private static bool ImportSuccessState(string file, string emailId, FileTypes fileType, bool overWriteExisting,
//            List<AsycudaDocumentSet> docSet, Template tmp, List<dynamic> csvLines)
//        {
//            if (fileType.Id != tmp.OcrInvoices.FileTypeId)
//                fileType = FileTypeManager.GetFileType(tmp.OcrInvoices.FileTypeId);


//          return  new DataFileProcessor().Process( new DataFile(fileType, docSet, overWriteExisting,
//                emailId,
//                file, csvLines)).GetAwaiter().GetResult();

            
//        }

//        private static bool ErrorState(string file, string emailId, string pdftxt, Client client,
//            List<AsycudaDocumentSet> docSet,
//            Template tmp, int fileTypeId, bool isLastdoc)
//        {
//            var failedlines = tmp.Lines.DistinctBy(x => x.OCR_Lines.Id).Where(z =>
//                z.FailedFields.Any() || (z.OCR_Lines.Fields.Any(f => f.IsRequired && f.FieldValue?.Value == null) && !z.Values.Any())).ToList();

//            failedlines.AddRange(tmp.Parts.SelectMany(z => z.FailedLines).ToList());

//            var allRequried = tmp.Lines.DistinctBy(x => x.OCR_Lines.Id).Where(z =>
//                z.OCR_Lines.Fields.Any(f => f.IsRequired && (f.Field != "SupplierCode" && f.Field != "Name"))).ToList();


            
//            if (
//                //---------Auto Add name and supplier code make this check redundant
//                //!tmp.Parts.Any(x => x.AllLines.Any(z =>
//                //    z.Values.Values.Any(v =>
//                //        v.Keys.Any(k => k.fields.Field == "Name") &&
//                //        v.Values.Any(kv => kv == tmp.OcrInvoices.Name))))) ||
//                failedlines.Count >= allRequried.Count && allRequried.Count > 0) return false;

//            if (//isLastdoc && 
//                failedlines.Any() && failedlines.Count < tmp.Lines.Count &&
//                (tmp.Parts.First().WasStarted || !tmp.Parts.First().OCR_Part.Start.Any()) &&
//                tmp.Lines.SelectMany(x => x.Values.Values).Any())
//            {
//                 ReportUnImportedFile(docSet, file, emailId, fileTypeId, client, pdftxt.ToString(),
//                    "Following fields failed to import",
//                    failedlines).GetAwaiter().GetResult();
                
//                    return true;
                
//            }


//            return false;
//        }

//        public static bool IsInvoiceDocument(Invoices invoice, string fileText)
//        {
//            return invoice.TemplateIdentificatonRegEx.Any() && invoice.TemplateIdentificatonRegEx.Any(x =>
//                Regex.IsMatch(fileText,
//                    x.OCR_RegularExpressions.RegEx,
//                    RegexOptions.IgnoreCase |RegexOptions.Multiline | RegexOptions.ExplicitCapture));
//        }

//        public static async Task<StringBuilder> GetPdftxt(string file)
//        {
//            StringBuilder pdftxt = new StringBuilder();


//            //pdftxt = parseUsingPDFBox(file);
//            var ripTask = Task.Run(() =>
//            {
//                var txt = "------------------------------------------Ripped Text-------------------------\r\n";
//                txt += PdfPigText(file); //TODO: need to implement the layout logic
//                return txt;
//            });


//            var singleColumnTask = Task.Run(() =>
//            {
//                var txt =
//                    "------------------------------------------Single Column-------------------------\r\n";
//                txt += new PdfOcr().Ocr(file, PageSegMode.SingleColumn);
//                return txt;
//            });

//            var sparseTextTask = Task.Run(() =>
//            {
//                var txt = "------------------------------------------SparseText-------------------------\r\n";
//                txt += new PdfOcr().Ocr(file, PageSegMode.SparseText);
//                return txt;
//            });



//            await Task.WhenAll(ripTask, singleColumnTask, sparseTextTask).ConfigureAwait(false); //RawLineTextTask, OsdOnlyTextTask, SingleWordTextTask, sparsOsdTextTask

//            pdftxt.AppendLine(singleColumnTask.Result);
//            pdftxt.AppendLine(sparseTextTask.Result);
//            pdftxt.AppendLine(ripTask.Result);
//            return pdftxt;
//        }

//        private static string PdfPigText(string file)
//        {
//            try
//            {



//                var sb = new StringBuilder();
//                using (var pdf = PdfDocument.Open(file))
//                {
//                    foreach (var page in pdf.GetPages())
//                    {
//                        // Either extract based on order in the underlying document with newlines and spaces.
//                        var text = ContentOrderTextExtractor.GetText(page);
//                        sb.AppendLine(text);

//                    }

//                }

//                return sb.ToString();
//            }
//            catch (Exception)
//            {
//                //Console.WriteLine(e);
//                //throw;
                
//            }

//            return "Error readying Ripped Text";

//        }



//        private static async Task ReportUnImportedFile(List<AsycudaDocumentSet> asycudaDocumentSets, string file, string emailId,
//            int fileTypeId,
//            Client client, string pdftxt, string error,
//            List<Line> failedlst)
//        {
//            var fileInfo = new FileInfo(file);
            
//            var txtFile = WriteTextFile(file, pdftxt);
//            var body = await CreateEmail(file, client, error, failedlst, fileInfo, txtFile).ConfigureAwait(false);
//            CreateTestCase(file, failedlst, txtFile, body);


//            SaveImportError(asycudaDocumentSets, file, emailId, fileTypeId, pdftxt, error, failedlst, fileInfo);
//        }

//        private static string WriteTextFile(string file, string pdftxt)
//        {
//            var txtFile = file + ".txt";
//            //if (File.Exists(txtFile)) return;
//            File.WriteAllText(txtFile, pdftxt);
//            return txtFile;
//        }

//        private static void SaveImportError(List<AsycudaDocumentSet> asycudaDocumentSets, string file, string emailId, int fileTypeId, string pdftxt,
//            string error, List<Line> failedlst, FileInfo fileInfo)
//        {
//            var existingAttachment = GetOrSaveDocSetAttachmentsList(asycudaDocumentSets, file, emailId, fileTypeId, fileInfo);

//            SaveDbErrorDetails(pdftxt, error, failedlst, existingAttachment);
//        }

//        private static void SaveDbErrorDetails(string pdftxt, string error, List<Line> failedlst, List<AsycudaDocumentSet_Attachments> existingAttachment)
//        {
//            using (var ctx = new OCRContext())
//            {
//                foreach (var att in existingAttachment)
//                {
//                    var importErr = ctx.ImportErrors.FirstOrDefault(x => x.Id == att.Id);
//                    if (importErr == null)
//                    {
//                        importErr = CreateNewImportErrors(pdftxt, error, failedlst, att);
//                        ctx.ImportErrors.Add(importErr);
//                    }
//                    else
//                    {
//                        UpdateImportError(pdftxt, error, failedlst, importErr, att);
//                    }

//                    ctx.SaveChanges();
//                }
//            }
//        }

//        private static void UpdateImportError(string pdftxt, string error, List<Line> failedlst, ImportErrors importErr,
//            AsycudaDocumentSet_Attachments att)
//        {
//            importErr.PdfText = pdftxt;
//            importErr.Error = error;
//            importErr.EntryDateTime = DateTime.Now;
//            importErr.OCR_FailedLines = failedlst.Select(x => CreateFailedLines(att, x)).ToList();
//        }

//        private static OCR_FailedLines CreateFailedLines(AsycudaDocumentSet_Attachments att, Line x)
//        {
//            return new OCR_FailedLines(true)
//            {
//                TrackingState = TrackingState.Added,
//                DocSetAttachmentId = att.Id,
//                LineId = x.OCR_Lines.Id,
//                Resolved = false,
//                OCR_FailedFields = x.FailedFields.SelectMany(z =>
//                        z.SelectMany(q => q.Value.Select(w => w.Key)))
//                    .DistinctBy(z => z.fields.Id)
//                    .Select(z => new OCR_FailedFields(true)
//                    {
//                        TrackingState = TrackingState.Added,
//                        FieldId = z.fields.Id,
//                    })
//                    .ToList()
//            };
//        }

//        private static ImportErrors CreateNewImportErrors(string pdftxt, string error, List<Line> failedlst,
//            AsycudaDocumentSet_Attachments att)
//        {
//            ImportErrors importErr;
//            importErr = new ImportErrors(true) { Id = att.Id };
//            UpdateImportError(pdftxt,error,failedlst,importErr, att);
//            return importErr;
//        }

//        private static List<AsycudaDocumentSet_Attachments> GetOrSaveDocSetAttachmentsList(List<AsycudaDocumentSet> asycudaDocumentSets, string file, string emailId,
//            int fileTypeId, FileInfo fileInfo) =>
//            asycudaDocumentSets.SelectMany(docSet => GetDocSetAttachements(file, emailId, fileTypeId, fileInfo, docSet)).ToList();

//        private static List<AsycudaDocumentSet_Attachments> GetDocSetAttachements(string file, string emailId, int fileTypeId, FileInfo fileInfo,
//            AsycudaDocumentSet docSet)
//        {
//            var existingAttachment = GetDocSetAttachments(file, docSet);
           
//            if (existingAttachment.Any()) return existingAttachment;

//            existingAttachment.Add(CreateDocSetAttachment(file, emailId, fileTypeId, fileInfo, docSet));

//            return existingAttachment;
//        }

//        private static AsycudaDocumentSet_Attachments CreateDocSetAttachment(string file, string emailId, int fileTypeId,
//            FileInfo fileInfo, AsycudaDocumentSet docSet)
//        {
//            using (var ctx = new CoreEntitiesContext())
//            {
//                var newAttachment = new CoreEntities.Business.Entities.AsycudaDocumentSet_Attachments(true)
//                {
//                    TrackingState = TrackingState.Added,
//                    AsycudaDocumentSetId = docSet.AsycudaDocumentSetId,
//                    Attachments = new Attachments(true)
//                    {
//                        TrackingState = TrackingState.Added,
//                        EmailId = emailId,
//                        FilePath = file
//                    },
//                    DocumentSpecific = true,
//                    FileDate = fileInfo.CreationTime,
//                    FileTypeId = fileTypeId,
//                };
//                ctx.AsycudaDocumentSet_Attachments.Add(newAttachment);
//                ctx.SaveChanges();
//                return newAttachment;
//            }
//        }

//        private static List<AsycudaDocumentSet_Attachments> GetDocSetAttachments(string file, AsycudaDocumentSet docSet)
//        {
//            using (var ctx = new CoreEntitiesContext())
//            {
//                var docSetAttachments = ctx.AsycudaDocumentSet_Attachments
//                    .Include(x => x.Attachments)
//                    .Where(x =>
//                        x.AsycudaDocumentSetId == docSet.AsycudaDocumentSetId && x.Attachments.FilePath == file)
//                    .ToList();
//                return docSetAttachments;
//            }
//        }

//        private static void CreateTestCase(string file, List<Line> failedlst, string txtFile, string body)
//        {
//            dynamic testCaseData = new BetterExpando();
//            testCaseData.DateTime = DateTime.Now;
//            testCaseData.Id = failedlst.FirstOrDefault()?.OCR_Lines.Parts.Invoices.Id;
//            testCaseData.Supplier = failedlst.FirstOrDefault()?.OCR_Lines.Parts.Invoices.Name;
//            testCaseData.PdfFile = file;
//            testCaseData.TxtFile = txtFile;
//            testCaseData.Message = body;
//            //write to info
//            UnitTestLogger.Log(
//                new List<String>
//                    {FunctionLibary.NameOfCallingClass(), failedlst.FirstOrDefault()?.OCR_Lines.Parts.Invoices.Name,},
//                BaseDataModel.Instance.CurrentApplicationSettings.DataFolder, testCaseData);
//        }

//        private static async Task<string> CreateEmail(string file, Client client, string error, List<Line> failedlst,
//            FileInfo fileInfo, string txtFile)
//        {
//            var body = $"Hey,\r\n\r\n {error}-'{fileInfo.Name}'.\r\n\r\n\r\n" +
//                       $"{(failedlst.Any() ? failedlst.FirstOrDefault()?.OCR_Lines.Parts.Invoices.Name + "\r\n\r\n\r\n" : "")}"+
//                       $"{failedlst.Select(x => $"Line:{x.OCR_Lines.Name} - RegId: {x.OCR_Lines.RegularExpressions.Id} - Regex: {x.OCR_Lines.RegularExpressions.RegEx} - Fields: {x.FailedFields.SelectMany(z => z.ToList()).SelectMany(z => z.Value.ToList()).Select(z => $"{z.Key.fields.Key} - '{z.Key.fields.Field}'").DefaultIfEmpty(string.Empty).Aggregate((o, c) => o + "\r\n\r\n" + c)}").DefaultIfEmpty(string.Empty).Aggregate((o, c) => o + "\r\n" + c)}\r\n\r\n" +
//                       "Thanks\r\n" +
//                       "Thanks\r\n" +
//                       $"AutoBot\r\n" +
//                       $"\r\n" +
//                       $"\r\n" +
//                       CommandsTxt
//                ;
//            await EmailDownloader.EmailDownloader.SendEmailAsync( client, null, "Template Template Not found!", EmailDownloader.EmailDownloader.GetContacts("Developer"), body, new[] {file, txtFile}).ConfigureAwait(false);
//            return body;
//        }

        


//        //private static void SeeWhatSticks(string pdftext)
//        //{
//        //    using (var ctx = new OCRContext())
//        //    {
//        //        var allLines = ctx.Lines.Include(x => x.RegularExpressions).ToList();

//        //        var goodLines = new List<Line>();
//        //        foreach (var regLine in allLines)
//        //        {
//        //            try
//        //            {
//        //                var match = Regex.Match(pdftext, regLine.RegularExpressions.RegEx,
//        //                    (regLine.RegularExpressions.MultiLine == true
//        //                        ? RegexOptions.Multiline
//        //                        : RegexOptions.Singleline) | RegexOptions.IgnoreCase | RegexOptions.ExplicitCapture,
//        //                    TimeSpan.FromSeconds(5));
//        //                if (match.Success)
//        //                {
//        //                    var line = new Line(regLine);
//        //                    line.Read(match.Value, 1, "SeeWhatSticks",1);
//        //                    if (line.Values.Any(x => x.Value.Values.Any())) goodLines.Add(line);
//        //                }
//        //            }
//        //            catch (RegexMatchTimeoutException e)
//        //            {

//        //            }
//        //            catch (Exception e)
//        //            {

//        //                Console.WriteLine(e);
//        //                throw;

//        //            }
//        //        }

//        //        foreach (var line in goodLines)
//        //        {

//        //        }

//        //    }
//        //}

//        public static string GetImageTxt(string directoryName)
//        {
            
//               var str = PdfOcr.GetTextFromImage(PageSegMode.SingleColumn, directoryName, Path.Combine(directoryName, "AllImagetxt"), false);
            

//            return str;
//        }
//    }
//}
