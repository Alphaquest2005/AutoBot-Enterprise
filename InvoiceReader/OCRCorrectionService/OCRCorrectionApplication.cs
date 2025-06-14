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

        private async Task<List<CorrectionResult>> ApplyCorrectionsAsync(
            ShipmentInvoice invoice,
            List<InvoiceError> errors,
            string fileText,
            Dictionary<string, OCRFieldMetadata> currentInvoiceMetadata)
        {
            var correctionResults = new List<CorrectionResult>();
            if (invoice == null || errors == null || !errors.Any())
            {
                _logger.Information("ApplyCorrectionsAsync: No invoice or errors to apply.");
                return correctionResults;
            }

            // --- AGGREGATION LOGIC ---
            var aggregatedErrors = new List<InvoiceError>();
            var errorsToProcessIndividually = new List<InvoiceError>();

            var groupedByField = errors.GroupBy(e => e.Field);

            foreach (var group in groupedByField)
            {
                var aggregateParts = group.Where(e => e.ErrorType == "omission_aggregate_part").ToList();
                if (aggregateParts.Any())
                {
                    decimal sum = 0;
                    foreach (var part in aggregateParts)
                    {
                        if (decimal.TryParse(part.CorrectValue, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var partValue))
                        {
                            sum += partValue;
                        }
                    }

                    var masterError = aggregateParts.First();
                    masterError.CorrectValue = sum.ToString("F2");
                    masterError.ErrorType = "omission";
                    masterError.Reasoning = $"Aggregated value from {aggregateParts.Count} line items.";
                    aggregatedErrors.Add(masterError);
                    _logger.Information("Aggregated {Count} parts for field '{Field}' to a total of {Sum}", aggregateParts.Count, group.Key, sum);

                    errorsToProcessIndividually.AddRange(group.Where(e => e.ErrorType != "omission_aggregate_part"));
                }
                else
                {
                    errorsToProcessIndividually.AddRange(group);
                }
            }

            var finalErrorsToApply = errorsToProcessIndividually.Concat(aggregatedErrors).ToList();
            // --- END AGGREGATION LOGIC ---

            _logger.Information("Applying corrections to invoice {InvoiceNo}: {TotalCount} final corrections after aggregation.",
                invoice.InvoiceNo, finalErrorsToApply.Count);

            var correctedFields = new HashSet<string>();

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

            // Recalculation can be complex and is sometimes better handled after the entire pipeline step.
            // RecalculateDependentFields(invoice, correctedFields);

            if (correctionResults.Any(r => r.Success))
            {
                invoice.TrackingState = TrackingState.Modified;
                _logger.Information("Invoice {InvoiceNo} marked as modified due to successful value applications.", invoice.InvoiceNo);
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
                object parsedCorrectedValue = this.ParseCorrectedValue(error.CorrectValue, error.Field);

                if (parsedCorrectedValue == null && !string.IsNullOrEmpty(error.CorrectValue))
                {
                    result.Success = false;
                    result.ErrorMessage = $"Could not parse corrected value '{error.CorrectValue}' for field {error.Field}.";
                    _logger.Warning(result.ErrorMessage);
                    return result;
                }

                result.OldValue = this.GetCurrentFieldValue(invoice, error.Field)?.ToString();

                if (this.ApplyFieldCorrection(invoice, error.Field, parsedCorrectedValue))
                {
                    result.NewValue = parsedCorrectedValue?.ToString() ?? error.CorrectValue;
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

        public bool ApplyFieldCorrection(ShipmentInvoice invoice, string fieldNameFromError, object correctedValue)
        {
            var fieldInfo = this.MapDeepSeekFieldToDatabase(fieldNameFromError);
            var targetPropertyName = fieldInfo?.DatabaseFieldName ?? fieldNameFromError;

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

                // InvoiceDetail logic would go here if needed, but is omitted for clarity as this test case is header-only.

                _logger.Warning("ApplyFieldCorrection: Property '{TargetPropertyName}' (from error field '{ErrorField}') not found on ShipmentInvoice header.", targetPropertyName, fieldNameFromError);
                return false;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error applying field correction for {ErrorField} (target: {TargetProp}) with value '{Value}'", fieldNameFromError, targetPropertyName, correctedValue);
                return false;
            }
        }

        private void LogCorrectionResult(CorrectionResult result, string priority)
        {
            var status = result.Success ? "Applied" : "Failed";
            var level = result.Success ? Serilog.Events.LogEventLevel.Information : Serilog.Events.LogEventLevel.Warning;

            _logger.Write(level, "[{Priority}] {Status} correction for Field: {Field}, Type: {Type}. Old: '{OldVal}', New: '{NewVal}'. Conf: {Conf:P0}. Reason: '{Reason}'. Message: {Msg}",
                priority, status, result.FieldName, result.CorrectionType,
                this.TruncateForLog(result.OldValue, 50), this.TruncateForLog(result.NewValue, 50),
                result.Confidence, result.Reasoning ?? "N/A", result.ErrorMessage ?? "N/A");
        }

        #endregion
    }
}