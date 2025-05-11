# Email MCP Server Usage Guide

This document provides instructions on how to use the tools provided by the `email-mcp-server`.

## Server Name
`email-mcp-server`

## Available Tools

### 1. `hello_world`
- **Description:** A simple tool that echoes back a message.
- **Input Schema:**
  ```json
  {
    "type": "object",
    "properties": {
      "message": {
        "type": "string",
        "description": "The message to echo."
      }
    },
    "required": ["message"]
  }
  ```
- **Example Usage:**
  ```xml
  <use_mcp_tool>
    <server_name>email-mcp-server</server_name>
    <tool_name>hello_world</tool_name>
    <arguments>
      {
        "message": "Test message"
      }
    </arguments>
  </use_mcp_tool>
  ```

### 2. `list_mailboxes`
- **Description:** Lists all available mailboxes on the IMAP server.
- **Input Schema:**
  ```json
  {
    "type": "object",
    "properties": {}
  }
  ```
- **Example Usage:**
  ```xml
  <use_mcp_tool>
    <server_name>email-mcp-server</server_name>
    <tool_name>list_mailboxes</tool_name>
    <arguments>{}</arguments>
  </use_mcp_tool>
  ```
- **Output:** A JSON string representing an array of mailbox objects, e.g.:
  ```json
  [
    { "path": "INBOX", "name": "INBOX", "specialUse": "\\Inbox" },
    { "path": "Sent", "name": "Sent", "specialUse": "\\Sent" }
  ]
  ```

### 3. `list_emails`
- **Description:** Lists emails from a specified mailbox, with optional filters and limit.
- **Input Schema:**
  ```json
  {
    "type": "object",
    "properties": {
      "mailbox": { "type": "string", "description": "Mailbox to search (e.g., 'INBOX'). Defaults to 'INBOX'." },
      "criteria": {
        "type": "object",
        "properties": {
          "since": { "type": "string", "description": "Search for emails since this date (YYYY-MM-DD)." },
          "from": { "type": "string", "description": "Search for emails from this sender." },
          "subject": { "type": "string", "description": "Search for emails with this subject." }
        },
        "additionalProperties": false
      },
      "limit": { "type": "number", "description": "Maximum number of emails to return. Defaults to 20." }
    }
  }
  ```
- **Example Usage:**
  ```xml
  <use_mcp_tool>
    <server_name>email-mcp-server</server_name>
    <tool_name>list_emails</tool_name>
    <arguments>
      {
        "mailbox": "INBOX",
        "criteria": { "since": "2024-01-01", "from": "user@example.com" },
        "limit": 10
      }
    </arguments>
  </use_mcp_tool>
  ```
- **Output:** A JSON string representing an array of email header objects, e.g.:
  ```json
  [
    {
      "uid": "123",
      "subject": "Test Email",
      "from": "sender@example.com",
      "date": "2024-05-10T10:00:00Z",
      "flags": ["\\Seen"]
    }
  ]
  ```

### 4. `get_email_details`
- **Description:** Fetches the full details of a specific email, including body and attachments.
- **Input Schema:**
  ```json
  {
    "type": "object",
    "properties": {
      "mailbox": { "type": "string", "description": "Mailbox where the email resides (e.g., 'INBOX'). Defaults to 'INBOX'." },
      "uid": { "type": "string", "description": "UID of the email to fetch." }
    },
    "required": ["uid"]
  }
  ```
- **Example Usage:**
  ```xml
  <use_mcp_tool>
    <server_name>email-mcp-server</server_name>
    <tool_name>get_email_details</tool_name>
    <arguments>
      {
        "mailbox": "INBOX",
        "uid": "123"
      }
    </arguments>
  </use_mcp_tool>
  ```
- **Output:** A JSON string representing the parsed email object (from `mailparser`), including headers, text body, HTML body, and attachments array.

### 5. `download_attachment`
- **Description:** Downloads a specific attachment from an email. The content is returned as a base64 encoded string.
- **Input Schema:**
  ```json
  {
    "type": "object",
    "properties": {
      "mailbox": { "type": "string", "description": "Mailbox where the email resides. Defaults to 'INBOX'." },
      "uid": { "type": "string", "description": "UID of the email containing the attachment." },
      "attachmentId": { "type": "string", "description": "Identifier for the attachment (zero-based index from the 'attachments' array in `get_email_details` output)." }
    },
    "required": ["uid", "attachmentId"]
  }
  ```
- **Example Usage:**
  ```xml
  <use_mcp_tool>
    <server_name>email-mcp-server</server_name>
    <tool_name>download_attachment</tool_name>
    <arguments>
      {
        "mailbox": "INBOX",
        "uid": "123",
        "attachmentId": "0" 
      }
    </arguments>
  </use_mcp_tool>
  ```
- **Output:** A JSON string containing the attachment's filename, contentType, size, and contentBase64, e.g.:
  ```json
  {
    "filename": "document.pdf",
    "contentType": "application/pdf",
    "size": 102400,
    "contentId": "some-content-id",
    "contentBase64": "JVBERi0xLjQKJe..."
  }
  ```

## Environment Variables
The server requires the following environment variables to be set by the MCP host (e.g., in `mcp_settings.json`):
- `IMAP_HOST`: IMAP server hostname (e.g., `mail.auto-brokerage.com`)
- `IMAP_PORT`: IMAP server port (e.g., `993`)
- `EMAIL_USER`: Email account username
- `EMAIL_PASSWORD`: Email account password
- `MCP_LOG_LEVEL` (optional): Set to `debug` for verbose IMAP logging.