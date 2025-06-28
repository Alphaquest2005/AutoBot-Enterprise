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

        // =============================== COMPREHENSIVE ESCAPING HELPERS ===============================
        
        /// <summary>
        /// Prepares a regex pattern for safe embedding inside a JSON string value.
        /// This means replacing every single backslash '\' with a double backslash '\\'.
        /// </summary>
        private string EscapeRegexForJson(string regexPattern)
        {
            if (string.IsNullOrEmpty(regexPattern))
            {
                return "";
            }
            // In JSON, a literal backslash must be escaped with another backslash.
            return regexPattern.Replace(@"\", @"\\");
        }

        /// <summary>
        /// Escapes regex patterns for display in documentation examples within prompt strings.
        /// These need quadruple escaping: C# string -> JSON display -> regex interpretation.
        /// </summary>
        private string EscapeRegexForDocumentation(string regexPattern)
        {
            if (string.IsNullOrEmpty(regexPattern))
            {
                return "";
            }
            // Four levels of escaping: \\\\ becomes \\ in C# string, then \\ in JSON, then \ in regex
            return regexPattern.Replace(@"\", @"\\\\");
        }

        /// <summary>
        /// Escapes regex patterns for JSON example blocks that will be parsed as actual JSON.
        /// This applies double escaping for proper JSON string embedding.
        /// </summary>
        private string EscapeRegexForJsonExample(string regexPattern)
        {
            if (string.IsNullOrEmpty(regexPattern))
            {
                return "";
            }
            // Double escaping for JSON examples
            return regexPattern.Replace(@"\", @"\\");
        }

        /// <summary>
        /// Escapes regex patterns for validation examples that demonstrate regex testing.
        /// These need to show the actual regex as it would appear in code.
        /// </summary>
        private string EscapeRegexForValidation(string regexPattern)
        {
            if (string.IsNullOrEmpty(regexPattern))
            {
                return "";
            }
            // Triple escaping for validation examples that show code
            return regexPattern.Replace(@"\", @"\\\");
        }
        
        // ===========================================================================

        /// <summary>
        /// OBJECT-ORIENTED INVOICE ANALYSIS (V13.0): Creates a prompt that treats invoice as structured business objects with grouped and independent field entities.
        /// </summary>
        private string CreateHeaderErrorDetectionPrompt(ShipmentInvoice invoice, string fileText, Dictionary<string, OCRFieldMetadata> metadata)
        {
            _logger.Information("🚀 **PROMPT_CREATION_START (V14.0 - Object-Oriented Architecture)**: Creating object-aware prompt for invoice {InvoiceNo}", invoice?.InvoiceNo ?? "NULL");

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

            string annotatedContext = "";
            if (metadata != null)
            {
                var annotatedContextBuilder = new StringBuilder();
                var fieldsGroupedByCanonicalName = metadata.Values
                    .Where(m => m != null && !string.IsNullOrEmpty(m.Field))
                    .GroupBy(m => m.Field);

                foreach (var group in fieldsGroupedByCanonicalName)
                {
                    if (group.Count() > 1)
                    {
                        var finalValue = GetCurrentFieldValue(invoice, group.Key);
                        annotatedContextBuilder.AppendLine($"\n- The value for `{group.Key}` ({finalValue}) was calculated by summing the following lines I found:");
                        foreach (var component in group)
                        {
                            annotatedContextBuilder.AppendLine($"  - Line {component.LineNumber}: Found value '{component.RawValue}' from rule '{component.LineName}' on text: \"{TruncateForLog(component.LineText, 100)}\"");
                        }
                    }
                }
                annotatedContext = annotatedContextBuilder.ToString();
            }
            _logger.Error("   - **PROMPT_ANNOTATION_DUMP**: Annotated context for aggregates: {AnnotatedContext}", string.IsNullOrWhiteSpace(annotatedContext) ? "None" : annotatedContext);

            var ocrSections = AnalyzeOCRSections(fileText);
            var ocrSectionsString = string.Join(", ", ocrSections);
            _logger.Error("   - **PROMPT_STRUCTURE_DUMP**: Detected OCR Sections: {OcrSections}", ocrSectionsString);

            double subTotal = invoice?.SubTotal ?? 0;
            double freight = invoice?.TotalInternalFreight ?? 0;
            double otherCost = invoice?.TotalOtherCost ?? 0;
            double deduction = invoice?.TotalDeduction ?? 0;
            double insurance = invoice?.TotalInsurance ?? 0;
            double reportedTotal = invoice?.InvoiceTotal ?? 0;
            double calculatedTotal = subTotal + freight + otherCost + insurance - deduction;
            double discrepancy = reportedTotal - calculatedTotal;

            string balanceCheckContext = $@"
**4. MATHEMATICAL BALANCE CHECK:**
My system's calculated total is {calculatedTotal:F2}. The reported InvoiceTotal is {reportedTotal:F2}.
The current discrepancy is: **{discrepancy:F2}**.
Your primary goal is to find all missing values in the text that account for this discrepancy.";

            // =============================== COMPREHENSIVE ESCAPING PREPARATION ===============================
            // Prepare patterns with different escaping levels for various contexts in the prompt
            
            // For JSON variable substitution (standard double escaping)
            string jsonSafeFormatPattern = EscapeRegexForJson(@"^(\d+\.?\d*)$");
            string jsonSafeInferredRegex = EscapeRegexForJson(@"Order Total:\s*[\$€£][\d.,]+");
            string jsonSafeOmissionRegex = EscapeRegexForJson(@"Free Shipping:\s*-?[\$€£]?(?<TotalDeduction>[\d,]+\.?\d*)");
            
            // For documentation examples (quadruple escaping for display)
            string docFormatPattern = EscapeRegexForDocumentation(@"^(\d+\.?\d*)$");
            string docOmissionRegex = EscapeRegexForDocumentation(@"Free Shipping:\s*-?[\$€£]?(?<TotalDeduction>[\d,]+\.?\d*)");
            string docInvoiceNoRegex = EscapeRegexForDocumentation(@"Invoice No:\s*(?<InvoiceNo>[l\d]+)");
            string docInvoiceTotalRegex = EscapeRegexForDocumentation(@"Invoice Total:\s*\$(?<InvoiceTotal>[l\d,O\.]+)");
            string docSupplierRegex = EscapeRegexForDocumentation(@"Supplier:\s*(?<SupplierName>.+?)\s*Address:");
            string docGiftCardRegex = EscapeRegexForDocumentation(@"Gift Card Amount:\s*-?[\$€£]?(?<TotalInsurance>[\d,]+\.?\d*)");
            string docMultilineSupplierRegex = EscapeRegexForDocumentation(@"^Budget.*\r\n\r\n(?<SupplierAddress>[A-Z\s\d\r\n\.,:()]*)");
            
            // For JSON example blocks (standard double escaping)
            string jsonExampleGiftCardRegex = EscapeRegexForJsonExample(@"Gift Card Amount:\s*-?[\$€£]?(?<TotalInsurance>[\d,]+\.?\d*)");
            string jsonExampleInvoiceNoRegex = EscapeRegexForJsonExample(@"Invoice No:\s*(?<InvoiceNo>[l\d]+)");
            string jsonExampleSupplierRegex = EscapeRegexForJsonExample(@"^Budget.*\\r\\n\\r\\n(?<SupplierAddress>[A-Z\\s\\d\\r\\n\\.\\,:()]*)");
            
            // For validation examples (triple escaping for code display)
            string validationInvoiceTotalRegex = EscapeRegexForValidation(@"Invoice Total:\s*\$(?<InvoiceTotal>[l\d,O\.]+)");
            string validationSupplierRegex = EscapeRegexForValidation(@"Supplier:\s*(?<SupplierName>.+?)\s*Address:");
            // ========================== END OF COMPREHENSIVE ESCAPING PREPARATION ==========================

            var prompt = $@"OBJECT-ORIENTED INVOICE ANALYSIS (V14.0 - Business Entity Framework):

**CONTEXT:**
You are analyzing a structured business document with defined object schemas. Think about the invoice as containing business entities with grouped and independent field relationships.

**1. EXTRACTED FIELDS:**
{currentJson}

**2. CONTEXT & COMPONENTS (if any):**
{annotatedContext}

{balanceCheckContext}

**3. COMPLETE OCR TEXT:**
{this.CleanTextForAnalysis(fileText)}"                + Environment.NewLine + Environment.NewLine +
                @"🎯 **V14.0 MANDATORY COMPLETION REQUIREMENTS**:

🚨 **CRITICAL**: FOR EVERY ERROR YOU REPORT, YOU MUST PROVIDE ALL OF THE FOLLOWING:

1. ✅ **field**: The exact field name (NEVER null)
2. ✅ **correct_value**: The actual value from the OCR text (NEVER null)
3. ✅ **error_type**: ""omission"" or ""format_correction"" or ""multi_field_omission"" (NEVER null)
4. ✅ **line_number**: The actual line number where the value appears (NEVER 0 or null)
5. ✅ **line_text**: The complete text of that line from the OCR (NEVER null)
6. ✅ **suggested_regex**: A working regex pattern that captures the value (NEVER null)
7. ✅ **reasoning**: Explain why this value was missed (NEVER null)

❌ **ABSOLUTELY FORBIDDEN**: 
   - ""Reasoning"": null
   - ""LineNumber"": 0
   - ""LineText"": null
   - ""SuggestedRegex"": null

🔥 **ERROR LEVEL REQUIREMENT**: Every field listed above MUST contain meaningful data.
If you cannot provide complete information for an error, DO NOT report that error.

" + Environment.NewLine +
                @"**🚨 CRITICAL REGEX REQUIREMENTS FOR PRODUCTION:**
⚠️ **MANDATORY**: ALL regex patterns MUST use named capture groups: (?<FieldName>pattern)
⚠️ **FORBIDDEN**: Never use numbered capture groups: (pattern) - these will fail in production
⚠️ **PRODUCTION CONSTRAINT**: The system requires named groups for field extraction
⚠️ **EXAMPLE CORRECT**: ""Free Shipping:\s*-?\$?(?<TotalDeduction>[\d,]+\.?\d*)""
⚠️ **EXAMPLE WRONG**: ""Free Shipping:\s*-?\$?([\d,]+\.?\d*)"" ← numbered group will fail

" + Environment.NewLine +
                @"**🏗️ OBJECT-ORIENTED ANALYSIS FRAMEWORK:**

You must analyze this invoice using object-oriented thinking. Identify business entities and their field relationships BEFORE creating regex patterns.

**GROUPED FIELD ENTITIES (Never split these - treat as single units):**

**A. Currency-Amount Objects:**
- **Fused Format**: ""EUR $123,00"", ""USD $1,234.56"", ""XCD $456.78""
  - Treatment: Single grouped field - currency correction affects amount format
  - Field Name: Use standard field name (e.g., ""InvoiceTotal"", ""SubTotal"")
  - Rule: Currency and decimal format are linked (EUR uses comma, USD uses period)
- **Separated Format**: ""Currency: EUR"" + ""Amount: 123.45"" on different lines
  - Treatment: Independent fields - can be corrected separately
  - Field Names: ""Currency"" and separate amount field

**B. Address Objects:**
- **Properties**: SupplierName, AddressLine1, AddressLine2, City, State, PostalCode, Country
- **Behavior**: Multi-line addresses are single objects
- **Rule**: If any address component is incomplete, expand boundaries to capture complete address block
- **Field Name**: ""SupplierAddress_MultiField_LineX"" with RequiresMultilineRegex: true

**C. Date-Time Objects:**
- **Fused Format**: ""July 15, 2024 3:53 PM""
  - Treatment: Single grouped field ""InvoiceDate""
- **Separated Format**: ""Date: July 15, 2024"" + ""Time: 3:53 PM""
  - Treatment: Independent fields

**INDEPENDENT FIELD ENTITIES (Can be corrected separately):**
- **Simple Fields**: InvoiceNo (when standalone), Currency (when separated from amount)
- **Financial Totals**: SubTotal, TotalInternalFreight, TotalOtherCost, TotalInsurance, TotalDeduction
- **Rule**: Each stands alone, OCR corrections don't affect other fields

**🧠 OBJECT COMPLETION RULES:**
1. **Analyze field relationships BEFORE creating regex patterns**
2. **Identify object boundaries (not just line boundaries)**
3. **If object spans multiple lines, use RequiresMultilineRegex: true**
4. **Prioritize object completeness over line-by-line parsing**
5. **One business object = One comprehensive regex pattern**

**OBJECT THINKING EXAMPLES:**
❌ Wrong: ""I see Currency 'EUR' on line 5 and Amount '123,00' on line 5 - create two separate fields""
✅ Right: ""I see a Currency-Amount object in fused format 'EUR $123,00' - create one field with currency-aware decimal formatting""

❌ Wrong: ""I see Address line 1 on line 10 and Address line 2 on line 11 - create separate fields""
✅ Right: ""I see an Address object spanning lines 10-11 - create one multi-line regex to capture complete address""

**YOUR ANALYSIS TASK:**
Analyze the OCR text and generate JSON objects in the `errors` array, applying object-oriented thinking for every business entity you discover.

---
### **Instructions for `omission` and `format_correction` types:**
*   **Definition:** Use these types when the correct value is **directly visible** in the OCR text but was missed or misformatted by my system.
*   **RULE: BE EXHAUSTIVE.** You must report **every single value** you find that was missed. If you see two ""Free Shipping"" lines, you MUST report two separate `omission` objects.
*   **RULE: IGNORE NOISE.** Some lines may contain multiple pieces of information (e.g., a payment method AND a deduction). If a financial value is present, report it.

#### **Caribbean Customs Field Mapping:**
*   **SUPPLIER-CAUSED REDUCTION** (e.g., 'Free Shipping', 'Discount'):
    *   Set `field` to ""TotalDeduction"". For `correct_value`, return the **absolute value** as a string (e.g., ""6.53"").
*   **CUSTOMER-CAUSED REDUCTION** (e.g., 'Gift Card', 'Store Credit'):
    *   Create an `omission` object: set `field` to ""TotalInsurance"", `correct_value` to the **negative absolute value** (e.g., ""-6.99"").

If you find no new omissions or corrections, return an empty errors array.

**🚨 CRITICAL EMPTY RESPONSE REQUIREMENT:**
If you return an empty errors array (no errors detected), you MUST include an ""explanation"" field in your JSON response explaining WHY no errors were found.

**MANDATORY RESPONSE FORMAT:**
- **If errors found**: {{ ""errors"": [error objects] }}
- **If NO errors found**: {{ ""errors"": [], ""explanation"": ""Detailed explanation of why no corrections are needed"" }}

**EXPLANATION REQUIREMENTS when returning empty errors array:**
1. ✅ **Document Recognition**: Confirm you recognize this as a valid invoice/business document
2. ✅ **Field Analysis**: State which financial fields you found in the OCR text 
3. ✅ **Balance Assessment**: Explain if the invoice appears mathematically balanced
4. ✅ **Missing Field Check**: Confirm whether all expected invoice fields are present
5. ✅ **Document Quality**: Assess if OCR quality allows accurate extraction

**EXAMPLE EXPLANATIONS:**
- ✅ ""This appears to be a well-structured MANGO invoice. I found SubTotal ($196.33), Tax ($13.74), and Total ($210.08) clearly visible in the OCR text. The invoice appears mathematically balanced and all major financial fields are extractable. No corrections needed.""
- ✅ ""This document contains mixed content (invoice + customs form) but the invoice portion is clearly structured. All financial fields (SubTotal, Tax, Total) are properly formatted and mathematically consistent. The extraction appears accurate.""
- ❌ ""No errors found."" (insufficient - does not explain document recognition or field analysis)

**CRITICAL**: Never return empty errors array without detailed explanation!";

            _logger.Information("🏁 **PROMPT_CREATION_COMPLETE (V14.0)**: Object-oriented business entity analysis prompt created with V14.0 mandatory completion requirements. Length: {PromptLength} characters.", prompt.Length);
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
  ""regex_pattern"": ""{EscapeRegexForJson(@"Free Shipping:\s*-?\$?(?<TotalDeduction>[\d,]+\.?\d*)")}"",
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
- **PRODUCTION REQUIREMENT**: ALWAYS use named capture groups: (?<FieldName>pattern)
- **NEVER use numbered capture groups**: (pattern) - these will fail in production
- Production code requires named capture groups for field extraction
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
- **PRODUCTION REQUIREMENT**: ALWAYS use named capture groups: (?<FieldName>pattern)
- **NEVER use numbered capture groups**: (pattern) - these will fail in production
- Production code requires named capture groups for field extraction
- Place currency symbols ($ € £) OUTSIDE the named capture group.
- Focus on fixing the specific reason for failure.";
        }

        /// <summary>
        /// Creates a prompt for DeepSeek to detect errors and omissions in product line item data.
        /// FOCUS: Only actual product line items with quantity, description, price, item codes
        /// </summary>
        private string CreateProductErrorDetectionPrompt(ShipmentInvoice invoice, string fileText)
        {
            _logger.Information("🚀 **PROMPT_CREATION_START (PRODUCT_DETECTION_V14.0)**: Creating object-oriented InvoiceDetail entity detection prompt");
            _logger.Debug("   - **DESIGN_INTENT**: Treat InvoiceDetail as complete business objects with grouped field properties");
            _logger.Debug("   - **OBJECT_ARCHITECTURE**: InvoiceDetail entities must be captured as complete units, never partial fields");

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
            _logger.Verbose("   - **INPUT_DATA_SERIALIZATION**: Product data for prompt: {ProductJson}", productsJson);

            // =============================== PRODUCT PROMPT ESCAPING PREPARATION ===============================
            // Prepare patterns with proper escaping for the product detection prompt
            
            // For JSON variable substitution in product prompt
            string productJsonQuantityRegex = EscapeRegexForJson(@"(?<Quantity>\d+)");
            string productJsonItemDescRegex = EscapeRegexForJson(@"(?<ItemDescription>[A-Za-z\s\d]+)");
            string productJsonUnitPriceRegex = EscapeRegexForJson(@"(?<UnitPrice>[\d.]+)");
            string productJsonMultiFieldExample = EscapeRegexForJson(@"(?<Quantity>\d+)\s+of:\s+(?<ItemDescription>.+?)\s*[\$](?<UnitPrice>[\d.]+)");
            
            // For documentation examples in product prompt (quadruple escaping)
            string productDocQuantityRegex = EscapeRegexForDocumentation(@"(?<Quantity>\d+)");
            string productDocItemDescRegex = EscapeRegexForDocumentation(@"(?<ItemDescription>[A-Za-z\s\d]+)");
            string productDocUnitPriceRegex = EscapeRegexForDocumentation(@"(?<UnitPrice>[\d.]+)");
            string productDocSingleLineRegex = EscapeRegexForDocumentation(@"(?<Quantity>\d+)\s*of:\s*(?<ItemDescription>.+?)\$(?<UnitPrice>\d+\.\d{2})");
            string productDocMultiLineRegex = EscapeRegexForDocumentation(@"(?<Quantity>\d+)\s*of:\s*(?<ItemDescription>.+?)\s*\$(?<UnitPrice>\d+\.\d{2})");
            string productDocNumberedGroupsRegex = EscapeRegexForDocumentation(@"(?<Quantity>\d+)\s*of:\s*(?<ItemDescription>[^\r\n]+)[\r\n\s]+(?<ItemDescription2>[^\$]+)\$(?<UnitPrice>\d+\.\d{2})");
            
            // For validation examples in product prompt
            string productValidationSingleRegex = EscapeRegexForValidation(@"(?<Quantity>\d+)\s*of:\s*(?<ItemDescription>.+?)\$(?<UnitPrice>\d+\.\d{2})");
            string productValidationMultiRegex = EscapeRegexForValidation(@"(?<Quantity>\d+)\s*of:\s*(?<ItemDescription>.+?)\s*\$(?<UnitPrice>\d+\.\d{2})");
            // ========================== END OF PRODUCT PROMPT ESCAPING PREPARATION ==========================

            _logger.Debug("   - **JSON_SAFE_REGEX_PREPARATION**: Escaped regex patterns for JSON examples prepared");

            // ============================== TEMPLATE BUILDING WITHOUT JSON COMPLEXITY ===============================
            // Following header prompt pattern - use string concatenation for complex JSON examples

            var prompt = "OBJECT-ORIENTED INVOICEDETAIL ENTITY DETECTION (V14.0):" + Environment.NewLine + Environment.NewLine +
                "**YOUR TASK**: Identify missing or incomplete InvoiceDetail business objects. Think of each line item as a complete business entity with related properties." + Environment.NewLine + Environment.NewLine +
                "**🏗️ INVOICEDETAIL OBJECT ARCHITECTURE**:" + Environment.NewLine + Environment.NewLine +
                "**InvoiceDetail Entity Properties**:" + Environment.NewLine +
                "- Quantity (required)" + Environment.NewLine +
                "- ItemDescription (required, may span multiple lines)" + Environment.NewLine +
                "- UnitPrice (required)" + Environment.NewLine +
                "- LineTotal (calculated: Quantity × UnitPrice)" + Environment.NewLine +
                "- ItemCode/SKU (optional)" + Environment.NewLine + Environment.NewLine +
                "**OBJECT COMPLETION RULES**:" + Environment.NewLine +
                "1. **Complete Object Capture**: If ANY property of an InvoiceDetail is incomplete, expand boundaries to capture the ENTIRE object" + Environment.NewLine +
                "2. **Multi-Line Descriptions**: ItemDescription spanning multiple lines = single object requiring multi-line regex" + Environment.NewLine +
                "3. **No Partial Objects**: Never create separate fields for parts of the same InvoiceDetail object" + Environment.NewLine +
                "4. **Object Validation**: Calculate LineTotal = Quantity × UnitPrice for each complete object" + Environment.NewLine + Environment.NewLine +
                "**OBJECT THINKING EXAMPLES**:" + Environment.NewLine +
                "❌ Wrong: \"I see ItemDescription on line 8 and continuation on line 9 - create two separate fields\"" + Environment.NewLine +
                "✅ Right: \"I see an InvoiceDetail object that spans lines 8-9 - create one multi-line regex to capture the complete object\"" + Environment.NewLine + Environment.NewLine +
                "❌ Wrong: \"Create InvoiceDetail_Line8_Quantity + InvoiceDetail_Line8_ItemDescription + InvoiceDetail_Line8_UnitPrice\"" + Environment.NewLine +
                "✅ Right: \"Create InvoiceDetail_MultiField_Line8 with captured_fields: [Quantity, ItemDescription, UnitPrice]\"" + Environment.NewLine + Environment.NewLine +
                "**EXTRACTED INVOICEDETAIL OBJECTS**:" + Environment.NewLine +
                productsJson + Environment.NewLine + Environment.NewLine +
                "**ORIGINAL INVOICE TEXT**:" + Environment.NewLine +
                this.CleanTextForAnalysis(fileText) + Environment.NewLine + Environment.NewLine +
                "**🎯 OBJECT-ORIENTED FIELD NAMING RULES**:" + Environment.NewLine + Environment.NewLine +
                "**COMPLETE INVOICEDETAIL OBJECTS (Preferred approach)**:" + Environment.NewLine +
                "- Field Name: InvoiceDetail_MultiField_LineX_Y (where X-Y spans all lines of the object)" + Environment.NewLine +
                "- Required: captured_fields array listing ALL properties [Quantity, ItemDescription, UnitPrice, LineTotal]" + Environment.NewLine +
                "- Required: RequiresMultilineRegex: true (if object spans multiple lines)" + Environment.NewLine +
                "- Required: MaxLines: N (number of lines the complete object spans)" + Environment.NewLine +
                "- Example: field: InvoiceDetail_MultiField_Line8_9, captured_fields: [Quantity, ItemDescription, UnitPrice]" + Environment.NewLine + Environment.NewLine +
                "**INDIVIDUAL FIELDS (Only when truly isolated)**:" + Environment.NewLine +
                "- Use only when a single InvoiceDetail property appears completely isolated" + Environment.NewLine +
                "- Pattern: InvoiceDetail_LineX_PropertyName" + Environment.NewLine +
                "- Example: InvoiceDetail_Line15_Quantity (if quantity appears alone without description or price)" + Environment.NewLine + Environment.NewLine +
                "**OBJECT COMPLETION VALIDATION**:" + Environment.NewLine +
                "Before creating individual fields, ask: \"Is this property part of a larger InvoiceDetail object?\"" + Environment.NewLine +
                "If YES → Create complete object with InvoiceDetail_MultiField_LineX_Y" + Environment.NewLine +
                "If NO → Individual field acceptable" + Environment.NewLine + Environment.NewLine +
                "**EXAMPLE - CORRECT MULTI-FIELD APPROACH**:" + Environment.NewLine +
                "Line: 5607416 AWLCARE 073240/1PTUS 2 PC $12.54 /PC $25.08" + Environment.NewLine +
                "✅ CORRECT: ONE error with field: InvoiceDetail_MultiField_Line8, captured_fields: [ItemCode, ItemDescription, Quantity, UnitPrice, LineTotal]" + Environment.NewLine +
                "❌ WRONG: 5 separate errors with individual field names" + Environment.NewLine + Environment.NewLine +
                "**EXAMPLE - MULTI-LINE OBJECT COMPLETION**:" + Environment.NewLine +
                "Line 8: \"3 of: MESAILUP 16 Inch LED Lighted Liquor Bottle Display\"" + Environment.NewLine +
                "Line 9: \"Lighting Shelves with Remote Control (2 Tier, 16 inch) $39.99\"" + Environment.NewLine +
                "✅ CORRECT: field: InvoiceDetail_MultiField_Line8_9, RequiresMultilineRegex: true, MaxLines: 2" + Environment.NewLine +
                "✅ CORRECT: captured_fields: [Quantity, ItemDescription, UnitPrice]" + Environment.NewLine +
                "❌ WRONG: Separate InvoiceDetail_Line8_ItemDescription + InvoiceDetail_Line9_ItemDescription" + Environment.NewLine + Environment.NewLine +
                "**EXAMPLE - CURRENCY-AMOUNT OBJECT**:" + Environment.NewLine +
                "Line 12: \"Total: EUR $123,00\" (fused format)" + Environment.NewLine +
                "✅ CORRECT: field: InvoiceTotal, currency-aware formatting (comma decimal for EUR)" + Environment.NewLine +
                "❌ WRONG: Separate Currency + InvoiceTotal fields" + Environment.NewLine + Environment.NewLine +
                "**GENERAL OBJECT-AWARE PATTERNS**:" + Environment.NewLine +
                "Use .+? for flexible text capture within object boundaries." + Environment.NewLine +
                $"Use {productDocQuantityRegex} for numbers, {productDocItemDescRegex} for text patterns." + Environment.NewLine +
                "Design patterns to capture complete business objects, not fragments." + Environment.NewLine +
                "Prioritize object completeness over pattern simplicity." + Environment.NewLine + Environment.NewLine +
                "**🔍 MANDATORY REGEX VALIDATION AND MULTI-LINE REQUIREMENTS**:" + Environment.NewLine +
                "Before submitting ANY regex pattern, you MUST:" + Environment.NewLine + Environment.NewLine +
                "**1. MULTI-LINE DETECTION AND RequiresMultilineRegex SETTING**:" + Environment.NewLine +
                "   - Check if LineText contains \\\\n (newlines)" + Environment.NewLine +
                "   - If YES: Set \"RequiresMultilineRegex\": true (enables RegexOptions.Singleline in production)" + Environment.NewLine +
                "   - If NO: Set \"RequiresMultilineRegex\": false (uses default regex mode)" + Environment.NewLine + Environment.NewLine +
                "**2. MULTI-LINE FIELD APPEND STRATEGY**:" + Environment.NewLine +
                "   For ItemDescription spanning multiple lines, use SEQUENTIAL NAMED CAPTURE GROUPS:" + Environment.NewLine +
                $"   - First line: {productDocItemDescRegex}" + Environment.NewLine +
                "   - Second line: (?<ItemDescription2>second line text)" + Environment.NewLine +
                "   - Third line: (?<ItemDescription3>third line text)" + Environment.NewLine +
                "   - Production will create ItemDescription2, ItemDescription3 fields with Append=true" + Environment.NewLine +
                "   - **CRITICAL**: Each must be a NAMED capture group with (?<name>pattern) syntax" + Environment.NewLine + Environment.NewLine +
                "**3. PATTERN TESTING VALIDATION**:" + Environment.NewLine +
                "   - Mentally apply your regex to the exact LineText provided" + Environment.NewLine +
                "   - Does it match the starting text pattern?" + Environment.NewLine +
                "   - Does it capture all required named groups?" + Environment.NewLine +
                "   - Does it stop at the correct ending pattern?" + Environment.NewLine +
                "   - For multi-line: Does .+? with Singleline mode capture across newlines?" + Environment.NewLine + Environment.NewLine +
                "**4. VALUE EXTRACTION VERIFICATION**:" + Environment.NewLine +
                "   - Check that captured groups match the values in CorrectValue" + Environment.NewLine +
                "   - For JSON CorrectValue, ensure all fields in captured_fields are extractable" + Environment.NewLine +
                "   - For multi-line descriptions, verify complete text capture across all lines" + Environment.NewLine + Environment.NewLine +
                "**MULTI-LINE EXAMPLES**:" + Environment.NewLine + Environment.NewLine +
                "**❌ WRONG SINGLE-LINE APPROACH**:" + Environment.NewLine +
                $"Pattern: \"{productDocSingleLineRegex}\"" + Environment.NewLine +
                "RequiresMultilineRegex: false" + Environment.NewLine +
                $"Applied to: \"3 of: PRODUCT NAME{EscapeRegexForDocumentation(@"\n")}CONTINUATION $39.99\"" + Environment.NewLine +
                "Problem: .+? stops at newline, only captures \"PRODUCT NAME\", misses \"CONTINUATION\"" + Environment.NewLine + Environment.NewLine +
                "**✅ CORRECT SEQUENTIAL NAMED GROUPS APPROACH**:" + Environment.NewLine +
                $"Pattern: \"{productDocNumberedGroupsRegex}\"" + Environment.NewLine +
                "RequiresMultilineRegex: true" + Environment.NewLine +
                $"Applied to: \"3 of: PRODUCT NAME{EscapeRegexForDocumentation(@"\n")}CONTINUATION $39.99\"" + Environment.NewLine +
                "Result: ItemDescription=\"PRODUCT NAME\", ItemDescription2=\"CONTINUATION\" (appends with space)" + Environment.NewLine +
                "captured_fields: [\"Quantity\", \"ItemDescription\", \"ItemDescription2\", \"UnitPrice\"]" + Environment.NewLine +
                "**NOTE**: All groups use named capture syntax (?<name>pattern) - NEVER use numbered groups (pattern)" + Environment.NewLine + Environment.NewLine +
                "**🚨 CRITICAL PRODUCTION RULES**:" + Environment.NewLine +
                $"1. If LineText contains {EscapeRegexForDocumentation(@"\n")}, you MUST set RequiresMultilineRegex: true" + Environment.NewLine +
                "2. **PRODUCTION CONSTRAINT**: For multi-line text, you MUST use sequential named groups (ItemDescription, ItemDescription2, etc.)" + Environment.NewLine +
                "3. **MANDATORY**: ALL capture groups MUST use named syntax: (?<FieldName>pattern)" + Environment.NewLine +
                "4. **FORBIDDEN**: Never use numbered capture groups: (pattern) - production code will fail" + Environment.NewLine +
                "5. Test your pattern mentally before submitting - if it can't extract CorrectValue, fix it!" + Environment.NewLine +
                "6. Include ALL named groups in captured_fields array" + Environment.NewLine + Environment.NewLine +
                "**OCR CHARACTER CONFUSION HANDLING**:" + Environment.NewLine +
                "When you find OCR character confusion (e.g. Email should be ENAMEL, BOTTOMK NT should be BOTTOM PAINT):" + Environment.NewLine +
                "- suggested_regex: Must match the ACTUAL text in the source (including OCR errors)" + Environment.NewLine +
                "- pattern: The incorrect OCR text to find and replace" + Environment.NewLine +
                "- replacement: The correct text to replace it with" + Environment.NewLine +
                "- error_type: Use format_correction for character/word confusion" + Environment.NewLine + Environment.NewLine +
                "🎯 **V14.0 MANDATORY COMPLETION REQUIREMENTS**:" + Environment.NewLine + Environment.NewLine +
                "🚨 **CRITICAL**: FOR EVERY ERROR YOU REPORT, YOU MUST PROVIDE ALL OF THE FOLLOWING:" + Environment.NewLine + Environment.NewLine +
                "1. ✅ **field**: The exact field name (NEVER null)" + Environment.NewLine +
                "2. ✅ **correct_value**: The actual value from the OCR text (NEVER null)" + Environment.NewLine +
                "3. ✅ **error_type**: \"omission\" or \"format_correction\" or \"multi_field_omission\" (NEVER null)" + Environment.NewLine +
                "4. ✅ **line_number**: The actual line number where the value appears (NEVER 0 or null)" + Environment.NewLine +
                "5. ✅ **line_text**: The complete text of that line from the OCR (NEVER null)" + Environment.NewLine +
                "6. ✅ **suggested_regex**: A working regex pattern that captures the value (NEVER null)" + Environment.NewLine +
                "7. ✅ **reasoning**: Explain why this value was missed (NEVER null)" + Environment.NewLine + Environment.NewLine +
                "❌ **ABSOLUTELY FORBIDDEN**: " + Environment.NewLine +
                "   - \"Reasoning\": null" + Environment.NewLine +
                "   - \"LineNumber\": 0" + Environment.NewLine +
                "   - \"LineText\": null" + Environment.NewLine +
                "   - \"SuggestedRegex\": null" + Environment.NewLine + Environment.NewLine +
                "🔥 **ERROR LEVEL REQUIREMENT**: Every field listed above MUST contain meaningful data." + Environment.NewLine +
                "If you cannot provide complete information for an error, DO NOT report that error." + Environment.NewLine + Environment.NewLine +
                "**CRITICAL REQUIREMENTS FOR EACH IDENTIFIED ERROR/OMISSION**:" + Environment.NewLine +
                "1. field: Use format InvoiceDetail_LineX_FieldName or InvoiceDetail_MultiField_LineX. X MUST be the 1-based line number from the ORIGINAL INVOICE TEXT where the item/field is found." + Environment.NewLine +
                "2. extracted_value: Current value from EXTRACTED PRODUCT DATA if a corresponding item/field is found; otherwise empty string or null for omissions." + Environment.NewLine +
                "3. correct_value: Correct value from ORIGINAL INVOICE TEXT." + Environment.NewLine +
                "4. line_text: EXACT line(s) from ORIGINAL INVOICE TEXT where the error/omission is evident." + Environment.NewLine +
                "5. line_number: 1-based starting line number in ORIGINAL INVOICE TEXT corresponding to line_text." + Environment.NewLine +
                "6. error_type: omission, omitted_line_item, format_error, value_error, calculation_error, character_confusion, multi_field_omission, format_correction." + Environment.NewLine +
                "7. confidence: Confidence score between 0.0 and 1.0." + Environment.NewLine +
                "8. reasoning: Explanation of the error/omission." + Environment.NewLine +
                "9. entity_type: Must be InvoiceDetails for product line items." + Environment.NewLine + Environment.NewLine +
                "**RESPONSE FORMAT (Strict JSON array of error objects under errors key)**:" + Environment.NewLine + Environment.NewLine +
                "**REQUIRED FIELDS FOR ALL ERRORS**:" + Environment.NewLine +
                "- field: Field name (InvoiceDetail_LineX_FieldName or InvoiceDetail_MultiField_LineX)" + Environment.NewLine +
                "- extracted_value: Current extracted value or empty string" + Environment.NewLine +
                "- correct_value: Correct value from invoice text" + Environment.NewLine +
                "- line_text: Exact line from invoice where error appears" + Environment.NewLine +
                "- line_number: Line number in original text" + Environment.NewLine +
                "- confidence: Score 0.0-1.0" + Environment.NewLine +
                "- error_type: omission, character_confusion, multi_field_omission, format_correction" + Environment.NewLine +
                "- entity_type: Must be InvoiceDetails" + Environment.NewLine +
                "- suggested_regex: REQUIRED - C# regex pattern with NAMED capture groups (?<FieldName>pattern)" + Environment.NewLine +
                "- reasoning: Brief explanation" + Environment.NewLine + Environment.NewLine +
                "**ADDITIONAL FIELDS FOR MULTI-FIELD ERRORS**:" + Environment.NewLine +
                "- captured_fields: Array of field names captured by the regex" + Environment.NewLine +
                "- field_corrections: Array of pattern/replacement corrections (optional)" + Environment.NewLine + Environment.NewLine +
                "🚨🚨🚨 CRITICAL REQUIREMENT - READ FIRST 🚨🚨🚨" + Environment.NewLine +
                "FOR MULTI_FIELD_OMISSION ERRORS: PATTERNS MUST BE 100% GENERALIZABLE!" + Environment.NewLine + Environment.NewLine +
                "❌ IMMEDIATE REJECTION CRITERIA - DO NOT SUBMIT IF YOUR PATTERN CONTAINS:" + Environment.NewLine +
                "- ANY specific product names in ItemDescription patterns" + Environment.NewLine +
                "- ANY hardcoded text like \"Circle design\", \"Beaded thread\", \"High-waist\", etc." + Environment.NewLine +
                "- ANY patterns that only work for ONE specific product" + Environment.NewLine +
                "- ANY literal product words instead of character classes" + Environment.NewLine + Environment.NewLine +
                "✅ MANDATORY PATTERN STYLE FOR MULTI-FIELD ERRORS:" + Environment.NewLine +
                "- ItemDescription: [A-Za-z\\\\s]+ (character classes ONLY, NO product names)" + Environment.NewLine +
                "- ItemCode: \\\\d+\\\\w+ (generic alphanumeric pattern)" + Environment.NewLine +
                "- UnitPrice: \\\\d+\\\\.\\\\d{2} (generic decimal pattern)" + Environment.NewLine +
                "- Quantity: \\\\d+ (generic number pattern)" + Environment.NewLine + Environment.NewLine +
                "🔥 MANDATORY TEST: Ask yourself \"Will this work for 10,000 different products?\"" + Environment.NewLine +
                "If the answer is NO, you MUST generalize it further!" + Environment.NewLine + Environment.NewLine +
                "❌ **FORBIDDEN EXAMPLES (WILL BE REJECTED)**:" + Environment.NewLine +
                "- \"(?<ItemDescription>Circle design ma[\\\\s\\\\S]*?xi earrings)\"" + Environment.NewLine +
                "- \"(?<ItemDescription>Beaded thread earrings)\"" + Environment.NewLine +
                "- \"(?<ItemDescription>High-waist straight shorts)\"" + Environment.NewLine + Environment.NewLine +
                "✅ **REQUIRED EXAMPLES (USE THIS STYLE)**:" + Environment.NewLine +
                "- \"(?<ItemDescription>[A-Za-z\\\\s]+(?:\\\\n[A-Za-z\\\\s]+)*)\"" + Environment.NewLine +
                "- \"(?<ItemDescription>[A-Za-z0-9\\\\s\\\\-,\\\\.]+)\"" + Environment.NewLine + Environment.NewLine +
                "**EXAMPLES**:" + Environment.NewLine +
                "Individual field: Use field name like InvoiceDetail_Line15_Quantity" + Environment.NewLine +
                "Multi-field line: Use field name like InvoiceDetail_MultiField_Line8 with captured_fields array" + Environment.NewLine + Environment.NewLine +
                "**CRITICAL**: Every error MUST include suggested_regex. For multi-field lines, regex MUST capture ALL fields in captured_fields." + Environment.NewLine + Environment.NewLine +
                "**🚨 FINAL PRODUCTION VALIDATION**: Before submitting response, verify EVERY suggested_regex:" + Environment.NewLine +
                "1. ✅ Pattern matches line_text completely" + Environment.NewLine +
                "2. ✅ Extracts correct_value accurately" + Environment.NewLine +
                "3. ✅ **CRITICAL**: If line_text contains newlines, MUST use sequential named groups (ItemDescription, ItemDescription2, etc.)" + Environment.NewLine +
                "4. ✅ **MANDATORY**: ALL patterns MUST use named capture groups (?<FieldName>pattern) - never numbered (pattern)" + Environment.NewLine +
                "5. ✅ Include ALL named groups in captured_fields array" + Environment.NewLine + Environment.NewLine +
                "Return format: errors array with suggested_regex field required for all responses.";

            _logger.Information("🏁 **PROMPT_CREATION_COMPLETE (PRODUCT_DETECTION_V14.0)**: Object-oriented InvoiceDetail entity detection prompt created with complete business object architecture. Length: {PromptLength} characters.", prompt.Length);
            return prompt;
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
