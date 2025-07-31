// File: OCRCorrectionService/DocumentCompletenessValidator.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using Serilog;

namespace WaterNut.DataSpace
{
    /// <summary>
    /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v4.2**: Document Completeness Validator
    /// 
    /// **100% COVERAGE MANDATE**: Ensures all input text is properly assigned to detected documents
    /// **MISSING CONTENT DETECTION**: Identifies text segments not covered by any document
    /// **OVERLAP DETECTION**: Finds overlapping document content that may indicate separation errors
    /// **GAP ANALYSIS**: Provides detailed analysis of coverage gaps for AI processing
    /// **VALIDATION METRICS**: Comprehensive completeness statistics and coverage reporting
    /// </summary>
    public class DocumentCompletenessValidator
    {
        private readonly ILogger _logger;

        public DocumentCompletenessValidator(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v4.2**: Validate 100% text coverage by detected documents
        /// 
        /// **COMPLETENESS ANALYSIS**: Ensure every character of input text is assigned to a document
        /// **GAP DETECTION**: Identify missing text segments that need additional document detection
        /// **OVERLAP ANALYSIS**: Find overlapping content that may indicate separation issues
        /// **COVERAGE METRICS**: Detailed statistics on text coverage and document assignment
        /// </summary>
        public async Task<CompletenessValidationResult> ValidateCompletenessAsync(string originalText, List<DetectedDocument> detectedDocuments)
        {
            _logger.Information("📊 **COMPLETENESS_VALIDATION_START**: Analyzing text coverage by detected documents");
            _logger.Information("   - **ORIGINAL_TEXT_LENGTH**: {Length} characters", originalText?.Length ?? 0);
            _logger.Information("   - **DETECTED_DOCUMENTS**: {Count} documents", detectedDocuments?.Count ?? 0);
            
            var result = new CompletenessValidationResult
            {
                OriginalTextLength = originalText?.Length ?? 0,
                DetectedDocumentCount = detectedDocuments?.Count ?? 0
            };
            
            if (string.IsNullOrWhiteSpace(originalText))
            {
                _logger.Warning("⚠️ **EMPTY_INPUT_TEXT**: No text provided for completeness validation");
                result.CoveragePercentage = 100.0; // Empty text is 100% covered
                return result;
            }
            
            if (detectedDocuments == null || !detectedDocuments.Any())
            {
                _logger.Warning("⚠️ **NO_DETECTED_DOCUMENTS**: No documents provided for coverage analysis");
                result.CoveragePercentage = 0.0;
                result.MissingTextLength = originalText.Length;
                result.MissingText = originalText;
                return result;
            }
            
            try
            {
                // **STEP 1: COVERAGE MAPPING** - Create byte-by-byte coverage map
                var coverageMap = await CreateCoverageMapAsync(originalText, detectedDocuments);
                result.CoverageMap = coverageMap;
                
                // **STEP 2: CALCULATE COVERAGE STATISTICS**
                var stats = CalculateCoverageStatistics(coverageMap, originalText.Length);
                result.CoveragePercentage = stats.CoveragePercentage;
                result.CoveredCharacters = stats.CoveredCharacters;
                result.UncoveredCharacters = stats.UncoveredCharacters;
                result.OverlappedCharacters = stats.OverlappedCharacters;
                
                // **STEP 3: EXTRACT MISSING TEXT SEGMENTS**
                var missingSegments = ExtractMissingTextSegments(originalText, coverageMap);
                result.MissingTextSegments = missingSegments;
                result.MissingTextLength = missingSegments.Sum(s => s.Length);
                result.MissingText = string.Join("\n---SEGMENT_BREAK---\n", missingSegments.Select(s => s.Content));
                
                // **STEP 4: DETECT OVERLAPPING DOCUMENTS**
                var overlaps = DetectDocumentOverlaps(detectedDocuments);
                result.DocumentOverlaps = overlaps;
                
                // **STEP 5: ANALYZE COVERAGE QUALITY**
                result.CoverageQuality = AnalyzeCoverageQuality(result);
                
                _logger.Information("📊 **COMPLETENESS_ANALYSIS_COMPLETE**: {Coverage:F1}% coverage, {Missing} chars missing", 
                    result.CoveragePercentage, result.MissingTextLength);
                
                if (result.CoveragePercentage < 100.0)
                {
                    _logger.Warning("⚠️ **INCOMPLETE_COVERAGE**: {Missing} characters not assigned to any document", 
                        result.MissingTextLength);
                    _logger.Information("🔍 **MISSING_SEGMENTS**: {Count} missing text segments identified", 
                        result.MissingTextSegments.Count);
                }
                
                if (result.OverlappedCharacters > 0)
                {
                    _logger.Warning("⚠️ **DOCUMENT_OVERLAPS**: {Overlapped} characters appear in multiple documents", 
                        result.OverlappedCharacters);
                }
                
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **COMPLETENESS_VALIDATION_ERROR**: Error validating document completeness");
                result.HasError = true;
                result.ErrorMessage = ex.Message;
            }
            
            return result;
        }

        /// <summary>
        /// **COVERAGE MAPPING**: Create byte-by-byte coverage map of original text
        /// </summary>
        private async Task<CoverageMap> CreateCoverageMapAsync(string originalText, List<DetectedDocument> documents)
        {
            _logger.Verbose("🗺️ **CREATING_COVERAGE_MAP**: Mapping {Length} characters across {Documents} documents", 
                originalText.Length, documents.Count);
            
            var coverageMap = new CoverageMap(originalText.Length);
            
            // **DOCUMENT COVERAGE MAPPING**: Mark covered positions for each document
            foreach (var document in documents.Where(d => d.Content != null))
            {
                await MapDocumentCoverageAsync(coverageMap, originalText, document);
            }
            
            _logger.Verbose("✅ **COVERAGE_MAP_CREATED**: {Covered} positions covered, {Uncovered} uncovered", 
                coverageMap.CoveredPositions, coverageMap.UncoveredPositions);
            
            return coverageMap;
        }

        /// <summary>
        /// **DOCUMENT COVERAGE MAPPING**: Map one document's coverage against original text
        /// </summary>
        private async Task MapDocumentCoverageAsync(CoverageMap coverageMap, string originalText, DetectedDocument document)
        {
            try
            {
                // **CONTENT LOCATION**: Find where document content appears in original text
                var contentPositions = FindContentPositions(originalText, document.Content);
                
                foreach (var position in contentPositions)
                {
                    // **MARK COVERAGE**: Mark characters as covered by this document
                    for (int i = 0; i < document.Content.Length && (position.Start + i) < originalText.Length; i++)
                    {
                        var textPos = position.Start + i;
                        coverageMap.MarkCovered(textPos, document.DocumentType);
                    }
                }
                
                _logger.Verbose("📍 **DOCUMENT_MAPPED**: {DocumentType} covers {Positions} positions", 
                    document.DocumentType, contentPositions.Sum(p => Math.Min(document.Content.Length, originalText.Length - p.Start)));
                
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **DOCUMENT_MAPPING_ERROR**: Error mapping coverage for {DocumentType}", document.DocumentType);
            }
        }

        /// <summary>
        /// **CONTENT POSITION FINDER**: Find all positions where document content appears in original text
        /// </summary>
        private List<ContentPosition> FindContentPositions(string originalText, string documentContent)
        {
            var positions = new List<ContentPosition>();
            
            if (string.IsNullOrWhiteSpace(documentContent) || documentContent.Length > originalText.Length)
            {
                return positions;
            }
            
            // **EXACT MATCH SEARCH**: Find exact content matches
            var startIndex = 0;
            while (startIndex <= originalText.Length - documentContent.Length)
            {
                var index = originalText.IndexOf(documentContent, startIndex, StringComparison.Ordinal);
                if (index == -1) break;
                
                positions.Add(new ContentPosition
                {
                    Start = index,
                    Length = documentContent.Length,
                    MatchType = "Exact"
                });
                
                startIndex = index + 1; // Look for overlapping matches
            }
            
            // **FUZZY MATCH SEARCH**: If no exact matches, try fuzzy matching for partial content
            if (!positions.Any() && documentContent.Length > 100)
            {
                var fuzzyPositions = FindFuzzyContentPositions(originalText, documentContent);
                positions.AddRange(fuzzyPositions);
            }
            
            return positions;
        }

        /// <summary>
        /// **FUZZY CONTENT MATCHING**: Find approximate content positions for partial matches
        /// </summary>
        private List<ContentPosition> FindFuzzyContentPositions(string originalText, string documentContent)
        {
            var positions = new List<ContentPosition>();
            
            try
            {
                // **CHUNK MATCHING**: Break document content into chunks and find matches
                var chunkSize = Math.Min(50, documentContent.Length / 4);
                var chunks = SplitIntoChunks(documentContent, chunkSize);
                
                foreach (var chunk in chunks.Where(c => c.Length >= 20)) // Minimum chunk size
                {
                    var chunkPositions = originalText.IndexOf(chunk, StringComparison.Ordinal);
                    if (chunkPositions != -1)
                    {
                        positions.Add(new ContentPosition
                        {
                            Start = chunkPositions,
                            Length = chunk.Length,
                            MatchType = "Fuzzy"
                        });
                    }
                }
                
            }
            catch (Exception ex)
            {
                _logger.Verbose("⚠️ **FUZZY_MATCHING_ERROR**: Error in fuzzy content matching: {Error}", ex.Message);
            }
            
            return positions;
        }

        /// <summary>
        /// **TEXT CHUNKING**: Split text into overlapping chunks for fuzzy matching
        /// </summary>
        private List<string> SplitIntoChunks(string text, int chunkSize)
        {
            var chunks = new List<string>();
            var overlap = chunkSize / 4; // 25% overlap
            
            for (int i = 0; i < text.Length; i += chunkSize - overlap)
            {
                var remainingLength = text.Length - i;
                var actualChunkSize = Math.Min(chunkSize, remainingLength);
                
                if (actualChunkSize >= 20) // Minimum useful chunk size
                {
                    chunks.Add(text.Substring(i, actualChunkSize));
                }
            }
            
            return chunks;
        }

        /// <summary>
        /// **COVERAGE STATISTICS**: Calculate detailed coverage metrics
        /// </summary>
        private CoverageStatistics CalculateCoverageStatistics(CoverageMap coverageMap, int totalLength)
        {
            return new CoverageStatistics
            {
                TotalCharacters = totalLength,
                CoveredCharacters = coverageMap.CoveredPositions,
                UncoveredCharacters = coverageMap.UncoveredPositions,
                OverlappedCharacters = coverageMap.OverlappedPositions,
                CoveragePercentage = totalLength > 0 ? (double)coverageMap.CoveredPositions / totalLength * 100.0 : 100.0
            };
        }

        /// <summary>
        /// **MISSING TEXT EXTRACTION**: Extract text segments not covered by any document
        /// </summary>
        private List<MissingTextSegment> ExtractMissingTextSegments(string originalText, CoverageMap coverageMap)
        {
            var segments = new List<MissingTextSegment>();
            var currentSegmentStart = -1;
            
            for (int i = 0; i < originalText.Length; i++)
            {
                if (!coverageMap.IsCovered(i))
                {
                    // **START NEW SEGMENT**: Uncovered character found
                    if (currentSegmentStart == -1)
                    {
                        currentSegmentStart = i;
                    }
                }
                else
                {
                    // **END CURRENT SEGMENT**: Covered character found
                    if (currentSegmentStart != -1)
                    {
                        var segmentLength = i - currentSegmentStart;
                        var segmentContent = originalText.Substring(currentSegmentStart, segmentLength);
                        
                        segments.Add(new MissingTextSegment
                        {
                            StartPosition = currentSegmentStart,
                            Length = segmentLength,
                            Content = segmentContent,
                            LineNumber = CalculateLineNumber(originalText, currentSegmentStart)
                        });
                        
                        currentSegmentStart = -1;
                    }
                }
            }
            
            // **HANDLE FINAL SEGMENT**: If text ends with uncovered content
            if (currentSegmentStart != -1)
            {
                var segmentLength = originalText.Length - currentSegmentStart;
                var segmentContent = originalText.Substring(currentSegmentStart, segmentLength);
                
                segments.Add(new MissingTextSegment
                {
                    StartPosition = currentSegmentStart,
                    Length = segmentLength,
                    Content = segmentContent,
                    LineNumber = CalculateLineNumber(originalText, currentSegmentStart)
                });
            }
            
            return segments;
        }

        /// <summary>
        /// **LINE NUMBER CALCULATION**: Calculate line number for a character position
        /// </summary>
        private int CalculateLineNumber(string text, int position)
        {
            if (position < 0 || position >= text.Length) return 1;
            
            return text.Substring(0, position).Count(c => c == '\n') + 1;
        }

        /// <summary>
        /// **OVERLAP DETECTION**: Find overlapping content between documents
        /// </summary>
        private List<DocumentOverlap> DetectDocumentOverlaps(List<DetectedDocument> documents)
        {
            var overlaps = new List<DocumentOverlap>();
            
            for (int i = 0; i < documents.Count; i++)
            {
                for (int j = i + 1; j < documents.Count; j++)
                {
                    var overlap = FindDocumentOverlap(documents[i], documents[j]);
                    if (overlap != null)
                    {
                        overlaps.Add(overlap);
                    }
                }
            }
            
            return overlaps;
        }

        /// <summary>
        /// **DOCUMENT OVERLAP FINDER**: Find overlap between two documents
        /// </summary>
        private DocumentOverlap FindDocumentOverlap(DetectedDocument doc1, DetectedDocument doc2)
        {
            // **POSITION-BASED OVERLAP**: Check if document positions overlap
            var doc1End = doc1.StartPosition + doc1.Length;
            var doc2End = doc2.StartPosition + doc2.Length;
            
            var overlapStart = Math.Max(doc1.StartPosition, doc2.StartPosition);
            var overlapEnd = Math.Min(doc1End, doc2End);
            
            if (overlapStart < overlapEnd)
            {
                return new DocumentOverlap
                {
                    Document1Type = doc1.DocumentType,
                    Document2Type = doc2.DocumentType,
                    OverlapStart = overlapStart,
                    OverlapLength = overlapEnd - overlapStart,
                    OverlapType = "Positional"
                };
            }
            
            return null;
        }

        /// <summary>
        /// **COVERAGE QUALITY ANALYSIS**: Analyze overall quality of document coverage
        /// </summary>
        private CoverageQuality AnalyzeCoverageQuality(CompletenessValidationResult result)
        {
            var quality = new CoverageQuality();
            
            // **COVERAGE SCORE**: Based on percentage covered
            if (result.CoveragePercentage >= 100.0)
            {
                quality.Score = 1.0;
                quality.Rating = "Perfect";
            }
            else if (result.CoveragePercentage >= 95.0)
            {
                quality.Score = 0.9;
                quality.Rating = "Excellent";
            }
            else if (result.CoveragePercentage >= 85.0)
            {
                quality.Score = 0.7;
                quality.Rating = "Good";
            }
            else if (result.CoveragePercentage >= 70.0)
            {
                quality.Score = 0.5;
                quality.Rating = "Fair";
            }
            else
            {
                quality.Score = 0.3;
                quality.Rating = "Poor";
            }
            
            // **QUALITY FACTORS**: Analyze factors affecting quality
            quality.Issues = new List<string>();
            
            if (result.MissingTextLength > 100)
            {
                quality.Issues.Add($"Significant missing content: {result.MissingTextLength} characters");
            }
            
            if (result.OverlappedCharacters > 0)
            {
                quality.Issues.Add($"Document overlaps detected: {result.OverlappedCharacters} characters");
            }
            
            if (result.MissingTextSegments.Count > 5)
            {
                quality.Issues.Add($"Fragmented coverage: {result.MissingTextSegments.Count} missing segments");
            }
            
            return quality;
        }
    }

    // Supporting classes and data structures for completeness validation...
    
    public class CompletenessValidationResult
    {
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
        public int OriginalTextLength { get; set; }
        public int DetectedDocumentCount { get; set; }
        public double CoveragePercentage { get; set; }
        public int CoveredCharacters { get; set; }
        public int UncoveredCharacters { get; set; }
        public int OverlappedCharacters { get; set; }
        public int MissingTextLength { get; set; }
        public string MissingText { get; set; }
        public List<MissingTextSegment> MissingTextSegments { get; set; } = new List<MissingTextSegment>();
        public List<DocumentOverlap> DocumentOverlaps { get; set; } = new List<DocumentOverlap>();
        public CoverageMap CoverageMap { get; set; }
        public CoverageQuality CoverageQuality { get; set; }
    }

    public class CoverageMap
    {
        private readonly bool[] _covered;
        private readonly List<string>[] _coveredBy;
        
        public CoverageMap(int length)
        {
            _covered = new bool[length];
            _coveredBy = new List<string>[length];
            for (int i = 0; i < length; i++)
            {
                _coveredBy[i] = new List<string>();
            }
        }
        
        public void MarkCovered(int position, string documentType)
        {
            if (position >= 0 && position < _covered.Length)
            {
                _covered[position] = true;
                _coveredBy[position].Add(documentType);
            }
        }
        
        public bool IsCovered(int position) => position >= 0 && position < _covered.Length && _covered[position];
        public List<string> GetCoveringDocuments(int position) => position >= 0 && position < _coveredBy.Length ? _coveredBy[position] : new List<string>();
        
        public int CoveredPositions => _covered.Count(c => c);
        public int UncoveredPositions => _covered.Count(c => !c);
        public int OverlappedPositions => _coveredBy.Count(docs => docs.Count > 1);
    }

    public class MissingTextSegment
    {
        public int StartPosition { get; set; }
        public int Length { get; set; }
        public string Content { get; set; }
        public int LineNumber { get; set; }
    }

    public class DocumentOverlap
    {
        public string Document1Type { get; set; }
        public string Document2Type { get; set; }
        public int OverlapStart { get; set; }
        public int OverlapLength { get; set; }
        public string OverlapType { get; set; }
    }

    public class ContentPosition
    {
        public int Start { get; set; }
        public int Length { get; set; }
        public string MatchType { get; set; }
    }

    public class CoverageStatistics
    {
        public int TotalCharacters { get; set; }
        public int CoveredCharacters { get; set; }
        public int UncoveredCharacters { get; set; }
        public int OverlappedCharacters { get; set; }
        public double CoveragePercentage { get; set; }
    }

    public class CoverageQuality
    {
        public double Score { get; set; }
        public string Rating { get; set; }
        public List<string> Issues { get; set; } = new List<string>();
    }
}