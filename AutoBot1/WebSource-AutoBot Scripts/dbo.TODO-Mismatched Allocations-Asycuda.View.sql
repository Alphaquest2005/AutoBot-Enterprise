USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[TODO-Mismatched Allocations-Asycuda]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[TODO-Mismatched Allocations-Asycuda]
AS
SELECT dbo.AsycudaDocumentItem.Item_Id, dbo.AsycudaDocumentItem.AsycudaDocumentId, dbo.AsycudaDocumentItem.EntryDataDetailsId, dbo.AsycudaDocumentItem.LineNumber, dbo.AsycudaDocumentItem.IsAssessed, 
                 dbo.AsycudaDocumentItem.DoNotAllocate, dbo.AsycudaDocumentItem.DoNotEX, dbo.AsycudaDocumentItem.AttributeOnlyAllocation, dbo.AsycudaDocumentItem.Description_of_goods, 
                 dbo.AsycudaDocumentItem.Commercial_Description, dbo.AsycudaDocumentItem.Gross_weight_itm, dbo.AsycudaDocumentItem.Net_weight_itm, dbo.AsycudaDocumentItem.Item_price, 
                 dbo.AsycudaDocumentItem.ItemQuantity, dbo.AsycudaDocumentItem.Suppplementary_unit_code, dbo.AsycudaDocumentItem.ItemNumber, dbo.AsycudaDocumentItem.TariffCode, 
                 dbo.AsycudaDocumentItem.TariffCodeLicenseRequired, dbo.AsycudaDocumentItem.TariffCategoryLicenseRequired, dbo.AsycudaDocumentItem.TariffCodeDescription, dbo.AsycudaDocumentItem.DutyLiability, 
                 dbo.AsycudaDocumentItem.Total_CIF_itm, dbo.AsycudaDocumentItem.Freight, dbo.AsycudaDocumentItem.Statistical_value, dbo.AsycudaDocumentItem.DPQtyAllocated, dbo.AsycudaDocumentItem.DFQtyAllocated, 
                 dbo.AsycudaDocumentItem.ImportComplete, dbo.AsycudaDocumentItem.CNumber, dbo.AsycudaDocumentItem.RegistrationDate, dbo.AsycudaDocumentItem.Number_of_packages, 
                 dbo.AsycudaDocumentItem.Country_of_origin_code, dbo.AsycudaDocumentItem.PiWeight, dbo.AsycudaDocumentItem.Currency_rate, dbo.AsycudaDocumentItem.Currency_code, 
                 dbo.AsycudaDocumentItem.InvalidHSCode, dbo.AsycudaDocumentItem.WarehouseError, dbo.AsycudaDocumentItem.PiQuantity, dbo.AsycudaDocumentItem.Cancelled, dbo.AsycudaDocumentItem.SalesFactor, 
                 dbo.AsycudaDocumentItem.ReferenceNumber, dbo.AsycudaDocumentItem.ExpiryDate, dbo.AsycudaDocumentItem.PreviousInvoiceNumber, dbo.AsycudaDocumentItem.PreviousInvoiceLineNumber, 
                 dbo.AsycudaDocumentItem.PreviousInvoiceItemNumber, dbo.AsycudaDocumentBasicInfo.DocumentType, dbo.AsycudaDocumentBasicInfo.ApplicationSettingsId
FROM    dbo.AsycudaDocumentItem WITH (nolock) INNER JOIN
                 dbo.AsycudaDocumentBasicInfo WITH (nolock) ON dbo.AsycudaDocumentItem.AsycudaDocumentId = dbo.AsycudaDocumentBasicInfo.ASYCUDA_Id
				 inner join AsycudaDocumentCustomsProcedures on AsycudaDocumentCustomsProcedures.ASYCUDA_Id = AsycudaDocumentBasicInfo.ASYCUDA_Id
WHERE (dbo.AsycudaDocumentItem.DFQtyAllocated + dbo.AsycudaDocumentItem.DPQtyAllocated <> dbo.AsycudaDocumentItem.PiQuantity) 
		and (AsycudaDocumentCustomsProcedures.CustomsOperation = 'Warehouse')
--AND (dbo.AsycudaDocumentBasicInfo.DocumentType = 'im7' OR                  dbo.AsycudaDocumentBasicInfo.DocumentType = 'OS7')
AND (ISNULL(dbo.AsycudaDocumentItem.Cancelled, 0) <> 1)
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[26] 4[22] 2[12] 3) )"
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
         Begin Table = "AsycudaDocumentItem"
            Begin Extent = 
               Top = 18
               Left = 412
               Bottom = 173
               Right = 689
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentBasicInfo"
            Begin Extent = 
               Top = 11
               Left = 838
               Bottom = 166
               Right = 1112
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
      Begin ColumnWidths = 47
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
      End' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-Mismatched Allocations-Asycuda'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 11
         Column = 3652
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-Mismatched Allocations-Asycuda'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'TODO-Mismatched Allocations-Asycuda'
GO
