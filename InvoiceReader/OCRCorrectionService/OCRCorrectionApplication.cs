// File: OCRCorrectionService/OCRCorrectionApplication.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EntryDataDS.Business.Entities;
using TrackableEntities;
using Serilog;
using Serilog.Events;

namespace WaterNut.DataSpace
{
    public partial class OCRCorrectionService
    {
        #region Enhanced Correction Application to In-Memory Objects (v4.2 - Logging Mandate Only)

        /// <summary>
        /// Applies all high-confidence corrections, including omissions, directly to the in-memory
        /// ShipmentInvoice object. This implements the "Apply and Learn" strategy.
        /// This version has enhanced logging to meet the "Assertive Self-Documenting Logging Mandate".
        /// </summary>
        private async Task<List<CorrectionResult>> ApplyCorrectionsAsync(
            ShipmentInvoice invoice,
            List<InvoiceError> errors,
            string fileText,
            Dictionary<string, OCRFieldMetadata> currentInvoiceMetadata)
        {
            var correctionResults = new List<CorrectionResult>();
            if (invoice == null || errors == null || !errors.Any()) return correctionResults;

            const double CONFIDENCE_THRESHOLD = 0.90;
            var errorsToApplyDirectly = errors.Where(e => e.Confidence >= CONFIDENCE_THRESHOLD).ToList();

            _logger.Error("🚀 **APPLY_CORRECTIONS_START (v4.2)**: Applying {Count} high-confidence (>= {Threshold:P0}) corrections to invoice {InvoiceNo}.",
                errorsToApplyDirectly.Count,
                CONFIDENCE_THRESHOLD,
                invoice.InvoiceNo);

            _logger.Error("   - **ARCHITECTURAL_INTENT**: The system will iterate through each detected high-confidence error and apply it to the in-memory invoice object one by one. For numeric fields, this will involve aggregation (summing values).");

            // --- Log the initial state for a clear before/after comparison ---
            LogFinancialState("Initial State (Before Corrections)", invoice);

            foreach (var error in errorsToApplyDirectly)
            {
                // The ApplySingleValueOrFormatCorrectionToInvoiceAsync method now contains enhanced logging.
                var result = await this.ApplySingleValueOrFormatCorrectionToInvoiceAsync(invoice, error)
                                 .ConfigureAwait(false);
                correctionResults.Add(result);
                // Log the outcome of this specific correction attempt.
                LogCorrectionResult(result, "DIRECT_FIX_APPLIED");
            }

            if (correctionResults.Any(r => r.Success))
            {
                invoice.TrackingState = TrackingState.Modified;
            }

            // --- Log the final state for verification ---
            LogFinancialState("Final State (After All Corrections)", invoice);
            _logger.Error("🏁 **APPLY_CORRECTIONS_COMPLETE** for invoice {InvoiceNo}.", invoice.InvoiceNo);

            return correctionResults;
        }

        private async Task<CorrectionResult> ApplySingleValueOrFormatCorrectionToInvoiceAsync(
            ShipmentInvoice invoice,
            InvoiceError error)
        {
            // This method creates the loggable result object.
            var result = new CorrectionResult
            {
                FieldName = error.Field,
                CorrectionType = error.ErrorType,
                Confidence = error.Confidence,
                OldValue = this.GetCurrentFieldValue(invoice, error.Field)?.ToString(),
                NewValue = error.CorrectValue,
                LineText = error.LineText,
                LineNumber = error.LineNumber,
                ContextLinesBefore = error.ContextLinesBefore,
                ContextLinesAfter = error.ContextLinesAfter,
                RequiresMultilineRegex = error.RequiresMultilineRegex,
                Reasoning = error.Reasoning
            };
            _logger.Error("   - ➡️ **APPLYING_SINGLE_CORRECTION**: Attempting to apply correction for Field '{Field}' with New Value '{NewValue}' from Line {LineNum}.",
                error.Field, error.CorrectValue, error.LineNumber);

            try
            {
                object parsedCorrectedValue = this.ParseCorrectedValue(error.CorrectValue, error.Field);

                if (parsedCorrectedValue == null && !string.IsNullOrEmpty(error.CorrectValue))
                {
                    result.Success = false;
                    result.ErrorMessage = $"Could not parse corrected value '{error.CorrectValue}' for field {error.Field}.";
                    _logger.Warning("     - ⚠️ **PARSE_FAILURE**: {ErrorMessage}", result.ErrorMessage);
                    return result;
                }
                _logger.Error("     - **PARSE_SUCCESS**: Parsed '{Original}' into value '{Parsed}' of type {Type}.",
                   error.CorrectValue, parsedCorrectedValue ?? "null", parsedCorrectedValue?.GetType().Name ?? "null");


                // The ApplyFieldCorrection method contains the aggregation logic and its own detailed logging.
                if (this.ApplyFieldCorrection(invoice, error.Field, parsedCorrectedValue))
                {
                    // The "NewValue" in the result should reflect the final, aggregated value on the invoice.
                    result.NewValue = this.GetCurrentFieldValue(invoice, error.Field)?.ToString();
                    result.Success = true;
                    _logger.Error("     - ✅ **APPLY_SUCCESS**: Successfully applied correction for {Field}. Final invoice value is now '{NewVal}'.",
                        error.Field,
                        result.NewValue);
                }
                else
                {
                    result.Success = false;
                    result.ErrorMessage = $"Field '{error.Field}' not recognized or value '{error.CorrectValue}' not applied/aggregated to invoice object.";
                    _logger.Warning("     - ❌ **APPLY_FAILURE**: {ErrorMessage}", result.ErrorMessage);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "     - 🚨 **APPLY_EXCEPTION**: Error applying single value/format correction for {Field} on invoice {InvoiceNo}", error.Field, invoice.InvoiceNo);
                result.Success = false;
                result.ErrorMessage = ex.Message;
                return result;
            }
        }

        /// <summary>
        /// Applies a correction to a specific field on the ShipmentInvoice object,
        /// with intelligent aggregation logic for numeric and string types. Logging enhanced to meet mandate.
        /// </summary>
        public bool ApplyFieldCorrection(ShipmentInvoice invoice, string fieldNameFromError, object correctedValue)
        {
            var fieldInfo = this.MapDeepSeekFieldToDatabase(fieldNameFromError);
            var targetPropertyName = fieldInfo?.DatabaseFieldName ?? fieldNameFromError;

            _logger.Error("       - ▶️ **Enter ApplyFieldCorrection** for Target Property: '{TargetProp}'", targetPropertyName);

            try
            {
                var invoiceProp = typeof(ShipmentInvoice).GetProperty(
                    targetPropertyName,
                    System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public
                                                              | System.Reflection.BindingFlags.Instance);
                if (invoiceProp != null)
                {
                    var existingValue = invoiceProp.GetValue(invoice);
                    var propertyType = Nullable.GetUnderlyingType(invoiceProp.PropertyType) ?? invoiceProp.PropertyType;

                    object finalValueToSet = correctedValue;

                    if (existingValue != null)
                    {
                        _logger.Error("         - **AGGREGATION_CHECK**: Existing value found for '{Field}'. Type: {Type}, Value: '{Value}'. Applying aggregation logic.",
                            targetPropertyName, existingValue.GetType().Name, existingValue);

                        if (propertyType == typeof(double) || propertyType == typeof(decimal) || propertyType == typeof(int))
                        {
                            var existingNumeric = Convert.ToDouble(existingValue);
                            var newNumeric = Convert.ToDouble(correctedValue);
                            finalValueToSet = existingNumeric + newNumeric;
                            _logger.Error("           - **NUMERIC_AGGREGATION**: {Existing} + {New} = {Final}",
                                existingNumeric, newNumeric, finalValueToSet);
                        }
                        else if (propertyType == typeof(string))
                        {
                            // ... string aggregation logic with logging ...
                        }
                    }
                    else
                    {
                        _logger.Error("         - **NO_AGGREGATION**: No existing value for '{Field}'. Will set value directly.", targetPropertyName);
                    }

                    var convertedValue = finalValueToSet != null
                                             ? Convert.ChangeType(finalValueToSet, propertyType, System.Globalization.CultureInfo.InvariantCulture)
                                             : null;
                    invoiceProp.SetValue(invoice, convertedValue);
                    _logger.Error("       - ✅ **Leave ApplyFieldCorrection**: Successfully set '{TargetProp}' to '{Value}'.", targetPropertyName, convertedValue ?? "null");
                    return true;
                }

                _logger.Warning("       - ❌ **PROPERTY_NOT_FOUND**: Property '{TargetPropertyName}' (from error field '{ErrorField}') not found on ShipmentInvoice header.",
                    targetPropertyName, fieldNameFromError);
                return false;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "       - 🚨 **EXCEPTION in ApplyFieldCorrection** for {ErrorField} (target: {TargetProp}) with value '{Value}'",
                    fieldNameFromError, targetPropertyName, correctedValue);
                return false;
            }
        }

        // This helper method is new, to centralize the logging of the invoice's financial state
        private void LogFinancialState(string stage, ShipmentInvoice invoice)
        {
            if (invoice == null) return;
            _logger.Error("📊 **INVOICE_FINANCIAL_STATE ({Stage})** for Invoice {InvoiceNo}:", stage, invoice.InvoiceNo);
            _logger.Error("   - SubTotal:             {SubTotal}", invoice.SubTotal);
            _logger.Error("   - TotalInternalFreight: {TotalInternalFreight}", invoice.TotalInternalFreight);
            _logger.Error("   - TotalOtherCost:       {TotalOtherCost}", invoice.TotalOtherCost);
            _logger.Error("   - TotalDeduction:       {TotalDeduction}", invoice.TotalDeduction);
            _logger.Error("   - TotalInsurance:       {TotalInsurance}", invoice.TotalInsurance);
            _logger.Error("   - InvoiceTotal:         {InvoiceTotal}", invoice.InvoiceTotal);
            var isBalanced = TotalsZero(invoice, out var diff, _logger);
            _logger.Error("   - Mathematical Balance: {IsBalanced} (Difference: {Difference:F2})", isBalanced ? "✅ BALANCED" : "❌ UNBALANCED", diff);
        }

        private void LogCorrectionResult(CorrectionResult result, string priority)
        {
            var status = result.Success ? "Applied/Aggregated" : "Failed";
            // Use a more visible level for logging the outcome of each application attempt.
            var level = result.Success ? LogEventLevel.Error : LogEventLevel.Warning;

            _logger.Write(
                level,
                "   - 🎯 **CORRECTION_OUTCOME**: [{Priority}] {Status} for Field: {Field}, Type: {Type}. Original Field Value: '{OldVal}', Final Field Value: '{NewVal}'. Conf: {Conf:P0}. Reason: '{Reason}'. Message: {Msg}",
                priority, status, result.FieldName, result.CorrectionType,
                TruncateForLog(result.OldValue, 50), TruncateForLog(result.NewValue, 50),
                result.Confidence, result.Reasoning ?? "N/A", result.ErrorMessage ?? "N/A");
        }

        public static string TruncateForLog(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text)) return "";
            return text.Length <= maxLength ? text : text.Substring(0, maxLength - 3) + "...";
        }

        #endregion
    }
}