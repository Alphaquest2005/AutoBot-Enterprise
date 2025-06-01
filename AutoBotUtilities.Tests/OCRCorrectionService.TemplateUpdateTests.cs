using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Serilog;
using EntryDataDS.Business.Entities; // For ShipmentInvoice
using OCR.Business.Entities; // For Invoice (template object)
using WaterNut.DataSpace; // For OCRCorrectionService
using System.Reflection; // For calling private static methods
using static AutoBotUtilities.Tests.TestHelpers;

namespace AutoBotUtilities.Tests.Production
{
    using Invoices = OCR.Business.Entities.Invoices;
    using OCRCorrectionService = WaterNut.DataSpace.OCRCorrectionService;

    /// <summary>
    /// Tests for template line value updates and template state management
    /// </summary>
    [TestFixture]
    [Category("Template")]
    [Category("LineValues")]
    public class OCRCorrectionService_TemplateUpdateTests
    {
        #region Test Setup

        private static ILogger _logger;
        private OCRCorrectionService _service; // Instance for any instance methods (though most are static)
        private Invoice _mockTemplate; // OCR.Business.Entities.Invoice

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File($"logs/OCRTemplateUpdateTests_{DateTime.Now:yyyyMMdd_HHmmss}.log")
                .CreateLogger();

            _logger.Information("Starting OCR Template Update Tests");
        }

        [SetUp]
        public void SetUp()
        {
            // OCRCorrectionService might not be strictly needed if all tested methods are static
            // but it's good practice if any instance helpers are involved.
            _service = new OCRCorrectionService(_logger);
            _mockTemplate = CreateMockTemplateWithLines();
        }

        [TearDown]
        public void TearDown()
        {
            _service?.Dispose();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _logger.Information("Completed OCR Template Update Tests");
        }

        #endregion

        #region Template Field Mapping Tests

        [Test]
        [Category("FieldMapping")]
        public void GetTemplateFieldMappings_ValidTemplate_ShouldCreateMappings()
        {
            // Arrange
            var template = CreateMockTemplateWithLinesAndFields(); // Enhanced mock

            // Act
            // OCRLegacySupport.GetTemplateFieldMappings
            var mappings = CallPrivateStaticMethod<Dictionary<string, (int LineId, int FieldId)>>(
                typeof(OCRCorrectionService), // Assuming this static method is in OCRLegacySupport
                "GetTemplateFieldMappings",
                template,
                _logger);

            // Assert
            Assert.That(mappings, Is.Not.Null);
            Assert.That(mappings.Count, Is.EqualTo(2), "Expected mappings for InvoiceTotal and SubTotal from mock.");
            Assert.That(mappings.ContainsKey("InvoiceTotal"), Is.True);
            Assert.That(mappings.ContainsKey("SubTotal"), Is.True);

            _logger.Information("✓ Template field mappings created successfully with {Count} mappings", mappings.Count);
        }

        [Test]
        [Category("FieldMapping")]
        public void MapTemplateFieldToPropertyName_CommonFieldNames_ShouldMapCorrectly()
        {
            // Arrange & Act
            // OCRCorrectionService.MapTemplateFieldToPropertyName (static in OCRUtilities part?)
            // Assuming it's in OCRUtilities (part of OCRCorrectionService)
            var invoiceTotalMapping = OCRCorrectionService.MapTemplateFieldToPropertyName("invoicetotal");
            var subTotalMapping = OCRCorrectionService.MapTemplateFieldToPropertyName("sub_total");
            var freightMapping = OCRCorrectionService.MapTemplateFieldToPropertyName("freight");
            var unknownMapping = OCRCorrectionService.MapTemplateFieldToPropertyName("unknown_field");

            // Assert
            Assert.That(invoiceTotalMapping, Is.EqualTo("InvoiceTotal"));
            Assert.That(subTotalMapping, Is.EqualTo("SubTotal"));
            Assert.That(freightMapping, Is.EqualTo("TotalInternalFreight"));
            Assert.That(unknownMapping, Is.EqualTo("unknown_field")); // Fallback is to return original

            _logger.Information("✓ Template Field name to Property Name mapping working correctly");
        }

        #endregion

        #region Template Line Value Update Tests

        [Test]
        [Category("LineValueUpdate")]
        public void UpdateTemplateLineValues_ValidCorrections_ShouldUpdateTemplate()
        {
            // Arrange
            var template = CreateMockTemplateWithLinesAndFields();
            var correctedInvoices = new List<ShipmentInvoice>
            {
                new ShipmentInvoice
                {
                    InvoiceNo = "CORRECTED-001",
                    InvoiceTotal = 150.00, // Mapped to template field "InvoiceTotal"
                    SubTotal = 135.00,   // Mapped to template field "SubTotal"
                    TotalInternalFreight = 10.00, // No direct mapping in simple mock
                    TotalOtherCost = 5.00
                }
            };

            // Act
            // OCRLegacySupport.UpdateTemplateLineValues
            CallPrivateStaticMethod<object>(
                typeof(OCRCorrectionService), // Assuming this static method is in OCRLegacySupport
                "UpdateTemplateLineValues",
                template,
                correctedInvoices,
                _logger);

            // Assert
            Assert.That(template, Is.Not.Null);
            // Verify actual LineValue update if possible with the mock structure
            var invoiceTotalValWrapper = template.Lines.First(l => l.OCR_Lines.Fields.Any(f => f.Field == "InvoiceTotal"));
            var subTotalValWrapper = template.Lines.First(l => l.OCR_Lines.Fields.Any(f => f.Field == "SubTotal"));

            // Assuming single instance in LineValue for simplicity
            Assert.That(invoiceTotalValWrapper.Values.First().Value.First(kvp => kvp.Key.Item1.Field == "InvoiceTotal").Value, Is.EqualTo("150.00"));
            Assert.That(subTotalValWrapper.Values.First().Value.First(kvp => kvp.Key.Item1.Field == "SubTotal").Value, Is.EqualTo("135.00"));


            _logger.Information("✓ Template line values update completed and values verified.");
        }

        // UpdateFieldInTemplate is a helper for UpdateTemplateLineValues,
        // its effects are tested via UpdateTemplateLineValues.
        // If direct testing is needed:
        [Test]
        [Category("LineValueUpdate")]
        public void UpdateFieldInTemplate_SpecificField_ShouldUpdateValue()
        {
            // Arrange
            var template = CreateMockTemplateWithLinesAndFields();
            var fieldMappings = CallPrivateStaticMethod<Dictionary<string, (int LineId, int FieldId)>>(
                typeof(OCRCorrectionService), "GetTemplateFieldMappings", template, _logger);

            // Act
            CallPrivateStaticMethod<object>(
                typeof(OCRCorrectionService),
                "UpdateFieldInTemplate",
                template,
                fieldMappings,
                "InvoiceTotal", // Canonical property name
                "999.99",
                _logger);

            // Assert
            var invoiceTotalValWrapper = template.Lines.First(l => l.OCR_Lines.Fields.Any(f => f.Field == "InvoiceTotal"));
            Assert.That(invoiceTotalValWrapper.Values.First().Value.First(kvp => kvp.Key.Item1.Field == "InvoiceTotal").Value, Is.EqualTo("999.99"));
            _logger.Information("✓ UpdateFieldInTemplate updated a specific field value correctly.");
        }


        #endregion

        #region Helper Methods

        private Invoice CreateMockTemplateWithLines() // Basic version from original test
        {
            // OcrInvoices is the DB representation of the template definition.
            // Invoice (OCR.Business.Entities) is the runtime object.
            var mockOcrInvoicesEntity = new Invoices() { Id = 1, Name = "TestTemplateWithLines" };
            var template = new Invoice(mockOcrInvoicesEntity, _logger) // Pass ILogger if constructor expects it
            {
                FilePath = "test_template_with_lines.pdf",
                
            };
            return template;
        }

        private Invoice CreateMockTemplateWithLinesAndFields()
        {
            var mockOcrInvoicesEntity = new Invoices() { Id = 1, Name = "TestTemplate1" };
            var template = new Invoice(mockOcrInvoicesEntity, _logger)
            {
                FilePath = "test_template.pdf",
              
            };

            // Create a mock Part (e.g., Header)
            var headerPartEntity = new Parts { Id = 1, Invoices = mockOcrInvoicesEntity, PartTypes = new PartTypes() { Name = "Header", Id = 1 }, PartTypeId = 1};
            var headerPartWrapper = new Part(headerPartEntity, _logger);
            template.Parts.Add(headerPartWrapper);

            // Create mock Line definitions (OCR_Lines) and link to Fields
            var line1Def = new Lines { Id = 10, PartId = headerPartEntity.Id, Name = "InvoiceTotalLine", RegExId = 100, IsActive = true, Fields = new List<Fields>() };
            var field1Def = new Fields { Id = 101, LineId = line1Def.Id, Field = "InvoiceTotal", Key = "Total", EntityType = "ShipmentInvoice", DataType = "decimal" };
            line1Def.Fields.Add(field1Def);

            var line2Def = new Lines { Id = 20, PartId = headerPartEntity.Id, Name = "SubTotalLine", RegExId = 200, IsActive = true, Fields = new List<Fields>() };
            var field2Def = new Fields { Id = 201, LineId = line2Def.Id, Field = "SubTotal", Key = "Sub_Total", EntityType = "ShipmentInvoice", DataType = "decimal" };
            line2Def.Fields.Add(field2Def);

            // Add these Lines definitions to the Part's Lines collection
            headerPartEntity.Lines = new List<Lines> { line1Def, line2Def };


            // Create InvoiceLine wrappers (runtime instances of line definitions)
            // And populate their Values dictionary to simulate extracted data
            var line1Wrapper = new Line(line1Def, _logger);
            line1Wrapper.Values.Add(
                (1, "1-1"), // (Section, InstanceNum)
                new Dictionary<(Fields, string), string> { { (field1Def, "1"), "0.00" } } // ((FieldDef, FieldInstance), Value)
            );
            template.Lines.Add(line1Wrapper);

            var line2Wrapper = new Line(line2Def, _logger);
            line2Wrapper.Values.Add(
               (2, "1-1"),
               new Dictionary<(Fields, string), string> { { (field2Def, "1"), "0.00" } }
           );
            template.Lines.Add(line2Wrapper);

            return template;
        }


        // Helper to call private static methods using reflection
        private T CallPrivateStaticMethod<T>(Type type, string methodName, params object[] parameters)
        {
            var method = type.GetMethod(methodName,
                BindingFlags.NonPublic | BindingFlags.Static);

            if (method == null)
            {
                throw new ArgumentException($"Method {methodName} not found on type {type.Name}");
            }

            var result = method.Invoke(null, parameters);

            if (result is T typedResult) return typedResult;
            if (result == null && !typeof(T).IsValueType || Nullable.GetUnderlyingType(typeof(T)) != null) return default(T); // Handle null for reference types and Nullable<T>
            if (typeof(T) == typeof(object) && result != null) return (T)result; // If T is object, allow any non-null result

            // If T is a value type and result is null (or method is void and T is not object), this will be problematic.
            // For void methods, T should be object, and this method will return null.
            // If T is a value type and method returns null, it's an issue.
            // Let's assume for now that T matches the return type or is object for void.
            return default(T);
        }
        #endregion

        [Test]
        public async Task UpdateRegexPatternsAsync_ValidCorrections_ShouldUpdateDatabase()
        {
            // Arrange
            var corrections = new List<CorrectionResult> {
                new CorrectionResult {
                    FieldName = "InvoiceTotal",
                    NewValue = "123.45",
                    Success = true
                    // Note: CorrectionResult doesn't have RegexPattern property
                    // RegexCreationResponse is separate from CorrectionResult
                }
            };
            var invoiceId = 123; // Example invoice template ID
            
            // Act - Call the private method using reflection
            var result = await InvokePrivateMethod<Task<bool>>(_service, "UpdateRegexPatternsAsync", corrections, invoiceId);
            
            // Assert
            Assert.That(result, Is.True, "Should successfully update patterns");
            
            // Verify the database was updated by retrieving the pattern
            using (var context = new OCRContext())
            {
                var updatedPattern = context.RegularExpressions
                    .FirstOrDefault(r => r.RegEx.Contains("Total") && r.RegEx.Contains(@"(\d+\.\d+)"));
                
                Assert.That(updatedPattern, Is.Not.Null, "Pattern should be saved to database");
            }
        }
    }
}
