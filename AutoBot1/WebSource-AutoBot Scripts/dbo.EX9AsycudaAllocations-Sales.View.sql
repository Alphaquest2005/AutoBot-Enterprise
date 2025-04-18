USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[EX9AsycudaAllocations-Sales]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE VIEW [dbo].[EX9AsycudaAllocations-Sales]
AS
SELECT   AsycudaSalesAllocations_1.AllocationId, EntryDataDetails.EntryData_Id, CAST(EntryDataDetails.Quantity * EntryDataDetails.Cost AS float(53)) AS TotalValue, CAST(AsycudaSalesAllocations_1.QtyAllocated * EntryDataDetails.Cost AS float(53)) 
                AS AllocatedValue, AsycudaSalesAllocations_1.Status, ISNULL(CAST(AsycudaSalesAllocations_1.QtyAllocated AS float(53)), 0) AS QtyAllocated, ISNULL(AsycudaSalesAllocations_1.PreviousItem_Id, 0) AS PreviousItem_Id, 
                EntryData.EntryDataDate AS InvoiceDate, ISNULL(CAST(ROUND(EntryDataDetails.Quantity, 1) AS float(53)), 0) AS SalesQuantity, ISNULL(CAST(ROUND(EntryDataDetails.QtyAllocated, 1) AS float(53)), 0) AS SalesQtyAllocated, 
                EntryDataDetails.EntryDataId AS InvoiceNo, EntryDataDetails.ItemNumber, EntryDataDetails.ItemDescription, EntryDataDetails.EntryDataDetailsId, CASE WHEN isnull(EntryDataDetails.TaxAmount, isnull(EntryData_Sales.Tax, 0)) 
                <> 0 THEN 'Duty Paid' ELSE 'Duty Free' END AS DutyFreePaid, xcuda_Registration.Number AS pCNumber, ISNULL(CAST(xcuda_Registration.Date AS DateTime), CONVERT(DATETIME, '2001-01-01 00:00:00', 102)) AS pRegistrationDate, 
                CAST(Primary_Supplementary_Unit.Suppplementary_unit_quantity AS float(53)) AS pQuantity, CAST(pItem.DPQtyAllocated + pItem.DFQtyAllocated AS float(53)) AS pQtyAllocated, CAST(ISNULL(AscyudaItemPiQuantity.PiQuantity, 0) AS float(53)) 
                AS PiQuantity, pDeclarant.Number AS pReferenceNumber, ISNULL(pItem.LineNumber, 0) AS pLineNumber, xcuda_Registration.ASYCUDA_Id AS pASYCUDA_Id, CAST(EntryDataDetails.Cost AS float(53)) AS Cost, 
                ISNULL(CAST(xcuda_Valuation_item.Total_CIF_itm AS float(53)), 0) AS Total_CIF_itm, CAST(ISNULL(AsycudaItemDutyLiablity.DutyLiability, 0) AS float(53)) AS DutyLiability, ISNULL(EntryDataDetails.TaxAmount, ISNULL(EntryData_Sales.Tax, 0)) 
                AS TaxAmount, pItem.IsAssessed AS pIsAssessed, EntryDataDetails.DoNotAllocate AS DoNotAllocateSales, pItem.DoNotAllocate AS DoNotAllocatePreviousEntry, AsycudaSalesAllocations_1.SANumber, 
                xcuda_Goods_description.Commercial_Description, xcuda_Goods_description.Country_of_origin_code, ISNULL(xcuda_Weight_itm.Gross_weight_itm, 0) AS Gross_weight_itm, ISNULL(xcuda_Weight_itm.Net_weight_itm, 0) AS Net_weight_itm, 
                xcuda_Office_segment.Customs_clearance_office_code, xcuda_Tarification.Item_price / CAST(Primary_Supplementary_Unit.Suppplementary_unit_quantity AS float(53)) AS pItemCost, InventoryItems.TariffCode, TariffCodes.Invalid, 
                ISNULL(xcuda_ASYCUDA_ExtendedProperties.EffectiveExpiryDate, ISNULL(DATEADD(d, CAST(xcuda_Warehouse.Delay AS int), CAST(xcuda_Registration.Date AS DateTime)), CONVERT(DATETIME, '2001-01-01 00:00:00', 102))) AS pExpiryDate, 
                ISNULL(xBondAllocations.xEntryItem_Id, 0) AS xBond_Item_Id, xcuda_HScode.Commodity_code AS pTariffCode, xcuda_HScode.Precision_1 AS pPrecision1, xcuda_HScode.Precision_4 AS pItemNumber, EntryData.ApplicationSettingsId, 
                EntryDataDetails.LineNumber AS SalesLineNumber, EntryDataDetails.EffectiveDate, ISNULL(pItem.DPQtyAllocated, 0) AS DPQtyAllocated, ISNULL(pItem.DFQtyAllocated, 0) AS DFQtyAllocated, pItem.WarehouseError, ISNULL(pItem.SalesFactor, 
                0) AS SalesFactor, pItem.DoNotEX, ISNULL(ISNULL(xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate, CASE WHEN isnull(ismanuallyassessed, 0) 
                = 0 THEN COALESCE (CASE WHEN dbo.xcuda_Registration.Date = '01/01/0001' THEN NULL ELSE CAST(dbo.xcuda_Registration.Date AS datetime) END, dbo.xcuda_ASYCUDA_ExtendedProperties.RegistrationDate) ELSE isnull(registrationdate, 
                effectiveregistrationDate) END), CONVERT(DATETIME, '2001-01-01 00:00:00', 102)) AS AssessmentDate, xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed, 
                xcuda_Type.Type_of_declaration + xcuda_Type.Declaration_gen_procedure_code AS DocumentType, Customs_Procedure.CustomsOperationId, EntryData.EmailId, EntryData.FileTypeId, AsycudaSalesAllocations_1.xStatus, 
                EntryDataDetails.Comment, Customs_Procedure.Customs_ProcedureId, EntryDataDetails.InventoryItemId, EntryData.SourceFile,Customs_Procedure.CustomsProcedure 
FROM      TariffCodes RIGHT OUTER JOIN
                EntryData_Sales WITH (NOLOCK) INNER JOIN
                EntryData WITH (NOLOCK) ON EntryData_Sales.EntryData_Id = EntryData.EntryData_Id INNER JOIN
                EntryDataDetails WITH (NOLOCK) ON EntryData_Sales.EntryData_Id = EntryDataDetails.EntryData_Id INNER JOIN
                AsycudaSalesAllocations AS AsycudaSalesAllocations_1 WITH (NOLOCK) ON EntryDataDetails.EntryDataDetailsId = AsycudaSalesAllocations_1.EntryDataDetailsId INNER JOIN
                InventoryItems WITH (NOLOCK) ON EntryData.ApplicationSettingsId = InventoryItems.ApplicationSettingsId AND EntryDataDetails.InventoryItemId = InventoryItems.Id ON TariffCodes.TariffCode = InventoryItems.TariffCode LEFT OUTER JOIN
                xBondAllocations WITH (NOLOCK) ON AsycudaSalesAllocations_1.AllocationId = xBondAllocations.AllocationId LEFT OUTER JOIN
                xcuda_Valuation_item WITH (NOLOCK) INNER JOIN
                xcuda_Weight_itm WITH (NOLOCK) ON xcuda_Valuation_item.Item_Id = xcuda_Weight_itm.Valuation_item_Id RIGHT OUTER JOIN
                xcuda_HScode INNER JOIN
                xcuda_Tarification INNER JOIN
                xcuda_Office_segment INNER JOIN
                xcuda_Declarant AS pDeclarant WITH (NOLOCK) INNER JOIN
                xcuda_Item AS pItem WITH (NOLOCK) ON pDeclarant.ASYCUDA_Id = pItem.ASYCUDA_Id ON xcuda_Office_segment.ASYCUDA_Id = pItem.ASYCUDA_Id ON xcuda_Tarification.Item_Id = pItem.Item_Id INNER JOIN
                xcuda_Warehouse ON pItem.ASYCUDA_Id = xcuda_Warehouse.ASYCUDA_Id ON xcuda_HScode.Item_Id = xcuda_Tarification.Item_Id INNER JOIN
                xcuda_ASYCUDA_ExtendedProperties ON pItem.ASYCUDA_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id INNER JOIN
                Customs_Procedure ON Customs_Procedure.Customs_ProcedureId = xcuda_ASYCUDA_ExtendedProperties.Customs_ProcedureId LEFT OUTER JOIN
                [AscyudaItemPiQuantity-Basic] AS AscyudaItemPiQuantity WITH (NOLOCK) ON pItem.Item_Id = AscyudaItemPiQuantity.Item_Id LEFT OUTER JOIN
                xcuda_Goods_description WITH (NOLOCK) ON pItem.Item_Id = xcuda_Goods_description.Item_Id LEFT OUTER JOIN
                AsycudaItemDutyLiablity WITH (NOLOCK) ON pItem.Item_Id = AsycudaItemDutyLiablity.Item_Id ON xcuda_Valuation_item.Item_Id = pItem.Item_Id LEFT OUTER JOIN
                xcuda_Registration WITH (NOLOCK) INNER JOIN
                xcuda_Type ON xcuda_Registration.ASYCUDA_Id = xcuda_Type.ASYCUDA_Id ON pItem.ASYCUDA_Id = xcuda_Registration.ASYCUDA_Id LEFT OUTER JOIN
                Primary_Supplementary_Unit WITH (NOLOCK) ON pItem.Item_Id = Primary_Supplementary_Unit.Tarification_Id ON AsycudaSalesAllocations_1.PreviousItem_Id = pItem.Item_Id
WHERE   (Primary_Supplementary_Unit.Suppplementary_unit_quantity <> 0)
GROUP BY AsycudaSalesAllocations_1.AllocationId, EntryDataDetails.Quantity, AsycudaSalesAllocations_1.QtyAllocated, AsycudaSalesAllocations_1.Status, AsycudaSalesAllocations_1.PreviousItem_Id, EntryData.EntryDataDate, 
                EntryDataDetails.EntryDataId, EntryDataDetails.ItemNumber, EntryDataDetails.ItemDescription, EntryDataDetails.EntryDataDetailsId, xcuda_Registration.Number, EntryDataDetails.TaxAmount, EntryData_Sales.Tax, pDeclarant.Number, 
                xcuda_Registration.ASYCUDA_Id, EntryDataDetails.Cost, pItem.IsAssessed, EntryDataDetails.DoNotAllocate, pItem.DoNotAllocate, AsycudaSalesAllocations_1.SANumber, xcuda_Goods_description.Commercial_Description, 
                Primary_Supplementary_Unit.Suppplementary_unit_quantity, xcuda_Goods_description.Country_of_origin_code, xcuda_ASYCUDA_ExtendedProperties.RegistrationDate, xcuda_Office_segment.Customs_clearance_office_code, 
                InventoryItems.TariffCode, EntryDataDetails.QtyAllocated, xcuda_Registration.Date, TariffCodes.Invalid, pItem.LineNumber, xcuda_Valuation_item.Total_CIF_itm, AsycudaItemDutyLiablity.DutyLiability, xcuda_Weight_itm.Gross_weight_itm, 
                xcuda_Weight_itm.Net_weight_itm, xcuda_HScode.Commodity_code, xcuda_HScode.Precision_4, EntryData.ApplicationSettingsId, EntryDataDetails.LineNumber, EntryDataDetails.EffectiveDate, pItem.DPQtyAllocated, pItem.DFQtyAllocated, 
                pItem.WarehouseError, pItem.DoNotEX, xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate, xcuda_Tarification.Item_price, xcuda_Warehouse.Delay, xBondAllocations.xEntryItem_Id, pItem.SalesFactor, 
                xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed, AscyudaItemPiQuantity.PiQuantity, xcuda_Type.Type_of_declaration + xcuda_Type.Declaration_gen_procedure_code, EntryData.EmailId, EntryData.FileTypeId, 
                AsycudaSalesAllocations_1.xStatus, EntryDataDetails.EntryData_Id, EntryDataDetails.Comment, Customs_Procedure.CustomsOperationId, Customs_Procedure.Customs_ProcedureId, EntryDataDetails.InventoryItemId, InventoryItems.Id, 
                xcuda_HScode.Precision_1, xcuda_ASYCUDA_ExtendedProperties.EffectiveExpiryDate, EntryData.SourceFile,Customs_Procedure.CustomsProcedure
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[45] 4[20] 2[13] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
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
         Configuration = "(H (1[56] 4[18] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1 [75] 4))"
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
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "EntryData_Sales"
            Begin Extent = 
               Top = 585
               Left = 517
               Bottom = 822
               Right = 761
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryData"
            Begin Extent = 
               Top = 646
               Left = 69
               Bottom = 851
               Right = 307
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryDataDetails"
            Begin Extent = 
               Top = 625
               Left = 103
               Bottom = 830
               Right = 369
            End
            DisplayFlags = 280
            TopColumn = 18
         End
         Begin Table = "AsycudaSalesAllocations_1"
            Begin Extent = 
               Top = 0
               Left = 26
               Bottom = 205
               Right = 619
            End
            DisplayFlags = 280
            TopColumn = 4
         End
         Begin Table = "xcuda_Valuation_item"
            Begin Extent = 
               Top = 75
               Left = 0
               Bottom = 280
               Right = 399
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Weight_itm"
            Begin Extent = 
               Top = 251
               Left = 726
               Bottom = 429
               Right = 985
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_HScode"
            Begin Extent = 
               Top = 864
               Left = 626
               Bottom = 1069
          ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'EX9AsycudaAllocations-Sales'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'     Right = 887
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Tarification"
            Begin Extent = 
               Top = 1078
               Left = 1049
               Bottom = 1283
               Right = 1404
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Office_segment"
            Begin Extent = 
               Top = 213
               Left = 662
               Bottom = 391
               Right = 1036
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "pDeclarant"
            Begin Extent = 
               Top = 101
               Left = 1597
               Bottom = 306
               Right = 1909
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "pItem"
            Begin Extent = 
               Top = 84
               Left = 1115
               Bottom = 289
               Right = 1490
            End
            DisplayFlags = 280
            TopColumn = 10
         End
         Begin Table = "xcuda_Warehouse"
            Begin Extent = 
               Top = 572
               Left = 1073
               Bottom = 777
               Right = 1311
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_ASYCUDA_ExtendedProperties"
            Begin Extent = 
               Top = 136
               Left = 1680
               Bottom = 291
               Right = 1941
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AscyudaItemPiQuantity"
            Begin Extent = 
               Top = 0
               Left = 662
               Bottom = 183
               Right = 965
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "TariffCodes"
            Begin Extent = 
               Top = 949
               Left = 1643
               Bottom = 1154
               Right = 1944
            End
            DisplayFlags = 280
            TopColumn = 9
         End
         Begin Table = "InventoryItems"
            Begin Extent = 
               Top = 345
               Left = 1508
               Bottom = 550
               Right = 1758
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Goods_description"
            Begin Extent = 
               Top = 640
               Left = 229
               Bottom = 845
               Right = 540
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaItemDutyLiablity"
            Begin Extent = 
               Top = 214
               Left = 848
               Bottom = 365
               Right = 1086
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Registration"
            Begin Extent = 
               Top = 312
               Left = 609
               Bottom = 490
               Right = 847
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Type"
            Begin Extent = 
               Top = 296
               Left = 1539
               Bottom = 430
               Right = 1849
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Primary_Supplementary_Unit"
            Begin Ext' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'EX9AsycudaAllocations-Sales'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane3', @value=N'ent = 
               Top = 294
               Left = 458
               Bottom = 499
               Right = 818
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xBondAllocations"
            Begin Extent = 
               Top = 43
               Left = 1141
               Bottom = 213
               Right = 1406
            End
            DisplayFlags = 280
            TopColumn = 1
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 54
         Width = 284
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 2186
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 2671
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 11075
         Alias = 4752
         Table = 3024
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1348
         SortOrder = 1414
         GroupBy = 1350
         Filter = 1348
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'EX9AsycudaAllocations-Sales'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=3 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'EX9AsycudaAllocations-Sales'
GO
