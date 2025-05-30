using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Serilog;
using EntryDataDS.Business.Entities;
using WaterNut.DataSpace;

namespace AutoBotUtilities.Tests.Production
{
    /// <summary>
    /// Tests for field format pattern creation and validation
    /// </summary>
    [TestFixture]
    [Category("FieldFormat")]
    [Category("Patterns")]
    public class OCRCorrectionService_FieldFormatPatternTests
    {
        #region Test Setup

        private static ILogger _logger;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File($"logs/OCRFieldFormatPatternTests_{DateTime.Now:yyyyMMdd_HHmmss}.log")
                .CreateLogger();

            _logger.Information("Starting OCR Field Format Pattern Tests");
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _logger.Information("Completed OCR Field Format Pattern Tests");
        }

        #endregion

        #region Decimal Separator Pattern Tests

        [Test]
        [Category("DecimalSeparator")]
        public void CreateFormatCorrectionPatterns_DecimalCommaToPoint_ShouldCreateCorrectPattern()
        {
            // Arrange
            var oldValue = "123,45";
            var newValue = "123.45";

            // Act
            var (regexPattern, replacementPattern) = OCRCorrectionService.CreateFormatCorrectionPatterns(oldValue, newValue);

            // Assert
            Assert.That(regexPattern, Is.EqualTo(@"(\d+),(\d{2})"));
            Assert.That(replacementPattern, Is.EqualTo("$1.$2"));

            _logger.Information("✓ Decimal comma to point pattern created correctly");
        }

        [Test]
        [Category("DecimalSeparator")]
        public void CreateFormatCorrectionPatterns_LargeDecimalCommaToPoint_ShouldCreateCorrectPattern()
        {
            // Arrange
            var oldValue = "1234,56";
            var newValue = "1234.56";

            // Act
            var (regexPattern, replacementPattern) = OCRCorrectionService.CreateFormatCorrectionPatterns(oldValue, newValue);

            // Assert
            Assert.That(regexPattern, Is.EqualTo(@"(\d+),(\d{2})"));
            Assert.That(replacementPattern, Is.EqualTo("$1.$2"));

            _logger.Information("✓ Large decimal comma to point pattern created correctly");
        }

        [Test]
        [Category("DecimalSeparator")]
        public void CreateFormatCorrectionPatterns_InvalidDecimalSeparator_ShouldReturnNull()
        {
            // Arrange
            var oldValue = "123.45";
            var newValue = "123,45";

            // Act
            var (regexPattern, replacementPattern) = OCRCorrectionService.CreateFormatCorrectionPatterns(oldValue, newValue);

            // Assert
            Assert.That(regexPattern, Is.Null);
            Assert.That(replacementPattern, Is.Null);

            _logger.Information("✓ Invalid decimal separator conversion correctly returns null");
        }

        #endregion

        #region Currency Symbol Pattern Tests

        [Test]
        [Category("CurrencySymbol")]
        public void CreateFormatCorrectionPatterns_AddDollarSign_ShouldCreateCorrectPattern()
        {
            // Arrange
            var oldValue = "99.99";
            var newValue = "$99.99";

            // Act
            var (regexPattern, replacementPattern) = OCRCorrectionService.CreateFormatCorrectionPatterns(oldValue, newValue);

            // Assert
            Assert.That(regexPattern, Is.EqualTo(@"^(\d+\.?\d*)$"));
            Assert.That(replacementPattern, Is.EqualTo("$$1"));

            _logger.Information("✓ Currency symbol addition pattern created correctly");
        }

        [Test]
        [Category("CurrencySymbol")]
        public void CreateFormatCorrectionPatterns_IntegerCurrency_ShouldCreateCorrectPattern()
        {
            // Arrange
            var oldValue = "100";
            var newValue = "$100";

            // Act
            var (regexPattern, replacementPattern) = OCRCorrectionService.CreateFormatCorrectionPatterns(oldValue, newValue);

            // Assert
            Assert.That(regexPattern, Is.EqualTo(@"^(\d+\.?\d*)$"));
            Assert.That(replacementPattern, Is.EqualTo("$$1"));

            _logger.Information("✓ Integer currency pattern created correctly");
        }

        [Test]
        [Category("CurrencySymbol")]
        public void CreateFormatCorrectionPatterns_AlreadyHasDollarSign_ShouldReturnNull()
        {
            // Arrange
            var oldValue = "$99.99";
            var newValue = "$99.99";

            // Act
            var (regexPattern, replacementPattern) = OCRCorrectionService.CreateFormatCorrectionPatterns(oldValue, newValue);

            // Assert
            Assert.That(regexPattern, Is.Null);
            Assert.That(replacementPattern, Is.Null);

            _logger.Information("✓ Already has currency symbol correctly returns null");
        }

        #endregion

        #region Negative Number Pattern Tests

        [Test]
        [Category("NegativeNumber")]
        public void CreateFormatCorrectionPatterns_TrailingMinusToLeading_ShouldCreateCorrectPattern()
        {
            // Arrange
            var oldValue = "50.00-";
            var newValue = "-50.00";

            // Act
            var (regexPattern, replacementPattern) = OCRCorrectionService.CreateFormatCorrectionPatterns(oldValue, newValue);

            // Assert
            Assert.That(regexPattern, Is.EqualTo(@"(\d+\.?\d*)-$"));
            Assert.That(replacementPattern, Is.EqualTo("-$1"));

            _logger.Information("✓ Trailing minus to leading pattern created correctly");
        }

        [Test]
        [Category("NegativeNumber")]
        public void CreateFormatCorrectionPatterns_IntegerTrailingMinus_ShouldCreateCorrectPattern()
        {
            // Arrange
            var oldValue = "25-";
            var newValue = "-25";

            // Act
            var (regexPattern, replacementPattern) = OCRCorrectionService.CreateFormatCorrectionPatterns(oldValue, newValue);

            // Assert
            Assert.That(regexPattern, Is.EqualTo(@"(\d+\.?\d*)-$"));
            Assert.That(replacementPattern, Is.EqualTo("-$1"));

            _logger.Information("✓ Integer trailing minus pattern created correctly");
        }

        [Test]
        [Category("NegativeNumber")]
        public void CreateFormatCorrectionPatterns_AlreadyNegative_ShouldReturnNull()
        {
            // Arrange
            var oldValue = "-50.00";
            var newValue = "-50.00";

            // Act
            var (regexPattern, replacementPattern) = OCRCorrectionService.CreateFormatCorrectionPatterns(oldValue, newValue);

            // Assert
            Assert.That(regexPattern, Is.Null);
            Assert.That(replacementPattern, Is.Null);

            _logger.Information("✓ Already negative number correctly returns null");
        }

        #endregion

        #region Space Removal Pattern Tests

        [Test]
        [Category("SpaceRemoval")]
        public void CreateFormatCorrectionPatterns_RemoveSpacesInNumbers_ShouldCreateCorrectPattern()
        {
            // Arrange
            var oldValue = "1 234.56";
            var newValue = "1234.56";

            // Act
            var (regexPattern, replacementPattern) = OCRCorrectionService.CreateFormatCorrectionPatterns(oldValue, newValue);

            // Assert
            Assert.That(regexPattern, Is.EqualTo(@"(\d+)\s+(\d+)"));
            Assert.That(replacementPattern, Is.EqualTo("$1$2"));

            _logger.Information("✓ Space removal pattern created correctly");
        }

        [Test]
        [Category("SpaceRemoval")]
        public void CreateFormatCorrectionPatterns_MultipleSpaces_ShouldCreateCorrectPattern()
        {
            // Arrange
            var oldValue = "12 345 678.90";
            var newValue = "12345678.90";

            // Act
            var (regexPattern, replacementPattern) = OCRCorrectionService.CreateFormatCorrectionPatterns(oldValue, newValue);

            // Assert
            Assert.That(regexPattern, Is.EqualTo(@"(\d+)\s+(\d+)"));
            Assert.That(replacementPattern, Is.EqualTo("$1$2"));

            _logger.Information("✓ Multiple spaces removal pattern created correctly");
        }

        #endregion

        #region OCR Character Confusion Pattern Tests

        [Test]
        [Category("CharacterConfusion")]
        public void CreateFormatCorrectionPatterns_OToZero_ShouldCreateCorrectPattern()
        {
            // Arrange
            var oldValue = "1O5.OO";
            var newValue = "105.00";

            // Act
            var (regexPattern, replacementPattern) = OCRCorrectionService.CreateFormatCorrectionPatterns(oldValue, newValue);

            // Assert
            Assert.That(regexPattern, Is.EqualTo(@"([^A-Z])O([^A-Z])"));
            Assert.That(replacementPattern, Is.EqualTo("$10$2"));

            _logger.Information("✓ O to zero pattern created correctly");
        }

        [Test]
        [Category("CharacterConfusion")]
        public void CreateFormatCorrectionPatterns_LToOne_ShouldCreateCorrectPattern()
        {
            // Arrange
            var oldValue = "l23.45";
            var newValue = "123.45";

            // Act
            var (regexPattern, replacementPattern) = OCRCorrectionService.CreateFormatCorrectionPatterns(oldValue, newValue);

            // Assert
            Assert.That(regexPattern, Is.EqualTo(@"([^a-z])l([^a-z])"));
            Assert.That(replacementPattern, Is.EqualTo("$11$2"));

            _logger.Information("✓ l to one pattern created correctly");
        }

        [Test]
        [Category("CharacterConfusion")]
        public void CreateFormatCorrectionPatterns_ValidLetters_ShouldReturnNull()
        {
            // Arrange
            var oldValue = "TOTAL";
            var newValue = "T0TAL";

            // Act
            var (regexPattern, replacementPattern) = OCRCorrectionService.CreateFormatCorrectionPatterns(oldValue, newValue);

            // Assert
            Assert.That(regexPattern, Is.Null);
            Assert.That(replacementPattern, Is.Null);

            _logger.Information("✓ Valid letters correctly return null");
        }

        #endregion

        #region Edge Cases and Invalid Input Tests

        [Test]
        [Category("EdgeCases")]
        public void CreateFormatCorrectionPatterns_NullInput_ShouldReturnNull()
        {
            // Arrange
            string oldValue = null;
            string newValue = "123.45";

            // Act
            var (regexPattern, replacementPattern) = OCRCorrectionService.CreateFormatCorrectionPatterns(oldValue, newValue);

            // Assert
            Assert.That(regexPattern, Is.Null);
            Assert.That(replacementPattern, Is.Null);

            _logger.Information("✓ Null input correctly returns null");
        }

        [Test]
        [Category("EdgeCases")]
        public void CreateFormatCorrectionPatterns_EmptyInput_ShouldReturnNull()
        {
            // Arrange
            var oldValue = "";
            var newValue = "123.45";

            // Act
            var (regexPattern, replacementPattern) = OCRCorrectionService.CreateFormatCorrectionPatterns(oldValue, newValue);

            // Assert
            Assert.That(regexPattern, Is.Null);
            Assert.That(replacementPattern, Is.Null);

            _logger.Information("✓ Empty input correctly returns null");
        }

        [Test]
        [Category("EdgeCases")]
        public void CreateFormatCorrectionPatterns_IdenticalValues_ShouldReturnNull()
        {
            // Arrange
            var oldValue = "123.45";
            var newValue = "123.45";

            // Act
            var (regexPattern, replacementPattern) = OCRCorrectionService.CreateFormatCorrectionPatterns(oldValue, newValue);

            // Assert
            Assert.That(regexPattern, Is.Null);
            Assert.That(replacementPattern, Is.Null);

            _logger.Information("✓ Identical values correctly return null");
        }

        [Test]
        [Category("EdgeCases")]
        public void CreateFormatCorrectionPatterns_CompletelyDifferentValues_ShouldReturnNull()
        {
            // Arrange
            var oldValue = "ABC123";
            var newValue = "XYZ789";

            // Act
            var (regexPattern, replacementPattern) = OCRCorrectionService.CreateFormatCorrectionPatterns(oldValue, newValue);

            // Assert
            Assert.That(regexPattern, Is.Null);
            Assert.That(replacementPattern, Is.Null);

            _logger.Information("✓ Completely different values correctly return null");
        }

        #endregion

        #region Pattern Validation Tests

        [Test]
        [Category("PatternValidation")]
        public void CreateFormatCorrectionPatterns_AllSupportedPatterns_ShouldCreateValidRegex()
        {
            // Arrange
            var testCases = new[]
            {
                ("123,45", "123.45", "Decimal separator"),
                ("99.99", "$99.99", "Currency symbol"),
                ("50.00-", "-50.00", "Negative number"),
                ("1 234.56", "1234.56", "Space removal"),
                ("1O5.OO", "105.00", "O to zero"),
                ("l23.45", "123.45", "l to one")
            };

            // Act & Assert
            foreach (var (oldValue, newValue, description) in testCases)
            {
                var (regexPattern, replacementPattern) = OCRCorrectionService.CreateFormatCorrectionPatterns(oldValue, newValue);

                Assert.That(regexPattern, Is.Not.Null, $"{description} should create regex pattern");
                Assert.That(replacementPattern, Is.Not.Null, $"{description} should create replacement pattern");

                // Validate that the regex pattern is valid
                Assert.DoesNotThrow(() => new System.Text.RegularExpressions.Regex(regexPattern),
                    $"{description} should create valid regex pattern");

                _logger.Information("✓ {Description}: {Pattern} → {Replacement}", description, regexPattern, replacementPattern);
            }
        }

        #endregion
    }
}
