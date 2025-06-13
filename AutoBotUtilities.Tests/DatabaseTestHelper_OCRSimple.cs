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
    /// Simple database test helper for OCR investigation
    /// </summary>
    [TestFixture]
    public class DatabaseTestHelper_OCRSimple
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
                        for (int i = 0; i < Math.Min(20, dataTable.Rows.Count); i++)
                        {
                            var row = dataTable.Rows[i];
                            var values = row.ItemArray.Select(field => field?.ToString() ?? "NULL").ToArray();
                            _logger.Information("📝 **SQL_ROW_{RowIndex}**: {Values}", i + 1, string.Join(" | ", values));
                        }

                        if (dataTable.Rows.Count > 20)
                        {
                            _logger.Information("📝 **SQL_MORE_ROWS**: ... and {MoreRows} more rows", dataTable.Rows.Count - 20);
                        }

                        return dataTable;
                    }
                }
            }
        }

        /// <summary>
        /// Show recent OCR corrections using all available columns
        /// </summary>
        [Test]
        public async Task ShowRecentOCRCorrections()
        {
            _logger.Information("📊 **RECENT_CORRECTIONS**: Showing recent OCR corrections with all columns");

            var recentScript = @"
                SELECT TOP 20 * 
                FROM OCRCorrectionLearning 
                ORDER BY Id DESC;
            ";

            await ExecuteSqlScript(recentScript, "Recent OCR corrections - all columns");
        }

        /// <summary>
        /// Search for Amazon or Gift Card related corrections
        /// </summary>
        [Test]
        public async Task SearchAmazonGiftCardCorrections()
        {
            _logger.Information("🎁 **AMAZON_SEARCH**: Searching for Amazon/Gift Card corrections");

            var searchScript = @"
                SELECT TOP 50 * 
                FROM OCRCorrectionLearning 
                WHERE LineText LIKE '%Gift Card%'
                   OR LineText LIKE '%Amazon%'
                   OR LineText LIKE '%Free Shipping%'
                   OR FieldName = 'TotalInsurance'
                   OR FieldName = 'TotalDeduction'
                   OR CorrectValue LIKE '%6.99%'
                   OR CorrectValue LIKE '%-6.99%'
                ORDER BY Id DESC;
            ";

            await ExecuteSqlScript(searchScript, "Amazon/Gift Card corrections search");
        }

        /// <summary>
        /// Get correction statistics
        /// </summary>
        [Test]
        public async Task GetCorrectionStatistics()
        {
            _logger.Information("📈 **STATS**: Getting OCR correction statistics");

            var statsScript = @"
                SELECT 
                    FieldName,
                    COUNT(*) AS Total,
                    COUNT(DISTINCT SUBSTRING(LineText, 1, 50)) AS UniqueLineTexts
                FROM OCRCorrectionLearning 
                GROUP BY FieldName
                ORDER BY Total DESC;
            ";

            await ExecuteSqlScript(statsScript, "Correction statistics by field");
        }

        /// <summary>
        /// Search for recent corrections by any date field available
        /// </summary>
        [Test]
        public async Task SearchRecentCorrections()
        {
            _logger.Information("🕒 **RECENT_SEARCH**: Searching for recent corrections");

            var recentScript = @"
                -- Try to find recent corrections using any available date field
                SELECT TOP 30 
                    Id, 
                    FieldName, 
                    SUBSTRING(LineText, 1, 100) AS LineTextPreview,
                    SUBSTRING(CorrectValue, 1, 50) AS CorrectValuePreview,
                    CorrectionType
                FROM OCRCorrectionLearning 
                WHERE Id > (SELECT MAX(Id) - 100 FROM OCRCorrectionLearning)  -- Last 100 records
                ORDER BY Id DESC;
            ";

            await ExecuteSqlScript(recentScript, "Recent corrections by ID");
        }
    }
}