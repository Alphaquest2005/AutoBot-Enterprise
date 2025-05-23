USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AsycudaDocumentSetPreviousEntries]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[AsycudaDocumentSetPreviousEntries]
AS
SELECT DISTINCT dbo.AsycudaDocumentSet.AsycudaDocumentSetId, dbo.xcuda_ASYCUDA.ASYCUDA_Id, dbo.xcuda_Item.Item_Id
FROM   dbo.AsycudaDocumentSet WITH (NOLOCK) INNER JOIN
             dbo.AsycudaDocumentSetEntryData WITH (NOLOCK) ON dbo.AsycudaDocumentSet.AsycudaDocumentSetId = dbo.AsycudaDocumentSetEntryData.AsycudaDocumentSetId INNER JOIN
             dbo.EntryData_Sales WITH (NOLOCK) INNER JOIN
             dbo.EntryData WITH (NOLOCK) ON dbo.EntryData_Sales.EntryData_Id = dbo.EntryData.EntryData_Id INNER JOIN
             dbo.AsycudaSalesAllocations WITH (NOLOCK) INNER JOIN
             dbo.EntryDataDetails WITH (NOLOCK) ON dbo.AsycudaSalesAllocations.EntryDataDetailsId = dbo.EntryDataDetails.EntryDataDetailsId INNER JOIN
             dbo.xcuda_Item WITH (NOLOCK) ON dbo.AsycudaSalesAllocations.PreviousItem_Id = dbo.xcuda_Item.Item_Id INNER JOIN
             dbo.xcuda_ASYCUDA WITH (NOLOCK) ON dbo.xcuda_Item.ASYCUDA_Id = dbo.xcuda_ASYCUDA.ASYCUDA_Id ON dbo.EntryData_Sales.EntryData_Id = dbo.EntryDataDetails.EntryData_Id ON 
             dbo.AsycudaDocumentSetEntryData.EntryData_Id = dbo.EntryData.EntryData_Id
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
         Top = -576
         Left = 0
      End
      Begin Tables = 
         Begin Table = "AsycudaDocumentSet"
            Begin Extent = 
               Top = 609
               Left = 1084
               Bottom = 772
               Right = 1375
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentSetEntryData"
            Begin Extent = 
               Top = 682
               Left = 663
               Bottom = 860
               Right = 958
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryData_Sales"
            Begin Extent = 
               Top = 973
               Left = 48
               Bottom = 1136
               Right = 246
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryData"
            Begin Extent = 
               Top = 665
               Left = 305
               Bottom = 806
               Right = 558
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaSalesAllocations"
            Begin Extent = 
               Top = 7
               Left = 48
               Bottom = 170
               Right = 267
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryDataDetails"
            Begin Extent = 
               Top = 82
               Left = 587
               Bottom = 245
               Right = 806
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Item"
            Begin Extent = 
               Top = 343
               Left = 48
               Bottom = 506
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaDocumentSetPreviousEntries'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'               Right = 358
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_ASYCUDA"
            Begin Extent = 
               Top = 511
               Left = 48
               Bottom = 652
               Right = 252
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
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
         Width = 1200
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
         Alias = 900
         Table = 1180
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1360
         SortOrder = 1420
         GroupBy = 1350
         Filter = 1360
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaDocumentSetPreviousEntries'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaDocumentSetPreviousEntries'
GO
