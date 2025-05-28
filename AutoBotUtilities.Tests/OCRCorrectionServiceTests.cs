using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using NUnit.Framework;
using Serilog;
using Serilog.Events;
using WaterNut.DataSpace;
using Core.Common.Extensions;

namespace AutoBotUtilities.Tests
{
    [TestFixture]
    public class OCRCorrectionServiceTests
    {
        private static Serilog.ILogger _logger;

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            // Initialize logging similar to other tests
            LogFilterState.TargetSourceContextForDetails = null;
            LogFilterState.TargetMethodNameForDetails = null;
            LogFilterState.DetailTargetMinimumLevel = LogEventLevel.Fatal;
            LogFilterState.EnabledCategoryLevels[LogCategory.Undefined] = LogEventLevel.Debug;

            try
            {
                Log.Logger = new LoggerConfiguration()
                    .MinimumLevel.Verbose()
                    .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                    .MinimumLevel.Override("System", LogEventLevel.Warning)
                    .Enrich.FromLogContext()
                    .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Verbose)
                    .CreateLogger();

                _logger = Log.ForContext<OCRCorrectionServiceTests>();
                _logger.Information("OCR Correction Service Tests - Serilog configured.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"ERROR configuring Serilog: {ex}");
                Log.Logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
                _logger = Log.ForContext<OCRCorrectionServiceTests>();
                _logger.Error(ex, "Error configuring Serilog.");
            }

            _logger.Information("--------------------------------------------------");
            _logger.Information("Starting OCR Correction Service Tests");
            _logger.Information("--------------------------------------------------");
        }

        [OneTimeTearDown]
        public void FixtureTearDown()
        {
            _logger.Information("--------------------------------------------------");
            _logger.Information("Finished OCR Correction Service Tests");
            _logger.Information("--------------------------------------------------");
            Log.CloseAndFlush();
        }

        [SetUp]
        public void SetUp()
        {
            _logger.Information("=== Starting Test: {TestName} ===", TestContext.CurrentContext.Test.Name);
        }

        [TearDown]
        public void TearDown()
        {
            _logger.Information("=== Finished Test: {TestName} ===", TestContext.CurrentContext.Test.Name);
        }

        [Test]
        public void CorrectionType_Enum_ShouldHaveCorrectValues()
        {
            _logger.Information("Testing CorrectionType enum values");

            // Test enum values exist and have correct names
            var updateLineRegex = CorrectionType.UpdateLineRegex;
            var addFieldFormat = CorrectionType.AddFieldFormatRegex;
            var createNewRegex = CorrectionType.CreateNewRegex;

            Assert.That(updateLineRegex.ToString(), Is.EqualTo("UpdateLineRegex"));
            Assert.That(addFieldFormat.ToString(), Is.EqualTo("AddFieldFormatRegex"));
            Assert.That(createNewRegex.ToString(), Is.EqualTo("CreateNewRegex"));

            _logger.Information("✓ CorrectionType enum values verified: {Values}", 
                string.Join(", ", Enum.GetNames(typeof(CorrectionType))));
        }

        [Test]
        public void LineInfo_DataStructure_ShouldWorkCorrectly()
        {
            _logger.Information("Testing LineInfo data structure");

            var lineInfo = new LineInfo
            {
                LineNumber = 5,
                LineText = "Total: $123.45",
                DeepSeekPrompt = "Test prompt for line detection",
                DeepSeekResponse = "Test response from DeepSeek"
            };

            Assert.That(lineInfo.LineNumber, Is.EqualTo(5));
            Assert.That(lineInfo.LineText, Is.EqualTo("Total: $123.45"));
            Assert.That(lineInfo.DeepSeekPrompt, Is.EqualTo("Test prompt for line detection"));
            Assert.That(lineInfo.DeepSeekResponse, Is.EqualTo("Test response from DeepSeek"));

            _logger.Information("✓ LineInfo structure verified: Line {LineNumber}, Text: '{LineText}'", 
                lineInfo.LineNumber, lineInfo.LineText);
        }

        [Test]
        public void CorrectionStrategy_DataStructure_ShouldWorkCorrectly()
        {
            _logger.Information("Testing CorrectionStrategy data structure");

            var strategy = new CorrectionStrategy
            {
                Type = CorrectionType.AddFieldFormatRegex,
                NewRegexPattern = @"\d+[\,\.]+\d+",
                ReplacementPattern = @"\d+\.\d+",
                Reasoning = "OCR confused comma with period in decimal number",
                Confidence = 0.85
            };

            Assert.That(strategy.Type, Is.EqualTo(CorrectionType.AddFieldFormatRegex));
            Assert.That(strategy.NewRegexPattern, Is.EqualTo(@"\d+[\,\.]+\d+"));
            Assert.That(strategy.ReplacementPattern, Is.EqualTo(@"\d+\.\d+"));
            Assert.That(strategy.Reasoning, Is.EqualTo("OCR confused comma with period in decimal number"));
            Assert.That(strategy.Confidence, Is.EqualTo(0.85));

            _logger.Information("✓ CorrectionStrategy structure verified: Type {Type}, Confidence {Confidence}", 
                strategy.Type, strategy.Confidence);
        }

        [Test]
        public void OCRCorrection_DataStructure_ShouldWorkCorrectly()
        {
            _logger.Information("Testing OCRCorrection data structure");

            var lineInfo = new LineInfo { LineNumber = 3, LineText = "Freight: 12,50" };
            var strategy = new CorrectionStrategy 
            { 
                Type = CorrectionType.AddFieldFormatRegex,
                Confidence = 0.9 
            };

            var correction = new OCRCorrection
            {
                Error = ("TotalInternalFreight", "12,50", "12.50"),
                LineInfo = lineInfo,
                Strategy = strategy,
                WindowLines = new[] { "Line 1", "Line 2", "Freight: 12,50", "Line 4", "Line 5" }
            };

            Assert.That(correction.Error.Field, Is.EqualTo("TotalInternalFreight"));
            Assert.That(correction.Error.Error, Is.EqualTo("12,50"));
            Assert.That(correction.Error.Value, Is.EqualTo("12.50"));
            Assert.That(correction.LineInfo, Is.EqualTo(lineInfo));
            Assert.That(correction.Strategy, Is.EqualTo(strategy));
            Assert.That(correction.WindowLines.Length, Is.EqualTo(5));

            _logger.Information("✓ OCRCorrection structure verified: Field {Field}, Error '{Error}' → '{Value}'", 
                correction.Error.Field, correction.Error.Error, correction.Error.Value);
        }

        [Test]
        public void OCRCorrectionService_Instantiation_ShouldSucceed()
        {
            _logger.Information("Testing OCRCorrectionService instantiation");

            OCRCorrectionService service = null;
            Assert.DoesNotThrow(() => 
            {
                service = new OCRCorrectionService();
            }, "OCRCorrectionService should instantiate without errors");

            Assert.That(service, Is.Not.Null, "OCRCorrectionService instance should not be null");

            _logger.Information("✓ OCRCorrectionService instantiated successfully");
        }

        [Test]
        public void JsonParsing_LineInfo_ShouldWorkCorrectly()
        {
            _logger.Information("Testing JSON parsing for LineInfo");

            var testJson = @"{""lineNumber"": 7, ""lineText"": ""Total Amount: $456.78""}";
            _logger.Debug("Testing with JSON: {Json}", testJson);

            LineInfo result = null;
            Assert.DoesNotThrow(() =>
            {
                result = ParseLineInfoJson(testJson);
            }, "JSON parsing should not throw exceptions");

            Assert.That(result, Is.Not.Null, "Parsed LineInfo should not be null");
            Assert.That(result.LineNumber, Is.EqualTo(7));
            Assert.That(result.LineText, Is.EqualTo("Total Amount: $456.78"));

            _logger.Information("✓ JSON parsing verified: Line {LineNumber}, Text: '{LineText}'", 
                result.LineNumber, result.LineText);
        }

        [Test]
        public void JsonParsing_CorrectionStrategy_ShouldWorkCorrectly()
        {
            _logger.Information("Testing JSON parsing for CorrectionStrategy");

            var testJson = @"{
                ""type"": ""CreateNewRegex"",
                ""newRegexPattern"": ""\\d+[\\,\\.]+\\d+"",
                ""replacementPattern"": ""\\d+\\.\\d+"",
                ""reasoning"": ""Test reasoning for regex creation"",
                ""confidence"": 0.75
            }";
            _logger.Debug("Testing with JSON: {Json}", testJson.Replace("\n", "").Replace("  ", ""));

            CorrectionStrategy result = null;
            Assert.DoesNotThrow(() =>
            {
                result = ParseCorrectionStrategyJson(testJson);
            }, "JSON parsing should not throw exceptions");

            Assert.That(result, Is.Not.Null, "Parsed CorrectionStrategy should not be null");
            Assert.That(result.Type, Is.EqualTo(CorrectionType.CreateNewRegex));
            Assert.That(result.NewRegexPattern, Is.EqualTo(@"\d+[\,\.]+\d+"));
            Assert.That(result.ReplacementPattern, Is.EqualTo(@"\d+\.\d+"));
            Assert.That(result.Reasoning, Is.EqualTo("Test reasoning for regex creation"));
            Assert.That(result.Confidence, Is.EqualTo(0.75));

            _logger.Information("✓ JSON parsing verified: Type {Type}, Confidence {Confidence}", 
                result.Type, result.Confidence);
        }

        [Test]
        public void JsonParsing_InvalidInput_ShouldHandleGracefully()
        {
            _logger.Information("Testing JSON parsing error handling");

            // Test with invalid JSON
            var invalidJson = "{ invalid json }";
            var result1 = ParseLineInfoJson(invalidJson);
            Assert.That(result1, Is.Null, "Invalid JSON should return null");

            // Test with empty input
            var result2 = ParseLineInfoJson("");
            Assert.That(result2, Is.Null, "Empty input should return null");

            // Test with null input
            var result3 = ParseLineInfoJson(null);
            Assert.That(result3, Is.Null, "Null input should return null");

            _logger.Information("✓ Error handling verified for invalid inputs");
        }

        // Helper methods for JSON parsing (simplified versions for testing)
        private static LineInfo ParseLineInfoJson(string json)
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

        private static CorrectionStrategy ParseCorrectionStrategyJson(string json)
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
}
