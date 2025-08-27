using System;
using System.Text.RegularExpressions;
using NUnit.Framework;
using Serilog;

namespace AutoBotUtilities.Tests
{
    /// <summary>
    /// Test corrected regex patterns against actual MANGO OCR text to verify they would work
    /// </summary>
    [TestFixture]
    public class RegexPatternTest
    {
        private static ILogger _logger;

        [OneTimeSetUp]
        public void Setup()
        {
            _logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
        }

        [Test]
        public void TestCorrectedRegexPatterns_ShouldMatchMangoOCRText()
        {
            _logger.Information("🧪 **REGEX_PATTERN_TEST**: Testing corrected patterns against actual MANGO OCR text");

            // Actual MANGO OCR text samples from 03152025_TOTAL AMOUNT.pdf.txt
            string orderText = "You will receive your order UCSJB6 shortly";
            string supplierText = "From: MANGO OUTLET (noreply@mango.com)";
            string dateText = "Date: Tuesday, July 23, 2024 at 03:42 PM EDT";
            string subtotalText = "Subtotal USS 196.33";
            string totalText = "TOTAL AMOUNT US$ 210.08";
            string taxText = "Estimated Tax US$ 13.74";

            _logger.Information("=== TESTING CORRECTED PATTERNS (SHOULD MATCH) ===");
            
            // Test corrected patterns
            Assert.That(TestPattern("InvoiceNo", @"order\s+(?<InvoiceNo>[A-Za-z0-9]+)\s+shortly", orderText, "UCSJB6"), Is.True);
            Assert.That(TestPattern("SupplierName", @"(?<SupplierName>MANGO\s+OUTLET)", supplierText, "MANGO OUTLET"), Is.True);
            Assert.That(TestPattern("InvoiceDate", @"(?<InvoiceDate>\w+,\s+\w+\s+\d{1,2},\s+\d{4})", dateText, "Tuesday, July 23, 2024"), Is.True);
            Assert.That(TestPattern("SubTotal", @"Subtotal\s+US[S$]\s*(?<SubTotal>\d+\.\d{2})", subtotalText, "196.33"), Is.True);
            Assert.That(TestPattern("InvoiceTotal", @"TOTAL\s+AMOUNT\s+US\$\s*(?<InvoiceTotal>\d+\.\d{2})", totalText, "210.08"), Is.True);
            Assert.That(TestPattern("TotalOtherCost", @"Estimated\s+Tax\s+US\$\s*(?<TotalOtherCost>\d+\.\d{2})", taxText, "13.74"), Is.True);
            
            _logger.Information("=== TESTING ORIGINAL DEEPSEEK PATTERNS (SHOULD FAIL) ===");
            
            // Test original DeepSeek patterns (should fail)
            Assert.That(TestPattern("InvoiceNo (Original)", @"Invoice No\.\s*(?<InvoiceNo>[A-Za-z0-9]+)", orderText, null), Is.False);
            Assert.That(TestPattern("SupplierName (Original)", @"OUTLET\s*(?<SupplierName>MANGO\s*OUTLET)", supplierText, null), Is.False);
            Assert.That(TestPattern("InvoiceDate (Original)", @"Date:\s*(?<InvoiceDate>\d{4}-\d{2}-\d{2})", dateText, null), Is.False);
            Assert.That(TestPattern("SubTotal (Original)", @"Subtotal\s*US\$\s*(?<SubTotal>\d+\.\d{2})", subtotalText, null), Is.False);
            Assert.That(TestPattern("InvoiceTotal (Original)", @"Order total\s*US\$\s*(?<InvoiceTotal>\d+\.\d{2})", totalText, null), Is.False);

            _logger.Information("✅ **REGEX_PATTERN_TEST_COMPLETE**: All tests passed - corrected patterns work, original patterns fail as expected");
        }
        
        private bool TestPattern(string fieldName, string pattern, string text, string expectedValue)
        {
            try
            {
                var match = Regex.Match(text, pattern, RegexOptions.IgnoreCase);
                if (match.Success)
                {
                    var extractedValue = match.Groups.Count > 1 ? match.Groups[1].Value : match.Value;
                    _logger.Information($"✅ {fieldName}: MATCHED -> '{extractedValue}'");
                    
                    if (expectedValue != null)
                    {
                        return extractedValue == expectedValue;
                    }
                    return true;
                }
                else
                {
                    _logger.Information($"❌ {fieldName}: NO MATCH");
                    return false;
                }
            }
            catch (Exception ex)
            {
                _logger.Error($"🚨 {fieldName}: ERROR -> {ex.Message}");
                return false;
            }
        }
    }
}