// File: OCRCorrectionService/OCRPromptCreation.cs
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Text.Json.Serialization;
using EntryDataDS.Business.Entities; // For ShipmentInvoice
using Serilog;

// WaterNut.DataSpace types (CorrectionResult, LineContext, OCRFieldMetadata) are in the same namespace.
namespace WaterNut.DataSpace
{
    public partial class OCRCorrectionService
    {
        #region Enhanced Prompt Creation Methods for DeepSeek

        /// <summary>
        /// FINAL, FULL CONTEXT (V11.1): Creates a prompt that generates both omission AND real, actionable format corrections.
        /// </summary>
        private string CreateHeaderErrorDetectionPrompt(ShipmentInvoice invoice, string fileText, Dictionary<string, OCRFieldMetadata> metadata)
        {
            _logger.Error("🚀 **PROMPT_CREATION_START (V11.7 - Consistent C# Escaping)**: Creating prompt for invoice {InvoiceNo}", invoice?.InvoiceNo ?? "NULL");

            // Build the simple JSON block of extracted values
            var currentValues = new Dictionary<string, object>
            {
                ["InvoiceNo"] = invoice?.InvoiceNo,
                ["InvoiceDate"] = invoice?.InvoiceDate,
                ["SupplierName"] = invoice?.SupplierName,
                ["Currency"] = invoice?.Currency,
                ["SubTotal"] = invoice?.SubTotal,
                ["TotalInternalFreight"] = invoice?.TotalInternalFreight,
                ["TotalOtherCost"] = invoice?.TotalOtherCost,
                ["TotalDeduction"] = invoice?.TotalDeduction,
                ["TotalInsurance"] = invoice?.TotalInsurance,
                ["InvoiceTotal"] = invoice?.InvoiceTotal,
            };
            var currentJson = JsonSerializer.Serialize(currentValues, new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });
            _logger.Error("   - **PROMPT_JSON_DUMP**: Basic extracted values: {JsonContext}", currentJson);

            // Build the human-readable annotated context block
            var annotatedContextBuilder = new StringBuilder();
            if (metadata != null)
            {
                var fieldsGroupedByCanonicalName = metadata.Values
                    .Where(m => m != null && !string.IsNullOrEmpty(m.Field))
                    .GroupBy(m => m.Field);

                foreach (var group in fieldsGroupedByCanonicalName)
                {
                    if (group.Count() > 1) // This is an aggregate field
                    {
                        var finalValue = GetCurrentFieldValue(invoice, group.Key);
                        annotatedContextBuilder.AppendLine($"\n- The value for `{group.Key}` ({finalValue}) was calculated by summing the following lines I found:");
                        foreach (var component in group)
                        {
                            annotatedContextBuilder.AppendLine($"  - Line {component.LineNumber}: Found value '{component.RawValue}' from rule '{component.LineName}' on text: \"{TruncateForLog(component.LineText, 100)}\"");
                        }
                    }
                }
            }
            string annotatedContext = annotatedContextBuilder.ToString();
            _logger.Error("   - **PROMPT_ANNOTATION_DUMP**: Annotated context for aggregates: {AnnotatedContext}", string.IsNullOrWhiteSpace(annotatedContext) ? "None" : annotatedContext);

            // Restore and integrate the OCR sections analysis
            var ocrSections = AnalyzeOCRSections(fileText);
            var ocrSectionsString = string.Join(", ", ocrSections);
            _logger.Error("   - **PROMPT_STRUCTURE_DUMP**: Detected OCR Sections: {OcrSections}", ocrSectionsString);

            var prompt = $@"INTELLIGENT OCR CORRECTION (V11.7 - Consistent C# Escaping):

**CONTEXT:**
I have extracted data from an OCR document. My goal is to find any values I missed, correct formatting errors, and infer missing data when there is strong contextual evidence.

**1. EXTRACTED FIELDS:**
{currentJson}

**2. CONTEXT & COMPONENTS:** (How totals were calculated)
{annotatedContext}
(If this section is empty, no fields were aggregated from multiple sources)

**3. COMPLETE OCR TEXT:**
{this.CleanTextForAnalysis(fileText)}

**YOUR TASK:**
Analyze the complete OCR text and generate a JSON object in the `errors` array for every omission, format correction, or inferred value you discover.
1.  An `omission` object with `suggested_regex` to add the missing value.
2.  A `format_correction` object with a `pattern` and `replacement` string to ensure the value is handled correctly in the future (ONLY for TotalInsurance).
3.  An `inferred` object with a `suggested_regex` to infer values that are not explicitly written but can be reliably deduced from context.


**CARIBBEAN CUSTOMS FIELD MAPPING & VALUE TRANSFORMATION (Apply to each new finding):**
- **If a line is a SUPPLIER-CAUSED REDUCTION** (e.g., 'Free Shipping', 'Discount'):
  - Set `field` to ""TotalDeduction"". For `correct_value`, return the **absolute value** as a string (e.g., ""6.53"").
- **If a line is a CUSTOMER-CAUSED REDUCTION** (e.g., 'Gift Card', 'Store Credit'):
  - **For the `omission` object:** Set `field` to ""TotalInsurance"". For `correct_value`, return the **negative absolute value** (e.g., ""-6.99"").
  - **For the `format_correction` object:** Set `field` to ""TotalInsurance"".
    - `extracted_value`: The number's **absolute value** (e.g., ""6.99"").
    - `correct_value`: The number's **absolute value multiplied by -1** (e.g., ""-6.99"").
    - `pattern`: A C# regex that captures a positive number (e.g., ""^(\\d+\\.?\\d*)$"").
    - `replacement`: The captured group with a negative sign prepended (e.g., ""-$1"").
- **For all other fields**, report the raw value from the text.

**SPECIAL CASE: CONTEXT-AWARE INFERRED VALUES**
If a value is not explicitly written but can be reliably inferred from contextual clues within the document, you must act like a detective and justify your inference.

**RULES FOR INFERENCE:**
1.  Generate an error object with `error_type`: `""inferred""`.
2.  The `suggested_regex` **MUST** match the line containing the primary clue (e.g., the dollar sign).
3.  The `correct_value` **MUST** be the static, inferred value (e.g., ""USD"").
4.  **CRITICAL:** The `reasoning` field **MUST** justify the inference by citing evidence from the document, such as the supplier's name or address. A simple symbol is NOT enough evidence on its own.

**EXAMPLE 1: VALID INFERENCE (Amazon Invoice)**
- **Text Shows:** `Sold by: Amazon.com Services, Inc` and `Order Total: $166.30`.
- **Your Reasoning:** The supplier is ""Amazon.com,"" a known US-based company, and the currency symbol is '$'. Therefore, it is safe to infer the currency is 'USD'.
- **Your JSON object:**
```json
{{
  ""field"": ""Currency"",
  ""extracted_value"": ""null"",
  ""correct_value"": ""USD"",
  ""line_text"": ""Order Total: $166.30"",
  ""line_number"": 7,
  ""confidence"": 0.99,
  ""error_type"": ""inferred"",
  ""suggested_regex"": ""Order Total:\\s*[\\$€£][\\d.,]+"",
  ""reasoning"": ""Inferred from document context: The supplier is 'Amazon.com', a US entity, and the total is prefixed with a '$' symbol. This combination strongly implies the currency is USD.""
}}
```

**EXAMPLE 2: INVALID INFERENCE (Ambiguous Supplier)**
- **Text Shows:** `Sold by: International Goods Ltd.` (a Trinidadian company) and `Total: $500.00`.
- **Your Reasoning:** The supplier is not explicitly US-based. The '$' symbol could mean TTD (Trinidad and Tobago Dollar) or USD. There is not enough evidence to confidently infer 'USD'.
- **Your Action:** DO NOT create an ""inferred"" error object for the currency in this case. It is better to leave it blank than to infer incorrectly.

**YOUR TASK:** Apply this detective-like reasoning. Only infer values when you have strong contextual evidence from the document itself to support your conclusion.

**YOUR RESPONSE FORMAT (A separate JSON object for EACH correction):**
```json
{{
  ""errors"": [
    {{
      ""field"": ""TotalDeduction"",
      ""extracted_value"": ""null"",
      ""correct_value"": ""6.53"",
      ""line_text"": ""Free Shipping: -$6.53"",
      ""line_number"": 75,
      ""confidence"": 0.98,
      ""error_type"": ""omission"",
      ""suggested_regex"": ""Free Shipping:\\s*-?\\$?(?<TotalDeduction>[\\d,]+\\.?\\d*)"",
      ""reasoning"": ""The OCR text contains a 'Free Shipping' line, which is a missed omission.""
    }},
    {{
      ""field"": ""TotalInsurance"",
      ""extracted_value"": ""null"",
      ""correct_value"": ""-6.99"",
      ""line_text"": ""Gift Card Amount: -$6.99"",
      ""line_number"": 77,
      ""confidence"": 0.98,
      ""error_type"": ""omission"",
      ""suggested_regex"": ""Gift Card Amount:\\s*-?\\$?(?<TotalInsurance>[\\d,]+\\.?\\d*)"",
      ""reasoning"": ""The OCR text contains a 'Gift Card Amount' line, which is a missed omission.""
    }},
    {{
      ""field"": ""TotalInsurance"",
      ""error_type"": ""format_correction"",
      ""extracted_value"": ""6.99"",
      ""correct_value"": ""-6.99"",
      ""line_text"": ""Gift Card Amount: -$6.99"",
      ""line_number"": 77,
      ""confidence"": 0.99,
      ""pattern"": ""^(\\d+\\.?\\d*)$"",
      ""replacement"": ""-$1"",
      ""reasoning"": ""This rule ensures any positive number captured for TotalInsurance is made negative, as per customs rules.""
    }}
  ]
}}
```

If you find no new omissions, return: {{""errors"": []}}";

            _logger.Error("🏁 **PROMPT_CREATION_COMPLETE (V11.7)**: Full context prompt with REAL FORMAT CORRECTION logic created. Length: {PromptLength} characters.", prompt.Length);
            return prompt;
        }




        /// <summary>
        /// Analyzes the file text to identify which OCR sections are present.
        /// </summary>
        private List<string> AnalyzeOCRSections(string fileText)
        {
            var sections = new List<string>();
            if (string.IsNullOrEmpty(fileText)) return sections;

            if (fileText.IndexOf("Single Column", StringComparison.OrdinalIgnoreCase) >= 0) sections.Add("Single Column");
            if (fileText.IndexOf("Ripped", StringComparison.OrdinalIgnoreCase) >= 0) sections.Add("Ripped");
            if (fileText.IndexOf("SparseText", StringComparison.OrdinalIgnoreCase) >= 0) sections.Add("SparseText");
            if (sections.Count == 0) sections.Add("Multiple OCR Methods");

            return sections;
        }

        /// <summary>
        /// Creates a prompt for DeepSeek to generate a new regex pattern, with self-correction guidance.
        /// </summary>
        public string CreateRegexCreationPrompt(CorrectionResult correction, LineContext lineContext)
        {
            var existingNamedGroups = lineContext.FieldsInLine?.Select(f => f.Key).Where(k => !string.IsNullOrEmpty(k)).Distinct().ToList() ?? new List<string>();
            var existingNamedGroupsString = existingNamedGroups.Any() ? string.Join(", ", existingNamedGroups) : "None";

            return $@"CREATE C# COMPLIANT REGEX PATTERN FOR OCR FIELD EXTRACTION:

**CRITICAL SELF-CORRECTION TASK:**
An upstream process may have provided an `Expected Value` that is aggregated or incorrect for the given `Text of the Line`.
**YOUR PRIMARY GOAL is to create a regex that extracts the value ACTUALLY PRESENT on the `Text of the Line`.**
If `Expected Value` ('{correction.NewValue}') does not appear in `Text of the Line` ('{correction.LineText}'), IGNORE the `Expected Value` and instead extract the correct value you see on that line. Your `test_match` MUST reflect the value you actually extracted.

OMITTED FIELD DETAILS:
- Field Name to Capture: ""{correction.FieldName}""
- Line Number in Original Text: {correction.LineNumber}
- Text of the Line Containing the Value: ""{correction.LineText}""
- Hint (Potentially Aggregated) Expected Value: ""{correction.NewValue}""

FULL TEXTUAL CONTEXT (the target line is marked with >>> <<<):
{lineContext.FullContextWithLineNumbers}

YOUR TASK & REQUIREMENTS:
1.  **Analyze the `Text of the Line`** to find the true value for ""{correction.FieldName}"".
2.  **Create a `regex_pattern`** that precisely captures this true value, including identifying text to prevent false matches.
3.  **Validate your pattern** by providing a `test_match` from the context that proves your pattern extracts the correct value from the line.

STRICT JSON RESPONSE FORMAT (EXAMPLE for a 'Free Shipping' amount of '-$0.46'):
{{
  ""strategy"": ""create_new_line"",
  ""regex_pattern"": ""Free Shipping:\\s*-?\\$?(?<TotalDeduction>[\\d,]+\\.?\\d*)"",
  ""complete_line_regex"": null,
  ""is_multiline"": false,
  ""max_lines"": 1,
  ""test_match"": ""Free Shipping: -$0.46"",
  ""confidence"": 0.98,
  ""reasoning"": ""Created a pattern to extract the granular value '-0.46' from the specific 'Free Shipping' line, ignoring the potentially aggregated hint value. This pattern is specific and robust."",
  ""preserves_existing_groups"": true
}}

⚠️ **CRITICAL REMINDERS:**
- All regex MUST be C# compliant (single backslashes).
- Place currency symbols ($ € £) OUTSIDE the named capture group.
- The system handles converting negative text ('-$0.46') to a positive deduction later. Your regex should just capture the number.

Focus on creating a robust pattern for the value you SEE on the provided line of text.";
        }

        /// <summary>
        /// NEW: Creates a prompt for DeepSeek to CORRECT a previously failed regex pattern.
        /// </summary>
        public string CreateRegexCorrectionPrompt(CorrectionResult correction, LineContext lineContext, RegexCreationResponse failedResponse, string failureReason)
        {
            return $@"REGEX CORRECTION TASK:

You previously generated a C# compliant regex pattern that FAILED validation. Your task is to analyze the failure and provide a corrected pattern.

THE FAILED ATTEMPT:
- Field to Capture: ""{correction.FieldName}""
- Failed Regex Pattern: ""{failedResponse.RegexPattern}""
- Text It Was Tested Against: ""{failedResponse.TestMatch ?? correction.LineText}""

REASON FOR FAILURE:
{failureReason}

FULL ORIGINAL CONTEXT (the target line is marked with >>> <<<):
{lineContext.FullContextWithLineNumbers}

YOUR TASK:
1.  Analyze the `FAILED ATTEMPT` and the `REASON FOR FAILURE`.
2.  Create a NEW, CORRECTED `regex_pattern` that successfully extracts the value from the text.
3.  Ensure the new pattern is specific and includes identifying text.
4.  Provide a new `test_match` that proves the corrected pattern works.

STRICT JSON RESPONSE FORMAT (Same as before):
{{
  ""strategy"": ""{failedResponse.Strategy}"",
  ""regex_pattern"": ""(your NEW, corrected pattern)"",
  ""complete_line_regex"": null,
  ""is_multiline"": {failedResponse.IsMultiline.ToString().ToLower()},
  ""max_lines"": {failedResponse.MaxLines},
  ""test_match"": ""(new test match proving the fix)"",
  ""confidence"": 0.99,
  ""reasoning"": ""(Explain what you changed and why it fixes the validation failure. E.g., 'Corrected the character class to include the negative sign which was previously missed.')"",
  ""preserves_existing_groups"": true
}}

⚠️ **CRITICAL REMINDERS:**
- All regex MUST be C# compliant (single backslashes).
- Place currency symbols ($ € £) OUTSIDE the named capture group.
- Focus on fixing the specific reason for failure.";
        }

        /// <summary>
        /// Creates a prompt for DeepSeek to detect errors and omissions in product line item data.
        /// </summary>
        private string CreateProductErrorDetectionPrompt(ShipmentInvoice invoice, string fileText)
        {
            var productDataForPrompt = invoice.InvoiceDetails?.Select(d => new
            {
                InputLineNumber = d.LineNumber,
                d.ItemDescription,
                d.Quantity,
                UnitCost = d.Cost,
                LineTotal = d.TotalCost,
                d.Discount,
                d.Units
            }).ToList();

            var productsJson = JsonSerializer.Serialize(productDataForPrompt, new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });

            return $@"OCR PRODUCT LINE ITEM ERROR DETECTION AND OMISSION ANALYSIS:
Analyze the original invoice text against the EXTRACTED PRODUCT DATA. Identify ALL discrepancies: format/value errors, calculation errors, MISSING fields within items, or ENTIRELY OMITTED line items.

EXTRACTED PRODUCT DATA (InputLineNumber is from current data, may not match text line number):
{productsJson}

ORIGINAL INVOICE TEXT:
{this.CleanTextForAnalysis(fileText)}

CRITICAL REQUIREMENTS FOR EACH IDENTIFIED ERROR/OMISSION:
1. ""field"": Use format ""InvoiceDetail_LineX_FieldName"". 'X' MUST be the 1-based line number from the ORIGINAL INVOICE TEXT where the item/field is found. For a completely missed line item, use a generic field like ""InvoiceDetail_LineX_OmittedLineItem"".
2. ""extracted_value"": Current value from EXTRACTED PRODUCT DATA if a corresponding item/field is found; otherwise empty string or null for omissions.
3. ""correct_value"": Correct value from ORIGINAL INVOICE TEXT.
4. ""line_text"": EXACT line(s) from ORIGINAL INVOICE TEXT where the error/omission is evident.
5. ""line_number"": 1-based *starting* line number in ORIGINAL INVOICE TEXT corresponding to ""line_text"".
6. ""error_type"": ""omission"", ""omitted_line_item"", ""format_error"", ""value_error"", ""calculation_error"", ""character_confusion"".
7. ""reasoning"": Explanation.

RESPONSE FORMAT (Strict JSON array of error objects under ""errors"" key):
{{
  ""errors"": [
    {{
      ""field"": ""InvoiceDetail_Line15_Quantity"", ""extracted_value"": ""l"", ""correct_value"": ""1"",
      ""line_text"": ""Super Widget - Part #SW123 Qty: l @ $19.99 ea."", ""line_number"": 15,
      ""confidence"": 0.92, ""error_type"": ""character_confusion"",
      ""reasoning"": ""OCR likely misread '1' as 'l' in the quantity.""
    }}
  ]
}}
If no errors or omissions are found, return: {{""errors"": []}}";
        }

        /// <summary>
        /// Creates a prompt for DeepSeek for direct data correction when regex/pattern fixes are insufficient.
        /// </summary>
        public string CreateDirectDataCorrectionPrompt(List<dynamic> invoiceDataList, string originalText)
        {
            var invoiceDataToSerialize = invoiceDataList?.FirstOrDefault() ?? (object)new Dictionary<string, object>();
            var invoiceJson = JsonSerializer.Serialize(invoiceDataToSerialize, new JsonSerializerOptions { WriteIndented = true, DefaultIgnoreCondition = JsonIgnoreCondition.WhenWritingNull });

            return $@"DIRECT DATA CORRECTION - MATHEMATICAL CONSISTENCY REQUIRED:
The automated OCR extraction resulted in an invoice that is not mathematically consistent. Your task is to analyze the ORIGINAL INVOICE TEXT and provide direct value changes to the EXTRACTED INVOICE DATA to ensure accuracy and mathematical balance.

EXTRACTED INVOICE DATA:
{invoiceJson}

ORIGINAL INVOICE TEXT:
{this.CleanTextForAnalysis(originalText)}

MATHEMATICAL VALIDATION RULES:
1. Overall: SubTotal + TotalInternalFreight + TotalOtherCost + TotalInsurance - TotalDeduction = InvoiceTotal (approximately).

CARIBBEAN CUSTOMS FIELD MAPPING (CRITICAL):
**SUPPLIER-CAUSED REDUCTIONS → TotalDeduction field:** Free shipping, discounts.
**CUSTOMER-CAUSED REDUCTIONS → TotalInsurance field (negative value):** Gift cards, store credits.

REQUIREMENTS FOR YOUR RESPONSE:
1. Analyze the ORIGINAL INVOICE TEXT to determine the true values for any fields that appear incorrect or cause mathematical imbalances.
2. Propose corrections by specifying the field name and its ""correct_value"" derived from the text.
3. The goal is to make the invoice data reflect the ORIGINAL INVOICE TEXT and be mathematically sound.
4. Provide a ""mathematical_check_after_corrections"" object showing how the invoice balances WITH YOUR PROPOSED CORRECTIONS APPLIED.

STRICT JSON RESPONSE FORMAT:
{{
  ""corrections"": [ 
    {{
      ""field"": ""InvoiceTotal"", 
      ""current_value"": ""(value from EXTRACTED INVOICE DATA)"", 
      ""correct_value"": ""(the true value from ORIGINAL INVOICE TEXT)"", 
      ""confidence"": 0.95,
      ""reasoning"": ""(brief explanation, e.g., 'InvoiceTotal in text is $150.55, not $155.00.')""
    }}
  ],
  ""mathematical_check_after_corrections"": {{
    ""subtotal"": 140.00, 
    ""freight"": 10.00,
    ""other_cost"": 5.00,
    ""insurance"": -5.00,
    ""deduction"": 10.00,
    ""calculated_invoice_total"": 140.00,
    ""final_invoice_total_field"": 140.00,
    ""balance_difference"": 0.00
  }}
}}

If the EXTRACTED INVOICE DATA is already correct, return an empty corrections array.
Only propose changes that are directly supported by evidence in the ORIGINAL INVOICE TEXT.";
        }

        #endregion
    }
}