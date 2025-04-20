using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;
using System.Collections.Generic; // Added
using System.IO; // Added
using Serilog; // Added
using System; // Added
using OCR.Business.Entities; // Added for Line
using System.Linq; // Added for Any()

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public static partial class InvoiceProcessingUtils
    {
        // Assuming _utilsLogger is defined in another partial class part
        // private static readonly ILogger _utilsLogger = Log.ForContext(typeof(InvoiceProcessingUtils));

        public static void SaveImportError(List<AsycudaDocumentSet> asycudaDocumentSets, string file, string emailId,
            int fileTypeId, string pdftxt,
            string error, List<Line> failedlst, FileInfo fileInfo)
        {
            string fileName = fileInfo?.Name ?? file ?? "UnknownFile"; // Use file path if FileInfo is null
            _utilsLogger.Debug("Starting SaveImportError process for File: {FileName}, EmailId: {EmailId}, FileTypeId: {FileTypeId}",
                fileName, emailId, fileTypeId);

            // Add null checks for critical inputs needed by downstream methods
            if (string.IsNullOrEmpty(file))
            {
                 _utilsLogger.Error("SaveImportError cannot proceed: file path is null or empty.");
                 return;
            }
             if (fileInfo == null)
            {
                 _utilsLogger.Warning("SaveImportError proceeding with null FileInfo for File: {FileName}. Downstream methods might be affected.", fileName);
                 // Allow proceeding but log warning
            }
             // Allow proceeding with null/empty lists/sets but log warnings
             if (asycudaDocumentSets == null)
            {
                  _utilsLogger.Warning("SaveImportError proceeding with null AsycudaDocumentSet list for File: {FileName}. Downstream methods might be affected.", fileName);
             }
              if (failedlst == null)
             {
                  _utilsLogger.Warning("SaveImportError proceeding with null failed lines list for File: {FileName}. Downstream methods might be affected.", fileName);
                  failedlst = new List<Line>(); // Use empty list to avoid null refs later
             }

            try
            {
                _utilsLogger.Debug("Calling GetOrSaveDocSetAttachmentsList for File: {FileName}", fileName);
                // Assuming GetOrSaveDocSetAttachmentsList handles its own logging
                var existingAttachment =
                    GetOrSaveDocSetAttachmentsList(asycudaDocumentSets, file, emailId, fileTypeId, fileInfo);
                _utilsLogger.Information("Retrieved/Saved {AttachmentCount} attachments for File: {FileName}", existingAttachment?.Count ?? 0, fileName);

                // Check if attachments were successfully retrieved/created before saving error details
                if (existingAttachment == null || !existingAttachment.Any())
                {
                     _utilsLogger.Error("Cannot save DB error details: No attachments found or created for File: {FileName}", fileName);
                     return; // Stop if no attachments to associate error with
                }

                _utilsLogger.Debug("Calling SaveDbErrorDetails for File: {FileName}", fileName);
                // Assuming SaveDbErrorDetails handles its own logging
                SaveDbErrorDetails(pdftxt, error, failedlst, existingAttachment);
                _utilsLogger.Information("Save DB error details process completed for File: {FileName}", fileName);

                 _utilsLogger.Debug("Finished SaveImportError process for File: {FileName}", fileName);
            }
            catch (Exception ex)
            {
                 _utilsLogger.Error(ex, "Error during SaveImportError process for File: {FileName}", fileName);
                 // Decide if exception should be propagated
            }
        }
    }
}