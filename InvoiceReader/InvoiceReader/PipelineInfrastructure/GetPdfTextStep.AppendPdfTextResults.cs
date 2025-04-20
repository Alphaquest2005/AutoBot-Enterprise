using System.Text;

namespace WaterNut.DataSpace.PipelineInfrastructure;

public partial class GetPdfTextStep
{
    private static void AppendPdfTextResults(InvoiceProcessingContext context, StringBuilder pdftxt,
        Task<string> ripTask, Task<string> singleColumnTask, Task<string> sparseTextTask, bool logErrors = false)
    {
        string filePath = context?.FilePath ?? "Unknown";
        _logger.Debug("Appending PDF text results. LogErrors: {LogErrors}", logErrors);

        // Helper to safely get result or log error
        Action<Task<string>, string> appendResult = (task, taskName) =>
        {
            string result = string.Empty;
            try
            {
                if (task != null)
                {
                    if (task.IsCompletedSuccessfully)
                    {
                        result = task.Result;
                        _logger.Verbose(
                            "Successfully retrieved result from {TaskName} task for File: {FilePath}. Length: {Length}",
                            taskName, filePath, result.Length);
                    }
                    else if (task.IsFaulted && logErrors)
                    {
                        // Log the specific error from the faulted task
                        _logger.Error(task.Exception?.GetBaseException(),
                            "Error recorded in faulted {TaskName} task for File: {FilePath}", taskName, filePath);
                        result = $"<{taskName} Task Failed - See Logs>"; // Placeholder in output
                    }
                    else if (logErrors) // Task might be null or in another state
                    {
                        _logger.Warning(
                            "{TaskName} task was null or not completed successfully when appending results for File: {FilePath}. Status: {Status}",
                            taskName, filePath, task?.Status.ToString() ?? "Null");
                        result = $"<{taskName} Task Not Completed Successfully>"; // Placeholder
                    }
                    // If !logErrors, we assume Task.WhenAll succeeded, so access Result directly (original behavior)
                    else if (task != null)
                    {
                        result = task.Result; // May throw if task faulted, but shouldn't happen if !logErrors
                        _logger.Verbose("Retrieved result from {TaskName} task for File: {FilePath}. Length: {Length}",
                            taskName, filePath, result.Length);
                    }
                }
                else if (logErrors)
                {
                    _logger.Warning("{TaskName} task was null when attempting to append results for File: {FilePath}",
                        taskName, filePath);
                    result = $"<{taskName} Task Was Null>"; // Placeholder
                }
            }
            catch (Exception ex) when (logErrors) // Catch unexpected errors during Result access if logging errors
            {
                _logger.Error(ex, "Unexpected error retrieving result from {TaskName} task for File: {FilePath}",
                    taskName, filePath);
                result = $"<Error Retrieving {taskName} Result - See Logs>"; // Placeholder
            }

            pdftxt.AppendLine(result);
        };

        // Append results safely
        appendResult(singleColumnTask, "Single Column OCR");
        appendResult(sparseTextTask, "Sparse Text OCR");
        appendResult(ripTask, "Ripped Text (PdfPig)");

        context.PdfText = pdftxt;
        _logger.Debug("Final PdfText assigned to context for File: {FilePath}. Total Length: {Length}", filePath,
            context.PdfText.Length);
    }
}