using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Serilog;
using EntryDataDS.Business.Entities; // For ShipmentInvoice
using WaterNut.DataSpace;            // For OCRCorrectionService
using OCR.Business.Entities;         // For OCRContext, DB entities for verification
using System.Data.Entity;

namespace AutoBotUtilities.Tests.Production
{
    using OCRCorrectionService = WaterNut.DataSpace.OCRCorrectionService;

    [TestFixture]
    [Category("Orchestration")]
    [Category("Integration")]
    [Category("LiveAPI")] // These tests will make live API calls
    public class OCRCorrectionService_MainOrchestrationTests
    {
        private ILogger _logger;
        private OCRCorrectionService _service;
        private string _testRunId;

        // Track IDs for cleanup
        private List<int> _createdRegexIds;
        private List<int> _createdFieldFormatIds;
        private List<int> _createdFieldIds;
        private List<int> _createdLineIds;
        private List<int> _createdLearningIds;

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _testRunId = $"OrchestrationTest_{DateTime.Now:yyyyMMddHHmmss}";
            _logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File($"logs/OCRMainOrchestrationTests_{_testRunId}.log")
                .CreateLogger();
            _logger.Information("=== Starting Main Orchestration Tests (Live API) ===");

            _createdRegexIds = new List<int>();
            _createdFieldFormatIds = new List<int>();
            _createdFieldIds = new List<int>();
            _createdLineIds = new List<int>();
            _createdLearningIds = new List<int>();
        }

        [SetUp]
        public void SetUp()
        {
            _service = new OCRCorrectionService(_logger);
            _logger.Information("Test Setup for: {TestName}", TestContext.CurrentContext.Test.Name);
        }

        [TearDown]
        public void TearDown()
        {
            _service?.Dispose();
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            _logger.Information("--- Starting Orchestration Test Data Cleanup ---");
            // Simplified Cleanup - a more robust version would be in a shared test helper
            using (var ctx = new OCRContext())
            {
                if (_createdLearningIds.Any()) { var items = await ctx.OCRCorrectionLearning.Where(x => this._createdLearningIds.Contains(x.Id)).ToListAsync().ConfigureAwait(false); if (items.Any()) ctx.OCRCorrectionLearning.RemoveRange(items); }
                if (_createdFieldFormatIds.Any()) { var items = await ctx.OCR_FieldFormatRegEx.Where(x => this._createdFieldFormatIds.Contains(x.Id)).ToListAsync().ConfigureAwait(false); if (items.Any()) ctx.OCR_FieldFormatRegEx.RemoveRange(items); }
                if (_createdFieldIds.Any()) { var items = await ctx.Fields.Where(x => this._createdFieldIds.Contains(x.Id)).ToListAsync().ConfigureAwait(false); if (items.Any()) ctx.Fields.RemoveRange(items); }
                if (_createdLineIds.Any()) { var items = await ctx.Lines.Where(x => this._createdLineIds.Contains(x.Id)).ToListAsync().ConfigureAwait(false); if (items.Any()) ctx.Lines.RemoveRange(items); }
                if (_createdRegexIds.Any())
                {
                    var regexesInUseByFieldFormat = await ctx.OCR_FieldFormatRegEx.Where(ffr => !this._createdFieldFormatIds.Contains(ffr.Id) && ((this._createdRegexIds.Contains(ffr.RegEx.Id)) || (this._createdRegexIds.Contains(ffr.ReplacementRegEx.Id)))).SelectMany(ffr => new[] { ffr.RegExId, ffr.ReplacementRegExId }).Select(id => id).Distinct().ToListAsync().ConfigureAwait(false);
                    var regexesInUseByLines = await ctx.Lines.Where(l => !this._createdLineIds.Contains(l.Id) &&  this._createdRegexIds.Contains(l.RegularExpressions.Id)).Select(l => l.RegularExpressions.Id).Distinct().ToListAsync().ConfigureAwait(false);
                    var regexesToDelete = _createdRegexIds.Except(regexesInUseByFieldFormat).Except(regexesInUseByLines).ToList();
                    if (regexesToDelete.Any()) { var items = await ctx.RegularExpressions.Where(x => regexesToDelete.Contains(x.Id)).ToListAsync().ConfigureAwait(false); if (items.Any()) ctx.RegularExpressions.RemoveRange(items); }
                }
                try { await ctx.SaveChangesAsync().ConfigureAwait(false); } catch (Exception ex) { _logger.Error(ex, "Error saving cleanup changes for orchestration tests."); }
            }
            _logger.Information("=== Completed Main Orchestration Tests ===");
        }

        #region CorrectInvoiceAsync Tests
        [Test]
        public async Task CorrectInvoiceAsync_InvoiceWithOmission_ShouldCorrectAndLearn()
        {
            // Arrange
            var invoiceNumber = $"ORCH-OMIT-{_testRunId.Substring(0, 6)}";
            var invoice = new ShipmentInvoice
            {
                InvoiceNo = invoiceNumber,
                SubTotal = 100.00,
                InvoiceTotal = 100.00 // Missing TotalDeduction
            };
            // Text clearly shows a deduction
            var fileText = $"Invoice: {invoiceNumber}\nItem: Product A, Price: $100.00\nSubtotal: $100.00\nDiscount Applied: -$10.00\nTotal Due: $90.00";

            // Act
            bool correctionResult = await this._service.CorrectInvoiceAsync(invoice, fileText).ConfigureAwait(false);

            // Assert
            Assert.That(correctionResult, Is.True, "CorrectInvoiceAsync should return true indicating changes or balance.");
            Assert.That(invoice.TotalDeduction, Is.EqualTo(10.00).Within(0.01), "TotalDeduction should be corrected in-memory.");
            Assert.That(OCRCorrectionService.TotalsZero(invoice, out _, _logger), Is.True, "Invoice should be balanced after correction.");

            // Verify database learning (conceptual - check for new Field and Regex related to TotalDeduction)
            using (var ctx = new OCRContext())
            {
                var learningEntry = await ctx.OCRCorrectionLearning
                                        .OrderByDescending(l => l.CreatedDate)
                                        .FirstOrDefaultAsync(l => l.FieldName == "TotalDeduction" && l.OriginalError == "" && l.CorrectValue == "10.00").ConfigureAwait(false); // Approximate check
                Assert.That(learningEntry, Is.Not.Null, "A learning entry for the TotalDeduction omission should exist.");
                _createdLearningIds.Add(learningEntry.Id);

                // Check if a new Field for TotalDeduction was potentially created by OmissionStrategy
                // This is complex as it depends on existing template structure.
                // For this test, we're mainly concerned that the orchestration attempted to learn.
                // A more specific test for OmissionUpdateStrategy would verify DB entities directly.
                _logger.Information("DB Learning: Found learning entry ID {LearningId} for TotalDeduction omission.", learningEntry.Id);
                var newFieldForDeduction = await ctx.Fields
                                               .Include(f => f.Lines.RegularExpressions)
                                               .Where(f => f.Key == "TotalDeduction" || f.Field == "TotalDeduction") // Omission strategy might use FieldName as Key
                                               .OrderByDescending(f => f.Id)
                                               .FirstOrDefaultAsync().ConfigureAwait(false);

                if (newFieldForDeduction != null && newFieldForDeduction.Id > 0) // crude check if recently created (removed CreatedDate check)
                {
                    _logger.Information("Found a potentially new/updated Field for TotalDeduction: FieldID {FieldId}, LineID {LineId}, Regex: '{Regex}'",
                                       newFieldForDeduction.Id, newFieldForDeduction.LineId, newFieldForDeduction.Lines?.RegularExpressions?.RegEx);
                    _createdFieldIds.Add(newFieldForDeduction.Id);
                    if (newFieldForDeduction.Lines != null) _createdLineIds.Add(newFieldForDeduction.LineId);
                    if (newFieldForDeduction.Lines?.RegularExpressions != null) _createdRegexIds.Add(newFieldForDeduction.Lines.RegularExpressions.Id);
                }
                else
                {
                    _logger.Warning("Could not definitively confirm a new Field/Line/Regex for TotalDeduction was created in this test run, or it existed previously.");
                }
            }
            _logger.Information("? CorrectInvoiceAsync_InvoiceWithOmission_ShouldCorrectAndLearn passed.");
        }

        [Test]
        public async Task CorrectInvoiceAsync_InvoiceWithFormatError_ShouldCorrectAndLearn()
        {
            var invoiceNumber = $"ORCH-FORMAT-{_testRunId.Substring(0, 6)}";
            var invoice = new ShipmentInvoice
            {
                InvoiceNo = invoiceNumber,
                SubTotal = 100.00,
                InvoiceTotal = (double)12345m //OCRed as "123,45"
            };
            var fileText = $"Invoice: {invoiceNumber}\nSubtotal: $100.00\nTotal: $123,45"; // Format error for total

            // To ensure FieldFormatStrategy can run, we need a Fields entry.
            Fields testFieldInvoiceTotal;
            using (var ctxSetup = new OCRContext())
            {
                var testPart = await this.GetOrCreateTestPartAsync(ctxSetup, "Header").ConfigureAwait(false); // Ensure a Header part exists
                var testLine = await this.GetOrCreateTestLineAsync(ctxSetup, testPart.Id, $"LineFor_{invoiceNumber}").ConfigureAwait(false);
                testFieldInvoiceTotal = await this.GetOrCreateTestFieldAsync(ctxSetup, "InvoiceTotal", testLine.Id, "ShipmentInvoice", "decimal").ConfigureAwait(false);
            }
            // Patch the extracted value to simulate OCR error after standard conversion
            invoice.InvoiceTotal = 12345; // Simulating "123,45" that got parsed as 12345 by a naive parser.
                                          // The DeepSeek error detection should identify "123,45" -> "123.45"

            // The error detection will use the original fileText.
            // The ProcessDeepSeekCorrectionResponse will return a CorrectionResult with OldValue="123,45" and NewValue="123.45" for "InvoiceTotal".
            // Then UpdateRegexPatternsAsync will get metadata for "InvoiceTotal", which includes testFieldInvoiceTotal.Id.
            // This FieldId is passed to FieldFormatUpdateStrategy.

            // Act
            bool correctionResult = await this._service.CorrectInvoiceAsync(invoice, fileText).ConfigureAwait(false);

            // Assert
            Assert.That(correctionResult, Is.True, "CorrectInvoiceAsync should return true.");
            Assert.That(invoice.InvoiceTotal, Is.EqualTo(123.45).Within(0.01), "InvoiceTotal should be corrected in-memory.");
            Assert.That(OCRCorrectionService.TotalsZero(invoice, out _, _logger), Is.True, "Invoice should be balanced.");

            using (var ctx = new OCRContext())
            {
                var fieldFormatEntry = await ctx.OCR_FieldFormatRegEx
                                           .Include(ffr => ffr.RegEx)
                                           .Include(ffr => ffr.ReplacementRegEx)
                                           .FirstOrDefaultAsync(ffr => ffr.FieldId == testFieldInvoiceTotal.Id && ffr.RegEx.RegEx == @"(\d+),(\d{1,4})").ConfigureAwait(false);

                Assert.That(fieldFormatEntry, Is.Not.Null, "FieldFormatRegEx for decimal correction should be created.");
                Assert.That(fieldFormatEntry.ReplacementRegEx.RegEx, Is.EqualTo("$1.$2"));
                _createdFieldFormatIds.Add(fieldFormatEntry.Id);
                _createdRegexIds.Add(fieldFormatEntry.RegEx.Id);
                _createdRegexIds.Add(fieldFormatEntry.ReplacementRegEx.Id);

                var learningEntry = await ctx.OCRCorrectionLearning
                                        .OrderByDescending(l => l.CreatedDate)
                                        .FirstOrDefaultAsync(l => l.FieldName == "InvoiceTotal" && l.OriginalError == "123,45").ConfigureAwait(false);
                Assert.That(learningEntry, Is.Not.Null, "Learning entry for format error should exist.");
                _createdLearningIds.Add(learningEntry.Id);
            }
            _logger.Information("? CorrectInvoiceAsync_InvoiceWithFormatError_ShouldCorrectAndLearn passed.");
        }

        #endregion

        // CorrectInvoicesAsync (batch) and CorrectInvoiceWithRegexUpdatesAsync are orchestrators
        // similar to CorrectInvoiceAsync but with slightly different input/output or scope.
        // Their tests would follow similar patterns:
        // 1. Setup invoice(s) with known errors (omissions, format issues).
        // 2. Provide corresponding file text.
        // 3. Setup prerequisite DB entities (like Fields for FieldFormatStrategy) if not handled by omission strategy.
        // 4. Call the orchestration method.
        // 5. Assert changes to in-memory invoice objects.
        // 6. Assert creation/update of relevant DB entities (OCRCorrectionLearning, FieldFormatRegEx, new Lines/Fields/Regexes for omissions).
        // These tests become quite involved due to the live API calls and DB state management.

        // Helper to create a dummy Field definition for tests
        private async Task<Fields> GetOrCreateTestFieldAsync(OCRContext ctx, string fieldName, int lineId, string entityType = "ShipmentInvoice", string dataType = "string")
        {
            // Simplified: assumes field key and name are the same for test setup ease
            var field = await ctx.Fields.FirstOrDefaultAsync(f => f.LineId == lineId && f.Field == fieldName && f.Key == fieldName).ConfigureAwait(false);
            if (field == null)
            {
                field = new Fields { LineId = lineId, Field = fieldName, Key = fieldName, EntityType = entityType, DataType = dataType, TrackingState = TrackableEntities.TrackingState.Added };
                ctx.Fields.Add(field);
                await ctx.SaveChangesAsync().ConfigureAwait(false);
                _createdFieldIds.Add(field.Id);
            }
            return field;
        }

        private async Task<Lines> GetOrCreateTestLineAsync(OCRContext ctx, int partId, string lineName = null, string initialRegex = ".*")
        {
            lineName = lineName ?? $"OrchTestLine_{_testRunId}_{Guid.NewGuid().ToString("N").Substring(0, 4)}";
            var line = await ctx.Lines.FirstOrDefaultAsync(l => l.Name == lineName && l.PartId == partId).ConfigureAwait(false);
            if (line == null)
            {
                var dummyRegex = new RegularExpressions { RegEx = initialRegex, Description = $"Dummy for {lineName}", TrackingState = TrackableEntities.TrackingState.Added };
                ctx.RegularExpressions.Add(dummyRegex);
                await ctx.SaveChangesAsync().ConfigureAwait(false);
                _createdRegexIds.Add(dummyRegex.Id);
                line = new Lines { Name = lineName, PartId = partId, RegExId = dummyRegex.Id, IsActive = true, TrackingState = TrackableEntities.TrackingState.Added };
                ctx.Lines.Add(line);
                await ctx.SaveChangesAsync().ConfigureAwait(false);
                _createdLineIds.Add(line.Id);
            }
            return line;
        }

        private async Task<Parts> GetOrCreateTestPartAsync(OCRContext ctx, string partTypeName = "Header", string partName = null)
        {
            partName = partName ?? $"OrchTestPart_{partTypeName}_{_testRunId.Substring(0, 4)}";
            var partType = await ctx.PartTypes.FirstOrDefaultAsync(pt => pt.Name == partTypeName).ConfigureAwait(false);
            if (partType == null)
            {
                partType = new PartTypes { Name = partTypeName, TrackingState = TrackableEntities.TrackingState.Added };
                ctx.PartTypes.Add(partType);
                await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
            var part = await ctx.Parts.FirstOrDefaultAsync(p => p.PartTypeId == partType.Id).ConfigureAwait(false); // Parts doesn't have Name property
            if (part == null)
            {
                var testInvoiceTemplate = await ctx.Templates.FirstOrDefaultAsync(i => i.Name.Contains("OrchTestTemplate")).ConfigureAwait(false) ?? await ctx.Templates.FirstOrDefaultAsync().ConfigureAwait(false);
                int invoiceIdToUse = testInvoiceTemplate?.Id ?? 0;
                if (invoiceIdToUse == 0)
                {
                    var tempOcrInv = new Templates { Name = $"OrchTestTemplate_{_testRunId}", TrackingState = TrackableEntities.TrackingState.Added };
                    ctx.Templates.Add(tempOcrInv);
                    await ctx.SaveChangesAsync().ConfigureAwait(false);
                    invoiceIdToUse = tempOcrInv.Id;
                }
                part = new Parts { PartTypes = partType, PartTypeId = partType.Id, Templates = testInvoiceTemplate, TrackingState = TrackableEntities.TrackingState.Added };
                ctx.Parts.Add(part);
                await ctx.SaveChangesAsync().ConfigureAwait(false);
            }
            return part;
        }
    }
}