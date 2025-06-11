-- SQL Script to verify the current state of Gift Card mapping
-- This checks whether the DatabaseValidator actually removed the duplicate mapping

USE [WebSource-AutoBot]
GO

PRINT '=== GIFT CARD MAPPING VERIFICATION ==='
PRINT 'Checking current state of LineId 1830 (Gift Card) field mappings...'
PRINT ''

-- Show all current mappings for Gift Card line
SELECT 
    f.Id as FieldId,
    f.LineId,
    l.Name as LineName,
    f.[Key] as FieldKey,
    f.Field as TargetField,
    f.EntityType,
    f.DataType,
    f.AppendValues,
    f.IsRequired,
    p.Id as PartId,
    p.TemplateId,
    i.Name as TemplateName
FROM Fields f
    INNER JOIN Lines l ON f.LineId = l.Id
    INNER JOIN Parts p ON l.PartId = p.Id
    INNER JOIN Invoices i ON p.TemplateId = i.Id
WHERE f.LineId = 1830 -- Gift Card line
ORDER BY f.Id
GO

-- Count the mappings
DECLARE @MappingCount INT
SELECT @MappingCount = COUNT(*) FROM Fields WHERE LineId = 1830

PRINT '=== ANALYSIS ==='
IF @MappingCount = 0
BEGIN
    PRINT '❌ NO MAPPINGS: Gift Card line has no field mappings'
    PRINT 'This will break Gift Card processing functionality'
END
ELSE IF @MappingCount = 1
BEGIN
    PRINT '✅ CLEAN STATE: Gift Card line has exactly 1 field mapping'
    
    -- Show which mapping remains
    SELECT 
        CASE 
            WHEN Field = 'TotalInsurance' THEN '✅ CORRECT: Maps to TotalInsurance (Caribbean customs compliant)'
            WHEN Field = 'TotalOtherCost' THEN '⚠️ SUBOPTIMAL: Maps to TotalOtherCost (not Caribbean customs compliant)'
            ELSE '❓ UNKNOWN: Maps to ' + Field + ' (review needed)'
        END as MappingStatus,
        [Key] + ' → ' + Field as Mapping,
        EntityType
    FROM Fields 
    WHERE LineId = 1830
END
ELSE
BEGIN
    PRINT '❌ DUPLICATES EXIST: Gift Card line has ' + CAST(@MappingCount AS NVARCHAR(10)) + ' field mappings'
    PRINT 'DatabaseValidator did NOT successfully remove duplicates'
    PRINT ''
    PRINT 'Current duplicate mappings:'
    
    SELECT 
        f.Id as FieldId,
        f.[Key] + ' → ' + f.Field as Mapping,
        f.EntityType,
        CASE 
            WHEN f.Field = 'TotalInsurance' THEN 'KEEP (Caribbean customs compliant)'
            WHEN f.Field = 'TotalOtherCost' AND f.[Key] = 'GiftCard' THEN 'REMOVE (duplicate)'
            ELSE 'REVIEW (unknown)'
        END as Recommendation
    FROM Fields f
    WHERE f.LineId = 1830
    ORDER BY f.Id
END
GO

-- Check specifically for the user-reported duplicate (FieldId: 2579)
PRINT '=== SPECIFIC DUPLICATE CHECK ==='
IF EXISTS (SELECT 1 FROM Fields WHERE Id = 2579)
BEGIN
    PRINT '❌ FIELDID 2579 STILL EXISTS: The specific duplicate mentioned by user is still in database'
    
    SELECT 
        'USER REPORTED DUPLICATE:' as Status,
        Id as FieldId,
        LineId,
        [Key],
        Field,
        EntityType,
        'This should be deleted' as Action
    FROM Fields 
    WHERE Id = 2579
END
ELSE
BEGIN
    PRINT '✅ FIELDID 2579 DELETED: The specific duplicate (FieldId: 2579) has been removed'
END
GO

-- Show all duplicates in Amazon template for context
PRINT '=== ALL AMAZON TEMPLATE DUPLICATES ==='
PRINT 'Showing all lines in Amazon template with multiple field mappings:'
PRINT ''

SELECT 
    f.LineId,
    l.Name as LineName,
    COUNT(*) as FieldCount,
    STRING_AGG(f.[Key] + ' → ' + f.Field, ', ') as AllMappings
FROM Fields f
    INNER JOIN Lines l ON f.LineId = l.Id
    INNER JOIN Parts p ON l.PartId = p.Id
    INNER JOIN Invoices i ON p.TemplateId = i.Id
WHERE i.Id = 5 -- Amazon template
GROUP BY f.LineId, l.Name
HAVING COUNT(*) > 1
ORDER BY f.LineId
GO

-- Summary and recommendations
PRINT '=== SUMMARY AND RECOMMENDATIONS ==='

DECLARE @GiftCardCount INT
SELECT @GiftCardCount = COUNT(*) FROM Fields WHERE LineId = 1830

IF @GiftCardCount = 1
BEGIN
    DECLARE @Field NVARCHAR(255)
    SELECT @Field = Field FROM Fields WHERE LineId = 1830
    
    IF @Field = 'TotalInsurance'
        PRINT '🎉 PERFECT: Gift Card mapping is clean and Caribbean customs compliant'
    ELSE
        PRINT '⚠️ CLEAN BUT SUBOPTIMAL: Gift Card mapping is clean but maps to ' + @Field + ' instead of TotalInsurance'
END
ELSE IF @GiftCardCount > 1
BEGIN
    PRINT '🚨 ACTION REQUIRED: Run the RemoveDuplicateGiftCardMapping.sql script to fix duplicates'
END
ELSE
BEGIN
    PRINT '🚨 CRITICAL ISSUE: Gift Card line has no mappings - functionality will be broken'
END

PRINT ''
PRINT '=== VERIFICATION COMPLETE ==='