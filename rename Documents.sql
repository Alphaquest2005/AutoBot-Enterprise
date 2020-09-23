UPDATE xcuda_Declarant
SET Number =  N'Cleanup2020-' + cast(xcuda_ASYCUDA_ExtendedProperties.FileNumber as nvarchar(5)) 
FROM    xcuda_ASYCUDA_ExtendedProperties INNER JOIN
                 xcuda_Declarant ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = xcuda_Declarant.ASYCUDA_Id
WHERE (xcuda_ASYCUDA_ExtendedProperties.ImportComplete = 0)

--------- rename
select Replace(number, 'August 2020', 'CleanUp2020') as t
FROM    xcuda_ASYCUDA_ExtendedProperties INNER JOIN
                 xcuda_Declarant ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = xcuda_Declarant.ASYCUDA_Id
WHERE (xcuda_ASYCUDA_ExtendedProperties.ImportComplete = 0)

--------- reset count
UPDATE xcuda_ASYCUDA_ExtendedProperties
SET FileNumber = num.id
FROM    xcuda_ASYCUDA_ExtendedProperties INNER JOIN
                 xcuda_Declarant ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = xcuda_Declarant.ASYCUDA_Id
				 inner join (select  cast(ROW_NUMBER() OVER (ORDER BY xcuda_ASYCUDA_ExtendedProperties.Asycuda_Id,
                  xcuda_ASYCUDA_ExtendedProperties.Asycuda_Id) AS int) as id, Asycuda_ID from xcuda_ASYCUDA_ExtendedProperties) as num on xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = num.ASYCUDA_Id
WHERE (xcuda_ASYCUDA_ExtendedProperties.ImportComplete = 0)

SET FileNumber = num.id
FROM    xcuda_ASYCUDA_ExtendedProperties INNER JOIN
                 xcuda_Declarant ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = xcuda_Declarant.ASYCUDA_Id
				 inner join (select  cast(ROW_NUMBER() OVER (ORDER BY xcuda_ASYCUDA_ExtendedProperties.Asycuda_Id,
                  xcuda_ASYCUDA_ExtendedProperties.Asycuda_Id) AS int) as id, Asycuda_ID from xcuda_ASYCUDA_ExtendedProperties) as num on xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = num.ASYCUDA_Id
WHERE (xcuda_ASYCUDA_ExtendedProperties.ImportComplete = 0)