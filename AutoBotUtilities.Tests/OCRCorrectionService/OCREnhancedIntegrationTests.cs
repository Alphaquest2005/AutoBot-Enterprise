using System;
using System.Collections.Generic;
using System.Linq;
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
    /// Tests for enhanced OCR metadata integration with field mapping and database updates
    /// </summary>
    [TestFixture]
    public class OCREnhancedIntegrationTests
    {
        private OCRCorrectionService _correctionService;
        private ILogger _logger;

        [SetUp]
        public void Setup()
        {
            // Configure logger for test output
            _logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();

            _correctionService = new OCRCorrectionService(_logger);
        }

        [Test]
        public void MapDeepSeekFieldToEnhancedInfo_WithValidFieldAndMetadata_ReturnsEnhancedFieldInfo()
        {
            // Arrange
            var metadata = CreateTestOCRMetadata("TotalDeduction", "5.99");

            // Act
            var result = _correctionService.MapDeepSeekFieldToEnhancedInfo("GiftCard", metadata);

            // Assert
            Assert.That(result, Is.Not.Null, "Enhanced field info should not be null");
            Assert.That(result.DatabaseFieldName, Is.EqualTo("TotalDeduction"), "Should map to correct database field");
            Assert.That(result.EntityType, Is.EqualTo("ShipmentInvoice"), "Should have correct entity type");
            Assert.That(result.HasOCRContext, Is.True, "Should indicate OCR context is available");
            Assert.That(result.CanUpdatePatternsViaContext, Is.True, "Should be able to update database with complete metadata");
            Assert.That(result.OCRMetadata, Is.SameAs(metadata), "Should preserve original metadata");
        }

        [Test]
        public void MapDeepSeekFieldToEnhancedInfo_WithUnknownField_ReturnsNull()
        {
            // Arrange
            var metadata = CreateTestOCRMetadata("UnknownField", "test");

            // Act
            var result = _correctionService.MapDeepSeekFieldToEnhancedInfo("UnknownField", metadata);

            // Assert
            Assert.That(result, Is.Null, "Should return null for unknown fields");
        }

        [Test]
        public void GetDatabaseUpdateContext_WithCompleteMetadata_ReturnsValidContext()
        {
            // Arrange
            var metadata = CreateTestOCRMetadata("TotalDeduction", "5.99");

            // Act
            var result = _correctionService.GetDatabaseUpdateContext("GiftCard", metadata);

            // Assert
            Assert.That(result, Is.Not.Null, "Update context should not be null");
            Assert.That(result.IsValid, Is.True, "Context should be valid");
            Assert.That(result.UpdateStrategy, Is.EqualTo(DatabaseUpdateStrategy.UpdateRegexPattern), "Should use regex pattern update strategy with complete metadata");
            Assert.That(result.RequiredIds, Is.Not.Null, "Required IDs should be populated");
            Assert.That(result.RequiredIds.FieldId, Is.EqualTo(123), "Should have correct field ID");
            Assert.That(result.RequiredIds.LineId, Is.EqualTo(456), "Should have correct line ID");
            Assert.That(result.RequiredIds.RegexId, Is.EqualTo(789), "Should have correct regex ID");
        }

        [Test]
        public void GetDatabaseUpdateContext_WithPartialMetadata_ReturnsAppropriateStrategy()
        {
            // Arrange - metadata without regex ID
            var metadata = CreateTestOCRMetadata("TotalDeduction", "5.99");
            metadata.RegexId = null; // Remove regex ID

            // Act
            var result = _correctionService.GetDatabaseUpdateContext("GiftCard", metadata);

            // Assert
            Assert.That(result, Is.Not.Null, "Update context should not be null");
            Assert.That(result.IsValid, Is.True, "Context should be valid");
            Assert.That(result.UpdateStrategy, Is.EqualTo(DatabaseUpdateStrategy.CreateNewPattern), "Should use create new pattern strategy without regex ID");
        }

        [Test]
        public void GetDatabaseUpdateContext_WithMinimalMetadata_ReturnsFieldFormatStrategy()
        {
            // Arrange - metadata with only field ID
            var metadata = CreateTestOCRMetadata("TotalDeduction", "5.99");
            metadata.LineId = null;
            metadata.RegexId = null;
            metadata.PartId = null; // Remove part ID to make it truly minimal

            // Act
            var result = _correctionService.GetDatabaseUpdateContext("GiftCard", metadata);

            // Assert
            Assert.That(result, Is.Not.Null, "Update context should not be null");
            Assert.That(result.IsValid, Is.True, "Context should be valid");
            Assert.That(result.UpdateStrategy, Is.EqualTo(DatabaseUpdateStrategy.UpdateFieldFormat), "Should use field format update strategy with minimal metadata");
        }

        [Test]
        public void GetDatabaseUpdateContext_WithNoMetadata_ReturnsLogOnlyStrategy()
        {
            // Arrange - no metadata
            OCRFieldMetadata metadata = null;

            // Act
            var result = _correctionService.GetDatabaseUpdateContext("GiftCard", metadata);

            // Assert
            Assert.That(result, Is.Not.Null, "Update context should not be null");
            Assert.That(result.IsValid, Is.True, "Context should be valid for known fields even without metadata");
            Assert.That(result.UpdateStrategy, Is.EqualTo(DatabaseUpdateStrategy.SkipUpdate), "Should skip update without metadata");
        }

        [Test]
        public async Task ProcessExternalCorrectionsForDBUpdateAsync_WithValidData_ProcessesSuccessfully()
        {
            // Arrange
            var corrections = CreateTestCorrections();
            var invoiceMetadata = CreateTestInvoiceMetadata();
            var fileText = "Sample OCR text with gift card: $5.99";
            var filePath = "test_invoice.txt";

            // Act
            var result = await _correctionService.ProcessExternalCorrectionsForDBUpdateAsync(
                corrections, invoiceMetadata, fileText, filePath);

            // Assert
            Assert.That(result, Is.Not.Null, "Processing result should not be null");
            Assert.That(result.TotalCorrections, Is.EqualTo(2), "Should process all corrections");
            Assert.That(result.ProcessedCorrections.Count, Is.EqualTo(2), "Should have processed correction details");
            Assert.That(result.HasErrors, Is.False, "Should not have errors");
            Assert.That(result.ProcessingDuration.TotalMilliseconds, Is.GreaterThan(0), "Should have processing duration");

            // Check individual correction processing
            var giftCardCorrection = result.ProcessedCorrections.FirstOrDefault(c =>
                c.Correction.FieldName == "GiftCard");
            Assert.That(giftCardCorrection, Is.Not.Null, "Should have processed gift card correction");
            Assert.That(giftCardCorrection.HasMetadata, Is.True, "Should have metadata for gift card field");
            Assert.That(giftCardCorrection.UpdateContext, Is.Not.Null, "Should have update context");
        }

        [Test]
        public void IsFieldSupported_WithSupportedFields_ReturnsTrue()
        {
            // Test various supported field names
            var supportedFields = new[] { "InvoiceTotal", "GiftCard", "Total", "Subtotal", "Price", "Qty" };

            foreach (var field in supportedFields)
            {
                var result = _correctionService.IsFieldSupported(field);
                Assert.That(result, Is.True, $"Field '{field}' should be supported");
            }
        }

        [Test]
        public void IsFieldSupported_WithUnsupportedField_ReturnsFalse()
        {
            // Act & Assert
            Assert.That(_correctionService.IsFieldSupported("UnsupportedField"), Is.False);
            Assert.That(_correctionService.IsFieldSupported(""), Is.False);
            Assert.That(_correctionService.IsFieldSupported(null), Is.False);
        }

        [Test]
        public void GetFieldValidationInfo_WithMonetaryField_ReturnsCorrectValidation()
        {
            // Act
            var result = _correctionService.GetFieldValidationInfo("TotalDeduction");

            // Assert
            Assert.That(result, Is.Not.Null, "Validation info should not be null");
            Assert.That(result.IsValid, Is.True, "Field should be valid");
            Assert.That(result.IsRequired, Is.False, "TotalDeduction should not be required");
            Assert.That(result.DataType, Is.EqualTo("decimal"), "Should have decimal data type");
            Assert.That(result.IsMonetary, Is.True, "Should be identified as monetary field");
            Assert.That(result.ValidationPattern, Is.Not.Null, "Should have validation pattern");
        }

        #region Helper Methods

        private OCRFieldMetadata CreateTestOCRMetadata(string fieldName, string value)
        {
            return new OCRFieldMetadata
            {
                FieldName = fieldName,
                Value = value,
                RawValue = value,
                LineNumber = 5,
                FieldId = 123,
                LineId = 456,
                RegexId = 789,
                Key = "Save",
                Field = fieldName,
                EntityType = "Invoice",
                DataType = "Number",
                LineName = "Gift Card Line",
                LineRegex = @"gift\s*card[:\s]*\$?(\d+\.?\d*)",
                IsRequired = false,
                PartId = 101,
                PartName = "Header",
                PartTypeId = 1,
                InvoiceId = 202,
                InvoiceName = "Amazon",
                Section = "Single",
                Instance = "1",
                Confidence = 0.95
            };
        }

        private List<CorrectionResult> CreateTestCorrections()
        {
            return new List<CorrectionResult>
            {
                new CorrectionResult
                {
                    FieldName = "GiftCard",
                    OldValue = "5,99",
                    NewValue = "5.99",
                    Success = true,
                    Confidence = 0.95,
                    CorrectionType = "DecimalSeparator",
                    Reasoning = "Corrected comma to period in decimal",
                    LineNumber = 5
                },
                new CorrectionResult
                {
                    FieldName = "InvoiceTotal",
                    OldValue = "123,45",
                    NewValue = "123.45",
                    Success = true,
                    Confidence = 0.98,
                    CorrectionType = "DecimalSeparator",
                    Reasoning = "Corrected comma to period in total",
                    LineNumber = 10
                }
            };
        }

        private Dictionary<string, OCRFieldMetadata> CreateTestInvoiceMetadata()
        {
            return new Dictionary<string, OCRFieldMetadata>
            {
                ["GiftCard"] = CreateTestOCRMetadata("TotalDeduction", "5.99"),
                ["InvoiceTotal"] = CreateTestOCRMetadata("InvoiceTotal", "123.45")
            };
        }

        #endregion
    }
}
