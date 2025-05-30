USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-SubmitDiscrepanciesToCustoms]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO










CREATE VIEW [dbo].[TODO-SubmitDiscrepanciesToCustoms]
AS
SELECT [TODO-SubmitXMLToCustoms].Id, [TODO-SubmitXMLToCustoms].CNumber, [TODO-SubmitXMLToCustoms].ASYCUDA_Id, [TODO-SubmitXMLToCustoms].RegistrationDate, 
                 [TODO-SubmitXMLToCustoms].ReferenceNumber, [TODO-SubmitXMLToCustoms].AsycudaDocumentSetId, [TODO-SubmitXMLToCustoms].DocumentType, [TODO-SubmitXMLToCustoms].AssessmentDate, 
                 [TODO-SubmitXMLToCustoms].ApplicationSettingsId, [TODO-SubmitXMLToCustoms].EmailId, [TODO-SubmitXMLToCustoms].CustomsProcedure, [TODO-SubmitXMLToCustoms].FilePath, 
                 [TODO-SubmitXMLToCustoms].Status, CASE WHEN IsPaid = 1 THEN 'Yes' ELSE 'No' END AS ToBePaid
FROM    [TODO-SubmitXMLToCustoms] INNER JOIN
                 AsycudaDocumentCustomsProcedures ON AsycudaDocumentCustomsProcedures.ASYCUDA_Id = [TODO-SubmitXMLToCustoms].ASYCUDA_Id
WHERE (AsycudaDocumentCustomsProcedures.Discrepancy = 1) AND (AsycudaDocumentCustomsProcedures.CustomsOperation <> 'Import')
GROUP BY [TODO-SubmitXMLToCustoms].Id, [TODO-SubmitXMLToCustoms].CNumber, [TODO-SubmitXMLToCustoms].ASYCUDA_Id, [TODO-SubmitXMLToCustoms].RegistrationDate, 
                 [TODO-SubmitXMLToCustoms].ReferenceNumber, [TODO-SubmitXMLToCustoms].AsycudaDocumentSetId, [TODO-SubmitXMLToCustoms].DocumentType, [TODO-SubmitXMLToCustoms].AssessmentDate, 
                 [TODO-SubmitXMLToCustoms].ApplicationSettingsId, [TODO-SubmitXMLToCustoms].EmailId, [TODO-SubmitXMLToCustoms].CustomsProcedure, [TODO-SubmitXMLToCustoms].FilePath, 
                 [TODO-SubmitXMLToCustoms].Status, ispaid
--HAVING ([TODO-SubmitXMLToCustoms].CustomsProcedure IN (N'4074-801', N'7400-OPS', N'7400-OPP'))
GO
