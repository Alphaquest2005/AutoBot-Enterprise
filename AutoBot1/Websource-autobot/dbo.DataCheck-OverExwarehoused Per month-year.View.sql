USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[DataCheck-OverExwarehoused Per month-year]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


CREATE VIEW [dbo].[DataCheck-OverExwarehoused Per month-year]
AS
SELECT Allocations.MonthYear, Allocations.ItemNumber, Allocations.QtyAllocated, ISNULL(PreviousItems.PiQty, 0) AS PiQty, Allocations.PiQuantity, ROUND(Allocations.pQtyAllocated, 2) AS pQtyAllocated, 
                 Allocations.pCNumber, Allocations.pLineNumber, Allocations.DutyFreePaid, Allocations.ApplicationSettingsId
FROM    (SELECT Sales.MonthYear, Sales.SalesFactor, Sales.ItemNumber, Sales.QtyAllocated, Sales.pQtyAllocated, Sales.pCNumber, Sales.pLineNumber, Sales.WarehouseError, Sales.DutyFreePaid, SUM(Pi.PiQuantity) 
                                  AS PiQuantity, Sales.ApplicationSettingsId
                 FROM     (SELECT DATENAME(MONTH, EntryData.InvoiceDate) + '-' + DATENAME(YEAR, EntryData.InvoiceDate) AS MonthYear, 
                                                    CASE WHEN xcuda_Item.SalesFactor = 0 THEN 1 ELSE xcuda_item.SalesFactor END AS SalesFactor, EntryDataDetails.ItemNumber, SUM(AsycudaSalesAllocations.QtyAllocated) AS QtyAllocated, 
                                                    xcuda_Item.DFQtyAllocated + xcuda_Item.DPQtyAllocated AS pQtyAllocated, xcuda_Registration.Number AS pCNumber, xcuda_Item.LineNumber AS pLineNumber, xcuda_Item.WarehouseError, 
                                                    EntryData.DutyFreePaid, EntryData.ApplicationSettingsId
                                   FROM     AsycudaSalesAllocations INNER JOIN
                                                    EntryDataDetails ON AsycudaSalesAllocations.EntryDataDetailsId = EntryDataDetails.EntryDataDetailsId INNER JOIN
                                                    EntryDataEx AS EntryData ON EntryDataDetails.EntryDataId = EntryData.InvoiceNo INNER JOIN
                                                    xcuda_Item ON AsycudaSalesAllocations.PreviousItem_Id = xcuda_Item.Item_Id INNER JOIN
                                                    xcuda_Registration ON xcuda_Item.ASYCUDA_Id = xcuda_Registration.ASYCUDA_Id
                                   GROUP BY DATENAME(MONTH, EntryData.InvoiceDate) + '-' + DATENAME(YEAR, EntryData.InvoiceDate), EntryDataDetails.ItemNumber, xcuda_Item.DFQtyAllocated + xcuda_Item.DPQtyAllocated, 
                                                    xcuda_Item.SalesFactor, xcuda_Registration.Number, xcuda_Item.WarehouseError, xcuda_Item.LineNumber, EntryData.DutyFreePaid, EntryData.ApplicationSettingsId
                                   HAVING (xcuda_Item.WarehouseError IS NULL) AND (SUM(AsycudaSalesAllocations.QtyAllocated) < xcuda_Item.DFQtyAllocated + xcuda_Item.DPQtyAllocated)) AS Sales INNER JOIN
                                      (SELECT SUM(PiQuantity) AS PiQuantity, DATENAME(MONTH, AssessmentDate) + '-' + DATENAME(YEAR, AssessmentDate) AS MonthYear, ItemNumber, DutyFreePaid, ApplicationSettingsId
                                       FROM     AsycudaItemPiQuantityData
                                       GROUP BY AssessmentDate, ItemNumber, DutyFreePaid, ApplicationSettingsId) AS Pi ON Sales.MonthYear = Pi.MonthYear AND Sales.ItemNumber = Pi.ItemNumber AND 
                                  Sales.DutyFreePaid = Pi.DutyFreePaid AND Sales.ApplicationSettingsId = Pi.ApplicationSettingsId
                 GROUP BY Sales.MonthYear, Sales.SalesFactor, Sales.ItemNumber, Sales.QtyAllocated, Sales.pQtyAllocated, Sales.pCNumber, Sales.pLineNumber, Sales.WarehouseError, Sales.DutyFreePaid, 
                                  Sales.ApplicationSettingsId) AS Allocations LEFT OUTER JOIN
                     (SELECT DATENAME(MONTH, PreviousItemsEx.AssessmentDate) + '-' + DATENAME(YEAR, PreviousItemsEx.AssessmentDate) AS MonthYear, ISNULL(InventoryItemAliasEx.AliasName, PreviousItemsEx.ItemNumber) 
                                       AS ItemNumber, SUM(PreviousItemsEx.Suplementary_Quantity * PreviousItemsEx.SalesFactor) AS PiQty, PreviousItemsEx.Prev_reg_nbr AS pCNumber, 
                                       PreviousItemsEx.Previous_item_number AS pLineNumber, PreviousItemsEx.DutyFreePaid, PreviousItemsEx.ApplicationSettingsId
                      FROM     PreviousItemsEx LEFT OUTER JOIN
                                       InventoryItemAliasEx ON PreviousItemsEx.ApplicationSettingsId = InventoryItemAliasEx.ApplicationSettingsId AND PreviousItemsEx.ItemNumber = InventoryItemAliasEx.ItemNumber
                      GROUP BY DATENAME(MONTH, PreviousItemsEx.AssessmentDate) + '-' + DATENAME(YEAR, PreviousItemsEx.AssessmentDate), ISNULL(InventoryItemAliasEx.AliasName, PreviousItemsEx.ItemNumber), 
                                       PreviousItemsEx.Prev_reg_nbr, PreviousItemsEx.Previous_item_number, PreviousItemsEx.DutyFreePaid, PreviousItemsEx.ApplicationSettingsId) AS PreviousItems ON 
                 Allocations.ApplicationSettingsId = PreviousItems.ApplicationSettingsId AND Allocations.DutyFreePaid = PreviousItems.DutyFreePaid AND Allocations.MonthYear = PreviousItems.MonthYear AND 
                 Allocations.ItemNumber = PreviousItems.ItemNumber AND Allocations.pCNumber = PreviousItems.pCNumber AND Allocations.pLineNumber = PreviousItems.pLineNumber AND ROUND(Allocations.pQtyAllocated, 2) 
                 > ROUND(ISNULL(PreviousItems.PiQty, 0) * Allocations.SalesFactor, 2)
WHERE (ROUND(Allocations.PiQuantity * Allocations.SalesFactor, 2) > ROUND(Allocations.pQtyAllocated, 2)) AND (Allocations.WarehouseError IS NULL) AND (ROUND(Allocations.pQtyAllocated / Allocations.SalesFactor, 2) > 0) 
                 AND (ROUND(Allocations.PiQuantity, 2) <> ROUND(ISNULL(PreviousItems.PiQty, 0), 2))
GO
