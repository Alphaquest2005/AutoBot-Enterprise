// File: OCRCorrectionService/OCRValidation.cs
using System;
using System.Collections.Generic;
using System.Linq;
using EntryDataDS.Business.Entities; // For ShipmentInvoice, InvoiceDetails
using Serilog; // ILogger is available as this._logger

namespace WaterNut.DataSpace
{
    public partial class OCRCorrectionService
    {
        #region Invoice Data Validation Methods

        /// <summary>
        /// Validates mathematical consistency within the invoice.
        /// Checks line item totals (Quantity * Cost - Discount = TotalCost).
        /// Also performs basic reasonableness checks on quantities and costs.
        /// </summary>
        /// <param name="invoice">The ShipmentInvoice to validate.</param>
        /// <returns>A list of InvoiceError objects for any detected inconsistencies.</returns>
        private List<InvoiceError> ValidateMathematicalConsistency(ShipmentInvoice invoice)
        {
            var errors = new List<InvoiceError>();
            if (invoice == null)
            {
                _logger.Warning("ValidateMathematicalConsistency: Null invoice provided.");
                return errors;
            }

            _logger.Debug("Validating mathematical consistency for invoice {InvoiceNo}.", invoice.InvoiceNo);

            try
            {
                if (invoice.InvoiceDetails != null)
                {
                    foreach (var detail in invoice.InvoiceDetails.Where(d => d != null)) // Ensure detail item is not null
                    {
                        // Ensure necessary values are present for calculation
                        double quantity = detail.Quantity; // Assuming Quantity is double, not nullable
                        double unitCost = detail.Cost;   // Assuming Cost is double, not nullable
                        double discount = detail.Discount ?? 0;
                        double reportedLineTotal = detail.TotalCost ?? 0; // Handle nullable TotalCost

                        // Avoid issues with extremely small or zero quantities if cost is significant
                        double calculatedLineTotal;
                        if (quantity == 0 && unitCost != 0) {
                             calculatedLineTotal = -discount; // If qty is 0, total is just negative discount
                        } else {
                             calculatedLineTotal = (quantity * unitCost) - discount;
                        }

                        if (Math.Abs(calculatedLineTotal - reportedLineTotal) > 0.015) // Relaxed tolerance for floating point
                        {
                            errors.Add(new InvoiceError {
                                Field = $"InvoiceDetail_Line{detail.LineNumber}_TotalCost",
                                ExtractedValue = reportedLineTotal.ToString("F2"), // Format for consistency
                                CorrectValue = calculatedLineTotal.ToString("F2"),
                                Confidence = 0.99, // High confidence as it's a direct calculation
                                ErrorType = "calculation_error",
                                Reasoning = $"Line total {reportedLineTotal:F2} mismatch. Expected (Qty {quantity:F2} * Cost {unitCost:F2}) - Discount {discount:F2} = {calculatedLineTotal:F2}."
                            });
                        }

                        // Basic Reasonableness Checks
                        if (quantity < 0) // Allow quantity of 0 for some scenarios (e.g. informational line)
                        {
                             errors.Add(new InvoiceError {
                                Field = $"InvoiceDetail_Line{detail.LineNumber}_Quantity", ExtractedValue = quantity.ToString("F2"),
                                CorrectValue = "0", // Suggest 0 if negative
                                Confidence = 0.75, ErrorType = "unreasonable_value", Reasoning = $"Quantity {quantity:F2} is negative."
                            });
                        }
                        if (quantity > 999999) // Arbitrary upper limit for "unreasonable"
                        {
                             errors.Add(new InvoiceError {
                                Field = $"InvoiceDetail_Line{detail.LineNumber}_Quantity", ExtractedValue = quantity.ToString("F2"),
                                CorrectValue = "1", // Hard to suggest a generic correct value
                                Confidence = 0.60, ErrorType = "unreasonable_value", Reasoning = $"Quantity {quantity:F2} seems excessively large."
                            });
                        }
                        if (unitCost < 0 && quantity > 0) // Negative cost only makes sense if quantity is also effectively negative (refund line)
                        {
                             errors.Add(new InvoiceError {
                                Field = $"InvoiceDetail_Line{detail.LineNumber}_Cost", ExtractedValue = unitCost.ToString("F2"),
                                CorrectValue = "0.00", Confidence = 0.80, ErrorType = "unreasonable_value",
                                Reasoning = $"Unit cost {unitCost:F2} is negative for a positive quantity."
                            });
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error validating mathematical consistency for invoice {InvoiceNo}", invoice.InvoiceNo);
            }
            return errors;
        }

        /// <summary>
        /// Validates consistency between summary fields (SubTotal, InvoiceTotal) and their derived components.
        /// Uses the static OCRCorrectionService.TotalsZero method (from OCRLegacySupport.cs) as the canonical check for overall balance.
        /// </summary>
        /// <param name="invoice">The ShipmentInvoice to validate.</param>
        /// <returns>A list of InvoiceError objects for any detected inconsistencies.</returns>
        private List<InvoiceError> ValidateCrossFieldConsistency(ShipmentInvoice invoice)
        {
            var errors = new List<InvoiceError>();
            if (invoice == null)
            {
                _logger.Warning("ValidateCrossFieldConsistency: Null invoice provided.");
                return errors;
            }
            _logger.Debug("Validating cross-field consistency for invoice {InvoiceNo}.", invoice.InvoiceNo);

            try
            {
                // 1. Validate SubTotal against the sum of (corrected) InvoiceDetail.TotalCost
                if (invoice.InvoiceDetails?.Any() == true)
                {
                    var calculatedSubTotalFromDetails = invoice.InvoiceDetails.Sum(d => d?.TotalCost ?? 0);
                    var reportedSubTotal = invoice.SubTotal ?? 0;

                    if (Math.Abs(calculatedSubTotalFromDetails - reportedSubTotal) > 0.015) // Relaxed tolerance
                    {
                        errors.Add(new InvoiceError {
                            Field = "SubTotal", ExtractedValue = reportedSubTotal.ToString("F2"),
                            CorrectValue = calculatedSubTotalFromDetails.ToString("F2"), Confidence = 0.95,
                            ErrorType = "subtotal_mismatch",
                            Reasoning = $"Reported SubTotal {reportedSubTotal:F2} differs from sum of line item totals {calculatedSubTotalFromDetails:F2}."
                        });
                    }
                }

                // 2. Validate InvoiceTotal against components using the TotalsZero logic
                // This directly checks if the invoice is "balanced" as per business rule.
                if (!OCRCorrectionService.TotalsZero(invoice, _logger)) // Static method from OCRLegacySupport
                {
                    // If TotalsZero is false, it means the reported InvoiceTotal doesn't match the calculation.
                    // Let's calculate what it *should* be based on the TotalsZero logic.
                    var baseTotal = (invoice.SubTotal ?? 0) + (invoice.TotalInternalFreight ?? 0) +
                                  (invoice.TotalOtherCost ?? 0) + (invoice.TotalInsurance ?? 0);
                    var deductionAmount = invoice.TotalDeduction ?? 0;
                    var expectedInvoiceTotalBasedOnComponents = baseTotal - deductionAmount;
                    
                    errors.Add(new InvoiceError {
                        Field = "InvoiceTotal", ExtractedValue = (invoice.InvoiceTotal ?? 0).ToString("F2"),
                        CorrectValue = expectedInvoiceTotalBasedOnComponents.ToString("F2"), Confidence = 0.98, // High confidence in this rule
                        ErrorType = "invoice_total_mismatch",
                        Reasoning = $"Invoice total is unbalanced. Reported: {(invoice.InvoiceTotal ?? 0):F2}, Expected based on components: {expectedInvoiceTotalBasedOnComponents:F2} (SubT: {invoice.SubTotal ?? 0:F2} + Frght: {invoice.TotalInternalFreight ?? 0:F2} + Other: {invoice.TotalOtherCost ?? 0:F2} + Ins: {invoice.TotalInsurance ?? 0:F2} - Ded: {deductionAmount:F2})."
                    });
                     _logger.Warning("Invoice {InvoiceNo} fails TotalsZero consistency check during cross-field validation. Reported Total: {ReportedTotal}, Calculated Expected: {CalculatedExpected}", 
                        invoice.InvoiceNo, invoice.InvoiceTotal ?? 0, expectedInvoiceTotalBasedOnComponents);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error validating cross-field consistency for invoice {InvoiceNo}", invoice.InvoiceNo);
            }
            return errors;
        }

        /// <summary>
        /// Resolves conflicts if multiple error proposals exist for the same field by choosing the one with
        /// the highest confidence. Then, it validates if applying this chosen set of corrections maintains
        /// or improves mathematical consistency of the invoice.
        /// </summary>
        /// <param name="allProposedErrors">The initial list of all detected InvoiceErrors.</param>
        /// <param name="originalInvoice">The original ShipmentInvoice object before any corrections.</param>
        /// <returns>A filtered list of InvoiceError objects that are deemed most reliable and consistent.</returns>
        private List<InvoiceError> ResolveFieldConflicts(List<InvoiceError> allProposedErrors, ShipmentInvoice originalInvoice)
        {
            if (allProposedErrors == null || !allProposedErrors.Any()) return new List<InvoiceError>();

            _logger.Debug("Resolving field conflicts for {ErrorCount} proposed errors on invoice {InvoiceNo}.", allProposedErrors.Count, originalInvoice?.InvoiceNo ?? "N/A");

            // Step 1: Deduplicate by field name, preferring higher confidence.
            // Ensure field names are treated consistently (e.g., case-insensitive).
            var uniqueFieldHighestConfidenceErrors = allProposedErrors
                .GroupBy(e => (this.MapDeepSeekFieldToDatabase(e.Field)?.DatabaseFieldName ?? e.Field).ToLowerInvariant())
                .Select(g => g.OrderByDescending(e => e.Confidence).First())
                .ToList();
            
            if (uniqueFieldHighestConfidenceErrors.Count < allProposedErrors.Count)
            {
                _logger.Information("Conflict resolution: Reduced {InitialCount} proposed errors to {FilteredCount} unique field errors by highest confidence.", 
                    allProposedErrors.Count, uniqueFieldHighestConfidenceErrors.Count);
            }

            // Step 2: Validate this refined set of errors against the original invoice's mathematical balance.
            // This helps filter out corrections that might be individually high-confidence but would break overall consistency.
            var mathValidatedErrors = ValidateAndFilterCorrectionsByMathImpact(uniqueFieldHighestConfidenceErrors, originalInvoice);
            
            return mathValidatedErrors;
        }

        /// <summary>
        /// Validates a list of proposed error corrections by temporarily applying them to a clone
        /// of the original invoice and checking if they improve or maintain overall mathematical balance (TotalsZero).
        /// </summary>
        private List<InvoiceError> ValidateAndFilterCorrectionsByMathImpact(List<InvoiceError> proposedErrors, ShipmentInvoice originalInvoice)
        {
            var consistentlyValidErrors = new List<InvoiceError>();
            if (originalInvoice == null)
            {
                _logger.Warning("ValidateAndFilterCorrectionsByMathImpact: Original invoice is null, cannot validate impact. Returning all proposed errors.");
                return proposedErrors; 
            }

            bool initialTotalsAreZero = OCRCorrectionService.TotalsZero(originalInvoice, _logger); // From LegacySupport

            foreach (var error in proposedErrors)
            {
                var testInvoice = CloneInvoiceForValidation(originalInvoice); // Use dedicated clone method
                
                // Attempt to parse the CorrectValue from the error using the same logic as correction application.
                object parsedCorrectedValue = this.ParseCorrectedValue(error.CorrectValue, error.Field); // From OCRUtilities

                if (parsedCorrectedValue != null || string.IsNullOrEmpty(error.CorrectValue)) // Allow empty string if that's the correction
                {
                    // Attempt to apply the correction to the testInvoice
                    if (this.ApplyFieldCorrection(testInvoice, error.Field, parsedCorrectedValue)) // From OCRCorrectionApplication
                    {
                        bool afterCorrectionTotalsAreZero = OCRCorrectionService.TotalsZero(testInvoice, _logger);

                        if (afterCorrectionTotalsAreZero) 
                        {
                            // Good: Correction leads to a balanced state (or maintains it).
                            consistentlyValidErrors.Add(error);
                             _logger.Verbose("Validation: Correction for {Field} to '{NewVal}' is consistent (results in TotalsZero=true).", error.Field, error.CorrectValue);
                        }
                        else if (initialTotalsAreZero && !afterCorrectionTotalsAreZero) 
                        {
                            // Bad: Original was balanced, but this correction unbalances it.
                            _logger.Warning("Validation: Correction for {Field} from '{OldVal}' to '{NewVal}' would UNBALANCE an initially balanced invoice. Discarding this correction.", 
                                error.Field, error.ExtractedValue, error.CorrectValue);
                            // Optionally, could add with drastically reduced confidence if desired. For now, discard.
                        }
                        else // Original was unbalanced, and still is. Or original was balanced, and still is (but this error wasn't financial).
                        {
                            // This correction didn't make things worse regarding overall balance.
                            // More advanced logic could check if the *magnitude* of imbalance was reduced.
                            // For now, accept it if it doesn't worsen a balanced state.
                            consistentlyValidErrors.Add(error);
                             _logger.Debug("Validation: Correction for {Field} to '{NewVal}' did not worsen TotalsZero state (Initial: {InitialTZ}, After: {AfterTZ}). Keeping.", error.Field, error.CorrectValue, initialTotalsAreZero, afterCorrectionTotalsAreZero);
                        }
                    }
                    else
                    {
                         _logger.Warning("Validation: Could not apply proposed correction for {Field} to '{NewVal}' on test invoice. Retaining error proposal.", error.Field, error.CorrectValue);
                        consistentlyValidErrors.Add(error); // Keep it if application itself failed, might be structural issue in test or data.
                    }
                }
                else
                {
                     _logger.Warning("Validation: Could not parse CorrectValue '{CorrectValText}' for field {Field}. Retaining error proposal.", error.CorrectValue, error.Field);
                    consistentlyValidErrors.Add(error); // Keep, as parsing failure doesn't mean error proposal is wrong, just hard to test this way.
                }
            }
            _logger.Information("Validated {ProposedCount} proposed errors by math impact, resulting in {FinalCount} errors.", proposedErrors.Count, consistentlyValidErrors.Count);
            return consistentlyValidErrors;
        }

        /// <summary>
        /// Creates a clone of a ShipmentInvoice suitable for testing corrections without modifying the original.
        /// </summary>
        private ShipmentInvoice CloneInvoiceForValidation(ShipmentInvoice original)
        {
            // This needs to be a sufficiently deep clone for financial fields.
            var clone = new ShipmentInvoice
            {
                // Copy all relevant properties for TotalsZero and other validations
                InvoiceNo = original.InvoiceNo,
                InvoiceDate = original.InvoiceDate, // If date validation occurs
                InvoiceTotal = original.InvoiceTotal,
                SubTotal = original.SubTotal,
                TotalInternalFreight = original.TotalInternalFreight,
                TotalOtherCost = original.TotalOtherCost,
                TotalInsurance = original.TotalInsurance,
                TotalDeduction = original.TotalDeduction,
                Currency = original.Currency,
                SupplierName = original.SupplierName,
                // Do NOT copy TrackingState or ModifiedProperties
            };

            if (original.InvoiceDetails != null)
            {
                clone.InvoiceDetails = original.InvoiceDetails.Select(d => new InvoiceDetails {
                    LineNumber = d.LineNumber,
                    ItemDescription = d.ItemDescription, // If description validation occurs
                    Quantity = d.Quantity,
                    Cost = d.Cost,
                    TotalCost = d.TotalCost,
                    Discount = d.Discount,
                    Units = d.Units // If unit validation occurs
                    // Do NOT copy TrackingState or ModifiedProperties
                }).ToList();
            } else {
                clone.InvoiceDetails = new List<InvoiceDetails>();
            }
            return clone;
        }

        #endregion
    }
}