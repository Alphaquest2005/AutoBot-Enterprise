using System;
using System.IO;
using System.Threading.Tasks;

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public partial class HandleErrorStateStep
    {
        private static async Task<bool> HandleErrorEmailPipeline(InvoiceProcessingContext context)
        {
            string filePath = context?.FilePath ?? "Unknown";
            _logger.Information("Starting HandleErrorEmailPipeline for File: {FilePath}", filePath);

            // Populate FileInfo and TxtFile in context for email pipeline
            try
            {
                _logger.Debug("Creating FileInfo for File: {FilePath}", filePath);
                context.FileInfo = new FileInfo(filePath); // Can throw if path is invalid

                // Assuming TxtFile was set in a previous step (e.g., WriteFormattedTextFileStep)
                if (string.IsNullOrEmpty(context.TxtFile))
                {
                    _logger.Warning(
                        "TxtFile is missing in context for File: {FilePath}. Email attachment might be incomplete.",
                        filePath);
                    // Decide if this is fatal for the email process
                }
                else
                {
                    _logger.Debug("Using TxtFile from context: {TxtFile}", context.TxtFile);
                }

                // Add FailedLines to context if not already there (needed by CreateEmailPipeline/ConstructEmailBodyStep)
                if (context.FailedLines == null)
                {
                    _logger.Warning(
                        "Context.FailedLines is null before calling CreateEmailPipeline. Attempting to populate for File: {FilePath}",
                        filePath);
                    // Re-calculate failed lines specifically for the email body generation
                    context.FailedLines = GetFailedLines(context); // Use the same logic
                    AddExistingFailedLines(context, context.FailedLines); // Add existing ones too
                    _logger.Information("Populated Context.FailedLines with {Count} lines for email generation.",
                        context.FailedLines.Count);
                }


                // Create and run the CreateEmailPipeline
                _logger.Debug("Creating CreateEmailPipeline instance for File: {FilePath}", filePath);
                var createEmailPipeline = new CreateEmailPipeline(context); // Handles its own logging

                _logger.Information("Running CreateEmailPipeline for File: {FilePath}", filePath);
                bool emailPipelineSuccess =
                    await createEmailPipeline.RunPipeline().ConfigureAwait(false); // Handles its own logging

                _logger.Information("CreateEmailPipeline finished for File: {FilePath}. Success: {Success}", filePath,
                    emailPipelineSuccess);
                return true; // Indicate that error handling (including email) was ATTEMPTED
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error during HandleErrorEmailPipeline for File: {FilePath}", filePath);
                return false; // Indicate failure in setting up or running the email pipeline
            }
        }
    }
}