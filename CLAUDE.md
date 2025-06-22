# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## 🚨 LATEST: DeepSeek Diagnostic Test Results (June 12, 2025)

### **✅ BREAKTHROUGH: Amazon Detection Working - Issue is Double Counting**

**Key Findings from DeepSeek Diagnostic Tests**:
- ✅ **Amazon-specific regex patterns work correctly** - Gift Card (-$6.99) and Free Shipping patterns detected
- ✅ **DeepSeek API integration functional** - Successfully making API calls and receiving responses
- ❌ **Free Shipping calculation error** - Total should be 6.99 but calculating as 13.98 (double counting)
- ❌ **Test condition error** - Test expects 0 corrections but Amazon detection finds 2 corrections

**Root Cause**: Amazon invoice text contains **duplicate Free Shipping entries** in different OCR sections:
```
Single Column Section:      SparseText Section:
Free Shipping: -$0.46      Free Shipping: -$0.46  
Free Shipping: -$6.53      Free Shipping: -$6.53
```

Current logic sums all 4 matches: `-$0.46 + -$6.53 + -$0.46 + -$6.53 = 13.98` instead of expected `6.99`.

### **IMMEDIATE FIXES NEEDED**

1. **Fix Free Shipping Deduplication** in `DetectAmazonSpecificErrors()`:
   - Add logic to detect duplicate values and sum only unique amounts
   - Current: 4 matches → 13.98 total
   - Expected: 2 unique amounts → 6.99 total

2. **Fix Test Condition** in `CanImportAmazoncomOrder11291264431163432()`:
   - Current test expects: `giftCardCorrections + freeShippingCorrections = 0`
   - Reality: Amazon detection finds 2 corrections (Gift Card + Free Shipping)
   - Test should expect corrections to be found, not zero

### **Amazon Invoice Reference Data**
```
Item(s) Subtotal: $161.95
Shipping & Handling: $6.99  
Free Shipping: -$0.46        } → TotalDeduction = 6.99 (supplier reduction)
Free Shipping: -$6.53        }
Estimated tax to be collected: $11.34
Gift Card Amount: -$6.99 → TotalInsurance = -6.99 (customer reduction, negative)
Grand Total: $166.30
```

**Expected Balanced Calculation**:
```
SubTotal (161.95) + Freight (6.99) + OtherCost (11.34) + Insurance (-6.99) - Deduction (6.99) = 166.30
InvoiceTotal (166.30) - Calculated (166.30) = TotalsZero (0.00) ✅
```

### **Files to Modify**
- **OCRErrorDetection.cs**: Fix duplicate detection in `DetectAmazonSpecificErrors()` lines 194-258
- **PDFImportTests.cs**: Update test expectations in `CanImportAmazoncomOrder11291264431163432()` line 618

### **Diagnostic Test Suite Created** ✅

**New File**: `OCRCorrectionService.DeepSeekDiagnosticTests.cs`
- ✅ Test 1: CleanTextForAnalysis preserves financial patterns  
- ✅ Test 2: Prompt generation includes Amazon data
- ✅ Test 3: Amazon-specific regex patterns work (PASSED - detected issue)
- ✅ Test 4: DeepSeek response analysis
- ✅ Test 5: Response parsing validation
- ✅ Test 6: Complete pipeline integration

**Run diagnostic tests**:
```bash
# Test Amazon regex patterns  
vstest.console.exe "AutoBotUtilities.Tests.dll" /TestCaseFilter:"DetectAmazonSpecificErrors_WithActualText_ShouldFindGiftCardAndFreeShipping"

# Test complete pipeline
vstest.console.exe "AutoBotUtilities.Tests.dll" /TestCaseFilter:"CompleteDetectionPipeline_WithAmazonData_ShouldIdentifyMissingFields"
```

## Quick Reference

### Build Commands
```bash
# WSL Build Command (working)
"/mnt/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" "AutoBotUtilities.Tests/AutoBotUtilities.Tests.csproj" /t:Rebuild /p:Configuration=Debug /p:Platform=x64
```

### Test Commands  
```bash
# Run Amazon invoice test (20 min timeout)
"/mnt/c/Program Files/Microsoft Visual Studio/2022/Enterprise/Common7/IDE/CommonExtensions/Microsoft/TestWindow/vstest.console.exe" "./AutoBotUtilities.Tests/bin/x64/Debug/net48/AutoBotUtilities.Tests.dll" /TestCaseFilter:"FullyQualifiedName=AutoBotUtilities.Tests.PDFImportTests.CanImportAmazoncomOrder11291264431163432" "/Logger:console;verbosity=detailed"

# Run diagnostic tests  
"/mnt/c/Program Files/Microsoft Visual Studio/2022/Enterprise/Common7/IDE/CommonExtensions/Microsoft/TestWindow/vstest.console.exe" "./AutoBotUtilities.Tests/bin/x64/Debug/net48/AutoBotUtilities.Tests.dll" /TestCaseFilter:"FullyQualifiedName~DeepSeekDiagnosticTests" "/Logger:console;verbosity=detailed"
```

### Key Paths
```bash
# Repository root
/mnt/c/Insight Software/AutoBot-Enterprise/

# Main Application Entry Points
/mnt/c/Insight Software/AutoBot-Enterprise/AutoBot1/Program.cs               # Console App (✅ Logging Implemented)
/mnt/c/Insight Software/AutoBot-Enterprise/WaterNut/App.xaml.cs              # WPF App (❌ No Logging)
/mnt/c/Insight Software/AutoBot-Enterprise/WCFConsoleHost/Program.cs         # WCF Service (⚠️ Basic Serilog)

# Project Files
/mnt/c/Insight Software/AutoBot-Enterprise/AutoBot1/AutoBot1.csproj
/mnt/c/Insight Software/AutoBot-Enterprise/WaterNut/AutoWaterNut.csproj
/mnt/c/Insight Software/AutoBot-Enterprise/WCFConsoleHost/AutoWaterNutServer.csproj

# Amazon test data
/mnt/c/Insight Software/AutoBot-Enterprise/AutoBotUtilities.Tests/Test Data/Amazon.com - Order 112-9126443-1163432.pdf.txt

# OCR service files
/mnt/c/Insight Software/AutoBot-Enterprise/InvoiceReader/OCRCorrectionService/

# Logging Infrastructure
/mnt/c/Insight Software/AutoBot-Enterprise/Core.Common/Core.Common/Extensions/LogLevelOverride.cs
/mnt/c/Insight Software/AutoBot-Enterprise/Logging-Unification-Implementation-Plan.md
```

## Logging Unification Status

### Current Implementation Status:
- ✅ **AutoBot1**: Fully implemented with LevelOverridableLogger and category-based filtering
- ❌ **AutoWaterNut**: WPF application with no logging configuration
- ⚠️ **AutoWaterNutServer**: Basic Serilog implementation, needs upgrade to LevelOverridableLogger
- 📋 **67 Rogue Static Loggers**: Identified across solution requiring refactoring

### Logging Strategy:
- **LogLevelOverride System**: Advanced logging with selective exposure for focused debugging
- **Global Minimum Level**: Set to Error to minimize log noise from extensive "log and test first" mandate
- **Category-Based Filtering**: LogCategory enum with runtime filtering capabilities
- **Centralized Entry Point**: Single logger creation at application entry points
- **Constructor Injection**: Logger propagated through call chains via dependency injection
- **Context Preservation**: InvocationId and structured logging maintained

#### **LogLevelOverride Usage Pattern**:
```csharp
// Global logger set to Error level minimizes vast log output
Log.Logger = new LoggerConfiguration()
    .MinimumLevel.Error()  // Only errors shown by default
    .CreateLogger();

// Use LogLevelOverride to expose specific code sections for debugging
using (LogLevelOverride.Begin(LogEventLevel.Information))
{
    // Only logs within this scope are visible at Information level
    ProcessSpecificCodeSection();
}
```

#### **Critical Issue - Inappropriate Error Logging**:
- ❌ **444 inappropriate .Error() calls** found across InvoiceReader/OCR projects
- ❌ **LLMs set logs to Error level** just to make them visible, not for actual errors
- ❌ **Normal processing appears as errors** - confuses troubleshooting
- 🔧 **Immediate Fix Needed**: OCRErrorDetection.cs (5 instances) and OCRDatabaseUpdates.cs (1 instance)

**Note**: For comprehensive documentation, architecture details, debugging methodology, and implementation status, see `/mnt/c/Insight Software/AutoBot-Enterprise/Claude OCR Correction Knowledge.md` and `/mnt/c/Insight Software/AutoBot-Enterprise/Logging-Unification-Implementation-Plan.md`.