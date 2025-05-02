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
    public class HandleImportSuccessStateStep : IPipelineStep<InvoiceProcessingContext>
    {
        // Add a static logger instance for this class
        private static readonly ILogger _logger = Log.ForContext<HandleImportSuccessStateStep>();

        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
             // Basic context validation
            if (context == null)
            {
                _logger.Error("HandleImportSuccessStateStep executed with null context.");
                return false;
            }
             if (!context.Templates.Any())
            {
                 _logger.Warning("Skipping HandleImportSuccessStateStep: No Templates found in context for File: {FilePath}", context.FilePath ?? "Unknown");
                 return true; // No templates to process, not a failure.
            }

            string filePath = context.FilePath ?? "Unknown";
            bool overallStepSuccess = true; // Track success across all templates

            foreach (var template in context.Templates)
            {
                 int? templateId = template?.OcrInvoices?.Id; // Safe access
                 _logger.Information("Executing HandleImportSuccessStateStep for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
                 _logger.Verbose("Context details at start of HandleImportSuccessStateStep: {@Context}", context);

                 try // Wrap processing for each template
                 {
                     // --- Validation for this template ---
                     if (IsRequiredDataMissing(template)) // Handles its own logging
                     {
                         string errorMsg = $"HandleImportSuccessStateStep cannot proceed due to missing required data for File: {filePath}, TemplateId: {templateId}";
                         _logger.Warning(errorMsg); // Logged by helper, but log again for step context
                         context.AddError(errorMsg); // Add error to context
                         overallStepSuccess = false; // Mark step as failed
                         break; // Stop processing other templates
                     }
                     // --- End Validation ---

                     // --- Resolve File Type ---
                     _logger.Debug("Resolving file type for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
                     FileTypes fileType = ResolveFileType(template); // Handles its own logging
                     if (fileType == null)
                     {
                         string errorMsg = $"ResolveFileType returned null for File: {filePath}, TemplateId: {templateId}. Cannot proceed.";
                         _logger.Error(errorMsg);
                         context.AddError(errorMsg); // Add error to context
                         overallStepSuccess = false;
                         break;
                     }
                     _logger.Information("Resolved FileType to Id: {FileTypeId} for File: {FilePath}", fileType.Id, filePath);
                     // --- End Resolve File Type ---

                     // --- Create DataFile ---
                     _logger.Debug("Creating DataFile object for File: {FilePath}", filePath);
                     DataFile dataFile = CreateDataFile(template, fileType); // Handles its own logging
                     if (dataFile == null)
                     {
                         string errorMsg = $"CreateDataFile returned null for File: {filePath}, TemplateId: {templateId}. Cannot proceed.";
                         _logger.Error(errorMsg);
                         context.AddError(errorMsg); // Add error to context
                         overallStepSuccess = false;
                         break;
                     }
                     _logger.Verbose("Created DataFile details: {@DataFile}", dataFile);
                     // --- End Create DataFile ---

                     // --- Process DataFile ---
                     _logger.Information("Starting DataFileProcessor for File: {FilePath}, FileTypeId: {FileTypeId}", filePath, fileType.Id);
                     var processor = new DataFileProcessor();
                     bool processResult = false; // Default to false
                     try
                     {
                         // Run potentially blocking code in background thread
                         processResult = await Task.Run(() => processor.Process(dataFile)).ConfigureAwait(false);
                     }
                     catch(Exception procEx) // Catch exceptions specifically from the processor
                     {
                          string errorMsg = $"DataFileProcessor threw an exception for File: {filePath}, TemplateId: {templateId}";
                          _logger.Error(procEx, errorMsg);
                          context.AddError($"{errorMsg}: {procEx.Message}");
                          processResult = false; // Ensure failure
                     }
                     _logger.Information("DataFileProcessor finished for File: {FilePath}. Success: {ProcessResult}", filePath, processResult);
                     
                     if (!LogImportProcessingOutcome(processResult, filePath)) // Checks result and logs
                     {
                          // LogImportProcessingOutcome logs the error, add context error here
                          string errorMsg = $"DataFileProcessor failed for File: {filePath}, TemplateId: {templateId}.";
                          context.AddError(errorMsg);
                          overallStepSuccess = false;
                          break;
                     }
                     // --- End Process DataFile ---

                     _logger.Information("Finished processing HandleImportSuccessStateStep successfully for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
                 }
                 catch (Exception ex) // Catch unexpected errors during processing for this template
                 {
                     string errorMsg = $"Unexpected error during HandleImportSuccessStateStep for File: {filePath}, TemplateId: {templateId}: {ex.Message}";
                     _logger.Error(ex, errorMsg); // Log the error with exception details
                     context.AddError(errorMsg); // Add error to context
                     overallStepSuccess = false; // Mark the overall step as failed
                     break; // Stop processing immediately on error
                 }
            }

            // Log final status based on whether all templates were processed without error
            if (overallStepSuccess)
            {
                 _logger.Information("HandleImportSuccessStateStep completed successfully for all applicable templates in File: {FilePath}.", filePath);
            }
            else
            {
                 _logger.Error("HandleImportSuccessStateStep failed for at least one template in File: {FilePath}. See previous errors.", filePath);
            }
            
            return overallStepSuccess;
        }

        private static bool IsRequiredDataMissing(Invoice Invoice)
        {
             _logger.Debug("Entering IsRequiredDataMissing check.");
             _logger.Verbose("Checking for missing required data in Invoice for success state.");
             // Check each property and log which one is missing if any
             // Context null check happens in Execute
             if (Invoice.FileType == null) { _logger.Warning("Missing required data for success state: FileType is null."); return true; }
             if (Invoice.DocSet == null) { _logger.Warning("Missing required data for success state: DocSet is null."); return true; }
             
             // Check Template.OcrInvoices as it's used in ResolveFileType
             if (Invoice.OcrInvoices == null) { _logger.Warning("Missing required data for success state: Template.OcrInvoices is null."); return true; }
             if (Invoice.CsvLines == null) { _logger.Warning("Missing required data for success state: CsvLines is null."); return true; }
             if (string.IsNullOrEmpty(Invoice.FilePath)) { _logger.Warning("Missing required data for success state: FilePath is null or empty."); return true; }
             if (string.IsNullOrEmpty(Invoice.EmailId)) { _logger.Warning("Missing required data for success state: EmailId is null or empty."); return true; }

             _logger.Debug("Exiting IsRequiredDataMissing check. Result: False (No missing data).");
             return false; // All required data is present
        }

        private static FileTypes ResolveFileType(Invoice template)
        {
             _logger.Debug("Entering ResolveFileType.");
             // Context and Template null checks happen in caller
             int? templateId = template.OcrInvoices?.Id; // Null check done by caller
             int originalFileTypeId = template.FileType.Id; // Null check done by caller
             int templateFileTypeId = template.OcrInvoices.FileTypeId; // Null check done by caller

             _logger.Debug("Resolving FileType. Original Context FileTypeId: {OriginalFileTypeId}, Template FileTypeId: {TemplateFileTypeId}", originalFileTypeId, templateFileTypeId);

             FileTypes fileType = template.FileType; // Start with the template's file type

             // Check if the template specifies a different FileTypeId
             if (fileType.Id != templateFileTypeId)
             {
                  _logger.Information("Template FileTypeId ({TemplateFileTypeId}) differs from template FileTypeId ({OriginalFileTypeId}). Attempting to get FileType from FileTypeManager.",
                     templateFileTypeId, originalFileTypeId);
                  try
                  {
                      // Assuming FileTypeManager has a static Instance property or similar access method
                      // Corrected: Call static GetFileType method directly
                      fileType = FileTypeManager.GetFileType(templateFileTypeId);
                      if (fileType == null)
                      {
                           _logger.Error("FileTypeManager.GetFileType returned null for FileTypeId: {FileTypeId}. Using original template FileType.", templateFileTypeId);
                           fileType = template.FileType; // Fallback to original if manager fails
                      } else {
                           // Removed .Name as it doesn't exist on FileTypes
                           _logger.Debug("Successfully retrieved FileType Id: {FileTypeId} from FileTypeManager.", fileType.Id);
                      }
                  }
                  catch (Exception ex)
                  {
                       _logger.Error(ex, "Error calling FileTypeManager.GetFileType for FileTypeId: {FileTypeId}. Using original template FileType.", templateFileTypeId);
                       fileType = template.FileType; // Fallback on error
                  }
             } else {
                  _logger.Debug("Using original FileType from template (Id: {FileTypeId}).", originalFileTypeId);
             }
             _logger.Debug("Exiting ResolveFileType. Resolved FileType Id: {FileTypeId}", fileType?.Id);
             return fileType;
        }

        // Added filePath for context
        private static bool LogImportProcessingOutcome(bool processResult, string filePath)
        {
             _logger.Debug("Entering LogImportProcessingOutcome for File: {FilePath}. ProcessResult: {ProcessResult}", filePath, processResult);
             // Replace Console.WriteLine
             if (processResult)
             {
                  _logger.Information("DataFileProcessor completed successfully for File: {FilePath}.", filePath);
             }
             else
             {
                  _logger.Error("DataFileProcessor failed for File: {FilePath}.", filePath);
             }
             _logger.Debug("Exiting LogImportProcessingOutcome for File: {FilePath}. Returning: {ProcessResult}", filePath, processResult);
            return processResult; // Indicate success based on the result of DataFileProcessor().Process
        }

        private static DataFile CreateDataFile(Invoice template, FileTypes fileType)
        {
             string filePath = template?.FilePath ?? "Unknown";
             _logger.Debug("Entering CreateDataFile for File: {FilePath}, FileTypeId: {FileTypeId}", filePath, fileType?.Id ?? -1);
             // Null checks for template properties used here happen in IsRequiredDataMissing
             DataFile dataFile = null;
             try
             {
                 // Ensure DocSet is not null before accessing its properties if needed by DataFile constructor
                 if (template.DocSet == null || !template.DocSet.Any()) {
                     _logger.Error("Cannot create DataFile: DocSet is null or empty for File: {FilePath}", filePath);
                     return null;
                 }

                 dataFile = new DataFile(fileType, template.DocSet, template.OverWriteExisting,
                                                   template.EmailId,
                                                   template.FilePath, template.CsvLines);
                  _logger.Verbose("DataFile object created successfully for File: {FilePath}", filePath);
             }
             catch (Exception ex)
             {
                  _logger.Error(ex, "Error creating DataFile object for File: {FilePath}", filePath);
                  dataFile = null;
             }
             _logger.Debug("Exiting CreateDataFile for File: {FilePath}. DataFile is null: {IsDataFileNull}", filePath, dataFile == null);
             return dataFile;
        }
    }
}