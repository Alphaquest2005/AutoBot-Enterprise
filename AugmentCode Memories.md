# AugmentCode Memories

This document contains all the accumulated knowledge and memories from previous interactions between the AI assistant and the user, organized by category for easy reference.

## Codebase Structure and Architecture

### Email Processing Capabilities
- The codebase has email processing capabilities for reading emails, importing PDFs, and creating shipment emails
- Database files are located in 'C:\Insight Software\AutoBot-Enterprise\AutoBot1\WebSource-AutoBot Scripts' folder

### Design Principles and Preferences
- User prefers SOLID, DRY, functional C# design principles
- Files should be under 300 lines
- Loose coupling architecture preferred
- Database-first approach is the standard
- User prefers partial classes to integrate custom code with auto-generated Entity Framework code

### Service Architecture
- DeepSeekInvoiceApi class is in WaterNut.Business.Services.Utils namespace
- Generalized to remove Amazon-specific references
- The codebase follows a pattern of GetInvoiceDataErrors -> UpdateRegex -> UpdateInvoice for processing invoice discrepancies

### Business Logic
- Gift cards should be stored as positive values in TotalDeduction field but should NOT affect the InvoiceTotal calculation

## Development Environment and Compatibility

### C# Legacy Compiler Compatibility
- When using .NET Framework 4.0 C# compiler (version 4.8.9232.0):
  - Avoid string interpolation
  - Avoid async/await
  - Avoid dictionary initializers
- **Critical**: This is specifically for .NET Framework 4.0 compatibility, not newer versions

### Package Management
- Use PackageReference format, not packages.config
- To prevent RuntimeIdentifier errors: add `<RuntimeIdentifiers>win-x64;win-x64</RuntimeIdentifiers>` to all .NET Framework projects
- **NuGet Package Management**: Use PackageReference format, not packages.config

### Database Access
- User prefers SQL MCP server over file-based database access for better performance

### Critical Coding Guidelines
- Never use dotnet command or .NET 4.0 incompatible syntax
- Always use MSBuild.exe with full VS2022 paths
- Always use VSTest.Console.exe for tests

## OCR and Invoice Processing

### Invoice Correction Logic
- Invoice correction logic should validate and correct both header fields and item-level data
- When updating OCR regex patterns, use DeepSeek LLM to create regex that preserves existing functionality while adding new patterns

### Email Processing
- UpdateInvoice.UpdateRegEx processes emails with subject 'Template Template Not found!' containing OCR template creation commands

### OCR Data Structure
- SetPartLineValues object tree for OCR extraction:
  - Part (currentPart) -> Lines (collection of Line objects) -> Values (dictionary)
  - Contains actual extracted field data with Key: (section, lineNumber) and Value: Dictionary<(Fields, Instance), string>

### Enhanced Diagnostics
- Enhanced SetPartLineValues.cs with SerializeEssentialOcrData() method to capture detailed OCR extraction data
- Includes parent and child part lines and values
- Custom diagnostic logging with SerializeEssentialOcrData() method successfully identified field mapping issues through detailed JSON output analysis

### Tropical Vendors OCR Issue Analysis
- Tropical Vendors OCR template extracts only 2 items instead of expected 66+ items due to field mapping issue, not parent-child relationships
- Tropical Vendors Header Part (PartId: 2493) has 5 lines with Line 4 (LineId: 2147) containing 81 InvoiceDate values instead of product fields (ItemNumber, ItemDescription, Cost, Quantity)
- Tropical Vendors Child Details Part (PartId: 2494) has 0 lines and 0 values

### Template Comparison Analysis
- Both Amazon and Tropical Vendors templates work without child parts - parent-child relationship theory was incorrect
- Amazon template (PartId: 1028) works correctly with ChildPartsCount: 0
- Amazon Line 4 (LineId: 78) contains 54 product field instances with correct fields: Quantity, ItemDescription, Cost, producing 9 final InvoiceDetails
- The 2 InvoiceDetails in Tropical Vendors final output come from different lines in header part that have correct product fields, not from the problematic Line 4

### Root Cause and Solution
- OCR_PARENT_CHILD_ANALYSIS_REPORT.md contains complete detailed analysis comparing Amazon vs Tropical Vendors with evidence of field mapping issue
- Solution requires updating Tropical Vendors Line 4 field mappings in OCR database to extract ItemNumber, ItemDescription, Cost, Quantity instead of InvoiceDate
- Database investigation needed: Compare field mappings between Amazon Line 4 (ID: 78) and Tropical Vendors Line 4 (ID: 2147) to fix extraction issue
- Paradigm shift achieved: OCR extraction issue is field mapping configuration in database, not parent-child relationships or regex pattern problems

## Build and Testing

### Build Configuration
- AutoBot-Enterprise build: Use MSBuild.exe from VS2022 Enterprise with /t:Clean,Restore,Rebuild /p:Configuration=Debug /p:Platform=x64

### Testing Framework and Location
- Tests should use NUnit framework
- Tests should be placed in C:\Insight Software\AutoBotUtilities.Tests\AutoBotUtilities.Tests.csproj
- User prefers simple integration tests with good logging over complex unit test suites

### Test Execution
- Test execution commands: Use "C:\Program Files\Microsoft Visual Studio\2022\Enterprise\Common7\IDE\Extensions\TestPlatform\vstest.console.exe"
- Use specific test names and /logger:console for detailed output

## Logging Configuration

### Logging Architecture
- User prefers centralized logging systems with dependency injection pattern
- LogLevelOverride functionality available
- LogFilterState.EnabledCategoryLevels configuration

### LogCategory Enum Values
- LogCategory.MethodBoundary
- LogCategory.InternalStep
- LogCategory.DiagnosticDetail
- LogCategory.Performance
- LogCategory.Undefined

### Logger Cleanup Pattern
- Test entry points use .ForContext<TestClass>()
- Application components receive logger via constructor or use context.Logger

### LogLevelOverride Usage
- When using LogLevelOverride in methods, ensure proper using block structure at method start
- Close before catch blocks
- LogLevelOverride system is available in Core.Common.Extensions namespace
- Begin() method returns IDisposable for proper using block structure

### Critical Debugging Directive
- **CRITICAL DIRECTIVE**: Before fixing any bug, the solution must be directly supported by the logs so no assumptions are made
- Logs must confirm diagnostic assumptions before implementing solutions

## Historical Context and Lessons Learned

### Failed Approaches and Paradigm Shifts
- **Initial Hypothesis**: OCR extraction issues were due to parent-child relationship problems between Parts
- **Reality Discovered**: OCR extraction issue is field mapping configuration in database, not parent-child relationships or regex pattern problems
- **Key Learning**: Always investigate field mappings in OCR database before assuming structural issues

### Debugging Journey Chronology
1. **Phase 1**: Suspected parent-child relationship issues in OCR templates
2. **Phase 2**: Enhanced diagnostic logging with SerializeEssentialOcrData() method
3. **Phase 3**: Detailed analysis comparing Amazon vs Tropical Vendors templates
4. **Phase 4**: Discovery that both templates work without child parts
5. **Phase 5**: Identification of field mapping issue in Line 4 of Tropical Vendors template
6. **Paradigm Shift**: Root cause is database field configuration, not code structure

### Evidence-Based Analysis
- Amazon Line 4 (LineId: 78) correctly maps to product fields: Quantity, ItemDescription, Cost
- Tropical Vendors Line 4 (LineId: 2147) incorrectly maps to InvoiceDate field (81 instances)
- This field mapping difference explains why Amazon extracts 9 InvoiceDetails vs Tropical Vendors' 2
- OCR_PARENT_CHILD_ANALYSIS_REPORT.md documents complete evidence trail

## Configuration Details

### API Keys and External Services
- User's OpenRouter API key: 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

### Email Server Configuration
- Email server: documents.websource@auto-brokerage.com
- Password: 'WebSource'
- IMAP server: mail.auto-brokerage.com:993
- SMTP server: mail.auto-brokerage.com:465

### Database Configuration
- Database: SQL Server on MINIJOE\SQLDEVELOPER2022
- Database name: 'WebSource-AutoBot'
- Username: 'sa'
- Password: 'pa$word'
- **Important**: AutoBrokerage-AutoBot database should only be used as a reference for comparison and never modified

## Development Workflow and Best Practices

### Code Review and Quality Standards
- Always use codebase-retrieval tool before making edits to understand existing code structure
- Respect existing codebase patterns and architecture
- Make conservative changes that align with established patterns

### Testing Philosophy
- Prefer simple integration tests with good logging over complex unit test suites
- Write tests to validate code changes and ensure functionality works as expected
- Use detailed logging in tests for better debugging capabilities

### Error Handling and Diagnostics
- Implement comprehensive logging at key decision points
- Use LogLevelOverride for detailed debugging when needed
- Always verify assumptions with logs before implementing solutions
- Document debugging discoveries for future reference

### Database Investigation Methodology
- Compare working vs non-working configurations to identify differences
- Use detailed diagnostic logging to capture data flow
- Focus on field mappings and configuration rather than assuming code issues
- Document evidence trail for complex debugging scenarios

---

*This document serves as a comprehensive reference for all accumulated knowledge and should be updated as new insights and configurations are discovered.*
