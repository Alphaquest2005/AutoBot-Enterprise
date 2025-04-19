using System;
using System.Collections.Generic;
using System.Linq;
using CoreEntities.Business.Entities;
using OCR.Business.Entities;
using WaterNut.Business.Services.Utils;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;
using System.Threading.Tasks;
using System.Text;
using DocumentDS.Business.Entities;
using System.IO;
using WaterNut.DataSpace;

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public partial class ProcessInvoiceTemplateStep : IPipelineStep<InvoiceProcessingContext>
    {
        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            Console.WriteLine(
                $"[OCR DEBUG] Pipeline Step: Attempting template '{_template.OcrInvoices.Name}' (ID: {_template.OcrInvoices.Id}) for file '{context.FilePath}'");
            try
            {
                var formattedPdfTxt = _template.Format(context.PdfText.ToString()); // Assuming Format is accessible on _template
                Console.WriteLine(
                    $"[OCR DEBUG] Pipeline Step: PDF text formatted using template {_template.OcrInvoices.Id}.");

                var csvLines = _template.Read(formattedPdfTxt); // Assuming Read is accessible on _template
                Console.WriteLine(
                    $"[OCR DEBUG] Pipeline Step: _template.Read returned {csvLines?.Count ?? 0} data structures.");
                if (csvLines != null && csvLines.Count > 0 && csvLines[0] is List<IDictionary<string, object>> list)
                {
                    Console.WriteLine(
                        $"[OCR DEBUG] Pipeline Step: First data structure contains {list.Count} items.");
                }

                AddNameSupplier(_template, csvLines); // Now AddNameSupplier is in this class

                AddMissingRequiredFieldValues(_template, csvLines); // Now AddMissingRequiredFieldValues is in this class

                InvoiceProcessingUtils.WriteTextFile(context.FilePath, formattedPdfTxt); // Now WriteTextFile is in this class


                ImportStatus importStatus;
                if (csvLines == null || csvLines.Count < 1 || !_template.Success) // Assuming Success is accessible on _template
                {
                    Console.WriteLine(
                        $"[OCR DEBUG] Pipeline Step: Read failed or returned no lines. _template.Success = {_template.Success}. Entering ErrorState.");
                    var errorResult = ErrorState(context.FilePath, context.EmailId, formattedPdfTxt, context.Client, context.DocSet, _template, context.FileTypeId,
                        _isLastTemplate); // Now ErrorState is in this class
                    Console.WriteLine($"[OCR DEBUG] Pipeline Step: ErrorState returned {errorResult}.");
                    importStatus = errorResult ? ImportStatus.HasErrors : ImportStatus.Failed;
                }
                else
                {
                    Console.WriteLine(
                        $"[OCR DEBUG] Pipeline Step: Read successful. Entering ImportSuccessState.");
                    var successResult = ImportSuccessState(context.FilePath, context.EmailId, context.FileType, context.OverWriteExisting, context.DocSet, _template,
                        csvLines); // Now ImportSuccessState is in this class
                    Console.WriteLine(
                        $"[OCR DEBUG] Pipeline Step: ImportSuccessState returned {successResult}.");
                    importStatus = successResult == true ? ImportStatus.Success : ImportStatus.HasErrors;
                }

                var fileDescription = FileTypeManager.GetFileType(_template.OcrInvoices.FileTypeId).Description; // Assuming FileTypeManager is accessible
                switch (importStatus)
                {
                    case ImportStatus.Success:
                        context.Imports.Add($"{context.FilePath}-{_template.OcrInvoices.Name}-{_template.OcrInvoices.Id}",
                            (context.FilePath, FileTypeManager.EntryTypes.GetEntryType(fileDescription), ImportStatus.Success));
                        break;
                    case ImportStatus.HasErrors:
                        context.Imports.Add($"{context.FilePath}-{_template.OcrInvoices.Name}-{_template.OcrInvoices.Id}",
                            (context.FilePath, FileTypeManager.EntryTypes.GetEntryType(fileDescription),
                                ImportStatus.HasErrors));
                        break;
                    case ImportStatus.Failed:
                        // Assuming ReportUnImportedFile is an existing method in InvoiceReader
                        // and can be accessed or moved here.
                        // await ReportUnimportedFile(context.DocSet, context.FilePath, context.EmailId, context.FileTypeId, context.Client, context.PdfText.ToString(),
                        //     "No template found for this File", new List<Line>()); // Need access to template.Lines

                        context.Imports.Add($"{context.FilePath}-{_template.OcrInvoices.Name}-{_template.OcrInvoices.Id}",
                            (context.FilePath, FileTypeManager.EntryTypes.GetEntryType(fileDescription), ImportStatus.Failed));
                        break;
                }


                return importStatus == ImportStatus.Success || importStatus == ImportStatus.HasErrors; // Indicate success if not completely failed
            }
            catch (Exception e)
            {
                Exception realerror = e;
                while (realerror.InnerException != null)
                    realerror = realerror.InnerException;

                // Assuming ReportUnImportedFile is an existing method in InvoiceReader
                // and can be accessed or moved here.
                // await ReportUnimportedFile(context.DocSet, context.FilePath, context.EmailId, context.FileTypeId, context.Client, context.PdfText.ToString(),
                //     $"Problem importing file:{context.FilePath} --- {realerror.Message}", new List<Line>()); // Need access to template.Lines

                var ex = new ApplicationException($"Problem importing file:{context.FilePath} --- {realerror.Message}", e);
                Console.WriteLine(ex);
                return false; // Indicate failure
            }
        }
    }
}