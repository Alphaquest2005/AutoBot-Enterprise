// Simple MCP SQL test
const sql = require('mssql');

const config = {
    user: 'sa',
    password: 'pa$$word', 
    server: 'MINIJOE\\SQLDEVELOPER2022',
    database: 'WebSource-AutoBot',
    options: {
        encrypt: false,
        trustServerCertificate: true
    }
};

async function test() {
    try {
        console.log('🔗 Connecting to SQL Server...');
        await sql.connect(config);
        const result = await sql.query('SELECT @@SERVERNAME AS ServerName, DB_NAME() AS Database');
        console.log('✅ Connection successful!');
        console.log('Server:', result.recordset[0].ServerName);
        console.log('Database:', result.recordset[0].Database);
        console.log('🚀 MCP server can connect! Ready to configure Claude Desktop.');
    } catch (err) {
        console.log('❌ Connection failed:', err.message);
    }
    process.exit();
}

test();