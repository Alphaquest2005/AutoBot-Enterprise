SELECT        TOP (1000) LineNumber, ItemNumber, Commercial_Description, ItemQuantity, Suppplementary_unit_code, TariffCode, LEN(ItemNumber) AS Expr1
FROM            AsycudaDocumentItem
WHERE        (ReferenceNumber = 'OPS-2018-10-01LM-F3')
ORDER BY Expr1 DESC