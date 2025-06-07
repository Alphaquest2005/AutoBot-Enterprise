const sql = require('mssql');

const dbConfig = {
  user: 'sa',
  password: 'pa$word',
  server: 'MINIJOE\\SQLDEVELOPER2022',
  database: 'WebSource-AutoBot',
  options: {
    encrypt: false,
    trustServerCertificate: true
  }
};

async function checkTropicalVendorsEntityType() {
  try {
    await sql.connect(dbConfig);
    console.log('Connected to database\n');

    // First, let's check if we have the right table names
    console.log('=== OCR Tables in Database ===');
    const tables = await sql.query`
      SELECT TABLE_NAME 
      FROM INFORMATION_SCHEMA.TABLES 
      WHERE TABLE_NAME LIKE 'OCR%' OR TABLE_NAME LIKE 'OCR_%'
      ORDER BY TABLE_NAME`;
    
    tables.recordset.forEach(t => console.log(t.TABLE_NAME));

    // Check for template 213
    console.log('\n=== Template 213 (Tropical Vendors) ===');
    const template = await sql.query`
      SELECT * FROM OCR_Templates WHERE Id = 213`;
    
    if (template.recordset.length === 0) {
      // Try other table names
      const altTemplate = await sql.query`
        SELECT * FROM [OCR-Invoices] WHERE Id = 213`;
      if (altTemplate.recordset.length > 0) {
        console.log('Found in OCR-Invoices table:', altTemplate.recordset[0]);
      }
    } else {
      console.log('Template found:', template.recordset[0]);
    }

    // Get parts for template 213
    console.log('\n=== Parts for Template 213 ===');
    const parts = await sql.query`
      SELECT 
        p.Id AS PartId,
        p.Name AS PartName,
        p.PartType,
        p.Parent_Id,
        p.Template_Id
      FROM OCR_Parts p
      WHERE p.Template_Id = 213
      ORDER BY COALESCE(p.Parent_Id, p.Id), p.Id`;
    
    if (parts.recordset.length === 0) {
      // Try alternative table name
      const altParts = await sql.query`
        SELECT 
          p.Id AS PartId,
          p.TemplateId,
          p.PartTypeId,
          p.ParentId
        FROM [OCR-Parts] p
        WHERE p.TemplateId = 213
        ORDER BY COALESCE(p.ParentId, p.Id), p.Id`;
      
      if (altParts.recordset.length > 0) {
        console.log('Found parts in OCR-Parts table:');
        altParts.recordset.forEach(p => console.log(p));
      }
    } else {
      parts.recordset.forEach(p => {
        const indent = p.Parent_Id ? '  └─ ' : '';
        console.log(`${indent}Part ${p.PartId}: ${p.PartName} (Parent: ${p.Parent_Id})`);
      });
    }

    // Check fields for EntityType
    console.log('\n=== Fields with EntityType ===');
    
    // Try different table/column combinations
    const queries = [
      // Standard naming
      `SELECT 
        p.Id AS PartId,
        p.Name AS PartName,
        p.Parent_Id,
        l.Id AS LineId,
        l.Name AS LineName,
        f.Id AS FieldId,
        f.Name AS FieldName,
        f.EntityType,
        f.DatabaseFieldName
      FROM OCR_Parts p
      LEFT JOIN OCR_Lines l ON l.Part_Id = p.Id
      LEFT JOIN OCR_Fields f ON f.Line_Id = l.Id
      WHERE p.Template_Id = 213 AND p.Parent_Id IS NOT NULL AND f.EntityType IS NOT NULL`,
      
      // Alternative naming with dashes
      `SELECT 
        p.Id AS PartId,
        p.TemplateId,
        p.ParentId,
        l.Id AS LineId,
        l.Name AS LineName,
        f.Id AS FieldId,
        f.Field AS FieldName,
        f.EntityType,
        f.FieldValue
      FROM [OCR-Parts] p
      LEFT JOIN [OCR-Lines] l ON l.PartId = p.Id
      LEFT JOIN [OCR-Fields] f ON f.LineId = l.Id
      WHERE p.TemplateId = 213 AND p.ParentId IS NOT NULL`
    ];

    for (let i = 0; i < queries.length; i++) {
      try {
        console.log(`\nTrying query ${i + 1}...`);
        const result = await sql.query(queries[i]);
        
        if (result.recordset.length > 0) {
          console.log('Results found:');
          result.recordset.forEach(row => {
            console.log(`\nPart ${row.PartId} (Parent: ${row.ParentId || row.Parent_Id})`);
            console.log(`  Field: ${row.FieldName}`);
            console.log(`  EntityType: "${row.EntityType || 'NULL'}"`);
            console.log(`  DatabaseField/Value: ${row.DatabaseFieldName || row.FieldValue || 'NULL'}`);
          });
          break;
        }
      } catch (err) {
        console.log(`Query ${i + 1} failed: ${err.message}`);
      }
    }

    // Get column info for Fields table
    console.log('\n=== OCR Fields Table Schema ===');
    const fieldsSchema = await sql.query`
      SELECT COLUMN_NAME, DATA_TYPE 
      FROM INFORMATION_SCHEMA.COLUMNS 
      WHERE TABLE_NAME LIKE '%OCR%Field%' 
      ORDER BY TABLE_NAME, ORDINAL_POSITION`;
    
    const uniqueTables = new Set();
    fieldsSchema.recordset.forEach(col => {
      if (col.COLUMN_NAME.toLowerCase().includes('entity')) {
        console.log(`Found column: ${col.COLUMN_NAME} (${col.DATA_TYPE})`);
      }
    });

  } catch (err) {
    console.error('Error:', err);
  } finally {
    await sql.close();
  }
}

checkTropicalVendorsEntityType();