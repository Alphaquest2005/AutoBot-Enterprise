// Assuming PipelineRunner is here

using System.Collections.Generic; // Needed for List<>
using System.Threading.Tasks;
using Serilog; // Add Serilog using statement

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public class CreateEmailPipeline
    {
        // Add a static logger instance for this class
        private static readonly ILogger _logger = Log.ForContext<CreateEmailPipeline>();

        private readonly InvoiceProcessingContext _context;

        public CreateEmailPipeline(InvoiceProcessingContext context)
        {
            _context = context;
             // Add null check for context before accessing FilePath
             _logger.Debug("CreateEmailPipeline initialized for File: {FilePath}", _context?.FilePath ?? "Unknown");
        }

        public async Task<bool> RunPipeline()
        {
             // Add null check for context before accessing FilePath
             string filePath = _context?.FilePath ?? "Unknown";
             _logger.Information("Starting CreateEmailPipeline execution for File: {FilePath}", filePath);

            List<IPipelineStep<InvoiceProcessingContext>> emailPipelineSteps = CreateEmailPipelineSteps();
             _logger.Debug("Created {StepCount} steps for email pipeline for File: {FilePath}", emailPipelineSteps.Count, filePath);

            // Create and run the email pipeline
            var emailPipelineRunner = new PipelineRunner<InvoiceProcessingContext>(emailPipelineSteps);
             _logger.Debug("Email PipelineRunner created for File: {FilePath}", filePath);

             _logger.Information("Running email pipeline steps for File: {FilePath}", filePath);
            bool success = await emailPipelineRunner.Run(_context).ConfigureAwait(false);

            return LogPipelineResult(success, filePath); // Pass FilePath for logging
        }

        // Modified to accept FilePath for logging context
        private static bool LogPipelineResult(bool success, string filePath)
        {
             // Replace Console.WriteLine with Serilog Information/Warning log
             if (success)
             {
                 _logger.Information("CreateEmailPipeline finished successfully for File: {FilePath}.", filePath);
             }
             else
             {
                 _logger.Warning("CreateEmailPipeline finished with failures for File: {FilePath}.", filePath);
             }

            return success; // Indicate if the email pipeline completed successfully
        }

        private static List<IPipelineStep<InvoiceProcessingContext>> CreateEmailPipelineSteps()
        {
             _logger.Debug("Creating email pipeline steps.");
            // Define the email creation pipeline steps
            var steps = new List<IPipelineStep<InvoiceProcessingContext>>
            {
                new ConstructEmailBodyStep(),
                new SendEmailStep()
                // Consider adding more steps here if needed for email processing
            };
             _logger.Debug("Finished creating email pipeline steps. Count: {StepCount}", steps.Count);
            return steps;
        }
    }
}