USE [WebSource-AutoBot]
GO
/****** Object:  View [dbo].[AsycudaSalesAllocationsEx]    Script Date: 4/8/2025 8:33:17 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo].[AsycudaSalesAllocationsEx]
AS
SELECT 
    Allo.AllocationId,
    sales.TotalValue,
    Allo.QtyAllocated * sales.Cost AS AllocatedValue,
    Allo.Status,
    Allo.QtyAllocated,
    xItem.xLineNumber,
    ISNULL(Allo.PreviousItem_Id, 0) AS PreviousItem_Id,
    sales.InvoiceDate,
    sales.SalesQuantity,
    sales.SalesQtyAllocated,
    sales.InvoiceNo,
    sales.SalesLineNumber,
    sales.ItemNumber,
    sales.ItemDescription,
    sales.EntryDataDetailsId,
    ISNULL(xItem.xBond_Item_Id, 0) AS xBond_Item_Id,
    pItem.pCNumber,
    pItem.pRegistrationDate,
    pItem.pQuantity,
    pItem.pQtyAllocated,
    pItem.PiQuantity,
    pItem.SalesFactor,
    xItem.xCNumber,
    xItem.xRegistrationDate,
    xItem.xQuantity,
    pItem.pReferenceNumber,
    pItem.pLineNumber,
    xItem.xASYCUDA_Id,
    pItem.pASYCUDA_Id,
    sales.Cost,
    pItem.Total_CIF_itm,
    pItem.DutyLiability,
    pItem.pIsAssessed,
    sales.DoNotAllocateSales,
    pItem.DoNotAllocatePreviousEntry,
    pItem.WarehouseError,
    xItem.xReferenceNumber,
    pItem.TariffCode,
    pItem.Invalid,
    pItem.pExpiryDate,
    pItem.pTariffCode,
    pItem.pItemNumber,
    sales.EffectiveDate,
    sales.Comment,
    sales.AsycudaDocumentSetId,
    pItem.AssessmentDate,
    sales.DutyFreePaid,
    sales.ApplicationSettingsId,
    sales.FileTypeId,
    sales.EmailId,
    Allo.xStatus,
    sales.Type,
    sales.TaxAmount,
    sales.CustomerName,
    sales.SANumber
FROM 
    AsycudaSalesAllocations AS Allo 
    INNER JOIN [AdjustmentShortAllocations-Sales] AS sales 
        ON Allo.EntryDataDetailsId = sales.EntryDataDetailsId 
    INNER JOIN ApplicationSettings 
        ON sales.ApplicationSettingsId = ApplicationSettings.ApplicationSettingsId 
    LEFT OUTER JOIN [AdjustmentShortAllocations-pDocumentItem] AS pItem 
        ON Allo.PreviousItem_Id = pItem.Item_Id 
    LEFT OUTER JOIN [AdjustmentShortAllocations-xDocumentItem] AS xItem 
        ON Allo.AllocationId = xItem.AllocationId 
           AND (ApplicationSettings.GroupEX9 = 1 
                OR xItem.xQuantity = Allo.QtyAllocated)
GO
