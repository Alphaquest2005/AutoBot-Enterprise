// File: OCRCorrectionService/OCRCorrectionApplication.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EntryDataDS.Business.Entities;
using TrackableEntities;

namespace WaterNut.DataSpace
{
    public partial class OCRCorrectionService
    {
        #region Correction Application

        /// <summary>
        /// Applies corrections with priority-based processing and field dependency validation
        /// </summary>
        private async Task<List<CorrectionResult>> ApplyCorrectionsAsync(
            ShipmentInvoice invoice,
            List<InvoiceError> errors,
            string fileText)
        {
            var results = new List<CorrectionResult>();

            // Apply field dependency validation and conflict resolution
            var filteredErrors = ResolveFieldConflicts(errors, invoice);

            // Group errors by priority (critical first)
            var criticalErrors = filteredErrors.Where(e => IsCriticalError(e)).ToList();
            var standardErrors = filteredErrors.Where(e => !IsCriticalError(e)).ToList();

            _logger.Information("Processing {CriticalCount} critical and {StandardCount} standard errors",
                criticalErrors.Count, standardErrors.Count);

            // Process critical errors first
            foreach (var error in criticalErrors)
            {
                var result = await this.ApplySingleCorrectionAsync(invoice, error).ConfigureAwait(false);
                results.Add(result);
                LogCorrectionResult(result, "CRITICAL");
            }

            // Process standard errors
            foreach (var error in standardErrors)
            {
                var result = await this.ApplySingleCorrectionAsync(invoice, error).ConfigureAwait(false);
                results.Add(result);
                LogCorrectionResult(result, "STANDARD");
            }

            // Recalculate dependent fields after all corrections
            RecalculateDependentFields(invoice);

            // Mark invoice as modified if any corrections were successful
            if (results.Any(r => r.Success))
            {
                invoice.TrackingState = TrackingState.Modified;
            }

            return results;
        }

        /// <summary>
        /// Applies a single correction to the invoice
        /// </summary>
        private async Task<CorrectionResult> ApplySingleCorrectionAsync(ShipmentInvoice invoice, InvoiceError error)
        {
            var result = new CorrectionResult
            {
                FieldName = error.Field,
                CorrectionType = error.ErrorType,
                Confidence = error.Confidence
            };

            try
            {
                // Parse the correct value based on field type
                var correctedValue = ParseCorrectedValue(error.CorrectValue, error.Field);
                if (correctedValue == null)
                {
                    result.Success = false;
                    result.ErrorMessage = $"Could not parse corrected value: {error.CorrectValue}";
                    return result;
                }

                // Get current value for logging
                result.OldValue = GetCurrentFieldValue(invoice, error.Field)?.ToString();

                // Apply the correction
                var applied = ApplyFieldCorrection(invoice, error.Field, correctedValue);
                if (applied)
                {
                    result.NewValue = correctedValue.ToString();
                    result.Success = true;
                }
                else
                {
                    result.Success = false;
                    result.ErrorMessage = "Field not recognized or value not applied";
                }

                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                return result;
            }
        }

        /// <summary>
        /// Applies correction to a specific field
        /// </summary>
        private bool ApplyFieldCorrection(ShipmentInvoice invoice, string fieldName, object correctedValue)
        {
            try
            {
                switch (fieldName.ToLower())
                {
                    case "invoicetotal":
                        if (correctedValue is decimal invoiceTotal)
                        {
                            invoice.InvoiceTotal = (double)invoiceTotal;
                            return true;
                        }
                        break;
                    case "subtotal":
                        if (correctedValue is decimal subTotal)
                        {
                            invoice.SubTotal = (double)subTotal;
                            return true;
                        }
                        break;
                    case "totalinternalfreight":
                        if (correctedValue is decimal freight)
                        {
                            invoice.TotalInternalFreight = (double)freight;
                            return true;
                        }
                        break;
                    case "totalothercost":
                        if (correctedValue is decimal otherCost)
                        {
                            invoice.TotalOtherCost = (double)otherCost;
                            return true;
                        }
                        break;
                    case "totalinsurance":
                        if (correctedValue is decimal insurance)
                        {
                            invoice.TotalInsurance = (double)insurance;
                            return true;
                        }
                        break;
                    case "totaldeduction":
                        if (correctedValue is decimal deduction)
                        {
                            invoice.TotalDeduction = (double)deduction;
                            return true;
                        }
                        break;
                    case "invoiceno":
                        if (correctedValue is string invoiceNo)
                        {
                            invoice.InvoiceNo = invoiceNo;
                            return true;
                        }
                        break;
                    case "suppliername":
                        if (correctedValue is string supplierName)
                        {
                            invoice.SupplierName = supplierName;
                            return true;
                        }
                        break;
                    case "currency":
                        if (correctedValue is string currency)
                        {
                            invoice.Currency = currency;
                            return true;
                        }
                        break;
                    default:
                        // Handle invoice detail corrections
                        if (fieldName.StartsWith("invoicedetail_", StringComparison.OrdinalIgnoreCase))
                        {
                            return ApplyInvoiceDetailCorrection(invoice, fieldName, correctedValue);
                        }
                        break;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error applying field correction for {FieldName}", fieldName);
                return false;
            }
        }

        /// <summary>
        /// Applies correction to invoice detail fields
        /// </summary>
        private bool ApplyInvoiceDetailCorrection(ShipmentInvoice invoice, string fieldName, object correctedValue)
        {
            try
            {
                var parts = fieldName.Split('_');
                if (parts.Length < 3) return false;

                var lineNumberStr = Regex.Replace(parts[1], "line", "", RegexOptions.IgnoreCase);
                if (!int.TryParse(lineNumberStr, out var lineNumber)) return false;

                var detailFieldName = parts[2];
                var detail = invoice.InvoiceDetails?.FirstOrDefault(d => d.LineNumber == lineNumber);
                if (detail == null) return false;

                switch (detailFieldName.ToLower())
                {
                    case "quantity":
                        if (correctedValue is decimal quantity)
                        {
                            detail.Quantity = (double)quantity;
                            RecalculateDetailTotal(detail);
                            detail.TrackingState = TrackingState.Modified;
                            return true;
                        }
                        break;
                    case "cost":
                        if (correctedValue is decimal cost)
                        {
                            detail.Cost = (double)cost;
                            RecalculateDetailTotal(detail);
                            detail.TrackingState = TrackingState.Modified;
                            return true;
                        }
                        break;
                    case "totalcost":
                        if (correctedValue is decimal totalCost)
                        {
                            detail.TotalCost = (double)totalCost;
                            detail.TrackingState = TrackingState.Modified;
                            return true;
                        }
                        break;
                    case "discount":
                        if (correctedValue is decimal discount)
                        {
                            detail.Discount = (double)discount;
                            RecalculateDetailTotal(detail);
                            detail.TrackingState = TrackingState.Modified;
                            return true;
                        }
                        break;
                }
                return false;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error applying invoice detail correction for {FieldName}", fieldName);
                return false;
            }
        }

        /// <summary>
        /// Determines if an error is critical (affects calculations)
        /// </summary>
        private bool IsCriticalError(InvoiceError error)
        {
            var criticalTypes = new[] {
                "calculation_error",
                "subtotal_mismatch",
                "invoice_total_mismatch",
                "unreasonable_quantity",
                "unreasonable_cost"
            };
            return criticalTypes.Contains(error.ErrorType);
        }

        /// <summary>
        /// Logs correction results with appropriate detail level
        /// </summary>
        private void LogCorrectionResult(CorrectionResult result, string priority)
        {
            if (result.Success)
            {
                _logger.Information("[{Priority}] Applied correction: {Field} {OldValue} → {NewValue} (confidence: {Confidence:P0})",
                    priority, result.FieldName, result.OldValue, result.NewValue, result.Confidence);
            }
            else
            {
                _logger.Warning("[{Priority}] Failed correction for {Field}: {Error}",
                    priority, result.FieldName, result.ErrorMessage);
            }
        }

        /// <summary>
        /// Recalculates dependent fields after corrections
        /// </summary>
        private void RecalculateDependentFields(ShipmentInvoice invoice)
        {
            try
            {
                // Recalculate line totals
                if (invoice.InvoiceDetails != null)
                {
                    foreach (var detail in invoice.InvoiceDetails)
                    {
                        RecalculateDetailTotal(detail);
                    }

                    // Recalculate subtotal from line items
                    var calculatedSubTotal = invoice.InvoiceDetails.Sum(d => d.TotalCost ?? 0);
                    if (Math.Abs((invoice.SubTotal ?? 0) - calculatedSubTotal) > 0.01)
                    {
                        _logger.Information("Updating SubTotal from {OldValue} to {NewValue} based on line items",
                            invoice.SubTotal, calculatedSubTotal);
                        invoice.SubTotal = calculatedSubTotal;
                    }
                }

                // Recalculate invoice total with gift card handling
                var baseTotal = (invoice.SubTotal ?? 0) +
                              (invoice.TotalInternalFreight ?? 0) +
                              (invoice.TotalOtherCost ?? 0) +
                              (invoice.TotalInsurance ?? 0);

                var deductionAmount = invoice.TotalDeduction ?? 0;
                var currentInvoiceTotal = invoice.InvoiceTotal ?? 0;

                // Check if the current total already has deductions applied
                var calculatedWithDeduction = baseTotal - deductionAmount;
                var calculatedWithoutDeduction = baseTotal;

                var diffWithDeduction = Math.Abs(calculatedWithDeduction - currentInvoiceTotal);
                var diffWithoutDeduction = Math.Abs(calculatedWithoutDeduction - currentInvoiceTotal);

                // Use the calculation that's closer to the current total
                var calculatedTotal = diffWithDeduction <= diffWithoutDeduction ?
                    calculatedWithDeduction : calculatedWithoutDeduction;

                if (Math.Abs(currentInvoiceTotal - calculatedTotal) > 0.01)
                {
                    _logger.Information("Updating InvoiceTotal from {OldValue} to {NewValue} based on calculation",
                        invoice.InvoiceTotal, calculatedTotal);
                    invoice.InvoiceTotal = calculatedTotal;
                }

                _logger.Debug("Dependent field recalculation complete for invoice {InvoiceNo}", invoice.InvoiceNo);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error recalculating dependent fields for invoice {InvoiceNo}", invoice?.InvoiceNo);
            }
        }

        #endregion
    }
}