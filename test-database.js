// Test database connection
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

async function testDatabase() {
  console.log('🗄️ Testing Database Connection...');
  
  try {
    await sql.connect(dbConfig);
    console.log('✅ Connected to SQL Server');

    // Get list of tables
    const tables = await sql.query(`
      SELECT TABLE_NAME 
      FROM INFORMATION_SCHEMA.TABLES 
      WHERE TABLE_TYPE = 'BASE TABLE'
      ORDER BY TABLE_NAME
    `);
    
    console.log(`✅ Found ${tables.recordset.length} tables`);
    console.log('First 10 tables:', tables.recordset.slice(0, 10).map(t => t.TABLE_NAME).join(', '));

    // Try ApplicationSettings table
    try {
      const appSettings = await sql.query('SELECT TOP 3 * FROM ApplicationSettings');
      console.log(`✅ ApplicationSettings table has ${appSettings.recordset.length} records`);
      console.log('Sample data:', appSettings.recordset);
    } catch (err) {
      console.log('ℹ️ ApplicationSettings table not found or accessible');
    }

  } catch (error) {
    console.error('❌ Database error:', error.message);
  } finally {
    await sql.close();
  }
}

testDatabase();
