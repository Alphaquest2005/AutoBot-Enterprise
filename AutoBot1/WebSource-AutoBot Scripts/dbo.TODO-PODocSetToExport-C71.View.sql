USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-PODocSetToExport-C71]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[TODO-PODocSetToExport-C71]
AS

SELECT (CASE WHEN xC71_Value_declaration_form_Registered.value_declaration_form_id IS NULL THEN 0 ELSE 1 END) AS HasC71, AsycudaDocumentSet_Attachments.AsycudaDocumentSetId, 
                 xC71_Value_declaration_form_Registered.RegNumber, xC71_Value_declaration_form_Registered.SourceFile, xC71_Value_declaration_form_Registered.DocumentReference, SUM(distinct CAST(xC71_Item.Net_Price AS float)) -- some bug making this thing double for unknown reason
                 AS C71Total
FROM    AsycudaDocumentSet_Attachments WITH (NOLOCK) INNER JOIN
                 Attachments WITH (NOLOCK) ON AsycudaDocumentSet_Attachments.AttachmentId = Attachments.Id LEFT OUTER JOIN
                 xC71_Item INNER JOIN
                 xC71_Value_declaration_form_Registered WITH (NOLOCK) ON xC71_Item.Value_declaration_form_Id = xC71_Value_declaration_form_Registered.Value_declaration_form_Id ON 
                 Attachments.FilePath = xC71_Value_declaration_form_Registered.SourceFile
WHERE (xC71_Value_declaration_form_Registered.SourceFile IS NOT NULL)
GROUP BY xC71_Value_declaration_form_Registered.Value_declaration_form_Id, AsycudaDocumentSet_Attachments.AsycudaDocumentSetId, xC71_Value_declaration_form_Registered.RegNumber, 
                 xC71_Value_declaration_form_Registered.SourceFile, xC71_Value_declaration_form_Registered.DocumentReference

--SELECT (CASE WHEN xC71_Value_declaration_form_Registered.value_declaration_form_id IS NULL THEN 0 ELSE 1 END) AS HasC71, AsycudaDocumentSet_Attachments.AsycudaDocumentSetId, 
--                 xC71_Value_declaration_form_Registered.RegNumber, xC71_Value_declaration_form_Registered.SourceFile, xC71_Value_declaration_form_Registered.DocumentReference, SUM(CAST(xC71_Item.Net_Price AS float)) 
--                 AS C71Total
--FROM    AsycudaDocumentSet_Attachments WITH (NOLOCK) INNER JOIN
--                 Attachments WITH (NOLOCK) ON AsycudaDocumentSet_Attachments.AttachmentId = Attachments.Id LEFT OUTER JOIN
--                 xC71_Item INNER JOIN
--                 xC71_Value_declaration_form_Registered WITH (NOLOCK) ON xC71_Item.Value_declaration_form_Id = xC71_Value_declaration_form_Registered.Value_declaration_form_Id ON 
--                 Attachments.FilePath = xC71_Value_declaration_form_Registered.SourceFile
--WHERE (xC71_Value_declaration_form_Registered.SourceFile IS NOT NULL)
--GROUP BY xC71_Value_declaration_form_Registered.Value_declaration_form_Id, AsycudaDocumentSet_Attachments.AsycudaDocumentSetId, xC71_Value_declaration_form_Registered.RegNumber, 
--                 xC71_Value_declaration_form_Registered.SourceFile, xC71_Value_declaration_form_Registered.DocumentReference
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
         Begin Table = "AsycudaDocumentSet_Attachments"
            Begin Extent = 
               Top = 6
               Left = 398
               Bottom = 161
               Right = 653
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Attachments"
            Begin Extent = 
               Top = 6
               Left = 697
               Bottom = 161
               Right = 902
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xC71_Value_declaration_form_Registered"
            Begin Extent = 
               Top = 6
               Left = 946
               Bottom = 161
               Right = 1215
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-PODocSetToExport-C71'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-PODocSetToExport-C71'
GO
