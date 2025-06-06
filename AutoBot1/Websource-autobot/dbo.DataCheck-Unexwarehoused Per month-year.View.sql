USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[DataCheck-Unexwarehoused Per month-year]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[DataCheck-Unexwarehoused Per month-year]
AS
SELECT Allocations.MonthYear, Allocations.ItemNumber, Allocations.QtyAllocated, ISNULL(PreviousItems.PiQty, 0) AS PiQty, Allocations.PiQuantity, ROUND(Allocations.pQtyAllocated, 2) AS pQtyAllocated, 
                 Allocations.pCNumber, Allocations.pLineNumber, Allocations.DutyFreePaid, Allocations.ApplicationSettingsId
FROM    (SELECT Sales.MonthYear, Sales.SalesFactor, Sales.ItemNumber, Sales.QtyAllocated, Sales.pQtyAllocated, Sales.pCNumber, Sales.pLineNumber, Sales.WarehouseError, Sales.DutyFreePaid, SUM(Pi.PiQuantity) 
                                  AS PiQuantity, Sales.ApplicationSettingsId
                 FROM     (SELECT DATENAME(MONTH, EntryData.InvoiceDate) + '-' + DATENAME(YEAR, EntryData.InvoiceDate) AS MonthYear, 
                                                    CASE WHEN xcuda_Item.SalesFactor = 0 THEN 1 ELSE xcuda_item.SalesFactor END AS SalesFactor, EntryDataDetails.ItemNumber, SUM(AsycudaSalesAllocations.QtyAllocated) AS QtyAllocated, 
                                                    xcuda_Item.DFQtyAllocated + xcuda_Item.DPQtyAllocated AS pQtyAllocated, xcuda_Registration.Number AS pCNumber, xcuda_Item.LineNumber AS pLineNumber, xcuda_Item.WarehouseError, 
                                                    EntryData.DutyFreePaid, EntryData.ApplicationSettingsId
                                   FROM     AsycudaSalesAllocations INNER JOIN
                                                    EntryDataDetails ON AsycudaSalesAllocations.EntryDataDetailsId = EntryDataDetails.EntryDataDetailsId INNER JOIN
                                                    EntryDataEx AS EntryData ON EntryDataDetails.EntryDataId = EntryData.InvoiceNo INNER JOIN
                                                    xcuda_Item ON AsycudaSalesAllocations.PreviousItem_Id = xcuda_Item.Item_Id INNER JOIN
                                                    xcuda_Registration ON xcuda_Item.ASYCUDA_Id = xcuda_Registration.ASYCUDA_Id
                                   GROUP BY DATENAME(MONTH, EntryData.InvoiceDate) + '-' + DATENAME(YEAR, EntryData.InvoiceDate), EntryDataDetails.ItemNumber, xcuda_Item.DFQtyAllocated + xcuda_Item.DPQtyAllocated, 
                                                    xcuda_Item.SalesFactor, xcuda_Registration.Number, xcuda_Item.WarehouseError, xcuda_Item.LineNumber, EntryData.DutyFreePaid, EntryData.ApplicationSettingsId
                                   HAVING (xcuda_Item.WarehouseError IS NULL) AND (SUM(AsycudaSalesAllocations.QtyAllocated) < xcuda_Item.DFQtyAllocated + xcuda_Item.DPQtyAllocated)) AS Sales INNER JOIN
                                      (SELECT SUM(PiQuantity) AS PiQuantity, DATENAME(MONTH, AssessmentDate) + '-' + DATENAME(YEAR, AssessmentDate) AS MonthYear, ItemNumber, DutyFreePaid, ApplicationSettingsId
                                       FROM     AsycudaItemPiQuantityData
                                       GROUP BY AssessmentDate, ItemNumber, DutyFreePaid, ApplicationSettingsId) AS Pi ON Sales.MonthYear = Pi.MonthYear AND Sales.ItemNumber = Pi.ItemNumber AND 
                                  Sales.DutyFreePaid = Pi.DutyFreePaid AND Sales.ApplicationSettingsId = Pi.ApplicationSettingsId
                 GROUP BY Sales.MonthYear, Sales.SalesFactor, Sales.ItemNumber, Sales.QtyAllocated, Sales.pQtyAllocated, Sales.pCNumber, Sales.pLineNumber, Sales.WarehouseError, Sales.DutyFreePaid, 
                                  Sales.ApplicationSettingsId) AS Allocations LEFT OUTER JOIN
                     (SELECT DATENAME(MONTH, PreviousItemsEx.AssessmentDate) + '-' + DATENAME(YEAR, PreviousItemsEx.AssessmentDate) AS MonthYear, ISNULL(InventoryItemAliasEx.AliasName, PreviousItemsEx.ItemNumber) 
                                       AS ItemNumber, SUM(PreviousItemsEx.Suplementary_Quantity * PreviousItemsEx.SalesFactor) AS PiQty, PreviousItemsEx.Prev_reg_nbr AS pCNumber, 
                                       PreviousItemsEx.Previous_item_number AS pLineNumber, PreviousItemsEx.DutyFreePaid, PreviousItemsEx.ApplicationSettingsId
                      FROM     PreviousItemsEx LEFT OUTER JOIN
                                       InventoryItemAliasEx ON PreviousItemsEx.ApplicationSettingsId = InventoryItemAliasEx.ApplicationSettingsId AND PreviousItemsEx.ItemNumber = InventoryItemAliasEx.ItemNumber
                      GROUP BY DATENAME(MONTH, PreviousItemsEx.AssessmentDate) + '-' + DATENAME(YEAR, PreviousItemsEx.AssessmentDate), ISNULL(InventoryItemAliasEx.AliasName, PreviousItemsEx.ItemNumber), 
                                       PreviousItemsEx.Prev_reg_nbr, PreviousItemsEx.Previous_item_number, PreviousItemsEx.DutyFreePaid, PreviousItemsEx.ApplicationSettingsId) AS PreviousItems ON 
                 Allocations.ApplicationSettingsId = PreviousItems.ApplicationSettingsId AND Allocations.DutyFreePaid = PreviousItems.DutyFreePaid AND Allocations.MonthYear = PreviousItems.MonthYear AND 
                 Allocations.ItemNumber = PreviousItems.ItemNumber AND Allocations.pCNumber = PreviousItems.pCNumber AND Allocations.pLineNumber = PreviousItems.pLineNumber AND ROUND(Allocations.pQtyAllocated, 2) 
                 > ROUND(ISNULL(PreviousItems.PiQty, 0) * Allocations.SalesFactor, 2)
WHERE (ROUND(Allocations.PiQuantity * Allocations.SalesFactor, 2) < ROUND(Allocations.pQtyAllocated, 2)) AND (Allocations.WarehouseError IS NULL) AND (ROUND(Allocations.pQtyAllocated / Allocations.SalesFactor, 2) > 0) 
                 AND (ROUND(Allocations.PiQuantity, 2) <> ROUND(ISNULL(PreviousItems.PiQty, 0), 2))
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[28] 4[30] 2[17] 3) )"
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
         Begin Table = "Allocations"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 216
               Right = 229
            End
            DisplayFlags = 280
            TopColumn = 4
         End
         Begin Table = "PreviousItems"
            Begin Extent = 
               Top = 0
               Left = 728
               Bottom = 155
               Right = 912
            End
            DisplayFlags = 280
            TopColumn = 2
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
         Width = 1505
         Width = 1505
         Width = 1505
         Width = 1505
         Width = 1505
         Width = 1505
         Width = 1505
         Width = 1505
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 5839
         Alias = 903
         Table = 1165
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1348
         SortOrder = 1414
         GroupBy = 1350
         Filter = 3207
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'DataCheck-Unexwarehoused Per month-year'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'DataCheck-Unexwarehoused Per month-year'
GO
