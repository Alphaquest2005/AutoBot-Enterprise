USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ToDo-POToXML-EntryDataExTotals]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[ToDo-POToXML-EntryDataExTotals]
AS
SELECT EntryData.EntryData_Id, SUM(CAST(EntryDataDetails.Quantity * EntryDataDetails.Cost AS float)) AS Total, COUNT(DISTINCT EntryDataDetails.EntryDataDetailsId) AS TotalLines, 
                 AsycudaDocumentSetEntryData.AsycudaDocumentSetId
FROM    EntryDataDetails WITH (NOLOCK) INNER JOIN
                 EntryData WITH (NOLOCK) ON EntryDataDetails.EntryData_Id = EntryData.EntryData_Id INNER JOIN
                 AsycudaDocumentSetEntryData ON EntryData.EntryData_Id = AsycudaDocumentSetEntryData.EntryData_Id
GROUP BY EntryData.EntryData_Id, AsycudaDocumentSetEntryData.AsycudaDocumentSetId
GO
