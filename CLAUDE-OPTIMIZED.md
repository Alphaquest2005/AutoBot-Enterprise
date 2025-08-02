# CLAUDE.md - AutoBot-Enterprise Development Hub

> **🎯 Optimized for Claude Code Efficiency** - Essential information with links to comprehensive documentation

## 🚀 QUICK START ESSENTIALS

### **🔥 CRITICAL COMMANDS** (Most Used)

#### **MANGO Test** (Primary OCR Integration Test)
```bash
"/mnt/c/Program Files/Microsoft Visual Studio/2022/Enterprise/Common7/IDE/CommonExtensions/Microsoft/TestWindow/vstest.console.exe" "./AutoBotUtilities.Tests/bin/x64/Debug/net48/AutoBotUtilities.Tests.dll" /TestCaseFilter:"FullyQualifiedName=AutoBotUtilities.Tests.PDFImportTests.CanImportMango03152025TotalAmount_AfterLearning" "/Logger:console;verbosity=detailed"
```

#### **Build Command** (WSL)
```bash
"/mnt/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" "AutoBotUtilities.Tests/AutoBotUtilities.Tests.csproj" /t:Rebuild /p:Configuration=Debug /p:Platform=x64
```

#### **Log Analysis** (MANDATORY - Console truncates!)
```bash
# ALWAYS read log files, never rely on console output
tail -100 "./AutoBotUtilities.Tests/bin/x64/Debug/net48/Logs/AutoBotTests-YYYYMMDD.log"

# Search for completion markers
grep -A5 -B5 "TEST_RESULT\|FINAL_STATUS\|STRATEGY_COMPLETE" LogFile.log
```

### **📁 CRITICAL FILE PATHS**

**Repository Root**: `/mnt/c/Insight Software/AutoBot-Enterprise/`

**Key OCR Service Files**:
- `/mnt/c/Insight Software/AutoBot-Enterprise/InvoiceReader/OCRCorrectionService/OCRCorrectionService.cs`
- `/mnt/c/Insight Software/AutoBot-Enterprise/InvoiceReader/OCRCorrectionService/OCRErrorDetection.cs`
- `/mnt/c/Insight Software/AutoBot-Enterprise/InvoiceReader/OCRCorrectionService/OCRPromptCreation.cs`

### **🚨 CRITICAL RULES** (NEVER VIOLATE)

1. **ALWAYS USE LOG FILES** - Console output truncates and hides failures
2. **NEVER DEGRADE CODE** - Fix compilation by correcting syntax, not removing functionality  
3. **RESPECT ESTABLISHED PATTERNS** - Research existing code before creating new solutions
4. **COMPREHENSIVE LOGGING** - Every method includes business success criteria validation

---

## 📚 COMPREHENSIVE DOCUMENTATION

### **🔥 HIGH PRIORITY** (Daily Development)
- **[BUILD-AND-TEST.md](BUILD-AND-TEST.md)** - All build commands, test procedures, diagnostic tests
- **[DEVELOPMENT-STANDARDS.md](DEVELOPMENT-STANDARDS.md)** - Critical mandates, logging requirements, debugging protocols
- **[ARCHITECTURE-OVERVIEW.md](ARCHITECTURE-OVERVIEW.md)** - OCR service architecture, system overview, core workflow

### **⚙️ SETUP & CONFIGURATION** (Initial Setup)
- **[DATABASE-AND-MCP.md](DATABASE-AND-MCP.md)** - MCP SQL Server setup, database configuration, credentials
- **[ADVANCED-FEATURES.md](ADVANCED-FEATURES.md)** - Git worktree system, folder synchronization, advanced workflows

### **🎯 SPECIALIZED TOPICS** (As Needed)
- **[AI-TEMPLATE-SYSTEM.md](AI-TEMPLATE-SYSTEM.md)** - Latest AI-powered template implementation (July 2025)
- **[LOGGING-DIAGNOSTICS.md](LOGGING-DIAGNOSTICS.md)** - Strategic logging system, LLM diagnosis capabilities
- **[HISTORICAL-SESSIONS.md](HISTORICAL-SESSIONS.md)** - Previous breakthroughs, archived sessions, lessons learned

---

## 🎯 DEVELOPMENT CONTEXT

### **Current System State**
- **Production System**: Fully functional customs document processing
- **OCR Service**: Latest addition to existing pipeline - must integrate seamlessly
- **AI Template System**: Ultra-simple implementation with advanced capabilities
- **Status**: 90% complete fallback configuration system implemented

### **Key Success Criteria** (8-Dimension Framework)
Every method validates: 🎯 Purpose Fulfillment, 📊 Output Completeness, ⚙️ Process Completion, 🔍 Data Quality, 🛡️ Error Handling, 💼 Business Logic, 🔗 Integration Success, ⚡ Performance Compliance

### **Current Focus**
- **MANGO Test Integration**: OCR service must create templates and process invoices end-to-end
- **Template System**: AI-powered multi-provider template optimization
- **Fail-Fast Architecture**: Production-ready termination mechanisms

---

## 📋 SUPERCLAUDE CONFIGURATION

### **Enhanced Claude Capabilities**
- **Advanced Token Economy**: Optimized for maximum efficiency
- **Strategic Logging Lens**: Surgical debugging with dynamic focus
- **Multi-Environment Development**: Parallel debugging with git worktree
- **Cognitive Archetypes**: Specialized personas for different development phases

### **MCP Integration** 
- **SQL Server Access**: `MINIJOE\SQLDEVELOPER2022` / `WebSource-AutoBot`
- **Credentials**: `sa` / `pa$word`
- **Quick Start**: `cd "C:\Insight Software\AutoBot-Enterprise\mcp-servers\mssql-mcp-server" && npm start`

---

## 🔍 QUICK PROBLEM SOLVING

### **Common Issues**
- **Test Failures**: Check log files (not console), search for "STRATEGY_COMPLETE"
- **Build Errors**: Use surgical fixes at specific line numbers, never remove functionality
- **OCR Integration**: Verify template context and database persistence
- **DeepSeek API**: Check timeout settings and response parsing

### **Emergency Commands**
```bash
# Full solution rebuild
"/mnt/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" AutoBot-Enterprise.sln /t:Clean,Restore,Rebuild /p:Configuration=Debug /p:Platform=x64

# Database verification
sqlcmd -S "MINIJOE\SQLDEVELOPER2022" -U sa -P "pa`$word" -d "WebSource-AutoBot" -Q "SELECT TOP 10 * FROM OCRCorrectionLearning ORDER BY Id DESC"

# Log file location
ls -la "./AutoBotUtilities.Tests/bin/x64/Debug/net48/Logs/"
```

---

## 📖 DOCUMENTATION STATUS

**✅ Complete Documentation Available**:
- All information from 26,413 token original file preserved
- Organized into focused, searchable documents
- Optimized for Claude Code quick access and navigation
- Historical context and implementation details maintained

**🎯 Optimization Benefits**:
- **Faster Claude Startup**: Streamlined main file loads instantly
- **Better Navigation**: Topic-specific documentation files
- **Improved Search**: Focused files easier to grep and analyze
- **Maintenance Efficiency**: Update specific areas without navigating massive file

---

*Last Updated: 2025-08-02 | SuperClaude v2.0.1 | Optimized Documentation Architecture*