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

        // Converted to regular method body for logging
        private static List<AsycudaDocumentSet_Attachments> GetOrSaveDocSetAttachmentsList(
            List<AsycudaDocumentSet> asycudaDocumentSets, string file, string emailId,
            int fileTypeId, FileInfo fileInfo)
        {
            // Cache context for logging, handle potential nulls
            string fileName = fileInfo?.Name ?? "UnknownFile";
            int docSetCount = asycudaDocumentSets?.Count ?? 0;
            _utilsLogger.Debug("GetOrSaveDocSetAttachmentsList called for File: {FileName}, EmailId: {EmailId}, FileTypeId: {FileTypeId} across {DocSetCount} DocumentSets.",
                fileName, emailId, fileTypeId, docSetCount);

            // Add null checks for critical inputs
            if (asycudaDocumentSets == null || !asycudaDocumentSets.Any())
            {
                _utilsLogger.Warning("GetOrSaveDocSetAttachmentsList called with null or empty AsycudaDocumentSet list for File: {FileName}. Returning empty list.", fileName);
                return new List<AsycudaDocumentSet_Attachments>();
            }
            if (fileInfo == null)
            {
                 _utilsLogger.Error("GetOrSaveDocSetAttachmentsList failed: FileInfo is null.");
                 return new List<AsycudaDocumentSet_Attachments>();
            }
             if (string.IsNullOrEmpty(file))
             {
                  _utilsLogger.Error("GetOrSaveDocSetAttachmentsList failed: file path string is null or empty.");
                  return new List<AsycudaDocumentSet_Attachments>();
             }

            List<AsycudaDocumentSet_Attachments> allAttachments = null;
            try
            {
                // Use SelectMany to process each docSet and flatten the results
                // Assuming GetDocSetAttachements handles its own internal logging and null checks
                allAttachments = asycudaDocumentSets
                    .Where(docSet => docSet != null) // Ensure docSet itself isn't null before processing
                    .SelectMany(docSet =>
                        {
                            // Log which docset is being processed inside the loop
                            _utilsLogger.Verbose("Processing DocSetId: {DocSetId} within GetOrSaveDocSetAttachmentsList for File: {FileName}", docSet.AsycudaDocumentSetId, fileName);
                            // GetDocSetAttachements returns List<...>
                            return GetDocSetAttachements(file, emailId, fileTypeId, fileInfo, docSet);
                        })
                    .Where(att => att != null) // Filter out any null attachments potentially returned by GetDocSetAttachements on error
                    .ToList();

                _utilsLogger.Information("GetOrSaveDocSetAttachmentsList completed for File: {FileName}. Total attachments retrieved/created: {TotalAttachmentCount} across {ProcessedDocSetCount} non-null DocumentSets.",
                    fileName, allAttachments?.Count ?? 0, asycudaDocumentSets.Count(ds => ds != null));
            }
            catch (Exception ex)
            {
                 _utilsLogger.Error(ex, "Error during GetOrSaveDocSetAttachmentsList processing for File: {FileName}", fileName);
                 return new List<AsycudaDocumentSet_Attachments>(); // Return empty list on error
            }

            // Ensure non-null return, although ToList usually handles this.
            return allAttachments ?? new List<AsycudaDocumentSet_Attachments>();
        }
    }
}