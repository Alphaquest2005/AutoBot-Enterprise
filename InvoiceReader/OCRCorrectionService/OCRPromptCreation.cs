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
        /// Creates a prompt for DeepSeek to detect errors and omissions in invoice header fields.
        /// </summary>
        private string CreateHeaderErrorDetectionPrompt(ShipmentInvoice invoice, string fileText)
        {
            // Serialize only relevant header fields for the prompt
            var headerData = new Dictionary<string, object>();
            var props = typeof(ShipmentInvoice).GetProperties().Where(p => p.DeclaringType == typeof(ShipmentInvoice) && p.Name != "InvoiceDetails" && p.Name != "Attachments" && p.Name != "TrackingState" && p.Name != "ModifiedProperties");
            
            foreach (var prop in props)
            {
                headerData[prop.Name] = prop.GetValue(invoice);
            }
            // Ensure key fields are present even if null, so LLM knows they are expected
            string[] ensureKeys = { "InvoiceTotal", "SubTotal", "TotalDeduction", "InvoiceNo", "InvoiceDate", "SupplierName", "Currency" };
            foreach(var key in ensureKeys) {
                if(!headerData.ContainsKey(key)) headerData[key] = null;
            }

            var headerJson = JsonSerializer.Serialize(headerData, new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });

            return $@"OCR HEADER FIELD ERROR DETECTION AND OMISSION ANALYSIS:
Analyze the original invoice text against the extracted header data. Identify ALL discrepancies, including format errors, value errors, and MISSING fields (omissions).

EXTRACTED HEADER DATA:
{headerJson}

ORIGINAL INVOICE TEXT (condensed for brevity if long):
{this.CleanTextForAnalysis(fileText)} 

CRITICAL REQUIREMENTS FOR EACH IDENTIFIED ERROR/OMISSION:
1. ""field"": The EXACT name of the field from EXTRACTED HEADER DATA keys (e.g., ""InvoiceTotal"", ""TotalDeduction""). For true omissions not in keys, use the canonical name from your knowledge or the context if evident (e.g. ""TotalDeduction"" if a gift card line is found but TotalDeduction is null/missing in EXTRACTED HEADER DATA).
2. ""extracted_value"": The current value from EXTRACTED HEADER DATA for that field. For omissions, this should be empty string or reflect the null/missing state from EXTRACTED HEADER DATA.
3. ""correct_value"": The correct value as found or inferred from the ORIGINAL INVOICE TEXT.
4. ""line_text"": The EXACT line from ORIGINAL INVOICE TEXT where the correct value (or clear evidence of omission) appears.
5. ""line_number"": The 1-based line number in ORIGINAL INVOICE TEXT corresponding to ""line_text"".
6. ""context_lines_before"": Array of up to 5 full text lines immediately PRECEDING ""line_text"".
7. ""context_lines_after"": Array of up to 5 full text lines immediately FOLLOWING ""line_text"".
8. ""requires_multiline_regex"": true/false. Indicate if extracting this field might require a regex pattern spanning multiple lines (e.g., for a multi-line address if SupplierAddress was the field).
9. ""confidence"": Your confidence in this correction (0.0 to 1.0).
10. ""error_type"": One of: ""omission"" (field present in text, missing/null in extracted), ""format_error"" (extracted, but wrong format e.g. 123,45 vs 123.45), ""value_error"" (extracted, format ok, but value wrong vs text), ""decimal_separator"", ""character_confusion"", ""calculation_error"" (if a total is inconsistent with components visible in header text).
11. ""reasoning"": Brief explanation for the correction.

DEDUCTION IDENTIFICATION (CRITICAL FOR OMISSIONS OF TotalDeduction):
Actively search for these terms in ORIGINAL INVOICE TEXT. If found and TotalDeduction in EXTRACTED HEADER DATA is 0 or null, report as ""omission"":
- ""Gift card"", ""Gift certificate"", ""GC Applied"", ""Discount Code"", ""Coupon Applied""
- ""Store credit"", ""Account credit"", ""Promotional Credit""
- Terms indicating a reduction in the total amount payable due to non-standard line item discounts.

RESPONSE FORMAT (Strict JSON array of error objects under ""errors"" key):
{{
  ""errors"": [
    {{
      ""field"": ""TotalDeduction"", ""extracted_value"": ""0.00"", ""correct_value"": ""5.99"",
      ""line_text"": ""Gift Card Applied: -$5.99"", ""line_number"": 28,
      ""context_lines_before"": [""L23: Item X"", ""L24: Subtotal: $100.00"", ""L25: Shipping: $10.00"", ""L26: Handling: $2.00"", ""L27: Tax: $7.00""],
      ""context_lines_after"": [""L29: Grand Total: $101.01"", ""L30: Due Date: 01/15/2024"", ""L31: Payment Method: Visa ****1234"", ""L32: Auth Code: XYZ789"", ""L33: Thank you for your order!""],
      ""requires_multiline_regex"": false, ""confidence"": 0.98, ""error_type"": ""omission"",
      ""reasoning"": ""Gift card deduction clearly stated in text but not reflected in extracted TotalDeduction.""
    }}
    // ... more error objects if any ...
  ]
}}
If no errors or omissions are found, return: {{""errors"": []}}
Focus on fields critical for financial accuracy and identification: InvoiceTotal, SubTotal, all cost components, deductions, InvoiceNo, InvoiceDate, SupplierName.
Provide full context lines as available; do not truncate them in the JSON.
";
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

            return $@"CREATE REGEX PATTERN FOR OCR FIELD EXTRACTION:
An OCR process failed to extract a field. Your task is to create or modify a regex pattern to capture this missing field.

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
    *   `regex_pattern`: ALWAYS provide the specific regex part that captures the `Expected Value` for `FieldName`, like `(?<{correction.FieldName}>your_pattern_for_value)`.
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

EXAMPLE - Creating a new line for a missing ""GiftWrapFee"":
Value ""$2.50"" found on line ""Gift Wrap Fee: $2.50""
{{
  ""strategy"": ""create_new_line"",
  ""regex_pattern"": ""(?<GiftWrapFee>\\$\\d+\\.\\d{{2}})"",
  ""complete_line_regex"": null,
  ""is_multiline"": false,
  ""max_lines"": 1,
  ""test_match"": ""$2.50"",
  ""confidence"": 0.95,
  ""reasoning"": ""Fee is on its own distinct line. Pattern captures currency value."",
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