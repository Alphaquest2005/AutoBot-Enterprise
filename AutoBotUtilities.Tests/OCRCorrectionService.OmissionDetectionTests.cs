using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using WaterNut.DataSpace;
using EntryDataDS.Business.Entities;
using static AutoBotUtilities.Tests.TestHelpers;

namespace AutoBotUtilities.Tests.Production
{
    /// <summary>
    /// Tests for omission detection functionality in OCR Correction Service
    /// </summary>
    public partial class OCRCorrectionService_ProductionTests
    {
        [Test]
        public async Task DetectDedicatedFieldOmissionsAsync_MissingFields_ShouldDetectOmissions()
        {
            // Arrange
            var invoice = new ShipmentInvoice
            {
                InvoiceNo = "OMIT-001",
                InvoiceTotal = 100.00
                // Missing SubTotal and TotalOtherCost
            };
            var fileText = "Invoice #OMIT-001\nSubTotal: 90.00\nTax: 10.00\nTotal: 100.00";

            // The method being tested requires metadata about what's already been extracted.
            var metadata = InvokePrivateMethod<Dictionary<string, OCRFieldMetadata>>(_service, "ExtractFullOCRMetadata", invoice, fileText);

            // Act - Call the correct private method using reflection and provide all required arguments.
            var omissions = await InvokePrivateMethod<Task<List<InvoiceError>>>(_service, "DetectDedicatedFieldOmissionsAsync", invoice, fileText, metadata);

            // Assert
            Assert.That(omissions, Is.Not.Null);
            Assert.That(omissions.Count, Is.GreaterThanOrEqualTo(2), "Should detect at least two omissions");
            Assert.That(omissions.Any(e => e.Field == "SubTotal" && e.CorrectValue == "90.00"), Is.True, "Should detect SubTotal omission");
            // The system maps "Tax" to the canonical field "TotalOtherCost". The assertion should check for the canonical name.
            Assert.That(omissions.Any(e => e.Field == "TotalOtherCost" && e.CorrectValue == "10.00"), Is.True, "Should detect TotalOtherCost omission");
            Assert.That(omissions.All(e => e.ErrorType == "omission"), Is.True, "All should be identified as omissions");
        }
    }
}