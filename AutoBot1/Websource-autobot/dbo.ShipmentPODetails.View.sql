USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ShipmentPODetails]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO








CREATE VIEW [dbo].[ShipmentPODetails]
AS
SELECT   EntryDataDetails.EntryDataDetailsId, ShipmentPOs.EntryData_Id, ShipmentPOs.InvoiceNo, ShipmentPOs.ApplicationSettingsId, MAX(ShipmentPOs.SupplierCode) AS SupplierCode, EntryDataDetails.ItemNumber, 
                EntryDataDetails.ItemDescription, InventoryItems.TariffCode, EntryDataDetails.Quantity, EntryDataDetails.Cost, EntryDataDetails.TotalCost, EntryDataDetails.InventoryItemId, MAX(InventoryItemAlias.AliasItemId) AS SupplierItemId, 
                MAX(InventoryItemAlias.AliasName) AS SupplierItemNumber, ShipmentPOs.ImportedLine, ShipmentPOs.InvoiceDate, ShipmentPOs.EmailDate
FROM      EntryDataDetails INNER JOIN
                ShipmentPOs ON EntryDataDetails.EntryData_Id = ShipmentPOs.EntryData_Id INNER JOIN
                InventoryItems ON EntryDataDetails.InventoryItemId = InventoryItems.Id LEFT OUTER JOIN
                [InventoryItemAliasEx] as InventoryItemAlias ON EntryDataDetails.InventoryItemId = InventoryItemAlias.InventoryItemId
GROUP BY EntryDataDetails.EntryDataDetailsId, ShipmentPOs.EntryData_Id, ShipmentPOs.InvoiceNo, ShipmentPOs.ApplicationSettingsId, EntryDataDetails.ItemNumber, EntryDataDetails.ItemDescription, EntryDataDetails.Quantity, 
                EntryDataDetails.Cost, EntryDataDetails.TotalCost, EntryDataDetails.InventoryItemId, ShipmentPOs.ImportedLine, InventoryItems.TariffCode, ShipmentPOs.InvoiceDate, ShipmentPOs.EmailDate
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
               Bottom = 282
               Right = 282
            End
            DisplayFlags = 280
            TopColumn = 20
         End
         Begin Table = "ShipmentPOs"
            Begin Extent = 
               Top = 0
               Left = 529
               Bottom = 278
               Right = 751
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "InventoryItemAlias"
            Begin Extent = 
               Top = 6
               Left = 326
               Bottom = 161
               Right = 518
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ShipmentPODetails'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ShipmentPODetails'
GO
