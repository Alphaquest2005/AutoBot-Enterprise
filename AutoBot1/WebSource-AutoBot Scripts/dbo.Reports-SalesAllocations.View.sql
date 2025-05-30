USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[Reports-SalesAllocations]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[Reports-SalesAllocations]
AS
SELECT        dbo.AsycudaSalesAllocationsEx.Status, dbo.AsycudaSalesAllocationsEx.DutyFreePaid, dbo.AsycudaSalesAllocationsEx.InvoiceNo, dbo.AsycudaSalesAllocationsEx.InvoiceDate, dbo.AsycudaSalesAllocationsEx.ItemNumber, 
                         dbo.AsycudaSalesAllocationsEx.ItemDescription, dbo.AsycudaSalesAllocationsEx.QtyAllocated, dbo.AsycudaSalesAllocationsEx.CustomerName, dbo.AsycudaSalesAllocationsEx.SalesQuantity, 
                         dbo.AsycudaSalesAllocationsEx.SalesQtyAllocated, dbo.AsycudaSalesAllocationsEx.Cost, dbo.AsycudaSalesAllocationsEx.AllocatedValue, dbo.AsycudaSalesAllocationsEx.TotalValue, 
                         dbo.AsycudaSalesAllocationsEx.pCNumber, dbo.AsycudaSalesAllocationsEx.pRegistrationDate, dbo.AsycudaSalesAllocationsEx.pReferenceNumber, dbo.AsycudaSalesAllocationsEx.pItemNumber, 
                         dbo.AsycudaSalesAllocationsEx.pQuantity, dbo.AsycudaSalesAllocationsEx.pQtyAllocated, dbo.AsycudaSalesAllocationsEx.PiQuantity, dbo.AsycudaSalesAllocationsEx.pExpiryDate, 
                         dbo.AsycudaSalesAllocationsEx.pLineNumber, dbo.AsycudaSalesAllocationsEx.pIsAssessed, dbo.AsycudaSalesAllocationsEx.SalesFactor, dbo.AsycudaSalesAllocationsEx.WarehouseError, 
                         dbo.AsycudaSalesAllocationsEx.Total_CIF_itm, dbo.AsycudaSalesAllocationsEx.DutyLiability, dbo.AsycudaSalesAllocationsEx.xCNumber, dbo.AsycudaSalesAllocationsEx.xRegistrationDate, 
                         dbo.AsycudaSalesAllocationsEx.xReferenceNumber, dbo.AsycudaSalesAllocationsEx.xLineNumber, dbo.AsycudaSalesAllocationsEx.xQuantity
FROM            dbo.[Reports-POSvsAsycudaData] CROSS JOIN
                         dbo.AsycudaSalesAllocationsEx
WHERE        (dbo.AsycudaSalesAllocationsEx.InvoiceDate >= dbo.[Reports-POSvsAsycudaData].StartDate AND dbo.AsycudaSalesAllocationsEx.InvoiceDate <= dbo.[Reports-POSvsAsycudaData].EndDate)
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[39] 4[27] 2[16] 3) )"
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
         Begin Table = "Reports-POSvsAsycudaData"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 119
               Right = 208
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaSalesAllocationsEx"
            Begin Extent = 
               Top = 0
               Left = 235
               Bottom = 338
               Right = 474
            End
            DisplayFlags = 280
            TopColumn = 29
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Reports-SalesAllocations'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'Reports-SalesAllocations'
GO
