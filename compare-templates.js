// Compare Amazon and Tropical Vendors OCR template configurations
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

async function compareTemplates() {
  console.log('🔍 Comparing Amazon vs Tropical Vendors OCR Templates...\n');

  try {
    await sql.connect(dbConfig);
    console.log('✅ Connected to SQL Server\n');

    // 1. Get basic template info
    console.log('=== TEMPLATE BASIC INFO ===');
    const templateInfo = await sql.query(`
      SELECT Id, Name, FileTypeId, IsActive, ApplicationSettingsId
      FROM [OCR-Invoices]
      WHERE Name IN ('Amazon', 'Tropical Vendors')
      ORDER BY Name
    `);
    console.table(templateInfo.recordset);

    // 2. Check OCR-Parts schema first
    console.log('\n=== OCR-PARTS SCHEMA ===');
    const partsSchema = await sql.query(`
      SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
      FROM INFORMATION_SCHEMA.COLUMNS
      WHERE TABLE_NAME = 'OCR-Parts'
      ORDER BY ORDINAL_POSITION
    `);
    console.table(partsSchema.recordset);

    // 3. Get parts configuration
    console.log('\n=== PARTS CONFIGURATION ===');
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
    console.table(partsConfig.recordset);

    // 4. Check OCR-Lines schema
    console.log('\n=== OCR-LINES SCHEMA ===');
    const linesSchema = await sql.query(`
      SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
      FROM INFORMATION_SCHEMA.COLUMNS
      WHERE TABLE_NAME = 'OCR-Lines'
      ORDER BY ORDINAL_POSITION
    `);
    console.table(linesSchema.recordset);

    // 5. Get lines configuration
    console.log('\n=== LINES CONFIGURATION ===');
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
    console.table(linesConfig.recordset);

    // 6. Check OCR-Fields schema
    console.log('\n=== OCR-FIELDS SCHEMA ===');
    const fieldsSchema = await sql.query(`
      SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
      FROM INFORMATION_SCHEMA.COLUMNS
      WHERE TABLE_NAME = 'OCR-Fields'
      ORDER BY ORDINAL_POSITION
    `);
    console.table(fieldsSchema.recordset);

    // 7. Get fields configuration
    console.log('\n=== FIELDS CONFIGURATION ===');
    const fieldsConfig = await sql.query(`
      SELECT
        i.Name as InvoiceName,
        p.Id as PartId,
        l.Id as LineId,
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
    console.table(fieldsConfig.recordset);

    // 8. Check for InvoiceDetails field specifically
    console.log('\n=== INVOICEDETAILS FIELD CHECK ===');
    const invoiceDetailsCheck = await sql.query(`
      SELECT
        i.Name as InvoiceName,
        p.Id as PartId,
        l.Id as LineId,
        f.Field as FieldName,
        f.FieldValue
      FROM [OCR-Invoices] i
      JOIN [OCR-Parts] p ON i.Id = p.TemplateId
      JOIN [OCR-Lines] l ON p.Id = l.PartId
      JOIN [OCR-Fields] f ON l.Id = f.LineId
      WHERE i.Name IN ('Amazon', 'Tropical Vendors')
        AND f.Field LIKE '%InvoiceDetails%'
      ORDER BY i.Name
    `);
    console.log('InvoiceDetails fields found:');
    console.table(invoiceDetailsCheck.recordset);

    // 9. Get child fields (this might be where InvoiceDetails comes from)
    console.log('\n=== CHILD FIELDS CONFIGURATION ===');
    const childFieldsConfig = await sql.query(`
      SELECT
        i.Name as InvoiceName,
        p.Id as PartId,
        l.Id as LineId,
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

    // 10. Summary comparison
    console.log('\n=== SUMMARY COMPARISON ===');
    const summary = await sql.query(`
      SELECT
        i.Name as InvoiceName,
        COUNT(DISTINCT p.Id) as PartCount,
        COUNT(DISTINCT l.Id) as LineCount,
        COUNT(DISTINCT f.Id) as FieldCount,
        COUNT(DISTINCT cf.Id) as ChildFieldCount
      FROM [OCR-Invoices] i
      LEFT JOIN [OCR-Parts] p ON i.Id = p.TemplateId
      LEFT JOIN [OCR-Lines] l ON p.Id = l.PartId
      LEFT JOIN [OCR-Fields] f ON l.Id = f.LineId
      LEFT JOIN [OCR-ChildFields] cf ON f.Id = cf.FieldId
      WHERE i.Name IN ('Amazon', 'Tropical Vendors')
      GROUP BY i.Id, i.Name
      ORDER BY i.Name
    `);
    console.table(summary.recordset);

  } catch (error) {
    console.error('❌ Error:', error.message);
  } finally {
    await sql.close();
  }
}

compareTemplates();
