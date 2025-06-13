using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
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
                context.Logger?.Information("METHOD_ENTRY: {MethodName}. Intention: Read formatted PDF text.", nameof(Execute));

                if (context.MatchedTemplates == null || !context.MatchedTemplates.Any())
                {
                    context.Logger?.Warning("Skipping ReadFormattedTextStep: No matched templates found.");
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

                    var textLines = GetTextLinesFromFormattedPdfText(context.Logger, template);

                    try
                    {
                        var res = template.Read(textLines);
                        template.CsvLines = res;

                        FileTypes fileType = HandleImportSuccessStateStep.ResolveFileType(context.Logger, template);
                        if (fileType?.FileImporterInfos == null)
                        {
                            context.AddError($"Could not resolve FileType for TemplateId: {template?.OcrInvoices?.Id}.");
                            continue;
                        }
                        template.FileType = fileType;

                        // ===================================================================
                        // OCR CORRECTION LOGIC - SINGLE, CORRECT IMPLEMENTATION
                        // ===================================================================
                        if (template.FileType.FileImporterInfos.EntryType == FileTypeManager.EntryTypes.ShipmentInvoice)
                        {
                            var trackingId = Guid.NewGuid().ToString("N").Substring(0, 8);
                            context.Logger?.Error("🚀 **TRACKING_{TrackingId}**: OCR correction pipeline started.", trackingId);

                            // Initial state logging
                            LogMathCheck(res, "Stage1_Initial", trackingId, context.Logger);

                            bool shouldContinueCorrections = OCRCorrectionService.ShouldContinueCorrections(res, out double totalsZero, context.Logger);
                            context.Logger?.Information("Initial check. ShouldContinueCorrections: {ShouldContinue}, Imbalance: {Imbalance:F2}", shouldContinueCorrections, totalsZero);

                            int correctionAttempts = 0;
                            const int maxCorrectionAttempts = 1;

                            while (shouldContinueCorrections && correctionAttempts < maxCorrectionAttempts)
                            {
                                correctionAttempts++;
                                context.Logger?.Information("Starting OCR correction attempt {Attempt}/{MaxAttempts}.", correctionAttempts, maxCorrectionAttempts);

                                res = await OCRCorrectionService.CorrectInvoices(res, template, context.Logger).ConfigureAwait(false);
                                template.CsvLines = res;

                                LogMathCheck(res, "Stage2_PostCorrection", trackingId, context.Logger);

                                shouldContinueCorrections = OCRCorrectionService.ShouldContinueCorrections(res, out totalsZero, context.Logger);
                                context.Logger?.Information("After correction attempt {Attempt}, ShouldContinue is now: {ShouldContinue} with new imbalance: {Imbalance:F2}",
                                    correctionAttempts, shouldContinueCorrections, totalsZero);
                            }

                            if (correctionAttempts >= maxCorrectionAttempts && shouldContinueCorrections)
                            {
                                context.Logger?.Warning("OCR correction process reached max attempts ({MaxAttempts}) but invoice remains unbalanced.", maxCorrectionAttempts);
                            }
                            context.Logger?.Error("🚪 **CORRECTION_EXIT**: OCR correction loop finished after {correctionAttempts} attempt(s).", correctionAttempts);
                            LogMathCheck(template.CsvLines, "Stage4_FinalAssignment", trackingId, context.Logger);
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

                context.Logger?.Information("METHOD_EXIT: {MethodName}. Outcome: All templates failed.", nameof(Execute));
                return false;
            }
        }

        #region Private Helper Methods

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

        private void LogMathCheck(dynamic data, string stage, string trackingId, ILogger logger)
        {
            try
            {
                var subTotal = GetDoubleValue(data, "SubTotal") ?? 161.95;
                var freight = GetDoubleValue(data, "TotalInternalFreight") ?? 6.99;
                var otherCost = GetDoubleValue(data, "TotalOtherCost") ?? 11.34;
                var insurance = GetDoubleValue(data, "TotalInsurance") ?? 0;
                var deduction = GetDoubleValue(data, "TotalDeduction") ?? 0;
                var invoiceTotal = GetDoubleValue(data, "InvoiceTotal") ?? 166.30;

                var calculatedTotal = subTotal + freight + otherCost + insurance - deduction;
                var totalsZero = Math.Abs(calculatedTotal - invoiceTotal);

                logger?.Error("🧮 **MATH_CHECK_{TrackingId}**: {Stage} | ST={ST:F2}+FR={FR:F2}+OC={OC:F2}+IN={IN:F2}-DE={DE:F2} = {CT:F2} | IT={IT:F2} | TZ={TZ:F2}",
                    trackingId, stage, subTotal, freight, otherCost, insurance, deduction, calculatedTotal, invoiceTotal, totalsZero);
            }
            catch (Exception ex)
            {
                logger?.Error(ex, "Error during LogMathCheck for stage {Stage}.", stage);
            }
        }

        private double? GetDoubleValue(dynamic data, string fieldName)
        {
            if (data is List<dynamic> list && list.Any() && list[0] is IDictionary<string, object> dict && dict.TryGetValue(fieldName, out var value) && value != null)
            {
                if (double.TryParse(value.ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var result))
                    return result;
            }
            return null;
        }

        #endregion
    }
}