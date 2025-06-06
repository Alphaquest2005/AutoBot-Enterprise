USE [WebSource-AutoBot]
GO
/****** Object:  StoredProcedure [dbo].[FixItemIssues]    Script Date: 4/8/2025 8:33:19 AM ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO


-- =============================================
-- Author:		<Author,,Name>
-- Create date: <Create Date,,>
-- Description:	<Description,,>
-- =============================================
CREATE PROCEDURE [dbo].[FixItemIssues]

AS
BEGIN
	-- SET NOCOUNT ON added to prevent extra result sets from
	-- interfering with SELECT statements.
	SET NOCOUNT ON;

    -- Insert statements for procedure here
	
IF OBJECT_ID('tempdb..##inventorywithRow') IS NOT NULL
drop table ##inventorywithRow

select id,trim(itemnumber) as itemnumber, (ROW_NUMBER() over (partition by trim(itemnumber) order by id asc))  row
into ##inventorywithRow
from InventoryItems
group by id,trim(itemnumber)

select * from ##inventorywithRow where row > 1 order by ItemNumber

--select * from ##inventorywithRow where itemnumber = 'AB111510'

UPDATE EntryDataDetails
SET         InventoryItemId = original.Id
FROM    EntryDataDetails INNER JOIN
                     (SELECT id, ItemNumber
                      FROM     [##inventorywithRow]
                      WHERE  (row = 1)  ) AS original ON trim(EntryDataDetails.ItemNumber) = trim(original.ItemNumber) INNER JOIN
                     (SELECT id, ItemNumber
                      FROM     [##inventorywithRow] AS [##inventorywithRow_1]
                      WHERE  (row > 1) ) AS Dups ON trim(EntryDataDetails.ItemNumber) = trim(Dups.ItemNumber) AND EntryDataDetails.InventoryItemId = Dups.id

UPDATE InventoryItemAlias
SET         InventoryItemId = original.Id
--SELECT original.id, original.ItemNumber, Dups.id AS Expr1, Dups.ItemNumber AS Expr2
FROM    InventoryItemAlias INNER JOIN
                     (SELECT id, trim(ItemNumber) as ItemNumber
                      FROM     [##inventorywithRow] AS [##inventorywithRow_1]
                      WHERE  (row > 1)) AS Dups ON InventoryItemAlias.InventoryItemId = Dups.id INNER JOIN
                     (SELECT id, trim(ItemNumber) as ItemNumber
                      FROM     [##inventorywithRow]
                      WHERE  (row = 1)) AS original ON trim(Dups.ItemNumber) = trim(original.ItemNumber) AND Dups.id <> original.id and InventoryItemAlias.AliasItemId <> original.id




UPDATE [InventoryItems-Lumped]
SET         InventoryItemId = original.id
FROM    [InventoryItems-Lumped] INNER JOIN
                     (SELECT id, trim(ItemNumber) as ItemNumber
                      FROM     [##inventorywithRow] AS [##inventorywithRow_1]
                      WHERE  (row > 1)) AS Dups ON [InventoryItems-Lumped].InventoryItemId = Dups.id INNER JOIN
                     (SELECT id, trim(ItemNumber) as ItemNumber
                      FROM     [##inventorywithRow]
                      WHERE  (row = 1)) AS original ON trim(Dups.ItemNumber) = trim(original.ItemNumber) AND Dups.id <> original.id

UPDATE [InventoryItems-NonStock]
SET         InventoryItemId = original.id
FROM    [InventoryItems-NonStock] INNER JOIN
                     (SELECT id, trim(ItemNumber) as ItemNumber
                      FROM     [##inventorywithRow] AS [##inventorywithRow_1]
                      WHERE  (row > 1)) AS Dups ON [InventoryItems-NonStock].InventoryItemId = Dups.id INNER JOIN
                     (SELECT id, trim(ItemNumber) as ItemNumber
                      FROM     [##inventorywithRow]
                      WHERE  (row = 1)) AS original ON trim(Dups.ItemNumber) = trim(original.ItemNumber) AND Dups.id <> original.id

UPDATE InventoryItemSource
SET         InventoryId = original.Id
FROM    InventoryItemSource INNER JOIN
                     (SELECT Id, ItemNumber
                      FROM     [##inventorywithRow] AS [##inventorywithRow_1]
                      WHERE  (row > 1)) AS Dups ON InventoryItemSource.InventoryId = Dups.Id INNER JOIN
                     (SELECT Id, ItemNumber
                      FROM     [##inventorywithRow]
                      WHERE  (row = 1)) AS original ON trim(Dups.ItemNumber) = trim(original.ItemNumber) AND Dups.Id <> original.Id

UPDATE xcuda_Inventory_Item
SET         InventoryItemId = original.id
FROM    xcuda_Inventory_Item INNER JOIN
                     (SELECT id, ItemNumber
                      FROM     [##inventorywithRow] AS [##inventorywithRow_1]
                      WHERE  (row > 1)) AS Dups ON xcuda_Inventory_Item.InventoryItemId = Dups.id INNER JOIN
                     (SELECT id, ItemNumber
                      FROM     [##inventorywithRow]
                      WHERE  (row = 1)) AS original ON trim(Dups.ItemNumber) = trim(original.ItemNumber) AND Dups.id <> original.id

--select * from [xcuda_Inventory_Item] where InventoryItemId in (select id from ##inventorywithRow where row > 1)
delete from InventoryItems where id in (SELECT id
FROM    [##inventorywithRow] left outer join
                 InventoryItemAlias on [##inventorywithRow].Id = InventoryItemAlias.AliasItemId
WHERE (row > 1) and  InventoryItemAlias.AliasItemId is null)


delete from InventoryItemAlias
where AliasId in
(select aliasid from (select AliasId, ROW_NUMBER() over (partition by inventoryitemid, aliasitemid order by inventoryitemid ) as rownum from InventoryItemAlias) as t 
where t.rownum > 1)

delete from InventoryItemAlias
where AliasId in
(SELECT InventoryItemAlias.AliasId--, original.id, original.ItemNumber, Dups.id AS Expr1, Dups.ItemNumber AS Expr2
FROM    InventoryItemAlias INNER JOIN
                     (SELECT id, ItemNumber
                      FROM     [##inventorywithRow] AS [##inventorywithRow_1]
                      WHERE  (row > 1) ) AS Dups ON InventoryItemAlias.aliasItemId = Dups.id INNER JOIN
                     (SELECT id, ItemNumber
                      FROM     [##inventorywithRow]
                      WHERE  (row = 1) ) AS original ON trim(Dups.ItemNumber) = trim(original.ItemNumber) AND Dups.id <> original.id and InventoryItemAlias.AliasItemId <> original.id)

--select * from InventoryItemAlias where aliasitemid in (select id from ##inventorywithRow where row > 1 )

--select * from inventoryitems where id in (47067, 49834)

SELECT id
FROM    [##inventorywithRow] left outer join
                 InventoryItemAlias on [##inventorywithRow].Id = InventoryItemAlias.AliasItemId
WHERE (row > 1) and  InventoryItemAlias.AliasItemId is null 


SELECT original.id, original.ItemNumber, Dups.id AS Expr1, Dups.ItemNumber AS Expr2
FROM    InventoryItemAlias INNER JOIN
                     (SELECT id, ItemNumber
                      FROM     [##inventorywithRow] AS [##inventorywithRow_1]
                      WHERE  (row > 1) ) AS Dups ON InventoryItemAlias.aliasItemId = Dups.id INNER JOIN
                     (SELECT id, ItemNumber
                      FROM     [##inventorywithRow]
                      WHERE  (row = 1) ) AS original ON trim(Dups.ItemNumber) = trim(original.ItemNumber) AND Dups.id <> original.id and InventoryItemAlias.AliasItemId <> original.id

-- clear out aliases with out inventory items
DELETE FROM InventoryItemAlias
--select InventoryItemAlias.* 
FROM      InventoryItems RIGHT OUTER JOIN
                InventoryItemAlias ON InventoryItems.Id = InventoryItemAlias.AliasItemId
WHERE   (InventoryItems.Id IS NULL)

DELETE FROM InventoryItemAlias
--select InventoryItemAlias.* 
FROM      InventoryItems RIGHT OUTER JOIN
                InventoryItemAlias ON InventoryItems.Id = InventoryItemAlias.InventoryItemId
WHERE   (InventoryItems.Id IS NULL)

------------------------------------------------fix iww problem

--drop table ##descriptionwithRow

--select EntryDataDetailsId, entrydataid, itemnumber,ItemDescription, (ROW_NUMBER() over (partition by itemnumber order by EntryDataDetailsId asc))  row
--into ##descriptionwithRow
--from EntryDataDetails  

--update EntryDataDetails
--set ItemDescription = good.itemdescription
--from
----select entrydatadetails.ItemDescription, good.ItemDescription from
--EntryDataDetails inner join 
--(select * from ##descriptionwithRow where row > 1 and entrydataid like 'A2O%') err on EntryDataDetails.EntryDataDetailsId = err.EntryDataDetailsId inner join 
--(select * from ##descriptionwithRow where row = 1 ) good on EntryDataDetails.ItemNumber = good.ItemNumber and EntryDataDetails.ItemDescription <> good.ItemDescription


--update InventoryItems
--set Description = good.itemdescription
--from
----select inventoryitems.Description ,entrydatadetails.ItemDescription, good.ItemDescription from
--InventoryItems inner join 
--EntryDataDetails on InventoryItems.id = EntryDataDetails.InventoryItemId inner join 
--(select * from ##descriptionwithRow where row > 1 and entrydataid like 'A2O%') err on EntryDataDetails.EntryDataDetailsId = err.EntryDataDetailsId inner join 
--(select * from ##descriptionwithRow where row = 1 ) good on EntryDataDetails.ItemNumber = good.ItemNumber 
END
GO
