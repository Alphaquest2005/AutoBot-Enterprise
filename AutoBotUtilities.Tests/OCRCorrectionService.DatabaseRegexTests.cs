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
    /// Tests for database regex pattern updates and field format corrections
    /// </summary>
    [TestFixture]
    [Category("Database")]
    [Category("Regex")]
    public class OCRCorrectionService_DatabaseRegexTests
    {
        #region Test Setup

        private static ILogger _logger;
        private OCRCorrectionService _service;
        private string _testDatabaseName;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File($"logs/OCRDatabaseRegexTests_{DateTime.Now:yyyyMMdd_HHmmss}.log")
                .CreateLogger();

            _testDatabaseName = $"OCRTest_{Guid.NewGuid():N}";
            _logger.Information("Starting OCR Database Regex Tests with test database: {Database}", _testDatabaseName);
        }

        [SetUp]
        public void SetUp()
        {
            _service = new OCRCorrectionService();
        }

        [TearDown]
        public void TearDown()
        {
            _service?.Dispose();
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _logger.Information("Completed OCR Database Regex Tests");
        }

        #endregion

        #region Field Format Regex Tests

        [Test]
        [Category("FieldFormat")]
        public async Task UpdateFieldFormatRegex_DecimalSeparatorCorrection_ShouldCreateDatabaseEntry()
        {
            // Arrange
            var corrections = new List<CorrectionResult>
            {
                new CorrectionResult
                {
                    FieldName = "InvoiceTotal",
                    OldValue = "123,45",
                    NewValue = "123.45",
                    Success = true,
                    Confidence = 0.95,
                    CorrectionType = "FieldFormat"
                }
            };

            var fileText = "Invoice Total: 123,45";

            // Act
            await _service.UpdateRegexPatternsAsync(corrections, fileText);

            // Assert
            using var ctx = new OCRContext();
            var fieldFormatRegex = ctx.OCR_FieldFormatRegEx
                .Where(ffr => ffr.Fields.Field == "InvoiceTotal")
                .OrderByDescending(ffr => ffr.Id)
                .FirstOrDefault();

            Assert.That(fieldFormatRegex, Is.Not.Null, "Field format regex should be created");
            Assert.That(fieldFormatRegex.RegEx.RegEx, Does.Contain(@"(\d+),(\d{2})"), "Should contain decimal separator pattern");
            Assert.That(fieldFormatRegex.ReplacementRegEx.RegEx, Is.EqualTo("$1.$2"), "Should have correct replacement pattern");

            _logger.Information("✓ Decimal separator correction created field format regex entry");
        }

        [Test]
        [Category("FieldFormat")]
        public async Task UpdateFieldFormatRegex_CurrencySymbolCorrection_ShouldCreateDatabaseEntry()
        {
            // Arrange
            var corrections = new List<CorrectionResult>
            {
                new CorrectionResult
                {
                    FieldName = "SubTotal",
                    OldValue = "99.99",
                    NewValue = "$99.99",
                    Success = true,
                    Confidence = 0.90,
                    CorrectionType = "FieldFormat"
                }
            };

            var fileText = "Subtotal: 99.99";

            // Act
            await _service.UpdateRegexPatternsAsync(corrections, fileText);

            // Assert
            using var ctx = new OCRContext();
            var fieldFormatRegex = ctx.OCR_FieldFormatRegEx
                .Where(ffr => ffr.Fields.Field == "SubTotal")
                .OrderByDescending(ffr => ffr.Id)
                .FirstOrDefault();

            Assert.That(fieldFormatRegex, Is.Not.Null, "Field format regex should be created");
            Assert.That(fieldFormatRegex.RegEx.RegEx, Does.Contain(@"^(\d+\.?\d*)$"), "Should contain currency pattern");
            Assert.That(fieldFormatRegex.ReplacementRegEx.RegEx, Is.EqualTo("$$1"), "Should have correct currency replacement");

            _logger.Information("✓ Currency symbol correction created field format regex entry");
        }

        [Test]
        [Category("FieldFormat")]
        public async Task UpdateFieldFormatRegex_NegativeNumberCorrection_ShouldCreateDatabaseEntry()
        {
            // Arrange
            var corrections = new List<CorrectionResult>
            {
                new CorrectionResult
                {
                    FieldName = "TotalDeduction",
                    OldValue = "50.00-",
                    NewValue = "-50.00",
                    Success = true,
                    Confidence = 0.88,
                    CorrectionType = "FieldFormat"
                }
            };

            var fileText = "Deduction: 50.00-";

            // Act
            await _service.UpdateRegexPatternsAsync(corrections, fileText);

            // Assert
            using var ctx = new OCRContext();
            var fieldFormatRegex = ctx.OCR_FieldFormatRegEx
                .Where(ffr => ffr.Fields.Field == "TotalDeduction")
                .OrderByDescending(ffr => ffr.Id)
                .FirstOrDefault();

            Assert.That(fieldFormatRegex, Is.Not.Null, "Field format regex should be created");
            Assert.That(fieldFormatRegex.RegEx.RegEx, Does.Contain(@"(\d+\.?\d*)-$"), "Should contain negative number pattern");
            Assert.That(fieldFormatRegex.ReplacementRegEx.RegEx, Is.EqualTo("-$1"), "Should have correct negative replacement");

            _logger.Information("✓ Negative number correction created field format regex entry");
        }

        [Test]
        [Category("FieldFormat")]
        public async Task UpdateFieldFormatRegex_OCRCharacterConfusion_ShouldCreateDatabaseEntry()
        {
            // Arrange
            var corrections = new List<CorrectionResult>
            {
                new CorrectionResult
                {
                    FieldName = "InvoiceTotal",
                    OldValue = "1O5.OO",
                    NewValue = "105.00",
                    Success = true,
                    Confidence = 0.92,
                    CorrectionType = "FieldFormat"
                }
            };

            var fileText = "Total: 1O5.OO";

            // Act
            await _service.UpdateRegexPatternsAsync(corrections, fileText);

            // Assert
            using var ctx = new OCRContext();
            var fieldFormatRegex = ctx.OCR_FieldFormatRegEx
                .Where(ffr => ffr.Fields.Field == "InvoiceTotal")
                .OrderByDescending(ffr => ffr.Id)
                .FirstOrDefault();

            Assert.That(fieldFormatRegex, Is.Not.Null, "Field format regex should be created");
            Assert.That(fieldFormatRegex.RegEx.RegEx, Does.Contain("O"), "Should contain OCR character confusion pattern");

            _logger.Information("✓ OCR character confusion correction created field format regex entry");
        }

        [Test]
        [Category("FieldFormat")]
        public async Task UpdateFieldFormatRegex_MultipleCorrections_ShouldCreateMultipleEntries()
        {
            // Arrange
            var corrections = new List<CorrectionResult>
            {
                new CorrectionResult
                {
                    FieldName = "InvoiceTotal",
                    OldValue = "123,45",
                    NewValue = "123.45",
                    Success = true,
                    Confidence = 0.95,
                    CorrectionType = "FieldFormat"
                },
                new CorrectionResult
                {
                    FieldName = "SubTotal",
                    OldValue = "99.99",
                    NewValue = "$99.99",
                    Success = true,
                    Confidence = 0.90,
                    CorrectionType = "FieldFormat"
                },
                new CorrectionResult
                {
                    FieldName = "TotalDeduction",
                    OldValue = "10.00-",
                    NewValue = "-10.00",
                    Success = true,
                    Confidence = 0.88,
                    CorrectionType = "FieldFormat"
                }
            };

            var fileText = "Invoice Total: 123,45\nSubtotal: 99.99\nDeduction: 10.00-";

            // Act
            await _service.UpdateRegexPatternsAsync(corrections, fileText);

            // Assert
            using var ctx = new OCRContext();
            
            var invoiceTotalRegex = ctx.OCR_FieldFormatRegEx
                .Where(ffr => ffr.Fields.Field == "InvoiceTotal")
                .OrderByDescending(ffr => ffr.Id)
                .FirstOrDefault();

            var subTotalRegex = ctx.OCR_FieldFormatRegEx
                .Where(ffr => ffr.Fields.Field == "SubTotal")
                .OrderByDescending(ffr => ffr.Id)
                .FirstOrDefault();

            var deductionRegex = ctx.OCR_FieldFormatRegEx
                .Where(ffr => ffr.Fields.Field == "TotalDeduction")
                .OrderByDescending(ffr => ffr.Id)
                .FirstOrDefault();

            Assert.That(invoiceTotalRegex, Is.Not.Null, "InvoiceTotal field format regex should be created");
            Assert.That(subTotalRegex, Is.Not.Null, "SubTotal field format regex should be created");
            Assert.That(deductionRegex, Is.Not.Null, "TotalDeduction field format regex should be created");

            _logger.Information("✓ Multiple field format corrections created multiple database entries");
        }

        #endregion

        #region Learning Table Tests

        [Test]
        [Category("Learning")]
        public async Task LogCorrectionsToLearningTable_SuccessfulCorrections_ShouldCreateLearningEntries()
        {
            // Arrange
            var corrections = new List<CorrectionResult>
            {
                new CorrectionResult
                {
                    FieldName = "InvoiceTotal",
                    OldValue = "123,45",
                    NewValue = "123.45",
                    Success = true,
                    Confidence = 0.95,
                    CorrectionType = "FieldFormat"
                },
                new CorrectionResult
                {
                    FieldName = "SubTotal",
                    OldValue = "wrong_value",
                    NewValue = "correct_value",
                    Success = true,
                    Confidence = 0.80,
                    CorrectionType = "Extraction"
                }
            };

            var fileText = "Test invoice text";

            // Act
            await _service.UpdateRegexPatternsAsync(corrections, fileText);

            // Assert
            using var ctx = new OCRContext();
            var learningEntries = ctx.OCRCorrectionLearning
                .Where(ocl => ocl.CreatedBy == "OCRCorrectionService")
                .OrderByDescending(ocl => ocl.CreatedDate)
                .Take(2)
                .ToList();

            Assert.That(learningEntries.Count, Is.EqualTo(2), "Should create learning entries for both corrections");

            var fieldFormatEntry = learningEntries.FirstOrDefault(e => e.CorrectionType == "FieldFormat");
            var extractionEntry = learningEntries.FirstOrDefault(e => e.CorrectionType == "Extraction");

            Assert.That(fieldFormatEntry, Is.Not.Null, "Should have field format learning entry");
            Assert.That(fieldFormatEntry.FieldName, Is.EqualTo("InvoiceTotal"));
            Assert.That(fieldFormatEntry.OriginalError, Is.EqualTo("123,45"));
            Assert.That(fieldFormatEntry.CorrectValue, Is.EqualTo("123.45"));

            Assert.That(extractionEntry, Is.Not.Null, "Should have extraction learning entry");
            Assert.That(extractionEntry.FieldName, Is.EqualTo("SubTotal"));
            Assert.That(extractionEntry.CorrectionType, Is.EqualTo("Extraction"));

            _logger.Information("✓ Corrections logged to learning table successfully");
        }

        #endregion

        #region Helper Methods

        private CorrectionResult CreateTestCorrection(string fieldName, string oldValue, string newValue, string correctionType = "FieldFormat")
        {
            return new CorrectionResult
            {
                FieldName = fieldName,
                OldValue = oldValue,
                NewValue = newValue,
                Success = true,
                Confidence = 0.90,
                CorrectionType = correctionType
            };
        }

        #endregion
    }
}
