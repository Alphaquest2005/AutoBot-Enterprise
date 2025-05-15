using System;
using System.IO;
using System.Threading.Tasks;
using CoreEntities.Business.Entities;
using Serilog;
using InvoiceReader.InvoiceReader.PipelineInfrastructure; // Assuming this is the correct namespace
using WaterNut.Business.Services.Importers; // Added for IImporter

namespace InvoiceReader.InvoiceReader.Importers
{
    using System.Linq;

    using WaterNut.DataSpace.PipelineInfrastructure;

    public class PDFImporter : IImporter // Assuming IImporter is accessible or needs to be moved/interfaced
    {
        private readonly FileTypes _fileType;
        private readonly ILogger _logger;

        public PDFImporter(FileTypes fileType, ILogger logger)
        {
            _fileType = fileType;
            _logger = logger;
        }

        public async Task Import(string fileName, bool overWrite, ILogger log)
        {
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();
            _logger.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
                nameof(this.Import), "Import PDF file", $"FileName: {fileName}, Overwrite: {overWrite}");

            try
            {
                // Create InvoiceProcessingContext and pass the logger and file info
                var context = new InvoiceProcessingContext()
                {
                    FilePath = fileName,
                    FileInfo = new FileInfo(fileName) // Assuming FileInfo is needed in the context
                };

                // Call the GetPdfTextStep to orchestrate text extraction
                _logger.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "GetPdfTextStep.Execute", "ASYNC_EXPECTED");
                var extractionStepStopwatch = System.Diagnostics.Stopwatch.StartNew();
                bool textExtractionSuccess = await new GetPdfTextStep().Execute(context).ConfigureAwait(false);
                extractionStepStopwatch.Stop();
                 _logger.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    "GetPdfTextStep.Execute", extractionStepStopwatch.ElapsedMilliseconds, "Async call completed (await).");


                if (textExtractionSuccess)
                {
                    _logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                        nameof(this.Import), "TextExtractionComplete", "PDF text extraction completed successfully.", $"FileName: {fileName}, ExtractedTextLength: {context.PdfText?.Length ?? 0}");

                    // Call the GetPossibleInvoicesStep to identify possible invoices
                    _logger.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})", "GetPossibleInvoicesStep.Execute", "ASYNC_EXPECTED");
                    var possibleInvoicesStepStopwatch = System.Diagnostics.Stopwatch.StartNew();
                    bool possibleInvoicesSuccess = await new InvoiceReader.InvoiceReader.PipelineInfrastructure.GetPossibleInvoicesStep().Execute(context).ConfigureAwait(false);
                    possibleInvoicesStepStopwatch.Stop();
                    _logger.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                       "GetPossibleInvoicesStep.Execute", possibleInvoicesStepStopwatch.ElapsedMilliseconds, "Async call completed (await).");


                    if (possibleInvoicesSuccess)
                    {
                        _logger.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                            nameof(this.Import), "PossibleInvoicesIdentified", "Possible invoice templates identified.", $"FileName: {fileName}, PossibleInvoiceCount: {context.Templates?.Count() ?? 0}");

                        // TODO: Add logic for subsequent pipeline steps (e.g., parsing, saving)

                        stopwatch.Stop();
                        _logger.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                            nameof(this.Import), "PDF import process initiated, text extracted, and possible invoices identified.", $"FileName: {fileName}, PossibleInvoiceCount: {context.Templates?.Count() ?? 0}", stopwatch.ElapsedMilliseconds);
                    }
                    else
                    {
                        // Get errors from context if the step failed
                        var errors = string.Join("; ", context.Errors);
                         _logger.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                            nameof(this.Import), "PossibleInvoicesFailed", "GetPossibleInvoicesStep failed to identify possible invoices.", $"FileName: {fileName}, Errors: {errors}");

                        stopwatch.Stop();
                        _logger.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                            nameof(this.Import), "Import PDF file", stopwatch.ElapsedMilliseconds, $"Identifying possible invoices failed for file: {fileName}. Errors: {errors}");
                        // Depending on requirements, you might want to throw an exception here or return a specific result indicating failure.
                    }
                }
                else
                {
                    // Get errors from context if the text extraction step failed
                    var errors = string.Join("; ", context.Errors);
                     _logger.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                        nameof(this.Import), "TextExtractionFailed", "GetPdfTextStep failed to extract text.", $"FileName: {fileName}, Errors: {errors}");

                    stopwatch.Stop();
                    _logger.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                        nameof(this.Import), "Import PDF file", stopwatch.ElapsedMilliseconds, $"Text extraction failed for file: {fileName}. Errors: {errors}");
                    // Depending on requirements, you might want to throw an exception here or return a specific result indicating failure.
                }
            }
            catch (Exception ex)
            {
                stopwatch.Stop();
                _logger.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(this.Import), "Import PDF file", stopwatch.ElapsedMilliseconds, ex.Message);
                throw; // Re-throw the exception after logging
            }
        }
    }
}