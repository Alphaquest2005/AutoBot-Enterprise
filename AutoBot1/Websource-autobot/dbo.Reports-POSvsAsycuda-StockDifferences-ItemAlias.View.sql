USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[Reports-POSvsAsycuda-StockDifferences-ItemAlias]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[Reports-POSvsAsycuda-StockDifferences-ItemAlias]
AS
SELECT        isnull(#OPS.InvoiceNo, 'Asycuda') as InvoiceNo, ISNULL(#OPS.ItemNumber, [#AsycudaData-ItemAlias].ItemNumber) AS ItemNumber,  ISNULL(#OPS.InventoryItemId, [#AsycudaData-ItemAlias].InventoryItemId) AS InventoryItemId, ISNULL(#OPS.ItemDescription, 
                         [#AsycudaData-ItemAlias].Description) AS Description, ISNULL(#OPS.Quantity, [#AsycudaData-ItemAlias].Quantity) AS Quantity,
						 #OPS.[OPS2zero-Adjustment],#OPS.Returns AS Returns, #OPS.Sales AS Sales,#OPS.QuantityOnHand as [OPS-QuantityOnHand], #OPS.Quantity AS [OPS-Quantity], 
                         ISNULL(#OPS.Cost, 0) AS OPSCost, [#AsycudaData-ItemAlias].Quantity AS [Asycuda-Quantity], [#AsycudaData-ItemAlias].PiQuantity AS [Asycuda-PiQuantity], ISNULL([#AsycudaData-ItemAlias].Cost, 0) AS AsycudaCost, 
                         COALESCE (#OPS.Quantity, 0) - COALESCE ([#AsycudaData-ItemAlias].Quantity, 0) AS Diff, ISNULL(#OPS.Cost, [#AsycudaData-ItemAlias].Cost) AS Cost, 
                         (COALESCE (#OPS.Quantity, 0) - COALESCE ([#AsycudaData-ItemAlias].Quantity, 0)) * ISNULL(#OPS.Cost, [#AsycudaData-ItemAlias].Cost) AS TotalCost
--into [#StockDifferences-ItemAlias]
FROM           dbo.[Reports-POSvsAsycuda-AsycudaSummary-Asycuda] as [#AsycudaData-ItemAlias] INNER JOIN
                        dbo.[Reports-POSvsAsycuda-OPS] as #OPS ON [#AsycudaData-ItemAlias].InventoryItemId = #OPS.InventoryItemId
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
         Begin Table = "#AsycudaData-ItemAlias"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 208
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "#OPS"
            Begin Extent = 
               Top = 6
               Left = 246
               Bottom = 136
               Right = 452
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Reports-POSvsAsycuda-StockDifferences-ItemAlias'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Reports-POSvsAsycuda-StockDifferences-ItemAlias'
GO
