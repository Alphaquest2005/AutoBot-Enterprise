using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Data.Entity;
using EntryDataDS.Business.Entities;
using OCR.Business.Entities;
using TrackableEntities;
using Serilog;
using InvoiceReader.OCRCorrectionService;

namespace WaterNut.DataSpace
{
    using WaterNut.Business.Services.Utils;

    public partial class OCRCorrectionService
    {
        #region Enhanced Public Static Methods (Now Multi-Invoice Aware)

        private static readonly System.Runtime.CompilerServices.ConditionalWeakTable<ShipmentInvoice, System.Runtime.CompilerServices.StrongBox<double>> _totalsZeroAmounts =
            new System.Runtime.CompilerServices.ConditionalWeakTable<ShipmentInvoice, System.Runtime.CompilerServices.StrongBox<double>>();

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

            bool isZero = differenceAmount < 0.01;

            _totalsZeroAmounts.Remove(invoice);
            _totalsZeroAmounts.Add(invoice, new System.Runtime.CompilerServices.StrongBox<double>(differenceAmount));
            return isZero;
        }

        public static bool TotalsZero(ShipmentInvoice invoice, ILogger logger) => TotalsZero(invoice, out _, logger);

        public static bool TotalsZero(
            List<dynamic> dynamicInvoiceResults,
            out double totalImbalanceSum,
            ILogger logger = null)
        {
            var log = logger ?? Log.Logger.ForContext(typeof(OCRCorrectionService));
            log.Information("Executing multi-invoice aware TotalsZero calculation.");
            var bals = TotalsZeroInternal(dynamicInvoiceResults, out totalImbalanceSum, log);
            if (!bals.Any())
            {
                log.Warning("TotalsZero check found no processable invoice dictionaries.");
                return true;
            }
            return bals.All(s => s.IsBalanced);
        }

        private static List<InvoiceBalanceStatus> TotalsZeroInternal(
            List<dynamic> dynamicInvoiceResults,
            out double totalImbalanceSum,
            ILogger logger = null)
        {
            var allStatuses = new List<InvoiceBalanceStatus>();
            totalImbalanceSum = 0.0;
            if (dynamicInvoiceResults == null) return allStatuses;

            var dictionaries = dynamicInvoiceResults.OfType<IDictionary<string, object>>().ToList();

            foreach (var invoiceDict in dictionaries)
            {
                var status = ProcessSingleDynamicInvoiceForListInternal(invoiceDict, "DirectItem", logger);
                allStatuses.Add(status);
                if (status.ErrorMessage == null)
                {
                    totalImbalanceSum += Math.Abs(status.ImbalanceAmount);
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
                    IsBalanced = false,
                    ErrorMessage = "Failed to convert dynamic data."
                };
            bool isBalanced = TotalsZero(tempInvoice, out double imbalance, log);
            return new InvoiceBalanceStatus
            {
                InvoiceIdentifier = tempInvoice.InvoiceNo ?? defaultIdentifier,
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

        public static async Task<List<dynamic>> CorrectInvoices(List<dynamic> res, Invoice template, ILogger logger)
        {
            var log = logger ?? Log.Logger.ForContext(typeof(OCRCorrectionService));
            try
            {
                log.Information("🏁 **CORRECT_INVOICES_START**: Beginning multi-invoice aware correction for template '{TemplateName}'.", template.OcrInvoices?.Name);

                string originalText = GetOriginalTextFromFile(template.FilePath, log);
                if (string.IsNullOrEmpty(originalText))
                {
                    log.Warning("  - Bailing out: Original text file not found or empty.");
                    return res;
                }

                // This method now correctly processes a simple List<IDictionary>
                return await ProcessSingleInvoiceList(res, template, log, originalText).ConfigureAwait(false);
            }
            catch (Exception ex)
            {
                log.Error(ex, "🚨 **CORRECT_INVOICES_FAILED**: An unexpected error occurred in CorrectInvoices.");
                return res;
            }
        }

        private static async Task<List<dynamic>> ProcessSingleInvoiceList(List<dynamic> singleInvoiceData, Invoice template, ILogger log, string originalText)
        {
            using (var correctionService = new OCRCorrectionService(log))
            {
                log.Information("  - Step 1: Converting {Count} dynamic dictionaries to ShipmentInvoice objects.", singleInvoiceData.Count);
                var shipmentInvoicesWithMeta = ConvertDynamicToShipmentInvoicesWithMetadata(singleInvoiceData, template, correctionService, log);
                var shipmentInvoices = shipmentInvoicesWithMeta.Select(iw => iw.Invoice).ToList();

                if (!shipmentInvoices.Any() || shipmentInvoices.All(i => i == null))
                {
                    log.Error("   -> CRITICAL: Conversion to ShipmentInvoice failed. No valid invoices to process. Check conversion logs.");
                    return singleInvoiceData;
                }

                log.Information("  - Step 2: Detecting and applying corrections for {Count} invoices.", shipmentInvoices.Count);
                foreach (var invoiceWrapper in shipmentInvoicesWithMeta)
                {
                    var invoiceToCorrect = invoiceWrapper.Invoice;
                    if (invoiceToCorrect == null)
                    {
                        log.Warning("Skipping null invoice in correction loop.");
                        continue;
                    }

                    log.Information("    - Processing InvoiceNo: {InvoiceNo}", invoiceToCorrect.InvoiceNo);

                    var errors = await correctionService.DetectInvoiceErrorsAsync(invoiceToCorrect, originalText, invoiceWrapper.FieldMetadata).ConfigureAwait(false);
                    if (!errors.Any())
                    {
                        log.Information("    - No errors detected for InvoiceNo: {InvoiceNo}. Skipping.", invoiceToCorrect.InvoiceNo);
                        continue;
                    }

                    log.Information("    - Found {ErrorCount} errors for InvoiceNo: {InvoiceNo}. Applying corrections...", errors.Count, invoiceToCorrect.InvoiceNo);
                    var appliedCorrections = await correctionService.ApplyCorrectionsAsync(invoiceToCorrect, errors, originalText, invoiceWrapper.FieldMetadata).ConfigureAwait(false);

                    var customsCorrections = correctionService.ApplyCaribbeanCustomsRules(invoiceToCorrect, appliedCorrections.Where(c => c.Success).ToList());
                    if (customsCorrections.Any())
                    {
                        correctionService.ApplyCaribbeanCustomsCorrectionsToInvoice(invoiceToCorrect, customsCorrections);
                        appliedCorrections.AddRange(customsCorrections);
                    }

                    var successfulDetectionsForDB = appliedCorrections
                        .Where(c => c.Success)
                        .Select(c =>
                        {
                            return correctionService.CreateRegexUpdateRequest(c, originalText, invoiceWrapper.FieldMetadata, invoiceToCorrect.Id);
                        }).ToList();

                    if (successfulDetectionsForDB.Any())
                    {
                        await correctionService.UpdateRegexPatternsAsync(successfulDetectionsForDB).ConfigureAwait(false);
                    }
                }
                log.Information("  - Step 2 Complete.");

                log.Information("  - Step 3: Re-importing template '{TemplateName}' to apply newly learned patterns.", template.OcrInvoices?.Name);
                template.ClearInvoiceForReimport();
                var reimportedRes = template.Read(originalText.Split(new[] { "\r\n", "\r", "\n" }, StringSplitOptions.None).ToList());
                log.Information("  - Step 3 Complete. Re-import extracted {Count} new CsvLines.", reimportedRes?.Count ?? 0);

                // FLATTEN THE RE-IMPORTED RESULT (as it will also be nested)
                if (reimportedRes != null && reimportedRes.Any() && reimportedRes[0] is IList nested)
                {
                    reimportedRes = nested.Cast<dynamic>().ToList();
                }

                var listToUpdate = reimportedRes ?? singleInvoiceData;
                log.Information("  - Step 4: Synchronizing in-memory corrections back to the dynamic result list.");
                UpdateDynamicResultsWithCorrections(listToUpdate, shipmentInvoices, log);
                log.Information("  - Step 4 Complete. Synchronization finished.");

                return listToUpdate;
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

        public static ShipmentInvoice ConvertDynamicToShipmentInvoice(
            IDictionary<string, object> dict,
            ILogger logger = null)
        {
            return CreateTempShipmentInvoice(dict, logger);
        }

        private static ShipmentInvoice CreateTempShipmentInvoice(IDictionary<string, object> x, ILogger logger)
        {
            logger?.Error("📋 **CONVERSION_START**: Attempting to convert IDictionary to ShipmentInvoice.");
            try
            {
                var invoice = new ShipmentInvoice { InvoiceDetails = new List<InvoiceDetails>() };

                double? GetNullableDouble(string key)
                {
                    logger?.Error("   -> Parsing Key: '{Key}'", key);
                    if (!x.TryGetValue(key, out var val) || val == null)
                    {
                        logger?.Error("      - Value: NOT FOUND or NULL. Returning null.");
                        return null;
                    }

                    var valStr = val.ToString();
                    var valType = val.GetType().FullName;
                    logger?.Error("      - Raw Value: '{Value}', Raw Type: {Type}", valStr, valType);

                    if (string.IsNullOrWhiteSpace(valStr))
                    {
                        logger?.Error("      - Value: Is Null or Whitespace. Returning null.");
                        return null;
                    }

                    string cleanedValue = Regex.Replace(valStr.Trim(), @"[^\d.,-]", "").Trim();
                    if (cleanedValue.Contains(',') && cleanedValue.Contains('.'))
                    {
                        cleanedValue = cleanedValue.LastIndexOf(',') < cleanedValue.LastIndexOf('.')
                                           ? cleanedValue.Replace(",", "")
                                           : cleanedValue.Replace(".", "").Replace(",", ".");
                    }
                    else if (cleanedValue.Contains(','))
                    {
                        cleanedValue = cleanedValue.Replace(",", ".");
                    }

                    if (double.TryParse(cleanedValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var dbl))
                    {
                        logger?.Error("      - Parse SUCCESS: {Result}", dbl);
                        return dbl;
                    }
                    else
                    {
                        logger?.Error("      - Parse FAILED. Returning null. This would cause a FormatException if not handled.");
                        return null;
                    }
                }

                T GetValue<T>(string key) where T : class => x.TryGetValue(key, out var v) ? v as T : null;

                invoice.InvoiceNo = GetValue<string>("InvoiceNo")
                                    ?? GetValue<string>("Name")
                                    ?? $"TempInv_{Guid.NewGuid().ToString().Substring(0, 8)}";
                invoice.InvoiceTotal = GetNullableDouble("InvoiceTotal");
                invoice.SubTotal = GetNullableDouble("SubTotal");
                invoice.TotalInternalFreight = GetNullableDouble("TotalInternalFreight");
                invoice.TotalOtherCost = GetNullableDouble("TotalOtherCost");
                invoice.TotalInsurance = GetNullableDouble("TotalInsurance");
                invoice.TotalDeduction = GetNullableDouble("TotalDeduction");

                logger?.Error("✅ **CONVERSION_SUCCESS**: Finished converting IDictionary. Final Invoice Total: {Total}", invoice.InvoiceTotal?.ToString() ?? "null");
                return invoice;
            }
            catch (Exception ex)
            {
                logger?.Error(ex, "🚨 **CONVERSION_EXCEPTION**: Error creating temporary ShipmentInvoice.");
                return null;
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
                if (shipmentInvoice != null)
                {
                    var metadata = serviceInstance.ExtractEnhancedOCRMetadata(item, template, fieldMappings);
                    allInvoices.Add(new ShipmentInvoiceWithMetadata { Invoice = shipmentInvoice, FieldMetadata = metadata });
                }
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