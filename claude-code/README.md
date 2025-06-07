# Claude Code Integration Setup

This directory contains the configuration files needed to integrate Claude Code with your AutoBot-Enterprise project.

## Overview

Claude Code is Anthropic's development tool integration that allows Claude to interact with your codebase through the Model Context Protocol (MCP). This setup provides Claude with access to your project files, git repository, search capabilities, and database operations.

## Files in this Directory

- `claude_desktop_config.json` - Main configuration file for Claude Desktop
- `README.md` - This setup guide

## Installation Steps

### 1. Install Claude Desktop

Download and install Claude Desktop from [Anthropic's website](https://claude.ai/download).

### 2. Configure Claude Desktop

Copy the `claude_desktop_config.json` file to your Claude Desktop configuration directory:

**Windows:**
```
%APPDATA%\Claude\claude_desktop_config.json
```

**macOS:**
```
~/Library/Application Support/Claude/claude_desktop_config.json
```

**Linux:**
```
~/.config/Claude/claude_desktop_config.json
```

### 3. Configure API Keys (Optional)

If you want to use the Brave Search integration, add your Brave API key to the configuration:

1. Get a Brave Search API key from [Brave Search API](https://api.search.brave.com/)
2. Edit the `claude_desktop_config.json` file
3. Replace the empty `BRAVE_API_KEY` value with your actual API key

### 4. Restart Claude Desktop

After copying the configuration file, restart Claude Desktop for the changes to take effect.

## Available MCP Servers

The configuration includes the following MCP servers:

### 1. Filesystem Server
- **Purpose:** Provides Claude with read/write access to your project files
- **Scope:** `c:/Insight Software/AutoBot-Enterprise`
- **Capabilities:** File reading, writing, directory listing, file operations

### 2. Git Server
- **Purpose:** Enables Claude to interact with your Git repository
- **Scope:** `c:/Insight Software/AutoBot-Enterprise`
- **Capabilities:** Git status, commit history, branch operations, diff viewing

### 3. Brave Search Server (Optional)
- **Purpose:** Allows Claude to search the web for information
- **Requirements:** Brave API key (optional)
- **Capabilities:** Web search, real-time information lookup

### 4. SQLite Server
- **Purpose:** Provides database access capabilities
- **Database:** `c:/Insight Software/AutoBot-Enterprise/data/database.db`
- **Capabilities:** SQL queries, database schema inspection

## Usage

Once configured, Claude Desktop will have access to:

1. **Your entire project codebase** - Claude can read, analyze, and modify files
2. **Git operations** - View commit history, check status, create commits
3. **Web search** - Look up documentation, examples, and solutions
4. **Database operations** - Query and analyze your project's data

## Global Shortcut

The configuration sets `CommandOrControl+Shift+Space` as the global shortcut to quickly access Claude Desktop.

## Troubleshooting

### Claude Desktop doesn't recognize the configuration
- Ensure the file is in the correct location for your operating system
- Verify the JSON syntax is valid
- Restart Claude Desktop after making changes

### MCP servers fail to start
- Check that Node.js is installed and accessible
- Verify the project path exists: `c:/Insight Software/AutoBot-Enterprise`
- Ensure you have the necessary permissions to access the project directory

### Brave Search not working
- Verify you have a valid Brave API key
- Check that the API key is correctly set in the configuration
- Ensure you have an active internet connection

## Security Considerations

- Claude will have read/write access to your entire project directory
- Be cautious when asking Claude to make file modifications
- Review any changes Claude suggests before applying them
- The Git integration allows Claude to view your commit history and repository state

## Integration with Existing Tools

This Claude Code setup is separate from your existing Roo Code configuration (located in `.roo/`). Both can coexist and provide different AI development experiences:

- **Roo Code** - Integrated development environment with specialized modes
- **Claude Code** - Direct Claude Desktop integration with MCP servers

## Next Steps

1. Copy the configuration file to the appropriate location
2. Restart Claude Desktop
3. Test the integration by asking Claude about your project
4. Optionally configure the Brave API key for web search capabilities

For more information about Claude Code and MCP, visit the [official documentation](https://docs.anthropic.com/en/docs/claude-code/overview).