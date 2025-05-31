// File: OCRCorrectionService/OCRPromptCreation.cs
using System;
using System.Text.Json;
using System.Linq;
using EntryDataDS.Business.Entities;

namespace WaterNut.DataSpace
{
    public partial class OCRCorrectionService
    {
        #region Enhanced Prompt Creation Methods

        /// <summary>
        /// Enhanced prompt that detects both format errors and omissions with context lines
        /// </summary>
        private string CreateHeaderErrorDetectionPrompt(ShipmentInvoice invoice, string fileText)
        {
            var headerData = new
            {
                InvoiceNo = invoice.InvoiceNo,
                InvoiceDate = invoice.InvoiceDate,
                InvoiceTotal = invoice.InvoiceTotal,
                SubTotal = invoice.SubTotal,
                TotalInternalFreight = invoice.TotalInternalFreight,
                TotalOtherCost = invoice.TotalOtherCost,
                TotalInsurance = invoice.TotalInsurance,
                TotalDeduction = invoice.TotalDeduction,
                Currency = invoice.Currency,
                SupplierName = invoice.SupplierName,
                SupplierAddress = invoice.SupplierAddress
            };

            var headerJson = JsonSerializer.Serialize(headerData, new JsonSerializerOptions { WriteIndented = true });

            return $@"OCR HEADER FIELD ERROR DETECTION WITH OMISSION ANALYSIS:

Compare the extracted header data with the original invoice text and identify ALL discrepancies including missing fields.

EXTRACTED HEADER DATA:
{headerJson}

ORIGINAL TEXT:
{CleanTextForAnalysis(fileText)}

CRITICAL REQUIREMENTS FOR EACH ERROR:
1. Provide EXACT line text where the correct value appears in the original document
2. Provide context lines (5 before + 5 after) for regex creation assistance
3. Indicate if this requires single-line or multi-line regex processing
4. Distinguish between FORMAT ERRORS (wrong format) and OMISSIONS (missing entirely)

ERROR TYPES TO DETECT:
- ""omission"": Field value exists in text but wasn't extracted (ExtractedValue will be null/empty)
- ""format_error"": Value extracted but wrong format (ExtractedValue differs from correct value)
- ""decimal_separator"": Comma vs period issues
- ""character_confusion"": OCR character substitution
- ""calculation_error"": Mathematical inconsistencies

DEDUCTION IDENTIFICATION RULES (CRITICAL FOR OMISSIONS):
Look for these terms and treat as TotalDeduction omissions if not extracted:
- ""Gift card applied"", ""Gift certificate used"", ""GC Applied""
- ""Amazon gift card"", ""Store gift card"", ""Gift balance""
- ""Store credit"", ""Account credit"", ""Credit applied""
- ""Promotional credit"", ""Promo code"", ""Coupon applied""
- ""Paid with gift card"", ""Gift card payment""

RESPONSE FORMAT (JSON only - follow this EXACT structure):
{{
  ""errors"": [
    {{
      ""field"": ""FieldName"",
      ""extracted_value"": ""CurrentValue"",
      ""correct_value"": ""CorrectValueFromText"",
      ""line_text"": ""EXACT line from original text where correct value appears"",
      ""line_number"": 15,
      ""context_lines_before"": [
        ""Line 10: text content"",
        ""Line 11: text content"",
        ""Line 12: text content"",
        ""Line 13: text content"",
        ""Line 14: text content""
      ],
      ""context_lines_after"": [
        ""Line 16: text content"",
        ""Line 17: text content"",
        ""Line 18: text content"",
        ""Line 19: text content"",
        ""Line 20: text content""
      ],
      ""requires_multiline_regex"": false,
      ""confidence"": 0.95,
      ""error_type"": ""omission"",
      ""reasoning"": ""Field exists in text but wasn't extracted - requires new regex pattern""
    }}
  ]
}}

EXAMPLE FOR OMISSION (missing field):
{{
  ""field"": ""TotalDeduction"",
  ""extracted_value"": """",
  ""correct_value"": ""6.99"",
  ""line_text"": ""Gift Card Applied: -$6.99"",
  ""line_number"": 28,
  ""context_lines_before"": [
    ""Line 23: Item 3: Widget C"",
    ""Line 24: Quantity: 1  Price: $15.00"",
    ""Line 25: Subtotal: $150.00"",
    ""Line 26: Shipping: $10.00"",
    ""Line 27: Tax: $12.00""
  ],
  ""context_lines_after"": [
    ""Line 29: Final Total: $165.01"",
    ""Line 30: Payment Method: Credit Card"",
    ""Line 31: Card ending in: 1234"",
    ""Line 32: Authorization: ABC123"",
    ""Line 33: Thank you for your order!""
  ],
  ""requires_multiline_regex"": false,
  ""error_type"": ""omission"",
  ""reasoning"": ""Gift card deduction exists in text but wasn't extracted by current OCR patterns""
}}

EXAMPLE FOR FORMAT ERROR (wrong format):
{{
  ""field"": ""InvoiceTotal"",
  ""extracted_value"": ""123,45"",
  ""correct_value"": ""123.45"",
  ""line_text"": ""Total: $123,45"",
  ""line_number"": 35,
  ""context_lines_before"": [],
  ""context_lines_after"": [],
  ""requires_multiline_regex"": false,
  ""error_type"": ""decimal_separator"",
  ""reasoning"": ""European decimal comma should be converted to period""
}}

IMPORTANT NOTES:
- line_text must be EXACT text from the original document
- For omissions, ALWAYS provide context_lines to help create extraction regex
- For simple format corrections, context_lines can be empty arrays
- Line numbers in context_lines should be actual line numbers from the document
- Focus on fields that affect TotalsZero calculation: InvoiceTotal, SubTotal, TotalDeduction, etc.

Return empty array if no errors: {{""errors"": []}}";
        }

        /// <summary>
        /// Enhanced product-level error detection with omission support
        /// </summary>
        private string CreateProductErrorDetectionPrompt(ShipmentInvoice invoice, string fileText)
        {
            var productData = invoice.InvoiceDetails?.Select(d => new
            {
                LineNumber = d.LineNumber,
                ItemDescription = d.ItemDescription,
                Quantity = d.Quantity,
                Cost = d.Cost,
                TotalCost = d.TotalCost,
                Discount = d.Discount,
                Units = d.Units
            }).ToList();

            var productsJson = JsonSerializer.Serialize(productData, new JsonSerializerOptions { WriteIndented = true });

            return $@"OCR PRODUCT DATA ERROR DETECTION WITH OMISSION ANALYSIS:

Compare the extracted product data with the original invoice text and identify ALL discrepancies including missing fields.

EXTRACTED PRODUCT DATA:
{productsJson}

ORIGINAL TEXT:
{CleanTextForAnalysis(fileText)}

CRITICAL REQUIREMENTS FOR EACH ERROR:
1. Provide EXACT line text where the correct value appears
2. Provide context lines (5 before + 5 after) for multi-line pattern detection
3. Indicate if this requires single-line or multi-line regex processing
4. Distinguish between FORMAT ERRORS and OMISSIONS

VALIDATION FOCUS:
1. QUANTITIES: Verify each item quantity matches the text exactly
   - Watch for: 1↔l, 5↔S, 6↔G, 8↔B, 0↔O character confusion
   - Check: Reasonable quantities (not negative, not extremely large)
   - OMISSIONS: Missing quantity fields in line items

2. UNIT PRICES: Verify unit costs are correct
   - Watch for: Decimal separators (12,50 vs 12.50)
   - Check: Currency formatting ($12.50 vs $1250)
   - Validate: Prices are reasonable (not negative, not zero for products)
   - OMISSIONS: Missing cost fields in line items

3. LINE TOTALS: Verify calculations and detect missing totals
   - Formula: (Quantity × Unit Price) - Discount = Line Total
   - Check: Math is correct within $0.01
   - OMISSIONS: Missing TotalCost calculations

4. ITEM DESCRIPTIONS: Verify product names and detect missing descriptions
   - Check: No truncated descriptions
   - Watch for: Character substitutions that change meaning
   - OMISSIONS: Missing ItemDescription fields

RESPONSE FORMAT (JSON only - follow this EXACT structure):
{{
  ""errors"": [
    {{
      ""field"": ""InvoiceDetail_Line1_Quantity"",
      ""extracted_value"": ""1Z"",
      ""correct_value"": ""12"",
      ""line_text"": ""EXACT line from original text where correct value appears"",
      ""line_number"": 15,
      ""context_lines_before"": [
        ""Line 10: previous line"",
        ""Line 11: previous line"",
        ""Line 12: previous line"",
        ""Line 13: previous line"",
        ""Line 14: previous line""
      ],
      ""context_lines_after"": [
        ""Line 16: next line"",
        ""Line 17: next line"",
        ""Line 18: next line"",
        ""Line 19: next line"",
        ""Line 20: next line""
      ],
      ""requires_multiline_regex"": false,
      ""confidence"": 0.90,
      ""error_type"": ""character_confusion"",
      ""reasoning"": ""OCR confused 'Z' with '2' in quantity field""
    }}
  ]
}}

EXAMPLE FOR LINE ITEM OMISSION:
{{
  ""field"": ""InvoiceDetail_Line2_TotalCost"",
  ""extracted_value"": """",
  ""correct_value"": ""25.98"",
  ""line_text"": ""Item 2: Premium Widget  Qty: 2  @$12.99  Total: $25.98"",
  ""line_number"": 45,
  ""context_lines_before"": [
    ""Line 40: Item 1: Basic Widget"",
    ""Line 41: Qty: 1  @$9.99  Total: $9.99"",
    ""Line 42: "",
    ""Line 43: Item 2: Premium Widget"",
    ""Line 44: Description: High-quality premium widget""
  ],
  ""context_lines_after"": [
    ""Line 46: Warranty: 2 years included"",
    ""Line 47: "",
    ""Line 48: Item 3: Deluxe Widget"",
    ""Line 49: Qty: 1  @$19.99"",
    ""Line 50: Total: $19.99""
  ],
  ""requires_multiline_regex"": true,
  ""error_type"": ""omission"",
  ""reasoning"": ""Line total exists in text but wasn't extracted - may require multi-line pattern""
}}

IMPORTANT NOTES:
- line_text must be EXACT text from the original document
- For omissions in line items, ALWAYS provide context_lines for pattern creation
- Multi-line patterns may be needed for complex line item structures
- Focus on fields critical for mathematical validation
- Field names use format: InvoiceDetail_Line{N}_{FieldName}

Return empty array if no errors: {{""errors"": []}}";
        }

        /// <summary>
        /// Creates prompt for DeepSeek to generate new regex patterns for omissions
        /// </summary>
        public string CreateRegexCreationPrompt(CorrectionResult correction, LineContext lineContext)
        {
            var existingNamedGroups = lineContext.FieldsInLine?.Select(f => f.Key).ToList() ?? new List<string>();

            return $@"CREATE REGEX PATTERN FOR OCR FIELD EXTRACTION:

A field '{correction.FieldName}' with value '{correction.NewValue}' was found in the invoice text but not extracted by current OCR processing.

CURRENT LINE REGEX: {lineContext.RegexPattern ?? "None"}
EXISTING NAMED GROUPS: {string.Join(", ", existingNamedGroups)}

TARGET LINE:
{correction.LineText}

FULL CONTEXT PROVIDED BY ANALYSIS:
{string.Join("\n", correction.ContextLinesBefore)}
>>> TARGET LINE {correction.LineNumber}: {correction.LineText} <<<
{string.Join("\n", correction.ContextLinesAfter)}

FIELD DETAILS:
- Field Name: {correction.FieldName}
- Expected Value: {correction.NewValue}
- DeepSeek Suggests Multiline: {correction.RequiresMultilineRegex}
- Error Type: {correction.CorrectionType}
- Context Lines Available: {correction.ContextLinesBefore.Count + correction.ContextLinesAfter.Count + 1} total

REQUIREMENTS:
1. Analyze the FULL context provided - use as much or as little as needed
2. Create the most robust regex pattern for this field extraction
3. Use named capture group: (?<{correction.FieldName}>pattern)
4. YOU decide if single-line or multi-line approach is best based on the context
5. YOU determine the optimal max_lines value (no artificial limits)
6. Choose strategy: modify existing line regex OR create completely new line

RESPONSE FORMAT (JSON only):
{{
  ""strategy"": ""modify_existing_line"" OR ""create_new_line"",
  ""regex_pattern"": ""(?<{correction.FieldName}>your_pattern_here)"",
  ""complete_line_regex"": ""full regex if modifying existing line (preserve all existing named groups)"",
  ""is_multiline"": true/false,
  ""max_lines"": number,
  ""test_match"": ""exact text from context that should be matched"",
  ""confidence"": 0.95,
  ""reasoning"": ""why you chose this approach and pattern"",
  ""preserves_existing_groups"": true/false
}}

STRATEGY DECISION GUIDELINES:
- ""modify_existing_line"": If the field logically belongs with existing fields on the same line
- ""create_new_line"": If the field appears on a separate line or requires different processing

EXAMPLES:
- Single line currency: ""(?<TotalDeduction>Gift Card Applied:\s*-?\$?([0-9]+\.?[0-9]*))""
- Multi-line with context: ""(?<Address>(?:.*\n){{2,4}}.*(?:Street|Ave|Blvd|Road))""
- Modify existing: Combine with existing pattern while preserving all named groups

CRITICAL SAFETY REQUIREMENTS:
- If modifying existing regex, MUST preserve ALL existing named groups: {string.Join(", ", existingNamedGroups)}
- Test pattern against provided context to ensure it works
- Provide test_match with exact text that should be captured
- Explain reasoning for strategy choice

Create the optimal pattern based on your analysis of the full context.";
        }

        /// <summary>
        /// Creates prompt for direct data correction fallback when regex fixes fail
        /// </summary>
        public string CreateDirectDataCorrectionPrompt(List<dynamic> invoiceData, string originalText)
        {
            var invoiceJson = JsonSerializer.Serialize(invoiceData, new JsonSerializerOptions { WriteIndented = true });

            return $@"DIRECT DATA CORRECTION - BYPASS REGEX APPROACH:

The OCR import patterns could not be fixed automatically. Please provide direct value corrections to make the invoice data mathematically consistent.

EXTRACTED INVOICE DATA:
{invoiceJson}

ORIGINAL INVOICE TEXT:
{CleanTextForAnalysis(originalText)}

REQUIREMENTS:
1. Analyze the original text and provide correct values for fields that affect mathematical balance
2. Ensure the corrected data satisfies: SubTotal + TotalInternalFreight + TotalOtherCost + TotalInsurance - TotalDeduction = InvoiceTotal
3. Focus on critical fields that impact TotalsZero calculation
4. Maintain data integrity and business logic consistency

RESPONSE FORMAT (JSON only):
{{
  ""corrections"": [
    {{
      ""field"": ""InvoiceTotal"",
      ""current_value"": ""wrong_value"",
      ""correct_value"": ""right_value"",
      ""confidence"": 0.95,
      ""reasoning"": ""explanation of why this correction is needed""
    }}
  ],
  ""mathematical_check"": {{
    ""subtotal"": 150.00,
    ""freight"": 10.00,
    ""other_cost"": 12.00,
    ""insurance"": 0.00,
    ""deduction"": 6.99,
    ""calculated_total"": 165.01,
    ""totals_zero"": 0.0
  }}
}}

CRITICAL FIELDS TO FOCUS ON:
- InvoiceTotal: Final amount charged
- SubTotal: Sum of all line items
- TotalDeduction: Gift cards, discounts, store credits
- TotalInternalFreight: Shipping charges
- TotalOtherCost: Taxes and fees
- TotalInsurance: Insurance charges

EXAMPLE CORRECTION:
{{
  ""corrections"": [
    {{
      ""field"": ""TotalDeduction"",
      ""current_value"": ""0.00"",
      ""correct_value"": ""6.99"",
      ""confidence"": 0.98,
      ""reasoning"": ""Gift card deduction of $6.99 found in text but not extracted""
    }}
  ],
  ""mathematical_check"": {{
    ""subtotal"": 150.00,
    ""freight"": 10.00,
    ""other_cost"": 12.00,
    ""insurance"": 0.00,
    ""deduction"": 6.99,
    ""calculated_total"": 165.01,
    ""totals_zero"": 0.0
  }}
}}

Provide corrections that will make TotalsZero = 0 and ensure mathematical consistency.";
        }

        #endregion
    }
}