# 🤖 **AUTOMATED SYSTEMATIC FIX PROMPT - MANGO TEST SUCCESS MANDATE**

**DIRECTIVE**: Work through ALL issues systematically until MANGO test passes completely. Do NOT stop for user confirmation. Keep working until `CanImportMango03152025TotalAmount_AfterLearning` test shows `✅ STEP 4 PASSED` and `ShipmentInvoice 'UCSJB6' or 'UCSJIB6' created successfully`.

## **📋 SYSTEMATIC FIX SEQUENCE - EXECUTE AUTOMATICALLY**

### **PHASE 1: IMMEDIATE TEMPLATE SPECIFICATION COMPLIANCE FIX**
1. **Fix Field Mapping Validation Failures**:
   - Investigate why `❌ **TEMPLATE_SPEC_FIELD_MAPPING_DUAL_LAYER**: ❌ AI Quality: True + ❌ Data Compliance: False` consistently fails
   - Check DatabaseTemplateHelper field mapping validation logic
   - Fix underlying data compliance issues causing validation failures
   - Ensure all template specification criteria pass before proceeding

### **PHASE 2: PATTERN CREATION OPTIMIZATION** 
2. **Resolve DeepSeek Pattern Creation Timeout**:
   - Analyze why pattern creation takes >2 minutes and times out
   - Optimize DeepSeek API calls and reduce prompt complexity if needed
   - Ensure pattern creation completes within reasonable timeframe
   - If still times out, increase test timeout or optimize further

### **PHASE 3: PATTERN-TEXT MATCHING INVESTIGATION**
3. **Execute Text Source Analysis**:
   - Run MANGO test to completion to trigger text source comparison logging
   - Capture and analyze the actual content differences between:
     - `TestMatch` (what DeepSeek provides)
     - `WindowText` (enhanced context)  
     - `LineText` (individual line fragments)
   - Document exact format mismatches causing pattern failures

### **PHASE 4: PATTERN CORRECTION IMPLEMENTATION**
4. **Fix Pattern Matching Issues**:
   - Based on text source analysis, identify why patterns don't match during `template.Read()`
   - Implement fixes to ensure DeepSeek patterns work with actual OCR text format
   - Options to consider:
     - Modify patterns to match Line.Read() text format
     - Enhance Line.Read() to provide context labels DeepSeek expects
     - Improve WindowText population for better validation context

### **PHASE 5: DATA EXTRACTION VERIFICATION**
5. **Resolve Empty CsvLines Issue**:
   - Ensure `VALID_DICTIONARIES_COUNT > 0` after pattern fixes
   - Verify template.Read() successfully extracts invoice data
   - Confirm CsvLines contains proper dictionary objects for processing

### **PHASE 6: END-TO-END VALIDATION**
6. **Complete Pipeline Testing**:
   - Run full MANGO test to verify STEP 4 passes
   - Confirm ShipmentInvoice creation succeeds
   - Validate database persistence works correctly
   - Ensure no regressions in other functionality

## **🔄 EXECUTION PROTOCOL**

**CONTINUOUS WORK MANDATE**: 
- Execute each phase systematically without stopping
- If a phase fails, investigate and fix immediately before proceeding
- Use comprehensive logging to capture all diagnostic information
- Build and test after each significant change
- Only move to next phase when current phase succeeds completely

**SUCCESS CRITERIA**: 
- ✅ All template specification validations pass
- ✅ DeepSeek pattern creation completes successfully  
- ✅ Patterns match actual OCR text during template.Read()
- ✅ CsvLines contains valid dictionary objects (VALID_DICTIONARIES_COUNT > 0)
- ✅ MANGO test passes with ShipmentInvoice 'UCSJB6' or 'UCSJIB6' created
- ✅ No compilation errors or regressions

**INVESTIGATION APPROACH**:
- **Ultrathink** each issue by examining root causes systematically
- Use comprehensive logging and diagnostic analysis
- Reference existing successful solutions and patterns
- Apply the validation fix patterns already implemented
- Maintain code integrity while resolving all issues

## **🚀 BEGIN AUTOMATED EXECUTION**

**START WITH PHASE 1**: Immediately investigate and fix the persistent template specification field mapping validation failures that appear in every test run. Do not proceed until this foundational issue is resolved.

**CONTINUE AUTOMATICALLY**: Work through each phase until the MANGO test passes completely. Use the TodoWrite tool to track progress through each phase.

---

**This prompt is designed to keep you working systematically until success is achieved. Execute now without waiting for further confirmation.**