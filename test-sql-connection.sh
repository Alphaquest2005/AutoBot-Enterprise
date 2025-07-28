#!/bin/bash

# Test script for WSL2 to Windows SQL Server connection
echo "🔍 Testing WSL2 to Windows SQL Server Connection"
echo "================================================"

# Get Windows host IP
WINDOWS_HOST=$(grep -m 1 nameserver /etc/resolv.conf | awk '{print$2}')
echo "Windows Host IP: $WINDOWS_HOST"

# Test 1: Connection to named instance with host IP
echo "🧪 Test 1: Named instance with host IP"
sqlcmd -S "$WINDOWS_HOST\SQLDEVELOPER2022" -U sa -P "pa\$word" -Q "SELECT @@VERSION" -C -l 30

# Test 2: Connection to default instance (port 1433)  
echo "🧪 Test 2: Default port 1433"
sqlcmd -S "$WINDOWS_HOST,1433" -U sa -P "pa\$word" -Q "SELECT @@VERSION" -C -l 30

# Test 3: Try with localhost (WSL2 mirrored mode)
echo "🧪 Test 3: Localhost with named instance"
sqlcmd -S "localhost\SQLDEVELOPER2022" -U sa -P "pa\$word" -Q "SELECT @@VERSION" -C -l 30

# Test 4: Check if SQL Browser service is reachable
echo "🧪 Test 4: Test UDP 1434 (SQL Browser)"
nc -u -z -v $WINDOWS_HOST 1434 2>&1 || echo "SQL Browser port 1434 not reachable"

echo "================================================"
echo "✅ Connection tests completed"