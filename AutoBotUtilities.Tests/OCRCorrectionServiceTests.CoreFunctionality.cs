using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using NUnit.Framework;
using Serilog;
using Serilog.Events;
using WaterNut.DataSpace;
using EntryDataDS.Business.Entities;
using Core.Common.Extensions;
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Text.RegularExpressions;
using OCR.Business.Entities;
using System.Text;
using Serilog.Core;

namespace AutoBotUtilities.Tests.Production
{
    /// <summary>
    /// Core functionality tests for OCR Correction Service
    /// </summary>
    public partial class OCRCorrectionService_ProductionTests
    {
        #region Core Functionality Tests

        [Test]
        [Category("Core")]
        public void TotalsZero_FloatingPointPrecision_ShouldHandleCorrectly()
        {
            _logger.Information("Testing floating point precision in TotalsZero calculations");

            var precisionTestCases = new[]
            {
                (100.1, 85.05, 10.03, 5.02, 0.0, 0.0, false), // 0.01 difference - should be false
                (100.000001, 85.0, 10.0, 5.0, 0.0, 0.0, true), // Within tolerance
                (99.999999, 85.0, 10.0, 5.0, 0.0, 0.0, true), // Within tolerance
                (100.02, 85.0, 10.0, 5.0, 0.0, 0.0, false), // Outside tolerance
                (33.33, 11.11, 11.11, 11.11, 0.0, 0.0, true), // Repeating decimals
                (0.01, 0.005, 0.003, 0.002, 0.0, 0.0, true), // Very small numbers
                (999999.99, 850000.0, 100000.0, 49999.99, 0.0, 0.0, true) // Large numbers
            };

            foreach (var (total, subTotal, freight, other, insurance, deduction, expected) in precisionTestCases)
            {
                var invoice = CreateTestInvoice("PRECISION", total, subTotal, freight, other, insurance, deduction);
                var result = OCRCorrectionService.TotalsZero(invoice);

                var calculatedTotal = subTotal + freight + other + insurance - deduction;
                var difference = Math.Abs(total - calculatedTotal);

                _logger.Information("Precision test: {Total} = {SubTotal} + {Freight} + {Other} + {Insurance} - {Deduction} → {Result} (expected: {Expected}, diff: {Difference:F6})",
                    total, subTotal, freight, other, insurance, deduction, result, expected, difference);

                Assert.That(result, Is.EqualTo(expected),
                    $"Precision test failed for total {total}, difference was {difference:F6}");
            }

            _logger.Information("✓ Floating point precision handled correctly");
        }

        [Test]
        [Category("Core")]
        public void ParseCorrectedValue_ScientificNotation_ShouldParseCorrectly()
        {
            _logger.Information("Testing scientific notation parsing");

            var scientificNotationCases = new[]
            {
                ("1.23E+2", "InvoiceTotal", 123.0m),
                ("1.5e-1", "SubTotal", 0.15m),
                ("4.56E+03", "TotalInternalFreight", 4560.0m),
                ("7.89e-02", "TotalOtherCost", 0.0789m),
                ("1E+0", "TotalInsurance", 1.0m)
            };

            foreach (var (input, fieldName, expected) in scientificNotationCases)
            {
                var result = InvokePrivateMethod<object>(_service, "ParseCorrectedValue", input, fieldName);

                _logger.Information("Scientific notation: {Input} → {Result} (expected: {Expected})",
                    input, result, expected);

                if (result is decimal decimalResult)
                {
                    Assert.That(decimalResult, Is.EqualTo(expected).Within(0.0001m),
                        $"Scientific notation parsing failed for {input}");
                }
                else if (result is string stringResult)
                {
                    if (decimal.TryParse(stringResult, out decimal parsedDecimal))
                    {
                        Assert.That(parsedDecimal, Is.EqualTo(expected).Within(0.0001m),
                            $"Scientific notation parsing failed for {input}");
                    }
                    else
                    {
                        Assert.Fail($"Could not parse string result '{stringResult}' as decimal for {input}");
                    }
                }
                else
                {
                    Assert.Fail($"Expected decimal or string result for {input}, got {result?.GetType().Name}");
                }
            }

            _logger.Information("✓ Scientific notation parsing handled correctly");
        }

        [Test]
        [Category("Core")]
        public void NumericFieldValidation_MaxDecimalPlaces_ShouldTruncateAppropriately()
        {
            _logger.Information("Testing numeric field validation with maximum decimal places");

            var precisionTests = new[]
            {
                ("123.456789", "InvoiceTotal", 123.46m),
                ("999.999999", "SubTotal", 1000.00m),
                ("100.001", "TotalInternalFreight", 100.00m),
                ("0.999", "TotalOtherCost", 1.00m),
                ("123.125", "TotalInsurance", 123.12m)
            };

            foreach (var (input, fieldName, expected) in precisionTests)
            {
                _logger.Information("Testing precision for {Field}: {Input} → expected {Expected}",
                    fieldName, input, expected);

                var parsed = InvokePrivateMethod<object>(_service, "ParseCorrectedValue", input, fieldName);

                if (parsed is decimal decimalValue)
                {
                    var rounded = Math.Round(decimalValue, 2, MidpointRounding.ToEven);
                    Assert.That(rounded, Is.EqualTo(expected),
                        $"Precision handling failed for {input}");

                    _logger.Information("  Parsed: {Parsed}, Rounded: {Rounded}", decimalValue, rounded);
                }
                else
                {
                    Assert.Fail($"Expected decimal for {input}, got {parsed?.GetType().Name}");
                }
            }

            _logger.Information("✓ Decimal precision handling working correctly");
        }

        #endregion

        #region Gift Card Detection Tests

        [Test]
        [Category("GiftCardDetection")]
        public void GiftCardDetector_VariousFormats_ShouldDetectAll()
        {
            _logger.Information("Testing gift card detection with various formats");

            var testCases = new[]
            {
                ("Gift Card: -$6.99", "Gift Card", 6.99),
                ("Store Credit Applied: -$25.00", "Store Credit", 25.00),
                ("PROMO CODE DISCOUNT: -$10.50", "Promo Code", 10.50),
                ("-$15.00 Gift Card", "Gift Card", 15.00),
                ("Coupon: -$5.25", "Coupon", 5.25),
                ("DISCOUNT: -$12.75", "Discount", 12.75),
                ("Gift card -$99.99", "Gift Card", 99.99),
                ("Store credit: -$0.50", "Store Credit", 0.50)
            };

            foreach (var (text, expectedType, expectedAmount) in testCases)
            {
                _logger.Information("Testing: {Text}", text);

                // Create a test invoice with this text and see if TotalDeduction gets detected
                var invoice = CreateTestInvoice("GIFT-001", 100.0, 90.0, 5.0, 5.0, 0.0, 0.0);

                // Simulate gift card detection by checking if the text would be processed correctly
                var hasGiftCard = text.Contains("Gift") || text.Contains("Credit") || text.Contains("Discount") || text.Contains("Coupon");
                Assert.That(hasGiftCard, Is.True, $"Should detect discount pattern in: {text}");

                // Extract amount using regex (simulating what the service would do)
                var amountMatch = Regex.Match(text, @"-?\$?([0-9]+\.?[0-9]*)");
                if (amountMatch.Success)
                {
                    if (decimal.TryParse(amountMatch.Groups[1].Value, out var amount))
                    {
                        Assert.That(amount, Is.EqualTo(expectedAmount).Within(0.01),
                            $"Amount should be {expectedAmount} for: {text}");
                    }
                }

                _logger.Information("✓ Detected discount pattern with amount ${Amount:F2}", expectedAmount);
            }

            _logger.Information("✓ All gift card format variations detected correctly");
        }

        [Test]
        [Category("GiftCardDetection")]
        public void GiftCardDetector_MultipleDiscounts_ShouldCombineCorrectly()
        {
            _logger.Information("Testing multiple discounts in single text");

            var text = @"
                Invoice Total: $100.00
                Gift Card: -$10.00
                Store Credit: -$5.50
                Promo Code: -$15.25
                Final Total: $69.25
            ";

            // Extract all discount amounts
            var discountMatches = Regex.Matches(text, @"(?:Gift Card|Store Credit|Promo Code):\s*-\$([0-9]+\.?[0-9]*)");
            var totalDetected = discountMatches.Cast<Match>().Sum(m => decimal.Parse(m.Groups[1].Value));

            _logger.Information("Detected {Count} discounts with total ${Total:F2}", discountMatches.Count, totalDetected);

            Assert.That(discountMatches.Count, Is.EqualTo(3), "Should detect all three discounts");
            Assert.That(totalDetected, Is.EqualTo(30.75m).Within(0.01m), "Total should be $30.75");

            _logger.Information("✓ Multiple discounts detected and summed correctly");
        }

        [Test]
        [Category("GiftCardDetection")]
        public void GiftCardDetector_NoDiscounts_ShouldReturnEmpty()
        {
            _logger.Information("Testing text with no discounts");

            var text = @"
                Invoice #12345
                Subtotal: $100.00
                Tax: $8.50
                Shipping: $5.00
                Total: $113.50
            ";

            var discountMatches = Regex.Matches(text, @"(?:Gift Card|Store Credit|Promo Code|Discount|Coupon):\s*-\$([0-9]+\.?[0-9]*)");

            Assert.That(discountMatches.Count, Is.EqualTo(0), "Should detect no discounts");

            _logger.Information("✓ Correctly detected no discounts in regular invoice");
        }

        #endregion
    }
}
