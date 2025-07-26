# Domain Model Regeneration Instructions

## Overview
After running the `OCRCorrectionLearning_Enhancement.sql` script, you need to regenerate the domain models to include the new `SuggestedRegex` field in the `OCRCorrectionLearning` entity.

## Step-by-Step Process

### 1. Run Database Update Script
First, execute the database enhancement script:
```sql
-- Run this script against your OCR database
-- File: Database_Updates/OCRCorrectionLearning_Enhancement.sql
```

### 2. Regenerate Business Entities

#### Option A: Using T4 Templates (Recommended)
If you're using T4 templates for code generation:

1. **Locate the T4 Template File**:
   ```
   /WaterNut.Business.Entities/Generated Business Entities/OCR/AllBusinessEntities.tt
   ```

2. **Right-click the .tt file** in Visual Studio and select **"Run Custom Tool"**

3. **Verify Generation**: Check that `OCRCorrectionLearning.cs` now includes:
   ```csharp
   [DataMember]
   public string SuggestedRegex 
   {
       get { return _suggestedregex; }
       set 
       { 
           _suggestedregex = value;
           NotifyPropertyChanged();
       }
   }
   string _suggestedregex;
   ```

#### Option B: Using Entity Framework Model Update
If you're using Entity Framework model-first approach:

1. **Update .edmx Model**:
   - Open your `.edmx` file
   - Right-click → **"Update Model from Database"**
   - Select the `OCRCorrectionLearning` table
   - Click **"Finish"**

2. **Regenerate Code**:
   - Right-click `.edmx` file
   - Select **"Run Custom Tool"**

#### Option C: Manual Update (If T4/EF not available)
If you need to manually update the entity:

1. **Edit OCRCorrectionLearning.cs**:
   ```csharp
   // Add this property to the OCRCorrectionLearning class
   [DataMember]
   public string SuggestedRegex 
   {
       get { return _suggestedregex; }
       set 
       { 
           _suggestedregex = value;
           NotifyPropertyChanged();
       }
   }
   string _suggestedregex;
   ```

### 3. Update Code References

After domain model regeneration, update the OCR correction service to use the proper field:

**File to Update**: `InvoiceReader/OCRCorrectionService/OCRDatabaseUpdates.cs`

**Changes Needed**:
- Remove enhanced WindowText logic
- Use direct SuggestedRegex field assignment
- Update extraction methods

### 4. Verification Steps

#### Database Verification
```sql
-- Verify the field was added
SELECT TOP 5 
    FieldName,
    SuggestedRegex,
    WindowText,
    Success,
    CreatedDate
FROM OCRCorrectionLearning 
WHERE SuggestedRegex IS NOT NULL
ORDER BY CreatedDate DESC

-- Check migration was successful
SELECT 
    COUNT(*) as [Total_Records],
    COUNT(SuggestedRegex) as [Records_With_SuggestedRegex],
    COUNT(CASE WHEN WindowText LIKE '%SUGGESTED_REGEX:%' THEN 1 END) as [Remaining_In_WindowText]
FROM OCRCorrectionLearning
```

#### Code Generation Verification
```csharp
// Test that the property exists
var learning = new OCRCorrectionLearning();
learning.SuggestedRegex = "test"; // Should compile without errors
```

#### Build Verification
1. **Clean Solution**: Build → Clean Solution
2. **Rebuild All**: Build → Rebuild Solution
3. **Run Tests**: Execute OCR correction tests to verify functionality

### 5. Code Update Script

After domain model regeneration, run this code update to remove the WindowText workaround:

**Updated OCRDatabaseUpdates.cs Logic**:
```csharp
// OLD (Enhanced WindowText approach):
var enhancedWindowText = request.WindowText ?? "";
if (!string.IsNullOrWhiteSpace(request.SuggestedRegex))
{
    enhancedWindowText = $"{enhancedWindowText}|SUGGESTED_REGEX:{request.SuggestedRegex}";
}

// NEW (Direct field approach):
var learning = new OCRCorrectionLearning
{
    // ... other fields ...
    WindowText = request.WindowText,
    SuggestedRegex = request.SuggestedRegex, // Direct assignment
    // ... other fields ...
};
```

## Expected Results

### Before Enhancement
```csharp
// OCRCorrectionLearning entity lacks SuggestedRegex property
// Data stored in WindowText as: "original_text|SUGGESTED_REGEX:pattern"
```

### After Enhancement
```csharp
// OCRCorrectionLearning entity has proper SuggestedRegex property
learning.SuggestedRegex = "(?<InvoiceTotal>[\\d\\.]+)"; // Clean, direct assignment
learning.WindowText = "TOTAL AMOUNT US$ 123.45"; // Separate, clean window text
```

## Troubleshooting

### Common Issues

1. **"SuggestedRegex property not found"**
   - Verify database script ran successfully
   - Regenerate domain models
   - Clean and rebuild solution

2. **"Build errors after regeneration"**
   - Check for duplicate property definitions
   - Verify T4 template ran correctly
   - Clean bin/obj folders and rebuild

3. **"Data migration incomplete"**
   - Check database script output
   - Verify migration counts in script results
   - Run verification queries

### Rollback Plan (if needed)
```sql
-- Emergency rollback (removes the field)
ALTER TABLE [dbo].[OCRCorrectionLearning] DROP COLUMN [SuggestedRegex]
```

## Next Steps
After successful domain model regeneration:
1. Update OCRCorrectionService code to use proper field
2. Run MANGO template creation test
3. Verify OCRCorrectionLearning records are created correctly
4. Validate learning system analytics functionality