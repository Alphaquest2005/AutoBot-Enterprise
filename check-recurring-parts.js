// Check RecuringPart configuration for Tropical Vendors vs Amazon
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

async function checkRecurringParts() {
  try {
    console.log('🔍 Checking RecuringPart Configuration...\n');
    
    await sql.connect(dbConfig);
    console.log('✅ Connected to SQL Server\n');

    // 1. Check OCR-RecuringPart schema
    console.log('=== OCR-RECURINGPART SCHEMA ===');
    const recuringPartSchema = await sql.query(`
      SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE, COLUMN_DEFAULT
      FROM INFORMATION_SCHEMA.COLUMNS
      WHERE TABLE_NAME = 'OCR-RecuringPart'
      ORDER BY ORDINAL_POSITION
    `);
    console.table(recuringPartSchema.recordset);

    // 2. First check OCR-Parts schema
    console.log('\n=== OCR-PARTS SCHEMA ===');
    const partsSchema = await sql.query(`
      SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
      FROM INFORMATION_SCHEMA.COLUMNS
      WHERE TABLE_NAME = 'OCR-Parts'
      ORDER BY ORDINAL_POSITION
    `);
    console.table(partsSchema.recordset);

    // 3. Get parts with RecuringPart configuration (using correct column names)
    console.log('\n=== PARTS WITH RECURINGPART CONFIGURATION ===');
    const partsWithRecuring = await sql.query(`
      SELECT
        i.Name as InvoiceName,
        p.Id as PartId,
        p.TemplateId,
        p.PartTypeId,
        rp.Id as RecuringPartId,
        rp.IsComposite
      FROM [OCR-Invoices] i
      JOIN [OCR-Parts] p ON i.Id = p.TemplateId
      LEFT JOIN [OCR-RecuringPart] rp ON p.Id = rp.Id
      WHERE i.Name IN ('Amazon', 'Tropical Vendors')
      ORDER BY i.Name, p.Id
    `);
    console.table(partsWithRecuring.recordset);

    // 4. Get detailed lines configuration for Details parts
    console.log('\n=== DETAILS PARTS LINES CONFIGURATION ===');
    const detailsLines = await sql.query(`
      SELECT
        i.Name as InvoiceName,
        p.Id as PartId,
        l.Id as LineId,
        l.PartId as LinePartId,
        l.RegExId,
        l.IsActive,
        re.RegEx as RegexPattern,
        re.MultiLine,
        re.MaxLines
      FROM [OCR-Invoices] i
      JOIN [OCR-Parts] p ON i.Id = p.TemplateId
      JOIN [OCR-Lines] l ON p.Id = l.PartId
      LEFT JOIN [OCR-RegularExpressions] re ON l.RegExId = re.Id
      WHERE i.Name IN ('Amazon', 'Tropical Vendors')
      ORDER BY i.Name, p.Id, l.Id
    `);
    console.table(detailsLines.recordset);

    // 5. Get fields for Details parts
    console.log('\n=== DETAILS PARTS FIELDS ===');
    const detailsFields = await sql.query(`
      SELECT
        i.Name as InvoiceName,
        p.Id as PartId,
        l.Id as LineId,
        f.Id as FieldId,
        f.Field as FieldName,
        f.[Key] as FieldKey,
        f.EntityType
      FROM [OCR-Invoices] i
      JOIN [OCR-Parts] p ON i.Id = p.TemplateId
      JOIN [OCR-Lines] l ON p.Id = l.PartId
      JOIN [OCR-Fields] f ON l.Id = f.LineId
      WHERE i.Name IN ('Amazon', 'Tropical Vendors')
      ORDER BY i.Name, p.Id, l.Id, f.Id
    `);
    console.table(detailsFields.recordset);

    // 6. Check for any MaxLines or similar restrictions
    console.log('\n=== REGEX MAXLINES CONFIGURATION ===');
    const maxLinesConfig = await sql.query(`
      SELECT
        i.Name as InvoiceName,
        p.Id as PartId,
        l.Id as LineId,
        re.RegEx,
        re.MultiLine,
        re.MaxLines
      FROM [OCR-Invoices] i
      JOIN [OCR-Parts] p ON i.Id = p.TemplateId
      JOIN [OCR-Lines] l ON p.Id = l.PartId
      JOIN [OCR-RegularExpressions] re ON l.RegExId = re.Id
      WHERE i.Name IN ('Amazon', 'Tropical Vendors')
        AND re.MaxLines IS NOT NULL
      ORDER BY i.Name, p.Id
    `);
    console.table(maxLinesConfig.recordset);

    console.log('\n=== ANALYSIS COMPLETE ===');
    console.log('Key findings:');
    console.log('1. Check IsComposite values for Details parts');
    console.log('2. Look for MaxLines restrictions');
    console.log('3. Compare Amazon vs Tropical Vendors configurations');

  } catch (error) {
    console.error('❌ Error:', error);
  } finally {
    await sql.close();
  }
}

checkRecurringParts();
