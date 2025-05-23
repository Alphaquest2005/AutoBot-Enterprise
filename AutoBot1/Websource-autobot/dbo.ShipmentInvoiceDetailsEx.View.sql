USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ShipmentInvoiceDetailsEx]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
















CREATE VIEW [dbo].[ShipmentInvoiceDetailsEx]
AS
SELECT   ShipmentInvoiceDetails.Id AS DetailId, ShipmentInvoice.Id AS InvoiceId, ShipmentInvoice.InvoiceDate, ShipmentInvoice.InvoiceNo, ShipmentInvoiceDetails.ItemNumber, ShipmentInvoiceDetails.ItemDescription, ShipmentInvoiceDetails.Cost, 
                ShipmentInvoiceDetails.Quantity, ShipmentInvoiceDetails.SalesFactor, ShipmentInvoiceDetails.TotalCost, ShipmentInvoice.ApplicationSettingsId, ShipmentInvoiceDetails.InventoryItemId, MIN(InventoryItemAlias.InventoryItemId) 
                AS POInventoryItemId, ShipmentInvoice.ImportedLines, InventoryItems.TariffCode, CASE WHEN Volume.units = 'Gallons' THEN Volume.quantity ELSE 0 END AS Gallons, Emails.EmailDate
FROM      ShipmentInvoice INNER JOIN
                ShipmentInvoiceDetails ON ShipmentInvoice.Id = ShipmentInvoiceDetails.ShipmentInvoiceId INNER JOIN
                InventoryItems ON ShipmentInvoiceDetails.InventoryItemId = InventoryItems.Id INNER JOIN
                Emails ON ShipmentInvoice.EmailId = Emails.EmailId LEFT OUTER JOIN
                [ShipmentInvoiceDetails-Volume] AS Volume ON ShipmentInvoiceDetails.Id = Volume.Id LEFT OUTER JOIN
               [InventoryItemAliasEx] as InventoryItemAlias ON ShipmentInvoiceDetails.InventoryItemId = InventoryItemAlias.AliasItemId
GROUP BY ShipmentInvoiceDetails.Id, ShipmentInvoice.Id, ShipmentInvoice.InvoiceNo, ShipmentInvoiceDetails.ItemNumber, ShipmentInvoiceDetails.ItemDescription, ShipmentInvoiceDetails.Cost, ShipmentInvoiceDetails.Quantity, 
                ShipmentInvoiceDetails.SalesFactor, ShipmentInvoiceDetails.TotalCost, ShipmentInvoice.ApplicationSettingsId, ShipmentInvoiceDetails.InventoryItemId, ShipmentInvoice.ImportedLines, InventoryItems.TariffCode, Volume.Quantity, 
                Volume.Units, ShipmentInvoice.InvoiceDate, Emails.EmailDate
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
         Begin Table = "ShipmentInvoice"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 161
               Right = 266
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ShipmentInvoiceDetails"
            Begin Extent = 
               Top = 6
               Left = 310
               Bottom = 161
               Right = 516
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ShipmentInvoiceDetailsEx'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ShipmentInvoiceDetailsEx'
GO
