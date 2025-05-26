// Get OCR table details and DeepSeek prompt information
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

async function getOCRDetails() {
  try {
    await sql.connect(dbConfig);
    
    console.log('=== OCR-Fields Table Schema ===');
    const ocrFields = await sql.query(`
      SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE, COLUMN_DEFAULT
      FROM INFORMATION_SCHEMA.COLUMNS 
      WHERE TABLE_NAME = 'OCR-Fields'
      ORDER BY ORDINAL_POSITION
    `);
    console.log(ocrFields.recordset);
    
    console.log('\n=== Sample OCR-Fields Data ===');
    const sampleFields = await sql.query('SELECT TOP 5 * FROM [OCR-Fields]');
    console.log(sampleFields.recordset);
    
    console.log('\n=== OCR-FieldFormatRegEx Schema ===');
    const regexSchema = await sql.query(`
      SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE, COLUMN_DEFAULT
      FROM INFORMATION_SCHEMA.COLUMNS 
      WHERE TABLE_NAME = 'OCR-FieldFormatRegEx'
      ORDER BY ORDINAL_POSITION
    `);
    console.log(regexSchema.recordset);
    
    console.log('\n=== Sample OCR-FieldFormatRegEx Data ===');
    const sampleRegex = await sql.query('SELECT TOP 5 * FROM [OCR-FieldFormatRegEx]');
    console.log(sampleRegex.recordset);
    
    console.log('\n=== Check for DeepSeek API Configuration ===');
    const deepseekConfig = await sql.query(`
      SELECT * FROM ApplicationSettings 
      WHERE Description LIKE '%deepseek%' OR Description LIKE '%prompt%' OR Description LIKE '%api%'
    `);
    console.log(deepseekConfig.recordset);
    
  } catch (error) {
    console.error('Error:', error.message);
  } finally {
    await sql.close();
  }
}

getOCRDetails();
