USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ShipmentInvoicePOItemMISData]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO




CREATE view [dbo].[ShipmentInvoicePOItemMISData]
as
SELECT DISTINCT 
                  cast(row_number() OVER ( ORDER BY isnull(INVDetails.DetailId, PODetails.EntryDataDetailsId)) AS int) AS id, PODetails.EntryDataDetailsId AS PODetailsId, INVDetails.DetailId AS INVDetailsId, PODetails.EntryData_Id AS POId, INVDetails.InvoiceId AS INVId, PODetails.InvoiceNo AS PONumber, INVDetails.InvoiceNo, 
                  PODetails.ItemNumber AS POItemCode, INVDetails.ItemNumber AS INVItemCode, PODetails.ItemDescription AS PODescription, INVDetails.ItemDescription AS INVDescription, PODetails.Cost AS POCost, INVDetails.Cost AS INVCost, 
                  PODetails.Quantity AS POQuantity, INVDetails.Quantity AS INVQuantity, INVDetails.SalesFactor AS INVSalesFactor, ROUND(PODetails.totalcost, 2) AS POTotalCost, ROUND(INVDetails.TotalCost, 2) AS INVTotalCost, 
                  ROUND(ISNULL(INVDetails.TotalCost, INVDetails.Cost * (INVDetails.Quantity * INVDetails.SalesFactor)), 2) AS INVAmount, ROUND(ISNULL(PODetails.totalcost, PODetails.Quantity * PODetails.Cost), 2) AS POAmount, 
                  INVDetails.POInventoryItemId, PODetails.InventoryItemId, PODetails.AliasItemId
FROM     [InventoryItemAliasEx] as InventoryItemAlias INNER JOIN
                  ShipmentInvoicePOItemMISPODetails AS PODetails ON InventoryItemAlias.AliasItemId = PODetails.AliasItemId FULL OUTER JOIN
                  ShipmentInvoicePOItemMISINVDetails AS INVDetails ON InventoryItemAlias.AliasName = INVDetails.ItemNumber AND (ISNULL(INVDetails.POInventoryItemId, 0) = ISNULL(PODetails.InventoryItemId, 0)/* OR
                  ISNULL(INVDetails.InventoryItemId, 0) = ISNULL(PODetails.AliasItemId, 0)*/) AND PODetails.EntryData_Id = INVDetails.EntryData_Id AND PODetails.InvoiceId = INVDetails.InvoiceId AND 
                  PODetails.ApplicationSettingsId = INVDetails.ApplicationSettingsId 
WHERE            --(
		(PODetails.EntryData_Id IS not NULL) and
                  (INVDetails.InvoiceId IS not NULL) and       (INVDetails.Quantity <> PODetails.Quantity) and (PODetails.SourceFile <> INVDetails.SourceFile)--) and (PODetails.InvoiceNo = '02714' or INVDetails.InvoiceNo = '15115') and (PODetails.ItemNumber = 'FAA/SSTD8X58')
GROUP BY INVDetails.DetailId, PODetails.EntryDataDetailsId, PODetails.EntryData_Id, INVDetails.InvoiceId, PODetails.InvoiceNo, INVDetails.InvoiceNo, INVDetails.ItemNumber, PODetails.ItemDescription, INVDetails.ItemDescription, 
                  PODetails.Cost, INVDetails.Cost, PODetails.Quantity, INVDetails.Quantity, INVDetails.SalesFactor, ROUND(PODetails.totalcost, 2), ROUND(INVDetails.TotalCost, 2), ROUND(ISNULL(INVDetails.TotalCost, 
                  INVDetails.Cost * (INVDetails.Quantity * INVDetails.SalesFactor)), 2), ROUND(ISNULL(PODetails.totalcost, PODetails.Quantity * PODetails.Cost), 2), INVDetails.POInventoryItemId, PODetails.InventoryItemId, PODetails.ItemNumber, PODetails.AliasItemId

union   

SELECT DISTINCT 
                  cast(row_number() OVER ( ORDER BY isnull(INVDetails.DetailId, PODetails.EntryDataDetailsId)) AS int) AS id, PODetails.EntryDataDetailsId AS PODetailsId, INVDetails.DetailId AS INVDetailsId, PODetails.EntryData_Id AS POId, INVDetails.InvoiceId AS INVId, PODetails.InvoiceNo AS PONumber, INVDetails.InvoiceNo, 
                  PODetails.ItemNumber AS POItemCode, INVDetails.ItemNumber AS INVItemCode, PODetails.ItemDescription AS PODescription, INVDetails.ItemDescription AS INVDescription, PODetails.Cost AS POCost, INVDetails.Cost AS INVCost, 
                  PODetails.Quantity AS POQuantity, INVDetails.Quantity AS INVQuantity, INVDetails.SalesFactor AS INVSalesFactor, ROUND(PODetails.totalcost, 2) AS POTotalCost, ROUND(INVDetails.TotalCost, 2) AS INVTotalCost, 
                  ROUND(ISNULL(INVDetails.TotalCost, INVDetails.Cost * (INVDetails.Quantity * INVDetails.SalesFactor)), 2) AS INVAmount, ROUND(ISNULL(PODetails.totalcost, PODetails.Quantity * PODetails.Cost), 2) AS POAmount, 
                  INVDetails.POInventoryItemId, PODetails.InventoryItemId, MAX(PODetails.AliasItemId) AS AliasItemId
FROM     InventoryItems INNER JOIN
                  ShipmentInvoicePOItemMISINVDetails AS INVDetails ON InventoryItems.Id = INVDetails.POInventoryItemId FULL OUTER JOIN
                  [InventoryItemAliasEx] as InventoryItemAlias INNER JOIN
                  ShipmentInvoicePOItemMISPODetails AS PODetails ON InventoryItemAlias.AliasItemId = PODetails.AliasItemId ON InventoryItems.ItemNumber = PODetails.ItemNumber AND INVDetails.ItemNumber = InventoryItemAlias.AliasName AND 
                  (/*ISNULL(INVDetails.POInventoryItemId, 0) = ISNULL(PODetails.InventoryItemId, 0) OR*/
                  ISNULL(INVDetails.InventoryItemId, 0) = ISNULL(PODetails.AliasItemId, 0)) AND INVDetails.EntryData_Id = PODetails.EntryData_Id AND INVDetails.InvoiceId = PODetails.InvoiceId AND 
                  INVDetails.ApplicationSettingsId = PODetails.ApplicationSettingsId
WHERE  (
		(PODetails.EntryData_Id IS NULL) OR
                  (INVDetails.InvoiceId IS NULL))  and PODetails.SourceFile <> INVDetails.SourceFile -- and (PODetails.InvoiceNo = '02714' or INVDetails.InvoiceNo = '15115') and (PODetails.ItemNumber = 'FAA/SSTD8X58')
GROUP BY INVDetails.DetailId, PODetails.EntryDataDetailsId, PODetails.EntryData_Id, INVDetails.InvoiceId, PODetails.InvoiceNo, INVDetails.InvoiceNo, INVDetails.ItemNumber, PODetails.ItemDescription, INVDetails.ItemDescription, 
                  PODetails.Cost, INVDetails.Cost, PODetails.Quantity, INVDetails.Quantity, INVDetails.SalesFactor, ROUND(PODetails.totalcost, 2), ROUND(INVDetails.TotalCost, 2), ROUND(ISNULL(INVDetails.TotalCost, 
                  INVDetails.Cost * (INVDetails.Quantity * INVDetails.SalesFactor)), 2), ROUND(ISNULL(PODetails.totalcost, PODetails.Quantity * PODetails.Cost), 2), INVDetails.POInventoryItemId, PODetails.InventoryItemId, PODetails.ItemNumber

union
----- remove itemalias contraint

/*, PODetails.AliasItemId*/
SELECT DISTINCT cast(row_number() OVER ( ORDER BY isnull(INVDetails.DetailId, PODetails.EntryDataDetailsId)) AS int) AS id,
                         PODetails.EntryDataDetailsId AS PODetailsId, INVDetails.DetailId AS INVDetailsId, PODetails.EntryData_Id AS POId, INVDetails.InvoiceId AS INVId, PODetails.InvoiceNo AS PONumber, INVDetails.InvoiceNo, 
                         PODetails.ItemNumber AS POItemCode, INVDetails.ItemNumber AS INVItemCode, PODetails.ItemDescription AS PODescription, INVDetails.ItemDescription AS INVDescription, PODetails.Cost AS POCost, 
                         INVDetails.Cost AS INVCost, PODetails.Quantity AS POQuantity, INVDetails.Quantity AS INVQuantity, INVDetails.SalesFactor AS INVSalesFactor, ROUND(PODetails.totalcost, 2) AS POTotalCost, ROUND(INVDetails.TotalCost, 2) 
                         AS INVTotalCost, ROUND(ISNULL(INVDetails.TotalCost, INVDetails.Cost * (INVDetails.Quantity * INVDetails.SalesFactor)), 2) AS INVAmount, ROUND(ISNULL(PODetails.totalcost, PODetails.Quantity * PODetails.Cost), 2) 
                         AS POAmount, INVDetails.POInventoryItemId, PODetails.InventoryItemId, MAX(PODetails.AliasItemId) AS AliasItemId
FROM            InventoryItems AS InventoryItems_1 INNER JOIN
                         ShipmentInvoicePOItemMISINVDetails AS INVDetails ON InventoryItems_1.Id = INVDetails.POInventoryItemId FULL OUTER JOIN
                         InventoryItems INNER JOIN
                         (select * from ShipmentInvoicePOItemMISPODetails where AliasItemId IS NOT NULL) AS PODetails ON InventoryItems.ItemNumber = PODetails.ItemNumber ON INVDetails.Quantity = PODetails.Quantity AND ISNULL(INVDetails.InventoryItemId, 0) 
                         = ISNULL(PODetails.AliasItemId, 0) AND INVDetails.EntryData_Id = PODetails.EntryData_Id AND INVDetails.InvoiceId = PODetails.InvoiceId AND INVDetails.ApplicationSettingsId = PODetails.ApplicationSettingsId and PODetails.SourceFile <> INVDetails.SourceFile
WHERE        ((PODetails.EntryData_Id IS NULL)  OR (INVDetails.InvoiceId IS NULL))  
GROUP BY INVDetails.DetailId, PODetails.EntryDataDetailsId, PODetails.EntryData_Id, INVDetails.InvoiceId, PODetails.InvoiceNo, INVDetails.InvoiceNo, INVDetails.ItemNumber, PODetails.ItemDescription, INVDetails.ItemDescription, 
                         PODetails.Cost, INVDetails.Cost, PODetails.Quantity, INVDetails.Quantity, INVDetails.SalesFactor, ROUND(PODetails.totalcost, 2), ROUND(INVDetails.TotalCost, 2), ROUND(ISNULL(INVDetails.TotalCost, 
                         INVDetails.Cost * (INVDetails.Quantity * INVDetails.SalesFactor)), 2), ROUND(ISNULL(PODetails.totalcost, PODetails.Quantity * PODetails.Cost), 2), INVDetails.POInventoryItemId, PODetails.InventoryItemId, 
                         PODetails.ItemNumber
union
------ this is full outer join on shipmentpo items

	SELECT DISTINCT			cast(row_number() OVER ( ORDER BY isnull(INVDetails.DetailId, PODetails.EntryDataDetailsId)) AS int) AS id,
                         PODetails.EntryDataDetailsId AS PODetailsId, INVDetails.DetailId AS INVDetailsId, PODetails.EntryData_Id AS POId, INVDetails.InvoiceId AS INVId, PODetails.InvoiceNo AS PONumber, INVDetails.InvoiceNo, 
                         PODetails.ItemNumber AS POItemCode, INVDetails.ItemNumber AS INVItemCode, PODetails.ItemDescription AS PODescription, INVDetails.ItemDescription AS INVDescription, PODetails.Cost AS POCost, 
                         INVDetails.Cost AS INVCost, PODetails.Quantity AS POQuantity, INVDetails.Quantity AS INVQuantity, INVDetails.SalesFactor AS INVSalesFactor, ROUND(PODetails.totalcost, 2) AS POTotalCost, ROUND(INVDetails.TotalCost, 2) 
                         AS INVTotalCost, ROUND(ISNULL(INVDetails.TotalCost, INVDetails.Cost * (INVDetails.Quantity * INVDetails.SalesFactor)), 2) AS INVAmount, ROUND(ISNULL(PODetails.totalcost, PODetails.Quantity * PODetails.Cost), 2) 
                         AS POAmount, INVDetails.POInventoryItemId, PODetails.InventoryItemId, MAX(PODetails.AliasItemId) AS AliasItemId
FROM            ShipmentInvoicePOItemMISINVDetails AS INVDetails RIGHT OUTER JOIN
                             (SELECT        ShipmentInvoicePOItemMISINVDetails.InventoryItemId, ShipmentInvoicePOItemMISINVDetails.ItemNumber, ShipmentInvoicePOs.InvoiceId, ShipmentInvoicePOs.EntryData_Id
                               FROM            ShipmentInvoicePOItemMISINVDetails INNER JOIN
                                                         ShipmentInvoicePOs ON ShipmentInvoicePOItemMISINVDetails.InvoiceId = ShipmentInvoicePOs.InvoiceId AND ShipmentInvoicePOItemMISINVDetails.EntryData_Id = ShipmentInvoicePOs.EntryData_Id
							    where POInventoryItemId is null
                               UNION
                               SELECT        ShipmentInvoicePOItemMISPODetails_1.InventoryItemId, ShipmentInvoicePOItemMISPODetails_1.ItemNumber, ShipmentInvoicePOs_1.InvoiceId, ShipmentInvoicePOs_1.EntryData_Id
                               FROM            ShipmentInvoicePOItemMISPODetails AS ShipmentInvoicePOItemMISPODetails_1 INNER JOIN
                                                        ShipmentInvoicePOs AS ShipmentInvoicePOs_1 ON ShipmentInvoicePOItemMISPODetails_1.InvoiceId = ShipmentInvoicePOs_1.InvoiceId AND 
                                                        ShipmentInvoicePOItemMISPODetails_1.EntryData_Id = ShipmentInvoicePOs_1.EntryData_Id
								where AliasItemId is null 
								) AS universiallst ON INVDetails.EntryData_Id = universiallst.EntryData_Id AND 
                         INVDetails.InvoiceId = universiallst.InvoiceId AND INVDetails.InventoryItemId = universiallst.InventoryItemId LEFT OUTER JOIN
                             (SELECT        ApplicationSettingsId, EntryDataDetailsId, EntryData_Id, InvoiceNo, ItemNumber, ItemDescription, TariffCode, Cost, Quantity, totalcost, InventoryItemId, InvoiceId, AliasItemId
                               FROM            ShipmentInvoicePOItemMISPODetails) AS PODetails ON universiallst.InvoiceId = PODetails.InvoiceId AND universiallst.EntryData_Id = PODetails.EntryData_Id AND 
                         universiallst.InventoryItemId = PODetails.InventoryItemId
WHERE        (PODetails.EntryDataDetailsId is null or INVDetails.DetailId is null)  -- and (universiallst.ItemNumber = '	')
GROUP BY INVDetails.DetailId, PODetails.EntryDataDetailsId, PODetails.EntryData_Id, INVDetails.InvoiceId, PODetails.InvoiceNo, INVDetails.InvoiceNo, INVDetails.ItemNumber, PODetails.ItemDescription, INVDetails.ItemDescription, 
                         PODetails.Cost, INVDetails.Cost, PODetails.Quantity, INVDetails.Quantity, INVDetails.SalesFactor, ROUND(PODetails.totalcost, 2), ROUND(INVDetails.TotalCost, 2), ROUND(ISNULL(INVDetails.TotalCost, 
                         INVDetails.Cost * (INVDetails.Quantity * INVDetails.SalesFactor)), 2), ROUND(ISNULL(PODetails.totalcost, PODetails.Quantity * PODetails.Cost), 2), INVDetails.POInventoryItemId, PODetails.InventoryItemId, 
                         PODetails.ItemNumber
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
         Begin Table = "ShipmentInvoicePOItemMISData-Data"
            Begin Extent = 
               Top = 6
               Left = 44
               Bottom = 161
               Right = 271
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
' , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ShipmentInvoicePOItemMISData'
GO
EXEC sys.sp_addextendedproperty @name=N'MS_DiagramPaneCount', @value=1 , @level0type=N'SCHEMA',@level0name=N'dbo', @level1type=N'VIEW',@level1name=N'ShipmentInvoicePOItemMISData'
GO
