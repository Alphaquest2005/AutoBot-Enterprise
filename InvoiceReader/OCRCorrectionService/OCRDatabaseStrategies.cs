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

            /// <summary>
            /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v4.1**: Regex entity creation/retrieval with LLM diagnostic workflow
            /// 
            /// **MANDATORY LLM BEHAVIOR RULES**: LOG PRESERVATION + LOG-FIRST ANALYSIS + CONTINUOUS LOG ENHANCEMENT
            /// **LLM DIAGNOSTIC WORKFLOW**: Phase 1 Analysis → Phase 2 Enhancement → Phase 3 Evidence-Based Implementation
            /// **REGEX MANAGEMENT**: Database-first lookup → local cache → immediate creation with ID assignment
            /// **RELATIONSHIP SAFETY**: Immediate save prevents foreign key conflicts in subsequent strategy operations
            /// **DIAGNOSTIC INTEGRATION**: Complete logging for LLM analysis of regex creation and caching workflows
            /// </summary>
            protected async Task<RegularExpressions> GetOrCreateRegexAsync(
    OCRContext context, string pattern, bool multiLine = false, int maxLines = 1, string description = null)
            {
                // 🧠 **ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v4.1**: Complete LLM diagnostic workflow for regex management
                
                // **STEP 1: MANDATORY LOG ANALYSIS PHASE**
                _logger.Error("🔍 **LLM_DIAGNOSTIC_PHASE_1**: Comprehensive log analysis starting for regex entity management");
                _logger.Error("📋 **AVAILABLE_LOG_DATA**: Regex lookup context with database-first pattern retrieval");
                _logger.Error("🔍 **PATTERN_ANALYSIS**: Database → local cache → immediate creation pattern with relationship safety");
                _logger.Error("❓ **EVIDENCE_GAPS**: Need database lookup confirmation, cache hit validation, creation success verification");
                _logger.Error("💡 **LOG_BASED_HYPOTHESIS**: Regex management requires comprehensive lookup and immediate save for foreign key safety");
                
                // **STEP 2: MANDATORY LOG ENHANCEMENT PHASE**
                _logger.Error("🔧 **LLM_DIAGNOSTIC_PHASE_2**: Enhancing logging to capture missing evidence for regex operations");
                _logger.Error("📊 **LOGGING_ENHANCEMENTS**: Adding detailed lookup results, cache status, creation outcomes");
                _logger.Error("🎯 **ENHANCED_CAPTURE_POINTS**: Database hits, cache hits, entity creation, ID assignment");
                
                // **STEP 3: MANDATORY EVIDENCE-BASED FIX PHASE**
                _logger.Error("🎯 **LLM_DIAGNOSTIC_PHASE_3**: Implementing evidence-based regex entity management");
                _logger.Error("📚 **FIX_RATIONALE**: Based on foreign key safety requirements, implementing immediate save pattern");
                _logger.Error("🔍 **FIX_VALIDATION**: Will validate success by monitoring ID assignment and relationship integrity");
                
                // **v4.1 DATABASE LOOKUP LOGGING**: Enhanced database-first pattern retrieval
                _logger.Error("🔍 **REGEX_LOOKUP_START**: Beginning database-first regex pattern lookup");
                _logger.Error("📋 **AVAILABLE_LOG_DATA**: Lookup parameters - Pattern='{Pattern}', MultiLine={MultiLine}, MaxLines={MaxLines}", 
                    pattern, multiLine, maxLines);
                
                // Checks DB first, then local cache, then creates. THIS IS THE CORRECT, FIXED VERSION.
                var existingRegex = await context.RegularExpressions
                                        .AsNoTracking()
                                        .FirstOrDefaultAsync(r => r.RegEx == pattern && r.MultiLine == multiLine && r.MaxLines == maxLines).ConfigureAwait(false);
                if (existingRegex != null)
                {
                    // **v4.1 DATABASE HIT LOGGING**: LLM diagnostic evidence for successful database retrieval
                    _logger.Error("✅ **DATABASE_HIT_EVIDENCE**: Found existing regex pattern in database");
                    _logger.Error("📋 **AVAILABLE_LOG_DATA**: Database hit - RegexId={RegexId}, Pattern='{Pattern}'", existingRegex.Id, pattern);
                    _logger.Error("🔍 **PATTERN_ANALYSIS**: Database lookup successful, returning existing entity for relationship safety");
                    _logger.Error("💡 **LOG_BASED_HYPOTHESIS**: Database hit provides optimal performance with established entity relationships");
                    _logger.Error("📚 **FIX_RATIONALE**: Existing regex entities eliminate creation overhead and relationship conflicts");
                    _logger.Error("🔍 **FIX_VALIDATION**: Database hit confirmed, regex entity ready for immediate use");
                    return existingRegex;
                }

                // **v4.1 LOCAL CACHE LOGGING**: Enhanced local cache lookup with diagnostic evidence
                _logger.Error("🔍 **LOCAL_CACHE_LOOKUP**: Database miss - checking local cache for regex pattern");
                var localRegex = context.RegularExpressions.Local
                                     .FirstOrDefault(r => r.RegEx == pattern && r.MultiLine == multiLine && r.MaxLines == maxLines);
                if (localRegex != null)
                {
                    // **v4.1 CACHE HIT LOGGING**: LLM diagnostic evidence for local cache retrieval
                    _logger.Error("✅ **CACHE_HIT_EVIDENCE**: Found existing regex pattern in local cache");
                    _logger.Error("📋 **AVAILABLE_LOG_DATA**: Cache hit - RegexId={RegexId}, Pattern='{Pattern}'", localRegex.Id, pattern);
                    _logger.Error("🔍 **PATTERN_ANALYSIS**: Local cache successful, returning cached entity for relationship safety");
                    _logger.Error("💡 **LOG_BASED_HYPOTHESIS**: Cache hit provides intermediate performance with pending entity relationships");
                    _logger.Error("📚 **FIX_RATIONALE**: Cached regex entities provide efficiency while maintaining relationship integrity");
                    _logger.Error("🔍 **FIX_VALIDATION**: Cache hit confirmed, regex entity ready for immediate use");
                    return localRegex;
                }

                // **v4.1 ENTITY CREATION LOGGING**: Enhanced immediate creation with relationship safety
                _logger.Error("🔧 **REGEX_CREATION_START**: Database and cache miss - creating new regex entity");
                _logger.Error("📊 **LOGGING_ENHANCEMENTS**: Enhanced creation with immediate save for foreign key safety");
                
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
                
                // **v4.1 IMMEDIATE SAVE LOGGING**: LLM diagnostic evidence for relationship safety
                _logger.Error("🎯 **ENHANCED_CAPTURE_POINTS**: Immediate save operation for foreign key relationship safety");
                _logger.Error("📚 **FIX_RATIONALE**: Immediate save prevents foreign key conflicts in subsequent strategy operations");
                
                // IMMEDIATELY save to database to get ID and prevent relationship conflicts
                await context.SaveChangesAsync().ConfigureAwait(false);
                
                _logger.Error("✅ **REGEX_CREATION_SUCCESS**: Created and saved new regex to database with ID assignment");
                _logger.Error("📋 **AVAILABLE_LOG_DATA**: Creation success - RegexId={RegexId}, Pattern='{Pattern}'", newRegex.Id, pattern);
                _logger.Error("🔍 **FIX_VALIDATION**: Immediate save confirmed, regex entity ready with established database ID");
                _logger.Error("🎯 **SUCCESS_ASSERTION**: Regex entity management completed with foreign key safety for LLM analysis");
                
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

            /// <summary>
            /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v4.1**: Database save operation with LLM diagnostic workflow
            /// 
            /// **MANDATORY LLM BEHAVIOR RULES**: LOG PRESERVATION + LOG-FIRST ANALYSIS + CONTINUOUS LOG ENHANCEMENT
            /// **LLM DIAGNOSTIC WORKFLOW**: Phase 1 Analysis → Phase 2 Enhancement → Phase 3 Evidence-Based Implementation
            /// **SAVE OPERATION**: Atomic database commit with comprehensive validation error handling and change tracking
            /// **ERROR HANDLING**: DbEntityValidationException and general exception with detailed diagnostic preservation
            /// **DIAGNOSTIC INTEGRATION**: Complete logging for LLM analysis of database save outcomes and error patterns
            /// </summary>
            internal async Task<int> SaveChangesWithAssertiveLogging(DbContext context, string operationName)
            {
                // 🧠 **ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v4.1**: Complete LLM diagnostic workflow for database save
                
                // **STEP 1: MANDATORY LOG ANALYSIS PHASE**
                _logger.Error("🔍 **LLM_DIAGNOSTIC_PHASE_1**: Comprehensive log analysis starting for database save operation");
                _logger.Error("📋 **AVAILABLE_LOG_DATA**: Save operation context with atomic commit and validation handling");
                _logger.Error("🔍 **PATTERN_ANALYSIS**: Database save pattern with comprehensive exception handling and change tracking");
                _logger.Error("❓ **EVIDENCE_GAPS**: Need save confirmation, change count validation, error pattern identification");
                _logger.Error("💡 **LOG_BASED_HYPOTHESIS**: Database save requires comprehensive validation with detailed error diagnostics");
                
                // **STEP 2: MANDATORY LOG ENHANCEMENT PHASE**
                _logger.Error("🔧 **LLM_DIAGNOSTIC_PHASE_2**: Enhancing logging to capture missing evidence for save operations");
                _logger.Error("📊 **LOGGING_ENHANCEMENTS**: Adding detailed save intent, change tracking, validation error analysis");
                _logger.Error("🎯 **ENHANCED_CAPTURE_POINTS**: Save operations, change counts, validation errors, exception details");
                
                // **STEP 3: MANDATORY EVIDENCE-BASED FIX PHASE**
                _logger.Error("🎯 **LLM_DIAGNOSTIC_PHASE_3**: Implementing evidence-based database save operation");
                _logger.Error("📚 **FIX_RATIONALE**: Based on data integrity requirements, implementing comprehensive save validation");
                _logger.Error("🔍 **FIX_VALIDATION**: Will validate success by monitoring change counts and error patterns");
                
                try
                {
                    // **v4.1 SAVE INTENT LOGGING**: Enhanced database save initiation with operation context
                    _logger.Error("💾 **DB_SAVE_INTENT_EVIDENCE**: Attempting atomic database save operation");
                    _logger.Error("📋 **AVAILABLE_LOG_DATA**: Save context - OperationName={OperationName}", operationName);
                    _logger.Error("🔍 **PATTERN_ANALYSIS**: Atomic save pattern with comprehensive change tracking and validation");
                    
                    int changes = await context.SaveChangesAsync().ConfigureAwait(false);
                    
                    // **v4.1 SAVE SUCCESS LOGGING**: LLM diagnostic evidence for successful database commit
                    _logger.Error("✅ **DB_SAVE_SUCCESS_EVIDENCE**: Successfully committed database changes");
                    _logger.Error("📋 **AVAILABLE_LOG_DATA**: Save success - ChangeCount={ChangeCount}, OperationName={OperationName}", changes, operationName);
                    _logger.Error("🔍 **PATTERN_ANALYSIS**: Successful atomic commit with verified change count");
                    _logger.Error("💡 **LOG_BASED_HYPOTHESIS**: Successful save indicates proper entity state management and validation compliance");
                    _logger.Error("📚 **FIX_RATIONALE**: Change count confirmation validates database operation completeness");
                    _logger.Error("🔍 **FIX_VALIDATION**: Save operation completed successfully with documented change metrics");
                    _logger.Error("🎯 **SUCCESS_ASSERTION**: Database save operation completed with comprehensive change tracking for LLM analysis");
                    
                    return changes;
                }
                catch (DbEntityValidationException vex)
                {
                    // **v4.1 VALIDATION ERROR LOGGING**: LLM diagnostic evidence for entity validation failures
                    var errorMessages = vex.EntityValidationErrors.SelectMany(x => x.ValidationErrors).Select(x => $"{x.PropertyName}: {x.ErrorMessage}");
                    var fullErrorMessage = string.Join("; ", errorMessages);
                    
                    _logger.Error(vex, "🚨 **DB_SAVE_VALIDATION_FAILED_EVIDENCE**: Database save operation failed due to entity validation errors");
                    _logger.Error("📋 **AVAILABLE_LOG_DATA**: Validation failure context - OperationName={OperationName}", operationName);
                    _logger.Error("🔍 **PATTERN_ANALYSIS**: Entity validation failure pattern with detailed property-level error analysis");
                    _logger.Error("❓ **EVIDENCE_GAPS**: Validation error details - {ValidationErrors}", fullErrorMessage);
                    _logger.Error("💡 **LOG_BASED_HYPOTHESIS**: Validation failures indicate entity state inconsistency or constraint violations");
                    _logger.Error("📊 **LOGGING_ENHANCEMENTS**: Enhanced validation error tracking with property-level diagnostic details");
                    _logger.Error("🎯 **ENHANCED_CAPTURE_POINTS**: Entity validation errors, property constraints, business rule violations");
                    _logger.Error("📚 **FIX_RATIONALE**: Validation error preservation enables entity state debugging and constraint analysis");
                    _logger.Error("🔍 **FIX_VALIDATION**: Validation exception documented with comprehensive error detail preservation");
                    throw;
                }
                catch (Exception ex)
                {
                    // **v4.1 GENERAL EXCEPTION LOGGING**: LLM diagnostic evidence for unexpected database errors
                    _logger.Error(ex, "🚨 **DB_SAVE_UNHANDLED_EXCEPTION_EVIDENCE**: Database save operation failed due to unexpected error");
                    _logger.Error("📋 **AVAILABLE_LOG_DATA**: Unhandled exception context - OperationName={OperationName}, ExceptionType={ExceptionType}", 
                        operationName, ex.GetType().Name);
                    _logger.Error("🔍 **PATTERN_ANALYSIS**: Unexpected exception pattern requiring comprehensive error state analysis");
                    _logger.Error("❓ **EVIDENCE_GAPS**: Exception details - {ExceptionMessage}", ex.Message);
                    _logger.Error("💡 **LOG_BASED_HYPOTHESIS**: Unexpected exceptions indicate infrastructure failures or configuration issues");
                    _logger.Error("📊 **LOGGING_ENHANCEMENTS**: Enhanced exception tracking with type-specific diagnostic preservation");
                    _logger.Error("🎯 **ENHANCED_CAPTURE_POINTS**: Exception types, error messages, stack traces, operation context");
                    _logger.Error("📚 **FIX_RATIONALE**: Exception preservation enables infrastructure debugging and error pattern analysis");
                    _logger.Error("🔍 **FIX_VALIDATION**: Exception documented with comprehensive diagnostic context for troubleshooting");
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

        /// <summary>
        /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v4.1**: Field format correction strategy with LLM diagnostic workflow
        /// 
        /// **MANDATORY LLM BEHAVIOR RULES**: LOG PRESERVATION + LOG-FIRST ANALYSIS + CONTINUOUS LOG ENHANCEMENT
        /// **LLM DIAGNOSTIC WORKFLOW**: Phase 1 Analysis → Phase 2 Enhancement → Phase 3 Evidence-Based Implementation
        /// **STRATEGY PURPOSE**: Creates formatting rules (pattern/replacement) attached to existing Field definitions
        /// **FORMAT CORRECTION**: Handles character confusion, decimal separators, and explicit format corrections
        /// **DIAGNOSTIC INTEGRATION**: Complete logging for LLM analysis of format correction workflows and validation
        /// </summary>
        public class FieldFormatUpdateStrategy : DatabaseUpdateStrategyBase
        {
            /// <summary>
            /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v4.1**: Field format strategy constructor with LLM diagnostic initialization
            /// 
            /// **MANDATORY LLM BEHAVIOR RULES**: LOG PRESERVATION + LOG-FIRST ANALYSIS + CONTINUOUS LOG ENHANCEMENT
            /// **CONSTRUCTOR LOGIC**: Initializes base strategy with logger injection for comprehensive diagnostic workflow
            /// **STRATEGY TYPE**: FieldFormat strategy for pattern/replacement correction rule creation
            /// **DIAGNOSTIC INTEGRATION**: Complete logging infrastructure for LLM analysis of strategy lifecycle
            /// </summary>
            public FieldFormatUpdateStrategy(ILogger logger) : base(logger) { }

            public override string StrategyType => "FieldFormat";

            /// <summary>
            /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v4.1**: Request handling validation with LLM diagnostic workflow
            /// 
            /// **MANDATORY LLM BEHAVIOR RULES**: LOG PRESERVATION + LOG-FIRST ANALYSIS + CONTINUOUS LOG ENHANCEMENT
            /// **LLM DIAGNOSTIC WORKFLOW**: Phase 1 Analysis → Phase 2 Enhancement → Phase 3 Evidence-Based Implementation
            /// **VALIDATION LOGIC**: Explicit format correction types + potential format correction detection
            /// **FORMAT TYPES**: FieldFormat, FORMAT_FIX, format_correction, decimal_separator, character_confusion
            /// **DIAGNOSTIC INTEGRATION**: Complete logging for LLM analysis of request validation and strategy selection
            /// </summary>
            public override bool CanHandle(RegexUpdateRequest request)
            {
                // 🧠 **ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v4.1**: Complete LLM diagnostic workflow for request validation
                
                // **STEP 1: MANDATORY LOG ANALYSIS PHASE**
                _logger.Error("🔍 **LLM_DIAGNOSTIC_PHASE_1**: Comprehensive log analysis starting for FieldFormat strategy validation");
                _logger.Error("📋 **AVAILABLE_LOG_DATA**: Request validation context with format correction type analysis");
                _logger.Error("🔍 **PATTERN_ANALYSIS**: Explicit format types + potential correction detection pattern");
                _logger.Error("❓ **EVIDENCE_GAPS**: Need explicit type confirmation, potential correction validation, request completeness");
                _logger.Error("💡 **LOG_BASED_HYPOTHESIS**: Format strategy requires comprehensive type analysis and value comparison");
                
                // **STEP 2: MANDATORY LOG ENHANCEMENT PHASE**
                _logger.Error("🔧 **LLM_DIAGNOSTIC_PHASE_2**: Enhancing logging to capture missing evidence for strategy validation");
                _logger.Error("📊 **LOGGING_ENHANCEMENTS**: Adding detailed type analysis, value comparison, correction detection");
                _logger.Error("🎯 **ENHANCED_CAPTURE_POINTS**: Correction types, value states, potential correction indicators");
                
                // **STEP 3: MANDATORY EVIDENCE-BASED FIX PHASE**
                _logger.Error("🎯 **LLM_DIAGNOSTIC_PHASE_3**: Implementing evidence-based FieldFormat strategy validation");
                _logger.Error("📚 **FIX_RATIONALE**: Based on format correction requirements, implementing comprehensive type detection");
                _logger.Error("🔍 **FIX_VALIDATION**: Will validate success by monitoring type matches and correction potential");
                
                // **v4.1 VALIDATION PRECONDITIONS**: Enhanced request state validation
                if (string.IsNullOrEmpty(request.OldValue) && !string.IsNullOrEmpty(request.NewValue))
                {
                    _logger.Error("❌ **VALIDATION_PRECONDITION_FAILED**: OldValue empty with NewValue present - not a format correction scenario");
                    _logger.Error("📋 **AVAILABLE_LOG_DATA**: Precondition failure - OldValue={OldValue}, NewValue={NewValue}", 
                        request.OldValue ?? "NULL", request.NewValue ?? "NULL");
                    _logger.Error("🔍 **PATTERN_ANALYSIS**: Format corrections require both old and new values for pattern/replacement generation");
                    _logger.Error("💡 **LOG_BASED_HYPOTHESIS**: Missing OldValue indicates omission rather than format correction scenario");
                    _logger.Error("📚 **FIX_RATIONALE**: Format strategy requires value comparison for pattern extraction");
                    _logger.Error("🔍 **FIX_VALIDATION**: Precondition failure documented - strategy cannot handle request");
                    return false;
                }

                // **v4.1 EXPLICIT FORMAT TYPE DETECTION**: Enhanced explicit correction type analysis
                bool isExplicitFormatCorrection =
                    request.CorrectionType == "FieldFormat" ||
                    request.CorrectionType == "FORMAT_FIX" ||
                    request.CorrectionType == "format_correction" ||
                    request.CorrectionType == "decimal_separator" ||
                    request.CorrectionType == "DecimalSeparator" ||
                    request.CorrectionType == "character_confusion";

                _logger.Error("🔍 **EXPLICIT_FORMAT_TYPE_ANALYSIS**: Analyzing explicit format correction types");
                _logger.Error("📋 **AVAILABLE_LOG_DATA**: CorrectionType='{CorrectionType}', IsExplicit={IsExplicit}", 
                    request.CorrectionType, isExplicitFormatCorrection);

                if (isExplicitFormatCorrection)
                {
                    _logger.Error("✅ **EXPLICIT_FORMAT_MATCH**: Request matches explicit format correction type");
                    _logger.Error("🔍 **PATTERN_ANALYSIS**: Explicit type match confirms FieldFormat strategy applicability");
                    _logger.Error("💡 **LOG_BASED_HYPOTHESIS**: Explicit format types require immediate strategy acceptance");
                    _logger.Error("📚 **FIX_RATIONALE**: Explicit type match eliminates need for potential correction analysis");
                    _logger.Error("🔍 **FIX_VALIDATION**: Explicit format correction confirmed - strategy can handle request");
                    _logger.Error("🎯 **SUCCESS_ASSERTION**: FieldFormat strategy validation successful for explicit type");
                    return true;
                }

                // **v4.1 POTENTIAL CORRECTION DETECTION**: Enhanced value comparison analysis
                _logger.Error("🔍 **POTENTIAL_CORRECTION_ANALYSIS**: Analyzing potential format correction indicators");
                var isPotential = IsPotentialFormatCorrection(request.OldValue, request.NewValue);
                _logger.Error("📋 **AVAILABLE_LOG_DATA**: Potential correction analysis - IsPotential={IsPotential}", isPotential);
                _logger.Error("🔍 **PATTERN_ANALYSIS**: Value comparison analysis for implicit format correction detection");
                _logger.Error("💡 **LOG_BASED_HYPOTHESIS**: Potential correction detection enables broader format correction coverage");
                _logger.Error("📚 **FIX_RATIONALE**: Potential detection extends strategy applicability beyond explicit types");
                _logger.Error("🔍 **FIX_VALIDATION**: Potential correction analysis completed with definitive result");
                _logger.Error("🎯 **SUCCESS_ASSERTION**: FieldFormat strategy validation completed with comprehensive type analysis");

                return isPotential;
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

        /// <summary>
        /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v4.1**: Omission strategy for missing field detection with LLM diagnostic workflow
        /// 
        /// **MANDATORY LLM BEHAVIOR RULES**: LOG PRESERVATION + LOG-FIRST ANALYSIS + CONTINUOUS LOG ENHANCEMENT
        /// **LLM DIAGNOSTIC WORKFLOW**: Phase 1 Analysis → Phase 2 Enhancement → Phase 3 Evidence-Based Implementation
        /// **STRATEGY PURPOSE**: Creates new line/field/regex combinations for detecting omitted fields in invoice text
        /// **OMISSION HANDLING**: DeepSeek regex generation, validation, and database entity creation workflow
        /// **DIAGNOSTIC INTEGRATION**: Complete logging for LLM analysis of omission detection and correction workflows
        /// </summary>
        public class OmissionUpdateStrategy : DatabaseUpdateStrategyBase
        {
            /// <summary>
            /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v4.1**: Omission strategy constructor with LLM diagnostic initialization
            /// 
            /// **MANDATORY LLM BEHAVIOR RULES**: LOG PRESERVATION + LOG-FIRST ANALYSIS + CONTINUOUS LOG ENHANCEMENT
            /// **CONSTRUCTOR LOGIC**: Initializes base strategy with logger injection for comprehensive diagnostic workflow
            /// **STRATEGY TYPE**: Omission strategy for missing field detection and regex-based correction
            /// **DIAGNOSTIC INTEGRATION**: Complete logging infrastructure for LLM analysis of strategy lifecycle
            /// </summary>
            public OmissionUpdateStrategy(ILogger logger) : base(logger) { }
            public override string StrategyType => "Omission";
            
            /// <summary>
            /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v4.1**: Omission request validation with LLM diagnostic workflow
            /// 
            /// **MANDATORY LLM BEHAVIOR RULES**: LOG PRESERVATION + LOG-FIRST ANALYSIS + CONTINUOUS LOG ENHANCEMENT
            /// **LLM DIAGNOSTIC WORKFLOW**: Phase 1 Analysis → Phase 2 Enhancement → Phase 3 Evidence-Based Implementation
            /// **VALIDATION LOGIC**: Omission type detection + multi-field omission handling + correction type analysis
            /// **OMISSION TYPES**: omission, multi_field_omission with case-insensitive matching
            /// **DIAGNOSTIC INTEGRATION**: Complete logging for LLM analysis of request validation and strategy selection
            /// </summary>
            public override bool CanHandle(RegexUpdateRequest request)
            {
                // 🧠 **ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v4.1**: Complete LLM diagnostic workflow for omission validation
                
                // **STEP 1: MANDATORY LOG ANALYSIS PHASE**
                _logger.Error("🔍 **LLM_DIAGNOSTIC_PHASE_1**: Comprehensive log analysis starting for Omission strategy validation");
                _logger.Error("📋 **AVAILABLE_LOG_DATA**: Request validation context with omission type detection and multi-field analysis");
                _logger.Error("🔍 **PATTERN_ANALYSIS**: Omission type matching + multi-field omission detection pattern");
                _logger.Error("❓ **EVIDENCE_GAPS**: Need omission type confirmation, multi-field validation, request completeness");
                _logger.Error("💡 **LOG_BASED_HYPOTHESIS**: Omission strategy requires comprehensive type analysis and field validation");
                
                // **STEP 2: MANDATORY LOG ENHANCEMENT PHASE**
                _logger.Error("🔧 **LLM_DIAGNOSTIC_PHASE_2**: Enhancing logging to capture missing evidence for omission validation");
                _logger.Error("📊 **LOGGING_ENHANCEMENTS**: Adding detailed type analysis, field validation, omission detection");
                _logger.Error("🎯 **ENHANCED_CAPTURE_POINTS**: Omission types, field states, multi-field indicators");
                
                // **STEP 3: MANDATORY EVIDENCE-BASED FIX PHASE**
                _logger.Error("🎯 **LLM_DIAGNOSTIC_PHASE_3**: Implementing evidence-based Omission strategy validation");
                _logger.Error("📚 **FIX_RATIONALE**: Based on field omission requirements, implementing comprehensive type detection");
                _logger.Error("🔍 **FIX_VALIDATION**: Will validate success by monitoring type matches and omission indicators");
                
                // **v4.1 OMISSION TYPE DETECTION**: Enhanced omission correction type analysis
                _logger.Error("🔍 **OMISSION_TYPE_ANALYSIS**: Analyzing omission correction types and field indicators");
                _logger.Error("📋 **AVAILABLE_LOG_DATA**: CorrectionType='{CorrectionType}'", request.CorrectionType);
                
                bool isOmissionType = request.CorrectionType.StartsWith("omission");
                bool isMultiFieldOmission = request.CorrectionType.Equals("multi_field_omission", StringComparison.OrdinalIgnoreCase);
                
                _logger.Error("📊 **LOGGING_ENHANCEMENTS**: Enhanced omission type detection with detailed analysis");
                _logger.Error("🎯 **ENHANCED_CAPTURE_POINTS**: IsOmissionType={IsOmissionType}, IsMultiFieldOmission={IsMultiFieldOmission}", 
                    isOmissionType, isMultiFieldOmission);
                _logger.Error("🔍 **PATTERN_ANALYSIS**: Omission type pattern matching with case-insensitive multi-field support");
                
                bool canHandle = isOmissionType || isMultiFieldOmission;
                
                if (canHandle)
                {
                    _logger.Error("✅ **OMISSION_TYPE_MATCH**: Request matches omission correction type patterns");
                    _logger.Error("💡 **LOG_BASED_HYPOTHESIS**: Omission type match confirms strategy applicability for field detection");
                    _logger.Error("📚 **FIX_RATIONALE**: Omission detection enables missing field regex creation and validation");
                    _logger.Error("🔍 **FIX_VALIDATION**: Omission type confirmed - strategy can handle request");
                    _logger.Error("🎯 **SUCCESS_ASSERTION**: Omission strategy validation successful for field detection workflow");
                }
                else
                {
                    _logger.Error("❌ **OMISSION_TYPE_MISMATCH**: Request does not match omission correction type patterns");
                    _logger.Error("💡 **LOG_BASED_HYPOTHESIS**: Non-omission types require different strategy handling");
                    _logger.Error("📚 **FIX_RATIONALE**: Type mismatch prevents omission strategy application");
                    _logger.Error("🔍 **FIX_VALIDATION**: Strategy validation failure documented with type analysis");
                }
                
                return canHandle;
            }

            /// <summary>
            /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v4.1**: Omission strategy execution with LLM diagnostic workflow
            /// 
            /// **MANDATORY LLM BEHAVIOR RULES**: LOG PRESERVATION + LOG-FIRST ANALYSIS + CONTINUOUS LOG ENHANCEMENT
            /// **LLM DIAGNOSTIC WORKFLOW**: Phase 1 Analysis → Phase 2 Enhancement → Phase 3 Evidence-Based Implementation
            /// **EXECUTION LOGIC**: Field mapping + DeepSeek regex generation + validation + database entity creation
            /// **OMISSION PROCESSING**: Multi-attempt generation, pattern validation, duplicate detection, entity persistence
            /// **DIAGNOSTIC INTEGRATION**: Complete logging for LLM analysis of omission correction workflow and outcomes
            /// </summary>
            public override async Task<DatabaseUpdateResult> ExecuteAsync(OCRContext context, RegexUpdateRequest request, OCRCorrectionService serviceInstance)
            {
                // 🧠 **ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v4.1**: Complete LLM diagnostic workflow for omission execution
                
                // **STEP 1: MANDATORY LOG ANALYSIS PHASE**
                _logger.Error("🔍 **LLM_DIAGNOSTIC_PHASE_1**: Comprehensive log analysis starting for omission strategy execution");
                _logger.Error("📋 **AVAILABLE_LOG_DATA**: Omission execution context with field mapping and regex generation workflow");
                _logger.Error("🔍 **PATTERN_ANALYSIS**: Field validation → mapping → generation → validation → persistence pattern");
                _logger.Error("❓ **EVIDENCE_GAPS**: Need field validation, mapping success, generation outcomes, validation results");
                _logger.Error("💡 **LOG_BASED_HYPOTHESIS**: Omission execution requires comprehensive field processing with database persistence");
                
                // **STEP 2: MANDATORY LOG ENHANCEMENT PHASE**
                _logger.Error("🔧 **LLM_DIAGNOSTIC_PHASE_2**: Enhancing logging to capture missing evidence for omission execution");
                _logger.Error("📊 **LOGGING_ENHANCEMENTS**: Adding detailed validation, mapping, generation, persistence outcomes");
                _logger.Error("🎯 **ENHANCED_CAPTURE_POINTS**: Field states, mapping results, regex patterns, validation outcomes");
                
                // **STEP 3: MANDATORY EVIDENCE-BASED FIX PHASE**
                _logger.Error("🎯 **LLM_DIAGNOSTIC_PHASE_3**: Implementing evidence-based omission strategy execution");
                _logger.Error("📚 **FIX_RATIONALE**: Based on field omission requirements, implementing comprehensive processing workflow");
                _logger.Error("🔍 **FIX_VALIDATION**: Will validate success by monitoring field processing and database outcomes");
                
                // **v4.1 OMISSION EXECUTION INITIALIZATION**: Enhanced field processing initiation
                _logger.Error("🚀 **OMISSION_EXECUTION_START**: Beginning comprehensive omission strategy execution");
                _logger.Error("📋 **AVAILABLE_LOG_DATA**: Execution context - FieldName='{FieldName}'", request.FieldName);
                _logger.Error("🔍 **PATTERN_ANALYSIS**: Omission execution pattern with field validation and processing workflow");

                // **v4.1 FIELD VALIDATION LOGGING**: Enhanced request precondition validation
                _logger.Error("🔍 **FIELD_VALIDATION_ANALYSIS**: Validating omission request field completeness");
                if (string.IsNullOrEmpty(request.FieldName) || request.NewValue == null)
                {
                    _logger.Error("❌ **FIELD_VALIDATION_FAILED**: Critical omission request fields missing");
                    _logger.Error("📋 **AVAILABLE_LOG_DATA**: Validation failure - FieldName='{FieldName}', NewValuePresent={NewValuePresent}", 
                        request.FieldName ?? "NULL", request.NewValue != null);
                    _logger.Error("🔍 **PATTERN_ANALYSIS**: Omission strategy requires both field name and target value for processing");
                    _logger.Error("💡 **LOG_BASED_HYPOTHESIS**: Missing required fields prevent regex generation and validation");
                    _logger.Error("📚 **FIX_RATIONALE**: Field validation ensures omission strategy has necessary data for processing");
                    _logger.Error("🔍 **FIX_VALIDATION**: Precondition failure documented - cannot proceed with omission execution");
                    return DatabaseUpdateResult.Failed("Request object has null FieldName or NewValue.");
                }
                
                _logger.Error("✅ **FIELD_VALIDATION_SUCCESS**: Omission request field validation successful");
                _logger.Error("📋 **AVAILABLE_LOG_DATA**: Validation success - FieldName='{FieldName}', NewValue='{NewValue}'", 
                    request.FieldName, request.NewValue);
                _logger.Error("🔍 **PATTERN_ANALYSIS**: Field validation successful, proceeding with omission processing workflow");
                _logger.Error("💡 **LOG_BASED_HYPOTHESIS**: Complete field data enables comprehensive omission strategy execution");
                _logger.Error("📚 **FIX_RATIONALE**: Field validation success confirms omission strategy can proceed with processing");
                _logger.Error("🔍 **FIX_VALIDATION**: Field validation completed successfully with comprehensive data confirmation");

                // **v4.1 REQUEST CONTEXT LOGGING**: Enhanced comprehensive request state documentation
                var requestJson = JsonConvert.SerializeObject(request, Formatting.Indented, new JsonSerializerSettings { NullValueHandling = NullValueHandling.Ignore, ReferenceLoopHandling = ReferenceLoopHandling.Ignore });
                _logger.Error("📊 **LOGGING_ENHANCEMENTS**: Enhanced request context documentation for comprehensive analysis");
                _logger.Error("🎯 **ENHANCED_CAPTURE_POINTS**: Complete request state - {RequestJson}", requestJson);
                _logger.Error("🔍 **PATTERN_ANALYSIS**: Full request context enables comprehensive omission processing analysis");
                _logger.Error("💡 **LOG_BASED_HYPOTHESIS**: Complete request documentation enables debugging and workflow validation");
                _logger.Error("📚 **FIX_RATIONALE**: Request context preservation enables comprehensive omission execution analysis");
                _logger.Error("🔍 **FIX_VALIDATION**: Request context documented with complete state information for LLM analysis");

                try
                {
                    // **v4.1 FIELD MAPPING LOGGING**: Enhanced database field mapping with comprehensive validation
                    _logger.Error("🗂️ **FIELD_MAPPING_START**: Beginning field mapping analysis for omission processing");
                    _logger.Error("📋 **AVAILABLE_LOG_DATA**: Mapping context - FieldName='{FieldName}'", request.FieldName);
                    
                    var fieldMappingInfo = serviceInstance.MapDeepSeekFieldToDatabase(request.FieldName);
                    
                    if (fieldMappingInfo == null)
                    {
                        _logger.Error("❌ **FIELD_MAPPING_FAILED**: Database field mapping not found for omission field");
                        _logger.Error("🔍 **PATTERN_ANALYSIS**: Unknown field mapping prevents omission strategy execution");
                        _logger.Error("💡 **LOG_BASED_HYPOTHESIS**: Field mapping required for database entity creation and validation");
                        _logger.Error("📚 **FIX_RATIONALE**: Field mapping failure prevents omission processing workflow");
                        _logger.Error("🔍 **FIX_VALIDATION**: Field mapping failure documented - cannot proceed with omission execution");
                        return DatabaseUpdateResult.Failed($"Unknown field mapping for '{request.FieldName}'.");
                    }
                    
                    _logger.Error("✅ **FIELD_MAPPING_SUCCESS**: Database field mapping successful for omission processing");
                    _logger.Error("📋 **AVAILABLE_LOG_DATA**: Mapping success - DatabaseField='{DatabaseField}', EntityType='{EntityType}'", 
                        fieldMappingInfo.DatabaseFieldName, fieldMappingInfo.EntityType);
                    _logger.Error("🔍 **PATTERN_ANALYSIS**: Successful field mapping enables omission database entity creation");
                    _logger.Error("💡 **LOG_BASED_HYPOTHESIS**: Field mapping success confirms omission strategy can proceed with processing");
                    _logger.Error("📚 **FIX_RATIONALE**: Field mapping enables proper database entity relationships and validation");
                    _logger.Error("🔍 **FIX_VALIDATION**: Field mapping completed successfully with database field confirmation");

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

                    // **v4.1 REGEX GENERATION INITIALIZATION**: Enhanced DeepSeek regex generation workflow
                    _logger.Error("🤖 **REGEX_GENERATION_START**: Initiating DeepSeek regex generation for omission field");
                    _logger.Error("📊 **LOGGING_ENHANCEMENTS**: Enhanced generation workflow with multi-attempt validation");
                    
                    RegexCreationResponse regexResponse = null;
                    string failureReason = "Initial generation failed.";
                    int maxAttempts = 2;
                    
                    _logger.Error("🎯 **ENHANCED_CAPTURE_POINTS**: MaxAttempts={MaxAttempts}, InitialFailureReason='{FailureReason}'", 
                        maxAttempts, failureReason);
                    _logger.Error("🔍 **PATTERN_ANALYSIS**: Multi-attempt generation pattern with validation and correction workflow");
                    _logger.Error("💡 **LOG_BASED_HYPOTHESIS**: Multi-attempt generation ensures reliable regex pattern creation");
                    _logger.Error("📚 **FIX_RATIONALE**: Generation attempts with validation enable robust omission pattern creation");
                    _logger.Error("🔍 **FIX_VALIDATION**: Regex generation initialization completed with multi-attempt framework");
                    
                    for (int attempt = 1; attempt <= maxAttempts; attempt++)
                    {
                        // **v4.1 GENERATION ATTEMPT LOGGING**: Enhanced attempt-specific processing documentation
                        _logger.Error("🔄 **GENERATION_ATTEMPT_START**: Beginning regex generation attempt for omission field");
                        _logger.Error("📋 **AVAILABLE_LOG_DATA**: Attempt {Attempt}/{MaxAttempts} for field '{FieldName}'", attempt, maxAttempts, request.FieldName);
                        _logger.Error("🔍 **PATTERN_ANALYSIS**: Incremental generation attempt with validation and correction workflow");

                        if (attempt == 1)
                        {
                            // **v4.1 INITIAL GENERATION LOGGING**: Enhanced first attempt regex generation
                            _logger.Error("🎯 **INITIAL_GENERATION**: Requesting new regex pattern from DeepSeek for omission field");
                            _logger.Error("💡 **LOG_BASED_HYPOTHESIS**: Initial generation provides foundation pattern for field detection");
                            regexResponse = await serviceInstance.RequestNewRegexFromDeepSeek(correctionForPrompt, lineContextForPrompt).ConfigureAwait(false);
                            _logger.Error("📊 **LOGGING_ENHANCEMENTS**: Initial generation request completed");
                        }
                        else
                        {
                            // **v4.1 CORRECTION GENERATION LOGGING**: Enhanced retry attempt with failure analysis
                            _logger.Error("🔧 **CORRECTION_GENERATION**: Requesting pattern correction from DeepSeek for failed validation");
                            _logger.Error("📋 **AVAILABLE_LOG_DATA**: Correction request - FailureReason='{FailureReason}'", failureReason);
                            _logger.Error("🔍 **PATTERN_ANALYSIS**: Correction generation pattern with failure feedback and improvement");
                            _logger.Error("💡 **LOG_BASED_HYPOTHESIS**: Correction generation improves pattern accuracy based on validation failures");
                            regexResponse = await serviceInstance.RequestRegexCorrectionFromDeepSeek(correctionForPrompt, lineContextForPrompt, regexResponse, failureReason).ConfigureAwait(false);
                            _logger.Error("📚 **FIX_RATIONALE**: Correction generation enables iterative pattern improvement and validation");
                        }

                        // **v4.1 RESPONSE VALIDATION LOGGING**: Enhanced regex response validation and error handling
                        _logger.Error("🔍 **RESPONSE_VALIDATION_ANALYSIS**: Validating DeepSeek regex generation response");
                        if (regexResponse == null || string.IsNullOrWhiteSpace(regexResponse.RegexPattern))
                        {
                            failureReason = "DeepSeek did not return a regex pattern.";
                            _logger.Error("❌ **RESPONSE_VALIDATION_FAILED**: DeepSeek generation response invalid or empty");
                            _logger.Error("📋 **AVAILABLE_LOG_DATA**: Validation failure - Attempt {Attempt}, Reason='{FailureReason}'", attempt, failureReason);
                            _logger.Error("🔍 **PATTERN_ANALYSIS**: Invalid response pattern requires additional generation attempts");
                            _logger.Error("💡 **LOG_BASED_HYPOTHESIS**: Empty responses indicate generation issues requiring retry or correction");
                            _logger.Error("📚 **FIX_RATIONALE**: Response validation ensures viable patterns for omission field detection");
                            _logger.Error("🔍 **FIX_VALIDATION**: Response validation failure documented with retry strategy");
                            continue;
                        }
                        
                        _logger.Error("✅ **RESPONSE_VALIDATION_SUCCESS**: DeepSeek regex response validation successful");
                        _logger.Error("📋 **AVAILABLE_LOG_DATA**: Response success - Pattern='{Pattern}'", regexResponse.RegexPattern);
                        _logger.Error("🔍 **PATTERN_ANALYSIS**: Valid response enables pattern validation and database processing");

                        // **v4.1 PATTERN VALIDATION LOGGING**: Enhanced regex pattern validation with comprehensive analysis
                        _logger.Error("🧪 **PATTERN_VALIDATION_START**: Beginning regex pattern validation against target value");
                        _logger.Error("📋 **AVAILABLE_LOG_DATA**: Validation context - Pattern='{Pattern}', TargetValue='{TargetValue}'", 
                            regexResponse.RegexPattern, correctionForPrompt.NewValue);
                        
                        if (serviceInstance.ValidateRegexPattern(regexResponse, correctionForPrompt))
                        {
                            _logger.Error("✅ **PATTERN_VALIDATION_SUCCESS**: Regex pattern validation successful for omission field");
                            _logger.Error("📋 **AVAILABLE_LOG_DATA**: Validation success - Field='{FieldName}', Attempt={Attempt}", request.FieldName, attempt);
                            _logger.Error("🔍 **PATTERN_ANALYSIS**: Successful validation enables database entity creation and persistence");
                            _logger.Error("💡 **LOG_BASED_HYPOTHESIS**: Pattern validation success confirms regex accuracy for field detection");
                            _logger.Error("📚 **FIX_RATIONALE**: Validation success enables omission strategy database processing workflow");
                            _logger.Error("🔍 **FIX_VALIDATION**: Pattern validation completed successfully with accuracy confirmation");

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

                        // **v4.1 VALIDATION FAILURE LOGGING**: Enhanced pattern validation failure analysis
                        failureReason = $"The pattern '{regexResponse.RegexPattern}' failed to extract the expected value '{correctionForPrompt.NewValue}' from the provided text '{correctionForPrompt.LineText}'.";
                        _logger.Error("❌ **PATTERN_VALIDATION_FAILED**: Regex pattern validation failed for omission field");
                        _logger.Error("📋 **AVAILABLE_LOG_DATA**: Validation failure - Attempt {Attempt}, Reason='{FailureReason}'", attempt, failureReason);
                        _logger.Error("🔍 **PATTERN_ANALYSIS**: Validation failure pattern requires pattern correction or regeneration");
                        _logger.Error("💡 **LOG_BASED_HYPOTHESIS**: Validation failures indicate pattern accuracy issues requiring correction");
                        _logger.Error("📚 **FIX_RATIONALE**: Validation failure enables pattern improvement through DeepSeek correction");
                        _logger.Error("🔍 **FIX_VALIDATION**: Validation failure documented with detailed pattern analysis for correction");
                    }

                    // **v4.1 GENERATION FAILURE LOGGING**: Enhanced comprehensive generation failure analysis
                    _logger.Error("❌ **GENERATION_WORKFLOW_FAILED**: Complete DeepSeek generation workflow failed for omission field");
                    _logger.Error("📋 **AVAILABLE_LOG_DATA**: Workflow failure - Field='{FieldName}', MaxAttempts={MaxAttempts}, LastFailure='{FailureReason}'", 
                        request.FieldName, maxAttempts, failureReason);
                    _logger.Error("🔍 **PATTERN_ANALYSIS**: Generation workflow failure prevents omission strategy execution");
                    _logger.Error("💡 **LOG_BASED_HYPOTHESIS**: Generation failures indicate field complexity or prompt issues");
                    _logger.Error("📚 **FIX_RATIONALE**: Generation failure documentation enables workflow debugging and improvement");
                    _logger.Error("🔍 **FIX_VALIDATION**: Complete generation failure documented with comprehensive attempt analysis");
                    
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