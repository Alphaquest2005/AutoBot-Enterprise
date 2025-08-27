// File: OCRCorrectionService/DocumentLearningSystem.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Data.Entity;
using OCR.Business.Entities;
using Serilog;

namespace WaterNut.DataSpace
{
    /// <summary>
    /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v4.2**: Document Learning System
    /// 
    /// **SELF-IMPROVING ARCHITECTURE**: AI discoveries automatically improve database detection speed
    /// **KEYWORD LEARNING**: AI-detected keywords added to OCR_TemplateTableMapping for future fast detection
    /// **INTELLIGENT DEDUPLICATION**: Prevents duplicate keywords and optimizes existing mappings
    /// **THRESHOLD OPTIMIZATION**: Adjusts MatchThreshold based on detection success rates
    /// **PATTERN EVOLUTION**: Document detection patterns evolve and improve over time
    /// **LEARNING ANALYTICS**: Tracks learning progress and system improvement metrics
    /// </summary>
    public class DocumentLearningSystem
    {
        private readonly ILogger _logger;
        private readonly OCRContext _context;

        public DocumentLearningSystem(ILogger logger, OCRContext context)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _context = context ?? throw new ArgumentNullException(nameof(context));
        }

        /// <summary>
        /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v4.2**: Learn from AI detections to improve database speed
        /// 
        /// **LEARNING WORKFLOW**: AI detections → Keyword extraction → Database updates → Performance improvement
        /// **INTELLIGENT UPDATES**: Add new mappings or enhance existing ones with AI-discovered keywords
        /// **DEDUPLICATION**: Prevent duplicate keywords and optimize keyword lists
        /// **THRESHOLD TUNING**: Adjust MatchThreshold based on actual detection success
        /// **LEARNING METRICS**: Track system improvement and learning effectiveness
        /// </summary>
        public async Task LearnFromAIDetectionsAsync(IEnumerable<DetectedDocument> aiDetections, string originalText)
        {
            _logger.Information("🧠 **LEARNING_SYSTEM_START**: Processing AI detections for database improvement");
            _logger.Information("   - **AI_DETECTIONS**: {Count} documents", aiDetections?.Count() ?? 0);
            _logger.Information("   - **LEARNING_MODE**: Keywords → Database → Future Speed");
            
            if (aiDetections == null || !aiDetections.Any())
            {
                _logger.Information("ℹ️ **NO_AI_DETECTIONS**: No AI detections to learn from");
                return;
            }
            
            var learningResults = new List<LearningResult>();
            
            try
            {
                foreach (var detection in aiDetections)
                {
                    _logger.Information("🔍 **LEARNING_FROM_DETECTION**: {DocumentType} with {Keywords} keywords", 
                        detection.DocumentType, detection.MatchedKeywords?.Count ?? 0);
                    
                    var result = await ProcessSingleDetectionLearningAsync(detection, originalText);
                    learningResults.Add(result);
                    
                    _logger.Information("   - **LEARNING_RESULT**: {Action} - {Message}", result.Action, result.Message);
                }
                
                // **LEARNING ANALYTICS**: Calculate and log improvement metrics
                await LogLearningAnalyticsAsync(learningResults);
                
                _logger.Information("✅ **LEARNING_SYSTEM_COMPLETE**: Processed {Count} detections, {Updates} database updates", 
                    aiDetections.Count(), learningResults.Count(r => r.DatabaseUpdated));
                
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **LEARNING_SYSTEM_ERROR**: Error in learning from AI detections");
                throw;
            }
        }

        /// <summary>
        /// **SINGLE DETECTION LEARNING**: Process one AI detection for database learning
        /// </summary>
        private async Task<LearningResult> ProcessSingleDetectionLearningAsync(DetectedDocument detection, string originalText)
        {
            var result = new LearningResult
            {
                DocumentType = detection.DocumentType,
                DetectedKeywords = detection.MatchedKeywords ?? new List<string>(),
                OriginalConfidence = detection.Confidence
            };
            
            try
            {
                // **STEP 1: CHECK EXISTING MAPPING**
                var existingMapping = await FindExistingMappingAsync(detection.DocumentType);
                
                if (existingMapping != null)
                {
                    // **ENHANCE EXISTING MAPPING**: Add new keywords to existing mapping
                    result = await EnhanceExistingMappingAsync(existingMapping, detection, result);
                }
                else
                {
                    // **CREATE NEW MAPPING**: Create new template table mapping
                    result = await CreateNewMappingAsync(detection, originalText, result);
                }
                
                // **STEP 2: OPTIMIZE MATCH THRESHOLD** (based on detection confidence)
                if (result.DatabaseUpdated)
                {
                    await OptimizeMatchThresholdAsync(result.MappingId.Value, detection.Confidence);
                }
                
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **SINGLE_LEARNING_ERROR**: Error learning from {DocumentType}", detection.DocumentType);
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            
            return result;
        }

        /// <summary>
        /// **FIND EXISTING MAPPING**: Check if document type already has a mapping
        /// </summary>
        private async Task<OCR_TemplateTableMapping> FindExistingMappingAsync(string documentType)
        {
            return await _context.OCR_TemplateTableMapping
                .FirstOrDefaultAsync(m => m.DocumentType.ToLower() == documentType.ToLower() && m.IsActive);
        }

        /// <summary>
        /// **ENHANCE EXISTING MAPPING**: Add AI keywords to existing mapping
        /// </summary>
        private async Task<LearningResult> EnhanceExistingMappingAsync(OCR_TemplateTableMapping mapping, DetectedDocument detection, LearningResult result)
        {
            _logger.Information("🔧 **ENHANCING_EXISTING_MAPPING**: {DocumentType} (ID: {Id})", mapping.DocumentType, mapping.Id);
            
            try
            {
                // **PARSE EXISTING KEYWORDS**
                var existingKeywords = ParseExistingKeywords(mapping.Keywords);
                var newKeywords = detection.MatchedKeywords ?? new List<string>();
                
                // **INTELLIGENT DEDUPLICATION**: Add only new keywords
                var keywordsToAdd = newKeywords
                    .Where(k => !string.IsNullOrWhiteSpace(k))
                    .Where(k => !existingKeywords.Any(ek => ek.Equals(k, StringComparison.OrdinalIgnoreCase)))
                    .ToList();
                
                if (keywordsToAdd.Any())
                {
                    // **UPDATE KEYWORDS**: Combine existing and new keywords
                    var combinedKeywords = existingKeywords.Concat(keywordsToAdd).ToList();
                    var updatedKeywordsString = string.Join(", ", combinedKeywords);
                    
                    mapping.Keywords = updatedKeywordsString;
                    mapping.LastUpdated = DateTime.Now;
                    mapping.ProcessingNotes = $"Enhanced by AI learning on {DateTime.Now:yyyy-MM-dd HH:mm} - Added {keywordsToAdd.Count} new keywords";
                    
                    await _context.SaveChangesAsync();
                    
                    result.Action = "Enhanced";
                    result.Message = $"Added {keywordsToAdd.Count} new keywords to existing mapping";
                    result.AddedKeywords = keywordsToAdd;
                    result.DatabaseUpdated = true;
                    result.MappingId = mapping.Id;
                    result.Success = true;
                    
                    _logger.Information("✅ **MAPPING_ENHANCED**: Added keywords: {Keywords}", string.Join(", ", keywordsToAdd));
                }
                else
                {
                    result.Action = "NoChange";
                    result.Message = "All AI keywords already exist in mapping";
                    result.Success = true;
                    
                    _logger.Information("ℹ️ **NO_NEW_KEYWORDS**: All AI keywords already present in mapping");
                }
                
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **ENHANCEMENT_ERROR**: Error enhancing mapping {MappingId}", mapping.Id);
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            
            return result;
        }

        /// <summary>
        /// **CREATE NEW MAPPING**: Create new template table mapping for unknown document type
        /// </summary>
        private async Task<LearningResult> CreateNewMappingAsync(DetectedDocument detection, string originalText, LearningResult result)
        {
            _logger.Information("🆕 **CREATING_NEW_MAPPING**: {DocumentType}", detection.DocumentType);
            
            try
            {
                var keywords = detection.MatchedKeywords ?? new List<string>();
                if (!keywords.Any())
                {
                    // **FALLBACK KEYWORD GENERATION**: Extract keywords from document type name
                    keywords = GenerateFallbackKeywords(detection.DocumentType);
                }
                
                var newMapping = new OCR_TemplateTableMapping
                {
                    DocumentType = detection.DocumentType,
                    TargetTable = "ShipmentInvoice", // Default target table
                    Keywords = string.Join(", ", keywords),
                    MatchThreshold = CalculateInitialMatchThreshold(detection.Confidence),
                    Priority = 100, // Low priority for AI-created mappings initially
                    IsActive = true,
                    Description = $"Auto-created by AI learning system on {DateTime.Now:yyyy-MM-dd HH:mm}",
                    CreatedDate = DateTime.Now,
                    LastUpdated = DateTime.Now,
                    CreatedBy = "AI_Learning_System",
                    ProcessingNotes = $"Created from AI detection with {keywords.Count} keywords and {detection.Confidence:F2} confidence",
                    IsSystemEntity = false,
                    RequiredFields = "InvoiceTotal", // Default required fields
                    OptionalFields = "SubTotal,TotalInternalFreight,TotalOtherCost,TotalInsurance,TotalDeduction"
                };
                
                _context.OCR_TemplateTableMapping.Add(newMapping);
                await _context.SaveChangesAsync();
                
                result.Action = "Created";
                result.Message = $"Created new mapping with {keywords.Count} keywords";
                result.AddedKeywords = keywords;
                result.DatabaseUpdated = true;
                result.MappingId = newMapping.Id;
                result.Success = true;
                
                _logger.Information("✅ **NEW_MAPPING_CREATED**: ID {Id} with keywords: {Keywords}", 
                    newMapping.Id, string.Join(", ", keywords));
                
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **NEW_MAPPING_ERROR**: Error creating mapping for {DocumentType}", detection.DocumentType);
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            
            return result;
        }

        /// <summary>
        /// **PARSE EXISTING KEYWORDS**: Extract keywords from database Keywords field
        /// </summary>
        private List<string> ParseExistingKeywords(string keywordsString)
        {
            if (string.IsNullOrWhiteSpace(keywordsString))
                return new List<string>();
            
            var separators = new[] { ',', ';', '|', '\n', '\r' };
            return keywordsString
                .Split(separators, StringSplitOptions.RemoveEmptyEntries)
                .Select(k => k.Trim())
                .Where(k => !string.IsNullOrWhiteSpace(k))
                .ToList();
        }

        /// <summary>
        /// **FALLBACK KEYWORD GENERATION**: Generate keywords from document type name
        /// </summary>
        private List<string> GenerateFallbackKeywords(string documentType)
        {
            var keywords = new List<string>();
            
            if (string.IsNullOrWhiteSpace(documentType))
                return keywords;
            
            // **DOCUMENT TYPE PARSING**: Extract meaningful terms from document type
            var parts = documentType.Split(new[] { '_', '-', ' ' }, StringSplitOptions.RemoveEmptyEntries);
            
            foreach (var part in parts)
            {
                if (part.Length >= 3) // Minimum keyword length
                {
                    keywords.Add(part);
                }
            }
            
            // **COMMON PATTERNS**: Add common document keywords if type suggests them
            var lowerType = documentType.ToLowerInvariant();
            if (lowerType.Contains("invoice"))
            {
                keywords.AddRange(new[] { "Invoice", "Total", "Amount", "Date" });
            }
            if (lowerType.Contains("customs"))
            {
                keywords.AddRange(new[] { "Customs", "Declaration", "Import", "Export" });
            }
            if (lowerType.Contains("shipping"))
            {
                keywords.AddRange(new[] { "Shipping", "Freight", "Delivery" });
            }
            
            return keywords.Distinct().ToList();
        }

        /// <summary>
        /// **INITIAL MATCH THRESHOLD CALCULATION**: Calculate threshold based on AI confidence
        /// </summary>
        private decimal CalculateInitialMatchThreshold(double aiConfidence)
        {
            // **CONSERVATIVE APPROACH**: Set threshold slightly below AI confidence to allow matches
            var threshold = Math.Max(0.3, aiConfidence * 0.8); // 80% of AI confidence, minimum 0.3
            return (decimal)Math.Round(threshold, 4);
        }

        /// <summary>
        /// **MATCH THRESHOLD OPTIMIZATION**: Adjust threshold based on detection success
        /// </summary>
        private async Task OptimizeMatchThresholdAsync(int mappingId, double detectionConfidence)
        {
            try
            {
                var mapping = await _context.OCR_TemplateTableMapping.FindAsync(mappingId);
                if (mapping == null) return;
                
                // **RECENT SUCCESS ANALYSIS**: Check recent detection success for this mapping
                var recentLogs = await _context.OCR_KeywordDetectionLog
                    .Where(log => log.DetectedMappingId == mappingId && 
                                 log.CreatedDate >= DateTime.Now.AddDays(-7))
                    .ToListAsync();
                
                if (recentLogs.Count >= 5) // Minimum data points for optimization
                {
                    var successRate = (double)recentLogs.Count(log => log.Success) / recentLogs.Count;
                    var averageScore = recentLogs.Average(log => (double)log.MatchScore);
                    
                    // **THRESHOLD ADJUSTMENT**: Optimize based on success rate and average score
                    var currentThreshold = (double)mapping.MatchThreshold;
                    var suggestedThreshold = CalculateOptimalThreshold(successRate, averageScore, currentThreshold);
                    
                    if (Math.Abs(suggestedThreshold - currentThreshold) > 0.05) // Significant change
                    {
                        mapping.MatchThreshold = (decimal)suggestedThreshold;
                        mapping.LastUpdated = DateTime.Now;
                        mapping.ProcessingNotes += $" | Threshold optimized from {currentThreshold:F3} to {suggestedThreshold:F3} based on {recentLogs.Count} detections";
                        
                        await _context.SaveChangesAsync();
                        
                        _logger.Information("⚡ **THRESHOLD_OPTIMIZED**: Mapping {Id} threshold updated from {Old:F3} to {New:F3}", 
                            mappingId, currentThreshold, suggestedThreshold);
                    }
                }
                
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **THRESHOLD_OPTIMIZATION_ERROR**: Error optimizing threshold for mapping {MappingId}", mappingId);
            }
        }

        /// <summary>
        /// **OPTIMAL THRESHOLD CALCULATION**: Calculate optimal threshold based on performance data
        /// </summary>
        private double CalculateOptimalThreshold(double successRate, double averageScore, double currentThreshold)
        {
            // **ADAPTIVE OPTIMIZATION**: Adjust threshold based on success patterns
            if (successRate > 0.9) // High success rate - can increase threshold for precision
            {
                return Math.Min(0.9, currentThreshold + 0.05);
            }
            else if (successRate < 0.7) // Low success rate - decrease threshold for recall
            {
                return Math.Max(0.3, currentThreshold - 0.05);
            }
            else // Good success rate - optimize based on average score
            {
                return Math.Max(0.3, Math.Min(0.9, averageScore * 0.9));
            }
        }

        /// <summary>
        /// **LEARNING ANALYTICS**: Log learning progress and system improvement metrics
        /// </summary>
        private async Task LogLearningAnalyticsAsync(List<LearningResult> results)
        {
            try
            {
                var totalDetections = results.Count;
                var databaseUpdates = results.Count(r => r.DatabaseUpdated);
                var newMappings = results.Count(r => r.Action == "Created");
                var enhancedMappings = results.Count(r => r.Action == "Enhanced");
                var totalNewKeywords = results.Sum(r => r.AddedKeywords?.Count ?? 0);
                
                _logger.Information("📊 **LEARNING_ANALYTICS**: Learning session complete");
                _logger.Information("   - **TOTAL_AI_DETECTIONS**: {Total}", totalDetections);
                _logger.Information("   - **DATABASE_UPDATES**: {Updates} ({Percentage:F1}%)", 
                    databaseUpdates, totalDetections > 0 ? (double)databaseUpdates / totalDetections * 100 : 0);
                _logger.Information("   - **NEW_MAPPINGS_CREATED**: {New}", newMappings);
                _logger.Information("   - **EXISTING_MAPPINGS_ENHANCED**: {Enhanced}", enhancedMappings);
                _logger.Information("   - **TOTAL_NEW_KEYWORDS_ADDED**: {Keywords}", totalNewKeywords);
                
                // **IMPROVEMENT METRICS**: Calculate system improvement
                var currentMappingCount = await _context.OCR_TemplateTableMapping.CountAsync(m => m.IsActive);
                _logger.Information("   - **CURRENT_ACTIVE_MAPPINGS**: {Count}", currentMappingCount);
                
                if (databaseUpdates > 0)
                {
                    _logger.Information("🎯 **SYSTEM_IMPROVEMENT**: Database detection will be faster for {Types} document types", 
                        string.Join(", ", results.Where(r => r.DatabaseUpdated).Select(r => r.DocumentType)));
                }
                
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **ANALYTICS_ERROR**: Error logging learning analytics");
            }
        }
    }

    /// <summary>
    /// **LEARNING RESULT**: Result of processing one AI detection for learning
    /// </summary>
    public class LearningResult
    {
        public bool Success { get; set; }
        public string ErrorMessage { get; set; }
        public string DocumentType { get; set; }
        public string Action { get; set; } // "Created", "Enhanced", "NoChange"
        public string Message { get; set; }
        public bool DatabaseUpdated { get; set; }
        public int? MappingId { get; set; }
        public List<string> DetectedKeywords { get; set; } = new List<string>();
        public List<string> AddedKeywords { get; set; } = new List<string>();
        public double OriginalConfidence { get; set; }
    }
}