USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AsycudaDocumentSetEntryDataPackages]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[AsycudaDocumentSetEntryDataPackages]
AS
SELECT AsycudaDocumentSetEntryData.AsycudaDocumentSetId, SUM( EntryData.Packages) AS Packages
FROM    EntryData INNER JOIN
                 AsycudaDocumentSetEntryData ON EntryData.EntryData_Id = AsycudaDocumentSetEntryData.EntryData_Id
GROUP BY AsycudaDocumentSetEntryData.AsycudaDocumentSetId
GO
