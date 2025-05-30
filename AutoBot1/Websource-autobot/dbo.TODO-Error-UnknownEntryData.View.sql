USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-Error-UnknownEntryData]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[TODO-Error-UnknownEntryData]
AS
SELECT DISTINCT 
                 EntryData.InvoiceDate, EntryData.InvoiceNo, dbo.EntryDataDetails.ItemNumber, dbo.EntryDataDetails.ItemDescription, dbo.EntryDataDetails.Cost, dbo.EntryDataDetails.Quantity, EntryData.ApplicationSettingsId, 
                 EntryData.Type, dbo.EntryDataDetails.LineNumber
FROM    dbo.[InventoryItems-NonStock] RIGHT OUTER JOIN
                 dbo.EntryDataEx AS EntryData INNER JOIN
                 dbo.EntryDataDetails INNER JOIN
                 dbo.InventoryItems ON dbo.EntryDataDetails.InventoryItemId = dbo.InventoryItems.Id ON EntryData.InvoiceNo = dbo.EntryDataDetails.EntryDataId ON 
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
         Begin Table = "InventoryItems-NonStock"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 98
               Right = 236
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryDataDetails"
            Begin Extent = 
               Top = 6
               Left = 546
               Bottom = 161
               Right = 784
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "InventoryItems"
            Begin Extent = 
               Top = 6
               Left = 828
               Bottom = 161
               Right = 1050
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_HScode"
            Begin Extent = 
               Top = 6
               Left = 1094
               Bottom = 161
               Right = 1294
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "InventoryItemAlias"
            Begin Extent = 
               Top = 6
               Left = 1338
               Bottom = 140
               Right = 1530
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_HScode_1"
            Begin Extent = 
               Top = 6
               Left = 1574
               Bottom = 161
               Right = 1774
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryData"
            Begin Extent = 
               Top = 125
               Left = 154
               Bottom = 280
               Righ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-Error-UnknownEntryData'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N't = 376
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
      Begin ColumnWidths = 11
         Column = 5433
         Alias = 1728
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-Error-UnknownEntryData'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-Error-UnknownEntryData'
GO
