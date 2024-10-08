/****** Script for SelectTopNRows command from SSMS  ******/

delete from IWWDB.dbo.EntryDataDetails
where EntryDataDetailsId in (
select EntryDataDetailsId from
(
SELECT EntryDataId, LineNumber, ItemNumber, Quantity, Units, ItemDescription, Cost, QtyAllocated, UnitWeight, max(EntryDataDetailsId) as EntryDataDetailsId
FROM     IWWDB.dbo.EntryDataDetails
GROUP BY EntryDataId, LineNumber, ItemNumber, Quantity, Units, ItemDescription, Cost, QtyAllocated, UnitWeight
HAVING (COUNT(*) > 1)
) as t
)
select * from IWWDB.dbo.EntryDataDetails
where entrydataid = 'GP-0001116'

