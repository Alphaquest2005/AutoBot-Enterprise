// File: AutoBotUtilities.Tests/TemplateEngine/TemplateEngineIntegrationTests.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using WaterNut.DataSpace.TemplateEngine;
using WaterNut.DataSpace.MetaAI;
using EntryDataDS.Business.Entities;
using Serilog;
using System.Text.Json;

namespace AutoBotUtilities.Tests.TemplateEngine
{
    /// <summary>
    /// Comprehensive integration tests for the template engine using actual MANGO test data.
    /// Based on log analysis from OCR correction development sessions.
    /// </summary>
    [TestClass]
    public class TemplateEngineIntegrationTests
    {
        private static ILogger _logger;
        private static HandlebarsTemplateEngine _templateEngine;
        private static string _templateBasePath;
        private static string _testDataPath;
        private static string _logOutputPath;

        [ClassInitialize]
        public static void ClassInitialize(TestContext context)
        {
            // Setup logging for comprehensive test analysis
            _logOutputPath = Path.Combine(Directory.GetCurrentDirectory(), "Logs", $"TemplateEngineTests-{DateTime.Now:yyyyMMdd}.log");
            Directory.CreateDirectory(Path.GetDirectoryName(_logOutputPath));

            _logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .WriteTo.File(_logOutputPath, 
                    outputTemplate: "{Timestamp:yyyy-MM-dd HH:mm:ss.fff zzz} [{Level:u3}] {Message:lj}{NewLine}{Exception}")
                .CreateLogger();

            // Setup template base path - create test templates directory
            _templateBasePath = Path.Combine(Directory.GetCurrentDirectory(), "TestTemplates");
            _testDataPath = Path.Combine(Directory.GetCurrentDirectory(), "TestData", "MANGO");
            
            Directory.CreateDirectory(_templateBasePath);
            Directory.CreateDirectory(_testDataPath);

            CreateTestTemplateStructure();

            // Initialize template engine with test configuration
            var config = new TemplateEngineConfig
            {
                EnableHotReload = true,
                FileWatcherDelay = TimeSpan.FromMilliseconds(50) // Faster for tests
            };

            _templateEngine = new HandlebarsTemplateEngine(_logger, _templateBasePath, config);

            _logger.Information("🧪 **TEST_CLASS_INITIALIZED**: Template engine integration tests ready");
        }

        [ClassCleanup]
        public static void ClassCleanup()
        {
            _templateEngine?.Dispose();
            _logger?.Information("🧹 **TEST_CLASS_CLEANUP**: Template engine integration tests completed");
        }

        #region MANGO Integration Tests - Based on Actual Test Data

        /// <summary>
        /// Tests template engine with actual MANGO invoice data that previously caused template creation failures.
        /// References the JSON truncation issue identified in logs at character position 70, line 324.
        /// </summary>
        [TestMethod]
        [TestCategory("MANGO")]
        [TestCategory("Integration")]
        public async Task TemplateEngine_MangoHeaderDetection_ProducesValidPrompt()
        {
            _logger.Information("🥭 **MANGO_HEADER_TEST_START**: Testing MANGO header detection template with actual invoice data");

            // Arrange - Load actual MANGO test data
            var mangoInvoiceData = CreateMangoTestInvoiceData();
            var mangoFileText = LoadMangoTestFileText();
            var mangoMetadata = CreateMangoTestMetadata();

            var context = new TemplateContext();
            context.SetInvoiceData(mangoInvoiceData);
            context.SetFileText(mangoFileText);
            context.SetMetadata(mangoMetadata);

            // Load the MANGO-specific header detection template
            var template = await _templateEngine.LoadTemplateAsync("OCR/HeaderDetection/mango-specific.hbs", "mango-header-detection");

            // Act
            var renderedPrompt = await _templateEngine.RenderAsync("mango-header-detection", context);

            // Assert
            Assert.IsNotNull(renderedPrompt, "Template should render successfully");
            Assert.IsTrue(renderedPrompt.Length > 1000, "Rendered prompt should be substantial (>1000 chars)");
            
            // Verify critical MANGO-specific content is present
            Assert.IsTrue(renderedPrompt.Contains("MANGO"), "Prompt should contain MANGO supplier reference");
            Assert.IsTrue(renderedPrompt.Contains("03152025"), "Prompt should contain test date reference");
            Assert.IsTrue(renderedPrompt.Contains("TOTAL AMOUNT"), "Prompt should contain the specific field being tested");
            
            // Verify proper JSON escaping (addresses the truncation issue found in logs)
            Assert.IsFalse(renderedPrompt.Contains("\\\\\\\\\\\\"), "Prompt should not contain excessive backslash escaping");
            Assert.IsTrue(renderedPrompt.Contains("(?<"), "Prompt should contain named capture groups");

            // Verify no placeholder variables remain unresolved
            Assert.IsFalse(renderedPrompt.Contains(" is undefined"), "No variables should be undefined");
            Assert.IsFalse(renderedPrompt.Contains("{{"), "No unresolved Handlebars variables should remain");

            _logger.Information("✅ **MANGO_HEADER_TEST_SUCCESS**: Template rendered {Length} characters with proper MANGO-specific content", 
                renderedPrompt.Length);

            // Log sample of rendered prompt for analysis
            _logger.Information("📄 **RENDERED_PROMPT_SAMPLE**: {PromptSample}...", 
                renderedPrompt.Substring(0, Math.Min(500, renderedPrompt.Length)));
        }

        /// <summary>
        /// Tests MANGO product detection template with multi-field line item extraction.
        /// Based on the InvoiceDetail entity requirements from the log analysis.
        /// </summary>
        [TestMethod]
        [TestCategory("MANGO")]
        [TestCategory("Integration")]
        public async Task TemplateEngine_MangoProductDetection_HandlesMultiFieldExtraction()
        {
            _logger.Information("🥭 **MANGO_PRODUCT_TEST_START**: Testing MANGO product detection with multi-field extraction");

            // Arrange - Create context with MANGO product data
            var mangoInvoiceWithProducts = CreateMangoInvoiceWithProductData();
            var mangoFileText = LoadMangoTestFileText();

            var context = new TemplateContext();
            context.SetInvoiceData(mangoInvoiceWithProducts);
            context.SetFileText(mangoFileText);

            // Additional context specific to product detection
            context.Variables["ocrSections"] = new[] { "Single Column", "Ripped Text", "SparseText" };
            context.Variables["expectedProducts"] = 3; // Based on MANGO test data

            var template = await _templateEngine.LoadTemplateAsync("OCR/ProductDetection/multi-field.hbs", "mango-product-detection");

            // Act
            var renderedPrompt = await _templateEngine.RenderAsync("mango-product-detection", context);

            // Assert
            Assert.IsNotNull(renderedPrompt, "Product detection template should render");
            Assert.IsTrue(renderedPrompt.Length > 2000, "Product detection prompt should be comprehensive");

            // Verify multi-field requirements from log analysis
            Assert.IsTrue(renderedPrompt.Contains("InvoiceDetail_MultiField"), "Should reference multi-field extraction");
            Assert.IsTrue(renderedPrompt.Contains("captured_fields"), "Should include captured fields array");
            Assert.IsTrue(renderedPrompt.Contains("ItemDescription"), "Should include ItemDescription field");
            Assert.IsTrue(renderedPrompt.Contains("UnitPrice"), "Should include UnitPrice field");
            Assert.IsTrue(renderedPrompt.Contains("Quantity"), "Should include Quantity field");

            // Verify OCR section handling (critical for MANGO test success)
            Assert.IsTrue(renderedPrompt.Contains("Single Column"), "Should reference Single Column OCR section");
            Assert.IsTrue(renderedPrompt.Contains("section-based analysis"), "Should include section-based approach");

            // Verify regex escaping for JSON compatibility
            Assert.IsTrue(renderedPrompt.Contains("(?<ItemDescription>"), "Should contain properly escaped named groups");
            Assert.IsTrue(renderedPrompt.Contains("RequiresMultilineRegex"), "Should handle multi-line requirements");

            _logger.Information("✅ **MANGO_PRODUCT_TEST_SUCCESS**: Product detection template validated for multi-field extraction");
        }

        /// <summary>
        /// Tests the complete MANGO template pipeline from load through render to validation.
        /// Simulates the exact workflow that was failing in the CanImportMango03152025TotalAmount_AfterLearning test.
        /// </summary>
        [TestMethod]
        [TestCategory("MANGO")]
        [TestCategory("Pipeline")]
        public async Task TemplateEngine_MangoCompletePipeline_ReplicatesOriginalWorkflow()
        {
            _logger.Information("🥭 **MANGO_PIPELINE_TEST_START**: Testing complete MANGO template pipeline");

            // Arrange - Replicate the exact context from the failing test
            var testContext = CreateFullMangoTestContext();
            
            // Act & Assert - Step through the complete pipeline
            
            // Step 1: Load header detection template
            _logger.Information("📋 **PIPELINE_STEP_1**: Loading header detection template");
            var headerTemplate = await _templateEngine.LoadTemplateAsync("OCR/HeaderDetection/mango-specific.hbs", "mango-header");
            Assert.IsNotNull(headerTemplate, "Header template should load successfully");

            // Step 2: Render header detection prompt
            _logger.Information("📋 **PIPELINE_STEP_2**: Rendering header detection prompt");
            var headerPrompt = await _templateEngine.RenderAsync("mango-header", testContext);
            Assert.IsNotNull(headerPrompt, "Header prompt should render");
            
            // Step 3: Validate header prompt structure
            _logger.Information("📋 **PIPELINE_STEP_3**: Validating header prompt structure");
            ValidatePromptStructure(headerPrompt, "header");

            // Step 4: Load product detection template
            _logger.Information("📋 **PIPELINE_STEP_4**: Loading product detection template");
            var productTemplate = await _templateEngine.LoadTemplateAsync("OCR/ProductDetection/multi-field.hbs", "mango-products");
            Assert.IsNotNull(productTemplate, "Product template should load successfully");

            // Step 5: Render product detection prompt
            _logger.Information("📋 **PIPELINE_STEP_5**: Rendering product detection prompt");
            var productPrompt = await _templateEngine.RenderAsync("mango-products", testContext);
            Assert.IsNotNull(productPrompt, "Product prompt should render");

            // Step 6: Validate product prompt structure
            _logger.Information("📋 **PIPELINE_STEP_6**: Validating product prompt structure");
            ValidatePromptStructure(productPrompt, "product");

            // Step 7: Compare lengths with original hardcoded prompts
            _logger.Information("📋 **PIPELINE_STEP_7**: Comparing with original prompt lengths");
            // Based on log analysis, original prompts were substantial
            Assert.IsTrue(headerPrompt.Length > 5000, "Header prompt should be comprehensive like original");
            Assert.IsTrue(productPrompt.Length > 8000, "Product prompt should be comprehensive like original");

            // Step 8: Validate JSON compliance (addresses the truncation issue)
            _logger.Information("📋 **PIPELINE_STEP_8**: Validating JSON compliance");
            ValidateJsonCompliance(headerPrompt);
            ValidateJsonCompliance(productPrompt);

            _logger.Information("✅ **MANGO_PIPELINE_TEST_SUCCESS**: Complete pipeline validation passed");
        }

        #endregion

        #region Hot Reload Integration Tests

        /// <summary>
        /// Tests hot reload functionality by modifying template files during execution.
        /// Critical for the "no compilation needed" requirement.
        /// </summary>
        [TestMethod]
        [TestCategory("HotReload")]
        [TestCategory("Integration")]
        public async Task TemplateEngine_HotReload_DetectsAndReloadsChanges()
        {
            _logger.Information("🔥 **HOT_RELOAD_TEST_START**: Testing hot reload functionality");

            // Arrange
            var templatePath = Path.Combine(_templateBasePath, "OCR", "HeaderDetection", "hot-reload-test.hbs");
            var originalContent = "Original template content: {{invoice.InvoiceNo}}";
            var modifiedContent = "Modified template content: {{invoice.InvoiceNo}} - UPDATED";

            // Create initial template file
            Directory.CreateDirectory(Path.GetDirectoryName(templatePath));
            await File.WriteAllTextAsync(templatePath, originalContent);

            // Load template initially
            var template = await _templateEngine.LoadTemplateAsync("OCR/HeaderDetection/hot-reload-test.hbs", "hot-reload-test");
            var context = new TemplateContext();
            context.SetInvoiceData(new { InvoiceNo = "TEST001" });

            // Render original
            var originalResult = await _templateEngine.RenderAsync("hot-reload-test", context);
            Assert.IsTrue(originalResult.Contains("Original template"), "Should render original content");

            // Act - Modify template file
            _logger.Information("📝 **MODIFYING_TEMPLATE**: Updating template file content");
            await File.WriteAllTextAsync(templatePath, modifiedContent);

            // Wait for file watcher to detect change
            await Task.Delay(200);

            // Assert - Reload should happen automatically
            var modifiedResult = await _templateEngine.RenderAsync("hot-reload-test", context);
            Assert.IsTrue(modifiedResult.Contains("UPDATED"), "Should render updated content after hot reload");
            Assert.IsFalse(modifiedResult.Contains("Original template"), "Should not contain original content");

            _logger.Information("✅ **HOT_RELOAD_TEST_SUCCESS**: Hot reload detected and applied changes automatically");
        }

        #endregion

        #region Template Validation Tests

        /// <summary>
        /// Tests template validation against OCR-specific requirements.
        /// Based on validation patterns found in log analysis.
        /// </summary>
        [TestMethod]
        [TestCategory("Validation")]
        [TestCategory("Integration")]
        public async Task TemplateEngine_Validation_CatchesOCRSpecificIssues()
        {
            _logger.Information("🔍 **VALIDATION_TEST_START**: Testing OCR-specific template validation");

            // Test Case 1: Missing named capture groups (critical OCR requirement)
            var invalidTemplate = "Invalid template with (\\d+) instead of (?<FieldName>\\d+)";
            await CreateTemporaryTemplate("invalid-regex.hbs", invalidTemplate);
            
            var validationResult = await _templateEngine.ValidateTemplateAsync("invalid-regex.hbs");
            Assert.IsFalse(validationResult.IsValid, "Should detect invalid regex patterns");
            Assert.IsTrue(validationResult.Warnings.Any(w => w.Contains("named capture")), "Should warn about missing named capture groups");

            // Test Case 2: Excessive backslash escaping (addresses log analysis findings)
            var excessiveEscapingTemplate = "Template with \\\\\\\\\\\\\\\\d+ excessive escaping";
            await CreateTemporaryTemplate("excessive-escaping.hbs", excessiveEscapingTemplate);
            
            var escapingValidation = await _templateEngine.ValidateTemplateAsync("excessive-escaping.hbs");
            Assert.IsTrue(escapingValidation.Warnings.Any(), "Should warn about excessive escaping");

            // Test Case 3: Valid OCR template with proper structure
            var validOCRTemplate = CreateValidOCRTemplate();
            await CreateTemporaryTemplate("valid-ocr.hbs", validOCRTemplate);
            
            var validValidation = await _templateEngine.ValidateTemplateAsync("valid-ocr.hbs");
            Assert.IsTrue(validValidation.IsValid, "Valid OCR template should pass validation");

            _logger.Information("✅ **VALIDATION_TEST_SUCCESS**: OCR-specific validation working correctly");
        }

        #endregion

        #region Performance and Comparison Tests

        /// <summary>
        /// Compares performance between template engine and original hardcoded prompts.
        /// Critical for production deployment validation.
        /// </summary>
        [TestMethod]
        [TestCategory("Performance")]
        [TestCategory("Integration")]
        public async Task TemplateEngine_Performance_CompareWithOriginalSystem()
        {
            _logger.Information("⚡ **PERFORMANCE_TEST_START**: Comparing template engine with original hardcoded system");

            var testIterations = 100;
            var mangoContext = CreateFullMangoTestContext();

            // Measure template engine performance
            var templateStart = DateTime.UtcNow;
            for (int i = 0; i < testIterations; i++)
            {
                await _templateEngine.RenderAsync("mango-header", mangoContext);
            }
            var templateDuration = DateTime.UtcNow - templateStart;

            // Measure original hardcoded system performance (simulation)
            var originalStart = DateTime.UtcNow;
            for (int i = 0; i < testIterations; i++)
            {
                // Simulate original prompt creation time
                SimulateOriginalPromptCreation(mangoContext);
            }
            var originalDuration = DateTime.UtcNow - originalStart;

            // Assert performance is reasonable
            var performanceRatio = templateDuration.TotalMilliseconds / originalDuration.TotalMilliseconds;
            Assert.IsTrue(performanceRatio < 5.0, $"Template engine should not be more than 5x slower than original (ratio: {performanceRatio:F2})");

            _logger.Information("⚡ **PERFORMANCE_RESULTS**: Template={TemplateMs}ms, Original={OriginalMs}ms, Ratio={Ratio:F2}", 
                templateDuration.TotalMilliseconds, originalDuration.TotalMilliseconds, performanceRatio);
        }

        #endregion

        #region Helper Methods

        private static void CreateTestTemplateStructure()
        {
            var directories = new[]
            {
                "OCR/HeaderDetection",
                "OCR/ProductDetection",
                "OCR/DirectCorrection",
                "OCR/Shared",
                "MetaAI",
                "System/backup"
            };

            foreach (var dir in directories)
            {
                Directory.CreateDirectory(Path.Combine(_templateBasePath, dir));
            }

            // Create test templates
            CreateMangoSpecificHeaderTemplate();
            CreateMultiFieldProductTemplate();
            CreateSharedVariablesFile();
        }

        private static void CreateMangoSpecificHeaderTemplate()
        {
            var templateContent = @"OBJECT-ORIENTED INVOICE ANALYSIS (V14.0 - Business Entity Framework):

**CONTEXT:**
You are analyzing a MANGO supplier invoice with defined object schemas. This invoice type requires specific handling for {{supplier.SupplierName}} format patterns.

**1. EXTRACTED FIELDS:**
{{toJson invoice}}

**2. COMPLETE OCR TEXT:**
{{fileText}}

**🎯 MANGO-SPECIFIC REQUIREMENTS:**
- Look for TOTAL AMOUNT field specifically (test case: {{testData.targetField}})
- Date format: {{testData.expectedDate}}
- Currency handling for fashion retailer invoices
- Multi-line address parsing for European suppliers

**CRITICAL REGEX REQUIREMENTS:**
⚠️ **MANDATORY**: ALL regex patterns MUST use named capture groups: (?<FieldName>pattern)
⚠️ **EXAMPLE CORRECT**: ""{{escapeForJson 'Order Total:\s*\$(?<InvoiceTotal>[\d,]+\.?\d*)'}}""

{{#if metadata.balanceDiscrepancy}}
**BALANCE CHECK:**
Current discrepancy: {{metadata.balanceDiscrepancy}}
{{/if}}

**RESPONSE FORMAT:** JSON with errors array containing complete field information.";

            File.WriteAllText(Path.Combine(_templateBasePath, "OCR", "HeaderDetection", "mango-specific.hbs"), templateContent);
        }

        private static void CreateMultiFieldProductTemplate()
        {
            var templateContent = @"OBJECT-ORIENTED INVOICEDETAIL ENTITY DETECTION (V14.0):

**YOUR TASK**: Identify missing or incomplete InvoiceDetail business objects for {{supplier.SupplierName}} invoices.

**EXTRACTED INVOICEDETAIL OBJECTS:**
{{toJson invoice.InvoiceDetails}}

**ORIGINAL INVOICE TEXT:**
{{truncate fileText 1000}}

**🎯 OBJECT-ORIENTED FIELD NAMING RULES:**

**COMPLETE INVOICEDETAIL OBJECTS (Preferred approach):**
- Field Name: InvoiceDetail_MultiField_LineX_Y (where X-Y spans all lines of the object)
- Required: captured_fields array listing ALL properties [Quantity, ItemDescription, UnitPrice, LineTotal]
- Required: RequiresMultilineRegex: true (if object spans multiple lines)

**CRITICAL REQUIREMENTS:**
1. ✅ **field**: The exact field name (NEVER null)
2. ✅ **correct_value**: The actual value from the OCR text (NEVER null)
3. ✅ **suggested_regex**: A working regex pattern with named capture groups (NEVER null)

**MANDATORY REGEX VALIDATION:**
- Pattern matches line_text completely
- Extracts correct_value accurately
- Uses named capture groups (?<FieldName>pattern) - never numbered (pattern)

**OCR SECTION ANALYSIS:**
{{#each ocrSections}}
- Section: {{this}} - Analyze for complete product entities
{{/each}}

Return JSON with errors array containing multi-field omission objects.";

            File.WriteAllText(Path.Combine(_templateBasePath, "OCR", "ProductDetection", "multi-field.hbs"), templateContent);
        }

        private static void CreateSharedVariablesFile()
        {
            var sharedVariables = new
            {
                regexEscaping = new
                {
                    jsonLevel = @"\\",
                    documentationLevel = @"\\\\",
                    validationLevel = @"\\\"
                },
                fieldMappings = new
                {
                    DeepSeekToDatabase = new
                    {
                        UnitPrice = "Cost",
                        ItemCode = "ItemNumber",
                        LineTotal = "TotalCost"
                    }
                },
                commonPatterns = new
                {
                    currency = @"[\$€£]",
                    decimal = @"[\d,]+\.?\d*",
                    namedGroup = @"(?<{0}>{1})"
                }
            };

            var json = JsonSerializer.Serialize(sharedVariables, new JsonSerializerOptions { WriteIndented = true });
            File.WriteAllText(Path.Combine(_templateBasePath, "OCR", "Shared", "common-variables.json"), json);
        }

        private ShipmentInvoice CreateMangoTestInvoiceData()
        {
            return new ShipmentInvoice
            {
                InvoiceNo = "MANGO-03152025",
                InvoiceDate = DateTime.Parse("2025-03-15"),
                SupplierName = "MANGO",
                Currency = "EUR",
                SubTotal = 196.33,
                InvoiceTotal = 210.08,
                TotalInternalFreight = 0,
                TotalOtherCost = 13.74, // Tax
                TotalInsurance = 0,
                TotalDeduction = 0
            };
        }

        private string LoadMangoTestFileText()
        {
            // Simulate MANGO OCR text that was causing issues
            return @"--Single Column--

MANGO Invoice
Date: March 15, 2025
Invoice No: MANGO-03152025

Items:
High-waist shorts € 3.30
Long jumpsuit € 18.99
Mixed necklace € 10.99

Subtotal: €196.33
Tax: €13.74
TOTAL AMOUNT: €210.08

--Ripped Text--

MANGO Invoice
Date March 15 2025
Invoice No MANGO 03152025

Items
High waist shorts 3 30
Long jumpsuit 18 99
Mixed necklace 10 99

Subtotal 196 33
Tax 13 74
TOTAL AMOUNT 210 08

--SparseText--

M A N G O   I n v o i c e
D a t e :   M a r c h   1 5 ,   2 0 2 5

T O T A L   A M O U N T :   € 2 1 0 . 0 8";
        }

        private Dictionary<string, object> CreateMangoTestMetadata()
        {
            return new Dictionary<string, object>
            {
                ["testData"] = new
                {
                    targetField = "TOTAL AMOUNT",
                    expectedDate = "03152025",
                    expectedTotal = 210.08
                },
                ["supplier"] = new
                {
                    SupplierName = "MANGO"
                },
                ["balanceDiscrepancy"] = 0.0,
                ["ocrSections"] = new[] { "Single Column", "Ripped Text", "SparseText" }
            };
        }

        private ShipmentInvoice CreateMangoInvoiceWithProductData()
        {
            var invoice = CreateMangoTestInvoiceData();
            invoice.InvoiceDetails = new List<ShipmentInvoiceDetails>
            {
                new ShipmentInvoiceDetails
                {
                    ItemDescription = "High-waist shorts",
                    Quantity = 1,
                    Cost = 3.30,
                    TotalCost = 3.30
                },
                new ShipmentInvoiceDetails
                {
                    ItemDescription = "Long jumpsuit",
                    Quantity = 1,
                    Cost = 18.99,
                    TotalCost = 18.99
                },
                new ShipmentInvoiceDetails
                {
                    ItemDescription = "Mixed necklace",
                    Quantity = 1,
                    Cost = 10.99,
                    TotalCost = 10.99
                }
            };
            return invoice;
        }

        private TemplateContext CreateFullMangoTestContext()
        {
            var context = new TemplateContext();
            context.SetInvoiceData(CreateMangoInvoiceWithProductData());
            context.SetFileText(LoadMangoTestFileText());
            context.SetMetadata(CreateMangoTestMetadata());
            
            // Additional context from log analysis
            context.Variables["ocrSections"] = new[] { "Single Column", "Ripped Text", "SparseText" };
            context.Variables["supplier"] = new { SupplierName = "MANGO" };
            context.Variables["testData"] = new 
            { 
                targetField = "TOTAL AMOUNT", 
                expectedDate = "03152025",
                expectedTotal = 210.08
            };

            return context;
        }

        private void ValidatePromptStructure(string prompt, string promptType)
        {
            Assert.IsFalse(string.IsNullOrWhiteSpace(prompt), $"{promptType} prompt should not be empty");
            Assert.IsTrue(prompt.Contains("(?<"), $"{promptType} prompt should contain named capture groups");
            Assert.IsFalse(prompt.Contains("{{"), $"{promptType} prompt should not contain unresolved variables");
            Assert.IsFalse(prompt.Contains(" is undefined"), $"{promptType} prompt should not contain undefined variables");
            
            // OCR-specific validations
            Assert.IsTrue(prompt.Contains("field"), $"{promptType} prompt should reference field extraction");
            Assert.IsTrue(prompt.Contains("correct_value"), $"{promptType} prompt should reference correct values");
            Assert.IsTrue(prompt.Contains("suggested_regex"), $"{promptType} prompt should reference regex patterns");
        }

        private void ValidateJsonCompliance(string prompt)
        {
            // Check for the JSON truncation issue that was identified in logs
            Assert.IsFalse(prompt.Contains("\"correct_val'"), "Prompt should not contain truncated JSON");
            Assert.IsFalse(prompt.Contains("\\\\\\\\\\\\"), "Prompt should not contain excessive escaping");
            
            // Verify proper JSON structure expectations
            if (prompt.Contains("JSON"))
            {
                Assert.IsTrue(prompt.Contains("errors"), "JSON prompts should reference errors array");
                Assert.IsTrue(prompt.Contains("field"), "JSON prompts should reference field property");
            }
        }

        private async Task CreateTemporaryTemplate(string fileName, string content)
        {
            var path = Path.Combine(_templateBasePath, fileName);
            await File.WriteAllTextAsync(path, content);
        }

        private string CreateValidOCRTemplate()
        {
            return @"Valid OCR Template with proper structure:

Find missing fields in the invoice data.

Required regex format: {{escapeForJson 'Free Shipping:\s*-?\$?(?<TotalDeduction>[\d,]+\.?\d*)'}}

Return JSON format:
{
  ""errors"": [
    {
      ""field"": ""{{fieldName}}"",
      ""correct_value"": ""{{correctValue}}"",
      ""suggested_regex"": ""{{escapeForJson regex}}""
    }
  ]
}";
        }

        private void SimulateOriginalPromptCreation(TemplateContext context)
        {
            // Simulate the string concatenation and escaping that the original system did
            var invoice = context.GetVariable<object>("invoice");
            var fileText = context.GetVariable<string>("fileText");
            
            // Simulate processing time
            var result = $"Hardcoded prompt with {invoice} and {fileText?.Length ?? 0} chars";
            System.Threading.Thread.Sleep(1); // Minimal processing delay
        }

        #endregion
    }
}