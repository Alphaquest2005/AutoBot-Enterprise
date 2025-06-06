USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ShipmentInvoiceEx]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO






CREATE VIEW [dbo].[ShipmentInvoiceEx]
AS
SELECT DISTINCT 
                TOP (100) PERCENT ShipmentInvoice.Id, ShipmentInvoice.InvoiceNo, ShipmentInvoice.InvoiceDate, ShipmentInvoiceTotals.SubTotal, ShipmentInvoice.InvoiceTotal, ShipmentInvoice.ApplicationSettingsId, ShipmentInvoiceTotals.ImportedLines, 
                ShipmentInvoice.EmailId, ShipmentInvoice.SupplierCode, ShipmentInvoice.SourceFile, Emails.EmailDate
FROM      ShipmentInvoiceTotals INNER JOIN
                ShipmentInvoice ON ShipmentInvoiceTotals.ShipmentInvoiceId = ShipmentInvoice.Id INNER JOIN
                Emails ON ShipmentInvoice.EmailId = Emails.EmailId LEFT OUTER JOIN
                ShipmentInvoiceExtraInfo ON ShipmentInvoice.Id = ShipmentInvoiceExtraInfo.InvoiceId
GROUP BY ShipmentInvoice.InvoiceNo, ShipmentInvoice.InvoiceDate, ShipmentInvoice.SubTotal, ShipmentInvoice.InvoiceTotal, ShipmentInvoice.ApplicationSettingsId, ShipmentInvoiceTotals.SubTotal, ShipmentInvoiceTotals.ImportedLines, 
                ShipmentInvoice.EmailId, ShipmentInvoice.Id, ShipmentInvoice.SupplierCode, ShipmentInvoice.SourceFile, Emails.EmailDate
ORDER BY ShipmentInvoiceTotals.SubTotal
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
         Begin Table = "ShipmentInvoiceDetailsEx"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 140
               Right = 266
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ShipmentInvoice"
            Begin Extent = 
               Top = 6
               Left = 310
               Bottom = 305
               Right = 548
            End
            DisplayFlags = 280
            TopColumn = 2
         End
         Begin Table = "ShipmentInvoiceExtraInfo"
            Begin Extent = 
               Top = 6
               Left = 592
               Bottom = 161
               Right = 792
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ShipmentInvoiceEx'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ShipmentInvoiceEx'
GO
