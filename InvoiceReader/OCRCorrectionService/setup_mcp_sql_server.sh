#!/bin/bash

# =============================================================================
# MCP SQL Server Setup Script for AutoBot-Enterprise WSL Environment
# =============================================================================
# This script automates the setup of Microsoft MCP SQL Server in WSL
# =============================================================================

set -e  # Exit on any error

# Colors for output
RED='\033[0;31m'
GREEN='\033[0;32m'
YELLOW='\033[1;33m'
BLUE='\033[0;34m'
NC='\033[0m' # No Color

echo -e "${BLUE}🚀 AutoBot-Enterprise MCP SQL Server Setup${NC}"
echo -e "${BLUE}=============================================${NC}"

# Function to print colored output
print_status() {
    echo -e "${GREEN}✅ $1${NC}"
}

print_warning() {
    echo -e "${YELLOW}⚠️  $1${NC}"
}

print_error() {
    echo -e "${RED}❌ $1${NC}"
}

print_info() {
    echo -e "${BLUE}ℹ️  $1${NC}"
}

# Check prerequisites
echo -e "\n${BLUE}📋 Checking Prerequisites...${NC}"

# Check if we're in WSL
if ! grep -q Microsoft /proc/version; then
    print_error "This script must be run in WSL environment"
    exit 1
fi
print_status "WSL environment detected"

# Check Node.js
if ! command -v node &> /dev/null; then
    print_error "Node.js is not installed. Please install Node.js 18+ first."
    exit 1
fi

NODE_VERSION=$(node --version | cut -d'v' -f2 | cut -d'.' -f1)
if [ "$NODE_VERSION" -lt 18 ]; then
    print_error "Node.js version 18+ is required. Current version: $(node --version)"
    exit 1
fi
print_status "Node.js $(node --version) is installed"

# Check npm
if ! command -v npm &> /dev/null; then
    print_error "npm is not installed"
    exit 1
fi
print_status "npm $(npm --version) is installed"

# Check git
if ! command -v git &> /dev/null; then
    print_error "git is not installed"
    exit 1
fi
print_status "git is installed"

# Setup directory
SETUP_DIR="$HOME/autobot-mcp-setup"
print_info "Setting up in directory: $SETUP_DIR"

# Create setup directory
mkdir -p "$SETUP_DIR"
cd "$SETUP_DIR"

# Clone Microsoft SQL AI Samples
echo -e "\n${BLUE}📦 Downloading Microsoft MCP SQL Server...${NC}"
if [ -d "SQL-AI-samples" ]; then
    print_warning "SQL-AI-samples directory already exists, updating..."
    cd SQL-AI-samples
    git pull
    cd ..
else
    git clone https://github.com/Azure-Samples/SQL-AI-samples.git
    print_status "Microsoft SQL AI Samples cloned"
fi

# Navigate to MCP server directory
MCP_SERVER_DIR="$SETUP_DIR/SQL-AI-samples/mcp-mssql-server"
if [ ! -d "$MCP_SERVER_DIR" ]; then
    print_error "MCP server directory not found. Repository structure may have changed."
    print_info "Looking for alternative paths..."
    find "$SETUP_DIR/SQL-AI-samples" -name "*mcp*" -type d
    exit 1
fi

cd "$MCP_SERVER_DIR"
print_status "Found MCP server directory: $MCP_SERVER_DIR"

# Install dependencies
echo -e "\n${BLUE}📦 Installing Dependencies...${NC}"
npm install
print_status "Dependencies installed"

# Build the project
echo -e "\n${BLUE}🔨 Building MCP Server...${NC}"
if npm run build; then
    print_status "MCP server built successfully"
else
    print_warning "Build command failed, checking if build directory exists..."
    if [ ! -d "build" ] && [ ! -f "index.js" ]; then
        print_error "Neither build directory nor index.js found. Build failed."
        exit 1
    fi
fi

# Create environment file template
echo -e "\n${BLUE}⚙️  Creating Configuration Template...${NC}"
ENV_FILE="$MCP_SERVER_DIR/.env"
cat > "$ENV_FILE" << 'EOF'
# AutoBot-Enterprise MCP SQL Server Configuration
# Modify these connection strings according to your environment

# Primary database connection (adjust as needed)
CONNECTION_STRING=Server=localhost;Database=WebSource-AutoBot-Original;Integrated Security=true;TrustServerCertificate=true;

# Alternative SQL Authentication (uncomment and modify if needed)
# CONNECTION_STRING=Server=localhost;Database=WebSource-AutoBot-Original;User Id=your_user;Password=your_password;TrustServerCertificate=true;

# For specific server instance (uncomment and modify if needed)  
# CONNECTION_STRING=Server=localhost\SQLEXPRESS;Database=WebSource-AutoBot-Original;Integrated Security=true;TrustServerCertificate=true;
EOF

print_status "Environment configuration created: $ENV_FILE"

# Test connection (optional)
echo -e "\n${BLUE}🧪 Testing SQL Server Connection...${NC}"
if command -v sqlcmd &> /dev/null; then
    if sqlcmd -S localhost -E -Q "SELECT @@VERSION" &> /dev/null; then
        print_status "SQL Server connection test successful"
    else
        print_warning "SQL Server connection test failed. Please verify SQL Server is running and accessible."
        print_info "You may need to configure SQL Server to accept connections or modify the connection string."
    fi
else
    print_warning "sqlcmd not available, skipping connection test"
fi

# Create Claude Desktop configuration
echo -e "\n${BLUE}📝 Generating Claude Desktop Configuration...${NC}"
CLAUDE_CONFIG_TEMPLATE="$SETUP_DIR/claude_desktop_config.json"

cat > "$CLAUDE_CONFIG_TEMPLATE" << EOF
{
  "mcpServers": {
    "mssql-autobot": {
      "command": "wsl.exe",
      "args": [
        "bash",
        "-c",
        "cd $MCP_SERVER_DIR && node build/index.js"
      ],
      "env": {
        "CONNECTION_STRING": "Server=localhost;Database=WebSource-AutoBot-Original;Integrated Security=true;TrustServerCertificate=true;"
      }
    }
  }
}
EOF

print_status "Claude Desktop configuration generated: $CLAUDE_CONFIG_TEMPLATE"

# Create startup script
echo -e "\n${BLUE}📜 Creating Startup Script...${NC}"
STARTUP_SCRIPT="$SETUP_DIR/start_mcp_server.sh"
cat > "$STARTUP_SCRIPT" << EOF
#!/bin/bash
# AutoBot-Enterprise MCP SQL Server Startup Script
cd "$MCP_SERVER_DIR"
echo "Starting MCP SQL Server..."
echo "Server directory: $MCP_SERVER_DIR"
echo "Connection string from .env file:"
cat .env | grep CONNECTION_STRING
echo ""
node build/index.js
EOF

chmod +x "$STARTUP_SCRIPT"
print_status "Startup script created: $STARTUP_SCRIPT"

# Display final instructions
echo -e "\n${GREEN}🎉 MCP SQL Server Setup Complete!${NC}"
echo -e "${BLUE}=============================================${NC}"
echo -e "\n${YELLOW}📋 NEXT STEPS:${NC}"
echo -e "1. ${BLUE}Review and modify the connection string:${NC}"
echo -e "   ${YELLOW}$ENV_FILE${NC}"
echo -e ""
echo -e "2. ${BLUE}Copy the Claude Desktop configuration:${NC}"
echo -e "   ${YELLOW}From:${NC} $CLAUDE_CONFIG_TEMPLATE"
echo -e "   ${YELLOW}To:${NC}   C:\\Users\\%USERNAME%\\AppData\\Roaming\\Claude\\claude_desktop_config.json"
echo -e ""
echo -e "3. ${BLUE}Test the MCP server manually:${NC}"
echo -e "   ${YELLOW}$STARTUP_SCRIPT${NC}"
echo -e ""
echo -e "4. ${BLUE}Restart Claude Desktop completely${NC} (exit, don't just close)"
echo -e ""
echo -e "5. ${BLUE}Test the connection in Claude${NC} by asking:"
echo -e "   ${YELLOW}\"Can you show me the tables in the WebSource-AutoBot-Original database?\"${NC}"

echo -e "\n${GREEN}🔗 Key Paths:${NC}"
echo -e "• MCP Server: $MCP_SERVER_DIR"
echo -e "• Configuration: $ENV_FILE" 
echo -e "• Startup Script: $STARTUP_SCRIPT"
echo -e "• Claude Config Template: $CLAUDE_CONFIG_TEMPLATE"

echo -e "\n${BLUE}💡 For troubleshooting, see:${NC}"
echo -e "   ${YELLOW}MCP_SQL_SERVER_SETUP_GUIDE.md${NC}"

echo -e "\n${GREEN}✨ Happy coding with AutoBot-Enterprise MCP integration!${NC}"