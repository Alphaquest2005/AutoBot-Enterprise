using CoreEntities.Business.Entities;
using OCR.Business.Entities;
using System.Collections.Generic; // Added
using Serilog; // Added
using System; // Added

namespace WaterNut.DataSpace.PipelineInfrastructure;

public static partial class InvoiceProcessingUtils
{
    // Assuming _utilsLogger is defined in another partial class part
    // private static readonly ILogger _utilsLogger = Log.ForContext(typeof(InvoiceProcessingUtils));

    private static ImportErrors CreateNewImportErrors(string pdftxt, string error, List<Line> failedlst,
        AsycudaDocumentSet_Attachments att)
    {
        // Null check for critical input
        if (att == null)
        {
            _utilsLogger.Error("CreateNewImportErrors called with null AsycudaDocumentSet_Attachments.");
            return null; // Cannot proceed
        }

        int attachmentId = att.Id; // Cache for logging
        _utilsLogger.Debug("Creating new ImportErrors for AttachmentId: {AttachmentId}", attachmentId);
        ImportErrors importErr = null;
        try
        {
            // Note: Setting Id explicitly might be unusual if it's an identity column.
            // If Id is auto-generated on save, this might need adjustment.
            // Assuming Id corresponds to the attachment Id based on original code.
            importErr = new ImportErrors(true) { Id = attachmentId };
            _utilsLogger.Verbose("ImportErrors object created in memory with Id: {ImportErrorId} (matching AttachmentId)", importErr.Id);

            _utilsLogger.Debug("Calling UpdateImportError for AttachmentId: {AttachmentId}", attachmentId);
            // Assuming UpdateImportError handles its own internal logging and null checks
            UpdateImportError(pdftxt, error, failedlst, importErr, att);
            _utilsLogger.Debug("UpdateImportError finished for AttachmentId: {AttachmentId}", attachmentId);

            _utilsLogger.Information("Successfully created new ImportErrors object for AttachmentId: {AttachmentId}", attachmentId);
        }
        catch (Exception ex)
        {
             _utilsLogger.Error(ex, "Error creating new ImportErrors for AttachmentId: {AttachmentId}", attachmentId);
             // Depending on requirements, return null or rethrow
             return null;
        }

        return importErr;
    }
}