SELECT EntryDataDetails.ItemNumber
FROM   EntryData_Sales  with (nolock)  INNER JOIN
             EntryDataDetails  with (nolock) ON EntryData_Sales.EntryDataId = EntryDataDetails.EntryDataId
WHERE (ISNULL(EntryDataDetails.DoNotAllocate, 0) <> 1) AND (EntryDataDetails.Quantity <> EntryDataDetails.QtyAllocated)
GROUP BY EntryDataDetails.ItemNumber

select count(allocationid)
from asycudasalesallocations with (nolock)

SELECT EntryDataDetailsId, PreviousItem_Id, Status, SUM(QtyAllocated) AS Expr1
FROM   AsycudaSalesAllocations WITH (nolock)
GROUP BY EntryDataDetailsId, PreviousItem_Id, Status