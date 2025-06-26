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
using Serilog;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Data.Entity.Validation;

namespace WaterNut.DataSpace
{
    public partial class OCRCorrectionService
    {
        #region Main Database Update Methods

        // File: OCRDatabaseUpdates.cs

        public async Task UpdateRegexPatternsAsync(IEnumerable<RegexUpdateRequest> regexUpdateRequests)
        {
            using (var context = new OCRContext())
            {
                await UpdateRegexPatternsAsync(context, regexUpdateRequests).ConfigureAwait(false);
            }
        }

        public async Task UpdateRegexPatternsAsync(OCRContext context, IEnumerable<RegexUpdateRequest> regexUpdateRequests)
        {
            // --- MANDATE LOG: Serialize the entire input collection to this method ---
            if (regexUpdateRequests != null)
            {
                try
                {
                    var options = new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
                    var serializedRequests = JsonSerializer.Serialize(regexUpdateRequests, options);
                    _logger.Debug("   - [DB_LEARNING_INPUT_DUMP]: Full list of {Count} RegexUpdateRequest objects received: {SerializedRequests}",
                        regexUpdateRequests.Count(), serializedRequests);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "🚨 **SERIALIZATION_ERROR**: Failed to serialize the incoming regexUpdateRequests list.");
                }
            }
            // --- END MANDATE LOG ---

            if (regexUpdateRequests == null || !regexUpdateRequests.Any())
            {
                _logger?.Information("UpdateRegexPatternsAsync: No requests provided.");
                return;
            }

            _logger?.Information("Starting database pattern updates for {RequestCount} requests.", regexUpdateRequests.Count());
            _strategyFactory ??= new DatabaseUpdateStrategyFactory(_logger);

            var requestsToProcess = regexUpdateRequests.ToList();
            var processedIndexes = new HashSet<int>();

            for (int i = 0; i < requestsToProcess.Count; i++)
            {
                if (processedIndexes.Contains(i)) continue;

                var request = requestsToProcess[i];

                DatabaseUpdateResult dbUpdateResult = null;
                try
                {
                    _logger.Information("  - Processing request for Field: '{FieldName}', Type: '{CorrectionType}'", request.FieldName, request.CorrectionType);
                    var validationResult = this.ValidateUpdateRequest(request);
                    if (!validationResult.IsValid)
                    {
                        dbUpdateResult = DatabaseUpdateResult.Failed($"Validation failed: {validationResult.ErrorMessage}");
                    }
                    else
                    {
                        var strategy = _strategyFactory.GetStrategy(request);
                        if (strategy != null)
                        {
                            dbUpdateResult = await strategy.ExecuteAsync(context, request, this).ConfigureAwait(false);

                            if (request.CorrectionType == "omission" && dbUpdateResult.IsSuccess && dbUpdateResult.RelatedRecordId.HasValue)
                            {
                                int newFieldId = dbUpdateResult.RelatedRecordId.Value;

                                var formatCorrectionRequestIndex = requestsToProcess.FindIndex(i + 1, r =>
                                    r.CorrectionType == "format_correction" &&
                                    r.FieldName == request.FieldName &&
                                    r.LineNumber == request.LineNumber &&
                                    !r.LineId.HasValue);

                                if (formatCorrectionRequestIndex != -1)
                                {
                                    var formatRequest = requestsToProcess[formatCorrectionRequestIndex];
                                    processedIndexes.Add(formatCorrectionRequestIndex);

                                    _logger.Error("  - 🗣️ **PAIRED_EXECUTION**: Found paired 'format_correction' for '{FieldName}'. Injecting new FieldId: {FieldId} and processing immediately within the same transaction.", formatRequest.FieldName, newFieldId);

                                    //formatRequest.LineId = newFieldId; // <-- OLD, CONFUSING WAY
                                    formatRequest.FieldId = newFieldId; // <-- NEW, EXPLICIT WAY

                                    var formatStrategy = _strategyFactory.GetStrategy(formatRequest);

                                    DatabaseUpdateResult formatResult;
                                    try
                                    {
                                        // Clear any pending Entity Framework changes to avoid relationship conflicts
                                        context.ChangeTracker.DetectChanges();
                                        _logger.Information("  - 🔄 **PAIRED_EXECUTION_PREP**: Context state validated before format_correction execution.");
                                        
                                        formatResult = await formatStrategy.ExecuteAsync(context, formatRequest, this).ConfigureAwait(false);
                                    }
                                    catch (System.Data.Entity.Infrastructure.DbUpdateException ex) when (ex.InnerException?.Message?.Contains("principal end") == true || ex.InnerException?.Message?.Contains("primary key") == true)
                                    {
                                        _logger.Warning("  - ⚠️ **PAIRED_EXECUTION_CONFLICT**: Entity Framework relationship conflict during paired execution. Attempting recovery...");
                                        
                                        // Detach any problematic entities from the context
                                        var addedEntities = context.ChangeTracker.Entries().Where(e => e.State == EntityState.Added).ToList();
                                        foreach (var entity in addedEntities)
                                        {
                                            _logger.Information("  - 🔄 Detaching entity: {EntityType}", entity.Entity.GetType().Name);
                                            entity.State = EntityState.Detached;
                                        }
                                        
                                        // Retry the format correction with a clean context state
                                        formatResult = await formatStrategy.ExecuteAsync(context, formatRequest, this).ConfigureAwait(false);
                                        _logger.Information("  - ✅ **PAIRED_EXECUTION_RECOVERY**: Format correction succeeded after recovery.");
                                    }

                                    var formatOutcome = formatResult.IsSuccess ? "SUCCESS" : "FAILURE";
                                    var formatLogLevel = formatResult.IsSuccess ? Serilog.Events.LogEventLevel.Information : Serilog.Events.LogEventLevel.Error;
                                    _logger.Write(formatLogLevel, "  - 🏁 **STRATEGY_OUTCOME (Paired)**: [{Outcome}] for Field '{FieldName}'. Message: {Message}",
                                        formatOutcome, formatRequest.FieldName, formatResult.Message);

                                    await this.LogCorrectionLearningAsync(context, formatRequest, formatResult).ConfigureAwait(false);
                                }
                            }
                        }
                        else
                        {
                            dbUpdateResult = DatabaseUpdateResult.Failed($"No strategy for type '{request.CorrectionType}'");
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger?.Error(ex, "Exception during DB update for field {FieldName}.", request.FieldName);
                    dbUpdateResult = DatabaseUpdateResult.Failed($"Outer exception: {ex.Message}", ex);
                }
                finally
                {
                    if (dbUpdateResult != null)
                    {
                        var outcome = dbUpdateResult.IsSuccess ? "SUCCESS" : "FAILURE";
                        var level = dbUpdateResult.IsSuccess ? Serilog.Events.LogEventLevel.Information : Serilog.Events.LogEventLevel.Error;
                        _logger.Write(level, "  - 🏁 **STRATEGY_OUTCOME**: [{Outcome}] for Field '{FieldName}'. Message: {Message}",
                            outcome, request.FieldName, dbUpdateResult.Message);
                    }
                    else
                    {
                        _logger.Error("  - 🏁 **STRATEGY_OUTCOME**: [UNKNOWN_FAILURE] for Field '{FieldName}'. The dbUpdateResult was unexpectedly null.", request.FieldName);
                    }

                    try
                    {
                        await this.LogCorrectionLearningAsync(context, request, dbUpdateResult).ConfigureAwait(false);
                    }
                    catch (Exception logEx)
                    {
                        _logger.Error(logEx, "CRITICAL: Failed to write to OCRCorrectionLearning audit log for field {FieldName}", request?.FieldName);
                    }
                }
            }
        }

        #endregion

        #region Helper Methods

        public FieldValidationInfo GetFieldValidationInfo(string rawFieldName)
        {
            var fieldInfo = this.MapDeepSeekFieldToDatabase(rawFieldName);
            if (fieldInfo == null)
                return new FieldValidationInfo { IsValid = false, ErrorMessage = $"Field '{rawFieldName}' is unknown or not mapped." };

            return new FieldValidationInfo { IsValid = true, };
        }

        public bool IsFieldSupported(string rawFieldName)
        {
            if (string.IsNullOrWhiteSpace(rawFieldName)) return false;
            return DeepSeekToDBFieldMapping.ContainsKey(rawFieldName.Trim());
        }

        private FieldValidationInfo ValidateUpdateRequest(RegexUpdateRequest request)
        {
            if (request == null) return new FieldValidationInfo { IsValid = false, ErrorMessage = "Request object is null." };
            if (string.IsNullOrWhiteSpace(request.FieldName)) return new FieldValidationInfo { IsValid = false, ErrorMessage = "Field name is required." };
            if (!this.IsFieldSupported(request.FieldName)) return new FieldValidationInfo { IsValid = false, ErrorMessage = $"Field '{request.FieldName}' is not supported." };
            return new FieldValidationInfo { IsValid = true };
        }

        private async Task LogCorrectionLearningAsync(
            OCRContext context,
            RegexUpdateRequest request,
            DatabaseUpdateResult dbUpdateResult)
        {
            if (request == null)
            {
                _logger.Error("🚨 **LEARNING_LOG_FAILED**: Attempted to log a null RegexUpdateRequest.");
                return;
            }

            dbUpdateResult ??= DatabaseUpdateResult.Failed("Internal error: DBUpdateResult was null.");

            try
            {
                // Step 1: Prepare and validate the data *before* any logging.
                double? safeConfidence = (request.Confidence >= 0 && request.Confidence <= 1.0)
                                             ? Math.Round(request.Confidence, 4)
                                             : (double?)null;

                // Step 2: Log the prepared, safe-to-log data.
                _logger.Information(
                    "📝 **LEARNING_LOG_PREP**: Field='{FieldName}', Type='{CorrectionType}', NewValue='{NewValue}', Confidence={Confidence}, Success={IsSuccess}, Message='{Message}'",
                    request.FieldName,
                    request.CorrectionType,
                    request.NewValue,
                    safeConfidence?.ToString(System.Globalization.CultureInfo.InvariantCulture) ?? "null",
                    dbUpdateResult.IsSuccess.ToString(),
                    dbUpdateResult.Message
                );

                // Step 3: Create the entity for the database.

                var learning = new OCRCorrectionLearning
                {
                    FieldName = request.FieldName,
                    OriginalError = request.OldValue ?? string.Empty,
                    CorrectValue = request.NewValue ?? string.Empty,
                    LineNumber = request.LineNumber,
                    LineText = request.LineText ?? string.Empty,
                    WindowText = request.WindowText,
                    CorrectionType = request.CorrectionType,
                    DeepSeekReasoning = TruncateForLog(request.DeepSeekReasoning, 1000),
                    Confidence = safeConfidence,
                    InvoiceType = request.InvoiceType,
                    FilePath = request.FilePath,
                    Success = dbUpdateResult.IsSuccess,
                    ErrorMessage = dbUpdateResult.IsSuccess ? null : TruncateForLog(dbUpdateResult.Message, 2000),
                    CreatedBy = "OCRCorrectionService",
                    CreatedDate = DateTime.Now,
                    RequiresMultilineRegex = request.RequiresMultilineRegex,
                    ContextLinesBefore = request.ContextLinesBefore != null ? string.Join("\n", request.ContextLinesBefore) : null,
                    ContextLinesAfter = request.ContextLinesAfter != null ? string.Join("\n", request.ContextLinesAfter) : null,
                    LineId = request.LineId,
                    PartId = request.PartId,
                    RegexId = dbUpdateResult.IsSuccess ? dbUpdateResult.RecordId : request.RegexId,
                    //InvoiceNumber = request.InvoiceNumber
                };

                context.OCRCorrectionLearning.Add(learning);
                await context.SaveChangesAsync().ConfigureAwait(false);
                _logger.Information("✅ **LEARNING_LOG_SUCCESS**: Successfully saved learning record ID {LearningId} for Field '{FieldName}'.", learning.Id, learning.FieldName);
            }
            catch (DbEntityValidationException vex)
            {
                var errorMessages = vex.EntityValidationErrors
                    .SelectMany(x => x.ValidationErrors)
                    .Select(x => $"{x.PropertyName}: {x.ErrorMessage}");
                var fullErrorMessage = string.Join("; ", errorMessages);
                _logger.Error(vex, "🚨 **LEARNING_LOG_DB_VALIDATION_FAILED**: CRITICAL - DbEntityValidationException while saving record for Field '{FieldName}'. Errors: {ValidationErrors}", request.FieldName, fullErrorMessage);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "🚨 **LEARNING_LOG_FAILED**: CRITICAL - Unhandled exception while saving OCRCorrectionLearning record for Field '{FieldName}'.", request.FieldName);
            }
        }

        #endregion
    }
}