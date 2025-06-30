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
    /// Simple database test helper for OCR Learning investigation
    /// </summary>
    [TestFixture]
    public class DatabaseTestHelper_OCRLearning_Simple
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

        /// <summary>
        /// Find all table names that contain "regex" or "RegEx"
        /// </summary>
        [Test]
        public async Task FindRegexTables()
        {
            _logger.Information("🔍 **REGEX_TABLES**: Finding all tables related to regex patterns");

            var tableScript = @"
                SELECT 
                    TABLE_NAME,
                    TABLE_TYPE
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_NAME LIKE '%regex%' 
                   OR TABLE_NAME LIKE '%RegEx%'
                   OR TABLE_NAME LIKE '%Regular%'
                ORDER BY TABLE_NAME;
            ";

            await this.ExecuteSqlScript(tableScript, "Regex-related tables").ConfigureAwait(false);
        }

        /// <summary>
        /// Check recent OCR correction learning entries with current column names
        /// </summary>
        [Test]
        public async Task CheckRecentOCRCorrections()
        {
            _logger.Information("📊 **RECENT_CORRECTIONS**: Checking recent OCR corrections");

            var recentScript = @"
                SELECT TOP 20 
                    Id,
                    FieldName,
                    OriginalError,
                    CorrectValue,
                    CorrectionType,
                    CreatedOn,
                    InvoiceNumber,
                    LineText
                FROM OCRCorrectionLearning 
                ORDER BY Id DESC;
            ";

            await this.ExecuteSqlScript(recentScript, "Recent OCR corrections").ConfigureAwait(false);
        }

        /// <summary>
        /// Search for Amazon-related corrections (112-9126443-1163432)
        /// </summary>
        [Test]
        public async Task SearchAmazonCorrections()
        {
            _logger.Information("🎯 **AMAZON_CORRECTIONS**: Searching for Amazon invoice corrections");

            var amazonScript = @"
                SELECT 
                    Id,
                    FieldName,
                    OriginalError,
                    CorrectValue,
                    CorrectionType,
                    CreatedOn,
                    InvoiceNumber,
                    LineText
                FROM OCRCorrectionLearning 
                WHERE InvoiceNumber LIKE '%112-9126443-1163432%'
                   OR InvoiceNumber LIKE '%Amazon%'
                   OR FieldName IN ('TotalInsurance', 'TotalDeduction')
                   OR LineText LIKE '%Gift Card%'
                   OR LineText LIKE '%Free Shipping%'
                ORDER BY CreatedOn DESC;
            ";

            await this.ExecuteSqlScript(amazonScript, "Amazon invoice corrections").ConfigureAwait(false);
        }

        /// <summary>
        /// Get correction counts by field name
        /// </summary>
        [Test]
        public async Task GetCorrectionCountsByField()
        {
            _logger.Information("📈 **CORRECTION_STATS**: Getting correction statistics by field");

            var statsScript = @"
                SELECT 
                    FieldName,
                    COUNT(*) AS CorrectionCount,
                    MIN(CreatedOn) AS EarliestCorrection,
                    MAX(CreatedOn) AS LatestCorrection
                FROM OCRCorrectionLearning 
                GROUP BY FieldName
                ORDER BY CorrectionCount DESC;
            ";

            await this.ExecuteSqlScript(statsScript, "Correction statistics by field").ConfigureAwait(false);
        }

        /// <summary>
        /// Search for Gift Card and Free Shipping corrections specifically
        /// </summary>
        [Test]
        public async Task SearchGiftCardAndFreeShippingCorrections()
        {
            _logger.Information("🎁 **GIFT_CARD_CORRECTIONS**: Searching for Gift Card and Free Shipping corrections");

            var giftCardScript = @"
                SELECT 
                    Id,
                    FieldName,
                    OriginalError,
                    CorrectValue,
                    CorrectionType,
                    CreatedOn,
                    InvoiceNumber,
                    SUBSTRING(LineText, 1, 100) AS LineTextPreview
                FROM OCRCorrectionLearning 
                WHERE LineText LIKE '%Gift Card%'
                   OR LineText LIKE '%Free Shipping%'
                   OR FieldName = 'TotalInsurance'
                   OR FieldName = 'TotalDeduction'
                   OR CorrectValue LIKE '%-6.99%'
                   OR CorrectValue LIKE '%6.99%'
                ORDER BY CreatedOn DESC;
            ";

            await this.ExecuteSqlScript(giftCardScript, "Gift Card and Free Shipping corrections").ConfigureAwait(false);
        }
    }
}