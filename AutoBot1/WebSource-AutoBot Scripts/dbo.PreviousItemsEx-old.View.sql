USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[PreviousItemsEx-old]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO








CREATE VIEW [dbo].[PreviousItemsEx-old]
AS
SELECT DISTINCT 
                 xcuda_PreviousItem.Packages_number, xcuda_PreviousItem.Previous_Packages_number, xcuda_PreviousItem.Hs_code, xcuda_PreviousItem.Commodity_code, xcuda_PreviousItem.Previous_item_number, 
                 xcuda_PreviousItem.Goods_origin, CAST(xcuda_PreviousItem.Net_weight AS decimal(9, 2)) AS Net_weight, CAST(xcuda_PreviousItem.Prev_net_weight AS decimal(9, 2)) AS Prev_net_weight, 
                 xcuda_PreviousItem.Prev_reg_ser, xcuda_PreviousItem.Prev_reg_nbr, xcuda_PreviousItem.Prev_reg_year, xcuda_PreviousItem.Prev_reg_cuo, xcuda_PreviousItem.Suplementary_Quantity, 
                 xcuda_PreviousItem.Preveious_suplementary_quantity, xcuda_PreviousItem.Current_value, xcuda_PreviousItem.Previous_value, xcuda_PreviousItem.Current_item_number, xcuda_PreviousItem.PreviousItem_Id, 
                 xcuda_PreviousItem.ASYCUDA_Id, xcuda_PreviousItem.QtyAllocated, pItem.Item_Id AS PreviousDocumentItemId, ROUND(xcuda_PreviousItem.Current_value, 2) AS RndCurrent_Value, 
                 xDoc.Reference AS ReferenceNumber, xDoc.CNumber, xDoc.RegistrationDate, xItem.Item_Id AS AsycudaDocumentItemId, xDoc.AssessmentDate, xcuda_PreviousItem.Prev_decl_HS_spec, 
                 CASE WHEN SalesFactor = 0 THEN 1 ELSE salesfactor END AS SalesFactor, xDoc.DocumentType, CASE WHEN documenttype = 'IM4' THEN 'Duty Paid' ELSE 'Duty Free' END AS DutyFreePaid, 
                 CASE WHEN aliasname IS NULL THEN inventoryitems.itemnumber ELSE inventoryitemaliasex.itemnumber END AS ItemNumber, CAST(xcuda_PreviousItem.Previous_item_number AS int) AS pLineNumber, 
                 xDoc.ApplicationSettingsId, ISNULL(AsycudaItemDutyLiablity.DutyLiability, 0) AS TotalDutyLiablity, 
                 ISNULL(ROUND(AsycudaItemDutyLiablity.DutyLiability * (xcuda_PreviousItem.Suplementary_Quantity / xcuda_PreviousItem.Preveious_suplementary_quantity), 4), 0) AS DutyLiablity, xdoc.Customs_ProcedureId, xdoc.CustomsProcedure
FROM    InventoryItemAliasEx RIGHT OUTER JOIN
                 AsycudaItemDutyLiablity INNER JOIN
                 AsycudaItemBasicInfo AS pItem INNER JOIN
                 InventoryItems RIGHT OUTER JOIN
                 xcuda_PreviousItem WITH (NOLOCK) INNER JOIN
                 xcuda_Item AS xItem ON xcuda_PreviousItem.PreviousItem_Id = xItem.Item_Id INNER JOIN
                 AsycudaDocumentBasicInfo AS xDoc ON xItem.ASYCUDA_Id = xDoc.ASYCUDA_Id ON InventoryItems.ApplicationSettingsId = xDoc.ApplicationSettingsId AND 
                 InventoryItems.ItemNumber = xcuda_PreviousItem.Prev_decl_HS_spec INNER JOIN
                 AsycudaDocumentCustomsProcedures ON AsycudaDocumentCustomsProcedures.ASYCUDA_Id = xDoc.ASYCUDA_Id ON pItem.ItemNumber = xcuda_PreviousItem.Prev_decl_HS_spec AND 
                 pItem.LineNumber = xcuda_PreviousItem.Previous_item_number AND pItem.CNumber = xcuda_PreviousItem.Prev_reg_nbr ON AsycudaItemDutyLiablity.Item_Id = pItem.Item_Id ON 
                 InventoryItemAliasEx.ApplicationSettingsId = InventoryItems.ApplicationSettingsId AND InventoryItemAliasEx.InventoryItemId = InventoryItems.Id
WHERE (ISNULL(xDoc.Cancelled, 0) <> 1) AND (AsycudaDocumentCustomsProcedures.CustomsOperation = 'Warehouse' OR
                 AsycudaDocumentCustomsProcedures.CustomsOperation = 'Exwarehouse')
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[44] 4[25] 2[11] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1[27] 4[49] 3) )"
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
         Left = -550
      End
      Begin Tables = 
         Begin Table = "InventoryItemAliasEx"
            Begin Extent = 
               Top = 0
               Left = 1397
               Bottom = 216
               Right = 1619
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_PreviousItem"
            Begin Extent = 
               Top = 0
               Left = 594
               Bottom = 327
               Right = 960
            End
            DisplayFlags = 280
            TopColumn = 9
         End
         Begin Table = "pItem"
            Begin Extent = 
               Top = 298
               Left = 1032
               Bottom = 453
               Right = 1323
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "pDoc"
            Begin Extent = 
               Top = 106
               Left = 2016
               Bottom = 311
               Right = 2410
            End
            DisplayFlags = 280
            TopColumn = 8
         End
         Begin Table = "InventoryItems"
            Begin Extent = 
               Top = 109
               Left = 1652
               Bottom = 264
               Right = 1845
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "DistinctEntryPreviousItems"
            Begin Extent = 
               Top = 14
               Left = 80
               Bottom = 165
               Right = 310
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 31
         Width = ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'PreviousItemsEx-old'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'284
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
         Width = 2474
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1937
         Width = 1008
         Width = 1008
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 10787
         Alias = 1492
         Table = 4006
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'PreviousItemsEx-old'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'PreviousItemsEx-old'
GO
