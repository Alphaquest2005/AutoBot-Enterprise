USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-LICToCreate]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[TODO-LICToCreate]
AS

SELECT [TODO-PODocSet].AsycudaDocumentSetId, [TODO-PODocSet].ApplicationSettingsId, [TODO-PODocSet].Country_of_origin_code, [TODO-PODocSet].Currency_Code, [TODO-PODocSet].Manifest_Number, 
                 [TODO-PODocSet].BLNumber, [TODO-PODocSet].Type_of_declaration, [TODO-PODocSet].Declaration_gen_procedure_code, [TODO-PODocSet].Declarant_Reference_Number, [TODO-PODocSet].TotalInvoices, 
                 [TODO-PODocSet].DocumentsCount, [TODO-PODocSet].InvoiceTotal, [TODO-PODocSet].LicenseLines, [TODO-PODocSet].TotalCIF, [TODO-PODocSet].QtyLicensesRequired
FROM    [TODO-PODocSet] INNER JOIN
                 ApplicationSettings ON [TODO-PODocSet].ApplicationSettingsId = ApplicationSettings.ApplicationSettingsId
				  --INNER JOIN        [TODO-LicenseRequired] ON [TODO-PODocSet].ApplicationSettingsId = [TODO-LicenseRequired].ApplicationSettingsId AND 
      --           [TODO-PODocSet].AsycudaDocumentSetId = [TODO-LicenseRequired].AsycudaDocumentSetId
where ([TODO-PODocSet].LicenseLines > 0) AND ([TODO-PODocSet].Country_of_origin_code IS NOT NULL)
GROUP BY [TODO-PODocSet].AsycudaDocumentSetId, [TODO-PODocSet].ApplicationSettingsId, [TODO-PODocSet].Country_of_origin_code, [TODO-PODocSet].Currency_Code, [TODO-PODocSet].Manifest_Number, 
                 [TODO-PODocSet].BLNumber, [TODO-PODocSet].Type_of_declaration, [TODO-PODocSet].Declaration_gen_procedure_code, [TODO-PODocSet].Declarant_Reference_Number, [TODO-PODocSet].TotalInvoices, 
                 [TODO-PODocSet].DocumentsCount, [TODO-PODocSet].InvoiceTotal, [TODO-PODocSet].LicenseLines, [TODO-PODocSet].TotalCIF, [TODO-PODocSet].QtyLicensesRequired


--SELECT [TODO-PODocSet].AsycudaDocumentSetId, [TODO-PODocSet].ApplicationSettingsId, [TODO-PODocSet].Country_of_origin_code, [TODO-PODocSet].Currency_Code, [TODO-PODocSet].Manifest_Number, 
--                 [TODO-PODocSet].BLNumber, [TODO-PODocSet].Type_of_declaration, [TODO-PODocSet].Declaration_gen_procedure_code, [TODO-PODocSet].Declarant_Reference_Number, [TODO-PODocSet].TotalInvoices, 
--                 [TODO-PODocSet].DocumentsCount, [TODO-PODocSet].InvoiceTotal, [TODO-PODocSet].LicenseLines, [TODO-PODocSet].TotalCIF, QtyLicensesRequired, COUNT([TODO-PODocSetToExport-License].HasLicense) HasLicense
				 
--FROM    [TODO-PODocSet] LEFT OUTER JOIN
--                 [TODO-PODocSetToExport-License] ON [TODO-PODocSet].AsycudaDocumentSetId = [TODO-PODocSetToExport-License].AsycudaDocumentSetId
--				 left outer join xLIC_General_segment on Exporter_address like '%' + [TODO-PODocSetToExport-License].DocumentReference + '%'
--WHERE ([TODO-PODocSetToExport-License].DocumentReference IS NULL)
--GROUP BY [TODO-PODocSet].AsycudaDocumentSetId, [TODO-PODocSet].ApplicationSettingsId, [TODO-PODocSet].Country_of_origin_code, [TODO-PODocSet].Currency_Code, [TODO-PODocSet].Manifest_Number, 
--                 [TODO-PODocSet].BLNumber, [TODO-PODocSet].Type_of_declaration, [TODO-PODocSet].Declaration_gen_procedure_code, [TODO-PODocSet].Declarant_Reference_Number, [TODO-PODocSet].TotalInvoices, 
--                 [TODO-PODocSet].DocumentsCount, [TODO-PODocSet].InvoiceTotal, [TODO-PODocSet].LicenseLines, [TODO-PODocSet].TotalCIF, QtyLicensesRequired
--HAVING ([TODO-PODocSet].LicenseLines > 0) AND (COUNT([TODO-PODocSetToExport-License].HasLicense) <> [TODO-PODocSet].QtyLicensesRequired)
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
         Begin Table = "TODO-PODocSet"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 161
               Right = 338
            End
            DisplayFlags = 280
            TopColumn = 10
         End
         Begin Table = "TODO-PODocSetToExport-License"
            Begin Extent = 
               Top = 0
               Left = 580
               Bottom = 155
               Right = 819
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
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 2016
         Width = 1309
         Width = 1309
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-LICToCreate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-LICToCreate'
GO
