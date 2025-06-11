// File: OCRCorrectionService/OCRDatabaseUpdates.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using OCR.Business.Entities;
using System.Text.RegularExpressions; // Needed for Regex.IsMatch in ValidateUpdateRequest
using System.Data.Entity; // For Include() and FirstOrDefaultAsync()
using global::EntryDataDS.Business.Entities; // For ShipmentInvoice
using Core.Common.Extensions; // For LogLevelOverride
using Serilog.Events; // For LogEventLevel

namespace WaterNut.DataSpace
{
    public partial class OCRCorrectionService
    {
        // _strategyFactory is initialized in OCRCorrectionService.cs constructor

        #region Main Database Update Methods

        public async Task UpdateRegexPatternsAsync(
            IEnumerable<CorrectionResult> successfulCorrections,
            string fileText,
            string filePath = null,
            Dictionary<string, OCRFieldMetadata> invoiceMetadata = null)
        {
            if (successfulCorrections == null || !successfulCorrections.Any())
            {
                _logger?.Information("UpdateRegexPatternsAsync: No successful corrections provided for database pattern updates.");
                return;
            }

            _logger?.Information("Starting database pattern updates for {CorrectionCount} successful corrections.", successfulCorrections.Count());
            _strategyFactory ??= new DatabaseUpdateStrategyFactory(_logger);

            int dbSuccessCount = 0;
            int dbFailureCount = 0;
            int omissionPatternUpdates = 0;
            int formatPatternUpdates = 0;

            using var context = new OCRContext();

            foreach (var correction in successfulCorrections)
            {
                DatabaseUpdateResult dbUpdateResult = null;
                IDatabaseUpdateStrategy strategy = null;
                RegexUpdateRequest request = null;

                try
                {
                    // ENHANCED LOGGING: Track complete correction input
                    _logger?.Error("🔍 **DB_UPDATE_CORRECTION_INPUT**: FieldName={FieldName} | OldValue={OldValue} | NewValue={NewValue} | CorrectionType={CorrectionType} | Success={Success} | Confidence={Confidence}", 
                        correction.FieldName, correction.OldValue, correction.NewValue, correction.CorrectionType, correction.Success, correction.Confidence);
                    
                    // 1. Check if database update should be skipped based on metadata availability
                    var fieldMetadata = invoiceMetadata?.ContainsKey(correction.FieldName) == true ? invoiceMetadata[correction.FieldName] : null;
                    
                    // ENHANCED LOGGING: Show metadata availability
                    if (fieldMetadata != null)
                    {
                        _logger?.Error("🔍 **DB_UPDATE_METADATA_FOUND**: FieldName={FieldName} | FieldId={FieldId} | LineId={LineId} | RegexId={RegexId} | PartId={PartId} | Field={Field} | EntityType={EntityType}", 
                            correction.FieldName, fieldMetadata.FieldId, fieldMetadata.LineId, fieldMetadata.RegexId, fieldMetadata.PartId, fieldMetadata.Field, fieldMetadata.EntityType);
                    }
                    else
                    {
                        _logger?.Error("❌ **DB_UPDATE_NO_METADATA**: FieldName={FieldName} | InvoiceMetadataCount={MetadataCount} | InvoiceMetadataKeys={MetadataKeys}", 
                            correction.FieldName, invoiceMetadata?.Count ?? 0, invoiceMetadata?.Keys.ToList() ?? new List<string>());
                    }
                    
                    var updateContext = this.GetDatabaseUpdateContext(correction.FieldName, fieldMetadata);
                    
                    // ENHANCED LOGGING: Show update context decision
                    _logger?.Error("🔍 **DB_UPDATE_CONTEXT**: FieldName={FieldName} | UpdateStrategy={UpdateStrategy} | IsValid={IsValid} | ErrorMessage={ErrorMessage}", 
                        correction.FieldName, updateContext.UpdateStrategy, updateContext.IsValid, updateContext.ErrorMessage ?? "NULL");

                    if (updateContext.UpdateStrategy == DatabaseUpdateStrategy.SkipUpdate)
                    {
                        _logger?.Error("⚠️ **DB_UPDATE_SKIPPED**: Field {FieldName} skipped - Strategy={Strategy} | Reason={Reason}",
                            correction.FieldName, updateContext.UpdateStrategy, updateContext.ErrorMessage ?? "No metadata available");
                        continue; // Skip this correction entirely
                    }

                    // 2. Create the RegexUpdateRequest from the CorrectionResult and other context
                    //    CreateUpdateRequestForStrategy is an instance method in OCRCorrectionService.cs (main part)
                    var lineContext = this.BuildLineContextForCorrection(correction, invoiceMetadata, fileText);
                    
                    // ENHANCED LOGGING: Show line context creation
                    if (lineContext != null)
                    {
                        _logger?.Error("🔍 **DB_UPDATE_LINE_CONTEXT**: FieldName={FieldName} | LineId={LineId} | RegexId={RegexId} | PartId={PartId} | LineNumber={LineNumber} | IsOrphaned={IsOrphaned}", 
                            correction.FieldName, lineContext.LineId, lineContext.RegexId, lineContext.PartId, lineContext.LineNumber, lineContext.IsOrphaned);
                    }
                    else
                    {
                        _logger?.Error("❌ **DB_UPDATE_NO_LINE_CONTEXT**: FieldName={FieldName} | Could not build line context", correction.FieldName);
                    }
                    
                    request = this.CreateUpdateRequestForStrategy(correction, lineContext, filePath, fileText);
                    
                    // ENHANCED LOGGING: Show created request
                    if (request != null)
                    {
                        _logger?.Error("🔍 **DB_UPDATE_REQUEST_CREATED**: FieldName={FieldName} | LineId={LineId} | RegexId={RegexId} | OldValue={OldValue} | NewValue={NewValue} | CorrectionType={CorrectionType}", 
                            request.FieldName, request.LineId, request.RegexId, request.OldValue ?? "NULL", request.NewValue ?? "NULL", request.CorrectionType ?? "NULL");
                    }
                    else
                    {
                        _logger?.Error("❌ **DB_UPDATE_REQUEST_FAILED**: FieldName={FieldName} | CreateUpdateRequestForStrategy returned null", correction.FieldName);
                        continue;
                    }

                    // 3. Validate the request for basic soundness before selecting a strategy
                    //    ValidateUpdateRequest is an instance method defined below in this file.
                    var validationResult = this.ValidateUpdateRequest(request);
                    
                    // ENHANCED LOGGING: Show validation result
                    _logger?.Error("🔍 **DB_UPDATE_VALIDATION**: FieldName={FieldName} | IsValid={IsValid} | ErrorMessage={ErrorMessage}", 
                        correction.FieldName, validationResult.IsValid, validationResult.ErrorMessage ?? "NULL");
                    
                    if (!validationResult.IsValid)
                    {
                        _logger?.Error("❌ **DB_UPDATE_VALIDATION_FAILED**: FieldName={FieldName} | ValidationError={ErrorMessage} | Skipping DB update", 
                            correction.FieldName, validationResult.ErrorMessage);
                        dbUpdateResult = DatabaseUpdateResult.Failed($"Validation failed: {validationResult.ErrorMessage}");
                        dbFailureCount++;
                        // LogCorrectionLearningAsync is an instance method defined below in this file.
                        await this.LogCorrectionLearningAsync(context, request, dbUpdateResult).ConfigureAwait(false);
                        continue;
                    }

                    // 4. Get the appropriate strategy
                    strategy = _strategyFactory.GetStrategy(correction);
                    
                    // ENHANCED LOGGING: Show strategy selection
                    if (strategy != null)
                    {
                        _logger?.Error("🔍 **DB_UPDATE_STRATEGY_SELECTED**: FieldName={FieldName} | CorrectionType={CorrectionType} | StrategyType={StrategyType}", 
                            correction.FieldName, correction.CorrectionType, strategy.StrategyType);
                    }
                    else
                    {
                        _logger?.Error("❌ **DB_UPDATE_NO_STRATEGY**: FieldName={FieldName} | CorrectionType={CorrectionType} | No strategy found for this type", 
                            correction.FieldName, correction.CorrectionType);
                        dbUpdateResult = DatabaseUpdateResult.Failed($"No strategy for type '{correction.CorrectionType}'");
                        dbFailureCount++;
                        await this.LogCorrectionLearningAsync(context, request, dbUpdateResult).ConfigureAwait(false);
                        continue;
                    }

                    // 4. Execute the strategy
                    _logger?.Error("🔍 **DB_UPDATE_STRATEGY_EXECUTING**: FieldName={FieldName} | StrategyType={StrategyType} | About to execute strategy", 
                        correction.FieldName, strategy.StrategyType);
                    dbUpdateResult = await strategy.ExecuteAsync(context, request, this).ConfigureAwait(false);
                    
                    // ENHANCED LOGGING: Show strategy execution result
                    _logger?.Error("🔍 **DB_UPDATE_STRATEGY_RESULT**: FieldName={FieldName} | StrategyType={StrategyType} | IsSuccess={IsSuccess} | Message={Message}", 
                        correction.FieldName, strategy.StrategyType, dbUpdateResult.IsSuccess, dbUpdateResult.Message ?? "NULL");

                    // 5. Process the result of the strategy execution
                    if (dbUpdateResult.IsSuccess)
                    {
                        dbSuccessCount++;
                        if (strategy is OmissionUpdateStrategy) omissionPatternUpdates++;
                        else if (strategy is FieldFormatUpdateStrategy) formatPatternUpdates++;

                        _logger?.Information("Successfully updated database for field {FieldName} using {StrategyType}: {OperationDetails}",
                            correction.FieldName, strategy.StrategyType, dbUpdateResult.Message);
                    }
                    else
                    {
                        dbFailureCount++;
                        _logger?.Warning("Database update failed for field {FieldName} using {StrategyType}: {ErrorMessage}",
                            correction.FieldName, strategy.StrategyType, dbUpdateResult.Message);
                    }

                    // 6. Log to learning table
                    await this.LogCorrectionLearningAsync(context, request, dbUpdateResult).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    dbFailureCount++;
                    _logger?.Error(ex, "Exception during database update processing for field {FieldName}. Strategy: {StrategyType}",
                        correction.FieldName, strategy?.StrategyType ?? "Unknown");

                    if (request != null)
                    {
                        var exceptionResult = DatabaseUpdateResult.Failed($"Outer exception during DB update: {ex.Message}", ex);
                        await this.LogCorrectionLearningAsync(context, request, exceptionResult).ConfigureAwait(false);
                    }
                }
            }

            _logger?.Information("Database pattern updates completed: {SuccessCount} successful ({OmissionUpdates} omissions, {FormatUpdates} format/value), {FailureCount} failed.",
                dbSuccessCount, omissionPatternUpdates, formatPatternUpdates, dbFailureCount);
        }

        #endregion

        #region Database Update Context Methods

        /// <summary>
        /// Gets the database update context for a field correction, determining the appropriate strategy and required IDs.
        /// </summary>
        public DatabaseUpdateContext GetDatabaseUpdateContext(string fieldName, OCRFieldMetadata metadata)
        {
            var context = new DatabaseUpdateContext();

            try
            {
                // ENHANCED LOGGING: Track context creation input
                _logger?.Error("🔍 **GET_DB_CONTEXT_INPUT**: FieldName={FieldName} | HasMetadata={HasMetadata}", 
                    fieldName, metadata != null);
                
                if (metadata != null)
                {
                    _logger?.Error("🔍 **GET_DB_CONTEXT_METADATA**: FieldName={FieldName} | FieldId={FieldId} | LineId={LineId} | RegexId={RegexId} | PartId={PartId}", 
                        fieldName, metadata.FieldId, metadata.LineId, metadata.RegexId, metadata.PartId);
                }
                
                // 1. Validate field support
                bool fieldSupported = this.IsFieldSupported(fieldName);
                _logger?.Error("🔍 **GET_DB_CONTEXT_FIELD_SUPPORT**: FieldName={FieldName} | IsSupported={IsSupported}", 
                    fieldName, fieldSupported);
                
                if (!fieldSupported)
                {
                    context.IsValid = false;
                    context.ErrorMessage = $"Field '{fieldName}' is not supported for database updates.";
                    context.UpdateStrategy = DatabaseUpdateStrategy.SkipUpdate;
                    _logger?.Error("❌ **GET_DB_CONTEXT_UNSUPPORTED**: FieldName={FieldName} | Not supported for database updates", fieldName);
                    return context;
                }

                // 2. Get field validation info
                var validationInfo = this.GetFieldValidationInfo(fieldName);
                context.ValidationRules = validationInfo;

                if (!validationInfo.IsValid)
                {
                    context.IsValid = false;
                    context.ErrorMessage = validationInfo.ErrorMessage;
                    context.UpdateStrategy = DatabaseUpdateStrategy.SkipUpdate;
                    return context;
                }

                // 3. Get enhanced field info
                var fieldInfo = this.MapDeepSeekFieldToEnhancedInfo(fieldName, metadata);
                context.FieldInfo = fieldInfo;

                // 4. Determine strategy based on available metadata
                if (metadata == null)
                {
                    // For omitted fields, lack of metadata is expected - they need new patterns created
                    // Allow the strategy to proceed with CreateNewPattern approach
                    context.UpdateStrategy = DatabaseUpdateStrategy.CreateNewPattern;
                    context.IsValid = true;
                    _logger?.Error("🔍 **GET_DB_CONTEXT_NO_METADATA_OMISSION**: FieldName={FieldName} | No metadata available (expected for omissions), using CreateNewPattern strategy", fieldName);
                    
                    // CRITICAL FIX: Determine PartId immediately using existing template structure
                    // PartId should NEVER be null for existing templates - header/detail parts are mandatory and known
                    int? determinedPartId = this.DeterminePartIdForOmissionField(fieldName, fieldInfo);
                    
                    _logger?.Error("🔍 **GET_DB_CONTEXT_PARTID_DETERMINATION**: FieldName={FieldName} | DeterminedPartId={PartId} | FieldEntityType={EntityType}", 
                        fieldName, determinedPartId?.ToString() ?? "NULL", fieldInfo?.EntityType ?? "NULL");
                    
                    if (!determinedPartId.HasValue)
                    {
                        context.IsValid = false;
                        context.ErrorMessage = $"Cannot determine PartId for omitted field '{fieldName}' - template structure incomplete";
                        context.UpdateStrategy = DatabaseUpdateStrategy.SkipUpdate;
                        _logger?.Error("❌ **GET_DB_CONTEXT_NO_PARTID**: Cannot determine PartId for {FieldName} - template may be incomplete", fieldName);
                        return context;
                    }
                    
                    // Create RequiredIds structure with determined PartId for omission processing
                    context.RequiredIds = new RequiredDatabaseIds
                    {
                        FieldId = null,     // Will be created
                        LineId = null,      // Will be created 
                        RegexId = null,     // Will be created
                        PartId = determinedPartId.Value // DETERMINED from existing template structure
                    };
                    
                    _logger?.Error("✅ **GET_DB_CONTEXT_PARTID_SUCCESS**: FieldName={FieldName} | Assigned PartId={PartId} based on field type", fieldName, determinedPartId.Value);
                    return context;
                }

                // 5. Set required IDs from metadata
                context.RequiredIds = new RequiredDatabaseIds
                {
                    FieldId = metadata.FieldId,
                    LineId = metadata.LineId,
                    RegexId = metadata.RegexId,
                    PartId = metadata.PartId
                };
                
                // ENHANCED LOGGING: Show ID availability for strategy decision
                _logger?.Error("🔍 **GET_DB_CONTEXT_IDS**: FieldName={FieldName} | FieldId={FieldId} | LineId={LineId} | RegexId={RegexId} | PartId={PartId}", 
                    fieldName, metadata.FieldId?.ToString() ?? "NULL", metadata.LineId?.ToString() ?? "NULL", 
                    metadata.RegexId?.ToString() ?? "NULL", metadata.PartId?.ToString() ?? "NULL");

                // 6. Determine update strategy based on available IDs
                // Priority: Complete regex context > Line context for new patterns > Field format updates > Log only
                if (metadata.RegexId.HasValue && metadata.LineId.HasValue && metadata.FieldId.HasValue)
                {
                    context.UpdateStrategy = DatabaseUpdateStrategy.UpdateRegexPattern;
                    _logger?.Error("🔍 **GET_DB_CONTEXT_STRATEGY**: FieldName={FieldName} | Strategy=UpdateRegexPattern | Has all required IDs", fieldName);
                }
                else if (metadata.LineId.HasValue || metadata.PartId.HasValue)
                {
                    // If we have line context but missing regex ID, create new pattern
                    context.UpdateStrategy = DatabaseUpdateStrategy.CreateNewPattern;
                    _logger?.Error("🔍 **GET_DB_CONTEXT_STRATEGY**: FieldName={FieldName} | Strategy=CreateNewPattern | Has line/part context", fieldName);
                }
                else if (metadata.FieldId.HasValue)
                {
                    // Only use field format updates when we don't have line context
                    context.UpdateStrategy = DatabaseUpdateStrategy.UpdateFieldFormat;
                    _logger?.Error("🔍 **GET_DB_CONTEXT_STRATEGY**: FieldName={FieldName} | Strategy=UpdateFieldFormat | Has field ID only", fieldName);
                }
                else
                {
                    context.UpdateStrategy = DatabaseUpdateStrategy.LogOnly;
                }

                context.IsValid = true;
                return context;
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "Error creating database update context for field {FieldName}", fieldName);
                context.IsValid = false;
                context.ErrorMessage = $"Error creating update context: {ex.Message}";
                context.UpdateStrategy = DatabaseUpdateStrategy.SkipUpdate;
                return context;
            }
        }

        #endregion

        #region Pipeline Instance Methods (Testable)

        /// <summary>
        /// Generates regex pattern for a correction using DeepSeek integration.
        /// Testable instance method called by extension method.
        /// </summary>
        internal async Task<CorrectionResult> GenerateRegexPatternInternal(CorrectionResult correction, LineContext lineContext)
        {
            if (correction == null)
            {
                _logger.Error("GenerateRegexPatternInternal: Correction is null");
                return null;
            }

            _logger.Information("🔍 **PIPELINE_REGEX_GENERATION_START**: Generating regex pattern for field {FieldName}", correction.FieldName);

            try
            {
                // For omissions, we need to create new regex patterns
                if (correction.CorrectionType == "omission" && lineContext != null)
                {
                    _logger.Information("🔍 **PIPELINE_OMISSION_PATTERN**: Creating new regex for omitted field {FieldName}", correction.FieldName);
                    
                    var regexResponse = await this.RequestNewRegexFromDeepSeek(correction, lineContext).ConfigureAwait(false);
                    if (regexResponse != null && !string.IsNullOrEmpty(regexResponse.RegexPattern))
                    {
                        correction.SuggestedRegex = regexResponse.RegexPattern;
                        correction.RequiresMultilineRegex = regexResponse.IsMultiline;
                        correction.Confidence = regexResponse.Confidence;
                        correction.Reasoning = regexResponse.Reasoning;
                        
                        _logger.Information("✅ **PIPELINE_REGEX_SUCCESS**: Generated regex pattern for {FieldName}: {Pattern}", 
                            correction.FieldName, regexResponse.RegexPattern);
                    }
                    else
                    {
                        _logger.Warning("❌ **PIPELINE_REGEX_FAILED**: Failed to generate regex pattern for {FieldName}", correction.FieldName);
                        correction.Success = false;
                        correction.Reasoning = "Failed to generate regex pattern from DeepSeek";
                    }
                }
                else
                {
                    _logger.Information("🔍 **PIPELINE_FORMAT_CORRECTION**: Field {FieldName} is format correction, no new regex needed", correction.FieldName);
                }

                return correction;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "🚨 **PIPELINE_REGEX_EXCEPTION**: Exception generating regex pattern for {FieldName}", correction.FieldName);
                correction.Success = false;
                correction.Reasoning = $"Exception during regex generation: {ex.Message}";
                return correction;
            }
        }

        /// <summary>
        /// Validates generated regex pattern against known constraints.
        /// Testable instance method called by extension method.
        /// </summary>
        internal CorrectionResult ValidatePatternInternal(CorrectionResult correction)
        {
            if (correction == null)
            {
                _logger.Error("ValidatePatternInternal: Correction is null");
                return null;
            }

            _logger.Information("🔍 **PIPELINE_PATTERN_VALIDATION_START**: Validating pattern for field {FieldName}", correction.FieldName);

            try
            {
                // Validate field is supported
                if (!this.IsFieldSupported(correction.FieldName))
                {
                    _logger.Warning("❌ **PIPELINE_VALIDATION_FAILED**: Field {FieldName} is not supported", correction.FieldName);
                    correction.Success = false;
                    correction.Reasoning = $"Field '{correction.FieldName}' is not supported for database updates";
                    return correction;
                }

                // Validate new value if present
                if (!string.IsNullOrEmpty(correction.NewValue))
                {
                    var fieldInfo = this.GetFieldValidationInfo(correction.FieldName);
                    if (!string.IsNullOrEmpty(fieldInfo.ValidationPattern))
                    {
                        try
                        {
                            if (!Regex.IsMatch(correction.NewValue, fieldInfo.ValidationPattern))
                            {
                                _logger.Warning("❌ **PIPELINE_VALUE_VALIDATION_FAILED**: Value '{Value}' for {FieldName} doesn't match pattern {Pattern}", 
                                    correction.NewValue, correction.FieldName, fieldInfo.ValidationPattern);
                                correction.Success = false;
                                correction.Reasoning = $"Value '{correction.NewValue}' doesn't match expected pattern";
                                return correction;
                            }
                        }
                        catch (ArgumentException ex)
                        {
                            _logger.Warning("⚠️ **PIPELINE_REGEX_VALIDATION_ERROR**: Invalid validation pattern for {FieldName}: {Error}", 
                                correction.FieldName, ex.Message);
                        }
                    }
                }

                // Validate suggested regex if present
                if (!string.IsNullOrEmpty(correction.SuggestedRegex))
                {
                    try
                    {
                        var testRegex = new Regex(correction.SuggestedRegex, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                        _logger.Information("✅ **PIPELINE_REGEX_VALID**: Suggested regex for {FieldName} is syntactically valid", correction.FieldName);
                    }
                    catch (ArgumentException ex)
                    {
                        _logger.Warning("❌ **PIPELINE_REGEX_INVALID**: Invalid suggested regex for {FieldName}: {Error}", 
                            correction.FieldName, ex.Message);
                        correction.Success = false;
                        correction.Reasoning = $"Invalid regex pattern: {ex.Message}";
                        return correction;
                    }
                }

                _logger.Information("✅ **PIPELINE_VALIDATION_SUCCESS**: Pattern validation passed for field {FieldName}", correction.FieldName);
                return correction;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "🚨 **PIPELINE_VALIDATION_EXCEPTION**: Exception validating pattern for {FieldName}", correction.FieldName);
                correction.Success = false;
                correction.Reasoning = $"Exception during validation: {ex.Message}";
                return correction;
            }
        }

        /// <summary>
        /// Applies correction to database using appropriate strategy.
        /// Testable instance method called by extension method.
        /// </summary>
        internal async Task<DatabaseUpdateResult> ApplyToDatabaseInternal(CorrectionResult correction, TemplateContext templateContext)
        {
            // EXPANDED DATABASE UPDATE LOGGING: Cover entire failing database update section
            using (LogLevelOverride.Begin(LogEventLevel.Verbose))
            {
                _logger.Information("🔍 **DATABASE_SECTION_ENTRY**: ApplyToDatabaseInternal called for field {FieldName}", correction?.FieldName ?? "NULL");
                
                // DATA FLOW ASSERTION: Validate input parameters with detailed data logging
                _logger.Information("🔍 **DATA_EXPECTATION**: Expecting non-null correction and templateContext parameters");
                _logger.Information("🔍 **DATA_ACTUAL_CORRECTION**: Correction = {IsNull} | FieldName = {FieldName} | CorrectionType = {CorrectionType} | Success = {Success}", 
                    correction == null ? "NULL" : "NOT_NULL", 
                    correction?.FieldName ?? "NULL", 
                    correction?.CorrectionType ?? "NULL", 
                    correction?.Success.ToString() ?? "NULL");
                _logger.Information("🔍 **DATA_ACTUAL_TEMPLATE_CONTEXT**: TemplateContext = {IsNull} | InvoiceId = {InvoiceId} | MetadataCount = {MetadataCount}", 
                    templateContext == null ? "NULL" : "NOT_NULL", 
                    templateContext?.InvoiceId?.ToString() ?? "NULL", 
                    templateContext?.Metadata?.Count.ToString() ?? "NULL");

                if (correction == null || templateContext == null)
                {
                    _logger.Error("❌ **ASSERTION_FAILED**: Input validation failed - correction or templateContext is null");
                    _logger.Error("🔍 **LOGIC_FLOW**: Returning Failed result due to null input parameters");
                    return DatabaseUpdateResult.Failed("Null input parameters");
                }
                
                _logger.Information("✅ **ASSERTION_PASSED**: Input validation passed - both correction and templateContext are non-null");

                _logger.Information("🔍 **PIPELINE_DATABASE_UPDATE_START**: Applying correction to database for field {FieldName}", correction.FieldName);

                try
                {
                    _logger.Information("🔍 **DATA_FLOW**: Creating OCRContext for database operations");
                    using var context = new OCRContext();
                    _logger.Information("✅ **DATABASE_CONNECTION**: OCRContext created successfully");
                    
                    // DETAILED METADATA ANALYSIS
                    _logger.Information("🔍 **DATA_EXPECTATION**: Attempting to get field metadata for {FieldName}", correction.FieldName);
                    var fieldMetadata = templateContext.GetFieldMetadata(correction.FieldName);
                    _logger.Information("🔍 **DATA_ACTUAL_METADATA**: FieldMetadata = {IsNull} | FieldId = {FieldId} | LineId = {LineId} | RegexId = {RegexId}", 
                        fieldMetadata == null ? "NULL" : "NOT_NULL",
                        fieldMetadata?.FieldId?.ToString() ?? "NULL",
                        fieldMetadata?.LineId?.ToString() ?? "NULL", 
                        fieldMetadata?.RegexId?.ToString() ?? "NULL");

                    _logger.Information("🔍 **LOGIC_FLOW**: Calling GetDatabaseUpdateContext to determine update strategy");
                    var updateContext = this.GetDatabaseUpdateContext(correction.FieldName, fieldMetadata);
                    _logger.Information("🔍 **DATA_ACTUAL_UPDATE_CONTEXT**: UpdateStrategy = {Strategy} | IsValid = {IsValid} | ErrorMessage = {ErrorMessage}", 
                        updateContext.UpdateStrategy.ToString(), 
                        updateContext.IsValid, 
                        updateContext.ErrorMessage ?? "NULL");

                    // STRATEGY DECISION ASSERTION
                    _logger.Information("🔍 **DATA_EXPECTATION**: UpdateStrategy should NOT be SkipUpdate for successful correction");
                    if (updateContext.UpdateStrategy == DatabaseUpdateStrategy.SkipUpdate)
                    {
                        _logger.Error("❌ **ASSERTION_FAILED**: UpdateStrategy is SkipUpdate - correction will be skipped");
                        _logger.Error("🔍 **LOGIC_FLOW**: Returning Failed result due to SkipUpdate strategy");
                        _logger.Information("🔍 **PIPELINE_DATABASE_SKIP**: Skipping database update for field {FieldName}: {Reason}", 
                            correction.FieldName, updateContext.ErrorMessage ?? "No metadata available");
                        return DatabaseUpdateResult.Failed("Update skipped - no metadata available");
                    }
                    _logger.Information("✅ **ASSERTION_PASSED**: UpdateStrategy is valid - proceeding with database update");

                    // LINE CONTEXT CREATION WITH DATA FLOW
                    _logger.Information("🔍 **DATA_FLOW**: Building line context for correction");
                    var lineContext = this.BuildLineContextForCorrection(correction, 
                        templateContext.Metadata, templateContext.FileText);
                    _logger.Information("🔍 **DATA_ACTUAL_LINE_CONTEXT**: LineContext = {IsNull} | LineId = {LineId} | RegexId = {RegexId} | IsOrphaned = {IsOrphaned} | RequiresNewLineCreation = {RequiresNewLineCreation}", 
                        lineContext == null ? "NULL" : "NOT_NULL",
                        lineContext?.LineId?.ToString() ?? "NULL",
                        lineContext?.RegexId?.ToString() ?? "NULL",
                        lineContext?.IsOrphaned.ToString() ?? "NULL",
                        lineContext?.RequiresNewLineCreation.ToString() ?? "NULL");

                    // REQUEST CREATION WITH DATA FLOW
                    _logger.Information("🔍 **DATA_FLOW**: Creating update request for strategy");
                    var request = this.CreateUpdateRequestForStrategy(correction, lineContext, 
                        templateContext.FilePath, templateContext.FileText);
                    _logger.Information("🔍 **DATA_ACTUAL_REQUEST**: Request = {IsNull} | FieldName = {FieldName} | LineId = {LineId} | RegexId = {RegexId} | NewValue = {NewValue} | CorrectionType = {CorrectionType}", 
                        request == null ? "NULL" : "NOT_NULL",
                        request?.FieldName ?? "NULL",
                        request?.LineId?.ToString() ?? "NULL",
                        request?.RegexId?.ToString() ?? "NULL",
                        request?.NewValue ?? "NULL",
                        request?.CorrectionType ?? "NULL");

                    // REQUEST VALIDATION WITH ASSERTIONS
                    _logger.Information("🔍 **DATA_EXPECTATION**: Request validation should pass for well-formed correction");
                    var validationResult = this.ValidateUpdateRequest(request);
                    _logger.Information("🔍 **DATA_ACTUAL_VALIDATION**: IsValid = {IsValid} | ErrorMessage = {ErrorMessage}", 
                        validationResult.IsValid, 
                        validationResult.ErrorMessage ?? "NULL");
                    
                    if (!validationResult.IsValid)
                    {
                        _logger.Error("❌ **ASSERTION_FAILED**: Request validation failed - {Error}", validationResult.ErrorMessage);
                        _logger.Error("🔍 **LOGIC_FLOW**: Returning Failed result due to validation failure");
                        _logger.Warning("❌ **PIPELINE_REQUEST_VALIDATION_FAILED**: Invalid request for {FieldName}: {Error}", 
                            correction.FieldName, validationResult.ErrorMessage);
                        return DatabaseUpdateResult.Failed($"Request validation failed: {validationResult.ErrorMessage}");
                    }
                    _logger.Information("✅ **ASSERTION_PASSED**: Request validation passed - proceeding with strategy execution");

                    // STRATEGY SELECTION WITH DATA FLOW
                    _logger.Information("🔍 **DATA_FLOW**: Getting database update strategy for correction type {CorrectionType}", correction.CorrectionType);
                    _logger.Information("🔍 **DATA_EXPECTATION**: Strategy factory should return non-null strategy for correction type {CorrectionType}", correction.CorrectionType);
                    var strategy = _strategyFactory.GetStrategy(correction);
                    _logger.Information("🔍 **DATA_ACTUAL_STRATEGY**: Strategy = {IsNull} | StrategyType = {StrategyType}", 
                        strategy == null ? "NULL" : "NOT_NULL",
                        strategy?.StrategyType ?? "NULL");
                    
                    if (strategy == null)
                    {
                        _logger.Error("❌ **ASSERTION_FAILED**: No strategy found for correction type {CorrectionType}", correction.CorrectionType);
                        _logger.Error("🔍 **LOGIC_FLOW**: Returning Failed result due to missing strategy");
                        _logger.Warning("❌ **PIPELINE_NO_STRATEGY**: No database update strategy for {FieldName} correction type {CorrectionType}", 
                            correction.FieldName, correction.CorrectionType);
                        return DatabaseUpdateResult.Failed($"No strategy for correction type '{correction.CorrectionType}'");
                    }
                    _logger.Information("✅ **ASSERTION_PASSED**: Strategy found - proceeding with strategy execution");

                    _logger.Information("🔍 **PIPELINE_EXECUTING_STRATEGY**: Executing {StrategyType} for field {FieldName}", 
                        strategy.StrategyType, correction.FieldName);

                    // CRITICAL STRATEGY EXECUTION WITH COMPREHENSIVE DATA FLOW
                    _logger.Information("🔍 **DATABASE_UPDATE_VERBOSE_START**: Starting detailed database update for {FieldName} with DeepSeek regex", correction.FieldName);
                    _logger.Information("🔍 **DATABASE_UPDATE_CORRECTION_DETAIL**: Correction Data = FieldName:{FieldName} | OldValue:{OldValue} | NewValue:{NewValue} | CorrectionType:{CorrectionType} | Success:{Success} | Confidence:{Confidence} | LineText:{LineText} | SuggestedRegex:{SuggestedRegex}", 
                        correction.FieldName, 
                        correction.OldValue ?? "NULL", 
                        correction.NewValue ?? "NULL", 
                        correction.CorrectionType ?? "NULL", 
                        correction.Success, 
                        correction.Confidence, 
                        correction.LineText ?? "NULL",
                        correction.SuggestedRegex ?? "NULL");
                    _logger.Information("🔍 **DATABASE_UPDATE_REQUEST_DETAIL**: Request Data = FieldName:{FieldName} | LineId:{LineId} | RegexId:{RegexId} | OldValue:{OldValue} | NewValue:{NewValue} | CorrectionType:{CorrectionType} | WindowText_Length:{WindowTextLength}", 
                        request.FieldName ?? "NULL", 
                        request.LineId?.ToString() ?? "NULL", 
                        request.RegexId?.ToString() ?? "NULL", 
                        request.OldValue ?? "NULL", 
                        request.NewValue ?? "NULL", 
                        request.CorrectionType ?? "NULL",
                        request.WindowText?.Length.ToString() ?? "NULL");
                    _logger.Information("🔍 **DATABASE_UPDATE_STRATEGY_DETAIL**: {StrategyType} strategy about to execute with OCRContext", strategy.StrategyType);
                    
                    _logger.Information("🔍 **DATA_EXPECTATION**: Strategy ExecuteAsync should return successful DatabaseUpdateResult");
                    DatabaseUpdateResult dbUpdateResult = await strategy.ExecuteAsync(context, request, this).ConfigureAwait(false);
                    
                    _logger.Information("🔍 **DATABASE_UPDATE_RESULT_DETAIL**: ExecuteAsync completed - Success:{Success} | Message:{Message} | RecordId:{RecordId} | IsException:{IsException}", 
                        dbUpdateResult.IsSuccess, 
                        dbUpdateResult.Message ?? "NULL", 
                        dbUpdateResult.RecordId?.ToString() ?? "NULL",
                        dbUpdateResult.Exception != null);
                    
                    if (!dbUpdateResult.IsSuccess)
                    {
                        _logger.Error("❌ **ASSERTION_FAILED**: Strategy ExecuteAsync returned failure - {Message}", dbUpdateResult.Message);
                        _logger.Error("🔍 **LOGIC_FLOW**: Database update failed during strategy execution");
                    }
                    else
                    {
                        _logger.Information("✅ **ASSERTION_PASSED**: Strategy ExecuteAsync returned success - database pattern should be updated");
                    }
                    
                    _logger.Information("🔍 **DATABASE_UPDATE_VERBOSE_END**: Completed detailed database update for {FieldName}", correction.FieldName);

                    // Log learning entry with data flow
                    _logger.Information("🔍 **DATA_FLOW**: Logging correction to OCRCorrectionLearning table");
                    await this.LogCorrectionLearningAsync(context, request, dbUpdateResult).ConfigureAwait(false);
                    _logger.Information("✅ **DATABASE_LEARNING**: OCRCorrectionLearning entry created");

                    if (dbUpdateResult.IsSuccess)
                    {
                        _logger.Information("✅ **PIPELINE_DATABASE_SUCCESS**: Database update successful for {FieldName}: {Message}", 
                            correction.FieldName, dbUpdateResult.Message);
                    }
                    else
                    {
                        _logger.Warning("❌ **PIPELINE_DATABASE_FAILED**: Database update failed for {FieldName}: {Error}", 
                            correction.FieldName, dbUpdateResult.Message);
                    }

                    _logger.Information("🔍 **DATABASE_SECTION_EXIT**: ApplyToDatabaseInternal returning with Success={Success}", dbUpdateResult.IsSuccess);
                    return dbUpdateResult;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "🚨 **PIPELINE_DATABASE_EXCEPTION**: Exception applying correction to database for {FieldName}", correction.FieldName);
                    _logger.Error("🔍 **EXCEPTION_DATA**: ExceptionType = {ExceptionType} | Message = {Message} | StackTrace = {StackTrace}", 
                        ex.GetType().Name, ex.Message, ex.StackTrace);
                    _logger.Error("🔍 **LOGIC_FLOW**: Returning Failed result due to exception");
                    return DatabaseUpdateResult.Failed($"Exception during database update: {ex.Message}", ex);
                }
            }
        }

        /// <summary>
        /// Re-imports template using updated patterns and validates results.
        /// Uses the existing ClearInvoiceForReimport() and template.Read() pattern.
        /// Testable instance method called by extension method.
        /// </summary>
        internal async Task<ReimportResult> ReimportAndValidateInternal(DatabaseUpdateResult updateResult, TemplateContext templateContext, string fileText)
        {
            var result = new ReimportResult();
            
            if (updateResult == null || !updateResult.IsSuccess || templateContext == null)
            {
                result.Success = false;
                result.ErrorMessage = "Invalid input parameters for re-import";
                return result;
            }

            _logger.Information("🔍 **PIPELINE_REIMPORT_START**: Re-importing template after database update");

            try
            {
                var startTime = DateTime.UtcNow;

                // Get the template from context - we need to reconstruct it from metadata
                if (!templateContext.InvoiceId.HasValue)
                {
                    result.Success = false;
                    result.ErrorMessage = "No InvoiceId available in template context for re-import";
                    return result;
                }

                // Load the updated template from database
                using var ocrContext = new OCRContext();
                var ocrInvoice = await ocrContext.Invoices
                    .Include(i => i.Parts.Select(p => p.Lines.Select(l => l.RegularExpressions)))
                    .Include(i => i.Parts.Select(p => p.Lines.Select(l => l.Fields)))
                    .FirstOrDefaultAsync(i => i.Id == templateContext.InvoiceId.Value)
                    .ConfigureAwait(false);

                if (ocrInvoice == null)
                {
                    result.Success = false;
                    result.ErrorMessage = $"Template with InvoiceId {templateContext.InvoiceId} not found in database";
                    return result;
                }

                _logger.Information("🔍 **PIPELINE_REIMPORT_TEMPLATE_LOADED**: Loaded template {InvoiceName} with {PartCount} parts", 
                    ocrInvoice.Name, ocrInvoice.Parts?.Count ?? 0);

                // Create new template instance with updated database patterns
                var template = new Invoice(ocrInvoice, _logger);

                // Step 1: Clear all mutable state (existing pattern from ReadFormattedTextStep.cs)
                _logger.Information("🔍 **PIPELINE_REIMPORT_CLEAR**: Clearing template mutable state for re-import");
                template.ClearInvoiceForReimport();

                // Step 2: Re-read template with updated patterns (existing pattern from ReadFormattedTextStep.cs)
                _logger.Information("🔍 **PIPELINE_REIMPORT_READ**: Re-reading template with updated database patterns");
                var textLines = fileText?.Split(new[] { '\r', '\n' }, StringSplitOptions.None).ToList() ?? new List<string>();
                var extractedData = template.Read(textLines);

                if (extractedData == null || !extractedData.Any())
                {
                    result.Success = false;
                    result.ErrorMessage = "Template re-read produced no data - possible template corruption";
                    _logger.Warning("❌ **PIPELINE_REIMPORT_NO_DATA**: Template re-read produced no extracted data");
                    return result;
                }

                _logger.Information("✅ **PIPELINE_REIMPORT_DATA_EXTRACTED**: Re-read extracted {RecordCount} data records", extractedData.Count);

                // Step 3: Calculate TotalsZero to validate correction (existing pattern from ReadFormattedTextStep.cs)
                bool isBalanced = OCRCorrectionService.TotalsZero(extractedData, out var totalsZero, _logger);
                result.TotalsZero = totalsZero;

                _logger.Information("🔍 **PIPELINE_REIMPORT_VALIDATION**: TotalsZero = {TotalsZero}, IsBalanced = {IsBalanced}", 
                    totalsZero, isBalanced);

                // Step 4: Extract individual field values for comparison
                result.ExtractedValues = new Dictionary<string, object>();
                if (extractedData.Any())
                {
                    var firstRecord = extractedData.First();
                    if (firstRecord != null)
                    {
                        // Extract values for Caribbean customs fields
                        foreach (var property in new[] { "InvoiceNo", "InvoiceTotal", "SubTotal", "TotalInternalFreight", 
                            "TotalOtherCost", "TotalInsurance", "TotalDeduction" })
                        {
                            try
                            {
                                var value = ((IDictionary<string, object>)firstRecord).ContainsKey(property) 
                                    ? ((IDictionary<string, object>)firstRecord)[property] 
                                    : null;
                                
                                if (value != null)
                                {
                                    result.ExtractedValues[property] = value;
                                    _logger.Information("🔍 **PIPELINE_REIMPORT_FIELD_{Field}**: Extracted value = {Value}", property, value);
                                }
                            }
                            catch (Exception fieldEx)
                            {
                                _logger.Warning("⚠️ **PIPELINE_REIMPORT_FIELD_ERROR**: Error extracting field {Field}: {Error}", 
                                    property, fieldEx.Message);
                            }
                        }
                    }
                }

                // Step 5: Determine success based on improvement
                result.Success = Math.Abs(totalsZero) < 0.01 || template.Success; // Balanced OR template reports success
                result.Duration = DateTime.UtcNow - startTime;

                if (result.Success)
                {
                    _logger.Information("✅ **PIPELINE_REIMPORT_SUCCESS**: Template re-import successful - TotalsZero = {TotalsZero}, Template.Success = {TemplateSuccess}", 
                        totalsZero, template.Success);
                }
                else
                {
                    _logger.Warning("⚠️ **PIPELINE_REIMPORT_PARTIAL**: Template re-import completed but not fully balanced - TotalsZero = {TotalsZero}", 
                        totalsZero);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "🚨 **PIPELINE_REIMPORT_EXCEPTION**: Exception during template re-import");
                result.Success = false;
                result.ErrorMessage = $"Exception during re-import: {ex.Message}";
                result.Duration = DateTime.UtcNow - DateTime.UtcNow; // Zero duration on exception
                return result;
            }
        }

        /// <summary>
        /// Updates actual invoice data using corrected values.
        /// Bridges OCRContext changes to EntryDataDSContext by mapping extracted field values to ShipmentInvoice entity.
        /// Applies Caribbean customs business rules for field mappings.
        /// Testable instance method called by extension method.
        /// </summary>
        internal async Task<InvoiceUpdateResult> UpdateInvoiceDataInternal(ReimportResult reimportResult, global::EntryDataDS.Business.Entities.ShipmentInvoice invoice)
        {
            var result = new InvoiceUpdateResult();
            
            if (reimportResult == null || !reimportResult.Success || invoice == null)
            {
                result.Success = false;
                result.ErrorMessage = "Invalid input parameters for invoice update";
                return result;
            }

            _logger.Information("🔍 **PIPELINE_INVOICE_UPDATE_START**: Updating invoice data for {InvoiceNo}", invoice.InvoiceNo);

            try
            {
                var startTime = DateTime.UtcNow;
                var fieldsUpdated = new List<string>();
                var valuesChanged = new Dictionary<string, object>();
                
                // Calculate TotalsZero before update
                double totalsZeroBefore;
                OCRCorrectionService.TotalsZero(invoice, out totalsZeroBefore, _logger);
                result.TotalsZeroBefore = totalsZeroBefore;
                _logger.Information("🔍 **PIPELINE_INVOICE_BEFORE**: TotalsZero before correction = {TotalsZero:F2}", result.TotalsZeroBefore);

                // Check if we have extracted values from re-import
                if (reimportResult.ExtractedValues == null || !reimportResult.ExtractedValues.Any())
                {
                    _logger.Warning("⚠️ **PIPELINE_NO_EXTRACTED_VALUES**: No extracted values available from re-import to apply to invoice");
                    result.Success = false;
                    result.ErrorMessage = "No extracted values available for invoice update";
                    return result;
                }

                // Map extracted values to ShipmentInvoice fields following Caribbean customs business rules
                _logger.Information("🔍 **PIPELINE_FIELD_MAPPING_START**: Applying {Count} extracted values to invoice fields", reimportResult.ExtractedValues.Count);

                foreach (var kvp in reimportResult.ExtractedValues)
                {
                    var fieldName = kvp.Key;
                    var extractedValue = kvp.Value;
                    
                    _logger.Debug("🔍 **PIPELINE_MAPPING_FIELD**: {FieldName} = {ExtractedValue} (Type: {ValueType})", 
                        fieldName, extractedValue, extractedValue?.GetType().Name ?? "null");

                    try
                    {
                        // Map Caribbean customs fields to ShipmentInvoice properties
                        switch (fieldName)
                        {
                            case "InvoiceNo":
                                // InvoiceNo typically doesn't change during correction, but validate consistency
                                if (extractedValue != null && extractedValue.ToString() != invoice.InvoiceNo)
                                {
                                    _logger.Warning("⚠️ **PIPELINE_INVOICE_NO_MISMATCH**: Extracted InvoiceNo '{ExtractedValue}' differs from invoice '{InvoiceNo}'", 
                                        extractedValue, invoice.InvoiceNo);
                                }
                                break;

                            case "SubTotal":
                                if (TryParseDecimal(extractedValue, out var subTotal) && (double)subTotal != (double)(invoice.SubTotal ?? 0))
                                {
                                    valuesChanged["SubTotal"] = $"{(invoice.SubTotal ?? 0):F2} → {subTotal:F2}";
                                    invoice.SubTotal = (double?)subTotal;
                                    fieldsUpdated.Add("SubTotal");
                                }
                                break;

                            case "InvoiceTotal":
                                if (TryParseDecimal(extractedValue, out var invoiceTotal) && (double)invoiceTotal != (double)(invoice.InvoiceTotal ?? 0))
                                {
                                    valuesChanged["InvoiceTotal"] = $"{(invoice.InvoiceTotal ?? 0):F2} → {invoiceTotal:F2}";
                                    invoice.InvoiceTotal = (double?)invoiceTotal;
                                    fieldsUpdated.Add("InvoiceTotal");
                                }
                                break;

                            case "TotalInternalFreight":
                                if (TryParseDecimal(extractedValue, out var freight) && (double)freight != (double)(invoice.TotalInternalFreight ?? 0))
                                {
                                    valuesChanged["TotalInternalFreight"] = $"{(invoice.TotalInternalFreight ?? 0):F2} → {freight:F2}";
                                    invoice.TotalInternalFreight = (double?)freight;
                                    fieldsUpdated.Add("TotalInternalFreight");
                                }
                                break;

                            case "TotalOtherCost":
                                if (TryParseDecimal(extractedValue, out var otherCost) && (double)otherCost != (double)(invoice.TotalOtherCost ?? 0))
                                {
                                    valuesChanged["TotalOtherCost"] = $"{(invoice.TotalOtherCost ?? 0):F2} → {otherCost:F2}";
                                    invoice.TotalOtherCost = (double?)otherCost;
                                    fieldsUpdated.Add("TotalOtherCost");
                                }
                                break;

                            case "TotalInsurance":
                                // Caribbean Customs Rule: Customer-caused reductions (gift cards, store credits) → TotalInsurance (negative values)
                                if (TryParseDecimal(extractedValue, out var insurance) && (double)insurance != (double)(invoice.TotalInsurance ?? 0))
                                {
                                    valuesChanged["TotalInsurance"] = $"{(invoice.TotalInsurance ?? 0):F2} → {insurance:F2}";
                                    invoice.TotalInsurance = (double?)insurance;
                                    fieldsUpdated.Add("TotalInsurance");
                                    _logger.Information("🔍 **PIPELINE_CARIBBEAN_RULE_INSURANCE**: Applied customer reduction to TotalInsurance = {Insurance:F2}", insurance);
                                }
                                break;

                            case "TotalDeduction":
                                // Caribbean Customs Rule: Supplier-caused reductions (free shipping, discounts) → TotalDeduction (positive values)
                                if (TryParseDecimal(extractedValue, out var deduction) && (double)deduction != (double)(invoice.TotalDeduction ?? 0))
                                {
                                    valuesChanged["TotalDeduction"] = $"{(invoice.TotalDeduction ?? 0):F2} → {deduction:F2}";
                                    invoice.TotalDeduction = (double?)deduction;
                                    fieldsUpdated.Add("TotalDeduction");
                                    _logger.Information("🔍 **PIPELINE_CARIBBEAN_RULE_DEDUCTION**: Applied supplier reduction to TotalDeduction = {Deduction:F2}", deduction);
                                }
                                break;

                            default:
                                _logger.Debug("🔍 **PIPELINE_UNMAPPED_FIELD**: Field '{FieldName}' not mapped to ShipmentInvoice property", fieldName);
                                break;
                        }
                    }
                    catch (Exception fieldEx)
                    {
                        _logger.Warning(fieldEx, "⚠️ **PIPELINE_FIELD_UPDATE_ERROR**: Error updating field {FieldName} with value {Value}", 
                            fieldName, extractedValue);
                    }
                }

                // Log all field updates
                if (fieldsUpdated.Any())
                {
                    _logger.Information("✅ **PIPELINE_FIELDS_UPDATED**: Updated {Count} fields: {Fields}", 
                        fieldsUpdated.Count, string.Join(", ", fieldsUpdated));
                    
                    foreach (var change in valuesChanged)
                    {
                        _logger.Information("🔍 **PIPELINE_VALUE_CHANGE**: {FieldName}: {Change}", change.Key, change.Value);
                    }
                }
                else
                {
                    _logger.Information("🔍 **PIPELINE_NO_CHANGES**: No field values changed during correction");
                }

                // Save changes to EntryDataDSContext
                _logger.Information("🔍 **PIPELINE_DATABASE_SAVE**: Saving invoice changes to EntryDataDSContext");
                
                using (var entryDataContext = new global::EntryDataDS.Business.Entities.EntryDataDSContext())
                {
                    // Attach the invoice to the context if not already tracked
                    if (entryDataContext.Entry(invoice).State == System.Data.Entity.EntityState.Detached)
                    {
                        entryDataContext.ShipmentInvoice.Attach(invoice);
                        entryDataContext.Entry(invoice).State = System.Data.Entity.EntityState.Modified;
                        _logger.Debug("🔍 **PIPELINE_ENTITY_ATTACHED**: Invoice attached to EntryDataDSContext as Modified");
                    }

                    int changesSaved = await entryDataContext.SaveChangesAsync().ConfigureAwait(false);
                    _logger.Information("✅ **PIPELINE_DATABASE_SAVED**: Saved {ChangeCount} changes to database", changesSaved);
                }

                // Calculate TotalsZero after update to verify correction
                double totalsZeroAfter;
                OCRCorrectionService.TotalsZero(invoice, out totalsZeroAfter, _logger);
                result.TotalsZeroAfter = totalsZeroAfter;
                result.Duration = DateTime.UtcNow - startTime;
                
                // Determine success based on improvement in balance
                bool isBalanced = Math.Abs(result.TotalsZeroAfter) <= 0.01;
                bool isImproved = Math.Abs(result.TotalsZeroAfter) < Math.Abs(result.TotalsZeroBefore);
                
                result.Success = isBalanced || isImproved;
                
                if (isBalanced)
                {
                    _logger.Information("✅ **PIPELINE_INVOICE_UPDATE_SUCCESS**: Invoice fully balanced - TotalsZero {Before:F2} → {After:F2}", 
                        result.TotalsZeroBefore, result.TotalsZeroAfter);
                }
                else if (isImproved)
                {
                    _logger.Information("⚠️ **PIPELINE_INVOICE_UPDATE_PARTIAL**: Invoice improved but not fully balanced - TotalsZero {Before:F2} → {After:F2}", 
                        result.TotalsZeroBefore, result.TotalsZeroAfter);
                }
                else
                {
                    _logger.Warning("❌ **PIPELINE_INVOICE_UPDATE_NO_IMPROVEMENT**: Invoice balance did not improve - TotalsZero {Before:F2} → {After:F2}", 
                        result.TotalsZeroBefore, result.TotalsZeroAfter);
                }

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "🚨 **PIPELINE_INVOICE_UPDATE_EXCEPTION**: Exception updating invoice data for {InvoiceNo}", invoice?.InvoiceNo);
                result.Success = false;
                result.ErrorMessage = $"Exception during invoice update: {ex.Message}";
                return result;
            }
        }

        /// <summary>
        /// Safely parses a value to decimal, handling null, empty, string, and numeric types.
        /// </summary>
        private bool TryParseDecimal(object value, out decimal result)
        {
            result = 0;
            
            if (value == null)
                return false;
                
            // Handle direct decimal/double/float types
            if (value is decimal d)
            {
                result = d;
                return true;
            }
            if (value is double dbl)
            {
                result = (decimal)dbl;
                return true;
            }
            if (value is float flt)
            {
                result = (decimal)flt;
                return true;
            }
            if (value is int i)
            {
                result = i;
                return true;
            }
            
            // Handle string parsing with culture-aware formatting
            var stringValue = value.ToString()?.Trim();
            if (string.IsNullOrEmpty(stringValue))
                return false;
                
            // Remove common currency symbols and formatting
            stringValue = stringValue.Replace("$", "").Replace(",", "").Replace("(", "-").Replace(")", "").Trim();
            
            return decimal.TryParse(stringValue, out result);
        }

        /// <summary>
        /// Determines PartId for an omitted field based on field type and existing template structure.
        /// This should NEVER return null for existing templates as header/detail parts are mandatory.
        /// </summary>
        /// <param name="fieldName">The field name being corrected</param>
        /// <param name="fieldInfo">Field mapping information</param>
        /// <returns>PartId for the appropriate template part</returns>
        private int? DeterminePartIdForOmissionField(string fieldName, DatabaseFieldInfo fieldInfo)
        {
            try
            {
                using var context = new OCRContext();
                
                // Determine target part type based on field information
                string targetPartTypeName = "Header"; // Default to Header for most invoice fields
                
                if (fieldInfo != null)
                {
                    if (fieldInfo.EntityType == "InvoiceDetails")
                    {
                        targetPartTypeName = "LineItem";
                    }
                    // ShipmentInvoice fields go to Header (default)
                }
                else if (!string.IsNullOrEmpty(fieldName))
                {
                    // Fallback: try to infer from field name
                    var inferredFieldInfo = this.MapDeepSeekFieldToDatabase(fieldName);
                    if (inferredFieldInfo?.EntityType == "InvoiceDetails" || fieldName.ToLower().Contains("invoicedetail"))
                    {
                        targetPartTypeName = "LineItem";
                    }
                }

                _logger?.Error("🔍 **DETERMINE_PARTID**: FieldName={FieldName} | TargetPartType={PartType} | FieldEntityType={EntityType}", 
                    fieldName, targetPartTypeName, fieldInfo?.EntityType ?? "INFERRED");

                // Find the part with matching PartType
                var part = context.Parts.Include(p => p.PartTypes)
                                       .FirstOrDefault(p => p.PartTypes.Name.Equals(targetPartTypeName, StringComparison.OrdinalIgnoreCase));

                if (part != null)
                {
                    _logger?.Error("✅ **DETERMINE_PARTID_SUCCESS**: FieldName={FieldName} | Found PartId={PartId} for PartType={PartType}", 
                        fieldName, part.Id, targetPartTypeName);
                    return part.Id;
                }

                _logger?.Error("⚠️ **DETERMINE_PARTID_FALLBACK**: Could not find part with PartType={PartType} for field {FieldName}, using first available part", 
                    targetPartTypeName, fieldName);
                
                // Fallback: use first available part (should not happen for well-formed templates)
                var firstPart = context.Parts.OrderBy(p => p.Id).FirstOrDefault();
                if (firstPart != null)
                {
                    _logger?.Error("⚠️ **DETERMINE_PARTID_FALLBACK_SUCCESS**: Using fallback PartId={PartId} for field {FieldName}", 
                        firstPart.Id, fieldName);
                    return firstPart.Id;
                }

                _logger?.Error("❌ **DETERMINE_PARTID_FAILED**: No Parts found in database for field {FieldName} - template structure incomplete", fieldName);
                return null;
            }
            catch (Exception ex)
            {
                _logger?.Error(ex, "❌ **DETERMINE_PARTID_EXCEPTION**: Exception determining PartId for field {FieldName}", fieldName);
                return null;
            }
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Builds a LineContext object for a given correction, using available metadata and file text.
        /// This is used to provide context to strategies, especially for omission handling.
        /// </summary>
        private LineContext BuildLineContextForCorrection(
            CorrectionResult correction,
            Dictionary<string, OCRFieldMetadata> invoiceMetadata,
            string fileText)
        {
            OCRFieldMetadata fieldMeta = null;
            if (invoiceMetadata != null)
            {
                if (invoiceMetadata.TryGetValue(correction.FieldName, out var directMeta))
                {
                    fieldMeta = directMeta;
                }
                else
                {
                    var mappedInfo = this.MapDeepSeekFieldToDatabase(correction.FieldName);
                    if (mappedInfo != null && invoiceMetadata.TryGetValue(mappedInfo.DatabaseFieldName, out var mappedMeta))
                    {
                        fieldMeta = mappedMeta;
                    }
                }
            }

            var lineContext = new LineContext
            {
                LineNumber = correction.LineNumber,
                LineText = correction.LineText ?? (correction.LineNumber > 0 ? this.GetOriginalLineText(fileText, correction.LineNumber) : null),
                ContextLinesBefore = correction.ContextLinesBefore,
                ContextLinesAfter = correction.ContextLinesAfter,
                RequiresMultilineRegex = correction.RequiresMultilineRegex,
                WindowText = (correction.LineNumber > 0) ? this.ExtractWindowText(fileText, correction.LineNumber, 5) : null
            };

            if (fieldMeta != null)
            {
                lineContext.LineId = fieldMeta.LineId;
                lineContext.RegexId = fieldMeta.RegexId;
                lineContext.RegexPattern = fieldMeta.LineRegex;
                lineContext.PartId = fieldMeta.PartId;
                lineContext.LineName = fieldMeta.LineName;
                lineContext.PartName = fieldMeta.PartName;
                lineContext.PartTypeId = fieldMeta.PartTypeId;
                // For FieldsInLine, the strategy would typically call GetFieldsByRegexNamedGroupsAsync if needed with lineContext.LineId
            }
            else if (correction.CorrectionType == "omission" || correction.CorrectionType == "omitted_line_item")
            {
                lineContext.IsOrphaned = true;
                lineContext.RequiresNewLineCreation = true;
            }

            return lineContext;
        }

        /// <summary>
        /// Validates a RegexUpdateRequest for essential data and conformity to field rules.
        /// </summary>
        private FieldValidationInfo ValidateUpdateRequest(RegexUpdateRequest request)
        {
            if (request == null) return new FieldValidationInfo { IsValid = false, ErrorMessage = "Request object is null." };
            if (string.IsNullOrWhiteSpace(request.FieldName))
                return new FieldValidationInfo { IsValid = false, ErrorMessage = "Field name is required in RegexUpdateRequest." };
            if (request.NewValue == null && request.CorrectionType != "removal")
                return new FieldValidationInfo { IsValid = false, ErrorMessage = "New value is required for non-removal corrections." };

            // IsFieldSupported and GetFieldValidationInfo are instance methods (likely in OCRFieldMapping.cs part)
            if (!this.IsFieldSupported(request.FieldName))
                return new FieldValidationInfo { IsValid = false, ErrorMessage = $"Field '{request.FieldName}' is not supported for database updates." };

            var fieldValidationRules = this.GetFieldValidationInfo(request.FieldName);
            if (!fieldValidationRules.IsValid) return fieldValidationRules; // Pass along error from GetFieldValidationInfo

            if (fieldValidationRules.IsRequired && string.IsNullOrWhiteSpace(request.NewValue))
                return new FieldValidationInfo { IsValid = false, ErrorMessage = $"Field '{request.FieldName}' is required and cannot be empty." };

            // Validate NewValue against pattern only if NewValue is not empty (empty might be a valid 'cleared' state)
            if (!string.IsNullOrEmpty(fieldValidationRules.ValidationPattern) && !string.IsNullOrWhiteSpace(request.NewValue))
            {
                if (!Regex.IsMatch(request.NewValue, fieldValidationRules.ValidationPattern))
                    return new FieldValidationInfo { IsValid = false, ErrorMessage = $"New value '{this.TruncateForLog(request.NewValue, 50)}' for '{request.FieldName}' does not match expected pattern: {fieldValidationRules.ValidationPattern}" };
            }
            return new FieldValidationInfo { IsValid = true, DatabaseFieldName = fieldValidationRules.DatabaseFieldName, EntityType = fieldValidationRules.EntityType };
        }

        /// <summary>
        /// Logs details of a correction attempt and its database update outcome to the OCRCorrectionLearning table.
        /// </summary>
        private async Task LogCorrectionLearningAsync(OCRContext context, RegexUpdateRequest request, DatabaseUpdateResult dbUpdateResult)
        {
            if (request == null)
            {
                _logger.Error("LogCorrectionLearningAsync: RegexUpdateRequest is null. Cannot log learning entry.");
                return;
            }
            if (dbUpdateResult == null)
            { // Should not happen if called correctly
                _logger.Error("LogCorrectionLearningAsync: DatabaseUpdateResult is null for field {FieldName}. Cannot log learning entry.", request.FieldName);
                dbUpdateResult = DatabaseUpdateResult.Failed("Internal error: DBUpdateResult was null.");
            }

            try
            {
                // DETAILED DATABASE LOGGING: Show exactly what's being saved
                _logger.Error("🔍 **DATABASE_SAVE_ENTRY**: Preparing OCRCorrectionLearning entry");
                _logger.Error("🔍 **DATABASE_SAVE_FIELD**: FieldName={FieldName} | OldValue={OldValue} | NewValue={NewValue}", 
                    request.FieldName, request.OldValue ?? "NULL", request.NewValue ?? "NULL");
                _logger.Error("🔍 **DATABASE_SAVE_LINE**: LineNumber={LineNumber} | LineText={LineText}", 
                    request.LineNumber, request.LineText ?? "NULL");
                _logger.Error("🔍 **DATABASE_SAVE_CORRECTION**: CorrectionType={CorrectionType} | Confidence={Confidence}", 
                    request.CorrectionType ?? "NULL", request.Confidence);
                _logger.Error("🔍 **DATABASE_SAVE_RESULT**: Success={Success} | Operation={Operation} | RecordId={RecordId}", 
                    dbUpdateResult.IsSuccess, dbUpdateResult.Operation ?? "NULL", dbUpdateResult.RecordId?.ToString() ?? "NULL");
                _logger.Error("🔍 **DATABASE_SAVE_IDS**: LineId={LineId} | PartId={PartId} | RegexId={RegexId}", 
                    request.LineId?.ToString() ?? "NULL", request.PartId?.ToString() ?? "NULL", request.RegexId?.ToString() ?? "NULL");

                var learning = new OCRCorrectionLearning
                {
                    FieldName = request.FieldName,
                    OriginalError = request.OldValue ?? "",
                    CorrectValue = request.NewValue ?? "", // Ensure not null
                    LineNumber = request.LineNumber,
                    LineText = request.LineText ?? "",
                    WindowText = request.WindowText,
                    CorrectionType = request.CorrectionType,
                    DeepSeekReasoning = this.TruncateForLog(request.DeepSeekReasoning, 1000),
                    Confidence = request.Confidence >= -1000000 && request.Confidence <= 1000000 ? (decimal?)request.Confidence : null, // Range check for decimal
                    InvoiceType = request.InvoiceType,
                    FilePath = this.TruncateForLog(request.FilePath, 260),
                    Success = dbUpdateResult.IsSuccess,
                    ErrorMessage = dbUpdateResult.IsSuccess ? null : this.TruncateForLog(dbUpdateResult.Message, 2000),
                    CreatedDate = DateTime.UtcNow,
                    CreatedBy = "OCRCorrectionService", // System identifier

                    RequiresMultilineRegex = request.RequiresMultilineRegex,
                    ContextLinesBefore = request.ContextLinesBefore != null ? string.Join("\n", request.ContextLinesBefore) : null,
                    ContextLinesAfter = request.ContextLinesAfter != null ? string.Join("\n", request.ContextLinesAfter) : null,
                    // LineId in request can be Fields.Id for FieldFormat strategy, or Lines.Id for Omission modify
                    LineId = request.LineId,
                    PartId = request.PartId,
                    // If DB update was successful and involved a Regex, log its ID.
                    RegexId = (dbUpdateResult.IsSuccess && dbUpdateResult.Operation != null && (dbUpdateResult.Operation.Contains("Regex") || dbUpdateResult.Operation.Contains("Pattern"))) ?
                                dbUpdateResult.RecordId : request.RegexId
                };
                
                _logger.Error("🔍 **DATABASE_SAVE_LEARNING_OBJECT**: Creating learning entry with computed RegexId={ComputedRegexId}", 
                    learning.RegexId?.ToString() ?? "NULL");
                
                context.OCRCorrectionLearning.Add(learning);
                
                _logger.Error("🔍 **DATABASE_SAVE_COMMIT**: About to save OCRCorrectionLearning entry to database");
                await context.SaveChangesAsync().ConfigureAwait(false); // Save learning entry
                _logger.Error("✅ **DATABASE_SAVE_SUCCESS**: OCRCorrectionLearning entry saved successfully for field {FieldName}", request.FieldName);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **DATABASE_SAVE_FAILED**: Failed to log correction learning entry for field {FieldName}. Error: {DbUpdateErrorMessage}", request.FieldName, dbUpdateResult.Message);
                // Do not re-throw, logging failure should not halt main processing.
            }
        }

        /// <summary>
        /// Creates a RegexUpdateRequest object populated from a CorrectionResult and other contextual information.
        /// This request object is then used by database update strategies.
        /// </summary>
        private RegexUpdateRequest CreateUpdateRequestForStrategy(
            CorrectionResult correction,
            LineContext lineContext, // Generated by BuildLineContextForCorrection
            string filePath,
            string fileText) // Full original file text
        {
            if (correction == null)
            {
                _logger.Error("CreateUpdateRequestForStrategy: CorrectionResult is null. Cannot create request.");
                return null; // Or throw argument null exception
            }
            if (lineContext == null)
            {
                // This might be acceptable for some corrections if they don't need line context for DB update,
                // but strategies for omissions or line modifications will likely require it.
                _logger.Warning("CreateUpdateRequestForStrategy: LineContext is null for field {FieldName}. Request may lack DB context.", correction.FieldName);
                // Create a minimal LineContext if it's absolutely null to avoid null refs,
                // though BuildLineContextForCorrection should ideally always return a (potentially sparse) object.
                lineContext = new LineContext
                {
                    LineNumber = correction.LineNumber,
                    LineText = correction.LineText,
                    ContextLinesBefore = correction.ContextLinesBefore,
                    ContextLinesAfter = correction.ContextLinesAfter,
                    RequiresMultilineRegex = correction.RequiresMultilineRegex
                };
            }


            var request = new RegexUpdateRequest
            {
                FieldName = correction.FieldName,
                OldValue = correction.OldValue,
                NewValue = correction.NewValue,
                CorrectionType = correction.CorrectionType,
                Confidence = correction.Confidence,
                DeepSeekReasoning = correction.Reasoning,

                LineNumber = correction.LineNumber, // From CorrectionResult, which should be authoritative
                LineText = correction.LineText,     // From CorrectionResult
                WindowText = (lineContext.LineNumber > 0 && !string.IsNullOrEmpty(fileText)) ?
                             this.ExtractWindowText(fileText, lineContext.LineNumber, 5) : // From OCRUtilities
                             (correction.LineNumber > 0 && !string.IsNullOrEmpty(fileText) ? this.ExtractWindowText(fileText, correction.LineNumber, 5) : null),
                ContextLinesBefore = correction.ContextLinesBefore,
                ContextLinesAfter = correction.ContextLinesAfter,
                RequiresMultilineRegex = correction.RequiresMultilineRegex,

                FilePath = filePath,
                InvoiceType = this.DetermineInvoiceType(filePath), // From OCRUtilities

                // Database context primarily from the passed LineContext
                LineId = lineContext.LineId,        // This is OCR.Business.Entities.Lines.Id from existing template line
                PartId = lineContext.PartId,        // OCR.Business.Entities.Parts.Id
                RegexId = lineContext.RegexId,      // OCR.Business.Entities.RegularExpressions.Id from existing template line
                ExistingRegex = lineContext.RegexPattern // Actual regex string of the existing line
            };

            // Special handling for FieldFormatUpdateStrategy:
            // It expects Fields.Id to be passed in request.LineId.
            // The BuildLineContextForCorrection might not have this specific Fields.Id directly,
            // as LineContext is about a "line of text".
            // The OCRFieldMetadata associated with the *specific field instance* being corrected would have Fields.Id.
            // This logic needs to be robust. If invoiceMetadata was passed down to here:
            // Dictionary<string, OCRFieldMetadata> invoiceMetadata = ... (would need to be parameter)
            // if (invoiceMetadata != null && 
            //     _strategyFactory.GetStrategy(correction) is FieldFormatUpdateStrategy && 
            //     invoiceMetadata.TryGetValue(correction.FieldName, out var fieldMetaForThisCorrection) &&
            //     fieldMetaForThisCorrection.FieldId.HasValue)
            // {
            //     request.LineId = fieldMetaForThisCorrection.FieldId; // Repurpose LineId for Fields.Id
            //      _logger.Debug("For FieldFormatStrategy on {FieldName}, setting request.LineId to Fields.Id: {FieldId}", 
            //          correction.FieldName, fieldMetaForThisCorrection.FieldId.Value);
            // }
            // For now, the caller (UpdateRegexPatternsAsync) will need to ensure that if the strategy
            // is FieldFormatUpdateStrategy, the `request.LineId` (originally from lineContext.LineId)
            // is appropriately set to the `Fields.Id` if that's the convention.
            // The current BuildLineContextForCorrection tries to set LineId from OCRFieldMetadata.LineId.
            // A cleaner way is for CreateUpdateRequestForStrategy to take invoiceMetadata.

            return request;
        }


        #endregion
    }
}