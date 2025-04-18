USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-SubmitPOInfo]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO









CREATE VIEW [dbo].[TODO-SubmitPOInfo]
AS
SELECT     ISNULL(CAST((row_number() OVER (ORDER BY dbo.AsycudaDocumentBasicInfo.CNumber)) AS int), 0) as Id,   xcuda_Registration.Number, xcuda_Registration.Date, EntryData_PurchaseOrders.PONumber, EntryData_PurchaseOrders.SupplierInvoiceNo, AsycudaDocumentBasicInfo.ApplicationSettingsId, EntryData.EmailId, 
                         EntryData.FileTypeId, AsycudaDocumentBasicInfo.ASYCUDA_Id, CASE WHEN Customs_Procedure.IsPaid = 1 THEN 'Print & Pay' ELSE '' END AS Status, Customs_Procedure.CustomsProcedure, 
                         AsycudaDocumentBasicInfo.DocumentType, AsycudaDocumentBasicInfo.CNumber, AsycudaDocumentBasicInfo.Reference, xcuda_Financial_Amounts.Totals_taxes, xcuda_Valuation.Total_CIF, 
                         ISNULL(WarehouseInfo.WarehouseNo, xcuda_Packages.Marks2_of_packages) AS WarehouseNo, 
                         AsycudaDocumentBasicInfo.Reference + N' - C# ' + xcuda_Registration.Number + N' - WH# ' + ISNULL(WarehouseInfo.WarehouseNo, xcuda_Packages.Marks2_of_packages) AS BillingLine, 
                         CAST(CASE WHEN [TODO-SubmittedPOs].ASYCUDA_Id IS NULL THEN 0 ELSE 1 END AS bit) AS IsSubmitted, xcuda_Packages.Marks2_of_packages, AsycudaDocumentBasicInfo.AsycudaDocumentSetId, 
                         EntryData_PurchaseOrders.EntryData_Id
FROM            xcuda_Financial_Amounts INNER JOIN
                         xcuda_Financial INNER JOIN
                         AsycudaDocumentBasicInfo INNER JOIN
                         xcuda_ASYCUDA_ExtendedProperties INNER JOIN
                             (SELECT        MIN(Item_Id) AS Item_id, ASYCUDA_Id, PreviousInvoiceNumber
                               FROM            xcuda_Item AS xcuda_Item_1
                               GROUP BY ASYCUDA_Id, PreviousInvoiceNumber) AS xcuda_Item ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = xcuda_Item.ASYCUDA_Id ON 
                         AsycudaDocumentBasicInfo.ASYCUDA_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id ON xcuda_Financial.ASYCUDA_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id ON 
                         xcuda_Financial_Amounts.Financial_Id = xcuda_Financial.Financial_Id INNER JOIN
                         xcuda_Valuation ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = xcuda_Valuation.ASYCUDA_Id INNER JOIN
                         xcuda_Registration ON xcuda_Item.ASYCUDA_Id = xcuda_Registration.ASYCUDA_Id INNER JOIN
                         xcuda_Packages ON xcuda_Item.Item_id = xcuda_Packages.Item_Id INNER JOIN
                         Attachments ON Attachments.FilePath = xcuda_ASYCUDA_ExtendedProperties.SourceFileName LEFT OUTER JOIN
                         Customs_Procedure ON xcuda_ASYCUDA_ExtendedProperties.Customs_ProcedureId = Customs_Procedure.Customs_ProcedureId LEFT OUTER JOIN
                         EntryData INNER JOIN
                         EntryData_PurchaseOrders ON EntryData.EntryData_Id = EntryData_PurchaseOrders.EntryData_Id ON AsycudaDocumentBasicInfo.ApplicationSettingsId = EntryData.ApplicationSettingsId AND 
                         xcuda_Item.PreviousInvoiceNumber = EntryData.EntryDataId LEFT OUTER JOIN
                         WarehouseInfo ON WarehouseInfo.WarehouseNo LIKE '%' + xcuda_Packages.Marks2_of_packages + '%' AND EntryData_PurchaseOrders.EntryData_Id = WarehouseInfo.EntryData_Id LEFT OUTER JOIN
                         [TODO-SubmittedPOs] ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = [TODO-SubmittedPOs].ASYCUDA_Id
WHERE        (Customs_Procedure.SubmitToCustoms = 1)
GROUP BY xcuda_Registration.Number, xcuda_Registration.Date, EntryData_PurchaseOrders.PONumber, EntryData_PurchaseOrders.SupplierInvoiceNo, AsycudaDocumentBasicInfo.ApplicationSettingsId, 
                         AsycudaDocumentBasicInfo.AsycudaDocumentSetId, EntryData.EmailId, EntryData.FileTypeId, AsycudaDocumentBasicInfo.ASYCUDA_Id, Customs_Procedure.CustomsProcedure, AsycudaDocumentBasicInfo.DocumentType, 
                         AsycudaDocumentBasicInfo.CNumber, AsycudaDocumentBasicInfo.Reference, xcuda_Financial_Amounts.Totals_taxes, xcuda_Valuation.Total_CIF, WarehouseInfo.WarehouseNo, Customs_Procedure.IsPaid, 
                         [TODO-SubmittedPOs].ASYCUDA_Id, xcuda_Packages.Marks2_of_packages, AsycudaDocumentBasicInfo.AsycudaDocumentSetId, EntryData_PurchaseOrders.EntryData_Id
GO
