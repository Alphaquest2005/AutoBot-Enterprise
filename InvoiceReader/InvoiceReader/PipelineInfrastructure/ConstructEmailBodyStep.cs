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
    public class ConstructEmailBodyStep : IPipelineStep<InvoiceProcessingContext>
    {
        // Add a static logger instance for this class
        private static readonly ILogger _logger = Log.ForContext<ConstructEmailBodyStep>();

        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
             _logger.Debug("Executing ConstructEmailBodyStep for File: {FilePath}", context.FilePath);

            // Check for required context properties
            if (string.IsNullOrEmpty(context.Error) || context.FileInfo == null || string.IsNullOrEmpty(context.TxtFile))
            {
                 _logger.Warning("Skipping ConstructEmailBodyStep due to missing Error, FileInfo, or TxtFile for File: {FilePath}. Error Null/Empty: {IsErrorNull}, FileInfo Null: {IsFileInfoNull}, TxtFile Null/Empty: {IsTxtFileNull}",
                    context.FilePath, string.IsNullOrEmpty(context.Error), context.FileInfo == null, string.IsNullOrEmpty(context.TxtFile));
                return false; // Required data is missing
            }

            _logger.Debug("Constructing email body for File: {FilePath}", context.FilePath);

            // Logic from the original CreateEmail method for constructing the body
            // Assuming CommandsTxt is accessible, perhaps from InvoiceProcessingUtils
            // If not, it might need to be passed in the context or a constant here.
            // For now, assuming InvoiceProcessingUtils.CommandsTxt is accessible.

            string failedLinesInfo = "No failed lines information available."; // Default message
            string firstInvoiceName = string.Empty;

            if (context.FailedLines != null && context.FailedLines.Any())
            {
                 _logger.Verbose("Processing {FailedLineCount} failed lines for File: {FilePath}", context.FailedLines.Count, context.FilePath);
                 try
                 {
                     // Safely get the first invoice name
                     firstInvoiceName = context.FailedLines.FirstOrDefault()?
                                            .OCR_Lines?.Parts?.Invoices?.Name ?? string.Empty;
                     if (!string.IsNullOrEmpty(firstInvoiceName))
                     {
                         _logger.Verbose("Extracted first invoice name: {InvoiceName} for File: {FilePath}", firstInvoiceName, context.FilePath);
                         firstInvoiceName += "\r\n\r\n\r\n";
                     } else {
                         _logger.Verbose("Could not extract first invoice name from failed lines for File: {FilePath}", context.FilePath);
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
                                         .SelectMany(list => list ?? Enumerable.Empty<KeyValuePair<(Fields fields, int instance), string>>()) // Flatten the lists of KVPs, handling null lists
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
                                 _logger.Error(fieldEx, "Error processing FailedFields structure for line '{LineName}' in File: {FilePath}", lineName, context.FilePath);
                                 fieldsDetail = "Error processing field details";
                             }


                             return $"Line:{lineName} - RegId: {regexId} - Regex: {regexPattern} - Fields: {fieldsDetail}";
                         })
                         .DefaultIfEmpty("No processable failed lines.")
                         .Aggregate((o, c) => o + "\r\n" + c);
                     _logger.Verbose("Constructed failed lines details string (Length: {Length}) for File: {FilePath}", failedLinesInfo.Length, context.FilePath);
                 }
                 catch (Exception ex)
                 {
                     _logger.Error(ex, "Error constructing detailed failed lines info for File: {FilePath}", context.FilePath);
                     failedLinesInfo = "Error occurred while processing failed lines information.";
                 }
            }
            else
            {
                 _logger.Debug("No failed lines found or FailedLines list is null for File: {FilePath}", context.FilePath);
            }


             _logger.Verbose("Using CommandsTxt from InvoiceProcessingUtils for File: {FilePath}", context.FilePath);
            var commandsText = InvoiceProcessingUtils.CommandsTxt; // Assuming CommandsTxt is accessible here

            var body = $"Hey,\r\n\r\n {context.Error}-'{context.FileInfo.Name}'.\r\n\r\n\r\n" +
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
             _logger.Debug("Email body constructed. Length: {BodyLength} for File: {FilePath}", body.Length, context.FilePath);

             // Replace Console.WriteLine with Serilog Information log
             _logger.Information("Constructed email body for File: {FilePath}.", context.FilePath);

             _logger.Debug("Finished executing ConstructEmailBodyStep successfully for File: {FilePath}", context.FilePath);
            return true; // Indicate success
        }
    }
}