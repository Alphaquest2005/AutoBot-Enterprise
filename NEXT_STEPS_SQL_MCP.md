# 🔧 Final Steps to Complete MCP SQL Server Setup

## 🏠 WORKTREE ENVIRONMENT DETECTION

### **🎯 Current Environment Commands**
```bash
# Always run this first to identify your current environment
echo "Current Environment: $(pwd)"
echo "Current Branch: $(git branch --show-current)"
echo "Repository Root: $(git rev-parse --show-toplevel)"
git worktree list
```

**Available Environments**:
- **Main Repository**: `AutoBot-Enterprise` (primary development)
- **Alpha Worktree**: `AutoBot-Enterprise-alpha` (experimental work)
- **Beta Worktree**: `AutoBot-Enterprise-beta` (baseline comparison)

---

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
# Location-agnostic command (works from any worktree)
REPO_ROOT=$(git rev-parse --show-toplevel)
cd "$REPO_ROOT/mcp-servers/mssql-mcp-server"
echo "Starting MCP server from: $(pwd)"
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
        "cd '$(wsl.exe bash -c 'git rev-parse --show-toplevel')/mcp-servers/mssql-mcp-server' && npm start"
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