
﻿// File: OCRCorrectionService/OCRLegacySupport.cs

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
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Text.RegularExpressions;
using WaterNut.Business.Services.Utils;
using Core.Common.Extensions;
using WaterNut.DataSpace.PipelineInfrastructure;

namespace WaterNut.DataSpace
{
    using System.Linq;

    using global::EntryDataDS.Business.Entities;
    using InvoiceReader.OCRCorrectionService;
    using InvoiceReader.PipelineInfrastructure;
    using MoreLinq;

    using Invoices = global::EntryDataDS.Business.Entities.Invoices;

    public partial class OCRCorrectionService
    {
        #region Enhanced Public Static Methods (Now Multi-Invoice and Nested-List Aware)

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

            var dictionaries = new List<IDictionary<string, object>>();
            if (dynamicInvoiceResults.Any() && dynamicInvoiceResults[0] is IList nestedList)
            {
                dictionaries = nestedList.OfType<IDictionary<string, object>>().ToList();
            }
            else
            {
                dictionaries = dynamicInvoiceResults.OfType<IDictionary<string, object>>().ToList();
            }

            if (!dictionaries.Any()) return true;

            var tempInvoice = CreateTempShipmentInvoice(dictionaries.First(), log);
            var isBalanced = TotalsZero(tempInvoice, out totalImbalanceSum, log);
            return isBalanced;
        }

        public static bool ShouldContinueCorrections(List<dynamic> res, out double totalImbalanceSum, ILogger logger = null)
        {
            return !TotalsZero(res, out totalImbalanceSum, logger);
        }

        public static async Task<List<dynamic>> CorrectInvoices(List<dynamic> res, Invoice template, List<string> textLines, ILogger logger)
        {
            var log = logger ?? Log.Logger.ForContext(typeof(OCRCorrectionService));

            try
            {
                log.Error("🏁 **CORRECT_INVOICES_ENTRY**: Starting correction process with native data structure handling.");
                log.Error("   - **ARCHITECTURAL_INTENT**: Heal DB -> Read -> Detect -> Apply -> Learn -> Sync -> Re-wrap.");

                var actualInvoiceData = new List<IDictionary<string, object>>();
                if (res != null && res.Any() && res[0] is IList nestedList)
                {
                    actualInvoiceData = nestedList.OfType<IDictionary<string, object>>().ToList();
                }
                else
                {
                    actualInvoiceData = res?.OfType<IDictionary<string, object>>().ToList() ?? new List<IDictionary<string, object>>();
                }

                using (var dbContextForValidation = new OCRContext())
                {
                    var validator = new DatabaseValidator(dbContextForValidation, log);
                    validator.ValidateAndHealTemplate();
                }

                GetTemplatesStep.InvalidateTemplateCache();
                var freshTemplate = GetTemplatesStep.GetAllTemplates(
                    new InvoiceProcessingContext(logger) { FilePath = template.FilePath },
                    new OCRContext()
                ).FirstOrDefault(x => x.OcrInvoices.Id == template.OcrInvoices.Id);

                if (freshTemplate == null)
                {
                    log.Error("   - ❌ FATAL_ERROR: Could not reload template with ID {TemplateId}. Aborting correction.", template.OcrInvoices.Id);
                    return res;
                }

                if (!ShouldContinueCorrections(res, out var imbalance, log))
                {
                    log.Error("     - ✅ **INTENTION_MET**: Invoice is balanced by {Imbalance:F2}. No AI correction needed.", imbalance);
                    return res;
                }
                log.Error("     - ❌ **INTENTION_FAILED**: Invoice unbalanced by {Imbalance:F2}. Proceeding to AI learning.", imbalance);

                using (var correctionService = new OCRCorrectionService(log))
                {
                    var shipmentInvoicesWithMeta = ConvertDynamicToShipmentInvoicesWithMetadata(actualInvoiceData, freshTemplate, correctionService, log);


                    if (!shipmentInvoicesWithMeta.Any())
                    {
                        log.Error("     - ❌ ERROR: Could not convert dynamic data to ShipmentInvoice object for AI analysis. Aborting.");
                        return res;
                    }

                    // ======================================================================================
                    //                          *** DEFINITIVE FIX IS HERE ***
                    // The logic now iterates over all invoices instead of just processing the first one.
                    // This ensures all invoices are corrected and their data can be synced back.
                    // ======================================================================================

                    var allUpdateRequests = new List<RegexUpdateRequest>();
                    var fullDocumentText = template.FormattedPdfText;

                    foreach (var invoiceWrapper in shipmentInvoicesWithMeta)
                    {
                        log.Information("   - 🔎 Processing corrections for InvoiceNo: {InvoiceNo}", invoiceWrapper.Invoice.InvoiceNo);

                        var allDetectedErrors = await correctionService.DetectInvoiceErrorsAsync(invoiceWrapper.Invoice, fullDocumentText, invoiceWrapper.FieldMetadata).ConfigureAwait(false);

                        if (!allDetectedErrors.Any())
                        {
                            log.Error("     - ⚠️ WARNING for Invoice {InvoiceNo}: Correction pipeline triggered, but AI error detection found no omissions. The imbalance may remain.", invoiceWrapper.Invoice.InvoiceNo);
                            continue; // Skip to the next invoice
                        }
                        log.Information("     - ✅ AI detected {ErrorCount} potential errors/omissions for Invoice {InvoiceNo}.", allDetectedErrors.Count, invoiceWrapper.Invoice.InvoiceNo);

                        var appliedCorrections = await correctionService.ApplyCorrectionsAsync(invoiceWrapper.Invoice, allDetectedErrors, fullDocumentText, invoiceWrapper.FieldMetadata).ConfigureAwait(false);
                        log.Information("     - ✅ Applied {AppliedCount} corrections directly to Invoice {InvoiceNo}.", appliedCorrections.Count(c => c.Success), invoiceWrapper.Invoice.InvoiceNo);

                        // Apply Caribbean customs rules for each invoice
                        var customsCorrections = correctionService.ApplyCaribbeanCustomsRules(
                            invoiceWrapper.Invoice,
                            appliedCorrections.Where(c => c.Success).ToList());
                        if (customsCorrections.Any())
                        {
                            correctionService.ApplyCaribbeanCustomsCorrectionsToInvoice(invoiceWrapper.Invoice, customsCorrections);
                            log.Information("     - ✅ Applied {CustomsCount} Caribbean customs rules to Invoice {InvoiceNo}.", customsCorrections.Count, invoiceWrapper.Invoice.InvoiceNo);
                        }

                        // Accumulate database update requests from all detected errors for this invoice
                        var updateRequestsForThisInvoice = allDetectedErrors.DistinctBy(x => new { x.Field, x.SuggestedRegex }).Select(e => new RegexUpdateRequest
                        {
                            FieldName = e.Field,
                            OldValue = e.ExtractedValue,
                            NewValue = e.CorrectValue,
                            CorrectionType = e.ErrorType,
                            Confidence = e.Confidence,
                            DeepSeekReasoning = e.Reasoning,
                            LineNumber = e.LineNumber,
                            LineText = e.LineText,
                            ContextLinesBefore = e.ContextLinesBefore,
                            ContextLinesAfter = e.ContextLinesAfter,
                            RequiresMultilineRegex = e.RequiresMultilineRegex,
                            SuggestedRegex = e.SuggestedRegex,
                            InvoiceId = template.OcrInvoices.Id,
                            FilePath = template.FilePath,
                           
                            // =================== THE FIX ===================
                            // Add the missing mappings here.
                            Pattern = e.Pattern,
                            Replacement = e.Replacement
                            // ===============================================
                        }).ToList();

                        allUpdateRequests.AddRange(updateRequestsForThisInvoice);
                    }

                    // Perform all database learning updates in one batch after processing all invoices
                    await correctionService.UpdateRegexPatternsAsync(allUpdateRequests).ConfigureAwait(false);

                    // Sync all corrected invoices back to the original dynamic data structure
                    var correctedInvoices = shipmentInvoicesWithMeta.Select(siwm => siwm.Invoice).ToList();
                    UpdateDynamicResultsWithCorrections(actualInvoiceData, correctedInvoices, log);
                }

                var finalResult = new List<dynamic> { actualInvoiceData };
                log.Error("   - **STEP 6: RE-WRAP_DATA**: Correction process complete. Re-wrapping corrected data into nested list structure before returning.");

                log.Error("🏁 **CORRECT_INVOICES_EXIT**: OCR correction pipeline complete.");

                return finalResult;

            }
            catch (Exception ex)
            {
                log.Error(ex, "🚨 **CORRECT_INVOICES_CRASH**: An unhandled exception occurred in the main correction pipeline.");
                return res; // Return original data on crash
            }
        }

        public static void UpdateDynamicResultsWithCorrections(
            List<IDictionary<string, object>> dynamicItems,
            List<ShipmentInvoice> correctedInvoices,
            ILogger logger)
        {
            if (dynamicItems == null || !correctedInvoices.Any()) return;
            var correctedInvoiceMap = correctedInvoices.ToDictionary(inv => inv.InvoiceNo, inv => inv);

            logger.Information("🔄 **SYNC_DATA_START**: Synchronizing {CorrectedCount} corrected ShipmentInvoice objects back into a list of {DynamicCount} dynamic data items.", correctedInvoices.Count, dynamicItems.Count);

            foreach (var dynamicItem in dynamicItems)
            {
                if (dynamicItem.TryGetValue("InvoiceNo", out var invNoObj) && invNoObj != null)
                {
                    if (correctedInvoiceMap.TryGetValue(invNoObj.ToString(), out var correctedInv))
                    {
                        logger.Information("   -  syncing InvoiceNo: {InvoiceNo}", correctedInv.InvoiceNo);

                        Action<string, object> logChange = (field, newVal) =>
                        {
                            object oldVal = dynamicItem.ContainsKey(field) ? dynamicItem[field] : null;

                            if (!object.Equals(oldVal, newVal))
                            {
                                logger.Information("     - 📝 Field '{Field}': OLD='{Old}' -> NEW='{New}'",
                                    field,
                                    oldVal?.ToString() ?? "null",
                                    newVal?.ToString() ?? "null");
                            }
                        };

                        logChange("InvoiceTotal", correctedInv.InvoiceTotal);
                        dynamicItem["InvoiceTotal"] = correctedInv.InvoiceTotal;
                        logChange("SubTotal", correctedInv.SubTotal);
                        dynamicItem["SubTotal"] = correctedInv.SubTotal;
                        logChange("TotalInternalFreight", correctedInv.TotalInternalFreight);
                        dynamicItem["TotalInternalFreight"] = correctedInv.TotalInternalFreight;
                        logChange("TotalOtherCost", correctedInv.TotalOtherCost);
                        dynamicItem["TotalOtherCost"] = correctedInv.TotalOtherCost;
                        logChange("TotalInsurance", correctedInv.TotalInsurance);
                        dynamicItem["TotalInsurance"] = correctedInv.TotalInsurance;
                        logChange("TotalDeduction", correctedInv.TotalDeduction);
                        dynamicItem["TotalDeduction"] = correctedInv.TotalDeduction;
                    }
                }
            }
            logger.Information("✅ **SYNC_DATA_COMPLETE**: Finished synchronizing corrected data. The dynamic data list is now updated.");
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
            var invoice = new ShipmentInvoice { InvoiceDetails = new List<InvoiceDetails>() };
            try
            {
                invoice.InvoiceNo = (x.TryGetValue("InvoiceNo", out var v) ? v?.ToString() : null) ?? (x.TryGetValue("Name", out var n) ? n?.ToString() : null) ?? $"TempInv_{Guid.NewGuid().ToString().Substring(0, 8)}";
                invoice.InvoiceTotal = GetNullableDouble(x, "InvoiceTotal", logger);
                invoice.SubTotal = GetNullableDouble(x, "SubTotal", logger);
                invoice.TotalInternalFreight = GetNullableDouble(x, "TotalInternalFreight", logger);
                invoice.TotalOtherCost = GetNullableDouble(x, "TotalOtherCost", logger);
                invoice.TotalInsurance = GetNullableDouble(x, "TotalInsurance", logger);
                invoice.TotalDeduction = GetNullableDouble(x, "TotalDeduction", logger);
            }
            catch (Exception ex)
            {
                logger?.Error(ex, "🚨 **CONVERSION_EXCEPTION**: Error creating temporary ShipmentInvoice.");
                return null;
            }
            return invoice;
        }

        private static double? GetNullableDouble(IDictionary<string, object> x, string key, ILogger logger)
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

        public static List<ShipmentInvoiceWithMetadata> ConvertDynamicToShipmentInvoicesWithMetadata(
            List<IDictionary<string, object>> res,
            Invoice template, OCRCorrectionService serviceInstance, ILogger logger)
        {
            var allInvoices = new List<ShipmentInvoiceWithMetadata>();
            if (serviceInstance == null || res == null) return allInvoices;
            var fieldMappings = serviceInstance.CreateEnhancedFieldMapping(template);

            foreach (var item in res)
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

        public static void UpdateTemplateLineValues(
            Invoice template,
            List<ShipmentInvoice> correctedInvoices,
            ILogger log)
        {
            log.Information("UpdateTemplateLineValues: This method is a placeholder and has no effect in this version.");
        }

        #endregion
    }
}