using System;
using System.Text;
using System.Threading.Tasks;

namespace WaterNut.DataSpace.PipelineInfrastructure
{

    public partial class GetPdfTextStep
    {
        private static void AppendPdfTextResults(InvoiceProcessingContext context, StringBuilder pdftxt, string filePath, // Add filePath parameter
            Task<string> ripTask, Task<string> singleColumnTask, Task<string> sparseTextTask, bool logErrors = false)
        {
            // string filePath = context?.FilePath ?? "Unknown"; // filePath is now a parameter
            context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): Appending PDF text results. LogErrors: {LogErrors}", nameof(AppendPdfTextResults), "StartAppend", logErrors); // Use logger from context
 
             // Helper to safely get result or log error
             Action<Task<string>, string> appendResult = (task, taskName) =>
             {
                 string result = string.Empty;
                 try
                 {
                     if (task != null)
                     {
                         if (task.IsCompleted)
                         {
                             result = task.Result;
                             context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): Successfully retrieved result from {TaskName} task for File: {FilePath}. Length: {Length}", // Use logger from context
                                 nameof(AppendPdfTextResults), $"Append{taskName.Replace(" ", "")}", taskName, filePath, result.Length);
                         }
                         else if (task.IsFaulted && logErrors)
                         {
                             // Log the specific error from the faulted task
                             context.Logger?.Error(task.Exception?.GetBaseException(), // Use logger from context
                                 "INTERNAL_STEP ({OperationName} - {Stage}): Error recorded in faulted {TaskName} task for File: {FilePath}", nameof(AppendPdfTextResults), $"Append{taskName.Replace(" ", "")}Error", taskName, filePath);
                             result = $"<{taskName} Task Failed - See Logs>"; // Placeholder in output
                         }
                         else if (logErrors) // Task might be null or in another state
                         {
                             context.Logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {TaskName} task was null or not completed successfully when appending results for File: {FilePath}. Status: {Status}", // Use logger from context
                                 nameof(AppendPdfTextResults), $"Append{taskName.Replace(" ", "")}Warning", taskName, filePath, task?.Status.ToString() ?? "Null");
                             result = $"<{taskName} Task Not Completed Successfully>"; // Placeholder
                         }
                         // If !logErrors, we assume Task.WhenAll succeeded, so access Result directly (original behavior)
                         else if (task != null)
                         {
                             result = task.Result; // May throw if task faulted, but shouldn't happen if !logErrors
                             context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): Retrieved result from {TaskName} task for File: {FilePath}. Length: {Length}", // Use logger from context
                                 nameof(AppendPdfTextResults), $"Append{taskName.Replace(" ", "")}", taskName, filePath, result.Length);
                         }
                     }
                     else if (logErrors)
                     {
                         context.Logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {TaskName} task was null when attempting to append results for File: {FilePath}", // Use logger from context
                             nameof(AppendPdfTextResults), $"Append{taskName.Replace(" ", "")}Warning", taskName, filePath);
                         result = $"<{taskName} Task Was Null>"; // Placeholder
                     }
                 }
                 catch (Exception ex) when (logErrors) // Catch unexpected errors during Result access if logging errors
                 {
                     context.Logger?.Error(ex, "INTERNAL_STEP ({OperationName} - {Stage}): Unexpected error retrieving result from {TaskName} task for File: {FilePath}", // Use logger from context
                         nameof(AppendPdfTextResults), $"Append{taskName.Replace(" ", "")}Error", taskName, filePath);
                     result = $"<Error Retrieving {taskName} Result - See Logs>"; // Placeholder
                 }
 
                 pdftxt.AppendLine(result);
             };
 
             // Append results safely
             appendResult(singleColumnTask, "Single Column OCR");
             appendResult(sparseTextTask, "Sparse Text OCR");
             appendResult(ripTask, "Ripped Text (PdfPig)");
 
             context.PdfText = pdftxt;
             context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): Final PdfText assigned to context for File: {FilePath}. Total Length: {Length}", nameof(AppendPdfTextResults), "FinalAssignment", filePath, // Use logger from context
                 context.PdfText.Length);
         }
     }
 }