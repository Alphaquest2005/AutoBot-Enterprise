declare @BatchNo int = 2, @itemNumber varchar(50)
set @itemNumber = ''


SELECT [History-Allocations].InvoiceNo, [History-Allocations].InvoiceDate, [History-Allocations].ItemNumber, [History-Allocations].SANumber, [History-Allocations].QtyAllocated AS History, 
                 AsycudaSalesAllocationsEx.QtyAllocated AS [Current]
FROM    [History-Allocations] INNER JOIN
                 AsycudaSalesAllocationsEx ON [History-Allocations].InvoiceDate = AsycudaSalesAllocationsEx.InvoiceDate AND [History-Allocations].InvoiceNo = AsycudaSalesAllocationsEx.InvoiceNo AND 
                 [History-Allocations].ItemNumber = AsycudaSalesAllocationsEx.ItemNumber AND [History-Allocations].pCNumber = AsycudaSalesAllocationsEx.pCNumber AND 
                 [History-Allocations].pRegistrationDate = AsycudaSalesAllocationsEx.pRegistrationDate AND [History-Allocations].pQuantity = AsycudaSalesAllocationsEx.pQuantity AND 
                 [History-Allocations].pItemNumber = AsycudaSalesAllocationsEx.pItemNumber AND [History-Allocations].QtyAllocated <> AsycudaSalesAllocationsEx.QtyAllocated AND 
                 [History-Allocations].SANumber = AsycudaSalesAllocationsEx.SANumber
WHERE ([History-Allocations].BatchNo = @BatchNo) AND ([History-Allocations].ItemNumber LIKE '%' + @ItemNumber + '%')

SELECT [History-Allocations].InvoiceNo, [History-Allocations].InvoiceDate, [History-Allocations].ItemNumber, [History-Allocations].SANumber, [History-Allocations].pCNumber AS [pCNumber-History], 
                 [History-Allocations].pRegistrationDate, AsycudaSalesAllocationsEx.pCNumber AS [pCNumber-Current], AsycudaSalesAllocationsEx.pRegistrationDate AS Expr1, 
                 [History-Allocations].QtyAllocated AS [QuantityAllocated-History], AsycudaSalesAllocationsEx.QtyAllocated AS [QuantityAllocated-Current]
FROM    [History-Allocations] INNER JOIN
                 AsycudaSalesAllocationsEx ON [History-Allocations].InvoiceDate = AsycudaSalesAllocationsEx.InvoiceDate AND [History-Allocations].InvoiceNo = AsycudaSalesAllocationsEx.InvoiceNo AND 
                 [History-Allocations].ItemNumber = AsycudaSalesAllocationsEx.ItemNumber AND [History-Allocations].pCNumber <> AsycudaSalesAllocationsEx.pCNumber
WHERE ([History-Allocations].BatchNo = @BatchNo) AND ([History-Allocations].ItemNumber LIKE '%' + @ItemNumber + '%')