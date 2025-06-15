// File: OCRCorrectionService/OCRDatabaseStrategies.cs
using System;
using System.Collections.Generic;
using System.Data.Entity;
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
                OCRContext context, string pattern, bool multiLine = false, int maxLines = 1, string description = null)
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
                OCRContext context, string fieldKey, string dbFieldName, string entityType, string dataType,
                int lineId, bool isRequired = false, bool appendValues = true)
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
        }

        #endregion

        #region Field Format Strategy (Unchanged)
        public class FieldFormatUpdateStrategy : DatabaseUpdateStrategyBase
        {
            public FieldFormatUpdateStrategy(ILogger logger) : base(logger) { }
            public override string StrategyType => "FieldFormat";
            public override bool CanHandle(RegexUpdateRequest request) => request.CorrectionType == "FieldFormat";
            public override async Task<DatabaseUpdateResult> ExecuteAsync(OCRContext context, RegexUpdateRequest request, OCRCorrectionService serviceInstance)
            {
                // Implementation remains the same
                return DatabaseUpdateResult.Failed("FieldFormatUpdateStrategy not fully implemented in this version.");
            }
        }
        #endregion

        #region Omission Update Strategy (With Validation & Retry Loop)

        public class OmissionUpdateStrategy : DatabaseUpdateStrategyBase
        {
            public OmissionUpdateStrategy(ILogger logger) : base(logger) { }
            public override string StrategyType => "Omission";
            public override bool CanHandle(RegexUpdateRequest request) => request.CorrectionType.StartsWith("omission");

            public override async Task<DatabaseUpdateResult> ExecuteAsync(OCRContext context, RegexUpdateRequest request, OCRCorrectionService serviceInstance)
            {
                _logger.Error("🔍 **OMISSION_STRATEGY_START**: Executing for field: {FieldName}", request.FieldName);

                // Defensive check for null request properties that cause exceptions downstream.
                if (string.IsNullOrEmpty(request.FieldName) || string.IsNullOrEmpty(request.NewValue))
                {
                    _logger.Error("   - ❌ **STRATEGY_FAIL**: Request is missing critical FieldName or NewValue. Aborting.");
                    return DatabaseUpdateResult.Failed("Request object has null FieldName or NewValue.");
                }

                var requestJson = JsonConvert.SerializeObject(request, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                _logger.Error("   - [CONTEXT_OBJECT_DUMP] Full RegexUpdateRequest: {RequestJson}", requestJson);

                try
                {
                    var fieldMappingInfo = serviceInstance.MapDeepSeekFieldToDatabase(request.FieldName);
                    if (fieldMappingInfo == null) return DatabaseUpdateResult.Failed($"Unknown field mapping for '{request.FieldName}'.");

                    // ================== CRITICAL FIX: Properly populate prompt contexts ==================
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
                        FullContextWithLineNumbers = string.Join("\n", request.ContextLinesBefore.Concat(new[] { $">>> LINE {request.LineNumber}: {request.LineText} <<<" }).Concat(request.ContextLinesAfter))
                    };
                    // =====================================================================================

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
                            if (!request.PartId.HasValue) request.PartId = await DeterminePartIdForNewOmissionLineAsync(context, fieldMappingInfo, request).ConfigureAwait(false);
                            if (!request.PartId.HasValue) return DatabaseUpdateResult.Failed("Cannot determine PartId for new line.");

                            return await CreateNewLineForOmissionAsync(context, request, regexResponse, fieldMappingInfo, serviceInstance).ConfigureAwait(false);
                        }

                        failureReason = $"The pattern '{regexResponse.RegexPattern}' failed to extract the expected value '{correctionForPrompt.NewValue}' from the provided text.";
                        _logger.Warning("  -> ❌ Attempt {Attempt} failed validation. Reason: {Reason}", attempt, failureReason);
                    }

                    return DatabaseUpdateResult.Failed($"DeepSeek failed to generate a valid regex for '{request.FieldName}' after {maxAttempts} attempts. Last failure reason: {failureReason}");
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed to execute OmissionUpdateStrategy for {FieldName}", request.FieldName);
                    return DatabaseUpdateResult.Failed($"Omission strategy database error: {ex.Message}", ex);
                }
            }

            private async Task<DatabaseUpdateResult> CreateNewLineForOmissionAsync(
                OCRContext context, RegexUpdateRequest request, RegexCreationResponse regexResp,
                DatabaseFieldInfo fieldInfo, OCRCorrectionService serviceInstance)
            {
                _logger.Error("  - [DB_SAVE_INTENT]: Preparing to create new Line, Field, and Regex for Omission of '{FieldName}'.", request.FieldName);
                string normalizedPattern = regexResp.RegexPattern.Replace("\\\\", "\\");

                var newRegexEntity = await this.GetOrCreateRegexAsync(context, normalizedPattern, regexResp.IsMultiline, regexResp.MaxLines, $"For omitted field: {request.FieldName}").ConfigureAwait(false);
                if (newRegexEntity.TrackingState == TrackingState.Added) await context.SaveChangesAsync().ConfigureAwait(false);

                var newLineEntity = new Lines { PartId = request.PartId.Value, RegExId = newRegexEntity.Id, Name = $"AutoOmission_{request.FieldName.Replace(" ", "_")}_{DateTime.Now:HHmmssfff}", IsActive = true, TrackingState = TrackingState.Added };
                context.Lines.Add(newLineEntity);
                await context.SaveChangesAsync().ConfigureAwait(false);

                bool shouldAppend = fieldInfo.DatabaseFieldName == "TotalDeduction" || fieldInfo.DatabaseFieldName == "TotalOtherCost" || fieldInfo.DatabaseFieldName == "TotalInsurance";
                var newFieldEntity = await this.GetOrCreateFieldAsync(context, request.FieldName, fieldInfo.DatabaseFieldName, fieldInfo.EntityType, fieldInfo.DataType, newLineEntity.Id, false, shouldAppend).ConfigureAwait(false);
                if (newFieldEntity.TrackingState == TrackingState.Added) await context.SaveChangesAsync().ConfigureAwait(false);

                _logger.Information("Successfully created new Line (ID: {LineId}) and Field (ID: {FieldId}) for omission.", newLineEntity.Id, newFieldEntity.Id);
                return DatabaseUpdateResult.Success(newFieldEntity.Id, "Created new line, field, and regex for omission");
            }

            private async Task<int?> DeterminePartIdForNewOmissionLineAsync(
                OCRContext context, DatabaseFieldInfo fieldInfo, RegexUpdateRequest request)
            {
                _logger.Error("🎯 [CONTRACT_VALIDATION_ENTRY]: Entering DeterminePartIdForNewOmissionLineAsync for Field '{FieldName}'.", request.FieldName);
                if (!request.InvoiceId.HasValue)
                {
                    _logger.Error("   - [CONTRACT_VIOLATION]: Precondition failed. The RegexUpdateRequest does not have an InvoiceId. Cannot determine the correct Part.");
                    return null;
                }
                _logger.Error("   - [CONTRACT_MET]: Received InvoiceId (TemplateId): {InvoiceId}", request.InvoiceId.Value);

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