USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[SalesData]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[SalesData]
AS
SELECT TOP (100) PERCENT dbo.EntryData.EntryData_Id, dbo.EntryData.EntryDataId, dbo.EntryData.EntryDataDate, 'Sales' AS Type, CAST(isnull(dbo.EntryDataExTotals.Tax, EntryData_Sales.Tax) AS float) AS TaxAmount, 
                 dbo.EntryData_Sales.CustomerName, CAST(dbo.EntryDataExTotals.Total AS float) AS Total, dbo.EntryDataExTotals.AllocatedTotal, dbo.AsycudaDocumentEntryData.AsycudaDocumentId, 
                 dbo.AsycudaDocumentSetEntryData.AsycudaDocumentSetId, dbo.EntryData.ApplicationSettingsId
FROM    dbo.EntryData WITH (NOLOCK) INNER JOIN
                 dbo.EntryData_Sales WITH (NOLOCK) ON dbo.EntryData.EntryData_Id = dbo.EntryData_Sales.EntryData_Id INNER JOIN
                 dbo.EntryDataExTotals WITH (NOLOCK) ON dbo.EntryData.EntryDataId = dbo.EntryDataExTotals.EntryDataId LEFT OUTER JOIN
                 dbo.AsycudaDocumentEntryData WITH (NOLOCK) ON dbo.EntryData.EntryData_Id = dbo.AsycudaDocumentEntryData.EntryData_Id LEFT OUTER JOIN
                 dbo.AsycudaDocumentSetEntryData WITH (NOLOCK) ON dbo.EntryData.EntryData_Id = dbo.AsycudaDocumentSetEntryData.EntryData_Id
GROUP BY dbo.EntryData.EntryDataId, dbo.EntryData.EntryDataDate,  dbo.EntryData_Sales.CustomerName,  
                 dbo.EntryDataExTotals.AllocatedTotal, dbo.AsycudaDocumentEntryData.AsycudaDocumentId, dbo.AsycudaDocumentSetEntryData.AsycudaDocumentSetId, dbo.EntryData.ApplicationSettingsId, 
                 dbo.EntryData.EntryData_Id,EntryDataExTotals.Tax,EntryData_Sales.Tax,EntryDataExTotals.Total
ORDER BY dbo.EntryData.EntryDataDate DESC
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[29] 4[24] 2[19] 3) )"
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
         Configuration = "(H (1[57] 4[30] 2) )"
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
         Begin Table = "EntryData"
            Begin Extent = 
               Top = 0
               Left = 428
               Bottom = 207
               Right = 723
            End
            DisplayFlags = 280
            TopColumn = 11
         End
         Begin Table = "EntryData_Sales"
            Begin Extent = 
               Top = 27
               Left = 1276
               Bottom = 223
               Right = 1504
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryDataExTotals"
            Begin Extent = 
               Top = 9
               Left = 57
               Bottom = 189
               Right = 295
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentEntryData"
            Begin Extent = 
               Top = 229
               Left = 643
               Bottom = 354
               Right = 1019
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentSetEntryData"
            Begin Extent = 
               Top = 229
               Left = 1126
               Bottom = 371
               Right = 1437
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
         Width = 995
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
      Be' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'SalesData'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'gin ColumnWidths = 12
         Column = 5485
         Alias = 1440
         Table = 2409
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'SalesData'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'SalesData'
GO
