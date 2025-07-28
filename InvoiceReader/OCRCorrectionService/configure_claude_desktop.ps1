# =============================================================================
# Claude Desktop MCP SQL Server Configuration Script for Windows
# =============================================================================
# Run this PowerShell script as Administrator to configure Claude Desktop
# =============================================================================

# Colors for PowerShell output
$RED = "Red"
$GREEN = "Green" 
$YELLOW = "Yellow"
$BLUE = "Blue"
$CYAN = "Cyan"

function Write-ColorOutput($ForegroundColor, $Message) {
    Write-Host $Message -ForegroundColor $ForegroundColor
}

function Write-Status($Message) {
    Write-ColorOutput $GREEN "✅ $Message"
}

function Write-Warning($Message) {
    Write-ColorOutput $YELLOW "⚠️  $Message"
}

function Write-Error($Message) {
    Write-ColorOutput $RED "❌ $Message"
}

function Write-Info($Message) {
    Write-ColorOutput $BLUE "ℹ️  $Message"
}

Write-ColorOutput $BLUE "🚀 AutoBot-Enterprise Claude Desktop MCP Configuration"
Write-ColorOutput $BLUE "===================================================="

# Get current user
$USERNAME = $env:USERNAME
Write-Info "Current user: $USERNAME"

# Define Claude config path
$CLAUDE_CONFIG_DIR = "$env:APPDATA\Claude"
$CLAUDE_CONFIG_FILE = "$CLAUDE_CONFIG_DIR\claude_desktop_config.json"

Write-Info "Claude config directory: $CLAUDE_CONFIG_DIR"
Write-Info "Claude config file: $CLAUDE_CONFIG_FILE"

# Create Claude directory if it doesn't exist
if (-not (Test-Path $CLAUDE_CONFIG_DIR)) {
    New-Item -ItemType Directory -Path $CLAUDE_CONFIG_DIR -Force | Out-Null
    Write-Status "Created Claude config directory"
} else {
    Write-Status "Claude config directory exists"
}

# Backup existing config if it exists
if (Test-Path $CLAUDE_CONFIG_FILE) {
    $BACKUP_FILE = "$CLAUDE_CONFIG_FILE.backup.$(Get-Date -Format 'yyyyMMdd-HHmmss')"
    Copy-Item $CLAUDE_CONFIG_FILE $BACKUP_FILE
    Write-Warning "Existing config backed up to: $BACKUP_FILE"
}

# Get WSL username
Write-Info "Detecting WSL username..."
try {
    $WSL_USERNAME = wsl.exe whoami 2>$null
    if ($WSL_USERNAME) {
        Write-Status "WSL username detected: $WSL_USERNAME"
    } else {
        $WSL_USERNAME = "ubuntu"  # Default fallback
        Write-Warning "Could not detect WSL username, using default: $WSL_USERNAME"
    }
} catch {
    $WSL_USERNAME = "ubuntu"  # Default fallback
    Write-Warning "Could not detect WSL username, using default: $WSL_USERNAME"
}

# Test WSL connectivity
Write-Info "Testing WSL connectivity..."
try {
    $WSL_TEST = wsl.exe echo "WSL Test Successful" 2>$null
    if ($WSL_TEST -eq "WSL Test Successful") {
        Write-Status "WSL is accessible from Windows"
    } else {
        Write-Warning "WSL test returned unexpected result: $WSL_TEST"
    }
} catch {
    Write-Error "WSL is not accessible. Please ensure WSL is installed and running."
    exit 1
}

# Prompt user for connection string customization
Write-ColorOutput $CYAN "`n📝 Database Connection Configuration"
Write-ColorOutput $CYAN "======================================"

$CONNECTION_STRINGS = @{
    "1" = "Server=localhost;Database=WebSource-AutoBot-Original;Integrated Security=true;TrustServerCertificate=true;"
    "2" = "Server=localhost\SQLEXPRESS;Database=WebSource-AutoBot-Original;Integrated Security=true;TrustServerCertificate=true;"
    "3" = "Server=localhost;Database=CoreEntities;Integrated Security=true;TrustServerCertificate=true;"
    "4" = "Custom - I'll enter my own connection string"
}

Write-Host "`nSelect your database configuration:"
foreach ($key in $CONNECTION_STRINGS.Keys | Sort-Object) {
    Write-Host "$key. $($CONNECTION_STRINGS[$key])"
}

do {
    $choice = Read-Host "`nEnter your choice (1-4)"
} while ($choice -notin @("1", "2", "3", "4"))

if ($choice -eq "4") {
    $CONNECTION_STRING = Read-Host "Enter your custom connection string"
} else {
    $CONNECTION_STRING = $CONNECTION_STRINGS[$choice]
}

Write-Status "Selected connection string: $CONNECTION_STRING"

# Generate Claude Desktop configuration
$MCP_SERVER_PATH = "/home/$WSL_USERNAME/autobot-mcp-setup/SQL-AI-samples/mcp-mssql-server"

$CLAUDE_CONFIG = @{
    mcpServers = @{
        "mssql-autobot" = @{
            command = "wsl.exe"
            args = @(
                "bash",
                "-c",
                "cd $MCP_SERVER_PATH && node build/index.js"
            )
            env = @{
                CONNECTION_STRING = $CONNECTION_STRING
            }
        }
    }
} | ConvertTo-Json -Depth 10

# Write configuration file
try {
    $CLAUDE_CONFIG | Out-File -FilePath $CLAUDE_CONFIG_FILE -Encoding UTF8
    Write-Status "Claude Desktop configuration written successfully"
} catch {
    Write-Error "Failed to write Claude Desktop configuration: $_"
    exit 1
}

# Verify configuration file
if (Test-Path $CLAUDE_CONFIG_FILE) {
    $CONFIG_SIZE = (Get-Item $CLAUDE_CONFIG_FILE).Length
    Write-Status "Configuration file created ($CONFIG_SIZE bytes)"
    
    # Display the configuration
    Write-Info "`nGenerated configuration:"
    Write-ColorOutput $YELLOW "========================"
    Get-Content $CLAUDE_CONFIG_FILE | Write-Host
    Write-ColorOutput $YELLOW "========================"
} else {
    Write-Error "Configuration file was not created successfully"
    exit 1
}

# Check if Claude Desktop is running
$CLAUDE_PROCESSES = Get-Process -Name "Claude" -ErrorAction SilentlyContinue
if ($CLAUDE_PROCESSES) {
    Write-Warning "Claude Desktop is currently running ($($CLAUDE_PROCESSES.Count) process(es))"
    Write-Warning "You need to completely EXIT Claude Desktop and restart it for changes to take effect"
    
    $CLOSE_CLAUDE = Read-Host "`nWould you like to close Claude Desktop now? (y/n)"
    if ($CLOSE_CLAUDE -eq "y" -or $CLOSE_CLAUDE -eq "Y") {
        try {
            Stop-Process -Name "Claude" -Force
            Write-Status "Claude Desktop processes terminated"
            Start-Sleep -Seconds 2
        } catch {
            Write-Warning "Could not automatically close Claude Desktop. Please close it manually."
        }
    }
} else {
    Write-Status "Claude Desktop is not currently running"
}

# Test MCP server availability in WSL
Write-Info "`nTesting MCP server availability in WSL..."
$MCP_TEST_COMMAND = "test -f $MCP_SERVER_PATH/build/index.js && echo 'MCP Server Found' || echo 'MCP Server Not Found'"
try {
    $MCP_TEST_RESULT = wsl.exe bash -c $MCP_TEST_COMMAND 2>$null
    if ($MCP_TEST_RESULT -eq "MCP Server Found") {
        Write-Status "MCP server is available in WSL"
    } else {
        Write-Warning "MCP server not found. Please run the WSL setup script first:"
        Write-Host "  wsl.exe bash /mnt/c/'Insight Software'/AutoBot-Enterprise/InvoiceReader/OCRCorrectionService/setup_mcp_sql_server.sh"
    }
} catch {
    Write-Warning "Could not test MCP server availability in WSL"
}

# Final instructions
Write-ColorOutput $GREEN "`n🎉 Claude Desktop Configuration Complete!"
Write-ColorOutput $BLUE "============================================="

Write-ColorOutput $YELLOW "`n📋 NEXT STEPS:"
Write-Host "1. " -NoNewline
Write-ColorOutput $GREEN "Start Claude Desktop"

Write-Host "2. " -NoNewline  
Write-ColorOutput $GREEN "Look for MCP server connection indicator in Claude"

Write-Host "3. " -NoNewline
Write-ColorOutput $GREEN "Test the connection by asking Claude:"
Write-ColorOutput $YELLOW '   "Can you show me the tables in the database?"'

Write-Host "4. " -NoNewline
Write-ColorOutput $GREEN "If connection fails, check the WSL setup and run:"
Write-ColorOutput $YELLOW "   wsl.exe bash /mnt/c/'Insight Software'/AutoBot-Enterprise/InvoiceReader/OCRCorrectionService/setup_mcp_sql_server.sh"

Write-ColorOutput $BLUE "`n🔗 Key Files:"
Write-Host "• Claude Config: " -NoNewline
Write-ColorOutput $YELLOW $CLAUDE_CONFIG_FILE
Write-Host "• MCP Server Path: " -NoNewline  
Write-ColorOutput $YELLOW $MCP_SERVER_PATH
Write-Host "• Connection String: " -NoNewline
Write-ColorOutput $YELLOW $CONNECTION_STRING

Write-ColorOutput $BLUE "`n💡 Troubleshooting:"
Write-Host "• If Claude doesn't show MCP connection, restart Claude Desktop completely"
Write-Host "• Check WSL is running: " -NoNewline
Write-ColorOutput $YELLOW "wsl.exe --status" 
Write-Host "• Test SQL connection: " -NoNewline
Write-ColorOutput $YELLOW "wsl.exe sqlcmd -S localhost -E -Q `"SELECT @@VERSION`""
Write-Host "• View detailed setup guide: " -NoNewline
Write-ColorOutput $YELLOW "MCP_SQL_SERVER_SETUP_GUIDE.md"

Write-ColorOutput $GREEN "`n✨ Happy coding with AutoBot-Enterprise MCP integration!"

# Pause to allow user to read the output
Write-Host "`nPress any key to continue..." -NoNewline
$null = $Host.UI.RawUI.ReadKey("NoEcho,IncludeKeyDown")