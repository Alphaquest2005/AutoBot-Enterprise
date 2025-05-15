using AsycudaDocumentSet = DocumentDS.Business.Entities.AsycudaDocumentSet;
using System.Collections.Generic; // Added
using System.IO; // Added
using Serilog; // Added
using System;
using System.Threading.Tasks; // Added
using OCR.Business.Entities; // Added for Line

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public static partial class InvoiceProcessingUtils
    {
        // Assuming _utilsLogger is defined in another partial class part
        // private static readonly ILogger _utilsLogger = Log.ForContext(typeof(InvoiceProcessingUtils));

        public static async Task ReportUnimportedFile(List<AsycudaDocumentSet> asycudaDocumentSets, string file, string emailId,
            int fileTypeId,
            EmailDownloader.Client client, string pdftxt, string error,
            List<Line> failedlst, ILogger logger)
        {
             logger.Warning("Starting ReportUnimportedFile process for File: {FilePath}, EmailId: {EmailId}, FileTypeId: {FileTypeId}",
                file, emailId, fileTypeId);

             // Add null checks for critical inputs
             if (string.IsNullOrEmpty(file))
             {
                  logger.Error("ReportUnimportedFile cannot proceed: file path is null or empty.");
                  return; // Stop processing if file path is invalid
             }
              if (client == null)
             {
                  logger.Error("ReportUnimportedFile cannot proceed: EmailDownloader.Client is null for File: {FilePath}", file);
                  return; // Stop processing if email client is missing
             }
              // Allow proceeding with null/empty lists/sets but log warnings
              if (asycudaDocumentSets == null)
             {
                  logger.Warning("ReportUnimportedFile proceeding with null AsycudaDocumentSet list for File: {FilePath}. SaveImportError might be affected.", file);
             }
              if (failedlst == null)
             {
                  logger.Warning("ReportUnimportedFile proceeding with null failed lines list for File: {FilePath}. Email/Test Case/Save Error might be affected.", file);
                  failedlst = new List<Line>(); // Use empty list to avoid null refs later
             }


             try
             {
                 logger.Debug("Creating FileInfo for File: {FilePath}", file);
                 var fileInfo = new FileInfo(file); // Can throw if path invalid

                 logger.Debug("Calling WriteTextFile for File: {FilePath}", file);
                 var txtFile = WriteTextFile(file, pdftxt, logger); // Pass logger
                 logger.Information("Text file written to: {TxtFilePath}", txtFile);

                 logger.Debug("Calling CreateEmail for File: {FilePath}", file);
                 // Assuming CreateEmail handles its own logging, including sending
                 var body = await CreateEmail(file, client, error, failedlst, fileInfo, txtFile).ConfigureAwait(false); // Need to check CreateEmail signature
                 logger.Information("Email creation/sending process initiated for File: {FilePath}. Body Length: {BodyLength}", file, body?.Length ?? 0);

                 logger.Debug("Calling CreateTestCase for File: {FilePath}", file);
                 // Assuming CreateTestCase handles its own logging
                 CreateTestCase(file, failedlst, txtFile, body); // Need to check CreateTestCase signature
                 logger.Information("Test case creation/logging process initiated for File: {FilePath}", file);


                 logger.Debug("Calling SaveImportError for File: {FilePath}", file);
                 // Assuming SaveImportError handles its own logging
                 SaveImportError(asycudaDocumentSets, file, emailId, fileTypeId, pdftxt, error, failedlst, fileInfo); // Need to check SaveImportError signature
                 logger.Information("Import error saving process initiated for File: {FilePath}", file); // Corrected logger name

                 logger.Warning("Finished ReportUnimportedFile process for File: {FilePath}", file);
             }
             catch (FileNotFoundException fnfEx) // More specific exception handling
             {
                  logger.Error(fnfEx, "Error creating FileInfo or accessing file during ReportUnimportedFile for Path: {FilePath}", file);
             }
             catch (IOException ioEx) // Handle IO errors (e.g., writing text file)
             {
                  logger.Error(ioEx, "IO Error during ReportUnimportedFile process for File: {FilePath}", file);
             }
             catch (Exception ex) // Catch other potential errors
             {
                  logger.Error(ex, "Unexpected error during ReportUnimportedFile process for File: {FilePath}", file);
                  // Decide if exception should be propagated or just logged
             }
        }
    }
}