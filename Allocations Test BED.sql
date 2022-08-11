////////////////////////////////////Batch vs Current Allocations comparison

declare @BatchNo int = 2, @itemNumber varchar(50), @appSettingId int = 7
set @itemNumber = '14002-220'


select * from  AsycudaSalesAndAdjustmentAllocationsEx where ItemNumber LIKE '%' + @ItemNumber + '%' and applicationsettingsid = @appSettingId 

select * from  [History-Allocations] where ItemNumber LIKE '%' + @ItemNumber + '%' and applicationsettingsid = @appSettingId 

select 'allocation Differences'

drop table #his
SELECT BatchNo, Status, QtyAllocated, InvoiceDate, CustomerName, SalesQuantity, SalesQtyAllocated, InvoiceNo, InvoiceLineNumber, ItemNumber, ItemDescription, DutyFreePaid, pCNumber, pRegistrationDate, 
                 pQuantity, pQtyAllocated, PiQuantity, SalesFactor, pReferenceNumber, pLineNumber, Cost, Total_CIF_itm, DutyLiability, TaxAmount, pIsAssessed, DoNotAllocateSales, 
                 DoNotAllocatePreviousEntry, WarehouseError, SANumber, TariffCode, Invalid, pExpiryDate, pTariffCode, pItemNumber, ApplicationSettingsId
into #his
FROM    [History-Allocations]
where (ItemNumber LIKE '%' + @ItemNumber + '%') and applicationsettingsid = @appSettingId and ([History-Allocations].BatchNo = @BatchNo) 

drop table #allo
SELECT        @BatchNo AS BatchNo, Status, QtyAllocated,  InvoiceDate, CustomerName, SalesQuantity, SalesQtyAllocated, InvoiceNo, SalesLineNumber, ItemNumber, ItemDescription, DutyFreePaid, pCNumber, pRegistrationDate, pQuantity, 
                         pQtyAllocated, PiQuantity, SalesFactor,  pReferenceNumber, pLineNumber, Cost, Total_CIF_itm, DutyLiability, TaxAmount, pIsAssessed, DoNotAllocateSales, 
                         DoNotAllocatePreviousEntry, WarehouseError, SANumber, TariffCode, Invalid, pExpiryDate, pTariffCode, pItemNumber, ApplicationSettingsId
into #allo
FROM            AsycudaSalesAndAdjustmentAllocationsEx
where (ItemNumber LIKE '%' + @ItemNumber + '%') and applicationsettingsid = @appSettingId

(   SELECT * FROM #allo
    EXCEPT
    SELECT * FROM #his)  
UNION ALL
(   SELECT * FROM #his
    EXCEPT
    SELECT * FROM #allo) 

select 'All Allocation Differences'

drop table #Allhis
SELECT BatchNo, Status, QtyAllocated, InvoiceDate, CustomerName, SalesQuantity, SalesQtyAllocated, InvoiceNo, InvoiceLineNumber, ItemNumber, ItemDescription, DutyFreePaid, pCNumber, pRegistrationDate, 
                 pQuantity, pQtyAllocated, PiQuantity, SalesFactor, pReferenceNumber, pLineNumber, Cost, Total_CIF_itm, DutyLiability, TaxAmount, pIsAssessed, DoNotAllocateSales, 
                 DoNotAllocatePreviousEntry, WarehouseError, SANumber, TariffCode, Invalid, pExpiryDate, pTariffCode, pItemNumber, ApplicationSettingsId
into #Allhis
FROM    [History-Allocations]
where  applicationsettingsid = @appSettingId and ([History-Allocations].BatchNo = 1) 

drop table #Allallo
SELECT        1 AS BatchNo, Status, QtyAllocated,  InvoiceDate, CustomerName, SalesQuantity, SalesQtyAllocated, InvoiceNo, SalesLineNumber, ItemNumber, ItemDescription, DutyFreePaid, pCNumber, pRegistrationDate, pQuantity, 
                         pQtyAllocated, PiQuantity, SalesFactor,  pReferenceNumber, pLineNumber, Cost, Total_CIF_itm, DutyLiability, TaxAmount, pIsAssessed, DoNotAllocateSales, 
                         DoNotAllocatePreviousEntry, WarehouseError, SANumber, TariffCode, Invalid, pExpiryDate, pTariffCode, pItemNumber, ApplicationSettingsId
into #Allallo
FROM            AsycudaSalesAndAdjustmentAllocationsEx
where applicationsettingsid = @appSettingId

(   SELECT * FROM #Allallo
    EXCEPT
    SELECT * FROM #Allhis)  
UNION ALL
(   SELECT * FROM #Allhis
    EXCEPT
    SELECT * FROM #Allallo) 





////////////////////////////////////issues comparison






declare @BatchNo int = 2, @itemNumber varchar(50), @appSettingId int = 7
--set @itemNumber = '14002-220'
drop table #issueHis
SELECT BatchNo, Status, QtyAllocated, InvoiceDate, CustomerName, SalesQuantity, SalesQtyAllocated, InvoiceNo, InvoiceLineNumber, isnull([History-Allocations].ItemNumber, issues.ItemNumber) as ItemNumber, ItemDescription, DutyFreePaid, pCNumber, pRegistrationDate, 
                 pQuantity, pQtyAllocated, PiQuantity, SalesFactor, pReferenceNumber, pLineNumber, Cost, Total_CIF_itm, DutyLiability, TaxAmount, pIsAssessed, DoNotAllocateSales, 
                 DoNotAllocatePreviousEntry, WarehouseError, SANumber, TariffCode, Invalid, pExpiryDate, pTariffCode, pItemNumber, issues.ApplicationSettingsId, xCNumber, xLineNumber,xRegistrationDate , xReferenceNumber, xStatus, xQuantity
into #issueHis
FROM    [History-Allocations] right outer join [History-Batch-Issues] issues on [History-Allocations].BatchNo = issues.BatchId and [History-Allocations].ItemNumber = issues.ItemNumber and [History-Allocations].ApplicationSettingsId = issues.ApplicationSettingsId
where  issues.applicationsettingsid = @appSettingId and ([History-Allocations].BatchNo = @BatchNo) 

drop table #issueAllo
SELECT        @BatchNo AS BatchNo, Status, QtyAllocated,  InvoiceDate, CustomerName, SalesQuantity, SalesQtyAllocated, InvoiceNo, SalesLineNumber, isnull(AsycudaSalesAndAdjustmentAllocationsEx.ItemNumber, issues.ItemNumber) as ItemNumber, ItemDescription, DutyFreePaid, pCNumber, pRegistrationDate, pQuantity, 
                         pQtyAllocated, PiQuantity, SalesFactor,  pReferenceNumber, pLineNumber, Cost, Total_CIF_itm, DutyLiability, TaxAmount, pIsAssessed, DoNotAllocateSales, 
                         DoNotAllocatePreviousEntry, WarehouseError, SANumber, TariffCode, Invalid, pExpiryDate, pTariffCode, pItemNumber, issues.ApplicationSettingsId, xCNumber, xLineNumber,xRegistrationDate,  xReferenceNumber, xStatus, xQuantity
into #issueAllo
FROM            AsycudaSalesAndAdjustmentAllocationsEx right outer join [History-Batch-Issues] issues on  AsycudaSalesAndAdjustmentAllocationsEx.ItemNumber = issues.ItemNumber and AsycudaSalesAndAdjustmentAllocationsEx.ApplicationSettingsId = issues.ApplicationSettingsId
where issues.applicationsettingsid = @appSettingId
select 'Existing Row Differences Differences'
select 'Issues Allocation'
select * from #issueAllo
select 'Issues History'
select * from #issueHis
select 'Issues Differences'
select * from (
(   SELECT ApplicationSettingsId,BatchNo, InvoiceNo, SalesLineNumber,  InvoiceDate, CustomerName, ItemNumber, ItemDescription, DutyFreePaid, Status, QtyAllocated, 'Allocations' as Source FROM #issueAllo
    EXCEPT
    SELECT ApplicationSettingsId, BatchNo, InvoiceNo, InvoiceLineNumber,  InvoiceDate, CustomerName, ItemNumber, ItemDescription, DutyFreePaid, Status, QtyAllocated, 'Allocations' as Source FROM #issueHis)  
UNION ALL
(   SELECT ApplicationSettingsId, BatchNo, InvoiceNo, InvoiceLineNumber,  InvoiceDate, CustomerName, ItemNumber, ItemDescription, DutyFreePaid, Status, QtyAllocated, 'History' as Source FROM #issueHis
    EXCEPT
    SELECT ApplicationSettingsId, BatchNo, InvoiceNo,  SalesLineNumber, InvoiceDate, CustomerName, ItemNumber, ItemDescription, DutyFreePaid, Status, QtyAllocated, 'History' as Source FROM #issueAllo) ) as diff

select 'p Differences'

SELECT [History-Allocations].InvoiceNo, [History-Allocations].InvoiceDate, [History-Allocations].ItemNumber, [History-Allocations].SANumber, [History-Allocations].pCNumber AS [pCNumber-History], 
                 [History-Allocations].pRegistrationDate, AsycudaSalesAllocationsEx.pCNumber AS [pCNumber-Current], AsycudaSalesAllocationsEx.pRegistrationDate AS Expr1, 
                 [History-Allocations].QtyAllocated AS [QuantityAllocated-History], AsycudaSalesAllocationsEx.QtyAllocated AS [QuantityAllocated-Current]
FROM     #issueHis as [History-Allocations] INNER JOIN
         #issueAllo as  AsycudaSalesAllocationsEx ON [History-Allocations].InvoiceDate = AsycudaSalesAllocationsEx.InvoiceDate 
																				AND [History-Allocations].InvoiceNo = AsycudaSalesAllocationsEx.InvoiceNo AND 
																				 [History-Allocations].InvoiceLineNumber = AsycudaSalesAllocationsEx.SalesLineNumber AND 
                 [History-Allocations].ItemNumber = AsycudaSalesAllocationsEx.ItemNumber AND ( [History-Allocations].pCNumber <> AsycudaSalesAllocationsEx.pCNumber
				 or [History-Allocations].pLineNumber <> AsycudaSalesAllocationsEx.pLineNumber
				 or [History-Allocations].QtyAllocated <> AsycudaSalesAllocationsEx.QtyAllocated 
								  or [History-Allocations].Status <> AsycudaSalesAllocationsEx.Status  )
WHERE ([History-Allocations].BatchNo = @BatchNo) AND ([History-Allocations].ItemNumber LIKE '%' + @ItemNumber + '%') and AsycudaSalesAllocationsEx.applicationsettingsid = @appSettingId and [History-Allocations].applicationsettingsid = @appSettingId 

select 'x Differences'

SELECT [History-Allocations].InvoiceNo, [History-Allocations].InvoiceDate, [History-Allocations].ItemNumber, [History-Allocations].SANumber, [History-Allocations].xCNumber AS [xCNumber-History], 
                 [History-Allocations].xRegistrationDate, AsycudaSalesAllocationsEx.xCNumber AS [pCNumber-Current], AsycudaSalesAllocationsEx.xRegistrationDate AS xCurrentRegistrationDate, 
                 [History-Allocations].QtyAllocated AS [QuantityAllocated-History], AsycudaSalesAllocationsEx.QtyAllocated AS [QuantityAllocated-Current]
FROM    #issueHis as [History-Allocations] INNER JOIN
                #issueAllo as  AsycudaSalesAllocationsEx ON [History-Allocations].InvoiceDate = AsycudaSalesAllocationsEx.InvoiceDate 
				AND [History-Allocations].InvoiceNo = AsycudaSalesAllocationsEx.InvoiceNo AND 
				[History-Allocations].InvoiceLineNumber = AsycudaSalesAllocationsEx.SalesLineNumber 
				AND 
                 [History-Allocations].ItemNumber = AsycudaSalesAllocationsEx.ItemNumber AND ( [History-Allocations].xCNumber <> AsycudaSalesAllocationsEx.xCNumber
				 or [History-Allocations].xLineNumber <> AsycudaSalesAllocationsEx.xLineNumber
				 or [History-Allocations].xQuantity <> AsycudaSalesAllocationsEx.xQuantity or [History-Allocations].xStatus <> AsycudaSalesAllocationsEx.xStatus  )
WHERE ([History-Allocations].BatchNo = @BatchNo) AND ([History-Allocations].ItemNumber LIKE '%' + @ItemNumber + '%') and AsycudaSalesAllocationsEx.applicationsettingsid = @appSettingId and [History-Allocations].applicationsettingsid = @appSettingId 



--select 'p Differences'

--SELECT [History-Allocations].InvoiceNo, [History-Allocations].InvoiceDate, [History-Allocations].ItemNumber, [History-Allocations].SANumber, [History-Allocations].pCNumber AS [pCNumber-History], 
--                 [History-Allocations].pRegistrationDate, AsycudaSalesAllocationsEx.pCNumber AS [pCNumber-Current], AsycudaSalesAllocationsEx.pRegistrationDate AS Expr1, 
--                 [History-Allocations].QtyAllocated AS [QuantityAllocated-History], AsycudaSalesAllocationsEx.QtyAllocated AS [QuantityAllocated-Current]
--FROM    [History-Allocations] INNER JOIN
--                AsycudaSalesAndAdjustmentAllocationsEx as AsycudaSalesAllocationsEx ON [History-Allocations].InvoiceDate = AsycudaSalesAllocationsEx.InvoiceDate 
--																				AND [History-Allocations].InvoiceNo = AsycudaSalesAllocationsEx.InvoiceNo AND 
--																				 [History-Allocations].InvoiceLineNumber = AsycudaSalesAllocationsEx.SalesLineNumber AND 
--                 [History-Allocations].ItemNumber = AsycudaSalesAllocationsEx.ItemNumber AND ( [History-Allocations].pCNumber <> AsycudaSalesAllocationsEx.pCNumber
--				 or [History-Allocations].pLineNumber <> AsycudaSalesAllocationsEx.pLineNumber
--				 or [History-Allocations].QtyAllocated <> AsycudaSalesAllocationsEx.QtyAllocated 
--								  or [History-Allocations].Status <> AsycudaSalesAllocationsEx.Status  )
--WHERE ([History-Allocations].BatchNo = @BatchNo) AND ([History-Allocations].ItemNumber LIKE '%' + @ItemNumber + '%') and AsycudaSalesAllocationsEx.applicationsettingsid = @appSettingId and [History-Allocations].applicationsettingsid = @appSettingId 

--select 'x Differences'

--SELECT [History-Allocations].InvoiceNo, [History-Allocations].InvoiceDate, [History-Allocations].ItemNumber, [History-Allocations].SANumber, [History-Allocations].xCNumber AS [xCNumber-History], 
--                 [History-Allocations].xRegistrationDate, AsycudaSalesAllocationsEx.xCNumber AS [pCNumber-Current], AsycudaSalesAllocationsEx.xRegistrationDate AS xCurrentRegistrationDate, 
--                 [History-Allocations].QtyAllocated AS [QuantityAllocated-History], AsycudaSalesAllocationsEx.QtyAllocated AS [QuantityAllocated-Current]
--FROM    [History-Allocations] INNER JOIN
--                AsycudaSalesAndAdjustmentAllocationsEx as AsycudaSalesAllocationsEx ON [History-Allocations].InvoiceDate = AsycudaSalesAllocationsEx.InvoiceDate 
--				AND [History-Allocations].InvoiceNo = AsycudaSalesAllocationsEx.InvoiceNo AND 
--				[History-Allocations].InvoiceLineNumber = AsycudaSalesAllocationsEx.SalesLineNumber 
--				AND 
--                 [History-Allocations].ItemNumber = AsycudaSalesAllocationsEx.ItemNumber AND ( [History-Allocations].xCNumber <> AsycudaSalesAllocationsEx.xCNumber
--				 or [History-Allocations].xLineNumber <> AsycudaSalesAllocationsEx.xLineNumber
--				 or [History-Allocations].xQuantity <> AsycudaSalesAllocationsEx.xQuantity or [History-Allocations].xStatus <> AsycudaSalesAllocationsEx.xStatus  )
--WHERE ([History-Allocations].BatchNo = @BatchNo) AND ([History-Allocations].ItemNumber LIKE '%' + @ItemNumber + '%') and AsycudaSalesAllocationsEx.applicationsettingsid = @appSettingId and [History-Allocations].applicationsettingsid = @appSettingId 



--SELECT [History-Allocations].InvoiceNo, [History-Allocations].InvoiceDate, [History-Allocations].ItemNumber, [History-Allocations].SANumber, [History-Allocations].pCNumber AS [pCNumber-History], 
--                 [History-Allocations].pRegistrationDate, AsycudaSalesAllocationsEx.pCNumber AS [pCNumber-Current], AsycudaSalesAllocationsEx.pRegistrationDate AS Expr1, 
--                 [History-Allocations].QtyAllocated AS [QuantityAllocated-History], AsycudaSalesAllocationsEx.QtyAllocated AS [QuantityAllocated-Current]
--FROM    [History-Allocations] INNER JOIN
--                AsycudaSalesAndAdjustmentAllocationsEx as AsycudaSalesAllocationsEx ON [History-Allocations].InvoiceDate = AsycudaSalesAllocationsEx.InvoiceDate 
--																				AND [History-Allocations].InvoiceNo = AsycudaSalesAllocationsEx.InvoiceNo AND 
--																				 [History-Allocations].InvoiceLineNumber = AsycudaSalesAllocationsEx.SalesLineNumber AND 
--                 [History-Allocations].ItemNumber = AsycudaSalesAllocationsEx.ItemNumber AND ( [History-Allocations].pCNumber <> AsycudaSalesAllocationsEx.pCNumber
--				 or [History-Allocations].pLineNumber <> AsycudaSalesAllocationsEx.pLineNumber
--				 or [History-Allocations].QtyAllocated <> AsycudaSalesAllocationsEx.QtyAllocated 
--								  or [History-Allocations].Status <> AsycudaSalesAllocationsEx.Status  )
--WHERE ([History-Allocations].BatchNo = @BatchNo) AND ([History-Allocations].ItemNumber LIKE '%' + @ItemNumber + '%') and AsycudaSalesAllocationsEx.applicationsettingsid = @appSettingId and [History-Allocations].applicationsettingsid = @appSettingId 



