declare @BatchNo int = 1, @itemNumber varchar(50)
set @itemNumber = '0211-0280MDZBLU'


select * from  AsycudaSalesAndAdjustmentAllocationsEx where ItemNumber LIKE '%' + @ItemNumber + '%'

select * from  [History-Allocations] where ItemNumber LIKE '%' + @ItemNumber + '%'

select 'allocation Differences'
SELECT [History-Allocations].InvoiceNo, [History-Allocations].InvoiceDate, [History-Allocations].ItemNumber, [History-Allocations].SANumber, [History-Allocations].QtyAllocated AS History, 
                 AsycudaSalesAllocationsEx.QtyAllocated AS [Current]
FROM    [History-Allocations] INNER JOIN
                AsycudaSalesAndAdjustmentAllocationsEx as AsycudaSalesAllocationsEx ON [History-Allocations].InvoiceDate = AsycudaSalesAllocationsEx.InvoiceDate AND [History-Allocations].InvoiceNo = AsycudaSalesAllocationsEx.InvoiceNo AND 
                 [History-Allocations].ItemNumber = AsycudaSalesAllocationsEx.ItemNumber AND [History-Allocations].pCNumber = AsycudaSalesAllocationsEx.pCNumber AND 
                 [History-Allocations].pRegistrationDate = AsycudaSalesAllocationsEx.pRegistrationDate AND [History-Allocations].pQuantity = AsycudaSalesAllocationsEx.pQuantity AND 
                                  [History-Allocations].SANumber = AsycudaSalesAllocationsEx.SANumber
								  ----- logic here
								  AND [History-Allocations].pItemNumber = AsycudaSalesAllocationsEx.pItemNumber AND ([History-Allocations].QtyAllocated <> AsycudaSalesAllocationsEx.QtyAllocated 
								 or  [History-Allocations].Status <> AsycudaSalesAllocationsEx.Status  )

WHERE ([History-Allocations].BatchNo = @BatchNo) AND ([History-Allocations].ItemNumber LIKE '%' + @ItemNumber + '%')

select 'p Differences'

SELECT [History-Allocations].InvoiceNo, [History-Allocations].InvoiceDate, [History-Allocations].ItemNumber, [History-Allocations].SANumber, [History-Allocations].pCNumber AS [pCNumber-History], 
                 [History-Allocations].pRegistrationDate, AsycudaSalesAllocationsEx.pCNumber AS [pCNumber-Current], AsycudaSalesAllocationsEx.pRegistrationDate AS Expr1, 
                 [History-Allocations].QtyAllocated AS [QuantityAllocated-History], AsycudaSalesAllocationsEx.QtyAllocated AS [QuantityAllocated-Current]
FROM    [History-Allocations] INNER JOIN
                AsycudaSalesAndAdjustmentAllocationsEx as AsycudaSalesAllocationsEx ON [History-Allocations].InvoiceDate = AsycudaSalesAllocationsEx.InvoiceDate AND [History-Allocations].InvoiceNo = AsycudaSalesAllocationsEx.InvoiceNo AND 
                 [History-Allocations].ItemNumber = AsycudaSalesAllocationsEx.ItemNumber AND ( [History-Allocations].pCNumber <> AsycudaSalesAllocationsEx.pCNumber
				 or [History-Allocations].pLineNumber <> AsycudaSalesAllocationsEx.pLineNumber
				 or [History-Allocations].QtyAllocated <> AsycudaSalesAllocationsEx.QtyAllocated 
								  or [History-Allocations].Status <> AsycudaSalesAllocationsEx.Status  )
WHERE ([History-Allocations].BatchNo = @BatchNo) AND ([History-Allocations].ItemNumber LIKE '%' + @ItemNumber + '%')

select 'x Differences'

SELECT [History-Allocations].InvoiceNo, [History-Allocations].InvoiceDate, [History-Allocations].ItemNumber, [History-Allocations].SANumber, [History-Allocations].xCNumber AS [xCNumber-History], 
                 [History-Allocations].xRegistrationDate, AsycudaSalesAllocationsEx.xCNumber AS [pCNumber-Current], AsycudaSalesAllocationsEx.xRegistrationDate AS xCurrentRegistrationDate, 
                 [History-Allocations].QtyAllocated AS [QuantityAllocated-History], AsycudaSalesAllocationsEx.QtyAllocated AS [QuantityAllocated-Current]
FROM    [History-Allocations] INNER JOIN
                AsycudaSalesAndAdjustmentAllocationsEx as AsycudaSalesAllocationsEx ON [History-Allocations].InvoiceDate = AsycudaSalesAllocationsEx.InvoiceDate AND [History-Allocations].InvoiceNo = AsycudaSalesAllocationsEx.InvoiceNo AND 
                 [History-Allocations].ItemNumber = AsycudaSalesAllocationsEx.ItemNumber AND ( [History-Allocations].xCNumber <> AsycudaSalesAllocationsEx.xCNumber
				 or [History-Allocations].xLineNumber <> AsycudaSalesAllocationsEx.xLineNumber
				 or [History-Allocations].xQuantity <> AsycudaSalesAllocationsEx.xQuantity or [History-Allocations].xStatus <> AsycudaSalesAllocationsEx.xStatus  )
WHERE ([History-Allocations].BatchNo = @BatchNo) AND ([History-Allocations].ItemNumber LIKE '%' + @ItemNumber + '%')