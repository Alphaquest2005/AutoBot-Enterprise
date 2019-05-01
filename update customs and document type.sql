UPDATE xcuda_ASYCUDA_ExtendedProperties
SET       Document_TypeId = Document_Type.Document_TypeId
FROM   xcuda_Type INNER JOIN
             Document_Type ON xcuda_Type.Type_of_declaration = Document_Type.Type_of_declaration AND 
             xcuda_Type.Declaration_gen_procedure_code = Document_Type.Declaration_gen_procedure_code INNER JOIN
             xcuda_ASYCUDA_ExtendedProperties ON xcuda_Type.ASYCUDA_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id

UPDATE xcuda_ASYCUDA_ExtendedProperties
SET       Customs_ProcedureId = xcuda_ASYCUDA_ExtendedProperties.Customs_ProcedureId
FROM   xcuda_Tarification INNER JOIN
             Customs_Procedure ON xcuda_Tarification.Extended_customs_procedure = Customs_Procedure.Extended_customs_procedure AND 
             xcuda_Tarification.National_customs_procedure = Customs_Procedure.National_customs_procedure INNER JOIN
             xcuda_Item ON xcuda_Tarification.Item_Id = xcuda_Item.Item_Id INNER JOIN
             xcuda_ASYCUDA_ExtendedProperties ON xcuda_Item.ASYCUDA_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id