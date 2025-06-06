USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-C71ToCreate]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO













CREATE VIEW [dbo].[TODO-C71ToCreate]
AS
--ISNULL(CAST((row_number() OVER (ORDER BY [TODO-PODocSet].AsycudaDocumentSetId)) AS int), 0) as Id,
SELECT isnull([TODO-PODocSet].AsycudaDocumentSetId,0) as AsycudaDocumentSetId, [TODO-PODocSet].ApplicationSettingsId, [TODO-PODocSet].Country_of_origin_code, [TODO-PODocSet].Currency_Code, [TODO-PODocSet].Manifest_Number, 
                 [TODO-PODocSet].BLNumber, [TODO-PODocSet].Type_of_declaration, [TODO-PODocSet].Declaration_gen_procedure_code, [TODO-PODocSet].Declarant_Reference_Number, [TODO-PODocSet].TotalInvoices, 
                 [TODO-PODocSet].DocumentsCount, [TODO-PODocSet].ExpectedEntries, [TODO-PODocSet].InvoiceTotal, [TODO-PODocSet].LicenseLines, [TODO-PODocSet].TotalCIF, ISNULL([TODO-PODocSetToExport-C71].C71Total, 
                 0) AS C71Total, [TODO-PODocSet].CurrencyRate AS Rate, C71Summary.Total AS GeneratedC71Total

				 ,C71Summary.Address, C71Summary.Value_declaration_form_Id,C71Summary.RegisteredID

FROM    C71Summary RIGHT OUTER JOIN
                [TODO-C71ToCreate-PODocSet] as [TODO-PODocSet] LEFT OUTER JOIN
                 [TODO-PODocSetToExport-C71] ON [TODO-PODocSet].AsycudaDocumentSetId = [TODO-PODocSetToExport-C71].AsycudaDocumentSetId 
                ON  C71Summary.Reference = [TODO-PODocSet].Declarant_Reference_Number
where
 ([TODO-PODocSet].Type_of_declaration = 'IM') and
((
--(([TODO-PODocSet].InvoiceTotal * [TODO-PODocSet].CurrencyRate >= 1) /*AND ([TODO-PODocSetToExport-C71].AsycudaDocumentSetId IS NULL and   ((C71Summary.Address IS not NULL) and (C71Summary.Value_declaration_form_Id IS NOT NULL) AND (C71Summary.RegisteredID IS NULL)))*/) OR
	   ([TODO-PODocSetToExport-C71].AsycudaDocumentSetId IS not NULL and ROUND([TODO-PODocSet].TotalCIF,2) <> ROUND([TODO-PODocSetToExport-C71].C71Total,2))OR
	   ([TODO-PODocSetToExport-C71].AsycudaDocumentSetId IS NULL and ROUND([TODO-PODocSet].InvoiceTotal,2) <> ROUND(C71Summary.Total,2)) OR
	    (ROUND(C71Summary.Total,2) <> ROUND(isnull([TODO-PODocSetToExport-C71].C71Total,c71summary.total),2)
	) 
and 
([TODO-PODocSet].ExpectedEntries = [TODO-PODocSet].DocumentsCount or [TODO-PODocSet].DocumentsCount = 0)
and ([TODO-PODocSet].TotalInvoices > 0)
and ([TODO-PODocSet].InvoiceTotal * [TODO-PODocSet].CurrencyRate >= 1)

)
or--switch
 (
 (C71Summary.Address IS NULL)
  OR
   ((C71Summary.Address IS not NULL) and (C71Summary.Value_declaration_form_Id IS NOT NULL) AND (C71Summary.RegisteredID IS NULL))))



GROUP BY [TODO-PODocSet].AsycudaDocumentSetId, [TODO-PODocSet].ApplicationSettingsId, [TODO-PODocSet].Country_of_origin_code, [TODO-PODocSet].Currency_Code, [TODO-PODocSet].Manifest_Number, 
                 [TODO-PODocSet].BLNumber, [TODO-PODocSet].Type_of_declaration, [TODO-PODocSet].Declaration_gen_procedure_code, [TODO-PODocSet].Declarant_Reference_Number, [TODO-PODocSet].TotalInvoices, 
                 [TODO-PODocSet].DocumentsCount, [TODO-PODocSet].InvoiceTotal, [TODO-PODocSet].LicenseLines, [TODO-PODocSet].TotalCIF, [TODO-PODocSetToExport-C71].AsycudaDocumentSetId, 
                 [TODO-PODocSetToExport-C71].C71Total, [TODO-PODocSet].CurrencyRate, [TODO-PODocSet].ExpectedEntries, C71Summary.Total
				 
				 ,C71Summary.Address, C71Summary.Value_declaration_form_Id,C71Summary.RegisteredID
-- C71Summary.Value_declaration_form_Id,
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
               Right = 354
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "TODO-PODocSetToExport-C71"
            Begin Extent = 
               Top = 6
               Left = 398
               Bottom = 161
               Right = 653
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
         Width = 1309
         Width = 1309
         Width = 1309
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-C71ToCreate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-C71ToCreate'
GO
