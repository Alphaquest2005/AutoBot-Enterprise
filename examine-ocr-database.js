// Examine OCR database structure and field naming conflicts
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

async function examineOCRDatabase() {
  console.log('🔍 Examining OCR Database Structure and Field Conflicts...\n');

  try {
    await sql.connect(dbConfig);
    console.log('✅ Connected to SQL Server\n');

    // 1. Count total invoice types
    console.log('=== INVOICE TYPES COUNT ===');
    const invoiceCount = await sql.query(`
      SELECT COUNT(*) as TotalInvoices
      FROM [OCR-Invoices]
      WHERE IsActive = 1
    `);
    console.log(`Total Active Invoice Types: ${invoiceCount.recordset[0].TotalInvoices}\n`);

    // 2. Show sample invoice types
    console.log('=== SAMPLE INVOICE TYPES ===');
    const sampleInvoices = await sql.query(`
      SELECT TOP 10 Id, Name, FileTypeId, ApplicationSettingsId
      FROM [OCR-Invoices]
      WHERE IsActive = 1
      ORDER BY Id
    `);
    console.table(sampleInvoices.recordset);

    // 3. Examine field naming conflicts
    console.log('\n=== FIELD NAMING CONFLICTS ===');
    const fieldConflicts = await sql.query(`
      SELECT
        f.Field,
        f.[Key],
        COUNT(*) as OccurrenceCount,
        COUNT(DISTINCT f.EntityType) as DifferentEntityTypes,
        COUNT(DISTINCT f.DataType) as DifferentDataTypes
      FROM [OCR-Fields] f
      GROUP BY f.Field, f.[Key]
      HAVING COUNT(*) > 1
      ORDER BY COUNT(*) DESC
    `);
    console.log(`Found ${fieldConflicts.recordset.length} field name conflicts:`);
    console.table(fieldConflicts.recordset.slice(0, 20)); // Show top 20 conflicts

    // 4. Show specific examples of field conflicts
    console.log('\n=== SPECIFIC FIELD CONFLICT EXAMPLES ===');
    const specificConflicts = await sql.query(`
      SELECT TOP 20
        f.Id,
        f.Field,
        f.[Key],
        f.EntityType,
        f.DataType,
        l.Name as LineName,
        i.Name as InvoiceName
      FROM [OCR-Fields] f
      INNER JOIN [OCR-Lines] l ON f.LineId = l.Id
      INNER JOIN [OCR-Parts] p ON l.PartId = p.Id
      INNER JOIN [OCR-Invoices] i ON p.InvoiceId = i.Id
      WHERE f.Field IN (
        SELECT Field
        FROM [OCR-Fields]
        GROUP BY Field
        HAVING COUNT(*) > 5
      )
      ORDER BY f.Field, i.Name
    `);
    console.table(specificConflicts.recordset);

    // 5. Show invoice structure complexity
    console.log('\n=== INVOICE STRUCTURE COMPLEXITY ===');
    const structureComplexity = await sql.query(`
      SELECT
        i.Name as InvoiceName,
        COUNT(DISTINCT p.Id) as PartCount,
        COUNT(DISTINCT l.Id) as LineCount,
        COUNT(DISTINCT f.Id) as FieldCount,
        COUNT(DISTINCT f.Field) as UniqueFieldNames
      FROM [OCR-Invoices] i
      LEFT JOIN [OCR-Parts] p ON i.Id = p.InvoiceId
      LEFT JOIN [OCR-Lines] l ON p.Id = l.PartId
      LEFT JOIN [OCR-Fields] f ON l.Id = f.LineId
      WHERE i.IsActive = 1
      GROUP BY i.Id, i.Name
      ORDER BY FieldCount DESC
    `);
    console.log('Top 15 most complex invoice structures:');
    console.table(structureComplexity.recordset.slice(0, 15));

    // 6. Show common field names across invoices
    console.log('\n=== MOST COMMON FIELD NAMES ===');
    const commonFields = await sql.query(`
      SELECT
        f.Field,
        COUNT(*) as TotalOccurrences,
        COUNT(DISTINCT i.Id) as InvoiceTypesUsing,
        CAST(COUNT(DISTINCT i.Id) * 100.0 / (SELECT COUNT(*) FROM [OCR-Invoices] WHERE IsActive = 1) as DECIMAL(5,2)) as PercentageOfInvoices
      FROM [OCR-Fields] f
      INNER JOIN [OCR-Lines] l ON f.LineId = l.Id
      INNER JOIN [OCR-Parts] p ON l.PartId = p.Id
      INNER JOIN [OCR-Invoices] i ON p.InvoiceId = i.Id
      WHERE i.IsActive = 1
      GROUP BY f.Field
      ORDER BY TotalOccurrences DESC
    `);
    console.log('Top 20 most common field names:');
    console.table(commonFields.recordset.slice(0, 20));

  } catch (error) {
    console.error('❌ Database error:', error.message);
  } finally {
    await sql.close();
  }
}

examineOCRDatabase();
