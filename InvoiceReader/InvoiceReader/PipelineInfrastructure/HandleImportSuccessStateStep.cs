using CoreEntities.Business.Entities; // Assuming FileTypes is here
using WaterNut.Business.Services.Utils; // Assuming FileTypeManager and DataFileProcessor are here
using WaterNut.Business.Services; // Added for DataFile
using System.Threading.Tasks; // Added
using Serilog; // Added
using System; // Added
using System.Linq; // Added for Any() checks if needed
using System.Collections.Generic; // Added for List checks if needed

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    using System.Diagnostics;

    public class HandleImportSuccessStateStep : IPipelineStep<InvoiceProcessingContext>
    {
        // Add a static logger instance for this class
        // Remove static logger
        // private static readonly ILogger _logger = Log.ForContext<HandleImportSuccessStateStep>();

        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            var methodStopwatch = Stopwatch.StartNew(); // Start stopwatch for method execution
            string filePath = context?.FilePath ?? "Unknown";
            context.Logger?.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
                nameof(Execute), "Handle successful import state and process data file", $"FilePath: {filePath}");

            context.Logger?.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]",
                nameof(HandleImportSuccessStateStep), $"Handling import success state for file: {filePath}");

             // Basic context validation
            if (context == null)
            {
                // Cannot use context.Logger if context is null
                Log.ForContext<HandleImportSuccessStateStep>().Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(Execute), "Handle successful import state and process data file", 0, "HandleImportSuccessStateStep executed with null context.");
                Log.ForContext<HandleImportSuccessStateStep>().Error("ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                    nameof(HandleImportSuccessStateStep), "Context validation", 0, "HandleImportSuccessStateStep executed with null context.");
                return false;
            }
             if (!context.MatchedTemplates.Any())
            {
                 context.Logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                     nameof(Execute), "Validation", "Skipping HandleImportSuccessStateStep: No Templates found in context.", $"FilePath: {filePath}", "Expected templates for processing.");
                 methodStopwatch.Stop(); // Stop stopwatch on skip
                 context.Logger?.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                     nameof(Execute), "Skipped due to no templates", $"FilePath: {filePath}", methodStopwatch.ElapsedMilliseconds);
                 context.Logger?.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                     nameof(HandleImportSuccessStateStep), $"Skipped handling import success state for file: {filePath} (no templates)", methodStopwatch.ElapsedMilliseconds);
                 return true; // No templates to process, not a failure.
            }

            bool overallStepSuccess = true; // Track success across all templates

            // Iterate over MatchedTemplates instead of all Templates
            foreach (var template in context.MatchedTemplates)
            {
                 int? templateId = template?.OcrInvoices?.Id; // Safe access
                 string templateName = template?.OcrInvoices?.Name ?? "Unknown";
                 context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                     nameof(Execute), "TemplateProcessing", "Processing matched template.", $"FilePath: {filePath}, TemplateId: {templateId}, TemplateName: '{templateName}'", "");
                 context.Logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                     nameof(Execute), "TemplateProcessing", "Context details at start of matched template processing.", $"FilePath: {filePath}, TemplateId: {templateId}", new { Context = context });

                 try // Wrap processing for each template
                 {
                     // --- Validation for this template ---
                     if (IsRequiredDataMissing(context.Logger, template)) // Handles its own logging, pass logger
                     {
                         string errorMsg = $"HandleImportSuccessStateStep cannot proceed due to missing required data for File: {filePath}, TemplateId: {templateId}";
                         // Logging is handled by helper
                         context.AddError(errorMsg); // Add error to context
                         overallStepSuccess = false; // Mark step as failed
                         continue; // Continue to the next template
                     }
                     // --- End Validation ---

                     // --- Resolve File Type ---
                     context.Logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                         nameof(Execute), "FileTypeResolution", "Resolving file type.", $"FilePath: {filePath}, TemplateId: {templateId}", "");
                     FileTypes fileType = ResolveFileType(context.Logger, template); // Handles its own logging, pass logger
                     if (fileType == null)
                     {
                         string errorMsg = $"ResolveFileType returned null for File: {filePath}, TemplateId: {templateId}. Cannot proceed.";
                         context.Logger?.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                             nameof(Execute), "Resolve file type", 0, errorMsg);
                         context.Logger?.Error("ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                             $"{nameof(HandleImportSuccessStateStep)} - Template {templateId}", "File type resolution", 0, errorMsg);
                         context.AddError(errorMsg); // Add error to context
                         overallStepSuccess = false;
                         continue;
                     }
                     context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                         nameof(Execute), "FileTypeResolution", "Resolved FileType.", $"FileTypeId: {fileType.Id}, FilePath: {filePath}", "");
                     // --- End Resolve File Type ---

                     // --- Create DataFile ---
                     context.Logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                         nameof(Execute), "DataFileCreation", "Creating DataFile object.", $"FilePath: {filePath}", "");
                     DataFile dataFile = CreateDataFile(context.Logger, template, fileType); // Handles its own logging, pass logger
                     if (dataFile == null)
                     {
                         string errorMsg = $"CreateDataFile returned null for File: {filePath}, TemplateId: {templateId}. Cannot proceed.";
                         context.Logger?.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                             nameof(Execute), "Create DataFile object", 0, errorMsg);
                         context.Logger?.Error("ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                             $"{nameof(HandleImportSuccessStateStep)} - Template {templateId}", "DataFile creation", 0, errorMsg);
                         context.AddError(errorMsg); // Add error to context
                         overallStepSuccess = false;
                         continue;
                     }
                     context.Logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                         nameof(Execute), "DataFileCreation", "Created DataFile details.", $"FilePath: {filePath}", new { DataFile = dataFile });
                     // --- End Create DataFile ---

                     // --- Process DataFile ---
                     context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                         nameof(Execute), "DataFileProcessing", "Starting DataFileProcessor.", $"FilePath: {filePath}, FileTypeId: {fileType.Id}", "");
                     var processor = new DataFileProcessor();
                     bool processResult = false; // Default to false
                     try
                     {
                         context.Logger?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                              $"DataFileProcessor.Process for File: {filePath}", "ASYNC_EXPECTED"); // Log before processor call
                         var processorStopwatch = Stopwatch.StartNew(); // Start stopwatch
                         // Run potentially blocking code in background thread
context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                             nameof(Execute), "DataFileProcessing", "Calling DataFileProcessor.Process.", $"FilePath: {filePath}, FileTypeId: {fileType.Id}", new { DataFileDetails = new { FileType = dataFile?.FileType?.Description, DocSetCount = dataFile?.DocSet?.Count() } });
                         processResult = await Task.Run(() => processor.Process(dataFile)).ConfigureAwait(false);
                         processorStopwatch.Stop(); // Stop stopwatch
                         context.Logger?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                             $"DataFileProcessor.Process for File: {filePath}", processorStopwatch.ElapsedMilliseconds, "If ASYNC_EXPECTED, this is pre-await return"); // Log after processor call
                     }
                     catch(Exception procEx) // Catch exceptions specifically from the processor
                     {
                          string errorMsg = $"DataFileProcessor threw an exception for File: {filePath}, TemplateId: {templateId}";
                          context.Logger?.Error(procEx, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                              nameof(Execute), "Process DataFile", 0, errorMsg);
                          context.Logger?.Error(procEx, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                              $"{nameof(HandleImportSuccessStateStep)} - Template {templateId}", "DataFile processing", 0, errorMsg);
                          context.AddError($"{errorMsg}: {procEx.Message}");
                          processResult = false; // Ensure failure
                     }
                     context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                         nameof(Execute), "DataFileProcessing", "DataFileProcessor finished.", $"FilePath: {filePath}, Success: {processResult}", "");

                     if (!LogImportProcessingOutcome(context.Logger, processResult, filePath)) // Checks result and logs, pass logger
                     {
                          // LogImportProcessingOutcome logs the error, add context error here
                          string errorMsg = $"DataFileProcessor failed for File: {filePath}, TemplateId: {templateId}.";
                          context.AddError(errorMsg);
                          overallStepSuccess = false;
                          continue;
                     }
                     // --- End Process DataFile ---

                     context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                         nameof(Execute), "TemplateCompletion", "Finished processing template successfully.", $"FilePath: {filePath}, TemplateId: {templateId}", "");
                 }
                 catch (Exception ex) // Catch unexpected errors during processing for this template
                 {
                     string errorMsg = $"Unexpected error during HandleImportSuccessStateStep for File: {filePath}, TemplateId: {templateId}: {ex.Message}";
                     context.Logger?.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                         nameof(Execute), "Handle successful import state and process data file", 0, errorMsg);
                     context.Logger?.Error(ex, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                         $"{nameof(HandleImportSuccessStateStep)} - Template {templateId}", "Unexpected error during template processing", 0, errorMsg);
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
                     nameof(Execute), "Successfully handled import success state for all applicable templates", $"OverallSuccess: true", methodStopwatch.ElapsedMilliseconds);
                 context.Logger?.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                     nameof(HandleImportSuccessStateStep), $"Successfully handled import success state for file: {filePath} for all applicable templates", methodStopwatch.ElapsedMilliseconds);
            }
            else
            {
                 // This case is hit if any template processing resulted in an error.
                 // The specific failure reason is logged within the loop.
                 context.Logger?.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                     nameof(Execute), "Handle successful import state and process data file", methodStopwatch.ElapsedMilliseconds, "Handling import success state failed for at least one template.");
                 context.Logger?.Error("ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                     nameof(HandleImportSuccessStateStep), "Processing templates", methodStopwatch.ElapsedMilliseconds, "Handling import success state failed for at least one template.");
            }

            return overallStepSuccess;
        }

        private static bool IsRequiredDataMissing(ILogger logger, Invoice Invoice) // Add logger parameter
        {
             logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                 nameof(IsRequiredDataMissing), "Validation", "Checking for missing required data in Template for success state.", "", "");
             // Check each property and log which one is missing if any
             // Context null check happens in Execute
             if (Invoice?.FileType == null) { logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}", nameof(IsRequiredDataMissing), "Validation", "Missing required data: FileType is null.", "", ""); return true; }

             // DEBUG: Add detailed logging for DocSet
             if (Invoice?.DocSet == null) {
                 logger?.Error("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                     nameof(IsRequiredDataMissing), "Validation", "CRITICAL: DocSet is null - this should have been populated by GetDocSets call!", $"TemplateId: {Invoice?.OcrInvoices?.Id}, FileType: {Invoice?.FileType?.Id}", "");
                 return true; // This is a real error - DocSet should not be null
             }

             logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                 nameof(IsRequiredDataMissing), "Validation", "DocSet is present.", $"DocSetCount: {Invoice.DocSet.Count}, TemplateId: {Invoice?.OcrInvoices?.Id}", "");

             // Check Template.OcrInvoices as it's used in ResolveFileType
             if (Invoice?.OcrInvoices == null) { logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}", nameof(IsRequiredDataMissing), "Validation", "Missing required data: Template.OcrInvoices is null.", "", ""); return true; }
             if (Invoice?.CsvLines == null) { logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}", nameof(IsRequiredDataMissing), "Validation", "Missing required data: CsvLines is null.", "", ""); return true; }
             if (string.IsNullOrEmpty(Invoice?.FilePath)) { logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}", nameof(IsRequiredDataMissing), "Validation", "Missing required data: FilePath is null or empty.", "", ""); return true; }
             if (string.IsNullOrEmpty(Invoice?.EmailId)) { logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}", nameof(IsRequiredDataMissing), "Validation", "Missing required data: EmailId is null or empty.", "", ""); return true; }

             logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                 nameof(IsRequiredDataMissing), "Validation", "All required data is present.", "", "");
             return false; // All required data is present
        }

        private static FileTypes ResolveFileType(ILogger logger, Invoice template) // Add logger parameter
        {
             logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                 nameof(ResolveFileType), "Resolution", "Entering ResolveFileType.", "", "");
             // Context and Template null checks happen in caller
             int? templateId = template?.OcrInvoices?.Id; // Null check done by caller
             int originalFileTypeId = template.FileType.Id; // Null check done by caller
             int templateFileTypeId = template.OcrInvoices.FileTypeId; // Null check done by caller

             logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                 nameof(ResolveFileType), "Resolution", "Resolving FileType.", $"OriginalContextFileTypeId: {originalFileTypeId}, TemplateFileTypeId: {templateFileTypeId}", "");

             FileTypes fileType = template.FileType; // Start with the template's file type

             // Check if the template specifies a different FileTypeId
             if (fileType.Id != templateFileTypeId)
             {
                  logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                      nameof(ResolveFileType), "Resolution", "Template FileTypeId differs from original. Attempting to get FileType from FileTypeManager.", $"TemplateFileTypeId: {templateFileTypeId}, OriginalFileTypeId: {originalFileTypeId}", "");
                  try
                  {
                      // Assuming FileTypeManager has a static Instance property or similar access method
                      // Corrected: Call static GetFileType method directly
                      logger?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                          $"FileTypeManager.GetFileType for FileTypeId: {templateFileTypeId}", "SYNC_EXPECTED"); // Log before GetFileType call
                      var getFileTypeStopwatch = Stopwatch.StartNew(); // Start stopwatch
                      fileType = FileTypeManager.GetFileType(templateFileTypeId);
                      getFileTypeStopwatch.Stop(); // Stop stopwatch
                      logger?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                          $"FileTypeManager.GetFileType for FileTypeId: {templateFileTypeId}", getFileTypeStopwatch.ElapsedMilliseconds, "Sync call returned"); // Log after GetFileType call

                      if (fileType == null)
                      {
                           logger?.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                               nameof(ResolveFileType), "Get FileType from manager", 0, $"FileTypeManager.GetFileType returned null for FileTypeId: {templateFileTypeId}. Using original template FileType.");
                           fileType = template.FileType; // Fallback to original if manager fails
                      } else {
                           // Removed .Name as it doesn't exist on FileTypes
                           logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                               nameof(ResolveFileType), "Resolution", "Successfully retrieved FileType from FileTypeManager.", $"FileTypeId: {fileType.Id}", "");
                      }
                  }
                  catch (Exception ex)
                  {
                       logger?.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                           nameof(ResolveFileType), "Get FileType from manager", 0, $"Error calling FileTypeManager.GetFileType for FileTypeId: {templateFileTypeId}. Using original template FileType. Error: {ex.Message}");
                       fileType = template.FileType; // Fallback on error
                  }
              } else {
                   logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                       nameof(ResolveFileType), "Resolution", "Using original FileType from template.", $"FileTypeId: {originalFileTypeId}", "");
              }
              logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                  nameof(ResolveFileType), "Resolution", "Exiting ResolveFileType.", $"ResolvedFileTypeId: {fileType?.Id}", "");
              return fileType;
         }

         // Added filePath for context
         private static bool LogImportProcessingOutcome(ILogger logger, bool processResult, string filePath) // Add logger parameter
         {
              logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                  nameof(LogImportProcessingOutcome), "OutcomeLogging", "Entering LogImportProcessingOutcome.", $"FilePath: {filePath}, ProcessResult: {processResult}", "");
              // Replace Console.WriteLine
              if (processResult)
              {
                   logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                       nameof(LogImportProcessingOutcome), "OutcomeLogging", "DataFileProcessor completed successfully.", $"FilePath: {filePath}", "");
              }
              else
              {
                   logger?.Error("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                       nameof(LogImportProcessingOutcome), "OutcomeLogging", "DataFileProcessor failed.", $"FilePath: {filePath}", "");
              }
              logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                  nameof(LogImportProcessingOutcome), "OutcomeLogging", "Exiting LogImportProcessingOutcome.", $"FilePath: {filePath}, Returning: {processResult}", "");
             return processResult; // Indicate success based on the result of DataFileProcessor().Process
         }

         private static DataFile CreateDataFile(ILogger logger, Invoice template, FileTypes fileType) // Add logger parameter
         {
              string filePath = template?.FilePath ?? "Unknown";
              logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                  nameof(CreateDataFile), "Creation", "Entering CreateDataFile.", $"FilePath: {filePath}, FileTypeId: {fileType?.Id ?? -1}", "");
              // Null checks for template properties used here happen in IsRequiredDataMissing
              DataFile dataFile = null;
              try
              {
                  // DEBUG: Add detailed logging for DocSet state
                  logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                      nameof(CreateDataFile), "Creation", "DocSet validation.", $"DocSetIsNull: {template?.DocSet == null}, DocSetCount: {template?.DocSet?.Count ?? 0}, TemplateId: {template?.OcrInvoices?.Id}", "");

                  // Ensure DocSet is not null before accessing its properties if needed by DataFile constructor
                  if (template?.DocSet == null || !template.DocSet.Any()) {
                      logger?.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                          nameof(CreateDataFile), "Create DataFile object", 0, $"Cannot create DataFile: DocSet is null or empty for File: {filePath}");
                      return null;
                  }

                  // Package OCR template with line values for OCR correction
                  var lineValues = template.Lines?.Where(line => line?.Values != null)
                      .SelectMany(line => line.Values)
                      .ToDictionary(kvp => kvp.Key, kvp => kvp.Value) ??
                      new Dictionary<(int lineNumber, string section), Dictionary<(OCR.Business.Entities.Fields Fields, string Instance), string>>();

                  dataFile = new DataFile(fileType, template.DocSet, template.OverWriteExisting,
                                                    template.EmailId,
                                                    template.FilePath, template.CsvLines, (template.OcrInvoices, lineValues));
                   logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                       nameof(CreateDataFile), "Creation", "DataFile object created successfully.", $"FilePath: {filePath}", "");
              }
              catch (Exception ex)
              {
                   logger?.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                       nameof(CreateDataFile), "Create DataFile object", 0, $"Error creating DataFile object for File: {filePath}. Error: {ex.Message}");
                   dataFile = null;
              }
              logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                  nameof(CreateDataFile), "Creation", "Exiting CreateDataFile.", $"FilePath: {filePath}, DataFileIsNull: {dataFile == null}", "");
              return dataFile;
         }
     }
 }