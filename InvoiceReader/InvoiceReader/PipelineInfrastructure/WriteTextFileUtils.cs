using System.IO; // Added
using Serilog; // Added
using System; // Added

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public static partial class InvoiceProcessingUtils
    {
        // Assuming _utilsLogger is defined in another partial class part
        // private static readonly ILogger _utilsLogger = Log.ForContext(typeof(InvoiceProcessingUtils));

        public static string WriteTextFile(string file, string pdftxt)
        {
             _utilsLogger.Debug("WriteTextFile called for base file: {BaseFilePath}", file);

             // Null/Empty checks for inputs
             if (string.IsNullOrEmpty(file))
             {
                  _utilsLogger.Error("WriteTextFile cannot proceed: base file path is null or empty.");
                  return null; // Return null or throw, depending on desired error handling
             }
             // pdftxt can be null or empty, WriteAllText handles this. Log a warning if null.
             if (pdftxt == null)
             {
                  _utilsLogger.Warning("WriteTextFile called with null pdftxt for base file: {BaseFilePath}. An empty file will be created/overwritten.", file);
                  pdftxt = string.Empty; // Ensure not null for WriteAllText
             }

             string txtFile = null; // Initialize outside try block
             try
             {
                 txtFile = file + ".txt";
                 _utilsLogger.Information("Writing text (Length: {Length}) to file: {TxtFilePath}", pdftxt.Length, txtFile);

                 // File.WriteAllText is synchronous. If this utility is called frequently
                 // or within performance-sensitive loops, consider async alternatives or Task.Run.
                 // For now, keeping it synchronous as per original code.
                 File.WriteAllText(txtFile, pdftxt);

                 _utilsLogger.Information("Successfully wrote text to {TxtFilePath}", txtFile);
                 return txtFile;
             }
             catch (UnauthorizedAccessException uaEx) // Specific exception
             {
                  _utilsLogger.Error(uaEx, "Unauthorized access error writing text file: {TxtFilePath}", txtFile ?? file + ".txt");
                  return null; // Indicate failure
             }
             catch (DirectoryNotFoundException dnfEx) // Specific exception
             {
                  _utilsLogger.Error(dnfEx, "Directory not found error writing text file: {TxtFilePath}", txtFile ?? file + ".txt");
                  return null; // Indicate failure
             }
             catch (IOException ioEx) // Catch specific IO errors
             {
                  _utilsLogger.Error(ioEx, "IO Error writing text file: {TxtFilePath}", txtFile ?? file + ".txt");
                  return null; // Indicate failure
             }
             catch (Exception ex) // Catch other potential errors
             {
                  _utilsLogger.Error(ex, "Error during WriteTextFile for base file: {BaseFilePath}", file);
                  return null; // Indicate failure
             }
        }
    }
}