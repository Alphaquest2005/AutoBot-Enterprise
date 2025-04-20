using WaterNut.Business.Services.Utils; // Assuming FileTypeManager is here
using WaterNut.DataSpace; // Added for ImportStatus enum
using System.Threading.Tasks; // Added
using Serilog; // Added
using System; // Added
using System.Collections.Generic; // Added for Dictionary access
using System.Linq; // Added for Any()
using CoreEntities.Business.Entities; // Added for FileTypes

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public class UpdateImportStatusStep : IPipelineStep<InvoiceProcessingContext>
    {
        // Add a static logger instance for this class
        private static readonly ILogger _logger = Log.ForContext<UpdateImportStatusStep>();

        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            string filePath = context?.FilePath ?? "Unknown";
            int? templateId = context?.Template?.OcrInvoices?.Id; // Safe access
            _logger.Debug("Executing UpdateImportStatusStep for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);

            // Null check context first
            if (context == null)
            {
                 _logger.Error("UpdateImportStatusStep executed with null context.");
                 return false;
            }

            // Corrected logic: Check if required data is PRESENT, not missing
            if (IsImportDataPresent(context)) // Handles its own logging
            {
                _logger.Debug("Required data is present. Processing import status update for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
                ImportStatus finalStatus = ProcessImportFile(context); // Handles its own logging
                // LogImportStatusUpdate handles its own logging
                return LogImportStatusUpdate(finalStatus, filePath, templateId); // Pass context
            }
            else
            {
                 _logger.Warning("UpdateImportStatusStep cannot proceed due to missing required data in context for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
                 return false; // Indicate failure as required data is missing
            }
        }

        // Renamed and corrected logic: Returns true if data needed for this step is PRESENT
        private static bool IsImportDataPresent(InvoiceProcessingContext context)
        {
             _logger.Verbose("Checking if required data is present for import status update.");
             // Check each property and log which one is missing if any
             // Context null check happens in Execute
             if (context.Template == null) { _logger.Warning("Required data missing for status update: Template is null."); return false; }
             // Check Template.OcrInvoices as it's used later
             if (context.Template.OcrInvoices == null) { _logger.Warning("Required data missing for status update: Template.OcrInvoices is null."); return false; }
             if (string.IsNullOrEmpty(context.FilePath)) { _logger.Warning("Required data missing for status update: FilePath is null or empty."); return false; }
             if (context.Imports == null) { _logger.Warning("Required data missing for status update: Imports dictionary is null."); return false; }
             // ImportStatus itself is read from the context, but its absence isn't checked here,
             // ProcessImportFile handles the default case (likely 'Failed' if not set previously).

             _logger.Verbose("Required data is present for import status update.");
             return true; // All required data is present
        }

        // Added context parameters for logging
        private static bool LogImportStatusUpdate(ImportStatus importStatus, string filePath, int? templateId)
        {
             _logger.Information("Import status processed as {ImportStatus} for File: {FilePath}, TemplateId: {TemplateId}", importStatus, filePath, templateId);
             // This step's success depends only on reaching this point after processing, not the status itself.
             return true;
        }

        private static ImportStatus ProcessImportFile(InvoiceProcessingContext context)
        {
             // Required data checks happen in caller (IsImportDataPresent)
             string filePath = context.FilePath;
             int templateId = context.Template.OcrInvoices.Id; // Already checked not null
             string templateName = context.Template.OcrInvoices.Name ?? "UnknownName";
             string fileDescription = "Unknown Description"; // Default

             _logger.Debug("Processing import file status for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);

             try
             {
                 // Safely get FileType description
                 int fileTypeId = context.Template.OcrInvoices.FileTypeId;
                 _logger.Verbose("Attempting to get FileType description for FileTypeId: {FileTypeId}", fileTypeId);
                 // Corrected: Use static GetFileType method
                 var fileType = FileTypeManager.GetFileType(fileTypeId);
                 if (fileType != null)
                 {
                     // Corrected: Use Description property if it exists, otherwise log ID
                     fileDescription = fileType.Description ?? $"FileTypeID_{fileTypeId}";
                     _logger.Verbose("Retrieved FileType Description: '{FileDescription}' for FileTypeId: {FileTypeId}", fileDescription, fileTypeId);
                 } else {
                     _logger.Warning("FileTypeManager returned null for FileTypeId: {FileTypeId}. Using default description.", fileTypeId);
                 }
             }
             catch (Exception ex)
             {
                  _logger.Error(ex, "Error getting FileType description from FileTypeManager for FileTypeId: {FileTypeId}", context.Template.OcrInvoices.FileTypeId);
                  // Continue with default description
             }


             // Assuming ImportStatus is available in the context based on previous steps
             // If not set, it defaults to 0 (likely 'Failed')
             ImportStatus importStatus = context.ImportStatus;
             _logger.Information("Current ImportStatus from context: {ImportStatus} for File: {FilePath}, TemplateId: {TemplateId}", importStatus, filePath, templateId);

             string importKey = $"{filePath}-{templateName}-{templateId}";
             _logger.Debug("Generated Imports dictionary key: {ImportKey}", importKey);

             try
             {
                 // Use TryAdd for safety, although key should be unique per template run
                 // If TryAdd fails, overwrite the existing entry.
                 if (!context.Imports.TryAdd(importKey, (filePath, fileDescription, importStatus)))
                 {
                      _logger.Warning("Failed to add entry to Imports dictionary (Key already exists?): Key: {ImportKey}, Status: {ImportStatus}. Overwriting existing entry.", importKey, importStatus);
                      // Overwrite the existing entry if TryAdd fails
                      context.Imports[importKey] = (filePath, fileDescription, importStatus);
                 } else {
                      _logger.Information("Added entry to Imports dictionary with Key: {ImportKey}, Status: {ImportStatus}", importKey, importStatus);
                 }
             }
             catch (Exception ex)
             {
                  _logger.Error(ex, "Error adding/updating entry in Imports dictionary for Key: {ImportKey}", importKey);
                  // Depending on requirements, might need to handle this failure more explicitly
             }


             // The method returns the status that was processed and added to the dictionary
             return importStatus;
        }
    }
}