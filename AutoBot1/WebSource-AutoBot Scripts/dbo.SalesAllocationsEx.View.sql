USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[SalesAllocationsEx]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[SalesAllocationsEx]
AS
SELECT dbo.AsycudaSalesAllocations.AllocationId, dbo.AsycudaSalesAllocations.EntryDataDetailsId, dbo.AsycudaSalesAllocations.PreviousItem_Id, dbo.AsycudaSalesAllocations.Status, 
                 dbo.AsycudaSalesAllocations.QtyAllocated, dbo.AsycudaSalesAllocations.xEntryItem_Id, ISNULL(dbo.EntryDataDetails.TaxAmount, isnull(EntryData_Sales.Tax,0)) AS TaxAmount, dbo.EntryData.EntryDataDate, 
                 dbo.EntryData.ApplicationSettingsId
FROM    dbo.AsycudaSalesAllocations INNER JOIN
                 dbo.xcuda_Item ON dbo.AsycudaSalesAllocations.PreviousItem_Id = dbo.xcuda_Item.Item_Id INNER JOIN
                 dbo.EntryPreviousItems ON dbo.xcuda_Item.Item_Id = dbo.EntryPreviousItems.Item_Id INNER JOIN
                 dbo.xcuda_Registration ON dbo.xcuda_Item.ASYCUDA_Id = dbo.xcuda_Registration.ASYCUDA_Id INNER JOIN
                 dbo.EntryDataDetails ON dbo.AsycudaSalesAllocations.EntryDataDetailsId = dbo.EntryDataDetails.EntryDataDetailsId INNER JOIN
                 dbo.EntryData INNER JOIN
                 dbo.EntryData_Sales ON dbo.EntryData.EntryData_Id = dbo.EntryData_Sales.EntryData_Id ON dbo.EntryDataDetails.EntryData_Id = dbo.EntryData.EntryData_Id INNER JOIN
                 dbo.xcuda_PreviousItem ON dbo.EntryPreviousItems.PreviousItem_Id = dbo.xcuda_PreviousItem.PreviousItem_Id
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
         Configuration = "(H (1[50] 2[25] 3) )"
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
         Configuration = "(H (2[66] 3) )"
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
         Left = -26
      End
      Begin Tables = 
         Begin Table = "AsycudaSalesAllocations"
            Begin Extent = 
               Top = 10
               Left = 988
               Bottom = 207
               Right = 1238
            End
            DisplayFlags = 280
            TopColumn = 5
         End
         Begin Table = "xcuda_Item"
            Begin Extent = 
               Top = 278
               Left = 812
               Bottom = 475
               Right = 1171
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryPreviousItems"
            Begin Extent = 
               Top = 123
               Left = 1292
               Bottom = 293
               Right = 1555
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Registration"
            Begin Extent = 
               Top = 311
               Left = 1414
               Bottom = 481
               Right = 1636
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryDataDetails"
            Begin Extent = 
               Top = 3
               Left = 593
               Bottom = 200
               Right = 843
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryData"
            Begin Extent = 
               Top = 6
               Left = 298
               Bottom = 203
               Right = 555
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryData_Sales"
            Begin Extent = 
               Top = 9
               Left = 26
               Bottom = 206
          ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'SalesAllocationsEx'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'     Right = 254
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_PreviousItem"
            Begin Extent = 
               Top = 45
               Left = 1663
               Bottom = 242
               Right = 2029
            End
            DisplayFlags = 280
            TopColumn = 15
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
      Begin ColumnWidths = 11
         Column = 7252
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'SalesAllocationsEx'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'SalesAllocationsEx'
GO
