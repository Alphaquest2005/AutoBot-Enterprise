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
    public class DatabaseCleanupVerificationTest
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
        public void DatabaseValidator_ShouldPermanentlyRemoveDuplicateFieldMappings()
        {
            _logger.Information("🔍 **DATABASE_CLEANUP_VERIFICATION_TEST_START**: Testing DatabaseValidator permanent cleanup");

            // Enhanced logging for this test
            using (LogLevelOverride.Begin(LogEventLevel.Verbose))
            {
                using (var context = new OCRContext())
                {
                    var validator = new DatabaseValidator(context, _logger);

                    // Step 1: Detect current duplicate mappings (should find Gift Card duplicates)
                    var initialDuplicates = validator.DetectDuplicateFieldMappings();
                    _logger.Information("🔍 **INITIAL_DUPLICATE_COUNT**: Found {DuplicateCount} duplicate field mapping groups", initialDuplicates.Count);

                    // Find specifically the Gift Card duplicate if it exists
                    var giftCardDuplicate = initialDuplicates.FirstOrDefault(d => 
                        d.Key?.IndexOf("GiftCard", StringComparison.OrdinalIgnoreCase) >= 0 ||
                        d.Key?.IndexOf("TotalInsurance", StringComparison.OrdinalIgnoreCase) >= 0);

                    if (giftCardDuplicate != null)
                    {
                        _logger.Information("🎯 **GIFT_CARD_DUPLICATE_FOUND**: LineId={LineId}, Key='{Key}', FieldCount={FieldCount}",
                            giftCardDuplicate.LineId, giftCardDuplicate.Key, giftCardDuplicate.DuplicateFields.Count);

                        // Show the specific duplicates
                        foreach (var field in giftCardDuplicate.DuplicateFields)
                        {
                            _logger.Information("🔍 **GIFT_CARD_FIELD**: FieldId={FieldId}, Field='{Field}', EntityType='{EntityType}'",
                                field.FieldId, field.Field, field.EntityType);
                        }

                        // Step 2: Execute cleanup and verify it works
                        var cleanupResult = validator.CleanupDuplicateFieldMappings(new[] { giftCardDuplicate }.ToList());
                        
                        _logger.Information("🔧 **CLEANUP_RESULT**: Success={Success}, Kept={Kept}, Removed={Removed}",
                            cleanupResult.Success, cleanupResult.KeptCount, cleanupResult.RemovedCount);

                        // Step 3: Verify the cleanup was permanent by re-detecting duplicates
                        var postCleanupDuplicates = validator.DetectDuplicateFieldMappings();
                        var postCleanupGiftCard = postCleanupDuplicates.FirstOrDefault(d => 
                            d.Key?.IndexOf("GiftCard", StringComparison.OrdinalIgnoreCase) >= 0 ||
                            d.Key?.IndexOf("TotalInsurance", StringComparison.OrdinalIgnoreCase) >= 0);

                        if (postCleanupGiftCard == null)
                        {
                            _logger.Information("✅ **DATABASE_CLEANUP_SUCCESS**: Gift Card duplicate mapping successfully removed from database");
                            Assert.Pass("DatabaseValidator successfully removed duplicate field mappings permanently");
                        }
                        else
                        {
                            _logger.Error("❌ **DATABASE_CLEANUP_FAILED**: Gift Card duplicate still exists after cleanup - LineId={LineId}, FieldCount={FieldCount}",
                                postCleanupGiftCard.LineId, postCleanupGiftCard.DuplicateFields.Count);
                            
                            // Show what's still there
                            foreach (var field in postCleanupGiftCard.DuplicateFields)
                            {
                                _logger.Error("🔍 **REMAINING_DUPLICATE**: FieldId={FieldId}, Field='{Field}', EntityType='{EntityType}'",
                                    field.FieldId, field.Field, field.EntityType);
                            }
                            
                            Assert.Fail($"DatabaseValidator cleanup failed - {postCleanupGiftCard.DuplicateFields.Count} duplicates still exist");
                        }
                    }
                    else
                    {
                        _logger.Information("✅ **NO_GIFT_CARD_DUPLICATES**: No Gift Card duplicates found - database may already be clean");
                        
                        if (initialDuplicates.Any())
                        {
                            _logger.Information("🔍 **OTHER_DUPLICATES_FOUND**: {Count} other duplicate groups found:", initialDuplicates.Count);
                            foreach (var duplicate in initialDuplicates.Take(3))
                            {
                                _logger.Information("🔍 **OTHER_DUPLICATE**: LineId={LineId}, Key='{Key}', FieldCount={FieldCount}",
                                    duplicate.LineId, duplicate.Key, duplicate.DuplicateFields.Count);
                            }
                            
                            // Test cleanup on first available duplicate
                            var testDuplicate = initialDuplicates.First();
                            var cleanupResult = validator.CleanupDuplicateFieldMappings(new[] { testDuplicate }.ToList());
                            
                            _logger.Information("🔧 **TEST_CLEANUP_RESULT**: Success={Success}, Kept={Kept}, Removed={Removed}",
                                cleanupResult.Success, cleanupResult.KeptCount, cleanupResult.RemovedCount);
                                
                            Assert.That(cleanupResult.Success, Is.True, "DatabaseValidator cleanup should succeed");
                            Assert.That(cleanupResult.RemovedCount, Is.GreaterThan(0), "DatabaseValidator should remove at least one duplicate mapping");
                        }
                        else
                        {
                            Assert.Pass("No duplicate field mappings found - database is clean");
                        }
                    }
                }
            }
        }
    }
}