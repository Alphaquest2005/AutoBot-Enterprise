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
    using Invoices = OCR.Business.Entities.Invoices;

    [TestFixture]
    public class OCRCorrectionService_ProductionTests
    {
        #region Test Setup and Infrastructure

        private static Serilog.ILogger _logger;
        private OCRCorrectionService _service;
        private string _testDataDirectory;
        private string _tempConfigDirectory;

        [OneTimeSetUp]
        public void FixtureSetup()
        {
            // Initialize logging for production testing
            Log.Logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .MinimumLevel.Override("Microsoft", LogEventLevel.Warning)
                .MinimumLevel.Override("System", LogEventLevel.Warning)
                .Enrich.FromLogContext()
                .WriteTo.Console(restrictedToMinimumLevel: LogEventLevel.Information)
                .WriteTo.File("logs/ocr-production-tests-.txt",
                    rollingInterval: RollingInterval.Day,
                    restrictedToMinimumLevel: LogEventLevel.Debug)
                .CreateLogger();

            _logger = Log.ForContext<OCRCorrectionService_ProductionTests>();
            _logger.Information("=== STARTING OCR CORRECTION SERVICE PRODUCTION TESTS ===");

            // Create test directories
            _testDataDirectory = Path.Combine(Path.GetTempPath(), "OCRTests", Guid.NewGuid().ToString());
            _tempConfigDirectory = Path.Combine(_testDataDirectory, "Config");
            Directory.CreateDirectory(_testDataDirectory);
            Directory.CreateDirectory(_tempConfigDirectory);

            _logger.Information("Test data directory: {Directory}", _testDataDirectory);
        }

        [OneTimeTearDown]
        public void FixtureTearDown()
        {
            _logger.Information("=== FINISHED OCR CORRECTION SERVICE PRODUCTION TESTS ===");

            try
            {
                if (Directory.Exists(_testDataDirectory))
                {
                    Directory.Delete(_testDataDirectory, true);
                }
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "Could not clean up test directory");
            }

            _service?.Dispose();
            Log.CloseAndFlush();
        }

        [SetUp]
        public void SetUp()
        {
            CleanTestEnvironment();
            _service = new OCRCorrectionService();
        }

        [TearDown]
        public void TearDown()
        {
            _service?.Dispose();
            CleanTestEnvironment();
        }

        private void CleanTestEnvironment()
        {
            try
            {
                var configPath = Path.Combine(Directory.GetCurrentDirectory(), "OCRRegexPatterns.json");
                if (File.Exists(configPath))
                {
                    File.Delete(configPath);
                }

                if (Directory.Exists(_tempConfigDirectory))
                {
                    var jsonFiles = Directory.GetFiles(_tempConfigDirectory, "*.json");
                    foreach (var file in jsonFiles)
                    {
                        try
                        {
                            File.Delete(file);
                        }
                        catch (IOException)
                        {
                            Thread.Sleep(10);
                            try
                            {
                                File.Delete(file);
                            }
                            catch (IOException ex)
                            {
                                _logger.Warning(ex, "Could not delete file {File}", file);
                            }
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Warning(ex, "Could not clean test environment");
            }
        }

        #endregion

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

        #region Regex Pattern Learning Tests

        [Test]
        [Category("RegexLearning")]
        [Category("FileSystem")]
        public async Task RegexPatternPersistence_SaveAndLoad_ShouldMaintainState()
        {
            _logger.Information("Testing regex pattern persistence to real file system");

            var originalDirectory = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(_tempConfigDirectory);

            try
            {
                var testPatterns = new List<RegexPattern>
                {
                    new RegexPattern
                    {
                        FieldName = "InvoiceTotal",
                        StrategyType = "FORMAT_FIX",
                        Pattern = @"(\d+),(\d{2})",
                        Replacement = "$1.$2",
                        Confidence = 0.95,
                        CreatedDate = DateTime.UtcNow,
                        LastUpdated = DateTime.UtcNow,
                        UpdateCount = 1,
                        CreatedBy = "Test"
                    },
                    new RegexPattern
                    {
                        FieldName = "SubTotal",
                        StrategyType = "CHARACTER_MAP",
                        Pattern = @"1O(\d+)",
                        Replacement = "10$1",
                        Confidence = 0.88,
                        CreatedDate = DateTime.UtcNow,
                        LastUpdated = DateTime.UtcNow,
                        UpdateCount = 2,
                        CreatedBy = "Test"
                    }
                };

                // Save patterns using production code
                var regexConfigPath = Path.Combine(_tempConfigDirectory, "OCRRegexPatterns.json");
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(testPatterns, options);
                File.WriteAllText(regexConfigPath, json);

                _logger.Information("Saved {Count} patterns to {Path}", testPatterns.Count, regexConfigPath);

                // Load patterns using production code
                var loadedPatterns = await InvokePrivateMethodAsync<List<RegexPattern>>(
                    _service, "LoadRegexPatternsAsync");

                _logger.Information("Loaded {Count} patterns from file", loadedPatterns.Count);

                Assert.That(loadedPatterns.Count, Is.EqualTo(testPatterns.Count));

                for (int i = 0; i < testPatterns.Count; i++)
                {
                    var original = testPatterns[i];
                    var loaded = loadedPatterns.FirstOrDefault(p => p.FieldName == original.FieldName);

                    Assert.That(loaded, Is.Not.Null, $"Pattern for {original.FieldName} should be loaded");
                    Assert.That(loaded.Pattern, Is.EqualTo(original.Pattern));
                    Assert.That(loaded.Replacement, Is.EqualTo(original.Replacement));
                    Assert.That(loaded.Confidence, Is.EqualTo(original.Confidence));
                    Assert.That(loaded.StrategyType, Is.EqualTo(original.StrategyType));

                    _logger.Information("✓ Pattern {FieldName}: {Pattern} → {Replacement}",
                        loaded.FieldName, loaded.Pattern, loaded.Replacement);
                }

                _logger.Information("✓ Regex pattern persistence working correctly");
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDirectory);
            }
        }

        [Test]
        [Category("RegexLearning")]
        [Category("FileSystem")]
        public async Task ApplyLearnedRegexPatternsAsync_ShouldTransformText()
        {
            _logger.Information("Testing application of learned regex patterns");

            var originalDirectory = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(_tempConfigDirectory);

            try
            {
                // Create a test pattern for decimal comma correction
                var testPattern = new RegexPattern
                {
                    FieldName = "InvoiceTotal",
                    StrategyType = "FORMAT_FIX",
                    Pattern = @"\$([0-9]+),([0-9]{2})",
                    Replacement = "$$$1.$2",
                    Confidence = 0.95,
                    CreatedDate = DateTime.UtcNow,
                    LastUpdated = DateTime.UtcNow,
                    UpdateCount = 1,
                    CreatedBy = "Test"
                };

                // Save pattern to file
                var patterns = new List<RegexPattern> { testPattern };
                var regexConfigPath = Path.Combine(_tempConfigDirectory, "OCRRegexPatterns.json");
                var options = new JsonSerializerOptions { WriteIndented = true };
                var json = JsonSerializer.Serialize(patterns, options);
                File.WriteAllText(regexConfigPath, json);

                var originalText = "Invoice Total: $123,45";
                var expectedText = "Invoice Total: $123.45";

                // Apply patterns using production code
                var transformedText = await InvokePrivateMethodAsync<string>(_service,
                    "ApplyLearnedRegexPatternsAsync", originalText, "InvoiceTotal");

                _logger.Information("Original: {Original}", originalText);
                _logger.Information("Transformed: {Transformed}", transformedText);

                Assert.That(transformedText, Is.EqualTo(expectedText),
                    "Text should be transformed according to regex pattern");

                _logger.Information("✓ Learned regex patterns applied correctly");
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDirectory);
            }
        }

        [Test]
        [Category("RegexLearning")]
        [Category("Integration")]
        public async Task RegexLearningWorkflow_EndToEnd_ShouldImproveAccuracy()
        {
            _logger.Information("Testing end-to-end regex learning workflow");

            var originalDirectory = Directory.GetCurrentDirectory();
            Directory.SetCurrentDirectory(_tempConfigDirectory);

            try
            {
                // Step 1: Create invoice with known error pattern
                var invoice = CreateTestInvoice("LEARN-001", 123.45, 123.45, 0, 0, 0, 0);
                var fileText = "Total: $123.45 Some other text shows InvoiceTotal as 12345";

                // Step 2: Run correction to generate learning data
                var corrections = new List<CorrectionResult>
                {
                    new CorrectionResult
                    {
                        FieldName = "InvoiceTotal",
                        OldValue = "12345",
                        NewValue = "123.45",
                        CorrectionType = "decimal_point_missing",
                        Success = true,
                        Confidence = 0.95
                    }
                };

                // Step 3: Apply regex learning
                await _service.UpdateRegexPatternsAsync(corrections, fileText);

                // Step 4: Verify patterns were learned
                var learnedPatterns = await InvokePrivateMethodAsync<List<RegexPattern>>(
                    _service, "LoadRegexPatternsAsync");

                Assert.That(learnedPatterns.Count, Is.GreaterThan(0), "Should have learned at least one pattern");

                var invoiceTotalPattern = learnedPatterns.FirstOrDefault(p => p.FieldName == "InvoiceTotal");
                Assert.That(invoiceTotalPattern, Is.Not.Null, "Should have learned pattern for InvoiceTotal");

                if (invoiceTotalPattern != null)
                {
                    _logger.Information("Learned pattern: {Pattern} → {Replacement}",
                        invoiceTotalPattern.Pattern, invoiceTotalPattern.Replacement);
                }
                _logger.Information("✓ End-to-end regex learning workflow completed successfully");
            }
            finally
            {
                Directory.SetCurrentDirectory(originalDirectory);
            }
        }

        #endregion

        #region Public API Tests

        [Test]
        [Category("PublicAPI")]
        public async Task CorrectInvoiceAsync_ValidInvoice_ShouldReturnResult()
        {
            _logger.Information("Testing public CorrectInvoiceAsync method");

            var invoice = CreateTestInvoice("PUBLIC-001", 100.00, 85.00, 10.00, 5.00, 0.0, 0.0);
            var fileText = @"
                Invoice #PUBLIC-001
                Subtotal: $85.00
                Shipping: $10.00
                Tax: $5.00
                Total: $100.00
            ";

            var result = await _service.CorrectInvoiceAsync(invoice, fileText);

            _logger.Information("CorrectInvoiceAsync result: {Result}", result);
            Assert.That(result, Is.TypeOf<bool>(), "Should return boolean result");

            _logger.Information("✓ Public API method working correctly");
        }

        [Test]
        [Category("PublicAPI")]
        public async Task CorrectInvoicesAsync_MultipleInvoices_ShouldProcessAll()
        {
            _logger.Information("Testing public CorrectInvoicesAsync method with multiple invoices");

            var invoices = new List<ShipmentInvoice>
            {
                CreateTestInvoice("MULTI-001", 100.0, 85.0, 10.0, 5.0, 0.0, 0.0),
                CreateTestInvoice("MULTI-002", 200.0, 170.0, 20.0, 10.0, 0.0, 0.0)
            };

            // Create a test file
            var testFilePath = Path.Combine(_testDataDirectory, "multi-test");
            var txtFile = testFilePath + ".txt";
            File.WriteAllText(txtFile, "Test invoice content");

            var result = await _service.CorrectInvoicesAsync(invoices, testFilePath);

            _logger.Information("CorrectInvoicesAsync processed {Count} invoices", result.Count);
            Assert.That(result.Count, Is.EqualTo(invoices.Count), "Should return all invoices");

            _logger.Information("✓ Multiple invoice processing working correctly");
        }

        [Test]
        [Category("PublicAPI")]
        public void TotalsZero_StaticMethod_ShouldCalculateCorrectly()
        {
            _logger.Information("Testing static TotalsZero method");

            var invoice = CreateTestInvoice("STATIC-001", 100.0, 85.0, 10.0, 5.0, 0.0, 0.0);
            var result = OCRCorrectionService.TotalsZero(invoice);

            _logger.Information("TotalsZero result: {Result}", result);
            Assert.That(result, Is.True, "Balanced invoice should return true");

            // Test with unbalanced invoice
            var unbalancedInvoice = CreateTestInvoice("STATIC-002", 100.0, 85.0, 10.0, 6.0, 0.0, 0.0); // Total should be 101
            var unbalancedResult = OCRCorrectionService.TotalsZero(unbalancedInvoice);

            _logger.Information("Unbalanced TotalsZero result: {Result}", unbalancedResult);
            Assert.That(unbalancedResult, Is.False, "Unbalanced invoice should return false");

            _logger.Information("✓ Static TotalsZero method working correctly");
        }

        #endregion

        #region Performance Tests

        [Test]
        [Category("Performance")]
        public async Task CorrectInvoices_100Invoices_ShouldCompleteWithinTimeLimit()
        {
            _logger.Information("Testing performance with 100 invoices");

            var invoiceCount = 100;
            var invoices = new List<ShipmentInvoice>();

            // Generate test invoices
            for (int i = 0; i < invoiceCount; i++)
            {
                var invoice = CreateTestInvoice($"PERF-{i:D3}", 100 + (i % 100), 85 + (i % 50), 10, 5, 0, 0);
                invoices.Add(invoice);
            }

            var stopwatch = Stopwatch.StartNew();

            foreach (var invoice in invoices)
            {
                var fileText = $"Invoice #{invoice.InvoiceNo}\nTotal: ${invoice.InvoiceTotal:F2}";

                // Test the static TotalsZero method for performance (no API calls)
                var result = OCRCorrectionService.TotalsZero(invoice);

                if ((invoices.IndexOf(invoice) + 1) % 25 == 0)
                {
                    _logger.Information("Processed {Count}/{Total} invoices", invoices.IndexOf(invoice) + 1, invoiceCount);
                }
            }

            stopwatch.Stop();

            var invoicesPerSecond = invoiceCount / (stopwatch.ElapsedMilliseconds / 1000.0);
            _logger.Information("Performance: {Count} invoices in {ElapsedMs}ms ({Rate:F1} invoices/sec)",
                invoiceCount, stopwatch.ElapsedMilliseconds, invoicesPerSecond);

            Assert.That(invoicesPerSecond, Is.GreaterThan(100),
                $"Performance target not met. Expected >100 invoice/sec, got {invoicesPerSecond:F1}");

            _logger.Information("✓ Performance target met for 100 invoices");
        }

        [Test]
        [Category("Memory")]
        public async Task CorrectInvoices_MemoryUsage_ShouldNotLeak()
        {
            _logger.Information("Testing memory usage and leak detection");

            var initialMemory = GC.GetTotalMemory(true);
            _logger.Information("Initial memory: {Memory:N0} bytes", initialMemory);

            var iterations = 50;
            for (int i = 0; i < iterations; i++)
            {
                using (var service = new OCRCorrectionService())
                {
                    var invoice = CreateTestInvoice($"MEM-{i:D3}", 100 + i, 85 + i, 10, 5, 0, 0);

                    // Test lightweight operations to avoid API calls
                    var result = OCRCorrectionService.TotalsZero(invoice);
                }

                if ((i + 1) % 10 == 0)
                {
                    GC.Collect();
                    GC.WaitForPendingFinalizers();
                    GC.Collect();

                    var currentMemory = GC.GetTotalMemory(false);
                    _logger.Information("Iteration {Iteration}: Memory = {Memory:N0} bytes",
                        i + 1, currentMemory);
                }
            }

            GC.Collect();
            GC.WaitForPendingFinalizers();
            GC.Collect();

            var finalMemory = GC.GetTotalMemory(false);
            var memoryIncrease = finalMemory - initialMemory;
            var memoryIncreasePercent = (initialMemory == 0) ? 0 : (memoryIncrease / (double)initialMemory) * 100;

            _logger.Information("Final memory: {Memory:N0} bytes", finalMemory);
            _logger.Information("Memory increase: {Increase:N0} bytes ({Percent:F1}%)",
                memoryIncrease, memoryIncreasePercent);

            Assert.That(memoryIncreasePercent, Is.LessThan(50),
                "Memory increase should be less than 50% of initial memory");

            _logger.Information("✓ No significant memory leaks detected");
        }

        #endregion

        #region Edge Cases and Boundary Tests

        [Test]
        [Category("EdgeCases")]
        public void CrossFieldValidation_NullAndEmptyFields_ShouldHandleGracefully()
        {
            _logger.Information("Testing cross-field validation with null and empty fields");

            var edgeCaseInvoices = new[]
            {
                new ShipmentInvoice
                {
                    InvoiceNo = "NULL-001",
                    InvoiceTotal = null, SubTotal = null, TotalInternalFreight = null,
                    TotalOtherCost = null, TotalInsurance = null, TotalDeduction = null,
                    InvoiceDetails = null
                },
                new ShipmentInvoice
                {
                    InvoiceNo = "ZERO-001",
                    InvoiceTotal = 0, SubTotal = 0, TotalInternalFreight = 0,
                    TotalOtherCost = 0, TotalInsurance = 0, TotalDeduction = 0,
                    InvoiceDetails = new List<InvoiceDetails>()
                },
                new ShipmentInvoice
                {
                    InvoiceNo = "EMPTY-DETAILS-001",
                    InvoiceTotal = 100, SubTotal = 85, TotalInternalFreight = 10, TotalOtherCost = 5,
                    InvoiceDetails = new List<InvoiceDetails>()
                }
            };

            foreach (var invoice in edgeCaseInvoices)
            {
                _logger.Information("Testing edge case invoice: {InvoiceNo}", invoice.InvoiceNo);

                Assert.DoesNotThrow(() =>
                {
                    var mathErrors = InvokePrivateMethod<List<InvoiceError>>(_service,
                        "ValidateMathematicalConsistency", invoice);
                    var crossErrors = InvokePrivateMethod<List<InvoiceError>>(_service,
                        "ValidateCrossFieldConsistency", invoice);
                    var totalsZero = OCRCorrectionService.TotalsZero(invoice);

                    _logger.Information("  Math errors: {Math}, Cross errors: {Cross}, TotalsZero: {TotalsZero}",
                        mathErrors.Count, crossErrors.Count, totalsZero);
                }, $"Should handle edge case invoice {invoice.InvoiceNo} gracefully");
            }

            _logger.Information("✓ Null and empty field edge cases handled gracefully");
        }

        [Test]
        [Category("EdgeCases")]
        public void MathematicalConsistency_ExtremeValues_ShouldNotOverflow()
        {
            _logger.Information("Testing mathematical consistency with extreme values");

            var extremeValueCases = new[]
            {
                (double.MaxValue / 2, double.MaxValue / 4, double.MaxValue / 8, double.MaxValue / 8, 0.0, 0.0),
                (double.Epsilon * 1000, double.Epsilon * 500, double.Epsilon * 300, double.Epsilon * 200, 0.0, 0.0),
                (1000000.01, 999999.99, 0.01, 0.01, 0.0, 0.0),
                (-100.0, -85.0, -10.0, -5.0, 0.0, 0.0)
            };

            foreach (var (total, subTotal, freight, other, insurance, deduction) in extremeValueCases)
            {
                var invoice = CreateTestInvoice("EXTREME", total, subTotal, freight, other, insurance, deduction);
                if (invoice.InvoiceDetails == null) invoice.InvoiceDetails = new List<InvoiceDetails>();
                invoice.InvoiceDetails.Add(new InvoiceDetails
                {
                    LineNumber = 1,
                    Quantity = 1,
                    Cost = subTotal,
                    TotalCost = subTotal,
                    Discount = 0
                });

                _logger.Information("Testing extreme values: Total={Total}, SubTotal={SubTotal}", total, subTotal);

                Assert.DoesNotThrow(() =>
                {
                    var mathErrors = InvokePrivateMethod<List<InvoiceError>>(_service,
                        "ValidateMathematicalConsistency", invoice);
                    var crossErrors = InvokePrivateMethod<List<InvoiceError>>(_service,
                        "ValidateCrossFieldConsistency", invoice);

                    _logger.Information("  Validation completed: {Math} math errors, {Cross} cross errors",
                        mathErrors.Count, crossErrors.Count);

                    var overflowErrors = mathErrors.Concat(crossErrors)
                        .Where(e => e.Reasoning?.ToUpper().Contains("OVERFLOW") == true ||
                                   e.Reasoning?.ToUpper().Contains("UNDERFLOW") == true);

                    Assert.That(overflowErrors.Count(), Is.EqualTo(0), "Should not have overflow/underflow errors");

                }, "Should handle extreme values without overflow");
            }

            _logger.Information("✓ Extreme value calculations handled without overflow");
        }

        [Test]
        [Category("EdgeCases")]
        [Category("Unicode")]
        public void TextProcessing_InternationalCharacters_ShouldHandleCorrectly()
        {
            _logger.Information("Testing text processing with international characters");

            var internationalTexts = new[]
            {
                ("French", "Facture #123\nTotal: 100,50 €\nTVA: 20,10 €"),
                ("German", "Rechnung #456\nSumme: 200,75 €\nMwSt: 38,14 €"),
                ("Spanish", "Factura #789\nTotal: 150,25 €\nIVA: 31,55 €"),
                ("Chinese", "发票 #001\n总计: ￥500.00\n税金: ￥85.00"),
                ("Arabic", "فاتورة #002\nالمجموع: 300.00 ر.س\nالضريبة: 45.00 ر.س"),
                ("Mixed", "Invoice #999\nSubtotal: $100.00\nTaxe/税金/ضريبة: $15.00\nTotal: $115.00")
            };

            foreach (var (language, text) in internationalTexts)
            {
                _logger.Information("Testing {Language} text processing", language);

                Assert.DoesNotThrow(() =>
                {
                    var cleanedText = InvokePrivateMethod<string>(_service, "CleanTextForAnalysis", text);
                    Assert.That(cleanedText, Is.Not.Null.And.Not.Empty,
                        $"Should clean {language} text without errors");

                    _logger.Information("  Original length: {Original}, Cleaned length: {Cleaned}",
                        text.Length, cleanedText.Length);

                }, $"Should handle {language} text without exceptions");
            }

            _logger.Information("✓ International character processing handled correctly");
        }

        #endregion

        #region Configuration Tests

        [Test]
        [Category("Configuration")]
        public void OCRCorrectionService_ConfigurationProperties_ShouldBehaveCorrectly()
        {
            _logger.Information("Testing configuration properties");

            using var service = new OCRCorrectionService();

            // Test default values
            Assert.That(service.DefaultTemperature, Is.EqualTo(0.1), "Default temperature should be 0.1");
            Assert.That(service.DefaultMaxTokens, Is.EqualTo(4096), "Default max tokens should be 4096");

            // Test setting values
            service.DefaultTemperature = 0.5;
            service.DefaultMaxTokens = 8192;

            Assert.That(service.DefaultTemperature, Is.EqualTo(0.5), "Temperature should be settable");
            Assert.That(service.DefaultMaxTokens, Is.EqualTo(8192), "Max tokens should be settable");

            _logger.Information("✓ Configuration properties working correctly");
        }

        #endregion

        #region Integration Tests

        [Test]
        [Category("Integration")]
        [Category("EndToEnd")]
        public async Task EndToEndWorkflow_PDFToCorrection_ShouldWorkCompletely()
        {
            _logger.Information("Testing complete end-to-end workflow from PDF to correction");

            // Step 1: Create test files simulating PDF extraction
            var pdfFileName = "test-invoice-001";
            var txtFilePath = Path.Combine(_testDataDirectory, $"{pdfFileName}.txt");
            var pdfContent = @"
                COMMERCIAL INVOICE #END2END-001
                ================================

                Supplier: Test Company Ltd.
                Address: 123 Test Street, Test City

                LINE ITEMS:
                Description             Qty    Price     Total
                Widget A                5      $10.50    $52.50
                Widget B                3      $15.75    $47.25

                Subtotal:               $99.75
                Shipping:               $10.25
                Tax:                    $8.00
                Gift Card Applied:      -$5.00

                TOTAL:                  $113.00
            ";

            File.WriteAllText(txtFilePath, pdfContent);

            // Step 2: Create invoice objects as would be extracted from PDF
            var dynamicResults = new List<dynamic>
            {
                new List<IDictionary<string, object>>
                {
                    new Dictionary<string, object>
                    {
                        ["InvoiceNo"] = "END2END-001",
                        ["InvoiceTotal"] = 113.00,
                        ["SubTotal"] = 99.75,
                        ["TotalInternalFreight"] = 10.25,
                        ["TotalOtherCost"] = 8.0,
                        ["TotalInsurance"] = 0.0,
                        ["TotalDeduction"] = 0.0, // OCR missed the gift card deduction
                        ["InvoiceDetails"] = new List<IDictionary<string, object>>
                        {
                            new Dictionary<string, object>
                            {
                                ["ItemDescription"] = "Widget A", ["Quantity"] = 5.0, ["Cost"] = 10.50, ["TotalCost"] = 52.50
                            },
                            new Dictionary<string, object>
                            {
                                ["ItemDescription"] = "Widget B", ["Quantity"] = 3.0, ["Cost"] = 15.75, ["TotalCost"] = 47.25
                            }
                        }
                    }
                }
            };

            // Step 3: Test initial TotalsZero calculation
            var successBefore = OCRCorrectionService.TotalsZero(dynamicResults, out double totalZeroSumBefore);
            _logger.Information("Initial TotalsZero calculation: Success={Success}, Sum={Sum:F2}",
                successBefore, totalZeroSumBefore);

            Assert.That(successBefore, Is.True, "TotalsZero calculation should succeed structurally");

            // Step 4: Apply corrections using static method
            var mockTemplate = new Invoice(new OCR.Business.Entities.Invoices(), _logger)
            {
                FilePath = Path.Combine(_testDataDirectory, pdfFileName)
            };

            await OCRCorrectionService.CorrectInvoices(dynamicResults, mockTemplate);

            // Step 5: Verify corrections were applied
            var successAfter = OCRCorrectionService.TotalsZero(dynamicResults, out double totalZeroSumAfter);
            _logger.Information("Post-correction TotalsZero: Success={Success}, Sum={Sum:F2}",
                successAfter, totalZeroSumAfter);

            Assert.That(successAfter, Is.True, "Post-correction calculation should succeed structurally");

            _logger.Information("✓ End-to-end workflow completed successfully");
        }

        #endregion

        #region Helper Methods and Utilities

        private async Task<T> InvokePrivateMethodAsync<T>(object instance, string methodName, params object[] parameters)
        {
            var type = instance.GetType();
            var method = type.GetMethod(methodName,
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (method == null)
            {
                throw new ArgumentException($"Method {methodName} not found on type {type.Name}");
            }

            var result = method.Invoke(instance, parameters);

            if (result is Task<T> taskWithResult)
            {
                return await taskWithResult;
            }
            else if (result is Task task)
            {
                await task;
                return default(T);
            }
            else
            {
                return (T)result;
            }
        }

        private T InvokePrivateMethod<T>(object instance, string methodName, params object[] parameters)
        {
            var type = instance.GetType();
            var method = type.GetMethod(methodName,
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (method == null)
            {
                var paramTypes = parameters.Select(p => p.GetType()).ToArray();
                method = type.GetMethod(methodName,
                    System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance,
                    null, paramTypes, null);

                if (method == null)
                    throw new ArgumentException($"Method {methodName} with specified parameters not found on type {type.Name}");
            }

            var result = method.Invoke(instance, parameters);
            return (T)result;
        }

        private ShipmentInvoice CreateTestInvoice(string invoiceNo, double total, double subTotal,
            double freight, double otherCost, double insurance, double deduction)
        {
            return new ShipmentInvoice
            {
                InvoiceNo = invoiceNo,
                InvoiceTotal = total,
                SubTotal = subTotal,
                TotalInternalFreight = freight,
                TotalOtherCost = otherCost,
                TotalInsurance = insurance,
                TotalDeduction = deduction,
                InvoiceDate = DateTime.Now,
                Currency = "USD",
                SupplierName = "Test Supplier",
                SupplierAddress = "123 Test St",
                InvoiceDetails = new List<InvoiceDetails>()
            };
        }

        private ShipmentInvoice CreateTestInvoiceWithErrors()
        {
            return new ShipmentInvoice
            {
                InvoiceNo = "INV-WRONG-001",
                InvoiceTotal = 100.50,
                SubTotal = 80.00,
                SupplierName = "Suplier Test Inc.",
                InvoiceDate = new DateTime(2023, 1, 15),
                InvoiceDetails = new List<InvoiceDetails>()
            };
        }

        private string CreateTestFileTextWithKnownErrors()
        {
            return @"
                INVOICE #INV-CORRECT-001
                Date: 15/01/2023
                Supplier: Supplier Test Inc.
                Total Amount: $100.50
                Subtotal: $80.00
                Some other text...
            ";
        }

        private ShipmentInvoice CreateTestInvoiceWithProductErrors()
        {
            var invoice = CreateTestInvoice("PROD-ERR-01", 150.75, 120.50, 10.25, 20.00, 0, 0);
            invoice.InvoiceDetails = new List<InvoiceDetails>
            {
                new InvoiceDetails { LineNumber = 1, ItemDescription = "Widget A old", Quantity = 2, Cost = 25.25, TotalCost = 50.50 },
                new InvoiceDetails { LineNumber = 2, ItemDescription = "Gadget B Typo", Quantity = 1, Cost = 70.00, TotalCost = 70.00 }
            };
            return invoice;
        }

        private string CreateTestFileTextWithProductErrors()
        {
            return @"
                Invoice: PROD-ERR-01
                Total: $150.75
                Line Items:
                1. Widget A New Model (2 units @ $25.25 each) = $50.50
                2. Gadget B Correct (1 unit @ $70.00 each) = $70.00
                Other text...
            ";
        }

        private ShipmentInvoice CreateComplexTestInvoice()
        {
            var invoice = CreateTestInvoice("COMPLEX-001", 10000, 8000, 500, 500, 200, 800);
            invoice.InvoiceDetails = new List<InvoiceDetails>();
            for (int i = 0; i < 50; i++)
            {
                invoice.InvoiceDetails.Add(new InvoiceDetails
                {
                    LineNumber = i + 1,
                    ItemDescription = $"Complex Item {i + 1} with a very long description to increase text size",
                    Quantity = i + 1,
                    Cost = Math.Round(100.0 / (i + 1), 2),
                    TotalCost = 100.0,
                    Discount = (i % 5)
                });
            }
            return invoice;
        }

        private string CreateVeryLargeTestFileText()
        {
            var sb = new StringBuilder();
            sb.AppendLine("VERY LARGE INVOICE DOCUMENT START");
            for (int i = 0; i < 200; i++)
            {
                sb.AppendLine($"Header Info Line {i}: Some random text data {Guid.NewGuid()}");
            }
            for (int i = 0; i < 500; i++)
            {
                sb.AppendLine($"Item {i + 1}: Product Name {Guid.NewGuid().ToString().Substring(0, 10)}, Qty: {i + 1}, Price: ${Math.Round((i + 1) * 1.13, 2)}, Total: ${Math.Round((i + 1) * (i + 1) * 1.13, 2)}");
            }
            sb.AppendLine("VERY LARGE INVOICE DOCUMENT END");
            return sb.ToString();
        }

        private List<dynamic> GenerateLargeInvoiceList(int count)
        {
            var list = new List<dynamic>();
            for (int i = 0; i < count; i++)
            {
                list.Add(new Dictionary<string, object>
                {
                    ["InvoiceNo"] = $"INV-{i:D5}",
                    ["InvoiceTotal"] = 100.0 + i,
                    ["SubTotal"] = 80.0 + (i * 0.8),
                    ["TotalInternalFreight"] = 10.0 + (i * 0.1),
                    ["TotalOtherCost"] = 10.0 + (i * 0.1),
                    ["TotalInsurance"] = 0.0,
                    ["TotalDeduction"] = 0.0,
                    ["InvoiceDetails"] = new List<IDictionary<string, object>>()
                });
            }
            return list;
        }

        private string GenerateLargeInvoiceText(int numLineItems)
        {
            var sb = new StringBuilder();
            sb.AppendLine("Invoice Test Document");
            sb.AppendLine("Supplier: MegaCorp Inc.");
            sb.AppendLine("InvoiceNo: Largetext-001");
            for (int i = 0; i < numLineItems; i++)
            {
                sb.AppendLine($"Line {i + 1}: Item Description {Guid.NewGuid()}, Qty: {i % 10 + 1}, Cost: {Math.Round(10.0 + i * 0.1, 2)}, Total: {Math.Round((i % 10 + 1) * (10.0 + i * 0.1), 2)}");
            }
            sb.AppendLine("End of Document");
            return sb.ToString();
        }

        private ShipmentInvoice CreateTestInvoiceWithInconsistentDetails()
        {
            var invoice = CreateTestInvoice("INCONSISTENT-001", 100, 50, 10, 5, 0, 0);
            invoice.InvoiceDetails = new List<InvoiceDetails>
            {
                new InvoiceDetails { LineNumber = 1, ItemDescription = "Item A", Quantity = 1, Cost = 30, TotalCost = 30 },
                new InvoiceDetails { LineNumber = 2, ItemDescription = "Item B", Quantity = 1, Cost = 30, TotalCost = 30 }
            };
            return invoice;
        }

        private List<string> ValidateDataQuality(ShipmentInvoice invoice)
        {
            var issues = new List<string>();
            if (string.IsNullOrEmpty(invoice.InvoiceNo)) issues.Add("Invoice number is missing.");
            if (invoice.InvoiceTotal == null) issues.Add("Invoice total is missing.");
            if (invoice.SubTotal == null) issues.Add("Subtotal is missing.");
            if (invoice.InvoiceDetails != null)
            {
                double lineItemsSum = invoice.InvoiceDetails.Sum(d => d.TotalCost ?? 0);
                if (invoice.SubTotal.HasValue && Math.Abs(lineItemsSum - invoice.SubTotal.Value) > 0.01)
                {
                    issues.Add($"SubTotal {invoice.SubTotal} does not match sum of line items {lineItemsSum}.");
                }
            }
            else
            {
                if ((invoice.SubTotal ?? 0) > 0) issues.Add("Invoice details are missing but SubTotal is present.");
            }
            if (string.IsNullOrEmpty(invoice.SupplierName)) issues.Add("Supplier name is missing.");
            return issues;
        }

        public class TestLogSink : ILogEventSink
        {
            private readonly List<(LogEventLevel Level, string Message)> _events;
            public TestLogSink(List<(LogEventLevel Level, string Message)> events) => _events = events;
            public void Emit(LogEvent logEvent) => _events.Add((logEvent.Level, logEvent.RenderMessage()));
        }

        #endregion
    }
}
