# 🚨 **ENTRYTYPE ENUM REFACTORING - COMPREHENSIVE IMPLEMENTATION PLAN**

**Status**: 🔥 **CRITICAL ARCHITECTURAL REFACTORING IN PROGRESS**  
**Created**: July 31, 2025  
**Objective**: Replace all hardcoded document type strings with `FileTypeManager.EntryTypes` enum as single source of truth

---

## 🎯 **EXECUTIVE SUMMARY**

**Root Cause Confirmed**: System uses 4+ different representations of the same document type:
- `"Shipment Invoice"` (EntryTypes.ShipmentInvoice - CORRECT ✅)
- `"invoice"` (lowercase - INCORRECT ❌)
- `"shipmentinvoice"` (no spaces - INCORRECT ❌) 
- `"InvoiceDetails"` (compound table name - SHOULD BE "ShipmentInvoiceDetails" ❌)

**Impact**: Template validation failures, inconsistent LLM prompts, database mapping chaos, MANGO test failures.

---

## 📋 **PHASE 1: DATABASE MAPPING FIXES** ✅ IN PROGRESS

### **1.1 Current Database Issues Identified:**
```sql
-- PROBLEM: NULL EntryTypes in database templates
DB_TEMPLATE: '03152025_TOTAL_AMOUNT_GENERIC_DOCUMENT' (ID: 1341)
└── DB_ENTRYTYPE: NULL (FileImporterInfos is null)

DB_TEMPLATE: '03152025_TOTAL_AMOUNT_UNKNOWN_DOCUMENT' (ID: 1342)  
└── DB_ENTRYTYPE: NULL (FileImporterInfos is null)
```

### **1.2 Database Schema Updates Required:**
```sql
-- Update FileTypes-FileImporterInfo EntryType mappings
UPDATE [WebSource-AutoBot].[dbo].[FileTypes-FileImporterInfo] 
SET EntryType = 'Shipment Invoice'  -- Use EntryTypes.ShipmentInvoice value
WHERE EntryType IS NULL OR EntryType IN ('invoice', 'shipmentinvoice');

-- Update template specifications to use correct table names
UPDATE [WebSource-AutoBot].[dbo].[OCR_TemplateTableMapping]
SET TableType = 'ShipmentInvoiceDetails'
WHERE TableType = 'InvoiceDetails';
```

---

## 📋 **PHASE 2: CORE FILE REFACTORING** ⏳ PENDING

### **2.1 Critical Files Requiring Immediate Changes:**

#### **A. AITemplateService.cs** - Lines 167, 173, 2281-2282, 2590-2600
```csharp
// BEFORE (INCORRECT):
"invoice" => new List<string> { "Invoice", "InvoiceDetails", "EntryData", "EntryDataDetails" },
"shipmentinvoice" => new List<string> { "Invoice", "InvoiceDetails", "EntryData", "EntryDataDetails" },

// AFTER (CORRECT):
FileTypeManager.EntryTypes.ShipmentInvoice => new List<string> { "Invoice", "ShipmentInvoiceDetails", "EntryData", "EntryDataDetails" },
```

#### **B. DeepSeekInvoiceApi.cs** - Lines 207, 397, 406
```csharp
// BEFORE (INCORRECT):
""InvoiceDetails"": [{{

// AFTER (CORRECT):  
""ShipmentInvoiceDetails"": [{{
```

#### **C. DatabaseTemplateHelper.cs** - Line 38
```csharp
// BEFORE (INCORRECT):
{ "Shipment Invoice", "Invoice" },

// AFTER (CORRECT):
{ FileTypeManager.EntryTypes.ShipmentInvoice, "ShipmentInvoice" },
```

#### **D. OCRCorrectionService.cs** - Lines 2594-2595 (Remove symptom fix)
```csharp
// REMOVE ENTIRELY (symptom fix):
"shipmentinvoice" => "invoice",          // Map to "invoice" (has database mappings)
"invoice" => "invoice",                  // Already normalized

// REPLACE WITH (root cause fix):
return FileTypeManager.EntryTypes.GetEntryType(documentType);
```

### **2.2 Service Files - Replace "InvoiceDetails" with "ShipmentInvoiceDetails":**
- ShipmentInvoiceService.cs (4 occurrences)
- ShipmentInvoicePOItemQueryMatchesService.cs (4 occurrences)
- ShipmentInvoiceDetailsItemAliasService.cs (4 occurrences)  
- InvoiceDetailsService.cs (12 occurrences)

---

## 📋 **PHASE 3: LLM PROMPT STANDARDIZATION** ⏳ PENDING

### **3.1 Prompt Template Updates:**
All LLM prompts must use:
- **Document Type**: `FileTypeManager.EntryTypes.ShipmentInvoice` 
- **Table Name**: `"ShipmentInvoiceDetails"` (not "InvoiceDetails")
- **Validation Rules**: Consistent with EntryTypes enum values

### **3.2 Template Specifications:**
```csharp
// Update all TemplateSpecification.CreateForUtilityOperation calls:
var templateSpec = TemplateSpecification.CreateForUtilityOperation(
    FileTypeManager.EntryTypes.ShipmentInvoice,  // Use enum constant
    "MethodName", 
    input, 
    output
);
```

---

## 📋 **PHASE 4: VALIDATION SYSTEM REFACTOR** ⏳ PENDING

### **4.1 Template Specification Validation:**
- Update GetEntityTypesForDocument to use EntryTypes enum
- Replace hardcoded strings with enum-based validation
- Update GetSupportedTableTypes mappings

### **4.2 Document Type Normalization:**
```csharp
// REMOVE symptom fixes, use enum directly:
public static string GetDocumentType(string input)
{
    return FileTypeManager.EntryTypes.GetEntryType(input);
}
```

---

## 📋 **PHASE 5: PIPELINE INTEGRATION** ⏳ PENDING

### **5.1 Pipeline Component Updates:**
- OCRDocumentSeparator.cs - Use EntryTypes for document detection
- All template creation methods - Use enum constants
- Document processing steps - Consistent enum usage

### **5.2 Import Utilities:**
- ImportPDFDeepSeek.cs - Remove hardcoded mappings
- FileTypeManager integration throughout pipeline

---

## 📋 **PHASE 6: SUCCESS CRITERIA VALIDATION** ⏳ PENDING

### **6.1 MANGO Test Requirements:**
- ✅ Template specifications pass validation
- ✅ "Invoice Details" becomes "Shipment Invoice Details"  
- ✅ Database mappings use EntryTypes enum values
- ✅ LLM prompts consistent across all providers
- ✅ No hardcoded document type strings remain

### **6.2 Validation Commands:**
```bash
# Run MANGO test to verify complete fix:
"/mnt/c/Program Files/Microsoft Visual Studio/2022/Enterprise/Common7/IDE/CommonExtensions/Microsoft/TestWindow/vstest.console.exe" "./AutoBotUtilities.Tests/bin/x64/Debug/net48/AutoBotUtilities.Tests.dll" /TestCaseFilter:"FullyQualifiedName=AutoBotUtilities.Tests.PDFImportTests.CanImportMango03152025TotalAmount_AfterLearning" "/Logger:console;verbosity=detailed"

# Verify database mappings:
SELECT EntryType, COUNT(*) FROM [FileTypes-FileImporterInfo] GROUP BY EntryType;
SELECT TableType, COUNT(*) FROM OCR_TemplateTableMapping GROUP BY TableType;
```

---

## 🎯 **IMPLEMENTATION STATUS TRACKING**

| Phase | Status | Files Modified | Tests Passing |
|-------|--------|----------------|---------------|
| **Phase 1: Database** | 🔄 IN PROGRESS | 0/2 | ❌ |
| **Phase 2: Core Files** | ⏳ PENDING | 0/6 | ❌ |  
| **Phase 3: LLM Prompts** | ⏳ PENDING | 0/8 | ❌ |
| **Phase 4: Validation** | ⏳ PENDING | 0/4 | ❌ |
| **Phase 5: Pipeline** | ⏳ PENDING | 0/10 | ❌ |
| **Phase 6: Testing** | ⏳ PENDING | 0/1 | ❌ |

**Overall Progress**: 🚨 **0% Complete** - Implementation starting

---

## 🚨 **CRITICAL SUCCESS INDICATORS**

1. **Zero hardcoded document type strings** - All use EntryTypes enum
2. **Consistent table naming** - "ShipmentInvoiceDetails" throughout  
3. **Database alignment** - EntryType values match enum constants
4. **LLM prompt consistency** - All providers use same document types
5. **MANGO test passes** - Complete end-to-end validation
6. **Template specifications validate** - No more dual-layer failures

---

## 📋 **NEXT IMMEDIATE ACTIONS**

1. **✅ COMPLETE** - Comprehensive analysis and problem identification
2. **🔄 IN PROGRESS** - Database mapping updates using DatabaseHelper test
3. **⏳ NEXT** - Systematic file refactoring starting with AITemplateService.cs
4. **⏳ QUEUED** - LLM prompt standardization across all providers
5. **⏳ QUEUED** - Validation system enum integration  
6. **⏳ QUEUED** - Pipeline component consistency updates
7. **⏳ QUEUED** - Complete MANGO test validation

**Implementation Mode**: 🚀 **AUTOMATED CONTINUOUS EXECUTION**
**Target**: 100% EntryTypes enum compliance with MANGO test passing

---

*This document will be updated in real-time as implementation progresses.*