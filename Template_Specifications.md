# Template Specifications

## Overview

This document provides comprehensive specifications for the OCR template system based on analysis of existing working templates. The system processes various document types from multiple suppliers, extracting structured data using regular expressions and mapping to the OCR EDMX domain model.

## Core Architecture

### Template Structure
Each template record consists of the following components:

| Column | Description | Usage |
|--------|-------------|-------|
| Id | Unique template identifier | Groups related patterns |
| Invoice | Supplier/Document name | Identifies the source |
| Part | Document section | Groups logical sections (Header, Details, etc.) |
| Line | Pattern description | Human-readable pattern name |
| RegEx | Regular expression pattern | Contains named capture groups |
| Key | Capture group name | Links RegEx groups to fields |
| Field | Target database field | Maps to OCR EDMX model |
| EntityType | Database table/entity | Specifies target table |
| IsRequired | Required field flag | Import validation control |
| DataType | Data type specification | Type conversion guidance |
| Value | Predefined value | Default/identifier values |
| AppendValues | Concatenation flag | Combines multiple matches |
| FieldId | Database field ID | Internal reference |
| ParentId | Parent relationship | Hierarchical structure |
| LineId | Line group ID | Groups related patterns |

## EntityType Mapping to OCR EDMX Domain Model

### Primary Entity Types

#### 1. **Invoice Processing Entities**
- **`Invoice`** - Main invoice header information
- **`InvoiceDetails`** - Line items and product details
- **`EntryData`** - Generic invoice data structure
- **`EntryDataDetails`** - Generic invoice line items

#### 2. **Shipping & Logistics Entities**
- **`ShipmentBL`** - Bill of Lading header information
- **`ShipmentBLDetails`** - Bill of Lading line items
- **`ShipmentFreight`** - Freight invoice headers
- **`ShipmentFreightDetails`** - Freight invoice line items
- **`ShipmentManifest`** - Shipping manifest information
- **`ShipmentRider`** - Shipping rider documents
- **`ShipmentRiderDetails`** - Shipping rider line items

#### 3. **Supporting Entities**
- **`ExtraInfo`** - Additional document metadata
- **`Suppliers`** - Supplier information
- **`SimplifiedDeclaration`** - Customs declaration data

#### 4. **System Entities**
- **`NULL`** - No database mapping (processing only)

### Header-Details Relationships

The system maintains parent-child relationships between header and detail entities:

```
ShipmentBL (Header) → ShipmentBLDetails (Line Items)
ShipmentFreight (Header) → ShipmentFreightDetails (Line Items)
ShipmentManifest (Header) → [No details entity]
Invoice (Header) → InvoiceDetails (Line Items)
EntryData (Header) → EntryDataDetails (Line Items)
```

## Data Type System

### Standard Data Types
- **`String`** - Text data
- **`Number`/`Numeric`** - Numerical values
- **`Date`** - Date fields
- **`English Date`** - Dates in English format requiring parsing

### Field Mapping Patterns

#### Common Field Mappings by EntityType

**Invoice/EntryData Entity:**
- `InvoiceNo` → `InvoiceNo` or `EntryDataId`
- `InvoiceDate` → `InvoiceDate` or `EntryDataDate`
- `InvoiceTotal` → `InvoiceTotal`
- `SubTotal` → `SubTotal`
- `Currency` → `Currency`
- `SupplierCode` → `SupplierCode`
- `PONumber` → `PONumber`

**InvoiceDetails/EntryDataDetails Entity:**
- `ItemNumber`/`ProductCode` → `ItemNumber`
- `Description`/`ItemDescription` → `ItemDescription`
- `Quantity`/`Shipped` → `Quantity`
- `UnitPrice`/`Price`/`Cost` → `Cost`
- `ExtendedPrice`/`TotalCost` → `TotalCost`
- `Units` → `Units`

**ShipmentBL Entity:**
- `BLNumber` → `BLNumber`
- `Vessel` → `Vessel`
- `Voyage` → `Voyage`
- `Container` → `Container`
- `WeightKG` → `WeightKG`
- `VolumeM3` → `VolumeM3`

## Multi-Supplier and Multi-Format Support

### Unified Entity Mapping Strategy

The system handles multiple scenarios:

1. **Multiple Suppliers → Same EntityType**
   - Amazon, Budget Invoice, International → `Invoice`
   - CMP, MPI, RedTree → `Invoice`
   - All map to same database structure

2. **Multiple Formats per Supplier → Same EntityType**
   - Amazon has: `InvoiceLine`, `InvoiceLine2`, `InvoiceLine3` → all map to `InvoiceDetails`
   - MARINECO has: `Details`, `Details2` → both map to `InvoiceDetails`

3. **Cross-Document Type Consistency**
   - Freight invoices, regular invoices, and manifest documents all contribute to unified data model
   - Consistent field naming across document types

## IsRequired Field Strategy

### Smart Required Field Management

The system uses an intelligent approach to handle required fields across multiple formats:

#### Strategy: Multiple Options = Required False
When a critical field (like `InvoiceNo`) has multiple regex patterns for the same supplier, **all patterns are marked as `IsRequired=0`** to prevent import failures.

**Example: MARINECO InvoiceNo**
```
MARINECO → InvoiceNo → IsRequired=0 (Pattern 1)
MARINECO → InvoiceNo → IsRequired=0 (Pattern 2)
```

**Rationale:** Since at least one pattern will match and populate the field, marking all as non-required prevents the import from failing if only one pattern matches.

#### Standard Required Fields by EntityType

**Invoice Entity (IsRequired=1):**
- InvoiceNo (unless multiple patterns exist)
- InvoiceTotal
- SupplierCode or Name

**InvoiceDetails Entity (IsRequired=1):**
- ItemNumber or ItemDescription
- Quantity
- Cost or TotalCost

**ShipmentBL Entity (IsRequired=1):**
- BLNumber
- WeightKG

### Required Field Override Examples
```
Most suppliers: InvoiceNo → IsRequired=1
3M: InvoiceNo → IsRequired=0 (multiple patterns)
MARINECO: InvoiceNo → IsRequired=0 (multiple patterns)
```

## Value Column Usage

### Predefined Values and Identifiers

The `Value` column serves two primary purposes:

#### 1. **Supplier Identification**
Used to set consistent supplier names regardless of document variations:
```
Supplier: "Budget Invoice" → Value: "Budget Invoice"
Supplier: "Amazon" → Value: "Amazon"
Supplier: "Xylem" → Value: "Xylem"
```

#### 2. **Currency Defaults**
Provides default currency codes for regional suppliers:
```
Portage Freight → Currency → Value: "XCD"
CAPTAIN'S FASTENERS → Currency → Value: "USD"
```

#### 3. **Fixed Values for Processing**
Sets required values that don't come from RegEx extraction:
```
Manifest → Name → Value: "Manifest"
XYLEM → SupplierCode → Value: "XYLEM"
```

### Implied Error Detection
The Value column enables validation by providing expected values that can be compared against extracted data to detect OCR errors or format changes.

## AppendValues Functionality

### Concatenation Control

The `AppendValues` field controls how multiple regex matches are combined:

#### AppendValues = 1 (Concatenate)
Multiple matches for the same field are concatenated together:
```
Amazon → ItemDescription → AppendValues=1
  Match 1: "Marine Battery"
  Match 2: "Deep Cycle 12V"
  Result: "Marine Battery Deep Cycle 12V"
```

#### AppendValues = 0 or NULL (Replace)
Each match replaces the previous value (standard behavior):
```
Most fields → AppendValues=NULL
  Match 1: "Marine Battery"
  Match 2: "Deep Cycle 12V"  
  Result: "Deep Cycle 12V" (last match wins)
```

### Common AppendValues Use Cases
- **ItemDescription**: Combining product name and specifications
- **PackagesNo**: Summing package counts from multiple lines
- **Weight fields**: Combining weights from different sources
- **Volume fields**: Accumulating volume measurements

## Regular Expression Patterns

### Pattern Structure
All regex patterns use named capture groups that correspond to the `Key` column:

```regex
(?<InvoiceNo>\d+)\s+(?<InvoiceDate>[\d/]+)\s+(?<PONumber>[\w/]+)
```

### Key Naming Conventions
- **InvoiceNo**: Invoice number
- **InvoiceDate**: Invoice date
- **InvoiceTotal**: Total amount
- **ItemNumber/ProductCode**: Product identifier
- **Description/ItemDescription**: Product description
- **Quantity/Shipped**: Quantity values
- **UnitPrice/Price/Cost**: Unit pricing
- **ExtendedPrice/TotalCost**: Line totals

### Multi-Line Pattern Support
Some patterns span multiple lines using `[\r\n]` and multiline matching:

```regex
^(?<SupplierName>Budget.*)\r\n\r\n(?<SupplierAddress>[A-Z\s\d\r\n\.\,:\(\)-]*)^Invoice\r\n\r\n
```

## Document Type Classifications

### Invoice Documents
- **Standard Invoices**: Product sales, services
- **Freight Invoices**: Shipping and logistics
- **Amazon Orders**: E-commerce purchases
- **International Invoices**: Cross-border transactions

### Shipping Documents
- **Bills of Lading**: Cargo manifests
- **Freight Bills**: Transportation charges
- **Shipping Manifests**: Cargo declarations
- **Packing Lists**: Item inventories

### Supporting Documents
- **Customs Declarations**: Import/export forms
- **Delivery Receipts**: Proof of delivery
- **Picking Lists**: Warehouse documents

## Implementation Guidelines

### Template Creation Rules

#### 1. **EntityType Selection**
- Use `Invoice` for standard commercial invoices
- Use `InvoiceDetails` for line items
- Use `ShipmentBL` for bills of lading
- Use `ExtraInfo` for metadata that doesn't fit primary entities

#### 2. **Field Mapping Standards**
- Map similar concepts to same field names across suppliers
- Use consistent data types for similar fields
- Preserve original field semantics when possible

#### 3. **Required Field Strategy**
- Mark fields as required only if single pattern exists
- Use `IsRequired=0` for multiple pattern scenarios
- Ensure at least one critical identifier is always required

#### 4. **Value Column Usage**
- Set supplier names for consistent identification
- Provide currency defaults for regional suppliers
- Use for validation and error detection

#### 5. **AppendValues Implementation**
- Use `AppendValues=1` for description fields that benefit from concatenation
- Use `AppendValues=1` for cumulative numeric fields
- Default to `AppendValues=NULL` for replacement behavior

### Testing and Validation

#### Template Testing Checklist
1. **Pattern Matching**: Verify regex captures expected data
2. **Entity Mapping**: Confirm correct EntityType assignment
3. **Required Fields**: Validate required field logic
4. **Data Types**: Test type conversion accuracy
5. **Multi-Format**: Test all format variations for supplier
6. **Edge Cases**: Handle partial matches and errors

#### Quality Assurance
- Test with actual document samples
- Verify database field mappings
- Validate business rule compliance
- Check for regression in existing templates

## Migration and Maintenance

### Adding New Suppliers
1. Analyze document format and identify patterns
2. Map to existing EntityTypes where possible
3. Follow established field naming conventions
4. Implement required field strategy
5. Test thoroughly with sample documents

### Template Updates
1. Preserve existing functionality
2. Add new patterns with `IsRequired=0` initially
3. Gradually migrate to new patterns
4. Maintain backward compatibility
5. Document changes and rationale

### Performance Considerations
- Optimize regex patterns for performance
- Use specific patterns over generic ones
- Consider pattern ordering for efficiency
- Monitor processing times and adjust as needed

## Conclusion

This template system provides a flexible, scalable approach to processing diverse document formats while maintaining data consistency through unified entity mapping. The intelligent handling of required fields, multi-format support, and extensible architecture enables reliable processing of complex business documents across multiple suppliers and document types.

The key to success is understanding that multiple suppliers and formats all contribute to the same underlying data model, with the system handling variations through intelligent field mapping, value defaults, and flexible validation rules.