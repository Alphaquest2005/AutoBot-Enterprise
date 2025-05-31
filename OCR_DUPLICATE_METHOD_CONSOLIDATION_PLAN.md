# OCR Duplicate Method Consolidation Plan

## Overview
This document outlines the systematic consolidation of duplicate methods across OCRCorrectionService partial class files to eliminate conflicts and maintain the most functional implementations.

## Current Duplicate Methods Analysis

### 1. GetFieldsByRegexNamedGroups
**Files:** 
- `OCRFieldMapping.cs` (Original - ENHANCED)
- `OCRMetadataExtractor.cs` (Enhanced version - REMOVE)

**Differences:**
- Enhanced version includes `IsRequired` property in FieldInfo
- Enhanced version has better error handling with try-catch
- Enhanced version has debug logging

**Action:** Keep enhanced version in OCRFieldMapping.cs, remove from OCRMetadataExtractor.cs

### 2. ExtractNamedGroupsFromRegex
**Files:**
- `OCRFieldMapping.cs` (Original)
- `OCRRegexManagement.cs` (Duplicate)
- `OCRMetadataExtractor.cs` (Enhanced - CORRUPTED, REMOVE)

**Differences:**
- All implementations are functionally identical
- Enhanced version uses fully qualified System.Text.RegularExpressions.Regex

**Action:** Keep version in OCRFieldMapping.cs, remove duplicates

### 3. FindLineByExactText
**Files:**
- `OCRCorrectionApplication.cs` (Comprehensive)
- `OCRMetadataExtractor.cs` (Local helper method)

**Differences:**
- Application version is more comprehensive
- MetadataExtractor version is a local helper

**Action:** Keep version in OCRCorrectionApplication.cs, remove local helper

### 4. FindLineBySimilarText
**Files:**
- `OCRCorrectionApplication.cs` (Comprehensive)
- `OCRMetadataExtractor.cs` (Local helper method)

**Differences:**
- Application version is more comprehensive
- MetadataExtractor version is a local helper

**Action:** Keep version in OCRCorrectionApplication.cs, remove local helper

### 5. CreateLineContextFromMetadata
**Files:**
- `OCRCorrectionApplication.cs` (Basic)
- `OCRMetadataExtractor.cs` (Enhanced with more properties)

**Differences:**
- MetadataExtractor version has more properties: LineName, LineRegex, RegexId, PartName, PartTypeId
- Application version is simpler

**Action:** Enhance the Application version with additional properties, remove MetadataExtractor version

### 6. CreateOrphanedLineContext
**Files:**
- `OCRCorrectionApplication.cs` (Standard)
- `OCRMetadataExtractor.cs` (Local helper)

**Differences:**
- Both are functionally similar
- MetadataExtractor version is a local helper

**Action:** Keep version in OCRCorrectionApplication.cs, remove local helper

### 7. GetOriginalLineText
**Files:**
- `OCRCorrectionApplication.cs` (Standard)
- `OCRUtilities.cs` (Alternative implementation)
- `OCRMetadataExtractor.cs` (Local helper)

**Differences:**
- Application version uses RemoveEmptyEntries
- Utilities version uses different split logic
- MetadataExtractor version is a local helper

**Action:** Keep Application version, remove others

### 8. CalculateTextSimilarity
**Files:**
- `OCRCorrectionApplication.cs` (Jaccard similarity)
- `OCRUtilities.cs` (Levenshtein distance)

**Differences:**
- Application version uses Jaccard similarity (word-based)
- Utilities version uses Levenshtein distance (character-based)

**Action:** Keep both but rename Utilities version to CalculateLevenshteinSimilarity

### 9. ExtractWindowText / ExtractWindowTextEnhanced
**Files:**
- `OCRDatabaseUpdates.cs` (Standard)
- `OCRMetadataExtractor.cs` (Enhanced with line numbers)

**Differences:**
- Enhanced version includes line numbers in output format
- Standard version is simpler

**Action:** Keep enhanced version in OCRDatabaseUpdates.cs, remove from OCRMetadataExtractor.cs

### 10. DetermineInvoiceType / DetermineInvoiceTypeEnhanced
**Files:**
- `OCRDatabaseUpdates.cs` (Standard)
- `OCRMetadataExtractor.cs` (Enhanced - identical)
- `ShipmentUtils.cs` (Different logic - invoice number based)

**Differences:**
- Database and MetadataExtractor versions are identical (file path based)
- ShipmentUtils version uses invoice number patterns

**Action:** Keep both - file path version in OCRDatabaseUpdates.cs, invoice number version in ShipmentUtils.cs

## Implementation Steps

### Phase 1: Fix Corrupted Methods
1. Fix corrupted ExtractNamedGroupsFromRegexEnhanced in OCRMetadataExtractor.cs
2. Fix corrupted IsFieldExistingInLineEnhanced method signature

### Phase 2: Remove Duplicate Methods
1. Remove GetFieldsByRegexNamedGroupsEnhanced from OCRMetadataExtractor.cs
2. Remove ExtractNamedGroupsFromRegex duplicates
3. Remove local helper methods from OCRMetadataExtractor.cs
4. Remove ExtractWindowTextEnhanced from OCRMetadataExtractor.cs
5. Remove DetermineInvoiceTypeEnhanced from OCRMetadataExtractor.cs

### Phase 3: Enhance Remaining Methods
1. Enhance CreateLineContextFromMetadata in OCRCorrectionApplication.cs
2. Enhance ExtractWindowText in OCRDatabaseUpdates.cs
3. Rename CalculateTextSimilarity in OCRUtilities.cs

### Phase 4: Update Method Calls
1. Search for all references to removed methods
2. Update calls to use consolidated versions
3. Test compilation

### Phase 5: Verification
1. Run diagnostics to ensure no conflicts
2. Test build to ensure all references are resolved
3. Verify functionality is preserved

## Expected Benefits
- Eliminate method name conflicts
- Reduce code duplication
- Maintain best functionality from all versions
- Improve maintainability
- Cleaner partial class structure
