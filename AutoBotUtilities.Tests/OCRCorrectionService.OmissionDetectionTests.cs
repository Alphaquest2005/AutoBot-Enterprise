using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using WaterNut.DataSpace;
using EntryDataDS.Business.Entities;

namespace AutoBotUtilities.Tests.Production
{
    /// <summary>
    /// Tests for omission detection functionality in OCR Correction Service
    /// </summary>
    public partial class OCRCorrectionService_ProductionTests
    {
        [Test]
        public async Task DetectOmittedFieldsAsync_MissingFields_ShouldDetectOmissions()
        {
            // Arrange
            var invoice = new ShipmentInvoice {
                InvoiceNo = "OMIT-001",
                InvoiceTotal = 100.00
                // Missing SubTotal and TotalTax
            };
            var fileText = "Invoice #OMIT-001\nSubTotal: 90.00\nTax: 10.00\nTotal: 100.00";

            // Act - Call the private method using reflection
            var omissions = await (Task<List<InvoiceError>>)InvokePrivateMethod<Task<List<InvoiceError>>>(_service, "DetectOmittedFieldsAsync", invoice, fileText);

            // Assert
            Assert.That(omissions, Is.Not.Null);
            Assert.That(omissions.Count, Is.GreaterThanOrEqualTo(2), "Should detect at least two omissions");
            Assert.That(omissions.Any(e => e.Field == "SubTotal" && e.CorrectValue == "90.00"), Is.True, "Should detect SubTotal omission");
            Assert.That(omissions.Any(e => e.Field == "TotalOtherCost" && e.CorrectValue == "10.00"), Is.True, "Should detect TotalOtherCost omission");
            Assert.That(omissions.All(e => e.ErrorType == "omission"), Is.True, "All should be identified as omissions");
        }
    }
}