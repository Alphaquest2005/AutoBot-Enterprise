using CoreEntities.Business.Entities;
using OCR.Business.Entities;
using System.Collections.Generic; // Added
using System.Linq; // Added
using Serilog; // Added
using System; // Added

namespace WaterNut.DataSpace.PipelineInfrastructure;

// Removed duplicate empty partial class definition
public static partial class InvoiceProcessingUtils
{
    // Assuming _utilsLogger is defined in another partial class part
    // private static readonly ILogger _utilsLogger = Log.ForContext(typeof(InvoiceProcessingUtils));

    private static void UpdateImportError(string pdftxt, string error, List<Line> failedlst, ImportErrors importErr,
        AsycudaDocumentSet_Attachments att)
    {
        // Null checks for critical inputs
        if (importErr == null)
        {
            _utilsLogger.Error("UpdateImportError called with null ImportErrors object. Cannot update.");
            return;
        }
        // Attachment can be null, but log a warning as it affects FailedLines creation
        if (att == null)
        {
             _utilsLogger.Warning("UpdateImportError called with null AsycudaDocumentSet_Attachments object for ImportErrors Id: {ImportErrorId}. Cannot create/update failed lines.", importErr.Id);
        }
         // Failed list can be null, default to empty
         if (failedlst == null)
        {
             _utilsLogger.Warning("UpdateImportError called with null failed lines list for ImportErrors Id: {ImportErrorId}. OCR_FailedLines will be empty.", importErr.Id);
             failedlst = new List<Line>(); // Use empty list to avoid null refs later
        }

        int importErrorId = importErr.Id; // Cache for logging
        _utilsLogger.Debug("Updating ImportErrors object with Id: {ImportErrorId}", importErrorId);

        try
        {
            importErr.PdfText = pdftxt;
             _utilsLogger.Verbose("Updated ImportErrors Id: {ImportErrorId} - PdfText Length: {Length}", importErrorId, pdftxt?.Length ?? 0);

            importErr.Error = error;
             _utilsLogger.Verbose("Updated ImportErrors Id: {ImportErrorId} - Error: {Error}", importErrorId, error); // Be cautious logging full error if it contains sensitive info

            importErr.EntryDateTime = DateTime.Now;
             _utilsLogger.Verbose("Updated ImportErrors Id: {ImportErrorId} - EntryDateTime set to: {DateTime}", importErrorId, importErr.EntryDateTime);

            // Process failed lines only if attachment is not null
            if (att != null)
            {
                 _utilsLogger.Debug("Processing {FailedLineCount} failed lines to create/update OCR_FailedLines for ImportErrors Id: {ImportErrorId}", failedlst.Count, importErrorId);
                 // Assuming CreateFailedLines handles its own logging and null checks within the list
                 // Also assuming CreateFailedLines returns a new object not attached to any context yet.
                 // If the existing importErr.OCR_FailedLines needs merging/updating, more complex logic is required here.
                 // Current code replaces the entire collection.
                 importErr.OCR_FailedLines = failedlst
                                                .Select(x => CreateFailedLines(att, x)) // CreateFailedLines handles logging
                                                .Where(fl => fl != null) // Filter out nulls if CreateFailedLines can return null on error
                                                .ToList();
                 _utilsLogger.Information("Updated ImportErrors Id: {ImportErrorId} - Replaced OCR_FailedLines collection with {FailedLinesCount} new entries.", importErrorId, importErr.OCR_FailedLines?.Count ?? 0);
            } else {
                 _utilsLogger.Warning("Skipped processing OCR_FailedLines for ImportErrors Id: {ImportErrorId} because attachment was null.", importErrorId);
                 // Decide if existing failed lines should be cleared or kept when attachment is null
                 // importErr.OCR_FailedLines = new List<OCR_FailedLines>(); // Option: Clear existing if attachment is missing
            }

             _utilsLogger.Debug("Finished updating ImportErrors object Id: {ImportErrorId}", importErrorId);
        }
        catch (Exception ex)
        {
             _utilsLogger.Error(ex, "Error updating ImportErrors object Id: {ImportErrorId}", importErrorId);
             // Decide if exception should be propagated, especially if called within a transaction
        }
    }
}