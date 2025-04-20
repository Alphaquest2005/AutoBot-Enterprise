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
    public partial class UpdateImportStatusStep : IPipelineStep<InvoiceProcessingContext>
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

        // Added context parameters for logging
    }
}