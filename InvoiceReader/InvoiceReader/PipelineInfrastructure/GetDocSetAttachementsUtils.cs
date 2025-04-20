using AsycudaDocumentSet_Attachments = CoreEntities.Business.Entities.AsycudaDocumentSet_Attachments;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;
using System.Collections.Generic; // Added
using System.IO; // Added
using System.Linq; // Added
using Serilog; // Added
using System; // Added

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public static partial class InvoiceProcessingUtils
    {
        // Assuming _utilsLogger is defined in another partial class part
        // private static readonly ILogger _utilsLogger = Log.ForContext(typeof(InvoiceProcessingUtils));

        private static List<AsycudaDocumentSet_Attachments> GetDocSetAttachements(string file, string emailId,
            int fileTypeId, FileInfo fileInfo,
            AsycudaDocumentSet docSet)
        {
            // Cache context for logging, handle potential nulls
            int docSetId = docSet?.AsycudaDocumentSetId ?? -1;
            string fileName = fileInfo?.Name ?? "UnknownFile";
            _utilsLogger.Debug("GetDocSetAttachements called for File: {FileName}, EmailId: {EmailId}, FileTypeId: {FileTypeId}, DocSetId: {DocSetId}",
                fileName, emailId, fileTypeId, docSetId);

            // Add null checks for critical inputs that prevent proceeding
            if (docSet == null)
            {
                _utilsLogger.Error("GetDocSetAttachements failed: AsycudaDocumentSet is null for File: {FileName}", fileName);
                return new List<AsycudaDocumentSet_Attachments>(); // Return empty list
            }
             if (fileInfo == null)
            {
                 _utilsLogger.Error("GetDocSetAttachements failed: FileInfo is null."); // File name unknown here
                 return new List<AsycudaDocumentSet_Attachments>(); // Return empty list
            }
             if (string.IsNullOrEmpty(file))
             {
                  _utilsLogger.Error("GetDocSetAttachements failed: file path string is null or empty for DocSetId: {DocSetId}", docSetId);
                  return new List<AsycudaDocumentSet_Attachments>(); // Return empty list
             }


            List<AsycudaDocumentSet_Attachments> existingAttachments = null;
            try
            {
                _utilsLogger.Debug("Calling GetDocSetAttachments (plural) to find existing attachments for File: {FileName}, DocSetId: {DocSetId}", fileName, docSetId);
                // Assuming GetDocSetAttachments handles its own logging and returns a non-null list
                existingAttachments = GetDocSetAttachments(file, docSet); // Note the 's' - different method?
                _utilsLogger.Debug("GetDocSetAttachments (plural) returned {AttachmentCount} attachments for File: {FileName}, DocSetId: {DocSetId}",
                    existingAttachments.Count, fileName, docSetId); // Assuming non-null return

                if (existingAttachments.Any())
                {
                    _utilsLogger.Information("Found {AttachmentCount} existing attachments for File: {FileName}, DocSetId: {DocSetId}. Returning existing.",
                        existingAttachments.Count, fileName, docSetId);
                    return existingAttachments;
                }

                // If no existing attachments found, create a new one
                _utilsLogger.Information("No existing attachments found for File: {FileName}, DocSetId: {DocSetId}. Creating new attachment.", fileName, docSetId);
                _utilsLogger.Debug("Calling CreateDocSetAttachment for File: {FileName}, DocSetId: {DocSetId}", fileName, docSetId);
                // Assuming CreateDocSetAttachment handles its own logging and null checks
                var newAttachment = CreateDocSetAttachment(file, emailId, fileTypeId, fileInfo, docSet);

                if (newAttachment != null)
                {
                    _utilsLogger.Information("Successfully created new attachment with Id: {AttachmentId} for File: {FileName}, DocSetId: {DocSetId}",
                        newAttachment.Id, fileName, docSetId);
                    // IMPORTANT: Original code added to the *result* of GetDocSetAttachments.
                    // If GetDocSetAttachments returned an empty list, we should return a *new* list containing the new item.
                    return new List<AsycudaDocumentSet_Attachments> { newAttachment };
                }
                else
                {
                    _utilsLogger.Error("CreateDocSetAttachment returned null for File: {FileName}, DocSetId: {DocSetId}. Returning empty list.", fileName, docSetId);
                    return new List<AsycudaDocumentSet_Attachments>(); // Return empty list if creation failed
                }
            }
            catch (Exception ex)
            {
                 _utilsLogger.Error(ex, "Error in GetDocSetAttachements for File: {FileName}, DocSetId: {DocSetId}", fileName, docSetId);
                 return new List<AsycudaDocumentSet_Attachments>(); // Return empty list on error
            }
            // The original code returned the potentially modified 'existingAttachment' list here.
            // However, if a new attachment was created, 'existingAttachment' would still be empty.
            // The logic above now correctly returns a list with the new attachment or an empty list on failure.
        }
    }
}