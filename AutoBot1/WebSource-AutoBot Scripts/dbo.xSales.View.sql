USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[xSales]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[xSales]
AS
SELECT        dbo.xSalesDetails.Id, dbo.xSalesDetails.xSalesFileId, dbo.xSalesDetails.FileLine, dbo.xSalesDetails.Line, dbo.xSalesDetails.Date, dbo.xSalesDetails.InvoiceNo, dbo.xSalesDetails.CustomerName, 
                         dbo.xSalesDetails.ItemNumber, dbo.xSalesDetails.ItemDescription, dbo.xSalesDetails.TariffCode, dbo.xSalesDetails.SalesQuantity, dbo.xSalesDetails.SalesFactor, dbo.xSalesDetails.xQuantity, dbo.xSalesDetails.Price, 
                         dbo.xSalesDetails.DutyFreePaid, dbo.xSalesDetails.pCNumber, dbo.xSalesDetails.pLineNumber, dbo.xSalesDetails.pRegDate, dbo.xSalesDetails.CIFValue, dbo.xSalesDetails.DutyLiablity, dbo.xSalesDetails.Comment, 
                         dbo.PreviousItemsEx.CNumber, dbo.PreviousItemsEx.RegistrationDate, dbo.xSalesFiles.ApplicationSettingsId, dbo.EntryDataDetails.EntryDataDetailsId, dbo.EntryDataDetails.LineNumber AS SalesLineNumber, 
                         dbo.PreviousItemsEx.AsycudaDocumentItemId AS xItemId, dbo.PreviousItemsEx.PreviousDocumentItemId AS pItemId
FROM            dbo.xSalesDetails INNER JOIN
                         dbo.PreviousItemsEx ON dbo.xSalesDetails.pLineNumber = dbo.PreviousItemsEx.pLineNumber AND dbo.xSalesDetails.pCNumber = dbo.PreviousItemsEx.Prev_reg_nbr AND 
                         dbo.xSalesDetails.xQuantity = dbo.PreviousItemsEx.Suplementary_Quantity AND dbo.xSalesDetails.DutyFreePaid = dbo.PreviousItemsEx.DutyFreePaid AND 
                         dbo.xSalesDetails.Line = dbo.PreviousItemsEx.Current_item_number INNER JOIN
                         dbo.xSalesFiles ON dbo.xSalesDetails.xSalesFileId = dbo.xSalesFiles.Id AND dbo.PreviousItemsEx.ApplicationSettingsId = dbo.xSalesFiles.ApplicationSettingsId INNER JOIN
                         dbo.EntryDataDetails ON dbo.xSalesDetails.InvoiceNo = dbo.EntryDataDetails.EntryDataId AND dbo.xSalesDetails.ItemNumber = dbo.EntryDataDetails.ItemNumber AND 
                         dbo.xSalesDetails.SalesQuantity = dbo.EntryDataDetails.Quantity INNER JOIN
                         dbo.EntryData ON dbo.EntryDataDetails.EntryData_Id = dbo.EntryData.EntryData_Id AND dbo.xSalesFiles.ApplicationSettingsId = dbo.EntryData.ApplicationSettingsId
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
         Begin Table = "xSalesDetails"
            Begin Extent = 
               Top = 6
               Left = 38
               Bottom = 136
               Right = 231
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "PreviousItemsEx"
            Begin Extent = 
               Top = 6
               Left = 269
               Bottom = 136
               Right = 535
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xSalesFiles"
            Begin Extent = 
               Top = 6
               Left = 573
               Bottom = 136
               Right = 775
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryDataDetails"
            Begin Extent = 
               Top = 6
               Left = 813
               Bottom = 136
               Right = 1029
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryData"
            Begin Extent = 
               Top = 6
               Left = 1067
               Bottom = 136
               Right = 1269
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
         O' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'xSales'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'r = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'xSales'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'xSales'
GO
