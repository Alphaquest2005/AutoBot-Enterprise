using CoreEntities.Business.Entities;
using TrackableEntities;
using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;
using System.IO; // Needed for FileInfo
using Serilog; // Add Serilog using statement
using System; // Needed for Exception

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public static partial class InvoiceProcessingUtils
    {
        // Add a static logger instance for this partial class
        // Note: If other partial parts of InvoiceProcessingUtils also define this, it's okay.
        // Ensure this logger is initialized where Serilog is configured (usually the main application).
        private static readonly ILogger _utilsLogger = Log.ForContext(typeof(InvoiceProcessingUtils));

        private static AsycudaDocumentSet_Attachments CreateDocSetAttachment(string file, string emailId,
            int fileTypeId,
            FileInfo fileInfo, AsycudaDocumentSet docSet)
        {
            _utilsLogger.Debug(
                "Attempting to create DocSet Attachment for File: {FilePath}, EmailId: {EmailId}, FileTypeId: {FileTypeId}, DocSetId: {DocSetId}",
                file, emailId, fileTypeId, docSet?.AsycudaDocumentSetId ?? -1); // Added null check for docSet

            if (docSet == null)
            {
                _utilsLogger.Error("Cannot create attachment because AsycudaDocumentSet is null for File: {FilePath}",
                    file);
                return null; // Or throw an exception
            }

            if (fileInfo == null)
            {
                _utilsLogger.Error("Cannot create attachment because FileInfo is null for File: {FilePath}", file);
                return null; // Or throw an exception
            }


            AsycudaDocumentSet_Attachments newAttachment = null;
            try
            {
                using (var ctx = new CoreEntitiesContext())
                {
                    _utilsLogger.Verbose(
                        "Creating new AsycudaDocumentSet_Attachments object in memory for File: {FilePath}", file);
                    newAttachment = new AsycudaDocumentSet_Attachments(true)
                    {
                        TrackingState = TrackingState.Added,
                        AsycudaDocumentSetId = docSet.AsycudaDocumentSetId,
                        Attachments = new Attachments(true)
                        {
                            TrackingState = TrackingState.Added,
                            EmailId = emailId,
                            FilePath = file
                        },
                        DocumentSpecific = true,
                        FileDate = fileInfo.CreationTime,
                        FileTypeId = fileTypeId,
                    };
                    ctx.AsycudaDocumentSet_Attachments.Add(newAttachment);
                    _utilsLogger.Debug("Saving new attachment to database for File: {FilePath}, DocSetId: {DocSetId}",
                        file, docSet.AsycudaDocumentSetId);
                    ctx.SaveChanges();
                    // Assuming AsycudaDocumentSet_Attachments has an 'Id' property populated after SaveChanges
                    _utilsLogger.Information(
                        "Successfully created and saved DocSet Attachment with Id: {AttachmentId} for File: {FilePath}, DocSetId: {DocSetId}",
                        newAttachment?.Id ?? -1, file, docSet.AsycudaDocumentSetId);
                }
            }
            catch (Exception ex)
            {
                _utilsLogger.Error(ex,
                    "Failed to create or save DocSet Attachment for File: {FilePath}, EmailId: {EmailId}, DocSetId: {DocSetId}",
                    file, emailId, docSet.AsycudaDocumentSetId);
                // Depending on requirements, you might want to re-throw, return null, or handle differently
                return null; // Return null on failure
            }

            return newAttachment;
        }
    }
}