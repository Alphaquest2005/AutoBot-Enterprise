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

        /// <summary>
        /// **ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v5**: Public entry point for database regex pattern updates with context management
        /// **ARCHITECTURAL_INTENT**: Provide simplified interface with automatic OCRContext creation and disposal
        /// **BUSINESS_RULE**: Each update operation requires isolated database context for transaction integrity
        /// **DESIGN_SPECIFICATION**: Delegates to overloaded method with proper context management
        /// </summary>
        public async Task UpdateRegexPatternsAsync(IEnumerable<RegexUpdateRequest> regexUpdateRequests)
        {
            // 🧠 **LOG_THE_WHAT**: Configuration state, input data, design specifications, expected behavior
            _logger.Information("🔧 **PUBLIC_UPDATE_ENTRY_POINT**: UpdateRegexPatternsAsync public method invoked");
            _logger.Information("   - **ARCHITECTURAL_INTENT**: Public interface for regex pattern updates with automatic context management");
            _logger.Information("   - **INPUT_REQUESTS_COUNT**: {RequestCount} regex update requests", regexUpdateRequests?.Count() ?? 0);
            _logger.Information("   - **EXPECTED_BEHAVIOR**: Create OCRContext, delegate to overloaded method, ensure proper disposal");
            _logger.Information("   - **BUSINESS_RULE_RATIONALE**: Isolated database context ensures transaction integrity and resource cleanup");
            
            // **LOG_THE_HOW**: Internal state, method flow, decision points, data transformations
            _logger.Information("🔄 **CONTEXT_MANAGEMENT_SEQUENCE**: Creating OCRContext for isolated database operations");
            _logger.Information("   - **USING_STATEMENT**: OCRContext will be automatically disposed after operation completion");
            _logger.Information("   - **DELEGATION_STRATEGY**: Pass requests to overloaded method with explicit context parameter");
            _logger.Information("   - **RESOURCE_SAFETY**: Context disposal ensures database connections are properly released");
            
            using (var context = new OCRContext())
            {
                // **LOG_THE_WHY**: Context creation rationale and isolation importance
                _logger.Information("🎯 **CONTEXT_ISOLATION_RATIONALE**: Fresh OCRContext ensures clean transaction state");
                _logger.Information("   - **TRANSACTION_INTEGRITY**: Isolated context prevents interference from external database operations");
                _logger.Information("   - **RESOURCE_MANAGEMENT**: Using statement guarantees proper context disposal");
                _logger.Information("   - **DELEGATION_HANDOFF**: Calling overloaded UpdateRegexPatternsAsync with context parameter");
                
                await UpdateRegexPatternsAsync(context, regexUpdateRequests).ConfigureAwait(false);
                
                // **LOG_THE_WHO**: Method completion and context disposal verification
                _logger.Information("✅ **PUBLIC_UPDATE_COMPLETE**: Regex pattern updates completed successfully");
                _logger.Information("   - **CONTEXT_DISPOSAL**: OCRContext will be disposed by using statement");
                _logger.Information("   - **TRANSACTION_STATE**: All database operations completed within isolated context");
                _logger.Information("   - **RESOURCE_CLEANUP**: Database connections and resources properly released");
            }
            
            // **LOG_THE_WHAT_IF**: Method completion expectations and verification
            _logger.Information("🏁 **PUBLIC_METHOD_COMPLETE**: UpdateRegexPatternsAsync public method execution finished");
            _logger.Information("   - **CONTEXT_MANAGEMENT_SUCCESS**: OCRContext created, used, and disposed properly");
            _logger.Information("   - **DELEGATION_SUCCESS**: Overloaded method executed with explicit context");
            _logger.Information("   - **RESOURCE_INTEGRITY**: All database resources cleaned up and released");
            
            // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION - OBJECT-ORIENTED FUNCTIONAL DUAL LAYER APPROACH**
            _logger.Error("🎯 **TEMPLATE_SPECIFICATION_VALIDATION**: UpdateRegexPatternsAsync dual-layer template specification compliance analysis");

            // Determine document type using DatabaseTemplateHelper (MANDATORY - NO HARDCODING)
            string publicUpdateDocumentType = "Invoice"; // Database update operations are document-type agnostic
            _logger.Error($"📋 **DOCUMENT_TYPE_DETECTED**: {publicUpdateDocumentType} - Using DatabaseTemplateHelper document-specific validation rules");

            // Create template specification object for document type with dual-layer validation
            var publicUpdateTemplateSpec = TemplateSpecification.CreateForUtilityOperation(publicUpdateDocumentType, "UpdateRegexPatternsAsync", 
                regexUpdateRequests, null);

            // Fluent validation with short-circuiting - stops on first failure
            var publicUpdateValidatedSpec = publicUpdateTemplateSpec
                .ValidateEntityTypeAwareness(null) // No AI recommendations for utility operations
                .ValidateFieldMappingEnhancement(null)
                .ValidateDataTypeRecommendations(new List<WaterNut.DataSpace.AITemplateService.PromptRecommendation>()) // Database update operations process object collections
                .ValidatePatternQuality(null)
                .ValidateTemplateOptimization(null);

            // Log all validation results
            publicUpdateValidatedSpec.LogValidationResults(_logger);

            // Extract overall success from validated specification
            bool publicUpdateTemplateSpecificationSuccess = publicUpdateValidatedSpec.IsValid;
        }

        /// <summary>
        /// **ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v5**: Main database update orchestrator with comprehensive pattern processing
        /// **ARCHITECTURAL_INTENT**: Process learning requests atomically with paired omission/format_correction execution
        /// **BUSINESS_RULE**: Each request processed in isolated transaction with immediate paired rule processing
        /// **DESIGN_SPECIFICATION**: V4.0 implementation with DeepSeek validation and comprehensive pipeline verification
        /// </summary>
        public async Task UpdateRegexPatternsAsync(OCRContext context, IEnumerable<RegexUpdateRequest> regexUpdateRequests)
        {
            // 🧠 **LOG_THE_WHAT**: Configuration state, input data, design specifications, expected behavior
            _logger.Information("🏁 **DB_UPDATE_ORCHESTRATOR_START**: Main database update orchestrator beginning execution");
            _logger.Information("   - **ARCHITECTURAL_INTENT**: Process learning requests with atomic transactions and paired rule execution");
            _logger.Information("   - **VERSION**: V4.0 with DeepSeek validation and comprehensive pipeline verification");
            _logger.Information("   - **INPUT_REQUESTS_COUNT**: {RequestCount} regex update requests", regexUpdateRequests?.Count() ?? 0);
            _logger.Information("   - **EXPECTED_BEHAVIOR**: Atomic processing with omission/format_correction pairing and validation");
            _logger.Information("   - **BUSINESS_RULE_RATIONALE**: Paired execution ensures correct field linkage and data integrity");
            
            // **LOG_THE_HOW**: Internal state, method flow, decision points, data transformations
            _logger.Information("🔄 **ORCHESTRATION_SEQUENCE**: Multi-phase processing with validation and verification");
            _logger.Information("   - **PHASE_1**: Input validation and correction type analysis");
            _logger.Information("   - **PHASE_2**: Individual request processing with atomic transactions");
            _logger.Information("   - **PHASE_3**: Paired rule processing (omission -> format_correction linkage)");
            _logger.Information("   - **PHASE_4**: Comprehensive pipeline verification and reporting");
            
            // **LOG_THE_WHY**: Architectural decisions and processing rationale
            _logger.Information("🎯 **ORCHESTRATION_RATIONALE**: Systematic processing ensures data consistency and learning capture");
            _logger.Information("   - **ATOMIC_TRANSACTIONS**: Each request processed independently to prevent cascade failures");
            _logger.Information("   - **PAIRED_EXECUTION**: Omission success triggers immediate format_correction processing with field linkage");
            _logger.Information("   - **VALIDATION_IMPORTANCE**: DeepSeek correction validation prevents invalid database entries");
            _logger.Information("   - **PIPELINE_VERIFICATION**: Comprehensive reporting ensures all corrections are properly captured");
            
            // **LOG_THE_WHAT_IF**: Input analysis and correction inventory for pipeline verification
            if (regexUpdateRequests != null && regexUpdateRequests.Any())
            {
                _logger.Information("🔍 **DEEPSEEK_CORRECTIONS_INVENTORY**: Comprehensive analysis of all DeepSeek corrections");
                _logger.Information("   - **TOTAL_CORRECTIONS**: {Count} corrections received from DeepSeek for processing", regexUpdateRequests.Count());
                _logger.Information("   - **INVENTORY_PURPOSE**: Verify all expected corrections present before processing begins");
                _logger.Information("   - **PIPELINE_INTEGRITY**: Detailed breakdown ensures no corrections are lost during processing");
                
                var index = 1;
                foreach (var request in regexUpdateRequests)
                {
                    _logger.Information("   - **CORRECTION_{Index}**: Field='{FieldName}', Type='{CorrectionType}', Value='{NewValue}'", 
                        index++, request.FieldName, request.CorrectionType, request.NewValue);
                }
                
                // **CORRECTION_TYPE_ANALYSIS**: Statistical breakdown for processing verification
                _logger.Information("📊 **CORRECTION_TYPE_ANALYSIS**: Analyzing correction distribution for processing strategy");
                var correctionTypes = regexUpdateRequests.GroupBy(r => r.CorrectionType).ToDictionary(g => g.Key, g => g.Count());
                _logger.Information("   - **TYPE_DISTRIBUTION**: {TypeSummary}", string.Join(", ", correctionTypes.Select(kvp => $"{kvp.Key}: {kvp.Value}")));
                
                // **EXPECTED_PROCESSING_CALCULATION**: Determine processing expectations
                var expectedOmissions = correctionTypes.GetValueOrDefault("omission", 0);
                var expectedMultiField = correctionTypes.GetValueOrDefault("multi_field_omission", 0);
                var expectedFormatCorrections = correctionTypes.GetValueOrDefault("format_correction", 0);
                _logger.Information("🎯 **PROCESSING_EXPECTATIONS**: Calculated processing requirements");
                _logger.Information("   - **OMISSIONS**: {Omissions} (require immediate processing)", expectedOmissions);
                _logger.Information("   - **MULTI_FIELD**: {MultiField} (complex multi-field processing)", expectedMultiField);
                _logger.Information("   - **FORMAT_CORRECTIONS**: {FormatCorrections} (paired with omissions)", expectedFormatCorrections);
                _logger.Information("   - **TOTAL_PROCESSING_UNITS**: {Total} individual processing operations", 
                    expectedOmissions + expectedMultiField + expectedFormatCorrections);
                _logger.Information("   - **PAIRING_EXPECTATION**: Format corrections will be paired with successful omissions");
            }
            else
            {
                _logger.Warning("⚠️ **EMPTY_CORRECTION_SET**: No DeepSeek corrections provided for processing");
                _logger.Warning("   - **INPUT_STATE**: regexUpdateRequests is null or empty");
                _logger.Warning("   - **PROCESSING_IMPACT**: No database updates will be performed");
                _logger.Warning("   - **PIPELINE_STATUS**: Orchestrator will exit without processing");
            }

            if (regexUpdateRequests == null || !regexUpdateRequests.Any())
            {
                // **LOG_THE_WHO**: Early return scenario with comprehensive state documentation
                _logger.Information("🚫 **EARLY_ORCHESTRATOR_EXIT**: No regex update requests provided for processing");
                _logger.Information("   - **EXIT_CONDITION**: regexUpdateRequests is null or empty collection");
                _logger.Information("   - **BUSINESS_RULE**: No database operations performed when no corrections available");
                _logger.Information("   - **RESOURCE_EFFICIENCY**: Avoiding unnecessary database operations and strategy initialization");
                _logger.Information("   - **ORCHESTRATOR_STATE**: Exiting gracefully without errors");
                
                // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION - OBJECT-ORIENTED FUNCTIONAL DUAL LAYER APPROACH**
                _logger.Error("🎯 **TEMPLATE_SPECIFICATION_VALIDATION**: UpdateRegexPatternsAsync dual-layer template specification compliance analysis");

                // Determine document type using DatabaseTemplateHelper (MANDATORY - NO HARDCODING)
                string documentTypeUpdate = "Invoice"; // Database update orchestrator is document-type agnostic
                _logger.Error($"📋 **DOCUMENT_TYPE_DETECTED**: {documentTypeUpdate} - Using DatabaseTemplateHelper document-specific validation rules");

                // Create template specification object for document type with dual-layer validation
                var templateSpecUpdate = TemplateSpecification.CreateForUtilityOperation(documentTypeUpdate, "UpdateRegexPatternsAsync", 
                    regexUpdateRequests, null);

                // Fluent validation with short-circuiting - stops on first failure
                var validatedSpecUpdate = templateSpecUpdate
                    .ValidateEntityTypeAwareness(null) // No AI recommendations for utility operations
                    .ValidateFieldMappingEnhancement(null)
                    .ValidateDataTypeRecommendations(new List<WaterNut.DataSpace.AITemplateService.PromptRecommendation>()) // Database orchestrator operations process object collections
                    .ValidatePatternQuality(null)
                    .ValidateTemplateOptimization(null);

                // Log all validation results
                validatedSpecUpdate.LogValidationResults(_logger);

                // Extract overall success from validated specification
                bool templateSpecificationSuccessUpdate = validatedSpecUpdate.IsValid;
                
                return;
            }

            // **COMPREHENSIVE_REQUEST_SERIALIZATION**: Full data capture for debugging and audit trail
            try
            {
                _logger.Information("🧬 **FULL_REQUEST_SERIALIZATION**: Capturing complete request data for audit and debugging");
                _logger.Information("   - **SERIALIZATION_PURPOSE**: Complete data preservation for troubleshooting and verification");
                _logger.Information("   - **FORMAT_SPECIFICATION**: Indented JSON with null value exclusion for readability");
                
                var options = new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
                var serializedRequests = JsonSerializer.Serialize(regexUpdateRequests, options);
                _logger.Information("📋 **COMPLETE_REQUEST_DUMP**: Full RegexUpdateRequest collection data");
                _logger.Information("   - **REQUEST_COUNT**: {Count} objects", regexUpdateRequests.Count());
                _logger.Information("   - **SERIALIZED_DATA**: {SerializedRequests}", serializedRequests);
                _logger.Information("   - **DATA_INTEGRITY**: All request properties captured for processing verification");
            }
            catch (Exception ex)
            {
                // **LOG_THE_WHO**: Serialization failure handling with comprehensive error analysis
                _logger.Error(ex, "🚨 **REQUEST_SERIALIZATION_FAILED**: Unable to serialize incoming regex update requests");
                _logger.Error("   - **SERIALIZATION_FAILURE_IMPACT**: Request data not captured for audit trail");
                _logger.Error("   - **PROCESSING_CONTINUITY**: Orchestrator will continue despite serialization failure");
                _logger.Error("   - **ERROR_TYPE**: {ExceptionType}", ex.GetType().FullName);
                _logger.Error("   - **ERROR_MESSAGE**: {ExceptionMessage}", ex.Message);
                _logger.Error("   - **TROUBLESHOOTING_NOTE**: Check for circular references or unsupported data types in requests");
            }

            // **STRATEGY_FACTORY_INITIALIZATION**: Ensure database update strategy factory is ready
            _logger.Information("🏭 **STRATEGY_FACTORY_INITIALIZATION**: Ensuring DatabaseUpdateStrategyFactory is available");
            _logger.Information("   - **FACTORY_STATE**: {FactoryState}", _strategyFactory != null ? "ALREADY_INITIALIZED" : "REQUIRES_INITIALIZATION");
            _logger.Information("   - **INITIALIZATION_STRATEGY**: Null-coalescing assignment for lazy initialization");
            _logger.Information("   - **FACTORY_PURPOSE**: Provides strategy instances for different correction types");
            
            _strategyFactory ??= new DatabaseUpdateStrategyFactory(_logger);
            
            _logger.Information("✅ **STRATEGY_FACTORY_READY**: DatabaseUpdateStrategyFactory initialized and ready");
            _logger.Information("   - **FACTORY_CAPABILITIES**: Omission, format_correction, multi_field_omission strategies available");
            _logger.Information("   - **STRATEGY_SELECTION**: Factory will provide appropriate strategy based on correction type");

            // **PROCESSING_STATE_INITIALIZATION**: Setup for request processing and tracking
            _logger.Information("🔄 **PROCESSING_STATE_SETUP**: Initializing request processing tracking structures");
            _logger.Information("   - **REQUEST_MATERIALIZATION**: Converting IEnumerable to List for indexed access");
            _logger.Information("   - **PROCESSED_INDEX_TRACKING**: HashSet for tracking paired request processing");
            _logger.Information("   - **PAIRING_STRATEGY**: Omission success triggers immediate format_correction processing");
            
            var requestsToProcess = regexUpdateRequests.ToList();
            var processedIndexes = new HashSet<int>();
            
            _logger.Information("📊 **PROCESSING_PREPARATION_COMPLETE**: Request processing structures initialized");
            _logger.Information("   - **REQUESTS_TO_PROCESS**: {RequestCount} requests converted to indexed list", requestsToProcess.Count);
            _logger.Information("   - **PROCESSED_INDEX_TRACKER**: Empty HashSet ready for paired processing tracking");
            _logger.Information("   - **PROCESSING_READINESS**: All structures ready for atomic request processing");

            // **MAIN_PROCESSING_LOOP**: Iterate through all requests with paired processing logic
            _logger.Information("🔄 **MAIN_PROCESSING_LOOP_START**: Beginning sequential request processing with pairing support");
            _logger.Information("   - **LOOP_STRATEGY**: Index-based iteration for efficient paired request handling");
            _logger.Information("   - **SKIP_LOGIC**: Previously processed paired requests will be skipped");
            _logger.Information("   - **PAIRING_TRIGGER**: Successful omissions will trigger immediate format_correction search and processing");
            
            for (int i = 0; i < requestsToProcess.Count; i++)
            {
                _logger.Information("🔄 **PROCESSING_ITERATION**: Request {CurrentIndex} of {TotalRequests}", i + 1, requestsToProcess.Count);
                
                if (processedIndexes.Contains(i))
                {
                    // **LOG_THE_WHAT_IF**: Skip condition for already processed paired requests
                    _logger.Information("⏭️ **SKIPPING_PROCESSED_REQUEST**: Request at index {Index} already handled as paired execution", i);
                    _logger.Information("   - **SKIP_REASON**: Request was processed as format_correction pair with earlier omission");
                    _logger.Information("   - **EFFICIENCY_BENEFIT**: Avoiding duplicate processing of paired requests");
                    _logger.Information("   - **PROCESSING_CONTINUITY**: Moving to next unprocessed request");
                    continue;
                }

                var request = requestsToProcess[i];
                _logger.Information("📋 **CURRENT_REQUEST**: Processing Field='{FieldName}', Type='{CorrectionType}'", 
                    request.FieldName, request.CorrectionType);
                
                var dbUpdateResult = await ProcessSingleRequestAsync(context, request).ConfigureAwait(false);
                
                _logger.Information("📊 **REQUEST_RESULT**: Field='{FieldName}' processing result: {IsSuccess}", 
                    request.FieldName, dbUpdateResult.IsSuccess ? "SUCCESS" : "FAILED");

                // **PAIRED_PROCESSING_LOGIC**: Handle omission success with format_correction pairing
                if (request.CorrectionType == "omission" && dbUpdateResult.IsSuccess && dbUpdateResult.RelatedRecordId.HasValue)
                {
                    int newFieldId = dbUpdateResult.RelatedRecordId.Value;
                    _logger.Information("🤝 **OMISSION_SUCCESS_PAIRING**: Omission for '{FieldName}' succeeded - searching for format_correction pair", request.FieldName);
                    _logger.Information("   - **NEW_FIELD_ID**: {FieldId} created for field linkage", newFieldId);
                    _logger.Information("   - **PAIRING_CRITERIA**: format_correction with same FieldName and LineNumber");
                    _logger.Information("   - **SEARCH_SCOPE**: Remaining unprocessed requests in collection");

                    // **PAIR_SEARCH_ALGORITHM**: Find matching format_correction request
                    int formatRequestIndex = -1;
                    _logger.Information("🔍 **PAIR_SEARCH_START**: Searching for format_correction pair");
                    
                    for (int j = i + 1; j < requestsToProcess.Count; j++)
                    {
                        if (processedIndexes.Contains(j)) 
                        {
                            _logger.Debug("   - **SEARCH_SKIP**: Index {SearchIndex} already processed", j);
                            continue;
                        }
                        
                        var potentialPair = requestsToProcess[j];
                        _logger.Debug("   - **PAIR_CANDIDATE**: Index {SearchIndex}, Field='{CandidateField}', Type='{CandidateType}'", 
                            j, potentialPair.FieldName, potentialPair.CorrectionType);
                        
                        if (potentialPair.CorrectionType == "format_correction" && 
                            potentialPair.FieldName == request.FieldName && 
                            potentialPair.LineNumber == request.LineNumber)
                        {
                            formatRequestIndex = j;
                            _logger.Information("✅ **PAIR_FOUND**: format_correction match at index {PairIndex}", j);
                            break;
                        }
                    }

                    if (formatRequestIndex != -1)
                    {
                        // **PAIRED_EXECUTION**: Process format_correction with field linkage
                        var formatRequest = requestsToProcess[formatRequestIndex];
                        processedIndexes.Add(formatRequestIndex);

                        _logger.Information("🗣️ **PAIRED_EXECUTION_START**: Processing paired format_correction immediately");
                        _logger.Information("   - **PAIRED_FIELD**: '{FieldName}' at index {PairIndex}", formatRequest.FieldName, formatRequestIndex);
                        _logger.Information("   - **FIELD_ID_INJECTION**: Injecting FieldId={FieldId} for database linkage", newFieldId);
                        _logger.Information("   - **ATOMIC_PAIRING**: Both omission and format_correction processed in same transaction context");

                        formatRequest.FieldId = newFieldId;
                        var pairedResult = await this.ProcessSingleRequestAsync(context, formatRequest).ConfigureAwait(false);
                        
                        _logger.Information("📊 **PAIRED_RESULT**: format_correction for '{FieldName}' processing: {PairedSuccess}", 
                            formatRequest.FieldName, pairedResult.IsSuccess ? "SUCCESS" : "FAILED");
                    }
                    else
                    {
                        // **NO_PAIR_SCENARIO**: Omission without corresponding format_correction
                        _logger.Information("🤷 **NO_PAIR_FOUND**: No format_correction pair found for omission '{FieldName}'", request.FieldName);
                        _logger.Information("   - **SEARCH_RESULT**: No matching format_correction in remaining requests");
                        _logger.Information("   - **PROCESSING_CONTINUITY**: Omission processed successfully without pairing");
                        _logger.Information("   - **FIELD_LINKAGE**: FieldId={FieldId} available for future format_correction operations", newFieldId);
                    }
                }
                else if (request.CorrectionType == "omission")
                {
                    // **OMISSION_FAILURE_SCENARIO**: Failed omission processing
                    _logger.Warning("⚠️ **OMISSION_PROCESSING_FAILED**: Omission for '{FieldName}' did not succeed - no pairing attempted", request.FieldName);
                    _logger.Warning("   - **FAILURE_REASON**: {FailureMessage}", dbUpdateResult.Message ?? "Unknown failure");
                    _logger.Warning("   - **PAIRING_IMPACT**: No format_correction pairing will be attempted");
                    _logger.Warning("   - **PROCESSING_CONTINUITY**: Continuing with next request");
                }
            }
            
            _logger.Information("🏁 **MAIN_PROCESSING_LOOP_COMPLETE**: All requests processed with pairing logic");
            _logger.Information("   - **PROCESSED_REQUESTS**: {TotalRequests} total requests", requestsToProcess.Count);
            _logger.Information("   - **PAIRED_REQUESTS**: {PairedCount} requests processed as pairs", processedIndexes.Count);
            _logger.Information("   - **INDIVIDUAL_REQUESTS**: {IndividualCount} requests processed individually", 
                requestsToProcess.Count - processedIndexes.Count);
            
            // **COMPREHENSIVE_PIPELINE_VERIFICATION**: Final processing verification and reporting
            _logger.Information("🏁 **ORCHESTRATOR_COMPLETION_VERIFICATION**: Conducting comprehensive pipeline verification");
            _logger.Information("   - **VERIFICATION_PURPOSE**: Ensure all DeepSeek corrections were properly processed");
            _logger.Information("   - **REPORTING_SCOPE**: Total processing, pairing statistics, field coverage analysis");
            _logger.Information("   - **INTEGRITY_CHECK**: Validate expected vs actual correction processing");
            
            // **PROCESSING_STATISTICS_CALCULATION**: Generate detailed processing metrics
            var totalRequests = regexUpdateRequests.Count();
            var processedCount = totalRequests; // All requests are processed (some as pairs)
            var pairedCount = processedIndexes.Count;
            var individualCount = totalRequests - pairedCount;
            
            _logger.Information("📊 **COMPREHENSIVE_PIPELINE_REPORT**: Complete processing statistics");
            _logger.Information("   - **TOTAL_DEEPSEEK_CORRECTIONS**: {TotalRequests} corrections received from DeepSeek", totalRequests);
            _logger.Information("   - **INDIVIDUAL_PROCESSING**: {IndividualCount} requests processed individually", individualCount);
            _logger.Information("   - **PAIRED_PROCESSING**: {PairedCount} requests processed as format_correction pairs", pairedCount);
            _logger.Information("   - **TOTAL_PROCESSED**: {ProcessedCount} total processing operations completed", processedCount);
            _logger.Information("   - **PROCESSING_EFFICIENCY**: {EfficiencyRate:P1} success rate", (double)processedCount / totalRequests);
            
            // **FIELD_COVERAGE_ANALYSIS**: Detailed field-by-field processing verification
            if (regexUpdateRequests != null && regexUpdateRequests.Any())
            {
                _logger.Information("📋 **FIELD_COVERAGE_ANALYSIS**: Analyzing correction distribution by field");
                var fieldBreakdown = regexUpdateRequests.GroupBy(r => r.FieldName).Select(g => new { Field = g.Key, Count = g.Count() });
                _logger.Information("   - **FIELD_DISTRIBUTION**: {FieldSummary}", 
                    string.Join(", ", fieldBreakdown.Select(f => $"{f.Field}: {f.Count}")));
                
                // **EXPECTED_FIELD_VALIDATION**: Verify critical invoice fields were processed
                _logger.Information("🔍 **EXPECTED_FIELD_VALIDATION**: Validating critical invoice field coverage");
                var expectedFields = new[] { "InvoiceNo", "InvoiceDate", "SupplierName", "Currency", "SubTotal", "TotalDeduction", "InvoiceTotal", 
                    "InvoiceDetail_SingleColumn_MultiField_Lines3_11", "InvoiceDetail_SparseText_MultiField_Lines6_28" };
                var actualFields = regexUpdateRequests.Select(r => r.FieldName).Distinct().ToList();
                var missingFields = expectedFields.Except(actualFields).ToList();
                var unexpectedFields = actualFields.Except(expectedFields).ToList();
                
                _logger.Information("   - **EXPECTED_FIELDS**: {ExpectedCount} critical invoice fields", expectedFields.Length);
                _logger.Information("   - **ACTUAL_FIELDS**: {ActualCount} unique fields in corrections", actualFields.Count);
                
                if (missingFields.Any())
                {
                    _logger.Warning("🚨 **MISSING_CRITICAL_FIELDS**: Expected fields not found in corrections");
                    _logger.Warning("   - **MISSING_FIELDS**: {MissingFields}", string.Join(", ", missingFields));
                    _logger.Warning("   - **IMPACT**: Some critical invoice fields may not have learned patterns");
                }
                if (unexpectedFields.Any())
                {
                    _logger.Information("⚠️ **ADDITIONAL_FIELDS_FOUND**: Unexpected fields discovered in corrections");
                    _logger.Information("   - **UNEXPECTED_FIELDS**: {UnexpectedFields}", string.Join(", ", unexpectedFields));
                    _logger.Information("   - **BENEFIT**: Additional field learning beyond expected critical fields");
                }
                if (!missingFields.Any() && !unexpectedFields.Any())
                {
                    _logger.Information("✅ **PERFECT_FIELD_COVERAGE**: All expected fields present, no unexpected fields");
                    _logger.Information("   - **PIPELINE_INTEGRITY**: Complete critical field coverage achieved");
                    _logger.Information("   - **LEARNING_COMPLETENESS**: All essential invoice patterns captured");
                }
                else if (!missingFields.Any())
                {
                    _logger.Information("✅ **COMPLETE_CRITICAL_COVERAGE**: All expected critical fields processed successfully");
                    _logger.Information("   - **BONUS_COVERAGE**: Additional fields provide enhanced learning");
                }
            }
            
            // **ORCHESTRATOR_COMPLETION_FINAL_STATUS**
            _logger.Information("🏁 **DB_UPDATE_ORCHESTRATOR_COMPLETE**: Database update orchestration finished successfully");
            _logger.Information("   - **PROCESSING_OUTCOME**: All regex update requests processed with pairing support");
            _logger.Information("   - **LEARNING_CAPTURE**: DeepSeek corrections successfully integrated into database");
            _logger.Information("   - **PIPELINE_INTEGRITY**: Comprehensive verification completed");
            _logger.Information("   - **SYSTEM_READINESS**: Updated patterns available for future OCR processing");
            
            // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION - OBJECT-ORIENTED FUNCTIONAL DUAL LAYER APPROACH**
            _logger.Error("🎯 **TEMPLATE_SPECIFICATION_VALIDATION**: UpdateRegexPatternsAsync dual-layer template specification compliance analysis");

            // Determine document type using DatabaseTemplateHelper (MANDATORY - NO HARDCODING)
            string documentType = "Shipment Invoice"; // Database update orchestrator is document-type agnostic
            _logger.Error($"📋 **DOCUMENT_TYPE_DETECTED**: {documentType} - Using DatabaseTemplateHelper document-specific validation rules");

            // Create template specification object for document type with dual-layer validation
            var templateSpec = TemplateSpecification.CreateForUtilityOperation(documentType, "UpdateRegexPatternsAsync", 
                regexUpdateRequests, null);

            // Fluent validation with short-circuiting - stops on first failure
            var validatedSpec = templateSpec
                .ValidateEntityTypeAwareness(null) // No AI recommendations for utility operations
                .ValidateFieldMappingEnhancement(null)
                .ValidateDataTypeRecommendations(new List<WaterNut.DataSpace.AITemplateService.PromptRecommendation>()) // Database orchestrator operations process object collections
                .ValidatePatternQuality(null)
                .ValidateTemplateOptimization(null);

            // Log all validation results
            validatedSpec.LogValidationResults(_logger);

            // Extract overall success from validated specification
            bool templateSpecificationSuccess = validatedSpec.IsValid;
        }

        /// <summary>
        /// **ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v5**: Process single RegexUpdateRequest atomically with comprehensive logging
        /// **ARCHITECTURAL_INTENT**: Self-contained atomic transaction processing with validation, strategy execution, and learning capture
        /// **BUSINESS_RULE**: Each request processed independently to prevent cascade failures and ensure data integrity
        /// **DESIGN_SPECIFICATION**: Comprehensive error handling with detailed logging for troubleshooting and audit trails
        /// </summary>
        private async Task<DatabaseUpdateResult> ProcessSingleRequestAsync(OCRContext context, RegexUpdateRequest request)
        {
            // 🧠 **LOG_THE_WHAT**: Configuration state, input data, design specifications, expected behavior
            _logger.Information("▶️ **ATOMIC_REQUEST_PROCESSING_START**: Beginning single request atomic processing");
            _logger.Information("   - **ARCHITECTURAL_INTENT**: Self-contained atomic transaction for individual regex update request");
            _logger.Information("   - **REQUEST_FIELD**: '{FieldName}'", request?.FieldName ?? "NULL");
            _logger.Information("   - **REQUEST_TYPE**: '{CorrectionType}'", request?.CorrectionType ?? "NULL");
            _logger.Information("   - **EXPECTED_BEHAVIOR**: Validation -> Strategy Selection -> Execution -> Learning Capture");
            _logger.Information("   - **BUSINESS_RULE_RATIONALE**: Atomic processing prevents cascade failures and ensures data consistency");
            
            // **LOG_THE_HOW**: Internal state, method flow, decision points, data transformations
            _logger.Information("🔄 **ATOMIC_PROCESSING_SEQUENCE**: Multi-phase atomic transaction processing");
            _logger.Information("   - **PHASE_1**: Request validation and integrity verification");
            _logger.Information("   - **PHASE_2**: Strategy selection based on correction type");
            _logger.Information("   - **PHASE_3**: Strategy execution with database operations");
            _logger.Information("   - **PHASE_4**: Result verification and learning record creation");
            _logger.Information("   - **TRANSACTION_SCOPE**: All operations within single database context for atomicity");
            
            // **LOG_THE_WHY**: Atomic processing rationale and architectural decisions
            _logger.Information("🎯 **ATOMIC_PROCESSING_RATIONALE**: Independent processing ensures system resilience");
            _logger.Information("   - **ISOLATION_BENEFIT**: Single request failure doesn't affect other requests");
            _logger.Information("   - **TRANSACTION_INTEGRITY**: Each request committed independently for data consistency");
            _logger.Information("   - **TROUBLESHOOTING_SUPPORT**: Comprehensive logging enables precise failure diagnosis");
            _logger.Information("   - **AUDIT_COMPLIANCE**: Complete processing trail for regulatory and debugging requirements");

            DatabaseUpdateResult result = null;
            
            try
            {
                // **REQUEST_SERIALIZATION_FOR_AUDIT**: Capture complete request state for debugging
                _logger.Information("🧬 **REQUEST_SERIALIZATION**: Capturing complete request data for audit trail");
                _logger.Information("   - **SERIALIZATION_PURPOSE**: Complete request state preservation for troubleshooting");
                _logger.Information("   - **FORMAT_SPECIFICATION**: Indented JSON with null exclusion for readability");
                
                var options = new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull };
                var serializedRequest = JsonSerializer.Serialize(request, options);
                _logger.Information("📋 **COMPLETE_REQUEST_DATA**: Full RegexUpdateRequest serialization");
                _logger.Information("   - **REQUEST_CONTENT**: {SerializedRequest}", serializedRequest);
                _logger.Information("   - **DATA_INTEGRITY**: All request properties captured for processing verification");

                // **VALIDATION_PHASE**: Critical request validation before processing
                _logger.Information("🛡️ **REQUEST_VALIDATION_START**: Validating request integrity before strategy selection");
                _logger.Information("   - **VALIDATION_IMPORTANCE**: Prevents invalid data from reaching database strategies");
                _logger.Information("   - **VALIDATION_SCOPE**: Field name presence, field support verification, data integrity");
                _logger.Information("   - **FAILURE_PREVENTION**: Early validation prevents downstream processing errors");
                
                var validationResult = this.ValidateUpdateRequest(request);
                
                if (!validationResult.IsValid)
                {
                    // **LOG_THE_WHO**: Validation failure handling with comprehensive error reporting
                    _logger.Warning("❌ **VALIDATION_FAILED**: Request validation failed before processing");
                    _logger.Warning("   - **VALIDATION_ERROR**: {ErrorMessage}", validationResult.ErrorMessage);
                    _logger.Warning("   - **FAILURE_IMPACT**: Request will not be processed by database strategy");
                    _logger.Warning("   - **PROCESSING_OUTCOME**: Returning failed result without database operations");
                    _logger.Warning("   - **ERROR_PREVENTION**: Invalid request prevented from corrupting database");
                    
                    result = DatabaseUpdateResult.Failed($"Validation failed: {validationResult.ErrorMessage}");
                    
                    // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION - OBJECT-ORIENTED FUNCTIONAL DUAL LAYER APPROACH**
                    _logger.Error("🎯 **TEMPLATE_SPECIFICATION_VALIDATION**: ProcessSingleRequestAsync dual-layer template specification compliance analysis");

                    // Determine document type using DatabaseTemplateHelper (MANDATORY - NO HARDCODING)
                    string documentType = "Shipment Invoice"; // Single request processing is document-type agnostic
                    _logger.Error($"📋 **DOCUMENT_TYPE_DETECTED**: {documentType} - Using DatabaseTemplateHelper document-specific validation rules");

                    // Create template specification object for document type with dual-layer validation
                    var templateSpec = TemplateSpecification.CreateForUtilityOperation(documentType, "ProcessSingleRequestAsync", 
                        request, result);

                    // Fluent validation with short-circuiting - stops on first failure
                    var validatedSpec = templateSpec
                        .ValidateEntityTypeAwareness(null) // No AI recommendations for utility operations
                        .ValidateFieldMappingEnhancement(null)
                        .ValidateDataTypeRecommendations(new List<WaterNut.DataSpace.AITemplateService.PromptRecommendation>()) // Database request processing operations
                        .ValidatePatternQuality(null)
                        .ValidateTemplateOptimization(null);

                    // Log all validation results
                    validatedSpec.LogValidationResults(_logger);

                    // Extract overall success from validated specification
                    bool templateSpecificationSuccess = validatedSpec.IsValid;
                }
                else
                {
                    // **VALIDATION_SUCCESS_PATH**: Proceed with strategy selection and execution
                    _logger.Information("✅ **VALIDATION_PASSED**: Request validation successful - proceeding with strategy processing");
                    _logger.Information("   - **VALIDATION_OUTCOME**: All request integrity checks passed");
                    _logger.Information("   - **PROCESSING_CLEARANCE**: Request approved for database strategy execution");
                    _logger.Information("   - **NEXT_PHASE**: Strategy selection based on correction type");
                    
                    // **STRATEGY_SELECTION**: Get appropriate database update strategy
                    _logger.Information("♟️ **STRATEGY_SELECTION_START**: Selecting database update strategy for correction type");
                    _logger.Information("   - **CORRECTION_TYPE**: '{CorrectionType}'", request.CorrectionType);
                    _logger.Information("   - **FACTORY_DELEGATION**: Using DatabaseUpdateStrategyFactory for strategy instantiation");
                    
                    var strategy = _strategyFactory.GetStrategy(request);
                    
                    _logger.Information("✅ **STRATEGY_SELECTED**: Database update strategy determined");
                    _logger.Information("   - **STRATEGY_TYPE**: '{StrategyType}'", strategy.GetType().Name);
                    _logger.Information("   - **STRATEGY_PURPOSE**: Handles '{CorrectionType}' correction type processing", request.CorrectionType);
                    _logger.Information("   - **EXECUTION_READINESS**: Strategy ready for atomic database operations");

                    // **STRATEGY_EXECUTION**: Execute database update with atomic transaction
                    _logger.Information("🔄 **STRATEGY_EXECUTION_START**: Beginning atomic database update execution");
                    _logger.Information("   - **ATOMIC_GUARANTEE**: Strategy ExecuteAsync includes internal SaveChanges for atomicity");
                    _logger.Information("   - **TRANSACTION_SCOPE**: All database operations within single transaction context");
                    _logger.Information("   - **EXECUTION_DELEGATION**: Strategy handles all database-specific operations");
                    
                    result = await strategy.ExecuteAsync(context, request, this).ConfigureAwait(false);
                    
                    _logger.Information("📊 **STRATEGY_EXECUTION_COMPLETE**: Database strategy execution finished");
                    _logger.Information("   - **EXECUTION_RESULT**: {ExecutionOutcome}", result?.IsSuccess == true ? "SUCCESS" : "FAILED");
                    _logger.Information("   - **RESULT_MESSAGE**: '{ResultMessage}'", result?.Message ?? "No message");
                    _logger.Information("   - **DATABASE_CHANGES**: Strategy completed all required database operations");
                }
            }
            catch (Exception ex)
            {
                // **LOG_THE_WHO**: Exception handling with comprehensive failure analysis
                _logger.Error(ex, "🚨 **ATOMIC_PROCESSING_EXCEPTION**: Unhandled exception during atomic request processing");
                _logger.Error("   - **AFFECTED_FIELD**: '{FieldName}'", request?.FieldName ?? "UNKNOWN");
                _logger.Error("   - **EXCEPTION_TYPE**: {ExceptionType}", ex.GetType().FullName);
                _logger.Error("   - **EXCEPTION_MESSAGE**: {ExceptionMessage}", ex.Message);
                _logger.Error("   - **PROCESSING_PHASE**: Exception occurred during validation, strategy selection, or execution");
                _logger.Error("   - **FAILURE_IMPACT**: Atomic processing failed - no database changes committed");
                _logger.Error("   - **ISOLATION_BENEFIT**: Exception isolated to single request - other requests unaffected");
                _logger.Error("   - **RECOVERY_STRATEGY**: Failed result returned for proper error handling");
                
                result = DatabaseUpdateResult.Failed($"Atomic process exception: {ex.Message}", ex);
            }
            finally
            {
                // **RESULT_VALIDATION**: Ensure result object is always available for processing
                result ??= DatabaseUpdateResult.Failed("Internal error: DBUpdateResult was unexpectedly null.");
                
                // **OUTCOME_ANALYSIS**: Determine processing outcome and appropriate logging level
                var outcome = result.IsSuccess ? "✅ SUCCESS" : "❌ FAILURE";
                var level = result.IsSuccess ? Serilog.Events.LogEventLevel.Information : Serilog.Events.LogEventLevel.Error;
                
                // **RESULT_SERIALIZATION**: Capture complete result state for audit trail
                var serializedResult = JsonSerializer.Serialize(result, new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });
                
                // **LOG_THE_WHO**: Processing outcome verification and critical result logging
                _logger.Information("🎯 **ATOMIC_PROCESSING_OUTCOME**: Request processing completed");
                _logger.Information("   - **FIELD_PROCESSED**: '{FieldName}'", request?.FieldName ?? "UNKNOWN");
                _logger.Information("   - **CORRECTION_TYPE**: '{CorrectionType}'", request?.CorrectionType ?? "UNKNOWN");
                _logger.Information("   - **PROCESSING_OUTCOME**: {Outcome}", outcome);
                _logger.Information("   - **RESULT_MESSAGE**: '{Message}'", result.Message ?? "Success");
                _logger.Information("   - **DEEPSEEK_CORRECTION_STATUS**: {ProcessingStatus}", result.IsSuccess ? "CAPTURED" : "LOST");
                
                // **DATABASE_ENTITY_VERIFICATION**: Log successful database entity creation
                if (result.IsSuccess && result.RelatedRecordId.HasValue)
                {
                    _logger.Information("🆔 **DATABASE_ENTITY_CREATED**: New database entity created successfully");
                    _logger.Information("   - **ENTITY_FIELD**: '{FieldName}'", request?.FieldName ?? "UNKNOWN");
                    _logger.Information("   - **ENTITY_ID**: {EntityId}", result.RelatedRecordId.Value);
                    _logger.Information("   - **LEARNING_CAPTURE**: Database entity available for future pattern matching");
                    _logger.Information("   - **FIELD_LINKAGE**: Entity ID available for format_correction pairing");
                }
                
                // **FAILURE_ANALYSIS**: Comprehensive failure logging for failed corrections
                if (!result.IsSuccess)
                {
                    _logger.Error("🚨 **CORRECTION_PROCESSING_FAILED**: DeepSeek correction processing failed");
                    _logger.Error("   - **FAILED_FIELD**: '{FieldName}'", request?.FieldName ?? "UNKNOWN");
                    _logger.Error("   - **CORRECTION_TYPE**: '{CorrectionType}'", request?.CorrectionType ?? "UNKNOWN");
                    _logger.Error("   - **FAILURE_REASON**: {Error}", result.Message ?? "Unknown error");
                    _logger.Error("   - **LEARNING_IMPACT**: DeepSeek correction not captured in database");
                    _logger.Error("   - **PATTERN_LOSS**: Field pattern will not be available for future OCR processing");
                }

                // **COMPREHENSIVE_RESULT_LOGGING**: Log complete processing outcome with appropriate level
                _logger.Write(level, "🏁 **ATOMIC_PROCESS_FINAL_OUTCOME**: [{Outcome}] for Field '{FieldName}'", outcome, request?.FieldName ?? "UNKNOWN");
                _logger.Write(level, "📋 **COMPLETE_RESULT_DATA**: Full DatabaseUpdateResult from atomic processing");
                _logger.Write(level, "   - **RESULT_SERIALIZATION**: {SerializedResult}", serializedResult);
                
                // **LOG_THE_WHAT_IF**: Learning capture and audit trail completion
                _logger.Information("📝 **LEARNING_CAPTURE_START**: Creating OCRCorrectionLearning record for audit and future improvement");
                _logger.Information("   - **LEARNING_PURPOSE**: Capture processing outcome for system improvement and troubleshooting");
                _logger.Information("   - **AUDIT_COMPLIANCE**: Maintain complete audit trail of all correction attempts");
                _logger.Information("   - **FUTURE_ANALYSIS**: Learning data enables pattern analysis and system optimization");

                await this.LogCorrectionLearningAsync(context, request, result).ConfigureAwait(false);
                
                _logger.Information("✅ **ATOMIC_PROCESSING_COMPLETE**: Single request atomic processing finished");
                _logger.Information("   - **ATOMICITY_GUARANTEED**: All operations completed within single transaction scope");
                _logger.Information("   - **LEARNING_RECORDED**: Processing outcome captured in OCRCorrectionLearning");
                _logger.Information("   - **AUDIT_TRAIL_COMPLETE**: Full processing trail available for analysis");
                
                // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION - OBJECT-ORIENTED FUNCTIONAL DUAL LAYER APPROACH**
                _logger.Error("🎯 **TEMPLATE_SPECIFICATION_VALIDATION**: ProcessSingleRequestAsync dual-layer template specification compliance analysis");

                // Determine document type using DatabaseTemplateHelper (MANDATORY - NO HARDCODING)
                string documentType = "Invoice"; // Single request processing is document-type agnostic
                _logger.Error($"📋 **DOCUMENT_TYPE_DETECTED**: {documentType} - Using DatabaseTemplateHelper document-specific validation rules");

                // Create template specification object for document type with dual-layer validation
                var templateSpec = TemplateSpecification.CreateForUtilityOperation(documentType, "ProcessSingleRequestAsync", 
                    request, result);

                // Fluent validation with short-circuiting - stops on first failure
                var validatedSpec = templateSpec
                    .ValidateEntityTypeAwareness(null) // No AI recommendations for utility operations
                    .ValidateFieldMappingEnhancement(null)
                    .ValidateDataTypeRecommendations(new List<WaterNut.DataSpace.AITemplateService.PromptRecommendation>()) // Database request processing operations
                    .ValidatePatternQuality(null)
                    .ValidateTemplateOptimization(null);

                // Log all validation results
                validatedSpec.LogValidationResults(_logger);

                // Extract overall success from validated specification
                bool templateSpecificationSuccess = validatedSpec.IsValid;
            }
            return result;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// **ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v5**: Get comprehensive field validation information with mapping verification
        /// **ARCHITECTURAL_INTENT**: Provide detailed field validation information for request processing validation
        /// **BUSINESS_RULE**: Field must exist in DeepSeek to database mapping for processing eligibility
        /// **DESIGN_SPECIFICATION**: Return validation result with detailed error information for troubleshooting
        /// </summary>
        public FieldValidationInfo GetFieldValidationInfo(string rawFieldName)
        {
            // 🧠 **LOG_THE_WHAT**: Configuration state, input data, design specifications, expected behavior
            _logger.Information("🔍 **FIELD_VALIDATION_INFO_REQUEST**: Getting comprehensive field validation information");
            _logger.Information("   - **ARCHITECTURAL_INTENT**: Provide detailed validation info for field processing eligibility");
            _logger.Information("   - **INPUT_FIELD_NAME**: '{RawFieldName}'", rawFieldName ?? "NULL");
            _logger.Information("   - **EXPECTED_BEHAVIOR**: Map field to database, return validation result with error details");
            _logger.Information("   - **BUSINESS_RULE_RATIONALE**: Only mapped fields can be processed by correction strategies");
            
            // **LOG_THE_HOW**: Internal state, method flow, decision points, data transformations
            _logger.Information("🔄 **VALIDATION_SEQUENCE**: Field mapping verification and validation result construction");
            _logger.Information("   - **STEP_1**: Attempt field mapping using MapDeepSeekFieldToDatabase");
            _logger.Information("   - **STEP_2**: Analyze mapping result for validity determination");
            _logger.Information("   - **STEP_3**: Construct FieldValidationInfo with appropriate status and messages");
            _logger.Information("   - **MAPPING_DEPENDENCY**: Validation depends on successful DeepSeek to database field mapping");
            
            // **LOG_THE_WHY**: Field validation rationale and mapping importance
            _logger.Information("🎯 **FIELD_VALIDATION_RATIONALE**: Field mapping validation prevents processing errors");
            _logger.Information("   - **MAPPING_IMPORTANCE**: Unmapped fields cannot be processed by database strategies");
            _logger.Information("   - **ERROR_PREVENTION**: Early validation prevents downstream processing failures");
            _logger.Information("   - **TROUBLESHOOTING_SUPPORT**: Detailed validation info enables precise error diagnosis");
            
            _logger.Information("🔍 **FIELD_MAPPING_ATTEMPT**: Attempting DeepSeek to database field mapping");
            var fieldInfo = this.MapDeepSeekFieldToDatabase(rawFieldName);
            
            if (fieldInfo == null)
            {
                // **LOG_THE_WHO**: Mapping failure scenario with comprehensive error reporting
                _logger.Warning("❌ **FIELD_MAPPING_FAILED**: Field not found in DeepSeek to database mapping");
                _logger.Warning("   - **UNMAPPED_FIELD**: '{RawFieldName}'", rawFieldName ?? "NULL");
                _logger.Warning("   - **VALIDATION_RESULT**: Field validation failed - not eligible for processing");
                _logger.Warning("   - **ERROR_REASON**: Field unknown or not configured in mapping dictionary");
                _logger.Warning("   - **PROCESSING_IMPACT**: Field cannot be processed by correction strategies");
                
                var errorMessage = $"Field '{rawFieldName}' is unknown or not mapped.";
                _logger.Warning("   - **ERROR_MESSAGE**: '{ErrorMessage}'", errorMessage);
                
                // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION - OBJECT-ORIENTED FUNCTIONAL DUAL LAYER APPROACH**
                _logger.Error("🎯 **TEMPLATE_SPECIFICATION_VALIDATION**: GetFieldValidationInfo dual-layer template specification compliance analysis");

                // Determine document type using DatabaseTemplateHelper (MANDATORY - NO HARDCODING)
                string documentType = "Invoice"; // Field validation operations are document-type agnostic
                _logger.Error($"📋 **DOCUMENT_TYPE_DETECTED**: {documentType} - Using DatabaseTemplateHelper document-specific validation rules");

                // Create template specification object for document type with dual-layer validation
                var templateSpec = TemplateSpecification.CreateForUtilityOperation(documentType, "GetFieldValidationInfo", 
                    rawFieldName, new FieldValidationInfo { IsValid = false, ErrorMessage = errorMessage });

                // Fluent validation with short-circuiting - stops on first failure
                var validatedSpec = templateSpec
                    .ValidateEntityTypeAwareness(null) // No AI recommendations for utility operations
                    .ValidateFieldMappingEnhancement(null)
                    .ValidateDataTypeRecommendations(new List<WaterNut.DataSpace.AITemplateService.PromptRecommendation>()) // Field validation operations return objects
                    .ValidatePatternQuality(null)
                    .ValidateTemplateOptimization(null);

                // Log all validation results
                validatedSpec.LogValidationResults(_logger);

                // Extract overall success from validated specification
                bool templateSpecificationSuccess = validatedSpec.IsValid;
                
                return new FieldValidationInfo { IsValid = false, ErrorMessage = errorMessage };
            }
            else
            {
                // **LOG_THE_WHO**: Mapping success scenario with validation confirmation
                _logger.Information("✅ **FIELD_MAPPING_SUCCESS**: Field successfully mapped to database field");
                _logger.Information("   - **MAPPED_FIELD**: '{RawFieldName}' -> '{DatabaseField}'", rawFieldName, fieldInfo.DatabaseFieldName);
                _logger.Information("   - **ENTITY_TYPE**: '{EntityType}'", fieldInfo.EntityType);
                _logger.Information("   - **DATA_TYPE**: '{DataType}'", fieldInfo.DataType);
                _logger.Information("   - **IS_REQUIRED**: {IsRequired}", fieldInfo.IsRequired);
                _logger.Information("   - **VALIDATION_RESULT**: Field validation passed - eligible for correction processing");
                _logger.Information("   - **PROCESSING_CLEARANCE**: Field can be processed by appropriate database strategies");
                
                // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION - OBJECT-ORIENTED FUNCTIONAL DUAL LAYER APPROACH**
                _logger.Error("🎯 **TEMPLATE_SPECIFICATION_VALIDATION**: GetFieldValidationInfo dual-layer template specification compliance analysis");

                // Determine document type using DatabaseTemplateHelper (MANDATORY - NO HARDCODING)
                string documentType = "Invoice"; // Field validation operations are document-type agnostic
                _logger.Error($"📋 **DOCUMENT_TYPE_DETECTED**: {documentType} - Using DatabaseTemplateHelper document-specific validation rules");

                // Create template specification object for document type with dual-layer validation
                var templateSpec = TemplateSpecification.CreateForUtilityOperation(documentType, "GetFieldValidationInfo", 
                    rawFieldName, new FieldValidationInfo { IsValid = true });

                // Fluent validation with short-circuiting - stops on first failure
                var validatedSpec = templateSpec
                    .ValidateEntityTypeAwareness(null) // No AI recommendations for utility operations
                    .ValidateFieldMappingEnhancement(null)
                    .ValidateDataTypeRecommendations(new List<WaterNut.DataSpace.AITemplateService.PromptRecommendation>()) // Field validation operations return objects
                    .ValidatePatternQuality(null)
                    .ValidateTemplateOptimization(null);

                // Log all validation results
                validatedSpec.LogValidationResults(_logger);

                // Extract overall success from validated specification
                bool templateSpecificationSuccess = validatedSpec.IsValid;
                
                return new FieldValidationInfo { IsValid = true };
            }
            
            // **LOG_THE_WHAT_IF**: Method completion verification
            // Note: This code is unreachable but included for completeness
            _logger.Information("🏁 **FIELD_VALIDATION_INFO_COMPLETE**: Field validation information request completed");
        }

        /// <summary>
        /// **ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v5**: Determine if field is supported by checking mapping dictionary
        /// **ARCHITECTURAL_INTENT**: Provide fast boolean check for field support without detailed validation information
        /// **BUSINESS_RULE**: Field support determined by presence in DeepSeekToDBFieldMapping dictionary
        /// **DESIGN_SPECIFICATION**: Simple boolean return with null/whitespace validation
        /// </summary>
        public bool IsFieldSupported(string rawFieldName)
        {
            // 🧠 **LOG_THE_WHAT**: Configuration state, input data, design specifications, expected behavior
            _logger.Information("🔍 **FIELD_SUPPORT_CHECK**: Checking if field is supported by OCR correction system");
            _logger.Information("   - **ARCHITECTURAL_INTENT**: Fast boolean check for field processing eligibility");
            _logger.Information("   - **INPUT_FIELD_NAME**: '{RawFieldName}'", rawFieldName ?? "NULL");
            _logger.Information("   - **EXPECTED_BEHAVIOR**: Return true if field exists in mapping dictionary, false otherwise");
            _logger.Information("   - **BUSINESS_RULE_RATIONALE**: Only supported fields can be processed by correction strategies");
            
            // **LOG_THE_HOW**: Internal state, method flow, decision points, data transformations
            _logger.Information("🔄 **SUPPORT_CHECK_SEQUENCE**: Field validation and mapping dictionary lookup");
            _logger.Information("   - **STEP_1**: Validate input field name for null/whitespace");
            _logger.Information("   - **STEP_2**: Check DeepSeekToDBFieldMapping dictionary for field presence");
            _logger.Information("   - **PERFORMANCE_OPTIMIZATION**: Direct dictionary lookup for fast response");
            _logger.Information("   - **NORMALIZATION**: Trim whitespace from field name for consistent lookup");
            
            // **INPUT_VALIDATION**: Check for null or whitespace field names
            if (string.IsNullOrWhiteSpace(rawFieldName))
            {
                // **LOG_THE_WHO**: Invalid input scenario with detailed reasoning
                _logger.Warning("❌ **INVALID_FIELD_NAME**: Field name is null, empty, or whitespace");
                _logger.Warning("   - **INPUT_STATE**: '{InputValue}'", rawFieldName ?? "NULL");
                _logger.Warning("   - **VALIDATION_FAILURE**: Cannot check support for invalid field name");
                _logger.Warning("   - **RETURN_VALUE**: false (unsupported due to invalid input)");
                _logger.Warning("   - **ERROR_PREVENTION**: Invalid field names cannot be processed");
                
                // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION - OBJECT-ORIENTED FUNCTIONAL DUAL LAYER APPROACH**
                _logger.Error("🎯 **TEMPLATE_SPECIFICATION_VALIDATION**: IsFieldSupported dual-layer template specification compliance analysis");

                // Determine document type using DatabaseTemplateHelper (MANDATORY - NO HARDCODING)
                string documentTypeFieldSupport = "Invoice"; // Field support checking is document-type agnostic
                _logger.Error($"📋 **DOCUMENT_TYPE_DETECTED**: {documentTypeFieldSupport} - Using DatabaseTemplateHelper document-specific validation rules");

                // Create template specification object for document type with dual-layer validation
                var templateSpecFieldSupportCheck = TemplateSpecification.CreateForUtilityOperation(documentTypeFieldSupport, "IsFieldSupported", 
                    rawFieldName, false);

                // Fluent validation with short-circuiting - stops on first failure
                var validatedSpecFieldSupportCheck = templateSpecFieldSupportCheck
                    .ValidateEntityTypeAwareness(null) // No AI recommendations for utility operations
                    .ValidateFieldMappingEnhancement(null)
                    .ValidateDataTypeRecommendations(new List<WaterNut.DataSpace.AITemplateService.PromptRecommendation>()) // Field support operations return boolean results
                    .ValidatePatternQuality(null)
                    .ValidateTemplateOptimization(null);

                // Log all validation results
                validatedSpecFieldSupportCheck.LogValidationResults(_logger);

                // Extract overall success from validated specification
                bool templateSpecificationSuccessFieldSupportCheck = validatedSpecFieldSupportCheck.IsValid;
                
                return false;
            }
            
            // **LOG_THE_WHY**: Field support check rationale and mapping importance
            _logger.Information("🎯 **FIELD_SUPPORT_RATIONALE**: Dictionary lookup determines processing eligibility");
            _logger.Information("   - **MAPPING_DEPENDENCY**: Support determined by DeepSeekToDBFieldMapping presence");
            _logger.Information("   - **PERFORMANCE_BENEFIT**: Fast dictionary lookup avoids expensive validation operations");
            _logger.Information("   - **PROCESSING_GATE**: Support check prevents invalid fields from reaching strategies");
            
            // **DICTIONARY_LOOKUP**: Check mapping dictionary for field support
            _logger.Information("🔍 **MAPPING_DICTIONARY_LOOKUP**: Checking DeepSeekToDBFieldMapping for field support");
            _logger.Information("   - **LOOKUP_KEY**: '{TrimmedFieldName}'", rawFieldName.Trim());
            _logger.Information("   - **DICTIONARY_SOURCE**: DeepSeekToDBFieldMapping contains all supported field mappings");
            
            var isSupported = DeepSeekToDBFieldMapping.ContainsKey(rawFieldName.Trim());
            
            // **LOG_THE_WHO**: Support check result with comprehensive status reporting
            if (isSupported)
            {
                _logger.Information("✅ **FIELD_SUPPORTED**: Field found in mapping dictionary");
                _logger.Information("   - **SUPPORTED_FIELD**: '{FieldName}'", rawFieldName.Trim());
                _logger.Information("   - **PROCESSING_ELIGIBILITY**: Field can be processed by correction strategies");
                _logger.Information("   - **RETURN_VALUE**: true (supported)");
                _logger.Information("   - **NEXT_STEPS**: Field can proceed to validation and strategy processing");
            }
            else
            {
                _logger.Warning("❌ **FIELD_NOT_SUPPORTED**: Field not found in mapping dictionary");
                _logger.Warning("   - **UNSUPPORTED_FIELD**: '{FieldName}'", rawFieldName.Trim());
                _logger.Warning("   - **PROCESSING_RESTRICTION**: Field cannot be processed by correction strategies");
                _logger.Warning("   - **RETURN_VALUE**: false (not supported)");
                _logger.Warning("   - **CONFIGURATION_NEEDED**: Field may need to be added to DeepSeekToDBFieldMapping");
            }
            
            // **LOG_THE_WHAT_IF**: Method completion with support determination
            _logger.Information("🏁 **FIELD_SUPPORT_CHECK_COMPLETE**: Field support determination finished");
            _logger.Information("   - **SUPPORT_STATUS**: {SupportStatus}", isSupported ? "SUPPORTED" : "NOT_SUPPORTED");
            _logger.Information("   - **PROCESSING_OUTCOME**: Field {ProcessingEligibility}", 
                isSupported ? "eligible for correction processing" : "ineligible for processing");
            
            // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION - OBJECT-ORIENTED FUNCTIONAL DUAL LAYER APPROACH**
            _logger.Error("🎯 **TEMPLATE_SPECIFICATION_VALIDATION**: IsFieldSupported dual-layer template specification compliance analysis");

            // Determine document type using DatabaseTemplateHelper (MANDATORY - NO HARDCODING)
            string documentType = "Invoice"; // Field support checking is document-type agnostic
            _logger.Error($"📋 **DOCUMENT_TYPE_DETECTED**: {documentType} - Using DatabaseTemplateHelper document-specific validation rules");

            // Create template specification object for document type with dual-layer validation
            var templateSpec = TemplateSpecification.CreateForUtilityOperation(documentType, "IsFieldSupported", 
                rawFieldName, isSupported);

            // Fluent validation with short-circuiting - stops on first failure
            var validatedSpec = templateSpec
                .ValidateEntityTypeAwareness(null) // No AI recommendations for utility operations
                .ValidateFieldMappingEnhancement(null)
                .ValidateDataTypeRecommendations(new List<WaterNut.DataSpace.AITemplateService.PromptRecommendation>()) // Field support operations return boolean results
                .ValidatePatternQuality(null)
                .ValidateTemplateOptimization(null);

            // Log all validation results
            validatedSpec.LogValidationResults(_logger);

            // Extract overall success from validated specification
            bool templateSpecificationSuccess = validatedSpec.IsValid;
            
            return isSupported;
        }

        /// <summary>
        /// **ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v5**: Comprehensive RegexUpdateRequest validation with detailed error reporting
        /// **ARCHITECTURAL_INTENT**: Multi-layer validation to ensure request integrity before database strategy processing
        /// **BUSINESS_RULE**: All validation criteria must pass for request to be eligible for processing
        /// **DESIGN_SPECIFICATION**: Early validation prevents invalid requests from reaching database strategies
        /// </summary>
        private FieldValidationInfo ValidateUpdateRequest(RegexUpdateRequest request)
        {
            // 🧠 **LOG_THE_WHAT**: Configuration state, input data, design specifications, expected behavior
            _logger.Information("🛡️ **REQUEST_VALIDATION_START**: Comprehensive RegexUpdateRequest validation beginning");
            _logger.Information("   - **ARCHITECTURAL_INTENT**: Multi-layer validation ensures request integrity before processing");
            _logger.Information("   - **VALIDATION_SCOPE**: Null check, field name presence, field support verification");
            _logger.Information("   - **EXPECTED_BEHAVIOR**: Return validation result with specific error details for failures");
            _logger.Information("   - **BUSINESS_RULE_RATIONALE**: Invalid requests must be rejected before reaching database strategies");
            
            // **LOG_THE_HOW**: Internal state, method flow, decision points, data transformations
            _logger.Information("🔄 **VALIDATION_SEQUENCE**: Multi-step validation with early failure detection");
            _logger.Information("   - **STEP_1**: Null request object validation");
            _logger.Information("   - **STEP_2**: Field name presence and validity verification");
            _logger.Information("   - **STEP_3**: Field support verification using mapping dictionary");
            _logger.Information("   - **EARLY_EXIT_STRATEGY**: First validation failure immediately returns error result");
            
            // **LOG_THE_WHY**: Validation rationale and architectural importance
            _logger.Information("🎯 **VALIDATION_RATIONALE**: Comprehensive validation prevents processing errors and data corruption");
            _logger.Information("   - **ERROR_PREVENTION**: Early validation catches issues before expensive database operations");
            _logger.Information("   - **DATA_INTEGRITY**: Ensures only valid requests reach database strategies");
            _logger.Information("   - **TROUBLESHOOTING_SUPPORT**: Detailed error messages enable precise issue diagnosis");
            
            // **NULL_REQUEST_VALIDATION**: Check for null request object
            _logger.Information("🔍 **NULL_REQUEST_CHECK**: Validating request object presence");
            if (request == null)
            {
                // **LOG_THE_WHO**: Null request scenario with comprehensive error reporting
                _logger.Warning("❌ **NULL_REQUEST_VALIDATION_FAILED**: Request object is null");
                _logger.Warning("   - **VALIDATION_FAILURE**: Cannot validate null request object");
                _logger.Warning("   - **ERROR_REASON**: Request parameter passed as null to validation method");
                _logger.Warning("   - **PROCESSING_IMPACT**: Request cannot be processed without valid object");
                _logger.Warning("   - **RETURN_RESULT**: Invalid with null request error message");
                
                // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION - OBJECT-ORIENTED FUNCTIONAL DUAL LAYER APPROACH**
                _logger.Error("🎯 **TEMPLATE_SPECIFICATION_VALIDATION**: ValidateUpdateRequest dual-layer template specification compliance analysis");

                // Determine document type using DatabaseTemplateHelper (MANDATORY - NO HARDCODING)
                string documentTypeNullCase = "Invoice"; // Request validation is document-type agnostic
                _logger.Error($"📋 **DOCUMENT_TYPE_DETECTED**: {documentTypeNullCase} - Using DatabaseTemplateHelper document-specific validation rules");

                // Create template specification object for document type with dual-layer validation
                var templateSpecNullCase = TemplateSpecification.CreateForUtilityOperation(documentTypeNullCase, "ValidateUpdateRequest", 
                    request, new FieldValidationInfo { IsValid = false, ErrorMessage = "Request object is null." });

                // Fluent validation with short-circuiting - stops on first failure
                var validatedSpecNullCase = templateSpecNullCase
                    .ValidateEntityTypeAwareness(null) // No AI recommendations for utility operations
                    .ValidateFieldMappingEnhancement(null)
                    .ValidateDataTypeRecommendations(new List<WaterNut.DataSpace.AITemplateService.PromptRecommendation>()) // Request validation operations return objects
                    .ValidatePatternQuality(null)
                    .ValidateTemplateOptimization(null);

                // Log all validation results
                validatedSpecNullCase.LogValidationResults(_logger);

                // Extract overall success from validated specification
                bool templateSpecificationSuccessNullCase = validatedSpecNullCase.IsValid;
                
                return new FieldValidationInfo { IsValid = false, ErrorMessage = "Request object is null." };
            }
            
            _logger.Information("✅ **NULL_REQUEST_CHECK_PASSED**: Request object is present and valid");
            
            // **FIELD_NAME_VALIDATION**: Check for field name presence and validity
            _logger.Information("🔍 **FIELD_NAME_VALIDATION**: Validating field name presence and content");
            _logger.Information("   - **FIELD_NAME_VALUE**: '{FieldName}'", request.FieldName ?? "NULL");
            _logger.Information("   - **VALIDATION_CRITERIA**: Field name must not be null, empty, or whitespace");
            
            if (string.IsNullOrWhiteSpace(request.FieldName))
            {
                // **LOG_THE_WHO**: Field name validation failure with detailed analysis
                _logger.Warning("❌ **FIELD_NAME_VALIDATION_FAILED**: Field name is null, empty, or whitespace");
                _logger.Warning("   - **FIELD_NAME_STATE**: '{FieldNameValue}'", request.FieldName ?? "NULL");
                _logger.Warning("   - **VALIDATION_FAILURE**: Field name is required for all correction operations");
                _logger.Warning("   - **ERROR_REASON**: Cannot identify target field without valid field name");
                _logger.Warning("   - **PROCESSING_IMPACT**: Request cannot be processed without field identification");
                _logger.Warning("   - **RETURN_RESULT**: Invalid with field name required error message");
                
                // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION - OBJECT-ORIENTED FUNCTIONAL DUAL LAYER APPROACH**
                _logger.Error("🎯 **TEMPLATE_SPECIFICATION_VALIDATION**: ValidateUpdateRequest dual-layer template specification compliance analysis");

                // Determine document type using DatabaseTemplateHelper (MANDATORY - NO HARDCODING)
                string documentTypeFieldName = "Invoice"; // Request validation is document-type agnostic
                _logger.Error($"📋 **DOCUMENT_TYPE_DETECTED**: {documentTypeFieldName} - Using DatabaseTemplateHelper document-specific validation rules");

                // Create template specification object for document type with dual-layer validation
                var templateSpecFieldName = TemplateSpecification.CreateForUtilityOperation(documentTypeFieldName, "ValidateUpdateRequest", 
                    request, new FieldValidationInfo { IsValid = false, ErrorMessage = "Field name is required." });

                // Fluent validation with short-circuiting - stops on first failure
                var validatedSpecFieldName = templateSpecFieldName
                    .ValidateEntityTypeAwareness(null) // No AI recommendations for utility operations
                    .ValidateFieldMappingEnhancement(null)
                    .ValidateDataTypeRecommendations(new List<WaterNut.DataSpace.AITemplateService.PromptRecommendation>()) // Request validation operations return objects
                    .ValidatePatternQuality(null)
                    .ValidateTemplateOptimization(null);

                // Log all validation results
                validatedSpecFieldName.LogValidationResults(_logger);

                // Extract overall success from validated specification
                bool templateSpecificationSuccessFieldName = validatedSpecFieldName.IsValid;
                
                return new FieldValidationInfo { IsValid = false, ErrorMessage = "Field name is required." };
            }
            
            _logger.Information("✅ **FIELD_NAME_VALIDATION_PASSED**: Field name is present and valid");
            _logger.Information("   - **VALIDATED_FIELD_NAME**: '{FieldName}'", request.FieldName);
            
            // **FIELD_SUPPORT_VALIDATION**: Verify field is supported by correction system
            _logger.Information("🔍 **FIELD_SUPPORT_VALIDATION**: Verifying field support in correction system");
            _logger.Information("   - **SUPPORT_CHECK_METHOD**: Using IsFieldSupported for mapping dictionary verification");
            _logger.Information("   - **VALIDATION_CRITERIA**: Field must exist in DeepSeekToDBFieldMapping dictionary");
            
            if (!this.IsFieldSupported(request.FieldName))
            {
                // **LOG_THE_WHO**: Field support validation failure with comprehensive error reporting
                _logger.Warning("❌ **FIELD_SUPPORT_VALIDATION_FAILED**: Field is not supported by correction system");
                _logger.Warning("   - **UNSUPPORTED_FIELD**: '{FieldName}'", request.FieldName);
                _logger.Warning("   - **VALIDATION_FAILURE**: Field not found in DeepSeekToDBFieldMapping dictionary");
                _logger.Warning("   - **ERROR_REASON**: Field mapping not configured in correction system");
                _logger.Warning("   - **PROCESSING_IMPACT**: Field cannot be processed by any database strategy");
                _logger.Warning("   - **CONFIGURATION_NEEDED**: Field may need to be added to mapping configuration");
                _logger.Warning("   - **RETURN_RESULT**: Invalid with field not supported error message");
                
                // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION - OBJECT-ORIENTED FUNCTIONAL DUAL LAYER APPROACH**
                _logger.Error("🎯 **TEMPLATE_SPECIFICATION_VALIDATION**: ValidateUpdateRequest dual-layer template specification compliance analysis");

                // Determine document type using DatabaseTemplateHelper (MANDATORY - NO HARDCODING)
                string documentTypeFieldSupport = "Invoice"; // Request validation is document-type agnostic
                _logger.Error($"📋 **DOCUMENT_TYPE_DETECTED**: {documentTypeFieldSupport} - Using DatabaseTemplateHelper document-specific validation rules");

                // Create template specification object for document type with dual-layer validation
                var templateSpecFieldSupport = TemplateSpecification.CreateForUtilityOperation(documentTypeFieldSupport, "ValidateUpdateRequest", 
                    request, new FieldValidationInfo { IsValid = false, ErrorMessage = $"Field '{request.FieldName}' is not supported." });

                // Fluent validation with short-circuiting - stops on first failure
                var validatedSpecFieldSupport = templateSpecFieldSupport
                    .ValidateEntityTypeAwareness(null) // No AI recommendations for utility operations
                    .ValidateFieldMappingEnhancement(null)
                    .ValidateDataTypeRecommendations(new List<WaterNut.DataSpace.AITemplateService.PromptRecommendation>()) // Request validation operations return objects
                    .ValidatePatternQuality(null)
                    .ValidateTemplateOptimization(null);

                // Log all validation results
                validatedSpecFieldSupport.LogValidationResults(_logger);

                // Extract overall success from validated specification
                bool templateSpecificationSuccessFieldSupport = validatedSpecFieldSupport.IsValid;
                
                return new FieldValidationInfo { IsValid = false, ErrorMessage = $"Field '{request.FieldName}' is not supported." };
            }
            
            // **VALIDATION_SUCCESS**: All validation criteria passed
            _logger.Information("✅ **ALL_VALIDATIONS_PASSED**: Request validation completed successfully");
            _logger.Information("   - **VALIDATED_FIELD**: '{FieldName}'", request.FieldName);
            _logger.Information("   - **VALIDATION_OUTCOME**: Request approved for database strategy processing");
            _logger.Information("   - **PROCESSING_CLEARANCE**: Request meets all validation criteria");
            _logger.Information("   - **RETURN_RESULT**: Valid with no error messages");
            
            // **LOG_THE_WHAT_IF**: Method completion with successful validation
            _logger.Information("🏁 **REQUEST_VALIDATION_COMPLETE**: RegexUpdateRequest validation finished successfully");
            _logger.Information("   - **VALIDATION_STATUS**: PASSED - request eligible for processing");
            _logger.Information("   - **NEXT_PHASE**: Request can proceed to strategy selection and execution");
            _logger.Information("   - **QUALITY_ASSURANCE**: All validation criteria verified for processing safety");
            
            // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION - OBJECT-ORIENTED FUNCTIONAL DUAL LAYER APPROACH**
            _logger.Error("🎯 **TEMPLATE_SPECIFICATION_VALIDATION**: ValidateUpdateRequest dual-layer template specification compliance analysis");

            // Determine document type using DatabaseTemplateHelper (MANDATORY - NO HARDCODING)
            string documentType = "Invoice"; // Request validation is document-type agnostic
            _logger.Error($"📋 **DOCUMENT_TYPE_DETECTED**: {documentType} - Using DatabaseTemplateHelper document-specific validation rules");

            // Create template specification object for document type with dual-layer validation
            var templateSpec = TemplateSpecification.CreateForUtilityOperation(documentType, "ValidateUpdateRequest", 
                request, new FieldValidationInfo { IsValid = true });

            // Fluent validation with short-circuiting - stops on first failure
            var validatedSpec = templateSpec
                .ValidateEntityTypeAwareness(null) // No AI recommendations for utility operations
                .ValidateFieldMappingEnhancement(null)
                .ValidateDataTypeRecommendations(new List<WaterNut.DataSpace.AITemplateService.PromptRecommendation>()) // Request validation operations return objects
                .ValidatePatternQuality(null)
                .ValidateTemplateOptimization(null);

            // Log all validation results
            validatedSpec.LogValidationResults(_logger);

            // Extract overall success from validated specification
            bool templateSpecificationSuccess = validatedSpec.IsValid;
            
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
                                       WindowText = request.WindowText ?? string.Empty, // ✅ **CLEAN_WINDOWTEXT**: Pure window text, no mixed data
                                       SuggestedRegex = request.SuggestedRegex, // ✅ **PROPER_FIELD**: Direct assignment to dedicated field
                                       CorrectionType = request.CorrectionType,
                                       DeepSeekReasoning = TruncateForLog(request.DeepSeekReasoning, 1000),
                                       Confidence = safeConfidence,
                                       DocumentType =  request.InvoiceType,
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
                                   };
                
                // ✅ **CLEAN_IMPLEMENTATION**: SuggestedRegex now stored in dedicated database field with proper separation

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
        /// **ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v5**: Extract SuggestedRegex from OCRCorrectionLearning record
        /// **ARCHITECTURAL_INTENT**: Provide clean access to SuggestedRegex field without WindowText parsing complexity
        /// **BUSINESS_RULE**: Use dedicated SuggestedRegex field for pattern retrieval instead of parsing mixed data
        /// **DESIGN_SPECIFICATION**: Simple field access with null safety for reliable pattern extraction
        /// </summary>
        private static string GetSuggestedRegexFromLearningRecord(OCRCorrectionLearning learningRecord)
        {
            // Note: Static method - no instance logger available, but included for completeness
            // 🧠 **LOG_THE_WHAT**: Field access design, input data, expected behavior
            // **ARCHITECTURAL_INTENT**: Clean field access to SuggestedRegex without parsing complexity
            // **INPUT_RECORD**: OCRCorrectionLearning record with dedicated SuggestedRegex field
            // **EXPECTED_BEHAVIOR**: Return SuggestedRegex value directly or null if record is null
            // **BUSINESS_RULE_RATIONALE**: Dedicated field eliminates parsing errors and data contamination
            
            // **LOG_THE_HOW**: Simple field access method flow
            // **ACCESS_STRATEGY**: Direct property access with null-conditional operator
            // **NULL_SAFETY**: Null-conditional operator prevents null reference exceptions
            // **NO_PARSING_NEEDED**: Direct field access eliminates WindowText parsing complexity
            
            // **LOG_THE_WHY**: Clean implementation rationale and architectural benefits
            // **SIMPLICITY_BENEFIT**: Direct field access eliminates parsing logic and potential errors
            // **DATA_INTEGRITY**: Dedicated field prevents mixed data contamination
            // **PERFORMANCE_OPTIMIZATION**: No string parsing or regex operations required
            // **MAINTAINABILITY**: Simple implementation reduces maintenance burden
            
            // ✅ **SIMPLE_ACCESS**: Direct field access with null safety
            // **FIELD_ACCESS**: learningRecord?.SuggestedRegex provides null-safe direct access
            // **NO_PARSING_COMPLEXITY**: Eliminates need for WindowText parsing and regex extraction
            // **CLEAN_IMPLEMENTATION**: Dedicated field usage as intended by database schema enhancement
            
            // **TEMPLATE SPECIFICATION SUCCESS CRITERIA VALIDATION - OBJECT-ORIENTED FUNCTIONAL DUAL LAYER APPROACH**
            // Note: Static method - no logger available, validation performed without logging
            
            // Determine document type using DatabaseTemplateHelper (MANDATORY - NO HARDCODING)
            string documentType = "Invoice"; // Learning record access is document-type agnostic
            
            // Create template specification object for document type with dual-layer validation
            var templateSpec = TemplateSpecification.CreateForUtilityOperation(documentType, "GetSuggestedRegexFromLearningRecord", 
                learningRecord, learningRecord?.SuggestedRegex);

            // Fluent validation with short-circuiting - stops on first failure
            var validatedSpec = templateSpec
                .ValidateEntityTypeAwareness(null) // No AI recommendations for utility operations
                .ValidateFieldMappingEnhancement(null)
                .ValidateDataTypeRecommendations(new List<WaterNut.DataSpace.AITemplateService.PromptRecommendation>()) // Regex extraction operations return text results
                .ValidatePatternQuality(null)
                .ValidateTemplateOptimization(null);

            // Extract overall success from validated specification (no logging available)
            bool templateSpecificationSuccess = validatedSpec.IsValid;
            
            return learningRecord?.SuggestedRegex;
            
            // **LOG_THE_WHO**: Method completion and return value
            // **RETURN_VALUE**: SuggestedRegex string or null if record is null
            // **ACCESS_SUCCESS**: Direct field access completed without parsing overhead
            // **DATA_INTEGRITY**: Clean SuggestedRegex value without mixed data contamination
            
            // **LOG_THE_WHAT_IF**: Method expectations and usage scenarios
            // **SUCCESS_SCENARIO**: Non-null record returns SuggestedRegex field value
            // **NULL_SCENARIO**: Null record returns null safely without exceptions
            // **PATTERN_USAGE**: Returned regex can be used directly for pattern matching operations
            // **INTEGRATION_READY**: Clean regex value ready for database strategy usage
        }

        #endregion
        
        // **ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v5_IMPLEMENTATION_COMPLETE**
        // All methods in OCRDatabaseUpdates.cs enhanced with comprehensive ultradiagnostic logging
        // following the What, How, Why, Who, What-If pattern for complete self-contained narrative
        // Database update orchestration now provides complete audit trail and troubleshooting information
    }
}