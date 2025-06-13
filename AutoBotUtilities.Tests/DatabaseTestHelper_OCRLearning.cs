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
    /// Database test helper specifically for OCR Learning investigation
    /// </summary>
    [TestFixture]
    public class DatabaseTestHelper_OCRLearning
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
                _logger.Information("📡 **DATABASE_CONNECTION**: Connected to OCR database");
            }
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
                await connection.OpenAsync();
                
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

        /// <summary>
        /// Check what OCR learning-related tables exist
        /// </summary>
        [Test]
        public async Task CheckOCRLearningTables()
        {
            _logger.Information("🔍 **OCR_TABLES**: Checking what OCR learning-related tables exist");

            var tableScript = @"
                SELECT 
                    TABLE_NAME,
                    TABLE_TYPE
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_NAME LIKE '%OCR%' 
                   OR TABLE_NAME LIKE '%Learning%'
                   OR TABLE_NAME LIKE '%Correction%'
                ORDER BY TABLE_NAME;
            ";

            await ExecuteSqlScript(tableScript, "OCR learning-related tables");
        }

        /// <summary>
        /// Get the actual structure of OCRCorrectionLearning table
        /// </summary>
        [Test]
        public async Task GetOCRCorrectionLearningStructure()
        {
            _logger.Information("🏗️ **TABLE_STRUCTURE**: Getting OCRCorrectionLearning table structure");

            var structureScript = @"
                IF OBJECT_ID('OCRCorrectionLearning', 'U') IS NOT NULL
                BEGIN
                    SELECT 
                        COLUMN_NAME,
                        DATA_TYPE,
                        IS_NULLABLE,
                        COLUMN_DEFAULT,
                        CHARACTER_MAXIMUM_LENGTH
                    FROM INFORMATION_SCHEMA.COLUMNS
                    WHERE TABLE_NAME = 'OCRCorrectionLearning'
                    ORDER BY ORDINAL_POSITION;
                END
                ELSE
                BEGIN
                    SELECT 'OCRCorrectionLearning table does not exist' AS Message;
                END
            ";

            await ExecuteSqlScript(structureScript, "OCRCorrectionLearning table structure");
        }

        /// <summary>
        /// Check OCRCorrectionLearning table contents with actual column names
        /// </summary>
        [Test]
        public async Task CheckOCRCorrectionLearningContents()
        {
            _logger.Information("📊 **OCR_DATA**: Checking OCRCorrectionLearning table contents");

            var contentsScript = @"
                IF OBJECT_ID('OCRCorrectionLearning', 'U') IS NOT NULL
                BEGIN
                    -- First, get row count
                    SELECT COUNT(*) AS TotalRows FROM OCRCorrectionLearning;
                    
                    -- Then get recent records (using SELECT * to see all columns)
                    SELECT TOP 20 * FROM OCRCorrectionLearning ORDER BY Id DESC;
                END
                ELSE
                BEGIN
                    SELECT 'OCRCorrectionLearning table does not exist' AS Message;
                END
            ";

            await ExecuteSqlScript(contentsScript, "OCRCorrectionLearning table contents");
        }

        /// <summary>
        /// Check recent database activity for Amazon template (InvoiceId 5)
        /// </summary>
        [Test]
        public async Task CheckRecentAmazonActivity()
        {
            _logger.Information("🔍 **AMAZON_ACTIVITY**: Checking recent database activity for Amazon template");

            var activityScript = @"
                -- Check for recent RegularExpressions changes
                SELECT 
                    'Recent RegularExpressions' AS ActivityType,
                    r.Id,
                    r.RegEx,
                    r.Description,
                    r.Created,
                    l.Name AS LineName
                FROM RegularExpressions r
                LEFT JOIN Lines l ON r.Id = l.RegExId
                LEFT JOIN Parts p ON l.PartId = p.Id
                WHERE p.TemplateId = 5 
                  AND r.Created > DATEADD(day, -7, GETDATE())
                
                UNION ALL
                
                -- Check for recent Fields changes (if they have Created dates)
                SELECT 
                    'Recent Fields' AS ActivityType,
                    f.Id,
                    f.Field,
                    f.EntityType,
                    ISNULL(CAST(f.Created AS DATETIME), '1900-01-01') AS Created,
                    l.Name AS LineName
                FROM Fields f
                INNER JOIN Lines l ON f.LineId = l.Id
                INNER JOIN Parts p ON l.PartId = p.Id
                WHERE p.TemplateId = 5 
                  AND f.Created > DATEADD(day, -7, GETDATE())
                
                ORDER BY Created DESC;
            ";

            await ExecuteSqlScript(activityScript, "Recent Amazon template activity");
        }

        /// <summary>
        /// Look for any Amazon-related patterns containing "Gift Card" or "Free Shipping"
        /// </summary>
        [Test]
        public async Task SearchForAmazonPatterns()
        {
            _logger.Information("🎯 **AMAZON_PATTERNS**: Searching for Gift Card and Free Shipping patterns");

            var patternScript = @"
                -- Search for Gift Card patterns
                SELECT 
                    'Gift Card Pattern' AS PatternType,
                    r.Id AS RegexId,
                    r.RegEx,
                    r.Description,
                    r.Created,
                    l.Id AS LineId,
                    l.Name AS LineName,
                    f.Field AS FieldName,
                    p.Name AS PartName
                FROM RegularExpressions r
                LEFT JOIN Lines l ON r.Id = l.RegExId
                LEFT JOIN Parts p ON l.PartId = p.Id
                LEFT JOIN Fields f ON l.Id = f.LineId
                WHERE (r.RegEx LIKE '%Gift Card%' OR r.RegEx LIKE '%gift card%' OR r.Description LIKE '%Gift Card%')
                  AND (p.TemplateId = 5 OR p.TemplateId IS NULL)
                
                UNION ALL
                
                -- Search for Free Shipping patterns
                SELECT 
                    'Free Shipping Pattern' AS PatternType,
                    r.Id AS RegexId,
                    r.RegEx,
                    r.Description,
                    r.Created,
                    l.Id AS LineId,
                    l.Name AS LineName,
                    f.Field AS FieldName,
                    p.Name AS PartName
                FROM RegularExpressions r
                LEFT JOIN Lines l ON r.Id = l.RegExId
                LEFT JOIN Parts p ON l.PartId = p.Id
                LEFT JOIN Fields f ON l.Id = f.LineId
                WHERE (r.RegEx LIKE '%Free Shipping%' OR r.RegEx LIKE '%free shipping%' OR r.Description LIKE '%Free Shipping%')
                  AND (p.TemplateId = 5 OR p.TemplateId IS NULL)
                
                ORDER BY PatternType, Created DESC;
            ";

            await ExecuteSqlScript(patternScript, "Amazon Gift Card and Free Shipping patterns");
        }

        /// <summary>
        /// Check for TotalInsurance and TotalDeduction field mappings in Amazon template
        /// </summary>
        [Test]
        public async Task CheckAmazonFieldMappings()
        {
            _logger.Information("🗺️ **FIELD_MAPPINGS**: Checking Amazon template field mappings for Caribbean customs");

            var mappingScript = @"
                SELECT 
                    f.Id AS FieldId,
                    f.Field AS FieldName,
                    f.EntityType,
                    f.DataType,
                    f.AppendValues,
                    l.Id AS LineId,
                    l.Name AS LineName,
                    r.Id AS RegexId,
                    r.RegEx,
                    p.Name AS PartName,
                    CASE 
                        WHEN f.Field = 'TotalInsurance' THEN 'Customer Reduction (Gift Cards, negative values)'
                        WHEN f.Field = 'TotalDeduction' THEN 'Supplier Reduction (Free Shipping, positive values)'
                        WHEN f.Field = 'TotalOtherCost' THEN 'Other Costs (should NOT be used for Gift Cards)'
                        ELSE 'Other Field'
                    END AS CaribbeanCustomsRole
                FROM Fields f
                INNER JOIN Lines l ON f.LineId = l.Id
                INNER JOIN Parts p ON l.PartId = p.Id
                LEFT JOIN RegularExpressions r ON l.RegExId = r.Id
                WHERE p.TemplateId = 5 
                  AND f.Field IN ('TotalInsurance', 'TotalDeduction', 'TotalOtherCost')
                ORDER BY f.Field, l.Name;
            ";

            await ExecuteSqlScript(mappingScript, "Amazon field mappings for Caribbean customs compliance");
        }
    }
}