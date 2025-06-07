# SetPartLineValues Version Testing Framework - How It Works

## 🎯 OVERVIEW

The version testing framework allows you to run any of the 5 historical versions of `SetPartLineValues` using the exact same input data. This enables direct comparison of how each version would process the same invoice.

---

## 🔧 HOW THE FRAMEWORK WORKS

### 1. Main Entry Point - Router Method
```csharp
private List<IDictionary<string, object>> SetPartLineValues(Part part, string filterInstance = null)
{
    // **VERSION TESTING FRAMEWORK**: Route to different versions for comparison
    var versionToTest = GetVersionToTest();
    
    _logger.Information("**VERSION_ROUTER**: Using version {Version} for testing", versionToTest);
    
    return versionToTest switch
    {
        "V1" => SetPartLineValues_V1_Working(part, filterInstance),
        "V2" => SetPartLineValues_V2_BudgetMarine(part, filterInstance), 
        "V3" => SetPartLineValues_V3_SheinNotAmazon(part, filterInstance),
        "V4" => SetPartLineValues_V4_WorkingAllTests(part, filterInstance),
        "V5" => SetPartLineValues_V5_Current(part, filterInstance),
        _ => SetPartLineValues_V5_Current(part, filterInstance) // Default to current
    };
}
```

**What this does:**
- **Intercepts** all calls to `SetPartLineValues()`
- **Routes** to the specific version based on configuration
- **Uses identical input data** for all versions
- **Logs** which version is being used

### 2. Version Selection Logic
```csharp
private string GetVersionToTest()
{
    // **VERSION CONTROL**: Check environment variable first
    var versionFromEnv = Environment.GetEnvironmentVariable("SETPARTLINEVALUES_VERSION");
    if (!string.IsNullOrEmpty(versionFromEnv))
    {
        _logger.Information("**VERSION_CONTROL**: Using version {Version} from environment variable", versionFromEnv);
        return versionFromEnv;
    }
    
    // Default to V5 (current implementation)
    return "V5";
}
```

**How to control which version runs:**

#### Option 1: Environment Variable (Recommended)
```bash
# Windows Command Prompt
set SETPARTLINEVALUES_VERSION=V1

# Windows PowerShell  
$env:SETPARTLINEVALUES_VERSION = "V1"

# Linux/WSL
export SETPARTLINEVALUES_VERSION=V1
```

#### Option 2: Modify Code Directly
```csharp
private string GetVersionToTest()
{
    // Force a specific version for testing
    return "V1"; // Change this to V1, V2, V3, V4, or V5
}
```

### 3. Version Implementations

Each version is a complete, standalone implementation:

```csharp
// Version 1 - Original working version
private List<IDictionary<string, object>> SetPartLineValues_V1_Working(Part part, string filterInstance = null)
{
    // Complete V1 implementation with extensive logging
}

// Version 2 - Budget Marine fixes  
private List<IDictionary<string, object>> SetPartLineValues_V2_BudgetMarine(Part part, string filterInstance = null)
{
    // Complete V2 implementation
}

// Version 3 - Shein ordering improvements
private List<IDictionary<string, object>> SetPartLineValues_V3_SheinNotAmazon(Part part, string filterInstance = null)
{
    // Complete V3 implementation
}

// Version 4 - Consolidation logic (breaks Tropical Vendors)
private List<IDictionary<string, object>> SetPartLineValues_V4_WorkingAllTests(Part part, string filterInstance = null)
{
    // Complete V4 implementation with consolidation
}

// Version 5 - Current with logging fixes
private List<IDictionary<string, object>> SetPartLineValues_V5_Current(Part part, string filterInstance = null)
{
    // Current implementation (same as V4 logic)
}
```

---

## 🧪 HOW TO USE THE FRAMEWORK

### Step 1: Choose Your Test Scenario

#### Test Amazon Invoice (Currently Failing in V1-V3)
```csharp
[Test]
public async Task TestAmazonWithAllVersions()
{
    var versions = new[] { "V1", "V2", "V3", "V4", "V5" };
    var testFile = @"C:\Insight Software\AutoBot-Enterprise\AutoBotUtilities.Tests\Test Data\Amazon.com - Order 112-9126443-1163432.pdf";
    
    foreach (var version in versions)
    {
        // Set version to test
        Environment.SetEnvironmentVariable("SETPARTLINEVALUES_VERSION", version);
        
        // Run the import
        var result = await RunInvoiceImport(testFile);
        
        // Log results
        _logger.Information("**VERSION_{Version}_RESULTS**: TotalsZero={TotalsZero}, ItemCount={ItemCount}", 
            version, result.TotalsZero, result.ItemCount);
    }
}
```

#### Test Tropical Vendors (Would work in V1-V3, broken in V4-V5)
```csharp
[Test] 
public async Task TestTropicalVendorsWithAllVersions()
{
    var versions = new[] { "V1", "V2", "V3", "V4", "V5" };
    var testFile = @"C:\path\to\tropical-vendors-invoice.pdf";
    
    foreach (var version in versions)
    {
        Environment.SetEnvironmentVariable("SETPARTLINEVALUES_VERSION", version);
        var result = await RunInvoiceImport(testFile);
        
        _logger.Information("**VERSION_{Version}_RESULTS**: InvoiceDetails={DetailCount}", 
            version, result.InvoiceDetails.Count);
    }
}
```

### Step 2: Run Tests and Analyze Logs

#### Expected Log Output:
```
**VERSION_ROUTER**: Using version V1 for testing
**VERSION_1_TEST**: Entering SetPartLineValues_V1_Working for PartId: 123
**VERSION_1_TEST**: Processing parent instance with 66 child instances
**VERSION_1_TEST**: Child processing - found product description field
**VERSION_1_TEST**: Final result - returning 1 parent with 66 child details attached

**VERSION_ROUTER**: Using version V4 for testing  
**VERSION_4_TEST**: Entering SetPartLineValues_V4_WorkingAllTests for PartId: 123
**VERSION_4_TEST**: Found 66 lines with product fields
**VERSION_4_TEST**: CRITICAL DECISION: hasProductFields && hasMultipleLines = TRUE
**VERSION_4_TEST**: ProcessAsSingleConsolidatedItem - consolidating 66 lines
**VERSION_4_TEST**: Final result - returning 2 consolidated items
```

---

## 🔍 DETAILED EXECUTION FLOW

### 1. Normal Invoice Processing Path
```
PDFUtils.ImportPDF()
  ↓
InvoiceReader.ReadFormattedTextStep()
  ↓
Template.Read() 
  ↓
SetPartLineValues() ← **INTERCEPT POINT**
  ↓
[Version Router]
  ↓
SetPartLineValues_V{X}() ← **ACTUAL PROCESSING**
  ↓
Return results to Template.Read()
  ↓
Continue normal processing...
```

### 2. Version-Specific Processing
```csharp
// V1-V3 Processing (Working for Tropical Vendors)
SetPartLineValues_V1()
  ↓
ProcessInstance_V1()
  ↓  
PopulateParentFields_V1() → ProcessChildParts_V1()
  ↓
Result: 1 parent + 66 children in InvoiceDetails array

// V4-V5 Processing (Broken for Tropical Vendors)
SetPartLineValues_V4()
  ↓
ProcessInstanceWithItemConsolidation_V4()
  ↓
GroupIntoLogicalInvoiceItems_V4() → ProcessAsSingleConsolidatedItem_V4()
  ↓
Result: 1-2 consolidated summary items
```

---

## 📊 COMPARISON TESTING STRATEGIES

### Strategy 1: Sequential Version Testing
```csharp
public async Task<Dictionary<string, TestResult>> TestAllVersionsSequentially(string testFile)
{
    var results = new Dictionary<string, TestResult>();
    var versions = new[] { "V1", "V2", "V3", "V4", "V5" };
    
    foreach (var version in versions)
    {
        // Clear database
        Infrastructure.Utils.ClearDataBase();
        
        // Set version
        Environment.SetEnvironmentVariable("SETPARTLINEVALUES_VERSION", version);
        
        // Run test
        var result = await RunSingleTest(testFile);
        results[version] = result;
        
        // Log comparison
        _logger.Information("**VERSION_{Version}_COMPLETE**: {ResultSummary}", version, result.Summary);
    }
    
    return results;
}
```

### Strategy 2: A/B Direct Comparison
```csharp
public async Task CompareV1VsV4(string testFile)
{
    // Test V1 (working for Tropical Vendors)
    Infrastructure.Utils.ClearDataBase();
    Environment.SetEnvironmentVariable("SETPARTLINEVALUES_VERSION", "V1");
    var v1Result = await RunInvoiceImport(testFile);
    
    // Test V4 (broken for Tropical Vendors)  
    Infrastructure.Utils.ClearDataBase();
    Environment.SetEnvironmentVariable("SETPARTLINEVALUES_VERSION", "V4");
    var v4Result = await RunInvoiceImport(testFile);
    
    // Compare results
    _logger.Information("**COMPARISON**: V1={V1Count} items, V4={V4Count} items", 
        v1Result.InvoiceDetails.Count, v4Result.InvoiceDetails.Count);
}
```

### Strategy 3: Performance Comparison
```csharp
public async Task MeasureVersionPerformance(string testFile)
{
    var versions = new[] { "V1", "V2", "V3", "V4", "V5" };
    
    foreach (var version in versions)
    {
        Environment.SetEnvironmentVariable("SETPARTLINEVALUES_VERSION", version);
        
        var stopwatch = Stopwatch.StartNew();
        var result = await RunInvoiceImport(testFile);
        stopwatch.Stop();
        
        _logger.Information("**PERFORMANCE_{Version}**: {ElapsedMs}ms, {ItemCount} items", 
            version, stopwatch.ElapsedMilliseconds, result.InvoiceDetails.Count);
    }
}
```

---

## 🔧 IMPLEMENTATION DETAILS

### Version-Specific Logging
Each version has distinctive log markers:
```csharp
// V1 logs
_logger.Information("**VERSION_1_TEST**: Processing parent instance with {Count} child instances", childCount);

// V4 logs  
_logger.Information("**VERSION_4_TEST**: CRITICAL DECISION: hasProductFields && hasMultipleLines = {Decision}", decision);

// V5 logs
_logger.Information("**VERSION_5_TEST**: Same consolidation logic as V4 with improved logging");
```

### Data Structure Preservation
Each version maintains its own helper methods:
```csharp
// V1 helpers
private void ProcessInstance_V1(...)
private void ProcessChildParts_V1(...)
private void PopulateParentFields_V1(...)

// V4 helpers
private void ProcessInstanceWithItemConsolidation_V4(...)
private List<LogicalInvoiceItem> GroupIntoLogicalInvoiceItems_V4(...)
private List<LogicalInvoiceItem> ProcessAsSingleConsolidatedItem_V4(...)
```

---

## 🎯 PRACTICAL USAGE EXAMPLES

### Example 1: Debug Tropical Vendors Issue
```bash
# Test with V3 (should work)
set SETPARTLINEVALUES_VERSION=V3
# Run your test - should get 66 items

# Test with V4 (broken)  
set SETPARTLINEVALUES_VERSION=V4
# Run same test - should get 2 items
```

### Example 2: Debug Amazon Issue
```bash
# Test with V1 (should fail with TotalsZero != 0)
set SETPARTLINEVALUES_VERSION=V1  
# Run Amazon test - should fail

# Test with V4 (should work)
set SETPARTLINEVALUES_VERSION=V4
# Run Amazon test - should pass
```

### Example 3: Validate New Fix
```bash
# Test proposed fix version
set SETPARTLINEVALUES_VERSION=V6_PROPOSED
# Should work for both Amazon AND Tropical Vendors
```

---

## 🚨 IMPORTANT CONSIDERATIONS

### 1. Database State
- **Clear database** between version tests
- **Same input data** for meaningful comparison
- **Isolated test environment** to avoid contamination

### 2. Version Completeness
- Each version is **complete and self-contained**
- **No shared state** between versions
- **Independent helper methods** for each version

### 3. Logging Strategy
- **Version-specific log markers** for easy filtering
- **Comprehensive debugging output** in each version
- **Performance metrics** for comparison

### 4. Test Data Requirements
- **Tropical Vendors**: Multi-page invoice with 50+ line items
- **Amazon**: Complex invoice with deduction structure
- **Other types**: Budget Marine, Shein, etc. for regression testing

This framework allows you to **directly compare** how the exact same invoice data is processed by different versions of the code, making it possible to see exactly where and why the processing logic changed.