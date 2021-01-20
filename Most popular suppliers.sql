SELECT EntryData.SupplierCode, COUNT(EntryDataDetails.EntryDataDetailsId) AS Expr1
FROM    EntryData_PurchaseOrders INNER JOIN
                 EntryData ON EntryData_PurchaseOrders.EntryData_Id = EntryData.EntryData_Id INNER JOIN
                 EntryDataDetails ON EntryData.EntryData_Id = EntryDataDetails.EntryData_Id
GROUP BY EntryData.SupplierCode
ORDER BY COUNT(EntryDataDetails.EntryDataDetailsId) DESC