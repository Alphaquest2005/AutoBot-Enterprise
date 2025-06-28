# Invoice Diagnostic Report - 03152025_TOTAL AMOUNT

## 📋 **File Identification**
- **Invoice File**: 03152025_TOTAL AMOUNT.txt
- **Test Version**: v1.1
- **Test Date**: 2025-06-28 15:02:56
- **Vendor Category**: Other
- **Issue Category**: DEEPSEEK_BALANCE_ERROR

## 🎯 **Issue Summary**
- **Primary Issue**: DEEPSEEK_BALANCE_ERROR
- **Severity**: HIGH
- **Balance Error**: 0.0000
- **Total Issues Found**: 1

## 🏗️ **Design Challenge Context**
### **System Architecture**:
- **Dual-LLM Comparison**: Claude Code (JSON extraction) vs DeepSeek (error detection)
- **Caribbean Customs Rules**: TotalInsurance (negative customer reductions), TotalDeduction (positive supplier reductions)
- **OCR Section Precedence**: Single Column > Ripped Text > SparseText
- **Balance Formula**: SubTotal + TotalInternalFreight + TotalOtherCost + TotalInsurance - TotalDeduction = InvoiceTotal
- **Claude Model**: Claude 4 (Sonnet), temperature 0.1 via Claude Code SDK
- **DeepSeek Model**: deepseek-chat, temperature 0.3, max tokens 8192

### **🔄 Version Evolution (v1.0 → v1.1)**:
#### **v1.0 Analysis Results**:
- **Primary Issue Found**: False positive credit detection
- **Root Cause**: System flagged 'Credit Card transactions' as customer credit
- **Impact**: Payment method info mistaken for TotalInsurance mapping

#### **v1.1 Design Changes**:
- **Enhanced Credit Detection**: Distinguish actual credits from payment methods
- **Actual Credits**: 'Store Credit', 'Account Credit', 'Gift Card' → TotalInsurance
- **Payment Methods**: 'Credit Card transactions', 'Visa ending in' → EXCLUDED
- **Smart Pattern Matching**: Context-aware credit classification

#### **Expected v1.1 Behavior**:
- **Amazon File**: Should show NO_ISSUES_FOUND (false positive eliminated)
- **Baseline File**: Should maintain perfect performance
- **Future LLM Guidance**: Use this pattern for credit vs payment method distinction


## 📄 **Document Structure Analysis**
- **Order Count**: 0
- **OCR Sections**: Single Column, Ripped Text, SparseText
- **Financial Patterns**: Tax
- **Special Characteristics**: 

## 📊 **Test Results**

### **Claude Code JSON Extraction Results**:
```json
{
  "extractedFields": {
    "InvoiceTotal": 210.08,
    "SubTotal": 196.33,
    "TotalInternalFreight": 13.0,
    "TotalOtherCost": 0.0,
    "TotalInsurance": 0.0,
    "TotalDeduction": 0.0
  },
  "validation": {
    "CalculatedTotal": 210.08,
    "BalanceCheck": 0.0,
    "ValidationPassed": true,
    "Formula": "SubTotal + TotalInternalFreight + TotalOtherCost + TotalInsurance - TotalDeduction"
  },
  "confidence": "high"
}
```

### **DeepSeek AI Detection Results**:
```json
{
  "detectedErrors": [
    {
      "Field": "InvoiceNo",
      "ExtractedValue": null,
      "CorrectValue": "UCSJIB6",
      "Confidence": 0.8,
      "ErrorType": "omission",
      "Reasoning": "The invoice number was missed in the initial extraction but is clearly visible in the order details section.",
      "LineNumber": 62,
      "LineText": "Order number: UCSJIB6",
      "ContextLinesBefore": [
        "",
        "3 Pack of beaded glasses chains",
        "ref. 470158569952",
        "",
        "12"
      ],
      "ContextLinesAfter": [
        "12",
        "",
        "12",
        "",
        "XL"
      ],
      "RequiresMultilineRegex": false,
      "SuggestedRegex": "Order number:\\s*(?<InvoiceNo>[A-Z0-9]+)",
      "CapturedFields": [],
      "FieldCorrections": [],
      "Pattern": null,
      "Replacement": null
    },
    {
      "Field": "InvoiceDate",
      "ExtractedValue": null,
      "CorrectValue": "Tuesday, July 23, 2024 at 03:42 PM EDT",
      "Confidence": 0.8,
      "ErrorType": "omission",
      "Reasoning": "The invoice date was not extracted but is present in the email header section.",
      "LineNumber": 60,
      "LineText": "Date: Tuesday, July 23, 2024 at 03:42 PM EDT",
      "ContextLinesBefore": [
        "Circle design maxi earrings",
        "ref. S700253399PL",
        "",
        "3 Pack of beaded glasses chains",
        "ref. 470158569952"
      ],
      "ContextLinesAfter": [
        "12",
        "",
        "12",
        "",
        "12"
      ],
      "RequiresMultilineRegex": false,
      "SuggestedRegex": "Date:\\s*(?<InvoiceDate>.+)",
      "CapturedFields": [],
      "FieldCorrections": [],
      "Pattern": null,
      "Replacement": null
    },
    {
      "Field": "SupplierName",
      "ExtractedValue": null,
      "CorrectValue": "MANGO OUTLET",
      "Confidence": 0.8,
      "ErrorType": "omission",
      "Reasoning": "The supplier name was missed but is clearly stated in the email header.",
      "LineNumber": 59,
      "LineText": "From: MANGO OUTLET (noreply@mango.com)",
      "ContextLinesBefore": [
        "",
        "Circle design maxi earrings",
        "ref. S700253399PL",
        "",
        "3 Pack of beaded glasses chains"
      ],
      "ContextLinesAfter": [
        "",
        "12",
        "",
        "12",
        ""
      ],
      "RequiresMultilineRegex": false,
      "SuggestedRegex": "From:\\s*(?<SupplierName>[^\\(]+)",
      "CapturedFields": [],
      "FieldCorrections": [],
      "Pattern": null,
      "Replacement": null
    },
    {
      "Field": "Currency",
      "ExtractedValue": null,
      "CorrectValue": "US$",
      "Confidence": 0.8,
      "ErrorType": "omission",
      "Reasoning": "The currency indicator was missed in the initial extraction but appears consistently with amounts.",
      "LineNumber": 10,
      "LineText": "Subtotal USS 196.33",
      "ContextLinesBefore": [
        "",
        "ref. 570003742302 ts",
        ": L US$",
        "Long jumpsuit with back opening @ 18.99",
        "ref. 570502122243"
      ],
      "ContextLinesAfter": [
        "Mixed spike necklace 10,99",
        "ref. 57077738990R",
        "Subtotal USS 196.33",
        "Shipping & Handling Free",
        "Estimated Tax US$ 13.74"
      ],
      "RequiresMultilineRegex": false,
      "SuggestedRegex": "Subtotal\\s*(?<Currency>[A-Z]{2,3}\\$?)",
      "CapturedFields": [],
      "FieldCorrections": [],
      "Pattern": null,
      "Replacement": null
    },
    {
      "Field": "SubTotal",
      "ExtractedValue": null,
      "CorrectValue": "196.33",
      "Confidence": 0.8,
      "ErrorType": "omission",
      "Reasoning": "The subtotal was missed despite being clearly labeled in the financial summary.",
      "LineNumber": 10,
      "LineText": "Subtotal USS 196.33",
      "ContextLinesBefore": [
        "",
        "ref. 570003742302 ts",
        ": L US$",
        "Long jumpsuit with back opening @ 18.99",
        "ref. 570502122243"
      ],
      "ContextLinesAfter": [
        "Mixed spike necklace 10,99",
        "ref. 57077738990R",
        "Subtotal USS 196.33",
        "Shipping & Handling Free",
        "Estimated Tax US$ 13.74"
      ],
      "RequiresMultilineRegex": false,
      "SuggestedRegex": "Subtotal\\s*[A-Z]{2,3}\\$?\\s*(?<SubTotal>[\\d,\\.]+)",
      "CapturedFields": [],
      "FieldCorrections": [],
      "Pattern": null,
      "Replacement": null
    },
    {
      "Field": "TotalDeduction",
      "ExtractedValue": null,
      "CorrectValue": "0.00",
      "Confidence": 0.8,
      "ErrorType": "omission",
      "Reasoning": "Free shipping should be recorded as a $0.00 deduction.",
      "LineNumber": 11,
      "LineText": "Shipping & Handling Free",
      "ContextLinesBefore": [
        "ref. 570003742302 ts",
        ": L US$",
        "Long jumpsuit with back opening @ 18.99",
        "ref. 570502122243",
        "r U & US$"
      ],
      "ContextLinesAfter": [
        "ref. 57077738990R",
        "Subtotal USS 196.33",
        "Shipping & Handling Free",
        "Estimated Tax US$ 13.74",
        "TOTAL AMOUNT US$ 210.08"
      ],
      "RequiresMultilineRegex": false,
      "SuggestedRegex": "Shipping & Handling\\s*(?<TotalDeduction>Free|0\\.00)",
      "CapturedFields": [],
      "FieldCorrections": [],
      "Pattern": null,
      "Replacement": null
    },
    {
      "Field": "InvoiceTotal",
      "ExtractedValue": null,
      "CorrectValue": "210.08",
      "Confidence": 0.8,
      "ErrorType": "omission",
      "Reasoning": "The total amount was missed despite being clearly labeled in the financial summary.",
      "LineNumber": 13,
      "LineText": "TOTAL AMOUNT US$ 210.08",
      "ContextLinesBefore": [
        "Long jumpsuit with back opening @ 18.99",
        "ref. 570502122243",
        "r U & US$",
        "Mixed spike necklace 10,99",
        "ref. 57077738990R"
      ],
      "ContextLinesAfter": [
        "Shipping & Handling Free",
        "Estimated Tax US$ 13.74",
        "TOTAL AMOUNT US$ 210.08",
        "",
        "For any enquiries, doubts or suggestions, you can contact our Customer Service."
      ],
      "RequiresMultilineRegex": false,
      "SuggestedRegex": "TOTAL AMOUNT\\s*[A-Z]{2,3}\\$?\\s*(?<InvoiceTotal>[\\d,\\.]+)",
      "CapturedFields": [],
      "FieldCorrections": [],
      "Pattern": null,
      "Replacement": null
    },
    {
      "Field": "InvoiceDetail_MultiField_Line3_5",
      "ExtractedValue": "",
      "CorrectValue": "{\n        \"Quantity\": \"\",\n        \"ItemDescription\": \"High-waist straight shorts\",\n        \"UnitPrice\": \"3.3\",\n        \"ItemCode\": \"570003742302\",\n        \"Size\": \"L\"\n      }",
      "Confidence": 0.95,
      "ErrorType": "multi_field_omission",
      "Reasoning": "Complete InvoiceDetail object spans multiple lines with description, price, reference code and size",
      "LineNumber": 3,
      "LineText": "High-waist straight shorts \" 3.3\n\nref. 570003742302 ts\n: L US$",
      "ContextLinesBefore": [
        "------------------------------------------Single Column-------------------------",
        " "
      ],
      "ContextLinesAfter": [
        "High-waist straight shorts “ 3.3",
        "",
        "ref. 570003742302 ts",
        ": L US$",
        "Long jumpsuit with back opening @ 18.99"
      ],
      "RequiresMultilineRegex": false,
      "SuggestedRegex": "(?<ItemDescription>[A-Za-z\\s]+)\\s*[\"@]\\s*(?<UnitPrice>\\d+\\.\\d{2})[\\s\\S]*?ref\\.\\s*(?<ItemCode>\\d+)[\\s\\S]*?(?<Size>[A-Z]+)",
      "CapturedFields": [
        "ItemDescription",
        "UnitPrice",
        "ItemCode",
        "Size"
      ],
      "FieldCorrections": [],
      "Pattern": null,
      "Replacement": null
    },
    {
      "Field": "InvoiceDetail_MultiField_Line6_8",
      "ExtractedValue": "",
      "CorrectValue": "{\n        \"Quantity\": \"\",\n        \"ItemDescription\": \"Long jumpsuit with back opening\",\n        \"UnitPrice\": \"18.99\",\n        \"ItemCode\": \"570502122243\",\n        \"Size\": \"U\"\n      }",
      "Confidence": 0.95,
      "ErrorType": "multi_field_omission",
      "Reasoning": "Complete object with description spanning multiple lines, price, reference code and size",
      "LineNumber": 6,
      "LineText": "Long jumpsuit with back opening @ 18.99\nref. 570502122243\nr U & US$",
      "ContextLinesBefore": [
        "------------------------------------------Single Column-------------------------",
        " ",
        "",
        "High-waist straight shorts “ 3.3",
        ""
      ],
      "ContextLinesAfter": [
        ": L US$",
        "Long jumpsuit with back opening @ 18.99",
        "ref. 570502122243",
        "r U & US$",
        "Mixed spike necklace 10,99"
      ],
      "RequiresMultilineRegex": false,
      "SuggestedRegex": "(?<ItemDescription>[A-Za-z\\s]+)\\s*@\\s*(?<UnitPrice>\\d+\\.\\d{2})[\\s\\S]*?ref\\.\\s*(?<ItemCode>\\d+)[\\s\\S]*?(?<Size>[A-Z]+)",
      "CapturedFields": [
        "ItemDescription",
        "UnitPrice",
        "ItemCode",
        "Size"
      ],
      "FieldCorrections": [],
      "Pattern": null,
      "Replacement": null
    },
    {
      "Field": "InvoiceDetail_MultiField_Line9_11",
      "ExtractedValue": "",
      "CorrectValue": "{\n        \"Quantity\": \"\",\n        \"ItemDescription\": \"Mixed spike necklace\",\n        \"UnitPrice\": \"10.99\",\n        \"ItemCode\": \"57077738990R\",\n        \"Size\": \"U\"\n      }",
      "Confidence": 0.95,
      "ErrorType": "multi_field_omission",
      "Reasoning": "Complete object with European decimal format price, reference code and size",
      "LineNumber": 9,
      "LineText": "Mixed spike necklace 10,99\nref. 57077738990R\nr U & US$",
      "ContextLinesBefore": [
        "High-waist straight shorts “ 3.3",
        "",
        "ref. 570003742302 ts",
        ": L US$",
        "Long jumpsuit with back opening @ 18.99"
      ],
      "ContextLinesAfter": [
        "r U & US$",
        "Mixed spike necklace 10,99",
        "ref. 57077738990R",
        "Subtotal USS 196.33",
        "Shipping & Handling Free"
      ],
      "RequiresMultilineRegex": false,
      "SuggestedRegex": "(?<ItemDescription>[A-Za-z\\s]+)\\s*(?<UnitPrice>\\d+,\\d{2})[\\s\\S]*?ref\\.\\s*(?<ItemCode>\\d+\\w+)[\\s\\S]*?(?<Size>[A-Z]+)",
      "CapturedFields": [
        "ItemDescription",
        "UnitPrice",
        "ItemCode",
        "Size"
      ],
      "FieldCorrections": [],
      "Pattern": null,
      "Replacement": null
    },
    {
      "Field": "InvoiceDetail_MultiField_Line28_30",
      "ExtractedValue": "",
      "CorrectValue": "{\n        \"Quantity\": \"12\",\n        \"ItemDescription\": \"Cut-out linen-ble\\nnd dr\",\n        \"UnitPrice\": \"22.99\",\n        \"ItemCode\": \"570502512488\"\n      }",
      "Confidence": 0.9,
      "ErrorType": "multi_field_omission",
      "Reasoning": "Multi-line description with quantity and price, requires sequential line capture",
      "LineNumber": 28,
      "LineText": "Cut-out linen-ble\nnd dr\nref, 570502519\n4\nA dress\nUS$\n22.99",
      "ContextLinesBefore": [
        " ",
        "",
        "wv WY",
        "",
        "High-waist 4;"
      ],
      "ContextLinesAfter": [
        "ret S70s00ssqaqe",
        "",
        "Cut-out linen-ble",
        "nd dr",
        "ref, 570502512488 ess"
      ],
      "RequiresMultilineRegex": false,
      "SuggestedRegex": "(?<ItemDescription>[A-Za-z\\s\\-]+\\n[A-Za-z\\s]+)[\\s\\S]*?ref[,\\.]\\s*(?<ItemCode>\\d+)[\\s\\S]*?(?<Quantity>\\d+)[\\s\\S]*?US\\$\\s*(?<UnitPrice>\\d+\\.\\d{2})",
      "CapturedFields": [
        "ItemDescription",
        "ItemCode",
        "Quantity",
        "UnitPrice"
      ],
      "FieldCorrections": [],
      "Pattern": null,
      "Replacement": null
    },
    {
      "Field": "InvoiceDetail_MultiField_Line31_33",
      "ExtractedValue": "",
      "CorrectValue": "{\n        \"Quantity\": \"12\",\n        \"ItemDescription\": \"Buttoned Printed shirt\",\n        \"UnitPrice\": \"8.99\",\n        \"ItemCode\": \"570728812446\"\n      }",
      "Confidence": 0.9,
      "ErrorType": "multi_field_omission",
      "Reasoning": "Multi-line description with reference code, quantity and price",
      "LineNumber": 31,
      "LineText": "Buttoned\nPrinted shirt\nAft\nref. 57072\n8812446\n12\nUS$ 8.99",
      "ContextLinesBefore": [
        "",
        "High-waist 4;",
        "nen sh",
        "ret S70s00ssqaqe",
        ""
      ],
      "ContextLinesAfter": [
        "nd dr",
        "ref, 570502512488 ess",
        "",
        "Buttoned Printed shirt",
        "ref. 570728812446"
      ],
      "RequiresMultilineRegex": false,
      "SuggestedRegex": "(?<ItemDescription>[A-Za-z\\s]+\\n[A-Za-z\\s]+)[\\s\\S]*?ref\\.\\s*(?<ItemCode>\\d+)[\\s\\S]*?(?<Quantity>\\d+)[\\s\\S]*?US\\$\\s*(?<UnitPrice>\\d+\\.\\d{2})",
      "CapturedFields": [
        "ItemDescription",
        "ItemCode",
        "Quantity",
        "UnitPrice"
      ],
      "FieldCorrections": [],
      "Pattern": null,
      "Replacement": null
    },
    {
      "Field": "InvoiceDetail_MultiField_Line34_36",
      "ExtractedValue": "",
      "CorrectValue": "{\n        \"Quantity\": \"\",\n        \"ItemDescription\": \"Circle design ma\\nxi earrings\",\n        \"UnitPrice\": \"10.99\",\n        \"ItemCode\": \"3700253399pL\"\n      }",
      "Confidence": 0.9,
      "ErrorType": "multi_field_omission",
      "Reasoning": "Multi-line description split across multiple lines with reference code and price",
      "LineNumber": 34,
      "LineText": "Circle design ma\nref. 3700253399p\ne €arrings\nUS$\n10.99",
      "ContextLinesBefore": [
        "ret S70s00ssqaqe",
        "",
        "Cut-out linen-ble",
        "nd dr",
        "ref, 570502512488 ess"
      ],
      "ContextLinesAfter": [
        "Buttoned Printed shirt",
        "ref. 570728812446",
        "",
        "Circle design ma",
        ""
      ],
      "RequiresMultilineRegex": false,
      "SuggestedRegex": "(?<ItemDescription>[A-Za-z\\s]+\\n[\\s\\S]*?[A-Za-z\\s]+)[\\s\\S]*?ref\\.\\s*(?<ItemCode>\\d+\\w+)[\\s\\S]*?US\\$\\s*(?<UnitPrice>\\d+\\.\\d{2})",
      "CapturedFields": [
        "ItemDescription",
        "ItemCode",
        "UnitPrice"
      ],
      "FieldCorrections": [],
      "Pattern": null,
      "Replacement": null
    },
    {
      "Field": "InvoiceDetail_MultiField_Line37_39",
      "ExtractedValue": "",
      "CorrectValue": "{\n        \"Quantity\": \"\",\n        \"ItemDescription\": \"Beaded thread earrings\",\n        \"UnitPrice\": \"10.99\",\n        \"ItemCode\": \"57004050990R\"\n      }",
      "Confidence": 0.9,
      "ErrorType": "multi_field_omission",
      "Reasoning": "Multi-line description with reference code and price",
      "LineNumber": 37,
      "LineText": "Beaded thread\nearrings\nUS$\nref. 5700405099\nOR\n10.99",
      "ContextLinesBefore": [
        "nd dr",
        "ref, 570502512488 ess",
        "",
        "Buttoned Printed shirt",
        "ref. 570728812446"
      ],
      "ContextLinesAfter": [
        "Circle design ma",
        "",
        "xi earrings",
        "ref, 3700253399pL 5",
        ""
      ],
      "RequiresMultilineRegex": false,
      "SuggestedRegex": "(?<ItemDescription>[A-Za-z\\s]+\\n[A-Za-z\\s]+)[\\s\\S]*?ref\\.\\s*(?<ItemCode>\\d+\\w+)[\\s\\S]*?US\\$\\s*(?<UnitPrice>\\d+\\.\\d{2})",
      "CapturedFields": [
        "ItemDescription",
        "ItemCode",
        "UnitPrice"
      ],
      "FieldCorrections": [],
      "Pattern": null,
      "Replacement": null
    },
    {
      "Field": "InvoiceDetail_MultiField_Line40_42",
      "ExtractedValue": "",
      "CorrectValue": "{\n        \"Quantity\": \"XL\",\n        \"ItemDescription\": \"Ribbed knit top\",\n        \"UnitPrice\": \"12.99\",\n        \"ItemCode\": \"570102602352\"\n      }",
      "Confidence": 0.95,
      "ErrorType": "multi_field_omission",
      "Reasoning": "Complete object with description, reference code, size and price",
      "LineNumber": 40,
      "LineText": "Ribbed knit top\nref. 570102602352\nXL\nUS$\n12.99",
      "ContextLinesBefore": [
        "Buttoned Printed shirt",
        "ref. 570728812446",
        "",
        "Circle design ma",
        ""
      ],
      "ContextLinesAfter": [
        "ref, 3700253399pL 5",
        "",
        "Beaded thread earrings",
        "ref. 57004050990R",
        ""
      ],
      "RequiresMultilineRegex": false,
      "SuggestedRegex": "(?<ItemDescription>[A-Za-z\\s]+)[\\s\\S]*?ref\\.\\s*(?<ItemCode>\\d+)[\\s\\S]*?(?<Size>[A-Z]+)[\\s\\S]*?US\\$\\s*(?<UnitPrice>\\d+\\.\\d{2})",
      "CapturedFields": [
        "ItemDescription",
        "ItemCode",
        "Size",
        "UnitPrice"
      ],
      "FieldCorrections": [],
      "Pattern": null,
      "Replacement": null
    },
    {
      "Field": "InvoiceDetail_MultiField_Line43_45",
      "ExtractedValue": "",
      "CorrectValue": "{\n        \"Quantity\": \"12\",\n        \"ItemDescription\": \"Openwork knit cotton top\",\n        \"UnitPrice\": \"4.49\",\n        \"ItemCode\": \"170820032205\"\n      }",
      "Confidence": 0.95,
      "ErrorType": "multi_field_omission",
      "Reasoning": "Complete object with description, reference code, quantity and price",
      "LineNumber": 43,
      "LineText": "Openwork knit cotton top\nref. 170820032205\n12\nUS$ 4.49",
      "ContextLinesBefore": [
        "Circle design ma",
        "",
        "xi earrings",
        "ref, 3700253399pL 5",
        ""
      ],
      "ContextLinesAfter": [
        "ref. 57004050990R",
        "",
        "Ribbed knit top",
        "ref. 570102602352",
        ""
      ],
      "RequiresMultilineRegex": false,
      "SuggestedRegex": "(?<ItemDescription>[A-Za-z\\s]+)[\\s\\S]*?ref\\.\\s*(?<ItemCode>\\d+)[\\s\\S]*?(?<Quantity>\\d+)[\\s\\S]*?US\\$\\s*(?<UnitPrice>\\d+\\.\\d{2})",
      "CapturedFields": [
        "ItemDescription",
        "ItemCode",
        "Quantity",
        "UnitPrice"
      ],
      "FieldCorrections": [],
      "Pattern": null,
      "Replacement": null
    },
    {
      "Field": "InvoiceDetail_MultiField_Line46_48",
      "ExtractedValue": "",
      "CorrectValue": "{\n        \"Quantity\": \"\",\n        \"ItemDescription\": \"Bead loop earrings\",\n        \"UnitPrice\": \"8.99\",\n        \"ItemCode\": \"57002545990R\"\n      }",
      "Confidence": 0.95,
      "ErrorType": "multi_field_omission",
      "Reasoning": "Complete object with description, reference code and price",
      "LineNumber": 46,
      "LineText": "Bead loop earrings\n~~\nref. 57002545990R\nUS$ 8.99",
      "ContextLinesBefore": [
        "ref, 3700253399pL 5",
        "",
        "Beaded thread earrings",
        "ref. 57004050990R",
        ""
      ],
      "ContextLinesAfter": [
        "ref. 570102602352",
        "",
        "Openwork knit cotton top",
        "ref. 170820032205",
        ""
      ],
      "RequiresMultilineRegex": false,
      "SuggestedRegex": "(?<ItemDescription>[A-Za-z\\s]+)[\\s\\S]*?ref\\.\\s*(?<ItemCode>\\d+\\w+)[\\s\\S]*?US\\$\\s*(?<UnitPrice>\\d+\\.\\d{2})",
      "CapturedFields": [
        "ItemDescription",
        "ItemCode",
        "UnitPrice"
      ],
      "FieldCorrections": [],
      "Pattern": null,
      "Replacement": null
    },
    {
      "Field": "InvoiceDetail_MultiField_Line49_51",
      "ExtractedValue": "",
      "CorrectValue": "{\n        \"Quantity\": \"\",\n        \"ItemDescription\": \"Circle design maxi earrings\",\n        \"UnitPrice\": \"10.99\",\n        \"ItemCode\": \"S700253399PL\"\n      }",
      "Confidence": 0.95,
      "ErrorType": "multi_field_omission",
      "Reasoning": "Complete object with description, reference code and price",
      "LineNumber": 49,
      "LineText": "Circle design maxi earrings\nref. S700253399PL\nUS$ 10.99",
      "ContextLinesBefore": [
        "ref. 57004050990R",
        "",
        "Ribbed knit top",
        "ref. 570102602352",
        ""
      ],
      "ContextLinesAfter": [
        "ref. 170820032205",
        "",
        "Bead loop earrings",
        "ref. 57002545990R",
        ""
      ],
      "RequiresMultilineRegex": false,
      "SuggestedRegex": "(?<ItemDescription>[A-Za-z\\s]+)[\\s\\S]*?ref\\.\\s*(?<ItemCode>\\w+\\d+\\w+)[\\s\\S]*?US\\$\\s*(?<UnitPrice>\\d+\\.\\d{2})",
      "CapturedFields": [
        "ItemDescription",
        "ItemCode",
        "UnitPrice"
      ],
      "FieldCorrections": [],
      "Pattern": null,
      "Replacement": null
    },
    {
      "Field": "InvoiceDetail_MultiField_Line52_54",
      "ExtractedValue": "",
      "CorrectValue": "{\n        \"Quantity\": \"\",\n        \"ItemDescription\": \"3 Pack of beaded glasses chains\",\n        \"UnitPrice\": \"6.99\",\n        \"ItemCode\": \"470158569952\"\n      }",
      "Confidence": 0.95,
      "ErrorType": "multi_field_omission",
      "Reasoning": "Complete object with description, reference code and price",
      "LineNumber": 52,
      "LineText": "3 Pack of beaded glasses chains\nref. 470158569952\nUS$ 6.99",
      "ContextLinesBefore": [
        "ref. 570102602352",
        "",
        "Openwork knit cotton top",
        "ref. 170820032205",
        ""
      ],
      "ContextLinesAfter": [
        "ref. 57002545990R",
        "",
        "Circle design maxi earrings",
        "ref. S700253399PL",
        ""
      ],
      "RequiresMultilineRegex": false,
      "SuggestedRegex": "(?<ItemDescription>[A-Za-z\\s]+)[\\s\\S]*?ref\\.\\s*(?<ItemCode>\\d+)[\\s\\S]*?US\\$\\s*(?<UnitPrice>\\d+\\.\\d{2})",
      "CapturedFields": [
        "ItemDescription",
        "ItemCode",
        "UnitPrice"
      ],
      "FieldCorrections": [],
      "Pattern": null,
      "Replacement": null
    },
    {
      "Field": "InvoiceDetail_MultiField_Line85_87",
      "ExtractedValue": "",
      "CorrectValue": "{\n        \"Quantity\": \"XL\",\n        \"ItemDescription\": \"Floral-print jumpsuit with tie\",\n        \"UnitPrice\": \"14.99\",\n        \"ItemCode\": \"570528822346\"\n      }",
      "Confidence": 0.95,
      "ErrorType": "multi_field_omission",
      "Reasoning": "Complete object with size, description, price and reference code",
      "LineNumber": 85,
      "LineText": "XL & US$\nFloral-print jumpsuit with tie 14.99\n\nref. 570528822346",
      "ContextLinesBefore": [
        "US$",
        "10.99",
        "",
        "US$",
        "12.99"
      ],
      "ContextLinesAfter": [
        "US$ 4.49",
        "",
        "US$ 8.99",
        "",
        "US$"
      ],
      "RequiresMultilineRegex": false,
      "SuggestedRegex": "(?<Size>[A-Z]+)[\\s\\S]*?(?<ItemDescription>[A-Za-z\\s\\-]+)\\s*(?<UnitPrice>\\d+\\.\\d{2})[\\s\\S]*?ref\\.\\s*(?<ItemCode>\\d+)",
      "CapturedFields": [
        "Size",
        "ItemDescription",
        "UnitPrice",
        "ItemCode"
      ],
      "FieldCorrections": [],
      "Pattern": null,
      "Replacement": null
    },
    {
      "Field": "InvoiceDetail_MultiField_Line88_90",
      "ExtractedValue": "",
      "CorrectValue": "{\n        \"Quantity\": \"\",\n        \"ItemDescription\": \"Crystal pendant earrings\",\n        \"UnitPrice\": \"10.99\",\n        \"ItemCode\": \"57047743990R\"\n      }",
      "Confidence": 0.95,
      "ErrorType": "multi_field_omission",
      "Reasoning": "Complete object with size, description, price and reference code",
      "LineNumber": 88,
      "LineText": "U & US$\nCrystal pendant earrings 10.99\n\nj j ref. 57047743990R",
      "ContextLinesBefore": [
        "US$",
        "12.99",
        "",
        "US$ 4.49",
        ""
      ],
      "ContextLinesAfter": [
        "",
        "US$",
        "",
        "10.99",
        ""
      ],
      "RequiresMultilineRegex": false,
      "SuggestedRegex": "(?<Size>[A-Z]+)[\\s\\S]*?(?<ItemDescription>[A-Za-z\\s]+)\\s*(?<UnitPrice>\\d+\\.\\d{2})[\\s\\S]*?ref\\.\\s*(?<ItemCode>\\d+\\w+)",
      "CapturedFields": [
        "Size",
        "ItemDescription",
        "UnitPrice",
        "ItemCode"
      ],
      "FieldCorrections": [],
      "Pattern": null,
      "Replacement": null
    },
    {
      "Field": "InvoiceDetail_MultiField_Line91_93",
      "ExtractedValue": "",
      "CorrectValue": "{\n        \"Quantity\": \"12\",\n        \"ItemDescription\": \"Linen dress with bead detail\",\n        \"UnitPrice\": \"8.99\",\n        \"ItemCode\": \"570002562488\"\n      }",
      "Confidence": 0.95,
      "ErrorType": "multi_field_omission",
      "Reasoning": "Complete object with quantity, price, description and reference code",
      "LineNumber": 91,
      "LineText": "12 @ USS 8.99\nLinen dress with bead detail\nref. 570002562488",
      "ContextLinesBefore": [
        "US$ 4.49",
        "",
        "US$ 8.99",
        "",
        "US$"
      ],
      "ContextLinesAfter": [
        "10.99",
        "",
        "US$ 6.99",
        "",
        "You will receive your order UCSJB6 shortly"
      ],
      "RequiresMultilineRegex": false,
      "SuggestedRegex": "(?<Quantity>\\d+)\\s*@\\s*US\\$?\\s*(?<UnitPrice>\\d+\\.\\d{2})[\\s\\S]*?(?<ItemDescription>[A-Za-z\\s]+)[\\s\\S]*?ref\\.\\s*(?<ItemCode>\\d+)",
      "CapturedFields": [
        "Quantity",
        "UnitPrice",
        "ItemDescription",
        "ItemCode"
      ],
      "FieldCorrections": [],
      "Pattern": null,
      "Replacement": null
    },
    {
      "Field": "InvoiceDetail_MultiField_Line94_96",
      "ExtractedValue": "",
      "CorrectValue": "{\n        \"Quantity\": \"U\",\n        \"ItemDescription\": \"Rhinestone thread hoop earrings\",\n        \"UnitPrice\": \"4.99\",\n        \"ItemCode\": \"5701252999PL\"\n      }",
      "Confidence": 0.95,
      "ErrorType": "multi_field_omission",
      "Reasoning": "Complete object with size, price, description and reference code",
      "LineNumber": 94,
      "LineText": "U US$ 4.99\nRhinestone thread hoop earrings\nref. 5701252999PL",
      "ContextLinesBefore": [
        "",
        "US$",
        "",
        "10.99",
        ""
      ],
      "ContextLinesAfter": [
        "",
        "You will receive your order UCSJB6 shortly",
        "",
        "From: MANGO OUTLET (noreply@mango.com)",
        "To: RLAGZ11@YAHOO.COM"
      ],
      "RequiresMultilineRegex": false,
      "SuggestedRegex": "(?<Size>[A-Z]+)\\s*US\\$\\s*(?<UnitPrice>\\d+\\.\\d{2})[\\s\\S]*?(?<ItemDescription>[A-Za-z\\s]+)[\\s\\S]*?ref\\.\\s*(?<ItemCode>\\d+\\w+)",
      "CapturedFields": [
        "Size",
        "UnitPrice",
        "ItemDescription",
        "ItemCode"
      ],
      "FieldCorrections": [],
      "Pattern": null,
      "Replacement": null
    }
  ],
  "detectionCount": 23,
  "errorTypes": {
    "omission": 7,
    "multi_field_omission": 16
  },
  "confidenceDistribution": {
    "medium": 7,
    "high": 16
  }
}
```

## ❌ **Specific Issues Identified**

### **Issue 1: DeepSeek AI Balance Error**
- **Problem**: DeepSeek detection produces balance error of 13.7500
- **Evidence**: Balance check: 13.7500 (should be 0.00)
- **Expected Behavior**: DeepSeek should produce perfect balance (0.00)
- **Actual Behavior**: DeepSeek has 13.7500 balance error
- **Proposed Fix**:
```
Enhance DeepSeek prompt with:
1. Better section precedence logic
2. Improved financial pattern detection
3. Balance validation in prompt
```

## 🎯 **Next Steps for LLM**
### **Immediate Actions Needed**:
- Implement prompt improvements
- Test affected files
- Verify balance accuracy

### **Success Criteria**:
- Balance error ≤ 0.01
- All fields detected correctly
- No regressions in other files

### **🎯 v1.1 Validation Results**:

## 📝 **Complete Prompt Versioning & Testing Protocol**

### **v1.1 Prompt Specifications**:

#### **JSON Extraction Prompt (Claude Code/DeepSeek Fallback) - JSON v1.1.0 + Detection v1.1.1**:
```
You are an expert invoice data extraction system that follows Caribbean Customs rules and production OCR section precedence logic. Extract all relevant invoice information from the provided OCR text and output a structured JSON object that matches the production ShipmentInvoice model.

**CARIBBEAN CUSTOMS FIELD MAPPING RULES:**

1. **TotalInsurance** = Customer-caused reductions (stored as NEGATIVE values):
   - Gift Cards → TotalInsurance = -amount  
   - Store Credits → TotalInsurance = -amount
   - Credits → TotalInsurance = -amount
   - Customer payments/refunds → TotalInsurance = -amount

2. **TotalDeduction** = Supplier-caused reductions (stored as POSITIVE values):
   - Free Shipping → TotalDeduction = +amount
   - Discounts → TotalDeduction = +amount  
   - Promotional reductions → TotalDeduction = +amount

**OUTPUT FORMAT (STRICT JSON):**
{
  "invoiceHeader": {
    "InvoiceNo": "string or null",
    "InvoiceDate": "YYYY-MM-DD or null",
    "SupplierName": "string or null",
    "Currency": "USD/CNY/XCD or null"
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
    "calculatedTotal": 0.00,
    "balanceCheck": 0.00,
    "validationPassed": false
  },
  "confidence": "high/medium/low"
}

Return ONLY the JSON object, no additional text.
```

#### **Error Detection Logic (v1.1.1) - Enhanced Credit Pattern Recognition**:
```csharp
// v1.1 IMPROVED: Distinguish actual credits from payment methods
private bool HasActualCredit(string ocrText)
{
    var ocrLower = ocrText.ToLower();
    
    // Positive patterns - actual customer credits
    var actualCreditPatterns = new[]
    {
        "store credit", "account credit", "credit applied",
        "gift card", "refund amount", "credit balance"
    };
    
    var hasActualCredit = actualCreditPatterns.Any(pattern => ocrLower.Contains(pattern));
    
    // Negative patterns - payment methods (exclude these)
    var paymentMethodPatterns = new[]
    {
        "credit card transactions", "visa ending in", "payment method",
        "mastercard ending in", "amex ending in"
    };
    
    var isPaymentMethod = paymentMethodPatterns.Any(pattern => ocrLower.Contains(pattern));
    
    return hasActualCredit && !isPaymentMethod;
}
```

### **Testing Protocol Enhancement**:

#### **Mandatory Steps for All Future Diagnostic Versions**:
1. **Version Documentation**: Record exact prompt version used (v1.1.0, v1.1.1, etc.)
2. **Prompt Preservation**: Include complete prompt text in diagnostic file for reproducibility
3. **Historical Context**: Document what worked perfectly in previous versions
4. **Status Tracking**: Mark files as "PERFECT STATUS ACHIEVED" when no issues found
5. **Regression Prevention**: Explicitly state what should NOT change in future iterations
6. **LLM Guidance**: Provide explicit instructions for future LLM analysis

---
**Generated by**: Detailed Diagnostic Generator v1.1
**Prompt Version**: JSON v1.1.0 + Detection v1.1.1
**For LLM Context**: This file contains complete prompt versioning, historical resolution, and success state documentation.
**Protocol**: All future diagnostics must include prompt versions and historical success tracking.
**Foundation for v1.2**: Use this analysis to validate that improvements don't break working functionality.
