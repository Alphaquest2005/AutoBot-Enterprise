USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-SubmitSalesToCustoms]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[TODO-SubmitSalesToCustoms]
AS
/*HAVING ([TODO-SubmitXMLToCustoms].CustomsProcedure IN (N'4074-801', N'7400-OPS', N'7400-OPP'))*/
SELECT [TODO-SubmitAllXMLToCustoms].Id, [TODO-SubmitAllXMLToCustoms].CNumber, [TODO-SubmitAllXMLToCustoms].ASYCUDA_Id, [TODO-SubmitAllXMLToCustoms].RegistrationDate, 
                 [TODO-SubmitAllXMLToCustoms].ReferenceNumber, [TODO-SubmitAllXMLToCustoms].AsycudaDocumentSetId, [TODO-SubmitAllXMLToCustoms].DocumentType, [TODO-SubmitAllXMLToCustoms].AssessmentDate, 
                 [TODO-SubmitAllXMLToCustoms].ApplicationSettingsId, [TODO-SubmitAllXMLToCustoms].EmailId, [TODO-SubmitAllXMLToCustoms].CustomsProcedure, [TODO-SubmitAllXMLToCustoms].FilePath, 
                 [TODO-SubmitAllXMLToCustoms].Status, ToBePaid
FROM    [TODO-SubmitAllXMLToCustoms] INNER JOIN
                 AsycudaDocumentCustomsProcedures ON AsycudaDocumentCustomsProcedures.ASYCUDA_Id = [TODO-SubmitAllXMLToCustoms].ASYCUDA_Id INNER JOIN
                 [TODO-ImportCompleteEntries] ON [TODO-SubmitAllXMLToCustoms].ASYCUDA_Id = [TODO-ImportCompleteEntries].AssessedAsycuda_Id
WHERE (AsycudaDocumentCustomsProcedures.Sales = 1) AND (AsycudaDocumentCustomsProcedures.CustomsOperation = 'Exwarehouse')
GROUP BY [TODO-SubmitAllXMLToCustoms].Id, [TODO-SubmitAllXMLToCustoms].CNumber, [TODO-SubmitAllXMLToCustoms].ASYCUDA_Id, [TODO-SubmitAllXMLToCustoms].RegistrationDate, 
                 [TODO-SubmitAllXMLToCustoms].ReferenceNumber, [TODO-SubmitAllXMLToCustoms].AsycudaDocumentSetId, [TODO-SubmitAllXMLToCustoms].DocumentType, [TODO-SubmitAllXMLToCustoms].AssessmentDate, 
                 [TODO-SubmitAllXMLToCustoms].ApplicationSettingsId, [TODO-SubmitAllXMLToCustoms].EmailId, [TODO-SubmitAllXMLToCustoms].CustomsProcedure, [TODO-SubmitAllXMLToCustoms].FilePath, 
                 [TODO-SubmitAllXMLToCustoms].Status, ToBePaid
GO
