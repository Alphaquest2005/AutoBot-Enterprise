-- =====================================================================
-- OCR Correction Learning Enhancement Script - FINAL CORRECTED VERSION
-- Purpose: Fix computed column indexing issue
-- Date: 2025-07-26 (Final Fix)
-- Author: Claude Code (AutoBot-Enterprise OCR Enhancement)
-- =====================================================================

-- NOTE: This script only fixes the remaining indexing issue
-- Run this after the previous script that added the fields

-- =====================================================================
-- STEP 1: Drop the problematic index if it exists (partial creation)
-- =====================================================================
IF EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[OCRCorrectionLearning]') AND name = 'IX_OCRCorrectionLearning_SuggestedRegex_Indexed')
BEGIN
    DROP INDEX [IX_OCRCorrectionLearning_SuggestedRegex_Indexed] ON [dbo].[OCRCorrectionLearning]
    PRINT 'INFO: Dropped existing problematic index'
END

-- =====================================================================
-- STEP 2: Create the corrected index (no WHERE filter on computed column)
-- =====================================================================
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[OCRCorrectionLearning]') AND name = 'IX_OCRCorrectionLearning_SuggestedRegex_Fixed')
BEGIN
    -- Option 1: Simple index without filter (most reliable)
    CREATE NONCLUSTERED INDEX [IX_OCRCorrectionLearning_SuggestedRegex_Fixed]
    ON [dbo].[OCRCorrectionLearning] ([SuggestedRegex_Indexed])
    
    PRINT 'SUCCESS: Created index IX_OCRCorrectionLearning_SuggestedRegex_Fixed (no filter)'
END
ELSE
BEGIN
    PRINT 'INFO: Index IX_OCRCorrectionLearning_SuggestedRegex_Fixed already exists'
END
GO

-- =====================================================================
-- STEP 3: Alternative - Create filtered index using base column
-- =====================================================================
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[OCRCorrectionLearning]') AND name = 'IX_OCRCorrectionLearning_SuggestedRegex_Filtered')
BEGIN
    -- Use the base column (SuggestedRegex) in the filter, index the computed column
    CREATE NONCLUSTERED INDEX [IX_OCRCorrectionLearning_SuggestedRegex_Filtered]
    ON [dbo].[OCRCorrectionLearning] ([SuggestedRegex_Indexed])
    WHERE [SuggestedRegex] IS NOT NULL AND [SuggestedRegex] != ''
    
    PRINT 'SUCCESS: Created filtered index IX_OCRCorrectionLearning_SuggestedRegex_Filtered (using base column filter)'
END
ELSE
BEGIN
    PRINT 'INFO: Filtered index IX_OCRCorrectionLearning_SuggestedRegex_Filtered already exists'
END
GO

-- =====================================================================
-- STEP 4: Verification
-- =====================================================================
PRINT '====================================================================='
PRINT 'FINAL INDEX FIX COMPLETE'
PRINT '====================================================================='

-- Check all indexes
PRINT 'CURRENT INDEXES ON OCRCorrectionLearning:'
SELECT 
    i.name as [Index_Name],
    i.type_desc as [Index_Type],
    CASE WHEN i.is_unique = 1 THEN 'Yes' ELSE 'No' END as [Is_Unique],
    CASE WHEN i.has_filter = 1 THEN 'Yes' ELSE 'No' END as [Has_Filter],
    CASE WHEN i.has_filter = 1 THEN i.filter_definition ELSE 'N/A' END as [Filter_Definition]
FROM sys.indexes i
WHERE i.object_id = OBJECT_ID(N'[dbo].[OCRCorrectionLearning]')
  AND i.name LIKE 'IX_OCRCorrectionLearning_%'
ORDER BY i.name

-- Test query to ensure indexing works
PRINT ''
PRINT 'INDEX FUNCTIONALITY TEST:'
SELECT 
    COUNT(*) as [Total_Records],
    COUNT(SuggestedRegex) as [Records_With_SuggestedRegex],
    COUNT(SuggestedRegex_Indexed) as [Records_With_Computed_Column]
FROM [dbo].[OCRCorrectionLearning]

PRINT ''
PRINT '====================================================================='
PRINT 'DATABASE ENHANCEMENT COMPLETE - Ready for domain model regeneration!'
PRINT 'SUMMARY:'
PRINT '- SuggestedRegex field: ADDED (NVARCHAR(MAX))'
PRINT '- SuggestedRegex_Indexed computed column: ADDED (first 450 chars)'
PRINT '- Indexes: CREATED (both simple and filtered options)'
PRINT '- Ready for T4 template regeneration'
PRINT '====================================================================='
GO