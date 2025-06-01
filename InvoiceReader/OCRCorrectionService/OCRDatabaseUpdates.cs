// File: OCRCorrectionService/OCRDatabaseUpdates.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Data.Entity;
using OCR.Business.Entities;
using Serilog;
using System.Text.RegularExpressions; // Needed for Regex.IsMatch in ValidateUpdateRequest

namespace WaterNut.DataSpace
{
    public partial class OCRCorrectionService
    {
        // _strategyFactory is initialized in OCRCorrectionService.cs constructor

        #region Main Database Update Methods

        public async Task UpdateRegexPatternsAsync(
            IEnumerable<CorrectionResult> successfulCorrections,
            string fileText,
            string filePath = null,
            Dictionary<string, OCRFieldMetadata> invoiceMetadata = null)
        {
            if (successfulCorrections == null || !successfulCorrections.Any())
            {
                _logger?.Information("UpdateRegexPatternsAsync: No successful corrections provided for database pattern updates.");
                return;
            }

            _logger?.Information("Starting database pattern updates for {CorrectionCount} successful corrections.", successfulCorrections.Count());
            _strategyFactory ??= new DatabaseUpdateStrategyFactory(_logger);

            int dbSuccessCount = 0;
            int dbFailureCount = 0;
            int omissionPatternUpdates = 0;
            int formatPatternUpdates = 0;

            using var context = new OCRContext();

            foreach (var correction in successfulCorrections)
            {
                DatabaseUpdateResult dbUpdateResult = null;
                IDatabaseUpdateStrategy strategy = null;
                RegexUpdateRequest request = null;

                try
                {
                    // 1. Create the RegexUpdateRequest from the CorrectionResult and other context
                    //    CreateUpdateRequestForStrategy is an instance method in OCRCorrectionService.cs (main part)
                    request = this.CreateUpdateRequestForStrategy(correction,
                                                                  this.BuildLineContextForCorrection(correction, invoiceMetadata, fileText),
                                                                  filePath,
                                                                  fileText);

                    // 2. Validate the request for basic soundness before selecting a strategy
                    //    ValidateUpdateRequest is an instance method defined below in this file.
                    var validationResult = this.ValidateUpdateRequest(request);
                    if (!validationResult.IsValid)
                    {
                        _logger?.Warning("Invalid RegexUpdateRequest for field {FieldName}: {ErrorMessage}. Skipping DB update.", correction.FieldName, validationResult.ErrorMessage);
                        dbUpdateResult = DatabaseUpdateResult.Failed($"Validation failed: {validationResult.ErrorMessage}");
                        dbFailureCount++;
                        // LogCorrectionLearningAsync is an instance method defined below in this file.
                        await this.LogCorrectionLearningAsync(context, request, dbUpdateResult).ConfigureAwait(false);
                        continue;
                    }

                    // 3. Get the appropriate strategy
                    strategy = _strategyFactory.GetStrategy(correction);
                    if (strategy == null)
                    {
                        _logger?.Warning("No database update strategy found for correction type '{CorrectionType}' on field {FieldName}. Skipping DB update.",
                            correction.CorrectionType, correction.FieldName);
                        dbUpdateResult = DatabaseUpdateResult.Failed($"No strategy for type '{correction.CorrectionType}'");
                        dbFailureCount++;
                        await this.LogCorrectionLearningAsync(context, request, dbUpdateResult).ConfigureAwait(false);
                        continue;
                    }

                    // 4. Execute the strategy
                    _logger?.Information("Executing DB update strategy '{StrategyType}' for field {FieldName}.", strategy.StrategyType, correction.FieldName);
                    dbUpdateResult = await strategy.ExecuteAsync(context, request, this).ConfigureAwait(false);

                    // 5. Process the result of the strategy execution
                    if (dbUpdateResult.IsSuccess)
                    {
                        dbSuccessCount++;
                        if (strategy is OmissionUpdateStrategy) omissionPatternUpdates++;
                        else if (strategy is FieldFormatUpdateStrategy) formatPatternUpdates++;

                        _logger?.Information("Successfully updated database for field {FieldName} using {StrategyType}: {OperationDetails}",
                            correction.FieldName, strategy.StrategyType, dbUpdateResult.Message);
                    }
                    else
                    {
                        dbFailureCount++;
                        _logger?.Warning("Database update failed for field {FieldName} using {StrategyType}: {ErrorMessage}",
                            correction.FieldName, strategy.StrategyType, dbUpdateResult.Message);
                    }

                    // 6. Log to learning table
                    await this.LogCorrectionLearningAsync(context, request, dbUpdateResult).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    dbFailureCount++;
                    _logger?.Error(ex, "Exception during database update processing for field {FieldName}. Strategy: {StrategyType}",
                        correction.FieldName, strategy?.StrategyType ?? "Unknown");

                    if (request != null)
                    {
                        var exceptionResult = DatabaseUpdateResult.Failed($"Outer exception during DB update: {ex.Message}", ex);
                        await this.LogCorrectionLearningAsync(context, request, exceptionResult).ConfigureAwait(false);
                    }
                }
            }

            _logger?.Information("Database pattern updates completed: {SuccessCount} successful ({OmissionUpdates} omissions, {FormatUpdates} format/value), {FailureCount} failed.",
                dbSuccessCount, omissionPatternUpdates, formatPatternUpdates, dbFailureCount);
        }


        /// <summary>
        /// Builds a LineContext object for a given correction, using available metadata and file text.
        /// This is used to provide context to strategies, especially for omission handling.
        /// </summary>
        private LineContext BuildLineContextForCorrection(
            CorrectionResult correction,
            Dictionary<string, OCRFieldMetadata> invoiceMetadata,
            string fileText)
        {
            OCRFieldMetadata fieldMeta = null;
            if (invoiceMetadata != null)
            {
                if (invoiceMetadata.TryGetValue(correction.FieldName, out var directMeta))
                {
                    fieldMeta = directMeta;
                }
                else
                {
                    var mappedInfo = this.MapDeepSeekFieldToDatabase(correction.FieldName);
                    if (mappedInfo != null && invoiceMetadata.TryGetValue(mappedInfo.DatabaseFieldName, out var mappedMeta))
                    {
                        fieldMeta = mappedMeta;
                    }
                }
            }

            var lineContext = new LineContext
            {
                LineNumber = correction.LineNumber,
                LineText = correction.LineText ?? (correction.LineNumber > 0 ? this.GetOriginalLineText(fileText, correction.LineNumber) : null),
                ContextLinesBefore = correction.ContextLinesBefore,
                ContextLinesAfter = correction.ContextLinesAfter,
                RequiresMultilineRegex = correction.RequiresMultilineRegex,
                WindowText = (correction.LineNumber > 0) ? this.ExtractWindowText(fileText, correction.LineNumber, 5) : null
            };

            if (fieldMeta != null)
            {
                lineContext.LineId = fieldMeta.LineId;
                lineContext.RegexId = fieldMeta.RegexId;
                lineContext.RegexPattern = fieldMeta.LineRegex;
                lineContext.PartId = fieldMeta.PartId;
                lineContext.LineName = fieldMeta.LineName;
                lineContext.PartName = fieldMeta.PartName;
                lineContext.PartTypeId = fieldMeta.PartTypeId;
                // For FieldsInLine, the strategy would typically call GetFieldsByRegexNamedGroupsAsync if needed with lineContext.LineId
            }
            else if (correction.CorrectionType == "omission" || correction.CorrectionType == "omitted_line_item")
            {
                lineContext.IsOrphaned = true;
                lineContext.RequiresNewLineCreation = true;
            }

            return lineContext;
        }

        /// <summary>
        /// Validates a RegexUpdateRequest for essential data and conformity to field rules.
        /// </summary>
        private FieldValidationInfo ValidateUpdateRequest(RegexUpdateRequest request)
        {
            if (request == null) return new FieldValidationInfo { IsValid = false, ErrorMessage = "Request object is null." };
            if (string.IsNullOrWhiteSpace(request.FieldName))
                return new FieldValidationInfo { IsValid = false, ErrorMessage = "Field name is required in RegexUpdateRequest." };
            if (request.NewValue == null && request.CorrectionType != "removal")
                return new FieldValidationInfo { IsValid = false, ErrorMessage = "New value is required for non-removal corrections." };

            // IsFieldSupported and GetFieldValidationInfo are instance methods (likely in OCRFieldMapping.cs part)
            if (!this.IsFieldSupported(request.FieldName))
                return new FieldValidationInfo { IsValid = false, ErrorMessage = $"Field '{request.FieldName}' is not supported for database updates." };

            var fieldValidationRules = this.GetFieldValidationInfo(request.FieldName);
            if (!fieldValidationRules.IsValid) return fieldValidationRules; // Pass along error from GetFieldValidationInfo

            if (fieldValidationRules.IsRequired && string.IsNullOrWhiteSpace(request.NewValue))
                return new FieldValidationInfo { IsValid = false, ErrorMessage = $"Field '{request.FieldName}' is required and cannot be empty." };

            // Validate NewValue against pattern only if NewValue is not empty (empty might be a valid 'cleared' state)
            if (!string.IsNullOrEmpty(fieldValidationRules.ValidationPattern) && !string.IsNullOrWhiteSpace(request.NewValue))
            {
                if (!Regex.IsMatch(request.NewValue, fieldValidationRules.ValidationPattern))
                    return new FieldValidationInfo { IsValid = false, ErrorMessage = $"New value '{this.TruncateForLog(request.NewValue, 50)}' for '{request.FieldName}' does not match expected pattern: {fieldValidationRules.ValidationPattern}" };
            }
            return new FieldValidationInfo { IsValid = true, DatabaseFieldName = fieldValidationRules.DatabaseFieldName, EntityType = fieldValidationRules.EntityType };
        }

        /// <summary>
        /// Logs details of a correction attempt and its database update outcome to the OCRCorrectionLearning table.
        /// </summary>
        private async Task LogCorrectionLearningAsync(OCRContext context, RegexUpdateRequest request, DatabaseUpdateResult dbUpdateResult)
        {
            if (request == null)
            {
                _logger.Error("LogCorrectionLearningAsync: RegexUpdateRequest is null. Cannot log learning entry.");
                return;
            }
            if (dbUpdateResult == null)
            { // Should not happen if called correctly
                _logger.Error("LogCorrectionLearningAsync: DatabaseUpdateResult is null for field {FieldName}. Cannot log learning entry.", request.FieldName);
                dbUpdateResult = DatabaseUpdateResult.Failed("Internal error: DBUpdateResult was null.");
            }

            try
            {
                var learning = new OCRCorrectionLearning
                {
                    FieldName = request.FieldName,
                    OriginalError = request.OldValue ?? "",
                    CorrectValue = request.NewValue ?? "", // Ensure not null
                    LineNumber = request.LineNumber,
                    LineText = request.LineText ?? "",
                    WindowText = request.WindowText,
                    CorrectionType = request.CorrectionType,
                    DeepSeekReasoning = this.TruncateForLog(request.DeepSeekReasoning, 1000),
                    Confidence = request.Confidence >= -1000000 && request.Confidence <= 1000000 ? (decimal?)request.Confidence : null, // Range check for decimal
                    InvoiceType = request.InvoiceType,
                    FilePath = this.TruncateForLog(request.FilePath, 260),
                    Success = dbUpdateResult.IsSuccess,
                    ErrorMessage = dbUpdateResult.IsSuccess ? null : this.TruncateForLog(dbUpdateResult.Message, 2000),
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "OCRCorrectionService", // System identifier

                    RequiresMultilineRegex = request.RequiresMultilineRegex,
                    ContextLinesBefore = request.ContextLinesBefore != null ? string.Join("\n", request.ContextLinesBefore) : null,
                    ContextLinesAfter = request.ContextLinesAfter != null ? string.Join("\n", request.ContextLinesAfter) : null,
                    // LineId in request can be Fields.Id for FieldFormat strategy, or Lines.Id for Omission modify
                    LineId = request.LineId,
                    PartId = request.PartId,
                    // If DB update was successful and involved a Regex, log its ID.
                    RegexId = (dbUpdateResult.IsSuccess && dbUpdateResult.Operation != null && (dbUpdateResult.Operation.Contains("Regex") || dbUpdateResult.Operation.Contains("Pattern"))) ?
                                dbUpdateResult.RecordId : request.RegexId
                };
                context.OCRCorrectionLearning.Add(learning);
                await context.SaveChangesAsync().ConfigureAwait(false); // Save learning entry
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Failed to log correction learning entry for field {FieldName}. Error: {DbUpdateErrorMessage}", request.FieldName, dbUpdateResult.Message);
                // Do not re-throw, logging failure should not halt main processing.
            }
        }

        /// <summary>
        /// Creates a RegexUpdateRequest object populated from a CorrectionResult and other contextual information.
        /// This request object is then used by database update strategies.
        /// </summary>
        private RegexUpdateRequest CreateUpdateRequestForStrategy(
            CorrectionResult correction,
            LineContext lineContext, // Generated by BuildLineContextForCorrection
            string filePath,
            string fileText) // Full original file text
        {
            if (correction == null)
            {
                _logger.Error("CreateUpdateRequestForStrategy: CorrectionResult is null. Cannot create request.");
                return null; // Or throw argument null exception
            }
            if (lineContext == null)
            {
                // This might be acceptable for some corrections if they don't need line context for DB update,
                // but strategies for omissions or line modifications will likely require it.
                _logger.Warning("CreateUpdateRequestForStrategy: LineContext is null for field {FieldName}. Request may lack DB context.", correction.FieldName);
                // Create a minimal LineContext if it's absolutely null to avoid null refs,
                // though BuildLineContextForCorrection should ideally always return a (potentially sparse) object.
                lineContext = new LineContext
                {
                    LineNumber = correction.LineNumber,
                    LineText = correction.LineText,
                    ContextLinesBefore = correction.ContextLinesBefore,
                    ContextLinesAfter = correction.ContextLinesAfter,
                    RequiresMultilineRegex = correction.RequiresMultilineRegex
                };
            }


            var request = new RegexUpdateRequest
            {
                FieldName = correction.FieldName,
                OldValue = correction.OldValue,
                NewValue = correction.NewValue,
                CorrectionType = correction.CorrectionType,
                Confidence = correction.Confidence,
                DeepSeekReasoning = correction.Reasoning,

                LineNumber = correction.LineNumber, // From CorrectionResult, which should be authoritative
                LineText = correction.LineText,     // From CorrectionResult
                WindowText = (lineContext.LineNumber > 0 && !string.IsNullOrEmpty(fileText)) ?
                             this.ExtractWindowText(fileText, lineContext.LineNumber, 5) : // From OCRUtilities
                             (correction.LineNumber > 0 && !string.IsNullOrEmpty(fileText) ? this.ExtractWindowText(fileText, correction.LineNumber, 5) : null),
                ContextLinesBefore = correction.ContextLinesBefore,
                ContextLinesAfter = correction.ContextLinesAfter,
                RequiresMultilineRegex = correction.RequiresMultilineRegex,

                FilePath = filePath,
                InvoiceType = this.DetermineInvoiceType(filePath), // From OCRUtilities

                // Database context primarily from the passed LineContext
                LineId = lineContext.LineId,        // This is OCR.Business.Entities.Lines.Id from existing template line
                PartId = lineContext.PartId,        // OCR.Business.Entities.Parts.Id
                RegexId = lineContext.RegexId,      // OCR.Business.Entities.RegularExpressions.Id from existing template line
                ExistingRegex = lineContext.RegexPattern // Actual regex string of the existing line
            };

            // Special handling for FieldFormatUpdateStrategy:
            // It expects Fields.Id to be passed in request.LineId.
            // The BuildLineContextForCorrection might not have this specific Fields.Id directly,
            // as LineContext is about a "line of text".
            // The OCRFieldMetadata associated with the *specific field instance* being corrected would have Fields.Id.
            // This logic needs to be robust. If invoiceMetadata was passed down to here:
            // Dictionary<string, OCRFieldMetadata> invoiceMetadata = ... (would need to be parameter)
            // if (invoiceMetadata != null && 
            //     _strategyFactory.GetStrategy(correction) is FieldFormatUpdateStrategy && 
            //     invoiceMetadata.TryGetValue(correction.FieldName, out var fieldMetaForThisCorrection) &&
            //     fieldMetaForThisCorrection.FieldId.HasValue)
            // {
            //     request.LineId = fieldMetaForThisCorrection.FieldId; // Repurpose LineId for Fields.Id
            //      _logger.Debug("For FieldFormatStrategy on {FieldName}, setting request.LineId to Fields.Id: {FieldId}", 
            //          correction.FieldName, fieldMetaForThisCorrection.FieldId.Value);
            // }
            // For now, the caller (UpdateRegexPatternsAsync) will need to ensure that if the strategy
            // is FieldFormatUpdateStrategy, the `request.LineId` (originally from lineContext.LineId)
            // is appropriately set to the `Fields.Id` if that's the convention.
            // The current BuildLineContextForCorrection tries to set LineId from OCRFieldMetadata.LineId.
            // A cleaner way is for CreateUpdateRequestForStrategy to take invoiceMetadata.

            return request;
        }


        #endregion
    }
}