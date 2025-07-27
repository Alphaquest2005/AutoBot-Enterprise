// Needed for FileInfo
// Needed for List<Line>

// Assuming Line is defined here

using System.IO; // Needed for FileInfo
using System.Collections.Generic; // Needed for List<>
using System.Linq;
using System.Threading.Tasks;
using Serilog; // Add Serilog using statement
using OCR.Business.Entities; // Assuming Line is defined here
using System; // Needed for Exception

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    using System.Diagnostics;

    public class ConstructEmailBodyStep : IPipelineStep<InvoiceProcessingContext>
    {
        // Add a static logger instance for this class
        // Remove static logger instance
        // private static readonly ILogger _logger = Log.ForContext<ConstructEmailBodyStep>();

        public Task<bool> Execute(InvoiceProcessingContext context)
        {
            var methodStopwatch = Stopwatch.StartNew(); // Start stopwatch for method execution
            string filePath = context?.FilePath ?? "Unknown";
            context.Logger?.Information("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
                nameof(Execute), "Construct email body for error notification", $"FilePath: {filePath}");

            // Check if there are errors and required context properties
            if (context?.Errors == null || !context.Errors.Any() || context.FileInfo == null || string.IsNullOrEmpty(context.TextFilePath))
            {
                context.Logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(Execute), "Validation", "Skipping ConstructEmailBodyStep due to no errors or missing FileInfo/TextFilePath.",
                    $"FilePath: {filePath}, HasErrors: {context?.Errors?.Any() ?? false}, FileInfo Null: {context?.FileInfo == null}, TextFilePath Null/Empty: {string.IsNullOrEmpty(context?.TextFilePath)}",
                    "Required data for email body construction is missing.");
                methodStopwatch.Stop(); // Stop stopwatch on skip
                context.Logger?.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                    nameof(Execute), "Skipped due to missing data", $"FilePath: {filePath}", methodStopwatch.ElapsedMilliseconds);
                return Task.FromResult(false); // Required data is missing
            }

            context.Logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(Execute), "Processing", "Constructing email body.", $"FilePath: {filePath}", "");

            // Logic from the original CreateEmail method for constructing the body
            // Assuming CommandsTxt is accessible, perhaps from InvoiceProcessingUtils
            // If not, it might need to be passed in the context or a constant here.
            // For now, assuming InvoiceProcessingUtils.CommandsTxt is accessible.

            string failedLinesInfo = "No failed lines information available."; // Default message
            string firstInvoiceName = string.Empty;

            if (context.FailedLines != null && context.FailedLines.Any())
            {
                context.Logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(Execute), "FailedLinesProcessing", "Processing failed lines for email body.", $"FailedLineCount: {context.FailedLines.Count}, FilePath: {filePath}", "");
                try
                {
                    // Safely get the first invoice name
                    firstInvoiceName = context.FailedLines.FirstOrDefault()?
                                           .OCR_Lines?.Parts?.Templates?.Name ?? string.Empty;
                    if (!string.IsNullOrEmpty(firstInvoiceName))
                    {
                        context.Logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                            nameof(Execute), "FailedLinesProcessing", "Extracted first invoice name.", $"InvoiceName: {firstInvoiceName}, FilePath: {filePath}", "");
                        firstInvoiceName += "\r\n\r\n\r\n";
                    } else {
                        context.Logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                            nameof(Execute), "FailedLinesProcessing", "Could not extract first invoice name from failed lines.", $"FilePath: {filePath}", "");
                    }


                    // Construct detailed failed lines info with extensive null checks
                    // WARNING: The structure of FailedFields and its nested dynamics is assumed.
                    // This LINQ might need significant adjustment based on the actual runtime types.
                    failedLinesInfo = context.FailedLines
                        .Where(line => line != null) // Filter out null lines
                        .Select(line =>
                        {
                            var ocrLines = line.OCR_Lines;
                            var regex = ocrLines?.RegularExpressions;
                            string lineName = ocrLines?.Name ?? "N/A";
                            string regexId = regex?.Id.ToString() ?? "N/A";
                            string regexPattern = regex?.RegEx ?? "N/A";

                            string fieldsDetail = "No specific field failures"; // Default
                            try // Add inner try-catch for the complex field processing
                            {
                                if (line.FailedFields != null)
                                {
                                    // This LINQ assumes FailedFields is List<Dictionary<dynamic, List<KeyValuePair<dynamic, dynamic>>>>
                                    // and the inner KVP's Key has a 'fields' property with 'Key' and 'Field'
                                    // Corrected LINQ based on the actual type: List<Dictionary<string, List<KeyValuePair<(Fields fields, int instance), string>>>>
                                    var details = line.FailedFields
                                        .Where(dict => dict != null) // Filter out null dictionaries in the list
                                        .SelectMany(dict => dict.Values) // Get all List<KVP<(Fields, int), string>> from dictionary values
                                        .SelectMany(list => list ?? Enumerable.Empty<KeyValuePair<(Fields fields, string instance), string>>()) // Flatten the lists of KVPs, handling null lists
                                        .Where(kvp => kvp.Key.fields != null) // Ensure the 'fields' part of the tuple Key is not null
                                        .Select(kvp =>
                                        {
                                            var fieldInfo = kvp.Key.fields; // Access the Fields object from the tuple Key
                                            string fieldKey = fieldInfo.Key?.ToString() ?? "UnknownKey";
                                            string fieldName = fieldInfo.Field?.ToString() ?? "UnknownField";
                                            return $"{fieldKey} - '{fieldName}'";
                                        })
                                        .ToList(); // Materialize before Aggregate

                                    if (details.Any())
                                    {
                                        fieldsDetail = details.Aggregate((o, c) => o + ", " + c); // Join field details
                                    }
                                }
                            }
                            catch (Exception fieldEx)
                            {
                                context.Logger?.Error(fieldEx, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                                    nameof(Execute), "Process FailedFields structure", 0, $"Error processing FailedFields structure for line '{lineName}' in File: {filePath}. Error: {fieldEx.Message}");
                                fieldsDetail = "Error processing field details";
                            }


                            return $"Line:{lineName} - RegId: {regexId} - Regex: {regexPattern} - Fields: {fieldsDetail}";
                        })
                        .DefaultIfEmpty("No processable failed lines.")
                        .Aggregate((o, c) => o + "\r\n" + c);
                    context.Logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                        nameof(Execute), "FailedLinesProcessing", "Constructed failed lines details string.", $"Length: {failedLinesInfo.Length}, FilePath: {filePath}", "");
                }
                catch (Exception ex)
                {
                    context.Logger?.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                        nameof(Execute), "Construct detailed failed lines info", 0, $"Error constructing detailed failed lines info for File: {filePath}. Error: {ex.Message}");
                    failedLinesInfo = "Error occurred while processing failed lines information.";
                }
            }
            else
            {
                context.Logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                    nameof(Execute), "FailedLinesProcessing", "No failed lines found or FailedLines list is null.", $"FilePath: {filePath}", "");
            }


            context.Logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(Execute), "BodyConstruction", "Using CommandsTxt from InvoiceProcessingUtils.", $"FilePath: {filePath}", "");
            var commandsText = InvoiceProcessingUtils.CommandsTxt; // Assuming CommandsTxt is accessible here

            var errorSummary = string.Join("\r\n", context.Errors);
            var body = $"Hey,\r\n\r\n Errors encountered for '{context.FileInfo.Name}':\r\n{errorSummary}\r\n\r\n" +
                       $"{firstInvoiceName}" + // Use the safely extracted name
                       $"{failedLinesInfo}\r\n\r\n" + // Use the safely constructed details
                       "Thanks\r\n" +
                       "Thanks\r\n" + // Note: Duplicate "Thanks" line exists in original code
                       $"AutoBot\r\n" +
                       $"\r\n" +
                       $"\r\n" +
                       commandsText
                ;

            context.EmailBody = body;
            context.Logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(Execute), "BodyConstruction", "Email body constructed.", $"Length: {body.Length}, FilePath: {filePath}", "");

            // Replace Console.WriteLine with Serilog Information log
            context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(Execute), "Completion", "Constructed email body.", $"File: {filePath}", "");

            methodStopwatch.Stop(); // Stop stopwatch
            context.Logger?.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                nameof(Execute), "Email body constructed successfully", $"FilePath: {filePath}, BodyLength: {body.Length}", methodStopwatch.ElapsedMilliseconds);
            return Task.FromResult(true); // Indicate success
        }
    }
}