// File: OCRCorrectionService/OCRPromptCreation.cs
using System;
using System.Collections.Generic; // For List<dynamic> in one prompt
using System.Linq;
using System.Text.Json;
using EntryDataDS.Business.Entities; // For ShipmentInvoice
// Serilog ILogger is available as this._logger
// WaterNut.DataSpace types (CorrectionResult, LineContext, OCRFieldMetadata) are in the same namespace.

namespace WaterNut.DataSpace
{
    using System.Text.Json.Serialization;

    public partial class OCRCorrectionService
    {
        #region Enhanced Prompt Creation Methods for DeepSeek

        /// <summary>
        /// Creates an enhanced prompt for DeepSeek with OCR architecture context and deduplication awareness.
        /// Explains multi-section OCR strategy and precedence hierarchy to improve field detection.
        /// </summary>
        private string CreateHeaderErrorDetectionPrompt(ShipmentInvoice invoice, string fileText)
        {
            // **PROMPT_CREATION_START**: Log the beginning of enhanced prompt creation
            _logger.Information("🔍 **PROMPT_CREATION_START**: Creating ENHANCED OCR-aware header error detection prompt for invoice {InvoiceNo}", invoice?.InvoiceNo ?? "NULL");
            
            // Create a simple current values summary
            var currentValues = new Dictionary<string, object>
            {
                ["InvoiceNo"] = invoice?.InvoiceNo,
                ["InvoiceTotal"] = invoice?.InvoiceTotal,
                ["SubTotal"] = invoice?.SubTotal,
                ["TotalInternalFreight"] = invoice?.TotalInternalFreight,
                ["TotalOtherCost"] = invoice?.TotalOtherCost,
                ["TotalDeduction"] = invoice?.TotalDeduction,
                ["TotalInsurance"] = invoice?.TotalInsurance,
                ["InvoiceDate"] = invoice?.InvoiceDate,
                ["SupplierName"] = invoice?.SupplierName,
                ["Currency"] = invoice?.Currency
            };
            
            var currentJson = JsonSerializer.Serialize(currentValues, new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });
            
            // Analyze OCR sections present in the file text
            var ocrSections = AnalyzeOCRSections(fileText);
            
            // **DATA_CHECK**: Log current invoice values and OCR analysis before prompt creation
            _logger.Information("🔍 **PROMPT_INVOICE_DATA**: InvoiceNo={InvoiceNo} | SubTotal={SubTotal} | TotalDeduction={TotalDeduction} | TotalInsurance={TotalInsurance} | InvoiceTotal={InvoiceTotal}", 
                invoice?.InvoiceNo, invoice?.SubTotal, invoice?.TotalDeduction, invoice?.TotalInsurance, invoice?.InvoiceTotal);
            _logger.Information("🔍 **PROMPT_OCR_SECTIONS**: Detected {SectionCount} OCR sections: {Sections}", ocrSections.Count, string.Join(", ", ocrSections));
            
            // **FILE_TEXT_CHECK**: Verify the file text contains financial information
            bool fileTextContainsFinancials = fileText?.Contains("$") == true || fileText?.Contains("Total") == true;
            bool fileTextContainsGiftCard = fileText?.Contains("Gift Card") == true;
            bool fileTextContainsFreeShipping = fileText?.Contains("Free Shipping") == true;
            _logger.Information("🔍 **PROMPT_FILE_TEXT_CHECK**: Financial={Financials} | GiftCard={GiftCard} | FreeShipping={FreeShipping}", 
                fileTextContainsFinancials, fileTextContainsGiftCard, fileTextContainsFreeShipping);
            _logger.Information("🔍 **PROMPT_FILE_TEXT_LENGTH**: File text length: {Length} characters", fileText?.Length ?? 0);
            
            var prompt = $@"INTELLIGENT OCR ERROR COMPENSATION AND MISSING FIELD DETECTION:

CRITICAL OCR ARCHITECTURE CONTEXT:
This invoice was processed using MULTIPLE OCR TECHNIQUES to compensate for OCR errors and ensure complete data extraction:

1. MULTIPLE OCR SECTIONS EXPLANATION:
   Detected sections: {string.Join(", ", ocrSections)}
   - 'Single Column': Primary OCR method, cleanest results but may miss complex layouts
   - 'Ripped': Secondary OCR method, handles fragmented text better  
   - 'SparseText': Tertiary OCR method, captures scattered text that others miss

2. ERROR COMPENSATION STRATEGY:
   Each OCR method has different strengths/weaknesses:
   - Single Column might miss data in complex table layouts or faded text
   - Ripped might capture data that Single Column omitted due to layout issues
   - SparseText might find scattered amounts that both others missed

3. DATA PRECEDENCE HIERARCHY: 'Single Column' > 'Ripped' > 'SparseText'
   - If Single Column has complete data, prefer it
   - If Single Column is incomplete/missing data, use Ripped to fill gaps
   - If both Single and Ripped miss data, use SparseText to complete
   - When you see IDENTICAL values in multiple sections, this is the SAME invoice data processed by different OCR methods

4. CRITICAL DEDUPLICATION RULES:
   - Same amount appearing in multiple sections = SAME invoice data, count ONCE
   - Different amounts in different sections = potential OCR errors, apply precedence
   - Missing data in higher precedence section = use lower precedence to fill gaps

5. CARIBBEAN CUSTOMS FIELD MAPPING (CRITICAL):
   **SUPPLIER-CAUSED REDUCTIONS → TotalDeduction field (positive values):**
   - Free shipping credits/discounts, promotional discounts, volume discounts
   - Any reduction where the SUPPLIER absorbs the cost
   
   **CUSTOMER-CAUSED REDUCTIONS → TotalInsurance field (negative values):**
   - Gift cards, store credits, account credits, loyalty points  
   - Any reduction where the CUSTOMER uses previously acquired value
   - ALWAYS use NEGATIVE values (e.g., -6.99 for $6.99 gift card)

CURRENTLY EXTRACTED FIELDS (from primary OCR):
{currentJson}

COMPLETE MULTI-SECTION OCR TEXT (analyze ALL sections for missing data):
{this.CleanTextForAnalysis(fileText)}

INTELLIGENT FIELD DETECTION TASK:
1. Identify fields missing from EXTRACTED DATA that exist in ANY OCR section
2. Use precedence rules to determine best value when conflicts exist  
3. Fill gaps where higher-precedence sections failed but lower-precedence succeeded
4. Deduplicate identical values that appear across multiple sections
5. Apply Caribbean customs field mapping rules correctly

RESPONSE FORMAT - Return EXACTLY this JSON structure:
{{
  ""errors"": [
    {{
      ""field"": ""TotalDeduction"",
      ""extracted_value"": ""null"",
      ""correct_value"": ""6.99"",
      ""line_text"": ""Free Shipping: -$0.46\\nFree Shipping: -$6.53"",
      ""line_number"": 69,
      ""confidence"": 0.95,
      ""error_type"": ""omission"",
      ""reasoning"": ""Free shipping amounts (-$0.46 + -$6.53 = $6.99 total) found in multiple OCR sections but missing from extracted data. Caribbean customs requires mapping supplier reductions to TotalDeduction. Deduplicated identical amounts across sections.""
    }},
    {{
      ""field"": ""TotalInsurance"",
      ""extracted_value"": ""null"",
      ""correct_value"": ""-6.99"",
      ""line_text"": ""Gift Card Amount: -$6.99"",
      ""line_number"": 85,
      ""confidence"": 0.98,
      ""error_type"": ""omission"",
      ""reasoning"": ""Gift card amount found in OCR sections but missing from extracted data. Caribbean customs requires mapping customer reductions to TotalInsurance as negative values.""
    }}
  ]
}}

If no missing fields found, return: {{""errors"": []}}

CRITICAL INSTRUCTIONS:
- When calculating totals from multiple occurrences, deduplicate identical amounts first
- Apply Caribbean customs field mapping rules (supplier→TotalDeduction, customer→TotalInsurance negative)
- Use precedence hierarchy to resolve conflicts between OCR sections
- Focus on financial fields critical for invoice balance and customs compliance
";
            
            // **PROMPT_CREATION_COMPLETE**: Log the final enhanced prompt being sent to DeepSeek
            _logger.Information("🔍 **PROMPT_CREATION_COMPLETE**: ENHANCED OCR-aware prompt created with {PromptLength} characters", prompt.Length);
            _logger.Information("🔍 **PROMPT_OCR_AWARENESS**: Prompt includes {SectionCount} OCR sections and deduplication context", ocrSections.Count);
            _logger.Information("🔍 **PROMPT_COMPLETE_CONTENT**: Complete enhanced prompt sent to DeepSeek: {CompletePrompt}", prompt);
            
            return prompt;
        }

        /// <summary>
        /// Analyzes the file text to identify which OCR sections are present.
        /// </summary>
        private List<string> AnalyzeOCRSections(string fileText)
        {
            var sections = new List<string>();
            
            if (string.IsNullOrEmpty(fileText))
            {
                return sections;
            }
            
            // Look for common OCR section indicators (using .NET Framework 4.8 compatible methods)
            if (fileText.IndexOf("Single Column", StringComparison.OrdinalIgnoreCase) >= 0 || 
                fileText.IndexOf("SingleColumn", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                sections.Add("Single Column");
            }
            
            if (fileText.IndexOf("Ripped", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                sections.Add("Ripped");
            }
            
            if (fileText.IndexOf("SparseText", StringComparison.OrdinalIgnoreCase) >= 0 || 
                fileText.IndexOf("Sparse Text", StringComparison.OrdinalIgnoreCase) >= 0)
            {
                sections.Add("SparseText");
            }
            
            // If no specific sections detected, assume multiple methods were used
            if (sections.Count == 0)
            {
                sections.Add("Multiple OCR Methods");
            }
            
            return sections;
        }

        /// <summary>
        /// Creates a prompt for DeepSeek to detect errors and omissions in product line item data.
        /// </summary>
        private string CreateProductErrorDetectionPrompt(ShipmentInvoice invoice, string fileText)
        {
            var productDataForPrompt = invoice.InvoiceDetails?.Select(d => new
            {
                // Providing the LineNumber from the current data helps DeepSeek correlate, but it should find the *actual* line in text.
                InputLineNumber = d.LineNumber, 
                d.ItemDescription,
                d.Quantity,
                UnitCost = d.Cost, // Clarify this is unit cost
                LineTotal = d.TotalCost,
                d.Discount,
                d.Units
            }).ToList();

            var productsJson = JsonSerializer.Serialize(productDataForPrompt, new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });

            return $@"OCR PRODUCT LINE ITEM ERROR DETECTION AND OMISSION ANALYSIS:
Analyze the original invoice text against the EXTRACTED PRODUCT DATA. Identify ALL discrepancies: format/value errors, calculation errors, MISSING fields within items, or ENTIRELY OMITTED line items.

EXTRACTED PRODUCT DATA (InputLineNumber is from current data, may not match text line number):
{productsJson}

ORIGINAL INVOICE TEXT (condensed for brevity if long):
{this.CleanTextForAnalysis(fileText)}

CRITICAL REQUIREMENTS FOR EACH IDENTIFIED ERROR/OMISSION:
1. ""field"": Use format ""InvoiceDetail_LineX_FieldName"". 'X' MUST be the 1-based line number from the ORIGINAL INVOICE TEXT where the item/field is found. FieldName examples: ""ItemDescription"", ""Quantity"", ""Cost"" (for unit cost), ""TotalCost"" (for line total), ""Discount"", ""Units"". For a completely missed line item, use a generic field like ""InvoiceDetail_LineX_OmittedLineItem"" where X is starting line of omitted item in text.
2. ""extracted_value"": Current value from EXTRACTED PRODUCT DATA if a corresponding item/field is found; otherwise empty string or null for omissions.
3. ""correct_value"": Correct value from ORIGINAL INVOICE TEXT. For ""OmittedLineItem"", this could be a string summarizing the item, e.g., ""Product XYZ, Qty 2, Price $10.00, Total $20.00"".
4. ""line_text"": EXACT line(s) from ORIGINAL INVOICE TEXT where the error/omission is evident. If item spans multiple lines, join with \n.
5. ""line_number"": 1-based *starting* line number in ORIGINAL INVOICE TEXT corresponding to ""line_text"".
6. ""context_lines_before"": Array of up to 5 full text lines PRECEDING the start of ""line_text"".
7. ""context_lines_after"": Array of up to 5 full text lines FOLLOWING the end of ""line_text"".
8. ""requires_multiline_regex"": true if extraction of this item/field likely needs multi-line regex.
9. ""confidence"": Your confidence (0.0 to 1.0).
10. ""error_type"": ""omission"" (missing field in an otherwise extracted line), ""omitted_line_item"" (entire line item missed), ""format_error"", ""value_error"", ""calculation_error"", ""character_confusion"".
11. ""reasoning"": Explanation.

VALIDATION FOCUS:
- COMPLETENESS: Are all line items from text present in extracted data? If not, ""omitted_line_item"".
- QUANTITIES: Verify. OCR confusion (1/l, 0/O, S/5, B/8). Reasonable values.
- UNIT PRICES (Cost): Verify. Decimal/currency format ($12.50 vs $1250). Reasonable values.
- LINE TOTALS (TotalCost): Verify calculation: (Quantity * UnitCost) - Discount = LineTotal.
- DESCRIPTIONS: Verify. Truncation, substitutions.

RESPONSE FORMAT (Strict JSON array of error objects under ""errors"" key):
{{
  ""errors"": [
    {{
      ""field"": ""InvoiceDetail_Line15_Quantity"", ""extracted_value"": ""l"", ""correct_value"": ""1"",
      ""line_text"": ""Super Widget - Part #SW123 Qty: l @ $19.99 ea."", ""line_number"": 15,
      ""context_lines_before"": [""L10..."", ..., ""L14...""], ""context_lines_after"": [""L16..."", ..., ""L20...""],
      ""requires_multiline_regex"": false, ""confidence"": 0.92, ""error_type"": ""character_confusion"",
      ""reasoning"": ""OCR likely misread '1' as 'l' in the quantity.""
    }},
    {{
      ""field"": ""InvoiceDetail_Line22_OmittedLineItem"", ""extracted_value"": """", ""correct_value"": ""Extra Gizmo, Qty 2, Unit Price $5.00, Line Total $10.00"",
      ""line_text"": ""Extra Gizmo\n  Qty: 2   Unit Price: $5.00    Total: $10.00"", ""line_number"": 22,
      ""context_lines_before"": [...], ""context_lines_after"": [...],
      ""requires_multiline_regex"": true, ""confidence"": 0.97, ""error_type"": ""omitted_line_item"",
      ""reasoning"": ""This line item is present in the text between line 21 and 24 but was not extracted.""
    }}
  ]
}}
If no errors or omissions are found, return: {{""errors"": []}}
Full context lines are important for multi-line items.
";
        }

        /// <summary>
        /// Creates a prompt for DeepSeek to generate a new regex pattern, typically for an omitted field.
        /// </summary>
        public string CreateRegexCreationPrompt(CorrectionResult correction, LineContext lineContext)
        {
            var existingNamedGroups = lineContext.FieldsInLine?.Select(f => f.Key).Where(k => !string.IsNullOrEmpty(k)).Distinct().ToList() ?? new List<string>();
            var existingNamedGroupsString = existingNamedGroups.Any() ? string.Join(", ", existingNamedGroups) : "None";

            return $@"CREATE C# COMPLIANT REGEX PATTERN FOR OCR FIELD EXTRACTION:
An OCR process failed to extract a field. Your task is to create or modify a C# compliant regex pattern to capture this missing field.

CRITICAL REQUIREMENTS:
- All regex patterns MUST be C# compliant (use single backslashes, not double backslashes)
- Use pseudo datatypes: ""Number"" (not decimal), ""String"" (not string), ""English Date"" (not DateTime)
- Patterns must work with .NET Regex class without escaping issues
- MANDATORY: Include identifying text in regex patterns to prevent false matches
- ⚠️  CRITICAL: Place currency symbols OUTSIDE named capture groups to avoid validation failures

⚠️  CRITICAL WARNING - AVOID GENERIC PATTERNS:
❌ WRONG: ""(?<TotalDeduction>\\d+\\.\\d{{2}})"" - This matches ANY decimal number!
✅ CORRECT: ""Free Shipping:\\s*(?<TotalDeduction>-?\\$[\\d,]+\\.?\\d*)"" - This only matches after ""Free Shipping:""

⚠️  CRITICAL WARNING - CURRENCY SYMBOL PLACEMENT:
❌ WRONG: ""Gift Card Amount:\\s*(?<TotalInsurance>-?\\$\\d+\\.\\d{{2}})"" - Currency symbol INSIDE capture group
✅ CORRECT: ""Gift Card Amount:\\s*\\$?(?<TotalInsurance>-?\\d+\\.\\d{{2}})"" - Currency symbol OUTSIDE capture group
   
The validation system expects numeric values without currency symbols. Always place $ € £ ¥ symbols outside capture groups!

Generic patterns cause incorrect field mapping and data corruption. Always include qualifying text!

OMITTED FIELD DETAILS:
- Field Name to Capture: ""{correction.FieldName}""
- Expected Value: ""{correction.NewValue}""
- Line Number in Original Text: {correction.LineNumber}
- Text of the Line Containing the Value: ""{correction.LineText}""

FULL TEXTUAL CONTEXT (target line where value ""{correction.NewValue}"" is found is marked with >>> <<<):
{string.Join("\n", lineContext.ContextLinesBefore)}
>>> LINE {lineContext.LineNumber}: {lineContext.LineText} <<<
{string.Join("\n", lineContext.ContextLinesAfter)}

EXISTING LINE CONTEXT (if applicable):
- Existing Regex for this Line (if any): {lineContext.RegexPattern ?? "None"}
- Named Groups Already Captured by Current Regex: {existingNamedGroupsString}
- System Hint for Multiline Need: {correction.RequiresMultilineRegex}

YOUR TASK & REQUIREMENTS:
1.  DECIDE STRATEGY:
    *   ""modify_existing_line"": Choose if `Current Regex` is not ""None"" AND the new field logically belongs on the same line AND you can safely add a new capture group `(?<{correction.FieldName}>...)` while PRESERVING ALL `Existing Named Groups`.
    *   ""create_new_line"": Choose if no `Current Regex`, or modification is too complex/risky, or the field appears on a new conceptual line.
2.  PROVIDE REGEX:
    *   `regex_pattern`: CRITICAL - MUST include qualifying/identifying text before the capture group to avoid false matches. Pattern format: `identifying_text\\s*(?<{correction.FieldName}>value_pattern)`. For example: `Gift Card Amount:\\s*(?<TotalInsurance>-?\\$[\\d,]+\\.?\\d*)` instead of just `(?<TotalInsurance>-?\\$[\\d,]+\\.?\\d*)`.
    *   `complete_line_regex`: IF strategy is ""modify_existing_line"", provide the FULL modified `Current Regex` string that includes the new group and preserves all old ones. Otherwise, leave empty or null.
3.  DEFINE EXTRACTION BEHAVIOR:
    *   `is_multiline`: true/false. Does your `regex_pattern` (or the new part in `complete_line_regex`) need to span multiple text lines?
    *   `max_lines`: If `is_multiline` is true, how many lines max (e.g., 1 to 5)? Default to 1 if not multiline.
4.  VALIDATE YOUR PATTERN:
    *   `test_match`: Provide the EXACT minimal substring from the `FULL TEXTUAL CONTEXT` that your `regex_pattern` (the part for the new field) should match to extract `Expected Value`.
5.  SAFETY:
    *   `preserves_existing_groups`: MUST be true if ""modify_existing_line"". For ""create_new_line"", it's implicitly true regarding prior groups.

STRICT JSON RESPONSE FORMAT:
{{
  ""strategy"": ""(either 'modify_existing_line' or 'create_new_line')"",
  ""regex_pattern"": ""(?<{correction.FieldName}>your_pattern_for_this_field_only)"",
  ""complete_line_regex"": ""(IF 'modify_existing_line', THE FULL UPDATED REGEX STRING, else null or empty)"",
  ""is_multiline"": (true or false),
  ""max_lines"": (integer, e.g., 1 or 3),
  ""test_match"": ""(exact text snippet your regex_pattern should match from context)"",
  ""confidence"": (0.0 to 1.0, your confidence in this pattern),
  ""reasoning"": ""(briefly explain your pattern and strategy choice)"",
  ""preserves_existing_groups"": (true if 'modify_existing_line', otherwise true)
}}

EXAMPLE - Creating a new line for a missing ""GiftWrapFee"" (C# COMPLIANT):
Value ""$2.50"" found on line ""Gift Wrap Fee: $2.50""
{{
  ""strategy"": ""create_new_line"",
  ""regex_pattern"": ""Gift Wrap Fee:\\s*(?<GiftWrapFee>\\$\\d+\\.\\d{{2}})"",
  ""complete_line_regex"": null,
  ""is_multiline"": false,
  ""max_lines"": 1,
  ""test_match"": ""Gift Wrap Fee: $2.50"",
  ""confidence"": 0.95,
  ""reasoning"": ""Fee is on its own distinct line. Pattern includes 'Gift Wrap Fee:' identifier to avoid false matches on other dollar amounts. C# compliant pattern using single backslashes."",
  ""preserves_existing_groups"": true
}}

EXAMPLE - Amazon Free Shipping (multiple values to sum):
Values ""-$0.46"" and ""-$6.53"" found on lines ""Free Shipping: -$0.46"" and ""Free Shipping: -$6.53""
{{
  ""strategy"": ""create_new_line"",
  ""regex_pattern"": ""Free Shipping:\\s*(?<TotalDeduction>-?\\$[\\d,]+\\.?\\d*)"",
  ""complete_line_regex"": null,
  ""is_multiline"": false,
  ""max_lines"": 1,
  ""test_match"": ""Free Shipping: -$0.46"",
  ""confidence"": 0.95,
  ""reasoning"": ""Pattern includes 'Free Shipping:' identifier to match only shipping deductions, not other dollar amounts. Multiple matches will be summed as absolute values."",
  ""preserves_existing_groups"": true
}}
Focus on creating a robust and accurate pattern. If modifying, ensure no existing data capture is broken.
";
        }

        /// <summary>
        /// Creates a prompt for DeepSeek for direct data correction when regex/pattern fixes are insufficient.
        /// </summary>
        public string CreateDirectDataCorrectionPrompt(List<dynamic> invoiceDataList, string originalText)
        {
            // This prompt assumes it's correcting ONE invoice structure at a time.
            // If invoiceDataList has multiple, the calling logic should iterate or this prompt needs adjustment.
            var invoiceDataToSerialize = invoiceDataList?.FirstOrDefault() ?? (object)new Dictionary<string, object>();
            var invoiceJson = JsonSerializer.Serialize(invoiceDataToSerialize, new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });

            return $@"DIRECT DATA CORRECTION - MATHEMATICAL CONSISTENCY REQUIRED:
The automated OCR extraction and pattern-based corrections have resulted in an invoice that may not be mathematically consistent or still contains errors. Your task is to analyze the ORIGINAL INVOICE TEXT and provide direct value changes to the EXTRACTED INVOICE DATA to ensure accuracy and mathematical balance.

EXTRACTED INVOICE DATA (current state, possibly after some automated fixes):
{invoiceJson}

ORIGINAL INVOICE TEXT (condensed for brevity if long):
{this.CleanTextForAnalysis(originalText)}

MATHEMATICAL VALIDATION RULES:
1. For each line item (in InvoiceDetails if present): (Quantity * Cost) - Discount = TotalCost (approximately).
2. Overall: SubTotal (sum of all line item TotalCosts) + TotalInternalFreight + TotalOtherCost + TotalInsurance - TotalDeduction = InvoiceTotal (approximately).
   (Allow for minor rounding differences, e.g., +/- $0.01 or $0.02).

CARIBBEAN CUSTOMS FIELD MAPPING (CRITICAL):
**SUPPLIER-CAUSED REDUCTIONS → TotalDeduction field:**
- Free shipping credits/discounts
- Supplier promotional discounts, volume discounts, manufacturer rebates
- Any reduction where the SUPPLIER absorbs the cost

**CUSTOMER-CAUSED REDUCTIONS → TotalInsurance field (negative value):**
- Gift cards, store credits, account credits, loyalty points
- Any reduction where the CUSTOMER uses previously acquired value
- When correcting, use NEGATIVE values in TotalInsurance (e.g., -6.99 for a $6.99 gift card)

REQUIREMENTS FOR YOUR RESPONSE:
1. Analyze the ORIGINAL INVOICE TEXT to determine the true values for any fields in EXTRACTED INVOICE DATA that appear incorrect or cause mathematical imbalances.
2. Propose corrections by specifying the field name and its ""correct_value"" derived from the text.
3. You can correct header fields (e.g., ""InvoiceTotal"", ""SubTotal"", ""TotalDeduction"") and line item fields.
4. For line item fields, use the 0-based index from the EXTRACTED INVOICE DATA's ""InvoiceDetails"" array. Format: ""InvoiceDetails[index].FieldName"" (e.g., ""InvoiceDetails[0].Quantity"", ""InvoiceDetails[1].TotalCost"").
5. The goal is to make the invoice data reflect the ORIGINAL INVOICE TEXT and be mathematically sound according to the rules above.
6. Provide a ""mathematical_check_after_corrections"" object showing how the invoice balances WITH YOUR PROPOSED CORRECTIONS APPLIED.

STRICT JSON RESPONSE FORMAT:
{{
  ""corrections"": [ 
    // Array of correction objects. Can be empty if no corrections are needed or possible.
    {{
      ""field"": ""InvoiceTotal"", // Or ""InvoiceDetails[0].TotalCost"" for a line item
      ""current_value"": ""(value from EXTRACTED INVOICE DATA)"", 
      ""correct_value"": ""(the true value from ORIGINAL INVOICE TEXT)"", 
      ""confidence"": (0.0 to 1.0, your confidence in this specific correction),
      ""reasoning"": ""(brief explanation, e.g., 'InvoiceTotal in text is $150.55, not $155.00.')""
    }}
  ],
  ""mathematical_check_after_corrections"": {{ // Reflects state AFTER your proposed 'corrections' are applied
    ""subtotal"": (calculated sum of all corrected InvoiceDetails.TotalCost), 
    ""freight"": (corrected TotalInternalFreight or original if unchanged),
    ""other_cost"": (corrected TotalOtherCost or original),
    ""insurance"": (corrected TotalInsurance or original),
    ""deduction"": (corrected TotalDeduction or original),
    ""calculated_invoice_total"": (subtotal + freight + other_cost + insurance - deduction),
    ""final_invoice_total_field"": (the value of InvoiceTotal field AFTER your corrections),
    ""balance_difference"": (calculated_invoice_total - final_invoice_total_field) // Should be close to 0.00
  }}
}}

If the EXTRACTED INVOICE DATA is already correct and balanced based on the ORIGINAL INVOICE TEXT, return:
{{ ""corrections"": [], ""mathematical_check_after_corrections"": {{ ... current balanced values ... }} }}
Only propose changes that are directly supported by evidence in the ORIGINAL INVOICE TEXT.
";
        }

        #endregion
    }
}