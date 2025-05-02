using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public partial class InvoiceProcessingPipeline
    {
        private async Task<bool> ProcessErrorPipeline()
        {
            string filePath = _context?.FilePath ?? "Unknown";
            _logger.Information("Starting ProcessErrorPipeline for File: {FilePath}", filePath);
            // Replace Console.WriteLine

            try
            {
                // Error handling pipeline
                _logger.Debug("Creating error pipeline steps for File: {FilePath}", filePath);
                var errorSteps = new List<IPipelineStep<InvoiceProcessingContext>>
                {
                    new HandleErrorStateStep(_isLastTemplate), // Handles email logic
                    new UpdateImportStatusStep() // Update status after error handling
                };
                _logger.Debug("Error pipeline steps created. Count: {Count}", errorSteps.Count);

                _logger.Information("Running error pipeline steps for File: {FilePath}", filePath);
                bool allStepsAttemptedSuccessfully = true; // Track if steps executed without *new* issues
                int stepCounter = 0;

                foreach (var step in errorSteps)
                {
                    stepCounter++;
                    var stepName = step.GetType().Name;
                    try
                    {
                        _logger.Debug("Executing error step {StepNumber}: {StepName} for File: {FilePath}", stepCounter, stepName, filePath);
                        
                        // Correctly await the Execute method from IPipelineStep
                        bool stepResult = await step.Execute(_context).ConfigureAwait(false);

                        if (!stepResult)
                        {
                            // Step explicitly returned false, indicating failure within the error handling itself
                            string errorMsg = $"Error step {stepCounter} ({stepName}) reported failure (returned false) for File: {filePath}.";
                            _logger.Warning(errorMsg); // Log the failure
                            _context.AddError(errorMsg); // Add this *new* error to context
                            allStepsAttemptedSuccessfully = false; // Mark that an error step itself failed
                            // Do NOT break - allow subsequent error steps to run if possible
                        }
                        else
                        {
                             _logger.Information("Error step {StepNumber} ({StepName}) completed successfully for File: {FilePath}", stepCounter, stepName, filePath);
                        }
                    }
                    catch (Exception stepEx) // Catch exceptions during error step execution
                    {
                        string errorMsg = $"Error during error step {stepCounter} ({stepName}) for File: {filePath}: {stepEx.Message}";
                        _logger.Error(stepEx, errorMsg); // Log the error with exception details
                        _context.AddError(errorMsg); // Add this *new* error to context
                        allStepsAttemptedSuccessfully = false; // Mark that an error step itself failed
                        // Do NOT break - allow subsequent error steps to run if possible
                    }
                }
                _logger.Information("Error pipeline steps finished processing for File: {FilePath}. All steps attempted successfully: {AllStepsAttemptedSuccessfully}", filePath, allStepsAttemptedSuccessfully);

                // The return value indicates if the *overall* pipeline run should be considered successful enough
                // to potentially proceed with other actions outside the pipeline, despite the initial error.
                // This logic might depend on the final ImportStatus set by the error steps.
                // For now, we assume that if the error pipeline ran (even with internal failures), the overall result is still 'failed'.
                // The context.Errors list contains the details.
                bool overallPipelineSuccess = false; // Error path always results in overall failure
                 _logger.Information(
                    "Final ImportStatus after error steps: {ImportStatus}. Overall Pipeline Success considered: {OverallPipelineSuccess}",
                    _context.ImportStatus, overallPipelineSuccess);
                return overallPipelineSuccess; // Return false as the initial steps failed
            }
            catch (Exception ex)
            {
                // Catch unexpected errors in the ProcessErrorPipeline method itself
                string errorMsg = $"Unexpected error during ProcessErrorPipeline for File: {filePath}: {ex.Message}";
                _logger.Error(ex, errorMsg);
                _context.AddError(errorMsg); // Add this *new* error to context
                return false; // Indicate overall pipeline failure
            }
        }
    }
}