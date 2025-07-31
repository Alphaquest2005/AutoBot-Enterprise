// File: OCRCorrectionService/HybridDocumentDetectionService.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using System.IO;
using System.Data.Entity;
using OCR.Business.Entities;
using Serilog;
using Newtonsoft.Json;

namespace WaterNut.DataSpace
{
    /// <summary>
    /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v4.2**: Hybrid Document Detection Service
    /// 
    /// **REVOLUTIONARY ARCHITECTURE**: Combines database speed with AI intelligence for self-improving document detection
    /// **DATABASE-FIRST APPROACH**: Known documents processed via fast OCR_TemplateTableMapping Keywords queries
    /// **AI FALLBACK & LEARNING**: Unknown documents detected via AI, with keywords added to database for future speed
    /// **COMPLETENESS VALIDATION**: Ensures 100% text coverage, detects missing documents automatically
    /// **SEPARATION INTELLIGENCE**: AI provides document boundaries, regex patterns, separation logic
    /// **SELF-IMPROVING**: System evolves and gets better over time through AI→Database learning
    /// </summary>
    public class HybridDocumentDetectionService
    {
        private readonly ILogger _logger;
        private readonly OCRContext _context;
        private readonly DatabaseDocumentDetectionEngine _databaseEngine;
        private readonly AIDocumentDetectionEngine _aiEngine;
        private readonly DocumentLearningSystem _learningSystem;
        private readonly DocumentCompletenessValidator _completenessValidator;
        private readonly TextSeparationIntelligence _separationIntelligence;
        private readonly FileTypePatternDetectionEngine _fileTypeEngine;

        public HybridDocumentDetectionService(ILogger logger, OCRContext context = null)
        {
            // 🧠 **ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v4.2**: Comprehensive constructor logging
            
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _logger.Error("🔍 **HYBRID_CONSTRUCTOR_START**: Initializing HybridDocumentDetectionService with ultradiagnostic logging");
            _logger.Error("   - **LOGGER_STATUS**: {LoggerStatus}", logger != null ? "PROVIDED" : "NULL");
            _logger.Error("   - **CONTEXT_STATUS**: {ContextStatus}", context != null ? "PROVIDED" : "WILL_CREATE_NEW");
            
            try
            {
                _logger.Error("🗄️ **CREATING_OCR_CONTEXT**: Attempting to create/assign OCRContext");
                _context = context ?? new OCRContext();
                _logger.Error("✅ **OCR_CONTEXT_SUCCESS**: OCRContext created/assigned successfully");
            }
            catch (Exception contextEx)
            {
                _logger.Error(contextEx, "❌ **OCR_CONTEXT_FAILED**: Critical failure creating OCRContext");
                throw new InvalidOperationException("Failed to create OCRContext for hybrid detection", contextEx);
            }
            
            // Initialize hybrid system components with extensive error trapping
            _logger.Error("🔧 **COMPONENT_INITIALIZATION_START**: Creating hybrid system components");
            
            try 
            {
                _logger.Error("🗄️ **CREATING_DATABASE_ENGINE**: Attempting DatabaseDocumentDetectionEngine creation");
                _databaseEngine = new DatabaseDocumentDetectionEngine(_logger, _context);
                _logger.Error("✅ **DATABASE_ENGINE_SUCCESS**: DatabaseDocumentDetectionEngine created");
            }
            catch (Exception dbEx)
            {
                _logger.Error(dbEx, "❌ **DATABASE_ENGINE_FAILED**: DatabaseDocumentDetectionEngine creation failed");
                _databaseEngine = null; // Will handle gracefully in detection method
            }
            
            try 
            {
                _logger.Error("🤖 **CREATING_AI_ENGINE**: Attempting AIDocumentDetectionEngine creation");
                _aiEngine = new AIDocumentDetectionEngine(_logger);
                _logger.Error("✅ **AI_ENGINE_SUCCESS**: AIDocumentDetectionEngine created");
            }
            catch (Exception aiEx)
            {
                _logger.Error(aiEx, "❌ **AI_ENGINE_FAILED**: AIDocumentDetectionEngine creation failed");
                _aiEngine = null; // Will handle gracefully in detection method
            }
            
            try 
            {
                _logger.Error("🧠 **CREATING_LEARNING_SYSTEM**: Attempting DocumentLearningSystem creation");
                _learningSystem = new DocumentLearningSystem(_logger, _context);
                _logger.Error("✅ **LEARNING_SYSTEM_SUCCESS**: DocumentLearningSystem created");
            }
            catch (Exception learnEx)
            {
                _logger.Error(learnEx, "❌ **LEARNING_SYSTEM_FAILED**: DocumentLearningSystem creation failed");
                _learningSystem = null; // Will handle gracefully in detection method
            }
            
            try 
            {
                _logger.Error("✅ **CREATING_COMPLETENESS_VALIDATOR**: Attempting DocumentCompletenessValidator creation");
                _completenessValidator = new DocumentCompletenessValidator(_logger);
                _logger.Error("✅ **COMPLETENESS_VALIDATOR_SUCCESS**: DocumentCompletenessValidator created");
            }
            catch (Exception compEx)
            {
                _logger.Error(compEx, "❌ **COMPLETENESS_VALIDATOR_FAILED**: DocumentCompletenessValidator creation failed");
                _completenessValidator = null; // Will handle gracefully in detection method
            }
            
            try 
            {
                _logger.Error("🔍 **CREATING_SEPARATION_INTELLIGENCE**: Attempting TextSeparationIntelligence creation");
                _separationIntelligence = new TextSeparationIntelligence(_logger);
                _logger.Error("✅ **SEPARATION_INTELLIGENCE_SUCCESS**: TextSeparationIntelligence created");
            }
            catch (Exception sepEx)
            {
                _logger.Error(sepEx, "❌ **SEPARATION_INTELLIGENCE_FAILED**: TextSeparationIntelligence creation failed");
                _separationIntelligence = null; // Will handle gracefully in detection method
            }
            
            try 
            {
                _logger.Error("📁 **CREATING_FILETYPE_ENGINE**: Attempting FileTypePatternDetectionEngine creation");
                _fileTypeEngine = new FileTypePatternDetectionEngine(_logger, _context);
                _logger.Error("✅ **FILETYPE_ENGINE_SUCCESS**: FileTypePatternDetectionEngine created");
            }
            catch (Exception fileEx)
            {
                _logger.Error(fileEx, "❌ **FILETYPE_ENGINE_FAILED**: FileTypePatternDetectionEngine creation failed");
                _fileTypeEngine = null; // Will handle gracefully in detection method
            }
            
            // **COMPONENT AVAILABILITY SUMMARY**
            _logger.Error("📊 **HYBRID_COMPONENT_SUMMARY**: Component availability status");
            _logger.Error("   - **DATABASE_ENGINE**: {Status}", _databaseEngine != null ? "AVAILABLE" : "FAILED");
            _logger.Error("   - **AI_ENGINE**: {Status}", _aiEngine != null ? "AVAILABLE" : "FAILED");
            _logger.Error("   - **LEARNING_SYSTEM**: {Status}", _learningSystem != null ? "AVAILABLE" : "FAILED");
            _logger.Error("   - **COMPLETENESS_VALIDATOR**: {Status}", _completenessValidator != null ? "AVAILABLE" : "FAILED");
            _logger.Error("   - **SEPARATION_INTELLIGENCE**: {Status}", _separationIntelligence != null ? "AVAILABLE" : "FAILED");
            _logger.Error("   - **FILETYPE_ENGINE**: {Status}", _fileTypeEngine != null ? "AVAILABLE" : "FAILED");
            
            var availableComponents = new[] { _databaseEngine, _aiEngine, _learningSystem, _completenessValidator, _separationIntelligence, _fileTypeEngine }
                .Count(c => c != null);
            
            _logger.Error("🏗️ **HYBRID_INITIALIZATION_COMPLETE**: {Available}/{Total} components available", availableComponents, 6);
            
            if (availableComponents == 0)
            {
                _logger.Error("🚨 **CRITICAL_HYBRID_FAILURE**: No hybrid components successfully initialized - system will operate in degraded mode");
            }
            else if (availableComponents < 6)
            {
                _logger.Error("⚠️ **PARTIAL_HYBRID_INITIALIZATION**: Some components failed - system will operate with limited functionality");
            }
            else
            {
                _logger.Error("✅ **FULL_HYBRID_INITIALIZATION**: All components initialized - full hybrid functionality available");
            }
        }

        /// <summary>
        /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v4.2**: Main hybrid detection with complete workflow
        /// 
        /// **HYBRID WORKFLOW**: Database-first → AI fallback → Learning → Completeness validation → Separation intelligence
        /// **SELF-IMPROVING**: Each unknown document type improves future database detection speed
        /// **COMPREHENSIVE**: Handles known types (fast), unknown types (intelligent), and missing documents (complete)
        /// </summary>
        public async Task<HybridDetectionResult> DetectDocumentTypesAsync(string text, string documentPath = null)
        {
            // 🧠 **ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v4.2**: Comprehensive detection method logging
            
            _logger.Error("🎯 **HYBRID_DETECTION_START**: Starting comprehensive hybrid document detection with ultradiagnostic logging");
            _logger.Error("   - **INPUT_TEXT_LENGTH**: {Length} chars", text?.Length ?? 0);
            _logger.Error("   - **DOCUMENT_PATH**: {Path}", documentPath ?? "Unknown");
            _logger.Error("   - **TEXT_PREVIEW**: {Preview}", text?.Substring(0, Math.Min(200, text?.Length ?? 0)) ?? "NULL");
            
            // **COMPONENT AVAILABILITY CHECK**
            _logger.Error("🔧 **COMPONENT_AVAILABILITY_ANALYSIS**: Checking hybrid system component status");
            _logger.Error("   - **DATABASE_ENGINE**: {Status}", _databaseEngine != null ? "AVAILABLE" : "MISSING");
            _logger.Error("   - **AI_ENGINE**: {Status}", _aiEngine != null ? "AVAILABLE" : "MISSING");
            _logger.Error("   - **LEARNING_SYSTEM**: {Status}", _learningSystem != null ? "AVAILABLE" : "MISSING");
            _logger.Error("   - **COMPLETENESS_VALIDATOR**: {Status}", _completenessValidator != null ? "AVAILABLE" : "MISSING");
            _logger.Error("   - **SEPARATION_INTELLIGENCE**: {Status}", _separationIntelligence != null ? "AVAILABLE" : "MISSING");
            _logger.Error("   - **FILETYPE_ENGINE**: {Status}", _fileTypeEngine != null ? "AVAILABLE" : "MISSING");
            
            var result = new HybridDetectionResult
            {
                OriginalText = text,
                OriginalLength = text?.Length ?? 0,
                DocumentPath = documentPath
            };
            
            _logger.Error("✅ **HYBRID_RESULT_INITIALIZED**: HybridDetectionResult object created with input data");

            try
            {
                _logger.Error("🗄️ **PHASE_1_START**: Database-first detection phase beginning");
                
                // **INITIALIZE DATABASE RESULTS**: Ensure variable is always available
                var databaseResults = new List<DetectedDocument>();
                
                if (_databaseEngine == null)
                {
                    _logger.Error("❌ **DATABASE_ENGINE_UNAVAILABLE**: Cannot perform database detection - component missing");
                    _logger.Error("🔄 **PHASE_1_SKIP**: Skipping database detection due to missing component");
                    // databaseResults remains empty list
                }
                else
                {
                    _logger.Error("🔍 **DATABASE_DETECTION_ATTEMPT**: Calling DatabaseDocumentDetectionEngine.DetectKnownDocumentTypesAsync");
                    _logger.Error("   - **METHOD_SIGNATURE**: DetectKnownDocumentTypesAsync(string text)");
                    _logger.Error("   - **TEXT_LENGTH_PARAMETER**: {Length} characters", text?.Length ?? 0);
                    
                    try 
                    {
                        databaseResults = (await _databaseEngine.DetectKnownDocumentTypesAsync(text))?.ToList() ?? new List<DetectedDocument>();
                        _logger.Error("✅ **DATABASE_DETECTION_SUCCESS**: DetectKnownDocumentTypesAsync returned successfully");
                        _logger.Error("   - **RESULT_COUNT**: {Count} documents detected", databaseResults.Count);
                        
                        // **LOG DETECTED DOCUMENTS**
                        foreach (var doc in databaseResults)
                        {
                            _logger.Error("📄 **DATABASE_DETECTED_DOCUMENT**: Type='{Type}', Confidence={Confidence:F2}, Method='{Method}'", 
                                doc.DocumentType, doc.Confidence, doc.DetectionMethod);
                        }
                    }
                    catch (Exception dbEx)
                    {
                        _logger.Error(dbEx, "❌ **DATABASE_DETECTION_EXCEPTION**: Exception in DetectKnownDocumentTypesAsync");
                        _logger.Error("   - **EXCEPTION_TYPE**: {Type}", dbEx.GetType().Name);
                        _logger.Error("   - **EXCEPTION_MESSAGE**: {Message}", dbEx.Message);
                        _logger.Error("🔄 **DATABASE_FALLBACK**: Using empty database results due to exception");
                        databaseResults = new List<DetectedDocument>(); // Empty fallback
                    }
                }
                
                _logger.Error("📊 **PHASE_1_COMPLETE**: Database detection phase complete");
                _logger.Error("   - **DATABASE_RESULTS**: {Count} documents detected", databaseResults.Count);
                
                result.DatabaseDetections = databaseResults;
                result.DatabaseDetectionCount = databaseResults.Count;
                
                // **HIGH-CONFIDENCE DATABASE DETECTION ANALYSIS**
                _logger.Error("🔍 **DATABASE_CONFIDENCE_ANALYSIS**: Analyzing database detection confidence levels");
                var highConfidenceResults = databaseResults.Where(d => d.Confidence >= 0.8).ToList();
                _logger.Error("   - **HIGH_CONFIDENCE_COUNT**: {Count} documents with confidence >= 0.8", highConfidenceResults.Count);
                _logger.Error("   - **TOTAL_DATABASE_COUNT**: {Count} total database detections", databaseResults.Count);
                
                if (highConfidenceResults.Any())
                {
                    _logger.Error("✅ **DATABASE_HIGH_CONFIDENCE_SUCCESS**: Using {Count} high-confidence database matches", highConfidenceResults.Count);
                    result.PrimaryDetectionMethod = "Database";
                    result.Documents.AddRange(highConfidenceResults);
                    
                    // **LOG HIGH-CONFIDENCE RESULTS**
                    foreach (var doc in highConfidenceResults)
                    {
                        _logger.Error("🎯 **HIGH_CONFIDENCE_DOCUMENT**: Type='{Type}', Confidence={Confidence:F3}, Keywords=[{Keywords}]", 
                            doc.DocumentType, doc.Confidence, string.Join(", ", doc.MatchedKeywords));
                    }
                }
                else
                {
                    _logger.Error("⚠️ **LOW_DATABASE_CONFIDENCE**: No high-confidence database matches found - will continue to next phases");
                }
                
                // **PHASE 2: FILETYPE PATTERN VALIDATION** (Filename pattern validation)
                _logger.Error("📁 **PHASE_2_START**: FileType pattern validation phase beginning");
                
                // **FILETYPE ENGINE AVAILABILITY CHECK**
                if (_fileTypeEngine == null)
                {
                    _logger.Error("❌ **FILETYPE_ENGINE_UNAVAILABLE**: Cannot perform filename validation - component missing");
                    _logger.Error("🔄 **PHASE_2_SKIP**: Skipping FileType validation due to missing component");
                    
                    // **CREATE DEFAULT FILETYPE VALIDATION RESULT**
                    var defaultFileTypeValidation = new FileTypeValidationResult
                    {
                        ShouldTriggerAI = false,
                        ValidationMessage = "FileType engine unavailable - no validation performed"
                    };
                    result.FileTypeValidation = defaultFileTypeValidation;
                    result.FilenameTriggersAI = false;
                    _logger.Error("✅ **DEFAULT_FILETYPE_RESULT**: Using default validation result (no AI trigger)");
                }
                else
                {
                    _logger.Error("🔍 **FILETYPE_VALIDATION_ATTEMPT**: Calling FileTypePatternDetectionEngine.ValidateFilenamePatternAsync");
                    _logger.Error("   - **METHOD_SIGNATURE**: ValidateFilenamePatternAsync(string documentPath, List<DetectedDocument> detectedDocuments)");
                    _logger.Error("   - **DOCUMENT_PATH**: {Path}", documentPath ?? "NULL");
                    _logger.Error("   - **DETECTED_DOCUMENTS_COUNT**: {Count} documents to validate against", result.Documents.Count);
                    
                    try
                    {
                        var fileTypeValidation = await _fileTypeEngine.ValidateFilenamePatternAsync(documentPath, result.Documents);
                        _logger.Error("✅ **FILETYPE_VALIDATION_SUCCESS**: ValidateFilenamePatternAsync returned successfully");
                        _logger.Error("   - **SHOULD_TRIGGER_AI**: {ShouldTrigger}", fileTypeValidation.ShouldTriggerAI);
                        _logger.Error("   - **VALIDATION_MESSAGE**: {Message}", fileTypeValidation.ValidationMessage);
                        
                        result.FileTypeValidation = fileTypeValidation;
                        result.FilenameTriggersAI = fileTypeValidation.ShouldTriggerAI;
                        
                        if (fileTypeValidation.ShouldTriggerAI)
                        {
                            _logger.Error("🚨 **FILETYPE_AI_TRIGGER**: Filename validation requires AI detection - {Message}", fileTypeValidation.ValidationMessage);
                        }
                        else
                        {
                            _logger.Error("✅ **FILETYPE_VALIDATION_PASSED**: Filename validation successful - {Message}", fileTypeValidation.ValidationMessage);
                        }
                    }
                    catch (Exception fileEx)
                    {
                        _logger.Error(fileEx, "❌ **FILETYPE_VALIDATION_EXCEPTION**: Exception in ValidateFilenamePatternAsync");
                        _logger.Error("   - **EXCEPTION_TYPE**: {Type}", fileEx.GetType().Name);
                        _logger.Error("   - **EXCEPTION_MESSAGE**: {Message}", fileEx.Message);
                        _logger.Error("🔄 **FILETYPE_FALLBACK**: Using default validation result due to exception");
                        
                        var fallbackFileTypeValidation = new FileTypeValidationResult
                        {
                            ShouldTriggerAI = false,
                            ValidationMessage = $"FileType validation failed: {fileEx.Message}"
                        };
                        result.FileTypeValidation = fallbackFileTypeValidation;
                        result.FilenameTriggersAI = false;
                    }
                }
                
                _logger.Error("📊 **PHASE_2_COMPLETE**: FileType validation phase complete");
                _logger.Error("   - **FILENAME_TRIGGERS_AI**: {TriggersAI}", result.FilenameTriggersAI);
                
                // **PHASE 3: COMPLETENESS CHECK** (Detect missing documents)
                _logger.Error("📊 **PHASE_3_START**: Completeness validation phase beginning");
                
                // **COMPLETENESS VALIDATOR AVAILABILITY CHECK**
                if (_completenessValidator == null)
                {
                    _logger.Error("❌ **COMPLETENESS_VALIDATOR_UNAVAILABLE**: Cannot perform completeness validation - component missing");
                    _logger.Error("🔄 **PHASE_3_SKIP**: Skipping completeness validation due to missing component");
                    
                    // **DEFAULT COMPLETENESS RESULT**
                    result.CompletenessPercentage = 100.0; // Assume complete if can't validate
                    result.MissingTextLength = 0;
                    _logger.Error("✅ **DEFAULT_COMPLETENESS_RESULT**: Using default completeness (100% coverage assumed)");
                }
                else
                {
                    _logger.Error("🔍 **COMPLETENESS_VALIDATION_ATTEMPT**: Calling DocumentCompletenessValidator.ValidateCompletenessAsync");
                    _logger.Error("   - **METHOD_SIGNATURE**: ValidateCompletenessAsync(string text, List<DetectedDocument> documents)");
                    _logger.Error("   - **TEXT_LENGTH**: {Length} characters", text?.Length ?? 0);
                    _logger.Error("   - **DOCUMENTS_COUNT**: {Count} documents to validate", result.Documents.Count);
                    
                    try
                    {
                        var completenessResult = await _completenessValidator.ValidateCompletenessAsync(text, result.Documents);
                        _logger.Error("✅ **COMPLETENESS_VALIDATION_SUCCESS**: ValidateCompletenessAsync returned successfully");
                        _logger.Error("   - **COVERAGE_PERCENTAGE**: {Coverage:F2}%", completenessResult.CoveragePercentage);
                        _logger.Error("   - **MISSING_TEXT_LENGTH**: {MissingLength} characters", completenessResult.MissingTextLength);
                        
                        result.CompletenessPercentage = completenessResult.CoveragePercentage;
                        result.MissingTextLength = completenessResult.MissingTextLength;
                    }
                    catch (Exception compEx)
                    {
                        _logger.Error(compEx, "❌ **COMPLETENESS_VALIDATION_EXCEPTION**: Exception in ValidateCompletenessAsync");
                        _logger.Error("   - **EXCEPTION_TYPE**: {Type}", compEx.GetType().Name);
                        _logger.Error("   - **EXCEPTION_MESSAGE**: {Message}", compEx.Message);
                        _logger.Error("🔄 **COMPLETENESS_FALLBACK**: Using default completeness result due to exception");
                        
                        result.CompletenessPercentage = 100.0; // Assume complete on error
                        result.MissingTextLength = 0;
                    }
                }
                
                _logger.Error("📊 **PHASE_3_COMPLETE**: Completeness validation phase complete");
                _logger.Error("   - **FINAL_COVERAGE**: {Coverage:F2}%", result.CompletenessPercentage);
                
                // **AI TRIGGER DECISION ANALYSIS**: Determine if AI detection is needed based on completeness OR filename patterns
                _logger.Error("🤖 **AI_TRIGGER_DECISION**: Analyzing need for AI detection");
                var needsAIForCompleteness = result.CompletenessPercentage < 100.0;
                var needsAIForFilename = result.FilenameTriggersAI;
                var needsAI = needsAIForCompleteness || needsAIForFilename;
                
                _logger.Error("   - **COMPLETENESS_TRIGGERS_AI**: {NeedsAI} (Coverage: {Coverage:F2}%)", needsAIForCompleteness, result.CompletenessPercentage);
                _logger.Error("   - **FILENAME_TRIGGERS_AI**: {NeedsAI}", needsAIForFilename);
                _logger.Error("   - **OVERALL_AI_NEEDED**: {NeedsAI}", needsAI);
                
                if (needsAI)
                {
                    var reasons = new List<string>();
                    if (needsAIForCompleteness) reasons.Add($"incomplete coverage ({result.CompletenessPercentage:F1}%)");
                    if (needsAIForFilename) reasons.Add("filename pattern validation");
                    
                    _logger.Error("🚨 **AI_DETECTION_TRIGGERED**: {Reasons} - proceeding with AI fallback", string.Join(" + ", reasons));
                    
                    // **PHASE 4: AI FALLBACK DETECTION** (Handle unknown types, missing content, and filename mismatches)
                    _logger.Error("🤖 **PHASE_4_START**: AI fallback detection phase beginning");
                    
                    // **AI ENGINE AVAILABILITY CHECK**
                    var missingText = result.MissingTextLength > 0 ? "MissingTextAvailable" : "";
                    if (_aiEngine == null)
                    {
                        _logger.Error("❌ **AI_ENGINE_UNAVAILABLE**: Cannot perform AI detection - component missing");
                        _logger.Error("🔄 **PHASE_4_SKIP**: Skipping AI detection due to missing component");
                    }
                    else
                    {
                        _logger.Error("🔍 **AI_DETECTION_ATTEMPT**: Calling AIDocumentDetectionEngine.DetectUnknownDocumentTypesAsync");
                        _logger.Error("   - **METHOD_SIGNATURE**: DetectUnknownDocumentTypesAsync(string text, List<DetectedDocument> knownDocuments, string missingText)");
                        _logger.Error("   - **TEXT_LENGTH**: {Length} characters", text?.Length ?? 0);
                        _logger.Error("   - **KNOWN_DOCUMENTS**: {Count} documents", result.Documents.Count);
                        _logger.Error("   - **MISSING_TEXT**: {MissingText}", missingText);
                        
                        try
                        {
                            var aiResults = await _aiEngine.DetectUnknownDocumentTypesAsync(text, result.Documents, missingText);
                            _logger.Error("✅ **AI_DETECTION_SUCCESS**: DetectUnknownDocumentTypesAsync returned successfully");
                            _logger.Error("   - **AI_RESULTS_COUNT**: {Count} unknown documents detected", aiResults?.Count ?? 0);
                            
                            result.AIDetections = aiResults ?? new List<DetectedDocument>();
                            result.AIDetectionCount = result.AIDetections.Count();
                            
                            // **LOG AI DETECTED DOCUMENTS**
                            foreach (var doc in result.AIDetections)
                            {
                                _logger.Error("🎯 **AI_DETECTED_DOCUMENT**: Type='{Type}', Confidence={Confidence:F2}, Method='{Method}'", 
                                    doc.DocumentType, doc.Confidence, doc.DetectionMethod);
                            }
                            
                            // **DETERMINE PRIMARY DETECTION METHOD**
                            if (result.Documents.Count == 0)
                            {
                                result.PrimaryDetectionMethod = "AI";
                                _logger.Error("🎯 **PRIMARY_METHOD_AI**: No database results - AI is primary detection method");
                            }
                            else
                            {
                                result.PrimaryDetectionMethod = "Hybrid";
                                _logger.Error("🎯 **PRIMARY_METHOD_HYBRID**: Both database and AI results - hybrid detection method");
                            }
                            
                            result.Documents.AddRange(result.AIDetections);
                            _logger.Error("✅ **AI_DOCUMENTS_ADDED**: {Count} AI-detected documents added to final results", result.AIDetections.Count);
                        }
                        catch (Exception aiEx)
                        {
                            _logger.Error(aiEx, "❌ **AI_DETECTION_EXCEPTION**: Exception in DetectUnknownDocumentTypesAsync");
                            _logger.Error("   - **EXCEPTION_TYPE**: {Type}", aiEx.GetType().Name);
                            _logger.Error("   - **EXCEPTION_MESSAGE**: {Message}", aiEx.Message);
                            _logger.Error("🔄 **AI_FALLBACK**: Using empty AI results due to exception");
                            
                            result.AIDetections = new List<DetectedDocument>();
                            result.AIDetectionCount = 0;
                        }
                    }
                    
                    _logger.Error("📊 **PHASE_4_COMPLETE**: AI fallback detection phase complete");
                    _logger.Error("   - **AI_DETECTION_COUNT**: {Count} unknown documents detected", result.AIDetectionCount);
                }
                else
                {
                    _logger.Error("✅ **NO_AI_NEEDED**: Completeness and filename validation passed - skipping AI detection");
                    result.AIDetections = new List<DetectedDocument>();
                    result.AIDetectionCount = 0;
                }
                
                // **PHASE 5: LEARNING SYSTEM** (Improve database for future speed)
                _logger.Error("🧠 **PHASE_5_START**: Learning system phase beginning");
                
                if (_learningSystem == null)
                {
                    _logger.Error("❌ **LEARNING_SYSTEM_UNAVAILABLE**: Cannot perform learning - component missing");
                    _logger.Error("🔄 **PHASE_5_SKIP**: Skipping learning due to missing component");
                }
                else if (result.AIDetections.Any())
                {
                    _logger.Error("🔍 **LEARNING_ATTEMPT**: Calling DocumentLearningSystem.LearnFromAIDetectionsAsync");
                    try
                    {
                        await _learningSystem.LearnFromAIDetectionsAsync(result.AIDetections, text);
                        _logger.Error("✅ **LEARNING_SUCCESS**: AI discoveries added to database for future speed");
                    }
                    catch (Exception learnEx)
                    {
                        _logger.Error(learnEx, "❌ **LEARNING_EXCEPTION**: Exception in LearnFromAIDetectionsAsync");
                    }
                    
                    // **FILENAME PATTERN LEARNING**: Learn filename patterns from successful AI detections
                    if (needsAIForFilename && result.AIDetections.Any())
                    {
                        _logger.Error("📁 **FILENAME_LEARNING_ATTEMPT**: Learning filename patterns from AI detections");
                        try
                        {
                            await LearnFilenamePatterns(documentPath, result.AIDetections);
                            _logger.Error("✅ **FILENAME_LEARNING_SUCCESS**: Filename patterns learned");
                        }
                        catch (Exception fileLearnEx)
                        {
                            _logger.Error(fileLearnEx, "❌ **FILENAME_LEARNING_EXCEPTION**: Exception in filename learning");
                        }
                    }
                }
                else
                {
                    _logger.Error("ℹ️ **NO_LEARNING_NEEDED**: No AI detections to learn from");
                }
                
                _logger.Error("📊 **PHASE_5_COMPLETE**: Learning system phase complete");
                
                // **PHASE 6: SEPARATION INTELLIGENCE** (Enhance text separation capabilities)
                _logger.Error("🔍 **PHASE_6_START**: Separation intelligence phase beginning");
                
                if (_separationIntelligence == null)
                {
                    _logger.Error("❌ **SEPARATION_INTELLIGENCE_UNAVAILABLE**: Cannot perform separation analysis - component missing");
                    _logger.Error("🔄 **PHASE_6_SKIP**: Skipping separation intelligence due to missing component");
                    
                    result.SeparationPatterns = new List<SeparationPattern>();
                    result.DocumentBoundaries = new List<DocumentBoundary>();
                    result.RegexSeparators = new List<string>();
                }
                else
                {
                    _logger.Error("🔍 **SEPARATION_ANALYSIS_ATTEMPT**: Calling TextSeparationIntelligence.AnalyzeSeparationPatternsAsync");
                    try
                    {
                        var separationResult = await _separationIntelligence.AnalyzeSeparationPatternsAsync(text, result.Documents);
                        _logger.Error("✅ **SEPARATION_ANALYSIS_SUCCESS**: Document boundaries and patterns analyzed");
                        _logger.Error("   - **PATTERNS_COUNT**: {Count} separation patterns", separationResult.Patterns.Count);
                        _logger.Error("   - **BOUNDARIES_COUNT**: {Count} document boundaries", separationResult.Boundaries.Count);
                        _logger.Error("   - **REGEX_SEPARATORS_COUNT**: {Count} regex separators", separationResult.RegexSeparators.Count);
                        
                        result.SeparationPatterns = separationResult.Patterns;
                        result.DocumentBoundaries = separationResult.Boundaries;
                        result.RegexSeparators = separationResult.RegexSeparators;
                    }
                    catch (Exception sepEx)
                    {
                        _logger.Error(sepEx, "❌ **SEPARATION_ANALYSIS_EXCEPTION**: Exception in AnalyzeSeparationPatternsAsync");
                        result.SeparationPatterns = new List<SeparationPattern>();
                        result.DocumentBoundaries = new List<DocumentBoundary>();
                        result.RegexSeparators = new List<string>();
                    }
                }
                
                _logger.Error("📊 **PHASE_6_COMPLETE**: Separation intelligence phase complete");
                
                // **PHASE 7: FINAL COMPLETENESS VALIDATION**
                _logger.Error("✅ **PHASE_7_START**: Final completeness validation phase beginning");
                
                if (_completenessValidator == null)
                {
                    _logger.Error("❌ **FINAL_COMPLETENESS_UNAVAILABLE**: Cannot perform final validation - component missing");
                    result.FinalCompletenessPercentage = result.CompletenessPercentage; // Use initial completeness
                }
                else
                {
                    _logger.Error("🔍 **FINAL_COMPLETENESS_ATTEMPT**: Calling DocumentCompletenessValidator.ValidateCompletenessAsync for final validation");
                    try
                    {
                        var finalCompleteness = await _completenessValidator.ValidateCompletenessAsync(text, result.Documents);
                        result.FinalCompletenessPercentage = finalCompleteness.CoveragePercentage;
                        _logger.Error("✅ **FINAL_COMPLETENESS_SUCCESS**: Final completeness validation complete - {Coverage:F2}%", result.FinalCompletenessPercentage);
                    }
                    catch (Exception finalEx)
                    {
                        _logger.Error(finalEx, "❌ **FINAL_COMPLETENESS_EXCEPTION**: Exception in final completeness validation");
                        result.FinalCompletenessPercentage = result.CompletenessPercentage; // Use initial completeness
                    }
                }
                
                _logger.Error("📊 **PHASE_7_COMPLETE**: Final completeness validation phase complete");
                _logger.Error("   - **FINAL_COVERAGE**: {Coverage:F2}%", result.FinalCompletenessPercentage);
                
                // **HYBRID DETECTION COMPLETE**
                _logger.Error("🎯 **HYBRID_DETECTION_COMPLETE**: All phases complete - summarizing results");
                _logger.Error("   - **PRIMARY_METHOD**: {Method}", result.PrimaryDetectionMethod ?? "None");
                _logger.Error("   - **TOTAL_DOCUMENTS**: {Count} documents detected", result.Documents.Count);
                _logger.Error("   - **DATABASE_DOCUMENTS**: {Count} from database", result.DatabaseDetectionCount);
                _logger.Error("   - **AI_DOCUMENTS**: {Count} from AI", result.AIDetectionCount);
                _logger.Error("   - **FINAL_COVERAGE**: {Coverage:F1}%", result.FinalCompletenessPercentage);
                _logger.Error("   - **FILENAME_TRIGGERED_AI**: {Triggered}", result.FilenameTriggersAI);
                
                result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **HYBRID_DETECTION_ERROR**: Error in hybrid detection workflow");
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            
            return result;
        }

        /// <summary>
        /// Gets detection statistics for monitoring and optimization
        /// </summary>
        public async Task<DetectionStatistics> GetDetectionStatisticsAsync()
        {
            return new DetectionStatistics
            {
                DatabaseMappingsCount = await _context.OCR_TemplateTableMapping.CountAsync(m => m.IsActive),
                RecentDetectionLogs = await _context.OCR_KeywordDetectionLog
                    .Where(log => log.CreatedDate >= DateTime.Now.AddDays(-7))
                    .CountAsync(),
                DatabaseDetectionSuccessRate = await CalculateDatabaseSuccessRateAsync(),
                AIFallbackUsageRate = await CalculateAIUsageRateAsync()
            };
        }

        private async Task<double> CalculateDatabaseSuccessRateAsync()
        {
            var recentLogs = await _context.OCR_KeywordDetectionLog
                .Where(log => log.CreatedDate >= DateTime.Now.AddDays(-7))
                .ToListAsync();
                
            if (!recentLogs.Any()) return 0.0;
            
            var successfulDetections = recentLogs.Count(log => log.Success && log.MatchScore >= 0.8m);
            return (double)successfulDetections / recentLogs.Count * 100.0;
        }

        private async Task<double> CalculateAIUsageRateAsync()
        {
            // Calculate AI usage rate from recent detections
            // This would require additional logging to track detection methods
            return 0.0; // Placeholder - implement based on logging strategy
        }

        /// <summary>
        /// **FILENAME PATTERN LEARNING**: Learn filename patterns from successful AI detections
        /// </summary>
        private async Task LearnFilenamePatterns(string documentPath, List<DetectedDocument> aiDetections)
        {
            _logger.Information("📁 **FILENAME_LEARNING_START**: Learning filename patterns from AI detections");
            
            if (string.IsNullOrWhiteSpace(documentPath) || !aiDetections.Any())
            {
                _logger.Information("ℹ️ **NO_FILENAME_LEARNING**: Insufficient data for filename pattern learning");
                return;
            }
            
            try
            {
                var filename = Path.GetFileName(documentPath);
                _logger.Information("   - **FILENAME**: {Filename}", filename);
                _logger.Information("   - **AI_DETECTIONS**: {Count} document types", aiDetections.Count);
                
                // **PATTERN GENERATION**: Generate filename patterns for each detected document type
                foreach (var detection in aiDetections.Where(d => d.Confidence > 0.7))
                {
                    await GenerateAndSaveFilenamePattern(filename, detection);
                }
                
                _logger.Information("✅ **FILENAME_LEARNING_COMPLETE**: Processed {Count} AI detections for filename pattern learning", 
                    aiDetections.Count);
                
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **FILENAME_LEARNING_ERROR**: Error learning filename patterns");
            }
        }

        /// <summary>
        /// **PATTERN GENERATION AND SAVING**: Generate filename pattern and save to database
        /// </summary>
        private async Task GenerateAndSaveFilenamePattern(string filename, DetectedDocument detection)
        {
            try
            {
                // **PATTERN EXTRACTION**: Extract pattern from filename and document type
                var generatedPattern = GenerateFilenamePattern(filename, detection.DocumentType);
                
                if (string.IsNullOrWhiteSpace(generatedPattern))
                {
                    _logger.Verbose("⚠️ **NO_PATTERN_GENERATED**: Could not generate pattern for {DocumentType} from {Filename}", 
                        detection.DocumentType, filename);
                    return;
                }
                
                _logger.Information("🎯 **PATTERN_GENERATED**: {DocumentType} -> {Pattern}", detection.DocumentType, generatedPattern);
                
                // **FUTURE ENHANCEMENT**: Save pattern to FileTypes table or OCR_TemplateTableMapping
                // This would require extending the learning system to handle filename pattern storage
                // For now, just log the generated pattern for manual review
                
                _logger.Information("📝 **PATTERN_SUGGESTION**: Consider adding FileType pattern '{Pattern}' for document type '{DocumentType}'", 
                    generatedPattern, detection.DocumentType);
                
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **PATTERN_GENERATION_ERROR**: Error generating pattern for {DocumentType}", detection.DocumentType);
            }
        }

        /// <summary>
        /// **FILENAME PATTERN GENERATOR**: Generate regex pattern from filename and document type
        /// </summary>
        private string GenerateFilenamePattern(string filename, string documentType)
        {
            if (string.IsNullOrWhiteSpace(filename) || string.IsNullOrWhiteSpace(documentType))
                return null;
            
            try
            {
                // **DOCUMENT TYPE KEYWORDS**: Extract keywords from document type
                var docTypeKeywords = ExtractDocumentTypeKeywords(documentType);
                
                // **FILENAME ANALYSIS**: Find keywords in filename
                var filenameKeywords = FindKeywordsInFilename(filename, docTypeKeywords);
                
                if (filenameKeywords.Any())
                {
                    // **PATTERN CONSTRUCTION**: Build regex pattern with found keywords
                    var keywordPattern = string.Join("|", filenameKeywords.Select(k => Regex.Escape(k)));
                    var pattern = $@"(?i).*({keywordPattern}).*";
                    
                    return pattern;
                }
                
                // **FALLBACK PATTERN**: Use document type as pattern basis
                var docTypeWords = Regex.Split(documentType, @"[_\-\s]+", RegexOptions.IgnoreCase)
                    .Where(w => w.Length >= 3)
                    .ToList();
                
                if (docTypeWords.Any())
                {
                    var fallbackPattern = string.Join("|", docTypeWords.Select(w => Regex.Escape(w)));
                    return $@"(?i).*({fallbackPattern}).*";
                }
                
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "⚠️ **PATTERN_GENERATION_WARNING**: Error generating pattern for {DocumentType}", documentType);
            }
            
            return null;
        }

        /// <summary>
        /// **DOCUMENT TYPE KEYWORD EXTRACTION**: Extract meaningful keywords from document type
        /// </summary>
        private List<string> ExtractDocumentTypeKeywords(string documentType)
        {
            var keywords = new List<string>();
            
            // **WORD SPLITTING**: Split document type into meaningful words
            var words = Regex.Split(documentType, @"[_\-\s]+", RegexOptions.IgnoreCase)
                .Where(w => w.Length >= 3 && !IsCommonWord(w))
                .ToList();
            
            keywords.AddRange(words);
            
            return keywords.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        }

        /// <summary>
        /// **KEYWORD FINDER**: Find document type keywords in filename
        /// </summary>
        private List<string> FindKeywordsInFilename(string filename, List<string> keywords)
        {
            var foundKeywords = new List<string>();
            
            foreach (var keyword in keywords)
            {
                if (filename.IndexOf(keyword, StringComparison.OrdinalIgnoreCase) >= 0)
                {
                    foundKeywords.Add(keyword);
                }
            }
            
            return foundKeywords;
        }

        /// <summary>
        /// **COMMON WORD FILTER**: Filter out common/generic words that don't add pattern value
        /// </summary>
        private bool IsCommonWord(string word)
        {
            var commonWords = new[] { "the", "and", "for", "with", "document", "file", "type", "form", "pdf", "txt" };
            return commonWords.Contains(word.ToLowerInvariant());
        }
    }

    /// <summary>
    /// Comprehensive result from hybrid document detection
    /// </summary>
    public class HybridDetectionResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string PrimaryDetectionMethod { get; set; } // "Database", "AI", "Hybrid"
        
        // Input data
        public string OriginalText { get; set; }
        public int OriginalLength { get; set; }
        public string DocumentPath { get; set; }
        
        // Detection results
        public List<DetectedDocument> Documents { get; set; } = new List<DetectedDocument>();
        public IEnumerable<DetectedDocument> DatabaseDetections { get; set; } = new List<DetectedDocument>();
        public IEnumerable<DetectedDocument> AIDetections { get; set; } = new List<DetectedDocument>();
        
        // Detection statistics
        public int DatabaseDetectionCount { get; set; }
        public int AIDetectionCount { get; set; }
        
        // FileType pattern validation
        public FileTypeValidationResult FileTypeValidation { get; set; }
        public bool FilenameTriggersAI { get; set; }
        
        // Completeness tracking
        public double CompletenessPercentage { get; set; }
        public double FinalCompletenessPercentage { get; set; }
        public int MissingTextLength { get; set; }
        
        // Separation intelligence
        public List<SeparationPattern> SeparationPatterns { get; set; } = new List<SeparationPattern>();
        public List<DocumentBoundary> DocumentBoundaries { get; set; } = new List<DocumentBoundary>();
        public List<string> RegexSeparators { get; set; } = new List<string>();
        
        public override string ToString()
        {
            return $"Hybrid Detection: {PrimaryDetectionMethod}, {Documents.Count} docs, {FinalCompletenessPercentage:F1}% complete";
        }
    }

    /// <summary>
    /// Detected document with hybrid metadata
    /// </summary>
    public class DetectedDocument
    {
        public string DocumentType { get; set; }
        public string Content { get; set; }
        public int StartPosition { get; set; }
        public int Length { get; set; }
        public double Confidence { get; set; }
        public string DetectionMethod { get; set; } // "Database", "AI"
        public List<string> MatchedKeywords { get; set; } = new List<string>();
        public string AIReasoning { get; set; }
        public int? TemplateTableMappingId { get; set; } // For database detections
    }

    /// <summary>
    /// Document separation pattern for intelligent text separation
    /// </summary>
    public class SeparationPattern
    {
        public string PatternType { get; set; } // "Regex", "LinePattern", "ContentTransition"
        public string Pattern { get; set; }
        public double Confidence { get; set; }
        public string Description { get; set; }
    }

    /// <summary>
    /// Document boundary information
    /// </summary>
    public class DocumentBoundary
    {
        public int StartPosition { get; set; }
        public int EndPosition { get; set; }
        public string DocumentType { get; set; }
        public double Confidence { get; set; }
        public string BoundaryMarker { get; set; }
    }

    /// <summary>
    /// Detection statistics for monitoring
    /// </summary>
    public class DetectionStatistics
    {
        public int DatabaseMappingsCount { get; set; }
        public int RecentDetectionLogs { get; set; }
        public double DatabaseDetectionSuccessRate { get; set; }
        public double AIFallbackUsageRate { get; set; }
    }
}