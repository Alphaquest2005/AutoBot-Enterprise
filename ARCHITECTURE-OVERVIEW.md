# ARCHITECTURE OVERVIEW - AutoBot-Enterprise

> **🏗️ Complete System Architecture** - High-level overview and OCR service implementation details

## 🏠 WORKTREE ENVIRONMENT DETECTION

### **🎯 Current Environment Commands**
```bash
# Always run this first to identify your current environment
echo "Current Environment: $(pwd)"
echo "Current Branch: $(git branch --show-current)"
echo "Repository Root: $(git rev-parse --show-toplevel)"
git worktree list
```

**Available Environments**:
- **Main Repository**: `AutoBot-Enterprise` (primary development)
- **Alpha Worktree**: `AutoBot-Enterprise-alpha` (experimental work)
- **Beta Worktree**: `AutoBot-Enterprise-beta` (baseline comparison)

### **🔄 Environment Navigation**
```bash
# Navigate between environments (adjust paths as needed)
cd "../AutoBot-Enterprise"        # Main environment
cd "../AutoBot-Enterprise-alpha"  # Alpha environment  
cd "../AutoBot-Enterprise-beta"   # Beta environment
```

---

## 📋 TABLE OF CONTENTS

1. [**🎯 System Overview**](#system-overview) - High-level architecture and core workflow
2. [**🔧 Key Components**](#key-components) - Main application components and libraries
3. [**🚀 OCR Correction Service Architecture**](#ocr-correction-service-architecture) - Complete implementation details
4. [**🔗 Integration Points**](#integration-points) - How components interact
5. [**📊 Data Flow**](#data-flow) - End-to-end processing pipeline
6. [**🏷️ Template System**](#template-system) - OCR template and context management
7. [**🧪 Testing Architecture**](#testing-architecture) - Test coverage and validation

---

## 🎯 System Overview {#system-overview}

AutoBot-Enterprise is a .NET Framework 4.8 application that automates customs document processing workflows. The system processes emails, PDFs, and various file formats to extract data and manage customs-related documents (Asycuda).

### **Core Technology Stack**
- **.NET Framework 4.8** - Primary application framework
- **Entity Framework** - Database ORM for SQL Server
- **Serilog** - Advanced logging with strategic lens system
- **Tesseract OCR** - Traditional OCR text extraction
- **DeepSeek AI API** - Advanced AI-powered error detection and pattern generation
- **IMAP/Email Processing** - Automated email attachment extraction
- **SQL Server** - Primary database (`WebSource-AutoBot`)

### **Primary Use Case**
Automated customs document processing for Caribbean customs operations, including:
- Email-based document ingestion
- PDF invoice data extraction and validation
- Template-based field mapping and regex pattern application
- AI-powered error detection and correction
- Asycuda document generation for customs submissions

---

## 🔧 Key Components {#key-components}

### **Main Application Entry Points**

#### **AutoBot1 (Console Application)**
- **File**: `./AutoBot1/Program.cs` (relative to repository root)
- **Status**: ✅ Logging Implemented
- **Purpose**: Main processing loop for automated document workflows
- **Functionality**: Processes emails and executes database sessions based on `ApplicationSettings`

#### **WaterNut (WPF Application)**
- **File**: `./WaterNut/App.xaml.cs` (relative to repository root)
- **Status**: ❌ No Logging
- **Purpose**: User interface for manual document processing and system configuration

#### **WCFConsoleHost (WCF Service)**
- **File**: `./WCFConsoleHost/Program.cs` (relative to repository root)
- **Status**: ⚠️ Basic Serilog
- **Purpose**: Web service endpoints for external system integration

### **Core Workflow Engine**

AutoBot-Enterprise follows a configurable action-based processing model:

1. **Email Processing**: Downloads emails via IMAP, extracts attachments, applies regex-based rules
2. **PDF Processing**: Extracts invoice data using OCR (Tesseract), pattern matching, or DeepSeek API
3. **Database Actions**: Executes configurable actions stored in database tables
4. **Document Management**: Creates and manages Asycuda documents for customs processing

### **Core Libraries**

#### **AutoBotUtilities** - Main utility library containing:
- **`FileUtils.cs`** - Static dictionary `FileActions` mapping action names to C# methods
- **`SessionsUtils.cs`** - Static dictionary `SessionActions` for scheduled/triggered actions
- **`ImportUtils.cs`** - Orchestrates execution of database-defined actions
- **`PDFUtils.cs`** - PDF import and processing orchestration

#### **WaterNut.Business.Services** - Business logic layer:
- **DeepSeek API Integration** - AI-powered invoice analysis and pattern generation
- **Database Services** - Entity Framework-based data access
- **Email Processing Services** - IMAP-based email and attachment handling

#### **InvoiceReader** - OCR and document processing:
- **OCR Correction Service** - Advanced AI-powered error detection and correction
- **Pipeline Infrastructure** - Configurable processing pipelines
- **Template Management** - Dynamic template creation and validation

---

## 🚀 OCR Correction Service Architecture {#ocr-correction-service-architecture}

### **🎯 Complete Implementation Status** ✅

The OCR Correction Service is a comprehensive AI-powered system for detecting and correcting OCR errors in invoice processing. **ALL PIPELINE METHODS IMPLEMENTED** with complete functional pipeline.

### **Main Service Components**

#### **🔧 Core Service** - `OCRCorrectionService.cs`
- **Location**: `./InvoiceReader/OCRCorrectionService/OCRCorrectionService.cs` (relative to repository root)
- **Purpose**: Main orchestration service with dependency injection and configuration management
- **Key Features**: Constructor injection, fallback configuration, template context management

#### **⚙️ Pipeline Methods** - `OCRDatabaseUpdates.cs`
- **Location**: `./InvoiceReader/OCRCorrectionService/OCRDatabaseUpdates.cs` (relative to repository root)
- **Implementation**: Complete functional pipeline with all methods implemented

**Key Pipeline Methods**:
- `GenerateRegexPatternInternal()` - Creates regex patterns using DeepSeek API
- `ValidatePatternInternal()` - Validates generated patterns with comprehensive testing
- `ApplyToDatabaseInternal()` - Applies corrections to database using strategic patterns
- `ReimportAndValidateInternal()` - Re-imports templates after updates with validation
- `UpdateInvoiceDataInternal()` - Updates invoice entities with corrected data
- `CreateTemplateContextInternal()` - Creates template contexts for new suppliers
- `CreateLineContextInternal()` - Creates line contexts for field mapping
- `ExecuteFullPipelineInternal()` - Orchestrates complete end-to-end pipeline
- `ExecuteBatchPipelineInternal()` - Handles batch processing with error recovery

#### **🔍 Error Detection** - `OCRErrorDetection.cs`
- **Location**: `./InvoiceReader/OCRCorrectionService/OCRErrorDetection.cs` (relative to repository root)
- **Implementation**: AI-powered comprehensive error detection system

**Key Detection Methods**:
- `DetectInvoiceErrorsAsync()` - Comprehensive error detection (private implementation)
- `AnalyzeTextForMissingFields()` - Omission detection using AI analysis
- `ExtractMonetaryValue()` - Value extraction and validation with business rules
- `ExtractFieldMetadataAsync()` - Field metadata extraction for template creation

#### **📝 Prompt Creation** - `OCRPromptCreation.cs`
- **Location**: `./InvoiceReader/OCRCorrectionService/OCRPromptCreation.cs` (relative to repository root)
- **Implementation**: Advanced AI prompt generation with Phase 2 v2.0 Enhanced Emphasis Strategy
- **Features**: Multi-provider optimization (DeepSeek, Gemini), generalization requirements, supplier-specific intelligence

#### **🔗 Pipeline Extension Methods** - `OCRCorrectionPipeline.cs`
- **Location**: `./InvoiceReader/OCRCorrectionService/OCRCorrectionPipeline.cs` (relative to repository root)
- **Implementation**: Functional extension methods that call internal implementations
- **Design**: Clean API (`correction.GenerateRegexPattern(service, lineContext)`) with internal method delegation for testability

#### **💾 Database Strategies** - `OCRDatabaseStrategies.cs`
- **Location**: `./InvoiceReader/OCRCorrectionService/OCRDatabaseStrategies.cs` (relative to repository root)
- **Implementation**: Strategy pattern for different correction types

**Database Update Strategies**:
- `OmissionUpdateStrategy` - Handles missing field corrections with template creation
- `FieldFormatUpdateStrategy` - Handles format corrections with regex pattern updates
- `DatabaseUpdateStrategyFactory` - Selects appropriate strategy based on error type

#### **🗺️ Field Mapping & Validation** - `OCRFieldMapping.cs`
- **Location**: `./InvoiceReader/OCRCorrectionService/OCRFieldMapping.cs` (relative to repository root)
- **Implementation**: Business rules and field validation system

**Key Validation Methods**:
- `IsFieldSupported()` - Validates supported fields (public API)
- `GetFieldValidationInfo()` - Returns field validation rules (public API)
- **Caribbean customs business rule implementation** - TotalInsurance vs TotalDeduction mapping

#### **🤖 DeepSeek Integration** - `OCRDeepSeekIntegration.cs`
- **Location**: `./InvoiceReader/OCRCorrectionService/OCRDeepSeekIntegration.cs` (relative to repository root)
- **Implementation**: Complete AI API integration with retry logic

**AI Integration Features**:
- AI-powered error detection and pattern generation
- 95%+ confidence regex pattern creation
- Full API integration with exponential backoff retry logic
- Timeout management (10 minutes for complex multi-field processing)

### **🔗 Template Context Integration** ✅

#### **Real Template Context Captured**
- **File**: `template_context_amazon.json` contains actual database IDs
- **Amazon Template Data**:
  - InvoiceId: 5 (Amazon template)
  - Real LineIds: 1830 (Gift Card), 1831 (Free Shipping)
  - Real RegexIds: 2030, 2031 with existing patterns
  - Real FieldIds: 2579, 2580 with correct field mappings

#### **Template Creation Process**
1. **Unknown Supplier Detection** - System detects new supplier (e.g., MANGO)
2. **AI-Powered Template Creation** - DeepSeek API generates field mappings and regex patterns
3. **Database Persistence** - Template saved with proper structure and relationships
4. **Validation** - Template validated against business rules and field requirements
5. **Integration** - Template integrated with existing pipeline for immediate use

---

## 🔗 Integration Points {#integration-points}

### **OCR Pipeline Entry Point** ✅

#### **ReadFormattedTextStep Integration**
- **Location**: `./InvoiceReader/InvoiceReader/PipelineInfrastructure/ReadFormattedTextStep.cs` (relative to repository root)
- **Integration**: Complete OCR correction pipeline integrated
- **Trigger**: TotalsZero calculation triggers OCR correction automatically
- **Process**: Uses `ExecuteFullPipelineForInvoiceAsync()` for invoice processing
- **Validation**: Template context creation and validation

### **Database Integration**
- **Primary Database**: `MINIJOE\SQLDEVELOPER2022` / `WebSource-AutoBot`
- **OCR Tables**: `OCR-Invoices`, `OCR-Parts`, `OCR-Lines`, `OCR-Fields`, `OCR-RegularExpressions`
- **Learning System**: `OCRCorrectionLearning` table for AI improvement tracking
- **Template Mapping**: Dynamic template creation and field mapping

### **AI Service Integration**
- **DeepSeek API**: Primary AI service for error detection and pattern generation
- **Gemini Fallback**: Secondary AI service for redundancy (configurable)
- **API Management**: Timeout handling, retry logic, and error recovery

---

## 📊 Data Flow {#data-flow}

### **End-to-End Processing Pipeline**

1. **Email Ingestion**
   - IMAP email download
   - Attachment extraction
   - File type validation

2. **PDF Processing**
   - Tesseract OCR text extraction
   - Text preprocessing and cleaning
   - Initial field detection

3. **OCR Correction**
   - TotalsZero calculation check
   - AI-powered error detection
   - Template lookup or creation
   - Regex pattern application

4. **Database Operations**
   - Template persistence
   - Field mapping updates
   - Learning data capture

5. **Document Generation**
   - Asycuda document creation
   - Export formatting
   - Customs submission preparation

### **OCR Correction Data Flow**

```
Invoice Text → Error Detection → Template Lookup → Pattern Generation → Database Update → Validation → Invoice Creation
     ↓              ↓                ↓                    ↓                  ↓             ↓            ↓
Tesseract OCR → DeepSeek AI → Template DB → AI Regex → Strategy Pattern → Validation → Success/Fail
```

---

## 🏷️ Template System {#template-system}

### **Template Hierarchy**
- **Invoice Templates** (`OCR-Invoices`) - Top-level template containers
- **Parts** (`OCR-Parts`) - Sections within templates (Header, LineItems, Footer)
- **Lines** (`OCR-Lines`) - Individual field groupings with regex patterns
- **Fields** (`OCR-Fields`) - Specific data fields with validation rules

### **Dynamic Template Creation**
1. **Supplier Detection** - Unknown supplier triggers template creation
2. **AI Analysis** - DeepSeek analyzes invoice structure and content
3. **Pattern Generation** - AI generates regex patterns for field extraction
4. **Validation** - Patterns tested against business rules and requirements
5. **Persistence** - Template saved with full relational structure

### **Template Context Management**
- **Context Creation** - Dynamic context generation for new suppliers
- **Field Mapping** - Automatic mapping of business fields to OCR fields
- **Pattern Optimization** - AI-optimized patterns for maximum accuracy
- **Version Control** - Template versioning and update tracking

---

## 🧪 Testing Architecture {#testing-architecture}

### **Comprehensive Test Coverage** ✅

#### **Simple Pipeline Tests** - `OCRCorrectionService.SimplePipelineTests.cs` (5/5 tests passing)
- DeepSeek integration validation
- Pattern validation testing
- Field support validation
- TotalsZero calculation testing
- Template context creation

#### **Database Pipeline Tests** - `OCRCorrectionService.DatabaseUpdatePipelineTests.cs`
- **Real Template Context**: Uses actual Amazon template metadata (InvoiceId: 5)
- **Database IDs**: Actual database IDs for Gift Card and Free Shipping patterns
- **End-to-End Testing**: Complete pipeline testing with existing methods
- **Strategy Validation**: Database update application testing

#### **Diagnostic Test Suite** - `OCRCorrectionService.DeepSeekDiagnosticTests.cs` ✅
- ✅ **Test 1**: CleanTextForAnalysis preserves financial patterns  
- ✅ **Test 2**: Prompt generation includes Amazon data
- ✅ **Test 3**: Amazon-specific regex patterns work (PASSED - detected issue)
- ✅ **Test 4**: DeepSeek response analysis
- ✅ **Test 5**: Response parsing validation
- ✅ **Test 6**: Complete pipeline integration

### **Key Test Scenarios**
- **MANGO Integration Test** - End-to-end template creation and invoice processing
- **Amazon Detection Test** - Existing template usage and field extraction
- **DeepSeek Diagnostic Tests** - AI service integration and response validation
- **Generic PDF Test Suite** - Comprehensive testing with strategic logging

---

## 🔍 Critical Implementation Notes

### **Architecture Principles**
- **Fail-Fast Design** - 90% complete fallback configuration system with controlled termination
- **Clean API Pattern** - Extension methods provide clean API while internal methods enable testability
- **Strategy Pattern** - Database update strategies handle all correction types (omission, format, validation)
- **Retry Logic** - Pipeline supports exponential backoff for robustness
- **Real Data Integration** - Uses actual template context data, no synthetic test data required

### **Caribbean Customs Business Rules**
- **TotalInsurance vs TotalDeduction** - Proper mapping for customs calculations
- **Field Validation** - Business rule compliance for customs requirements
- **Document Type Detection** - Automatic classification for processing workflows

### **AI Integration Standards**
- **95%+ Confidence Threshold** - High-confidence regex pattern generation
- **Generalization Requirements** - Patterns work for thousands of products, not single items
- **Multi-Provider Support** - DeepSeek primary, Gemini fallback, extensible architecture
- **Timeout Management** - 10-minute timeouts for complex processing

---

## 📖 ADDITIONAL REFERENCES

**For detailed technical analysis**:
- **COMPLETE-DEEPSEEK-INTEGRATION-ANALYSIS.md** - Ultra-detailed end-to-end pipeline analysis
- **DEVELOPMENT-STANDARDS.md** - Critical mandates and logging requirements
- **BUILD-AND-TEST.md** - Complete build and test procedures

**For specialized topics**:
- **AI-TEMPLATE-SYSTEM.md** - Latest AI-powered template implementation
- **DATABASE-AND-MCP.md** - Database configuration and MCP setup
- **LOGGING-DIAGNOSTICS.md** - Strategic logging system for LLM diagnosis

---

## 🏆 Verification Status ✅

**All architecture components verified as working**:
- ✅ MSBuild.exe path exists and functional
- ✅ vstest.console.exe path exists and functional
- ✅ Repository root path accessible
- ✅ Solution file (AutoBot-Enterprise.sln) exists and builds
- ✅ All specified test data files exist and accessible
- ✅ All OCR correction service files exist and compile
- ✅ Pipeline infrastructure files exist and integrate properly
- ✅ DeepSeek API integration working with proper authentication
- ✅ OCR correction pipeline methods implemented and tested
- ✅ Database update strategies implemented and validated
- ✅ Template creation system operational and tested

**Last verified**: Current session (2025-08-02)

---

*Architecture Overview v1.0 | Complete System Documentation | Production-Ready Implementation*