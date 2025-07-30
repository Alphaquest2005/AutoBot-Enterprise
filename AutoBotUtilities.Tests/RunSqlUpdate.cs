using System;
using System.Data.SqlClient;
using System.IO;
using System.Threading.Tasks;
using NUnit.Framework;
using Serilog;
using CoreEntities.Business.Entities;

namespace AutoBotUtilities.Tests
{
    /// <summary>
    /// Execute SQL script to update MANGO template patterns
    /// </summary>
    [TestFixture]
    public class RunSqlUpdate
    {
        private static ILogger _logger;

        [OneTimeSetUp]
        public void Setup()
        {
            _logger = new LoggerConfiguration()
                .WriteTo.Console()
                .CreateLogger();
        }

        [Test]
        public async Task UpdateMangoTemplatePatterns()
        {
            _logger.Information("🔧 **SQL_UPDATE_START**: Updating MANGO template regex patterns");

            using (var context = new CoreEntitiesContext())
            {
                if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                    await context.Database.Connection.OpenAsync().ConfigureAwait(false);

                // Execute each update individually to handle scope issues
                await ExecuteUpdate(context, "InvoiceNo", 5444, @"order\s+(?<InvoiceNo>[A-Za-z0-9]+)\s+shortly").ConfigureAwait(false);
                await ExecuteUpdate(context, "InvoiceDate", 5445, @"(?<InvoiceDate>\w+,\s+\w+\s+\d{1,2},\s+\d{4})").ConfigureAwait(false);
                await ExecuteUpdate(context, "Currency", 5447, @"(?<Currency>US[S$])").ConfigureAwait(false);
                await ExecuteUpdate(context, "SubTotal", 5448, @"Subtotal\s+US[S$]\s*(?<SubTotal>\d+\.\d{2})").ConfigureAwait(false);
                await ExecuteUpdate(context, "InvoiceTotal", 5449, @"TOTAL\s+AMOUNT\s+US\$\s*(?<InvoiceTotal>\d+\.\d{2})").ConfigureAwait(false);

                // Update existing SupplierName pattern
                await UpdateExistingPattern(context, "SupplierName", 5446, @"From:\s*(?<SupplierName>MANGO\s+OUTLET)").ConfigureAwait(false);

                // Verify all updates
                await VerifyUpdates(context).ConfigureAwait(false);
            }

            _logger.Information("✅ **SQL_UPDATE_COMPLETE**: MANGO template patterns updated successfully");
        }

        private async Task ExecuteUpdate(CoreEntitiesContext context, string fieldName, int lineId, string pattern)
        {
            _logger.Information("🔧 **UPDATING_FIELD**: {FieldName} for Line {LineId}", fieldName, lineId);

            // Insert new regex pattern
            var insertRegexScript = @"
                INSERT INTO RegularExpressions (RegEx, CreatedBy, CreatedDate)
                VALUES (@Pattern, 'SYSTEM', GETUTCDATE());
                SELECT SCOPE_IDENTITY();";

            int regexId;
            using (var command = new SqlCommand(insertRegexScript, context.Database.Connection as SqlConnection))
            {
                command.Parameters.AddWithValue("@Pattern", pattern);
                var result = await command.ExecuteScalarAsync().ConfigureAwait(false);
                regexId = Convert.ToInt32(result);
            }

            // Update the line to use the new regex
            var updateLineScript = @"
                UPDATE Lines 
                SET RegularExpressionsId = @RegexId
                WHERE Id = @LineId;";

            using (var command = new SqlCommand(updateLineScript, context.Database.Connection as SqlConnection))
            {
                command.Parameters.AddWithValue("@RegexId", regexId);
                command.Parameters.AddWithValue("@LineId", lineId);
                int rowsAffected = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                _logger.Information("✅ **FIELD_UPDATED**: {FieldName} - RegexId={RegexId}, RowsAffected={Rows}", fieldName, regexId, rowsAffected);
            }
        }

        private async Task UpdateExistingPattern(CoreEntitiesContext context, string fieldName, int lineId, string pattern)
        {
            _logger.Information("🔧 **UPDATING_EXISTING**: {FieldName} for Line {LineId}", fieldName, lineId);

            var updateScript = @"
                UPDATE RegularExpressions 
                SET RegEx = @Pattern
                WHERE Id IN (
                    SELECT RegularExpressionsId 
                    FROM Lines 
                    WHERE Id = @LineId 
                    AND RegularExpressionsId IS NOT NULL
                );";

            using (var command = new SqlCommand(updateScript, context.Database.Connection as SqlConnection))
            {
                command.Parameters.AddWithValue("@Pattern", pattern);
                command.Parameters.AddWithValue("@LineId", lineId);
                int rowsAffected = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                _logger.Information("✅ **EXISTING_UPDATED**: {FieldName} - RowsAffected={Rows}", fieldName, rowsAffected);
            }
        }

        private async Task VerifyUpdates(CoreEntitiesContext context)
        {
            _logger.Information("🔍 **VERIFICATION**: Checking updated patterns for Template 1339");

            var verifyScript = @"
                SELECT 
                    l.Id as LineId,
                    l.Name as LineName,
                    r.Id as RegExId,
                    r.RegEx as Pattern
                FROM Lines l
                INNER JOIN Parts p ON l.PartId = p.Id
                INNER JOIN Templates t ON p.TemplateId = t.Id
                LEFT JOIN RegularExpressions r ON l.RegularExpressionsId = r.Id
                WHERE t.Id = 1339
                ORDER BY l.Id;";

            using (var command = new SqlCommand(verifyScript, context.Database.Connection as SqlConnection))
            {
                using (var reader = await command.ExecuteReaderAsync().ConfigureAwait(false))
                {
                    while (await reader.ReadAsync().ConfigureAwait(false))
                    {
                        var lineId = reader.GetInt32(0); // LineId column
                        var lineName = reader.IsDBNull(1) ? "NULL" : reader.GetString(1); // LineName column
                        var regexId = reader.IsDBNull(2) ? "NULL" : reader.GetInt32(2).ToString(); // RegExId column
                        var pattern = reader.IsDBNull(3) ? "NO_REGEX" : reader.GetString(3); // Pattern column

                        _logger.Information("📋 **LINE_PATTERN**: {LineId} | {LineName} | RegExId={RegexId} | Pattern={Pattern}", 
                            lineId, lineName, regexId, pattern);
                    }
                }
            }
        }
    }
}