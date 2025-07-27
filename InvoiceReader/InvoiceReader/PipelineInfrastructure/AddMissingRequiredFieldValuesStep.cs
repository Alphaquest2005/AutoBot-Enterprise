using System;
// Assuming Template and related entities are here
// Assuming OCR_Lines and Fields are here

using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Serilog; // Add Serilog using statement
using OCR.Business.Entities; // Assuming Fields is in this namespace

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    using System.Diagnostics;

    public class AddMissingRequiredFieldValuesStep : IPipelineStep<InvoiceProcessingContext>
    {
        // Add a static logger instance for this class
        // Remove static logger
        // private static readonly ILogger _logger = Log.ForContext<AddMissingRequiredFieldValuesStep>();

        public Task<bool> Execute(InvoiceProcessingContext context)
        {
            var methodStopwatch = Stopwatch.StartNew(); // Start stopwatch for method execution
            string filePath = context?.FilePath ?? "Unknown";
            context.Logger?.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
                nameof(Execute), "Add missing required field values to extracted data", $"FilePath: {filePath}");

            context.Logger?.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]",
                nameof(AddMissingRequiredFieldValuesStep), $"Adding missing required field values for file: {filePath}");

             // Basic context validation
            if (context == null)
            {
                // Cannot use context.Logger if context is null - throw exception instead
                throw new ArgumentNullException(nameof(context), "AddMissingRequiredFieldValuesStep executed with null context.");
            }
             if ( !context.MatchedTemplates.Any())
            {
                 context.Logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                     nameof(Execute), "Validation", "Skipping AddMissingRequiredFieldValuesStep: No Templates found in context.", $"FilePath: {filePath}", "Expected templates with extracted data.");
                 methodStopwatch.Stop(); // Stop stopwatch on skip
                 context.Logger?.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                     nameof(Execute), "Skipped due to no templates", $"FilePath: {filePath}", methodStopwatch.ElapsedMilliseconds);
                 context.Logger?.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                     nameof(AddMissingRequiredFieldValuesStep), $"Skipped adding missing required field values for file: {filePath} (no templates)", methodStopwatch.ElapsedMilliseconds);
                 return Task.FromResult(true); // No templates to process, not a failure.
            }

            bool overallStepSuccess = true; // Track success across all templates

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
                         string warningMsg = $"Skipping AddMissingRequiredFieldValuesStep for TemplateId: {templateId} due to missing Template or CsvLines for File: {filePath}.";
                         context.Logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                             nameof(Execute), "TemplateValidation", "Skipping template due to missing Template or CsvLines.", $"TemplateId: {templateId}, FilePath: {filePath}", "Expected template with extracted CsvLines.");
                         // Don't add error or return false here, just skip this template.
                         // If *no* templates have CsvLines, the step effectively does nothing but succeeds.
                         continue; // Move to the next template
                     }
                     // --- End Validation ---

                     // --- Core Logic ---
                     context.Logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                         nameof(Execute), "CoreLogic", "Starting core logic for adding missing required field values.", $"FilePath: {filePath}, TemplateId: {templateId}", "");

                     var requiredFieldsList = GetRequiredFieldsWithValues(context, template); // Pass logger
                     context.Logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                         nameof(Execute), "CoreLogic", "Found required fields with default values.", $"RequiredFieldCount: {requiredFieldsList.Count}, FilePath: {filePath}, TemplateId: {templateId}.", "");

                     if (requiredFieldsList.Any()) // Only proceed if there are fields to add
                     {
                         AddRequiredFieldValuesToDocuments(context.Logger, template, requiredFieldsList); // Pass logger
                         context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                             nameof(Execute), "CoreLogic", "Attempted to add missing required field values.", $"FilePath: {filePath}, TemplateId: {templateId}.", "");
                     }
                     else
                     {
                          context.Logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                              nameof(Execute), "CoreLogic", "No required fields with default values found to add.", $"FilePath: {filePath}, TemplateId: {templateId}.", "");
                     }
                     // --- End Core Logic ---

                     context.Logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                         nameof(Execute), "TemplateCompletion", "Finished processing template.", $"FilePath: {filePath}, TemplateId: {templateId}", "");
                 }
                 catch (Exception ex) // Catch unexpected errors during processing for this template
                 {
                     string errorMsg = $"Error during AddMissingRequiredFieldValuesStep for File: {filePath}, TemplateId: {templateId}: {ex.Message}";
                     context.Logger?.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                         nameof(Execute), "Add missing required field values to extracted data", 0, errorMsg);
                     context.Logger?.Error(ex, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                         $"{nameof(AddMissingRequiredFieldValuesStep)} - Template {templateId}", "Processing template", 0, errorMsg);
                     context.AddError(errorMsg); // Add error to context
                     overallStepSuccess = false; // Mark the overall step as failed
                     continue; // Continue to next template
                 }
            }

            // Log final status based on whether all templates were processed without error
            methodStopwatch.Stop(); // Stop stopwatch
            if (overallStepSuccess)
            {
                 context.Logger?.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                     nameof(Execute), "Successfully added missing required field values for all applicable templates", $"OverallSuccess: true", methodStopwatch.ElapsedMilliseconds);
                 context.Logger?.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                     nameof(AddMissingRequiredFieldValuesStep), $"Successfully added missing required field values for file: {filePath} for all applicable templates", methodStopwatch.ElapsedMilliseconds);
            }
            else
            {
                 // This case is hit if an unexpected exception occurred for at least one template.
                 // The specific failure reason is logged within the loop.
                 context.Logger?.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                     nameof(Execute), "Add missing required field values to extracted data", methodStopwatch.ElapsedMilliseconds, "Adding missing required field values failed for at least one template.");
                 context.Logger?.Error("ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                     nameof(AddMissingRequiredFieldValuesStep), "Processing templates", methodStopwatch.ElapsedMilliseconds, "Adding missing required field values failed for at least one template.");
            }
            
            return Task.FromResult(overallStepSuccess);
        }



        private void AddRequiredFieldValuesToDocuments(ILogger logger, Template template, // Add logger parameter
            List<Fields> requiredFieldsList)
        {
             // Use FilePath as the identifier
             logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                 nameof(AddRequiredFieldValuesToDocuments), "Processing", "Starting to add required field values to documents.", $"FilePath: {template?.FilePath ?? "Unknown"}", "");

            // Assuming CsvLines is List<object> where the first object is List<IDictionary<string, object>>
            // Need null checks for safety
            // Added check for template null
            if (template == null || template.CsvLines == null || !template.CsvLines.Any() || !(template.CsvLines.First() is List<IDictionary<string, object>> firstDocList))
            {
                 int? templateId = template?.OcrTemplates?.Id; // Safe access
                 // Use FilePath as the identifier
                logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(AddRequiredFieldValuesToDocuments), "Validation", "CsvLines is null, empty, or not in the expected format.", $"TemplateId:{templateId}, File: {template?.FilePath ?? "Unknown"}", "Expected List<IDictionary<string, object>> format.");
                return; // Exit this method for this template
            }

             // Use FilePath as the identifier
             logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                 nameof(AddRequiredFieldValuesToDocuments), "Processing", "Processing documents.", $"DocumentCount: {firstDocList.Count}, FilePath: {template?.FilePath ?? "Unknown"}", "");

            foreach (var field in requiredFieldsList)
            {
                 // Use FilePath as the identifier
                 logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                     nameof(AddRequiredFieldValuesToDocuments), "FieldProcessing", "Processing required field.", $"FieldName: {field?.Field ?? "NULL_FIELD"}, FilePath: {template?.FilePath ?? "Unknown"}", ""); // Use Verbose for potentially high-volume logs
                 if (field == null) continue; // Skip null fields

                foreach (var doc in firstDocList)
                {
                    // Ensure doc is not null before processing
                    if (doc != null)
                    {
                        AddMissingFieldToDocument(logger, doc, field); // Pass logger
                    }
                    else
                    {
                         // Use FilePath as the identifier
                        logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                            nameof(AddRequiredFieldValuesToDocuments), "DocumentProcessing", "Encountered a null document while processing field, skipping.", $"FieldName: {field.Field}, FilePath: {template?.FilePath ?? "Unknown"}", "");
                    }
                }
            }
             // Use FilePath as the identifier
             logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                 nameof(AddRequiredFieldValuesToDocuments), "Completion", "Finished adding required field values to documents.", $"FilePath: {template?.FilePath ?? "Unknown"}", "");
        }

       private List<Fields> GetRequiredFieldsWithValues(InvoiceProcessingContext context, Template template) // Add logger parameter
       {
            // Use FilePath as the identifier
           context.Logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
               nameof(GetRequiredFieldsWithValues), "Processing", "Getting required fields with values.", $"FilePath: {context.FilePath}", "");
           // Added null check for safety
           if (template?.Lines == null)
           {
                // Use FilePath as the identifier
                context.Logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                   nameof(GetRequiredFieldsWithValues), "Validation", "Template or Template.Lines is null.", $"FilePath: {context.FilePath}", "Returning empty list.");
               return new List<Fields>();
           }

           var fields = template.Lines
               .Where(line => line?.OCR_Lines?.Fields != null) // Ensure line and fields are not null
               .SelectMany(x => x.OCR_Lines.Fields)
               .Where(z => z != null && z.IsRequired && z.FieldValue != null && !string.IsNullOrEmpty(z.FieldValue.Value)) // Ensure field and FieldValue are not null
               .ToList();
            // Use FilePath as the identifier
           context.Logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
               nameof(GetRequiredFieldsWithValues), "Completion", "Found required fields with values.", $"FieldCount: {fields.Count}, FilePath: {context.FilePath}", "");
           return fields;
       }

       private void AddMissingFieldToDocument(ILogger logger, IDictionary<string, object> doc, Fields field) // Add logger parameter
       {
           // Added null checks for safety - doc null check happens in caller
           if (field == null || string.IsNullOrEmpty(field.Field) || field.FieldValue == null)
           {
                logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(AddMissingFieldToDocument), "Validation", "Skipping due to null field or missing FieldName/FieldValue.", "", "");
                return;
           }

           if (!doc.ContainsKey(field.Field)) // Use ContainsKey for dictionaries
           {
                logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(AddMissingFieldToDocument), "AddingField", "Adding missing field with value to document.", $"FieldName: '{field.Field}', FieldValue: '{field.FieldValue.Value}'", "");
               doc.Add(field.Field, field.FieldValue.Value);
           }
           else
           {
                 logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                     nameof(AddMissingFieldToDocument), "FieldExists", "Field already exists in document, skipping.", $"FieldName: '{field.Field}'", "");
           }
       }
   }
}
