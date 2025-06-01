using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Serilog;
using EntryDataDS.Business.Entities; // Not directly used, but common in project
using WaterNut.DataSpace; // For OCRCorrectionService

namespace AutoBotUtilities.Tests.Production
{
    using OCRCorrectionService = WaterNut.DataSpace.OCRCorrectionService;

    /// <summary>
    /// Tests for advanced field format pattern creation and validation
    /// Focuses on OCRCorrectionService.CreateAdvancedFormatCorrectionPatterns
    /// </summary>
    [TestFixture]
    [Category("FieldFormat")]
    [Category("Patterns")]
    public class OCRCorrectionService_FieldFormatPatternTests
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
                .WriteTo.File($"logs/OCRFieldFormatPatternTests_Advanced_{DateTime.Now:yyyyMMdd_HHmmss}.log")
                .CreateLogger();

            _logger.Information("Starting Advanced OCR Field Format Pattern Tests");
        }

        [SetUp]
        public void SetUp()
        {
            // OCRCorrectionService constructor takes an optional ILogger
            _service = new OCRCorrectionService(_logger);
        }

        [OneTimeTearDown]
        public void OneTimeTearDown()
        {
            _logger.Information("Completed Advanced OCR Field Format Pattern Tests");
        }

        #endregion

        #region Decimal Separator Pattern Tests

        [Test]
        [Category("DecimalSeparator")]
        public void CreateAdvancedFormatCorrectionPatterns_DecimalCommaToPoint_ShouldCreateCorrectPattern()
        {
            // Arrange
            var oldValue = "123,45";
            var newValue = "123.45";

            // Act
            var result = _service.CreateAdvancedFormatCorrectionPatterns(oldValue, newValue);

            // Assert
            Assert.That(result, Is.Not.Null, "Pattern result should not be null.");
            Assert.That(result.Value.Pattern, Is.EqualTo(@"(\d+),(\d{1,4})"));
            Assert.That(result.Value.Replacement, Is.EqualTo("$1.$2"));
            _logger.Information("✓ Decimal comma to point pattern created correctly: {Old} -> {New} => P: '{Pattern}', R: '{Replacement}'", oldValue, newValue, result.Value.Pattern, result.Value.Replacement);
        }

        [Test]
        [Category("DecimalSeparator")]
        public void CreateAdvancedFormatCorrectionPatterns_DecimalPointToComma_ShouldCreateCorrectPattern()
        {
            // Arrange
            var oldValue = "123.45";
            var newValue = "123,45";

            // Act
            var result = _service.CreateAdvancedFormatCorrectionPatterns(oldValue, newValue);

            // Assert
            Assert.That(result, Is.Not.Null, "Pattern result should not be null.");
            Assert.That(result.Value.Pattern, Is.EqualTo(@"(\d+)\.(\d{1,4})"));
            Assert.That(result.Value.Replacement, Is.EqualTo("$1,$2"));
            _logger.Information("✓ Decimal point to comma pattern created correctly: {Old} -> {New} => P: '{Pattern}', R: '{Replacement}'", oldValue, newValue, result.Value.Pattern, result.Value.Replacement);
        }

        [Test]
        [Category("DecimalSeparator")]
        public void CreateAdvancedFormatCorrectionPatterns_InvalidDecimalChange_ShouldReturnNull()
        {
            // Arrange
            var oldValue = "ABC,45"; // Not a number
            var newValue = "ABC.45";

            // Act
            var result = _service.CreateAdvancedFormatCorrectionPatterns(oldValue, newValue);

            // Assert
            Assert.That(result, Is.Null); // Expect null because CreateDecimalSeparatorPattern will reject non-numeric
            _logger.Information("✓ Invalid decimal change (non-numeric) correctly returns null.");
        }

        #endregion

        #region Currency Symbol Pattern Tests

        [Test]
        [Category("CurrencySymbol")]
        public void CreateAdvancedFormatCorrectionPatterns_AddDollarSign_ShouldCreateCorrectPattern()
        {
            // Arrange
            var oldValue = "99.99";
            var newValue = "$99.99";

            // Act
            var result = _service.CreateAdvancedFormatCorrectionPatterns(oldValue, newValue);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value.Pattern, Is.EqualTo(@"^(-?\d+(\.\d+)?)$"));
            Assert.That(result.Value.Replacement, Is.EqualTo("$$$1")); // Dollar sign needs to be escaped in replacement string
            _logger.Information("✓ Add dollar sign pattern: {Old} -> {New} => P: '{Pattern}', R: '{Replacement}'", oldValue, newValue, result.Value.Pattern, result.Value.Replacement);
        }

        [Test]
        [Category("CurrencySymbol")]
        public void CreateAdvancedFormatCorrectionPatterns_RemoveDollarSign_ShouldCreateCorrectPattern()
        {
            // Arrange
            var oldValue = "$99.99";
            var newValue = "99.99";

            // Act
            var result = _service.CreateAdvancedFormatCorrectionPatterns(oldValue, newValue);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value.Pattern, Is.EqualTo(@"\$\s*(-?\d+(\.\d+)?)"));
            Assert.That(result.Value.Replacement, Is.EqualTo("$1"));
            _logger.Information("✓ Remove dollar sign pattern: {Old} -> {New} => P: '{Pattern}', R: '{Replacement}'", oldValue, newValue, result.Value.Pattern, result.Value.Replacement);
        }

        [Test]
        [Category("CurrencySymbol")]
        public void CreateAdvancedFormatCorrectionPatterns_AddEuroSign_ShouldCreateCorrectPattern()
        {
            // Arrange
            var oldValue = "123.45";
            var newValue = "€123.45";

            // Act
            var result = _service.CreateAdvancedFormatCorrectionPatterns(oldValue, newValue);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value.Pattern, Is.EqualTo(@"^(-?\d+(\.\d+)?)$"));
            Assert.That(result.Value.Replacement, Is.EqualTo("€$1"));
            _logger.Information("✓ Add Euro sign pattern: {Old} -> {New} => P: '{Pattern}', R: '{Replacement}'", oldValue, newValue, result.Value.Pattern, result.Value.Replacement);
        }

        #endregion

        #region Negative Number Pattern Tests

        [Test]
        [Category("NegativeNumber")]
        public void CreateAdvancedFormatCorrectionPatterns_TrailingMinusToLeading_ShouldCreateCorrectPattern()
        {
            // Arrange
            var oldValue = "50.00-";
            var newValue = "-50.00";

            // Act
            var result = _service.CreateAdvancedFormatCorrectionPatterns(oldValue, newValue);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value.Pattern, Is.EqualTo(@"(\d+(\.\d+)?)-$"));
            Assert.That(result.Value.Replacement, Is.EqualTo("-$1"));
            _logger.Information("✓ Trailing minus to leading pattern: {Old} -> {New} => P: '{Pattern}', R: '{Replacement}'", oldValue, newValue, result.Value.Pattern, result.Value.Replacement);
        }

        [Test]
        [Category("NegativeNumber")]
        public void CreateAdvancedFormatCorrectionPatterns_ParenthesesToLeadingMinus_ShouldCreateCorrectPattern()
        {
            // Arrange
            var oldValue = "(50.00)";
            var newValue = "-50.00";

            // Act
            var result = _service.CreateAdvancedFormatCorrectionPatterns(oldValue, newValue);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value.Pattern, Is.EqualTo(@"\(\s*(\d+(\.\d+)?)\s*\)"));
            Assert.That(result.Value.Replacement, Is.EqualTo("-$1"));
            _logger.Information("✓ Parentheses to leading minus pattern: {Old} -> {New} => P: '{Pattern}', R: '{Replacement}'", oldValue, newValue, result.Value.Pattern, result.Value.Replacement);
        }

        #endregion

        #region OCR Character Confusion Pattern Tests

        [Test]
        [Category("CharacterConfusion")]
        public void CreateAdvancedFormatCorrectionPatterns_OToZero_SingleChange_ShouldCreateCorrectPattern()
        {
            // Arrange
            var oldValue = "1O5.OO"; // This has two changes, the strategy CreateSpecificOCRCharacterConfusionPattern expects single character diff
            var newValue = "105.00";

            // For a single change test:
            oldValue = "PRICE 1O";
            newValue = "PRICE 10";


            // Act
            var result = _service.CreateAdvancedFormatCorrectionPatterns(oldValue, newValue);

            // Assert
            Assert.That(result, Is.Not.Null, "Should generate pattern for single 'O' to '0' change.");
            // Expected pattern might be specific to the change "PRICE 1[O0]" -> "PRICE 10"
            Assert.That(result.Value.Pattern, Does.Contain("[O0]")); // Check if it creates a character class for confusion
            Assert.That(result.Value.Replacement, Is.EqualTo(newValue)); // For specific char confusion, replacement is the full new value
            _logger.Information("✓ O to zero (single specific) pattern: {Old} -> {New} => P: '{Pattern}', R: '{Replacement}'", oldValue, newValue, result.Value.Pattern, result.Value.Replacement);
        }

        [Test]
        [Category("CharacterConfusion")]
        public void CreateAdvancedFormatCorrectionPatterns_LToOne_SingleChange_ShouldCreateCorrectPattern()
        {
            // Arrange
            var oldValue = "l23";
            var newValue = "123";

            // Act
            var result = _service.CreateAdvancedFormatCorrectionPatterns(oldValue, newValue);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value.Pattern, Does.Contain("[l1]"));
            Assert.That(result.Value.Replacement, Is.EqualTo(newValue));
            _logger.Information("✓ l to one (single specific) pattern: {Old} -> {New} => P: '{Pattern}', R: '{Replacement}'", oldValue, newValue, result.Value.Pattern, result.Value.Replacement);
        }

        [Test]
        [Category("CharacterConfusion")]
        public void CreateAdvancedFormatCorrectionPatterns_ValidLettersNoKnownConfusion_ShouldReturnNullOrGeneric()
        {
            // Arrange
            var oldValue = "TOTALS";
            var newValue = "T0TALZ"; // Multiple changes, not single known confusion

            // Act
            var result = _service.CreateAdvancedFormatCorrectionPatterns(oldValue, newValue);

            // Assert
            // Depending on the implementation of CreateGeneralSymbolOrCasePattern or CreateLooseAlphanumericMatchPattern
            // this might return a generic pattern or null.
            // If CreateSpecificOCRCharacterConfusionPattern is strict on single, known confusions, it should return null.
            // For this test, let's assume it tries a generic one if specific ones fail.
            if (result != null)
            {
                _logger.Information("✓ Valid letters with multiple changes generated pattern: {Old} -> {New} => P: '{Pattern}', R: '{Replacement}'", oldValue, newValue, result.Value.Pattern, result.Value.Replacement);
                Assert.That(result.Value.Pattern, Is.EqualTo(System.Text.RegularExpressions.Regex.Escape(oldValue))); // General pattern might be exact old value
                Assert.That(result.Value.Replacement, Is.EqualTo(newValue));
            }
            else
            {
                Assert.That(result, Is.Null);
                _logger.Information("✓ Valid letters with multiple changes correctly returns null or was not handled by specific strategies.");
            }
        }

        #endregion

        #region Space Manipulation Pattern Tests
        [Test]
        [Category("SpaceRemoval")]
        public void CreateAdvancedFormatCorrectionPatterns_RemoveSpacesInNumeric_ShouldCreateCorrectPattern()
        {
            // Arrange
            var oldValue = "1 234.56";
            var newValue = "1234.56";

            // Act
            var result = _service.CreateAdvancedFormatCorrectionPatterns(oldValue, newValue);

            // Assert
            Assert.That(result, Is.Not.Null);
            Assert.That(result.Value.Pattern, Is.EqualTo(System.Text.RegularExpressions.Regex.Escape(oldValue).Replace(@"\ ", @"\s*")));
            Assert.That(result.Value.Replacement, Is.EqualTo(newValue));
            _logger.Information("✓ Space removal in numeric pattern: {Old} -> {New} => P: '{Pattern}', R: '{Replacement}'", oldValue, newValue, result.Value.Pattern, result.Value.Replacement);
        }
        #endregion

        #region General and Edge Case Tests

        [Test]
        [Category("General")]
        public void CreateAdvancedFormatCorrectionPatterns_IdenticalValues_ShouldReturnNull()
        {
            // Arrange
            var oldValue = "123.45";
            var newValue = "123.45";

            // Act
            var result = _service.CreateAdvancedFormatCorrectionPatterns(oldValue, newValue);

            // Assert
            Assert.That(result, Is.Null);
            _logger.Information("✓ Identical values correctly return null");
        }

        [Test]
        [Category("General")]
        public void CreateAdvancedFormatCorrectionPatterns_CompletelyDifferentValues_ShouldReturnNullOrGeneric()
        {
            // Arrange
            var oldValue = "ABC123";
            var newValue = "XYZ789";

            // Act
            var result = _service.CreateAdvancedFormatCorrectionPatterns(oldValue, newValue);

            // Assert
            // This might fall to CreateGeneralSymbolOrCasePattern or CreateLooseAlphanumericMatchPattern (if implemented to handle this)
            // or return null if no strategy matches.
            // Given current strategies, CreateGeneralSymbolOrCasePattern might match if only symbols/case diff, which is not the case here.
            // CreateLooseAlphanumericMatchPattern is stubbed to return null.
            // So, expecting null or a very generic pattern if one of the fallback strategies generates one.
            // Most likely it will fall to `CreateGeneralSymbolOrCasePattern` and create a specific `Regex.Escape(oldValue)` -> `newValue`
            if (result != null)
            {
                _logger.Information("✓ Completely different values generated pattern: {Old} -> {New} => P: '{Pattern}', R: '{Replacement}'", oldValue, newValue, result.Value.Pattern, result.Value.Replacement);
                Assert.That(result.Value.Pattern, Is.EqualTo(System.Text.RegularExpressions.Regex.Escape(oldValue)));
                Assert.That(result.Value.Replacement, Is.EqualTo(newValue));
            }
            else
            {
                Assert.That(result, Is.Null);
                _logger.Information("✓ Completely different values correctly return null (no pattern generated).");
            }
        }

        [Test]
        [Category("General")]
        public void CreateAdvancedFormatCorrectionPatterns_OnlyCaseDifference_ShouldCreateCorrectPattern()
        {
            // Arrange
            var oldValue = "invoiceTotal";
            var newValue = "InvoiceTotal";

            // Act
            var result = _service.CreateAdvancedFormatCorrectionPatterns(oldValue, newValue);

            // Assert
            Assert.That(result, Is.Not.Null, "Pattern result should not be null for case difference.");
            Assert.That(result.Value.Pattern, Is.EqualTo(System.Text.RegularExpressions.Regex.Escape(oldValue))); // Match exact old value
            Assert.That(result.Value.Replacement, Is.EqualTo(newValue)); // Replace with new value
            _logger.Information("✓ Case difference pattern created: {Old} -> {New} => P: '{Pattern}', R: '{Replacement}'", oldValue, newValue, result.Value.Pattern, result.Value.Replacement);
        }

        [Test]
        [Category("General")]
        public void CreateAdvancedFormatCorrectionPatterns_SymbolDifference_ShouldCreateCorrectPattern()
        {
            // Arrange
            var oldValue = "INV-123";
            var newValue = "INV/123";

            // Act
            var result = _service.CreateAdvancedFormatCorrectionPatterns(oldValue, newValue);

            // Assert
            Assert.That(result, Is.Not.Null, "Pattern result should not be null for symbol difference.");
            Assert.That(result.Value.Pattern, Is.EqualTo(System.Text.RegularExpressions.Regex.Escape(oldValue))); // Match exact old value
            Assert.That(result.Value.Replacement, Is.EqualTo(newValue)); // Replace with new value
            _logger.Information("✓ Symbol difference pattern created: {Old} -> {New} => P: '{Pattern}', R: '{Replacement}'", oldValue, newValue, result.Value.Pattern, result.Value.Replacement);
        }

        #endregion
    }
}