using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using Serilog;
using WaterNut.DataSpace;
using OCR.Business.Entities;
using static AutoBotUtilities.Tests.TestHelpers;

namespace AutoBotUtilities.Tests.OCRCorrectionService
{
    using OCRCorrectionService = WaterNut.DataSpace.OCRCorrectionService;
    /// <summary>
    /// Integration tests that demonstrate the complete enhanced OCR correction workflow
    /// </summary>
    [TestFixture]
    public class OCRWorkflowIntegrationTests
    {
        private OCRCorrectionService _correctionService;
        private ILogger _logger;

        [SetUp]
        public void Setup()
        {
            // Configure logger for detailed test output
            _logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            _correctionService = new OCRCorrectionService(_logger);
        }

        [Test]
        public async Task CompleteWorkflow_AmazonInvoiceWithGiftCard_ProcessesCorrectly()
        {
            // Arrange - Simulate Amazon invoice processing
            var invoiceText = @"
Amazon.com
Order #123-4567890-1234567
Invoice Date: 12/15/2023

Items:
Product A    Qty: 2    Price: $15,99    Total: $31,98
Product B    Qty: 1    Price: $8,50     Total: $8,50

Subtotal: $40,48
Shipping: $5,99
Tax: $3,24
Gift Card Applied: -$5,99
Total: $43,72
";

            // Simulate OCR template processing results
            var template = CreateMockInvoiceTemplate();
            var invoiceDict = CreateMockInvoiceDict();
            var fieldMappings = CreateMockFieldMappings();

            // Step 1: Extract enhanced OCR metadata
            _logger.Information("=== Step 1: Extracting Enhanced OCR Metadata ===");
            var metadata = _correctionService.ExtractEnhancedOCRMetadata(invoiceDict, template, fieldMappings);

            // Verify metadata extraction
            Assert.That(metadata, Is.Not.Null, "Metadata should be extracted");
            Assert.That(metadata.Count, Is.GreaterThan(0), "Should have extracted metadata for fields");

            foreach (var kvp in metadata)
            {
                _logger.Information("Extracted metadata for field {FieldName}: FieldId={FieldId}, LineId={LineId}, RegexId={RegexId}",
                    kvp.Key, kvp.Value.FieldId, kvp.Value.LineId, kvp.Value.RegexId);
            }

            // Step 2: Simulate DeepSeek corrections
            _logger.Information("=== Step 2: Processing DeepSeek Corrections ===");
            var corrections = CreateMockDeepSeekCorrections();

            foreach (var correction in corrections)
            {
                _logger.Information("DeepSeek correction: {FieldName} '{OldValue}' -> '{NewValue}' (Confidence: {Confidence})",
                    correction.FieldName, correction.OldValue, correction.NewValue, correction.Confidence);
            }

            // Step 3: Process corrections with enhanced metadata
            _logger.Information("=== Step 3: Processing Corrections with Enhanced Metadata ===");
            var result = await _correctionService.ProcessExternalCorrectionsForDBUpdateAsync(
                corrections, metadata, invoiceText, "test_amazon_invoice.txt");

            // Step 4: Verify results
            _logger.Information("=== Step 4: Verifying Results ===");
            Assert.That(result, Is.Not.Null, "Processing result should not be null");
            Assert.That(result.HasErrors, Is.False, $"Processing should not have errors: {result.ErrorMessage}");
            Assert.That(result.TotalCorrections, Is.EqualTo(corrections.Count), "Should process all corrections");

            _logger.Information("Processing completed: {SuccessfulUpdates} successful, {FailedUpdates} failed, Duration: {Duration}ms",
                result.SuccessfulUpdates, result.FailedUpdates, result.ProcessingDuration.TotalMilliseconds);

            // Verify each correction was processed with appropriate strategy
            foreach (var detail in result.ProcessedCorrections)
            {
                var correction = detail.Correction;
                _logger.Information("Processed {FieldName}: Strategy={Strategy}, HasMetadata={HasMetadata}, Success={Success}",
                    correction.FieldName, detail.UpdateContext?.UpdateStrategy, detail.HasMetadata,
                    detail.DatabaseUpdate?.IsSuccess);

                Assert.That(detail.UpdateContext, Is.Not.Null, $"Update context should exist for {correction.FieldName}");

                // Verify strategy selection based on metadata availability
                if (detail.HasMetadata && detail.OCRMetadata?.RegexId.HasValue == true)
                {
                    Assert.That(detail.UpdateContext.UpdateStrategy, Is.EqualTo(DatabaseUpdateStrategy.UpdateRegexPattern), $"Should use regex pattern update for {correction.FieldName} with complete metadata");
                }
                else if (detail.HasMetadata && detail.OCRMetadata?.FieldId.HasValue == true)
                {
                    Assert.That(detail.UpdateContext.UpdateStrategy,
                        Is.AnyOf(DatabaseUpdateStrategy.CreateNewPattern, DatabaseUpdateStrategy.UpdateFieldFormat),
                        $"Should use appropriate strategy for {correction.FieldName} with partial metadata");
                }
            }
        }

        [Test]
        public void FieldMapping_AllSupportedFields_MapCorrectly()
        {
            // Test comprehensive field mapping
            var testMappings = new Dictionary<string, (string ExpectedField, string ExpectedEntity)>
            {
                ["InvoiceTotal"] = ("InvoiceTotal", "ShipmentInvoice"),
                ["Total"] = ("InvoiceTotal", "ShipmentInvoice"),
                ["GiftCard"] = ("TotalDeduction", "ShipmentInvoice"),
                ["Deduction"] = ("TotalDeduction", "ShipmentInvoice"),
                ["Subtotal"] = ("SubTotal", "ShipmentInvoice"),
                ["Freight"] = ("TotalInternalFreight", "ShipmentInvoice"),
                ["Shipping"] = ("TotalInternalFreight", "ShipmentInvoice"),
                ["Tax"] = ("TotalOtherCost", "ShipmentInvoice"),
                ["Price"] = ("Cost", "InvoiceDetails"),
                ["Qty"] = ("Quantity", "InvoiceDetails"),
                ["Description"] = ("ItemDescription", "InvoiceDetails")
            };

            _logger.Information("=== Testing Field Mappings ===");

            foreach (var kvp in testMappings)
            {
                var deepSeekField = kvp.Key;
                var (expectedField, expectedEntity) = kvp.Value;

                var mappedField = _correctionService.MapDeepSeekFieldToDatabase(deepSeekField);

                Assert.That(mappedField, Is.Not.Null, $"Field '{deepSeekField}' should be mapped");
                Assert.That(mappedField.DatabaseFieldName, Is.EqualTo(expectedField), $"Field '{deepSeekField}' should map to '{expectedField}'");
                Assert.That(mappedField.EntityType, Is.EqualTo(expectedEntity), $"Field '{deepSeekField}' should map to entity '{expectedEntity}'");

                _logger.Information("✓ {DeepSeekField} -> {DatabaseField} ({Entity})",
                    deepSeekField, mappedField.DatabaseFieldName, mappedField.EntityType);
            }
        }

        #region Helper Methods

        private Invoice CreateMockInvoiceTemplate()
        {
            // Create a minimal mock template for testing
            var ocrInvoices = new Invoices { Id = 1, Name = "Amazon" };
            return new Invoice(ocrInvoices, _logger);
            // Note: Parts are initialized in the constructor, Lines is computed from Parts
        }

        private Dictionary<string, object> CreateMockInvoiceDict()
        {
            return new Dictionary<string, object>
            {
                ["InvoiceTotal"] = "43,72",
                ["SubTotal"] = "40,48",
                ["TotalInternalFreight"] = "5,99",
                ["TotalOtherCost"] = "3,24",
                ["TotalDeduction"] = "5,99",
                ["InvoiceNo"] = "123-4567890-1234567",
                ["InvoiceDate"] = "12/15/2023"
            };
        }

        private Dictionary<string, (int LineId, int FieldId, int? PartId)> CreateMockFieldMappings()
        {
            return new Dictionary<string, (int LineId, int FieldId, int? PartId)>
            {
                ["InvoiceTotal"] = (1, 101, 1),
                ["SubTotal"] = (2, 102, 1),
                ["TotalInternalFreight"] = (3, 103, 1),
                ["TotalOtherCost"] = (4, 104, 1),
                ["TotalDeduction"] = (5, 105, 1),
                ["InvoiceNo"] = (6, 106, 1),
                ["InvoiceDate"] = (7, 107, 1)
            };
        }

        private List<CorrectionResult> CreateMockDeepSeekCorrections()
        {
            return new List<CorrectionResult>
            {
                new CorrectionResult
                {
                    FieldName = "InvoiceTotal",
                    OldValue = "43,72",
                    NewValue = "43.72",
                    Success = true,
                    Confidence = 0.98,
                    CorrectionType = "DecimalSeparator",
                    Reasoning = "Corrected European decimal separator to US format",
                    LineNumber = 12
                },
                new CorrectionResult
                {
                    FieldName = "GiftCard",
                    OldValue = "5,99",
                    NewValue = "5.99",
                    Success = true,
                    Confidence = 0.95,
                    CorrectionType = "DecimalSeparator",
                    Reasoning = "Corrected gift card amount decimal separator",
                    LineNumber = 11
                },
                new CorrectionResult
                {
                    FieldName = "Subtotal",
                    OldValue = "40,48",
                    NewValue = "40.48",
                    Success = true,
                    Confidence = 0.97,
                    CorrectionType = "DecimalSeparator",
                    Reasoning = "Corrected subtotal decimal separator",
                    LineNumber = 8
                }
            };
        }

        #endregion
    }
}
