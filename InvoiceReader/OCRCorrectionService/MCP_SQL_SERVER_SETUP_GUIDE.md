# 🚀 COMPREHENSIVE MCP SQL SERVER SETUP GUIDE FOR WSL ENVIRONMENT
**Model Context Protocol Integration with AutoBot-Enterprise SQL Server Databases**

## 📋 **OVERVIEW**

This guide sets up the Model Context Protocol (MCP) SQL Server to enable direct database access from Claude Code/Desktop in your WSL environment. This will allow you to execute the DATABASE_INTEGRATION_SCRIPTS.sql and perform database operations directly through Claude.

## 🎯 **ARCHITECTURE OVERVIEW**

```
Claude Desktop/Code → MCP SQL Server → WSL → SQL Server Database
                                     ↓
                              AutoBot-Enterprise
                              ├── CoreEntities DB
                              ├── WebSource-AutoBot-Original DB
                              └── OCR_TemplateTableMapping
```

## 📦 **OPTION 1: Microsoft Official MSSQL MCP Server (RECOMMENDED)**

### **Prerequisites**
```bash
# Verify WSL and required tools
wsl --version
node --version  # Should be 18+ 
npm --version
git --version
```

### **Step 1: Clone Microsoft SQL AI Samples**
```bash
# In WSL terminal
cd /home/$USER
git clone https://github.com/Azure-Samples/SQL-AI-samples.git
cd SQL-AI-samples/mcp-mssql-server
```

### **Step 2: Install Dependencies and Build**
```bash
npm install
npm run build
```

### **Step 3: Configure Connection String**
Create a `.env` file in the mcp-mssql-server directory:
```bash
# .env file content
CONNECTION_STRING="Server=localhost;Database=WebSource-AutoBot-Original;Integrated Security=true;TrustServerCertificate=true;"
# Alternative with SQL Auth:
# CONNECTION_STRING="Server=localhost;Database=WebSource-AutoBot-Original;User Id=your_user;Password=your_password;TrustServerCertificate=true;"
```

### **Step 4: Test MCP Server**
```bash
# Test the server can connect
node build/index.js
```

## 📦 **OPTION 2: Community MSSQL MCP Server (ALTERNATIVE)**

### **Step 1: Clone Community Implementation**
```bash
cd /home/$USER
git clone https://github.com/RichardHan/mssql_mcp_server.git
cd mssql_mcp_server
```

### **Step 2: Install and Configure**
```bash
npm install
# Create config file
cp config.example.json config.json
```

Edit `config.json`:
```json
{
  "server": "localhost",
  "database": "WebSource-AutoBot-Original",
  "options": {
    "encrypt": false,
    "trustServerCertificate": true,
    "enableArithAbort": true
  },
  "authentication": {
    "type": "default"
  }
}
```

## 🔧 **CLAUDE DESKTOP CONFIGURATION**

### **Step 1: Locate Configuration File**
- **Windows Path**: `C:\Users\%USERNAME%\AppData\Roaming\Claude\claude_desktop_config.json`
- **Quick Access**: Press `Win+R`, type `%appdata%\Claude`, press Enter

### **Step 2: Create/Edit Configuration**
Create or edit `claude_desktop_config.json`:

#### **For Microsoft Official MCP Server:**
```json
{
  "mcpServers": {
    "mssql-autobot": {
      "command": "wsl.exe",
      "args": [
        "bash",
        "-c",
        "cd /home/$USER/SQL-AI-samples/mcp-mssql-server && node build/index.js"
      ],
      "env": {
        "CONNECTION_STRING": "Server=localhost;Database=WebSource-AutoBot-Original;Integrated Security=true;TrustServerCertificate=true;"
      }
    }
  }
}
```

#### **For Community MCP Server:**
```json
{
  "mcpServers": {
    "mssql-autobot": {
      "command": "wsl.exe",
      "args": [
        "bash", 
        "-c",
        "cd /home/$USER/mssql_mcp_server && node index.js"
      ]
    }
  }
}
```

#### **WSL Environment Variable Alternative (If env doesn't work):**
```json
{
  "mcpServers": {
    "mssql-autobot": {
      "command": "wsl.exe",
      "args": [
        "bash",
        "-c", 
        "CONNECTION_STRING='Server=localhost;Database=WebSource-AutoBot-Original;Integrated Security=true;TrustServerCertificate=true;' node /home/$USER/SQL-AI-samples/mcp-mssql-server/build/index.js"
      ]
    }
  }
}
```

### **Step 3: Restart Claude Desktop**
- **Exit** Claude Desktop completely (not just close)
- **Restart** Claude Desktop
- **Verify** MCP connection in Claude interface

## 🔍 **MULTI-DATABASE CONFIGURATION (OPTIONAL)**

For accessing multiple databases in your AutoBot-Enterprise environment:

```json
{
  "mcpServers": {
    "mssql-autobot-original": {
      "command": "wsl.exe",
      "args": [
        "bash", "-c",
        "CONNECTION_STRING='Server=localhost;Database=WebSource-AutoBot-Original;Integrated Security=true;TrustServerCertificate=true;' node /home/$USER/SQL-AI-samples/mcp-mssql-server/build/index.js"
      ]
    },
    "mssql-autobot-coreentities": {
      "command": "wsl.exe", 
      "args": [
        "bash", "-c",
        "CONNECTION_STRING='Server=localhost;Database=CoreEntities;Integrated Security=true;TrustServerCertificate=true;' node /home/$USER/SQL-AI-samples/mcp-mssql-server/build/index.js"
      ]
    }
  }
}
```

## 🧪 **TESTING THE CONNECTION**

### **Step 1: Verify MCP Server Status**
In Claude Desktop/Code, look for MCP server indicators in the interface.

### **Step 2: Test Database Query**
Try asking Claude:
```
"Can you show me the tables in the WebSource-AutoBot-Original database?"
```

### **Step 3: Test AutoBot-Enterprise Integration**
```
"Can you check if the OCR_TemplateTableMapping table exists in the database?"
```

## 🛠️ **TROUBLESHOOTING**

### **Common Issues & Solutions**

#### **Connection String Issues**
```bash
# Test connection directly in WSL
sqlcmd -S localhost -d WebSource-AutoBot-Original -E
# If successful, your connection string should work
```

#### **Node Path Issues**
```bash
# Find exact node path
which node
# Use full path in configuration: /home/$USER/.nvm/versions/node/v20.x.x/bin/node
```

#### **WSL Communication Issues**
```bash
# Ensure WSL can access Windows SQL Server
# Check SQL Server is listening on TCP/IP
# Verify SQL Server Browser service is running
```

#### **Authentication Issues**
- **Windows Authentication**: Ensure SQL Server accepts Windows Authentication
- **SQL Authentication**: Use specific username/password in connection string
- **Mixed Mode**: Configure SQL Server for mixed mode authentication

### **Debug Commands**
```bash
# Check WSL network connectivity
ping localhost

# Test SQL Server connection from WSL
sqlcmd -S localhost -E -Q "SELECT @@VERSION"

# Test MCP server manually
cd /path/to/mcp/server
node build/index.js
```

## 📋 **CONFIGURATION TEMPLATES**

### **Development Environment**
```json
{
  "mcpServers": {
    "mssql-dev": {
      "command": "wsl.exe",
      "args": ["bash", "-c", "CONNECTION_STRING='Server=localhost;Database=WebSource-AutoBot-Original;Integrated Security=true;TrustServerCertificate=true;' node /home/$USER/SQL-AI-samples/mcp-mssql-server/build/index.js"]
    }
  }
}
```

### **Production Environment** 
```json
{
  "mcpServers": {
    "mssql-prod": {
      "command": "wsl.exe",
      "args": ["bash", "-c", "CONNECTION_STRING='Server=production_server;Database=AutoBot_Production;User Id=app_user;Password=secure_password;TrustServerCertificate=true;' node /home/$USER/SQL-AI-samples/mcp-mssql-server/build/index.js"]
    }
  }
}
```

## 🎯 **IMMEDIATE NEXT STEPS**

1. **Choose Option**: Select Microsoft Official (recommended) or Community implementation
2. **Clone Repository**: Download and build the MCP server
3. **Configure Connection**: Set up appropriate connection string for your environment
4. **Update Claude Config**: Add MCP server configuration to Claude Desktop
5. **Test Connection**: Verify database access through Claude
6. **Execute Database Scripts**: Run the DATABASE_INTEGRATION_SCRIPTS.sql through Claude

## 🔒 **SECURITY CONSIDERATIONS**

- ✅ Use **Integrated Security** when possible
- ✅ **Encrypt connection strings** in production
- ✅ **Limit database permissions** for MCP connections
- ✅ **Use read-only connections** when possible for safety
- ✅ **Test in development** environment first

## 📚 **ADDITIONAL RESOURCES**

- [Model Context Protocol Documentation](https://modelcontextprotocol.io/)
- [Microsoft MCP Server Documentation](https://github.com/Azure-Samples/SQL-AI-samples)
- [Claude Desktop MCP Configuration](https://docs.anthropic.com/en/docs/claude-code/mcp)
- [WSL SQL Server Setup](https://learn.microsoft.com/en-us/sql/linux/quickstart-install-connect-wsl-2)

---

**🎉 Once configured, you'll be able to:**
- ✅ Execute database queries directly through Claude
- ✅ Run the DATABASE_INTEGRATION_SCRIPTS.sql without manual SQL management
- ✅ Perform real-time database operations for AutoBot-Enterprise
- ✅ Test and validate the OCR_TemplateTableMapping system directly
- ✅ Analyze template specification data through natural language queries

This setup transforms Claude into a powerful database administration and development tool for your AutoBot-Enterprise project!