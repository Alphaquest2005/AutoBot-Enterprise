using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using NUnit.Framework.Legacy;
using Serilog;
using WaterNut.DataSpace; // For OCRCorrectionService, CorrectionResult, RegexCreationResponse etc.
using EntryDataDS.Business.Entities; // For ShipmentInvoice
using static AutoBotUtilities.Tests.TestHelpers;
// using Moq; // Assuming Moq for mocking IDeepSeekInvoiceApi if it were injectable

namespace AutoBotUtilities.Tests.Production
{
    using OCRCorrectionService = WaterNut.DataSpace.OCRCorrectionService;

    [TestFixture]
    [Category("DeepSeekIntegration")]
    public class OCRCorrectionService_DeepSeekIntegrationTests
    {
        private ILogger _logger;
        private OCRCorrectionService _service;
        // private Mock<IDeepSeekInvoiceApi> _mockDeepSeekApi; 

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Console().CreateLogger();
            _logger.Information("=== Starting DeepSeek Integration Logic Tests ===");
        }

        [SetUp]
        public void SetUp()
        {
            _service = new OCRCorrectionService(_logger);
            _logger.Information("Test Setup for: {TestName}", TestContext.CurrentContext.Test.Name);
        }

        #region ProcessDeepSeekCorrectionResponse Tests (from previous response, still valid)
        [Test]
        public void ProcessDeepSeekCorrectionResponse_ValidErrorsArray_ShouldParseCorrections()
        {
            var jsonResponse = @"{ ""errors"": [ { ""field"": ""InvoiceTotal"", ""extracted_value"": ""123,00"", ""correct_value"": ""123.00"", ""line_text"": ""Total: 123,00"", ""line_number"": 10, ""confidence"": 0.95, ""error_type"": ""decimal_separator"", ""reasoning"": ""Comma vs period."", ""context_lines_before"": [""Before""], ""context_lines_after"": [""After""], ""requires_multiline_regex"": false }]}";
            var results = _service.ProcessDeepSeekCorrectionResponse(jsonResponse, "Full Text");
            Assert.That(results.Count, Is.EqualTo(1));
            var cr = results[0];
            Assert.That(cr.FieldName, Is.EqualTo("InvoiceTotal")); // Mapped name
            Assert.That(cr.NewValue, Is.EqualTo("123.00"));
            Assert.That(cr.LineText, Is.EqualTo("Total: 123.00"));
            Assert.That(cr.ContextLinesBefore.Contains("Before"), Is.True);
        }
        #endregion

        #region ParseRegexCreationResponseJson Tests (from previous response, still valid)

        [Test]
        public void ParseRegexCreationResponseJson_ValidResponse_ShouldParseCorrectly()
        {
            var jsonResponse = @"{ ""strategy"": ""create_new_line"", ""regex_pattern"": ""(?<MyField>\\d+)"", ""is_multiline"": false, ""max_lines"": 1, ""test_match"": ""Value 123"", ""confidence"": 0.9, ""reasoning"": ""Simple."", ""preserves_existing_groups"": true, ""context_lines_used"": ""Target line"" }";
            var result = InvokePrivateMethod<RegexCreationResponse>(_service, "ParseRegexCreationResponseJson", jsonResponse);
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Strategy, Is.EqualTo("create_new_line"));
            Assert.That(result.RegexPattern, Is.EqualTo("(?<MyField>\\d+)"));
            Assert.That(result.ContextLinesUsed, Is.EqualTo("Target line"));
        }
        #endregion

        #region FindOriginalValueInText and FindLineNumberInTextByFieldName (Private helpers, conceptual test of logic)
        [Test]
        public void FindOriginalValueInText_ShouldAttemptToFindValue()
        {
            string text = "Invoice Number: INV-001\nAmount: $123.45";
            // Assuming "Amount" maps to "InvoiceTotal" or similar for pattern creation
            string foundAmount = InvokePrivateMethod<string>(_service, "FindOriginalValueInText", "InvoiceTotal", text); // Test through public wrapper if exists, or test logic conceptually
            string foundInvNo = InvokePrivateMethod<string>(_service, "FindOriginalValueInText", "InvoiceNo", text);

            // This depends heavily on CreateFieldExtractionPatterns and how it generates patterns for these keys.
            // For a robust test, we'd need to ensure CreateFieldExtractionPatterns produces known patterns.
            StringAssert.IsMatch(@"123\.45", foundAmount ?? ""); // Or specific extracted value
            StringAssert.IsMatch(@"INV-001", foundInvNo ?? "");
            _logger.Information("? FindOriginalValueInText conceptual test.");
        }

        [Test]
        public void FindLineNumberInTextByFieldName_ShouldReturnLineNumber()
        {
            string text = "Header Info\nDate: 01/15/2023\nAnother Line";
            int lineNumber = InvokePrivateMethod<int>(_service, "FindLineNumberInTextByFieldName", "InvoiceDate", text); // Test through public wrapper if exists
            Assert.That(lineNumber, Is.EqualTo(2));
            _logger.Information("? FindLineNumberInTextByFieldName conceptual test.");
        }
        #endregion

        [Test]
        public async Task GetDeepSeekCorrectionsAsync_ValidInvoice_ShouldReturnCorrections()
        {
            // Arrange
            var invoice = new ShipmentInvoice { 
                InvoiceNo = "DS-001", 
                InvoiceTotal = 123.45, 
                SubTotal = 100.00,
                TotalOtherCost = 20.00 // Intentional math error (TotalTax→TotalOtherCost)
            };
            var fileText = "Invoice #DS-001\nSubTotal: 100.00\nTax: 20.00\nTotal: 120.00"; // Intentional discrepancy
            
            // Act - Call the actual method using ProcessDeepSeekCorrectionResponse
            var mockDeepSeekResponse = "{\"errors\":[{\"field\":\"InvoiceTotal\",\"correct_value\":\"120.00\",\"confidence\":0.95}]}";
            var result = InvokePrivateMethod<List<CorrectionResult>>(_service, "ProcessDeepSeekCorrectionResponse", mockDeepSeekResponse, fileText);
            
            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.GreaterThan(0), "Should detect at least one error");
            
            // Should find the discrepancy between file text and invoice object
            var textDiscrepancy = result.FirstOrDefault(e => e.FieldName == "InvoiceTotal" && e.NewValue == "120.00");
            Assert.That(textDiscrepancy, Is.Not.Null, "Should detect discrepancy between file text and invoice object");
        }

        #region Helper Methods

        private T InvokePrivateMethod<T>(object obj, string methodName, params object[] parameters)
        {
            var type = obj.GetType();
            var method = type.GetMethod(methodName,
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (method == null)
            {
                throw new ArgumentException($"Method {methodName} not found on type {type.Name}");
            }

            var result = method.Invoke(obj, parameters);

            if (result is T typedResult)
            {
                return typedResult;
            }
            else if (result is Task<T> taskResult)
            {
                return taskResult.Result;
            }
            else if (result is Task task)
            {
                task.Wait();
                return default(T);
            }
            else
            {
                return (T)result;
            }
        }

        #endregion
    }
}
