// Test email connection and retrieve latest emails
import { ImapFlow } from 'imapflow';
import dotenv from 'dotenv';

dotenv.config({ path: './.env' });

const emailConfig = {
  host: process.env.IMAP_HOST,
  port: parseInt(process.env.IMAP_PORT),
  secure: true,
  auth: {
    user: process.env.EMAIL_USER,
    pass: process.env.EMAIL_PASSWORD
  }
};

async function testEmailConnection() {
  console.log('Testing email connection...');
  console.log('Config:', {
    host: emailConfig.host,
    port: emailConfig.port,
    user: emailConfig.auth.user
  });

  const client = new ImapFlow(emailConfig);

  try {
    await client.connect();
    console.log('✅ Connected to email server');

    await client.mailboxOpen('INBOX');
    console.log('✅ Opened INBOX');

    // Get the 3 most recent messages
    const messages = [];
    for await (const message of client.fetch('1:3', { envelope: true })) {
      messages.push({
        id: message.uid,
        subject: message.envelope.subject,
        from: message.envelope.from?.[0]?.address || 'Unknown',
        date: message.envelope.date
      });
    }

    console.log('✅ Latest emails:');
    messages.forEach((msg, i) => {
      console.log(`${i + 1}. Subject: ${msg.subject}`);
      console.log(`   From: ${msg.from}`);
      console.log(`   Date: ${msg.date}`);
      console.log('');
    });

  } catch (error) {
    console.error('❌ Error:', error.message);
  } finally {
    await client.logout();
  }
}

testEmailConnection();
