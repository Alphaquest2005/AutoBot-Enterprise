using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Serilog;
using WaterNut.DataSpace;
using EntryDataDS.Business.Entities;

namespace AutoBotUtilities.Tests
{
    /// <summary>
    /// Simplified OCR correction pipeline tests that focus on core functionality
    /// without requiring full database setup.
    /// </summary>
    [TestFixture]
    public class OCRCorrectionServiceSimplePipelineTests
    {
        private ILogger _logger;
        private WaterNut.DataSpace.OCRCorrectionService _service;

        [SetUp]
        public void Setup()
        {
            _logger = new LoggerConfiguration()
                .MinimumLevel.Error()
                .WriteTo.Console()
                .CreateLogger();
            _service = new WaterNut.DataSpace.OCRCorrectionService(_logger);
            
            _logger.Information("🔍 **TEST_SETUP**: Simple pipeline test initialized");
        }

        [TearDown]
        public void TearDown()
        {
            _service?.Dispose();
        }

        /// <summary>
        /// Test DeepSeek integration for regex generation without database operations.
        /// This validates the core API integration and response parsing.
        /// </summary>
        [Test]
        public async Task DeepSeekIntegration_ShouldGenerateRegexPattern_ForGiftCardOmission()
        {
            // Arrange
            var correction = new CorrectionResult
            {
                FieldName = "TotalInsurance", // Caribbean customs rule: customer reductions → TotalInsurance
                OldValue = null, // Field was missing (omission)
                NewValue = "-6.99", // Gift Card Amount: -$6.99
                CorrectionType = "omission",
                Success = true,
                Confidence = 0.95,
                Reasoning = "Gift Card Amount represents customer-caused reduction",
                LineNumber = 15,
                LineText = "Gift Card Amount: -$6.99",
                ContextLinesBefore = new List<string> { "Order Total: $166.30", "" },
                ContextLinesAfter = new List<string> { "", "Shipped on April 17, 2025" },
                RequiresMultilineRegex = false
            };

            var lineContext = new LineContext
            {
                LineNumber = correction.LineNumber,
                LineText = correction.LineText,
                ContextLinesBefore = correction.ContextLinesBefore,
                ContextLinesAfter = correction.ContextLinesAfter,
                RequiresMultilineRegex = false,
                WindowText = "Order Total: $166.30\n\nGift Card Amount: -$6.99\n\nShipped on April 17, 2025",
                IsOrphaned = true,
                RequiresNewLineCreation = true
            };

            _logger.Information("🔍 **TEST_ARRANGE**: Testing DeepSeek regex generation for {FieldName}", correction.FieldName);

            // Act
            var result = await _service.GenerateRegexPatternInternal(correction, lineContext);

            // Assert
            Assert.That(result, Is.Not.Null, "GenerateRegexPatternInternal should return a result");
            Assert.That(result.Success, Is.True, "Regex generation should succeed");
            Assert.That(result.SuggestedRegex, Is.Not.Null.And.Not.Empty, "Should generate a regex pattern");
            
            // Log results for verification
            _logger.Information("✅ **TEST_SUCCESS**: DeepSeek integration working - FieldName: {FieldName}, Pattern: {Pattern}", 
                result.FieldName, result.SuggestedRegex);
            
            // Basic pattern validation - should contain some pattern elements
            Assert.That(result.SuggestedRegex, Does.Contain("?<"), "Regex should contain named capture group");
            
            _logger.Information("🔍 **TEST_COMPLETE**: DeepSeek integration test completed successfully");
        }

        /// <summary>
        /// Test pattern validation logic without database dependencies.
        /// This validates the regex pattern validation step.
        /// </summary>
        [Test]
        public void PatternValidation_ShouldValidateBasicRegexPattern_ForSupportedField()
        {
            // Arrange
            var correction = new CorrectionResult
            {
                FieldName = "TotalInsurance",
                CorrectionType = "omission",
                SuggestedRegex = @"Gift Card Amount:\s*(?<TotalInsurance>-?\$?\d+\.?\d*)",
                Success = true,
                Confidence = 0.9
            };

            _logger.Information("🔍 **TEST_ARRANGE**: Testing pattern validation for {FieldName}", correction.FieldName);

            // Act
            var result = _service.ValidatePatternInternal(correction);

            // Assert
            Assert.That(result, Is.Not.Null, "ValidatePatternInternal should return a result");
            Assert.That(result.Success, Is.True, "Pattern validation should succeed for valid regex");
            
            _logger.Information("✅ **TEST_SUCCESS**: Pattern validation working for field {FieldName}", result.FieldName);
            _logger.Information("🔍 **TEST_COMPLETE**: Pattern validation test completed successfully");
        }

        /// <summary>
        /// Test field support validation to ensure we're only processing supported fields.
        /// </summary>
        [Test]
        public void FieldSupportValidation_ShouldAcceptSupportedFields_AndRejectUnsupported()
        {
            _logger.Information("🔍 **TEST_ARRANGE**: Testing field support validation");

            // Act & Assert - Supported fields
            var supportedFields = new[] { "TotalInsurance", "TotalDeduction", "SubTotal", "InvoiceTotal", "TotalInternalFreight", "TotalOtherCost" };
            
            foreach (var field in supportedFields)
            {
                var isSupported = _service.IsFieldSupported(field);
                Assert.That(isSupported, Is.True, $"Field {field} should be supported");
                _logger.Information("✅ **FIELD_SUPPORTED**: {FieldName} is correctly identified as supported", field);
            }

            // Act & Assert - Unsupported fields
            var unsupportedFields = new[] { "UnsupportedField", "InvalidField", "TestField" };
            
            foreach (var field in unsupportedFields)
            {
                var isSupported = _service.IsFieldSupported(field);
                Assert.That(isSupported, Is.False, $"Field {field} should not be supported");
                _logger.Information("✅ **FIELD_UNSUPPORTED**: {FieldName} is correctly identified as unsupported", field);
            }

            _logger.Information("🔍 **TEST_COMPLETE**: Field support validation test completed successfully");
        }

        /// <summary>
        /// Test the TotalsZero calculation that drives the need for OCR correction.
        /// This ensures the triggering logic is working correctly.
        /// </summary>
        [Test]
        public void TotalsZeroCalculation_ShouldDetectImbalance_WhenFieldsMissing()
        {
            _logger.Information("🔍 **TEST_ARRANGE**: Testing TotalsZero calculation with missing fields");

            // Create invoice with missing TotalInsurance (gift card)
            var unbalancedInvoice = new ShipmentInvoice
            {
                InvoiceNo = "112-9126443-1163432",
                InvoiceTotal = 166.30,
                SubTotal = 161.95,
                TotalInternalFreight = 6.99,
                TotalOtherCost = 11.34,
                TotalInsurance = 0.0,  // Missing gift card (should be -6.99)
                TotalDeduction = 6.99  // Free shipping detected correctly
            };

            // Act
            bool isBalanced = WaterNut.DataSpace.OCRCorrectionService.TotalsZero(unbalancedInvoice, out double difference, _logger);

            // Assert
            Assert.That(isBalanced, Is.False, "Invoice with missing gift card should be unbalanced");
            Assert.That(Math.Abs(difference - 6.99), Is.LessThan(0.01), "Difference should be approximately 6.99 (missing gift card amount)");

            _logger.Information("✅ **TEST_SUCCESS**: TotalsZero correctly detected imbalance of {Difference:F2}", difference);

            // Test ShouldContinueCorrections logic
            var csvLines = new List<dynamic>
            {
                new Dictionary<string, object>
                {
                    ["InvoiceNo"] = unbalancedInvoice.InvoiceNo,
                    ["InvoiceTotal"] = unbalancedInvoice.InvoiceTotal,
                    ["SubTotal"] = unbalancedInvoice.SubTotal,
                    ["TotalInternalFreight"] = unbalancedInvoice.TotalInternalFreight,
                    ["TotalOtherCost"] = unbalancedInvoice.TotalOtherCost,
                    ["TotalInsurance"] = unbalancedInvoice.TotalInsurance,
                    ["TotalDeduction"] = unbalancedInvoice.TotalDeduction
                }
            };

            bool shouldContinue = WaterNut.DataSpace.OCRCorrectionService.ShouldContinueCorrections(csvLines, out double totalImbalance, _logger);

            Assert.That(shouldContinue, Is.True, "ShouldContinueCorrections should return TRUE for unbalanced invoice");
            Assert.That(totalImbalance, Is.GreaterThan(0.01), "Total imbalance should be greater than tolerance");

            _logger.Information("✅ **TEST_SUCCESS**: ShouldContinueCorrections correctly returned TRUE for unbalanced invoice");
            _logger.Information("🔍 **TEST_COMPLETE**: TotalsZero calculation test completed successfully");
        }

        /// <summary>
        /// Test template context creation without requiring actual database templates.
        /// This validates the metadata preparation logic.
        /// </summary>
        [Test]
        public void TemplateContextCreation_ShouldCreateValidContext_FromMetadata()
        {
            _logger.Information("🔍 **TEST_ARRANGE**: Testing template context creation");

            // Arrange
            var metadata = new Dictionary<string, OCRFieldMetadata>
            {
                ["TotalInsurance"] = new OCRFieldMetadata
                {
                    FieldName = "TotalInsurance",
                    Field = "TotalInsurance",
                    EntityType = "ShipmentInvoice",
                    DataType = "double?",
                    IsRequired = false,
                    LineNumber = 15,
                    LineText = "Gift Card Amount: -$6.99",
                    InvoiceId = 1
                }
            };

            var fileText = "Order Total: $166.30\n\nGift Card Amount: -$6.99\n\nShipped on April 17, 2025";

            // Act
            var templateContext = _service.CreateTemplateContextInternal(metadata, fileText);

            // Assert
            Assert.That(templateContext, Is.Not.Null, "Template context should be created");
            Assert.That(templateContext.Metadata, Is.Not.Null, "Metadata should be populated");
            Assert.That(templateContext.Metadata.ContainsKey("TotalInsurance"), Is.True, "Should contain TotalInsurance metadata");
            Assert.That(templateContext.FileText, Is.EqualTo(fileText), "File text should be preserved");

            _logger.Information("✅ **TEST_SUCCESS**: Template context created successfully with {MetadataCount} fields", 
                templateContext.Metadata.Count);
            _logger.Information("🔍 **TEST_COMPLETE**: Template context creation test completed successfully");
        }
    }
}