DELETE FROM EntryPreviousItems

--select EntryPreviousItems.*
FROM            EntryPreviousItems INNER JOIN
                         AsycudaItemBasicInfo AS PreviousItem ON EntryPreviousItems.Item_Id = PreviousItem.Item_Id LEFT OUTER JOIN
                         AsycudaItemBasicInfo AS Item ON EntryPreviousItems.PreviousItem_Id = Item.Item_Id
WHERE         (Item.Item_Id IS NULL) --AND(EntryPreviousItems.Item_Id = 1404023) 