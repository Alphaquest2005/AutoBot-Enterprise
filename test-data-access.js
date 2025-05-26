// Test data access to both email and database
import { dataHelper } from './data-access-helper.js';

async function testDataAccess() {
  console.log('🧪 Testing Data Access Helper...\n');

  try {
    // Test email access
    console.log('📧 Testing Email Access...');
    const emails = await dataHelper.getRecentEmails(3);
    console.log(`✅ Retrieved ${emails.length} recent emails:`);
    emails.forEach((email, i) => {
      console.log(`${i + 1}. ${email.subject} (from: ${email.from})`);
    });
    console.log('');

    // Test database access
    console.log('🗄️ Testing Database Access...');
    
    // First, let's see what tables exist
    const tables = await dataHelper.executeQuery(`
      SELECT TABLE_NAME 
      FROM INFORMATION_SCHEMA.TABLES 
      WHERE TABLE_TYPE = 'BASE TABLE'
      ORDER BY TABLE_NAME
    `);
    console.log(`✅ Found ${tables.length} tables in database`);
    console.log('First 10 tables:', tables.slice(0, 10).map(t => t.TABLE_NAME).join(', '));
    console.log('');

    // Try to get ApplicationSettings if it exists
    const hasAppSettings = tables.some(t => t.TABLE_NAME.toLowerCase().includes('applicationsettings'));
    if (hasAppSettings) {
      const appSettings = await dataHelper.getTableData('ApplicationSettings', 5);
      console.log(`✅ ApplicationSettings table has ${appSettings.length} records (showing first 5)`);
      console.log(appSettings);
    } else {
      console.log('ℹ️ ApplicationSettings table not found, trying other common tables...');
      
      // Try some other common table names
      const commonTables = ['Users', 'Settings', 'Configuration', 'Documents', 'Emails'];
      for (const tableName of commonTables) {
        const tableExists = tables.some(t => t.TABLE_NAME.toLowerCase() === tableName.toLowerCase());
        if (tableExists) {
          const data = await dataHelper.getTableData(tableName, 3);
          console.log(`✅ ${tableName} table has data:`, data.length, 'records');
          break;
        }
      }
    }

  } catch (error) {
    console.error('❌ Error during testing:', error.message);
  }
}

testDataAccess();
