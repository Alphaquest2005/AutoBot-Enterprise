USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AllocationsPreviousItem]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[AllocationsPreviousItem]
AS
SELECT dbo.xcuda_Item.Item_Id, dbo.xcuda_Supplementary_unit.Suppplementary_unit_quantity AS ItemQuantity, dbo.xcuda_Tarification.Item_price, dbo.xcuda_HScode.Precision_4, dbo.InventoryItems.TariffCode, 
                 dbo.xcuda_Valuation_item.Total_CIF_itm, dbo.AsycudaItemDutyLiablity.DutyLiability, SUM(dbo.[AscyudaItemPiQuantity-Basic].PiQuantity) AS PiQuantity, dbo.xcuda_Goods_description.Country_of_origin_code, 
                 dbo.xcuda_Item_Invoice.Amount_national_currency, dbo.xcuda_Weight_itm.Gross_weight_itm, dbo.xcuda_Weight_itm.Net_weight_itm
FROM    dbo.xcuda_Valuation_item WITH (NOLOCK) INNER JOIN
                 dbo.xcuda_Item WITH (NOLOCK) INNER JOIN
                 dbo.xcuda_Tarification WITH (NOLOCK) ON dbo.xcuda_Item.Item_Id = dbo.xcuda_Tarification.Item_Id INNER JOIN
                 dbo.xcuda_Supplementary_unit WITH (NOLOCK) ON dbo.xcuda_Tarification.Item_Id = dbo.xcuda_Supplementary_unit.Tarification_Id INNER JOIN
                 dbo.xcuda_HScode WITH (NOLOCK) ON dbo.xcuda_Tarification.Item_Id = dbo.xcuda_HScode.Item_Id ON dbo.xcuda_Valuation_item.Item_Id = dbo.xcuda_Item.Item_Id INNER JOIN
                 dbo.xcuda_Item_Invoice WITH (NOLOCK) ON dbo.xcuda_Valuation_item.Item_Id = dbo.xcuda_Item_Invoice.Valuation_item_Id INNER JOIN
                 dbo.xcuda_Weight_itm WITH (NOLOCK) ON dbo.xcuda_Valuation_item.Item_Id = dbo.xcuda_Weight_itm.Valuation_item_Id INNER JOIN
                 dbo.InventoryItems WITH (NOLOCK) ON dbo.xcuda_HScode.Precision_4 = dbo.InventoryItems.ItemNumber LEFT OUTER JOIN
                 dbo.xcuda_Goods_description WITH (NOLOCK) ON dbo.xcuda_Item.Item_Id = dbo.xcuda_Goods_description.Item_Id LEFT OUTER JOIN
                 dbo.[AscyudaItemPiQuantity-Basic] WITH (NOLOCK) ON dbo.xcuda_Item.Item_Id = dbo.[AscyudaItemPiQuantity-Basic].Item_Id LEFT OUTER JOIN
                 dbo.AsycudaItemDutyLiablity WITH (NOLOCK) ON dbo.xcuda_Item.Item_Id = dbo.AsycudaItemDutyLiablity.Item_Id
WHERE (dbo.xcuda_Supplementary_unit.Suppplementary_unit_code = N'NMB')
GROUP BY dbo.xcuda_Item.Item_Id, dbo.xcuda_Supplementary_unit.Suppplementary_unit_quantity, dbo.xcuda_Tarification.Item_price, dbo.xcuda_HScode.Precision_4, dbo.InventoryItems.TariffCode, 
                 dbo.xcuda_Valuation_item.Total_CIF_itm, dbo.AsycudaItemDutyLiablity.DutyLiability, dbo.xcuda_Goods_description.Country_of_origin_code, dbo.xcuda_Item_Invoice.Amount_national_currency, 
                 dbo.xcuda_Weight_itm.Gross_weight_itm, dbo.xcuda_Weight_itm.Net_weight_itm
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[32] 4[23] 2[21] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1[50] 4[25] 3) )"
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
         Configuration = "(H (1[64] 4) )"
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
         Top = -432
         Left = 0
      End
      Begin Tables = 
         Begin Table = "xcuda_Valuation_item"
            Begin Extent = 
               Top = 366
               Left = 546
               Bottom = 562
               Right = 945
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Item"
            Begin Extent = 
               Top = 24
               Left = 0
               Bottom = 251
               Right = 316
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Tarification"
            Begin Extent = 
               Top = 0
               Left = 487
               Bottom = 185
               Right = 778
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Supplementary_unit"
            Begin Extent = 
               Top = 178
               Left = 771
               Bottom = 341
               Right = 1132
            End
            DisplayFlags = 280
            TopColumn = 3
         End
         Begin Table = "xcuda_HScode"
            Begin Extent = 
               Top = 41
               Left = 1273
               Bottom = 263
               Right = 1485
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Item_Invoice"
            Begin Extent = 
               Top = 406
               Left = 1273
               Bottom = 611
               Right = 1601
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Weight_itm"
            Begin Extent = 
               Top = 183
               Left = 1257
               Bottom = 361' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AllocationsPreviousItem'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'
               Right = 1516
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "InventoryItems"
            Begin Extent = 
               Top = 74
               Left = 1605
               Bottom = 325
               Right = 1945
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Goods_description"
            Begin Extent = 
               Top = 5
               Left = 781
               Bottom = 210
               Right = 1092
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AscyudaItemPiQuantity"
            Begin Extent = 
               Top = 450
               Left = 263
               Bottom = 601
               Right = 501
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaItemDutyLiablity"
            Begin Extent = 
               Top = 0
               Left = 539
               Bottom = 142
               Right = 777
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
      Begin ColumnWidths = 29
         Width = 284
         Width = 1204
         Width = 1204
         Width = 1204
         Width = 1204
         Width = 1204
         Width = 1204
         Width = 1204
         Width = 1204
         Width = 1204
         Width = 1204
         Width = 1204
         Width = 1204
         Width = 1204
         Width = 1204
         Width = 1204
         Width = 1204
         Width = 1204
         Width = 1204
         Width = 1204
         Width = 1204
         Width = 1204
         Width = 1204
         Width = 1204
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 8300
         Alias = 2487
         Table = 2657
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1361
         SortOrder = 1414
         GroupBy = 1350
         Filter = 1361
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AllocationsPreviousItem'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AllocationsPreviousItem'
GO
