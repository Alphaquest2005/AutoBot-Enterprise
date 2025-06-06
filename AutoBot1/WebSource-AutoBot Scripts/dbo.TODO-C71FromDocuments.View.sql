USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-C71FromDocuments]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE VIEW [dbo].[TODO-C71FromDocuments]
AS
SELECT dbo.xcuda_Delivery_terms.Code, dbo.AsycudaDocumentBasicInfo.ASYCUDA_Id, dbo.AsycudaDocumentBasicInfo.ApplicationSettingsId, ISNULL(EntryDataEx.InvoiceTotal, 0) AS InvoiceTotal, EntryDataEx.Type, 
                 EntryDataEx.InvoiceDate, EntryDataEx.InvoiceNo, ISNULL(EntryDataEx.Currency, dbo.[AsycudaDocumentSet-CurrencyInfo].Currency_Code) AS Currency, dbo.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId, 
                 EntryDataEx.SupplierCode, dbo.[AsycudaDocumentSet-CurrencyInfo].CurrencyRate
FROM    dbo.xcuda_Delivery_terms INNER JOIN
                 dbo.xcuda_Transport ON dbo.xcuda_Delivery_terms.Transport_Id = dbo.xcuda_Transport.Transport_Id INNER JOIN
                 dbo.AsycudaDocumentBasicInfo ON dbo.xcuda_Transport.ASYCUDA_Id = dbo.AsycudaDocumentBasicInfo.ASYCUDA_Id INNER JOIN
                 dbo.xcuda_ASYCUDA_ExtendedProperties ON dbo.AsycudaDocumentBasicInfo.ASYCUDA_Id = dbo.xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id INNER JOIN
                 dbo.[AsycudaDocumentSet-CurrencyInfo] ON dbo.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId = dbo.[AsycudaDocumentSet-CurrencyInfo].AsycudaDocumentSetId INNER JOIN
                 dbo.AsycudaDocumentEntryData ON dbo.AsycudaDocumentBasicInfo.ASYCUDA_Id = dbo.AsycudaDocumentEntryData.AsycudaDocumentId INNER JOIN
                 dbo.[EntryDataEx-Basic] as EntryDataEx ON dbo.AsycudaDocumentEntryData.EntryData_Id = EntryDataEx.EntryData_Id AND dbo.AsycudaDocumentBasicInfo.ApplicationSettingsId = EntryDataEx.ApplicationSettingsId
WHERE (dbo.AsycudaDocumentBasicInfo.ImportComplete = 0)
GROUP BY dbo.xcuda_Delivery_terms.Code, dbo.AsycudaDocumentBasicInfo.ASYCUDA_Id, dbo.AsycudaDocumentBasicInfo.ApplicationSettingsId, EntryDataEx.InvoiceTotal, EntryDataEx.Type, 
                 EntryDataEx.InvoiceDate, EntryDataEx.InvoiceNo, EntryDataEx.Currency, dbo.[AsycudaDocumentSet-CurrencyInfo].Currency_Code, dbo.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId, 
                 EntryDataEx.SupplierCode, dbo.[AsycudaDocumentSet-CurrencyInfo].CurrencyRate
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
         Begin Table = "xcuda_Delivery_terms"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 161
               Right = 261
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Transport"
            Begin Extent = 
               Top = 6
               Left = 305
               Bottom = 161
               Right = 528
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentBasicInfo"
            Begin Extent = 
               Top = 6
               Left = 572
               Bottom = 161
               Right = 862
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_ASYCUDA_ExtendedProperties"
            Begin Extent = 
               Top = 6
               Left = 906
               Bottom = 161
               Right = 1167
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "TODO-PODocSet"
            Begin Extent = 
               Top = 162
               Left = 44
               Bottom = 317
               Right = 354
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentEntryData"
            Begin Extent = 
               Top = 6
               Left = 1565
               Bottom = 140
               Right = 1801
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryDataEx"
            Begin Extent = 
               Top = 128
               Left = 1249
               B' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-C71FromDocuments'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'ottom = 283
               Right = 1487
            End
            DisplayFlags = 280
            TopColumn = 5
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 16
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
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 4909
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-C71FromDocuments'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-C71FromDocuments'
GO
