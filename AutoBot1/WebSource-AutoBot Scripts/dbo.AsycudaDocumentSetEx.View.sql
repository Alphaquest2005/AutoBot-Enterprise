USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AsycudaDocumentSetEx]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO












CREATE VIEW [dbo].[AsycudaDocumentSetEx]
AS
SELECT        AsycudaDocumentSet.AsycudaDocumentSetId, AsycudaDocumentSet.Declarant_Reference_Number, CAST(AsycudaDocumentSet.Exchange_Rate AS float) AS Exchange_Rate, AsycudaDocumentSet.Customs_ProcedureId, 
                         AsycudaDocumentSet.Country_of_origin_code, AsycudaDocumentSet.Currency_Code,isnull( Customs_Procedure.Document_TypeId,0) as Document_TypeId , AsycudaDocumentSet.Description, AsycudaDocumentSet.Manifest_Number, 
                         AsycudaDocumentSet.BLNumber, AsycudaDocumentSet.EntryTimeStamp, AsycudaDocumentSet.StartingFileCount, AsycudaDocumentSet.ApportionMethod, COUNT(DISTINCT xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id) 
                         AS DocumentsCount, AsycudaDocumentSet.ExpectedEntries, SUM(AsycudaDocumentItemValueWeights.Total_CIF_itm) AS TotalCIF, SUM(AsycudaDocumentItemValueWeights.Gross_weight_itm) AS TotalWeight, 
                         AsycudaDocumentSet.TotalFreight, ISNULL(AsycudaDocumentSet.ApplicationSettingsId, 0) AS ApplicationSettingsId, AsycudaDocumentSet.TotalPackages, AsycudaDocumentSet.LastFileNumber, 
                         AsycudaDocumentSet.TotalInvoices, AsycudaDocumentSetEntryDataExTotals.ClassifiedLines, AsycudaDocumentSetEntryDataExTotals.TotalLines, AsycudaDocumentSet.MaxLines, AsycudaDocumentSet.LocationOfGoods, 
                         AsycudaDocumentSetEntryDataExTotals.LicenseLines, AsycudaDocumentSetEntryDataExTotals.QtyLicensesRequired, AsycudaDocumentSetEntryDataExTotals.Total AS InvoiceTotal, AsycudaDocumentSet.FreightCurrencyCode, 
                         AsycudaDocumentSetEntryDataExTotals.ImportedInvoices, AsycudaDocumentSetEntryDataPackages.Packages AS EntryPackages, CurrencyRates.Rate AS CurrencyRate, FreightCurrency.Rate AS FreightCurrencyRate
FROM            AsycudaDocumentSetEntryDataExTotals RIGHT OUTER JOIN
                         AsycudaDocumentSet WITH (NOLOCK) ON AsycudaDocumentSetEntryDataExTotals.AsycudaDocumentSetId = AsycudaDocumentSet.AsycudaDocumentSetId LEFT OUTER JOIN
                         AsycudaDocumentItemValueWeights INNER JOIN
                         xcuda_ASYCUDA_ExtendedProperties WITH (NOLOCK) ON AsycudaDocumentItemValueWeights.ASYCUDA_Id = xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id ON 
                         AsycudaDocumentSet.AsycudaDocumentSetId = xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId LEFT OUTER JOIN
                         AsycudaDocumentSetEntryDataPackages ON AsycudaDocumentSet.AsycudaDocumentSetId = AsycudaDocumentSetEntryDataPackages.AsycudaDocumentSetId INNER JOIN
                         CurrencyRates ON CurrencyRates.CurrencyCode = AsycudaDocumentSet.Currency_Code INNER JOIN
                         CurrencyRates AS FreightCurrency ON FreightCurrency.CurrencyCode = AsycudaDocumentSet.FreightCurrencyCode LEFT OUTER JOIN
                         Customs_Procedure ON AsycudaDocumentSet.Customs_ProcedureId = Customs_Procedure.Customs_ProcedureId
GROUP BY AsycudaDocumentSet.AsycudaDocumentSetId, AsycudaDocumentSet.Declarant_Reference_Number, CAST(AsycudaDocumentSet.Exchange_Rate AS float), AsycudaDocumentSet.Customs_ProcedureId, 
                         AsycudaDocumentSet.Country_of_origin_code, AsycudaDocumentSet.Currency_Code, Customs_Procedure.Document_TypeId, AsycudaDocumentSet.Description, AsycudaDocumentSet.Manifest_Number, 
                         AsycudaDocumentSet.BLNumber, AsycudaDocumentSet.EntryTimeStamp, AsycudaDocumentSet.StartingFileCount, AsycudaDocumentSet.ApportionMethod, AsycudaDocumentSet.TotalFreight, 
                         AsycudaDocumentSet.ApplicationSettingsId, AsycudaDocumentSet.TotalPackages, AsycudaDocumentSet.LastFileNumber, AsycudaDocumentSet.TotalInvoices, AsycudaDocumentSet.MaxLines, 
                         AsycudaDocumentSet.LocationOfGoods, AsycudaDocumentSet.FreightCurrencyCode, AsycudaDocumentSetEntryDataExTotals.ClassifiedLines, AsycudaDocumentSetEntryDataExTotals.TotalLines, 
                         AsycudaDocumentSetEntryDataExTotals.QtyLicensesRequired, AsycudaDocumentSetEntryDataExTotals.Total, AsycudaDocumentSetEntryDataExTotals.LicenseLines, AsycudaDocumentSetEntryDataExTotals.ImportedInvoices, 
                         AsycudaDocumentSetEntryDataPackages.Packages, CurrencyRates.Rate, FreightCurrency.Rate, AsycudaDocumentSet.ExpectedEntries
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[37] 4[15] 2[20] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1 [50] 4 [25] 3))"
      End
      Begin PaneConfiguration = 2
         NumPanes = 3
         Configuration = "(H (1[50] 2[25] 3) )"
      End
      Begin PaneConfiguration = 3
         NumPanes = 3
         Configuration = "(H (4 [30] 2 [40] 3))"
      End
      Begin PaneConfiguration = 4
         NumPanes = 2
         Configuration = "(H (1[56] 3) )"
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
         Configuration = "(H (1[51] 4) )"
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
         Begin Table = "AsycudaDocumentSetEntryData"
            Begin Extent = 
               Top = 14
               Left = 277
               Bottom = 148
               Right = 588
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentSet"
            Begin Extent = 
               Top = 0
               Left = 658
               Bottom = 299
               Right = 992
            End
            DisplayFlags = 280
            TopColumn = 11
         End
         Begin Table = "AsycudaDocumentItemValueWeights"
            Begin Extent = 
               Top = 35
               Left = 1498
               Bottom = 232
               Right = 1826
            End
            DisplayFlags = 280
            TopColumn = 2
         End
         Begin Table = "xcuda_ASYCUDA_ExtendedProperties"
            Begin Extent = 
               Top = 14
               Left = 1112
               Bottom = 268
               Right = 1418
            End
            DisplayFlags = 280
            TopColumn = 17
         End
         Begin Table = "EntryDataExTotals"
            Begin Extent = 
               Top = 57
               Left = 42
               Bottom = 212
               Right = 242
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
      Begin ColumnWidths = 29
         Width = 284
         Width = 1008
         Width = 2749
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Wid' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaDocumentSetEx'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'th = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
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
         Width = 1309
         Width = 1309
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 6925
         Alias = 2985
         Table = 3024
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1348
         SortOrder = 1414
         GroupBy = 2527
         Filter = 1348
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaDocumentSetEx'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaDocumentSetEx'
GO
