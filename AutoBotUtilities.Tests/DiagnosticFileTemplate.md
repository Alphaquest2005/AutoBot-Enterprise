# Invoice Diagnostic Report Template

## 📋 **File Identification**
- **Invoice File**: {FileName}.txt
- **Test Version**: {Version} (e.g., v1.0, v1.1)
- **Test Date**: {DateTime}
- **Vendor Category**: {VendorType} (Amazon, TEMU, COJAY, etc.)
- **Issue Category**: {IssueType} (Multi-Order, Credit Detection, Balance Error, etc.)

## 🎯 **Issue Summary**
- **Primary Issue**: {MainIssue}
- **Severity**: {HIGH/MEDIUM/LOW}
- **Impact**: {Description of business impact}
- **Root Cause**: {Technical explanation of why this fails}

## 🏗️ **Design Challenge Context**
### **System Architecture**:
- **Caribbean Customs Rules**: TotalInsurance (negative customer reductions), TotalDeduction (positive supplier reductions)
- **OCR Section Precedence**: Single Column > Ripped Text > SparseText  
- **Balance Formula**: SubTotal + TotalInternalFreight + TotalOtherCost + TotalInsurance - TotalDeduction = InvoiceTotal
- **DeepSeek Model**: deepseek-chat, temperature 0.3, max tokens 8192

### **Known System Limitations**:
- DeepSeek lacks SetPartLineValues section precedence logic
- Multi-order documents cause field mixing
- Credit patterns inconsistently mapped to TotalInsurance
- Missing FREE shipping → TotalInternalFreight=0.00 detection

## 📄 **Invoice Details**
### **Document Structure**:
```
{OCR_STRUCTURE_ANALYSIS}
- Number of orders detected: {OrderCount}
- OCR sections present: {SectionList}
- Financial patterns found: {PatternList}
- Special characteristics: {SpecialNotes}
```

### **Key Financial Data**:
```
Expected Financial Values (Ground Truth):
- InvoiceTotal: ${ExpectedTotal}
- SubTotal: ${ExpectedSubTotal}  
- TotalInternalFreight: ${ExpectedFreight}
- TotalOtherCost: ${ExpectedOtherCost}
- TotalInsurance: ${ExpectedInsurance}
- TotalDeduction: ${ExpectedDeduction}

Balance Check: {ExpectedBalance} (should be 0.00)
```

## 🔧 **Current Approach/Plan**
### **Detection Strategy**:
{CurrentStrategy}

### **Prompt Improvements Attempted**:
{PromptChanges}

### **Version History**:
- **v1.0**: {BaselineDescription}
- **v1.1**: {FirstImprovementDescription}
- **v1.2**: {SecondImprovementDescription}

## 📊 **Test Results**

### **JSON Reference System Results**:
```json
{
  "extractedFields": {
    "InvoiceTotal": {ActualJsonTotal},
    "SubTotal": {ActualJsonSubTotal},
    "TotalOtherCost": {ActualJsonOtherCost},
    "TotalInsurance": {ActualJsonInsurance},
    "TotalDeduction": {ActualJsonDeduction}
  },
  "validation": {
    "calculatedTotal": {JsonCalculatedTotal},
    "balanceCheck": {JsonBalanceError},
    "validationPassed": {JsonValidationPassed}
  },
  "confidence": "{JsonConfidenceLevel}"
}
```

### **DeepSeek AI Results**:
```json
{
  "detectedErrors": [
    {ActualDeepSeekErrorList}
  ],
  "financialCalculation": {
    "calculatedTotal": {DeepSeekCalculatedTotal},
    "balanceCheck": {DeepSeekBalanceError},
    "validationPassed": {DeepSeekValidationPassed}
  },
  "detectionCount": {DeepSeekErrorCount}
}
```

### **Comparison Analysis**:
```
JSON vs DeepSeek Performance:
- JSON Balance Error: {JsonError:F4}
- DeepSeek Balance Error: {DeepSeekError:F4}
- Winner: {WinnerSystem}
- F1 Score: {F1Score:F3}
- Precision: {Precision:F3}
- Recall: {Recall:F3}
```

## ❌ **Specific Issues Identified**

### **Issue 1: {IssueTitle}**
- **Problem**: {IssueDescription}
- **Evidence**: {LineNumbers, Patterns, Examples}
- **Expected Behavior**: {WhatShouldHappen}
- **Actual Behavior**: {WhatActuallyHappens}
- **Proposed Fix**: {SpecificSolution}

### **Issue 2: {SecondIssueTitle}**
- **Problem**: {SecondIssueDescription}
- **Evidence**: {SecondIssueEvidence}
- **Proposed Fix**: {SecondIssueeSolution}

## 📝 **OCR Text Analysis**
### **Critical Sections**:
```
------------------------------------------Single Column-------------------------
{CriticalSingleColumnText}

------------------------------------------SparseText-------------------------  
{CriticalSparseTextText}

------------------------------------------Ripped Text-------------------------
{CriticalRippedTextText}
```

### **Pattern Detection**:
- **Multi-Order Patterns**: {MultiOrderEvidence}
- **Financial Patterns**: {FinancialPatternList}
- **Credit/Deduction Patterns**: {CreditPatternList}
- **Missing Patterns**: {MissingPatternList}

## 🎯 **Next Steps for LLM**
### **Immediate Actions Needed**:
1. {ActionItem1}
2. {ActionItem2}
3. {ActionItem3}

### **Prompt Improvements Required**:
#### **DeepSeek Prompt Changes**:
```
{SpecificDeepSeekPromptImprovements}
```

#### **JSON Prompt Changes**:
```
{SpecificJsonPromptImprovements}
```

### **Testing Validation**:
- **Success Criteria**: {SuccessCriteria}
- **Test Method**: {TestMethod}
- **Expected Outcome**: {ExpectedOutcome}

## 🔄 **Version Comparison**
### **Changes from Previous Version**:
{VersionDiff}

### **Performance Improvement**:
{PerformanceChange}

### **Regression Check**:
{RegressionStatus}

## 📚 **Reference Information**
### **Related Issues**:
- Similar files with same issue: {RelatedFiles}
- Related issue categories: {RelatedIssues}

### **System Context**:
- **Caribbean Customs Documentation**: {LinkToCaricomRules}
- **DeepSeek API Documentation**: {LinkToDeepSeekDocs}
- **OCR Section Processing**: {LinkToSetPartLineValues}

---
**Generated by**: Systematic Testing Framework v{SystemVersion}  
**For LLM Context**: This file contains complete context for diagnosing and fixing this specific invoice processing issue.  
**Next Iteration**: Copy this file to v{NextVersion} folder and update with improvements.