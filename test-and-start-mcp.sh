#!/bin/bash
# Simple MCP Test and Start Script

echo "🧪 Testing SQL connection from WSL2..."

# Test connection
if sqlcmd -S "10.255.255.254\SQLDEVELOPER2022" -U sa -P 'pa$$word' -Q "SELECT @@SERVERNAME" -C -l 10 >/dev/null 2>&1; then
    echo "✅ SQL connection works!"
    
    echo "🚀 Starting MCP server..."
    cd "/mnt/c/Insight Software/AutoBot-Enterprise/mcp-servers/mssql-mcp-server"
    echo "MCP server starting on stdio transport..."
    npm start
else
    echo "❌ SQL connection failed. Run the PowerShell script first."
    echo "From Windows PowerShell as Admin:"
    echo "  & '/mnt/c/Insight Software/AutoBot-Enterprise/fix-sql-mcp.ps1'"
fi