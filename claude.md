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
- **Project-Specific**: `/mnt/c/Insight Software/AutoBot-Enterprise-alpha/.claude` (project context)

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
ls -la "/home/joseph/.claude" "/mnt/c/Insight Software/AutoBot-Enterprise-alpha/.claude"

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
chmod +x "/mnt/c/Insight Software/AutoBot-Enterprise-alpha/.claude/claude-folder-sync.sh"
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
diff ~/.claude/CLAUDE.md "/mnt/c/Insight Software/AutoBot-Enterprise-alpha/.claude/CLAUDE.md"  # Check differences
crontab -l                                  # Check auto-sync
```

### **System Status**
**✅ Status**: Fully operational - Both folders synchronized and identical  
**🔄 Sync Method**: Bidirectional merge with intelligent conflict resolution  
**💾 Backup System**: Automatic timestamped backups before manual sync  
**⚡ Auto-Sync**: Optional 15-minute automated synchronization available  

**📖 Full Documentation**: `CLAUDE_FOLDER_SYNC_DOCUMENTATION.md`

---

## 🔀 GIT WORKTREE PARALLEL DEBUGGING SYSTEM

### **🎯 3-Environment Architecture for Parallel Development**

**This repository operates with THREE synchronized debugging environments for parallel development and testing:**

#### **Environment 1: Main Repository** 
- **Location**: `/mnt/c/Insight Software/AutoBot-Enterprise-alpha/` (current directory)
- **Branch**: `Autobot-Enterprise.2.0` (primary development branch)
- **Purpose**: Primary development and testing environment
- **Space**: 13GB (original repository)

#### **Environment 2: Alpha Worktree**
- **Location**: `/mnt/c/Insight Software/AutoBot-Enterprise-alpha/`
- **Branch**: Configurable (can be different branch/commit for parallel testing)
- **Purpose**: Secondary debugging environment for experimental work
- **Space**: ~1-2GB additional (shares .git metadata with main)

#### **Environment 3: Beta Worktree**
- **Location**: `/mnt/c/Insight Software/AutoBot-Enterprise-beta/`
- **Branch**: Configurable (can be different branch/commit for comparison)
- **Purpose**: Third debugging environment for baseline comparison
- **Space**: ~1-2GB additional (shares .git metadata with main)

**Total Space Efficiency**: ~15-16GB instead of 39GB (3×13GB with separate clones)

### **⚡ Worktree Setup Commands**

#### **Initial Setup** (Run from main repository)
```bash
# Navigate to parent directory
cd "/mnt/c/Insight Software/"

# Create Alpha worktree (experimental debugging)
git worktree add -b "debug-alpha" "../AutoBot-Enterprise-alpha" "Autobot-Enterprise.2.0"

# Create Beta worktree (baseline comparison)
git worktree add -b "debug-beta" "../AutoBot-Enterprise-beta" "master"

# Verify worktree creation
git worktree list
```

#### **Alternative Setup Strategies**
```bash
# Option 1: Same Branch Testing (parallel debugging on same code)
git worktree add "../AutoBot-Enterprise-alpha" "Autobot-Enterprise.2.0"
git worktree add "../AutoBot-Enterprise-beta" "Autobot-Enterprise.2.0"

# Option 2: Experimental Branch Creation  
git worktree add -b "experimental-alpha" "../AutoBot-Enterprise-alpha" "Autobot-Enterprise.2.0"
git worktree add -b "experimental-beta" "../AutoBot-Enterprise-beta" "Autobot-Enterprise.2.0"

# Option 3: Baseline Comparison (current vs stable)
git worktree add "../AutoBot-Enterprise-alpha" "Autobot-Enterprise.2.0" 
git worktree add "../AutoBot-Enterprise-beta" "master"
```

### **🚀 Environment Navigation for LLMs**

#### **Switching Between Environments**
```bash
# Move to Main Environment
cd "/mnt/c/Insight Software/AutoBot-Enterprise-alpha/"

# Move to Alpha Environment  
cd "/mnt/c/Insight Software/AutoBot-Enterprise-alpha/"

# Move to Beta Environment
cd "/mnt/c/Insight Software/AutoBot-Enterprise-beta/"
```

#### **Environment Awareness Commands**
```bash
# Identify current environment
pwd && git branch --show-current

# List all available environments
git worktree list

# Check status across all environments
git worktree list --porcelain
```

### **🔄 Merge Strategy: Integrating Changes Back to Main**

#### **Scenario 1: Single Environment Changes**

**Alpha → Main Integration**
```bash
# From main repository directory
cd "/mnt/c/Insight Software/AutoBot-Enterprise-alpha/"

# Fetch changes from alpha branch
git fetch . debug-alpha:debug-alpha

# Review changes before merge
git diff Autobot-Enterprise.2.0..debug-alpha
git log Autobot-Enterprise.2.0..debug-alpha --oneline

# Merge alpha changes
git merge debug-alpha
# OR selective merge specific files
git checkout debug-alpha -- path/to/specific/files
```

#### **Scenario 2: Multi-Environment Integration**

**Alpha + Beta → Main** (Comprehensive Integration)
```bash
# 1. Create integration branch for testing
git checkout -b integration-merge Autobot-Enterprise.2.0

# 2. Merge alpha changes first
git merge debug-alpha

# 3. Test build after alpha integration
"/mnt/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" AutoBot-Enterprise.sln /t:Rebuild

# 4. If successful, merge beta changes
git merge debug-beta

# 5. Test complete integration build
"/mnt/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" AutoBot-Enterprise.sln /t:Rebuild

# 6. If all tests pass, merge to main branch
git checkout Autobot-Enterprise.2.0
git merge integration-merge
```

#### **Scenario 3: Selective Cherry-Picking**
```bash
# Cherry-pick specific commits from alpha
git log debug-alpha --oneline
git cherry-pick <commit-hash-1> <commit-hash-2>

# Cherry-pick specific commits from beta
git log debug-beta --oneline  
git cherry-pick <commit-hash-3> <commit-hash-4>
```

### **🛡️ Conflict Resolution Strategy**

**Priority Order**: Main > Alpha > Beta

```bash
# During merge conflicts
git status                              # See conflicted files
git diff --name-only --diff-filter=U   # List conflict files

# For .NET project conflicts (common scenarios)
# 1. Project files (.csproj) - prioritize main repository version
# 2. Config files (app.config, web.config) - merge carefully  
# 3. Database scripts - manual review required
# 4. Code files - contextual resolution based on functionality

# Complete conflict resolution
git add .
git commit -m "Merge: Resolved conflicts between alpha/beta changes"
```

### **🧹 Worktree Management Commands**

#### **Status and Information**
```bash
# List all worktrees with status
git worktree list

# Show worktree details
git worktree list --porcelain

# Check if worktree is clean
git status --porcelain
```

#### **Cleanup and Removal**
```bash
# Remove worktree when done (from main repository)
git worktree remove "../AutoBot-Enterprise-alpha" 
git worktree remove "../AutoBot-Enterprise-beta"

# Prune stale worktree references
git worktree prune

# Force remove if worktree has uncommitted changes
git worktree remove --force "../AutoBot-Enterprise-alpha"
```

### **🎯 LLM Best Practices for Worktree Operation**

#### **Environment Identification Protocol**
```bash
# ALWAYS run this when starting work in any environment
echo "Current Environment: $(pwd)"
echo "Current Branch: $(git branch --show-current)"  
echo "Git Status: $(git status --porcelain)"
git worktree list
```

#### **Cross-Environment Awareness**
- **Before making changes**: Understand which environment you're in
- **Before committing**: Verify changes are in the intended environment
- **Before merging**: Always merge FROM worktrees TO main repository
- **Environment switching**: Use full absolute paths to avoid confusion

#### **Build Testing Strategy**  
```bash
# Test build in current environment before merging
"/mnt/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" "AutoBotUtilities.Tests/AutoBotUtilities.Tests.csproj" /t:Rebuild /p:Configuration=Debug /p:Platform=x64

# Run critical tests to verify functionality
"/mnt/c/Program Files/Microsoft Visual Studio/2022/Enterprise/Common7/IDE/CommonExtensions/Microsoft/TestWindow/vstest.console.exe" "./AutoBotUtilities.Tests/bin/x64/Debug/net48/AutoBotUtilities.Tests.dll" /TestCaseFilter:"FullyQualifiedName=AutoBotUtilities.Tests.PDFImportTests.CanImportMango03152025TotalAmount_AfterLearning" "/Logger:console;verbosity=detailed"
```

### **⚠️ Critical Warnings for LLMs**

1. **NEVER delete worktrees directly** - Always use `git worktree remove`
2. **ALWAYS merge TO main repository** - Never merge between worktrees directly  
3. **VERIFY environment before major changes** - Wrong environment = lost work
4. **BUILD TEST before merging** - Broken merges cause development delays
5. **PRESERVE .git integrity** - Worktrees share metadata, corruption affects all environments

### **🔧 Troubleshooting Common Issues**

#### **Worktree Creation Fails**
```bash
# If branch already exists
git worktree add --force "../AutoBot-Enterprise-alpha" existing-branch

# If directory already exists  
rm -rf "../AutoBot-Enterprise-alpha"
git worktree add "../AutoBot-Enterprise-alpha" branch-name
```

#### **Synchronization Issues**
```bash
# Refresh worktree tracking
git worktree prune
git worktree repair

# If worktree appears "missing"
git worktree list
cd "/path/to/worktree" && git status
```

#### **Merge Conflicts Resolution**
```bash
# Use merge tools for complex conflicts
git mergetool

# Manual resolution for simple conflicts
git diff --name-only --diff-filter=U | xargs code
# Edit files, then:
git add .
git commit
```

### **📊 Parallel Debugging Workflow Example**

**Typical Multi-Environment Development Session**:

1. **Main Environment**: Continue primary development on `Autobot-Enterprise.2.0` 
2. **Alpha Environment**: Test experimental OCR improvements on same branch
3. **Beta Environment**: Compare against stable `master` branch baseline
4. **Integration**: Merge successful experiments from Alpha back to Main
5. **Validation**: Run full test suite in Main environment before deployment

**Result**: 3× faster debugging with parallel testing and immediate comparison capabilities

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

# CLAUDE.md - COMPREHENSIVE RESTORE

*This file restores the complete CLAUDE.md content that was lost, combining all historical versions and optimizing for Claude usage.*

## 📋 TABLE OF CONTENTS

### 🔥 CRITICAL SECTIONS (Most Important)
1. [**🚨 CRITICAL LOGGING MANDATE**](#critical-logging-mandate) - Always use log files, never console output
2. [**🎯 CRITICAL TEST REFERENCE**](#critical-test-reference) - MANGO test command and key testing procedures
3. [**Build Commands**](#build-commands) - Essential build and test commands
4. [**Tool Usage - Correct File Paths**](#tool-usage---correct-file-paths) - Repository structure and key paths

### 📈 CURRENT DEVELOPMENT
5. [**🚀 AI-POWERED TEMPLATE SYSTEM**](#ai-powered-template-system) - Latest template implementation (July 2025)
6. [**🚨 LATEST: OCRCorrectionLearning System**](#latest-ocrcorrectionlearning-system) - Production-ready enhancements

### 🏗️ ARCHITECTURE & HISTORY
7. [**High-Level Architecture**](#high-level-architecture) - System overview and core workflow
8. [**OCR Correction Service Architecture**](#ocr-correction-service-architecture) - Complete implementation details
9. [**🚨 CRITICAL BREAKTHROUGHS**](#critical-breakthroughs) - Previous sessions archive

### 📋 GOVERNANCE & STANDARDS
10. [**Session Management & Continuity Protocol**](#session-management--continuity-protocol) - Development session tracking
11. [**🧠 Enhanced Ultradiagnostic Logging**](#enhanced-ultradiagnostic-logging) - Business success criteria validation
12. [**🏗️ The Established Codebase Respect Mandate**](#the-established-codebase-respect-mandate) - Development principles

### 📊 LOGGING & DIAGNOSTICS
13. [**🔍 Strategic Logging System**](#strategic-logging-system) - LLM diagnosis capabilities
14. [**Logging Unification Status**](#logging-unification-status) - Implementation status across projects

### 📚 HISTORICAL REFERENCES  
15. [**🚨 DeepSeek Generalization Enhancement**](#deepseek-generalization-enhancement) - June 2025 breakthrough
16. [**🚨 ARCHIVED: DeepSeek Diagnostic Test Results**](#archived-deepseek-diagnostic-test-results) - Amazon detection work

---

## 🚀 AI-POWERED TEMPLATE SYSTEM - ULTRA-SIMPLE IMPLEMENTATION (July 26, 2025) {#ai-powered-template-system}

### **🎯 REVOLUTIONARY APPROACH: Simple + Powerful = Success**

**Architecture**: ✅ **ULTRA-SIMPLE** - Single file implementation with advanced AI capabilities  
**Complexity**: ✅ **MINIMAL** - No external dependencies, pragmatic design  
**Functionality**: 🎯 **MAXIMUM** - Multi-provider AI, validation, recommendations, supplier intelligence

### **🏗️ SIMPLIFIED ARCHITECTURE OVERVIEW:**

```
📁 OCRCorrectionService/
├── AITemplateService.cs          # SINGLE FILE - ALL FUNCTIONALITY
├── 📁 Templates/
│   ├── 📁 deepseek/              # DeepSeek-optimized prompts
│   │   ├── header-detection.txt
│   │   └── mango-header.txt
│   ├── 📁 gemini/                # Gemini-optimized prompts
│   │   ├── header-detection.txt  
│   │   └── mango-header.txt
│   └── 📁 default/               # Fallback templates
│       └── header-detection.txt
├── 📁 Config/
│   ├── ai-providers.json         # AI provider configurations
│   └── template-config.json      # Template system settings
└── 📁 Recommendations/           # AI-generated improvements
    ├── deepseek-suggestions.json
    └── gemini-suggestions.json
```

### **🚀 6-PHASE IMPLEMENTATION PLAN (7-8 Hours Total)**

| Phase | Task | Duration | Status |
|-------|------|----------|--------|
| **Phase 1** | Create AITemplateService.cs (single file) | 2-3 hours | 🔄 Starting |
| **Phase 2** | Create provider-specific template files | 1 hour | ⏳ Pending |
| **Phase 3** | Create configuration files | 30 min | ⏳ Pending |
| **Phase 4** | Integrate with OCRPromptCreation.cs | 1 hour | ⏳ Pending |
| **Phase 5** | Create & run integration tests | 2 hours | ⏳ Pending |
| **Phase 6** | Run MANGO test until it passes | 1 hour | ⏳ Pending |

### **✨ FEATURES DELIVERED BY SIMPLE IMPLEMENTATION:**

✅ **Multi-Provider AI Integration**: DeepSeek + Gemini + extensible  
✅ **Template Validation**: Ensures templates 