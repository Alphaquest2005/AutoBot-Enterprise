USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-C71FromEntryData]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[TODO-C71FromEntryData]
AS
SELECT 'FOB' AS Code, 0 AS ASYCUDA_Id, AsycudaDocumentSetEx.ApplicationSettingsId, ISNULL(EntryDataEx.InvoiceTotal, 0) AS InvoiceTotal, EntryDataEx.ExpectedTotal, EntryDataEx.Type, EntryDataEx.InvoiceDate, 
                 EntryDataEx.InvoiceNo, ISNULL(EntryDataEx.Currency, AsycudaDocumentSetEx.Currency_Code) AS Currency, EntryDataEx.SupplierCode, AsycudaDocumentSetEx.AsycudaDocumentSetId, 
                 AsycudaDocumentSetEx.CurrencyRate
FROM    AsycudaDocumentSetEntryData INNER JOIN
                 [TODO-C71FromEntryData-AsycudaDocumentSetEx] as AsycudaDocumentSetEx INNER JOIN
                 EntryDataEx ON AsycudaDocumentSetEx.ApplicationSettingsId = EntryDataEx.ApplicationSettingsId ON AsycudaDocumentSetEntryData.AsycudaDocumentSetId = AsycudaDocumentSetEx.AsycudaDocumentSetId AND
                  AsycudaDocumentSetEntryData.EntryData_Id = EntryDataEx.EntryData_Id LEFT OUTER JOIN
                 AsycudaDocumentEntryData ON EntryDataEx.EntryData_Id = AsycudaDocumentEntryData.EntryData_Id inner join 
				 Suppliers on entrydataex.SupplierCode = Suppliers.SupplierCode
WHERE (AsycudaDocumentEntryData.Id IS NULL) AND (AsycudaDocumentSetEx.ImportedInvoices = AsycudaDocumentSetEx.TotalInvoices) and Suppliers.CountryCode is not null
GROUP BY AsycudaDocumentSetEx.ApplicationSettingsId, EntryDataEx.InvoiceTotal, EntryDataEx.Type, EntryDataEx.InvoiceDate, EntryDataEx.InvoiceNo, EntryDataEx.Currency, 
                 AsycudaDocumentSetEx.Currency_Code, EntryDataEx.SupplierCode, EntryDataEx.ExpectedTotal, AsycudaDocumentSetEx.AsycudaDocumentSetId, AsycudaDocumentSetEx.CurrencyRate
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
         Begin Table = "AsycudaDocumentSetEntryData"
            Begin Extent = 
               Top = 15
               Left = 60
               Bottom = 149
               Right = 315
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentSetEx"
            Begin Extent = 
               Top = 12
               Left = 557
               Bottom = 167
               Right = 845
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "TODO-PODocSet"
            Begin Extent = 
               Top = 150
               Left = 44
               Bottom = 305
               Right = 354
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryDataEx"
            Begin Extent = 
               Top = 6
               Left = 1029
               Bottom = 161
               Right = 1267
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentEntryData"
            Begin Extent = 
               Top = 6
               Left = 1311
               Bottom = 140
               Right = 1547
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
      Begin ColumnWidths = 11
         Width = 284
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
      End
   End
  ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-C71FromEntryData'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N' Begin CriteriaPane = 
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-C71FromEntryData'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-C71FromEntryData'
GO
