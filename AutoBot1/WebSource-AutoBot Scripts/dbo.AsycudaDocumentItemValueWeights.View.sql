USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AsycudaDocumentItemValueWeights]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[AsycudaDocumentItemValueWeights]
AS
SELECT xcuda_Item.ASYCUDA_Id, xcuda_Valuation_item.Total_CIF_itm, xcuda_Item_Invoice.Amount_national_currency, xcuda_Weight_itm.Gross_weight_itm, xcuda_Weight_itm.Net_weight_itm, xcuda_Valuation_item.Item_Id, 
                 xcuda_Goods_description.Country_of_origin_code, xcuda_Packages.Number_of_packages
FROM    xcuda_Valuation_item WITH (NOLOCK) INNER JOIN
                 xcuda_Item_Invoice WITH (NOLOCK) ON xcuda_Valuation_item.Item_Id = xcuda_Item_Invoice.Valuation_item_Id INNER JOIN
                 xcuda_Weight_itm WITH (NOLOCK) ON xcuda_Valuation_item.Item_Id = xcuda_Weight_itm.Valuation_item_Id INNER JOIN
                 xcuda_Goods_description WITH (NOLOCK) ON xcuda_Valuation_item.Item_Id = xcuda_Goods_description.Item_Id INNER JOIN
                 xcuda_Item WITH (nolock) ON xcuda_Valuation_item.Item_Id = xcuda_Item.Item_Id LEFT OUTER JOIN
                 xcuda_Packages ON xcuda_Item.Item_Id = xcuda_Packages.Item_Id
GROUP BY xcuda_Valuation_item.Total_CIF_itm, xcuda_Item_Invoice.Amount_national_currency, xcuda_Weight_itm.Gross_weight_itm, xcuda_Weight_itm.Net_weight_itm, xcuda_Valuation_item.Item_Id, 
                 xcuda_Goods_description.Country_of_origin_code, xcuda_Item.ASYCUDA_Id, xcuda_Packages.Number_of_packages
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
         Begin Table = "xcuda_Valuation_item"
            Begin Extent = 
               Top = 0
               Left = 441
               Bottom = 265
               Right = 840
            End
            DisplayFlags = 280
            TopColumn = 2
         End
         Begin Table = "xcuda_Item_Invoice"
            Begin Extent = 
               Top = 53
               Left = 1232
               Bottom = 258
               Right = 1560
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Weight_itm"
            Begin Extent = 
               Top = 236
               Left = 950
               Bottom = 414
               Right = 1209
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Goods_description"
            Begin Extent = 
               Top = 2
               Left = 1006
               Bottom = 207
               Right = 1317
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Item"
            Begin Extent = 
               Top = 16
               Left = 10
               Bottom = 221
               Right = 385
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Packages"
            Begin Extent = 
               Top = 144
               Left = 433
               Bottom = 299
               Right = 689
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
      Begin ColumnWidths = 9
         W' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaDocumentItemValueWeights'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'idth = 284
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 1440
         Alias = 903
         Table = 1165
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaDocumentItemValueWeights'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaDocumentItemValueWeights'
GO
