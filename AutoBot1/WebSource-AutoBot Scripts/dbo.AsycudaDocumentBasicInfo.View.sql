USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AsycudaDocumentBasicInfo]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO










CREATE VIEW [dbo].[AsycudaDocumentBasicInfo]
 
AS
SELECT AsycudaDocumentSet.AsycudaDocumentSetId, xcuda_ASYCUDA.ASYCUDA_Id, xcuda_Type.Type_of_declaration + xcuda_Type.Declaration_gen_procedure_code AS DocumentType, xcuda_Registration.Number AS CNumber, 
                 Customs_Procedure.Extended_customs_procedure, Customs_Procedure.National_customs_procedure,
				 
				 CASE WHEN ismanuallyassessed = 0 THEN COALESCE (CASE WHEN dbo.xcuda_Registration.Date = '01/01/0001' THEN NULL 
                 ELSE dbo.xcuda_Registration.Date END, dbo.xcuda_ASYCUDA_ExtendedProperties.RegistrationDate) ELSE isnull(registrationdate, effectiveregistrationDate) END AS RegistrationDate,
				 
                 ISNULL(case when xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate > xcuda_Registration.Date or xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate = '01/01/0001' then xcuda_Registration.Date else xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate end, CASE WHEN ismanuallyassessed 
                 = 0 THEN COALESCE (CASE WHEN dbo.xcuda_Registration.Date = '01/01/0001' THEN NULL ELSE dbo.xcuda_Registration.Date END, xcuda_Registration.Date) 
                 ELSE isnull(registrationdate, case when xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate > xcuda_Registration.Date or xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate = '01/01/0001'  then xcuda_Registration.Date else xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate end) END) AS AssessmentDate,
				 

                 ISNULL(xcuda_ASYCUDA_ExtendedProperties.EffectiveExpiryDate, DATEADD(d, xcuda_warehouse.Delay, 
                 xcuda_Registration.Date)) AS ExpiryDate, xcuda_Declarant.Number AS Reference, xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed, xcuda_ASYCUDA_ExtendedProperties.Cancelled, 
                 xcuda_ASYCUDA_ExtendedProperties.DoNotAllocate, ApplicationSettings.ApplicationSettingsId, xcuda_ASYCUDA_ExtendedProperties.ImportComplete, Customs_Procedure.Customs_ProcedureId, CustomsProcedure, SourceFileName, SubmitToCustoms, IsPaid
FROM    dbo.xcuda_ASYCUDA WITH (NOLOCK) INNER JOIN
                 dbo.xcuda_ASYCUDA_ExtendedProperties WITH (NOLOCK) ON xcuda_ASYCUDA.ASYCUDA_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id INNER JOIN
                 dbo.Customs_Procedure WITH (NOLOCK) ON xcuda_ASYCUDA_ExtendedProperties.Customs_ProcedureId = Customs_Procedure.Customs_ProcedureId INNER JOIN
                 dbo.AsycudaDocumentSet WITH (NOLOCK) ON xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId = AsycudaDocumentSet.AsycudaDocumentSetId INNER JOIN
                 dbo.ApplicationSettings ON AsycudaDocumentSet.ApplicationSettingsId = ApplicationSettings.ApplicationSettingsId LEFT OUTER JOIN
                 dbo.xcuda_Declarant WITH (NOLOCK) ON xcuda_ASYCUDA.ASYCUDA_Id = xcuda_Declarant.ASYCUDA_Id LEFT OUTER JOIN
                 dbo.xcuda_Type WITH (NOLOCK) ON xcuda_ASYCUDA.ASYCUDA_Id = xcuda_Type.ASYCUDA_Id LEFT OUTER JOIN
                 dbo.xcuda_Registration WITH (NOLOCK) ON xcuda_ASYCUDA.ASYCUDA_Id = xcuda_Registration.ASYCUDA_Id LEFT OUTER JOIN
                 dbo.xcuda_Warehouse WITH (NOLOCK) ON xcuda_ASYCUDA.ASYCUDA_Id = xcuda_Warehouse.ASYCUDA_Id
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[41] 4[20] 2[8] 3) )"
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
               Top = 176
               Left = 109
               Bottom = 289
               Right = 286
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_ASYCUDA_ExtendedProperties"
            Begin Extent = 
               Top = 141
               Left = 699
               Bottom = 271
               Right = 920
            End
            DisplayFlags = 280
            TopColumn = 15
         End
         Begin Table = "Customs_Procedure"
            Begin Extent = 
               Top = 6
               Left = 1022
               Bottom = 136
               Right = 1268
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentSet"
            Begin Extent = 
               Top = 153
               Left = 1147
               Bottom = 308
               Right = 1419
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ApplicationSettings"
            Begin Extent = 
               Top = 94
               Left = 1539
               Bottom = 249
               Right = 1889
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Declarant"
            Begin Extent = 
               Top = 301
               Left = 438
               Bottom = 431
               Right = 656
            End
            DisplayFlags = 280
            TopColumn = 1
         End
         Begin Table = "xcuda_Type"
            Begin Extent = 
               Top = 6
               Left = 720
               Bottom ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaDocumentBasicInfo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'= 119
               Right = 984
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Registration"
            Begin Extent = 
               Top = 6
               Left = 512
               Bottom = 119
               Right = 682
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Warehouse"
            Begin Extent = 
               Top = 239
               Left = 510
               Bottom = 369
               Right = 680
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
         Width = 1505
         Width = 1505
         Width = 1505
         Width = 1505
         Width = 1505
         Width = 1505
         Width = 3325
         Width = 1505
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 15238
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaDocumentBasicInfo'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaDocumentBasicInfo'
GO
