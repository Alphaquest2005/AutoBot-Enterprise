using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Serilog;
using WaterNut.DataSpace; // For OCRCorrectionService and its models
using EntryDataDS.Business.Entities; // For ShipmentInvoice
using OCR.Business.Entities; // For OCRContext, Lines, Fields, RegularExpressions etc.
using System.Data.Entity; // For EF operations
using static AutoBotUtilities.Tests.TestHelpers;

namespace AutoBotUtilities.Tests.Production // Changed namespace to match others
{
    using OCRCorrectionService = WaterNut.DataSpace.OCRCorrectionService;

    [TestFixture]
    [Category("Integration")]
    [Category("Omission")]
    public class OcrOmissionIntegrationTests
    {
        private ILogger _logger;
        private OCRCorrectionService _service; // Use instance for tests
        private string _testRunId;
        private List<int> _createdFieldIds = new List<int>();
        private List<int> _createdLineIds = new List<int>();
        private List<int> _createdRegexIds = new List<int>();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _testRunId = $"OmissionTest_{DateTime.Now:yyyyMMddHHmmss}";
            _logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File($"logs/OcrOmissionIntegrationTests_{_testRunId}.log")
                .CreateLogger();
            _logger.Information("=== Starting Omission Integration Tests ===");
        }

        [SetUp]
        public void Setup()
        {
            _service = new OCRCorrectionService(_logger);
            _logger.Information("Test Setup: {TestName}", TestContext.CurrentContext.Test.Name);
        }

        [TearDown]
        public async Task TearDown()
        {
            _service?.Dispose();
            // Minimal cleanup here, robust cleanup in OneTimeTearDown
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            _logger.Information("--- Starting Omission Test Data Cleanup ---");
            using (var ctx = new OCRContext())
            {
                // Order of deletion matters: Fields -> Lines -> Regexes
                if (_createdFieldIds.Any())
                {
                    var items = await ctx.Fields.Where(x => this._createdFieldIds.Contains(x.Id)).ToListAsync().ConfigureAwait(false);
                    if (items.Any()) ctx.Fields.RemoveRange(items);
                }
                if (_createdLineIds.Any())
                {
                    var items = await ctx.Lines.Where(x => this._createdLineIds.Contains(x.Id)).ToListAsync().ConfigureAwait(false);
                    if (items.Any()) ctx.Lines.RemoveRange(items);
                }
                if (_createdRegexIds.Any())
                {
                    var items = await ctx.RegularExpressions.Where(x => this._createdRegexIds.Contains(x.Id)).ToListAsync().ConfigureAwait(false);
                    if (items.Any()) ctx.RegularExpressions.RemoveRange(items);
                }
                try { await ctx.SaveChangesAsync().ConfigureAwait(false); } catch (Exception ex) { _logger.Error(ex, "Error saving cleanup changes."); }
            }
            _logger.Information("=== Completed Omission Integration Tests ===");
        }


        // This test needs a mock for DeepSeekInvoiceApi to control regex creation response.
        // For now, it tests the flow up to the point of calling DeepSeek for regex.
        // A full integration test would require a testable DeepSeek environment or more complex mocking.
        [Test]
        [Ignore("Full end-to-end Omission with DB update requires DeepSeek mocking or live API (not suitable for unit tests). This tests the logic flow conceptually.")]
        public async Task ProcessOmissionCorrection_CreateNewLineStrategy_ShouldPrepareForDbUpdate()
        {
            // Arrange
            var invoice = CreateTestInvoiceWithOmissions(); // TotalDeduction is null
            var fileText = CreateTestInvoiceTextWithGiftCard(); // Contains "Gift Card Applied: -$6.99"

            // Simulate an omission error detected for TotalDeduction
            var omissionError = new InvoiceError
            {
                Field = "TotalDeduction", // Canonical name
                ExtractedValue = null,
                CorrectValue = "6.99", // What DeepSeek thinks it should be
                ErrorType = "omission",
                Confidence = 0.98,
                LineText = "Gift Card Applied: -$6.99", // Line from text
                LineNumber = 7, // Example line number
                ContextLinesBefore = new List<string> { "Subtotal: $100.00", "Tax: $5.00" },
                ContextLinesAfter = new List<string> { "Final Total: $98.01" },
                RequiresMultilineRegex = false
            };
            var errors = new List<InvoiceError> { omissionError };
            var metadata = InvokePrivateMethod<Dictionary<string, OCRFieldMetadata>>(_service, "ExtractFullOCRMetadata", invoice, fileText); // Get some base metadata

            // Act: Process this single error through the ApplyCorrectionsAsync path,
            // which will lead to UpdateRegexPatternsAsync
            var correctionResults = await ((Task<List<CorrectionResult>>)InvokePrivateMethod<Task<List<CorrectionResult>>>(this._service, "ApplyCorrectionsAsync", invoice, errors, fileText, metadata)).ConfigureAwait(false);

            // This part would then call UpdateRegexPatternsAsync internally.
            // For this conceptual test, we check if a CorrectionResult for DB update was generated.
            var dbUpdateCorrection = correctionResults.FirstOrDefault(cr => cr.FieldName == "TotalDeduction" && cr.CorrectionType == "omission");

            // Assert
            Assert.That(dbUpdateCorrection, Is.Not.Null, "A CorrectionResult for DB update of TotalDeduction omission should be generated.");
            Assert.That(dbUpdateCorrection.Success, Is.True, "Omission correction should be marked as success for DB processing attempt.");
            // Further assertions would involve mocking DeepSeek for regex generation and checking DB state.

            // The actual DB update would happen in UpdateRegexPatternsAsync using OmissionUpdateStrategy
            // For this test, we can manually simulate the strategy call if DeepSeek is mocked.
            if (dbUpdateCorrection != null)
            {
                // Example of how UpdateRegexPatternsAsync would proceed:
                var lineContext = InvokePrivateMethod<LineContext>(_service, "BuildLineContextForCorrection", dbUpdateCorrection, metadata, fileText);
                var request = InvokePrivateMethod<RegexUpdateRequest>(_service, "CreateUpdateRequestForStrategy", dbUpdateCorrection, lineContext, "test_path.pdf", fileText);

                // --- This is where DeepSeek would be called by OmissionUpdateStrategy ---
                // For a real test, mock _service.RequestNewRegexFromDeepSeek to return a specific RegexCreationResponse:
                // var mockRegexResponse = new RegexCreationResponse { 
                //    Strategy = "create_new_line", // or "modify_existing_line"
                //    RegexPattern = "(?<TotalDeduction>\\d+\\.\\d{2})", IsMultiline = false, MaxLines = 1, /* ... */ 
                // };
                // And then invoke strategy.ExecuteAsync.

                _logger.Information("Conceptual Omission Test: Prepared RegexUpdateRequest for {FieldName}", request.FieldName);
                Assert.That(request.CorrectionType, Is.EqualTo("omission"));
            }
        }


        [Test]
        public void MapDeepSeekFieldToDatabase_ShouldMapCommonNames()
        {
            // This test method is more suited for OCRFieldMappingTests.cs
            // but since it's in the original OcrOmissionIntegrationTests, let's keep a version here.
            var totalMapping = InvokePrivateMethod<OCRCorrectionService.DatabaseFieldInfo>(_service, "MapDeepSeekFieldToDatabase", "Total");
            Assert.That(totalMapping, Is.Not.Null, "Should map 'Total' to database field");
            Assert.That(totalMapping.DatabaseFieldName, Is.EqualTo("InvoiceTotal"), "Should map to InvoiceTotal");

            var giftCardMapping = InvokePrivateMethod<OCRCorrectionService.DatabaseFieldInfo>(_service, "MapDeepSeekFieldToDatabase", "GiftCard");
            Assert.That(giftCardMapping, Is.Not.Null, "Should map 'GiftCard' to database field");
            Assert.That(giftCardMapping.DatabaseFieldName, Is.EqualTo("TotalDeduction"), "Should map to TotalDeduction");
            _logger.Information("✓ MapDeepSeekFieldToDatabase maps common names.");
        }

        [Test]
        public void ExtractNamedGroupsFromRegex_WithComplexPattern_ShouldExtractGroups()
        {
            // Also more suited for OCRUtilitiesTests or OCRFieldMappingTests
            var regexPattern = @"(?<Total>\d+\.\d{2}).*(?<Tax>\d+\.\d{2}).*(?<Deduction>-?\$?\d+\.\d{2})";
            var namedGroups = InvokePrivateMethod<List<string>>(_service, "ExtractNamedGroupsFromRegex", regexPattern);

            Assert.That(namedGroups.Count, Is.EqualTo(3), "Should extract 3 named groups");
            Assert.That(namedGroups, Does.Contain("Total"));
            Assert.That(namedGroups, Does.Contain("Tax"));
            Assert.That(namedGroups, Does.Contain("Deduction"));
            _logger.Information("✓ ExtractNamedGroupsFromRegex works for complex patterns.");
        }


        #region Helper Methods

        private ShipmentInvoice CreateTestInvoiceWithOmissions()
        {
            return new ShipmentInvoice
            {
                InvoiceNo = $"OMI-{_testRunId}-001",
                InvoiceTotal = 105.00, // Example: SubTotal 100 + Tax 5. If Deduction is missed, this will be off.
                SubTotal = 100.00,
                TotalOtherCost = 5.00, // e.g. Tax
                TotalDeduction = null, // Omitted - text will have $6.99
                Currency = "USD",
                SupplierName = "Test Supplier Inc."
            };
        }

        private string CreateTestInvoiceTextWithGiftCard()
        {
            return @"
INVOICE #OMI-TEST-001
Date: 2024-01-15
Supplier: Test Supplier Inc.

Item 1: Product A - Qty: 1 - Price: $100.00 - Total: $100.00
Subtotal: $100.00
Tax: $5.00
Gift Card Applied: -$6.99
Final Total: $98.01
";
        }
        #endregion
    }
}