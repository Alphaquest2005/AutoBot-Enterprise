USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[Reports-POSvsAsycuda-SalesData]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[Reports-POSvsAsycuda-SalesData]
AS

SELECT        EntryData.EntryDataId as InvoiceNo, EntryData.EntryDataDate as Date, entrydatadetails.ItemNumber AS ItemNumber, EntryDataDetails.InventoryItemId, EntryDataDetails.ItemDescription, 
                         SUM(EntryDataDetails.Quantity) AS Quantity, AVG(EntryDataDetails.Cost) AS Cost
--into #salesData
FROM            EntryData_Sales with (nolock) INNER JOIN
                         EntryData with (nolock) ON EntryData_Sales.EntryData_Id = EntryData.EntryData_Id INNER JOIN
                         EntryDataDetails with (nolock) ON EntryData.EntryDataId = EntryDataDetails.EntryDataId 
                        inner join [Reports-POSvsAsycudaData] on EntryData.EntryDataDate BETWEEN [Reports-POSvsAsycudaData].StartDate AND [Reports-POSvsAsycudaData].EndDate
						inner join ApplicationSettings on EntryData.ApplicationSettingsId = ApplicationSettings.ApplicationSettingsId 
GROUP BY entrydatadetails.ItemNumber, EntryDataDetails.ItemDescription, EntryData.EntryDataId, EntryData.EntryDataDate,EntryDataDetails.InventoryItemId

--SELECT        dbo.EntryData.EntryDataId AS InvoiceNo, dbo.EntryData.EntryDataDate AS Date, CASE WHEN AliasName IS NOT NULL THEN inventoryitemalias.AliasName ELSE entrydatadetails.ItemNumber END AS ItemNumber, 
--                         dbo.EntryDataDetails.ItemDescription, SUM(dbo.EntryDataDetails.Quantity) AS Quantity, AVG(dbo.EntryDataDetails.Cost) AS Cost, EntryDataDetails.InventoryItemId 
--FROM            dbo.EntryData_Sales INNER JOIN
--                         dbo.EntryData ON dbo.EntryData_Sales.EntryData_Id = dbo.EntryData.EntryData_Id INNER JOIN
--                         dbo.EntryDataDetails ON dbo.EntryData.EntryData_Id = dbo.EntryDataDetails.EntryData_Id LEFT OUTER JOIN
--                         dbo.InventoryItemAlias ON dbo.EntryDataDetails.InventoryItemId = dbo.InventoryItemAlias.InventoryItemId CROSS JOIN
--                         dbo.[Reports-POSvsAsycudaData]
--WHERE        (dbo.EntryData.EntryDataDate BETWEEN dbo.[Reports-POSvsAsycudaData].StartDate AND dbo.[Reports-POSvsAsycudaData].EndDate)
--GROUP BY CASE WHEN AliasName IS NOT NULL THEN inventoryitemalias.AliasName ELSE entrydatadetails.ItemNumber END, dbo.EntryDataDetails.ItemDescription, dbo.EntryData.EntryDataId, dbo.EntryData.EntryDataDate, EntryDataDetails.InventoryItemId 
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[29] 4[31] 2[20] 3) )"
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
         Begin Table = "EntryData_Sales"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 227
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryData"
            Begin Extent = 
               Top = 6
               Left = 265
               Bottom = 136
               Right = 474
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryDataDetails"
            Begin Extent = 
               Top = 6
               Left = 512
               Bottom = 136
               Right = 713
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "InventoryItemAlias"
            Begin Extent = 
               Top = 6
               Left = 751
               Bottom = 119
               Right = 937
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Reports-POSvsAsycudaData"
            Begin Extent = 
               Top = 108
               Left = 1026
               Bottom = 221
               Right = 1212
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
        ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Reports-POSvsAsycuda-SalesData'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N' Column = 1440
         Alias = 900
         Table = 1170
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1350
         SortOrder = 1410
         GroupBy = 1350
         Filter = 5805
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Reports-POSvsAsycuda-SalesData'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Reports-POSvsAsycuda-SalesData'
GO
