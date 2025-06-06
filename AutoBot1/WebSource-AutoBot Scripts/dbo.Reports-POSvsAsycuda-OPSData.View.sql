USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[Reports-POSvsAsycuda-OPSData]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE VIEW [dbo].[Reports-POSvsAsycuda-OPSData]
AS


SELECT        Coalesce(OPS.OPSNumber, (case when sales.ItemNumber is null then Null else 'Sales' end),(case when returns.ItemNumber is null then Null else 'Returns' end),(case when adjustments.ItemNumber is null then Null else 'Adjustments' end)) AS InvoiceNo, Coalesce(OPS.ItemNumber, sales.ItemNumber, returns.ItemNumber,adjustments.ItemNumber) AS ItemNumber,  Coalesce(OPS.InventoryItemId, sales.InventoryItemId, returns.InventoryItemId,adjustments.InventoryItemId) AS InventoryItemId, Coalesce(OPS.ItemDescription, sales.ItemDescription, returns.ItemDescription, adjustments.ItemDescription) AS ItemDescription, CAST(Coalesce(OPS.Cost, sales.Cost, returns.cost,adjustments.Cost) AS float) AS Cost, sales.Quantity AS Sales, 
                         OPS.Quantity AS QuantityOnHand, returns.Quantity as Returns, adjustments.Quantity as Adjustments
--into #OPSData
FROM            (SELECT        [#Sales].ItemNumber, InventoryItemId, [#Sales].Quantity, [#Sales].ItemDescription, [#Sales].Cost
                          FROM          [dbo].[Reports-POSvsAsycuda-Sales] as [#Sales]) AS sales FULL OUTER JOIN
                             (SELECT        EntryData_OpeningStock_1.OPSNumber, sum(EntryDataDetails_1.Quantity) as Quantity, EntryDataDetails_1.ItemNumber,EntryDataDetails_1.InventoryItemId,  EntryDataDetails_1.ItemDescription, CAST(EntryDataDetails_1.Cost AS float) AS Cost
                               FROM            EntryData_OpeningStock AS EntryData_OpeningStock_1  with (nolock) INNER JOIN
                                                         EntryDataDetails AS EntryDataDetails_1  with (nolock) ON EntryData_OpeningStock_1.OPSNumber = EntryDataDetails_1.EntryDataId
											   inner join EntryData on EntryData_OpeningStock_1.EntryData_Id = EntryData.EntryData_Id
											   inner join [Reports-POSvsAsycudaData] on EntryData_OpeningStock_1.OPSNumber = [Reports-POSvsAsycudaData].OPSNumber and EntryData.ApplicationSettingsId = [Reports-POSvsAsycudaData].ApplicationSettingsId
                               --WHERE   (EntryData.ApplicationSettingsId = @ApplicationSettingsId) and       (EntryData_OpeningStock_1.OPSNumber = @OPSNumber) 
							    -- WHERE   (EntryData.ApplicationSettingsId = 2) and       (EntryData_OpeningStock_1.OPSNumber = 'OPS-Feb-28-2023') and ItemNumber = 'REF/RF0A3YMHBK2'
                               GROUP BY EntryData_OpeningStock_1.OPSNumber, EntryDataDetails_1.ItemNumber,EntryDataDetails_1.InventoryItemId ,  EntryDataDetails_1.ItemDescription, CAST(EntryDataDetails_1.Cost AS float)) AS OPS ON 
                         sales.InventoryItemId = OPS.InventoryItemId ---- changed from itemnumber to inventoryitemid because of "CRB/HIF-SR290" has 2 iventoryitemid's 21234, & 20096
						 full outer join (SELECT        [#Returns].ItemNumber, InventoryItemId, [#Returns].Quantity, [#Returns].ItemDescription, [#Returns].Cost
                          FROM           dbo.[Reports-POSvsAsycuda-Returns] as [#Returns]) AS returns on returns.InventoryItemId = OPS.InventoryItemId
						 full outer join (SELECT        [#adjustments].ItemNumber, InventoryItemId, [#adjustments].Quantity, [#adjustments].ItemDescription, [#adjustments].Cost
                          FROM         dbo.[Reports-POSvsAsycuda-Adjustments] as [#adjustments]) AS adjustments on adjustments.InventoryItemId = OPS.InventoryItemId
GROUP BY sales.Cost, sales.ItemNumber, OPS.ItemNumber, sales.Quantity, OPS.Quantity, OPS.OPSNumber, ops.ItemDescription, sales.ItemDescription, ops.Cost, sales.cost, returns.ItemNumber, returns.ItemDescription, returns.Cost, returns.Quantity,
	 adjustments.ItemNumber, adjustments.ItemDescription, adjustments.Cost, adjustments.Quantity,  Coalesce(OPS.InventoryItemId, sales.InventoryItemId, returns.InventoryItemId,adjustments.InventoryItemId)


--SELECT        COALESCE (OPS.EntryDataId, (CASE WHEN sales.ItemNumber IS NULL THEN NULL ELSE 'Sales' END), (CASE WHEN returns.ItemNumber IS NULL THEN NULL ELSE 'Returns' END), 
--                         (CASE WHEN adjustments.ItemNumber IS NULL THEN NULL ELSE 'Adjustments' END)) AS InvoiceNo, COALESCE (OPS.ItemNumber, sales.ItemNumber, [returns].ItemNumber, adjustments.ItemNumber) AS ItemNumber, 
--                         ISNULL(OPS.Quantity, 0) - ISNULL(sales.Quantity, 0) + ISNULL([returns].Quantity, 0) + ISNULL(adjustments.Quantity, 0) AS Quantity, COALESCE (OPS.ItemDescription, sales.ItemDescription, [returns].ItemDescription, 
--                         adjustments.ItemDescription) AS ItemDescription, CAST(COALESCE (OPS.Cost, sales.Cost, [returns].Cost, adjustments.Cost) AS float) AS Cost, sales.Quantity AS Sales, OPS.Quantity AS QuantityOnHand, 
--                         [returns].Quantity AS [Returns], adjustments.Quantity AS Adjustments, COALESCE (OPS.InventoryItemId, sales.InventoryItemId, [returns].InventoryItemId, adjustments.InventoryItemId) AS InventoryItemId
--FROM            (SELECT        ItemNumber, Quantity, ItemDescription, Cost,InventoryItemId
--                          FROM            dbo.[Reports-POSvsAsycuda-Sales] AS [#Sales]) AS sales FULL OUTER JOIN
--                             (SELECT        EntryDataDetails_1.EntryDataId, EntryDataDetails_1.Quantity, EntryDataDetails_1.ItemNumber, EntryDataDetails_1.ItemDescription, CAST(EntryDataDetails_1.Cost AS float) AS Cost, EntryDataDetails_1.InventoryItemId 
--                               FROM            dbo.EntryData_OpeningStock AS EntryData_OpeningStock_1 INNER JOIN
--                                                         dbo.EntryDataDetails AS EntryDataDetails_1 ON EntryData_OpeningStock_1.EntryData_Id = EntryDataDetails_1.EntryData_Id INNER JOIN
--                                                         dbo.[Reports-POSvsAsycudaData] ON EntryData_OpeningStock_1.EntryData_Id = dbo.[Reports-POSvsAsycudaData].EntryData_Id
--                               GROUP BY EntryDataDetails_1.EntryDataId, EntryDataDetails_1.ItemNumber, EntryDataDetails_1.Quantity, EntryDataDetails_1.ItemDescription, CAST(EntryDataDetails_1.Cost AS float), EntryDataDetails_1.InventoryItemId) AS OPS ON 
--                         sales.InventoryItemId = OPS.InventoryItemId FULL OUTER JOIN
--                             (SELECT        ItemNumber, Quantity, ItemDescription, Cost, InventoryItemId
--                               FROM            dbo.[Reports-POSvsAsycuda-Returns] AS [#Returns]) AS [returns] ON [returns].InventoryItemId = OPS.InventoryItemId FULL OUTER JOIN
--                             (SELECT        ItemNumber, Quantity, ItemDescription, Cost, InventoryItemId
--                               FROM            dbo.[Reports-POSvsAsycuda-Adjustments] AS [#adjustments]) AS adjustments ON adjustments.InventoryItemId = OPS.InventoryItemId
--GROUP BY sales.Cost, sales.ItemNumber, OPS.ItemNumber, sales.Quantity, OPS.Quantity, OPS.EntryDataId, OPS.ItemDescription, sales.ItemDescription, OPS.Cost, sales.Cost, [returns].ItemNumber, [returns].ItemDescription, 
--                         [returns].Cost, [returns].Quantity, adjustments.ItemNumber, adjustments.ItemDescription, adjustments.Cost, adjustments.Quantity, OPS.InventoryItemId, sales.InventoryItemId, [returns].InventoryItemId, adjustments.InventoryItemId
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[29] 4[18] 2[43] 3) )"
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
         Begin Table = "sales"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 227
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "OPS"
            Begin Extent = 
               Top = 6
               Left = 265
               Bottom = 136
               Right = 454
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "returns"
            Begin Extent = 
               Top = 6
               Left = 492
               Bottom = 136
               Right = 681
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "adjustments"
            Begin Extent = 
               Top = 6
               Left = 719
               Bottom = 136
               Right = 908
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
   End' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Reports-POSvsAsycuda-OPSData'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Reports-POSvsAsycuda-OPSData'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Reports-POSvsAsycuda-OPSData'
GO
