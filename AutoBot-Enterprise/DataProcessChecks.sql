
--------------------------Data Check
select 'Data Check'

SELECT        AsycudaDocumentBasicInfo.DocumentType, AsycudaDocumentBasicInfo.CNumber, AsycudaDocumentBasicInfo.RegistrationDate, AsycudaDocumentBasicInfo.AssessmentDate, [DataCheck-AllErrors].Lines, 
                         [DataCheck-AllErrors].Type
FROM            [DataCheck-AllErrors] INNER JOIN
                         AsycudaDocumentBasicInfo ON [DataCheck-AllErrors].Asycuda_id = AsycudaDocumentBasicInfo.ASYCUDA_Id

--------------------------Unmapped Items
select 'Un Mapped Asycuda Items'
SELECT        ItemNumber, Commercial_Description, CNumber, LineNumber, AssessmentDate, RegistrationDate, DocumentType
FROM            [DataProcessCheck-UnMappedAsycudaItems]

select 'Un known Asycuda Items'
SELECT        Precision_4, Commercial_Description, CNumber, LineNumber, DocumentType, RegistrationDate
FROM            [DataProcessCheck-Unknown Asycuda Items]

select 'Un Mapped POS Items'
SELECT        ItemNumber, ItemDescription, EntryDataId, EntryDataDate
FROM            [DataProcessCheck-Unknown EntryData Items]

-------------------------Expired Entries----------------------
SELECT        DocumentType, CNumber, Extended_customs_procedure, RegistrationDate, AssessmentDate, ExpiryDate
FROM            AsycudaDocumentBasicInfo
WHERE        (ExpiryDate <= DATEADD(m, 3, GETDATE()))

-------------------------Warehouse Error Codes-----------------


