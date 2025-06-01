// File: OCRCorrectionService/OCRDatabaseStrategies.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.RegularExpressions;
using OCR.Business.Entities; // For DB entities like RegularExpressions, Fields, Lines, FieldFormatRegEx
using Serilog;
using System.Data.Entity; // For EF operations like FirstOrDefaultAsync
using TrackableEntities; // For TrackingState

namespace WaterNut.DataSpace
{
    public partial class OCRCorrectionService
    {
        #region Strategy Interfaces and Base Classes

        public interface IDatabaseUpdateStrategy
        {
            string StrategyType { get; }
            Task<DatabaseUpdateResult> ExecuteAsync(OCRContext context, RegexUpdateRequest request, OCRCorrectionService serviceInstance);
            bool CanHandle(CorrectionResult correction);
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
            public abstract bool CanHandle(CorrectionResult correction);

            protected async Task<RegularExpressions> GetOrCreateRegexAsync(OCRContext context, string pattern, bool multiLine = false, int maxLines = 1, string description = null)
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
                _logger.Information("Prepared new regex pattern for creation: {Pattern}", pattern);
                return newRegex;
            }

            protected async Task<Fields> GetOrCreateFieldAsync(OCRContext context, string fieldKey, string dbFieldName, string entityType, string dataType, int lineId, bool isRequired = false)
            {
                var existingField = await context.Fields
                                        .FirstOrDefaultAsync(f => f.LineId == lineId &&
                                                                  ((!string.IsNullOrEmpty(fieldKey) && f.Key == fieldKey) ||
                                                                   (string.IsNullOrEmpty(fieldKey) && f.Field == dbFieldName))).ConfigureAwait(false);

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
                    AppendValues = true,
                    TrackingState = TrackingState.Added
                };
                context.Fields.Add(newField);
                _logger.Information("Prepared new field definition for LineId {LineId}, Key '{Key}', DBField '{DbField}'", lineId, fieldKey, dbFieldName);
                return newField;
            }
        }
        #endregion

        #region Field Format Strategy
        public class FieldFormatUpdateStrategy : DatabaseUpdateStrategyBase
        {
            public FieldFormatUpdateStrategy(ILogger logger) : base(logger) { }

            public override string StrategyType => "FieldFormat";

            public override bool CanHandle(CorrectionResult correction)
            {
                if (string.IsNullOrEmpty(correction.OldValue) && !string.IsNullOrEmpty(correction.NewValue))
                    return false;

                return correction.CorrectionType == "FieldFormat" ||
                       correction.CorrectionType == "FORMAT_FIX" ||
                       correction.CorrectionType == "format_correction" ||
                       correction.CorrectionType == "decimal_separator" ||
                       correction.CorrectionType == "DecimalSeparator" ||  // Handle both cases
                       correction.CorrectionType == "character_confusion" ||
                       IsPotentialFormatCorrection(correction.OldValue, correction.NewValue);
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
                _logger.Information("Executing FieldFormatUpdateStrategy for field: {FieldName}, Value: '{OldValue}' -> '{NewValue}'", request.FieldName, request.OldValue, request.NewValue);
                try
                {
                    if (!request.LineId.HasValue)
                    {
                        return DatabaseUpdateResult.Failed($"Field Definition ID (Fields.Id) is required for FieldFormatUpdateStrategy for field '{request.FieldName}'. It should be passed via RegexUpdateRequest.LineId.");
                    }
                    int fieldDefinitionId = request.LineId.Value;

                    var fieldDef = await context.Fields.FindAsync(fieldDefinitionId).ConfigureAwait(false);
                    if (fieldDef == null) return DatabaseUpdateResult.Failed($"Field definition with ID {fieldDefinitionId} not found for field '{request.FieldName}'.");

                    var formatPatterns = serviceInstance.CreateAdvancedFormatCorrectionPatterns(request.OldValue, request.NewValue);

                    if (!formatPatterns.HasValue || string.IsNullOrEmpty(formatPatterns.Value.Pattern))
                    {
                        return DatabaseUpdateResult.Failed($"Could not generate format correction regex for '{request.FieldName}': '{request.OldValue}' -> '{request.NewValue}'.");
                    }

                    var patternRegexEntity = await this.GetOrCreateRegexAsync(context, formatPatterns.Value.Pattern, description: $"Pattern for format fix: {request.FieldName}").ConfigureAwait(false);
                    var replacementRegexEntity = await this.GetOrCreateRegexAsync(context, formatPatterns.Value.Replacement, description: $"Replacement for format fix: {request.FieldName}").ConfigureAwait(false);

                    if (patternRegexEntity.TrackingState == TrackingState.Added || replacementRegexEntity.TrackingState == TrackingState.Added)
                    {
                        await context.SaveChangesAsync().ConfigureAwait(false);
                    }

                    var existingFieldFormat = await context.OCR_FieldFormatRegEx
                                                  .FirstOrDefaultAsync(ffr => ffr.FieldId == fieldDefinitionId &&
                                                                              ffr.RegExId == patternRegexEntity.Id &&
                                                                              ffr.ReplacementRegExId == replacementRegexEntity.Id).ConfigureAwait(false);

                    if (existingFieldFormat != null)
                    {
                        _logger.Information("FieldFormatRegEx (ID: {Id}) already exists for FieldId {FieldId}.", existingFieldFormat.Id, fieldDefinitionId);
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
                    await context.SaveChangesAsync().ConfigureAwait(false);

                    _logger.Information("Created new FieldFormatRegEx (ID: {NewId}) for FieldId {FieldId}: Pattern='{Pattern}' -> Replacement='{Replacement}'",
                        newFieldFormatRegex.Id, fieldDefinitionId, formatPatterns.Value.Pattern, formatPatterns.Value.Replacement);
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

        #region Omission Update Strategy
        public class OmissionUpdateStrategy : DatabaseUpdateStrategyBase
        {
            public OmissionUpdateStrategy(ILogger logger) : base(logger) { }

            public override string StrategyType => "Omission";
            public override bool CanHandle(CorrectionResult correction) => correction.CorrectionType == "omission" || correction.CorrectionType == "omitted_line_item";

            public override async Task<DatabaseUpdateResult> ExecuteAsync(OCRContext context, RegexUpdateRequest request, OCRCorrectionService serviceInstance)
            {
                _logger.Information("Executing OmissionUpdateStrategy for field: {FieldName}, New Value: '{NewValue}'", request.FieldName, request.NewValue);
                try
                {
                    var fieldMappingInfo = serviceInstance.MapDeepSeekFieldToDatabase(request.FieldName);
                    if (fieldMappingInfo == null && request.CorrectionType != "omitted_line_item")
                    {
                        return DatabaseUpdateResult.Failed($"Unknown field mapping for omitted field '{request.FieldName}'. Cannot determine EntityType/DataType.");
                    }

                    var correctionForPrompt = new CorrectionResult
                    {
                        FieldName = request.FieldName,
                        NewValue = request.NewValue,
                        OldValue = request.OldValue,
                        LineText = request.LineText,
                        LineNumber = request.LineNumber,
                        ContextLinesBefore = request.ContextLinesBefore,
                        ContextLinesAfter = request.ContextLinesAfter,
                        RequiresMultilineRegex = request.RequiresMultilineRegex,
                        CorrectionType = request.CorrectionType
                    };

                    var lineContextForPrompt = new LineContext
                    {
                        LineId = request.LineId,
                        LineNumber = request.LineNumber,
                        LineText = request.LineText,
                        PartId = request.PartId,
                        RegexPattern = request.ExistingRegex,
                        ContextLinesBefore = request.ContextLinesBefore,
                        ContextLinesAfter = request.ContextLinesAfter,
                        RequiresMultilineRegex = request.RequiresMultilineRegex
                    };
                    if (!string.IsNullOrEmpty(request.ExistingRegex))
                    {
                        lineContextForPrompt.FieldsInLine = serviceInstance.ExtractNamedGroupsFromRegex(request.ExistingRegex)
                                                                .Select(g => new FieldInfo { Key = g }).ToList();
                    }

                    var regexResponse = await serviceInstance.RequestNewRegexFromDeepSeek(correctionForPrompt, lineContextForPrompt).ConfigureAwait(false);

                    if (regexResponse == null || !serviceInstance.ValidateRegexPattern(regexResponse, correctionForPrompt))
                    {
                        return DatabaseUpdateResult.Failed($"Failed to get or validate regex pattern from DeepSeek for omission: '{request.FieldName}'.");
                    }

                    if (regexResponse.Strategy == "modify_existing_line" && request.LineId.HasValue && request.RegexId.HasValue)
                    {
                        _logger.Information("Omission strategy for {FieldName}: Modifying existing line {LineId}", request.FieldName, request.LineId.Value);
                        return await this.ModifyExistingLineForOmissionAsync(context, request, regexResponse, fieldMappingInfo, serviceInstance).ConfigureAwait(false);
                    }
                    else
                    {
                        _logger.Information("Omission strategy for {FieldName}: Creating new line definition.", request.FieldName);
                        if (!request.PartId.HasValue)
                        {
                            request.PartId = await this.DeterminePartIdForNewOmissionLineAsync(context, fieldMappingInfo, request.FieldName, serviceInstance).ConfigureAwait(false);
                            if (!request.PartId.HasValue) return DatabaseUpdateResult.Failed($"Cannot create new line for omission '{request.FieldName}' without a valid PartId.");
                            _logger.Information("Determined PartId {PartId} for new omission line of field {FieldName}", request.PartId.Value, request.FieldName);
                        }
                        return await this.CreateNewLineForOmissionAsync(context, request, regexResponse, fieldMappingInfo, serviceInstance).ConfigureAwait(false);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed to execute OmissionUpdateStrategy for {FieldName}", request.FieldName);
                    return DatabaseUpdateResult.Failed($"Omission strategy database error: {ex.Message}", ex);
                }
            }

            private async Task<DatabaseUpdateResult> ModifyExistingLineForOmissionAsync(OCRContext context, RegexUpdateRequest request, RegexCreationResponse regexResp, DatabaseFieldInfo fieldInfo, OCRCorrectionService serviceInstance)
            {
                var existingLineDbEntity = await context.Lines.Include(l => l.RegularExpressions)
                                               .FirstOrDefaultAsync(l => l.Id == request.LineId.Value).ConfigureAwait(false);
                if (existingLineDbEntity == null) return DatabaseUpdateResult.Failed($"Existing Line with ID {request.LineId.Value} not found for modification.");

                if (existingLineDbEntity.RegularExpressions == null)
                {
                    var newRegexForExistingLine = await this.GetOrCreateRegexAsync(context, regexResp.CompleteLineRegex, regexResp.IsMultiline, regexResp.MaxLines, $"Modified for omission: {request.FieldName}").ConfigureAwait(false);
                    // Intentionally not saving here, GetOrCreateRegexAsync just prepares. Main SaveChangesAsync will commit.
                    if (newRegexForExistingLine.TrackingState == TrackingState.Added) await context.SaveChangesAsync().ConfigureAwait(false); // Save if new to get ID
                    existingLineDbEntity.RegExId = newRegexForExistingLine.Id;
                    _logger.Warning("Existing Line {LineId} had no Regex assigned. Created and assigned new Regex ID {RegexId}", existingLineDbEntity.Id, newRegexForExistingLine.Id);
                }
                else
                {
                    existingLineDbEntity.RegularExpressions.RegEx = regexResp.CompleteLineRegex;
                    existingLineDbEntity.RegularExpressions.MultiLine = regexResp.IsMultiline;
                    existingLineDbEntity.RegularExpressions.MaxLines = regexResp.MaxLines;
                    existingLineDbEntity.RegularExpressions.LastUpdated = DateTime.UtcNow;
                    // No need to set TrackingState if EF tracks changes on loaded entities
                }
                // No need to set existingLineDbEntity.TrackingState = TrackingState.Modified explicitly if EF change tracking is on.

                string dbFieldName = fieldInfo?.DatabaseFieldName ?? request.FieldName;
                string entityType = fieldInfo?.EntityType ?? (request.FieldName.Contains("InvoiceDetail") ? "InvoiceDetails" : "ShipmentInvoice");
                string dataType = fieldInfo?.DataType ?? "string";

                var newFieldEntity = await this.GetOrCreateFieldAsync(context, request.FieldName, dbFieldName, entityType, dataType, request.LineId.Value).ConfigureAwait(false);

                await context.SaveChangesAsync().ConfigureAwait(false); // Commit all prepared changes for this operation
                _logger.Information("Successfully modified Line {LineId} (Regex {RegexId}) and added/verified Field {FieldId} for omitted field {OmittedFieldName}",
                    existingLineDbEntity.Id, existingLineDbEntity.RegExId, newFieldEntity.Id, request.FieldName);
                return DatabaseUpdateResult.Success(newFieldEntity.Id, "Modified existing line for omission");
            }

            private async Task<DatabaseUpdateResult> CreateNewLineForOmissionAsync(OCRContext context, RegexUpdateRequest request, RegexCreationResponse regexResp, DatabaseFieldInfo fieldInfo, OCRCorrectionService serviceInstance)
            {
                var newRegexEntity = await this.GetOrCreateRegexAsync(context, regexResp.RegexPattern, regexResp.IsMultiline, regexResp.MaxLines, $"For omitted field: {request.FieldName}").ConfigureAwait(false);
                if (newRegexEntity.TrackingState == TrackingState.Added) await context.SaveChangesAsync().ConfigureAwait(false);

                var newLineEntity = new Lines
                {
                    PartId = request.PartId.Value,
                    RegExId = newRegexEntity.Id,
                    Name = $"AutoOmission_{request.FieldName.Replace(" ", "_").Substring(0, Math.Min(request.FieldName.Length, 40))}_{DateTime.Now:HHmmssfff}",
                    IsActive = true,
                    // SortOrder logic removed assuming Lines entity does not have it. Add back if it does.
                    // SortOrder = (await context.Lines.Where(l => l.PartId == request.PartId.Value).MaxAsync(l => (int?)l.SortOrder) ?? 0) + 10, 
                    TrackingState = TrackingState.Added
                };
                context.Lines.Add(newLineEntity);
                await context.SaveChangesAsync().ConfigureAwait(false);

                string dbFieldName = fieldInfo?.DatabaseFieldName ?? request.FieldName;
                string entityType = fieldInfo?.EntityType ?? (request.FieldName.Contains("InvoiceDetail") ? "InvoiceDetails" : "ShipmentInvoice");
                string dataType = fieldInfo?.DataType ?? "string";

                var newFieldEntity = await this.GetOrCreateFieldAsync(context, request.FieldName, dbFieldName, entityType, dataType, newLineEntity.Id).ConfigureAwait(false);

                await context.SaveChangesAsync().ConfigureAwait(false);
                _logger.Information("Successfully created new Line {NewLineId} (Regex {RegexId}) and Field {NewFieldId} for omitted field {OmittedFieldName}",
                    newLineEntity.Id, newRegexEntity.Id, newFieldEntity.Id, request.FieldName);
                return DatabaseUpdateResult.Success(newFieldEntity.Id, "Created new line for omission");
            }

            private async Task<int?> DeterminePartIdForNewOmissionLineAsync(OCRContext context, DatabaseFieldInfo fieldInfo, string originalFieldNameFromRequest, OCRCorrectionService serviceInstance)
            {
                string targetPartTypeName = "Header";

                if (fieldInfo != null)
                {
                    if (fieldInfo.EntityType == "InvoiceDetails")
                    {
                        targetPartTypeName = "LineItem";
                    }
                }
                else if (!string.IsNullOrEmpty(originalFieldNameFromRequest))
                {
                    var inferredFieldInfo = serviceInstance.MapDeepSeekFieldToDatabase(originalFieldNameFromRequest);
                    if (inferredFieldInfo?.EntityType == "InvoiceDetails" || originalFieldNameFromRequest.ToLower().Contains("invoicedetail"))
                    {
                        targetPartTypeName = "LineItem";
                    }
                }

                _logger.Debug("DeterminePartId: Target PartTypeName '{TargetPartTypeName}' for field '{OriginalFieldName}'.", targetPartTypeName, originalFieldNameFromRequest);

                var part = await context.Parts.Include(p => p.PartTypes)
                               .FirstOrDefaultAsync(p => p.PartTypes.Name.Equals(targetPartTypeName, StringComparison.OrdinalIgnoreCase)).ConfigureAwait(false);

                if (part != null) return part.Id;

                _logger.Warning("Could not determine PartId for new omission line based on EntityType/FieldName '{OriginalFieldName}'. Falling back to first available Part.", originalFieldNameFromRequest);
                var firstPart = await context.Parts.OrderBy(p => p.Id).FirstOrDefaultAsync().ConfigureAwait(false);
                if (firstPart == null) _logger.Error("No Parts defined in database. Cannot create new line for omission.");
                return firstPart?.Id;
            }
        }
        #endregion

        #region Strategy Factory
        public class DatabaseUpdateStrategyFactory
        {
            private readonly ILogger _logger;

            public DatabaseUpdateStrategyFactory(ILogger logger)
            {
                _logger = logger;
            }

            public IDatabaseUpdateStrategy GetStrategy(CorrectionResult correction)
            {
                if (correction.CorrectionType == "omission" || correction.CorrectionType == "omitted_line_item")
                {
                    return new OmissionUpdateStrategy(_logger);
                }

                var ffStrategy = new FieldFormatUpdateStrategy(_logger);
                if (ffStrategy.CanHandle(correction))
                {
                    return ffStrategy;
                }

                _logger.Warning("No specific database update strategy found for correction type: {CorrectionType} on field {FieldName}. DB update might be skipped or logged only.",
                    correction.CorrectionType, correction.FieldName);

                return null;
            }
        }
        #endregion
    }
}