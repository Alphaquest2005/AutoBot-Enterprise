// Helper script to access email and database data for task assistance
import { ImapFlow } from 'imapflow';
import sql from 'mssql';
import dotenv from 'dotenv';

// Load email configuration
dotenv.config({ path: './email-mcp-server/.env' });

// Email configuration
const emailConfig = {
  host: process.env.IMAP_HOST,
  port: parseInt(process.env.IMAP_PORT),
  secure: true,
  auth: {
    user: process.env.EMAIL_USER,
    pass: process.env.EMAIL_PASSWORD
  }
};

// Database configuration
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

export class DataAccessHelper {
  
  // Get recent emails
  async getRecentEmails(limit = 10) {
    const client = new ImapFlow(emailConfig);
    try {
      await client.connect();
      await client.mailboxOpen('INBOX');
      
      const messages = [];
      for await (const message of client.fetch(`1:${limit}`, { envelope: true, bodyStructure: true })) {
        messages.push({
          id: message.uid,
          subject: message.envelope.subject,
          from: message.envelope.from?.[0]?.address || 'Unknown',
          date: message.envelope.date,
          hasAttachments: message.bodyStructure?.childNodes?.length > 0
        });
      }
      
      return messages.sort((a, b) => b.date.getTime() - a.date.getTime());
    } finally {
      await client.logout();
    }
  }

  // Search emails by criteria
  async searchEmails(criteria) {
    const client = new ImapFlow(emailConfig);
    try {
      await client.connect();
      await client.mailboxOpen('INBOX');
      
      const uids = await client.search(criteria, { uid: true });
      const messages = [];
      
      if (uids.length > 0) {
        for await (const message of client.fetch(uids.slice(0, 20), { envelope: true })) {
          messages.push({
            id: message.uid,
            subject: message.envelope.subject,
            from: message.envelope.from?.[0]?.address || 'Unknown',
            date: message.envelope.date
          });
        }
      }
      
      return messages;
    } finally {
      await client.logout();
    }
  }

  // Execute SQL query
  async executeQuery(query) {
    try {
      await sql.connect(dbConfig);
      const result = await sql.query(query);
      return result.recordset;
    } catch (error) {
      console.error('Database query error:', error);
      throw error;
    } finally {
      await sql.close();
    }
  }

  // Get table data
  async getTableData(tableName, limit = 10) {
    const query = `SELECT TOP ${limit} * FROM ${tableName}`;
    return await this.executeQuery(query);
  }

  // Get database schema info
  async getTableSchema(tableName) {
    const query = `
      SELECT 
        COLUMN_NAME,
        DATA_TYPE,
        IS_NULLABLE,
        COLUMN_DEFAULT
      FROM INFORMATION_SCHEMA.COLUMNS 
      WHERE TABLE_NAME = '${tableName}'
      ORDER BY ORDINAL_POSITION
    `;
    return await this.executeQuery(query);
  }
}

// Export instance for use
export const dataHelper = new DataAccessHelper();
