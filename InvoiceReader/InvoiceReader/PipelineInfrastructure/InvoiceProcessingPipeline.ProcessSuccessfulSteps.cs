using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public partial class InvoiceProcessingPipeline
    {
        private async Task<bool> ProcessSuccessfulSteps()
        {
            string filePath = _context?.FilePath ?? "Unknown";
            _logger.Information("Starting ProcessSuccessfulSteps for File: {FilePath}", filePath);
            // Replace Console.WriteLine

            try
            {
                // Success handling pipeline
                _logger.Debug("Creating success pipeline steps for File: {FilePath}", filePath);
                var successSteps = new List<IPipelineStep<InvoiceProcessingContext>>
                {
                    new AddNameSupplierStep(),
                    new AddMissingRequiredFieldValuesStep(),
                    new WriteFormattedTextFileStep(), // Assuming this step exists
                    new HandleImportSuccessStateStep(),
                    new UpdateImportStatusStep() // Update status after success handling
                };
                _logger.Debug("Success pipeline steps created. Count: {Count}", successSteps.Count);

                _logger.Information("Running success pipeline steps for File: {FilePath}", filePath);
                bool allStepsSucceeded = true; // Track success across all steps in this sequence
                int stepCounter = 0;

                foreach (var step in successSteps)
                {
                    stepCounter++;
                    var stepName = step.GetType().Name;
                    try
                    {
                        _logger.Debug("Executing success step {StepNumber}: {StepName} for File: {FilePath}", stepCounter, stepName, filePath);
                        
                        // Correctly await the Execute method from IPipelineStep
                        bool stepResult = await step.Execute(_context).ConfigureAwait(false);

                        if (!stepResult)
                        {
                            // Step explicitly returned false, indicating failure
                            string errorMsg = $"Success step {stepCounter} ({stepName}) reported failure (returned false) for File: {filePath}.";
                            _logger.Warning(errorMsg); // Log the failure
                            _context.AddError(errorMsg); // Add error to context
                            allStepsSucceeded = false; // Mark failure
                            break; // Stop processing further success steps
                        }
                        _logger.Information("Success step {StepNumber} ({StepName}) completed successfully for File: {FilePath}", stepCounter, stepName, filePath);
                    }
                    catch (Exception stepEx) // Catch exceptions during step execution
                    {
                        string errorMsg = $"Error during success step {stepCounter} ({stepName}) for File: {filePath}: {stepEx.Message}";
                        _logger.Error(stepEx, errorMsg); // Log the error with exception details
                        _context.AddError(errorMsg); // Add error to context
                        allStepsSucceeded = false; // Mark failure
                        break; // Stop processing further success steps on error
                    }
                }
                _logger.Information("Success pipeline steps finished processing for File: {FilePath}. All steps succeeded: {AllStepsSucceeded}", filePath, allStepsSucceeded);

                // Determine overall success based on step execution success AND final import status
                // ImportStatus might be HasErrors even if steps succeeded, which is still considered a form of overall success for the pipeline run.
                bool overallSuccess = allStepsSucceeded && (_context.ImportStatus == ImportStatus.Success ||
                                          _context.ImportStatus == ImportStatus.HasErrors); // Adjusted status check based on typical usage
                _logger.Information(
                    "Final ImportStatus after success steps: {ImportStatus}. Overall Pipeline Success considered: {OverallSuccess}",
                    _context.ImportStatus, overallSuccess);
                return overallSuccess; // Return the calculated overall success
            }
            catch (Exception ex)
            {
                // Catch unexpected errors in the ProcessSuccessfulSteps method itself
                string errorMsg = $"Unexpected error during ProcessSuccessfulSteps for File: {filePath}: {ex.Message}";
                _logger.Error(ex, errorMsg);
                _context.AddError(errorMsg); // Add error to context
                return false; // Indicate failure
            }
        }
    }
}