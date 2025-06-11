-- SQL Script to permanently remove the duplicate Gift Card mapping
-- This addresses the issue where LineId 1830 (Gift Card) has two field mappings:
-- 1. FieldId 2579: GiftCard → TotalOtherCost (DUPLICATE - should be removed)
-- 2. FieldId 3181: TotalInsurance → TotalInsurance (CORRECT - should be kept)

USE [WebSource-AutoBot]
GO

-- First, let's see the current state
PRINT '=== CURRENT STATE OF GIFT CARD MAPPINGS ==='
SELECT 
    f.Id as FieldId,
    f.LineId,
    l.Name as LineName,
    f.Key,
    f.Field,
    f.EntityType,
    f.DataType,
    f.AppendValues,
    p.TemplateId,
    i.Name as TemplateName
FROM Fields f
    INNER JOIN Lines l ON f.LineId = l.Id
    INNER JOIN Parts p ON l.PartId = p.Id
    INNER JOIN Invoices i ON p.TemplateId = i.Id
WHERE f.LineId = 1830 -- Gift Card line
ORDER BY f.Id
GO

-- Check if FieldId 2579 exists and is the problematic duplicate
IF EXISTS (
    SELECT 1 FROM Fields 
    WHERE Id = 2579 
    AND LineId = 1830 
    AND Field = 'TotalOtherCost' 
    AND [Key] = 'GiftCard'
)
BEGIN
    PRINT '=== FOUND DUPLICATE MAPPING TO REMOVE ==='
    SELECT 
        'DUPLICATE FOUND:' as Status,
        Id as FieldId,
        LineId,
        [Key],
        Field,
        EntityType
    FROM Fields 
    WHERE Id = 2579

    -- Remove the duplicate mapping
    PRINT '=== REMOVING DUPLICATE MAPPING ==='
    DELETE FROM Fields 
    WHERE Id = 2579 
    AND LineId = 1830 
    AND Field = 'TotalOtherCost' 
    AND [Key] = 'GiftCard'
    
    PRINT 'Duplicate Gift Card mapping (FieldId: 2579) has been permanently removed.'
END
ELSE
BEGIN
    PRINT '=== DUPLICATE MAPPING NOT FOUND ==='
    PRINT 'FieldId 2579 with GiftCard → TotalOtherCost mapping was not found.'
    PRINT 'It may have already been removed or the mapping details have changed.'
END
GO

-- Verify the final state
PRINT '=== FINAL STATE AFTER CLEANUP ==='
SELECT 
    f.Id as FieldId,
    f.LineId,
    l.Name as LineName,
    f.Key,
    f.Field,
    f.EntityType,
    f.DataType,
    f.AppendValues,
    p.TemplateId,
    i.Name as TemplateName
FROM Fields f
    INNER JOIN Lines l ON f.LineId = l.Id
    INNER JOIN Parts p ON l.PartId = p.Id
    INNER JOIN Invoices i ON p.TemplateId = i.Id
WHERE f.LineId = 1830 -- Gift Card line
ORDER BY f.Id
GO

-- Final verification
DECLARE @FieldCount INT
SELECT @FieldCount = COUNT(*) FROM Fields WHERE LineId = 1830

IF @FieldCount = 0
BEGIN
    PRINT '⚠️ WARNING: No field mappings remain for Gift Card line (LineId: 1830)'
    PRINT 'This might break Gift Card processing functionality.'
END
ELSE IF @FieldCount = 1
BEGIN
    PRINT '✅ SUCCESS: Gift Card line now has exactly 1 field mapping (clean state)'
    SELECT 
        'REMAINING MAPPING:' as Status,
        [Key] + ' → ' + Field as Mapping,
        EntityType,
        DataType
    FROM Fields 
    WHERE LineId = 1830
END
ELSE
BEGIN
    PRINT '❌ STILL DUPLICATED: Gift Card line still has ' + CAST(@FieldCount AS NVARCHAR(10)) + ' field mappings'
    PRINT 'Manual review required - additional duplicates may exist.'
END
GO

-- Check for FieldId 2579 specifically
IF NOT EXISTS (SELECT 1 FROM Fields WHERE Id = 2579)
BEGIN
    PRINT '✅ CONFIRMED: FieldId 2579 has been permanently deleted from database'
END
ELSE
BEGIN
    PRINT '❌ ISSUE: FieldId 2579 still exists in database'
    SELECT 
        'STILL EXISTS:' as Status,
        Id as FieldId,
        LineId,
        [Key],
        Field,
        EntityType
    FROM Fields 
    WHERE Id = 2579
END
GO

PRINT '=== CLEANUP SCRIPT COMPLETED ==='