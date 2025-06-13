// File: OCRCorrectionService/OCRDatabaseUpdates.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OCR.Business.Entities;
using System.Text.RegularExpressions;
using System.Data.Entity;
using global::EntryDataDS.Business.Entities;
using Core.Common.Extensions;
using Serilog.Events;
using Serilog;

namespace WaterNut.DataSpace
{
    public partial class OCRCorrectionService
    {
        #region Main Database Update Methods

        public async Task UpdateRegexPatternsAsync(IEnumerable<RegexUpdateRequest> regexUpdateRequests)
        {
            if (regexUpdateRequests == null || !regexUpdateRequests.Any())
            {
                _logger?.Information("UpdateRegexPatternsAsync: No requests provided.");
                return;
            }

            _logger?.Information(
                "Starting database pattern updates for {RequestCount} requests.",
                regexUpdateRequests.Count());
            _strategyFactory ??= new DatabaseUpdateStrategyFactory(_logger);

            using var context = new OCRContext();
            foreach (var request in regexUpdateRequests)
            {
                DatabaseUpdateResult dbUpdateResult = null;
                IDatabaseUpdateStrategy strategy = null;
                try
                {
                    var validationResult = this.ValidateUpdateRequest(request);
                    if (!validationResult.IsValid)
                    {
                        dbUpdateResult =
                            DatabaseUpdateResult.Failed($"Validation failed: {validationResult.ErrorMessage}");
                    }
                    else
                    {
                        strategy = _strategyFactory.GetStrategy(request);
                        if (strategy != null)
                        {
                            dbUpdateResult = await strategy.ExecuteAsync(context, request, this).ConfigureAwait(false);
                        }
                        else
                        {
                            dbUpdateResult =
                                DatabaseUpdateResult.Failed($"No strategy for type '{request.CorrectionType}'");
                        }
                    }

                    if (dbUpdateResult.IsSuccess)
                    {
                        _logger?.Information(
                            "✅ DB_UPDATE_SUCCESS: Field '{FieldName}' updated via {StrategyType}.",
                            request.FieldName,
                            strategy?.StrategyType ?? "N/A");
                    }
                    else
                    {
                        _logger?.Warning(
                            "❌ DB_UPDATE_FAILED: Field '{FieldName}' using {StrategyType}: {Message}",
                            request.FieldName,
                            strategy?.StrategyType ?? "N/A",
                            dbUpdateResult.Message);
                    }
                }
                catch (Exception ex)
                {
                    _logger?.Error(ex, "Exception during DB update for field {FieldName}.", request.FieldName);
                    dbUpdateResult = DatabaseUpdateResult.Failed($"Outer exception: {ex.Message}", ex);
                }
                finally
                {
                    try
                    {
                        await this.LogCorrectionLearningAsync(context, request, dbUpdateResult).ConfigureAwait(false);
                    }
                    catch (Exception logEx)
                    {
                        _logger.Error(
                            logEx,
                            "CRITICAL: Failed to write to OCRCorrectionLearning audit log for field {FieldName}",
                            request?.FieldName);
                    }
                }
            }
        }

        #endregion

        #region Helper Methods

        private FieldValidationInfo ValidateUpdateRequest(RegexUpdateRequest request)
        {
            if (request == null)
                return new FieldValidationInfo { IsValid = false, ErrorMessage = "Request object is null." };
            if (string.IsNullOrWhiteSpace(request.FieldName))
                return new FieldValidationInfo { IsValid = false, ErrorMessage = "Field name is required." };
            if (!this.IsFieldSupported(request.FieldName))
                return new FieldValidationInfo
                           {
                               IsValid = false, ErrorMessage = $"Field '{request.FieldName}' is not supported."
                           };
            return new FieldValidationInfo { IsValid = true };
        }

        // File: OCRCorrectionService/OCRDatabaseUpdates.cs

        private async Task LogCorrectionLearningAsync(
            OCRContext context,
            RegexUpdateRequest request,
            DatabaseUpdateResult dbUpdateResult)
        {
            if (request == null) return;
            dbUpdateResult ??= DatabaseUpdateResult.Failed("Internal error: DBUpdateResult was null.");

            try
            {
                // Log the data being prepared for the database
                _logger.Information("📝 **LEARNING_LOG_PREP**: Preparing to log correction for Field '{FieldName}'.", request.FieldName);
                _logger.Verbose("  - OriginalError: '{OriginalError}'", request.OldValue);
                _logger.Verbose("  - CorrectValue: '{CorrectValue}'", request.NewValue);
                _logger.Verbose("  - CorrectionType: {CorrectionType}", request.CorrectionType);
                _logger.Verbose("  - Confidence (raw double): {ConfidenceDouble}", request.Confidence);

                // FIX: Correct range check for decimal(5, 4) and safe rounding.
                double? safeConfidence = (request.Confidence >= 0 && request.Confidence <= 1.0)
                                                ? (double?)Math.Round(request.Confidence, 4)
                                                : null;
                _logger.Information("  - Confidence (safe decimal?): {SafeConfidence}", safeConfidence?.ToString() ?? "null");

                var learning = new OCRCorrectionLearning
                {
                    FieldName = request.FieldName,
                    OriginalError = request.OldValue ?? "",
                    CorrectValue = request.NewValue ?? "",
                    LineNumber = request.LineNumber,
                    LineText = request.LineText ?? "",
                    WindowText = request.WindowText,
                    CorrectionType = request.CorrectionType,
                    DeepSeekReasoning = this.TruncateForLog(request.DeepSeekReasoning, 1000),
                    Confidence = safeConfidence,
                    InvoiceType = request.InvoiceType,
                    FilePath = request.FilePath,
                    Success = dbUpdateResult.IsSuccess,
                    ErrorMessage = dbUpdateResult.IsSuccess ? null : this.TruncateForLog(dbUpdateResult.Message, 2000),
                    CreatedBy = "OCRCorrectionService",
                    RequiresMultilineRegex = request.RequiresMultilineRegex,
                    ContextLinesBefore = request.ContextLinesBefore != null ? string.Join("\n", request.ContextLinesBefore) : null,
                    ContextLinesAfter = request.ContextLinesAfter != null ? string.Join("\n", request.ContextLinesAfter) : null,
                    LineId = request.LineId,
                    PartId = request.PartId,
                    RegexId = dbUpdateResult.IsSuccess ? dbUpdateResult.RecordId : request.RegexId
                };

                _logger.Information("  - Adding new OCRCorrectionLearning entity to context.");
                context.OCRCorrectionLearning.Add(learning);

                _logger.Information("  - Calling SaveChangesAsync() to commit learning record...");
                await context.SaveChangesAsync().ConfigureAwait(false);
                _logger.Information("✅ **LEARNING_LOG_SUCCESS**: Successfully saved learning record ID {LearningId} to database.", learning.Id);
            }
            catch (Exception ex)
            {
                // This log is critical. It exposes errors that were previously silent.
                _logger.Error(ex, "🚨 **LEARNING_LOG_FAILED**: CRITICAL - Failed to save OCRCorrectionLearning record for Field '{FieldName}'. This will prevent pattern learning.", request.FieldName);
                // Do not rethrow; the main pipeline should continue, but the error is now visible.
            }
        }

        #endregion
    }

}