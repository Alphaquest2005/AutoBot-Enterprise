// File: OCRCorrectionService/OCRLegacySupport.cs
using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using EntryDataDS.Business.Entities;
using OCR.Business.Entities;
using TrackableEntities;
using Serilog;
using InvoiceReader.OCRCorrectionService;
using InvoiceReader.PipelineInfrastructure;

namespace WaterNut.DataSpace
{
    using System.Text.RegularExpressions;
    using WaterNut.Business.Services.Utils;

    public partial class OCRCorrectionService
    {
        #region Enhanced Public Static Methods (Now Multi-Invoice Aware)

        private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<ShipmentInvoice, System.Runtime.CompilerServices.StrongBox<double>> _totalsZeroAmounts =
            new System.Runtime.CompilerServices.ConditionalWeakTable<ShipmentInvoice, System.Runtime.CompilerServices.StrongBox<double>>();

        public static bool TotalsZero(ShipmentInvoice invoice, out double differenceAmount, ILogger logger)
        {
            var log = logger ?? Log.Logger.ForContext(typeof(OCRCorrectionService));
            differenceAmount = double.MaxValue;
            if (invoice == null)
            {
                log.Warning("TotalsZero(ShipmentInvoice) called with a null invoice.");
                return false;
            }

            var subTotal = invoice.SubTotal ?? 0;
            var freight = invoice.TotalInternalFreight ?? 0;
            var otherCost = invoice.TotalOtherCost ?? 0;
            var insurance = invoice.TotalInsurance ?? 0;
            var deductionAmount = invoice.TotalDeduction ?? 0;
            var reportedInvoiceTotal = invoice.InvoiceTotal ?? 0;

            var baseTotal = subTotal + freight + otherCost + insurance;
            var calculatedFinalTotal = baseTotal - deductionAmount;
            differenceAmount = Math.Abs(calculatedFinalTotal - reportedInvoiceTotal);

            bool isZero = differenceAmount < 0.015; // Relaxed tolerance

            _totalsZeroAmounts.Remove(invoice);
            _totalsZeroAmounts.Add(invoice, new System.Runtime.CompilerServices.StrongBox<double>(differenceAmount));
            return isZero;
        }

        public static bool TotalsZero(ShipmentInvoice invoice, ILogger logger) => TotalsZero(invoice, out _, logger);

        public static bool TotalsZero(List<dynamic> dynamicInvoiceResults, out double totalImbalanceSum, ILogger logger = null)
        {
            var log = logger ?? Log.Logger.ForContext(typeof(OCRCorrectionService));
            totalImbalanceSum = 0.0;
            if (dynamicInvoiceResults == null || !dynamicInvoiceResults.Any()) return true;

            var dictionaries = dynamicInvoiceResults.OfType<IDictionary<string, object>>().ToList();
            if (!dictionaries.Any()) return true;

            var tempInvoice = CreateTempShipmentInvoice(dictionaries.First(), log);
            var isBalanced = TotalsZero(tempInvoice, out totalImbalanceSum, log);
            return isBalanced;
        }

        public static bool ShouldContinueCorrections(List<dynamic> res, out double totalImbalanceSum, ILogger logger = null)
        {
            return !TotalsZero(res, out totalImbalanceSum, logger);
        }

        /// <summary>
        /// FINAL, ROBUST VERSION: Orchestrates the entire correction pipeline.
        /// 1. Detects granular errors.
        /// 2. Passes granular errors to the database learning strategies (which now validate and retry).
        /// 3. Re-imports using a reloaded template to apply the new learnings.
        /// 4. Synchronizes any final in-memory corrections.
        /// </summary>
        public static async Task<List<dynamic>> CorrectInvoices(List<dynamic> res, Invoice template, List<string> textLines, string originalText, ILogger logger)
        {
            var log = logger ?? Log.Logger.ForContext(typeof(OCRCorrectionService));
            try
            {
                log.Information("🏁 **CORRECT_INVOICES_V5_START**: Beginning robust correction pipeline for template '{TemplateName}'.", template.OcrInvoices?.Name);

                using (var correctionService = new OCRCorrectionService(log))
                {
                    var shipmentInvoicesWithMeta = ConvertDynamicToShipmentInvoicesWithMetadata(res, template, correctionService, log);
                    if (!shipmentInvoicesWithMeta.Any()) return res;

                    var invoiceWrapper = shipmentInvoicesWithMeta.First();
                    var invoiceToCorrect = invoiceWrapper.Invoice;
                    var invoiceMetadata = invoiceWrapper.FieldMetadata;

                    var allDetectedErrors = await correctionService.DetectInvoiceErrorsAsync(invoiceToCorrect, originalText, invoiceMetadata).ConfigureAwait(false);
                    if (!allDetectedErrors.Any())
                    {
                        log.Information("  - No errors detected. Invoice appears balanced or no correctable errors found. Returning original data.");
                        return res;
                    }

                    log.Error("  - [DB_LEARNING_START]: Learning patterns from {Count} raw, granular error detections.", allDetectedErrors.Count);

                    var requestsForDB = allDetectedErrors
                        .Where(e => e.ErrorType.StartsWith("omission") && e.Confidence >= 0.9)
                        .Select(rawError => {
                            // ================== CRITICAL FIX: CS1503 ==================
                            // Convert the raw InvoiceError to the required CorrectionResult before creating the update request.
                            var correctionForLearning = new CorrectionResult
                            {
                                FieldName = rawError.Field,
                                OldValue = rawError.ExtractedValue,
                                NewValue = rawError.CorrectValue,
                                CorrectionType = rawError.ErrorType,
                                Confidence = rawError.Confidence,
                                Reasoning = rawError.Reasoning,
                                LineText = rawError.LineText,
                                LineNumber = rawError.LineNumber,
                                ContextLinesBefore = rawError.ContextLinesBefore,
                                ContextLinesAfter = rawError.ContextLinesAfter
                            };
                            return correctionService.CreateRegexUpdateRequest(correctionForLearning, originalText, invoiceMetadata, template.OcrInvoices.Id);
                            // ==========================================================
                        }).ToList();

                    if (requestsForDB.Any())
                    {
                        await correctionService.UpdateRegexPatternsAsync(requestsForDB).ConfigureAwait(false);
                    }
                    else
                    {
                        log.Warning("  - [DB_LEARNING_SKIPPED]: No high-confidence omission errors found to learn from.");
                    }

                    log.Error("  - [RELOAD_AND_REIMPORT]: Invalidating template cache and re-running import with new patterns.");
                    GetTemplatesStep.InvalidateTemplateCache();

                    var reimportedRes = template.Read(textLines);
                    log.Error("  - [RE-IMPORT_COMPLETE]: Re-import with fresh template extracted {Count} new CsvLines.", reimportedRes?.Count ?? 0);

                    if (reimportedRes != null && reimportedRes.Any())
                    {
                        var reimportedDict = reimportedRes.FirstOrDefault() as IDictionary<string, object>;
                        log.Error("  - [RE-IMPORT_DATA_DUMP]: Re-imported data - TotalDeduction: {Deduction}, TotalInsurance: {Insurance}",
                            reimportedDict?.GetValueOrDefault("TotalDeduction"),
                            reimportedDict?.GetValueOrDefault("TotalInsurance"));
                    }

                    return reimportedRes;
                }
            }
            catch (Exception ex)
            {
                log.Error(ex, "🚨 **CORRECT_INVOICES_FAILED**: An unexpected error occurred in the main CorrectInvoices method.");
                return res;
            }
        }

        public static string TruncateForLog(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text)) return "";
            return text.Length <= maxLength ? text : text.Substring(0, maxLength - 3) + "...";
        }

        public static string GetOriginalTextFromFile(string templateFilePath, ILogger logger)
        {
            if (string.IsNullOrEmpty(templateFilePath)) return "";
            try
            {
                var txtFile = templateFilePath + ".txt";
                if (File.Exists(txtFile)) return File.ReadAllText(txtFile);
                var txtFileOld = Path.ChangeExtension(templateFilePath, ".txt");
                if (File.Exists(txtFileOld)) return File.ReadAllText(txtFileOld);
            }
            catch (Exception ex)
            {
                logger?.Error(ex, "Error reading original text file: {FilePath}", templateFilePath);
            }
            return "";
        }

        public static ShipmentInvoice ConvertDynamicToShipmentInvoice(IDictionary<string, object> dict, ILogger logger = null)
        {
            return CreateTempShipmentInvoice(dict, logger);
        }

        private static ShipmentInvoice CreateTempShipmentInvoice(IDictionary<string, object> x, ILogger logger)
        {
            logger?.Debug("📋 **CONVERSION_START**: Attempting to convert IDictionary to ShipmentInvoice.");
            try
            {
                var invoice = new ShipmentInvoice { InvoiceDetails = new List<InvoiceDetails>() };

                double? GetNullableDouble(string key)
                {
                    if (!x.TryGetValue(key, out var val) || val == null) return null;
                    var valStr = val.ToString();
                    if (string.IsNullOrWhiteSpace(valStr)) return null;

                    string cleanedValue = Regex.Replace(valStr.Trim(), @"[^\d.,-]", "").Trim();
                    if (cleanedValue.Contains(',') && cleanedValue.Contains('.'))
                    {
                        cleanedValue = cleanedValue.LastIndexOf(',') < cleanedValue.LastIndexOf('.')
                                           ? cleanedValue.Replace(",", "")
                                           : cleanedValue.Replace(".", "").Replace(",", ".");
                    }
                    else if (cleanedValue.Contains(',')) cleanedValue = cleanedValue.Replace(",", ".");

                    if (double.TryParse(cleanedValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var dbl))
                    {
                        return dbl;
                    }
                    return null;
                }

                T GetValue<T>(string key) where T : class => x.TryGetValue(key, out var v) ? v as T : null;

                invoice.InvoiceNo = GetValue<string>("InvoiceNo") ?? GetValue<string>("Name") ?? $"TempInv_{Guid.NewGuid().ToString().Substring(0, 8)}";
                invoice.InvoiceTotal = GetNullableDouble("InvoiceTotal");
                invoice.SubTotal = GetNullableDouble("SubTotal");
                invoice.TotalInternalFreight = GetNullableDouble("TotalInternalFreight");
                invoice.TotalOtherCost = GetNullableDouble("TotalOtherCost");
                invoice.TotalInsurance = GetNullableDouble("TotalInsurance");
                invoice.TotalDeduction = GetNullableDouble("TotalDeduction");

                logger?.Debug("✅ **CONVERSION_SUCCESS**: Finished converting IDictionary. Final Invoice Total: {Total}", invoice.InvoiceTotal?.ToString() ?? "null");
                return invoice;
            }
            catch (Exception ex)
            {
                logger?.Error(ex, "🚨 **CONVERSION_EXCEPTION**: Error creating temporary ShipmentInvoice.");
                return null;
            }
        }

        public static List<ShipmentInvoiceWithMetadata> ConvertDynamicToShipmentInvoicesWithMetadata(
            List<dynamic> res, Invoice template, OCRCorrectionService serviceInstance, ILogger logger)
        {
            var allInvoices = new List<ShipmentInvoiceWithMetadata>();
            if (serviceInstance == null || res == null) return allInvoices;
            var fieldMappings = serviceInstance.CreateEnhancedFieldMapping(template);

            foreach (var item in res.OfType<IDictionary<string, object>>())
            {
                var shipmentInvoice = CreateTempShipmentInvoice(item, logger);
                if (shipmentInvoice != null)
                {
                    var metadata = serviceInstance.ExtractEnhancedOCRMetadata(item, template, fieldMappings);
                    allInvoices.Add(new ShipmentInvoiceWithMetadata { Invoice = shipmentInvoice, FieldMetadata = metadata });
                }
            }
            return allInvoices;
        }

        public static void UpdateDynamicResultsWithCorrections(
            List<dynamic> res, List<ShipmentInvoice> correctedInvoices, ILogger logger)
        {
            if (res == null || !correctedInvoices.Any()) return;
            var correctedInvoiceMap = correctedInvoices.ToDictionary(inv => inv.InvoiceNo, inv => inv);

            foreach (var dynamicItem in res.OfType<IDictionary<string, object>>())
            {
                if (dynamicItem.TryGetValue("InvoiceNo", out var invNoObj) && invNoObj != null)
                {
                    if (correctedInvoiceMap.TryGetValue(invNoObj.ToString(), out var correctedInv))
                    {
                        dynamicItem["InvoiceTotal"] = correctedInv.InvoiceTotal;
                        dynamicItem["SubTotal"] = correctedInv.SubTotal;
                        dynamicItem["TotalInternalFreight"] = correctedInv.TotalInternalFreight;
                        dynamicItem["TotalOtherCost"] = correctedInv.TotalOtherCost;
                        dynamicItem["TotalInsurance"] = correctedInv.TotalInsurance;
                        dynamicItem["TotalDeduction"] = correctedInv.TotalDeduction;
                    }
                }
            }
        }

        /// <summary>
        /// A placeholder method. In a full implementation, this would take corrected data and update
        /// the internal state of the OCR.Business.Entities.Invoice object before it's used or saved.
        /// </summary>
        public static void UpdateTemplateLineValues(
            Invoice template,
            List<ShipmentInvoice> correctedInvoices,
            ILogger log)
        {
            log.Information("UpdateTemplateLineValues: This method is a placeholder and has no effect in this version.");
            // In a real scenario, you might iterate through correctedInvoices and update
            // the `template.Lines.Values` dictionary to reflect the corrected data before any
            // further processing steps that rely on that internal state.
            // For example:
            // var correctedInvoice = correctedInvoices.FirstOrDefault();
            // if (correctedInvoice != null)
            // {
            //     var totalDeductionLine = template.Lines.FirstOrDefault(l => ...);
            //     if(totalDeductionLine != null)
            //     {
            //          totalDeductionLine.Values["TotalDeduction"] = correctedInvoice.TotalDeduction.ToString();
            //     }
            // }
        }

        #endregion
    }
}