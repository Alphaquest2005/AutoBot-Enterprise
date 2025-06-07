// Check EntityType fields for Tropical Vendors template (ID 213)
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

async function checkTropicalVendorsEntityType() {
  console.log('🔍 Checking EntityType for Tropical Vendors Template (ID 213)...\n');

  try {
    await sql.connect(dbConfig);
    console.log('✅ Connected to SQL Server\n');

    // 1. First verify the template exists
    console.log('=== VERIFYING TEMPLATE 213 ===');
    const template = await sql.query(`
      SELECT Id, Name, FileTypeId, IsActive, ApplicationSettingsId
      FROM [OCR-Invoices]
      WHERE Id = 213
    `);
    
    if (template.recordset.length === 0) {
      console.log('❌ Template 213 not found!');
      return;
    }
    
    console.log('Template found:', template.recordset[0]);

    // 2. Get all parts for template 213
    console.log('\n=== ALL PARTS FOR TEMPLATE 213 ===');
    const allParts = await sql.query(`
      SELECT 
        p.Id AS PartId,
        p.InvoiceId,
        p.PartTypeId,
        p.ParentId,
        pt.Name AS PartTypeName,
        pp.PartTypeId AS ParentPartTypeId,
        ppt.Name AS ParentPartTypeName
      FROM [OCR-Parts] p
      LEFT JOIN [OCR-PartTypes] pt ON p.PartTypeId = pt.Id
      LEFT JOIN [OCR-Parts] pp ON p.ParentId = pp.Id
      LEFT JOIN [OCR-PartTypes] ppt ON pp.PartTypeId = ppt.Id
      WHERE p.InvoiceId = 213
      ORDER BY COALESCE(p.ParentId, p.Id), p.Id
    `);
    
    console.log('\nParts Structure:');
    allParts.recordset.forEach(part => {
      if (part.ParentId) {
        console.log(`  └─ Child Part ${part.PartId}: ${part.PartTypeName} (Parent: ${part.ParentId} - ${part.ParentPartTypeName})`);
      } else {
        console.log(`Parent Part ${part.PartId}: ${part.PartTypeName}`);
      }
    });

    // 3. Get fields for child parts with EntityType
    console.log('\n=== CHILD PARTS AND THEIR FIELDS WITH ENTITYTYPE ===');
    const childPartsFields = await sql.query(`
      SELECT 
        p.Id AS PartId,
        p.ParentId,
        pt.Name AS PartTypeName,
        ppt.Name AS ParentPartTypeName,
        l.Id AS LineId,
        l.Name AS LineName,
        f.Id AS FieldId,
        f.Field AS FieldName,
        f.[Key] AS FieldKey,
        f.EntityType,
        f.DataType,
        f.FieldValue,
        f.DatabaseFieldName
      FROM [OCR-Parts] p
      INNER JOIN [OCR-PartTypes] pt ON p.PartTypeId = pt.Id
      LEFT JOIN [OCR-Parts] pp ON p.ParentId = pp.Id
      LEFT JOIN [OCR-PartTypes] ppt ON pp.PartTypeId = ppt.Id
      LEFT JOIN [OCR-Lines] l ON l.PartId = p.Id
      LEFT JOIN [OCR-Fields] f ON f.LineId = l.Id
      WHERE p.InvoiceId = 213 AND p.ParentId IS NOT NULL
      ORDER BY p.ParentId, p.Id, l.Id, f.Id
    `);
    
    let currentPart = null;
    let currentLine = null;
    childPartsFields.recordset.forEach(row => {
      if (row.PartId !== currentPart) {
        currentPart = row.PartId;
        console.log(`\n📦 Child Part ${row.PartId}: ${row.PartTypeName}`);
        console.log(`   Parent: ${row.ParentId} (${row.ParentPartTypeName})`);
      }
      if (row.LineId && row.LineId !== currentLine) {
        currentLine = row.LineId;
        console.log(`   📋 Line ${row.LineId}: ${row.LineName || 'unnamed'}`);
      }
      if (row.FieldId) {
        console.log(`      🔹 Field: ${row.FieldName || row.FieldKey}`);
        console.log(`         - EntityType: "${row.EntityType || 'NULL'}"`);
        console.log(`         - DataType: ${row.DataType || 'NULL'}`);
        console.log(`         - DatabaseFieldName: ${row.DatabaseFieldName || 'NULL'}`);
        console.log(`         - FieldValue: ${row.FieldValue || 'NULL'}`);
      }
    });

    // 4. Summary of EntityTypes in child parts
    console.log('\n\n=== ENTITYTYPE SUMMARY FOR CHILD PARTS ===');
    const entityTypeSummary = await sql.query(`
      SELECT 
        f.EntityType,
        COUNT(DISTINCT p.Id) AS ChildPartCount,
        COUNT(DISTINCT f.Id) AS FieldCount,
        STRING_AGG(DISTINCT pt.Name, ', ') AS PartTypes,
        STRING_AGG(DISTINCT f.Field, ', ') AS FieldNames
      FROM [OCR-Parts] p
      INNER JOIN [OCR-PartTypes] pt ON p.PartTypeId = pt.Id
      INNER JOIN [OCR-Lines] l ON l.PartId = p.Id
      INNER JOIN [OCR-Fields] f ON f.LineId = l.Id
      WHERE p.InvoiceId = 213 AND p.ParentId IS NOT NULL
      GROUP BY f.EntityType
      ORDER BY f.EntityType
    `);
    
    console.log('\nEntityType Usage in Child Parts:');
    entityTypeSummary.recordset.forEach(row => {
      console.log(`\n🏷️  EntityType: "${row.EntityType || 'NULL'}"`);
      console.log(`   - Used in ${row.ChildPartCount} child part(s) of type: ${row.PartTypes}`);
      console.log(`   - Total fields: ${row.FieldCount}`);
      console.log(`   - Field names: ${row.FieldNames}`);
    });

    // 5. Check if any child parts have EntityType = 'InvoiceDetails'
    console.log('\n\n=== CHECKING FOR ENTITYTYPE = "InvoiceDetails" ===');
    const invoiceDetailsCheck = await sql.query(`
      SELECT 
        p.Id AS PartId,
        pt.Name AS PartTypeName,
        f.Field AS FieldName,
        f.EntityType,
        f.DatabaseFieldName
      FROM [OCR-Parts] p
      INNER JOIN [OCR-PartTypes] pt ON p.PartTypeId = pt.Id
      INNER JOIN [OCR-Lines] l ON l.PartId = p.Id
      INNER JOIN [OCR-Fields] f ON f.LineId = l.Id
      WHERE p.InvoiceId = 213 
        AND p.ParentId IS NOT NULL 
        AND f.EntityType = 'InvoiceDetails'
    `);
    
    if (invoiceDetailsCheck.recordset.length === 0) {
      console.log('❌ No child parts have EntityType = "InvoiceDetails"');
      console.log('This is likely the root cause of the issue!');
    } else {
      console.log('✅ Found fields with EntityType = "InvoiceDetails":');
      console.table(invoiceDetailsCheck.recordset);
    }

    // 6. Compare with Amazon template
    console.log('\n\n=== COMPARISON WITH AMAZON TEMPLATE ===');
    const amazonComparison = await sql.query(`
      SELECT 
        i.Name AS InvoiceName,
        f.EntityType,
        COUNT(DISTINCT p.Id) AS PartCount,
        COUNT(DISTINCT f.Id) AS FieldCount,
        STRING_AGG(DISTINCT pt.Name, ', ') AS PartTypes
      FROM [OCR-Invoices] i
      INNER JOIN [OCR-Parts] p ON i.Id = p.InvoiceId
      INNER JOIN [OCR-PartTypes] pt ON p.PartTypeId = pt.Id
      INNER JOIN [OCR-Lines] l ON l.PartId = p.Id
      INNER JOIN [OCR-Fields] f ON f.LineId = l.Id
      WHERE i.Name IN ('Amazon', 'Tropical Vendors')
        AND p.ParentId IS NOT NULL
        AND f.EntityType IS NOT NULL
      GROUP BY i.Name, f.EntityType
      ORDER BY i.Name, f.EntityType
    `);
    
    console.log('EntityType usage comparison:');
    console.table(amazonComparison.recordset);

  } catch (error) {
    console.error('❌ Error:', error.message);
    console.error('Stack:', error.stack);
  } finally {
    await sql.close();
  }
}

checkTropicalVendorsEntityType();