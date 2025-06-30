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
    OCRContext context, string pattern, bool multiLine = false, int maxLines = 1, string description = null)
            {
                // Checks DB first, then local cache, then creates. THIS IS THE CORRECT, FIXED VERSION.
                var existingRegex = await context.RegularExpressions
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(r => r.RegEx == pattern && r.MultiLine == multiLine && r.MaxLines == maxLines).ConfigureAwait(false);
                if (existingRegex != null)
                {
                    _logger.Debug("Found existing regex pattern in DB (ID: {RegexId}): {Pattern}", existingRegex.Id, pattern);
                    return existingRegex;
                }

                var localRegex = context.RegularExpressions.Local
                                     .FirstOrDefault(r => r.RegEx == pattern && r.MultiLine == multiLine && r.MaxLines == maxLines);
                if (localRegex != null)
                {
                    _logger.Debug("Found existing regex pattern in LOCAL CACHE (ID: {RegexId}): {Pattern}", localRegex.Id, pattern);
                    return localRegex;
                }

                // CRITICAL FIX: Create and IMMEDIATELY save regex to get database ID
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
                
                // IMMEDIATELY save to database to get ID and prevent relationship conflicts
                await context.SaveChangesAsync().ConfigureAwait(false);
                _logger.Debug("🔧 **REGEX_SAVED_IMMEDIATELY**: Created and saved new regex to database, got ID={RegexId}, Pattern={Pattern}", newRegex.Id, pattern);
                
                return newRegex;
            }

        

            //protected async Task<Fields> GetOrCreateFieldAsync(
            //    OCRContext context,
            //    string fieldKey,
            //    string dbFieldName,
            //    string entityType,
            //    string dataType,
            //    int lineId,
            //    bool isRequired = false,
            //    bool appendValues = true)
            //{
            //    var existingField = await context.Fields.FirstOrDefaultAsync(
            //                                f => f.LineId == lineId && ((!string.IsNullOrEmpty(fieldKey) && f.Key == fieldKey) || (string.IsNullOrEmpty(fieldKey) && f.Field == dbFieldName)))
            //                            .ConfigureAwait(false);

            //    if (existingField != null)
            //    {
            //        _logger.Debug("Found existing field definition (ID: {FieldId}) for LineId {LineId}, Key '{Key}', DBField '{DbField}'", existingField.Id, lineId, fieldKey, dbFieldName);
            //        return existingField;
            //    }

            //    var newField = new Fields
            //    {
            //        LineId = lineId,
            //        Key = fieldKey,
            //        Field = dbFieldName,
            //        EntityType = entityType,
            //        DataType = dataType,
            //        IsRequired = isRequired,
            //        AppendValues = appendValues,
            //        TrackingState = TrackingState.Added
            //    };
            //    context.Fields.Add(newField);
            //    _logger.Information("Prepared new field definition for LineId {LineId}, Key '{Key}', DBField '{DbField}'", lineId, fieldKey, dbFieldName);
            //    return newField;
            //}

            /// <summary>
            /// Calculates the maximum lines needed for a regex pattern based on context lines.
            /// </summary>
            protected int CalculateMaxLinesFromContext(RegexUpdateRequest request)
            {
                int contextLines = 0;
                if (request.ContextLinesBefore?.Count > 0) contextLines += request.ContextLinesBefore.Count;
                if (request.ContextLinesAfter?.Count > 0) contextLines += request.ContextLinesAfter.Count;
                
                // Default to 1 for single line, or context + 2 for multiline patterns
                return request.RequiresMultilineRegex ? Math.Max(contextLines + 2, 3) : 1;
            }

            /// <summary>
            /// Extracts all named group names from a regex pattern for multi-field support.
            /// </summary>
            protected List<string> ExtractNamedGroupsFromRegex(string regexPattern)
            {
                var namedGroups = new List<string>();
                if (string.IsNullOrEmpty(regexPattern)) return namedGroups;

                try
                {
                    // Use regex to find named groups: ?<name>
                    var groupPattern = @"\(\?<([^>]+)>";
                    var matches = System.Text.RegularExpressions.Regex.Matches(regexPattern, groupPattern);
                    
                    foreach (System.Text.RegularExpressions.Match match in matches)
                    {
                        if (match.Groups.Count > 1)
                        {
                            var groupName = match.Groups[1].Value;
                            if (!namedGroups.Contains(groupName))
                            {
                                namedGroups.Add(groupName);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    _logger.Warning(ex, "Failed to extract named groups from regex pattern: {Pattern}", regexPattern);
                }

                return namedGroups;
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

            // =============================== FIX 1: MOVE HELPER METHOD HERE ===============================
            protected async Task<int?> DeterminePartIdForNewOmissionLineAsync(
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
            // ===================================== END OF FIX 1 =====================================
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

                bool isExplicitFormatCorrection =
                    request.CorrectionType == "FieldFormat" ||
                    request.CorrectionType == "FORMAT_FIX" ||
                    request.CorrectionType == "format_correction" ||
                    request.CorrectionType == "decimal_separator" ||
                    request.CorrectionType == "DecimalSeparator" ||
                    request.CorrectionType == "character_confusion";

                if (isExplicitFormatCorrection)
                {
                    return true;
                }

                return IsPotentialFormatCorrection(request.OldValue, request.NewValue);
            }

            private bool IsPotentialFormatCorrection(string oldValue, string newValue)
            {
                if (string.IsNullOrEmpty(oldValue) || string.IsNullOrEmpty(newValue) || oldValue == newValue) return false;

                var oldNormalized = System.Text.RegularExpressions.Regex.Replace(oldValue, @"[\s\$,€£\-()]", "");
                var newNormalized = System.Text.RegularExpressions.Regex.Replace(newValue, @"[\s\$,€£\-()]", "");

                return string.Equals(oldNormalized, newNormalized, StringComparison.OrdinalIgnoreCase);
            }

            public override async Task<DatabaseUpdateResult> ExecuteAsync(OCRContext context, RegexUpdateRequest request, OCRCorrectionService serviceInstance)
            {
                _logger.Information("         - **STRATEGY_INTENT (FieldFormat)**: To create a formatting rule (pattern/replacement) and attach it to an EXISTING Field definition to transform captured data.");
                try
                {
                    // =============================== STEP 1: VALIDATE PRECONDITIONS ===============================
                    _logger.Information("           - [STEP 1] Validating preconditions for the format rule.");
                    if (!request.FieldId.HasValue)
                    {
                        var reason = $"Precondition Failed: The format_correction request for '{request.FieldName}' is missing the FieldId it needs to attach to. This usually happens if its paired 'omission' rule failed to save first.";
                        _logger.Warning("           - ❌ {Reason}", reason);
                        return DatabaseUpdateResult.Failed(reason);
                    }
                    int fieldDefinitionId = request.FieldId.Value;
                    _logger.Information("           - ✅ Precondition met: FieldId is present ({FieldId}).", fieldDefinitionId);

                    if (string.IsNullOrEmpty(request.Pattern) || request.Replacement == null)
                    {
                        var reason = $"Precondition Failed: AI-generated format correction for '{request.FieldName}' was incomplete (missing pattern or replacement).";
                        _logger.Warning("           - ❌ {Reason}", reason);
                        return DatabaseUpdateResult.Failed(reason);
                    }
                    _logger.Information("           - ✅ Precondition met: Pattern ('{Pattern}') and Replacement ('{Replacement}') are present.", request.Pattern, request.Replacement);

                    // =================================== FIX START ===================================
                    // This summary log block has been restored for clarity.
                    _logger.Information("   -> [DB_SAVE_INTENT]: Preparing to create OCR_FieldFormatRegEx entry.");
                    _logger.Information("      - FieldId: {FieldId}", fieldDefinitionId);
                    _logger.Information("      - Pattern Regex: '{Pattern}'", request.Pattern);
                    _logger.Information("      - Replacement Regex: '{Replacement}'", request.Replacement);
                    // ==================================== FIX END ====================================

                    // =============================== STEP 2: GET OR CREATE REGEX ENTITIES ===============================
                    _logger.Information("           - [STEP 2] Getting/creating Regex entity for the PATTERN string: '{Pattern}'", request.Pattern);
                    var patternRegexEntity = await this.GetOrCreateRegexAsync(context, request.Pattern, description: $"Pattern for format fix: {request.FieldName}").ConfigureAwait(false);
                    _logger.Information("           - ✅ Got/created pattern Regex entity with ID: {RegexId}", patternRegexEntity.Id);

                    _logger.Information("           - [STEP 2] Getting/creating Regex entity for the REPLACEMENT string: '{Replacement}'", request.Replacement);
                    var replacementRegexEntity = await this.GetOrCreateRegexAsync(context, request.Replacement, description: $"Replacement for format fix: {request.FieldName}").ConfigureAwait(false);
                    _logger.Information("           - ✅ Got/created replacement Regex entity with ID: {RegexId}", replacementRegexEntity.Id);

                    // =============================== STEP 3: PREPARE AND SAVE THE FORMAT RULE ===============================
                    _logger.Information("           - [STEP 3] Preparing to create the final FieldFormatRegEx link.");
                    _logger.Information("             - **EXPECTED_BEHAVIOR**: Creating new FieldFormatRegEx to link Field ID {FieldId} with Pattern Regex ID {PatternId} and Replacement Regex ID {ReplacementId}.", fieldDefinitionId, patternRegexEntity.Id, replacementRegexEntity.Id);

                    var newFieldFormatRegex = new FieldFormatRegEx
                    {
                        FieldId = fieldDefinitionId,
                        RegEx = patternRegexEntity,
                        ReplacementRegEx = replacementRegexEntity,
                        TrackingState = TrackingState.Added
                    };
                    context.OCR_FieldFormatRegEx.Add(newFieldFormatRegex);

                    try
                    {
                        // This is the atomic commit for this specific strategy.
                        await SaveChangesWithAssertiveLogging(context, "CreateFieldFormatRule").ConfigureAwait(false);
                        _logger.Information("           - ✅ [STEP 3] SaveChanges completed successfully.");
                        return DatabaseUpdateResult.Success(newFieldFormatRegex.Id, "Created FieldFormatRegEx");
                    }
                    // =============================== FIX START: RESTORED ERROR HANDLING ===============================
                    catch (System.Data.Entity.Infrastructure.DbUpdateException ex) when (ex.InnerException?.Message?.Contains("primary key") == true || ex.InnerException?.Message?.Contains("UNIQUE constraint") == true)
                    {
                        _logger.Warning(ex, "           - ⚠️ **DEVIATION_FROM_EXPECTED (DB Conflict)**: A primary key or unique constraint violation occurred. This can happen in high-concurrency scenarios where another process created the same rule between our read and write. Attempting to recover by finding the existing rule.");

                        // Detach the failed entities from the context to clear the error state.
                        context.ChangeTracker.Entries().Where(e => e.State == EntityState.Added).ToList().ForEach(e => e.State = EntityState.Detached);

                        // Now, query the database to find the rule that must now exist.
                        var existingRule = await context.OCR_FieldFormatRegEx
                            .AsNoTracking()
                            .FirstOrDefaultAsync(ffr => ffr.FieldId == fieldDefinitionId &&
                                                      ffr.RegEx.RegEx == request.Pattern &&
                                                      ffr.ReplacementRegEx.RegEx == request.Replacement)
                            .ConfigureAwait(false);

                        if (existingRule != null)
                        {
                            _logger.Information("           - ✅ **RECOVERY_SUCCESS**: Found existing FieldFormatRegEx rule after conflict (ID: {RuleId}). The operation is considered a success.", existingRule.Id);
                            return DatabaseUpdateResult.Success(existingRule.Id, "Found existing FieldFormatRegEx after conflict");
                        }

                        _logger.Error("           - ❌ **RECOVERY_FAILED**: Could not find or create FieldFormatRegEx rule after primary key conflict. This indicates a more serious issue.");
                        return DatabaseUpdateResult.Failed($"Primary key conflict occurred, but recovery failed for field '{request.FieldName}': {ex.Message}");
                    }
                    // ================================ FIX END: RESTORED ERROR HANDLING ================================
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "         - Strategy execution failed for {FieldName}", request.FieldName);
                    return DatabaseUpdateResult.Failed($"Database error in FieldFormatUpdateStrategy: {ex.Message}", ex);
                }
            }
        }

        #endregion



        #region Omission Update Strategy (With Final Fix)

        public class OmissionUpdateStrategy : DatabaseUpdateStrategyBase
        {
            public OmissionUpdateStrategy(ILogger logger) : base(logger) { }
            public override string StrategyType => "Omission";
            public override bool CanHandle(RegexUpdateRequest request) => 
                request.CorrectionType.StartsWith("omission") || 
                request.CorrectionType.Equals("multi_field_omission", StringComparison.OrdinalIgnoreCase);

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

                            return await CreateNewLineForOmissionAsync(context, request, regexResponse, fieldMappingInfo, serviceInstance).ConfigureAwait(false);
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
                DatabaseFieldInfo fieldInfo,
                OCRCorrectionService serviceInstance)
            {
                _logger.Error("  - [DB_SAVE_INTENT]: Preparing to create new Line, Field, and Regex for Omission of '{FieldName}'.", request.FieldName);
                string normalizedPattern = regexResp.RegexPattern.Replace("\\\\", "\\");

                // Step 1: Prepare all entities in memory without saving.
                // Enhanced with multiline support from DeepSeek RequiresMultilineRegex flag
                bool isMultiline = regexResp.IsMultiline || request.RequiresMultilineRegex;
                int maxLines = regexResp.MaxLines > 0 ? regexResp.MaxLines : CalculateMaxLinesFromContext(request);
                var newRegexEntity = await this.GetOrCreateRegexAsync(context, normalizedPattern, isMultiline, maxLines, $"For omitted field: {request.FieldName}").ConfigureAwait(false);

                var newLineEntity = new Lines { PartId = request.PartId.Value, Name = $"AutoOmission_{request.FieldName.Replace(" ", "_").Substring(0, Math.Min(request.FieldName.Length, 40))}_{DateTime.Now:HHmmssfff}", IsActive = true, TrackingState = TrackingState.Added };
                // Associate the regex with the line. EF will handle the foreign key relationship.
                newLineEntity.RegularExpressions = newRegexEntity;
                context.Lines.Add(newLineEntity);

                // Enhanced: Multi-field regex support - extract all named groups from regex
                var namedGroups = ExtractNamedGroupsFromRegex(normalizedPattern);
                _logger.Information("  - Multi-field support: Found {GroupCount} named groups in regex: {Groups}", 
                    namedGroups.Count, string.Join(", ", namedGroups));

                // Create Field entries for each named group
                var createdFields = new List<Fields>();
                foreach (var groupName in namedGroups)
                {
                    // Map the group name to database field info
                    var groupFieldInfo = serviceInstance.MapDeepSeekFieldToDatabase(groupName);
                    if (groupFieldInfo == null)
                    {
                        _logger.Warning("  - Skipping named group '{GroupName}': No database field mapping found", groupName);
                        continue;
                    }

                    bool shouldAppend = groupFieldInfo.DatabaseFieldName == "TotalDeduction" || 
                                      groupFieldInfo.DatabaseFieldName == "TotalOtherCost" || 
                                      groupFieldInfo.DatabaseFieldName == "TotalInsurance" || 
                                      groupFieldInfo.DatabaseFieldName == "TotalInternalFreight";
                    
                    var fieldEntity = new Fields
                    {
                        Key = groupName,
                        Field = groupFieldInfo.DatabaseFieldName,
                        EntityType = groupFieldInfo.EntityType,
                        DataType = groupFieldInfo.DataType,
                        IsRequired = false,
                        AppendValues = shouldAppend,
                        TrackingState = TrackingState.Added
                    };
                    
                    newLineEntity.Fields.Add(fieldEntity);
                    createdFields.Add(fieldEntity);
                    _logger.Information("  - Created field mapping: '{GroupName}' → '{DatabaseField}' (EntityType: {EntityType})", 
                        groupName, groupFieldInfo.DatabaseFieldName, groupFieldInfo.EntityType);
                }

                if (createdFields.Count == 0)
                {
                    _logger.Error("  - ❌ No valid field mappings created from regex named groups. Cannot proceed.");
                    return DatabaseUpdateResult.Failed($"No valid database field mappings found for regex groups in pattern: {normalizedPattern}");
                }

                // Step 2: Commit all prepared entities in a single transaction.
                await SaveChangesWithAssertiveLogging(context, "CreateNewLineForOmission").ConfigureAwait(false);

                var fieldIds = string.Join(", ", createdFields.Select(f => f.Id.ToString()));
                var fieldNames = string.Join(", ", createdFields.Select(f => f.Field));
                _logger.Information("Successfully created new Line (ID: {LineId}), {FieldCount} Fields (IDs: {FieldIds} - {FieldNames}), and Regex (ID: {RegexId}) for omission.", 
                    newLineEntity.Id, createdFields.Count, fieldIds, fieldNames, newRegexEntity.Id);

                // Return LineId as the primary RecordId and the first field ID as the RelatedRecordId.
                return DatabaseUpdateResult.Success(newLineEntity.Id, $"Created new line with {createdFields.Count} fields and regex for omission", createdFields.First().Id);
            }


        }
        #endregion


        #region Inferred Value Update Strategy

        public class InferredValueUpdateStrategy : DatabaseUpdateStrategyBase
        {
            public InferredValueUpdateStrategy(ILogger logger) : base(logger) { }
            public override string StrategyType => "Inferred";
            public override bool CanHandle(RegexUpdateRequest request) => request.CorrectionType == "inferred";

            public override async Task<DatabaseUpdateResult> ExecuteAsync(OCRContext context, RegexUpdateRequest request, OCRCorrectionService serviceInstance)
            {
                _logger.Error("🔍 **INFERRED_STRATEGY_START**: Executing for field: {FieldName}", request.FieldName);

                if (string.IsNullOrEmpty(request.FieldName) || string.IsNullOrEmpty(request.NewValue) || string.IsNullOrEmpty(request.SuggestedRegex))
                {
                    _logger.Error("   - ❌ **STRATEGY_FAIL**: Request is missing FieldName, NewValue (static value), or SuggestedRegex (line finder). Aborting strategy.");
                    return DatabaseUpdateResult.Failed("Request for inferred value is missing critical fields.");
                }

                try
                {
                    var fieldMappingInfo = serviceInstance.MapDeepSeekFieldToDatabase(request.FieldName);
                    if (fieldMappingInfo == null) return DatabaseUpdateResult.Failed($"Unknown field mapping for '{request.FieldName}'.");

                    // Determine the PartId for the new rule
                    // This now calls the inherited helper method from the base class.
                    if (!request.PartId.HasValue) request.PartId = await DeterminePartIdForNewOmissionLineAsync(context, fieldMappingInfo, request).ConfigureAwait(false);
                    if (!request.PartId.HasValue) return DatabaseUpdateResult.Failed("Cannot determine PartId for new inferred value rule.");

                    // Create the new Line and Field with the static value
                    return await CreateNewLineForInferredValueAsync(context, request, fieldMappingInfo).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Failed to execute InferredValueUpdateStrategy for {FieldName}", request.FieldName);
                    return DatabaseUpdateResult.Failed($"Inferred value strategy database error: {ex.Message}", ex);
                }
            }

            private async Task<DatabaseUpdateResult> CreateNewLineForInferredValueAsync(
                OCRContext context,
                RegexUpdateRequest request,
                DatabaseFieldInfo fieldInfo)
            {
                _logger.Error("  - [DB_SAVE_INTENT]: Preparing to create new Line and static FieldValue for Inferred Value of '{FieldName}'.", request.FieldName);
                string lineFinderPattern = request.SuggestedRegex.Replace("\\\\", "\\");

                // Step 1: Get or create the regex for the line finder.
                var newRegexEntity = await this.GetOrCreateRegexAsync(context, lineFinderPattern, request.RequiresMultilineRegex, 1, $"Line finder for inferred value: {request.FieldName}").ConfigureAwait(false);

                // Step 2: Create the new Line definition.
                var newLineEntity = new Lines
                {
                    PartId = request.PartId.Value,
                    Name = $"AutoInferred_{request.FieldName.Replace(" ", "_").Substring(0, Math.Min(request.FieldName.Length, 40))}_{DateTime.Now:HHmmssfff}",
                    IsActive = true,
                    TrackingState = TrackingState.Added
                };
                newLineEntity.RegularExpressions = newRegexEntity;
                context.Lines.Add(newLineEntity);

                // Step 3: Create the Field definition and its associated STATIC FieldValue entity.
                var newFieldEntity = new Fields
                {
                    // =============================== FIX 1 START ===============================
                    // Key cannot be null. For static values, using the field name is a robust convention.
                    Key = request.FieldName,
                    // ================================ FIX 1 END ================================
                    Field = fieldInfo.DatabaseFieldName,
                    EntityType = fieldInfo.EntityType,
                    DataType = fieldInfo.DataType,
                    IsRequired = false,
                    AppendValues = false, // Static values should not append.
                    TrackingState = TrackingState.Added,

                    // =============================== YOUR CORRECT FIX IS HERE ===============================
                    // Create a new OCR_FieldValue entity and assign it.
                    // This assumes 'FieldValue' is the name of the navigation property on the 'Fields' entity.
                    // If the navigation property is a collection (e.g., 'FieldValues'), you would use .Add().
                    // Assuming a one-to-one relationship for simplicity here.
                    FieldValue = new OCR_FieldValue()
                    {
                        Value = request.NewValue,
                        TrackingState = TrackingState.Added
                    }
                    // ========================================================================================
                };

                _logger.Error("  - ✅ **INFERRED_RULE_DETECTED**: This is an inferred value rule. A new OCR_FieldValue entity will be created with the static value '{StaticValue}'.", request.NewValue);

                newLineEntity.Fields.Add(newFieldEntity);

                // Step 4: Save everything in one transaction.
                await SaveChangesWithAssertiveLogging(context, "CreateNewLineForInferredValue").ConfigureAwait(false);

                _logger.Information("Successfully created new Line (ID: {LineId}) and Field (ID: {FieldId}) with static FieldValue for inferred field.", newLineEntity.Id, newFieldEntity.Id);
                return DatabaseUpdateResult.Success(newLineEntity.Id, "Created new line and static field value for inferred value", newFieldEntity.Id);
            }
        }

        #endregion

        #region Strategy Factory

        public class DatabaseUpdateStrategyFactory
        {
            private readonly ILogger _logger;
            private readonly List<IDatabaseUpdateStrategy> _strategies;

            public DatabaseUpdateStrategyFactory(ILogger logger)
            {
                _logger = logger;
                _strategies = new List<IDatabaseUpdateStrategy>
                {
                    new OCRCorrectionService.TemplateCreationStrategy(_logger),
                    new InferredValueUpdateStrategy(_logger),
                    new OmissionUpdateStrategy(_logger),
                    new FieldFormatUpdateStrategy(_logger)
                };
            }

            public IDatabaseUpdateStrategy GetStrategy(RegexUpdateRequest request)
            {
                if (request == null) throw new ArgumentNullException(nameof(request));

                foreach (var strategy in _strategies)
                {
                    if (strategy.CanHandle(request)) return strategy;
                }

                _logger.Warning("No suitable update strategy found for correction type: {CorrectionType}", request.CorrectionType);
                throw new InvalidOperationException($"No suitable update strategy found for correction type: {request.CorrectionType}");
            }
        }
        #endregion
    }
}