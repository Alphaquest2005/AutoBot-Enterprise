using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using OCR.Business.Entities;

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public static partial class InvoiceProcessingUtils
    {
        private static string CreateEmailBody(string error, List<Line> failedlst, FileInfo fileInfo)
        {
            string fileName = fileInfo?.Name ?? "UnknownFile";
            _utilsLogger.Debug("Creating email body for File: {FileName}", fileName);

            string failedLinesInfo = "No failed lines information available."; // Default message
            string firstInvoiceName = string.Empty;

            if (failedlst != null && failedlst.Any())
            {
                _utilsLogger.Verbose("Processing {FailedLineCount} failed lines for File: {FileName}", failedlst.Count,
                    fileName);
                try
                {
                    // Safely get the first invoice name
                    firstInvoiceName = failedlst.FirstOrDefault()?
                        .OCR_Lines?.Parts?.Templates?.Name ?? string.Empty;
                    if (!string.IsNullOrEmpty(firstInvoiceName))
                    {
                        _utilsLogger.Verbose("Extracted first invoice name: {InvoiceName} for File: {FileName}",
                            firstInvoiceName, fileName);
                        firstInvoiceName += "\r\n\r\n\r\n";
                    }
                    else
                    {
                        _utilsLogger.Verbose(
                            "Could not extract first invoice name from failed lines for File: {FileName}",
                            fileName);
                    }

                    // Construct detailed failed lines info using the corrected LINQ
                    failedLinesInfo = failedlst
                        .Where(line => line != null)
                        .Select(line =>
                        {
                            var ocrLines = line.OCR_Lines;
                            var regex = ocrLines?.RegularExpressions;
                            string lineName = ocrLines?.Name ?? "N/A";
                            string regexId = regex?.Id.ToString() ?? "N/A";
                            string regexPattern = regex?.RegEx ?? "N/A";

                            string fieldsDetail = "No specific field failures";
                            try
                            {
                                if (line.FailedFields != null)
                                {
                                    // Corrected LINQ based on the actual type: List<Dictionary<string, List<KeyValuePair<(Fields fields, int instance), string>>>>
                                    var details = line.FailedFields
                                        .Where(dict => dict != null)
                                        .SelectMany(dict => dict.Values)
                                        .SelectMany(list =>
                                            list ?? Enumerable
                                                .Empty<KeyValuePair<(Fields fields, string instance), string>>())
                                        .Where(kvp => kvp.Key.fields != null)
                                        .Select(kvp =>
                                        {
                                            var fieldInfo = kvp.Key.fields;
                                            string fieldKey = fieldInfo.Key?.ToString() ?? "UnknownKey";
                                            string fieldName = fieldInfo.Field?.ToString() ?? "UnknownField";
                                            return $"{fieldKey} - '{fieldName}'";
                                        })
                                        .ToList();

                                    if (details.Any())
                                    {
                                        fieldsDetail = details.Aggregate((o, c) => o + ", " + c);
                                    }
                                }
                            }
                            catch (Exception fieldEx)
                            {
                                _utilsLogger.Error(fieldEx,
                                    "Error processing FailedFields structure for line '{LineName}' in File: {FileName}",
                                    lineName, fileName);
                                fieldsDetail = "Error processing field details";
                            }

                            return
                                $"Line:{lineName} - RegId: {regexId} - Regex: {regexPattern} - Fields: {fieldsDetail}";
                        })
                        .DefaultIfEmpty("No processable failed lines.")
                        .Aggregate((o, c) => o + "\r\n" + c);
                    _utilsLogger.Verbose(
                        "Constructed failed lines details string (Length: {Length}) for File: {FileName}",
                        failedLinesInfo.Length, fileName);
                }
                catch (Exception ex)
                {
                    _utilsLogger.Error(ex, "Error constructing detailed failed lines info for File: {FileName}",
                        fileName);
                    failedLinesInfo = "Error occurred while processing failed lines information.";
                }
            }
            else
            {
                _utilsLogger.Debug("No failed lines found or FailedLines list is null for File: {FileName}", fileName);
            }

            _utilsLogger.Verbose("Using CommandsTxt from InvoiceProcessingUtils for File: {FileName}", fileName);
            var commandsText = CommandsTxt; // Assuming CommandsTxt is accessible here

            var body = $"Hey,\r\n\r\n {error}-'{fileName}'.\r\n\r\n\r\n" +
                       $"{firstInvoiceName}" +
                       $"{failedLinesInfo}\r\n\r\n" +
                       "Thanks\r\n" +
                       "Thanks\r\n" +
                       $"AutoBot\r\n" +
                       $"\r\n" +
                       $"\r\n" +
                       commandsText;

            _utilsLogger.Debug("Finished creating email body for File: {FileName}. Length: {BodyLength}", fileName,
                body.Length);
            return body;
        }
    }
}