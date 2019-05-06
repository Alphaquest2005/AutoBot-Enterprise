declare @BatchNo int = 4

INSERT INTO [History-Allocations]
                         (BatchNo, Status, QtyAllocated, xLineNumber, InvoiceDate, CustomerName, SalesQuantity, SalesQtyAllocated, InvoiceNo, ItemNumber, ItemDescription, DutyFreePaid, pCNumber, pRegistrationDate, pQuantity, pQtyAllocated, 
                         PiQuantity, SalesFactor, xCNumber, xRegistrationDate, xQuantity, pReferenceNumber, pLineNumber, Cost, Total_CIF_itm, DutyLiability, TaxAmount, pIsAssessed, DoNotAllocateSales, DoNotAllocatePreviousEntry, 
                         WarehouseError, SANumber, xReferenceNumber, TariffCode, Invalid, pExpiryDate, pTariffCode, pItemNumber, EntryDateTime)
SELECT        @BatchNo AS BatchNo, Status, QtyAllocated, xLineNumber, InvoiceDate, CustomerName, SalesQuantity, SalesQtyAllocated, InvoiceNo, ItemNumber, ItemDescription, DutyFreePaid, pCNumber, pRegistrationDate, pQuantity, 
                         pQtyAllocated, PiQuantity, SalesFactor, xCNumber, xRegistrationDate, xQuantity, pReferenceNumber, pLineNumber, Cost, Total_CIF_itm, DutyLiability, TaxAmount, pIsAssessed, DoNotAllocateSales, 
                         DoNotAllocatePreviousEntry, WarehouseError, SANumber, xReferenceNumber, TariffCode, Invalid, pExpiryDate, pTariffCode, pItemNumber, GETDATE() AS EntryDateTime
FROM            AsycudaSalesAllocationsEx

