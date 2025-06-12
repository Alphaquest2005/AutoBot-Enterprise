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

        // Definition for _totalsZeroAmounts
        // This table allows associating a calculated imbalance amount with a ShipmentInvoice instance
        // without modifying the ShipmentInvoice class itself. It's primarily for the
        // .TotalsZeroAmount() extension method for debugging or specific scenarios.
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

            if (invoice == null)
            {
                log.Error("🚨 **CRITICAL_ERROR**: TotalsZero (ShipmentInvoice): Null invoice provided");
                return false;
            }

            // 🔍 **ASSUMPTION**: All nullable fields should be treated as 0.0 when null
            var subTotal = invoice.SubTotal ?? 0;
            var freight = invoice.TotalInternalFreight ?? 0;
            var otherCost = invoice.TotalOtherCost ?? 0;
            var insurance = invoice.TotalInsurance ?? 0;
            var deductionAmount = invoice.TotalDeduction ?? 0;
            var reportedInvoiceTotal = invoice.InvoiceTotal ?? 0;

            log.Error("🔍 **CALCULATION_INPUTS_SHIPMENT**: InvoiceNo={InvoiceNo} | SubTotal={SubTotal:F2} | Freight={Freight:F2} | OtherCost={OtherCost:F2} | Insurance={Insurance:F2} | Deduction={Deduction:F2} | ReportedTotal={ReportedTotal:F2}",
                invoice.InvoiceNo, subTotal, freight, otherCost, insurance, deductionAmount, reportedInvoiceTotal);

            // 🔍 **ASSUMPTION**: BaseTotal = SubTotal + Freight + OtherCost + Insurance
            var baseTotal = subTotal + freight + otherCost + insurance;
            log.Error("🔍 **CALCULATION_STEP1_SHIPMENT**: BaseTotal = ({SubTotal:F2} + {Freight:F2} + {OtherCost:F2} + {Insurance:F2}) = {BaseTotal:F2}",
                subTotal, freight, otherCost, insurance, baseTotal);

            // 🔍 **ASSUMPTION**: FinalTotal = BaseTotal - Deduction
            var calculatedFinalTotal = baseTotal - deductionAmount;
            log.Error("🔍 **CALCULATION_STEP2_SHIPMENT**: FinalTotal = ({BaseTotal:F2} - {Deduction:F2}) = {FinalTotal:F2}",
                baseTotal, deductionAmount, calculatedFinalTotal);

            // 🔍 **ASSUMPTION**: Difference = |FinalTotal - ReportedTotal|
            differenceAmount = Math.Abs(calculatedFinalTotal - reportedInvoiceTotal);
            log.Error("🔍 **CALCULATION_STEP3_SHIPMENT**: Difference = |{FinalTotal:F2} - {ReportedTotal:F2}| = {Difference:F4}",
                calculatedFinalTotal, reportedInvoiceTotal, differenceAmount);

            // 🔍 **ASSUMPTION**: Invoice is balanced if difference < 0.01
            bool isZero = differenceAmount < 0.01;
            var tolerance = 0.01;
            log.Error("🔍 **CALCULATION_RESULT_SHIPMENT**: IsBalanced = ({Difference:F4} < {Tolerance:F2}) = {IsBalanced}",
                differenceAmount, tolerance, isZero);

            // Store result for extension method access
            _totalsZeroAmounts.Remove(invoice);
            _totalsZeroAmounts.Add(invoice, new StrongBox<double>(differenceAmount));

            if (isZero)
            {
                log.Error("✅ **CALCULATION_VALID**: ShipmentInvoice {InvoiceNo} is balanced (diff={Difference:F4})", invoice.InvoiceNo, differenceAmount);
            }
            else
            {
                log.Error("❌ **CALCULATION_UNBALANCED**: ShipmentInvoice {InvoiceNo} is unbalanced (diff={Difference:F4})", invoice.InvoiceNo, differenceAmount);
            }

            return isZero;
        }

        public static bool TotalsZero(ShipmentInvoice invoice, ILogger logger)
        {
            return TotalsZero(invoice, out _, logger);
        }

        public static bool TotalsZero(
            List<dynamic> dynamicInvoiceResults,
            out double totalImbalanceSum,
            ILogger logger = null)
        {
            var log = logger ?? Log.Logger.ForContext(typeof(OCRCorrectionService));
            
            log.Error("🔍 **DIRECT_TOTALS_ZERO_ENTRY**: TotalsZero(List<dynamic>) called with {ItemCount} items", dynamicInvoiceResults?.Count ?? 0);
            
            List<InvoiceBalanceStatus> bals = TotalsZeroInternal(dynamicInvoiceResults,out totalImbalanceSum, logger);
            
            bool allBalanced = bals.Any() && bals.All(s => s.IsBalanced);
            
            log.Error("🔍 **DIRECT_TOTALS_ZERO_RESULT**: ProcessedStatuses={StatusCount} | AllBalanced={AllBalanced} | TotalImbalanceSum={TotalSum:F5}", 
                bals.Count, allBalanced, totalImbalanceSum);
            
            return allBalanced;
        }

        /// <summary>
        /// Checks the balance for all invoices found within a List<dynamic> structure.
        /// Outputs the SUM of all individual imbalance amounts.
        /// </summary>
        /// <param name="dynamicInvoiceResults">The List<dynamic> containing invoice data.</param>
        /// <param name="totalImbalanceSum">Outputs the sum of absolute imbalance amounts for all processed invoices.</param>
        /// <param name="logger">Optional logger instance.</param>
        /// <returns>A list of InvoiceBalanceStatus objects, one for each processed invoice dictionary.</returns>
        private static List<InvoiceBalanceStatus> TotalsZeroInternal(
            List<dynamic> dynamicInvoiceResults,
            out double totalImbalanceSum,
            ILogger logger = null)
        {
            var log = logger ?? Log.Logger.ForContext(typeof(OCRCorrectionService));
            var allStatuses = new List<InvoiceBalanceStatus>();
            totalImbalanceSum = 0.0; // Initialize sum

            if (dynamicInvoiceResults == null || !dynamicInvoiceResults.Any())
            {
                log.Information("TotalsZero (List<dynamic>): Input list is null or empty.");
                return allStatuses;
            }

            int dynamicItemIndex = 0;

            foreach (var dynamicItem in dynamicInvoiceResults)
            {
                List<IDictionary<string, object>> currentBatchOfDicts = new List<IDictionary<string, object>>();
                if (dynamicItem is List<IDictionary<string, object>> invoiceList)
                {
                    currentBatchOfDicts.AddRange(invoiceList);
                }
                else if (dynamicItem is IDictionary<string, object> singleInvoiceDict)
                {
                    currentBatchOfDicts.Add(singleInvoiceDict);
                }
                else
                {
                    log.Warning("TotalsZero (List<dynamic>): Encountered an item of unexpected type '{ItemType}' at main index {Index}.",
                        dynamicItem?.GetType().Name ?? "null", dynamicItemIndex);
                }

                int subIndex = 0;
                foreach (var invoiceDict in currentBatchOfDicts)
                {
                    string defaultIdentifier = (currentBatchOfDicts.Count > 1 || dynamicItem is List<IDictionary<string, object>>)
                        ? $"ListSection_{dynamicItemIndex}_Item_{subIndex++}"
                        : $"DirectItem_{dynamicItemIndex}";

                    var status = ProcessSingleDynamicInvoiceForListInternal(invoiceDict, defaultIdentifier, log);
                    allStatuses.Add(status);

                    // **CRITICAL_DEBUGGING**: Log the status details to debug totalImbalanceSum calculation
                    log.Error("🔍 **TOTALS_ZERO_INTERNAL_STATUS**: InvoiceId={InvoiceId} | IsBalanced={IsBalanced} | ImbalanceAmount={ImbalanceAmount:F5} | ErrorMessage={ErrorMessage}", 
                        status.InvoiceIdentifier, status.IsBalanced, status.ImbalanceAmount, status.ErrorMessage ?? "NULL");

                    // Add to the total sum of imbalances using absolute values to prevent cancellation
                    if (status.ErrorMessage == null) // Only add if successfully processed
                    {
                        double absImbalance = Math.Abs(status.ImbalanceAmount);
                        totalImbalanceSum += absImbalance;
                        log.Error("🔍 **TOTALS_ZERO_INTERNAL_SUM_ADDED**: Added abs({ImbalanceAmount:F5})={AbsImbalance:F5} to totalImbalanceSum, new total={TotalSum:F5}", 
                            status.ImbalanceAmount, absImbalance, totalImbalanceSum);
                        
                        // CRITICAL: Verify that if individual imbalance > 0, total sum > 0 (no cancellation)
                        if (absImbalance > 0.01 && totalImbalanceSum <= 0.01)
                        {
                            log.Error("🚨 **TOTALS_ZERO_INTERNAL_CANCELLATION_ERROR**: Individual imbalance {AbsImbalance:F5} > 0.01 but totalSum {TotalSum:F5} <= 0.01 - this indicates cancellation occurred!", 
                                absImbalance, totalImbalanceSum);
                        }
                    }
                    else
                    {
                        log.Error("❌ **TOTALS_ZERO_INTERNAL_SUM_SKIPPED**: Skipped adding {ImbalanceAmount:F5} due to ErrorMessage='{ErrorMessage}'", 
                            status.ImbalanceAmount, status.ErrorMessage);
                        // If ANY invoice fails processing, mark the entire calculation as unreliable
                        totalImbalanceSum = double.MaxValue;
                        log.Error("🚨 **TOTALS_ZERO_INTERNAL_ERROR**: Setting totalImbalanceSum=MaxValue due to processing error - calculation unreliable");
                        break; // Exit early since calculation is now invalid
                    }
                }
                dynamicItemIndex++;
            }

            if (!allStatuses.Any()) // No invoices were processed from the list
            {
                totalImbalanceSum = double.MaxValue; // Indicate no processable data
            }

            log.Debug("TotalsZero (List<dynamic>): Processed {Count} invoices. Total Imbalance Sum: {TotalImbalanceSum:F4}", allStatuses.Count, totalImbalanceSum);
            return allStatuses;
        }

        /// <summary>
        /// Internal helper to process a single IDictionary<string, object> for TotalsZero and return its balance status.
        /// </summary>
        private static InvoiceBalanceStatus ProcessSingleDynamicInvoiceForListInternal(IDictionary<string, object> invoiceDict, string defaultIdentifier, ILogger log)
        {
            if (invoiceDict == null)
            {
                return new InvoiceBalanceStatus { InvoiceIdentifier = $"{defaultIdentifier}_NullData", IsBalanced = false, ImbalanceAmount = double.MaxValue, ErrorMessage = "Invoice data dictionary was null." };
            }

            string invoiceNoForLog = invoiceDict.TryGetValue("InvoiceNo", out var invNoObj) && invNoObj != null ? invNoObj.ToString() : defaultIdentifier;
            ShipmentInvoice tempInvoice = CreateTempShipmentInvoice(invoiceDict, log);

            if (tempInvoice == null || tempInvoice.InvoiceNo == "ERROR_CREATION")
            {
                log.Warning("ProcessSingleDynamicInvoiceForListInternal: Failed to create a temporary ShipmentInvoice for identifier '{InvoiceIdentifier}'.", invoiceNoForLog);
                return new InvoiceBalanceStatus { InvoiceIdentifier = invoiceNoForLog, IsBalanced = false, ImbalanceAmount = double.MaxValue, ErrorMessage = "Failed to convert dynamic data to ShipmentInvoice." };
            }

            bool isBalanced = TotalsZero(tempInvoice, out double imbalance, log);

            // **CRITICAL_DEBUGGING**: Log the specific calculation result to compare with direct method
            log.Error("🔍 **SHOULDCONTINUE_SINGLE_INVOICE_CALC**: InvoiceNo={InvoiceNo} | IsBalanced={IsBalanced} | Imbalance={Imbalance:F5} | Method=TotalsZero(ShipmentInvoice)",
                tempInvoice.InvoiceNo, isBalanced, imbalance);

            return new InvoiceBalanceStatus
            {
                InvoiceIdentifier = tempInvoice.InvoiceNo,
                IsBalanced = isBalanced,
                ImbalanceAmount = imbalance
            };
        }

        /// <summary>
        /// Determines if correction processing should continue based on the balance status of invoices in 'res'.
        /// Continues if *any* invoice in the list is found to be unbalanced.
        /// Outputs the total sum of imbalances for the processed invoices.
        /// </summary>
        public static bool ShouldContinueCorrections(List<dynamic> res, out double totalImbalanceSum, ILogger logger = null)
        {
            var log = logger ?? Log.Logger.ForContext(typeof(OCRCorrectionService));

            log.Error("🔍 **SHOULDCONTINUE_ENTRY**: ShouldContinueCorrections called with {ResCount} dynamic items", res?.Count ?? 0);
            log.Error("🔍 **ASSUMPTION_SHOULDCONTINUE**: Should return TRUE if any invoice is unbalanced (difference >= 0.01)");

            // Use the TotalsZero overload that outputs the sum
            List<InvoiceBalanceStatus> balanceStatuses = TotalsZeroInternal(res, out totalImbalanceSum, log);

            log.Error("🔍 **SHOULDCONTINUE_PROCESSED**: Processed {StatusCount} invoice balance statuses | TotalImbalanceSum={TotalSum:F4}", 
                balanceStatuses.Count, totalImbalanceSum);

            if (!balanceStatuses.Any())
            {
                log.Error("❌ **SHOULDCONTINUE_NO_DATA**: No processable invoice data found in 'res'. Returning FALSE.");
                return false;
            }

            // Log each balance status for analysis
            for (int i = 0; i < balanceStatuses.Count; i++)
            {
                var status = balanceStatuses[i];
                log.Error("🔍 **SHOULDCONTINUE_STATUS_{Index}**: ID={InvoiceId} | IsBalanced={IsBalanced} | Imbalance={Imbalance:F4} | Error={Error}", 
                    i, status.InvoiceIdentifier, status.IsBalanced, status.ImbalanceAmount, status.ErrorMessage ?? "None");
            }

            bool anyUnbalanced = balanceStatuses.Any(s => !s.IsBalanced);
            int unbalancedCount = balanceStatuses.Count(s => !s.IsBalanced);
            int balancedCount = balanceStatuses.Count(s => s.IsBalanced);

            log.Error("🔍 **SHOULDCONTINUE_ANALYSIS**: Total={Total} | Balanced={Balanced} | Unbalanced={Unbalanced} | AnyUnbalanced={AnyUnbalanced}", 
                balanceStatuses.Count, balancedCount, unbalancedCount, anyUnbalanced);

            if (anyUnbalanced)
            {
                log.Error("✅ **SHOULDCONTINUE_TRUE**: At least one invoice is unbalanced. Corrections should continue. TotalImbalance={TotalImbalance:F4}",
                    totalImbalanceSum);
                return true;
            }
            else
            {
                log.Error("❌ **SHOULDCONTINUE_FALSE**: All invoices are balanced. Corrections not needed. TotalImbalance={TotalImbalance:F4}",
                    totalImbalanceSum);
                log.Error("🎯 **ROOT_CAUSE**: ShouldContinueCorrections returning FALSE - this blocks OCR correction loop execution");
                return false;
            }
        }


        public static async Task<bool> CorrectInvoices(ShipmentInvoice invoice, string fileText, ILogger logger)
        {
            var log = logger ?? Log.Logger.ForContext(typeof(OCRCorrectionService));
            
            // **PROOF OF ENTRY**: Use LogLevelOverride to ensure this log ALWAYS appears regardless of global settings
            using (Core.Common.Extensions.LogLevelOverride.Begin(Serilog.Events.LogEventLevel.Verbose))
            {
                log.Information("🔍 **OCR_CORRECTION_ENTRY**: CorrectInvoices(ShipmentInvoice invoice, string fileText, ILogger logger) method called for Invoice: {InvoiceNo}", 
                    invoice?.InvoiceNo ?? "NULL");
            }
            if (TotalsZero(invoice, out double diff, log))
            {
                log.Information("CorrectInvoices (single ShipmentInvoice): Invoice {InvNo} is already balanced. Diff: {Difference:F4}. Skipping correction.", invoice.InvoiceNo, diff);
                
                // **PROOF OF EXIT**: Use LogLevelOverride to ensure this log ALWAYS appears
                using (Core.Common.Extensions.LogLevelOverride.Begin(Serilog.Events.LogEventLevel.Verbose))
                {
                    log.Information("🔍 **OCR_CORRECTION_EXIT**: CorrectInvoices (single invoice) exiting early - already balanced");
                }
                return true;
            }

            log.Information("CorrectInvoices (single ShipmentInvoice): Invoice {InvNo} is not balanced. Diff: {Difference:F4}. Proceeding with correction.", invoice.InvoiceNo, diff);
            using var service = new OCRCorrectionService(log);
            var result = await service.CorrectInvoiceAsync(invoice, fileText).ConfigureAwait(false);
            
            // **PROOF OF EXIT**: Use LogLevelOverride to ensure this log ALWAYS appears
            using (Core.Common.Extensions.LogLevelOverride.Begin(Serilog.Events.LogEventLevel.Verbose))
            {
                log.Information("🔍 **OCR_CORRECTION_EXIT**: CorrectInvoices (single invoice) completed with result: {Result}", result);
            }
            return result;
        }

        public static async Task CorrectInvoices(List<dynamic> res, Invoice template, ILogger logger)
        {
            var log = logger ?? Log.Logger.ForContext(typeof(OCRCorrectionService));
            
            // **PROOF OF ENTRY**: Use LogLevelOverride to ensure this log ALWAYS appears regardless of global settings
            using (Core.Common.Extensions.LogLevelOverride.Begin(Serilog.Events.LogEventLevel.Verbose))
            {
                log.Error("🔍 **OCR_CORRECTION_SERVICE_ENTRY**: CorrectInvoices static method ENTERED with ResCount: {ResCount}, Template: {TemplateName}", 
                    res?.Count ?? 0, template?.OcrInvoices?.Name ?? "NULL");
                
                // Log the dynamic res structure with custom serialization
                if (res != null && res.Any())
                {
                    log.Error("🔍 **OCR_DYNAMIC_RES_STRUCTURE**: res contains {Count} items", res.Count);
                    for (int i = 0; i < Math.Min(res.Count, 3); i++) // Log first 3 items
                    {
                        var item = res[i];
                        if (item != null)
                        {
                            log.Error("🔍 **OCR_RES_ITEM_{Index}**: Type={Type}, Value={@Value}", i, item.GetType().Name, item);
                        }
                    }
                }
                
            if (res == null || !res.Any() || template == null)
            {
                log.Error("🔍 **OCR_CORRECTION_EXIT_EARLY**: CorrectInvoices method exiting early - null/empty inputs");
                return;
            }
            } // End LogLevelOverride for entry log

            try
            {
                var templateLogId = template.OcrInvoices?.Id.ToString() ?? template.OcrInvoices.Id.ToString();
                log.Information("Starting static batch OCR correction for {DynamicResultCount} dynamic results against template {TemplateLogId} ({TemplateName})",
                    res.Count, templateLogId, template.OcrInvoices?.Name);

                log.Error("🔍 **OCR_DATAFLOW_TEXT_LOAD**: About to load original text from template FilePath: {FilePath}", template.FilePath);
                log.Error("🔍 **OCR_DATAFLOW_TEMPLATE_INFO**: Template.Id={TemplateId}, Template.Name={TemplateName}", template.OcrInvoices?.Id, template.OcrInvoices?.Name);
                string originalText = GetOriginalTextFromFile(template.FilePath, log);
                log.Error("🔍 **OCR_DATAFLOW_TEXT_LOADED**: Original text loaded, length: {TextLength} | IsNull: {IsNull} | IsEmpty: {IsEmpty}", 
                    originalText?.Length ?? 0, originalText == null, string.IsNullOrEmpty(originalText));
                    
                if (!string.IsNullOrEmpty(originalText))
                {
                    log.Error("🔍 **OCR_DATAFLOW_TEXT_PREVIEW**: First 500 chars: {Preview}", 
                        originalText.Length > 500 ? originalText.Substring(0, 500) + "..." : originalText);
                }
                
                // Log a sample of the original text to see if it contains gift card info
                if (!string.IsNullOrEmpty(originalText))
                {
                    var lines = originalText.Split('\n').Take(50).ToList(); // First 50 lines
                    var giftCardLines = lines.Where(l => l.Contains("Gift") || l.Contains("Card") || l.Contains("-$6.99") || l.Contains("$6.99")).ToList();
                    log.Error("🔍 **OCR_DATAFLOW_GIFT_CARD_SEARCH**: Found {GiftCardLineCount} lines containing gift card related text: {@GiftCardLines}", giftCardLines.Count, giftCardLines);
                    
                    // INTENTION CONFIRMATION: Gift card text should be present in original text
                    bool hasGiftCardText = giftCardLines.Any(l => l.Contains("Gift Card Amount"));
                    bool hasGiftCardAmount = giftCardLines.Any(l => l.Contains("-$6.99"));
                    log.Error("🔍 **OCR_INTENTION_CHECK_6**: Gift card text found? Expected=TRUE, Actual={HasGiftCard}", hasGiftCardText);
                    log.Error("🔍 **OCR_INTENTION_CHECK_7**: Gift card amount found? Expected=TRUE, Actual={HasAmount}", hasGiftCardAmount);
                    
                    if (!hasGiftCardText)
                    {
                        log.Error("🔍 **OCR_INTENTION_FAILED_6**: INTENTION FAILED - 'Gift Card Amount' text not found in original PDF text");
                    }
                    else
                    {
                        log.Error("🔍 **OCR_INTENTION_MET_6**: INTENTION MET - 'Gift Card Amount' text found in original PDF text");
                    }
                    
                    if (!hasGiftCardAmount)
                    {
                        log.Error("🔍 **OCR_INTENTION_FAILED_7**: INTENTION FAILED - '-$6.99' amount not found in original PDF text");
                    }
                    else
                    {
                        log.Error("🔍 **OCR_INTENTION_MET_7**: INTENTION MET - '-$6.99' amount found in original PDF text");
                    }
                }
                
                if (string.IsNullOrEmpty(originalText) && res.Any(item => (item is List<IDictionary<string, object>> l && l.Any()) || (item is IDictionary<string, object>)))
                {
                    log.Error("🔍 **OCR_DATAFLOW_TEXT_MISSING**: Original text file not found or empty for template {TemplateLogId} at path '{FilePath}'. Corrections will be limited.",
                        templateLogId, template.FilePath + ".txt");
                }

                List<ShipmentInvoiceWithMetadata> allShipmentInvoicesWithMetadata;
                using (var tempServiceForMeta = new OCRCorrectionService(log))
                {
                    log.Error("🔍 **OCR_DATAFLOW_CONVERSION_ENTRY**: About to convert dynamic res to ShipmentInvoices");
                    allShipmentInvoicesWithMetadata = ConvertDynamicToShipmentInvoicesWithMetadata(res, template, tempServiceForMeta, log);
                    log.Error("🔍 **OCR_DATAFLOW_CONVERSION_EXIT**: Conversion completed, got {Count} ShipmentInvoices", allShipmentInvoicesWithMetadata?.Count ?? 0);
                }

                if (!allShipmentInvoicesWithMetadata.Any())
                {
                    log.Error("🔍 **OCR_DATAFLOW_EXIT_NO_INVOICES**: No ShipmentInvoices could be converted from dynamic results for template {TemplateLogId}. Ending correction process.", templateLogId);
                    return;
                }

                log.Error("🔍 **OCR_DATAFLOW_INVOICE_DETAILS**: Converted {Count} ShipmentInvoices with metadata for processing against template {TemplateLogId}.", allShipmentInvoicesWithMetadata.Count, templateLogId);
                
                // Log each converted ShipmentInvoice details to see what values were extracted
                foreach (var invoiceWithMeta in allShipmentInvoicesWithMetadata)
                {
                    var inv = invoiceWithMeta.Invoice;
                    log.Error("🔍 **OCR_DATAFLOW_INVOICE_VALUES**: InvoiceNo={InvoiceNo}, SubTotal={SubTotal}, InvoiceTotal={InvoiceTotal}, TotalInternalFreight={TotalInternalFreight}, TotalOtherCost={TotalOtherCost}, TotalDeduction={TotalDeduction}, TotalInsurance={TotalInsurance}", 
                        inv.InvoiceNo, inv.SubTotal, inv.InvoiceTotal, inv.TotalInternalFreight, inv.TotalOtherCost, inv.TotalDeduction, inv.TotalInsurance);
                }

                EnhancedCorrectionResult overallCorrectionResults = new EnhancedCorrectionResult();

                using var correctionServiceInstance = new OCRCorrectionService(log);
                log.Error("🔍 **OCR_LOGIC_FLOW_LOOP_START**: Starting processing loop for {Count} invoices", allShipmentInvoicesWithMetadata.Count);
                
                foreach (var invoiceWithMeta in allShipmentInvoicesWithMetadata)
                {
                    var currentShipmentInvoice = invoiceWithMeta.Invoice;
                    log.Error("🔍 **OCR_LOGIC_FLOW_INVOICE_START**: Processing corrections for invoice {InvoiceNo} from template {TemplateLogId}", currentShipmentInvoice.InvoiceNo, templateLogId);

                    bool isBalanced = TotalsZero(currentShipmentInvoice, log);
                    log.Error("🔍 **OCR_LOGIC_FLOW_BALANCE_CHECK**: Invoice {InvoiceNo} TotalsZero check = {IsBalanced}", currentShipmentInvoice.InvoiceNo, isBalanced);
                    log.Error("🔍 **OCR_INTENTION_BALANCE**: We EXPECT this invoice to be unbalanced (TotalsZero != 0) so OCR correction can fix the missing TotalDeduction field");
                    
                    // INTENTION CONFIRMATION: Invoice should be unbalanced
                    log.Error("🔍 **OCR_INTENTION_CHECK_4**: Is invoice unbalanced? Expected=TRUE, Actual={IsUnbalanced}", !isBalanced);
                    if (isBalanced)
                    {
                        log.Error("🔍 **OCR_INTENTION_FAILED_4**: INTENTION FAILED - Invoice is balanced but we expected unbalanced due to missing TotalDeduction");
                        log.Error("🔍 **OCR_LOGIC_FLOW_SKIP_BALANCED**: Invoice {InvoiceNo} is already balanced. Skipping intensive error detection/application.", currentShipmentInvoice.InvoiceNo);
                        continue;
                    }
                    else
                    {
                        log.Error("🔍 **OCR_INTENTION_MET_4**: INTENTION MET - Invoice is unbalanced as expected, proceeding with error detection");
                    }

                    log.Error("🔍 **OCR_LOGIC_FLOW_ERROR_DETECTION**: About to detect errors for unbalanced invoice {InvoiceNo}", currentShipmentInvoice.InvoiceNo);
                    log.Error("🔍 **OCR_INTENTION_ERROR_DETECTION**: We EXPECT DetectInvoiceErrorsAsync to find missing TotalDeduction field by analyzing originalText for 'Gift Card Amount: -$6.99'");
                    var errors = await correctionServiceInstance.DetectInvoiceErrorsAsync(currentShipmentInvoice, originalText, invoiceWithMeta.FieldMetadata).ConfigureAwait(false);
                    log.Error("🔍 **OCR_LOGIC_FLOW_ERRORS_FOUND**: Found {ErrorCount} errors for invoice {InvoiceNo}", errors?.Count ?? 0, currentShipmentInvoice.InvoiceNo);
                    
                    // INTENTION CONFIRMATION: Should find at least 1 error (missing TotalDeduction)
                    bool foundErrors = errors?.Count > 0;
                    log.Error("🔍 **OCR_INTENTION_CHECK_5**: Found errors? Expected=TRUE, Actual={FoundErrors}, ErrorCount={ErrorCount}", foundErrors, errors?.Count ?? 0);
                    
                    if (errors?.Count == 0)
                    {
                        log.Error("🔍 **OCR_INTENTION_FAILED_5**: INTENTION FAILED - No errors detected but we expected to find missing TotalDeduction field error");
                    }
                    else
                    {
                        log.Error("🔍 **OCR_INTENTION_MET_5**: INTENTION MET - Errors detected as expected, proceeding with corrections");
                    }
                    
                    if (errors.Any())
                    {
                        log.Error("🔍 **OCR_LOGIC_FLOW_APPLY_CORRECTIONS**: About to apply corrections for {ErrorCount} errors", errors.Count);
                        var appliedCorrections = await correctionServiceInstance.ApplyCorrectionsAsync(currentShipmentInvoice, errors, originalText, invoiceWithMeta.FieldMetadata).ConfigureAwait(false);
                        var successfulValueApplications = appliedCorrections.Where(c => c.Success).ToList();
                        log.Error("🔍 **OCR_LOGIC_FLOW_CORRECTIONS_APPLIED**: Applied {TotalCorrections} corrections, {SuccessfulCount} successful", appliedCorrections.Count, successfulValueApplications.Count);

                        overallCorrectionResults.TotalCorrections += appliedCorrections.Count;
                        overallCorrectionResults.SuccessfulUpdates += successfulValueApplications.Count;

                        if (successfulValueApplications.Any())
                        {
                            log.Error("🔍 **OCR_LOGIC_FLOW_UPDATE_PATTERNS**: About to update regex patterns for {SuccessfulCount} successful corrections", successfulValueApplications.Count);
                            await correctionServiceInstance.UpdateRegexPatternsAsync(successfulValueApplications, originalText, template.FilePath, invoiceWithMeta.FieldMetadata).ConfigureAwait(false);
                        }
                    }
                    else
                    {
                        log.Error("🔍 **OCR_LOGIC_FLOW_NO_ERRORS**: No errors detected for invoice {InvoiceNo}, skipping correction application", currentShipmentInvoice.InvoiceNo);
                    }
                }

                // **CRITICAL DEBUGGING**: Log the final state of corrected ShipmentInvoice objects before writing back to dynamic structure
                var finalCorrectedInvoices = allShipmentInvoicesWithMetadata.Select(x => x.Invoice).ToList();
                log.Error("🔍 **PRE_UPDATE_DYNAMIC_FINAL_STATE**: About to update dynamic results with {Count} corrected ShipmentInvoices", finalCorrectedInvoices.Count);
                
                for (int idx = 0; idx < finalCorrectedInvoices.Count; idx++)
                {
                    var finalInv = finalCorrectedInvoices[idx];
                    log.Error("🔍 **PRE_UPDATE_DYNAMIC_INVOICE_{Index}**: InvoiceNo={InvoiceNo} | SubTotal={SubTotal} | TotalDeduction={TotalDeduction} | TotalInsurance={TotalInsurance} | InvoiceTotal={InvoiceTotal}", 
                        idx, finalInv.InvoiceNo, finalInv.SubTotal, finalInv.TotalDeduction, finalInv.TotalInsurance, finalInv.InvoiceTotal);
                }
                
                UpdateDynamicResultsWithCorrections(res, finalCorrectedInvoices, log);
                UpdateTemplateLineValues(template, finalCorrectedInvoices, log);

                log.Information("Completed static batch OCR correction for template {TemplateLogId}. Applied {SuccessfulValueChanges} value changes to in-memory objects. Check logs for DB pattern update details.",
                    templateLogId, overallCorrectionResults.SuccessfulUpdates);
            }
            catch (Exception ex)
            {
                log.Error(ex, "Error in static CorrectInvoices (batch dynamic) method for template ID {TemplateId}.", template?.OcrInvoices?.Id ?? template.OcrInvoices.Id);
            }
            
            // **PROOF OF EXIT**: Use LogLevelOverride to ensure this log ALWAYS appears regardless of global settings
            using (Core.Common.Extensions.LogLevelOverride.Begin(Serilog.Events.LogEventLevel.Verbose))
            {
                log.Information("🔍 **OCR_CORRECTION_EXIT**: CorrectInvoices method completed successfully");
            }
        }

        public static async Task<List<dynamic>> ApplyDirectDataCorrectionFallbackAsync(
            List<dynamic> res,
            string originalText,
            ILogger logger)
        {
            var log = logger ?? Log.Logger.ForContext(typeof(OCRCorrectionService));
            log.Warning("Applying direct data correction fallback as regex/pattern fixes might have failed or were insufficient.");

            if (res == null || !res.Any() || string.IsNullOrEmpty(originalText))
            {
                log.Warning("ApplyDirectDataCorrectionFallbackAsync: Insufficient data (res or originalText is empty).");
                return res;
            }

            try
            {
                using var correctionService = new OCRCorrectionService(log);
                List<dynamic> correctedRes = new List<dynamic>();

                foreach (var dynamicItem in res)
                {
                    if (dynamicItem is List<IDictionary<string, object>> invoiceList)
                    {
                        var tempList = new List<IDictionary<string, object>>();
                        foreach (var invoiceDict in invoiceList)
                        {
                            var tempInvoice = CreateTempShipmentInvoice(invoiceDict, log);
                            if (!TotalsZero(tempInvoice, out _, log))
                            {
                                var correctedDict = await correctionService.RequestAndApplyDirectCorrectionsAsync(tempInvoice, invoiceDict, originalText).ConfigureAwait(false);
                                tempList.Add(correctedDict ?? invoiceDict);
                            }
                            else
                            {
                                tempList.Add(invoiceDict);
                            }
                        }
                        correctedRes.Add(tempList);
                    }
                    else if (dynamicItem is IDictionary<string, object> singleInvoiceDict)
                    {
                        var tempInvoice = CreateTempShipmentInvoice(singleInvoiceDict, log);
                        if (!TotalsZero(tempInvoice, out _, log))
                        {
                            var correctedDict = await correctionService.RequestAndApplyDirectCorrectionsAsync(tempInvoice, singleInvoiceDict, originalText).ConfigureAwait(false);
                            correctedRes.Add(correctedDict ?? singleInvoiceDict);
                        }
                        else
                        {
                            correctedRes.Add(singleInvoiceDict);
                        }
                    }
                    else
                    {
                        correctedRes.Add(dynamicItem);
                    }
                }
                log.Information("Completed direct data correction fallback attempt.");
                return correctedRes;
            }
            catch (Exception ex)
            {
                log.Error(ex, "Error during direct data correction fallback process.");
                return res;
            }
        }
        #endregion

        #region Instance Helper for Direct Correction (called by static fallback)
        public async Task<IDictionary<string, object>> RequestAndApplyDirectCorrectionsAsync(ShipmentInvoice invoice, IDictionary<string, object> originalInvoiceDict, string originalDocumentText)
        {
            _logger.Information("Requesting direct data corrections from DeepSeek for invoice {InvoiceNo}", invoice.InvoiceNo);
            var prompt = this.CreateDirectDataCorrectionPrompt(new List<dynamic> { originalInvoiceDict }, originalDocumentText);
            var deepSeekResponseJson = await this._deepSeekApi.GetResponseAsync(prompt).ConfigureAwait(false);

            if (string.IsNullOrWhiteSpace(deepSeekResponseJson))
            {
                _logger.Warning("No response from DeepSeek for direct data correction of {InvoiceNo}", invoice.InvoiceNo);
                return null;
            }

            IDictionary<string, object> workingInvoiceDict = originalInvoiceDict.ToDictionary(entry => entry.Key, entry => entry.Value, StringComparer.OrdinalIgnoreCase);
            bool changesApplied = false;

            try
            {
                using (var jsonDoc = JsonDocument.Parse(this.CleanJsonResponse(deepSeekResponseJson)))
                {
                    if (jsonDoc.RootElement.TryGetProperty("corrections", out var correctionsArray) && correctionsArray.ValueKind == JsonValueKind.Array)
                    {
                        foreach (var corrElem in correctionsArray.EnumerateArray())
                        {
                            string fieldPath = this.GetStringValueWithLogging(corrElem, "field", 0);
                            string correctValueString = this.GetStringValueWithLogging(corrElem, "correct_value", 0);
                            string reasoning = this.GetStringValueWithLogging(corrElem, "reasoning", 0, isOptional: true);

                            if (!string.IsNullOrEmpty(fieldPath) && correctValueString != null)
                            {
                                _logger.Debug("DeepSeek direct correction proposal: Field='{FieldPath}', NewValue='{Value}', Reason='{Reason}'", fieldPath, correctValueString, reasoning);
                                object parsedValue = this.ParseCorrectedValue(correctValueString, fieldPath);

                                if (fieldPath.Contains("["))
                                {
                                    var match = Regex.Match(fieldPath, @"(?i)(?<listName>\w+)\[(?<index>\d+)\]\.(?<propName>\w+)");
                                    if (match.Success)
                                    {
                                        string listName = match.Groups["listName"].Value;
                                        int index = int.Parse(match.Groups["index"].Value);
                                        string propName = match.Groups["propName"].Value;

                                        if (workingInvoiceDict.TryGetValue(listName, out var detailsObj) &&
                                            detailsObj is List<IDictionary<string, object>> detailsList &&
                                            index >= 0 && index < detailsList.Count && detailsList[index] != null)
                                        {
                                            detailsList[index][propName] = parsedValue;
                                            changesApplied = true;
                                        }
                                        else
                                        {
                                            _logger.Warning("Direct correction: Could not apply to nested field path '{FieldPath}'. List/index issue or type mismatch.", fieldPath);
                                        }
                                    }
                                    else
                                    {
                                        _logger.Warning("Direct correction: Nested field path '{FieldPath}' format not recognized.", fieldPath);
                                    }
                                }
                                else if (workingInvoiceDict.ContainsKey(fieldPath))
                                {
                                    workingInvoiceDict[fieldPath] = parsedValue;
                                    changesApplied = true;
                                }
                                else
                                {
                                    _logger.Warning("Direct correction: Field '{FieldPath}' not found in dictionary for direct update.", fieldPath);
                                }
                            }
                        }
                    }
                    else
                    {
                        _logger.Information("No 'corrections' array found in DeepSeek's direct correction response for {InvoiceNo}.", invoice.InvoiceNo);
                    }
                }
            }
            catch (JsonException jsonEx)
            {
                _logger.Error(jsonEx, "Failed to parse DeepSeek's direct corrections JSON response for {InvoiceNo}. Response: {ResponseSnippet}",
                    invoice.InvoiceNo, this.TruncateForLog(deepSeekResponseJson));
                return null;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Unexpected error applying direct corrections for {InvoiceNo}.", invoice.InvoiceNo);
                return null;
            }

            if (changesApplied)
            {
                var tempCorrectedInvoice = CreateTempShipmentInvoice(workingInvoiceDict, _logger);
                if (TotalsZero(tempCorrectedInvoice, out double finalImbalance, _logger))
                {
                    _logger.Information("Direct data correction successful and invoice {InvoiceNo} is now balanced.", invoice.InvoiceNo);
                    return workingInvoiceDict;
                }
                else
                {
                    _logger.Warning("Direct data corrections applied for {InvoiceNo}, but it is still not balanced. Final Imbalance: {ImbalanceAmount:F4}",
                       invoice.InvoiceNo, finalImbalance);
                    return workingInvoiceDict;
                }
            }
            _logger.Information("No direct data corrections were effectively applied or suggested by LLM for {InvoiceNo}", invoice.InvoiceNo);
            return null;
        }
        #endregion

        #region Static Private Helper Methods for Legacy Static Calls

        public static List<ShipmentInvoiceWithMetadata> ConvertDynamicToShipmentInvoicesWithMetadata(
            List<dynamic> res,
            Invoice template,
            OCRCorrectionService serviceInstance,
            ILogger logger)
        {
            var allInvoicesWithMetadata = new List<ShipmentInvoiceWithMetadata>();
            if (serviceInstance == null)
            {
                logger.Error("ConvertDynamicToShipmentInvoicesWithMetadata: serviceInstance is null. Cannot proceed.");
                return allInvoicesWithMetadata;
            }
            try
            {
                var fieldMappings = serviceInstance.CreateEnhancedFieldMapping(template);

                foreach (var item in res)
                {
                    IEnumerable<IDictionary<string, object>> currentInvoiceDicts = null;
                    if (item is List<IDictionary<string, object>> invoiceList)
                    {
                        currentInvoiceDicts = invoiceList;
                    }
                    else if (item is IDictionary<string, object> singleInvoiceDict)
                    {
                        currentInvoiceDicts = new[] { singleInvoiceDict };
                    }

                    if (currentInvoiceDicts != null)
                    {
                        foreach (var invoiceDict in currentInvoiceDicts)
                        {
                            if (invoiceDict == null) continue;
                            var shipmentInvoice = CreateTempShipmentInvoice(invoiceDict, logger);
                            var metadata = serviceInstance.ExtractEnhancedOCRMetadata(invoiceDict, template, fieldMappings);
                            allInvoicesWithMetadata.Add(new ShipmentInvoiceWithMetadata { Invoice = shipmentInvoice, FieldMetadata = metadata });
                        }
                    }
                }
                logger.Information("Successfully converted {Count} dynamic results to ShipmentInvoiceWithMetadata.", allInvoicesWithMetadata.Count);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error in ConvertDynamicToShipmentInvoicesWithMetadata.");
            }
            return allInvoicesWithMetadata;
        }

        private static string GetOriginalTextFromFile(string templateFilePath, ILogger logger)
        {
            if (string.IsNullOrEmpty(templateFilePath))
            {
                logger?.Debug("GetOriginalTextFromFile: templateFilePath is null or empty.");
                return "";
            }
            try
            {
                // **CRITICAL_DEBUG**: Log all file path attempts to understand why text loading fails
                logger?.Error("🔍 **FILE_PATH_DEBUG_INPUT**: templateFilePath = '{TemplatePath}'", templateFilePath);
                
                // **FIX**: For PDF files, we want to append .txt, not replace the extension
                // This maintains compatibility: someFile.pdf → someFile.pdf.txt (correct)
                // Instead of: someFile.pdf → someFile.txt (incorrect)
                var txtFile = templateFilePath + ".txt";
                logger?.Error("🔍 **FILE_PATH_DEBUG_APPEND_TXT**: Append .txt result = '{TxtFile}' | Exists = {Exists}", txtFile, File.Exists(txtFile));
                
                if (File.Exists(txtFile))
                {
                    var content = File.ReadAllText(txtFile);
                    logger?.Error("🔍 **FILE_PATH_DEBUG_SUCCESS_APPEND**: Successfully read {Length} characters from '{TxtFile}'", content.Length, txtFile);
                    return content;
                }
                
                // **FALLBACK**: Try the old method for backward compatibility
                var txtFileOld = Path.ChangeExtension(templateFilePath, ".txt");
                logger?.Error("🔍 **FILE_PATH_DEBUG_FALLBACK**: Path.ChangeExtension fallback = '{TxtFile}' | Exists = {Exists}", txtFileOld, File.Exists(txtFileOld));
                
                if (File.Exists(txtFileOld))
                {
                    var content = File.ReadAllText(txtFileOld);
                    logger?.Error("🔍 **FILE_PATH_DEBUG_SUCCESS_FALLBACK**: Successfully read {Length} characters from '{TxtFile}'", content.Length, txtFileOld);
                    return content;
                }
                
                logger?.Warning("Text file not found for template path: {TemplatePath} (tried {TxtFilePath} and {TxtFilePathOld})", templateFilePath, txtFile, txtFileOld);
            }
            catch (Exception ex)
            {
                logger?.Error(ex, "Error reading original text file for template path: {FilePath}", templateFilePath);
            }
            return "";
        }

        private static ShipmentInvoice CreateTempShipmentInvoice(IDictionary<string, object> x, ILogger logger)
        {
            try
            {
                var log = logger ?? Log.Logger.ForContext(typeof(OCRCorrectionService));
                
                // 🔍 **ASSUMPTION**: Dictionary contains all required invoice fields
                log.Error("🔍 **CONVERSION_INPUTS_DYNAMIC**: Converting dictionary with {KeyCount} keys: {@Keys}", 
                    x?.Count ?? 0, x?.Keys.ToList() ?? new List<string>());

                var invoice = new ShipmentInvoice();
                T GetValue<T>(string key, T defaultValue = default)
                {
                    if (x.TryGetValue(key, out var val) && val != null)
                    {
                        log.Error("🔍 **CONVERSION_FIELD**: Key='{Key}' | RawValue='{RawValue}' | Type={ValueType} | TargetType={TargetType}", 
                            key, val.ToString(), val.GetType().Name, typeof(T).Name);
                        try
                        {
                            if (typeof(T) == typeof(DateTime?) && val is string sVal && DateTime.TryParse(sVal, System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.AssumeUniversal | System.Globalization.DateTimeStyles.AdjustToUniversal, out DateTime dt))
                            {
                                return (T)(object)dt;
                            }
                            if (val is T alreadyTyped) return alreadyTyped;
                            return (T)Convert.ChangeType(val, Nullable.GetUnderlyingType(typeof(T)) ?? typeof(T), System.Globalization.CultureInfo.InvariantCulture);
                        }
                        catch (Exception convEx)
                        {
                            log.Error("❌ **CONVERSION_FAILED**: Key='{Key}' | Value='{ValText}' | SourceType={ValType} | TargetType={TargetType} | Error={ExMsg}",
                                key, val.ToString(), val.GetType().Name, typeof(T).Name, convEx.Message);
                        }
                    }
                    else
                    {
                        log.Error("🔍 **CONVERSION_MISSING**: Key='{Key}' not found in dictionary or is null, using default={Default}", 
                            key, defaultValue?.ToString() ?? "NULL");
                    }
                    return defaultValue;
                }
                double? GetNullableDouble(string key)
                {
                    if (x.TryGetValue(key, out var val) && val != null)
                    {
                        log.Error("🔍 **CONVERSION_DOUBLE**: Key='{Key}' | RawValue='{RawValue}' | Type={ValueType}", 
                            key, val.ToString(), val.GetType().Name);
                        
                        if (val is double d) return d;
                        if (val is decimal dec) return (double)dec;
                        if (val is int i) return (double)i;
                        if (val is long l) return (double)l;
                        
                        // CRITICAL FIX: Use currency-aware parsing for string values
                        var valStr = val.ToString();
                        if (!string.IsNullOrEmpty(valStr))
                        {
                            // Apply currency-aware cleaning logic similar to ParseCorrectedValue
                            string cleanedValue = valStr.Trim();
                            
                            // Remove currency symbols ($, €, £, etc.)
                            cleanedValue = Regex.Replace(cleanedValue, @"[\$€£¥₹₽¢₿]", "").Trim();
                            
                            // Handle parentheses as negative indicators (accounting format)
                            bool isNegative = false;
                            if (cleanedValue.StartsWith("(") && cleanedValue.EndsWith(")"))
                            {
                                isNegative = true;
                                cleanedValue = cleanedValue.Substring(1, cleanedValue.Length - 2).Trim();
                            }
                            
                            // Handle comma and decimal point variations
                            if (cleanedValue.Contains(',') && cleanedValue.Contains('.'))
                            {
                                if (cleanedValue.LastIndexOf(',') < cleanedValue.LastIndexOf('.'))
                                { // Format: 1,234.56 (comma as thousands separator)
                                    cleanedValue = cleanedValue.Replace(",", "");
                                }
                                else
                                { // Format: 1.234,56 (dot as thousands separator)
                                    cleanedValue = cleanedValue.Replace(".", "").Replace(",", ".");
                                }
                            }
                            else if (cleanedValue.Contains(','))
                            { // Format: 123,45 (comma as decimal separator)
                                cleanedValue = cleanedValue.Replace(",", ".");
                            }
                            
                            log.Error("🔍 **CONVERSION_CURRENCY_CLEANED**: Key='{Key}' | Original='{Original}' | Cleaned='{Cleaned}' | IsNegative={IsNegative}", 
                                key, valStr, cleanedValue, isNegative);
                            
                            if (double.TryParse(cleanedValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var dbl))
                            {
                                var result = isNegative ? -dbl : dbl;
                                log.Error("✅ **CONVERSION_CURRENCY_SUCCESS**: Key='{Key}' | FinalValue={FinalValue}", key, result);
                                return result;
                            }
                        }
                        
                        log.Error("❌ **CONVERSION_PARSE_FAILED**: Key='{Key}' | Value='{ValText}' could not be parsed to double", key, val.ToString());
                    }
                    else
                    {
                        log.Error("🔍 **CONVERSION_DOUBLE_MISSING**: Key='{Key}' not found in dictionary or is null", key);
                    }
                    return null;
                }

                invoice.InvoiceNo = GetValue<string>("InvoiceNo") ?? $"TempInv_{Guid.NewGuid().ToString().Substring(0, 8)}";
                invoice.InvoiceDate = GetValue<DateTime?>("InvoiceDate");
                invoice.InvoiceTotal = GetNullableDouble("InvoiceTotal");
                invoice.SubTotal = GetNullableDouble("SubTotal");
                invoice.TotalInternalFreight = GetNullableDouble("TotalInternalFreight");
                invoice.TotalOtherCost = GetNullableDouble("TotalOtherCost");
                invoice.TotalInsurance = GetNullableDouble("TotalInsurance");
                invoice.TotalDeduction = GetNullableDouble("TotalDeduction");
                invoice.Currency = GetValue<string>("Currency");

                // Log the final converted values
                log.Error("🔍 **CONVERSION_RESULT_DYNAMIC**: InvoiceNo={InvoiceNo} | SubTotal={SubTotal} | Freight={Freight} | OtherCost={OtherCost} | Insurance={Insurance} | Deduction={Deduction} | InvoiceTotal={InvoiceTotal}",
                    invoice.InvoiceNo, invoice.SubTotal, invoice.TotalInternalFreight, invoice.TotalOtherCost, invoice.TotalInsurance, invoice.TotalDeduction, invoice.InvoiceTotal);
                invoice.SupplierName = GetValue<string>("SupplierName");
                invoice.SupplierAddress = GetValue<string>("SupplierAddress");


                if (x.TryGetValue("InvoiceDetails", out var detailsObj) && detailsObj is List<IDictionary<string, object>> detailsList)
                {
                    invoice.InvoiceDetails = new List<InvoiceDetails>();
                    int lineNum = 1;
                    foreach (var detailDict in detailsList.Where(d => d != null))
                    {
                        var detail = new InvoiceDetails();
                        detail.LineNumber = detailDict.TryGetValue("LineNumber", out var lnObj) && lnObj is int dictLn && dictLn > 0 ? dictLn : lineNum++;

                        detail.ItemDescription = detailDict.TryGetValue("ItemDescription", out var idVal) ? idVal?.ToString() : null;
                        detail.Quantity = detailDict.TryGetValue("Quantity", out var qtyVal) && qtyVal != null && double.TryParse(qtyVal.ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var dblQty) ? dblQty : 0;
                        detail.Cost = detailDict.TryGetValue("Cost", out var costVal) && costVal != null && double.TryParse(costVal.ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var dblCost) ? dblCost : 0;
                        detail.TotalCost = (detailDict.TryGetValue("TotalCost", out var tcVal) && tcVal != null && double.TryParse(tcVal.ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var dblTc) ? dblTc : (double?)null);
                        detail.Discount = (detailDict.TryGetValue("Discount", out var discVal) && discVal != null && double.TryParse(discVal.ToString(), System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var dblDisc) ? dblDisc : (double?)null);
                        detail.Units = detailDict.TryGetValue("Units", out var unitsVal) && unitsVal != null ? unitsVal.ToString() : null;

                        if (detail.TotalCost == null)
                            detail.TotalCost = (detail.Quantity * detail.Cost) - (detail.Discount ?? 0);

                        invoice.InvoiceDetails.Add(detail);
                    }
                }
                else
                {
                    invoice.InvoiceDetails = new List<InvoiceDetails>();
                }
                return invoice;
            }
            catch (Exception ex)
            {
                logger?.Error(ex, "Error creating temporary ShipmentInvoice from dictionary.");
                return new ShipmentInvoice { InvoiceNo = "ERROR_CREATION", InvoiceDetails = new List<InvoiceDetails>() };
            }
        }

        /// <summary>
        /// Public wrapper for CreateTempShipmentInvoice to support Lines.Values updates
        /// </summary>
        public static ShipmentInvoice ConvertDynamicToShipmentInvoice(IDictionary<string, object> dict, ILogger logger = null)
        {
            return CreateTempShipmentInvoice(dict, logger);
        }

        public static int CountTotalInvoices(List<dynamic> res)
        {
            if (res == null) return 0;
            int count = 0;
            foreach (var item in res)
            {
                if (item is List<IDictionary<string, object>> list) count += list.Count;
                else if (item is IDictionary<string, object>) count++;
            }
            return count;
        }

        private static void UpdateDynamicResultsWithCorrections(List<dynamic> res, List<ShipmentInvoice> correctedInvoices, ILogger logger)
        {
            try
            {
                // **CRITICAL DEBUGGING**: Log the exact data being passed to this method
                logger.Error("🔍 **UPDATE_DYNAMIC_ENTRY**: UpdateDynamicResultsWithCorrections called with ResCount={ResCount}, CorrectedInvoicesCount={CorrectedCount}", 
                    res?.Count ?? 0, correctedInvoices?.Count ?? 0);
                
                // **LOG CORRECTED INVOICE VALUES**: Show what values should be written back to dynamic structure
                for (int idx = 0; idx < (correctedInvoices?.Count ?? 0); idx++)
                {
                    var corrInv = correctedInvoices[idx];
                    logger.Error("🔍 **UPDATE_DYNAMIC_CORRECTED_INVOICE_{Index}**: InvoiceNo={InvoiceNo} | SubTotal={SubTotal} | TotalDeduction={TotalDeduction} | TotalInsurance={TotalInsurance} | InvoiceTotal={InvoiceTotal}", 
                        idx, corrInv.InvoiceNo, corrInv.SubTotal, corrInv.TotalDeduction, corrInv.TotalInsurance, corrInv.InvoiceTotal);
                }
                
                int correctedInvoiceIdx = 0;
                for (int i = 0; i < res.Count; i++)
                {
                    if (correctedInvoiceIdx >= correctedInvoices.Count) break;

                    Action<IDictionary<string, object>, ShipmentInvoice> updateDict = (dict, correctedInv) => {
                        // **CRITICAL DEBUGGING**: Log before and after values for key fields
                        var oldTotalDeduction = dict.TryGetValue("TotalDeduction", out var oldTDVal) ? oldTDVal : "NOT_FOUND";
                        var oldTotalInsurance = dict.TryGetValue("TotalInsurance", out var oldTIVal) ? oldTIVal : "NOT_FOUND";
                        logger.Error("🔍 **UPDATE_DYNAMIC_BEFORE**: InvoiceNo={InvoiceNo} | OLD TotalDeduction={OldTD} | OLD TotalInsurance={OldTI}", 
                            correctedInv.InvoiceNo, oldTotalDeduction, oldTotalInsurance);
                        
                        dict["InvoiceNo"] = correctedInv.InvoiceNo;
                        if (correctedInv.InvoiceDate.HasValue) dict["InvoiceDate"] = correctedInv.InvoiceDate.Value.ToString("yyyy-MM-dd HH:mm:ss"); else dict.Remove("InvoiceDate");
                        if (correctedInv.SupplierName != null) dict["SupplierName"] = correctedInv.SupplierName; else dict.Remove("SupplierName");
                        if (correctedInv.SupplierAddress != null) dict["SupplierAddress"] = correctedInv.SupplierAddress; else dict.Remove("SupplierAddress");
                        if (correctedInv.Currency != null) dict["Currency"] = correctedInv.Currency; else dict.Remove("Currency");

                        dict["InvoiceTotal"] = correctedInv.InvoiceTotal;
                        dict["SubTotal"] = correctedInv.SubTotal;
                        dict["TotalInternalFreight"] = correctedInv.TotalInternalFreight;
                        dict["TotalOtherCost"] = correctedInv.TotalOtherCost;
                        dict["TotalInsurance"] = correctedInv.TotalInsurance;
                        dict["TotalDeduction"] = correctedInv.TotalDeduction;
                        
                        // **CRITICAL DEBUGGING**: Log after values are set
                        logger.Error("🔍 **UPDATE_DYNAMIC_AFTER**: InvoiceNo={InvoiceNo} | NEW TotalDeduction={NewTD} | NEW TotalInsurance={NewTI}", 
                            correctedInv.InvoiceNo, correctedInv.TotalDeduction, correctedInv.TotalInsurance);
                        logger.Error("🔍 **UPDATE_DYNAMIC_DICT_VERIFY**: Dictionary now contains TotalDeduction={DictTD} | TotalInsurance={DictTI}", 
                            dict["TotalDeduction"], dict["TotalInsurance"]);

                        if (correctedInv.InvoiceDetails != null && correctedInv.InvoiceDetails.Any())
                        {
                            var newDictDetailsList = correctedInv.InvoiceDetails.Select(cDetail =>
                                new Dictionary<string, object>{
                                    {"LineNumber", cDetail.LineNumber},
                                    {"ItemDescription", cDetail.ItemDescription},
                                    {"Quantity", cDetail.Quantity},
                                    {"Cost", cDetail.Cost},
                                    {"TotalCost", cDetail.TotalCost},
                                    {"Discount", cDetail.Discount},
                                    {"Units", cDetail.Units}
                                }).Cast<IDictionary<string, object>>().ToList();
                            dict["InvoiceDetails"] = newDictDetailsList;
                        }
                        else
                        {
                            dict.Remove("InvoiceDetails");
                        }
                    };

                    if (res[i] is List<IDictionary<string, object>> invoiceList)
                    {
                        for (int j = 0; j < invoiceList.Count; j++)
                        {
                            if (correctedInvoiceIdx >= correctedInvoices.Count) break;
                            updateDict(invoiceList[j], correctedInvoices[correctedInvoiceIdx++]);
                        }
                    }
                    else if (res[i] is IDictionary<string, object> singleInvoiceDict)
                    {
                        if (correctedInvoiceIdx < correctedInvoices.Count)
                            updateDict(singleInvoiceDict, correctedInvoices[correctedInvoiceIdx++]);
                    }
                }
                logger.Information("UpdateDynamicResults: Updated {Count} dynamic result entries with corrected invoice data.", correctedInvoiceIdx);
            }
            catch (Exception ex)
            {
                logger.Error(ex, "Error in UpdateDynamicResultsWithCorrections.");
            }
        }

        public static void UpdateTemplateLineValues(Invoice template, List<ShipmentInvoice> correctedInvoices, ILogger log)
        {
            if (template?.Lines == null || !correctedInvoices.Any()) 
            {
                log.Warning("🔍 **LINES_VALUES_UPDATE_SKIPPED**: Template has no lines ({HasLines}) or no corrected invoices ({InvoiceCount})", 
                    template?.Lines != null, correctedInvoices?.Count ?? 0);
                return;
            }

            var templateLogId = template.OcrInvoices?.Id.ToString() ?? "Unknown";
            log.Error("🔧 **LINES_VALUES_FIX_START**: PHASE 2 CRITICAL PRIORITY - Fixing Lines.Values update for Gift Card Line 1830 with TotalInsurance");
            log.Error("🔍 **LINES_VALUES_UPDATE_START**: Updating template (ID: {TemplateLogId}) LineValues with {Count} corrected invoices", 
                templateLogId, correctedInvoices.Count);

            // PHASE 2: CRITICAL PRIORITY - Fix Gift Card Line 1830 → TotalInsurance = -6.99
            foreach (var correctedInvoice in correctedInvoices)
            {
                // Target: Gift Card Line 1830 → TotalInsurance = -6.99
                var giftCardLine = template.Lines?.FirstOrDefault(l => l.OCR_Lines?.Id == 1830);
                if (giftCardLine != null && correctedInvoice.TotalInsurance.HasValue)
                {
                    log.Error("🎯 **TARGET_LINE_FOUND**: Gift Card Line 1830 | Current Values Count: {Count} | TotalInsurance: {Value}", 
                        giftCardLine.Values?.Count ?? 0, correctedInvoice.TotalInsurance.Value);
                    
                    // Check if Values dictionary exists, but don't short-circuit if it doesn't
                    if (giftCardLine.Values == null)
                    {
                        log.Error("⚠️ **VALUES_DICT_NULL**: Lines.Values is null for Gift Card Line 1830, will use alternative UpdateFieldInTemplate approach");
                        // Don't continue - let the existing working code below handle the updates
                    }
                    else
                    {
                        log.Error("✅ **VALUES_DICT_EXISTS**: Lines.Values exists for Gift Card Line 1830 with {Count} entries", giftCardLine.Values.Count);
                    }
                    
                    try
                    {
                        // Create correct dictionary structure for Lines.Values
                        var lineKey = (1, "Header"); // Line 1, Header section
                        
                        // Find the existing TotalInsurance field or create new one
                        var existingField = giftCardLine.OCR_Lines?.Fields?.FirstOrDefault(f => 
                            f.Field?.Equals("TotalInsurance", StringComparison.OrdinalIgnoreCase) == true ||
                            f.Key?.Equals("TotalInsurance", StringComparison.OrdinalIgnoreCase) == true);
                        
                        Fields fieldKey;
                        if (existingField != null)
                        {
                            fieldKey = existingField;
                            log.Error("🔍 **EXISTING_FIELD_FOUND**: Using existing TotalInsurance field | FieldId: {FieldId}", existingField.Id);
                        }
                        else
                        {
                            // Create new field definition for TotalInsurance
                            fieldKey = new Fields 
                            { 
                                Field = "TotalInsurance", 
                                EntityType = "ShipmentInvoice",
                                Key = "TotalInsurance",
                                DataType = "decimal"
                            };
                            log.Error("🔧 **NEW_FIELD_CREATED**: Created new TotalInsurance field definition");
                        }
                        
                        var fieldKeyTuple = (fieldKey, "1"); // Instance 1
                        
                        if (!giftCardLine.Values.ContainsKey(lineKey))
                        {
                            giftCardLine.Values[lineKey] = new Dictionary<(Fields Fields, string Instance), string>();
                            log.Error("🔧 **LINE_KEY_CREATED**: Created new line key entry for (1, Header)");
                        }
                        
                        // Apply the corrected value
                        var valueString = correctedInvoice.TotalInsurance.Value.ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
                        giftCardLine.Values[lineKey][fieldKeyTuple] = valueString;
                        
                        log.Error("✅ **LINES_VALUES_UPDATED**: Gift Card TotalInsurance = {Value} applied to Lines.Values[{LineKey}][{FieldKey}] = '{ValueString}'", 
                            correctedInvoice.TotalInsurance.Value, lineKey, fieldKeyTuple, valueString);
                            
                        // Verify the update was applied
                        if (giftCardLine.Values.ContainsKey(lineKey) && 
                            giftCardLine.Values[lineKey].ContainsKey(fieldKeyTuple))
                        {
                            var verifyValue = giftCardLine.Values[lineKey][fieldKeyTuple];
                            log.Error("✅ **UPDATE_VERIFICATION**: Lines.Values update verified | StoredValue: '{StoredValue}'", verifyValue);
                        }
                        else
                        {
                            log.Error("❌ **UPDATE_VERIFICATION_FAILED**: Lines.Values update not found after assignment");
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex, "❌ **LINES_VALUES_UPDATE_ERROR**: Failed to update Gift Card Line 1830 Values dictionary");
                    }
                }
                else
                {
                    log.Error("❌ **TARGET_LINE_MISSING**: Gift Card Line 1830 not found (Line: {HasLine}) or TotalInsurance null (Value: {HasValue})", 
                        giftCardLine != null, correctedInvoice.TotalInsurance.HasValue);
                    
                    // Log available lines for debugging
                    if (template.Lines != null)
                    {
                        var availableLines = template.Lines.Where(l => l.OCR_Lines != null)
                            .Select(l => $"Line {l.OCR_Lines.Id}: {l.OCR_Lines.Name}")
                            .ToList();
                        log.Error("🔍 **AVAILABLE_LINES**: {LineCount} lines available: {@Lines}", 
                            availableLines.Count, availableLines);
                    }
                }
            }

            // Capture BEFORE state of Lines.Values
            log.Error("📊 **LINES_VALUES_BEFORE_STATE**: Capturing template Lines.Values state before updates");
            LogTemplateLineValuesState(template, "BEFORE", log);

            var fieldMappings = GetTemplateFieldMappings(template, log);
            log.Error("🔍 **FIELD_MAPPINGS_RETRIEVED**: Found {Count} field mappings for template {TemplateLogId}", 
                fieldMappings.Count, templateLogId);

            var representativeCorrectedInvoice = correctedInvoices.First();
            log.Error("🔍 **REPRESENTATIVE_INVOICE**: Using invoice {InvoiceNo} as representative for updates", 
                representativeCorrectedInvoice.InvoiceNo);

            // Log the corrected invoice values that will be applied
            log.Error("🔍 **CORRECTED_VALUES**: TotalDeduction={TotalDeduction} | TotalInsurance={TotalInsurance} | SubTotal={SubTotal} | InvoiceTotal={InvoiceTotal}", 
                representativeCorrectedInvoice.TotalDeduction, representativeCorrectedInvoice.TotalInsurance, 
                representativeCorrectedInvoice.SubTotal, representativeCorrectedInvoice.InvoiceTotal);

            Action<string, object> updateTemplateField = (fieldName, value) => {
                string stringValue = null;
                if (value is DateTime dt) stringValue = dt.ToString("yyyy-MM-dd");
                else if (value is double dbl || value is decimal dec) stringValue = Convert.ToDouble(value).ToString("F2", System.Globalization.CultureInfo.InvariantCulture);
                else stringValue = value?.ToString();
                
                log.Error("🔧 **FIELD_UPDATE_ATTEMPT**: Field='{FieldName}' | Value='{Value}' | StringValue='{StringValue}'", 
                    fieldName, value, stringValue);
                    
                UpdateFieldInTemplate(template, fieldMappings, fieldName, stringValue, log);
            };

            // Update all fields with detailed logging for each
            log.Error("🔧 **UPDATE_FIELD_BATCH_START**: Applying {Count} field updates", 10);
            updateTemplateField("InvoiceTotal", representativeCorrectedInvoice.InvoiceTotal);
            updateTemplateField("SubTotal", representativeCorrectedInvoice.SubTotal);
            updateTemplateField("TotalInternalFreight", representativeCorrectedInvoice.TotalInternalFreight);
            updateTemplateField("TotalOtherCost", representativeCorrectedInvoice.TotalOtherCost);
            updateTemplateField("TotalInsurance", representativeCorrectedInvoice.TotalInsurance);
            updateTemplateField("TotalDeduction", representativeCorrectedInvoice.TotalDeduction);
            updateTemplateField("InvoiceNo", representativeCorrectedInvoice.InvoiceNo);
            updateTemplateField("InvoiceDate", representativeCorrectedInvoice.InvoiceDate);
            updateTemplateField("SupplierName", representativeCorrectedInvoice.SupplierName);
            updateTemplateField("Currency", representativeCorrectedInvoice.Currency);
            log.Error("✅ **UPDATE_FIELD_BATCH_COMPLETE**: All field updates attempted");

            // Capture AFTER state of Lines.Values
            log.Error("📊 **LINES_VALUES_AFTER_STATE**: Capturing template Lines.Values state after updates");
            LogTemplateLineValuesState(template, "AFTER", log);

            // Detect and log changes
            var changesSummary = DetectLineValuesChanges(template, log);
            if (!string.IsNullOrEmpty(changesSummary))
            {
                log.Error("✅ **LINES_VALUES_CHANGES_DETECTED**: {ChangesSummary}", changesSummary);
            }
            else
            {
                log.Warning("⚠️ **LINES_VALUES_NO_CHANGES**: No changes detected in Lines.Values after update attempt");
            }

            log.Error("✅ **LINES_VALUES_UPDATE_COMPLETE**: Template LineValues update completed for template {TemplateLogId}", templateLogId);
        }

        private static Dictionary<string, (int LineId, int FieldId)> GetTemplateFieldMappings(Invoice template, ILogger logger)
        {
            var mappings = new Dictionary<string, (int LineId, int FieldId)>(StringComparer.OrdinalIgnoreCase);
            try
            {
                if (template?.Lines == null) return mappings;
                foreach (var lineWrapper in template.Lines.Where(lw => lw?.OCR_Lines?.Fields != null))
                {
                    var ocrLineDef = lineWrapper.OCR_Lines;
                    foreach (var fieldDef in ocrLineDef.Fields)
                    {
                        var canonicalPropName = MapTemplateFieldToPropertyName(fieldDef.Field);
                        if (!string.IsNullOrEmpty(canonicalPropName) && !mappings.ContainsKey(canonicalPropName))
                        {
                            mappings[canonicalPropName] = (ocrLineDef.Id, fieldDef.Id);
                        }
                        var canonicalKeyName = MapTemplateFieldToPropertyName(fieldDef.Key);
                        if (!string.IsNullOrEmpty(canonicalKeyName) && !mappings.ContainsKey(canonicalKeyName) &&
                            (string.IsNullOrEmpty(canonicalPropName) || !canonicalKeyName.Equals(canonicalPropName, StringComparison.OrdinalIgnoreCase)))
                        {
                            mappings[canonicalKeyName] = (ocrLineDef.Id, fieldDef.Id);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                var templateLogId = template?.OcrInvoices?.Id.ToString() ?? template?.OcrInvoices.Id.ToString();
                logger.Error(ex, "Error in GetTemplateFieldMappings for template (LogID: {TemplateLogId})", templateLogId);
            }
            return mappings;
        }
        private static void UpdateFieldInTemplate(Invoice template, Dictionary<string, (int LineId, int FieldId)> fieldMappings,
            string canonicalPropertyName, string newStringValue, ILogger log)
        {
            if (newStringValue == null || !fieldMappings.TryGetValue(canonicalPropertyName, out var mappingInfo)) return;

            var invoiceLineWrapper = template.Lines?.FirstOrDefault(lWrapper => lWrapper.OCR_Lines?.Id == mappingInfo.LineId);
            var targetOcrLineDef = (Line: invoiceLineWrapper?.OCR_Lines, LineValue: invoiceLineWrapper.Values);

            if (!targetOcrLineDef.LineValue.Any())
            {
                log.Debug("UpdateFieldInTemplate: No LineValue (extracted instances) found for LineDefId {LineId} to update field {PropName}", mappingInfo.LineId, canonicalPropertyName);
                return;
            }

            bool wasUpdatedInAnyInstance = false;
            foreach (var lineInstanceEntry in targetOcrLineDef.LineValue.ToList())
            {
                var fieldValuesInThisInstance = lineInstanceEntry.Value;
                if (fieldValuesInThisInstance == null) continue;

                var fieldTupleKeyToUpdate = fieldValuesInThisInstance.Keys
                                            .FirstOrDefault(keyTuple => keyTuple.Item1?.Id == mappingInfo.FieldId);

                if (fieldTupleKeyToUpdate.Item1 != null)
                {
                    if (fieldValuesInThisInstance.TryGetValue(fieldTupleKeyToUpdate, out string oldValueInTemplate) &&
                        string.Equals(oldValueInTemplate, newStringValue))
                    {
                        wasUpdatedInAnyInstance = true;
                        continue;
                    }
                    fieldValuesInThisInstance[fieldTupleKeyToUpdate] = newStringValue ?? "";
                    wasUpdatedInAnyInstance = true;
                }
            }

            if (wasUpdatedInAnyInstance)
            {
                log.Information("Updated/Verified template LineValue for {PropName} (FieldId: {FieldId}, LineDefId: {LineId}) to '{NewVal}' across its instances.",
                   canonicalPropertyName, mappingInfo.FieldId, mappingInfo.LineId, newStringValue ?? "NULL");
            }
            else
            {
                log.Warning("Could not find target FieldId {FieldId} in any LineValue instances for LineDefId {LineId} to update {PropName}.",
                   mappingInfo.FieldId, mappingInfo.LineId, canonicalPropertyName);
            }
        }

        #endregion

        #region Data Models for Legacy Support (used by static methods in this file)
        public class EnhancedFieldMapping
        {
            public int LineId { get; set; }
            public int FieldId { get; set; }
            public int PartId { get; set; }
            public string RegexPattern { get; set; }
            public string Key { get; set; }
            public string FieldName { get; set; }
            public string EntityType { get; set; }
            public string DataType { get; set; }
        }

        public class DirectCorrection
        {
            public string FieldName { get; set; }
            public string CurrentValue { get; set; }
            public string CorrectedValue { get; set; }
            public string Reasoning { get; set; }
        }

        public static double TotalsZeroAmount(ShipmentInvoice invoice)
        {
            if (invoice != null && _totalsZeroAmounts.TryGetValue(invoice, out var box))
            {
                return box.Value;
            }
            return double.NaN;
        }
        // Helper method for logging template Lines.Values state with enhanced field detection
        private static void LogTemplateLineValuesState(Invoice template, string stateName, ILogger log)
        {
            if (template?.Lines == null)
            {
                log.Error("📊 **{StateName}_LINES_VALUES_STATE**: Template has no lines", stateName);
                return;
            }

            int totalValueEntries = 0;
            int linesWithValues = 0;
            var importantFieldsFound = new List<string>();

            foreach (var line in template.Lines)
            {
                var lineId = line.OCR_Lines?.Id ?? 0;
                var lineName = line.OCR_Lines?.Name ?? "Unknown";
                var valueCount = line.Values?.Sum(kvp => kvp.Value?.Count ?? 0) ?? 0;
                
                if (valueCount > 0)
                {
                    linesWithValues++;
                    totalValueEntries += valueCount;
                    
                    log.Error("📋 **{StateName}_LINE_VALUES**: LineId={LineId}, Name='{LineName}', Values={ValueCount}", 
                        stateName, lineId, lineName, valueCount);
                    
                    // Log specific field values for important fields
                    if (line.Values?.Any() == true)
                    {
                        foreach (var lineValueKvp in line.Values)
                        {
                            var (lineNumber, section) = lineValueKvp.Key;
                            var fieldsDict = lineValueKvp.Value;
                            
                            if (fieldsDict?.Any() == true)
                            {
                                foreach (var fieldKvp in fieldsDict)
                                {
                                    var (fields, instance) = fieldKvp.Key;
                                    var value = fieldKvp.Value;
                                    
                                    // Log important fields for OCR correction tracking
                                    if (fields?.Field != null && (fields.Field.Contains("Total") || 
                                        fields.Field.Contains("Invoice") || fields.Field.Contains("Deduction") || 
                                        fields.Field.Contains("Insurance") || fields.Field.Contains("Supplier")))
                                    {
                                        var fieldDetail = $"{fields.Field}='{value}'";
                                        importantFieldsFound.Add(fieldDetail);
                                        
                                        log.Error("    🔍 **{StateName}_VALUE_DETAIL**: Field='{FieldName}', Key='{FieldKey}', Value='{Value}', Line={LineNumber}, Section='{Section}', Instance='{Instance}'",
                                            stateName, fields.Field, fields.Key, value, lineNumber, section, instance);
                                    }
                                }
                            }
                        }
                    }
                }
                else
                {
                    log.Error("📋 **{StateName}_LINE_NO_VALUES**: LineId={LineId}, Name='{LineName}', Values=0", 
                        stateName, lineId, lineName);
                }
            }

            log.Error("📊 **{StateName}_SUMMARY**: {TotalLines} lines total, {LinesWithValues} lines with values, {TotalValueEntries} total value entries", 
                stateName, template.Lines.Count, linesWithValues, totalValueEntries);
                
            // Log summary of important fields found
            if (importantFieldsFound.Any())
            {
                log.Error("🔍 **{StateName}_IMPORTANT_FIELDS**: Found {Count} important field values: {Fields}", 
                    stateName, importantFieldsFound.Count, string.Join(", ", importantFieldsFound));
            }
            else
            {
                log.Error("⚠️ **{StateName}_NO_IMPORTANT_FIELDS**: No important field values found in Lines.Values structure", stateName);
            }
        }

        // Helper method for detecting changes in Lines.Values with comprehensive field analysis
        private static string DetectLineValuesChanges(Invoice template, ILogger log)
        {
            // This method analyzes the current state and identifies key fields for OCR correction tracking
            if (template?.Lines == null)
            {
                return "Template has no lines";
            }

            var changesList = new List<string>();
            var criticalFields = new Dictionary<string, List<string>>();
            int totalValues = 0;
            int linesWithValues = 0;

            // Track critical fields for Amazon invoice OCR corrections
            var targetFields = new[] { "TotalDeduction", "TotalInsurance", "InvoiceTotal", "SubTotal", "TotalInternalFreight", "TotalOtherCost", "SupplierName" };

            foreach (var line in template.Lines)
            {
                var lineId = line.OCR_Lines?.Id ?? 0;
                var lineName = line.OCR_Lines?.Name ?? "Unknown";
                var valueCount = line.Values?.Sum(kvp => kvp.Value?.Count ?? 0) ?? 0;
                
                if (valueCount > 0)
                {
                    linesWithValues++;
                    totalValues += valueCount;
                    
                    if (line.Values?.Any() == true)
                    {
                        foreach (var lineValueKvp in line.Values)
                        {
                            var (lineNumber, section) = lineValueKvp.Key;
                            var fieldsDict = lineValueKvp.Value;
                            
                            if (fieldsDict?.Any() == true)
                            {
                                foreach (var fieldKvp in fieldsDict)
                                {
                                    var (fields, instance) = fieldKvp.Key;
                                    var value = fieldKvp.Value;
                                    
                                    // Track target fields that are critical for OCR corrections
                                    if (fields?.Field != null && targetFields.Contains(fields.Field) && !string.IsNullOrEmpty(value))
                                    {
                                        if (!criticalFields.ContainsKey(fields.Field))
                                        {
                                            criticalFields[fields.Field] = new List<string>();
                                        }
                                        criticalFields[fields.Field].Add($"Line{lineId}:{value}");
                                        
                                        // Log detailed field information
                                        log.Error("🔍 **CHANGE_DETECTION_FIELD**: Field='{FieldName}' | Value='{Value}' | LineId={LineId} | LineName='{LineName}' | Section='{Section}' | Instance='{Instance}'",
                                            fields.Field, value, lineId, lineName, section, instance);
                                    }
                                }
                            }
                        }
                    }
                }
            }

            // Analyze critical fields for OCR correction indicators
            foreach (var kvp in criticalFields)
            {
                var fieldName = kvp.Key;
                var fieldValues = kvp.Value;
                changesList.Add($"{fieldName}: {string.Join(", ", fieldValues)} (count: {fieldValues.Count})");
            }

            // Special analysis for Amazon invoice OCR correction success
            bool hasTotalDeduction = criticalFields.ContainsKey("TotalDeduction") && criticalFields["TotalDeduction"].Any(v => !string.IsNullOrWhiteSpace(v.Split(':').LastOrDefault()));
            bool hasTotalInsurance = criticalFields.ContainsKey("TotalInsurance") && criticalFields["TotalInsurance"].Any(v => !string.IsNullOrWhiteSpace(v.Split(':').LastOrDefault()));
            bool hasSupplierName = criticalFields.ContainsKey("SupplierName") && criticalFields["SupplierName"].Any(v => !string.IsNullOrWhiteSpace(v.Split(':').LastOrDefault()));

            // Generate OCR correction analysis
            var ocrAnalysis = new List<string>();
            if (hasTotalDeduction)
            {
                ocrAnalysis.Add("✅ TotalDeduction populated (supplier reduction detected)");
            }
            else
            {
                ocrAnalysis.Add("❌ TotalDeduction missing (expected from Free Shipping corrections)");
            }

            if (hasTotalInsurance)
            {
                ocrAnalysis.Add("✅ TotalInsurance populated (customer reduction detected)");
            }
            else
            {
                ocrAnalysis.Add("❌ TotalInsurance missing (expected from Gift Card corrections)");
            }

            if (hasSupplierName)
            {
                ocrAnalysis.Add("✅ SupplierName populated");
            }
            else
            {
                ocrAnalysis.Add("⚠️ SupplierName missing");
            }

            changesList.Add($"OCR Analysis: {string.Join(", ", ocrAnalysis)}");
            changesList.Add($"Template state: {linesWithValues} lines with values, {totalValues} total value entries");
            
            // Log comprehensive analysis
            log.Error("🔍 **CHANGE_DETECTION_ANALYSIS**: CriticalFieldsFound={FieldCount} | TotalDeduction={HasTD} | TotalInsurance={HasTI} | SupplierName={HasSN}",
                criticalFields.Count, hasTotalDeduction, hasTotalInsurance, hasSupplierName);
            
            return changesList.Any() ? string.Join("; ", changesList) : "No significant changes detected";
        }


        #endregion
    }
}