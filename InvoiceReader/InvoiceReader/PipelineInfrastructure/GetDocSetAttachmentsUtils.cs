using System.Data.Entity;
using CoreEntities.Business.Entities;
using AsycudaDocumentSet_Attachments = CoreEntities.Business.Entities.AsycudaDocumentSet_Attachments;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;
using System.Collections.Generic; // Added
using System.Linq; // Added
using Serilog; // Added
using System; // Added

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public static partial class InvoiceProcessingUtils
    {
        // Assuming _utilsLogger is defined in another partial class part
        // private static readonly ILogger _utilsLogger = Log.ForContext(typeof(InvoiceProcessingUtils));

        private static List<AsycudaDocumentSet_Attachments> GetDocSetAttachments(string file, AsycudaDocumentSet docSet)
        {
            // Cache context for logging, handle potential nulls
            int docSetId = docSet?.AsycudaDocumentSetId ?? -1;
            _utilsLogger.Debug("GetDocSetAttachments (plural) called for File: {FilePath}, DocSetId: {DocSetId}", file, docSetId);

            // Add null checks for critical inputs
            if (docSet == null)
            {
                 _utilsLogger.Error("GetDocSetAttachments failed: AsycudaDocumentSet is null for File: {FilePath}", file);
                 return new List<AsycudaDocumentSet_Attachments>(); // Return empty list
            }
             if (string.IsNullOrEmpty(file))
             {
                  _utilsLogger.Error("GetDocSetAttachments failed: file path string is null or empty for DocSetId: {DocSetId}", docSetId);
                  return new List<AsycudaDocumentSet_Attachments>(); // Return empty list
             }

            // Delegate to the specific query method, assuming it handles logging
            return GetAttachmentsByFilePath(file, docSet);
        }

        private static List<AsycudaDocumentSet_Attachments> GetAttachmentsByFilePath(string file, AsycudaDocumentSet docSet)
        {
            // docSet and file null checks happen in the caller (GetDocSetAttachments)
            int docSetId = docSet.AsycudaDocumentSetId;
            _utilsLogger.Debug("Querying database for attachments by FilePath: {FilePath}, DocSetId: {DocSetId}", file, docSetId);

            List<AsycudaDocumentSet_Attachments> docSetAttachments = null;
            try
            {
                using (var ctx = new CoreEntitiesContext())
                {
                    // Log the query details before execution
                    _utilsLogger.Verbose("Executing DB query: AsycudaDocumentSet_Attachments.Include(Attachments).Where(AsycudaDocumentSetId == {DocSetId} && Attachments.FilePath == {FilePath})",
                        docSetId, file);

                    docSetAttachments = ctx.AsycudaDocumentSet_Attachments
                        .Include(x => x.Attachments) // Eager load Attachments
                        .Where(x =>
                            x.AsycudaDocumentSetId == docSetId && x.Attachments.FilePath == file)
                        .ToList();

                    _utilsLogger.Information("Found {AttachmentCount} attachments in database for FilePath: {FilePath}, DocSetId: {DocSetId}",
                        docSetAttachments?.Count ?? 0, file, docSetId); // Use null-conditional count
                }
            }
            catch (Exception ex)
            {
                 _utilsLogger.Error(ex, "Error querying database for attachments by FilePath: {FilePath}, DocSetId: {DocSetId}", file, docSetId);
                 return new List<AsycudaDocumentSet_Attachments>(); // Return empty list on error to prevent downstream null refs
            }

            // Ensure a non-null list is always returned
            return docSetAttachments ?? new List<AsycudaDocumentSet_Attachments>();
        }
    }
}