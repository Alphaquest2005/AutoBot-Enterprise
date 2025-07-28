# 🔧 Final Steps to Complete MCP SQL Server Setup

## ✅ **Completed Successfully**
- Windows Firewall rules configured for WSL2 access
- SQL Browser service enabled and started
- MCP servers fully installed and configured

## ⚠️ **Still Need: Restart SQL Server Services**

The connection is still failing because SQL Server services need to be restarted after the configuration changes.

**Run this in PowerShell as Administrator:**

```powershell
# Restart SQL Server services to apply configuration
Restart-Service "MSSQL`$SQLDEVELOPER2022"
Restart-Service "SQLBrowser"

# Verify services are running
Get-Service -Name "*SQL*" | Where-Object {$_.Status -eq "Running"}
```

## 🧪 **Test Command After Restart**

```bash
# From WSL2 terminal:
sqlcmd -S "10.255.255.254\SQLDEVELOPER2022" -U sa -P 'pa$$word' -Q "SELECT @@SERVERNAME, DB_NAME()" -C
```

## 🚀 **Start MCP Server Once Connection Works**

```bash
cd "/mnt/c/Insight Software/AutoBot-Enterprise/mcp-servers/mssql-mcp-server"
npm start
```

## 📋 **Final Claude Desktop Configuration**

Add to `C:\Users\%USERNAME%\AppData\Roaming\Claude\claude_desktop_config.json`:

```json
{
  "mcpServers": {
    "mssql-autobot": {
      "command": "wsl.exe",
      "args": [
        "bash", "-c", 
        "cd '/mnt/c/Insight Software/AutoBot-Enterprise/mcp-servers/mssql-mcp-server' && npm start"
      ]
    }
  }
}
```

## 🎯 **Expected Result**
- WSL2 can connect to SQL Server ✅
- MCP server starts without errors ✅  
- Claude Desktop can query your AutoBot-Enterprise database ✅

**You're one service restart away from full MCP functionality!**