USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-LicenseToXML]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[TODO-LicenseToXML]
AS

SELECT t.TariffCode, t.TariffCategoryCode, t.Quantity, t.VolumeLiters, t.LicenseDescription, t.AsycudaDocumentSetId, t.ApplicationSettingsId, t.Declarant_Reference_Number, t.Country_of_origin_code, t.UOM, t.EntryDataId, t.SourceFile, 
                 t.EmailId, t.ItemNumber, t.ItemDescription, t.LineNumber
FROM    (SELECT TariffCode, TariffCategoryCode, SUM(Quantity) AS Quantity, VolumeLiters, LicenseDescription, AsycudaDocumentSetId, ApplicationSettingsId, Declarant_Reference_Number, Country_of_origin_code, UOM, EntryDataId, 
                                  SourceFile, EmailId, ItemNumber, ItemDescription, LineNumber, SupplierInvoiceNo
                 FROM     [TODO-LicenseFromDocuments]
                 GROUP BY TariffCode, TariffCategoryCode, LicenseDescription, AsycudaDocumentSetId, ApplicationSettingsId, Declarant_Reference_Number, Country_of_origin_code, UOM, EntryDataId, SourceFile, EmailId, 
                                  ItemNumber, ItemDescription, LineNumber, SupplierInvoiceNo, VolumeLiters
                 UNION
                 SELECT TariffCode, TariffCategoryCode, Quantity,VolumeLiters, LicenseDescription, AsycudaDocumentSetId, ApplicationSettingsId, Declarant_Reference_Number, Country_of_origin_code, UOM, EntryDataId, SourceFile, EmailId, 
                                  ItemNumber, ItemDescription, LineNumber,SupplierInvoiceNo
                 FROM    [TODO-LicenseFromEntryData]
                 GROUP BY TariffCode, TariffCategoryCode, Quantity,VolumeLiters, LicenseDescription, AsycudaDocumentSetId, ApplicationSettingsId, Declarant_Reference_Number, Country_of_origin_code, UOM, EntryDataId, SourceFile, EmailId, 
                                  ItemNumber, ItemDescription, LineNumber,SupplierInvoiceNo) AS t LEFT OUTER JOIN
                 [TODO-LicenseDetails] ON t.ApplicationSettingsId = [TODO-LicenseDetails].ApplicationSettingsId AND t.TariffCode = [TODO-LicenseDetails].Commodity_code
				 and ([TODO-LicenseDetails].Exporter_address like '%' + t.EntryDataId + '%' or [TODO-LicenseDetails].Exporter_address like '%' + t.SupplierInvoiceNo + '%')
WHERE ([TODO-LicenseDetails].RegistrationNumber IS NULL)
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
         Begin Table = "TODO-LicenseFromDocuments"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 305
               Right = 316
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-LicenseToXML'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-LicenseToXML'
GO
