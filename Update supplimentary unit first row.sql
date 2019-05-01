
update dbo.xcuda_Supplementary_unit
set isfirstrow = 1
where Supplementary_unit_Id in (
SELECT Supplementary_unit_Id
    FROM (SELECT *, ROW_NUMBER() OVER(PARTITION BY Tarification_Id ORDER BY Supplementary_unit_Id) AS RowNum
              FROM dbo.xcuda_Supplementary_unit WITH (NOLOCK)) t
    WHERE t.RowNum = 1)