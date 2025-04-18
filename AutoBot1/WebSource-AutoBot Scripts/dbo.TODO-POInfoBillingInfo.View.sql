USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-POInfoBillingInfo]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[TODO-POInfoBillingInfo]
AS
SELECT ISNULL(CAST((row_number() OVER (ORDER BY dbo.AsycudaDocumentBasicInfo.CNumber)) AS int), 0) as Id, xcuda_Registration.Number, xcuda_Registration.Date, [TODO-ImportCompleteEntries].EntryDataId, EntryData_PurchaseOrders.SupplierInvoiceNo, [TODO-ImportCompleteEntries].ApplicationSettingsId, 
                 [TODO-ImportCompleteEntries].AsycudaDocumentSetId, [TODO-ImportCompleteEntries].EmailId, [TODO-ImportCompleteEntries].FileTypeId, [TODO-ImportCompleteEntries].NewAsycuda_Id, 
                 [TODO-ImportCompleteEntries].AssessedAsycuda_Id, CASE WHEN Customs_Procedure.IsPaid = 1 THEN 'Print & Pay' ELSE '' END AS Status, Customs_Procedure.CustomsProcedure, 
                 AsycudaDocumentBasicInfo.DocumentType, AsycudaDocumentBasicInfo.CNumber, AsycudaDocumentBasicInfo.Reference, xcuda_Financial_Amounts.Totals_taxes, xcuda_Valuation.Total_CIF, 
                 WarehouseNo, AsycudaDocumentBasicInfo.Reference + N' - C# ' + xcuda_Registration.Number + N' - WH# '  + isnull(WarehouseInfo.WarehouseNo,'') AS BillingLine
FROM    xcuda_ASYCUDA_ExtendedProperties INNER JOIN
                 Customs_Procedure ON xcuda_ASYCUDA_ExtendedProperties.Customs_ProcedureId = Customs_Procedure.Customs_ProcedureId INNER JOIN
                 EntryData INNER JOIN
                 EntryData_PurchaseOrders ON EntryData.EntryData_Id = EntryData_PurchaseOrders.EntryData_Id INNER JOIN
                 [TODO-ImportCompleteEntries] INNER JOIN
                 xcuda_Registration ON [TODO-ImportCompleteEntries].AssessedAsycuda_Id = xcuda_Registration.ASYCUDA_Id ON EntryData.EntryDataId = [TODO-ImportCompleteEntries].EntryDataId ON 
                 xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = [TODO-ImportCompleteEntries].AssessedAsycuda_Id INNER JOIN
                 AsycudaDocumentBasicInfo ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = AsycudaDocumentBasicInfo.ASYCUDA_Id INNER JOIN
                 xcuda_Financial ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = xcuda_Financial.ASYCUDA_Id INNER JOIN
                 xcuda_Financial_Amounts ON xcuda_Financial.Financial_Id = xcuda_Financial_Amounts.Financial_Id INNER JOIN
                 xcuda_Valuation ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = xcuda_Valuation.ASYCUDA_Id LEFT OUTER JOIN
                 WarehouseInfo ON EntryData_PurchaseOrders.EntryData_Id = WarehouseInfo.EntryData_Id LEFT OUTER JOIN
                 [TODO-SubmittedPOs] ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = [TODO-SubmittedPOs].ASYCUDA_Id

GROUP BY xcuda_Registration.Number, xcuda_Registration.Date, [TODO-ImportCompleteEntries].EntryDataId, EntryData_PurchaseOrders.SupplierInvoiceNo, [TODO-ImportCompleteEntries].ApplicationSettingsId, 
                 [TODO-ImportCompleteEntries].AsycudaDocumentSetId, [TODO-ImportCompleteEntries].EmailId, [TODO-ImportCompleteEntries].FileTypeId, [TODO-ImportCompleteEntries].NewAsycuda_Id, 
                 [TODO-ImportCompleteEntries].AssessedAsycuda_Id, Customs_Procedure.CustomsProcedure, AsycudaDocumentBasicInfo.DocumentType, AsycudaDocumentBasicInfo.CNumber, 
                 AsycudaDocumentBasicInfo.Reference, xcuda_Financial_Amounts.Totals_taxes, xcuda_Valuation.Total_CIF, WarehouseNo, 
                 AsycudaDocumentBasicInfo.Reference + N' - C# ' + xcuda_ASYCUDA_ExtendedProperties.CNumber + N' - WH# ' + WarehouseNo, Customs_Procedure.IsPaid
GO
