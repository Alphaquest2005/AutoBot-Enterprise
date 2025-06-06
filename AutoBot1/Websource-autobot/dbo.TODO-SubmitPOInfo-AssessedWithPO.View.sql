USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-SubmitPOInfo-AssessedWithPO]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[TODO-SubmitPOInfo-AssessedWithPO]
AS



SELECT ISNULL(CAST((row_number() OVER (ORDER BY dbo.AsycudaDocumentBasicInfo.CNumber)) AS int), 0) as Id, xcuda_Registration.Number, xcuda_Registration.Date, EntryData_PurchaseOrders.PONumber, EntryData_PurchaseOrders.SupplierInvoiceNo, AsycudaDocumentBasicInfo.ApplicationSettingsId, EntryData.EmailId, 
                 EntryData.FileTypeId, AsycudaDocumentBasicInfo.ASYCUDA_Id, CASE WHEN Customs_Procedure.IsPaid = 1 THEN 'Print & Pay' ELSE '' END AS Status, Customs_Procedure.CustomsProcedure, 
                 AsycudaDocumentBasicInfo.DocumentType, AsycudaDocumentBasicInfo.CNumber, AsycudaDocumentBasicInfo.Reference, xcuda_Financial_Amounts.Totals_taxes, xcuda_Valuation.Total_CIF, 
                 WarehouseInfo.WarehouseNo, AsycudaDocumentBasicInfo.Reference + N' - C# ' + xcuda_Registration.Number + N' - WH# ' + ISNULL(WarehouseInfo.WarehouseNo, '') AS BillingLine, 
                 CAST(CASE WHEN [TODO-SubmittedPOs].ASYCUDA_Id IS NULL THEN 0 ELSE 1 END AS bit) AS IsSubmitted
FROM    AsycudaDocumentBasicInfo INNER JOIN
                 xcuda_ASYCUDA_ExtendedProperties INNER JOIN
                 Customs_Procedure ON xcuda_ASYCUDA_ExtendedProperties.Customs_ProcedureId = Customs_Procedure.Customs_ProcedureId INNER JOIN
                 xcuda_Item INNER JOIN
                 EntryData INNER JOIN
                 EntryData_PurchaseOrders ON EntryData.EntryData_Id = EntryData_PurchaseOrders.EntryData_Id ON xcuda_Item.PreviousInvoiceNumber = EntryData.EntryDataId ON 
                 xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = xcuda_Item.ASYCUDA_Id ON AsycudaDocumentBasicInfo.ASYCUDA_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id AND 
                 AsycudaDocumentBasicInfo.ApplicationSettingsId = EntryData.ApplicationSettingsId INNER JOIN
                 xcuda_Financial ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = xcuda_Financial.ASYCUDA_Id INNER JOIN
                 xcuda_Financial_Amounts ON xcuda_Financial.Financial_Id = xcuda_Financial_Amounts.Financial_Id INNER JOIN
                 xcuda_Valuation ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = xcuda_Valuation.ASYCUDA_Id INNER JOIN
                 xcuda_Registration ON xcuda_Item.ASYCUDA_Id = xcuda_Registration.ASYCUDA_Id LEFT OUTER JOIN
                     (SELECT WarehouseInfo_1.WarehouseNo, WarehouseInfo_1.Packages, EntryData_PurchaseOrders_1.PONumber, EntryData_PurchaseOrders_1.SupplierInvoiceNo
                      FROM     WarehouseInfo AS WarehouseInfo_1 INNER JOIN
                                       EntryData_PurchaseOrders AS EntryData_PurchaseOrders_1 ON WarehouseInfo_1.EntryData_Id = EntryData_PurchaseOrders_1.EntryData_Id) AS WarehouseInfo ON 
                 xcuda_Item.PreviousInvoiceNumber = WarehouseInfo.PONumber AND EntryData.EntryDataId = WarehouseInfo.PONumber LEFT OUTER JOIN
                 [TODO-SubmittedPOs] ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = [TODO-SubmittedPOs].ASYCUDA_Id
WHERE /*(AsycudaDocumentBasicInfo.Reference LIKE '%SUCR450220%') and */ AsycudaDocumentBasicInfo.ImportComplete = 1
GROUP BY xcuda_Registration.Number, xcuda_Registration.Date, EntryData_PurchaseOrders.PONumber, EntryData_PurchaseOrders.SupplierInvoiceNo, AsycudaDocumentBasicInfo.ApplicationSettingsId, 
                 AsycudaDocumentBasicInfo.AsycudaDocumentSetId, EntryData.EmailId, EntryData.FileTypeId, AsycudaDocumentBasicInfo.ASYCUDA_Id, Customs_Procedure.CustomsProcedure, 
                 AsycudaDocumentBasicInfo.DocumentType, AsycudaDocumentBasicInfo.CNumber, AsycudaDocumentBasicInfo.Reference, xcuda_Financial_Amounts.Totals_taxes, xcuda_Valuation.Total_CIF, 
                 WarehouseInfo.WarehouseNo, Customs_Procedure.IsPaid, 
                 [TODO-SubmittedPOs].ASYCUDA_Id
GO
