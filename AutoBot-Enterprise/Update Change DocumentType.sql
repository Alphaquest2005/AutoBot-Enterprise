UPDATE       xcuda_ASYCUDA_ExtendedProperties
SET                Document_TypeId = Document_Type.Document_TypeId, Customs_ProcedureId = Customs_Procedure.Customs_ProcedureId

FROM            Document_Type INNER JOIN
                         Customs_Procedure ON Document_Type.Document_TypeId = Customs_Procedure.Document_TypeId CROSS JOIN
                         AsycudaDocumentBasicInfo INNER JOIN
                         xcuda_ASYCUDA_ExtendedProperties ON AsycudaDocumentBasicInfo.ASYCUDA_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id
WHERE        (AsycudaDocumentBasicInfo.Reference like '%sepim7%') AND (Document_Type.Type_of_declaration = 'OS') AND (Document_Type.Declaration_gen_procedure_code = '7') AND 
                         (Customs_Procedure.Extended_customs_procedure = '7400') AND (Customs_Procedure.National_customs_procedure = 'OPS')

UPDATE   xcuda_Type    
SET    Declaration_gen_procedure_code = '7', Type_of_declaration = 'OS'            
FROM            AsycudaDocumentBasicInfo INNER JOIN
                         xcuda_Type AS xcuda_Type ON AsycudaDocumentBasicInfo.ASYCUDA_Id = xcuda_Type.ASYCUDA_Id
WHERE        (AsycudaDocumentBasicInfo.Reference like '%sepim7%')


select ReferenceNumber, DocumentType  from AsycudaDocument where ReferenceNumber like '%sepim7%'