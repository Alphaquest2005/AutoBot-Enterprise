# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Build Commands

```powershell
# Full solution build (x64 platform required)
& "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe" AutoBot-Enterprise.sln /t:Clean,Restore,Rebuild /p:Configuration=Debug /p:Platform=x64

# Build specific project (e.g., tests)
& "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\MSBuild\Current\Bin\MSBuild.exe" "AutoBotUtilities.Tests\AutoBotUtilities.Tests.csproj" /t:Clean,Restore,Rebuild /p:Configuration=Debug /p:Platform=x64

# WSL Build Command (working build command for tests)
"/mnt/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" "AutoBotUtilities.Tests/AutoBotUtilities.Tests.csproj" /t:Rebuild /p:Configuration=Debug /p:Platform=x64
```

## Test Commands

```powershell
# Run all tests
& "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" ".\AutoBotUtilities.Tests\bin\x64\Debug\net48\AutoBotUtilities.Tests.dll" "/Logger:console;verbosity=detailed"

# Run specific test
& "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" ".\AutoBotUtilities.Tests\bin\x64\Debug\net48\AutoBotUtilities.Tests.dll" /TestCaseFilter:"FullyQualifiedName=AutoBotUtilities.Tests.PDFImportTests.CanImportAmazonMultiSectionInvoice_WithLogging" "/Logger:console;verbosity=detailed"

# Run tests in a class
& "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\Common7\IDE\CommonExtensions\Microsoft\TestWindow\vstest.console.exe" ".\AutoBotUtilities.Tests\bin\x64\Debug\net48\AutoBotUtilities.Tests.dll" /TestCaseFilter:"FullyQualifiedName~PDFImportTests" "/Logger:console;verbosity=detailed"
```

## High-Level Architecture

AutoBot-Enterprise is a .NET Framework 4.8 application that automates customs document processing workflows. The system processes emails, PDFs, and various file formats to extract data and manage customs-related documents (Asycuda).

### Core Workflow
1. **Email Processing**: Downloads emails via IMAP, extracts attachments, applies regex-based rules
2. **PDF Processing**: Extracts invoice data using OCR (Tesseract), pattern matching, or DeepSeek API
3. **Database Actions**: Executes configurable actions stored in database tables
4. **Document Management**: Creates and manages Asycuda documents for customs processing

### Key Components

#### Main Entry Point
- `AutoBot1/Program.cs` - Console application that runs the main processing loop
- Processes emails and executes database sessions based on `ApplicationSettings`

#### Core Libraries
- `AutoBotUtilities` - Main utility library containing:
  - `FileUtils.cs` - Static dictionary `FileActions` mapping action names to C# methods
  - `SessionsUtils.cs` - Static dictionary `SessionActions` for scheduled/triggered actions
  - `ImportUtils.cs` - Orchestrates execution of database-defined actions
  - `PDFUtils.cs` - PDF import and processing orchestration
  - Various utility classes for specific document types (EX9, ADJ, DIS, LIC, C71, PO)

#### Data Access
- Entity Framework 6 with EDMX models split across multiple contexts:
  - `CoreEntitiesContext` - Settings, FileTypes, Emails, Attachments
  - `DocumentDSContext` - Asycuda documents and related entities
  - `EntryDataDSContext` - Suppliers, Shipments, Invoices
  - `AllocationDSContext` - Inventory and allocation management

#### PDF/OCR Processing
- `InvoiceReader` - Core PDF data extraction with configurable patterns
- OCR configuration stored in database tables (`OCR_Parts`, `OCR_RegularExpressions`)
- Supports recurring sections and parent-child relationships in documents
- Falls back to DeepSeek API for complex documents

### Database-Driven Configuration
The system is highly configurable through database tables:
- `FileTypes` - Defines document types and processing rules
- `Actions` - C# method names to execute
- `FileTypeActions` - Maps FileTypes to Actions with execution order
- `EmailMapping` - Rules for processing specific email patterns
- `SessionSchedule` - Scheduled or triggered database actions

### Action Execution Pattern
1. Database stores action names as strings
2. `FileUtils.FileActions` and `SessionsUtils.SessionActions` map these to C# methods
3. `ImportUtils` looks up and executes actions based on context
4. Actions can be immediate or deferred, data-specific or general

### Testing Considerations
- Uses NUnit 4.3.2 with custom Serilog logging
- Test configuration in `AutoBotUtilities.Tests/appsettings.json`
- Integration tests require database setup
- Some tests depend on external services (email, APIs)
- NCrunch configuration exists but parallel execution is disabled

### Important Notes
- All projects must target x64 platform
- Requires SQL Server 2019+ for database
- External dependencies: IMAP email server, DeepSeek API, SikuliX for UI automation
- Static utility classes are prevalent - consider dependency injection when refactoring
- Database actions are dynamically mapped - verify action names match C# methods when debugging

## Evidence-Based Debugging Methodology

This codebase follows a strict **evidence-based debugging approach** where all diagnostic assumptions must be directly supported by logs before any code changes are made.

### Logging Strategy
1. **Global Serilog Level**: Set to `Error` or `Fatal` to suppress most logs
2. **Targeted Debugging**: Use `LogLevelOverride.Begin(LogEventLevel.Verbose)` in specific code sections
3. **Scoped Logging**: Wrap debugging code in `using` blocks to see only relevant logs

### Critical Debugging Pattern
```csharp
// Example of evidence-based debugging approach
using (LogLevelOverride.Begin(LogEventLevel.Verbose))
{
    _logger.Error("CRITICAL_DEBUG: Method entry with parameters: {@params}", parameters);
    
    // Your debugging code here
    var result = ProcessData(parameters);
    
    _logger.Error("CRITICAL_DEBUG: Method exit with result: {@result}", result);
    
    return result;
}
```

### Key Principles
- **Never assume root causes** - logs must confirm diagnostic hypotheses
- **Add comprehensive logging** before implementing solutions  
- **Use LogLevelOverride** to isolate specific method/section logging
- **Evidence first, then fix** - no code changes without log confirmation
- **Minimal scope changes** - wrap only the code of interest in LogLevelOverride

## Critical Architecture Details

### String-Based Action Mapping System
The core of AutoBot-Enterprise is a **runtime action resolution system** where database tables store C# method names as strings:

```csharp
// FileUtils.FileActions maps 126+ action names to methods
var actionName = "ImportPDF"; // Stored in database Actions table
var method = FileUtils.FileActions[actionName]; // Runtime resolution
await method.Invoke(logger, fileType, files);
```

**Critical debugging points:**
- Action name mismatches cause `KeyNotFoundException` at runtime
- Verify action names in database exactly match dictionary keys
- Use `FileUtils.FileActions.Keys` to see all available actions
- Check `ImportUtils.cs` orchestration logic for action execution flow

### Main Processing Loop Architecture
1. **Program.cs** loads `ApplicationSettings` with all related database mappings
2. **For each active setting:** Process emails → Execute file actions → Execute session actions
3. **ImportUtils** orchestrates three action types:
   - `ExecuteEmailMappingActions()` - Email pattern-based actions
   - `ExecuteDataSpecificFileActions()` - File processing actions  
   - `ExecuteNonSpecificFileActions()` - General scheduled actions

### Database Context Usage
- **CoreEntitiesContext** - Configuration (FileTypes, Actions, Emails, ApplicationSettings)
- **DocumentDSContext** - Asycuda documents and customs processing
- **EntryDataDSContext** - Suppliers, Shipments, Invoices from PDF processing
- **AllocationDSContext** - Inventory management and allocations

**Debugging tip:** Always check which context is being used when investigating data access issues