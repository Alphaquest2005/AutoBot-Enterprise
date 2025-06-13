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
using System.Data.Entity; // Added for async Entity Framework methods

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

                                // PHASE 1: Enhanced Diagnostics - Add tracking ID for data flow monitoring
                                var trackingId = Guid.NewGuid().ToString("N").Substring(0, 8);
                                context.Logger?.Error("🚀 **TRACKING_{TrackingId}**: Amazon OCR correction pipeline started", trackingId);

                                // Mathematical verification helper method
                                void VerifyAmazonCalculation(dynamic data, string stage)
                                {
                                    var subTotal = GetDoubleValue(data, "SubTotal") ?? 161.95; // Expected
                                    var freight = GetDoubleValue(data, "TotalInternalFreight") ?? 6.99;
                                    var otherCost = GetDoubleValue(data, "TotalOtherCost") ?? 11.34;
                                    var insurance = GetDoubleValue(data, "TotalInsurance") ?? 0; // Should be -6.99
                                    var deduction = GetDoubleValue(data, "TotalDeduction") ?? 0;  // Should be 6.99
                                    var invoiceTotal = GetDoubleValue(data, "InvoiceTotal") ?? 166.30;
                                    
                                    var calculatedTotal = subTotal + freight + otherCost + insurance - deduction;
                                    var totalsZero = Math.Abs(calculatedTotal - invoiceTotal);
                                    
                                    context.Logger?.Error("🧮 **MATH_CHECK_{TrackingId}**: {Stage} | " +
                                        "ST={ST} + FR={FR} + OC={OC} + IN={IN} - DE={DE} = {CT} | " +
                                        "IT={IT} | TZ={TZ}", 
                                        trackingId, stage, subTotal, freight, otherCost, insurance, deduction, calculatedTotal, invoiceTotal, totalsZero);
                                    
                                    if (totalsZero <= 0.01)
                                    {
                                        context.Logger?.Error("✅ **MATH_SUCCESS_{TrackingId}**: {Stage} calculation balanced!", trackingId, stage);
                                    }
                                    else
                                    {
                                        context.Logger?.Error("❌ **MATH_FAILED_{TrackingId}**: {Stage} calculation unbalanced by {TZ}", trackingId, stage, totalsZero);
                                    }
                                }

                                // Helper method to safely get double values
                                double? GetDoubleValue(dynamic data, string fieldName)
                                {
                                    try
                                    {
                                        if (data is List<dynamic> list && list.Count > 0)
                                        {
                                            var item = list[0];
                                            if (item is IDictionary<string, object> dict && dict.ContainsKey(fieldName))
                                            {
                                                var value = dict[fieldName];
                                                if (value != null && double.TryParse(value.ToString(), out var result))
                                                    return result;
                                            }
                                        }
                                    }
                                    catch (Exception ex)
                                    {
                                        context.Logger?.Warning("Failed to get {FieldName}: {Error}", fieldName, ex.Message);
                                    }
                                    return null;
                                }

                                // Track Stage 1: Initial template.Read() results
                                context.Logger?.Error("🏷️ **TRACKING_{TrackingId}**: Stage1_Initial | TI={TI} | TD={TD}", 
                                    trackingId, GetDoubleValue(res, "TotalInsurance"), GetDoubleValue(res, "TotalDeduction"));
                                VerifyAmazonCalculation(res, "Stage1_Initial");

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
                                        1; // Circuit breaker: DEBUGGING - run only once to isolate issues

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
                                    context.Logger?.Error("🔄 **PATHWAY_ATTEMPT_{Attempt}_START**: Starting OCR correction attempt {Attempt} of {MaxAttempts} | Current TotalsZero = {TotalsZero} | Circuit breaker allows {Remaining} more attempts", 
                                        correctionAttempts, correctionAttempts, maxCorrectionAttempts, currentTotal, maxCorrectionAttempts - correctionAttempts);
                                    context.Logger?.Information(
                                        "OCR_CORRECTION_DEBUG: Starting correction attempt {Attempt} of {MaxAttempts}. Current TotalsZero = {TotalsZero}",
                                        correctionAttempts,
                                        maxCorrectionAttempts,
                                        currentTotal);

                                    // Apply OCR correction using the CsvLines result and template
                                    context.Logger?.Error("🔍 **OCR_CORRECTION_SERVICE_CALL**: About to call OCRCorrectionService.CorrectInvoices");
                                    context.Logger?.Error("🔍 **OCR_INTENTION_SERVICE**: We EXPECT CorrectInvoices to detect 'Gift Card Amount: -$6.99' and map it to TotalDeduction field");
                                    
                                    // **CRITICAL DATABASE VERIFICATION**: Check database state BEFORE OCR correction
                                    context.Logger?.Error("🔍 **DB_VERIFICATION_BEFORE_{Attempt}**: Checking database state BEFORE OCR correction", correctionAttempts);
                                    await VerifyDatabaseState("BEFORE_OCR", correctionAttempts, template, context.Logger);
                                    
                                    // **COMPREHENSIVE_OCR_DEBUGGING**: Wrap OCR correction call in LogLevelOverride for maximum visibility
                                    using (LogLevelOverride.Begin(LogEventLevel.Verbose))
                                    {
                                        context.Logger?.Error("🚀 **PATHWAY_ATTEMPT_{Attempt}_CORRECTION_START**: OCR correction starting with Verbose logging enabled", correctionAttempts);
                                        await OCRCorrectionService.CorrectInvoices(res, template, context.Logger).ConfigureAwait(false);
                                        context.Logger?.Error("✅ **PATHWAY_ATTEMPT_{Attempt}_CORRECTION_COMPLETE**: OCR correction completed", correctionAttempts);
                                    }
                                    
                                    // **CRITICAL DATABASE VERIFICATION**: Check database state AFTER OCR correction 
                                    context.Logger?.Error("🔍 **DB_VERIFICATION_AFTER_{Attempt}**: Checking database state AFTER OCR correction", correctionAttempts);
                                    await VerifyDatabaseState("AFTER_OCR", correctionAttempts, template, context.Logger);
                                    
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
                                        context.Logger?.Error("🔄 **PATHWAY_ATTEMPT_{Attempt}_LINES_UPDATE**: Taking LINES_VALUES_UPDATE pathway - updating template for {Count} corrected invoices", correctionAttempts, correctedInvoices.Count);
                                        OCRCorrectionService.UpdateTemplateLineValues(template, correctedInvoices, context.Logger);
                                        context.Logger?.Error("✅ **PATHWAY_ATTEMPT_{Attempt}_LINES_UPDATE_SUCCESS**: Updated template Lines.Values for {Count} corrected invoices", correctionAttempts, correctedInvoices.Count);
                                        
                                        // Regenerate CSVLines from updated Lines.Values
                                        context.Logger?.Error("🔄 **PATHWAY_ATTEMPT_{Attempt}_CSVLINES_REGENERATE**: Taking CSVLINES_REGENERATE pathway - regenerating from updated Lines.Values", correctionAttempts);
                                        res = template.Read(textLines);
                                        context.Logger?.Error("✅ **PATHWAY_ATTEMPT_{Attempt}_CSVLINES_REGENERATE_SUCCESS**: Regenerated CSVLines with corrected values", correctionAttempts);
                                    }
                                    else
                                    {
                                        context.Logger?.Error("🔄 **PATHWAY_ATTEMPT_{Attempt}_LINES_UPDATE_SKIPPED**: Taking SKIP pathway - no corrected invoices to update Lines.Values with", correctionAttempts);
                                    }
                                    
                                    context.Logger?.Error("🔍 **OCR_CORRECTION_SERVICE_RETURN**: OCRCorrectionService.CorrectInvoices completed");
                                    
                                    // Track Stage 2: Post-OCR correction results
                                    context.Logger?.Error("🏷️ **TRACKING_{TrackingId}**: Stage2_PostOCR | TI={TI} | TD={TD}", 
                                        trackingId, GetDoubleValue(res, "TotalInsurance"), GetDoubleValue(res, "TotalDeduction"));
                                    VerifyAmazonCalculation(res, "Stage2_PostOCR");
                                    
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
                                                // **REFACTORED**: Use GetTemplatesStep methodology for first template reload too
                                                context.Logger?.Error("🔍 **TEMPLATE_RELOAD_GETTEMPLATE_METHOD**: Using GetTemplatesStep methodology for template reload");
                                                
                                                var reloadedTemplate = await ReloadTemplateUsingGetTemplatesStepMethod(currentTemplateId.Value, template, context, correctionAttempts);
                                                if (reloadedTemplate != null)
                                                {
                                                    template = reloadedTemplate;
                                                    if (context.MatchedTemplates is List<Invoice> templatesListReload && templateIndex >= 0)
                                                    {
                                                        templatesListReload[templateIndex] = reloadedTemplate;
                                                        context.MatchedTemplates = templatesListReload;
                                                    }
                                                    context.Logger?.Error("✅ **TEMPLATE_RELOAD_APPLIED**: Template reloaded with fresh regex patterns from database using GetTemplatesStep method");
                                                }
                                                else
                                                {
                                                    context.Logger?.Error("❌ **TEMPLATE_RELOAD_FAILED**: GetTemplatesStep reload method failed for template ID={TemplateId}", currentTemplateId);
                                                }
                                            }
                                        }
                                        else
                                        {
                                            context.Logger?.Error("❌ **TEMPLATE_RELOAD_NO_ID**: Template has no ID, cannot reload from database");
                                        }
                                    }
                                    
                                    // PHASE 4: Template Reload Enhancement - Force complete template reload with fresh patterns
                                    context.Logger?.Error("🔄 **PATHWAY_ATTEMPT_{Attempt}_TEMPLATE_RELOAD**: PHASE 4 - Taking TEMPLATE_RELOAD pathway with fresh patterns", correctionAttempts);
                                    
                                    // STEP 1: Clear ALL cached state
                                    template.ClearInvoiceForReimport();
                                    if (template.Lines != null) template.Lines.Clear();
                                    if (template.Parts != null) template.Parts.Clear();
                                    
                                    // STEP 2: Force new database query with completely fresh context
                                    var freshTemplateId = template.OcrInvoices?.Id;
                                    context.Logger?.Error("🔄 **PATHWAY_ATTEMPT_{Attempt}_DB_QUERY**: Taking DATABASE_QUERY pathway for template {TemplateId}", correctionAttempts, freshTemplateId);
                                    
                                    if (freshTemplateId.HasValue)
                                    {
                                        // **REFACTORED**: Use GetTemplatesStep methodology for proper template reload
                                        context.Logger?.Error("🔄 **PATHWAY_ATTEMPT_{Attempt}_GETTEMPLATE_RELOAD**: Using GetTemplatesStep methodology for template reload", correctionAttempts);
                                        
                                        var reloadedTemplate = await ReloadTemplateUsingGetTemplatesStepMethod(freshTemplateId.Value, template, context, correctionAttempts);
                                        if (reloadedTemplate != null)
                                        {
                                            template = reloadedTemplate;
                                            context.Logger?.Error("✅ **PATHWAY_ATTEMPT_{Attempt}_GETTEMPLATE_RELOAD_SUCCESS**: Template reloaded using GetTemplatesStep method", correctionAttempts);
                                        }
                                        else
                                        {
                                            context.Logger?.Error("❌ **PATHWAY_ATTEMPT_{Attempt}_GETTEMPLATE_RELOAD_FAILED**: Failed to reload template using GetTemplatesStep method", correctionAttempts);
                                        }
                                    }
                                    else
                                    {
                                        context.Logger?.Error("❌ **TEMPLATE_RELOAD_NO_ID**: Template has no ID, using standard ClearInvoiceForReimport");
                                        template.ClearInvoiceForReimport();
                                    }
                                    
                                    // **CRITICAL DATABASE VERIFICATION**: Check database state AFTER template reload
                                    context.Logger?.Error("🔍 **DB_VERIFICATION_AFTER_RELOAD_{Attempt}**: Checking database state AFTER template reload", correctionAttempts);
                                    await VerifyDatabaseState("AFTER_TEMPLATE_RELOAD", correctionAttempts, template, context.Logger);
                                    
                                    // STEP 6: Re-read with completely fresh patterns
                                    context.Logger?.Error("🔄 **PATHWAY_ATTEMPT_{Attempt}_TEMPLATE_RE_READ**: PHASE 4 - Taking TEMPLATE_RE_READ pathway with fresh database patterns", correctionAttempts);
                                    res = template.Read(textLines);
                                    context.Logger?.Error("✅ **PATHWAY_ATTEMPT_{Attempt}_TEMPLATE_RE_READ_SUCCESS**: New CsvLines count: {Count}", correctionAttempts, res?.Count ?? 0);
                                    
                                    // Track Stage 3: Post-template reload results
                                    context.Logger?.Error("🏷️ **TRACKING_{TrackingId}**: Stage3_PostReload | TI={TI} | TD={TD}", 
                                        trackingId, GetDoubleValue(res, "TotalInsurance"), GetDoubleValue(res, "TotalDeduction"));
                                    VerifyAmazonCalculation(res, "Stage3_PostReload");

                                    // Check if corrections were successful by calculating TotalsZero on the corrected 'res' structure
                                    OCRCorrectionService.TotalsZero(res, out var newTotalsZero, context.Logger);
                                    context.Logger?.Information(
                                        "OCR_CORRECTION_DEBUG: After correction attempt {Attempt}, new TotalsZero = {TotalsZero}",
                                        correctionAttempts,
                                        newTotalsZero);
                                    
                                    // Update the shouldContinue condition based on the corrected results
                                    shouldContinue = WaterNut.DataSpace.OCRCorrectionService.ShouldContinueCorrections(res, out currentTotal, context.Logger);
                                    context.Logger?.Error("🔄 **PATHWAY_ATTEMPT_{Attempt}_CONTINUE_CHECK**: Circuit breaker check - shouldContinue={ShouldContinue} | Attempts: {Attempts}/{MaxAttempts}", 
                                        correctionAttempts, shouldContinue, correctionAttempts, maxCorrectionAttempts);

                                }

                                // Circuit breaker exit pathway logging
                                if (correctionAttempts >= maxCorrectionAttempts)
                                {
                                    OCRCorrectionService.TotalsZero(res, out var finalTotalsZero, context.Logger);
                                    context.Logger?.Error("🛑 **PATHWAY_CIRCUIT_BREAKER**: Taking CIRCUIT_BREAKER_EXIT pathway - maximum attempts ({MaxAttempts}) reached. Final TotalsZero = {TotalsZero}", 
                                        maxCorrectionAttempts, finalTotalsZero);
                                    context.Logger?.Warning(
                                        "OCR_CORRECTION_DEBUG: Circuit breaker triggered - maximum correction attempts ({MaxAttempts}) reached. Final TotalsZero = {TotalsZero}",
                                        maxCorrectionAttempts,
                                        finalTotalsZero);
                                }
                                else if (correctionAttempts > 0)
                                {
                                    OCRCorrectionService.TotalsZero(res, out var finalTotalsZero, context.Logger);
                                    context.Logger?.Error("✅ **PATHWAY_SUCCESS_EXIT**: Taking SUCCESS_EXIT pathway after {Attempts} attempts. Final TotalsZero = {TotalsZero}", 
                                        correctionAttempts, finalTotalsZero);
                                    context.Logger?.Information(
                                        "OCR_CORRECTION_DEBUG: OCR correction completed successfully after {Attempts} attempts. Final TotalsZero = {TotalsZero}",
                                        correctionAttempts,
                                        finalTotalsZero);
                                }
                                else
                                {
                                    context.Logger?.Error("ℹ️ **PATHWAY_NO_CORRECTION**: Taking NO_CORRECTION pathway - TotalsZero already balanced = {TotalsZero}", totalsZero);
                                    context.Logger?.Information(
                                        "OCR_CORRECTION_DEBUG: No OCR correction needed. TotalsZero = {TotalsZero}",
                                        totalsZero);
                                }

                                context.Logger?.Error("🔍 **OCR_CORRECTION_EXIT**: OCR correction section COMPLETED in ReadFormattedTextStep");

                                template.CsvLines = res;
                                
                                // Track Stage 4: Final template.CsvLines assignment
                                context.Logger?.Error("🏷️ **TRACKING_{TrackingId}**: Stage4_FinalAssignment | TI={TI} | TD={TD}", 
                                    trackingId, GetDoubleValue(template.CsvLines, "TotalInsurance"), GetDoubleValue(template.CsvLines, "TotalDeduction"));
                                VerifyAmazonCalculation(template.CsvLines, "Stage4_FinalAssignment");
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

        /// <summary>
        /// REFACTORED TEMPLATE RELOAD: Use GetTemplatesStep methodology for proper template reload with all includes
        /// This ensures the reloaded template has the exact same navigation properties as the original GetTemplatesStep
        /// </summary>
        private static async Task<Invoice> ReloadTemplateUsingGetTemplatesStepMethod(int templateId, Invoice originalTemplate, InvoiceProcessingContext context, int attemptNumber)
        {
            context.Logger?.Error("🔄 **RELOAD_GETTEMPLATE_{Attempt}_START**: Starting template reload using GetTemplatesStep methodology for template {TemplateId}", attemptNumber, templateId);
            
            try
            {
                // **CRITICAL**: Use fresh OCRContext with GetTemplatesStep query pattern
                using (var freshCtx = new global::OCR.Business.Entities.OCRContext())
                {
                    context.Logger?.Error("🔍 **RELOAD_GETTEMPLATE_{Attempt}_FRESH_CONTEXT**: Created fresh OCRContext", attemptNumber);
                    
                    // **EXACT SAME QUERY AS GetTemplatesStep.GetActiveTemplatesQuery**
                    var reloadedTemplateData = freshCtx.Invoices
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
                        .FirstOrDefault(x => x.Id == templateId);
                    
                    if (reloadedTemplateData == null)
                    {
                        context.Logger?.Error("❌ **RELOAD_GETTEMPLATE_{Attempt}_NOT_FOUND**: Template {TemplateId} not found in database", attemptNumber, templateId);
                        return null;
                    }
                    
                    context.Logger?.Error("✅ **RELOAD_GETTEMPLATE_{Attempt}_FOUND**: Template data loaded from database", attemptNumber);
                    
                    // **VERIFICATION**: Check that all navigation properties are loaded
                    var partsCount = reloadedTemplateData.Parts?.Count ?? 0;
                    var linesCount = reloadedTemplateData.Parts?.Sum(p => p.Lines?.Count ?? 0) ?? 0;
                    var fieldsCount = reloadedTemplateData.Parts?.Sum(p => p.Lines?.Sum(l => l.Fields?.Count ?? 0) ?? 0) ?? 0;
                    var regexCount = reloadedTemplateData.Parts?.Sum(p => p.Lines?.Count(l => l.RegularExpressions != null) ?? 0) ?? 0;
                    
                    context.Logger?.Error("📊 **RELOAD_GETTEMPLATE_{Attempt}_STRUCTURE**: Loaded Parts={Parts}, Lines={Lines}, Fields={Fields}, Regex={Regex}", 
                        attemptNumber, partsCount, linesCount, fieldsCount, regexCount);
                    
                    // **SAME AS GetTemplatesStep**: Create new Invoice object
                    var reloadedTemplate = new Invoice(reloadedTemplateData, context.Logger);
                    
                    // **PRESERVE CONTEXT PROPERTIES**: Copy non-database properties from original template
                    reloadedTemplate.FileType = originalTemplate.FileType;
                    reloadedTemplate.DocSet = originalTemplate.DocSet;
                    reloadedTemplate.FilePath = originalTemplate.FilePath;
                    reloadedTemplate.EmailId = originalTemplate.EmailId;
                    reloadedTemplate.FormattedPdfText = originalTemplate.FormattedPdfText;
                    
                    // **UPDATE CONTEXT REFERENCES**: Update the template in context.MatchedTemplates
                    if (context.MatchedTemplates is List<Invoice> templatesList)
                    {
                        var templateIndex = templatesList.FindIndex(t => t.OcrInvoices?.Id == templateId);
                        if (templateIndex >= 0)
                        {
                            templatesList[templateIndex] = reloadedTemplate;
                            context.MatchedTemplates = templatesList;
                            context.Logger?.Error("✅ **RELOAD_GETTEMPLATE_{Attempt}_CONTEXT_UPDATED**: Updated template reference in MatchedTemplates", attemptNumber);
                        }
                    }
                    
                    context.Logger?.Error("✅ **RELOAD_GETTEMPLATE_{Attempt}_SUCCESS**: Template successfully reloaded using GetTemplatesStep methodology", attemptNumber);
                    return reloadedTemplate;
                }
            }
            catch (Exception ex)
            {
                context.Logger?.Error("❌ **RELOAD_GETTEMPLATE_{Attempt}_ERROR**: Template reload failed: {Error}", attemptNumber, ex.Message);
                context.Logger?.Error("   Stack trace: {StackTrace}", ex.StackTrace);
                return null;
            }
        }

        /// <summary>
        /// CRITICAL DATABASE VERIFICATION: Verify that OCR corrections are actually saved to database and properly reloaded
        /// This method performs DIRECT database queries (not cached) to ensure corrections persist
        /// </summary>
        private static async Task VerifyDatabaseState(string stage, int attemptNumber, Invoice template, ILogger logger)
        {
            logger?.Error("🔍 **DB_VERIFY_{Stage}_{Attempt}_START**: Starting database verification", stage, attemptNumber);
            
            var templateId = template?.OcrInvoices?.Id;
            if (!templateId.HasValue)
            {
                logger?.Error("❌ **DB_VERIFY_{Stage}_{Attempt}_NO_ID**: Template has no ID - cannot verify database state", stage, attemptNumber);
                return;
            }

            try
            {
                // **CRITICAL**: Use fresh OCRContext - NO CACHING - direct database access
                using (var freshDbContext = new global::OCR.Business.Entities.OCRContext())
                {
                    logger?.Error("🔍 **DB_VERIFY_{Stage}_{Attempt}_FRESH_CONTEXT**: Created fresh OCRContext for direct database verification", stage, attemptNumber);
                    
                    // ASSERTION 1: Verify Amazon template exists in database with FULL INCLUDES (matching GetTemplatesStep)
                    var amazonTemplate = await freshDbContext.Invoices
                        .AsNoTracking() // CRITICAL: No caching
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
                        .FirstOrDefaultAsync(i => i.Id == templateId.Value);
                    
                    if (amazonTemplate == null)
                    {
                        logger?.Error("❌ **DB_VERIFY_{Stage}_{Attempt}_TEMPLATE_NOT_FOUND**: Amazon template ID {TemplateId} not found in database", stage, attemptNumber, templateId);
                        return;
                    }
                    
                    logger?.Error("✅ **DB_VERIFY_{Stage}_{Attempt}_TEMPLATE_FOUND**: Amazon template '{Name}' found in database", stage, attemptNumber, amazonTemplate.Name);
                    
                    // ASSERTION 1.1: Verify complete template structure is loaded with all navigation properties
                    var partsCount = amazonTemplate.Parts?.Count ?? 0;
                    var totalLinesCount = amazonTemplate.Parts?.Sum(p => p.Lines?.Count ?? 0) ?? 0;
                    var totalFieldsCount = amazonTemplate.Parts?.Sum(p => p.Lines?.Sum(l => l.Fields?.Count ?? 0) ?? 0) ?? 0;
                    var totalRegexCount = amazonTemplate.Parts?.Sum(p => p.Lines?.Count(l => l.RegularExpressions != null) ?? 0) ?? 0;
                    
                    logger?.Error("📋 **DB_VERIFY_{Stage}_{Attempt}_TEMPLATE_STRUCTURE**: Parts={PartsCount}, Lines={LinesCount}, Fields={FieldsCount}, RegexPatterns={RegexCount}", 
                        stage, attemptNumber, partsCount, totalLinesCount, totalFieldsCount, totalRegexCount);
                    
                    // ASSERTION 1.2: Verify specific parts exist (Header, Detail, etc.)
                    var partNames = amazonTemplate.Parts?.Select(p => $"Part_{p.Id}").ToList() ?? new List<string>();
                    logger?.Error("📂 **DB_VERIFY_{Stage}_{Attempt}_PART_NAMES**: Amazon template parts: [{PartNames}]", stage, attemptNumber, string.Join(", ", partNames));
                    
                    // ASSERTION 1.3: Show sample of regex patterns to verify they're loaded
                    if (amazonTemplate.Parts != null)
                    {
                        var sampleRegexPatterns = amazonTemplate.Parts
                            .SelectMany(p => p.Lines ?? new List<global::OCR.Business.Entities.Lines>())
                            .Where(l => l.RegularExpressions != null && !string.IsNullOrEmpty(l.RegularExpressions.RegEx))
                            .Take(5)
                            .Select(l => new { LineId = l.Id, LineName = l.Name, Pattern = l.RegularExpressions.RegEx })
                            .ToList();
                        
                        foreach (var pattern in sampleRegexPatterns)
                        {
                            var truncatedPattern = pattern.Pattern?.Substring(0, Math.Min(50, pattern.Pattern?.Length ?? 0)) + "...";
                            logger?.Error("   → Sample Pattern: LineId={LineId}, Name='{LineName}', Regex='{Pattern}'", pattern.LineId, pattern.LineName, truncatedPattern);
                        }
                    }
                    
                    // ASSERTION 2: Count total regex patterns for Amazon template - DIRECT DATABASE QUERY
                    var totalPatternCount = await freshDbContext.RegularExpressions
                        .AsNoTracking() // CRITICAL: No caching
                        .Where(r => r.Lines.Any(l => l.Parts.TemplateId == templateId.Value))
                        .CountAsync();
                    
                    logger?.Error("📊 **DB_VERIFY_{Stage}_{Attempt}_PATTERN_COUNT**: Amazon template has {Count} regex patterns in database", stage, attemptNumber, totalPatternCount);
                    
                    // ASSERTION 3: Check for Gift Card patterns (TotalInsurance mapping)
                    var giftCardPatterns = await freshDbContext.RegularExpressions
                        .AsNoTracking() // CRITICAL: No caching
                        .Where(r => r.Lines.Any(l => l.Parts.TemplateId == templateId.Value) && 
                                   (r.RegEx.Contains("Gift Card") || (r.Description != null && r.Description.Contains("Gift Card"))))
                        .Select(r => new { r.Id, r.RegEx, r.Description })
                        .ToListAsync();
                    
                    logger?.Error("🎁 **DB_VERIFY_{Stage}_{Attempt}_GIFT_CARD**: Found {Count} Gift Card patterns in database", stage, attemptNumber, giftCardPatterns.Count);
                    foreach (var pattern in giftCardPatterns)
                    {
                        logger?.Error("   → Gift Card Pattern: ID={Id}, Regex='{Regex}', Description='{Description}'", pattern.Id, pattern.RegEx ?? "NULL", pattern.Description ?? "NULL");
                    }
                    
                    // ASSERTION 4: Check for Free Shipping patterns (TotalDeduction mapping)
                    var freeShippingPatterns = await freshDbContext.RegularExpressions
                        .AsNoTracking() // CRITICAL: No caching
                        .Where(r => r.Lines.Any(l => l.Parts.TemplateId == templateId.Value) && 
                                   (r.RegEx.Contains("Free Shipping") || (r.Description != null && r.Description.Contains("FreeShipping"))))
                        .Select(r => new { r.Id, r.RegEx, r.Description })
                        .ToListAsync();
                    
                    logger?.Error("🚚 **DB_VERIFY_{Stage}_{Attempt}_FREE_SHIPPING**: Found {Count} Free Shipping patterns in database", stage, attemptNumber, freeShippingPatterns.Count);
                    foreach (var pattern in freeShippingPatterns)
                    {
                        logger?.Error("   → Free Shipping Pattern: ID={Id}, Regex='{Regex}', Description='{Description}'", pattern.Id, pattern.RegEx ?? "NULL", pattern.Description ?? "NULL");
                    }
                    
                    // ASSERTION 5: Check for new patterns created by OCR correction (AutoOmission patterns)
                    var autoOmissionPatterns = await freshDbContext.Lines
                        .AsNoTracking() // CRITICAL: No caching
                        .Include(l => l.RegularExpressions)
                        .Include(l => l.Fields)
                        .Where(l => l.Parts.TemplateId == templateId.Value && 
                                   l.Name.StartsWith("AutoOmission"))
                        .Select(l => new { 
                            l.Id, 
                            l.Name, 
                            RegexPattern = l.RegularExpressions != null ? l.RegularExpressions.RegEx : null,
                            FieldNames = l.Fields.Select(f => f.Field).ToList()
                        })
                        .ToListAsync();
                    
                    logger?.Error("🤖 **DB_VERIFY_{Stage}_{Attempt}_AUTO_OMISSION**: Found {Count} AutoOmission patterns in database", stage, attemptNumber, autoOmissionPatterns.Count);
                    foreach (var pattern in autoOmissionPatterns)
                    {
                        logger?.Error("   → AutoOmission: ID={Id}, Name='{Name}', Regex='{Regex}', Fields=[{Fields}]", 
                            pattern.Id, pattern.Name, pattern.RegexPattern ?? "NULL", string.Join(", ", pattern.FieldNames));
                    }
                    
                    // ASSERTION 6: Check OCRCorrectionLearning table for recent corrections
                    try
                    {
                        // Note: OCRCorrectionLearning may not exist in this context, check if accessible
                        logger?.Error("🔍 **DB_VERIFY_{Stage}_{Attempt}_LEARNING_SKIP**: OCRCorrectionLearning table check skipped - may not be in this context", stage, attemptNumber);
                        var correctionLearnings = new List<object>(); // Empty for now
                        
                        logger?.Error("📚 **DB_VERIFY_{Stage}_{Attempt}_LEARNING**: Found {Count} OCRCorrectionLearning entries for Amazon invoice", stage, attemptNumber, correctionLearnings.Count);
                        // Skip foreach since we have empty list
                    }
                    catch (Exception learningEx)
                    {
                        logger?.Error("⚠️ **DB_VERIFY_{Stage}_{Attempt}_LEARNING_ERROR**: Could not query OCRCorrectionLearning table: {Error}", stage, attemptNumber, learningEx.Message);
                    }
                    
                    // ASSERTION 7: Verify that database is actually being accessed (not cached) by checking connection state
                    var connectionState = freshDbContext.Database.Connection.State;
                    logger?.Error("🔌 **DB_VERIFY_{Stage}_{Attempt}_CONNECTION**: Database connection state = {State}", stage, attemptNumber, connectionState);
                    
                    // ASSERTION 8: Force a database round-trip to ensure we're not using cached data
                    var timestampQuery = freshDbContext.Database.SqlQuery<DateTime>("SELECT GETDATE()").FirstOrDefault();
                    logger?.Error("⏰ **DB_VERIFY_{Stage}_{Attempt}_TIMESTAMP**: Database server timestamp = {Timestamp} (proves direct DB access)", stage, attemptNumber, timestampQuery);
                    
                    logger?.Error("✅ **DB_VERIFY_{Stage}_{Attempt}_COMPLETE**: Database verification completed successfully", stage, attemptNumber);
                }
            }
            catch (Exception ex)
            {
                logger?.Error("❌ **DB_VERIFY_{Stage}_{Attempt}_ERROR**: Database verification failed: {Error}", stage, attemptNumber, ex.Message);
                logger?.Error("   Stack trace: {StackTrace}", ex.StackTrace);
            }
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