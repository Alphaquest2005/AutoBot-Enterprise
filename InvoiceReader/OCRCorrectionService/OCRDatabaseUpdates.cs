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
using Serilog; // For ILogger

namespace WaterNut.DataSpace
{
    public partial class OCRCorrectionService
    {
        // _strategyFactory is initialized in OCRCorrectionService.cs constructor

        #region Main Database Update Methods

        public async Task UpdateRegexPatternsAsync(
            IEnumerable<RegexUpdateRequest> regexUpdateRequests)
        {
            if (regexUpdateRequests == null || !regexUpdateRequests.Any())
            {
                _logger?.Information("UpdateRegexPatternsAsync: No regex update requests provided for database pattern updates.");
                return;
            }

            _logger?.Information("Starting database pattern updates for {RequestCount} regex update requests.", regexUpdateRequests.Count());
            _strategyFactory ??= new DatabaseUpdateStrategyFactory(_logger);

            int dbSuccessCount = 0;
            int dbFailureCount = 0;
            int omissionPatternUpdates = 0;
            int formatPatternUpdates = 0;

            using var context = new OCRContext();

            foreach (var request in regexUpdateRequests)
            {
                DatabaseUpdateResult dbUpdateResult = null;
                IDatabaseUpdateStrategy strategy = null;

                try
                {
                    // ENHANCED LOGGING: Track complete request input
                    _logger?.Error("🔍 **DB_UPDATE_REQUEST_INPUT**: FieldName={FieldName} | OldValue={OldValue} | NewValue={NewValue} | CorrectionType={CorrectionType} | InvoiceId={InvoiceId} | PartName={PartName}",
                        request.FieldName, request.OldValue, request.NewValue, request.CorrectionType, request.InvoiceId, request.PartName);
                    
                    // 1. Validate the request for basic soundness before selecting a strategy
                    var validationResult = this.ValidateUpdateRequest(request);
                    
                    // ENHANCED LOGGING: Show validation result
                    _logger?.Error("🔍 **DB_UPDATE_VALIDATION**: FieldName={FieldName} | IsValid={IsValid} | ErrorMessage={ErrorMessage}",
                        request.FieldName, validationResult.IsValid, validationResult.ErrorMessage ?? "NULL");
                    
                    if (!validationResult.IsValid)
                    {
                        _logger?.Error("❌ **DB_UPDATE_VALIDATION_FAILED**: FieldName={FieldName} | ValidationError={ErrorMessage} | Skipping DB update",
                            request.FieldName, validationResult.ErrorMessage);
                        dbUpdateResult = DatabaseUpdateResult.Failed($"Validation failed: {validationResult.ErrorMessage}");
                        dbFailureCount++;
                        await this.LogCorrectionLearningAsync(context, request, dbUpdateResult).ConfigureAwait(false);
                        continue;
                    }

                    // 2. Get the appropriate strategy
                    strategy = _strategyFactory.GetStrategy(request);
                    
                    // ENHANCED LOGGING: Show strategy selection
                    if (strategy != null)
                    {
                        _logger?.Error("🔍 **DB_UPDATE_STRATEGY_SELECTED**: FieldName={FieldName} | CorrectionType={CorrectionType} | StrategyType={StrategyType}",
                            request.FieldName, request.CorrectionType, strategy.StrategyType);
                    }
                    else
                    {
                        _logger?.Error("❌ **DB_UPDATE_NO_STRATEGY**: FieldName={FieldName} | CorrectionType={CorrectionType} | No strategy found for this type",
                            request.FieldName, request.CorrectionType);
                        dbUpdateResult = DatabaseUpdateResult.Failed($"No strategy for type '{request.CorrectionType}'");
                        dbFailureCount++;
                        await this.LogCorrectionLearningAsync(context, request, dbUpdateResult).ConfigureAwait(false);
                        continue;
                    }

                    // 3. Execute the strategy
                    _logger?.Error("🔍 **DB_UPDATE_STRATEGY_EXECUTING**: FieldName={FieldName} | StrategyType={StrategyType} | About to execute strategy",
                        request.FieldName, strategy.StrategyType);
                    dbUpdateResult = await strategy.ExecuteAsync(context, request, this).ConfigureAwait(false);
                    
                    // ENHANCED LOGGING: Show strategy execution result
                    _logger?.Error("🔍 **DB_UPDATE_STRATEGY_RESULT**: FieldName={FieldName} | StrategyType={StrategyType} | IsSuccess={IsSuccess} | Message={Message}",
                        request.FieldName, strategy.StrategyType, dbUpdateResult.IsSuccess, dbUpdateResult.Message ?? "NULL");

                    // 4. Process the result of the strategy execution
                    if (dbUpdateResult.IsSuccess)
                    {
                        dbSuccessCount++;
                        if (strategy is OmissionUpdateStrategy) omissionPatternUpdates++;
                        else if (strategy is FieldFormatUpdateStrategy) formatPatternUpdates++;

                        _logger?.Information("Successfully updated database for field {FieldName} using {StrategyType}: {OperationDetails}",
                            request.FieldName, strategy.StrategyType, dbUpdateResult.Message);
                    }
                    else
                    {
                        dbFailureCount++;
                        _logger?.Warning("Database update failed for field {FieldName} using {StrategyType}: {ErrorMessage}",
                            request.FieldName, strategy.StrategyType, dbUpdateResult.Message);
                    }

                    // 5. Log to learning table
                    await this.LogCorrectionLearningAsync(context, request, dbUpdateResult).ConfigureAwait(false);
                }
                catch (Exception ex)
                {
                    dbFailureCount++;
                    _logger?.Error(ex, "Exception during database update processing for field {FieldName}. Strategy: {StrategyType}",
                        request.FieldName, strategy?.StrategyType ?? "Unknown");

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
        /// Applies corrections to the database based on corrected ShipmentInvoice data.
        /// This method handles the final step of persisting corrections to the EntryDataDS context.
        /// </summary>
        /// <param name="correctedInvoices">The corrected ShipmentInvoice entities to persist</param>
        /// <param name="logger">Logger for tracking the operation</param>
        /// <returns>True if database updates were successful</returns>
        public async Task<bool> ApplyToDatabaseInternal(List<ShipmentInvoice> correctedInvoices, ILogger logger = null)
        {
            var log = logger ?? _logger ?? Serilog.Log.Logger.ForContext<OCRCorrectionService>();
            
            if (correctedInvoices == null || !correctedInvoices.Any())
            {
                log.Information("ApplyToDatabaseInternal: No corrected invoices provided for database update.");
                return true; // No work to do, consider successful
            }

            log.Information("Starting database persistence for {InvoiceCount} corrected invoices.", correctedInvoices.Count);

            try
            {
                using var entryContext = new global::EntryDataDS.Business.Entities.EntryDataDSContext();
                
                int updatedCount = 0;
                int errorCount = 0;

                foreach (var correctedInvoice in correctedInvoices)
                {
                    try
                    {
                        // Find the existing invoice in the database
                        var existingInvoice = await entryContext.ShipmentInvoice
                            .FirstOrDefaultAsync(si => si.InvoiceNo == correctedInvoice.InvoiceNo)
                            .ConfigureAwait(false);

                        if (existingInvoice != null)
                        {
                            // Update the existing invoice with corrected values
                            existingInvoice.InvoiceTotal = correctedInvoice.InvoiceTotal;
                            existingInvoice.SubTotal = correctedInvoice.SubTotal;
                            existingInvoice.TotalInternalFreight = correctedInvoice.TotalInternalFreight;
                            existingInvoice.TotalOtherCost = correctedInvoice.TotalOtherCost;
                            existingInvoice.TotalInsurance = correctedInvoice.TotalInsurance;
                            existingInvoice.TotalDeduction = correctedInvoice.TotalDeduction;
                            existingInvoice.Currency = correctedInvoice.Currency;
                            existingInvoice.SupplierName = correctedInvoice.SupplierName;
                            existingInvoice.SupplierAddress = correctedInvoice.SupplierAddress;
                            existingInvoice.InvoiceDate = correctedInvoice.InvoiceDate;

                            // Mark as modified for Entity Framework tracking
                            entryContext.Entry(existingInvoice).State = System.Data.Entity.EntityState.Modified;
                            updatedCount++;

                            log.Debug("Marked invoice {InvoiceNo} for database update with corrected values.", correctedInvoice.InvoiceNo);
                        }
                        else
                        {
                            log.Warning("Could not find existing invoice {InvoiceNo} in database for update.", correctedInvoice.InvoiceNo);
                            errorCount++;
                        }
                    }
                    catch (Exception ex)
                    {
                        log.Error(ex, "Error preparing invoice {InvoiceNo} for database update.", correctedInvoice.InvoiceNo);
                        errorCount++;
                    }
                }

                if (updatedCount > 0)
                {
                    // Save all changes to the database
                    int savedChanges = await entryContext.SaveChangesAsync().ConfigureAwait(false);
                    log.Information("Successfully applied {UpdatedCount} invoice corrections to database. {SavedChanges} records saved.", 
                        updatedCount, savedChanges);
                }

                bool success = updatedCount > 0 && errorCount == 0;
                log.Information("Database update completed: {UpdatedCount} updated, {ErrorCount} errors. Success: {Success}", 
                    updatedCount, errorCount, success);

                return success;
            }
            catch (Exception ex)
            {
                log.Error(ex, "Exception during database update operation for corrected invoices.");
                return false;
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
                    
                    // PHASE 5: Check for Amazon-specific patterns first
                    var amazonPattern = GetAmazonSpecificPattern(correction.FieldName, lineContext.WindowText ?? "");
                    if (!string.IsNullOrEmpty(amazonPattern))
                    {
                        _logger.Error("🎯 **AMAZON_PATTERN_APPLIED**: Using pre-defined Amazon pattern for {FieldName}", correction.FieldName);
                        correction.SuggestedRegex = amazonPattern;
                        correction.RequiresMultilineRegex = false;
                        correction.Confidence = 95; // High confidence for pre-tested patterns
                        correction.Reasoning = $"Pre-defined Amazon-specific pattern for {correction.FieldName}";
                        
                        _logger.Information("✅ **PIPELINE_AMAZON_PATTERN_SUCCESS**: Applied Amazon pattern for {FieldName}: {Pattern}", 
                            correction.FieldName, amazonPattern);
                        
                        return correction;
                    }
                    
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
            // COMPREHENSIVE VALIDATION LOGGING: Use LogLevelOverride to focus on validation logic
            using (LogLevelOverride.Begin(LogEventLevel.Verbose))
            {
                _logger.Error("🔍 **VALIDATION_SECTION_ENTRY**: ValidatePatternInternal called for validation analysis");
                
                if (correction == null)
                {
                    _logger.Error("❌ **VALIDATION_NULL_INPUT**: Correction parameter is null - cannot validate");
                    return null;
                }

                // LOG COMPLETE CORRECTION DATA FOR ANALYSIS
                _logger.Error("🔍 **VALIDATION_INPUT_DATA**: Complete correction object analysis");
                _logger.Error("🔍 **VALIDATION_FIELD_NAME**: FieldName = '{FieldName}' (Length: {Length})", 
                    correction.FieldName ?? "NULL", correction.FieldName?.Length ?? 0);
                _logger.Error("🔍 **VALIDATION_OLD_VALUE**: OldValue = '{OldValue}' (Type: {Type})", 
                    correction.OldValue ?? "NULL", correction.OldValue?.GetType().Name ?? "NULL");
                _logger.Error("🔍 **VALIDATION_NEW_VALUE**: NewValue = '{NewValue}' (Type: {Type}, Length: {Length})", 
                    correction.NewValue ?? "NULL", correction.NewValue?.GetType().Name ?? "NULL", correction.NewValue?.Length ?? 0);
                _logger.Error("🔍 **VALIDATION_CORRECTION_TYPE**: CorrectionType = '{CorrectionType}'", 
                    correction.CorrectionType ?? "NULL");
                _logger.Error("🔍 **VALIDATION_SUGGESTED_REGEX**: SuggestedRegex = '{SuggestedRegex}' (Length: {Length})", 
                    correction.SuggestedRegex ?? "NULL", correction.SuggestedRegex?.Length ?? 0);
                _logger.Error("🔍 **VALIDATION_SUCCESS_STATE**: Current Success = {Success}", correction.Success);
                _logger.Error("🔍 **VALIDATION_CONFIDENCE**: Confidence = {Confidence}", correction.Confidence);
                _logger.Error("🔍 **VALIDATION_REASONING**: Current Reasoning = '{Reasoning}'", 
                    correction.Reasoning ?? "NULL");

                _logger.Error("🔍 **VALIDATION_STEP_1_START**: Checking field support for '{FieldName}'", correction.FieldName);

                try
                {
                    // STEP 1: Validate field is supported with detailed logging
                    bool fieldSupported = this.IsFieldSupported(correction.FieldName);
                    _logger.Error("🔍 **VALIDATION_FIELD_SUPPORT_RESULT**: IsFieldSupported('{FieldName}') = {IsSupported}", 
                        correction.FieldName, fieldSupported);
                    
                    if (!fieldSupported)
                    {
                        _logger.Error("❌ **VALIDATION_STEP_1_FAILED**: Field '{FieldName}' is not supported for database updates", correction.FieldName);
                        _logger.Error("🔍 **VALIDATION_SUPPORTED_FIELDS**: Available supported fields: {SupportedFields}", 
                            string.Join(", ", this.GetSupportedMappedFields() ?? new List<string>()));
                        
                        correction.Success = false;
                        correction.Reasoning = $"Field '{correction.FieldName}' is not supported for database updates";
                        _logger.Error("🔍 **VALIDATION_SECTION_EXIT**: Exiting due to unsupported field");
                        return correction;
                    }
                    _logger.Error("✅ **VALIDATION_STEP_1_PASSED**: Field '{FieldName}' is supported", correction.FieldName);

                    // STEP 2: Validate new value if present with detailed field info analysis
                    _logger.Error("🔍 **VALIDATION_STEP_2_START**: Validating new value if present");
                    if (!string.IsNullOrEmpty(correction.NewValue))
                    {
                        _logger.Error("🔍 **VALIDATION_NEW_VALUE_PRESENT**: NewValue is present, getting field validation info");
                        
                        var fieldInfo = this.GetFieldValidationInfo(correction.FieldName);
                        _logger.Error("🔍 **VALIDATION_FIELD_INFO**: Field validation info retrieved");
                        _logger.Error("🔍 **VALIDATION_FIELD_INFO_DETAILS**: IsValid={IsValid} | DatabaseFieldName='{DbFieldName}' | EntityType='{EntityType}' | DataType='{DataType}' | IsRequired={IsRequired} | IsMonetary={IsMonetary}", 
                            fieldInfo.IsValid, 
                            fieldInfo.DatabaseFieldName ?? "NULL", 
                            fieldInfo.EntityType ?? "NULL", 
                            fieldInfo.DataType ?? "NULL", 
                            fieldInfo.IsRequired, 
                            fieldInfo.IsMonetary);
                        _logger.Error("🔍 **VALIDATION_FIELD_PATTERN**: ValidationPattern = '{Pattern}' (Length: {Length})", 
                            fieldInfo.ValidationPattern ?? "NULL", fieldInfo.ValidationPattern?.Length ?? 0);
                        _logger.Error("🔍 **VALIDATION_FIELD_MAX_LENGTH**: MaxLength = {MaxLength}", fieldInfo.MaxLength?.ToString() ?? "NULL");
                        _logger.Error("🔍 **VALIDATION_FIELD_ERROR**: ErrorMessage = '{ErrorMessage}'", fieldInfo.ErrorMessage ?? "NULL");
                        
                        if (!string.IsNullOrEmpty(fieldInfo.ValidationPattern))
                        {
                            _logger.Error("🔍 **VALIDATION_PATTERN_MATCHING_START**: Testing NewValue '{NewValue}' against pattern '{Pattern}'", 
                                correction.NewValue, fieldInfo.ValidationPattern);
                            
                            try
                            {
                                bool patternMatches = Regex.IsMatch(correction.NewValue, fieldInfo.ValidationPattern);
                                _logger.Error("🔍 **VALIDATION_PATTERN_MATCH_RESULT**: Regex.IsMatch('{Value}', '{Pattern}') = {MatchResult}", 
                                    correction.NewValue, fieldInfo.ValidationPattern, patternMatches);
                                
                                if (!patternMatches)
                                {
                                    _logger.Error("❌ **VALIDATION_STEP_2_FAILED**: Value '{Value}' for {FieldName} doesn't match pattern {Pattern}", 
                                        correction.NewValue, correction.FieldName, fieldInfo.ValidationPattern);
                                    
                                    // TEST WHAT WOULD MATCH THE PATTERN
                                    _logger.Error("🔍 **VALIDATION_PATTERN_ANALYSIS**: Analyzing what would match pattern '{Pattern}'", fieldInfo.ValidationPattern);
                                    var testValues = new[] { "6.99", "$6.99", "-6.99", "-$6.99", "123.45", "$123.45" };
                                    foreach (var testValue in testValues)
                                    {
                                        try
                                        {
                                            bool testMatches = Regex.IsMatch(testValue, fieldInfo.ValidationPattern);
                                            _logger.Error("🔍 **VALIDATION_PATTERN_TEST**: TestValue '{TestValue}' matches pattern = {TestResult}", 
                                                testValue, testMatches);
                                        }
                                        catch (Exception testEx)
                                        {
                                            _logger.Error("🔍 **VALIDATION_PATTERN_TEST_ERROR**: TestValue '{TestValue}' caused error: {Error}", 
                                                testValue, testEx.Message);
                                        }
                                    }
                                    
                                    correction.Success = false;
                                    correction.Reasoning = $"Value '{correction.NewValue}' doesn't match expected pattern for field type '{fieldInfo.DataType}'";
                                    _logger.Error("🔍 **VALIDATION_SECTION_EXIT**: Exiting due to pattern mismatch");
                                    return correction;
                                }
                                _logger.Error("✅ **VALIDATION_STEP_2_PASSED**: Value '{Value}' matches validation pattern", correction.NewValue);
                            }
                            catch (ArgumentException ex)
                            {
                                _logger.Error("⚠️ **VALIDATION_PATTERN_REGEX_ERROR**: Invalid validation pattern for {FieldName}: {Error}", 
                                    correction.FieldName, ex.Message);
                                _logger.Error("🔍 **VALIDATION_PATTERN_REGEX_DETAILS**: Pattern='{Pattern}' | Exception={Exception}", 
                                    fieldInfo.ValidationPattern, ex.ToString());
                                // Continue validation despite pattern error
                            }
                        }
                        else
                        {
                            _logger.Error("🔍 **VALIDATION_NO_PATTERN**: No validation pattern defined for field '{FieldName}' - skipping pattern validation", correction.FieldName);
                        }
                    }
                    else
                    {
                        _logger.Error("🔍 **VALIDATION_NO_NEW_VALUE**: NewValue is null or empty - skipping value validation");
                    }

                    // STEP 3: Validate suggested regex if present with comprehensive syntax testing
                    _logger.Error("🔍 **VALIDATION_STEP_3_START**: Validating suggested regex if present");
                    if (!string.IsNullOrEmpty(correction.SuggestedRegex))
                    {
                        _logger.Error("🔍 **VALIDATION_SUGGESTED_REGEX_PRESENT**: SuggestedRegex is present, testing compilation");
                        _logger.Error("🔍 **VALIDATION_REGEX_PATTERN**: Full pattern = '{Pattern}'", correction.SuggestedRegex);
                        
                        try
                        {
                            var testRegex = new Regex(correction.SuggestedRegex, RegexOptions.IgnoreCase | RegexOptions.Multiline);
                            _logger.Error("✅ **VALIDATION_REGEX_COMPILATION_SUCCESS**: Suggested regex compiled successfully");
                            _logger.Error("🔍 **VALIDATION_REGEX_OPTIONS**: Compiled with options: IgnoreCase={IgnoreCase}, Multiline={Multiline}", 
                                testRegex.Options.HasFlag(RegexOptions.IgnoreCase), 
                                testRegex.Options.HasFlag(RegexOptions.Multiline));
                            
                            // TEST REGEX WITH SAMPLE DATA
                            if (!string.IsNullOrEmpty(correction.NewValue))
                            {
                                _logger.Error("🔍 **VALIDATION_REGEX_TEST_START**: Testing compiled regex against NewValue");
                                try
                                {
                                    bool regexMatches = testRegex.IsMatch(correction.NewValue);
                                    _logger.Error("🔍 **VALIDATION_REGEX_TEST_RESULT**: Regex.IsMatch('{Value}') = {MatchResult}", 
                                        correction.NewValue, regexMatches);
                                    
                                    if (regexMatches)
                                    {
                                        var match = testRegex.Match(correction.NewValue);
                                        _logger.Error("🔍 **VALIDATION_REGEX_MATCH_DETAILS**: Match.Success={Success} | Match.Value='{MatchValue}' | Groups.Count={GroupCount}", 
                                            match.Success, match.Value, match.Groups.Count);
                                        
                                        foreach (Group group in match.Groups)
                                        {
                                            _logger.Error("🔍 **VALIDATION_REGEX_GROUP**: Group[{Index}] = '{Value}' (Name: {Name})", 
                                                group.Index, group.Value, group.Name);
                                        }
                                    }
                                }
                                catch (Exception regexTestEx)
                                {
                                    _logger.Error("⚠️ **VALIDATION_REGEX_TEST_ERROR**: Error testing regex against value: {Error}", regexTestEx.Message);
                                }
                            }
                            
                            _logger.Error("✅ **VALIDATION_STEP_3_PASSED**: Suggested regex for {FieldName} is syntactically valid", correction.FieldName);
                        }
                        catch (ArgumentException ex)
                        {
                            _logger.Error("❌ **VALIDATION_STEP_3_FAILED**: Invalid suggested regex for {FieldName}: {Error}", 
                                correction.FieldName, ex.Message);
                            _logger.Error("🔍 **VALIDATION_REGEX_ERROR_DETAILS**: Pattern='{Pattern}' | Exception={Exception}", 
                                correction.SuggestedRegex, ex.ToString());
                            
                            correction.Success = false;
                            correction.Reasoning = $"Invalid regex pattern: {ex.Message}";
                            _logger.Error("🔍 **VALIDATION_SECTION_EXIT**: Exiting due to invalid regex");
                            return correction;
                        }
                    }
                    else
                    {
                        _logger.Error("🔍 **VALIDATION_NO_SUGGESTED_REGEX**: SuggestedRegex is null or empty - skipping regex validation");
                    }

                    _logger.Error("✅ **VALIDATION_ALL_STEPS_PASSED**: All validation steps completed successfully for field {FieldName}", correction.FieldName);
                    _logger.Error("🔍 **VALIDATION_FINAL_STATE**: Final Success={Success} | Final Reasoning='{Reasoning}'", 
                        correction.Success, correction.Reasoning ?? "NULL");
                    _logger.Error("🔍 **VALIDATION_SECTION_EXIT**: ValidatePatternInternal completed successfully");
                    
                    return correction;
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "🚨 **VALIDATION_EXCEPTION**: Exception during pattern validation for {FieldName}", correction.FieldName);
                    _logger.Error("🔍 **VALIDATION_EXCEPTION_DETAILS**: ExceptionType={Type} | Message={Message} | StackTrace={StackTrace}", 
                        ex.GetType().Name, ex.Message, ex.StackTrace);
                    
                    correction.Success = false;
                    correction.Reasoning = $"Exception during validation: {ex.Message}";
                    _logger.Error("🔍 **VALIDATION_SECTION_EXIT**: Exiting due to exception");
                    return correction;
                }
            }
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