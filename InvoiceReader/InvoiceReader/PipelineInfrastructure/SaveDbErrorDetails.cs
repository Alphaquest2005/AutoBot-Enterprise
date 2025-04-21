using CoreEntities.Business.Entities;
using OCR.Business.Entities;
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

        private static void SaveDbErrorDetails(string pdftxt, string error, List<Line> failedlst,
            List<AsycudaDocumentSet_Attachments> existingAttachment)
        {
            int attachmentCount = existingAttachment?.Count ?? 0;
            _utilsLogger.Debug("Starting SaveDbErrorDetails for {AttachmentCount} attachments.", attachmentCount);

            // Null check for the list itself
            if (existingAttachment == null)
            {
                _utilsLogger.Warning("SaveDbErrorDetails called with null attachment list. Nothing to process.");
                return;
            }

            if (failedlst == null)
            {
                _utilsLogger.Warning(
                    "SaveDbErrorDetails called with null failed lines list. Error details might be incomplete.");
                failedlst = new List<Line>(); // Use empty list to avoid null refs later
            }


            foreach (var att in existingAttachment)
            {
                if (att == null)
                {
                    _utilsLogger.Warning("Skipping null attachment found in the list during SaveDbErrorDetails.");
                    continue;
                }

                _utilsLogger.Debug("Processing attachment Id: {AttachmentId} in SaveDbErrorDetails.", att.Id);
                // ProcessImportError handles its own logging and try/catch
                ProcessImportError(pdftxt, error, failedlst, att);
            }

            _utilsLogger.Debug("Finished SaveDbErrorDetails for {AttachmentCount} attachments.", attachmentCount);
        }

        private static void ProcessImportError(string pdftxt, string error, List<Line> failedlst,
            AsycudaDocumentSet_Attachments att)
        {
            // Attachment null check happens in caller
            int attachmentId = att.Id;
            _utilsLogger.Debug("Starting ProcessImportError for AttachmentId: {AttachmentId}", attachmentId);

            try
            {
                using (var ctx = new OCRContext()) // Assuming OCRContext is accessible and disposable
                {
                    _utilsLogger.Verbose("Querying for existing ImportErrors with Id: {AttachmentId}", attachmentId);
                    ImportErrors importErr = null;
                    try
                    {
                        importErr = ctx.ImportErrors.FirstOrDefault(x => x.Id == attachmentId);
                    }
                    catch (Exception queryEx)
                    {
                        _utilsLogger.Error(queryEx, "Error querying ImportErrors for Id: {AttachmentId}", attachmentId);
                        return; // Cannot proceed if query fails
                    }


                    if (importErr == null)
                    {
                        _utilsLogger.Information(
                            "No existing ImportErrors found for Id: {AttachmentId}. Creating new entry.", attachmentId);
                        // CreateNewImportErrors handles its own logging
                        importErr = CreateNewImportErrors(pdftxt, error, failedlst, att);
                        if (importErr != null)
                        {
                            _utilsLogger.Debug("Adding new ImportErrors (Id: {ImportErrorId}) to context.",
                                importErr.Id);
                            ctx.ImportErrors.Add(importErr);
                        }
                        else
                        {
                            _utilsLogger.Error(
                                "CreateNewImportErrors returned null for AttachmentId: {AttachmentId}. Cannot add error.",
                                attachmentId);
                            return; // Don't proceed if creation failed
                        }
                    }
                    else
                    {
                        _utilsLogger.Information("Existing ImportErrors found for Id: {AttachmentId}. Updating entry.",
                            attachmentId);
                        // UpdateImportError handles its own logging
                        UpdateImportError(pdftxt, error, failedlst, importErr, att);
                    }

                    _utilsLogger.Debug("Saving changes to OCRContext for AttachmentId: {AttachmentId}", attachmentId);
                    ctx.SaveChanges();
                    _utilsLogger.Information("Successfully saved ImportErrors details for AttachmentId: {AttachmentId}",
                        attachmentId);
                } // Context disposed here
            }
            catch (Exception ex) // Catch context creation or SaveChanges errors
            {
                _utilsLogger.Error(ex,
                    "Error during ProcessImportError (Context creation or SaveChanges) for AttachmentId: {AttachmentId}",
                    attachmentId);
                // Decide if exception should be propagated
            }

            _utilsLogger.Debug("Finished ProcessImportError for AttachmentId: {AttachmentId}", attachmentId);
        }
    }
}