USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-Error-DuplicateEntry]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[TODO-Error-DuplicateEntry]
AS
SELECT dbo.xcuda_ASYCUDA.ASYCUDA_Id, 'DuplicateEntry' AS Type, dbo.AsycudaDocumentLines.Lines, DupEntry.id, dbo.AsycudaDocumentBasicInfo.DocumentType, dbo.AsycudaDocumentBasicInfo.CNumber, 
                 dbo.AsycudaDocumentBasicInfo.Extended_customs_procedure, dbo.AsycudaDocumentBasicInfo.RegistrationDate, dbo.AsycudaDocumentBasicInfo.ApplicationSettingsId, 
                 dbo.AsycudaDocumentBasicInfo.Reference
FROM    dbo.xcuda_ASYCUDA WITH (Nolock) INNER JOIN
                 dbo.AsycudaDocumentLines WITH (Nolock) ON dbo.xcuda_ASYCUDA.ASYCUDA_Id = dbo.AsycudaDocumentLines.ASYCUDA_Id INNER JOIN
                     (SELECT id
                      FROM     dbo.xcuda_ASYCUDA AS xcuda_ASYCUDA_1 WITH (nolock)
                      GROUP BY id
                      HAVING (COUNT(id) > 1)) AS DupEntry ON dbo.xcuda_ASYCUDA.id = DupEntry.id INNER JOIN
                 dbo.AsycudaDocumentBasicInfo ON dbo.xcuda_ASYCUDA.ASYCUDA_Id = dbo.AsycudaDocumentBasicInfo.ASYCUDA_Id
GROUP BY dbo.AsycudaDocumentLines.Lines, dbo.xcuda_ASYCUDA.ASYCUDA_Id, DupEntry.id, dbo.AsycudaDocumentBasicInfo.DocumentType, dbo.AsycudaDocumentBasicInfo.CNumber, 
                 dbo.AsycudaDocumentBasicInfo.Extended_customs_procedure, dbo.AsycudaDocumentBasicInfo.RegistrationDate, dbo.AsycudaDocumentBasicInfo.ApplicationSettingsId, dbo.AsycudaDocumentBasicInfo.Reference
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
         Begin Table = "xcuda_ASYCUDA"
            Begin Extent = 
               Top = 7
               Left = 406
               Bottom = 141
               Right = 615
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentLines"
            Begin Extent = 
               Top = 116
               Left = 808
               Bottom = 229
               Right = 1008
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "DupEntry"
            Begin Extent = 
               Top = 0
               Left = 1044
               Bottom = 92
               Right = 1244
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentBasicInfo"
            Begin Extent = 
               Top = 10
               Left = 28
               Bottom = 311
               Right = 318
            End
            DisplayFlags = 280
            TopColumn = 2
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 10
         Width = 284
         Width = 2173
         Width = 1623
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
         O' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-Error-DuplicateEntry'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'r = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-Error-DuplicateEntry'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-Error-DuplicateEntry'
GO
