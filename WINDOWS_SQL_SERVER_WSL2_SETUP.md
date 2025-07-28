# 🚀 Windows SQL Server Configuration for WSL2 Access

## **Issue Identified**
All WSL2 connection tests are failing with timeout errors, indicating Windows-side configuration issues.

## **Required Windows Configuration (Run as Administrator)**

### **1. Enable SQL Server Browser Service**
```powershell
# Open PowerShell as Administrator and run:
Set-Service "SQLBrowser" -StartupType Automatic
Start-Service "SQLBrowser"
Get-Service "SQLBrowser"  # Verify it's running
```

### **2. Configure Windows Firewall**
```powershell
# Allow SQL Server (TCP 1433) and SQL Browser (UDP 1434)
New-NetFirewallRule -DisplayName "SQL Server WSL2" -Direction Inbound -Protocol TCP -LocalPort 1433 -Action Allow
New-NetFirewallRule -DisplayName "SQL Browser WSL2" -Direction Inbound -Protocol UDP -LocalPort 1434 -Action Allow

# Enable Virtual Machine Monitoring (recommended for WSL2)
Enable-NetFirewallRule -DisplayGroup "Virtual Machine Monitoring"

# Alternative: Allow sqlservr.exe directly
New-NetFirewallRule -DisplayName "SQL Server Process" -Direction Inbound -Program "C:\Program Files\Microsoft SQL Server\MSSQL16.SQLDEVELOPER2022\MSSQL\Binn\sqlservr.exe" -Action Allow
```

### **3. Enable SQL Server Remote Connections**
```powershell
# Check SQL Server services
Get-Service -Name "*SQL*" | Where-Object {$_.Status -eq "Running"}

# Restart SQL Server service after configuration changes
Restart-Service "MSSQL`$SQLDEVELOPER2022"
Restart-Service "SQLBrowser"
```

### **4. SQL Server Configuration Manager Steps**
1. Open **SQL Server Configuration Manager**
2. Navigate to **SQL Server Network Configuration** → **Protocols for SQLDEVELOPER2022**
3. **Enable TCP/IP** protocol
4. Right-click **TCP/IP** → **Properties** → **IP Addresses** tab
5. Set **TCP Dynamic Ports** to blank and **TCP Port** to **1433** (for static port)
6. **Restart SQL Server service**

### **5. SQL Server Authentication Mode**
```sql
-- Run in SQL Server Management Studio or sqlcmd on Windows:
USE [master]
GO
EXEC xp_instance_regwrite N'HKEY_LOCAL_MACHINE', 
     N'Software\Microsoft\MSSQLServer\MSSQLServer',
     N'LoginMode', REG_DWORD, 2
-- Restart SQL Server service after this change
```

## **WSL2 Testing After Windows Configuration**

### **Test Connection from WSL2:**
```bash
# Run from WSL2 terminal:
WINDOWS_HOST=$(grep -m 1 nameserver /etc/resolv.conf | awk '{print$2}')
echo "Testing connection to: $WINDOWS_HOST"

# Test with named instance
sqlcmd -S "$WINDOWS_HOST\SQLDEVELOPER2022" -U sa -P "pa\$word" -Q "SELECT @@SERVERNAME, DB_NAME()" -C

# Test with static port (if configured)
sqlcmd -S "$WINDOWS_HOST,1433" -U sa -P "pa\$word" -Q "SELECT @@SERVERNAME, DB_NAME()" -C
```

## **Alternative: WSL2 Mirrored Networking Mode**

### **For Windows 11 22H2+:**
Create/edit `C:\Users\%USERNAME%\.wslconfig`:
```ini
[wsl2]
networkingMode=mirrored
```

Restart WSL2:
```powershell
wsl --shutdown
wsl
```

Test with localhost:
```bash
sqlcmd -S "localhost\SQLDEVELOPER2022" -U sa -P "pa\$word" -Q "SELECT @@SERVERNAME" -C
```

## **Troubleshooting Commands**

### **Windows PowerShell (as Administrator):**
```powershell
# Check firewall rules
Get-NetFirewallRule -DisplayName "*SQL*" | Select-Object DisplayName, Enabled, Direction

# Check SQL Server ports
netstat -an | findstr :1433
netstat -an | findstr :1434

# Test network connectivity from Windows
Test-NetConnection -ComputerName localhost -Port 1433
```

### **WSL2 Troubleshooting:**
```bash
# Check WSL2 network connectivity
ping $(grep -m 1 nameserver /etc/resolv.conf | awk '{print$2}')

# Test specific ports (install netcat if needed)
sudo apt install netcat-openbsd
nc -zv $(grep -m 1 nameserver /etc/resolv.conf | awk '{print$2}') 1433
```

## **Expected Results After Configuration**
- SQL Server Browser service: **Running**
- Windows Firewall: **Allowing ports 1433 (TCP) and 1434 (UDP)**
- SQL Server TCP/IP: **Enabled**
- WSL2 to Windows connection: **Successful**

## **Next Steps**
1. **Complete all Windows configuration steps above**
2. **Restart all SQL Server services**
3. **Test WSL2 connection using the test commands**
4. **Configure MCP servers with working connection details**
5. **Test MCP functionality with Claude Desktop**