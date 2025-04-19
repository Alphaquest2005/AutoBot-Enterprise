--SELECT        Asycuda_id, Lines, Type
--FROM            dbo.[DataCheck-DuplicateEntry]
--UNION
--SELECT        Asycuda_id, Lines, Type
--FROM            dbo.[DataCheck-IncompleteEntry]
--UNION
--SELECT        ASYCUDA_Id, COUNT(Item_Id) AS Lines, [Type]
--FROM            [DataCheck-IncompleteItems]
--GROUP BY ASYCUDA_Id, [type]
--UNION
--SELECT        ASYCUDA_Id, COUNT(Item_Id) AS Lines, Type
--FROM            [DataCheck-NullItemNumber]
--GROUP BY ASYCUDA_Id, Type
--UNION
--SELECT        ASYCUDA_Id, COUNT(PreviousItem_Id) AS Lines, Type
--FROM            [DataCheck-UnlinkedPreviousItem]
--GROUP BY ASYCUDA_Id, Type
--UNION
--SELECT        ASYCUDA_Id, Lines, Type
--FROM            [DataCheck-MissingLines]
--UNION
UPDATE       xcuda_Item
SET                WarehouseError = expired.Type
FROM            xcuda_Item INNER JOIN
                             (SELECT        xcuda_Item_1.Item_Id, AsycudaDocumentBasicInfo.ExpiryDate, 'Expired Entries - ' + CONVERT(varchar(10), AsycudaDocumentBasicInfo.ExpiryDate, 127) AS Type
                               FROM            xcuda_Item AS xcuda_Item_1 INNER JOIN
                                                         AsycudaDocumentBasicInfo ON xcuda_Item_1.ASYCUDA_Id = AsycudaDocumentBasicInfo.ASYCUDA_Id
                               WHERE        (AsycudaDocumentBasicInfo.ExpiryDate <= GETDATE())) AS expired ON xcuda_Item.Item_Id = expired.Item_Id

UPDATE       xcuda_Item
SET                WarehouseError = [DataCheck-MissingPrimarySupplementaryUnit].Type
FROM            xcuda_Item INNER JOIN
                         [DataCheck-MissingPrimarySupplementaryUnit] ON xcuda_Item.Item_Id = [DataCheck-MissingPrimarySupplementaryUnit].Item_Id


UPDATE       xcuda_Item
SET                WarehouseError = [DataCheck-NullItemNumber].Type
FROM            xcuda_Item INNER JOIN
                         [DataCheck-NullItemNumber] ON xcuda_Item.Item_Id = [DataCheck-NullItemNumber].Item_Id


--UNION
--SELECT        asycuda_id, COUNT(item_id) AS Lines, type
--FROM            [DataCheck-MissingSupplimentaryUnit]
--GROUP BY ASYCUDA_Id, Type



