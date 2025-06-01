using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using Serilog;
using WaterNut.DataSpace; // For OCRCorrectionService and its inner classes/models
using OCR.Business.Entities; // For OCRContext, Fields, Lines (for GetFieldsByRegexNamedGroupsAsync)
using System.Data.Entity; // For EF async operations
using TrackableEntities;
using NUnit.Framework.Legacy;
using static AutoBotUtilities.Tests.TestHelpers;

namespace AutoBotUtilities.Tests.Production
{
    using OCRCorrectionService = WaterNut.DataSpace.OCRCorrectionService;

    [TestFixture]
    [Category("FieldMapping")]
    public class OCRCorrectionService_FieldMappingTests
    {
        private ILogger _logger;
        private OCRCorrectionService _service;
        private string _testRunId;
        private List<int> _createdFieldIds = new List<int>();
        private List<int> _createdLineIds = new List<int>();
        private List<int> _createdRegexIds = new List<int>();

        [OneTimeSetUp]
        public void OneTimeSetUp()
        {
            _testRunId = $"FieldMapTest_{DateTime.Now:yyyyMMddHHmmss}";
            _logger = new LoggerConfiguration().MinimumLevel.Debug().WriteTo.Console().CreateLogger();
            _logger.Information("=== Starting Field Mapping Tests ===");
        }

        [SetUp]
        public void SetUp()
        {
            _service = new OCRCorrectionService(_logger);
            _logger.Information("Test Setup: {TestName}", TestContext.CurrentContext.Test.Name);
        }

        [OneTimeTearDown]
        public async Task OneTimeTearDown()
        {
            _logger.Information("--- Starting Field Mapping Test Data Cleanup ---");
            using (var ctx = new OCRContext())
            {
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
            _logger.Information("=== Completed Field Mapping Tests ===");
        }


        #region MapDeepSeekFieldToDatabase Tests
        [Test]
        public void MapDeepSeekFieldToDatabase_KnownFieldsAndAliases_ShouldMapCorrectly()
        {
            // Canonical Names
            var totalInfo = _service.MapDeepSeekFieldToDatabase("InvoiceTotal");
            Assert.That(totalInfo, Is.Not.Null);
            Assert.That(totalInfo.DatabaseFieldName, Is.EqualTo("InvoiceTotal"));
            Assert.That(totalInfo.EntityType, Is.EqualTo("ShipmentInvoice"));

            var qtyInfo = _service.MapDeepSeekFieldToDatabase("Quantity");
            Assert.That(qtyInfo, Is.Not.Null);
            Assert.That(qtyInfo.DatabaseFieldName, Is.EqualTo("Quantity"));
            Assert.That(qtyInfo.EntityType, Is.EqualTo("InvoiceDetails"));

            // Aliases
            var grandTotalInfo = _service.MapDeepSeekFieldToDatabase("GrandTotal");
            Assert.That(grandTotalInfo, Is.Not.Null);
            Assert.That(grandTotalInfo.DatabaseFieldName, Is.EqualTo("InvoiceTotal"));

            var shippingInfo = _service.MapDeepSeekFieldToDatabase("Shipping");
            Assert.That(shippingInfo, Is.Not.Null);
            Assert.That(shippingInfo.DatabaseFieldName, Is.EqualTo("TotalInternalFreight"));

            // Prefixed line item field
            var lineItemQtyInfo = _service.MapDeepSeekFieldToDatabase("InvoiceDetail_Line1_Quantity");
            Assert.That(lineItemQtyInfo, Is.Not.Null);
            Assert.That(lineItemQtyInfo.DatabaseFieldName, Is.EqualTo("Quantity"));
            Assert.That(lineItemQtyInfo.EntityType, Is.EqualTo("InvoiceDetails"));

            _logger.Information("? MapDeepSeekFieldToDatabase maps known fields and aliases.");
        }

        [Test]
        public void MapDeepSeekFieldToDatabase_UnknownField_ShouldReturnNull()
        {
            var unknownInfo = _service.MapDeepSeekFieldToDatabase("NonExistentField123");
            Assert.That(unknownInfo, Is.Null);
            _logger.Information("? MapDeepSeekFieldToDatabase returns null for unknown field.");
        }
        #endregion

        #region IsFieldSupported and GetFieldValidationInfo Tests
        [Test]
        public void IsFieldSupported_ShouldReturnCorrectStatus()
        {
            Assert.That(_service.IsFieldSupported("InvoiceTotal"), Is.True);
            Assert.That(_service.IsFieldSupported("InvoiceDetail_Line5_Cost"), Is.True); // Prefixed
            Assert.That(_service.IsFieldSupported("UnsupportedGarbageField"), Is.False);
            _logger.Information("? IsFieldSupported works correctly.");
        }

        [Test]
        public void GetFieldValidationInfo_KnownField_ShouldReturnValidationRules()
        {
            var info = _service.GetFieldValidationInfo("InvoiceTotal");
            Assert.That(info.IsValid, Is.True);
            Assert.That(info.DatabaseFieldName, Is.EqualTo("InvoiceTotal"));
            Assert.That(info.EntityType, Is.EqualTo("ShipmentInvoice"));
            Assert.That(info.IsRequired, Is.True);
            Assert.That(info.DataType, Is.EqualTo("decimal"));
            Assert.That(info.IsMonetary, Is.True);
            StringAssert.IsMatch(@"^-?\$?�?�?\s*(?:\d{1,3}(?:[,.]\d{3})*|\d+)(?:[.,]\d{1,4})?$", info.ValidationPattern);

            var descInfo = _service.GetFieldValidationInfo("ItemDescription");
            Assert.That(descInfo.IsValid, Is.True);
            Assert.That(descInfo.DatabaseFieldName, Is.EqualTo("ItemDescription"));
            Assert.That(descInfo.DataType, Is.EqualTo("string"));
            Assert.That(descInfo.MaxLength, Is.EqualTo(1000)); // Example from helper

            _logger.Information("? GetFieldValidationInfo returns correct rules for known field.");
        }

        [Test]
        public void GetFieldValidationInfo_UnknownField_ShouldReturnInvalid()
        {
            var info = _service.GetFieldValidationInfo("MysteryField");
            Assert.That(info.IsValid, Is.False);
            StringAssert.Contains("unknown or not mapped", info.ErrorMessage);
            _logger.Information("? GetFieldValidationInfo handles unknown field.");
        }
        #endregion

        #region GetFieldsByRegexNamedGroupsAsync Tests
        // Helper to create a dummy Line with a Regex and Fields for testing
        private async Task<(Lines line, List<Fields> fields)> SetupTestLineWithFieldsAsync(OCRContext ctx, string regexText, Dictionary<string, string> fieldKeyToDbNameMap)
        {
            var testRegex = new RegularExpressions { RegEx = regexText, Description = $"TestRegex_{_testRunId}", TrackingState = TrackingState.Added };
            ctx.RegularExpressions.Add(testRegex);
            await ctx.SaveChangesAsync().ConfigureAwait(false);
            _createdRegexIds.Add(testRegex.Id);

            // Assume PartId 1 exists or create it. For simplicity, using a fixed PartId.
            var testLine = new Lines { Name = $"TestLine_FG_{_testRunId}", PartId = 1, RegExId = testRegex.Id, IsActive = true, TrackingState = TrackingState.Added };
            ctx.Lines.Add(testLine);
            await ctx.SaveChangesAsync().ConfigureAwait(false);
            _createdLineIds.Add(testLine.Id);

            var createdFields = new List<Fields>();
            foreach (var kvp in fieldKeyToDbNameMap)
            {
                var field = new Fields
                {
                    LineId = testLine.Id,
                    Key = kvp.Key,
                    Field = kvp.Value,
                    EntityType = "TestEntity",
                    DataType = "string",
                    TrackingState = TrackingState.Added
                };
                ctx.Fields.Add(field);
                createdFields.Add(field);
            }
            await ctx.SaveChangesAsync().ConfigureAwait(false);
            _createdFieldIds.AddRange(createdFields.Select(f => f.Id));
            return (testLine, createdFields);
        }


        [Test]
        public async Task GetFieldsByRegexNamedGroupsAsync_ShouldReturnMatchingFields()
        {
            string regexPattern = @"Date:\s*(?<InvoiceDate>\d{2}/\d{2}/\d{4})\s*Inv#:\s*(?<InvoiceNo>[A-Z0-9\-]+)";
            var fieldMappings = new Dictionary<string, string> {
                { "InvoiceDate", "InvoiceDate" }, // Key matches DB Field Name
                { "InvoiceNo", "InvoiceNumberMapped" }  // Key different from DB Field Name
            };

            Lines testLine;
            using (var ctx = new OCRContext())
            {
                (testLine, _) = await this.SetupTestLineWithFieldsAsync(ctx, regexPattern, fieldMappings).ConfigureAwait(false);
            }

            // Act
            var resultFields = await InvokePrivateMethod<Task<List<Fields>>>(_service, "GetFieldsByRegexNamedGroupsAsync", regexPattern, testLine.Id);

            // Assert
            Assert.That(resultFields.Count, Is.EqualTo(2));
            Assert.That(resultFields.Any(f => f.Key == "InvoiceDate" && f.Field == "InvoiceDate"));
            Assert.That(resultFields.Any(f => f.Key == "InvoiceNo" && f.Field == "InvoiceNumberMapped"));
            _logger.Information("? GetFieldsByRegexNamedGroupsAsync returned correct fields based on regex groups and Keys.");
        }
        #endregion

        #region IsFieldExistingInLineContext Tests
        [Test]
        public void IsFieldExistingInLineContext_FieldExistsByKey_ShouldReturnTrue()
        {
            var lineContext = new LineContext
            {
                RegexPattern = @"Total:\s*(?<TotalAmount>\d+\.\d{2})", // FieldInfo should be populated from this
                FieldsInLine = new List<FieldInfo> { new FieldInfo { Key = "TotalAmount", Field = "InvoiceTotal" } }
            };
            // DeepSeek might return "TotalAmount" (matching key) or "InvoiceTotal" (matching mapped field)
            Assert.That(InvokePrivateMethod<bool>(_service, "IsFieldExistingInLineContext", "TotalAmount", lineContext), Is.True);
            _logger.Information("? IsFieldExistingInLineContext found field by Key.");
        }

        [Test]
        public void IsFieldExistingInLineContext_FieldExistsByMappedName_ShouldReturnTrue()
        {
            var lineContext = new LineContext
            {
                RegexPattern = @"Total:\s*(?<TotalKey>\d+\.\d{2})",
                FieldsInLine = new List<FieldInfo> { new FieldInfo { Key = "TotalKey", Field = "InvoiceTotal" } }
            };
            // DeepSeek might return "InvoiceTotal", which maps to "InvoiceTotal", matching FieldInfo.Field
            Assert.That(InvokePrivateMethod<bool>(_service, "IsFieldExistingInLineContext", "InvoiceTotal", lineContext), Is.True);
            _logger.Information("? IsFieldExistingInLineContext found field by mapped DB field name.");
        }

        [Test]
        public void IsFieldExistingInLineContext_FieldDoesNotExist_ShouldReturnFalse()
        {
            var lineContext = new LineContext
            {
                RegexPattern = @"SubTotal:\s*(?<SubTotalVal>\d+\.\d{2})",
                FieldsInLine = new List<FieldInfo> { new FieldInfo { Key = "SubTotalVal", Field = "SubTotal" } }
            };
            Assert.That(InvokePrivateMethod<bool>(_service, "IsFieldExistingInLineContext", "TaxAmount", lineContext), Is.False); // TaxAmount not in regex/fields
            _logger.Information("? IsFieldExistingInLineContext correctly reports non-existent field.");
        }
        #endregion
    }
}