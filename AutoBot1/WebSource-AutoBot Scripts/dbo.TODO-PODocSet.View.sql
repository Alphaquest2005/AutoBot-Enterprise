USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-PODocSet]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[TODO-PODocSet]
AS
SELECT AsycudaDocumentSetEx.AsycudaDocumentSetId, AsycudaDocumentSetEx.ApplicationSettingsId, AsycudaDocumentSetEx.Country_of_origin_code, AsycudaDocumentSetEx.Currency_Code, 
                 AsycudaDocumentSetEx.Manifest_Number, AsycudaDocumentSetEx.BLNumber, Document_Type.Type_of_declaration, Document_Type.Declaration_gen_procedure_code, 
                 AsycudaDocumentSetEx.Declarant_Reference_Number, AsycudaDocumentSetEx.TotalInvoices, AsycudaDocumentSetEx.DocumentsCount, AsycudaDocumentSetEx.ExpectedEntries, 
                 AsycudaDocumentSetEx.InvoiceTotal, AsycudaDocumentSetEx.LicenseLines, AsycudaDocumentSetEx.QtyLicensesRequired, AsycudaDocumentSetEx.TotalCIF, AsycudaDocumentSetEx.TotalFreight, 
                 AsycudaDocumentSetEx.ClassifiedLines, AsycudaDocumentSetEx.TotalLines, AsycudaDocumentSetEx.TotalPackages, AsycudaDocumentSetEx.TotalWeight, AsycudaDocumentSetEx.EntryPackages, 
                 AsycudaDocumentSetEx.FreightCurrencyCode, AsycudaDocumentSetEx.CurrencyRate, AsycudaDocumentSetEx.FreightCurrencyRate
FROM    SystemDocumentSets WITH (NOLOCK) RIGHT OUTER JOIN
                 AsycudaDocumentSetEntryData WITH (NOLOCK) INNER JOIN
                 AsycudaDocumentSetEx WITH (NOLOCK) INNER JOIN
                 Document_Type WITH (NOLOCK) ON AsycudaDocumentSetEx.Document_TypeId = Document_Type.Document_TypeId ON 
                 AsycudaDocumentSetEntryData.AsycudaDocumentSetId = AsycudaDocumentSetEx.AsycudaDocumentSetId ON SystemDocumentSets.Id = AsycudaDocumentSetEx.AsycudaDocumentSetId LEFT OUTER JOIN
                 AsycudaDocumentSetEntryData as AsycudaDocumentItemEntryDataDetails WITH (NOLOCK) ON AsycudaDocumentItemEntryDataDetails.EntryData_Id = AsycudaDocumentSetEntryData.EntryData_Id AND 
                 AsycudaDocumentItemEntryDataDetails.AsycudaDocumentSetId = AsycudaDocumentSetEx.AsycudaDocumentSetId LEFT OUTER JOIN
                 [TODO-DocumentsToDelete] ON AsycudaDocumentSetEx.AsycudaDocumentSetId = [TODO-DocumentsToDelete].AsycudaDocumentSetId
WHERE /*(ISNULL(AsycudaDocumentItemEntryDataDetails.ImportComplete, 0) = 0) AND*/ ([TODO-DocumentsToDelete].AsycudaDocumentSetId IS NULL) AND (SystemDocumentSets.Id IS NULL) 
GROUP BY AsycudaDocumentSetEx.AsycudaDocumentSetId, AsycudaDocumentSetEx.ApplicationSettingsId, AsycudaDocumentSetEx.Country_of_origin_code, AsycudaDocumentSetEx.Currency_Code, 
                 AsycudaDocumentSetEx.Manifest_Number, AsycudaDocumentSetEx.BLNumber, Document_Type.Type_of_declaration, Document_Type.Declaration_gen_procedure_code, 
                 AsycudaDocumentSetEx.Declarant_Reference_Number, AsycudaDocumentSetEx.TotalInvoices, AsycudaDocumentSetEx.DocumentsCount/*, AsycudaDocumentItemEntryDataDetails.ImportComplete*/, 
                 AsycudaDocumentSetEx.InvoiceTotal, AsycudaDocumentSetEx.LicenseLines, AsycudaDocumentSetEx.TotalCIF, AsycudaDocumentSetEx.QtyLicensesRequired, AsycudaDocumentSetEx.TotalFreight, 
                 AsycudaDocumentSetEx.ClassifiedLines, AsycudaDocumentSetEx.TotalLines, AsycudaDocumentSetEx.TotalPackages, AsycudaDocumentSetEx.TotalWeight, AsycudaDocumentSetEx.EntryPackages, 
                 AsycudaDocumentSetEx.FreightCurrencyCode, AsycudaDocumentSetEx.CurrencyRate, AsycudaDocumentSetEx.FreightCurrencyRate, AsycudaDocumentSetEx.ExpectedEntries
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[47] 4[20] 2[9] 3) )"
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
         Begin Table = "AsycudaDocumentSet_Attachments"
            Begin Extent = 
               Top = 11
               Left = 992
               Bottom = 207
               Right = 1337
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentItemEntryDataDetails"
            Begin Extent = 
               Top = 179
               Left = 1593
               Bottom = 357
               Right = 1816
            End
            DisplayFlags = 280
            TopColumn = 2
         End
         Begin Table = "AsycudaDocumentSetEntryData"
            Begin Extent = 
               Top = 220
               Left = 877
               Bottom = 354
               Right = 1177
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentSetEx"
            Begin Extent = 
               Top = 1
               Left = 430
               Bottom = 372
               Right = 718
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Document_Type"
            Begin Extent = 
               Top = 26
               Left = 34
               Bottom = 160
               Right = 328
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "TODO-SubmitInadequatePackages"
            Begin Extent = 
               Top = 164
               Left = 44
               Bottom = 319
               Right = 332
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Attachments"
            Begin Extent = 
               Top = 26
             ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-PODocSet'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'  Left = 1401
               Bottom = 200
               Right = 1600
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "SystemDocumentSets"
            Begin Extent = 
               Top = 113
               Left = 918
               Bottom = 205
               Right = 1118
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
      Begin ColumnWidths = 15
         Width = 284
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 2448
         Width = 2422
         Width = 1309
         Width = 1309
         Width = 2016
         Width = 1964
         Width = 1309
         Width = 2356
         Width = 1309
         Width = 2657
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 6506
         Alias = 903
         Table = 5184
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1348
         SortOrder = 1414
         GroupBy = 1350
         Filter = 11009
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-PODocSet'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-PODocSet'
GO
