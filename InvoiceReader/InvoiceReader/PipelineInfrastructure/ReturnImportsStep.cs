// Assuming ImportStatus is here

using System.Linq; // Added
using System.Threading.Tasks; // Added
using Serilog; // Added
using System; // Added
using WaterNut.DataSpace;
using WaterNut.DataSpace.PipelineInfrastructure;

 // Added for ImportStatus enum
using System.Collections.Generic; // Added for Dictionary

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public class ReturnImportsStep : IPipelineStep<InvoiceProcessingContext>
    {
        // Add a static logger instance for this class
        private static readonly ILogger _logger = Log.ForContext<ReturnImportsStep>();

        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            // FilePath might not be relevant here if this is a final summary step across multiple files/templates
            _logger.Debug("Executing ReturnImportsStep. Finalizing import process.");

            // Null check context
            if (context == null)
            {
                 _logger.Error("ReturnImportsStep executed with null context.");
                 return false; // Cannot determine success without context
            }

            bool overallSuccess = false; // Default to false
            int successCount = 0;
            int hasErrorsCount = 0;
            int failedCount = 0;
            int totalImports = 0;

            try
            {
                // Determine overall success based on the ImportStatus of processed files
                if (context.Imports != null)
                {
                    totalImports = context.Imports.Count;
                    // Count statuses for detailed logging
                    successCount = context.Imports.Count(x => x.Value.Success == ImportStatus.Success);
                    hasErrorsCount = context.Imports.Count(x => x.Value.Success == ImportStatus.HasErrors);
                    // Assuming any status not Success or HasErrors is Failed for counting purposes
                    failedCount = totalImports - successCount - hasErrorsCount;

                    // Original logic: success if *any* import was Success or HasErrors
                    overallSuccess = context.Imports.Any(x => x.Value.Success == ImportStatus.Success ||
                                                               x.Value.Success == ImportStatus.HasErrors);

                     _logger.Information("Final import summary: Total Processed={Total}, Success={Success}, HasErrors={HasErrors}, Failed={Failed}. Overall Success Flag: {OverallSuccess}",
                        totalImports, successCount, hasErrorsCount, failedCount, overallSuccess);
                }
                else
                {
                     _logger.Warning("Context.Imports dictionary is null. Cannot determine overall import success.");
                     overallSuccess = false; // Treat as failure if Imports dictionary is missing
                }

                 _logger.Debug("Finished executing ReturnImportsStep. Returning Overall Success: {OverallSuccess}", overallSuccess);
                return overallSuccess; // Indicate overall success or failure based on the check
            }
            catch (Exception ex)
            {
                 _logger.Error(ex, "Error during ReturnImportsStep execution.");
                 return false; // Indicate failure on error
            }
        }
    }
}