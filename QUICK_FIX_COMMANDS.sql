-- 🚨 CRITICAL FIX: Delete problematic AutoOmission TotalDeduction patterns
-- These patterns incorrectly capture Grand Total ($166.30) as TotalDeduction
-- User confirmed: "yes,,, that is completley wrong"

USE [WebSource-AutoBot]
GO

PRINT '🔍 **BEFORE_FIX**: Checking current problematic patterns...'

-- Show what we're about to delete
SELECT 
    r.Id,
    r.RegEx,
    r.Description,
    r.CreatedDate
FROM RegularExpressions r
WHERE r.RegEx = '(?<TotalDeduction>\d+\.\d{2})'
ORDER BY r.CreatedDate DESC

-- Count patterns before deletion
DECLARE @BeforeCount INT
SELECT @BeforeCount = COUNT(*) FROM RegularExpressions WHERE RegEx = '(?<TotalDeduction>\d+\.\d{2})'
PRINT '📊 Patterns before deletion: ' + CAST(@BeforeCount AS VARCHAR(10))

-- 🗑️ DELETE THE PROBLEMATIC PATTERNS
DELETE FROM RegularExpressions 
WHERE RegEx = '(?<TotalDeduction>\d+\.\d{2})'

-- Check results
DECLARE @DeletedCount INT = @@ROWCOUNT
PRINT '🗑️ Deleted patterns: ' + CAST(@DeletedCount AS VARCHAR(10))

-- Verify no problematic patterns remain
DECLARE @AfterCount INT
SELECT @AfterCount = COUNT(*) FROM RegularExpressions WHERE RegEx = '(?<TotalDeduction>\d+\.\d{2})'
PRINT '📊 Patterns remaining: ' + CAST(@AfterCount AS VARCHAR(10))

-- Show remaining TotalDeduction patterns (should be more specific ones)
PRINT '✅ **REMAINING_PATTERNS**: More specific TotalDeduction patterns:'
SELECT 
    r.Id,
    r.RegEx,
    r.Description,
    LEN(r.RegEx) as PatternLength
FROM RegularExpressions r
WHERE r.RegEx LIKE '%TotalDeduction%'
ORDER BY LEN(r.RegEx), r.CreatedDate

-- Verify Amazon template state
PRINT '🔍 **AMAZON_TEMPLATE**: Checking Amazon template after fix:'
SELECT 
    i.Name as TemplateName,
    i.Id as TemplateId,
    COUNT(DISTINCT r.Id) as RegexCount
FROM Invoices i
LEFT JOIN Parts p ON p.TemplateId = i.Id  
LEFT JOIN Lines l ON l.PartId = p.Id
LEFT JOIN RegularExpressions r ON l.RegularExpressionsId = r.Id
WHERE i.Id = 5  -- Amazon template
GROUP BY i.Name, i.Id

PRINT '✅ **FIX_COMPLETE**: Problematic patterns deleted - Grand Total should no longer map to TotalDeduction'