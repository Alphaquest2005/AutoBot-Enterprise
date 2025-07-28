# 🔧 SQL Server Configuration Commands (No Configuration Manager Needed)

## **PowerShell Commands to Replace Configuration Manager**

### **1. Enable TCP/IP Protocol via Registry**

**Run in PowerShell as Administrator:**

```powershell
# Enable TCP/IP for SQLDEVELOPER2022 instance
$instanceName = "SQLDEVELOPER2022"
$regPath = "HKLM:\SOFTWARE\Microsoft\Microsoft SQL Server\MSSQL16.$instanceName\MSSQLServer\SuperSocketNetLib\Tcp"

# Enable TCP/IP protocol
Set-ItemProperty -Path $regPath -Name "Enabled" -Value 1

# Set static port 1433
Set-ItemProperty -Path "$regPath\IPAll" -Name "TcpPort" -Value "1433"
Set-ItemProperty -Path "$regPath\IPAll" -Name "TcpDynamicPorts" -Value ""

Write-Host "TCP/IP enabled and port 1433 configured for $instanceName"
```

### **2. Configure SQL Server Browser Service**

```powershell
# Set SQL Server Browser to Automatic startup
Set-Service "SQLBrowser" -StartupType Automatic

# Start the service
Start-Service "SQLBrowser"

# Verify it's running
Get-Service "SQLBrowser"
```

### **3. Enable Remote Connections via SQL Command**

**Connect to SQL Server locally first (if possible):**

```sql
-- Enable remote connections
EXEC sys.sp_configure N'remote access', N'1'
GO
RECONFIGURE WITH OVERRIDE
GO

-- Show remote DAC connections (optional)
EXEC sys.sp_configure N'remote admin connections', N'1'
GO
RECONFIGURE WITH OVERRIDE
GO
```

### **4. Alternative: Use SQL Server PowerShell Module**

**If you have SQL Server PowerShell module installed:**

```powershell
# Import SQL Server module
Import-Module SqlServer

# Enable TCP/IP
$smo = [Microsoft.SqlServer.Management.Smo.Wmi.ManagedComputer]::new()
$uri = "ManagedComputer[@Name='$env:COMPUTERNAME']/ServerInstance[@Name='SQLDEVELOPER2022']/ServerProtocol[@Name='Tcp']"
$tcp = $smo.GetSmoObject($uri)
$tcp.IsEnabled = $true
$tcp.Alter()

# Set static port
$tcpip = $smo.GetSmoObject($uri + "/IPAddress[@Name='IPAll']")
$tcpip.IPAddressProperties["TcpPort"].Value = "1433"
$tcpip.IPAddressProperties["TcpDynamicPorts"].Value = ""
$tcpip.Alter()
```

## **🔄 Simpler Approach: Manual Service Configuration**

### **1. Configure Services Only (Recommended)**

```powershell
# Stop SQL Server instance
Stop-Service "MSSQL`$SQLDEVELOPER2022" -Force

# Configure and start SQL Browser (critical for named instances)
Set-Service "SQLBrowser" -StartupType Automatic
Start-Service "SQLBrowser"

# Start SQL Server instance
Start-Service "MSSQL`$SQLDEVELOPER2022"

# Verify both services are running
Get-Service -Name "*SQL*" | Where-Object {$_.Status -eq "Running"}
```

### **2. Test Named Instance Discovery**

```cmd
# Test if SQL Browser can locate the instance
sqlcmd -L
```

This should list your `MINIJOE\SQLDEVELOPER2022` instance.

### **3. Alternative Connection Methods**

**Try connecting with specific port instead of instance name:**

```bash
# If SQLDEVELOPER2022 uses a specific port (check Event Viewer for startup port)
sqlcmd -S "10.255.255.254,1433" -U sa -P 'pa$$word' -Q "SELECT @@SERVERNAME" -C

# Or try default instance connection
sqlcmd -S "10.255.255.254" -U sa -P 'pa$$word' -Q "SELECT @@SERVERNAME" -C
```

## **🎯 Quick Test Sequence**

**Run these commands in order:**

```powershell
# 1. Configure services
Set-Service "SQLBrowser" -StartupType Automatic
Start-Service "SQLBrowser"
Restart-Service "MSSQL`$SQLDEVELOPER2022"

# 2. Test locally
sqlcmd -S "MINIJOE\SQLDEVELOPER2022" -U sa -P "pa$$word" -Q "SELECT @@SERVERNAME"

# 3. If local works, test from WSL2
```

## **🔄 If Still Not Working**

**The registry changes require a restart, so:**
1. Run the PowerShell registry commands above
2. **Restart the computer**  
3. Test the connection

**Most SQL Server network configuration changes need a reboot to take effect, especially for named instances.**