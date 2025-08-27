using System;
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
    /// Demonstrates complete template creation from DeepSeek analysis.
    /// This test shows how to create OCR templates for any unknown supplier using actual OCR text.
    /// </summary>
    [TestFixture]
    public class TemplateCreationTest
    {
        private ILogger _logger;
        private OCRCorrectionService _ocrService;
        private const string TestDataPath = @"C:\Insight Software\AutoBot-Enterprise\AutoBotUtilities.Tests\Test Data";

        [SetUp]
        public void Setup()
        {
            _logger = new LoggerConfiguration()
                .MinimumLevel.Verbose()
                .WriteTo.Console()
                .WriteTo.NUnitOutput()
                .CreateLogger();
            
            _ocrService = new OCRCorrectionService(_logger);
        }

        [TearDown]
        public void TearDown()
        {
            _ocrService?.Dispose();
        }

        /// <summary>
        /// 🎯 **DEMONSTRATION**: Creates a complete MANGO template from scratch using actual MANGO OCR text.
        /// This test shows the complete pipeline: DeepSeek analysis → Template creation → Database integration.
        /// </summary>
        [Test]
        public async Task CreateMangoTemplate_FromActualOCRText_ShouldCreateCompleteTemplate()
        {
            _logger.Information("🏗️ **TEMPLATE_CREATION_DEMO_START**: Demonstrating template creation for MANGO supplier");

            // **STEP 1**: Load actual MANGO OCR text
            var mangoTextPath = Path.Combine(TestDataPath, "03152025_TOTAL AMOUNT.txt");
            
            if (!File.Exists(mangoTextPath))
            {
                Assert.Fail($"❌ **TEST_DATA_MISSING**: MANGO test file not found at {mangoTextPath}");
                return;
            }

            var mangoOcrText = File.ReadAllText(mangoTextPath);
            _logger.Information("📄 **OCR_TEXT_LOADED**: Loaded {CharCount} characters of MANGO OCR text", mangoOcrText.Length);

            // **STEP 2**: Create template using the consolidated production method
            _logger.Information("🚀 **TEMPLATE_CREATION_START**: Creating MANGO template from DeepSeek analysis");
            var templates = await _ocrService.CreateInvoiceTemplateAsync(mangoOcrText, mangoTextPath).ConfigureAwait(false);

            // **STEP 3**: Verify results and log comprehensive details
            if (templates != null && templates.Any())
            {
                _logger.Information("✅ **VERIFICATION_SUCCESS**: Templates created successfully");
                _logger.Information("   - Templates Count: {TemplateCount}", templates.Count);
                
                // Use first template for existing single-template test logic
                var template = templates.First();
                
                _logger.Information("   - Template ID: {TemplateId}", template.OcrTemplates?.Id);
                _logger.Information("   - Template Name: {TemplateName}", template.OcrTemplates?.Name);
                _logger.Information("   - Parts Count: {PartsCount}", template.Parts?.Count ?? 0);
                _logger.Information("   - Lines Count: {LinesCount}", template.Lines?.Count ?? 0);
                _logger.Information("   - FileType: {FileType}", template.FileType?.FileImporterInfos?.EntryType);

                // Verify minimum expected entities were created
                Assert.That(template.OcrTemplates?.Id, Is.Not.Null, "Template should have been created with an ID");
                Assert.That(template.Parts?.Count ?? 0, Is.GreaterThan(0), "At least one part should have been created");
                Assert.That(template.Lines?.Count ?? 0, Is.GreaterThan(0), "At least one line should have been created");

                _logger.Information("🎯 **TEMPLATE_VALIDATION_PASSED**: All assertions passed - template creation successful");
            }
            else
            {
                _logger.Error("❌ **TEMPLATE_CREATION_FAILED**: CreateInvoiceTemplateAsync returned null or empty");
                Assert.Fail("Template creation failed: CreateInvoiceTemplateAsync returned null or empty");
            }
        }

        /// <summary>
        /// 🎯 **PRODUCTION_EXAMPLE**: Shows how to create templates for any new supplier.
        /// This method demonstrates the exact pattern to use in production for unknown suppliers.
        /// </summary>
        [Test]
        public async Task CreateTemplate_ProductionPattern_ShouldWorkForAnySupplier()
        {
            _logger.Information("🏭 **PRODUCTION_PATTERN_DEMO**: Demonstrating production template creation pattern");

            // This is the exact pattern you would use in production for any unknown supplier:
            
            // 1. Get supplier name from invoice or filename
            string supplierName = "MANGO";  // In production, extract from invoice data
            
            // 2. Get OCR text from the invoice
            var ocrTextPath = Path.Combine(TestDataPath, "03152025_TOTAL AMOUNT.txt");
            string ocrText = File.ReadAllText(ocrTextPath);
            
            // 3. Optional: Create sample invoice data for better context
            var sampleInvoice = new ShipmentInvoice
            {
                InvoiceNo = "UCSJIB6",           // Extracted from OCR
                SupplierName = "MANGO OUTLET",   // Extracted from OCR
                InvoiceDate = DateTime.Parse("2024-07-23"), // Converted to standard format
                InvoiceTotal = (double?)210.08m,          // Extracted from OCR
                SubTotal = (double?)196.33m,              // Extracted from OCR
                Currency = "USD"                 // Standardized from USS/US$
            };

            _logger.Information("📋 **PRODUCTION_INPUTS**: Supplier='{Supplier}', OCRLength={OCRLength}, SampleInvoice='{InvoiceNo}'", 
                supplierName, ocrText.Length, sampleInvoice.InvoiceNo);

            // 4. Create the template (this is the main production call)
            var templates = await _ocrService.CreateInvoiceTemplateAsync(ocrText, "production_sample.pdf").ConfigureAwait(false);

            // 5. Handle the result (production error handling)
            if (templates != null && templates.Any())
            {
                _logger.Information("🎯 **PRODUCTION_SUCCESS**: {TemplateCount} templates created for future invoice processing", templates.Count);
                
                // Use first template for existing single-template test logic
                var template = templates.First();
                
                // In production, you might:
                // - Log the template creation for audit
                // - Update configuration to include the new template
                // - Notify administrators of the new supplier
                // - Re-process the original invoice with the new template
                
                Assert.Pass($"Templates created successfully: Count={templates.Count}, First Template: ID={template.OcrTemplates?.Id}, Name={template.OcrTemplates?.Name}");
            }
            else
            {
                _logger.Error("❌ **PRODUCTION_FAILURE**: Template creation failed - CreateInvoiceTemplateAsync returned null or empty");
                
                // In production, you might:
                // - Alert administrators to manual template creation needed
                // - Queue the invoice for manual processing
                // - Log the failure for analysis
                
                Assert.Fail("Production template creation failed: CreateInvoiceTemplateAsync returned null or empty");
            }
        }

        /// <summary>
        /// 🔧 **DATABASE_VERIFICATION**: Demonstrates how to verify created templates in the database.
        /// This shows the exact database queries to validate template creation success.
        /// </summary>
        [Test]
        [Explicit("Manual verification test - run only when debugging database integration")]
        public void VerifyDatabaseIntegration_AfterTemplateCreation_ShouldShowAllEntities()
        {
            _logger.Information("🗄️ **DATABASE_VERIFICATION_DEMO**: Showing database verification commands");

            // These are the exact SQL commands to verify template creation:
            var verificationCommands = new[]
            {
                @"-- Verify template creation (OCR-Invoices)
SELECT TOP 5 Id, Name, FileTypeId, ApplicationSettingsId, IsActive 
FROM [OCR-Invoices] 
WHERE Name = 'MANGO' 
ORDER BY Id DESC;",

                @"-- Verify parts creation (OCR-Parts)  
SELECT p.Id, p.TemplateId, p.PartTypeId, t.Name as TemplateName
FROM [OCR-Parts] p
INNER JOIN [OCR-Invoices] t ON p.TemplateId = t.Id
WHERE t.Name = 'MANGO'
ORDER BY p.Id DESC;",

                @"-- Verify lines creation (OCR-Lines)
SELECT l.Id, l.Name, l.PartId, r.RegEx, r.Description
FROM [OCR-Lines] l
INNER JOIN [OCR-Parts] p ON l.PartId = p.Id  
INNER JOIN [OCR-Invoices] t ON p.TemplateId = t.Id
INNER JOIN [OCR-RegularExpressions] r ON l.RegularExpressionsId = r.Id
WHERE t.Name = 'MANGO'
ORDER BY l.Id DESC;",

                @"-- Verify fields creation (OCR-Fields)
SELECT f.Id, f.Field, f.Key, f.LineId, f.DisplayName, f.DataType, f.IsRequired
FROM [OCR-Fields] f
INNER JOIN [OCR-Lines] l ON f.LineId = l.Id
INNER JOIN [OCR-Parts] p ON l.PartId = p.Id
INNER JOIN [OCR-Invoices] t ON p.TemplateId = t.Id  
WHERE t.Name = 'MANGO'
ORDER BY f.Id DESC;",

                @"-- Verify format corrections (OCR_FieldFormatRegEx)
SELECT ff.Id, f.Field, f.Key, pr.RegEx as Pattern, rr.RegEx as Replacement
FROM OCR_FieldFormatRegEx ff
INNER JOIN [OCR-Fields] f ON ff.FieldId = f.Id
INNER JOIN [OCR-RegularExpressions] pr ON ff.RegularExpressionsId = pr.Id
INNER JOIN [OCR-RegularExpressions] rr ON ff.ReplacementRegularExpressionsId = rr.Id  
INNER JOIN [OCR-Lines] l ON f.LineId = l.Id
INNER JOIN [OCR-Parts] p ON l.PartId = p.Id
INNER JOIN [OCR-Invoices] t ON p.TemplateId = t.Id
WHERE t.Name = 'MANGO'
ORDER BY ff.Id DESC;"
            };

            foreach (var command in verificationCommands)
            {
                _logger.Information("📝 **DATABASE_QUERY**:\n{SQLCommand}\n", command);
            }

            // Database connection info (from App.config):
            _logger.Information("🔌 **DATABASE_CONNECTION**: Server='MINIJOE\\SQLDEVELOPER2022', Database='WebSource-AutoBot'");
            _logger.Information("💡 **VERIFICATION_INSTRUCTIONS**: Run these SQL commands to verify template creation in SQL Server Management Studio");
            
            Assert.Pass("Database verification commands logged - check output for SQL queries");
        }
    }
}