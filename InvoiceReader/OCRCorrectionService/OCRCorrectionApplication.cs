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
        /// Database pattern updates are handled separately by UpdateRegexPatternsAsync using these CorrectionResults.
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

            // Resolve conflicting error proposals for the same field (e.g., prefer highest confidence)
            var filteredErrors = this.ResolveFieldConflicts(errors, invoice); // From OCRValidation.cs

            var omissionErrors = filteredErrors.Where(e => e.ErrorType == "omission" || e.ErrorType == "omitted_line_item").ToList();
            var formatAndValueErrors = filteredErrors.Where(e => e.ErrorType != "omission" && e.ErrorType != "omitted_line_item").ToList();

            _logger.Information("Applying corrections to invoice {InvoiceNo}: {OmissionCount} omissions, {FormatValueCount} format/value errors.",
                invoice.InvoiceNo, omissionErrors.Count, formatAndValueErrors.Count);

            // 1. Process Omissions
            // For omissions, the primary action is usually updating DB patterns so the field is extracted next time.
            // Applying the value to the *current* in-memory invoice object might also be desired.
            foreach (var error in omissionErrors)
            {
                // ProcessOmissionCorrectionAsync will attempt DB updates via strategy AND apply to current invoice if applicable.
                var result = await this.ProcessOmissionCorrectionAndApplyToInvoiceAsync(invoice, error, currentInvoiceMetadata, fileText).ConfigureAwait(false);
                correctionResults.Add(result);
                LogCorrectionResult(result, "OMISSION_APPLIED");
            }
            
            // 2. Process Format/Value Errors for already extracted (but incorrect) fields
            // These are applied directly to the invoice object.
            // The resulting CorrectionResult can then be used by UpdateRegexPatternsAsync to learn/update FieldFormatRegEx or Line Regexes.
            var criticalFormatErrors = formatAndValueErrors.Where(e => IsCriticalError(e)).ToList();
            var standardFormatErrors = formatAndValueErrors.Where(e => !IsCriticalError(e)).ToList();

            // Apply critical errors first, then standard ones
            foreach (var error in criticalFormatErrors.Concat(standardFormatErrors))
            {
                var result = await this.ApplySingleValueOrFormatCorrectionToInvoiceAsync(invoice, error).ConfigureAwait(false);
                correctionResults.Add(result);
                LogCorrectionResult(result, IsCriticalError(error) ? "CRITICAL_FIX_APPLIED" : "STANDARD_FIX_APPLIED");
            }

            // 3. Recalculate any fields that depend on corrected values (e.g., SubTotal from line items, InvoiceTotal from components)
            RecalculateDependentFields(invoice);

            // 4. Mark the invoice object as modified if any successful corrections were applied to it.
            if (correctionResults.Any(r => r.Success && r.CorrectionType != "omission_db_only")) // "omission_db_only" if an omission only updated DB patterns but not the in-memory invoice.
            {
                invoice.TrackingState = TrackingState.Modified;
                 _logger.Information("Invoice {InvoiceNo} marked as modified due to successful value applications.", invoice.InvoiceNo);
            }

            return correctionResults;
        }

        /// <summary>
        /// Handles an "omission" type error.
        /// The primary goal for an omission is to teach the system to extract it next time (DB pattern update).
        /// Optionally, it can also apply the omitted value to the current in-memory ShipmentInvoice object.
        /// </summary>
        private async Task<CorrectionResult> ProcessOmissionCorrectionAndApplyToInvoiceAsync(
            ShipmentInvoice invoiceToUpdate, // The in-memory invoice object
            InvoiceError omissionError,      // The detected omission
            Dictionary<string, OCRFieldMetadata> currentInvoiceMetadata, // Metadata of currently extracted fields
            string fileText)                 // Full text for context
        {
            var correctionResultForDB = new CorrectionResult // This is what gets sent for DB pattern learning
            {
                FieldName = omissionError.Field, CorrectionType = omissionError.ErrorType, Confidence = omissionError.Confidence,
                OldValue = omissionError.ExtractedValue ?? "", NewValue = omissionError.CorrectValue,
                LineText = omissionError.LineText, LineNumber = omissionError.LineNumber,
                ContextLinesBefore = omissionError.ContextLinesBefore, ContextLinesAfter = omissionError.ContextLinesAfter,
                RequiresMultilineRegex = omissionError.RequiresMultilineRegex, Reasoning = omissionError.Reasoning,
                Success = false // Will be set based on DB update success
            };

            try
            {
                _logger.Information("Processing omission for DB update: Field/Item '{OmissionField}' for Invoice {InvoiceNo}", omissionError.Field, invoiceToUpdate.InvoiceNo);

                // The core DB update logic for omissions (calling DeepSeek for regex, updating DB)
                // is now encapsulated within the OmissionUpdateStrategy, called by UpdateRegexPatternsAsync.
                // Here, we're creating the CorrectionResult that will FEED into UpdateRegexPatternsAsync later.
                // For the purpose of THIS method (ApplyCorrectionsAsync), we decide if the *in-memory* invoice should be updated.

                bool appliedToMemory = false;
                if (omissionError.ErrorType != "omitted_line_item") // For now, don't try to add whole new line items to memory
                {
                    var fieldInfo = this.MapDeepSeekFieldToDatabase(omissionError.Field);
                    if (fieldInfo != null && fieldInfo.EntityType == "ShipmentInvoice") // Only apply to known header fields for now
                    {
                        var parsedCorrectValue = this.ParseCorrectedValue(omissionError.CorrectValue, omissionError.Field);
                        if (parsedCorrectValue != null)
                        {
                            if (this.ApplyFieldCorrection(invoiceToUpdate, omissionError.Field, parsedCorrectValue))
                            {
                                _logger.Information("Applied omitted value for header field {OmittedField} = '{CorrectValue}' to in-memory invoice {InvoiceNo}.",
                                    omissionError.Field, omissionError.CorrectValue, invoiceToUpdate.InvoiceNo);
                                appliedToMemory = true;
                                // The CorrectionResult's success should reflect the DB update attempt, not this in-memory application.
                                // We set it to true here to indicate it's a valid correction to *try* to learn from.
                                correctionResultForDB.Success = true; 
                            }
                        }
                    }
                } else {
                    // For omitted_line_item, the DB strategy will handle creating new line definitions.
                    // Adding to in-memory invoice.InvoiceDetails is more complex and might be skipped here.
                    _logger.Information("Omitted line item '{OmittedItemField}' detected. DB strategy will handle pattern creation. In-memory invoice not modified for this item.", omissionError.Field);
                    correctionResultForDB.Success = true; // It's a valid omission to learn from.
                }
                
                // If not applied to memory but it's a valid omission, still mark success for DB learning.
                if (!appliedToMemory && omissionError.ErrorType == "omission") {
                    correctionResultForDB.Success = true; 
                    correctionResultForDB.CorrectionType = "omission_db_only"; // Custom type if not applied to memory
                }


                return correctionResultForDB;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error during ProcessOmissionCorrectionAndApplyToInvoiceAsync for field {OmissionField} on invoice {InvoiceNo}", omissionError.Field, invoiceToUpdate.InvoiceNo);
                correctionResultForDB.Success = false;
                correctionResultForDB.ErrorMessage = $"Processing Omission Error: {ex.Message}";
                return correctionResultForDB;
            }
        }

        /// <summary>
        /// Applies a single format or value correction to the in-memory ShipmentInvoice object.
        /// </summary>
        private async Task<CorrectionResult> ApplySingleValueOrFormatCorrectionToInvoiceAsync(
            ShipmentInvoice invoice, 
            InvoiceError error)
        {
            var result = new CorrectionResult // This will be returned and can be used for DB learning
            {
                FieldName = error.Field, CorrectionType = error.ErrorType, Confidence = error.Confidence,
                OldValue = error.ExtractedValue, NewValue = error.CorrectValue, LineText = error.LineText, 
                LineNumber = error.LineNumber, ContextLinesBefore = error.ContextLinesBefore, ContextLinesAfter = error.ContextLinesAfter,
                RequiresMultilineRegex = error.RequiresMultilineRegex, Reasoning = error.Reasoning
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

                // Get current value from invoice object for logging before change
                result.OldValue = this.GetCurrentFieldValue(invoice, error.Field)?.ToString(); // From OCRUtilities

                if (this.ApplyFieldCorrection(invoice, error.Field, parsedCorrectedValue)) // Applies to in-memory invoice
                {
                    // Update result with the actual applied value (after parsing and potential type conversion)
                    result.NewValue = parsedCorrectedValue?.ToString() ?? (error.CorrectValue == "" ? "" : null); // Handle intentional empty strings
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
                // Handle header fields
                var invoiceProp = typeof(ShipmentInvoice).GetProperty(targetPropertyName, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if (invoiceProp != null)
                {
                    if (correctedValue == null && Nullable.GetUnderlyingType(invoiceProp.PropertyType) == null && invoiceProp.PropertyType.IsValueType) {
                         _logger.Warning("Cannot assign null to non-nullable property {PropertyName}", targetPropertyName);
                        return false; // Cannot assign null to non-nullable value type
                    }
                    var convertedValue = correctedValue != null ? Convert.ChangeType(correctedValue, Nullable.GetUnderlyingType(invoiceProp.PropertyType) ?? invoiceProp.PropertyType) : null;
                    invoiceProp.SetValue(invoice, convertedValue);
                    return true;
                }

                // Handle invoice detail corrections (if fieldName indicates a detail field)
                if (targetPropertyName.StartsWith("invoicedetail", StringComparison.OrdinalIgnoreCase) || 
                    (fieldInfo != null && fieldInfo.EntityType == "InvoiceDetails"))
                {
                    // Pass the original fieldNameFromError as it might contain line number prefix
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
            if (parts.Length < 3 || !parts[0].Equals("InvoiceDetail", StringComparison.OrdinalIgnoreCase)) {
                _logger.Warning("ApplyInvoiceDetailCorrection: Field name '{FieldName}' not in expected 'InvoiceDetail_LineX_Field' format.", prefixedErrorFieldName);
                return false;
            }

            if (!int.TryParse(Regex.Match(parts[1], @"\d+").Value, out var lineNumber)) { // Extract number from "LineX"
                _logger.Warning("ApplyInvoiceDetailCorrection: Could not parse line number from '{LinePart}' in field '{FieldName}'.", parts[1], prefixedErrorFieldName);
                return false;
            }
            
            string detailFieldName = parts[2];
            var detailItem = invoice.InvoiceDetails?.FirstOrDefault(d => d.LineNumber == lineNumber);

            if (detailItem == null) {
                _logger.Warning("ApplyInvoiceDetailCorrection: No InvoiceDetail found for LineNumber {LineNo} from field '{FieldName}'.", lineNumber, prefixedErrorFieldName);
                return false;
            }

            try
            {
                var detailProp = typeof(InvoiceDetails).GetProperty(detailFieldName, System.Reflection.BindingFlags.IgnoreCase | System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);
                if (detailProp != null)
                {
                     if (correctedValue == null && Nullable.GetUnderlyingType(detailProp.PropertyType) == null && detailProp.PropertyType.IsValueType) {
                         _logger.Warning("Cannot assign null to non-nullable property {PropertyName} on InvoiceDetail Line {LineNum}", detailProp.Name, lineNumber);
                        return false;
                    }
                    var convertedValue = correctedValue != null ? Convert.ChangeType(correctedValue, Nullable.GetUnderlyingType(detailProp.PropertyType) ?? detailProp.PropertyType) : null;
                    detailProp.SetValue(detailItem, convertedValue);
                    
                    // If a quantity-affecting field changed, recalculate line total.
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

            // Consider value/format errors on key financial fields as critical
            if (error.ErrorType == "value_error" || error.ErrorType == "format_error" || error.ErrorType == "character_confusion")
            {
                var fieldInfo = this.MapDeepSeekFieldToDatabase(error.Field);
                string canonicalFieldName = fieldInfo?.DatabaseFieldName ?? error.Field;
                var criticalDbFields = new[] {"invoicetotal", "subtotal", "totaldeduction", "quantity", "cost", "totalcost"};
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
        private void RecalculateDependentFields(ShipmentInvoice invoice)
        {
            if (invoice == null) return;
            try
            {
                // Recalculate line totals first if they are not authoritative
                if (invoice.InvoiceDetails != null)
                {
                    foreach (var detail in invoice.InvoiceDetails.Where(d => d != null))
                    {
                        // Only recalculate if Qty or Cost might have changed and TotalCost exists.
                        // If TotalCost was directly corrected, trust it unless Qty/Cost also corrected.
                        // This assumes direct corrections to TotalCost are authoritative.
                        // A more robust system might flag which fields were AI-corrected.
                        // For now, always recalculate:
                        RecalculateDetailTotal(detail);
                    }
                    
                    // Recalculate subtotal from line items
                    var calculatedSubTotal = invoice.InvoiceDetails.Sum(d => d?.TotalCost ?? 0);
                    if (Math.Abs((invoice.SubTotal ?? 0) - calculatedSubTotal) > 0.01)
                    {
                        _logger.Information("Recalculating SubTotal for {InvoiceNo}: From {OldSubTotal:F2} to {NewSubTotal:F2} based on line items sum.", 
                            invoice.InvoiceNo, invoice.SubTotal ?? 0, calculatedSubTotal);
                        invoice.SubTotal = calculatedSubTotal;
                    }
                }

                // Recalculate invoice total using the same logic as TotalsZero for consistency
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
            // Basic calculation: Quantity * Cost - Discount
            // Ensure Qty and Cost are positive or handle appropriately
            var quantity = detail.Quantity > 0 ? detail.Quantity : 0;
            var cost = detail.Cost > 0 ? detail.Cost : 0; // Or handle negative costs if they are refunds etc.
            var discount = detail.Discount ?? 0;
            
            detail.TotalCost = (quantity * cost) - discount;
        }

        #endregion
    }
}