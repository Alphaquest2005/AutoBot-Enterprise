// Get database schema information
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

async function getSchemaInfo() {
  try {
    await sql.connect(dbConfig);

    console.log('=== OCR_Fields Table Schema ===');
    const ocrFields = await sql.query(`
      SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE, COLUMN_DEFAULT
      FROM INFORMATION_SCHEMA.COLUMNS
      WHERE TABLE_NAME = 'OCR_Fields'
      ORDER BY ORDINAL_POSITION
    `);
    console.log(ocrFields.recordset);

    console.log('\n=== ShipmentInvoice Table Schema ===');
    const shipmentInvoice = await sql.query(`
      SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE, COLUMN_DEFAULT
      FROM INFORMATION_SCHEMA.COLUMNS
      WHERE TABLE_NAME = 'ShipmentInvoice'
      ORDER BY ORDINAL_POSITION
    `);
    console.log(shipmentInvoice.recordset);

    console.log('\n=== ShipmentInvoiceDetails Table Schema ===');
    const shipmentInvoiceDetails = await sql.query(`
      SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE, COLUMN_DEFAULT
      FROM INFORMATION_SCHEMA.COLUMNS
      WHERE TABLE_NAME = 'ShipmentInvoiceDetails'
      ORDER BY ORDINAL_POSITION
    `);
    console.log(shipmentInvoiceDetails.recordset);

    console.log('\n=== Tables containing OCR or Fields ===');
    const ocrTables = await sql.query(`
      SELECT TABLE_NAME
      FROM INFORMATION_SCHEMA.TABLES
      WHERE TABLE_TYPE = 'BASE TABLE'
      AND (TABLE_NAME LIKE '%OCR%' OR TABLE_NAME LIKE '%Field%' OR TABLE_NAME LIKE '%Regex%')
      ORDER BY TABLE_NAME
    `);
    console.log(ocrTables.recordset);

    console.log('\n=== DeepSeek Related Data ===');
    const deepseekData = await sql.query(`
      SELECT TOP 5 * FROM ApplicationSettings
    `);
    console.log(deepseekData.recordset);

  } catch (error) {
    console.error('Error:', error.message);
  } finally {
    await sql.close();
  }
}

getSchemaInfo();
