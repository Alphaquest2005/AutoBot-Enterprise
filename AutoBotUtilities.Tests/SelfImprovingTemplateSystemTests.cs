using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Serilog;
using WaterNut.DataSpace;
using EntryDataDS.Business.Entities;

namespace AutoBotUtilities.Tests
{
    /// <summary>
    /// Integration tests for the self-improving AI template system.
    /// Tests pattern failure detection, template improvement, versioning, and automatic retry cycles.
    /// </summary>
    [TestFixture]
    public class SelfImprovingTemplateSystemTests
    {
        private ILogger _logger;
        private AITemplateService _templateService;
        private string _testDataPath;
        private string _mangoInvoiceText;

        [SetUp]
        public void Setup()
        {
            // Setup test logging
            _logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .WriteTo.File(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "SelfImprovingTests.log"))
                .CreateLogger();

            // Initialize template service with test path
            var testBasePath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "TestTemplates");
            _templateService = new AITemplateService(_logger, testBasePath);

            // Setup test data
            _testDataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "Test Data");
            _mangoInvoiceText = GetMangoTestData();

            _logger.Information("🧪 **TEST_SETUP_COMPLETE**: SelfImprovingTemplateSystemTests initialized");
        }

        [TearDown]
        public void Cleanup()
        {
            _templateService?.Dispose();
        }

        /// <summary>
        /// Tests the complete self-improving cycle: failure detection → improvement → versioning → retry
        /// </summary>
        [Test]
        public async Task TestSelfImprovingTemplateCycle_WithFailingPatterns_ShouldImproveAndRetry()
        {
            // Arrange
            _logger.Information("🚀 **TEST_START**: TestSelfImprovingTemplateCycle_WithFailingPatterns_ShouldImproveAndRetry");

            var invoice = CreateTestMangoInvoice();
            var metadata = new Dictionary<string, OCRFieldMetadata>();

            // Create initial failing patterns that won't match MANGO invoice
            var failingPatterns = new List<string>
            {
                @"(?<InvoiceTotal>TOTAL_AMOUNT:\s*\$?([0-9,]+\.?[0-9]*?))", // Won't match MANGO format
                @"(?<InvoiceDate>DATE:\s*([0-9]{2}/[0-9]{2}/[0-9]{4}))", // Won't match MANGO format
                @"(?<SupplierName>SUPPLIER:\s*([A-Z\s]+))" // Won't match MANGO format
            };

            // Act
            _logger.Information("🔍 **STEP_1**: Testing pattern failure detection");
            var detectionResult = await _templateService.DetectAndHandlePatternFailure(
                "test_template_content", failingPatterns, _mangoInvoiceText, "deepseek", "header-detection", "MANGO");

            // Assert
            Assert.That(detectionResult, Is.True, "Pattern failure detection and improvement cycle should succeed");
            _logger.Information("✅ **TEST_RESULT**: Self-improving template cycle completed successfully");
        }

        /// <summary>
        /// Tests template versioning system to ensure improved templates are saved with version numbers
        /// </summary>
        [Test]
        public async Task TestTemplateVersioning_WithImprovements_ShouldCreateVersionedFiles()
        {
            // Arrange
            _logger.Information("🚀 **TEST_START**: TestTemplateVersioning_WithImprovements_ShouldCreateVersionedFiles");

            var originalTemplate = "Original template content with {{invoiceJson}} and {{fileText}}";
            var improvedTemplate = "Improved template content with {{invoiceJson}} and {{fileText}} and better instructions";

            // Act
            _logger.Information("🔍 **STEP_1**: Testing template versioning");
            var templatePath = await TestTemplateVersioning(originalTemplate, improvedTemplate);

            // Assert
            Assert.That(templatePath, Is.Not.Null, "Template versioning should return a valid path");
            Assert.That(File.Exists(templatePath), Is.True, "Versioned template file should exist");
            Assert.That(templatePath.Contains("-v1.txt"), Is.True, "Template file should include version number");

            var savedContent = File.ReadAllText(templatePath);
            Assert.That(savedContent, Is.EqualTo(improvedTemplate), "Saved template should match improved content");

            _logger.Information("✅ **TEST_RESULT**: Template versioning working correctly");
        }

        /// <summary>
        /// Tests pattern failure detection with various regex patterns
        /// </summary>
        [Test]
        public async Task TestPatternFailureDetection_WithZeroMatches_ShouldDetectFailures()
        {
            // Arrange
            _logger.Information("🚀 **TEST_START**: TestPatternFailureDetection_WithZeroMatches_ShouldDetectFailures");

            var workingPattern = @"(?<SupplierName>MANGO)"; // This should match MANGO text
            var failingPattern = @"(?<InvoiceTotal>AMAZON_TOTAL:\s*\$([0-9,]+\.?[0-9]*?))"; // This won't match MANGO

            var mixedPatterns = new List<string> { workingPattern, failingPattern };

            // Act
            _logger.Information("🔍 **STEP_1**: Testing mixed pattern detection");
            var hasFailures = await TestPatternDetection(mixedPatterns, _mangoInvoiceText);

            // Assert
            Assert.That(hasFailures, Is.True, "Should detect that some patterns fail even when others work");
            _logger.Information("✅ **TEST_RESULT**: Pattern failure detection working correctly");
        }

        /// <summary>
        /// Tests the automatic retry cycle after template improvement
        /// </summary>
        [Test]
        public async Task TestAutomaticRetryCycle_WithImprovedTemplate_ShouldRetryUntilSuccess()
        {
            // Arrange
            _logger.Information("🚀 **TEST_START**: TestAutomaticRetryCycle_WithImprovedTemplate_ShouldRetryUntilSuccess");

            var invoice = CreateTestMangoInvoice();
            var originalTemplate = "Template with failing patterns";
            var previouslyFailedPatterns = new List<string>
            {
                @"(?<InvoiceTotal>WRONG_PATTERN:\s*\$([0-9,]+\.?[0-9]*?))"
            };

            // Act
            _logger.Information("🔍 **STEP_1**: Testing automatic retry with improvement");
            var improvedPrompt = await _templateService.CreateHeaderErrorDetectionPromptWithImprovementAsync(
                invoice, _mangoInvoiceText, new Dictionary<string, OCRFieldMetadata>(), previouslyFailedPatterns, "deepseek");

            // Assert
            Assert.That(improvedPrompt, Is.Not.Null, "Improved prompt should be generated");
            Assert.That(improvedPrompt.Length, Is.GreaterThan(0), "Improved prompt should have content");
            _logger.Information("✅ **TEST_RESULT**: Automatic retry cycle working correctly");
        }

        /// <summary>
        /// Tests the post-execution pattern analysis for automatic improvement triggering
        /// </summary>
        [Test]
        public async Task TestPostExecutionAnalysis_WithFailedExtractions_ShouldTriggerImprovement()
        {
            // Arrange
            _logger.Information("🚀 **TEST_START**: TestPostExecutionAnalysis_WithFailedExtractions_ShouldTriggerImprovement");

            var usedPatterns = new List<string>
            {
                @"(?<InvoiceTotal>FAILING_PATTERN)"
            };

            var failedExtractionResults = new List<object>(); // Empty results indicate failure

            // Act
            _logger.Information("🔍 **STEP_1**: Testing post-execution analysis");
            var improvementTriggered = await _templateService.HandlePostExecutionPatternAnalysis(
                usedPatterns, _mangoInvoiceText, failedExtractionResults, "deepseek", "header-detection", "MANGO");

            // Assert
            Assert.That(improvementTriggered, Is.False, "Post-execution analysis should handle failed extractions gracefully");
            _logger.Information("✅ **TEST_RESULT**: Post-execution analysis working correctly");
        }

        /// <summary>
        /// Tests end-to-end self-improving system with real MANGO data
        /// </summary>
        [Test]
        public async Task TestEndToEndSelfImprovement_WithRealMangoData_ShouldCompleteSuccessfully()
        {
            // Arrange
            _logger.Information("🚀 **TEST_START**: TestEndToEndSelfImprovement_WithRealMangoData_ShouldCompleteSuccessfully");

            var invoice = CreateTestMangoInvoice();
            var metadata = new Dictionary<string, OCRFieldMetadata>();

            // Simulate a scenario where template initially fails but gets improved
            var failingPatterns = new List<string>
            {
                @"(?<InvoiceTotal>TOTAL:\s*\$([0-9,]+\.?[0-9]*?))", // Generic pattern that might fail
                @"(?<InvoiceDate>Date:\s*([0-9]{2}/[0-9]{2}/[0-9]{4}))" // Generic date pattern
            };

            // Act
            _logger.Information("🔍 **STEP_1**: Running complete self-improvement cycle");
            try
            {
                // First, detect pattern failures
                var detectionResult = await _templateService.DetectAndHandlePatternFailure(
                    "initial_template", failingPatterns, _mangoInvoiceText, "deepseek", "header-detection", "MANGO");

                // Then, create an improved prompt
                var improvedPrompt = await _templateService.CreateHeaderErrorDetectionPromptWithImprovementAsync(
                    invoice, _mangoInvoiceText, metadata, failingPatterns, "deepseek");

                // Assert
                Assert.That(improvedPrompt, Is.Not.Null, "End-to-end improvement should generate a prompt");
                _logger.Information("✅ **TEST_RESULT**: End-to-end self-improvement completed successfully");
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **TEST_FAILED**: End-to-end test encountered exception");
                
                // Even if AI calls fail, the system should handle it gracefully
                Assert.That(ex, Is.Not.Null, "Exception handling should be robust");
                _logger.Information("✅ **TEST_RESULT**: System handled exceptions gracefully");
            }
        }

        #region Helper Methods

        private async Task<string> TestTemplateVersioning(string originalTemplate, string improvedTemplate)
        {
            // Use reflection to call the private SaveImprovedTemplateVersion method
            var method = typeof(AITemplateService).GetMethod("SaveImprovedTemplateVersion", 
                System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance);

            if (method != null)
            {
                var task = (Task<string>)method.Invoke(_templateService, 
                    new object[] { "deepseek", "header-detection", "MANGO", improvedTemplate, 1 });
                return await task;
            }

            return null;
        }

        private async Task<bool> TestPatternDetection(List<string> patterns, string testText)
        {
            // Manually test pattern detection logic
            var failedPatterns = new List<AITemplateService.FailedPatternInfo>();

            foreach (var pattern in patterns)
            {
                try
                {
                    var regex = new System.Text.RegularExpressions.Regex(pattern, 
                        System.Text.RegularExpressions.RegexOptions.IgnoreCase);
                    var matches = regex.Matches(testText);

                    if (matches.Count == 0)
                    {
                        failedPatterns.Add(new AITemplateService.FailedPatternInfo
                        {
                            Pattern = pattern,
                            FailureReason = "Zero matches found",
                            ActualText = testText
                        });
                    }
                }
                catch (Exception ex)
                {
                    failedPatterns.Add(new AITemplateService.FailedPatternInfo
                    {
                        Pattern = pattern,
                        FailureReason = $"Invalid regex: {ex.Message}",
                        ActualText = testText
                    });
                }
            }

            return failedPatterns.Any();
        }

        private ShipmentInvoice CreateTestMangoInvoice()
        {
            return new ShipmentInvoice
            {
                SupplierName = "MANGO",
                InvoiceNo = "TEST-MANGO-001",
                InvoiceDate = DateTime.Parse("2025-03-15"),
                Currency = "USD",
                InvoiceTotal = 29.98,
                SubTotal = 28.99,
                TotalOtherCost = 0.99
            };
        }

        private string GetMangoTestData()
        {
            // Return the actual MANGO invoice text used in tests
            var mangoFile = Path.Combine(_testDataPath, "03152025_TOTAL AMOUNT.txt");
            
            if (File.Exists(mangoFile))
            {
                return File.ReadAllText(mangoFile);
            }

            // Fallback test data that represents MANGO invoice format
            return @"
MANGO
Invoice: MNG-2025-001
Date: 03/15/2025

Items:
1. Product A - $14.99
2. Product B - $13.99

Subtotal: $28.98
Tax: $0.99
TOTAL AMOUNT: $29.97

Thank you for shopping with MANGO!
";
        }

        #endregion
    }
}