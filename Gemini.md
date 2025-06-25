# Gemini.md

This file provides guidance to Gemini when working with code in this repository.

## General Principles & Known Issues

### OCR Data Processing
- **Duplicate Data:** Be aware that OCR text can contain duplicate information in different sections (e.g., `Single Column Section` vs. `SparseText Section`). Logic must be implemented to deduplicate values to avoid calculation errors (like double-counting).
- **Test Expectations:** When implementing features based on OCR, ensure tests are written to expect the *actual* results of the detection, not just zero or placeholder values. If a feature is designed to find corrections, the test should verify that the correct number and type of corrections are found.

### Amazon Invoice Processing
- A known issue involves the double-counting of "Free Shipping" amounts from Amazon invoices due to duplicate entries in the OCR text.
- The fix involves deduplicating matched values before summing them.
- Refer to `OCRErrorDetection.cs` for the implementation of `DetectAmazonSpecificErrors()` and `PDFImportTests.cs` for related tests.

## Quick Reference

### Build Commands
```bash
# WSL Build Command (working)
"/mnt/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" "AutoBotUtilities.Tests/AutoBotUtilities.Tests.csproj" /t:Rebuild /p:Configuration=Debug /p:Platform=x64
```

### Test Commands
```bash
# Run a specific, potentially long-running test (e.g., Amazon invoice)
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

## Logging Mandate & Strategy

Due to a "log and test first" development history, the codebase generates a high volume of logs. A strict logging mandate is in place to manage this and ensure logs remain useful for debugging.

### **Core Rule: Global Minimum Log Level is `Error`**
- The default logging level for the entire application is set to **`Error`**.
- Under normal operation, only critical errors are recorded.

### **The `LogLevelOverride` System: The ONLY Way to Get Detailed Logs**
- To see detailed logs (`Information`, `Verbose`, etc.) for a specific code section, you **MUST** use the `LogLevelOverride` system.
- This system temporarily increases log verbosity *only* within a specific `using` block, preventing global log pollution.

#### **Correct Usage Pattern**:
```csharp
// Use LogLevelOverride to expose a specific code section for debugging
using (LogLevelOverride.Begin(LogEventLevel.Information))
{
    // Logs within this scope are now visible at the Information level
    // This is the ONLY correct way to view non-error logs.
    ProcessSpecificCodeSection();
}
```

### **CRITICAL - Do NOT Log at `Error` Level for Visibility**
- A major historical issue is the logging of normal operational messages at the `.Error()` level simply to make them visible.
- **This is strictly forbidden.** The `Error` level is for genuine application errors only.
- Misusing `.Error()` pollutes the logs, masks real issues, and hinders troubleshooting.
- All new code must adhere to this rule, and existing inappropriate `.Error()` calls should be refactored.

### Implementation Status
- ✅ **AutoBot1**: Fully implemented with LevelOverridableLogger.
- ❌ **AutoWaterNut**: No logging configuration.
- ⚠️ **AutoWaterNutServer**: Basic Serilog implementation; needs upgrade.
- 📋 **Legacy Issues**: 67 rogue static loggers and 444+ inappropriate `.Error()` calls have been identified for refactoring.

**Note**: For comprehensive documentation, see `/mnt/c/Insight Software/AutoBot-Enterprise/Claude OCR Correction Knowledge.md` and `/mnt/c/Insight Software/AutoBot-Enterprise/Logging-Unification-Implementation-Plan.md`.
