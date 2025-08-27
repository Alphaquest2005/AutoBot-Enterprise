# DATABASE AND MCP SETUP - AutoBot-Enterprise

> **🗄️ Complete Database & MCP Configuration** - SQL Server access and Model Context Protocol setup

## 📋 TABLE OF CONTENTS

1. [**🚀 QUICK START**](#quick-start) - Get MCP running in 2 minutes
2. [**🗄️ MCP SQL Server Setup**](#mcp-sql-server-setup) - Complete configuration guide
3. [**💾 Database Configuration**](#database-configuration) - Connection details and credentials
4. [**🔧 Troubleshooting**](#troubleshooting) - Common issues and solutions
5. [**🧪 Testing & Verification**](#testing-verification) - Verify setup and connectivity
6. [**📊 Database Operations**](#database-operations) - Common queries and commands
7. [**🔍 Monitoring & Maintenance**](#monitoring-maintenance) - Health checks and performance

---

## 🚀 QUICK START {#quick-start}

### **⚡ 2-Minute MCP Setup** (Working Configuration)

#### **🔍 Environment Detection First**
```bash
# Identify current repository location
echo "Repository Root: $(git rev-parse --show-toplevel)"
echo "Current Environment: $(pwd)"
echo "Current Branch: $(git branch --show-current)"
```

#### **🚀 Start MCP Server** (Location-Agnostic)
```powershell
# Windows PowerShell - Navigate to MCP server directory (works from any worktree)
$RepoRoot = git rev-parse --show-toplevel
$McpPath = "$RepoRoot\mcp-servers\mssql-mcp-server" -replace '/', '\'
cd $McpPath
echo "Starting MCP server from: $(pwd)"

# Start MCP Server
npm start
```

```bash
# WSL/Bash - Navigate to MCP server directory (works from any worktree)
REPO_ROOT=$(git rev-parse --show-toplevel)
MCP_PATH="$REPO_ROOT/mcp-servers/mssql-mcp-server"
cd "$MCP_PATH"
echo "Starting MCP server from: $(pwd)"

# Start MCP Server
npm start
```

**Result**: Claude Code now has direct SQL Server access for database queries and analysis

### **✅ Verify Working**
Once MCP server is running, test Claude Code queries:
- "Show me tables in WebSource-AutoBot database"
- "Query the OCR_TemplateTableMapping table"  
- "Execute SELECT TOP 10 * FROM OCRCorrectionLearning ORDER BY Id DESC"

---

## 🗄️ MCP SQL Server Setup {#mcp-sql-server-setup}

### **Configuration Overview**

The Model Context Protocol (MCP) provides Claude Code with direct SQL Server database access for real-time querying, analysis, and troubleshooting of the AutoBot-Enterprise system.

#### **✅ Pre-Configured Setup**
- **Database Server**: `MINIJOE\SQLDEVELOPER2022`
- **Database Name**: `WebSource-AutoBot`
- **Authentication**: SQL Server Authentication
- **Credentials**: `sa` / `pa$word` (literal password with single $)
- **MCP Location**: `<REPO_ROOT>/mcp-servers/mssql-mcp-server/` (location-agnostic)
- **Claude Settings**: Already configured in `/home/joseph/.claude/settings.json`

#### **🔍 Verify MCP Location** (Any Worktree)
```bash
# Verify MCP server directory exists in current environment
REPO_ROOT=$(git rev-parse --show-toplevel)
MCP_PATH="$REPO_ROOT/mcp-servers/mssql-mcp-server"
echo "Expected MCP path: $MCP_PATH"
ls -la "$MCP_PATH" && echo "✅ MCP directory found" || echo "❌ MCP directory not found"
ls -la "$MCP_PATH/.env" && echo "✅ .env file found" || echo "❌ .env file not found"
ls -la "$MCP_PATH/package.json" && echo "✅ package.json found" || echo "❌ package.json not found"
```

### **Key Architecture Decisions**

#### **🔧 Key Issues Resolved**
- **Password Escaping**: Use `pa$$word` in .env file (Node.js dotenv interprets as `pa$word`)
- **Network Configuration**: Run MCP server on Windows (bypasses WSL2 networking complexity)
- **Transport Protocol**: Uses stdio transport for Claude Code integration
- **Security**: Authenticated SQL Server access with proper credential management

#### **🏗️ Technical Implementation**
- **Runtime**: Node.js-based MCP server
- **Protocol**: stdio transport for reliable communication
- **Database Driver**: SQL Server-compatible drivers
- **Configuration**: Environment-based configuration with .env file

---

## 💾 Database Configuration {#database-configuration}

### **Primary Database Connection**

#### **Connection String Details**
```
Server: MINIJOE\SQLDEVELOPER2022
Database: WebSource-AutoBot
Authentication: SQL Server Authentication
Username: sa
Password: pa$word
```

#### **Environment Configuration** (`.env` file) - Location-Agnostic
```bash
# Expected .env content in <REPO_ROOT>/mcp-servers/mssql-mcp-server/.env
DB_PASSWORD=pa$$word      # Double $ for Node.js dotenv interpretation
DB_SERVER=MINIJOE\SQLDEVELOPER2022
DB_DATABASE=WebSource-AutoBot
DB_USER=sa
```

#### **🔧 Verify .env Configuration** (Any Worktree)
```bash
# Check .env file contents (works from any worktree)
REPO_ROOT=$(git rev-parse --show-toplevel)
ENV_FILE="$REPO_ROOT/mcp-servers/mssql-mcp-server/.env"
echo "Checking .env file: $ENV_FILE"
if [ -f "$ENV_FILE" ]; then
    echo "✅ .env file found"
    grep "^DB_" "$ENV_FILE" | head -5
else
    echo "❌ .env file not found at: $ENV_FILE"
fi
```

```powershell
# PowerShell - Check .env file contents (works from any worktree)
$RepoRoot = git rev-parse --show-toplevel
$EnvFile = "$RepoRoot\mcp-servers\mssql-mcp-server\.env" -replace '/', '\'
echo "Checking .env file: $EnvFile"
if (Test-Path $EnvFile) {
    echo "✅ .env file found"
    Get-Content $EnvFile | Select-String "^DB_" | Select-Object -First 5
} else {
    echo "❌ .env file not found at: $EnvFile"
}
```

### **Database Schema Overview**

#### **Core OCR Tables**
- **`OCR-Invoices`** - Invoice template definitions
- **`OCR-Parts`** - Template sections (Header, LineItems, Footer)
- **`OCR-Lines`** - Field groupings with regex patterns  
- **`OCR-Fields`** - Individual data fields with validation rules
- **`OCR-RegularExpressions`** - Regex pattern library
- **`OCR-FieldFormatRegEx`** - Field-to-regex mapping relationships

#### **Learning & Correction Tables**
- **`OCRCorrectionLearning`** - AI learning data and corrections
- **`TemplateTableMapping`** - Template-to-table relationship mapping
- **`ApplicationSettings`** - System configuration parameters

#### **Business Data Tables**
- **`Invoices`** - Processed invoice records
- **`ShipmentInvoices`** - Customs shipment invoice data
- **`Fields`** - Business field definitions and mappings

---

## 🔧 Troubleshooting {#troubleshooting}

### **Common Setup Issues**

#### **🔍 Verify Database Connectivity**
```powershell
# Test SQL connection directly
sqlcmd -S "MINIJOE\SQLDEVELOPER2022" -U sa -P "pa`$word" -Q "SELECT @@SERVERNAME"

# Expected output: MINIJOE\SQLDEVELOPER2022
```

#### **🔍 Verify MCP Configuration** (Location-Agnostic)
```powershell
# Check .env file contents (works from any worktree)
$RepoRoot = git rev-parse --show-toplevel
$EnvFile = "$RepoRoot\mcp-servers\mssql-mcp-server\.env" -replace '/', '\'
Get-Content $EnvFile | Select-String "DB_"

# Expected output:
# DB_PASSWORD=pa$$word
# DB_SERVER=MINIJOE\SQLDEVELOPER2022  
# DB_DATABASE=WebSource-AutoBot
```

#### **🔍 Verify MCP Server Status** (Location-Agnostic)
```powershell
# Check if MCP server is running (look for node.js process)
Get-Process node

# Check MCP server directory exists (works from any worktree)
$RepoRoot = git rev-parse --show-toplevel
$McpPath = "$RepoRoot\mcp-servers\mssql-mcp-server" -replace '/', '\'
Test-Path $McpPath
echo "MCP Path: $McpPath"
```

```bash
# WSL/Bash - Check MCP server status (works from any worktree)
# Check if MCP server is running
ps aux | grep node | grep -v grep

# Check MCP server directory exists
REPO_ROOT=$(git rev-parse --show-toplevel)
MCP_PATH="$REPO_ROOT/mcp-servers/mssql-mcp-server"
ls -la "$MCP_PATH" && echo "✅ MCP directory exists" || echo "❌ MCP directory not found"
echo "MCP Path: $MCP_PATH"
```

### **Password Escaping Issues**

#### **❌ Common Problem**: Password contains special characters
```bash
# WRONG - Single $ gets interpreted by dotenv
DB_PASSWORD=pa$word

# CORRECT - Double $$ escapes to single $
DB_PASSWORD=pa$$word
```

#### **❌ PowerShell Escaping**: When using sqlcmd in PowerShell
```powershell
# WRONG - PowerShell interprets $ as variable
sqlcmd -S "MINIJOE\SQLDEVELOPER2022" -U sa -P "pa$word"

# CORRECT - Escape with backtick
sqlcmd -S "MINIJOE\SQLDEVELOPER2022" -U sa -P "pa`$word"
```

### **Network & Connectivity Issues**

#### **🔧 WSL2 Network Bypass**
- **Issue**: WSL2 networking can complicate database connections
- **Solution**: Run MCP server on Windows host, not within WSL2
- **Result**: Direct Windows-to-Windows SQL Server connectivity

#### **🔧 Firewall Configuration**
```powershell
# Verify SQL Server port accessibility (if needed)
Test-NetConnection -ComputerName "MINIJOE" -Port 1433
```

---

## 🧪 Testing & Verification {#testing-verification}

### **MCP Functionality Tests**

#### **✅ Basic Connectivity Test**
```sql
-- Via Claude Code MCP: "Execute this query"
SELECT @@SERVERNAME as ServerName, DB_NAME() as DatabaseName, GETDATE() as CurrentTime
```

#### **✅ OCR System Verification**  
```sql
-- Via Claude Code MCP: "Show OCR template count"
SELECT COUNT(*) as TemplateCount FROM [OCR-Invoices]
```

#### **✅ Learning System Status**
```sql
-- Via Claude Code MCP: "Check recent OCR corrections"
SELECT TOP 10 FieldName, Success, CreatedDate 
FROM OCRCorrectionLearning 
ORDER BY CreatedDate DESC
```

### **Database Schema Verification**

#### **📋 Core Table Existence Check**
```sql
-- Via Claude Code MCP: "Verify OCR tables exist"
SELECT TABLE_NAME 
FROM INFORMATION_SCHEMA.TABLES 
WHERE TABLE_NAME LIKE 'OCR%' 
ORDER BY TABLE_NAME
```

#### **📊 Data Volume Check**
```sql
-- Via Claude Code MCP: "Show table sizes"
SELECT 
    t.TABLE_NAME,
    ISNULL(p.rows, 0) as RowCount
FROM INFORMATION_SCHEMA.TABLES t
LEFT JOIN (
    SELECT 
        OBJECT_NAME(OBJECT_ID) as TableName,
        SUM(rows) as rows
    FROM sys.partitions 
    WHERE index_id IN (0,1)
    GROUP BY OBJECT_ID
) p ON t.TABLE_NAME = p.TableName
WHERE t.TABLE_TYPE = 'BASE TABLE'
ORDER BY p.rows DESC
```

---

## 📊 Database Operations {#database-operations}

### **Common Diagnostic Queries**

#### **🔍 OCR Correction Analysis**
```sql
-- Recent OCR corrections with success status
SELECT TOP 20 
    FieldName,
    OriginalError,
    CorrectValue,
    Success,
    Confidence,
    CreatedDate
FROM OCRCorrectionLearning 
ORDER BY CreatedDate DESC
```

#### **🔍 Template Analysis**
```sql
-- Template structure analysis
SELECT 
    i.Name as TemplateName,
    COUNT(DISTINCT p.Id) as PartCount,
    COUNT(DISTINCT l.Id) as LineCount,
    COUNT(DISTINCT f.Id) as FieldCount
FROM [OCR-Invoices] i
LEFT JOIN [OCR-Parts] p ON i.Id = p.TemplateId
LEFT JOIN [OCR-Lines] l ON p.Id = l.PartId  
LEFT JOIN [OCR-Fields] f ON l.Id = f.LineId
GROUP BY i.Id, i.Name
ORDER BY i.Name
```

#### **🔍 Regex Pattern Analysis**
```sql
-- Active regex patterns
SELECT 
    Id,
    LEFT(RegEx, 100) as RegexPattern,
    Description,
    CreatedDate
FROM [OCR-RegularExpressions]
WHERE CreatedDate >= DATEADD(day, -30, GETDATE())
ORDER BY CreatedDate DESC
```

### **System Health Queries**

#### **📈 Performance Metrics**
```sql
-- Database size and growth
SELECT 
    database_name = DB_NAME(),
    data_size_mb = CAST(SUM(CASE WHEN type = 0 THEN size END) * 8.0 / 1024 AS DECIMAL(10,2)),
    log_size_mb = CAST(SUM(CASE WHEN type = 1 THEN size END) * 8.0 / 1024 AS DECIMAL(10,2))
FROM sys.database_files
```

#### **🔄 Recent Activity**
```sql
-- Recent invoice processing activity
SELECT TOP 10
    Name,
    ApplicationSettingsId,
    CreatedDate,
    FileTypeId
FROM Invoices 
ORDER BY CreatedDate DESC
```

---

## 🔍 Monitoring & Maintenance {#monitoring-maintenance}

### **Daily Health Checks**

#### **✅ MCP Server Status** (Location-Agnostic)
```powershell
# Verify MCP server is running
Get-Process node | Where-Object {$_.Path -like "*mcp*"}

# Restart MCP server if needed (works from any worktree)
$RepoRoot = git rev-parse --show-toplevel
$McpPath = "$RepoRoot\mcp-servers\mssql-mcp-server" -replace '/', '\'
cd $McpPath
echo "Restarting MCP server from: $(pwd)"
npm start
```

```bash
# WSL/Bash - MCP server status (works from any worktree)
# Verify MCP server is running
ps aux | grep node | grep mcp

# Restart MCP server if needed
REPO_ROOT=$(git rev-parse --show-toplevel)
MCP_PATH="$REPO_ROOT/mcp-servers/mssql-mcp-server"
cd "$MCP_PATH"
echo "Restarting MCP server from: $(pwd)"
npm start
```

#### **✅ Database Connectivity** 
```powershell
# Quick connection test
sqlcmd -S "MINIJOE\SQLDEVELOPER2022" -U sa -P "pa`$word" -Q "SELECT GETDATE()"
```

### **Performance Monitoring**

#### **📊 Query Performance**
```sql
-- Slow query identification (if needed)
SELECT TOP 10 
    SUBSTRING(qt.text, (qs.statement_start_offset/2)+1,
        ((CASE qs.statement_end_offset
            WHEN -1 THEN DATALENGTH(qt.text)
            ELSE qs.statement_end_offset
        END - qs.statement_start_offset)/2)+1) as statement_text,
    qs.execution_count,
    qs.total_logical_reads,
    qs.total_logical_writes,
    qs.total_worker_time,
    qs.total_elapsed_time,
    qs.creation_time
FROM sys.dm_exec_query_stats qs
CROSS APPLY sys.dm_exec_sql_text(qs.sql_handle) qt
WHERE qt.text like '%OCR%'
ORDER BY qs.total_worker_time DESC
```

#### **📈 Database Growth Tracking**
```sql
-- Database file sizes and growth
SELECT 
    name as FileName,
    physical_name as FilePath,
    size * 8.0 / 1024 as SizeMB,
    max_size * 8.0 / 1024 as MaxSizeMB,
    growth as GrowthSetting,
    is_percent_growth as IsPercentGrowth
FROM sys.database_files
```

### **Backup & Recovery**

#### **💾 Database Backup Status**
```sql
-- Recent backup information
SELECT 
    database_name,
    backup_start_date,
    backup_finish_date,
    type,
    backup_size / 1024 / 1024 as backup_size_mb
FROM msdb.dbo.backupset 
WHERE database_name = 'WebSource-AutoBot'
ORDER BY backup_start_date DESC
```

---

## 🔗 Integration with Development Workflow

### **Claude Code Usage Patterns**

#### **🔍 Debugging Database Issues**
- Use MCP to query OCRCorrectionLearning for failed operations
- Analyze template structure for field mapping issues
- Check regex pattern effectiveness and coverage

#### **📊 Performance Analysis**
- Monitor OCR correction success rates
- Analyze template creation patterns
- Track learning system improvements

#### **🧪 Test Data Verification**
- Verify test data exists and is accessible
- Check template completeness for test scenarios
- Validate database state after test execution

### **Emergency Recovery Procedures**

#### **🚨 MCP Server Issues** (Location-Agnostic)
```powershell
# Stop all node processes (if needed)
Get-Process node | Stop-Process -Force

# Restart MCP server (works from any worktree)
$RepoRoot = git rev-parse --show-toplevel
$McpPath = "$RepoRoot\mcp-servers\mssql-mcp-server" -replace '/', '\'
cd $McpPath
echo "Emergency restart from: $(pwd)"
npm start
```

```bash
# WSL/Bash - Emergency MCP server recovery (works from any worktree)
# Stop all node processes (if needed)
pkill -f node

# Restart MCP server
REPO_ROOT=$(git rev-parse --show-toplevel)
MCP_PATH="$REPO_ROOT/mcp-servers/mssql-mcp-server"
cd "$MCP_PATH"
echo "Emergency restart from: $(pwd)"
npm start
```

#### **🚨 Database Connection Issues**
```powershell
# Test basic SQL Server connectivity
sqlcmd -S "MINIJOE\SQLDEVELOPER2022" -E -Q "SELECT @@SERVERNAME"

# Test with specific credentials
sqlcmd -S "MINIJOE\SQLDEVELOPER2022" -U sa -P "pa`$word" -Q "SELECT @@VERSION"
```

---

## 📖 ADDITIONAL REFERENCES

**Related Documentation**:
- **BUILD-AND-TEST.md** - Database verification commands used in testing
- **ARCHITECTURE-OVERVIEW.md** - Database schema and OCR table relationships
- **DEVELOPMENT-STANDARDS.md** - Database operation logging standards

**Configuration Files** (Relative to Repository Root):
- `./mcp-servers/mssql-mcp-server/.env` - MCP server environment configuration
- `/home/joseph/.claude/settings.json` - Claude Code MCP client configuration (global)
- `./mcp-servers/mssql-mcp-server/package.json` - MCP server dependencies and scripts

**🔍 Quick Configuration Check** (Any Worktree):
```bash
# Verify all configuration files exist
REPO_ROOT=$(git rev-parse --show-toplevel)
echo "Repository: $REPO_ROOT"
ls -la "$REPO_ROOT/mcp-servers/mssql-mcp-server/.env" && echo "✅ .env found" || echo "❌ .env missing"
ls -la "/home/joseph/.claude/settings.json" && echo "✅ Claude settings found" || echo "❌ Claude settings missing"
ls -la "$REPO_ROOT/mcp-servers/mssql-mcp-server/package.json" && echo "✅ package.json found" || echo "❌ package.json missing"
```

---

*Database & MCP Setup v1.0 | Production-Ready Configuration | Real-Time Database Access*