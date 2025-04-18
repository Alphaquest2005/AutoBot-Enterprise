USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ShipmentInvoicePOItemMISPODetails]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[ShipmentInvoicePOItemMISPODetails]
AS
SELECT DISTINCT 
                 ShipmentPODetails.ApplicationSettingsId, ShipmentPODetails.EntryDataDetailsId AS EntryDataDetailsId, ShipmentPODetails.EntryData_Id, ShipmentPODetails.InvoiceNo, ShipmentPODetails.ItemNumber, 
                 ShipmentPODetails.ItemDescription,TariffCode, ShipmentPODetails.Cost,  ShipmentPODetails.Quantity AS Quantity, case when ShipmentPODetails.TotalCost = 0 then ShipmentPODetails.Quantity * ShipmentPODetails.Cost else ShipmentPODetails.TotalCost end AS totalcost, ShipmentPODetails.InventoryItemId, 
                 ShipmentInvoicePOs_1.InvoiceId as InvoiceId, InventoryItemAlias.AliasItemId AS AliasItemId, SourceFile
FROM    ShipmentPODetails INNER JOIN
                 ShipmentPOs ON ShipmentPODetails.EntryData_Id = ShipmentPOs.EntryData_Id INNER JOIN
                 ShipmentInvoicePOs AS ShipmentInvoicePOs_1 ON ShipmentPOs.EntryData_Id = ShipmentInvoicePOs_1.EntryData_Id LEFT OUTER JOIN
                 [InventoryItemAliasEx] as InventoryItemAlias ON ShipmentPODetails.InventoryItemId = InventoryItemAlias.InventoryItemId
where InventoryItemAlias.AliasName <> '' or InventoryItemAlias.AliasName is null
GROUP BY ShipmentPODetails.ApplicationSettingsId, ShipmentPODetails.EntryData_Id, ShipmentPODetails.InvoiceNo, ShipmentPODetails.ItemNumber, ShipmentPODetails.ItemDescription, ShipmentPODetails.Cost, 
                 ShipmentPODetails.InventoryItemId,TariffCode, InventoryItemAlias.AliasItemId, ShipmentInvoicePOs_1.InvoiceId, ShipmentPODetails.EntryDataDetailsId, ShipmentPODetails.Quantity, ShipmentPODetails.TotalCost, SourceFile

--SELECT DISTINCT 
--                 ShipmentPODetails.ApplicationSettingsId, MAX(ShipmentPODetails.EntryDataDetailsId) AS EntryDataDetailsId, ShipmentPODetails.EntryData_Id, ShipmentPODetails.InvoiceNo, ShipmentPODetails.ItemNumber, 
--                 ShipmentPODetails.ItemDescription,TariffCode, ShipmentPODetails.Cost, SUM( ShipmentPODetails.Quantity) AS Quantity, SUM(case when ShipmentPODetails.TotalCost = 0 then ShipmentPODetails.Quantity * ShipmentPODetails.Cost else ShipmentPODetails.TotalCost end) AS totalcost, ShipmentPODetails.InventoryItemId, 
--                 ShipmentInvoicePOs_1.InvoiceId as InvoiceId, InventoryItemAlias.AliasItemId AS AliasItemId
--FROM    ShipmentPODetails INNER JOIN
--                 ShipmentPOs ON ShipmentPODetails.EntryData_Id = ShipmentPOs.EntryData_Id INNER JOIN
--                 ShipmentInvoicePOs AS ShipmentInvoicePOs_1 ON ShipmentPOs.EntryData_Id = ShipmentInvoicePOs_1.EntryData_Id LEFT OUTER JOIN
--                 InventoryItemAlias ON ShipmentPODetails.InventoryItemId = InventoryItemAlias.InventoryItemId
--where InventoryItemAlias.AliasName <> '' or InventoryItemAlias.AliasName is null
--GROUP BY ShipmentPODetails.ApplicationSettingsId, ShipmentPODetails.EntryData_Id, ShipmentPODetails.InvoiceNo, ShipmentPODetails.ItemNumber, ShipmentPODetails.ItemDescription, ShipmentPODetails.Cost, 
--                 ShipmentPODetails.InventoryItemId,TariffCode, InventoryItemAlias.AliasItemId, ShipmentInvoicePOs_1.InvoiceId
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
               Left = 44
               Bottom = 161
               Right = 283
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryData_PurchaseOrders"
            Begin Extent = 
               Top = 6
               Left = 327
               Bottom = 161
               Right = 544
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ShipmentInvoicePOs_1"
            Begin Extent = 
               Top = 6
               Left = 588
               Bottom = 140
               Right = 772
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "InventoryItemAlias"
            Begin Extent = 
               Top = 6
               Left = 816
               Bottom = 161
               Right = 1008
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
   ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ShipmentInvoicePOItemMISPODetails'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'      Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ShipmentInvoicePOItemMISPODetails'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ShipmentInvoicePOItemMISPODetails'
GO
