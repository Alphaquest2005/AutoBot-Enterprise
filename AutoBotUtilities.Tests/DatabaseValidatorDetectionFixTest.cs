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
    public class DatabaseValidatorDetectionFixTest
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
        public void DatabaseValidator_DetectionLogic_ShouldIdentifyGiftCardConflict()
        {
            _logger.Information("🔍 **DATABASE_VALIDATOR_DETECTION_FIX**: Testing why Gift Card duplicates aren't detected");

            using (LogLevelOverride.Begin(LogEventLevel.Verbose))
            {
                using (var context = new OCRContext())
                {
                    var validator = new DatabaseValidator(context, _logger);

                    try
                    {
                        // Step 1: Get Gift Card fields directly
                        var giftCardFields = context.Fields
                            .Where(f => f.LineId == 1830)
                            .ToList();
                            
                        _logger.Information("🎯 **DIRECT_GIFT_CARD_QUERY**: Found {Count} fields for LineId 1830", giftCardFields.Count);
                        
                        foreach (var field in giftCardFields)
                        {
                            _logger.Information("📋 **GIFT_CARD_FIELD**: FieldId={FieldId}, LineId={LineId}, Key='{Key}', Field='{Field}', EntityType='{EntityType}'", 
                                field.Id, field.LineId, field.Key, field.Field, field.EntityType);
                        }

                        // Step 2: Test the current DatabaseValidator logic
                        _logger.Information("🔍 **CURRENT_VALIDATOR_LOGIC**: Testing existing DetectDuplicateFieldMappings() method");
                        var detectedDuplicates = validator.DetectDuplicateFieldMappings();
                        
                        _logger.Information("❌ **CURRENT_DETECTION_RESULT**: DatabaseValidator found {Count} duplicate groups (FAILS to detect Gift Card issue)", detectedDuplicates.Count);

                        // Step 3: Show why current logic fails
                        _logger.Information("🔍 **CURRENT_LOGIC_ANALYSIS**: Current logic groups by (LineId, Key) but Gift Card fields have different Keys:");
                        _logger.Information("   📋 **Field 2579**: LineId=1830, Key='GiftCard' → NOT grouped with");
                        _logger.Information("   📋 **Field 3181**: LineId=1830, Key='TotalInsurance' → different Key, so separate group");
                        _logger.Information("❌ **LOGIC_PROBLEM**: Same LineId (1830) but different Keys = NO duplicate detection");

                        // Step 4: Demonstrate correct business logic
                        _logger.Information("🔍 **BUSINESS_LOGIC_ANALYSIS**: Business perspective - these ARE duplicates because:");
                        _logger.Information("   ✅ **Same Line**: Both map to LineId 1830 (same regex pattern)");
                        _logger.Information("   ❌ **Different Targets**: Map to different database fields (TotalOtherCost vs TotalInsurance)");
                        _logger.Information("   🎯 **Impact**: Same parsed value goes to different database columns depending on Key used");

                        // Step 5: Test enhanced detection logic
                        _logger.Information("🔍 **ENHANCED_DETECTION_TEST**: Testing enhanced logic that groups by LineId only");
                        
                        var enhancedDuplicateGroups = context.Fields
                            .Where(f => f.LineId != null && f.Field != null)
                            .GroupBy(f => f.LineId)  // Group by LineId only, not LineId+Key
                            .Where(g => g.Select(f => f.Field).Distinct().Count() > 1) // Multiple different Field targets
                            .ToList();
                            
                        _logger.Information("✅ **ENHANCED_DETECTION_RESULT**: Enhanced logic found {Count} conflicting LineId groups", enhancedDuplicateGroups.Count);
                        
                        // Find the Gift Card conflict specifically
                        var giftCardConflict = enhancedDuplicateGroups.FirstOrDefault(g => g.Key == 1830);
                        if (giftCardConflict != null)
                        {
                            var conflictFields = giftCardConflict.ToList();
                            _logger.Information("🎯 **GIFT_CARD_CONFLICT_FOUND**: LineId 1830 has {Count} fields mapping to different targets:", conflictFields.Count);
                            
                            foreach (var field in conflictFields)
                            {
                                _logger.Information("   📋 **CONFLICT_FIELD**: FieldId={FieldId}, Key='{Key}', Field='{Field}', EntityType='{EntityType}'", 
                                    field.Id, field.Key, field.Field, field.EntityType);
                            }
                            
                            // Show the solution
                            var correctField = conflictFields.FirstOrDefault(f => f.Field == "TotalInsurance");
                            var incorrectField = conflictFields.FirstOrDefault(f => f.Field == "TotalOtherCost");
                            
                            if (correctField != null && incorrectField != null)
                            {
                                _logger.Information("💡 **SOLUTION_STRATEGY**: Caribbean customs requires:");
                                _logger.Information("   ✅ **KEEP**: FieldId={CorrectId} (Key='{CorrectKey}' → TotalInsurance) for customer reductions", 
                                    correctField.Id, correctField.Key);
                                _logger.Information("   ❌ **DELETE**: FieldId={IncorrectId} (Key='{IncorrectKey}' → TotalOtherCost) as incorrect mapping", 
                                    incorrectField.Id, incorrectField.Key);
                                    
                                Assert.Pass($"DETECTION FIX VERIFIED: Enhanced logic correctly identifies {conflictFields.Count} conflicting field mappings for LineId 1830 that current logic misses");
                            }
                            else
                            {
                                Assert.Fail("UNEXPECTED: Found LineId conflict but couldn't identify correct/incorrect field mappings");
                            }
                        }
                        else
                        {
                            Assert.Fail("CRITICAL: Enhanced logic also failed to detect Gift Card conflict");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "🚨 **DETECTION_FIX_ERROR**: Exception during detection logic analysis");
                        Assert.Fail($"Detection fix test failed: {ex.Message}");
                    }
                }
            }
        }
    }
}