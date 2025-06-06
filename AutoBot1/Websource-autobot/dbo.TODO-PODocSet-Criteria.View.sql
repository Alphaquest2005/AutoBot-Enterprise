USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-PODocSet-Criteria]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[TODO-PODocSet-Criteria]
AS

SELECT        AsycudaDocumentSetEx.AsycudaDocumentSetId
FROM            AsycudaDocumentBasicInfo INNER JOIN
                         AsycudaDocumentEntryData INNER JOIN
                         AsycudaDocumentSet AS AsycudaDocumentSetEx WITH (NOLOCK) INNER JOIN
                         AsycudaDocumentSetEntryData WITH (NOLOCK) ON AsycudaDocumentSetEx.AsycudaDocumentSetId = AsycudaDocumentSetEntryData.AsycudaDocumentSetId ON 
                         AsycudaDocumentEntryData.EntryData_Id = AsycudaDocumentSetEntryData.EntryData_Id ON AsycudaDocumentBasicInfo.ASYCUDA_Id = AsycudaDocumentEntryData.AsycudaDocumentId AND 
                         AsycudaDocumentBasicInfo.AsycudaDocumentSetId = AsycudaDocumentSetEx.AsycudaDocumentSetId INNER JOIN
                         Customs_Procedure ON AsycudaDocumentSetEx.Customs_ProcedureId = Customs_Procedure.Customs_ProcedureId INNER JOIN
                         Document_Type WITH (NOLOCK) ON Customs_Procedure.Document_TypeId = Document_Type.Document_TypeId LEFT OUTER JOIN
                         SystemDocumentSets WITH (NOLOCK) ON AsycudaDocumentSetEx.AsycudaDocumentSetId = SystemDocumentSets.Id LEFT OUTER JOIN
                         [TODO-DocumentsToDelete] ON AsycudaDocumentSetEx.AsycudaDocumentSetId = [TODO-DocumentsToDelete].AsycudaDocumentSetId
WHERE        ([TODO-DocumentsToDelete].AsycudaDocumentSetId IS NULL) AND (SystemDocumentSets.Id IS NULL) AND (AsycudaDocumentBasicInfo.ImportComplete = 0)
GROUP BY AsycudaDocumentSetEx.AsycudaDocumentSetId

--SELECT AsycudaDocumentSetEx.AsycudaDocumentSetId
--FROM    SystemDocumentSets WITH (NOLOCK) RIGHT OUTER JOIN
--                 AsycudaDocumentSetEntryData WITH (NOLOCK) INNER JOIN
--              AsycudaDocumentSet as  AsycudaDocumentSetEx WITH (NOLOCK) INNER JOIN
--                 Document_Type WITH (NOLOCK) ON AsycudaDocumentSetEx.Document_TypeId = Document_Type.Document_TypeId ON 
--                 AsycudaDocumentSetEntryData.AsycudaDocumentSetId = AsycudaDocumentSetEx.AsycudaDocumentSetId ON SystemDocumentSets.Id = AsycudaDocumentSetEx.AsycudaDocumentSetId LEFT OUTER JOIN
--                 AsycudaDocumentItemEntryDataDetails WITH (NOLOCK) ON AsycudaDocumentItemEntryDataDetails.EntryData_Id = AsycudaDocumentSetEntryData.EntryData_Id AND 
--                 AsycudaDocumentItemEntryDataDetails.AsycudaDocumentSetId = AsycudaDocumentSetEx.AsycudaDocumentSetId LEFT OUTER JOIN
--                 [TODO-DocumentsToDelete] ON AsycudaDocumentSetEx.AsycudaDocumentSetId = [TODO-DocumentsToDelete].AsycudaDocumentSetId
--WHERE (ISNULL(AsycudaDocumentItemEntryDataDetails.ImportComplete, 0) = 0) AND ([TODO-DocumentsToDelete].AsycudaDocumentSetId IS NULL) AND (SystemDocumentSets.Id IS NULL)
--GROUP BY AsycudaDocumentSetEx.AsycudaDocumentSetId
GO
