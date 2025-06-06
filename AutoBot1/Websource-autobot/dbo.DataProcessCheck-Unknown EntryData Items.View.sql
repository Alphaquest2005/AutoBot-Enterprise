USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[DataProcessCheck-Unknown EntryData Items]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[DataProcessCheck-Unknown EntryData Items]
AS
SELECT DISTINCT dbo.EntryData.EntryDataDate, dbo.EntryData.EntryDataId, dbo.EntryDataDetails.ItemNumber, dbo.EntryDataDetails.ItemDescription, dbo.EntryDataDetails.Cost, dbo.EntryDataDetails.Quantity
FROM    dbo.[InventoryItems-NonStock] RIGHT OUTER JOIN
                 dbo.EntryData INNER JOIN
                 dbo.EntryDataDetails INNER JOIN
                 dbo.InventoryItems ON dbo.EntryDataDetails.ItemNumber = dbo.InventoryItems.ItemNumber ON dbo.EntryData.EntryDataId = dbo.EntryDataDetails.EntryDataId ON 
                 dbo.[InventoryItems-NonStock].InventoryItemId = dbo.InventoryItems.Id LEFT OUTER JOIN
                 dbo.xcuda_HScode INNER JOIN
                 dbo.[InventoryItemAliasEx] as InventoryItemAlias ON dbo.xcuda_HScode.Precision_4 = InventoryItemAlias.AliasName ON dbo.InventoryItems.Id = InventoryItemAlias.InventoryItemId LEFT OUTER JOIN
                 dbo.xcuda_HScode AS xcuda_HScode_1 ON dbo.InventoryItems.ItemNumber = xcuda_HScode_1.Precision_4
WHERE (xcuda_HScode_1.Precision_4 IS NULL) AND (dbo.xcuda_HScode.Precision_4 IS NULL) AND (dbo.[InventoryItems-NonStock].InventoryItemId IS NULL)
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[31] 4[15] 2[18] 3) )"
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
         Begin Table = "InventoryItems-NonStock"
            Begin Extent = 
               Top = 175
               Left = 1453
               Bottom = 267
               Right = 1645
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryData"
            Begin Extent = 
               Top = 9
               Left = 57
               Bottom = 206
               Right = 314
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryDataDetails"
            Begin Extent = 
               Top = 9
               Left = 371
               Bottom = 206
               Right = 621
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "InventoryItems"
            Begin Extent = 
               Top = 9
               Left = 678
               Bottom = 206
               Right = 912
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_HScode"
            Begin Extent = 
               Top = 156
               Left = 1671
               Bottom = 360
               Right = 1916
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "InventoryItemAlias"
            Begin Extent = 
               Top = 37
               Left = 1667
               Bottom = 235
               Right = 1837
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_HScode_1"
            Begin Extent = 
               Top = 17
               Left = 1232
               Bottom = 226
              ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'DataProcessCheck-Unknown EntryData Items'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N' Right = 1416
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
         Width = 284
         Width = 1977
         Width = 3011
         Width = 2186
         Width = 2199
         Width = 2291
         Width = 1008
         Width = 1008
         Width = 1008
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 5263
         Alias = 903
         Table = 5119
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1348
         SortOrder = 1414
         GroupBy = 1350
         Filter = 6873
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'DataProcessCheck-Unknown EntryData Items'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'DataProcessCheck-Unknown EntryData Items'
GO
