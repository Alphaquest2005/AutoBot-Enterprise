USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[EntryData-SummaryView]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[EntryData-SummaryView]
AS
SELECT EntryData_Id, SUM(CAST(Quantity * Cost AS float)) AS Total, SUM(CAST(QtyAllocated * Cost AS float)) AS AllocatedTotal, COUNT(DISTINCT EntryDataDetailsId) AS TotalLines, SUM(TaxAmount) AS DetailsTax
FROM    EntryDataDetails WITH (NOLOCK)
GROUP BY EntryData_Id
GO
