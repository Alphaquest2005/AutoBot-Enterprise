USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[SalesDataDetails]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[SalesDataDetails]
AS
SELECT dbo.EntryDataDetails.EntryDataDetailsId,dbo.EntryDataDetails.EntryData_Id, dbo.EntryDataDetails.EntryDataId, dbo.EntryDataDetails.LineNumber, dbo.EntryDataDetails.ItemNumber, dbo.EntryDataDetails.Quantity, dbo.EntryDataDetails.Units, 
                 dbo.EntryDataDetails.ItemDescription, dbo.EntryDataDetails.Cost, dbo.EntryDataDetails.QtyAllocated, dbo.EntryDataDetails.UnitWeight, dbo.EntryDataDetails.DoNotAllocate, dbo.InventoryItems.TariffCode, 
                 dbo.xcuda_Registration.Number AS CNumber, dbo.xcuda_Item.LineNumber AS CLineNumber, CAST(CASE WHEN Item_Id IS NULL THEN 1 ELSE 0 END AS bit) AS Downloaded, dbo.xcuda_Registration.ASYCUDA_Id, 
                 dbo.EntryDataDetails.Cost * dbo.EntryDataDetails.Quantity AS SalesValue, dbo.EntryData.EntryDataDate, dbo.EntryData.ApplicationSettingsId
FROM    dbo.EntryData INNER JOIN
                 dbo.EntryDataDetails WITH (NOLOCK) INNER JOIN
                 dbo.EntryData_Sales WITH (NOLOCK) ON dbo.EntryDataDetails.EntryData_Id = dbo.EntryData_Sales.EntryData_Id ON dbo.EntryData.EntryData_Id = dbo.EntryDataDetails.EntryData_Id LEFT OUTER JOIN
                 dbo.InventoryItems WITH (NOLOCK) ON dbo.EntryDataDetails.InventoryItemId = dbo.InventoryItems.Id LEFT OUTER JOIN
                 dbo.xcuda_Item WITH (NOLOCK) ON dbo.EntryDataDetails.EntryDataDetailsId = dbo.xcuda_Item.EntryDataDetailsId LEFT OUTER JOIN
                 dbo.xcuda_Registration WITH (NOLOCK) ON dbo.xcuda_Item.ASYCUDA_Id = dbo.xcuda_Registration.ASYCUDA_Id
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[33] 4[29] 2[20] 3) )"
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
         Configuration = "(H (1[49] 4) )"
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
         Begin Table = "EntryData"
            Begin Extent = 
               Top = 60
               Left = 1239
               Bottom = 257
               Right = 1496
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryDataDetails"
            Begin Extent = 
               Top = 36
               Left = 521
               Bottom = 232
               Right = 771
            End
            DisplayFlags = 280
            TopColumn = 2
         End
         Begin Table = "EntryData_Sales"
            Begin Extent = 
               Top = 35
               Left = 874
               Bottom = 273
               Right = 1171
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "InventoryItems"
            Begin Extent = 
               Top = 196
               Left = 1026
               Bottom = 392
               Right = 1260
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Item"
            Begin Extent = 
               Top = 9
               Left = 57
               Bottom = 205
               Right = 416
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Registration"
            Begin Extent = 
               Top = 9
               Left = 1071
               Bottom = 190
               Right = 1293
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
       ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'SalesDataDetails'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'  Width = 995
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
      Begin ColumnWidths = 11
         Column = 5236
         Alias = 903
         Table = 2108
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'SalesDataDetails'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'SalesDataDetails'
GO
