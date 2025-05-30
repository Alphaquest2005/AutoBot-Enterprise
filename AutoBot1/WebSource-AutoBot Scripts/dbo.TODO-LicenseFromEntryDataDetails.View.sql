USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-LicenseFromEntryDataDetails]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO











CREATE VIEW [dbo].[TODO-LicenseFromEntryDataDetails]
AS

SELECT TOP (100) PERCENT EntryDataDetailsEx.TariffCode + N'00' AS TariffCode, TariffCodes.TariffCategoryCode, EntryDataDetailsEx.Quantity, EntryDataDetailsEx.VolumeLiters, EntryDataDetailsEx.Quantity * EntryDataDetailsEx.Cost AS Total, 
                 ISNULL(TariffCodes.LicenseDescription, EntryDataDetailsEx.ItemDescription) AS LicenseDescription, AsycudaDocumentSet.AsycudaDocumentSetId, AsycudaDocumentSet.ApplicationSettingsId, 
                 AsycudaDocumentSet.Declarant_Reference_Number, AsycudaDocumentSet.Country_of_origin_code, [License-UnitOfMeasure].SuppUnitCode2 AS UOM, EntryDataDetailsEx.EntryDataId, 
                 EntryDataDetailsEx.ItemNumber, EntryDataDetailsEx.LineNumber, EntryDataDetailsEx.ItemDescription, EntryDataEx.Type, EntryDataEx.SourceFile, EntryDataEx.EmailId, EntryDataEx.SupplierInvoiceNo
FROM    EntryDataDetailsEx INNER JOIN
                 AsycudaDocumentSetEx AS AsycudaDocumentSet ON EntryDataDetailsEx.ApplicationSettingsId = AsycudaDocumentSet.ApplicationSettingsId INNER JOIN
                 TariffCodes ON EntryDataDetailsEx.TariffCode = TariffCodes.TariffCode INNER JOIN
                 [License-UnitOfMeasure] ON TariffCodes.TariffCode = [License-UnitOfMeasure].TariffCode INNER JOIN
                 [EntryDataEx-Classifications] as EntryDataEx ON EntryDataDetailsEx.EntryDataId = EntryDataEx.InvoiceNo AND AsycudaDocumentSet.ApplicationSettingsId = EntryDataEx.ApplicationSettingsId AND 
                 AsycudaDocumentSet.AsycudaDocumentSetId = EntryDataEx.AsycudaDocumentSetId LEFT OUTER JOIN
                 SystemDocumentSets ON AsycudaDocumentSet.AsycudaDocumentSetId = SystemDocumentSets.Id LEFT OUTER JOIN
                 AsycudaDocumentItemEntryDataDetails ON EntryDataDetailsEx.EntryDataDetailsId = AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId
WHERE (EntryDataEx.ClassifiedLines = EntryDataEx.TotalLines) and (TariffCodes.LicenseRequired = 1) AND (AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId IS NULL) AND (SystemDocumentSets.Id IS NULL) and (EntryDataEx.Type = N'PO')
GROUP BY EntryDataDetailsEx.TariffCode , TariffCodes.LicenseDescription, TariffCodes.TariffCategoryCode, AsycudaDocumentSet.AsycudaDocumentSetId, AsycudaDocumentSet.ApplicationSettingsId, 
                 AsycudaDocumentSet.Declarant_Reference_Number, AsycudaDocumentSet.Country_of_origin_code, [License-UnitOfMeasure].SuppUnitCode2, EntryDataDetailsEx.EntryDataId, EntryDataDetailsEx.ItemNumber, 
                 EntryDataDetailsEx.LineNumber, EntryDataDetailsEx.ItemDescription, EntryDataEx.Type, EntryDataDetailsEx.Quantity, EntryDataDetailsEx.Quantity * EntryDataDetailsEx.Cost, EntryDataEx.SourceFile, 
                 EntryDataDetailsEx.ItemDescription, EntryDataEx.EmailId, EntryDataEx.SupplierInvoiceNo, EntryDataDetailsEx.VolumeLiters

ORDER BY AsycudaDocumentSet.ApplicationSettingsId, AsycudaDocumentSet.AsycudaDocumentSetId
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[41] 4[20] 2[12] 3) )"
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
         Top = -26
         Left = 0
      End
      Begin Tables = 
         Begin Table = "EntryDataDetailsEx"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 305
               Right = 299
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentSet"
            Begin Extent = 
               Top = 145
               Left = 602
               Bottom = 300
               Right = 890
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "TariffCodes"
            Begin Extent = 
               Top = 6
               Left = 1296
               Bottom = 161
               Right = 1545
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "License-UnitOfMeasure"
            Begin Extent = 
               Top = 308
               Left = 44
               Bottom = 442
               Right = 250
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryDataEx"
            Begin Extent = 
               Top = 138
               Left = 1216
               Bottom = 310
               Right = 1518
            End
            DisplayFlags = 280
            TopColumn = 12
         End
         Begin Table = "AsycudaDocumentItemEntryDataDetails"
            Begin Extent = 
               Top = 0
               Left = 388
               Bottom = 155
               Right = 611
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
      Begin ColumnWidths =' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-LicenseFromEntryDataDetails'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N' 15
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
         Width = 2081
         Width = 1309
         Width = 4687
         Width = 1309
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 3639
         Alias = 2304
         Table = 4346
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1348
         SortOrder = 1414
         GroupBy = 1350
         Filter = 6794
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-LicenseFromEntryDataDetails'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-LicenseFromEntryDataDetails'
GO
