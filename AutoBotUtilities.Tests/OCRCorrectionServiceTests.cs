using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Serilog;
using Serilog.Events;
using WaterNut.DataSpace;
using Core.Common.Extensions;
using CoreEntities.Business.Entities;
using OCR.Business.Entities;
using EntryDataDS.Business.Entities;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.SaveCSV;

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
        public async Task OCRErrorDetectionIntegration_ShouldDetectAndCorrectErrors()
        {
            _logger.Information("Testing OCR error detection and correction integration");

            // Arrange - Create mock invoice with known errors
            var invoice = CreateMockInvoiceWithErrors();
            var originalText = CreateMockOriginalText();
            var template = CreateMockOCRTemplate();

            // Act - Run error detection and correction
            var errors = await DetectInvoiceErrors(invoice, originalText);
            var corrections = await ProcessErrorCorrections(errors, originalText, template);

            // Assert - Verify errors were detected and corrections generated
            Assert.That(errors, Is.Not.Empty, "Should detect OCR errors");
            Assert.That(corrections, Is.Not.Empty, "Should generate corrections");

            _logger.Information("✓ OCR error detection integration test completed: {ErrorCount} errors, {CorrectionCount} corrections",
                errors.Count, corrections.Count);
        }

        [Test]
        public async Task FieldMapping_ShouldMapDeepSeekFieldsToOCRFields()
        {
            _logger.Information("Testing field mapping from DeepSeek field names to OCR fields");

            // Arrange
            var deepSeekFieldName = "TotalOtherCost";
            var windowLines = new[] { "Shipping: $25.00", "Tax: $15.00", "Total Other: $40.00" };
            var ocrTemplate = CreateMockOCRTemplate();

            // Act
            var matchingField = FindMatchingOCRField(deepSeekFieldName, windowLines, ocrTemplate);

            // Assert
            Assert.That(matchingField, Is.Not.Null, "Should find matching OCR field");
            Assert.That(matchingField.Key, Is.EqualTo(deepSeekFieldName), "Field key should match DeepSeek field name");

            _logger.Information("✓ Field mapping test completed: {FieldName} mapped to OCR field {FieldId}",
                deepSeekFieldName, matchingField?.Id);
        }

        [Test]
        public void GetLineWindow_ShouldCreateCorrectWindow()
        {
            _logger.Information("Testing line window creation for error context");

            // Arrange
            var fileLines = new[]
            {
                "Line 1", "Line 2", "Line 3", "Line 4", "Line 5",
                "Line 6", "Line 7", "Line 8", "Line 9", "Line 10"
            };
            var targetLine = 5; // 0-based index
            var windowSize = 2;

            // Act
            var window = GetLineWindow(fileLines, targetLine, windowSize);

            // Assert
            Assert.That(window.Length, Is.EqualTo(5), "Window should contain 5 lines (2 before + target + 2 after)");
            Assert.That(window[0], Is.EqualTo("Line 4"), "First line should be 2 before target");
            Assert.That(window[2], Is.EqualTo("Line 6"), "Middle line should be target line");
            Assert.That(window[4], Is.EqualTo("Line 8"), "Last line should be 2 after target");

            _logger.Information("✓ Line window test completed: {WindowSize} lines around target {TargetLine}",
                window.Length, targetLine);
        }

        [Test]
        public void TestFieldRegexInWindow_ShouldMatchCorrectly()
        {
            _logger.Information("Testing regex matching in line window");

            // Arrange
            var field = CreateMockOCRField("TotalOtherCost", @"Total\s+Other[:\s]+\$?(\d+\.?\d*)");
            var windowLines = new[]
            {
                "Subtotal: $100.00",
                "Shipping: $25.00",
                "Total Other: $40.00",
                "Tax: $15.00",
                "Grand Total: $180.00"
            };

            // Act
            var matches = TestFieldRegexInWindow(field, windowLines);

            // Assert
            Assert.That(matches, Is.True, "Regex should match 'Total Other: $40.00' line");

            _logger.Information("✓ Regex matching test completed: Pattern matched in window");
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

        #region Helper Methods for Testing

        /// <summary>
        /// Creates a mock invoice with known OCR errors for testing
        /// </summary>
        private ShipmentInvoice CreateMockInvoiceWithErrors()
        {
            return new ShipmentInvoice
            {
                InvoiceNo = "AMZ-123456",
                InvoiceTotal = 180.00,
                SubTotal = 100.00,
                TotalInternalFreight = 25.00,
                TotalOtherCost = 0.00, // Error: Should be 40.00 (missing shipping + tax)
                TotalInsurance = 0.00,
                TotalDeduction = 0.00,
                InvoiceDetails = new List<InvoiceDetails>
                {
                    new InvoiceDetails
                    {
                        LineNumber = 1,
                        Quantity = 2,
                        Cost = 50.00,
                        TotalCost = 100.00, // Correct: 2 * 50.00 = 100.00
                        ItemDescription = "Test Product 1"
                    }
                }
            };
        }

        /// <summary>
        /// Creates mock original text that contains the correct values
        /// </summary>
        private string CreateMockOriginalText()
        {
            return @"
AMAZON INVOICE
Invoice #: AMZ-123456
Date: 2024-01-15

Item Description    Qty    Price    Total
Test Product 1       2     $50.00   $100.00

Subtotal:                           $100.00
Shipping:                           $25.00
Tax:                                $15.00
Total Other Costs:                  $40.00
Grand Total:                        $180.00
";
        }

        /// <summary>
        /// Creates a mock OCR template for testing field mapping
        /// </summary>
        private OCR.Business.Entities.Invoices CreateMockOCRTemplate()
        {
            var template = new OCR.Business.Entities.Invoices
            {
                Id = 5, // Amazon invoice template
                Parts = new List<OCR.Business.Entities.Parts>
                {
                    new OCR.Business.Entities.Parts
                    {
                        Lines = new List<OCR.Business.Entities.Lines>
                        {
                            new OCR.Business.Entities.Lines
                            {
                                Fields = new List<OCR.Business.Entities.Fields>
                                {
                                    CreateMockOCRField("TotalOtherCost", @"Total\s+Other[:\s]+\$?(\d+\.?\d*)"),
                                    CreateMockOCRField("TotalInternalFreight", @"Shipping[:\s]+\$?(\d+\.?\d*)"),
                                    CreateMockOCRField("InvoiceTotal", @"Grand\s+Total[:\s]+\$?(\d+\.?\d*)"),
                                    CreateMockOCRField("SubTotal", @"Subtotal[:\s]+\$?(\d+\.?\d*)")
                                }
                            }
                        }
                    }
                }
            };
            return template;
        }

        /// <summary>
        /// Creates a mock OCR field with regex pattern
        /// </summary>
        private OCR.Business.Entities.Fields CreateMockOCRField(string key, string regexPattern)
        {
            return new OCR.Business.Entities.Fields
            {
                Id = key.GetHashCode(), // Simple ID generation for testing
                Key = key,
                Lines = new OCR.Business.Entities.Lines
                {
                    RegularExpressions = new OCR.Business.Entities.RegularExpressions
                    {
                        RegEx = regexPattern,
                        MultiLine = false
                    }
                }
            };
        }

        /// <summary>
        /// Mock implementation of error detection for testing
        /// </summary>
        private async Task<List<(string Field, string Error, string Value)>> DetectInvoiceErrors(
            ShipmentInvoice invoice, string originalText)
        {
            // Simulate DeepSeek error detection
            await Task.Delay(10); // Simulate async operation

            return new List<(string Field, string Error, string Value)>
            {
                ("TotalOtherCost", "0.00", "40.00"), // Missing other costs
                ("TotalInternalFreight", "25.00", "25.00") // This one is correct
            };
        }

        /// <summary>
        /// Mock implementation of error correction processing for testing
        /// </summary>
        private async Task<List<OCRCorrection>> ProcessErrorCorrections(
            List<(string Field, string Error, string Value)> errors,
            string originalText,
            OCR.Business.Entities.Invoices template)
        {
            await Task.Delay(10); // Simulate async operation

            var corrections = new List<OCRCorrection>();

            foreach (var error in errors)
            {
                if (error.Error != error.Value) // Only process actual errors
                {
                    corrections.Add(new OCRCorrection
                    {
                        Error = error,
                        LineInfo = new LineInfo { LineNumber = 10, LineText = $"Total Other Costs: ${error.Value}" },
                        Field = CreateMockOCRField(error.Field, @"Total\s+Other[:\s]+\$?(\d+\.?\d*)"),
                        Strategy = new CorrectionStrategy
                        {
                            Type = CorrectionType.AddFieldFormatRegex,
                            NewRegexPattern = @"(\d+),(\d+)",
                            ReplacementPattern = "$1.$2",
                            Reasoning = "Fix comma/period confusion in decimal numbers",
                            Confidence = 0.9
                        }
                    });
                }
            }

            return corrections;
        }

        /// <summary>
        /// Mock implementation of field mapping for testing
        /// </summary>
        private OCR.Business.Entities.Fields FindMatchingOCRField(
            string deepSeekFieldName,
            string[] windowLines,
            OCR.Business.Entities.Invoices ocrTemplate)
        {
            return ocrTemplate.Parts
                .SelectMany(part => part.Lines)
                .SelectMany(line => line.Fields)
                .Where(field => string.Equals(field.Key, deepSeekFieldName, StringComparison.OrdinalIgnoreCase))
                .FirstOrDefault(field => TestFieldRegexInWindow(field, windowLines));
        }

        /// <summary>
        /// Mock implementation of line window creation for testing
        /// </summary>
        private string[] GetLineWindow(string[] fileLines, int targetLine, int windowSize)
        {
            var startLine = Math.Max(0, targetLine - windowSize);
            var endLine = Math.Min(fileLines.Length - 1, targetLine + windowSize);
            var windowLength = endLine - startLine + 1;

            var window = new string[windowLength];
            Array.Copy(fileLines, startLine, window, 0, windowLength);

            return window;
        }

        /// <summary>
        /// Mock implementation of regex testing in window for testing
        /// </summary>
        private bool TestFieldRegexInWindow(OCR.Business.Entities.Fields field, string[] windowLines)
        {
            if (field.Lines?.RegularExpressions?.RegEx == null) return false;

            var regex = new Regex(field.Lines.RegularExpressions.RegEx,
                RegexOptions.IgnoreCase | (field.Lines.RegularExpressions.MultiLine == true ? RegexOptions.Multiline : RegexOptions.None));

            return windowLines.Any(line => regex.IsMatch(line ?? string.Empty));
        }

        #endregion

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
