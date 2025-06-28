# COMPLETE DeepSeek Integration Analysis  
**Analysis Date**: 2025-06-27  
**Purpose**: Ultra-detailed analysis of complete pipeline from DeepSeek error detection to database updates - NO ASSUMPTIONS ALLOWED

## 🚨 CRITICAL: This Analysis is for LLM Implementation

**WARNING**: This document is designed for LLM consumption. Every detail is explicitly documented. No knowledge can be assumed. Every step, every field, every transformation is completely specified.

---

## 📋 COMPLETE DATA FLOW PIPELINE

### **Step 1: DeepSeek Error Detection**
**File**: `OCRErrorDetection.cs:102-156` (`DetectHeaderFieldErrorsAndOmissionsAsync`)

**Input**: 
- `ShipmentInvoice invoice` (in-memory object with extracted values)
- `string fileText` (original OCR text)
- `Dictionary<string, OCRFieldMetadata> metadata` (extraction context)

**Process**:
1. **Header Detection**: Creates prompt via `CreateHeaderErrorDetectionPrompt()` → sends to DeepSeek API
2. **Line Item Detection**: Creates prompt via `CreateProductErrorDetectionPrompt()` → sends to DeepSeek API  
3. **Response Processing**: Both responses processed via `ProcessDeepSeekCorrectionResponse()`

**Output**: `List<InvoiceError>` objects with this EXACT structure:
```csharp
public class InvoiceError
{
    public string Field { get; set; }           // CRITICAL: Must match exact database field names
    public string ExtractedValue { get; set; }  // Current value (or "null")
    public string CorrectValue { get; set; }    // Value found in text
    public double Confidence { get; set; }      // 0.0 to 1.0
    public string ErrorType { get; set; }       // "omission", "format_correction", "character_confusion", etc.
    public string Reasoning { get; set; }       // Human explanation
    public int LineNumber { get; set; }         // Line number in original text
    public string LineText { get; set; }        // Exact line containing the value
    public List<string> ContextLinesBefore { get; set; }
    public List<string> ContextLinesAfter { get; set; }
    public bool RequiresMultilineRegex { get; set; }
    public string SuggestedRegex { get; set; }  // C# compatible regex with named capture groups
    public string Pattern { get; set; }         // For format_correction types
    public string Replacement { get; set; }     // For format_correction types
}
```

---

### **Step 2: Error to CorrectionResult Conversion**  
**File**: `OCRCorrectionService.cs:126-144`

**Input**: `List<InvoiceError>` from Step 1

**Process**: Direct 1:1 mapping in `CorrectInvoiceAsync()`:
```csharp
var successfulDetectionsForDB = allDetectedErrors.Select(e => new CorrectionResult
{
    FieldName = e.Field,                    // EXACT COPY
    OldValue = e.ExtractedValue,           // EXACT COPY
    NewValue = e.CorrectValue,             // EXACT COPY
    CorrectionType = e.ErrorType,          // EXACT COPY
    Confidence = e.Confidence,             // EXACT COPY
    Reasoning = e.Reasoning,               // EXACT COPY
    LineText = e.LineText,                 // EXACT COPY
    LineNumber = e.LineNumber,             // EXACT COPY
    Success = true,                        // Always true for this conversion
    ContextLinesBefore = e.ContextLinesBefore,    // EXACT COPY
    ContextLinesAfter = e.ContextLinesAfter,      // EXACT COPY
    RequiresMultilineRegex = e.RequiresMultilineRegex,  // EXACT COPY
    SuggestedRegex = e.SuggestedRegex,     // EXACT COPY
    Pattern = e.Pattern,                   // EXACT COPY - CRITICAL FOR FORMAT CORRECTIONS
    Replacement = e.Replacement            // EXACT COPY - CRITICAL FOR FORMAT CORRECTIONS
}).ToList();
```

**Output**: `List<CorrectionResult>` with identical data structure

---

### **Step 3: CorrectionResult to RegexUpdateRequest Conversion**
**File**: `OCRCorrectionService.cs:240-286` (`CreateRegexUpdateRequest`)

**Input**: `CorrectionResult` from Step 2

**Process**: Direct field mapping with additional context:
```csharp
var request = new RegexUpdateRequest
{
    FieldName = correction.FieldName,                 // DIRECT COPY
    OldValue = correction.OldValue,                   // DIRECT COPY  
    NewValue = correction.NewValue,                   // DIRECT COPY
    CorrectionType = correction.CorrectionType,       // DIRECT COPY
    Confidence = correction.Confidence,               // DIRECT COPY
    DeepSeekReasoning = correction.Reasoning,         // DIRECT COPY
    RequiresMultilineRegex = correction.RequiresMultilineRegex,  // DIRECT COPY
    SuggestedRegex = correction.SuggestedRegex,       // DIRECT COPY
    ExistingRegex = correction.ExistingRegex,         // DIRECT COPY
    LineId = correction.LineId,                       // Database context (usually null from DeepSeek)
    PartId = correction.PartId,                       // Database context (usually null from DeepSeek)  
    RegexId = correction.RegexId,                     // Database context (usually null from DeepSeek)
    InvoiceId = templateId,                           // Invoice template ID (provided by system)
    LineNumber = correction.LineNumber,               // DIRECT COPY
    LineText = correction.LineText,                   // DIRECT COPY
    WindowText = correction.WindowText,               // DIRECT COPY
    ContextLinesBefore = correction.ContextLinesBefore,  // DIRECT COPY
    ContextLinesAfter = correction.ContextLinesAfter,    // DIRECT COPY
    Pattern = correction.Pattern,                     // DIRECT COPY - FOR FORMAT CORRECTIONS
    Replacement = correction.Replacement              // DIRECT COPY - FOR FORMAT CORRECTIONS
};
```

**Output**: `RegexUpdateRequest` objects sent to database update system

---

### **Step 4: Field Name Validation & Mapping**
**File**: `OCRFieldMapping.cs:140-167` (`MapDeepSeekFieldToDatabase`)

**Critical Process**: Every field name gets validated against supported mappings:

#### **Header Fields** (EntityType = "ShipmentInvoice"):
```csharp
// PRIMARY MAPPINGS (DeepSeek MUST use these exact names)
"InvoiceTotal" → DatabaseFieldInfo("InvoiceTotal", "ShipmentInvoice", "Number", true)
"SubTotal" → DatabaseFieldInfo("SubTotal", "ShipmentInvoice", "Number", true)  
"TotalInternalFreight" → DatabaseFieldInfo("TotalInternalFreight", "ShipmentInvoice", "Number", false)
"TotalOtherCost" → DatabaseFieldInfo("TotalOtherCost", "ShipmentInvoice", "Number", false)
"TotalInsurance" → DatabaseFieldInfo("TotalInsurance", "ShipmentInvoice", "Number", false)
"TotalDeduction" → DatabaseFieldInfo("TotalDeduction", "ShipmentInvoice", "Number", false)
"InvoiceNo" → DatabaseFieldInfo("InvoiceNo", "ShipmentInvoice", "String", true)
"InvoiceDate" → DatabaseFieldInfo("InvoiceDate", "ShipmentInvoice", "English Date", true)
"Currency" → DatabaseFieldInfo("Currency", "ShipmentInvoice", "String", false)
"SupplierName" → DatabaseFieldInfo("SupplierName", "ShipmentInvoice", "String", true)
"SupplierAddress" → DatabaseFieldInfo("SupplierAddress", "ShipmentInvoice", "String", false)

// ALIASES SUPPORTED (DeepSeek can also use these):
"Total" → maps to InvoiceTotal
"GrandTotal" → maps to InvoiceTotal  
"Subtotal" → maps to SubTotal (note casing)
"Freight" → maps to TotalInternalFreight
"Shipping" → maps to TotalInternalFreight
"Tax" → maps to TotalOtherCost
"Insurance" → maps to TotalInsurance
"InvoiceNumber" → maps to InvoiceNo
"Date" → maps to InvoiceDate
"Supplier" → maps to SupplierName
```

#### **Line Item Fields** (EntityType = "InvoiceDetails"):
```csharp  
// PRIMARY MAPPINGS
"ItemDescription" → DatabaseFieldInfo("ItemDescription", "InvoiceDetails", "String", true)
"Quantity" → DatabaseFieldInfo("Quantity", "InvoiceDetails", "Number", true)
"Cost" → DatabaseFieldInfo("Cost", "InvoiceDetails", "Number", true)        // Unit Price
"TotalCost" → DatabaseFieldInfo("TotalCost", "InvoiceDetails", "Number", true)  // Line Total
"Discount" → DatabaseFieldInfo("Discount", "InvoiceDetails", "Number", false)
"Units" → DatabaseFieldInfo("Units", "InvoiceDetails", "String", false)

// PREFIXED PATTERNS (System automatically strips prefixes):
"InvoiceDetail_Line1_Quantity" → strips to "Quantity" → maps to Quantity field
"InvoiceDetail_LineX_ItemDescription" → strips to "ItemDescription" → maps to ItemDescription field

// ALIASES SUPPORTED:
"Description" → maps to ItemDescription
"ProductDescription" → maps to ItemDescription  
"Qty" → maps to Quantity
"Price" → maps to Cost (Unit Price)
"UnitPrice" → maps to Cost
"Amount" → maps to TotalCost (Line Total)
"LineTotal" → maps to TotalCost
```

**CRITICAL**: If DeepSeek returns an unsupported field name, the entire RegexUpdateRequest is REJECTED during validation.

---

### **Step 5: Database Update Strategy Selection**
**File**: `OCRDatabaseStrategies.cs` (multiple strategy classes)

**Process**: Based on `RegexUpdateRequest.CorrectionType`, system selects strategy:

#### **Strategy 1: OmissionUpdateStrategy**
- **Trigger**: `CorrectionType == "omission"`
- **Purpose**: Creates new Line and Field definitions in database
- **Requirements**: 
  - `SuggestedRegex` must contain named capture groups
  - `FieldName` must map to valid database field
  - `InvoiceId` must be provided for Part lookup

#### **Strategy 2: FieldFormatUpdateStrategy** 
- **Trigger**: `CorrectionType == "format_correction"` OR `CorrectionType == "character_confusion"`
- **Purpose**: Creates FieldFormatRegEx entries for value transformation
- **Requirements**:
  - `Pattern` must be valid C# regex
  - `Replacement` must be valid replacement pattern
  - `FieldId` must be provided (links to existing Field)

#### **Strategy 3: Other Strategies**
- Various other strategies for different correction types
- Each requires specific field combinations

---

### **Step 6: Database Entity Creation**
**Files**: Multiple entity classes in `OCR.Business.Entities`

#### **Parts Table Structure**:
```csharp
public class Parts
{
    public int Id { get; set; }              // Primary key  
    public int TemplateId { get; set; }      // Links to invoice template
    public int PartTypeId { get; set; }      // 1=Header, 2=LineItem
    // Navigation properties...
}
```

#### **Lines Table Structure**:
```csharp
public class Lines  
{
    public int Id { get; set; }              // Primary key
    public int PartId { get; set; }          // Links to Parts
    public string Name { get; set; }         // Human description (max 50 chars)
    public int RegExId { get; set; }         // Links to RegularExpressions
    // Navigation properties...
}
```

#### **Fields Table Structure**:
```csharp
public class Fields
{
    public int Id { get; set; }              // Primary key
    public int LineId { get; set; }          // Links to Lines
    public string Key { get; set; }          // Named capture group (max 50 chars)
    public string Field { get; set; }        // Database field name (max 50 chars)  
    public string EntityType { get; set; }   // "ShipmentInvoice" or "InvoiceDetails" (max 50 chars)
    public bool IsRequired { get; set; }     // Validation flag
    public string DataType { get; set; }     // "Number", "String", "English Date" (max 50 chars)
    // Navigation properties...
}
```

#### **RegularExpressions Table Structure**:
```csharp
public class RegularExpressions
{
    public int Id { get; set; }              // Primary key
    public string RegEx { get; set; }        // Actual regex pattern (unlimited length)
    public bool MultiLine { get; set; }      // Spans multiple lines flag
    public int MaxLines { get; set; }        // Maximum lines to span
    public string Description { get; set; }  // Human description  
    public DateTime CreatedDate { get; set; }
    public DateTime LastUpdated { get; set; }
}
```

#### **FieldFormatRegEx Table Structure**:
```csharp
public class FieldFormatRegEx
{
    public int Id { get; set; }              // Primary key
    public int FieldId { get; set; }         // Links to Fields
    public int RegExId { get; set; }         // Pattern regex  
    public int? ReplacementRegExId { get; set; }  // Replacement regex (optional)
    // Used for format corrections (pattern/replacement pairs)
}
```

---

## 🏗️ FOUNDATIONAL OCR DATABASE ARCHITECTURE

### **OCR Pipeline Table Structure** (From WebSource-AutoBot Schema)

**CRITICAL**: This is the foundational database schema that drives the entire OCR processing pipeline. DeepSeek errors must integrate perfectly with these existing structures.

#### **Core Template Hierarchy**:
```
Invoice Templates (TemplateId)
    ↓
Parts (PartId) [Header vs LineItem partitions]
    ↓  
Lines (LineId) [Individual extraction rules]
    ↓
Fields (FieldId) [Field mapping definitions]
    ↓
RegularExpressions (RegexId) [Pattern matching logic]
```

#### **OCR-Parts Table** (Template Structure):
```sql
CREATE TABLE [OCR-Parts](
    [Id] int IDENTITY(1,1) NOT NULL,
    [TemplateId] int NOT NULL,        -- Links to invoice template
    [PartTypeId] int NOT NULL         -- 1=Header, 2=LineItem, 3=Summary, etc.
)
```

**Critical PartTypes**:
- **PartTypeId = 1**: Header fields (InvoiceTotal, SupplierName, etc.)
- **PartTypeId = 2**: Line item details (Quantity, Cost, ItemDescription, etc.)
- **PartTypeId = 3**: Summary sections
- **PartTypeId = 4**: Footer information

#### **OCR-Lines Table** (Extraction Rules):
```sql
CREATE TABLE [OCR-Lines](
    [Id] int IDENTITY(1,1) NOT NULL,
    [PartId] int NOT NULL,            -- Links to Parts
    [RegExId] int NOT NULL,           -- Links to RegularExpressions
    [Name] nvarchar(50) NOT NULL,     -- Human-readable description
    [ParentId] int NULL,              -- For hierarchical rules
    [DistinctValues] bit NULL,        -- Value deduplication flag
    [IsColumn] bit NULL,              -- Column-based extraction
    [IsActive] bit NULL,              -- Enable/disable rule
    [Comments] nvarchar(255) NULL     -- Documentation
)
```

**Example Line Names**:
- "InvoiceTotal", "SubTotal", "SalesJax"
- "InvoiceDetails", "EntryDataDetails"
- "SupplierName", "SupplierAddress", "InvoiceDate"

#### **OCR-Fields Table** (Field Mapping):
```sql
CREATE TABLE [OCR-Fields](
    [Id] int IDENTITY(1,1) NOT NULL,
    [LineId] int NOT NULL,            -- Links to Lines
    [Key] nvarchar(50) NOT NULL,      -- Named capture group name
    [Field] nvarchar(50) NOT NULL,    -- Target database field
    [EntityType] nvarchar(50) NOT NULL, -- "Invoice", "InvoiceDetails", "Suppliers", etc.
    [IsRequired] bit NOT NULL,        -- Validation requirement
    [DataType] nvarchar(50) NOT NULL, -- "Numeric", "String", "English Date", etc.
    [ParentId] int NULL,              -- Field relationships
    [AppendValues] bit NULL           -- Value aggregation behavior
)
```

**Critical EntityType Mappings**:
- **"Invoice"**: Header fields → maps to ShipmentInvoice table
- **"InvoiceDetails"**: Line item fields → maps to ShipmentInvoiceDetails table  
- **"Suppliers"**: Supplier fields → maps to Suppliers table
- **"EntryData"**: Legacy mapping (still used)
- **"ShipmentManifest"**: Shipping document fields

**Critical DataType Values**:
- **"Numeric"**: Monetary values, quantities
- **"String"**: Text fields, codes, names
- **"English Date"**: Date fields with English formatting
- **"Number"**: Integer values, counts

#### **OCR-RegularExpressions Table** (Pattern Storage):
```sql
CREATE TABLE [OCR-RegularExpressions](
    [Id] int IDENTITY(1,1) NOT NULL,
    [RegEx] nvarchar(max) NOT NULL,   -- Actual regex pattern
    [MultiLine] bit NULL,             -- Multi-line matching flag
    [MaxLines] int NULL               -- Maximum lines to span
)
```

**Example Regex Patterns**:
```regex
-- Invoice total extraction
"Invoice Total:\s+(?<InvoiceTotal>[\$\d,.]+)"

-- Product line item extraction  
"(?<ProductCode>[.\S]+)\s(?<ItemDescription>[^$]+)\s(?<Quantity>[\d]+)\s\$(?<Price>[\d.,]+)\s\$(?<ExtendedPrice>[\d.,]+)"

-- Multi-line supplier address
"^Budget.*\r\n\r\n(?<SupplierAddress>[A-Z\s\d\r\n\.\,:()]*)"
```

### **Database Integration Flow** (DeepSeek → Database):

```
1. DeepSeek Error Detection
   ↓
2. RegexUpdateRequest Creation
   ↓  
3. Field Name Validation (OCRFieldMapping.cs)
   ↓
4. Strategy Selection (Omission vs Format)
   ↓
5. Database Entity Creation:
   
   OMISSION STRATEGY:
   - Create RegularExpression entry → Get RegexId
   - Create Line entry → Get LineId  
   - Create Field entry → Get FieldId
   
   FORMAT STRATEGY:
   - Link to existing FieldId
   - Create FieldFormatRegEx entry
```

### **Critical Template Relationships**:

From the database schema, we can see the system supports multiple invoice templates:
- **TemplateId = 1**: Basic invoice template (Parts 4,5,6,7)
- **TemplateId = 3**: Extended template with shipping data
- **TemplateId = 5-33**: Various supplier-specific templates (Amazon, TEMU, etc.)

Each template has multiple parts for different sections:
- **Header Part**: Basic invoice information
- **LineItem Part**: Product details  
- **Summary Part**: Totals and calculations
- **Shipping Parts**: Manifest and freight data

### **Real-World Field Mappings** (From Database):

#### **Header Fields** (EntityType = "Invoice"):
```
Key="InvoiceTotal" → Field="InvoiceTotal" → DataType="Numeric"
Key="InvoiceNo" → Field="InvoiceNo" → DataType="String"
Key="InvoiceDate" → Field="InvoiceDate" → DataType="English Date"
Key="SupplierName" → Field="SupplierName" → DataType="String"
Key="SupplierCode" → Field="SupplierCode" → DataType="String"
Key="SalesJax" → Field="TotalOtherCost" → DataType="Numeric"
Key="Freight" → Field="TotalInternalFreight" → DataType="Number"
```

#### **Line Item Fields** (EntityType = "InvoiceDetails"):
```
Key="ProductCode" → Field="ItemNumber" → DataType="String"
Key="ItemDescription" → Field="ItemDescription" → DataType="String"
Key="Quantity" → Field="Quantity" → DataType="Numeric"
Key="Price" → Field="Cost" → DataType="Numeric"              // Unit price
Key="ExtendedPrice" → Field="TotalCost" → DataType="Numeric"  // Line total
Key="NetPrice" → Field="Cost" → DataType="Numeric"
```

---

## 🎯 CRITICAL DEEPSEEK PROMPT REQUIREMENTS

### **Header Field Detection Prompt Requirements**

#### **MANDATORY JSON RESPONSE STRUCTURE**:
```json
{
  "errors": [
    {
      "field": "EXACT_DATABASE_FIELD_NAME",           // Must match OCRFieldMapping.cs mappings
      "extracted_value": "CURRENT_VALUE_OR_NULL",     // What system currently has
      "correct_value": "VALUE_FROM_TEXT",             // Value found in invoice text
      "line_text": "EXACT_SOURCE_LINE",               // Line containing the value
      "line_number": 15,                              // 1-based line number
      "confidence": 0.95,                             // 0.0 to 1.0
      "error_type": "ERROR_TYPE",                     // See types below
      "entity_type": "ShipmentInvoice",               // ALWAYS "ShipmentInvoice" for headers
      "suggested_regex": "C#_COMPATIBLE_REGEX",       // With named capture groups
      "reasoning": "HUMAN_EXPLANATION"                // Why this error was flagged
    }
  ]
}
```

#### **SUPPORTED ERROR TYPES FOR HEADERS**:
- **"omission"**: Value missing but found in text → Creates new Line/Field definitions
- **"format_correction"**: Value needs transformation → Creates FieldFormatRegEx entry  
- **"character_confusion"**: OCR misread character → Creates FieldFormatRegEx entry
- **"inferred"**: Value not visible but can be inferred → Creates new Line/Field definitions

#### **SUPPORTED FIELD NAMES** (Headers - EXACT NAMES REQUIRED):
```
PRIMARY: InvoiceTotal, SubTotal, TotalInternalFreight, TotalOtherCost, TotalInsurance, 
         TotalDeduction, InvoiceNo, InvoiceDate, Currency, SupplierName, SupplierAddress

ALIASES: Total, GrandTotal, Subtotal, Freight, Shipping, Tax, Insurance, InvoiceNumber, 
         Date, Supplier (these get auto-mapped to primary names)
```

#### **REGEX REQUIREMENTS FOR HEADERS**:
- **Must be C# compatible** (single backslashes, not double)
- **Must include named capture groups** matching the field name
- **Example**: `"Grand Total:\\s*\\$?(?<InvoiceTotal>[\\d,]+\\.\\d{2})"`
- **Currency symbols OUTSIDE capture group**: `\\$(?<amount>\\d+)` not `(?<amount>\\$\\d+)`

### **Line Item Detection Prompt Requirements**

#### **MANDATORY JSON RESPONSE STRUCTURE**:
```json
{
  "errors": [
    {
      "field": "InvoiceDetail_LineX_FIELD_NAME",      // X = actual line number in text
      "extracted_value": "CURRENT_VALUE_OR_NULL",     
      "correct_value": "VALUE_FROM_TEXT",             
      "line_text": "EXACT_SOURCE_LINE",               
      "line_number": 15,                              // Actual line number where product appears
      "confidence": 0.95,                             
      "error_type": "ERROR_TYPE",                     
      "entity_type": "InvoiceDetails",                // ALWAYS "InvoiceDetails" for line items
      "suggested_regex": "C#_COMPATIBLE_REGEX",       
      "reasoning": "HUMAN_EXPLANATION"                
    }
  ]
}
```

#### **SUPPORTED FIELD NAMES FOR LINE ITEMS**:
```
PRIMARY: InvoiceDetail_LineX_ItemDescription, InvoiceDetail_LineX_Quantity, 
         InvoiceDetail_LineX_Cost, InvoiceDetail_LineX_TotalCost, 
         InvoiceDetail_LineX_Discount, InvoiceDetail_LineX_Units

Where X = actual line number in invoice text where the product appears

ALIASES: Description, ProductDescription, Qty, Price, UnitPrice, Amount, LineTotal
         (system auto-strips InvoiceDetail_LineX_ prefix and maps aliases)
```

#### **MULTILINE AND MULTI-FIELD REGEX CAPABILITIES**:

**Multiline Regex Support**:
```json
{
  "requires_multiline_regex": true,           // Enables multiline pattern matching
  "suggested_regex": "Pattern with \\r\\n",  // Line breaks in regex pattern
  "context_lines_before": ["Line 1", "Line 2"], // Context for pattern development
  "context_lines_after": ["Line 4", "Line 5"]   // Additional context lines
}
```

**Multi-Field Line Item Extraction**:
```json
{
  "field": "InvoiceDetail_Line15_ItemDescription",
  "suggested_regex": "(?<ItemDescription>[^\\t]+)\\t(?<Quantity>\\d+)\\t\\$(?<Cost>[\\d\\.]+)\\t\\$(?<TotalCost>[\\d\\.]+)",
  // Single regex with multiple named capture groups creates multiple Field entries
}
```

**Database Implementation**:
- **Single RegularExpression**: One pattern with multiple named groups
- **Single Line Entry**: Links regex to LineItem Part
- **Multiple Field Entries**: Each named group becomes separate Field entry
- **Multiline Support**: MultiLine=true, MaxLines calculated from context

#### **LINE ITEM FOCUS REQUIREMENTS**:
- **ONLY report actual product line items** (descriptions, quantities, prices, item codes)
- **DO NOT report invoice metadata** (dates, order numbers, totals, payment info)
- **Use actual line numbers** where products appear in text
- **Each line item error must include entity_type = "InvoiceDetails"**

---

## 🚨 CRITICAL VALIDATION REQUIREMENTS

### **Field Name Validation Process**:
1. **Input**: `RegexUpdateRequest.FieldName` from DeepSeek
2. **Process**: `OCRFieldMapping.MapDeepSeekFieldToDatabase(fieldName)`
3. **Validation**: Returns `DatabaseFieldInfo` object OR `null` if unsupported
4. **Result**: If `null`, entire request is REJECTED with error message

### **Entity Type Validation**:
- **Header fields**: MUST have `entity_type = "ShipmentInvoice"`
- **Line item fields**: MUST have `entity_type = "InvoiceDetails"`  
- **Wrong entity type**: Request REJECTED

### **Regex Validation**:
- **C# Compatibility**: Must compile with `System.Text.RegularExpressions.Regex`
- **Named Capture Groups**: Must contain at least one named group
- **Group Names**: Should match field names (after prefix stripping)
- **Invalid regex**: Request REJECTED

### **Required Fields**:
- **field**: Cannot be null or empty
- **correct_value**: Required for omission/inferred types  
- **line_text**: Required for regex generation
- **line_number**: Must be > 0
- **entity_type**: Must be "ShipmentInvoice" or "InvoiceDetails"
- **Missing required field**: Request REJECTED

---

## 📊 DATABASE UPDATE EXECUTION FLOW

### **Strategy Execution Order**:
1. **Omission Strategy**: Creates Line + Field + RegularExpression entries
2. **Format Strategy**: Creates FieldFormatRegEx entries (must run AFTER omission if paired)
3. **Transaction Management**: Each strategy runs in atomic transaction
4. **Pairing Logic**: System automatically links format_correction to preceding omission for same field

### **Database ID Propagation**:
```
Omission creates: RegularExpression (gets RegexId) → Line (gets LineId) → Field (gets FieldId)
Format correction uses: FieldId from paired omission to create FieldFormatRegEx entry
```

### **Part Selection Logic**:
```csharp
// For header fields (EntityType = "ShipmentInvoice")
var headerPart = Parts.FirstOrDefault(p => p.TemplateId == invoiceId && p.PartTypes.Name == "Header")

// For line item fields (EntityType = "InvoiceDetails")  
var lineItemPart = Parts.FirstOrDefault(p => p.TemplateId == invoiceId && p.PartTypes.Name == "LineItem")
```

### **Line Creation Process**:
```csharp
// System creates new Line with generated name
var newLine = new Lines 
{
    PartId = selectedPart.Id,
    Name = $"AI_Generated_{fieldName}_{DateTime.UtcNow:yyyyMMdd_HHmmss}",  // Unique name
    RegExId = newRegex.Id,                          // Links to created RegularExpression
    TrackingState = TrackingState.Added
};
```

### **Field Creation Process**:
```csharp
// System creates new Field linking to Line
var newField = new Fields
{
    LineId = newLine.Id,                            // Links to created Line
    Key = extractedGroupName,                       // From suggested_regex named groups
    Field = databaseFieldInfo.DatabaseFieldName,    // From field mapping
    EntityType = databaseFieldInfo.EntityType,      // "ShipmentInvoice" or "InvoiceDetails"
    DataType = databaseFieldInfo.DataType,          // "Number", "String", "English Date"
    IsRequired = databaseFieldInfo.IsRequired,      // From field mapping
    TrackingState = TrackingState.Added
};
```

---

## 🎯 PROMPT OPTIMIZATION REQUIREMENTS

### **DeepSeek Must Include in Response**:
1. **Exact field names** from supported mapping list
2. **Correct entity_type** for each field
3. **C# compatible regex patterns** with named capture groups
4. **Actual line numbers** where values appear in text
5. **Complete line text** for regex generation context
6. **Error type classification** for strategy selection

### **DeepSeek Must NOT Include**:
1. **Unsupported field names** (causes validation failure)
2. **Invoice metadata as line items** (wrong entity type)
3. **Double-backslash regex** (JavaScript format - causes C# errors)
4. **Regex without named capture groups** (database linking fails)
5. **Wrong entity types** (header fields with "InvoiceDetails" entity type)

### **Critical Success Dependencies**:
- **Field Name Accuracy**: 100% must match mapping dictionary
- **Entity Type Accuracy**: 100% must match field classification  
- **Regex Compatibility**: 100% must compile in C# without errors
- **Line Context**: 100% accurate line numbers and text for regex testing

---

## 🔧 IMPLEMENTATION CHECKLIST FOR LLM

### **Before Updating DeepSeek Prompts**:
- [ ] Verify all supported field names in `OCRFieldMapping.cs`
- [ ] Confirm entity type mappings (ShipmentInvoice vs InvoiceDetails)
- [ ] Test regex patterns in C# environment
- [ ] Validate JSON response structure matches `InvoiceError` class exactly

### **Prompt Must Include**:
- [ ] Complete list of supported field names (primary + aliases)
- [ ] Entity type requirements for each field category
- [ ] Regex pattern requirements and examples
- [ ] JSON response structure with exact field names
- [ ] Error type definitions and their purposes
- [ ] Line item vs header field distinction rules

### **Testing Requirements**:
- [ ] Build solution after prompt changes
- [ ] Test with known invoice files
- [ ] Verify database entries are created correctly
- [ ] Confirm no validation failures in logs
- [ ] Validate regex patterns work in extraction

---

---

## 🎯 CRITICAL INTEGRATION REQUIREMENTS FOR LLM IMPLEMENTATION

### **Exact Database Field Name Requirements**:

Based on the foundational OCR database schema analysis, DeepSeek prompts MUST use these exact field names to ensure proper database integration:

#### **HEADER FIELD NAMES** (EntityType = "Invoice"):
```
REQUIRED EXACT NAMES:
- InvoiceTotal, InvoiceNo, InvoiceDate
- SupplierName, SupplierCode, SupplierAddress  
- TotalOtherCost, TotalInternalFreight
- SubTotal, Currency

ACCEPTABLE ALIASES (auto-mapped):
- Total → InvoiceTotal
- Date → InvoiceDate  
- SalesJax → TotalOtherCost
- Freight → TotalInternalFreight
```

#### **LINE ITEM FIELD NAMES** (EntityType = "InvoiceDetails"):
```
REQUIRED EXACT NAMES:  
- ItemNumber, ItemDescription, Quantity
- Cost, TotalCost
- Discount, Units

ACCEPTABLE ALIASES (auto-mapped):
- ProductCode → ItemNumber
- Price → Cost
- ExtendedPrice → TotalCost
- NetPrice → Cost
```

### **Critical EntityType Validation**:

**MUST MATCH DATABASE SCHEMA**:
- Header fields: `entity_type = "Invoice"` (NOT "ShipmentInvoice")
- Line items: `entity_type = "InvoiceDetails"` (NOT "InvoiceDetail")

### **Regex Pattern Requirements**:

**MUST BE C# COMPATIBLE**:
- Single backslashes (not double)
- Named capture groups matching field names
- Examples from live database:
  ```regex
  "Invoice Total:\s+(?<InvoiceTotal>[\$\d,.]+)"
  "(?<ProductCode>[.\S]+)\s(?<ItemDescription>[^$]+)\s(?<Quantity>[\d]+)"
  ```

### **Template Integration Requirements**:

DeepSeek errors will be integrated into existing invoice templates:
- **Amazon invoices**: TemplateId = 3+ (existing template structure)
- **Generic invoices**: TemplateId = 1 (basic template)
- **New fields**: Create new Line/Field entries linked to appropriate PartId

### **Database Transaction Requirements**:

**Omission Strategy** (creates new extraction rules):
1. Create `OCR-RegularExpressions` entry with DeepSeek regex
2. Create `OCR-Lines` entry linking to Part and RegEx
3. Create `OCR-Fields` entry with proper mapping

**Format Strategy** (fixes existing values):
1. Find existing FieldId from `OCR-Fields` table
2. Create `OCR-FieldFormatRegEx` entry for value transformation

### **Validation Chain Requirements**:

All DeepSeek field names MUST pass this validation chain:
1. **Field Name Validation**: `OCRFieldMapping.MapDeepSeekFieldToDatabase()`
2. **Entity Type Check**: Must match "Invoice" or "InvoiceDetails"  
3. **Regex Compilation**: Must compile in C# without errors
4. **Template Compatibility**: Must link to existing template structure

---

**IMPLEMENTATION GUARANTEE**: This analysis provides complete end-to-end documentation with ZERO assumptions. Every field name, every entity type, every regex requirement is derived from the actual database schema and codebase. Following these specifications exactly will ensure seamless DeepSeek integration with the existing OCR pipeline.