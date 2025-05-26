# STANDARD OPERATING PROCEDURE (SOP)

## 🚨 CRITICAL LESSONS LEARNED - ALWAYS FOLLOW

### 1. PowerShell Command Syntax
- ✅ USE: `;` for command chaining in PowerShell
- ❌ NEVER USE: `&&` (that's bash syntax)
- ✅ EXAMPLE: `cd directory; npm install`

### 2. Directory Navigation
- ✅ ALWAYS check current working directory with `pwd` or `Get-Location`
- ✅ Use proper PowerShell navigation: `cd "path with spaces"`
- ✅ Remember supervisor messages about directory changes
- ❌ NEVER assume you're in the right directory

### 3. File Path Handling
- ✅ Use full paths when uncertain: `"C:\Full\Path\To\File"`
- ✅ Check file existence before running: `Test-Path filename`
- ✅ Use proper escaping for special characters in passwords

### 4. Database Connection Patterns
- ✅ WORKING CONFIG:
  ```javascript
  const dbConfig = {
    user: 'sa',
    password: 'pa$$word',  // Note: double $$ works
    server: 'MINIJOE\\SQLDEVELOPER2022',
    database: 'WebSource-AutoBot',
    options: { encrypt: false, trustServerCertificate: true }
  };
  ```

### 5. Email Connection Patterns
- ✅ WORKING CONFIG:
  ```javascript
  const emailConfig = {
    host: 'mail.auto-brokerage.com',
    port: 993,
    secure: true,
    auth: {
      user: 'websource@auto-brokerage.com',
      pass: 'WebSource'
    }
  };
  ```

### 6. Node.js Module Issues
- ✅ Install dependencies in correct directory
- ✅ Use ES module syntax: `import` not `require`
- ✅ Check package.json location
- ✅ Use proper file extensions (.js, .mjs)

## 📝 PROCEDURE FOR COMPLEX TASKS

### Phase 1: Information Gathering
1. Use codebase-retrieval to understand existing code
2. Use database queries to understand schema
3. Check file locations and dependencies
4. Identify all required components

### Phase 2: Planning
1. Create detailed step-by-step plan
2. Identify potential failure points
3. Plan testing strategy
4. Get user approval before implementation

### Phase 3: Implementation
1. Follow established patterns from working code
2. Test each component individually
3. Use incremental approach
4. Document any new patterns discovered

### Phase 4: Validation
1. Run comprehensive tests
2. Verify all requirements met
3. Document results
4. Update SOP if new patterns discovered

## 🔧 CURRENT WORKING PATTERNS

### Database Query Pattern:
```javascript
import sql from 'mssql';
const dbConfig = { /* config above */ };
await sql.connect(dbConfig);
const result = await sql.query('SELECT * FROM TableName');
await sql.close();
```

### Email Query Pattern:
```javascript
import { ImapFlow } from 'imapflow';
const client = new ImapFlow(emailConfig);
await client.connect();
await client.mailboxOpen('INBOX');
// operations
await client.logout();
```

## ⚠️ BEFORE EVERY COMMAND
1. Check current directory
2. Verify file paths
3. Use established patterns
4. Test incrementally

## 🔄 PROBLEM RESOLUTIONS LOG

### Problem: Password Escaping in Node.js Inline Commands
- **Issue**: Using `pa\$\$word` in node -e commands causes syntax errors
- **Solution**: Create separate .js files instead of inline commands
- **Pattern**: Always create script files for complex operations
- **Date**: Current session
- **Status**: ✅ RESOLVED

### Problem: Command Cancellation Pattern
- **Issue**: User cancels commands when I make repeated mistakes
- **Root Cause**: Not following established patterns
- **Solution**: Always reference SOP before executing commands
- **Pattern**: Check SOP → Plan → Execute → Update SOP
- **Date**: Current session
- **Status**: ✅ RESOLVED

### Success: Database Schema Discovery
- **Achievement**: Successfully identified OCR table names with hyphens
- **Key Finding**: OCR tables use hyphen format: 'OCR-Fields', 'OCR-FieldFormatRegEx'
- **Pattern**: Use LIKE '%OCR%' to find all related tables
- **Date**: Current session
- **Status**: ✅ WORKING PATTERN ESTABLISHED

### Success: Task Master Integration
- **Achievement**: Successfully used Task Master to create comprehensive task breakdown
- **Key Finding**: Task Master provides superior planning with AI-generated subtasks
- **Pattern**: Use `task-master add-task --prompt="detailed requirements"` then `task-master expand --id=X`
- **Result**: Generated 8 detailed subtasks with dependencies and implementation details
- **Date**: Current session
- **Status**: ✅ SUPERIOR PLANNING TOOL CONFIRMED

### Success: OCR Error Detection Implementation
- **Achievement**: Successfully implemented comprehensive unit test with OCR error detection methods
- **Components Implemented**:
  - CanImportPOInvoiceWithErrorDetection() test method
  - GetInvoiceDataErrors() with DeepSeek API integration
  - UpdateRegex() for OCR pattern correction
  - UpdateInvoice() for data correction
- **Key Features**: TotalsZero validation, DeepSeek prompt templates, error handling
- **Pattern**: Follow existing test structure, use DeepSeek API for intelligent corrections
- **Date**: Current session
- **Status**: ✅ CORE IMPLEMENTATION COMPLETE

### Success: DeepSeek API Integration Fix
- **Problem**: GetCompletionAsync method is private in DeepSeekInvoiceApi class
- **Solution**: Use public ExtractShipmentInvoice method with custom prompt template override
- **Pattern**: Temporarily override PromptTemplate property, call public method, restore original
- **Implementation**: Created ParseErrorResponseFromExtraction to compare extracted vs original data
- **Date**: Current session
- **Status**: ✅ API INTEGRATION CORRECTED

### Success: TotalsZero Property Analysis
- **Achievement**: Found TotalsZero as calculated property in ShipmentInvoice.cs
- **Key Finding**: TotalsZero = detailLevelDifference + headerLevelDifference
- **Formula**: Validates (TotalCost vs Cost*Quantity) + (SubTotal+Freight+Other+Insurance-Deduction vs InvoiceTotal)
- **Usage**: Used in ShipmentUtils.cs for import validation (should equal 0)
- **Pattern**: Property validates invoice mathematical consistency
- **Date**: Current session
- **Status**: ✅ CRITICAL UNDERSTANDING ACHIEVED

### Success: DeepSeek Prompt Structure Analysis
- **Achievement**: Found complete DeepSeek invoice prompt with field mappings
- **Key Mappings**: TotalInternalFreight, TotalOtherCost, TotalInsurance, TotalDeduction
- **Pattern**: Prompt guides LLM to extract specific financial components from text
- **Usage**: Template shows exact JSON structure expected by system
- **Date**: Current session
- **Status**: ✅ FOUNDATION FOR NEW PROMPTS ESTABLISHED
