USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AsycudaDocument]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO













CREATE VIEW [dbo].[AsycudaDocument]
AS

SELECT xcuda_ASYCUDA.ASYCUDA_Id, xcuda_ASYCUDA.id, CASE WHEN ismanuallyassessed = 0 THEN COALESCE (dbo.xcuda_Registration.Number, dbo.xcuda_ASYCUDA_ExtendedProperties.CNumber) 
                 ELSE CNumber END AS CNumber, CASE WHEN ismanuallyassessed = 0 THEN COALESCE (CASE WHEN dbo.xcuda_Registration.Date = '01/01/0001' THEN NULL 
                 ELSE dbo.xcuda_Registration.Date END, dbo.xcuda_ASYCUDA_ExtendedProperties.RegistrationDate) ELSE isnull(registrationdate, effectiveregistrationDate) END AS RegistrationDate, 
                 xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed, COALESCE (xcuda_Declarant.Number, xcuda_ASYCUDA_ExtendedProperties.ReferenceNumber) AS ReferenceNumber, 
                 (case when xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate = '01/01/0001' then null else  xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate end) as EffectiveRegistrationDate, xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId, xcuda_ASYCUDA_ExtendedProperties.DoNotAllocate, 
                 xcuda_ASYCUDA_ExtendedProperties.AutoUpdate, xcuda_ASYCUDA_ExtendedProperties.BLNumber, xcuda_ASYCUDA_ExtendedProperties.Description, Document_Type.Type_of_declaration, 
                 Document_Type.Declaration_gen_procedure_code, Customs_Procedure.Extended_customs_procedure, Document_Type.Type_of_declaration + Document_Type.Declaration_gen_procedure_code AS DocumentType, 
                 Document_Type.Document_TypeId, Customs_Procedure.Customs_ProcedureId, Customs_Procedure.CustomsOperationId, Customs_Procedure.CustomsProcedure, xcuda_Country.Country_first_destination, 
                 xcuda_Gs_Invoice.Currency_code, xcuda_Gs_Invoice.Currency_rate, xcuda_Identification.Manifest_reference_number, xcuda_Office_segment.Customs_clearance_office_code, AsycudaDocumentLines.Lines, 
                 xcuda_ASYCUDA_ExtendedProperties.ImportComplete, xcuda_ASYCUDA_ExtendedProperties.Cancelled, SUM(AsycudaDocumentItemValueWeights.Total_CIF_itm) AS TotalCIF, 
                 SUM(AsycudaDocumentItemValueWeights.Gross_weight_itm) AS TotalGrossWeight,
				 
				 --ISNULL(case when xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate > xcuda_Registration.Date then xcuda_Registration.Date else xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate end, CASE WHEN ismanuallyassessed
     --            = 0 THEN COALESCE (CASE WHEN dbo.xcuda_Registration.Date = '01/01/0001' THEN NULL ELSE dbo.xcuda_Registration.Date END, xcuda_Registration.Date) 
     --            ELSE isnull(registrationdate, case when xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate > xcuda_Registration.Date  then xcuda_Registration.Date else xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate end) END) AS AssessmentDate,
				ISNULL(CASE WHEN xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate > xcuda_Registration.Date or xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate = '01/01/0001' THEN xcuda_Registration.Date ELSE xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate END, 
                CASE WHEN ismanuallyassessed = 0 THEN COALESCE (CASE WHEN dbo.xcuda_Registration.Date = '01/01/0001' THEN NULL ELSE dbo.xcuda_Registration.Date END, xcuda_Registration.Date) ELSE isnull(registrationdate, 
                CASE WHEN xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate > xcuda_Registration.Date or xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate = '01/01/0001' THEN xcuda_Registration.Date ELSE xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate END) END) AS AssessmentDate,
				 
				 xcuda_Gs_external_freight.Amount_foreign_currency AS TotalFreight, 
                 ISNULL(xcuda_ASYCUDA_ExtendedProperties.EffectiveExpiryDate, DATEADD(d,  xcuda_warehouse.Delay , 
                 xcuda_Registration.Date)) AS ExpiryDate, ApplicationSettings.ApplicationSettingsId, xcuda_ASYCUDA_ExtendedProperties.SourceFileName, Customs_Procedure.IsPaid, 
                 Customs_Procedure.SubmitToCustoms, xcuda_Gs_deduction.Amount_foreign_currency AS TotalDeduction, xcuda_Gs_other_cost.Amount_foreign_currency AS TotalOtherCost, 
                 xcuda_Gs_internal_freight.Amount_foreign_currency AS TotalInternalFreight, xcuda_Gs_insurance.Amount_foreign_currency AS TotalInsurance
FROM    xcuda_Gs_external_freight WITH (NOLOCK) RIGHT OUTER JOIN
                 xcuda_Gs_deduction INNER JOIN
                 xcuda_Gs_Invoice WITH (NOLOCK) INNER JOIN
                 xcuda_Valuation WITH (NOLOCK) ON xcuda_Gs_Invoice.Valuation_Id = xcuda_Valuation.ASYCUDA_Id ON xcuda_Gs_deduction.Valuation_Id = xcuda_Valuation.ASYCUDA_Id ON 
                 xcuda_Gs_external_freight.Valuation_Id = xcuda_Valuation.ASYCUDA_Id LEFT OUTER JOIN
                 xcuda_Gs_other_cost ON xcuda_Valuation.ASYCUDA_Id = xcuda_Gs_other_cost.Valuation_Id LEFT OUTER JOIN
                 xcuda_Gs_insurance ON xcuda_Valuation.ASYCUDA_Id = xcuda_Gs_insurance.Valuation_Id LEFT OUTER JOIN
                 xcuda_Gs_internal_freight ON xcuda_Valuation.ASYCUDA_Id = xcuda_Gs_internal_freight.Valuation_Id RIGHT OUTER JOIN
                 xcuda_Country WITH (NOLOCK) INNER JOIN
                 xcuda_General_information WITH (NOLOCK) ON xcuda_Country.Country_Id = xcuda_General_information.ASYCUDA_Id RIGHT OUTER JOIN
                 xcuda_ASYCUDA WITH (NOLOCK) LEFT OUTER JOIN
                 xcuda_Warehouse ON xcuda_ASYCUDA.ASYCUDA_Id = xcuda_Warehouse.ASYCUDA_Id LEFT OUTER JOIN
                 AsycudaDocumentItemValueWeights ON xcuda_ASYCUDA.ASYCUDA_Id = AsycudaDocumentItemValueWeights.ASYCUDA_Id LEFT OUTER JOIN
                 AsycudaDocumentLines WITH (NOLOCK) ON xcuda_ASYCUDA.ASYCUDA_Id = AsycudaDocumentLines.ASYCUDA_Id LEFT OUTER JOIN
                 xcuda_Office_segment WITH (NOLOCK) RIGHT OUTER JOIN
                 xcuda_Identification WITH (NOLOCK) ON xcuda_Office_segment.ASYCUDA_Id = xcuda_Identification.ASYCUDA_Id ON xcuda_ASYCUDA.ASYCUDA_Id = xcuda_Identification.ASYCUDA_Id LEFT OUTER JOIN
                 xcuda_Registration WITH (NOLOCK) ON xcuda_ASYCUDA.ASYCUDA_Id = xcuda_Registration.ASYCUDA_Id ON xcuda_General_information.ASYCUDA_Id = xcuda_ASYCUDA.ASYCUDA_Id ON 
                 xcuda_Valuation.ASYCUDA_Id = xcuda_ASYCUDA.ASYCUDA_Id LEFT OUTER JOIN
                 xcuda_Declarant WITH (NOLOCK) ON xcuda_ASYCUDA.ASYCUDA_Id = xcuda_Declarant.ASYCUDA_Id LEFT OUTER JOIN
                 Document_Type WITH (NOLOCK) INNER JOIN
                 Customs_Procedure WITH (NOLOCK) ON Document_Type.Document_TypeId = Customs_Procedure.Document_TypeId RIGHT OUTER JOIN
                 ApplicationSettings INNER JOIN
                 AsycudaDocumentSet INNER JOIN
                 xcuda_ASYCUDA_ExtendedProperties WITH (NOLOCK) ON AsycudaDocumentSet.AsycudaDocumentSetId = xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId ON 
                 ApplicationSettings.ApplicationSettingsId = AsycudaDocumentSet.ApplicationSettingsId ON Customs_Procedure.Customs_ProcedureId = xcuda_ASYCUDA_ExtendedProperties.Customs_ProcedureId AND 
                 Document_Type.Document_TypeId = Customs_Procedure.Document_TypeId ON xcuda_ASYCUDA.ASYCUDA_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id
--WHERE (COALESCE (xcuda_Declarant.Number, xcuda_ASYCUDA_ExtendedProperties.ReferenceNumber) LIKE '%PEVGRE11193-NG%')
GROUP BY xcuda_ASYCUDA.ASYCUDA_Id, xcuda_ASYCUDA.id, xcuda_Declarant.Number, xcuda_ASYCUDA_ExtendedProperties.ReferenceNumber, 
                 xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate, xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId, xcuda_ASYCUDA_ExtendedProperties.BLNumber, 
                 xcuda_ASYCUDA_ExtendedProperties.Description, Document_Type.Type_of_declaration, Document_Type.Declaration_gen_procedure_code, Customs_Procedure.Extended_customs_procedure, 
                 Document_Type.Type_of_declaration + Document_Type.Declaration_gen_procedure_code, Document_Type.Document_TypeId, Customs_Procedure.Customs_ProcedureId, xcuda_Country.Country_first_destination, 
                 xcuda_Gs_Invoice.Currency_code, xcuda_Gs_Invoice.Currency_rate, xcuda_Identification.Manifest_reference_number, xcuda_Office_segment.Customs_clearance_office_code, AsycudaDocumentLines.Lines, 
                 xcuda_ASYCUDA_ExtendedProperties.Cancelled, xcuda_ASYCUDA_ExtendedProperties.ImportComplete, xcuda_ASYCUDA_ExtendedProperties.DoNotAllocate, xcuda_ASYCUDA_ExtendedProperties.AutoUpdate, 
                 xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed, xcuda_Registration.Number, xcuda_ASYCUDA_ExtendedProperties.CNumber, xcuda_ASYCUDA_ExtendedProperties.RegistrationDate, 
                 xcuda_Registration.Date, xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate,ismanuallyassessed,  xcuda_Gs_external_freight.Amount_foreign_currency, 
                 xcuda_warehouse.Delay , ApplicationSettings.ApplicationSettingsId, xcuda_ASYCUDA_ExtendedProperties.EffectiveExpiryDate,
                 xcuda_ASYCUDA_ExtendedProperties.SourceFileName, Customs_Procedure.CustomsProcedure, Customs_Procedure.CustomsOperationId, Customs_Procedure.IsPaid, Customs_Procedure.SubmitToCustoms, 
                 xcuda_Gs_deduction.Amount_foreign_currency, xcuda_Gs_other_cost.Amount_foreign_currency, xcuda_Gs_internal_freight.Amount_foreign_currency, xcuda_Gs_insurance.Amount_foreign_currency
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[58] 4[4] 2[12] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1[51] 4[26] 3) )"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1[50] 2[25] 3) )"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4[30] 2[40] 3) )"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1[56] 3) )"
      End
      Begin PaneConfiguration = 5
         NumPanes = 2
         Configuration = "(H (2[66] 3) )"
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
         Configuration = "(H (1[41] 4[32] 2) )"
      End
      Begin PaneConfiguration = 9
         NumPanes = 2
         Configuration = "(H (1[75] 4) )"
      End
      Begin PaneConfiguration = 10
         NumPanes = 2
         Configuration = "(H (1[56] 2) )"
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
         Left = -202
      End
      Begin Tables = 
         Begin Table = "xcuda_Country"
            Begin Extent = 
               Top = 376
               Left = 1935
               Bottom = 572
               Right = 2234
            End
            DisplayFlags = 280
            TopColumn = 1
         End
         Begin Table = "xcuda_General_information"
            Begin Extent = 
               Top = 0
               Left = 323
               Bottom = 196
               Right = 609
            End
            DisplayFlags = 280
            TopColumn = 1
         End
         Begin Table = "xcuda_ASYCUDA"
            Begin Extent = 
               Top = 15
               Left = 0
               Bottom = 228
               Right = 234
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Warehouse"
            Begin Extent = 
               Top = 242
               Left = 495
               Bottom = 378
               Right = 681
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentItemValueWeights"
            Begin Extent = 
               Top = 79
               Left = 1156
               Bottom = 276
               Right = 1468
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentLines"
            Begin Extent = 
               Top = 513
               Left = 111
               Bottom = 671
               Right = 439
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Office_segment"
            Begin Extent = 
               Top = 492
               Left = 1683
            ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaDocument'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'   Bottom = 670
               Right = 2041
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Identification"
            Begin Extent = 
               Top = 276
               Left = 943
               Bottom = 418
               Right = 1263
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Registration"
            Begin Extent = 
               Top = 347
               Left = 685
               Bottom = 516
               Right = 907
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Gs_external_freight"
            Begin Extent = 
               Top = 317
               Left = 1426
               Bottom = 514
               Right = 1754
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Gs_Invoice"
            Begin Extent = 
               Top = 139
               Left = 1435
               Bottom = 335
               Right = 1747
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Valuation"
            Begin Extent = 
               Top = 290
               Left = 219
               Bottom = 495
               Right = 538
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Declarant"
            Begin Extent = 
               Top = 433
               Left = 471
               Bottom = 629
               Right = 767
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Document_Type"
            Begin Extent = 
               Top = 33
               Left = 1672
               Bottom = 229
               Right = 2036
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Customs_Procedure"
            Begin Extent = 
               Top = 0
               Left = 2168
               Bottom = 196
               Right = 2507
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "ApplicationSettings"
            Begin Extent = 
               Top = 212
               Left = 2160
               Bottom = 367
               Right = 2526
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentSet"
            Begin Extent = 
               Top = 205
               Left = 1807
               Bottom = 360
               Right = 2095
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_ASYCUDA_ExtendedProperties"
            Begin Extent = 
               Top = 38
               Left = 653
               Bottom = 324
               Right = 976
            End
            DisplayFlags = 280
            TopColumn = 15
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 17
         Width = 284
         Width = 1008
         Width = 1008
         Width = 1859
         Width = 2029
         Width = 1008
         Width = 1008
         Width = 1414
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1649
         Width = 1754
         Width = 1008
         Width = 1008
      End
   End
   Begin CriteriaPane ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaDocument'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane3', @value=N'= 
      Begin ColumnWidths = 12
         Column = 10604
         Alias = 2487
         Table = 3076
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaDocument'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=3 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaDocument'
GO
