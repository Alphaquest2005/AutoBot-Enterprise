// Product Line Regex Investigation - Why Line 2148 has 0 matches
// Based on database investigation showing Tropical Vendors product fields are on Line 2148
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

async function investigateProductLineRegex() {
  console.log('🔍 PRODUCT LINE REGEX INVESTIGATION');
  console.log('===================================\n');

  try {
    await sql.connect(dbConfig);
    console.log('✅ Connected to SQL Server\n');

    // 1. GET PRODUCT LINE REGEX PATTERNS
    console.log('🎯 PRODUCT LINE REGEX PATTERNS:');
    console.log('Amazon working lines vs Tropical Vendors failing line\n');
    
    const productLineRegex = await sql.query(`
      SELECT 
        i.Name as TemplateName,
        l.Id as LineId,
        re.Id as RegexId,
        re.RegEx as RegexPattern,
        re.MultiLine,
        re.MaxLines,
        COUNT(f.Id) as FieldCount,
        STRING_AGG(f.Field, ', ') as Fields
      FROM [OCR-Invoices] i
      JOIN [OCR-Parts] p ON i.Id = p.TemplateId
      JOIN [OCR-Lines] l ON p.Id = l.PartId
      JOIN [OCR-RegularExpressions] re ON l.RegExId = re.Id
      JOIN [OCR-Fields] f ON l.Id = f.LineId
      WHERE l.Id IN (40, 1606, 2091, 2148)  -- Amazon product lines + Tropical Vendors product line
        AND f.EntityType = 'InvoiceDetails'
      GROUP BY i.Name, l.Id, re.Id, re.RegEx, re.MultiLine, re.MaxLines
      ORDER BY i.Name, l.Id
    `);
    
    console.log('Product Line Regex Patterns:');
    console.table(productLineRegex.recordset);

    // 2. DETAILED ANALYSIS OF EACH PATTERN
    console.log('\n🔍 DETAILED REGEX ANALYSIS:');
    
    for (const row of productLineRegex.recordset) {
      console.log(`\n📊 ${row.TemplateName} - Line ${row.LineId}:`);
      console.log(`Fields: ${row.Fields}`);
      console.log(`MultiLine: ${row.MultiLine}, MaxLines: ${row.MaxLines}`);
      console.log(`Regex Pattern:`);
      console.log(row.RegexPattern);
      console.log('─'.repeat(80));
    }

    // 3. COMPARE AMAZON VS TROPICAL VENDORS PATTERNS
    console.log('\n🎯 PATTERN COMPARISON:');
    
    const amazonPatterns = productLineRegex.recordset.filter(r => r.TemplateName === 'Amazon');
    const tropicalPatterns = productLineRegex.recordset.filter(r => r.TemplateName === 'Tropical Vendors');
    
    console.log('\n📊 AMAZON PATTERNS (WORKING):');
    amazonPatterns.forEach(p => {
      console.log(`Line ${p.LineId}: ${p.Fields}`);
      console.log(`Pattern: ${p.RegexPattern.substring(0, 100)}...`);
    });
    
    console.log('\n📊 TROPICAL VENDORS PATTERNS (FAILING):');
    tropicalPatterns.forEach(p => {
      console.log(`Line ${p.LineId}: ${p.Fields}`);
      console.log(`Pattern: ${p.RegexPattern}`);
    });

    // 4. GET FULL TEMPLATE STRUCTURE FOR COMPARISON
    console.log('\n🔍 FULL TEMPLATE STRUCTURE COMPARISON:');
    
    const templateStructure = await sql.query(`
      SELECT 
        i.Name as TemplateName,
        p.Id as PartId,
        p.Name as PartName,
        l.Id as LineId,
        COUNT(f.Id) as FieldCount,
        STRING_AGG(f.Field, ', ') as Fields,
        f.EntityType
      FROM [OCR-Invoices] i
      JOIN [OCR-Parts] p ON i.Id = p.TemplateId
      JOIN [OCR-Lines] l ON p.Id = l.PartId
      LEFT JOIN [OCR-Fields] f ON l.Id = f.LineId
      WHERE i.Name IN ('Amazon', 'Tropical Vendors')
        AND f.EntityType = 'InvoiceDetails'
      GROUP BY i.Name, p.Id, p.Name, l.Id, f.EntityType
      ORDER BY i.Name, p.Id, l.Id
    `);
    
    console.log('\nTemplate Structure (InvoiceDetails only):');
    console.table(templateStructure.recordset);

    // 5. IDENTIFY THE PROBLEM
    console.log('\n🚨 PROBLEM IDENTIFICATION:');
    
    if (tropicalPatterns.length === 0) {
      console.log('❌ NO Tropical Vendors product line patterns found!');
    } else {
      console.log('✅ Tropical Vendors has product line patterns');
      console.log('🎯 Issue: Regex pattern on Line 2148 is not matching the text');
      console.log('📝 Need to examine the actual regex pattern and test it against sample text');
    }

    // 6. GET TROPICAL VENDORS REGEX PATTERN FOR TESTING
    console.log('\n🔧 TROPICAL VENDORS PRODUCT REGEX FOR TESTING:');
    
    const tropicalRegex = await sql.query(`
      SELECT 
        re.RegEx as RegexPattern,
        re.MultiLine,
        re.MaxLines
      FROM [OCR-Lines] l
      JOIN [OCR-RegularExpressions] re ON l.RegExId = re.Id
      WHERE l.Id = 2148
    `);
    
    if (tropicalRegex.recordset.length > 0) {
      const pattern = tropicalRegex.recordset[0];
      console.log('Tropical Vendors Product Line Regex (Line 2148):');
      console.log(`Pattern: ${pattern.RegexPattern}`);
      console.log(`MultiLine: ${pattern.MultiLine}`);
      console.log(`MaxLines: ${pattern.MaxLines}`);
      
      console.log('\n💡 NEXT STEPS:');
      console.log('1. Test this regex pattern against Tropical Vendors text file');
      console.log('2. Compare with working Amazon patterns');
      console.log('3. Fix the regex pattern if it\'s not matching correctly');
    } else {
      console.log('❌ No regex pattern found for Line 2148!');
    }

  } catch (error) {
    console.error('❌ Database Error:', error);
  } finally {
    await sql.close();
    console.log('\n🔚 Investigation Complete');
  }
}

// Execute the investigation
investigateProductLineRegex().catch(console.error);
