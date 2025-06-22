// File: OCRCorrectionService/OCRDatabaseStrategies.cs
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using OCR.Business.Entities;
using Serilog;
using TrackableEntities;
using Newtonsoft.Json;

namespace WaterNut.DataSpace
{
    public partial class OCRCorrectionService
    {
        #region Strategy Interfaces and Base Classes

        public interface IDatabaseUpdateStrategy
        {
            string StrategyType { get; }
            Task<DatabaseUpdateResult> ExecuteAsync(OCRContext context, RegexUpdateRequest request, OCRCorrectionService serviceInstance);
            bool CanHandle(RegexUpdateRequest request);
        }

        public abstract class DatabaseUpdateStrategyBase : IDatabaseUpdateStrategy
        {
            protected readonly ILogger _logger;

            protected DatabaseUpdateStrategyBase(ILogger logger)
            {
                _logger = logger ?? Log.Logger;
            }

            public abstract string StrategyType { get; }
            public abstract Task<DatabaseUpdateResult> ExecuteAsync(OCRContext context, RegexUpdateRequest request, OCRCorrectionService serviceInstance);
            public abstract bool CanHandle(RegexUpdateRequest request);

            protected async Task<RegularExpressions> GetOrCreateRegexAsync(
                OCRContext context,
                string pattern,
                bool multiLine = false,
                int maxLines = 1,
                string description = null)
            {
                var existingRegex = await context.RegularExpressions
                                        .FirstOrDefaultAsync(r => r.RegEx == pattern && r.MultiLine == multiLine && r.MaxLines == maxLines).ConfigureAwait(false);
                if (existingRegex != null)
                {
                    _logger.Debug("Found existing regex pattern (ID: {RegexId}): {Pattern}", existingRegex.Id, pattern);
                    return existingRegex;
                }

                var newRegex = new RegularExpressions
                {
                    RegEx = pattern,
                    MultiLine = multiLine,
                    MaxLines = maxLines,
                    Description = description ?? $"Auto-generated: {DateTime.UtcNow}",
                    CreatedDate = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow,
                    TrackingState = TrackingState.Added
                };
                context.RegularExpressions.Add(newRegex);
                _logger.Information("Prepared new regex pattern for creation: {Pattern} with MultiLine={IsMultiLine}", pattern, multiLine);
                return newRegex;
            }

            protected async Task<Fields> GetOrCreateFieldAsync(
                OCRContext context,
                string fieldKey,
                string dbFieldName,
                string entityType,
                string dataType,
                int lineId,
                bool isRequired = false,
                bool appendValues = true)
            {
                var existingField = await context.Fields.FirstOrDefaultAsync(
                                            f => f.LineId == lineId && ((!string.IsNullOrEmpty(fieldKey) && f.Key == fieldKey) || (string.IsNullOrEmpty(fieldKey) && f.Field == dbFieldName)))
                                        .ConfigureAwait(false);

                if (existingField != null)
                {
                    _logger.Debug("Found existing field definition (ID: {FieldId}) for LineId {LineId}, Key '{Key}', DBField '{DbField}'", existingField.Id, lineId, fieldKey, dbFieldName);
                    return existingField;
                }

                var newField = new Fields
                {
                    LineId = lineId,
                    Key = fieldKey,
                    Field = dbFieldName,
                    EntityType = entityType,
                    DataType = dataType,
                    IsRequired = isRequired,
                    AppendValues = appendValues,
                    TrackingState = TrackingState.Added
                };
                context.Fields.Add(newField);
                _logger.Information("Prepared new field definition for LineId {LineId}, Key '{Key}', DBField '{DbField}'", lineId, fieldKey, dbFieldName);
                return newField;
            }

            internal async Task<int> SaveChangesWithAssertiveLogging(DbContext context, string operationName)
            {
                try
                {
                    _logger.Information("   - 💾 **DB_SAVE_INTENT**: Attempting to save changes to the database for operation: {OperationName}", operationName);
                    int changes = await context.SaveChangesAsync().ConfigureAwait(false);
                    _logger.Information("   - ✅ **DB_SAVE_SUCCESS**: Successfully committed {ChangeCount} changes for {OperationName}.", changes, operationName);
                    return changes;
                }
                catch (DbEntityValidationException vex)
                {
                    var errorMessages = vex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => $"{x.PropertyName}: {x.ErrorMessage}");
                    var fullErrorMessage = string.Join("; ", errorMessages);
                    _logger.Error(vex, "🚨 **DB_SAVE_VALIDATION_FAILED**: Operation {OperationName} failed due to validation errors. Details: {ValidationErrors}", operationName, fullErrorMessage);
                    throw;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "🚨 **DB_SAVE_UNHANDLED_EXCEPTION**: Operation {OperationName} failed due to an unexpected database error.", operationName);
                    throw;
                }
            }
        }

        #endregion

        #region Field Format Strategy

        public class FieldFormatUpdateStrategy : DatabaseUpdateStrategyBase
        {
            public FieldFormatUpdateStrategy(ILogger logger) : base(logger) { }

            public override string StrategyType => "FieldFormat";

            public override bool CanHandle(RegexUpdateRequest request)
            {
                if (string.IsNullOrEmpty(request.OldValue) && !string.IsNullOrEmpty(request.NewValue)) return false;
                return request.CorrectionType == "FieldFormat" || request.CorrectionType == "FORMAT_FIX" || request.CorrectionType == "format_correction" || request.CorrectionType == "decimal_separator" || request.CorrectionType == "DecimalSeparator" || request.CorrectionType == "character_confusion" || IsPotentialFormatCorrection(request.OldValue, request.NewValue);
            }

            private bool IsPotentialFormatCorrection(string oldValue, string newValue)
            {
                if (string.IsNullOrEmpty(oldValue) || string.IsNullOrEmpty(newValue) || oldValue == newValue) return false;
                var oldNormalized = Regex.Replace(oldValue, @"[\s\$,€£\-()]", "");
                var newNormalized = Regex.Replace(newValue, @"[\s\$,€£\-()]", "");
                return string.Equals(oldNormalized, newNormalized, StringComparison.OrdinalIgnoreCase);
            }

            public override async Task<DatabaseUpdateResult> ExecuteAsync(OCRContext context, RegexUpdateRequest request, OCRCorrectionService serviceInstance)
            {
                _logger.Error("🔍 **FieldFormatUpdateStrategy_START**: Executing for field: {FieldName}, Value: '{OldValue}' -> '{NewValue}'", request.FieldName, request.OldValue, request.NewValue);

                var requestJson = JsonConvert.SerializeObject(request, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                _logger.Error("   - [CONTEXT_OBJECT_DUMP] Full RegexUpdateRequest entering strategy: {RequestJson}", requestJson);

                try
                {
                    if (!request.LineId.HasValue)
                    {
                        return DatabaseUpdateResult.Failed($"Field Definition ID (Fields.Id) is required for FieldFormatUpdateStrategy for field '{request.FieldName}'. It should be passed via RegexUpdateRequest.LineId.");
                    }

                    int fieldDefinitionId = request.LineId.Value;
                    var fieldDef = await context.Fields.FindAsync(fieldDefinitionId).ConfigureAwait(false);
                    if (fieldDef == null)
                        return DatabaseUpdateResult.Failed($"Field definition with ID {fieldDefinitionId} not found for field '{request.FieldName}'.");

                    var formatPatterns = serviceInstance.CreateAdvancedFormatCorrectionPatterns(request.OldValue, request.NewValue);
                    if (!formatPatterns.HasValue || string.IsNullOrEmpty(formatPatterns.Value.Pattern))
                    {
                        return DatabaseUpdateResult.Failed($"Could not generate format correction regex for '{request.FieldName}': '{request.OldValue}' -> '{request.NewValue}'.");
                    }

                    // ======================================================================================
                    //                          *** DEFINITIVE FIX IS HERE ***
                    //       The detailed DB_SAVE_INTENT logging has been restored.
                    // ======================================================================================
                    _logger.Error("   -> [DB_SAVE_INTENT]: Preparing to create OCR_FieldFormatRegEx entry.");
                    _logger.Error("      - FieldId: {FieldId}", fieldDefinitionId);
                    _logger.Error("      - Pattern Regex: '{Pattern}'", formatPatterns.Value.Pattern);
                    _logger.Error("      - Replacement Regex: '{Replacement}'", formatPatterns.Value.Replacement);

                    var patternRegexEntity = await this.GetOrCreateRegexAsync(context, formatPatterns.Value.Pattern, description: $"Pattern for format fix: {request.FieldName}").ConfigureAwait(false);
                    var replacementRegexEntity = await this.GetOrCreateRegexAsync(context, formatPatterns.Value.Replacement, description: $"Replacement for format fix: {request.FieldName}").ConfigureAwait(false);

                    var existingFieldFormat = await context.OCR_FieldFormatRegEx
                        .FirstOrDefaultAsync(ffr => ffr.FieldId == fieldDefinitionId && ffr.RegExId == patternRegexEntity.Id && ffr.ReplacementRegExId == replacementRegexEntity.Id)
                        .ConfigureAwait(false);

                    if (existingFieldFormat != null)
                    {
                        _logger.Information("   -> Found existing FieldFormatRegEx rule (ID: {RuleId}). No changes needed.", existingFieldFormat.Id);
                        return DatabaseUpdateResult.Success(existingFieldFormat.Id, "Existing FieldFormatRegEx");
                    }

                    var newFieldFormatRegex = new FieldFormatRegEx
                    {
                        FieldId = fieldDefinitionId,
                        RegExId = patternRegexEntity.Id,
                        ReplacementRegExId = replacementRegexEntity.Id,
                        TrackingState = TrackingState.Added
                    };
                    context.OCR_FieldFormatRegEx.Add(newFieldFormatRegex);

                    await SaveChangesWithAssertiveLogging(context, "CreateFieldFormatRule").ConfigureAwait(false);

                    return DatabaseUpdateResult.Success(newFieldFormatRegex.Id, "Created FieldFormatRegEx");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed to execute FieldFormatUpdateStrategy for {FieldName}", request.FieldName);
                    return DatabaseUpdateResult.Failed($"Database error in FieldFormatUpdateStrategy: {ex.Message}", ex);
                }
            }
        }

        #endregion

        // File: OCRCorrectionService/OCRDatabaseStrategies.cs

        #region Omission Update Strategy (With Final Fix)

        public class OmissionUpdateStrategy : DatabaseUpdateStrategyBase
        {
            public OmissionUpdateStrategy(ILogger logger) : base(logger) { }
            public override string StrategyType => "Omission";
            public override bool CanHandle(RegexUpdateRequest request) => request.CorrectionType.StartsWith("omission");

            public override async Task<DatabaseUpdateResult> ExecuteAsync(OCRContext context, RegexUpdateRequest request, OCRCorrectionService serviceInstance)
            {
                _logger.Error("🔍 **OMISSION_STRATEGY_START**: Executing for field: {FieldName}", request.FieldName);

                if (string.IsNullOrEmpty(request.FieldName) || request.NewValue == null)
                {
                    _logger.Error("   - ❌ **STRATEGY_FAIL**: Request is missing critical FieldName or NewValue. Aborting strategy.");
                    return DatabaseUpdateResult.Failed("Request object has null FieldName or NewValue.");
                }

                var requestJson = JsonConvert.SerializeObject(request, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                _logger.Error("   - [CONTEXT_OBJECT_DUMP] Full RegexUpdateRequest: {RequestJson}", requestJson);

                try
                {
                    var fieldMappingInfo = serviceInstance.MapDeepSeekFieldToDatabase(request.FieldName);
                    if (fieldMappingInfo == null) return DatabaseUpdateResult.Failed($"Unknown field mapping for '{request.FieldName}'.");

                    var correctionForPrompt = new CorrectionResult
                    {
                        FieldName = request.FieldName,
                        NewValue = request.NewValue,
                        LineText = request.LineText,
                        LineNumber = request.LineNumber
                    };
                    var lineContextForPrompt = new LineContext
                    {
                        LineNumber = request.LineNumber,
                        LineText = request.LineText,
                        ContextLinesBefore = request.ContextLinesBefore,
                        ContextLinesAfter = request.ContextLinesAfter
                    };

                    RegexCreationResponse regexResponse = null;
                    string failureReason = "Initial generation failed.";
                    int maxAttempts = 2;
                    for (int attempt = 1; attempt <= maxAttempts; attempt++)
                    {
                        _logger.Information("  -> Regex generation attempt {Attempt}/{MaxAttempts} for field '{FieldName}'", attempt, maxAttempts, request.FieldName);

                        if (attempt == 1)
                        {
                            regexResponse = await serviceInstance.RequestNewRegexFromDeepSeek(correctionForPrompt, lineContextForPrompt).ConfigureAwait(false);
                        }
                        else
                        {
                            _logger.Warning("  -> Requesting correction from DeepSeek for failed pattern. Reason: {FailureReason}", failureReason);
                            regexResponse = await serviceInstance.RequestRegexCorrectionFromDeepSeek(correctionForPrompt, lineContextForPrompt, regexResponse, failureReason).ConfigureAwait(false);
                        }

                        if (regexResponse == null || string.IsNullOrWhiteSpace(regexResponse.RegexPattern))
                        {
                            failureReason = "DeepSeek did not return a regex pattern.";
                            _logger.Warning("  -> ❌ Attempt {Attempt} failed: {Reason}", attempt, failureReason);
                            continue;
                        }

                        if (serviceInstance.ValidateRegexPattern(regexResponse, correctionForPrompt))
                        {
                            _logger.Information("  -> ✅ Regex validation successful for field '{FieldName}' on attempt {Attempt}.", request.FieldName, attempt);

                            // ======================================================================================
                            //                          *** DEFINITIVE FIX IS HERE ***
                            //  Before creating a new line, check if a functionally identical rule already exists.
                            // ======================================================================================
                            string normalizedPattern = regexResponse.RegexPattern.Replace("\\\\", "\\");

                            // Step 1: Get all Part IDs associated with the current template.
                            var partIdsForTemplate = await context.Parts
                                .Where(p => p.TemplateId == request.InvoiceId)
                                .Select(p => p.Id)
                                .ToListAsync()
                                .ConfigureAwait(false);

                            // Step 2: Check for an existing line within those parts that has the same name prefix and regex.
                            var existingLine = await context.Lines
                                .AsNoTracking() // Use AsNoTracking for a read-only check.
                                .Include(l => l.RegularExpressions)
                                .FirstOrDefaultAsync(l =>
                                    partIdsForTemplate.Contains(l.PartId) && // Check if the line belongs to a part in the current template
                                    l.Name.StartsWith("AutoOmission_" + request.FieldName) &&
                                    l.RegularExpressions.RegEx == normalizedPattern)
                                .ConfigureAwait(false);

                            if (existingLine != null)
                            {
                                _logger.Error("  -> ⏭️ SKIPPING RULE CREATION: An identical AutoOmission rule (LineId: {LineId}) with the same regex pattern ('{Pattern}') already exists for this template. This prevents database pollution and is the correct behavior.", existingLine.Id, normalizedPattern);
                                return DatabaseUpdateResult.Success(existingLine.Id, "Skipped creation, identical rule already exists.");
                            }

                            // If no identical rule exists, proceed with creation.
                            if (!request.PartId.HasValue) request.PartId = await DeterminePartIdForNewOmissionLineAsync(context, fieldMappingInfo, request).ConfigureAwait(false);
                            if (!request.PartId.HasValue) return DatabaseUpdateResult.Failed("Cannot determine PartId for new line.");

                            return await CreateNewLineForOmissionAsync(context, request, regexResponse, fieldMappingInfo).ConfigureAwait(false);
                        }

                        failureReason = $"The pattern '{regexResponse.RegexPattern}' failed to extract the expected value '{correctionForPrompt.NewValue}' from the provided text '{correctionForPrompt.LineText}'.";
                        _logger.Warning("  -> ❌ Attempt {Attempt} failed validation. Reason: {Reason}", attempt, failureReason);
                    }

                    return DatabaseUpdateResult.Failed($"DeepSeek failed to generate/validate a regex for '{request.FieldName}' after {maxAttempts} attempts. Last failure reason: {failureReason}");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed to execute OmissionUpdateStrategy for {FieldName}", request.FieldName);
                    return DatabaseUpdateResult.Failed($"Omission strategy database error: {ex.Message}", ex);
                }
            }

            private async Task<DatabaseUpdateResult> CreateNewLineForOmissionAsync(
                OCRContext context,
                RegexUpdateRequest request,
                RegexCreationResponse regexResp,
                DatabaseFieldInfo fieldInfo)
            {
                _logger.Error("  - [DB_SAVE_INTENT]: Preparing to create new Line, Field, and Regex for Omission of '{FieldName}'.", request.FieldName);
                string normalizedPattern = regexResp.RegexPattern.Replace("\\\\", "\\");

                // Step 1: Prepare all entities in memory without saving.
                var newRegexEntity = await this.GetOrCreateRegexAsync(context, normalizedPattern, regexResp.IsMultiline, regexResp.MaxLines, $"For omitted field: {request.FieldName}").ConfigureAwait(false);

                var newLineEntity = new Lines { PartId = request.PartId.Value, Name = $"AutoOmission_{request.FieldName.Replace(" ", "_").Substring(0, Math.Min(request.FieldName.Length, 40))}_{DateTime.Now:HHmmssfff}", IsActive = true, TrackingState = TrackingState.Added };
                // Associate the regex with the line. EF will handle the foreign key relationship.
                newLineEntity.RegularExpressions = newRegexEntity;
                context.Lines.Add(newLineEntity);

                bool shouldAppend = fieldInfo.DatabaseFieldName == "TotalDeduction" || fieldInfo.DatabaseFieldName == "TotalOtherCost" || fieldInfo.DatabaseFieldName == "TotalInsurance" || fieldInfo.DatabaseFieldName == "TotalInternalFreight";
                var newFieldEntity = new Fields
                {
                    Key = request.FieldName,
                    Field = fieldInfo.DatabaseFieldName,
                    EntityType = fieldInfo.EntityType,
                    DataType = fieldInfo.DataType,
                    IsRequired = false,
                    AppendValues = shouldAppend,
                    TrackingState = TrackingState.Added
                };
                // Associate the field with the line. EF will set the LineId upon saving.
                newLineEntity.Fields.Add(newFieldEntity);

                // Step 2: Commit all prepared entities in a single transaction.
                await SaveChangesWithAssertiveLogging(context, "CreateNewLineForOmission").ConfigureAwait(false);

                _logger.Information("Successfully created new Line (ID: {LineId}), Field (ID: {FieldId}), and Regex (ID: {RegexId}) for omission.", newLineEntity.Id, newFieldEntity.Id, newRegexEntity.Id);
                return DatabaseUpdateResult.Success(newLineEntity.Id, "Created new line, field, and regex for omission");
            }

            private async Task<int?> DeterminePartIdForNewOmissionLineAsync(
                OCRContext context,
                DatabaseFieldInfo fieldInfo,
                RegexUpdateRequest request)
            {
                _logger.Error("🎯 [CONTRACT_VALIDATION_ENTRY]: Entering DeterminePartIdForNewOmissionLineAsync for Field '{FieldName}'.", request.FieldName);
                if (!request.InvoiceId.HasValue)
                {
                    _logger.Error("   - [CONTRACT_VIOLATION]: Precondition failed. The RegexUpdateRequest does not have an InvoiceId. Cannot determine the correct Part.");
                    return null;
                }

                string targetPartTypeName = (fieldInfo?.EntityType == "InvoiceDetails") ? "LineItem" : "Header";
                _logger.Error("   - [LOGIC]: Determined Target Part Type is '{PartType}'.", targetPartTypeName);

                var part = await context.Parts.Include(p => p.PartTypes)
                    .FirstOrDefaultAsync(p => p.TemplateId == request.InvoiceId.Value && p.PartTypes.Name.Equals(targetPartTypeName, StringComparison.OrdinalIgnoreCase))
                    .ConfigureAwait(false);

                if (part == null)
                {
                    _logger.Error("   - [LOOKUP_FAILURE]: Could not find a Part of type '{PartType}' for TemplateId {TemplateId}.", targetPartTypeName, request.InvoiceId.Value);
                    return null;
                }

                _logger.Error("   - [LOOKUP_SUCCESS]: Found correct Part. PartId: {PartId}, Name: '{PartName}'.", part.Id, part.PartTypes.Name);
                return part.Id;
            }
        }
        #endregion

        #region Strategy Factory

        public class DatabaseUpdateStrategyFactory
        {
            private readonly ILogger _logger;
            public DatabaseUpdateStrategyFactory(ILogger logger) { _logger = logger; }
            public IDatabaseUpdateStrategy GetStrategy(RegexUpdateRequest request)
            {
                if (request == null) throw new ArgumentNullException(nameof(request));
                if (new OmissionUpdateStrategy(_logger).CanHandle(request)) return new OmissionUpdateStrategy(_logger);
                if (new FieldFormatUpdateStrategy(_logger).CanHandle(request)) return new FieldFormatUpdateStrategy(_logger);
                _logger.Warning("No suitable update strategy found for correction type: {CorrectionType}", request.CorrectionType);
                throw new InvalidOperationException($"No suitable update strategy found for correction type: {request.CorrectionType}");
            }
        }

        #endregion
    }
}