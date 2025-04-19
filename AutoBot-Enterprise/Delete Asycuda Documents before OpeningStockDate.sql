delete from asycudasalesallocations 
where PreviousItem_Id in 
(select Item_Id from xcuda_Item
where ASYCUDA_Id in (
SELECT xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id
FROM     xcuda_ASYCUDA_ExtendedProperties INNER JOIN
                  xcuda_Registration ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = xcuda_Registration.ASYCUDA_Id 
WHERE  (xcuda_Registration.Date < (select OpeningStockDate from dbo.ApplicationSettings))))--

delete from xcuda_Item
where ASYCUDA_Id in (
SELECT xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id
FROM     xcuda_ASYCUDA_ExtendedProperties INNER JOIN
                  xcuda_Registration ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = xcuda_Registration.ASYCUDA_Id
WHERE  (xcuda_Registration.Date < (select OpeningStockDate from dbo.ApplicationSettings)))--

delete from xcuda_ASYCUDA
where ASYCUDA_Id in (
SELECT xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id
FROM     xcuda_ASYCUDA_ExtendedProperties INNER JOIN
                  xcuda_Registration ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = xcuda_Registration.ASYCUDA_Id
WHERE  (xcuda_Registration.Date < (select OpeningStockDate from dbo.ApplicationSettings)))--