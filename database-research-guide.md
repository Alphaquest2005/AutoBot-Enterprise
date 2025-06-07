# Database Research Guide - OCR Template Analysis

## Overview
This guide provides best practices for researching the OCR database to prevent common mistakes like wrong column names, incorrect joins, and missing relationships.

## Common Mistakes to Avoid

### 1. Column Name Errors
**Problem**: Using incorrect column names like `RegularExpressionId` instead of `RegExId`
**Solution**: Always check schema first

```javascript
// ALWAYS do this first
const schema = await sql.query(`
  SELECT COLUMN_NAME, DATA_TYPE, IS_NULLABLE
  FROM INFORMATION_SCHEMA.COLUMNS 
  WHERE TABLE_NAME = 'OCR-Lines'
  ORDER BY ORDINAL_POSITION
`);
console.table(schema.recordset);
```

### 2. Incorrect Join Relationships
**Problem**: Using `p.InvoiceId` instead of `p.TemplateId`
**Solution**: Check foreign key relationships

```javascript
// Check foreign keys
const fks = await sql.query(`
  SELECT 
    fk.name AS FK_Name,
    tp.name AS Parent_Table,
    cp.name AS Parent_Column,
    tr.name AS Referenced_Table,
    cr.name AS Referenced_Column
  FROM sys.foreign_keys fk
  INNER JOIN sys.foreign_key_columns fkc ON fk.object_id = fkc.constraint_object_id
  INNER JOIN sys.tables tp ON fkc.parent_object_id = tp.object_id
  INNER JOIN sys.columns cp ON fkc.parent_object_id = cp.object_id AND fkc.parent_column_id = cp.column_id
  INNER JOIN sys.tables tr ON fkc.referenced_object_id = tr.object_id
  INNER JOIN sys.columns cr ON fkc.referenced_object_id = cr.object_id AND fkc.referenced_column_id = cr.column_id
  WHERE tp.name = 'OCR-Parts'
`);
```

### 3. Missing Table Discovery
**Problem**: Assuming table names without verification
**Solution**: Discover all OCR tables first

```javascript
// Discover all OCR tables
const ocrTables = await sql.query(`
  SELECT TABLE_NAME, TABLE_TYPE
  FROM INFORMATION_SCHEMA.TABLES 
  WHERE TABLE_NAME LIKE 'OCR%'
  ORDER BY TABLE_NAME
`);
```

## Database Schema Reference

### Key OCR Tables
Based on schema discovery, here are the verified table structures:

#### OCR-Invoices
- `Id` (int, NOT NULL)
- `Name` (nvarchar)
- `FileTypeId` (int)
- `IsActive` (bit)
- `ApplicationSettingsId` (int)

#### OCR-Parts
- `Id` (int, NOT NULL)
- `TemplateId` (int, NOT NULL) ← **Key: Links to OCR-Invoices.Id**
- `PartTypeId` (int, NOT NULL)

#### OCR-Lines
- `Id` (int, NOT NULL)
- `PartId` (int, NOT NULL) ← **Key: Links to OCR-Parts.Id**
- `RegExId` (int, NOT NULL) ← **Note: NOT RegularExpressionId**
- `Name` (nvarchar, NOT NULL)
- `ParentId` (int, NULLABLE)
- `DistinctValues` (bit, NULLABLE)
- `IsColumn` (bit, NULLABLE)
- `IsActive` (bit, NULLABLE)
- `Comments` (nvarchar, NULLABLE)

#### OCR-Fields
- `Id` (int, NOT NULL)
- `LineId` (int, NOT NULL) ← **Key: Links to OCR-Lines.Id**
- `Field` (nvarchar)
- `FieldValue` (nvarchar)

#### OCR-PartTypes
- `Id` (int, NOT NULL)
- `Name` (nvarchar)

## Correct Join Patterns

### Template to Parts to Lines to Fields
```sql
SELECT 
  i.Name as InvoiceName,
  p.Id as PartId,
  pt.Name as PartTypeName,
  l.Id as LineId,
  l.Name as LineName,
  f.Field as FieldName
FROM [OCR-Invoices] i
JOIN [OCR-Parts] p ON i.Id = p.TemplateId          -- ✅ Correct
LEFT JOIN [OCR-PartTypes] pt ON p.PartTypeId = pt.Id
JOIN [OCR-Lines] l ON p.Id = l.PartId              -- ✅ Correct
JOIN [OCR-Fields] f ON l.Id = f.LineId             -- ✅ Correct
WHERE i.Name IN ('Amazon', 'Tropical Vendors')
```

### Common Wrong Patterns to Avoid
```sql
-- ❌ WRONG: Using InvoiceId instead of TemplateId
JOIN [OCR-Parts] p ON i.Id = p.InvoiceId

-- ❌ WRONG: Using RegularExpressionId instead of RegExId
SELECT l.RegularExpressionId FROM [OCR-Lines] l

-- ❌ WRONG: Using LineNumber instead of Name
SELECT l.LineNumber FROM [OCR-Lines] l
```

## Research Workflow

### Step 1: Schema Discovery
Always start by discovering table schemas and relationships:

```javascript
// 1. Find all OCR tables
const tables = await getOCRTables();

// 2. Get schema for each key table
const schemas = {};
for (const table of keyTables) {
  schemas[table] = await getTableSchema(table);
}

// 3. Get foreign key relationships
const relationships = await getForeignKeys(table);
```

### Step 2: Template Analysis
Compare templates systematically:

```javascript
// 1. Basic template info
const templates = await getTemplateInfo(['Amazon', 'Tropical Vendors']);

// 2. Parts comparison
const parts = await getPartsConfig(['Amazon', 'Tropical Vendors']);

// 3. Lines comparison
const lines = await getLinesConfig(['Amazon', 'Tropical Vendors']);

// 4. Fields comparison
const fields = await getFieldsConfig(['Amazon', 'Tropical Vendors']);
```

### Step 3: Root Cause Analysis
Look for specific issues:

```javascript
// 1. Check for missing InvoiceDetails field
const invoiceDetailsFields = await findInvoiceDetailsFields();

// 2. Check child fields configuration
const childFields = await getChildFieldsConfig();

// 3. Compare field counts and types
const summary = await getTemplateSummary();
```

## Key Findings Template

When analyzing templates, always document:

1. **Template Structure**:
   - Number of parts per template
   - Part types (Header, Details, InvoiceLine, etc.)
   - Lines per part
   - Fields per line

2. **Critical Differences**:
   - Missing parts in failing template
   - Missing fields (especially InvoiceDetails)
   - Different part types

3. **Root Cause Hypothesis**:
   - Based on data comparison
   - Supported by test logs
   - Actionable fix recommendations

## Usage Examples

### Quick Template Comparison
```bash
node database-research-helper.js
```

### Custom Analysis
```javascript
import { getTableSchema, getForeignKeys } from './database-research-helper.js';

// Get specific table info
const schema = await getTableSchema('OCR-Fields');
const fks = await getForeignKeys('OCR-Parts');
```

## Troubleshooting Common Issues

### Issue: "Invalid column name"
1. Check table schema first
2. Verify column exists
3. Check for typos in column names

### Issue: "Invalid object name"
1. Verify table exists in database
2. Check table name spelling
3. Ensure proper database connection

### Issue: No data returned
1. Check join conditions
2. Verify WHERE clause filters
3. Check if data exists for test cases

### Issue: Unexpected results
1. Verify foreign key relationships
2. Check for NULL values in joins
3. Use LEFT JOIN vs INNER JOIN appropriately

## Best Practices

1. **Always verify schema before writing queries**
2. **Use consistent naming conventions**
3. **Document your findings clearly**
4. **Test queries incrementally**
5. **Save working queries for reuse**
6. **Include error handling in scripts**
7. **Log intermediate results for debugging**
