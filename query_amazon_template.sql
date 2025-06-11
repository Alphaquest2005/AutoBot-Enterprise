
SELECT 
    l.Id as LineId,
    l.Name as LineName,
    r.Id as RegexId,
    r.RegEx as Pattern,
    f.Id as FieldId,
    f.Key,
    f.Field,
    f.EntityType
FROM Lines l
INNER JOIN RegularExpressions r ON l.RegularExpressionId = r.Id
LEFT JOIN Fields f ON f.LineId = l.Id
INNER JOIN Parts p ON l.PartId = p.Id
INNER JOIN Invoices i ON p.TemplateId = i.Id
WHERE i.Name = 'Amazon'
ORDER BY l.Id;

