using System;
using WaterNut.Business.Services.Utils;

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    using System.Diagnostics;

    using Serilog;

    public partial class UpdateImportStatusStep
    {
        private static ImportStatus ProcessImportFile(ILogger logger, InvoiceProcessingContext context) // Add logger parameter
        {
            logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(ProcessImportFile), "Processing", "Entering ProcessImportFile.", $"FilePath: {context?.FilePath ?? "Unknown"}", "");
            // Required data checks happen in caller (IsImportDataPresent)
            string filePath = context.FilePath;
            ImportStatus finalStatus = ImportStatus.Failed; // Default to Failed

            foreach (var template in context.MatchedTemplates)
            {
                int templateId = template.OcrTemplates.Id; // Already checked not null
                string templateName = template.OcrTemplates.Name ?? "UnknownName";
                string fileDescription = "Unknown Description"; // Default

                logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(ProcessImportFile), "TemplateProcessing", "Processing import file status for template.", $"FilePath: {filePath}, TemplateId: {templateId}, TemplateName: '{templateName}'", "");

                try
                {
                    // Safely get FileType description
                    int fileTypeId = template.OcrTemplates.FileTypeId;
                    logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                        nameof(ProcessImportFile), "FileTypeDescription", "Attempting to get FileType description.", $"FileTypeId: {fileTypeId}", "");
                    
                    logger?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                        $"FileTypeManager.GetFileType for FileTypeId: {fileTypeId}", "SYNC_EXPECTED"); // Log before GetFileType call
                    var getFileTypeStopwatch = Stopwatch.StartNew(); // Start stopwatch
                    // Corrected: Use static GetFileType method
                    var fileType = FileTypeManager.GetFileType(fileTypeId);
                    getFileTypeStopwatch.Stop(); // Stop stopwatch
                    logger?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                        $"FileTypeManager.GetFileType for FileTypeId: {fileTypeId}", getFileTypeStopwatch.ElapsedMilliseconds, "Sync call returned"); // Log after GetFileType call

                    if (fileType != null)
                    {
                        // Corrected: Use Description property if it exists, otherwise log ID
                        fileDescription = fileType.Description ?? $"FileTypeID_{fileTypeId}";
                        logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                            nameof(ProcessImportFile), "FileTypeDescription", "Retrieved FileType Description.", $"FileDescription: '{fileDescription}', FileTypeId: {fileTypeId}", "");
                    }
                    else
                    {
                        logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                            nameof(ProcessImportFile), "FileTypeDescription", "FileTypeManager returned null. Using default description.", $"FileTypeId: {fileTypeId}", "");
                    }
                }
                catch (Exception ex)
                {
                    logger?.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                        nameof(ProcessImportFile), "Get FileType description", 0, $"Error getting FileType description from FileTypeManager for FileTypeId: {template.OcrTemplates.FileTypeId}. Error: {ex.Message}");
                    // Continue with default description
                }


                // Assuming ImportStatus is available in the context based on previous steps
                // If not set, it defaults to 0 (likely 'Failed')
                ImportStatus importStatus = template.ImportStatus;
                logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(ProcessImportFile), "StatusRetrieval", "Current ImportStatus from context.", $"ImportStatus: {importStatus}, FilePath: {filePath}, TemplateId: {templateId}", "");

                string importKey = $"{filePath}-{templateName}-{templateId}";
                logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(ProcessImportFile), "KeyGeneration", "Generated Imports dictionary key.", $"ImportKey: {importKey}", "");

                try
                {
                    // Use TryAdd for safety, although key should be unique per template run
                    // If TryAdd fails, overwrite the existing entry.
                    context.Imports.Add(importKey, (filePath, fileDescription, importStatus));
                    if (!context.Imports.ContainsKey(importKey))
                    {
                        logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                            nameof(ProcessImportFile), "DictionaryUpdate", "Failed to add entry to Imports dictionary (Key already exists?). Overwriting existing entry.", $"Key: {importKey}, Status: {importStatus}", "");
                        // Overwrite the existing entry if TryAdd fails
                        context.Imports[importKey] = (filePath, fileDescription, importStatus);
                    }
                    else
                    {
                        logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                            nameof(ProcessImportFile), "DictionaryUpdate", "Added entry to Imports dictionary.", $"Key: {importKey}, Status: {importStatus}", "");
                    }
                }
                catch (Exception ex)
                {
                    logger?.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                        nameof(ProcessImportFile), "Add/update Imports dictionary", 0, $"Error adding/updating entry in Imports dictionary for Key: {importKey}. Error: {ex.Message}");
                    // Depending on requirements, might need to handle this failure more explicitly
                }

                // Update finalStatus based on the template's status
                finalStatus = importStatus;
            }

            logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(ProcessImportFile), "Completion", "Exiting ProcessImportFile.", $"FinalStatus: {finalStatus}", "");
            // The method returns the status that was processed and added to the dictionary
            return finalStatus;
        }
    }
}