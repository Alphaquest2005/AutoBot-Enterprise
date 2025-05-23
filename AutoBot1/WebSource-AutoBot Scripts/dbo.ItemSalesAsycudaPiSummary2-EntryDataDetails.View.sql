USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ItemSalesAsycudaPiSummary2-EntryDataDetails]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[ItemSalesAsycudaPiSummary2-EntryDataDetails]
AS
SELECT   EntryDataDetails.ItemNumber, SUM(EntryDataDetails.Quantity) AS SalesQuantity, SUM(EntryDataDetails.QtyAllocated) AS SalesQTYAllocated, CONVERT(varchar(7), MAX(EntryDataEx.InvoiceDate), 126) AS MonthYear, 
                CASE WHEN isnull(EntryDataDetails.TaxAmount, isnull(EntryDataEx.Tax, 0)) <> 0 THEN 'Duty Paid' ELSE 'Duty Free' END AS DutyFreePaid, EntryDataEx.ApplicationSettingsId, 
                EntryDataEx.Type as EntryDataType
FROM      EntryDataDetails INNER JOIN
                EntryDataEx ON EntryDataDetails.EntryData_Id = EntryDataEx.EntryData_Id
WHERE   (NOT (EntryDataEx.Type IN (N'PO', N'OPS')))
GROUP BY EntryDataDetails.ItemNumber, CONVERT(varchar(7), EntryDataEx.InvoiceDate, 126), CASE WHEN isnull(EntryDataDetails.TaxAmount, isnull(EntryDataEx.Tax, 0)) <> 0 THEN 'Duty Paid' ELSE 'Duty Free' END, EntryDataEx.ApplicationSettingsId, 
                EntryDataEx.Type
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
         Begin Table = "EntryDataDetails"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 146
               Right = 280
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryDataEx"
            Begin Extent = 
               Top = 6
               Left = 318
               Bottom = 146
               Right = 559
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ItemSalesAsycudaPiSummary2-EntryDataDetails'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ItemSalesAsycudaPiSummary2-EntryDataDetails'
GO
