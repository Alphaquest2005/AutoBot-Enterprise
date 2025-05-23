USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AdjustmentShortAllocations-Adjustments]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





CREATE VIEW [dbo].[AdjustmentShortAllocations-Adjustments]
AS
SELECT  EntryDataDetails.TotalValue, isnull(EntryDataDetails.EffectiveDate, EntryData.EntryDataDate) AS InvoiceDate, ROUND(EntryDataDetails.Quantity, 1) AS SalesQuantity, ROUND(EntryDataDetails.QtyAllocated, 1) 
                 AS SalesQtyAllocated, EntryDataDetails.EntryDataId AS InvoiceNo, EntryDataDetails.LineNumber AS SalesLineNumber, EntryDataDetails.ItemNumber, EntryDataDetails.ItemDescription, 
                 EntryDataDetails.EntryDataDetailsId, EntryDataDetails.Cost, EntryDataDetails.DoNotAllocate AS DoNotAllocateSales, 
                  EntryDataDetails.EffectiveDate, 
                 EntryDataDetails.Comment, AsycudaDocumentSetEntryData.AsycudaDocumentSetId, coalesce(
		CASE when AdjustmentComments.Comments is not null then AdjustmentComments.DutyFreePaid else null end,
		case when isnull(EntryDataDetails.TaxAmount, isnull(EntryData_Sales.Tax, 0)) <> 0
			THEN 'Duty Paid'
		ELSE 'Duty Free'
		END) AS DutyFreePaid, EntryData.ApplicationSettingsId, EntryData.FileTypeId, EntryData.EmailId, EntryData_Sales.Type,
				 0 as SANumber
				 -- cast(row_number() OVER (partition BY dbo.Entrydatadetails.entrydataid	ORDER BY dbo.Entrydatadetails.entrydatadetailsid) AS int) AS SANumber
					, ISNULL(EntryDataDetails.TaxAmount, ISNULL(EntryData_Sales.Tax, 
                 0)) AS TaxAmount
FROM    AsycudaDocumentSetEntryData WITH (NOLOCK) INNER JOIN
                 EntryData_Adjustments AS EntryData_Sales WITH (NOLOCK) INNER JOIN
                 EntryDataDetails WITH (NOLOCK) ON EntryData_Sales.EntryData_Id = EntryDataDetails.EntryData_Id INNER JOIN
                 EntryData WITH (NOLOCK) ON EntryData_Sales.EntryData_Id = EntryData.EntryData_Id ON AsycudaDocumentSetEntryData.EntryData_Id = EntryDataDetails.EntryData_Id INNER JOIN
                 SystemDocumentSets WITH (nolock) ON AsycudaDocumentSetEntryData.AsycudaDocumentSetId = SystemDocumentSets.Id
				 left outer join AdjustmentComments on lower(EntryDataDetails.Comment) = lower(AdjustmentComments.Comments)
--GROUP BY EntryData.EntryDataDate, EntryDataDetails.Quantity, EntryDataDetails.QtyAllocated, EntryDataDetails.EntryDataId, EntryDataDetails.ItemNumber, EntryDataDetails.ItemDescription, 
--                 EntryDataDetails.EntryDataDetailsId, EntryDataDetails.LineNumber, EntryDataDetails.Cost, EntryDataDetails.Quantity, EntryDataDetails.DoNotAllocate, EntryDataDetails.Cost, EntryDataDetails.EffectiveDate, 
--                 EntryDataDetails.Comment, AsycudaDocumentSetEntryData.AsycudaDocumentSetId, EntryDataDetails.TaxAmount, EntryData.ApplicationSettingsId, EntryData_Sales.Tax, EntryData.FileTypeId, EntryData.EmailId, 
--                 EntryData_Sales.Type, EntryDataDetails.LineNumber, EntryDataDetails.TaxAmount, EntryData_Sales.Tax
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
         Begin Table = "AsycudaDocumentSetEntryData"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 140
               Right = 299
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryData_Sales"
            Begin Extent = 
               Top = 6
               Left = 343
               Bottom = 140
               Right = 543
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryDataDetails"
            Begin Extent = 
               Top = 6
               Left = 587
               Bottom = 161
               Right = 841
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryData"
            Begin Extent = 
               Top = 6
               Left = 885
               Bottom = 161
               Right = 1123
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "SystemDocumentSets"
            Begin Extent = 
               Top = 6
               Left = 1167
               Bottom = 98
               Right = 1367
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
         O' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AdjustmentShortAllocations-Adjustments'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'r = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AdjustmentShortAllocations-Adjustments'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AdjustmentShortAllocations-Adjustments'
GO
