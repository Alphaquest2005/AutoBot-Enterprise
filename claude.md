# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Architecture Overview

AutoBot-Enterprise is a comprehensive customs broker automation system built around ASYCUDA (Automated System for Customs Data) operations. It automates customs declarations, trade document processing, inventory management, and regulatory compliance for customs brokers and importers.

### Key Business Domain
- **Primary Function**: Customs broker automation and ASYCUDA document processing
- **Core Workflow**: Email-driven document ingestion → OCR/parsing → customs declaration generation → electronic filing
- **Target Users**: Customs brokers, importers, freight forwarders dealing with international trade

## Essential Build Commands

### Building the Solution
```bash
# Build entire solution (from repository root)
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

# Run Mango test (key reference test)
"/mnt/c/Program Files/Microsoft Visual Studio/2022/Enterprise/Common7/IDE/CommonExtensions/Microsoft/TestWindow/vstest.console.exe" "./AutoBotUtilities.Tests/bin/x64/Debug/net48/AutoBotUtilities.Tests.dll" /TestCaseFilter:"FullyQualifiedName=AutoBotUtilities.Tests.PDFImportTests.CanImportMango03152025TotalAmount_AfterLearning" "/Logger:console;verbosity=detailed"
```

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
- `AutoBot/Utils.cs` - Main business orchestrator
- `AutoBot/PDFUtils.cs` - Document processing engine
- `AutoBot/ImportUtils.cs` - File processing workflows

### Configuration
- `CoreEntities/CoreEntities.edmx` - Main data model
- `AutoBot/App.config` - Application configuration
- Database: ApplicationSettings table for runtime config

### Tests
- `AutoBotUtilities.Tests/` - All test files
- `AutoBotUtilities.Tests/Test Data/` - Sample documents for testing

### Documentation
- SQL files in root directory contain database queries and maintenance scripts
- Markdown files contain architectural documentation and workflow analysis

## Development Notes

- Always use log files instead of console output for debugging (console logs truncate)
- When debugging after code changes, verify your changes didn't create the problem
- Run tests and check current log files, not old ones
- Build commands require full paths due to WSL2 environment
- Test execution requires x64 platform configuration
- The system is designed for high-volume automated processing with minimal manual intervention