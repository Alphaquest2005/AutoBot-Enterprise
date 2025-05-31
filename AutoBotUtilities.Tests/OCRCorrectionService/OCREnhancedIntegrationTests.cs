using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Serilog;
using WaterNut.DataSpace;
using InvoiceReader.OCRCorrectionService;
using OCR.Business.Entities;

namespace AutoBotUtilities.Tests.OCRCorrectionService
{
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

            _correctionService = new OCRCorrectionService();
        }

        [Test]
        public void MapDeepSeekFieldWithMetadata_WithValidFieldAndMetadata_ReturnsEnhancedFieldInfo()
        {
            // Arrange
            var metadata = CreateTestOCRMetadata("TotalDeduction", "5.99");

            // Act
            var result = _correctionService.MapDeepSeekFieldWithMetadata("GiftCard", metadata);

            // Assert
            Assert.IsNotNull(result, "Enhanced field info should not be null");
            Assert.AreEqual("TotalDeduction", result.DatabaseFieldName, "Should map to correct database field");
            Assert.AreEqual("ShipmentInvoice", result.EntityType, "Should have correct entity type");
            Assert.IsTrue(result.HasOCRContext, "Should indicate OCR context is available");
            Assert.IsTrue(result.CanUpdateDatabase, "Should be able to update database with complete metadata");
            Assert.AreSame(metadata, result.OCRMetadata, "Should preserve original metadata");
        }

        [Test]
        public void MapDeepSeekFieldWithMetadata_WithUnknownField_ReturnsNull()
        {
            // Arrange
            var metadata = CreateTestOCRMetadata("UnknownField", "test");

            // Act
            var result = _correctionService.MapDeepSeekFieldWithMetadata("UnknownField", metadata);

            // Assert
            Assert.IsNull(result, "Should return null for unknown fields");
        }

        [Test]
        public void GetDatabaseUpdateContext_WithCompleteMetadata_ReturnsValidContext()
        {
            // Arrange
            var metadata = CreateTestOCRMetadata("TotalDeduction", "5.99");

            // Act
            var result = _correctionService.GetDatabaseUpdateContext("GiftCard", metadata);

            // Assert
            Assert.IsNotNull(result, "Update context should not be null");
            Assert.IsTrue(result.IsValid, "Context should be valid");
            Assert.AreEqual(DatabaseUpdateStrategy.UpdateRegexPattern, result.UpdateStrategy,
                "Should use regex pattern update strategy with complete metadata");
            Assert.IsNotNull(result.RequiredIds, "Required IDs should be populated");
            Assert.AreEqual(123, result.RequiredIds.FieldId, "Should have correct field ID");
            Assert.AreEqual(456, result.RequiredIds.LineId, "Should have correct line ID");
            Assert.AreEqual(789, result.RequiredIds.RegexId, "Should have correct regex ID");
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
            Assert.IsNotNull(result, "Update context should not be null");
            Assert.IsTrue(result.IsValid, "Context should be valid");
            Assert.AreEqual(DatabaseUpdateStrategy.CreateNewPattern, result.UpdateStrategy,
                "Should use create new pattern strategy without regex ID");
        }

        [Test]
        public void GetDatabaseUpdateContext_WithMinimalMetadata_ReturnsFieldFormatStrategy()
        {
            // Arrange - metadata with only field ID
            var metadata = CreateTestOCRMetadata("TotalDeduction", "5.99");
            metadata.LineId = null;
            metadata.RegexId = null;

            // Act
            var result = _correctionService.GetDatabaseUpdateContext("GiftCard", metadata);

            // Assert
            Assert.IsNotNull(result, "Update context should not be null");
            Assert.IsTrue(result.IsValid, "Context should be valid");
            Assert.AreEqual(DatabaseUpdateStrategy.UpdateFieldFormat, result.UpdateStrategy,
                "Should use field format update strategy with minimal metadata");
        }

        [Test]
        public void GetDatabaseUpdateContext_WithNoMetadata_ReturnsLogOnlyStrategy()
        {
            // Arrange - no metadata
            OCRFieldMetadata metadata = null;

            // Act
            var result = _correctionService.GetDatabaseUpdateContext("GiftCard", metadata);

            // Assert
            Assert.IsNotNull(result, "Update context should not be null");
            Assert.IsTrue(result.IsValid, "Context should be valid for known fields even without metadata");
            Assert.AreEqual(DatabaseUpdateStrategy.SkipUpdate, result.UpdateStrategy,
                "Should skip update without metadata");
        }

        [Test]
        public async Task ProcessCorrectionsWithEnhancedMetadataAsync_WithValidData_ProcessesSuccessfully()
        {
            // Arrange
            var corrections = CreateTestCorrections();
            var invoiceMetadata = CreateTestInvoiceMetadata();
            var fileText = "Sample OCR text with gift card: $5.99";
            var filePath = "test_invoice.txt";

            // Act
            var result = await _correctionService.ProcessCorrectionsWithEnhancedMetadataAsync(
                corrections, invoiceMetadata, fileText, filePath);

            // Assert
            Assert.IsNotNull(result, "Processing result should not be null");
            Assert.AreEqual(2, result.TotalCorrections, "Should process all corrections");
            Assert.AreEqual(2, result.ProcessedCorrections.Count, "Should have processed correction details");
            Assert.IsFalse(result.HasErrors, "Should not have errors");
            Assert.Greater(result.ProcessingDuration.TotalMilliseconds, 0, "Should have processing duration");

            // Check individual correction processing
            var giftCardCorrection = result.ProcessedCorrections.FirstOrDefault(c =>
                c.Correction.FieldName == "GiftCard");
            Assert.IsNotNull(giftCardCorrection, "Should have processed gift card correction");
            Assert.IsTrue(giftCardCorrection.HasMetadata, "Should have metadata for gift card field");
            Assert.IsNotNull(giftCardCorrection.UpdateContext, "Should have update context");
        }

        [Test]
        public void IsFieldSupported_WithSupportedFields_ReturnsTrue()
        {
            // Test various supported field names
            var supportedFields = new[] { "InvoiceTotal", "GiftCard", "Total", "Subtotal", "Price", "Qty" };

            foreach (var field in supportedFields)
            {
                var result = _correctionService.IsFieldSupported(field);
                Assert.IsTrue(result, $"Field '{field}' should be supported");
            }
        }

        [Test]
        public void IsFieldSupported_WithUnsupportedField_ReturnsFalse()
        {
            // Act & Assert
            Assert.IsFalse(_correctionService.IsFieldSupported("UnsupportedField"));
            Assert.IsFalse(_correctionService.IsFieldSupported(""));
            Assert.IsFalse(_correctionService.IsFieldSupported(null));
        }

        [Test]
        public void GetFieldValidationInfo_WithMonetaryField_ReturnsCorrectValidation()
        {
            // Act
            var result = _correctionService.GetFieldValidationInfo("TotalDeduction");

            // Assert
            Assert.IsNotNull(result, "Validation info should not be null");
            Assert.IsTrue(result.IsValid, "Field should be valid");
            Assert.IsFalse(result.IsRequired, "TotalDeduction should not be required");
            Assert.AreEqual("decimal", result.DataType, "Should have decimal data type");
            Assert.IsTrue(result.IsMonetary, "Should be identified as monetary field");
            Assert.IsNotNull(result.ValidationPattern, "Should have validation pattern");
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
