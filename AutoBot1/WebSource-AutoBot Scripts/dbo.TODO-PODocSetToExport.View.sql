USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-PODocSetToExport]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO












CREATE VIEW [dbo].[TODO-PODocSetToExport]
AS

SELECT distinct  [TODO-PODocSet].AsycudaDocumentSetId, [TODO-PODocSet].ApplicationSettingsId, [TODO-PODocSet].Country_of_origin_code, [TODO-PODocSet].Currency_Code, [TODO-PODocSet].FreightCurrencyCode, [TODO-PODocSet].Manifest_Number, 
                 [TODO-PODocSet].BLNumber, [TODO-PODocSet].Type_of_declaration, [TODO-PODocSet].Declaration_gen_procedure_code, [TODO-PODocSet].Declarant_Reference_Number, [TODO-PODocSet].TotalInvoices, 
                 [TODO-PODocSet].DocumentsCount,[TODO-PODocSet].ExpectedEntries, [TODO-PODocSet].InvoiceTotal, [TODO-PODocSet].LicenseLines, [TODO-PODocSet].TotalCIF, CASE WHEN InvoiceTotal > 185 AND [TODO-PODocSet].Currency_Code <> 'XCD' OR
                 (InvoiceTotal > 500 AND [TODO-PODocSet].Currency_Code = 'XCD') THEN 1 ELSE 0 END AS NeedC71, CASE WHEN licenselines > 0 THEN 1 ELSE 0 END AS NeedLicense, [TODO-PODocSetToExport-C71].HasC71, 
                 [TODO-PODocSetToExport-License].HasLicense, [TODO-PODocSet].TotalFreight, [TODO-PODocSet].ClassifiedLines, [TODO-PODocSet].TotalLines, [TODO-PODocSet].EntryPackages,  [TODO-PODocSet].TotalWeight, [TODO-PODocSet].TotalPackages
FROM    [TODO-PODocSetToExport-C71] RIGHT OUTER JOIN
                 [TODO-PODocSet] ON [TODO-PODocSetToExport-C71].AsycudaDocumentSetId = [TODO-PODocSet].AsycudaDocumentSetId LEFT OUTER JOIN
                 [TODO-PODocSetToExport-License] ON [TODO-PODocSet].AsycudaDocumentSetId = [TODO-PODocSetToExport-License].AsycudaDocumentSetId
WHERE 
				([TODO-PODocSet].Type_of_declaration <> 'IM' or
				([TODO-PODocSet].Type_of_declaration = 'IM' and
				(CASE WHEN InvoiceTotal > 185 AND [TODO-PODocSet].Currency_Code <> 'XCD' OR
                 (InvoiceTotal > 500 AND [TODO-PODocSet].Currency_Code = 'XCD') THEN 1 ELSE 0 END = ISNULL([TODO-PODocSetToExport-C71].HasC71, 0)))) AND (CASE WHEN licenselines > 0 THEN 1 ELSE 0 END = ISNULL(haslicense, 
                 0)) and DocumentsCount > 0 and TotalWeight > 0


--select LIC.* 
--from
--(SELECT [TODO-PODocSet].AsycudaDocumentSetId, [TODO-PODocSet].ApplicationSettingsId, [TODO-PODocSet].Country_of_origin_code, [TODO-PODocSet].Currency_Code, [TODO-PODocSet].Manifest_Number, 
--                 [TODO-PODocSet].BLNumber, [TODO-PODocSet].Type_of_declaration, [TODO-PODocSet].Declaration_gen_procedure_code, [TODO-PODocSet].Declarant_Reference_Number, [TODO-PODocSet].TotalInvoices, 
--                 CASE WHEN InvoiceTotal > 185 AND [TODO-PODocSet].Currency_Code <> 'XCD' OR
--                 (InvoiceTotal > 500 AND [TODO-PODocSet].Currency_Code = 'XCD') THEN 1 ELSE 0 END AS NeedC71, CASE WHEN licenselines > 0 THEN 1 ELSE 0 END AS NeedLicense, DocumentsCount, 
--                 InvoiceTotal, Attachments.FilePath
--FROM    [TODO-PODocSet] WITH (NOLOCK) INNER JOIN
--                 AsycudaDocumentSet_Attachments WITH (NOLOCK) ON [TODO-PODocSet].AsycudaDocumentSetId = AsycudaDocumentSet_Attachments.AsycudaDocumentSetId INNER JOIN
                
--                 Attachments WITH (NOLOCK) ON AsycudaDocumentSet_Attachments.AttachmentId = Attachments.Id LEFT OUTER JOIN
--                 xC71_Value_declaration_form_Registered WITH (NOLOCK) ON Attachments.FilePath = xC71_Value_declaration_form_Registered.SourceFile
--WHERE (CASE WHEN TotalCIF > 185 AND [TODO-PODocSet].Currency_Code = 'USD' THEN 1 ELSE 0 END = CASE WHEN value_declaration_form_id IS NULL THEN 0 ELSE 1 END) 
--GROUP BY [TODO-PODocSet].AsycudaDocumentSetId, [TODO-PODocSet].ApplicationSettingsId, [TODO-PODocSet].Country_of_origin_code, [TODO-PODocSet].Currency_Code, [TODO-PODocSet].Manifest_Number, 
--                 [TODO-PODocSet].BLNumber, [TODO-PODocSet].Type_of_declaration, [TODO-PODocSet].Declaration_gen_procedure_code, [TODO-PODocSet].Declarant_Reference_Number, [TODO-PODocSet].TotalInvoices, 
--                 InvoiceTotal, LicenseLines, DocumentsCount, TotalCIF, 
--                 xC71_Value_declaration_form_Registered.Value_declaration_form_Id, Attachments.FilePath, InvoiceTotal, licenselines
--HAVING (DocumentsCount > 0)) as C71 
--inner join

--(SELECT [TODO-PODocSet].AsycudaDocumentSetId, [TODO-PODocSet].ApplicationSettingsId, [TODO-PODocSet].Country_of_origin_code, [TODO-PODocSet].Currency_Code, [TODO-PODocSet].Manifest_Number, 
--                 [TODO-PODocSet].BLNumber, [TODO-PODocSet].Type_of_declaration, [TODO-PODocSet].Declaration_gen_procedure_code, [TODO-PODocSet].Declarant_Reference_Number, [TODO-PODocSet].TotalInvoices, 
--                 CASE WHEN InvoiceTotal > 185 AND [TODO-PODocSet].Currency_Code <> 'XCD' OR
--                 (InvoiceTotal > 500 AND [TODO-PODocSet].Currency_Code = 'XCD') THEN 1 ELSE 0 END AS NeedC71, CASE WHEN licenselines > 0 THEN 1 ELSE 0 END AS NeedLicense
--FROM    Attachments WITH (NOLOCK) INNER JOIN
--                 AsycudaDocumentSet_Attachments WITH (NOLOCK) ON Attachments.Id = AsycudaDocumentSet_Attachments.AttachmentId INNER JOIN
--                 [TODO-PODocSet] WITH (NOLOCK) ON AsycudaDocumentSet_Attachments.AsycudaDocumentSetId = [TODO-PODocSet].AsycudaDocumentSetId LEFT OUTER JOIN
--                 xLIC_License_Registered WITH (NOLOCK) ON Attachments.FilePath = xLIC_License_Registered.SourceFile
--WHERE (CASE WHEN licenselines > 0 THEN 1 ELSE 0 END = CASE WHEN licenseid IS NULL THEN 0 ELSE 1 END)
--GROUP BY [TODO-PODocSet].AsycudaDocumentSetId, [TODO-PODocSet].ApplicationSettingsId, [TODO-PODocSet].Country_of_origin_code, [TODO-PODocSet].Currency_Code, [TODO-PODocSet].Manifest_Number, 
--                 [TODO-PODocSet].BLNumber, [TODO-PODocSet].Type_of_declaration, [TODO-PODocSet].Declaration_gen_procedure_code, [TODO-PODocSet].Declarant_Reference_Number, [TODO-PODocSet].TotalInvoices , InvoiceTotal, licenselines) as LIC 

--on  C71.AsycudaDocumentSetId = LIC.AsycudaDocumentSetId

--SELECT dbo.[TODO-PODocSet].AsycudaDocumentSetId, dbo.[TODO-PODocSet].ApplicationSettingsId, dbo.[TODO-PODocSet].Country_of_origin_code, dbo.[TODO-PODocSet].Currency_Code, 
--                 dbo.[TODO-PODocSet].Manifest_Number, dbo.[TODO-PODocSet].BLNumber, dbo.[TODO-PODocSet].Type_of_declaration, dbo.[TODO-PODocSet].Declaration_gen_procedure_code, 
--                 dbo.[TODO-PODocSet].Declarant_Reference_Number, dbo.[TODO-PODocSet].TotalInvoices, CASE WHEN InvoiceTotal > 185 AND [TODO-PODocSet].Currency_Code <> 'XCD' OR
--                 (InvoiceTotal > 500 AND [TODO-PODocSet].Currency_Code = 'XCD') THEN 1 ELSE 0 END AS NeedC71, CASE WHEN licenselines > 0 THEN 1 ELSE 0 END AS NeedLicense, dbo.AsycudaDocumentSetEx.DocumentsCount, 
--                 dbo.AsycudaDocumentSetEx.InvoiceTotal
--FROM    dbo.[TODO-PODocSet] INNER JOIN
--                 dbo.AsycudaDocumentSetEx ON dbo.[TODO-PODocSet].AsycudaDocumentSetId = dbo.AsycudaDocumentSetEx.AsycudaDocumentSetId INNER JOIN
--                 dbo.AsycudaDocumentSet_Attachments ON dbo.AsycudaDocumentSetEx.AsycudaDocumentSetId = dbo.AsycudaDocumentSet_Attachments.AsycudaDocumentSetId INNER JOIN
--                 dbo.Attachments ON dbo.AsycudaDocumentSet_Attachments.AttachmentId = dbo.Attachments.Id LEFT OUTER JOIN
--                 dbo.xC71_Value_declaration_form_Registered ON dbo.Attachments.FilePath = dbo.xC71_Value_declaration_form_Registered.SourceFile LEFT OUTER JOIN
--                 dbo.xLIC_License_Registered ON dbo.Attachments.FilePath = dbo.xLIC_License_Registered.SourceFile
--WHERE  (CASE WHEN TotalCIF > 185 AND [TODO-PODocSet].Currency_Code = 'USD' THEN 1 ELSE 0 END = CASE WHEN value_declaration_form_id IS NULL THEN 0 ELSE 1 END) AND 
--                 (CASE WHEN licenselines > 0 THEN 1 ELSE 0 END = CASE WHEN licenseid IS NULL THEN 0 ELSE 1 END)
--GROUP BY dbo.[TODO-PODocSet].AsycudaDocumentSetId, dbo.[TODO-PODocSet].ApplicationSettingsId, dbo.[TODO-PODocSet].Country_of_origin_code, dbo.[TODO-PODocSet].Currency_Code, 
--                 dbo.[TODO-PODocSet].Manifest_Number, dbo.[TODO-PODocSet].BLNumber, dbo.[TODO-PODocSet].Type_of_declaration, dbo.[TODO-PODocSet].Declaration_gen_procedure_code, 
--                 dbo.[TODO-PODocSet].Declarant_Reference_Number, dbo.[TODO-PODocSet].TotalInvoices, dbo.AsycudaDocumentSetEx.InvoiceTotal, dbo.AsycudaDocumentSetEx.LicenseLines, 
--                 dbo.AsycudaDocumentSetEx.DocumentsCount, TotalCIF, value_declaration_form_id
--HAVING (dbo.AsycudaDocumentSetEx.DocumentsCount > 0)
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
            TopColumn = 7
         End
         Begin Table = "AsycudaDocumentSetEx"
            Begin Extent = 
               Top = 6
               Left = 398
               Bottom = 161
               Right = 681
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentSet_Attachments"
            Begin Extent = 
               Top = 6
               Left = 730
               Bottom = 161
               Right = 985
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Attachments"
            Begin Extent = 
               Top = 6
               Left = 1029
               Bottom = 161
               Right = 1234
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xC71_Value_declaration_form_Registered"
            Begin Extent = 
               Top = 6
               Left = 1278
               Bottom = 161
               Right = 1547
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xLIC_License_Registered"
            Begin Extent = 
               Top = 6
               Left = 1591
               Bottom = 161
               Right = 1823
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
      Begin' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-PODocSetToExport'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N' ColumnWidths = 16
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
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 10015
         Alias = 9530
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-PODocSetToExport'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-PODocSetToExport'
GO
