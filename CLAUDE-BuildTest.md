# CLAUDE-BuildTest.md - Build & Test Commands

Essential build and test commands with environment adaptation for all AutoBot-Enterprise environments.

## 🚨 **ENVIRONMENT ADAPTATION**

This file uses environment-agnostic placeholders that work across all git worktrees:

### **Visual Studio Path Resolution:**
Replace `{VISUAL_STUDIO_PATH}` with your environment-specific path:

```bash
# Windows (PowerShell/CMD)
{VISUAL_STUDIO_PATH} = "C:\Program Files\Microsoft Visual Studio\2022\{YOUR_EDITION}"

# WSL2 (Linux/Ubuntu)  
{VISUAL_STUDIO_PATH} = "/mnt/c/Program Files/Microsoft Visual Studio/2022/{YOUR_EDITION}"

# Alternative Editions
# Replace {YOUR_EDITION} with "Enterprise", "Professional", or "Community"
```

### **Repository Root Resolution:**
All relative paths (starting with `./`) are automatically resolved by Claude Code to your current working directory.

## 🔨 **BUILD COMMANDS**

### **Full Solution Build**
```bash
{VISUAL_STUDIO_PATH}/MSBuild/Current/Bin/MSBuild.exe "AutoBot-Enterprise.sln" /t:Clean,Restore,Rebuild /p:Configuration=Debug /p:Platform=x64
```

### **Test Project Build** (Recommended)
```bash
{VISUAL_STUDIO_PATH}/MSBuild/Current/Bin/MSBuild.exe "./AutoBotUtilities.Tests/AutoBotUtilities.Tests.csproj" /t:Rebuild /p:Configuration=Debug /p:Platform=x64
```

### **Specific Project Build**
```bash
{VISUAL_STUDIO_PATH}/MSBuild/Current/Bin/MSBuild.exe "./AutoBot/AutoBotUtilities.csproj" /t:Rebuild /p:Configuration=Debug /p:Platform=x64
```

## 🧪 **TEST COMMANDS**

### **🎯 CRITICAL TEST: MANGO OCR Integration**
```bash
{VISUAL_STUDIO_PATH}/Common7/IDE/CommonExtensions/Microsoft/TestWindow/vstest.console.exe "./AutoBotUtilities.Tests/bin/x64/Debug/net48/AutoBotUtilities.Tests.dll" /TestCaseFilter:"FullyQualifiedName=AutoBotUtilities.Tests.PDFImportTests.CanImportMango03152025TotalAmount_AfterLearning" "/Logger:console;verbosity=detailed"
```

### **Run All Tests**
```bash
{VISUAL_STUDIO_PATH}/Common7/IDE/CommonExtensions/Microsoft/TestWindow/vstest.console.exe "./AutoBotUtilities.Tests/bin/x64/Debug/net48/AutoBotUtilities.Tests.dll" "/Logger:console;verbosity=detailed"
```

### **PDF Import Tests**
```bash
{VISUAL_STUDIO_PATH}/Common7/IDE/CommonExtensions/Microsoft/TestWindow/vstest.console.exe "./AutoBotUtilities.Tests/bin/x64/Debug/net48/AutoBotUtilities.Tests.dll" /TestCaseFilter:"FullyQualifiedName~PDFImportTests" "/Logger:console;verbosity=detailed"
```

### **Amazon Invoice Test**
```bash
{VISUAL_STUDIO_PATH}/Common7/IDE/CommonExtensions/Microsoft/TestWindow/vstest.console.exe "./AutoBotUtilities.Tests/bin/x64/Debug/net48/AutoBotUtilities.Tests.dll" /TestCaseFilter:"FullyQualifiedName=AutoBotUtilities.Tests.PDFImportTests.CanImportAmazoncomOrder11291264431163432" "/Logger:console;verbosity=detailed"
```

### **DeepSeek Diagnostic Tests**
```bash
{VISUAL_STUDIO_PATH}/Common7/IDE/CommonExtensions/Microsoft/TestWindow/vstest.console.exe "./AutoBotUtilities.Tests/bin/x64/Debug/net48/AutoBotUtilities.Tests.dll" /TestCaseFilter:"FullyQualifiedName~DeepSeekDiagnosticTests" "/Logger:console;verbosity=detailed"
```

### **Generic PDF Test Suite**
```bash
{VISUAL_STUDIO_PATH}/Common7/IDE/CommonExtensions/Microsoft/TestWindow/vstest.console.exe "./AutoBotUtilities.Tests/bin/x64/Debug/net48/AutoBotUtilities.Tests.dll" /TestCaseFilter:"FullyQualifiedName~GenericPDFImportTest" "/Logger:console;verbosity=detailed"
```

## 📊 **LOG ANALYSIS** (CRITICAL)

### **🚨 MANDATORY: Use Log Files, Never Console Output**

**Console output truncates and hides critical failures!**

```bash
# Read COMPLETE log file (MANDATORY)
tail -100 "./AutoBotUtilities.Tests/bin/x64/Debug/net48/Logs/AutoBotTests-YYYYMMDD.log"

# Search for completion markers
grep -A5 -B5 "TEST_RESULT\|FINAL_STATUS\|STRATEGY_COMPLETE" ./AutoBotUtilities.Tests/bin/x64/Debug/net48/Logs/AutoBotTests-*.log

# Check latest log file
ls -la "./AutoBotUtilities.Tests/bin/x64/Debug/net48/Logs/" | tail -5
```

### **Database Verification** (Requires MCP Server)
```bash
# Verify OCR correction results (example)
# Replace {DATABASE_SERVER} with your SQL Server instance
sqlcmd -S "{DATABASE_SERVER}" -Q "SELECT Success FROM OCRCorrectionLearning WHERE CreatedDate >= '2025-08-01'"
```

## ⚠️ **CRITICAL TESTING PRINCIPLES**

1. **ALWAYS check log files** - Console output is incomplete and misleading
2. **Read from END of log file** - Critical results appear at the end
3. **Verify database outcomes** - Not just API call success
4. **Complete builds before testing** - Partial builds cause confusing failures
5. **Use x64 platform configuration** - Required for all builds and tests

## 🔧 **ENVIRONMENT-SPECIFIC EXAMPLES**

### **Windows PowerShell Example:**
```powershell
# Set your environment path (replace {YOUR_EDITION} with Enterprise/Professional/Community)
$VS_PATH = "C:\Program Files\Microsoft Visual Studio\2022\{YOUR_EDITION}"

# Build and test
& "$VS_PATH\MSBuild\Current\Bin\MSBuild.exe" "AutoBotUtilities.Tests\AutoBotUtilities.Tests.csproj" /t:Rebuild /p:Configuration=Debug /p:Platform=x64
& "$VS_PATH\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" ".\AutoBotUtilities.Tests\bin\x64\Debug\net48\AutoBotUtilities.Tests.dll" /TestCaseFilter:"FullyQualifiedName~PDFImportTests" "/Logger:console;verbosity=detailed"
```

### **WSL2 Bash Example:**
```bash
# Set your environment path (replace {YOUR_EDITION} with Enterprise/Professional/Community)
VS_PATH="/mnt/c/Program Files/Microsoft Visual Studio/2022/{YOUR_EDITION}"

# Build and test
"$VS_PATH/MSBuild/Current/Bin/MSBuild.exe" "./AutoBotUtilities.Tests/AutoBotUtilities.Tests.csproj" /t:Rebuild /p:Configuration=Debug /p:Platform=x64
"$VS_PATH/Common7/IDE/CommonExtensions/Microsoft/TestWindow/vstest.console.exe" "./AutoBotUtilities.Tests/bin/x64/Debug/net48/AutoBotUtilities.Tests.dll" /TestCaseFilter:"FullyQualifiedName~PDFImportTests" "/Logger:console;verbosity=detailed"
```

## 📋 **BUILD TROUBLESHOOTING**

### **Common Issues:**

**Platform Mismatch:**
- Ensure `/p:Platform=x64` is specified for all builds
- AnyCPU will cause test failures

**Path Issues:**
- Use quoted paths for directories with spaces
- Verify Visual Studio installation path
- Check .NET Framework 4.8 is installed

**Permission Issues:**
- Run with appropriate permissions
- WSL2 may require additional configuration

---

*This build guide works across all AutoBot-Enterprise environments. Replace placeholders with your environment-specific paths.*