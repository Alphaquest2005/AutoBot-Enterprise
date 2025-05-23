USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ToDo-POToXML]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[ToDo-POToXML]
AS
SELECT dbo.EntryDataDetails.EntryDataDetailsId, dbo.ApplicationSettings.ApplicationSettingsId, dbo.AsycudaDocumentSetEntryData.AsycudaDocumentSetId, CAST(CASE WHEN dbo.InventoryItems.TariffCode IS NULL 
                 THEN 0 ELSE 1 END AS bit) AS IsClassified
FROM    dbo.[TODO-PODocSet] AS AsycudaDocumentSet INNER JOIN
                 dbo.ApplicationSettings ON AsycudaDocumentSet.ApplicationSettingsId = dbo.ApplicationSettings.ApplicationSettingsId INNER JOIN
                 dbo.AsycudaDocumentSetEntryData ON AsycudaDocumentSet.AsycudaDocumentSetId = dbo.AsycudaDocumentSetEntryData.AsycudaDocumentSetId INNER JOIN
                 dbo.InventoryItems ON dbo.ApplicationSettings.ApplicationSettingsId = dbo.InventoryItems.ApplicationSettingsId INNER JOIN
                 dbo.EntryData_PurchaseOrders INNER JOIN
                 dbo.EntryDataDetails ON dbo.EntryData_PurchaseOrders.EntryData_Id = dbo.EntryDataDetails.EntryData_Id ON dbo.InventoryItems.Id = dbo.EntryDataDetails.InventoryItemId INNER JOIN
                dbo.[ToDo-POToXML-EntryDataEx] as EntryDataEx ON dbo.ApplicationSettings.ApplicationSettingsId = EntryDataEx.ApplicationSettingsId AND dbo.EntryData_PurchaseOrders.EntryData_Id = EntryDataEx.EntryData_Id AND 
                 dbo.AsycudaDocumentSetEntryData.EntryData_Id = EntryDataEx.EntryData_Id LEFT OUTER JOIN
                 dbo.AsycudaDocumentItemEntryDataDetails ON dbo.EntryDataDetails.EntryDataDetailsId = dbo.AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId
where (ABS(EntryDataEx.ExpectedTotal - EntryDataEx.InvoiceTotal) < 0.01)
GROUP BY dbo.EntryDataDetails.EntryDataDetailsId, dbo.ApplicationSettings.ApplicationSettingsId, dbo.AsycudaDocumentSetEntryData.AsycudaDocumentSetId,  dbo.InventoryItems.TariffCode
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[41] 4[20] 2[15] 3) )"
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
         Begin Table = "EntryData_PurchaseOrders"
            Begin Extent = 
               Top = 82
               Left = 0
               Bottom = 195
               Right = 200
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryDataDetails"
            Begin Extent = 
               Top = 221
               Left = 303
               Bottom = 427
               Right = 565
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ApplicationSettings"
            Begin Extent = 
               Top = 169
               Left = 1055
               Bottom = 324
               Right = 1421
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentSetEntryData"
            Begin Extent = 
               Top = 0
               Left = 1324
               Bottom = 134
               Right = 1579
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "InventoryItemsEx"
            Begin Extent = 
               Top = 253
               Left = 732
               Bottom = 463
               Right = 970
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentItemEntryDataDetails"
            Begin Extent = 
               Top = 6
               Left = 1577
               Bottom = 161
               Right = 1800
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryDataEx"
            Begin Extent = 
               Top = 0
               Left = 550
  ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ToDo-POToXML'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'             Bottom = 188
               Right = 788
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentSet"
            Begin Extent = 
               Top = 224
               Left = 1290
               Bottom = 379
               Right = 1600
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
         Width = 1977
         Width = 1309
         Width = 3796
         Width = 1309
         Width = 1309
         Width = 1309
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 8719
         Alias = 903
         Table = 3705
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ToDo-POToXML'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ToDo-POToXML'
GO
