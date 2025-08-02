# CLAUDE-Configuration.md - System Configuration Guide

Database, MCP server, and tool configuration for AutoBot-Enterprise environments.

## 🗄️ **DATABASE CONFIGURATION**

### **SQL Server Setup**
AutoBot-Enterprise requires SQL Server with the WebSource-AutoBot database.

**Configuration Template:**
```
Server: {DATABASE_SERVER}
Database: WebSource-AutoBot  
Authentication: SQL Server Authentication
Username: sa
Password: {DATABASE_PASSWORD}
```

### **Environment-Specific Examples:**
```bash
# Local Development Example
DATABASE_SERVER = "localhost\SQLEXPRESS"
DATABASE_PASSWORD = "your_sa_password"

# Network SQL Server Example  
DATABASE_SERVER = "SERVERNAME\INSTANCENAME"
DATABASE_PASSWORD = "your_secure_password"

# Default Port SQL Server Example
DATABASE_SERVER = "SERVERNAME,1433"
DATABASE_PASSWORD = "your_secure_password"
```

### **Connection String Format:**
```
Server={DATABASE_SERVER};Database=WebSource-AutoBot;User Id=sa;Password={DATABASE_PASSWORD};
```

## 🔌 **MCP SQL SERVER SETUP**

The Model Context Protocol (MCP) server provides Claude Code with direct database access for queries and analysis.

### **MCP Server Setup**

**1. Navigate to MCP Server Directory:**
```bash
# Adapt path for your environment:
# Main repo: {REPO_ROOT}/mcp-servers/mssql-mcp-server
# Alpha worktree: {REPO_ROOT}-alpha/mcp-servers/mssql-mcp-server  
# Beta worktree: {REPO_ROOT}-beta/mcp-servers/mssql-mcp-server

cd "{MCP_SERVER_PATH}/mcp-servers/mssql-mcp-server"
npm start
```

**2. Environment File Configuration:**
Create `.env` file in MCP server directory:
```bash
# .env file content (adapt for your environment)
DB_SERVER={DATABASE_SERVER}
DB_DATABASE=WebSource-AutoBot
DB_USERNAME=sa
DB_PASSWORD={DATABASE_PASSWORD}
```

**3. Password Handling:**
```bash
# Node.js dotenv requires double $ for literal $ character
# If your password is: pa$word
# Use in .env file: DB_PASSWORD=pa$$word
```

### **MCP Server Configuration Details**
- **Transport**: stdio (for Claude Code integration)
- **Purpose**: Direct database access for Claude Code queries
- **Location**: Repository-relative path `/mcp-servers/mssql-mcp-server/`
- **Network**: Bypasses WSL2 networking complexity when run on Windows

### **MCP Server Usage**
Once running, Claude Code can execute queries like:
- "Show me tables in WebSource-AutoBot database"
- "Query the OCR_TemplateTableMapping table"  
- "Execute SELECT * FROM [table_name] LIMIT 10"

### **Troubleshooting MCP Server**

**Test SQL Connection:**
```bash
# Windows
sqlcmd -S "{DATABASE_SERVER}" -U sa -P "{DATABASE_PASSWORD}" -Q "SELECT @@SERVERNAME"

# If using complex password, escape special characters
sqlcmd -S "{DATABASE_SERVER}" -U sa -P "pa`$word" -Q "SELECT @@SERVERNAME"
```

**Verify .env File:**
```bash
# Windows PowerShell
Get-Content "{MCP_SERVER_PATH}/mcp-servers/mssql-mcp-server/.env" | Select-String "DB_"

# Linux/WSL2
grep "DB_" "{MCP_SERVER_PATH}/mcp-servers/mssql-mcp-server/.env"
```

## ⚙️ **APPLICATION CONFIGURATION**

### **Test Configuration**
```bash
# Test configuration file
./AutoBotUtilities.Tests/appsettings.json
```

Key test settings to verify:
- Database connection strings
- Logging configuration  
- DeepSeek API settings
- File path configurations

### **Main Application Configuration**
```bash
# Main application configuration
./AutoBot/App.config
```

Contains:
- Entity Framework connection strings
- Application settings
- Service configurations

### **Visual Studio Configuration**
Ensure Visual Studio 2022 with:
- .NET Framework 4.8 development tools
- Entity Framework 6 support
- SQL Server Data Tools (optional, for database work)

## 🎛️ **CLAUDE CODE CONFIGURATION**

### **Claude Settings Location**
Claude Code settings are stored in the user's Claude configuration directory:
- **Windows**: `%USERPROFILE%\.claude\settings.json`
- **Linux/WSL2**: `~/.claude/settings.json`

### **MCP Integration**
The MCP server configuration should already be present in Claude settings if previously configured. No manual Claude settings modification should be required.

## 🔧 **ENVIRONMENT VARIABLES**

### **For Development**
```bash
# Optional environment variables for development
AUTOBOT_DATABASE_SERVER={DATABASE_SERVER}
AUTOBOT_DATABASE_PASSWORD={DATABASE_PASSWORD}
DEEPSEEK_API_KEY={YOUR_DEEPSEEK_API_KEY}
```

### **For Testing**
```bash
# Test environment variables
TEST_DATABASE_SERVER={DATABASE_SERVER}
TEST_LOG_LEVEL=Verbose
```

## 🛡️ **SECURITY CONSIDERATIONS**

### **Password Management**
- Store passwords securely (consider Windows Credential Manager, environment variables, or secure configuration)
- Avoid hardcoding passwords in configuration files
- Use `.env` files for development (ensure they're in .gitignore)

### **Network Security** 
- SQL Server should allow connections from development machine
- Consider firewall rules for database access
- MCP server runs locally and doesn't expose network endpoints

### **Database Permissions**
- `sa` account used for development simplicity
- Production environments should use dedicated service accounts with minimal required permissions

## 📋 **CONFIGURATION VALIDATION**

### **Database Connection Test**
```bash
# Test database connectivity
sqlcmd -S "{DATABASE_SERVER}" -U sa -P "{DATABASE_PASSWORD}" -Q "SELECT COUNT(*) FROM ApplicationSettings"
```

### **MCP Server Test**  
```bash
# Verify MCP server is running (check for npm process)
# Windows
tasklist | findstr node

# Linux/WSL2
ps aux | grep node
```

### **Application Test**
```bash
# Build and run basic application test
{VISUAL_STUDIO_PATH}/MSBuild/Current/Bin/MSBuild.exe "./AutoBotUtilities.Tests/AutoBotUtilities.Tests.csproj" /t:Rebuild /p:Configuration=Debug /p:Platform=x64
```

## 🚀 **QUICK SETUP CHECKLIST**

1. ✅ **SQL Server accessible** with WebSource-AutoBot database
2. ✅ **Database credentials configured** (sa account with password)
3. ✅ **MCP server directory located** at {REPO_ROOT}/mcp-servers/mssql-mcp-server/
4. ✅ **MCP .env file created** with correct database settings
5. ✅ **Visual Studio 2022** installed with .NET Framework 4.8 support
6. ✅ **Build test successful** using environment-specific paths
7. ✅ **MCP server running** with `npm start`
8. ✅ **Claude Code connected** to MCP server for database queries

---

*This configuration guide uses environment-agnostic placeholders. Replace {DATABASE_SERVER}, {DATABASE_PASSWORD}, {MCP_SERVER_PATH}, and {REPO_ROOT} with your environment-specific values.*