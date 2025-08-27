-- =====================================================================
-- OCR Correction Learning Enhancement Script - CORRECTED VERSION
-- Purpose: Add SuggestedRegex field to OCRCorrectionLearning table
-- Date: 2025-07-26 (Corrected)
-- Author: Claude Code (AutoBot-Enterprise OCR Enhancement)
-- =====================================================================

-- NOTE: Script will run against your current database connection
-- No USE statement - works with whatever database you're connected to

-- =====================================================================
-- STEP 1: Add SuggestedRegex field to OCRCorrectionLearning table
-- =====================================================================
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[OCRCorrectionLearning]') AND name = 'SuggestedRegex')
BEGIN
    -- Using NVARCHAR(MAX) for unlimited regex pattern length
    ALTER TABLE [dbo].[OCRCorrectionLearning]
    ADD [SuggestedRegex] NVARCHAR(MAX) NULL
    
    PRINT 'SUCCESS: Added SuggestedRegex field to OCRCorrectionLearning table'
END
ELSE
BEGIN
    PRINT 'INFO: SuggestedRegex field already exists in OCRCorrectionLearning table'
END
GO

-- =====================================================================
-- STEP 2: Create computed column index for SuggestedRegex (workaround for NVARCHAR(MAX))
-- =====================================================================
-- Since NVARCHAR(MAX) can't be indexed directly, we create a computed column
-- that takes the first 450 characters for indexing purposes

-- Add computed column for indexing
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[OCRCorrectionLearning]') AND name = 'SuggestedRegex_Indexed')
BEGIN
    ALTER TABLE [dbo].[OCRCorrectionLearning]
    ADD [SuggestedRegex_Indexed] AS CAST(LEFT(ISNULL([SuggestedRegex], ''), 450) AS NVARCHAR(450)) PERSISTED
    
    PRINT 'SUCCESS: Added SuggestedRegex_Indexed computed column for indexing'
END
ELSE
BEGIN
    PRINT 'INFO: SuggestedRegex_Indexed computed column already exists'
END
GO

-- Create index on the computed column
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[OCRCorrectionLearning]') AND name = 'IX_OCRCorrectionLearning_SuggestedRegex_Indexed')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_OCRCorrectionLearning_SuggestedRegex_Indexed]
    ON [dbo].[OCRCorrectionLearning] ([SuggestedRegex_Indexed])
    WHERE [SuggestedRegex_Indexed] IS NOT NULL AND [SuggestedRegex_Indexed] != ''
    
    PRINT 'SUCCESS: Created index IX_OCRCorrectionLearning_SuggestedRegex_Indexed'
END
ELSE
BEGIN
    PRINT 'INFO: Index IX_OCRCorrectionLearning_SuggestedRegex_Indexed already exists'
END
GO

-- =====================================================================
-- STEP 3: Migrate existing SuggestedRegex data from enhanced WindowText
-- =====================================================================
PRINT 'STEP 3: Migrating existing SuggestedRegex data from WindowText field...'

UPDATE [dbo].[OCRCorrectionLearning]
SET [SuggestedRegex] = 
    CASE 
        WHEN [WindowText] LIKE '%SUGGESTED_REGEX:%' THEN
            CASE
                WHEN [WindowText] LIKE '%|SUGGESTED_REGEX:%' THEN
                    -- Format: "original_text|SUGGESTED_REGEX:regex_value"
                    SUBSTRING([WindowText], CHARINDEX('|SUGGESTED_REGEX:', [WindowText]) + 17, LEN([WindowText]))
                WHEN [WindowText] LIKE 'SUGGESTED_REGEX:%' THEN
                    -- Format: "SUGGESTED_REGEX:regex_value"
                    SUBSTRING([WindowText], 17, LEN([WindowText]))
                ELSE NULL
            END
        ELSE NULL
    END
WHERE [WindowText] LIKE '%SUGGESTED_REGEX:%'
  AND ([SuggestedRegex] IS NULL OR [SuggestedRegex] = '')

DECLARE @MigratedCount INT = @@ROWCOUNT
PRINT 'SUCCESS: Migrated ' + CAST(@MigratedCount AS VARCHAR(10)) + ' SuggestedRegex values from WindowText'

-- =====================================================================
-- STEP 4: Clean up WindowText field by removing migrated SuggestedRegex data
-- =====================================================================
PRINT 'STEP 4: Cleaning up WindowText field...'

UPDATE [dbo].[OCRCorrectionLearning]
SET [WindowText] = 
    CASE 
        WHEN [WindowText] LIKE '%|SUGGESTED_REGEX:%' THEN
            -- Remove "|SUGGESTED_REGEX:value" from end
            LEFT([WindowText], CHARINDEX('|SUGGESTED_REGEX:', [WindowText]) - 1)
        WHEN [WindowText] LIKE 'SUGGESTED_REGEX:%' THEN
            -- If entire WindowText was just SuggestedRegex, set to empty
            ''
        ELSE [WindowText]
    END
WHERE [WindowText] LIKE '%SUGGESTED_REGEX:%'
  AND [SuggestedRegex] IS NOT NULL

DECLARE @CleanedCount INT = @@ROWCOUNT
PRINT 'SUCCESS: Cleaned ' + CAST(@CleanedCount AS VARCHAR(10)) + ' WindowText entries'

-- =====================================================================
-- STEP 5: Create analytics indexes (these work fine as they don't index NVARCHAR(MAX))
-- =====================================================================
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[OCRCorrectionLearning]') AND name = 'IX_OCRCorrectionLearning_Learning_Analytics')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_OCRCorrectionLearning_Learning_Analytics]
    ON [dbo].[OCRCorrectionLearning] ([Success], [Confidence], [CreatedDate])
    INCLUDE ([FieldName], [CorrectionType], [InvoiceType])
    
    PRINT 'SUCCESS: Created index IX_OCRCorrectionLearning_Learning_Analytics for analytics queries'
END
ELSE
BEGIN
    PRINT 'INFO: Index IX_OCRCorrectionLearning_Learning_Analytics already exists'
END
GO

-- =====================================================================
-- STEP 6: Verification and summary report  
-- =====================================================================
PRINT '====================================================================='
PRINT 'OCR CORRECTION LEARNING ENHANCEMENT SUMMARY - CORRECTED VERSION'
PRINT '====================================================================='

-- Check table structure
PRINT 'TABLE STRUCTURE VERIFICATION:'
SELECT 
    COLUMN_NAME,
    DATA_TYPE,
    IS_NULLABLE,
    CHARACTER_MAXIMUM_LENGTH
FROM INFORMATION_SCHEMA.COLUMNS 
WHERE TABLE_NAME = 'OCRCorrectionLearning' 
  AND COLUMN_NAME IN ('WindowText', 'SuggestedRegex', 'SuggestedRegex_Indexed')
ORDER BY COLUMN_NAME

-- Check data migration results
PRINT ''
PRINT 'DATA MIGRATION RESULTS:'
SELECT 
    'Total Records' as [Metric],
    COUNT(*) as [Count]
FROM [dbo].[OCRCorrectionLearning]

UNION ALL

SELECT 
    'Records with SuggestedRegex' as [Metric],
    COUNT(*) as [Count]
FROM [dbo].[OCRCorrectionLearning]
WHERE [SuggestedRegex] IS NOT NULL AND [SuggestedRegex] != ''

UNION ALL

SELECT 
    'Records still with SUGGESTED_REGEX in WindowText' as [Metric],
    COUNT(*) as [Count]
FROM [dbo].[OCRCorrectionLearning]
WHERE [WindowText] LIKE '%SUGGESTED_REGEX:%'

-- Check index creation
PRINT ''
PRINT 'INDEX VERIFICATION:'
SELECT 
    i.name as [Index_Name],
    i.type_desc as [Index_Type],
    CASE WHEN i.is_unique = 1 THEN 'Yes' ELSE 'No' END as [Is_Unique],
    CASE WHEN i.has_filter = 1 THEN 'Yes' ELSE 'No' END as [Has_Filter]
FROM sys.indexes i
WHERE i.object_id = OBJECT_ID(N'[dbo].[OCRCorrectionLearning]')
  AND i.name LIKE 'IX_OCRCorrectionLearning_%'
ORDER BY i.name

-- Test the computed column and index
PRINT ''
PRINT 'COMPUTED COLUMN TEST:'
SELECT TOP 3
    FieldName,
    LEFT(ISNULL(SuggestedRegex, 'NULL'), 50) + '...' as [SuggestedRegex_Sample],
    LEFT(ISNULL(SuggestedRegex_Indexed, 'NULL'), 50) + '...' as [SuggestedRegex_Indexed_Sample]
FROM [dbo].[OCRCorrectionLearning]
WHERE SuggestedRegex IS NOT NULL
ORDER BY CreatedDate DESC

PRINT ''
PRINT '====================================================================='
PRINT 'ENHANCEMENT COMPLETE - Issues Fixed:'
PRINT '1. Removed database name dependency - works with current connection'
PRINT '2. Fixed NVARCHAR(MAX) indexing issue with computed column approach'
PRINT '3. Ready for domain model regeneration'
PRINT '====================================================================='
GO