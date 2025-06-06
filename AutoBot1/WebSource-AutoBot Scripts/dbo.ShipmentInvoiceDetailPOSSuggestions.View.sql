USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ShipmentInvoiceDetailPOSSuggestions]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[ShipmentInvoiceDetailPOSSuggestions]
AS
SELECT cast(row_number() OVER ( ORDER BY ShipmentInvoiceDetails.Id) AS int) AS id, dbo.ShipmentInvoiceDetails.ItemNumber AS SupplierCode, dbo.InventoryItems.ItemNumber AS POSCode, dbo.ShipmentInvoiceDetails.ItemDescription AS SupplierDescription, 
                 dbo.InventoryItems.Description AS ItemDescription, dbo.InventoryItems.Id as InventoryItemId,dbo.ShipmentInvoiceDetails.Id as InvoiceDetailId, dbo.InventoryItems.TariffCode
FROM    dbo.ShipmentInvoiceDetails INNER JOIN
                 dbo.InventoryItems ON dbo.ShipmentInvoiceDetails.ItemNumber LIKE '%' + dbo.InventoryItems.ItemNumber + '%' OR dbo.InventoryItems.ItemNumber LIKE '%' + dbo.ShipmentInvoiceDetails.ItemNumber + '%' INNER JOIN
                 dbo.InventoryItemSource ON dbo.InventoryItems.Id = dbo.InventoryItemSource.InventoryId INNER JOIN
                 dbo.InventorySources ON dbo.InventoryItemSource.InventorySourceId = dbo.InventorySources.Id INNER JOIN
                 dbo.ShipmentInvoice ON dbo.ShipmentInvoiceDetails.ShipmentInvoiceId = dbo.ShipmentInvoice.Id AND dbo.InventoryItems.ApplicationSettingsId = dbo.ShipmentInvoice.ApplicationSettingsId
WHERE (dbo.InventorySources.Name = N'POS')
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
         Begin Table = "ShipmentInvoiceDetails"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 161
               Right = 250
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "InventoryItems"
            Begin Extent = 
               Top = 6
               Left = 294
               Bottom = 161
               Right = 516
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "InventoryItemSource"
            Begin Extent = 
               Top = 6
               Left = 560
               Bottom = 140
               Right = 765
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "InventorySources"
            Begin Extent = 
               Top = 6
               Left = 809
               Bottom = 119
               Right = 993
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ShipmentInvoice"
            Begin Extent = 
               Top = 6
               Left = 1037
               Bottom = 161
               Right = 1259
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
         ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ShipmentInvoiceDetailPOSSuggestions'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ShipmentInvoiceDetailPOSSuggestions'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ShipmentInvoiceDetailPOSSuggestions'
GO
