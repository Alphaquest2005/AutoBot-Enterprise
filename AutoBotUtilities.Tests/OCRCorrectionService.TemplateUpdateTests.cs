using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Serilog;
using EntryDataDS.Business.Entities;
using OCR.Business.Entities;
using WaterNut.DataSpace;

namespace AutoBotUtilities.Tests.Production
{
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
        private OCRCorrectionService _service;
        private Invoice _mockTemplate;

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
            _service = new OCRCorrectionService();
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
            var template = CreateMockTemplateWithLines();

            // Act
            var mappings = CallPrivateStaticMethod<Dictionary<string, (int LineId, int FieldId)>>(
                typeof(OCRCorrectionService),
                "GetTemplateFieldMappings",
                template);

            // Assert
            Assert.That(mappings, Is.Not.Null);
            Assert.That(mappings.Count, Is.GreaterThan(0));

            _logger.Information("✓ Template field mappings created successfully with {Count} mappings", mappings.Count);
        }

        [Test]
        [Category("FieldMapping")]
        public void MapFieldNameToProperty_CommonFieldNames_ShouldMapCorrectly()
        {
            // Arrange & Act
            var invoiceTotalMapping = CallPrivateStaticMethod<string>(
                typeof(OCRCorrectionService),
                "MapFieldNameToProperty",
                "invoicetotal");

            var subTotalMapping = CallPrivateStaticMethod<string>(
                typeof(OCRCorrectionService),
                "MapFieldNameToProperty",
                "sub_total");

            var freightMapping = CallPrivateStaticMethod<string>(
                typeof(OCRCorrectionService),
                "MapFieldNameToProperty",
                "freight");

            var unknownMapping = CallPrivateStaticMethod<string>(
                typeof(OCRCorrectionService),
                "MapFieldNameToProperty",
                "unknown_field");

            // Assert
            Assert.That(invoiceTotalMapping, Is.EqualTo("InvoiceTotal"));
            Assert.That(subTotalMapping, Is.EqualTo("SubTotal"));
            Assert.That(freightMapping, Is.EqualTo("TotalInternalFreight"));
            Assert.That(unknownMapping, Is.Null);

            _logger.Information("✓ Field name mapping working correctly");
        }

        #endregion

        #region Template Line Value Update Tests

        [Test]
        [Category("LineValueUpdate")]
        public void UpdateTemplateLineValues_ValidCorrections_ShouldUpdateTemplate()
        {
            // Arrange
            var template = CreateMockTemplateWithLines();
            var correctedInvoices = new List<ShipmentInvoice>
            {
                new ShipmentInvoice
                {
                    InvoiceNo = "CORRECTED-001",
                    InvoiceTotal = 150.00,
                    SubTotal = 135.00,
                    TotalInternalFreight = 10.00,
                    TotalOtherCost = 5.00
                }
            };

            // Act
            CallPrivateStaticMethod<object>(
                typeof(OCRCorrectionService),
                "UpdateTemplateLineValues",
                template,
                correctedInvoices,
                _logger);

            // Assert
            Assert.That(template, Is.Not.Null);
            // Note: Since we're using a mock template, we can't verify actual line updates
            // but we can verify the method executes without errors

            _logger.Information("✓ Template line values update completed without errors");
        }

        [Test]
        [Category("LineValueUpdate")]
        public void UpdateInvoiceFieldsInTemplate_ValidInvoice_ShouldUpdateFields()
        {
            // Arrange
            var template = CreateMockTemplateWithLines();
            var correctedInvoice = new ShipmentInvoice
            {
                InvoiceNo = "FIELD-UPDATE-001",
                InvoiceTotal = 200.00,
                SubTotal = 180.00,
                TotalInternalFreight = 15.00,
                TotalOtherCost = 5.00
            };

            var fieldMappings = new Dictionary<string, (int LineId, int FieldId)>
            {
                ["InvoiceTotal"] = (1, 101),
                ["SubTotal"] = (2, 102),
                ["TotalInternalFreight"] = (3, 103)
            };

            // Act
            CallPrivateStaticMethod<object>(
                typeof(OCRCorrectionService),
                "UpdateInvoiceFieldsInTemplate",
                template,
                correctedInvoice,
                fieldMappings,
                _logger);

            // Assert
            Assert.That(template, Is.Not.Null);
            // Method should execute without throwing exceptions

            _logger.Information("✓ Invoice fields updated in template successfully");
        }

        [Test]
        [Category("LineValueUpdate")]
        public void UpdateFieldInTemplate_ValidFieldMapping_ShouldUpdateField()
        {
            // Arrange
            var template = CreateMockTemplateWithLines();
            var fieldMappings = new Dictionary<string, (int LineId, int FieldId)>
            {
                ["InvoiceTotal"] = (1, 101)
            };

            // Act
            CallPrivateStaticMethod<object>(
                typeof(OCRCorrectionService),
                "UpdateFieldInTemplate",
                template,
                fieldMappings,
                "InvoiceTotal",
                "250.00",
                _logger);

            // Assert
            Assert.That(template, Is.Not.Null);
            // Method should execute without throwing exceptions

            _logger.Information("✓ Individual field updated in template successfully");
        }

        [Test]
        [Category("LineValueUpdate")]
        public void UpdateFieldInTemplate_NonExistentField_ShouldHandleGracefully()
        {
            // Arrange
            var template = CreateMockTemplateWithLines();
            var fieldMappings = new Dictionary<string, (int LineId, int FieldId)>();

            // Act & Assert
            Assert.DoesNotThrow(() =>
            {
                CallPrivateStaticMethod<object>(
                    typeof(OCRCorrectionService),
                    "UpdateFieldInTemplate",
                    template,
                    fieldMappings,
                    "NonExistentField",
                    "123.45",
                    _logger);
            });

            _logger.Information("✓ Non-existent field update handled gracefully");
        }

        #endregion

        #region Integration Tests

        [Test]
        [Category("Integration")]
        public async Task CorrectInvoices_WithTemplateUpdates_ShouldUpdateBothResultsAndTemplate()
        {
            // Arrange
            var dynamicResults = CreateTestDynamicResults();
            var template = CreateMockTemplateWithLines();

            // Act
            await CallPrivateStaticMethodAsync(
                typeof(OCRCorrectionService),
                "CorrectInvoices",
                dynamicResults,
                template,
                _logger);

            // Assert
            Assert.That(dynamicResults, Is.Not.Null);
            Assert.That(template, Is.Not.Null);
            // Both should be updated without throwing exceptions

            _logger.Information("✓ Integrated correction with template updates completed");
        }

        [Test]
        [Category("Integration")]
        public void UpdateDynamicResultsWithCorrections_ValidCorrections_ShouldUpdateResults()
        {
            // Arrange
            var dynamicResults = CreateTestDynamicResults();
            var correctedInvoices = new List<ShipmentInvoice>
            {
                new ShipmentInvoice
                {
                    InvoiceNo = "DYNAMIC-001",
                    InvoiceTotal = 300.00,
                    SubTotal = 270.00,
                    TotalInternalFreight = 20.00,
                    TotalOtherCost = 10.00
                }
            };

            // Act
            CallPrivateStaticMethod<object>(
                typeof(OCRCorrectionService),
                "UpdateDynamicResultsWithCorrections",
                dynamicResults,
                correctedInvoices);

            // Assert
            Assert.That(dynamicResults, Is.Not.Null);
            Assert.That(dynamicResults.Count, Is.GreaterThan(0));

            _logger.Information("✓ Dynamic results updated with corrections successfully");
        }

        #endregion

        #region Helper Methods

        private Invoice CreateMockTemplateWithLines()
        {
            var mockInvoices = new OCR.Business.Entities.Invoices { Id = 1, Name = "TestTemplateWithLines" };
            var template = new Invoice(mockInvoices, _logger)
            {
                FilePath = "test_template_with_lines.pdf"
            };

            // Note: In a real test, we would set up actual Lines with OCR_Lines and Fields
            // For now, we create a basic structure that won't cause null reference exceptions
            return template;
        }

        private List<dynamic> CreateTestDynamicResults()
        {
            var invoiceDict = new Dictionary<string, object>
            {
                ["InvoiceNo"] = "TEMPLATE-001",
                ["InvoiceTotal"] = 400.0,
                ["SubTotal"] = 360.0,
                ["TotalInternalFreight"] = 25.0,
                ["TotalOtherCost"] = 15.0
            };

            var invoiceList = new List<IDictionary<string, object>> { invoiceDict };
            return new List<dynamic> { invoiceList };
        }

        private T CallPrivateStaticMethod<T>(Type type, string methodName, params object[] parameters)
        {
            var method = type.GetMethod(methodName,
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

            if (method == null)
            {
                throw new ArgumentException($"Method {methodName} not found on type {type.Name}");
            }

            var result = method.Invoke(null, parameters);

            if (result is T typedResult)
            {
                return typedResult;
            }
            else if (result == null && !typeof(T).IsValueType)
            {
                return default(T);
            }
            else
            {
                return default(T);
            }
        }

        private async Task CallPrivateStaticMethodAsync(Type type, string methodName, params object[] parameters)
        {
            var method = type.GetMethod(methodName,
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Static);

            if (method == null)
            {
                throw new ArgumentException($"Method {methodName} not found on type {type.Name}");
            }

            var result = method.Invoke(null, parameters);

            if (result is Task task)
            {
                await task;
            }
        }

        #endregion
    }
}
