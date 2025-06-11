using System;
using System.Linq;
using NUnit.Framework;
using OCR.Business.Entities;
using Serilog;
using InvoiceReader.OCRCorrectionService;
using Core.Common.Extensions;

namespace AutoBotUtilities.Tests
{
    /// <summary>
    /// Integration tests for DatabaseValidator using real database to detect and fix issues.
    /// Implements user requirement: "i want the code to test and detect this kind of problems automatically and do the delete"
    /// </summary>
    [TestFixture]
    [Category("Integration")]
    [Category("Database")]
    public class DatabaseValidationIntegrationTests
    {
        private ILogger _logger;
        private OCRContext _context;
        private DatabaseValidator _validator;

        [SetUp]
        public void Setup()
        {
            // Setup test logging with Information level to see database validation logs
            _logger = new LoggerConfiguration()
                .MinimumLevel.Information()
                .WriteTo.Console()
                .CreateLogger();

            _context = new OCRContext();
            _validator = new DatabaseValidator(_context, _logger);

            // Configure logging for maximum visibility during database validation
            LogFilterState.EnabledCategoryLevels[LogCategory.Undefined] = Serilog.Events.LogEventLevel.Information;
        }

        [TearDown]
        public void TearDown()
        {
            _context?.Dispose();
        }

        [Test]
        public void DetectDuplicateFieldMappings_WithRealDatabase_ShouldFindActualIssues()
        {
            _logger.Information("🧪 **TEST_START**: Testing duplicate field mapping detection with real database");

            using (LogLevelOverride.Begin(Serilog.Events.LogEventLevel.Information))
            {
                var duplicates = _validator.DetectDuplicateFieldMappings();

                _logger.Information("🔍 **TEST_RESULT**: Found {Count} duplicate field mapping groups in real database", duplicates.Count);

                // Validate results structure
                Assert.That(duplicates, Is.Not.Null, "Should return results from real database");

                // Look specifically for the Gift Card issue mentioned by user
                var giftCardDuplicates = duplicates.Where(d => 
                    d.Key?.IndexOf("GiftCard", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    d.Key?.IndexOf("TotalInsurance", StringComparison.OrdinalIgnoreCase) >= 0 ||
                    d.LineName?.IndexOf("Gift Card", StringComparison.OrdinalIgnoreCase) >= 0).ToList();

                foreach (var giftCardDup in giftCardDuplicates)
                {
                    _logger.Information("🎯 **GIFT_CARD_DUPLICATE**: Line='{LineName}', Key='{Key}', Invoice='{Invoice}', Fields={Count}",
                        giftCardDup.LineName, giftCardDup.Key, giftCardDup.InvoiceName, giftCardDup.DuplicateFields.Count);

                    foreach (var field in giftCardDup.DuplicateFields)
                    {
                        _logger.Information("  📋 **DUPLICATE_FIELD**: ID={FieldId}, Field='{Field}', EntityType='{EntityType}', DataType='{DataType}', AppendValues={AppendValues}",
                            field.FieldId, field.Field, field.EntityType, field.DataType, field.AppendValues);
                    }

                    // Verify this matches the user's reported structure
                    var totalOtherCostField = giftCardDup.DuplicateFields.FirstOrDefault(f => f.Field == "TotalOtherCost");
                    var totalInsuranceField = giftCardDup.DuplicateFields.FirstOrDefault(f => f.Field == "TotalInsurance");

                    if (totalOtherCostField != null && totalInsuranceField != null)
                    {
                        _logger.Information("✅ **USER_REPORTED_ISSUE_CONFIRMED**: Found both TotalOtherCost (ID={OtherCostId}) and TotalInsurance (ID={InsuranceId}) for same key",
                            totalOtherCostField.FieldId, totalInsuranceField.FieldId);
                    }
                }

                // Log all duplicates for review
                foreach (var duplicate in duplicates.Take(10)) // Show first 10
                {
                    _logger.Information("📋 **DUPLICATE_SUMMARY**: Line='{LineName}', Key='{Key}', Invoice='{Invoice}', Fields=[{Fields}]",
                        duplicate.LineName, duplicate.Key, duplicate.InvoiceName, 
                        string.Join(", ", duplicate.DuplicateFields.Select(f => $"{f.Field}({f.EntityType})")));
                }

                if (duplicates.Any())
                {
                    _logger.Information("⚠️ **PRODUCTION_DATABASE_ISSUES**: Found {Count} duplicate field mapping groups that need cleanup", duplicates.Count);
                }
                else
                {
                    _logger.Information("✅ **DATABASE_CLEAN**: No duplicate field mappings detected in production database");
                }
            }

            _logger.Information("✅ **TEST_COMPLETE**: Real database duplicate detection test completed");
        }

        [Test]
        public void AnalyzeAppendValuesUsage_WithRealDatabase_ShouldUnderstandImportBehavior()
        {
            _logger.Information("🧪 **TEST_START**: Testing AppendValues analysis with real database for import behavior understanding");

            using (LogLevelOverride.Begin(Serilog.Events.LogEventLevel.Information))
            {
                var analysis = _validator.AnalyzeAppendValuesUsage();

                _logger.Information("📊 **APPENDVALUES_REAL_DATABASE**: Total={Total}, Append={Append}, Replace={Replace}, Null={Null}",
                    analysis.TotalFields, analysis.AppendTrueCount, analysis.AppendFalseCount, analysis.AppendNullCount);

                // Validate results from real database
                Assert.That(analysis, Is.Not.Null, "Should return AppendValues analysis from real database");
                Assert.That(analysis.TotalFields, Is.GreaterThan(0), "Real database should have fields with DataType values");
                Assert.That(analysis.UsagePatterns, Is.Not.Null, "Should provide usage patterns from real data");

                // Critical analysis: Understanding how numeric fields behave during import
                var numericPatterns = analysis.UsagePatterns
                    .Where(p => p.DataType == "Number" || p.DataType == "Numeric")
                    .ToList();

                _logger.Information("🔢 **NUMERIC_FIELD_BEHAVIOR_ANALYSIS**: Found {Count} numeric field patterns", numericPatterns.Count);

                foreach (var pattern in numericPatterns)
                {
                    string importBehavior = pattern.AppendValues == true ? "SUM/AGGREGATE values across multiple matches" : 
                                          pattern.AppendValues == false ? "REPLACE with last matching value" : 
                                          "UNDEFINED behavior (AppendValues=null)";
                    
                    _logger.Information("💡 **NUMERIC_IMPORT_BEHAVIOR**: DataType='{DataType}', AppendValues={AppendValues} → {Behavior} | Fields={Count}",
                        pattern.DataType, pattern.AppendValues, importBehavior, pattern.Count);

                    // Show sample field names for understanding
                    var sampleFields = pattern.FieldNames.Take(5);
                    _logger.Information("  📝 **SAMPLE_FIELDS**: {SampleFields}", string.Join(", ", sampleFields));

                    // Critical validation: Numeric fields should have explicit AppendValues for predictable behavior
                    if (pattern.AppendValues == null && pattern.Count > 0)
                    {
                        _logger.Warning("⚠️ **UNDEFINED_BEHAVIOR_WARNING**: {Count} numeric fields have AppendValues=null - import behavior may be unpredictable",
                            pattern.Count);
                    }
                }

                // String field behavior
                var stringPatterns = analysis.UsagePatterns.Where(p => p.DataType == "String").ToList();
                foreach (var stringPattern in stringPatterns)
                {
                    _logger.Information("📝 **STRING_IMPORT_BEHAVIOR**: AppendValues={AppendValues} → Concatenation with space separator | Fields={Count}",
                        stringPattern.AppendValues, stringPattern.Count);
                }

                // Validate we have comprehensive coverage
                Assert.That(analysis.UsagePatterns.Any(), "Real database should have AppendValues patterns");
                
                // The most critical insight: Understanding aggregation vs replacement
                if (numericPatterns.Any())
                {
                    var aggregationCount = numericPatterns.Where(p => p.AppendValues == true).Sum(p => p.Count);
                    var replacementCount = numericPatterns.Where(p => p.AppendValues == false).Sum(p => p.Count);
                    
                    _logger.Information("🎯 **CRITICAL_INSIGHT**: {AggregationCount} numeric fields will AGGREGATE (sum values), {ReplacementCount} will REPLACE (take last value)",
                        aggregationCount, replacementCount);
                }
            }

            _logger.Information("✅ **TEST_COMPLETE**: Real database AppendValues analysis completed - understanding achieved");
        }

        [Test]
        public void GenerateHealthReport_WithRealDatabase_ShouldIdentifyActualIssues()
        {
            _logger.Information("🧪 **TEST_START**: Testing comprehensive database health report with real database");

            using (LogLevelOverride.Begin(Serilog.Events.LogEventLevel.Information))
            {
                var report = _validator.GenerateHealthReport();

                _logger.Information("🏥 **HEALTH_REPORT_SUMMARY**: Generated at {ReportDate}, Overall Status = {Status}",
                    report.ReportDate, report.OverallStatus);

                // Validate comprehensive health report
                Assert.That(report, Is.Not.Null, "Should generate health report from real database");
                Assert.That(report.Categories, Is.Not.Null, "Should have health categories");
                Assert.That(report.Categories.Any(), "Should have at least one health category");

                // Check each category with real database data
                foreach (var category in report.Categories)
                {
                    _logger.Information("📋 **HEALTH_CATEGORY**: {CategoryName} - Status: {Status}, Issues: {IssueCount}",
                        category.Name, category.Status, category.Issues.Count);

                    if (category.Issues.Any())
                    {
                        _logger.Warning("⚠️ **CATEGORY_ISSUES**: {CategoryName} has {IssueCount} issues in production database:",
                            category.Name, category.Issues.Count);

                        foreach (var issue in category.Issues.Take(3)) // Sample first 3 issues
                        {
                            _logger.Warning("  ❌ **ISSUE**: {Description} | Details: {Details}", issue.Description, issue.Details);
                        }

                        if (category.Issues.Count > 3)
                        {
                            _logger.Information("  📋 **MORE_ISSUES**: ... and {MoreCount} additional issues", category.Issues.Count - 3);
                        }
                    }
                    else
                    {
                        _logger.Information("  ✅ **CATEGORY_CLEAN**: {CategoryName} has no issues", category.Name);
                    }
                }

                // Overall assessment
                var failedCategories = report.Categories.Where(c => c.Status == "FAIL").ToList();
                var totalIssues = report.Categories.Sum(c => c.Issues.Count);

                _logger.Information("🎯 **OVERALL_ASSESSMENT**: Categories={Total}, Failed={Failed}, Total Issues={Issues}, Status={OverallStatus}",
                    report.Categories.Count, failedCategories.Count, totalIssues, report.OverallStatus);

                if (totalIssues > 0)
                {
                    _logger.Warning("🚨 **PRODUCTION_DATABASE_NEEDS_ATTENTION**: {Issues} total issues found across {FailedCategories} categories",
                        totalIssues, failedCategories.Count);
                }
                else
                {
                    _logger.Information("✅ **PRODUCTION_DATABASE_HEALTHY**: No issues detected in any category");
                }

                // Validate status logic
                var expectedStatus = failedCategories.Any() ? "FAIL" : "PASS";
                Assert.That(report.OverallStatus, Is.EqualTo(expectedStatus), 
                    "Overall status should be FAIL if any category fails, PASS if all pass");
            }

            _logger.Information("✅ **TEST_COMPLETE**: Real database health report generated and analyzed");
        }

        [Test]
        [Explicit("Run manually when you want to actually clean up production database issues")]
        public void CleanupProductionDatabaseIssues_AutomatedRepair()
        {
            _logger.Information("🧪 **PRODUCTION_CLEANUP_START**: Automated cleanup of production database issues");
            _logger.Warning("⚠️ **PRODUCTION_WARNING**: This test will make actual changes to the production database");

            using (LogLevelOverride.Begin(Serilog.Events.LogEventLevel.Information))
            {
                // Step 1: Generate baseline health report
                _logger.Information("🏥 **STEP_1**: Generating baseline health report");
                var baselineReport = _validator.GenerateHealthReport();
                var baselineIssues = baselineReport.Categories.Sum(c => c.Issues.Count);
                
                _logger.Information("📊 **BASELINE**: {Issues} total issues detected before cleanup", baselineIssues);

                if (baselineIssues == 0)
                {
                    _logger.Information("✅ **NO_CLEANUP_NEEDED**: Database is already healthy");
                    Assert.Pass("No issues found - database is clean");
                    return;
                }

                // Step 2: Detect and cleanup duplicate field mappings
                var duplicateMappingCategory = baselineReport.Categories.FirstOrDefault(c => c.Name == "Duplicate Field Mappings");
                if (duplicateMappingCategory != null && duplicateMappingCategory.Status == "FAIL")
                {
                    _logger.Information("🔧 **STEP_2**: Cleaning up duplicate field mappings");
                    
                    var duplicates = _validator.DetectDuplicateFieldMappings();
                    if (duplicates.Any())
                    {
                        _logger.Warning("🚨 **DUPLICATES_DETECTED**: {Count} duplicate groups found - proceeding with automated cleanup", duplicates.Count);

                        foreach (var duplicate in duplicates)
                        {
                            _logger.Information("🔧 **CLEANING_DUPLICATE**: Line='{LineName}', Key='{Key}', Fields={Count}",
                                duplicate.LineName, duplicate.Key, duplicate.DuplicateFields.Count);
                        }

                        var cleanupResult = _validator.CleanupDuplicateFieldMappings(duplicates);
                        
                        if (cleanupResult.Success)
                        {
                            _logger.Information("✅ **CLEANUP_SUCCESS**: Removed {RemovedCount} duplicate mappings, kept {KeptCount} primary mappings",
                                cleanupResult.RemovedCount, cleanupResult.KeptCount);

                            // Log cleanup actions for audit trail
                            foreach (var action in cleanupResult.CleanupActions)
                            {
                                _logger.Information("📋 **CLEANUP_ACTION**: {ActionType} - Line='{LineName}', Key='{Key}', Field='{Field}', Reason='{Reason}'",
                                    action.ActionType, action.LineName, action.Key, action.Field, action.Reason);
                            }
                        }
                        else
                        {
                            _logger.Error("❌ **CLEANUP_FAILED**: {ErrorMessage}", cleanupResult.ErrorMessage);
                            Assert.Fail($"Duplicate cleanup failed: {cleanupResult.ErrorMessage}");
                        }
                    }
                }

                // Step 3: Verify cleanup effectiveness
                _logger.Information("🔍 **STEP_3**: Verifying cleanup effectiveness");
                var postCleanupReport = _validator.GenerateHealthReport();
                var postCleanupIssues = postCleanupReport.Categories.Sum(c => c.Issues.Count);

                _logger.Information("📊 **CLEANUP_RESULTS**: Before={BeforeIssues}, After={AfterIssues}, Improvement={Improvement}",
                    baselineIssues, postCleanupIssues, baselineIssues - postCleanupIssues);

                // Validate improvement
                Assert.That(postCleanupIssues, Is.LessThanOrEqualTo(baselineIssues), 
                    "Cleanup should not increase the number of issues");

                if (postCleanupIssues < baselineIssues)
                {
                    _logger.Information("🎯 **CLEANUP_EFFECTIVE**: Successfully reduced database issues from {Before} to {After}",
                        baselineIssues, postCleanupIssues);
                }

                // Final status check
                if (postCleanupReport.OverallStatus == "PASS")
                {
                    _logger.Information("🏆 **DATABASE_FULLY_REPAIRED**: All issues resolved - database is now healthy");
                }
                else
                {
                    _logger.Warning("⚠️ **PARTIAL_REPAIR**: Some issues remain - may require manual intervention");
                }
            }

            _logger.Information("✅ **PRODUCTION_CLEANUP_COMPLETE**: Automated database repair completed");
        }
    }
}