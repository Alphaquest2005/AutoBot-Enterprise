USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-Mismatched Allocations-EntryDataDetails]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[TODO-Mismatched Allocations-EntryDataDetails]
AS
SELECT TOP (100) PERCENT dbo.AsycudaDocumentItem.ApplicationSettingsId, dbo.EntryDataDetails.EntryDataDetailsId, dbo.EntryDataDetails.ItemNumber, dbo.EntryDataDetails.Quantity, dbo.EntryDataDetails.QtyAllocated, 
                 dbo.EntryDataDetails.Status, SUM(dbo.AsycudaDocumentItemEntryDataDetails.Quantity) AS PiQuantity, dbo.EntryDataEx.Type, dbo.EntryDataEx.InvoiceDate, dbo.EntryDataEx.InvoiceNo, 
                 dbo.EntryDataDetails.LineNumber, dbo.EntryDataDetails.ItemDescription
FROM    dbo.EntryDataDetails INNER JOIN
                 dbo.AsycudaDocumentItemEntryDataDetails ON dbo.EntryDataDetails.EntryDataDetailsId = dbo.AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId AND 
                 dbo.EntryDataDetails.QtyAllocated <> dbo.AsycudaDocumentItemEntryDataDetails.Quantity INNER JOIN
                 dbo.AsycudaDocumentItem ON dbo.AsycudaDocumentItemEntryDataDetails.Item_Id = dbo.AsycudaDocumentItem.Item_Id INNER JOIN
                 dbo.EntryDataEx ON dbo.EntryDataDetails.EntryDataId = dbo.EntryDataEx.InvoiceNo LEFT OUTER JOIN
                 dbo.EntryData_Adjustments ON dbo.EntryDataEx.EntryData_Id = dbo.EntryData_Adjustments.EntryData_Id LEFT OUTER JOIN
                 dbo.EntryData_Sales ON dbo.EntryDataEx.EntryData_Id = dbo.EntryData_Sales.EntryData_Id
				 inner join AsycudaDocumentCustomsProcedures on AsycudaDocumentCustomsProcedures.ASYCUDA_Id = AsycudaDocumentItemEntryDataDetails.ASYCUDA_Id
WHERE (dbo.AsycudaDocumentItem.ImportComplete = 1) 
			and (AsycudaDocumentCustomsProcedures.CustomsOperationId = 'Exwarehouse')
--AND (dbo.AsycudaDocumentItemEntryDataDetails.DocumentType <> N'IM7' OR  dbo.AsycudaDocumentItemEntryDataDetails.DocumentType <> N'IM9')
GROUP BY dbo.AsycudaDocumentItem.ApplicationSettingsId, dbo.EntryDataDetails.EntryDataDetailsId, dbo.EntryDataDetails.ItemNumber, dbo.EntryDataDetails.Quantity, dbo.EntryDataDetails.QtyAllocated, 
                 dbo.EntryDataDetails.Status, dbo.EntryDataEx.Type, dbo.EntryDataEx.InvoiceDate, dbo.EntryDataEx.InvoiceNo, dbo.EntryDataDetails.LineNumber, dbo.EntryDataDetails.ItemDescription
HAVING (SUM(dbo.AsycudaDocumentItemEntryDataDetails.Quantity) <> dbo.EntryDataDetails.QtyAllocated)
ORDER BY dbo.EntryDataDetails.EntryDataDetailsId
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
            TopColumn = 4
         End
         Begin Table = "AsycudaDocumentItemEntryDataDetails"
            Begin Extent = 
               Top = 91
               Left = 663
               Bottom = 246
               Right = 886
            End
            DisplayFlags = 280
            TopColumn = 2
         End
         Begin Table = "AsycudaDocumentItem"
            Begin Extent = 
               Top = 150
               Left = 985
               Bottom = 305
               Right = 1278
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryData_Sales"
            Begin Extent = 
               Top = 81
               Left = 1277
               Bottom = 215
               Right = 1482
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryData_Adjustments"
            Begin Extent = 
               Top = 6
               Left = 1477
               Bottom = 119
               Right = 1677
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryDataEx"
            Begin Extent = 
               Top = 28
               Left = 402
               Bottom = 183
               Right = 640
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
      Begin ColumnWidths ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-Mismatched Allocations-EntryDataDetails'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'= 11
         Width = 284
         Width = 1309
         Width = 1309
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
      Begin ColumnWidths = 12
         Column = 1440
         Alias = 903
         Table = 2068
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1348
         SortOrder = 1414
         GroupBy = 1350
         Filter = 3796
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-Mismatched Allocations-EntryDataDetails'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-Mismatched Allocations-EntryDataDetails'
GO
