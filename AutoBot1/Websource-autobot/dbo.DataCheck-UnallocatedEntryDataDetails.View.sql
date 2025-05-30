USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[DataCheck-UnallocatedEntryDataDetails]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[DataCheck-UnallocatedEntryDataDetails]
AS
SELECT distinct  dbo.EntryDataEx.Type, dbo.EntryDataEx.InvoiceNo, dbo.EntryDataEx.InvoiceDate, EntryDataDetailsEx.EntryDataDetailsId , dbo.EntryDataDetailsEx.LineNumber, dbo.EntryDataDetailsEx.ItemNumber, dbo.EntryDataDetailsEx.Quantity, dbo.EntryDataDetailsEx.ItemDescription, 
                dbo.EntryDataDetailsEx.Cost, dbo.EntryDataDetailsEx.QtyAllocated
FROM      dbo.EntryDataDetailsEx INNER JOIN
                dbo.EntryDataEx ON dbo.EntryDataDetailsEx.EntryData_Id = dbo.EntryDataEx.EntryData_Id INNER JOIN
                dbo.SystemDocumentSets ON dbo.EntryDataEx.AsycudaDocumentSetId = dbo.SystemDocumentSets.Id LEFT OUTER JOIN
                dbo.AsycudaSalesAllocations ON dbo.EntryDataDetailsEx.EntryDataDetailsId = dbo.AsycudaSalesAllocations.EntryDataDetailsId
WHERE   (dbo.EntryDataEx.Type <> N'PO') AND (dbo.EntryDataEx.Type <> N'OPS') and EntryDataDetailsEx.InvoiceQty - ReceivedQty > 0 --- ignore overs 
		and AllocationId is null
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
         Begin Table = "EntryDataDetailsEx"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 146
               Right = 264
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryDataEx"
            Begin Extent = 
               Top = 6
               Left = 302
               Bottom = 146
               Right = 527
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "SystemDocumentSets"
            Begin Extent = 
               Top = 6
               Left = 565
               Bottom = 89
               Right = 739
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaSalesAllocations"
            Begin Extent = 
               Top = 6
               Left = 777
               Bottom = 146
               Right = 972
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
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 1440
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'DataCheck-UnallocatedEntryDataDetails'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'DataCheck-UnallocatedEntryDataDetails'
GO
