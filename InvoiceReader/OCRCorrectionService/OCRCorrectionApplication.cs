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
        #region Enhanced Correction Application to In-Memory Objects

        /// <summary>
        /// Applies all high-confidence corrections directly to the in-memory ShipmentInvoice object.
        /// This version contains the definitive fix for preventing double-counting aggregation errors.
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

            _logger.Error("🚀 **APPLY_CORRECTIONS_START**: Applying {Count} high-confidence (>= {Threshold:P0}) corrections to invoice {InvoiceNo}.",
                errorsToApplyDirectly.Count, CONFIDENCE_THRESHOLD, invoice.InvoiceNo);
            _logger.Error("   - **ARCHITECTURAL_INTENT**: To apply AI-found corrections to the in-memory invoice. Aggregate fields will be reset to zero before summing new components to prevent double-counting.");

            LogFinancialState("Initial State (Before Corrections)", invoice);

            // ================== DEFINITIVE FIX START ==================
            // Identify all unique numeric fields that the AI is providing 'omission' corrections for.
            var numericFieldsToReset = errorsToApplyDirectly
                .Where(e => e.ErrorType == "omission")
                .Select(e => this.MapDeepSeekFieldToDatabase(e.Field))
                .Where(info => info != null && (info.DataType == "Number" || info.DataType.Contains("currency") || info.DataType.Contains("decimal") || info.DataType.Contains("double")))
                .Select(info => info.DatabaseFieldName)
                .Distinct()
                .ToList();

            // Reset these fields to 0 before applying corrections. This prevents adding AI-found omissions
            // to a potentially flawed or already complete aggregate from the initial OCR read.
            if (numericFieldsToReset.Any())
            {
                _logger.Error("   - **LOGIC_PATH**: Identified numeric fields with AI omissions: [{Fields}]. Resetting them to 0.0 to prevent double-counting.", string.Join(", ", numericFieldsToReset));
                foreach (var fieldName in numericFieldsToReset)
                {
                    var prop = typeof(ShipmentInvoice).GetProperty(fieldName, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                    if (prop != null && prop.CanWrite && (prop.PropertyType == typeof(double?) || prop.PropertyType == typeof(double)))
                    {
                        prop.SetValue(invoice, 0.0d);
                    }
                }
                LogFinancialState("State After Numeric Field Reset", invoice);
            }
            // =================== DEFINITIVE FIX END ===================

            foreach (var error in errorsToApplyDirectly)
            {
                var result = await this.ApplySingleValueOrFormatCorrectionToInvoiceAsync(invoice, error)
                                 .ConfigureAwait(false);
                correctionResults.Add(result);
                LogCorrectionResult(result, "DIRECT_FIX_APPLIED");
            }

            if (correctionResults.Any(r => r.Success))
            {
                invoice.TrackingState = TrackingState.Modified;
            }

            LogFinancialState("Final State (After All Corrections)", invoice);
            _logger.Error("🏁 **APPLY_CORRECTIONS_COMPLETE** for invoice {InvoiceNo}.", invoice.InvoiceNo);

            return correctionResults;
        }

        private async Task<CorrectionResult> ApplySingleValueOrFormatCorrectionToInvoiceAsync(
            ShipmentInvoice invoice,
            InvoiceError error)
        {
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
            _logger.Error("   - ➡️ **APPLYING_SINGLE_CORRECTION**: Field='{Field}', CorrectValue='{NewValue}', Type='{Type}', Line={LineNum}, Text='{LineText}'",
                error.Field, error.CorrectValue, error.ErrorType, error.LineNumber, TruncateForLog(error.LineText, 70));

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
                _logger.Error("     - **PARSE_SUCCESS**: Parsed CorrectValue '{Original}' into system value '{Parsed}' of type {Type}.",
                    error.CorrectValue, parsedCorrectedValue ?? "null", parsedCorrectedValue?.GetType().Name ?? "null");

                if (this.ApplyFieldCorrection(invoice, error.Field, parsedCorrectedValue))
                {
                    result.NewValue = this.GetCurrentFieldValue(invoice, error.Field)?.ToString();
                    result.Success = true;
                    _logger.Error("     - ✅ **APPLY_SUCCESS**: Correction for {Field} applied. Invoice's final value for this field is now '{NewVal}'.",
                        error.Field, result.NewValue);
                }
                else
                {
                    result.Success = false;
                    result.ErrorMessage = $"Field '{error.Field}' not recognized or value '{error.CorrectValue}' not applied/aggregated.";
                    _logger.Warning("     - ❌ **APPLY_FAILURE**: {ErrorMessage}", result.ErrorMessage);
                }
                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "     - 🚨 **APPLY_EXCEPTION** while applying correction for {Field}.", error.Field);
                result.Success = false;
                result.ErrorMessage = ex.Message;
                return result;
            }
        }

        public bool ApplyFieldCorrection(ShipmentInvoice invoice, string fieldNameFromError, object correctedValue)
        {
            var fieldInfo = this.MapDeepSeekFieldToDatabase(fieldNameFromError);
            var targetPropertyName = fieldInfo?.DatabaseFieldName ?? fieldNameFromError;

            _logger.Error("       - ▶️ **Enter ApplyFieldCorrection**: Target Property='{TargetProp}', Incoming Value='{Value}' (Type: {Type})",
                targetPropertyName, correctedValue ?? "null", correctedValue?.GetType().Name ?? "null");

            try
            {
                var invoiceProp = typeof(ShipmentInvoice).GetProperty(
                    targetPropertyName,
                    System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

                if (invoiceProp != null)
                {
                    object existingValue = invoiceProp.GetValue(invoice);
                    _logger.Error("         - **STATE_CHECK**: Current value of '{TargetProp}' is '{ExistingValue}' (Type: {Type}).",
                        targetPropertyName, existingValue ?? "null", existingValue?.GetType().Name ?? "null");

                    var propertyType = Nullable.GetUnderlyingType(invoiceProp.PropertyType) ?? invoiceProp.PropertyType;
                    object finalValueToSet = correctedValue;

                    if (existingValue != null)
                    {
                        _logger.Error("         - **AGGREGATION_CHECK**: Existing value is not null. Applying aggregation logic.");

                        if (propertyType == typeof(double) || propertyType == typeof(decimal) || propertyType == typeof(int))
                        {
                            _logger.Error("           - **LOGIC_PATH**: Matched Numeric Aggregation Path.");
                            var existingNumeric = Convert.ToDouble(existingValue, System.Globalization.CultureInfo.InvariantCulture);
                            var newNumeric = Convert.ToDouble(correctedValue, System.Globalization.CultureInfo.InvariantCulture);
                            finalValueToSet = existingNumeric + newNumeric;
                            _logger.Error("           - **NUMERIC_AGGREGATION**: Calculation: {Existing} + {New} = {Final}",
                                existingNumeric, newNumeric, finalValueToSet);
                        }
                        else if (propertyType == typeof(string))
                        {
                            _logger.Error("           - **LOGIC_PATH**: Matched String Aggregation Path.");
                            var existingString = existingValue.ToString();
                            var newString = correctedValue?.ToString() ?? "";
                            if (!string.IsNullOrWhiteSpace(existingString) && !string.IsNullOrWhiteSpace(newString))
                            {
                                finalValueToSet = $"{existingString}{Environment.NewLine}{newString}";
                                _logger.Error("           - **STRING_AGGREGATION**: Concatenating values.");
                            }
                            else
                            {
                                finalValueToSet = string.IsNullOrWhiteSpace(existingString) ? newString : existingString;
                            }
                        }
                    }
                    else
                    {
                        _logger.Error("         - **NO_AGGREGATION**: Existing value is null. Will set value directly.");
                    }

                    _logger.Error("         - **TYPE_CONVERSION**: Preparing to set final value '{FinalValue}' (Type: {Type}) to property of type {PropType}.",
                        finalValueToSet ?? "null", finalValueToSet?.GetType().Name ?? "null", propertyType.Name);

                    var convertedValue = finalValueToSet != null
                                             ? Convert.ChangeType(finalValueToSet, propertyType, System.Globalization.CultureInfo.InvariantCulture)
                                             : null;
                    invoiceProp.SetValue(invoice, convertedValue);

                    _logger.Error("       - ✅ **Leave ApplyFieldCorrection**: Successfully set '{TargetProp}' to '{Value}'.", targetPropertyName, convertedValue ?? "null");
                    return true;
                }

                _logger.Warning("       - ❌ **PROPERTY_NOT_FOUND**: Property '{TargetPropertyName}' not found on ShipmentInvoice.", targetPropertyName);
                return false;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "       - 🚨 **EXCEPTION in ApplyFieldCorrection** for {TargetProp} with value '{Value}'", targetPropertyName, correctedValue);
                return false;
            }
        }

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