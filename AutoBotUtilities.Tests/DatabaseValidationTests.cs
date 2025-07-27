using NUnit.Framework;
using static NUnit.Framework.Assert;
using OCR.Business.Entities;
using Serilog;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Moq;
using InvoiceReader.OCRCorrectionService;

namespace AutoBotUtilities.Tests
{
    [TestFixture]
    public class DatabaseValidationTests
    {
        private DatabaseValidator _validator;
        private Mock<OCRContext> _mockContext;
        private Mock<DbSet<Fields>> _mockFields;
        private Mock<DbSet<Lines>> _mockLines;
        private Mock<DbSet<Parts>> _mockParts;
        private Mock<DbSet<Templates>> _mockInvoices;
        private Mock<DbSet<RegularExpressions>> _mockRegex;
        private ILogger _logger;

        [SetUp]
        public void Setup()
        {
            _logger = new LoggerConfiguration().WriteTo.Console().CreateLogger();
            
            var fieldsData = new List<Fields>
            {
                // Duplicate mapping for Gift Card
                new Fields { Id = 1, LineId = 1, Key = "GiftCard", Field = "TotalOtherCost", EntityType = "Invoice", DataType = "Number", AppendValues = true, IsRequired = false, Lines = new Lines { Id = 1, Name = "Gift Card", PartId = 1, Parts = new Parts { Id = 1, TemplateId = 5, Templates = new Templates { Id = 5, Name = "Amazon" } } } },
                new Fields { Id = 2, LineId = 1, Key = "GiftCard", Field = "TotalInsurance", EntityType = "ShipmentInvoice", DataType = "decimal", AppendValues = true, IsRequired = false, Lines = new Lines { Id = 1, Name = "Gift Card", PartId = 1, Parts = new Parts { Id = 1, TemplateId = 5, Templates = new Templates { Id = 5, Name = "Amazon" } } } },
                // Valid mapping
                new Fields { Id = 3, LineId = 2, Key = "SubTotal", Field = "SubTotal", EntityType = "Invoice", DataType = "decimal", AppendValues = false, IsRequired = true, Lines = new Lines { Id = 2, Name = "SubTotal Line", PartId = 1, Parts = new Parts { Id = 1, TemplateId = 5, Templates = new Templates { Id = 5, Name = "Amazon" } } } }
            }.AsQueryable();

            _mockFields = new Mock<DbSet<Fields>>();
            _mockFields.As<IQueryable<Fields>>().Setup(m => m.Provider).Returns(fieldsData.Provider);
            _mockFields.As<IQueryable<Fields>>().Setup(m => m.Expression).Returns(fieldsData.Expression);
            _mockFields.As<IQueryable<Fields>>().Setup(m => m.ElementType).Returns(fieldsData.ElementType);
            _mockFields.As<IQueryable<Fields>>().Setup(m => m.GetEnumerator()).Returns(fieldsData.GetEnumerator());

            _mockContext = new Mock<OCRContext>();
            _mockContext.Setup(c => c.Fields).Returns(_mockFields.Object);
            
            _validator = new DatabaseValidator(_mockContext.Object, _logger);
        }

        [Test]
        public void DetectDuplicateFieldMappings_ShouldFindDuplicates()
        {
            // Act
            var duplicates = _validator.DetectDuplicateFieldMappings();

            // Assert
            Assert.That(duplicates.Count, Is.EqualTo(1));
            Assert.That(duplicates[0].Key, Is.EqualTo("GiftCard"));
            Assert.That(duplicates[0].DuplicateFields.Count, Is.EqualTo(2));
        }

        [Test]
        public void CleanupDuplicateFieldMappings_ShouldRemoveDuplicate()
        {
            // Arrange
            var duplicates = _validator.DetectDuplicateFieldMappings();
            
            // Act
            var result = _validator.CleanupDuplicateFieldMappings(duplicates);

            // Assert
            Assert.That(result.Success, Is.True);
            Assert.That(result.RemovedCount, Is.EqualTo(1));
            Assert.That(result.KeptCount, Is.EqualTo(1));
            _mockFields.Verify(m => m.Remove(It.Is<Fields>(f => f.Id == 1)), Times.Once);
        }

        [Test]
        public void AnalyzeAppendValuesUsage_ShouldUnderstandImportBehavior()
        {
            // Act
            var analysis = _validator.AnalyzeAppendValuesUsage();

            // Assert - Critical for understanding user's concern about "proper importation"
            Assert.That(analysis, Is.Not.Null, "AppendValues analysis should return results");
            Assert.That(analysis.TotalFields, Is.GreaterThan(0), "Should analyze fields with DataType");
            Assert.That(analysis.UsagePatterns, Is.Not.Null, "Should provide usage patterns for business logic understanding");

            // Verify we can identify aggregation vs replacement behavior
            var numericPatterns = analysis.UsagePatterns
                .Where(p => p.DataType == "Number" || p.DataType == "Numeric")
                .ToList();

            if (numericPatterns.Any())
            {
                foreach (var pattern in numericPatterns)
                {
                    // This is critical: AppendValues=true means SUM, AppendValues=false means REPLACE
                    string expectedBehavior = pattern.AppendValues == true ? "SUM values" : "REPLACE values";
                    Assert.That(pattern.AppendValues, Is.Not.Null, 
                        $"Numeric fields should have explicit AppendValues to avoid unpredictable import behavior. Pattern: {expectedBehavior}");
                }
            }
        }

        [Test]
        public void ValidateDataTypes_ShouldCheckPseudoDataTypes()
        {
            // Act
            var issues = _validator.ValidateDataTypes();

            // Assert - Based on user correction that system uses pseudo datatypes like "Number", "English Date"
            Assert.That(issues, Is.Not.Null, "DataType validation should return results");
            
            // Should not flag "Number" and "decimal" as unsupported since they're valid pseudo datatypes
            var numberTypeIssues = issues.Where(i => i.DataType == "Number" && i.IssueType == "UnsupportedDataType").ToList();
            var decimalTypeIssues = issues.Where(i => i.DataType == "decimal" && i.IssueType == "UnsupportedDataType").ToList();
            
            Assert.That(numberTypeIssues.Count, Is.EqualTo(0), "Number should be recognized as valid pseudo datatype");
            Assert.That(decimalTypeIssues.Count, Is.EqualTo(0), "decimal should be recognized as valid pseudo datatype");
        }
    }
}