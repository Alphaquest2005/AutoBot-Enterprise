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
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? new OCRContext();
            
            // Initialize hybrid system components
            _databaseEngine = new DatabaseDocumentDetectionEngine(_logger, _context);
            _aiEngine = new AIDocumentDetectionEngine(_logger);
            _learningSystem = new DocumentLearningSystem(_logger, _context);
            _completenessValidator = new DocumentCompletenessValidator(_logger);
            _separationIntelligence = new TextSeparationIntelligence(_logger);
            _fileTypeEngine = new FileTypePatternDetectionEngine(_logger, _context);
            
            _logger.Information("🏗️ **HYBRID_DETECTION_INITIALIZED**: Database + AI + Learning + FileType pattern detection ready");
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
            _logger.Information("🎯 **HYBRID_DETECTION_START**: Database-first with AI fallback and learning");
            _logger.Information("   - **INPUT**: {Length} chars, Path: {Path}", text?.Length ?? 0, documentPath ?? "Unknown");
            
            var result = new HybridDetectionResult
            {
                OriginalText = text,
                OriginalLength = text?.Length ?? 0,
                DocumentPath = documentPath
            };

            try
            {
                // **PHASE 1: DATABASE-FIRST DETECTION** (Fast path for known document types)
                _logger.Information("🗄️ **PHASE_1_DATABASE_DETECTION**: Checking OCR_TemplateTableMapping Keywords");
                var databaseResults = await _databaseEngine.DetectKnownDocumentTypesAsync(text);
                
                result.DatabaseDetections = databaseResults;
                result.DatabaseDetectionCount = databaseResults.Count();
                
                if (databaseResults.Any(d => d.Confidence >= 0.8))
                {
                    _logger.Information("✅ **DATABASE_DETECTION_SUCCESS**: {Count} high-confidence matches found", 
                        databaseResults.Count(d => d.Confidence >= 0.8));
                    result.PrimaryDetectionMethod = "Database";
                    result.Documents.AddRange(databaseResults.Where(d => d.Confidence >= 0.8));
                }
                
                // **PHASE 2: FILETYPE PATTERN VALIDATION** (Filename pattern validation)
                _logger.Information("📁 **PHASE_2_FILETYPE_VALIDATION**: Validating filename against FileType patterns");
                var fileTypeValidation = await _fileTypeEngine.ValidateFilenamePatternAsync(documentPath, result.Documents);
                
                result.FileTypeValidation = fileTypeValidation;
                result.FilenameTriggersAI = fileTypeValidation.ShouldTriggerAI;
                
                if (fileTypeValidation.ShouldTriggerAI)
                {
                    _logger.Warning("🚨 **FILETYPE_AI_TRIGGER**: {Message}", fileTypeValidation.ValidationMessage);
                }
                else
                {
                    _logger.Information("✅ **FILETYPE_VALIDATION_PASSED**: {Message}", fileTypeValidation.ValidationMessage);
                }
                
                // **PHASE 3: COMPLETENESS CHECK** (Detect missing documents)
                _logger.Information("📊 **PHASE_2_COMPLETENESS_CHECK**: Validating 100% text coverage");
                var completenessResult = await _completenessValidator.ValidateCompletenessAsync(text, result.Documents);
                
                result.CompletenessPercentage = completenessResult.CoveragePercentage;
                result.MissingTextLength = completenessResult.MissingTextLength;
                
                // **AI TRIGGER DECISION**: Determine if AI detection is needed based on completeness OR filename patterns
                var needsAIForCompleteness = completenessResult.CoveragePercentage < 100.0;
                var needsAIForFilename = fileTypeValidation.ShouldTriggerAI;
                var needsAI = needsAIForCompleteness || needsAIForFilename;
                
                if (needsAI)
                {
                    var reasons = new List<string>();
                    if (needsAIForCompleteness) reasons.Add($"incomplete coverage ({completenessResult.CoveragePercentage:F1}%)");
                    if (needsAIForFilename) reasons.Add("filename pattern validation");
                    
                    _logger.Warning("⚠️ **AI_DETECTION_NEEDED**: {Reasons} - triggering AI fallback", string.Join(" + ", reasons));
                    
                    // **PHASE 4: AI FALLBACK DETECTION** (Handle unknown types, missing content, and filename mismatches)
                    _logger.Information("🤖 **PHASE_4_AI_FALLBACK**: Detecting unknown document types and resolving validation issues");
                    var aiResults = await _aiEngine.DetectUnknownDocumentTypesAsync(text, result.Documents, completenessResult.MissingText);
                    
                    result.AIDetections = aiResults;
                    result.AIDetectionCount = aiResults.Count();
                    
                    if (result.Documents.Count == 0)
                    {
                        result.PrimaryDetectionMethod = "AI";
                    }
                    else
                    {
                        result.PrimaryDetectionMethod = "Hybrid";
                    }
                    
                    result.Documents.AddRange(aiResults);
                    
                    // **PHASE 5: LEARNING SYSTEM** (Improve database for future speed)
                    _logger.Information("🧠 **PHASE_5_LEARNING**: Adding AI discoveries to database and FileType patterns");
                    await _learningSystem.LearnFromAIDetectionsAsync(aiResults, text);
                    
                    // **FILENAME PATTERN LEARNING**: Learn filename patterns from successful AI detections
                    if (needsAIForFilename && aiResults.Any())
                    {
                        await LearnFilenamePatterns(documentPath, aiResults);
                    }
                }
                
                // **PHASE 6: SEPARATION INTELLIGENCE** (Enhance text separation capabilities)
                _logger.Information("🔍 **PHASE_6_SEPARATION_INTELLIGENCE**: Analyzing document boundaries and patterns");
                var separationResult = await _separationIntelligence.AnalyzeSeparationPatternsAsync(text, result.Documents);
                
                result.SeparationPatterns = separationResult.Patterns;
                result.DocumentBoundaries = separationResult.Boundaries;
                result.RegexSeparators = separationResult.RegexSeparators;
                
                // **PHASE 7: FINAL COMPLETENESS VALIDATION**
                var finalCompleteness = await _completenessValidator.ValidateCompletenessAsync(text, result.Documents);
                result.FinalCompletenessPercentage = finalCompleteness.CoveragePercentage;
                
                _logger.Information("🎯 **HYBRID_DETECTION_COMPLETE**: {Method} detection, {Count} documents, {Coverage:F1}% coverage", 
                    result.PrimaryDetectionMethod, result.Documents.Count, result.FinalCompletenessPercentage);
                
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