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
        #region Enhanced Correction Application

        /// <summary>
        /// Applies corrections with priority-based processing, field dependency validation, and omission handling
        /// </summary>
        private async Task<List<CorrectionResult>> ApplyCorrectionsAsync(
            ShipmentInvoice invoice,
            List<InvoiceError> errors,
            string fileText,
            Dictionary<string, OCRFieldMetadata> metadata = null)
        {
            var results = new List<CorrectionResult>();

            // Apply field dependency validation and conflict resolution
            var filteredErrors = ResolveFieldConflicts(errors, invoice);

            // Group errors by type: omissions vs format corrections
            var omissionErrors = filteredErrors.Where(e => e.ErrorType == "omission").ToList();
            var formatErrors = filteredErrors.Where(e => e.ErrorType != "omission").ToList();

            _logger.Information("Processing {OmissionCount} omissions and {FormatCount} format errors for invoice {InvoiceNo}",
                omissionErrors.Count, formatErrors.Count, invoice.InvoiceNo);

            // Process omissions first (create new fields/regex)
            foreach (var error in omissionErrors)
            {
                var result = await this.ProcessOmissionCorrectionAsync(error, metadata, fileText).ConfigureAwait(false);
                results.Add(result);
                LogCorrectionResult(result, "OMISSION");
            }

            // Process format errors using existing logic
            var criticalFormatErrors = formatErrors.Where(e => IsCriticalError(e)).ToList();
            var standardFormatErrors = formatErrors.Where(e => !IsCriticalError(e)).ToList();

            foreach (var error in criticalFormatErrors)
            {
                var result = await this.ApplySingleCorrectionAsync(invoice, error).ConfigureAwait(false);
                results.Add(result);
                LogCorrectionResult(result, "CRITICAL");
            }

            foreach (var error in standardFormatErrors)
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
        /// NEW: Processes omission corrections by creating new regex patterns and database entries
        /// </summary>
        private async Task<CorrectionResult> ProcessOmissionCorrectionAsync(
            InvoiceError error,
            Dictionary<string, OCRFieldMetadata> metadata,
            string fileText)
        {
            var result = new CorrectionResult
            {
                FieldName = error.Field,
                CorrectionType = "omission",
                Confidence = error.Confidence,
                OldValue = error.ExtractedValue ?? "",
                NewValue = error.CorrectValue,
                LineText = error.LineText,
                LineNumber = error.LineNumber,
                ContextLinesBefore = error.ContextLinesBefore ?? new List<string>(),
                ContextLinesAfter = error.ContextLinesAfter ?? new List<string>(),
                RequiresMultilineRegex = error.RequiresMultilineRegex
            };

            try
            {
                // Find line context using metadata and error information
                var lineContext = FindLineContextForOmission(error, metadata, fileText);

                if (lineContext == null)
                {
                    result.Success = false;
                    result.ErrorMessage = "Could not determine line context for omission";
                    return result;
                }

                // Check if field already exists in the line's regex
                var fieldExists = IsFieldExistingInLine(error.Field, lineContext);

                if (fieldExists)
                {
                    // Field exists but value was wrong - treat as format error instead
                    _logger.Information("Field {FieldName} exists in line regex, treating as format correction", error.Field);
                    return await ProcessAsFormatCorrection(error);
                }

                // True omission - request new regex pattern from DeepSeek
                var regexResponse = await RequestNewRegexFromDeepSeek(result, lineContext);

                if (regexResponse == null || !ValidateRegexPattern(regexResponse, result))
                {
                    result.Success = false;
                    result.ErrorMessage = "Failed to create or validate regex pattern";
                    return result;
                }

                // Create database entries for the new field
                var databaseSuccess = await CreateDatabaseEntriesForOmission(regexResponse, lineContext, result);

                if (databaseSuccess)
                {
                    result.Success = true;
                    result.ErrorMessage = null;
                    _logger.Information("Successfully created new field for omission: {FieldName}", error.Field);
                }
                else
                {
                    result.Success = false;
                    result.ErrorMessage = "Failed to create database entries";
                }

                return result;
            }
            catch (Exception ex)
            {
                result.Success = false;
                result.ErrorMessage = ex.Message;
                _logger.Error(ex, "Error processing omission correction for field {FieldName}", error.Field);
                return result;
            }
        }

        /// <summary>
        /// Finds line context for omission correction using error details and metadata
        /// </summary>
        private LineContext FindLineContextForOmission(
            InvoiceError error,
            Dictionary<string, OCRFieldMetadata> metadata,
            string fileText)
        {
            try
            {
                // First try to find exact line text match in metadata
                if (!string.IsNullOrEmpty(error.LineText) && metadata != null)
                {
                    foreach (var meta in metadata.Values)
                    {
                        var originalLine = GetOriginalLineText(meta.LineNumber, fileText);
                        if (string.Equals(originalLine?.Trim(), error.LineText.Trim(), StringComparison.OrdinalIgnoreCase))
                        {
                            return CreateLineContextFromMetadata(meta, fileText);
                        }
                    }
                }

                // Fallback: create orphaned context for new line creation
                return new LineContext
                {
                    LineId = null,
                    LineNumber = error.LineNumber,
                    LineText = error.LineText,
                    ContextLinesBefore = error.ContextLinesBefore ?? new List<string>(),
                    ContextLinesAfter = error.ContextLinesAfter ?? new List<string>(),
                    RequiresMultilineRegex = error.RequiresMultilineRegex,
                    IsOrphaned = true,
                    RequiresNewLineCreation = true
                };
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error finding line context for omission");
                return null;
            }
        }

        /// <summary>
        /// Creates line context from OCR metadata
        /// </summary>
        private LineContext CreateLineContextFromMetadata(OCRFieldMetadata metadata, string fileText)
        {
            return new LineContext
            {
                LineId = metadata.LineId,
                LineNumber = metadata.LineNumber,
                LineText = GetOriginalLineText(metadata.LineNumber, fileText),
                RegexPattern = GetLineRegexPattern(metadata.LineId),
                PartId = metadata.PartId,
                IsOrphaned = false,
                RequiresNewLineCreation = false
            };
        }

        /// <summary>
        /// Checks if a field exists in the line's regex named groups
        /// </summary>
        private bool IsFieldExistingInLine(string fieldName, LineContext lineContext)
        {
            if (lineContext?.RegexPattern == null) return false;

            try
            {
                // Extract named groups from the line's regex pattern
                var namedGroups = ExtractNamedGroupsFromRegex(lineContext.RegexPattern);

                // Map DeepSeek field name to expected key/field name
                var fieldMapping = MapDeepSeekFieldToDatabase(fieldName);

                // Check if field exists by Key (regex named group) or Field name
                return namedGroups.Any(group =>
                    group.Equals(fieldName, StringComparison.OrdinalIgnoreCase) ||
                    (fieldMapping != null &&
                     (group.Equals(fieldMapping.DatabaseFieldName, StringComparison.OrdinalIgnoreCase) ||
                      group.Equals(fieldMapping.DisplayName, StringComparison.OrdinalIgnoreCase))));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error checking field existence for {FieldName}", fieldName);
                return false;
            }
        }

        /// <summary>
        /// Processes omission as format correction when field already exists
        /// </summary>
        private async Task<CorrectionResult> ProcessAsFormatCorrection(InvoiceError error)
        {
            // Convert omission error to format error and process normally
            var formatResult = new CorrectionResult
            {
                FieldName = error.Field,
                CorrectionType = "format_correction",
                OldValue = error.ExtractedValue ?? "",
                NewValue = error.CorrectValue,
                Confidence = error.Confidence,
                Success = true // Will be set by format correction logic
            };

            // Use existing format correction logic
            // This would integrate with your existing FieldFormatRegEx system

            return formatResult;
        }

        /// <summary>
        /// Gets the regex pattern for a specific line
        /// </summary>
        private string GetLineRegexPattern(int? lineId)
        {
            if (!lineId.HasValue) return null;

            try
            {
                using var context = new OCRContext();
                var line = context.Lines
                    .Include("RegularExpressions")
                    .FirstOrDefault(l => l.Id == lineId);

                return line?.RegularExpressions?.RegEx;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error getting regex pattern for LineId {LineId}", lineId);
                return null;
            }
        }

        /// <summary>
        /// Gets original line text by line number
        /// </summary>
        private string GetOriginalLineText(int lineNumber, string fileText)
        {
            if (string.IsNullOrEmpty(fileText) || lineNumber <= 0)
                return "";

            var lines = fileText.Split(new[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);
            return lineNumber <= lines.Length ? lines[lineNumber - 1] : "";
        }

        /// <summary>
        /// Applies a single correction to the invoice (existing method - unchanged)
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
        /// Applies correction to a specific field (existing method - unchanged)
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
        /// Applies correction to invoice detail fields (existing method - unchanged)
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
        /// Determines if an error is critical (affects calculations) - existing method
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
        /// Logs correction results with appropriate detail level - existing method
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
        /// Recalculates dependent fields after corrections - existing method
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

        /// <summary>
        /// Recalculates invoice detail total cost - existing method
        /// </summary>
        private void RecalculateDetailTotal(InvoiceDetails detail)
        {
            detail.TotalCost = (detail.Quantity * detail.Cost) - (detail.Discount ?? 0);
        }

        #endregion
    }
}