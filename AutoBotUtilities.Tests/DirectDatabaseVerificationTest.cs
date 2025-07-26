using System;
using System.Linq;
using NUnit.Framework;
using Serilog;
using OCR.Business.Entities;
using Core.Common.Extensions;
using Serilog.Events;

namespace AutoBotUtilities.Tests
{
    [TestFixture]
    public class DirectDatabaseVerificationTest
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
        public void VerifyDatabaseConnection_ShouldShowActualFieldMappings()
        {
            _logger.Information("🔍 **DIRECT_DATABASE_VERIFICATION**: Checking actual database connection and Gift Card field mappings");

            // using (LogLevelOverride.Begin(LogEventLevel.Verbose)) // COMMENTED OUT: Preventing singleton conflicts
            // {
                using (var context = new OCRContext())
                {
                    // Test database connectivity
                    _logger.Information("💾 **DATABASE_CONNECTION_TEST**: Testing OCRContext database connection");
                    
                    try
                    {
                        var connectionString = context.Database.Connection.ConnectionString;
                        _logger.Information("💾 **DATABASE_CONNECTION_STRING**: {ConnectionString}", connectionString);
                        
                        // Test basic query execution
                        var totalFieldsCount = context.Fields.Count();
                        _logger.Information("💾 **DATABASE_TOTAL_FIELDS**: Found {TotalCount} total fields in database", totalFieldsCount);
                        
                        // Search specifically for Gift Card fields (LineId 1830)
                        var giftCardFields = context.Fields
                            .Where(f => f.LineId == 1830)
                            .ToList();
                            
                        _logger.Information("🎯 **GIFT_CARD_FIELDS_FOUND**: Found {Count} fields for LineId 1830 (Gift Card)", giftCardFields.Count);
                        
                        foreach (var field in giftCardFields)
                        {
                            _logger.Information("🔍 **GIFT_CARD_FIELD_DETAIL**: FieldId={FieldId}, Key='{Key}', Field='{Field}', EntityType='{EntityType}', LineId={LineId}", 
                                field.Id, field.Key, field.Field, field.EntityType, field.LineId);
                        }
                        
                        // Test if we can find the specific FieldIds mentioned by user
                        var userFieldIds = new[] { 2579, 3181 };
                        foreach (var fieldId in userFieldIds)
                        {
                            var specificField = context.Fields.FirstOrDefault(f => f.Id == fieldId);
                            if (specificField != null)
                            {
                                _logger.Information("✅ **USER_FIELD_FOUND**: FieldId={FieldId} found - Key='{Key}', Field='{Field}', EntityType='{EntityType}', LineId={LineId}", 
                                    specificField.Id, specificField.Key, specificField.Field, specificField.EntityType, specificField.LineId);
                            }
                            else
                            {
                                _logger.Warning("❌ **USER_FIELD_NOT_FOUND**: FieldId={FieldId} not found in database", fieldId);
                            }
                        }
                        
                        // Check for any field mapping duplicates across the entire database
                        var duplicateGroups = context.Fields
                            .Where(f => f.LineId != null && f.Key != null && f.Field != null)
                            .GroupBy(f => new { f.LineId, f.Key })
                            .Where(g => g.Select(f => f.Field).Distinct().Count() > 1)
                            .Take(10) // Limit to first 10 for visibility
                            .ToList();
                            
                        _logger.Information("🔍 **DATABASE_DUPLICATE_GROUPS**: Found {Count} duplicate field mapping groups in database", duplicateGroups.Count);
                        
                        foreach (var group in duplicateGroups)
                        {
                            var fieldsInGroup = group.ToList();
                            _logger.Information("❌ **DUPLICATE_GROUP**: LineId={LineId}, Key='{Key}' has {FieldCount} different field mappings:", 
                                group.Key.LineId, group.Key.Key, fieldsInGroup.Count);
                                
                            foreach (var field in fieldsInGroup)
                            {
                                _logger.Information("   📋 **DUPLICATE_FIELD**: FieldId={FieldId}, Field='{Field}', EntityType='{EntityType}'", 
                                    field.Id, field.Field, field.EntityType);
                            }
                        }
                        
                        // Final verification: This should match user's expectation
                        if (giftCardFields.Count >= 2)
                        {
                            Assert.Pass($"CONFIRMED: Found {giftCardFields.Count} Gift Card field mappings in database - duplicates exist as user reported");
                        }
                        else if (giftCardFields.Count == 1)
                        {
                            Assert.Fail($"MISMATCH: Found only {giftCardFields.Count} Gift Card field mapping - user reports duplicates but we see single mapping");
                        }
                        else
                        {
                            Assert.Fail("CRITICAL MISMATCH: Found 0 Gift Card field mappings - different database than user's data");
                        }
                    }
                    catch (Exception ex)
                    {
                        _logger.Error(ex, "🚨 **DATABASE_CONNECTION_ERROR**: Failed to connect to or query database");
                        Assert.Fail($"Database connection failed: {ex.Message}");
                    }
                }
            // }
        }
    }
}