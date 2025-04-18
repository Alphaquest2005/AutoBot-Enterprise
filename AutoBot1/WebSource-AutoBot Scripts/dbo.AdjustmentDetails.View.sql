USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AdjustmentDetails]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO





















CREATE VIEW [dbo].[AdjustmentDetails]
AS
SELECT EntryDataDetailsEx.EntryDataDetailsId
	,Adjustments.EntryData_Id
	,EntryDataDetailsEx.EntryDataId
	,EntryDataDetailsEx.LineNumber
	,EntryDataDetailsEx.ItemNumber
	,case when ISNULL(EntryDataDetailsEx.ReceivedQty, 0) = ISNULL(EntryDataDetailsEx.InvoiceQty, 0) and ISNULL(EntryDataDetailsEx.ReceivedQty, 0) > 0 then ISNULL(EntryDataDetailsEx.ReceivedQty, 0) else  ISNULL(EntryDataDetailsEx.ReceivedQty, 0) - ISNULL(EntryDataDetailsEx.InvoiceQty, 0) end AS Quantity-- done because normal entries are submitted as overage if not in shipment
	,EntryDataDetailsEx.Units
	,EntryDataDetailsEx.ItemDescription
	,CASE 
		WHEN Cost = 0
			THEN isnull(lastcost, 0)
		ELSE cost
		END AS Cost
	,EntryDataDetailsEx.QtyAllocated
	,EntryDataDetailsEx.UnitWeight
	,EntryDataDetailsEx.DoNotAllocate
	,EntryDataDetailsEx.TariffCode
	,EntryDataDetailsEx.Total
	,EntryDataDetailsEx.AsycudaDocumentSetId
	,EntryDataDetailsEx.InvoiceQty
	,EntryDataDetailsEx.ReceivedQty
	,EntryDataDetailsEx.Status
	,EntryDataDetailsEx.PreviousInvoiceNumber
	,EntryDataDetailsEx.CNumber
	,EntryDataDetailsEx.CLineNumber
	,EntryDataDetailsEx.Downloaded
	,EntryDataDetailsEx.PreviousCNumber
	,EntryDataDetailsEx.PreviousCLineNumber
	,coalesce(
		CASE when AdjustmentComments.Comments is not null then AdjustmentComments.DutyFreePaid else null end,
		case when isnull(EntryDataDetailsEx.TaxAmount, isnull(Adjustments.Tax, 0)) <> 0
			THEN 'Duty Paid'
		ELSE 'Duty Free'
		END) AS DutyFreePaid
	,Adjustments.Type
	,EntryDataDetailsEx.Comment
	,EntryDataDetailsEx.EffectiveDate
	,Adjustments.Currency
	,EntryDataDetailsEx.IsReconciled
	,Adjustments.ApplicationSettingsId
	,Adjustments.EmailId
	,Adjustments.FileTypeId
	,Adjustments.InvoiceDate
	,Emails.Subject
	,Emails.EmailDate
	,EntryDataDetailsEx.InventoryItemId,
	isnull(EntryDataDetailsEx.TaxAmount, Adjustments.Tax) as TaxAmount,
	Adjustments.Vendor
	,Adjustments.SourceFile
	
FROM [AdjustmentDetails-EntryDataDetailsEx] AS EntryDataDetailsEx WITH (NOLOCK)
INNER JOIN [AdjustmentDetails-EntryDataEx] AS Adjustments WITH (NOLOCK) ON EntryDataDetailsEx.EntryData_Id = Adjustments.EntryData_Id	
LEFT OUTER JOIN Emails ON Adjustments.EmailId = Emails.EmailId
LEFT OUTER JOIN (
	SELECT EntryDataId as InvoiceNo, entrydata.ApplicationSettingsId
FROM    [EntryData_Sales] AS sales WITH (NOLOCK) inner JOIN
                 EntryData on sales.EntryData_Id = entrydata.entrydata_id
	
	) AS Sales ON Adjustments.ApplicationSettingsId = Sales.ApplicationSettingsId
	AND EntryDataDetailsEx.PreviousInvoiceNumber = Sales.InvoiceNo 
left outer join AdjustmentComments on lower(EntryDataDetailsEx.Comment) = lower(AdjustmentComments.Comments)
WHERE (Sales.InvoiceNo IS NULL)
	AND (Adjustments.Type = N'ADJ')
	OR (Adjustments.Type = N'DIS')
GROUP BY EntryDataDetailsEx.EntryDataDetailsId
	,EntryDataDetailsEx.EntryDataId
	,EntryDataDetailsEx.LineNumber
	,EntryDataDetailsEx.ItemNumber
	,EntryDataDetailsEx.Units
	,EntryDataDetailsEx.ItemDescription
	,EntryDataDetailsEx.QtyAllocated
	,EntryDataDetailsEx.UnitWeight
	,EntryDataDetailsEx.TariffCode
	,EntryDataDetailsEx.Total
	,EntryDataDetailsEx.AsycudaDocumentSetId
	,EntryDataDetailsEx.InvoiceQty
	,EntryDataDetailsEx.ReceivedQty
	,EntryDataDetailsEx.STATUS
	,EntryDataDetailsEx.PreviousInvoiceNumber
	,EntryDataDetailsEx.CNumber
	,EntryDataDetailsEx.CLineNumber
	,EntryDataDetailsEx.PreviousCNumber
	,EntryDataDetailsEx.PreviousCLineNumber
	,EntryDataDetailsEx.DutyFreePaid
	,Adjustments.Type
	,EntryDataDetailsEx.Comment
	,EntryDataDetailsEx.EffectiveDate
	,Adjustments.Currency
	,Adjustments.ApplicationSettingsId
	,EntryDataDetailsEx.DoNotAllocate
	,EntryDataDetailsEx.Downloaded
	,EntryDataDetailsEx.IsReconciled
	,EntryDataDetailsEx.Cost
	,EntryDataDetailsEx.LastCost
	,Adjustments.EmailId
	,Adjustments.FileTypeId
	,Adjustments.InvoiceDate
	,Emails.Subject
	,Emails.EmailDate
	,Adjustments.EntryData_Id
	,EntryDataDetailsEx.InventoryItemId
	,Adjustments.Tax, EntryDataDetailsEx.TaxAmount
	,AdjustmentComments.Comments
	,AdjustmentComments.DutyFreePaid
	,Vendor
	,Adjustments.SourceFile
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[39] 4[19] 2[25] 3) )"
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
         Left = -220
      End
      Begin Tables = 
         Begin Table = "Adjustments"
            Begin Extent = 
               Top = 15
               Left = 1257
               Bottom = 211
               Right = 1441
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Emails"
            Begin Extent = 
               Top = 132
               Left = 244
               Bottom = 266
               Right = 444
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Sales"
            Begin Extent = 
               Top = 51
               Left = 1552
               Bottom = 280
               Right = 1807
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "EntryDataDetailsEx"
            Begin Extent = 
               Top = 0
               Left = 591
               Bottom = 239
               Right = 971
            End
            DisplayFlags = 280
            TopColumn = 14
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 30
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
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
  ' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AdjustmentDetails'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'       Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
         Width = 1309
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 9910
         Alias = 903
         Table = 1964
         Output = 720
         Append = 1400
         NewValue = 1170
         SortType = 1348
         SortOrder = 1414
         GroupBy = 1350
         Filter = 5695
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AdjustmentDetails'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AdjustmentDetails'
GO
