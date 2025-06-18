// File: OCRCorrectionService/OCRCorrectionApplication.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using EntryDataDS.Business.Entities;
using TrackableEntities;
using Serilog;

namespace WaterNut.DataSpace
{
    public partial class OCRCorrectionService
    {
        #region Enhanced Correction Application to In-Memory Objects

        /// <summary>
        /// Applies all high-confidence corrections, including omissions, directly to the in-memory
        /// ShipmentInvoice object. This implements the "Apply and Learn" strategy.
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
            // The ErrorType filter is REMOVED. All high-confidence errors will be processed.
            var errorsToApplyDirectly = errors.Where(e => e.Confidence >= CONFIDENCE_THRESHOLD).ToList();

            _logger.Information(
                "Applying {Count} high-confidence (>= {Threshold:P0}) corrections directly to in-memory invoice {InvoiceNo}.",
                errorsToApplyDirectly.Count,
                CONFIDENCE_THRESHOLD,
                invoice.InvoiceNo);

            foreach (var error in errorsToApplyDirectly)
            {
                // Pass the existing invoice object to be modified with aggregation logic.
                var result = await this.ApplySingleValueOrFormatCorrectionToInvoiceAsync(invoice, error)
                                 .ConfigureAwait(false);
                correctionResults.Add(result);
                LogCorrectionResult(result, "DIRECT_FIX_APPLIED");
            }

            if (correctionResults.Any(r => r.Success))
            {
                invoice.TrackingState = TrackingState.Modified;
            }

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

            try
            {
                object parsedCorrectedValue = this.ParseCorrectedValue(error.CorrectValue, error.Field);

                if (parsedCorrectedValue == null && !string.IsNullOrEmpty(error.CorrectValue))
                {
                    result.Success = false;
                    result.ErrorMessage =
                        $"Could not parse corrected value '{error.CorrectValue}' for field {error.Field}.";
                    _logger.Warning(result.ErrorMessage);
                    return result;
                }

                // The ApplyFieldCorrection method contains the aggregation logic.
                if (this.ApplyFieldCorrection(invoice, error.Field, parsedCorrectedValue))
                {
                    // The "NewValue" in the result should reflect the final, aggregated value on the invoice.
                    result.NewValue = this.GetCurrentFieldValue(invoice, error.Field)?.ToString();
                    result.Success = true;
                    _logger.Debug(
                        "Successfully applied/aggregated in-memory correction for {Field}: From '{Old}' to final value '{New}'",
                        error.Field,
                        result.OldValue,
                        result.NewValue);
                }
                else
                {
                    result.Success = false;
                    result.ErrorMessage =
                        $"Field '{error.Field}' not recognized or value '{error.CorrectValue}' not applied/aggregated to invoice object.";
                    _logger.Warning(result.ErrorMessage);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Error applying single value/format correction for {Field} on invoice {InvoiceNo}",
                    error.Field,
                    invoice.InvoiceNo);
                result.Success = false;
                result.ErrorMessage = ex.Message;
                return result;
            }
        }

        /// <summary>
        /// Applies a correction to a specific field on the ShipmentInvoice object,
        /// now with intelligent aggregation logic for numeric and string types.
        /// </summary>
        public bool ApplyFieldCorrection(ShipmentInvoice invoice, string fieldNameFromError, object correctedValue)
        {
            var fieldInfo = this.MapDeepSeekFieldToDatabase(fieldNameFromError);
            var targetPropertyName = fieldInfo?.DatabaseFieldName ?? fieldNameFromError;

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
                        _logger.Debug(
                            "Aggregation Check for Field '{Field}': Existing value found. Type: {Type}, Value: '{Value}'",
                            targetPropertyName,
                            existingValue.GetType().Name,
                            existingValue);

                        if (propertyType == typeof(double) || propertyType == typeof(decimal)
                                                           || propertyType == typeof(int))
                        {
                            var existingNumeric = Convert.ToDouble(existingValue);
                            var newNumeric = Convert.ToDouble(correctedValue);
                            finalValueToSet = existingNumeric + newNumeric;
                            _logger.Information(
                                "   -> Numeric Aggregation: {Existing} + {New} = {Final}",
                                existingNumeric,
                                newNumeric,
                                finalValueToSet);
                        }
                        else if (propertyType == typeof(string))
                        {
                            var existingString = existingValue.ToString();
                            var newString = correctedValue?.ToString() ?? "";
                            if (!string.IsNullOrWhiteSpace(existingString) && !string.IsNullOrWhiteSpace(newString))
                            {
                                finalValueToSet = $"{existingString}{Environment.NewLine}{newString}";
                                _logger.Information("   -> String Aggregation: Concatenating new value.");
                            }
                            else
                            {
                                finalValueToSet =
                                    string.IsNullOrWhiteSpace(existingString) ? newString : existingString;
                            }
                        }
                    }

                    var convertedValue = finalValueToSet != null
                                             ? Convert.ChangeType(finalValueToSet, propertyType)
                                             : null;
                    invoiceProp.SetValue(invoice, convertedValue);
                    return true;
                }

                _logger.Warning(
                    "ApplyFieldCorrection: Property '{TargetPropertyName}' (from error field '{ErrorField}') not found on ShipmentInvoice header.",
                    targetPropertyName,
                    fieldNameFromError);
                return false;
            }
            catch (Exception ex)
            {
                _logger.Error(
                    ex,
                    "Error applying/aggregating field correction for {ErrorField} (target: {TargetProp}) with value '{Value}'",
                    fieldNameFromError,
                    targetPropertyName,
                    correctedValue);
                return false;
            }
        }

        private void LogCorrectionResult(CorrectionResult result, string priority)
        {
            var status = result.Success ? "Applied/Aggregated" : "Failed";
            var level = result.Success
                            ? Serilog.Events.LogEventLevel.Information
                            : Serilog.Events.LogEventLevel.Warning;

            _logger.Write(
                level,
                "[{Priority}] {Status} correction for Field: {Field}, Type: {Type}. Original Field Value: '{OldVal}', Final Field Value: '{NewVal}'. Conf: {Conf:P0}. Reason: '{Reason}'. Message: {Msg}",
                priority,
                status,
                result.FieldName,
                result.CorrectionType,
                TruncateForLog(result.OldValue, 50),
                TruncateForLog(result.NewValue, 50),
                result.Confidence,
                result.Reasoning ?? "N/A",
                result.ErrorMessage ?? "N/A");
        }

        /// <summary>
        /// Truncates a string to a specified maximum length, adding an ellipsis if truncated.
        /// Useful for logging or displaying long strings.
        /// </summary>
        public static string TruncateForLog(string text, int maxLength)
        {
            if (string.IsNullOrEmpty(text)) return "";
            return text.Length <= maxLength ? text : text.Substring(0, maxLength - 3) + "...";
        }

        #endregion


    }
}

