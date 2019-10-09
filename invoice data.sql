SELECT EntryDataEx.SupplierCode, AsycudaDocument.RegistrationDate, AsycudaDocument.CNumber, AsycudaDocument.ReferenceNumber, AsycudaDocument.DocumentType, MAX(EntryDataEx.InvoiceNo) AS InvoiceNo, 
                 AsycudaDocument.TotalCIF, AsycudaDocument.ApplicationSettingsId, AsycudaDocument.ImportComplete
FROM    EntryDataDetails INNER JOIN
                 AsycudaDocumentItemEntryDataDetails ON EntryDataDetails.EntryDataDetailsId = AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId INNER JOIN
                 EntryDataEx ON EntryDataDetails.EntryDataId = EntryDataEx.InvoiceNo INNER JOIN
                 xcuda_Item ON AsycudaDocumentItemEntryDataDetails.Item_Id = xcuda_Item.Item_Id RIGHT OUTER JOIN
                 AsycudaDocument ON xcuda_Item.ASYCUDA_Id = AsycudaDocument.ASYCUDA_Id
GROUP BY EntryDataEx.SupplierCode, AsycudaDocument.RegistrationDate, AsycudaDocument.CNumber, AsycudaDocument.DocumentType, AsycudaDocument.TotalCIF, AsycudaDocument.ApplicationSettingsId, 
                 AsycudaDocument.ImportComplete, AsycudaDocument.ReferenceNumber,MAX(EntryDataEx.InvoiceNo)
HAVING (AsycudaDocument.ImportComplete = 1) AND (AsycudaDocument.ApplicationSettingsId = 2) AND (AsycudaDocument.RegistrationDate >= CONVERT(DATETIME, '2019-08-25 00:00:00', 102))