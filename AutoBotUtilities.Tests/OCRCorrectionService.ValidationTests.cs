using System;
using System.Collections.Generic;
using System.Linq;
using NUnit.Framework;
using Serilog;
using EntryDataDS.Business.Entities; // For ShipmentInvoice
using WaterNut.DataSpace; // For OCRCorrectionService and its models
using static AutoBotUtilities.Tests.TestHelpers; // For InvokePrivateMethod

namespace AutoBotUtilities.Tests.Production
{
    using OCRCorrectionService = WaterNut.DataSpace.OCRCorrectionService;

    [TestFixture]
    [Category("ValidationLogic")] // Category for OCRValidation specific logic
    public class OCRCorrectionService_ValidationTests
    {
        private ILogger _logger;
        private OCRCorrectionService _service;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Console().CreateLogger();
            _logger.Information("=== Starting Internal Validation Logic Tests (OCRValidation.cs part) ===");
        }

        [SetUp]
        public void SetUp()
        {
            _service = new OCRCorrectionService(_logger);
            _logger.Information("Test Setup for: {TestName}", TestContext.CurrentContext.Test.Name);
        }

        #region ValidateMathematicalConsistency (already in ErrorDetectionTests, but keep core here)
        [Test]
        public void ValidateMathematicalConsistency_LineItemCalculation_Correct()
        {
            var invoice = new ShipmentInvoice
            {
                InvoiceDetails = new List<InvoiceDetails> {
                    new InvoiceDetails { Quantity = 2, Cost = 10.50, Discount = 1.00, TotalCost = 20.00 } // 2*10.50 - 1 = 21-1=20
                }
            };
            var errors = InvokePrivateMethod<List<InvoiceError>>(_service, "ValidateMathematicalConsistency", invoice);
            Assert.That(errors, Is.Empty);
            _logger.Information("✓ ValidateMathematicalConsistency: Correct line item calculation found no errors.");
        }

        [Test]
        public void ValidateMathematicalConsistency_LineItemCalculation_Incorrect()
        {
            var invoice = new ShipmentInvoice
            {
                InvoiceDetails = new List<InvoiceDetails> {
                    new InvoiceDetails { LineNumber=1, Quantity = 3, Cost = 5.00, Discount = 0.50, TotalCost = 14.00 } // Expected 3*5 - 0.5 = 14.50
                }
            };
            var errors = InvokePrivateMethod<List<InvoiceError>>(_service, "ValidateMathematicalConsistency", invoice);
            Assert.That(errors.Count, Is.EqualTo(1));
            Assert.That(errors[0].ErrorType, Is.EqualTo("calculation_error"));
            Assert.That(errors[0].Field, Is.EqualTo("InvoiceDetail_Line1_TotalCost"));
            Assert.That(decimal.Parse(errors[0].CorrectValue), Is.EqualTo(14.50m));
            _logger.Information("✓ ValidateMathematicalConsistency: Incorrect line item calculation detected error.");
        }
        #endregion

        #region ValidateCrossFieldConsistency (already in ErrorDetectionTests, but keep core here)
        [Test]
        public void ValidateCrossFieldConsistency_HeaderTotals_Correct()
        {
            var invoice = new ShipmentInvoice
            {
                SubTotal = 100.00,
                TotalInternalFreight = 10.00,
                TotalOtherCost = 5.00,
                TotalInsurance = 2.00,
                TotalDeduction = 7.00,
                InvoiceTotal = 110.00, // 100+10+5+2-7 = 110
                InvoiceDetails = new List<InvoiceDetails> { new InvoiceDetails { TotalCost = 100.00 } } // SubTotal matches details
            };
            var errors = InvokePrivateMethod<List<InvoiceError>>(_service, "ValidateCrossFieldConsistency", invoice);
            Assert.That(errors, Is.Empty);
            _logger.Information("✓ ValidateCrossFieldConsistency: Correct header totals found no errors.");
        }

        [Test]
        public void ValidateCrossFieldConsistency_HeaderInvoiceTotal_Incorrect()
        {
            var invoice = new ShipmentInvoice
            {
                SubTotal = 100.00,
                TotalInternalFreight = 10.00,
                TotalDeduction = 0.00,
                InvoiceTotal = 105.00, // Expected 100+10 = 110
                InvoiceDetails = new List<InvoiceDetails> { new InvoiceDetails { TotalCost = 100.00 } }
            };
            var errors = InvokePrivateMethod<List<InvoiceError>>(_service, "ValidateCrossFieldConsistency", invoice);
            Assert.That(errors.Count(e => e.Field == "InvoiceTotal" && e.ErrorType == "invoice_total_mismatch"), Is.EqualTo(1));
            var totalError = errors.First(e => e.Field == "InvoiceTotal");
            Assert.That(decimal.Parse(totalError.CorrectValue), Is.EqualTo(110.00m));
            _logger.Information("✓ ValidateCrossFieldConsistency: Incorrect header InvoiceTotal detected error.");
        }

        [Test]
        public void ValidateCrossFieldConsistency_HeaderSubTotal_Incorrect()
        {
            var invoice = new ShipmentInvoice
            {
                SubTotal = 90.00, // Should be 80 from details
                InvoiceTotal = 90.00, // Balanced with reported subtotal for this test
                InvoiceDetails = new List<InvoiceDetails>{
                    new InvoiceDetails { TotalCost = 50.00}, new InvoiceDetails { TotalCost = 30.00}
                }
            };
            var errors = InvokePrivateMethod<List<InvoiceError>>(_service, "ValidateCrossFieldConsistency", invoice);
            Assert.That(errors.Count(e => e.Field == "SubTotal" && e.ErrorType == "subtotal_mismatch"), Is.EqualTo(1));
            var subTotalError = errors.First(e => e.Field == "SubTotal");
            Assert.That(decimal.Parse(subTotalError.CorrectValue), Is.EqualTo(80.00m));
            _logger.Information("✓ ValidateCrossFieldConsistency: Incorrect header SubTotal detected error.");
        }
        #endregion

        #region ResolveFieldConflicts (already in ErrorDetectionTests, but core logic is here)
        [Test]
        public void ResolveFieldConflicts_PicksHighestConfidence()
        {
            var originalInvoice = new ShipmentInvoice { InvoiceTotal = 100 };
            var errors = new List<InvoiceError> {
                new InvoiceError { Field = "InvoiceTotal", CorrectValue = "101", Confidence = 0.8 },
                new InvoiceError { Field = "InvoiceTotal", CorrectValue = "102", Confidence = 0.9 } // Higher confidence
            };
            var resolved = InvokePrivateMethod<List<InvoiceError>>(_service, "ResolveFieldConflicts", errors, originalInvoice);
            Assert.That(resolved.Count, Is.EqualTo(1));
            Assert.That(resolved[0].CorrectValue, Is.EqualTo("102"));
            _logger.Information("✓ ResolveFieldConflicts: Picked highest confidence error.");
        }

        [Test]
        public void ResolveFieldConflicts_DiscardsConflictThatUnbalances()
        {
            var originalInvoice = new ShipmentInvoice { SubTotal = 50, InvoiceTotal = 50 }; // Balanced
            var errors = new List<InvoiceError> {
                new InvoiceError { Field = "SubTotal", CorrectValue = "60", Confidence = 0.9 } // This would unbalance
            };
            var resolved = InvokePrivateMethod<List<InvoiceError>>(_service, "ResolveFieldConflicts", errors, originalInvoice);
            Assert.That(resolved, Is.Empty); // Should be discarded
            _logger.Information("✓ ResolveFieldConflicts: Discarded error that unbalances an initially balanced invoice.");
        }

        [Test]
        public void ResolveFieldConflicts_KeepsConflictIfImprovesOrMaintainsBalance()
        {
            var originalInvoice = new ShipmentInvoice { SubTotal = 50, InvoiceTotal = 55 }; // Unbalanced by 5
            var errors = new List<InvoiceError> {
                // This error, if applied, makes InvoiceTotal = 50, which would balance it.
                new InvoiceError { Field = "InvoiceTotal", CorrectValue = "50", Confidence = 0.9, ErrorType = "value_error" }
            };
            var resolved = InvokePrivateMethod<List<InvoiceError>>(_service, "ResolveFieldConflicts", errors, originalInvoice);
            Assert.That(resolved.Count, Is.EqualTo(1));
            Assert.That(resolved[0].CorrectValue, Is.EqualTo("50"));
            _logger.Information("✓ ResolveFieldConflicts: Kept error that improves balance.");
        }
        #endregion

        [Test]
        public void ValidateMathematicalConsistency_UnbalancedInvoice_ShouldReturnErrors()
        {
            // Arrange
            var invoice = new ShipmentInvoice { 
                InvoiceNo = "MATH-001", 
                SubTotal = 100.00, 
                TotalOtherCost = 10.00, // Tax is now OtherCost, so this is the same as TotalTax in previous tests. taxes are found on the invoices but for shipment invoice its appended to TotalOtherCost.
                InvoiceTotal = 115.00 // Should be 110.00
            };
            
            // Act - Call the actual method
            var errors = InvokePrivateMethod<List<InvoiceError>>(_service, "ValidateMathematicalConsistency", invoice);
            
            // Assert
            Assert.That(errors, Is.Not.Empty, "Should detect mathematical inconsistency");
            Assert.That(errors.First().Field, Is.EqualTo("InvoiceTotal"), "Should identify InvoiceTotal as the problematic field");
            Assert.That(double.Parse(errors.First().CorrectValue), Is.EqualTo(110.00).Within(0.001), "Should suggest correct value");
            Assert.That(errors.First().ErrorType, Is.EqualTo("value_error"), "Should identify as a value error");
        }

        [Test]
        public void ValidateCrossFieldConsistency_InconsistentDates_ShouldReturnErrors()
        {
            // Arrange - Test with a future invoice date which is logically inconsistent
            var invoice = new ShipmentInvoice {
                InvoiceNo = "DATE-001",
                InvoiceDate = DateTime.Now.AddDays(30) // Invoice date in the future is logically inconsistent
            };

            // Act - Call the private method using helper
            var errors = InvokePrivateMethod<List<InvoiceError>>(_service, "ValidateCrossFieldConsistency", invoice);

            // Assert
            Assert.That(errors, Is.Not.Empty, "Should detect date inconsistency");
            Assert.That(errors.Any(e => e.Field == "InvoiceDate"), Is.True, "Should identify InvoiceDate as problematic");
            Assert.That(errors.First().ErrorType, Is.EqualTo("logical_error"), "Should identify as a logical error");
        }
    }
}
