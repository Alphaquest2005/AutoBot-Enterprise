# JSON Invoice Data Extraction Prompt (Production-Aligned)

## Purpose
Extract structured invoice data from OCR text using exact ShipmentInvoice model field names and Caribbean Customs mapping rules for AI validation testing against DeepSeek detection.

## Production Model Alignment
This prompt extracts data in the exact format expected by the production DeepSeek system, using ShipmentInvoice field names and Caribbean Customs business rules.

## Prompt Template

```
You are an expert invoice data extraction system that follows Caribbean Customs rules and production OCR section precedence logic. Extract all relevant invoice information from the provided OCR text and output a structured JSON object that matches the production ShipmentInvoice model.

**OCR TEXT TO ANALYZE:**
{INVOICE_TEXT}

**OCR SECTION PRECEDENCE RULES (CRITICAL - Apply SetPartLineValues Logic):**

The OCR text contains three sections with PRIORITY RANKING:
1. **Single Column** (Priority 1 - HIGHEST) - marked with `------------------------------------------Single Column-------------------------`
2. **Ripped Text** (Priority 2 - MEDIUM) - marked with `------------------------------------------Ripped Text-------------------------`  
3. **SparseText** (Priority 3 - LOWEST) - marked with `------------------------------------------SparseText-------------------------`

**FIELD EXTRACTION PRECEDENCE:**
- For NON-AGGREGATABLE fields (InvoiceNo, SupplierName, etc.): Use value from HIGHEST priority section that contains the field
- For AGGREGATABLE fields (FreeShipping, TotalDeduction, TotalInsurance, Tax): Collect from ALL sections but DEDUPLICATE identical values

**AGGREGATABLE FIELDS:** FreeShipping, TotalDeduction, Deduction, Discount, Tax, TotalOtherCost, TotalInternalFreight, Freight, Shipping, GiftCard, TotalInsurance, Insurance, Coupon

**DEDUPLICATION LOGIC:**
- If "Free Shipping: -$6.53" appears in both Single Column AND SparseText sections, count it only ONCE
- If different amounts appear (e.g., Single: "$6.53", Sparse: "$0.46"), sum both as they represent different items
- This mirrors the production RefineFieldsByPrecedence() method

**CARIBBEAN CUSTOMS FIELD MAPPING RULES:**

1. **TotalInsurance** = Customer-caused reductions (stored as NEGATIVE values):
   - Gift Cards → TotalInsurance = -amount  
   - Store Credits → TotalInsurance = -amount
   - Credits → TotalInsurance = -amount
   - [G Credit → TotalInsurance = -amount
   - Customer payments/refunds → TotalInsurance = -amount
   - Account credits → TotalInsurance = -amount

2. **TotalDeduction** = Supplier-caused reductions (stored as POSITIVE values):
   - Free Shipping → TotalDeduction = +amount
   - Discounts → TotalDeduction = +amount  
   - Promotional reductions → TotalDeduction = +amount

3. **Financial Balance Formula:**
   SubTotal + TotalInternalFreight + TotalOtherCost + TotalInsurance - TotalDeduction = InvoiceTotal

**SHIPMENTINVOICE FIELD EXTRACTION:**

**Header Fields:**
- InvoiceNo (Order ID/Invoice Number)
- InvoiceDate (convert to YYYY-MM-DD format)
- SupplierName (Vendor/Company name)
- Currency (USD, CNY, XCD, etc.)
- EmailId (source filename)

**Financial Fields (all as double values):**
- InvoiceTotal (final total amount)
- SubTotal (items subtotal before shipping/tax)
- TotalInternalFreight (shipping/freight costs)
- TotalOtherCost (taxes, fees, duties, surcharges)
- TotalInsurance (customer reductions as NEGATIVE values)
- TotalDeduction (supplier reductions as POSITIVE values)

**OUTPUT FORMAT:**
Return ONLY a valid JSON object matching the ShipmentInvoice model:

```json
{
  "invoiceHeader": {
    "InvoiceNo": "string",
    "InvoiceDate": "YYYY-MM-DD",
    "SupplierName": "string", 
    "SupplierCode": "string",
    "Currency": "string",
    "EmailId": "filename.txt"
  },
  "financialFields": {
    "InvoiceTotal": 0.00,
    "SubTotal": 0.00,
    "TotalInternalFreight": 0.00,
    "TotalOtherCost": 0.00,
    "TotalInsurance": 0.00,
    "TotalDeduction": 0.00
  },
  "calculatedValidation": {
    "CalculatedTotal": 0.00,
    "BalanceCheck": 0.00,
    "ValidationPassed": true,
    "Formula": "SubTotal + TotalInternalFreight + TotalOtherCost + TotalInsurance - TotalDeduction"
  },
  "extractionMetadata": {
    "SourceText": "First 100 chars of OCR text...",
    "ExtractedFieldCount": 0,
    "TotalFieldCount": 11,
    "ConfidenceLevel": "high|medium|low",
    "ProcessingNotes": ["Caribbean Customs rules applied", "Multiple orders detected"]
  }
}
```

**CRITICAL EXTRACTION RULES:**

1. **Sign Convention (Caribbean Customs):**
   - TotalInsurance: Always NEGATIVE for customer reductions
   - TotalDeduction: Always POSITIVE for supplier reductions

2. **Field Mapping Examples:**
   - "Gift Card: -$6.99" → TotalInsurance = -6.99
   - "Credit: -$0.02" → TotalInsurance = -0.02
   - "You have applied $0.02 [G Credit" → TotalInsurance = -0.02
   - "Store Credit: -$5.00" → TotalInsurance = -5.00
   - "Free Shipping: -$6.53" → TotalDeduction = 6.53
   - "Sales Tax: $11.34" → TotalOtherCost = 11.34
   - "Shipping: $6.99" → TotalInternalFreight = 6.99

3. **Balance Validation:**
   - Calculate: SubTotal + TotalInternalFreight + TotalOtherCost + TotalInsurance - TotalDeduction
   - Compare with InvoiceTotal
   - BalanceCheck = CalculatedTotal - InvoiceTotal (should be 0.00)
   - ValidationPassed = true only if |BalanceCheck| ≤ 0.01

4. **Multiple Orders:**
   - If multiple orders in document, extract the first/primary order
   - Note additional orders in ProcessingNotes

5. **Missing Fields:**
   - Use null for missing numerical values
   - Use empty string for missing text values
   - Document missing fields in ProcessingNotes

**VALIDATION REQUIREMENT:**
The extracted data must pass the Caribbean Customs balance validation formula exactly as used in the production DeepSeek system.
```

## Usage Instructions

1. Replace `{INVOICE_TEXT}` with the actual OCR extracted text
2. Send to AI model (Claude, DeepSeek, etc.)
3. Parse JSON response for validation testing
4. Compare extracted data with AI detection results

## Expected Benefits

- **Structured Data**: Clean JSON format for programmatic comparison
- **Financial Validation**: Built-in balance checking
- **Metadata Tracking**: Confidence levels and extraction notes
- **Automation Ready**: Can process all 28 text files in batch
- **DeepSeek Comparison**: Direct field-by-field comparison capability

## Integration with Test Framework

The JSON output will be used by:
1. **Reference Data Generation**: Create ground truth datasets
2. **AI Validation Tests**: Compare DeepSeek detection against JSON
3. **Performance Metrics**: Calculate field detection accuracy
4. **Bias Detection**: Identify vendor-specific detection patterns