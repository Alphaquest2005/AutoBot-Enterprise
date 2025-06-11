-- Fix Free Shipping regex pattern to make currency code optional and capture negative values
UPDATE RegularExpressions 
SET RegEx = 'Free Shipping:\s*-?\$?(?<FreeShipping>[\d,.]+)'
WHERE Id = 2031;

-- Verify the update
SELECT 
    r.Id,
    r.RegEx,
    l.Name as LineName
FROM RegularExpressions r
INNER JOIN Lines l ON l.RegularExpressionId = r.Id
WHERE r.Id = 2031;
