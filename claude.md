# CLAUDE.md - AutoBot-Enterprise Configuration Guide

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## 🎯 **ENVIRONMENT AUTO-DETECTION**

**Claude Code automatically detects your environment**. This documentation works in:
- **Main Repository**: `/mnt/c/Insight Software/AutoBot-Enterprise` (master/Autobot-Enterprise.2.0 branches)
- **Alpha Worktree**: `/mnt/c/Insight Software/AutoBot-Enterprise-alpha` (debug-alpha branch)  
- **Beta Worktree**: `/mnt/c/Insight Software/AutoBot-Enterprise-beta` (debug-beta branch)
- **Any Future Worktrees**: Claude Code adapts paths automatically using `<env>` context

> **Note**: All paths below are relative to repository root. Claude Code will resolve them to your current working directory.

## Architecture Overview

AutoBot-Enterprise is a comprehensive customs broker automation system built around ASYCUDA (Automated System for Customs Data) operations. It automates customs declarations, trade document processing, inventory management, and regulatory compliance for customs brokers and importers.

### Key Business Domain
- **Primary Function**: Customs broker automation and ASYCUDA document processing
- **Core Workflow**: Email-driven document ingestion → OCR/parsing → customs declaration generation → electronic filing
- **Target Users**: Customs brokers, importers, freight forwarders dealing with international trade

## Essential Build Commands

### Building the Solution
```bash
# Build entire solution (from repository root - Claude Code auto-detects working directory)
"/mnt/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" "AutoBot-Enterprise.sln" /t:Rebuild /p:Configuration=Debug /p:Platform=x64

# Build specific project (AutoBotUtilities)
"/mnt/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" "./AutoBot/AutoBotUtilities.csproj" /t:Rebuild /p:Configuration=Debug /p:Platform=x64

# Build test project
"/mnt/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" "./AutoBotUtilities.Tests/AutoBotUtilities.Tests.csproj" /t:Rebuild /p:Configuration=Debug /p:Platform=x64
```

### Running Tests
```bash
# Run all tests
"/mnt/c/Program Files/Microsoft Visual Studio/2022/Enterprise/Common7/IDE/CommonExtensions/Microsoft/TestWindow/vstest.console.exe" "./AutoBotUtilities.Tests/bin/x64/Debug/net48/AutoBotUtilities.Tests.dll" "/Logger:console;verbosity=detailed"

# Run specific test (example: PDF import test)
"/mnt/c/Program Files/Microsoft Visual Studio/2022/Enterprise/Common7/IDE/CommonExtensions/Microsoft/TestWindow/vstest.console.exe" "./AutoBotUtilities.Tests/bin/x64/Debug/net48/AutoBotUtilities.Tests.dll" /TestCaseFilter:"FullyQualifiedName~ImportShipment" "/Logger:console;verbosity=detailed"

# 🎯 CRITICAL TEST REFERENCE: MANGO Test (OCR Service Integration)
"/mnt/c/Program Files/Microsoft Visual Studio/2022/Enterprise/Common7/IDE/CommonExtensions/Microsoft/TestWindow/vstest.console.exe" "./AutoBotUtilities.Tests/bin/x64/Debug/net48/AutoBotUtilities.Tests.dll" /TestCaseFilter:"FullyQualifiedName=AutoBotUtilities.Tests.PDFImportTests.CanImportMango03152025TotalAmount_AfterLearning" "/Logger:console;verbosity=detailed"
```

## 🚨 CRITICAL LOGGING MANDATE: ALWAYS USE LOG FILES FOR COMPLETE ANALYSIS

### **❌ CATASTROPHIC MISTAKE TO AVOID: Console Log Truncation**

**NEVER rely on console output for test analysis - it truncates and hides critical failures!**

#### **🎯 MANDATORY LOG FILE ANALYSIS PROTOCOL:**
1. **ALWAYS use log files, NEVER console output** for test result analysis
2. **Read from END of log file** to see final test results and failures  
3. **Search for specific completion markers** (TEST_RESULT, FINAL_STATUS, etc.)
4. **Verify database operation outcomes** - not just attempts
5. **Check OCRCorrectionLearning table** for Success=0 indicating failures

```bash
# Read COMPLETE log file, especially the END
tail -100 "./AutoBotUtilities.Tests/bin/x64/Debug/net48/Logs/AutoBotTests-YYYYMMDD.log"

# Search for completion markers
grep -A5 -B5 "TEST_RESULT\|FINAL_STATUS\|STRATEGY_COMPLETE" LogFile.log

# Verify database results (requires MCP server running)
sqlcmd -Q "SELECT Success FROM OCRCorrectionLearning WHERE CreatedDate >= '2025-06-29'"
```

**🚨 Key Lesson from MANGO Test:**
- Console showed: "✅ DeepSeek API calls successful"  
- **REALITY**: Database strategies ALL failed (Success=0 in OCRCorrectionLearning)
- **ROOT CAUSE**: Console logs truncated, hid the actual failure messages

**Remember: Logs tell stories, but only COMPLETE logs tell the TRUTH.**

## 🗄️ MCP SQL Server Setup (AutoBot-Enterprise Database Access)

### **Quick Start (Working Configuration)**
```powershell
# 1. Start MCP Server (Windows PowerShell)
# Note: MCP server path is fixed regardless of worktree location
cd "C:\Insight Software\AutoBot-Enterprise\mcp-servers\mssql-mcp-server"
npm start
```

### **Configuration Details**
- **Database**: `MINIJOE\SQLDEVELOPER2022` / `WebSource-AutoBot`
- **Credentials**: `sa` / `pa$word` (literal password with single $)
- **MCP Location**: `C:\Insight Software\AutoBot-Enterprise\mcp-servers\mssql-mcp-server\`
- **Claude Settings**: Already configured in `/home/joseph/.claude/settings.json`

### **Usage**
Once MCP server is running, use Claude Code queries like:
- "Show me tables in WebSource-AutoBot database"
- "Query the OCR_TemplateTableMapping table"
- "Execute SELECT * FROM [table_name] LIMIT 10"

## Core Architecture Components

### Primary Business Entities
- **AsycudaDocument**: Individual customs declarations/entries
- **AsycudaDocumentSet**: Container for grouped customs documents
- **EntryData/EntryDataDetails**: Parsed invoice and trade document data
- **InventoryItems**: Master product catalog with tariff classifications

### Key Services and Utilities
- **Utils.cs**: Main business logic orchestrator (AutoBot project)
- **DocumentUtils**: Customs document creation and management
- **PDFUtils**: Document processing and OCR integration
- **ImportUtils**: File processing workflow orchestration
- **AllocateSalesUtils**: Inventory allocation algorithms
- **ShipmentUtils**: Shipment consolidation and reporting

### Email-Driven Processing Pipeline
1. **EmailDownloader**: Automated email monitoring and attachment processing
2. **FileTypeManager**: Document type identification and classification
3. **OCR Integration**: Multiple parsing engines (InvoiceReader, DeepSeekInvoiceApi)
4. **Action-Based Processing**: Database-configured workflows with C# implementations
5. **ASYCUDA XML Generation**: Customs declaration creation and electronic filing

## Data Architecture

### Entity Framework Structure
- **CoreEntities**: Main business entities and database context
- **Multiple Domain Contexts**: Separate contexts for different business areas
  - AllocationDS/QS: Inventory allocation
  - DocumentDS/QS: Document management
  - EntryDataDS/QS: Import data processing
  - InventoryDS/QS: Product catalog
  - SalesDataQS: Sales transaction data

### Database Integration
- **SQL Server**: Primary database using Entity Framework 6.x
- **Multiple EDMX Models**: Domain-specific data models
- **TODO Views**: Database views for identifying incomplete/problematic entries
- **Action-Based Configuration**: Database-driven workflow definitions

## 🚀 AI-POWERED TEMPLATE SYSTEM - ULTRA-SIMPLE IMPLEMENTATION

### **🎯 REVOLUTIONARY APPROACH: Simple + Powerful = Success**

**Architecture**: ✅ **ULTRA-SIMPLE** - Single file implementation with advanced AI capabilities  
**Complexity**: ✅ **MINIMAL** - No external dependencies, pragmatic design  
**Functionality**: 🎯 **MAXIMUM** - Multi-provider AI, validation, recommendations, supplier intelligence

### **🏗️ SIMPLIFIED ARCHITECTURE OVERVIEW:**

```
📁 OCRCorrectionService/                    # Relative to repository root
├── AITemplateService.cs                   # SINGLE FILE - ALL FUNCTIONALITY
├── 📁 Templates/
│   ├── 📁 deepseek/                       # DeepSeek-optimized prompts
│   │   ├── header-detection.txt
│   │   └── mango-header.txt
│   ├── 📁 gemini/                         # Gemini-optimized prompts
│   │   ├── header-detection.txt  
│   │   └── mango-header.txt
│   └── 📁 default/                        # Fallback templates
│       └── header-detection.txt
├── 📁 Config/
│   ├── ai-providers.json                  # AI provider configurations
│   └── template-config.json               # Template system settings
└── 📁 Recommendations/                    # AI-generated improvements
    ├── deepseek-suggestions.json
    └── gemini-suggestions.json
```

### **✨ FEATURES DELIVERED BY SIMPLE IMPLEMENTATION:**

✅ **Multi-Provider AI Integration**: DeepSeek + Gemini + extensible  
✅ **Template Validation**: Ensures templates work before deployment  
✅ **AI-Powered Recommendations**: AIs suggest prompt improvements  
✅ **Supplier Intelligence**: MANGO gets MANGO-optimized prompts  
✅ **Provider Optimization**: Each AI gets tailored prompts  
✅ **Graceful Fallback**: Automatic fallback to hardcoded prompts  
✅ **Zero External Dependencies**: No Handlebars.NET or complex packages  
✅ **File-Based Templates**: Modify prompts without recompilation  

## OCR Correction Service Architecture - COMPLETE IMPLEMENTATION

### Main Components (Environment-Agnostic Paths)
- **Main Service**: `./OCRCorrectionService/OCRCorrectionService.cs`
- **Pipeline Methods**: `./OCRCorrectionService/OCRDatabaseUpdates.cs`
  - `GenerateRegexPatternInternal()` - Creates regex patterns using DeepSeek API
  - `ValidatePatternInternal()` - Validates generated patterns  
  - `ApplyToDatabaseInternal()` - Applies corrections to database using strategies
  - `ReimportAndValidateInternal()` - Re-imports templates after updates
  - `UpdateInvoiceDataInternal()` - Updates invoice entities
  - `CreateTemplateContextInternal()` - Creates template contexts
  - `CreateLineContextInternal()` - Creates line contexts
  - `ExecuteFullPipelineInternal()` - Orchestrates complete pipeline
  - `ExecuteBatchPipelineInternal()` - Handles batch processing

- **Error Detection**: `./OCRCorrectionService/OCRErrorDetection.cs`
  - `DetectInvoiceErrorsAsync()` - Comprehensive error detection (private)
  - `AnalyzeTextForMissingFields()` - Omission detection using AI
  - `ExtractMonetaryValue()` - Value extraction and validation
  - `ExtractFieldMetadataAsync()` - Field metadata extraction

- **Pipeline Extension Methods**: `./OCRCorrectionService/OCRCorrectionPipeline.cs`
  - Functional extension methods that call internal implementations
  - Clean API: `correction.GenerateRegexPattern(service, lineContext)`
  - All extension methods delegate to internal methods for testability
  - Complete pipeline orchestration support

- **Database Strategies**: `./OCRCorrectionService/OCRDatabaseStrategies.cs` 
  - `OmissionUpdateStrategy` - Handles missing field corrections
  - `FieldFormatUpdateStrategy` - Handles format corrections  
  - `DatabaseUpdateStrategyFactory` - Selects appropriate strategy

- **Field Mapping & Validation**: `./OCRCorrectionService/OCRFieldMapping.cs`
  - `IsFieldSupported()` - Validates supported fields (public)
  - `GetFieldValidationInfo()` - Returns field validation rules (public)
  - Caribbean customs business rule implementation

- **DeepSeek Integration**: `./OCRCorrectionService/OCRDeepSeekIntegration.cs`
  - AI-powered error detection and pattern generation
  - 95%+ confidence regex pattern creation
  - Full API integration with retry logic

## 🎛️ COMPREHENSIVE FALLBACK CONFIGURATION SYSTEM - PRODUCTION READY

### **🎉 COMPLETE SUCCESS: 90% Fallback Control System Implemented**

**Complete Implementation Delivered**: Successfully implemented comprehensive fallback configuration system that transforms OCR service architecture from silent fallback masking to controlled fail-fast behavior with comprehensive diagnostics.

#### **🎯 4-FLAG CONTROL SYSTEM**

**Production Configuration** (`fallback-config.json`):
```json
{
  "EnableLogicFallbacks": false,           // Fail-fast on missing data/corrections
  "EnableGeminiFallback": true,            // Keep LLM redundancy  
  "EnableTemplateFallback": false,         // Force template system usage
  "EnableDocumentTypeAssumption": false    // Force proper DocumentType detection
}
```

## Development Patterns

### Action-Based Architecture
The system uses a database-driven action pattern where workflows are configured in the database and mapped to C# implementations. This allows for runtime configuration of business processes without code changes.

### Multi-Tenant Support
- **ApplicationSettings**: Per-company configuration management
- **Company-specific processing**: Isolated data processing per client
- **Configurable workflows**: Different processing rules per business entity

### Error Handling and Quality Assurance
- **TODO System**: Database views identify incomplete operations
- **Automated Reporting**: Exception reports for manual review
- **Stakeholder Notifications**: Email alerts to brokers and clients
- **Comprehensive Audit Trails**: Full logging of processing actions

## Testing Strategy

### Test Categories
- **PDF Import Tests**: Document parsing accuracy validation
- **Business Logic Tests**: Customs calculations and compliance rules
- **Integration Tests**: End-to-end workflow validation
- **Data Processing Tests**: CSV/Excel import/export functionality
- **External API Tests**: Service integration validation

### Key Test Files
- **PDFImportTests.cs**: Primary document processing tests
- **AllocationsTests.cs**: Inventory allocation logic
- **CSVUtilsTests.cs**: Data import/export validation
- **DeepSeekApiTests.cs**: AI service integration

## External Dependencies and Integrations

### Document Processing
- **iText7**: PDF manipulation and generation
- **Tesseract/TesserNet**: OCR text extraction
- **NPOI**: Excel file processing
- **PdfPig**: Advanced PDF analysis

### Business Logic
- **Entity Framework 6.x**: Data access layer
- **TrackableEntities**: Change tracking for distributed systems
- **MoreLinq**: Extended LINQ functionality
- **SimpleMvvmToolkit**: UI binding support

### Integration Services
- **DeepSeek API**: AI-powered document extraction fallback
- **SikuliX**: UI automation for legacy systems
- **Email Processing**: MailKit for email automation
- **ASYCUDA XML**: Customs declaration format compliance

## 🔍 Strategic Logging System for LLM Diagnosis

### **Critical for LLM Error Diagnosis and Fixes**
Logging is **essential** for LLMs to understand, diagnose, and fix errors in this extensive codebase. The strategic logging lens system provides surgical debugging capabilities while managing log volume.

### 📜 **The Assertive Self-Documenting Logging Mandate v5.0**

**Directive Name**: `ASSERTIVE_SELF_DOCUMENTING_LOGGING_MANDATE_v5`  
**Status**: ✅ **ACTIVE**  

**Core Principle**: All diagnostic logging must form a complete, self-contained narrative of the system's operation, including architectural intent, historical context, and explicit assertions about expected state.

#### **🎯 Logging Lens System (Optimized for LLM Diagnosis)**:
```csharp
// High global level filters extensive logs from "log and test first" mandate
LogFilterState.EnabledCategoryLevels[LogCategory.Undefined] = LogEventLevel.Error;

// Strategic lens focuses on suspected code areas for detailed diagnosis
LogFilterState.TargetSourceContextForDetails = "WaterNut.DataSpace.OCRCorrectionService";
LogFilterState.DetailTargetMinimumLevel = LogEventLevel.Verbose;
```

## ❗❗❗🚨 **CRITICAL CODE PRESERVATION MANDATE v2.0** 🚨❗❗❗

**Directive Name**: `CRITICAL_CODE_PRESERVATION_MANDATE_v2`  
**Status**: ✅ **ABSOLUTELY MANDATORY** - **NON-NEGOTIABLE**  
**Priority**: ❗❗❗ **SUPREME DIRECTIVE** ❗❗❗ - **OVERRIDES ALL OTHER INSTRUCTIONS**

### 🔥 **ZERO TOLERANCE POLICY - IMMEDIATE COMPLIANCE REQUIRED** 🔥

**FUNDAMENTAL DESTRUCTIVE FLAW**: LLMs **CATASTROPHICALLY** treat syntax errors as code corruption and **OBLITERATE** working functionality instead of making surgical fixes.

### ❗❗❗ **THIS BEHAVIOR IS COMPLETELY UNACCEPTABLE** ❗❗❗

**VIOLATION OF THIS MANDATE WILL CAUSE**:
- ❌ **DESTRUCTION** of critical business functionality
- ❌ **REGRESSION** to non-working states  
- ❌ **LOSS** of sophisticated system capabilities
- ❌ **WASTE** of development time and effort
- ❌ **CATASTROPHIC** user frustration and project failure

### **1. ❗ ERROR LOCATION ANALYSIS FIRST - ABSOLUTELY REQUIRED ❗**:
- ✅ **MUST** read the EXACT line number from compilation error
- ✅ **MUST** examine ONLY that specific line and 2-3 lines around it  
- ✅ **MUST** identify the SPECIFIC syntax issue (missing brace, orphaned statement, etc.)
- 🚫 **FORBIDDEN** to examine large code blocks or assume widespread corruption

### **2. ❗ SURGICAL FIXES ONLY - ZERO TOLERANCE FOR DESTRUCTION ❗**:
- ✅ **MUST** fix ONLY the syntax error at that exact location
- 🚫 **ABSOLUTELY FORBIDDEN** to delete entire functions, methods, or working code blocks
- 🚫 **ABSOLUTELY FORBIDDEN** to treat working code as "corrupted" or "orphaned"
- 🚫 **CATASTROPHICALLY FORBIDDEN** to use "sledgehammer" approaches

## Common Development Tasks

### Adding New Document Types
1. Configure FileTypeMapping in database
2. Create parsing logic in appropriate Utils class
3. Add action mapping in FileActions dictionary
4. Create tests in AutoBotUtilities.Tests
5. Deploy configuration changes

### Extending OCR Capabilities
1. Add parsing logic to PDFUtils or create new utility class
2. Integrate with existing template system
3. Add fallback to DeepSeek API if needed
4. Create comprehensive tests with sample documents
5. Update error handling and logging

### Database Schema Changes
1. Update appropriate EDMX model
2. Regenerate entity classes using T4 templates
3. Update business service layer
4. Create migration scripts
5. Update related tests

## Important File Locations

### Core Business Logic
- `./AutoBot/Utils.cs` - Main business orchestrator
- `./AutoBot/PDFUtils.cs` - Document processing engine
- `./AutoBot/ImportUtils.cs` - File processing workflows

### Configuration
- `./CoreEntities/CoreEntities.edmx` - Main data model
- `./AutoBot/App.config` - Application configuration
- Database: ApplicationSettings table for runtime config

### Tests
- `./AutoBotUtilities.Tests/` - All test files
- `./AutoBotUtilities.Tests/Test Data/` - Sample documents for testing

### Documentation
- SQL files in root directory contain database queries and maintenance scripts
- Markdown files contain architectural documentation and workflow analysis

### OCR Correction Service Files
```bash
# All paths relative to repository root - Claude Code resolves automatically
./OCRCorrectionService/OCRCorrectionService.cs
./OCRCorrectionService/OCRErrorDetection.cs
./OCRCorrectionService/OCRPromptCreation.cs
./OCRCorrectionService/OCRDeepSeekIntegration.cs
./OCRCorrectionService/OCRCaribbeanCustomsProcessor.cs

# DeepSeek API
./WaterNut.Business.Services/Utils/DeepSeek/DeepSeekInvoiceApi.cs
```

## 🚨 CRITICAL BREAKTHROUGHS (Previous Sessions Archive)

### **DeepSeek Generalization Enhancement (June 28, 2025)** ✅
**BREAKTHROUGH**: DeepSeek was generating overly specific regex patterns for multi-field line item descriptions that only worked for single products instead of being generalizable.

**Problem Example**:
```regex
❌ OVERLY SPECIFIC: "(?<ItemDescription>Circle design ma[\\s\\S]*?xi earrings)"
   → Only works for one specific product

✅ GENERALIZED: "(?<ItemDescription>[A-Za-z\\s]+)"
   → Works for thousands of different products
```

### **ThreadAbortException Resolution (July 25, 2025)** ✅
**BREAKTHROUGH**: Persistent ThreadAbortException completely resolved using `Thread.ResetAbort()`.

**Key Discovery**: ThreadAbortException has special .NET semantics - automatically re-throws unless explicitly reset.

**Fix Pattern**:
```csharp
catch (System.Threading.ThreadAbortException threadAbortEx)
{
    context.Logger?.Warning(threadAbortEx, "🚨 ThreadAbortException caught");
    txt += "** OCR processing was interrupted - partial results may be available **\r\n";
    
    // **CRITICAL**: Reset thread abort to prevent automatic re-throw
    System.Threading.Thread.ResetAbort();
    context.Logger?.Information("✅ Thread abort reset successfully");
    
    // Don't re-throw - allow processing to continue with partial results
}
```

## Patience and Methodology
- Always complete builds fully - even if it takes 20 minutes
- Let tests run to completion - even if it takes 5+ minutes
- Trust the process - don't interrupt critical validation steps

## Development Notes

- Always use log files instead of console output for debugging (console logs truncate)
- When debugging after code changes, verify your changes didn't create the problem
- Run tests and check current log files, not old ones
- Build commands require full paths due to WSL2 environment
- Test execution requires x64 platform configuration
- The system is designed for high-volume automated processing with minimal manual intervention

## 🚀 QUICK REFERENCE FOR CLAUDE

### **🔥 MOST CRITICAL COMMANDS**

#### **MANGO Test (Primary OCR Test)**
```bash
"/mnt/c/Program Files/Microsoft Visual Studio/2022/Enterprise/Common7/IDE/CommonExtensions/Microsoft/TestWindow/vstest.console.exe" "./AutoBotUtilities.Tests/bin/x64/Debug/net48/AutoBotUtilities.Tests.dll" /TestCaseFilter:"FullyQualifiedName=AutoBotUtilities.Tests.PDFImportTests.CanImportMango03152025TotalAmount_AfterLearning" "/Logger:console;verbosity=detailed"
```

#### **Build Command (WSL)**
```bash
"/mnt/c/Program Files/Microsoft Visual Studio/2022/Enterprise/MSBuild/Current/Bin/MSBuild.exe" "AutoBotUtilities.Tests/AutoBotUtilities.Tests.csproj" /t:Rebuild /p:Configuration=Debug /p:Platform=x64
```

#### **Log Analysis (MANDATORY)**
```bash
# ALWAYS read log files, never console output
tail -100 "./AutoBotUtilities.Tests/bin/x64/Debug/net48/Logs/AutoBotTests-YYYYMMDD.log"

# Search for completion markers
grep -A5 -B5 "TEST_RESULT\|FINAL_STATUS\|STRATEGY_COMPLETE" LogFile.log
```

### **📁 CRITICAL FILE PATHS**

**Repository Root**: Auto-detected by Claude Code from `<env>` context

**Key Test Files**:
```bash
# Test configuration
./AutoBotUtilities.Tests/appsettings.json

# Test data 
./AutoBotUtilities.Tests/Test Data/
```

### **⚠️ CRITICAL MANDATES**

1. **ALWAYS USE LOG FILES** - Console output truncates and hides failures
2. **NEVER DEGRADE CODE** - Fix compilation by correcting syntax, not removing functionality  
3. **RESPECT ESTABLISHED PATTERNS** - Research existing code before creating new solutions
4. **COMPREHENSIVE LOGGING** - Every method includes business success criteria validation
5. **ENVIRONMENT AWARENESS** - Let Claude Code auto-detect paths using `<env>` context

### **🎯 SUCCESS CRITERIA FRAMEWORK**

Every method must validate these 8 dimensions:
1. 🎯 **PURPOSE_FULFILLMENT** - Method achieves stated business objective
2. 📊 **OUTPUT_COMPLETENESS** - Returns complete, well-formed data structures
3. ⚙️ **PROCESS_COMPLETION** - All required processing steps executed
4. 🔍 **DATA_QUALITY** - Output meets business rules and validation
5. 🛡️ **ERROR_HANDLING** - Appropriate error detection and recovery
6. 💼 **BUSINESS_LOGIC** - Behavior aligns with business requirements
7. 🔗 **INTEGRATION_SUCCESS** - External dependencies respond appropriately
8. ⚡ **PERFORMANCE_COMPLIANCE** - Execution within reasonable timeframes

---

*This environment-agnostic CLAUDE.md works seamlessly across all repository locations - main branch, worktrees, and future environments. Claude Code automatically adapts paths using its built-in environment awareness.*