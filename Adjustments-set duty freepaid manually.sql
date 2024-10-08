/****** Script for SelectTopNRows command from SSMS  ******/
SELECT TOP (1000) [EntryDataDetailsId]
      ,[EntryData_Id]
      ,[EntryDataId]
      ,[LineNumber]
      ,[ItemNumber]
      ,[Quantity]
      ,[Units]
      ,[ItemDescription]
      ,[Cost]
      ,[TotalCost]
      ,[QtyAllocated]
      ,[UnitWeight]
      ,[DoNotAllocate]
      ,[Freight]
      ,[Weight]
      ,[InternalFreight]
      ,[Status]
      ,[InvoiceQty]
      ,[ReceivedQty]
      ,[PreviousInvoiceNumber]
      ,[CNumber]
      ,[Comment]
      ,[EffectiveDate]
      ,[IsReconciled]
      ,[TaxAmount]
      ,[LastCost]
      ,[InventoryItemId]
      ,[FileLineNumber]
  FROM [AutoBot-EnterpriseDB].[dbo].[EntryDataDetails]
  where entrydataid like 'ADJ%' and Comment is not null and TaxAmount = 1 

  select distinct comment, TaxAmount  from entrydatadetails  where entrydataid like 'ADJ%'

  update EntryDataDetails
  set taxamount = 1
  where comment in ('Promotional Expenses','Charity','Shop / Office / Warehouse Expenses','Office Expenses','PROMOTION','Promotional Expense','Promotional Expenses','Sample',
					'Shop / Office / Warehouse Expenses','SHOP EXPENSES','SP','SPONSORSHIP','STORE / WAREHOUSE EXPENSE','Store / Warehouse Expenses','STORE EXPENSE','VEHICLE EXPENSE'
					,'Vehicle Expenses','WAREHOUSE EXPENSE','WAREHOUSE EXPENSES','DONATION')


  update EntryDataDetails
  set taxamount = 0
  where comment in ('Annual Stock Take Adjustments','COST CHANGE','Cost of Warranty','Cycle Count Adjustment','Cycle Count Adjustment - item Stolen','Cycle Count Adjustments','Cycle Count Adjustments (Test)',
  'DAMAGED','Damaged Goods','Error in System - Negative on Hand Report','Expired / Obsolete Goods','Expired Goods','GROSS STOCK USED INTERNATLLY- INCORRECT CODING USED','Incorrect Internal Code Used',
  'LOST / STOLEN','Lost / Stolen Goods','LOST/ STOLEN','Lost/ Stolen Goods','PHYSICAL COUNT ADJUSTMENT','Warr Exp (Stock Write-Off) - Gen','WARRANTY','WARRANTY REPLACEMENT','WARRANTY REPLACEMENT - WRITTEN OFF','WARRANTY REPLACEMENT/ WRITTEN OFF','WARRANTY REPLACEMENT/DAMAGED IN WAREHOUSE','Warranty Replacment','Written Off - Damage Item','Written Off - Expired Goods')

update EntryDataDetails
  set taxamount = 1
  where entrydataid like 'ADJ%' and TaxAmount is null