declare @BatchNo int = 1

-- insert into issues and automatically update allocations

INSERT INTO [History-Allocations]
                         (BatchNo, Status, QtyAllocated, xLineNumber, InvoiceDate, CustomerName, SalesQuantity, SalesQtyAllocated, InvoiceNo, invoicelineNumber, ItemNumber, ItemDescription, DutyFreePaid, pCNumber, pRegistrationDate, pQuantity, pQtyAllocated, 
                         PiQuantity, SalesFactor, xCNumber, xRegistrationDate, xQuantity, pReferenceNumber, pLineNumber, Cost, Total_CIF_itm, DutyLiability, TaxAmount, pIsAssessed, DoNotAllocateSales, DoNotAllocatePreviousEntry, 
                         WarehouseError, SANumber, xReferenceNumber, TariffCode, Invalid, pExpiryDate, pTariffCode, pItemNumber, EntryDateTime,xStatus, ApplicationSettingsId)
SELECT        @BatchNo AS BatchNo, Status, QtyAllocated, xLineNumber, InvoiceDate, CustomerName, SalesQuantity, SalesQtyAllocated, InvoiceNo, SalesLineNumber, ItemNumber, ItemDescription, DutyFreePaid, pCNumber, pRegistrationDate, pQuantity, 
                         pQtyAllocated, PiQuantity, SalesFactor, xCNumber, xRegistrationDate, xQuantity, pReferenceNumber, pLineNumber, Cost, Total_CIF_itm, DutyLiability, TaxAmount, pIsAssessed, DoNotAllocateSales, 
                         DoNotAllocatePreviousEntry, WarehouseError, SANumber, xReferenceNumber, TariffCode, Invalid, pExpiryDate, pTariffCode, pItemNumber, GETDATE() AS EntryDateTime,xStatus , ApplicationSettingsId
FROM            AsycudaSalesAndAdjustmentAllocationsEx
--where ItemNumber = '320865'
-- delete from [History-Allocations] where ItemNumber <> '320865'


select itemnumber from AsycudaSalesAndAdjustmentAllocationsEx where status = 'Returned More than Sold' and ApplicationSettingsId = 6