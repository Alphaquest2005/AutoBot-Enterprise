// File: OCRCorrectionService/OCRValidation.cs
using System;
using System.Collections.Generic;
using System.Linq;
using EntryDataDS.Business.Entities;

namespace WaterNut.DataSpace
{
    public partial class OCRCorrectionService
    {
        #region Validation Methods

        /// <summary>
        /// Validates mathematical consistency within the invoice
        /// </summary>
        private List<InvoiceError> ValidateMathematicalConsistency(ShipmentInvoice invoice)
        {
            var errors = new List<InvoiceError>();

            try
            {
                // Validate individual line item calculations
                if (invoice.InvoiceDetails != null)
                {
                    foreach (var detail in invoice.InvoiceDetails)
                    {
                        var calculatedTotal = (detail.Quantity * detail.Cost) - (detail.Discount ?? 0);
                        var reportedTotal = detail.TotalCost ?? 0;

                        if (Math.Abs(calculatedTotal - reportedTotal) > 0.01)
                        {
                            errors.Add(new InvoiceError
                            {
                                Field = $"InvoiceDetail_Line{detail.LineNumber}_TotalCost",
                                ExtractedValue = reportedTotal.ToString("F2"),
                                CorrectValue = calculatedTotal.ToString("F2"),
                                Confidence = 0.99,
                                ErrorType = "calculation_error",
                                Reasoning = $"Line total should be (Qty {detail.Quantity} × Cost {detail.Cost:F2}) - Discount {detail.Discount ?? 0:F2} = {calculatedTotal:F2}"
                            });
                        }

                        // Validate reasonable quantities
                        if (detail.Quantity <= 0 || detail.Quantity > 10000)
                        {
                            errors.Add(new InvoiceError
                            {
                                Field = $"InvoiceDetail_Line{detail.LineNumber}_Quantity",
                                ExtractedValue = detail.Quantity.ToString(),
                                CorrectValue = "1",
                                Confidence = 0.7,
                                ErrorType = "unreasonable_quantity",
                                Reasoning = $"Quantity {detail.Quantity} seems unreasonable"
                            });
                        }

                        // Validate reasonable unit costs
                        if (detail.Cost <= 0)
                        {
                            errors.Add(new InvoiceError
                            {
                                Field = $"InvoiceDetail_Line{detail.LineNumber}_Cost",
                                ExtractedValue = detail.Cost.ToString("F2"),
                                CorrectValue = "0.01",
                                Confidence = 0.8,
                                ErrorType = "unreasonable_cost",
                                Reasoning = $"Unit cost {detail.Cost:F2} is negative or zero"
                            });
                        }
                    }
                }

                return errors;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error validating mathematical consistency");
                return errors;
            }
        }

        /// <summary>
        /// Enhanced cross-field validation that properly validates invoice totals
        /// </summary>
        private List<InvoiceError> ValidateCrossFieldConsistency(ShipmentInvoice invoice)
        {
            var errors = new List<InvoiceError>();

            try
            {
                // Check if detail totals match subtotal
                if (invoice.InvoiceDetails?.Any() == true)
                {
                    var calculatedSubTotal = invoice.InvoiceDetails.Sum(d => d.TotalCost ?? 0);
                    var reportedSubTotal = invoice.SubTotal ?? 0;

                    if (Math.Abs(calculatedSubTotal - reportedSubTotal) > 0.01)
                    {
                        errors.Add(new InvoiceError
                        {
                            Field = "SubTotal",
                            ExtractedValue = reportedSubTotal.ToString("F2"),
                            CorrectValue = calculatedSubTotal.ToString("F2"),
                            Confidence = 0.95,
                            ErrorType = "subtotal_mismatch",
                            Reasoning = $"SubTotal should equal sum of line items: {calculatedSubTotal:F2}"
                        });
                    }
                }

                // FIXED: Enhanced invoice total validation with gift card handling
                var baseTotal = (invoice.SubTotal ?? 0) +
                              (invoice.TotalInternalFreight ?? 0) +
                              (invoice.TotalOtherCost ?? 0) +
                              (invoice.TotalInsurance ?? 0);

                var deductionAmount = invoice.TotalDeduction ?? 0;
                var reportedInvoiceTotal = invoice.InvoiceTotal ?? 0;

                // Check if the invoice total already has deductions applied (like Amazon gift cards)
                var calculatedWithDeduction = baseTotal - deductionAmount;  // Standard formula
                var calculatedWithoutDeduction = baseTotal;                 // Total before deductions

                var diffWithDeduction = Math.Abs(calculatedWithDeduction - reportedInvoiceTotal);
                var diffWithoutDeduction = Math.Abs(calculatedWithoutDeduction - reportedInvoiceTotal);

                // Use the calculation that's closer to the reported total
                var calculatedInvoiceTotal = diffWithDeduction <= diffWithoutDeduction ?
                    calculatedWithDeduction : calculatedWithoutDeduction;
                var difference = Math.Min(diffWithDeduction, diffWithoutDeduction);

                // Log the analysis for debugging
                _logger.Debug("Invoice total analysis for {InvoiceNo}: BaseTotal={BaseTotal}, " +
                            "WithDeduction={WithDeduction}, WithoutDeduction={WithoutDeduction}, " +
                            "Reported={Reported}, DiffWith={DiffWith}, DiffWithout={DiffWithout}",
                    invoice.InvoiceNo, baseTotal, calculatedWithDeduction, calculatedWithoutDeduction,
                    reportedInvoiceTotal, diffWithDeduction, diffWithoutDeduction);

                _logger.Information("Invoice Total Validation for {InvoiceNo}:", invoice.InvoiceNo);
                _logger.Information("  SubTotal: {SubTotal:F2}", invoice.SubTotal ?? 0);
                _logger.Information("  TotalInternalFreight: {Freight:F2}", invoice.TotalInternalFreight ?? 0);
                _logger.Information("  TotalOtherCost: {OtherCost:F2}", invoice.TotalOtherCost ?? 0);
                _logger.Information("  TotalInsurance: {Insurance:F2}", invoice.TotalInsurance ?? 0);
                _logger.Information("  TotalDeduction: {Deduction:F2}", invoice.TotalDeduction ?? 0);
                _logger.Information("  Calculated Total: {Calculated:F2}", calculatedInvoiceTotal);
                _logger.Information("  Reported Total: {Reported:F2}", reportedInvoiceTotal);
                _logger.Information("  Difference: {Difference:F4}", difference);

                // Only flag as error if difference is significant (> $0.01)
                if (difference > 0.01)
                {
                    errors.Add(new InvoiceError
                    {
                        Field = "InvoiceTotal",
                        ExtractedValue = reportedInvoiceTotal.ToString("F2"),
                        CorrectValue = calculatedInvoiceTotal.ToString("F2"),
                        Confidence = 0.95,
                        ErrorType = "invoice_total_mismatch",
                        Reasoning = $"Invoice total calculation: {invoice.SubTotal:F2} + {invoice.TotalInternalFreight:F2} + {invoice.TotalOtherCost:F2} + {invoice.TotalInsurance:F2} - {invoice.TotalDeduction:F2} = {calculatedInvoiceTotal:F2}"
                    });

                    _logger.Warning("Invoice total mismatch detected for {InvoiceNo}: Expected {Expected:F2}, Got {Actual:F2}",
                        invoice.InvoiceNo, calculatedInvoiceTotal, reportedInvoiceTotal);
                }
                else
                {
                    _logger.Information("Invoice total validation PASSED for {InvoiceNo} (difference: {Difference:F4})",
                        invoice.InvoiceNo, difference);
                }

                return errors;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error validating cross-field consistency for invoice {InvoiceNo}", invoice.InvoiceNo);
                return errors;
            }
        }

        /// <summary>
        /// Resolves field conflicts using dependency validation
        /// </summary>
        private List<InvoiceError> ResolveFieldConflicts(List<InvoiceError> errors, ShipmentInvoice invoice)
        {
            var filteredErrors = new List<InvoiceError>();

            // Group errors by field to detect conflicts
            var errorGroups = errors.GroupBy(e => e.Field?.ToLower()).ToList();

            foreach (var group in errorGroups)
            {
                if (group.Count() == 1)
                {
                    // No conflict, add the error
                    filteredErrors.Add(group.First());
                }
                else
                {
                    // Multiple corrections for same field - choose highest confidence
                    var bestError = group.OrderByDescending(e => e.Confidence).First();
                    filteredErrors.Add(bestError);

                    _logger.Warning("Resolved field conflict for {Field}: chose correction with confidence {Confidence:P0} over {AlternativeCount} alternatives",
                        bestError.Field, bestError.Confidence, group.Count() - 1);
                }
            }

            // Apply mathematical validation to detect conflicting corrections
            var mathematicallyValidErrors = ValidateMathematicalConsistency(filteredErrors, invoice);

            return mathematicallyValidErrors;
        }

        /// <summary>
        /// Validates mathematical consistency of proposed corrections
        /// </summary>
        private List<InvoiceError> ValidateMathematicalConsistency(List<InvoiceError> errors, ShipmentInvoice invoice)
        {
            var validErrors = new List<InvoiceError>();

            // Create a copy of the invoice to test corrections
            var testInvoice = CloneInvoiceForTesting(invoice);

            foreach (var error in errors)
            {
                // Apply the correction to test invoice
                var correctedValue = ParseCorrectedValue(error.CorrectValue, error.Field);
                if (correctedValue != null && ApplyFieldCorrection(testInvoice, error.Field, correctedValue))
                {
                    // Check if the correction improves mathematical consistency
                    var beforeTotalsZero = TotalsZero(invoice, _logger);
                    var afterTotalsZero = TotalsZero(testInvoice, _logger);

                    if (afterTotalsZero || (!beforeTotalsZero && afterTotalsZero))
                    {
                        validErrors.Add(error);
                        _logger.Debug("Correction for {Field} improves mathematical consistency", error.Field);
                    }
                    else
                    {
                        _logger.Warning("Correction for {Field} does not improve mathematical consistency, reducing confidence",
                            error.Field);

                        // Reduce confidence but still include if above threshold
                        error.Confidence *= 0.6; // Reduce by 40%
                        if (error.Confidence > 0.3)
                        {
                            validErrors.Add(error);
                        }
                    }
                }
                else
                {
                    _logger.Warning("Could not apply correction for {Field} during validation", error.Field);
                }
            }

            return validErrors;
        }

        /// <summary>
        /// Creates a copy of invoice for testing corrections
        /// </summary>
        private ShipmentInvoice CloneInvoiceForTesting(ShipmentInvoice original)
        {
            return new ShipmentInvoice
            {
                InvoiceNo = original.InvoiceNo,
                InvoiceTotal = original.InvoiceTotal,
                SubTotal = original.SubTotal,
                TotalInternalFreight = original.TotalInternalFreight,
                TotalOtherCost = original.TotalOtherCost,
                TotalInsurance = original.TotalInsurance,
                TotalDeduction = original.TotalDeduction,
                Currency = original.Currency,
                SupplierName = original.SupplierName,
                InvoiceDetails = original.InvoiceDetails?.Select(d => new InvoiceDetails
                {
                    LineNumber = d.LineNumber,
                    Quantity = d.Quantity,
                    Cost = d.Cost,
                    TotalCost = d.TotalCost,
                    Discount = d.Discount,
                    ItemDescription = d.ItemDescription
                }).ToList()
            };
        }

        #endregion
    }
}