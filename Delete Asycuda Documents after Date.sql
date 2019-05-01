

delete from xcuda_Item
where ASYCUDA_Id in (
SELECT        xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id
FROM            xcuda_ASYCUDA_ExtendedProperties INNER JOIN
                         xcuda_Registration ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = xcuda_Registration.ASYCUDA_Id
WHERE        (cast(xcuda_Registration.[Date] as Date) >= '11/15/2016'))

delete from xcuda_ASYCUDA
where ASYCUDA_Id in (
SELECT        xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id
FROM            xcuda_ASYCUDA_ExtendedProperties INNER JOIN
                         xcuda_Registration ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = xcuda_Registration.ASYCUDA_Id
WHERE        (cast(xcuda_Registration.[Date] as Date) >= '11/15/2016'))