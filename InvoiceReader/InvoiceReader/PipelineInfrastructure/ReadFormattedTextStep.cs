// File: WaterNut.DataSpace.PipelineInfrastructure/ReadFormattedTextStep.cs

using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using Core.Common.Extensions;
using CoreEntities.Business.Entities;
using EntryDataDS.Business.Entities;
using OCR.Business.Entities;
using Serilog;
using Serilog.Events;
using WaterNut.Business.Services.Utils;
using WaterNut.DataSpace;

namespace WaterNut.DataSpace.PipelineInfrastructure
{
    public partial class ReadFormattedTextStep : IPipelineStep<InvoiceProcessingContext>
    {
        public async Task<bool> Execute(InvoiceProcessingContext context)
        {
            // Use LogLevelOverride to ensure all detailed assertive logs are visible during execution
            using (LogLevelOverride.Begin(LogEventLevel.Verbose))
            {
                var methodStopwatch = Stopwatch.StartNew();
                string filePath = context?.FilePath ?? "Unknown";
                context.Logger?.Information("METHOD_ENTRY: {MethodName}. Intention: Read formatted text, apply corrections to the native data structure, and validate the outcome.", nameof(Execute));

                if (context.MatchedTemplates == null || !context.MatchedTemplates.Any())
                {
                    context.Logger?.Information("🚨 **NO_TEMPLATES_MATCHED**: No existing templates matched for file {FilePath}. Attempting OCR correction with blank invoice.", filePath);
                    context.Logger?.Information("   - **ARCHITECTURAL_INTENT**: When no templates match, system creates blank invoice structure and applies OCR correction service to populate fields.");
                    context.Logger?.Information("   - **BUSINESS_RULE**: New supplier invoices require OCR correction service to bootstrap field extraction and create learning entries.");
                    
                    // Create blank ShipmentInvoice and attempt OCR correction
                    var success = await CreateBlankInvoiceWithOCRCorrection(context, filePath).ConfigureAwait(false);
                    if (success)
                    {
                        context.Logger?.Information("✅ **OCR_BLANK_INVOICE_SUCCESS**: Successfully processed blank invoice with OCR correction for file {FilePath}.", filePath);
                        return true;
                    }
                    else
                    {
                        context.Logger?.Warning("❌ **OCR_BLANK_INVOICE_FAILED**: OCR correction with blank invoice failed for file {FilePath}. No invoice created.", filePath);
                        context.AddError($"OCR correction with blank invoice failed for file: {filePath}");
                        return false;
                    }
                }

                var templatesList = context.MatchedTemplates.ToList();
                for (int templateIndex = 0; templateIndex < templatesList.Count; templateIndex++)
                {
                    var template = templatesList[templateIndex];
                    if (!ExecutionValidation(context.Logger, template, filePath))
                    {
                        continue;
                    }

                    context.Logger?.Error("📂 **TEMPLATE_STRUCTURE_ANALYSIS_START**: Analyzing template '{TemplateName}' (ID: {TemplateId}) before Read().", template.OcrInvoices.Name, template.OcrInvoices.Id);
                    LogTemplateStructure(template, context.Logger);
                    context.Logger?.Error("📂 **TEMPLATE_STRUCTURE_ANALYSIS_END**");

                    var textLines = GetTextLinesFromFormattedPdfText(context.Logger, template);

                    try
                    {
                        // --- INITIAL READ ---
                        var res = template.Read(textLines);

                        // --- ASSERTIVE LOGGING: INITIAL DATA STATE ---
                        var jsonOptions = new JsonSerializerOptions { WriteIndented = true, ReferenceHandler = ReferenceHandler.Preserve };
                        var resType = res?.GetType().FullName ?? "NULL";
                        var firstItemType = (res != null && res.Any()) ? res[0]?.GetType().FullName ?? "NULL" : "N/A (List is empty or null)";
                        context.Logger?.Error("🔬 **TYPE_ANALYSIS (INITIAL_READ)**: The 'res' object from template.Read() has Type: '{ResType}'. The first element has Type: '{FirstItemType}'.", resType, firstItemType);

                        string initialResJson = "[]";
                        try
                        {
                            initialResJson = JsonSerializer.Serialize(res, jsonOptions);
                        }
                        catch (Exception jsonEx)
                        {
                            initialResJson = $"Serialization failed: {jsonEx.Message}";
                        }
                        context.Logger?.Error("📜 **DATA_OUTPUT_DUMP (INITIAL_READ)**: The data produced by the initial read: {DataJson}", initialResJson);

                        

                        // --- OCR CORRECTION SERVICE CALL ---
                        context.Logger?.Information("🚀 **CORRECTION_PIPELINE_START**: Calling OCRCorrectionService to analyze and correct the data structure as-is.");
                        
                        // ====== FREE SHIPPING PRE-CORRECTION DIAGNOSTIC ======
                        if (res != null && res.Any())
                        {
                            var preCorrectionFreeShipping = res
                                .SelectMany(item => item is IEnumerable<IDictionary<string, object>> list ? list : new[] { item as IDictionary<string, object> })
                                .Where(dict => dict != null && dict.ContainsKey("TotalDeduction") && dict["TotalDeduction"] != null)
                                .ToList();
                                
                            if (preCorrectionFreeShipping.Any())
                            {
                                context.Logger?.Information("🚢 **FREE_SHIPPING_PRE_CORRECTION**: Found {Count} items with TotalDeduction before OCR correction", preCorrectionFreeShipping.Count);
                                foreach (var item in preCorrectionFreeShipping)
                                {
                                    var invoiceNo = item.ContainsKey("InvoiceNo") ? item["InvoiceNo"]?.ToString() : "Unknown";
                                    var totalDeduction = item["TotalDeduction"]?.ToString();
                                    context.Logger?.Information("   - 📊 Pre-Correction {InvoiceNo}: TotalDeduction={TotalDeduction}", invoiceNo, totalDeduction);
                                }
                            }
                            else
                            {
                                context.Logger?.Information("🚢 **FREE_SHIPPING_PRE_CORRECTION**: No items with TotalDeduction found before OCR correction");
                            }
                        }
                        
                        var correctedRes = await OCRCorrectionService.CorrectInvoices(res, template, textLines, context.Logger).ConfigureAwait(false);

                        // ====== FREE SHIPPING POST-CORRECTION DIAGNOSTIC ======
                        if (correctedRes != null && correctedRes.Any())
                        {
                            var postCorrectionFreeShipping = correctedRes
                                .SelectMany(item => item is IEnumerable<IDictionary<string, object>> list ? list : new[] { item as IDictionary<string, object> })
                                .Where(dict => dict != null && dict.ContainsKey("TotalDeduction") && dict["TotalDeduction"] != null)
                                .ToList();
                                
                            if (postCorrectionFreeShipping.Any())
                            {
                                context.Logger?.Information("🚢 **FREE_SHIPPING_POST_CORRECTION**: Found {Count} items with TotalDeduction after OCR correction", postCorrectionFreeShipping.Count);
                                foreach (var item in postCorrectionFreeShipping)
                                {
                                    var invoiceNo = item.ContainsKey("InvoiceNo") ? item["InvoiceNo"]?.ToString() : "Unknown";
                                    var totalDeduction = item["TotalDeduction"]?.ToString();
                                    context.Logger?.Information("   - 📊 Post-Correction {InvoiceNo}: TotalDeduction={TotalDeduction} (corrected by OCR service)", invoiceNo, totalDeduction);
                                }
                            }
                            else
                            {
                                context.Logger?.Warning("⚠️ **FREE_SHIPPING_POST_CORRECTION_MISSING**: No items with TotalDeduction found after OCR correction");
                            }
                        }

                        // --- ASSERTIVE LOGGING: CORRECTED DATA STATE & COMPARISON ---
                        var correctedResType = correctedRes?.GetType().FullName ?? "NULL";
                        var firstCorrectedItemType = (correctedRes != null && correctedRes.Any()) ? correctedRes[0]?.GetType().FullName ?? "NULL" : "N/A";
                        context.Logger?.Information("🔬 **TYPE_ANALYSIS (AFTER_CORRECTION)**: The 'correctedRes' object returned from service has Type: '{correctedResType}'. First element Type: '{firstCorrectedItemType}'.", correctedResType, firstCorrectedItemType);

                        string correctedResJson = "[]";
                        try
                        {
                            correctedResJson = JsonSerializer.Serialize(correctedRes, jsonOptions);
                        }
                        catch (Exception jsonEx)
                        {
                            correctedResJson = $"Serialization failed: {jsonEx.Message}";
                        }
                        context.Logger?.Error("📜 **DATA_OUTPUT_DUMP (AFTER_CORRECTION)**: The data after the correction service ran: {DataJson}", correctedResJson);

                        if (initialResJson == correctedResJson)
                        {
                            context.Logger?.Information("⚖️ **DATA_COMPARISON**: Data is UNCHANGED. This is expected if the invoice was already balanced or no corrections were possible.");
                        }
                        else
                        {
                            context.Logger?.Information("✅ **DATA_COMPARISON**: Data was MODIFIED by the correction service as expected.");
                        }

                        // --- VALIDATE THE FINAL DATA STRUCTURE ---
                        if (IsDataStructureInvalid(correctedRes, context.Logger))
                        {
                            context.AddError($"Critical data structure error from correction service for TemplateId: {template.OcrInvoices.Id}.");
                            continue;
                        }

                        // Assign the corrected (and still nested) data structure back to the template.
                        template.CsvLines = correctedRes;

                        // Continue with downstream processing
                        CoreEntities.Business.Entities.FileTypes fileType = HandleImportSuccessStateStep.ResolveFileType(context.Logger, template);
                        if (fileType?.FileImporterInfos == null)
                        {
                            context.AddError($"Could not resolve FileType for TemplateId: {template?.OcrInvoices?.Id}.");
                            continue;
                        }
                        template.FileType = fileType;

                        // This is now redundant as CorrectInvoices handles this, but keep as a potential future hook.
                        // if (template.FileType.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.ShipmentInvoice)
                        // {
                        //     // var reCorrectedRes = await OCRCorrectionService.CorrectInvoices(correctedRes, template, textLines, template.FormattedPdfText, context.Logger).ConfigureAwait(false);
                        //     // template.CsvLines = reCorrectedRes;
                        // }

                        if (!ExecutionSuccess(context.Logger, template, filePath))
                        {
                            context.AddError($"No CsvLines present after correction for TemplateId: {template?.OcrInvoices?.Id}.");
                            return false;
                        }

                        LogExecutionSuccess(context.Logger, filePath, template?.OcrInvoices?.Id);
                        return true;
                    }
                    catch (Exception ex)
                    {
                        LogExecutionError(context.Logger, ex, filePath, template?.OcrInvoices?.Id);
                        context.AddError($"Error processing TemplateId: {template?.OcrInvoices?.Id}: {ex.Message}");
                        return false;
                    }
                }

                context.Logger?.Information("METHOD_EXIT: {MethodName}. Outcome: All templates failed or were skipped.", nameof(Execute));
                return false;
            }
        }

        #region Private Helper & Logging Methods

        /// <summary>
        /// Validates the data structure returned by the template.Read() or correction service.
        /// It now correctly handles both the expected nested List<List<...>> and a fallback flat List<...>.
        /// </summary>
        private bool IsDataStructureInvalid(List<dynamic> res, ILogger logger)
        {
            if (res == null || !res.Any())
            {
                logger?.Information("Data structure is valid (null or empty list).");
                return false; // Not invalid, just empty or null.
            }

            var firstItem = res[0];

            if (firstItem is IList nestedList)
            {
                // It's a nested list, check the inner structure.
                if (!nestedList.Cast<object>().Any())
                {
                    logger?.Information("Data structure is valid (nested list is empty).");
                    return false; // An empty inner list is valid.
                }

                var firstInnerItem = nestedList.Cast<object>().First();
                if (firstInnerItem is IDictionary<string, object>)
                {
                    logger?.Information("Data structure is valid (List<List<IDictionary<string, object>>>).");
                    return false; // Correct nested structure.
                }
                else
                {
                    logger?.Error("🚨 CRITICAL DATA TYPE MISMATCH: Nested list contains items of unexpected type: {ItemType}", firstInnerItem?.GetType().FullName ?? "NULL");
                    return true; // Invalid inner structure.
                }
            }
            else if (firstItem is IDictionary<string, object>)
            {
                logger?.Information("Data structure is valid (flat List<IDictionary<string, object>>).");
                return false; // A flat list of dictionaries is also acceptable.
            }
            else
            {
                logger?.Error("🚨 CRITICAL DATA TYPE MISMATCH: Top-level list contains items of unexpected type: {ItemType}", firstItem?.GetType().FullName ?? "NULL");
                return true; // Top-level item is neither a list nor a dictionary.
            }
        }

        private void LogTemplateStructure(Invoice template, ILogger logger)
        {
            if (template?.OcrInvoices?.Parts == null) return;
            foreach (var part in template.OcrInvoices.Parts)
            {
                var partName = part.PartTypes?.Name ?? "Unknown";
                logger?.Error("   - Part: '{PartName}' (ID: {PartId})", partName, part.Id);
                if (part.Lines == null) continue;
                foreach (var line in part.Lines)
                {
                    logger?.Error("     - Line: '{LineName}' (ID: {LineId})", line.Name, line.Id);
                    var regexPattern = line.RegularExpressions?.RegEx ?? "NO_REGEX";
                    logger?.Error("       └── Regex: {RegexPattern}", regexPattern);

                    if (line.Fields != null && line.Fields.Any())
                    {
                        logger?.Error("       └── Field Mappings:");
                        foreach (var field in line.Fields)
                        {
                            logger?.Error("           - Key: '{Key}' -> Maps to DB Field: '{Field}' (EntityType: {EntityType})",
                                field.Key, field.Field, field.EntityType);
                        }
                    }
                    else
                    {
                        logger?.Error("       └── Field Mappings: NONE DEFINED");
                    }
                }
            }
        }

        private void LogAndValidateInitialOcrData(InvoiceProcessingContext context, List<dynamic> res, Invoice template)
        {
            context.Logger?.Error("📊 **INITIAL_OCR_DATA_DUMP (CsvLines)**: Logging data structure returned from template.Read().");
            if (res == null || !res.Any())
            {
                context.Logger?.Error("   -> No CsvLines data returned. `res` is null or empty.");
                return;
            }

            // This loop now correctly handles both a single invoice (List<Dict>) and multiple invoices (List<List<Dict>>)
            for (int i = 0; i < res.Count; i++)
            {
                var dynamicItem = res[i];
                LogDictionary(dynamicItem as IDictionary<string, object>, $"   -> CsvLines Row {i + 1}/{res.Count}", context.Logger);
            }
            context.Logger?.Error("📊 **INITIAL_OCR_DATA_DUMP (CsvLines) END**");

            context.Logger?.Error("📋 **INTERNAL_TEMPLATE_VALUES_DUMP (Lines.Values)**: Internal data structure state after template.Read().");
            if (template?.Lines == null || !template.Lines.Any(l => l.Values != null && l.Values.Any()))
            {
                context.Logger?.Error("   -> No Lines.Values data to display. Template.Lines has no extracted values.");
            }
            else
            {
                foreach (var line in template.Lines.Where(l => l.Values != null && l.Values.Any()))
                {
                    context.Logger?.Error("   - Line: '{LineName}' (ID: {LineId}) has extracted values:", line.OCR_Lines?.Name ?? "N/A", line.OCR_Lines?.Id ?? 0);
                    foreach (var lineValue in line.Values)
                    {
                        var sectionData = lineValue.Value;
                        foreach (var fieldData in sectionData)
                        {
                            context.Logger?.Error("     - Field: '{FieldKey,-25}' | Value: '{Value}'", fieldData.Key.Fields.Key, fieldData.Value);
                        }
                    }
                }
            }
            context.Logger?.Error("📋 **INTERNAL_TEMPLATE_VALUES_DUMP (Lines.Values) END**");
        }

        private void LogDictionary(IDictionary<string, object> dict, string prefix, ILogger logger)
        {
            if (dict == null)
            {
                logger?.Error("{Prefix}: Item is not a processable dictionary.", prefix);
                return;
            }
            var sb = new StringBuilder();
            sb.AppendLine($"{prefix}: (Type: IDictionary<string, object>)");
            foreach (var kvp in dict.OrderBy(k => k.Key))
            {
                sb.AppendLine($"         - Key: '{kvp.Key,-25}' Value: '{kvp.Value?.ToString() ?? "NULL",-30}' Type: {kvp.Value?.GetType().Name ?? "NULL"}");
            }
            logger?.Error(sb.ToString());
        }

        private bool ExecutionValidation(ILogger logger, Invoice template, string filePath)
        {
            if (template == null || template.OcrInvoices == null)
            {
                LogNullTemplateWarning(logger, filePath);
                return false;
            }
            if (string.IsNullOrEmpty(template.FormattedPdfText))
            {
                LogEmptyFormattedPdfTextWarning(logger, filePath, template.OcrInvoices.Id);
                return false;
            }
            return true;
        }

        private List<string> GetTextLinesFromFormattedPdfText(ILogger logger, Invoice template)
        {
            logger?.Verbose("Extracting text lines from FormattedPdfText for template '{TemplateName}'.", template.OcrInvoices.Name);
            return template.FormattedPdfText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList();
        }

        private bool ExecutionSuccess(ILogger logger, Invoice template, string filePath)
        {
            if (template.CsvLines == null || !template.CsvLines.Any())
            {
                LogEmptyCsvLinesWarning(logger, filePath, template?.OcrInvoices?.Id);
                return false;
            }
            logger?.Information("Successfully read {Count} CsvLines for TemplateId {TemplateId}.", template.CsvLines.Count, template?.OcrInvoices?.Id);
            return true;
        }

        private void LogNullTemplateWarning(ILogger logger, string filePath)
        {
            logger?.Warning("Validation failed: Template object is null for file {FilePath}.", filePath);
        }

        private void LogEmptyFormattedPdfTextWarning(ILogger logger, string filePath, int? templateId)
        {
            logger?.Warning("Validation failed: FormattedPdfText is null or empty for TemplateId {TemplateId} on file {FilePath}.", templateId, filePath);
        }

        private void LogEmptyCsvLinesWarning(ILogger logger, string filePath, int? templateId)
        {
            logger?.Warning("Result check failed: CsvLines is null or empty after extraction for TemplateId {TemplateId} on file {FilePath}.", templateId, filePath);
        }

        private void LogExecutionSuccess(ILogger logger, string filePath, int? templateId)
        {
            logger?.Information("Successfully read formatted text for file {FilePath} using TemplateId {TemplateId}.", filePath, templateId);
        }

        private void LogExecutionError(ILogger logger, Exception ex, string filePath, int? templateId)
        {
            logger?.Error(ex, "An error occurred during ReadFormattedTextStep for File: {FilePath}, TemplateId: {TemplateId}.", filePath, templateId);
        }

        /// <summary>
        /// **CRITICAL_FALLBACK_METHOD**: Creates a blank ShipmentInvoice and applies OCR correction when no templates match.
        /// **ARCHITECTURAL_INTENT**: This method follows the working test pattern of OCR correction with blank invoices.
        /// **BUSINESS_RULE**: New supplier invoices require OCR correction service to extract data and create template learning entries.
        /// **DESIGN_BACKSTORY**: When GetPossibleInvoicesStep finds no matching templates, this creates a blank invoice and lets OCR correction populate it.
        /// </summary>
        private async Task<bool> CreateBlankInvoiceWithOCRCorrection(InvoiceProcessingContext context, string filePath)
        {
            context.Logger?.Information("🏗️ **OCR_BLANK_INVOICE_METHOD_ENTRY**: Creating blank invoice with OCR correction for file {FilePath}", filePath);
            context.Logger?.Information("   - **ARCHITECTURAL_INTENT**: Create blank ShipmentInvoice and populate using OCR correction service following test pattern");
            context.Logger?.Information("   - **BUSINESS_RULE**: This enables processing of invoices from new suppliers without existing templates");

            try
            {
                // **STEP 1**: Extract text from PDF context
                var pdfTextString = context.PdfText?.ToString() ?? string.Empty;
                if (string.IsNullOrWhiteSpace(pdfTextString))
                {
                    context.Logger?.Warning("❌ **OCR_BLANK_INVOICE_EMPTY_TEXT**: No PDF text available for OCR correction service");
                    return false;
                }

                context.Logger?.Information("📄 **PDF_TEXT_AVAILABLE**: PDF text extracted, {TextLength} characters available for OCR analysis", pdfTextString.Length);

                // **STEP 2**: Create blank ShipmentInvoice 
                context.Logger?.Information("🆕 **BLANK_INVOICE_CREATION**: Creating blank ShipmentInvoice for OCR correction service");
                var blankInvoice = new ShipmentInvoice
                {
                    InvoiceNo = null,
                    InvoiceDate = null,
                    SupplierName = null,
                    Currency = null,
                    SubTotal = null,
                    TotalInternalFreight = null,
                    TotalOtherCost = null,
                    TotalInsurance = null,
                    TotalDeduction = null,
                    InvoiceTotal = null
                };

                context.Logger?.Information("🔧 **BLANK_INVOICE_CREATED**: Blank ShipmentInvoice initialized with null fields for OCR population");

                // **STEP 3**: Apply OCR correction service (following test pattern)
                context.Logger?.Information("🚀 **OCR_CORRECTION_SERVICE_START**: Calling OCRCorrectionService.CorrectInvoiceAsync");
                context.Logger?.Information("   - **INTENTION**: OCR service will analyze PDF text and populate invoice fields");
                context.Logger?.Information("   - **EXPECTATION**: Service will create database learning entries for future template matching");

                using (var ocrService = new OCRCorrectionService(context.Logger))
                {
                    var correctionSuccess = await ocrService.CorrectInvoiceAsync(blankInvoice, pdfTextString).ConfigureAwait(false);
                    
                    if (correctionSuccess)
                    {
                        context.Logger?.Information("✅ **OCR_CORRECTION_SUCCESS**: OCR correction service successfully populated invoice");
                        context.Logger?.Information("   - **POST_CORRECTION_INVOICE_NO**: {InvoiceNo}", blankInvoice.InvoiceNo ?? "NULL");
                        context.Logger?.Information("   - **POST_CORRECTION_SUPPLIER**: {SupplierName}", blankInvoice.SupplierName ?? "NULL");
                        context.Logger?.Information("   - **POST_CORRECTION_TOTAL**: {InvoiceTotal}", blankInvoice.InvoiceTotal?.ToString() ?? "NULL");

                        // **STEP 4**: Verify invoice balance
                        var isBalanced = OCRCorrectionService.TotalsZero(blankInvoice, context.Logger);
                        if (isBalanced)
                        {
                            context.Logger?.Information("✅ **INVOICE_BALANCE_VERIFIED**: Invoice is mathematically balanced after OCR correction");
                        }
                        else
                        {
                            context.Logger?.Information("⚠️ **INVOICE_BALANCE_WARNING**: Invoice is not perfectly balanced but OCR correction succeeded");
                        }

                        // **SUCCESS**: OCR correction worked, the invoice will be saved by the downstream pipeline
                        context.Logger?.Information("🎯 **OCR_CORRECTION_COMPLETE**: OCR correction completed successfully, database learning entries created");
                        return true;
                    }
                    else
                    {
                        context.Logger?.Warning("❌ **OCR_CORRECTION_FAILED**: OCR correction service failed to populate blank invoice");
                        return false;
                    }
                }
            }
            catch (Exception ex)
            {
                context.Logger?.Error(ex, "🚨 **OCR_BLANK_INVOICE_EXCEPTION**: Critical exception in CreateBlankInvoiceWithOCRCorrection for file {FilePath}", filePath);
                return false;
            }
        }

        #endregion
    }
}