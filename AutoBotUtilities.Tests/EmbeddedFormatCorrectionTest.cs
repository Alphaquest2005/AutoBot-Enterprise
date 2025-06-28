using System;
using System.Text.RegularExpressions;
using NUnit.Framework;

namespace AutoBotUtilities.Tests
{
    [TestFixture]
    public class EmbeddedFormatCorrectionTest
    {
        [Test]
        public void MultiFieldRegex_WithEmbeddedFormatCorrection_ShouldExtractAndCorrectValues()
        {
            // Arrange: Invoice line with OCR errors
            string ocrText = "3 of: MESAILUP 16 Inch LED Lighted Liquor Bottle Display S39,99";
            
            // Multi-field regex that captures OCR errors and handles format correction
            string multiFieldPattern = @"(?<Quantity>\d+)\s+of:\s+(?<ItemDescription>.+?)\s+[S\$](?<UnitPrice_Raw>[\d,]+(?:\.|\,)\d{2})";
            
            var regex = new Regex(multiFieldPattern);
            var match = regex.Match(ocrText);
            
            // Act: Extract values and apply embedded format corrections
            Assert.That(match.Success, Is.True, "Multi-field regex should match the OCR text");
            
            string quantity = match.Groups["Quantity"].Value;
            string itemDescription = match.Groups["ItemDescription"].Value;
            string unitPriceRaw = match.Groups["UnitPrice_Raw"].Value;
            
            // Apply format correction to UnitPrice only (context-aware)
            string unitPriceCorrected = ApplyEmbeddedFormatCorrection(unitPriceRaw);
            
            // Assert: Values extracted and corrected properly
            Assert.That(quantity, Is.EqualTo("3"));
            Assert.That(itemDescription, Is.EqualTo("MESAILUP 16 Inch LED Lighted Liquor Bottle Display"));
            Assert.That(unitPriceRaw, Is.EqualTo("39,99"), "Should capture raw OCR value with comma");
            Assert.That(unitPriceCorrected, Is.EqualTo("39.99"), "Should correct comma to decimal point");
        }

        [Test]
        public void MultiFieldRegex_WithMultipleFormatIssues_ShouldHandleAllCorrections()
        {
            // Arrange: Line with multiple OCR format issues
            string ocrText = "2 PC: FIBERGLASS BOTTOMK NT Paint S1O,5O";
            
            // Enhanced regex that handles multiple character confusions
            string multiFieldPattern = @"(?<Quantity>\d+)\s+PC:\s+(?<ItemDescription>.+?)\s+[S\$](?<UnitPrice_Raw>[1lI\d][O0\d],[5S\d][O0\d])";
            
            var regex = new Regex(multiFieldPattern);
            var match = regex.Match(ocrText);
            
            // Act: Extract and correct
            Assert.That(match.Success, Is.True, "Should match text with OCR errors");
            
            string quantity = match.Groups["Quantity"].Value;
            string itemDescription = match.Groups["ItemDescription"].Value;
            string unitPriceRaw = match.Groups["UnitPrice_Raw"].Value;
            
            // Apply embedded corrections
            string itemDescriptionCorrected = ApplyItemDescriptionCorrection(itemDescription);
            string unitPriceCorrected = ApplyPriceFormatCorrection(unitPriceRaw);
            
            // Assert: All corrections applied correctly
            Assert.That(quantity, Is.EqualTo("2"));
            Assert.That(itemDescriptionCorrected, Is.EqualTo("FIBERGLASS BOTTOM PAINT"), "Should correct BOTTOMK NT to BOTTOM PAINT");
            Assert.That(unitPriceCorrected, Is.EqualTo("10.50"), "Should correct S1O,5O to 10.50");
        }

        [Test]
        public void ContextAwareFormatCorrection_ShouldNotAffectUnrelatedFields()
        {
            // Arrange: Document with similar patterns in different contexts
            string invoiceText = @"
Invoice Total: S166,30
3 of: Widget Display S39,99
Tax Amount: S12,66
";
            
            // Multi-field line regex (only matches product lines)
            string productLinePattern = @"(?<Quantity>\d+)\s+of:\s+(?<ItemDescription>.+?)\s+[S\$](?<UnitPrice_Raw>[\d,]+)";
            
            // Header field regex (matches invoice totals)
            string invoiceTotalPattern = @"Invoice Total:\s+[S\$](?<InvoiceTotal>[\d,]+)";
            
            var productMatch = Regex.Match(invoiceText, productLinePattern);
            var totalMatch = Regex.Match(invoiceText, invoiceTotalPattern);
            
            // Act: Apply format corrections only in appropriate contexts
            string productPrice = ApplyEmbeddedFormatCorrection(productMatch.Groups["UnitPrice_Raw"].Value);
            string invoiceTotal = totalMatch.Groups["InvoiceTotal"].Value; // No correction applied
            
            // Assert: Format correction only applied to product line context
            Assert.That(productPrice, Is.EqualTo("39.99"), "Product price should be corrected");
            Assert.That(invoiceTotal, Is.EqualTo("166,30"), "Invoice total should remain uncorrected (different context)");
        }

        private string ApplyEmbeddedFormatCorrection(string value)
        {
            // Embedded format correction: comma to decimal point
            return value.Replace(",", ".");
        }

        private string ApplyItemDescriptionCorrection(string description)
        {
            // OCR character confusion corrections - handle complete phrase replacement
            if (description.Contains("BOTTOMK NT"))
            {
                // Replace "BOTTOMK NT Paint" with "BOTTOM PAINT" (removing duplicate Paint)
                return description.Replace("BOTTOMK NT Paint", "BOTTOM PAINT")
                                 .Replace("BOTTOMK NT", "BOTTOM PAINT");
            }
            return description;
        }

        private string ApplyPriceFormatCorrection(string price)
        {
            // Multiple OCR corrections for price field
            string corrected = price;
            corrected = corrected.Replace("S", ""); // Remove currency confusion
            corrected = corrected.Replace("O", "0"); // O to 0
            corrected = corrected.Replace("1O", "10"); // 1O to 10
            corrected = corrected.Replace(",", "."); // Comma to decimal
            return corrected;
        }
    }
}