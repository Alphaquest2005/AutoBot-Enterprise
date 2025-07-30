using System;
using System.Data;
using System.Data.SqlClient;
using System.Threading.Tasks;
using NUnit.Framework;
using Serilog;
using CoreEntities.Business.Entities;

namespace AutoBotUtilities.Tests
{
    /// <summary>
    /// Fix MANGO template regex patterns to match actual OCR text format
    /// </summary>
    [TestFixture]
    public class FixMangoRegexPatterns
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
        public async Task CheckAndUpdateMangoPatterns()
        {
            _logger.Information("🔧 **MANGO_PATTERN_FIX**: Starting MANGO template pattern analysis and update");

            using (var context = new CoreEntitiesContext())
            {
                if (context.Database.Connection.State != ConnectionState.Open)
                    await context.Database.Connection.OpenAsync().ConfigureAwait(false);

                // Step 1: Check OCR table structure
                await CheckDatabaseStructure(context).ConfigureAwait(false);

                // Step 2: Find MANGO template
                await FindMangoTemplate(context).ConfigureAwait(false);

                // Step 3: Update regex patterns
                await UpdateRegexPatterns(context).ConfigureAwait(false);
            }

            _logger.Information("✅ **MANGO_PATTERN_FIX**: Pattern update completed successfully");
        }

        private async Task CheckDatabaseStructure(CoreEntitiesContext context)
        {
            _logger.Information("🔍 **STRUCTURE_CHECK**: Checking database structure for OCR tables");

            var checkTablesScript = @"
                SELECT TABLE_NAME 
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_SCHEMA = 'dbo' 
                AND (TABLE_NAME LIKE '%Template%' OR TABLE_NAME LIKE '%RegEx%' OR TABLE_NAME LIKE '%Part%' OR TABLE_NAME LIKE '%Line%' OR TABLE_NAME LIKE '%Field%')
                ORDER BY TABLE_NAME;";

            await ExecuteQuery(context, checkTablesScript, "Available OCR-related tables").ConfigureAwait(false);
        }

        private async Task FindMangoTemplate(CoreEntitiesContext context)
        {
            _logger.Information("🔍 **TEMPLATE_SEARCH**: Looking for MANGO template in database");

            var findTemplateScript = @"
                -- Check Templates table
                SELECT TOP 5 Id, Name, IsActive
                FROM Templates 
                WHERE Name LIKE '%MANGO%' OR Name LIKE '%03152025%' OR Name LIKE '%TOTAL_AMOUNT%'
                ORDER BY Id DESC;";

            await ExecuteQuery(context, findTemplateScript, "MANGO template search").ConfigureAwait(false);

            // Also check with ID 1339 from test logs
            var checkTemplate1339Script = @"
                SELECT Id, Name, IsActive
                FROM Templates 
                WHERE Id = 1339;";

            await ExecuteQuery(context, checkTemplate1339Script, "Template ID 1339 check").ConfigureAwait(false);
        }

        private async Task UpdateRegexPatterns(CoreEntitiesContext context)
        {
            _logger.Information("🔧 **PATTERN_UPDATE**: Updating regex patterns for MANGO template");

            // First, get the template structure
            var getStructureScript = @"
                SELECT 
                    t.Id as TemplateId, t.Name as TemplateName,
                    p.Id as PartId, p.Name as PartName,
                    l.Id as LineId, l.Name as LineName, l.RegularExpressionsId,
                    f.Id as FieldId, f.Field as FieldName,
                    r.Id as RegExId, r.RegEx as CurrentPattern
                FROM Templates t
                INNER JOIN Parts p ON t.Id = p.TemplateId
                INNER JOIN Lines l ON p.Id = l.PartId
                INNER JOIN Fields f ON l.Id = f.LineId
                LEFT JOIN RegularExpressions r ON l.RegularExpressionsId = r.Id
                WHERE t.Name = '03152025_TOTAL_AMOUNT_GENERIC_DOCUMENT' OR t.Id = 1339
                ORDER BY f.Field;";

            var structureData = await ExecuteQuery(context, getStructureScript, "Template structure analysis").ConfigureAwait(false);

            if (structureData.Rows.Count > 0)
            {
                _logger.Information("📋 **STRUCTURE_FOUND**: Found {Count} field mappings in MANGO template", structureData.Rows.Count);

                // Update patterns based on our corrected patterns
                await UpdateSpecificPatterns(context).ConfigureAwait(false);
            }
            else
            {
                _logger.Information("⚠️ **NO_STRUCTURE**: MANGO template structure not found - may need to create template first");
            }
        }

        private async Task UpdateSpecificPatterns(CoreEntitiesContext context)
        {
            _logger.Information("🎯 **SPECIFIC_UPDATES**: Applying corrected regex patterns to database");

            // Define the corrected patterns
            var patternUpdates = new[]
            {
                new { Field = "InvoiceNo", Pattern = @"order\s+(?<InvoiceNo>[A-Za-z0-9]+)\s+shortly" },
                new { Field = "SupplierName", Pattern = @"(?<SupplierName>MANGO\s+OUTLET)" },
                new { Field = "InvoiceDate", Pattern = @"(?<InvoiceDate>\w+,\s+\w+\s+\d{1,2},\s+\d{4})" },
                new { Field = "SubTotal", Pattern = @"Subtotal\s+US[S$]\s*(?<SubTotal>\d+\.\d{2})" },
                new { Field = "InvoiceTotal", Pattern = @"TOTAL\s+AMOUNT\s+US\$\s*(?<InvoiceTotal>\d+\.\d{2})" },
                new { Field = "TotalOtherCost", Pattern = @"Estimated\s+Tax\s+US\$\s*(?<TotalOtherCost>\d+\.\d{2})" }
            };

            foreach (var update in patternUpdates)
            {
                var updateScript = $@"
                    UPDATE RegularExpressions 
                    SET RegEx = @Pattern
                    WHERE Id IN (
                        SELECT l.RegularExpressionsId
                        FROM Templates t
                        INNER JOIN Parts p ON t.Id = p.TemplateId
                        INNER JOIN Lines l ON p.Id = l.PartId
                        INNER JOIN Fields f ON l.Id = f.LineId
                        WHERE (t.Name = '03152025_TOTAL_AMOUNT_GENERIC_DOCUMENT' OR t.Id = 1339)
                        AND f.Field = @Field
                        AND l.RegularExpressionsId IS NOT NULL
                    );";

                using (var command = new SqlCommand(updateScript, context.Database.Connection as SqlConnection))
                {
                    command.Parameters.AddWithValue("@Pattern", update.Pattern);
                    command.Parameters.AddWithValue("@Field", update.Field);

                    int rowsAffected = await command.ExecuteNonQueryAsync().ConfigureAwait(false);
                    _logger.Information("✅ **PATTERN_UPDATED**: {Field} pattern updated - {Rows} rows affected", update.Field, rowsAffected);
                }
            }

            // Verify the updates
            var verifyScript = @"
                SELECT 
                    f.Field as FieldName,
                    r.RegEx as UpdatedPattern
                FROM Templates t
                INNER JOIN Parts p ON t.Id = p.TemplateId
                INNER JOIN Lines l ON p.Id = l.PartId
                INNER JOIN Fields f ON l.Id = f.LineId
                LEFT JOIN RegularExpressions r ON l.RegularExpressionsId = r.Id
                WHERE (t.Name = '03152025_TOTAL_AMOUNT_GENERIC_DOCUMENT' OR t.Id = 1339)
                ORDER BY f.Field;";

            await ExecuteQuery(context, verifyScript, "Pattern update verification").ConfigureAwait(false);
        }

        private async Task<DataTable> ExecuteQuery(CoreEntitiesContext context, string query, string description)
        {
            _logger.Information("🔍 **SQL_EXECUTION**: {Description}", description);
            _logger.Information("📄 **SQL_QUERY**: {Query}", query);

            using (var command = new SqlCommand(query, context.Database.Connection as SqlConnection))
            {
                using (var adapter = new SqlDataAdapter(command))
                {
                    var dataTable = new DataTable();
                    adapter.Fill(dataTable);

                    _logger.Information("📊 **SQL_RESULT**: {Rows} rows, {Columns} columns", dataTable.Rows.Count, dataTable.Columns.Count);

                    // Log results
                    for (int i = 0; i < Math.Min(10, dataTable.Rows.Count); i++)
                    {
                        var row = dataTable.Rows[i];
                        var values = new string[row.ItemArray.Length];
                        for (int j = 0; j < row.ItemArray.Length; j++)
                        {
                            values[j] = row.ItemArray[j]?.ToString() ?? "NULL";
                        }
                        _logger.Information("📝 **ROW_{Index}**: {Values}", i + 1, string.Join(" | ", values));
                    }

                    return dataTable;
                }
            }
        }
    }
}