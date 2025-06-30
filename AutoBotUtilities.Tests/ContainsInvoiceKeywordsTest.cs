using System;
using System.IO;
using NUnit.Framework;
using Serilog;
using WaterNut.DataSpace.PipelineInfrastructure;

namespace AutoBotUtilities.Tests
{
    /// <summary>
    /// Tests the ContainsInvoiceKeywords method to verify invoice content detection logic.
    /// This is critical for hybrid document processing where OCR template creation depends on keyword detection.
    /// </summary>
    [TestFixture]
    public class ContainsInvoiceKeywordsTest
    {
        private ILogger _logger;
        private const string TestDataPath = @"C:\Insight Software\AutoBot-Enterprise\AutoBotUtilities.Tests\Test Data";

        [SetUp]
        public void Setup()
        {
            _logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .WriteTo.NUnitOutput()
                .CreateLogger();
        }

        /// <summary>
        /// 🎯 **MANGO_KEYWORD_DETECTION**: Tests why MANGO content fails invoice keyword detection
        /// despite containing clear invoice keywords like "TOTAL AMOUNT", "Subtotal", etc.
        /// </summary>
        [Test]
        public void ContainsInvoiceKeywords_WithMangoContent_ShouldDetectInvoiceKeywords()
        {
            _logger.Information("🔍 **MANGO_KEYWORD_TEST_START**: Testing invoice keyword detection with actual MANGO content");

            // Load actual MANGO OCR text
            var mangoTextPath = Path.Combine(TestDataPath, "03152025_TOTAL AMOUNT.pdf.txt");
            
            if (!File.Exists(mangoTextPath))
            {
                Assert.Fail($"❌ **TEST_DATA_MISSING**: MANGO test file not found at {mangoTextPath}");
                return;
            }

            var mangoText = File.ReadAllText(mangoTextPath);
            _logger.Information("📄 **MANGO_TEXT_LOADED**: {CharCount} characters loaded", mangoText.Length);

            // Create test instance to access the ContainsInvoiceKeywords method
            var testInstance = new TestableGetPossibleInvoicesStep();
            
            // Test the method with MANGO content
            _logger.Information("🚀 **TESTING_KEYWORD_DETECTION**: Calling ContainsInvoiceKeywords with MANGO content");
            var result = testInstance.TestContainsInvoiceKeywords(mangoText, _logger);
            
            _logger.Information("🎯 **KEYWORD_DETECTION_RESULT**: {Result}", result);

            // Verify that MANGO content is detected as containing invoice keywords
            Assert.That(result, Is.True, 
                "MANGO content should be detected as containing invoice keywords - it has 'TOTAL AMOUNT', 'Subtotal', 'Shipping & Handling', 'Estimated Tax'");
        }

        /// <summary>
        /// 🧪 **KEYWORD_VALIDATION**: Tests individual keywords that should be found in MANGO content
        /// </summary>
        [Test]
        public void ContainsInvoiceKeywords_MangoSpecificKeywords_ShouldFindMultipleMatches()
        {
            _logger.Information("🔍 **SPECIFIC_KEYWORD_TEST**: Testing specific MANGO keywords");

            // Sample MANGO content with known keywords
            var mangoSample = @"
High-waist straight shorts
Subtotal USS 196.33
Shipping & Handling Free
Estimated Tax US$ 13.74
TOTAL AMOUNT US$ 210.08
Order number: UCSJIB6
";

            var testInstance = new TestableGetPossibleInvoicesStep();
            var result = testInstance.TestContainsInvoiceKeywords(mangoSample, _logger);

            _logger.Information("🎯 **SAMPLE_TEST_RESULT**: {Result}", result);

            // This should definitely pass - contains "Subtotal", "TOTAL AMOUNT", "Shipping", "Order number:"
            Assert.That(result, Is.True, 
                "Sample MANGO content with clear keywords should be detected as invoice content");
        }

        /// <summary>
        /// 🚫 **NEGATIVE_TEST**: Ensures non-invoice content is not detected as invoice content
        /// </summary>
        [Test]
        public void ContainsInvoiceKeywords_WithNonInvoiceContent_ShouldReturnFalse()
        {
            _logger.Information("🚫 **NEGATIVE_TEST**: Testing with non-invoice content");

            var nonInvoiceText = @"
This is just some random text
about a product description
with no invoice keywords
just regular content here
";

            var testInstance = new TestableGetPossibleInvoicesStep();
            var result = testInstance.TestContainsInvoiceKeywords(nonInvoiceText, _logger);

            _logger.Information("🎯 **NEGATIVE_TEST_RESULT**: {Result}", result);

            // This should return false
            Assert.That(result, Is.False, 
                "Non-invoice content should not be detected as invoice content");
        }

        /// <summary>
        /// 🔧 **EDGE_CASE_TESTS**: Tests edge cases like empty/null content
        /// </summary>
        [Test]
        public void ContainsInvoiceKeywords_EdgeCases_ShouldHandleGracefully()
        {
            _logger.Information("🔧 **EDGE_CASE_TESTS**: Testing null and empty content");

            var testInstance = new TestableGetPossibleInvoicesStep();

            // Test null content
            var nullResult = testInstance.TestContainsInvoiceKeywords(null, _logger);
            Assert.That(nullResult, Is.False, "Null content should return false");

            // Test empty content
            var emptyResult = testInstance.TestContainsInvoiceKeywords("", _logger);
            Assert.That(emptyResult, Is.False, "Empty content should return false");

            // Test whitespace only
            var whitespaceResult = testInstance.TestContainsInvoiceKeywords("   \n\t  ", _logger);
            Assert.That(whitespaceResult, Is.False, "Whitespace-only content should return false");

            _logger.Information("✅ **EDGE_CASES_PASSED**: All edge cases handled correctly");
        }
    }

    /// <summary>
    /// Test wrapper class to access the private ContainsInvoiceKeywords method
    /// </summary>
    public class TestableGetPossibleInvoicesStep : GetPossibleInvoicesStep
    {
        /// <summary>
        /// Public wrapper to test the private ContainsInvoiceKeywords method
        /// </summary>
        public bool TestContainsInvoiceKeywords(string pdfText, ILogger logger = null)
        {
            // Use reflection to access the private method
            var method = typeof(GetPossibleInvoicesStep).GetMethod("ContainsInvoiceKeywords", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);
            
            if (method == null)
            {
                throw new InvalidOperationException("ContainsInvoiceKeywords method not found - check method name and access level");
            }

            return (bool)method.Invoke(this, new object[] { pdfText, logger });
        }
    }
}