USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ItemSalesAsycudaPiSummary]    Script Date: 3/27/2025 1:48:24 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO








CREATE VIEW [dbo].[ItemSalesAsycudaPiSummary]
AS

select * from [ItemSalesAsycudaPiSummary-CTE]


--SELECT  cast(ROW_NUMBER() OVER (ORDER BY COALESCE (ItemSalesPiSummary.PreviousItem_Id, AsycudaItemPiQuantityData.PreviousItem_Id)) AS int) AS Id,
--			     COALESCE (ItemSalesPiSummary.PreviousItem_Id, AsycudaItemPiQuantityData.PreviousItem_Id) AS PreviousItem_Id, COALESCE (AsycudaItemPiQuantityData.ItemNumber,ItemSalesPiSummary.ItemNumber ) AS ItemNumber, 
--                 ISNULL(ItemSalesPiSummary.SalesQty , 0) AS SalesQty, sum(ISNULL(ItemSalesPiSummary.QtyAllocated , 0)) AS QtyAllocated, ISNULL(ItemSalesPiSummary.pQtyAllocated, 0) AS pQtyAllocated, isnull(PiQuantity, 0) as PiQuantity, 
--                 COALESCE (ItemSalesPiSummary.pCNumber, AsycudaItemPiQuantityData.pCNumber) AS pCNumber, COALESCE (ItemSalesPiSummary.pLineNumber, AsycudaItemPiQuantityData.pLineNumber) AS pLineNumber, 
--                 COALESCE (ItemSalesPiSummary.ApplicationSettingsId, AsycudaItemPiQuantityData.ApplicationSettingsId) AS ApplicationSettingsId, ItemSalesPiSummary.Type, COALESCE (ItemSalesPiSummary.MonthYear, AsycudaItemPiQuantityData.MonthYear) AS MonthYear, MAX(COALESCE (ItemSalesPiSummary.EntryDataDate, 
--                 AsycudaItemPiQuantityData.EntryDataDate)) AS EntryDataDate, COALESCE (ItemSalesPiSummary.DutyFreePaid, AsycudaItemPiQuantityData.DutyFreePaid) AS DutyFreePaid,
--				 COALESCE (ItemSalesPiSummary.EntryDataType, AsycudaItemPiQuantityData.EntryDataType) AS EntryDataType--, entrydatadetailsid

--FROM    [ItemSalesAsycudaPiSummary-Sales] as ItemSalesPiSummary FULL OUTER JOIN
--                 [ItemSalesAsycudaPiSummary-Asycuda] as AsycudaItemPiQuantityData ON ItemSalesPiSummary.DutyFreePaid = AsycudaItemPiQuantityData.DutyFreePaid AND ItemSalesPiSummary.PreviousItem_Id = AsycudaItemPiQuantityData.PreviousItem_Id AND 
--                 ItemSalesPiSummary.MonthYear = AsycudaItemPiQuantityData.MonthYear
--GROUP BY ItemSalesPiSummary.PreviousItem_Id, AsycudaItemPiQuantityData.PreviousItem_Id, AsycudaItemPiQuantityData.ItemNumber, ItemSalesPiSummary.ItemNumber, 
--               AsycudaItemPiQuantityData.pCNumber,ItemSalesPiSummary.pLineNumber, AsycudaItemPiQuantityData.pLineNumber, ItemSalesPiSummary.MonthYear,  AsycudaItemPiQuantityData.MonthYear,
--               ItemSalesPiSummary.ApplicationSettingsId, AsycudaItemPiQuantityData.ApplicationSettingsId, ItemSalesPiSummary.Type, ItemSalesPiSummary.pQtyAllocated, ItemSalesPiSummary.DutyFreePaid, AsycudaItemPiQuantityData.DutyFreePaid,AsycudaItemPiQuantityData.PiQuantity,AsycudaItemPiQuantityData.PreviousItem_Id,
--			ItemSalesPiSummary.EntryDataType, AsycudaItemPiQuantityData.EntryDataType, ItemSalesPiSummary.SalesQty,ItemSalesPiSummary.pCNumber--, entrydatadetailsid
GO
