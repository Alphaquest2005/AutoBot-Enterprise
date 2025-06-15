using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;
using Core.Common.Extensions;
using CoreEntities.Business.Entities;
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
            using (LogLevelOverride.Begin(LogEventLevel.Verbose))
            {
                var methodStopwatch = Stopwatch.StartNew();
                string filePath = context?.FilePath ?? "Unknown";
                context.Logger?.Information("METHOD_ENTRY: {MethodName}. Intention: Read formatted PDF text and apply OCR corrections if necessary.", nameof(Execute));

                if (context.MatchedTemplates == null || !context.MatchedTemplates.Any())
                {
                    context.Logger?.Warning("Skipping ReadFormattedTextStep: No matched templates found for file {FilePath}.", filePath);
                    return true;
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
                        var res = template.Read(textLines);

                        // --- LOGGING AND VALIDATION OF RAW DATA ---
                        var resType = res?.GetType().FullName ?? "NULL";
                        var firstItemType = (res != null && res.Any()) ? res[0]?.GetType().FullName ?? "NULL" : "N/A (List is empty or null)";
                        context.Logger?.Error("🔬 **TYPE_ANALYSIS**: The 'res' object from template.Read() has Type: '{ResType}'. The first element has Type: '{FirstItemType}'.", resType, firstItemType);

                        try
                        {
                            var jsonOptions = new JsonSerializerOptions { WriteIndented = true };
                            string resAsJson = JsonSerializer.Serialize(res, jsonOptions);
                            context.Logger?.Error("📜 **JSON_SERIALIZATION_DUMP**: Full 'res' object serialized to JSON:\n{JsonOutput}", resAsJson);
                        }
                        catch (Exception jsonEx)
                        {
                            context.Logger?.Error(jsonEx, "📜 **JSON_SERIALIZATION_FAILED**: Could not serialize the 'res' object.");
                        }

                        // --- DEFINITIVE BUG FIX: FLATTEN NESTED LIST ---
                        if (res != null && res.Any() && res[0] is IList nestedList)
                        {
                            var flattenedList = new List<dynamic>();
                            foreach (var item in nestedList)
                            {
                                flattenedList.Add(item);
                            }

                            context.Logger?.Error("🚨 **NESTED_LIST_DETECTED**: template.Read() returned a nested list. Flattening structure from List<List<...>> to List<...> to correct the bug.");
                            res = flattenedList;

                            var newResType = res?.GetType().FullName ?? "NULL";
                            var newFirstItemType = (res != null && res.Any()) ? res[0]?.GetType().FullName ?? "NULL" : "N/A";
                            context.Logger?.Error("✅ **FLATTENING_COMPLETE**: The 'res' object is now Type: '{NewResType}'. The first element is now Type: '{NewFirstItemType}'.", newResType, newFirstItemType);
                        }

                        LogAndValidateInitialOcrData(context, res, template);

                        if (IsDataStructureInvalid(res))
                        {
                            var typeName = (res != null && res.Any()) ? res[0]?.GetType().FullName : "N/A";
                            context.Logger?.Error("🚨🚨🚨 CRITICAL DATA TYPE MISMATCH: After potential flattening, the data structure is still invalid. First item type: {ItemType}. Aborting.", typeName);
                            context.AddError($"Critical data structure error from template.Read() for TemplateId: {template.OcrInvoices.Id}.");
                            continue;
                        }

                        template.CsvLines = res;

                        FileTypes fileType = HandleImportSuccessStateStep.ResolveFileType(context.Logger, template);
                        if (fileType?.FileImporterInfos == null)
                        {
                            context.AddError($"Could not resolve FileType for TemplateId: {template?.OcrInvoices?.Id}.");
                            continue;
                        }
                        template.FileType = fileType;

                        if (template.FileType.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.ShipmentInvoice)
                        {
                            var correctedRes = await OCRCorrectionService.CorrectInvoices(res, template, textLines, template.FormattedPdfText, context.Logger).ConfigureAwait(false);
                            template.CsvLines = correctedRes;
                        }

                        if (!ExecutionSuccess(context.Logger, template, filePath))
                        {
                            context.AddError($"No CsvLines generated for TemplateId: {template?.OcrInvoices?.Id}.");
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

        private bool IsDataStructureInvalid(List<dynamic> res)
        {
            if (res == null || !res.Any()) return false; // Not invalid, just empty
            return !(res[0] is IDictionary<string, object>);
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

        #endregion
    }
}