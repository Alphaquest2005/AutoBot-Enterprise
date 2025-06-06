USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-DiscrepanciesExecutionReport-Adjustments]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[TODO-DiscrepanciesExecutionReport-Adjustments]
AS
SELECT dbo.EntryDataDetails.EntryDataDetailsId, dbo.ApplicationSettings.ApplicationSettingsId, dbo.AsycudaDocumentSetEntryData.AsycudaDocumentSetId, CAST(CASE WHEN dbo.InventoryItems.TariffCode IS NULL 
                 THEN 0 ELSE 1 END AS bit) AS IsClassified, dbo.EntryData_Adjustments.Type AS AdjustmentType, dbo.EntryData.EntryDataId AS InvoiceNo, dbo.EntryDataDetails.InvoiceQty, dbo.EntryDataDetails.ReceivedQty, 
                 dbo.EntryData.EntryDataDate AS InvoiceDate, dbo.EntryDataDetails.ItemNumber, dbo.AsycudaSalesAllocations.Status, dbo.EntryDataDetails.CNumber, dbo.AsycudaDocumentSet.Declarant_Reference_Number, 
                 dbo.EntryData.EmailId
FROM    dbo.ApplicationSettings INNER JOIN
                 dbo.EntryData ON dbo.ApplicationSettings.ApplicationSettingsId = dbo.EntryData.ApplicationSettingsId INNER JOIN
                 dbo.AsycudaDocumentSetEntryData ON dbo.EntryData.EntryData_Id = dbo.AsycudaDocumentSetEntryData.EntryData_Id INNER JOIN
                 dbo.AsycudaDocumentSet ON dbo.ApplicationSettings.ApplicationSettingsId = dbo.AsycudaDocumentSet.ApplicationSettingsId AND 
                 dbo.AsycudaDocumentSetEntryData.AsycudaDocumentSetId = dbo.AsycudaDocumentSet.AsycudaDocumentSetId INNER JOIN
                 dbo.InventoryItems ON dbo.ApplicationSettings.ApplicationSettingsId = dbo.InventoryItems.ApplicationSettingsId INNER JOIN
                 dbo.EntryDataDetails ON dbo.InventoryItems.Id = dbo.EntryDataDetails.InventoryItemId INNER JOIN
                 dbo.EntryData_Adjustments ON dbo.EntryData.EntryData_Id = dbo.EntryData_Adjustments.EntryData_Id AND dbo.EntryDataDetails.EntryData_Id = dbo.EntryData_Adjustments.EntryData_Id LEFT OUTER JOIN
                 dbo.AsycudaSalesAllocations ON dbo.EntryDataDetails.EntryDataDetailsId = dbo.AsycudaSalesAllocations.EntryDataDetailsId LEFT OUTER JOIN
                 dbo.SystemDocumentSets ON dbo.AsycudaDocumentSet.AsycudaDocumentSetId = dbo.SystemDocumentSets.Id
WHERE (dbo.SystemDocumentSets.Id IS NULL) AND (dbo.EntryData_Adjustments.Type IS NOT NULL) AND (ISNULL(dbo.EntryDataDetails.DoNotAllocate, 0) <> 1) AND (ISNULL(dbo.EntryDataDetails.IsReconciled, 0) <> 1)
GROUP BY dbo.EntryDataDetails.EntryDataDetailsId, dbo.ApplicationSettings.ApplicationSettingsId, dbo.AsycudaDocumentSetEntryData.AsycudaDocumentSetId, dbo.InventoryItems.TariffCode, 
                 dbo.EntryData_Adjustments.Type, dbo.EntryData.EntryDataId, dbo.EntryDataDetails.InvoiceQty, dbo.EntryDataDetails.ReceivedQty, dbo.EntryData.EntryDataDate, dbo.EntryDataDetails.ItemNumber, 
                 dbo.AsycudaSalesAllocations.Status, dbo.EntryDataDetails.CNumber, dbo.AsycudaDocumentSet.Declarant_Reference_Number, dbo.EntryData.EmailId
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
         Begin Table = "ApplicationSettings"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 161
               Right = 410
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryData"
            Begin Extent = 
               Top = 6
               Left = 454
               Bottom = 161
               Right = 692
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentSetEntryData"
            Begin Extent = 
               Top = 6
               Left = 736
               Bottom = 140
               Right = 991
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentSet"
            Begin Extent = 
               Top = 6
               Left = 1035
               Bottom = 161
               Right = 1323
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "InventoryItems"
            Begin Extent = 
               Top = 6
               Left = 1367
               Bottom = 161
               Right = 1605
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryDataDetails"
            Begin Extent = 
               Top = 144
               Left = 736
               Bottom = 299
               Right = 990
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryData_Adjustments"
            Begin Extent = 
               Top = 162
               Left = 44
               Bottom = 296
  ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-DiscrepanciesExecutionReport-Adjustments'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'             Right = 244
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaSalesAllocations"
            Begin Extent = 
               Top = 162
               Left = 288
               Bottom = 317
               Right = 511
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "SystemDocumentSets"
            Begin Extent = 
               Top = 162
               Left = 1034
               Bottom = 254
               Right = 1234
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
      Begin ColumnWidths = 12
         Column = 1440
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-DiscrepanciesExecutionReport-Adjustments'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-DiscrepanciesExecutionReport-Adjustments'
GO
