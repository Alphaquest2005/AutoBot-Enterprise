using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;

namespace AutoBotUtilities.Tests
{
    /// <summary>
    /// Standalone OCR database analysis for template specification integration
    /// Console application approach that bypasses NUnit dependencies
    /// </summary>
    public class OCRDatabaseAnalysisStandalone
    {
        private readonly string _connectionString = "data source=MINIJOE\\SQLDEVELOPER2022;initial catalog=WebSource-AutoBot;user=sa;password=pa$$word;Connect Timeout=30;MultipleActiveResultSets=True";

        public static async Task Main(string[] args)
        {
            var analyzer = new OCRDatabaseAnalysisStandalone();
            await analyzer.RunAnalysis();
        }

        public async Task RunAnalysis()
        {
            Console.WriteLine("🔍 **OCR_TEMPLATE_ANALYSIS**: Starting comprehensive OCR template structure analysis");

            try
            {
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

                await ExecuteSqlScript(templateMappingStructureScript, "OCR_TemplateTableMapping table structure");

                // 2. Check current OCR_TemplateTableMapping data
                var templateMappingDataScript = @"
                    SELECT * FROM [WebSource-AutoBot].[dbo].[OCR_TemplateTableMapping];
                ";

                await ExecuteSqlScript(templateMappingDataScript, "Current OCR_TemplateTableMapping data");

                // 3. First check FileTypes-FileImporterInfo structure
                var fileTypesStructureScript = @"
                    SELECT 
                        COLUMN_NAME, 
                        DATA_TYPE, 
                        IS_NULLABLE, 
                        COLUMN_DEFAULT
                    FROM INFORMATION_SCHEMA.COLUMNS 
                    WHERE TABLE_NAME = 'FileTypes-FileImporterInfo' 
                    AND TABLE_SCHEMA = 'dbo'
                    ORDER BY ORDINAL_POSITION;
                ";

                await ExecuteSqlScript(fileTypesStructureScript, "FileTypes-FileImporterInfo table structure");

                // 4. Check FileTypes-FileImporterInfo data (DocumentType enum mappings)
                var fileTypesScript = @"
                    SELECT * FROM [WebSource-AutoBot].[dbo].[FileTypes-FileImporterInfo]
                    ORDER BY EntryType;
                ";

                await ExecuteSqlScript(fileTypesScript, "FileTypes-FileImporterInfo DocumentType mappings");

                // 5. Check if FileTypeId column exists in OCR_TemplateTableMapping
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

                await ExecuteSqlScript(columnExistsScript, "FileTypeId column existence check");

                // 6. Check OCR-PartLineFields view for EntityType analysis
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

                await ExecuteSqlScript(entityTypeAnalysisScript, "OCR-PartLineFields EntityType analysis");

                // 7. Check current database relationships 
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

                await ExecuteSqlScript(relationshipsScript, "Current FileType to OCR template relationships");

                // 8. Get all OCR and FileType related tables
                var ocrTablesScript = @"
                    SELECT TABLE_NAME 
                    FROM INFORMATION_SCHEMA.TABLES 
                    WHERE TABLE_SCHEMA = 'dbo' 
                    AND (TABLE_NAME LIKE '%OCR%' OR TABLE_NAME LIKE '%FileType%' OR TABLE_NAME LIKE '%Template%')
                    ORDER BY TABLE_NAME;
                ";

                await ExecuteSqlScript(ocrTablesScript, "All OCR and FileType related tables");

                Console.WriteLine("✅ **OCR_TEMPLATE_ANALYSIS**: Database analysis completed successfully");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"❌ **OCR_TEMPLATE_ANALYSIS_ERROR**: {ex.Message}");
                Console.WriteLine($"**STACK_TRACE**: {ex.StackTrace}");
            }
        }

        /// <summary>
        /// Execute a SQL script and display results
        /// </summary>
        public async Task<DataTable> ExecuteSqlScript(string script, string description = null)
        {
            Console.WriteLine($"🔍 **SQL_EXECUTION**: {description ?? "Executing SQL script"}");
            Console.WriteLine($"📄 **SQL_SCRIPT**: {script}");

            using (var connection = new SqlConnection(_connectionString))
            {
                await connection.OpenAsync();
                
                using (var command = new SqlCommand(script, connection))
                {
                    using (var adapter = new SqlDataAdapter(command))
                    {
                        var dataTable = new DataTable();
                        adapter.Fill(dataTable);

                        Console.WriteLine($"📊 **SQL_RESULT**: Returned {dataTable.Rows.Count} rows, {dataTable.Columns.Count} columns");

                        // Log column names
                        var columnNames = dataTable.Columns.Cast<DataColumn>().Select(c => c.ColumnName).ToArray();
                        Console.WriteLine($"📋 **SQL_COLUMNS**: {string.Join(", ", columnNames)}");

                        // Log all rows for verification
                        for (int i = 0; i < dataTable.Rows.Count; i++)
                        {
                            var row = dataTable.Rows[i];
                            var values = row.ItemArray.Select(field => field?.ToString() ?? "NULL").ToArray();
                            Console.WriteLine($"📝 **SQL_ROW_{i + 1}**: {string.Join(" | ", values)}");
                        }

                        Console.WriteLine("");
                        return dataTable;
                    }
                }
            }
        }
    }
}