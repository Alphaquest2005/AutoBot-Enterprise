using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using Serilog;
using WaterNut.DataSpace; // For OCRCorrectionService, CorrectionResult, LineContext, RegexCreationResponse
using static AutoBotUtilities.Tests.TestHelpers;

namespace AutoBotUtilities.Tests.Production
{
    using OCRCorrectionService = WaterNut.DataSpace.OCRCorrectionService;

    [TestFixture]
    [Category("PatternCreation")]
    public class OCRCorrectionService_PatternCreationTests
    {
        private ILogger _logger;
        private OCRCorrectionService _service;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Console().CreateLogger();
            _logger.Information("=== Starting Pattern Creation Tests ===");
        }

        [SetUp]
        public void SetUp()
        {
            _service = new OCRCorrectionService(_logger);
            _logger.Information("Test Setup: {TestName}", TestContext.CurrentContext.Test.Name);
        }

        #region CreateAdvancedFormatCorrectionPatterns Tests (already covered in FieldFormatPatternTests, but can add more specific cases if needed)
        // These are well-covered by OCRCorrectionService.FieldFormatPatternTests.cs
        // Add any specific edge cases for CreateAdvancedFormatCorrectionPatterns here if necessary.
        #endregion

        #region CreateLocalOmissionExtractionPattern Tests

        private CorrectionResult CreateOmissionCorrection(string fieldName, string newValue, string lineText, int lineNumber = 1, bool multiline = false)
        {
            return new CorrectionResult
            {
                FieldName = fieldName,
                NewValue = newValue,
                OldValue = "",
                CorrectionType = "omission",
                LineText = lineText,
                LineNumber = lineNumber,
                RequiresMultilineRegex = multiline,
                ContextLinesBefore = new List<string> { "Before: " + lineText },
                ContextLinesAfter = new List<string> { "After: " + lineText }
            };
        }
        private LineContext CreateLineCtx(CorrectionResult cr, string existingRegex = null)
        {
            return new LineContext
            {
                LineNumber = cr.LineNumber,
                LineText = cr.LineText,
                RegexPattern = existingRegex,
                ContextLinesBefore = cr.ContextLinesBefore,
                ContextLinesAfter = cr.ContextLinesAfter,
                RequiresMultilineRegex = cr.RequiresMultilineRegex
            };
        }

        [Test]
        public void CreateLocalOmissionExtractionPattern_SimpleNumeric_ShouldCreateReasonablePattern()
        {
            var correction = CreateOmissionCorrection("InvoiceTotal", "123.45", "Invoice Total is 123.45 USD");
            var lineContext = CreateLineCtx(correction);

            var response = InvokePrivateMethod<RegexCreationResponse>(_service, "CreateLocalOmissionExtractionPattern", correction, lineContext);

            Assert.That(response, Is.Not.Null);
            Assert.That(response.Strategy, Is.EqualTo("create_new_line")); // Default without existing complex regex
            StringAssert.Contains(@"(?<InvoiceTotal>-?\$?�?�?\s*(?:[0-9]{1,3}(?:[,.]\d{3})*|[0-9]+)(?:[.,]\d{1,4})?)", response.RegexPattern);
            StringAssert.Contains(System.Text.RegularExpressions.Regex.Escape("Invoice Total is "), response.RegexPattern, "Should include prefix from line text.");
            StringAssert.Contains(System.Text.RegularExpressions.Regex.Escape(" USD"), response.RegexPattern, "Should include suffix from line text.");
            Assert.That(response.TestMatch, Is.EqualTo("Invoice Total is 123.45 USD"));
            _logger.Information("? CreateLocalOmissionExtractionPattern for simple numeric created: {Pattern}", response.RegexPattern);
        }

        [Test]
        public void CreateLocalOmissionExtractionPattern_StringValue_ShouldCreatePattern()
        {
            var correction = CreateOmissionCorrection("SupplierName", "ACME Corp", "Supplier: ACME Corp, Location TX");
            var lineContext = CreateLineCtx(correction);

            var response = InvokePrivateMethod<RegexCreationResponse>(_service, "CreateLocalOmissionExtractionPattern", correction, lineContext);

            Assert.That(response, Is.Not.Null);
            StringAssert.Contains(@"(?<SupplierName>" + System.Text.RegularExpressions.Regex.Escape("ACME Corp") + ")", response.RegexPattern);
            StringAssert.Contains(System.Text.RegularExpressions.Regex.Escape("Supplier: "), response.RegexPattern);
            _logger.Information("? CreateLocalOmissionExtractionPattern for string value created: {Pattern}", response.RegexPattern);
        }

        [Test]
        public void CreateLocalOmissionExtractionPattern_WithExistingLineRegex_MaySuggestModify()
        {
            var correction = CreateOmissionCorrection("TaxAmount", "10.00", "Subtotal: 100.00 Tax: 10.00 Total: 110.00");
            var lineContext = CreateLineCtx(correction, existingRegex: @"Subtotal:\s*(?<SubTotal>\d+\.\d{2}).*Total:\s*(?<InvoiceTotal>\d+\.\d{2})");
            lineContext.FieldsInLine = new List<FieldInfo> { // Simulate fields already in regex
                new FieldInfo { Key = "SubTotal" }, new FieldInfo { Key = "InvoiceTotal" }
            };

            var response = InvokePrivateMethod<RegexCreationResponse>(_service, "CreateLocalOmissionExtractionPattern", correction, lineContext);

            Assert.That(response, Is.Not.Null);
            // If existing regex is simple enough, it might try to modify
            Assert.That(response.Strategy, Is.EqualTo("modify_existing_line").Or.EqualTo("create_new_line"));
            if (response.Strategy == "modify_existing_line")
            {
                StringAssert.Contains("(?<TaxAmount>", response.CompleteLineRegex);
                StringAssert.Contains("(?<SubTotal>", response.CompleteLineRegex); // Preserved
            }
            else // create_new_line
            {
                StringAssert.Contains("(?<TaxAmount>", response.RegexPattern);
            }
            _logger.Information("? CreateLocalOmissionExtractionPattern with existing regex: Strategy={Strategy}, Pattern='{FullPattern}'", response.Strategy, response.CompleteLineRegex ?? response.RegexPattern);
        }


        #endregion

        #region ValidateRegexPattern Tests
        [Test]
        public void ValidateRegexPattern_ValidPatternAndMatch_ShouldReturnTrue()
        {
            var regexResponse = new RegexCreationResponse
            {
                Strategy = "create_new_line",
                RegexPattern = @"Amount:\s*(?<TestValue>\d+)",
                IsMultiline = false,
                TestMatch = "Amount: 123"
            };
            var correction = new CorrectionResult { FieldName = "TestValue", NewValue = "123", LineText = "Amount: 123" };

            Assert.That(InvokePrivateMethod<bool>(_service, "ValidateRegexPattern", regexResponse, correction), Is.True);
            _logger.Information("? ValidateRegexPattern for valid pattern and match passed.");
        }

        [Test]
        public void ValidateRegexPattern_PatternMismatch_ShouldReturnFalse()
        {
            var regexResponse = new RegexCreationResponse
            {
                RegexPattern = @"Value:\s*(?<TestValue>\d+)",
                TestMatch = "Amount: 123" // Pattern looks for "Value:", text has "Amount:"
            };
            var correction = new CorrectionResult { FieldName = "TestValue", NewValue = "123", LineText = "Amount: 123" };

            Assert.That(InvokePrivateMethod<bool>(_service, "ValidateRegexPattern", regexResponse, correction), Is.False);
            _logger.Information("? ValidateRegexPattern for pattern mismatch passed.");
        }

        [Test]
        public void ValidateRegexPattern_ValueMismatch_ShouldReturnFalse()
        {
            var regexResponse = new RegexCreationResponse
            {
                RegexPattern = @"Amount:\s*(?<TestValue>\d+)",
                TestMatch = "Amount: 123"
            };
            var correction = new CorrectionResult { FieldName = "TestValue", NewValue = "456", LineText = "Amount: 123" }; // Expected NewValue different

            Assert.That(InvokePrivateMethod<bool>(_service, "ValidateRegexPattern", regexResponse, correction), Is.False);
            _logger.Information("? ValidateRegexPattern for value mismatch passed.");
        }

        [Test]
        public void ValidateRegexPattern_InvalidSyntax_ShouldReturnFalse()
        {
            var regexResponse = new RegexCreationResponse { RegexPattern = @"(?<TestValue>\d+" }; // Missing closing parenthesis
            var correction = new CorrectionResult { FieldName = "TestValue", NewValue = "123" };

            Assert.That(InvokePrivateMethod<bool>(_service, "ValidateRegexPattern", regexResponse, correction), Is.False);
            _logger.Information("? ValidateRegexPattern for invalid syntax passed.");
        }

        [Test]
        public void ValidateRegexPattern_SyntaxOnly_ShouldPassIfValid()
        {
            Assert.That(InvokePrivateMethod<bool>(_service, "ValidateRegexPattern", @"(?<TestName>[A-Za-z]+)"), Is.True);
            Assert.That(InvokePrivateMethod<bool>(_service, "ValidateRegexPattern", @"(?<TestName>[A-Za-z]+"), Is.False); // Invalid
            _logger.Information("? ValidateRegexPattern (syntax only) passed.");
        }

        [Test]
        public void ValidateRegexPattern_ValidPattern_ShouldReturnTrue()
        {
            // Arrange
            var correction = new CorrectionResult { FieldName = "InvoiceTotal", NewValue = "123.45" };
            var regexResponse = new RegexCreationResponse {
                RegexPattern = @"Total:\s*(\d+\.\d+)",
                Strategy = "new_line"
            };
            var fileText = "Invoice Details\nTotal: 123.45\nDate: 2023-01-01";
            
            // Act - Call the private method using reflection
            bool result = InvokePrivateMethod<bool>(_service, "ValidateRegexPattern", correction, regexResponse, fileText);
            
            // Assert
            Assert.That(result, Is.True, "Pattern should be validated successfully");
        }

        [Test]
        public void ValidateRegexPattern_InvalidPattern_ShouldReturnFalse()
        {
            // Arrange
            var correction = new CorrectionResult { FieldName = "InvoiceTotal", NewValue = "123.45" };
            var regexResponse = new RegexCreationResponse {
                RegexPattern = @"Amount:\s*(\d+\.\d+)", // This pattern won't match "Total: 123.45"
                Strategy = "new_line"
            };
            var fileText = "Invoice Details\nTotal: 123.45\nDate: 2023-01-01";
            
            // Act - Call the private method using reflection
            bool result = InvokePrivateMethod<bool>(_service, "ValidateRegexPattern", correction, regexResponse, fileText);
            
            // Assert
            Assert.That(result, Is.False, "Pattern should fail validation");
        }

        #endregion

        #region CreateFieldExtractionPatterns Tests
        [Test]
        public void CreateFieldExtractionPatterns_MonetaryField_ShouldGenerateMonetaryPatterns()
        {
            var patterns = InvokePrivateMethod<List<string>>(_service, "CreateFieldExtractionPatterns", "InvoiceTotal", new[] { "123.45", "$50.00" });
            Assert.That(patterns, Is.Not.Empty);
            Assert.That(patterns.Any(p => p.Contains(System.Text.RegularExpressions.Regex.Escape("InvoiceTotal")) && p.Contains(@"\$?�?�?")));
            Assert.That(patterns.Any(p => p.Contains(System.Text.RegularExpressions.Regex.Escape("123.45"))));
            _logger.Information("? CreateFieldExtractionPatterns for monetary field generated appropriate patterns.");
        }

        [Test]
        public void CreateFieldExtractionPatterns_DateField_ShouldGenerateDatePatterns()
        {
            var patterns = InvokePrivateMethod<List<string>>(_service, "CreateFieldExtractionPatterns", "InvoiceDate", new[] { "01/15/2023", "Jan 20, 2024" });
            Assert.That(patterns, Is.Not.Empty);
            Assert.That(patterns.Any(p => p.Contains(System.Text.RegularExpressions.Regex.Escape("InvoiceDate")) && p.Contains(@"\d{1,2}[/\-.]\d{1,2}[/\-.]\d{2,4}")));
            Assert.That(patterns.Any(p => p.Contains(System.Text.RegularExpressions.Regex.Escape("01/15/2023"))));
            _logger.Information("? CreateFieldExtractionPatterns for date field generated appropriate patterns.");
        }
        #endregion
    }
}
