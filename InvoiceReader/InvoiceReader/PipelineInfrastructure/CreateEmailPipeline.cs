// Assuming PipelineRunner is here

using System.Collections.Generic; // Needed for List<>
using System.Threading.Tasks;
using Serilog; // Add Serilog using statement

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    using System;
    using System.Diagnostics;

    public class CreateEmailPipeline
    {
        // Add a static logger instance for this class
        // Remove static logger instance
        // private static readonly ILogger _logger = Log.ForContext<CreateEmailPipeline>();

        private readonly InvoiceProcessingContext _context;
        private readonly ILogger _logger; // Add instance logger field

        public CreateEmailPipeline(ILogger logger, InvoiceProcessingContext context) // Add logger parameter
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger)); // Assign logger, add null check
            _context = context ?? throw new ArgumentNullException(nameof(context)); // Assign context, add null check
             // Add null check for context before accessing FilePath
             _logger.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                 nameof(CreateEmailPipeline), "Initialization", "CreateEmailPipeline initialized.", $"File: {_context.FilePath ?? "Unknown"}", "");
        }

        public async Task<bool> RunPipeline()
        {
            var pipelineStopwatch = Stopwatch.StartNew(); // Start stopwatch for pipeline execution
            string filePath = _context.FilePath ?? "Unknown";
            _logger.Information("ACTION_START: {ActionName}. Context: [{ActionContext}]",
                nameof(CreateEmailPipeline), $"Starting email pipeline execution for file: {filePath}");

            try
            {
                List<IPipelineStep<InvoiceProcessingContext>> emailPipelineSteps = CreateEmailPipelineSteps();
                _logger.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(RunPipeline), "Setup", "Created steps for email pipeline.", $"StepCount: {emailPipelineSteps.Count}, File: {filePath}", "");

                // Create and run the email pipeline
                var emailPipelineRunner = new PipelineRunner<InvoiceProcessingContext>(emailPipelineSteps);
                _logger.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(RunPipeline), "Execution", "Email PipelineRunner created.", $"File: {filePath}", "");

                _logger.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                    $"PipelineRunner.Run for email pipeline for File: {filePath}", "ASYNC_EXPECTED"); // Log before running pipeline
                var runnerStopwatch = Stopwatch.StartNew(); // Start stopwatch for runner
                bool success = await emailPipelineRunner.Run(_context).ConfigureAwait(false);
                runnerStopwatch.Stop(); // Stop stopwatch
                _logger.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    $"PipelineRunner.Run for email pipeline for File: {filePath}", runnerStopwatch.ElapsedMilliseconds, "If ASYNC_EXPECTED, this is pre-await return"); // Log after running pipeline

                pipelineStopwatch.Stop(); // Stop pipeline stopwatch
                return LogPipelineResult(success, filePath, pipelineStopwatch.ElapsedMilliseconds); // Pass duration
            }
            catch (Exception ex)
            {
                pipelineStopwatch.Stop(); // Stop pipeline stopwatch on error
                _logger.Error(ex, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                    nameof(CreateEmailPipeline), "Pipeline Execution", pipelineStopwatch.ElapsedMilliseconds, $"Unexpected error during email pipeline execution for File: {filePath}. Error: {ex.Message}");
                return false; // Indicate pipeline failure
            }
        }

        // Modified to accept FilePath and duration for logging context
        private bool LogPipelineResult(bool success, string filePath, long durationMs) // Use instance logger, add duration
        {
             // Replace Console.WriteLine with Serilog Information/Warning log
             if (success)
             {
                 _logger.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                     nameof(CreateEmailPipeline), $"Email pipeline finished successfully for File: {filePath}.", durationMs);
             }
             else
             {
                 _logger.Warning("ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                     nameof(CreateEmailPipeline), "Pipeline Completion", durationMs, $"Email pipeline finished with failures for File: {filePath}.");
             }

            return success; // Indicate if the email pipeline completed successfully
        }

        private List<IPipelineStep<InvoiceProcessingContext>> CreateEmailPipelineSteps() // Use instance logger
        {
             _logger.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                 nameof(CreateEmailPipelineSteps), "Processing", "Creating email pipeline steps.", "", "");
            // Define the email creation pipeline steps
            var steps = new List<IPipelineStep<InvoiceProcessingContext>>
            {
                new ConstructEmailBodyStep(), // Need to instrument this step
                new SendEmailStep() // Need to instrument this step
                // Consider adding more steps here if needed for email processing
            };
             _logger.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                 nameof(CreateEmailPipelineSteps), "Completion", "Finished creating email pipeline steps.", $"Count: {steps.Count}", "");
            return steps;
        }
    }
}