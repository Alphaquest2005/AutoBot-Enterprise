USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-PODocSetToAssess]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[TODO-PODocSetToAssess]
AS

SELECT [TODO-PODocSetToExport].AsycudaDocumentSetId, [TODO-PODocSetToExport].ApplicationSettingsId, [TODO-PODocSetToExport].Country_of_origin_code, [TODO-PODocSetToExport].Currency_Code, 
                 [TODO-PODocSetToExport].Manifest_Number, [TODO-PODocSetToExport].BLNumber, [TODO-PODocSetToExport].Type_of_declaration, [TODO-PODocSetToExport].Declaration_gen_procedure_code, 
                 [TODO-PODocSetToExport].Declarant_Reference_Number, [TODO-PODocSetToExport].TotalInvoices, [TODO-PODocSetToExport].DocumentsCount, [TODO-PODocSetToExport].InvoiceTotal, 
                 [TODO-PODocSetToExport].LicenseLines, [TODO-PODocSetToExport].TotalCIF, AsycudaDocumentSetEntryDataCIF.InvoiceTotal AS EntryDataInvoiceTotal, [TODO-PODocSetToExport].NeedC71, 
                 [TODO-PODocSetToExport].NeedLicense, [TODO-PODocSetToExport].HasC71, [TODO-PODocSetToExport].HasLicense, [TODO-PODocSetToExport].TotalFreight, 
                 AsycudaDocumentSetFreight.TotalFreight AS GeneratedFreight, [TODO-PODocSetToExport].ClassifiedLines, [TODO-PODocSetToExport].TotalLines, AsycudaDocumentSetLines.Lines AS GeneratedLines, 
                 COUNT(DISTINCT AsycudaDocumentAttachments.Id) AS DocumentAttachments, COUNT(DISTINCT AsycudaDocumentAttachments.ASYCUDA_Id) AS AttachedDocuments
FROM    [TODO-PODocSetToExport] INNER JOIN
                 AsycudaDocumentSetLines ON [TODO-PODocSetToExport].AsycudaDocumentSetId = AsycudaDocumentSetLines.AsycudaDocumentSetId AND 
                 [TODO-PODocSetToExport].TotalLines = AsycudaDocumentSetLines.Lines INNER JOIN
                 AsycudaDocumentSetPackages ON [TODO-PODocSetToExport].AsycudaDocumentSetId = AsycudaDocumentSetPackages.AsycudaDocumentSetId AND 
                 [TODO-PODocSetToExport].EntryPackages = AsycudaDocumentSetPackages.Packages INNER JOIN
                 AsycudaDocumentAttachments ON [TODO-PODocSetToExport].AsycudaDocumentSetId = AsycudaDocumentAttachments.AsycudaDocumentSetId LEFT OUTER JOIN
                 AsycudaDocumentSetEntryDataCIF ON [TODO-PODocSetToExport].AsycudaDocumentSetId = AsycudaDocumentSetEntryDataCIF.AsycudaDocumentSetId LEFT OUTER JOIN
                 AsycudaDocumentSetFreight ON [TODO-PODocSetToExport].AsycudaDocumentSetId = AsycudaDocumentSetFreight.AsycudaDocumentSetId LEFT OUTER JOIN
                 AsycudaDocumentSetTotals ON [TODO-PODocSetToExport].AsycudaDocumentSetId = AsycudaDocumentSetTotals.AsycudaDocumentSetId LEFT OUTER JOIN
                 [TODO-PODocSetAttachementErrors] ON [TODO-PODocSetToExport].AsycudaDocumentSetId = [TODO-PODocSetAttachementErrors].AsycudaDocumentSetId
GROUP BY [TODO-PODocSetToExport].AsycudaDocumentSetId, [TODO-PODocSetToExport].ApplicationSettingsId, [TODO-PODocSetToExport].Country_of_origin_code, [TODO-PODocSetToExport].Currency_Code, 
                 [TODO-PODocSetToExport].Manifest_Number, [TODO-PODocSetToExport].BLNumber, [TODO-PODocSetToExport].Type_of_declaration, [TODO-PODocSetToExport].Declaration_gen_procedure_code, 
                 [TODO-PODocSetToExport].Declarant_Reference_Number, [TODO-PODocSetToExport].TotalInvoices, [TODO-PODocSetToExport].DocumentsCount, [TODO-PODocSetToExport].InvoiceTotal, 
                 [TODO-PODocSetToExport].LicenseLines, [TODO-PODocSetToExport].TotalCIF, AsycudaDocumentSetEntryDataCIF.InvoiceTotal, [TODO-PODocSetToExport].NeedC71, [TODO-PODocSetToExport].NeedLicense, 
                 [TODO-PODocSetToExport].HasC71, [TODO-PODocSetToExport].HasLicense, [TODO-PODocSetToExport].TotalFreight, AsycudaDocumentSetFreight.TotalFreight, [TODO-PODocSetToExport].ClassifiedLines, 
                 [TODO-PODocSetToExport].TotalLines, AsycudaDocumentSetLines.Lines, AsycudaDocumentSetTotals.InternalFreight, AsycudaDocumentSetTotals.Insurance, AsycudaDocumentSetTotals.OtherCost, 
                 AsycudaDocumentSetTotals.Deductions, [TODO-PODocSetToExport].ExpectedEntries, [TODO-PODocSetToExport].TotalPackages, [TODO-PODocSetToExport].EntryPackages, AsycudaDocumentSetPackages.Packages, 
                 [TODO-PODocSetAttachementErrors].Status, AsycudaDocumentSetEntryDataCIF.ImportedInvoices
HAVING ([TODO-PODocSetToExport].TotalFreight IS NOT NULL) AND ([TODO-PODocSetToExport].ClassifiedLines = [TODO-PODocSetToExport].TotalLines) AND (ABS(ROUND(AsycudaDocumentSetEntryDataCIF.InvoiceTotal, 2) 
                 - ROUND([TODO-PODocSetToExport].TotalCIF + AsycudaDocumentSetTotals.InternalFreight + AsycudaDocumentSetTotals.Insurance + AsycudaDocumentSetTotals.OtherCost - AsycudaDocumentSetTotals.Deductions, 
                 2)) <= 0.01) AND (ROUND(AsycudaDocumentSetFreight.TotalFreight, 2) = ROUND(ISNULL([TODO-PODocSetToExport].TotalFreight, 0), 2)) AND ([TODO-PODocSetAttachementErrors].Status IS NULL OR
                 [TODO-PODocSetAttachementErrors].Status = 'Missing Required Attachment: FREIGHT INVOICE' AND [TODO-PODocSetToExport].TotalFreight = 0) AND ([TODO-PODocSetToExport].Manifest_Number IS NOT NULL) AND 
                 ([TODO-PODocSetToExport].Manifest_Number <> '') AND (ISNULL([TODO-PODocSetToExport].ExpectedEntries, 0) = [TODO-PODocSetToExport].DocumentsCount) AND (ISNULL([TODO-PODocSetToExport].TotalInvoices, 
                 0) = AsycudaDocumentSetEntryDataCIF.ImportedInvoices) AND ([TODO-PODocSetToExport].EntryPackages = [TODO-PODocSetToExport].TotalPackages) AND 
                 ([TODO-PODocSetToExport].EntryPackages = AsycudaDocumentSetPackages.Packages)
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
         Begin Table = "TODO-PODocSetToExport"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 161
               Right = 338
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-PODocSetToAssess'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-PODocSetToAssess'
GO
