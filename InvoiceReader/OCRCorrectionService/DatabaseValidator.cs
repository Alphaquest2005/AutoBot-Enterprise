using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Text.RegularExpressions;
using OCR.Business.Entities;
using Serilog;
using TrackableEntities;

namespace InvoiceReader.OCRCorrectionService
{
    /// <summary>
    /// Comprehensive database validator for detecting and fixing OCR database configuration issues.
    /// Addresses user-reported problems with duplicate field mappings and data integrity.
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

        #region Duplicate Field Mapping Detection

        /// <summary>
        /// Detects duplicate field mappings where the same regex key maps to multiple fields.
        /// ENHANCED: Also detects LineId conflicts where same regex pattern maps to different fields through different Keys.
        /// Critical issue: Gift Card regex (LineId 1830) maps to both TotalOtherCost and TotalInsurance through different Keys.
        /// </summary>
        public List<DuplicateFieldMapping> DetectDuplicateFieldMappings()
        {
            return DetectDuplicateFieldMappings(null);
        }

        public List<DuplicateFieldMapping> DetectDuplicateFieldMappings(int? invoiceId)
        {
            var scope = invoiceId.HasValue ? $"InvoiceId {invoiceId.Value}" : "entire database";
            _logger.Information("🔍 **DUPLICATE_DETECTION_START**: Analyzing {Scope} for duplicate field mappings and LineId conflicts", scope);

            try
            {
                var duplicates = new List<DuplicateFieldMapping>();

                // Build base query with optional invoice filtering
                var baseQuery = _context.Fields
                    .Include(f => f.Lines)
                    .Include(f => f.Lines.Parts)
                    .Include(f => f.Lines.Parts.Invoices)
                    .Where(f => f.Lines != null && f.Field != null);

                if (invoiceId.HasValue)
                {
                    baseQuery = baseQuery.Where(f => f.Lines.Parts.TemplateId == invoiceId.Value);
                }

                // Original logic: Find fields with same LineId and Key but different Field names
                var keyDuplicateGroups = baseQuery
                    .Where(f => f.Key != null)
                    .GroupBy(f => new { f.LineId, f.Key })
                    .Where(g => g.Select(f => f.Field).Distinct().Count() > 1) // Multiple different Field values for same LineId+Key
                    .ToList();

                // NEW ENHANCED LOGIC: Find LineId conflicts (same LineId, different Keys, different Fields)
                var lineIdConflictGroups = baseQuery
                    .Where(f => f.LineId != null)
                    .GroupBy(f => f.LineId) // Group by LineId only, not LineId + Key
                    .Where(g => g.Select(f => f.Field).Distinct().Count() > 1) // Multiple different Field targets for same LineId
                    .ToList();

                _logger.Information("🔍 **DETECTION_RESULTS**: Found {KeyDuplicates} Key-based duplicates and {LineIdConflicts} LineId conflicts", 
                    keyDuplicateGroups.Count, lineIdConflictGroups.Count);

                // Process Key-based duplicates (original logic)
                foreach (var group in keyDuplicateGroups)
                {
                    var fieldsInGroup = group.ToList();
                    var firstField = fieldsInGroup.First();
                    
                    var duplicate = new DuplicateFieldMapping
                    {
                        LineId = group.Key.LineId,
                        Key = group.Key.Key,
                        LineName = firstField.Lines?.Name ?? "Unknown",
                        InvoiceName = firstField.Lines?.Parts?.Invoices?.Name ?? "Unknown",
                        DuplicateFields = fieldsInGroup.Select(f => new FieldMappingInfo
                        {
                            FieldId = f.Id,
                            Field = f.Field,
                            EntityType = f.EntityType,
                            DataType = f.DataType,
                            AppendValues = f.AppendValues,
                            IsRequired = f.IsRequired
                        }).ToList()
                    };
                    duplicate.Key += " (Key-based duplicate)"; // Mark type for clarity
                    duplicates.Add(duplicate);

                    _logger.Warning("❌ **KEY_DUPLICATE_DETECTED**: Line '{LineName}' (ID={LineId}) Key='{Key}' maps to {Count} different fields: {Fields}",
                        duplicate.LineName, duplicate.LineId, group.Key.Key, duplicate.DuplicateFields.Count,
                        string.Join(", ", duplicate.DuplicateFields.Select(f => $"{f.Field}({f.EntityType})")));
                }

                // Process LineId conflicts (enhanced logic for Gift Card type issues)
                foreach (var group in lineIdConflictGroups)
                {
                    var fieldsInGroup = group.ToList();
                    var firstField = fieldsInGroup.First();
                    
                    // Skip if this conflict was already captured as a Key-based duplicate
                    var lineIdValue = group.Key;
                    bool alreadyCaptured = duplicates.Any(d => d.LineId == lineIdValue);
                    if (alreadyCaptured)
                    {
                        _logger.Debug("⏭️ **LINEID_CONFLICT_SKIPPED**: LineId={LineId} already captured as Key-based duplicate", lineIdValue);
                        continue;
                    }
                    
                    var duplicate = new DuplicateFieldMapping
                    {
                        LineId = lineIdValue,
                        Key = $"LineId-{lineIdValue} (LineId conflict)", // Mark as LineId conflict
                        LineName = firstField.Lines?.Name ?? "Unknown",
                        InvoiceName = firstField.Lines?.Parts?.Invoices?.Name ?? "Unknown",
                        DuplicateFields = fieldsInGroup.Select(f => new FieldMappingInfo
                        {
                            FieldId = f.Id,
                            Field = f.Field,
                            EntityType = f.EntityType,
                            DataType = f.DataType,
                            AppendValues = f.AppendValues,
                            IsRequired = f.IsRequired
                        }).ToList()
                    };
                    duplicates.Add(duplicate);

                    _logger.Warning("❌ **LINEID_CONFLICT_DETECTED**: Line '{LineName}' (ID={LineId}) has {Count} different field mappings through different Keys: {Fields}",
                        duplicate.LineName, duplicate.LineId, duplicate.DuplicateFields.Count,
                        string.Join(", ", duplicate.DuplicateFields.Select(f => $"{f.Field}({f.EntityType}) via Key '{f.FieldId}'")));
                        
                    // Special logging for Gift Card conflict
                    if (lineIdValue == 1830)
                    {
                        _logger.Error("🎯 **GIFT_CARD_CONFLICT**: LineId 1830 (Gift Card) has {Count} conflicting field mappings - this is the reported issue", duplicate.DuplicateFields.Count);
                        foreach (var field in duplicate.DuplicateFields)
                        {
                            var status = field.Field == "TotalInsurance" ? "✅ CORRECT (Caribbean customs)" : "❌ INCORRECT (should be deleted)";
                            _logger.Error("   📋 **CONFLICT_DETAIL**: FieldId={FieldId}, Field='{Field}', EntityType='{EntityType}' - {Status}",
                                field.FieldId, field.Field, field.EntityType, status);
                        }
                    }
                }

                _logger.Information("🔍 **DUPLICATE_DETECTION_COMPLETE**: Found {Count} total duplicate/conflict groups ({KeyCount} Key-based + {LineIdCount} LineId conflicts)", 
                    duplicates.Count, keyDuplicateGroups.Count, lineIdConflictGroups.Count - duplicates.Count(d => d.Key.Contains("Key-based")));
                return duplicates;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "🚨 **DUPLICATE_DETECTION_ERROR**: Exception during duplicate field mapping detection");
                return new List<DuplicateFieldMapping>();
            }
        }

        /// <summary>
        /// Cleans up duplicate field mappings by keeping the most appropriate mapping and removing others.
        /// </summary>
        public CleanupResult CleanupDuplicateFieldMappings(List<DuplicateFieldMapping> duplicates)
        {
            _logger.Information("🔧 **CLEANUP_START**: Starting cleanup of {Count} duplicate field mapping groups", duplicates.Count);

            var result = new CleanupResult();

            try
            {
                foreach (var duplicate in duplicates)
                {
                    _logger.Information("🔧 **CLEANUP_GROUP**: Processing duplicate group - Line='{LineName}', Key='{Key}', Fields={Count}",
                        duplicate.LineName, duplicate.Key, duplicate.DuplicateFields.Count);

                    // Determine which field mapping to keep based on priority rules
                    var primaryField = DeterminePrimaryFieldMapping(duplicate);
                    var fieldsToRemove = duplicate.DuplicateFields.Where(f => f.FieldId != primaryField.FieldId).ToList();

                    _logger.Information("✅ **CLEANUP_PRIMARY**: Keeping primary field mapping - Field='{Field}', EntityType='{EntityType}', Reason='{Reason}'",
                        primaryField.Field, primaryField.EntityType, primaryField.SelectionReason);

                    result.CleanupActions.Add(new CleanupAction
                    {
                        ActionType = "KEEP_PRIMARY",
                        LineName = duplicate.LineName,
                        Key = duplicate.Key,
                        Field = primaryField.Field,
                        Reason = primaryField.SelectionReason,
                        FieldId = primaryField.FieldId
                    });
                    result.KeptCount++;

                    // Remove duplicate fields
                    foreach (var fieldToRemove in fieldsToRemove)
                    {
                        try
                        {
                            var entityToRemove = _context.Fields.FirstOrDefault(f => f.Id == fieldToRemove.FieldId);
                            if (entityToRemove != null)
                            {
                                _context.Fields.Remove(entityToRemove);
                                
                                _logger.Warning("🗑️ **CLEANUP_REMOVE**: Removing duplicate field mapping - Field='{Field}', EntityType='{EntityType}'",
                                    fieldToRemove.Field, fieldToRemove.EntityType);

                                result.CleanupActions.Add(new CleanupAction
                                {
                                    ActionType = "REMOVE_DUPLICATE",
                                    LineName = duplicate.LineName,
                                    Key = duplicate.Key,
                                    Field = fieldToRemove.Field,
                                    Reason = $"Duplicate of {primaryField.Field}",
                                    FieldId = fieldToRemove.FieldId
                                });
                                result.RemovedCount++;
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.Error(ex, "🚨 **CLEANUP_REMOVE_ERROR**: Failed to remove duplicate field ID={FieldId}", fieldToRemove.FieldId);
                        }
                    }
                }

                // Save changes with enhanced verification
                var changesSaved = _context.SaveChanges();
                _logger.Information("💾 **CLEANUP_SAVE**: Saved {ChangeCount} database changes", changesSaved);

                // Verify that the expected number of changes were actually saved
                if (changesSaved != result.RemovedCount)
                {
                    _logger.Warning("⚠️ **CLEANUP_SAVE_MISMATCH**: Expected to save {ExpectedChanges} deletions, but SaveChanges() returned {ActualChanges}", 
                        result.RemovedCount, changesSaved);
                }

                // Additional verification: try to reload deleted entities to confirm they're gone
                var verificationFailed = false;
                foreach (var action in result.CleanupActions.Where(a => a.ActionType == "REMOVE_DUPLICATE" && a.FieldId.HasValue))
                {
                    try
                    {
                        // Try to reload the deleted entity - it should not exist
                        var deletedEntity = _context.Fields.FirstOrDefault(f => f.Id == action.FieldId.Value);
                        if (deletedEntity != null)
                        {
                            _logger.Error("🚨 **CLEANUP_VERIFICATION_FAILED**: Deleted entity FieldId={FieldId} still exists in database", 
                                action.FieldId.Value);
                            verificationFailed = true;
                        }
                        else
                        {
                            _logger.Debug("✅ **CLEANUP_VERIFICATION_SUCCESS**: Confirmed deletion of FieldId={FieldId} (Field='{Field}')", 
                                action.FieldId.Value, action.Field);
                        }
                    }
                    catch (Exception verifyEx)
                    {
                        _logger.Warning(verifyEx, "⚠️ **CLEANUP_VERIFICATION_ERROR**: Could not verify deletion of FieldId={FieldId}", 
                            action.FieldId.Value);
                    }
                }

                if (verificationFailed)
                {
                    _logger.Error("🚨 **CLEANUP_VERIFICATION_FAILED**: Some deleted entities could still be found in database");
                    result.Success = false;
                    result.ErrorMessage = "Database verification failed - some entities may not have been deleted";
                }
                else
                {
                    result.Success = true;
                    _logger.Information("✅ **CLEANUP_COMPLETE**: Successfully cleaned up {RemovedCount} duplicate mappings, kept {KeptCount} primary mappings",
                        result.RemovedCount, result.KeptCount);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "🚨 **CLEANUP_ERROR**: Exception during duplicate field mapping cleanup");
                result.Success = false;
                result.ErrorMessage = ex.Message;
            }

            return result;
        }

        private FieldMappingInfo DeterminePrimaryFieldMapping(DuplicateFieldMapping duplicate)
        {
            // Priority rules for determining which field mapping to keep:
            // 1. Caribbean customs compliance (TotalInsurance for Gift Card customer reductions)
            // 2. Required fields take priority
            // 3. More specific EntityType (ShipmentInvoice > Invoice)
            // 4. Newer creation date if available

            var candidates = duplicate.DuplicateFields.ToList();

            // Rule 1: Caribbean customs compliance for Gift Card
            if (duplicate.Key.IndexOf("GiftCard", StringComparison.OrdinalIgnoreCase) >= 0 ||
                duplicate.Key.IndexOf("TotalInsurance", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                var insuranceField = candidates.FirstOrDefault(f => f.Field == "TotalInsurance");
                if (insuranceField != null)
                {
                    insuranceField.SelectionReason = "Caribbean customs compliance - Gift Card maps to TotalInsurance for customer reductions";
                    return insuranceField;
                }
            }

            // Rule 2: Required fields take priority
            var requiredField = candidates.FirstOrDefault(f => f.IsRequired);
            if (requiredField != null)
            {
                requiredField.SelectionReason = "Required field takes priority";
                return requiredField;
            }

            // Rule 3: More specific EntityType
            var shipmentInvoiceField = candidates.FirstOrDefault(f => f.EntityType == "ShipmentInvoice");
            if (shipmentInvoiceField != null)
            {
                shipmentInvoiceField.SelectionReason = "ShipmentInvoice EntityType is more specific than Invoice";
                return shipmentInvoiceField;
            }

            // Rule 4: Default to first field with reason
            var defaultField = candidates.First();
            defaultField.SelectionReason = "Default selection (first in list)";
            return defaultField;
        }

        #endregion

        #region Data Type Validation

        /// <summary>
        /// Validates DataType field values against supported pseudo datatypes.
        /// User feedback: System uses pseudo datatypes like "Number", "English Date", not standard .NET types.
        /// </summary>
        public List<DataTypeIssue> ValidateDataTypes()
        {
            _logger.Information("🔍 **DATATYPE_VALIDATION_START**: Analyzing DataType field values");

            var issues = new List<DataTypeIssue>();

            try
            {
                // Define supported pseudo datatypes based on ImportByDataType.cs analysis
                var supportedPseudoTypes = new HashSet<string>(StringComparer.OrdinalIgnoreCase)
                {
                    "String",
                    "Number", 
                    "Numeric",
                    "Date",
                    "English Date"
                };

                var allFields = _context.Fields
                    .Where(f => f.DataType != null)
                    .ToList();

                _logger.Information("📊 **DATATYPE_ANALYSIS**: Analyzing {FieldCount} fields with DataType values", allFields.Count);

                // Group by DataType to analyze usage patterns
                var dataTypeGroups = allFields
                    .GroupBy(f => f.DataType)
                    .OrderByDescending(g => g.Count())
                    .ToList();

                foreach (var group in dataTypeGroups)
                {
                    var dataType = group.Key;
                    var count = group.Count();
                    var isSupported = supportedPseudoTypes.Contains(dataType);

                    _logger.Information("📊 **DATATYPE_USAGE**: DataType='{DataType}', Count={Count}, Supported={IsSupported}",
                        dataType, count, isSupported);

                    if (!isSupported)
                    {
                        foreach (var field in group.Take(5)) // Sample first 5 fields
                        {
                            issues.Add(new DataTypeIssue
                            {
                                FieldId = field.Id,
                                FieldName = field.Field,
                                EntityType = field.EntityType,
                                DataType = dataType,
                                IssueType = "UnsupportedDataType",
                                Description = $"DataType '{dataType}' is not in the list of supported pseudo datatypes"
                            });
                        }

                        if (group.Count() > 5)
                        {
                            _logger.Information("  📋 **DATATYPE_SAMPLE**: Logged 5 sample fields, {More} more fields have this unsupported DataType", group.Count() - 5);
                        }
                    }
                }

                // Check for empty or null DataType values
                var emptyDataTypeCount = _context.Fields.Count(f => string.IsNullOrEmpty(f.DataType));
                if (emptyDataTypeCount > 0)
                {
                    _logger.Warning("⚠️ **DATATYPE_EMPTY**: Found {Count} fields with empty or null DataType", emptyDataTypeCount);
                    
                    issues.Add(new DataTypeIssue
                    {
                        IssueType = "EmptyDataType",
                        Description = $"{emptyDataTypeCount} fields have empty or null DataType values"
                    });
                }

                _logger.Information("🔍 **DATATYPE_VALIDATION_COMPLETE**: Found {IssueCount} datatype issues across {FieldCount} fields",
                    issues.Count, allFields.Count);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "🚨 **DATATYPE_VALIDATION_ERROR**: Exception during datatype validation");
            }

            return issues;
        }

        #endregion

        #region AppendValues Analysis

        /// <summary>
        /// Analyzes AppendValues column usage to understand aggregation vs replacement patterns.
        /// Critical for proper importation as identified by user.
        /// </summary>
        public AppendValuesAnalysis AnalyzeAppendValuesUsage()
        {
            _logger.Information("🔍 **APPENDVALUES_ANALYSIS_START**: Analyzing AppendValues column usage patterns");

            try
            {
                var allFields = _context.Fields
                    .Where(f => f.DataType != null)
                    .ToList();

                var analysis = new AppendValuesAnalysis
                {
                    TotalFields = allFields.Count,
                    AppendTrueCount = allFields.Count(f => f.AppendValues == true),
                    AppendFalseCount = allFields.Count(f => f.AppendValues == false),
                    AppendNullCount = allFields.Count(f => f.AppendValues == null)
                };

                _logger.Information("📊 **APPENDVALUES_SUMMARY**: Total={Total}, True={True}, False={False}, Null={Null}",
                    analysis.TotalFields, analysis.AppendTrueCount, analysis.AppendFalseCount, analysis.AppendNullCount);

                // Analyze patterns by DataType and AppendValues combination
                var patterns = allFields
                    .GroupBy(f => new { f.DataType, f.AppendValues })
                    .Select(g => new AppendValuesPattern
                    {
                        DataType = g.Key.DataType,
                        AppendValues = g.Key.AppendValues,
                        Count = g.Count(),
                        FieldNames = g.Select(f => f.Field).Distinct().ToList()
                    })
                    .OrderBy(p => p.DataType)
                    .ThenBy(p => p.AppendValues)
                    .ToList();

                analysis.UsagePatterns = patterns;

                // Analyze business logic implications
                foreach (var pattern in patterns)
                {
                    string businessLogic = GetAppendValuesBusinessLogic(pattern.DataType, pattern.AppendValues);
                    
                    _logger.Information("💡 **APPENDVALUES_BUSINESS_LOGIC**: DataType='{DataType}', AppendValues={AppendValues}, Logic='{Logic}', Count={Count}",
                        pattern.DataType, pattern.AppendValues, businessLogic, pattern.Count);
                }

                // Special analysis for numeric fields (most critical for understanding)
                var numericPatterns = patterns.Where(p => p.DataType == "Number" || p.DataType == "Numeric").ToList();
                foreach (var numericPattern in numericPatterns)
                {
                    var action = numericPattern.AppendValues == true ? "ADD/SUM VALUES" : "REPLACE VALUES";
                    _logger.Information("🔢 **NUMERIC_APPENDVALUES**: {Action} - {Count} fields will {Action} during import",
                        action, numericPattern.Count, action.ToLower());
                }

                _logger.Information("✅ **APPENDVALUES_ANALYSIS_COMPLETE**: Analyzed {PatternCount} usage patterns", patterns.Count);
                return analysis;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "🚨 **APPENDVALUES_ANALYSIS_ERROR**: Exception during AppendValues analysis");
                return new AppendValuesAnalysis { TotalFields = 0 };
            }
        }

        private string GetAppendValuesBusinessLogic(string dataType, bool? appendValues)
        {
            return dataType switch
            {
                "String" => "Concatenate with space separator",
                "Number" or "Numeric" when appendValues == true => "Add/sum values together (aggregation)",
                "Number" or "Numeric" when appendValues == false => "Replace with new value (overwrite)",
                "Number" or "Numeric" when appendValues == null => "Replace with new value (default behavior)",
                "Date" or "English Date" => "Replace with new value (dates don't aggregate)",
                _ => "Replace with new value (default behavior)"
            };
        }

        #endregion

        #region Orphaned Records Detection

        /// <summary>
        /// Detects orphaned records and broken references in the OCR database.
        /// </summary>
        public List<OrphanedRecord> DetectOrphanedRecords()
        {
            _logger.Information("🔍 **ORPHANED_RECORDS_START**: Detecting orphaned records and broken references");

            var orphanedRecords = new List<OrphanedRecord>();

            try
            {
                // Fields without Lines
                var fieldsWithoutLines = _context.Fields
                    .Where(f => f.LineId != null && !_context.Lines.Any(l => l.Id == f.LineId))
                    .Take(10)
                    .ToList();

                if (fieldsWithoutLines.Any())
                {
                    orphanedRecords.Add(new OrphanedRecord
                    {
                        TableName = "Fields",
                        Count = fieldsWithoutLines.Count,
                        IssueDescription = "Fields reference non-existent Lines",
                        SampleIds = fieldsWithoutLines.Select(f => f.Id.ToString()).ToList()
                    });
                }

                // Lines without Parts
                var linesWithoutParts = _context.Lines
                    .Where(l => l.PartId != null && !_context.Parts.Any(p => p.Id == l.PartId))
                    .Take(10)
                    .ToList();

                if (linesWithoutParts.Any())
                {
                    orphanedRecords.Add(new OrphanedRecord
                    {
                        TableName = "Lines",
                        Count = linesWithoutParts.Count,
                        IssueDescription = "Lines reference non-existent Parts",
                        SampleIds = linesWithoutParts.Select(l => l.Id.ToString()).ToList()
                    });
                }

                // Parts without Invoices
                var partsWithoutInvoices = _context.Parts
                    .Where(p => p.TemplateId != null && !_context.Invoices.Any(i => i.Id == p.TemplateId))
                    .Take(10)
                    .ToList();

                if (partsWithoutInvoices.Any())
                {
                    orphanedRecords.Add(new OrphanedRecord
                    {
                        TableName = "Parts",
                        Count = partsWithoutInvoices.Count(),
                        IssueDescription = "Parts reference non-existent Invoices",
                        SampleIds = partsWithoutInvoices.Select(p => p.Id.ToString()).ToList()
                    });
                }

                _logger.Information("🔍 **ORPHANED_RECORDS_COMPLETE**: Found {Count} types of orphaned records", orphanedRecords.Count);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "🚨 **ORPHANED_RECORDS_ERROR**: Exception during orphaned records detection");
            }

            return orphanedRecords;
        }

        #endregion

        #region Regex Pattern Validation

        /// <summary>
        /// Validates regex patterns for compilation errors and syntax issues.
        /// </summary>
        public List<RegexIssue> ValidateRegexPatterns()
        {
            _logger.Information("🔍 **REGEX_VALIDATION_START**: Validating regex patterns for compilation errors");

            var issues = new List<RegexIssue>();

            try
            {
                var regexPatterns = _context.RegularExpressions.ToList();
                _logger.Information("📊 **REGEX_VALIDATION**: Analyzing {Count} regex patterns", regexPatterns.Count);

                foreach (var regexEntity in regexPatterns)
                {
                    if (string.IsNullOrEmpty(regexEntity.RegEx))
                    {
                        issues.Add(new RegexIssue
                        {
                            RegexId = regexEntity.Id,
                            Pattern = regexEntity.RegEx,
                            IssueType = "EmptyPattern",
                            ErrorMessage = "Regex pattern is null or empty"
                        });
                        continue;
                    }

                    try
                    {
                        // Attempt to compile the regex
                        _ = new Regex(regexEntity.RegEx, RegexOptions.Compiled);
                    }
                    catch (ArgumentException ex)
                    {
                        issues.Add(new RegexIssue
                        {
                            RegexId = regexEntity.Id,
                            Pattern = regexEntity.RegEx,
                            IssueType = "CompilationError",
                            ErrorMessage = ex.Message
                        });
                    }
                }

                _logger.Information("🔍 **REGEX_VALIDATION_COMPLETE**: Found {IssueCount} regex issues", issues.Count);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "🚨 **REGEX_VALIDATION_ERROR**: Exception during regex pattern validation");
            }

            return issues;
        }

        #endregion

        #region Database Health Report

        /// <summary>
        /// Generates a comprehensive health report of the OCR database.
        /// </summary>
        public DatabaseHealthReport GenerateHealthReport()
        {
            _logger.Information("🏥 **HEALTH_REPORT_START**: Generating comprehensive OCR database health report");

            var report = new DatabaseHealthReport
            {
                ReportDate = DateTime.UtcNow,
                Categories = new List<HealthCategory>()
            };

            try
            {
                // Category: Duplicate Field Mappings
                var duplicateMappings = DetectDuplicateFieldMappings();
                report.Categories.Add(new HealthCategory
                {
                    Name = "Duplicate Field Mappings",
                    Status = duplicateMappings.Any() ? "FAIL" : "PASS",
                    Issues = duplicateMappings.Select(d => new HealthIssue
                    {
                        Description = $"Line '{d.LineName}' Key='{d.Key}' maps to multiple fields: {string.Join(", ", d.DuplicateFields.Select(f => f.Field))}",
                        Details = $"LineId: {d.LineId}, Fields: {d.DuplicateFields.Count}"
                    }).ToList()
                });

                // Category: Data Type Validation
                var dataTypeIssues = ValidateDataTypes();
                report.Categories.Add(new HealthCategory
                {
                    Name = "Data Type Validation",
                    Status = dataTypeIssues.Any() ? "FAIL" : "PASS",
                    Issues = dataTypeIssues.Select(i => new HealthIssue
                    {
                        Description = i.Description,
                        Details = $"IssueType: {i.IssueType}, FieldId: {i.FieldId}"
                    }).ToList()
                });

                // Category: Orphaned Records
                var orphanedRecords = DetectOrphanedRecords();
                report.Categories.Add(new HealthCategory
                {
                    Name = "Orphaned Records",
                    Status = orphanedRecords.Any() ? "FAIL" : "PASS",
                    Issues = orphanedRecords.Select(o => new HealthIssue
                    {
                        Description = o.IssueDescription,
                        Details = $"Table: {o.TableName}, Count: {o.Count}"
                    }).ToList()
                });

                // Category: Regex Pattern Validation
                var regexIssues = ValidateRegexPatterns();
                report.Categories.Add(new HealthCategory
                {
                    Name = "Regex Pattern Validation",
                    Status = regexIssues.Any() ? "FAIL" : "PASS",
                    Issues = regexIssues.Select(r => new HealthIssue
                    {
                        Description = r.ErrorMessage,
                        Details = $"RegexId: {r.RegexId}, Pattern: {r.Pattern}"
                    }).ToList()
                });

                report.OverallStatus = report.Categories.All(c => c.Status == "PASS") ? "PASS" : "FAIL";
                _logger.Information("🏥 **HEALTH_REPORT_COMPLETE**: Overall Status = {Status}", report.OverallStatus);
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "🚨 **HEALTH_REPORT_ERROR**: Exception during health report generation");
                report.OverallStatus = "ERROR";
            }

            return report;
        }

        #endregion
    }

    #region Data Structures

    public class DuplicateFieldMapping
    {
        public int? LineId { get; set; }
        public string Key { get; set; }
        public string LineName { get; set; }
        public string InvoiceName { get; set; }
        public List<FieldMappingInfo> DuplicateFields { get; set; } = new List<FieldMappingInfo>();
    }

    public class FieldMappingInfo
    {
        public int FieldId { get; set; }
        public string Field { get; set; }
        public string EntityType { get; set; }
        public string DataType { get; set; }
        public bool? AppendValues { get; set; }
        public bool IsRequired { get; set; }
        public string SelectionReason { get; set; }
    }

    public class CleanupResult
    {
        public bool Success { get; set; }
        public int KeptCount { get; set; }
        public int RemovedCount { get; set; }
        public string ErrorMessage { get; set; }
        public List<CleanupAction> CleanupActions { get; set; } = new List<CleanupAction>();
    }

    public class CleanupAction
    {
        public string ActionType { get; set; }
        public string LineName { get; set; }
        public string Key { get; set; }
        public string Field { get; set; }
        public string Reason { get; set; }
        public int? FieldId { get; set; }  // Added to track specific field IDs for verification
    }

    public class DataTypeIssue
    {
        public int FieldId { get; set; }
        public string FieldName { get; set; }
        public string EntityType { get; set; }
        public string DataType { get; set; }
        public string IssueType { get; set; }
        public string Description { get; set; }
    }

    public class AppendValuesAnalysis
    {
        public int TotalFields { get; set; }
        public int AppendTrueCount { get; set; }
        public int AppendFalseCount { get; set; }
        public int AppendNullCount { get; set; }
        public List<AppendValuesPattern> UsagePatterns { get; set; } = new List<AppendValuesPattern>();
    }

    public class AppendValuesPattern
    {
        public string DataType { get; set; }
        public bool? AppendValues { get; set; }
        public int Count { get; set; }
        public List<string> FieldNames { get; set; } = new List<string>();
    }

    public class OrphanedRecord
    {
        public string TableName { get; set; }
        public int Count { get; set; }
        public string IssueDescription { get; set; }
        public List<string> SampleIds { get; set; } = new List<string>();
    }

    public class RegexIssue
    {
        public int RegexId { get; set; }
        public string Pattern { get; set; }
        public string IssueType { get; set; }
        public string ErrorMessage { get; set; }
    }

    public class DatabaseHealthReport
    {
        public DateTime ReportDate { get; set; }
        public string OverallStatus { get; set; }
        public List<HealthCategory> Categories { get; set; } = new List<HealthCategory>();
    }

    public class HealthCategory
    {
        public string Name { get; set; }
        public string Status { get; set; }
        public List<HealthIssue> Issues { get; set; } = new List<HealthIssue>();
    }

    public class HealthIssue
    {
        public string Description { get; set; }
        public string Details { get; set; }
    }

    #endregion
}