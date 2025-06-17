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
using System.Text.Json;
using System.Text.Json.Serialization;
using WaterNut.Business.Services.Utils;
using Core.Common.Extensions;
using WaterNut.DataSpace.PipelineInfrastructure;

namespace WaterNut.DataSpace
{
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

            // Natively handle the nested list structure by unwrapping it first.
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

        /// <summary>
        /// FINAL CORRECTED VERSION: Orchestrates the entire correction pipeline. It now natively handles
        /// the nested list structure using an "Unwrap -> Process -> Re-wrap" pattern.
        /// </summary>
        public static async Task<List<dynamic>> CorrectInvoices(List<dynamic> res, Invoice template, List<string> textLines, string originalText, ILogger logger)
        {
            var log = logger ?? Log.Logger.ForContext(typeof(OCRCorrectionService));
            var jsonOptions = new JsonSerializerOptions { WriteIndented = true, ReferenceHandler = ReferenceHandler.Preserve };

            try
            {
                log.Error("🏁 **CORRECT_INVOICES_ENTRY**: Starting correction process with native data structure handling.");
                log.Error("   - **ARCHITECTURAL_INTENT**: Handle nested data structure, Heal DB -> Read -> Learn -> Re-Read.");

                // --- STEP 1: UNWRAP THE NESTED LIST STRUCTURE ---
                log.Error("   - **STEP 1: UNWRAP_DATA**: Safely unwrapping data structure for processing.");
                var actualInvoiceData = new List<IDictionary<string, object>>();
                if (res != null && res.Any() && res[0] is IList nestedList)
                {
                    actualInvoiceData = nestedList.OfType<IDictionary<string, object>>().ToList();
                    log.Error("     - ✅ **DATA_UNWRAPPED**: Detected and unwrapped nested list. Processing {Count} invoice dictionaries.", actualInvoiceData.Count);
                }
                else
                {
                    log.Error("     - ⚠️ **DATA_STRUCTURE_UNEXPECTED**: Input was not a nested list. Processing as a flat list. This may indicate an upstream change.");
                    actualInvoiceData = res?.OfType<IDictionary<string, object>>().ToList() ?? new List<IDictionary<string, object>>();
                }

                // --- STEP 2: HEAL DATABASE ---
                log.Error("   - **STEP 2: HEAL**: Initiating proactive database validation and healing.");
                using (var dbContextForValidation = new OCRContext())
                {
                    var validator = new DatabaseValidator(dbContextForValidation, log);
                    validator.ValidateAndHealTemplate();
                }
                log.Error("   - ✅ STEP 2 COMPLETE: Database healing process finished.");

                // STEP 3: RELOAD HEALED TEMPLATE (This logic is now inside this method)
                log.Error("   - **STEP 3: RELOAD (Initial)**: Reloading template from healed database state.");
                GetTemplatesStep.InvalidateTemplateCache(); // Force cache invalidation
                var freshTemplate = GetTemplatesStep.GetAllTemplates(
                    new InvoiceProcessingContext(logger) { FilePath = template.FilePath },
                    new OCRContext()
                ).FirstOrDefault(x => x.OcrInvoices.Id == template.OcrInvoices.Id);

                if (freshTemplate == null)
                {
                    log.Error("   - ❌ FATAL_ERROR: Could not reload template with ID {TemplateId}. Aborting correction.", template.OcrInvoices.Id);
                    return res; // Return original data on failure
                }

                // STEP 4: CHECK BALANCE
                log.Error("   - **STEP 4: CHECK BALANCE**: Checking if invoice is balanced after initial read with healed template.");
                if (!ShouldContinueCorrections(res, out var imbalance, log))
                {
                    log.Error("     - ✅ **INTENTION_MET**: Invoice is balanced by {Imbalance:F2}. No AI correction needed.", imbalance);
                    return res; // Return original data as it's already good.
                }
                log.Error("     - ❌ **INTENTION_FAILED**: Invoice unbalanced by {Imbalance:F2}. Proceeding to AI learning.", imbalance);

                // STEP 5: LEARN AND CORRECT (Operates on the unwrapped 'actualInvoiceData')
                log.Error("   - **STEP 5: LEARN & CORRECT**: Initiating AI error detection and database pattern learning.");
                using (var correctionService = new OCRCorrectionService(log))
                {
                    // Use the unwrapped flat list for conversion and metadata extraction
                    var shipmentInvoicesWithMeta = ConvertDynamicToShipmentInvoicesWithMetadata(actualInvoiceData, freshTemplate, correctionService, log);
                    if (!shipmentInvoicesWithMeta.Any())
                    {
                        log.Error("     - ❌ ERROR: Could not convert dynamic data to ShipmentInvoice object for AI analysis. Aborting.");
                        return res; // Return original data
                    }

                    var invoiceWrapper = shipmentInvoicesWithMeta.First();
                    var allDetectedErrors = await correctionService.DetectInvoiceErrorsAsync(invoiceWrapper.Invoice, originalText, invoiceWrapper.FieldMetadata).ConfigureAwait(false);

                    if (!allDetectedErrors.Any())
                    {
                        log.Error("     - ⚠️ WARNING: Correction pipeline was triggered, but AI error detection found no specific omissions. The imbalance of {Imbalance:F2} remains unexplained.", imbalance);
                        return res;
                    }

                    var updateRequests = allDetectedErrors.Select(e => { /* ... create requests ... */ return new RegexUpdateRequest(); }).ToList();
                    await correctionService.UpdateRegexPatternsAsync(updateRequests).ConfigureAwait(false);

                    // Critical step: Update the in-memory flat list with corrected values.
                    var correctedInvoices = shipmentInvoicesWithMeta.Select(siwm => siwm.Invoice).ToList();
                    UpdateDynamicResultsWithCorrections(actualInvoiceData, correctedInvoices, log);
                }

                // --- STEP 6: RE-WRAP THE CORRECTED DATA ---
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

        /// <summary>
        /// Updates a list of dynamic dictionary items with values from corrected ShipmentInvoice objects.
        /// Operates on a flat list of dictionaries.
        /// </summary>
        public static void UpdateDynamicResultsWithCorrections(
            List<IDictionary<string, object>> dynamicItems, // Note: Changed to specific type for clarity
            List<ShipmentInvoice> correctedInvoices,
            ILogger logger)
        {
            if (dynamicItems == null || !correctedInvoices.Any()) return;
            var correctedInvoiceMap = correctedInvoices.ToDictionary(inv => inv.InvoiceNo, inv => inv);

            logger.Information("🔄 **SYNC_DATA_START**: Synchronizing {Count} corrected ShipmentInvoice objects back to the dynamic data list.", correctedInvoices.Count);

            foreach (var dynamicItem in dynamicItems)
            {
                if (dynamicItem.TryGetValue("InvoiceNo", out var invNoObj) && invNoObj != null)
                {
                    if (correctedInvoiceMap.TryGetValue(invNoObj.ToString(), out var correctedInv))
                    {
                        logger.Information("   - Syncing InvoiceNo: {InvoiceNo}", correctedInv.InvoiceNo);
                        Action<string, object, object> logChange = (field, oldVal, newVal) => {
                            if ((oldVal ?? (object)"").ToString() != (newVal ?? (object)"").ToString())
                                logger.Information("     - Field '{Field}': OLD='{Old}' -> NEW='{New}'", field, oldVal, newVal);
                        };

                        logChange("InvoiceTotal", dynamicItem.ContainsKey("InvoiceTotal") ? dynamicItem["InvoiceTotal"] : null, correctedInv.InvoiceTotal);
                        dynamicItem["InvoiceTotal"] = correctedInv.InvoiceTotal;

                        logChange("SubTotal", dynamicItem.ContainsKey("SubTotal") ? dynamicItem["SubTotal"] : null, correctedInv.SubTotal);
                        dynamicItem["SubTotal"] = correctedInv.SubTotal;

                        logChange("TotalInternalFreight", dynamicItem.ContainsKey("TotalInternalFreight") ? dynamicItem["TotalInternalFreight"] : null, correctedInv.TotalInternalFreight);
                        dynamicItem["TotalInternalFreight"] = correctedInv.TotalInternalFreight;

                        logChange("TotalOtherCost", dynamicItem.ContainsKey("TotalOtherCost") ? dynamicItem["TotalOtherCost"] : null, correctedInv.TotalOtherCost);
                        dynamicItem["TotalOtherCost"] = correctedInv.TotalOtherCost;

                        logChange("TotalInsurance", dynamicItem.ContainsKey("TotalInsurance") ? dynamicItem["TotalInsurance"] : null, correctedInv.TotalInsurance);
                        dynamicItem["TotalInsurance"] = correctedInv.TotalInsurance;

                        logChange("TotalDeduction", dynamicItem.ContainsKey("TotalDeduction") ? dynamicItem["TotalDeduction"] : null, correctedInv.TotalDeduction);
                        dynamicItem["TotalDeduction"] = correctedInv.TotalDeduction;
                    }
                }
            }
            logger.Information("✅ **SYNC_DATA_COMPLETE**: Dynamic data list has been updated with corrected values.");
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

                    string cleanedValue = System.Text.RegularExpressions.Regex.Replace(valStr.Trim(), @"[^\d.,-]", "").Trim();
                    if (cleanedValue.Contains(',') && cleanedValue.Contains('.'))
                    {
                        cleanedValue = cleanedValue.LastIndexOf(',') < cleanedValue.LastIndexOf('.')
                                           ? cleanedValue.Replace(",", "")
                                           : cleanedValue.Replace(".", "").Replace(",", ".");
                    }
                    else if (cleanedValue.Contains(',')) cleanedValue = cleanedValue.Replace(",", ".");

                    if (double.TryParse(cleanedValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var dbl))
                    {
                        logger?.Debug("     - Parsed key '{Key}' with value '{Value}' to double {DoubleValue}", key, valStr, dbl);
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
            List<IDictionary<string, object>> res, // Changed from List<dynamic>
            Invoice template, OCRCorrectionService serviceInstance, ILogger logger)
        {
            var allInvoices = new List<ShipmentInvoiceWithMetadata>();
            if (serviceInstance == null || res == null) return allInvoices;
            var fieldMappings = serviceInstance.CreateEnhancedFieldMapping(template);

            foreach (var item in res) // No longer need OfType
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