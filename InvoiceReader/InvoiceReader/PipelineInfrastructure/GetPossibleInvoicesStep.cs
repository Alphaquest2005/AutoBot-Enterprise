using OCR.Business.Entities;
using System.Text.RegularExpressions; // Added using directive
using System.Collections.Generic; // Added
using System.Linq; // Added
using System.Threading.Tasks; // Added
using Serilog; // Added
using System;
using InvoiceReader.PipelineInfrastructure;
using WaterNut.Business.Services.Utils; // Added

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    using System.Diagnostics;

    public partial class GetPossibleInvoicesStep : IPipelineStep<InvoiceProcessingContext>
    {
        // Remove static logger
        // private static readonly ILogger _logger = Log.ForContext<GetPossibleInvoicesStep>();
        private static readonly TimeSpan RegexTimeout = TimeSpan.FromSeconds(5);

        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            var methodStopwatch = Stopwatch.StartNew(); // Start stopwatch for method execution
            string filePath = context?.FilePath ?? "Unknown";
            context.Logger?.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
                nameof(Execute), "Identify possible invoice templates based on PDF text", $"FilePath: {filePath}");

            context.Logger?.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]",
                nameof(GetPossibleInvoicesStep), $"Identifying possible invoices for file: {filePath}");

            if (!ValidateContext(context, filePath))
            {
                methodStopwatch.Stop();
                context.Logger?.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(Execute), "Identify possible invoice templates based on PDF text", methodStopwatch.ElapsedMilliseconds, "Context validation failed.");
                context.Logger?.Error("ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                    nameof(GetPossibleInvoicesStep), "Context validation", methodStopwatch.ElapsedMilliseconds, "Context validation failed.");
                return false;
            }

            try
            {
                string pdfTextString = context.PdfText.ToString();
                int totalTemplateCount = context.Templates?.Count() ?? 0;
                context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                    nameof(Execute), "Processing", "Processing templates to find possible invoices.", $"TotalTemplateCount: {totalTemplateCount}, FilePath: {filePath}");

                var possibleInvoices = await GetPossibleInvoices(context, pdfTextString, filePath).ConfigureAwait(false);

                //if (possibleInvoices.All(x => FileTypeManager.GetFileType(x.OcrInvoices.FileTypeId).FileImporterInfos.EntryType != "Shipment Template"))
                //    throw new ApplicationException("No Shipment Template Templates found");

                context.Templates = possibleInvoices;
                LogPossibleInvoices(context, possibleInvoices, totalTemplateCount, filePath);

                methodStopwatch.Stop();
                context.Logger?.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                    nameof(Execute), "Successfully identified possible invoice templates", $"PossibleInvoiceCount: {context.Templates?.Count() ?? 0}", methodStopwatch.ElapsedMilliseconds);
                context.Logger?.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                    nameof(GetPossibleInvoicesStep), $"Successfully identified {context.Templates?.Count() ?? 0} possible invoices for file: {filePath}", methodStopwatch.ElapsedMilliseconds);
                return true;
            }
            catch (Exception ex)
            {
                methodStopwatch.Stop();
                string errorMessage = $"Error during GetPossibleInvoicesStep processing templates for File: {filePath}: {ex.Message}";
                context.Logger?.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(Execute), "Identify possible invoice templates based on PDF text", methodStopwatch.ElapsedMilliseconds, errorMessage);
                context.Logger?.Error(ex, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                    nameof(GetPossibleInvoicesStep), "Processing templates", methodStopwatch.ElapsedMilliseconds, errorMessage);
                context.AddError(errorMessage);
                context.Templates = new List<Invoice>();
                return false;
            }
        }

        private bool ValidateContext(InvoiceProcessingContext context, string filePath)
        {
            if (context == null)
            {
                // Cannot use context.Logger if context is null
                Log.ForContext<GetPossibleInvoicesStep>().Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(ValidateContext), "Validate pipeline context", 0, "GetPossibleInvoicesStep executed with null context.");
                return false;
            }

            if (context.Templates == null)
            {
                context.Logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}] Expected templates for processing.",
                    nameof(ValidateContext), "Validation", "Skipping GetPossibleInvoicesStep: Templates collection is null.", $"FilePath: {filePath}");
                context.Templates = new List<Invoice>();
                return true; // Treat as successful validation but no work done
            }

            if (context.PdfText == null)
            {
                context.Logger?.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(ValidateContext), "Validate pipeline context", 0, $"Skipping GetPossibleInvoicesStep: PdfText (StringBuilder) is null for File: {filePath}.");
                context.Logger?.Error("ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                    nameof(GetPossibleInvoicesStep), "Context validation", 0, $"Skipping GetPossibleInvoicesStep: PdfText (StringBuilder) is null for File: {filePath}.");
                context.AddError($"PdfText is null for file: {filePath}");
                return false; // Indicate validation failure
            }

            context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                nameof(ValidateContext), "Validation", "Context validation successful.", $"FilePath: {filePath}");
            return true;
        }

        private async Task<List<Invoice>> GetPossibleInvoices(InvoiceProcessingContext context, string pdfTextString, string filePath)
        {
            context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                nameof(GetPossibleInvoices), "Filtering", "Ordering templates and filtering based on PDF text match.", $"FilePath: {filePath}");

            var possibleInvoices = context.Templates
                .OrderBy(x => !(x?.OcrInvoices?.Name?.ToUpperInvariant().Contains("TROPICAL") ?? false))
                .ThenBy(x => x?.OcrInvoices?.Id ?? int.MaxValue)
                .Where(tmp =>
                {
                    if (tmp == null)
                    {
                        context.Logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                            nameof(GetPossibleInvoices), "Filtering", "Skipping null template during filtering.", "");
                        return false;
                    }

                    if (tmp.OcrInvoices == null)
                    {
                        context.Logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                            nameof(GetPossibleInvoices), "Filtering", "Skipping template with null OcrInvoices.", $"TemplateId: {tmp.OcrInvoices.Id}");
                        return false;
                    }

                    context.Logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                        nameof(GetPossibleInvoices), "Filtering", "Checking template match.", $"TemplateId: {tmp.OcrInvoices.Id}, TemplateName: {tmp.OcrInvoices.Name}");

                    // Call the partial method IsInvoiceDocument
                    context.Logger?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                        $"IsInvoiceDocument for Template {tmp.OcrInvoices.Id}", "SYNC_EXPECTED");
                    var isMatchStopwatch = Stopwatch.StartNew();
                    bool isMatch = IsInvoiceDocument(tmp, pdfTextString, filePath, context.Logger);
                    isMatchStopwatch.Stop();
                    context.Logger?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                        $"IsInvoiceDocument for Template {tmp.OcrInvoices.Id}", isMatchStopwatch.ElapsedMilliseconds, "Sync call returned");

                    context.Logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                        nameof(GetPossibleInvoices), "Filtering", "Template match result.", $"TemplateId: {tmp.OcrInvoices.Id}, IsMatch: {isMatch}");
                    return isMatch;
                })
                // .Select(x =>
                // {
                //     x.CsvLines = null;
                //     x.FileType = context.FileType;
                //     x.DocSet = context.DocSet ?? WaterNut.DataSpace.Utils.GetDocSets(context.FileType);
                //     x.FilePath = context.FilePath;
                //     x.EmailId = context.EmailId;
                //     foreach (var part in x.Parts)
                //     {
                //         part.AllLines.ForEach(z => z.Values.Clear());
                //     }

                //     return x;
                // })
                .ToList()
                ;

            context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                nameof(GetPossibleInvoices), "Filtering", "Finished filtering templates.", $"PossibleInvoiceCount: {possibleInvoices.Count}, FilePath: {filePath}");

            // need to get fresh templates
            var lst = possibleInvoices.Select(x => x.OcrInvoices.Id).ToList();
            if (!lst.Any())
            {
                 context.Logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}] Expected at least one possible invoice.",
                     nameof(GetPossibleInvoices), "TemplateRefresh", "No possible invoices found, skipping template refresh.", $"FilePath: {filePath}");
                 return new List<Invoice>(); // Return empty list if no possible invoices
            }

            context.Logger?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                "GetTemplatesStep.GetTemplates (Refresh)", "ASYNC_EXPECTED");
            var getTemplatesStopwatch = Stopwatch.StartNew();
            var res = await GetTemplatesStep.GetTemplates(context, invoices => lst.Contains(invoices.Id)).ConfigureAwait(false);
            getTemplatesStopwatch.Stop();
            context.Logger?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                "GetTemplatesStep.GetTemplates (Refresh)", getTemplatesStopwatch.ElapsedMilliseconds, "If ASYNC_EXPECTED, this is pre-await return");

            context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                nameof(GetPossibleInvoices), "TemplateRefresh", "Refreshed possible invoice templates.", $"RefreshedCount: {res.Count}, FilePath: {filePath}");

            return res;
        }

        private void LogPossibleInvoices(InvoiceProcessingContext context, List<Invoice> possibleInvoices, int totalTemplateCount, string filePath)
        {
            context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                nameof(GetPossibleInvoicesStep), "Summary", "Possible invoices found.", $"PossibleInvoiceCount: {possibleInvoices.Count}, TotalTemplateCount: {totalTemplateCount}, FilePath: {filePath}");

            if (possibleInvoices.Any())
            {
                var invoiceDetails = possibleInvoices.Select(inv => new { Name = inv.OcrInvoices?.Name, Id = inv.OcrInvoices?.Id }).ToList();
                context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}] {OptionalData}",
                    nameof(GetPossibleInvoicesStep), "Summary", "Details of possible invoices.", $"FilePath: {filePath}", new { InvoiceDetails = invoiceDetails });
            }
            else
            {
                context.Logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}] Expected at least one possible invoice.",
                    nameof(GetPossibleInvoicesStep), "Summary", "No possible invoices found.", $"FilePath: {filePath}");
            }
        }
    }
 }