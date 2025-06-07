// Check Database Schema to Get Correct Column Names
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

async function checkDatabaseSchema() {
  console.log('🔍 DATABASE SCHEMA INVESTIGATION');
  console.log('================================\n');

  try {
    await sql.connect(dbConfig);
    console.log('✅ Connected to SQL Server\n');

    // 1. CHECK OCR-PARTS SCHEMA
    console.log('📊 OCR-PARTS TABLE SCHEMA:');
    const partsSchema = await sql.query(`
      SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
      FROM INFORMATION_SCHEMA.COLUMNS
      WHERE TABLE_NAME = 'OCR-Parts'
      ORDER BY ORDINAL_POSITION
    `);
    console.table(partsSchema.recordset);

    // 2. CHECK OCR-LINES SCHEMA
    console.log('\n📊 OCR-LINES TABLE SCHEMA:');
    const linesSchema = await sql.query(`
      SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
      FROM INFORMATION_SCHEMA.COLUMNS
      WHERE TABLE_NAME = 'OCR-Lines'
      ORDER BY ORDINAL_POSITION
    `);
    console.table(linesSchema.recordset);

    // 3. SIMPLE LINE 2148 CHECK
    console.log('\n🎯 SIMPLE LINE 2148 CHECK:');
    const line2148Simple = await sql.query(`
      SELECT 
        l.Id as LineId,
        l.RegExId,
        re.RegEx,
        re.MultiLine,
        re.MaxLines
      FROM [OCR-Lines] l
      LEFT JOIN [OCR-RegularExpressions] re ON l.RegExId = re.Id
      WHERE l.Id = 2148
    `);
    console.table(line2148Simple.recordset);

    // 4. CHECK MULTILINE CONFIGURATION COMPARISON
    console.log('\n🔍 MULTILINE CONFIGURATION COMPARISON:');
    const multilineComparison = await sql.query(`
      SELECT 
        l.Id as LineId,
        re.MultiLine,
        re.MaxLines,
        CASE 
          WHEN re.MultiLine = 1 THEN 'TRUE'
          WHEN re.MultiLine = 0 THEN 'FALSE'
          ELSE 'NULL'
        END as MultiLineStatus
      FROM [OCR-Lines] l
      JOIN [OCR-RegularExpressions] re ON l.RegExId = re.Id
      WHERE l.Id IN (40, 1606, 2091, 2148)
      ORDER BY l.Id
    `);
    console.table(multilineComparison.recordset);

    // 5. CHECK FIELD MAPPINGS FOR LINE 2148
    console.log('\n🔍 FIELD MAPPINGS FOR LINE 2148:');
    const fieldMappings = await sql.query(`
      SELECT 
        f.Id as FieldId,
        f.Field as FieldName,
        f.EntityType,
        f.LineId
      FROM [OCR-Fields] f
      WHERE f.LineId = 2148
      ORDER BY f.Id
    `);
    console.table(fieldMappings.recordset);

    // 6. CRITICAL ISSUE IDENTIFICATION
    console.log('\n🚨 CRITICAL ISSUE IDENTIFICATION:');
    
    const line2148Data = line2148Simple.recordset[0];
    if (line2148Data) {
      console.log('Line 2148 Configuration:');
      console.log(`- RegExId: ${line2148Data.RegExId}`);
      console.log(`- MultiLine: ${line2148Data.MultiLine}`);
      console.log(`- MaxLines: ${line2148Data.MaxLines}`);
      
      // Compare with Amazon working lines
      const amazonLines = multilineComparison.recordset.filter(r => [40, 1606, 2091].includes(r.LineId));
      const tropicalLine = multilineComparison.recordset.find(r => r.LineId === 2148);
      
      console.log('\nAmazon Working Lines MultiLine Settings:');
      amazonLines.forEach(line => {
        console.log(`- Line ${line.LineId}: MultiLine=${line.MultiLineStatus}, MaxLines=${line.MaxLines}`);
      });
      
      console.log(`\nTropical Vendors Line 2148: MultiLine=${tropicalLine.MultiLineStatus}, MaxLines=${tropicalLine.MaxLines}`);
      
      // Identify the issue
      if (tropicalLine.MultiLine === null) {
        console.log('\n🚨 CRITICAL ISSUE FOUND: MultiLine is NULL for Line 2148!');
        console.log('💡 SOLUTION: Set MultiLine=1 (TRUE) for Line 2148');
        console.log('📝 This explains why the regex pattern works in isolation but fails in OCR pipeline');
      } else if (tropicalLine.MultiLine === 0 && amazonLines.some(a => a.MultiLine === 1)) {
        console.log('\n🚨 CRITICAL ISSUE FOUND: MultiLine is FALSE for Line 2148 while Amazon uses TRUE!');
        console.log('💡 SOLUTION: Set MultiLine=1 (TRUE) for Line 2148');
      } else {
        console.log('\n✅ MultiLine configuration looks correct');
      }
    }

  } catch (error) {
    console.error('❌ Database Error:', error);
  } finally {
    await sql.close();
    console.log('\n🔚 Schema Investigation Complete');
  }
}

// Execute the investigation
checkDatabaseSchema().catch(console.error);
