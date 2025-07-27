using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Serilog;
using EntryDataDS.Business.Entities; // For ShipmentInvoice if needed
using OCR.Business.Entities; // For OCRContext, Fields, RegularExpressions, FieldFormatRegEx etc.
using WaterNut.DataSpace; // For OCRCorrectionService and its inner classes
using System.Data.Entity; // For Include, FirstOrDefaultAsync
using TrackableEntities; // For TrackingState, if entities are manually set to Added

namespace AutoBotUtilities.Tests.Production
{
    using NUnit.Framework.Legacy;

    using OCRCorrectionService = WaterNut.DataSpace.OCRCorrectionService;

    /// <summary>
    /// Tests for OCR correction service database update strategies.
    /// </summary>
    [TestFixture]
    [Category("Database")]
    [Category("Strategies")]
    [Category("OCRCorrection")]
    public class OCRCorrectionService_DatabaseUpdateStrategyTests
    {
        #region Test Setup and Configuration

        private static ILogger _logger;
        private OCRCorrectionService _service; // Instance of the main service
        private OCRCorrectionService.DatabaseUpdateStrategyFactory _strategyFactory;

        // For tracking created DB entities to clean up
        private List<int> _createdRegexIds;
        private List<int> _createdFieldFormatIds;
        private List<int> _createdFieldIds;
        private List<int> _createdLineIds;
        private string _testRunId;


        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _testRunId = $"DBStrategyTest_{DateTime.Now:yyyyMMddHHmmss}";
            _logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .WriteTo.File($"logs/OCRDatabaseStrategyTests_{_testRunId}.log")
                .CreateLogger();

            _logger.Information("=== Starting OCR Database Strategy Tests ===");
            _createdRegexIds = new List<int>();
            _createdFieldFormatIds = new List<int>();
            _createdFieldIds = new List<int>();
            _createdLineIds = new List<int>();
        }

        [SetUp]
        public void SetUp()
        {
            _service = new OCRCorrectionService(_logger);
            _strategyFactory = new OCRCorrectionService.DatabaseUpdateStrategyFactory(_logger);
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
            _logger.Information("--- Starting Test Data Cleanup ---");
            await this.CleanupTestDataAsync().ConfigureAwait(false);
            _logger.Information("=== Completed OCR Database Strategy Tests ===");
        }

        private async Task CleanupTestDataAsync()
        {
            // Order of deletion: FieldFormatRegEx -> Fields (if created fields are specific to lines being deleted) -> Lines -> RegularExpressions
            // This order helps avoid foreign key constraint violations.
            try
            {
                using (var ctx = new OCRContext())
                {
                    if (_createdFieldFormatIds.Any())
                    {
                        var items = await ctx.OCR_FieldFormatRegEx.Where(x => this._createdFieldFormatIds.Contains(x.Id)).ToListAsync().ConfigureAwait(false);
                        if (items.Any()) { ctx.OCR_FieldFormatRegEx.RemoveRange(items); _logger.Debug("Marked {Count} OCR_FieldFormatRegEx entries for deletion.", items.Count); }
                    }
                    if (_createdFieldIds.Any())
                    {
                        var items = await ctx.Fields.Where(x => this._createdFieldIds.Contains(x.Id)).ToListAsync().ConfigureAwait(false);
                        if (items.Any()) { ctx.Fields.RemoveRange(items); _logger.Debug("Marked {Count} Fields entries for deletion.", items.Count); }
                    }
                    if (_createdLineIds.Any())
                    {
                        var items = await ctx.Lines.Where(x => this._createdLineIds.Contains(x.Id)).ToListAsync().ConfigureAwait(false);
                        if (items.Any()) { ctx.Lines.RemoveRange(items); _logger.Debug("Marked {Count} Lines entries for deletion.", items.Count); }
                    }
                    if (_createdRegexIds.Any())
                    {
                        // Fetch regexes not associated with still-existing lines or field formats that weren't cleaned up
                        var regexesInUseByFieldFormat = await ctx.OCR_FieldFormatRegEx
                                                            .Where(ffr => !this._createdFieldFormatIds.Contains(ffr.Id) && (( this._createdRegexIds.Contains(ffr.RegEx.Id)) || (this._createdRegexIds.Contains(ffr.ReplacementRegEx.Id))))
                                                            .SelectMany(ffr => new[] { ffr.RegExId, ffr.ReplacementRegExId })
                                                            .Select(id => id)
                                                            .Distinct()
                                                            .ToListAsync().ConfigureAwait(false);

                        var regexesInUseByLines = await ctx.Lines
                                                      .Where(l => !this._createdLineIds.Contains(l.Id) && this._createdRegexIds.Contains(l.RegExId))
                                                      .Select(l => l.RegExId)
                                                      .Distinct()
                                                      .ToListAsync().ConfigureAwait(false);

                        var regexesToDelete = _createdRegexIds.Except(regexesInUseByFieldFormat).Except(regexesInUseByLines).ToList();

                        if (regexesToDelete.Any())
                        {
                            var items = await ctx.RegularExpressions.Where(x => regexesToDelete.Contains(x.Id)).ToListAsync().ConfigureAwait(false);
                            if (items.Any()) { ctx.RegularExpressions.RemoveRange(items); _logger.Debug("Marked {Count} RegularExpressions entries for deletion.", items.Count); }
                        }
                        else
                        {
                            _logger.Debug("No RegularExpressions marked for deletion as they might be in use or already handled.");
                        }
                    }
                    await ctx.SaveChangesAsync().ConfigureAwait(false);
                    _logger.Information("Test data cleanup SaveChanges executed.");
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "Error during test data cleanup.");
                // Log and continue, don't let cleanup failure break tests.
            }
            _createdRegexIds.Clear();
            _createdFieldFormatIds.Clear();
            _createdFieldIds.Clear();
            _createdLineIds.Clear();
        }


        // Helper to create a dummy Field definition for tests
        private async Task<Fields> GetOrCreateTestFieldAsync(OCRContext ctx, string fieldName, int lineId, string entityType = "ShipmentInvoice", string dataType = "string")
        {
            var field = await ctx.Fields.FirstOrDefaultAsync(f => f.LineId == lineId && f.Field == fieldName && f.Key == fieldName).ConfigureAwait(false);
            if (field == null)
            {
                field = new Fields
                {
                    LineId = lineId,
                    Field = fieldName, // Database column name
                    Key = fieldName,   // Regex capture group name
                    EntityType = entityType,
                    DataType = dataType,
                    IsRequired = false,
                    AppendValues = false,
                    TrackingState = TrackingState.Added // Important for EF context
                };
                ctx.Fields.Add(field);
                await ctx.SaveChangesAsync().ConfigureAwait(false); // Save to get ID
                _createdFieldIds.Add(field.Id);
                _logger.Debug("Created test Field: ID={FieldId}, Name={FieldName}, LineId={TestLineId}", field.Id, fieldName, lineId);
            }
            return field;
        }

        // Helper to create a dummy Line definition for tests
        private async Task<Lines> GetOrCreateTestLineAsync(OCRContext ctx, int partId, string lineName = null, string initialRegex = ".*")
        {
            lineName = lineName ?? $"TestLine_{_testRunId}_{Guid.NewGuid().ToString("N").Substring(0, 8)}";
            var line = await ctx.Lines.FirstOrDefaultAsync(l => l.Name == lineName && l.PartId == partId).ConfigureAwait(false);
            if (line == null)
            {
                var dummyRegex = new RegularExpressions {
                    RegEx = initialRegex,
                    Description = $"Dummy for {lineName}",
                    CreatedDate = DateTime.Now,
                    LastUpdated = DateTime.Now,
                    TrackingState = TrackingState.Added
                };
                ctx.RegularExpressions.Add(dummyRegex);
                await ctx.SaveChangesAsync().ConfigureAwait(false);
                _createdRegexIds.Add(dummyRegex.Id);

                line = new Lines
                {
                    Name = lineName,
                    PartId = partId,
                    RegExId = dummyRegex.Id,
                    IsActive = true,
                    TrackingState = TrackingState.Added
                };
                ctx.Lines.Add(line);
                await ctx.SaveChangesAsync().ConfigureAwait(false);
                _createdLineIds.Add(line.Id);
                _logger.Debug("Created test Line: ID={LineId}, Name={LineName}, RegexID={RegexId}", line.Id, line.Name, line.RegExId);
            }
            return line;
        }

        // Helper to ensure a test Part exists (e.g., Header)
        private async Task<Parts> GetOrCreateTestPartAsync(OCRContext ctx, string partTypeName = "Header", string partName = null)
        {
            partName = partName ?? $"TestPart_{partTypeName}_{_testRunId}";
            var partType = await ctx.PartTypes.FirstOrDefaultAsync(pt => pt.Name == partTypeName).ConfigureAwait(false);
            if (partType == null)
            {
                partType = new PartTypes { Name = partTypeName, TrackingState = TrackingState.Added };
                ctx.PartTypes.Add(partType);
                await ctx.SaveChangesAsync().ConfigureAwait(false); // Save to get ID
                _logger.Debug("Created test PartType: ID={PartTypeId}, Name={PartTypeName}", partType.Id, partTypeName);
            }

            var part = await ctx.Parts.FirstOrDefaultAsync(p => p.PartTypes.Name == partName && p.PartTypeId == partType.Id).ConfigureAwait(false);
            if (part == null)
            {
                // Assuming OcrInvoicesId 1 exists or is a valid test Invoice template.
                // For robust tests, you might need to create a dummy OcrInvoices entry as well.
                var testInvoiceTemplate = await ctx.Templates.FirstOrDefaultAsync().ConfigureAwait(false);
                int invoiceIdToUse = testInvoiceTemplate?.Id ?? 0;
                if (invoiceIdToUse == 0)
                {
                    var tempOcrInv = new Templates { Name = $"TestTempInvoice_{_testRunId}", TrackingState = TrackingState.Added };
                    ctx.Templates.Add(tempOcrInv);
                    await ctx.SaveChangesAsync().ConfigureAwait(false);
                    invoiceIdToUse = tempOcrInv.Id;
                    _logger.Debug("Created temporary OcrInvoices entry (ID: {TempInvoiceId}) for part creation.", invoiceIdToUse);
                }


                part = new Parts { PartTypes = partType, Templates = testInvoiceTemplate, TrackingState = TrackingState.Added };
                ctx.Parts.Add(part);
                await ctx.SaveChangesAsync().ConfigureAwait(false); // Save to get ID
                                              // Do not add to a cleanup list as Parts are usually fundamental.
                _logger.Debug("Created/Ensured test Part: ID={PartId}, Name={PartName}, Type={PartTypeName}", part.Id, part.PartTypes.Name, partTypeName);
            }
            return part;
        }


        #endregion

        #region FieldFormatUpdateStrategy Tests (Largely same as before)

        [Test]
        [Category("FieldFormatStrategy")]
        public async Task FieldFormatUpdateStrategy_DecimalCommaToPoint_ShouldCreateDbEntries()
        {
            // Arrange
            var correctionType = "FieldFormat";
            var fieldName = "InvoiceTotal";

            using (var ctx = new OCRContext())
            {
                var testPart = await this.GetOrCreateTestPartAsync(ctx).ConfigureAwait(false);
                var testLine = await this.GetOrCreateTestLineAsync(ctx, testPart.Id).ConfigureAwait(false);
                var testField = await this.GetOrCreateTestFieldAsync(ctx, fieldName, testLine.Id, dataType: "decimal").ConfigureAwait(false);

                var request = new RegexUpdateRequest
                {
                    FieldName = fieldName,
                    OldValue = "123,45",
                    NewValue = "123.45",
                    CorrectionType = correctionType,
                    LineId = testField.Id // IMPORTANT: For FieldFormatStrategy, LineId in request is Fields.Id
                };

                OCRCorrectionService.IDatabaseUpdateStrategy strategy = _strategyFactory.GetStrategy(request);
                Assert.That(strategy, Is.InstanceOf<OCRCorrectionService.FieldFormatUpdateStrategy>(), "Strategy should be FieldFormatUpdateStrategy");

                // Act
                var dbResult = await strategy.ExecuteAsync(ctx, request, this._service).ConfigureAwait(false);

                // Assert
                Assert.That(dbResult.IsSuccess, Is.True, $"Database update failed: {dbResult.Message}");
                Assert.That(dbResult.RecordId, Is.Not.Null, "RecordId should be populated for successful FieldFormatRegEx creation.");
                _createdFieldFormatIds.Add(dbResult.RecordId.Value);

                var fieldFormatEntry = await ctx.OCR_FieldFormatRegEx
                                           .Include(ffr => ffr.RegEx)
                                           .Include(ffr => ffr.ReplacementRegEx)
                                           .FirstOrDefaultAsync(ffr => ffr.Id == dbResult.RecordId.Value).ConfigureAwait(false);

                Assert.That(fieldFormatEntry, Is.Not.Null, "FieldFormatRegEx entry not found in DB.");
                Assert.That(fieldFormatEntry.FieldId, Is.EqualTo(testField.Id));
                Assert.That(fieldFormatEntry.RegEx.RegEx, Is.EqualTo(@"(\d+),(\d{1,4})"));
                Assert.That(fieldFormatEntry.ReplacementRegEx.RegEx, Is.EqualTo("$1.$2"));
                _createdRegexIds.Add(fieldFormatEntry.RegEx.Id);
                _createdRegexIds.Add(fieldFormatEntry.ReplacementRegEx.Id);

                _logger.Information("✓ FieldFormatUpdateStrategy_DecimalCommaToPoint test passed.");
            }
        }
        // ... other FieldFormatUpdateStrategy tests remain similar to previous version ...
        // Make sure GetOrCreateTestFieldAsync and GetOrCreateTestLineAsync are used to setup prerequisites.

        [Test]
        [Category("FieldFormatStrategy")]
        public async Task FieldFormatUpdateStrategy_AddDollarSign_ShouldCreateDbEntries()
        {
            var correctionType = "FieldFormat";
            var fieldName = "SubTotal";

            using (var ctx = new OCRContext())
            {
                var testPart = await this.GetOrCreateTestPartAsync(ctx).ConfigureAwait(false);
                var testLine = await this.GetOrCreateTestLineAsync(ctx, testPart.Id).ConfigureAwait(false);
                var testField = await this.GetOrCreateTestFieldAsync(ctx, fieldName, testLine.Id, dataType: "decimal").ConfigureAwait(false);

                var request = new RegexUpdateRequest
                                  {
                                      FieldName = fieldName,
                                      OldValue = "99.99",
                                      NewValue = "$99.99",
                                      CorrectionType = correctionType,
                                      LineId = testField.Id
                                  };

                OCRCorrectionService.IDatabaseUpdateStrategy strategy = _strategyFactory.GetStrategy(request);
                var dbResult = await strategy.ExecuteAsync(ctx, request, this._service).ConfigureAwait(false);

                Assert.That(dbResult.IsSuccess, Is.True, dbResult.Message);
                _createdFieldFormatIds.Add(dbResult.RecordId.Value);
                var entry = await ctx.OCR_FieldFormatRegEx.Include(f => f.RegEx).Include(f => f.ReplacementRegEx).FirstAsync(f => f.Id == dbResult.RecordId.Value).ConfigureAwait(false);
                Assert.That(entry.RegEx.RegEx, Is.EqualTo(@"^(-?\d+(\.\d+)?)$"));
                StringAssert.AreEqualIgnoringCase(@"\$$1", entry.ReplacementRegEx.RegEx);
                _createdRegexIds.Add(entry.RegEx.Id);
                _createdRegexIds.Add(entry.ReplacementRegEx.Id);
            }
        }
        #endregion

        #region OmissionUpdateStrategy Tests (Making Real DeepSeek Calls)

        [Test]
        [Category("OmissionStrategy")]
        [Category("LiveAPI")] // Mark as live API test
        public async Task OmissionUpdateStrategy_CreateNewLine_WithLiveDeepSeek_ShouldCreateDbEntries()
        {
            // Arrange - Use a known field name that exists in the field mapping
            var fieldNameToOmit = "TotalDeduction"; // Use a known field instead of random field name

            using (var ctx = new OCRContext())
            {
                var testPart = await this.GetOrCreateTestPartAsync(ctx, "Header").ConfigureAwait(false); // Ensure a Header part exists

                var request = new RegexUpdateRequest
                {
                    FieldName = fieldNameToOmit,
                    NewValue = "25.00",
                    OldValue = "", // Key for omission
                    CorrectionType = "omission",
                    Confidence = 0.95,
                    LineNumber = 10,
                    LineText = $"Total Deduction: 25.00 USD", // Line where value is found
                    ContextLinesBefore = new List<string> { "Context Before 1", "Context Before 2" },
                    ContextLinesAfter = new List<string> { "Context After 1", "Context After 2" },
                    RequiresMultilineRegex = false,
                    PartId = testPart.Id // Explicitly provide PartId for new line
                };

                OCRCorrectionService.IDatabaseUpdateStrategy strategy = _strategyFactory.GetStrategy(request);
                Assert.That(strategy, Is.InstanceOf<OCRCorrectionService.OmissionUpdateStrategy>(), "Strategy should be OmissionUpdateStrategy");

                _logger.Information("Attempting OmissionUpdateStrategy with LIVE DeepSeek call for field: {FieldName}", request.FieldName);

                // Act
                DatabaseUpdateResult dbResult = null;
                try
                {
                    dbResult = await strategy.ExecuteAsync(ctx, request, this._service).ConfigureAwait(false);
                }
                catch (Exception apiEx)
                {
                    _logger.Error(apiEx, "DeepSeek API call or subsequent processing failed during test.");
                    Assert.Fail($"Live DeepSeek call or processing failed: {apiEx.Message}");
                }

                // Assert
                Assert.That(dbResult, Is.Not.Null, "DBResult should not be null.");
                _logger.Information("Omission Strategy Result: Success={IsSuccess}, Message='{Message}', RecordId={RecordId}",
                                    dbResult.IsSuccess, dbResult.Message, dbResult.RecordId);

                if (!dbResult.IsSuccess)
                {
                    _logger.Warning("OmissionUpdateStrategy failed. Message: {FailureMessage}", dbResult.Message);
                }
                Assert.That(dbResult.IsSuccess, Is.True, $"Omission DB update failed: {dbResult.Message}");

                Assert.That(dbResult.RecordId, Is.Not.Null, "RecordId (new Field.Id) should be populated on success.");
                _createdFieldIds.Add(dbResult.RecordId.Value);

                var newField = await ctx.Fields.Include(f => f.Lines.RegularExpressions)
                                   .FirstOrDefaultAsync(f => f.Id == dbResult.RecordId.Value).ConfigureAwait(false);
                Assert.That(newField, Is.Not.Null, "Newly created Field not found in DB.");
                Assert.That(newField.Key, Is.EqualTo(fieldNameToOmit));
                Assert.That(newField.Lines, Is.Not.Null, "New field should be associated with a Line.");
                _createdLineIds.Add(newField.LineId);

                Assert.That(newField.Lines.RegularExpressions, Is.Not.Null, "New Line should have an associated Regex.");
                _createdRegexIds.Add(newField.Lines.RegularExpressions.Id);

                StringAssert.Contains($"(?<{fieldNameToOmit}>", newField.Lines.RegularExpressions.RegEx, "Regex should contain the named capture group.");

                var regex = new System.Text.RegularExpressions.Regex(newField.Lines.RegularExpressions.RegEx);
                var match = regex.Match(request.LineText);
                Assert.That(match.Success, Is.True, $"Generated regex '{newField.Lines.RegularExpressions.RegEx}' did not match line text '{request.LineText}'");
                Assert.That(match.Groups[fieldNameToOmit].Value, Is.EqualTo(request.NewValue), "Captured value does not match expected NewValue.");

                _logger.Information("✓ OmissionUpdateStrategy_CreateNewLine_WithLiveDeepSeek test passed for field {FieldName}. New Field ID: {FieldId}, New Line ID: {LineId}, New Regex ID: {RegexId}. Regex: '{Regex}'",
                                    fieldNameToOmit, newField.Id, newField.LineId, newField.Lines.RegularExpressions.Id, newField.Lines.RegularExpressions.RegEx);
            }
        }
        // Add a similar test for "modify_existing_line" strategy if you want to test that path.
        // It would involve setting up an existing Line with a simple regex, and ensuring DeepSeek
        // modifies it correctly. This is more complex to make deterministic with live API calls.

        #endregion
    }
}