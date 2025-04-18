delete from xcuda_Item
where Item_id in 
(select AsycudaDocumentItemId from ( SELECT xcuda_Item.Item_Id, AsycudaDocument.ExpiryDate, PreviousItemsEx.AsycudaDocumentItemId, PreviousItemsEx.CNumber, PreviousItemsEx.RegistrationDate, PreviousItemsEx.ReferenceNumber, 
                 PreviousItemsEx.AssessmentDate
FROM    xcuda_Item INNER JOIN
                 AsycudaDocument ON xcuda_Item.ASYCUDA_Id = AsycudaDocument.ASYCUDA_Id INNER JOIN
                 PreviousItemsEx ON xcuda_Item.Item_Id = PreviousItemsEx.PreviousDocumentItemId
WHERE (AsycudaDocument.ExpiryDate <= GETDATE()) AND (PreviousItemsEx.CNumber IS NULL)) as t)


delete from xcuda_Item
where Item_id in 
(select AsycudaDocumentItemId from ( SELECT xcuda_Item.Item_Id, AsycudaDocument.ExpiryDate, PreviousItemsEx.AsycudaDocumentItemId, PreviousItemsEx.CNumber, PreviousItemsEx.RegistrationDate, PreviousItemsEx.ReferenceNumber, 
                 PreviousItemsEx.AssessmentDate, PreviousItemsEx.Goods_origin, AsycudaDocument.Country_first_destination
FROM    xcuda_Item INNER JOIN
                 AsycudaDocument ON xcuda_Item.ASYCUDA_Id = AsycudaDocument.ASYCUDA_Id INNER JOIN
                 PreviousItemsEx ON xcuda_Item.Item_Id = PreviousItemsEx.PreviousDocumentItemId
WHERE (PreviousItemsEx.CNumber IS NULL) and Country_first_destination <> Goods_origin ) as t)



delete from xcuda_Item
where Item_id in 
(select AsycudaDocumentItemId from ( SELECT xcuda_Item.Item_Id, AsycudaDocument.ExpiryDate, PreviousItemsEx.AsycudaDocumentItemId, PreviousItemsEx.CNumber, PreviousItemsEx.RegistrationDate, PreviousItemsEx.ReferenceNumber, 
                 PreviousItemsEx.AssessmentDate, PreviousItemsEx.Goods_origin, AsycudaDocument.Country_first_destination
FROM    xcuda_Item INNER JOIN
                 AsycudaDocument ON xcuda_Item.ASYCUDA_Id = AsycudaDocument.ASYCUDA_Id INNER JOIN
                 PreviousItemsEx ON xcuda_Item.Item_Id = PreviousItemsEx.PreviousDocumentItemId
WHERE (PreviousItemsEx.CNumber IS NULL) and Country_first_destination <> Goods_origin ) as t)


delete from xcuda_Item
where Item_id in 
(select t.AsycudaDocumentItemId from (SELECT xcuda_Item.Item_Id, AsycudaDocument.ExpiryDate, PreviousItemsEx.AsycudaDocumentItemId, PreviousItemsEx.CNumber, PreviousItemsEx.RegistrationDate, PreviousItemsEx.ReferenceNumber, 
                 PreviousItemsEx.AssessmentDate, PreviousItemsEx.Goods_origin, AsycudaDocument.Country_first_destination, PreviousItemsEx.Previous_item_number
FROM    xcuda_Item INNER JOIN
                 AsycudaDocument ON xcuda_Item.ASYCUDA_Id = AsycudaDocument.ASYCUDA_Id INNER JOIN
                 PreviousItemsEx ON xcuda_Item.Item_Id = PreviousItemsEx.PreviousDocumentItemId
WHERE (PreviousItemsEx.CNumber IS NULL) AND (PreviousItemsEx.Prev_reg_nbr in ('16000','15571', '15575', '15586','15814', '16010','15592','15589','15811','16009','15827','15595', '16002')) 
--AND (PreviousItemsEx.Previous_item_number = N'25')
) as t)

delete from xcuda_Item
where Item_id in 
(select t.AsycudaDocumentItemId from (SELECT xcuda_Item.Item_Id, AsycudaDocument.ExpiryDate, PreviousItemsEx.AsycudaDocumentItemId, PreviousItemsEx.CNumber, PreviousItemsEx.RegistrationDate, PreviousItemsEx.ReferenceNumber, 
                 PreviousItemsEx.AssessmentDate, PreviousItemsEx.Goods_origin, AsycudaDocument.Country_first_destination, PreviousItemsEx.Previous_item_number
FROM    xcuda_Item INNER JOIN
                 AsycudaDocument ON xcuda_Item.ASYCUDA_Id = AsycudaDocument.ASYCUDA_Id INNER JOIN
                 PreviousItemsEx ON xcuda_Item.Item_Id = PreviousItemsEx.PreviousDocumentItemId
WHERE (PreviousItemsEx.CNumber IS NULL) AND (PreviousItemsEx.Prev_reg_nbr = '23925') 
AND (PreviousItemsEx.Previous_item_number = N'2')
) as t)