using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using NUnit.Framework;
using Serilog;
using EntryDataDS.Business.Entities; // For ShipmentInvoice
using WaterNut.DataSpace; // For OCRCorrectionService and its models
using static AutoBotUtilities.Tests.TestHelpers;

namespace AutoBotUtilities.Tests.Production
{
    using OCRCorrectionService = WaterNut.DataSpace.OCRCorrectionService;

    [TestFixture]
    [Category("PromptCreation")]
    public class OCRCorrectionService_PromptCreationTests
    {
        private ILogger _logger;
        private OCRCorrectionService _service;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Console().CreateLogger();
            _logger.Information("=== Starting Prompt Creation Tests ===");
        }

        [SetUp]
        public void SetUp()
        {
            _service = new OCRCorrectionService(_logger);
            _logger.Information("Test Setup for: {TestName}", TestContext.CurrentContext.Test.Name);
        }

        private ShipmentInvoice CreateBasicInvoice()
        {
            return new ShipmentInvoice
            {
                InvoiceNo = "INV-PROMPT-001",
                InvoiceDate = new DateTime(2023, 1, 10),
                InvoiceTotal = 120.50,
                SubTotal = 100.00,
                TotalInternalFreight = 15.00,
                TotalOtherCost = 5.50,
                SupplierName = "Prompt Supplier Ltd.",
                Currency = "USD"
            };
        }

        private string GetSampleFileText()
        {
            return "Invoice # INV-PROMPT-001\nDate: 10/01/2023\nFrom: Prompt Supplier Ltd.\n" +
                   "Item 1: Widget A Qty:2 Price:50.00 Total:100.00\n" +
                   "Subtotal $100.00\nShipping $15.00\nTax $5.50\nTOTAL $120.50 USD";
        }

        [Test]
        public void CreateHeaderErrorDetectionPrompt_GeneratesValidPromptStructure()
        {
            var invoice = CreateBasicInvoice();
            var fileText = GetSampleFileText();

            var prompt = InvokePrivateMethod<string>(_service, "CreateHeaderErrorDetectionPrompt", invoice, fileText);

            Assert.That(prompt, Does.Contain("OCR HEADER FIELD ERROR DETECTION AND OMISSION ANALYSIS:"));
            Assert.That(prompt, Does.Contain("EXTRACTED HEADER DATA:"));
            // Check for some serialized invoice data
            Assert.That(prompt, Does.Contain(invoice.InvoiceNo));
            Assert.That(prompt, Does.Contain(invoice.SupplierName));
            Assert.That(prompt, Does.Contain("ORIGINAL INVOICE TEXT"));
            Assert.That(prompt, Does.Contain(fileText.Substring(0, 50))); // Part of file text
            Assert.That(prompt, Does.Contain("RESPONSE FORMAT (Strict JSON array of error objects under \"errors\" key):"));
            Assert.That(prompt, Does.Contain("\"field\": \"TotalDeduction\"")); // Example from prompt
            _logger.Information("✓ CreateHeaderErrorDetectionPrompt generated valid structure.");
        }

        [Test]
        public void CreateProductErrorDetectionPrompt_GeneratesValidPromptStructure()
        {
            var invoice = CreateBasicInvoice();
            invoice.InvoiceDetails = new List<InvoiceDetails> {
                new InvoiceDetails { LineNumber = 1, ItemDescription = "Widget A", Quantity = 2, Cost = 50, TotalCost = 100 }
            };
            var fileText = GetSampleFileText();

            var prompt = InvokePrivateMethod<string>(_service, "CreateProductErrorDetectionPrompt", invoice, fileText);

            Assert.That(prompt, Does.Contain("OCR PRODUCT LINE ITEM ERROR DETECTION AND OMISSION ANALYSIS:"));
            Assert.That(prompt, Does.Contain("EXTRACTED PRODUCT DATA:"));
            Assert.That(prompt, Does.Contain("\"ItemDescription\": \"Widget A\""));
            Assert.That(prompt, Does.Contain("\"Quantity\": 2"));
            Assert.That(prompt, Does.Contain("ORIGINAL INVOICE TEXT"));
            Assert.That(prompt, Does.Contain(fileText.Substring(0, 50)));
            Assert.That(prompt, Does.Contain("RESPONSE FORMAT (Strict JSON array of error objects under \"errors\" key):"));
            Assert.That(prompt, Does.Contain("\"field\": \"InvoiceDetail_Line15_Quantity\"")); // Example
            _logger.Information("✓ CreateProductErrorDetectionPrompt generated valid structure.");
        }

        [Test]
        public void CreateRegexCreationPrompt_GeneratesValidPromptStructureForOmission()
        {
            var correction = new CorrectionResult
            {
                FieldName = "OmittedTax",
                NewValue = "7.89",
                CorrectionType = "omission",
                LineNumber = 20,
                LineText = "VAT (20%): 7.89",
                RequiresMultilineRegex = false,
                ContextLinesBefore = new List<string> { "L18: Subtotal: 39.45", "L19: Shipping: 5.00" },
                ContextLinesAfter = new List<string> { "L21: Total: 52.34" }
            };
            var lineContext = new LineContext // Simulate a context
            {
                LineNumber = correction.LineNumber,
                LineText = correction.LineText,
                ContextLinesBefore = correction.ContextLinesBefore,
                ContextLinesAfter = correction.ContextLinesAfter,
                RegexPattern = "SomeExistingPatternForLine", // Can be null if new line
                FieldsInLine = new List<FieldInfo> { new FieldInfo { Key = "ExistingField" } }
            };

            var prompt = _service.CreateRegexCreationPrompt(correction, lineContext);

            Assert.That(prompt, Does.Contain("CREATE REGEX PATTERN FOR OCR FIELD EXTRACTION:"));
            Assert.That(prompt, Does.Contain($"Field Name to Capture: \"{correction.FieldName}\""));
            Assert.That(prompt, Does.Contain($"Expected Value: \"{correction.NewValue}\""));
            Assert.That(prompt, Does.Contain($">>> LINE {correction.LineNumber}: {correction.LineText} <<<"));
            // More robust check for actual content:
            Assert.That(prompt, Does.Contain(correction.ContextLinesBefore[0]), "Prompt should contain first 'before' context line content.");
            Assert.That(prompt, Does.Contain(correction.ContextLinesBefore[1]), "Prompt should contain second 'before' context line content.");
            Assert.That(prompt, Does.Contain(correction.ContextLinesAfter[0]), "Prompt should contain first 'after' context line content.");
            Assert.That(prompt, Does.Contain("Existing Regex for this Line (if any): SomeExistingPatternForLine"));
            Assert.That(prompt, Does.Contain("Named Groups Already Captured by Current Regex: ExistingField"));
            Assert.That(prompt, Does.Contain("STRICT JSON RESPONSE FORMAT:"));
            Assert.That(prompt, Does.Contain($"\"regex_pattern\": \"(?<{correction.FieldName}>"));
            _logger.Information("✓ CreateRegexCreationPrompt generated valid structure for omission.");
        }

        [Test]
        public void CreateOmissionDetectionPrompt_GeneratesValidPromptStructure()
        {
            // This prompt is defined in OCRErrorDetection.cs, test its generation.
            var invoice = CreateBasicInvoice();
            invoice.TotalDeduction = 0; // Simulate a case where deduction might be missed
            var fileText = "Some text mentioning Gift Card $10.00 that was missed.";
            var metadata = InvokePrivateMethod<Dictionary<string, OCRFieldMetadata>>(_service, "ExtractFullOCRMetadata", invoice, fileText);

            var prompt = InvokePrivateMethod<string>(_service, "CreateOmissionDetectionPrompt", invoice, fileText, metadata);

            Assert.That(prompt, Does.Contain("OCR OMISSION DETECTION TASK:"));
            Assert.That(prompt, Does.Contain("SUMMARY OF CURRENTLY EXTRACTED DATA"));
            Assert.That(prompt, Does.Contain($"\"TotalDeduction\": {invoice.TotalDeduction}"));
            Assert.That(prompt, Does.Contain("ORIGINAL INVOICE TEXT"));
            Assert.That(prompt, Does.Contain("INSTRUCTIONS FOR IDENTIFYING OMISSIONS:"));
            Assert.That(prompt, Does.Contain("\"error_type\": \"omission\"")); // From example in prompt
            _logger.Information("✓ CreateOmissionDetectionPrompt generated valid structure.");
        }

        [Test]
        public void CreateDirectDataCorrectionPrompt_GeneratesValidPromptStructure()
        {
            var dynamicData = new List<dynamic> {
                new Dictionary<string, object> {
                    {"InvoiceNo", "DIRECT-001"}, {"InvoiceTotal", 105.00}, {"SubTotal", 100.00}
                }
            };
            var fileText = "Total: 100.00\nSubtotal: 100.00"; // Text implies total should be 100

            var prompt = _service.CreateDirectDataCorrectionPrompt(dynamicData, fileText);

            Assert.That(prompt, Does.Contain("DIRECT DATA CORRECTION - MATHEMATICAL CONSISTENCY REQUIRED:"));
            Assert.That(prompt, Does.Contain("EXTRACTED INVOICE DATA"));
            Assert.That(prompt, Does.Contain("\"InvoiceTotal\": 105"));
            Assert.That(prompt, Does.Contain("ORIGINAL INVOICE TEXT"));
            Assert.That(prompt, Does.Contain("MATHEMATICAL VALIDATION RULES:"));
            Assert.That(prompt, Does.Contain("\"field\": \"InvoiceTotal\"")); // From example
            Assert.That(prompt, Does.Contain("\"mathematical_check_after_corrections\""));
            _logger.Information("✓ CreateDirectDataCorrectionPrompt generated valid structure.");
        }


    }
}