using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;
using NUnit.Framework;
using Serilog;
using EntryDataDS.Business.Entities;
using WaterNut.DataSpace; // For OCRCorrectionService, InvoiceError, OCRFieldMetadata
using static AutoBotUtilities.Tests.TestHelpers;
// using Moq; // Would be used if IDeepSeekInvoiceApi was injectable

namespace AutoBotUtilities.Tests.Production
{
    using OCRCorrectionService = WaterNut.DataSpace.OCRCorrectionService;

    [TestFixture]
    [Category("ErrorDetection")]
    public class OCRCorrectionService_ErrorDetectionTests
    {
        private ILogger _logger;
        // private Mock<IDeepSeekInvoiceApi> _mockDeepSeekApi; 
        private OCRCorrectionService _service;

        private OCRCorrectionService CreateServiceWithPotentiallyMockedApi()
        {
            // For these tests, we are primarily testing logic that processes DeepSeek responses,
            // or internal validation logic. Direct calls to DeepSeek for error detection are
            // harder to unit test without DI.
            return new OCRCorrectionService(_logger); 
        }

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Console().CreateLogger();
            _logger.Information("=== Starting Error Detection Tests ===");
        }

        [SetUp]
        public void SetUp()
        {
            // _mockDeepSeekApi = new Mock<IDeepSeekInvoiceApi>();
            _service = CreateServiceWithPotentiallyMockedApi();
             _logger.Information("Test Setup for: {TestName}", TestContext.CurrentContext.Test.Name);
        }

        #region DetectInvoiceErrorsAsync Orchestration Tests (Conceptual/Simulated DeepSeek)
        
        [Test]
        public async Task DetectInvoiceErrorsAsync_CombinesInternalValidationsWithDeepSeek()
        {
            // Arrange
            var invoice = new ShipmentInvoice { 
                InvoiceNo = "ERR-TEST-001", 
                InvoiceTotal = 100.00, // Correct according to text below
                SubTotal = 90.00,      // Incorrect, text has 80
                InvoiceDetails = new List<InvoiceDetails> {
                    new InvoiceDetails { LineNumber = 1, Quantity = 2, Cost = 40, TotalCost = 75 } // Math error: 2*40=80, not 75
                }
            };
            // Text implies SubTotal is 80 (from line item), and InvoiceTotal should be 80.
            var fileText = "InvoiceNo: ERR-TEST-001\nItem: Product X Qty:2 Price:40 Total:75\nSubTotal: $80.00\nTotal: $100.00"; 
            var metadata = InvokePrivateMethod<Dictionary<string, OCRFieldMetadata>>(_service, "ExtractFullOCRMetadata", invoice, fileText); // Use instance method

            // Simulate DeepSeek finding a format error for SubTotal, but agreeing with InvoiceTotal based on its (wrong) calculation
            // To truly test DetectInvoiceErrorsAsync, we'd mock the internal calls to DetectHeader/Product errors.
            // For now, we simulate what it *would* return from a DeepSeek call processing part
            // This is not a true unit test of DetectInvoiceErrorsAsync's orchestration but rather components it uses.
            
            // This part simulates what DetectHeaderFieldErrorsAndOmissionsAsync would do internally after a DeepSeek call.
            string simulatedDeepSeekHeaderResponse = @"{
                ""errors"": [
                    {
                        ""field"": ""SubTotal"", ""extracted_value"": ""90.00"", ""correct_value"": ""80.00"", 
                        ""line_text"": ""SubTotal: $80.00"", ""line_number"": 3, ""error_type"": ""value_error"", 
                        ""confidence"": 0.92, ""reasoning"": ""Text shows 80.00 for SubTotal""
                    }
                ]
            }";
            var deepSeekHeaderCorrections = InvokePrivateMethod<List<CorrectionResult>>(_service, "ProcessDeepSeekCorrectionResponse", simulatedDeepSeekHeaderResponse, fileText);
            var deepSeekHeaderErrors = deepSeekHeaderCorrections.Select(cr => InvokePrivateMethod<InvoiceError>(_service, "ConvertCorrectionResultToInvoiceError", cr)).ToList();
            // Assuming product detection finds no errors from DeepSeek for this test.

            // Act - We are manually combining errors as DetectInvoiceErrorsAsync itself makes live calls.
            var combinedErrors = new List<InvoiceError>();
            combinedErrors.AddRange(deepSeekHeaderErrors); // Simulated DeepSeek errors
            combinedErrors.AddRange(InvokePrivateMethod<List<InvoiceError>>(_service, "ValidateMathematicalConsistency", invoice)); // Internal validation
            combinedErrors.AddRange(InvokePrivateMethod<List<InvoiceError>>(_service, "ValidateCrossFieldConsistency", invoice));   // Internal validation

            var finalResolvedErrors = InvokePrivateMethod<List<InvoiceError>>(_service, "ResolveFieldConflicts", combinedErrors, invoice);


            // Assert
            Assert.That(finalResolvedErrors, Is.Not.Empty);

            var subTotalValueError = finalResolvedErrors.FirstOrDefault(e => e.Field == "SubTotal" && e.ErrorType == "value_error");
            Assert.That(subTotalValueError, Is.Not.Null, "Should have value_error for SubTotal from simulated DeepSeek.");
            Assert.That(subTotalValueError.CorrectValue, Is.EqualTo("80.00"));

            var lineItemMathError = finalResolvedErrors.FirstOrDefault(e => e.Field == "InvoiceDetail_Line1_TotalCost" && e.ErrorType == "calculation_error");
            Assert.That(lineItemMathError, Is.Not.Null, "Should have calculation_error for line item.");
            Assert.That(decimal.Parse(lineItemMathError.CorrectValue), Is.EqualTo(80.00m)); // 2 * 40

            // After SubTotal is hypothetically corrected to 80 (from DeepSeek) and Line TotalCost to 80 (from math validation)
            // Original Invoice: SubTotal=90, InvoiceTotal=100, Details.Sum=75
            // Corrected Values for Test: SubTotal=80, InvoiceTotal=100 (original), Details.Sum=80 (corrected)
            // ValidateCrossFieldConsistency on original invoice: SubTotal mismatch (90 vs 75), InvoiceTotal mismatch (100 vs (90-0)=90)
            // ResolveFieldConflicts will take the proposed corrections (SubTotal=80, LineTotal=80)
            // and check their impact on a clone.
            // Clone with these: SubTotal=80, InvoiceTotal=100, Details.Sum=80.
            // TotalsZero on clone: (SubTotal 80) - (Deductions 0) = 80. Reported InvoiceTotal=100. Still a mismatch.
            // So, an InvoiceTotalMismatch error should persist or be generated based on *corrected* components.
            
            var invoiceTotalMismatch = finalResolvedErrors.FirstOrDefault(e => e.Field == "InvoiceTotal" && e.ErrorType == "invoice_total_mismatch");
            Assert.That(invoiceTotalMismatch, Is.Not.Null, "Should retain or generate invoice_total_mismatch after considering other corrections.");
            // Expected InvoiceTotal = Corrected SubTotal (80) + Freight (0) + Other (0) + Insurance (0) - Deduction (0) = 80
            Assert.That(decimal.Parse(invoiceTotalMismatch.CorrectValue), Is.EqualTo(80.00m));


            _logger.Information("✓ DetectInvoiceErrorsAsync conceptual test with combined and resolved errors passed.");
        }
        #endregion

        // ... ValidateMathematicalConsistency Tests (from previous response, largely okay) ...
        #region ValidateMathematicalConsistency Tests
        [Test]
        public void ValidateMathematicalConsistency_CorrectLineItems_ShouldReturnNoErrors()
        {
            var invoice = new ShipmentInvoice { InvoiceDetails = new List<InvoiceDetails> { new InvoiceDetails { LineNumber = 1, Quantity = 2, Cost = 10, Discount = 1, TotalCost = 19 }}};
            Assert.That(InvokePrivateMethod<List<InvoiceError>>(_service, "ValidateMathematicalConsistency", invoice), Is.Empty);
        }
        [Test]
        public void ValidateMathematicalConsistency_IncorrectLineTotal_ShouldReturnError()
        {
            var invoice = new ShipmentInvoice { InvoiceDetails = new List<InvoiceDetails> { new InvoiceDetails { LineNumber = 1, Quantity = 2, Cost = 10, TotalCost = 19.90 }}}; // Expected 20
            var errors = InvokePrivateMethod<List<InvoiceError>>(_service, "ValidateMathematicalConsistency", invoice);
            Assert.That(errors.Any(e => e.Field == "InvoiceDetail_Line1_TotalCost" && e.ErrorType == "calculation_error"), Is.True);
        }
        #endregion

        // ... ValidateCrossFieldConsistency Tests (from previous response, largely okay) ...
        #region ValidateCrossFieldConsistency Tests
        [Test]
        public void ValidateCrossFieldConsistency_BalancedInvoice_ShouldReturnNoErrors()
        {
            var invoice = new ShipmentInvoice { SubTotal = 100, InvoiceTotal = 100, InvoiceDetails = new List<InvoiceDetails> { new InvoiceDetails { TotalCost = 100 }}};
            Assert.That(InvokePrivateMethod<List<InvoiceError>>(_service, "ValidateCrossFieldConsistency", invoice), Is.Empty);
        }
        [Test]
        public void ValidateCrossFieldConsistency_InvoiceTotalMismatch_ShouldReturnError()
        {
            var invoice = new ShipmentInvoice { SubTotal = 100, InvoiceTotal = 90, InvoiceDetails = new List<InvoiceDetails> { new InvoiceDetails { TotalCost = 100 }}}; // Expected 100
            var errors = InvokePrivateMethod<List<InvoiceError>>(_service, "ValidateCrossFieldConsistency", invoice);
            Assert.That(errors.Any(e => e.Field == "InvoiceTotal" && e.ErrorType == "invoice_total_mismatch"), Is.True);
        }
        #endregion
        
        // ... ResolveFieldConflicts Tests (from previous response, largely okay) ...
        #region ResolveFieldConflicts Tests
        [Test]
        public void ResolveFieldConflicts_ShouldPickHighestConfidenceAndValidateMath()
        {
            var originalInvoice = new ShipmentInvoice { InvoiceNo="CFL-001", SubTotal = 100, InvoiceTotal = 100 };
            var proposedErrors = new List<InvoiceError> {
                new InvoiceError { Field = "InvoiceTotal", CorrectValue = "110.00", Confidence = 0.9, ErrorType="value_error" },
                new InvoiceError { Field = "InvoiceTotal", CorrectValue = "105.00", Confidence = 0.8, ErrorType="value_error" },
                new InvoiceError { Field = "SubTotal", CorrectValue = "110.00", Confidence = 0.95, ErrorType="value_error" }
            };
            var resolved = InvokePrivateMethod<List<InvoiceError>>(_service, "ResolveFieldConflicts", proposedErrors, originalInvoice);
            Assert.That(resolved.Count(e=>e.Field=="InvoiceTotal"), Is.EqualTo(1));
            Assert.That(resolved.First(e=>e.Field=="InvoiceTotal").CorrectValue, Is.EqualTo("110.00")); // Higher confidence pick
            Assert.That(resolved.Any(e=>e.Field=="SubTotal"), Is.True);
        }
        #endregion

        #region ConvertCorrectionResultToInvoiceError Test
        [Test]
        public void ConvertCorrectionResultToInvoiceError_ShouldMapAllFields()
        {
            var cr = new CorrectionResult
            {
                FieldName = "TestField", OldValue = "Old", NewValue = "New", CorrectionType = "TestType",
                Confidence = 0.77, Reasoning = "TestReason", LineNumber = 5, LineText = "Test Line Text",
                ContextLinesBefore = new List<string>{"B1"}, ContextLinesAfter = new List<string>{"A1"},
                RequiresMultilineRegex = true
            };

            var ie = InvokePrivateMethod<InvoiceError>(_service, "ConvertCorrectionResultToInvoiceError", cr);

            Assert.That(ie.Field, Is.EqualTo(cr.FieldName));
            Assert.That(ie.ExtractedValue, Is.EqualTo(cr.OldValue));
            Assert.That(ie.CorrectValue, Is.EqualTo(cr.NewValue));
            Assert.That(ie.ErrorType, Is.EqualTo(cr.CorrectionType));
            Assert.That(ie.Confidence, Is.EqualTo(cr.Confidence));
            Assert.That(ie.Reasoning, Is.EqualTo(cr.Reasoning));
            Assert.That(ie.LineNumber, Is.EqualTo(cr.LineNumber));
            Assert.That(ie.LineText, Is.EqualTo(cr.LineText));
            Assert.That(ie.ContextLinesBefore, Is.EquivalentTo(cr.ContextLinesBefore));
            Assert.That(ie.ContextLinesAfter, Is.EquivalentTo(cr.ContextLinesAfter));
            Assert.That(ie.RequiresMultilineRegex, Is.EqualTo(cr.RequiresMultilineRegex));
            _logger.Information("✓ ConvertCorrectionResultToInvoiceError maps fields correctly.");
        }
        #endregion
        
        #region EnrichDetectedErrorWithContext Test
        [Test]
        public void EnrichDetectedErrorWithContext_MissingInfo_ShouldBeEnriched()
        {
            var error = new InvoiceError { Field = "InvoiceNo", LineNumber = 0, LineText = null }; // Missing line info
            var metadata = new Dictionary<string, OCRFieldMetadata> {
                {"InvoiceNo", new OCRFieldMetadata { FieldName="InvoiceNo", LineNumber=3, LineText="Invoice No: ABC" } }
            };
            var fileText = "Line1\nLine2\nInvoice No: ABC\nLine4";

            InvokePrivateMethod<object>(_service, "EnrichDetectedErrorWithContext", error, metadata, fileText);

            Assert.That(error.LineNumber, Is.EqualTo(3));
            Assert.That(error.LineText, Is.EqualTo("Invoice No: ABC"));
            Assert.That(error.ContextLinesBefore.Count, Is.GreaterThan(0)); // Should pick up "Line2", "Line1"
            Assert.That(error.ContextLinesAfter.Count, Is.GreaterThan(0));   // Should pick up "Line4"
            _logger.Information("✓ EnrichDetectedErrorWithContext enriched missing info.");
        }
        #endregion

    }
}
