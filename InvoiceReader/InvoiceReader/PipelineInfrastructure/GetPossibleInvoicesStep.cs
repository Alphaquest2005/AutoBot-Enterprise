using OCR.Business.Entities;
using System.Text.RegularExpressions; // Added using directive
using System.Collections.Generic; // Added
using System.Linq; // Added
using System.Threading.Tasks; // Added
using Serilog; // Added
using Serilog.Events; // Added for LogLevelOverride
using System;
using InvoiceReader.PipelineInfrastructure;
using WaterNut.Business.Services.Utils; // Added
using Core.Common.Extensions; // Added for LogLevelOverride
using System.Data.Entity; // Added for FirstOrDefaultAsync

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    using System.Diagnostics;

    public partial class GetPossibleInvoicesStep : IPipelineStep<InvoiceProcessingContext>
    {
        // Remove static logger
        // private static readonly ILogger _logger = Log.ForContext<GetPossibleInvoicesStep>();
        private static readonly TimeSpan RegexTimeout = TimeSpan.FromSeconds(5);

        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            var methodStopwatch = Stopwatch.StartNew(); // Start stopwatch for method execution
            string filePath = context?.FilePath ?? "Unknown";
                context.Logger?.Information(
                    "METHOD_ENTRY: {MethodName}. Intention: {MethodIntention}. InitialState: [{InitialStateContext}]",
                    nameof(Execute),
                    "Identify possible invoice templates based on PDF text",
                    $"FilePath: {filePath}");

                context.Logger?.Information(
                    "ACTION_START: {ActionName}. Context: [{ActionContext}]",
                    nameof(GetPossibleInvoicesStep),
                    $"Identifying possible invoices for file: {filePath}");

                if (!ValidateContext(context, filePath))
                {
                    methodStopwatch.Stop();
                    context.Logger?.Error(
                        "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                        nameof(Execute),
                        "Identify possible invoice templates based on PDF text",
                        methodStopwatch.ElapsedMilliseconds,
                        "Context validation failed.");
                    context.Logger?.Error(
                        "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                        nameof(GetPossibleInvoicesStep),
                        "Context validation",
                        methodStopwatch.ElapsedMilliseconds,
                        "Context validation failed.");
                    return false;
                }

                try
                {
                    string pdfTextString = context.PdfText.ToString();
                    int totalTemplateCount = context.Templates?.Count() ?? 0;
                    context.Logger?.Information(
                        "INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                        nameof(Execute),
                        "Processing",
                        "Processing templates to find possible invoices.",
                        $"TotalTemplateCount: {totalTemplateCount}, FilePath: {filePath}");

                    var possibleInvoices =
                        await GetPossibleInvoices(context, pdfTextString, filePath).ConfigureAwait(false);

                    //if (possibleInvoices.All(x => FileTypeManager.GetFileType(x.OcrInvoices.FileTypeId).FileImporterInfos.EntryType != "Shipment Template"))
                    //    throw new ApplicationException("No Shipment Template Templates found");

                    // Assign the identified possible invoices to the new MatchedTemplates property
                    context.MatchedTemplates = possibleInvoices;

                    // 🔍 **HYBRID_DOCUMENT_DETECTION**: Check if templates found but missing ShipmentInvoice when invoice content exists
                    context.Logger?.Information(
                        "🔍 **HYBRID_DOCUMENT_DETECTION_START**: Checking for missing ShipmentInvoice templates when invoice content present");
                    context.Logger?.Information(
                        "   - **MATCHED_TEMPLATES_COUNT**: {TemplateCount} templates found",
                        context.MatchedTemplates?.Count() ?? 0);
                    context.Logger?.Information(
                        "   - **TEMPLATE_TYPES**: {TemplateTypes}",
                        string.Join(
                            ", ",
                            context.MatchedTemplates?.Select(
                                t =>
                                    $"{t.OcrTemplates?.Name ?? "NULL"}({t.FileType?.FileImporterInfos?.EntryType ?? "NULL_ENTRYTYPE"})")
                            ?? new string[0]));

                    // **VERIFICATION_LOGGING**: Add detailed logging to verify FileType and EntryType mappings
                    context.Logger?.Information(
                        "🔍 **TEMPLATE_VERIFICATION_START**: Detailed analysis of each matched template");
                    foreach (var template in context.MatchedTemplates ?? Enumerable.Empty<Template>())
                    {
                        context.Logger?.Information(
                            "   - **TEMPLATE_DETAIL**: '{TemplateName}' (ID: {TemplateId})",
                            template.OcrTemplates?.Name ?? "NULL",
                            template.OcrTemplates?.Id ?? 0);
                        context.Logger?.Information(
                            "     └── **FILETYPE_ID**: {FileTypeId}",
                            template.FileType?.Id ?? 0);
                        context.Logger?.Information(
                            "     └── **FILETYPE_DESCRIPTION**: '{FileTypeDescription}'",
                            template.FileType?.Description ?? "NULL");
                        context.Logger?.Information(
                            "     └── **FILETYPE_PATTERN**: '{FilePattern}'",
                            template.FileType?.FilePattern ?? "NULL");
                        context.Logger?.Information(
                            "     └── **FILEIMPORTERINFOS_NULL**: {IsNull}",
                            template.FileType?.FileImporterInfos == null);

                        if (template.FileType?.FileImporterInfos != null)
                        {
                            context.Logger?.Information(
                                "     └── **FILEIMPORTERINFOS_ENTRYTYPE**: '{EntryType}'",
                                template.FileType.FileImporterInfos.EntryType ?? "NULL");
                            context.Logger?.Information(
                                "     └── **FILEIMPORTERINFOS_ID**: {FileImporterInfoId}",
                                template.FileType.FileImporterInfos.Id);
                            context.Logger?.Information(
                                "     └── **ENTRYTYPE_COMPARISON**: '{ActualEntryType}' == '{ExpectedEntryType}' = {IsMatch}",
                                template.FileType.FileImporterInfos.EntryType ?? "NULL",
                                FileTypeManager.EntryTypes.ShipmentInvoice,
                                template.FileType.FileImporterInfos.EntryType
                                == FileTypeManager.EntryTypes.ShipmentInvoice);
                        }
                        else
                        {
                            context.Logger?.Information(
                                "     └── **FILEIMPORTERINFOS**: NULL - Cannot determine EntryType");
                        }
                    }

                    context.Logger?.Information("🔍 **TEMPLATE_VERIFICATION_END**: Analysis complete");

                    // **CONSTANT_VERIFICATION**: Log the actual constant value being compared
                    context.Logger?.Information(
                        "🔍 **CONSTANT_CHECK**: FileTypeManager.EntryTypes.ShipmentInvoice = '{ShipmentInvoiceConstant}'",
                        FileTypeManager.EntryTypes.ShipmentInvoice);

                    var hasShipmentInvoiceTemplate = context.MatchedTemplates.Any(
                        t => t.FileType?.FileImporterInfos?.EntryType == FileTypeManager.EntryTypes.ShipmentInvoice);

                    context.Logger?.Information(
                        "🎯 **CRITICAL_DECISION_POINT**: Evaluating template creation condition");
                    context.Logger?.Information(
                        "   - **SHIPMENT_INVOICE_CHECK**: HasShipmentInvoiceTemplate = {HasShipmentInvoice}",
                        hasShipmentInvoiceTemplate);
                    context.Logger?.Information(
                        "   - **PDF_TEXT_LENGTH**: {PdfTextLength} characters available for analysis",
                        pdfTextString?.Length ?? 0);
                    context.Logger?.Information(
                        "   - **PDF_TEXT_IS_NULL_OR_EMPTY**: {IsNullOrEmpty}",
                        string.IsNullOrEmpty(pdfTextString));
                    context.Logger?.Information(
                        "   - **PDF_TEXT_SAMPLE**: '{TextSample}...'",
                        pdfTextString?.Substring(0, Math.Min(200, pdfTextString?.Length ?? 0)) ?? "NULL");

                    // Debug the condition logic
                    context.Logger?.Information(
                        "🔍 **CONDITION_ANALYSIS**: Checking hybrid template creation condition");
                    context.Logger?.Information(
                        "   - **CONDITION_1**: !hasShipmentInvoiceTemplate = {Condition1}",
                        !hasShipmentInvoiceTemplate);
                    context.Logger?.Information(
                        "   - **CONDITION_2**: !string.IsNullOrEmpty(pdfTextString) = {Condition2}",
                        !string.IsNullOrEmpty(pdfTextString));
                    context.Logger?.Information(
                        "   - **COMBINED_CONDITION**: ({Condition1} && {Condition2}) = {CombinedResult}",
                        !hasShipmentInvoiceTemplate,
                        !string.IsNullOrEmpty(pdfTextString),
                        !hasShipmentInvoiceTemplate && !string.IsNullOrEmpty(pdfTextString));

                    context.Logger?.Information(
                        "🎯 **DECISION_OUTCOME**: Will template creation be triggered? {WillCreate}",
                        !hasShipmentInvoiceTemplate && !string.IsNullOrEmpty(pdfTextString));

                    if (!hasShipmentInvoiceTemplate && !string.IsNullOrEmpty(pdfTextString))
                    {
                        context.Logger?.Information(
                            "🎯 **CONDITION_SATISFIED**: Template creation condition is TRUE, entering OCR template creation pathway");

                        // **SURGICAL_DEBUGGING**: Use LogLevelOverride to capture ALL details of template creation process

                        context.Logger?.Information(
                            "🚀 **LOGLEVELOVERRIDE_ACTIVATED**: Surgical debugging enabled - capturing complete OCR call chain");
                        context.Logger?.Information(
                            "   - **OVERRIDE_LEVEL**: Verbose - all logs within this scope will be captured");
                        context.Logger?.Information(
                            "   - **EXPECTED_TERMINATION**: Application will terminate when this scope closes");
                        context.Logger?.Information(
                            "   - **SCOPE_PURPOSE**: Capture complete CreateInvoiceTemplateAsync call chain with all internal logging");

                        context.Logger?.Information("✅ **CONDITION_MET**: Proceeding with template creation logic");
                        context.Logger?.Information(
                            "🔍 **INVOICE_KEYWORD_ANALYSIS**: No ShipmentInvoice template found, analyzing PDF content for invoice keywords");
                        context.Logger?.Information(
                            "   - **CURRENT_MATCHED_TEMPLATES**: {TemplateList}",
                            string.Join(
                                ", ",
                                context.MatchedTemplates?.Select(
                                    t => $"{t.OcrTemplates?.Name}({t.FileType?.FileImporterInfos?.EntryType})")
                                ?? new string[0]));

                        var containsInvoiceKeywords = ContainsInvoiceKeywords(pdfTextString, context.Logger);
                        context.Logger?.Information(
                            "🔍 **KEYWORD_DETECTION_RESULT**: ContainsInvoiceKeywords = {ContainsKeywords}",
                            containsInvoiceKeywords);

                        if (containsInvoiceKeywords)
                        {
                            context.Logger?.Information(
                                "🚀 **HYBRID_TEMPLATE_CREATION**: Invoice content detected, creating ShipmentInvoice template via OCR correction service");
                            context.Logger?.Information(
                                "   - **CREATION_INTENT**: Create minimal Invoice template that pipeline can process alongside existing templates");
                            context.Logger?.Information(
                                "   - **EXPECTED_RESULT**: Both existing templates AND new ShipmentInvoice template will be processed");
                            context.Logger?.Information(
                                "   - **PIPELINE_INTEGRATION**: New template will be added to context.MatchedTemplates for downstream processing");

                            // **PRE-CALL STATE CAPTURE**: Log everything before the OCR service call
                            context.Logger?.Information(
                                "📊 **PRE_CALL_STATE_CAPTURE**: Capturing complete state before CreateInvoiceTemplateAsync call");
                            context.Logger?.Information(
                                "   - **PDF_TEXT_LENGTH**: {Length} characters",
                                pdfTextString?.Length ?? 0);
                            context.Logger?.Information("   - **FILE_PATH**: '{FilePath}'", filePath);
                            context.Logger?.Information(
                                "   - **CURRENT_TEMPLATE_COUNT**: {Count}",
                                context.MatchedTemplates?.Count() ?? 0);
                            context.Logger?.Information(
                                "   - **CONTEXT_DOCSET_COUNT**: {DocSetCount}",
                                context.DocSet?.Count ?? 0);
                            context.Logger?.Information(
                                "   - **CONTEXT_EMAIL_ID**: '{EmailId}'",
                                context.EmailId ?? "NULL");
                            context.Logger?.Information(
                                "   - **CONTEXT_FILE_PATH**: '{ContextFilePath}'",
                                context.FilePath ?? "NULL");

                            context.Logger?.Information(
                                "🔍 **OCR_SERVICE_CALL_START**: Calling CreateInvoiceTemplateAsync - COMPLETE CALL CHAIN SHOULD FOLLOW");
                            context.Logger?.Information(
                                "   - **METHOD_SIGNATURE**: CreateInvoiceTemplateAsync(string pdfText, string filePath)");
                            context.Logger?.Information(
                                "   - **EXPECTED_LOGS**: Template creation orchestration, DeepSeek analysis, database operations, template retrieval");
                            context.Logger?.Information(
                                "   - **CRITICAL_EXPECTATION**: All internal OCR service logs should appear due to LogLevelOverride scope");

                            try
                            {
                                context.Logger?.Information(
                                    "🏗️ **CREATING_OCR_SERVICE**: Instantiating OCRCorrectionService with context logger");
                                using (var ocrService = new WaterNut.DataSpace.OCRCorrectionService(context.Logger))
                                {
                                    context.Logger?.Information(
                                        "✅ **OCR_SERVICE_CREATED**: Service instantiated successfully, calling CreateInvoiceTemplateAsync");

                                    var ocrTemplates =
                                        await ocrService.CreateInvoiceTemplateAsync(pdfTextString, filePath)
                                            .ConfigureAwait(false);

                                    // **POST-CALL STATE CAPTURE**: Log everything after the OCR service call
                                    context.Logger?.Information(
                                        "📊 **POST_CALL_STATE_CAPTURE**: CreateInvoiceTemplateAsync returned, analyzing result");
                                    context.Logger?.Information(
                                        "🔍 **OCR_SERVICE_RESULT**: OCR template result = {Result}",
                                        ocrTemplates != null && ocrTemplates.Any() ? $"SUCCESS: {ocrTemplates.Count} templates" : "NULL or EMPTY");

                                    if (ocrTemplates != null && ocrTemplates.Any())
                                    {
                                        context.Logger?.Information(
                                            "✅ **HYBRID_TEMPLATE_SUCCESS**: OCR correction service created Invoice templates successfully");
                                        context.Logger?.Information(
                                            "   - **RETURNED_TEMPLATES**: {TemplateCount}", ocrTemplates.Count);
                                        foreach (var template in ocrTemplates)
                                        {
                                            context.Logger?.Information(
                                                "   - Template: '{Name}' (ID: {Id})",
                                                template.OcrTemplates?.Name ?? "NULL", template.OcrTemplates?.Id ?? 0);
                                            context.Logger?.Information(
                                                "     └── **FILE_TYPE**: {FileType}",
                                                template.FileType?.FileImporterInfos?.EntryType ?? "NULL");
                                            context.Logger?.Information(
                                                "     └── **FILE_TYPE_ID**: {FileTypeId}",
                                                template.FileType?.Id ?? 0);
                                            context.Logger?.Information(
                                                "     └── **PARTS_COUNT**: {PartsCount}",
                                                template.Parts?.Count ?? 0);
                                            context.Logger?.Information(
                                                "     └── **LINES_COUNT**: {LinesCount}",
                                                template.Lines?.Count ?? 0);
                                            context.Logger?.Information(
                                                "     └── **PDF_TEXT_ASSIGNED**: {HasPdfText} ({Length} chars)",
                                                !string.IsNullOrEmpty(template.FormattedPdfText),
                                                template.FormattedPdfText?.Length ?? 0);
                                        }

                                        // **CRITICAL**: Use GetContextTemplates method to properly assign context properties
                                        context.Logger?.Information(
                                            "🔧 **CONTEXT_PROPERTY_ASSIGNMENT**: Using GetContextTemplates pattern for OCR templates");

                                        // Apply the same context property assignment pattern as GetContextTemplates to all templates
                                        foreach (var ocrTemplate in ocrTemplates)
                                        {
                                            ocrTemplate.DocSet = context.DocSet;
                                            ocrTemplate.FilePath = context.FilePath;
                                            ocrTemplate.EmailId = context.EmailId;
                                        }

                                        context.Logger?.Information(
                                            "   - **DOCSET_ASSIGNED**: {DocSetCount} document sets to {TemplateCount} templates",
                                            context.DocSet?.Count ?? 0, ocrTemplates.Count);
                                        context.Logger?.Information(
                                            "   - **FILEPATH_ASSIGNED**: '{FilePath}' to {TemplateCount} templates",
                                            context.FilePath ?? "NULL", ocrTemplates.Count);
                                        context.Logger?.Information(
                                            "   - **EMAILID_ASSIGNED**: '{EmailId}' to {TemplateCount} templates",
                                            context.EmailId ?? "NULL", ocrTemplates.Count);

                                        // **INTEGRATION**: Add to MatchedTemplates for pipeline processing
                                        context.Logger?.Information(
                                            "🔗 **TEMPLATE_INTEGRATION_START**: Adding OCR templates to MatchedTemplates for pipeline processing");
                                        context.Logger?.Information(
                                            "   - **BEFORE_INTEGRATION_COUNT**: {Count}",
                                            context.MatchedTemplates?.Count() ?? 0);

                                        var templateList = context.MatchedTemplates?.ToList() ?? new List<Template>();
                                        templateList.AddRange(ocrTemplates);
                                        context.MatchedTemplates = templateList;

                                        context.Logger?.Information(
                                            "✅ **TEMPLATE_INTEGRATION_COMPLETE**: OCR templates successfully added to MatchedTemplates");
                                        context.Logger?.Information(
                                            "   - **AFTER_INTEGRATION_COUNT**: {Count} templates will be processed",
                                            context.MatchedTemplates.Count());
                                        context.Logger?.Information(
                                            "   - **COMPLETE_TEMPLATE_LIST**: [{TemplateNames}]",
                                            string.Join(
                                                ", ",
                                                context.MatchedTemplates.Select(
                                                    t =>
                                                        $"{t.OcrTemplates?.Name ?? "NULL"}({t.FileType?.FileImporterInfos?.EntryType ?? "NULL"})")));

                                        context.Logger?.Information(
                                            "🎯 **TEMPLATE_CREATION_SUCCESS_SUMMARY**: OCR template creation and integration completed successfully");
                                        context.Logger?.Information(
                                            "   - **NEW_TEMPLATES_COUNT**: {Count} templates created", ocrTemplates.Count);
                                        foreach (var template in ocrTemplates)
                                        {
                                            context.Logger?.Information(
                                                "   - **NEW_TEMPLATE**: '{Name}' ({Type})",
                                                template.OcrTemplates?.Name, template.FileType?.FileImporterInfos?.EntryType);
                                        }
                                        context.Logger?.Information(
                                            "   - **PIPELINE_READY**: Templates are now part of MatchedTemplates and ready for downstream processing");
                                    }
                                    else
                                    {
                                        context.Logger?.Error(
                                            "❌ **OCR_TEMPLATE_CREATION_FAILED**: CreateInvoiceTemplateAsync returned NULL or empty list");
                                        context.Logger?.Error(
                                            "   - **FAILURE_IMPACT**: No new templates will be added to MatchedTemplates");
                                        context.Logger?.Error(
                                            "   - **PIPELINE_CONTINUATION**: Will proceed with existing {Count} templates",
                                            context.MatchedTemplates?.Count() ?? 0);
                                    }
                                }

                                context.Logger?.Information(
                                    "🔍 **OCR_SERVICE_CALL_COMPLETE**: CreateInvoiceTemplateAsync call completed, OCR service disposed");
                            }
                            catch (Exception ocrEx)
                            {
                                context.Logger?.Error(
                                    ocrEx,
                                    "🚨 **OCR_INVESTIGATION_EXCEPTION**: Exception during OCR service investigation");
                                context.Logger?.Error(
                                    "   - **EXCEPTION_TYPE**: {ExceptionType}",
                                    ocrEx.GetType().FullName);
                                context.Logger?.Error("   - **EXCEPTION_MESSAGE**: {ExceptionMessage}", ocrEx.Message);
                                context.Logger?.Error("   - **STACK_TRACE**: {StackTrace}", ocrEx.StackTrace);
                                context.Logger?.Error(
                                    "   - **FAILURE_IMPACT**: Template creation failed, proceeding with existing templates only");
                            }
                        }
                        else
                        {
                            context.Logger?.Information(
                                "ℹ️ **NO_INVOICE_CONTENT**: No invoice keywords detected in PDF content");
                            context.Logger?.Information(
                                "   - **ANALYSIS_RESULT**: PDF contains templates but no invoice-specific content requiring ShipmentInvoice creation");
                            context.Logger?.Information(
                                "   - **PIPELINE_CONTINUATION**: Processing existing {TemplateCount} templates normally",
                                context.MatchedTemplates.Count());
                        }

                        context.Logger?.Information(
                            "🔚 **LOGLEVELOVERRIDE_SCOPE_END**: Surgical debugging scope ending - application will terminate");

                    }
                    else if (hasShipmentInvoiceTemplate)
                    {
                        context.Logger?.Information(
                            "ℹ️ **CONDITION_NOT_MET_SHIPMENT_EXISTS**: ShipmentInvoice template already exists");
                        context.Logger?.Information(
                            "   - **EXISTING_SHIPMENT_TEMPLATES**: {ShipmentTemplates}",
                            string.Join(
                                ", ",
                                context.MatchedTemplates
                                    .Where(
                                        t => t.FileType?.FileImporterInfos?.EntryType
                                             == FileTypeManager.EntryTypes.ShipmentInvoice)
                                    .Select(t => t.OcrTemplates?.Name)));
                        context.Logger?.Information(
                            "   - **REASON**: No template creation needed - ShipmentInvoice already present");
                    }
                    else if (string.IsNullOrEmpty(pdfTextString))
                    {
                        context.Logger?.Information("⚠️ **CONDITION_NOT_MET_NO_PDF_TEXT**: PDF text is null or empty");
                        context.Logger?.Information(
                            "   - **PDF_TEXT_STATE**: {PdfTextState}",
                            pdfTextString == null ? "NULL" : "EMPTY");
                        context.Logger?.Information(
                            "   - **REASON**: Cannot perform hybrid document analysis without PDF text");
                    }
                    else
                    {
                        context.Logger?.Information(
                            "⚠️ **CONDITION_NOT_MET_UNKNOWN**: Unknown reason why template creation condition failed");
                        context.Logger?.Information(
                            "   - **hasShipmentInvoiceTemplate**: {HasShipment}",
                            hasShipmentInvoiceTemplate);
                        context.Logger?.Information(
                            "   - **pdfTextString null or empty**: {IsEmpty}",
                            string.IsNullOrEmpty(pdfTextString));
                    }

                    context.Logger?.Information(
                        "🔍 **HYBRID_DOCUMENT_DETECTION_COMPLETE**: Analysis finished, proceeding with {FinalTemplateCount} templates",
                        context.MatchedTemplates?.Count() ?? 0);

                    // Log the identified possible invoices
                    LogPossibleInvoices(context, possibleInvoices, totalTemplateCount, filePath);

                    methodStopwatch.Stop();
                    context.Logger?.Information(
                        "METHOD_EXIT_SUCCESS: {MethodName}. IntentionAchieved: {IntentionAchievedStatus}. FinalState: [{FinalStateContext}]. Total execution time: {ExecutionDurationMs}ms.",
                        nameof(Execute),
                        "Successfully identified possible invoice templates",
                        $"PossibleInvoiceCount: {context.MatchedTemplates?.Count() ?? 0}",
                        methodStopwatch.ElapsedMilliseconds);
                    context.Logger?.Information(
                        "ACTION_END_SUCCESS: {ActionName}. Outcome: {ActionOutcome}. Total observed duration: {TotalObservedDurationMs}ms.",
                        nameof(GetPossibleInvoicesStep),
                        $"Successfully identified {context.MatchedTemplates?.Count() ?? 0} possible invoices for file: {filePath}",
                        methodStopwatch.ElapsedMilliseconds);
                    return true;
                }
                catch (Exception ex)
                {
                    methodStopwatch.Stop();
                    string errorMessage =
                        $"Error during GetPossibleInvoicesStep processing templates for File: {filePath}: {ex.Message}";
                    context.Logger?.Error(
                        ex,
                        "METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                        nameof(Execute),
                        "Identify possible invoice templates based on PDF text",
                        methodStopwatch.ElapsedMilliseconds,
                        errorMessage);
                    context.Logger?.Error(
                        ex,
                        "ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                        nameof(GetPossibleInvoicesStep),
                        "Processing templates",
                        methodStopwatch.ElapsedMilliseconds,
                        errorMessage);
                    context.AddError(errorMessage);
                    context.MatchedTemplates = new List<Template>();
                    return false;
                }
        }

        private bool ValidateContext(InvoiceProcessingContext context, string filePath)
        {
            if (context == null)
            {
                throw new ArgumentNullException(nameof(context), "GetPossibleInvoicesStep executed with null context.");
            }

            if (context.Templates == null)
            {
                context.Logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}] Expected templates for processing.",
                    nameof(ValidateContext), "Validation", "Skipping GetPossibleInvoicesStep: Templates collection is null.", $"FilePath: {filePath}");
                context.Templates = new List<Template>();
                return true; // Treat as successful validation but no work done
            }

            if (context.PdfText == null)
            {
                context.Logger?.Error("METHOD_EXIT_FAILURE: {MethodName}. IntentionAtFailure: {MethodIntention}. Execution time: {ExecutionDurationMs}ms. Error: {ErrorMessage}",
                    nameof(ValidateContext), "Validate pipeline context", 0, $"Skipping GetPossibleInvoicesStep: PdfText (StringBuilder) is null for File: {filePath}.");
                context.Logger?.Error("ACTION_END_FAILURE: {ActionName}. StageOfFailure: {StageOfFailure}. Duration: {TotalObservedDurationMs}ms. Error: {ErrorMessage}",
                    nameof(GetPossibleInvoicesStep), "Context validation", 0, $"Skipping GetPossibleInvoicesStep: PdfText (StringBuilder) is null for File: {filePath}.");
                context.AddError($"PdfText is null for file: {filePath}");
                return false; // Indicate validation failure
            }

            context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                nameof(ValidateContext), "Validation", "Context validation successful.", $"FilePath: {filePath}");
            return true;
        }

        private async Task<List<Template>> GetPossibleInvoices(InvoiceProcessingContext context, string pdfTextString, string filePath)
        {
            context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                nameof(GetPossibleInvoices), "Filtering", "Ordering templates and filtering based on PDF text match.", $"FilePath: {filePath}");

            var possibleInvoices = context.Templates
                .OrderBy(x => !(x?.OcrTemplates?.Name?.ToUpperInvariant().Contains("TROPICAL") ?? false))
                .ThenBy(x => x?.OcrTemplates?.Id ?? int.MaxValue)
                .Where(tmp =>
                {
                    if (tmp == null)
                    {
                        context.Logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                            nameof(GetPossibleInvoices), "Filtering", "Skipping null template during filtering.", "");
                        return false;
                    }

                    if (tmp.OcrTemplates == null)
                    {
                        context.Logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                            nameof(GetPossibleInvoices), "Filtering", "Skipping template with null OcrInvoices.", $"TemplateId: {tmp.OcrTemplates.Id}");
                        return false;
                    }

                    context.Logger?.Debug("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                        nameof(GetPossibleInvoices), "Filtering", "Checking template match.", $"TemplateId: {tmp.OcrTemplates.Id}, TemplateName: {tmp.OcrTemplates.Name}");

                    // Call the partial method IsInvoiceDocument
                    context.Logger?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                        $"IsInvoiceDocument for Template {tmp.OcrTemplates.Id}", "SYNC_EXPECTED");
                    var isMatchStopwatch = Stopwatch.StartNew();
                    bool isMatch = IsInvoiceDocument(tmp, pdfTextString, filePath, context.Logger);
                    isMatchStopwatch.Stop();
                    context.Logger?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                        $"IsInvoiceDocument for Template {tmp.OcrTemplates.Id}", isMatchStopwatch.ElapsedMilliseconds, "Sync call returned");

                    context.Logger?.Verbose("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                        nameof(GetPossibleInvoices), "Filtering", "Template match result.", $"TemplateId: {tmp.OcrTemplates.Id}, IsMatch: {isMatch}");
                    return isMatch;
                })
                // .Select(x =>
                // {
                //     x.CsvLines = null;
                //     x.FileType = context.FileType;
                //     x.DocSet = context.DocSet ?? WaterNut.DataSpace.Utils.GetDocSets(context.FileType);
                //     x.FilePath = context.FilePath;
                //     x.EmailId = context.EmailId;
                //     foreach (var part in x.Parts)
                //     {
                //         part.AllLines.ForEach(z => z.Values.Clear());
                //     }

                //     return x;
                // })
                .ToList()
                ;

            context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                nameof(GetPossibleInvoices), "Filtering", "Finished filtering templates.", $"PossibleInvoiceCount: {possibleInvoices.Count}, FilePath: {filePath}");

            // need to get fresh templates
            var lst = possibleInvoices.Select(x => x.OcrTemplates.Id).ToList();
            if (!lst.Any())
            {
                 context.Logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}] Expected at least one possible invoice.",
                     nameof(GetPossibleInvoices), "TemplateRefresh", "No possible invoices found, skipping template refresh.", $"FilePath: {filePath}");
                 return new List<Template>(); // Return empty list if no possible invoices
            }

            context.Logger?.Information("INVOKING_OPERATION: {OperationDescription} ({AsyncExpectation})",
                "GetTemplatesStep.GetTemplates (Refresh)", "ASYNC_EXPECTED");
            var getTemplatesStopwatch = Stopwatch.StartNew();
            var res = await GetTemplatesStep.GetTemplates(context, invoices => lst.Contains(invoices.OcrTemplates.Id)).ConfigureAwait(false);
            getTemplatesStopwatch.Stop();
            context.Logger?.Information("OPERATION_INVOKED_AND_CONTROL_RETURNED: {OperationDescription}. Initial call took {InitialCallDurationMs}ms. ({AsyncGuidance})",
                "GetTemplatesStep.GetTemplates (Refresh)", getTemplatesStopwatch.ElapsedMilliseconds, "If ASYNC_EXPECTED, this is pre-await return");

            context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                nameof(GetPossibleInvoices), "TemplateRefresh", "Refreshed possible invoice templates.", $"RefreshedCount: {res.Count}, FilePath: {filePath}");

            return res;
        }

        private void LogPossibleInvoices(InvoiceProcessingContext context, List<Template> possibleInvoices, int totalTemplateCount, string filePath)
        {
            context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}]",
                nameof(GetPossibleInvoicesStep), "Summary", "Possible invoices found.", $"PossibleInvoiceCount: {possibleInvoices.Count}, TotalTemplateCount: {totalTemplateCount}, FilePath: {filePath}");

            if (possibleInvoices.Any())
            {
                var invoiceDetails = possibleInvoices.Select(inv => new { Name = inv.OcrTemplates?.Name, Id = inv.OcrTemplates?.Id }).ToList();
                context.Logger?.Information("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}] {OptionalData}",
                    nameof(GetPossibleInvoicesStep), "Summary", "Details of possible invoices.", $"FilePath: {filePath}", new { InvoiceDetails = invoiceDetails });
            }
            else
            {
                context.Logger?.Warning("INTERNAL_STEP ({OperationName} - {Stage}): {StepMessage}. CurrentState: [{CurrentStateContext}] Expected at least one possible invoice.",
                    nameof(GetPossibleInvoicesStep), "Summary", "No possible invoices found.", $"FilePath: {filePath}");
            }
        }

        /// <summary>
        /// **INVOICE_KEYWORD_DETECTION**: Detects if PDF text contains invoice-related keywords that indicate ShipmentInvoice content.
        /// **BUSINESS_RULE**: Used to identify hybrid documents that contain both invoice and customs declaration content.
        /// **ARCHITECTURAL_INTENT**: Supports hybrid document processing by identifying when OCR template creation is needed.
        /// </summary>
        private bool ContainsInvoiceKeywords(string pdfText, ILogger logger = null)
        {
            if (string.IsNullOrEmpty(pdfText)) 
            {
                return false;
            }
            
            var invoiceKeywords = new[]
            {
                "TOTAL AMOUNT", "Subtotal", "Invoice", "Order number:", "Order Number", 
                "Invoice No", "Invoice Number", "Invoice Date", "Bill To", "Ship To",
                "Item Description", "Quantity", "Unit Price", "Line Total", "Tax",
                "Shipping", "Handling", "Grand Total", "Amount Due", "Payment",
                "Order:", "Receipt", "Purchase"
            };

            var textUpper = pdfText.ToUpperInvariant();
            var foundKeywords = new List<string>();
            
            foreach (var keyword in invoiceKeywords)
            {
                if (textUpper.Contains(keyword.ToUpperInvariant()))
                {
                    foundKeywords.Add(keyword);
                }
            }
            
            // Require at least 2 invoice keywords to reduce false positives
            var hasInvoiceKeywords = foundKeywords.Count >= 2;
            
            // Add comprehensive logging for diagnosis using passed context logger
            logger?.Information("🔍 **INVOICE_KEYWORD_DETECTION_DEBUG**: Analyzing PDF text for invoice keywords");
            logger?.Information("   - **PDF_TEXT_LENGTH**: {Length} characters", pdfText.Length);
            logger?.Information("   - **KEYWORDS_FOUND**: {Count} out of {Total}", foundKeywords.Count, invoiceKeywords.Length);
            logger?.Information("   - **FOUND_KEYWORDS**: [{Keywords}]", string.Join(", ", foundKeywords));
            logger?.Information("   - **DETECTION_THRESHOLD**: Requires >= 2 keywords");
            logger?.Information("   - **DETECTION_RESULT**: {HasKeywords}", hasInvoiceKeywords);
            
            if (hasInvoiceKeywords)
            {
                logger?.Information("✅ **INVOICE_CONTENT_DETECTED**: PDF contains sufficient invoice keywords");
            }
            else
            {
                logger?.Information("❌ **INVOICE_CONTENT_NOT_DETECTED**: PDF does not contain sufficient invoice keywords");
                logger?.Information("   - **TEXT_SAMPLE**: '{Sample}...'", pdfText.Substring(0, Math.Min(500, pdfText.Length)));
            }
            
            return hasInvoiceKeywords;
        }
    }
}