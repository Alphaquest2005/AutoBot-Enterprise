using AutoBot;
using CoreEntities.Business.Entities;
using System;
using System.Collections.Generic;
using System.Data.Entity; // Added for Include
using System.IO;
using System.Linq;
using System.Linq.Expressions; // Added for Expression
using System.Text;
using System.Text.RegularExpressions; // Added for Regex
using System.Threading.Tasks;
using WaterNut.DataSpace;
using WaterNut.DataSpace.PipelineInfrastructure;
using Utils = AutoBot.Utils;
using Serilog;
using MoreLinq; // Added for DistinctBy, ForEach
using OCR.Business.Entities; // Added for OCRContext, Invoices etc.
using Tesseract; // Added for PageSegMode
using UglyToad.PdfPig; // Added for PdfDocument
using UglyToad.PdfPig.Content; // Added for Page
using UglyToad.PdfPig.DocumentLayoutAnalysis.TextExtractor; // Added for ContentOrderTextExtractor
using pdf_ocr; // Added for PdfOcr
using WaterNut.Business.Services.Utils; // Added for FileTypeManager
using Core.Common; // Added for BaseDataModel
using Core.Common.Utils; // Added for Client
using DocumentDS.Business.Entities; // Added for AsycudaDocumentSet

// Renaming to avoid ambiguity with WaterNut.DataSpace.Invoice
using OCRInvoice = OCR.Business.Entities.Invoices;
using OCRLine = OCR.Business.Entities.Lines;
using OCRPart = OCR.Business.Entities.Parts;
using OCRField = OCR.Business.Entities.Fields;
// Removed unused alias causing compilation error
using OCRInvoiceIdRegEx = OCR.Business.Entities.InvoiceIdentificatonRegEx;

// Define a wrapper for the OCR template to be used locally
using LocalInvoiceTemplate = OCR.Business.Entities.Invoices; // Corrected: Plural based on usage in GetTemplates


namespace InvoiceReader
{
    public class InvoiceReader
    {
        private static readonly ILogger _logger = Log.ForContext<InvoiceReader>();

        // Main entry point - Refactored to use content-based matching first
        public static async Task<List<KeyValuePair<string, (string file, string DocumentType, ImportStatus Status)>>> ImportPDF(FileInfo[] pdfFiles, FileTypes initialFileType)
        {
            _logger.Information("Starting PDF import process for {FileCount} files. Initial FileType Hint: {FileTypeName} (ID: {FileTypeId})",
                pdfFiles.Length, initialFileType.Description, initialFileType.Id);

            // Corrected Tuple Definition
            List<KeyValuePair<string, (string file, string DocumentType, ImportStatus Status)>> allImports = new List<KeyValuePair<string, (string file, string DocumentType, ImportStatus Status)>>();

            // Load all active OCR templates once
            _logger.Debug("Loading all active OCR templates from database.");
            var allTemplates = GetTemplates(x => true); // Load all active templates
            _logger.Debug("Loaded {TemplateCount} active templates.", allTemplates.Count);

            foreach (var fileInfo in pdfFiles.Where(x => x.Extension.ToLower() == ".pdf"))
            {
                string file = fileInfo.FullName;
                _logger.Information("Processing file: {FileName}", fileInfo.Name);
                _logger.Verbose("File details - Path: {FilePath}, Size: {Size} bytes, LastWriteTime: {LastWriteTime}",
                    file, fileInfo.Length, fileInfo.LastWriteTime);

                string emailId = null;
                int? initialFileTypeId = initialFileType.Id; // Use the provided hint

                // Resolve initial EmailId (can be overridden by attachment info)
                using (var ctx = new CoreEntitiesContext())
                {
                    _logger.Debug("Querying database for existing attachments matching file path: {FilePath}", file);
                    var res = ctx.AsycudaDocumentSet_Attachments.Where(x => x.Attachments.FilePath == file)
                        .Select(x => new { x.EmailId }).FirstOrDefault(); // Only need EmailId here
                    emailId = res?.EmailId ?? initialFileType.EmailId;
                    _logger.Debug("Resolved EmailId: {EmailId}", emailId);
                }

                try
                {
                    _logger.Debug("Entering main processing block for file: {FileName}", fileInfo.Name); // Added log

                    // 1. Extract Text
                    _logger.Information("Attempting text extraction from PDF: {FileName}", fileInfo.Name); // Modified log
                    var pdfTxt = await GetPdftxt(file).ConfigureAwait(false);
                    _logger.Debug("Text extraction task completed for {FileName}. Result is null: {IsResultNull}, Length: {Length}",
                        fileInfo.Name, pdfTxt == null, pdfTxt?.Length ?? 0); // Added log

                    if (pdfTxt == null || pdfTxt.Length == 0)
                    {
                        _logger.Warning("Text extraction failed or returned empty for file: {FileName}. Skipping file.", fileInfo.Name);
                        // Corrected Add call with KeyValuePair and named tuple elements
                        var failureTuple = (file: file, DocumentType: initialFileType.Description, Status: ImportStatus.Failed);
                        allImports.Add(new KeyValuePair<string, (string file, string DocumentType, ImportStatus Status)>($"{file}-TextExtractionFailed", failureTuple));
                        continue;
                    }
                    _logger.Information("Text extraction completed for: {FileName}. Length: {Length}", fileInfo.Name, pdfTxt.Length);
                    WriteTextFile(file, pdfTxt.ToString()); // Write extracted text for debugging

                    // 2. Identify Possible Templates based on Content
                    _logger.Debug("Attempting to identify possible templates based on content matching for: {FileName}", fileInfo.Name); // Modified log
                    var possibleInvoices = GetPossibleInvoices(allTemplates, pdfTxt);
                    _logger.Information("Content matching finished. Found {PossibleCount} possible templates for file: {FileName}", possibleInvoices.Count, fileInfo.Name); // Modified log

                    if (!possibleInvoices.Any())
                    {
                        _logger.Warning("No matching OCR templates found based on content for file: {FileName}. Skipping file.", fileInfo.Name);
                        // Corrected Add call
                        var failureTuple = (file: file, DocumentType: initialFileType.Description, Status: ImportStatus.Failed);
                        allImports.Add(new KeyValuePair<string, (string, string, ImportStatus)>($"{file}-NoTemplateMatch", failureTuple));
                        continue;
                    }

                    // 3. Iterate and Process Matched Templates using the Pipeline
                    bool importSucceededForFile = false;
                    foreach (var matchedTemplate in possibleInvoices)
                    {
                        // Corrected: Access properties directly on matchedTemplate
                        _logger.Information("Attempting processing with matched template: '{TemplateName}' (OCR ID: {TemplateId}) for file: {FileName}",
                            matchedTemplate.Name, matchedTemplate.Id, fileInfo.Name);

                        InvoiceProcessingContext context = null;
                        try
                        {
                            // 3a. Get FileType info associated with the matched OCR template
                            // Corrected: Access FileTypeId directly
                            var matchedFileTypeId = matchedTemplate.FileTypeId;
                            FileTypes matchedFileType = FileTypeManager.GetFileType(matchedFileTypeId);
                            if (matchedFileType == null)
                            {
                                // Corrected: Access Id directly
                                _logger.Error("Could not retrieve FileType details for matched template OCR ID {OcrId}, FileTypeId: {FileTypeId}. Skipping this template.",
                                    matchedTemplate.Id, matchedFileTypeId);
                                continue; // Try the next matched template
                            }
                            _logger.Debug("Using FileType from matched template: '{Desc}' (ID: {Id})", matchedFileType.Description, matchedFileType.Id);

                            // 3b. Get DocSets based on the *matched* FileType
                            var docSet = WaterNut.DataSpace.Utils.GetDocSets(matchedFileType);
                            if (docSet == null || !docSet.Any())
                            {
                                _logger.Warning("Could not retrieve DocSet for matched FileType ID: {FileTypeId}. Proceeding without DocSet.", matchedFileType.Id);
                                // Decide if this is critical - for now, allow proceeding
                            }
                            else
                            {
                                _logger.Debug("Retrieved {DocSetCount} DocSets for matched FileType ID: {FileTypeId}", docSet.Count, matchedFileType.Id);
                            }

                            // 3c. Create Context for the pipeline
                            context = new InvoiceProcessingContext
                            {
                                FilePath = file,
                                FileTypeId = matchedFileTypeId, // Use ID from matched template
                                EmailId = emailId, // Use initially resolved EmailId
                                OverWriteExisting = true, // Default, adjust if needed
                                DocSet = docSet,
                                FileType = matchedFileType, // Use matched FileType object
                                Client = Utils.Client, // Assuming Utils.Client is accessible
                                PdfText = pdfTxt, // Pass extracted text
                                FormattedPdfText = string.Empty, // Pipeline might populate this
                                Imports = new Dictionary<string, (string file, string, ImportStatus Success)>(),
                                // IMPORTANT: Pass the matched OCR template if the pipeline needs its rules directly
                                // This assumes InvoiceProcessingContext has a property like 'OcrTemplate'
                                Template = new WaterNut.DataSpace.Invoice(matchedTemplate) // Instantiate WaterNut.DataSpace.Invoice with the OCR entity
                            };
                            // Corrected: Access Name directly
                            _logger.Debug("Created InvoiceProcessingContext for matched template '{TemplateName}'.", matchedTemplate.Name);

                            // 3d. Run the pipeline
                            // Corrected: Access Name directly
                            _logger.Information("Starting invoice processing pipeline with matched template '{TemplateName}'", matchedTemplate.Name);
                            var pipe = new InvoiceProcessingPipeline(context, false); // Assuming 'false' is correct for isLastTemplate
                            var pipeResult = await pipe.RunPipeline().ConfigureAwait(false); // Pipeline handles its own logging
                            // Corrected: Access Name directly
                            _logger.Information("Pipeline completed for template '{TemplateName}'. Import Count: {ImportCount}, Final Status: {Status}",
                                 matchedTemplate.Name, context.Imports.Count, context.ImportStatus); // Assuming context tracks final status

                            // 3e. Process results from this template attempt
                            if (context.Imports.Any(imp => imp.Value.Success == ImportStatus.Success))
                            {
                                // Corrected: Access Name directly
                                _logger.Information("Pipeline reported successful imports for template '{TemplateName}'.", matchedTemplate.Name);
                                importSucceededForFile = true;
                                foreach (var import in context.Imports)
                                {
                                    // Use a more robust unique key
                                    // Corrected: Access Name directly
                                    string uniqueKey = $"{fileInfo.Name}-{matchedTemplate.Name}-{import.Key}";
                                    // Pipeline context Imports tuple is (string file, string, ImportStatus Success) - access by position
                                    // Target allImports tuple is (string file, string DocumentType, ImportStatus Status) - use names
                                    string docTypeFromPipeline = import.Value.Item2; // Get DocumentType string from pipeline result tuple
                                    ImportStatus statusFromPipeline = import.Value.Item3; // Get Status from pipeline result tuple

                                    _logger.Debug("Recording import result - Key: {Key}, File: {File}, Status: {Status}, DocType: {DocType}",
                                        uniqueKey, import.Value.Item1, statusFromPipeline, docTypeFromPipeline);

                                    // Create the named tuple for allImports list
                                    var resultTuple = (file: file, DocumentType: docTypeFromPipeline ?? matchedFileType.Description, Status: statusFromPipeline);
                                    allImports.Add(new KeyValuePair<string, (string file, string DocumentType, ImportStatus Status)>(uniqueKey, resultTuple));
                                }
                                // Optional: Break after first successful template match?
                                // Corrected: Access Name directly
                                _logger.Information("Stopping further template processing for {FileName} as a successful import was achieved with template '{TemplateName}'.", fileInfo.Name, matchedTemplate.Name);
                                break; // Stop trying other templates for this file
                            }
                            else
                            {
                                // Corrected: Access Name directly
                                _logger.Warning("Pipeline did not report successful imports for template '{TemplateName}'. Trying next matched template if available.", matchedTemplate.Name);
                            }
                        }
                        catch (Exception ex)
                        {
                            // Corrected: Access properties directly
                            _logger.Error(ex, "Error processing file {FileName} with template '{TemplateName}' (OCR ID: {TemplateId}).",
                                fileInfo.Name, matchedTemplate?.Name ?? "N/A", matchedTemplate?.Id ?? 0);
                            // Log context state if available
                            if (context != null)
                            {
                                // _logger.Debug("Context state at time of error: {@Context}", context); // Disabled detailed entity logging
                            }
                            // Continue to the next template
                        }
                    } // End foreach matchedTemplate

                    if (!importSucceededForFile)
                    {
                         _logger.Warning("No template resulted in a successful import for file: {FileName}", fileInfo.Name);
                         // Corrected Add call
                         var failureTuple = (file: file, DocumentType: initialFileType.Description, Status: ImportStatus.Failed);
                         allImports.Add(new KeyValuePair<string, (string, string, ImportStatus)>($"{file}-NoSuccessfulTemplate", failureTuple));
                    }

                }
                catch (Exception e)
                {
                    _logger.Error(e, "Unhandled error during import process for file {FileName}", fileInfo.Name);
                    // Corrected Add call
                    var failureTuple = (file: file, DocumentType: initialFileType.Description, Status: ImportStatus.Failed);
                    allImports.Add(new KeyValuePair<string, (string, string, ImportStatus)>($"{file}-UnhandledException", failureTuple));
                    // Potentially send email notification like old code if required
                }
            } // End foreach pdfFile

            // Corrected lambda to access tuple element by name 'Status'
            _logger.Information("PDF import process completed. Total successful/partial imports recorded: {SuccessCount}", allImports.Count(kvp => kvp.Value.Status != ImportStatus.Failed));
            return allImports;
        }


        // --- Helper methods adapted from old InvoiceReader ---

        // Gets Templates from OCR DB
        private static List<LocalInvoiceTemplate> GetTemplates(Expression<Func<OCRInvoice, bool>> filter)
        {
            _logger.Debug("Executing GetTemplates with filter.");
            List<LocalInvoiceTemplate> templates;
            try
            {
                using (var ctx = new OCRContext()) // Ensure OCRContext is available
                {
                    // Load templates with all necessary related data (copied from old code)
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
                        .Where(x => x.ApplicationSettingsId == BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId) // Ensure BaseDataModel is accessible
                        .Where(filter)
                        .ToList();
                        // Removed redundant and incorrect .Select statement. The result of ToList() is already List<OCRInvoice> (aliased as List<LocalInvoiceTemplate>)
                }
                _logger.Debug("GetTemplates retrieved {Count} templates from database.", templates.Count);
            }
            catch (Exception ex)
            {
                 _logger.Error(ex, "Error retrieving templates from OCR database.");
                 templates = new List<LocalInvoiceTemplate>(); // Return empty list on error
            }
            return templates;
        }

        // Filters templates based on content matching
        private static List<LocalInvoiceTemplate> GetPossibleInvoices(List<LocalInvoiceTemplate> templates, StringBuilder pdfTxt)
        {
             _logger.Debug("Filtering {TotalTemplates} templates based on PDF content.", templates.Count);
             var pdfContent = pdfTxt.ToString();
             var matchedTemplates = templates
                        // Corrected: Pass the template directly to IsInvoiceDocument
                        .Where(tmp => IsInvoiceDocument(tmp, pdfContent))
                        // Corrected: Access properties directly
                        .OrderBy(x => !x.Name.ToUpper().Contains("Tropical".ToUpper())) // Example ordering from old code
                        .ThenBy(x => x.Id)
                        .ToList();
             _logger.Debug("Found {MatchedCount} possible templates after content matching.", matchedTemplates.Count);
             // Corrected: Access properties directly
             matchedTemplates.ForEach(t => _logger.Verbose("- Matched Template: {Name} (ID: {Id})", t.Name, t.Id));
             return matchedTemplates;
         }

        // Checks if identification regex matches text (from old code)
        private static bool IsInvoiceDocument(OCRInvoice invoice, string fileText)
        {
            if (invoice == null || !invoice.InvoiceIdentificatonRegEx.Any())
            {
                //_logger.Verbose("IsInvoiceDocument check skipped for template ID {Id}: No identification regex found.", invoice?.Id ?? 0);
                return false; // Cannot identify if no regex defined
            }

            bool isMatch = invoice.InvoiceIdentificatonRegEx.Any(x =>
                {
                    if (x.OCR_RegularExpressions == null || string.IsNullOrEmpty(x.OCR_RegularExpressions.RegEx))
                    {
                        _logger.Warning("Skipping null or empty identification regex for Invoice ID {InvoiceId}, Regex ID {RegexId}", invoice.Id, x.Id);
                        return false;
                    }
                    try
                    {
                        var match = Regex.IsMatch(fileText, x.OCR_RegularExpressions.RegEx, RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture);
                        //_logger.Verbose("Regex check for Template ID {TemplateId}, Regex ID {RegexId} ('{RegexPattern}'): {Result}", invoice.Id, x.Id, x.OCR_RegularExpressions.RegEx, match);
                        return match;
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "Error executing identification regex '{RegexPattern}' for Template ID {TemplateId}, Regex ID {RegexId}", x.OCR_RegularExpressions.RegEx, invoice.Id, x.Id);
                        return false;
                    }
                });

            _logger.Verbose("IsInvoiceDocument result for Template ID {TemplateId}: {IsMatch}", invoice.Id, isMatch);
            return isMatch;
        }


        // Extracts text using PdfPig and Tesseract OCR (adapted from old code)
        private static async Task<StringBuilder> GetPdftxt(string file)
        {
            _logger.Debug("Starting text extraction for file: {File}", file);
            StringBuilder pdftxt = new StringBuilder();
            var tasks = new List<Task<string>>();

            // Task 1: PdfPig Content Extraction
            tasks.Add(Task.Run(() => {
                try {
                    _logger.Verbose("Starting PdfPig text extraction for {File}", file);
                    var txt = "------------------------------------------Ripped Text (PdfPig)-------------------------\r\n";
                    txt += PdfPigText(file);
                    _logger.Verbose("PdfPig extraction finished for {File}. Length: {Length}", file, txt.Length);
                    return txt;
                } catch (Exception ex) {
                    _logger.Error(ex, "Error during PdfPig text extraction for {File}", file);
                    return "Error during PdfPig extraction.";
                }
            }));

            // Task 2: Tesseract OCR - Single Column
            tasks.Add(Task.Run(() => {
                 try {
                    _logger.Verbose("Starting Tesseract OCR (SingleColumn) for {File}", file);
                    var txt = "------------------------------------------Single Column (Tesseract)-------------------------\r\n";
                    // Ensure PdfOcr() can be instantiated and path to tesseract data is correct
                    txt += new PdfOcr().Ocr(file, PageSegMode.SingleColumn);
                    _logger.Verbose("Tesseract OCR (SingleColumn) finished for {File}. Length: {Length}", file, txt.Length);
                    return txt;
                 } catch (Exception ex) {
                    _logger.Error(ex, "Error during Tesseract OCR (SingleColumn) for {File}", file);
                    return "Error during Tesseract SingleColumn OCR.";
                 }
            }));

            // Task 3: Tesseract OCR - Sparse Text
            tasks.Add(Task.Run(() => {
                 try {
                    _logger.Verbose("Starting Tesseract OCR (SparseText) for {File}", file);
                    var txt = "------------------------------------------SparseText (Tesseract)-------------------------\r\n";
                    txt += new PdfOcr().Ocr(file, PageSegMode.SparseText);
                     _logger.Verbose("Tesseract OCR (SparseText) finished for {File}. Length: {Length}", file, txt.Length);
                    return txt;
                 } catch (Exception ex) {
                    _logger.Error(ex, "Error during Tesseract OCR (SparseText) for {File}", file);
                    return "Error during Tesseract SparseText OCR.";
                 }
            }));

            try
            {
                await Task.WhenAll(tasks).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                 _logger.Error(ex, "Error occurred while waiting for text extraction tasks for file {File}", file);
                 // Decide if we should return partial results or null
            }

            // Append results
            tasks.ForEach(task => {
                // Corrected Task Status Check
                if (task.Status == TaskStatus.RanToCompletion) {
                    pdftxt.AppendLine(task.Result);
                } else if (task.IsFaulted) {
                    pdftxt.AppendLine($"Task failed: {task.Exception?.Flatten().InnerExceptions.FirstOrDefault()?.Message ?? "Unknown error"}");
                } else {
                     pdftxt.AppendLine("A text extraction task did not complete successfully.");
                }
            });


            _logger.Debug("Finished text extraction for file: {File}. Total Length: {Length}", file, pdftxt.Length);
            return pdftxt;
        }

        // Helper for PdfPig extraction (from old code)
        private static string PdfPigText(string file)
        {
            StringBuilder sb = new StringBuilder();
            try
            {
                using (var pdf = PdfDocument.Open(file))
                {
                    foreach (var page in pdf.GetPages())
                    {
                        // Using ContentOrderTextExtractor as in the old code
                        var text = ContentOrderTextExtractor.GetText(page);
                        sb.AppendLine(text);
                        _logger.Verbose("Extracted text from page {PageNumber} using PdfPig.", page.Number);
                    }
                }
                 _logger.Debug("PdfPigText extraction successful for {File}", file);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "PdfPigText extraction failed for file {File}", file);
                sb.AppendLine("Error reading Ripped Text (PdfPig)"); // Provide error indication
            }
            return sb.ToString();
        }

        // Helper to write extracted text to a file for debugging (from old code)
        private static string WriteTextFile(string pdfFilePath, string textContent)
        {
            string txtFilePath = pdfFilePath + ".txt";
            try
            {
                File.WriteAllText(txtFilePath, textContent);
                _logger.Verbose("Successfully wrote extracted text to debug file: {TxtFile}", txtFilePath);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to write extracted text to debug file: {TxtFile}", txtFilePath);
                return null; // Indicate failure
            }
            return txtFilePath;
        }

        // NOTE: Error reporting methods (ReportUnImportedFile, SaveImportError etc.) from the old code
        // have not been included in this refactoring pass to focus on the core import logic.
        // They can be added back if detailed error reporting and test case generation are required.

    }
}
