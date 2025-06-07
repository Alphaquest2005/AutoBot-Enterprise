// Detailed Template Analysis - Find how InvoiceDetails is generated
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

async function detailedTemplateAnalysis() {
  try {
    console.log('🔍 Detailed Template Analysis - Finding InvoiceDetails Generation...\n');
    
    await sql.connect(dbConfig);
    console.log('✅ Connected to SQL Server\n');

    // 1. Get complete Amazon template configuration
    console.log('=== AMAZON TEMPLATE DETAILED CONFIGURATION ===');
    const amazonConfig = await sql.query(`
      SELECT
        p.Id as PartId,
        pt.Name as PartType,
        l.Id as LineId,
        l.Name as LineName,
        f.Id as FieldId,
        f.Field as FieldName,
        f.[Key] as FieldKey,
        f.EntityType,
        f.DataType,
        f.IsRequired
      FROM [OCR-Invoices] i
      JOIN [OCR-Parts] p ON i.Id = p.TemplateId
      JOIN [OCR-PartTypes] pt ON p.PartTypeId = pt.Id
      JOIN [OCR-Lines] l ON p.Id = l.PartId
      JOIN [OCR-Fields] f ON l.Id = f.LineId
      WHERE i.Name = 'Amazon'
      ORDER BY p.Id, l.Id, f.Id
    `);
    console.table(amazonConfig.recordset);

    // 2. Get complete Tropical Vendors template configuration
    console.log('\n=== TROPICAL VENDORS TEMPLATE DETAILED CONFIGURATION ===');
    const tropicalConfig = await sql.query(`
      SELECT
        p.Id as PartId,
        pt.Name as PartType,
        l.Id as LineId,
        l.Name as LineName,
        f.Id as FieldId,
        f.Field as FieldName,
        f.[Key] as FieldKey,
        f.EntityType,
        f.DataType,
        f.IsRequired
      FROM [OCR-Invoices] i
      JOIN [OCR-Parts] p ON i.Id = p.TemplateId
      JOIN [OCR-PartTypes] pt ON p.PartTypeId = pt.Id
      JOIN [OCR-Lines] l ON p.Id = l.PartId
      JOIN [OCR-Fields] f ON l.Id = f.LineId
      WHERE i.Name = 'Tropical Vendors'
      ORDER BY p.Id, l.Id, f.Id
    `);
    console.table(tropicalConfig.recordset);

    // 3. Look for fields that might generate InvoiceDetails
    console.log('\n=== FIELDS THAT MIGHT GENERATE INVOICEDETAILS ===');
    const potentialFields = await sql.query(`
      SELECT
        i.Name as InvoiceName,
        pt.Name as PartType,
        l.Name as LineName,
        f.Field as FieldName,
        f.[Key] as FieldKey,
        f.EntityType,
        f.DataType
      FROM [OCR-Invoices] i
      JOIN [OCR-Parts] p ON i.Id = p.TemplateId
      JOIN [OCR-PartTypes] pt ON p.PartTypeId = pt.Id
      JOIN [OCR-Lines] l ON p.Id = l.PartId
      JOIN [OCR-Fields] f ON l.Id = f.LineId
      WHERE i.Name IN ('Amazon', 'Tropical Vendors')
        AND (f.Field LIKE '%Details%'
             OR f.Field LIKE '%Invoice%'
             OR f.[Key] LIKE '%Details%'
             OR f.[Key] LIKE '%Invoice%'
             OR f.EntityType LIKE '%Details%'
             OR f.EntityType LIKE '%Invoice%')
      ORDER BY i.Name, pt.Name, l.Name
    `);
    console.table(potentialFields.recordset);

    // 4. Compare part types between templates
    console.log('\n=== PART TYPE COMPARISON ===');
    const partTypeComparison = await sql.query(`
      SELECT 
        i.Name as InvoiceName,
        pt.Name as PartType,
        COUNT(l.Id) as LineCount,
        COUNT(f.Id) as FieldCount
      FROM [OCR-Invoices] i
      JOIN [OCR-Parts] p ON i.Id = p.TemplateId
      JOIN [OCR-PartTypes] pt ON p.PartTypeId = pt.Id
      LEFT JOIN [OCR-Lines] l ON p.Id = l.PartId
      LEFT JOIN [OCR-Fields] f ON l.Id = f.LineId
      WHERE i.Name IN ('Amazon', 'Tropical Vendors')
      GROUP BY i.Name, pt.Name
      ORDER BY i.Name, pt.Name
    `);
    console.table(partTypeComparison.recordset);

    // 5. Look for EntityType patterns
    console.log('\n=== ENTITY TYPE ANALYSIS ===');
    const entityTypes = await sql.query(`
      SELECT 
        i.Name as InvoiceName,
        f.EntityType,
        COUNT(*) as FieldCount,
        STRING_AGG(f.Field, ', ') as FieldNames
      FROM [OCR-Invoices] i
      JOIN [OCR-Parts] p ON i.Id = p.TemplateId
      JOIN [OCR-Lines] l ON p.Id = l.PartId
      JOIN [OCR-Fields] f ON l.Id = f.LineId
      WHERE i.Name IN ('Amazon', 'Tropical Vendors')
      GROUP BY i.Name, f.EntityType
      ORDER BY i.Name, f.EntityType
    `);
    console.table(entityTypes.recordset);

    // 6. Check for parent-child relationships in fields
    console.log('\n=== PARENT-CHILD FIELD RELATIONSHIPS ===');
    const parentChildFields = await sql.query(`
      SELECT 
        i.Name as InvoiceName,
        pt.Name as PartType,
        l.Name as LineName,
        f.Field as FieldName,
        f.ParentId,
        pf.Field as ParentFieldName
      FROM [OCR-Invoices] i
      JOIN [OCR-Parts] p ON i.Id = p.TemplateId
      JOIN [OCR-PartTypes] pt ON p.PartTypeId = pt.Id
      JOIN [OCR-Lines] l ON p.Id = l.PartId
      JOIN [OCR-Fields] f ON l.Id = f.LineId
      LEFT JOIN [OCR-Fields] pf ON f.ParentId = pf.Id
      WHERE i.Name IN ('Amazon', 'Tropical Vendors')
        AND f.ParentId IS NOT NULL
      ORDER BY i.Name, pt.Name, l.Name
    `);
    console.table(parentChildFields.recordset);

    // 7. Find missing configurations in Tropical Vendors
    console.log('\n=== MISSING CONFIGURATIONS IN TROPICAL VENDORS ===');
    
    // Get Amazon's unique field configurations
    const amazonFields = amazonConfig.recordset.map(r => ({
      PartType: r.PartType,
      FieldName: r.FieldName,
      FieldKey: r.FieldKey,
      EntityType: r.EntityType
    }));
    
    const tropicalFields = tropicalConfig.recordset.map(r => ({
      PartType: r.PartType,
      FieldName: r.FieldName,
      FieldKey: r.FieldKey,
      EntityType: r.EntityType
    }));
    
    console.log('\nAmazon has these part types:');
    const amazonPartTypes = [...new Set(amazonFields.map(f => f.PartType))];
    console.log(amazonPartTypes);
    
    console.log('\nTropical Vendors has these part types:');
    const tropicalPartTypes = [...new Set(tropicalFields.map(f => f.PartType))];
    console.log(tropicalPartTypes);
    
    console.log('\nMissing part types in Tropical Vendors:');
    const missingPartTypes = amazonPartTypes.filter(pt => !tropicalPartTypes.includes(pt));
    console.log(missingPartTypes);

    // 8. Key findings summary
    console.log('\n=== KEY FINDINGS SUMMARY ===');
    console.log('1. Amazon template configuration:');
    console.log(`   - Parts: ${[...new Set(amazonConfig.recordset.map(r => r.PartType))].join(', ')}`);
    console.log(`   - Total fields: ${amazonConfig.recordset.length}`);
    
    console.log('\n2. Tropical Vendors template configuration:');
    console.log(`   - Parts: ${[...new Set(tropicalConfig.recordset.map(r => r.PartType))].join(', ')}`);
    console.log(`   - Total fields: ${tropicalConfig.recordset.length}`);
    
    console.log('\n3. Missing in Tropical Vendors:');
    console.log(`   - Missing part types: ${missingPartTypes.join(', ')}`);
    console.log(`   - Field count difference: ${amazonConfig.recordset.length - tropicalConfig.recordset.length}`);

    console.log('\n4. Next steps:');
    console.log('   - Check if InvoiceDetails is generated by InvoiceLine part types');
    console.log('   - Look for EntityType patterns that create line item details');
    console.log('   - Investigate parent-child field relationships');

  } catch (error) {
    console.error('❌ Error:', error.message);
    console.error('Stack:', error.stack);
  } finally {
    await sql.close();
  }
}

detailedTemplateAnalysis();
