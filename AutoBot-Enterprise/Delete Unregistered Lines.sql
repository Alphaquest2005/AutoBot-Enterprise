--update xcuda_Item
--set DoNotEx = 1
--where item_id in (
--SELECT xcuda_Item.Item_Id
--FROM   xcuda_ASYCUDA_ExtendedProperties INNER JOIN
--             xcuda_Registration ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = xcuda_Registration.ASYCUDA_Id INNER JOIN
--             xcuda_Item ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = xcuda_Item.ASYCUDA_Id INNER JOIN
--             xcuda_HScode ON xcuda_Item.Item_Id = xcuda_HScode.Item_Id
--WHERE (xcuda_Registration.Number IS not NULL) AND (xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed IS NULL) AND (xcuda_HScode.Precision_4 LIKE '%SEH/1005GH%'))--



delete from xcuda_Item
where item_id in (
SELECT xcuda_Item.Item_Id
FROM   xcuda_ASYCUDA_ExtendedProperties INNER JOIN
             xcuda_Registration ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = xcuda_Registration.ASYCUDA_Id INNER JOIN
             xcuda_Item ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = xcuda_Item.ASYCUDA_Id INNER JOIN
             xcuda_HScode ON xcuda_Item.Item_Id = xcuda_HScode.Item_Id
WHERE (xcuda_Registration.Number IS NULL) AND (xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed IS NULL) AND (xcuda_HScode.Precision_4 LIKE '%UNC/7010%'))--


--delete from xcuda_Item
--where item_id in (
--SELECT xcuda_Item.Item_Id
--FROM   xcuda_ASYCUDA_ExtendedProperties INNER JOIN
--             xcuda_Registration ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = xcuda_Registration.ASYCUDA_Id INNER JOIN
--             xcuda_Item ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = xcuda_Item.ASYCUDA_Id INNER JOIN
--             xcuda_HScode ON xcuda_Item.Item_Id = xcuda_HScode.Item_Id
--WHERE (xcuda_Registration.Number IS NULL) AND (xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed IS NULL) AND (xcuda_HScode.Commodity_Code LIKE '%8536900%'))--