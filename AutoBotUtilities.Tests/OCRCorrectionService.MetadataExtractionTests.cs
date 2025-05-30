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
    /// Tests for OCR metadata extraction and ShipmentInvoiceWithMetadata functionality
    /// </summary>
    [TestFixture]
    [Category("Metadata")]
    [Category("OCR")]
    public class OCRCorrectionService_MetadataExtractionTests
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
                .WriteTo.File($"logs/OCRMetadataTests_{DateTime.Now:yyyyMMdd_HHmmss}.log")
                .CreateLogger();

            _logger.Information("Starting OCR Metadata Extraction Tests");
        }

        [SetUp]
        public void SetUp()
        {
            _service = new OCRCorrectionService();
            _mockTemplate = CreateMockTemplate();
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

        #endregion

        #region OCR Metadata Tests

        [Test]
        [Category("MetadataExtraction")]
        public void OCRFieldMetadata_Creation_ShouldSetAllProperties()
        {
            // Arrange & Act
            var metadata = new OCRFieldMetadata
            {
                LineNumber = 15,
                FieldId = 123,
                LineId = 456,
                RegexId = 789,
                Section = "Header",
                Instance = "1",
                RawValue = "$123.45",
                FieldName = "InvoiceTotal"
            };

            // Assert
            Assert.That(metadata.LineNumber, Is.EqualTo(15));
            Assert.That(metadata.FieldId, Is.EqualTo(123));
            Assert.That(metadata.LineId, Is.EqualTo(456));
            Assert.That(metadata.RegexId, Is.EqualTo(789));
            Assert.That(metadata.Section, Is.EqualTo("Header"));
            Assert.That(metadata.Instance, Is.EqualTo("1"));
            Assert.That(metadata.RawValue, Is.EqualTo("$123.45"));
            Assert.That(metadata.FieldName, Is.EqualTo("InvoiceTotal"));

            _logger.Information("✓ OCRFieldMetadata properties set correctly");
        }

        [Test]
        [Category("MetadataExtraction")]
        public void ShipmentInvoiceWithMetadata_Creation_ShouldInitializeCorrectly()
        {
            // Arrange
            var invoice = CreateTestInvoice("TEST-001", 100.0);
            var metadata = new Dictionary<string, OCRFieldMetadata>
            {
                ["InvoiceTotal"] = new OCRFieldMetadata
                {
                    LineNumber = 10,
                    FieldId = 1,
                    FieldName = "InvoiceTotal",
                    RawValue = "100.00"
                }
            };

            // Act
            var invoiceWithMetadata = new ShipmentInvoiceWithMetadata
            {
                Invoice = invoice,
                FieldMetadata = metadata
            };

            // Assert
            Assert.That(invoiceWithMetadata.Invoice, Is.Not.Null);
            Assert.That(invoiceWithMetadata.Invoice.InvoiceNo, Is.EqualTo("TEST-001"));
            Assert.That(invoiceWithMetadata.FieldMetadata.Count, Is.EqualTo(1));
            Assert.That(invoiceWithMetadata.FieldMetadata.ContainsKey("InvoiceTotal"), Is.True);

            var fieldMetadata = invoiceWithMetadata.GetFieldMetadata("InvoiceTotal");
            Assert.That(fieldMetadata, Is.Not.Null);
            Assert.That(fieldMetadata.LineNumber, Is.EqualTo(10));
            Assert.That(fieldMetadata.FieldId, Is.EqualTo(1));

            _logger.Information("✓ ShipmentInvoiceWithMetadata created and accessed correctly");
        }

        [Test]
        [Category("MetadataExtraction")]
        public void ShipmentInvoiceWithMetadata_GetFieldMetadata_ShouldReturnCorrectMetadata()
        {
            // Arrange
            var invoiceWithMetadata = new ShipmentInvoiceWithMetadata
            {
                Invoice = CreateTestInvoice("TEST-002", 200.0),
                FieldMetadata = new Dictionary<string, OCRFieldMetadata>()
            };

            var metadata = new OCRFieldMetadata
            {
                LineNumber = 20,
                FieldId = 2,
                FieldName = "SubTotal",
                RawValue = "180.00"
            };

            invoiceWithMetadata.SetFieldMetadata("SubTotal", metadata);

            // Act
            var retrievedMetadata = invoiceWithMetadata.GetFieldMetadata("SubTotal");
            var nonExistentMetadata = invoiceWithMetadata.GetFieldMetadata("NonExistent");

            // Assert
            Assert.That(retrievedMetadata, Is.Not.Null);
            Assert.That(retrievedMetadata.LineNumber, Is.EqualTo(20));
            Assert.That(retrievedMetadata.FieldId, Is.EqualTo(2));
            Assert.That(retrievedMetadata.FieldName, Is.EqualTo("SubTotal"));
            Assert.That(retrievedMetadata.RawValue, Is.EqualTo("180.00"));

            Assert.That(nonExistentMetadata, Is.Null);

            _logger.Information("✓ Field metadata retrieval working correctly");
        }

        [Test]
        [Category("MetadataExtraction")]
        public void ShipmentInvoiceWithMetadata_SetFieldMetadata_ShouldUpdateMetadata()
        {
            // Arrange
            var invoiceWithMetadata = new ShipmentInvoiceWithMetadata
            {
                Invoice = CreateTestInvoice("TEST-003", 300.0),
                FieldMetadata = new Dictionary<string, OCRFieldMetadata>()
            };

            var originalMetadata = new OCRFieldMetadata
            {
                LineNumber = 30,
                FieldId = 3,
                FieldName = "InvoiceTotal",
                RawValue = "300.00"
            };

            var updatedMetadata = new OCRFieldMetadata
            {
                LineNumber = 31,
                FieldId = 3,
                FieldName = "InvoiceTotal",
                RawValue = "$300.00"
            };

            // Act
            invoiceWithMetadata.SetFieldMetadata("InvoiceTotal", originalMetadata);
            var firstRetrieval = invoiceWithMetadata.GetFieldMetadata("InvoiceTotal");

            invoiceWithMetadata.SetFieldMetadata("InvoiceTotal", updatedMetadata);
            var secondRetrieval = invoiceWithMetadata.GetFieldMetadata("InvoiceTotal");

            // Assert
            Assert.That(firstRetrieval.LineNumber, Is.EqualTo(30));
            Assert.That(firstRetrieval.RawValue, Is.EqualTo("300.00"));

            Assert.That(secondRetrieval.LineNumber, Is.EqualTo(31));
            Assert.That(secondRetrieval.RawValue, Is.EqualTo("$300.00"));

            _logger.Information("✓ Field metadata update working correctly");
        }

        [Test]
        [Category("MetadataExtraction")]
        public void ConvertDynamicToShipmentInvoicesWithMetadata_ValidInput_ShouldExtractMetadata()
        {
            // Arrange
            var dynamicResults = CreateTestDynamicResults();

            // Act
            var result = CallPrivateStaticMethod<List<ShipmentInvoiceWithMetadata>>(
                typeof(OCRCorrectionService),
                "ConvertDynamicToShipmentInvoicesWithMetadata",
                dynamicResults,
                _mockTemplate);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Count, Is.GreaterThan(0));

            var firstInvoiceWithMetadata = result.First();
            Assert.That(firstInvoiceWithMetadata.Invoice, Is.Not.Null);
            Assert.That(firstInvoiceWithMetadata.FieldMetadata, Is.Not.Null);

            _logger.Information("✓ Dynamic conversion with metadata extraction successful");
        }

        [Test]
        [Category("MetadataExtraction")]
        public void ExtractOCRMetadata_ValidInvoiceDict_ShouldCreateMetadata()
        {
            // Arrange
            var invoiceDict = new Dictionary<string, object>
            {
                ["InvoiceNo"] = "TEST-004",
                ["InvoiceTotal"] = 400.0,
                ["SubTotal"] = 350.0,
                ["TotalInternalFreight"] = 25.0,
                ["TotalOtherCost"] = 15.0,
                ["TotalInsurance"] = 10.0
            };

            var fieldMappings = new Dictionary<string, (int LineId, int FieldId)>
            {
                ["InvoiceTotal"] = (1, 101),
                ["SubTotal"] = (2, 102),
                ["TotalInternalFreight"] = (3, 103)
            };

            // Act
            var metadata = CallPrivateStaticMethod<Dictionary<string, OCRFieldMetadata>>(
                typeof(OCRCorrectionService),
                "ExtractOCRMetadata",
                invoiceDict,
                _mockTemplate,
                fieldMappings);

            // Assert
            Assert.That(metadata, Is.Not.Null);
            Assert.That(metadata.Count, Is.GreaterThan(0));

            if (metadata.ContainsKey("InvoiceTotal"))
            {
                var invoiceTotalMetadata = metadata["InvoiceTotal"];
                Assert.That(invoiceTotalMetadata.FieldName, Is.EqualTo("InvoiceTotal"));
            }

            _logger.Information("✓ OCR metadata extraction from invoice dictionary successful");
        }

        #endregion

        #region Helper Methods

        private ShipmentInvoice CreateTestInvoice(string invoiceNo, double total)
        {
            return new ShipmentInvoice
            {
                InvoiceNo = invoiceNo,
                InvoiceTotal = total,
                SubTotal = total * 0.9,
                TotalInternalFreight = total * 0.05,
                TotalOtherCost = total * 0.03,
                TotalInsurance = total * 0.02,
                InvoiceDetails = new List<InvoiceDetails>()
            };
        }

        private Invoice CreateMockTemplate()
        {
            // Create a mock template with basic structure
            var mockInvoices = new OCR.Business.Entities.Invoices { Id = 1, Name = "TestTemplate" };
            return new Invoice(mockInvoices, _logger)
            {
                FilePath = "test_template.pdf"
            };
        }

        private List<dynamic> CreateTestDynamicResults()
        {
            var invoiceDict = new Dictionary<string, object>
            {
                ["InvoiceNo"] = "DYN-001",
                ["InvoiceTotal"] = 500.0,
                ["SubTotal"] = 450.0,
                ["TotalInternalFreight"] = 30.0,
                ["TotalOtherCost"] = 20.0
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
            else if (result is Task<T> taskResult)
            {
                return taskResult.Result;
            }
            else
            {
                return default(T);
            }
        }

        #endregion
    }
}
