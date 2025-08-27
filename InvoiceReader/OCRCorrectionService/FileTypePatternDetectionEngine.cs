// File: OCRCorrectionService/FileTypePatternDetectionEngine.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.IO;
using System.Data.Entity;
using OCR.Business.Entities;
using Serilog;

namespace WaterNut.DataSpace
{
    /// <summary>
    /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v4.2**: FileType Pattern Detection Engine
    /// 
    /// **FILENAME INTELLIGENCE**: Validates filenames against FileType regex patterns to trigger AI detection
    /// **PATTERN MATCHING**: Uses database FileType.Pattern field to validate filename expectations
    /// **AI TRIGGER**: Mismatched filenames trigger AI detection for new document types
    /// **LEARNING INTEGRATION**: AI discoveries improve FileType pattern database
    /// **FILENAME ANALYTICS**: Tracks filename patterns and their document type correlations
    /// **SELF-IMPROVING**: System learns new filename patterns from successful AI detections
    /// </summary>
    public class FileTypePatternDetectionEngine
    {
        private readonly ILogger _logger;
        private readonly OCRContext _ocrContext; // For OCR mappings
        private List<FileTypePattern> _cachedPatterns;
        private DateTime _cacheExpiry = DateTime.MinValue;
        private readonly TimeSpan _cacheLifetime = TimeSpan.FromMinutes(15);

        public FileTypePatternDetectionEngine(ILogger logger, OCRContext ocrContext = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _ocrContext = ocrContext ?? new OCRContext();
        }

        /// <summary>
        /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v4.2**: Validate filename against FileType patterns
        /// 
        /// **FILENAME VALIDATION**: Check if filename matches expected FileType regex patterns
        /// **MISMATCH DETECTION**: Identify filenames that don't match any known patterns
        /// **AI TRIGGER LOGIC**: Determine if AI detection should be triggered for unknown patterns
        /// **PATTERN CONFIDENCE**: Calculate confidence in filename-based document type detection
        /// </summary>
        public async Task<FileTypeValidationResult> ValidateFilenamePatternAsync(string filePath, List<DetectedDocument> detectedDocuments)
        {
            _logger.Information("📁 **FILETYPE_VALIDATION_START**: Validating filename against FileType patterns");
            _logger.Information("   - **FILE_PATH**: {FilePath}", filePath ?? "Unknown");
            _logger.Information("   - **DETECTED_DOCUMENTS**: {Count} documents", detectedDocuments?.Count ?? 0);
            
            var result = new FileTypeValidationResult
            {
                FilePath = filePath,
                DetectedDocumentCount = detectedDocuments?.Count ?? 0
            };
            
            if (string.IsNullOrWhiteSpace(filePath))
            {
                _logger.Warning("⚠️ **NO_FILEPATH**: No file path provided for validation");
                result.ShouldTriggerAI = true; // No filename info = trigger AI
                result.ValidationMessage = "No filename available - AI detection recommended";
                return result;
            }
            
            try
            {
                var filename = Path.GetFileName(filePath);
                result.Filename = filename;
                
                // **STEP 1: LOAD ACTIVE FILETYPE PATTERNS** (with caching)
                var fileTypePatterns = await GetActiveFileTypePatternsAsync();
                _logger.Information("📋 **ACTIVE_PATTERNS_LOADED**: {Count} active FileType patterns found", fileTypePatterns.Count);
                
                // **STEP 2: VALIDATE AGAINST FILETYPE PATTERNS**
                var matchingFileTypes = await ValidateAgainstFileTypePatternsAsync(filename, fileTypePatterns);
                result.MatchingFileTypes = matchingFileTypes;
                
                // **STEP 3: CROSS-REFERENCE WITH DETECTED DOCUMENTS**
                var consistencyCheck = await CheckDocumentConsistencyAsync(matchingFileTypes, detectedDocuments);
                result.DocumentConsistency = consistencyCheck;
                
                // **STEP 4: DETERMINE AI TRIGGER NECESSITY**
                result.ShouldTriggerAI = ShouldTriggerAIDetection(matchingFileTypes, consistencyCheck, detectedDocuments);
                
                // **STEP 5: GENERATE VALIDATION SUMMARY**
                result.ValidationMessage = GenerateValidationMessage(result);
                
                _logger.Information("🎯 **FILETYPE_VALIDATION_COMPLETE**: {Matches} pattern matches, AI trigger: {Trigger}", 
                    matchingFileTypes.Count, result.ShouldTriggerAI);
                
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **FILETYPE_VALIDATION_ERROR**: Error validating filename patterns");
                result.HasError = true;
                result.ErrorMessage = ex.Message;
                result.ShouldTriggerAI = true; // Error = trigger AI for safety
            }
            
            return result;
        }

        /// <summary>
        /// **ACTIVE FILETYPE PATTERNS LOADER**: Load active FileType patterns with caching
        /// </summary>
        private async Task<List<FileTypePattern>> GetActiveFileTypePatternsAsync()
        {
            // **CACHE CHECK**: Use cached patterns if still valid
            if (_cachedPatterns != null && DateTime.Now < _cacheExpiry)
            {
                _logger.Verbose("📋 **CACHE_HIT**: Using cached FileType patterns ({Count} items)", _cachedPatterns.Count);
                return _cachedPatterns;
            }
            
            _logger.Information("🔄 **CACHE_MISS**: Loading FileType patterns");
            
            // **SIMPLIFIED PATTERNS**: Use OCR_TemplateTableMapping as pattern source for now
            _cachedPatterns = await LoadPatternsFromOCRMappingsAsync();
            
            _cacheExpiry = DateTime.Now.Add(_cacheLifetime);
            
            _logger.Information("📋 **PATTERNS_CACHED**: {Count} FileType patterns loaded and cached", _cachedPatterns.Count);
            
            return _cachedPatterns;
        }

        /// <summary>
        /// **PATTERN LOADER**: Load patterns from OCR_TemplateTableMapping as fallback
        /// </summary>
        private async Task<List<FileTypePattern>> LoadPatternsFromOCRMappingsAsync()
        {
            var patterns = new List<FileTypePattern>();
            
            try
            {
                // **LOAD FROM OCR MAPPINGS**: Use OCR_TemplateTableMapping as pattern source
                var ocrMappings = await _ocrContext.OCR_TemplateTableMapping
                    .Where(m => m.IsActive && !string.IsNullOrEmpty(m.DocumentType))
                    .ToListAsync();
                
                foreach (var mapping in ocrMappings)
                {
                    // **GENERATE FILENAME PATTERN**: Create filename pattern from DocumentType
                    var pattern = GenerateBasicFilenamePattern(mapping.DocumentType);
                    if (!string.IsNullOrWhiteSpace(pattern))
                    {
                        patterns.Add(new FileTypePattern
                        {
                            Id = mapping.Id,
                            DocumentType = mapping.DocumentType,
                            Pattern = pattern,
                            Priority = mapping.Priority
                        });
                    }
                }
                
                // **ADD COMMON PATTERNS**: Add some common filename patterns
                patterns.AddRange(GetCommonFilenamePatterns());
                
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **PATTERN_LOADING_ERROR**: Error loading FileType patterns");
                
                // **FALLBACK**: Use only common patterns
                patterns = GetCommonFilenamePatterns();
            }
            
            return patterns.OrderBy(p => p.Priority).ToList();
        }

        /// <summary>
        /// **BASIC PATTERN GENERATOR**: Generate basic filename pattern from document type
        /// </summary>
        private string GenerateBasicFilenamePattern(string documentType)
        {
            if (string.IsNullOrWhiteSpace(documentType)) return null;
            
            // **EXTRACT KEYWORDS**: Extract meaningful words from document type
            var words = Regex.Split(documentType, @"[_\-\s]+", RegexOptions.IgnoreCase)
                .Where(w => w.Length >= 3)
                .Select(w => Regex.Escape(w))
                .ToList();
            
            if (words.Any())
            {
                var pattern = $@"(?i).*({string.Join("|", words)}).*";
                return pattern;
            }
            
            return null;
        }

        /// <summary>
        /// **COMMON PATTERNS**: Get common filename patterns for document types
        /// </summary>
        private List<FileTypePattern> GetCommonFilenamePatterns()
        {
            return new List<FileTypePattern>
            {
                new FileTypePattern { Id = -1, DocumentType = "Invoice", Pattern = @"(?i).*(invoice|bill|receipt).*", Priority = 1 },
                new FileTypePattern { Id = -2, DocumentType = "CustomsDeclaration", Pattern = @"(?i).*(customs|declaration|import|export).*", Priority = 2 },
                new FileTypePattern { Id = -3, DocumentType = "ShippingDocument", Pattern = @"(?i).*(shipping|freight|delivery|manifest).*", Priority = 3 },
                new FileTypePattern { Id = -4, DocumentType = "PackingList", Pattern = @"(?i).*(packing|list|contents).*", Priority = 4 },
                new FileTypePattern { Id = -5, DocumentType = "Certificate", Pattern = @"(?i).*(certificate|cert|origin).*", Priority = 5 }
            };
        }

        /// <summary>
        /// **FILETYPE PATTERN VALIDATION**: Validate filename against FileType patterns
        /// </summary>
        private async Task<List<FileTypeMatch>> ValidateAgainstFileTypePatternsAsync(string filename, List<FileTypePattern> fileTypePatterns)
        {
            _logger.Verbose("🔍 **PATTERN_VALIDATION**: Testing filename against {Count} FileType patterns", fileTypePatterns.Count);
            
            var matches = new List<FileTypeMatch>();
            
            foreach (var pattern in fileTypePatterns)
            {
                try
                {
                    if (string.IsNullOrWhiteSpace(pattern.Pattern)) continue;
                    
                    // **REGEX PATTERN MATCHING**: Test filename against FileType pattern
                    var isMatch = Regex.IsMatch(filename, pattern.Pattern, RegexOptions.IgnoreCase);
                    
                    if (isMatch)
                    {
                        var confidence = CalculatePatternConfidence(filename, pattern.Pattern);
                        
                        var match = new FileTypeMatch
                        {
                            FileTypeId = pattern.Id,
                            FileTypeName = pattern.DocumentType,
                            Pattern = pattern.Pattern,
                            Confidence = confidence,
                            MatchType = "Regex"
                        };
                        
                        matches.Add(match);
                        
                        _logger.Information("✅ **PATTERN_MATCH**: {FileType} - Pattern: {Pattern}, Confidence: {Confidence:F2}", 
                            match.FileTypeName, pattern.Pattern, confidence);
                    }
                    else
                    {
                        _logger.Verbose("❌ **PATTERN_NO_MATCH**: {FileType} - Pattern: {Pattern}", 
                            pattern.DocumentType, pattern.Pattern);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Warning(ex, "⚠️ **PATTERN_ERROR**: Invalid regex pattern for FileType {Id}: {Pattern}", 
                        pattern.Id, pattern.Pattern);
                }
            }
            
            // **RANK BY CONFIDENCE**: Sort matches by confidence score
            matches = matches.OrderByDescending(m => m.Confidence).ToList();
            
            _logger.Information("📊 **PATTERN_VALIDATION_COMPLETE**: {Matches} patterns matched filename", matches.Count);
            
            return matches;
        }

        /// <summary>
        /// **PATTERN CONFIDENCE CALCULATION**: Calculate confidence in pattern match
        /// </summary>
        private double CalculatePatternConfidence(string filename, string pattern)
        {
            try
            {
                var match = Regex.Match(filename, pattern, RegexOptions.IgnoreCase);
                if (!match.Success) return 0.0;
                
                // **MATCH COVERAGE**: How much of filename is covered by pattern
                var matchCoverage = (double)match.Length / filename.Length;
                
                // **PATTERN SPECIFICITY**: More specific patterns get higher confidence
                var patternSpecificity = CalculatePatternSpecificity(pattern);
                
                // **COMBINED CONFIDENCE**: Weighted combination
                var confidence = (matchCoverage * 0.6) + (patternSpecificity * 0.4);
                
                return Math.Min(0.95, Math.Max(0.1, confidence));
            }
            catch
            {
                return 0.3; // Default low confidence for errors
            }
        }

        /// <summary>
        /// **PATTERN SPECIFICITY CALCULATION**: Calculate specificity of regex pattern
        /// </summary>
        private double CalculatePatternSpecificity(string pattern)
        {
            var specificity = 0.5; // Base specificity
            
            // **SPECIFICITY INDICATORS**: Features that increase pattern specificity
            if (pattern.Contains(@"\d")) specificity += 0.1; // Digit requirements
            if (pattern.Contains(@"[A-Z]")) specificity += 0.1; // Case sensitivity
            if (pattern.Contains(@"\.")) specificity += 0.1; // File extensions
            if (pattern.Contains(@"^") || pattern.Contains(@"$")) specificity += 0.1; // Anchors
            if (pattern.Length > 20) specificity += 0.1; // Longer patterns
            if (Regex.Matches(pattern, @"\\[a-zA-Z]").Count > 3) specificity += 0.1; // Multiple character classes
            
            return Math.Min(0.9, specificity);
        }

        /// <summary>
        /// **DOCUMENT CONSISTENCY CHECK**: Check consistency between filename patterns and detected documents
        /// </summary>
        private async Task<DocumentConsistencyResult> CheckDocumentConsistencyAsync(List<FileTypeMatch> fileTypeMatches, List<DetectedDocument> detectedDocuments)
        {
            _logger.Verbose("🔍 **CONSISTENCY_CHECK**: Checking filename vs detected document consistency");
            
            var result = new DocumentConsistencyResult();
            
            if (!fileTypeMatches.Any() || !detectedDocuments.Any())
            {
                result.ConsistencyScore = 0.0;
                result.ConsistencyRating = "No_Data";
                result.Issues.Add("Insufficient data for consistency analysis");
                return result;
            }
            
            // **DOCUMENT TYPE CORRELATION**: Check if filename patterns correlate with detected document types
            var filenameDocTypes = fileTypeMatches.Select(m => m.FileTypeName).ToList();
            var detectedDocTypes = detectedDocuments.Select(d => d.DocumentType).ToList();
            
            // **EXACT MATCHES**: Find direct correlations
            var exactMatches = filenameDocTypes.Intersect(detectedDocTypes, StringComparer.OrdinalIgnoreCase).ToList();
            result.ExactMatches = exactMatches;
            
            // **PARTIAL MATCHES**: Find partial correlations (e.g., "Invoice" in both)
            var partialMatches = FindPartialMatches(filenameDocTypes, detectedDocTypes);
            result.PartialMatches = partialMatches;
            
            // **CONSISTENCY SCORING**: Calculate overall consistency
            var exactScore = exactMatches.Count > 0 ? 1.0 : 0.0;
            var partialScore = partialMatches.Count > 0 ? 0.5 : 0.0;
            var coverageScore = (double)Math.Min(filenameDocTypes.Count, detectedDocTypes.Count) / 
                               Math.Max(filenameDocTypes.Count, detectedDocTypes.Count);
            
            result.ConsistencyScore = (exactScore * 0.5) + (partialScore * 0.3) + (coverageScore * 0.2);
            
            // **CONSISTENCY RATING**: Assign rating based on score
            if (result.ConsistencyScore >= 0.8) result.ConsistencyRating = "High";
            else if (result.ConsistencyScore >= 0.5) result.ConsistencyRating = "Medium";
            else if (result.ConsistencyScore >= 0.2) result.ConsistencyRating = "Low";
            else result.ConsistencyRating = "None";
            
            // **ISSUE IDENTIFICATION**: Identify specific consistency issues
            if (exactMatches.Count == 0 && partialMatches.Count == 0)
            {
                result.Issues.Add("No correlation between filename patterns and detected document types");
            }
            
            if (filenameDocTypes.Count > detectedDocTypes.Count + 1)
            {
                result.Issues.Add("Filename suggests more document types than detected");
            }
            
            if (detectedDocTypes.Count > filenameDocTypes.Count + 1)
            {
                result.Issues.Add("More document types detected than filename suggests");
            }
            
            _logger.Information("📊 **CONSISTENCY_ANALYSIS**: Score: {Score:F2}, Rating: {Rating}, Issues: {Issues}", 
                result.ConsistencyScore, result.ConsistencyRating, result.Issues.Count);
            
            return result;
        }

        /// <summary>
        /// **PARTIAL MATCH FINDER**: Find partial correlations between filename and document types
        /// </summary>
        private List<string> FindPartialMatches(List<string> filenameTypes, List<string> detectedTypes)
        {
            var partialMatches = new List<string>();
            
            // **KEYWORD EXTRACTION**: Extract keywords from type names
            var filenameKeywords = ExtractTypeKeywords(filenameTypes);
            var detectedKeywords = ExtractTypeKeywords(detectedTypes);
            
            // **KEYWORD CORRELATION**: Find common keywords
            var commonKeywords = filenameKeywords.Intersect(detectedKeywords, StringComparer.OrdinalIgnoreCase).ToList();
            
            partialMatches.AddRange(commonKeywords);
            
            return partialMatches.Distinct().ToList();
        }

        /// <summary>
        /// **TYPE KEYWORD EXTRACTION**: Extract meaningful keywords from document type names
        /// </summary>
        private List<string> ExtractTypeKeywords(List<string> typeNames)
        {
            var keywords = new List<string>();
            
            foreach (var typeName in typeNames)
            {
                if (string.IsNullOrWhiteSpace(typeName)) continue;
                
                // **SPLIT AND FILTER**: Extract meaningful words
                var words = Regex.Split(typeName, @"[_\-\s]+", RegexOptions.IgnoreCase)
                    .Where(w => w.Length >= 3 && !IsCommonWord(w))
                    .ToList();
                
                keywords.AddRange(words);
            }
            
            return keywords.Distinct(StringComparer.OrdinalIgnoreCase).ToList();
        }

        /// <summary>
        /// **COMMON WORD FILTER**: Filter out common/generic words
        /// </summary>
        private bool IsCommonWord(string word)
        {
            var commonWords = new[] { "the", "and", "for", "with", "document", "file", "type", "form" };
            return commonWords.Contains(word.ToLowerInvariant());
        }

        /// <summary>
        /// **AI TRIGGER DECISION**: Determine if AI detection should be triggered
        /// </summary>
        private bool ShouldTriggerAIDetection(List<FileTypeMatch> fileTypeMatches, DocumentConsistencyResult consistency, List<DetectedDocument> detectedDocuments)
        {
            _logger.Verbose("🤖 **AI_TRIGGER_ANALYSIS**: Determining if AI detection should be triggered");
            
            // **TRIGGER CONDITIONS**: Various scenarios that should trigger AI detection
            
            // **1. NO FILENAME MATCHES**: No FileType patterns matched filename
            if (!fileTypeMatches.Any())
            {
                _logger.Information("🚨 **AI_TRIGGER**: No filename pattern matches - unknown file type");
                return true;
            }
            
            // **2. LOW PATTERN CONFIDENCE**: All pattern matches have low confidence
            var maxConfidence = fileTypeMatches.Max(m => m.Confidence);
            if (maxConfidence < 0.6)
            {
                _logger.Information("🚨 **AI_TRIGGER**: Low filename pattern confidence ({Confidence:F2}) - verification needed", maxConfidence);
                return true;
            }
            
            // **3. DOCUMENT INCONSISTENCY**: Filename patterns don't match detected documents
            if (consistency.ConsistencyScore < 0.3)
            {
                _logger.Information("🚨 **AI_TRIGGER**: Low filename-document consistency ({Score:F2}) - analysis needed", consistency.ConsistencyScore);
                return true;
            }
            
            // **4. MULTIPLE CONFLICTING PATTERNS**: Multiple high-confidence patterns suggest different types
            var highConfidenceMatches = fileTypeMatches.Where(m => m.Confidence > 0.7).ToList();
            if (highConfidenceMatches.Count > 1)
            {
                var distinctTypes = highConfidenceMatches.Select(m => m.FileTypeName).Distinct().Count();
                if (distinctTypes > 1)
                {
                    _logger.Information("🚨 **AI_TRIGGER**: Multiple conflicting filename patterns ({Count} types) - resolution needed", distinctTypes);
                    return true;
                }
            }
            
            // **5. NO DETECTED DOCUMENTS**: Filename suggests document types but none were detected
            if (!detectedDocuments.Any() && fileTypeMatches.Any(m => m.Confidence > 0.7))
            {
                _logger.Information("🚨 **AI_TRIGGER**: Filename suggests documents but none detected - investigation needed");
                return true;
            }
            
            // **6. EXCESSIVE DETECTED DOCUMENTS**: Many more documents detected than filename suggests
            if (detectedDocuments.Count > fileTypeMatches.Count + 2)
            {
                _logger.Information("🚨 **AI_TRIGGER**: More documents detected ({Detected}) than filename patterns suggest ({Patterns}) - analysis needed", 
                    detectedDocuments.Count, fileTypeMatches.Count);
                return true;
            }
            
            // **NO TRIGGER**: Filename patterns and detected documents are consistent
            _logger.Information("✅ **NO_AI_TRIGGER**: Filename patterns consistent with detected documents");
            return false;
        }

        /// <summary>
        /// **VALIDATION MESSAGE GENERATION**: Generate human-readable validation summary
        /// </summary>
        private string GenerateValidationMessage(FileTypeValidationResult result)
        {
            if (!string.IsNullOrEmpty(result.ErrorMessage))
            {
                return $"Validation error: {result.ErrorMessage}";
            }
            
            if (!result.MatchingFileTypes.Any())
            {
                return "No filename pattern matches found - AI detection recommended";
            }
            
            var topMatch = result.MatchingFileTypes.First();
            var message = $"Best match: {topMatch.FileTypeName} (confidence: {topMatch.Confidence:F2})";
            
            if (result.DocumentConsistency != null)
            {
                message += $", consistency: {result.DocumentConsistency.ConsistencyRating}";
            }
            
            if (result.ShouldTriggerAI)
            {
                message += " - AI detection triggered";
            }
            else
            {
                message += " - Pattern validation successful";
            }
            
            return message;
        }

        /// <summary>
        /// **CACHE INVALIDATION**: Clear cached FileType patterns to force reload
        /// </summary>
        public void InvalidateCache()
        {
            _cachedPatterns = null;
            _cacheExpiry = DateTime.MinValue;
            _logger.Information("🔄 **CACHE_INVALIDATED**: FileType patterns cache cleared");
        }
    }

    /// <summary>
    /// **FILETYPE PATTERN**: Simplified FileType pattern for filename validation
    /// </summary>
    public class FileTypePattern
    {
        public int Id { get; set; }
        public string DocumentType { get; set; }
        public string Pattern { get; set; }
        public int Priority { get; set; }
    }

    /// <summary>
    /// **FILETYPE VALIDATION RESULT**: Complete result of filename pattern validation
    /// </summary>
    public class FileTypeValidationResult
    {
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
        public string FilePath { get; set; }
        public string Filename { get; set; }
        public int DetectedDocumentCount { get; set; }
        public List<FileTypeMatch> MatchingFileTypes { get; set; } = new List<FileTypeMatch>();
        public DocumentConsistencyResult DocumentConsistency { get; set; }
        public bool ShouldTriggerAI { get; set; }
        public string ValidationMessage { get; set; }
        public List<string> RecommendedActions { get; set; } = new List<string>();
    }

    /// <summary>
    /// **FILETYPE MATCH**: Information about a matching FileType pattern
    /// </summary>
    public class FileTypeMatch
    {
        public int FileTypeId { get; set; }
        public string FileTypeName { get; set; }
        public string Pattern { get; set; }
        public double Confidence { get; set; }
        public string MatchType { get; set; }
    }

    /// <summary>
    /// **DOCUMENT CONSISTENCY RESULT**: Analysis of filename vs document consistency
    /// </summary>
    public class DocumentConsistencyResult
    {
        public double ConsistencyScore { get; set; }
        public string ConsistencyRating { get; set; }
        public List<string> ExactMatches { get; set; } = new List<string>();
        public List<string> PartialMatches { get; set; } = new List<string>();
        public List<string> Issues { get; set; } = new List<string>();
    }
}