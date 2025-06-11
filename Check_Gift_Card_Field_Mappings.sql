-- Check Gift Card Field Mappings in Database
-- Purpose: Verify current state of LineId 1830 (Gift Card) field mappings
-- Shows duplicate field mappings that need cleanup

USE [WebSource-AutoBot]
GO

PRINT '========================================='
PRINT 'Gift Card Field Mappings Analysis'
PRINT 'LineId 1830 (Gift Card Amount: -$6.99)'
PRINT '========================================='
PRINT ''

-- Query all field mappings for Gift Card line
SELECT 
    F.FieldId,
    F.LineId,
    F.[Key],
    F.Field,
    F.EntityType,
    F.CreationDate,
    F.UpdateDate
FROM [OCR].[Fields] F
WHERE F.LineId = 1830
ORDER BY F.FieldId;

PRINT ''
PRINT '========================================='
PRINT 'Analysis Summary'
PRINT '========================================='

-- Show count of mappings
SELECT 
    'Total Field Mappings for LineId 1830' as [Description],
    COUNT(*) as [Count]
FROM [OCR].[Fields] F
WHERE F.LineId = 1830;

-- Show specific mapping details
SELECT 
    CASE 
        WHEN F.FieldId = 2579 THEN 'DUPLICATE - Should be DELETED'
        WHEN F.FieldId = 3181 THEN 'CORRECT - Should be KEPT'
        ELSE 'UNKNOWN MAPPING'
    END as [Status],
    F.FieldId,
    F.[Key] + ' → ' + F.Field as [Mapping],
    F.EntityType,
    F.CreationDate
FROM [OCR].[Fields] F
WHERE F.LineId = 1830
ORDER BY F.FieldId;

PRINT ''
PRINT '========================================='
PRINT 'Expected Cleanup Action'
PRINT '========================================='
PRINT 'DELETE FieldId 2579: GiftCard → TotalOtherCost (incorrect mapping)'
PRINT 'KEEP   FieldId 3181: TotalInsurance → TotalInsurance (correct mapping)'
PRINT ''
PRINT 'Caribbean Customs Rule: Gift Card Amount (-$6.99) should map to TotalInsurance (customer reduction, negative value)'