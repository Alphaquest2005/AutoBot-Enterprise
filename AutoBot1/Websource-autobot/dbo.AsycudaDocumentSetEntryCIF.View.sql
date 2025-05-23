USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AsycudaDocumentSetEntryCIF]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[AsycudaDocumentSetEntryCIF]
AS
SELECT EntryDataEx.Type, EntryDataEx.InvoiceNo, EntryDataEx.SupplierInvoiceNo,ImportedTotal as Total, AsycudaDocumentSetEx.Declarant_Reference_Number, AsycudaDocumentSetEx.TotalPackages, 
                 AsycudaDocumentSetEx.AsycudaDocumentSetId, AsycudaDocumentSetEx.ApplicationSettingsId
FROM    EntryDataEx INNER JOIN
                 AsycudaDocumentSetEx ON EntryDataEx.AsycudaDocumentSetId = AsycudaDocumentSetEx.AsycudaDocumentSetId

GO
