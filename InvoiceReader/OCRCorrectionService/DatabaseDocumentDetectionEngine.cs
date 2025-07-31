// File: OCRCorrectionService/DatabaseDocumentDetectionEngine.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Data.Entity;
using OCR.Business.Entities;
using Serilog;

namespace WaterNut.DataSpace
{
    /// <summary>
    /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v4.2**: Database-Driven Document Detection Engine
    /// 
    /// **HIGH-SPEED DATABASE DETECTION**: Uses OCR_TemplateTableMapping Keywords for lightning-fast known document type detection
    /// **KEYWORD SCORING ALGORITHM**: TF-IDF style scoring against database Keywords field
    /// **MATCH THRESHOLD VALIDATION**: Uses database MatchThreshold for confidence determination
    /// **LOGGING INTEGRATION**: All detections logged to OCR_KeywordDetectionLog for analysis
    /// **PERFORMANCE OPTIMIZED**: Cached mappings, efficient queries, minimal database round-trips
    /// </summary>
    public class DatabaseDocumentDetectionEngine
    {
        private readonly ILogger _logger;
        private readonly OCRContext _context;
        private List<OCR_TemplateTableMapping> _cachedMappings;
        private DateTime _cacheExpiry = DateTime.MinValue;
        private readonly TimeSpan _cacheLifetime = TimeSpan.FromMinutes(30);

        public DatabaseDocumentDetectionEngine(ILogger logger, OCRContext context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v4.2**: Fast database-driven document type detection
        /// 
        /// **DATABASE-FIRST APPROACH**: Query OCR_TemplateTableMapping for all active mappings with Keywords
        /// **KEYWORD SCORING**: Calculate TF-IDF style scores against each mapping's Keywords field
        /// **THRESHOLD VALIDATION**: Use MatchThreshold to determine if confidence meets requirements
        /// **DETECTION LOGGING**: Log all attempts to OCR_KeywordDetectionLog for analysis and learning
        /// </summary>
        public async Task<List<DetectedDocument>> DetectKnownDocumentTypesAsync(string text)
        {
            _logger.Information("🗄️ **DATABASE_DETECTION_START**: Querying OCR_TemplateTableMapping for known document types");
            _logger.Information("   - **INPUT_LENGTH**: {Length} characters", text?.Length ?? 0);
            
            var detectedDocuments = new List<DetectedDocument>();
            
            try
            {
                // **STEP 1: LOAD ACTIVE MAPPINGS** (with caching for performance)
                var mappings = await GetActiveMappingsAsync();
                _logger.Information("📋 **ACTIVE_MAPPINGS_LOADED**: {Count} active template mappings found", mappings.Count);
                
                if (!mappings.Any())
                {
                    _logger.Warning("⚠️ **NO_MAPPINGS_FOUND**: No active OCR_TemplateTableMapping records found");
                    return detectedDocuments;
                }
                
                // **STEP 2: SCORE TEXT AGAINST EACH MAPPING**
                foreach (var mapping in mappings)
                {
                    _logger.Verbose("🔍 **SCORING_MAPPING**: {DocumentType} (ID: {Id})", mapping.DocumentType, mapping.Id);
                    
                    var score = await ScoreTextAgainstMappingAsync(text, mapping);
                    
                    _logger.Verbose("   - **KEYWORD_SCORE**: {Score:F3} (Threshold: {Threshold:F3})", 
                        score.KeywordScore, mapping.MatchThreshold);
                    
                    // **STEP 3: THRESHOLD VALIDATION**
                    if (score.KeywordScore >= (double)mapping.MatchThreshold)
                    {
                        var detectedDoc = new DetectedDocument
                        {
                            DocumentType = mapping.DocumentType,
                            Content = text, // Full content for single document detection
                            StartPosition = 0,
                            Length = text.Length,
                            Confidence = score.KeywordScore,
                            DetectionMethod = "Database",
                            MatchedKeywords = score.MatchedKeywords,
                            TemplateTableMappingId = mapping.Id
                        };
                        
                        detectedDocuments.Add(detectedDoc);
                        
                        _logger.Information("✅ **DATABASE_MATCH_FOUND**: {DocumentType} - Score: {Score:F3}, Keywords: {Keywords}", 
                            mapping.DocumentType, score.KeywordScore, string.Join(", ", score.MatchedKeywords));
                        
                        // **STEP 4: LOG SUCCESSFUL DETECTION**
                        await LogDetectionAsync(text, mapping, score, true);
                    }
                    else
                    {
                        _logger.Verbose("❌ **BELOW_THRESHOLD**: {DocumentType} score {Score:F3} < threshold {Threshold:F3}", 
                            mapping.DocumentType, score.KeywordScore, mapping.MatchThreshold);
                        
                        // Log failed detection for learning purposes
                        await LogDetectionAsync(text, mapping, score, false);
                    }
                }
                
                // **STEP 5: RANK RESULTS BY CONFIDENCE**
                detectedDocuments = detectedDocuments.OrderByDescending(d => d.Confidence).ToList();
                
                _logger.Information("🎯 **DATABASE_DETECTION_COMPLETE**: {Count} documents detected above threshold", 
                    detectedDocuments.Count);
                
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **DATABASE_DETECTION_ERROR**: Error in database document detection");
                throw;
            }
            
            return detectedDocuments;
        }

        /// <summary>
        /// **PERFORMANCE-OPTIMIZED MAPPING LOADER**: Cached active mappings with cache expiry
        /// </summary>
        private async Task<List<OCR_TemplateTableMapping>> GetActiveMappingsAsync()
        {
            // **CACHE CHECK**: Use cached mappings if still valid
            if (_cachedMappings != null && DateTime.Now < _cacheExpiry)
            {
                _logger.Verbose("📋 **CACHE_HIT**: Using cached mappings ({Count} items)", _cachedMappings.Count);
                return _cachedMappings;
            }
            
            _logger.Information("🔄 **CACHE_MISS**: Loading fresh mappings from database");
            
            // **DATABASE QUERY**: Load active mappings with non-empty Keywords
            _cachedMappings = await _context.OCR_TemplateTableMapping
                .Where(m => m.IsActive && 
                           !string.IsNullOrEmpty(m.Keywords) && 
                           !string.IsNullOrEmpty(m.DocumentType))
                .OrderBy(m => m.Priority)
                .ToListAsync();
            
            _cacheExpiry = DateTime.Now.Add(_cacheLifetime);
            
            _logger.Information("📋 **MAPPINGS_CACHED**: {Count} active mappings loaded and cached", _cachedMappings.Count);
            
            return _cachedMappings;
        }

        /// <summary>
        /// **KEYWORD SCORING ALGORITHM**: TF-IDF style scoring against mapping Keywords
        /// </summary>
        private async Task<KeywordScoringResult> ScoreTextAgainstMappingAsync(string text, OCR_TemplateTableMapping mapping)
        {
            var result = new KeywordScoringResult();
            
            if (string.IsNullOrWhiteSpace(text) || string.IsNullOrWhiteSpace(mapping.Keywords))
            {
                return result;
            }
            
            try
            {
                // **KEYWORDS PARSING**: Extract keywords from mapping (comma/semicolon separated)
                var keywords = ParseKeywords(mapping.Keywords);
                _logger.Verbose("🔍 **KEYWORDS_PARSED**: {Count} keywords for {DocumentType}", 
                    keywords.Count, mapping.DocumentType);
                
                // **TEXT NORMALIZATION**: Normalize text for case-insensitive matching
                var normalizedText = text.ToLowerInvariant();
                var textWords = Regex.Split(normalizedText, @"\W+").Where(w => w.Length > 2).ToArray();
                var textWordCount = textWords.Length;
                
                double totalScore = 0.0;
                var matchedKeywords = new List<string>();
                
                // **KEYWORD SCORING**: Score each keyword against text
                foreach (var keyword in keywords)
                {
                    var normalizedKeyword = keyword.ToLowerInvariant().Trim();
                    if (string.IsNullOrWhiteSpace(normalizedKeyword)) continue;
                    
                    // **EXACT PHRASE MATCHING**: Check for exact keyword phrase
                    bool exactMatch = normalizedText.Contains(normalizedKeyword);
                    if (exactMatch)
                    {
                        matchedKeywords.Add(keyword);
                        
                        // **TF-IDF STYLE SCORING**: Term frequency * inverse document frequency approximation
                        var keywordWords = normalizedKeyword.Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);
                        var keywordScore = CalculateKeywordScore(normalizedText, keywordWords, textWordCount);
                        
                        totalScore += keywordScore;
                        
                        _logger.Verbose("   ✅ **KEYWORD_MATCH**: '{Keyword}' -> Score: {Score:F3}", 
                            keyword, keywordScore);
                    }
                    else
                    {
                        _logger.Verbose("   ❌ **KEYWORD_MISS**: '{Keyword}' not found", keyword);
                    }
                }
                
                // **FINAL SCORE CALCULATION**: Normalize by keyword count
                result.KeywordScore = keywords.Count > 0 ? totalScore / keywords.Count : 0.0;
                result.MatchedKeywords = matchedKeywords;
                result.TotalKeywords = keywords.Count;
                result.MatchedKeywordCount = matchedKeywords.Count;
                
                _logger.Verbose("📊 **FINAL_SCORE**: {Score:F3} ({Matched}/{Total} keywords matched)", 
                    result.KeywordScore, result.MatchedKeywordCount, result.TotalKeywords);
                
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **SCORING_ERROR**: Error scoring text against mapping {MappingId}", mapping.Id);
            }
            
            return result;
        }

        /// <summary>
        /// **KEYWORD PARSER**: Extract keywords from database Keywords field
        /// </summary>
        private List<string> ParseKeywords(string keywordsString)
        {
            if (string.IsNullOrWhiteSpace(keywordsString))
                return new List<string>();
            
            // **MULTI-DELIMITER PARSING**: Support comma, semicolon, pipe separators
            var separators = new[] { ',', ';', '|', '\n', '\r' };
            var keywords = keywordsString
                .Split(separators, StringSplitOptions.RemoveEmptyEntries)
                .Select(k => k.Trim())
                .Where(k => !string.IsNullOrWhiteSpace(k))
                .ToList();
            
            return keywords;
        }

        /// <summary>
        /// **TF-IDF STYLE SCORING**: Calculate keyword relevance score
        /// </summary>
        private double CalculateKeywordScore(string text, string[] keywordWords, int totalWords)
        {
            double score = 0.0;
            
            foreach (var word in keywordWords)
            {
                if (string.IsNullOrWhiteSpace(word) || word.Length < 2) continue;
                
                // **TERM FREQUENCY**: Count occurrences of word in text
                var wordCount = Regex.Matches(text, $@"\b{Regex.Escape(word)}\b", RegexOptions.IgnoreCase).Count;
                if (wordCount > 0)
                {
                    // **FREQUENCY SCORING**: Higher frequency = higher score, but with diminishing returns
                    var termFrequency = Math.Log(1 + wordCount); // Log dampening
                    var wordImportance = Math.Max(0.1, Math.Min(1.0, word.Length / 10.0)); // Longer words more important
                    
                    score += termFrequency * wordImportance;
                }
            }
            
            // **NORMALIZATION**: Normalize by keyword length and text length
            return score / Math.Max(1, keywordWords.Length);
        }

        /// <summary>
        /// **DETECTION LOGGING**: Log detection attempts to OCR_KeywordDetectionLog
        /// </summary>
        private async Task LogDetectionAsync(string text, OCR_TemplateTableMapping mapping, KeywordScoringResult score, bool success)
        {
            try
            {
                var log = new OCR_KeywordDetectionLog
                {
                    DocumentPath = "HybridDetection", // Will be updated by caller if available
                    DocumentContent = text.Length > 1000 ? text.Substring(0, 1000) + "..." : text,
                    DetectedMappingId = success ? mapping.Id : (int?)null,
                    KeywordMatches = string.Join(", ", score.MatchedKeywords),
                    MatchScore = (decimal)score.KeywordScore,
                    ProcessingTimeMs = 0, // Will be calculated at higher level
                    Success = success,
                    ErrorMessage = success ? null : $"Score {score.KeywordScore:F3} below threshold {mapping.MatchThreshold:F3}",
                    CreatedDate = DateTime.Now
                };
                
                _context.OCR_KeywordDetectionLog.Add(log);
                await _context.SaveChangesAsync();
                
                _logger.Verbose("📝 **DETECTION_LOGGED**: {DocumentType} - Success: {Success}, Score: {Score:F3}", 
                    mapping.DocumentType, success, score.KeywordScore);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **LOGGING_ERROR**: Failed to log detection for mapping {MappingId}", mapping.Id);
            }
        }

        /// <summary>
        /// **CACHE INVALIDATION**: Clear cached mappings to force reload
        /// </summary>
        public void InvalidateCache()
        {
            _cachedMappings = null;
            _cacheExpiry = DateTime.MinValue;
            _logger.Information("🔄 **CACHE_INVALIDATED**: Mappings cache cleared");
        }
    }

    /// <summary>
    /// **KEYWORD SCORING RESULT**: Complete scoring information
    /// </summary>
    public class KeywordScoringResult
    {
        public double KeywordScore { get; set; }
        public List<string> MatchedKeywords { get; set; } = new List<string>();
        public int TotalKeywords { get; set; }
        public int MatchedKeywordCount { get; set; }
        public Dictionary<string, double> IndividualKeywordScores { get; set; } = new Dictionary<string, double>();
    }
}