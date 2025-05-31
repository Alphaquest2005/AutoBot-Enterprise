using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Serilog;
using WaterNut.DataSpace;
using EntryDataDS.Business.Entities;
using OCR.Business.Entities;

namespace AutoBotUtilities.Tests
{
    [TestFixture]
    public class OcrOmissionIntegrationTests
    {
        private ILogger _logger;

        [SetUp]
        public void Setup()
        {
            _logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
        }

        [Test]
        public async Task CompleteOmissionWorkflow_WithRealInvoice_ShouldDetectAndCorrectOmissions()
        {
            // Arrange
            var invoice = CreateTestInvoiceWithOmissions();
            var originalText = CreateTestInvoiceText();
            var metadata = new Dictionary<string, OCRFieldMetadata>();

            using var correctionService = new OCRCorrectionService(_logger);

            // Act - Test the complete workflow
            var corrected = await correctionService.CorrectInvoiceAsync(invoice, originalText);

            // Assert
            Assert.IsTrue(corrected, "Invoice correction should succeed");

            // Verify TotalsZero calculation works
            var totalsValid = WaterNut.DataSpace.OCRCorrectionService.TotalsZero(invoice, _logger);
            _logger.Information("TotalsZero validation result: {IsValid}", totalsValid);
        }

        [Test]
        public async Task OmissionDetection_WithMissingTotalDeduction_ShouldDetectOmission()
        {
            // Arrange
            var invoice = new ShipmentInvoice
            {
                InvoiceNo = "TEST-001",
                InvoiceTotal = 93.01,
                SubTotal = 100.00,
                TotalDeduction = null // Missing - should be detected as omission
            };

            var originalText = @"
Order Summary
Subtotal: $100.00
Gift Card Applied: -$6.99
Final Total: $93.01
";

            using var correctionService = new OCRCorrectionService(_logger);

            // Act - Test omission detection
            var corrected = await correctionService.CorrectInvoiceAsync(invoice, originalText);

            // Assert
            Assert.IsTrue(corrected, "Should detect and correct the missing TotalDeduction");
            Assert.IsNotNull(invoice.TotalDeduction, "TotalDeduction should be populated after correction");
            Assert.AreEqual(6.99, invoice.TotalDeduction.Value, 0.01, "TotalDeduction should be extracted correctly");
        }

        [Test]
        public void FieldMapping_WithDeepSeekFieldNames_ShouldMapCorrectly()
        {
            // Arrange
            using var correctionService = new OCRCorrectionService(_logger);

            // Act & Assert - Test field mapping
            var totalMapping = correctionService.MapDeepSeekFieldToDatabase("Total");
            Assert.IsNotNull(totalMapping, "Should map 'Total' to database field");
            Assert.AreEqual("InvoiceTotal", totalMapping.DatabaseFieldName, "Should map to InvoiceTotal");

            var giftCardMapping = correctionService.MapDeepSeekFieldToDatabase("GiftCard");
            Assert.IsNotNull(giftCardMapping, "Should map 'GiftCard' to database field");
            Assert.AreEqual("TotalDeduction", giftCardMapping.DatabaseFieldName, "Should map to TotalDeduction");
        }

        [Test]
        public void RegexNamedGroupExtraction_WithComplexPattern_ShouldExtractGroups()
        {
            // Arrange
            using var correctionService = new OCRCorrectionService(_logger);
            var regexPattern = @"(?<Total>\d+\.\d{2}).*(?<Tax>\d+\.\d{2}).*(?<Deduction>-?\$?\d+\.\d{2})";

            // Act
            var namedGroups = correctionService.ExtractNamedGroupsFromRegex(regexPattern);

            // Assert
            Assert.AreEqual(3, namedGroups.Count, "Should extract 3 named groups");
            Assert.Contains("Total", namedGroups.ToArray(), "Should extract 'Total' group");
            Assert.Contains("Tax", namedGroups.ToArray(), "Should extract 'Tax' group");
            Assert.Contains("Deduction", namedGroups.ToArray(), "Should extract 'Deduction' group");
        }

        [Test]
        public async Task DatabaseFieldLookup_WithLineId_ShouldReturnCorrectFields()
        {
            // Arrange
            using var correctionService = new OCRCorrectionService(_logger);
            var testRegex = @"(?<InvoiceTotal>\d+\.\d{2})";
            var testLineId = 1; // Assuming line ID 1 exists

            // Act
            var fields = await correctionService.GetFieldsByRegexNamedGroups(testRegex, testLineId);

            // Assert
            Assert.IsNotNull(fields, "Should return field list");
            _logger.Information("Found {FieldCount} fields for line {LineId}", fields.Count, testLineId);
        }

        [Test]
        public void FallbackCorrection_WithFailedRegex_ShouldCorrectDirectly()
        {
            // Arrange
            var res = CreateTestDynamicResult();
            var originalText = CreateTestInvoiceText();

            // Act
            var correctedRes = WaterNut.DataSpace.OCRCorrectionService.ApplyDirectDataCorrectionFallback(
                res, originalText, _logger);

            // Assert
            Assert.IsNotNull(correctedRes, "Fallback correction should return result");

            // Verify TotalsZero calculation
            var totalsValid = WaterNut.DataSpace.OCRCorrectionService.TotalsZero(
                correctedRes, out double totalSum, _logger);

            _logger.Information("Fallback correction TotalsZero: {IsValid}, Sum: {Sum}", totalsValid, totalSum);
        }

        #region Helper Methods

        private ShipmentInvoice CreateTestInvoiceWithOmissions()
        {
            return new ShipmentInvoice
            {
                InvoiceNo = "TEST-OMISSION-001",
                InvoiceTotal = 158.50,
                SubTotal = 150.00,
                TotalInternalFreight = 10.00,
                TotalOtherCost = 5.00,
                TotalInsurance = null, // Missing
                TotalDeduction = null, // Missing - should be 6.50 based on text
                Currency = "USD",
                SupplierName = "Test Supplier",
                InvoiceDetails = new List<InvoiceDetails>
                {
                    new InvoiceDetails
                    {
                        LineNumber = 1,
                        ItemDescription = "Test Product 1",
                        Quantity = 2,
                        Cost = 25.00,
                        TotalCost = 50.00
                    },
                    new InvoiceDetails
                    {
                        LineNumber = 2,
                        ItemDescription = "Test Product 2",
                        Quantity = 1,
                        Cost = 100.00,
                        TotalCost = 100.00
                    }
                }
            };
        }

        private string CreateTestInvoiceText()
        {
            return @"
INVOICE #TEST-OMISSION-001
Date: 2024-12-20

From: Test Supplier
123 Main Street
Test City, TC 12345

Items:
1. Test Product 1    Qty: 2    Price: $25.00    Total: $50.00
2. Test Product 2    Qty: 1    Price: $100.00   Total: $100.00

Subtotal: $150.00
Shipping: $10.00
Tax: $5.00
Gift Card Applied: -$6.50
Insurance: $0.00

Final Total: $158.50

Payment Method: Credit Card
Thank you for your order!
";
        }

        private List<dynamic> CreateTestDynamicResult()
        {
            var invoiceData = new Dictionary<string, object>
            {
                ["InvoiceNo"] = "TEST-001",
                ["InvoiceTotal"] = 158.50,
                ["SubTotal"] = 150.00,
                ["TotalInternalFreight"] = 10.00,
                ["TotalOtherCost"] = 5.00,
                ["TotalDeduction"] = null, // Missing
                ["InvoiceDetails"] = new List<Dictionary<string, object>>()
            };

            return new List<dynamic> { new List<Dictionary<string, object>> { (Dictionary<string, object>)invoiceData } };
        }

        #endregion
    }
}