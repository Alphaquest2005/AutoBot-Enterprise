USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[ItemSalesAsycudaPiSummary-Sales]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO







CREATE VIEW [dbo].[ItemSalesAsycudaPiSummary-Sales]
AS
-- take out sales qty it doubling
SELECT ItemSalesPiSummary.PreviousItem_Id AS PreviousItem_Id, ItemSalesPiSummary.ItemNumber AS ItemNumber, 
                 sum(ISNULL(ItemSalesPiSummary.QtyAllocated , 0)) AS QtyAllocated, ISNULL(ItemSalesPiSummary.pQtyAllocated, 0) AS pQtyAllocated, 
                 ItemSalesPiSummary.pCNumber AS pCNumber, ItemSalesPiSummary.pLineNumber AS pLineNumber, 
				 sum(ISNULL(ItemSalesPiSummary.SalesQty , 0)) AS SalesQty,
                 ItemSalesPiSummary.ApplicationSettingsId AS ApplicationSettingsId, ItemSalesPiSummary.Type, CONVERT(varchar(7), max(ItemSalesPiSummary.EntryDataDate), 126) AS MonthYear, MAX(ItemSalesPiSummary.EntryDataDate) AS EntryDataDate, ItemSalesPiSummary.DutyFreePaid AS DutyFreePaid,
				 ItemSalesPiSummary.Type as EntryDataType--, entrydatadetailsid
FROM    ItemSalesPiSummary 
GROUP BY ItemSalesPiSummary.PreviousItem_Id,ItemSalesPiSummary.ItemNumber, 
                 ItemSalesPiSummary.pCNumber, ItemSalesPiSummary.pLineNumber, CONVERT(varchar(7), ItemSalesPiSummary.EntryDataDate, 126),
                 ItemSalesPiSummary.ApplicationSettingsId, ItemSalesPiSummary.Type, ItemSalesPiSummary.pQtyAllocated, ItemSalesPiSummary.DutyFreePaid, ItemSalesPiSummary.Type--, entrydatadetailsid
GO
