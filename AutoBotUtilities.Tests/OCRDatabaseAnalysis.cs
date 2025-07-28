using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using NUnit.Framework;
using Serilog;

namespace AutoBotUtilities.Tests
{
    /// <summary>
    /// Standalone OCR database analysis for template specification integration
    /// Executes critical SQL queries to analyze database structure and EntityType patterns
    /// </summary>
    [TestFixture]
    public class OCRDatabaseAnalysis
    {
        private static ILogger _logger;
        private readonly string _connectionString = "data source=MINIJOE\\SQLDEVELOPER2022;initial catalog=WebSource-AutoBot;user=sa;password=pa$word;Connect Timeout=30;MultipleActiveResultSets=True";

        [OneTimeSetUp]
        public void Setup()
        {
            _logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();

            _logger.Information("🔍 **OCR_DATABASE_ANALYSIS**: Initializing database analysis for template specification integration");
        }

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

            await ExecuteSqlScript(templateMappingStructureScript, "OCR_TemplateTableMapping table structure").ConfigureAwait(false);

            // 2. Check current OCR_TemplateTableMapping data
            var templateMappingDataScript = @"
                SELECT * FROM [WebSource-AutoBot].[dbo].[OCR_TemplateTableMapping];
            ";

            await ExecuteSqlScript(templateMappingDataScript, "Current OCR_TemplateTableMapping data").ConfigureAwait(false);

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

            await ExecuteSqlScript(fileTypesScript, "FileTypes-FileImporterInfo DocumentType mappings").ConfigureAwait(false);

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

            await ExecuteSqlScript(columnExistsScript, "FileTypeId column existence check").ConfigureAwait(false);

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

            await ExecuteSqlScript(entityTypeAnalysisScript, "OCR-PartLineFields EntityType analysis").ConfigureAwait(false);

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

            await ExecuteSqlScript(relationshipsScript, "Current FileType to OCR template relationships").ConfigureAwait(false);

            // 7. Get all OCR and FileType related tables
            var ocrTablesScript = @"
                SELECT TABLE_NAME 
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_SCHEMA = 'dbo' 
                AND (TABLE_NAME LIKE '%OCR%' OR TABLE_NAME LIKE '%FileType%' OR TABLE_NAME LIKE '%Template%')
                ORDER BY TABLE_NAME;
            ";

            await ExecuteSqlScript(ocrTablesScript, "All OCR and FileType related tables").ConfigureAwait(false);

            _logger.Information("✅ **OCR_TEMPLATE_ANALYSIS**: Database analysis completed successfully");
        }

        /// <summary>
        /// Execute a SQL script with parameters
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
    }
}