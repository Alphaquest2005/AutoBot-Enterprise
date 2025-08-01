# CLAUDE.md - WORKTREE-SPECIFIC Configuration

## 🚨 **WORKTREE ENVIRONMENT** (debug-beta branch)
**Current Path**: `/mnt/c/Insight Software/AutoBot-Enterprise-beta`
**Branch**: `debug-beta`  
**Status**: Active development worktree

### **⚠️ CRITICAL PATH ISSUE**
- **This Worktree**: `/mnt/c/Insight Software/AutoBot-Enterprise-beta` (debug-beta branch)
- **Main Branch**: `/mnt/c/Insight Software/AutoBot-Enterprise` (master branch)
- **Problem**: InvoiceReader project exists only in main branch, NOT in this worktree
- **MCP Server**: Must use main branch location
- **Git Status**: CLAUDE.md now in .gitignore - won't sync with main branch

### **Worktree Build Commands**
```bash
# Build test project (current worktree)
"/mnt/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" "./AutoBotUtilities.Tests/AutoBotUtilities.Tests.csproj" /t:Rebuild /p:Configuration=Debug /p:Platform=x64

# Run tests (current worktree)
"/mnt/c/Program Files/Microsoft Visual Studio/2022/Enterprise/Common7/IDE/CommonExtensions/Microsoft/TestWindow/vstest.console.exe" "./AutoBotUtilities.Tests/bin/x64/Debug/net48/AutoBotUtilities.Tests.dll" /TestCaseFilter:"FullyQualifiedName~ImportShipment" "/Logger:console;verbosity=detailed"
```

### **MCP Server (Main Branch Only)**
```powershell
# Start MCP Server - MUST use main branch
cd "C:\Insight Software\AutoBot-Enterprise\mcp-servers\mssql-mcp-server"
npm start
```

## Development Notes
- Always use logfile, console logs truncate
- Double-check changes after code modifications
- Use current log file, not old ones
- **CRITICAL**: All paths must use worktree location except MCP server

## Git Worktree Sync Issue Resolution
**Problem**: Files added to master after worktree creation are missing (InvoiceReader)
**Current Fix**: CLAUDE.md added to .gitignore to prevent sync conflicts
**Next Step**: Restart Claude Code to reload correct paths in memory
**Status**: ✅ Ready for Claude Code restart

## 🎯 **COMPLETED TASKS**
✅ Fixed critical validation failure in AITemplateService.cs  
✅ Replaced magic strings with FileTypeManager.EntryTypes constants  
✅ Updated CLAUDE.md with correct worktree paths  
✅ Added CLAUDE.md to .gitignore to prevent main branch conflicts