USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AsycudaDocumentRegistrationDate]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO



CREATE VIEW [dbo].[AsycudaDocumentRegistrationDate]
AS
SELECT dbo.xcuda_Registration.ASYCUDA_Id, dbo.xcuda_Registration.Number AS CNumber, CASE WHEN ismanuallyassessed = 0 THEN COALESCE (CASE WHEN dbo.xcuda_Registration.Date = '01/01/0001' THEN NULL 
                 ELSE isnull(effectiveregistrationDate, dbo.xcuda_Registration.Date ) END, dbo.xcuda_ASYCUDA_ExtendedProperties.RegistrationDate) ELSE isnull(effectiveregistrationDate, registrationdate) 
                 END AS AssessmentDate, Document_Type.Type_of_declaration + Document_Type.Declaration_gen_procedure_code AS DocumentType,dbo.xcuda_Registration.Date AS RegistrationDate, 
                 dbo.xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed, dbo.xcuda_ASYCUDA_ExtendedProperties.Cancelled, dbo.xcuda_ASYCUDA_ExtendedProperties.DoNotAllocate
FROM    dbo.xcuda_Registration  WITH (NOLOCK)INNER JOIN
                 dbo.xcuda_ASYCUDA_ExtendedProperties  WITH (NOLOCK) ON dbo.xcuda_Registration.ASYCUDA_Id = dbo.xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id INNER JOIN
                 dbo.xcuda_Type AS Document_Type  WITH (NOLOCK) ON dbo.xcuda_Registration.ASYCUDA_Id = Document_Type.ASYCUDA_Id
WHERE (dbo.xcuda_ASYCUDA_ExtendedProperties.Cancelled <> 1) OR
                 (dbo.xcuda_ASYCUDA_ExtendedProperties.Cancelled IS NULL)
--GROUP BY dbo.xcuda_Registration.ASYCUDA_Id, dbo.xcuda_Registration.Number, Document_Type.Type_of_declaration + Document_Type.Declaration_gen_procedure_code, 
--                 dbo.xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed, dbo.xcuda_ASYCUDA_ExtendedProperties.RegistrationDate, 
--                 dbo.xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate, dbo.xcuda_Registration.Date, dbo.xcuda_ASYCUDA_ExtendedProperties.Cancelled, dbo.xcuda_ASYCUDA_ExtendedProperties.DoNotAllocate
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[26] 4[35] 2[20] 3) )"
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
         Begin Table = "xcuda_Registration"
            Begin Extent = 
               Top = 13
               Left = 449
               Bottom = 183
               Right = 671
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_ASYCUDA_ExtendedProperties"
            Begin Extent = 
               Top = 0
               Left = 842
               Bottom = 284
               Right = 1144
            End
            DisplayFlags = 280
            TopColumn = 14
         End
         Begin Table = "Document_Type"
            Begin Extent = 
               Top = 100
               Left = 0
               Bottom = 270
               Right = 364
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
         Width = 1008
         Width = 1008
         Width = 3404
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 6441
         Alias = 3247
         Table = 3391
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1348
         SortOrder = 1414
         GroupBy = 1350
         Filter = 4137
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaDocumentRegistrationDate'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaDocumentRegistrationDate'
GO
