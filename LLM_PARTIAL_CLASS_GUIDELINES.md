# LLM-Friendly Partial Class Development Guidelines

## 🎯 **The Real Problem**
Partial classes are necessary for managing large codebases and context windows, but LLMs need strategies to work with them safely without creating conflicts.

## 🛡️ **LLM Development Strategies for Partial Classes**

### 1. **Always Use Codebase Retrieval First**
Before adding ANY method to a partial class, search for existing implementations.

```bash
# Search pattern for LLM:
1. codebase-retrieval: "Find all methods in [ClassName] partial class files"
2. Check for method name conflicts
3. Use unique method names if conflicts exist
4. Document which partial file contains which functionality
```

### 2. **Establish Clear Partial Class Boundaries**
Each partial class file should have a specific, well-defined purpose.

#### Current OCRCorrectionService Structure:
```
OCRCorrectionService/
├── OCRCorrectionService.cs          # Core service, constructor, main methods
├── OCRMetadataExtractor.cs          # Metadata extraction functionality  
├── OCRDeepSeekIntegration.cs        # DeepSeek API integration
├── OCRPatternCreation.cs            # Regex pattern creation
├── OCRCorrectionApplication.cs      # Applying corrections to invoices
├── OCRErrorDetection.cs             # Error detection logic
└── OCRLegacySupport.cs              # Static helper methods
```

### 3. **Use Descriptive Method Naming Conventions**
Prevent conflicts by using file-specific prefixes or suffixes.

#### Naming Convention:
```csharp
// OCRMetadataExtractor.cs - prefix with "Metadata"
private bool IsMetadataFieldInternal(string fieldName)
private OCRFieldMetadata ExtractMetadataFromTemplate(...)

// OCRDeepSeekIntegration.cs - prefix with "DeepSeek"  
private bool IsDeepSeekFieldValid(string fieldName)
private void ProcessDeepSeekResponse(...)

// OCRPatternCreation.cs - prefix with "Pattern"
private bool IsPatternApplicable(string pattern)
private string CreatePatternFromCorrection(...)
```

### 4. **Create Partial Class Documentation**
Maintain a clear map of what's in each file.

#### Implementation:
```csharp
// At the top of each partial class file:
/// <summary>
/// OCRCorrectionService - Metadata Extraction Functionality
/// 
/// Contains:
/// - ExtractEnhancedOCRMetadata()
/// - ExtractFieldMetadata() 
/// - IsMetadataFieldInternal()
/// - FindFieldInTemplate()
/// 
/// Dependencies: OCRCorrectionService.cs (main class)
/// </summary>
public partial class OCRCorrectionService
{
    // Implementation here
}
```

### 5. **Use Region Organization**
Organize methods within partial files using regions.

```csharp
public partial class OCRCorrectionService
{
    #region Metadata Extraction Core
    public Dictionary<string, OCRFieldMetadata> ExtractEnhancedOCRMetadata(...)
    #endregion

    #region Field Context Resolution  
    private OCRFieldMetadata ExtractFieldMetadata(...)
    #endregion

    #region Template Processing
    private FieldContext FindFieldInTemplate(...)
    #endregion

    #region Helper Methods
    private bool IsMetadataFieldInternal(string fieldName)
    #endregion
}
```

### 6. **LLM Workflow for Partial Class Changes**

#### Step-by-Step Process:
1. **Identify Target Functionality**: Determine which partial file should contain the new code
2. **Search Existing Methods**: Use codebase-retrieval to find all methods in the class
3. **Check for Conflicts**: Verify method names don't already exist
4. **Choose Appropriate File**: Add to the most logical partial file
5. **Use Unique Names**: If unsure, use file-specific prefixes
6. **Document Changes**: Update the file header documentation

#### Example LLM Workflow:
```bash
# 1. Search for existing methods
codebase-retrieval: "Find all IsMetadataField methods in OCRCorrectionService"

# 2. Check specific partial file
view: "InvoiceReader/OCRCorrectionService/OCRMetadataExtractor.cs"

# 3. If conflict exists, use unique name
# Instead of: IsMetadataField()
# Use: IsMetadataFieldInternal() or IsMetadataFieldForExtraction()

# 4. Add method to appropriate file
str-replace-editor: Add method to OCRMetadataExtractor.cs
```

### 7. **Conflict Resolution Strategies**

#### When Duplicate Methods Are Found:
1. **Rename with Purpose**: Add descriptive suffix indicating purpose
2. **Consolidate if Identical**: Remove duplicate if functionality is the same
3. **Refactor to Shared**: Move common functionality to base partial file
4. **Use Internal vs Public**: Different access levels for different purposes

#### Example Resolution:
```csharp
// OCRMetadataExtractor.cs
private bool IsMetadataFieldForExtraction(string fieldName)
{
    var metadataFields = new[] { "LineNumber", "FileLineNumber", "Section", "Instance" };
    return metadataFields.Contains(fieldName);
}

// OCRLegacySupport.cs  
private static bool IsMetadataFieldForLegacy(string fieldName)
{
    var metadataFields = new[] { "LineNumber", "FileLineNumber", "Section", "Instance" };
    return metadataFields.Contains(fieldName);
}
```

### 8. **Build Verification Strategy**

#### Always Test After Changes:
```bash
# Quick build test for specific project
MSBuild.exe "InvoiceReader\InvoiceReader.csproj" /t:Build /p:Configuration=Debug

# If build fails, check for:
# - Duplicate method definitions
# - Missing using statements
# - Incorrect access modifiers
```

## 🔧 **Immediate Fix for Current Issue**

The current `IsMetadataField` conflict should be resolved by:

1. **Keep the static version** in `OCRLegacySupport.cs` (used by static methods)
2. **Rename the instance version** in `OCRMetadataExtractor.cs` to `IsMetadataFieldInternal`
3. **Update the call site** to use the renamed method

## 📋 **Best Practices Summary**

### DO:
- ✅ Always search for existing methods before adding new ones
- ✅ Use descriptive, file-specific method names
- ✅ Document what each partial file contains
- ✅ Test builds after making changes
- ✅ Use regions to organize code within partial files

### DON'T:
- ❌ Add methods without checking for existing implementations
- ❌ Use generic method names like `Process()`, `IsValid()`, etc.
- ❌ Assume a method doesn't exist without searching
- ❌ Mix unrelated functionality in the same partial file
- ❌ Forget to update documentation when adding new methods

This approach maintains the benefits of partial classes (managing large files, context windows) while preventing LLM-induced conflicts.
