USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AsycudaDocumentSetEntryDataCIF]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[AsycudaDocumentSetEntryDataCIF]
AS
SELECT AsycudaDocumentSetEntryData.AsycudaDocumentSetId, SUM(EntryData.InvoiceTotal) AS InvoiceTotal, Count(EntryData.InvoiceTotal) AS ImportedInvoices
FROM    EntryData INNER JOIN
                 AsycudaDocumentSetEntryData ON EntryData.EntryData_Id = AsycudaDocumentSetEntryData.EntryData_Id
GROUP BY AsycudaDocumentSetEntryData.AsycudaDocumentSetId
GO
