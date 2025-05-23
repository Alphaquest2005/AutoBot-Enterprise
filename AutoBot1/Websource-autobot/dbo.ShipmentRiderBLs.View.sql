USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ShipmentRiderBLs]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO









CREATE VIEW [dbo].[ShipmentRiderBLs]
AS
SELECT distinct  cast(row_number() OVER ( ORDER BY ShipmentRider.Id) AS int) AS id,
			dbo.ShipmentRider.Id as RiderId, ShipmentRiderDetails.id as RiderDetailId,ShipmentBL.Id as BLId, dbo.ShipmentRider.ETA, dbo.ShipmentBL.BLNumber, dbo.ShipmentBLDetails.Quantity,  dbo.ShipmentBLDetails.Marks, dbo.ShipmentRiderDetails.WarehouseCode
			, ShipmentBLDetails.Id as BLDetailId
FROM    dbo.ShipmentRider INNER JOIN
                 dbo.ShipmentRiderDetails ON dbo.ShipmentRider.Id = dbo.ShipmentRiderDetails.RiderId INNER JOIN
                 dbo.ShipmentBLDetails INNER JOIN
                 dbo.ShipmentBL ON dbo.ShipmentBLDetails.BLId = dbo.ShipmentBL.Id 
				 ON (dbo.ShipmentRiderDetails.WarehouseCode = dbo.ShipmentBLDetails.Marks or CHARINDEX(ShipmentBLDetails.Marks, ShipmentRiderDetails.WarehouseCode) > 0 or  CHARINDEX(ShipmentRiderDetails.WarehouseCode, ShipmentBLDetails.Marks) > 0) and ShipmentRider.ApplicationSettingsId = ShipmentBL.ApplicationSettingsId
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
         Begin Table = "ShipmentRider"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 161
               Right = 230
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ShipmentRiderDetails"
            Begin Extent = 
               Top = 6
               Left = 274
               Bottom = 161
               Right = 468
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ShipmentBLDetails"
            Begin Extent = 
               Top = 6
               Left = 512
               Bottom = 161
               Right = 696
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ShipmentBL"
            Begin Extent = 
               Top = 6
               Left = 740
               Bottom = 161
               Right = 924
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ShipmentRiderBLs'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ShipmentRiderBLs'
GO
