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

            public override bool CanHandle(RegexUpdateRequest request)
            {
                if (string.IsNullOrEmpty(request.OldValue) && !string.IsNullOrEmpty(request.NewValue))
                    return false;

                return request.CorrectionType == "FieldFormat" ||
                       request.CorrectionType == "FORMAT_FIX" ||
                       request.CorrectionType == "format_correction" ||
                       request.CorrectionType == "decimal_separator" ||
                       request.CorrectionType == "DecimalSeparator" ||  // Handle both cases
                       request.CorrectionType == "character_confusion" ||
                       IsPotentialFormatCorrection(request.OldValue, request.NewValue);
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
            public override bool CanHandle(RegexUpdateRequest request) => request.CorrectionType == "omission" || request.CorrectionType == "omitted_line_item";

            public override async Task<DatabaseUpdateResult> ExecuteAsync(OCRContext context, RegexUpdateRequest request, OCRCorrectionService serviceInstance)
            {
                _logger.Error("🔍 **OMISSION_STRATEGY_START**: Executing OmissionUpdateStrategy for field: {FieldName}", request.FieldName);
                _logger.Error("🔍 **OMISSION_STRATEGY_INPUT**: OldValue={OldValue} | NewValue={NewValue} | LineText={LineText}", 
                    request.OldValue ?? "NULL", request.NewValue ?? "NULL", request.LineText ?? "NULL");
                _logger.Error("🔍 **OMISSION_STRATEGY_CONTEXT**: LineId={LineId} | PartId={PartId} | RegexId={RegexId}", 
                    request.LineId?.ToString() ?? "NULL", request.PartId?.ToString() ?? "NULL", request.RegexId?.ToString() ?? "NULL");
                try
                {
                    // STEP 1: Field mapping validation with detailed logging
                    _logger.Error("🔍 **STRATEGY_STEP_1**: Getting field mapping for {FieldName}", request.FieldName);
                    var fieldMappingInfo = serviceInstance.MapDeepSeekFieldToDatabase(request.FieldName);
                    _logger.Error("🔍 **STRATEGY_FIELD_MAPPING**: FieldMapping = {IsNull} | DatabaseFieldName = {DatabaseFieldName} | EntityType = {EntityType} | DataType = {DataType}", 
                        fieldMappingInfo == null ? "NULL" : "NOT_NULL",
                        fieldMappingInfo?.DatabaseFieldName ?? "NULL",
                        fieldMappingInfo?.EntityType ?? "NULL", 
                        fieldMappingInfo?.DataType ?? "NULL");
                    
                    if (fieldMappingInfo == null && request.CorrectionType != "omitted_line_item")
                    {
                        _logger.Error("❌ **STRATEGY_STEP_1_FAILED**: Unknown field mapping for field {FieldName}", request.FieldName);
                        return DatabaseUpdateResult.Failed($"Unknown field mapping for omitted field '{request.FieldName}'. Cannot determine EntityType/DataType.");
                    }
                    _logger.Error("✅ **STRATEGY_STEP_1_SUCCESS**: Field mapping validation passed for {FieldName}", request.FieldName);

                    // STEP 2: Create correction and line context for DeepSeek
                    _logger.Error("🔍 **STRATEGY_STEP_2**: Creating correction and line context for DeepSeek API call");
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
                        _logger.Error("🔍 **STRATEGY_EXISTING_REGEX**: Found existing regex with {FieldCount} fields", lineContextForPrompt.FieldsInLine?.Count ?? 0);
                    }

                    // STEP 3: Call DeepSeek API for regex generation
                    _logger.Error("🔍 **STRATEGY_STEP_3**: Calling DeepSeek API for regex generation");
                    var regexResponse = await serviceInstance.RequestNewRegexFromDeepSeek(correctionForPrompt, lineContextForPrompt).ConfigureAwait(false);
                    _logger.Error("🔍 **STRATEGY_DEEPSEEK_RESPONSE**: RegexResponse = {IsNull} | Pattern = {Pattern} | Strategy = {Strategy}", 
                        regexResponse == null ? "NULL" : "NOT_NULL",
                        regexResponse?.RegexPattern ?? "NULL",
                        regexResponse?.Strategy ?? "NULL");

                    if (regexResponse == null)
                    {
                        _logger.Error("❌ **STRATEGY_STEP_3_FAILED**: DeepSeek API returned null response for field {FieldName}", request.FieldName);
                        return DatabaseUpdateResult.Failed($"DeepSeek API returned null response for omission: '{request.FieldName}'.");
                    }

                    // STEP 4: Validate the regex pattern
                    _logger.Error("🔍 **STRATEGY_STEP_4**: Validating regex pattern from DeepSeek");
                    bool isValidPattern = serviceInstance.ValidateRegexPattern(regexResponse, correctionForPrompt);
                    _logger.Error("🔍 **STRATEGY_PATTERN_VALIDATION**: Pattern validation result = {IsValid}", isValidPattern);
                    
                    if (!isValidPattern)
                    {
                        _logger.Error("❌ **STRATEGY_STEP_4_FAILED**: Regex pattern validation failed for field {FieldName}", request.FieldName);
                        return DatabaseUpdateResult.Failed($"Failed to validate regex pattern from DeepSeek for omission: '{request.FieldName}'.");
                    }
                    _logger.Error("✅ **STRATEGY_STEP_3_4_SUCCESS**: DeepSeek API call and pattern validation successful for {FieldName}", request.FieldName);

                    // STEP 5: Choose strategy based on DeepSeek response and available context
                    _logger.Error("🔍 **STRATEGY_STEP_5**: Choosing database update strategy");
                    _logger.Error("🔍 **STRATEGY_DECISION_CONTEXT**: Strategy={Strategy} | HasLineId={HasLineId} | HasRegexId={HasRegexId} | HasPartId={HasPartId}", 
                        regexResponse.Strategy ?? "NULL", 
                        request.LineId.HasValue, 
                        request.RegexId.HasValue, 
                        request.PartId.HasValue);
                    
                    if (regexResponse.Strategy == "modify_existing_line" && request.LineId.HasValue && request.RegexId.HasValue)
                    {
                        _logger.Error("🔍 **STRATEGY_STEP_5_MODIFY**: Executing modify existing line strategy for {FieldName} with LineId {LineId}", request.FieldName, request.LineId.Value);
                        var modifyResult = await this.ModifyExistingLineForOmissionAsync(context, request, regexResponse, fieldMappingInfo, serviceInstance).ConfigureAwait(false);
                        _logger.Error("🔍 **STRATEGY_MODIFY_RESULT**: ModifyExistingLine result - Success={Success} | Message={Message}", 
                            modifyResult.IsSuccess, modifyResult.Message ?? "NULL");
                        return modifyResult;
                    }
                    else
                    {
                        _logger.Error("🔍 **STRATEGY_STEP_5_CREATE**: Executing create new line strategy for {FieldName}", request.FieldName);
                        if (!request.PartId.HasValue)
                        {
                            // This should NOT happen now that GetDatabaseUpdateContext determines PartId upfront
                            _logger.Error("❌ **STRATEGY_UNEXPECTED_NO_PARTID**: PartId is null - this should have been determined in GetDatabaseUpdateContext for field {FieldName}", request.FieldName);
                            _logger.Error("🔍 **STRATEGY_FALLBACK_PARTID**: Attempting fallback PartId determination");
                            request.PartId = await this.DeterminePartIdForNewOmissionLineAsync(context, fieldMappingInfo, request.FieldName, serviceInstance).ConfigureAwait(false);
                            if (!request.PartId.HasValue) 
                            {
                                _logger.Error("❌ **STRATEGY_STEP_5_FAILED**: Cannot determine PartId for new line creation even with fallback");
                                return DatabaseUpdateResult.Failed($"Cannot create new line for omission '{request.FieldName}' without a valid PartId.");
                            }
                            _logger.Error("⚠️ **STRATEGY_PARTID_FALLBACK_SUCCESS**: Fallback determined PartId {PartId} for new omission line of field {FieldName}", request.PartId.Value, request.FieldName);
                        }
                        else
                        {
                            _logger.Error("✅ **STRATEGY_PARTID_PROVIDED**: Using pre-determined PartId {PartId} for new omission line of field {FieldName}", request.PartId.Value, request.FieldName);
                        }
                        var createResult = await this.CreateNewLineForOmissionAsync(context, request, regexResponse, fieldMappingInfo, serviceInstance).ConfigureAwait(false);
                        _logger.Error("🔍 **STRATEGY_CREATE_RESULT**: CreateNewLine result - Success={Success} | Message={Message}", 
                            createResult.IsSuccess, createResult.Message ?? "NULL");
                        return createResult;
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error("🚨 **STRATEGY_EXCEPTION**: Exception in OmissionUpdateStrategy for field {FieldName}", request.FieldName);
                    _logger.Error("🚨 **STRATEGY_EXCEPTION_DETAILS**: ExceptionType={ExceptionType} | Message={Message}", 
                        ex.GetType().Name, ex.Message);
                    _logger.Error("🚨 **STRATEGY_EXCEPTION_STACK**: StackTrace={StackTrace}", ex.StackTrace);
                    _logger.Error(ex, "Failed to execute OmissionUpdateStrategy for {FieldName}", request.FieldName);
                    return DatabaseUpdateResult.Failed($"Omission strategy database error: {ex.Message}", ex);
                }
            }

            private async Task<DatabaseUpdateResult> ModifyExistingLineForOmissionAsync(OCRContext context, RegexUpdateRequest request, RegexCreationResponse regexResp, DatabaseFieldInfo fieldInfo, OCRCorrectionService serviceInstance)
            {
                var existingLineDbEntity = await context.Lines.Include(l => l.RegularExpressions)
                                               .FirstOrDefaultAsync(l => l.Id == request.LineId.Value).ConfigureAwait(false);
                if (existingLineDbEntity == null) return DatabaseUpdateResult.Failed($"Existing Line with ID {request.LineId.Value} not found for modification.");

                // Normalize double-escaped regex patterns from DeepSeek JSON responses
                string normalizedCompleteLineRegex = !string.IsNullOrEmpty(regexResp.CompleteLineRegex) && regexResp.CompleteLineRegex.Contains("\\\\")
                    ? regexResp.CompleteLineRegex.Replace("\\\\", "\\")
                    : regexResp.CompleteLineRegex;

                if (existingLineDbEntity.RegularExpressions == null)
                {
                    var newRegexForExistingLine = await this.GetOrCreateRegexAsync(context, normalizedCompleteLineRegex, regexResp.IsMultiline, regexResp.MaxLines, $"Modified for omission: {request.FieldName}").ConfigureAwait(false);
                    // Intentionally not saving here, GetOrCreateRegexAsync just prepares. Main SaveChangesAsync will commit.
                    if (newRegexForExistingLine.TrackingState == TrackingState.Added) await context.SaveChangesAsync().ConfigureAwait(false); // Save if new to get ID
                    existingLineDbEntity.RegExId = newRegexForExistingLine.Id;
                    _logger.Warning("Existing Line {LineId} had no Regex assigned. Created and assigned new Regex ID {RegexId}", existingLineDbEntity.Id, newRegexForExistingLine.Id);
                }
                else
                {
                    // PHASE 3: Database Pattern Persistence Fix - Enhanced with verification
                    var oldPattern = existingLineDbEntity.RegularExpressions.RegEx;
                    existingLineDbEntity.RegularExpressions.RegEx = normalizedCompleteLineRegex;
                    existingLineDbEntity.RegularExpressions.MultiLine = regexResp.IsMultiline;
                    existingLineDbEntity.RegularExpressions.MaxLines = regexResp.MaxLines;
                    existingLineDbEntity.RegularExpressions.LastUpdated = DateTime.UtcNow;
                    
                    _logger.Error("🔄 **PATTERN_UPDATE**: LineId {LineId} | Old: '{Old}' → New: '{New}'", 
                        existingLineDbEntity.Id, oldPattern, normalizedCompleteLineRegex);
                    // No need to set TrackingState if EF tracks changes on loaded entities
                }
                // No need to set existingLineDbEntity.TrackingState = TrackingState.Modified explicitly if EF change tracking is on.

                string dbFieldName = fieldInfo?.DatabaseFieldName ?? request.FieldName;
                string entityType = fieldInfo?.EntityType ?? (request.FieldName.Contains("InvoiceDetail") ? "InvoiceDetails" : "ShipmentInvoice");
                string dataType = fieldInfo?.DataType ?? "string";

                var newFieldEntity = await this.GetOrCreateFieldAsync(context, request.FieldName, dbFieldName, entityType, dataType, request.LineId.Value).ConfigureAwait(false);

                _logger.Error("🔍 **DATABASE_COMMIT_MODIFY_LINE**: About to save modified line to database - LineId={LineId} | RegexId={RegexId}", 
                    existingLineDbEntity.Id, existingLineDbEntity.RegExId);
                
                // PHASE 3: Database Pattern Persistence Fix - Enhanced with verification
                await context.SaveChangesAsync().ConfigureAwait(false); // Commit all prepared changes for this operation
                
                // CRITICAL: Verify the pattern was actually saved to database
                using (var verifyCtx = new OCRContext())
                {
                    var verifyLine = await verifyCtx.Lines.Include(l => l.RegularExpressions)
                        .FirstOrDefaultAsync(l => l.Id == existingLineDbEntity.Id).ConfigureAwait(false);
                    var savedPattern = verifyLine?.RegularExpressions?.RegEx;
                    
                    if (savedPattern == normalizedCompleteLineRegex)
                    {
                        _logger.Error("✅ **DATABASE_SAVE_VERIFIED**: Pattern successfully persisted to database for LineId {LineId}", existingLineDbEntity.Id);
                    }
                    else
                    {
                        _logger.Error("❌ **DATABASE_SAVE_FAILED**: Expected '{Expected}', Found '{Actual}' for LineId {LineId}", 
                            normalizedCompleteLineRegex, savedPattern, existingLineDbEntity.Id);
                        return new DatabaseUpdateResult { IsSuccess = false, Message = "Pattern save verification failed" };
                    }
                }
                
                _logger.Error("✅ **DATABASE_COMMIT_SUCCESS_MODIFY**: Successfully modified Line {LineId} (Regex {RegexId}) and added/verified Field {FieldId} for omitted field {OmittedFieldName}",
                    existingLineDbEntity.Id, existingLineDbEntity.RegExId, newFieldEntity.Id, request.FieldName);
                return DatabaseUpdateResult.Success(newFieldEntity.Id, "Modified existing line for omission");
            }

            private async Task<DatabaseUpdateResult> CreateNewLineForOmissionAsync(OCRContext context, RegexUpdateRequest request, RegexCreationResponse regexResp, DatabaseFieldInfo fieldInfo, OCRCorrectionService serviceInstance)
            {
                _logger.Error("🔍 **CREATE_NEW_LINE_START**: Creating new line for omission - FieldName={FieldName} | Pattern={Pattern}", 
                    request.FieldName, regexResp.RegexPattern);
                
                // Normalize double-escaped regex patterns from DeepSeek JSON responses
                string normalizedPattern = regexResp.RegexPattern.Contains("\\\\") 
                    ? regexResp.RegexPattern.Replace("\\\\", "\\") 
                    : regexResp.RegexPattern;
                    
                _logger.Error("🔍 **CREATE_NEW_REGEX_NORMALIZED**: Pattern normalization - Original={Original} | Normalized={Normalized}", 
                    regexResp.RegexPattern, normalizedPattern);
                
                var newRegexEntity = await this.GetOrCreateRegexAsync(context, normalizedPattern, regexResp.IsMultiline, regexResp.MaxLines, $"For omitted field: {request.FieldName}").ConfigureAwait(false);
                
                _logger.Error("🔍 **CREATE_NEW_REGEX**: Created/found regex entity - RegexId={RegexId} | Pattern={Pattern} | TrackingState={TrackingState}", 
                    newRegexEntity.Id, newRegexEntity.RegEx, newRegexEntity.TrackingState.ToString());
                
                if (newRegexEntity.TrackingState == TrackingState.Added) {
                    _logger.Error("🔍 **DATABASE_SAVE_NEW_REGEX**: Saving new regex to get ID");
                    await context.SaveChangesAsync().ConfigureAwait(false);
                    _logger.Error("✅ **DATABASE_SAVE_NEW_REGEX_SUCCESS**: New regex saved with ID={RegexId}", newRegexEntity.Id);
                }

                var lineName = $"AutoOmission_{request.FieldName.Replace(" ", "_").Substring(0, Math.Min(request.FieldName.Length, 40))}_{DateTime.Now:HHmmssfff}";
                
                var newLineEntity = new Lines
                {
                    PartId = request.PartId.Value,
                    RegExId = newRegexEntity.Id,
                    Name = lineName,
                    IsActive = true,
                    // SortOrder logic removed assuming Lines entity does not have it. Add back if it does.
                    // SortOrder = (await context.Lines.Where(l => l.PartId == request.PartId.Value).MaxAsync(l => (int?)l.SortOrder) ?? 0) + 10, 
                    TrackingState = TrackingState.Added
                };
                
                _logger.Error("🔍 **CREATE_NEW_LINE_ENTITY**: Creating line entity - LineName={LineName} | PartId={PartId} | RegexId={RegexId}", 
                    lineName, request.PartId.Value, newRegexEntity.Id);
                
                context.Lines.Add(newLineEntity);
                
                _logger.Error("🔍 **DATABASE_SAVE_NEW_LINE**: About to save new line to database");
                await context.SaveChangesAsync().ConfigureAwait(false);
                _logger.Error("✅ **DATABASE_SAVE_NEW_LINE_SUCCESS**: New line saved with ID={LineId}", newLineEntity.Id);

                string dbFieldName = fieldInfo?.DatabaseFieldName ?? request.FieldName;
                string entityType = fieldInfo?.EntityType ?? (request.FieldName.Contains("InvoiceDetail") ? "InvoiceDetails" : "ShipmentInvoice");
                string dataType = fieldInfo?.DataType ?? "string";

                _logger.Error("🔍 **CREATE_NEW_FIELD**: Creating field entity - FieldName={FieldName} | DbFieldName={DbFieldName} | EntityType={EntityType} | DataType={DataType} | LineId={LineId}", 
                    request.FieldName, dbFieldName, entityType, dataType, newLineEntity.Id);

                var newFieldEntity = await this.GetOrCreateFieldAsync(context, request.FieldName, dbFieldName, entityType, dataType, newLineEntity.Id).ConfigureAwait(false);

                _logger.Error("🔍 **DATABASE_SAVE_NEW_FIELD**: About to save new field to database");
                await context.SaveChangesAsync().ConfigureAwait(false);
                _logger.Error("✅ **DATABASE_SAVE_SUCCESS_CREATE**: Successfully created new Line {NewLineId} (Regex {RegexId}) and Field {NewFieldId} for omitted field {OmittedFieldName}",
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

            public IDatabaseUpdateStrategy GetStrategy(RegexUpdateRequest request)
            {
                if (request == null) throw new ArgumentNullException(nameof(request));

                // Prioritize specific strategies
                if (new OmissionUpdateStrategy(_logger).CanHandle(request))
                {
                    return new OmissionUpdateStrategy(_logger);
                }
                if (new FieldFormatUpdateStrategy(_logger).CanHandle(request))
                {
                    return new FieldFormatUpdateStrategy(_logger);
                }

                // Default or fallback strategy (if any)
                throw new InvalidOperationException($"No suitable update strategy found for correction type: {request.CorrectionType}");
            }
        }
        #endregion
    }
}