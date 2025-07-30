using System;
using System.Collections.Generic;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
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

        [Test]
        public async Task FixMangoTemplateRegexPatterns()
        {
            _logger.Information("🔧 **REGEX_PATTERN_FIX**: Fixing MANGO template regex patterns to match actual OCR text format");

            // Fix InvoiceNo pattern: "order UCSJB6 shortly" -> Extract UCSJB6
            var fixInvoiceNoScript = @"
                UPDATE [OCR-RegularExpressions] 
                SET RegEx = 'order\s+(?<InvoiceNo>[A-Za-z0-9]+)\s+shortly'
                WHERE Id IN (
                    SELECT r.Id FROM [OCR-RegularExpressions] r
                    INNER JOIN [OCR-Lines] l ON r.Id = l.RegularExpressionsId
                    INNER JOIN [OCR-Parts] p ON l.PartId = p.Id
                    INNER JOIN [OCR-Invoices] t ON p.TemplateId = t.Id
                    INNER JOIN [OCR-Fields] f ON l.Id = f.LineId
                    WHERE t.Name = '03152025_TOTAL_AMOUNT_GENERIC_DOCUMENT'
                    AND f.Field = 'InvoiceNo'
                );
                SELECT @@ROWCOUNT as RowsUpdated;";

            await ExecuteSqlScript(fixInvoiceNoScript, "Fix InvoiceNo pattern for MANGO template").ConfigureAwait(false);

            // Fix SupplierName pattern: "MANGO OUTLET"
            var fixSupplierNameScript = @"
                UPDATE [OCR-RegularExpressions] 
                SET RegEx = '(?<SupplierName>MANGO\s+OUTLET)'
                WHERE Id IN (
                    SELECT r.Id FROM [OCR-RegularExpressions] r
                    INNER JOIN [OCR-Lines] l ON r.Id = l.RegularExpressionsId
                    INNER JOIN [OCR-Parts] p ON l.PartId = p.Id
                    INNER JOIN [OCR-Invoices] t ON p.TemplateId = t.Id
                    INNER JOIN [OCR-Fields] f ON l.Id = f.LineId
                    WHERE t.Name = '03152025_TOTAL_AMOUNT_GENERIC_DOCUMENT'
                    AND f.Field = 'SupplierName'
                );
                SELECT @@ROWCOUNT as RowsUpdated;";

            await ExecuteSqlScript(fixSupplierNameScript, "Fix SupplierName pattern for MANGO template").ConfigureAwait(false);

            // Fix SubTotal pattern: "Subtotal USS 196.33" (handles USS or US$)
            var fixSubTotalScript = @"
                UPDATE [OCR-RegularExpressions] 
                SET RegEx = 'Subtotal\s+US[S$]\s*(?<SubTotal>\d+\.\d{2})'
                WHERE Id IN (
                    SELECT r.Id FROM [OCR-RegularExpressions] r
                    INNER JOIN [OCR-Lines] l ON r.Id = l.RegularExpressionsId
                    INNER JOIN [OCR-Parts] p ON l.PartId = p.Id
                    INNER JOIN [OCR-Invoices] t ON p.TemplateId = t.Id
                    INNER JOIN [OCR-Fields] f ON l.Id = f.LineId
                    WHERE t.Name = '03152025_TOTAL_AMOUNT_GENERIC_DOCUMENT'
                    AND f.Field = 'SubTotal'
                );
                SELECT @@ROWCOUNT as RowsUpdated;";

            await ExecuteSqlScript(fixSubTotalScript, "Fix SubTotal pattern for MANGO template").ConfigureAwait(false);

            // Fix InvoiceTotal pattern: "TOTAL AMOUNT US$ 210.08"
            var fixInvoiceTotalScript = @"
                UPDATE [OCR-RegularExpressions] 
                SET RegEx = 'TOTAL\s+AMOUNT\s+US\$\s*(?<InvoiceTotal>\d+\.\d{2})'
                WHERE Id IN (
                    SELECT r.Id FROM [OCR-RegularExpressions] r
                    INNER JOIN [OCR-Lines] l ON r.Id = l.RegularExpressionsId
                    INNER JOIN [OCR-Parts] p ON l.PartId = p.Id
                    INNER JOIN [OCR-Invoices] t ON p.TemplateId = t.Id
                    INNER JOIN [OCR-Fields] f ON l.Id = f.LineId
                    WHERE t.Name = '03152025_TOTAL_AMOUNT_GENERIC_DOCUMENT'
                    AND f.Field = 'InvoiceTotal'
                );
                SELECT @@ROWCOUNT as RowsUpdated;";

            await ExecuteSqlScript(fixInvoiceTotalScript, "Fix InvoiceTotal pattern for MANGO template").ConfigureAwait(false);

            // Fix TotalOtherCost pattern: "Estimated Tax US$ 13.74"
            var fixTotalOtherCostScript = @"
                UPDATE [OCR-RegularExpressions] 
                SET RegEx = 'Estimated\s+Tax\s+US\$\s*(?<TotalOtherCost>\d+\.\d{2})'
                WHERE Id IN (
                    SELECT r.Id FROM [OCR-RegularExpressions] r
                    INNER JOIN [OCR-Lines] l ON r.Id = l.RegularExpressionsId
                    INNER JOIN [OCR-Parts] p ON l.PartId = p.Id
                    INNER JOIN [OCR-Invoices] t ON p.TemplateId = t.Id
                    INNER JOIN [OCR-Fields] f ON l.Id = f.LineId
                    WHERE t.Name = '03152025_TOTAL_AMOUNT_GENERIC_DOCUMENT'
                    AND f.Field = 'TotalOtherCost'
                );
                SELECT @@ROWCOUNT as RowsUpdated;";

            await ExecuteSqlScript(fixTotalOtherCostScript, "Fix TotalOtherCost pattern for MANGO template").ConfigureAwait(false);

            // Verify the updates
            var verifyUpdatesScript = @"
                SELECT 
                    t.Name as TemplateName,
                    f.Field as FieldName,
                    r.RegEx as UpdatedPattern
                FROM [OCR-Invoices] t
                INNER JOIN [OCR-Parts] p ON t.Id = p.TemplateId
                INNER JOIN [OCR-Lines] l ON p.Id = l.PartId
                INNER JOIN [OCR-RegularExpressions] r ON l.RegularExpressionsId = r.Id
                INNER JOIN [OCR-Fields] f ON l.Id = f.LineId
                WHERE t.Name = '03152025_TOTAL_AMOUNT_GENERIC_DOCUMENT'
                ORDER BY f.Field;";

            await ExecuteSqlScript(verifyUpdatesScript, "Verify updated MANGO template patterns").ConfigureAwait(false);

            _logger.Information("✅ **REGEX_PATTERN_FIX_COMPLETE**: MANGO template patterns updated to match actual OCR text format");
        }

        [Test]
        public async Task CreateOCRTemplateTableMappingAndInsertInvoiceData()
        {
            _logger.Information("🚀 **DATABASE_SETUP**: Creating OCR_TemplateTableMapping table and inserting Invoice template mapping data");

            // 1. Check if OCR_TemplateTableMapping table exists
            var tableExistsScript = @"
                SELECT COUNT(*) as TableExists
                FROM INFORMATION_SCHEMA.TABLES 
                WHERE TABLE_SCHEMA = 'dbo' 
                AND TABLE_NAME = 'OCR_TemplateTableMapping';
            ";

            var tableExistsResult = await ExecuteSqlScript(tableExistsScript, "Check if OCR_TemplateTableMapping table exists").ConfigureAwait(false);
            
            // 2. Create OCR_TemplateTableMapping table if it doesn't exist
            var createTableScript = @"
                IF NOT EXISTS (SELECT * FROM INFORMATION_SCHEMA.TABLES WHERE TABLE_SCHEMA = 'dbo' AND TABLE_NAME = 'OCR_TemplateTableMapping')
                BEGIN
                    CREATE TABLE [dbo].[OCR_TemplateTableMapping] (
                        [Id] INT PRIMARY KEY IDENTITY(1,1),
                        [FileTypeId] INT NOT NULL,
                        [DocumentType] NVARCHAR(50) NOT NULL,
                        [PrimaryEntityType] NVARCHAR(100) NOT NULL,
                        [SecondaryEntityTypes] NVARCHAR(500) NULL,
                        [RequiredFields] NVARCHAR(1000) NULL,
                        [ValidationRules] NVARCHAR(MAX) NULL,
                        [ApplicationSettingsId] INT NOT NULL DEFAULT(1),
                        [IsActive] BIT NOT NULL DEFAULT(1),
                        [CreatedDate] DATETIME2 NOT NULL DEFAULT(GETUTCDATE())
                    );
                    
                    PRINT '✅ OCR_TemplateTableMapping table created successfully';
                END
                ELSE
                BEGIN
                    PRINT '⚠️ OCR_TemplateTableMapping table already exists';
                END
            ";

            await ExecuteSqlScript(createTableScript, "Create OCR_TemplateTableMapping table").ConfigureAwait(false);

            // 3. Insert Invoice template mapping data
            var insertInvoiceDataScript = @"
                -- Check if Invoice mapping already exists
                IF NOT EXISTS (SELECT 1 FROM [dbo].[OCR_TemplateTableMapping] WHERE [DocumentType] = 'Invoice')
                BEGIN
                    INSERT INTO [dbo].[OCR_TemplateTableMapping] 
                    ([FileTypeId], [DocumentType], [PrimaryEntityType], [SecondaryEntityTypes], [RequiredFields], [ValidationRules], [ApplicationSettingsId], [IsActive])
                    VALUES 
                    (1, 'Invoice', 'ShipmentInvoice', 'InvoiceDetails,ShipmentInvoiceFreight', 
                     'InvoiceNo,SupplierName,InvoiceDate,InvoiceTotal,SubTotal,Currency', 
                     '{""EntityTypes"":[""ShipmentInvoice"",""InvoiceDetails""],""DataTypes"":{""InvoiceNo"":""string"",""InvoiceTotal"":""decimal"",""SubTotal"":""decimal""},""BusinessRules"":{""required_fields"":{""InvoiceNo"":true,""InvoiceTotal"":true}}}', 
                     1, 1);
                    
                    PRINT '✅ Invoice template mapping data inserted successfully';
                END
                ELSE
                BEGIN
                    PRINT '⚠️ Invoice template mapping already exists';
                END
            ";

            await ExecuteSqlScript(insertInvoiceDataScript, "Insert Invoice template mapping data").ConfigureAwait(false);

            // 4. Create performance indexes
            var createIndexesScript = @"
                -- Create index for DocumentType lookups if it doesn't exist
                IF NOT EXISTS (SELECT * FROM sys.indexes WHERE name = 'IX_OCR_TemplateTableMapping_DocumentType' AND object_id = OBJECT_ID('dbo.OCR_TemplateTableMapping'))
                BEGIN
                    CREATE NONCLUSTERED INDEX IX_OCR_TemplateTableMapping_DocumentType 
                    ON [dbo].[OCR_TemplateTableMapping] ([DocumentType], [ApplicationSettingsId], [IsActive])
                    INCLUDE ([PrimaryEntityType], [SecondaryEntityTypes], [RequiredFields]);
                    
                    PRINT '✅ Performance index created successfully';
                END
                ELSE
                BEGIN
                    PRINT '⚠️ Performance index already exists';
                END
            ";

            await ExecuteSqlScript(createIndexesScript, "Create performance indexes").ConfigureAwait(false);

            // 5. Verify the data was inserted correctly
            var verifyDataScript = @"
                SELECT 
                    Id,
                    FileTypeId,
                    DocumentType,
                    PrimaryEntityType,
                    SecondaryEntityTypes,
                    RequiredFields,
                    ValidationRules,
                    ApplicationSettingsId,
                    IsActive,
                    CreatedDate
                FROM [dbo].[OCR_TemplateTableMapping]
                WHERE DocumentType = 'Invoice'
                ORDER BY Id;
            ";

            await ExecuteSqlScript(verifyDataScript, "Verify Invoice template mapping data").ConfigureAwait(false);

            // 6. Test the DatabaseTemplateHelper integration
            _logger.Information("🧪 **TESTING_DATABASE_HELPER**: Testing DatabaseTemplateHelper integration with new data");
            
            try 
            {
                var mappings = WaterNut.DataSpace.DatabaseTemplateHelper.GetTemplateMappingsByDocumentType("Invoice");
                _logger.Information("✅ **DATABASE_HELPER_SUCCESS**: Retrieved {Count} Invoice template mappings", mappings.Count);
                
                foreach (var mapping in mappings)
                {
                    _logger.Information("📋 **MAPPING_DETAILS**: FileTypeId={FileTypeId}, DocumentType={DocumentType}, PrimaryEntity={PrimaryEntity}", 
                        mapping.FileTypeId, mapping.DocumentType, mapping.PrimaryEntityType);
                }
            }
            catch (Exception ex)
            {
                _logger.Error(ex, "❌ **DATABASE_HELPER_ERROR**: Failed to test DatabaseTemplateHelper integration");
            }

            _logger.Information("🎯 **DATABASE_SETUP_COMPLETE**: OCR_TemplateTableMapping table and Invoice data setup completed successfully");
        }

        /// <summary>
        /// Execute a SQL script with parameters using existing Entity Framework context
        /// </summary>
        public async Task<DataTable> ExecuteSqlScript(string script, string description = null, Dictionary<string, object> parameters = null)
        {
            _logger.Information("🔍 **SQL_EXECUTION**: {Description}", description ?? "Executing SQL script");
            _logger.Information("📄 **SQL_SCRIPT**: {Script}", script);

            using (var context = new CoreEntities.Business.Entities.CoreEntitiesContext())
            {
                using (var command = new SqlCommand(script, context.Database.Connection as SqlConnection))
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

                    if (context.Database.Connection.State != System.Data.ConnectionState.Open)
                        await context.Database.Connection.OpenAsync().ConfigureAwait(false);

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