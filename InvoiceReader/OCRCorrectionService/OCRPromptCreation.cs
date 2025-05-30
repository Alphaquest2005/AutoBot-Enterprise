// File: OCRCorrectionService/OCRPromptCreation.cs
using System;
using System.Text.Json;
using System.Linq;
using EntryDataDS.Business.Entities;

namespace WaterNut.DataSpace
{
    public partial class OCRCorrectionService
    {
        #region Prompt Creation Methods

        /// <summary>
        /// Enhanced prompt that specifically teaches DeepSeek to recognize gift cards and negative discounts
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

            return $@"OCR HEADER FIELD ERROR DETECTION:

Compare the extracted header data with the original invoice text and identify discrepancies.

EXTRACTED HEADER DATA:
{headerJson}

ORIGINAL TEXT:
{CleanTextForAnalysis(fileText)}

FOCUS AREAS:
1. Invoice totals and subtotals - look for decimal separator errors (10,00 vs 10.00)
2. Supplier information - verify names and addresses match exactly
3. Currency symbols and formatting
4. Fee breakdowns (shipping, taxes, insurance, deductions)

CRITICAL FINANCIAL VALIDATION RULES:
The invoice must follow this exact equation:
SubTotal + TotalInternalFreight + TotalOtherCost - TotalDeduction = InvoiceTotal

DEDUCTION IDENTIFICATION RULES:
- Gift card applications: ADD to TotalDeduction (not InvoiceTotal)
- Store credit usage: ADD to TotalDeduction (not InvoiceTotal)
- Promotional discounts: ADD to TotalDeduction (not InvoiceTotal)
- Any ""applied credit"" or ""balance used"": ADD to TotalDeduction (not InvoiceTotal)

DEDUCTION KEYWORDS TO IDENTIFY:
Look for these terms and treat as TotalDeduction (positive values):
- ""Gift card applied"", ""Gift certificate used"", ""GC Applied""
- ""Amazon gift card"", ""Store gift card"", ""Gift balance""
- ""Store credit"", ""Account credit"", ""Credit applied""
- ""Promotional credit"", ""Promo code"", ""Coupon applied""
- ""Paid with gift card"", ""Gift card payment""

CRITICAL - GIFT CARD AND DISCOUNT RECOGNITION:
- Gift cards, store credit, and promotional discounts should be mapped to TotalDeduction
- Look for patterns like: ""Gift Card: -$6.99"", ""Store Credit: -$10.00""
- IMPORTANT CALCULATION LOGIC: When you see ""-$6.99"" or ""$-6.99"" in the text:
  * TotalDeduction = 6.99 (store as positive: TotalDeduction = value * -1)
  * This negative amount is ALREADY APPLIED to the displayed InvoiceTotal
  * Do NOT suggest increasing InvoiceTotal - the merchant already subtracted the discount

COMMON OCR ERRORS:
- Decimal separators: 123,45 → 123.45
- Character confusion: 1↔l, 1↔I, 0↔O, 5↔S, 6↔G, 8↔B
- Currency symbols: $ vs S, € vs C
- Missing decimals: 1000 → 10.00
- Negative signs: Missing '-' in front of discounts

RESPONSE FORMAT (JSON only - follow this EXACT structure):
{{
  ""errors"": [
    {{
      ""field"": ""FieldName"",
      ""extracted_value"": ""CurrentWrongValue"",
      ""correct_value"": ""CorrectValueFromText"",
      ""confidence"": 0.95,
      ""error_type"": ""decimal_separator"",
      ""reasoning"": ""Brief explanation of what was wrong and how it was corrected""
    }}
  ]
}}

Return empty array if no errors: {{""errors"": []}}";
        }

        /// <summary>
        /// Creates specialized prompt for product-level error detection
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

            return $@"OCR PRODUCT DATA ERROR DETECTION:

Compare the extracted product data with the original invoice text and identify discrepancies in quantities, prices, and calculations.

EXTRACTED PRODUCT DATA:
{productsJson}

ORIGINAL TEXT:
{CleanTextForAnalysis(fileText)}

VALIDATION FOCUS:
1. QUANTITIES: Verify each item quantity matches the text exactly
   - Watch for: 1↔l, 5↔S, 6↔G, 8↔B, 0↔O
   - Check: Reasonable quantities (not negative, not extremely large)

2. UNIT PRICES: Verify unit costs are correct
   - Watch for: Decimal separators (12,50 vs 12.50)
   - Check: Currency formatting ($12.50 vs $1250)
   - Validate: Prices are reasonable (not negative, not zero for products)

3. LINE TOTALS: Verify calculations
   - Formula: (Quantity × Unit Price) - Discount = Line Total
   - Check: Math is correct within $0.01

4. ITEM DESCRIPTIONS: Verify product names are complete and correct
   - Check: No truncated descriptions
   - Watch for: Character substitutions that change meaning

COMMON PRODUCT OCR ERRORS:
- Quantity errors: 12 → 1Z, 5 → S, 10 → IO
- Price errors: $12.50 → $1Z.S0, 29.99 → Z9.99
- Decimal issues: 12,50 → 12.50, 1000 → 10.00
- Missing digits: $12.5 → $12.50, .99 → 0.99

RESPONSE FORMAT (JSON only - follow this EXACT structure):
{{
  ""errors"": [
    {{
      ""field"": ""InvoiceDetail_Line1_Quantity"",
      ""extracted_value"": ""1Z"",
      ""correct_value"": ""12"",
      ""confidence"": 0.90,
      ""error_type"": ""character_confusion"",
      ""reasoning"": ""OCR confused 'Z' with '2' in quantity field""
    }}
  ]
}}

Return empty array if no errors: {{""errors"": []}}";
        }

        #endregion
    }
}