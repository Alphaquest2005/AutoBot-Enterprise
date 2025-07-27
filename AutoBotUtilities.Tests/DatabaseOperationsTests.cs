using System;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using OCR.Business.Entities;
using Serilog;

namespace AutoBotUtilities.Tests
{
    [TestFixture]
    public class DatabaseOperationsTests
    {
        private static ILogger _logger;

        [OneTimeSetUp]
        public void Setup()
        {
            _logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
        }

        /// <summary>
        /// Quick verification that the problematic patterns were deleted successfully
        /// </summary>
        [Test]
        public async Task VerifyProblematicPatternsDeleted()
        {
            _logger.Information("🔍 **VERIFICATION**: Checking if problematic AutoOmission patterns were deleted");
            
            using (var context = new OCRContext())
            {
                // Check for any remaining overly broad TotalDeduction patterns
                var broadPatterns = await context.RegularExpressions
                                        .Where(r => r.RegEx == "(?<TotalDeduction>\\d+\\.\\d{2})")
                                        .ToListAsync().ConfigureAwait(false);

                _logger.Information("📊 **BROAD_PATTERNS**: Found {Count} overly broad TotalDeduction patterns", broadPatterns.Count);

                foreach (var pattern in broadPatterns)
                {
                    _logger.Warning("⚠️ **REMAINING_BROAD_PATTERN**: Id={Id}, Description='{Description}', Regex='{Regex}'", 
                        pattern.Id, pattern.Description, pattern.RegEx);
                }

                // Should be 0 after deletion
                Assert.That(broadPatterns.Count, Is.EqualTo(0), 
                    "There should be no overly broad TotalDeduction patterns remaining after deletion");

                // Check remaining TotalDeduction patterns (should be more specific ones)
                var remainingPatterns = await context.RegularExpressions
                                            .Where(r => r.RegEx.Contains("TotalDeduction"))
                                            .ToListAsync().ConfigureAwait(false);

                _logger.Information("📋 **REMAINING_TOTALDEDUCTION_PATTERNS**: Found {Count} specific TotalDeduction patterns", remainingPatterns.Count);

                foreach (var pattern in remainingPatterns)
                {
                    _logger.Information("✅ **SPECIFIC_PATTERN**: Id={Id}, Length={Length}, Regex='{Regex}'", 
                        pattern.Id, pattern.RegEx.Length, pattern.RegEx);
                }

                _logger.Information("✅ **VERIFICATION_COMPLETE**: Problematic patterns successfully deleted");
            }
        }

        /// <summary>
        /// Check Amazon template state after pattern deletion
        /// </summary>
        [Test]
        public async Task VerifyAmazonTemplateState()
        {
            _logger.Information("🔍 **AMAZON_VERIFICATION**: Checking Amazon template state after pattern fixes");
            
            using (var context = new OCRContext())
            {
                // Get Amazon template (InvoiceId 5)
                var amazonTemplate = await context.Templates
                                         .Where(i => i.Id == 5)
                                         .FirstOrDefaultAsync().ConfigureAwait(false);

                Assert.That(amazonTemplate, Is.Not.Null, "Amazon template (ID 5) should exist");
                _logger.Information("✅ **AMAZON_TEMPLATE**: Found template '{Name}' (ID={Id})", amazonTemplate.Name, amazonTemplate.Id);

                // Get all patterns associated with Amazon template
                var amazonPatterns = await context.RegularExpressions
                                         .Include(r => r.Lines)
                                         .Where(r => r.Lines.Any(l => l.Parts.TemplateId == 5))
                                         .ToListAsync().ConfigureAwait(false);

                _logger.Information("📊 **AMAZON_PATTERNS**: Found {Count} regex patterns in Amazon template", amazonPatterns.Count);

                // Check for Gift Card patterns
                var giftCardPatterns = amazonPatterns
                    .Where(r => r.RegEx.Contains("Gift Card") || (r.Description != null && r.Description.Contains("Gift Card")))
                    .ToList();

                _logger.Information("🎁 **GIFT_CARD_PATTERNS**: Found {Count} Gift Card patterns", giftCardPatterns.Count);
                foreach (var pattern in giftCardPatterns)
                {
                    _logger.Information("   → Id={Id}, Regex='{Regex}'", pattern.Id, pattern.RegEx);
                }

                // Check for Free Shipping patterns
                var freeShippingPatterns = amazonPatterns
                    .Where(r => r.RegEx.Contains("Free Shipping") || (r.Description != null && r.Description.Contains("FreeShipping")))
                    .ToList();

                _logger.Information("🚚 **FREE_SHIPPING_PATTERNS**: Found {Count} Free Shipping patterns", freeShippingPatterns.Count);
                foreach (var pattern in freeShippingPatterns)
                {
                    _logger.Information("   → Id={Id}, Regex='{Regex}'", pattern.Id, pattern.RegEx);
                }

                // Check for any TotalDeduction patterns in Amazon template
                var totalDeductionPatterns = amazonPatterns
                    .Where(r => r.RegEx.Contains("TotalDeduction"))
                    .ToList();

                _logger.Information("💰 **TOTALDEDUCTION_PATTERNS**: Found {Count} TotalDeduction patterns in Amazon template", totalDeductionPatterns.Count);
                foreach (var pattern in totalDeductionPatterns)
                {
                    var isProblematic = pattern.RegEx == "(?<TotalDeduction>\\d+\\.\\d{2})";
                    var status = isProblematic ? "❌ PROBLEMATIC" : "✅ OK";
                    _logger.Information("   → Id={Id}, Status={Status}, Regex='{Regex}'", pattern.Id, status, pattern.RegEx);
                    
                    Assert.That(isProblematic, Is.False, $"Pattern {pattern.Id} should not be the problematic broad pattern");
                }

                _logger.Information("✅ **AMAZON_VERIFICATION_COMPLETE**: Amazon template state verified");
            }
        }

        /// <summary>
        /// Show database statistics after the fix
        /// </summary>
        [Test]
        public async Task ShowDatabaseStatistics()
        {
            _logger.Information("📊 **DATABASE_STATS**: Showing OCR database statistics after fix");
            
            using (var context = new OCRContext())
            {
                // Total regex patterns
                var totalPatterns = await context.RegularExpressions.CountAsync().ConfigureAwait(false);
                _logger.Information("📈 **TOTAL_PATTERNS**: {Count} regex patterns in database", totalPatterns);

                // Patterns by type
                var totalDeductionCount = await context.RegularExpressions
                                              .CountAsync(r => r.RegEx.Contains("TotalDeduction")).ConfigureAwait(false);
                _logger.Information("💰 **TOTALDEDUCTION_PATTERNS**: {Count} TotalDeduction patterns", totalDeductionCount);

                var invoiceTotalCount = await context.RegularExpressions
                                            .CountAsync(r => r.RegEx.Contains("InvoiceTotal")).ConfigureAwait(false);
                _logger.Information("🧾 **INVOICETOTAL_PATTERNS**: {Count} InvoiceTotal patterns", invoiceTotalCount);

                var giftCardCount = await context.RegularExpressions
                                        .CountAsync(r => r.RegEx.Contains("Gift Card") || r.RegEx.Contains("TotalInsurance")).ConfigureAwait(false);
                _logger.Information("🎁 **GIFT_CARD_PATTERNS**: {Count} Gift Card/TotalInsurance patterns", giftCardCount);

                // Templates
                var templateCount = await context.Templates.CountAsync().ConfigureAwait(false);
                _logger.Information("📋 **TEMPLATES**: {Count} invoice templates", templateCount);

                // Lines and Parts
                var lineCount = await context.Lines.CountAsync().ConfigureAwait(false);
                var partCount = await context.Parts.CountAsync().ConfigureAwait(false);
                _logger.Information("🧩 **STRUCTURE**: {LineCount} lines, {PartCount} parts", lineCount, partCount);

                _logger.Information("✅ **STATS_COMPLETE**: Database statistics collected");
            }
        }

        /// <summary>
        /// Helper method to clean up any remaining problematic patterns (if needed)
        /// </summary>
        [Test]
        [Explicit("Run manually only if problematic patterns are found")]
        public async Task CleanupRemainingProblematicPatterns()
        {
            _logger.Warning("⚠️ **MANUAL_CLEANUP**: This will delete any remaining problematic patterns");
            
            using (var context = new OCRContext())
            {
                // Find any remaining problematic patterns
                var problematicPatterns = await context.RegularExpressions
                                              .Where(r => r.RegEx == "(?<TotalDeduction>\\d+\\.\\d{2})")
                                              .ToListAsync().ConfigureAwait(false);

                if (problematicPatterns.Any())
                {
                    _logger.Warning("🗑️ **DELETING_REMAINING**: Found {Count} additional problematic patterns to delete", problematicPatterns.Count);
                    
                    foreach (var pattern in problematicPatterns)
                    {
                        _logger.Warning("   → Deleting Id={Id}, Description='{Description}'", pattern.Id, pattern.Description);
                        context.RegularExpressions.Remove(pattern);
                    }

                    var deletedCount = await context.SaveChangesAsync().ConfigureAwait(false);
                    _logger.Information("✅ **CLEANUP_COMPLETE**: Deleted {Count} additional problematic patterns", deletedCount);
                }
                else
                {
                    _logger.Information("✅ **NO_CLEANUP_NEEDED**: No additional problematic patterns found");
                }
            }
        }

        /// <summary>
        /// Show patterns that could potentially cause similar issues
        /// </summary>
        [Test]
        public async Task AnalyzePotentialIssues()
        {
            _logger.Information("🔍 **ISSUE_ANALYSIS**: Looking for other potentially problematic patterns");
            
            using (var context = new OCRContext())
            {
                // Find very short, potentially overly broad patterns
                var shortPatterns = await context.RegularExpressions
                                        .Where(r => r.RegEx.Length < 30 && r.RegEx.Contains("(?<"))
                                        .OrderBy(r => r.RegEx.Length)
                                        .ToListAsync().ConfigureAwait(false);

                _logger.Information("📏 **SHORT_PATTERNS**: Found {Count} potentially overly broad patterns (< 30 chars)", shortPatterns.Count);

                foreach (var pattern in shortPatterns.Take(10)) // Show first 10
                {
                    var riskLevel = pattern.RegEx.Length < 20 ? "🚨 HIGH RISK" : "⚠️ MEDIUM RISK";
                    _logger.Information("   → {RiskLevel}: Id={Id}, Length={Length}, Regex='{Regex}'", 
                        riskLevel, pattern.Id, pattern.RegEx.Length, pattern.RegEx);
                }

                // Find patterns that capture just numbers (could conflict with InvoiceTotal)
                var numberOnlyPatterns = await context.RegularExpressions
                                             .Where(r => r.RegEx.Contains("\\d+\\.\\d") && r.RegEx.Length < 40)
                                             .ToListAsync().ConfigureAwait(false);

                _logger.Information("🔢 **NUMBER_PATTERNS**: Found {Count} patterns that capture numeric values", numberOnlyPatterns.Count);

                var potentialConflicts = numberOnlyPatterns
                    .Where(r => !r.RegEx.Contains("TotalDeduction")) // Exclude ones we just fixed
                    .Take(5)
                    .ToList();

                foreach (var pattern in potentialConflicts)
                {
                    _logger.Information("   → POTENTIAL_CONFLICT: Id={Id}, Regex='{Regex}'", pattern.Id, pattern.RegEx);
                }

                _logger.Information("✅ **ANALYSIS_COMPLETE**: Potential issue analysis finished");
            }
        }
    }
}