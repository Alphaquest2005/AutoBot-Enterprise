// File: OCRCorrectionService/OCRLegacySupport.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using EntryDataDS.Business.Entities;
using OCR.Business.Entities;
using Serilog;
using System.IO;
using System.Runtime.CompilerServices;
using System.Text.RegularExpressions;

namespace WaterNut.DataSpace
{
    public partial class OCRCorrectionService
    {
        #region Enhanced Public Static Methods (Legacy Support)

        private static readonly ConditionalWeakTable<ShipmentInvoice, StrongBox<double>> _totalsZeroAmounts =
            new ConditionalWeakTable<ShipmentInvoice, StrongBox<double>>();

        public struct InvoiceBalanceStatus
        {
            public string InvoiceIdentifier { get; set; }

            public bool IsBalanced { get; set; }

            public double ImbalanceAmount { get; set; }

            public string ErrorMessage { get; set; }
        }

        public static bool TotalsZero(ShipmentInvoice invoice, out double differenceAmount, ILogger logger)
        {
            var log = logger ?? Log.Logger.ForContext(typeof(OCRCorrectionService));
            differenceAmount = double.MaxValue;
            if (invoice == null) return false;

            var subTotal = invoice.SubTotal ?? 0;
            var freight = invoice.TotalInternalFreight ?? 0;
            var otherCost = invoice.TotalOtherCost ?? 0;
            var insurance = invoice.TotalInsurance ?? 0;
            var deductionAmount = invoice.TotalDeduction ?? 0;
            var reportedInvoiceTotal = invoice.InvoiceTotal ?? 0;

            var baseTotal = subTotal + freight + otherCost + insurance;
            var calculatedFinalTotal = baseTotal - deductionAmount;
            differenceAmount = Math.Abs(calculatedFinalTotal - reportedInvoiceTotal);

            bool isZero = differenceAmount < 0.01;

            _totalsZeroAmounts.Remove(invoice);
            _totalsZeroAmounts.Add(invoice, new StrongBox<double>(differenceAmount));
            return isZero;
        }

        public static bool TotalsZero(ShipmentInvoice invoice, ILogger logger) => TotalsZero(invoice, out _, logger);

        public static bool TotalsZero(
            List<dynamic> dynamicInvoiceResults,
            out double totalImbalanceSum,
            ILogger logger = null)
        {
            var bals = TotalsZeroInternal(dynamicInvoiceResults, out totalImbalanceSum, logger ?? Log.Logger);
            return bals.Any() && bals.All(s => s.IsBalanced);
        }

        private static List<InvoiceBalanceStatus> TotalsZeroInternal(
            List<dynamic> dynamicInvoiceResults,
            out double totalImbalanceSum,
            ILogger logger = null)
        {
            var allStatuses = new List<InvoiceBalanceStatus>();
            totalImbalanceSum = 0.0;
            if (dynamicInvoiceResults == null) return allStatuses;

            foreach (var dynamicItem in dynamicInvoiceResults)
            {
                if (dynamicItem is List<IDictionary<string, object>> list)
                {
                    foreach (var dict in list)
                    {
                        var status = ProcessSingleDynamicInvoiceForListInternal(dict, "ListItem", logger);
                        allStatuses.Add(status);
                        if (status.ErrorMessage == null) totalImbalanceSum += Math.Abs(status.ImbalanceAmount);
                    }
                }
                else if (dynamicItem is IDictionary<string, object> invoiceDict)
                {
                    var status = ProcessSingleDynamicInvoiceForListInternal(invoiceDict, "DirectItem", logger);
                    allStatuses.Add(status);
                    if (status.ErrorMessage == null) totalImbalanceSum += Math.Abs(status.ImbalanceAmount);
                }
            }

            return allStatuses;
        }

        private static InvoiceBalanceStatus ProcessSingleDynamicInvoiceForListInternal(
            IDictionary<string, object> invoiceDict,
            string defaultIdentifier,
            ILogger log)
        {
            var tempInvoice = CreateTempShipmentInvoice(invoiceDict, log);
            if (tempInvoice == null)
                return new InvoiceBalanceStatus
                           {
                               IsBalanced = false, ErrorMessage = "Failed to convert dynamic data."
                           };
            bool isBalanced = TotalsZero(tempInvoice, out double imbalance, log);
            return new InvoiceBalanceStatus
                       {
                           InvoiceIdentifier = tempInvoice.InvoiceNo,
                           IsBalanced = isBalanced,
                           ImbalanceAmount = imbalance
                       };
        }

        public static bool ShouldContinueCorrections(
            List<dynamic> res,
            out double totalImbalanceSum,
            ILogger logger = null)
        {
            TotalsZeroInternal(res, out totalImbalanceSum, logger);
            return totalImbalanceSum > 0.01;
        }

        // File: OCRCorrectionService/OCRLegacySupport.cs

        public static async Task<List<dynamic>> CorrectInvoices(List<dynamic> res, Invoice template, ILogger logger)
        {
            var log = logger ?? Log.Logger.ForContext(typeof(OCRCorrectionService));
            try
            {
                log.Information("🏁 **CORRECT_INVOICES_START**: Beginning OCR correction process for template '{TemplateName}'.", template.OcrInvoices?.Name);

                string originalText = GetOriginalTextFromFile(template.FilePath, log);
                if (string.IsNullOrEmpty(originalText))
                {
                    log.Warning("  - Bailing out: Original text file not found or empty.");
                    return res;
                }

                using var correctionService = new OCRCorrectionService(log);

                // ===================================================================
                // STEP 1: CONVERT DYNAMIC DATA TO STRONGLY-TYPED OBJECTS
                // ===================================================================
                log.Information("  - Step 1: Converting {Count} dynamic CsvLines to ShipmentInvoice objects.", res.Count);
                var shipmentInvoicesWithMeta = ConvertDynamicToShipmentInvoicesWithMetadata(res, template, correctionService, log);
                var shipmentInvoices = shipmentInvoicesWithMeta.Select(iw => iw.Invoice).ToList();
                log.Information("  - Step 1 Complete. Found {Count} ShipmentInvoice(s) to process.", shipmentInvoices.Count);
                // Log initial state for debugging
                foreach (var inv in shipmentInvoices)
                {
                    log.Verbose("    - Initial State for InvoiceNo '{InvNo}': TotalDeduction={TD}, TotalInsurance={TI}",
                        inv.InvoiceNo, inv.TotalDeduction?.ToString("F2") ?? "null", inv.TotalInsurance?.ToString("F2") ?? "null");
                }

                // ===================================================================
                // STEP 2: DETECT ERRORS AND LEARN NEW PATTERNS
                // ===================================================================
                log.Information("  - Step 2: Detecting errors, applying in-memory corrections, and learning new DB patterns.");
                foreach (var invoiceWrapper in shipmentInvoicesWithMeta)
                {
                    var invoiceToCorrect = invoiceWrapper.Invoice;
                    log.Information("    - Processing InvoiceNo: {InvoiceNo}", invoiceToCorrect.InvoiceNo);

                    var errors = await correctionService.DetectInvoiceErrorsAsync(invoiceToCorrect, originalText, invoiceWrapper.FieldMetadata).ConfigureAwait(false);
                    if (!errors.Any())
                    {
                        log.Information("    - No errors detected for InvoiceNo: {InvoiceNo}. Skipping.", invoiceToCorrect.InvoiceNo);
                        continue;
                    }

                    log.Information("    - Found {ErrorCount} errors for InvoiceNo: {InvoiceNo}. Applying corrections and learning...", errors.Count, invoiceToCorrect.InvoiceNo);
                    await correctionService.ApplyCorrectionsAsync(invoiceToCorrect, errors, originalText, invoiceWrapper.FieldMetadata).ConfigureAwait(false);

                    var regexUpdateRequests = errors.Select(error =>
                        correctionService.CreateRegexUpdateRequest(
                            new CorrectionResult
                            {
                                FieldName = error.Field,
                                OldValue = error.ExtractedValue,
                                NewValue = error.CorrectValue,
                                CorrectionType = error.ErrorType,
                                Confidence = error.Confidence,
                                LineText = error.LineText,
                                LineNumber = error.LineNumber,
                                RequiresMultilineRegex = error.RequiresMultilineRegex,
                                Reasoning = error.Reasoning,
                                ContextLinesBefore = error.ContextLinesBefore,
                                ContextLinesAfter = error.ContextLinesAfter
                            },
                            originalText, invoiceWrapper.FieldMetadata, invoiceToCorrect.Id
                        )
                    ).ToList();

                    await correctionService.UpdateRegexPatternsAsync(regexUpdateRequests).ConfigureAwait(false);
                    log.Information("    - Finished processing corrections for InvoiceNo: {InvoiceNo}", invoiceToCorrect.InvoiceNo);
                }
                log.Information("  - Step 2 Complete.");

                // ===================================================================
                // STEP 3: RE-IMPORT TEMPLATE WITH NEWLY LEARNED PATTERNS
                // ===================================================================
                log.Information("  - Step 3: Re-importing template '{TemplateName}' to apply newly learned patterns.", template.OcrInvoices?.Name);
                template.ClearInvoiceForReimport();
                var reimportedRes = template.Read(originalText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList());
                log.Information("  - Step 3 Complete. Re-import extracted {Count} new CsvLines.", reimportedRes?.Count ?? 0);

                // ===================================================================
                // STEP 4: SYNCHRONIZE CORRECTED DATA BACK TO DYNAMIC LIST
                // ===================================================================
                log.Information("  - Step 4: Synchronizing in-memory corrections back to the dynamic result list.");

                // Robustly choose which list to update
                var listToUpdate = reimportedRes ?? res;
                log.Information("    - Target list for update has {Count} items. (Using re-imported list: {IsReimported})", listToUpdate.Count, reimportedRes != null);

                // Log state BEFORE sync
                if (listToUpdate.Any() && listToUpdate[0] is IDictionary<string, object> beforeDict)
                {
                    log.Verbose("    - State BEFORE Sync: TotalDeduction='{TD}', TotalInsurance='{TI}'",
                        beforeDict.TryGetValue("TotalDeduction", out var td) ? td : "null",
                        beforeDict.TryGetValue("TotalInsurance", out var ti) ? ti : "null");
                }

                UpdateDynamicResultsWithCorrections(listToUpdate, shipmentInvoices, log);

                // Log state AFTER sync
                if (listToUpdate.Any() && listToUpdate[0] is IDictionary<string, object> afterDict)
                {
                    log.Verbose("    - State AFTER Sync: TotalDeduction='{TD}', TotalInsurance='{TI}'",
                        afterDict.TryGetValue("TotalDeduction", out var td) ? td : "null",
                        afterDict.TryGetValue("TotalInsurance", out var ti) ? ti : "null");
                }
                log.Information("  - Step 4 Complete. Synchronization finished.");

                // ===================================================================
                // STEP 5: FINAL VALIDATION AND RETURN
                // ===================================================================
                log.Information("  - Step 5: Final validation of corrected data.");
                if (TotalsZero(listToUpdate, out var finalImbalance, log))
                {
                    log.Information("✅ **CORRECT_INVOICES_SUCCESS**: The corrected invoice is now balanced (Imbalance: {Imbalance:F2}).", finalImbalance);
                }
                else
                {
                    log.Warning("⚠️ **CORRECT_INVOICES_PARTIAL**: The corrected invoice remains unbalanced (Imbalance: {Imbalance:F2}).", finalImbalance);
                }

                return listToUpdate;
            }
            catch (Exception ex)
            {
                log.Error(ex, "🚨 **CORRECT_INVOICES_FAILED**: An unexpected error occurred in CorrectInvoices.");
                return res; // Return original data on failure
            }
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

        // This static method is now a public entry point for external callers like the test suite.
        public static ShipmentInvoice ConvertDynamicToShipmentInvoice(
            IDictionary<string, object> dict,
            ILogger logger = null)
        {
            return CreateTempShipmentInvoice(dict, logger);
        }

        private static ShipmentInvoice CreateTempShipmentInvoice(IDictionary<string, object> x, ILogger logger)
        {
            try
            {
                var invoice = new ShipmentInvoice { InvoiceDetails = new List<InvoiceDetails>() };

                double? GetNullableDouble(string key)
                {
                    if (x.TryGetValue(key, out var val) && val != null)
                    {
                        if (val is double d) return d;
                        if (val is decimal dec) return (double)dec;
                        var valStr = val.ToString();
                        if (!string.IsNullOrEmpty(valStr))
                        {
                            string cleanedValue = Regex.Replace(valStr.Trim(), @"[^\d.,-]", "").Trim();
                            if (cleanedValue.Contains(',') && cleanedValue.Contains('.'))
                                cleanedValue = cleanedValue.LastIndexOf(',') < cleanedValue.LastIndexOf('.')
                                                   ? cleanedValue.Replace(",", "")
                                                   : cleanedValue.Replace(".", "").Replace(",", ".");
                            else if (cleanedValue.Contains(',')) cleanedValue = cleanedValue.Replace(",", ".");
                            if (double.TryParse(
                                    cleanedValue,
                                    System.Globalization.NumberStyles.Any,
                                    System.Globalization.CultureInfo.InvariantCulture,
                                    out var dbl)) return dbl;
                        }
                    }

                    return null;
                }

                T GetValue<T>(string key)
                    where T : class =>
                    x.TryGetValue(key, out var v) ? v as T : null;

                invoice.InvoiceNo = GetValue<string>("InvoiceNo")
                                    ?? $"TempInv_{Guid.NewGuid().ToString().Substring(0, 8)}";
                invoice.InvoiceTotal = GetNullableDouble("InvoiceTotal");
                invoice.SubTotal = GetNullableDouble("SubTotal");
                invoice.TotalInternalFreight = GetNullableDouble("TotalInternalFreight");
                invoice.TotalOtherCost = GetNullableDouble("TotalOtherCost");
                invoice.TotalInsurance = GetNullableDouble("TotalInsurance");
                invoice.TotalDeduction = GetNullableDouble("TotalDeduction");

                return invoice;
            }
            catch (Exception ex)
            {
                logger?.Error(ex, "Error creating temporary ShipmentInvoice.");
                return new ShipmentInvoice { InvoiceNo = "ERROR_CREATION" };
            }
        }

        public static List<ShipmentInvoiceWithMetadata> ConvertDynamicToShipmentInvoicesWithMetadata(
            List<dynamic> res,
            Invoice template,
            OCRCorrectionService serviceInstance,
            ILogger logger)
        {
            var allInvoices = new List<ShipmentInvoiceWithMetadata>();
            if (serviceInstance == null || res == null) return allInvoices;
            var fieldMappings = serviceInstance.CreateEnhancedFieldMapping(template);
            foreach (var item in res.OfType<IDictionary<string, object>>())
            {
                var shipmentInvoice = CreateTempShipmentInvoice(item, logger);
                var metadata = serviceInstance.ExtractEnhancedOCRMetadata(item, template, fieldMappings);
                allInvoices.Add(
                    new ShipmentInvoiceWithMetadata { Invoice = shipmentInvoice, FieldMetadata = metadata });
            }

            return allInvoices;
        }

        public static void UpdateDynamicResultsWithCorrections(
            List<dynamic> res,
            List<ShipmentInvoice> correctedInvoices,
            ILogger logger)
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

        public static void UpdateTemplateLineValues(
            Invoice template,
            List<ShipmentInvoice> correctedInvoices,
            ILogger log)
        {
            log.Information("UpdateTemplateLineValues: This method is a placeholder.");
        }

        #endregion
    }

}