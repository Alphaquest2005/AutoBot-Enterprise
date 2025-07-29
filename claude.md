# CLAUDE.md - SuperClaude Configuration + Development Notes

## Development Notes
- always use the logfile the console logs will truncate
- when debugging after code change double check if your changes created the problem
- run the test and make sure to check the current log file, not the old one.

## 🗄️ MCP SQL Server Setup (AutoBot-Enterprise Database Access)

### **Quick Start (Working Configuration)**
```powershell
# 1. Start MCP Server (Windows PowerShell)
cd "C:\Insight Software\AutoBot-Enterprise\mcp-servers\mssql-mcp-server"
npm start
```

### **Configuration Details**
- **Database**: `MINIJOE\SQLDEVELOPER2022` / `WebSource-AutoBot`
- **Credentials**: `sa` / `pa$word` (literal password with single $)
- **MCP Location**: `C:\Insight Software\AutoBot-Enterprise\mcp-servers\mssql-mcp-server\`
- **Claude Settings**: Already configured in `/home/joseph/.claude/settings.json`

### **Key Issues Resolved**
- **Password Escaping**: Use `pa$$word` in .env file (Node.js dotenv interprets as `pa$word`)
- **Network**: Run MCP server on Windows (bypasses WSL2 networking complexity)
- **Transport**: Uses stdio transport for Claude Code integration

### **Troubleshooting**
```powershell
# Test SQL connection
sqlcmd -S "MINIJOE\SQLDEVELOPER2022" -U sa -P "pa`$word" -Q "SELECT @@SERVERNAME"

# Verify .env file
Get-Content "C:\Insight Software\AutoBot-Enterprise\mcp-servers\mssql-mcp-server\.env" | Select-String "DB_"

# Expected .env content:
# DB_PASSWORD=pa$$word  (double $ for Node.js)
# DB_SERVER=MINIJOE\SQLDEVELOPER2022
# DB_DATABASE=WebSource-AutoBot
```

### **Usage**
Once MCP server is running, use Claude Code queries like:
- "Show me tables in WebSource-AutoBot database"
- "Query the OCR_TemplateTableMapping table"
- "Execute SELECT * FROM [table_name] LIMIT 10"

## Patience and Methodology
- Always complete builds fully - even if it takes 20 minutes
- Let tests run to completion - even if it takes 5+ minutes
- Trust the process - don't interrupt critical validation steps

## 🔄 Folder Synchronization System

### **Dual-Location Architecture**
Claude operates from **two synchronized .claude folders**:
- **Global WSL**: `/home/joseph/.claude` (cross-project configuration)
- **Project-Specific**: `/mnt/c/Insight Software/AutoBot-Enterprise/.claude` (project context)

**🎯 Result**: Identical Claude functionality regardless of access location

### **Essential Sync Commands**

#### **Primary Operations**
```bash
# Full bidirectional sync (recommended)
./claude-folder-sync.sh

# Preview changes without applying
./claude-folder-sync.sh --dry-run

# Sync specific direction
./claude-folder-sync.sh global-to-project   # Global → Project
./claude-folder-sync.sh project-to-global   # Project → Global

# Skip backup creation (faster)
./claude-folder-sync.sh --no-backup
```

#### **Auto-Sync Setup**
```bash
# Setup automatic 15-minute synchronization
./auto-sync-setup.sh

# Manual cron management
crontab -e  # Add/remove auto-sync
crontab -l  # List current jobs
```

### **Integration with Workflow**

#### **Session Start Protocol** 
1. **Verify sync status**: Check if folders are synchronized
2. **Run sync if needed**: `./claude-folder-sync.sh --dry-run` → `./claude-folder-sync.sh`
3. **Continue with normal workflow**: Load master context, activate SuperClaude

#### **Before Major Changes**
```bash
# Create safety backup
./claude-folder-sync.sh --backup bidirectional

# Work on changes...

# Synchronize after completion
./claude-folder-sync.sh
```

#### **Command Integration**
- **All existing commands work identically** from both locations
- **Sync logs available**: `/home/joseph/.claude/sync.log`
- **Backup recovery**: Timestamped backups in `/home/joseph/.claude-sync-backups/`

### **Maintenance & Troubleshooting**

#### **Health Checks**
```bash
# Verify both folders exist and accessible
ls -la "/home/joseph/.claude" "/mnt/c/Insight Software/AutoBot-Enterprise/.claude"

# Check sync history
tail -20 /home/joseph/.claude/sync.log

# Test sync functionality  
./claude-folder-sync.sh --dry-run
```

#### **Common Issues & Solutions**

**🔧 Folders Out of Sync**
```bash
# Force full sync
./claude-folder-sync.sh bidirectional
```

**🔧 Sync Script Permissions**
```bash
chmod +x /home/joseph/.claude/claude-folder-sync.sh
chmod +x "/mnt/c/Insight Software/AutoBot-Enterprise/.claude/claude-folder-sync.sh"
```

**🔧 Auto-Sync Not Working**
```bash
# Check cron job
crontab -l | grep claude-folder-sync

# Re-setup auto-sync
./auto-sync-setup.sh
```

**🔧 Emergency Recovery**
```bash
# List available backups
ls -la /home/joseph/.claude-sync-backups/

# Restore from backup (replace TIMESTAMP)
rm -rf /home/joseph/.claude
cp -r /home/joseph/.claude-sync-backups/global_TIMESTAMP /home/joseph/.claude
```

### **Advanced Usage**

#### **Selective Operations**
- **Documentation changes**: Often auto-sync via cron is sufficient
- **Major configuration changes**: Manual sync with backup recommended
- **Development workflow**: Sync at session start/end for safety

#### **Performance Optimization**
- **Auto-sync frequency**: 15 minutes (configurable in cron)
- **Manual sync timing**: 5-30 seconds depending on changes
- **Backup strategy**: Auto-backups for manual sync, no backup for auto-sync

#### **Security Considerations**
- Sync operations are **local filesystem only** (no network)
- Backup directories contain **sensitive configuration data**
- **Proper permissions** maintained on sync scripts

### **Quick Reference Cards**

#### **🚀 Daily Usage**
```bash
./claude-folder-sync.sh                    # Full sync
./claude-folder-sync.sh --dry-run          # Preview
tail -f /home/joseph/.claude/sync.log      # Monitor
```

#### **🛠️ Setup Commands**  
```bash
./auto-sync-setup.sh                       # Enable auto-sync
chmod +x claude-folder-sync.sh             # Fix permissions
./claude-folder-sync.sh --help             # Show options
```

#### **🔍 Diagnostics**
```bash
ls -la ~/.claude*/CLAUDE.md                # Verify files exist
diff ~/.claude/CLAUDE.md "/mnt/c/Insight Software/AutoBot-Enterprise/.claude/CLAUDE.md"  # Check differences
crontab -l                                  # Check auto-sync
```

### **System Status**
**✅ Status**: Fully operational - Both folders synchronized and identical  
**🔄 Sync Method**: Bidirectional merge with intelligent conflict resolution  
**💾 Backup System**: Automatic timestamped backups before manual sync  
**⚡ Auto-Sync**: Optional 15-minute automated synchronization available  

**📖 Full Documentation**: `CLAUDE_FOLDER_SYNC_DOCUMENTATION.md`

---

# SuperClaude Configuration

You are SuperClaude, an enhanced version of Claude optimized for maximum efficiency and capability.
You should use the following configuration to guide your behavior.

## Legend
@include commands/shared/universal-constants.yml#Universal_Legend

## Core Configuration
@include shared/superclaude-core.yml#Core_Philosophy

## Thinking Modes
@include commands/shared/flag-inheritance.yml#Universal Flags (All Commands)

## Introspection Mode
@include commands/shared/introspection-patterns.yml#Introspection_Mode
@include shared/superclaude-rules.yml#Introspection_Standards

## Advanced Token Economy
@include shared/superclaude-core.yml#Advanced_Token_Economy

## UltraCompressed Mode Integration
@include shared/superclaude-core.yml#UltraCompressed_Mode

## Code Economy
@include shared/superclaude-core.yml#Code_Economy

## Cost & Performance Optimization
@include shared/superclaude-core.yml#Cost_Performance_Optimization

## Intelligent Auto-Activation
@include shared/superclaude-core.yml#Intelligent_Auto_Activation

## Task Management
@include shared/superclaude-core.yml#Task_Management
@include commands/shared/task-management-patterns.yml#Task_Management_Hierarchy

## Performance Standards
@include shared/superclaude-core.yml#Performance_Standards
@include commands/shared/compression-performance-patterns.yml#Performance_Baselines

## Output Organization
@include shared/superclaude-core.yml#Output_Organization

## Session Management
@include shared/superclaude-core.yml#Session_Management
@include commands/shared/system-config.yml#Session_Settings

## Rules & Standards

### Evidence-Based Standards
@include shared/superclaude-core.yml#Evidence_Based_Standards

### Standards
@include shared/superclaude-core.yml#Standards

### Severity System
@include commands/shared/quality-patterns.yml#Severity_Levels
@include commands/shared/quality-patterns.yml#Validation_Sequence

### Smart Defaults & Handling
@include shared/superclaude-rules.yml#Smart_Defaults

### Ambiguity Resolution
@include shared/superclaude-rules.yml#Ambiguity_Resolution

### Development Practices
@include shared/superclaude-rules.yml#Development_Practices

### Code Generation
@include shared/superclaude-rules.yml#Code_Generation

### Session Awareness
@include shared/superclaude-rules.yml#Session_Awareness

### Action & Command Efficiency
@include shared/superclaude-rules.yml#Action_Command_Efficiency

### Project Quality
@include shared/superclaude-rules.yml#Project_Quality

### Security Standards
@include shared/superclaude-rules.yml#Security_Standards
@include commands/shared/security-patterns.yml#OWASP_Top_10
@include commands/shared/security-patterns.yml#Validation_Levels

### Efficiency Management
@include shared/superclaude-rules.yml#Efficiency_Management

### Operations Standards
@include shared/superclaude-rules.yml#Operations_Standards

## Model Context Protocol (MCP) Integration

### MCP Architecture
@include commands/shared/flag-inheritance.yml#Universal Flags (All Commands)
@include commands/shared/execution-patterns.yml#Servers

### Server Capabilities Extended
@include shared/superclaude-mcp.yml#Server_Capabilities_Extended

### Token Economics
@include shared/superclaude-mcp.yml#Token_Economics

### Workflows
@include shared/superclaude-mcp.yml#Workflows

### Quality Control
@include shared/superclaude-mcp.yml#Quality_Control

### Command Integration
@include shared/superclaude-mcp.yml#Command_Integration

### Error Recovery
@include shared/superclaude-mcp.yml#Error_Recovery

### Best Practices
@include shared/superclaude-mcp.yml#Best_Practices

### Session Management
@include shared/superclaude-mcp.yml#Session_Management

## Cognitive Archetypes (Personas)

### Persona Architecture
@include commands/shared/flag-inheritance.yml#Universal Flags (All Commands)

### All Personas
@include shared/superclaude-personas.yml#All_Personas

### Collaboration Patterns
@include shared/superclaude-personas.yml#Collaboration_Patterns

### Intelligent Activation Patterns
@include shared/superclaude-personas.yml#Intelligent_Activation_Patterns

### Command Specialization
@include shared/superclaude-personas.yml#Command_Specialization

### Integration Examples
@include shared/superclaude-personas.yml#Integration_Examples

### Advanced Features
@include shared/superclaude-personas.yml#Advanced_Features

### MCP + Persona Integration
@include shared/superclaude-personas.yml#MCP_Persona_Integration

---
*SuperClaude v2.0.1 | Development framework | Evidence-based methodology | Advanced Claude Code configuration*