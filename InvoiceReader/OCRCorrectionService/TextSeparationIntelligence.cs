// File: OCRCorrectionService/TextSeparationIntelligence.cs
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
    /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v4.2**: Text Separation Intelligence
    /// 
    /// **AI-POWERED SEPARATION**: Analyzes document boundaries and separation patterns for mixed content
    /// **REGEX PATTERN GENERATION**: Creates regex patterns for automatic document boundary detection
    /// **SEPARATION LEARNING**: Learns successful separation patterns for future document processing
    /// **BOUNDARY ANALYSIS**: Identifies headers, footers, page breaks, and content transitions
    /// **PATTERN OPTIMIZATION**: Evolves separation logic based on document structure analysis
    /// </summary>
    public class TextSeparationIntelligence
    {
        private readonly ILogger _logger;

        public TextSeparationIntelligence(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v4.2**: Analyze separation patterns for document boundaries
        /// 
        /// **PATTERN ANALYSIS**: Identify separation patterns, boundaries, and content transitions
        /// **REGEX GENERATION**: Create regex patterns for automated document separation
        /// **BOUNDARY DETECTION**: Find headers, footers, page breaks, and document transitions
        /// **LEARNING READY**: Generate reusable patterns for future document separation
        /// </summary>
        public async Task<SeparationAnalysisResult> AnalyzeSeparationPatternsAsync(string originalText, List<DetectedDocument> detectedDocuments)
        {
            _logger.Information("🔍 **SEPARATION_ANALYSIS_START**: Analyzing text separation patterns and document boundaries");
            _logger.Information("   - **TEXT_LENGTH**: {Length} characters", originalText?.Length ?? 0);
            _logger.Information("   - **DOCUMENTS**: {Count} detected documents", detectedDocuments?.Count ?? 0);
            
            var result = new SeparationAnalysisResult
            {
                OriginalTextLength = originalText?.Length ?? 0,
                DetectedDocumentCount = detectedDocuments?.Count ?? 0
            };
            
            if (string.IsNullOrWhiteSpace(originalText) || detectedDocuments == null || !detectedDocuments.Any())
            {
                _logger.Warning("⚠️ **INSUFFICIENT_DATA**: Not enough data for separation analysis");
                return result;
            }
            
            try
            {
                // **STEP 1: DOCUMENT BOUNDARY ANALYSIS**
                var boundaries = await AnalyzeDocumentBoundariesAsync(originalText, detectedDocuments);
                result.Boundaries = boundaries;
                
                // **STEP 2: SEPARATION PATTERN DETECTION**
                var patterns = await DetectSeparationPatternsAsync(originalText, boundaries);
                result.Patterns = patterns;
                
                // **STEP 3: REGEX SEPARATOR GENERATION**
                var regexSeparators = await GenerateRegexSeparatorsAsync(originalText, patterns);
                result.RegexSeparators = regexSeparators;
                
                // **STEP 4: CONTENT TRANSITION ANALYSIS**
                var transitions = await AnalyzeContentTransitionsAsync(originalText, detectedDocuments);
                result.ContentTransitions = transitions;
                
                // **STEP 5: SEPARATION QUALITY ASSESSMENT**
                result.SeparationQuality = AssessSeparationQuality(result);
                
                _logger.Information("✅ **SEPARATION_ANALYSIS_COMPLETE**: {Patterns} patterns, {Boundaries} boundaries, {Regex} regex separators", 
                    result.Patterns.Count, result.Boundaries.Count, result.RegexSeparators.Count);
                
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **SEPARATION_ANALYSIS_ERROR**: Error analyzing separation patterns");
                result.HasError = true;
                result.ErrorMessage = ex.Message;
            }
            
            return result;
        }

        /// <summary>
        /// **DOCUMENT BOUNDARY ANALYSIS**: Identify boundaries between documents
        /// </summary>
        private async Task<List<DocumentBoundary>> AnalyzeDocumentBoundariesAsync(string text, List<DetectedDocument> documents)
        {
            _logger.Verbose("📏 **BOUNDARY_ANALYSIS**: Analyzing document boundaries");
            
            var boundaries = new List<DocumentBoundary>();
            var lines = text.Split('\n');
            
            // **SORTED DOCUMENTS**: Sort by start position for boundary analysis
            var sortedDocuments = documents.OrderBy(d => d.StartPosition).ToList();
            
            for (int i = 0; i < sortedDocuments.Count - 1; i++)
            {
                var currentDoc = sortedDocuments[i];
                var nextDoc = sortedDocuments[i + 1];
                
                // **BOUNDARY DETECTION**: Find boundary between current and next document
                var boundary = await DetectBoundaryBetweenDocumentsAsync(text, lines, currentDoc, nextDoc);
                if (boundary != null)
                {
                    boundaries.Add(boundary);
                    _logger.Verbose("📍 **BOUNDARY_DETECTED**: Between {Doc1} and {Doc2} at position {Position}", 
                        currentDoc.DocumentType, nextDoc.DocumentType, boundary.StartPosition);
                }
            }
            
            return boundaries;
        }

        /// <summary>
        /// **BOUNDARY DETECTION**: Detect boundary between two consecutive documents
        /// </summary>
        private async Task<DocumentBoundary> DetectBoundaryBetweenDocumentsAsync(string text, string[] lines, DetectedDocument doc1, DetectedDocument doc2)
        {
            var doc1End = doc1.StartPosition + doc1.Length;
            var doc2Start = doc2.StartPosition;
            
            if (doc2Start <= doc1End)
            {
                return null; // No clear boundary (overlapping or adjacent)
            }
            
            // **BOUNDARY CONTENT**: Extract content between documents
            var boundaryContent = text.Substring(doc1End, doc2Start - doc1End);
            var boundaryLines = boundaryContent.Split('\n');
            
            // **BOUNDARY PATTERN ANALYSIS**: Analyze boundary content for patterns
            var boundaryMarkers = AnalyzeBoundaryMarkers(boundaryLines);
            
            return new DocumentBoundary
            {
                StartPosition = doc1End,
                EndPosition = doc2Start,
                DocumentType = $"{doc1.DocumentType}_to_{doc2.DocumentType}",
                Confidence = CalculateBoundaryConfidence(boundaryMarkers),
                BoundaryMarker = string.Join(" | ", boundaryMarkers)
            };
        }

        /// <summary>
        /// **BOUNDARY MARKER ANALYSIS**: Analyze content for boundary markers
        /// </summary>
        private List<string> AnalyzeBoundaryMarkers(string[] boundaryLines)
        {
            var markers = new List<string>();
            
            foreach (var line in boundaryLines)
            {
                var trimmedLine = line.Trim();
                if (string.IsNullOrWhiteSpace(trimmedLine)) continue;
                
                // **COMMON BOUNDARY PATTERNS**
                if (Regex.IsMatch(trimmedLine, @"^-{3,}$")) markers.Add("horizontal_line");
                if (Regex.IsMatch(trimmedLine, @"^={3,}$")) markers.Add("equals_line");
                if (Regex.IsMatch(trimmedLine, @"^_{3,}$")) markers.Add("underscore_line");
                if (Regex.IsMatch(trimmedLine, @"^Page\s+\d+", RegexOptions.IgnoreCase)) markers.Add("page_header");
                if (Regex.IsMatch(trimmedLine, @"^\d+\s+of\s+\d+$", RegexOptions.IgnoreCase)) markers.Add("page_number");
                if (trimmedLine.All(c => char.IsUpper(c) || char.IsWhiteSpace(c) || char.IsPunctuation(c)) && trimmedLine.Length > 5) markers.Add("section_header");
                if (Regex.IsMatch(trimmedLine, @"^\*{3,}$")) markers.Add("asterisk_line");
                if (Regex.IsMatch(trimmedLine, @"^#{3,}$")) markers.Add("hash_line");
            }
            
            return markers.Distinct().ToList();
        }

        /// <summary>
        /// **BOUNDARY CONFIDENCE CALCULATION**: Calculate confidence in boundary detection
        /// </summary>
        private double CalculateBoundaryConfidence(List<string> markers)
        {
            if (!markers.Any()) return 0.3; // Low confidence for no markers
            
            // **STRONG MARKERS**: Some patterns are stronger indicators
            var strongMarkers = new[] { "horizontal_line", "page_header", "section_header" };
            var hasStrongMarker = markers.Any(m => strongMarkers.Contains(m));
            
            var baseConfidence = hasStrongMarker ? 0.8 : 0.6;
            var markerBonus = Math.Min(0.2, markers.Count * 0.05); // Bonus for multiple markers
            
            return Math.Min(0.95, baseConfidence + markerBonus);
        }

        /// <summary>
        /// **SEPARATION PATTERN DETECTION**: Detect recurring patterns used for document separation
        /// </summary>
        private async Task<List<SeparationPattern>> DetectSeparationPatternsAsync(string text, List<DocumentBoundary> boundaries)
        {
            _logger.Verbose("🎯 **PATTERN_DETECTION**: Detecting separation patterns from boundaries");
            
            var patterns = new List<SeparationPattern>();
            var lines = text.Split('\n');
            
            // **BOUNDARY MARKER AGGREGATION**: Collect all boundary markers
            var allMarkers = boundaries.SelectMany(b => b.BoundaryMarker.Split('|')).Select(m => m.Trim()).ToList();
            var markerGroups = allMarkers.GroupBy(m => m).Where(g => g.Count() > 1); // Recurring markers
            
            foreach (var markerGroup in markerGroups)
            {
                var pattern = await GeneratePatternForMarker(markerGroup.Key, text, lines);
                if (pattern != null)
                {
                    patterns.Add(pattern);
                    _logger.Verbose("🎯 **PATTERN_FOUND**: {Type} - {Pattern}", pattern.PatternType, pattern.Pattern);
                }
            }
            
            // **STRUCTURAL PATTERNS**: Detect structural separation patterns
            var structuralPatterns = await DetectStructuralPatternsAsync(text, lines);
            patterns.AddRange(structuralPatterns);
            
            return patterns;
        }

        /// <summary>
        /// **PATTERN GENERATION**: Generate regex pattern for a boundary marker
        /// </summary>
        private async Task<SeparationPattern> GeneratePatternForMarker(string marker, string text, string[] lines)
        {
            switch (marker)
            {
                case "horizontal_line":
                    return new SeparationPattern
                    {
                        PatternType = "Regex",
                        Pattern = @"^-{3,}$",
                        Confidence = 0.9,
                        Description = "Horizontal line separator (3 or more dashes)"
                    };
                    
                case "equals_line":
                    return new SeparationPattern
                    {
                        PatternType = "Regex", 
                        Pattern = @"^={3,}$",
                        Confidence = 0.9,
                        Description = "Equals line separator (3 or more equals)"
                    };
                    
                case "page_header":
                    return new SeparationPattern
                    {
                        PatternType = "Regex",
                        Pattern = @"^Page\s+\d+",
                        Confidence = 0.95,
                        Description = "Page header with page number"
                    };
                    
                case "section_header":
                    return new SeparationPattern
                    {
                        PatternType = "Regex",
                        Pattern = @"^[A-Z\s]{5,}$",
                        Confidence = 0.7,
                        Description = "All-caps section header"
                    };
                    
                default:
                    return null;
            }
        }

        /// <summary>
        /// **STRUCTURAL PATTERN DETECTION**: Detect structural separation patterns
        /// </summary>
        private async Task<List<SeparationPattern>> DetectStructuralPatternsAsync(string text, string[] lines)
        {
            var patterns = new List<SeparationPattern>();
            
            // **EMPTY LINE CLUSTERING**: Detect multiple empty lines as separators
            var emptyLinePattern = DetectEmptyLinePatterns(lines);
            if (emptyLinePattern != null) patterns.Add(emptyLinePattern);
            
            // **INDENTATION CHANGES**: Detect significant indentation changes
            var indentationPattern = DetectIndentationPatterns(lines);
            if (indentationPattern != null) patterns.Add(indentationPattern);
            
            // **FONT SIZE CHANGES**: Detect formatting changes (approximate via line length)
            var formattingPattern = DetectFormattingPatterns(lines);
            if (formattingPattern != null) patterns.Add(formattingPattern);
            
            return patterns;
        }

        /// <summary>
        /// **EMPTY LINE PATTERN DETECTION**: Detect patterns of empty lines as separators
        /// </summary>
        private SeparationPattern DetectEmptyLinePatterns(string[] lines)
        {
            var emptyLineGroups = new List<int>();
            var currentEmptyCount = 0;
            
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line))
                {
                    currentEmptyCount++;
                }
                else
                {
                    if (currentEmptyCount >= 2) // 2+ empty lines
                    {
                        emptyLineGroups.Add(currentEmptyCount);
                    }
                    currentEmptyCount = 0;
                }
            }
            
            if (emptyLineGroups.Count >= 2) // Pattern appears multiple times
            {
                var avgEmptyLines = emptyLineGroups.Average();
                return new SeparationPattern
                {
                    PatternType = "EmptyLines",
                    Pattern = $@"^(\s*\n){{{(int)Math.Round(avgEmptyLines)},}}",
                    Confidence = 0.6,
                    Description = $"Multiple empty lines separator (avg: {avgEmptyLines:F1} lines)"
                };
            }
            
            return null;
        }

        /// <summary>
        /// **INDENTATION PATTERN DETECTION**: Detect indentation-based separation
        /// </summary>
        private SeparationPattern DetectIndentationPatterns(string[] lines)
        {
            var indentationChanges = new List<int>();
            var previousIndent = -1;
            
            foreach (var line in lines)
            {
                if (string.IsNullOrWhiteSpace(line)) continue;
                
                var currentIndent = line.Length - line.TrimStart().Length;
                
                if (previousIndent != -1 && Math.Abs(currentIndent - previousIndent) >= 4)
                {
                    indentationChanges.Add(Math.Abs(currentIndent - previousIndent));
                }
                
                previousIndent = currentIndent;
            }
            
            if (indentationChanges.Count >= 3) // Multiple significant indentation changes
            {
                return new SeparationPattern
                {
                    PatternType = "Indentation",
                    Pattern = @"^(\s{4,}|\S)", // 4+ spaces or non-whitespace
                    Confidence = 0.5,
                    Description = "Significant indentation changes"
                };
            }
            
            return null;
        }

        /// <summary>
        /// **FORMATTING PATTERN DETECTION**: Detect formatting-based separation patterns
        /// </summary>
        private SeparationPattern DetectFormattingPatterns(string[] lines)
        {
            var lineLengths = lines.Where(l => !string.IsNullOrWhiteSpace(l)).Select(l => l.Length).ToList();
            
            if (lineLengths.Count < 10) return null; // Need enough data
            
            var avgLength = lineLengths.Average();
            var shortLines = lineLengths.Count(l => l < avgLength * 0.5);
            var longLines = lineLengths.Count(l => l > avgLength * 1.5);
            
            if (shortLines > lineLengths.Count * 0.1 && longLines > lineLengths.Count * 0.1)
            {
                return new SeparationPattern
                {
                    PatternType = "LineLength",
                    Pattern = $@"^.{{1,{(int)(avgLength * 0.5)}}}$|^.{{{(int)(avgLength * 1.5)},}}$",
                    Confidence = 0.4,
                    Description = "Line length variation patterns"
                };
            }
            
            return null;
        }

        /// <summary>
        /// **REGEX SEPARATOR GENERATION**: Generate regex patterns for document separation
        /// </summary>
        private async Task<List<string>> GenerateRegexSeparatorsAsync(string text, List<SeparationPattern> patterns)
        {
            _logger.Verbose("🔧 **REGEX_GENERATION**: Generating regex separators from patterns");
            
            var regexSeparators = new List<string>();
            
            foreach (var pattern in patterns.Where(p => p.PatternType == "Regex"))
            {
                // **PATTERN VALIDATION**: Test pattern against text
                try
                {
                    var matches = Regex.Matches(text, pattern.Pattern, RegexOptions.Multiline);
                    if (matches.Count > 0)
                    {
                        regexSeparators.Add(pattern.Pattern);
                        _logger.Verbose("✅ **REGEX_VALIDATED**: {Pattern} - {Matches} matches", 
                            pattern.Pattern, matches.Count);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Warning("⚠️ **REGEX_INVALID**: Pattern {Pattern} - {Error}", pattern.Pattern, ex.Message);
                }
            }
            
            // **COMBINED PATTERNS**: Create combined regex for multiple separators
            if (regexSeparators.Count > 1)
            {
                var combinedPattern = string.Join("|", regexSeparators.Select(r => $"({r})"));
                regexSeparators.Add(combinedPattern);
                _logger.Verbose("🔗 **COMBINED_REGEX**: Created combined pattern with {Count} components", regexSeparators.Count - 1);
            }
            
            return regexSeparators;
        }

        /// <summary>
        /// **CONTENT TRANSITION ANALYSIS**: Analyze transitions between document content
        /// </summary>
        private async Task<List<ContentTransition>> AnalyzeContentTransitionsAsync(string text, List<DetectedDocument> documents)
        {
            _logger.Verbose("🔄 **TRANSITION_ANALYSIS**: Analyzing content transitions between documents");
            
            var transitions = new List<ContentTransition>();
            var sortedDocs = documents.OrderBy(d => d.StartPosition).ToList();
            
            for (int i = 0; i < sortedDocs.Count - 1; i++)
            {
                var transition = await AnalyzeTransitionBetweenDocuments(text, sortedDocs[i], sortedDocs[i + 1]);
                if (transition != null)
                {
                    transitions.Add(transition);
                }
            }
            
            return transitions;
        }

        /// <summary>
        /// **DOCUMENT TRANSITION ANALYSIS**: Analyze transition between two documents
        /// </summary>
        private async Task<ContentTransition> AnalyzeTransitionBetweenDocuments(string text, DetectedDocument doc1, DetectedDocument doc2)
        {
            var doc1End = doc1.StartPosition + doc1.Length;
            var doc2Start = doc2.StartPosition;
            
            if (doc2Start <= doc1End) return null; // No transition space
            
            var transitionContent = text.Substring(doc1End, doc2Start - doc1End);
            var transitionLines = transitionContent.Split('\n').Select(l => l.Trim()).Where(l => !string.IsNullOrEmpty(l)).ToList();
            
            if (!transitionLines.Any()) return null;
            
            return new ContentTransition
            {
                FromDocumentType = doc1.DocumentType,
                ToDocumentType = doc2.DocumentType,
                TransitionPosition = doc1End,
                TransitionLength = transitionContent.Length,
                TransitionContent = string.Join(" | ", transitionLines),
                TransitionType = ClassifyTransitionType(transitionLines)
            };
        }

        /// <summary>
        /// **TRANSITION TYPE CLASSIFICATION**: Classify the type of content transition
        /// </summary>
        private string ClassifyTransitionType(List<string> transitionLines)
        {
            if (transitionLines.Any(l => Regex.IsMatch(l, @"^-{3,}$"))) return "Horizontal_Line";
            if (transitionLines.Any(l => Regex.IsMatch(l, @"^Page\s+\d+", RegexOptions.IgnoreCase))) return "Page_Break";
            if (transitionLines.Any(l => l.All(c => char.IsUpper(c) || char.IsWhiteSpace(c) || char.IsPunctuation(c)))) return "Section_Header";
            if (transitionLines.Count == 1 && transitionLines[0].Length < 20) return "Short_Marker";
            if (transitionLines.Count > 3) return "Content_Block";
            return "Simple_Transition";
        }

        /// <summary>
        /// **SEPARATION QUALITY ASSESSMENT**: Assess overall quality of separation analysis
        /// </summary>
        private SeparationQuality AssessSeparationQuality(SeparationAnalysisResult result)
        {
            var quality = new SeparationQuality();
            
            // **PATTERN QUALITY**: Based on number and confidence of patterns
            var avgPatternConfidence = result.Patterns.Any() ? result.Patterns.Average(p => p.Confidence) : 0.0;
            var patternScore = Math.Min(1.0, result.Patterns.Count * 0.2 + avgPatternConfidence * 0.5);
            
            // **BOUNDARY QUALITY**: Based on boundary detection
            var avgBoundaryConfidence = result.Boundaries.Any() ? result.Boundaries.Average(b => b.Confidence) : 0.0;
            var boundaryScore = Math.Min(1.0, result.Boundaries.Count * 0.3 + avgBoundaryConfidence * 0.5);
            
            // **REGEX QUALITY**: Based on regex separator generation
            var regexScore = Math.Min(1.0, result.RegexSeparators.Count * 0.25);
            
            // **OVERALL SCORE**: Weighted combination
            quality.Score = (patternScore * 0.4 + boundaryScore * 0.4 + regexScore * 0.2);
            
            if (quality.Score >= 0.8) quality.Rating = "Excellent";
            else if (quality.Score >= 0.6) quality.Rating = "Good";
            else if (quality.Score >= 0.4) quality.Rating = "Fair";
            else quality.Rating = "Poor";
            
            return quality;
        }
    }

    /// <summary>
    /// **SEPARATION ANALYSIS RESULT**: Complete result of text separation analysis
    /// </summary>
    public class SeparationAnalysisResult
    {
        public bool HasError { get; set; }
        public string ErrorMessage { get; set; }
        public int OriginalTextLength { get; set; }
        public int DetectedDocumentCount { get; set; }
        public List<SeparationPattern> Patterns { get; set; } = new List<SeparationPattern>();
        public List<DocumentBoundary> Boundaries { get; set; } = new List<DocumentBoundary>();
        public List<string> RegexSeparators { get; set; } = new List<string>();
        public List<ContentTransition> ContentTransitions { get; set; } = new List<ContentTransition>();
        public SeparationQuality SeparationQuality { get; set; }
    }

    /// <summary>
    /// **CONTENT TRANSITION**: Information about transitions between documents
    /// </summary>
    public class ContentTransition
    {
        public string FromDocumentType { get; set; }
        public string ToDocumentType { get; set; }
        public int TransitionPosition { get; set; }
        public int TransitionLength { get; set; }
        public string TransitionContent { get; set; }
        public string TransitionType { get; set; }
    }

    /// <summary>
    /// **SEPARATION QUALITY**: Assessment of separation analysis quality
    /// </summary>
    public class SeparationQuality
    {
        public double Score { get; set; }
        public string Rating { get; set; }
        public List<string> Recommendations { get; set; } = new List<string>();
    }
}