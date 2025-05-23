USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ItemSalesAsycudaPiSummary2-pQtyAllocated]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[ItemSalesAsycudaPiSummary2-pQtyAllocated]
AS
SELECT   dbo.AsycudaSalesAllocations.PreviousItem_Id, dbo.EntryDataDetails.ItemNumber, SUM(dbo.AsycudaSalesAllocations.QtyAllocated) AS QtyAllocated, 
                dbo.AsycudaItemBasicInfo.DPQtyAllocated + dbo.AsycudaItemBasicInfo.DFQtyAllocated AS pQtyAllocated, CONVERT(varchar(7), entrydata.InvoiceDate, 126) AS MonthYear, entrydata.Type, dbo.AsycudaItemBasicInfo.ItemQuantity as pCNumber, AsycudaItemBasicInfo.LineNumber as pLineNumber
FROM      dbo.AsycudaSalesAllocations INNER JOIN
                dbo.AsycudaItemBasicInfo ON dbo.AsycudaSalesAllocations.PreviousItem_Id = dbo.AsycudaItemBasicInfo.Item_Id INNER JOIN
                dbo.EntryDataDetails ON dbo.AsycudaSalesAllocations.EntryDataDetailsId = dbo.EntryDataDetails.EntryDataDetailsId INNER JOIN
                dbo.EntryDataEx AS entrydata ON dbo.EntryDataDetails.EntryData_Id = entrydata.EntryData_Id INNER JOIN
                dbo.SystemDocumentSets ON entrydata.AsycudaDocumentSetId = dbo.SystemDocumentSets.Id
WHERE   (dbo.AsycudaSalesAllocations.Status IS NULL)
GROUP BY dbo.EntryDataDetails.ItemNumber, dbo.AsycudaItemBasicInfo.DPQtyAllocated + dbo.AsycudaItemBasicInfo.DFQtyAllocated, dbo.AsycudaSalesAllocations.PreviousItem_Id, entrydata.Type, CONVERT(varchar(7), entrydata.InvoiceDate, 126), 
                dbo.AsycudaItemBasicInfo.ItemQuantity, AsycudaItemBasicInfo.CNumber , AsycudaItemBasicInfo.LineNumber
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
         Begin Table = "AsycudaSalesAllocations"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 146
               Right = 249
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaItemBasicInfo"
            Begin Extent = 
               Top = 6
               Left = 287
               Bottom = 208
               Right = 532
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryDataDetails"
            Begin Extent = 
               Top = 6
               Left = 570
               Bottom = 146
               Right = 812
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "entrydata"
            Begin Extent = 
               Top = 6
               Left = 850
               Bottom = 146
               Right = 1091
            End
            DisplayFlags = 280
            TopColumn = 2
         End
         Begin Table = "SystemDocumentSets"
            Begin Extent = 
               Top = 6
               Left = 1129
               Bottom = 89
               Right = 1319
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
      Begin ColumnWidths = 12
         Column = 11460
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 1350
        ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ItemSalesAsycudaPiSummary2-pQtyAllocated'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N' Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ItemSalesAsycudaPiSummary2-pQtyAllocated'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ItemSalesAsycudaPiSummary2-pQtyAllocated'
GO
