# Invoice Processing Pipeline Analysis
**Analysis Date**: 2025-06-27  
**Purpose**: Complete understanding of invoice processing from PDF extraction to database updates for DeepSeek error integration

## 🏗️ Database Architecture Overview

### **Core Entity Relationships**
```
Invoice Template (Invoices) 
    ↓ TemplateId
Parts (Header/LineItem sections)
    ↓ PartId  
Lines (Regex patterns for extraction)
    ↓ LineId
Fields (Individual data points)
    ↓ FieldId
RegularExpressions (Actual regex patterns)
```

### **Critical Database Tables**

#### **1. Parts Table**
- **Purpose**: Defines sections of invoice template (Header vs LineItem)
- **Key Fields**:
  - `Id` (PartId): Primary key
  - `TemplateId`: Links to specific invoice template
  - `PartTypeId`: References PartTypes (Header=1, LineItem=2)
- **Relationship**: `Invoice → Parts → Lines → Fields`

#### **2. Lines Table** 
- **Purpose**: Defines regex patterns that extract multiple fields from invoice text
- **Key Fields**:
  - `Id` (LineId): Primary key  
  - `PartId`: Links to Parts table
  - `Name`: Human-readable line description (e.g., "TotalAmounts", "ProductLine")
  - `RegExId`: References RegularExpressions table for extraction pattern
- **Critical Function**: One Line can extract multiple Fields using a single regex

#### **3. Fields Table**
- **Purpose**: Defines individual data points extracted from Lines
- **Key Fields**:
  - `Id` (FieldId): Primary key
  - `LineId`: Links to Lines table  
  - `Key`: Regex named capture group (e.g., "InvoiceTotal", "ItemDescription")
  - `Field`: Database column name (e.g., "InvoiceTotal", "ItemDescription") 
  - `EntityType`: Target entity ("ShipmentInvoice" or "InvoiceDetails")
  - `DataType`: Data type validation ("decimal", "string", "datetime")
  - `IsRequired`: Whether field is mandatory for successful import

#### **4. RegularExpressions Table**
- **Purpose**: Stores actual regex patterns used by Lines
- **Key Fields**:
  - `Id` (RegexId): Primary key
  - `RegEx`: The actual regex pattern with named capture groups
  - `MultiLine`: Whether pattern spans multiple lines
  - `MaxLines`: Maximum lines the pattern can span
  - `Description`: Human-readable pattern description

## 📋 Invoice Processing Pipeline Flow

### **Step 1: PDF → Text Extraction**
1. PDF converted to text using OCR
2. Text formatted and sectioned (Single Column, Ripped Text, SparseText)
3. Formatted text stored for template matching

### **Step 2: Template Matching**
1. System identifies invoice type (Amazon, TEMU, etc.)
2. Loads corresponding template with Parts/Lines/Fields structure
3. Template contains all regex patterns needed for extraction

### **Step 3: Data Extraction Process**
```csharp
// For each Part in template (Header, LineItem)
foreach (var part in template.Parts)
{
    // For each Line in the part
    foreach (var line in part.Lines)
    {
        // Apply the Line's regex to extract all fields at once
        var regex = line.RegularExpression.RegEx;
        var matches = Regex.Matches(invoiceText, regex);
        
        // For each Field defined in this Line
        foreach (var field in line.Fields)
        {
            // Extract value using field's Key (named capture group)
            var value = match.Groups[field.Key].Value;
            
            // Map to database entity based on field.EntityType
            if (field.EntityType == "ShipmentInvoice")
                invoice[field.Field] = value;  // e.g., invoice.InvoiceTotal = value
            else if (field.EntityType == "InvoiceDetails")
                invoiceDetail[field.Field] = value;  // e.g., detail.ItemDescription = value
        }
    }
}
```

### **Step 4: OCR Correction Integration**
1. **Error Detection**: DeepSeek identifies missing/incorrect values
2. **Regex Generation**: System creates new regex patterns for corrections  
3. **Database Updates**: New Lines/Fields/RegularExpressions added to template
4. **Re-extraction**: Invoice re-processed with enhanced template

## 🎯 DeepSeek Error Integration Requirements

### **Critical Data Structure Mapping**

#### **For Header Fields** (EntityType = "ShipmentInvoice"):
```json
{
  "field": "InvoiceTotal",           // Maps to Fields.Field and Fields.Key
  "extracted_value": "null",         // Current extracted value
  "correct_value": "166.30",         // Value found in text
  "line_text": "Grand Total: $166.30",  // Source line for regex creation
  "suggested_regex": "Grand Total:\\s*\\$?(?<InvoiceTotal>[\\d,]+\\.\\d{2})"
}
```

#### **For Line Item Fields** (EntityType = "InvoiceDetails"):
```json
{
  "field": "InvoiceDetail_Line1_ItemDescription",  // Field naming convention
  "extracted_value": "",
  "correct_value": "LED Light Display",
  "line_text": "3 of: LED Light Display $39.99",
  "suggested_regex": "\\d+\\s+of:\\s+(?<ItemDescription>[^$]+)\\s*\\$"
}
```

### **Essential Field Naming Conventions**

#### **Header Fields** (EntityType = "ShipmentInvoice"):
- Direct mapping: `"InvoiceTotal"` → `invoice.InvoiceTotal`
- Standard fields: `InvoiceNo`, `InvoiceDate`, `SupplierName`, `Currency`
- Financial fields: `SubTotal`, `TotalInternalFreight`, `TotalOtherCost`, `TotalInsurance`, `TotalDeduction`

#### **Line Item Fields** (EntityType = "InvoiceDetails"):
- Pattern: `"InvoiceDetail_LineX_FieldName"` where X = line number
- Standard fields: `ItemDescription`, `Quantity`, `UnitCost`, `TotalCost`, `ItemCode`
- Example: `"InvoiceDetail_Line1_ItemDescription"` → `invoiceDetail.ItemDescription`

### **Database Update Process for DeepSeek Errors**

#### **When DeepSeek Finds Missing Field**:
1. **Determine Part Type**: 
   - Header fields → Find Part with PartType "Header"
   - Line item fields → Find Part with PartType "LineItem"

2. **Create/Update Line**:
   - Create new Line with descriptive name
   - Generate RegularExpression with named capture groups
   - Link Line to appropriate Part

3. **Create Field Definition**:
   - Extract field name from DeepSeek error
   - Map to correct EntityType (ShipmentInvoice/InvoiceDetails)
   - Set appropriate DataType and IsRequired flags
   - Link Field to Line with correct Key (capture group name)

4. **Validation & Testing**:
   - Test regex against source text
   - Verify field extraction works correctly
   - Update template for future use

## 🔧 DeepSeek Prompt Requirements

### **Must Include for Proper Integration**:
1. **Precise Field Names**: Exact database field names for proper mapping
2. **Entity Type Context**: Clear distinction between header vs line item fields
3. **Line Number Tracking**: Essential for InvoiceDetail field naming
4. **Regex-Ready Output**: Patterns that can be directly used in database
5. **Part Context**: Understanding of template structure for proper Line creation

### **Required Error Object Structure**:
```json
{
  "field": "ExactDatabaseFieldName",
  "extracted_value": "CurrentValue", 
  "correct_value": "CorrectValue",
  "line_text": "ExactSourceLine",
  "line_number": 15,
  "entity_type": "ShipmentInvoice|InvoiceDetails",
  "suggested_regex": "PatternWithNamedCaptureGroup",
  "confidence": 0.95,
  "error_type": "omission|format_correction|character_confusion"
}
```

## ⚠️ Critical Integration Points

### **1. Field Name Precision**
- DeepSeek field names must exactly match database Field.Field values
- Header fields use direct names: `"InvoiceTotal"`
- Line item fields use numbered pattern: `"InvoiceDetail_Line1_ItemDescription"`

### **2. Regex Pattern Requirements**
- Must include named capture groups matching Field.Key
- Pattern must be C# regex compatible 
- Should be specific enough to avoid false matches
- Must work with OCR text formatting variations

### **3. Entity Type Mapping**
- `"ShipmentInvoice"` → Header fields (invoice-level data)
- `"InvoiceDetails"` → Line item fields (product-level data)
- Incorrect mapping will cause database update failures

### **4. Line Number Accuracy**
- Essential for creating proper InvoiceDetail field names
- Must correspond to actual line position in invoice text
- Used for generating Line.Name in database

---
**Status**: Complete analysis for DeepSeek integration  
**Next Steps**: Update DeepSeek prompts to ensure proper field naming and entity type mapping  
**Validation**: Test with actual Amazon invoice to verify end-to-end flow