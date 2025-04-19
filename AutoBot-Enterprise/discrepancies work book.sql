SELECT AsycudaDocument.CNumber, AsycudaDocument.ReferenceNumber, AsycudaDocument.AssessmentDate
FROM    Customs_Procedure INNER JOIN
                 AsycudaDocument ON Customs_Procedure.Customs_ProcedureId = AsycudaDocument.Customs_ProcedureId INNER JOIN
                 AsycudaDocumentItem ON AsycudaDocument.ASYCUDA_Id = AsycudaDocumentItem.AsycudaDocumentId
WHERE (AsycudaDocumentItem.ItemNumber in ('WAL/3002030170')) /*and (PreviousInvoiceNumber = 'IB513846')*/ AND (AsycudaDocumentItem.ApplicationSettingsId = 2) AND (Customs_Procedure.Discrepancy = 1)

select item_id, itemnumber, itemquantity, PiQuantity, PreviousInvoiceNumber, CNumber from asycudadocumentitem where  CNumber = 22986 order by LineNumber

select item_id, itemnumber, itemquantity, PiQuantity, PreviousInvoiceNumber, CNumber from asycudadocumentitem where Item_Id order by LineNumber


 and ItemNumber = 'AOR/115005F' 
ReferenceNumber = 'Marinco-F8'
 9887
9509

select * from AsycudaDocument where ReferenceNumber like '%January-F56%'--CNumber = 7800

select * from AsycudaDocument where ASYCUDA_Id = 29900

select * from PreviousItemsEx where Prev_reg_nbr = '52374' and ItemNumber = 'AOR/115005F'

select item_id, itemnumber, itemquantity, PiQuantity, PreviousInvoiceNumber, CNumber from asycudadocumentitem where ItemNumber = 'WAL/3002030170'  order by LineNumber


23305
9887

select * from Attachments where id between 8941 and 9845

select * from AsycudaDocumentSet where Declarant_Reference_Number like '%Import%' -- asycudadocumentsetid = 4322

select * from AsycudaDocumentSetEntryData where asycudadocumentsetid = 4323

select * from EntryDataDetailsEx where entrydataid = '5038976'