USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-ImportCompleteEntries-New]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE VIEW [dbo].[TODO-ImportCompleteEntries-New]
AS
SELECT dbo.EntryDataDetailsEx.AsycudaDocumentSetId, dbo.EntryDataDetailsEx.ApplicationSettingsId, dbo.EntryDataDetailsEx.EntryDataId, dbo.EntryDataDetailsEx.EntryData_Id, dbo.EntryDataDetailsEx.EmailId, 
                 dbo.EntryDataDetailsEx.FileTypeId, y.ASYCUDA_Id, y.DocumentType, dbo.EntryDataDetailsEx.EntryDataDetailsId,
				 				 CAST(dbo.EntryDataDetailsEx.AsycudaDocumentSetId AS VARCHAR(50))+'-'+Y.DocumentType+'-'+CAST(dbo.EntryDataDetailsEx.EntryDataDetailsId   AS VARCHAR(50)) AS ID

FROM    [dbo].[TODO-ImportCompleteEntries-New-Data] AS y INNER JOIN
                 dbo.EntryDataDetailsEx WITH (NOLOCK) ON y.EntryDataDetailsId = dbo.EntryDataDetailsEx.EntryDataDetailsId INNER JOIN
                 dbo.AsycudaDocumentSetEntryData WITH (NOLOCK) ON y.EntryData_Id = dbo.AsycudaDocumentSetEntryData.EntryData_Id AND 
                 dbo.EntryDataDetailsEx.AsycudaDocumentSetId = dbo.AsycudaDocumentSetEntryData.AsycudaDocumentSetId 
GROUP BY dbo.EntryDataDetailsEx.AsycudaDocumentSetId, dbo.EntryDataDetailsEx.ApplicationSettingsId, dbo.EntryDataDetailsEx.EntryDataId, dbo.EntryDataDetailsEx.EntryData_Id, dbo.EntryDataDetailsEx.EmailId, 
                 dbo.EntryDataDetailsEx.FileTypeId, y.ASYCUDA_Id, y.DocumentType, dbo.EntryDataDetailsEx.EntryDataDetailsId
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
         Begin Table = "y"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 161
               Right = 251
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryDataDetailsEx"
            Begin Extent = 
               Top = 6
               Left = 295
               Bottom = 161
               Right = 534
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentSetEntryData"
            Begin Extent = 
               Top = 6
               Left = 578
               Bottom = 140
               Right = 817
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "SystemDocumentSets"
            Begin Extent = 
               Top = 6
               Left = 861
               Bottom = 98
               Right = 1045
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-ImportCompleteEntries-New'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-ImportCompleteEntries-New'
GO
