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
    using WaterNut.DataSpace.PipelineInfrastructure;
    using Core.Common.Extensions; // Required for BetterExpando

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

        public static async Task<List<dynamic>> CorrectInvoices(List<dynamic> res, Invoice template, List<string> textLines, string originalText, ILogger logger)
        {
            var log = logger ?? Log.Logger.ForContext(typeof(OCRCorrectionService));
            try
            {
                log.Information("🏁 **CORRECT_INVOICES_V10_START**: Final version with type safety fix.");

                // STEP 1: Heal DB
                using (var dbContextForValidation = new OCRContext())
                {
                    var validator = new DatabaseValidator(dbContextForValidation, log);
                    validator.ValidateAndHealTemplate();
                }

                // STEP 2: Reload healed template
                GetTemplatesStep.InvalidateTemplateCache();
                //Invoice freshTemplate;
                //using (var ctx = new OCRContext())
                //{
                //    var freshTemplateData = await ctx.Invoices
                //        .AsNoTracking()
                //        .Include(i => i.Parts.Select(p => p.Lines.Select(l => l.Fields)))
                //        .Include(i => i.Parts.Select(p => p.Lines.Select(l => l.RegularExpressions)))
                //        .Include(i => i.Parts.Select(p => p.PartTypes))
                //        .FirstOrDefaultAsync(x => x.Id == template.OcrInvoices.Id).ConfigureAwait(false);

                //    if (freshTemplateData == null)
                //    {
                //        log.Error("   - ❌ FATAL: Could not reload template data from DB. Aborting correction.");
                //        return res;
                //    }
                //    freshTemplate = new Invoice(freshTemplateData, log)
                //    {
                //        DocSet = template.DocSet,
                //        FilePath = template.FilePath,
                //        EmailId = template.EmailId,
                //        FileType = template.FileType
                //    };
                //}

                var freshTemplate = GetTemplatesStep.GetAllTemplates(
                    new InvoiceProcessingContext(logger)
                        {
                            DocSet = template.DocSet,
                            FilePath = template.FilePath,
                            EmailId = template.EmailId,
                            FileType = template.FileType
                        },
                    new OCRContext()).First(x => x.OcrInvoices.Id == template.OcrInvoices.Id);

                // STEP 3: Initial read with healed template
                var initialReadRes = freshTemplate.Read(textLines);
                if (initialReadRes != null && initialReadRes.Any() && initialReadRes[0] is IList)
                {
                    var flattened = (initialReadRes[0] as IList).Cast<object>().ToList();
                    res = flattened.Select(item => (IDictionary<string, object>)item).Cast<dynamic>().ToList();
                }

                // STEP 4: Check balance
                if (!ShouldContinueCorrections(res, out var imbalance, log))
                {
                    log.Information("✅ Invoice is balanced after reading with the healed template. No further correction needed.");
                    return res;
                }
                log.Warning("  - Invoice still unbalanced by {Imbalance} after healing. Proceeding with AI learning.", imbalance);

                // STEP 5: Learn and correct
                using (var correctionService = new OCRCorrectionService(log))
                {
                    var shipmentInvoicesWithMeta = ConvertDynamicToShipmentInvoicesWithMetadata(res, freshTemplate, correctionService, log);
                    if (!shipmentInvoicesWithMeta.Any()) return res;

                    var invoiceWrapper = shipmentInvoicesWithMeta.First();
                    var allDetectedErrors = await correctionService.DetectInvoiceErrorsAsync(invoiceWrapper.Invoice, originalText, invoiceWrapper.FieldMetadata).ConfigureAwait(false);
                    if (!allDetectedErrors.Any()) return res;

                    var updateRequests = allDetectedErrors.Select(e =>
                    {
                        var correctionResult = new CorrectionResult
                        {
                            FieldName = e.Field,
                            OldValue = e.ExtractedValue,
                            NewValue = e.CorrectValue,
                            CorrectionType = e.ErrorType,
                            Confidence = e.Confidence,
                            Reasoning = e.Reasoning,
                            LineText = e.LineText,
                            LineNumber = e.LineNumber,
                            RequiresMultilineRegex = e.RequiresMultilineRegex,
                            SuggestedRegex = e.SuggestedRegex
                        };
                        return correctionService.CreateRegexUpdateRequest(correctionResult, originalText, invoiceWrapper.FieldMetadata, freshTemplate.OcrInvoices.Id);
                    }).ToList();

                    await correctionService.UpdateRegexPatternsAsync(updateRequests).ConfigureAwait(false);

                    // STEP 6: Final re-read
                    log.Information("  - [FINAL_RE_READ]: Reloading template again to apply newly learned patterns.");
                    GetTemplatesStep.InvalidateTemplateCache();
                    Invoice finalTemplate;
                    using (var ctx = new OCRContext())
                    {
                        var finalTemplateData = await ctx.Invoices.AsNoTracking().Include(i => i.Parts.Select(p => p.Lines.Select(l => l.Fields))).Include(i => i.Parts.Select(p => p.Lines.Select(l => l.RegularExpressions))).Include(i => i.Parts.Select(p => p.PartTypes)).FirstOrDefaultAsync(x => x.Id == template.OcrInvoices.Id).ConfigureAwait(false);
                        finalTemplate = new Invoice(finalTemplateData, log);
                    }

                    var finalReadRes = finalTemplate.Read(textLines);

                    // ================== CRITICAL TYPE-SAFETY FIX ==================
                    if (finalReadRes != null && finalReadRes.Any() && finalReadRes[0] is IList)
                    {
                        var flattenedFinal = (finalReadRes[0] as IList).Cast<object>().ToList();

                        // Convert the BetterExpando objects back to standard dictionaries
                        // that the downstream pipeline can understand.
                        res = flattenedFinal.Select(item => (IDictionary<string, object>)item).Cast<dynamic>().ToList();

                        log.Information("✅ **TYPE_SAFETY_FIX_APPLIED**: Converted re-read result back to List<IDictionary<string, object>> for downstream compatibility.");
                    }
                    // ================== END OF FIX ==================

                    if (!ShouldContinueCorrections(res, out var finalImbalance, log))
                    {
                        log.Information("✅✅✅ **CORRECTION_SUCCESS**: Final re-read produced a balanced invoice! Final Imbalance: {FinalImbalance}", finalImbalance);
                    }
                    else
                    {
                        log.Error("🔥🔥🔥 **CORRECTION_FAILURE**: Even after learning and re-reading, invoice is still unbalanced by {FinalImbalance}. Further investigation needed.", finalImbalance);
                    }
                }

                log.Information("🏁 **CORRECT_INVOICES_V10_END**: OCR correction pipeline complete.");
                return res;
            }
            catch (Exception ex)
            {
                log.Error(ex, "🚨 **CORRECT_INVOICES_FAILED**: An unexpected error occurred.");
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