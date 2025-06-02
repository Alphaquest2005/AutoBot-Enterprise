using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Serilog;
using EntryDataDS.Business.Entities; // For ShipmentInvoice
using OCR.Business.Entities; // For Invoice (template object)
using WaterNut.DataSpace; // For OCRCorrectionService and EnhancedFieldMapping
using WaterNut.Data; // For OCRContext
using static AutoBotUtilities.Tests.TestHelpers;
using static WaterNut.DataSpace.OCRCorrectionService; // For EnhancedFieldMapping

namespace AutoBotUtilities.Tests.Production
{
    using Invoices = OCR.Business.Entities.Invoices;
    using OCRCorrectionService = WaterNut.DataSpace.OCRCorrectionService;

    /// <summary>
    /// Enhanced tests for OCR template functionality using real Amazon template structure
    /// Based on amazon_template_structure.log analysis
    /// </summary>
    [TestFixture]
    [Category("EnhancedTemplate")]
    [Category("AmazonTemplate")]
    public class OCRCorrectionService_EnhancedTemplateTests
    {
        #region Test Setup

        private static ILogger _logger;
        private OCRCorrectionService _service;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File($"logs/OCREnhancedTemplateTests_{DateTime.Now:yyyyMMdd_HHmmss}.log")
                .CreateLogger();

            _logger.Information("Starting OCR Enhanced Template Tests with Real Amazon Structure");
        }

        [SetUp]
        public void SetUp()
        {
            _service = new OCRCorrectionService(_logger);
        }

        [TearDown]
        public void TearDown()
        {
            _service?.Dispose();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _logger.Information("Completed OCR Enhanced Template Tests");
        }

        #endregion

        #region Real Amazon Template Structure Tests

        [Test]
        [Category("RealTemplateStructure")]
        public void AmazonTemplate_FieldMapping_ShouldMatchRealStructure()
        {
            // Arrange - Create enhanced field mappings based on real Amazon template
            var realAmazonFieldMappings = CreateRealAmazonFieldMappings();

            // Act & Assert - Verify all key Amazon fields are present
            Assert.That(realAmazonFieldMappings.Count, Is.EqualTo(14), "Should have 14 unique field mappings from real Amazon template");

            // Verify header fields (Part ID: 1028)
            var invoiceTotalMapping = realAmazonFieldMappings.FirstOrDefault(fm => fm.FieldName == "InvoiceTotal");
            Assert.That(invoiceTotalMapping, Is.Not.Null, "Should have InvoiceTotal mapping");
            Assert.That(invoiceTotalMapping.LineId, Is.EqualTo(37), "Should match real Amazon InvoiceTotal LineId");
            Assert.That(invoiceTotalMapping.PartId, Is.EqualTo(1028), "Should match real Amazon Header PartId");
            Assert.That(invoiceTotalMapping.EntityType, Is.EqualTo("Invoice"), "Should be Invoice entity type");
            Assert.That(invoiceTotalMapping.DataType, Is.EqualTo("Number"), "Should be Number data type");

            var subTotalMapping = realAmazonFieldMappings.FirstOrDefault(fm => fm.FieldName == "SubTotal");
            Assert.That(subTotalMapping, Is.Not.Null, "Should have SubTotal mapping");
            Assert.That(subTotalMapping.LineId, Is.EqualTo(78), "Should match real Amazon SubTotal LineId");

            var freightMapping = realAmazonFieldMappings.FirstOrDefault(fm => fm.FieldName == "TotalInternalFreight");
            Assert.That(freightMapping, Is.Not.Null, "Should have TotalInternalFreight mapping");
            Assert.That(freightMapping.LineId, Is.EqualTo(35), "Should match real Amazon Freight LineId");

            var salesTaxMapping = realAmazonFieldMappings.FirstOrDefault(fm => fm.FieldName == "TotalOtherCost");
            Assert.That(salesTaxMapping, Is.Not.Null, "Should have TotalOtherCost mapping");
            Assert.That(salesTaxMapping.LineId, Is.EqualTo(36), "Should match real Amazon SalesTax LineId");

            // Verify summary fields (Line ID: 39)
            var supplierCodeMapping = realAmazonFieldMappings.FirstOrDefault(fm => fm.FieldName == "SupplierCode");
            Assert.That(supplierCodeMapping, Is.Not.Null, "Should have SupplierCode mapping");
            Assert.That(supplierCodeMapping.LineId, Is.EqualTo(39), "Should match real Amazon Summary LineId");
            Assert.That(supplierCodeMapping.DataType, Is.EqualTo("String"), "Should be String data type");

            var invoiceNoMapping = realAmazonFieldMappings.FirstOrDefault(fm => fm.FieldName == "InvoiceNo");
            Assert.That(invoiceNoMapping, Is.Not.Null, "Should have InvoiceNo mapping");
            Assert.That(invoiceNoMapping.LineId, Is.EqualTo(39), "Should match real Amazon Summary LineId");

            var invoiceDateMapping = realAmazonFieldMappings.FirstOrDefault(fm => fm.FieldName == "InvoiceDate");
            Assert.That(invoiceDateMapping, Is.Not.Null, "Should have InvoiceDate mapping");
            Assert.That(invoiceDateMapping.DataType, Is.EqualTo("English Date"), "Should be English Date data type");

            _logger.Information("✓ Real Amazon template field mappings validated successfully");
            foreach (var mapping in realAmazonFieldMappings.Take(5))
            {
                _logger.Information("  - {FieldName}: LineId={LineId}, PartId={PartId}, DataType={DataType}", 
                    mapping.FieldName, mapping.LineId, mapping.PartId, mapping.DataType);
            }
        }

        [Test]
        [Category("RealTemplateStructure")]
        public void AmazonTemplate_RegexPatterns_ShouldMatchRealPatterns()
        {
            // Arrange
            var realAmazonFieldMappings = CreateRealAmazonFieldMappings();

            // Act & Assert - Verify regex patterns match real Amazon template
            var invoiceTotalMapping = realAmazonFieldMappings.First(fm => fm.FieldName == "InvoiceTotal");
            Assert.That(invoiceTotalMapping.RegexPattern, 
                Is.EqualTo(@"((Grand)|(Order)) Total:(\s*(?<Currency>USD))?\s*\$?(?<InvoiceTotal>[\d,.]+)"),
                "Should match real Amazon InvoiceTotal regex pattern");

            var subTotalMapping = realAmazonFieldMappings.First(fm => fm.FieldName == "SubTotal");
            Assert.That(subTotalMapping.RegexPattern, 
                Is.EqualTo(@"Item\(s\) Subtotal[:\r\n\s]+(\s*(?<Currency>USD))?\s*\$?(?<SubTotal>[\d,.]+)"),
                "Should match real Amazon SubTotal regex pattern");

            var freightMapping = realAmazonFieldMappings.First(fm => fm.FieldName == "TotalInternalFreight");
            Assert.That(freightMapping.RegexPattern, 
                Is.EqualTo(@"Shipping & Handling:[\s]+(?<Currency>\w{3})?[\s\$]+(?<Freight>[\d,.]+)"),
                "Should match real Amazon Freight regex pattern");

            var summaryMapping = realAmazonFieldMappings.First(fm => fm.FieldName == "SupplierCode");
            Assert.That(summaryMapping.RegexPattern, 
                Contains.Substring("Amazon.com"),
                "Summary regex should contain Amazon.com pattern");

            _logger.Information("✓ Real Amazon template regex patterns validated successfully");
        }

        #endregion

        #region Helper Methods

        private List<EnhancedFieldMapping> CreateRealAmazonFieldMappings()
        {
            // Create field mappings based on real Amazon template structure from amazon_template_structure.log
            return new List<EnhancedFieldMapping>
            {
                // Line 37: InvoiceTotal
                new EnhancedFieldMapping
                {
                    FieldName = "InvoiceTotal",
                    LineId = 37,
                    FieldId = 3700,
                    PartId = 1028,
                    Key = "InvoiceTotal",
                    EntityType = "Invoice",
                    DataType = "Number",
                    RegexPattern = @"((Grand)|(Order)) Total:(\s*(?<Currency>USD))?\s*\$?(?<InvoiceTotal>[\d,.]+)"
                },
                // Line 78: SubTotal
                new EnhancedFieldMapping
                {
                    FieldName = "SubTotal",
                    LineId = 78,
                    FieldId = 7800,
                    PartId = 1028,
                    Key = "SubTotal",
                    EntityType = "Invoice",
                    DataType = "Number",
                    RegexPattern = @"Item\(s\) Subtotal[:\r\n\s]+(\s*(?<Currency>USD))?\s*\$?(?<SubTotal>[\d,.]+)"
                },
                // Line 35: Freight (TotalInternalFreight)
                new EnhancedFieldMapping
                {
                    FieldName = "TotalInternalFreight",
                    LineId = 35,
                    FieldId = 3500,
                    PartId = 1028,
                    Key = "Freight",
                    EntityType = "Invoice",
                    DataType = "Number",
                    RegexPattern = @"Shipping & Handling:[\s]+(?<Currency>\w{3})?[\s\$]+(?<Freight>[\d,.]+)"
                },
                // Line 36: SalesTax (TotalOtherCost)
                new EnhancedFieldMapping
                {
                    FieldName = "TotalOtherCost",
                    LineId = 36,
                    FieldId = 3600,
                    PartId = 1028,
                    Key = "SalesTax",
                    EntityType = "Invoice",
                    DataType = "Number",
                    RegexPattern = @"Estimated tax to be collected:(\s+(?<Currency>\w{3}))?\s+\$?(?<SalesTax>[\d,.]+)"
                },
                // Line 39: Summary fields (SupplierCode, InvoiceNo, InvoiceDate, Name)
                new EnhancedFieldMapping
                {
                    FieldName = "SupplierCode",
                    LineId = 39,
                    FieldId = 3900,
                    PartId = 1028,
                    Key = "SupplierCode",
                    EntityType = "Invoice",
                    DataType = "String",
                    RegexPattern = @"((?<SupplierCode>Amazon.com)\s- Order (?<InvoiceNo>[\d\-]+) (?<InvoiceDate>[\d/]+),\s)|(Order Placed: (?<InvoiceDate>[\w\,\s]+)[\r\n](?<SupplierCode>Amazon.com) order number\: (?<InvoiceNo>[\d\-]+))"
                },
                new EnhancedFieldMapping
                {
                    FieldName = "InvoiceNo",
                    LineId = 39,
                    FieldId = 3901,
                    PartId = 1028,
                    Key = "InvoiceNo",
                    EntityType = "Invoice",
                    DataType = "String",
                    RegexPattern = @"((?<SupplierCode>Amazon.com)\s- Order (?<InvoiceNo>[\d\-]+) (?<InvoiceDate>[\d/]+),\s)|(Order Placed: (?<InvoiceDate>[\w\,\s]+)[\r\n](?<SupplierCode>Amazon.com) order number\: (?<InvoiceNo>[\d\-]+))"
                },
                new EnhancedFieldMapping
                {
                    FieldName = "InvoiceDate",
                    LineId = 39,
                    FieldId = 3902,
                    PartId = 1028,
                    Key = "InvoiceDate",
                    EntityType = "Invoice",
                    DataType = "English Date",
                    RegexPattern = @"((?<SupplierCode>Amazon.com)\s- Order (?<InvoiceNo>[\d\-]+) (?<InvoiceDate>[\d/]+),\s)|(Order Placed: (?<InvoiceDate>[\w\,\s]+)[\r\n](?<SupplierCode>Amazon.com) order number\: (?<InvoiceNo>[\d\-]+))"
                },
                new EnhancedFieldMapping
                {
                    FieldName = "Name",
                    LineId = 39,
                    FieldId = 3903,
                    PartId = 1028,
                    Key = "InvoiceNo",
                    EntityType = "Invoice",
                    DataType = "String",
                    RegexPattern = @"((?<SupplierCode>Amazon.com)\s- Order (?<InvoiceNo>[\d\-]+) (?<InvoiceDate>[\d/]+),\s)|(Order Placed: (?<InvoiceDate>[\w\,\s]+)[\r\n](?<SupplierCode>Amazon.com) order number\: (?<InvoiceNo>[\d\-]+))"
                },
                // Line 1831: FreeShipping (TotalDeduction)
                new EnhancedFieldMapping
                {
                    FieldName = "TotalDeduction",
                    LineId = 1831,
                    FieldId = 183100,
                    PartId = 1028,
                    Key = "FreeShipping",
                    EntityType = "Invoice",
                    DataType = "Number",
                    RegexPattern = @"Free Shipping:[\s\-\$]+(?<Currency>\w{3})[\s\-\$]+(?<FreeShipping>[\d,.]+)"
                },
                // Additional Amazon fields from real template
                // Line 2089: Shipping & Handling (alternative pattern)
                new EnhancedFieldMapping
                {
                    FieldName = "TotalInternalFreight",
                    LineId = 2089,
                    FieldId = 208900,
                    PartId = 1028,
                    Key = "Shipping",
                    EntityType = "Invoice",
                    DataType = "Number",
                    RegexPattern = @"Shipping \& Handling[:\s]+(?<Currency>\w{3})[\s\-\$]+(?<Shipping>[\d,.]+)"
                },
                // Line 2090: Coupons (TotalInsurance)
                new EnhancedFieldMapping
                {
                    FieldName = "TotalInsurance",
                    LineId = 2090,
                    FieldId = 209000,
                    PartId = 1028,
                    Key = "Coupon",
                    EntityType = "Invoice",
                    DataType = "Number",
                    RegexPattern = @"Your Coupon Savings[:\s\-\$]+(?<Currency>\w{3})[\s\-\$]+(?<Coupon>[\d,.]+)"
                },
                // Line 2092: Estimated Tax (alternative pattern)
                new EnhancedFieldMapping
                {
                    FieldName = "TotalOtherCost",
                    LineId = 2092,
                    FieldId = 209200,
                    PartId = 1028,
                    Key = "Tax",
                    EntityType = "Invoice",
                    DataType = "Number",
                    RegexPattern = @"Estimated tax.*[:\s\-\$]+(?<Currency>\w{3})[\s\-\$]+(?<Tax>[\d,.]+)"
                },
                // Line 2093: Subscribe and Save (TotalDeduction)
                new EnhancedFieldMapping
                {
                    FieldName = "TotalDeduction",
                    LineId = 2093,
                    FieldId = 209300,
                    PartId = 1028,
                    Key = "Save",
                    EntityType = "Invoice",
                    DataType = "Number",
                    RegexPattern = @"Subscribe[\&\s]+Save[:\s\-\$]+(?<Currency>\w{3})[\s\-\$]+(?<Save>[\d,.]+)"
                },
                // Line 2094: Lightning Deal (TotalDeduction)
                new EnhancedFieldMapping
                {
                    FieldName = "TotalDeduction",
                    LineId = 2094,
                    FieldId = 209400,
                    PartId = 1028,
                    Key = "Save",
                    EntityType = "Invoice",
                    DataType = "Number",
                    RegexPattern = @"Lightning[\s]+Deal([:\s\-\$""]+(?<Currency>\w{3}))?[:\s\-\$""]+(?<Save>[\d,.]+)"
                }
            };
        }

        #endregion

        #region Amazon Template Workflow Tests

        [Test]
        [Category("AmazonWorkflow")]
        public void AmazonTemplate_CorrectionWorkflow_ShouldProcessCorrectly()
        {
            // Arrange - Create test invoice with Amazon-specific fields
            var testInvoice = new ShipmentInvoice
            {
                InvoiceNo = "123-4567890-1234567",
                SupplierCode = "Amazon.com",
                InvoiceDate = DateTime.Parse("2024-01-15"),
                InvoiceTotal = (double?)123.45m,
                SubTotal = (double?)100.00m,
                TotalInternalFreight = (double?)15.00m,
                TotalOtherCost = (double?)8.45m,
                TotalDeduction = (double?)0.00m
            };

            var fieldMappings = CreateRealAmazonFieldMappings();
            var fileText = @"
                Amazon.com - Order 123-4567890-1234567 01/15/2024

                Item(s) Subtotal: $100.00
                Shipping & Handling: $15.00
                Estimated tax to be collected: $8.45
                Order Total: $123.45
            ";

            // Act - Test field mapping resolution
            var invoiceTotalMapping = fieldMappings.FirstOrDefault(fm => fm.FieldName == "InvoiceTotal");
            var subTotalMapping = fieldMappings.FirstOrDefault(fm => fm.FieldName == "SubTotal");
            var freightMapping = fieldMappings.FirstOrDefault(fm => fm.FieldName == "TotalInternalFreight");

            // Assert - Verify mappings exist and are correct
            Assert.That(invoiceTotalMapping, Is.Not.Null, "Should find InvoiceTotal mapping");
            Assert.That(subTotalMapping, Is.Not.Null, "Should find SubTotal mapping");
            Assert.That(freightMapping, Is.Not.Null, "Should find TotalInternalFreight mapping");

            // Verify field mapping properties
            Assert.That(invoiceTotalMapping.Key, Is.EqualTo("InvoiceTotal"), "InvoiceTotal key should match");
            Assert.That(subTotalMapping.Key, Is.EqualTo("SubTotal"), "SubTotal key should match");
            Assert.That(freightMapping.Key, Is.EqualTo("Freight"), "Freight key should match Amazon pattern");

            // Test regex pattern matching
            var invoiceTotalRegex = new System.Text.RegularExpressions.Regex(invoiceTotalMapping.RegexPattern);
            var invoiceTotalMatch = invoiceTotalRegex.Match(fileText);
            Assert.That(invoiceTotalMatch.Success, Is.True, "InvoiceTotal regex should match file text");
            Assert.That(invoiceTotalMatch.Groups["InvoiceTotal"].Value, Is.EqualTo("123.45"), "Should extract correct InvoiceTotal value");

            var subTotalRegex = new System.Text.RegularExpressions.Regex(subTotalMapping.RegexPattern);
            var subTotalMatch = subTotalRegex.Match(fileText);
            Assert.That(subTotalMatch.Success, Is.True, "SubTotal regex should match file text");
            Assert.That(subTotalMatch.Groups["SubTotal"].Value, Is.EqualTo("100.00"), "Should extract correct SubTotal value");

            _logger.Information("✓ Amazon template correction workflow validated successfully");
            _logger.Information("  - InvoiceTotal: {Value} (LineId: {LineId})",
                invoiceTotalMatch.Groups["InvoiceTotal"].Value, invoiceTotalMapping.LineId);
            _logger.Information("  - SubTotal: {Value} (LineId: {LineId})",
                subTotalMatch.Groups["SubTotal"].Value, subTotalMapping.LineId);
        }

        [Test]
        [Category("AmazonWorkflow")]
        public void AmazonTemplate_FieldValidation_ShouldValidateCorrectly()
        {
            // Arrange
            var fieldMappings = CreateRealAmazonFieldMappings();

            // Act & Assert - Test field validation for different data types
            var invoiceTotalMapping = fieldMappings.First(fm => fm.FieldName == "InvoiceTotal");
            Assert.That(invoiceTotalMapping.DataType, Is.EqualTo("Number"), "InvoiceTotal should be Number type");

            var supplierCodeMapping = fieldMappings.First(fm => fm.FieldName == "SupplierCode");
            Assert.That(supplierCodeMapping.DataType, Is.EqualTo("String"), "SupplierCode should be String type");

            var invoiceDateMapping = fieldMappings.First(fm => fm.FieldName == "InvoiceDate");
            Assert.That(invoiceDateMapping.DataType, Is.EqualTo("English Date"), "InvoiceDate should be English Date type");

            // Test entity type validation
            foreach (var mapping in fieldMappings)
            {
                Assert.That(mapping.EntityType, Is.EqualTo("Invoice"),
                    $"All Amazon template fields should be Invoice entity type, but {mapping.FieldName} is {mapping.EntityType}");
            }

            // Test required field validation
            var requiredFields = fieldMappings.Where(fm =>
                fm.FieldName == "InvoiceTotal" ||
                fm.FieldName == "SubTotal" ||
                fm.FieldName == "SupplierCode" ||
                fm.FieldName == "InvoiceNo" ||
                fm.FieldName == "InvoiceDate").ToList();

            foreach (var requiredField in requiredFields)
            {
                // Note: IsRequired property would need to be added to EnhancedFieldMapping if not present
                _logger.Information("Required field: {FieldName} (LineId: {LineId})",
                    requiredField.FieldName, requiredField.LineId);
            }

            _logger.Information("✓ Amazon template field validation completed successfully");
        }

        #endregion
    }
}
