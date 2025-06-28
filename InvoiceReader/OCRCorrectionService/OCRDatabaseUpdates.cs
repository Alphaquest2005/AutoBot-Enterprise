// File: OCRCorrectionService/OCRDatabaseUpdates.cs
using OCR.Business.Entities;
using Serilog;
using Serilog.Events;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;

namespace WaterNut.DataSpace
{
    public partial class OCRCorrectionService
    {
        #region Main Database Update Methods

        public async Task UpdateRegexPatternsAsync(IEnumerable<RegexUpdateRequest> regexUpdateRequests)
        {
            using (var context = new OCRContext())
            {
                await UpdateRegexPatternsAsync(context, regexUpdateRequests).ConfigureAwait(false);
            }
        }

        public async Task UpdateRegexPatternsAsync(OCRContext context, IEnumerable<RegexUpdateRequest> regexUpdateRequests)
        {
            _logger.Error("🏁 **DB_UPDATE_ORCHESTRATOR_START (V3.1 - Enhanced Logging)**: Preparing to update database with {RequestCount} learning requests.", regexUpdateRequests?.Count() ?? 0);
            _logger.Error("   - **ARCHITECTURAL_INTENT**: To process a list of learning requests. Each request will be handled in an atomic transaction. If an 'omission' rule is successfully created, the orchestrator will immediately find and process its dependent 'format_correction' rule, ensuring correct linkage.");

            if (regexUpdateRequests == null || !regexUpdateRequests.Any())
            {
                _logger.Information("   - No requests provided. Exiting.");
                return;
            }

            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
                var serializedRequests = JsonSerializer.Serialize(regexUpdateRequests, options);
                _logger.Information("   - 🧬 **DATA_IN_DUMP (Full)**: Complete list of {Count} RegexUpdateRequest objects received by the orchestrator: {SerializedRequests}",
                    regexUpdateRequests.Count(), serializedRequests);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "🚨 **SERIALIZATION_ERROR**: Failed to serialize the incoming regexUpdateRequests list.");
            }

            _strategyFactory ??= new DatabaseUpdateStrategyFactory(_logger);

            var requestsToProcess = regexUpdateRequests.ToList();
            var processedIndexes = new HashSet<int>();

            for (int i = 0; i < requestsToProcess.Count; i++)
            {
                if (processedIndexes.Contains(i))
                {
                    _logger.Information("  - ⏭️ **SKIPPING_PROCESSED_REQUEST**: Request at index {Index} was already handled as part of a paired execution.", i);
                    continue;
                }

                var request = requestsToProcess[i];
                var dbUpdateResult = await ProcessSingleRequestAsync(context, request).ConfigureAwait(false);

                if (request.CorrectionType == "omission" && dbUpdateResult.IsSuccess && dbUpdateResult.RelatedRecordId.HasValue)
                {
                    int newFieldId = dbUpdateResult.RelatedRecordId.Value;
                    _logger.Information("  - 🤝 **PAIR_SEARCH**: Omission for '{FieldName}' succeeded. Searching for a paired 'format_correction' to link with new FieldId: {FieldId}", request.FieldName, newFieldId);

                    int formatRequestIndex = -1;
                    for (int j = i + 1; j < requestsToProcess.Count; j++)
                    {
                        if (processedIndexes.Contains(j)) continue;
                        var potentialPair = requestsToProcess[j];
                        if (potentialPair.CorrectionType == "format_correction" && potentialPair.FieldName == request.FieldName && potentialPair.LineNumber == request.LineNumber)
                        {
                            formatRequestIndex = j;
                            break;
                        }
                    }

                    if (formatRequestIndex != -1)
                    {
                        var formatRequest = requestsToProcess[formatRequestIndex];
                        processedIndexes.Add(formatRequestIndex);

                        _logger.Error("  - 🗣️ **PAIRED_EXECUTION_START**: Found paired 'format_correction' for '{FieldName}' at index {Index}. Injecting new FieldId: {FieldId} and processing immediately.", formatRequest.FieldName, formatRequestIndex, newFieldId);

                        formatRequest.FieldId = newFieldId;
                        await ProcessSingleRequestAsync(context, formatRequest);
                    }
                    else
                    {
                        _logger.Information("  - 🤷 **NO_PAIR_FOUND**: No corresponding 'format_correction' request found for the '{FieldName}' omission.", request.FieldName);
                    }
                }
            }
            _logger.Error("🏁 **DB_UPDATE_ORCHESTRATOR_COMPLETE**: Finished processing all requests.");
        }

        /// <summary>
        /// Helper method to process a single RegexUpdateRequest atomically.
        /// It contains the validation, strategy execution, and logging for one request.
        /// </summary>
        private async Task<DatabaseUpdateResult> ProcessSingleRequestAsync(OCRContext context, RegexUpdateRequest request)
        {
            _logger.Information("    - ▶️ **ATOMIC_PROCESS_START**: Field: '{FieldName}', Type: '{CorrectionType}'", request.FieldName, request.CorrectionType);
            _logger.Information("       - **ARCHITECTURAL_INTENT**: This block represents a single, self-contained unit of work that will be committed to the database in one transaction.");

            DatabaseUpdateResult result = null;
            try
            {
                var options = new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
                var serializedRequest = JsonSerializer.Serialize(request, options);
                _logger.Information("       - 🧬 **DATA_IN_DUMP (Single)**: Full RegexUpdateRequest for this operation: {SerializedRequest}", serializedRequest);

                // =============================== VALIDATION FIX IS HERE ===============================
                // This validation step is critical and has been restored.
                _logger.Information("       - 🛡️ **VALIDATING_REQUEST**: Checking if the request is valid before selecting a strategy.");
                var validationResult = this.ValidateUpdateRequest(request);
                if (!validationResult.IsValid)
                {
                    _logger.Warning("       - ❌ **VALIDATION_FAILED**: {ErrorMessage}", validationResult.ErrorMessage);
                    result = DatabaseUpdateResult.Failed($"Validation failed: {validationResult.ErrorMessage}");
                }
                else
                {
                    _logger.Information("       - ✅ **VALIDATION_PASSED**: Request is valid.");
                    var strategy = _strategyFactory.GetStrategy(request);
                    _logger.Information("       - ♟️ **STRATEGY_SELECTED**: Using '{StrategyType}' strategy.", strategy.GetType().Name);

                    // Each strategy's ExecuteAsync will call SaveChanges internally, ensuring an atomic operation.
                    result = await strategy.ExecuteAsync(context, request, this).ConfigureAwait(false);
                }
                // ======================================================================================
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "       - 🚨 **DEVIATION_FROM_EXPECTED (Exception)**: An unhandled exception occurred during the atomic process for field {FieldName}.", request.FieldName);
                result = DatabaseUpdateResult.Failed($"Atomic process exception: {ex.Message}", ex);
            }
            finally
            {
                result ??= DatabaseUpdateResult.Failed("Internal error: DBUpdateResult was unexpectedly null.");

                var outcome = result.IsSuccess ? "✅ SUCCESS" : "❌ FAILURE";
                var level = result.IsSuccess ? Serilog.Events.LogEventLevel.Information : Serilog.Events.LogEventLevel.Error;
                var serializedResult = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });

                _logger.Write(level, "    - 🏁 **ATOMIC_PROCESS_OUTCOME**: [{Outcome}] for Field '{FieldName}'.", outcome, request.FieldName);
                _logger.Write(level, "       - 🧬 **DATA_OUT_DUMP (Single)**: Full DatabaseUpdateResult from this operation: {SerializedResult}", serializedResult);

                await this.LogCorrectionLearningAsync(context, request, result).ConfigureAwait(false);
            }
            return result;
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
                var safeConfidence = (request.Confidence >= 0 && request.Confidence <= 1.0) ? Math.Round(request.Confidence, 4) : (double?)null;
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

               
                // 🔍 **ENHANCED_LOGGING**: Log the SuggestedRegex field before attempting to save
                _logger.Error("🔍 **LEARNING_RECORD_PREP**: Preparing OCRCorrectionLearning record for Field '{FieldName}'", request.FieldName);
                _logger.Error("   - **SuggestedRegex**: '{SuggestedRegex}'", request.SuggestedRegex ?? "NULL");
                _logger.Error("   - **Pattern**: '{Pattern}'", request.Pattern ?? "NULL");
                _logger.Error("   - **Replacement**: '{Replacement}'", request.Replacement ?? "NULL");
                _logger.Error("   - **CorrectionType**: '{CorrectionType}'", request.CorrectionType);
                
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
                                       // 🚨 **CRITICAL_MISSING_FIELD**: SuggestedRegex not being saved - will be lost!
                                       // TODO: Add SuggestedRegex = request.SuggestedRegex when database schema is updated
                                   };
                
                _logger.Error("🚨 **CRITICAL_ISSUE**: SuggestedRegex field '{SuggestedRegex}' will be LOST - not saved to database", request.SuggestedRegex ?? "NULL");

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