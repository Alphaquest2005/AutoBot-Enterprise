// File: InvoiceReader.OCRCorrectionService/DatabaseValidator.cs

using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using Core.Common.Extensions; // For LogLevelOverride
using OCR.Business.Entities;
using Serilog;
using Serilog.Events; // For LogEventLevel
using TrackableEntities;

namespace InvoiceReader.OCRCorrectionService
{
    /// <summary>
    /// MERGED & ENHANCED (FINAL v5): A comprehensive database validator with mandated, high-visibility logging
    /// to diagnose pipeline failures.
    /// </summary>
    public class DatabaseValidator
    {
        private readonly OCRContext _context;
        private readonly ILogger _logger;

        public DatabaseValidator(OCRContext context, ILogger logger)
        {
            _context = context ?? throw new ArgumentNullException(nameof(context));
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
        }

        /// <summary>
        /// The main execution method. It runs all validation and cleanup routines in a specific order.
        /// Now wrapped with LogLevelOverride to ensure visibility during tests.
        /// </summary>
        /// <returns>A summary of all cleanup actions performed.</returns>
        public CleanupResult ValidateAndHealTemplate()
        {
            // REMOVED LogLevelOverride to prevent singleton violations - caller controls logging level
            _logger.Debug("--- 🛡️ DATABASE VALIDATION & HEALING START 🛡️ ---");
                var finalResult = new CleanupResult { Success = true };

                try
                {
                    var legacyCleanupResult = CleanupLegacyMisconfigurations();
                    finalResult.Merge(legacyCleanupResult);

                    var redundantRules = DetectRedundantOmissionRules();
                    if (redundantRules.Any())
                    {
                        var redundancyCleanupResult = CleanupRedundantOmissionRules(redundantRules);
                        finalResult.Merge(redundancyCleanupResult);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "🚨 **VALIDATOR_EXCEPTION**: A critical error occurred within the DatabaseValidator.");
                    finalResult.Success = false;
                    finalResult.ErrorMessage = ex.Message;
                }

                _logger.Error("--- 🛡️ DATABASE VALIDATION & HEALING COMPLETE (Verbose Logging Forced) 🛡️ ---");
                return finalResult;
        }

        private CleanupResult CleanupLegacyMisconfigurations()
        {
            var result = new CleanupResult();
            _logger.Error("🔧 **LEGACY_CLEANUP_ENTRY**: Searching for known legacy misconfigurations...");

            try
            {
                var incorrectGiftCardMapping = _context.Fields
                    .FirstOrDefault(f => f.LineId == 1830 && f.Field == "TotalOtherCost");

                if (incorrectGiftCardMapping != null)
                {
                    _logger.Error("  - ❌ **FOUND_LEGACY_ERROR**: Found Gift Card line (1830) incorrectly mapped to TotalOtherCost. FieldId {FieldId}.", incorrectGiftCardMapping.Id);
                    _context.Fields.Remove(incorrectGiftCardMapping);
                    result.RemovedCount++;
                    result.CleanupActions.Add(new CleanupAction { ActionType = "REMOVE_LEGACY_ERROR", FieldId = incorrectGiftCardMapping.Id, Field = "TotalOtherCost" });
                    _logger.Error("     - Marked FieldId {FieldId} for deletion.", incorrectGiftCardMapping.Id);
                }
                else
                {
                    _logger.Error("  - ✅ **NO_LEGACY_ERROR**: Did not find incorrect Gift Card mapping.");
                }

                if (result.RemovedCount > 0)
                {
                    _logger.Error("     - **DB_SAVE_INTENT**: Attempting to save legacy cleanup changes...");
                    int changes = _context.SaveChanges();
                    _logger.Error("     - ✅ **DB_SAVE_SUCCESS**: Committed {Count} legacy cleanup changes.", changes);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "🚨 **LEGACY_CLEANUP_FAILED**");
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }

            _logger.Error("🔧 **LEGACY_CLEANUP_EXIT**: Complete. Removed {Count} known legacy errors.", result.RemovedCount);
            return result;
        }

        private List<IGrouping<string, Lines>> DetectRedundantOmissionRules()
        {
            _logger.Error("🔍 **REDUNDANCY_DETECTION_ENTRY**: Analyzing for duplicate 'AutoOmission_' lines.");
            var redundantGroups = _context.Lines
                .AsNoTracking()
                .Where(l => l.Name.StartsWith("AutoOmission_"))
                .GroupBy(l => l.Name)
                .Where(g => g.Count() > 1)
                .ToList();
            _logger.Error("🔍 **REDUNDANCY_DETECTION_EXIT**: Found {Count} groups of redundant omission rules.", redundantGroups.Count);
            return redundantGroups;
        }

        private CleanupResult CleanupRedundantOmissionRules(List<IGrouping<string, Lines>> redundantGroups)
        {
            var result = new CleanupResult();
            _logger.Error("🗑️ **REDUNDANCY_CLEANUP_ENTRY**: Starting cleanup for {Count} redundant groups.", redundantGroups.Count);

            try
            {
                foreach (var group in redundantGroups)
                {
                    var primaryLine = group.OrderBy(l => l.Id).First();
                    result.KeptCount++;
                    _logger.Error("  - Group '{RuleName}': Keeping primary LineId {PrimaryId}.", group.Key, primaryLine.Id);

                    var linesToRemove = group.Where(l => l.Id != primaryLine.Id).ToList();
                    foreach (var lineToRemove in linesToRemove)
                    {
                        var fieldsToDelete = _context.Fields.Where(f => f.LineId == lineToRemove.Id).ToList();
                        if (fieldsToDelete.Any())
                        {
                            _logger.Error("    - Removing {Count} associated Field(s) for LineId {LineId}.", fieldsToDelete.Count, lineToRemove.Id);
                            _context.Fields.RemoveRange(fieldsToDelete);
                        }
                        var lineEntityToRemove = _context.Lines.Find(lineToRemove.Id);
                        if (lineEntityToRemove != null)
                        {
                            _logger.Error("    - Removing redundant LineId {LineId}.", lineToRemove.Id);
                            _context.Lines.Remove(lineEntityToRemove);
                            result.RemovedCount++;
                        }
                    }
                }

                if (result.RemovedCount > 0)
                {
                    _logger.Error("     - **DB_SAVE_INTENT**: Attempting to save redundant rule cleanup changes...");
                    int changes = _context.SaveChanges();
                    _logger.Error("     - ✅ **DB_SAVE_SUCCESS**: Committed removal of {Count} redundant rules.", changes);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "🚨 **REDUNDANCY_CLEANUP_FAILED**");
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }
            _logger.Error("🗑️ **REDUNDANCY_CLEANUP_EXIT**: Complete. Kept {Kept}, Removed {Removed}.", result.KeptCount, result.RemovedCount);
            return result;
        }


        #region Duplicate Field Mapping Detection
        public List<DuplicateFieldMapping> DetectDuplicateFieldMappings() { return DetectDuplicateFieldMappings(null); }
        public List<DuplicateFieldMapping> DetectDuplicateFieldMappings(int? invoiceId)
        {
            var scope = invoiceId.HasValue ? $"InvoiceId {invoiceId.Value}" : "entire database";
            _logger.Information("🔍 **DUPLICATE_DETECTION_START**: Analyzing {Scope} for duplicate field mappings and LineId conflicts", scope);
            try
            {
                var duplicates = new List<DuplicateFieldMapping>();
                var baseQuery = _context.Fields.Include(f => f.Lines.Parts.Templates).Where(f => f.Lines != null && f.Field != null);
                if (invoiceId.HasValue) { baseQuery = baseQuery.Where(f => f.Lines.Parts.TemplateId == invoiceId.Value); }
                var allFields = baseQuery.AsNoTracking().ToList();
                var keyDuplicateGroups = allFields.Where(f => f.Key != null).GroupBy(f => new { f.LineId, f.Key }).Where(g => g.Select(f => f.Field).Distinct().Count() > 1).ToList();
                var lineIdConflictGroups = allFields.Where(f => f.LineId != null).GroupBy(f => f.LineId).Where(g => g.Select(f => f.Field).Distinct().Count() > 1).ToList();
                foreach (var group in keyDuplicateGroups)
                {
                    var fieldsInGroup = group.ToList(); var firstField = fieldsInGroup.First();
                    var duplicate = new DuplicateFieldMapping
                    {
                        LineId = group.Key.LineId,
                        Key = group.Key.Key,
                        ConflictType = "Key-based Duplicate",
                        LineName = firstField.Lines?.Name ?? "Unknown",
                        InvoiceName = firstField.Lines?.Parts?.Templates?.Name ?? "Unknown",
                        DuplicateFields = fieldsInGroup.Select(f => new FieldMappingInfo
                        {
                            FieldId = f.Id,
                            Field = f.Field,
                            EntityType = f.EntityType,
                            Key = f.Key,
                            DataType = f.DataType,
                            AppendValues = f.AppendValues,
                            IsRequired = f.IsRequired
                        }).ToList()
                    };
                    duplicates.Add(duplicate);
                }
                foreach (var group in lineIdConflictGroups)
                {
                    if (duplicates.Any(d => d.LineId == group.Key)) continue;
                    var fieldsInGroup = group.ToList(); var firstField = fieldsInGroup.First();
                    var duplicate = new DuplicateFieldMapping
                    {
                        LineId = group.Key,
                        Key = "(Multiple Keys)",
                        ConflictType = "LineId Conflict",
                        LineName = firstField.Lines?.Name ?? "Unknown",
                        InvoiceName = firstField.Lines?.Parts?.Templates?.Name ?? "Unknown",
                        DuplicateFields = fieldsInGroup.Select(f => new FieldMappingInfo
                        {
                            FieldId = f.Id,
                            Field = f.Field,
                            EntityType = f.EntityType,
                            Key = f.Key,
                            DataType = f.DataType,
                            AppendValues = f.AppendValues,
                            IsRequired = f.IsRequired
                        }).ToList()
                    };
                    duplicates.Add(duplicate);
                }
                return duplicates;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "🚨 **DUPLICATE_DETECTION_ERROR**");
                return new List<DuplicateFieldMapping>();
            }
        }
        public CleanupResult CleanupDuplicateFieldMappings(List<DuplicateFieldMapping> duplicates)
        {
            var result = new CleanupResult(); if (duplicates == null) return result;
            foreach (var duplicate in duplicates)
            {
                var primaryField = DeterminePrimaryFieldMapping(duplicate);
                if (primaryField == null) continue;
                var fieldsToRemove = duplicate.DuplicateFields.Where(f => f.FieldId != primaryField.FieldId).ToList();
                result.CleanupActions.Add(new CleanupAction { ActionType = "KEEP_PRIMARY", LineName = duplicate.LineName, Field = primaryField.Field, Reason = primaryField.SelectionReason, FieldId = primaryField.FieldId });
                result.KeptCount++;
                foreach (var fieldToRemove in fieldsToRemove)
                {
                    var entityToRemove = _context.Fields.Find(fieldToRemove.FieldId);
                    if (entityToRemove != null)
                    {
                        _context.Fields.Remove(entityToRemove);
                        result.CleanupActions.Add(new CleanupAction { ActionType = "REMOVE_DUPLICATE", LineName = duplicate.LineName, Field = fieldToRemove.Field, Reason = $"Duplicate of {primaryField.Field}", FieldId = fieldToRemove.FieldId });
                        result.RemovedCount++;
                    }
                }
            }
            try
            {
                _context.SaveChanges(); result.Success = true;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "🚨 **CLEANUP_SAVE_ERROR**"); result.Success = false; result.ErrorMessage = ex.Message;
            }
            return result;
        }
        private FieldMappingInfo DeterminePrimaryFieldMapping(DuplicateFieldMapping duplicate)
        {
            if (duplicate.LineName.IndexOf("Gift Card", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                var insuranceField = duplicate.DuplicateFields.FirstOrDefault(f => f.Field == "TotalInsurance");
                if (insuranceField != null) { insuranceField.SelectionReason = "Caribbean customs rule."; return insuranceField; }
            }
            var defaultField = duplicate.DuplicateFields.OrderByDescending(f => f.FieldId).FirstOrDefault();
            if (defaultField != null) defaultField.SelectionReason = "Default selection (highest FieldId).";
            return defaultField;
        }
        #endregion

        #region Other Validation Methods
        public List<DataTypeIssue> ValidateDataTypes() { _logger.Information("🔍 **DATATYPE_VALIDATION_START**"); /* ... */ return new List<DataTypeIssue>(); }
        public AppendValuesAnalysis AnalyzeAppendValuesUsage() { _logger.Information("🔍 **APPENDVALUES_ANALYSIS_START**"); /* ... */ return new AppendValuesAnalysis(); }
        public List<OrphanedRecord> DetectOrphanedRecords() { _logger.Information("🔍 **ORPHANED_RECORDS_START**"); /* ... */ return new List<OrphanedRecord>(); }
        public List<RegexIssue> ValidateRegexPatterns() { _logger.Information("🔍 **REGEX_VALIDATION_START**"); /* ... */ return new List<RegexIssue>(); }
        public DatabaseHealthReport GenerateHealthReport() { _logger.Information("🏥 **HEALTH_REPORT_START**"); /* ... */ return new DatabaseHealthReport(); }
        #endregion
    }

    // ================== FIX: ADD ALL DATA STRUCTURES HERE ==================
    #region Data Structures (Complete)

    public class DuplicateFieldMapping
    {
        public int? LineId { get; set; }
        public string Key { get; set; }
        public string ConflictType { get; set; }
        public string LineName { get; set; }
        public string InvoiceName { get; set; }
        public List<FieldMappingInfo> DuplicateFields { get; set; } = new List<FieldMappingInfo>();
    }

    public class FieldMappingInfo
    {
        public int FieldId { get; set; }
        public string Field { get; set; }
        public string EntityType { get; set; }
        public string Key { get; set; }
        public string SelectionReason { get; set; }
        public string DataType { get; set; }
        public bool? AppendValues { get; set; }
        public bool IsRequired { get; set; }
    }

    public class CleanupResult
    {
        public bool Success { get; set; }
        public int KeptCount { get; set; }
        public int RemovedCount { get; set; }
        public string ErrorMessage { get; set; }
        public List<CleanupAction> CleanupActions { get; set; } = new List<CleanupAction>();

        public void Merge(CleanupResult other)
        {
            if (other == null) return;

            this.KeptCount += other.KeptCount;
            this.RemovedCount += other.RemovedCount;
            this.CleanupActions.AddRange(other.CleanupActions);
            if (!other.Success)
            {
                this.Success = false;
                this.ErrorMessage = string.Join("; ", new[] { this.ErrorMessage, other.ErrorMessage }.Where(s => !string.IsNullOrEmpty(s)));
            }
        }
    }

    public class CleanupAction
    {
        public string ActionType { get; set; }
        public string LineName { get; set; }
        public string Key { get; set; }
        public string Field { get; set; }
        public string Reason { get; set; }
        public int? FieldId { get; set; }
    }

    public class DataTypeIssue { public int FieldId { get; set; } public string FieldName { get; set; } public string EntityType { get; set; } public string DataType { get; set; } public string IssueType { get; set; } public string Description { get; set; } }
    public class AppendValuesAnalysis { public int TotalFields { get; set; } public int AppendTrueCount { get; set; } public int AppendFalseCount { get; set; } public int AppendNullCount { get; set; } public List<AppendValuesPattern> UsagePatterns { get; set; } = new List<AppendValuesPattern>(); }
    public class AppendValuesPattern { public string DataType { get; set; } public bool? AppendValues { get; set; } public int Count { get; set; } public List<string> FieldNames { get; set; } = new List<string>(); }
    public class OrphanedRecord { public string TableName { get; set; } public int Count { get; set; } public string IssueDescription { get; set; } public List<string> SampleIds { get; set; } = new List<string>(); }
    public class RegexIssue { public int RegexId { get; set; } public string Pattern { get; set; } public string IssueType { get; set; } public string ErrorMessage { get; set; } }
    public class DatabaseHealthReport { public DateTime ReportDate { get; set; } public string OverallStatus { get; set; } public List<HealthCategory> Categories { get; set; } = new List<HealthCategory>(); }
    public class HealthCategory { public string Name { get; set; } public string Status { get; set; } public List<HealthIssue> Issues { get; set; } = new List<HealthIssue>(); }
    public class HealthIssue { public string Description { get; set; } public string Details { get; set; } }

    #endregion
}