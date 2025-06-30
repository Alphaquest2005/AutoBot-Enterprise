using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Serilog;
using EntryDataDS.Business.Entities;
using WaterNut.DataSpace; // For OCRCorrectionService, InvoiceError, CorrectionResult, OCRFieldMetadata
using TrackableEntities; // For TrackingState
using static AutoBotUtilities.Tests.TestHelpers;

namespace AutoBotUtilities.Tests.Production
{
    using OCRCorrectionService = WaterNut.DataSpace.OCRCorrectionService;

    [TestFixture]
    [Category("CorrectionApplication")]
    public class OCRCorrectionService_CorrectionApplicationTests
    {
        private ILogger _logger;
        private OCRCorrectionService _service;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Console().CreateLogger();
            _logger.Information("=== Starting Correction Application Tests ===");
        }

        [SetUp]
        public void SetUp()
        {
            _service = new OCRCorrectionService(_logger);
            _logger.Information("Test Setup for: {TestName}", TestContext.CurrentContext.Test.Name);
        }

        #region ApplyFieldCorrection Tests (from previous response, still valid)
        [Test]
        public void ApplyFieldCorrection_HeaderField_ShouldUpdateInvoiceProperty()
        {
            var invoice = new ShipmentInvoice { InvoiceNo = "OLD-NO", InvoiceTotal = 100.0 };
            _service.ApplyFieldCorrection(invoice, "InvoiceNo", "NEW-NO");
            _service.ApplyFieldCorrection(invoice, "InvoiceTotal", 150.50m);
            Assert.That(invoice.InvoiceNo, Is.EqualTo("NEW-NO"));
            Assert.That(invoice.InvoiceTotal, Is.EqualTo(150.50));
        }

        [Test]
        public void ApplyFieldCorrection_InvoiceDetailField_ShouldUpdateDetailPropertyAndRecalculate()
        {
            var invoice = new ShipmentInvoice { InvoiceDetails = new List<InvoiceDetails> { new InvoiceDetails { LineNumber = 1, Quantity = 1, Cost = 10, TotalCost = 10 } } };
            _service.ApplyFieldCorrection(invoice, "InvoiceDetail_Line1_Quantity", 2.0);
            _service.ApplyFieldCorrection(invoice, "InvoiceDetail_Line1_Cost", 12.50);
            Assert.That(invoice.InvoiceDetails[0].Quantity, Is.EqualTo(2.0));
            Assert.That(invoice.InvoiceDetails[0].Cost, Is.EqualTo(12.50));
            Assert.That(invoice.InvoiceDetails[0].TotalCost, Is.EqualTo(25.00), "Line TotalCost should be recalculated");
        }
        #endregion

        #region ApplyCorrectionsAsync Tests (from previous response, enhanced for clarity)
        [Test]
        public async Task ApplyCorrectionsAsync_ValueAndFormatErrors_ShouldApplyAndReturnResults()
        {
            // Arrange
            var invoice = new ShipmentInvoice { 
                InvoiceNo = "APP-001", 
                InvoiceTotal = 123.00, 
                SubTotal = 100.00 
            };
            var fileText = "Total: 123.45\nSubTotal: 100,00";
            var metadata = new Dictionary<string, OCRFieldMetadata>();
            var errors = new List<InvoiceError> {
                new InvoiceError { Field = "InvoiceTotal", CorrectValue = "123.45", ErrorType = "value_error"},
                new InvoiceError { Field = "SubTotal", CorrectValue = "100.00", ErrorType = "format_error"}
            };
            
            // Act - Call the private method using reflection
            var results = await InvokePrivateMethod<Task<List<CorrectionResult>>>(this._service, "ApplyCorrectionsAsync", invoice, errors, fileText, metadata).ConfigureAwait(false);
            
            // Assert
            Assert.That(results, Is.Not.Null);
            Assert.That(results.Count, Is.EqualTo(2), "Should return two correction results");
            Assert.That(results.All(r => r.Success), Is.True, "All corrections should be successful");
            
            // Check that the invoice was actually updated
            Assert.That(invoice.InvoiceTotal, Is.EqualTo(123.45).Within(0.01));
            Assert.That(invoice.SubTotal, Is.EqualTo(100.00).Within(0.01));
            Assert.That(invoice.TrackingState, Is.EqualTo(TrackingState.Modified));
        }

        [Test]
        public async Task ApplyCorrectionsAsync_OmissionErrorForHeader_ShouldApplyToMemoryAndMarkForDB()
        {
            var invoice = new ShipmentInvoice { InvoiceNo = "APP-OMIT-001", SubTotal = 50.00 }; // TotalDeduction is null
            var fileText = "Gift Card: 5.00";
            var metadata = InvokePrivateMethod<Dictionary<string, OCRFieldMetadata>>(_service, "ExtractFullOCRMetadata", invoice, fileText);
            var errors = new List<InvoiceError> {
                new InvoiceError { Field = "TotalDeduction", CorrectValue = "5.00", ErrorType = "omission"}
            };
            var results = await InvokePrivateMethod<Task<List<CorrectionResult>>>(this._service, "ApplyCorrectionsAsync", invoice, errors, fileText, metadata).ConfigureAwait(false);
            Assert.That(results.Count, Is.EqualTo(1));
            var cr = results.First();
            Assert.That(cr.Success, Is.True, "CorrectionResult Success should be true for DB processing.");
            Assert.That(invoice.TotalDeduction, Is.EqualTo(5.00), "Omitted value should be applied to in-memory invoice.");
            Assert.That(invoice.TrackingState, Is.EqualTo(TrackingState.Modified));
        }

        [Test]
        public async Task ApplyCorrectionsAsync_OmittedLineItem_ShouldMarkForDB_NoMemoryApply()
        {
            // Omitted_line_item type currently doesn't attempt to add to in-memory InvoiceDetails
            // but should still produce a CorrectionResult for DB pattern learning.
            var invoice = new ShipmentInvoice { InvoiceNo = "APP-OMIT-LINE-001" };
            var fileText = "Item XYZ, Qty 1, Price 10.00"; // This line was missed
            var metadata = new Dictionary<string, OCRFieldMetadata>();
            var errors = new List<InvoiceError> {
                new InvoiceError { Field = "InvoiceDetail_Line5_OmittedLineItem", CorrectValue = "Item XYZ, Qty 1...", ErrorType = "omitted_line_item"}
            };
            var results = await InvokePrivateMethod<Task<List<CorrectionResult>>>(this._service, "ApplyCorrectionsAsync", invoice, errors, fileText, metadata).ConfigureAwait(false);
            Assert.That(results.Count, Is.EqualTo(1));
            var cr = results.First();
            Assert.That(cr.Success, Is.True, "OmittedLineItem CorrectionResult Success should be true for DB pattern learning.");
            Assert.That(invoice.InvoiceDetails == null || !invoice.InvoiceDetails.Any(), Is.True, "In-memory invoice details should not be modified for OmittedLineItem by default.");
            Assert.That(invoice.TrackingState, Is.Not.EqualTo(TrackingState.Modified), "Invoice should not be marked modified if only DB pattern learning for omitted line item occurred.");
        }

        #endregion

        #region RecalculateDependentFields Tests (from previous response, still valid)
        [Test]
        public void RecalculateDependentFields_LineItemChange_ShouldUpdateTotals()
        {
            var invoice = new ShipmentInvoice { SubTotal = 100, InvoiceTotal = 100, InvoiceDetails = new List<InvoiceDetails> { new InvoiceDetails { Quantity = 1, Cost = 50, TotalCost = 50 }, new InvoiceDetails { Quantity = 1, Cost = 50, TotalCost = 50 } } };
            invoice.InvoiceDetails[0].Quantity = 2; // TotalCost becomes 100
            InvokePrivateMethod<object>(_service, "RecalculateDetailTotal", invoice.InvoiceDetails[0]); // Manually trigger detail recalc
            InvokePrivateMethod<object>(_service, "RecalculateDependentFields", invoice, new HashSet<string>());
            Assert.That(invoice.SubTotal, Is.EqualTo(150.0)); // 100 + 50
            Assert.That(invoice.InvoiceTotal, Is.EqualTo(150.0));
        }
        #endregion

        #region ProcessOmissionCorrectionAndApplyToInvoiceAsync (Private method helper test)
        [Test]
        public async Task ProcessOmissionCorrectionAndApplyToInvoiceAsync_HeaderField_ShouldApplyAndReturnResult()
        {
            var invoice = new ShipmentInvoice { InvoiceNo = "OMIT-PROC-001" }; // TotalDeduction is null
            var error = new InvoiceError { Field = "TotalDeduction", CorrectValue = "10.99", ErrorType = "omission" };
            var metadata = new Dictionary<string, OCRFieldMetadata>();
            var fileText = "Deduction: 10.99";

            var result = await InvokePrivateMethod<Task<CorrectionResult>>(this._service, "ProcessOmissionCorrectionAndApplyToInvoiceAsync", invoice, error, metadata, fileText).ConfigureAwait(false);

            Assert.That(result.Success, Is.True);
            Assert.That(result.FieldName, Is.EqualTo("TotalDeduction"));
            Assert.That(result.NewValue, Is.EqualTo("10.99"));
            Assert.That(result.CorrectionType, Is.EqualTo("omission")); // Or "omission_db_only" if not applied to memory
            Assert.That(invoice.TotalDeduction, Is.EqualTo(10.99), "Value should be applied to in-memory invoice.");
        }
        #endregion

        #region ApplySingleValueOrFormatCorrectionToInvoiceAsync (Private method helper test)
        [Test]
        public async Task ApplySingleValueOrFormatCorrectionToInvoiceAsync_ShouldApplyAndReturnResult()
        {
            var invoice = new ShipmentInvoice { InvoiceTotal = 100.00 };
            var error = new InvoiceError { Field = "InvoiceTotal", ExtractedValue = "100.00", CorrectValue = "100.55", ErrorType = "value_error" };

            var result = await InvokePrivateMethod<Task<CorrectionResult>>(this._service, "ApplySingleValueOrFormatCorrectionToInvoiceAsync", invoice, error).ConfigureAwait(false);

            Assert.That(result.Success, Is.True);
            Assert.That(result.FieldName, Is.EqualTo("InvoiceTotal"));
            Assert.That(result.NewValue, Is.EqualTo("100.55"));
            Assert.That(invoice.InvoiceTotal, Is.EqualTo(100.55));
        }
        #endregion
    }
}
