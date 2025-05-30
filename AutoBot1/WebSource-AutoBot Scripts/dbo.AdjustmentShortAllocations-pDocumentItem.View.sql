USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AdjustmentShortAllocations-pDocumentItem]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO








CREATE VIEW [dbo].[AdjustmentShortAllocations-pDocumentItem]
AS
SELECT xcuda_Registration.Number AS pCNumber, xcuda_Registration.Date AS pRegistrationDate, Primary_Supplementary_Unit.Suppplementary_unit_quantity AS pQuantity, 
                 pItem.DPQtyAllocated + pItem.DFQtyAllocated AS pQtyAllocated, ISNULL(AscyudaItemPiQuantity.PiQuantity, 0) AS PiQuantity, pItem.SalesFactor, pDeclarant.Number AS pReferenceNumber, 
                 pItem.LineNumber AS pLineNumber, xcuda_Registration.ASYCUDA_Id AS pASYCUDA_Id, xcuda_Valuation_item.Total_CIF_itm, AsycudaItemDutyLiablity.DutyLiability, pItem.IsAssessed AS pIsAssessed, 
                 pItem.DoNotAllocate AS DoNotAllocatePreviousEntry, pItem.WarehouseError, TariffCodes.TariffCode, TariffCodes.Invalid,isnull(EffectiveExpiryDate,  DATEADD(d, 
                 CAST((CASE WHEN xcuda_warehouse.Delay = '' THEN 730 ELSE isnull(xcuda_Warehouse.Delay, 730) END) AS int), xcuda_Registration.Date )) AS pExpiryDate, 
                 xcuda_HScode.Commodity_code AS pTariffCode, xcuda_HScode.Precision_1 AS pPrecision1, xcuda_HScode.Precision_4 AS pItemNumber, ISNULL(xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate, 
                 CASE WHEN isnull(ismanuallyassessed, 0) = 0 THEN COALESCE (CASE WHEN dbo.xcuda_Registration.Date = '01/01/0001' THEN NULL ELSE dbo.xcuda_Registration.Date END, 
                 dbo.xcuda_ASYCUDA_ExtendedProperties.RegistrationDate) ELSE isnull(registrationdate, effectiveregistrationDate) END) AS AssessmentDate, pItem.Item_Id
FROM    xcuda_ASYCUDA_ExtendedProperties WITH (NOLOCK) RIGHT OUTER JOIN
                 xcuda_Registration WITH (NOLOCK) RIGHT OUTER JOIN
                 xcuda_Warehouse WITH (NOLOCK) RIGHT OUTER JOIN
                 xcuda_HScode WITH (NOLOCK) LEFT OUTER JOIN
                 TariffCodes WITH (NOLOCK) ON xcuda_HScode.Commodity_code = TariffCodes.TariffCode RIGHT OUTER JOIN
                 xcuda_Item AS pItem WITH (NOLOCK) ON xcuda_HScode.Item_Id = pItem.Item_Id ON xcuda_Warehouse.ASYCUDA_Id = pItem.ASYCUDA_Id LEFT OUTER JOIN
                 xcuda_Declarant AS pDeclarant WITH (NOLOCK) ON pItem.ASYCUDA_Id = pDeclarant.ASYCUDA_Id LEFT OUTER JOIN
                 AsycudaItemDutyLiablity WITH (NOLOCK) ON pItem.Item_Id = AsycudaItemDutyLiablity.Item_Id LEFT OUTER JOIN
                 xcuda_Valuation_item WITH (NOLOCK) ON pItem.Item_Id = xcuda_Valuation_item.Item_Id LEFT OUTER JOIN
                 [AscyudaItemPiQuantity-Basic] AS AscyudaItemPiQuantity ON pItem.Item_Id = AscyudaItemPiQuantity.Item_Id ON xcuda_Registration.ASYCUDA_Id = pItem.ASYCUDA_Id LEFT OUTER JOIN
                 Primary_Supplementary_Unit WITH (NOLOCK) ON pItem.Item_Id = Primary_Supplementary_Unit.Tarification_Id ON xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = xcuda_Registration.ASYCUDA_Id
--GROUP BY 
--					dbo.xcuda_Registration.Number, dbo.xcuda_Registration.Date, dbo.Primary_Supplementary_Unit.Suppplementary_unit_quantity, pItem.DPQtyAllocated, pItem.DFQtyAllocated, pDeclarant.Number, pItem.LineNumber,
--                 dbo.xcuda_Registration.ASYCUDA_Id, dbo.xcuda_Valuation_item.Total_CIF_itm, dbo.AsycudaItemDutyLiablity.DutyLiability, pItem.IsAssessed, pItem.DoNotAllocate, InventoryItems.TariffCode,
--                 dbo.TariffCodes.Invalid, dbo.xcuda_Warehouse.Delay, dbo.xcuda_Registration.Date, dbo.xcuda_HScode.Commodity_code, dbo.xcuda_HScode.Precision_4, pItem.SalesFactor, pItem.WarehouseError,
--                 AscyudaItemPiQuantity.PiQuantity, dbo.xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate, dbo.xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed, dbo.xcuda_Registration.Date,
--                 dbo.xcuda_ASYCUDA_ExtendedProperties.RegistrationDate, pItem.Item_Id
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
         Begin Table = "xcuda_Registration"
            Begin Extent = 
               Top = 6
               Left = 349
               Bottom = 140
               Right = 549
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Warehouse"
            Begin Extent = 
               Top = 6
               Left = 593
               Bottom = 161
               Right = 793
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_HScode"
            Begin Extent = 
               Top = 6
               Left = 837
               Bottom = 161
               Right = 1053
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "InventoryItems"
            Begin Extent = 
               Top = 6
               Left = 1097
               Bottom = 161
               Right = 1335
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "TariffCodes"
            Begin Extent = 
               Top = 6
               Left = 1379
               Bottom = 161
               Right = 1628
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "pItem"
            Begin Extent = 
               Top = 162
               Left = 44
               Bottom = 317
               Right' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AdjustmentShortAllocations-pDocumentItem'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N' = 351
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "pDeclarant"
            Begin Extent = 
               Top = 162
               Left = 395
               Bottom = 317
               Right = 655
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaItemDutyLiablity"
            Begin Extent = 
               Top = 162
               Left = 699
               Bottom = 275
               Right = 899
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Valuation_item"
            Begin Extent = 
               Top = 162
               Left = 943
               Bottom = 317
               Right = 1266
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AscyudaItemPiQuantity"
            Begin Extent = 
               Top = 162
               Left = 1310
               Bottom = 317
               Right = 1548
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Primary_Supplementary_Unit"
            Begin Extent = 
               Top = 318
               Left = 44
               Bottom = 473
               Right = 340
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
         Or = 1350
         Or = 1350
         Or = 1350
      End
   End
End
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AdjustmentShortAllocations-pDocumentItem'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=2 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AdjustmentShortAllocations-pDocumentItem'
GO
