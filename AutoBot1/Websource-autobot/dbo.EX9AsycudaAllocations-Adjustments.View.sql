USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[EX9AsycudaAllocations-Adjustments]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


















CREATE VIEW [dbo].[EX9AsycudaAllocations-Adjustments]
AS
SELECT AsycudaSalesAllocations_1.AllocationId, EntryDataDetails.EntryData_Id, CAST(EntryDataDetails.Quantity * EntryDataDetails.Cost AS float(53)) AS TotalValue, 
                 CAST(AsycudaSalesAllocations_1.QtyAllocated * EntryDataDetails.Cost AS float(53)) AS AllocatedValue, AsycudaSalesAllocations_1.Status, ISNULL(CAST(AsycudaSalesAllocations_1.QtyAllocated AS float(53)), 0) 
                 AS QtyAllocated, ISNULL(AsycudaSalesAllocations_1.PreviousItem_Id, 0) AS PreviousItem_Id, ISNULL(EntryDataDetails.EffectiveDate, EntryData.EntryDataDate) AS InvoiceDate, 
                 ISNULL(CAST(ROUND(EntryDataDetails.Quantity, 1) AS float(53)), 0) AS SalesQuantity, ISNULL(CAST(ROUND(EntryDataDetails.QtyAllocated, 1) AS float(53)), 0) AS SalesQtyAllocated, 
                 EntryDataDetails.EntryDataId AS InvoiceNo, EntryDataDetails.ItemNumber, EntryDataDetails.ItemDescription, EntryDataDetails.EntryDataDetailsId, 
                 COALESCE (CASE WHEN AdjustmentComments.Comments IS NOT NULL THEN AdjustmentComments.DutyFreePaid ELSE NULL END, CASE WHEN isnull(EntryDataDetails.TaxAmount, isnull(EntryData_Sales.Tax, 0)) 
                 <> 0 THEN 'Duty Paid' ELSE 'Duty Free' END) AS DutyFreePaid, xcuda_Registration.Number AS pCNumber, ISNULL(CAST(xcuda_Registration.Date AS DateTime), CONVERT(DATETIME, '2001-01-01 00:00:00', 102)) 
                 AS pRegistrationDate, CAST(Primary_Supplementary_Unit.Suppplementary_unit_quantity AS float(53)) AS pQuantity, CAST(pItem.DPQtyAllocated + pItem.DFQtyAllocated AS float(53)) AS pQtyAllocated, 
                 CAST(SUM(ISNULL(AscyudaItemPiQuantity.PiQuantity, 0)) AS float(53)) AS PiQuantity, pDeclarant.Number AS pReferenceNumber, ISNULL(pItem.LineNumber, 0) AS pLineNumber, 
                 xcuda_Registration.ASYCUDA_Id AS pASYCUDA_Id, CAST(EntryDataDetails.Cost AS float(53)) AS Cost, ISNULL(CAST(xcuda_Valuation_item.Total_CIF_itm AS float(53)), 0) AS Total_CIF_itm, 
                 CAST(ISNULL(AsycudaItemDutyLiablity.DutyLiability, 0) AS float(53)) AS DutyLiability, ISNULL(EntryDataDetails.TaxAmount, ISNULL(EntryData_Sales.Tax, 0)) AS TaxAmount, pItem.IsAssessed AS pIsAssessed, 
                 EntryDataDetails.DoNotAllocate AS DoNotAllocateSales, pItem.DoNotAllocate AS DoNotAllocatePreviousEntry, AsycudaSalesAllocations_1.SANumber, xcuda_Goods_description.Commercial_Description, 
                 xcuda_Goods_description.Country_of_origin_code, ISNULL(xcuda_Weight_itm.Gross_weight_itm, 0) AS Gross_weight_itm, ISNULL(xcuda_Weight_itm.Net_weight_itm, 0) AS Net_weight_itm, 
                 xcuda_Office_segment.Customs_clearance_office_code, xcuda_Tarification.Item_price / CAST(Primary_Supplementary_Unit.Suppplementary_unit_quantity AS float(53)) AS pItemCost, InventoryItems.TariffCode, 
                 TariffCodes.Invalid, isnull(effectiveExpiryDate, ISNULL(DATEADD(d, CAST(xcuda_Warehouse.Delay AS int), CAST(xcuda_Registration.Date AS DateTime)), CONVERT(DATETIME, '2001-01-01 00:00:00', 102))) AS pExpiryDate, 
                 ISNULL(xBondAllocations.xEntryItem_Id, 0) AS xBond_Item_Id, xcuda_HScode.Commodity_code AS pTariffCode, xcuda_HScode.Precision_1 AS pPrecision1, xcuda_HScode.Precision_4 AS pItemNumber, 
                 EntryData.ApplicationSettingsId, EntryDataDetails.LineNumber AS SalesLineNumber, EntryDataDetails.EffectiveDate, ISNULL(pItem.DPQtyAllocated, 0) AS DPQtyAllocated, ISNULL(pItem.DFQtyAllocated, 0) 
                 AS DFQtyAllocated, pItem.WarehouseError, ISNULL(pItem.SalesFactor, 0) AS SalesFactor, pItem.DoNotEX, ISNULL(ISNULL(xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate, 
                 CASE WHEN isnull(ismanuallyassessed, 0) = 0 THEN COALESCE (CASE WHEN dbo.xcuda_Registration.Date = '01/01/0001' THEN NULL ELSE CAST(dbo.xcuda_Registration.Date AS datetime) END, 
                 dbo.xcuda_ASYCUDA_ExtendedProperties.RegistrationDate) ELSE isnull(registrationdate, effectiveregistrationDate) END), CONVERT(DATETIME, '2001-01-01 00:00:00', 102)) AS AssessmentDate, 
                 xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed, xcuda_Type.Type_of_declaration + xcuda_Type.Declaration_gen_procedure_code AS DocumentType, Customs_Procedure.CustomsOperationId, 
                 EntryData.EmailId, EntryData.FileTypeId, AsycudaSalesAllocations_1.xStatus, EntryDataDetails.Comment, Customs_Procedure.Customs_ProcedureId, EntryDataDetails.InventoryItemId, EntryData.SourceFile,Customs_Procedure.CustomsProcedure 
FROM    Primary_Supplementary_Unit WITH (NOLOCK) RIGHT OUTER JOIN
                 xcuda_Valuation_item WITH (NOLOCK) INNER JOIN
                 xcuda_Weight_itm WITH (NOLOCK) ON xcuda_Valuation_item.Item_Id = xcuda_Weight_itm.Valuation_item_Id RIGHT OUTER JOIN
                 [AscyudaItemPiQuantity-Basic] AS AscyudaItemPiQuantity WITH (NOLOCK) RIGHT OUTER JOIN
                 xcuda_HScode INNER JOIN
                 xcuda_Tarification INNER JOIN
                 xcuda_Office_segment INNER JOIN
                 xcuda_Declarant AS pDeclarant WITH (NOLOCK) INNER JOIN
                 xcuda_Item AS pItem WITH (NOLOCK) ON pDeclarant.ASYCUDA_Id = pItem.ASYCUDA_Id ON xcuda_Office_segment.ASYCUDA_Id = pItem.ASYCUDA_Id ON xcuda_Tarification.Item_Id = pItem.Item_Id INNER JOIN
                 xcuda_Warehouse ON pItem.ASYCUDA_Id = xcuda_Warehouse.ASYCUDA_Id ON xcuda_HScode.Item_Id = xcuda_Tarification.Item_Id INNER JOIN
                 xcuda_ASYCUDA_ExtendedProperties ON pItem.ASYCUDA_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id INNER JOIN
                 Customs_Procedure ON Customs_Procedure.Customs_ProcedureId = xcuda_ASYCUDA_ExtendedProperties.Customs_ProcedureId RIGHT OUTER JOIN
                 TariffCodes RIGHT OUTER JOIN
                     (SELECT Type, EntryData_Id, Tax
                      FROM     EntryData_Adjustments
                      WHERE  (Type = 'ADJ')) AS EntryData_Sales INNER JOIN
                 EntryData WITH (NOLOCK) ON EntryData_Sales.EntryData_Id = EntryData.EntryData_Id INNER JOIN
                 EntryDataDetails WITH (NOLOCK) ON EntryData_Sales.EntryData_Id = EntryDataDetails.EntryData_Id INNER JOIN
                 AsycudaSalesAllocations AS AsycudaSalesAllocations_1 WITH (NOLOCK) ON EntryDataDetails.EntryDataDetailsId = AsycudaSalesAllocations_1.EntryDataDetailsId INNER JOIN
                 InventoryItems WITH (NOLOCK) ON EntryData.ApplicationSettingsId = InventoryItems.ApplicationSettingsId AND EntryDataDetails.InventoryItemId = InventoryItems.Id ON 
                 TariffCodes.TariffCode = InventoryItems.TariffCode ON pItem.Item_Id = AsycudaSalesAllocations_1.PreviousItem_Id LEFT OUTER JOIN
                 xBondAllocations WITH (NOLOCK) ON AsycudaSalesAllocations_1.AllocationId = xBondAllocations.AllocationId LEFT OUTER JOIN
                 AdjustmentComments ON LOWER(EntryDataDetails.Comment) = LOWER(AdjustmentComments.Comments) ON AscyudaItemPiQuantity.Item_Id = pItem.Item_Id LEFT OUTER JOIN
                 xcuda_Goods_description WITH (NOLOCK) ON pItem.Item_Id = xcuda_Goods_description.Item_Id LEFT OUTER JOIN
                 AsycudaItemDutyLiablity WITH (NOLOCK) ON pItem.Item_Id = AsycudaItemDutyLiablity.Item_Id ON xcuda_Valuation_item.Item_Id = pItem.Item_Id LEFT OUTER JOIN
                 xcuda_Registration WITH (NOLOCK) INNER JOIN
                 xcuda_Type ON xcuda_Registration.ASYCUDA_Id = xcuda_Type.ASYCUDA_Id ON pItem.ASYCUDA_Id = xcuda_Registration.ASYCUDA_Id ON Primary_Supplementary_Unit.Tarification_Id = pItem.Item_Id
where Primary_Supplementary_Unit.Suppplementary_unit_quantity <> 0
GROUP BY AsycudaSalesAllocations_1.AllocationId, EntryDataDetails.Quantity, AsycudaSalesAllocations_1.QtyAllocated, AsycudaSalesAllocations_1.Status, AsycudaSalesAllocations_1.PreviousItem_Id, 
                 EntryData.EntryDataDate, EntryDataDetails.EntryDataId, EntryDataDetails.ItemNumber, EntryDataDetails.ItemDescription, EntryDataDetails.EntryDataDetailsId, xcuda_Registration.Number, 
                 EntryDataDetails.TaxAmount, EntryData_Sales.Tax, pDeclarant.Number, xcuda_Registration.ASYCUDA_Id, EntryDataDetails.Cost, pItem.IsAssessed, EntryDataDetails.DoNotAllocate, pItem.DoNotAllocate, 
                 AsycudaSalesAllocations_1.SANumber, xcuda_Goods_description.Commercial_Description, Primary_Supplementary_Unit.Suppplementary_unit_quantity, xcuda_Goods_description.Country_of_origin_code, 
                 xcuda_ASYCUDA_ExtendedProperties.RegistrationDate, xcuda_Office_segment.Customs_clearance_office_code, InventoryItems.TariffCode, EntryDataDetails.QtyAllocated, xcuda_Registration.Date, 
                 TariffCodes.Invalid, pItem.LineNumber, xcuda_Valuation_item.Total_CIF_itm, AsycudaItemDutyLiablity.DutyLiability, xcuda_Weight_itm.Gross_weight_itm, xcuda_Weight_itm.Net_weight_itm, 
                 xcuda_HScode.Commodity_code, xcuda_HScode.Precision_4, EntryData.ApplicationSettingsId, EntryDataDetails.LineNumber, EntryDataDetails.EffectiveDate, pItem.DPQtyAllocated, pItem.DFQtyAllocated, 
                 pItem.WarehouseError, pItem.DoNotEX, xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate, xcuda_Tarification.Item_price, xcuda_Warehouse.Delay, xBondAllocations.xEntryItem_Id, pItem.SalesFactor, 
                 xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed, xcuda_Type.Type_of_declaration + xcuda_Type.Declaration_gen_procedure_code, EntryData.EmailId, EntryData.FileTypeId, 
                 AsycudaSalesAllocations_1.xStatus, EntryDataDetails.EntryData_Id, EntryDataDetails.Comment, Customs_Procedure.CustomsOperationId, Customs_Procedure.Customs_ProcedureId, 
                 EntryDataDetails.InventoryItemId, AdjustmentComments.Comments, AdjustmentComments.DutyFreePaid, xcuda_HScode.Precision_1, EffectiveExpiryDate, EntryData.SourceFile,Customs_Procedure.CustomsProcedure
GO
