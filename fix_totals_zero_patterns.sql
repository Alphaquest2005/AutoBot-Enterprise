-- CRITICAL FIX: Delete problematic AutoOmission TotalDeduction patterns
-- These patterns incorrectly capture Grand Total ($166.30) as TotalDeduction
-- User confirmed: "yes,,, that is completley wrong"

USE [OCR]
GO

-- First, show what we're about to delete for verification
PRINT '=== PROBLEMATIC PATTERNS TO DELETE ==='
SELECT 
    r.Id,
    r.RegEx,
    r.Description,
    r.CreatedDate,
    COUNT(l.Id) as LineCount
FROM RegularExpressions r
LEFT JOIN Lines l ON l.RegularExpressionsId = r.Id
WHERE r.RegEx = '(?<TotalDeduction>\d+\.\d{2})'
   AND (r.Description LIKE '%AutoOmission%TotalDeduction%' 
        OR r.Description LIKE '%AutoOmission_TotalDeduction%'
        OR r.Description IS NULL)
GROUP BY r.Id, r.RegEx, r.Description, r.CreatedDate
ORDER BY r.CreatedDate DESC

-- Check current state before deletion
DECLARE @BeforeCount INT
SELECT @BeforeCount = COUNT(*) 
FROM RegularExpressions 
WHERE RegEx = '(?<TotalDeduction>\d+\.\d{2})'
PRINT 'Patterns before deletion: ' + CAST(@BeforeCount AS VARCHAR(10))

-- BEGIN TRANSACTION for safety
BEGIN TRANSACTION DeleteProblematicPatterns

    -- Delete the problematic AutoOmission patterns
    DELETE FROM RegularExpressions 
    WHERE RegEx = '(?<TotalDeduction>\d+\.\d{2})'
       AND (Description LIKE '%AutoOmission%TotalDeduction%' 
            OR Description LIKE '%AutoOmission_TotalDeduction%'
            OR Description IS NULL)
    
    -- Check how many were deleted
    DECLARE @DeletedCount INT = @@ROWCOUNT
    PRINT 'Deleted patterns: ' + CAST(@DeletedCount AS VARCHAR(10))
    
    -- Verify no problematic patterns remain
    DECLARE @AfterCount INT
    SELECT @AfterCount = COUNT(*) 
    FROM RegularExpressions 
    WHERE RegEx = '(?<TotalDeduction>\d+\.\d{2})'
    PRINT 'Patterns remaining: ' + CAST(@AfterCount AS VARCHAR(10))
    
    -- Show remaining TotalDeduction patterns (should be more specific ones)
    PRINT '=== REMAINING TOTALDEDUCTION PATTERNS ==='
    SELECT 
        r.Id,
        r.RegEx,
        r.Description,
        r.CreatedDate
    FROM RegularExpressions r
    WHERE r.RegEx LIKE '%TotalDeduction%'
    ORDER BY r.CreatedDate DESC
    
    -- If everything looks good, commit
    IF @DeletedCount > 0 AND @AfterCount < @BeforeCount
    BEGIN
        PRINT 'SUCCESS: Committing deletion of ' + CAST(@DeletedCount AS VARCHAR(10)) + ' problematic patterns'
        COMMIT TRANSACTION DeleteProblematicPatterns
    END
    ELSE
    BEGIN
        PRINT 'ERROR: Unexpected results, rolling back transaction'
        ROLLBACK TRANSACTION DeleteProblematicPatterns
    END

GO

-- Final verification - Check Amazon template state
PRINT '=== AMAZON TEMPLATE VERIFICATION ==='
SELECT 
    i.Name as TemplateName,
    i.Id as TemplateId,
    COUNT(DISTINCT p.Id) as PartCount,
    COUNT(DISTINCT l.Id) as LineCount,
    COUNT(DISTINCT r.Id) as RegexCount
FROM Invoices i
LEFT JOIN Parts p ON p.TemplateId = i.Id  
LEFT JOIN Lines l ON l.PartId = p.Id
LEFT JOIN RegularExpressions r ON l.RegularExpressionsId = r.Id
WHERE i.Id = 5  -- Amazon template
GROUP BY i.Name, i.Id

-- Check for any remaining overly broad patterns
PRINT '=== POTENTIAL REMAINING ISSUES ==='
SELECT 
    r.Id,
    r.RegEx,
    r.Description,
    LEN(r.RegEx) as PatternLength
FROM RegularExpressions r
WHERE r.RegEx LIKE '%TotalDeduction%'
  AND LEN(r.RegEx) < 50  -- Short patterns are likely too broad
ORDER BY LEN(r.RegEx), r.CreatedDate

PRINT '=== FIX COMPLETE ==='