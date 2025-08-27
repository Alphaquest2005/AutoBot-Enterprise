# 🔧 SQL Server Named Instance Configuration Fix

## ⚠️ **Root Cause Identified**
The "Error Locating Server/Instance Specified" indicates that the SQL Server Browser service and TCP/IP protocol are not properly configured for remote connections to your named instance `SQLDEVELOPER2022`.

## 🛠️ **Required Configuration Steps**

### **1. SQL Server Configuration Manager (Critical)**

**Open SQL Server Configuration Manager as Administrator:**
1. Press `Win+R`, type `SQLServerManager16.msc` (or find it in Start Menu)
2. **Run as Administrator** (very important!)

**Enable TCP/IP Protocol:**
1. Expand **SQL Server Network Configuration**
2. Click **Protocols for SQLDEVELOPER2022**
3. Right-click **TCP/IP** → **Enable**
4. Right-click **TCP/IP** → **Properties**
5. Go to **IP Addresses** tab
6. Scroll to **IPAll** section at bottom:
   - Set **TCP Dynamic Ports** to **blank** (delete any value)
   - Set **TCP Port** to **1433**
7. Click **OK**

**Configure SQL Server Browser:**
1. Click **SQL Server Services** in left panel
2. Right-click **SQL Server Browser** → **Properties**
3. Set **Start Mode** to **Automatic**
4. Click **OK**
5. Right-click **SQL Server Browser** → **Start**

### **2. Enable Remote Connections in SQL Server**

**Open SQL Server Management Studio:**
1. Connect to your `MINIJOE\SQLDEVELOPER2022` instance
2. Right-click server name → **Properties**
3. Go to **Connections** page
4. Check **"Allow remote connections to this server"**
5. Click **OK**

### **3. Restart Services (Required)**

**In PowerShell as Administrator:**
```powershell
# Stop services
Stop-Service "MSSQL`$SQLDEVELOPER2022"
Stop-Service "SQLBrowser"

# Start services in correct order
Start-Service "SQLBrowser"
Start-Service "MSSQL`$SQLDEVELOPER2022"

# Verify services are running
Get-Service -Name "*SQL*" | Where-Object {$_.Status -eq "Running"}
```

### **4. Test Connection**

**From Windows Command Prompt:**
```cmd
sqlcmd -S "MINIJOE\SQLDEVELOPER2022" -U sa -P "pa$$word" -Q "SELECT @@SERVERNAME"
```

**From WSL2:**
```bash
sqlcmd -S "10.255.255.254\SQLDEVELOPER2022" -U sa -P 'pa$$word' -Q "SELECT @@SERVERNAME" -C
```

## 🔄 **Alternative: Computer Restart**

**If configuration steps don't work immediately:**
- **Yes, restart the computer** - This ensures all SQL Server services start with the new configuration
- Windows services sometimes need a full restart to apply network configuration changes
- After restart, test the connection again

## 🎯 **Expected Results After Fix**
- ✅ SQL Server Browser service running on UDP 1434
- ✅ SQL Server instance listening on TCP 1433 (static port)
- ✅ Remote connections enabled
- ✅ Connection from WSL2 successful
- ✅ MCP server can connect to database

## 📋 **If Still Failing After Restart**

**Check these in order:**
1. SQL Server Configuration Manager shows TCP/IP **Enabled**
2. SQL Server Browser service is **Running**
3. Windows Firewall has rules for ports 1433 (TCP) and 1434 (UDP)
4. SQL Server Authentication mode is **Mixed Mode**

**The restart should resolve the issue - SQL Server network configuration often requires a reboot to fully take effect.**