using System;
// Assuming Template is defined here

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog; // Add Serilog using statement

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    using System.Diagnostics;

    public class AddNameSupplierStep : IPipelineStep<InvoiceProcessingContext>
    {
        // Add a static logger instance for this class
        // Remove static logger
        // private static readonly ILogger _logger = Log.ForContext<AddNameSupplierStep>();

        public Task<bool> Execute(InvoiceProcessingContext context)
        {
            var methodStopwatch = Stopwatch.StartNew(); // Start stopwatch for method execution
            string filePath = context?.FilePath ?? "Unknown";
            context.Logger?.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
                nameof(Execute), "Add Name and SupplierCode to extracted data if missing", $"FilePath: {filePath}");

            context.Logger?.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]",
                nameof(AddNameSupplierStep), $"Adding Name/SupplierCode for file: {filePath}");

             // Basic context validation
            if (context == null)
            {
                // Cannot use context.Logger if context is null - throw exception instead
                throw new ArgumentNullException(nameof(context), "AddNameSupplierStep executed with null context.");
            }
             if (!context.MatchedTemplates.Any())
            {
                 context.Logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                     nameof(Execute), "Validation", "Skipping AddNameSupplierStep: No Templates found in context.", $"FilePath: {filePath}", "Expected templates with extracted data.");
                 methodStopwatch.Stop(); // Stop stopwatch on skip
                 context.Logger?.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                     nameof(Execute), "Skipped due to no templates", $"FilePath: {filePath}", methodStopwatch.ElapsedMilliseconds);
                 context.Logger?.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                     nameof(AddNameSupplierStep), $"Skipped adding Name/SupplierCode for file: {filePath} (no templates)", methodStopwatch.ElapsedMilliseconds);
                 return Task.FromResult(true); // No templates to process, not a failure.
            }

            bool anyTemplateProcessedSuccessfully = false; // Track if at least one template was processed without critical error

            foreach (var template in context.MatchedTemplates)
            {
                 int? templateId = template?.OcrTemplates?.Id; // Safe access
                 string templateName = template?.OcrTemplates?.Name ?? "Unknown";
                 context.Logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                     nameof(Execute), "TemplateProcessing", "Processing template.", $"FilePath: {filePath}, TemplateId: {templateId}, TemplateName: '{templateName}'", "");

                 try // Wrap processing for each template
                 {
                     // --- Validation for this template ---
                     if (template == null || template.CsvLines == null || !template.CsvLines.Any())
                     {
                         string errorMsg = $"Skipping AddNameSupplierStep for TemplateId: {templateId} due to missing Template or CsvLines for File: {filePath}.";
                         context.Logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                             nameof(Execute), "TemplateValidation", "Skipping template due to missing Template or CsvLines.", $"TemplateId: {templateId}, FilePath: {filePath}", "Expected template with extracted CsvLines.");
                         context.AddError(errorMsg);
                         continue; // Continue to the next template
                     }
                     // --- End Validation ---

                     // --- Core Logic ---
                     context.Logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                         nameof(Execute), "ConditionCheck", "Checking condition to add Name/SupplierCode.", $"FilePath: {filePath}, TemplateId: {templateId}", "");
                     bool conditionMet = template.CsvLines.Count == 1 &&
                                         template.Lines != null &&
                                         template.OcrTemplates != null &&
                                         !template.Lines.All(x =>
                                             x?.OCR_Lines != null &&
                                             "Name, SupplierCode".Contains(x.OCR_Lines.Name));

                     if (conditionMet)
                     {
                         context.Logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                             nameof(Execute), "ConditionMet", "Condition met to add Name/SupplierCode for single CSV line set.", $"FilePath: {filePath}, TemplateId: {templateId}", "");
                         
                         // Check format and process
                         if (template.CsvLines.First() is List<IDictionary<string, object>> firstDocList)
                         {
                             context.Logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                                 nameof(Execute), "FormatCheck", "CsvLines format is expected (List<IDictionary<string, object>>).", $"FilePath: {filePath}, TemplateId: {templateId}", "");
                             foreach (var doc in firstDocList)
                             {
                                 if (doc == null)
                                 {
                                      context.Logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                                          nameof(Execute), "DocumentProcessing", "Encountered a null document within CsvLines list, skipping Name/SupplierCode addition for this doc.", $"FilePath: {filePath}, TemplateId: {templateId}", "");
                                      continue; // Skip this specific null document
                                 }

                                 // Add SupplierCode if missing
                                 if (!doc.ContainsKey("SupplierCode"))
                                 {
                                     context.Logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                                         nameof(Execute), "AddingSupplierCode", "Adding SupplierCode to document.", $"SupplierCode: '{template.OcrTemplates.Name}', FilePath: {filePath}, TemplateId: {templateId}", "");
                                     doc.Add("SupplierCode", template.OcrTemplates.Name);
                                 }
                                 
                                 // Add Name if missing
                                 if (!doc.ContainsKey("Name"))
                                 {
                                     context.Logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                                         nameof(Execute), "AddingName", "Adding Name to document.", $"Name: '{template.OcrTemplates.Name}', FilePath: {filePath}, TemplateId: {templateId}", "");
                                     doc.Add("Name", template.OcrTemplates.Name);
                                 }
                             }
                              context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                                  nameof(Execute), "ProcessingComplete", "Added Name and/or Supplier information where missing.", $"FilePath: {filePath}, TemplateId: {templateId}", "");
                         }
                         else
                         {
                              // Log warning if format is wrong, but don't fail the whole step unless required.
                              context.Logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                                  nameof(Execute), "FormatCheck", "CsvLines is not in the expected format (List<IDictionary<string, object>>). Cannot add Name/SupplierCode.", $"FilePath: {filePath}, TemplateId: {templateId}", "Expected List<IDictionary<string, object>> format.");
                              // Decide if this is critical. For now, let the step succeed but log the issue.
                         }
                     }
                     else
                     {
                         context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                             nameof(Execute), "ConditionNotMet", "Condition not met or skipped adding Name/SupplierCode.", $"FilePath: {filePath}, TemplateId: {templateId}", "");
                     }
                     // --- End Core Logic ---

                     context.Logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                         nameof(Execute), "TemplateCompletion", "Finished processing template.", $"FilePath: {filePath}, TemplateId: {templateId}", "");
                     anyTemplateProcessedSuccessfully = true; // Mark that at least one template was processed
                 }
                 catch (Exception ex) // Catch unexpected errors during processing for this template
                 {
                     string errorMsg = $"Error during AddNameSupplierStep for File: {filePath}, TemplateId: {templateId}: {ex.Message}";
                     context.Logger?.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                         nameof(Execute), "Add Name and SupplierCode to extracted data", 0, errorMsg);
                     context.Logger?.Error(ex, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                         $"{nameof(AddNameSupplierStep)} - Template {templateId}", "Processing template", 0, errorMsg);
                     context.AddError(errorMsg); // Add error to context
                     // Do not set overallSuccess to false here, as other templates might succeed.
                     continue; // Continue to the next template
                 }
            }

            // If the loop completes, check if any template was processed successfully
            methodStopwatch.Stop(); // Stop stopwatch
            if (anyTemplateProcessedSuccessfully)
            {
                context.Logger?.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                    nameof(Execute), "Successfully added Name/SupplierCode for at least one template", $"OverallSuccess: true", methodStopwatch.ElapsedMilliseconds);
                context.Logger?.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                    nameof(AddNameSupplierStep), $"Successfully added Name/SupplierCode for file: {filePath} using at least one template", methodStopwatch.ElapsedMilliseconds);
            }
            else
            {
                // This case is hit if the initial template validation failed or all templates in the loop failed.
                // The specific failure reason is logged within the loop or initial validation.
                context.Logger?.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(Execute), "Add Name and SupplierCode to extracted data", methodStopwatch.ElapsedMilliseconds, "Adding Name/SupplierCode failed for all applicable templates.");
                context.Logger?.Error("ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                    nameof(AddNameSupplierStep), "Processing templates", methodStopwatch.ElapsedMilliseconds, "Adding Name/SupplierCode failed for all applicable templates.");
            }

            return Task.FromResult(anyTemplateProcessedSuccessfully); // Return true if at least one template succeeded
        }
    }
}