delete from xcuda_Item
where ASYCUDA_Id in (select ASYCUDA_Id FROM [BudgetMarineDB-Enterprise].[dbo].[DataCheck-UnlinkedPreviousItem])

delete from xcuda_ASYCUDA
where ASYCUDA_Id in (select ASYCUDA_Id FROM [BudgetMarineDB-Enterprise].[dbo].[DataCheck-UnlinkedPreviousItem])