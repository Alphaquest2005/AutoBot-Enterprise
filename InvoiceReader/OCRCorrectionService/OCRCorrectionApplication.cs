// File: OCRCorrectionService/OCRCorrectionApplication.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions; // Keep for ApplyInvoiceDetailCorrection
using System.Threading.Tasks;
using EntryDataDS.Business.Entities; // For ShipmentInvoice, InvoiceDetails
using TrackableEntities; // For TrackingState
using Serilog; // For ILogger, assumed available as this._logger

namespace WaterNut.DataSpace
{
    public partial class OCRCorrectionService
    {
        #region Enhanced Correction Application to In-Memory Objects

        /// <summary>
        /// Applies a list of identified errors (InvoiceError) as corrections to the provided ShipmentInvoice object.
        /// This method orchestrates the application of both omission-type corrections and format/value corrections.
        /// It updates the in-memory ShipmentInvoice and returns a list of CorrectionResult.
        /// </summary>
        private async Task<List<CorrectionResult>> ApplyCorrectionsAsync(
            ShipmentInvoice invoice,
            List<InvoiceError> errors, // Detected errors/omissions
            string fileText,           // Full original text for context
            Dictionary<string, OCRFieldMetadata> currentInvoiceMetadata) // Metadata for fields already extracted
        {
            var correctionResults = new List<CorrectionResult>();
            if (invoice == null || errors == null || !errors.Any())
            {
                _logger.Information("ApplyCorrectionsAsync: No invoice or errors to apply.");
                return correctionResults;
            }

            // --- AGGREGATION LOGIC ---
            // Identify fields that need aggregation and sum them before application.
            var aggregatedErrors = new List<InvoiceError>();
            var errorsToProcessIndividually = new List<InvoiceError>();

            var groupedByField = errors.GroupBy(e => e.Field);

            foreach (var group in groupedByField)
            {
                var aggregateParts = group.Where(e => e.ErrorType == "omission_aggregate_part").ToList();
                if (aggregateParts.Any())
                {
                    // This field has parts that need to be summed
                    decimal sum = 0;
                    foreach (var part in aggregateParts)
                    {
                        if (decimal.TryParse(part.CorrectValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var partValue))
                        {
                            sum += partValue;
                        }
                    }

                    // Create a single "master" error for application to the invoice object
                    var masterError = aggregateParts.First(); // Use the first part as a template
                    masterError.CorrectValue = sum.ToString("F2");
                    masterError.ErrorType = "omission"; // Change type to a standard omission for application
                    masterError.Reasoning = $"Aggregated value from {aggregateParts.Count} line items.";
                    aggregatedErrors.Add(masterError);
                    _logger.Information("Aggregated {Count} parts for field '{Field}' to a total of {Sum}", aggregateParts.Count, group.Key, sum);

                    // Add all other errors for this field to be processed individually
                    errorsToProcessIndividually.AddRange(group.Where(e => e.ErrorType != "omission_aggregate_part"));
                }
                else
                {
                    // This field has no aggregate parts, process all its errors individually
                    errorsToProcessIndividually.AddRange(group);
                }
            }

            // Combine the master aggregated errors with the other individual errors for application.
            var finalErrorsToApply = errorsToProcessIndividually.Concat(aggregatedErrors).ToList();
            // --- END AGGREGATION LOGIC ---

            _logger.Information("Applying corrections to invoice {InvoiceNo}: {TotalCount} final corrections after aggregation.",
                invoice.InvoiceNo, finalErrorsToApply.Count);

            var correctedFields = new HashSet<string>();

            // The rest of the method now processes the final list of errors.
            foreach (var error in finalErrorsToApply)
            {
                var result = await this.ApplySingleValueOrFormatCorrectionToInvoiceAsync(invoice, error).ConfigureAwait(false);
                correctionResults.Add(result);
                LogCorrectionResult(result, "STANDARD_FIX_APPLIED");

                if (result.Success)
                {
                    correctedFields.Add(result.FieldName);
                }
            }

            RecalculateDependentFields(invoice, correctedFields);

            if (correctionResults.Any(r => r.Success))
            {
                invoice.TrackingState = TrackingState.Modified;
                _logger.Information("Invoice {InvoiceNo} marked as modified due to successful value applications.", invoice.InvoiceNo);
            }

            return correctionResults;
        }

        /// <summary>
        /// Applies a single format or value correction to the in-memory ShipmentInvoice object.
        /// </summary>
        private async Task<CorrectionResult> ApplySingleValueOrFormatCorrectionToInvoiceAsync(
            ShipmentInvoice invoice,
            InvoiceError error)
        {
            var result = new CorrectionResult
            {
                FieldName = error.Field,
                CorrectionType = error.ErrorType,
                Confidence = error.Confidence,
                OldValue = error.ExtractedValue,
                NewValue = error.CorrectValue,
                LineText = error.LineText,
                LineNumber = error.LineNumber,
                ContextLinesBefore = error.ContextLinesBefore,
                ContextLinesAfter = error.ContextLinesAfter,
                RequiresMultilineRegex = error.RequiresMultilineRegex,
                Reasoning = error.Reasoning
            };

            try
            {
                object parsedCorrectedValue = this.ParseCorrectedValue(error.CorrectValue, error.Field); // From OCRUtilities

                if (parsedCorrectedValue == null && !string.IsNullOrEmpty(error.CorrectValue))
                {
                    result.Success = false;
                    result.ErrorMessage = $"Could not parse corrected value '{error.CorrectValue}' for field {error.Field}.";
                    _logger.Warning(result.ErrorMessage);
                    return result;
                }

                result.OldValue = this.GetCurrentFieldValue(invoice, error.Field)?.ToString(); // From OCRUtilities

                if (this.ApplyFieldCorrection(invoice, error.Field, parsedCorrectedValue)) // Applies to in-memory invoice
                {
                    result.NewValue = parsedCorrectedValue?.ToString() ?? (error.CorrectValue == "" ? "" : null);
                    result.Success = true;
                    _logger.Debug("Successfully applied in-memory correction for {Field}: From '{Old}' to '{New}'", error.Field, result.OldValue, result.NewValue);
                }
                else
                {
                    result.Success = false;
                    result.ErrorMessage = $"Field '{error.Field}' not recognized or value '{error.CorrectValue}' not applied to invoice object.";
                    _logger.Warning(result.ErrorMessage);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error applying single value/format correction for {Field} on invoice {InvoiceNo}", error.Field, invoice.InvoiceNo);
                result.Success = false;
                result.ErrorMessage = ex.Message;
                return result;
            }
        }

        /// <summary>
        /// Applies correction to a specific field on the ShipmentInvoice object.
        /// </summary>
        public bool ApplyFieldCorrection(ShipmentInvoice invoice, string fieldNameFromError, object correctedValue)
        {
            var fieldInfo = this.MapDeepSeekFieldToDatabase(fieldNameFromError); // From OCRFieldMapping.cs
            var targetPropertyName = fieldInfo?.DatabaseFieldName ?? fieldNameFromError; // Use mapped name if available

            try
            {
                var invoiceProp = typeof(ShipmentInvoice).GetProperty(targetPropertyName, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if (invoiceProp != null)
                {
                    if (correctedValue == null && Nullable.GetUnderlyingType(invoiceProp.PropertyType) == null && invoiceProp.PropertyType.IsValueType)
                    {
                        _logger.Warning("Cannot assign null to non-nullable property {PropertyName}", targetPropertyName);
                        return false;
                    }
                    var convertedValue = correctedValue != null ? Convert.ChangeType(correctedValue, Nullable.GetUnderlyingType(invoiceProp.PropertyType) ?? invoiceProp.PropertyType) : null;
                    invoiceProp.SetValue(invoice, convertedValue);
                    return true;
                }

                if (targetPropertyName.StartsWith("invoicedetail", StringComparison.OrdinalIgnoreCase) ||
                    (fieldInfo != null && fieldInfo.EntityType == "InvoiceDetails"))
                {
                    return ApplyInvoiceDetailCorrection(invoice, fieldNameFromError, correctedValue);
                }

                _logger.Warning("ApplyFieldCorrection: Property '{TargetPropertyName}' (from error field '{ErrorField}') not found on ShipmentInvoice header or not identified as detail.", targetPropertyName, fieldNameFromError);
                return false;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error applying field correction for {ErrorField} (target: {TargetProp}) with value '{Value}'", fieldNameFromError, targetPropertyName, correctedValue);
                return false;
            }
        }

        /// <summary>
        /// Applies correction to a specific field within an InvoiceDetail item.
        /// </summary>
        private bool ApplyInvoiceDetailCorrection(ShipmentInvoice invoice, string prefixedErrorFieldName, object correctedValue)
        {
            var parts = prefixedErrorFieldName.Split('_');
            if (parts.Length < 3 || !parts[0].Equals("InvoiceDetail", StringComparison.OrdinalIgnoreCase))
            {
                _logger.Warning("ApplyInvoiceDetailCorrection: Field name '{FieldName}' not in expected 'InvoiceDetail_LineX_Field' format.", prefixedErrorFieldName);
                return false;
            }

            if (!int.TryParse(Regex.Match(parts[1], @"\d+").Value, out var lineNumber))
            { // Extract number from "LineX"
                _logger.Warning("ApplyInvoiceDetailCorrection: Could not parse line number from '{LinePart}' in field '{FieldName}'.", parts[1], prefixedErrorFieldName);
                return false;
            }

            string detailFieldName = parts[2];
            var detailItem = invoice.InvoiceDetails?.FirstOrDefault(d => d.LineNumber == lineNumber);

            if (detailItem == null)
            {
                _logger.Warning("ApplyInvoiceDetailCorrection: No InvoiceDetail found for LineNumber {LineNo} from field '{FieldName}'.", lineNumber, prefixedErrorFieldName);
                return false;
            }

            try
            {
                var detailProp = typeof(InvoiceDetails).GetProperty(detailFieldName, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if (detailProp != null)
                {
                    if (correctedValue == null && Nullable.GetUnderlyingType(detailProp.PropertyType) == null && detailProp.PropertyType.IsValueType)
                    {
                        _logger.Warning("Cannot assign null to non-nullable property {PropertyName} on InvoiceDetail Line {LineNum}", detailProp.Name, lineNumber);
                        return false;
                    }
                    var convertedValue = correctedValue != null ? Convert.ChangeType(correctedValue, Nullable.GetUnderlyingType(detailProp.PropertyType) ?? detailProp.PropertyType) : null;
                    detailProp.SetValue(detailItem, convertedValue);

                    if (detailFieldName.Equals("Quantity", StringComparison.OrdinalIgnoreCase) ||
                        detailFieldName.Equals("Cost", StringComparison.OrdinalIgnoreCase) ||
                        detailFieldName.Equals("Discount", StringComparison.OrdinalIgnoreCase))
                    {
                        RecalculateDetailTotal(detailItem);
                    }
                    detailItem.TrackingState = TrackingState.Modified;
                    return true;
                }
                _logger.Warning("ApplyInvoiceDetailCorrection: Property '{DetailField}' not found on InvoiceDetails for Line {LineNo}.", detailFieldName, lineNumber);
                return false;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error applying invoice detail correction for field {DetailField} on Line {LineNo} with value '{Value}'", detailFieldName, lineNumber, correctedValue);
                return false;
            }
        }

        /// <summary>
        /// Determines if an error is critical (affects calculations or core data).
        /// </summary>
        private bool IsCriticalError(InvoiceError error) // Error from OCRErrorDetection
        {
            var criticalErrorTypes = new[] {
                "calculation_error", "subtotal_mismatch", "invoice_total_mismatch",
                "unreasonable_value", "omitted_line_item"
            };
            if (criticalErrorTypes.Contains(error.ErrorType?.ToLowerInvariant())) return true;

            if (error.ErrorType == "value_error" || error.ErrorType == "format_error" || error.ErrorType == "character_confusion")
            {
                var fieldInfo = this.MapDeepSeekFieldToDatabase(error.Field);
                string canonicalFieldName = fieldInfo?.DatabaseFieldName ?? error.Field;
                var criticalDbFields = new[] { "invoicetotal", "subtotal", "totaldeduction", "quantity", "cost", "totalcost" };
                if (criticalDbFields.Contains(canonicalFieldName.ToLowerInvariant())) return true;
            }
            return false;
        }

        /// <summary>
        /// Logs the result of a correction attempt.
        /// </summary>
        private void LogCorrectionResult(CorrectionResult result, string priority) // CorrectionResult from this file
        {
            var status = result.Success ? "Applied" : "Failed";
            var level = result.Success ? Serilog.Events.LogEventLevel.Information : Serilog.Events.LogEventLevel.Warning;

            _logger.Write(level, "[{Priority}] {Status} correction for Field: {Field}, Type: {Type}. Old: '{OldVal}', New: '{NewVal}'. Conf: {Conf:P0}. Reason: '{Reason}'. Message: {Msg}",
                priority, status, result.FieldName, result.CorrectionType,
                this.TruncateForLog(result.OldValue, 50), this.TruncateForLog(result.NewValue, 50),
                result.Confidence, result.Reasoning ?? "N/A", result.ErrorMessage ?? "N/A");
        }

        /// <summary>
        /// Recalculates dependent fields like SubTotal and InvoiceTotal after individual corrections are applied.
        /// </summary>
        private void RecalculateDependentFields(ShipmentInvoice invoice, HashSet<string> correctedFields = null)
        {
            if (invoice == null) return;
            correctedFields = correctedFields ?? new HashSet<string>();

            try
            {
                if (invoice.InvoiceDetails != null)
                {
                    foreach (var detail in invoice.InvoiceDetails.Where(d => d != null))
                    {
                        RecalculateDetailTotal(detail);
                    }

                    if (!correctedFields.Contains("SubTotal"))
                    {
                        var calculatedSubTotal = invoice.InvoiceDetails.Sum(d => d?.TotalCost ?? 0);
                        if (Math.Abs((invoice.SubTotal ?? 0) - calculatedSubTotal) > 0.01)
                        {
                            _logger.Information("Recalculating SubTotal for {InvoiceNo}: From {OldSubTotal:F2} to {NewSubTotal:F2} based on line items sum.",
                                invoice.InvoiceNo, invoice.SubTotal ?? 0, calculatedSubTotal);
                            invoice.SubTotal = calculatedSubTotal;
                        }
                    }
                    else
                    {
                        _logger.Debug("Skipping SubTotal recalculation for {InvoiceNo} - field was directly corrected", invoice.InvoiceNo);
                    }
                }

                if (!correctedFields.Contains("InvoiceTotal"))
                {
                    var baseTotalForFinalCalc = (invoice.SubTotal ?? 0) +
                                                (invoice.TotalInternalFreight ?? 0) +
                                                (invoice.TotalOtherCost ?? 0) +
                                                (invoice.TotalInsurance ?? 0);
                    var deductionForFinalCalc = invoice.TotalDeduction ?? 0;
                    var expectedFinalInvoiceTotal = baseTotalForFinalCalc - deductionForFinalCalc;

                    if (Math.Abs((invoice.InvoiceTotal ?? 0) - expectedFinalInvoiceTotal) > 0.01)
                    {
                        _logger.Information("Recalculating InvoiceTotal for {InvoiceNo}: From {OldInvoiceTotal:F2} to {NewInvoiceTotal:F2} based on components.",
                           invoice.InvoiceNo, invoice.InvoiceTotal ?? 0, expectedFinalInvoiceTotal);
                        invoice.InvoiceTotal = expectedFinalInvoiceTotal;
                    }
                }
                else
                {
                    _logger.Debug("Skipping InvoiceTotal recalculation for {InvoiceNo} - field was directly corrected", invoice.InvoiceNo);
                }

                _logger.Debug("Dependent field recalculation complete for invoice {InvoiceNo}", invoice.InvoiceNo);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error recalculating dependent fields for invoice {InvoiceNo}", invoice?.InvoiceNo);
            }
        }

        /// <summary>
        /// Recalculates the TotalCost for a single InvoiceDetail item.
        /// </summary>
        private void RecalculateDetailTotal(InvoiceDetails detail)
        {
            if (detail == null) return;
            var quantity = detail.Quantity > 0 ? detail.Quantity : 0;
            var cost = detail.Cost > 0 ? detail.Cost : 0;
            var discount = detail.Discount ?? 0;

            detail.TotalCost = (quantity * cost) - discount;
        }

        #endregion
    }
}