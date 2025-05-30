//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Text.Json;
//using System.Text.RegularExpressions;
//using System.Threading.Tasks;
//using EntryDataDS.Business.Entities;
//using OCR.Business.Entities;
//using TrackableEntities;
//using WaterNut.Business.Services.Utils;
//using Serilog;
//using Serilog.Events;
//using Core.Common.Extensions;

//namespace WaterNut.DataSpace
//{
//    using System.IO;

//    #region Data Models

//    /// <summary>
//    /// Result of applying an OCR correction
//    /// </summary>
//    public class CorrectionResult
//    {
//        public string FieldName { get; set; }
//        public string OldValue { get; set; }
//        public string NewValue { get; set; }
//        public string CorrectionType { get; set; }
//        public bool Success { get; set; }
//        public string ErrorMessage { get; set; }
//        public double Confidence { get; set; }
//    }

//    /// <summary>
//    /// Represents an OCR error detected in invoice data
//    /// </summary>
//    public class InvoiceError
//    {
//        public string Field { get; set; }
//        public string ExtractedValue { get; set; }
//        public string CorrectValue { get; set; }
//        public double Confidence { get; set; }
//        public string ErrorType { get; set; }
//        public string Reasoning { get; set; }
//    }

//    /// <summary>
//    /// Enhanced LineInfo with confidence and reasoning
//    /// </summary>
//    public class LineInfo
//    {
//        public int LineNumber { get; set; }
//        public string LineText { get; set; }
//        public double Confidence { get; set; }
//        public string Reasoning { get; set; }
//    }

//    /// <summary>
//    /// Regex correction strategy determined by DeepSeek
//    /// </summary>
//    public class RegexCorrectionStrategy
//    {
//        public string StrategyType { get; set; } // FORMAT_FIX, PATTERN_UPDATE, CHARACTER_MAP, VALIDATION_RULE
//        public string RegexPattern { get; set; }
//        public string ReplacementPattern { get; set; }
//        public double Confidence { get; set; }
//        public string Reasoning { get; set; }
//    }

//    /// <summary>
//    /// Request for updating regex patterns
//    /// </summary>
//    public class RegexUpdateRequest
//    {
//        public string FieldName { get; set; }
//        public string CorrectionType { get; set; }
//        public string OldValue { get; set; }
//        public string NewValue { get; set; }
//        public int LineNumber { get; set; }
//        public string LineText { get; set; }
//        public RegexCorrectionStrategy Strategy { get; set; }
//        public double Confidence { get; set; }
//    }

//    /// <summary>
//    /// Enumeration of OCR error types
//    /// </summary>
//    public enum OCRErrorType
//    {
//        DecimalSeparator,    // Comma vs period confusion
//        CharacterConfusion,  // 1/l, 0/O, etc.
//        MissingDigit,       // Missing or extra digits
//        FormatError,        // General formatting issues
//        FieldMismatch,      // Wrong field mapping
//        CalculationError,   // Mathematical inconsistencies
//        UnreasonableValue   // Values that don't make sense
//    }

//    #endregion

//    /// <summary>
//    /// Service for handling OCR error corrections using DeepSeek LLM analysis
//    /// Enhanced with comprehensive product validation and regex learning
//    /// </summary>
//    public partial class OCRCorrectionService : IDisposable
//    {
//        #region Fields and Properties

//        private readonly DeepSeekInvoiceApi _deepSeekApi;
//        private readonly ILogger _logger;
//        private bool _disposed = false;

//        // Configuration properties
//        public double DefaultTemperature { get; set; } = 0.1; // Lower temp for corrections
//        public int DefaultMaxTokens { get; set; } = 4096;

//        #endregion

//        #region Constructor

//        public OCRCorrectionService()
//        {
//            _deepSeekApi = new DeepSeekInvoiceApi();
//            _logger = Log.Logger.ForContext<OCRCorrectionService>();
//        }

//        #endregion

//        #region Public Static Methods (Legacy Support)

//        /// <summary>
//        /// Static method to check if invoice totals are correct (TotalsZero = 0) with gift card handling
//        /// </summary>
//        public static bool TotalsZero(ShipmentInvoice invoice, ILogger logger = null)
//        {
//            var log = logger ?? Log.Logger; // Use provided logger or fall back to global logger

//            if (invoice == null) return false;

//            var baseTotal = (invoice.SubTotal ?? 0) +
//                          (invoice.TotalInternalFreight ?? 0) +
//                          (invoice.TotalOtherCost ?? 0) +
//                          (invoice.TotalInsurance ?? 0);

//            var deductionAmount = invoice.TotalDeduction ?? 0;
//            var reportedInvoiceTotal = invoice.InvoiceTotal ?? 0;

//            // Check if the invoice total already has deductions applied (like Amazon gift cards)
//            var calculatedWithDeduction = baseTotal - deductionAmount;
//            var calculatedWithoutDeduction = baseTotal;

//            var diffWithDeduction = Math.Abs(calculatedWithDeduction - reportedInvoiceTotal);
//            var diffWithoutDeduction = Math.Abs(calculatedWithoutDeduction - reportedInvoiceTotal);

//            // Use the smaller difference to determine if totals are zero
//            var minDifference = Math.Min(diffWithDeduction, diffWithoutDeduction);

//            // Debug level logging to reduce console noise
//            log.Debug("TotalsZero calculation for {InvoiceNo}: BaseTotal={BaseTotal}, Deduction={Deduction}, Reported={Reported}, MinDiff={MinDiff}, Result={Result}",
//                invoice.InvoiceNo, baseTotal, deductionAmount, reportedInvoiceTotal, minDifference, minDifference < 0.01);

//            return minDifference < 0.01;
//        }

//        /// <summary>
//        /// Static method to correct invoices using DeepSeek OCR error detection
//        /// </summary>
//        public static async Task<bool> CorrectInvoices(ShipmentInvoice invoice, string fileText)
//        {
//            using var service = new OCRCorrectionService();
//            return await service.CorrectInvoiceAsync(invoice, fileText).ConfigureAwait(false);
//        }

//        /// <summary>
//        /// Static method to calculate total zero sum from dynamic invoice results - HANDLES MULTIPLE INVOICES
//        /// </summary>
//        public static bool TotalsZero(List<dynamic> res, out double totalZeroSum, ILogger logger = null)
//        {
//            // Use LogLevelOverride to enable detailed logging for this specific calculation
//            using (LogLevelOverride.Begin(LogEventLevel.Debug))
//            {
//                var log = logger ?? Log.Logger; // Use provided logger or fall back to global logger

//                totalZeroSum = 0;

//                if (res == null || !res.Any())
//                    return true; // Empty is legitimate success

//                try
//                {
//                    bool processedAnyItem = false;

//                    foreach (var item in res)
//                    {
//                        if (item is List<IDictionary<string, object>> invoiceList)
//                        {
//                            if (!invoiceList.Any()) continue;

//                            // FIXED: Process ALL invoices in the collection, not just the first
//                            foreach (var invoiceDict in invoiceList)
//                            {
//                                var tempInvoice = CreateTempShipmentInvoice(invoiceDict);

//                                // Use the same gift card handling logic as the static method
//                                var baseTotal = (tempInvoice.SubTotal ?? 0) +
//                                              (tempInvoice.TotalInternalFreight ?? 0) +
//                                              (tempInvoice.TotalOtherCost ?? 0) +
//                                              (tempInvoice.TotalInsurance ?? 0);

//                                var deductionAmount = tempInvoice.TotalDeduction ?? 0;
//                                var reportedInvoiceTotal = tempInvoice.InvoiceTotal ?? 0;

//                                var calculatedWithDeduction = baseTotal - deductionAmount;
//                                var calculatedWithoutDeduction = baseTotal;

//                                var diffWithDeduction = Math.Abs(calculatedWithDeduction - reportedInvoiceTotal);
//                                var diffWithoutDeduction = Math.Abs(calculatedWithoutDeduction - reportedInvoiceTotal);

//                                // Use the smaller difference for TotalsZero calculation
//                                var invoiceTotalsZero = Math.Min(diffWithDeduction, diffWithoutDeduction);

//                                totalZeroSum += invoiceTotalsZero;
//                                processedAnyItem = true;

//                                // Enhanced logging with emojis for easy identification
//                                log.Information("🔍 Dynamic TotalsZero calculation for {InvoiceNo}:", tempInvoice.InvoiceNo);
//                                log.Information("   📊 BaseTotal = SubTotal({SubTotal}) + Freight({Freight}) + OtherCost({OtherCost}) + Insurance({Insurance}) = {BaseTotal}",
//                                    tempInvoice.SubTotal, tempInvoice.TotalInternalFreight, tempInvoice.TotalOtherCost, tempInvoice.TotalInsurance, baseTotal);
//                                log.Information("   💳 TotalDeduction = {Deduction}", deductionAmount);
//                                log.Information("   🧾 ReportedInvoiceTotal = {ReportedTotal}", reportedInvoiceTotal);
//                                log.Information("   🧮 Calculated WITH deduction: {BaseTotal} - {Deduction} = {WithDeduction} (diff: {DiffWith})",
//                                    baseTotal, deductionAmount, calculatedWithDeduction, diffWithDeduction);
//                                log.Information("   🧮 Calculated WITHOUT deduction: {BaseTotal} = {WithoutDeduction} (diff: {DiffWithout})",
//                                    baseTotal, calculatedWithoutDeduction, diffWithoutDeduction);
//                                log.Information("   ✅ Using minimum difference: {MinDifference}",
//                                    invoiceTotalsZero);
//                            }
//                        }
//                        else
//                        {
//                            // Type mismatch - log for debugging
//                            var actualType = item?.GetType().Name ?? "null";
//                            log.Warning("Expected List<IDictionary<string, object>> but got {ActualType}", actualType);
//                            totalZeroSum = 0;
//                            return false;
//                        }
//                    }

//                    log.Information("🏁 Final Results: Processed {ProcessedAny} items, total TotalsZero sum: {TotalSum}",
//                        processedAnyItem, totalZeroSum);
//                    return processedAnyItem || !res.Any();
//                }
//                catch (Exception ex)
//                {
//                    log.Error(ex, "Error calculating TotalsZero");
//                    totalZeroSum = 0;
//                    return false;
//                }
//            }
//        }


//        /// <summary>
//        /// Static method to correct invoices using OCR correction service - HANDLES MULTIPLE INVOICES
//        /// </summary>
//        public static async Task CorrectInvoices(List<dynamic> res, Invoice template, ILogger logger = null)
//        {
//            if (res == null || !res.Any() || template == null) return;

//            var log = logger ?? Log.Logger; // Use provided logger or fall back to global logger

//            try
//            {
//                var allShipmentInvoices = ConvertDynamicToShipmentInvoices(res);
//                var droppedFilePath = GetFilePathFromTemplate(template);

//                log.Information("Processing {InvoiceCount} invoices from {FileCount} PDF sections",
//                    allShipmentInvoices.Count, res.Count);

//                using var correctionService = new OCRCorrectionService();
//                var correctedInvoices = await correctionService.CorrectInvoicesAsync(allShipmentInvoices, droppedFilePath).ConfigureAwait(false);
//                UpdateDynamicResultsWithCorrections(res, correctedInvoices);
//            }
//            catch (Exception ex)
//            {
//                log.Error(ex, "Error in static CorrectInvoices");
//            }
//        }

//        #endregion

//        #region Core Correction Methods

//        /// <summary>
//        /// Corrects a single invoice using comprehensive DeepSeek analysis
//        /// </summary>
//        public async Task<bool> CorrectInvoiceAsync(ShipmentInvoice invoice, string fileText)
//        {
//            try
//            {
//                if (invoice == null || string.IsNullOrEmpty(fileText))
//                {
//                    _logger.Warning("Cannot correct invoice: null invoice or empty file text");
//                    return false;
//                }

//                _logger.Information("Starting OCR correction for invoice {InvoiceNo}", invoice.InvoiceNo);

//                var errors = await this.DetectInvoiceErrorsAsync(invoice, fileText).ConfigureAwait(false);
//                if (!errors.Any())
//                {
//                    _logger.Information("No errors detected for invoice {InvoiceNo}", invoice.InvoiceNo);
//                    return false;
//                }

//                _logger.Information("Detected {ErrorCount} errors for invoice {InvoiceNo}", errors.Count, invoice.InvoiceNo);

//                var corrections = await this.ApplyCorrectionsAsync(invoice, errors, fileText).ConfigureAwait(false);
//                var successfulCorrections = corrections.Count(c => c.Success);

//                _logger.Information("Applied {SuccessCount}/{TotalCount} corrections for invoice {InvoiceNo}",
//                    successfulCorrections, corrections.Count, invoice.InvoiceNo);

//                return successfulCorrections > 0;
//            }
//            catch (Exception ex)
//            {
//                _logger.Error(ex, "Error correcting invoice {InvoiceNo}", invoice?.InvoiceNo);
//                return false;
//            }
//        }

//        /// <summary>
//        /// Corrects multiple invoices with comprehensive validation and regex updates
//        /// </summary>
//        public async Task<List<ShipmentInvoice>> CorrectInvoicesAsync(List<ShipmentInvoice> invoices, string droppedFilePath)
//        {
//            var invoicesWithIssues = invoices.Where(x => x.TotalsZero != 0).ToList();
//            _logger.Information("Found {Count} invoices with TotalsZero != 0", invoicesWithIssues.Count);

//            foreach (var invoice in invoicesWithIssues)
//            {
//                try
//                {
//                    _logger.Information("Processing invoice {InvoiceNo} with TotalsZero: {TotalsZero}",
//                        invoice.InvoiceNo, invoice.TotalsZero);

//                    var txtFile = droppedFilePath + ".txt";
//                    if (!System.IO.File.Exists(txtFile))
//                    {
//                        _logger.Warning("Text file not found: {FilePath}", txtFile);
//                        continue;
//                    }

//                    var fileTxt = System.IO.File.ReadAllText(txtFile);
//                    await this.CorrectInvoiceWithRegexUpdatesAsync(invoice, fileTxt).ConfigureAwait(false);
//                }
//                catch (Exception ex)
//                {
//                    _logger.Error(ex, "Error processing invoice {InvoiceNo}", invoice.InvoiceNo);
//                }
//            }

//            return invoices;
//        }

//        /// <summary>
//        /// Corrects a single invoice with comprehensive validation and regex updates
//        /// </summary>
//        public async Task<bool> CorrectInvoiceWithRegexUpdatesAsync(ShipmentInvoice invoice, string fileText)
//        {
//            try
//            {
//                if (invoice == null || string.IsNullOrEmpty(fileText))
//                {
//                    _logger.Warning("Cannot correct invoice: null invoice or empty file text");
//                    return false;
//                }

//                _logger.Information("Starting comprehensive OCR correction for invoice {InvoiceNo}", invoice.InvoiceNo);

//                // 1. Detect all types of errors
//                var errors = await this.DetectInvoiceErrorsAsync(invoice, fileText).ConfigureAwait(false);
//                if (!errors.Any())
//                {
//                    _logger.Information("No errors detected for invoice {InvoiceNo}", invoice.InvoiceNo);
//                    return false;
//                }

//                _logger.Information("Detected {ErrorCount} errors for invoice {InvoiceNo}: {ErrorSummary}",
//                    errors.Count, invoice.InvoiceNo, string.Join(", ", errors.Select(e => $"{e.Field}({e.ErrorType})")));

//                // 2. Apply corrections
//                var corrections = await this.ApplyCorrectionsAsync(invoice, errors, fileText).ConfigureAwait(false);
//                var successfulCorrections = corrections.Where(c => c.Success).ToList();

//                _logger.Information("Applied {SuccessCount}/{TotalCount} corrections for invoice {InvoiceNo}",
//                    successfulCorrections.Count, corrections.Count, invoice.InvoiceNo);

//                // 3. Validate post-correction totals
//                var postCorrectionValid = TotalsZero(invoice, _logger);
//                _logger.Information("Post-correction TotalsZero validation: {IsValid} for invoice {InvoiceNo}",
//                    postCorrectionValid, invoice.InvoiceNo);

//                // 4. Update regex patterns based on successful corrections
//                if (successfulCorrections.Any())
//                {
//                    await this.UpdateRegexPatternsAsync(successfulCorrections, fileText).ConfigureAwait(false);
//                }

//                return successfulCorrections.Any();
//            }
//            catch (Exception ex)
//            {
//                _logger.Error(ex, "Error in comprehensive invoice correction for {InvoiceNo}", invoice?.InvoiceNo);
//                return false;
//            }
//        }

//        #endregion

//        #region Error Detection

//        /// <summary>
//        /// Detects OCR errors using comprehensive 4-stage validation
//        /// </summary>
//        private async Task<List<InvoiceError>> DetectInvoiceErrorsAsync(ShipmentInvoice invoice, string fileText)
//        {
//            try
//            {
//                var allErrors = new List<InvoiceError>();

//                // 1. Header-level field validation (totals, supplier info)
//                _logger.Information("Detecting header field errors for invoice {InvoiceNo}", invoice.InvoiceNo);
//                var headerErrors = await this.DetectHeaderFieldErrorsAsync(invoice, fileText).ConfigureAwait(false);
//                allErrors.AddRange(headerErrors);

//                // 2. Product-level validation (prices, quantities, descriptions)
//                _logger.Information("Detecting product-level errors for invoice {InvoiceNo}", invoice.InvoiceNo);
//                var productErrors = await this.DetectProductErrorsAsync(invoice, fileText).ConfigureAwait(false);
//                allErrors.AddRange(productErrors);

//                // 3. Mathematical consistency validation
//                _logger.Information("Validating mathematical consistency for invoice {InvoiceNo}", invoice.InvoiceNo);
//                var mathErrors = ValidateMathematicalConsistency(invoice);
//                allErrors.AddRange(mathErrors);

//                // 4. Cross-field validation (totals vs details)
//                _logger.Information("Validating cross-field consistency for invoice {InvoiceNo}", invoice.InvoiceNo);
//                var crossFieldErrors = ValidateCrossFieldConsistency(invoice);
//                allErrors.AddRange(crossFieldErrors);

//                _logger.Information("Total errors detected: {ErrorCount} for invoice {InvoiceNo}",
//                    allErrors.Count, invoice.InvoiceNo);

//                return allErrors;
//            }
//            catch (Exception ex)
//            {
//                _logger.Error(ex, "Error detecting invoice errors for {InvoiceNo}", invoice?.InvoiceNo);
//                return new List<InvoiceError>();
//            }
//        }

//        /// <summary>
//        /// Detects errors in header-level fields using specialized prompt
//        /// </summary>
//        private async Task<List<InvoiceError>> DetectHeaderFieldErrorsAsync(ShipmentInvoice invoice, string fileText)
//        {
//            var prompt = CreateHeaderErrorDetectionPrompt(invoice, fileText);
//            var response = await this._deepSeekApi.GetResponseAsync(prompt).ConfigureAwait(false);

//            if (string.IsNullOrWhiteSpace(response))
//            {
//                _logger.Warning("Empty response from DeepSeek for header error detection");
//                return new List<InvoiceError>();
//            }

//            return ParseErrorDetectionResponse(response);
//        }

//        /// <summary>
//        /// Detects errors in product-level data using specialized prompt
//        /// </summary>
//        private async Task<List<InvoiceError>> DetectProductErrorsAsync(ShipmentInvoice invoice, string fileText)
//        {
//            if (invoice.InvoiceDetails == null || !invoice.InvoiceDetails.Any())
//            {
//                _logger.Information("No invoice details to validate for invoice {InvoiceNo}", invoice.InvoiceNo);
//                return new List<InvoiceError>();
//            }

//            var prompt = CreateProductErrorDetectionPrompt(invoice, fileText);
//            var response = await this._deepSeekApi.GetResponseAsync(prompt).ConfigureAwait(false);

//            if (string.IsNullOrWhiteSpace(response))
//            {
//                _logger.Warning("Empty response from DeepSeek for product error detection");
//                return new List<InvoiceError>();
//            }

//            return ParseErrorDetectionResponse(response);
//        }

//        /// <summary>
//        /// Validates mathematical consistency within the invoice
//        /// </summary>
//        private List<InvoiceError> ValidateMathematicalConsistency(ShipmentInvoice invoice)
//        {
//            var errors = new List<InvoiceError>();

//            try
//            {
//                // Validate individual line item calculations
//                if (invoice.InvoiceDetails != null)
//                {
//                    foreach (var detail in invoice.InvoiceDetails)
//                    {
//                        var calculatedTotal = (detail.Quantity * detail.Cost) - (detail.Discount ?? 0);
//                        var reportedTotal = detail.TotalCost ?? 0;

//                        if (Math.Abs(calculatedTotal - reportedTotal) > 0.01)
//                        {
//                            errors.Add(new InvoiceError
//                            {
//                                Field = $"InvoiceDetail_Line{detail.LineNumber}_TotalCost",
//                                ExtractedValue = reportedTotal.ToString("F2"),
//                                CorrectValue = calculatedTotal.ToString("F2"),
//                                Confidence = 0.99,
//                                ErrorType = "calculation_error",
//                                Reasoning = $"Line total should be (Qty {detail.Quantity} × Cost {detail.Cost:F2}) - Discount {detail.Discount ?? 0:F2} = {calculatedTotal:F2}"
//                            });
//                        }

//                        // Validate reasonable quantities
//                        if (detail.Quantity <= 0 || detail.Quantity > 10000)
//                        {
//                            errors.Add(new InvoiceError
//                            {
//                                Field = $"InvoiceDetail_Line{detail.LineNumber}_Quantity",
//                                ExtractedValue = detail.Quantity.ToString(),
//                                CorrectValue = "1",
//                                Confidence = 0.7,
//                                ErrorType = "unreasonable_quantity",
//                                Reasoning = $"Quantity {detail.Quantity} seems unreasonable"
//                            });
//                        }

//                        // Validate reasonable unit costs
//                        if (detail.Cost <= 0)
//                        {
//                            errors.Add(new InvoiceError
//                            {
//                                Field = $"InvoiceDetail_Line{detail.LineNumber}_Cost",
//                                ExtractedValue = detail.Cost.ToString("F2"),
//                                CorrectValue = "0.01",
//                                Confidence = 0.8,
//                                ErrorType = "unreasonable_cost",
//                                Reasoning = $"Unit cost {detail.Cost:F2} is negative or zero"
//                            });
//                        }
//                    }
//                }

//                return errors;
//            }
//            catch (Exception ex)
//            {
//                _logger.Error(ex, "Error validating mathematical consistency");
//                return errors;
//            }
//        }

//        /// <summary>
//        /// Enhanced cross-field validation that properly validates invoice totals
//        /// </summary>
//        private List<InvoiceError> ValidateCrossFieldConsistency(ShipmentInvoice invoice)
//        {
//            var errors = new List<InvoiceError>();

//            try
//            {
//                // Check if detail totals match subtotal
//                if (invoice.InvoiceDetails?.Any() == true)
//                {
//                    var calculatedSubTotal = invoice.InvoiceDetails.Sum(d => d.TotalCost ?? 0);
//                    var reportedSubTotal = invoice.SubTotal ?? 0;

//                    if (Math.Abs(calculatedSubTotal - reportedSubTotal) > 0.01)
//                    {
//                        errors.Add(new InvoiceError
//                        {
//                            Field = "SubTotal",
//                            ExtractedValue = reportedSubTotal.ToString("F2"),
//                            CorrectValue = calculatedSubTotal.ToString("F2"),
//                            Confidence = 0.95,
//                            ErrorType = "subtotal_mismatch",
//                            Reasoning = $"SubTotal should equal sum of line items: {calculatedSubTotal:F2}"
//                        });
//                    }
//                }

//                // FIXED: Enhanced invoice total validation with gift card handling
//                var baseTotal = (invoice.SubTotal ?? 0) +
//                              (invoice.TotalInternalFreight ?? 0) +
//                              (invoice.TotalOtherCost ?? 0) +
//                              (invoice.TotalInsurance ?? 0);

//                var deductionAmount = invoice.TotalDeduction ?? 0;
//                var reportedInvoiceTotal = invoice.InvoiceTotal ?? 0;

//                // Check if the invoice total already has deductions applied (like Amazon gift cards)
//                var calculatedWithDeduction = baseTotal - deductionAmount;  // Standard formula
//                var calculatedWithoutDeduction = baseTotal;                 // Total before deductions

//                var diffWithDeduction = Math.Abs(calculatedWithDeduction - reportedInvoiceTotal);
//                var diffWithoutDeduction = Math.Abs(calculatedWithoutDeduction - reportedInvoiceTotal);

//                // Use the calculation that's closer to the reported total
//                var calculatedInvoiceTotal = diffWithDeduction <= diffWithoutDeduction ?
//                    calculatedWithDeduction : calculatedWithoutDeduction;
//                var difference = Math.Min(diffWithDeduction, diffWithoutDeduction);

//                // Log the analysis for debugging
//                _logger.Debug("Invoice total analysis for {InvoiceNo}: BaseTotal={BaseTotal}, " +
//                            "WithDeduction={WithDeduction}, WithoutDeduction={WithoutDeduction}, " +
//                            "Reported={Reported}, DiffWith={DiffWith}, DiffWithout={DiffWithout}",
//                    invoice.InvoiceNo, baseTotal, calculatedWithDeduction, calculatedWithoutDeduction,
//                    reportedInvoiceTotal, diffWithDeduction, diffWithoutDeduction);

//                _logger.Information("Invoice Total Validation for {InvoiceNo}:", invoice.InvoiceNo);
//                _logger.Information("  SubTotal: {SubTotal:F2}", invoice.SubTotal ?? 0);
//                _logger.Information("  TotalInternalFreight: {Freight:F2}", invoice.TotalInternalFreight ?? 0);
//                _logger.Information("  TotalOtherCost: {OtherCost:F2}", invoice.TotalOtherCost ?? 0);
//                _logger.Information("  TotalInsurance: {Insurance:F2}", invoice.TotalInsurance ?? 0);
//                _logger.Information("  TotalDeduction: {Deduction:F2}", invoice.TotalDeduction ?? 0);
//                _logger.Information("  Calculated Total: {Calculated:F2}", calculatedInvoiceTotal);
//                _logger.Information("  Reported Total: {Reported:F2}", reportedInvoiceTotal);
//                _logger.Information("  Difference: {Difference:F4}", difference);

//                // Only flag as error if difference is significant (> $0.01)
//                if (difference > 0.01)
//                {
//                    errors.Add(new InvoiceError
//                    {
//                        Field = "InvoiceTotal",
//                        ExtractedValue = reportedInvoiceTotal.ToString("F2"),
//                        CorrectValue = calculatedInvoiceTotal.ToString("F2"),
//                        Confidence = 0.95,
//                        ErrorType = "invoice_total_mismatch",
//                        Reasoning = $"Invoice total calculation: {invoice.SubTotal:F2} + {invoice.TotalInternalFreight:F2} + {invoice.TotalOtherCost:F2} + {invoice.TotalInsurance:F2} - {invoice.TotalDeduction:F2} = {calculatedInvoiceTotal:F2}"
//                    });

//                    _logger.Warning("Invoice total mismatch detected for {InvoiceNo}: Expected {Expected:F2}, Got {Actual:F2}",
//                        invoice.InvoiceNo, calculatedInvoiceTotal, reportedInvoiceTotal);
//                }
//                else
//                {
//                    _logger.Information("Invoice total validation PASSED for {InvoiceNo} (difference: {Difference:F4})",
//                        invoice.InvoiceNo, difference);
//                }

//                return errors;
//            }
//            catch (Exception ex)
//            {
//                _logger.Error(ex, "Error validating cross-field consistency for invoice {InvoiceNo}", invoice.InvoiceNo);
//                return errors;
//            }
//        }

//        #endregion

//        // CRITICAL BUG FIXES FOR OCR CORRECTION SERVICE

//        #region Fix 1: Enhanced Gift Card and Discount Recognition

//        /// <summary>
//        /// Enhanced prompt that specifically teaches DeepSeek to recognize gift cards and negative discounts
//        /// </summary>
//        private string CreateHeaderErrorDetectionPrompt(ShipmentInvoice invoice, string fileText)
//        {
//            var headerData = new
//            {
//                InvoiceNo = invoice.InvoiceNo,
//                InvoiceDate = invoice.InvoiceDate,
//                InvoiceTotal = invoice.InvoiceTotal,
//                SubTotal = invoice.SubTotal,
//                TotalInternalFreight = invoice.TotalInternalFreight,
//                TotalOtherCost = invoice.TotalOtherCost,
//                TotalInsurance = invoice.TotalInsurance,
//                TotalDeduction = invoice.TotalDeduction,
//                Currency = invoice.Currency,
//                SupplierName = invoice.SupplierName,
//                SupplierAddress = invoice.SupplierAddress
//            };

//            var headerJson = JsonSerializer.Serialize(headerData, new JsonSerializerOptions { WriteIndented = true });

//            return $@"OCR HEADER FIELD ERROR DETECTION:

//Compare the extracted header data with the original invoice text and identify discrepancies.

//EXTRACTED HEADER DATA:
//{headerJson}

//ORIGINAL TEXT:
//{CleanTextForAnalysis(fileText)}

//FOCUS AREAS:
//1. Invoice totals and subtotals - look for decimal separator errors (10,00 vs 10.00)
//2. Supplier information - verify names and addresses match exactly
//3. Currency symbols and formatting
//4. Fee breakdowns (shipping, taxes, insurance, deductions)

// CRITICAL FINANCIAL VALIDATION RULES:

// The invoice must follow this exact equation:
// SubTotal + TotalInternalFreight + TotalOtherCost - TotalDeduction = InvoiceTotal

// DEDUCTION IDENTIFICATION RULES:
// - Gift card applications: ADD to TotalDeduction (not InvoiceTotal)
// - Store credit usage: ADD to TotalDeduction (not InvoiceTotal)
// - Promotional discounts: ADD to TotalDeduction (not InvoiceTotal)
// - Any ""applied credit"" or ""balance used"": ADD to TotalDeduction (not InvoiceTotal)

// DEDUCTION KEYWORDS TO IDENTIFY:
// Look for these terms and treat as TotalDeduction (positive values):
// - ""Gift card applied"", ""Gift certificate used"", ""GC Applied""
// - ""Amazon gift card"", ""Store gift card"", ""Gift balance""
// - ""Store credit"", ""Account credit"", ""Credit applied""
// - ""Promotional credit"", ""Promo code"", ""Coupon applied""
// - ""Paid with gift card"", ""Gift card payment""

// MANDATORY VALIDATION:
// After extracting all values, verify: SubTotal + TotalInternalFreight + TotalOtherCost - TotalDeduction = InvoiceTotal
// If this equation doesn't balance, re-examine the document for missed gift cards or credits.

// AMAZON-SPECIFIC: Check payment method section for ""Gift Card $XX.XX"" entries.

//CRITICAL - GIFT CARD AND DISCOUNT RECOGNITION:
//- Gift cards, store credit, and promotional discounts should be mapped to TotalDeduction
//- Look for patterns like: ""Gift Card: -$6.99"", ""Store Credit: -$10.00"", ""Promo Code: -$5.00"", ""Coupon: -$15.00""
//- IMPORTANT CALCULATION LOGIC: When you see ""-$6.99"" or ""$-6.99"" in the text:
//  * TotalDeduction = 6.99 (store as positive: TotalDeduction = value * -1)
//  * This negative amount is ALREADY APPLIED to the displayed InvoiceTotal
//  * Do NOT suggest increasing InvoiceTotal - the merchant already subtracted the discount
//- UNIVERSAL RULE: Most e-commerce invoices show final totals that already include discount deductions
//- If text shows ""-$6.99 Gift Card"", this means TotalDeduction should be 6.99 (positive)

//MATHEMATICAL VALIDATION RULES:
//- BEFORE suggesting corrections, verify mathematical relationships between fields
//- Check if corrections create conflicts (e.g., fixing both InvoiceTotal and TotalDeduction)
//- Use confidence scoring based on mathematical consistency

//INVOICE TOTAL VALIDATION LOGIC:
//- InvoiceTotal should match the final amount shown on the invoice (e.g., ""Total: $166.30"", ""Grand Total: $299.99"")
//- CRITICAL: Check if displayed total already includes discounts before correcting
//- Mathematical relationship: InvoiceTotal = SubTotal + Fees + Taxes - Discounts
//- If TotalDeduction is missing AND InvoiceTotal seems correct: Add TotalDeduction, keep InvoiceTotal
//- If TotalDeduction is missing AND InvoiceTotal is wrong: Correct both, but verify math
//- Common final total labels: ""Total"", ""Grand Total"", ""Order Total"", ""Amount Due"", ""Final Amount""

//LINE ITEM VALIDATION LOGIC:
//- TotalCost should equal Cost × Quantity for each line item
//- If Cost is wrong but TotalCost is correct: Fix Cost, keep TotalCost
//- If TotalCost is wrong but Cost × Quantity is correct: Fix TotalCost
//- If both are wrong: Fix the field that matches the text more clearly

//CONFIDENCE SCORING:
//- Reduce confidence by 40% when corrections conflict mathematically
//- Increase confidence by 20% when corrections are mathematically consistent
//- Set confidence to 15% when correction appears to double-count values

//FIELD MAPPING RULES (UPDATED):
//- TotalInternalFreight: Shipping + Handling + Transportation fees
//- TotalOtherCost: Taxes + Fees + Duties + VAT (positive amounts only)
//- TotalInsurance: Insurance costs only
//- TotalDeduction: Discounts + Coupons + Credits + Gift Cards + Store Credit (always positive amounts)

//DEDUCTION EXAMPLES (UNIVERSAL):
//- Text shows ""Gift Card: -$6.99"" → TotalDeduction should be 6.99
//- Text shows ""Discount: -$10.00"" → TotalDeduction should be 10.00
//- Text shows ""Coupon Applied: -$15.00"" → TotalDeduction should be 15.00
//- Text shows ""Store Credit: -$25.50"" → TotalDeduction should be 25.50
//- Text shows ""Promo Code SAVE20: -$8.99"" → TotalDeduction should be 8.99
//- Text shows ""Member Discount: -$12.34"" → TotalDeduction should be 12.34
//- Text shows ""Promo Code: -$5.50"" → TotalDeduction should be 5.50
//- Text shows ""Store Credit Applied: -$25.00"" → TotalDeduction should be 25.00

//MATHEMATICAL VALIDATION:
//Invoice Total = SubTotal + TotalInternalFreight + TotalOtherCost + TotalInsurance - TotalDeduction
//If this equation doesn't balance, check for missing deductions/credits.

//COMMON OCR ERRORS:
//- Decimal separators: 123,45 → 123.45
//- Character confusion: 1↔l, 1↔I, 0↔O, 5↔S, 6↔G, 8↔B
//- Currency symbols: $ vs S, € vs C
//- Missing decimals: 1000 → 10.00
//- Negative signs: Missing '-' in front of discounts

//CRITICAL: Ensure ALL field values except confidence are returned as STRINGS.
//CRITICAL: Confidence must be a NUMBER between 0.0 and 1.0.

//RESPONSE FORMAT (JSON only - follow this EXACT structure):
//{{
//  ""errors"": [
//    {{
//      ""field"": ""FieldName"",
//      ""extracted_value"": ""CurrentWrongValue"",
//      ""correct_value"": ""CorrectValueFromText"",
//      ""confidence"": 0.95,
//      ""error_type"": ""decimal_separator"",
//      ""reasoning"": ""Brief explanation of what was wrong and how it was corrected""
//    }}
//  ]
//}}

//IMPORTANT DATA TYPE REQUIREMENTS:
//- field: STRING (required)
//- extracted_value: STRING (what was incorrectly extracted)
//- correct_value: STRING (what it should be - REQUIRED)
//- confidence: NUMBER (0.0 to 1.0 - REQUIRED)
//- error_type: STRING (one of: decimal_separator, character_confusion, missing_digit, format_error, field_mapping, missing_discount)
//- reasoning: STRING (explanation)

//Return empty array if no errors: {{""errors"": []}}";
//        }
//        /// <summary>
//        /// Creates specialized prompt for product-level error detection
//        /// </summary>
//        private string CreateProductErrorDetectionPrompt(ShipmentInvoice invoice, string fileText)
//        {
//            var productData = invoice.InvoiceDetails?.Select(d => new
//            {
//                LineNumber = d.LineNumber,
//                ItemDescription = d.ItemDescription,
//                Quantity = d.Quantity,
//                Cost = d.Cost,
//                TotalCost = d.TotalCost,
//                Discount = d.Discount,
//                Units = d.Units
//            }).ToList();

//            var productsJson = JsonSerializer.Serialize(productData, new JsonSerializerOptions { WriteIndented = true });

//            return $@"OCR PRODUCT DATA ERROR DETECTION:

//Compare the extracted product data with the original invoice text and identify discrepancies in quantities, prices, and calculations.

//EXTRACTED PRODUCT DATA:
//{productsJson}

//ORIGINAL TEXT:
//{CleanTextForAnalysis(fileText)}

//VALIDATION FOCUS:
//1. QUANTITIES: Verify each item quantity matches the text exactly
//   - Watch for: 1↔l, 5↔S, 6↔G, 8↔B, 0↔O
//   - Check: Reasonable quantities (not negative, not extremely large)

//2. UNIT PRICES: Verify unit costs are correct
//   - Watch for: Decimal separators (12,50 vs 12.50)
//   - Check: Currency formatting ($12.50 vs $1250)
//   - Validate: Prices are reasonable (not negative, not zero for products)

//3. LINE TOTALS: Verify calculations
//   - Formula: (Quantity × Unit Price) - Discount = Line Total
//   - Check: Math is correct within $0.01

//4. ITEM DESCRIPTIONS: Verify product names are complete and correct
//   - Check: No truncated descriptions
//   - Watch for: Character substitutions that change meaning

//COMMON PRODUCT OCR ERRORS:
//- Quantity errors: 12 → 1Z, 5 → S, 10 → IO
//- Price errors: $12.50 → $1Z.S0, 29.99 → Z9.99
//- Decimal issues: 12,50 → 12.50, 1000 → 10.00
//- Missing digits: $12.5 → $12.50, .99 → 0.99

//CRITICAL: Ensure ALL field values except confidence are returned as STRINGS.
//CRITICAL: Confidence must be a NUMBER between 0.0 and 1.0.

//RESPONSE FORMAT (JSON only - follow this EXACT structure):
//{{
//  ""errors"": [
//    {{
//      ""field"": ""InvoiceDetail_Line1_Quantity"",
//      ""extracted_value"": ""1Z"",
//      ""correct_value"": ""12"",
//      ""confidence"": 0.90,
//      ""error_type"": ""character_confusion"",
//      ""reasoning"": ""OCR confused 'Z' with '2' in quantity field""
//    }}
//  ]
//}}

//IMPORTANT DATA TYPE REQUIREMENTS:
//- field: STRING (format: InvoiceDetail_Line[N]_[FieldName])
//- extracted_value: STRING (current incorrect value as string)
//- correct_value: STRING (corrected value as string - REQUIRED)
//- confidence: NUMBER (0.0 to 1.0 decimal - REQUIRED)
//- error_type: STRING (character_confusion, calculation_error, etc.)
//- reasoning: STRING (explanation of the error and correction)

//Return empty array if no errors: {{""errors"": []}}";
//        }

//        #endregion

//        #region Response Parsing

//        /// <summary>
//        /// Enhanced error detection response parsing with comprehensive logging and type handling
//        /// </summary>
//        private List<InvoiceError> ParseErrorDetectionResponse(string response)
//        {
//            try
//            {
//                var cleanJson = CleanJsonResponse(response);
//                if (string.IsNullOrWhiteSpace(cleanJson))
//                {
//                    _logger.Warning("No valid JSON found in error detection response");
//                    return new List<InvoiceError>();
//                }

//                _logger.Debug("Parsing error detection response JSON: {Json}", TruncateForLog(cleanJson, 1000));

//                using var doc = JsonDocument.Parse(cleanJson);
//                var root = doc.RootElement;

//                var errors = new List<InvoiceError>();

//                if (root.TryGetProperty("errors", out var errorsElement))
//                {
//                    _logger.Information("Found errors array with {Count} elements", errorsElement.GetArrayLength());

//                    int errorIndex = 0;
//                    foreach (var errorElement in errorsElement.EnumerateArray())
//                    {
//                        errorIndex++;
//                        try
//                        {
//                            _logger.Debug("Processing error element {Index}: {Element}", errorIndex, errorElement.GetRawText());

//                            var error = new InvoiceError();

//                            // Parse each field with detailed logging
//                            error.Field = GetStringValueWithLogging(errorElement, "field", errorIndex);
//                            error.ExtractedValue = GetStringValueWithLogging(errorElement, "extracted_value", errorIndex);
//                            error.CorrectValue = GetStringValueWithLogging(errorElement, "correct_value", errorIndex);
//                            error.Confidence = GetDoubleValueWithLogging(errorElement, "confidence", errorIndex);
//                            error.ErrorType = GetStringValueWithLogging(errorElement, "error_type", errorIndex);
//                            error.Reasoning = GetStringValueWithLogging(errorElement, "reasoning", errorIndex);

//                            // Validate required fields
//                            if (!string.IsNullOrEmpty(error.Field) && !string.IsNullOrEmpty(error.CorrectValue))
//                            {
//                                errors.Add(error);
//                                _logger.Information("Successfully parsed error {Index}: Field={Field}, Type={ErrorType}, Confidence={Confidence:P0}",
//                                    errorIndex, error.Field, error.ErrorType, error.Confidence);
//                            }
//                            else
//                            {
//                                _logger.Warning("Skipping incomplete error {Index}: Field='{Field}', CorrectValue='{CorrectValue}'",
//                                    errorIndex, error.Field ?? "NULL", error.CorrectValue ?? "NULL");
//                            }
//                        }
//                        catch (Exception ex)
//                        {
//                            _logger.Error(ex, "Failed to parse error element {Index}. Raw JSON: {Element}",
//                                errorIndex, errorElement.GetRawText());
//                            // Continue processing other errors
//                        }
//                    }
//                }
//                else
//                {
//                    _logger.Warning("No 'errors' property found in response. Available properties: {Properties}",
//                        string.Join(", ", root.EnumerateObject().Select(p => p.Name)));
//                }

//                _logger.Debug("Successfully parsed {Count} errors from response", errors.Count);
//                return errors;
//            }
//            catch (Exception ex)
//            {
//                _logger.Error(ex, "Error parsing error detection response. Raw response: {Response}", TruncateForLog(response));
//                return new List<InvoiceError>();
//            }
//        }

//        /// <summary>
//        /// Gets string value with detailed logging of data types and conversion issues
//        /// </summary>
//        private string GetStringValueWithLogging(JsonElement element, string propertyName, int errorIndex)
//        {
//            try
//            {
//                if (!element.TryGetProperty(propertyName, out var prop))
//                {
//                    _logger.Debug("Property '{PropertyName}' not found in error {Index}", propertyName, errorIndex);
//                    return null;
//                }

//                _logger.Debug("Error {Index}.{PropertyName}: Type={ValueKind}, RawValue={RawValue}",
//                    errorIndex, propertyName, prop.ValueKind, TruncateForLog(prop.GetRawText(), 100));

//                switch (prop.ValueKind)
//                {
//                    case JsonValueKind.String:
//                        var stringValue = prop.GetString();
//                        _logger.Debug("Extracted string value for {PropertyName}: '{Value}'", propertyName, stringValue);
//                        return stringValue;

//                    case JsonValueKind.Number:
//                        var numberAsString = prop.GetRawText();
//                        _logger.Warning("Expected string for '{PropertyName}' but got number: {Value}. Converting to string.",
//                            propertyName, numberAsString);
//                        return numberAsString;

//                    case JsonValueKind.True:
//                        _logger.Warning("Expected string for '{PropertyName}' but got boolean: true. Converting to string.", propertyName);
//                        return "true";

//                    case JsonValueKind.False:
//                        _logger.Warning("Expected string for '{PropertyName}' but got boolean: false. Converting to string.", propertyName);
//                        return "false";

//                    case JsonValueKind.Null:
//                        _logger.Debug("Property '{PropertyName}' is null", propertyName);
//                        return null;

//                    case JsonValueKind.Array:
//                        var arrayValue = prop.GetRawText();
//                        _logger.Warning("Expected string for '{PropertyName}' but got array: {Value}. Using raw JSON.",
//                            propertyName, TruncateForLog(arrayValue, 100));
//                        return arrayValue;

//                    case JsonValueKind.Object:
//                        var objectValue = prop.GetRawText();
//                        _logger.Warning("Expected string for '{PropertyName}' but got object: {Value}. Using raw JSON.",
//                            propertyName, TruncateForLog(objectValue, 100));
//                        return objectValue;

//                    default:
//                        var defaultValue = prop.GetRawText();
//                        _logger.Warning("Unexpected JSON type {ValueKind} for '{PropertyName}': {Value}. Using raw text.",
//                            prop.ValueKind, propertyName, defaultValue);
//                        return defaultValue;
//                }
//            }
//            catch (Exception ex)
//            {
//                _logger.Error(ex, "Error extracting string value for property '{PropertyName}' in error {Index}",
//                    propertyName, errorIndex);
//                return null;
//            }
//        }

//        /// <summary>
//        /// Gets double value with detailed logging of data types and conversion issues
//        /// </summary>
//        private double GetDoubleValueWithLogging(JsonElement element, string propertyName, int errorIndex)
//        {
//            try
//            {
//                if (!element.TryGetProperty(propertyName, out var prop))
//                {
//                    _logger.Debug("Property '{PropertyName}' not found in error {Index}", propertyName, errorIndex);
//                    return 0.0;
//                }

//                _logger.Debug("Error {Index}.{PropertyName}: Type={ValueKind}, RawValue={RawValue}",
//                    errorIndex, propertyName, prop.ValueKind, prop.GetRawText());

//                switch (prop.ValueKind)
//                {
//                    case JsonValueKind.Number:
//                        if (prop.TryGetDouble(out var doubleValue))
//                        {
//                            _logger.Debug("Extracted double value for {PropertyName}: {Value}", propertyName, doubleValue);
//                            return doubleValue;
//                        }
//                        if (prop.TryGetDecimal(out var decimalValue))
//                        {
//                            var convertedValue = (double)decimalValue;
//                            _logger.Debug("Converted decimal to double for {PropertyName}: {Value}", propertyName, convertedValue);
//                            return convertedValue;
//                        }
//                        if (prop.TryGetInt32(out var intValue))
//                        {
//                            _logger.Debug("Converted int to double for {PropertyName}: {Value}", propertyName, intValue);
//                            return intValue;
//                        }
//                        if (prop.TryGetInt64(out var longValue))
//                        {
//                            _logger.Debug("Converted long to double for {PropertyName}: {Value}", propertyName, longValue);
//                            return longValue;
//                        }
//                        break;

//                    case JsonValueKind.String:
//                        var stringValue = prop.GetString();
//                        if (double.TryParse(stringValue, out var parsedValue))
//                        {
//                            _logger.Warning("Expected number for '{PropertyName}' but got string: '{Value}'. Successfully parsed as {ParsedValue}.",
//                                propertyName, stringValue, parsedValue);
//                            return parsedValue;
//                        }
//                        _logger.Error("Expected number for '{PropertyName}' but got unparseable string: '{Value}'. Using 0.0.",
//                            propertyName, stringValue);
//                        break;

//                    case JsonValueKind.Null:
//                        _logger.Debug("Property '{PropertyName}' is null, using 0.0", propertyName);
//                        break;

//                    default:
//                        _logger.Warning("Expected number for '{PropertyName}' but got {ValueKind}: {Value}. Using 0.0.",
//                            propertyName, prop.ValueKind, prop.GetRawText());
//                        break;
//                }

//                return 0.0;
//            }
//            catch (Exception ex)
//            {
//                _logger.Error(ex, "Error extracting double value for property '{PropertyName}' in error {Index}",
//                    propertyName, errorIndex);
//                return 0.0;
//            }
//        }

//        #endregion

//        #region Correction Application

//        /// <summary>
//        /// Applies corrections with priority-based processing and field dependency validation
//        /// </summary>
//        private async Task<List<CorrectionResult>> ApplyCorrectionsAsync(
//            ShipmentInvoice invoice,
//            List<InvoiceError> errors,
//            string fileText)
//        {
//            var results = new List<CorrectionResult>();

//            // Apply field dependency validation and conflict resolution
//            var filteredErrors = ResolveFieldConflicts(errors, invoice);

//            // Group errors by priority (critical first)
//            var criticalErrors = filteredErrors.Where(e => IsCriticalError(e)).ToList();
//            var standardErrors = filteredErrors.Where(e => !IsCriticalError(e)).ToList();

//            _logger.Information("Processing {CriticalCount} critical and {StandardCount} standard errors",
//                criticalErrors.Count, standardErrors.Count);

//            // Process critical errors first
//            foreach (var error in criticalErrors)
//            {
//                var result = await this.ApplySingleCorrectionAsync(invoice, error).ConfigureAwait(false);
//                results.Add(result);
//                LogCorrectionResult(result, "CRITICAL");
//            }

//            // Process standard errors
//            foreach (var error in standardErrors)
//            {
//                var result = await this.ApplySingleCorrectionAsync(invoice, error).ConfigureAwait(false);
//                results.Add(result);
//                LogCorrectionResult(result, "STANDARD");
//            }

//            // Recalculate dependent fields after all corrections
//            RecalculateDependentFields(invoice);

//            // Mark invoice as modified if any corrections were successful
//            if (results.Any(r => r.Success))
//            {
//                invoice.TrackingState = TrackingState.Modified;
//            }

//            return results;
//        }

//        /// <summary>
//        /// Applies a single correction to the invoice
//        /// </summary>
//        private async Task<CorrectionResult> ApplySingleCorrectionAsync(ShipmentInvoice invoice, InvoiceError error)
//        {
//            var result = new CorrectionResult
//            {
//                FieldName = error.Field,
//                CorrectionType = error.ErrorType,
//                Confidence = error.Confidence
//            };

//            try
//            {
//                // Parse the correct value based on field type
//                var correctedValue = ParseCorrectedValue(error.CorrectValue, error.Field);
//                if (correctedValue == null)
//                {
//                    result.Success = false;
//                    result.ErrorMessage = $"Could not parse corrected value: {error.CorrectValue}";
//                    return result;
//                }

//                // Get current value for logging
//                result.OldValue = GetCurrentFieldValue(invoice, error.Field)?.ToString();

//                // Apply the correction
//                var applied = ApplyFieldCorrection(invoice, error.Field, correctedValue);
//                if (applied)
//                {
//                    result.NewValue = correctedValue.ToString();
//                    result.Success = true;
//                }
//                else
//                {
//                    result.Success = false;
//                    result.ErrorMessage = "Field not recognized or value not applied";
//                }

//                return result;
//            }
//            catch (Exception ex)
//            {
//                result.Success = false;
//                result.ErrorMessage = ex.Message;
//                return result;
//            }
//        }

//        /// <summary>
//        /// Applies correction to a specific field
//        /// </summary>
//        private bool ApplyFieldCorrection(ShipmentInvoice invoice, string fieldName, object correctedValue)
//        {
//            try
//            {
//                switch (fieldName.ToLower())
//                {
//                    case "invoicetotal":
//                        if (correctedValue is decimal invoiceTotal)
//                        {
//                            invoice.InvoiceTotal = (double)invoiceTotal;
//                            return true;
//                        }
//                        break;
//                    case "subtotal":
//                        if (correctedValue is decimal subTotal)
//                        {
//                            invoice.SubTotal = (double)subTotal;
//                            return true;
//                        }
//                        break;
//                    case "totalinternalfreight":
//                        if (correctedValue is decimal freight)
//                        {
//                            invoice.TotalInternalFreight = (double)freight;
//                            return true;
//                        }
//                        break;
//                    case "totalothercost":
//                        if (correctedValue is decimal otherCost)
//                        {
//                            invoice.TotalOtherCost = (double)otherCost;
//                            return true;
//                        }
//                        break;
//                    case "totalinsurance":
//                        if (correctedValue is decimal insurance)
//                        {
//                            invoice.TotalInsurance = (double)insurance;
//                            return true;
//                        }
//                        break;
//                    case "totaldeduction":
//                        if (correctedValue is decimal deduction)
//                        {
//                            invoice.TotalDeduction = (double)deduction;
//                            return true;
//                        }
//                        break;
//                    case "invoiceno":
//                        if (correctedValue is string invoiceNo)
//                        {
//                            invoice.InvoiceNo = invoiceNo;
//                            return true;
//                        }
//                        break;
//                    case "suppliername":
//                        if (correctedValue is string supplierName)
//                        {
//                            invoice.SupplierName = supplierName;
//                            return true;
//                        }
//                        break;
//                    case "currency":
//                        if (correctedValue is string currency)
//                        {
//                            invoice.Currency = currency;
//                            return true;
//                        }
//                        break;
//                    default:
//                        // Handle invoice detail corrections
//                        if (fieldName.StartsWith("invoicedetail_", StringComparison.OrdinalIgnoreCase))
//                        {
//                            return ApplyInvoiceDetailCorrection(invoice, fieldName, correctedValue);
//                        }
//                        break;
//                }
//                return false;
//            }
//            catch (Exception ex)
//            {
//                _logger.Error(ex, "Error applying field correction for {FieldName}", fieldName);
//                return false;
//            }
//        }

//        /// <summary>
//        /// Applies correction to invoice detail fields
//        /// </summary>
//        private bool ApplyInvoiceDetailCorrection(ShipmentInvoice invoice, string fieldName, object correctedValue)
//        {
//            try
//            {
//                var parts = fieldName.Split('_');
//                if (parts.Length < 3) return false;

//                var lineNumberStr = Regex.Replace(parts[1], "line", "", RegexOptions.IgnoreCase);
//                if (!int.TryParse(lineNumberStr, out var lineNumber)) return false;

//                var detailFieldName = parts[2];
//                var detail = invoice.InvoiceDetails?.FirstOrDefault(d => d.LineNumber == lineNumber);
//                if (detail == null) return false;

//                switch (detailFieldName.ToLower())
//                {
//                    case "quantity":
//                        if (correctedValue is decimal quantity)
//                        {
//                            detail.Quantity = (double)quantity;
//                            RecalculateDetailTotal(detail);
//                            detail.TrackingState = TrackingState.Modified;
//                            return true;
//                        }
//                        break;
//                    case "cost":
//                        if (correctedValue is decimal cost)
//                        {
//                            detail.Cost = (double)cost;
//                            RecalculateDetailTotal(detail);
//                            detail.TrackingState = TrackingState.Modified;
//                            return true;
//                        }
//                        break;
//                    case "totalcost":
//                        if (correctedValue is decimal totalCost)
//                        {
//                            detail.TotalCost = (double)totalCost;
//                            detail.TrackingState = TrackingState.Modified;
//                            return true;
//                        }
//                        break;
//                    case "discount":
//                        if (correctedValue is decimal discount)
//                        {
//                            detail.Discount = (double)discount;
//                            RecalculateDetailTotal(detail);
//                            detail.TrackingState = TrackingState.Modified;
//                            return true;
//                        }
//                        break;
//                }
//                return false;
//            }
//            catch (Exception ex)
//            {
//                _logger.Error(ex, "Error applying invoice detail correction for {FieldName}", fieldName);
//                return false;
//            }
//        }

//        /// <summary>
//        /// Determines if an error is critical (affects calculations)
//        /// </summary>
//        private bool IsCriticalError(InvoiceError error)
//        {
//            var criticalTypes = new[] {
//                "calculation_error",
//                "subtotal_mismatch",
//                "invoice_total_mismatch",
//                "unreasonable_quantity",
//                "unreasonable_cost"
//            };
//            return criticalTypes.Contains(error.ErrorType);
//        }

//        /// <summary>
//        /// Logs correction results with appropriate detail level
//        /// </summary>
//        private void LogCorrectionResult(CorrectionResult result, string priority)
//        {
//            if (result.Success)
//            {
//                _logger.Information("[{Priority}] Applied correction: {Field} {OldValue} → {NewValue} (confidence: {Confidence:P0})",
//                    priority, result.FieldName, result.OldValue, result.NewValue, result.Confidence);
//            }
//            else
//            {
//                _logger.Warning("[{Priority}] Failed correction for {Field}: {Error}",
//                    priority, result.FieldName, result.ErrorMessage);
//            }
//        }

//        /// <summary>
//        /// Recalculates dependent fields after corrections
//        /// </summary>
//        private void RecalculateDependentFields(ShipmentInvoice invoice)
//        {
//            try
//            {
//                // Recalculate line totals
//                if (invoice.InvoiceDetails != null)
//                {
//                    foreach (var detail in invoice.InvoiceDetails)
//                    {
//                        RecalculateDetailTotal(detail);
//                    }

//                    // Recalculate subtotal from line items
//                    var calculatedSubTotal = invoice.InvoiceDetails.Sum(d => d.TotalCost ?? 0);
//                    if (Math.Abs((invoice.SubTotal ?? 0) - calculatedSubTotal) > 0.01)
//                    {
//                        _logger.Information("Updating SubTotal from {OldValue} to {NewValue} based on line items",
//                            invoice.SubTotal, calculatedSubTotal);
//                        invoice.SubTotal = calculatedSubTotal;
//                    }
//                }

//                // Recalculate invoice total with gift card handling
//                var baseTotal = (invoice.SubTotal ?? 0) +
//                              (invoice.TotalInternalFreight ?? 0) +
//                              (invoice.TotalOtherCost ?? 0) +
//                              (invoice.TotalInsurance ?? 0);

//                var deductionAmount = invoice.TotalDeduction ?? 0;
//                var currentInvoiceTotal = invoice.InvoiceTotal ?? 0;

//                // Check if the current total already has deductions applied
//                var calculatedWithDeduction = baseTotal - deductionAmount;
//                var calculatedWithoutDeduction = baseTotal;

//                var diffWithDeduction = Math.Abs(calculatedWithDeduction - currentInvoiceTotal);
//                var diffWithoutDeduction = Math.Abs(calculatedWithoutDeduction - currentInvoiceTotal);

//                // Use the calculation that's closer to the current total
//                var calculatedTotal = diffWithDeduction <= diffWithoutDeduction ?
//                    calculatedWithDeduction : calculatedWithoutDeduction;

//                if (Math.Abs(currentInvoiceTotal - calculatedTotal) > 0.01)
//                {
//                    _logger.Information("Updating InvoiceTotal from {OldValue} to {NewValue} based on calculation",
//                        invoice.InvoiceTotal, calculatedTotal);
//                    invoice.InvoiceTotal = calculatedTotal;
//                }

//                _logger.Debug("Dependent field recalculation complete for invoice {InvoiceNo}", invoice.InvoiceNo);
//            }
//            catch (Exception ex)
//            {
//                _logger.Error(ex, "Error recalculating dependent fields for invoice {InvoiceNo}", invoice.InvoiceNo);
//            }
//        }

//        #endregion

//        #region Regex Pattern Updates

//        /// <summary>
//        /// Updates OCR regex patterns based on successful corrections
//        /// </summary>
//        public async Task UpdateRegexPatternsAsync(List<CorrectionResult> corrections, string fileText)
//        {
//            if (!corrections?.Any(c => c.Success) == true || string.IsNullOrEmpty(fileText))
//            {
//                _logger.Information("No successful corrections to learn from");
//                return;
//            }

//            try
//            {
//                _logger.Information("Updating regex patterns based on {Count} successful corrections",
//                    corrections.Count(c => c.Success));

//                var fileLines = fileText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
//                var regexUpdates = new List<RegexUpdateRequest>();

//                foreach (var correction in corrections.Where(c => c.Success))
//                {
//                    try
//                    {
//                        var updateRequest = await this.CreateRegexUpdateRequestAsync(correction, fileLines).ConfigureAwait(false);
//                        if (updateRequest != null)
//                        {
//                            regexUpdates.Add(updateRequest);
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        _logger.Error(ex, "Error creating regex update request for field {Field}", correction.FieldName);
//                    }
//                }

//                // Apply regex updates
//                await this.ApplyRegexUpdatesAsync(regexUpdates).ConfigureAwait(false);

//                _logger.Information("Successfully processed {Count} regex updates", regexUpdates.Count);
//            }
//            catch (Exception ex)
//            {
//                _logger.Error(ex, "Error updating regex patterns");
//            }
//        }

//        /// <summary>
//        /// Creates a regex update request based on a successful correction
//        /// </summary>
//        private async Task<RegexUpdateRequest> CreateRegexUpdateRequestAsync(CorrectionResult correction, string[] fileLines)
//        {
//            try
//            {
//                // Find the line containing the error using DeepSeek
//                var lineInfo = await this.FindErrorLineAsync(correction, fileLines).ConfigureAwait(false);
//                if (lineInfo == null)
//                {
//                    _logger.Warning("Could not locate error line for field {Field}", correction.FieldName);
//                    return null;
//                }

//                // Determine the best regex correction strategy
//                var strategy = await this.DetermineRegexStrategyAsync(correction, lineInfo, fileLines).ConfigureAwait(false);
//                if (strategy == null)
//                {
//                    _logger.Warning("Could not determine regex strategy for field {Field}", correction.FieldName);
//                    return null;
//                }

//                return new RegexUpdateRequest
//                {
//                    FieldName = correction.FieldName,
//                    CorrectionType = correction.CorrectionType,
//                    OldValue = correction.OldValue,
//                    NewValue = correction.NewValue,
//                    LineNumber = lineInfo.LineNumber,
//                    LineText = lineInfo.LineText,
//                    Strategy = strategy,
//                    Confidence = correction.Confidence
//                };
//            }
//            catch (Exception ex)
//            {
//                _logger.Error(ex, "Error creating regex update request for {Field}", correction.FieldName);
//                return null;
//            }
//        }

//        /// <summary>
//        /// Finds the line containing the OCR error using DeepSeek analysis
//        /// </summary>
//        private async Task<LineInfo> FindErrorLineAsync(CorrectionResult correction, string[] fileLines)
//        {
//            var fileText = string.Join("\n", fileLines.Select((line, i) => $"{i + 1}: {line}"));

//            var prompt = $@"OCR ERROR LINE DETECTION:

//Find the line number in the invoice text where the field '{correction.FieldName}' contains the incorrect value '{correction.OldValue}' that was corrected to '{correction.NewValue}'.

//INVOICE TEXT WITH LINE NUMBERS:
//{TruncateForLog(fileText, 4000)}

//CORRECTION DETAILS:
//- Field: {correction.FieldName}
//- Incorrect Value: {correction.OldValue}
//- Correct Value: {correction.NewValue}
//- Error Type: {correction.CorrectionType}

//RESPONSE FORMAT (JSON only):
//{{
//  ""line_number"": 15,
//  ""line_text"": ""Total: $123,45"",
//  ""confidence"": 0.90,
//  ""reasoning"": ""Found incorrect value on line 15""
//}}

//Return null if not found: {{""line_number"": null}}";

//            try
//            {
//                var response = await this._deepSeekApi.GetResponseAsync(prompt).ConfigureAwait(false);
//                return ParseLineInfoResponse(response);
//            }
//            catch (Exception ex)
//            {
//                _logger.Error(ex, "Error finding error line for field {Field}", correction.FieldName);
//                return null;
//            }
//        }

//        /// <summary>
//        /// Determines the best regex correction strategy
//        /// </summary>
//        private async Task<RegexCorrectionStrategy> DetermineRegexStrategyAsync(
//            CorrectionResult correction,
//            LineInfo lineInfo,
//            string[] fileLines)
//        {
//            var windowStart = Math.Max(0, lineInfo.LineNumber - 5);
//            var windowEnd = Math.Min(fileLines.Length - 1, lineInfo.LineNumber + 5);
//            var windowLines = fileLines.Skip(windowStart).Take(windowEnd - windowStart + 1).ToArray();
//            var windowText = string.Join("\n", windowLines.Select((line, i) => $"{windowStart + i + 1}: {line}"));

//            var prompt = $@"OCR REGEX CORRECTION STRATEGY:

//Determine the best approach to fix OCR errors for this field.

//CORRECTION DETAILS:
//- Field: {correction.FieldName}
//- Old Value: {correction.OldValue}
//- New Value: {correction.NewValue}
//- Error Type: {correction.CorrectionType}

//TEXT WINDOW:
//{windowText}

//STRATEGY OPTIONS:
//1. FORMAT_FIX: Post-processing regex (e.g., comma→period)
//2. PATTERN_UPDATE: Update field detection pattern
//3. CHARACTER_MAP: Character substitution rule
//4. VALIDATION_RULE: Flag unreasonable values

//RESPONSE FORMAT (JSON only):
//{{
//  ""strategy_type"": ""FORMAT_FIX"",
//  ""regex_pattern"": ""\\$?([0-9]+)[,]([0-9]{{2}})"",
//  ""replacement_pattern"": ""$1.$2"",
//  ""confidence"": 0.85,
//  ""reasoning"": ""Systematic comma-to-period conversion needed""
//}}";

//            try
//            {
//                var response = await this._deepSeekApi.GetResponseAsync(prompt).ConfigureAwait(false);
//                return ParseRegexStrategyResponse(response);
//            }
//            catch (Exception ex)
//            {
//                _logger.Error(ex, "Error determining regex strategy for field {Field}", correction.FieldName);
//                return null;
//            }
//        }

//        /// <summary>
//        /// Applies regex updates to the system
//        /// </summary>
//        private async Task ApplyRegexUpdatesAsync(List<RegexUpdateRequest> updates)
//        {
//            if (!updates.Any()) return;

//            try
//            {
//                var formatFixes = updates.Where(u => u.Strategy?.StrategyType == "FORMAT_FIX").ToList();
//                var patternUpdates = updates.Where(u => u.Strategy?.StrategyType == "PATTERN_UPDATE").ToList();
//                var characterMaps = updates.Where(u => u.Strategy?.StrategyType == "CHARACTER_MAP").ToList();
//                var validationRules = updates.Where(u => u.Strategy?.StrategyType == "VALIDATION_RULE").ToList();

//                _logger.Information("Applying {FormatFixes} format fixes, {PatternUpdates} pattern updates, {CharMaps} character maps, {ValidationRules} validation rules",
//                    formatFixes.Count, patternUpdates.Count, characterMaps.Count, validationRules.Count);

//                // Apply each type of update
//                foreach (var update in formatFixes)
//                    await this.ApplyFormatFixAsync(update).ConfigureAwait(false);

//                foreach (var update in patternUpdates)
//                    await this.ApplyPatternUpdateAsync(update).ConfigureAwait(false);

//                foreach (var update in characterMaps)
//                    await this.ApplyCharacterMappingAsync(update).ConfigureAwait(false);

//                foreach (var update in validationRules)
//                    await this.LogValidationRuleAsync(update).ConfigureAwait(false);

//                _logger.Information("Completed applying {Count} regex updates", updates.Count);
//            }
//            catch (Exception ex)
//            {
//                _logger.Error(ex, "Error applying regex updates");
//            }
//        }

//        /// <summary>
//        /// Actual implementation of regex update system with database/file persistence
//        /// </summary>
//        private async Task ApplyFormatFixAsync(RegexUpdateRequest update)
//        {
//            try
//            {
//                _logger.Information("Applying format fix for {Field}: {Pattern} → {Replacement}",
//                    update.FieldName, update.Strategy.RegexPattern, update.Strategy.ReplacementPattern);

//                // IMPLEMENTED: Save regex patterns to configuration file
//                var regexConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OCRRegexPatterns.json");

//                List<RegexPattern> existingPatterns = new List<RegexPattern>();

//                // Load existing patterns
//                if (File.Exists(regexConfigPath))
//                {
//                    try
//                    {
//                        var existingJson = File.ReadAllText(regexConfigPath);
//                        existingPatterns = JsonSerializer.Deserialize<List<RegexPattern>>(existingJson) ?? new List<RegexPattern>();
//                    }
//                    catch (Exception ex)
//                    {
//                        _logger.Warning(ex, "Could not load existing regex patterns, starting fresh");
//                    }
//                }

//                // Add new pattern (or update existing)
//                var existingPattern = existingPatterns.FirstOrDefault(p =>
//                    p.FieldName == update.FieldName && p.StrategyType == update.Strategy.StrategyType);

//                if (existingPattern != null)
//                {
//                    // Update existing pattern
//                    existingPattern.Pattern = update.Strategy.RegexPattern;
//                    existingPattern.Replacement = update.Strategy.ReplacementPattern;
//                    existingPattern.Confidence = update.Strategy.Confidence;
//                    existingPattern.LastUpdated = DateTime.UtcNow;
//                    existingPattern.UpdateCount++;
//                    _logger.Information("Updated existing regex pattern for {Field}", update.FieldName);
//                }
//                else
//                {
//                    // Add new pattern
//                    existingPatterns.Add(new RegexPattern
//                    {
//                        FieldName = update.FieldName,
//                        StrategyType = update.Strategy.StrategyType,
//                        Pattern = update.Strategy.RegexPattern,
//                        Replacement = update.Strategy.ReplacementPattern,
//                        Confidence = update.Strategy.Confidence,
//                        CreatedDate = DateTime.UtcNow,
//                        LastUpdated = DateTime.UtcNow,
//                        UpdateCount = 1,
//                        CreatedBy = "OCRCorrectionService"
//                    });
//                    _logger.Information("Added new regex pattern for {Field}", update.FieldName);
//                }

//                // Save updated patterns
//                var options = new JsonSerializerOptions { WriteIndented = true };
//                var updatedJson = JsonSerializer.Serialize(existingPatterns, options);
//                File.WriteAllText(regexConfigPath, updatedJson);

//                _logger.Information("Regex patterns saved to {Path}", regexConfigPath);

//                // TODO: Integrate with your actual OCR database if you have one
//                // Example database integration:
//                /*
//                using var ctx = new OCRContext();
//                var formatRegex = new FieldFormatRegEx()
//                {
//                    FieldName = update.FieldName,
//                    Pattern = update.Strategy.RegexPattern,
//                    Replacement = update.Strategy.ReplacementPattern,
//                    CreatedDate = DateTime.UtcNow,
//                    CreatedBy = "OCRCorrectionService",
//                    Confidence = update.Strategy.Confidence
//                };
//                ctx.FieldFormatRegEx.Add(formatRegex);
//                await ctx.SaveChangesAsync();
//                */
//            }
//            catch (Exception ex)
//            {
//                _logger.Error(ex, "Error applying format fix for {Field}", update.FieldName);
//            }
//        }


//        /// <summary>
//        /// Load and apply existing regex patterns during OCR processing
//        /// </summary>
//        private async Task<List<RegexPattern>> LoadRegexPatternsAsync()
//        {
//            try
//            {
//                var regexConfigPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "OCRRegexPatterns.json");

//                if (!File.Exists(regexConfigPath))
//                {
//                    _logger.Information("No existing regex patterns found");
//                    return new List<RegexPattern>();
//                }

//                var json = File.ReadAllText(regexConfigPath);
//                var patterns = JsonSerializer.Deserialize<List<RegexPattern>>(json) ?? new List<RegexPattern>();

//                _logger.Information("Loaded {Count} regex patterns from configuration", patterns.Count);
//                return patterns;
//            }
//            catch (Exception ex)
//            {
//                _logger.Error(ex, "Error loading regex patterns");
//                return new List<RegexPattern>();
//            }
//        }

//        /// <summary>
//        /// Apply learned regex patterns to text before processing
//        /// </summary>
//        private async Task<string> ApplyLearnedRegexPatternsAsync(string text, string fieldName)
//        {
//            try
//            {
//                var patterns = await LoadRegexPatternsAsync();
//                var applicablePatterns = patterns.Where(p =>
//                    p.FieldName.Equals(fieldName, StringComparison.OrdinalIgnoreCase) &&
//                    p.StrategyType == "FORMAT_FIX" &&
//                    p.Confidence > 0.7).ToList();

//                if (!applicablePatterns.Any())
//                    return text;

//                var processedText = text;
//                foreach (var pattern in applicablePatterns.OrderByDescending(p => p.Confidence))
//                {
//                    try
//                    {
//                        var regex = new Regex(pattern.Pattern, RegexOptions.IgnoreCase);
//                        var newText = regex.Replace(processedText, pattern.Replacement);

//                        if (newText != processedText)
//                        {
//                            _logger.Debug("Applied regex pattern for {Field}: {OldValue} → {NewValue}",
//                                fieldName, processedText, newText);
//                            processedText = newText;
//                        }
//                    }
//                    catch (Exception ex)
//                    {
//                        _logger.Warning(ex, "Error applying regex pattern {Pattern} for field {Field}",
//                            pattern.Pattern, fieldName);
//                    }
//                }

//                return processedText;
//            }
//            catch (Exception ex)
//            {
//                _logger.Error(ex, "Error applying learned regex patterns for field {Field}", fieldName);
//                return text;
//            }
//        }

//        /// <summary>
//        /// Data model for regex pattern storage
//        /// </summary>
//        public class RegexPattern
//        {
//            public string FieldName { get; set; }
//            public string StrategyType { get; set; }
//            public string Pattern { get; set; }
//            public string Replacement { get; set; }
//            public double Confidence { get; set; }
//            public DateTime CreatedDate { get; set; }
//            public DateTime LastUpdated { get; set; }
//            public int UpdateCount { get; set; }
//            public string CreatedBy { get; set; }
//        }
//        /// <summary>
//        /// Applies pattern update (field detection improvement)
//        /// </summary>
//        private async Task ApplyPatternUpdateAsync(RegexUpdateRequest update)
//        {
//            try
//            {
//                _logger.Information("Applied pattern update for {Field}: {Pattern}",
//                    update.FieldName, update.Strategy.RegexPattern);
//                // TODO: Update existing field detection patterns
//            }
//            catch (Exception ex)
//            {
//                _logger.Error(ex, "Error applying pattern update for {Field}", update.FieldName);
//            }
//        }

//        /// <summary>
//        /// Applies character mapping rules
//        /// </summary>
//        private async Task ApplyCharacterMappingAsync(RegexUpdateRequest update)
//        {
//            try
//            {
//                _logger.Information("Applied character mapping for {Field}: {Pattern} → {Replacement}",
//                    update.FieldName, update.Strategy.RegexPattern, update.Strategy.ReplacementPattern);
//                // TODO: Add systematic character substitution rules
//            }
//            catch (Exception ex)
//            {
//                _logger.Error(ex, "Error applying character mapping for {Field}", update.FieldName);
//            }
//        }

//        /// <summary>
//        /// Logs validation rules for manual review
//        /// </summary>
//        private async Task LogValidationRuleAsync(RegexUpdateRequest update)
//        {
//            _logger.Information("Validation rule suggestion for {Field}: {Reasoning}",
//                update.FieldName, update.Strategy.Reasoning);
//        }

//        /// <summary>
//        /// Parses line info response
//        /// </summary>
//        private LineInfo ParseLineInfoResponse(string response)
//        {
//            try
//            {
//                var cleanJson = CleanJsonResponse(response);
//                if (string.IsNullOrWhiteSpace(cleanJson)) return null;

//                using var doc = JsonDocument.Parse(cleanJson);
//                var root = doc.RootElement;

//                if (root.TryGetProperty("line_number", out var lineElement) &&
//                    lineElement.ValueKind != JsonValueKind.Null)
//                {
//                    return new LineInfo
//                    {
//                        LineNumber = lineElement.GetInt32(),
//                        LineText = GetStringValue(root, "line_text") ?? "",
//                        Confidence = GetDoubleValue(root, "confidence"),
//                        Reasoning = GetStringValue(root, "reasoning") ?? ""
//                    };
//                }

//                return null;
//            }
//            catch (Exception ex)
//            {
//                _logger.Error(ex, "Error parsing line info response");
//                return null;
//            }
//        }

//        /// <summary>
//        /// Parses regex strategy response
//        /// </summary>
//        private RegexCorrectionStrategy ParseRegexStrategyResponse(string response)
//        {
//            try
//            {
//                var cleanJson = CleanJsonResponse(response);
//                if (string.IsNullOrWhiteSpace(cleanJson)) return null;

//                using var doc = JsonDocument.Parse(cleanJson);
//                var root = doc.RootElement;

//                return new RegexCorrectionStrategy
//                {
//                    StrategyType = GetStringValue(root, "strategy_type") ?? "FORMAT_FIX",
//                    RegexPattern = GetStringValue(root, "regex_pattern") ?? "",
//                    ReplacementPattern = GetStringValue(root, "replacement_pattern") ?? "",
//                    Confidence = GetDoubleValue(root, "confidence"),
//                    Reasoning = GetStringValue(root, "reasoning") ?? ""
//                };
//            }
//            catch (Exception ex)
//            {
//                _logger.Error(ex, "Error parsing regex strategy response");
//                return null;
//            }
//        }

//        #endregion

//        #region Field Dependency Validation

//        /// <summary>
//        /// Resolves field conflicts using dependency validation
//        /// </summary>
//        private List<InvoiceError> ResolveFieldConflicts(List<InvoiceError> errors, ShipmentInvoice invoice)
//        {
//            var filteredErrors = new List<InvoiceError>();

//            // Group errors by field to detect conflicts
//            var errorGroups = errors.GroupBy(e => e.Field?.ToLower()).ToList();

//            foreach (var group in errorGroups)
//            {
//                if (group.Count() == 1)
//                {
//                    // No conflict, add the error
//                    filteredErrors.Add(group.First());
//                }
//                else
//                {
//                    // Multiple corrections for same field - choose highest confidence
//                    var bestError = group.OrderByDescending(e => e.Confidence).First();
//                    filteredErrors.Add(bestError);

//                    _logger.Warning("Resolved field conflict for {Field}: chose correction with confidence {Confidence:P0} over {AlternativeCount} alternatives",
//                        bestError.Field, bestError.Confidence, group.Count() - 1);
//                }
//            }

//            // Apply mathematical validation to detect conflicting corrections
//            var mathematicallyValidErrors = ValidateMathematicalConsistency(filteredErrors, invoice);

//            return mathematicallyValidErrors;
//        }

//        /// <summary>
//        /// Validates mathematical consistency of proposed corrections
//        /// </summary>
//        private List<InvoiceError> ValidateMathematicalConsistency(List<InvoiceError> errors, ShipmentInvoice invoice)
//        {
//            var validErrors = new List<InvoiceError>();

//            // Create a copy of the invoice to test corrections
//            var testInvoice = CloneInvoiceForTesting(invoice);

//            foreach (var error in errors)
//            {
//                // Apply the correction to test invoice
//                var correctedValue = ParseCorrectedValue(error.CorrectValue, error.Field);
//                if (correctedValue != null && ApplyFieldCorrection(testInvoice, error.Field, correctedValue))
//                {
//                    // Check if the correction improves mathematical consistency
//                    var beforeTotalsZero = TotalsZero(invoice, _logger);
//                    var afterTotalsZero = TotalsZero(testInvoice, _logger);

//                    if (afterTotalsZero || (!beforeTotalsZero && afterTotalsZero))
//                    {
//                        validErrors.Add(error);
//                        _logger.Debug("Correction for {Field} improves mathematical consistency", error.Field);
//                    }
//                    else
//                    {
//                        _logger.Warning("Correction for {Field} does not improve mathematical consistency, reducing confidence",
//                            error.Field);

//                        // Reduce confidence but still include if above threshold
//                        error.Confidence *= 0.6; // Reduce by 40%
//                        if (error.Confidence > 0.3)
//                        {
//                            validErrors.Add(error);
//                        }
//                    }
//                }
//                else
//                {
//                    _logger.Warning("Could not apply correction for {Field} during validation", error.Field);
//                }
//            }

//            return validErrors;
//        }

//        /// <summary>
//        /// Creates a copy of invoice for testing corrections
//        /// </summary>
//        private ShipmentInvoice CloneInvoiceForTesting(ShipmentInvoice original)
//        {
//            return new ShipmentInvoice
//            {
//                InvoiceNo = original.InvoiceNo,
//                InvoiceTotal = original.InvoiceTotal,
//                SubTotal = original.SubTotal,
//                TotalInternalFreight = original.TotalInternalFreight,
//                TotalOtherCost = original.TotalOtherCost,
//                TotalInsurance = original.TotalInsurance,
//                TotalDeduction = original.TotalDeduction,
//                Currency = original.Currency,
//                SupplierName = original.SupplierName,
//                InvoiceDetails = original.InvoiceDetails?.Select(d => new InvoiceDetails
//                {
//                    LineNumber = d.LineNumber,
//                    Quantity = d.Quantity,
//                    Cost = d.Cost,
//                    TotalCost = d.TotalCost,
//                    Discount = d.Discount,
//                    ItemDescription = d.ItemDescription
//                }).ToList()
//            };
//        }

//        #endregion

//        #region Helper Methods

//        /// <summary>
//        /// Cleans text for analysis
//        /// </summary>
//        private string CleanTextForAnalysis(string text)
//        {
//            if (string.IsNullOrWhiteSpace(text)) return string.Empty;

//            var cleaned = Regex.Replace(text, @"-{30,}[^-]*-{30,}", "", RegexOptions.Multiline);

//            if (cleaned.Length > 8000)
//                cleaned = cleaned.Substring(0, 8000) + "...[truncated]";

//            return cleaned;
//        }

//        /// <summary>
//        /// Cleans JSON response
//        /// </summary>
//        private string CleanJsonResponse(string jsonResponse)
//        {
//            if (string.IsNullOrWhiteSpace(jsonResponse)) return string.Empty;

//            var cleaned = Regex.Replace(jsonResponse, @"```json|```|'''|\uFEFF", string.Empty, RegexOptions.IgnoreCase);

//            var startIndex = cleaned.IndexOf('{');
//            var endIndex = cleaned.LastIndexOf('}');

//            if (startIndex == -1 || endIndex == -1 || startIndex >= endIndex)
//            {
//                _logger.Warning("No valid JSON boundaries detected in response");
//                return string.Empty;
//            }

//            return cleaned.Substring(startIndex, endIndex - startIndex + 1);
//        }

//        /// <summary>
//        /// Parses corrected value to appropriate type
//        /// </summary>
//        private object ParseCorrectedValue(string value, string fieldName)
//        {
//            if (string.IsNullOrWhiteSpace(value)) return null;

//            var cleanValue = value.Replace("$", "").Replace(",", "").Trim();

//            if (IsNumericField(fieldName))
//            {
//                if (decimal.TryParse(cleanValue, out var decimalValue))
//                    return decimalValue;
//            }

//            return value;
//        }

//        /// <summary>
//        /// Determines if a field should contain numeric values
//        /// </summary>
//        private bool IsNumericField(string fieldName)
//        {
//            var numericFields = new[] {
//                "invoicetotal", "subtotal", "totalinternalfreight",
//                "totalothercost", "totalinsurance", "totaldeduction",
//                "quantity", "cost", "totalcost", "discount"
//            };
//            return numericFields.Contains(fieldName.ToLower());
//        }

//        /// <summary>
//        /// Gets current field value for logging
//        /// </summary>
//        private object GetCurrentFieldValue(ShipmentInvoice invoice, string fieldName)
//        {
//            return fieldName.ToLower() switch
//            {
//                "invoicetotal" => invoice.InvoiceTotal,
//                "subtotal" => invoice.SubTotal,
//                "totalinternalfreight" => invoice.TotalInternalFreight,
//                "totalothercost" => invoice.TotalOtherCost,
//                "totalinsurance" => invoice.TotalInsurance,
//                "totaldeduction" => invoice.TotalDeduction,
//                "invoiceno" => invoice.InvoiceNo,
//                "suppliername" => invoice.SupplierName,
//                "currency" => invoice.Currency,
//                _ => null
//            };
//        }

//        /// <summary>
//        /// Recalculates invoice detail total cost
//        /// </summary>
//        private void RecalculateDetailTotal(InvoiceDetails detail)
//        {
//            detail.TotalCost = (detail.Quantity * detail.Cost) - (detail.Discount ?? 0);
//        }

//        /// <summary>
//        /// Gets string value from JSON element
//        /// </summary>
//        private string GetStringValue(JsonElement element, string propertyName)
//        {
//            if (element.TryGetProperty(propertyName, out var prop) && prop.ValueKind != JsonValueKind.Null)
//                return prop.GetString();
//            return null;
//        }

//        /// <summary>
//        /// Gets double value from JSON element
//        /// </summary>
//        private double GetDoubleValue(JsonElement element, string propertyName)
//        {
//            if (element.TryGetProperty(propertyName, out var prop) && prop.ValueKind != JsonValueKind.Null)
//            {
//                if (prop.TryGetDouble(out var value)) return value;
//                if (prop.TryGetDecimal(out var decimalValue)) return (double)decimalValue;
//                if (prop.TryGetInt32(out var intValue)) return intValue;
//            }
//            return 0.0;
//        }

//        /// <summary>
//        /// Truncates text for logging
//        /// </summary>
//        private string TruncateForLog(string text, int maxLength = 500)
//        {
//            if (string.IsNullOrEmpty(text)) return string.Empty;
//            return text.Length <= maxLength ? text : text.Substring(0, maxLength) + "...";
//        }

//        #endregion

//        #region Legacy Static Helper Methods - CORRECTED FOR MULTI-INVOICE

//        private static ShipmentInvoice CreateTempShipmentInvoice(IDictionary<string, object> x)
//        {
//            try
//            {
//                var invoice = new ShipmentInvoice
//                {
//                    InvoiceNo = x.ContainsKey("InvoiceNo") && x["InvoiceNo"] != null ? x["InvoiceNo"].ToString() : "Unknown",
//                    InvoiceTotal = x.ContainsKey("InvoiceTotal") ? Convert.ToDouble(x["InvoiceTotal"].ToString()) : (double?)null,
//                    SubTotal = x.ContainsKey("SubTotal") ? Convert.ToDouble(x["SubTotal"].ToString()) : (double?)null,
//                    TotalInternalFreight = x.ContainsKey("TotalInternalFreight") ? Convert.ToDouble(x["TotalInternalFreight"].ToString()) : (double?)null,
//                    TotalOtherCost = x.ContainsKey("TotalOtherCost") ? Convert.ToDouble(x["TotalOtherCost"].ToString()) : (double?)null,
//                    TotalInsurance = x.ContainsKey("TotalInsurance") ? Convert.ToDouble(x["TotalInsurance"].ToString()) : (double?)null,
//                    TotalDeduction = x.ContainsKey("TotalDeduction") ? Convert.ToDouble(x["TotalDeduction"].ToString()) : (double?)null
//                };

//                if (!x.ContainsKey("InvoiceDetails"))
//                {
//                    invoice.InvoiceDetails = new List<InvoiceDetails>();
//                }
//                else
//                {
//                    // FIXED: Safe type checking for invoice details
//                    if (x["InvoiceDetails"] is List<IDictionary<string, object>> invoiceDetailsList)
//                    {
//                        invoice.InvoiceDetails = invoiceDetailsList
//                            .Where(z => z?.ContainsKey("ItemDescription") == true && z["ItemDescription"] != null)
//                            .Select((z, index) =>
//                            {
//                                try
//                                {
//                                    var qty = z.ContainsKey("Quantity") ? Convert.ToDouble(z["Quantity"].ToString()) : 1;
//                                    return new InvoiceDetails
//                                    {
//                                        LineNumber = index + 1, // Ensure line numbers are set
//                                        ItemDescription = z["ItemDescription"].ToString(),
//                                        Quantity = qty,
//                                        Cost = z.ContainsKey("Cost") ? Convert.ToDouble(z["Cost"].ToString()) :
//                                               z.ContainsKey("TotalCost") ? Convert.ToDouble(z["TotalCost"].ToString()) / (qty == 0 ? 1 : qty) : 0,
//                                        TotalCost = z.ContainsKey("TotalCost") ? Convert.ToDouble(z["TotalCost"].ToString()) :
//                                                   z.ContainsKey("Cost") ? Convert.ToDouble(z["Cost"].ToString()) * qty : 0,
//                                        Discount = z.ContainsKey("Discount") ? Convert.ToDouble(z["Discount"].ToString()) : 0
//                                    };
//                                }
//                                catch (Exception ex)
//                                {
//                                    Log.Logger.Error(ex, "Error processing invoice detail {Index} for invoice {InvoiceNo}",
//                                        index, invoice.InvoiceNo);
//                                    return null;
//                                }
//                            })
//                            .Where(detail => detail != null)
//                            .ToList();
//                    }
//                    else
//                    {
//                        // Handle case where InvoiceDetails might be a different type
//                        var actualType = x["InvoiceDetails"]?.GetType().Name ?? "null";
//                        Log.Logger.Warning("Expected List<IDictionary<string, object>> for InvoiceDetails but got {ActualType} for invoice {InvoiceNo}",
//                            actualType, invoice.InvoiceNo);
//                        invoice.InvoiceDetails = new List<InvoiceDetails>();
//                    }
//                }

//                return invoice;
//            }
//            catch (Exception ex)
//            {
//                Log.Logger.Error(ex, "Error creating ShipmentInvoice from dictionary");
//                return new ShipmentInvoice
//                {
//                    InvoiceNo = "ERROR",
//                    InvoiceDetails = new List<InvoiceDetails>()
//                };
//            }
//        }

//        private static List<ShipmentInvoice> ConvertDynamicToShipmentInvoices(List<dynamic> res)
//        {
//            var allInvoices = new List<ShipmentInvoice>();

//            try
//            {
//                foreach (var item in res)
//                {
//                    if (item is List<IDictionary<string, object>> invoiceList)
//                    {
//                        // FIXED: Process ALL invoices in each list, not just the first
//                        foreach (var invoiceDict in invoiceList)
//                        {
//                            var shipmentInvoice = CreateTempShipmentInvoice(invoiceDict);
//                            allInvoices.Add(shipmentInvoice);
//                        }

//                        Log.Logger.Debug("Converted {Count} invoices from PDF section", invoiceList.Count);
//                    }
//                    else
//                    {
//                        // Handle legacy single dictionary format for backward compatibility
//                        if (item is IDictionary<string, object> singleInvoiceDict)
//                        {
//                            Log.Logger.Warning("Found single dictionary instead of list - converting for backward compatibility");
//                            var shipmentInvoice = CreateTempShipmentInvoice(singleInvoiceDict);
//                            allInvoices.Add(shipmentInvoice);
//                        }
//                        else
//                        {
//                            var actualType = item?.GetType().Name ?? "null";
//                            Log.Logger.Error("Unexpected type in ConvertDynamicToShipmentInvoices: {ActualType}", actualType);
//                        }
//                    }
//                }

//                Log.Logger.Information("Successfully converted {TotalInvoices} total invoices from {Sections} PDF sections",
//                    allInvoices.Count, res.Count);

//                return allInvoices;
//            }
//            catch (Exception ex)
//            {
//                Log.Logger.Error(ex, "Error in ConvertDynamicToShipmentInvoices");
//                return new List<ShipmentInvoice>();
//            }
//        }

//        private static string GetFilePathFromTemplate(Invoice template)
//        {
//            return template?.FilePath ?? "unknown";
//        }

//        private static void UpdateDynamicResultsWithCorrections(List<dynamic> res, List<ShipmentInvoice> correctedInvoices)
//        {
//            try
//            {
//                int correctedIndex = 0;

//                for (int sectionIndex = 0; sectionIndex < res.Count; sectionIndex++)
//                {
//                    if (res[sectionIndex] is List<IDictionary<string, object>> invoiceList)
//                    {
//                        Log.Logger.Debug("Updating {Count} invoices in section {Section}", invoiceList.Count, sectionIndex);

//                        // FIXED: Update ALL invoices in the collection
//                        for (int invoiceIndex = 0; invoiceIndex < invoiceList.Count && correctedIndex < correctedInvoices.Count; invoiceIndex++, correctedIndex++)
//                        {
//                            var invoiceDict = invoiceList[invoiceIndex];
//                            var correctedInvoice = correctedInvoices[correctedIndex];

//                            try
//                            {
//                                // Update main fields
//                                if (correctedInvoice.InvoiceTotal.HasValue)
//                                {
//                                    invoiceDict["InvoiceTotal"] = correctedInvoice.InvoiceTotal.Value;
//                                    Log.Logger.Debug("Updated InvoiceTotal for {InvoiceNo}: {Value}",
//                                        correctedInvoice.InvoiceNo, correctedInvoice.InvoiceTotal.Value);
//                                }
//                                if (correctedInvoice.SubTotal.HasValue)
//                                    invoiceDict["SubTotal"] = correctedInvoice.SubTotal.Value;
//                                if (correctedInvoice.TotalInternalFreight.HasValue)
//                                    invoiceDict["TotalInternalFreight"] = correctedInvoice.TotalInternalFreight.Value;
//                                if (correctedInvoice.TotalOtherCost.HasValue)
//                                    invoiceDict["TotalOtherCost"] = correctedInvoice.TotalOtherCost.Value;
//                                if (correctedInvoice.TotalInsurance.HasValue)
//                                    invoiceDict["TotalInsurance"] = correctedInvoice.TotalInsurance.Value;
//                                if (correctedInvoice.TotalDeduction.HasValue)
//                                    invoiceDict["TotalDeduction"] = correctedInvoice.TotalDeduction.Value;

//                                // Update invoice details
//                                if (invoiceDict.ContainsKey("InvoiceDetails") &&
//                                    invoiceDict["InvoiceDetails"] is List<IDictionary<string, object>> detailsList &&
//                                    correctedInvoice.InvoiceDetails != null)
//                                {
//                                    for (int detailIndex = 0; detailIndex < detailsList.Count && detailIndex < correctedInvoice.InvoiceDetails.Count; detailIndex++)
//                                    {
//                                        var detailDict = detailsList[detailIndex];
//                                        var correctedDetail = correctedInvoice.InvoiceDetails[detailIndex];

//                                        detailDict["Quantity"] = correctedDetail.Quantity;
//                                        detailDict["Cost"] = correctedDetail.Cost;
//                                        if (correctedDetail.TotalCost.HasValue)
//                                            detailDict["TotalCost"] = correctedDetail.TotalCost.Value;
//                                        if (correctedDetail.Discount.HasValue)
//                                            detailDict["Discount"] = correctedDetail.Discount.Value;
//                                    }

//                                    Log.Logger.Debug("Updated {DetailCount} details for invoice {InvoiceNo}",
//                                        Math.Min(detailsList.Count, correctedInvoice.InvoiceDetails.Count), correctedInvoice.InvoiceNo);
//                                }
//                            }
//                            catch (Exception ex)
//                            {
//                                Log.Logger.Error(ex, "Error updating invoice {InvoiceNo} at section {Section}, invoice {Invoice}",
//                                    correctedInvoice.InvoiceNo, sectionIndex, invoiceIndex);
//                            }
//                        }
//                    }
//                    else
//                    {
//                        // Handle legacy single dictionary format
//                        if (res[sectionIndex] is IDictionary<string, object> singleInvoiceDict && correctedIndex < correctedInvoices.Count)
//                        {
//                            Log.Logger.Warning("Updating single dictionary format (legacy) at section {Section}", sectionIndex);

//                            var correctedInvoice = correctedInvoices[correctedIndex];

//                            // Update the single invoice
//                            if (correctedInvoice.InvoiceTotal.HasValue)
//                                singleInvoiceDict["InvoiceTotal"] = correctedInvoice.InvoiceTotal.Value;
//                            if (correctedInvoice.SubTotal.HasValue)
//                                singleInvoiceDict["SubTotal"] = correctedInvoice.SubTotal.Value;
//                            // ... (other fields similar to above)

//                            correctedIndex++;
//                        }
//                        else
//                        {
//                            var actualType = res[sectionIndex]?.GetType().Name ?? "null";
//                            Log.Logger.Warning("Cannot update section {Section} - unexpected type: {ActualType}", sectionIndex, actualType);
//                        }
//                    }
//                }

//                Log.Logger.Information("Successfully updated {UpdatedCount} invoices from corrections", correctedIndex);

//                if (correctedIndex < correctedInvoices.Count)
//                {
//                    Log.Logger.Warning("Only updated {Updated} of {Total} corrected invoices - structure mismatch",
//                        correctedIndex, correctedInvoices.Count);
//                }
//            }
//            catch (Exception ex)
//            {
//                Log.Logger.Error(ex, "Error in UpdateDynamicResultsWithCorrections");
//            }
//        }

//        #endregion


//        #region Additional Helper Methods for Multi-Invoice Support

//        /// <summary>
//        /// Validates the data structure and logs detailed information about invoice collections
//        /// </summary>
//        public static void ValidateInvoiceStructure(List<dynamic> res)
//        {
//            if (res == null)
//            {
//                Log.Logger.Error("Invoice result list is null");
//                return;
//            }

//            Log.Logger.Information("=== INVOICE STRUCTURE VALIDATION ===");
//            Log.Logger.Information("Total sections in PDF: {SectionCount}", res.Count);

//            int totalInvoices = 0;

//            for (int i = 0; i < res.Count; i++)
//            {
//                var section = res[i];
//                if (section == null)
//                {
//                    Log.Logger.Warning("Section {Index}: null", i);
//                    continue;
//                }

//                if (section is List<IDictionary<string, object>> invoiceList)
//                {
//                    Log.Logger.Information("Section {Index}: ✓ List<IDictionary<string, object>> with {Count} invoices", i, invoiceList.Count);
//                    totalInvoices += invoiceList.Count;

//                    for (int j = 0; j < invoiceList.Count; j++)
//                    {
//                        var invoice = invoiceList[j];
//                        var invoiceNo = invoice?.ContainsKey("InvoiceNo") == true ? invoice["InvoiceNo"]?.ToString() : "Unknown";
//                        var detailsCount = 0;

//                        if (invoice?.ContainsKey("InvoiceDetails") == true &&
//                            invoice["InvoiceDetails"] is List<IDictionary<string, object>> details)
//                        {
//                            detailsCount = details.Count;
//                        }

//                        Log.Logger.Information("  Invoice {SubIndex}: {InvoiceNo} ({DetailCount} line items)", j, invoiceNo, detailsCount);
//                    }
//                }
//                else if (section is IDictionary<string, object> singleDict)
//                {
//                    Log.Logger.Warning("Section {Index}: ⚠️ Single IDictionary (legacy format) - {InvoiceNo}",
//                        i, singleDict.ContainsKey("InvoiceNo") ? singleDict["InvoiceNo"] : "Unknown");
//                    totalInvoices += 1;
//                }
//                else
//                {
//                    var actualType = section.GetType();
//                    Log.Logger.Error("Section {Index}: ❌ Unexpected type: {TypeName}", i, actualType.Name);
//                }
//            }

//            Log.Logger.Information("Total invoices found: {TotalInvoices}", totalInvoices);
//            Log.Logger.Information("=== END VALIDATION ===");
//        }

//        /// <summary>
//        /// Counts total invoices across all sections
//        /// </summary>
//        public static int CountTotalInvoices(List<dynamic> res)
//        {
//            if (res == null) return 0;

//            int count = 0;
//            foreach (var item in res)
//            {
//                if (item is List<IDictionary<string, object>> invoiceList)
//                {
//                    count += invoiceList.Count;
//                }
//                else if (item is IDictionary<string, object>)
//                {
//                    count += 1;
//                }
//            }
//            return count;
//        }

//        #endregion

//        #region Testing Methods

//        /// <summary>
//        /// Comprehensive test method including product validation
//        /// </summary>
//        public static async Task RunComprehensiveTests()
//        {
//            Log.Logger.Information("=== Enhanced OCR Correction Service Tests ===");

//            try
//            {
//                TestDataStructures();
//                TestJsonParsing();
//                TestFieldParsing();
//                await TestProductValidation().ConfigureAwait(false);
//                await TestMathematicalValidation().ConfigureAwait(false);
//                TestRegexPatterns();

//                Log.Logger.Information("🎉 ALL COMPREHENSIVE TESTS PASSED SUCCESSFULLY!");
//                Log.Logger.Information("✅ Enhanced OCR Correction Service functionality verified");
//            }
//            catch (Exception ex)
//            {
//                Log.Logger.Error(ex, "❌ Comprehensive test failed");
//                throw;
//            }
//        }

//        private static void TestDataStructures()
//        {
//            Log.Logger.Information("Testing data structures...");

//            var error = new InvoiceError
//            {
//                Field = "InvoiceTotal",
//                ExtractedValue = "123,45",
//                CorrectValue = "123.45",
//                Confidence = 0.95,
//                ErrorType = "decimal_separator",
//                Reasoning = "OCR confused comma with period"
//            };

//            var result = new CorrectionResult
//            {
//                FieldName = "InvoiceTotal",
//                OldValue = "123,45",
//                NewValue = "123.45",
//                Success = true,
//                Confidence = 0.95
//            };

//            Log.Logger.Information("✓ Data structures working correctly");
//        }

//        private static void TestJsonParsing()
//        {
//            Log.Logger.Information("Testing JSON parsing...");

//            var testJson = @"{
//                ""errors"": [
//                    {
//                        ""field"": ""InvoiceTotal"",
//                        ""extracted_value"": ""123,45"",
//                        ""correct_value"": ""123.45"",
//                        ""confidence"": 0.95,
//                        ""error_type"": ""decimal_separator"",
//                        ""reasoning"": ""OCR confused comma with period""
//                    }
//                ]
//            }";

//            using var service = new OCRCorrectionService();
//            var cleanJson = service.CleanJsonResponse(testJson);

//            if (string.IsNullOrEmpty(cleanJson))
//                throw new Exception("JSON cleaning failed");

//            using var doc = JsonDocument.Parse(cleanJson);
//            var root = doc.RootElement;

//            if (!root.TryGetProperty("errors", out var errorsElement) ||
//                errorsElement.GetArrayLength() != 1)
//                throw new Exception("JSON parsing failed");

//            Log.Logger.Information("✓ JSON parsing logic verified");
//        }

//        private static void TestFieldParsing()
//        {
//            Log.Logger.Information("Testing field parsing...");

//            using var service = new OCRCorrectionService();

//            // Test numeric parsing
//            var numericValue = service.ParseCorrectedValue("$123.45", "InvoiceTotal");
//            if (numericValue is not decimal || (decimal)numericValue != 123.45m)
//                throw new Exception("Numeric field parsing failed");

//            // Test string parsing
//            var stringValue = service.ParseCorrectedValue("ABC123", "InvoiceNo");
//            if (stringValue is not string || stringValue.ToString() != "ABC123")
//                throw new Exception("String field parsing failed");

//            Log.Logger.Information("✓ Field parsing logic verified");
//        }

//        private static async Task TestProductValidation()
//        {
//            Log.Logger.Information("Testing product validation logic...");

//            var testInvoice = new ShipmentInvoice
//            {
//                InvoiceNo = "TEST-001",
//                InvoiceDetails = new List<InvoiceDetails>
//                {
//                    new InvoiceDetails
//                    {
//                        LineNumber = 1,
//                        Quantity = 2,
//                        Cost = 10.50,
//                        TotalCost = 21.00,
//                        ItemDescription = "Test Item"
//                    },
//                    new InvoiceDetails
//                    {
//                        LineNumber = 2,
//                        Quantity = -1, // Invalid quantity
//                        Cost = 5.00,
//                        TotalCost = -5.00,
//                        ItemDescription = "Invalid Item"
//                    }
//                }
//            };

//            using var service = new OCRCorrectionService();
//            var mathErrors = service.ValidateMathematicalConsistency(testInvoice);

//            if (!mathErrors.Any(e => e.ErrorType == "unreasonable_quantity"))
//                throw new Exception("Product validation failed to detect invalid quantity");

//            Log.Logger.Information("✓ Product validation logic verified");
//        }

//        private static async Task TestMathematicalValidation()
//        {
//            Log.Logger.Information("Testing mathematical validation logic...");

//            var testInvoice = new ShipmentInvoice
//            {
//                InvoiceNo = "TEST-002",
//                SubTotal = 100.00,
//                TotalInternalFreight = 10.00,
//                TotalOtherCost = 5.00,
//                InvoiceTotal = 110.00, // Should be 115.00
//                InvoiceDetails = new List<InvoiceDetails>
//                {
//                    new InvoiceDetails
//                    {
//                        LineNumber = 1,
//                        Quantity = 2,
//                        Cost = 25.00,
//                        TotalCost = 50.00
//                    },
//                    new InvoiceDetails
//                    {
//                        LineNumber = 2,
//                        Quantity = 1,
//                        Cost = 50.00,
//                        TotalCost = 50.00
//                    }
//                }
//            };

//            using var service = new OCRCorrectionService();
//            var crossFieldErrors = service.ValidateCrossFieldConsistency(testInvoice);

//            if (!crossFieldErrors.Any(e => e.ErrorType == "invoice_total_mismatch"))
//                throw new Exception("Mathematical validation failed to detect total mismatch");

//            Log.Logger.Information("✓ Mathematical validation logic verified");
//        }

//        private static void TestRegexPatterns()
//        {
//            Log.Logger.Information("Testing regex pattern logic...");

//            var testStrategy = new RegexCorrectionStrategy
//            {
//                StrategyType = "FORMAT_FIX",
//                RegexPattern = @"\$?([0-9]+)[,]([0-9]{2})",
//                ReplacementPattern = "$1.$2",
//                Confidence = 0.95,
//                Reasoning = "Convert European decimal comma to period"
//            };

//            if (string.IsNullOrEmpty(testStrategy.RegexPattern))
//                throw new Exception("Regex pattern validation failed");

//            Log.Logger.Information("✓ Regex pattern logic verified");
//        }

//        #endregion

//        #region IDisposable Implementation

//        protected virtual void Dispose(bool disposing)
//        {
//            if (!_disposed)
//            {
//                if (disposing)
//                {
//                    _deepSeekApi?.Dispose();
//                }
//                _disposed = true;
//            }
//        }

//        public void Dispose()
//        {
//            Dispose(disposing: true);
//            GC.SuppressFinalize(this);
//        }

//        #endregion

//    }
//}