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
    public class DeleteSpecificAutoOmissionLine
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
        /// Deletes the specific problematic AutoOmission line: AutoOmission_TotalDeduction_085003263
        /// Line ID: 2327, Field ID: 3322, RegEx: (?<TotalDeduction>\d+\.\d{2})
        /// </summary>
        [Test]
        [Explicit("Run manually to delete specific problematic line")]
        public async Task DeleteSpecificProblematicAutoOmissionLine()
        {
            _logger.Information("🗑️ **DELETE_SPECIFIC_LINE**: Starting deletion of AutoOmission_TotalDeduction_085003263");
            
            using (var context = new OCRContext())
            {
                // Find the specific line by name
                var targetLine = await context.Lines
                    .Include(l => l.RegularExpressions)
                    .Include(l => l.Fields)
                    .Where(l => l.Name == "AutoOmission_TotalDeduction_085003263")
                    .FirstOrDefaultAsync();

                if (targetLine != null)
                {
                    _logger.Information("🔍 **FOUND_TARGET_LINE**: LineId={LineId}, Name='{Name}', RegexPattern='{Pattern}'", 
                        targetLine.Id, targetLine.Name, targetLine.RegularExpressions?.RegEx ?? "NULL");

                    // Get associated field (should be FieldId 3322)
                    var associatedField = targetLine.Fields?.FirstOrDefault();
                    if (associatedField != null)
                    {
                        _logger.Information("🔍 **ASSOCIATED_FIELD**: FieldId={FieldId}, Key='{Key}', Field='{Field}', EntityType='{EntityType}'", 
                            associatedField.Id, associatedField.Key, associatedField.Field, associatedField.EntityType);
                    }

                    // Get associated regex pattern
                    var associatedRegex = targetLine.RegularExpressions;
                    if (associatedRegex != null)
                    {
                        _logger.Information("🔍 **ASSOCIATED_REGEX**: RegexId={RegexId}, Pattern='{Pattern}', Description='{Description}'", 
                            associatedRegex.Id, associatedRegex.RegEx, associatedRegex.Description ?? "NULL");
                    }

                    // Delete associated field first (due to foreign key constraints)
                    if (associatedField != null)
                    {
                        _logger.Information("🗑️ **DELETING_FIELD**: Removing Field ID {FieldId}", associatedField.Id);
                        context.Fields.Remove(associatedField);
                    }

                    // Delete the line
                    _logger.Information("🗑️ **DELETING_LINE**: Removing Line ID {LineId} ('{Name}')", targetLine.Id, targetLine.Name);
                    context.Lines.Remove(targetLine);

                    // Delete associated regex if it's not used elsewhere
                    if (associatedRegex != null)
                    {
                        var otherLinesUsingRegex = await context.Lines
                            .Where(l => l.RegExId == associatedRegex.Id && l.Id != targetLine.Id)
                            .CountAsync();

                        if (otherLinesUsingRegex == 0)
                        {
                            _logger.Information("🗑️ **DELETING_REGEX**: Removing unused RegEx ID {RegexId}", associatedRegex.Id);
                            context.RegularExpressions.Remove(associatedRegex);
                        }
                        else
                        {
                            _logger.Information("⚠️ **KEEPING_REGEX**: RegEx ID {RegexId} is used by {Count} other lines, keeping it", 
                                associatedRegex.Id, otherLinesUsingRegex);
                        }
                    }

                    // Save changes
                    int deletedCount = await context.SaveChangesAsync();
                    _logger.Information("✅ **DELETION_SUCCESS**: Deleted {Count} database entities for AutoOmission_TotalDeduction_085003263", deletedCount);

                    // Verify deletion
                    var verifyLine = await context.Lines
                        .Where(l => l.Name == "AutoOmission_TotalDeduction_085003263")
                        .FirstOrDefaultAsync();

                    if (verifyLine == null)
                    {
                        _logger.Information("✅ **VERIFICATION_SUCCESS**: AutoOmission_TotalDeduction_085003263 line successfully deleted");
                    }
                    else
                    {
                        _logger.Error("❌ **VERIFICATION_FAILED**: Line still exists after deletion attempt");
                    }
                }
                else
                {
                    _logger.Information("ℹ️ **LINE_NOT_FOUND**: AutoOmission_TotalDeduction_085003263 line not found - may have been already deleted");
                    
                    // Check for any lines with the problematic regex pattern
                    var problematicLines = await context.Lines
                        .Include(l => l.RegularExpressions)
                        .Where(l => l.RegularExpressions != null && l.RegularExpressions.RegEx == "(?<TotalDeduction>\\d+\\.\\d{2})")
                        .ToListAsync();

                    if (problematicLines.Any())
                    {
                        _logger.Warning("⚠️ **FOUND_SIMILAR_PATTERNS**: Found {Count} other lines with the same problematic pattern", problematicLines.Count);
                        foreach (var line in problematicLines)
                        {
                            _logger.Warning("   → LineId={LineId}, Name='{Name}', Pattern='{Pattern}'", 
                                line.Id, line.Name, line.RegularExpressions.RegEx);
                        }
                    }
                    else
                    {
                        _logger.Information("✅ **NO_PROBLEMATIC_PATTERNS**: No lines found with the problematic (?<TotalDeduction>\\d+\\.\\d{2}) pattern");
                    }
                }
            }
        }

        /// <summary>
        /// Alternative method to delete by specific IDs if name-based lookup fails
        /// </summary>
        [Test]
        [Explicit("Run manually to delete by specific database IDs")]
        public async Task DeleteBySpecificIds()
        {
            _logger.Information("🗑️ **DELETE_BY_IDS**: Starting deletion using specific database IDs");
            
            using (var context = new OCRContext())
            {
                // Delete Field ID 3322 first (foreign key constraint)
                var targetField = await context.Fields.FindAsync(3322);
                if (targetField != null)
                {
                    _logger.Information("🗑️ **DELETING_FIELD_BY_ID**: Removing Field ID 3322 - Key='{Key}', Field='{Field}'", 
                        targetField.Key, targetField.Field);
                    context.Fields.Remove(targetField);
                }

                // Delete Line ID 2327
                var targetLine = await context.Lines.FindAsync(2327);
                if (targetLine != null)
                {
                    _logger.Information("🗑️ **DELETING_LINE_BY_ID**: Removing Line ID 2327 - Name='{Name}'", targetLine.Name);
                    context.Lines.Remove(targetLine);
                }

                // Check if RegEx pattern is used elsewhere before deleting
                var regexPattern = await context.RegularExpressions
                    .Where(r => r.RegEx == "(?<TotalDeduction>\\d+\\.\\d{2})")
                    .FirstOrDefaultAsync();

                if (regexPattern != null)
                {
                    var otherLinesUsingRegex = await context.Lines
                        .Where(l => l.RegExId == regexPattern.Id && l.Id != 2327)
                        .CountAsync();

                    if (otherLinesUsingRegex == 0)
                    {
                        _logger.Information("🗑️ **DELETING_REGEX_BY_PATTERN**: Removing unused RegEx pattern '{Pattern}'", regexPattern.RegEx);
                        context.RegularExpressions.Remove(regexPattern);
                    }
                    else
                    {
                        _logger.Information("⚠️ **KEEPING_REGEX_BY_PATTERN**: RegEx pattern used by {Count} other lines, keeping it", otherLinesUsingRegex);
                    }
                }

                // Save changes
                int deletedCount = await context.SaveChangesAsync();
                _logger.Information("✅ **DELETION_BY_IDS_SUCCESS**: Deleted {Count} database entities using specific IDs", deletedCount);
            }
        }

        /// <summary>
        /// Comprehensive cleanup of all AutoOmission_TotalDeduction patterns
        /// </summary>
        [Test]
        [Explicit("Run manually to clean up all AutoOmission TotalDeduction patterns")]
        public async Task CleanupAllAutoOmissionTotalDeductionPatterns()
        {
            _logger.Information("🧹 **COMPREHENSIVE_CLEANUP**: Starting cleanup of all AutoOmission TotalDeduction patterns");
            
            using (var context = new OCRContext())
            {
                // Find all lines that start with "AutoOmission_TotalDeduction"
                var autoOmissionLines = await context.Lines
                    .Include(l => l.RegularExpressions)
                    .Include(l => l.Fields)
                    .Where(l => l.Name.StartsWith("AutoOmission_TotalDeduction"))
                    .ToListAsync();

                _logger.Information("🔍 **FOUND_AUTO_OMISSION_LINES**: Found {Count} AutoOmission TotalDeduction lines", autoOmissionLines.Count);

                foreach (var line in autoOmissionLines)
                {
                    _logger.Information("📋 **AUTO_OMISSION_LINE**: LineId={LineId}, Name='{Name}', Pattern='{Pattern}'", 
                        line.Id, line.Name, line.RegularExpressions?.RegEx ?? "NULL");

                    // Check if this is a problematic pattern (too broad)
                    bool isProblematic = line.RegularExpressions?.RegEx == "(?<TotalDeduction>\\d+\\.\\d{2})" ||
                                        (line.RegularExpressions?.RegEx?.Length ?? 0) < 30;

                    if (isProblematic)
                    {
                        _logger.Warning("🚨 **PROBLEMATIC_PATTERN**: Marking for deletion - LineId={LineId}, Pattern='{Pattern}'", 
                            line.Id, line.RegularExpressions?.RegEx);

                        // Delete associated fields
                        var fieldsToDelete = line.Fields?.ToList() ?? new System.Collections.Generic.List<Fields>();
                        foreach (var field in fieldsToDelete)
                        {
                            _logger.Information("🗑️ **DELETING_ASSOCIATED_FIELD**: FieldId={FieldId}", field.Id);
                            context.Fields.Remove(field);
                        }

                        // Delete the line
                        _logger.Information("🗑️ **DELETING_PROBLEMATIC_LINE**: LineId={LineId}", line.Id);
                        context.Lines.Remove(line);
                    }
                    else
                    {
                        _logger.Information("✅ **KEEPING_SPECIFIC_PATTERN**: Pattern appears specific enough - LineId={LineId}", line.Id);
                    }
                }

                // Clean up unused regex patterns
                var broadTotalDeductionPatterns = await context.RegularExpressions
                    .Where(r => r.RegEx == "(?<TotalDeduction>\\d+\\.\\d{2})")
                    .ToListAsync();

                foreach (var regex in broadTotalDeductionPatterns)
                {
                    var linesUsingRegex = await context.Lines
                        .Where(l => l.RegExId == regex.Id)
                        .CountAsync();

                    if (linesUsingRegex == 0)
                    {
                        _logger.Information("🗑️ **DELETING_UNUSED_REGEX**: RegexId={RegexId}, Pattern='{Pattern}'", regex.Id, regex.RegEx);
                        context.RegularExpressions.Remove(regex);
                    }
                }

                // Save all changes
                int deletedCount = await context.SaveChangesAsync();
                _logger.Information("✅ **COMPREHENSIVE_CLEANUP_SUCCESS**: Deleted {Count} total database entities", deletedCount);

                // Final verification
                var remainingProblematicLines = await context.Lines
                    .Include(l => l.RegularExpressions)
                    .Where(l => l.RegularExpressions != null && l.RegularExpressions.RegEx == "(?<TotalDeduction>\\d+\\.\\d{2})")
                    .CountAsync();

                _logger.Information("📊 **FINAL_VERIFICATION**: {Count} lines remain with problematic TotalDeduction pattern", remainingProblematicLines);
            }
        }
    }
}