USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[Stp_TODO_ImportCompleteEntries-EntryDataDetailsEx]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[Stp_TODO_ImportCompleteEntries-EntryDataDetailsEx]
AS
SELECT dbo.EntryDataDetails.EntryDataDetailsId, dbo.EntryDataDetails.EntryData_Id, dbo.EntryDataDetails.EntryDataId, 
                  ISNULL(dbo.AsycudaDocumentSetEntryData.AsycudaDocumentSetId, 0) AS AsycudaDocumentSetId, 
                 dbo.EntryData.ApplicationSettingsId, dbo.EntryData.EmailId, dbo.EntryData.FileTypeId
FROM    dbo.AsycudaDocumentSetEntryData AS AsycudaDocumentSetEntryData_1 WITH (NOLOCK) INNER JOIN
                 dbo.AsycudaDocumentSetEntryData INNER JOIN
                 dbo.EntryData WITH (NOLOCK) ON dbo.AsycudaDocumentSetEntryData.EntryData_Id = dbo.EntryData.EntryData_Id ON AsycudaDocumentSetEntryData_1.EntryData_Id = dbo.EntryData.EntryData_Id AND 
                 AsycudaDocumentSetEntryData_1.AsycudaDocumentSetId = dbo.AsycudaDocumentSetEntryData.AsycudaDocumentSetId RIGHT OUTER JOIN
                 dbo.EntryDataDetails WITH (NOLOCK) ON dbo.EntryData.EntryData_Id = dbo.EntryDataDetails.EntryData_Id LEFT OUTER JOIN
                 dbo.EntryData_Sales WITH (NOLOCK) ON dbo.EntryData.EntryData_Id = dbo.EntryData_Sales.EntryData_Id
GO
