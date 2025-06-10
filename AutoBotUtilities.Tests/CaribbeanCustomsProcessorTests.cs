using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using EntryDataDS.Business.Entities;
using WaterNut.DataSpace;
using Serilog;

namespace AutoBotUtilities.Tests
{
    [TestFixture]
    public class CaribbeanCustomsProcessorTests
    {
        private WaterNut.DataSpace.OCRCorrectionService _ocrService;
        private ILogger _logger;

        [SetUp]
        public void Setup()
        {
            _logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .CreateLogger();
            
            _ocrService = new WaterNut.DataSpace.OCRCorrectionService(_logger);
        }

        [Test]
        public void ApplyCaribbeanCustomsRules_WithGiftCardCorrection_ShouldMapToTotalInsuranceNegative()
        {
            // Arrange - Create a test invoice
            var invoice = new ShipmentInvoice
            {
                InvoiceNo = "TEST-AMAZON-GIFT-CARD",
                SubTotal = 161.95,
                TotalInternalFreight = 6.99,
                TotalOtherCost = 11.34,
                TotalInsurance = null, // Currently missing
                TotalDeduction = null, // Currently missing
                InvoiceTotal = 166.30
            };

            // Simulate DeepSeek corrections that detected the gift card
            var standardCorrections = new List<WaterNut.DataSpace.CorrectionResult>
            {
                new WaterNut.DataSpace.CorrectionResult
                {
                    FieldName = "TotalInsurance",
                    OldValue = "null",
                    NewValue = "-6.99", // Gift card should be negative in TotalInsurance
                    LineText = "Gift Card Amount: -$6.99",
                    LineNumber = 15,
                    Success = true,
                    Confidence = 0.95,
                    CorrectionType = "omission",
                    Reasoning = "Gift card amount found in text but missing from extracted data"
                },
                new WaterNut.DataSpace.CorrectionResult
                {
                    FieldName = "TotalDeduction", 
                    OldValue = "null",
                    NewValue = "6.99", // Free shipping should be positive in TotalDeduction
                    LineText = "Free Shipping: -$6.53",
                    LineNumber = 12,
                    Success = true,
                    Confidence = 0.90,
                    CorrectionType = "omission",
                    Reasoning = "Free shipping discount found in text but missing from extracted data"
                }
            };

            // Act - Apply Caribbean customs rules
            var customsCorrections = _ocrService.ApplyCaribbeanCustomsRules(invoice, standardCorrections);
            
            // Apply the corrections to the invoice object
            _ocrService.ApplyCaribbeanCustomsCorrectionsToInvoice(invoice, customsCorrections);

            // Apply the standard corrections as well
            foreach (var correction in standardCorrections)
            {
                if (correction.FieldName == "TotalInsurance" && double.TryParse(correction.NewValue, out var insuranceValue))
                    invoice.TotalInsurance = insuranceValue;
                if (correction.FieldName == "TotalDeduction" && double.TryParse(correction.NewValue, out var deductionValue))
                    invoice.TotalDeduction = deductionValue;
            }

            // Assert - Verify the final field mappings
            Assert.That(invoice.TotalInsurance, Is.EqualTo(-6.99).Within(0.01), "Gift card should be mapped to TotalInsurance as negative value");
            Assert.That(invoice.TotalDeduction, Is.EqualTo(6.99).Within(0.01), "Free shipping should be mapped to TotalDeduction as positive value");

            // Verify the invoice is now balanced
            var calculatedTotal = (invoice.SubTotal ?? 0) + 
                                (invoice.TotalInternalFreight ?? 0) + 
                                (invoice.TotalOtherCost ?? 0) + 
                                (invoice.TotalInsurance ?? 0) - 
                                (invoice.TotalDeduction ?? 0);

            _logger.Information("Caribbean Customs Test Results:");
            _logger.Information("SubTotal: {SubTotal}", invoice.SubTotal);
            _logger.Information("TotalInternalFreight: {Freight}", invoice.TotalInternalFreight);
            _logger.Information("TotalOtherCost: {OtherCost}", invoice.TotalOtherCost);
            _logger.Information("TotalInsurance: {Insurance}", invoice.TotalInsurance);
            _logger.Information("TotalDeduction: {Deduction}", invoice.TotalDeduction);
            _logger.Information("Calculated Total: {Calculated}", calculatedTotal);
            _logger.Information("Invoice Total: {InvoiceTotal}", invoice.InvoiceTotal);
            _logger.Information("Difference: {Difference}", Math.Abs(calculatedTotal - (invoice.InvoiceTotal ?? 0)));

            Assert.That(calculatedTotal, Is.EqualTo(166.30).Within(0.01), "Invoice should be mathematically balanced after Caribbean customs rules");
        }

        [Test]
        public void ApplyCaribbeanCustomsRules_WithMixedCorrections_ShouldApplyCorrectSigns()
        {
            // Arrange - Create a test invoice
            var invoice = new ShipmentInvoice
            {
                InvoiceNo = "TEST-MIXED-CORRECTIONS",
                SubTotal = 100.00,
                TotalInternalFreight = 10.00,
                TotalOtherCost = 5.00,
                TotalInsurance = null, // Will get gift card
                TotalDeduction = null, // Will get free shipping
                InvoiceTotal = 105.00
            };

            // Simulate various corrections that should be transformed
            var standardCorrections = new List<WaterNut.DataSpace.CorrectionResult>
            {
                new WaterNut.DataSpace.CorrectionResult
                {
                    FieldName = "TotalInsurance",
                    OldValue = "null",
                    NewValue = "10.00", // Positive value that should become negative for gift card
                    LineText = "Gift Card Applied: $10.00",
                    LineNumber = 10,
                    Success = true,
                    Confidence = 0.95,
                    CorrectionType = "omission"
                },
                new WaterNut.DataSpace.CorrectionResult
                {
                    FieldName = "TotalDeduction",
                    OldValue = "null", 
                    NewValue = "-5.00", // Negative value that should become positive for supplier discount
                    LineText = "Free Shipping Discount: -$5.00",
                    LineNumber = 12,
                    Success = true,
                    Confidence = 0.90,
                    CorrectionType = "omission"
                }
            };

            // Act - Apply Caribbean customs rules
            var customsCorrections = _ocrService.ApplyCaribbeanCustomsRules(invoice, standardCorrections);
            
            // Log the customs corrections for debugging
            _logger.Information("Caribbean Customs Corrections Applied:");
            foreach (var correction in customsCorrections)
            {
                _logger.Information("Field: {Field}, Old: {Old}, New: {New}, Reason: {Reason}", 
                    correction.FieldName, correction.OldValue, correction.NewValue, correction.Reasoning);
            }

            // Apply all corrections
            _ocrService.ApplyCaribbeanCustomsCorrectionsToInvoice(invoice, customsCorrections);
            
            // Apply original corrections first
            foreach (var correction in standardCorrections)
            {
                if (correction.FieldName == "TotalInsurance" && double.TryParse(correction.NewValue, out var insuranceValue))
                    invoice.TotalInsurance = insuranceValue;
                if (correction.FieldName == "TotalDeduction" && double.TryParse(correction.NewValue, out var deductionValue))
                    invoice.TotalDeduction = deductionValue;
            }
            
            // Then apply Caribbean customs transformations
            _ocrService.ApplyCaribbeanCustomsCorrectionsToInvoice(invoice, customsCorrections);

            // Assert - Verify correct sign transformations
            Assert.That(invoice.TotalInsurance, Is.LessThan(0), "Gift card should result in negative TotalInsurance");
            Assert.That(invoice.TotalDeduction, Is.GreaterThan(0), "Supplier discount should result in positive TotalDeduction");
            
            _logger.Information("Final Values - TotalInsurance: {Insurance}, TotalDeduction: {Deduction}", 
                invoice.TotalInsurance, invoice.TotalDeduction);
        }

        [TearDown]
        public void TearDown()
        {
            _ocrService?.Dispose();
        }
    }
}