using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using NUnit.Framework;
using Serilog;
using WaterNut.DataSpace;
using InvoiceReader.OCRCorrectionService;
using OCR.Business.Entities;

namespace AutoBotUtilities.Tests.OCRCorrectionService
{
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

            _correctionService = new OCRCorrectionService();
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
            var metadata = OCRCorrectionService.ExtractEnhancedOCRMetadata(invoiceDict, template, fieldMappings);

            // Verify metadata extraction
            Assert.IsNotNull(metadata, "Metadata should be extracted");
            Assert.Greater(metadata.Count, 0, "Should have extracted metadata for fields");

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
            var result = await _correctionService.ProcessCorrectionsWithEnhancedMetadataAsync(
                corrections, metadata, invoiceText, "test_amazon_invoice.txt");

            // Step 4: Verify results
            _logger.Information("=== Step 4: Verifying Results ===");
            Assert.IsNotNull(result, "Processing result should not be null");
            Assert.IsFalse(result.HasErrors, $"Processing should not have errors: {result.ErrorMessage}");
            Assert.AreEqual(corrections.Count, result.TotalCorrections, "Should process all corrections");

            _logger.Information("Processing completed: {SuccessfulUpdates} successful, {FailedUpdates} failed, Duration: {Duration}ms",
                result.SuccessfulUpdates, result.FailedUpdates, result.ProcessingDuration.TotalMilliseconds);

            // Verify each correction was processed with appropriate strategy
            foreach (var detail in result.ProcessedCorrections)
            {
                var correction = detail.Correction;
                _logger.Information("Processed {FieldName}: Strategy={Strategy}, HasMetadata={HasMetadata}, Success={Success}",
                    correction.FieldName, detail.UpdateContext?.UpdateStrategy, detail.HasMetadata,
                    detail.DatabaseUpdate?.IsSuccess);

                Assert.IsNotNull(detail.UpdateContext, $"Update context should exist for {correction.FieldName}");

                // Verify strategy selection based on metadata availability
                if (detail.HasMetadata && detail.OCRMetadata?.RegexId.HasValue == true)
                {
                    Assert.AreEqual(DatabaseUpdateStrategy.UpdateRegexPattern, detail.UpdateContext.UpdateStrategy,
                        $"Should use regex pattern update for {correction.FieldName} with complete metadata");
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

                Assert.IsNotNull(mappedField, $"Field '{deepSeekField}' should be mapped");
                Assert.AreEqual(expectedField, mappedField.DatabaseFieldName,
                    $"Field '{deepSeekField}' should map to '{expectedField}'");
                Assert.AreEqual(expectedEntity, mappedField.EntityType,
                    $"Field '{deepSeekField}' should map to entity '{expectedEntity}'");

                _logger.Information("✓ {DeepSeekField} -> {DatabaseField} ({Entity})",
                    deepSeekField, mappedField.DatabaseFieldName, mappedField.EntityType);
            }
        }

        #region Helper Methods

        private Invoice CreateMockInvoiceTemplate()
        {
            // Create a minimal mock template for testing
            return new Invoice
            {
                OcrInvoices = new OCR_Invoices { Id = 1, Name = "Amazon" },
                Parts = new List<Part>(),
                Lines = new List<Line>()
            };
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

        private Dictionary<string, (int LineId, int FieldId)> CreateMockFieldMappings()
        {
            return new Dictionary<string, (int LineId, int FieldId)>
            {
                ["InvoiceTotal"] = (1, 101),
                ["SubTotal"] = (2, 102),
                ["TotalInternalFreight"] = (3, 103),
                ["TotalOtherCost"] = (4, 104),
                ["TotalDeduction"] = (5, 105),
                ["InvoiceNo"] = (6, 106),
                ["InvoiceDate"] = (7, 107)
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
