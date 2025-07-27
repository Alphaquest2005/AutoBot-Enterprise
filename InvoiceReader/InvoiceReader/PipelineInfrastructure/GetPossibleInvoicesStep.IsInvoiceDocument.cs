using System;
using System.Linq;
using System.Text.RegularExpressions;
using OCR.Business.Entities;

namespace WaterNut.DataSpace.PipelineInfrastructure{
    using System.Diagnostics;

    using Serilog;

    public partial class GetPossibleInvoicesStep
{
    public static bool IsInvoiceDocument(Template template, string fileText, string filePath, ILogger logger) // Add logger parameter
    {
        var methodStopwatch = Stopwatch.StartNew(); // Start stopwatch for method execution
        // Template null check happens in caller's Where clause
        int invoiceId = template.OcrTemplates?.Id ?? -1; // Handle null OcrInvoices defensively
        logger?.Debug("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
            nameof(IsInvoiceDocument), "Check if document matches a specific invoice template", $"InvoiceId: {invoiceId}, FilePath: {filePath}");

        logger?.Debug("ACTION_START: {ActionName}. Context: [{ActionContext}]",
            $"{nameof(IsInvoiceDocument)} - Invoice {invoiceId}", $"Checking if document matches template {invoiceId} for file: {filePath}");

        try
        {
            // Check if TemplateIdentificatonRegEx collection exists and has items
            if (template.OcrTemplates?.TemplateIdentificatonRegEx == null || !template.OcrTemplates.TemplateIdentificatonRegEx.Any())
            {
                logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}] Cannot determine if it's an invoice document based on regex.",
                    nameof(IsInvoiceDocument), "Validation", "No Template Identification Regex patterns found.", $"InvoiceId: {invoiceId}");

                methodStopwatch.Stop();
                logger?.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                    nameof(IsInvoiceDocument), "Document does not match template (no regex patterns)", $"IsMatch: false, InvoiceId: {invoiceId}", methodStopwatch.ElapsedMilliseconds);
                logger?.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                    $"{nameof(IsInvoiceDocument)} - Invoice {invoiceId}", $"Document does not match template {invoiceId} (no regex patterns) for file: {filePath}", methodStopwatch.ElapsedMilliseconds);
                return false; // Cannot match without patterns
            }

            bool isMatch = false;
            // Iterate through regex patterns, ensuring inner objects aren't null
            foreach (var regexInfo in template.OcrTemplates.TemplateIdentificatonRegEx.Where(r => r?.OCR_RegularExpressions != null))
            {
                string pattern = regexInfo.OCR_RegularExpressions.RegEx;
                int regexId = regexInfo.OCR_RegularExpressions.Id;

                if (string.IsNullOrEmpty(pattern))
                {
                    logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                        nameof(IsInvoiceDocument), "RegexProcessing", "Skipping empty regex pattern.", $"RegexId: {regexId}, InvoiceId: {invoiceId}");
                    continue; // Skip empty patterns
                }

                logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                    nameof(IsInvoiceDocument), "RegexProcessing", "Testing regex pattern against file text.", $"RegexId: {regexId}, Pattern: '{pattern}', InvoiceId: {invoiceId}");

                logger?.Debug("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                    $"Regex.IsMatch for RegexId {regexId}", "SYNC_EXPECTED");
                var regexStopwatch = Stopwatch.StartNew();
                isMatch = Regex.IsMatch(fileText,
                    pattern,
                    RegexOptions.IgnoreCase | RegexOptions.Multiline | RegexOptions.ExplicitCapture,
                    RegexTimeout); // Use defined timeout
                regexStopwatch.Stop();
                logger?.Debug("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                    $"Regex.IsMatch for RegexId {regexId}", regexStopwatch.ElapsedMilliseconds, "Sync call returned");

                if (isMatch)
                {
                    logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                        nameof(IsInvoiceDocument), "RegexProcessing", "Regex match FOUND.", $"RegexId: {regexId}, InvoiceId: {invoiceId}, FilePath: {filePath}");

                    // Check if all parts can start
                    logger?.Debug("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                        $"CanPartStart for Invoice {invoiceId}", "SYNC_EXPECTED");
                    var canPartStartStopwatch = Stopwatch.StartNew();
                    var res = template.Parts.All(x => CanPartStart(fileText, x, logger) && x.ChildParts.All(z => CanPartStart(fileText, z, logger))); // Pass logger
                    canPartStartStopwatch.Stop();
                    logger?.Debug("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                        $"CanPartStart for Invoice {invoiceId}", canPartStartStopwatch.ElapsedMilliseconds, "Sync call returned");

                    if (res)
                    {
                        logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                            nameof(IsInvoiceDocument), "PartValidation", "All parts can start.", $"InvoiceId: {invoiceId}");
                        methodStopwatch.Stop();
                        logger?.Debug("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                            nameof(IsInvoiceDocument), "Document matches template", $"IsMatch: true, InvoiceId: {invoiceId}", methodStopwatch.ElapsedMilliseconds);
                        logger?.Debug("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                            $"{nameof(IsInvoiceDocument)} - Invoice {invoiceId}", $"Document identified as template {invoiceId} for file: {filePath}", methodStopwatch.ElapsedMilliseconds);
                        return res; // Exit method on first match and successful part check
                    }
                    else
                    {
                        logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}] Template match requires all parts to start.",
                            nameof(IsInvoiceDocument), "PartValidation", "Regex matched, but not all parts can start.", $"InvoiceId: {invoiceId}");
                        // Continue checking other regex patterns if this one matched but parts didn't start
                    }
                }
                else
                {
                    logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                        nameof(IsInvoiceDocument), "RegexProcessing", "Regex did not match.", $"RegexId: {regexId}, InvoiceId: {invoiceId}");
                }
            }

            // Log only if no patterns matched after checking all of them
            logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}] Document NOT identified as this invoice type.",
                nameof(IsInvoiceDocument), "Summary", "No identifying regex patterns matched.", $"InvoiceId: {invoiceId}, FilePath: {filePath}");

            methodStopwatch.Stop();
            logger?.Information("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                nameof(IsInvoiceDocument), "Document does not match template (no regex match)", $"IsMatch: false, InvoiceId: {invoiceId}", methodStopwatch.ElapsedMilliseconds);
            logger?.Information("ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                $"{nameof(IsInvoiceDocument)} - Invoice {invoiceId}", $"Document NOT identified as template {invoiceId} for file: {filePath}", methodStopwatch.ElapsedMilliseconds);
            return false; // No match found
        }
        catch (RegexMatchTimeoutException timeoutEx)
        {
            methodStopwatch.Stop();
            // Log timeout specifically
            logger?.Error(timeoutEx, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                nameof(IsInvoiceDocument), "Check if document matches a specific invoice template", methodStopwatch.ElapsedMilliseconds, $"Regex match timed out (>{RegexTimeout.TotalSeconds}s) while checking InvoiceId: {invoiceId} for File: {filePath}.");
            logger?.Error(timeoutEx, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                $"{nameof(IsInvoiceDocument)} - Invoice {invoiceId}", "Regex matching timeout", methodStopwatch.ElapsedMilliseconds, $"Regex match timed out (>{RegexTimeout.TotalSeconds}s) while checking InvoiceId: {invoiceId} for File: {filePath}.");
            return false; // Treat timeout as non-match
        }
        catch (Exception ex)
        {
            methodStopwatch.Stop();
            // Log any other exceptions during regex processing
            logger?.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                nameof(IsInvoiceDocument), "Check if document matches a specific invoice template", methodStopwatch.ElapsedMilliseconds, $"Error during regex matching process for InvoiceId: {invoiceId} for File: {filePath}. Error: {ex.Message}");
            logger?.Error(ex, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                $"{nameof(IsInvoiceDocument)} - Invoice {invoiceId}", "Unexpected error during regex matching", methodStopwatch.ElapsedMilliseconds, $"Error during regex matching process for InvoiceId: {invoiceId} for File: {filePath}. Error: {ex.Message}");
            return false; // Treat error as non-match
        }
    }

    private static bool CanPartStart(string fileText, Part x, ILogger logger) // Added logger parameter
    {
        var methodStopwatch = Stopwatch.StartNew(); // Start stopwatch
        int partId = x?.OCR_Part?.Id ?? -1; // Handle nulls defensively
        logger?.Debug("METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
            nameof(CanPartStart), "Check if all start patterns for a part are found in the document text", $"PartId: {partId}");

        try
        {
            if (x?.OCR_Part?.Start == null || !x.OCR_Part.Start.Any())
            {
                logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}] Cannot check part start without patterns.",
                    nameof(CanPartStart), "Validation", "No start patterns found for part.", $"PartId: {partId}");

                methodStopwatch.Stop(); // Stop stopwatch
                logger?.Debug("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                    nameof(CanPartStart), "Part cannot start (no patterns)", $"CanStart: false, PartId: {partId}", methodStopwatch.ElapsedMilliseconds);
                return false; // Cannot check without patterns
            }

            bool canStart = x.OCR_Part.Start.All(
                startRegexInfo =>
                {
                    if (startRegexInfo?.RegularExpressions == null || string.IsNullOrEmpty(startRegexInfo.RegularExpressions.RegEx))
                    {
                        logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                            nameof(CanPartStart), "RegexProcessing", "Skipping empty start regex pattern for part.", $"PartId: {partId}");
                        return false; // Treat empty pattern as non-match for All()
                    }

                    string pattern = startRegexInfo.RegularExpressions.RegEx;
                    int regexId = startRegexInfo.RegularExpressions.Id;

                    logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                        nameof(CanPartStart), "RegexProcessing", "Testing start regex pattern against file text.", $"PartId: {partId}, RegexId: {regexId}, Pattern: '{pattern}'");

                    logger?.Debug("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                        $"Regex.IsMatch for Part {partId} Start RegexId {regexId}", "SYNC_EXPECTED");
                    var regexStopwatch = Stopwatch.StartNew();
                    bool isMatch = Regex.IsMatch(
                        fileText,
                        pattern,
                        RegexOptions.IgnoreCase | RegexOptions.Singleline | RegexOptions.Multiline | RegexOptions.ExplicitCapture,
                        RegexTimeout);
                    regexStopwatch.Stop();
                    logger?.Debug("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                        $"Regex.IsMatch for Part {partId} Start RegexId {regexId}", regexStopwatch.ElapsedMilliseconds, "Sync call returned");

                    logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                        nameof(CanPartStart), "RegexProcessing", "Start regex match result.", $"PartId: {partId}, RegexId: {regexId}, IsMatch: {isMatch}");

                    return isMatch;
                });

            methodStopwatch.Stop(); // Stop stopwatch
            logger?.Debug("METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                nameof(CanPartStart), $"Part can start: {canStart}", $"CanStart: {canStart}, PartId: {partId}", methodStopwatch.ElapsedMilliseconds);
            return canStart;
        }
        catch (RegexMatchTimeoutException timeoutEx)
        {
            methodStopwatch.Stop(); // Stop stopwatch
            logger?.Error(timeoutEx, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                nameof(CanPartStart), "Check if all start patterns for a part are found in the document text", methodStopwatch.ElapsedMilliseconds, $"Regex match timed out (>{RegexTimeout.TotalSeconds}s) while checking PartId: {partId}.");
            return false; // Treat timeout as non-match
        }
        catch (Exception ex)
        {
            methodStopwatch.Stop(); // Stop stopwatch
            logger?.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                nameof(CanPartStart), "Check if all start patterns for a part are found in the document text", methodStopwatch.ElapsedMilliseconds, $"Error during regex matching process for PartId: {partId}. Error: {ex.Message}");
            return false; // Treat error as non-match
        }
    }
}
}