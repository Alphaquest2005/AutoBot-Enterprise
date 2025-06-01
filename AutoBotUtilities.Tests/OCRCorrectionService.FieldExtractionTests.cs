using System;
using System.Linq;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace AutoBotUtilities.Tests.Production
{
    /// <summary>
    /// Tests for field extraction pattern creation in OCR Correction Service
    /// </summary>
    public partial class OCRCorrectionService_ProductionTests
    {
        [Test]
        public void CreateFieldExtractionPatterns_NumericField_ShouldCreateMonetaryPatterns()
        {
            // Arrange
            var fieldName = "InvoiceTotal";
            var sampleValues = new[] { "123.45", "678.90" };

            // Act - Call the actual method
            var patterns = _service.CreateFieldExtractionPatterns(fieldName, sampleValues);

            // Assert
            Assert.That(patterns, Is.Not.Empty, "Should create patterns");
            Assert.That(patterns.Any(p => p.Contains("InvoiceTotal")), Is.True, "Should include field name in patterns");
            Assert.That(patterns.Any(p => p.Contains(Regex.Escape("123.45"))), Is.True, "Should include sample values in patterns");

            // Test pattern effectiveness
            var testText = "Invoice Total: 123.45";
            var anyPatternMatches = patterns.Any(p => Regex.IsMatch(testText, p));
            Assert.That(anyPatternMatches, Is.True, "At least one pattern should match test text");
        }

        [Test]
        public void CreateFieldExtractionPatterns_DateField_ShouldCreateDatePatterns()
        {
            // Arrange
            var fieldName = "InvoiceDate";
            var sampleValues = new[] { "2023-01-15", "Jan 15, 2023" };

            // Act - Call the actual method
            var patterns = _service.CreateFieldExtractionPatterns(fieldName, sampleValues);

            // Assert
            Assert.That(patterns, Is.Not.Empty, "Should create patterns");

            // Test pattern effectiveness
            var testText1 = "Invoice Date: 2023-01-15";
            var testText2 = "Date of Invoice: Jan 15, 2023";

            var anyPatternMatchesText1 = patterns.Any(p => Regex.IsMatch(testText1, p));
            var anyPatternMatchesText2 = patterns.Any(p => Regex.IsMatch(testText2, p));

            Assert.That(anyPatternMatchesText1, Is.True, "At least one pattern should match ISO date format");
            Assert.That(anyPatternMatchesText2, Is.True, "At least one pattern should match text date format");
        }
    }
}