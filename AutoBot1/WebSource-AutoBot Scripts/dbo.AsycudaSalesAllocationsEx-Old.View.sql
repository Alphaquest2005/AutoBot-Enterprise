USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AsycudaSalesAllocationsEx-Old]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[AsycudaSalesAllocationsEx-Old]
AS

SELECT TOP (999999) AsycudaSalesAllocations.AllocationId, CAST(EntryDataDetails.Quantity * EntryDataDetails.Cost AS float(53)) AS TotalValue, 
                 CAST(AsycudaSalesAllocations.QtyAllocated * EntryDataDetails.Cost AS float(53)) AS AllocatedValue, AsycudaSalesAllocations.Status, CAST(AsycudaSalesAllocations.QtyAllocated AS float(53)) AS QtyAllocated, 
                 XBondItem.LineNumber AS xLineNumber, ISNULL(AsycudaSalesAllocations.PreviousItem_Id, 0) AS PreviousItem_Id, EntryData.EntryDataDate AS InvoiceDate, EntryData_Sales.CustomerName, 
                 CAST(ROUND(EntryDataDetails.Quantity, 1) AS float(53)) AS SalesQuantity, CAST(ROUND(EntryDataDetails.QtyAllocated, 1) AS float(53)) AS SalesQtyAllocated, EntryDataDetails.EntryDataId AS InvoiceNo, 
                 EntryDataDetails.LineNumber AS SalesLineNumber, EntryDataDetails.ItemNumber, EntryDataDetails.ItemDescription, EntryDataDetails.EntryDataDetailsId, ISNULL(XBondItem.Item_Id, 0) AS xBond_Item_Id, 
                 CASE WHEN isnull(EntryDataDetails.TaxAmount, isnull(EntryData_Sales.Tax, 0)) <> 0 THEN 'Duty Paid' ELSE 'Duty Free' END AS DutyFreePaid, xcuda_Registration.Number AS pCNumber, 
                 CAST(xcuda_Registration.Date AS DateTime) AS pRegistrationDate, CAST(Primary_Supplementary_Unit.Suppplementary_unit_quantity AS float(53)) AS pQuantity, 
                 CAST(pItem.DPQtyAllocated + pItem.DFQtyAllocated AS float(53)) AS pQtyAllocated, CAST(ISNULL(AscyudaItemPiQuantity.PiQuantity, 0) AS float(53)) AS PiQuantity, CAST(pItem.SalesFactor AS float(53)) AS SalesFactor, 
                 xReg.Number AS xCNumber, CAST(xReg.Date AS DateTime) AS xRegistrationDate, XBondItem_SuppUnit.Suppplementary_unit_quantity AS xQuantity, pDeclarant.Number AS pReferenceNumber, 
                 pItem.LineNumber AS pLineNumber, XBondItem.ASYCUDA_Id AS xASYCUDA_Id, xcuda_Registration.ASYCUDA_Id AS pASYCUDA_Id, CAST(EntryDataDetails.Cost AS float(53)) AS Cost, 
                 CAST(xcuda_Valuation_item.Total_CIF_itm AS float(53)) AS Total_CIF_itm, CAST(AsycudaItemDutyLiablity.DutyLiability AS float(53)) AS DutyLiability, ISNULL(EntryDataDetails.TaxAmount, ISNULL(EntryData_Sales.Tax, 
                 0)) AS TaxAmount, pItem.IsAssessed AS pIsAssessed, EntryDataDetails.DoNotAllocate AS DoNotAllocateSales, pItem.DoNotAllocate AS DoNotAllocatePreviousEntry, pItem.WarehouseError, 
                 xDeclarant.Number AS xReferenceNumber, pInventoryItems.TariffCode, TariffCodes.Invalid, DATEADD(d, CAST((CASE WHEN xcuda_warehouse.Delay = '' THEN 730 ELSE isnull(xcuda_Warehouse.Delay, 730) END) 
                 AS int), CAST(xcuda_Registration.Date AS DateTime)) AS pExpiryDate, xcuda_HScode.Commodity_code AS pTariffCode, xcuda_HScode.Precision_4 AS pItemNumber, 
                 ISNULL(xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate, CASE WHEN isnull(ismanuallyassessed, 0) = 0 THEN COALESCE (CASE WHEN dbo.xcuda_Registration.Date = '01/01/0001' THEN NULL 
                 ELSE CAST(dbo.xcuda_Registration.Date AS datetime) END, dbo.xcuda_ASYCUDA_ExtendedProperties.RegistrationDate) ELSE isnull(registrationdate, effectiveregistrationDate) END) AS AssessmentDate, 
                 EntryData.ApplicationSettingsId, AsycudaSalesAllocations.xStatus
				  ,cast(row_number() OVER (partition BY dbo.Entrydatadetails.entrydataid	ORDER BY dbo.Entrydatadetails.entrydatadetailsid) AS int) AS SANumber
FROM    xcuda_ASYCUDA_ExtendedProperties WITH (NOLOCK) INNER JOIN
                 xcuda_Registration WITH (NOLOCK) ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = xcuda_Registration.ASYCUDA_Id RIGHT OUTER JOIN
                 AsycudaItemDutyLiablity WITH (NOLOCK) RIGHT OUTER JOIN
                 TariffCodes WITH (NOLOCK) RIGHT OUTER JOIN
                     (SELECT InventoryItems.ItemNumber, InventoryItems.ApplicationSettingsId, InventoryItems.Description, InventoryItems.Category, InventoryItems.TariffCode, InventoryItems.EntryTimeStamp, InventoryItems.Id, 
                                       InventoryItems.UpgradeKey
                      FROM     InventoryItems WITH (NOLOCK) INNER JOIN
                                       InventoryItemSource WITH (NOLOCK) ON InventoryItems.Id = InventoryItemSource.InventoryId INNER JOIN
                                       InventorySources WITH (NOLOCK) ON InventoryItemSource.InventorySourceId = InventorySources.Id
                      WHERE  (InventorySources.Name = N'Asycuda')) AS pInventoryItems RIGHT OUTER JOIN
                 xcuda_HScode WITH (NOLOCK) INNER JOIN
                 xcuda_Item AS pItem WITH (NOLOCK) ON xcuda_HScode.Item_Id = pItem.Item_Id RIGHT OUTER JOIN
                 EntryData_Sales WITH (NOLOCK) INNER JOIN
                 EntryDataDetails WITH (NOLOCK) ON EntryData_Sales.EntryData_Id = EntryDataDetails.EntryData_Id INNER JOIN
                 EntryData WITH (NOLOCK) ON EntryData_Sales.EntryData_Id = EntryData.EntryData_Id INNER JOIN
                 AsycudaSalesAllocations WITH (NOLOCK) ON EntryDataDetails.EntryDataDetailsId = AsycudaSalesAllocations.EntryDataDetailsId ON pItem.Item_Id = AsycudaSalesAllocations.PreviousItem_Id LEFT OUTER JOIN
                 Primary_Supplementary_Unit AS XBondItem_SuppUnit WITH (NOLOCK) INNER JOIN
                 xcuda_Registration AS xReg WITH (NOLOCK) INNER JOIN
                 xcuda_Item AS XBondItem WITH (NOLOCK) INNER JOIN
                 xBondAllocations WITH (NOLOCK) ON XBondItem.Item_Id = xBondAllocations.xEntryItem_Id ON xReg.ASYCUDA_Id = XBondItem.ASYCUDA_Id INNER JOIN
                 xcuda_Declarant AS xDeclarant WITH (Nolock) ON XBondItem.ASYCUDA_Id = xDeclarant.ASYCUDA_Id ON XBondItem_SuppUnit.Tarification_Id = XBondItem.Item_Id ON 
                 AsycudaSalesAllocations.AllocationId = xBondAllocations.AllocationId ON pInventoryItems.ItemNumber = xcuda_HScode.Precision_4 ON TariffCodes.TariffCode = xcuda_HScode.Commodity_code LEFT OUTER JOIN
                 AscyudaItemPiQuantity WITH (NOLOCK) ON pItem.Item_Id = AscyudaItemPiQuantity.Item_Id LEFT OUTER JOIN
                 xcuda_Warehouse WITH (NOLOCK) ON pItem.ASYCUDA_Id = xcuda_Warehouse.ASYCUDA_Id LEFT OUTER JOIN
                 xcuda_Declarant AS pDeclarant WITH (NOLOCK) ON pItem.ASYCUDA_Id = pDeclarant.ASYCUDA_Id ON AsycudaItemDutyLiablity.Item_Id = pItem.Item_Id LEFT OUTER JOIN
                 xcuda_Valuation_item WITH (NOLOCK) ON pItem.Item_Id = xcuda_Valuation_item.Item_Id ON xcuda_Registration.ASYCUDA_Id = pItem.ASYCUDA_Id LEFT OUTER JOIN
                 Primary_Supplementary_Unit WITH (NOLOCK) ON pItem.Item_Id = Primary_Supplementary_Unit.Tarification_Id 
GROUP BY AsycudaSalesAllocations.AllocationId,EntryDataDetails.QtyAllocated,EntryDataDetails.Quantity, EntryDataDetails.Cost, AsycudaSalesAllocations.QtyAllocated, 
                 AsycudaSalesAllocations.Status, XBondItem.LineNumber, AsycudaSalesAllocations.PreviousItem_Id, EntryData.EntryDataDate, 
                 EntryDataDetails.EntryDataId, EntryDataDetails.ItemNumber, EntryDataDetails.ItemDescription, 
                 EntryDataDetails.EntryDataDetailsId, XBondItem.Item_Id, xcuda_Registration.Number, xcuda_Registration.Date, 
                 Primary_Supplementary_Unit.Suppplementary_unit_quantity , pItem.DPQtyAllocated , pItem.DFQtyAllocated, EntryDataDetails.TaxAmount, EntryData_Sales.Tax, xReg.Number, 
                 xReg.Date, pDeclarant.Number, EntryDataDetails.LineNumber, pItem.LineNumber, XBondItem.ASYCUDA_Id, xcuda_Registration.ASYCUDA_Id, EntryDataDetails.Cost, 
                 xcuda_Valuation_item.Total_CIF_itm, AsycudaItemDutyLiablity.DutyLiability, pItem.IsAssessed, 
                 EntryDataDetails.DoNotAllocate, pItem.DoNotAllocate, EntryDataDetails.Cost, xDeclarant.Number, pInventoryItems.TariffCode, TariffCodes.Invalid, xcuda_warehouse.Delay, xcuda_Registration.Date , xcuda_HScode.Commodity_code, 
                 xcuda_HScode.Precision_4, EntryData_Sales.CustomerName, XBondItem_SuppUnit.Suppplementary_unit_quantity, pItem.SalesFactor, pItem.WarehouseError, 
                 xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate, xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed, xcuda_Registration.Date, xcuda_ASYCUDA_ExtendedProperties.RegistrationDate, 
                 EntryData.ApplicationSettingsId, AsycudaSalesAllocations.xStatus, EntryDataDetails.LineNumber, AscyudaItemPiQuantity.PiQuantity
ORDER BY AsycudaSalesAllocations.AllocationId

--SELECT TOP (999999) AsycudaSalesAllocations.AllocationId, CAST(EntryDataDetails.Quantity * EntryDataDetails.Cost AS float(53)) AS TotalValue, 
--                 CAST(AsycudaSalesAllocations.QtyAllocated * EntryDataDetails.Cost AS float(53)) AS AllocatedValue, AsycudaSalesAllocations.Status, CAST(AsycudaSalesAllocations.QtyAllocated AS float(53)) AS QtyAllocated, 
--                 XBondItem.LineNumber AS xLineNumber, ISNULL(AsycudaSalesAllocations.PreviousItem_Id, 0) AS PreviousItem_Id, EntryData.EntryDataDate AS InvoiceDate, EntryData_Sales.CustomerName, 
--                 CAST(ROUND(EntryDataDetails.Quantity, 1) AS float(53)) AS SalesQuantity, CAST(ROUND(EntryDataDetails.QtyAllocated, 1) AS float(53)) AS SalesQtyAllocated, EntryDataDetails.EntryDataId AS InvoiceNo, 
--                 EntryDataDetails.LineNumber AS SalesLineNumber, EntryDataDetails.ItemNumber, EntryDataDetails.ItemDescription, EntryDataDetails.EntryDataDetailsId, ISNULL(XBondItem.Item_Id, 0) AS xBond_Item_Id, 
--                 CASE WHEN isnull(EntryDataDetails.TaxAmount, isnull(EntryData_Sales.Tax, 0)) <> 0 THEN 'Duty Paid' ELSE 'Duty Free' END AS DutyFreePaid, xcuda_Registration.Number AS pCNumber, 
--                 CAST(xcuda_Registration.Date AS DateTime) AS pRegistrationDate, CAST(Primary_Supplementary_Unit.Suppplementary_unit_quantity AS float(53)) AS pQuantity, 
--                 CAST(pItem.DPQtyAllocated + pItem.DFQtyAllocated AS float(53)) AS pQtyAllocated, CAST(ISNULL(AscyudaItemPiQuantity.PiQuantity, 0) AS float(53)) AS PiQuantity, CAST(pItem.SalesFactor AS float(53)) AS SalesFactor, 
--                 xReg.Number AS xCNumber, CAST(xReg.Date AS DateTime) AS xRegistrationDate, XBondItem_SuppUnit.Suppplementary_unit_quantity AS xQuantity, pDeclarant.Number AS pReferenceNumber, 
--                 pItem.LineNumber AS pLineNumber, XBondItem.ASYCUDA_Id AS xASYCUDA_Id, xcuda_Registration.ASYCUDA_Id AS pASYCUDA_Id, CAST(EntryDataDetails.Cost AS float(53)) AS Cost, 
--                 CAST(xcuda_Valuation_item.Total_CIF_itm AS float(53)) AS Total_CIF_itm, CAST(AsycudaItemDutyLiablity.DutyLiability AS float(53)) AS DutyLiability, ISNULL(EntryDataDetails.TaxAmount, ISNULL(EntryData_Sales.Tax, 
--                 0)) AS TaxAmount, pItem.IsAssessed AS pIsAssessed, EntryDataDetails.DoNotAllocate AS DoNotAllocateSales, pItem.DoNotAllocate AS DoNotAllocatePreviousEntry, pItem.WarehouseError, 
--                 xDeclarant.Number AS xReferenceNumber, pInventoryItems.TariffCode, TariffCodes.Invalid, DATEADD(d, CAST((CASE WHEN xcuda_warehouse.Delay = '' THEN 730 ELSE isnull(xcuda_Warehouse.Delay, 730) END) 
--                 AS int), CAST(xcuda_Registration.Date AS DateTime)) AS pExpiryDate, xcuda_HScode.Commodity_code AS pTariffCode, xcuda_HScode.Precision_4 AS pItemNumber, 
--                 ISNULL(xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate, CASE WHEN isnull(ismanuallyassessed, 0) = 0 THEN COALESCE (CASE WHEN dbo.xcuda_Registration.Date = '01/01/0001' THEN NULL 
--                 ELSE CAST(dbo.xcuda_Registration.Date AS datetime) END, dbo.xcuda_ASYCUDA_ExtendedProperties.RegistrationDate) ELSE isnull(registrationdate, effectiveregistrationDate) END) AS AssessmentDate, 
--                 EntryData.ApplicationSettingsId, AsycudaSalesAllocations.xStatus
--				  ,cast(row_number() OVER (partition BY dbo.Entrydatadetails.entrydataid	ORDER BY dbo.Entrydatadetails.entrydatadetailsid) AS int) AS SANumber
--FROM    xcuda_ASYCUDA_ExtendedProperties WITH (NOLOCK) INNER JOIN
--                 xcuda_Registration WITH (NOLOCK) ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = xcuda_Registration.ASYCUDA_Id RIGHT OUTER JOIN
--                 AsycudaItemDutyLiablity WITH (NOLOCK) RIGHT OUTER JOIN
--                 TariffCodes WITH (NOLOCK) RIGHT OUTER JOIN
--                     (SELECT InventoryItems.ItemNumber, InventoryItems.ApplicationSettingsId, InventoryItems.Description, InventoryItems.Category, InventoryItems.TariffCode, InventoryItems.EntryTimeStamp, InventoryItems.Id, 
--                                       InventoryItems.UpgradeKey
--                      FROM     InventoryItems WITH (NOLOCK) INNER JOIN
--                                       InventoryItemSource WITH (NOLOCK) ON InventoryItems.Id = InventoryItemSource.InventoryId INNER JOIN
--                                       InventorySources WITH (NOLOCK) ON InventoryItemSource.InventorySourceId = InventorySources.Id
--                      WHERE  (InventorySources.Name = N'Asycuda')) AS pInventoryItems RIGHT OUTER JOIN
--                 xcuda_HScode WITH (NOLOCK) INNER JOIN
--                 xcuda_Item AS pItem WITH (NOLOCK) ON xcuda_HScode.Item_Id = pItem.Item_Id RIGHT OUTER JOIN
--                 EntryData_Sales WITH (NOLOCK) INNER JOIN
--                 EntryDataDetails WITH (NOLOCK) ON EntryData_Sales.EntryData_Id = EntryDataDetails.EntryData_Id INNER JOIN
--                 EntryData WITH (NOLOCK) ON EntryData_Sales.EntryData_Id = EntryData.EntryData_Id INNER JOIN
--                 AsycudaSalesAllocations WITH (NOLOCK) ON EntryDataDetails.EntryDataDetailsId = AsycudaSalesAllocations.EntryDataDetailsId ON pItem.Item_Id = AsycudaSalesAllocations.PreviousItem_Id LEFT OUTER JOIN
--                 Primary_Supplementary_Unit AS XBondItem_SuppUnit WITH (NOLOCK) INNER JOIN
--                 xcuda_Registration AS xReg WITH (NOLOCK) INNER JOIN
--                 xcuda_Item AS XBondItem WITH (NOLOCK) INNER JOIN
--                 xBondAllocations WITH (NOLOCK) ON XBondItem.Item_Id = xBondAllocations.xEntryItem_Id ON xReg.ASYCUDA_Id = XBondItem.ASYCUDA_Id INNER JOIN
--                 xcuda_Declarant AS xDeclarant WITH (Nolock) ON XBondItem.ASYCUDA_Id = xDeclarant.ASYCUDA_Id ON XBondItem_SuppUnit.Tarification_Id = XBondItem.Item_Id ON 
--                 AsycudaSalesAllocations.AllocationId = xBondAllocations.AllocationId ON pInventoryItems.ItemNumber = xcuda_HScode.Precision_4 ON TariffCodes.TariffCode = xcuda_HScode.Commodity_code LEFT OUTER JOIN
--                 AscyudaItemPiQuantity WITH (NOLOCK) ON pItem.Item_Id = AscyudaItemPiQuantity.Item_Id LEFT OUTER JOIN
--                 xcuda_Warehouse WITH (NOLOCK) ON pItem.ASYCUDA_Id = xcuda_Warehouse.ASYCUDA_Id LEFT OUTER JOIN
--                 xcuda_Declarant AS pDeclarant WITH (NOLOCK) ON pItem.ASYCUDA_Id = pDeclarant.ASYCUDA_Id ON AsycudaItemDutyLiablity.Item_Id = pItem.Item_Id LEFT OUTER JOIN
--                 xcuda_Valuation_item WITH (NOLOCK) ON pItem.Item_Id = xcuda_Valuation_item.Item_Id ON xcuda_Registration.ASYCUDA_Id = pItem.ASYCUDA_Id LEFT OUTER JOIN
--                 Primary_Supplementary_Unit WITH (NOLOCK) ON pItem.Item_Id = Primary_Supplementary_Unit.Tarification_Id
--GROUP BY AsycudaSalesAllocations.AllocationId, CAST(EntryDataDetails.Quantity * EntryDataDetails.Cost AS float(53)), CAST(AsycudaSalesAllocations.QtyAllocated * EntryDataDetails.Cost AS float(53)), 
--                 AsycudaSalesAllocations.Status, CAST(AsycudaSalesAllocations.QtyAllocated AS float(53)), XBondItem.LineNumber, ISNULL(AsycudaSalesAllocations.PreviousItem_Id, 0), EntryData.EntryDataDate, 
--                 CAST(EntryDataDetails.Quantity AS float(53)), CAST(ROUND(EntryDataDetails.QtyAllocated, 1) AS float(53)), EntryDataDetails.EntryDataId, EntryDataDetails.ItemNumber, EntryDataDetails.ItemDescription, 
--                 EntryDataDetails.EntryDataDetailsId, ISNULL(XBondItem.Item_Id, 0), xcuda_Registration.Number, CAST(xcuda_Registration.Date AS DateTime), 
--                 CAST(Primary_Supplementary_Unit.Suppplementary_unit_quantity AS float(53)), CAST(pItem.DPQtyAllocated + pItem.DFQtyAllocated AS float(53)), EntryDataDetails.TaxAmount, EntryData_Sales.Tax, xReg.Number, 
--                 CAST(xReg.Date AS DateTime), pDeclarant.Number, EntryDataDetails.LineNumber, pItem.LineNumber, XBondItem.ASYCUDA_Id, xcuda_Registration.ASYCUDA_Id, EntryDataDetails.Cost, 
--                 CAST(xcuda_Valuation_item.Total_CIF_itm AS float(53)), CAST(AsycudaItemDutyLiablity.DutyLiability AS float(53)), CAST(ROUND(EntryDataDetails.Quantity, 1) AS float(53)), pItem.IsAssessed, 
--                 EntryDataDetails.DoNotAllocate, pItem.DoNotAllocate, CAST(EntryDataDetails.Cost AS float(53)), xDeclarant.Number, pInventoryItems.TariffCode, TariffCodes.Invalid, DATEADD(d, 
--                 CAST((CASE WHEN xcuda_warehouse.Delay = '' THEN 730 ELSE isnull(xcuda_Warehouse.Delay, 730) END) AS int), CAST(xcuda_Registration.Date AS DateTime)), xcuda_HScode.Commodity_code, 
--                 xcuda_HScode.Precision_4, EntryData_Sales.CustomerName, XBondItem_SuppUnit.Suppplementary_unit_quantity, pItem.SalesFactor, pItem.WarehouseError, 
--                 xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate, xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed, xcuda_Registration.Date, xcuda_ASYCUDA_ExtendedProperties.RegistrationDate, 
--                 EntryData.ApplicationSettingsId, AsycudaSalesAllocations.xStatus, EntryDataDetails.LineNumber, AscyudaItemPiQuantity.PiQuantity
--ORDER BY AsycudaSalesAllocations.AllocationId
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[26] 4[27] 2[26] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1[81] 4[15] 3) )"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1 [50] 2 [25] 3))"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1 [56] 3))"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2 [66] 3))"
      End
      Begin PaneConfiguration = 6
         NumPanes = 2
         Configuration = "(H (4 [50] 3))"
      End
      Begin PaneConfiguration = 7
         NumPanes = 1
         Configuration = "(V (3))"
      End
      Begin PaneConfiguration = 8
         NumPanes = 3
         Configuration = "(H (1[53] 4[21] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1[62] 4) )"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[66] 2) )"
      End
      Begin PaneConfiguration = 11
         NumPanes = 2
         Configuration = "(H (4 [60] 2))"
      End
      Begin PaneConfiguration = 12
         NumPanes = 1
         Configuration = "(H (1) )"
      End
      Begin PaneConfiguration = 13
         NumPanes = 1
         Configuration = "(V (4))"
      End
      Begin PaneConfiguration = 14
         NumPanes = 1
         Configuration = "(V (2))"
      End
      ActivePaneConfig = 0
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = -110
         Left = -576
      End
      Begin Tables = 
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 42
         Width = 284
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 2055
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1951
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 7724
         Alias = 1833
         Table = 2867
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1361
         SortOrder = 1427
         GroupBy = 1350
         Filter = 1361
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaSalesAllocationsEx-Old'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaSalesAllocationsEx-Old'
GO
