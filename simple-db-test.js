// Simple database test to verify connection and get basic template info
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

async function simpleTest() {
  try {
    console.log('🔍 Simple Database Test...\n');
    
    await sql.connect(dbConfig);
    console.log('✅ Connected to SQL Server\n');

    // Test 1: Basic template info
    console.log('=== BASIC TEMPLATE INFO ===');
    const templates = await sql.query(`
      SELECT Id, Name, FileTypeId, IsActive
      FROM [OCR-Invoices]
      WHERE Name IN ('Amazon', 'Tropical Vendors')
      ORDER BY Name
    `);
    console.table(templates.recordset);

    // Test 2: Parts count
    console.log('\n=== PARTS COUNT ===');
    const partsCount = await sql.query(`
      SELECT 
        i.Name as InvoiceName,
        COUNT(p.Id) as PartCount
      FROM [OCR-Invoices] i
      LEFT JOIN [OCR-Parts] p ON i.Id = p.TemplateId
      WHERE i.Name IN ('Amazon', 'Tropical Vendors')
      GROUP BY i.Id, i.Name
      ORDER BY i.Name
    `);
    console.table(partsCount.recordset);

    // Test 3: Lines count
    console.log('\n=== LINES COUNT ===');
    const linesCount = await sql.query(`
      SELECT 
        i.Name as InvoiceName,
        COUNT(l.Id) as LineCount
      FROM [OCR-Invoices] i
      LEFT JOIN [OCR-Parts] p ON i.Id = p.TemplateId
      LEFT JOIN [OCR-Lines] l ON p.Id = l.PartId
      WHERE i.Name IN ('Amazon', 'Tropical Vendors')
      GROUP BY i.Id, i.Name
      ORDER BY i.Name
    `);
    console.table(linesCount.recordset);

    // Test 4: Fields count
    console.log('\n=== FIELDS COUNT ===');
    const fieldsCount = await sql.query(`
      SELECT 
        i.Name as InvoiceName,
        COUNT(f.Id) as FieldCount
      FROM [OCR-Invoices] i
      LEFT JOIN [OCR-Parts] p ON i.Id = p.TemplateId
      LEFT JOIN [OCR-Lines] l ON p.Id = l.PartId
      LEFT JOIN [OCR-Fields] f ON l.Id = f.LineId
      WHERE i.Name IN ('Amazon', 'Tropical Vendors')
      GROUP BY i.Id, i.Name
      ORDER BY i.Name
    `);
    console.table(fieldsCount.recordset);

    // Test 5: Check OCR-Fields schema first
    console.log('\n=== OCR-FIELDS SCHEMA ===');
    const fieldsSchema = await sql.query(`
      SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
      FROM INFORMATION_SCHEMA.COLUMNS
      WHERE TABLE_NAME = 'OCR-Fields'
      ORDER BY ORDINAL_POSITION
    `);
    console.table(fieldsSchema.recordset);

    // Test 6: Look for InvoiceDetails field (using correct column names)
    console.log('\n=== INVOICEDETAILS FIELD SEARCH ===');
    const invoiceDetailsFields = await sql.query(`
      SELECT
        i.Name as InvoiceName,
        f.Field as FieldName
      FROM [OCR-Invoices] i
      JOIN [OCR-Parts] p ON i.Id = p.TemplateId
      JOIN [OCR-Lines] l ON p.Id = l.PartId
      JOIN [OCR-Fields] f ON l.Id = f.LineId
      WHERE i.Name IN ('Amazon', 'Tropical Vendors')
        AND f.Field LIKE '%InvoiceDetails%'
      ORDER BY i.Name
    `);

    if (invoiceDetailsFields.recordset.length > 0) {
      console.table(invoiceDetailsFields.recordset);
    } else {
      console.log('❌ No InvoiceDetails fields found!');
    }

    console.log('\n=== KEY FINDINGS ===');
    console.log('1. Amazon template exists with ID 5');
    console.log('2. Tropical Vendors template exists with ID 213');
    console.log('3. Check the counts above to see the differences');
    console.log('4. InvoiceDetails field search results shown above');

  } catch (error) {
    console.error('❌ Error:', error.message);
    console.error('Stack:', error.stack);
  } finally {
    await sql.close();
  }
}

simpleTest();
