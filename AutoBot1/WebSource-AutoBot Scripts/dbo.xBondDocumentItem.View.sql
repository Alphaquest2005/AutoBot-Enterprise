USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[xBondDocumentItem]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[xBondDocumentItem]
AS
SELECT dbo.xcuda_Item.Item_Id, dbo.xcuda_Item.ASYCUDA_Id AS AsycudaDocumentId, dbo.xcuda_Item.EntryDataDetailsId, dbo.xcuda_Item.LineNumber, dbo.xcuda_Item.IsAssessed, 
             dbo.xcuda_Goods_description.Description_of_goods, dbo.xcuda_Goods_description.Commercial_Description, dbo.xcuda_Weight_itm.Gross_weight_itm, dbo.xcuda_Weight_itm.Net_weight_itm, 
             dbo.xcuda_Tarification.Item_price, CAST(dbo.xcuda_Supplementary_unit.Suppplementary_unit_quantity AS numeric) AS ItemQuantity, dbo.xcuda_Supplementary_unit.Suppplementary_unit_code, 
             dbo.xcuda_HScode.Precision_4 AS ItemNumber, dbo.xcuda_HScode.Commodity_code AS TariffCode, dbo.xcuda_Valuation_item.Total_CIF_itm, 
             dbo.xcuda_item_external_freight.Amount_national_currency AS Freight, dbo.xcuda_Valuation_item.Statistical_value
FROM   dbo.xcuda_Item WITH (NOLOCK) INNER JOIN
             dbo.xcuda_Goods_description WITH (NOLOCK) ON dbo.xcuda_Item.Item_Id = dbo.xcuda_Goods_description.Item_Id INNER JOIN
             dbo.xcuda_Tarification WITH (NOLOCK) ON dbo.xcuda_Item.Item_Id = dbo.xcuda_Tarification.Item_Id INNER JOIN
             dbo.xcuda_HScode WITH (NOLOCK) ON dbo.xcuda_Tarification.Item_Id = dbo.xcuda_HScode.Item_Id INNER JOIN
             dbo.xcuda_Supplementary_unit WITH (NOLOCK) ON dbo.xcuda_Tarification.Item_Id = dbo.xcuda_Supplementary_unit.Tarification_Id INNER JOIN
             dbo.xcuda_Valuation_item WITH (NOLOCK) ON dbo.xcuda_Item.Item_Id = dbo.xcuda_Valuation_item.Item_Id INNER JOIN
             dbo.xcuda_Weight_itm WITH (NOLOCK) ON dbo.xcuda_Valuation_item.Item_Id = dbo.xcuda_Weight_itm.Valuation_item_Id LEFT OUTER JOIN
             dbo.xcuda_item_external_freight WITH (NOLOCK) ON dbo.xcuda_Valuation_item.Item_Id = dbo.xcuda_item_external_freight.Valuation_item_Id
GROUP BY dbo.xcuda_Item.Item_Id, dbo.xcuda_Item.ASYCUDA_Id, dbo.xcuda_Item.EntryDataDetailsId, dbo.xcuda_Item.LineNumber, dbo.xcuda_Goods_description.Description_of_goods, 
             dbo.xcuda_Goods_description.Commercial_Description, dbo.xcuda_Weight_itm.Gross_weight_itm, dbo.xcuda_Weight_itm.Net_weight_itm, dbo.xcuda_Tarification.Item_price, 
            dbo.xcuda_Supplementary_unit.Suppplementary_unit_quantity, dbo.xcuda_Supplementary_unit.Suppplementary_unit_code, dbo.xcuda_HScode.Precision_4, 
             dbo.xcuda_HScode.Commodity_code, dbo.xcuda_Valuation_item.Total_CIF_itm, dbo.xcuda_item_external_freight.Amount_national_currency, dbo.xcuda_Valuation_item.Statistical_value, 
             dbo.xcuda_Item.IsAssessed
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[40] 4[20] 2[20] 3) )"
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
         Configuration = "(H (1[43] 4[31] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1[65] 4) )"
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
      ActivePaneConfig = 8
   End
   Begin DiagramPane = 
      Begin Origin = 
         Top = 0
         Left = 0
      End
      Begin Tables = 
         Begin Table = "xcuda_Item"
            Begin Extent = 
               Top = 9
               Left = 57
               Bottom = 205
               Right = 416
            End
            DisplayFlags = 280
            TopColumn = 12
         End
         Begin Table = "xcuda_Goods_description"
            Begin Extent = 
               Top = 8
               Left = 528
               Bottom = 204
               Right = 823
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Tarification"
            Begin Extent = 
               Top = 190
               Left = 466
               Bottom = 386
               Right = 805
            End
            DisplayFlags = 280
            TopColumn = 2
         End
         Begin Table = "xcuda_HScode"
            Begin Extent = 
               Top = 9
               Left = 1270
               Bottom = 205
               Right = 1515
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Supplementary_unit"
            Begin Extent = 
               Top = 13
               Left = 1610
               Bottom = 209
               Right = 1954
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Valuation_item"
            Begin Extent = 
               Top = 71
               Left = 849
               Bottom = 337
               Right = 1232
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Weight_itm"
            Begin Extent = 
               Top = 303
               Left = 1574
               Bottom = 472' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'xBondDocumentItem'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'
               Right = 1817
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_item_external_freight"
            Begin Extent = 
               Top = 162
               Left = 1460
               Bottom = 358
               Right = 1772
            End
            DisplayFlags = 280
            TopColumn = 0
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      PaneHidden = 
      Begin ParameterDefaults = ""
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 2450
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'xBondDocumentItem'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'xBondDocumentItem'
GO
