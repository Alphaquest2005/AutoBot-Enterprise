using System.Collections.Generic; // Added
using System.Linq; // Added
using System.Threading.Tasks; // Added
using Serilog; // Added
using System; // Added

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public class PipelineRunner<TContext>
    {
        // Add a static logger instance for this generic class
        private static readonly ILogger _logger = Log.ForContext<PipelineRunner<TContext>>();

        private readonly IReadOnlyList<IPipelineStep<TContext>> _steps; // Use IReadOnlyList for count and indexer
        private readonly string _pipelineName; // Optional: Add a name for better logging

        // Modified constructor to accept an optional pipeline name and materialize steps
        public PipelineRunner(IEnumerable<IPipelineStep<TContext>> steps, string pipelineName = "Unnamed Pipeline")
        {
            // Use ToList() to materialize the steps, prevent multiple enumerations, and get an accurate count.
            _steps = steps?.ToList() ?? new List<IPipelineStep<TContext>>();
            _pipelineName = pipelineName;
             _logger.Debug("{PipelineName} runner initialized with {StepCount} steps.", _pipelineName, _steps.Count);
             if (_steps.Count == 0) // Use Count property after ToList()
             {
                 _logger.Warning("{PipelineName} runner initialized with zero steps.", _pipelineName);
             }
        }

        public async Task<bool> Run(TContext context)
        {
             // Try to get a meaningful identifier from the context if possible
             // For now, logging generic start/end.
             _logger.Information("Starting execution of {PipelineName}.", _pipelineName);
             int stepCounter = 0;
             bool continuePipeline = true;

             // Null check context
             if (context == null)
             {
                  _logger.Error("{PipelineName} cannot run with a null context.", _pipelineName);
                  return false;
             }

             _logger.Verbose("Entering steps loop for {PipelineName}.", _pipelineName); // Add log before loop
             foreach (var step in _steps)
             {
                 _logger.Verbose("Start of loop iteration {StepNumber} in {PipelineName}.", stepCounter + 1, _pipelineName); // Log start of iteration
                 stepCounter++;
                 _logger.Verbose("Incremented stepCounter to {StepNumber}.", stepCounter); // Log counter increment

                 // Simplify logging first to rule out issues
                 _logger.Verbose("Attempting to get step name for step {StepNumber} in {PipelineName}.", stepCounter, _pipelineName); // Log before getting name
                 string stepName = step?.GetType().Name ?? $"Unnamed Step {stepCounter}"; // Get step name safely
                 _logger.Verbose("Got step name: {StepName}.", stepName); // Log after getting name

                 if (step == null)
                 {
                      _logger.Warning("Skipping null step at position {StepNumber} in {PipelineName}.", stepCounter, _pipelineName);
                      continue; // Skip null steps
                 }

                 _logger.Verbose("Step is not null. Proceeding to try block for {StepName}.", stepName); // Log before try block
                 try
                 {
                     _logger.Verbose("Inside TRY block for step {StepName} in {PipelineName}.", stepName, _pipelineName); // Add log inside try
                     // Individual steps should handle their own detailed logging
                     _logger.Verbose("Executing step {StepName}...", stepName); // Log before execution
                     bool stepResult = await step.Execute(context).ConfigureAwait(false);
                     _logger.Verbose("Step {StepName} execution completed with result: {StepResult}.", stepName, stepResult); // Log after execution

                     if (!stepResult)

                     if (!stepResult)
                     {
                          _logger.Warning("Step {StepNumber} ({StepName}) in {PipelineName} returned false. Stopping pipeline execution.",
                             stepCounter, stepName, _pipelineName);
                         continuePipeline = false;
                         break; // Stop the pipeline if a step returns false
                     }
                     else
                     {
                          _logger.Information("Step {StepNumber} ({StepName}) in {PipelineName} completed successfully.",
                             stepCounter, stepName, _pipelineName);
                     }
                 }
                 catch (Exception ex)
                 {
                      _logger.Error(ex, "Error executing Step {StepNumber} ({StepName}) in {PipelineName}. Stopping pipeline execution.",
                         stepCounter, stepName, _pipelineName);
                      continuePipeline = false;
                      break; // Stop pipeline on unhandled exception in a step
                 }
             }

             if (continuePipeline)
             {
                  _logger.Information("{PipelineName} completed all {StepCount} steps successfully.", _pipelineName, _steps.Count); // Use Count property
             } else {
                  _logger.Warning("{PipelineName} execution stopped prematurely after Step {StepNumber}.", _pipelineName, stepCounter);
             }

             return continuePipeline; // Return true if all steps succeeded, false otherwise
        }
    }
}
