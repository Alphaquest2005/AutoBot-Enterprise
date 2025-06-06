USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-SubmitIncompletePODocSet]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[TODO-SubmitIncompletePODocSet]
AS
SELECT        AsycudaDocumentSet.AsycudaDocumentSetId, AsycudaDocumentSet.ApplicationSettingsId, AsycudaDocumentSet.Country_of_origin_code, AsycudaDocumentSet.Currency_Code, AsycudaDocumentSet.Manifest_Number, 
                         AsycudaDocumentSet.BLNumber, Document_Type.Type_of_declaration, Document_Type.Declaration_gen_procedure_code, AsycudaDocumentSet.Declarant_Reference_Number, 
                         COUNT(DISTINCT AsycudaDocumentSetEntryData.EntryData_Id) AS Invoices, AsycudaDocumentSet.TotalInvoices
FROM            AsycudaDocumentSet INNER JOIN
                         AsycudaDocumentSetEntryData ON AsycudaDocumentSet.AsycudaDocumentSetId = AsycudaDocumentSetEntryData.AsycudaDocumentSetId INNER JOIN
                         Customs_Procedure ON AsycudaDocumentSet.Customs_ProcedureId = Customs_Procedure.Customs_ProcedureId INNER JOIN
                         Document_Type ON Customs_Procedure.Document_TypeId = Document_Type.Document_TypeId LEFT OUTER JOIN
                         [TODO-SubmitInadequatePackages] ON AsycudaDocumentSet.AsycudaDocumentSetId = [TODO-SubmitInadequatePackages].AsycudaDocumentSetId LEFT OUTER JOIN
                         AsycudaDocumentSet_Attachments ON AsycudaDocumentSet.AsycudaDocumentSetId = AsycudaDocumentSet_Attachments.AsycudaDocumentSetId LEFT OUTER JOIN
                         Attachments ON AsycudaDocumentSet_Attachments.AttachmentId = Attachments.Id
WHERE        (Document_Type.Type_of_declaration = N'IM') AND (Document_Type.Declaration_gen_procedure_code = N'7') AND (AsycudaDocumentSet.BLNumber IS NOT NULL) AND (AsycudaDocumentSet.BLNumber <> '') AND 
                         (AsycudaDocumentSet.Manifest_Number IS NOT NULL) AND (AsycudaDocumentSet.Manifest_Number <> '') AND (AsycudaDocumentSet.Currency_Code IS NOT NULL) AND (AsycudaDocumentSet.Currency_Code <> '') AND 
                         (AsycudaDocumentSet.Country_of_origin_code IS NOT NULL) AND ([TODO-SubmitInadequatePackages].RequiredPackages IS NULL) OR
                         (AsycudaDocumentSet_Attachments.FileDate = GETDATE())
GROUP BY AsycudaDocumentSet.AsycudaDocumentSetId, AsycudaDocumentSet.ApplicationSettingsId, AsycudaDocumentSet.Country_of_origin_code, AsycudaDocumentSet.Currency_Code, AsycudaDocumentSet.Manifest_Number, 
                         AsycudaDocumentSet.BLNumber, Document_Type.Type_of_declaration, Document_Type.Declaration_gen_procedure_code, AsycudaDocumentSet.Declarant_Reference_Number, AsycudaDocumentSet.TotalInvoices
HAVING        (COUNT(DISTINCT AsycudaDocumentSetEntryData.EntryData_Id) = ISNULL(AsycudaDocumentSet.TotalInvoices, 0))
GO
