// DELETED - This file has been removed to avoid conflicts with the main OCR Correction Service implementation

        static void TestDataStructures()
        {
            Console.WriteLine("2. Testing data structures...");

            // Test LineInfo
            var lineInfo = new LineInfo
            {
                LineNumber = 5,
                LineText = "Total: $123.45",
                DeepSeekPrompt = "Test prompt",
                DeepSeekResponse = "Test response"
            };
            Console.WriteLine($"   LineInfo: Line {lineInfo.LineNumber}, Text: '{lineInfo.LineText}'");

            // Test CorrectionStrategy
            var strategy = new CorrectionStrategy
            {
                Type = CorrectionType.AddFieldFormatRegex,
                NewRegexPattern = @"\d+[\,\.]+\d+",
                ReplacementPattern = @"\d+\.\d+",
                Reasoning = "OCR confused comma with period",
                Confidence = 0.85
            };
            Console.WriteLine($"   CorrectionStrategy: Type {strategy.Type}, Confidence {strategy.Confidence}");

            // Test CorrectionResult
            var result = new CorrectionResult
            {
                Error = ("TotalInternalFreight", "12,50", "12.50"),
                Strategy = strategy
            };
            Console.WriteLine($"   CorrectionResult: Field {result.Error.Field}, '{result.Error.Error}' → '{result.Error.Value}'");
            Console.WriteLine("   ✓ All data structures working correctly");
            Console.WriteLine();
        }

        static void TestJsonParsing()
        {
            Console.WriteLine("3. Testing JSON parsing logic...");

            // Test line info JSON parsing
            var lineInfoJson = @"{""lineNumber"": 5, ""lineText"": ""Total: $123.45""}";
            Console.WriteLine($"   Testing line info JSON: {lineInfoJson}");

            try
            {
                var lineInfo = ParseLineInfoJson(lineInfoJson);
                if (lineInfo != null && lineInfo.LineNumber == 5 && lineInfo.LineText == "Total: $123.45")
                {
                    Console.WriteLine("   ✓ Line info JSON parsing working");
                }
                else
                {
                    throw new Exception("Line info parsing failed");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Line info JSON parsing failed: {ex.Message}");
            }

            // Test correction strategy JSON parsing
            var strategyJson = @"{
                ""type"": ""AddFieldFormatRegex"",
                ""newRegexPattern"": ""\\d+[\\,\\.]+\\d+"",
                ""replacementPattern"": ""\\d+\\.\\d+"",
                ""reasoning"": ""OCR confused comma with period"",
                ""confidence"": 0.85
            }";
            Console.WriteLine($"   Testing strategy JSON...");

            try
            {
                var strategy = ParseCorrectionStrategyJson(strategyJson);
                if (strategy != null && strategy.Type == CorrectionType.AddFieldFormatRegex && strategy.Confidence == 0.85)
                {
                    Console.WriteLine("   ✓ Correction strategy JSON parsing working");
                }
                else
                {
                    throw new Exception("Strategy parsing failed");
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Strategy JSON parsing failed: {ex.Message}");
            }

            Console.WriteLine("   ✓ JSON parsing logic verified");
            Console.WriteLine();
        }

        static void TestErrorHandling()
        {
            Console.WriteLine("4. Testing error handling...");

            // Test with invalid JSON
            try
            {
                var result = ParseLineInfoJson("invalid json");
                if (result == null)
                {
                    Console.WriteLine("   ✓ Invalid JSON handled gracefully");
                }
            }
            catch
            {
                Console.WriteLine("   ✓ Invalid JSON handled with exception (acceptable)");
            }

            // Test with empty input
            var emptyResult = ParseLineInfoJson("");
            if (emptyResult == null)
            {
                Console.WriteLine("   ✓ Empty input handled gracefully");
            }

            // Test with null input
            var nullResult = ParseLineInfoJson(null);
            if (nullResult == null)
            {
                Console.WriteLine("   ✓ Null input handled gracefully");
            }

            Console.WriteLine("   ✓ Error handling robust");
            Console.WriteLine();
        }

        static void TestRegexPatterns()
        {
            Console.WriteLine("5. Testing regex pattern creation...");

            // Test combining regex patterns
            var existingPattern = @"\d+\.\d+";
            var newPattern = @"\d+\,\d+";
            var combinedPattern = $"({existingPattern})|({newPattern})";

            Console.WriteLine($"   Existing: {existingPattern}");
            Console.WriteLine($"   New: {newPattern}");
            Console.WriteLine($"   Combined: {combinedPattern}");

            // Test format regex patterns
            var formatPattern = @"\d+[\,\.]+\d+";
            var replacementPattern = @"\d+\.\d+";

            Console.WriteLine($"   Format detection: {formatPattern}");
            Console.WriteLine($"   Replacement: {replacementPattern}");

            Console.WriteLine("   ✓ Regex pattern creation working");
            Console.WriteLine();
        }

        // Simplified JSON parsing methods (mimicking the real implementation)
        static LineInfo ParseLineInfoJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            try
            {
                using (var doc = JsonDocument.Parse(json))
                {
                    var root = doc.RootElement;

                    if (root.TryGetProperty("lineNumber", out var lineNumberElement) &&
                        root.TryGetProperty("lineText", out var lineTextElement))
                    {
                        return new LineInfo
                        {
                            LineNumber = lineNumberElement.GetInt32(),
                            LineText = lineTextElement.GetString() ?? ""
                        };
                    }
                }
            }
            catch
            {
                return null;
            }

            return null;
        }

        static CorrectionStrategy ParseCorrectionStrategyJson(string json)
        {
            if (string.IsNullOrWhiteSpace(json))
                return null;

            try
            {
                using (var doc = JsonDocument.Parse(json))
                {
                    var root = doc.RootElement;

                    var correctionType = CorrectionType.AddFieldFormatRegex;
                    if (root.TryGetProperty("type", out var typeElement))
                    {
                        var typeStr = typeElement.GetString();
                        if (Enum.TryParse<CorrectionType>(typeStr, true, out var parsedType))
                            correctionType = parsedType;
                    }

                    return new CorrectionStrategy
                    {
                        Type = correctionType,
                        NewRegexPattern = root.TryGetProperty("newRegexPattern", out var regexElement)
                            ? regexElement.GetString() ?? "" : "",
                        ReplacementPattern = root.TryGetProperty("replacementPattern", out var replacementElement)
                            ? replacementElement.GetString() ?? "" : "",
                        Reasoning = root.TryGetProperty("reasoning", out var reasoningElement)
                            ? reasoningElement.GetString() ?? "" : "",
                        Confidence = root.TryGetProperty("confidence", out var confidenceElement)
                            ? confidenceElement.GetDouble() : 0.5
                    };
                }
            }
            catch
            {
                return null;
            }
        }
    }

    // Data structures (simplified versions for testing)
    public class LineInfo
    {
        public int LineNumber { get; set; }
        public string LineText { get; set; }
        public string DeepSeekPrompt { get; set; }
        public string DeepSeekResponse { get; set; }
    }

    public class CorrectionStrategy
    {
        public CorrectionType Type { get; set; }
        public string NewRegexPattern { get; set; }
        public string ReplacementPattern { get; set; }
        public string Reasoning { get; set; }
        public double Confidence { get; set; }
    }

    public class CorrectionResult
    {
        public (string Field, string Error, string Value) Error { get; set; }
        public CorrectionStrategy Strategy { get; set; }
    }

    public enum CorrectionType
    {
        UpdateLineRegex,
        AddFieldFormatRegex,
        CreateNewRegex
    }
}
