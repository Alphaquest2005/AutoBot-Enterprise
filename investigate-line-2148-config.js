// Investigate Line 2148 Configuration Issues
// Why does a working regex pattern produce 0 matches in OCR pipeline?

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

async function investigateLine2148Config() {
  console.log('🔍 LINE 2148 CONFIGURATION INVESTIGATION');
  console.log('========================================\n');

  try {
    await sql.connect(dbConfig);
    console.log('✅ Connected to SQL Server\n');

    // 1. GET COMPLETE LINE 2148 CONFIGURATION
    console.log('🎯 COMPLETE LINE 2148 CONFIGURATION:');
    
    const line2148Config = await sql.query(`
      SELECT 
        i.Id as InvoiceId,
        i.Name as InvoiceName,
        p.Id as PartId,
        p.Name as PartName,
        p.IsRecurring,
        p.IsChild,
        l.Id as LineId,
        l.Name as LineName,
        l.RegExId,
        re.RegEx as RegexPattern,
        re.MultiLine,
        re.MaxLines,
        COUNT(f.Id) as FieldCount,
        STRING_AGG(f.Field, ', ') as Fields
      FROM [OCR-Invoices] i
      JOIN [OCR-Parts] p ON i.Id = p.TemplateId
      JOIN [OCR-Lines] l ON p.Id = l.PartId
      JOIN [OCR-RegularExpressions] re ON l.RegExId = re.Id
      LEFT JOIN [OCR-Fields] f ON l.Id = f.LineId
      WHERE l.Id = 2148
      GROUP BY i.Id, i.Name, p.Id, p.Name, p.IsRecurring, p.IsChild, l.Id, l.Name, l.RegExId, re.RegEx, re.MultiLine, re.MaxLines
    `);
    
    console.log('Line 2148 Complete Configuration:');
    console.table(line2148Config.recordset);

    // 2. CHECK PART HIERARCHY
    console.log('\n🔍 TROPICAL VENDORS PART HIERARCHY:');
    
    const partHierarchy = await sql.query(`
      SELECT 
        i.Name as InvoiceName,
        p.Id as PartId,
        p.Name as PartName,
        p.IsRecurring,
        p.IsChild,
        COUNT(l.Id) as LineCount,
        COUNT(f.Id) as FieldCount
      FROM [OCR-Invoices] i
      JOIN [OCR-Parts] p ON i.Id = p.TemplateId
      LEFT JOIN [OCR-Lines] l ON p.Id = l.PartId
      LEFT JOIN [OCR-Fields] f ON l.Id = f.LineId
      WHERE i.Name = 'Tropical Vendors'
      GROUP BY i.Name, p.Id, p.Name, p.IsRecurring, p.IsChild
      ORDER BY p.Id
    `);
    
    console.log('Tropical Vendors Part Hierarchy:');
    console.table(partHierarchy.recordset);

    // 3. CHECK IF LINE 2148 IS ACTIVE/ENABLED
    console.log('\n🔍 LINE 2148 STATUS CHECK:');
    
    const lineStatus = await sql.query(`
      SELECT 
        l.Id as LineId,
        l.Name as LineName,
        l.RegExId,
        l.PartId,
        re.RegEx,
        CASE WHEN re.Id IS NULL THEN 'MISSING REGEX' ELSE 'REGEX OK' END as RegexStatus,
        CASE WHEN f.Id IS NULL THEN 'NO FIELDS' ELSE 'HAS FIELDS' END as FieldStatus
      FROM [OCR-Lines] l
      LEFT JOIN [OCR-RegularExpressions] re ON l.RegExId = re.Id
      LEFT JOIN [OCR-Fields] f ON l.Id = f.LineId
      WHERE l.Id = 2148
    `);
    
    console.log('Line 2148 Status:');
    console.table(lineStatus.recordset);

    // 4. COMPARE WITH WORKING AMAZON LINES
    console.log('\n🔍 COMPARISON WITH WORKING AMAZON LINES:');
    
    const amazonComparison = await sql.query(`
      SELECT 
        i.Name as InvoiceName,
        l.Id as LineId,
        l.Name as LineName,
        re.MultiLine,
        re.MaxLines,
        COUNT(f.Id) as FieldCount,
        STRING_AGG(f.Field, ', ') as Fields
      FROM [OCR-Invoices] i
      JOIN [OCR-Parts] p ON i.Id = p.TemplateId
      JOIN [OCR-Lines] l ON p.Id = l.PartId
      JOIN [OCR-RegularExpressions] re ON l.RegExId = re.Id
      LEFT JOIN [OCR-Fields] f ON l.Id = f.LineId
      WHERE l.Id IN (40, 1606, 2091, 2148)  -- Amazon working lines + Tropical Vendors
        AND f.EntityType = 'InvoiceDetails'
      GROUP BY i.Name, l.Id, l.Name, re.MultiLine, re.MaxLines
      ORDER BY i.Name, l.Id
    `);
    
    console.log('Amazon vs Tropical Vendors Line Comparison:');
    console.table(amazonComparison.recordset);

    // 5. CHECK REGEX CONFIGURATION DIFFERENCES
    console.log('\n🔍 REGEX CONFIGURATION DIFFERENCES:');
    
    const regexConfig = await sql.query(`
      SELECT 
        i.Name as InvoiceName,
        l.Id as LineId,
        re.Id as RegexId,
        re.MultiLine,
        re.MaxLines,
        LEN(re.RegEx) as PatternLength,
        CASE 
          WHEN re.MultiLine = 1 THEN 'MULTILINE'
          WHEN re.MultiLine = 0 THEN 'SINGLE LINE'
          ELSE 'NULL'
        END as LineMode,
        CASE 
          WHEN re.MaxLines IS NULL THEN 'UNLIMITED'
          ELSE CAST(re.MaxLines as VARCHAR)
        END as MaxLinesConfig
      FROM [OCR-Invoices] i
      JOIN [OCR-Parts] p ON i.Id = p.TemplateId
      JOIN [OCR-Lines] l ON p.Id = l.PartId
      JOIN [OCR-RegularExpressions] re ON l.RegExId = re.Id
      WHERE l.Id IN (40, 1606, 2091, 2148)
      ORDER BY i.Name, l.Id
    `);
    
    console.log('Regex Configuration Comparison:');
    console.table(regexConfig.recordset);

    // 6. IDENTIFY POTENTIAL ISSUES
    console.log('\n🚨 POTENTIAL ISSUES IDENTIFIED:');
    
    const line2148Data = line2148Config.recordset[0];
    if (line2148Data) {
      console.log('Line 2148 Analysis:');
      console.log(`- Part: ${line2148Data.PartName} (ID: ${line2148Data.PartId})`);
      console.log(`- IsRecurring: ${line2148Data.IsRecurring}`);
      console.log(`- IsChild: ${line2148Data.IsChild}`);
      console.log(`- MultiLine: ${line2148Data.MultiLine}`);
      console.log(`- MaxLines: ${line2148Data.MaxLines}`);
      console.log(`- Field Count: ${line2148Data.FieldCount}`);
      
      // Check for issues
      const issues = [];
      if (line2148Data.MultiLine === null) issues.push('MultiLine is NULL (should be true/false)');
      if (line2148Data.MaxLines === null) issues.push('MaxLines is NULL (may need a value)');
      if (line2148Data.FieldCount === 0) issues.push('No fields defined');
      if (line2148Data.IsChild === true) issues.push('Line is in child part (may not be processed)');
      
      if (issues.length > 0) {
        console.log('\n❌ ISSUES FOUND:');
        issues.forEach(issue => console.log(`   - ${issue}`));
      } else {
        console.log('\n✅ No obvious configuration issues found');
      }
    }

    // 7. RECOMMENDATION
    console.log('\n💡 INVESTIGATION RESULTS:');
    console.log('─'.repeat(40));
    
    const tropicalConfig = regexConfig.recordset.find(r => r.LineId === 2148);
    const amazonConfigs = regexConfig.recordset.filter(r => r.InvoiceName === 'Amazon');
    
    if (tropicalConfig) {
      console.log('Tropical Vendors Line 2148:');
      console.log(`- MultiLine: ${tropicalConfig.MultiLine} (${tropicalConfig.LineMode})`);
      console.log(`- MaxLines: ${tropicalConfig.MaxLinesConfig}`);
      
      console.log('\nAmazon Working Lines:');
      amazonConfigs.forEach(config => {
        console.log(`- Line ${config.LineId}: MultiLine=${config.MultiLine}, MaxLines=${config.MaxLinesConfig}`);
      });
      
      // Check for differences
      if (tropicalConfig.MultiLine === null && amazonConfigs.some(a => a.MultiLine === true)) {
        console.log('\n🚨 CRITICAL ISSUE: Tropical Vendors MultiLine is NULL while Amazon uses MultiLine=true');
        console.log('💡 SOLUTION: Set MultiLine=true for Line 2148');
      }
    }

  } catch (error) {
    console.error('❌ Database Error:', error);
  } finally {
    await sql.close();
    console.log('\n🔚 Investigation Complete');
  }
}

// Execute the investigation
investigateLine2148Config().catch(console.error);
