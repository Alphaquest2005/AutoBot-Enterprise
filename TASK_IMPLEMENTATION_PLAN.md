# COMPREHENSIVE TASK IMPLEMENTATION PLAN

## 🎯 TASK OVERVIEW
Create unit test for PO-211-17318585790070596.pdf with OCR error detection and correction using DeepSeek LLM.

## 📋 COMPONENTS TO IMPLEMENT

### 1. Unit Test: `CanImportPOInvoiceWithErrorDetection()`
**Location**: `AutoBotUtilities.Tests\PDFImportTests.cs`
**Template**: Based on `CanImportAmazonMultiSectionInvoice()`
**Additional Checks**:
- Import PDF: `C:\Insight Software\AutoBot-Enterprise\AutoBotUtilities.Tests\Test Data\PO-211-17318585790070596.pdf`
- Retrieve ShipmentInvoice from database with details
- Assert `TotalsZero` property == 0
- Call `GetInvoiceDataErrors()` method
- Call `UpdateRegex()` method  
- Call `UpdateInvoice()` method

### 2. Method: `GetInvoiceDataErrors()`
**Purpose**: Compare ShipmentInvoice data with original file text using DeepSeek
**Input**: 
- `ShipmentInvoice shipmentInvoice`
- `string fileText`
**Output**: `List<(string Field, string Error, string Value)>`
**Process**:
1. Create comparison prompt based on existing DeepSeek invoice prompt
2. Send to DeepSeek API
3. Parse response to identify field discrepancies
4. Map to OCR-Fields table structure

### 3. Method: `UpdateRegex()`
**Purpose**: Fix regex patterns in OCR-FieldFormatRegEx table
**Input**: `List<(string Field, string Error, string Value)> errors, string fileTxt`
**Process**:
1. For each error, get current regex from OCR-FieldFormatRegEx
2. Create DeepSeek prompt to fix regex
3. Test new regex against file text
4. Update database with corrected regex

### 4. Method: `UpdateInvoice()`
**Purpose**: Update ShipmentInvoice with corrected data
**Input**: `ShipmentInvoice shipmentInvoice, List<(string Field, string Error, string Value)> errors`
**Process**:
1. Apply corrections to ShipmentInvoice object
2. Save to database

## 🔧 DATABASE SCHEMA REFERENCE

### ShipmentInvoice Key Fields:
- InvoiceNo, InvoiceDate, InvoiceTotal, SupplierName, Currency, etc.

### OCR-Fields Structure:
- Id, LineId, Key, Field, EntityType, IsRequired, DataType, ParentId

### OCR-FieldFormatRegEx Structure:
- Id, FieldId, RegExId, ReplacementRegExId

## 📝 DEEPSEEK PROMPT TEMPLATES

### Error Detection Prompt:
```
Compare the extracted invoice data with the original text and identify discrepancies.

EXTRACTED DATA:
{shipmentInvoiceJson}

ORIGINAL TEXT:
{fileText}

Return JSON with errors:
{
  "errors": [
    {
      "field": "FieldName",
      "extracted_value": "WrongValue", 
      "correct_value": "CorrectValue",
      "error_description": "Description"
    }
  ]
}
```

### Regex Correction Prompt:
```
Fix this regex pattern to correctly extract the value from the text.

CURRENT REGEX: {currentRegex}
TARGET VALUE: {targetValue}
SOURCE TEXT: {sourceText}
FIELD: {fieldName}

Return only the corrected regex pattern.
```

## 🧪 IMPLEMENTATION STEPS

1. **Create Unit Test Structure**
2. **Implement GetInvoiceDataErrors Method**
3. **Implement UpdateRegex Method** 
4. **Implement UpdateInvoice Method**
5. **Test Integration**
6. **Validate TotalsZero Property**

## ⚠️ CRITICAL CONSIDERATIONS

- Use existing DeepSeek API configuration
- Maintain minimal regex changes
- Preserve existing functionality
- Handle database transactions properly
- Include comprehensive error handling
- Follow established patterns from codebase
