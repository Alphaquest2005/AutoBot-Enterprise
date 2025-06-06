USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[Reports-POSvsAsycuda-Results]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE VIEW [dbo].[Reports-POSvsAsycuda-Results]
AS
SELECT       coalesce([#StockDifferences].InvoiceNo, [#OPS to Zero - Overs].InvoiceNo, [#P2O-Returns].InvoiceNo) as InvoiceNo,
			 coalesce([#StockDifferences].ItemNumber,[#OPS to Zero - Overs].ItemNumber, [#P2O-Returns].ItemNumber) as ItemNumber,
			 coalesce([#StockDifferences].InventoryItemId,[#OPS to Zero - Overs].InventoryItemId, [#P2O-Returns].InventoryItemId) as InventoryItemId,
			 max(coalesce([#StockDifferences].Description,[#OPS to Zero - Overs].ItemDescription, [#P2O-Returns].ItemDescription)) as Description,
			 avg(isnull([#StockDifferences].Quantity,0)) +  avg(isnull(Adj.Quantity,0)) + avg(isnull([#OPS to Zero - Overs].Quantity,0)) + avg(isnull([#P2O-Returns].Quantity,0)) AS Quantity,
			 avg(isnull([#OPS to Zero - Overs].Quantity,0)) as OPS2Zero,
			 avg(isnull(Adj.Quantity,0)) as Adjustments,
			 avg(isnull([#StockDifferences].Sales,0)) as [OPS-Sales],
			 avg(isnull([#StockDifferences].Returns,0)) as [OPS-Returns],
			 avg(isnull([#StockDifferences].[OPS2zero-Adjustment],0)) as [OPS2zero-Adjustment],
			 avg(isnull([#P2O-Returns].Quantity,0)) as Returns,
			 isnull(avg([OPS-QuantityOnHand]),0) AS [OPS-QuantityOnHand],
			 AVG(isnull(OPSCost,0)) AS OPSCost,
			 sum(isnull([Asycuda-PiQuantity],0)) AS [Asycuda-PiQuantity],
			 AVG(isnull(AsycudaCost,0)) AS AsycudaCost,
			 (isnull(avg([#StockDifferences].[OPS-Quantity]),0) +  avg(isnull(Adj.Quantity,0)) + avg(isnull([#P2O-Returns].Quantity,0)) + avg(isnull([#OPS to Zero - Overs].Quantity,0))) AS [OPS-Quantity],
			 sum(isnull([Asycuda-Quantity],0)) AS [Asycuda-Quantity],
			 (sum(isnull([Asycuda-Quantity],0)) - sum(isnull([Asycuda-PiQuantity],0))) AS [Asycuda-QuantityOnHand],
			 isnull(avg([OPS-QuantityOnHand]),0) -(sum(isnull([Asycuda-Quantity],0)) - sum(isnull([Asycuda-PiQuantity],0))) AS Diff,
			 AVG(isnull([#StockDifferences].Cost,0)) AS Cost,
			 AVG(isnull(TotalCost,0)) AS TotalCost
--into #Results
FROM           dbo.[Reports-POSvsAsycuda-StockDifferences] as [#StockDifferences] left outer join dbo.[Reports-POSvsAsycuda-P2O-Returns] as [#P2O-Returns] on [#StockDifferences].InventoryItemId = [#P2O-Returns].InventoryItemId
			    left outer join dbo.[Reports-POSvsAsycuda-OPS to Zero - Overs] as [#OPS to Zero - Overs] on [#StockDifferences].ItemNumber = [#OPS to Zero - Overs].ItemNumber
				left outer join (select * from dbo.[Reports-POSvsAsycuda-Adjustments] as [#adjustments] where isnull([#adjustments].IsReconciled,0) <> 1) as Adj on [#StockDifferences].ItemNumber = Adj.ItemNumber
GROUP BY coalesce([#StockDifferences].InvoiceNo, [#OPS to Zero - Overs].InvoiceNo, [#P2O-Returns].InvoiceNo),  coalesce([#StockDifferences].ItemNumber,[#OPS to Zero - Overs].ItemNumber, [#P2O-Returns].ItemNumber), coalesce([#StockDifferences].InventoryItemId,[#OPS to Zero - Overs].InventoryItemId, [#P2O-Returns].InventoryItemId)
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[41] 4[20] 2[18] 3) )"
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
         Begin Table = "#StockDifferences"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 239
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
      Begin ColumnWidths = 12
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Reports-POSvsAsycuda-Results'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Reports-POSvsAsycuda-Results'
GO
