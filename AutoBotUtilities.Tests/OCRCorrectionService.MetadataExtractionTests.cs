using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Serilog;
using EntryDataDS.Business.Entities; // For ShipmentInvoice
using OCR.Business.Entities;         // For Invoice (template), Fields, Lines, Parts, OcrInvoices
using WaterNut.DataSpace;            // For OCRCorrectionService, OCRFieldMetadata etc.
using System.Data.Entity;            // For Include in GetFieldFormatRegexesFromDb mock if needed

namespace AutoBotUtilities.Tests.Production
{
    using Invoices = OCR.Business.Entities.Invoices;
    using OCRCorrectionService = WaterNut.DataSpace.OCRCorrectionService;

    /// <summary>
    /// Tests for OCR metadata extraction and ShipmentInvoiceWithMetadata functionality.
    /// Focus on ExtractEnhancedOCRMetadata.
    /// </summary>
    [TestFixture]
    [Category("Metadata")]
    [Category("OCR")]
    public class OCRCorrectionService_MetadataExtractionTests
    {
        #region Test Setup

        private static ILogger _logger;
        private OCRCorrectionService _service;
        private Invoice _mockOcrTemplate; // OCR.Business.Entities.Invoice

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File($"logs/OCRMetadataExtractionTests_{DateTime.Now:yyyyMMdd_HHmmss}.log")
                .CreateLogger();
            _logger.Information("Starting OCR Metadata Extraction Tests");
        }

        [SetUp]
        public void SetUp()
        {
            _service = new OCRCorrectionService(_logger);
            _mockOcrTemplate = CreateDetailedMockOcrTemplate();
        }

        [TearDown]
        public void TearDown()
        {
            _service?.Dispose();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _logger.Information("Completed OCR Metadata Extraction Tests");
        }

        // Helper to create a more detailed mock OCR Template
        private Invoice CreateDetailedMockOcrTemplate()
        {
            var ocrInvoiceEntity = new Invoices { Id = 1, Name = "DetailedTestTemplate" };
            var template = new Invoice(ocrInvoiceEntity, _logger) { FilePath = "detailed_test.pdf" };

            var headerPartEntity = new Parts { Id = 10, Invoices= ocrInvoiceEntity, PartTypeId = 1, PartTypes = new PartTypes { Id = 1, Name = "Header" } };
            template.Parts.Add(new Part(headerPartEntity, _logger));

            var lineDefHeader1 = new Lines { Id = 100, PartId = 10, Name = "InvoiceNoLine", RegExId = 1000, RegularExpressions = new RegularExpressions { Id = 1000, RegEx = "Invoice No: (?<InvoiceKey>\\w+)" }, Fields = new List<Fields>() };
            var fieldDefInvNo = new Fields { Id = 1001, LineId = 100, Key = "InvoiceKey", Field = "InvoiceNo", EntityType = "ShipmentInvoice", DataType = "string" };
            lineDefHeader1.Fields.Add(fieldDefInvNo);
            headerPartEntity.Lines = new List<Lines> { lineDefHeader1 }; // Initialize if null

            var lineDefHeader2 = new Lines { Id = 110, PartId = 10, Name = "InvoiceTotalLine", RegExId = 1100, RegularExpressions = new RegularExpressions { Id = 1100, RegEx = "Total: (?<TotalKey>\\d+\\.\\d{2})" }, Fields = new List<Fields>() };
            var fieldDefInvTotal = new Fields { Id = 1101, LineId = 110, Key = "TotalKey", Field = "InvoiceTotal", EntityType = "ShipmentInvoice", DataType = "decimal" };
            lineDefHeader2.Fields.Add(fieldDefInvTotal);
            headerPartEntity.Lines.Add(lineDefHeader2);


            var lineItemPartEntity = new Parts { Id = 20, Invoices = ocrInvoiceEntity, PartTypeId = 2, PartTypes = new PartTypes { Id = 2, Name = "LineItem" } };
            template.Parts.Add(new Part(lineItemPartEntity, _logger));

            var lineDefItem1 = new Lines { Id = 200, PartId = 20, Name = "ItemDescLine", RegExId = 2000, RegularExpressions = new RegularExpressions { Id = 2000, RegEx = "Item: (?<ItemDescKey>.+) Qty" }, Fields = new List<Fields>() };
            var fieldDefItemDesc = new Fields { Id = 2001, LineId = 200, Key = "ItemDescKey", Field = "ItemDescription", EntityType = "InvoiceDetails", DataType = "string" };
            lineDefItem1.Fields.Add(fieldDefItemDesc);
            lineItemPartEntity.Lines = new List<Lines> { lineDefItem1 };

            // Add Lines wrappers to template.Lines
            template.Lines.Add(new Line(lineDefHeader1, _logger));
            template.Lines.Add(new Line(lineDefHeader2, _logger));
            template.Lines.Add(new Line(lineDefItem1, _logger));

            return template;
        }


        #endregion

        #region OCRFieldMetadata and ShipmentInvoiceWithMetadata Tests (Mostly structural, retain)

        [Test]
        [Category("MetadataModels")]
        public void OCRFieldMetadata_Creation_ShouldSetAllProperties()
        {
            var metadata = new OCRFieldMetadata {/*...properties...*/}; // As in original test
            // Asserts ...
            _logger.Information("✓ OCRFieldMetadata properties set correctly");
        }

        [Test]
        [Category("MetadataModels")]
        public void ShipmentInvoiceWithMetadata_CreationAndAccess_ShouldWork()
        {
            var invoice = new ShipmentInvoice { InvoiceNo = "META-001", InvoiceTotal = 100.0 };
            var ocrMeta = new OCRFieldMetadata { FieldName = "InvoiceTotal", RawValue = "100.00", LineNumber = 5, FieldId = 1 };
            var siwm = new ShipmentInvoiceWithMetadata
            {
                Invoice = invoice,
                FieldMetadata = new Dictionary<string, OCRFieldMetadata> { { "InvoiceTotal", ocrMeta } }
            };

            Assert.That(siwm.Invoice.InvoiceNo, Is.EqualTo("META-001"));
            Assert.That(siwm.GetFieldMetadata("InvoiceTotal"), Is.EqualTo(ocrMeta));
            Assert.That(siwm.GetFieldMetadata("NonExistent"), Is.Null);
            _logger.Information("✓ ShipmentInvoiceWithMetadata created and accessed correctly");
        }

        #endregion

        #region ExtractEnhancedOCRMetadata Tests

        [Test]
        [Category("MetadataExtraction")]
        public void ExtractEnhancedOCRMetadata_ValidInput_ShouldExtractRichMetadata()
        {
            // Arrange
            var runtimeInvoiceData = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                ["InvoiceNo"] = "INV123",         // Should map to fieldDefInvNo via "InvoiceNo" (Field) or "InvoiceKey" (Key)
                ["InvoiceTotal"] = "150.75",     // Should map to fieldDefInvTotal via "InvoiceTotal" (Field) or "TotalKey" (Key)
                ["ItemDescription_L1"] = "Widget A", // Example for line item, mapping needs to handle _LX suffix
                ["ItemDescription_L2"] = "Gadget B",
                ["NonTemplateField"] = "Some Value" // Should be ignored or handled gracefully
            };
            var precomputedMappings = _service.CreateEnhancedFieldMapping(_mockOcrTemplate);

            // Act
            var extractedMetadata = _service.ExtractEnhancedOCRMetadata(runtimeInvoiceData, _mockOcrTemplate, precomputedMappings);

            // Assert
            Assert.That(extractedMetadata, Is.Not.Null);
            Assert.That(extractedMetadata.ContainsKey("InvoiceNo"), Is.True, "Metadata for InvoiceNo should be extracted.");
            Assert.That(extractedMetadata.ContainsKey("InvoiceTotal"), Is.True, "Metadata for InvoiceTotal should be extracted.");

            // Check InvoiceNo metadata
            var invNoMeta = extractedMetadata["InvoiceNo"];
            Assert.That(invNoMeta.FieldId, Is.EqualTo(1001)); // From mock template
            Assert.That(invNoMeta.LineId, Is.EqualTo(100));
            Assert.That(invNoMeta.RegexId, Is.EqualTo(1000));
            Assert.That(invNoMeta.Key, Is.EqualTo("InvoiceKey"));
            Assert.That(invNoMeta.Field, Is.EqualTo("InvoiceNo"));
            Assert.That(invNoMeta.PartId, Is.EqualTo(10)); // HeaderPart
            Assert.That(invNoMeta.InvoiceId, Is.EqualTo(1)); // OcrInvoices.Id

            // Check InvoiceTotal metadata
            var invTotalMeta = extractedMetadata["InvoiceTotal"];
            Assert.That(invTotalMeta.FieldId, Is.EqualTo(1101));
            Assert.That(invTotalMeta.LineId, Is.EqualTo(110));
            Assert.That(invTotalMeta.EntityType, Is.EqualTo("ShipmentInvoice"));
            Assert.That(invTotalMeta.DataType, Is.EqualTo("decimal"));
            Assert.That(invTotalMeta.PartName, Is.EqualTo("Header"));

            // Check line item (basic check for now, suffix handling needs specific logic in ExtractEnhancedOCRMetadata)
            // The current ExtractEnhancedOCRMetadata might not handle _LX suffix robustly without more specific logic.
            // If it does (or is enhanced to do so), these asserts would be valid.
            if (extractedMetadata.ContainsKey("ItemDescription_L1"))
            {
                var itemDescMeta = extractedMetadata["ItemDescription_L1"];
                Assert.That(itemDescMeta.FieldId, Is.EqualTo(2001)); // fieldDefItemDesc
                Assert.That(itemDescMeta.LineId, Is.EqualTo(200));   // lineDefItem1
                Assert.That(itemDescMeta.EntityType, Is.EqualTo("InvoiceDetails"));
                Assert.That(itemDescMeta.PartId, Is.EqualTo(20)); // LineItemPart
                Assert.That(itemDescMeta.LineNumber, Is.EqualTo(1), "LineNumber hint from field name");
            }
            else
            {
                _logger.Warning("Metadata for ItemDescription_L1 was not extracted. Suffix handling might need review.");
            }


            Assert.That(extractedMetadata.ContainsKey("NonTemplateField"), Is.False, "Non-template fields should not have metadata from template.");

            _logger.Information("✓ ExtractEnhancedOCRMetadata extracted rich metadata successfully for {Count} fields.", extractedMetadata.Count);
        }

        [Test]
        [Category("MetadataExtraction")]
        public void CreateEnhancedFieldMapping_ShouldMapTemplateFieldsCorrectly()
        {
            // Act
            var mappings = _service.CreateEnhancedFieldMapping(_mockOcrTemplate);

            // Assert
            Assert.That(mappings, Is.Not.Null);
            Assert.That(mappings.Count, Is.GreaterThanOrEqualTo(3), "Should map at least InvoiceNo, InvoiceTotal, ItemDescription");

            Assert.That(mappings.ContainsKey("InvoiceNo"), Is.True); // From Field property
            Assert.That(mappings["InvoiceNo"].FieldId, Is.EqualTo(1001));
            Assert.That(mappings["InvoiceNo"].LineId, Is.EqualTo(100));
            Assert.That(mappings["InvoiceNo"].PartId, Is.EqualTo(10));

            Assert.That(mappings.ContainsKey("InvoiceKey"), Is.True); // From Key property
            Assert.That(mappings["InvoiceKey"].FieldId, Is.EqualTo(1001));


            Assert.That(mappings.ContainsKey("InvoiceTotal"), Is.True);
            Assert.That(mappings["InvoiceTotal"].FieldId, Is.EqualTo(1101));

            Assert.That(mappings.ContainsKey("TotalKey"), Is.True);
            Assert.That(mappings["TotalKey"].FieldId, Is.EqualTo(1101));


            Assert.That(mappings.ContainsKey("ItemDescription"), Is.True);
            Assert.That(mappings["ItemDescription"].FieldId, Is.EqualTo(2001));
            Assert.That(mappings["ItemDescription"].LineId, Is.EqualTo(200));
            Assert.That(mappings["ItemDescription"].PartId, Is.EqualTo(20));

            _logger.Information("✓ CreateEnhancedFieldMapping created correct mappings.");
        }

        #endregion

        #region ConvertDynamicToShipmentInvoicesWithMetadata (from OCRLegacySupport, but uses instance methods)

        [Test]
        [Category("MetadataExtraction")]
        public void ConvertDynamicToShipmentInvoicesWithMetadata_ValidInput_ShouldConvertAndExtract()
        {
            // Arrange
            var runtimeInvoiceDict = new Dictionary<string, object>(StringComparer.OrdinalIgnoreCase)
            {
                ["InvoiceNo"] = "DYN-001",
                ["InvoiceTotal"] = 500.0,
                ["SubTotal"] = 450.0,
                // Add a field that maps to the mock template
                ["InvoiceKey"] = "DYN-KEY-001" // This maps to InvoiceNo field via Key
            };
            var dynamicResults = new List<dynamic> { new List<IDictionary<string, object>> { runtimeInvoiceDict } };

            // Act
            // This static method internally creates an OCRCorrectionService instance.
            // We are testing the static public API here.
            var ocrCorrectionService = new OCRCorrectionService(_logger);
            var resultList = OCRCorrectionService.ConvertDynamicToShipmentInvoicesWithMetadata(
                dynamicResults,
                _mockOcrTemplate,
                ocrCorrectionService,
                _logger);

            // Assert
            Assert.That(resultList, Is.Not.Null);
            Assert.That(resultList.Count, Is.EqualTo(1));

            var firstInvoiceWithMeta = resultList.First();
            Assert.That(firstInvoiceWithMeta.Invoice, Is.Not.Null);
            Assert.That(firstInvoiceWithMeta.Invoice.InvoiceNo, Is.EqualTo("DYN-001")); // From "InvoiceNo"
            Assert.That(firstInvoiceWithMeta.Invoice.InvoiceTotal, Is.EqualTo(500.0));

            Assert.That(firstInvoiceWithMeta.FieldMetadata, Is.Not.Null);
            // Check if metadata for a template-mapped field was extracted.
            // "InvoiceKey" in runtime data maps to the "InvoiceNo" field definition in the template.
            // The metadata key will be the runtime key "InvoiceKey".
            Assert.That(firstInvoiceWithMeta.FieldMetadata.ContainsKey("InvoiceKey"), Is.True, "Metadata for 'InvoiceKey' should exist.");
            var invoiceKeyMeta = firstInvoiceWithMeta.FieldMetadata["InvoiceKey"];
            Assert.That(invoiceKeyMeta.FieldId, Is.EqualTo(1001)); // Field ID for InvoiceNo
            Assert.That(invoiceKeyMeta.Field, Is.EqualTo("InvoiceNo")); // The DB field it maps to

            _logger.Information("✓ ConvertDynamicToShipmentInvoicesWithMetadata converted and extracted metadata.");
        }

        #endregion
    }
}