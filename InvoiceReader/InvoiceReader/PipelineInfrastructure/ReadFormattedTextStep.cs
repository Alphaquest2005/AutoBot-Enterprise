// Assuming this is needed for _template.Read

using System.Collections.Generic; // Added
using System.Linq; // Added
using System.Threading.Tasks; // Added
using Serilog; // Added
using Serilog.Events; // Added for LogEventLevel
using System;
using System.Runtime.InteropServices; // Added
using OCR.Business.Entities; // Added for Template
using Core.Common.Extensions; // Added for BetterExpando
using WaterNut.DataSpace; // Added for functional pipeline extensions
using System.Text.Json; // Added for template context serialization

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    using System.Diagnostics;

    using CoreEntities.Business.Entities;

    using WaterNut.Business.Services.Utils;

    public partial class ReadFormattedTextStep : IPipelineStep<InvoiceProcessingContext>
    {
        // Remove static logger
        // private static readonly ILogger _logger = Log.ForContext<ReadFormattedTextStep>();

        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            using (LogLevelOverride.Begin(LogEventLevel.Verbose)) // ENABLED TO SEE TEMPLATE RELOAD DEBUGGING
            {
                var methodStopwatch = Stopwatch.StartNew(); // Start stopwatch for method execution
                string filePath = context?.FilePath ?? "Unknown";
                context.Logger?.Information(
                    "METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
                    nameof(Execute),
                    "Read formatted PDF text based on template structure",
                    $"FilePath: {filePath}");

                context.Logger?.Information(
                    "ACTION_START: {ActionName}. Context: [{ActionContext}]",
                    nameof(ReadFormattedTextStep),
                    $"Reading formatted text for file: {filePath}");


                if (!context.MatchedTemplates.Any())
                {
                    context.Logger?.Warning(
                        "INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                        nameof(Execute),
                        "Validation",
                        "Skipping ReadFormattedTextStep: No Templates found in context.",
                        $"FilePath: {filePath}",
                        "Expected templates for reading.");
                    // Not necessarily an error, but nothing to process. Consider if this should be true or false based on pipeline logic.
                    // Returning true as no processing *failed*, just skipped.
                    methodStopwatch.Stop(); // Stop stopwatch on skip
                    context.Logger?.Information(
                        "METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                        nameof(Execute),
                        "Skipped due to no templates",
                        $"FilePath: {filePath}",
                        methodStopwatch.ElapsedMilliseconds);
                    context.Logger?.Information(
                        "ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                        nameof(ReadFormattedTextStep),
                        $"Skipped reading formatted text for file: {filePath} (no templates)",
                        methodStopwatch.ElapsedMilliseconds);
                    return true;
                }

                bool overallSuccess = true; // Track if at least one template was read successfully

                var templatesList = context.MatchedTemplates.ToList();
                for (int templateIndex = 0; templateIndex < templatesList.Count; templateIndex++)
                {
                    var template = templatesList[templateIndex];
                    int? templateId = template?.OcrInvoices?.Id; // Get template ID safely
                    string templateName = template?.OcrInvoices?.Name ?? "Unknown";

                    try
                    {
                        // --- Validation ---
                        if (!ExecutionValidation(context.Logger, template, filePath)) // Pass logger
                        {
                            // ExecutionValidation logs the specific reason
                            string errorMsg =
                                $"Validation failed for TemplateId: {templateId} in ReadFormattedTextStep for File: {filePath}.";
                            context.AddError(errorMsg); // Add error to context
                            overallSuccess = false; // Mark that this template failed
                            context.AddError(errorMsg); // Add error to context
                            methodStopwatch.Stop(); // Stop stopwatch immediately
                            context.Logger?.Error(
                                "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                                nameof(Execute),
                                "Read formatted PDF text based on template structure",
                                methodStopwatch.ElapsedMilliseconds,
                                "Validation failed for a template. Terminating early.");
                            context.Logger?.Error(
                                "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                                nameof(ReadFormattedTextStep),
                                "Validation",
                                methodStopwatch.ElapsedMilliseconds,
                                "Validation failed for a template. Terminating early.");
                            continue;
                        }
                        // --- End Validation ---

                        var textLines = GetTextLinesFromFormattedPdfText(
                            context.Logger,
                            template,
                            filePath); // Pass logger

                        // --- Template Read Execution ---
                        try
                        {
                            LogCallingTemplateRead(
                                context.Logger,
                                textLines.Count,
                                filePath,
                                templateId); // Pass logger
                            context.Logger?.Information(
                                "INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                                $"Template.Read for Template {templateId}",
                                "SYNC_EXPECTED"); // Log before Read call
                            var readStopwatch = Stopwatch.StartNew(); // Start stopwatch

                            context.Logger?.Verbose(
                                "Template Parts for TemplateId: {TemplateId}: {@Parts}",
                                templateId,
                                template.OcrInvoices?.Parts); // Log template parts
                            context.Logger?.Verbose(
                                "Template RegEx for TemplateId: {TemplateId}: {@RegEx}",
                                templateId,
                                template.OcrInvoices?.RegEx); // Log template regex

                            context.Logger?.Verbose(
                                "Calling template.Read() for TemplateId: {TemplateId}. Input textLines: {@TextLines}",
                                templateId,
                                textLines); // Log input textLines

                            template.CsvLines = template.Read(textLines); // The core operation

                            // --- Resolve File Type ---
                            context.Logger?.Debug(
                                "INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                                nameof(Execute),
                                "FileTypeResolution",
                                "Resolving file type.",
                                $"FilePath: {filePath}, TemplateId: {templateId}",
                                "");
                            FileTypes fileType =
                                HandleImportSuccessStateStep.ResolveFileType(
                                    context.Logger,
                                    template); // Handles its own logging, pass logger
                            if (fileType == null)
                            {
                                string errorMsg =
                                    $"ResolveFileType returned null for File: {filePath}, TemplateId: {templateId}. Cannot proceed.";
                                context.Logger?.Error(
                                    "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                                    nameof(Execute),
                                    "Resolve file type",
                                    0,
                                    errorMsg);
                                context.Logger?.Error(
                                    "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                                    $"{nameof(HandleImportSuccessStateStep)} - Template {templateId}",
                                    "File type resolution",
                                    0,
                                    errorMsg);
                                context.AddError(errorMsg); // Add error to context
                                continue;
                            }

                            context.Logger?.Information(
                                "INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                                nameof(Execute),
                                "FileTypeResolution",
                                "Resolved FileType.",
                                $"FileTypeId: {fileType.Id}, FilePath: {filePath}",
                                "");
                            // --- End Resolve File Type ---

                            template.FileType = fileType;

                            if (template.FileType.FileImporterInfos.EntryType
                                == FileTypeManager.EntryTypes.ShipmentInvoice)
                            {
                                var res = template.CsvLines;

                                // Log the initial CsvLines result for totals calculation debugging
                                context.Logger?.Information(
                                    "OCR_CORRECTION_DEBUG: Initial CsvLines result from template.Read()");
                                if (res != null)
                                {
                                    context.Logger?.Information(
                                        "OCR_CORRECTION_DEBUG: Initial CsvLines Count: {Count}",
                                        res.Count);
                                    for (int i = 0; i < res.Count; i++)
                                    {
                                        context.Logger?.Information(
                                            "OCR_CORRECTION_DEBUG: Initial CsvLine[{Index}]: {@CsvLineData}",
                                            i,
                                            res[i]);
                                    }
                                }
                                else
                                {
                                    context.Logger?.Warning("OCR_CORRECTION_DEBUG: Initial CsvLines is null");
                                }

                                // === OCR CORRECTION SECTION ENTRY ===
                                context.Logger?.Error("🔍 **OCR_CORRECTION_ENTRY**: OCR correction section ENTERED in ReadFormattedTextStep");

                                // Calculate and log TotalsZero using the CsvLines result
                                OCRCorrectionService.TotalsZero(res, out var totalsZero, context.Logger);
                                context.Logger?.Error(
                                    "🔍 **OCR_CORRECTION_TOTALS**: Initial TotalsZero calculation from CsvLines = {TotalsZero}",
                                    totalsZero);
                                
                                // INTENTION CONFIRMATION: Check if TotalsZero matches expected unbalanced state
                                bool isTotalsZeroUnbalanced = Math.Abs(totalsZero) > 0.01;
                                context.Logger?.Error("🔍 **OCR_INTENTION_CHECK_1**: Is TotalsZero unbalanced (abs > 0.01)? Expected=TRUE, Actual={IsUnbalanced}", isTotalsZeroUnbalanced);
                                if (!isTotalsZeroUnbalanced)
                                {
                                    context.Logger?.Error("🔍 **OCR_INTENTION_FAILED_1**: INTENTION FAILED - TotalsZero is balanced but we expected unbalanced (-147.97)");
                                }
                                else
                                {
                                    context.Logger?.Error("🔍 **OCR_INTENTION_MET_1**: INTENTION MET - TotalsZero is unbalanced as expected");
                                }

                                // Log line values for DeepSeek mapping
                                context.Logger?.Information(
                                    "OCR_CORRECTION_DEBUG: Template line values for DeepSeek mapping");
                                if (template.Lines != null)
                                {
                                    context.Logger?.Information(
                                        "OCR_CORRECTION_DEBUG: Template Lines Count: {Count}",
                                        template.Lines.Count);
                                    for (int lineIndex = 0; lineIndex < template.Lines.Count; lineIndex++)
                                    {
                                        var line = template.Lines[lineIndex];
                                        if (line?.Values != null && line.Values.Any())
                                        {
                                            context.Logger?.Information(
                                                "OCR_CORRECTION_DEBUG: Template Line[{LineIndex}] Values: {@LineValues}",
                                                lineIndex,
                                                line.Values);
                                        }
                                    }
                                }
                                else
                                {
                                    context.Logger?.Warning("OCR_CORRECTION_DEBUG: Template Lines is null");
                                }

                                int correctionAttempts = 0;
                                const int
                                    maxCorrectionAttempts =
                                        3; // Circuit breaker: allow multiple attempts to find all omissions (Gift Card + Free Shipping)

                                context.Logger?.Error("🔍 **OCR_CORRECTION_WHILE_CHECK**: About to check ShouldContinueCorrections condition");
                                context.Logger?.Error("🔍 **OCR_INTENTION_SHOULD_CONTINUE**: We EXPECT ShouldContinueCorrections to return TRUE because TotalsZero should be -147.97 (unbalanced)");
                                bool shouldContinue = WaterNut.DataSpace.OCRCorrectionService.ShouldContinueCorrections(res, out double currentTotal, context.Logger);
                                context.Logger?.Error("🔍 **OCR_CORRECTION_SHOULD_CONTINUE**: ShouldContinueCorrections = {ShouldContinue}, CurrentTotal = {CurrentTotal}", shouldContinue, currentTotal);
                                
                                // INTENTION CONFIRMATION: ShouldContinueCorrections should return TRUE for unbalanced invoice
                                context.Logger?.Error("🔍 **OCR_INTENTION_CHECK_2**: Should continue corrections? Expected=TRUE, Actual={ShouldContinue}", shouldContinue);
                                if (!shouldContinue)
                                {
                                    context.Logger?.Error("🔍 **OCR_INTENTION_FAILED_2**: INTENTION FAILED - ShouldContinueCorrections returned FALSE but we expected TRUE due to unbalanced invoice");
                                }
                                else
                                {
                                    context.Logger?.Error("🔍 **OCR_INTENTION_MET_2**: INTENTION MET - ShouldContinueCorrections returned TRUE as expected for unbalanced invoice");
                                }

                                while (shouldContinue && correctionAttempts < maxCorrectionAttempts)
                                {
                                    correctionAttempts++;
                                    context.Logger?.Information(
                                        "OCR_CORRECTION_DEBUG: Starting correction attempt {Attempt} of {MaxAttempts}. Current TotalsZero = {TotalsZero}",
                                        correctionAttempts,
                                        maxCorrectionAttempts,
                                        currentTotal);

                                    // Apply OCR correction using the CsvLines result and template
                                    context.Logger?.Error("🔍 **OCR_CORRECTION_SERVICE_CALL**: About to call OCRCorrectionService.CorrectInvoices");
                                    context.Logger?.Error("🔍 **OCR_INTENTION_SERVICE**: We EXPECT CorrectInvoices to detect 'Gift Card Amount: -$6.99' and map it to TotalDeduction field");
                                    
                                    
                                    await OCRCorrectionService.ExecuteFullPipelineForInvoiceAsync(res, template, context.Logger).ConfigureAwait(false);
                                    
                                    // CRITICAL: Update template Lines.Values with corrected values before regenerating CSVLines
                                    context.Logger?.Error("🔍 **LINES_VALUES_UPDATE_START**: Updating template Lines.Values with corrected invoice values");
                                    
                                    // Convert 'res' back to ShipmentInvoice objects for UpdateTemplateLineValues
                                    var correctedInvoices = res.Select(dynamic =>
                                    {
                                        if (dynamic is IDictionary<string, object> dict)
                                        {
                                            return OCRCorrectionService.ConvertDynamicToShipmentInvoice(dict, context.Logger);
                                        }
                                        return null;
                                    }).Where(inv => inv != null).ToList();
                                    
                                    // Update the template Lines.Values with corrected values
                                    if (correctedInvoices.Any())
                                    {
                                        OCRCorrectionService.UpdateTemplateLineValues(template, correctedInvoices, context.Logger);
                                        context.Logger?.Error("✅ **LINES_VALUES_UPDATE_SUCCESS**: Updated template Lines.Values for {Count} corrected invoices", correctedInvoices.Count);
                                        
                                        // Regenerate CSVLines from updated Lines.Values
                                        context.Logger?.Error("🔍 **CSVLINES_REGENERATE_START**: Regenerating CSVLines from updated Lines.Values");
                                        res = template.Read(textLines);
                                        context.Logger?.Error("✅ **CSVLINES_REGENERATE_SUCCESS**: Regenerated CSVLines with corrected values");
                                    }
                                    else
                                    {
                                        context.Logger?.Error("⚠️ **LINES_VALUES_UPDATE_SKIPPED**: No corrected invoices to update Lines.Values with");
                                    }
                                    
                                    context.Logger?.Error("🔍 **OCR_CORRECTION_SERVICE_RETURN**: OCRCorrectionService.CorrectInvoices completed");
                                    
                                    // **CRITICAL DEBUGGING**: Log the state of 'res' immediately after OCR correction to verify corrections were applied
                                    context.Logger?.Error("🔍 **POST_OCR_CORRECTION_RES_CHECK**: Checking 'res' dynamic structure for corrected values");
                                    if (res != null && res.Count > 0)
                                    {
                                        for (int resIdx = 0; resIdx < res.Count; resIdx++)
                                        {
                                            var resItem = res[resIdx];
                                            if (resItem is IDictionary<string, object> dict)
                                            {
                                                var tdVal = dict.TryGetValue("TotalDeduction", out var td) ? td?.ToString() : "NOT_FOUND";
                                                var tiVal = dict.TryGetValue("TotalInsurance", out var ti) ? ti?.ToString() : "NOT_FOUND";
                                                context.Logger?.Error("🔍 **POST_OCR_RES_ITEM_{Index}**: TotalDeduction={TD} | TotalInsurance={TI}", 
                                                    resIdx, tdVal, tiVal);
                                            }
                                        }
                                    }
                                    
                                    // INTENTION CONFIRMATION: Check if TotalDeduction was populated after correction
                                    // Re-read the corrected values to check
                                    if (OCRCorrectionService.TotalsZero(res, out var postCorrectionTotal, context.Logger))
                                    {
                                        context.Logger?.Error("🔍 **OCR_INTENTION_CHECK_3**: Post-correction TotalsZero = {PostTotal}, Expected=0 (balanced)", postCorrectionTotal);
                                        if (Math.Abs(postCorrectionTotal) <= 0.01)
                                        {
                                            context.Logger?.Error("🔍 **OCR_INTENTION_MET_3**: INTENTION MET - OCR correction successfully balanced the invoice");
                                        }
                                        else
                                        {
                                            context.Logger?.Error("🔍 **OCR_INTENTION_FAILED_3**: INTENTION FAILED - OCR correction did not balance the invoice, TotalsZero still = {PostTotal}", postCorrectionTotal);
                                        }
                                    }

                                    // Clear all mutable state and re-read to get updated values
                                    // This validates that the OCR correction service successfully updated the database regex patterns
                                    context.Logger?.Error("🔍 **TEMPLATE_CLEAR_START**: Clearing template state with ClearInvoiceForReimport");
                                    template.ClearInvoiceForReimport();
                                    context.Logger?.Error("✅ **TEMPLATE_CLEAR_SUCCESS**: Template state cleared");
                                    
                                    // CRITICAL FIX: Force template reload from database to pick up new regex patterns
                                    context.Logger?.Error("🔍 **TEMPLATE_RELOAD_START**: Reloading template from database to pick up new regex patterns");
                                    
                                    // Log current template state before reload
                                    var currentTemplateId = template.OcrInvoices?.Id;
                                    var currentPartsCount = template.Parts?.Count ?? 0;
                                    var currentLinesCount = template.Lines?.Count ?? 0;
                                    context.Logger?.Error("🔍 **TEMPLATE_RELOAD_CURRENT_STATE**: TemplateId={TemplateId} | Parts={PartsCount} | Lines={LinesCount}", 
                                        currentTemplateId, currentPartsCount, currentLinesCount);
                                    
                                    // Get fresh template from database with new regex patterns
                                    using (var ocrCtx = new OCR.Business.Entities.OCRContext())
                                    {
                                        context.Logger?.Error("🔍 **TEMPLATE_RELOAD_DATABASE_CONNECTION**: OCRContext created for template reload");
                                        
                                        // Check if template exists in database first
                                        context.Logger?.Error("🔍 **TEMPLATE_RELOAD_ID**: Reloading template ID={TemplateId}", currentTemplateId);
                                        
                                        if (currentTemplateId.HasValue)
                                        {
                                            // First verify template exists
                                            var templateExists = ocrCtx.Invoices.Any(x => x.Id == currentTemplateId.Value);
                                            context.Logger?.Error("🔍 **TEMPLATE_RELOAD_EXISTS_CHECK**: Template ID {TemplateId} exists in database: {Exists}", currentTemplateId.Value, templateExists);
                                            
                                            if (!templateExists)
                                            {
                                                context.Logger?.Error("❌ **TEMPLATE_RELOAD_NOT_FOUND**: Template ID {TemplateId} not found in database", currentTemplateId.Value);
                                            }
                                            else
                                            {
                                                // Use the same query pattern as GetTemplatesStep.GetActiveTemplatesQuery
                                                context.Logger?.Error("🔍 **TEMPLATE_RELOAD_QUERY_START**: Executing database query for template reload");
                                                var reloadedTemplateData = ocrCtx.Invoices
                                                    .AsNoTracking()
                                                    .Include("Parts")
                                                    .Include("InvoiceIdentificatonRegEx.OCR_RegularExpressions")
                                                    .Include("RegEx.RegEx")
                                                    .Include("RegEx.ReplacementRegEx")
                                                    .Include("Parts.RecuringPart")
                                                    .Include("Parts.Start.RegularExpressions")
                                                    .Include("Parts.End.RegularExpressions")
                                                    .Include("Parts.PartTypes")
                                                    .Include("Parts.Lines.RegularExpressions")
                                                    .Include("Parts.Lines.Fields.FieldValue")
                                                    .Include("Parts.Lines.Fields.FormatRegEx.RegEx")
                                                    .Include("Parts.Lines.Fields.FormatRegEx.ReplacementRegEx")
                                                    .Include("Parts.Lines.Fields.ChildFields.FieldValue")
                                                    .Include("Parts.Lines.Fields.ChildFields.FormatRegEx.RegEx")
                                                    .Include("Parts.Lines.Fields.ChildFields.FormatRegEx.ReplacementRegEx")
                                                    .FirstOrDefault(x => x.Id == currentTemplateId.Value);
                                                context.Logger?.Error("🔍 **TEMPLATE_RELOAD_QUERY_COMPLETE**: Database query executed");
                                                    
                                                if (reloadedTemplateData != null)
                                                {
                                                    var reloadedPartsCount = reloadedTemplateData.Parts?.Count ?? 0;
                                                    var reloadedLinesCount = reloadedTemplateData.Parts?.SelectMany(p => p.Lines ?? new List<OCR.Business.Entities.Lines>()).Count() ?? 0;
                                                    
                                                    context.Logger?.Error("✅ **TEMPLATE_RELOAD_SUCCESS**: Found reloaded template data | Parts={PartsCount} | Lines={LinesCount}", 
                                                        reloadedPartsCount, reloadedLinesCount);
                                                    
                                                    // Log comparison of regex patterns between old and new
                                                    var oldRegexPatterns = new List<string>();
                                                    foreach (var part in template.Parts ?? new List<Part>())
                                                    {
                                                        foreach (var line in part.Lines ?? new List<Line>())
                                                        {
                                                            if (line.OCR_Lines?.RegularExpressions != null && !string.IsNullOrEmpty(line.OCR_Lines.RegularExpressions.RegEx))
                                                            {
                                                                oldRegexPatterns.Add($"Line{line.OCR_Lines.Id}:{line.OCR_Lines.RegularExpressions.RegEx.Substring(0, Math.Min(50, line.OCR_Lines.RegularExpressions.RegEx.Length))}...");
                                                            }
                                                        }
                                                    }
                                                    
                                                    var newRegexPatterns = new List<string>();
                                                    foreach (var part in reloadedTemplateData.Parts ?? new List<OCR.Business.Entities.Parts>())
                                                    {
                                                        foreach (var line in part.Lines ?? new List<OCR.Business.Entities.Lines>())
                                                        {
                                                            if (line.RegularExpressions != null && !string.IsNullOrEmpty(line.RegularExpressions.RegEx))
                                                            {
                                                                newRegexPatterns.Add($"Line{line.Id}:{line.RegularExpressions.RegEx.Substring(0, Math.Min(50, line.RegularExpressions.RegEx.Length))}...");
                                                            }
                                                        }
                                                    }
                                                    
                                                    context.Logger?.Error("🔍 **TEMPLATE_RELOAD_REGEX_COMPARISON**: Old patterns: {OldCount} | New patterns: {NewCount}", oldRegexPatterns.Count, newRegexPatterns.Count);
                                                    
                                                    // Log specific differences
                                                    var addedPatterns = newRegexPatterns.Except(oldRegexPatterns).ToList();
                                                    var removedPatterns = oldRegexPatterns.Except(newRegexPatterns).ToList();
                                                    
                                                    if (addedPatterns.Any())
                                                    {
                                                        context.Logger?.Error("🔍 **TEMPLATE_RELOAD_ADDED_PATTERNS**: {AddedCount} new patterns: {@AddedPatterns}", addedPatterns.Count, addedPatterns);
                                                    }
                                                    if (removedPatterns.Any())
                                                    {
                                                        context.Logger?.Error("🔍 **TEMPLATE_RELOAD_REMOVED_PATTERNS**: {RemovedCount} removed patterns: {@RemovedPatterns}", removedPatterns.Count, removedPatterns);
                                                    }
                                                    if (!addedPatterns.Any() && !removedPatterns.Any())
                                                    {
                                                        context.Logger?.Error("⚠️ **TEMPLATE_RELOAD_NO_CHANGES**: No regex pattern changes detected between old and new template");
                                                    }
                                                    
                                                    // Create new Invoice object with reloaded data but preserve context
                                                    context.Logger?.Error("🔍 **TEMPLATE_RELOAD_CREATE_INVOICE**: Creating new Invoice object from reloaded data");
                                                    var reloadedTemplate = new Invoice(reloadedTemplateData, context.Logger);
                                                    reloadedTemplate.FileType = template.FileType;
                                                    reloadedTemplate.DocSet = template.DocSet;
                                                    reloadedTemplate.FilePath = template.FilePath;
                                                    reloadedTemplate.EmailId = template.EmailId;
                                                    reloadedTemplate.FormattedPdfText = template.FormattedPdfText; // Preserve the original text
                                                    
                                                    context.Logger?.Error("🔍 **TEMPLATE_RELOAD_UPDATE_REFERENCES**: Updating template references in context");
                                                    // Update the reference in the context to use the reloaded template
                                                    // Update the current template in the list and local reference
                                                    templatesList[templateIndex] = reloadedTemplate;
                                                    context.MatchedTemplates = templatesList;
                                                    template = reloadedTemplate; // Update local reference too
                                                    
                                                    context.Logger?.Error("✅ **TEMPLATE_RELOAD_APPLIED**: Template reloaded with fresh regex patterns from database");
                                                }
                                                else
                                                {
                                                    context.Logger?.Error("❌ **TEMPLATE_RELOAD_FAILED**: Query returned null - could not find template ID={TemplateId} in database", currentTemplateId);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            context.Logger?.Error("❌ **TEMPLATE_RELOAD_NO_ID**: Template has no ID, cannot reload from database");
                                        }
                                    }
                                    
                                    context.Logger?.Error("🔍 **TEMPLATE_RELOAD_RE_READ_START**: Re-reading template with updated patterns from database");
                                    res = template.Read(textLines); // Re-read with updated patterns from database
                                    context.Logger?.Error("🔍 **TEMPLATE_RELOAD_RE_READ_COMPLETE**: Template re-read completed, result count: {ResultCount}", res?.Count ?? 0);

                                    // Check if corrections were successful by calculating TotalsZero on the corrected 'res' structure
                                    OCRCorrectionService.TotalsZero(res, out var newTotalsZero, context.Logger);
                                    context.Logger?.Information(
                                        "OCR_CORRECTION_DEBUG: After correction attempt {Attempt}, new TotalsZero = {TotalsZero}",
                                        correctionAttempts,
                                        newTotalsZero);
                                    
                                    // Update the shouldContinue condition based on the corrected results
                                    shouldContinue = WaterNut.DataSpace.OCRCorrectionService.ShouldContinueCorrections(res, out currentTotal, context.Logger);

                                }

                                if (correctionAttempts >= maxCorrectionAttempts)
                                {
                                    OCRCorrectionService.TotalsZero(res, out var finalTotalsZero, context.Logger);
                                    context.Logger?.Warning(
                                        "OCR_CORRECTION_DEBUG: Circuit breaker triggered - maximum correction attempts ({MaxAttempts}) reached. Final TotalsZero = {TotalsZero}",
                                        maxCorrectionAttempts,
                                        finalTotalsZero);
                                }
                                else if (correctionAttempts > 0)
                                {
                                    OCRCorrectionService.TotalsZero(res, out var finalTotalsZero, context.Logger);
                                    context.Logger?.Information(
                                        "OCR_CORRECTION_DEBUG: OCR correction completed successfully after {Attempts} attempts. Final TotalsZero = {TotalsZero}",
                                        correctionAttempts,
                                        finalTotalsZero);
                                }
                                else
                                {
                                    context.Logger?.Information(
                                        "OCR_CORRECTION_DEBUG: No OCR correction needed. TotalsZero = {TotalsZero}",
                                        totalsZero);
                                }

                                context.Logger?.Error("🔍 **OCR_CORRECTION_EXIT**: OCR correction section COMPLETED in ReadFormattedTextStep");

                                template.CsvLines = res;
                            }


                            readStopwatch.Stop(); // Stop stopwatch

                            context.Logger?.Verbose(
                                "template.Read() returned. TemplateId: {TemplateId}. CsvLines: {@CsvLines}",
                                templateId,
                                template.CsvLines); // Log output CsvLines

                            context.Logger?.Information(
                                "OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                                $"Template.Read for Template {templateId}",
                                readStopwatch.ElapsedMilliseconds,
                                "Sync call returned"); // Log after Read call
                            LogTemplateReadFinished(
                                context.Logger,
                                filePath,
                                templateId,
                                template.CsvLines?.Count ?? 0); // Pass logger
                        }
                        catch (Exception readEx) // Catch errors specifically from template.Read()
                        {
                            string errorMsg =
                                $"Error executing template.Read() for TemplateId: {templateId}, File: {filePath}: {readEx.Message}";
                            LogExecutionError(
                                context.Logger,
                                readEx,
                                filePath,
                                templateId); // Log detailed error, pass logger
                            context.AddError(errorMsg); // Add error to context
                            template.CsvLines = null; // Ensure CsvLines is null after failure
                            overallSuccess = false;
                            methodStopwatch.Stop(); // Stop stopwatch immediately
                            context.Logger?.Error(
                                "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                                nameof(Execute),
                                "Read formatted PDF text based on template structure",
                                methodStopwatch.ElapsedMilliseconds,
                                "Template.Read() failed. Terminating early.");
                            context.Logger?.Error(
                                "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                                nameof(ReadFormattedTextStep),
                                "Template reading",
                                methodStopwatch.ElapsedMilliseconds,
                                "Template.Read() failed. Terminating early.");

                            continue;
                        }
                        // --- End Template Read Execution ---


                        // --- Result Check ---
                        if (!ExecutionSuccess(
                                context.Logger,
                                template,
                                filePath)) // Checks if CsvLines is null or empty, pass logger
                        {
                            // ExecutionSuccess logs the specific reason (empty CsvLines)
                            string errorMsg =
                                $"No CsvLines generated after read for TemplateId: {templateId}, File: {filePath}.";
                            context.AddError(errorMsg); // Add error to context
                            methodStopwatch.Stop(); // Stop stopwatch immediately
                            context.Logger?.Error(
                                "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                                nameof(Execute),
                                "Read formatted PDF text based on template structure",
                                methodStopwatch.ElapsedMilliseconds,
                                "No CsvLines generated. Terminating early.");
                            context.Logger?.Error(
                                "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                                nameof(ReadFormattedTextStep),
                                "Result check",
                                methodStopwatch.ElapsedMilliseconds,
                                "No CsvLines generated. Terminating early.");
                            return false; // Terminate pipeline on first empty CsvLines result
                        }
                        // --- End Result Check ---

                        // If we reach here, this template was processed successfully.
                        LogExecutionSuccess(
                            context.Logger,
                            filePath,
                            templateId); // Log individual template success, pass logger
                        // If a template is successful, we assume this is the correct one and stop processing others.
                        methodStopwatch.Stop(); // Stop stopwatch
                        context.Logger?.Information(
                            "METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                            nameof(Execute),
                            "Successfully read formatted text for a template. Terminating early as successful.",
                            $"OverallSuccess: {true}, TemplateId: {templateId}",
                            methodStopwatch.ElapsedMilliseconds);
                        context.Logger?.Information(
                            "ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                            nameof(ReadFormattedTextStep),
                            $"Successfully read formatted text for file: {filePath} using TemplateId: {templateId}. Terminating early.",
                            methodStopwatch.ElapsedMilliseconds);
                        return true; // Indicate success and stop processing further templates
                    }
                    catch (Exception ex) // Catch unexpected errors within the loop but outside template.Read()
                    {
                        string errorMsg =
                            $"Unexpected error processing TemplateId: {templateId} in ReadFormattedTextStep for File: {filePath}: {ex.Message}";
                        LogExecutionError(context.Logger, ex, filePath, templateId); // Log detailed error, pass logger
                        context.AddError(errorMsg); // Add error to context
                        template.CsvLines = null; // Ensure CsvLines is null
                        methodStopwatch.Stop(); // Stop stopwatch immediately
                        context.Logger?.Error(
                            "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                            nameof(Execute),
                            "Read formatted PDF text based on template structure",
                            methodStopwatch.ElapsedMilliseconds,
                            "Unexpected error processing template. Terminating early.");
                        context.Logger?.Error(
                            "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                            nameof(ReadFormattedTextStep),
                            "Unexpected error during template processing",
                            methodStopwatch.ElapsedMilliseconds,
                            "Unexpected error processing template. Terminating early.");
                        return false; // Terminate pipeline on first unexpected error
                    }
                }

                // If the loop completes without finding a successful template, or if it was empty initially
                methodStopwatch.Stop(); // Stop stopwatch
                if (overallSuccess) // This branch will only be hit if context.Templates was empty initially
                {
                    context.Logger?.Information(
                        "METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                        nameof(Execute),
                        "Skipped due to no templates or all templates failed but no early exit triggered (should not happen with new logic).",
                        $"OverallSuccess: {overallSuccess}",
                        methodStopwatch.ElapsedMilliseconds);
                    context.Logger?.Information(
                        "ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                        nameof(ReadFormattedTextStep),
                        $"Skipped reading formatted text for file: {filePath} (no templates or all templates failed)",
                        methodStopwatch.ElapsedMilliseconds);
                }
                else // This branch will be hit if all templates failed and no early exit was triggered (should not happen with new logic)
                {
                    context.Logger?.Error(
                        "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                        nameof(Execute),
                        "Read formatted PDF text based on template structure",
                        methodStopwatch.ElapsedMilliseconds,
                        "Reading formatted text failed for all templates.");
                    context.Logger?.Error(
                        "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                        nameof(ReadFormattedTextStep),
                        "Processing templates",
                        methodStopwatch.ElapsedMilliseconds,
                        "Reading formatted text failed for all templates.");
                }

                return overallSuccess; // Return overall success status

            } // ENABLED: Closing brace for the 'using' block
        } // Closing brace for the 'Execute' method

        // Validation specific to one template instance
        private bool ExecutionValidation(ILogger logger, Invoice template, string filePath) // Add logger parameter
        {
             if (template == null || template.OcrInvoices == null)
             {
                  LogNullTemplateWarning(logger, filePath); // Logs appropriate message, pass logger
                  return false;
             }

             int? templateId = template.OcrInvoices.Id; // Safe now
             string templateName = template.OcrInvoices.Name; // Safe now
             LogExecutionStart(logger, filePath, templateId, templateName); // Pass logger

            if (string.IsNullOrEmpty(template.FormattedPdfText))
            {
                LogEmptyFormattedPdfTextWarning(logger, filePath, templateId); // Pass logger
                return false;
            }

            return true;
        }

        private static bool ShouldContinueCorrections(List<dynamic> res, out double totalZero, ILogger logger = null)
        {
            if (OCRCorrectionService.TotalsZero(res, out totalZero, logger))
            {
                return Math.Abs(totalZero) > 0.01;
            }

            // Error case - don't continue
            var log = logger ?? Log.Logger;
            log.Warning("Could not calculate TotalsZero - stopping correction attempts");
            return false;
        }

        // Checks if the result of template.Read() is valid (not null/empty)
        private bool ExecutionSuccess(ILogger logger, Invoice template, string filePath) // Add logger parameter
        {
            // Note: Logging for finish/counts moved to main Execute method for better flow control view

            if (template.CsvLines == null || !template.CsvLines.Any())
            {
                LogEmptyCsvLinesWarning(logger, filePath, template?.OcrInvoices?.Id); // Log the specific issue, pass logger
                return false; // Indicate failure
            }

            // Logging for success moved to main Execute method after this check passes
            return true; // Indicate success
        }

        private List<string> GetTextLinesFromFormattedPdfText(ILogger logger, Invoice template, string filePath) // Add logger parameter
        {
            LogDataExtractionStart(logger, filePath, template.OcrInvoices.Id); // Pass logger
            LogFormattedPdfText(logger, template.FormattedPdfText); // Pass logger

            if (template?.OcrInvoices?.Parts != null)
            {
                LogTemplateRegexPatterns(logger, template.OcrInvoices.Parts); // Pass logger
            }

            var textLines = template.FormattedPdfText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();
            LogSplitTextLines(logger, textLines.Count); // Pass logger

            var topLevelParts = template.OcrInvoices.Parts
                .Where(p => (p.ParentParts.Any() && !p.ChildParts.Any()) ||
                            (!p.ParentParts.Any() && !p.ChildParts.Any()))
                .ToList();

            LogTopLevelPartsIdentified(logger, topLevelParts.Count); // Pass logger
            // Logging moved to main Execute method just before the call
            return textLines;
        }

        private void LogExecutionStart(ILogger logger, string filePath, int? templateId, string templateName) // Add logger parameter
        {
            logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(Execute), "Execution", "Executing ReadFormattedTextStep for template.", $"FilePath: {filePath}, TemplateId: {templateId}, TemplateName: '{templateName}'", "");
        }

        private void LogNullContextError()
        {
            // This is logged before context is validated, so cannot use context.Logger
            Log.ForContext<ReadFormattedTextStep>().Error("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(Execute), "ContextValidation", "ReadFormattedTextStep executed with null context.", "", "");
        }

        private void LogNullTemplateWarning(ILogger logger, string filePath) // Add logger parameter
        {
            logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(ExecutionValidation), "Validation", "Skipping template: Template is null.", $"FilePath: {filePath}", "Expected a valid template object.");
        }

        private void LogEmptyFormattedPdfTextWarning(ILogger logger, string filePath, int? templateId) // Add logger parameter
        {
            logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(ExecutionValidation), "Validation", "Skipping template: FormattedPdfText is null or empty.", $"FilePath: {filePath}, TemplateId: {templateId}", "Expected formatted text for reading.");
        }

        private void LogDataExtractionStart(ILogger logger, string filePath, int? templateId) // Add logger parameter
        {
            logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(GetTextLinesFromFormattedPdfText), "Extraction", "Starting data extraction using template.", $"FilePath: {filePath}, TemplateId: {templateId}", "");
        }

        private void LogFormattedPdfText(ILogger logger, string formattedPdfText) // Add logger parameter
        {
            logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(GetTextLinesFromFormattedPdfText), "Extraction", "FormattedPdfText content.", "", new { FormattedPdfText = formattedPdfText });
        }

        private void LogTemplateRegexPatterns(ILogger logger, List<Parts> parts) // Add logger parameter
        {
            logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(GetTextLinesFromFormattedPdfText), "Extraction", "Template Regex Patterns.", "", new { Parts = parts.Select(p => new { PartId = p.Id, Lines = p.Lines?.Select(l => new { LineName = l.Name, Regex = l.RegularExpressions?.RegEx }) }) });
        }

        private void LogSplitTextLines(ILogger logger, int lineCount) // Add logger parameter
        {
            logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(GetTextLinesFromFormattedPdfText), "Extraction", "Split FormattedPdfText into lines.", $"LineCount: {lineCount}", "");
        }

        private void LogTopLevelPartsIdentified(ILogger logger, int topLevelPartCount) // Add logger parameter
        {
            logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(GetTextLinesFromFormattedPdfText), "Extraction", "Identified top-level parts from template.", $"TopLevelPartCount: {topLevelPartCount}", "");
        }

        private void LogCallingTemplateRead(ILogger logger, int lineCount, string filePath, int? templateId) // Add logger parameter
        {
            logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(Execute), "TemplateRead", "Calling template.Read().", $"LineCount: {lineCount}, FilePath: {filePath}, TemplateId: {templateId}", "");
        }

        // Log message updated slightly for clarity
        private void LogTemplateReadFinished(ILogger logger, string filePath, int? templateId, int resultCount) // Add logger parameter
        {
            logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(Execute), "TemplateReadResult", "template.Read() finished.", $"FilePath: {filePath}, TemplateId: {templateId}, ResultingCsvLinesCount: {resultCount}", "");
        }

        private void LogEmptyCsvLinesWarning(ILogger logger, string filePath, int? templateId) // Add logger parameter
        {
            logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(ExecutionSuccess), "ResultCheck", "CsvLines is null or empty after extraction attempt.", $"FilePath: {filePath}, TemplateId: {templateId}", "Step fails for this template.");
        }

        private void LogExecutionSuccess(ILogger logger, string filePath, int? templateId) // Add logger parameter
        {
            logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]. {OptionalData}",
                nameof(Execute), "TemplateCompletion", "ReadFormattedTextStep finished successfully for template.", $"FilePath: {filePath}, TemplateId: {templateId}", "");
        }

        private void LogExecutionError(ILogger logger, Exception ex, string filePath, int? templateId) // Add logger parameter
        {
            logger?.Error(ex, "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                nameof(Execute), "Read formatted PDF text based on template structure", 0, $"Error during ReadFormattedTextStep for File: {filePath}, TemplateId: {templateId}. Error: {ex.Message}");
            logger?.Error(ex, "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                $"{nameof(ReadFormattedTextStep)} - Template {templateId}", "Processing template", 0, $"Error during ReadFormattedTextStep for File: {filePath}, TemplateId: {templateId}. Error: {ex.Message}");
        }
    }
}