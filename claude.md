# CLAUDE.md - AutoBot-Enterprise Configuration Guide

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## 🎯 **ENVIRONMENT AUTO-DETECTION**

**Claude Code automatically detects your environment**. This documentation works seamlessly in:
- **Main Repository**: Any path ending in `/AutoBot-Enterprise` 
- **Alpha Worktree**: Any path ending in `/AutoBot-Enterprise-alpha`
- **Beta Worktree**: Any path ending in `/AutoBot-Enterprise-beta`
- **Any Future Worktrees**: Claude Code adapts paths automatically using `<env>` context

> **Note**: All commands and paths below are environment-agnostic. Claude Code will resolve them to your current working directory automatically.

## 📋 **DOCUMENTATION STRUCTURE**

This documentation is organized into specialized files for better maintainability:

### 🚀 **Quick Start (Essential)**
- **[Build & Test Commands](CLAUDE-BuildTest.md)** - Essential build and test commands with environment adaptation
- **[Configuration Setup](CLAUDE-Configuration.md)** - Database, MCP server, and tool configuration
- **[Critical Logging Guide](CLAUDE-Logging.md)** - Mandatory log file usage and analysis protocols

### 🏗️ **Architecture & Development**  
- **[System Architecture](CLAUDE-Architecture.md)** - High-level system overview and core workflows
- **[OCR Service Architecture](CLAUDE-OCR.md)** - Complete OCR correction service implementation
- **[Development Practices](CLAUDE-Development.md)** - Code preservation mandates and development standards

### 📚 **Advanced & Historical**
- **[Session History](CLAUDE-History.md)** - Critical breakthroughs and previous session archive
- **[AI Template System](CLAUDE-AI-Templates.md)** - Latest AI-powered template implementation
- **[Fallback Control System](CLAUDE-Fallbacks.md)** - Production-ready fallback configuration

## 🚨 **MOST CRITICAL COMMANDS** (Quick Reference)

### **MANGO Test (Primary OCR Test)**
```bash
{VISUAL_STUDIO_PATH}/vstest.console.exe "./AutoBotUtilities.Tests/bin/x64/Debug/net48/AutoBotUtilities.Tests.dll" /TestCaseFilter:"FullyQualifiedName=AutoBotUtilities.Tests.PDFImportTests.CanImportMango03152025TotalAmount_AfterLearning" "/Logger:console;verbosity=detailed"
```

### **Build Command**
```bash
{VISUAL_STUDIO_PATH}/MSBuild.exe "AutoBotUtilities.Tests/AutoBotUtilities.Tests.csproj" /t:Rebuild /p:Configuration=Debug /p:Platform=x64
```

### **Log Analysis (MANDATORY)**
```bash
# ALWAYS read log files, never console output - console truncates critical failures
tail -100 "./AutoBotUtilities.Tests/bin/x64/Debug/net48/Logs/AutoBotTests-YYYYMMDD.log"
```

## 🔧 **ENVIRONMENT ADAPTATION GUIDE**

### **Path Placeholders Used in Documentation:**
- `{REPO_ROOT}` - Your repository root (auto-detected by Claude Code)
- `{VISUAL_STUDIO_PATH}` - Visual Studio installation path (see below)
- `{DATABASE_SERVER}` - Your SQL Server instance (see configuration docs)
- `{USER_HOME}` - Your user home directory
- `{MCP_SERVER_PATH}` - MCP server location (see configuration docs)

### **Visual Studio Path Resolution:**
```bash
# Windows (standard)
"C:\Program Files\Microsoft Visual Studio\2022\{YOUR_EDITION}\MSBuild\Current\Bin\MSBuild.exe"

# WSL2
"/mnt/c/Program Files/Microsoft Visual Studio/2022/{YOUR_EDITION}/MSBuild/Current/Bin/MSBuild.exe"

# Alternative editions (replace {YOUR_EDITION} with Enterprise/Professional/Community as needed)
```

## 🚨 **CRITICAL MANDATES** (MUST READ - OVERRIDE ALL OTHER INSTRUCTIONS)

### ❗❗❗ **SUPREME DIRECTIVE: CODE PRESERVATION MANDATE v2.0** ❗❗❗

**🚫 ABSOLUTELY FORBIDDEN DESTRUCTIVE PATTERN:**
```
See compilation error → "Code must be corrupted" → DELETE ENTIRE FUNCTIONS → OBLITERATE FUNCTIONALITY
```

**✅ MANDATORY SURGICAL PATTERN:**
```
See compilation error → "Line X has syntax error" → FIX ONLY THAT LINE → PRESERVE ALL FUNCTIONALITY
```

**ZERO TOLERANCE VIOLATIONS:**
- ❌ Deleting entire functions/methods to fix syntax errors
- ❌ Removing working code blocks during compilation fixes
- ❌ Treating working code as "corrupted" or "orphaned"

### 🚨 **CRITICAL LOGGING MANDATE**
1. **ALWAYS USE LOG FILES** - Console output truncates and hides failures
2. **READ FROM END OF LOG FILES** - Critical results appear at the end
3. **VERIFY DATABASE OUTCOMES** - Not just API call success

### 🏗️ **DEVELOPMENT MANDATES**
1. **RESPECT ESTABLISHED PATTERNS** - Research existing code before creating new solutions
2. **ENVIRONMENT AWARENESS** - Let Claude Code auto-detect paths using `<env>` context
3. **SURGICAL FIXES ONLY** - Fix specific syntax errors without removing functionality

**📋 Complete Details**: See [Development Practices](CLAUDE-Development.md) and [Critical Logging Guide](CLAUDE-Logging.md)

## 🎯 **SUCCESS CRITERIA FRAMEWORK**

Every method must validate these 8 dimensions:
1. 🎯 **PURPOSE_FULFILLMENT** - Method achieves stated business objective
2. 📊 **OUTPUT_COMPLETENESS** - Returns complete, well-formed data structures
3. ⚙️ **PROCESS_COMPLETION** - All required processing steps executed
4. 🔍 **DATA_QUALITY** - Output meets business rules and validation
5. 🛡️ **ERROR_HANDLING** - Appropriate error detection and recovery
6. 💼 **BUSINESS_LOGIC** - Behavior aligns with business requirements
7. 🔗 **INTEGRATION_SUCCESS** - External dependencies respond appropriately
8. ⚡ **PERFORMANCE_COMPLIANCE** - Execution within reasonable timeframes

## 📁 **KEY FILE LOCATIONS** (Relative to Repository Root)

### **Core Business Logic**
- `./AutoBot/Utils.cs` - Main business orchestrator
- `./AutoBot/PDFUtils.cs` - Document processing engine
- `./AutoBot/ImportUtils.cs` - File processing workflows

### **OCR Correction Service**
- `./InvoiceReader/OCRCorrectionService/OCRCorrectionService.cs`
- `./InvoiceReader/OCRCorrectionService/OCRErrorDetection.cs`
- `./WaterNut.Business.Services/Utils/DeepSeek/DeepSeekInvoiceApi.cs`

### **Tests**
- `./AutoBotUtilities.Tests/` - All test files
- `./AutoBotUtilities.Tests/Test Data/` - Sample documents for testing

### **Configuration**
- `./CoreEntities/CoreEntities.edmx` - Main data model
- `./AutoBot/App.config` - Application configuration

---

*This environment-agnostic CLAUDE.md works seamlessly across all repository locations - main branch, worktrees, and future environments. Claude Code automatically adapts paths using its built-in environment awareness.*