USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-C71FromDocuments-Old]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE VIEW [dbo].[TODO-C71FromDocuments-Old]
AS
SELECT dbo.xcuda_Delivery_terms.Code, dbo.AsycudaDocumentBasicInfo.ASYCUDA_Id, dbo.AsycudaDocumentBasicInfo.ApplicationSettingsId, ISNULL(dbo.EntryDataEx.InvoiceTotal, 0) AS InvoiceTotal, dbo.EntryDataEx.Type, 
                 dbo.EntryDataEx.InvoiceDate, dbo.EntryDataEx.InvoiceNo, ISNULL(dbo.EntryDataEx.Currency, dbo.[TODO-PODocSet].Currency_Code) AS Currency, dbo.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId, 
                 dbo.EntryDataEx.SupplierCode, dbo.[TODO-PODocSet].CurrencyRate
FROM    dbo.xcuda_Delivery_terms INNER JOIN
                 dbo.xcuda_Transport ON dbo.xcuda_Delivery_terms.Transport_Id = dbo.xcuda_Transport.Transport_Id INNER JOIN
                 dbo.AsycudaDocumentBasicInfo ON dbo.xcuda_Transport.ASYCUDA_Id = dbo.AsycudaDocumentBasicInfo.ASYCUDA_Id INNER JOIN
                 dbo.xcuda_ASYCUDA_ExtendedProperties ON dbo.AsycudaDocumentBasicInfo.ASYCUDA_Id = dbo.xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id INNER JOIN
                 dbo.[TODO-PODocSet] ON dbo.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId = dbo.[TODO-PODocSet].AsycudaDocumentSetId INNER JOIN
                 dbo.AsycudaDocumentEntryData ON dbo.AsycudaDocumentBasicInfo.ASYCUDA_Id = dbo.AsycudaDocumentEntryData.AsycudaDocumentId INNER JOIN
                 dbo.EntryDataEx ON dbo.AsycudaDocumentEntryData.EntryData_Id = dbo.EntryDataEx.EntryData_Id AND dbo.AsycudaDocumentBasicInfo.ApplicationSettingsId = dbo.EntryDataEx.ApplicationSettingsId
WHERE (dbo.AsycudaDocumentBasicInfo.ImportComplete = 0)
GROUP BY dbo.xcuda_Delivery_terms.Code, dbo.AsycudaDocumentBasicInfo.ASYCUDA_Id, dbo.AsycudaDocumentBasicInfo.ApplicationSettingsId, dbo.EntryDataEx.InvoiceTotal, dbo.EntryDataEx.Type, 
                 dbo.EntryDataEx.InvoiceDate, dbo.EntryDataEx.InvoiceNo, dbo.EntryDataEx.Currency, dbo.[TODO-PODocSet].Currency_Code, dbo.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId, 
                 dbo.EntryDataEx.SupplierCode, dbo.[TODO-PODocSet].CurrencyRate
GO
