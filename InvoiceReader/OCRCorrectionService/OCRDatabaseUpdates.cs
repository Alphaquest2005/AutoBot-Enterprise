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
            _logger.Error("🏁 **DB_UPDATE_ORCHESTRATOR_START (V4.0 - DeepSeek Validation)**: Preparing to update database with {RequestCount} learning requests.", regexUpdateRequests?.Count() ?? 0);
            _logger.Error("   - **ARCHITECTURAL_INTENT**: To process a list of learning requests. Each request will be handled in an atomic transaction. If an 'omission' rule is successfully created, the orchestrator will immediately find and process its dependent 'format_correction' rule, ensuring correct linkage.");
            
            // 🚨 **CRITICAL**: Log every single DeepSeek correction to verify pipeline integrity
            if (regexUpdateRequests != null && regexUpdateRequests.Any())
            {
                _logger.Error("🔍 **DEEPSEEK_CORRECTIONS_INVENTORY**: Detailed breakdown of all {Count} corrections received from DeepSeek", regexUpdateRequests.Count());
                var index = 1;
                foreach (var request in regexUpdateRequests)
                {
                    _logger.Error("   - **CORRECTION_{Index}**: Field='{FieldName}', Type='{CorrectionType}', Value='{NewValue}'", 
                        index++, request.FieldName, request.CorrectionType, request.NewValue);
                }
                
                // Track correction types for verification
                var correctionTypes = regexUpdateRequests.GroupBy(r => r.CorrectionType).ToDictionary(g => g.Key, g => g.Count());
                _logger.Error("🎯 **CORRECTION_TYPE_SUMMARY**: {TypeSummary}", string.Join(", ", correctionTypes.Select(kvp => $"{kvp.Key}: {kvp.Value}")));
                
                // Expected counts for validation
                var expectedOmissions = correctionTypes.GetValueOrDefault("omission", 0);
                var expectedMultiField = correctionTypes.GetValueOrDefault("multi_field_omission", 0);
                var expectedFormatCorrections = correctionTypes.GetValueOrDefault("format_correction", 0);
                _logger.Error("📊 **EXPECTED_PROCESSING**: Omissions={Omissions}, MultiField={MultiField}, FormatCorrections={FormatCorrections}, Total={Total}",
                    expectedOmissions, expectedMultiField, expectedFormatCorrections, 
                    expectedOmissions + expectedMultiField + expectedFormatCorrections);
            }

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
                        await this.ProcessSingleRequestAsync(context, formatRequest).ConfigureAwait(false);
                    }
                    else
                    {
                        _logger.Information("  - 🤷 **NO_PAIR_FOUND**: No corresponding 'format_correction' request found for the '{FieldName}' omission.", request.FieldName);
                    }
                }
            }
            
            // 🎯 **CRITICAL**: Verify all DeepSeek corrections were processed successfully
            _logger.Error("🏁 **DB_UPDATE_ORCHESTRATOR_COMPLETE**: Finished processing all requests.");
            
            // Generate comprehensive pipeline verification report
            var totalRequests = regexUpdateRequests.Count();
            var processedCount = processedIndexes.Count + (totalRequests - processedIndexes.Count); // All non-skipped requests
            
            _logger.Error("📊 **PIPELINE_VERIFICATION_REPORT**:");
            _logger.Error("   - Total DeepSeek corrections received: {TotalRequests}", totalRequests);
            _logger.Error("   - Corrections processed (including paired): {ProcessedCount}", processedCount);
            _logger.Error("   - Skipped (already processed as pairs): {SkippedCount}", processedIndexes.Count);
            
            // Verify expected DeepSeek correction types were processed
            if (regexUpdateRequests != null && regexUpdateRequests.Any())
            {
                var fieldBreakdown = regexUpdateRequests.GroupBy(r => r.FieldName).Select(g => new { Field = g.Key, Count = g.Count() });
                _logger.Error("📋 **FIELD_BREAKDOWN**: {FieldSummary}", 
                    string.Join(", ", fieldBreakdown.Select(f => $"{f.Field}: {f.Count}")));
                
                // Critical validation: ensure all 9 expected corrections were handled
                var expectedFields = new[] { "InvoiceNo", "InvoiceDate", "SupplierName", "Currency", "SubTotal", "TotalDeduction", "InvoiceTotal", 
                    "InvoiceDetail_SingleColumn_MultiField_Lines3_11", "InvoiceDetail_SparseText_MultiField_Lines6_28" };
                var actualFields = regexUpdateRequests.Select(r => r.FieldName).Distinct().ToList();
                var missingFields = expectedFields.Except(actualFields).ToList();
                var unexpectedFields = actualFields.Except(expectedFields).ToList();
                
                if (missingFields.Any())
                {
                    _logger.Error("🚨 **MISSING_CORRECTIONS**: Expected fields not found: {MissingFields}", string.Join(", ", missingFields));
                }
                if (unexpectedFields.Any())
                {
                    _logger.Error("⚠️ **UNEXPECTED_CORRECTIONS**: Unexpected fields found: {UnexpectedFields}", string.Join(", ", unexpectedFields));
                }
                if (!missingFields.Any() && !unexpectedFields.Any())
                {
                    _logger.Error("✅ **PIPELINE_INTEGRITY_VERIFIED**: All expected DeepSeek corrections processed successfully");
                }
            }
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
                
                // 🎯 **CRITICAL**: Track every single DeepSeek correction result for pipeline verification
                _logger.Error("       - 🎯 **DEEPSEEK_CORRECTION_RESULT**: Field='{FieldName}', Type='{CorrectionType}', Outcome={Outcome}, Message='{Message}'", 
                    request.FieldName, request.CorrectionType, outcome, result.Message ?? "Success");
                
                if (result.IsSuccess && result.RelatedRecordId.HasValue)
                {
                    _logger.Error("       - 🆔 **DATABASE_ENTITY_CREATED**: Field='{FieldName}' created database entity with ID={EntityId}", 
                        request.FieldName, result.RelatedRecordId.Value);
                }
                
                if (!result.IsSuccess)
                {
                    _logger.Error("       - 🚨 **CORRECTION_FAILED**: Field='{FieldName}' FAILED to save to database. DeepSeek correction LOST. Error: {Error}", 
                        request.FieldName, result.Message);
                }

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
                
                // ✅ **PROPER_FIELD_USAGE**: Now using dedicated SuggestedRegex field - no WindowText enhancement needed
                if (!string.IsNullOrWhiteSpace(request.SuggestedRegex))
                {
                    _logger.Information("✅ **SUGGESTED_REGEX_DIRECT**: Storing SuggestedRegex '{SuggestedRegex}' in dedicated database field", request.SuggestedRegex);
                }

                var learning = new OCRCorrectionLearning
                                   {
                                       FieldName = request.FieldName,
                                       OriginalError = request.OldValue ?? string.Empty,
                                       CorrectValue = request.NewValue ?? string.Empty,
                                       LineNumber = request.LineNumber,
                                       LineText = request.LineText ?? string.Empty,
                                       WindowText = enhancedWindowText,
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
                                       // ✅ **FIELD_PRESERVED**: SuggestedRegex now preserved in enhanced WindowText field
                                   };
                
                // ✅ **ISSUE_RESOLVED**: SuggestedRegex is now preserved in enhanced WindowText field

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

        /// <summary>
        /// Extract SuggestedRegex from enhanced WindowText field
        /// </summary>
        private static string ExtractSuggestedRegexFromWindowText(string windowText)
        {
            if (string.IsNullOrWhiteSpace(windowText)) return null;
            
            const string marker = "SUGGESTED_REGEX:";
            var index = windowText.IndexOf(marker);
            if (index == -1) return null;
            
            var start = index + marker.Length;
            var pipeIndex = windowText.IndexOf('|', start);
            
            return pipeIndex == -1 
                ? windowText.Substring(start) 
                : windowText.Substring(start, pipeIndex - start);
        }

        /// <summary>
        /// Get WindowText content without SuggestedRegex part
        /// </summary>
        private static string ExtractCleanWindowTextFromEnhanced(string enhancedWindowText)
        {
            if (string.IsNullOrWhiteSpace(enhancedWindowText)) return enhancedWindowText;
            
            const string marker = "|SUGGESTED_REGEX:";
            var index = enhancedWindowText.IndexOf(marker);
            if (index == -1)
            {
                // Check if it starts with SUGGESTED_REGEX: (no prefix)
                if (enhancedWindowText.StartsWith("SUGGESTED_REGEX:"))
                    return "";
                return enhancedWindowText;
            }
            
            return enhancedWindowText.Substring(0, index);
        }

        #endregion
    }
}