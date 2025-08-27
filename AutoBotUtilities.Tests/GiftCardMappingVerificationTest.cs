using System;
using System.Linq;
using NUnit.Framework;
using OCR.Business.Entities;
using Serilog;
using System.Data.Entity;

namespace AutoBotUtilities.Tests
{
    /// <summary>
    /// Verification test to check if DatabaseValidator actually removed the duplicate Gift Card mapping permanently
    /// </summary>
    [TestFixture]
    [Category("Database")]
    [Category("Verification")]
    public class GiftCardMappingVerificationTest
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
        public void VerifyGiftCardFieldMappingState_LineId1830()
        {
            _logger.Information("🔍 **VERIFICATION_START**: Checking current state of Gift Card mapping for LineId 1830");

            using (var ctx = new OCRContext())
            {
                // Query all fields for LineId 1830 (Gift Card line)
                var giftCardFields = ctx.Fields
                    .Include(f => f.Lines)
                    .Include(f => f.Lines.Parts)
                    .Include(f => f.Lines.Parts.Templates)
                    .Where(f => f.LineId == 1830)
                    .ToList();

                _logger.Information("📊 **FIELD_COUNT**: Found {Count} fields mapped to LineId 1830 (Gift Card)", giftCardFields.Count);

                // Log all mappings for LineId 1830
                foreach (var field in giftCardFields)
                {
                    _logger.Information("🔍 **FIELD_MAPPING**: FieldId={FieldId} | Field='{Field}' | EntityType='{EntityType}' | DataType='{DataType}' | Key='{Key}' | AppendValues={AppendValues}",
                        field.Id, field.Field, field.EntityType, field.DataType, field.Key, field.AppendValues);

                    if (field.Lines != null)
                    {
                        _logger.Information("  📋 **LINE_INFO**: LineId={LineId} | Name='{LineName}' | PartId={PartId}",
                            field.Lines.Id, field.Lines.Name, field.Lines.PartId);

                        if (field.Lines.Parts?.Templates != null)
                        {
                            _logger.Information("  🏷️ **TEMPLATE_INFO**: InvoiceId={InvoiceId} | TemplateName='{TemplateName}'",
                                field.Lines.Parts.Templates.Id, field.Lines.Parts.Templates.Name);
                        }
                    }
                }

                // Check specifically for the duplicate we expect was removed
                var totalOtherCostField = giftCardFields.FirstOrDefault(f => f.Field == "TotalOtherCost");
                var totalInsuranceField = giftCardFields.FirstOrDefault(f => f.Field == "TotalInsurance");

                // Report what we found
                if (totalOtherCostField != null && totalInsuranceField != null)
                {
                    _logger.Error("❌ **DUPLICATE_STILL_EXISTS**: Both TotalOtherCost (FieldId: {OtherCostId}) and TotalInsurance (FieldId: {InsuranceId}) still mapped to LineId 1830",
                        totalOtherCostField.Id, totalInsuranceField.Id);
                    _logger.Error("🚨 **DATABASE_CLEANUP_FAILED**: DatabaseValidator did NOT permanently remove the duplicate mapping");
                }
                else if (totalOtherCostField != null && totalInsuranceField == null)
                {
                    _logger.Information("✅ **DUPLICATE_REMOVED**: Only TotalOtherCost (FieldId: {FieldId}) remains - TotalInsurance mapping was successfully removed",
                        totalOtherCostField.Id);
                    _logger.Information("🎯 **DATABASE_CLEANUP_SUCCESS**: DatabaseValidator successfully removed the duplicate TotalInsurance mapping");
                }
                else if (totalOtherCostField == null && totalInsuranceField != null)
                {
                    _logger.Information("✅ **DUPLICATE_REMOVED**: Only TotalInsurance (FieldId: {FieldId}) remains - TotalOtherCost mapping was successfully removed",
                        totalInsuranceField.Id);
                    _logger.Information("🎯 **DATABASE_CLEANUP_SUCCESS**: DatabaseValidator successfully removed the duplicate TotalOtherCost mapping");
                }
                else
                {
                    _logger.Warning("⚠️ **NO_MAPPINGS_FOUND**: Neither TotalOtherCost nor TotalInsurance found for LineId 1830");
                    _logger.Warning("🔍 **INVESTIGATE**: This might indicate a different issue or the line was removed entirely");
                }

                // Also check by specific FieldId 2579 (the user-reported duplicate)
                var fieldId2579 = ctx.Fields
                    .Include(f => f.Lines)
                    .FirstOrDefault(f => f.Id == 2579);

                if (fieldId2579 != null)
                {
                    _logger.Information("🔍 **FIELDID_2579_STATUS**: Field='{Field}' | EntityType='{EntityType}' | LineId={LineId} | Key='{Key}'",
                        fieldId2579.Field, fieldId2579.EntityType, fieldId2579.LineId, fieldId2579.Key);
                    _logger.Warning("⚠️ **FIELDID_2579_STILL_EXISTS**: The specific duplicate field (ID: 2579) mentioned by user still exists in database");
                }
                else
                {
                    _logger.Information("✅ **FIELDID_2579_REMOVED**: FieldId 2579 (user-reported duplicate) was successfully deleted from database");
                }

                // Summary assertion
                if (giftCardFields.Count <= 1)
                {
                    _logger.Information("✅ **VERIFICATION_PASSED**: Gift Card line has {Count} field mapping(s) - no duplicates detected", giftCardFields.Count);
                    Assert.Pass($"Gift Card mapping is clean with {giftCardFields.Count} field(s)");
                }
                else
                {
                    _logger.Error("❌ **VERIFICATION_FAILED**: Gift Card line has {Count} field mappings - duplicates still exist", giftCardFields.Count);
                    Assert.Fail($"Duplicate mappings still exist: {giftCardFields.Count} fields found for LineId 1830");
                }
            }

            _logger.Information("✅ **VERIFICATION_COMPLETE**: Gift Card mapping verification completed");
        }

        [Test]
        public void CheckAllFieldMappingsForAmazonTemplate()
        {
            _logger.Information("🔍 **AMAZON_TEMPLATE_SCAN**: Checking all field mappings for Amazon template (InvoiceId 5)");

            using (var ctx = new OCRContext())
            {
                // Get all fields for Amazon template (InvoiceId 5)
                var amazonFields = ctx.Fields
                    .Include(f => f.Lines)
                    .Include(f => f.Lines.Parts)
                    .Include(f => f.Lines.Parts.Templates)
                    .Where(f => f.Lines.Parts.Templates.Id == 5) // Amazon template
                    .OrderBy(f => f.LineId)
                    .ThenBy(f => f.Field)
                    .ToList();

                _logger.Information("📊 **AMAZON_FIELD_COUNT**: Found {Count} total field mappings for Amazon template", amazonFields.Count);

                // Group by LineId and check for duplicates
                var fieldsByLine = amazonFields.GroupBy(f => f.LineId).ToList();

                foreach (var lineGroup in fieldsByLine)
                {
                    var lineId = lineGroup.Key;
                    var fields = lineGroup.ToList();
                    var lineName = fields.FirstOrDefault()?.Lines?.Name ?? "Unknown";

                    _logger.Information("📋 **LINE_FIELDS**: LineId={LineId} | Name='{LineName}' | FieldCount={FieldCount}",
                        lineId, lineName, fields.Count);

                    if (fields.Count > 1)
                    {
                        _logger.Warning("⚠️ **POTENTIAL_DUPLICATE**: LineId {LineId} ('{LineName}') has {Count} field mappings:",
                            lineId, lineName, fields.Count);

                        foreach (var field in fields)
                        {
                            _logger.Warning("  🔍 **DUPLICATE_FIELD**: FieldId={FieldId} | Field='{Field}' | EntityType='{EntityType}' | Key='{Key}'",
                                field.Id, field.Field, field.EntityType, field.Key);
                        }

                        // Check if these are actually duplicates (same Key pointing to different Fields)
                        var duplicateKeys = fields.GroupBy(f => f.Key).Where(g => g.Count() > 1).ToList();
                        foreach (var keyGroup in duplicateKeys)
                        {
                            var key = keyGroup.Key;
                            var duplicateFields = keyGroup.ToList();
                            _logger.Error("🚨 **CONFIRMED_DUPLICATE**: Key='{Key}' maps to {Count} different fields: [{Fields}]",
                                key, duplicateFields.Count, string.Join(", ", duplicateFields.Select(f => f.Field)));
                        }
                    }
                    else
                    {
                        var field = fields.First();
                        _logger.Information("  ✅ **CLEAN_MAPPING**: FieldId={FieldId} | Field='{Field}' | Key='{Key}'",
                            field.Id, field.Field, field.Key);
                    }
                }

                // Count total duplicate lines
                var duplicateLines = fieldsByLine.Where(g => g.Count() > 1).ToList();
                if (duplicateLines.Any())
                {
                    _logger.Error("❌ **AMAZON_TEMPLATE_HAS_DUPLICATES**: {Count} lines have multiple field mappings", duplicateLines.Count);
                    Assert.Fail($"Amazon template still has {duplicateLines.Count} lines with duplicate field mappings");
                }
                else
                {
                    _logger.Information("✅ **AMAZON_TEMPLATE_CLEAN**: All lines have single field mappings - no duplicates detected");
                    Assert.Pass("Amazon template field mappings are clean");
                }
            }

            _logger.Information("✅ **AMAZON_TEMPLATE_SCAN_COMPLETE**: Amazon template field mapping scan completed");
        }
    }
}