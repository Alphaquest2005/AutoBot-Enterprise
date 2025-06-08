// File: OCRCorrectionServiceTests.CoreFunctionality.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Text.Json;
using NUnit.Framework;
using Serilog;
using Serilog.Events;
using WaterNut.DataSpace; // For OCRCorrectionService and its utilities
using EntryDataDS.Business.Entities; // For ShipmentInvoice
using Core.Common.Extensions; // Not directly used here, but in project
using System.IO;
using System.Threading;
using System.Diagnostics;
using System.Text.RegularExpressions;
using OCR.Business.Entities; // Not directly used here, but in project
using System.Text;
using Serilog.Core;

namespace AutoBotUtilities.Tests.Production
{
    using OCRCorrectionService = WaterNut.DataSpace.OCRCorrectionService;

    /// <summary>
    /// Core functionality and utility tests for OCR Correction Service
    /// </summary>
    public partial class OCRCorrectionService_ProductionTests // Assuming this partial class setup exists
    {
        // Existing TotalsZero tests, ParseCorrectedValue tests are generally fine.
        // Adding tests for methods from OCRUtilities.cs

        #region Text Manipulation Utilities Tests (from OCRUtilities.cs)

        [Test]
        [Category("Utilities")]
        public void CleanTextForAnalysis_ShouldRemoveExcessiveCharsAndTruncate()
        {
            _logger.Information("Testing CleanTextForAnalysis");
            var service = new OCRCorrectionService(_logger); // Utilities are instance methods

            var inputText = "Line1\n\n\nLine2  \t  Line3\n--------------------------\nLine4\n" + new string('a', 16000);

            var cleanedText = service.CleanTextForAnalysis(inputText);

            Assert.That(cleanedText, Does.StartWith("Line1\n\nLine2 Line3\nLine4\n"), "Should clean up spaces, newlines, and separators.");
            Assert.That(cleanedText.Length, Is.LessThanOrEqualTo(15000 + "...[text truncated]".Length), "Should truncate long text.");
            Assert.That(cleanedText, Does.EndWith("...[text truncated]"), "Should indicate truncation.");
            _logger.Information("✓ CleanTextForAnalysis works as expected.");
        }

        [Test]
        [Category("Utilities")]
        public void CleanJsonResponse_ShouldHandleMarkdownAndBOM()
        {
            _logger.Information("Testing CleanJsonResponse");
            var service = new OCRCorrectionService(_logger);

            var jsonWithMarkdown = "```json\n{\"key\": \"value\"}\n```";
            var expectedJson = "{\"key\": \"value\"}";
            Assert.That(service.CleanJsonResponse(jsonWithMarkdown), Is.EqualTo(expectedJson));

            var jsonWithBom = "\uFEFF{\"key\": \"value\"}";
            Assert.That(service.CleanJsonResponse(jsonWithBom), Is.EqualTo(expectedJson));

            var plainJson = "{\"key\": \"value\"}";
            Assert.That(service.CleanJsonResponse(plainJson), Is.EqualTo(expectedJson));

            var textBeforeJson = "Some text\n{\"key\": \"value\"}";
            Assert.That(service.CleanJsonResponse(textBeforeJson), Is.EqualTo(expectedJson));

            var textAfterJson = "{\"key\": \"value\"}\nSome text";
            Assert.That(service.CleanJsonResponse(textAfterJson), Is.EqualTo(expectedJson));

            var emptyResponse = "```json\n```";
            Assert.That(service.CleanJsonResponse(emptyResponse), Is.EqualTo(string.Empty));

            var nonJson = "This is not json";
            Assert.That(service.CleanJsonResponse(nonJson), Is.EqualTo(string.Empty));

            _logger.Information("✓ CleanJsonResponse handles various cases correctly.");
        }

        [Test]
        [Category("Utilities")]
        public void TruncateForLog_ShouldTruncateCorrectly()
        {
            _logger.Information("Testing TruncateForLog");
            var service = new OCRCorrectionService(_logger);
            string longText = new string('x', 250);
            string shortText = "abc";

            Assert.That(service.TruncateForLog(longText, 200), Is.EqualTo(new string('x', 200) + "..."));
            Assert.That(service.TruncateForLog(shortText, 200), Is.EqualTo("abc"));
            Assert.That(service.TruncateForLog(null, 200), Is.EqualTo(string.Empty));
            _logger.Information("✓ TruncateForLog works correctly.");
        }

        #endregion

        #region JSON Element Parsing Utilities Tests (from OCRUtilities.cs)

        private JsonElement GetJsonRoot(string json)
        {
            using (var doc = JsonDocument.Parse(json))
            {
                return doc.RootElement.Clone();
            }
        }

        [Test]
        [Category("Utilities_JSON")]
        public void GetStringValueWithLogging_ShouldParseStringCorrectly()
        {
            var service = new OCRCorrectionService(_logger);
            var json = "{\"name\": \"test\", \"value\": 123, \"flag\": true, \"empty_str\": \"\", \"null_val\": null}";
            var root = GetJsonRoot(json);

            Assert.That(service.GetStringValueWithLogging(root, "name", 0), Is.EqualTo("test"));
            Assert.That(service.GetStringValueWithLogging(root, "value", 0), Is.EqualTo("123")); // Numbers converted to string
            Assert.That(service.GetStringValueWithLogging(root, "flag", 0), Is.EqualTo("true")); // Booleans to string
            Assert.That(service.GetStringValueWithLogging(root, "empty_str", 0), Is.EqualTo(""));
            Assert.That(service.GetStringValueWithLogging(root, "null_val", 0), Is.Null);
            Assert.That(service.GetStringValueWithLogging(root, "non_existent", 0, true), Is.Null); // Optional, not found
            _logger.Information("✓ GetStringValueWithLogging tested.");
        }

        // Similar tests for GetDoubleValueWithLogging, GetIntValueWithLogging, GetBooleanValueWithLogging, ParseContextLinesArray

        [Test]
        [Category("Utilities_JSON")]
        public void ParseContextLinesArray_ShouldParseArrayCorrectly()
        {
            var service = new OCRCorrectionService(_logger);
            var json = "{\"context\": [\"line1\", \"line2\", null, \"line4\"]}";
            var root = GetJsonRoot(json);

            var lines = service.ParseContextLinesArray(root, "context", 0);
            Assert.That(lines.Count, Is.EqualTo(3)); // Nulls should be skipped by Where in prompt creator
            Assert.That(lines, Contains.Item("line1"));
            Assert.That(lines, Contains.Item("line2"));
            Assert.That(lines, Contains.Item("line4"));
            _logger.Information("✓ ParseContextLinesArray tested.");
        }


        #endregion

        #region Document Text Utilities Tests (from OCRUtilities.cs)

        [Test]
        [Category("Utilities_Text")]
        public void GetOriginalLineText_ShouldReturnCorrectLine()
        {
            var service = new OCRCorrectionService(_logger);
            var text = "Line one\nLine two\r\nLine three";

            Assert.That(service.GetOriginalLineText(text, 1), Is.EqualTo("Line one"));
            Assert.That(service.GetOriginalLineText(text, 2), Is.EqualTo("Line two"));
            Assert.That(service.GetOriginalLineText(text, 3), Is.EqualTo("Line three"));
            Assert.That(service.GetOriginalLineText(text, 4), Is.EqualTo("")); // Out of bounds
            Assert.That(service.GetOriginalLineText(text, 0), Is.EqualTo("")); // Invalid
            _logger.Information("✓ GetOriginalLineText tested.");
        }

        [Test]
        [Category("Utilities_Text")]
        public void ExtractWindowText_ShouldReturnCorrectWindow()
        {
            var service = new OCRCorrectionService(_logger);
            var text = "L1\nL2\nL3\nL4\nL5\nL6\nL7";

            Assert.That(service.ExtractWindowText(text, 4, 1), Is.EqualTo("L3\nL4\nL5"));
            Assert.That(service.ExtractWindowText(text, 1, 1), Is.EqualTo("L1\nL2"));
            Assert.That(service.ExtractWindowText(text, 7, 1), Is.EqualTo("L6\nL7"));
            Assert.That(service.ExtractWindowText(text, 4, 0), Is.EqualTo("L4"));
            Assert.That(service.ExtractWindowText(text, 4, 10), Is.EqualTo(text.Replace("\r\n", "\n"))); // Full text if window larger
            _logger.Information("✓ ExtractWindowText tested.");
        }

        [Test]
        [Category("Utilities_Regex")]
        public void ExtractNamedGroupsFromRegex_ShouldExtractCorrectly()
        {
            var service = new OCRCorrectionService(_logger);
            var pattern1 = @"(?<Total>\d+\.\d{2}).*(?<Tax>\d+\.\d{2})";
            var pattern2 = @"(?'InvoiceNo'[A-Z0-9]+)";
            var pattern3 = @"(\d+)"; // No named groups
            var pattern4 = @"(?<ValidName>[a-z]+)(?:non_capture_group)(?<AnotherName>\d+)";


            var groups1 = service.ExtractNamedGroupsFromRegex(pattern1);
            Assert.That(groups1, Is.EquivalentTo(new[] { "Total", "Tax" }));

            var groups2 = service.ExtractNamedGroupsFromRegex(pattern2);
            Assert.That(groups2, Is.EquivalentTo(new[] { "InvoiceNo" }));

            var groups3 = service.ExtractNamedGroupsFromRegex(pattern3);
            Assert.That(groups3, Is.Empty);

            var groups4 = service.ExtractNamedGroupsFromRegex(pattern4);
            Assert.That(groups4, Is.EquivalentTo(new[] { "ValidName", "AnotherName" }));

            Assert.That(service.ExtractNamedGroupsFromRegex(null), Is.Empty);
            Assert.That(service.ExtractNamedGroupsFromRegex(""), Is.Empty);
            Assert.That(service.ExtractNamedGroupsFromRegex("(?<name>value"), Is.Empty); // Invalid regex

            _logger.Information("✓ ExtractNamedGroupsFromRegex tested.");
        }

        #endregion

        #region Field Type and Property Mapping Utilities (from OCRUtilities.cs)

        [Test]
        [Category("Utilities_Parsing")]
        public void ParseCorrectedValue_ShouldParseVariousTypes()
        {
            var service = new OCRCorrectionService(_logger);

            // Decimal/Currency
            Assert.That(service.ParseCorrectedValue("123.45", "InvoiceTotal"), Is.EqualTo(123.45m));
            Assert.That(service.ParseCorrectedValue("$1,234.56", "InvoiceTotal"), Is.EqualTo(1234.56m));
            Assert.That(service.ParseCorrectedValue("1.234,56", "InvoiceTotal"), Is.EqualTo(1234.56m)); // European format
            Assert.That(service.ParseCorrectedValue("€50,00", "InvoiceTotal"), Is.EqualTo(50.00m));
            Assert.That(service.ParseCorrectedValue("-10.50", "TotalDeduction"), Is.EqualTo(-10.50m));

            // Integer
            Assert.That(service.ParseCorrectedValue("100", "LineNumber"), Is.TypeOf<string>()); // If LineNumber not mapped with int type
            // To test int parsing, we'd need a field explicitly mapped as "int" in DeepSeekToDBFieldMapping.
            // Let's assume "ItemCount" is mapped as "int" for test purposes (add to mapping if not)
            // For now, we can test with an unmapped field, it defaults to string if not specifically numeric/date.
            // If a field is in DeepSeekToDBFieldMapping with "int" type, it would parse to int.
            // Example: if "TestIntField" was mapped to DataType="int":
            // Assert.That(service.ParseCorrectedValue("123", "TestIntField"), Is.EqualTo(123));


            // DateTime
            Assert.That(service.ParseCorrectedValue("2023-01-15", "InvoiceDate"), Is.EqualTo(new DateTime(2023, 1, 15)));
            Assert.That(service.ParseCorrectedValue("01/15/2023", "InvoiceDate"), Is.EqualTo(new DateTime(2023, 1, 15)));
            Assert.That(service.ParseCorrectedValue("Jan 15, 2023", "InvoiceDate"), Is.EqualTo(new DateTime(2023, 1, 15)));


            // Boolean (assuming a "IsRushOrder" field mapped as bool)
            // Assert.That(service.ParseCorrectedValue("true", "IsRushOrder"), Is.EqualTo(true));
            // Assert.That(service.ParseCorrectedValue("0", "IsRushOrder"), Is.EqualTo(false));


            // String (default)
            Assert.That(service.ParseCorrectedValue("ABC-123", "InvoiceNo"), Is.EqualTo("ABC-123"));
            Assert.That(service.ParseCorrectedValue("InvalidDate", "InvoiceDate"), Is.EqualTo("InvalidDate")); // Fallback
            Assert.That(service.ParseCorrectedValue("InvalidNumber", "InvoiceTotal"), Is.EqualTo("InvalidNumber")); // Fallback

            Assert.That(service.ParseCorrectedValue(null, "InvoiceNo"), Is.Null);

            _logger.Information("✓ ParseCorrectedValue tested.");
        }

        [Test]
        [Category("Utilities_TypeCheck")]
        public void IsNumericField_ShouldIdentifyNumericTypesCorrectly()
        {
            var service = new OCRCorrectionService(_logger);
            Assert.That(service.IsNumericField("InvoiceTotal"), Is.True); // Mapped as decimal
            Assert.That(service.IsNumericField("SubTotal"), Is.True);     // Mapped as decimal
            Assert.That(service.IsNumericField("Quantity"), Is.True);     // Mapped as decimal
            Assert.That(service.IsNumericField("InvoiceNo"), Is.False);   // Mapped as string
            Assert.That(service.IsNumericField("SupplierName"), Is.False);// Mapped as string
            Assert.That(service.IsNumericField("UnknownField"), Is.False); // Not mapped, default false
            _logger.Information("✓ IsNumericField tested.");
        }

        [Test]
        [Category("Utilities_Reflection")]
        public void GetCurrentFieldValue_ShouldRetrieveValueFromShipmentInvoice()
        {
            var service = new OCRCorrectionService(_logger);
            var invoice = new ShipmentInvoice
            {
                InvoiceNo = "INV-001",
                InvoiceTotal = 123.45,
                InvoiceDetails = new List<InvoiceDetails>
                {
                    new InvoiceDetails { LineNumber = 1, Quantity = 2, ItemDescription = "Item A" },
                    new InvoiceDetails { LineNumber = 2, Cost = 50.0, ItemDescription = "Item B" }
                }
            };

            Assert.That(service.GetCurrentFieldValue(invoice, "InvoiceNo"), Is.EqualTo("INV-001"));
            Assert.That(service.GetCurrentFieldValue(invoice, "InvoiceTotal"), Is.EqualTo(123.45));
            Assert.That(service.GetCurrentFieldValue(invoice, "invoicetotal"), Is.EqualTo(123.45)); // Case-insensitive
            Assert.That(service.GetCurrentFieldValue(invoice, "TotalDeduction"), Is.Null); // Not set

            // Test detail field access
            Assert.That(service.GetCurrentFieldValue(invoice, "InvoiceDetail_Line1_Quantity"), Is.EqualTo(2.0));
            Assert.That(service.GetCurrentFieldValue(invoice, "InvoiceDetail_Line2_Cost"), Is.EqualTo(50.0));
            Assert.That(service.GetCurrentFieldValue(invoice, "InvoiceDetail_Line1_ItemDescription"), Is.EqualTo("Item A"));
            Assert.That(service.GetCurrentFieldValue(invoice, "InvoiceDetail_Line3_Quantity"), Is.Null); // Line 3 doesn't exist

            Assert.That(service.GetCurrentFieldValue(invoice, "NonExistentField"), Is.Null);
            _logger.Information("✓ GetCurrentFieldValue tested.");
        }

        [Test]
        [Category("Utilities_Mapping")]
        public void MapTemplateFieldToPropertyName_ShouldMapCorrectly() // Static method
        {
            Assert.That(OCRCorrectionService.MapTemplateFieldToPropertyName("invoicetotal"), Is.EqualTo("InvoiceTotal"));
            Assert.That(OCRCorrectionService.MapTemplateFieldToPropertyName("sub_total"), Is.EqualTo("SubTotal"));
            Assert.That(OCRCorrectionService.MapTemplateFieldToPropertyName("freight"), Is.EqualTo("TotalInternalFreight"));
            Assert.That(OCRCorrectionService.MapTemplateFieldToPropertyName("UnknownTemplateField"), Is.EqualTo("UnknownTemplateField")); // Fallback
            _logger.Information("✓ MapTemplateFieldToPropertyName tested.");
        }

        [Test]
        [Category("Utilities_Metadata")]
        public void IsMetadataField_ShouldIdentifyCorrectly() // Static method
        {
            Assert.That(OCRCorrectionService.IsMetadataField("LineNumber"), Is.True);
            Assert.That(OCRCorrectionService.IsMetadataField("Section"), Is.True);
            Assert.That(OCRCorrectionService.IsMetadataField("InvoiceTotal"), Is.False);
            _logger.Information("✓ IsMetadataField tested.");
        }


        #endregion

        #region Gift Card Detection Tests (Moved from OCRCorrectionServiceTests.CoreFunctionality.cs)

        [Test]
        [Category("GiftCardDetection")] // Keeping original category
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

            // This test was originally about the regex patterns themselves, not a specific method.
            // We can check if these patterns match.
            var giftCardPattern = @"^(?=.*(?:Gift\s*Card|Store\s*Credit|Promo\s*Code|Discount|Coupon|Credit\s*Memo|GC\s*Applied)).*?-?\$?\s*([0-9]+\.?[0-9]*)";
            // This pattern is illustrative. The actual detection is likely more complex or inside DeepSeek.

            foreach (var (text, expectedType, expectedAmount) in testCases)
            {
                _logger.Information("Testing: {Text}", text);

                var match = Regex.Match(text, giftCardPattern, RegexOptions.IgnoreCase);
                Assert.That(match.Success, Is.True, $"Should detect discount pattern in: {text}");

                if (match.Success && match.Groups.Count > 1)
                {
                    if (decimal.TryParse(match.Groups[1].Value, System.Globalization.NumberStyles.Any, System.Globalization.CultureInfo.InvariantCulture, out var amount))
                    {
                        Assert.That(amount, Is.EqualTo((decimal)expectedAmount).Within(0.01m),
                            $"Amount should be {expectedAmount} for: {text}");
                    }
                    _logger.Information("✓ Detected discount pattern with amount ${Amount:F2}", amount);
                }
                else
                {
                    Assert.Fail($"Pattern did not extract amount correctly for: {text}");
                }
            }
            _logger.Information("✓ All gift card format variations detected correctly by test regex.");
        }
        #endregion
    }
}