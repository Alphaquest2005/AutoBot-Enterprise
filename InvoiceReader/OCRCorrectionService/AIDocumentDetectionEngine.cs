// File: OCRCorrectionService/AIDocumentDetectionEngine.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Threading;
using Serilog;
using Newtonsoft.Json;

namespace WaterNut.DataSpace
{
    /// <summary>
    /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v4.2**: Enhanced AI Document Detection Engine
    /// 
    /// **AI FALLBACK SYSTEM**: Detects unknown document types not found in database
    /// **SEPARATION INTELLIGENCE**: Provides document boundaries, regex patterns, separation logic
    /// **LEARNING INTEGRATION**: Generates keywords and patterns for database storage
    /// **COMPLETENESS FOCUSED**: Handles missing document content and mixed document scenarios
    /// **ENHANCED PROMPTING**: Specialized prompts for unknown type detection and separation analysis
    /// </summary>
    public class AIDocumentDetectionEngine
    {
        private readonly ILogger _logger;
        private readonly OCRLlmClient _llmClient;

        public AIDocumentDetectionEngine(ILogger logger)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _llmClient = new OCRLlmClient(_logger);
        }

        /// <summary>
        /// **🧠 ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v4.2**: AI detection for unknown document types with separation intelligence
        /// 
        /// **UNKNOWN TYPE FOCUS**: Specifically targets document types not detected by database engine
        /// **SEPARATION INTELLIGENCE**: Provides boundaries, patterns, and separation logic
        /// **LEARNING READY**: Generates keywords and metadata for database integration
        /// **COMPLETENESS DRIVEN**: Ensures missing content is properly identified and classified
        /// </summary>
        public async Task<List<DetectedDocument>> DetectUnknownDocumentTypesAsync(string text, List<DetectedDocument> knownDocuments, string missingText = null)
        {
            _logger.Information("🤖 **AI_UNKNOWN_DETECTION_START**: Detecting unknown document types with separation intelligence");
            _logger.Information("   - **INPUT_LENGTH**: {Length} chars", text?.Length ?? 0);
            _logger.Information("   - **KNOWN_DOCUMENTS**: {Count} already detected", knownDocuments?.Count ?? 0);
            _logger.Information("   - **MISSING_TEXT_LENGTH**: {Length} chars", missingText?.Length ?? 0);
            
            var detectedDocuments = new List<DetectedDocument>();
            
            try
            {
                // **ENHANCED AI PROMPT**: Focus on unknown types and separation intelligence
                var detectionPrompt = BuildEnhancedDetectionPrompt(text, knownDocuments, missingText);
                
                _logger.Information("🔄 **AI_ENHANCED_CALL**: Calling DeepSeek for unknown type detection and separation analysis");
                var aiResponse = await _llmClient.GetResponseAsync(detectionPrompt, 0.3, 2000, CancellationToken.None);
                
                _logger.Information("📊 **AI_ENHANCED_RESPONSE**: Received {Length} chars from DeepSeek", aiResponse?.Length ?? 0);
                
                // **PARSE ENHANCED RESPONSE**: Extract documents, keywords, and separation intelligence
                var parsedResult = ParseEnhancedAIResponse(aiResponse, text);
                
                detectedDocuments = parsedResult.Documents;
                
                _logger.Information("✅ **AI_UNKNOWN_DETECTION_COMPLETE**: Detected {Count} unknown document types", detectedDocuments.Count);
                
                // Log detected types
                foreach (var doc in detectedDocuments)
                {
                    _logger.Information("🎯 **AI_DETECTED_UNKNOWN**: {Type} - {Length} chars (Confidence: {Confidence:F2})", 
                        doc.DocumentType, doc.Length, doc.Confidence);
                    _logger.Information("   - **SUGGESTED_KEYWORDS**: {Keywords}", string.Join(", ", doc.MatchedKeywords));
                    _logger.Verbose("   - **AI_REASONING**: {Reasoning}", doc.AIReasoning);
                }
                
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **AI_UNKNOWN_DETECTION_ERROR**: Error in AI detection for unknown types");
                
                // **FALLBACK**: Create basic unknown document if AI fails
                if (!string.IsNullOrWhiteSpace(missingText))
                {
                    detectedDocuments.Add(new DetectedDocument
                    {
                        DocumentType = "Unknown_Document",
                        Content = missingText,
                        StartPosition = 0,
                        Length = missingText.Length,
                        Confidence = 0.5,
                        DetectionMethod = "AI_Fallback",
                        AIReasoning = "AI detection failed, classified as unknown document"
                    });
                }
            }
            
            return detectedDocuments;
        }

        /// <summary>
        /// **ENHANCED DETECTION PROMPT**: Specialized for unknown types and separation intelligence
        /// </summary>
        private string BuildEnhancedDetectionPrompt(string text, List<DetectedDocument> knownDocuments, string missingText)
        {
            var knownTypes = knownDocuments?.Select(d => d.DocumentType).ToList() ?? new List<string>();
            var knownTypesText = knownTypes.Any() ? string.Join(", ", knownTypes) : "None";
            
            var analysisText = !string.IsNullOrWhiteSpace(missingText) ? missingText : text;
            
            var prompt = $@"🎯 **ADVANCED DOCUMENT ANALYSIS REQUEST**

You are an expert document analyst. I need you to analyze text content and focus specifically on UNKNOWN document types not already detected.

**KNOWN DOCUMENT TYPES ALREADY DETECTED**: {knownTypesText}

**TEXT TO ANALYZE FOR UNKNOWN TYPES**:
{analysisText}

**YOUR MISSION**:
1. 🔍 **UNKNOWN TYPE DETECTION**: Find document types NOT in the known list above
2. 🎯 **SEPARATION INTELLIGENCE**: Provide document boundaries and separation patterns
3. 🧠 **LEARNING KEYWORDS**: Suggest keywords for future database-driven detection
4. 📏 **DOCUMENT BOUNDARIES**: Identify where each document starts and ends

**SPECIALIZED REQUIREMENTS**:
- IGNORE document types already in the known list: {knownTypesText}
- FOCUS on new/unknown document types not yet detected
- For each unknown type, provide 5-10 specific keywords for database storage
- Identify document separation patterns (line breaks, headers, footers, transitions)
- Suggest regex patterns for document boundary detection

**RETURN THIS EXACT JSON FORMAT**:
{{
  ""unknown_documents"": [
    {{
      ""document_type"": ""Specific_DocumentType_Name"",
      ""confidence"": 0.95,
      ""content_start_position"": 0,
      ""content_length"": 1234,
      ""suggested_keywords"": [""keyword1"", ""keyword2"", ""keyword3"", ""keyword4"", ""keyword5""],
      ""boundary_patterns"": [""---"", ""Page [0-9]+"", ""^[A-Z\\s]+$""],
      ""separation_logic"": ""Documents separated by horizontal lines and page headers"",
      ""ai_reasoning"": ""Detected customs declaration based on legal terminology and form structure""
    }}
  ],
  ""separation_intelligence"": {{
    ""document_boundaries"": [
      {{
        ""boundary_type"": ""horizontal_line"",
        ""pattern"": ""---+"",
        ""positions"": [123, 456, 789],
        ""confidence"": 0.9
      }}
    ],
    ""text_separation_patterns"": [
      ""^[A-Z\\s]+DECLARATION[A-Z\\s]+$"",
      ""^Page\\s+\\d+\\s+of\\s+\\d+$"",
      ""^-{{10,}}$""
    ],
    ""content_transition_markers"": [
      ""WARNING:"",
      ""CUSTOMS DECLARATION"",
      ""INVOICE SUMMARY""
    ]
  }}
}}

🚨 **CRITICAL**: Only return document types NOT already detected. Focus on the UNKNOWN and provide detailed separation intelligence for future processing.";

            return prompt;
        }

        /// <summary>
        /// **ENHANCED RESPONSE PARSER**: Parse AI response with separation intelligence
        /// </summary>
        private EnhancedAIResponse ParseEnhancedAIResponse(string aiResponse, string originalText)
        {
            var result = new EnhancedAIResponse();
            
            try
            {
                _logger.Information("📊 **PARSING_ENHANCED_AI_RESPONSE**: Extracting unknown documents and separation intelligence");
                
                // **JSON PARSING**: Extract enhanced response structure
                var responseData = JsonConvert.DeserializeObject<dynamic>(aiResponse);
                var unknownDocs = responseData?.unknown_documents;
                var separationIntel = responseData?.separation_intelligence;
                
                // **PARSE UNKNOWN DOCUMENTS**
                if (unknownDocs != null)
                {
                    foreach (var doc in unknownDocs)
                    {
                        var docType = doc.document_type?.ToString() ?? "Unknown";
                        var confidence = double.TryParse(doc.confidence?.ToString(), out double conf) ? conf : 0.5;
                        var startPos = int.TryParse(doc.content_start_position?.ToString(), out int start) ? start : 0;
                        var length = int.TryParse(doc.content_length?.ToString(), out int len) ? len : 0;
                        
                        // **EXTRACT CONTENT**: Get actual content from original text based on AI positions
                        var content = ExtractContentFromPosition(originalText, startPos, length);
                        
                        // **PARSE KEYWORDS**: Extract suggested keywords for database storage
                        var keywords = new List<string>();
                        if (doc.suggested_keywords != null)
                        {
                            foreach (var keyword in doc.suggested_keywords)
                            {
                                var keywordStr = keyword?.ToString();
                                if (!string.IsNullOrWhiteSpace(keywordStr))
                                {
                                    keywords.Add(keywordStr);
                                }
                            }
                        }
                        
                        // **PARSE BOUNDARY PATTERNS**: Extract separation patterns
                        var boundaryPatterns = new List<string>();
                        if (doc.boundary_patterns != null)
                        {
                            foreach (var pattern in doc.boundary_patterns)
                            {
                                var patternStr = pattern?.ToString();
                                if (!string.IsNullOrWhiteSpace(patternStr))
                                {
                                    boundaryPatterns.Add(patternStr);
                                }
                            }
                        }
                        
                        var detectedDoc = new DetectedDocument
                        {
                            DocumentType = docType,
                            Content = content,
                            StartPosition = startPos,
                            Length = content.Length,
                            Confidence = confidence,
                            DetectionMethod = "AI",
                            MatchedKeywords = keywords,
                            AIReasoning = doc.ai_reasoning?.ToString() ?? "AI detected unknown document type"
                        };
                        
                        result.Documents.Add(detectedDoc);
                        
                        _logger.Information("📄 **PARSED_UNKNOWN_DOCUMENT**: {Type} - Keywords: {Keywords}", 
                            docType, string.Join(", ", keywords));
                    }
                }
                
                // **PARSE SEPARATION INTELLIGENCE**
                if (separationIntel != null)
                {
                    result.SeparationIntelligence = ParseSeparationIntelligence(separationIntel);
                    _logger.Information("🔍 **SEPARATION_INTELLIGENCE_PARSED**: {Patterns} patterns, {Boundaries} boundaries", 
                        result.SeparationIntelligence.TextSeparationPatterns.Count,
                        result.SeparationIntelligence.DocumentBoundaries.Count);
                }
                
                _logger.Information("✅ **ENHANCED_PARSING_COMPLETE**: {Documents} unknown documents, separation intelligence included", 
                    result.Documents.Count);
                
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **ENHANCED_PARSING_ERROR**: Failed to parse enhanced AI response");
                
                // **FALLBACK PARSING**: Try basic JSON structure
                result = FallbackParseAIResponse(aiResponse, originalText);
            }
            
            return result;
        }

        /// <summary>
        /// **CONTENT EXTRACTION**: Extract document content from original text based on AI positions
        /// </summary>
        private string ExtractContentFromPosition(string originalText, int startPos, int length)
        {
            if (string.IsNullOrWhiteSpace(originalText) || startPos < 0 || length <= 0)
            {
                return originalText ?? "";
            }
            
            try
            {
                // **BOUNDS CHECKING**: Ensure positions are within text bounds
                var actualStart = Math.Max(0, Math.Min(startPos, originalText.Length - 1));
                var maxLength = originalText.Length - actualStart;
                var actualLength = Math.Min(length, maxLength);
                
                if (actualLength > 0)
                {
                    return originalText.Substring(actualStart, actualLength);
                }
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "⚠️ **CONTENT_EXTRACTION_ERROR**: Failed to extract content at position {Start}, length {Length}", 
                    startPos, length);
            }
            
            // **FALLBACK**: Return full text if extraction fails
            return originalText;
        }

        /// <summary>
        /// **SEPARATION INTELLIGENCE PARSER**: Parse AI separation patterns and boundaries
        /// </summary>
        private SeparationIntelligenceResult ParseSeparationIntelligence(dynamic separationIntel)
        {
            var result = new SeparationIntelligenceResult();
            
            try
            {
                // **PARSE DOCUMENT BOUNDARIES**
                if (separationIntel.document_boundaries != null)
                {
                    foreach (var boundary in separationIntel.document_boundaries)
                    {
                        var boundaryType = boundary.boundary_type?.ToString() ?? "unknown";
                        var pattern = boundary.pattern?.ToString() ?? "";
                        var confidence = double.TryParse(boundary.confidence?.ToString(), out double conf) ? conf : 0.5;
                        
                        result.DocumentBoundaries.Add(new DocumentBoundary
                        {
                            StartPosition = 0, // Will be calculated based on pattern
                            EndPosition = 0,
                            DocumentType = boundaryType,
                            Confidence = confidence,
                            BoundaryMarker = pattern
                        });
                    }
                }
                
                // **PARSE TEXT SEPARATION PATTERNS**
                if (separationIntel.text_separation_patterns != null)
                {
                    foreach (var pattern in separationIntel.text_separation_patterns)
                    {
                        var patternStr = pattern?.ToString();
                        if (!string.IsNullOrWhiteSpace(patternStr))
                        {
                            result.TextSeparationPatterns.Add(patternStr);
                        }
                    }
                }
                
                // **PARSE CONTENT TRANSITION MARKERS**
                if (separationIntel.content_transition_markers != null)
                {
                    foreach (var marker in separationIntel.content_transition_markers)
                    {
                        var markerStr = marker?.ToString();
                        if (!string.IsNullOrWhiteSpace(markerStr))
                        {
                            result.ContentTransitionMarkers.Add(markerStr);
                        }
                    }
                }
                
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **SEPARATION_INTELLIGENCE_PARSE_ERROR**: Error parsing separation intelligence");
            }
            
            return result;
        }

        /// <summary>
        /// **FALLBACK PARSER**: Basic parsing if enhanced parsing fails
        /// </summary>
        private EnhancedAIResponse FallbackParseAIResponse(string aiResponse, string originalText)
        {
            _logger.Warning("🔄 **FALLBACK_PARSING**: Using basic AI response parsing");
            
            var result = new EnhancedAIResponse();
            
            // **BASIC DOCUMENT DETECTION**: Create single unknown document
            result.Documents.Add(new DetectedDocument
            {
                DocumentType = "Unknown_Document",
                Content = originalText,
                StartPosition = 0,
                Length = originalText.Length,
                Confidence = 0.6,
                DetectionMethod = "AI_Fallback",
                AIReasoning = "Fallback parsing - could not parse detailed AI response"
            });
            
            return result;
        }
    }

    /// <summary>
    /// **ENHANCED AI RESPONSE**: Complete response with separation intelligence
    /// </summary>
    public class EnhancedAIResponse
    {
        public List<DetectedDocument> Documents { get; set; } = new List<DetectedDocument>();
        public SeparationIntelligenceResult SeparationIntelligence { get; set; } = new SeparationIntelligenceResult();
    }

    /// <summary>
    /// **SEPARATION INTELLIGENCE RESULT**: AI-provided separation patterns and boundaries
    /// </summary>
    public class SeparationIntelligenceResult
    {
        public List<DocumentBoundary> DocumentBoundaries { get; set; } = new List<DocumentBoundary>();
        public List<string> TextSeparationPatterns { get; set; } = new List<string>();
        public List<string> ContentTransitionMarkers { get; set; } = new List<string>();
    }
}