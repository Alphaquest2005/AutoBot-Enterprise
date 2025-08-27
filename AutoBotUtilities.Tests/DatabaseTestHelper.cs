using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using NUnit.Framework;
using OCR.Business.Entities;
using Serilog;

namespace AutoBotUtilities.Tests
{
    /// <summary>
    /// Comprehensive database test helper for AutoBot-Enterprise OCR system
    /// Provides direct database access, script execution, and data analysis capabilities
    /// </summary>
    [TestFixture]
    public class DatabaseTestHelper
    {
        private static ILogger _logger;
        private string _connectionString;

        [OneTimeSetUp]
        public void Setup()
        {
            _logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            // Get connection string from OCRContext
            using (var context = new OCRContext())
            {
                _connectionString = context.Database.Connection.ConnectionString;
                _logger.Information("📡 **DATABASE_CONNECTION**: Using connection string: {ConnectionString}", 
                    _connectionString.Substring(0, Math.Min(100, _connectionString.Length)) + "...");
            }
        }

        #region Direct SQL Script Execution

        /// <summary>
        /// Execute a raw SQL script against the database
        /// </summary>
        [Test]
        [Explicit("Run manually with specific SQL scripts")]
        public async Task ExecuteRawSqlScript()
        {
            string script = @"
                -- Example: Find all problematic TotalDeduction patterns
                SELECT l.Id AS LineId, l.Name, r.Id AS RegexId, r.RegEx, r.Description
                FROM Lines l
                INNER JOIN RegularExpressions r ON l.RegExId = r.Id
                WHERE r.RegEx LIKE '%TotalDeduction%'
                ORDER BY l.Name;
            ";

            await this.ExecuteSqlScript(script, "Find TotalDeduction patterns").ConfigureAwait(false);
        }

        /// <summary>
        /// Execute a custom SQL script with parameters
        /// </summary>
        public async Task<DataTable> ExecuteSqlScript(string script, string description = null, Dictionary<string, object> parameters = null)
        {
            _logger.Information("🔍 **SQL_EXECUTION**: {Description}", description ?? "Executing SQL script");
            _logger.Information("📄 **SQL_SCRIPT**: {Script}", script);

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync().ConfigureAwait(false);
                
                using (var command = new SqlCommand(script, connection))
                {
                    // Add parameters if provided
                    if (parameters != null)
                    {
                        foreach (var param in parameters)
                        {
                            command.Parameters.AddWithValue($"@{param.Key}", param.Value ?? DBNull.Value);
                            _logger.Information("📝 **SQL_PARAMETER**: @{ParamName} = {ParamValue}", param.Key, param.Value);
                        }
                    }

                    using (var adapter = new SqlDataAdapter(command))
                    {
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        _logger.Information("📊 **SQL_RESULT**: Returned {RowCount} rows, {ColumnCount} columns", 
                            dataTable.Rows.Count, dataTable.Columns.Count);

                        // Log column names
                        var columnNames = dataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray();
                        _logger.Information("📋 **SQL_COLUMNS**: {Columns}", string.Join(", ", columnNames));

                        // Log first few rows for verification
                        for (int i = 0; i < Math.Min(10, dataTable.Rows.Count); i++)
                        {
                            var row = dataTable.Rows[i];
                            var values = row.ItemArray.Select(field => field?.ToString() ?? "NULL").ToArray();
                            _logger.Information("📝 **SQL_ROW_{RowIndex}**: {Values}", i + 1, string.Join(" | ", values));
                        }

                        if (dataTable.Rows.Count > 10)
                        {
                            _logger.Information("📝 **SQL_MORE_ROWS**: ... and {MoreRows} more rows", dataTable.Rows.Count - 10);
                        }

                        return dataTable;
                    }
                }
            }
        }

        #endregion

        #region Amazon Template Analysis

        /// <summary>
        /// Comprehensive analysis of Amazon template configuration
        /// </summary>
        [Test]
        public async Task AnalyzeAmazonTemplate()
        {
            _logger.Information("🔍 **AMAZON_ANALYSIS**: Starting comprehensive Amazon template analysis");

            // Get Amazon template info
            var templateScript = @"
                SELECT i.Id, i.Name, i.Description, 
                       COUNT(DISTINCT p.Id) AS PartCount,
                       COUNT(DISTINCT l.Id) AS LineCount,
                       COUNT(DISTINCT f.Id) AS FieldCount
                FROM Invoices i
                LEFT JOIN Parts p ON i.Id = p.TemplateId
                LEFT JOIN Lines l ON p.Id = l.PartId
                LEFT JOIN Fields f ON l.Id = f.LineId
                WHERE i.Id = 5
                GROUP BY i.Id, i.Name, i.Description;
            ";

            await this.ExecuteSqlScript(templateScript, "Amazon template overview").ConfigureAwait(false);

            // Get all lines and patterns
            var patternsScript = @"
                SELECT 
                    l.Id AS LineId,
                    l.Name AS LineName,
                    p.Name AS PartName,
                    r.Id AS RegexId,
                    r.RegEx,
                    r.Description AS RegexDescription,
                    f.Id AS FieldId,
                    f.Key AS FieldKey,
                    f.Field AS FieldName,
                    f.EntityType,
                    f.DataType,
                    f.AppendValues
                FROM Lines l
                INNER JOIN Parts p ON l.PartId = p.Id
                LEFT JOIN RegularExpressions r ON l.RegExId = r.Id
                LEFT JOIN Fields f ON l.Id = f.LineId
                WHERE p.TemplateId = 5
                ORDER BY p.Name, l.Name, f.Field;
            ";

            await this.ExecuteSqlScript(patternsScript, "Amazon template patterns and fields").ConfigureAwait(false);

            // Check for Gift Card and Free Shipping patterns specifically
            var specificPatternsScript = @"
                SELECT 
                    'Gift Card' AS PatternType,
                    l.Id AS LineId,
                    l.Name AS LineName,
                    r.RegEx,
                    f.Field AS FieldName
                FROM Lines l
                INNER JOIN Parts p ON l.PartId = p.Id
                LEFT JOIN RegularExpressions r ON l.RegExId = r.Id
                LEFT JOIN Fields f ON l.Id = f.LineId
                WHERE p.TemplateId = 5 
                  AND (r.RegEx LIKE '%Gift Card%' OR l.Name LIKE '%Gift%' OR f.Field LIKE '%Insurance%')
                
                UNION ALL
                
                SELECT 
                    'Free Shipping' AS PatternType,
                    l.Id AS LineId,
                    l.Name AS LineName,
                    r.RegEx,
                    f.Field AS FieldName
                FROM Lines l
                INNER JOIN Parts p ON l.PartId = p.Id
                LEFT JOIN RegularExpressions r ON l.RegExId = r.Id
                LEFT JOIN Fields f ON l.Id = f.LineId
                WHERE p.TemplateId = 5 
                  AND (r.RegEx LIKE '%Free Shipping%' OR l.Name LIKE '%Shipping%' OR f.Field LIKE '%Deduction%')
                
                ORDER BY PatternType, LineId;
            ";

            await this.ExecuteSqlScript(specificPatternsScript, "Amazon Gift Card and Free Shipping patterns").ConfigureAwait(false);
        }

        #endregion

        #region Database Statistics and Health Checks

        /// <summary>
        /// Get comprehensive database statistics
        /// </summary>
        [Test]
        public async Task GetDatabaseStatistics()
        {
            _logger.Information("📊 **DATABASE_STATS**: Collecting comprehensive database statistics");

            var statsScript = @"
                SELECT 'Templates' AS EntityType, COUNT(*) AS Count FROM Invoices
                UNION ALL
                SELECT 'Parts' AS EntityType, COUNT(*) AS Count FROM Parts
                UNION ALL
                SELECT 'Lines' AS EntityType, COUNT(*) AS Count FROM Lines
                UNION ALL
                SELECT 'RegularExpressions' AS EntityType, COUNT(*) AS Count FROM RegularExpressions
                UNION ALL
                SELECT 'Fields' AS EntityType, COUNT(*) AS Count FROM Fields
                UNION ALL
                SELECT 'FieldFormatRegEx' AS EntityType, COUNT(*) AS Count FROM OCR_FieldFormatRegEx
                ORDER BY EntityType;
            ";

            await this.ExecuteSqlScript(statsScript, "Overall database statistics").ConfigureAwait(false);

            // Pattern analysis
            var patternStatsScript = @"
                SELECT 
                    'TotalDeduction patterns' AS Category,
                    COUNT(*) AS Count,
                    MIN(LEN(RegEx)) AS MinLength,
                    MAX(LEN(RegEx)) AS MaxLength,
                    AVG(LEN(RegEx)) AS AvgLength
                FROM RegularExpressions 
                WHERE RegEx LIKE '%TotalDeduction%'
                
                UNION ALL
                
                SELECT 
                    'TotalInsurance patterns' AS Category,
                    COUNT(*) AS Count,
                    MIN(LEN(RegEx)) AS MinLength,
                    MAX(LEN(RegEx)) AS MaxLength,
                    AVG(LEN(RegEx)) AS AvgLength
                FROM RegularExpressions 
                WHERE RegEx LIKE '%TotalInsurance%'
                
                UNION ALL
                
                SELECT 
                    'InvoiceTotal patterns' AS Category,
                    COUNT(*) AS Count,
                    MIN(LEN(RegEx)) AS MinLength,
                    MAX(LEN(RegEx)) AS MaxLength,
                    AVG(LEN(RegEx)) AS AvgLength
                FROM RegularExpressions 
                WHERE RegEx LIKE '%InvoiceTotal%';
            ";

            await this.ExecuteSqlScript(patternStatsScript, "Pattern analysis statistics").ConfigureAwait(false);
        }

        /// <summary>
        /// Check for potentially problematic patterns
        /// </summary>
        [Test]
        public async Task CheckProblematicPatterns()
        {
            _logger.Information("🚨 **PROBLEMATIC_PATTERNS**: Checking for potentially problematic regex patterns");

            var problematicScript = @"
                -- Very short patterns that might be too broad
                SELECT 
                    'Short patterns' AS IssueType,
                    r.Id,
                    r.RegEx,
                    LEN(r.RegEx) AS PatternLength,
                    r.Description,
                    COUNT(l.Id) AS LinesUsingPattern
                FROM RegularExpressions r
                LEFT JOIN Lines l ON r.Id = l.RegExId
                WHERE LEN(r.RegEx) < 30 AND r.RegEx LIKE '%(?<%'
                GROUP BY r.Id, r.RegEx, r.Description
                HAVING COUNT(l.Id) > 0
                
                UNION ALL
                
                -- Patterns that just capture numbers (potentially conflicting)
                SELECT 
                    'Number-only patterns' AS IssueType,
                    r.Id,
                    r.RegEx,
                    LEN(r.RegEx) AS PatternLength,
                    r.Description,
                    COUNT(l.Id) AS LinesUsingPattern
                FROM RegularExpressions r
                LEFT JOIN Lines l ON r.Id = l.RegExId
                WHERE r.RegEx LIKE '%\\d+%' 
                  AND r.RegEx LIKE '%\.%' 
                  AND LEN(r.RegEx) < 40
                  AND r.RegEx NOT LIKE '%[A-Za-z]%'
                GROUP BY r.Id, r.RegEx, r.Description
                HAVING COUNT(l.Id) > 0
                
                ORDER BY IssueType, PatternLength;
            ";

            await this.ExecuteSqlScript(problematicScript, "Potentially problematic patterns").ConfigureAwait(false);
        }

        #endregion

        #region OCR Correction Learning Analysis

        /// <summary>
        /// Analyze OCR correction learning data
        /// </summary>
        [Test]
        public async Task AnalyzeOCRLearningData()
        {
            _logger.Information("🧠 **OCR_LEARNING**: Analyzing OCR correction learning data");

            // Check if OCRCorrectionLearning table exists and has data
            var learningScript = @"
                IF OBJECT_ID('OCRCorrectionLearning', 'U') IS NOT NULL
                BEGIN
                    SELECT 
                        COUNT(*) AS TotalCorrections,
                        COUNT(DISTINCT InvoiceNo) AS UniqueInvoices,
                        COUNT(DISTINCT FieldName) AS UniqueFields,
                        MIN(CreatedDate) AS EarliestCorrection,
                        MAX(CreatedDate) AS LatestCorrection
                    FROM OCRCorrectionLearning;
                    
                    SELECT TOP 20
                        InvoiceNo,
                        FieldName,
                        OldValue,
                        NewValue,
                        CorrectionType,
                        CreatedDate
                    FROM OCRCorrectionLearning
                    ORDER BY CreatedDate DESC;
                END
                ELSE
                BEGIN
                    SELECT 'OCRCorrectionLearning table does not exist' AS Message;
                END
            ";

            await this.ExecuteSqlScript(learningScript, "OCR correction learning analysis").ConfigureAwait(false);
        }

        #endregion

        #region Field Mapping Analysis

        /// <summary>
        /// Analyze field mappings for Caribbean customs compliance
        /// </summary>
        [Test]
        public async Task AnalyzeFieldMappings()
        {
            _logger.Information("🗺️ **FIELD_MAPPING**: Analyzing field mappings for Caribbean customs compliance");

            var mappingScript = @"
                -- All TotalInsurance field mappings (should be customer reductions)
                SELECT 
                    'TotalInsurance mappings' AS MappingType,
                    f.Id AS FieldId,
                    f.Key,
                    f.Field,
                    f.EntityType,
                    f.DataType,
                    f.AppendValues,
                    l.Name AS LineName,
                    r.RegEx
                FROM Fields f
                INNER JOIN Lines l ON f.LineId = l.Id
                LEFT JOIN RegularExpressions r ON l.RegExId = r.Id
                WHERE f.Field = 'TotalInsurance'
                
                UNION ALL
                
                -- All TotalDeduction field mappings (should be supplier reductions)
                SELECT 
                    'TotalDeduction mappings' AS MappingType,
                    f.Id AS FieldId,
                    f.Key,
                    f.Field,
                    f.EntityType,
                    f.DataType,
                    f.AppendValues,
                    l.Name AS LineName,
                    r.RegEx
                FROM Fields f
                INNER JOIN Lines l ON f.LineId = l.Id
                LEFT JOIN RegularExpressions r ON l.RegExId = r.Id
                WHERE f.Field = 'TotalDeduction'
                
                ORDER BY MappingType, FieldId;
            ";

            await this.ExecuteSqlScript(mappingScript, "Caribbean customs field mappings").ConfigureAwait(false);

            // Check for potential mapping conflicts
            var conflictScript = @"
                -- Look for lines that might have conflicting field mappings
                SELECT 
                    l.Id AS LineId,
                    l.Name AS LineName,
                    r.RegEx,
                    STRING_AGG(f.Field, ', ') AS FieldsMapped,
                    COUNT(f.Id) AS FieldCount
                FROM Lines l
                LEFT JOIN RegularExpressions r ON l.RegExId = r.Id
                LEFT JOIN Fields f ON l.Id = f.LineId
                WHERE f.Field IN ('TotalInsurance', 'TotalDeduction', 'TotalOtherCost')
                GROUP BY l.Id, l.Name, r.RegEx
                HAVING COUNT(f.Id) > 1
                ORDER BY FieldCount DESC;
            ";

            await this.ExecuteSqlScript(conflictScript, "Potential field mapping conflicts").ConfigureAwait(false);
        }

        #endregion

        #region Template Context Export

        /// <summary>
        /// Export complete template context for testing
        /// </summary>
        [Test]
        [Explicit("Run manually to export template context")]
        public async Task ExportTemplateContext()
        {
            string templateId = "5"; // Amazon template
            _logger.Information("📤 **TEMPLATE_EXPORT**: Exporting complete context for template {TemplateId}", templateId);

            var exportScript = @"
                -- Complete template export with all related data
                SELECT 
                    'Template' AS EntityType,
                    i.Id AS Id,
                    i.Name AS Name,
                    i.Description AS Description,
                    NULL AS ParentId,
                    NULL AS RegEx,
                    NULL AS Field,
                    NULL AS EntityName,
                    NULL AS DataType
                FROM Invoices i WHERE i.Id = @templateId
                
                UNION ALL
                
                SELECT 
                    'Part' AS EntityType,
                    p.Id AS Id,
                    p.Name AS Name,
                    p.Description AS Description,
                    p.TemplateId AS ParentId,
                    NULL AS RegEx,
                    NULL AS Field,
                    NULL AS EntityName,
                    NULL AS DataType
                FROM Parts p WHERE p.TemplateId = @templateId
                
                UNION ALL
                
                SELECT 
                    'Line' AS EntityType,
                    l.Id AS Id,
                    l.Name AS Name,
                    NULL AS Description,
                    l.PartId AS ParentId,
                    r.RegEx AS RegEx,
                    NULL AS Field,
                    NULL AS EntityName,
                    NULL AS DataType
                FROM Lines l
                LEFT JOIN RegularExpressions r ON l.RegExId = r.Id
                WHERE l.PartId IN (SELECT Id FROM Parts WHERE TemplateId = @templateId)
                
                UNION ALL
                
                SELECT 
                    'Field' AS EntityType,
                    f.Id AS Id,
                    f.Key AS Name,
                    NULL AS Description,
                    f.LineId AS ParentId,
                    NULL AS RegEx,
                    f.Field AS Field,
                    f.EntityType AS EntityName,
                    f.DataType AS DataType
                FROM Fields f
                WHERE f.LineId IN (
                    SELECT l.Id FROM Lines l
                    INNER JOIN Parts p ON l.PartId = p.Id
                    WHERE p.TemplateId = @templateId
                )
                
                ORDER BY EntityType, ParentId, Id;
            ";

            var parameters = new Dictionary<string, object> { { "templateId", templateId } };
            await this.ExecuteSqlScript(exportScript, $"Complete template context export for template {templateId}", parameters).ConfigureAwait(false);
        }

        #endregion

        #region Data Cleanup Utilities

        /// <summary>
        /// Clean up orphaned database records
        /// </summary>
        [Test]
        [Explicit("Run manually to clean up orphaned records")]
        public async Task CleanupOrphanedRecords()
        {
            _logger.Information("🧹 **CLEANUP**: Checking for orphaned database records");

            var orphanScript = @"
                -- Find orphaned Fields (pointing to non-existent Lines)
                SELECT 
                    'Orphaned Fields' AS OrphanType,
                    f.Id,
                    f.Key,
                    f.Field,
                    f.LineId AS OrphanedReferenceId
                FROM Fields f
                LEFT JOIN Lines l ON f.LineId = l.Id
                WHERE l.Id IS NULL
                
                UNION ALL
                
                -- Find orphaned Lines (pointing to non-existent Parts)
                SELECT 
                    'Orphaned Lines' AS OrphanType,
                    l.Id,
                    l.Name,
                    NULL AS Field,
                    l.PartId AS OrphanedReferenceId
                FROM Lines l
                LEFT JOIN Parts p ON l.PartId = p.Id
                WHERE p.Id IS NULL
                
                UNION ALL
                
                -- Find orphaned Parts (pointing to non-existent Templates)
                SELECT 
                    'Orphaned Parts' AS OrphanType,
                    p.Id,
                    p.Name,
                    NULL AS Field,
                    p.TemplateId AS OrphanedReferenceId
                FROM Parts p
                LEFT JOIN Invoices i ON p.TemplateId = i.Id
                WHERE i.Id IS NULL
                
                ORDER BY OrphanType, Id;
            ";

            await this.ExecuteSqlScript(orphanScript, "Orphaned records check").ConfigureAwait(false);
        }

        /// <summary>
        /// Backup critical database entities before making changes
        /// </summary>
        [Test]
        [Explicit("Run manually to backup critical entities")]
        public async Task BackupCriticalEntities()
        {
            _logger.Information("💾 **BACKUP**: Creating backup of critical database entities");

            var backupScript = @"
                -- Create backup tables with timestamp
                DECLARE @timestamp VARCHAR(20) = FORMAT(GETDATE(), 'yyyyMMdd_HHmmss');
                DECLARE @sql NVARCHAR(MAX);
                
                -- Backup RegularExpressions
                SET @sql = 'SELECT * INTO RegularExpressions_Backup_' + @timestamp + ' FROM RegularExpressions';
                EXEC sp_executesql @sql;
                
                -- Backup Lines  
                SET @sql = 'SELECT * INTO Lines_Backup_' + @timestamp + ' FROM Lines';
                EXEC sp_executesql @sql;
                
                -- Backup Fields
                SET @sql = 'SELECT * INTO Fields_Backup_' + @timestamp + ' FROM Fields';
                EXEC sp_executesql @sql;
                
                SELECT 'Backup completed with timestamp: ' + @timestamp AS BackupResult;
            ";

            await this.ExecuteSqlScript(backupScript, "Backup critical entities").ConfigureAwait(false);
        }

        #endregion

        #region OCR Template Structure Analysis

        /// <summary>
        /// Analyze OCR template structure and FileType relationships for template specification integration
        /// </summary>
        [Test]
        public async Task AnalyzeOCRTemplateStructure()
        {
            _logger.Information("🔍 **OCR_TEMPLATE_ANALYSIS**: Starting comprehensive OCR template structure analysis");

            // 1. Check OCR_TemplateTableMapping structure
            var templateMappingStructureScript = @"
                SELECT 
                    COLUMN_NAME, 
                    DATA_TYPE, 
                    IS_NULLABLE, 
                    COLUMN_DEFAULT,
                    CHARACTER_MAXIMUM_LENGTH
                FROM INFORMATION_SCHEMA.COLUMNS 
                WHERE TABLE_NAME = 'OCR_TemplateTableMapping' 
                AND TABLE_SCHEMA = 'dbo'
                ORDER BY ORDINAL_POSITION;
            ";

            await this.ExecuteSqlScript(templateMappingStructureScript, "OCR_TemplateTableMapping table structure").ConfigureAwait(false);

            // 2. Check current OCR_TemplateTableMapping data
            var templateMappingDataScript = @"
                SELECT * FROM [WebSource-AutoBot].[dbo].[OCR_TemplateTableMapping];
            ";

            await this.ExecuteSqlScript(templateMappingDataScript, "Current OCR_TemplateTableMapping data").ConfigureAwait(false);

            // 3. Check FileTypes-FileImporterInfo data (DocumentType enum mappings)
            var fileTypesScript = @"
                SELECT 
                    Id as FileTypeId,
                    EntryType as DocumentTypeEnum,
                    CASE EntryType
                        WHEN 0 THEN 'Invoice'
                        WHEN 1 THEN 'PurchaseOrder'
                        WHEN 2 THEN 'Receipt'
                        WHEN 3 THEN 'Statement'
                        WHEN 4 THEN 'Other'
                        ELSE 'Unknown (' + CAST(EntryType as VARCHAR) + ')'
                    END as DocumentTypeName,
                    FileExtension,
                    Description
                FROM [WebSource-AutoBot].[dbo].[FileTypes-FileImporterInfo]
                ORDER BY EntryType;
            ";

            await this.ExecuteSqlScript(fileTypesScript, "FileTypes-FileImporterInfo DocumentType mappings").ConfigureAwait(false);

            // 4. Check if FileTypeId column exists in OCR_TemplateTableMapping
            var columnExistsScript = @"
                SELECT 
                    CASE 
                        WHEN EXISTS (
                            SELECT 1 FROM INFORMATION_SCHEMA.COLUMNS 
                            WHERE TABLE_NAME = 'OCR_TemplateTableMapping' 
                            AND COLUMN_NAME = 'FileTypeId'
                        ) THEN 'FileTypeId column EXISTS'
                        ELSE 'FileTypeId column MISSING - needs to be added'
                    END as FileTypeIdColumnStatus;
            ";

            await this.ExecuteSqlScript(columnExistsScript, "FileTypeId column existence check").ConfigureAwait(false);

            // 5. Check OCR-PartLineFields view for EntityType analysis
            var entityTypeAnalysisScript = @"
                IF OBJECT_ID('OCR-PartLineFields', 'V') IS NOT NULL OR OBJECT_ID('OCR-PartLineFields', 'U') IS NOT NULL
                BEGIN
                    SELECT DISTINCT EntityType, COUNT(*) as FieldCount
                    FROM [WebSource-AutoBot].[dbo].[OCR-PartLineFields]
                    WHERE EntityType IS NOT NULL AND EntityType != ''
                    GROUP BY EntityType 
                    ORDER BY EntityType;
                END
                ELSE
                BEGIN
                    SELECT 'OCR-PartLineFields view/table does not exist' AS Message;
                END
            ";

            await this.ExecuteSqlScript(entityTypeAnalysisScript, "OCR-PartLineFields EntityType analysis").ConfigureAwait(false);

            // 6. Check current database relationships 
            var relationshipsScript = @"
                -- Show current relationship between FileTypes and OCR templates (if exists)
                SELECT 
                    fti.Id as FileTypeId,
                    fti.EntryType as DocumentTypeEnum,
                    CASE fti.EntryType
                        WHEN 0 THEN 'Invoice'
                        WHEN 1 THEN 'PurchaseOrder'
                        WHEN 2 THEN 'Receipt'
                        WHEN 3 THEN 'Statement'
                        WHEN 4 THEN 'Other'
                        ELSE 'Unknown'
                    END as DocumentTypeName,
                    fti.FileExtension,
                    CASE 
                        WHEN otm.Id IS NULL THEN 'MISSING - OCR Template needed'
                        ELSE 'OCR Template exists (ID: ' + CAST(otm.Id as VARCHAR) + ')'
                    END as TemplateStatus
                FROM [WebSource-AutoBot].[dbo].[FileTypes-FileImporterInfo] fti
                LEFT JOIN [WebSource-AutoBot].[dbo].[OCR_TemplateTableMapping] otm ON fti.Id = otm.FileTypeId
                ORDER BY fti.EntryType;
            ";

            await this.ExecuteSqlScript(relationshipsScript, "Current FileType to OCR template relationships").ConfigureAwait(false);

            // 7. Get all OCR and FileType related tables
            var ocrTablesScript = @"
                SELECT TABLE_NAME 
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_SCHEMA = 'dbo' 
                AND (TABLE_NAME LIKE '%OCR%' OR TABLE_NAME LIKE '%FileType%' OR TABLE_NAME LIKE '%Template%')
                ORDER BY TABLE_NAME;
            ";

            await this.ExecuteSqlScript(ocrTablesScript, "All OCR and FileType related tables").ConfigureAwait(false);
        }

        #endregion

        #region Custom Test Helpers

        /// <summary>
        /// Custom helper to test specific scenarios
        /// </summary>
        [Test]
        [Explicit("Run manually for custom testing scenarios")]
        public async Task CustomTestScenario()
        {
            _logger.Information("🧪 **CUSTOM_TEST**: Running custom test scenario");

            // Example: Check the current state after fixes
            var currentStateScript = @"
                -- Check current Amazon template state
                SELECT 
                    l.Name AS LineName,
                    r.RegEx,
                    f.Field AS FieldName,
                    CASE 
                        WHEN f.Field = 'TotalInsurance' THEN 'Customer Reduction (negative values)'
                        WHEN f.Field = 'TotalDeduction' THEN 'Supplier Reduction (positive values)'
                        ELSE 'Other Field'
                    END AS CaribbeanCustomsRole
                FROM Lines l
                INNER JOIN Parts p ON l.PartId = p.Id
                LEFT JOIN RegularExpressions r ON l.RegExId = r.Id
                LEFT JOIN Fields f ON l.Id = f.LineId
                WHERE p.TemplateId = 5 
                  AND f.Field IN ('TotalInsurance', 'TotalDeduction')
                ORDER BY f.Field, l.Name;
            ";

            await this.ExecuteSqlScript(currentStateScript, "Current Amazon template state for Caribbean customs").ConfigureAwait(false);
        }

        /// <summary>
        /// Investigate FileTypes-FileImporterInfo table for FileTypeId=1147 to fix EntryType issue
        /// </summary>
        [Test]
        [Explicit("Run manually to investigate FileTypes-FileImporterInfo")]
        public async Task InvestigateFileTypesFileImporterInfo()
        {
            _logger.Information("🔍 **FILETYPES_INVESTIGATION**: Investigating FileTypes-FileImporterInfo for FileTypeId=1147");

            // Check what's in FileTypes-FileImporterInfo table
            var fileTypesScript = @"
                SELECT Id, EntryType, FileExtension, Description, Format
                FROM [dbo].[FileTypes-FileImporterInfo]
                WHERE Id = 1147 OR EntryType LIKE '%Invoice%' OR EntryType LIKE '%Shipment%'
                ORDER BY Id;
            ";

            await this.ExecuteSqlScript(fileTypesScript, "FileTypes-FileImporterInfo for 1147 and Invoice types").ConfigureAwait(false);

            // Check all FileTypeIds around 1147
            var nearbyScript = @"
                SELECT Id, EntryType, FileExtension, Description, Format
                FROM [dbo].[FileTypes-FileImporterInfo]
                WHERE Id BETWEEN 1145 AND 1150
                ORDER BY Id;
            ";

            await this.ExecuteSqlScript(nearbyScript, "FileTypes-FileImporterInfo around 1147").ConfigureAwait(false);

            // Check what EntryType should be for Shipment Invoice
            var shipmentInvoiceScript = @"
                SELECT Id, EntryType, FileExtension, Description, Format
                FROM [dbo].[FileTypes-FileImporterInfo]
                WHERE EntryType = 'Shipment Invoice' OR Description LIKE '%Shipment%' OR Description LIKE '%Invoice%'
                ORDER BY Id;
            ";

            await this.ExecuteSqlScript(shipmentInvoiceScript, "Look for Shipment Invoice EntryType").ConfigureAwait(false);
            
            _logger.Information("✅ **INVESTIGATION_COMPLETE**: FileTypes-FileImporterInfo investigation complete");
        }

        /// <summary>
        /// Fix FileTypeId for existing "Shipment Invoice" mapping to match MANGO templates (FileTypeId 1147)
        /// </summary>
        [Test]
        [Explicit("Run manually to fix Shipment Invoice FileTypeId")]
        public async Task FixShipmentInvoiceFileTypeId()
        {
            _logger.Information("🔧 **DATABASE_FILETYPE_FIX**: Updating Shipment Invoice mapping to use correct FileTypeId=1147");

            // Check current FileTypeId
            var checkCurrentScript = @"
                SELECT Id, DocumentType, TargetTable, FileTypeId, IsActive
                FROM [dbo].[OCR_TemplateTableMapping]
                WHERE DocumentType = 'Shipment Invoice';
            ";

            await this.ExecuteSqlScript(checkCurrentScript, "Current Shipment Invoice mapping").ConfigureAwait(false);

            // Update FileTypeId to 1147 (matches MANGO templates)
            var updateScript = @"
                UPDATE [dbo].[OCR_TemplateTableMapping] 
                SET FileTypeId = 1147
                WHERE DocumentType = 'Shipment Invoice' AND IsActive = 1;
            ";

            await this.ExecuteSqlScript(updateScript, "Update Shipment Invoice FileTypeId to 1147").ConfigureAwait(false);

            // Verify the update was successful
            var verifyScript = @"
                SELECT Id, DocumentType, TargetTable, FileTypeId, IsActive
                FROM [dbo].[OCR_TemplateTableMapping]
                WHERE DocumentType = 'Shipment Invoice';
            ";

            await this.ExecuteSqlScript(verifyScript, "Verify FileTypeId update").ConfigureAwait(false);
            
            _logger.Information("✅ **FILETYPE_FIX_COMPLETE**: Shipment Invoice FileTypeId updated to 1147");
        }

        /// <summary>
        /// Insert missing "Shipment Invoice" mapping into OCR_TemplateTableMapping table
        /// This enables EntryTypes enum compliance by providing the required database mapping
        /// </summary>
        [Test]
        [Explicit("Run manually to create Shipment Invoice database mapping")]
        public async Task CreateShipmentInvoiceMapping()
        {
            _logger.Information("🔧 **DATABASE_MAPPING_CREATION**: Creating missing 'Shipment Invoice' mapping");

            // First check what mappings currently exist
            var checkExistingScript = @"
                SELECT DocumentType, TargetTable, RequiredFields, OptionalFields, FileTypeId, IsActive
                FROM [dbo].[OCR_TemplateTableMapping]
                WHERE DocumentType LIKE '%Invoice%'
                ORDER BY DocumentType;
            ";

            await this.ExecuteSqlScript(checkExistingScript, "Current Invoice-related mappings").ConfigureAwait(false);

            // Check if 'Shipment Invoice' mapping already exists
            var checkSpecificScript = @"
                SELECT COUNT(*) as ExistingCount
                FROM [dbo].[OCR_TemplateTableMapping]
                WHERE DocumentType = 'Shipment Invoice' AND IsActive = 1;
            ";

            var existingResult = await this.ExecuteSqlScript(checkSpecificScript, "Check if Shipment Invoice mapping exists").ConfigureAwait(false);
            
            if (existingResult.Rows.Count > 0 && Convert.ToInt32(existingResult.Rows[0]["ExistingCount"]) > 0)
            {
                _logger.Information("✅ **MAPPING_EXISTS**: Shipment Invoice mapping already exists - no action needed");
                return;
            }

            // Find the most commonly used FileTypeId to use for our new mapping
            var fileTypeScript = @"
                SELECT TOP 3 FileTypeId, COUNT(*) as UsageCount 
                FROM [dbo].[OCR_TemplateTableMapping] 
                WHERE IsActive = 1
                GROUP BY FileTypeId 
                ORDER BY COUNT(*) DESC;
            ";

            var fileTypeResult = await this.ExecuteSqlScript(fileTypeScript, "Most commonly used FileTypeId values").ConfigureAwait(false);
            
            int fileTypeId = 1;
            if (fileTypeResult.Rows.Count > 0)
            {
                fileTypeId = Convert.ToInt32(fileTypeResult.Rows[0]["FileTypeId"]);
                _logger.Information("🎯 **FILETYPE_SELECTED**: Using FileTypeId={FileTypeId} (most common)", fileTypeId);
            }

            // Create the missing mapping
            var insertScript = @"
                INSERT INTO [dbo].[OCR_TemplateTableMapping] 
                ([DocumentType], [TargetTable], [RequiredFields], [OptionalFields], [FileTypeId], [IsActive], [Keywords], [TemplatePrefix])
                VALUES 
                ('Shipment Invoice', 'ShipmentInvoice', 'InvoiceNo,InvoiceTotal,SupplierCode', 'InvoiceDate,Currency,SubTotal,TotalInternalFreight,TotalOtherCost,TotalInsurance,TotalDeduction', @FileTypeId, 1, 'Shipment,Invoice,Customs,Import,Export,Freight', 'SI');
            ";

            var parameters = new Dictionary<string, object>
            {
                { "FileTypeId", fileTypeId }
            };

            await this.ExecuteSqlScript(insertScript, "Insert Shipment Invoice mapping", parameters).ConfigureAwait(false);

            // Verify the insertion was successful
            var verifyScript = @"
                SELECT DocumentType, TargetTable, RequiredFields, OptionalFields, FileTypeId, IsActive
                FROM [dbo].[OCR_TemplateTableMapping]
                WHERE DocumentType = 'Shipment Invoice';
            ";

            await this.ExecuteSqlScript(verifyScript, "Verify Shipment Invoice mapping creation").ConfigureAwait(false);
            
            _logger.Information("✅ **MAPPING_CREATION_COMPLETE**: Shipment Invoice mapping successfully created");
        }

        #endregion
    }
}