USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AsycudaDocumentItem]    Script Date: 3/27/2025 1:48:23 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO









--select * from AsycudaDocumentItem


CREATE VIEW [dbo].[AsycudaDocumentItem]
AS
SELECT TOP (9999999) xcuda_Item.Item_Id, xcuda_Item.ASYCUDA_Id AS AsycudaDocumentId, xcuda_Item.EntryDataDetailsId, CAST(xcuda_Item.LineNumber AS nvarchar(50)) AS LineNumber, xcuda_Item.IsAssessed, 
                 xcuda_Item.DoNotAllocate, xcuda_Item.DoNotEX, xcuda_Item.AttributeOnlyAllocation, xcuda_Goods_description_1.Description_of_goods, xcuda_Goods_description_1.Commercial_Description, 
                 ISNULL(dbo.xcuda_Weight_itm.Gross_weight_itm, 0) AS Gross_weight_itm, ISNULL(dbo.xcuda_Weight_itm.Net_weight_itm, 0) AS Net_weight_itm, dbo.xcuda_Tarification.Item_price, 
                 CAST(dbo.Primary_Supplementary_Unit.Suppplementary_unit_quantity AS float) AS ItemQuantity, dbo.Primary_Supplementary_Unit.Suppplementary_unit_code, dbo.xcuda_HScode.Precision_4 AS ItemNumber, 
                 dbo.xcuda_HScode.Commodity_code AS TariffCode, dbo.TariffCodes.LicenseRequired AS TariffCodeLicenseRequired, dbo.TariffCategory.LicenseRequired AS TariffCategoryLicenseRequired, 
                 dbo.TariffCodes.Description AS TariffCodeDescription, dbo.AsycudaItemDutyLiablity.DutyLiability, dbo.AsycudaDocumentItemValueWeights.Total_CIF_itm, 
                 dbo.xcuda_item_external_freight.Amount_national_currency AS Freight, CAST(0 AS float) AS Statistical_value, xcuda_Item.DPQtyAllocated, xcuda_Item.DFQtyAllocated, xcuda_Item.ImportComplete, 
                 xcuda_Registration_1.Number AS CNumber, CAST(xcuda_Registration_1.Date AS datetime) AS RegistrationDate, dbo.xcuda_Packages.Number_of_packages, dbo.xcuda_Goods_description.Country_of_origin_code, 
                 PiQuantities.PiWeight, dbo.xcuda_Item_Invoice.Currency_rate, dbo.xcuda_Item_Invoice.Currency_code, dbo.TariffCodes.Invalid AS InvalidHSCode, xcuda_Item.WarehouseError, PiQuantities.PiQuantity, 
                 xcuda_Item.SalesFactor, dbo.xcuda_Declarant.Number AS ReferenceNumber, xcuda_Item.PreviousInvoiceNumber, xcuda_Item.PreviousInvoiceLineNumber, xcuda_Item.PreviousInvoiceItemNumber, 
                 dbo.AsycudaDocumentSet.ApplicationSettingsId, isnull(dbo.xcuda_ASYCUDA_ExtendedProperties.Cancelled,0) as Cancelled, ISNULL(dbo.xcuda_ASYCUDA_ExtendedProperties.EffectiveExpiryDate, DATEADD(d, 
                 CAST((CASE WHEN xcuda_warehouse.Delay = '' THEN 730 ELSE isnull(xcuda_Warehouse.Delay, 730) END) AS int), CAST(xcuda_Registration_1.Date AS DateTime))) AS ExpiryDate, xcuda_Inventory_Item.InventoryItemId,
				 xcuda_Tarification.Extended_customs_procedure + '-' + xcuda_Tarification.National_customs_procedure as CustomsProcedure,
				 
				 ISNULL(case when xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate > CAST(xcuda_Registration_1.Date AS datetime) then CAST(xcuda_Registration_1.Date AS datetime) else xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate end, CASE WHEN isnull(ismanuallyassessed, 0) 
                 = 0 THEN COALESCE (CASE WHEN xcuda_Registration_1.Date = '01/01/0001' THEN NULL ELSE CAST(xcuda_Registration_1.Date AS datetime) END, dbo.xcuda_ASYCUDA_ExtendedProperties.RegistrationDate) 
                 ELSE isnull(registrationdate, case when xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate > CAST(xcuda_Registration_1.Date AS datetime) then CAST(xcuda_Registration_1.Date AS datetime) else xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate end) END) AS AssessmentDate


FROM    dbo.xcuda_HScode WITH (NOLOCK) LEFT OUTER JOIN
                 dbo.TariffCategory WITH (NOLOCK) INNER JOIN
                 dbo.TariffCodes WITH (NOLOCK) ON dbo.TariffCategory.TariffCategoryCode = dbo.TariffCodes.TariffCategoryCode ON dbo.xcuda_HScode.Commodity_code = dbo.TariffCodes.TariffCode FULL OUTER JOIN
                 dbo.xcuda_Warehouse WITH (NOLOCK) RIGHT OUTER JOIN
                     dbo.[AscyudaItemPiQuantity-basic] AS PiQuantities RIGHT OUTER JOIN
                 dbo.xcuda_ASYCUDA_ExtendedProperties INNER JOIN
                 dbo.AsycudaDocumentSet ON dbo.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId = dbo.AsycudaDocumentSet.AsycudaDocumentSetId INNER JOIN
                 dbo.xcuda_Declarant WITH (nolock) INNER JOIN
                     (SELECT TOP (100) PERCENT Amount_deducted_from_licence, Quantity_deducted_from_licence, Item_Id, ASYCUDA_Id, Licence_number, Free_text_1, Free_text_2, EntryDataDetailsId, LineNumber, IsAssessed, 
                                       DPQtyAllocated, DFQtyAllocated, EntryTimeStamp, AttributeOnlyAllocation, DoNotAllocate, DoNotEX, ImportComplete, WarehouseError, SalesFactor, PreviousInvoiceNumber, PreviousInvoiceLineNumber, 
                                       PreviousInvoiceItemNumber
                      FROM     dbo.xcuda_Item AS xcuda_Item_1 WITH (NOLOCK)
                      ORDER BY LineNumber) AS xcuda_Item INNER JOIN
                 dbo.xcuda_Registration AS xcuda_Registration_1 WITH (NOLOCK) ON xcuda_Item.ASYCUDA_Id = xcuda_Registration_1.ASYCUDA_Id ON dbo.xcuda_Declarant.ASYCUDA_Id = xcuda_Item.ASYCUDA_Id ON 
                 dbo.xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = xcuda_Item.ASYCUDA_Id LEFT OUTER JOIN
                 dbo.AsycudaDocumentItemValueWeights WITH (NOLOCK) ON xcuda_Item.Item_Id = dbo.AsycudaDocumentItemValueWeights.Item_Id ON PiQuantities.Item_Id = xcuda_Item.Item_Id ON 
                 dbo.xcuda_Warehouse.ASYCUDA_Id = xcuda_Item.ASYCUDA_Id LEFT OUTER JOIN
                 dbo.AsycudaItemDutyLiablity WITH (NOLOCK) ON xcuda_Item.Item_Id = dbo.AsycudaItemDutyLiablity.Item_Id LEFT OUTER JOIN
                 dbo.xcuda_Goods_description WITH (nolock) ON xcuda_Item.Item_Id = dbo.xcuda_Goods_description.Item_Id LEFT OUTER JOIN
                 dbo.xcuda_Packages WITH (NOLOCK) ON xcuda_Item.Item_Id = dbo.xcuda_Packages.Item_Id LEFT OUTER JOIN
                 dbo.xcuda_Goods_description AS xcuda_Goods_description_1 WITH (NOLOCK) ON xcuda_Item.Item_Id = xcuda_Goods_description_1.Item_Id LEFT OUTER JOIN
                 dbo.xcuda_Weight_itm WITH (NOLOCK) RIGHT OUTER JOIN
                 dbo.xcuda_Valuation_item WITH (NOLOCK) INNER JOIN
                 dbo.xcuda_Item_Invoice ON dbo.xcuda_Valuation_item.Item_Id = dbo.xcuda_Item_Invoice.Valuation_item_Id ON dbo.xcuda_Weight_itm.Valuation_item_Id = dbo.xcuda_Valuation_item.Item_Id LEFT OUTER JOIN
                 dbo.xcuda_item_external_freight WITH (NOLOCK) ON dbo.xcuda_Valuation_item.Item_Id = dbo.xcuda_item_external_freight.Valuation_item_Id ON 
                 xcuda_Item.Item_Id = dbo.xcuda_Valuation_item.Item_Id LEFT OUTER JOIN
                 dbo.EntryPreviousItems WITH (NOLOCK) ON xcuda_Item.Item_Id = dbo.EntryPreviousItems.Item_Id LEFT OUTER JOIN
                 dbo.Primary_Supplementary_Unit WITH (NOLOCK) RIGHT OUTER JOIN
                 dbo.xcuda_Tarification WITH (NOLOCK) ON dbo.Primary_Supplementary_Unit.Tarification_Id = dbo.xcuda_Tarification.Item_Id ON xcuda_Item.Item_Id = dbo.xcuda_Tarification.Item_Id ON 
                 dbo.xcuda_HScode.Item_Id = dbo.xcuda_Tarification.Item_Id
				 left outer join xcuda_Inventory_Item on xcuda_HScode.Item_Id = xcuda_Inventory_Item.Item_Id
GROUP BY xcuda_Item.Item_Id, xcuda_Item.ASYCUDA_Id, xcuda_Item.EntryDataDetailsId, xcuda_Item.LineNumber, xcuda_Goods_description_1.Description_of_goods, 
                 xcuda_Goods_description_1.Commercial_Description, dbo.xcuda_Weight_itm.Gross_weight_itm, dbo.xcuda_Weight_itm.Net_weight_itm, dbo.xcuda_Tarification.Item_price, 
                 dbo.Primary_Supplementary_Unit.Suppplementary_unit_quantity, dbo.Primary_Supplementary_Unit.Suppplementary_unit_code, dbo.xcuda_HScode.Precision_4, dbo.xcuda_HScode.Commodity_code, 
                 dbo.TariffCodes.Description, dbo.xcuda_item_external_freight.Amount_national_currency, xcuda_Item.DPQtyAllocated, xcuda_Item.DFQtyAllocated, xcuda_Item.IsAssessed, xcuda_Item.DoNotAllocate, 
                 xcuda_Item.DoNotEX, dbo.TariffCodes.LicenseRequired, dbo.TariffCategory.LicenseRequired, xcuda_Item.AttributeOnlyAllocation, xcuda_Item.ImportComplete, xcuda_Registration_1.Number, 
                 xcuda_Registration_1.Date, dbo.xcuda_Packages.Number_of_packages, dbo.xcuda_Goods_description.Country_of_origin_code, dbo.xcuda_Item_Invoice.Currency_rate, 
                 dbo.xcuda_Item_Invoice.Currency_code, dbo.TariffCodes.Invalid, xcuda_Item.WarehouseError, dbo.AsycudaItemDutyLiablity.DutyLiability, dbo.AsycudaDocumentItemValueWeights.Total_CIF_itm, 
                 xcuda_Item.SalesFactor, dbo.xcuda_Declarant.Number, dbo.xcuda_ASYCUDA_ExtendedProperties.EffectiveExpiryDate, xcuda_warehouse.Delay, xcuda_Registration_1.Date, xcuda_Item.PreviousInvoiceNumber, 
                 xcuda_Item.PreviousInvoiceLineNumber, xcuda_Item.PreviousInvoiceItemNumber, PiQuantities.PiWeight, PiQuantities.PiQuantity, dbo.AsycudaDocumentSet.ApplicationSettingsId, xcuda_Item.LineNumber, 
                 dbo.xcuda_ASYCUDA_ExtendedProperties.Cancelled, xcuda_Inventory_Item.InventoryItemId,xcuda_Tarification.Extended_customs_procedure, xcuda_Tarification.National_customs_procedure, dbo.xcuda_ASYCUDA_ExtendedProperties.EffectiveRegistrationDate, 
				 dbo.xcuda_ASYCUDA_ExtendedProperties.IsManuallyAssessed,dbo.xcuda_ASYCUDA_ExtendedProperties.RegistrationDate
ORDER BY Cast(LineNumber as float)

--SELECT TOP (9999999) xcuda_Item.Item_Id, xcuda_Item.ASYCUDA_Id AS AsycudaDocumentId, xcuda_Item.EntryDataDetailsId, CAST(xcuda_Item.LineNumber AS nvarchar(50)) AS LineNumber, xcuda_Item.IsAssessed, 
--                 xcuda_Item.DoNotAllocate, xcuda_Item.DoNotEX, xcuda_Item.AttributeOnlyAllocation, xcuda_Goods_description_1.Description_of_goods, xcuda_Goods_description_1.Commercial_Description, 
--                 ISNULL(dbo.xcuda_Weight_itm.Gross_weight_itm, 0) AS Gross_weight_itm, ISNULL(dbo.xcuda_Weight_itm.Net_weight_itm, 0) AS Net_weight_itm, dbo.xcuda_Tarification.Item_price, 
--                 CAST(dbo.Primary_Supplementary_Unit.Suppplementary_unit_quantity AS float) AS ItemQuantity, dbo.Primary_Supplementary_Unit.Suppplementary_unit_code, dbo.xcuda_HScode.Precision_4 AS ItemNumber, 
--                 dbo.xcuda_HScode.Commodity_code AS TariffCode, dbo.TariffCodes.LicenseRequired AS TariffCodeLicenseRequired, dbo.TariffCategory.LicenseRequired AS TariffCategoryLicenseRequired, 
--                 dbo.TariffCodes.Description AS TariffCodeDescription, dbo.AsycudaItemDutyLiablity.DutyLiability, dbo.AsycudaDocumentItemValueWeights.Total_CIF_itm, 
--                 dbo.xcuda_item_external_freight.Amount_national_currency AS Freight, CAST(0 AS float) AS Statistical_value, xcuda_Item.DPQtyAllocated, xcuda_Item.DFQtyAllocated, xcuda_Item.ImportComplete, 
--                 xcuda_Registration_1.Number AS CNumber, CAST(xcuda_Registration_1.Date AS datetime) AS RegistrationDate, dbo.xcuda_Packages.Number_of_packages, dbo.xcuda_Goods_description.Country_of_origin_code, 
--                 PiQuantities.PiWeight, dbo.xcuda_Item_Invoice.Currency_rate, dbo.xcuda_Item_Invoice.Currency_code, dbo.TariffCodes.Invalid AS InvalidHSCode, xcuda_Item.WarehouseError, PiQuantities.PiQuantity, 
--                 xcuda_Item.SalesFactor, dbo.xcuda_Declarant.Number AS ReferenceNumber, xcuda_Item.PreviousInvoiceNumber, xcuda_Item.PreviousInvoiceLineNumber, xcuda_Item.PreviousInvoiceItemNumber, 
--                 dbo.AsycudaDocumentSet.ApplicationSettingsId, dbo.xcuda_ASYCUDA_ExtendedProperties.Cancelled, ISNULL(dbo.xcuda_ASYCUDA_ExtendedProperties.EffectiveExpiryDate, DATEADD(d, 
--                 CAST((CASE WHEN xcuda_warehouse.Delay = '' THEN 730 ELSE isnull(xcuda_Warehouse.Delay, 730) END) AS int), CAST(xcuda_Registration_1.Date AS DateTime))) AS ExpiryDate, xcuda_Inventory_Item.InventoryItemId
--FROM    dbo.xcuda_HScode WITH (NOLOCK) LEFT OUTER JOIN
--                 dbo.TariffCategory WITH (NOLOCK) INNER JOIN
--                 dbo.TariffCodes WITH (NOLOCK) ON dbo.TariffCategory.TariffCategoryCode = dbo.TariffCodes.TariffCategoryCode ON dbo.xcuda_HScode.Commodity_code = dbo.TariffCodes.TariffCode FULL OUTER JOIN
--                 dbo.xcuda_Warehouse WITH (NOLOCK) RIGHT OUTER JOIN
--                     (SELECT Item_Id, SUM(PiQuantity) AS PiQuantity, SUM(PiWeight) AS PiWeight
--                      FROM     dbo.AscyudaItemPiQuantity AS AscyudaItemPiQuantity_1 WITH (NOLOCK)
--                      GROUP BY Item_Id) AS PiQuantities RIGHT OUTER JOIN
--                 dbo.xcuda_ASYCUDA_ExtendedProperties INNER JOIN
--                 dbo.AsycudaDocumentSet ON dbo.xcuda_ASYCUDA_ExtendedProperties.AsycudaDocumentSetId = dbo.AsycudaDocumentSet.AsycudaDocumentSetId INNER JOIN
--                 dbo.xcuda_Declarant WITH (nolock) INNER JOIN
--                     (SELECT TOP (100) PERCENT Amount_deducted_from_licence, Quantity_deducted_from_licence, Item_Id, ASYCUDA_Id, Licence_number, Free_text_1, Free_text_2, EntryDataDetailsId, LineNumber, IsAssessed, 
--                                       DPQtyAllocated, DFQtyAllocated, EntryTimeStamp, AttributeOnlyAllocation, DoNotAllocate, DoNotEX, ImportComplete, WarehouseError, SalesFactor, PreviousInvoiceNumber, PreviousInvoiceLineNumber, 
--                                       PreviousInvoiceItemNumber
--                      FROM     dbo.xcuda_Item AS xcuda_Item_1 WITH (NOLOCK)
--                      ORDER BY LineNumber) AS xcuda_Item INNER JOIN
--                 dbo.xcuda_Registration AS xcuda_Registration_1 WITH (NOLOCK) ON xcuda_Item.ASYCUDA_Id = xcuda_Registration_1.ASYCUDA_Id ON dbo.xcuda_Declarant.ASYCUDA_Id = xcuda_Item.ASYCUDA_Id ON 
--                 dbo.xcuda_ASYCUDA_ExtendedProperties.ASYCUDA_Id = xcuda_Item.ASYCUDA_Id LEFT OUTER JOIN
--                 dbo.AsycudaDocumentItemValueWeights WITH (NOLOCK) ON xcuda_Item.Item_Id = dbo.AsycudaDocumentItemValueWeights.Item_Id ON PiQuantities.Item_Id = xcuda_Item.Item_Id ON 
--                 dbo.xcuda_Warehouse.ASYCUDA_Id = xcuda_Item.ASYCUDA_Id LEFT OUTER JOIN
--                 dbo.AsycudaItemDutyLiablity WITH (NOLOCK) ON xcuda_Item.Item_Id = dbo.AsycudaItemDutyLiablity.Item_Id LEFT OUTER JOIN
--                 dbo.xcuda_Goods_description WITH (nolock) ON xcuda_Item.Item_Id = dbo.xcuda_Goods_description.Item_Id LEFT OUTER JOIN
--                 dbo.xcuda_Packages WITH (NOLOCK) ON xcuda_Item.Item_Id = dbo.xcuda_Packages.Item_Id LEFT OUTER JOIN
--                 dbo.xcuda_Goods_description AS xcuda_Goods_description_1 WITH (NOLOCK) ON xcuda_Item.Item_Id = xcuda_Goods_description_1.Item_Id LEFT OUTER JOIN
--                 dbo.xcuda_Weight_itm WITH (NOLOCK) RIGHT OUTER JOIN
--                 dbo.xcuda_Valuation_item WITH (NOLOCK) INNER JOIN
--                 dbo.xcuda_Item_Invoice ON dbo.xcuda_Valuation_item.Item_Id = dbo.xcuda_Item_Invoice.Valuation_item_Id ON dbo.xcuda_Weight_itm.Valuation_item_Id = dbo.xcuda_Valuation_item.Item_Id LEFT OUTER JOIN
--                 dbo.xcuda_item_external_freight WITH (NOLOCK) ON dbo.xcuda_Valuation_item.Item_Id = dbo.xcuda_item_external_freight.Valuation_item_Id ON 
--                 xcuda_Item.Item_Id = dbo.xcuda_Valuation_item.Item_Id LEFT OUTER JOIN
--                 dbo.DistinctEntryPreviousItems WITH (NOLOCK) ON xcuda_Item.Item_Id = dbo.DistinctEntryPreviousItems.Item_Id LEFT OUTER JOIN
--                 dbo.Primary_Supplementary_Unit WITH (NOLOCK) RIGHT OUTER JOIN
--                 dbo.xcuda_Tarification WITH (NOLOCK) ON dbo.Primary_Supplementary_Unit.Tarification_Id = dbo.xcuda_Tarification.Item_Id ON xcuda_Item.Item_Id = dbo.xcuda_Tarification.Item_Id ON 
--                 dbo.xcuda_HScode.Item_Id = dbo.xcuda_Tarification.Item_Id
--				 left outer join xcuda_Inventory_Item on xcuda_HScode.Item_Id = xcuda_Inventory_Item.Item_Id
--GROUP BY xcuda_Item.Item_Id, xcuda_Item.ASYCUDA_Id, xcuda_Item.EntryDataDetailsId, CAST(xcuda_Item.LineNumber AS nvarchar(50)), xcuda_Goods_description_1.Description_of_goods, 
--                 xcuda_Goods_description_1.Commercial_Description, ISNULL(dbo.xcuda_Weight_itm.Gross_weight_itm, 0), ISNULL(dbo.xcuda_Weight_itm.Net_weight_itm, 0), dbo.xcuda_Tarification.Item_price, 
--                 CAST(dbo.Primary_Supplementary_Unit.Suppplementary_unit_quantity AS float), dbo.Primary_Supplementary_Unit.Suppplementary_unit_code, dbo.xcuda_HScode.Precision_4, dbo.xcuda_HScode.Commodity_code, 
--                 dbo.TariffCodes.Description, dbo.xcuda_item_external_freight.Amount_national_currency, xcuda_Item.DPQtyAllocated, xcuda_Item.DFQtyAllocated, xcuda_Item.IsAssessed, xcuda_Item.DoNotAllocate, 
--                 xcuda_Item.DoNotEX, dbo.TariffCodes.LicenseRequired, dbo.TariffCategory.LicenseRequired, xcuda_Item.AttributeOnlyAllocation, xcuda_Item.ImportComplete, xcuda_Registration_1.Number, 
--                 CAST(xcuda_Registration_1.Date AS datetime), dbo.xcuda_Packages.Number_of_packages, dbo.xcuda_Goods_description.Country_of_origin_code, dbo.xcuda_Item_Invoice.Currency_rate, 
--                 dbo.xcuda_Item_Invoice.Currency_code, dbo.TariffCodes.Invalid, xcuda_Item.WarehouseError, dbo.AsycudaItemDutyLiablity.DutyLiability, dbo.AsycudaDocumentItemValueWeights.Total_CIF_itm, 
--                 xcuda_Item.SalesFactor, dbo.xcuda_Declarant.Number, ISNULL(dbo.xcuda_ASYCUDA_ExtendedProperties.EffectiveExpiryDate, DATEADD(d, 
--                 CAST((CASE WHEN xcuda_warehouse.Delay = '' THEN 730 ELSE isnull(xcuda_Warehouse.Delay, 730) END) AS int), CAST(xcuda_Registration_1.Date AS DateTime))), xcuda_Item.PreviousInvoiceNumber, 
--                 xcuda_Item.PreviousInvoiceLineNumber, xcuda_Item.PreviousInvoiceItemNumber, PiQuantities.PiWeight, PiQuantities.PiQuantity, dbo.AsycudaDocumentSet.ApplicationSettingsId, xcuda_Item.LineNumber, 
--                 dbo.xcuda_ASYCUDA_ExtendedProperties.Cancelled, xcuda_Inventory_Item.InventoryItemId
--ORDER BY Cast(LineNumber as float)
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane1', @value=N'[0E232FF0-B466-11cf-A24F-00AA00A3EFFF, 1.00]
Begin DesignProperties = 
   Begin PaneConfigurations = 
      Begin PaneConfiguration = 0
         NumPanes = 4
         Configuration = "(H (1[62] 4[17] 2[11] 3) )"
      End
      Begin PaneConfiguration = 1
         NumPanes = 3
         Configuration = "(H (1[58] 4[18] 3) )"
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
         Configuration = "(H (1[75] 4) )"
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
         Begin Table = "AsycudaDocumentSet"
            Begin Extent = 
               Top = 428
               Left = 1634
               Bottom = 717
               Right = 1922
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaDocumentItemValueWeights"
            Begin Extent = 
               Top = 176
               Left = 1049
               Bottom = 306
               Right = 1446
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "PiQuantities"
            Begin Extent = 
               Top = 0
               Left = 669
               Bottom = 134
               Right = 869
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Declarant"
            Begin Extent = 
               Top = 175
               Left = 212
               Bottom = 305
               Right = 446
            End
            DisplayFlags = 280
            TopColumn = 3
         End
         Begin Table = "xcuda_Item"
            Begin Extent = 
               Top = 5
               Left = 69
               Bottom = 160
               Right = 376
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Registration_1"
            Begin Extent = 
               Top = 0
               Left = 1001
               Bottom = 178
               Right = 1239
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Warehouse"
            Begin Extent = 
               Top = 115
               Left = 698
               Bottom = 245' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaDocumentItem'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane2', @value=N'
               Right = 884
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "AsycudaItemDutyLiablity"
            Begin Extent = 
               Top = 169
               Left = 1035
               Bottom = 265
               Right = 1221
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Goods_description"
            Begin Extent = 
               Top = 23
               Left = 1638
               Bottom = 230
               Right = 2071
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Packages"
            Begin Extent = 
               Top = 141
               Left = 641
               Bottom = 346
               Right = 952
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Goods_description_1"
            Begin Extent = 
               Top = 0
               Left = 1914
               Bottom = 160
               Right = 2209
            End
            DisplayFlags = 280
            TopColumn = 2
         End
         Begin Table = "xcuda_Weight_itm"
            Begin Extent = 
               Top = 181
               Left = 2342
               Bottom = 350
               Right = 2585
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Valuation_item"
            Begin Extent = 
               Top = 227
               Left = 562
               Bottom = 397
               Right = 945
            End
            DisplayFlags = 280
            TopColumn = 1
         End
         Begin Table = "xcuda_Item_Invoice"
            Begin Extent = 
               Top = 340
               Left = 44
               Bottom = 545
               Right = 372
            End
            DisplayFlags = 280
            TopColumn = 1
         End
         Begin Table = "xcuda_item_external_freight"
            Begin Extent = 
               Top = 593
               Left = 215
               Bottom = 789
               Right = 527
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "DistinctEntryPreviousItems"
            Begin Extent = 
               Top = 209
               Left = 1528
               Bottom = 360
               Right = 1938
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "Primary_Supplementary_Unit"
            Begin Extent = 
               Top = 369
               Left = 626
               Bottom = 574
               Right = 986
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_Tarification"
            Begin Extent = 
               Top = 47
               Left = 1338
               Bottom = 284
               Right = 1677
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "xcuda_HScode"
            Begin Extent = 
               Top = 251
               Left = 1909
               Bottom = 447
               Right = 2154
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "TariffCategory"
            Begin Extent = 
               Top = 576
               Left = 651
               Bottom = 772
               Right = 959
            End
            DisplayFlags = 280
            TopColumn = 0
         End
         Begin Table = "TariffC' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaDocumentItem'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPane3', @value=N'odes"
            Begin Extent = 
               Top = 502
               Left = 1108
               Bottom = 815
               Right = 1393
            End
            DisplayFlags = 280
            TopColumn = 5
         End
         Begin Table = "xcuda_ASYCUDA_ExtendedProperties"
            Begin Extent = 
               Top = 336
               Left = 1152
               Bottom = 491
               Right = 1413
            End
            DisplayFlags = 280
            TopColumn = 19
         End
      End
   End
   Begin SQLPane = 
   End
   Begin DataPane = 
      Begin ParameterDefaults = ""
      End
      Begin ColumnWidths = 38
         Width = 284
         Width = 2186
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 2343
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1008
         Width = 1505
         Width = 1505
         Width = 1505
         Width = 1505
      End
   End
   Begin CriteriaPane = 
      Begin ColumnWidths = 12
         Column = 12449
         Alias = 1322
         Table = 4097
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaDocumentItem'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=3 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'AsycudaDocumentItem'
GO
