using System;
using System.Linq;
using NUnit.Framework;
using Serilog;
using OCR.Business.Entities;
using InvoiceReader.OCRCorrectionService;
using Core.Common.Extensions;
using Serilog.Events;

namespace AutoBotUtilities.Tests
{
    [TestFixture]
    public class DatabaseCleanupExecutionTest
    {
        private ILogger _logger;

        [SetUp]
        public void Setup()
        {
            _logger = new LoggerConfiguration()
                .MinimumLevel.Debug()
                .WriteTo.Console()
                .CreateLogger();
        }

        [Test]
        public void DatabaseValidator_ShouldExecuteCleanupForGiftCardConflict()
        {
            _logger.Information("🔍 **DATABASE_CLEANUP_EXECUTION_TEST**: Testing actual cleanup execution for Gift Card conflict");

            using (LogLevelOverride.Begin(LogEventLevel.Verbose))
            {
                using (var context = new OCRContext())
                {
                    var validator = new DatabaseValidator(context, _logger);

                    try
                    {
                        // Step 1: Detect duplicates (should now find Gift Card conflict)
                        var duplicates = validator.DetectDuplicateFieldMappings();
                        _logger.Information("🔍 **DETECTION_RESULT**: Found {Count} duplicate/conflict groups", duplicates.Count);

                        // Step 2: Find the Gift Card conflict specifically
                        var giftCardConflict = duplicates.FirstOrDefault(d => d.LineId == 1830);
                        if (giftCardConflict == null)
                        {
                            Assert.Fail("CRITICAL: Enhanced DatabaseValidator failed to detect Gift Card conflict for LineId 1830");
                        }

                        _logger.Information("✅ **GIFT_CARD_CONFLICT_DETECTED**: Found Gift Card conflict with {Count} field mappings", 
                            giftCardConflict.DuplicateFields.Count);

                        foreach (var field in giftCardConflict.DuplicateFields)
                        {
                            var status = field.Field == "TotalInsurance" ? "✅ KEEP" : "❌ DELETE";
                            _logger.Information("   📋 **CONFLICT_MAPPING**: FieldId={FieldId}, Field='{Field}', EntityType='{EntityType}' - {Status}",
                                field.FieldId, field.Field, field.EntityType, status);
                        }

                        // Step 3: Execute cleanup for Gift Card conflict only (safer for testing)
                        _logger.Information("🔧 **CLEANUP_EXECUTION**: Executing cleanup for Gift Card conflict only");
                        var cleanupResult = validator.CleanupDuplicateFieldMappings(new[] { giftCardConflict }.ToList());

                        _logger.Information("🔧 **CLEANUP_RESULT**: Success={Success}, Kept={Kept}, Removed={Removed}, Message='{Message}'",
                            cleanupResult.Success, cleanupResult.KeptCount, cleanupResult.RemovedCount, cleanupResult.ErrorMessage);

                        // Step 4: Log cleanup actions taken
                        foreach (var action in cleanupResult.CleanupActions)
                        {
                            var actionSymbol = action.ActionType == "KEEP_PRIMARY" ? "✅" : "❌";
                            _logger.Information("   {Symbol} **CLEANUP_ACTION**: {ActionType} - Field='{Field}', FieldId={FieldId}, Reason='{Reason}'",
                                actionSymbol, action.ActionType, action.Field, action.FieldId, action.Reason);
                        }

                        // Step 5: Verify cleanup result expectations
                        if (cleanupResult.Success)
                        {
                            if (cleanupResult.KeptCount == 1 && cleanupResult.RemovedCount == 1)
                            {
                                _logger.Information("✅ **CLEANUP_SUCCESS**: Gift Card conflict resolved - kept TotalInsurance, removed TotalOtherCost");
                                
                                // Step 6: Verify the correct field was kept and incorrect was removed
                                var keepAction = cleanupResult.CleanupActions.FirstOrDefault(a => a.ActionType == "KEEP_PRIMARY");
                                var removeAction = cleanupResult.CleanupActions.FirstOrDefault(a => a.ActionType == "REMOVE_DUPLICATE");
                                
                                if (keepAction?.Field == "TotalInsurance" && removeAction?.Field == "TotalOtherCost")
                                {
                                    _logger.Information("✅ **CARIBBEAN_CUSTOMS_COMPLIANCE**: Correct field mapping maintained for customer reductions");
                                    Assert.Pass($"SUCCESS: Gift Card conflict cleaned up correctly - kept TotalInsurance (FieldId={keepAction.FieldId}), removed TotalOtherCost (FieldId={removeAction.FieldId})");
                                }
                                else
                                {
                                    Assert.Fail($"INCORRECT CLEANUP: Expected to keep TotalInsurance and remove TotalOtherCost, but kept '{keepAction?.Field}' and removed '{removeAction?.Field}'");
                                }
                            }
                            else
                            {
                                Assert.Fail($"UNEXPECTED RESULT: Expected to keep 1 and remove 1, but kept {cleanupResult.KeptCount} and removed {cleanupResult.RemovedCount}");
                            }
                        }
                        else
                        {
                            _logger.Error("❌ **CLEANUP_FAILED**: Database cleanup failed - {ErrorMessage}", cleanupResult.ErrorMessage);
                            Assert.Fail($"Database cleanup failed: {cleanupResult.ErrorMessage}");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "🚨 **CLEANUP_EXECUTION_ERROR**: Exception during cleanup execution test");
                        Assert.Fail($"Cleanup execution test failed: {ex.Message}");
                    }
                }
            }
        }
    }
}