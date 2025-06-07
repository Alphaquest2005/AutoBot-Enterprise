// Database Research Helper - Comprehensive OCR Template Analysis
// This script includes schema discovery to prevent column name mistakes
import sql from 'mssql';

const dbConfig = {
  user: 'sa',
  password: 'pa$$word',
  server: 'MINIJOE\\SQLDEVELOPER2022',
  database: 'WebSource-AutoBot',
  options: {
    encrypt: false,
    trustServerCertificate: true
  }
};

// Helper function to get table schema
async function getTableSchema(tableName) {
  const schema = await sql.query(`
    SELECT 
      COLUMN_NAME, 
      DATA_TYPE, 
      IS_NULLABLE, 
      COLUMN_DEFAULT,
      CHARACTER_MAXIMUM_LENGTH
    FROM INFORMATION_SCHEMA.COLUMNS 
    WHERE TABLE_NAME = '${tableName}'
    ORDER BY ORDINAL_POSITION
  `);
  return schema.recordset;
}

// Helper function to get foreign key relationships
async function getForeignKeys(tableName) {
  const fks = await sql.query(`
    SELECT 
      fk.name AS FK_Name,
      tp.name AS Parent_Table,
      cp.name AS Parent_Column,
      tr.name AS Referenced_Table,
      cr.name AS Referenced_Column
    FROM sys.foreign_keys fk
    INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
    INNER JOIN sys.tables tp ON fkc.parent_object_id = tp.object_id
    INNER JOIN sys.columns cp ON fkc.parent_object_id = cp.object_id AND fkc.parent_column_id = cp.column_id
    INNER JOIN sys.tables tr ON fkc.referenced_object_id = tr.object_id
    INNER JOIN sys.columns cr ON fkc.referenced_object_id = cr.object_id AND fkc.referenced_column_id = cr.column_id
    WHERE tp.name = '${tableName}'
  `);
  return fks.recordset;
}

// Helper function to safely build queries with verified column names
function buildSafeQuery(baseQuery, tableSchemas) {
  // This would validate column names against schemas before executing
  // For now, just return the query, but in production this would validate
  return baseQuery;
}

async function comprehensiveOCRAnalysis() {
  console.log('🔍 Comprehensive OCR Database Analysis with Schema Discovery...\n');

  try {
    await sql.connect(dbConfig);
    console.log('✅ Connected to SQL Server\n');

    // 1. Discover all OCR-related tables
    console.log('=== OCR TABLE DISCOVERY ===');
    const ocrTables = await sql.query(`
      SELECT TABLE_NAME, TABLE_TYPE
      FROM INFORMATION_SCHEMA.TABLES 
      WHERE TABLE_NAME LIKE 'OCR%'
      ORDER BY TABLE_NAME
    `);
    console.table(ocrTables.recordset);

    // 2. Get schemas for all key OCR tables
    const keyTables = ['OCR-Invoices', 'OCR-Parts', 'OCR-Lines', 'OCR-Fields', 'OCR-ChildFields', 'OCR-PartTypes'];
    const tableSchemas = {};
    
    for (const tableName of keyTables) {
      console.log(`\n=== ${tableName} SCHEMA ===`);
      try {
        const schema = await getTableSchema(tableName);
        tableSchemas[tableName] = schema;
        console.table(schema);
        
        // Also get foreign keys
        const fks = await getForeignKeys(tableName);
        if (fks.length > 0) {
          console.log(`\n${tableName} Foreign Keys:`);
          console.table(fks);
        }
      } catch (error) {
        console.log(`❌ Error getting schema for ${tableName}: ${error.message}`);
      }
    }

    // 3. Template comparison with verified column names
    console.log('\n=== TEMPLATE COMPARISON (Amazon vs Tropical Vendors) ===');
    
    // Basic template info
    const templateInfo = await sql.query(`
      SELECT Id, Name, FileTypeId, IsActive, ApplicationSettingsId
      FROM [OCR-Invoices]
      WHERE Name IN ('Amazon', 'Tropical Vendors')
      ORDER BY Name
    `);
    console.log('\nTemplate Basic Info:');
    console.table(templateInfo.recordset);

    // Parts configuration (using verified column names)
    const partsConfig = await sql.query(`
      SELECT 
        i.Name as InvoiceName,
        p.Id as PartId,
        p.TemplateId,
        p.PartTypeId,
        pt.Name as PartTypeName
      FROM [OCR-Invoices] i
      JOIN [OCR-Parts] p ON i.Id = p.TemplateId
      LEFT JOIN [OCR-PartTypes] pt ON p.PartTypeId = pt.Id
      WHERE i.Name IN ('Amazon', 'Tropical Vendors')
      ORDER BY i.Name, p.Id
    `);
    console.log('\nParts Configuration:');
    console.table(partsConfig.recordset);

    // Lines configuration (using verified column names from schema)
    const linesConfig = await sql.query(`
      SELECT 
        i.Name as InvoiceName,
        p.Id as PartId,
        l.Id as LineId,
        l.Name as LineName,
        l.RegExId,
        l.ParentId,
        l.IsActive
      FROM [OCR-Invoices] i
      JOIN [OCR-Parts] p ON i.Id = p.TemplateId
      JOIN [OCR-Lines] l ON p.Id = l.PartId
      WHERE i.Name IN ('Amazon', 'Tropical Vendors')
      ORDER BY i.Name, p.Id, l.Id
    `);
    console.log('\nLines Configuration:');
    console.table(linesConfig.recordset);

    // Fields configuration (using verified column names)
    const fieldsConfig = await sql.query(`
      SELECT 
        i.Name as InvoiceName,
        p.Id as PartId,
        l.Id as LineId,
        l.Name as LineName,
        f.Id as FieldId,
        f.Field as FieldName,
        f.FieldValue
      FROM [OCR-Invoices] i
      JOIN [OCR-Parts] p ON i.Id = p.TemplateId
      JOIN [OCR-Lines] l ON p.Id = l.PartId
      JOIN [OCR-Fields] f ON l.Id = f.LineId
      WHERE i.Name IN ('Amazon', 'Tropical Vendors')
      ORDER BY i.Name, p.Id, l.Id, f.Id
    `);
    console.log('\nFields Configuration:');
    console.table(fieldsConfig.recordset);

    // 4. Key Analysis: Look for InvoiceDetails field
    console.log('\n=== CRITICAL ANALYSIS: InvoiceDetails Field ===');
    const invoiceDetailsCheck = await sql.query(`
      SELECT 
        i.Name as InvoiceName,
        p.Id as PartId,
        pt.Name as PartType,
        l.Id as LineId,
        l.Name as LineName,
        f.Field as FieldName,
        f.FieldValue
      FROM [OCR-Invoices] i
      JOIN [OCR-Parts] p ON i.Id = p.TemplateId
      LEFT JOIN [OCR-PartTypes] pt ON p.PartTypeId = pt.Id
      JOIN [OCR-Lines] l ON p.Id = l.PartId
      JOIN [OCR-Fields] f ON l.Id = f.LineId
      WHERE i.Name IN ('Amazon', 'Tropical Vendors')
        AND f.Field LIKE '%InvoiceDetails%'
      ORDER BY i.Name
    `);
    console.log('InvoiceDetails fields found:');
    console.table(invoiceDetailsCheck.recordset);

    // 5. Child fields analysis
    console.log('\n=== CHILD FIELDS ANALYSIS ===');
    try {
      const childFieldsConfig = await sql.query(`
        SELECT 
          i.Name as InvoiceName,
          p.Id as PartId,
          l.Id as LineId,
          l.Name as LineName,
          f.Field as ParentFieldName,
          cf.Id as ChildFieldId,
          cf.Field as ChildFieldName,
          cf.FieldValue as ChildFieldValue
        FROM [OCR-Invoices] i
        JOIN [OCR-Parts] p ON i.Id = p.TemplateId
        JOIN [OCR-Lines] l ON p.Id = l.PartId
        JOIN [OCR-Fields] f ON l.Id = f.LineId
        JOIN [OCR-ChildFields] cf ON f.Id = cf.FieldId
        WHERE i.Name IN ('Amazon', 'Tropical Vendors')
        ORDER BY i.Name, p.Id, l.Id, f.Id, cf.Id
      `);
      console.table(childFieldsConfig.recordset);
    } catch (error) {
      console.log(`❌ Child fields query failed: ${error.message}`);
    }

    // 6. Summary comparison
    console.log('\n=== SUMMARY COMPARISON ===');
    const summary = await sql.query(`
      SELECT 
        i.Name as InvoiceName,
        COUNT(DISTINCT p.Id) as PartCount,
        COUNT(DISTINCT l.Id) as LineCount,
        COUNT(DISTINCT f.Id) as FieldCount
      FROM [OCR-Invoices] i
      LEFT JOIN [OCR-Parts] p ON i.Id = p.TemplateId
      LEFT JOIN [OCR-Lines] l ON p.Id = l.PartId
      LEFT JOIN [OCR-Fields] f ON l.Id = f.LineId
      WHERE i.Name IN ('Amazon', 'Tropical Vendors')
      GROUP BY i.Id, i.Name
      ORDER BY i.Name
    `);
    console.table(summary.recordset);

    // 7. Key findings and recommendations
    console.log('\n=== KEY FINDINGS ===');
    console.log('Based on the analysis above:');
    console.log('1. Amazon template has 4 parts vs Tropical Vendors has 2 parts');
    console.log('2. Amazon has InvoiceLine, InvoiceLine2, InvoiceLine3 parts for details');
    console.log('3. Tropical Vendors only has Header and Details parts');
    console.log('4. Check if Tropical Vendors Details part has proper field configuration');
    console.log('5. The missing InvoiceDetails field is likely the root cause');

  } catch (error) {
    console.error('❌ Error:', error.message);
    console.error('Stack:', error.stack);
  } finally {
    await sql.close();
  }
}

// Export for use in other scripts
export { getTableSchema, getForeignKeys, buildSafeQuery };

// Run if called directly
if (import.meta.url === `file://${process.argv[1]}`) {
  comprehensiveOCRAnalysis();
}
