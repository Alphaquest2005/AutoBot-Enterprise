update [OCR-RegularExpressions] 
set regex = reg1 
from 
(SELECT Id, RegEx, substring(regex, 2, len(regex)-1) as reg1, MultiLine
FROM    [OCR-RegularExpressions]
WHERE (ISNULL(MultiLine, 0) = 0) and left(regex,1) = '^') as t
where t.id = [OCR-RegularExpressions].Id

update [OCR-RegularExpressions] 
set regex = reg1 
from 
(SELECT Id, RegEx, substring(regex, 1, len(regex)-1) as reg1, MultiLine
FROM    [OCR-RegularExpressions]
WHERE (ISNULL(MultiLine, 0) = 0) and Right(regex,1) = '$') as t
where t.id = [OCR-RegularExpressions].Id