USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[XSales-UnAllocated]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[XSales-UnAllocated]
AS
SELECT DISTINCT 
                 xSalesDetails.Line, xSalesDetails.Date, xSalesDetails.InvoiceNo, xSalesDetails.CustomerName, xSalesDetails.ItemNumber, xSalesDetails.ItemDescription, xSalesDetails.TariffCode, xSalesDetails.SalesQuantity, 
                 xSalesDetails.SalesFactor, xSalesDetails.xQuantity, xSalesDetails.Price, xSalesDetails.DutyFreePaid, xSalesDetails.pCNumber, xSalesDetails.pLineNumber, xSalesDetails.pRegDate, xSalesDetails.CIFValue, 
                 xSalesDetails.DutyLiablity, xSalesDetails.Comment, PreviousItemsEx.CNumber, PreviousItemsEx.RegistrationDate, xSalesFiles.ApplicationSettingsId, EntryDataDetails.EntryDataDetailsId, 
                 EntryDataDetails.LineNumber AS SalesLineNumber, PreviousItemsEx.AsycudaDocumentItemId AS xItemId, PreviousItemsEx.PreviousDocumentItemId AS pItemId, EntryDataDetails.InventoryItemId, 
                 xcuda_Inventory_Item.InventoryItemId AS PreviousDocumentInventoryItemId
FROM    xSalesDetails INNER JOIN
                 PreviousItemsEx ON xSalesDetails.pLineNumber = PreviousItemsEx.pLineNumber AND xSalesDetails.pCNumber = PreviousItemsEx.Prev_reg_nbr AND 
                 xSalesDetails.xQuantity = PreviousItemsEx.Suplementary_Quantity AND xSalesDetails.DutyFreePaid = PreviousItemsEx.DutyFreePaid AND xSalesDetails.Line = PreviousItemsEx.Current_item_number INNER JOIN
                 xSalesFiles ON xSalesDetails.xSalesFileId = xSalesFiles.Id AND PreviousItemsEx.ApplicationSettingsId = xSalesFiles.ApplicationSettingsId INNER JOIN
                 EntryDataDetails ON xSalesDetails.InvoiceNo = EntryDataDetails.EntryDataId AND xSalesDetails.ItemNumber = EntryDataDetails.ItemNumber AND xSalesDetails.SalesQuantity = EntryDataDetails.Quantity INNER JOIN
                 EntryData ON EntryDataDetails.EntryData_Id = EntryData.EntryData_Id AND xSalesFiles.ApplicationSettingsId = EntryData.ApplicationSettingsId INNER JOIN
                 xcuda_Inventory_Item ON PreviousItemsEx.PreviousDocumentItemId = xcuda_Inventory_Item.Item_Id LEFT OUTER JOIN
                 AsycudaSalesAllocations ON PreviousItemsEx.PreviousDocumentItemId = AsycudaSalesAllocations.PreviousItem_Id
WHERE (AsycudaSalesAllocations.AllocationId IS NULL)
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
         Begin Table = "PreviousItemsEx"
            Begin Extent = 
               Top = 38
               Left = 960
               Bottom = 168
               Right = 1226
            End
            DisplayFlags = 280
            TopColumn = 28
         End
         Begin Table = "xSalesFiles"
            Begin Extent = 
               Top = 214
               Left = 1202
               Bottom = 344
               Right = 1404
            End
            DisplayFlags = 280
            TopColumn = 4
         End
         Begin Table = "AsycudaSalesAllocations"
            Begin Extent = 
               Top = 51
               Left = 1414
               Bottom = 181
               Right = 1599
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryDataDetails"
            Begin Extent = 
               Top = 172
               Left = 235
               Bottom = 432
               Right = 451
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryData"
            Begin Extent = 
               Top = 9
               Left = 0
               Bottom = 256
               Right = 202
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xSalesDetails"
            Begin Extent = 
               Top = 31
               Left = 628
               Bottom = 356
               Right = 801
            End
            DisplayFlags = 280
            TopColumn = 3
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 26
         Width = 284
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'XSales-UnAllocated'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
         Width = 1500
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'XSales-UnAllocated'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'XSales-UnAllocated'
GO
