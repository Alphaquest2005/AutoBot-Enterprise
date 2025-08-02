# BUILD AND TEST - AutoBot-Enterprise

> **🔧 Complete Build & Test Reference** - All commands, procedures, and diagnostic tools

## 📋 TABLE OF CONTENTS

1. [**🔥 CRITICAL COMMANDS**](#critical-commands) - Most used build/test commands
2. [**🏗️ BUILD COMMANDS**](#build-commands) - Solution and project builds
3. [**🧪 TEST COMMANDS**](#test-commands) - Comprehensive test execution
4. [**🎯 DIAGNOSTIC TESTS**](#diagnostic-tests) - Specialized diagnostic procedures
5. [**📁 FILE PATHS & LOCATIONS**](#file-paths-locations) - Critical paths and test data
6. [**🔍 SEARCH PATTERNS**](#search-patterns) - Common code search operations
7. [**📊 LOG ANALYSIS**](#log-analysis) - Log file analysis procedures

---

## 🔥 CRITICAL COMMANDS {#critical-commands}

### **🎯 MANGO Test** (Primary OCR Integration Test)
```bash
"/mnt/c/Program Files/Microsoft Visual Studio/2022/Enterprise/Common7/IDE/CommonExtensions/Microsoft/TestWindow/vstest.console.exe" "./AutoBotUtilities.Tests/bin/x64/Debug/net48/AutoBotUtilities.Tests.dll" /TestCaseFilter:"FullyQualifiedName=AutoBotUtilities.Tests.PDFImportTests.CanImportMango03152025TotalAmount_AfterLearning" "/Logger:console;verbosity=detailed"
```
**Purpose**: Tests OCR service end-to-end integration - template creation, database persistence, and invoice processing

### **🔨 Quick Build** (WSL)
```bash
"/mnt/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" "AutoBotUtilities.Tests/AutoBotUtilities.Tests.csproj" /t:Rebuild /p:Configuration=Debug /p:Platform=x64
```

### **📋 Log Analysis** (MANDATORY - Console truncates!)
```bash
# ALWAYS read log files, never rely on console output
tail -100 "./AutoBotUtilities.Tests/bin/x64/Debug/net48/Logs/AutoBotTests-YYYYMMDD.log"

# Search for completion markers
grep -A5 -B5 "TEST_RESULT\|FINAL_STATUS\|STRATEGY_COMPLETE" LogFile.log
```

---

## 🏗️ BUILD COMMANDS {#build-commands}

### **Full Solution Build**
```powershell
# PowerShell - Complete solution rebuild (x64 platform required)
& "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe" AutoBot-Enterprise.sln /t:Clean,Restore,Rebuild /p:Configuration=Debug /p:Platform=x64
```

### **Test Project Build**
```powershell
# PowerShell - Build specific project
& "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe" "AutoBotUtilities.Tests\AutoBotUtilities.Tests.csproj" /t:Clean,Restore,Rebuild /p:Configuration=Debug /p:Platform=x64
```

### **WSL Build Commands** (Verified Working)
```bash
# Test project rebuild (most common)
"/mnt/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" "AutoBotUtilities.Tests/AutoBotUtilities.Tests.csproj" /t:Rebuild /p:Configuration=Debug /p:Platform=x64

# Full solution rebuild
"/mnt/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" AutoBot-Enterprise.sln /t:Clean,Restore,Rebuild /p:Configuration=Debug /p:Platform=x64

# OCR Service rebuild
"/mnt/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" "InvoiceReader/InvoiceReader.csproj" /t:Rebuild /p:Configuration=Debug /p:Platform=x64
```

### **Build Troubleshooting**
- **Platform Requirements**: Must use `x64` platform (not AnyCPU)
- **Dependencies**: Some projects require `Clean,Restore,Rebuild` sequence
- **Timeout**: Large builds may take 10-20 minutes - don't interrupt
- **Path Spaces**: Always quote paths containing spaces

---

## 🧪 TEST COMMANDS {#test-commands}

### **Basic Test Execution**
```powershell
# Run all tests
& "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" ".\AutoBotUtilities.Tests\bin\x64\Debug\net48\AutoBotUtilities.Tests.dll" "/Logger:console;verbosity=detailed"

# Run specific test
& "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" ".\AutoBotUtilities.Tests\bin\x64\Debug\net48\AutoBotUtilities.Tests.dll" /TestCaseFilter:"FullyQualifiedName=AutoBotUtilities.Tests.PDFImportTests.CanImportAmazonMultiSectionInvoice_WithLogging" "/Logger:console;verbosity=detailed"

# Run tests in a class
& "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" ".\AutoBotUtilities.Tests\bin\x64\Debug\net48\AutoBotUtilities.Tests.dll" /TestCaseFilter:"FullyQualifiedName~PDFImportTests" "/Logger:console;verbosity=detailed"
```

### **Critical Test Suites**
```bash
# Amazon invoice test (20 min timeout) - Tests production invoice processing
"/mnt/c/Program Files/Microsoft Visual Studio/2022/Enterprise/Common7/IDE/CommonExtensions/Microsoft/TestWindow/vstest.console.exe" "./AutoBotUtilities.Tests/bin/x64/Debug/net48/AutoBotUtilities.Tests.dll" /TestCaseFilter:"FullyQualifiedName=AutoBotUtilities.Tests.PDFImportTests.CanImportAmazoncomOrder11291264431163432" "/Logger:console;verbosity=detailed"

# Generic PDF Test Suite (comprehensive with strategic logging)
"/mnt/c/Program Files/Microsoft Visual Studio/2022/Enterprise/Common7/IDE/CommonExtensions/Microsoft/TestWindow/vstest.console.exe" "./AutoBotUtilities.Tests/bin/x64/Debug/net48/AutoBotUtilities.Tests.dll" /TestCaseFilter:"FullyQualifiedName~GenericPDFImportTest" "/Logger:console;verbosity=detailed"

# DeepSeek diagnostic tests
"/mnt/c/Program Files/Microsoft Visual Studio/2022/Enterprise/Common7/IDE/CommonExtensions/Microsoft/TestWindow/vstest.console.exe" "./AutoBotUtilities.Tests/bin/x64/Debug/net48/AutoBotUtilities.Tests.dll" /TestCaseFilter:"FullyQualifiedName~DeepSeekDiagnosticTests" "/Logger:console;verbosity=detailed"

# Batch OCR Comparison Test
"/mnt/c/Program Files/Microsoft Visual Studio/2022/Enterprise/Common7/IDE/CommonExtensions/Microsoft/TestWindow/vstest.console.exe" "./AutoBotUtilities.Tests/bin/x64/Debug/net48/AutoBotUtilities.Tests.dll" /TestCaseFilter:"FullyQualifiedName~BatchOCRCorrectionComparison" "/Logger:console;verbosity=detailed"
```

---

## 🎯 DIAGNOSTIC TESTS {#diagnostic-tests}

### **Production Environment Tests**
```bash
# Amazon 03/14/2025 Order - Tests DeepSeek prompts in production with multi-field corrections
"/mnt/c/Program Files/Microsoft Visual Studio/2022/Enterprise/Common7/IDE/CommonExtensions/Microsoft/TestWindow/vstest.console.exe" "./AutoBotUtilities.Tests/bin/x64/Debug/net48/AutoBotUtilities.Tests.dll" /TestCaseFilter:"FullyQualifiedName=AutoBotUtilities.Tests.PDFImportTests.CanImportAmazon03142025Order_AfterLearning" "/Logger:console;verbosity=detailed"
```

### **DeepSeek Error Analysis Tests**
```bash
# Focused Diagnostic Test - Generates v1.1_Improved_Credit_Detection diagnostic files
"/mnt/c/Program Files/Microsoft Visual Studio/2022/Enterprise/Common7/IDE/CommonExtensions/Microsoft/TestWindow/vstest.console.exe" "./AutoBotUtilities.Tests/bin/x64/Debug/net48/AutoBotUtilities.Tests.dll" /TestCaseFilter:"FullyQualifiedName=AutoBotUtilities.Tests.DetailedDiagnosticGenerator.GenerateDetailedDiagnosticFiles_v1_1_FocusedTest" "/Logger:console;verbosity=detailed"

# MANGO Challenge Test - Specific MANGO invoice analysis (03152025_TOTAL AMOUNT.pdf)
"/mnt/c/Program Files/Microsoft Visual Studio/2022/Enterprise/Common7/IDE/CommonExtensions/Microsoft/TestWindow/vstest.console.exe" "./AutoBotUtilities.Tests/bin/x64/Debug/net48/AutoBotUtilities.Tests.dll" /TestCaseFilter:"FullyQualifiedName=AutoBotUtilities.Tests.DetailedDiagnosticGenerator.GenerateDetailedDiagnosticFiles_v1_1_MangoChallenge" "/Logger:console;verbosity=detailed"
```

### **Diagnostic Test Suite** ✅
**File**: `OCRCorrectionService.DeepSeekDiagnosticTests.cs`
- ✅ **Test 1**: CleanTextForAnalysis preserves financial patterns  
- ✅ **Test 2**: Prompt generation includes Amazon data
- ✅ **Test 3**: Amazon-specific regex patterns work (PASSED - detected issue)
- ✅ **Test 4**: DeepSeek response analysis
- ✅ **Test 5**: Response parsing validation
- ✅ **Test 6**: Complete pipeline integration

### **Test Filtering Patterns**
```bash
# Filter by test name
/TestCaseFilter:"TestName~Amazon"

# Filter by class
/TestCaseFilter:"FullyQualifiedName~PDFImportTests"

# Filter by category
/TestCaseFilter:"TestCategory=OCRCorrection"

# Combine filters
/TestCaseFilter:"FullyQualifiedName~GenericPDFImportTest AND TestCategory~01987"
```

---

## 📁 FILE PATHS & LOCATIONS {#file-paths-locations}

### **Repository Structure**
**Root**: `/mnt/c/Insight Software/AutoBot-Enterprise/`

### **Test Data Files**
```bash
# Amazon invoice test data
/mnt/c/Insight Software/AutoBot-Enterprise/AutoBotUtilities.Tests/Test Data/Amazon.com - Order 112-9126443-1163432.pdf
/mnt/c/Insight Software/AutoBot-Enterprise/AutoBotUtilities.Tests/Test Data/Amazon.com - Order 112-9126443-1163432.pdf.txt

# MANGO test data
/mnt/c/Insight Software/AutoBot-Enterprise/AutoBotUtilities.Tests/Test Data/03152025_TOTAL AMOUNT.pdf
/mnt/c/Insight Software/AutoBot-Enterprise/AutoBotUtilities.Tests/Test Data/03152025_TOTAL AMOUNT.txt

# Test configuration
/mnt/c/Insight Software/AutoBot-Enterprise/AutoBotUtilities.Tests/appsettings.json
```

### **Critical Analysis Files**
```bash
/mnt/c/Insight Software/AutoBot-Enterprise/COMPLETE-DEEPSEEK-INTEGRATION-ANALYSIS.md         # Complete pipeline analysis (REQUIRED READING)
/mnt/c/Insight Software/AutoBot-Enterprise/Claude OCR Correction Knowledge.md                # Extended knowledge base
/mnt/c/Insight Software/AutoBot-Enterprise/DEEPSEEK_OCR_TEMPLATE_CREATION_KNOWLEDGEBASE.md   # Template creation system
```

### **OCR Service Files**
```bash
# Main service files
/mnt/c/Insight Software/AutoBot-Enterprise/InvoiceReader/OCRCorrectionService/OCRCorrectionService.cs
/mnt/c/Insight Software/AutoBot-Enterprise/InvoiceReader/OCRCorrectionService/OCRErrorDetection.cs
/mnt/c/Insight Software/AutoBot-Enterprise/InvoiceReader/OCRCorrectionService/OCRPromptCreation.cs
/mnt/c/Insight Software/AutoBot-Enterprise/InvoiceReader/OCRCorrectionService/OCRDeepSeekIntegration.cs

# Pipeline infrastructure
/mnt/c/Insight Software/AutoBot-Enterprise/InvoiceReader/InvoiceReader/PipelineInfrastructure/ReadFormattedTextStep.cs

# DeepSeek API
/mnt/c/Insight Software/AutoBot-Enterprise/WaterNut.Business.Services/Utils/DeepSeek/DeepSeekInvoiceApi.cs
```

### **Application Entry Points**
```bash
/mnt/c/Insight Software/AutoBot-Enterprise/AutoBot1/Program.cs               # Console App (✅ Logging Implemented)
/mnt/c/Insight Software/AutoBot-Enterprise/WaterNut/App.xaml.cs              # WPF App (❌ No Logging)
/mnt/c/Insight Software/AutoBot-Enterprise/WCFConsoleHost/Program.cs         # WCF Service (⚠️ Basic Serilog)
```

### **Project Files**
```bash
/mnt/c/Insight Software/AutoBot-Enterprise/AutoBot1/AutoBot1.csproj
/mnt/c/Insight Software/AutoBot-Enterprise/WaterNut/AutoWaterNut.csproj
/mnt/c/Insight Software/AutoBot-Enterprise/WCFConsoleHost/AutoWaterNutServer.csproj
/mnt/c/Insight Software/AutoBot-Enterprise/AutoBotUtilities.Tests/AutoBotUtilities.Tests.csproj
```

---

## 🔍 SEARCH PATTERNS {#search-patterns}

### **OCR-Related Code**
```bash
# Search for OCR functionality
Grep pattern="OCR|DeepSeek" include="*.cs" path="/mnt/c/Insight Software/AutoBot-Enterprise/InvoiceReader"

# Find specific OCR methods
Grep pattern="DetectHeaderFieldErrors|GenerateRegexPattern" include="*.cs"
```

### **Test Files**
```bash
# Find all test files
Glob pattern="*Test*.cs" path="/mnt/c/Insight Software/AutoBot-Enterprise/AutoBotUtilities.Tests"

# Find specific test categories
Grep pattern="TestCategory.*OCR" include="*.cs"
```

### **Business Logic**
```bash
# Search for specific functionality
Grep pattern="Gift Card|TotalDeduction|MANGO" include="*.cs"

# Find database operations
Grep pattern="OCRCorrectionLearning|TemplateId" include="*.cs"
```

### **Configuration Files**
```bash
# Find configuration files
Glob pattern="*config*.json" path="/mnt/c/Insight Software/AutoBot-Enterprise"
Glob pattern="appsettings*.json" path="/mnt/c/Insight Software/AutoBot-Enterprise"
```

---

## 📊 LOG ANALYSIS {#log-analysis}

### **🚨 CRITICAL LOG ANALYSIS PROTOCOL**

#### **MANDATORY RULE**: Always Use Log Files
- **Console output truncates** and hides critical failures
- **Log files contain complete information** for diagnosis
- **Never rely on console** for test result analysis

#### **Log File Locations**
```bash
# Main log directory
ls -la "./AutoBotUtilities.Tests/bin/x64/Debug/net48/Logs/"

# Current log file
tail -100 "./AutoBotUtilities.Tests/bin/x64/Debug/net48/Logs/AutoBotTests-YYYYMMDD.log"

# Specific run logs (with RunID)
ls -la "./AutoBotUtilities.Tests/bin/x64/Debug/net48/Logs/AutoBotTests-*-Run*.log"
```

### **Log Analysis Commands**
```bash
# Read end of log file (most important)
tail -100 "./AutoBotUtilities.Tests/bin/x64/Debug/net48/Logs/AutoBotTests-YYYYMMDD.log"

# Search for completion markers
grep -A5 -B5 "TEST_RESULT\|FINAL_STATUS\|STRATEGY_COMPLETE" LogFile.log

# Search for failure indicators
grep -i "fail\|error\|exception" LogFile.log

# Search for success criteria
grep "🏆.*SUCCESS\|✅.*PASS" LogFile.log

# Find specific test phases
grep "MANGO\|TEMPLATE\|DATABASE" LogFile.log
```

### **Key Log Markers to Search For**
- `TEST_RESULT` - Final test outcome
- `FINAL_STATUS` - Overall operation result
- `STRATEGY_COMPLETE` - Database strategy execution completion
- `🏆 **OVERALL_METHOD_SUCCESS**` - Business success criteria validation
- `TEMPLATE_SPECIFICATION_VALIDATION` - Template validation results
- `OCR_CORRECTION_PIPELINE` - Pipeline execution status

### **Database Verification Commands**
```bash
# Check OCR correction results
sqlcmd -S "MINIJOE\SQLDEVELOPER2022" -U sa -P "pa\$word" -d "WebSource-AutoBot" -Q "SELECT TOP 10 * FROM OCRCorrectionLearning ORDER BY Id DESC"

# Verify template creation
sqlcmd -S "MINIJOE\SQLDEVELOPER2022" -U sa -P "pa\$word" -d "WebSource-AutoBot" -Q "SELECT * FROM [OCR-Invoices] WHERE Name LIKE '%MANGO%'"
```

---

## 🚨 CRITICAL TESTING PROTOCOLS

### **Test Execution Best Practices**
1. **Always build before testing** - Use rebuild commands above
2. **Read log files** - Never rely on console output for analysis
3. **Allow full completion** - Tests may take 5-20 minutes, don't interrupt
4. **Check database state** - Verify database operations completed successfully
5. **Use strategic logging** - Focus logging lens on specific areas when needed

### **Test Failure Analysis**
1. **Check build status** - Ensure successful compilation
2. **Read complete log file** - Search for first failure point
3. **Verify database state** - Check for incomplete database operations
4. **Check file paths** - Ensure test data files exist and are accessible
5. **Review business success criteria** - Look for `❌ FAIL` indicators in logs

### **Emergency Commands**
```bash
# Full rebuild everything
"/mnt/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" AutoBot-Enterprise.sln /t:Clean,Restore,Rebuild /p:Configuration=Debug /p:Platform=x64

# Check test DLL exists
ls -la "./AutoBotUtilities.Tests/bin/x64/Debug/net48/AutoBotUtilities.Tests.dll"

# Verify database connectivity
sqlcmd -S "MINIJOE\SQLDEVELOPER2022" -U sa -P "pa\$word" -Q "SELECT @@SERVERNAME"
```

---

## 📖 ADDITIONAL REFERENCES

**For comprehensive DeepSeek integration understanding**:
- **COMPLETE-DEEPSEEK-INTEGRATION-ANALYSIS.md** - Ultra-detailed end-to-end pipeline analysis
- **DEVELOPMENT-STANDARDS.md** - Critical mandates and logging requirements
- **ARCHITECTURE-OVERVIEW.md** - OCR service architecture and system overview

**Key Testing Notes**:
- Test data files have `.txt` extensions for OCR text content
- OCR service is split across multiple partial class files
- Always use forward slashes `/` in paths for tools
- Include spaces in quoted paths: `/mnt/c/Insight Software/AutoBot-Enterprise/`

---

*Build & Test Reference v1.0 | Complete Testing Infrastructure | Production-Ready Procedures*