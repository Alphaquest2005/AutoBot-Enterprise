USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AsycudaDocumentSetC71]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[AsycudaDocumentSetC71]
AS
SELECT C71Summary.Address, C71Summary.Value_declaration_form_Id, C71Summary.Total, AsycudaDocumentSetEx.AsycudaDocumentSetId, isnull(Attachments.FilePath, C71Summary.SourceFile) as FilePath, isnull( Attachments.Id,0) AS AttachmentId, 
                 xC71_Value_declaration_form_Registered.RegNumber
FROM    C71Summary INNER JOIN
                 AsycudaDocumentSetEx ON C71Summary.Reference = AsycudaDocumentSetEx.Declarant_Reference_Number INNER JOIN
                 xC71_Value_declaration_form_Registered ON C71Summary.RegisteredId = xC71_Value_declaration_form_Registered.Value_declaration_form_Id AND 
                 AsycudaDocumentSetEx.ApplicationSettingsId = xC71_Value_declaration_form_Registered.ApplicationSettingsId LEFT OUTER JOIN
                 Attachments ON xC71_Value_declaration_form_Registered.SourceFile = Attachments.FilePath
WHERE (C71Summary.RegisteredId IS NOT NULL)
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
         Begin Table = "C71Summary"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 161
               Right = 297
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentSetEx"
            Begin Extent = 
               Top = 6
               Left = 341
               Bottom = 249
               Right = 613
            End
            DisplayFlags = 280
            TopColumn = 13
         End
         Begin Table = "xC71_Value_declaration_form_Registered"
            Begin Extent = 
               Top = 6
               Left = 657
               Bottom = 253
               Right = 910
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Attachments"
            Begin Extent = 
               Top = 6
               Left = 954
               Bottom = 161
               Right = 1143
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
   ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaDocumentSetC71'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'      Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaDocumentSetC71'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaDocumentSetC71'
GO
