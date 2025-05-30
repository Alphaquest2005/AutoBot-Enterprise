USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-LicenseFromDocuments]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[TODO-LicenseFromDocuments]
AS
SELECT TOP (100) PERCENT AsycudaDocumentItem.TariffCode + N'00' AS TariffCode, TariffCodes.TariffCategoryCode, AsycudaDocumentItem.ItemQuantity AS Quantity, /*CAST(AsycudaDocumentItem.Item_price AS float)*/0 
                 AS Total, AsycudaDocumentItem.Commercial_Description, ISNULL(TariffCodes.LicenseDescription, AsycudaDocumentItem.Commercial_Description) AS LicenseDescription, 
                 AsycudaDocumentSet.AsycudaDocumentSetId, AsycudaDocumentSet.ApplicationSettingsId, AsycudaDocumentSet.Declarant_Reference_Number, EntryDataDetailsEx.EntryDataId, 
                 AsycudaDocumentSet.Country_of_origin_code, [License-UnitOfMeasure].SuppUnitCode2 AS UOM, AsycudaDocumentItem.Item_Id, EntryDataDetailsEx.ItemNumber, EntryData.SourceFile, EntryData.EmailId, 
                 EntryDataDetailsEx.ItemDescription, EntryDataDetailsEx.LineNumber, EntryData_PurchaseOrders.SupplierInvoiceNo, EntryDataDetailsEx.VolumeLiters
FROM    xcuda_ASYCUDA_ExtendedProperties INNER JOIN
                 AsycudaDocumentSetEx AS AsycudaDocumentSet ON xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId = AsycudaDocumentSet.AsycudaDocumentSetId INNER JOIN
                AsycudaItemBasicInfo as AsycudaDocumentItem ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = AsycudaDocumentItem.ASYCUDA_Id INNER JOIN
                 AsycudaDocumentItemEntryDataDetails ON AsycudaDocumentItem.Item_Id = AsycudaDocumentItemEntryDataDetails.Item_Id INNER JOIN
                 EntryDataDetailsEx ON AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId = EntryDataDetailsEx.EntryDataDetailsId AND 
                 AsycudaDocumentSet.ApplicationSettingsId = EntryDataDetailsEx.ApplicationSettingsId INNER JOIN
                 [License-UnitOfMeasure] ON AsycudaDocumentItem.TariffCode = [License-UnitOfMeasure].TariffCode INNER JOIN
                 TariffCodes ON AsycudaDocumentItem.TariffCode = TariffCodes.TariffCode INNER JOIN
                 EntryData ON EntryDataDetailsEx.EntryData_Id = EntryData.EntryData_Id INNER JOIN
                 EntryData_PurchaseOrders ON EntryData.EntryData_Id = EntryData_PurchaseOrders.EntryData_Id LEFT OUTER JOIN
                 SystemDocumentSets ON AsycudaDocumentSet.AsycudaDocumentSetId = SystemDocumentSets.Id
WHERE (xcuda_ASYCUDA_ExtendedProperties.ImportComplete = 0) AND (AsycudaDocumentSet.ClassifiedLines = AsycudaDocumentSet.TotalLines) AND (SystemDocumentSets.Id IS NULL) and (TariffCodes.LicenseRequired = 1)
GROUP BY AsycudaDocumentItem.TariffCode, TariffCodes.TariffCategoryCode, TariffCodes.LicenseDescription, AsycudaDocumentSet.AsycudaDocumentSetId, 
                  AsycudaDocumentSet.ApplicationSettingsId, AsycudaDocumentItem.Commercial_Description, AsycudaDocumentSet.Declarant_Reference_Number, 
                 EntryDataDetailsEx.EntryDataId, AsycudaDocumentSet.Country_of_origin_code, AsycudaDocumentItem.Item_Id, EntryDataDetailsEx.ItemNumber, AsycudaDocumentItem.ItemQuantity, 
                [License-UnitOfMeasure].SuppUnitCode2, EntryData.SourceFile, EntryData.EmailId, EntryDataDetailsEx.ItemNumber, EntryDataDetailsEx.ItemDescription, 
                 EntryDataDetailsEx.LineNumber, EntryData_PurchaseOrders.SupplierInvoiceNo, EntryDataDetailsEx.VolumeLiters
ORDER BY AsycudaDocumentSet.ApplicationSettingsId, AsycudaDocumentSet.AsycudaDocumentSetId
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[43] 4[14] 2[19] 3) )"
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
         Begin Table = "xcuda_ASYCUDA_ExtendedProperties"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 161
               Right = 305
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentSet"
            Begin Extent = 
               Top = 239
               Left = 641
               Bottom = 394
               Right = 929
            End
            DisplayFlags = 280
            TopColumn = 20
         End
         Begin Table = "AsycudaDocumentItem"
            Begin Extent = 
               Top = 12
               Left = 610
               Bottom = 167
               Right = 903
            End
            DisplayFlags = 280
            TopColumn = 17
         End
         Begin Table = "AsycudaDocumentItemEntryDataDetails"
            Begin Extent = 
               Top = 3
               Left = 1083
               Bottom = 245
               Right = 1396
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryDataDetailsEx"
            Begin Extent = 
               Top = 125
               Left = 1518
               Bottom = 280
               Right = 1773
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "License-UnitOfMeasure"
            Begin Extent = 
               Top = 162
               Left = 44
               Bottom = 296
               Right = 250
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "TariffCodes"
            Begin Extent = 
               Top = 79
               Left = ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-LicenseFromDocuments'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'999
               Bottom = 234
               Right = 1248
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
      Begin ColumnWidths = 14
         Width = 284
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 4556
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 2212
         Width = 1309
         Width = 1309
         Width = 1309
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 4687
         Alias = 903
         Table = 3024
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-LicenseFromDocuments'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-LicenseFromDocuments'
GO
