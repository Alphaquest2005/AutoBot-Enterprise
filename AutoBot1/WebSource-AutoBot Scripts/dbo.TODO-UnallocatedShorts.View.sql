USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-UnallocatedShorts]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[TODO-UnallocatedShorts]
AS
SELECT dbo.EntryDataDetails.EntryDataId, dbo.EntryDataDetails.ItemNumber, dbo.EntryDataDetails.DoNotAllocate, dbo.EntryData.ApplicationSettingsId, dbo.EntryDataDetails.EffectiveDate, dbo.EntryDataDetails.Quantity, 
                 dbo.EntryDataDetails.QtyAllocated, dbo.EntryDataDetails.Cost, dbo.AdjustmentShorts.Type, dbo.EntryDataDetails.Status
FROM    dbo.EntryDataDetails INNER JOIN
                 dbo.EntryData ON dbo.EntryDataDetails.EntryDataId = dbo.EntryData.EntryDataId INNER JOIN
                 dbo.AdjustmentShorts ON dbo.EntryDataDetails.EntryDataDetailsId = dbo.AdjustmentShorts.EntryDataDetailsId LEFT OUTER JOIN
                 dbo.AsycudaSalesAllocations ON dbo.EntryDataDetails.EntryDataDetailsId = dbo.AsycudaSalesAllocations.EntryDataDetailsId
where  (dbo.AsycudaSalesAllocations.AllocationId IS NULL) AND (dbo.EntryDataDetails.Quantity > 0) AND (ISNULL(dbo.EntryDataDetails.DoNotAllocate, 0) <> 1)
GROUP BY dbo.EntryDataDetails.EntryDataId, dbo.EntryDataDetails.ItemNumber, dbo.AsycudaSalesAllocations.AllocationId, dbo.EntryDataDetails.DoNotAllocate, dbo.EntryData.ApplicationSettingsId, 
                 dbo.EntryDataDetails.EffectiveDate, dbo.EntryDataDetails.Quantity, dbo.EntryDataDetails.QtyAllocated, dbo.EntryDataDetails.Cost, dbo.AdjustmentShorts.Type, dbo.EntryDataDetails.Status
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
         Begin Table = "EntryDataDetails"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 161
               Right = 298
            End
            DisplayFlags = 280
            TopColumn = 13
         End
         Begin Table = "EntryData"
            Begin Extent = 
               Top = 140
               Left = 478
               Bottom = 295
               Right = 716
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AdjustmentShorts"
            Begin Extent = 
               Top = 0
               Left = 645
               Bottom = 155
               Right = 900
            End
            DisplayFlags = 280
            TopColumn = 24
         End
         Begin Table = "AsycudaSalesAllocations"
            Begin Extent = 
               Top = 6
               Left = 923
               Bottom = 161
               Right = 1146
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
      Begin ColumnWidths = 10
         Width = 284
         Width = 2880
         Width = 2579
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
         Column = 6467
         Alias = 903
         Table = 3129
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1348
         SortOrder = 1414
         GroupBy = 1350
         Filter = 1348
         Or' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-UnallocatedShorts'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N' = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-UnallocatedShorts'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-UnallocatedShorts'
GO
