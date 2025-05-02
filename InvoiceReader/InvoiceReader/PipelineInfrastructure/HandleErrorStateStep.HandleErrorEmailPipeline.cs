using System;
using System.IO;
using System.Threading.Tasks;

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public partial class HandleErrorStateStep
    {
        // Added InvoiceProcessingContext context parameter
        private static async Task<bool> HandleErrorEmailPipeline(InvoiceProcessingContext context, Invoice template, string filePath)
        {
             filePath = filePath ?? context?.FilePath ?? "Unknown"; // Get filePath from context if null
            _logger.Information("Starting HandleErrorEmailPipeline for File: {FilePath}", filePath);

            // Populate FileInfo and TextFilePath in template for email pipeline
            try
            {
                _logger.Debug("Creating FileInfo for File: {FilePath}", filePath);
                

                // Assuming TextFilePath was set in a previous step (e.g., WriteFormattedTextFileStep)
                if (string.IsNullOrEmpty(template.FormattedPdfText))
                {
                    _logger.Warning(
                        "TextFilePath is missing in template for File: {FilePath}. Email attachment might be incomplete.",
                        filePath);
                    // Decide if this is fatal for the email process
                }
                else
                {
                    _logger.Debug("Using TextFilePath from template: {TextFilePath}", template.FormattedPdfText);
                }

                // Add FailedLines to template if not already there (needed by CreateEmailPipeline/ConstructEmailBodyStep)
                if (template.FailedLines == null)
                {
                    _logger.Warning(
                        "Context.FailedLines is null before calling CreateEmailPipeline. Attempting to populate for File: {FilePath}",
                        filePath);
                    // Re-calculate failed lines specifically for the email body generation
                    template.FailedLines = GetFailedLines(template); // Use the same logic
                    AddExistingFailedLines(template, template.FailedLines); // Add existing ones too
                    _logger.Information("Populated Context.FailedLines with {Count} lines for email generation.",
                        template.FailedLines.Count);
                }


                // Create and run the CreateEmailPipeline
                _logger.Debug("Creating CreateEmailPipeline instance for File: {FilePath}", filePath);
                var createEmailPipeline = new CreateEmailPipeline(context); // Pass the context parameter

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