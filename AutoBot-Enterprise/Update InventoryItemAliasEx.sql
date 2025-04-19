
declare @alias nvarchar(50), @itemNumber nvarchar(50), @itemId int

set @itemNumber = '1277GL'	
set @alias = '1277/GL'	

set @itemId = 
(SELECT Id
FROM    InventoryItems
WHERE (ItemNumber = @itemNumber and ApplicationSettingsId = 1))

insert into InventoryItemAlias(InventoryItemId, AliasName) values (@itemId, @alias)

select * from InventoryItemAliasEx where itemnumber =  @itemNumber and ApplicationSettingsId = 1

INSERT INTO InventoryItemAlias
                 (InventoryItemId, AliasName)
SELECT InventoryItems.Id, alias.AliasName
FROM    InventoryItems INNER JOIN
                     (SELECT ItemNumber, AliasName
                      FROM     [Budget-ENTERPRISEDB].dbo.InventoryItemAlias AS InventoryItemAlias_1) AS alias ON InventoryItems.ItemNumber = alias.ItemNumber
WHERE (InventoryItems.ApplicationSettingsId = 2)


--1186320	1863
--15987381	30667
--55737994	125646
--1108606	1086
--1126106	1261
--1317508	3175
--1614908	6149
--SBN81M	SBP81M
--15987381	19274034
--1107906	1079
--1277GL	1277/GL