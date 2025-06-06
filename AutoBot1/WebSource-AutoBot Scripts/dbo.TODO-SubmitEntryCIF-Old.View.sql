USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-SubmitEntryCIF-Old]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[TODO-SubmitEntryCIF-Old]
AS
SELECT EntryDataEx.Type, EntryDataEx.InvoiceNo, EntryDataEx.SupplierInvoiceNo, EntryDataEx.InvoiceTotal, WarehouseInfo.WarehouseNo, WarehouseInfo.Packages, AsycudaDocumentSetEx.Declarant_Reference_Number, 
                 AsycudaDocumentSetEx.TotalPackages, [TODO-PODocSet].AsycudaDocumentSetId, [TODO-PODocSet].ApplicationSettingsId
FROM    EntryDataEx INNER JOIN
                 WarehouseInfo ON EntryDataEx.EntryData_Id = WarehouseInfo.EntryData_Id INNER JOIN
                 AsycudaDocumentSetEx ON EntryDataEx.AsycudaDocumentSetId = AsycudaDocumentSetEx.AsycudaDocumentSetId INNER JOIN
                 [TODO-PODocSet] ON EntryDataEx.AsycudaDocumentSetId = [TODO-PODocSet].AsycudaDocumentSetId
where AsycudaDocumentSetEx.DocumentsCount = AsycudaDocumentSetEx.ExpectedEntries
GO
