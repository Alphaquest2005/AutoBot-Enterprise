USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-CreateEx9]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[TODO-CreateEx9]
AS
SELECT dbo.EX9AsycudaSalesAllocations.ItemNumber, dbo.ApplicationSettings.ApplicationSettingsId, SUM(dbo.EX9AsycudaSalesAllocations.QtyAllocated) AS QtyAllocated, dbo.EX9AsycudaSalesAllocations.pQuantity, 
                 dbo.EX9AsycudaSalesAllocations.PreviousItem_Id
FROM    dbo.EX9AsycudaSalesAllocations INNER JOIN
                 dbo.ApplicationSettings ON dbo.EX9AsycudaSalesAllocations.ApplicationSettingsId = dbo.ApplicationSettings.ApplicationSettingsId AND 
                 dbo.EX9AsycudaSalesAllocations.pRegistrationDate >= dbo.ApplicationSettings.OpeningStockDate LEFT OUTER JOIN
                 dbo.AllocationErrors ON dbo.ApplicationSettings.ApplicationSettingsId = dbo.AllocationErrors.ApplicationSettingsId AND dbo.EX9AsycudaSalesAllocations.ItemNumber = dbo.AllocationErrors.ItemNumber
WHERE (dbo.EX9AsycudaSalesAllocations.PreviousItem_Id IS NOT NULL) AND (dbo.EX9AsycudaSalesAllocations.xBond_Item_Id = 0) AND (dbo.EX9AsycudaSalesAllocations.QtyAllocated IS NOT NULL) AND 
                 (dbo.EX9AsycudaSalesAllocations.EntryDataDetailsId IS NOT NULL) AND (dbo.EX9AsycudaSalesAllocations.Status IS NULL OR
                 dbo.EX9AsycudaSalesAllocations.Status = '') AND (ISNULL(dbo.EX9AsycudaSalesAllocations.DoNotAllocateSales, 0) <> 1) AND (ISNULL(dbo.EX9AsycudaSalesAllocations.DoNotAllocatePreviousEntry, 0) <> 1) AND 
                 (ISNULL(dbo.EX9AsycudaSalesAllocations.DoNotEX, 0) <> 1) AND (dbo.EX9AsycudaSalesAllocations.WarehouseError IS NULL)
				 --AND (dbo.EX9AsycudaSalesAllocations.DocumentType = 'IM7' OR dbo.EX9AsycudaSalesAllocations.DocumentType = 'OS7') 
				 AND (dbo.AllocationErrors.ItemNumber IS NULL) AND (dbo.ApplicationSettings.ApplicationSettingsId = 2) AND 
                 (dbo.EX9AsycudaSalesAllocations.InvoiceDate >= CONVERT(DATETIME, '2019-04-01 00:00:00', 102))
GROUP BY dbo.EX9AsycudaSalesAllocations.ItemNumber, dbo.ApplicationSettings.ApplicationSettingsId, dbo.EX9AsycudaSalesAllocations.pQuantity, dbo.EX9AsycudaSalesAllocations.PreviousItem_Id
HAVING (SUM(dbo.EX9AsycudaSalesAllocations.PiQuantity) < SUM(dbo.EX9AsycudaSalesAllocations.pQtyAllocated)) AND (SUM(dbo.EX9AsycudaSalesAllocations.QtyAllocated) > 0)
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[21] 4[40] 2[20] 3) )"
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
         Begin Table = "EX9AsycudaSalesAllocations"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 161
               Right = 342
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ApplicationSettings"
            Begin Extent = 
               Top = 6
               Left = 386
               Bottom = 161
               Right = 752
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AllocationErrors"
            Begin Extent = 
               Top = 6
               Left = 796
               Bottom = 119
               Right = 1034
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
      Begin ColumnWidths = 12
         Column = 1440
         Alias = 903
         Table = 1165
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1348
         SortOrder = 1414
         GroupBy = 1350
         Filter = 1348
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-CreateEx9'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-CreateEx9'
GO
