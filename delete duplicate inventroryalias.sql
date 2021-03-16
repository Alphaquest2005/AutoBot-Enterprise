delete from InventoryItemAlias
where AliasId in
(select aliasid from (select AliasId, ROW_NUMBER() over (partition by inventoryitemid, aliasitemid order by inventoryitemid ) as rownum from InventoryItemAlias) as t 
where t.rownum > 1)