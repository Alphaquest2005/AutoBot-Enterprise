using System.IO; // Added
using System.Threading.Tasks; // Added
using Serilog; // Added
using System; // Added

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public class WriteFormattedTextFileStep : IPipelineStep<InvoiceProcessingContext>
    {
        // Add a static logger instance for this class
        private static readonly ILogger _logger = Log.ForContext<WriteFormattedTextFileStep>();

        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            string filePath = context?.FilePath ?? "Unknown";
            _logger.Debug("Executing WriteFormattedTextFileStep for File: {FilePath}", filePath);

            // Null check context first
            if (context == null)
            {
                 _logger.Error("WriteFormattedTextFileStep executed with null context.");
                 return false;
            }

            // Corrected logic: Check if data is PRESENT
            if (!IsContextDataPresent(context)) // Handles its own logging
            {
                 _logger.Warning("WriteFormattedTextFileStep cannot proceed due to missing required data in context for File: {FilePath}", filePath);
                 return false; // Step fails if required data is missing
            }

            string txtFile = null; // Initialize outside try block
            try
            {
                _logger.Debug("Creating formatted text file path for File: {FilePath}", filePath);
                txtFile = CreateFormattedTextFilePath(context); // Renamed for clarity
                _logger.Information("Target text file path: {TxtFilePath}", txtFile);

                _logger.Debug("Writing formatted text (Length: {Length}) to file: {TxtFilePath}",
                    context.FormattedPdfText?.Length ?? 0, txtFile);

                // Wrap synchronous file IO in Task.Run to avoid blocking async pipeline thread
                // Use WriteAllTextAsync for true async operation if targeting .NET Core/.NET 5+
                // For now, using Task.Run with WriteAllText for compatibility.
                await Task.Run(() => File.WriteAllText(txtFile, context.FormattedPdfText)).ConfigureAwait(false);
                _logger.Information("Successfully wrote formatted text to {TxtFilePath}", txtFile);

                context.TxtFile = txtFile; // Set TxtFile in context
                 _logger.Debug("Set context.TxtFile to: {TxtFilePath}", txtFile);

                // Log success (replaces LogFileWriteSuccess)
                 _logger.Debug("Finished executing WriteFormattedTextFileStep successfully for File: {FilePath}", filePath);
                return true; // Indicate success
            }
            catch (UnauthorizedAccessException uaEx) // Specific exception
            {
                 _logger.Error(uaEx, "Unauthorized access error writing formatted text file: {TxtFilePath}", txtFile ?? filePath + ".txt");
                 return false;
            }
            catch (DirectoryNotFoundException dnfEx) // Specific exception
            {
                 _logger.Error(dnfEx, "Directory not found error writing formatted text file: {TxtFilePath}", txtFile ?? filePath + ".txt");
                 return false;
            }
            catch (IOException ioEx) // Catch specific IO errors
            {
                 _logger.Error(ioEx, "IO Error writing formatted text file: {TxtFilePath}", txtFile ?? filePath + ".txt");
                 return false;
            }
            catch (Exception ex) // Catch other potential errors
            {
                 _logger.Error(ex, "Error during WriteFormattedTextFileStep for File: {FilePath}", filePath);
                 return false; // Indicate failure
            }
        }

        // Renamed and corrected logic: Returns true if data is PRESENT
        private static bool IsContextDataPresent(InvoiceProcessingContext context)
        {
             _logger.Verbose("Checking if required data is present for writing formatted text file.");
             // Context null check happens in Execute
             if (string.IsNullOrEmpty(context.FilePath)) { _logger.Warning("Required data missing for writing text file: FilePath is null or empty."); return false; }
             // FormattedPdfText can be empty, but not null
             if (context.FormattedPdfText == null) { _logger.Warning("Required data missing for writing text file: FormattedPdfText is null."); return false; }

             _logger.Verbose("Required data is present for writing formatted text file.");
             return true; // All required data is present
        }

        // Renamed for clarity - only creates the path string
        private static string CreateFormattedTextFilePath(InvoiceProcessingContext context)
        {
            // Logic from the original WriteTextFile method
            // FilePath null/empty check happens in IsContextDataPresent
            var txtFile = context.FilePath + ".txt";
             _logger.Verbose("Generated text file path: {TxtFilePath}", txtFile);
            //if (File.Exists(txtFile)) return; // Original code had this commented out - File.WriteAllText overwrites anyway
            return txtFile;
        }

        // Removed LogFileWriteSuccess as logging is now inline
    }
}