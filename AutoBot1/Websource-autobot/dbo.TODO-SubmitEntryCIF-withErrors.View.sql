USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-SubmitEntryCIF-withErrors]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[TODO-SubmitEntryCIF-withErrors]
AS
SELECT EntryDataEx.Type, EntryDataEx.InvoiceNo, EntryDataEx.SupplierInvoiceNo, EntryDataEx.InvoiceTotal, WarehouseInfo.WarehouseNo, WarehouseInfo.Packages, AsycudaDocumentSetEx.Declarant_Reference_Number, 
                 AsycudaDocumentSetEx.TotalPackages, [TODO-PODocSet].AsycudaDocumentSetId, [TODO-PODocSet].ApplicationSettingsId
FROM    EntryDataEx INNER JOIN
                 WarehouseInfo ON EntryDataEx.EntryData_Id = WarehouseInfo.EntryData_Id INNER JOIN
                 AsycudaDocumentSetEx ON EntryDataEx.AsycudaDocumentSetId = AsycudaDocumentSetEx.AsycudaDocumentSetId INNER JOIN
                 [TODO-PODocSet] ON EntryDataEx.AsycudaDocumentSetId = [TODO-PODocSet].AsycudaDocumentSetId
--where AsycudaDocumentSetEx.DocumentsCount = AsycudaDocumentSetEx.ExpectedEntries
GO
