SELECT        AsycudaDocumentItem.CNumber, AsycudaDocumentItem.RegistrationDate, AsycudaDocumentItem.LineNumber, AsycudaDocumentItem.ItemNumber, AsycudaDocumentItem.ItemQuantity, 
                         xcuda_PreviousItem.QtyAllocated, AsycudaDocumentItem.IsAssessed
FROM            AsycudaDocumentItem INNER JOIN
                         xcuda_PreviousItem ON AsycudaDocumentItem.Item_Id = xcuda_PreviousItem.PreviousItem_Id AND AsycudaDocumentItem.ItemQuantity <> xcuda_PreviousItem.QtyAllocated
WHERE        isassessed = 1 and (AsycudaDocumentItem.CNumber = '41156')