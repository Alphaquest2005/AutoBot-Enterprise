// File: OCRCorrectionService/Services/AuditTrailService.cs
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.Json;
using System.Text.Json.Serialization;
using System.Threading.Tasks;
using WaterNut.DataSpace.AutoImplementation;
using Serilog;

namespace WaterNut.DataSpace.Services
{
    /// <summary>
    /// Service for creating and managing audit trails of template modifications.
    /// Provides comprehensive tracking of all auto-implementation operations with
    /// detailed change history, rollback capabilities, and analytics.
    /// CRITICAL: All audit files stored within OCRCorrectionService directory structure.
    /// </summary>
    public class AuditTrailService : IDisposable
    {
        private readonly ILogger _logger;
        private readonly string _auditTrailPath;
        private readonly JsonSerializerOptions _jsonOptions;
        private readonly object _fileLock = new object();

        public AuditTrailService(ILogger logger, string auditTrailPath = null)
        {
            _logger = logger ?? throw new ArgumentNullException(nameof(logger));
            _auditTrailPath = auditTrailPath ?? GetDefaultAuditPath();

            _jsonOptions = new JsonSerializerOptions
            {
                WriteIndented = true,
                PropertyNamingPolicy = JsonNamingPolicy.CamelCase,
                DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull,
                Converters = { new JsonStringEnumConverter() }
            };

            EnsureAuditDirectoryExists();

            _logger.Information("📋 **AUDIT_TRAIL_SERVICE_INIT**: Service initialized with audit path '{AuditPath}'", _auditTrailPath);
        }

        #region Core Audit Methods

        /// <summary>
        /// Creates a comprehensive audit entry for template modification operations.
        /// Includes detailed change tracking, user context, and rollback information.
        /// </summary>
        public async Task<AuditEntryResult> CreateAuditEntryAsync(AuditTrailEntry auditEntry)
        {
            _logger.Information("📋 **CREATING_AUDIT_ENTRY**: Template='{TemplateName}', Action='{Action}', ExecutionId='{ExecutionId}'",
                auditEntry.TemplateName, auditEntry.Action, auditEntry.ExecutionId);

            var result = new AuditEntryResult
            {
                AuditEntryId = auditEntry.Id ?? Guid.NewGuid().ToString(),
                TemplateName = auditEntry.TemplateName,
                Timestamp = DateTime.UtcNow
            };

            try
            {
                // Enrich audit entry with additional metadata
                auditEntry.Id = result.AuditEntryId;
                auditEntry.AuditVersion = "1.0";
                auditEntry.SystemInfo = await CollectSystemInfoAsync();
                auditEntry.EnvironmentInfo = CollectEnvironmentInfo();

                // Validate audit entry
                var validationResult = ValidateAuditEntry(auditEntry);
                if (!validationResult.IsValid)
                {
                    result.Success = false;
                    result.ErrorMessage = validationResult.ErrorMessage;
                    return result;
                }

                // Write to individual audit file
                var individualFilePath = await WriteIndividualAuditFileAsync(auditEntry);
                result.AuditFilePath = individualFilePath;

                // Append to consolidated audit log
                await AppendToConsolidatedAuditLogAsync(auditEntry);

                // Update audit statistics
                await UpdateAuditStatisticsAsync(auditEntry);

                // Create searchable index entry
                await CreateSearchableIndexEntryAsync(auditEntry);

                result.Success = true;
                result.FileSize = new FileInfo(individualFilePath).Length;

                _logger.Information("✅ **AUDIT_ENTRY_CREATED**: Successfully created audit entry '{AuditId}' for '{TemplateName}'",
                    result.AuditEntryId, auditEntry.TemplateName);

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **AUDIT_ENTRY_ERROR**: Failed to create audit entry for '{TemplateName}'",
                    auditEntry.TemplateName);

                result.Success = false;
                result.ErrorMessage = $"Audit entry creation failed: {ex.Message}";
                return result;
            }
        }

        /// <summary>
        /// Retrieves audit history for a specific template with filtering and pagination.
        /// </summary>
        public async Task<AuditHistoryResult> GetTemplateAuditHistoryAsync(
            string templateName, 
            AuditHistoryFilter filter = null)
        {
            _logger.Information("📋 **RETRIEVING_AUDIT_HISTORY**: Template='{TemplateName}'", templateName);

            var result = new AuditHistoryResult
            {
                TemplateName = templateName,
                RequestTimestamp = DateTime.UtcNow
            };

            try
            {
                filter = filter ?? new AuditHistoryFilter();

                // Load audit entries from consolidated log
                var allEntries = await LoadConsolidatedAuditLogAsync();
                
                // Filter by template name
                var templateEntries = allEntries
                    .Where(e => string.Equals(e.TemplateName, templateName, StringComparison.OrdinalIgnoreCase))
                    .ToList();

                // Apply filters
                if (filter.StartDate.HasValue)
                {
                    templateEntries = templateEntries.Where(e => e.Timestamp >= filter.StartDate.Value).ToList();
                }

                if (filter.EndDate.HasValue)
                {
                    templateEntries = templateEntries.Where(e => e.Timestamp <= filter.EndDate.Value).ToList();
                }

                if (!string.IsNullOrEmpty(filter.ActionType))
                {
                    templateEntries = templateEntries.Where(e => 
                        string.Equals(e.Action, filter.ActionType, StringComparison.OrdinalIgnoreCase)).ToList();
                }

                if (filter.SuccessOnly.HasValue)
                {
                    templateEntries = templateEntries.Where(e => e.Success == filter.SuccessOnly.Value).ToList();
                }

                // Sort by timestamp (newest first)
                templateEntries = templateEntries.OrderByDescending(e => e.Timestamp).ToList();

                // Apply pagination
                var totalEntries = templateEntries.Count;
                if (filter.PageSize > 0)
                {
                    var skip = Math.Max(0, filter.PageNumber) * filter.PageSize;
                    templateEntries = templateEntries.Skip(skip).Take(filter.PageSize).ToList();
                }

                result.AuditEntries = templateEntries;
                result.TotalEntries = totalEntries;
                result.FilteredEntries = templateEntries.Count;
                result.Success = true;

                _logger.Information("✅ **AUDIT_HISTORY_RETRIEVED**: Found {TotalEntries} total, {FilteredEntries} filtered entries for '{TemplateName}'",
                    totalEntries, templateEntries.Count, templateName);

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **AUDIT_HISTORY_ERROR**: Failed to retrieve audit history for '{TemplateName}'", templateName);

                result.Success = false;
                result.ErrorMessage = $"Failed to retrieve audit history: {ex.Message}";
                return result;
            }
        }

        /// <summary>
        /// Generates comprehensive audit analytics and reporting.
        /// </summary>
        public async Task<AuditAnalyticsResult> GenerateAuditAnalyticsAsync(AuditAnalyticsRequest request = null)
        {
            _logger.Information("📊 **GENERATING_AUDIT_ANALYTICS**: Analyzing audit trail data");

            var result = new AuditAnalyticsResult
            {
                GeneratedAt = DateTime.UtcNow,
                AnalysisPeriod = request?.AnalysisPeriod ?? TimeSpan.FromDays(30)
            };

            try
            {
                // Load all audit entries
                var allEntries = await LoadConsolidatedAuditLogAsync();

                // Filter by analysis period
                var cutoffDate = DateTime.UtcNow - result.AnalysisPeriod;
                var analysisEntries = allEntries.Where(e => e.Timestamp >= cutoffDate).ToList();

                // Overall statistics
                result.TotalOperations = analysisEntries.Count;
                result.SuccessfulOperations = analysisEntries.Count(e => e.Success);
                result.FailedOperations = analysisEntries.Count(e => !e.Success);
                result.SuccessRate = result.TotalOperations > 0 ? 
                    (double)result.SuccessfulOperations / result.TotalOperations : 0.0;

                // Template-specific analytics
                result.TemplateStatistics = analysisEntries
                    .GroupBy(e => e.TemplateName)
                    .Select(g => new TemplateAuditStatistics
                    {
                        TemplateName = g.Key,
                        TotalOperations = g.Count(),
                        SuccessfulOperations = g.Count(e => e.Success),
                        FailedOperations = g.Count(e => !e.Success),
                        LastModified = g.Max(e => e.Timestamp),
                        AverageRecommendationsPerOperation = g.Where(e => e.RecommendationsCount > 0)
                            .DefaultIfEmpty()
                            .Average(e => e?.RecommendationsCount ?? 0)
                    })
                    .OrderByDescending(t => t.TotalOperations)
                    .ToList();

                // Action type analytics
                result.ActionTypeStatistics = analysisEntries
                    .GroupBy(e => e.Action)
                    .Select(g => new ActionTypeStatistics
                    {
                        ActionType = g.Key,
                        TotalCount = g.Count(),
                        SuccessCount = g.Count(e => e.Success),
                        FailureCount = g.Count(e => !e.Success),
                        AverageExecutionTime = g.Where(e => e.ExecutionDuration.HasValue)
                            .DefaultIfEmpty()
                            .Average(e => e?.ExecutionDuration?.TotalSeconds ?? 0)
                    })
                    .OrderByDescending(a => a.TotalCount)
                    .ToList();

                // Timeline analysis
                result.TimelineData = analysisEntries
                    .GroupBy(e => e.Timestamp.Date)
                    .Select(g => new DailyAuditStatistics
                    {
                        Date = g.Key,
                        TotalOperations = g.Count(),
                        SuccessfulOperations = g.Count(e => e.Success),
                        UniqueTemplatesModified = g.Select(e => e.TemplateName).Distinct().Count()
                    })
                    .OrderBy(d => d.Date)
                    .ToList();

                // Error analysis
                result.ErrorAnalysis = analysisEntries
                    .Where(e => !e.Success && !string.IsNullOrEmpty(e.ErrorMessage))
                    .GroupBy(e => ExtractErrorCategory(e.ErrorMessage))
                    .Select(g => new ErrorCategoryStatistics
                    {
                        ErrorCategory = g.Key,
                        Count = g.Count(),
                        AffectedTemplates = g.Select(e => e.TemplateName).Distinct().ToList(),
                        RecentOccurrences = g.OrderByDescending(e => e.Timestamp).Take(3)
                            .Select(e => new { e.TemplateName, e.Timestamp, e.ErrorMessage }).ToList()
                    })
                    .OrderByDescending(e => e.Count)
                    .ToList();

                result.Success = true;

                _logger.Information("✅ **AUDIT_ANALYTICS_GENERATED**: Analyzed {TotalOperations} operations across {TemplateCount} templates",
                    result.TotalOperations, result.TemplateStatistics.Count);

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **AUDIT_ANALYTICS_ERROR**: Failed to generate audit analytics");

                result.Success = false;
                result.ErrorMessage = $"Analytics generation failed: {ex.Message}";
                return result;
            }
        }

        /// <summary>
        /// Purges old audit entries based on retention policy.
        /// </summary>
        public async Task<AuditPurgeResult> PurgeOldAuditEntriesAsync(AuditRetentionPolicy retentionPolicy = null)
        {
            _logger.Information("🗑️ **PURGING_OLD_AUDIT_ENTRIES**: Starting audit trail cleanup");

            retentionPolicy = retentionPolicy ?? new AuditRetentionPolicy();

            var result = new AuditPurgeResult
            {
                StartTime = DateTime.UtcNow,
                RetentionPolicy = retentionPolicy
            };

            try
            {
                var cutoffDate = DateTime.UtcNow - retentionPolicy.RetentionPeriod;

                // Load current audit entries
                var allEntries = await LoadConsolidatedAuditLogAsync();
                var entriesToKeep = allEntries.Where(e => e.Timestamp >= cutoffDate).ToList();
                var entriesToPurge = allEntries.Where(e => e.Timestamp < cutoffDate).ToList();

                // Apply minimum entry count policy
                if (allEntries.Count - entriesToPurge.Count < retentionPolicy.MinimumEntriesToKeep)
                {
                    var excessPurgeCount = retentionPolicy.MinimumEntriesToKeep - (allEntries.Count - entriesToPurge.Count);
                    var oldestToPurge = entriesToPurge.OrderBy(e => e.Timestamp).Take(entriesToPurge.Count - excessPurgeCount).ToList();
                    entriesToPurge = oldestToPurge;
                    entriesToKeep.AddRange(allEntries.Except(entriesToKeep).Except(entriesToPurge));
                }

                result.TotalEntriesBeforePurge = allEntries.Count;
                result.EntriesPurged = entriesToPurge.Count;
                result.EntriesRetained = entriesToKeep.Count;

                if (entriesToPurge.Any())
                {
                    // Create backup before purging
                    if (retentionPolicy.CreateBackupBeforePurge)
                    {
                        await CreatePurgeBackupAsync(entriesToPurge);
                        result.BackupCreated = true;
                    }

                    // Rewrite consolidated log with retained entries
                    await WriteConsolidatedAuditLogAsync(entriesToKeep);

                    // Remove individual audit files for purged entries
                    foreach (var entry in entriesToPurge)
                    {
                        try
                        {
                            var filePath = GetIndividualAuditFilePath(entry);
                            if (File.Exists(filePath))
                            {
                                File.Delete(filePath);
                                result.FilesDeleted++;
                            }
                        }
                        catch (Exception ex)
                        {
                            _logger.Warning("⚠️ **PURGE_FILE_DELETE_WARNING**: Could not delete audit file for entry '{EntryId}': {Error}",
                                entry.Id, ex.Message);
                        }
                    }

                    // Update statistics
                    await UpdateAuditStatisticsAfterPurgeAsync(result);
                }

                result.Success = true;
                result.EndTime = DateTime.UtcNow;
                result.Duration = result.EndTime - result.StartTime;

                _logger.Information("✅ **AUDIT_PURGE_COMPLETE**: Purged {PurgedCount} entries, retained {RetainedCount} entries in {Duration}ms",
                    result.EntriesPurged, result.EntriesRetained, result.Duration.TotalMilliseconds);

                return result;
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **AUDIT_PURGE_ERROR**: Failed to purge old audit entries");

                result.Success = false;
                result.ErrorMessage = $"Purge operation failed: {ex.Message}";
                result.EndTime = DateTime.UtcNow;
                result.Duration = result.EndTime - result.StartTime;

                return result;
            }
        }

        #endregion

        #region File Management Methods

        /// <summary>
        /// Writes an individual audit file for detailed entry storage.
        /// </summary>
        private async Task<string> WriteIndividualAuditFileAsync(AuditTrailEntry auditEntry)
        {
            var fileName = $"{auditEntry.Timestamp:yyyyMMdd_HHmmss}_{auditEntry.TemplateName}_{auditEntry.Id}.json";
            var filePath = Path.Combine(_auditTrailPath, "entries", fileName);

            Directory.CreateDirectory(Path.GetDirectoryName(filePath));

            var json = JsonSerializer.Serialize(auditEntry, _jsonOptions);
            
            lock (_fileLock)
            {
                File.WriteAllText(filePath, json);
            }

            _logger.Verbose("📁 **INDIVIDUAL_AUDIT_FILE_CREATED**: {FilePath}", filePath);
            return filePath;
        }

        /// <summary>
        /// Appends entry to consolidated audit log for efficient searching.
        /// </summary>
        private async Task AppendToConsolidatedAuditLogAsync(AuditTrailEntry auditEntry)
        {
            var consolidatedLogPath = Path.Combine(_auditTrailPath, "consolidated_audit.jsonl");
            var logEntry = JsonSerializer.Serialize(auditEntry, _jsonOptions);

            lock (_fileLock)
            {
                File.AppendAllText(consolidatedLogPath, logEntry + Environment.NewLine);
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// Loads all entries from consolidated audit log.
        /// </summary>
        private async Task<List<AuditTrailEntry>> LoadConsolidatedAuditLogAsync()
        {
            var consolidatedLogPath = Path.Combine(_auditTrailPath, "consolidated_audit.jsonl");
            var entries = new List<AuditTrailEntry>();

            if (!File.Exists(consolidatedLogPath))
            {
                return entries;
            }

            try
            {
                var lines = await File.ReadAllLinesAsync(consolidatedLogPath);
                foreach (var line in lines)
                {
                    if (!string.IsNullOrWhiteSpace(line))
                    {
                        try
                        {
                            var entry = JsonSerializer.Deserialize<AuditTrailEntry>(line, _jsonOptions);
                            entries.Add(entry);
                        }
                        catch (JsonException ex)
                        {
                            _logger.Warning("⚠️ **AUDIT_LOG_PARSE_WARNING**: Could not parse audit log line: {Error}", ex.Message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **AUDIT_LOG_LOAD_ERROR**: Failed to load consolidated audit log");
            }

            return entries;
        }

        /// <summary>
        /// Writes complete consolidated audit log (used during purge operations).
        /// </summary>
        private async Task WriteConsolidatedAuditLogAsync(List<AuditTrailEntry> entries)
        {
            var consolidatedLogPath = Path.Combine(_auditTrailPath, "consolidated_audit.jsonl");
            
            var lines = entries.Select(e => JsonSerializer.Serialize(e, _jsonOptions));
            
            lock (_fileLock)
            {
                File.WriteAllLines(consolidatedLogPath, lines);
            }

            await Task.CompletedTask;
        }

        /// <summary>
        /// Updates audit statistics file with current metrics.
        /// </summary>
        private async Task UpdateAuditStatisticsAsync(AuditTrailEntry auditEntry)
        {
            var statsPath = Path.Combine(_auditTrailPath, "statistics.json");
            
            AuditStatistics stats;
            if (File.Exists(statsPath))
            {
                var statsJson = await File.ReadAllTextAsync(statsPath);
                stats = JsonSerializer.Deserialize<AuditStatistics>(statsJson, _jsonOptions) ?? new AuditStatistics();
            }
            else
            {
                stats = new AuditStatistics();
            }

            stats.TotalOperations++;
            if (auditEntry.Success)
            {
                stats.SuccessfulOperations++;
            }
            else
            {
                stats.FailedOperations++;
            }

            stats.LastUpdated = DateTime.UtcNow;

            var updatedStatsJson = JsonSerializer.Serialize(stats, _jsonOptions);
            
            lock (_fileLock)
            {
                File.WriteAllText(statsPath, updatedStatsJson);
            }
        }

        /// <summary>
        /// Creates searchable index entry for efficient querying.
        /// </summary>
        private async Task CreateSearchableIndexEntryAsync(AuditTrailEntry auditEntry)
        {
            var indexPath = Path.Combine(_auditTrailPath, "search_index.json");
            
            var indexEntry = new AuditIndexEntry
            {
                Id = auditEntry.Id,
                TemplateName = auditEntry.TemplateName,
                Action = auditEntry.Action,
                Timestamp = auditEntry.Timestamp,
                Success = auditEntry.Success,
                ExecutionId = auditEntry.ExecutionId,
                Keywords = ExtractKeywords(auditEntry)
            };

            // For simplicity, this could be enhanced with a proper search index implementation
            await Task.CompletedTask;
        }

        #endregion

        #region Helper Methods

        /// <summary>
        /// Gets the default audit path within OCRCorrectionService structure.
        /// </summary>
        private static string GetDefaultAuditPath()
        {
            var baseDirectory = AppDomain.CurrentDomain.BaseDirectory;
            return Path.Combine(baseDirectory, "OCRCorrectionService", "Templates", "System", "Audit");
        }

        /// <summary>
        /// Ensures the audit directory structure exists.
        /// </summary>
        private void EnsureAuditDirectoryExists()
        {
            try
            {
                Directory.CreateDirectory(_auditTrailPath);
                Directory.CreateDirectory(Path.Combine(_auditTrailPath, "entries"));
                Directory.CreateDirectory(Path.Combine(_auditTrailPath, "backups"));
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **AUDIT_DIRECTORY_ERROR**: Failed to create audit directory structure");
                throw;
            }
        }

        /// <summary>
        /// Validates audit entry before writing.
        /// </summary>
        private AuditEntryValidationResult ValidateAuditEntry(AuditTrailEntry auditEntry)
        {
            if (auditEntry == null)
                return new AuditEntryValidationResult { IsValid = false, ErrorMessage = "Audit entry is null" };

            if (string.IsNullOrEmpty(auditEntry.TemplateName))
                return new AuditEntryValidationResult { IsValid = false, ErrorMessage = "Template name is required" };

            if (string.IsNullOrEmpty(auditEntry.Action))
                return new AuditEntryValidationResult { IsValid = false, ErrorMessage = "Action is required" };

            if (string.IsNullOrEmpty(auditEntry.ExecutionId))
                return new AuditEntryValidationResult { IsValid = false, ErrorMessage = "Execution ID is required" };

            return new AuditEntryValidationResult { IsValid = true };
        }

        /// <summary>
        /// Collects system information for audit context.
        /// </summary>
        private async Task<SystemInfo> CollectSystemInfoAsync()
        {
            return await Task.FromResult(new SystemInfo
            {
                MachineName = Environment.MachineName,
                UserName = Environment.UserName,
                OSVersion = Environment.OSVersion.ToString(),
                CLRVersion = Environment.Version.ToString(),
                ProcessorCount = Environment.ProcessorCount,
                WorkingSet = Environment.WorkingSet,
                CollectedAt = DateTime.UtcNow
            });
        }

        /// <summary>
        /// Collects environment information for audit context.
        /// </summary>
        private EnvironmentInfo CollectEnvironmentInfo()
        {
            return new EnvironmentInfo
            {
                EnvironmentName = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT") ?? "Development",
                ApplicationVersion = System.Reflection.Assembly.GetExecutingAssembly().GetName().Version?.ToString(),
                BaseDirectory = AppDomain.CurrentDomain.BaseDirectory,
                CollectedAt = DateTime.UtcNow
            };
        }

        /// <summary>
        /// Extracts error category from error message for analytics.
        /// </summary>
        private string ExtractErrorCategory(string errorMessage)
        {
            if (string.IsNullOrEmpty(errorMessage))
                return "Unknown";

            var lowerMessage = errorMessage.ToLower();

            if (lowerMessage.Contains("validation"))
                return "Validation Error";
            if (lowerMessage.Contains("compilation"))
                return "Compilation Error";
            if (lowerMessage.Contains("syntax"))
                return "Syntax Error";
            if (lowerMessage.Contains("timeout"))
                return "Timeout Error";
            if (lowerMessage.Contains("file") || lowerMessage.Contains("path"))
                return "File System Error";
            if (lowerMessage.Contains("network") || lowerMessage.Contains("api"))
                return "Network Error";

            return "General Error";
        }

        /// <summary>
        /// Extracts keywords from audit entry for search indexing.
        /// </summary>
        private List<string> ExtractKeywords(AuditTrailEntry auditEntry)
        {
            var keywords = new List<string>
            {
                auditEntry.TemplateName,
                auditEntry.Action,
                auditEntry.Success ? "success" : "failure"
            };

            if (!string.IsNullOrEmpty(auditEntry.ErrorMessage))
            {
                keywords.Add("error");
                keywords.Add(ExtractErrorCategory(auditEntry.ErrorMessage).ToLower());
            }

            return keywords.Distinct().ToList();
        }

        /// <summary>
        /// Gets the file path for an individual audit entry.
        /// </summary>
        private string GetIndividualAuditFilePath(AuditTrailEntry auditEntry)
        {
            var fileName = $"{auditEntry.Timestamp:yyyyMMdd_HHmmss}_{auditEntry.TemplateName}_{auditEntry.Id}.json";
            return Path.Combine(_auditTrailPath, "entries", fileName);
        }

        /// <summary>
        /// Creates backup of entries being purged.
        /// </summary>
        private async Task CreatePurgeBackupAsync(List<AuditTrailEntry> entriesToPurge)
        {
            var backupFileName = $"purged_entries_{DateTime.UtcNow:yyyyMMdd_HHmmss}.json";
            var backupPath = Path.Combine(_auditTrailPath, "backups", backupFileName);

            Directory.CreateDirectory(Path.GetDirectoryName(backupPath));

            var backupData = new
            {
                PurgeTimestamp = DateTime.UtcNow,
                EntryCount = entriesToPurge.Count,
                Entries = entriesToPurge
            };

            var backupJson = JsonSerializer.Serialize(backupData, _jsonOptions);
            await File.WriteAllTextAsync(backupPath, backupJson);

            _logger.Information("💾 **PURGE_BACKUP_CREATED**: Backed up {EntryCount} entries to '{BackupPath}'",
                entriesToPurge.Count, backupPath);
        }

        /// <summary>
        /// Updates statistics after purge operation.
        /// </summary>
        private async Task UpdateAuditStatisticsAfterPurgeAsync(AuditPurgeResult purgeResult)
        {
            var statsPath = Path.Combine(_auditTrailPath, "statistics.json");
            
            AuditStatistics stats;
            if (File.Exists(statsPath))
            {
                var statsJson = await File.ReadAllTextAsync(statsPath);
                stats = JsonSerializer.Deserialize<AuditStatistics>(statsJson, _jsonOptions) ?? new AuditStatistics();
            }
            else
            {
                stats = new AuditStatistics();
            }

            stats.TotalPurgeOperations++;
            stats.TotalEntriesPurged += purgeResult.EntriesPurged;
            stats.LastPurgeDate = purgeResult.StartTime;
            stats.LastUpdated = DateTime.UtcNow;

            var updatedStatsJson = JsonSerializer.Serialize(stats, _jsonOptions);
            
            lock (_fileLock)
            {
                File.WriteAllText(statsPath, updatedStatsJson);
            }
        }

        #endregion

        #region IDisposable Implementation

        public void Dispose()
        {
            _logger?.Information("🧹 **AUDIT_TRAIL_SERVICE_DISPOSED**: Service disposed successfully");
        }

        #endregion
    }

    #region Supporting Data Models

    /// <summary>
    /// Comprehensive audit trail entry with full change tracking.
    /// </summary>
    public class AuditTrailEntry
    {
        public string Id { get; set; } = Guid.NewGuid().ToString();
        public string TemplateName { get; set; }
        public string ExecutionId { get; set; }
        public DateTime Timestamp { get; set; } = DateTime.UtcNow;
        public string Action { get; set; }
        public bool Success { get; set; }
        public int RecommendationsCount { get; set; }
        public int ImplementedCount { get; set; }
        public string BackupId { get; set; }
        public string ErrorMessage { get; set; }
        public TimeSpan? ExecutionDuration { get; set; }
        public List<object> Steps { get; set; } = new List<object>();
        public string AuditVersion { get; set; }
        public SystemInfo SystemInfo { get; set; }
        public EnvironmentInfo EnvironmentInfo { get; set; }
        public Dictionary<string, object> Metadata { get; set; } = new Dictionary<string, object>();
    }

    /// <summary>
    /// Result of audit entry creation.
    /// </summary>
    public class AuditEntryResult
    {
        public bool Success { get; set; }
        public string AuditEntryId { get; set; }
        public string TemplateName { get; set; }
        public DateTime Timestamp { get; set; }
        public string AuditFilePath { get; set; }
        public long FileSize { get; set; }
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// Filter for audit history queries.
    /// </summary>
    public class AuditHistoryFilter
    {
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string ActionType { get; set; }
        public bool? SuccessOnly { get; set; }
        public int PageNumber { get; set; } = 0;
        public int PageSize { get; set; } = 50;
    }

    /// <summary>
    /// Result of audit history query.
    /// </summary>
    public class AuditHistoryResult
    {
        public bool Success { get; set; }
        public string TemplateName { get; set; }
        public DateTime RequestTimestamp { get; set; }
        public List<AuditTrailEntry> AuditEntries { get; set; } = new List<AuditTrailEntry>();
        public int TotalEntries { get; set; }
        public int FilteredEntries { get; set; }
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// System information for audit context.
    /// </summary>
    public class SystemInfo
    {
        public string MachineName { get; set; }
        public string UserName { get; set; }
        public string OSVersion { get; set; }
        public string CLRVersion { get; set; }
        public int ProcessorCount { get; set; }
        public long WorkingSet { get; set; }
        public DateTime CollectedAt { get; set; }
    }

    /// <summary>
    /// Environment information for audit context.
    /// </summary>
    public class EnvironmentInfo
    {
        public string EnvironmentName { get; set; }
        public string ApplicationVersion { get; set; }
        public string BaseDirectory { get; set; }
        public DateTime CollectedAt { get; set; }
    }

    /// <summary>
    /// Audit entry validation result.
    /// </summary>
    public class AuditEntryValidationResult
    {
        public bool IsValid { get; set; }
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// Audit statistics for tracking.
    /// </summary>
    public class AuditStatistics
    {
        public int TotalOperations { get; set; }
        public int SuccessfulOperations { get; set; }
        public int FailedOperations { get; set; }
        public int TotalPurgeOperations { get; set; }
        public int TotalEntriesPurged { get; set; }
        public DateTime? LastPurgeDate { get; set; }
        public DateTime LastUpdated { get; set; }
    }

    /// <summary>
    /// Searchable index entry for efficient querying.
    /// </summary>
    public class AuditIndexEntry
    {
        public string Id { get; set; }
        public string TemplateName { get; set; }
        public string Action { get; set; }
        public DateTime Timestamp { get; set; }
        public bool Success { get; set; }
        public string ExecutionId { get; set; }
        public List<string> Keywords { get; set; } = new List<string>();
    }

    /// <summary>
    /// Audit retention policy configuration.
    /// </summary>
    public class AuditRetentionPolicy
    {
        public TimeSpan RetentionPeriod { get; set; } = TimeSpan.FromDays(90);
        public int MinimumEntriesToKeep { get; set; } = 100;
        public bool CreateBackupBeforePurge { get; set; } = true;
    }

    /// <summary>
    /// Result of audit purge operation.
    /// </summary>
    public class AuditPurgeResult
    {
        public bool Success { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public TimeSpan Duration { get; set; }
        public AuditRetentionPolicy RetentionPolicy { get; set; }
        public int TotalEntriesBeforePurge { get; set; }
        public int EntriesPurged { get; set; }
        public int EntriesRetained { get; set; }
        public int FilesDeleted { get; set; }
        public bool BackupCreated { get; set; }
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// Request for audit analytics generation.
    /// </summary>
    public class AuditAnalyticsRequest
    {
        public TimeSpan AnalysisPeriod { get; set; } = TimeSpan.FromDays(30);
        public bool IncludeErrorAnalysis { get; set; } = true;
        public bool IncludeTemplateBreakdown { get; set; } = true;
        public bool IncludeTimelineData { get; set; } = true;
    }

    /// <summary>
    /// Comprehensive audit analytics result.
    /// </summary>
    public class AuditAnalyticsResult
    {
        public bool Success { get; set; }
        public DateTime GeneratedAt { get; set; }
        public TimeSpan AnalysisPeriod { get; set; }
        public int TotalOperations { get; set; }
        public int SuccessfulOperations { get; set; }
        public int FailedOperations { get; set; }
        public double SuccessRate { get; set; }
        public List<TemplateAuditStatistics> TemplateStatistics { get; set; } = new List<TemplateAuditStatistics>();
        public List<ActionTypeStatistics> ActionTypeStatistics { get; set; } = new List<ActionTypeStatistics>();
        public List<DailyAuditStatistics> TimelineData { get; set; } = new List<DailyAuditStatistics>();
        public List<ErrorCategoryStatistics> ErrorAnalysis { get; set; } = new List<ErrorCategoryStatistics>();
        public string ErrorMessage { get; set; }
    }

    /// <summary>
    /// Template-specific audit statistics.
    /// </summary>
    public class TemplateAuditStatistics
    {
        public string TemplateName { get; set; }
        public int TotalOperations { get; set; }
        public int SuccessfulOperations { get; set; }
        public int FailedOperations { get; set; }
        public DateTime LastModified { get; set; }
        public double AverageRecommendationsPerOperation { get; set; }
    }

    /// <summary>
    /// Action type statistics.
    /// </summary>
    public class ActionTypeStatistics
    {
        public string ActionType { get; set; }
        public int TotalCount { get; set; }
        public int SuccessCount { get; set; }
        public int FailureCount { get; set; }
        public double AverageExecutionTime { get; set; }
    }

    /// <summary>
    /// Daily audit statistics for timeline analysis.
    /// </summary>
    public class DailyAuditStatistics
    {
        public DateTime Date { get; set; }
        public int TotalOperations { get; set; }
        public int SuccessfulOperations { get; set; }
        public int UniqueTemplatesModified { get; set; }
    }

    /// <summary>
    /// Error category statistics for error analysis.
    /// </summary>
    public class ErrorCategoryStatistics
    {
        public string ErrorCategory { get; set; }
        public int Count { get; set; }
        public List<string> AffectedTemplates { get; set; } = new List<string>();
        public List<object> RecentOccurrences { get; set; } = new List<object>();
    }

    #endregion
}