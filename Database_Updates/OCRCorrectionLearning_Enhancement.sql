-- =====================================================================
-- OCR Correction Learning Enhancement Script
-- Purpose: Add SuggestedRegex field to OCRCorrectionLearning table
-- Date: 2025-07-26
-- Author: Claude Code (AutoBot-Enterprise OCR Enhancement)
-- =====================================================================

USE [AutoBot_OCR] -- Replace with your actual database name
GO

-- =====================================================================
-- STEP 1: Add SuggestedRegex field to OCRCorrectionLearning table
-- =====================================================================
IF NOT EXISTS (SELECT * FROM sys.columns WHERE object_id = OBJECT_ID(N'[dbo].[OCRCorrectionLearning]') AND name = 'SuggestedRegex')
BEGIN
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
-- STEP 2: Create index for better query performance on SuggestedRegex
-- =====================================================================
IF NOT EXISTS (SELECT * FROM sys.indexes WHERE object_id = OBJECT_ID(N'[dbo].[OCRCorrectionLearning]') AND name = 'IX_OCRCorrectionLearning_SuggestedRegex')
BEGIN
    CREATE NONCLUSTERED INDEX [IX_OCRCorrectionLearning_SuggestedRegex]
    ON [dbo].[OCRCorrectionLearning] ([SuggestedRegex])
    WHERE [SuggestedRegex] IS NOT NULL
    
    PRINT 'SUCCESS: Created index IX_OCRCorrectionLearning_SuggestedRegex'
END
ELSE
BEGIN
    PRINT 'INFO: Index IX_OCRCorrectionLearning_SuggestedRegex already exists'
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
-- STEP 5: Add additional helpful indexes for learning system
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
PRINT 'OCR CORRECTION LEARNING ENHANCEMENT SUMMARY'
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
  AND COLUMN_NAME IN ('WindowText', 'SuggestedRegex')
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
    CASE WHEN i.is_unique = 1 THEN 'Yes' ELSE 'No' END as [Is_Unique]
FROM sys.indexes i
WHERE i.object_id = OBJECT_ID(N'[dbo].[OCRCorrectionLearning]')
  AND i.name LIKE 'IX_OCRCorrectionLearning_%'
ORDER BY i.name

PRINT ''
PRINT '====================================================================='
PRINT 'ENHANCEMENT COMPLETE - Ready for domain model regeneration'
PRINT '====================================================================='
GO