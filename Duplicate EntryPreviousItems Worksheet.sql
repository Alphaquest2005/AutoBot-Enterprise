SELECT xcuda_Item.ASYCUDA_Id, xcuda_Item.Item_Id, 'Duplicate Entry Previous Items' AS Type, EntryPreviousItems.PreviousItem_Id, xcuda_PreviousItem.Current_item_number
FROM   xcuda_Item INNER JOIN
             EntryPreviousItems ON xcuda_Item.Item_Id = EntryPreviousItems.Item_Id INNER JOIN
             xcuda_PreviousItem ON EntryPreviousItems.PreviousItem_Id = xcuda_PreviousItem.PreviousItem_Id
GROUP BY xcuda_Item.Item_Id, EntryPreviousItems.PreviousItem_Id, xcuda_PreviousItem.Current_item_number, xcuda_Item.ASYCUDA_Id
HAVING (COUNT(EntryPreviousItems.PreviousItem_Id) > 1)

SELECT xcuda_Item.ASYCUDA_Id, xcuda_Item.Item_Id, 'Duplicate Entry Previous Items' AS Type, EntryPreviousItems.PreviousItem_Id, xcuda_PreviousItem.Current_item_number
FROM   xcuda_Item INNER JOIN
             EntryPreviousItems ON xcuda_Item.Item_Id = EntryPreviousItems.Item_Id INNER JOIN
             xcuda_PreviousItem ON EntryPreviousItems.PreviousItem_Id = xcuda_PreviousItem.PreviousItem_Id
GROUP BY xcuda_Item.Item_Id, EntryPreviousItems.PreviousItem_Id, xcuda_PreviousItem.Current_item_number, xcuda_Item.ASYCUDA_Id



SELECT Min(EntryPreviousItems.EntryPreviousItemId)
FROM   xcuda_Item INNER JOIN
             EntryPreviousItems ON xcuda_Item.Item_Id = EntryPreviousItems.Item_Id INNER JOIN
             xcuda_PreviousItem ON EntryPreviousItems.PreviousItem_Id = xcuda_PreviousItem.PreviousItem_Id
GROUP BY xcuda_Item.Item_Id, EntryPreviousItems.PreviousItem_Id, xcuda_PreviousItem.Current_item_number, xcuda_Item.ASYCUDA_Id
HAVING (COUNT(EntryPreviousItems.PreviousItem_Id) > 1)

