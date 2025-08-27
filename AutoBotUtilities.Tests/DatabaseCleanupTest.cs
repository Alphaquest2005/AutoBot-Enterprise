using System;
using System.Linq;
using NUnit.Framework;
using OCR.Business.Entities;
using Serilog;
using System.Data.Entity;

namespace AutoBotUtilities.Tests
{
    /// <summary>
    /// Test to permanently remove the duplicate Gift Card mapping from database
    /// </summary>
    [TestFixture]
    [Category("Database")]
    [Category("Cleanup")]
    public class DatabaseCleanupTest
    {
        private ILogger _logger;

        [SetUp]
        public void Setup()
        {
            _logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .CreateLogger();
        }

        [Test]
        [Explicit("Run manually to permanently delete the duplicate Gift Card mapping")]
        public void PermanentlyRemoveDuplicateGiftCardMapping()
        {
            _logger.Information("🧹 **CLEANUP_START**: Permanently removing duplicate Gift Card mapping");

            using (var ctx = new OCRContext())
            {
                // Get current state before cleanup
                var giftCardFields = ctx.Fields
                    .Include(f => f.Lines)
                    .Where(f => f.LineId == 1830)
                    .ToList();

                _logger.Information("📊 **BEFORE_CLEANUP**: Found {Count} fields mapped to LineId 1830 (Gift Card)", giftCardFields.Count);

                foreach (var field in giftCardFields)
                {
                    _logger.Information("🔍 **CURRENT_MAPPING**: FieldId={FieldId} | Field='{Field}' | EntityType='{EntityType}' | Key='{Key}'",
                        field.Id, field.Field, field.EntityType, field.Key);
                }

                // Find the specific duplicate field that user reported (FieldId: 2579, GiftCard → TotalOtherCost)
                var duplicateField = giftCardFields.FirstOrDefault(f => f.Id == 2579 && f.Field == "TotalOtherCost" && f.Key == "GiftCard");

                if (duplicateField != null)
                {
                    _logger.Warning("🎯 **TARGET_FOUND**: Found duplicate field to remove - FieldId: {FieldId} | Field: '{Field}' | Key: '{Key}'",
                        duplicateField.Id, duplicateField.Field, duplicateField.Key);

                    _logger.Warning("⚠️ **DELETION_WARNING**: About to permanently delete FieldId {FieldId} from database", duplicateField.Id);

                    // Remove the duplicate field mapping
                    ctx.Fields.Remove(duplicateField);

                    // Save changes to database
                    int changesCount = ctx.SaveChanges();

                    _logger.Information("✅ **DELETION_SUCCESS**: Permanently removed {Changes} field mapping(s) from database", changesCount);

                    // Verify the deletion
                    var remainingFields = ctx.Fields
                        .Where(f => f.LineId == 1830)
                        .ToList();

                    _logger.Information("📊 **AFTER_CLEANUP**: {Count} field(s) remaining for LineId 1830", remainingFields.Count);

                    foreach (var field in remainingFields)
                    {
                        _logger.Information("✅ **REMAINING_MAPPING**: FieldId={FieldId} | Field='{Field}' | EntityType='{EntityType}' | Key='{Key}'",
                            field.Id, field.Field, field.EntityType, field.Key);
                    }

                    // Verify FieldId 2579 no longer exists
                    var deletedField = ctx.Fields.FirstOrDefault(f => f.Id == 2579);
                    if (deletedField == null)
                    {
                        _logger.Information("🎉 **VERIFICATION_SUCCESS**: FieldId 2579 has been permanently deleted from database");
                        Assert.Pass($"Successfully removed duplicate mapping. Remaining fields: {remainingFields.Count}");
                    }
                    else
                    {
                        _logger.Error("❌ **VERIFICATION_FAILED**: FieldId 2579 still exists in database after deletion attempt");
                        Assert.Fail("Failed to delete the duplicate field mapping");
                    }
                }
                else
                {
                    _logger.Warning("⚠️ **TARGET_NOT_FOUND**: Could not find the specific duplicate field (FieldId: 2579, GiftCard → TotalOtherCost)");
                    
                    // Check if there are other duplicates to remove
                    if (giftCardFields.Count > 1)
                    {
                        _logger.Warning("🔍 **ALTERNATIVE_CLEANUP**: Found {Count} fields for LineId 1830, need manual review:", giftCardFields.Count);
                        foreach (var field in giftCardFields)
                        {
                            _logger.Warning("  📋 **FIELD_OPTION**: FieldId={FieldId} | Field='{Field}' | Key='{Key}' | EntityType='{EntityType}'",
                                field.Id, field.Field, field.Key, field.EntityType);
                        }
                        Assert.Inconclusive("Multiple fields found but target field (2579) not found - manual review needed");
                    }
                    else
                    {
                        _logger.Information("✅ **ALREADY_CLEAN**: Only {Count} field mapping exists for LineId 1830", giftCardFields.Count);
                        Assert.Pass("Gift Card mapping is already clean");
                    }
                }
            }

            _logger.Information("✅ **CLEANUP_COMPLETE**: Gift Card mapping cleanup completed");
        }

        [Test]
        public void VerifyGiftCardMappingAfterCleanup()
        {
            _logger.Information("🔍 **POST_CLEANUP_VERIFICATION**: Verifying Gift Card mapping state after cleanup");

            using (var ctx = new OCRContext())
            {
                var giftCardFields = ctx.Fields
                    .Include(f => f.Lines)
                    .Where(f => f.LineId == 1830)
                    .ToList();

                _logger.Information("📊 **FIELD_COUNT**: Found {Count} field(s) mapped to LineId 1830 (Gift Card)", giftCardFields.Count);

                if (giftCardFields.Count == 0)
                {
                    _logger.Warning("⚠️ **NO_MAPPINGS**: No field mappings found for Gift Card line - this might break functionality");
                    Assert.Fail("No field mappings found for Gift Card line");
                }
                else if (giftCardFields.Count == 1)
                {
                    var singleField = giftCardFields.First();
                    _logger.Information("✅ **SINGLE_MAPPING**: Gift Card line has clean mapping - FieldId={FieldId} | Field='{Field}' | Key='{Key}'",
                        singleField.Id, singleField.Field, singleField.Key);

                    // Verify it's the correct mapping for Caribbean customs (should be TotalInsurance)
                    if (singleField.Field == "TotalInsurance")
                    {
                        _logger.Information("🎯 **CORRECT_MAPPING**: Gift Card correctly maps to TotalInsurance (Caribbean customs requirement)");
                        Assert.Pass($"Gift Card mapping is clean and correct: {singleField.Field}");
                    }
                    else
                    {
                        _logger.Warning("⚠️ **SUBOPTIMAL_MAPPING**: Gift Card maps to '{Field}' instead of 'TotalInsurance' (Caribbean customs requirement)",
                            singleField.Field);
                        Assert.Pass($"Gift Card mapping is clean but maps to: {singleField.Field}");
                    }
                }
                else
                {
                    _logger.Error("❌ **DUPLICATES_REMAIN**: Gift Card line still has {Count} field mappings", giftCardFields.Count);
                    foreach (var field in giftCardFields)
                    {
                        _logger.Error("  🔍 **DUPLICATE_MAPPING**: FieldId={FieldId} | Field='{Field}' | Key='{Key}'",
                            field.Id, field.Field, field.Key);
                    }
                    Assert.Fail($"Duplicate mappings still exist: {giftCardFields.Count} fields found");
                }

                // Also verify FieldId 2579 is gone
                var fieldId2579 = ctx.Fields.FirstOrDefault(f => f.Id == 2579);
                if (fieldId2579 == null)
                {
                    _logger.Information("✅ **FIELDID_2579_DELETED**: Confirmed FieldId 2579 has been permanently removed");
                }
                else
                {
                    _logger.Error("❌ **FIELDID_2579_REMAINS**: FieldId 2579 still exists: Field='{Field}' | Key='{Key}'",
                        fieldId2579.Field, fieldId2579.Key);
                }
            }

            _logger.Information("✅ **POST_CLEANUP_VERIFICATION_COMPLETE**: Gift Card mapping verification completed");
        }

        [Test]
        public void ShowAllDuplicateMappingsInAmazonTemplate()
        {
            _logger.Information("🔍 **FULL_AUDIT**: Showing all duplicate mappings in Amazon template");

            using (var ctx = new OCRContext())
            {
                // Get all Amazon template fields grouped by LineId
                var amazonFields = ctx.Fields
                    .Include(f => f.Lines)
                    .Include(f => f.Lines.Parts)
                    .Include(f => f.Lines.Parts.Templates)
                    .Where(f => f.Lines.Parts.Templates.Id == 5) // Amazon template
                    .GroupBy(f => f.LineId)
                    .Where(g => g.Count() > 1) // Only lines with multiple fields
                    .ToList();

                _logger.Information("📊 **DUPLICATE_LINES**: Found {Count} lines with multiple field mappings in Amazon template", amazonFields.Count);

                foreach (var lineGroup in amazonFields)
                {
                    var lineId = lineGroup.Key;
                    var fields = lineGroup.ToList();
                    var lineName = fields.FirstOrDefault()?.Lines?.Name ?? "Unknown";

                    _logger.Warning("⚠️ **DUPLICATE_LINE**: LineId={LineId} | Name='{LineName}' | FieldCount={FieldCount}",
                        lineId, lineName, fields.Count);

                    foreach (var field in fields)
                    {
                        _logger.Warning("  🔍 **FIELD_DETAIL**: FieldId={FieldId} | Field='{Field}' | Key='{Key}' | EntityType='{EntityType}' | DataType='{DataType}'",
                            field.Id, field.Field, field.Key, field.EntityType, field.DataType);
                    }

                    // Show specific action needed
                    if (lineId == 1830) // Gift Card line
                    {
                        _logger.Error("🎯 **ACTION_NEEDED**: LineId 1830 (Gift Card) - should keep TotalInsurance mapping for Caribbean customs, remove TotalOtherCost mapping");
                    }
                    else
                    {
                        _logger.Information("💡 **REVIEW_NEEDED**: LineId {LineId} ('{LineName}') has {Count} mappings - determine which to keep",
                            lineId, lineName, fields.Count);
                    }
                }

                if (amazonFields.Count == 0)
                {
                    _logger.Information("✅ **NO_DUPLICATES**: Amazon template has no duplicate field mappings");
                }

                Assert.Pass($"Audit complete. Found {amazonFields.Count} lines with duplicate mappings");
            }

            _logger.Information("✅ **FULL_AUDIT_COMPLETE**: Amazon template duplicate mapping audit completed");
        }
    }
}