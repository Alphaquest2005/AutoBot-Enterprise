USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[EntryDataExTotals]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[EntryDataExTotals]
AS
SELECT EntryData.EntryData_Id, EntryData.EntryDataId, SUM(CAST(EntryDataDetails.Quantity * EntryDataDetails.Cost AS float)) AS Total, SUM(CAST(EntryDataDetails.QtyAllocated * EntryDataDetails.Cost AS float)) 
                 AS AllocatedTotal, COUNT(DISTINCT EntryDataDetails.EntryDataDetailsId) AS TotalLines, SUM(EntryDataDetails.TaxAmount) AS Tax, COUNT(InventoryItems.TariffCode) AS ClassifiedLines, 
                 SUM(CAST(ISNULL(TariffCodes.LicenseRequired, 0) AS int)) AS LicenseLines, COUNT(DISTINCT (CASE WHEN dbo.TariffCodes.LicenseRequired = 1 THEN dbo.TariffCodes.TariffCode ELSE NULL END)) 
                 AS QtyLicensesRequired, AsycudaDocumentSetEntryData.AsycudaDocumentSetId, isnull(entrydata.Packages,0) as Packages
FROM    EntryDataDetails WITH (NOLOCK) INNER JOIN
                 EntryData WITH (NOLOCK) ON EntryDataDetails.EntryData_Id = EntryData.EntryData_Id INNER JOIN
                 AsycudaDocumentSetEntryData ON EntryData.EntryData_Id = AsycudaDocumentSetEntryData.EntryData_Id LEFT OUTER JOIN
                 InventoryItemSource INNER JOIN
                 InventorySources ON InventoryItemSource.InventorySourceId = InventorySources.Id RIGHT OUTER JOIN
                 InventoryItems ON InventoryItemSource.InventoryId = InventoryItems.Id LEFT OUTER JOIN
                 TariffCodes ON InventoryItems.TariffCode = TariffCodes.TariffCode ON EntryData.ApplicationSettingsId = InventoryItems.ApplicationSettingsId AND EntryDataDetails.InventoryItemId = InventoryItems.Id
WHERE (InventorySources.Name = N'POS') 
GROUP BY EntryData.EntryData_Id, EntryData.EntryDataId, AsycudaDocumentSetEntryData.AsycudaDocumentSetId, Packages
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
         Begin Table = "TariffCodes"
            Begin Extent = 
               Top = 213
               Left = 734
               Bottom = 368
               Right = 983
            End
            DisplayFlags = 280
            TopColumn = 10
         End
         Begin Table = "InventoryItems"
            Begin Extent = 
               Top = 33
               Left = 270
               Bottom = 294
               Right = 508
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryDataDetails"
            Begin Extent = 
               Top = 0
               Left = 586
               Bottom = 196
               Right = 864
            End
            DisplayFlags = 280
            TopColumn = 3
         End
         Begin Table = "EntryData"
            Begin Extent = 
               Top = 41
               Left = 990
               Bottom = 210
               Right = 1285
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "InventoryItemSource"
            Begin Extent = 
               Top = 24
               Left = 20
               Bottom = 158
               Right = 241
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "InventorySources"
            Begin Extent = 
               Top = 218
               Left = 3
               Bottom = 331
               Right = 203
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
        ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'EntryDataExTotals'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N' Width = 2278
         Width = 2095
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
         Width = 995
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 7737
         Alias = 1872
         Table = 2435
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1348
         SortOrder = 1414
         GroupBy = 1350
         Filter = 5210
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'EntryDataExTotals'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'EntryDataExTotals'
GO
