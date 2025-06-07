// Field Mapping Investigation - Amazon vs Tropical Vendors Line 4 Comparison
// Based on OCR_PARENT_CHILD_ANALYSIS_REPORT.md findings
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

async function investigateFieldMappings() {
  console.log('🔍 FIELD MAPPING INVESTIGATION - Amazon vs Tropical Vendors Line 4');
  console.log('=================================================================\n');

  try {
    await sql.connect(dbConfig);
    console.log('✅ Connected to SQL Server\n');

    // 1. CRITICAL COMPARISON: Line 4 Field Mappings
    console.log('🎯 CRITICAL COMPARISON: Line 4 Field Mappings');
    console.log('Amazon Line 4 (LineId: 78) vs Tropical Vendors Line 4 (LineId: 2147)\n');
    
    const line4Comparison = await sql.query(`
      SELECT 
        i.Name as TemplateName,
        l.Id as LineId,
        f.Id as FieldId,
        f.Field as FieldName,
        f.EntityType,
        re.Id as RegexId,
        re.RegEx as RegexPattern,
        re.MultiLine,
        re.MaxLines
      FROM [OCR-Invoices] i
      JOIN [OCR-Parts] p ON i.Id = p.TemplateId
      JOIN [OCR-Lines] l ON p.Id = l.PartId
      JOIN [OCR-Fields] f ON l.Id = f.LineId
      JOIN [OCR-RegularExpressions] re ON l.RegExId = re.Id
      WHERE l.Id IN (78, 2147)  -- Amazon Line 4 vs Tropical Vendors Line 4
      ORDER BY i.Name, f.Id
    `);
    
    console.log('Line 4 Field Mappings Comparison:');
    console.table(line4Comparison.recordset);

    // 2. DETAILED FIELD ANALYSIS: Group by template
    console.log('\n🔍 DETAILED FIELD ANALYSIS BY TEMPLATE:');
    
    const amazonLine4 = line4Comparison.recordset.filter(r => r.TemplateName === 'Amazon');
    const tropicalLine4 = line4Comparison.recordset.filter(r => r.TemplateName === 'Tropical Vendors');
    
    console.log('\n📊 AMAZON LINE 4 (LineId: 78) - WORKING:');
    console.log(`Total Fields: ${amazonLine4.length}`);
    console.table(amazonLine4);
    
    console.log('\n📊 TROPICAL VENDORS LINE 4 (LineId: 2147) - FAILING:');
    console.log(`Total Fields: ${tropicalLine4.length}`);
    console.table(tropicalLine4);

    // 3. FIELD TYPE ANALYSIS
    console.log('\n🎯 FIELD TYPE ANALYSIS:');
    
    const amazonFields = amazonLine4.map(f => f.FieldName).sort();
    const tropicalFields = tropicalLine4.map(f => f.FieldName).sort();
    
    console.log('\nAmazon Line 4 Field Types:', amazonFields);
    console.log('Tropical Vendors Line 4 Field Types:', tropicalFields);
    
    // 4. REGEX PATTERN COMPARISON
    console.log('\n🔍 REGEX PATTERN COMPARISON:');
    
    const amazonRegex = amazonLine4.length > 0 ? amazonLine4[0].RegexPattern : 'N/A';
    const tropicalRegex = tropicalLine4.length > 0 ? tropicalLine4[0].RegexPattern : 'N/A';
    
    console.log('\nAmazon Line 4 Regex Pattern:');
    console.log(amazonRegex);
    console.log('\nTropical Vendors Line 4 Regex Pattern:');
    console.log(tropicalRegex);

    // 5. PRODUCT FIELD SEARCH: Look for ItemNumber, ItemDescription, Cost, Quantity
    console.log('\n🎯 PRODUCT FIELD SEARCH - All Lines:');
    console.log('Looking for ItemNumber, ItemDescription, Cost, Quantity fields...\n');
    
    const productFields = await sql.query(`
      SELECT 
        i.Name as TemplateName,
        l.Id as LineId,
        f.Field as FieldName,
        f.EntityType,
        COUNT(*) as FieldCount
      FROM [OCR-Invoices] i
      JOIN [OCR-Parts] p ON i.Id = p.TemplateId
      JOIN [OCR-Lines] l ON p.Id = l.PartId
      JOIN [OCR-Fields] f ON l.Id = f.LineId
      WHERE i.Name IN ('Amazon', 'Tropical Vendors')
        AND (f.Field LIKE '%Item%' OR f.Field LIKE '%Cost%' OR f.Field LIKE '%Quantity%' OR f.Field = 'InvoiceDate')
      GROUP BY i.Name, l.Id, f.Field, f.EntityType
      ORDER BY i.Name, l.Id, f.Field
    `);
    
    console.log('Product Fields Found:');
    console.table(productFields.recordset);

    // 6. INVOICEDATE FIELD ANALYSIS
    console.log('\n🚨 INVOICEDATE FIELD ANALYSIS:');
    console.log('Why is Tropical Vendors Line 4 extracting InvoiceDate instead of product fields?\n');
    
    const invoiceDateFields = productFields.recordset.filter(f => f.FieldName === 'InvoiceDate');
    console.log('InvoiceDate Field Occurrences:');
    console.table(invoiceDateFields);

    // 7. SOLUTION IDENTIFICATION
    console.log('\n🔧 SOLUTION IDENTIFICATION:');
    
    const amazonProductFields = productFields.recordset.filter(f => 
      f.TemplateName === 'Amazon' && 
      (f.FieldName.includes('Item') || f.FieldName.includes('Cost') || f.FieldName.includes('Quantity'))
    );
    
    const tropicalProductFields = productFields.recordset.filter(f => 
      f.TemplateName === 'Tropical Vendors' && 
      (f.FieldName.includes('Item') || f.FieldName.includes('Cost') || f.FieldName.includes('Quantity'))
    );
    
    console.log('\nAmazon Product Fields (Working):');
    console.table(amazonProductFields);
    
    console.log('\nTropical Vendors Product Fields (Current):');
    console.table(tropicalProductFields);

    // 8. RECOMMENDATION
    console.log('\n💡 RECOMMENDATION:');
    if (tropicalProductFields.length === 0) {
      console.log('❌ Tropical Vendors has NO product fields defined!');
      console.log('✅ Solution: Add ItemNumber, ItemDescription, Cost, Quantity fields to Tropical Vendors Line 4');
    } else {
      console.log('⚠️  Tropical Vendors has product fields but they may be on wrong lines');
      console.log('✅ Solution: Move product fields to Line 4 or fix field mappings');
    }

  } catch (error) {
    console.error('❌ Database Error:', error);
  } finally {
    await sql.close();
    console.log('\n🔚 Investigation Complete');
  }
}

// Execute the investigation
investigateFieldMappings().catch(console.error);
