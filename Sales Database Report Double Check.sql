

--UPDATE       AllData
--SET                RefNumber = AsycudaDocument.ReferenceNumber
--FROM            AllData INNER JOIN
--                         AsycudaDocument ON AllData.RefNumber = AsycudaDocument.CNumber

--update alldata
--set refnumber = REPLACE(refnumber, 'DF SALES', 'DF-Nov')

--update alldata
--set refnumber = REPLACE(refnumber, 'DP SALES', 'DP-Nov')

--update alldata
--set refnumber = REPLACE(refnumber, ' ', '')

--update alldata
--set refnumber = REPLACE(refnumber, 'DF-Nov-43561-F100', 'DF-NOC-43561-F100')

--update alldata
--set refnumber = REPLACE(refnumber, 'DF-Nov-22434-F126', 'DF-Nov-22434-F122')

declare @RefNumber varchar(50), @isAssessed bit
set @RefNumber = N'%mar2018%'
set @isAssessed = 0


select 'Sales Data'
drop table #sales
Select * into #sales
from
(SELECT        xReferenceNumber as RefNumber, xLineNumber AS SalesLine, SUM(QtyAllocated) AS SalesQuantity, ItemNumber, pCNumber as PreviousCNumber, pLineNumber as PreviousLineNumber
FROM            AsycudaSalesAllocationsEx
GROUP BY xReferenceNumber, xLineNumber, ItemNumber, pCNumber, pLineNumber
HAVING        (xReferenceNumber LIKE @refnumber)) as t

select * from #sales

select 'xEntries'
drop table #xEntries
Select * into #xEntries
from
(SELECT        AsycudaDocument.ReferenceNumber, AsycudaDocumentItem.RegistrationDate, AsycudaDocumentItem.CNumber, AsycudaDocumentItem.LineNumber AS xLine, SUM(AsycudaDocumentItem.ItemQuantity) AS xQuantity, 
                         AsycudaDocumentItem.ItemNumber, xcuda_PreviousItem.Prev_reg_nbr AS PreviousCNumber, xcuda_PreviousItem.Previous_item_number AS PreviousLineNumber, 
                         CASE WHEN SalesFactor = 0 THEN 1 ELSE salesfactor END AS SalesFactor
FROM            AsycudaDocumentItem INNER JOIN
                         AsycudaDocument ON AsycudaDocumentItem.AsycudaDocumentId = AsycudaDocument.ASYCUDA_Id INNER JOIN
                         xcuda_PreviousItem ON AsycudaDocumentItem.Item_Id = xcuda_PreviousItem.PreviousItem_Id
WHERE        (AsycudaDocumentItem.IsAssessed = @isAssessed)
GROUP BY AsycudaDocumentItem.LineNumber, AsycudaDocumentItem.ItemNumber, AsycudaDocument.ReferenceNumber, AsycudaDocumentItem.CNumber, AsycudaDocumentItem.RegistrationDate, xcuda_PreviousItem.Prev_reg_nbr, 
                         xcuda_PreviousItem.Previous_item_number, CASE WHEN SalesFactor = 0 THEN 1 ELSE salesfactor END
HAVING        (AsycudaDocument.ReferenceNumber IN
                             (SELECT DISTINCT RefNumber
                               FROM            [#sales])) AND (AsycudaDocument.ReferenceNumber LIKE @refnumber)) as s
select * from #xEntries

select 'Sales Reports Differences'

SELECT        xEntries.ReferenceNumber, xEntries.RegistrationDate, xEntries.CNumber, xEntries.xLine, Sales.SalesLine, xEntries.xQuantity, ROUND((Sales.SalesQuantity / xEntries.SalesFactor), 2) AS SalesQuantity, 
                         xEntries.ItemNumber, Sales.ItemNumber AS SalesName, xEntries.SalesFactor, xEntries.PreviousCNumber, xEntries.PreviousLineNumber
FROM          #sales AS Sales LEFT OUTER JOIN
                             #xEntries as xEntries ON Sales.RefNumber = xEntries.ReferenceNumber AND 
                         ROUND((Sales.SalesQuantity / xEntries.SalesFactor), 2) <> ISNULL(xEntries.xQuantity, 0) AND Sales.PreviousCNumber = xEntries.PreviousCNumber AND 
                         Sales.PreviousLineNumber = xEntries.PreviousLineNumber
GROUP BY xEntries.ReferenceNumber, xEntries.RegistrationDate, xEntries.CNumber, xEntries.xLine, xEntries.xQuantity, Sales.SalesQuantity, xEntries.ItemNumber, Sales.SalesLine, xEntries.SalesFactor, Sales.ItemNumber, xEntries.PreviousCNumber, xEntries.PreviousLineNumber
HAVING        (xEntries.ReferenceNumber LIKE @RefNumber)
ORDER BY xEntries.ReferenceNumber, xEntries.xLine


---///////////////////// missing xitems but on sales report
select 'missing xitems but on sales report'

SELECT        Sales.RefNumber, NULL AS RegistrationDate, NULL AS CNumber, NULL AS xLineNumber, Sales.SalesLine, NULL AS ItemQuantity, Sales.SalesQuantity, Sales.ItemNumber, Sales.PreviousCNumber, 
                         Sales.PreviousLineNumber
FROM            #sales AS Sales LEFT OUTER JOIN
                             #xEntries as xEntries ON Sales.PreviousCNumber = xEntries.PreviousCNumber AND 
                         Sales.PreviousLineNumber = xEntries.PreviousLineNumber
WHERE        (Sales.RefNumber LIKE @RefNumber) AND (xEntries.PreviousCNumber IS NULL)




---///////////////////// missing sales  but on  xitems
select 'missing sales  but on  xitems'
SELECT        xEntries.ReferenceNumber, xEntries.RegistrationDate, xEntries.CNumber, xEntries.xLine, NULL AS SalesLine, xEntries.xQuantity, NULL AS SalesQty, xEntries.ItemNumber, Null as SalesName,xEntries.SalesFactor, xEntries.PreviousCnumber, 
                         xEntries.PreviousLineNumber
FROM            #sales AS Sales RIGHT OUTER JOIN
                             #xEntries as xEntries ON Sales.PreviousCNumber = xEntries.PreviousCnumber AND 
                         Sales.PreviousLineNumber = xEntries.PreviousLineNumber
WHERE        (xEntries.ReferenceNumber LIKE @RefNumber) AND (Sales.ItemNumber IS NULL)


select distinct refnumber from #sales

