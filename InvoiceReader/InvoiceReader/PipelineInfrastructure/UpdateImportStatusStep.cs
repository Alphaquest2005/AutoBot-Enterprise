using System.Threading.Tasks;
using WaterNut.Business.Services.Utils; // Assuming FileTypeManager is here
using WaterNut.DataSpace;
using WaterNut.DataSpace.PipelineInfrastructure;

 // Added for ImportStatus enum
using System.Threading.Tasks; // Added
using Serilog; // Added
using System; // Added
using System.Collections.Generic; // Added for Dictionary access
using System.Linq; // Added for Any()
using CoreEntities.Business.Entities; // Added for FileTypes

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public partial class UpdateImportStatusStep : IPipelineStep<InvoiceProcessingContext>
    {
        // Add a static logger instance for this class
        private static readonly ILogger _logger = Log.ForContext<UpdateImportStatusStep>();

        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
             // Basic context validation
            if (context == null)
            {
                _logger.Error("UpdateImportStatusStep executed with null context.");
                return false;
            }
             if (!context.Templates.Any())
            {
                 _logger.Warning("Skipping UpdateImportStatusStep: No Templates found in context for File: {FilePath}", context.FilePath ?? "Unknown");
                 return true; // No templates to process, not a failure of the step itself.
            }

            string filePath = context.FilePath ?? "Unknown";

            foreach (var template in context.Templates)
            {
                 int? templateId = template?.OcrInvoices?.Id; // Safe access
                 _logger.Debug("Executing UpdateImportStatusStep for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);

                 try
                 {
                     // --- Data Presence Check ---
                     if (!IsImportDataPresent(context)) // Handles its own logging
                     {
                         string errorMsg = $"UpdateImportStatusStep cannot proceed due to missing required data for File: {filePath}, TemplateId: {templateId}";
                         _logger.Warning(errorMsg); // Logged by IsImportDataPresent, but log again here for step context
                         context.AddError(errorMsg); // Add error to context
                         return false; // Stop processing immediately
                     }
                     // --- End Data Presence Check ---

                     _logger.Debug("Required data is present. Processing import status update for File: {FilePath}, TemplateId: {TemplateId}", filePath, templateId);
                     
                     // --- Process and Log Status ---
                     // Assuming ProcessImportFile and LogImportStatusUpdate might throw exceptions
                     ImportStatus finalStatus = ProcessImportFile(context); // Handles its own logging
                     bool logSuccess = LogImportStatusUpdate(finalStatus, filePath, templateId); // Handles its own logging
                     
                     if (!logSuccess) // If LogImportStatusUpdate indicates an issue (e.g., couldn't save status)
                     {
                          string errorMsg = $"LogImportStatusUpdate reported failure for File: {filePath}, TemplateId: {templateId}, Status: {finalStatus}";
                           _logger.Warning(errorMsg); // Log the failure from the logging step
                           context.AddError(errorMsg); // Add error to context
                           // Decide if this is critical enough to stop the whole step. Assuming yes for now.
                           return false;
                     }
                     // --- End Process and Log Status ---

                     // If we reach here, status update for this template was successful.
                 }
                 catch (Exception ex) // Catch unexpected errors during processing for this template
                 {
                     string errorMsg = $"Error during UpdateImportStatusStep for File: {filePath}, TemplateId: {templateId}: {ex.Message}";
                     _logger.Error(ex, errorMsg); // Log the error with exception details
                     context.AddError(errorMsg); // Add error to context
                     return false; // Stop processing immediately
                 }
            }

            // If the loop completes without any template causing a 'return false', the step is successful.
            _logger.Information("UpdateImportStatusStep completed successfully for all applicable templates in File: {FilePath}.", filePath);
            return true;
        }

        // Renamed and corrected logic: Returns true if data needed for this step is PRESENT

        // Added context parameters for logging
    }
}