declare @startdate datetime, @endDate datetime,@asycudaEndDate datetime, @OPSNumber varchar(50),@lastCnumber varchar(50),@ItemNumber varchar(50),
		 @DoNotAllocateStockIssues bit,@DoNotAllocateStockIssuesWithStockOvers bit, @DoNotAllocateStockIssuesWithSalesErrors bit, @ApplicationSettingsId int,
		 @AutoMapp bit
--set @startdate = '9/1/2018'
--set @endDate = '12/31/2018'
--set @OPSNumber = 'OPS-12-31-2018'

--set @startdate = '1/1/2019'
--set @endDate = '2/28/2019'
--set @OPSNumber = 'OPS-02-28-2019'
set @ApplicationSettingsId = 5
set @startdate = '11/30/2021'
set @endDate = '11/30/2021'
set @OPSNumber = 'OPS-30-Nov-2021'
set @lastCnumber = '50580'
set @asycudaEndDate = isnull((select AssessmentDate from AsycudaDocumentBasicInfo where CNumber = @lastCnumber),@endDate) 
select @asycudaEndDate

							
set @DoNotAllocateStockIssues = 0
set @DoNotAllocateStockIssuesWithStockOvers = 0
set @DoNotAllocateStockIssuesWithSalesErrors = 0
set @AutoMapp = 0




drop table #adjmentsData

SELECT AdjustmentDetails.EntryDataId AS InvoiceNo, AdjustmentDetails.EffectiveDate, CASE WHEN AliasName IS NOT NULL THEN inventoryitemalias.itemnumber ELSE AdjustmentDetails.ItemNumber END AS ItemNumber, CASE WHEN AliasName IS NOT NULL THEN inventoryitemalias.InventoryItemId ELSE AdjustmentDetails.InventoryItemId END AS InventoryItemId, 
                 AdjustmentDetails.ItemDescription, sum( distinct ISNULL(AdjustmentDetails.ReceivedQty, 0) - ISNULL(AdjustmentDetails.InvoiceQty, 0)) AS Quantity, AVG(AdjustmentDetails.Cost) AS Cost, AdjustmentDetails.IsReconciled
into #adjmentsData
FROM    AsycudaDocumentItemEntryDataDetails  with (nolock)RIGHT OUTER JOIN
                 AdjustmentDetails with (nolock) LEFT OUTER JOIN
                inventoryitemaliasEX as InventoryItemAlias with (nolock) ON AdjustmentDetails.ItemNumber = InventoryItemAlias.AliasName ON AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId = AdjustmentDetails.EntryDataDetailsId
WHERE (AdjustmentDetails.ApplicationSettingsId = @ApplicationSettingsId) and (InventoryItemAlias.ApplicationSettingsId = @ApplicationSettingsId) and (AdjustmentDetails.EffectiveDate BETWEEN @startdate AND @endDate) and (ISNULL(AdjustmentDetails.ReceivedQty, 0) < ISNULL(AdjustmentDetails.InvoiceQty, 0))
     -- 7IT/PR-LBL-2X1 --- DID only show shorts because they have to act like sales, not include overs
      --AND (AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId IS NULL or ( AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId IS NULL and isnull(AdjustmentDetails.IsReconciled, 0) = 1))
GROUP BY CASE WHEN AliasName IS NOT NULL THEN inventoryitemalias.itemnumber ELSE AdjustmentDetails.ItemNumber END, AdjustmentDetails.ItemDescription, AdjustmentDetails.EntryDataId, 
                 AdjustmentDetails.EffectiveDate,  AdjustmentDetails.IsReconciled, AdjustmentDetails.EntryDataDetailsId, CASE WHEN AliasName IS NOT NULL THEN inventoryitemalias.InventoryItemId ELSE AdjustmentDetails.InventoryItemId END



--select 'Adjustments Data'
--select * from  #adjmentsData where itemnumber = @ItemNumber


drop table #salesData

SELECT        EntryData.EntryDataId as InvoiceNo, EntryData.EntryDataDate as Date, CASE WHEN AliasName IS NOT NULL THEN inventoryitemalias.itemnumber ELSE entrydatadetails.ItemNumber END AS ItemNumber, EntryDataDetails.InventoryItemId, EntryDataDetails.ItemDescription, 
                         SUM(EntryDataDetails.Quantity) AS Quantity, AVG(EntryDataDetails.Cost) AS Cost
into #salesData
FROM            EntryData_Sales with (nolock) INNER JOIN
                         EntryData with (nolock) ON EntryData_Sales.EntryData_Id = EntryData.EntryData_Id INNER JOIN
                         EntryDataDetails with (nolock) ON EntryData.EntryDataId = EntryDataDetails.EntryDataId LEFT OUTER JOIN
                         inventoryitemaliasEX as InventoryItemAlias with (nolock) ON EntryDataDetails.ItemNumber = InventoryItemAlias.AliasName
WHERE   (EntryData.ApplicationSettingsId = @ApplicationSettingsId) and      (EntryData.EntryDataDate BETWEEN @startdate AND @endDate) 
GROUP BY CASE WHEN AliasName IS NOT NULL THEN inventoryitemalias.itemnumber ELSE entrydatadetails.ItemNumber END, EntryDataDetails.ItemDescription, EntryData.EntryDataId, EntryData.EntryDataDate,EntryDataDetails.InventoryItemId

--select 'Sales Data'
--select * from  #salesData where itemnumber = @ItemNumber

drop table #returns
SELECT        ItemNumber, InventoryItemId, max(ItemDescription) as ItemDescription , SUM(Quantity*-1)  AS Quantity, AVG(Cost) AS Cost
into #returns
FROM #salesData 
WHERE        (Date BETWEEN @startdate AND @endDate)  and (Quantity < 0) and ItemNumber in (select ItemNumber from #salesData group by itemnumber having sum(Quantity) <> 0)
GROUP by ItemNumber, InventoryItemId


--select 'Returns'
--select * from  #returns where itemnumber = @ItemNumber


drop table #sales
SELECT        ItemNumber, InventoryItemId, max(ItemDescription) as ItemDescription, SUM(Quantity) AS Quantity, 
                         AVG(Cost) AS Cost
into #sales
FROM   #salesData
WHERE        (Date BETWEEN @startdate AND @endDate)  and quantity > 0  and ItemNumber in (select ItemNumber from #salesData group by itemnumber having sum(Quantity) <> 0)
GROUP BY ItemNumber, InventoryItemId

--select 'Sales'
--select * from  #sales where itemnumber = @ItemNumber


drop table #adjustments
SELECT        ItemNumber, InventoryItemId, max(ItemDescription) as ItemDescription, abs(SUM(Quantity)) AS Quantity, 
                         AVG(Cost) AS Cost, IsReconciled
into #adjustments
FROM   #adjmentsData
WHERE        (EffectiveDate BETWEEN @startdate AND @endDate) 
GROUP BY ItemNumber, IsReconciled, InventoryItemId

--select 'adjustments'
--select * from  #adjustments where itemnumber = @ItemNumber


drop table #AsycudaData
----
SELECT       'C#' + CAST(AsycudaWarehouseData.REG_NBR AS nvarchar(50)) + '-' + CAST(AsycudaWarehouseData.ITEM_NBR AS nvarchar(50)) AS PrevDoc, AsycudaWarehouseData.PRODUCT_ID as Precision_4, AsycudaWarehouseData.TAR_DSC AS Description, 
                         SUM(ISNULL(AsycudaWarehouseData.INIT_QTY - AsycudaWarehouseData.REM_QTY, 0)) AS PiQuantity,sum(isnull(AsycudaWarehouseData.INIT_QTY,0) - isnull(AsycudaWarehouseData.REM_QTY,0)) as QtyAllocated, SUM(AsycudaWarehouseData.INIT_QTY) AS Quantity, 
                         SUM(CAST(ISNULL(AsycudaWarehouseData.REM_QTY, 0) AS float)) AS Amount, AVG(CAST(0 AS float)) AS Cost, 
                         AsycudaWarehouseData.PRODUCT_ID AS ItemNumber, isnull(InventoryItemAliasEx.InventoryItemId,InventoryItems.Id) AS InventoryItemId,  AsycudaDocument.AssessmentDate,  AsycudaWarehouseData.IDE_REG_DAT as RegistrationDate , AsycudaWarehouseData.REF_NBER as Reference
into #AsycudaData
FROM           AsycudaWarehouseData left outer join AsycudaDocument on AsycudaWarehouseData.REG_NBR = AsycudaDocument.CNumber and AsycudaWarehouseData.OFFICE = AsycudaDocument.Customs_clearance_office_code and AsycudaWarehouseData.YEAR = Year(AsycudaDocument.RegistrationDate)
				left outer join InventoryItems on AsycudaWarehouseData.PRODUCT_ID = InventoryItems.ItemNumber
				left outer join InventoryItemAliasEx on AsycudaWarehouseData.PRODUCT_ID = InventoryItemAliasEx.AliasName
WHERE   (AsycudaWarehouseData.ApplicationSettingsId = @ApplicationSettingsId) and      (AsycudaWarehouseData.IDE_REG_DAT <= @asycudaEndDate) /*AND (isnull(AsycudaDocumentBasicInfo.Cancelled, 0) <> 1)*/ 
			and (CAST(AsycudaWarehouseData.IDE_TYP_SAD AS nvarchar(50)) + CAST(AsycudaWarehouseData.IDE_TYP_PRC AS nvarchar(50)) in ('IM7','OS7'))
			
GROUP BY AsycudaWarehouseData.REG_NBR, AsycudaWarehouseData.ITEM_NBR, PRODUCT_ID, TAR_DSC, INIT_QTY, 
                         REM_QTY, AssessmentDate,  IDE_REG_DAT,
						 REF_NBER, InventoryItemAliasEx.InventoryItemId,InventoryItems.Id

select 'AsycudaData'						 
select * from #AsycudaData 

drop table #AsycudaSummary

select ItemNumber,InventoryItemId , Max(Description) as Description, sum(quantity) as Quantity, sum(amount) as Amount, sum(PiQuantity) as PiQuantity, avg(cost) as Cost
into #AsycudaSummary
from #AsycudaData
group by ItemNumber,InventoryItemId


select 'AsycudaSummary'
select * from #AsycudaSummary --where itemnumber = @ItemNumber

drop table #OPSData


SELECT        Coalesce(OPS.OPSNumber, (case when sales.ItemNumber is null then Null else 'Sales' end),(case when returns.ItemNumber is null then Null else 'Returns' end),(case when adjustments.ItemNumber is null then Null else 'Adjustments' end)) AS InvoiceNo, Coalesce(OPS.ItemNumber, sales.ItemNumber, returns.ItemNumber,adjustments.ItemNumber) AS ItemNumber,  Coalesce(OPS.InventoryItemId, sales.InventoryItemId, returns.InventoryItemId,adjustments.InventoryItemId) AS InventoryItemId, Coalesce(OPS.ItemDescription, sales.ItemDescription, returns.ItemDescription, adjustments.ItemDescription) AS ItemDescription, CAST(Coalesce(OPS.Cost, sales.Cost, returns.cost,adjustments.Cost) AS float) AS Cost, sales.Quantity AS Sales, 
                         OPS.Quantity AS QuantityOnHand, returns.Quantity as Returns, adjustments.Quantity as Adjustments
into #OPSData
FROM            (SELECT        [#Sales].ItemNumber, InventoryItemId, [#Sales].Quantity, [#Sales].ItemDescription, [#Sales].Cost
                          FROM            [#Sales]) AS sales FULL OUTER JOIN
                             (SELECT        EntryData_OpeningStock_1.OPSNumber, EntryDataDetails_1.Quantity, EntryDataDetails_1.ItemNumber,EntryDataDetails_1.InventoryItemId,  EntryDataDetails_1.ItemDescription, CAST(EntryDataDetails_1.Cost AS float) AS Cost
                               FROM            EntryData_OpeningStock AS EntryData_OpeningStock_1  with (nolock) INNER JOIN
                                                         EntryDataDetails AS EntryDataDetails_1  with (nolock) ON EntryData_OpeningStock_1.OPSNumber = EntryDataDetails_1.EntryDataId
											   inner join EntryData on EntryData_OpeningStock_1.EntryData_Id = EntryData.EntryData_Id
                               WHERE   (EntryData.ApplicationSettingsId = @ApplicationSettingsId) and       (EntryData_OpeningStock_1.OPSNumber = @OPSNumber) 
                               GROUP BY EntryData_OpeningStock_1.OPSNumber, EntryDataDetails_1.ItemNumber,EntryDataDetails_1.InventoryItemId , EntryDataDetails_1.Quantity, EntryDataDetails_1.ItemDescription, CAST(EntryDataDetails_1.Cost AS float)) AS OPS ON 
                         sales.ItemNumber = OPS.ItemNumber
						 full outer join (SELECT        [#Returns].ItemNumber, InventoryItemId, [#Returns].Quantity, [#Returns].ItemDescription, [#Returns].Cost
                          FROM            [#Returns]) AS returns on returns.ItemNumber = OPS.ItemNumber
						 full outer join (SELECT        [#adjustments].ItemNumber, InventoryItemId, [#adjustments].Quantity, [#adjustments].ItemDescription, [#adjustments].Cost
                          FROM            [#adjustments]) AS adjustments on adjustments.ItemNumber = OPS.ItemNumber
GROUP BY sales.Cost, sales.ItemNumber, OPS.ItemNumber, sales.Quantity, OPS.Quantity, OPS.OPSNumber, ops.ItemDescription, sales.ItemDescription, ops.Cost, sales.cost, returns.ItemNumber, returns.ItemDescription, returns.Cost, returns.Quantity,
	 adjustments.ItemNumber, adjustments.ItemDescription, adjustments.Cost, adjustments.Quantity,  Coalesce(OPS.InventoryItemId, sales.InventoryItemId, returns.InventoryItemId,adjustments.InventoryItemId)

select '#OPSData'
select * from #OPSData 

drop table #OPS

select	max(InvoiceNo) as InvoiceNo,
		ItemNumber,
		InventoryItemId,
		max(ItemDescription) as ItemDescription,
		+ sum(isnull(sales,0)) 
			- sum(isnull(returns,0)) 
			--+ sum(isnull(Adjustments,0)) 
			+ sum(isnull(QuantityOnHand,0)) 
			+  (case when Sum(Quantityonhand) < 0  then Sum(Quantityonhand) *-1 else 0 end)
			 as Quantity,
		avg(cost) as Cost,
		Sum(Sales) as Sales,
		Sum(QuantityOnhand) as QuantityOnHand,
		sum(Returns) as Returns,
	--	sum(Adjustments) as Adjustments,
		(case when Sum(QuantityOnHand) < 0  then Sum(QuantityOnHand) *-1 else 0 end) as [OPS2zero-Adjustment]
into #OPS
from #OPSData
group by ItemNumber,InventoryItemId


select 'OPS'
select * from #OPS --where itemnumber = @ItemNumber



		drop table [#MappingIssues]

		------------- replace '/' & '-'
		SELECT        [#OPS].Itemnumber, [#AsycudaData].ItemNumber AS Alias, [#OPS].ItemDescription, [#AsycudaData].description AS AlisasDescripton
		into  [#MappingIssues]
		FROM            [#AsycudaData] AS [#AsycudaData] INNER JOIN
								  [#OPS] AS [#OPS] ON REPLACE(REPLACE([#AsycudaData].ItemNumber, '/', ''),'-','') = REPLACE(REPLACE([#OPS].ItemNumber, '/', ''),'-','') and [#AsycudaData].ItemNumber <> [#OPS].ItemNumber


		INSERT INTO [#MappingIssues]
								 (ItemNumber,Alias,ItemDescription,AlisasDescripton)
		SELECT        [#OPS].Itemnumber, [#AsycudaData].ItemNumber AS Alias, [#OPS].ItemDescription, [#AsycudaData].description AS AlisasDescripton
		FROM            [#AsycudaData] AS [#AsycudaData] INNER JOIN
								 [#OPS] AS [#OPS] ON REPLACE([#AsycudaData].ItemNumber, ' ', '') = REPLACE([#OPS].ItemNumber, ' ', '') and [#AsycudaData].ItemNumber <> [#OPS].ItemNumber

		--where charIndex('-',[#AsycudaData].ItemNumber) > 0-- charIndex('/',[#OPS].ItemNumber) > 0 and 

		--select * from [#AsycudaData]
		--where charIndex('-',[#AsycudaData].ItemNumber) > 0

		--select * from [#OPS]
		--where charIndex('-',[#OPS].ItemNumber) > 0

		--select 'Mapping Issues'
		-- select * from [#MappingIssues] where itemnumber = @ItemNumber

if @AutoMapp = 1 
Begin

		INSERT INTO InventoryItemAlias
								 (InventoryItemId,AliasName)
		SELECT   distinct     InventoryItems.Id, [#MappingIssues].Alias
		FROM           InventoryItems 
					inner join InventoryItemAlias AS InventoryItemAlias_1 on InventoryItems.Id = InventoryItemAlias_1.InventoryItemId Right OUTER JOIN [#MappingIssues]
								  ON [#MappingIssues].itemnumber = InventoryItems.ItemNumber  AND [#MappingIssues].Alias = InventoryItemAlias_1.AliasName
								  
		WHERE    (InventoryItems.ApplicationSettingsId = @ApplicationSettingsId) and      (InventoryItems.ItemNumber IS NULL)

end 

drop table [#AsycudaData-ItemAlias]
SELECT   distinct    'C#' + AsycudaDocumentRegistrationDate.CNumber + '-' + cast(xcuda_Item.LineNumber as varchar(50)) AS PrevDoc, inventoryitemalias.AliasName AS Precision_4, InventoryItemId, dbo.xcuda_Goods_description.Commercial_Description AS Description, 
                         SUM(ISNULL(AscyudaItemPiQuantity.PiQuantity, 0)) AS PiQuantity, SUM(dbo.Primary_Supplementary_Unit.Suppplementary_unit_quantity) AS Quantity, 
                         SUM(CAST(dbo.Primary_Supplementary_Unit.Suppplementary_unit_quantity AS float) - CAST(ISNULL(AscyudaItemPiQuantity.PiQuantity, 0) AS float)) AS Amount, AVG(CAST(dbo.AsycudaDocumentItemCost.LocalItemCost AS float)) 
                         AS Cost, InventoryItemAlias.ItemNumber
into [#AsycudaData-ItemAlias]
FROM            dbo.xcuda_Item with (nolock) INNER JOIN
                         dbo.AsycudaDocumentRegistrationDate with (nolock) ON dbo.AsycudaDocumentRegistrationDate.ASYCUDA_Id = dbo.xcuda_Item.ASYCUDA_Id INNER JOIN
                         dbo.xcuda_HScode with (nolock) ON dbo.xcuda_Item.Item_Id = dbo.xcuda_HScode.Item_Id INNER JOIN
                         dbo.ApplicationSettings with (nolock) ON dbo.ApplicationSettings.OpeningStockDate <= dbo.AsycudaDocumentRegistrationDate.AssessmentDate AND 
                         dbo.ApplicationSettings.OpeningStockDate <= dbo.AsycudaDocumentRegistrationDate.AssessmentDate INNER JOIN
                         dbo.InventoryItemAliasEX as inventoryitemalias with (nolock) ON dbo.xcuda_HScode.Precision_4 = InventoryItemAlias.AliasName LEFT OUTER JOIN
                         dbo.AsycudaDocumentItemCost with (nolock) ON dbo.xcuda_Item.Item_Id = dbo.AsycudaDocumentItemCost.Item_Id LEFT OUTER JOIN
                         (SELECT Item_Id, SUM(PiQuantity) AS PiQuantity, SUM(PiWeight) AS PiWeight
								FROM    [AsycudaItemPiQuantityData-Basic] with (nolock)
								--where AssessmentDate <= @asycudaEndDate
								GROUP BY Item_Id ) as AscyudaItemPiQuantity ON dbo.xcuda_Item.Item_Id = AscyudaItemPiQuantity.Item_Id LEFT OUTER JOIN
                         dbo.xcuda_Goods_description with (nolock) ON dbo.xcuda_Item.Item_Id = dbo.xcuda_Goods_description.Item_Id LEFT OUTER JOIN
                         dbo.Primary_Supplementary_Unit with (nolock) ON dbo.xcuda_Item.Item_Id = dbo.Primary_Supplementary_Unit.Tarification_Id left outer join 
						 AsycudaDocumentBasicInfo with (nolock) on xcuda_Item.ASYCUDA_Id = AsycudaDocumentBasicInfo.ASYCUDA_Id
						 
WHERE   (AsycudaDocumentBasicInfo.ApplicationSettingsId = @ApplicationSettingsId) and 
       (AsycudaDocumentRegistrationDate.AssessmentDate <= @asycudaEndDate and xcuda_Item.WarehouseError is null) AND (isnull(AsycudaDocumentRegistrationDate.Cancelled,0) <> 1) AND (isnull(AsycudaDocumentRegistrationDate.DoNotAllocate,0) <> 1) 
	   and (dbo.AsycudaDocumentRegistrationDate.DocumentType = 'IM7') OR (dbo.AsycudaDocumentRegistrationDate.DocumentType = 'OS7')
						--TODO: prevent unregistered entries IM7s
			--and (AsycudaDocumentBasicInfo.CNumber <> null or AsycudaDocumentBasicInfo.IsManuallyAssessed = 1)
GROUP BY CASE WHEN aliasname IS NOT NULL THEN inventoryitemalias.itemnumber ELSE precision_4 END, dbo.AsycudaDocumentRegistrationDate.DocumentType, InventoryItemAlias.ItemNumber, 
                         dbo.xcuda_Goods_description.Commercial_Description, AsycudaDocumentRegistrationDate.CNumber,xcuda_Item.LineNumber,inventoryitemalias.AliasName, InventoryItemId
      

select '#AsycudaData-ItemAlias'
select * from [#AsycudaData-ItemAlias] --where itemnumber = @ItemNumber


drop table [#AsycudaSummary-ItemAlias]

select ItemNumber, InventoryItemId, Max(Description) as Description, sum(quantity) as Quantity, sum(amount) as Amount, sum(PiQuantity) as PiQuantity, avg(cost) as Cost
into [#AsycudaSummary-ItemAlias]
from  [#AsycudaData-ItemAlias]
group by ItemNumber, InventoryItemId

select '#AsycudaSummary-ItemAlias'
select * from [#AsycudaSummary-ItemAlias] --where itemnumber = @ItemNumber

insert into [#AsycudaSummary]
  (ItemNumber, InventoryItemId, Description, Quantity, Amount, PiQuantity, Cost)
select * from [#AsycudaSummary-ItemAlias]

select '[#AsycudaSummary] with itemalias'
select * from [#AsycudaSummary]


drop table  [#StockDifferences-ItemAlias]
SELECT        isnull(#OPS.InvoiceNo, 'Asycuda') as InvoiceNo, ISNULL(#OPS.ItemNumber, [#AsycudaData-ItemAlias].ItemNumber) AS ItemNumber, ISNULL(#OPS.InventoryItemId, [#AsycudaData-ItemAlias].InventoryItemId) AS InventoryItemId, ISNULL(#OPS.ItemDescription, 
                         [#AsycudaData-ItemAlias].Description) AS Description, ISNULL(#OPS.Quantity, [#AsycudaData-ItemAlias].Amount) AS Quantity, 
                         #OPS.[OPS2zero-Adjustment],#OPS.Returns AS Returns, #OPS.Sales AS Sales, #OPS.Quantity AS [OPS-Quantity], ISNULL(#OPS.Cost, 0) AS OPSCost, [#AsycudaData-ItemAlias].Amount AS [Asycuda-Quantity], [#AsycudaData-ItemAlias].PiQuantity AS [Asycuda-PiQuantity],
                         ISNULL([#AsycudaData-ItemAlias].Cost, 0) AS AsycudaCost, COALESCE (#OPS.Quantity, 0) - COALESCE ([#AsycudaData-ItemAlias].Amount, 0) AS Diff, 
                         ISNULL(#OPS.Cost, [#AsycudaData-ItemAlias].Cost) AS Cost, (COALESCE (#OPS.Quantity, 0) - COALESCE ([#AsycudaData-ItemAlias].Amount, 0)) 
                         * ISNULL(#OPS.Cost, [#AsycudaData-ItemAlias].Cost) AS TotalCost
into [#StockDifferences-ItemAlias]
FROM           [#AsycudaSummary-ItemAlias] as [#AsycudaData-ItemAlias] INNER JOIN
                         #OPS ON [#AsycudaData-ItemAlias].ItemNumber = #OPS.ItemNumber

--select '#StockDifferences-ItemAlias'
--select * from [#StockDifferences-ItemAlias] where itemnumber = @ItemNumber

drop table [#StockDifferences]
SELECT        isnull(#OPS.InvoiceNo, 'Asycuda') as InvoiceNo, ISNULL(#OPS.ItemNumber, #AsycudaData.ItemNumber) AS ItemNumber,  ISNULL(#OPS.InventoryItemId, #AsycudaData.InventoryItemId) AS InventoryItemId, ISNULL(#OPS.ItemDescription, 
                         #AsycudaData.Description) AS Description, ISNULL(#OPS.Quantity, #AsycudaData.Quantity) AS Quantity,
						 #OPS.[OPS2zero-Adjustment],#OPS.Returns AS Returns, #OPS.Sales AS Sales,#OPS.QuantityOnHand as [OPS-QuantityOnHand], #OPS.Quantity AS [OPS-Quantity], 
                         ISNULL(#OPS.Cost, 0) AS OPSCost, #AsycudaData.Quantity AS [Asycuda-Quantity], #AsycudaData.PiQuantity AS [Asycuda-PiQuantity], ISNULL(#AsycudaData.Cost, 0) AS AsycudaCost, 
                         COALESCE (#OPS.Quantity, 0) - COALESCE (#AsycudaData.Quantity, 0) AS Diff, ISNULL(#OPS.Cost, #AsycudaData.Cost) AS Cost, 
                         (COALESCE (#OPS.Quantity, 0) - COALESCE (#AsycudaData.Quantity, 0)) * ISNULL(#OPS.Cost, #AsycudaData.Cost) AS TotalCost
into  [#StockDifferences]
FROM            #AsycudaSummary as #AsycudaData FULL OUTER JOIN
                 #OPS  as #OPS ON #AsycudaData.InventoryItemId = #OPS.InventoryItemId

select '#StockDifferences'
select * from [#StockDifferences] --where itemnumber = @ItemNumber


select 'OPS to Zero - Overs'
drop table [#OPS to Zero - Overs]
SELECT  distinct     [#OPS].InvoiceNo, ItemNumber, [#OPS].InventoryItemId,[#OPS].ItemDescription, (case when QuantityOnHand is null then abs([#OPS].Quantity) else [OPS2zero-Adjustment] end)  as Quantity, [#OPS].Cost
into [#OPS to Zero - Overs]
FROM     [#OPS]       left outer join [InventoryItems-NonStock] on [#OPS].InventoryItemId = [InventoryItems-NonStock].InventoryItemId   
		-- left outer join [#AsycudaSummary] on  [#OPS].ItemNumber = [#AsycudaSummary].ItemNumber              
WHERE      ( [OPS2zero-Adjustment] > 0  ) and [InventoryItems-NonStock].InventoryItemId is null --and [#AsycudaSummary].ItemNumber is null

--- already included in overs - could delete
--select * from [#OPS to Zero - Overs]  where ItemNumber = @ItemNumber


select 'P2O-Returns'
drop table [#P2O-Returns]
SELECT  distinct     [#OPS].InvoiceNo, [#OPS].ItemNumber, [#OPS].InventoryItemId,[#OPS].ItemDescription, [#OPS].Sales,[#OPS].Returns,[#OPS].QuantityOnHand, isnull([#OPS].Returns,0) - isnull([#OPS].Sales,0) as Quantity, [#OPS].Cost
into [#P2O-Returns]
FROM     [#OPS]   left outer join [InventoryItems-NonStock] on [#OPS].InventoryItemId = [InventoryItems-NonStock].InventoryItemId     
				                     
WHERE       ((isnull([#OPS].Sales,0) < isnull([#OPS].Returns,0)) or ( ((isnull([#OPS].Sales,0) - isnull([#OPS].Returns,0))) = [#OPS].QuantityOnHand) ) and isnull([#OPS].Returns,0) - isnull([#OPS].Sales,0) > 0 and [InventoryItems-NonStock].InventoryItemId is null 

--select * from [#P2O-Returns] where ItemNumber = @ItemNumber


drop table #results

SELECT       coalesce([#StockDifferences].InvoiceNo, [#OPS to Zero - Overs].InvoiceNo, [#P2O-Returns].InvoiceNo) as InvoiceNo,
			 coalesce([#StockDifferences].ItemNumber,[#OPS to Zero - Overs].ItemNumber, [#P2O-Returns].ItemNumber) as ItemNumber,
			 coalesce([#StockDifferences].InventoryItemId,[#OPS to Zero - Overs].InventoryItemId, [#P2O-Returns].InventoryItemId) as InventoryItemId,
			 max(coalesce([#StockDifferences].Description,[#OPS to Zero - Overs].ItemDescription, [#P2O-Returns].ItemDescription)) as Description,
			 avg(isnull([#StockDifferences].Quantity,0)) +  avg(isnull(Adj.Quantity,0)) + avg(isnull([#OPS to Zero - Overs].Quantity,0)) + avg(isnull([#P2O-Returns].Quantity,0)) AS Quantity,
			 avg(isnull([#OPS to Zero - Overs].Quantity,0)) as OPS2Zero,
			 avg(isnull(Adj.Quantity,0)) as Adjustments,
			 avg(isnull([#StockDifferences].Sales,0)) as [OPS-Sales],
			 avg(isnull([#StockDifferences].Returns,0)) as [OPS-Returns],
			 avg(isnull([#StockDifferences].[OPS2zero-Adjustment],0)) as [OPS2zero-Adjustment],
			 avg(isnull([#P2O-Returns].Quantity,0)) as Returns,
			 isnull(avg([OPS-QuantityOnHand]),0) AS [OPS-QuantityOnHand],
			 AVG(isnull(OPSCost,0)) AS OPSCost,
			 sum(isnull([Asycuda-PiQuantity],0)) AS [Asycuda-PiQuantity],
			 AVG(isnull(AsycudaCost,0)) AS AsycudaCost,
			 (isnull(avg([#StockDifferences].[OPS-Quantity]),0) +  avg(isnull(Adj.Quantity,0)) + avg(isnull([#P2O-Returns].Quantity,0)) + avg(isnull([#OPS to Zero - Overs].Quantity,0))) AS [OPS-Quantity],
			 sum(isnull([Asycuda-Quantity],0)) AS [Asycuda-Quantity],
			 (isnull(avg([#StockDifferences].[OPS-Quantity]),0) +  avg(isnull(Adj.Quantity,0)) + avg(isnull([#P2O-Returns].Quantity,0)) + avg(isnull([#OPS to Zero - Overs].Quantity,0)) - (isnull(sum([Asycuda-Quantity]), 0) -  isnull(sum([Asycuda-PiQuantity]), 0))) AS Diff,
			 AVG(isnull([#StockDifferences].Cost,0)) AS Cost,
			 AVG(isnull(TotalCost,0)) AS TotalCost
into #Results
FROM            [#StockDifferences] left outer join [#P2O-Returns] on [#StockDifferences].InventoryItemId = [#P2O-Returns].InventoryItemId
			    left outer join [#OPS to Zero - Overs] on [#StockDifferences].ItemNumber = [#OPS to Zero - Overs].ItemNumber
				left outer join (select * from [#adjustments] where isnull([#adjustments].IsReconciled,0) <> 1) as Adj on [#StockDifferences].ItemNumber = Adj.ItemNumber
GROUP BY coalesce([#StockDifferences].InvoiceNo, [#OPS to Zero - Overs].InvoiceNo, [#P2O-Returns].InvoiceNo),  coalesce([#StockDifferences].ItemNumber,[#OPS to Zero - Overs].ItemNumber, [#P2O-Returns].ItemNumber), coalesce([#StockDifferences].InventoryItemId,[#OPS to Zero - Overs].InventoryItemId, [#P2O-Returns].InventoryItemId)

--having not ((isnull(sum([Asycuda-Quantity]), 0) = 0 and isnull(sum([OPS-QuantityOnHand]), 0) = 0) --- remain quantities is zero for both asycuda and ops
--        and (isnull(sum([Asycuda-PiQuantity]), 0)) > isnull(avg([OPS-Quantity]),0))--- and pi is less than quantity in question

order by abs((isnull(avg([OPS-Quantity]),0) - isnull(sum([Asycuda-Quantity]), 0))) asc, coalesce([#StockDifferences].ItemNumber,[#OPS to Zero - Overs].ItemNumber, [#P2O-Returns].ItemNumber)


select '#Results'
select * from #Results 

select '#Completions'
drop table [#Completions]
select *  
into [#Completions]
from #Results
where ((isnull([Asycuda-Quantity], 0) = 0 and isnull([OPS-QuantityOnHand], 0) = 0) --- remain quantities is zero for both asycuda and ops
        --and (isnull([Asycuda-PiQuantity], 0)) > isnull([OPS-Quantity],0)
		)--- and pi is less than quantity in question
		and (diff = 0)
		and isnull([Asycuda-Quantity], 0) + isnull([OPS-Quantity],0) = 0

select * from [#Completions]


drop table [#InComplete]
select *  
into [#InComplete]
from #Results
where --not ((isnull([Asycuda-Quantity], 0) = 0 and isnull([OPS-QuantityOnHand], 0) = 0) --- remain quantities is zero for both asycuda and ops
       -- and (isnull([Asycuda-PiQuantity], 0)) > isnull([OPS-Quantity],0)
		--)--- and pi is less than quantity in question
		--and 
		(Diff <> 0)
		and isnull([Asycuda-Quantity], 0) + isnull([OPS-Quantity],0) <> 0
--select '#InComplete'
--select * from [#InComplete] where ItemNumber = @ItemNumber


drop table [#P2O-Overs]
SELECT  distinct 'P2O-Overs-' + FORMAT(@startdate, 'MMMyy') + '-' + FORMAT(@endDate, 'MMMyy') as [Invoice #], @startdate as [Date],[#Results].InvoiceNo, [#Results].ItemNumber,[#Results].InventoryItemId ,[#Results].Description, [#Results].[Asycuda-Quantity] as [From Quantity], [#Results].OPSCost, [#Results].[Quantity] as [To Quantity], [#Results].AsycudaCost, abs([#Results].Diff) as Quantity, [#Results].Cost, 'XCD' as Currency
into [#P2O-Overs]
FROM     [#InComplete] as [#Results]       left outer join [InventoryItems-NonStock] on [#Results].InventoryItemId = [InventoryItems-NonStock].InventoryItemId    
			                 
WHERE        ([#Results].InvoiceNo <> 'Asycuda') and cost <> 0 and [#Results].[Asycuda-Quantity] <> 0
				AND (diff > 0 and [Quantity] > 0 -- and [#Results].[OPS-Quantity] <> [#Results].[Asycuda-Quantity]
				)
				--and (Quantity <> [Asycuda-PiQuantity])  -- put this to prevent instructions for closed out items
				--and (diff <> [Asycuda-PiQuantity]) -- this where change was already exwarehoused eg - SPY/0799320
			    and [InventoryItems-NonStock].InventoryItemId is null
--select '#P2O-Overs'
--select * from [#P2O-Overs] where ItemNumber = @ItemNumber



drop table [#P2O-Shorts]
SELECT  distinct  'P2O-Shorts-' + FORMAT(@startdate, 'MMMyy') + '-' + FORMAT(@endDate, 'MMMyy') as [Invoice #], @endDate as [Date], [#Results].InvoiceNo, [#Results].ItemNumber, [#Results].InventoryItemId,[#Results].Description, [#Results].[Asycuda-Quantity] as [From Quantity], [#Results].OPSCost, [#Results].[Quantity] as [To Quantity], [#Results].AsycudaCost, [#Results].Diff * -1 as Quantity, [#Results].Cost, 'XCD' as Currency
into [#P2O-Shorts]
FROM     [#InComplete] as [#Results]   left outer join [InventoryItems-NonStock] on [#Results].InventoryItemId = [InventoryItems-NonStock].InventoryItemId   
			
WHERE        ([#Results].InvoiceNo <> 'Asycuda') AND (diff < 0 and [Quantity] >= 0) and [InventoryItems-NonStock].InventoryItemId is null 


--select 'P2O-Shorts'
--select * from [#P2O-Shorts] where ItemNumber = @ItemNumber



drop table  [#A2O-Overs]
SELECT  distinct      'A2O-Overs-' + FORMAT(@startdate, 'MMMyy') + '-' + FORMAT(@endDate, 'MMMyy') as [Invoice #], @startdate as [Date], [#Results].InvoiceNo, [#Results].ItemNumber, [#Results].InventoryItemId,[#Results].Description,[#Results].Returns,[#Results].OPS2Zero,[#Results].Adjustments , [#Results].[Asycuda-Quantity] as [From Quantity], [#Results].OPSCost, [#Results].[Quantity] as [To Quantity], [#Results].AsycudaCost, [#Results].Diff as Quantity, [#Results].Cost, 'XCD' as Currency
into [#A2O-Overs]
FROM     [#InComplete] as [#Results]       left outer join [InventoryItems-NonStock] on [#Results].InventoryItemId = [InventoryItems-NonStock].InventoryItemId                 
WHERE       ([#Results].InvoiceNo <> 'Asycuda' and [#Results].[Asycuda-Quantity] = 0)  and cost <> 0 and (diff > 0) and [InventoryItems-NonStock].InventoryItemId is null
			--and (Quantity <> [Asycuda-PiQuantity]) -- put this to prevent instructions for closed out items
			-- and (diff <> [Asycuda-PiQuantity]) -- this where change was already exwarehoused eg - SPY/0799320

--select 'A2O Overs'
--select * from  [#A2O-Overs] where ItemNumber = @ItemNumber


drop table  [#A2O-Shorts]
SELECT  distinct    'A2O-Shorts-' + FORMAT(@startdate, 'MMMyy') + '-' + FORMAT(@endDate, 'MMMyy') as [Invoice #], @endDate as [Date], [#Results].InvoiceNo, [#Results].ItemNumber, [#Results].InventoryItemId,[#Results].Description, [#Results].[Asycuda-Quantity] as [From Quantity], [#Results].OPSCost, [#Results].[OPS-Quantity] as [To Quantity], [#Results].AsycudaCost, [#Results].Diff * -1 as Quantity, [#Results].Cost, 'XCD' as Currency
into [#A2O-Shorts]
FROM   [#InComplete] as   [#Results]   left outer join [InventoryItems-NonStock] on [#Results].InventoryItemId = [InventoryItems-NonStock].InventoryItemId                       
WHERE        ([#Results].InvoiceNo = 'Asycuda') AND (diff < 0 and [OPS-Quantity] <= 0) and [InventoryItems-NonStock].InventoryItemId is null

--select 'A2O Shorts'
--select * from  [#A2O-Shorts] where ItemNumber = @ItemNumber


----------------------------------------------------------------------------------------------

drop table  [#Unresolved Issues]
SELECT  distinct     [#Results].*
into [#Unresolved Issues]
FROM     [#Results]   left outer join [InventoryItems-NonStock] on [#Results].InventoryItemId = [InventoryItems-NonStock].InventoryItemId                       
WHERE       diff <> 0 and cost <> 0 and  [InventoryItems-NonStock].InventoryItemId is null
		and (Quantity <> [Asycuda-PiQuantity]) -- put this to prevent instructions for closed out items

select 'Unresolved Issues'
select * from [#Unresolved Issues]
----------------------------------------------------------------------------------------------

drop table  [#Zero Cost Items]
SELECT  distinct     [#Results].*
into [#Zero Cost Items]
FROM     [#Results]   left outer join [InventoryItems-NonStock] on [#Results].InventoryItemId = [InventoryItems-NonStock].InventoryItemId                       
WHERE       Cost = 0 and  [InventoryItems-NonStock].InventoryItemId is null and diff <> 0

select 'Zero Cost Items'
select * from [#Zero Cost Items]
---------------------------------------------------------------------------------------------



drop table  [#Non-Stock-Differences]
SELECT  distinct     [#Results].InvoiceNo, [#Results].ItemNumber, [#Results].InventoryItemId,[#Results].Description, [#Results].[OPS-Quantity], [#Results].OPSCost, [#Results].[Asycuda-Quantity], [#Results].AsycudaCost, [#Results].Diff  as Diff, [#Results].Cost
into [#Non-Stock-Differences]
FROM     [#Results]   inner join [InventoryItems-NonStock] with (nolock) on [#Results].InventoryItemId = [InventoryItems-NonStock].InventoryItemId                       

select '#Non-Stock-Differences'
select * from  [#Non-Stock-Differences] where ItemNumber = @ItemNumber

----------------------------------------------------------------------------------------------

drop table [#Sales Errors -> to OPS - Overs]
SELECT  distinct     res.*
into [#Sales Errors -> to OPS - Overs]
FROM     [#P2O-Overs] as res inner join  EntryDataDetails  with (nolock) on res.InventoryItemId = Entrydatadetails.InventoryItemId INNER JOIN
                         AsycudaSalesAllocations with (nolock) ON EntryDataDetails.EntryDataDetailsId = AsycudaSalesAllocations.EntryDataDetailsId INNER JOIN
                         EntryData_Sales with (nolock) ON EntryDataDetails.EntryData_Id = EntryData_Sales.EntryData_Id INNER JOIN
                         EntryData with (nolock) ON EntryData_Sales.EntryData_Id = EntryData.EntryData_Id
                         
WHERE        (EntryData.EntryDataDate BETWEEN @startDate AND @enddate) and AsycudaSalesAllocations.Status is not null


drop table [#Sales Errors -> to OPS - Shorts]
SELECT  distinct     res.*
into [#Sales Errors -> to OPS - Shorts]
FROM     [#P2O-Shorts] as res inner join  EntryDataDetails  with (nolock) on res.InventoryItemId = Entrydatadetails.InventoryItemId INNER JOIN
                         AsycudaSalesAllocations with (nolock) ON EntryDataDetails.EntryDataDetailsId = AsycudaSalesAllocations.EntryDataDetailsId INNER JOIN
                         EntryData_Sales with (nolock) ON EntryDataDetails.EntryData_Id = EntryData_Sales.EntryData_Id INNER JOIN
                         EntryData with (nolock) ON EntryData_Sales.EntryData_Id = EntryData.EntryData_Id
                         
WHERE        (EntryData.EntryDataDate BETWEEN @startDate AND @enddate) and AsycudaSalesAllocations.Status is not null


drop table [#Sales Errors -> to A2O - Overs]
SELECT  distinct     res.*
into [#Sales Errors -> to A2O - Overs]
FROM     [#A2O-Overs] as res inner join  EntryDataDetails  with (nolock) on res.InventoryItemId = Entrydatadetails.InventoryItemId INNER JOIN
                         AsycudaSalesAllocations with (nolock) ON EntryDataDetails.EntryDataDetailsId = AsycudaSalesAllocations.EntryDataDetailsId INNER JOIN
                         EntryData_Sales with (nolock) ON EntryDataDetails.EntryData_Id = EntryData_Sales.EntryData_Id INNER JOIN
                         EntryData with (nolock) ON EntryData_Sales.EntryData_Id = EntryData.EntryData_Id                         
WHERE        (EntryData.EntryDataDate BETWEEN @startDate AND @enddate) and AsycudaSalesAllocations.Status is not null


drop table [#Sales Errors -> to A2O - Shorts]
SELECT  distinct     res.*
into [#Sales Errors -> to A2O - Shorts]
FROM     [#A2O-Shorts] as res inner join  EntryDataDetails  with (nolock) on res.InventoryItemId = Entrydatadetails.InventoryItemId INNER JOIN
                         AsycudaSalesAllocations with (nolock) ON EntryDataDetails.EntryDataDetailsId = AsycudaSalesAllocations.EntryDataDetailsId INNER JOIN
                         EntryData_Sales with (nolock) ON EntryDataDetails.EntryData_Id = EntryData_Sales.EntryData_Id INNER JOIN
                         EntryData with (nolock) ON EntryData_Sales.EntryData_Id = EntryData.EntryData_Id                         
WHERE        (EntryData.EntryDataDate BETWEEN @startDate AND @enddate) and AsycudaSalesAllocations.Status is not null


----------------------- Do Not Allocate Sales With Stock Issues-----------------------------------------
if(@DoNotAllocateStockIssues = 1)
Begin
update EntryDataDetails 
set DoNotAllocate = Null, [Status] = Null
where [Status] = 'Stock Issues'



update EntryDataDetails 
set DoNotAllocate = 1, [Status] = 'Stock Issues'
FROM     [#Results] inner join  EntryDataDetails  on #Results.InventoryItemId = Entrydatadetails.InventoryItemId  INNER JOIN
                         EntryData_Sales ON EntryDataDetails.EntryData_Id = EntryData_Sales.EntryData_Id INNER JOIN
                         EntryData ON EntryData_Sales.EntryData_Id = EntryData.EntryData_Id
                         
WHERE        ([#Results].InvoiceNo <> 'Asycuda') and (EntryDataDetails.[Status] = 'Stock Issues' or EntryDataDetails.[Status] is null) AND (diff <> 0) AND (EntryData.EntryDataDate BETWEEN @startDate AND @enddate)
End


if(@DoNotAllocateStockIssuesWithStockOvers = 1)
Begin
update EntryDataDetails 
set DoNotAllocate = Null, [Status] = Null
where [Status] = 'Stock Issues'



update EntryDataDetails 
set DoNotAllocate = 1, [Status] = 'Stock Issues'
FROM     [#Results] inner join  EntryDataDetails  on #Results.InventoryItemId = Entrydatadetails.InventoryItemId  INNER JOIN
                         EntryData_Sales ON EntryDataDetails.EntryData_Id = EntryData_Sales.EntryData_Id INNER JOIN
                         EntryData ON EntryData_Sales.EntryData_Id = EntryData.EntryData_Id
                         
WHERE        ([#Results].InvoiceNo <> 'Asycuda') and (EntryDataDetails.[Status] = 'Stock Issues' or EntryDataDetails.[Status] is null) AND (diff > 0) AND (EntryData.EntryDataDate BETWEEN @startDate AND @enddate)
End


--if(@DoNotAllocateStockIssuesWithSalesErrors = 1)
--Begin
--update EntryDataDetails 
--set DoNotAllocate = Null, [Status] = Null
--where [Status] = 'Stock Issues'



--update EntryDataDetails 
--set DoNotAllocate = 1, [Status] = 'Stock Issues'
--FROM     [#Results] inner join  EntryDataDetails  on #Results.ItemNumber = Entrydatadetails.ItemNumber INNER JOIN
--                         AsycudaSalesAllocations ON EntryDataDetails.EntryDataDetailsId = AsycudaSalesAllocations.EntryDataDetailsId INNER JOIN
--                         EntryData_Sales ON EntryDataDetails.InvoiceNo = EntryData_Sales.InvoiceNo INNER JOIN
--                         EntryData ON EntryData_Sales.InvoiceNo = EntryData.InvoiceNo
                         
--WHERE        (EntryDataDetails.InvoiceNo <> 'Asycuda') and (EntryDataDetails.[Status] = 'Stock Issues' or EntryDataDetails.[Status] is null) AND (diff > 0) AND (EntryData.Date BETWEEN @startDate AND @enddate) and AsycudaSalesAllocations.Status is not null
--End



----------------------------------------------------------------------------------------------

select 'Sales with Stock Issues'
SELECT        EntryDataDetails.EntryDataId, EntryData.EntryDataDate, EntryDataDetails.LineNumber, EntryDataDetails.TaxAmount, EntryDataDetails.ItemNumber, EntryDataDetails.ItemDescription, EntryDataDetails.Quantity, 
                         EntryDataDetails.Cost, EntryDataDetails.DoNotAllocate, EntryDataDetails.Status
FROM            EntryData with (nolock) INNER JOIN
                         EntryData_Sales  with (nolock) ON EntryData.EntryData_Id = EntryData_Sales.EntryData_Id INNER JOIN
                         EntryDataDetails  with (nolock) ON EntryData.EntryData_Id = EntryDataDetails.EntryData_Id
where EntryDataDetails.[Status] = 'Stock Issues' 


select '/////////////////////////////////////////////////////////////////////////////////////////////'


--////////////////////////////////////////////////////////////
select 'Unaddressed Results in incomplete table'
select [#InComplete].* from [#InComplete] left outer join [#P2O-Overs] on [#InComplete].InventoryItemId = [#P2O-Overs].InventoryItemId
							left outer join [#P2O-Shorts] on [#InComplete].InventoryItemId = [#P2O-Shorts].InventoryItemId
							left outer join [#A2O-Overs] on [#InComplete].InventoryItemId = [#A2O-Overs].InventoryItemId
							left outer join [#A2O-Shorts] on [#InComplete].InventoryItemId = [#A2O-Shorts].InventoryItemId
							left outer join [#Non-Stock-Differences] on [#InComplete].InventoryItemId = [#Non-Stock-Differences].InventoryItemId
where  [#P2O-Overs].ItemNumber is null and [#P2O-Shorts].ItemNumber is null and [#A2O-Overs].ItemNumber is null and [#A2O-Shorts].ItemNumber is null and [#Non-Stock-Differences].ItemNumber is null
       and [#InComplete].diff <> 0
	   and ([#InComplete].[Quantity] <> [Asycuda-PiQuantity]) -- put this to prevent instructions for closed out items

--select '///////////////////////////////////////////////////'
--select 'Partial Adjustments'
--SELECT AdjustmentDetails.EntryDataId AS InvoiceNo, AdjustmentDetails.EffectiveDate, CASE WHEN AliasName IS NOT NULL THEN inventoryitemalias.itemnumber ELSE AdjustmentDetails.ItemNumber END AS ItemNumber, 
--                 AdjustmentDetails.ItemDescription, SUM(ISNULL(AdjustmentDetails.ReceivedQty, 0) - ISNULL(AdjustmentDetails.InvoiceQty, 0)) AS Quantity, AVG(AdjustmentDetails.Cost) AS Cost, 
--                sum(AsycudaItemBasicInfo.ItemQuantity) as ItemQuantity
--FROM    AsycudaItemBasicInfo INNER JOIN
--                 AsycudaDocumentItemEntryDataDetails ON AsycudaItemBasicInfo.Item_Id = AsycudaDocumentItemEntryDataDetails.Item_Id RIGHT OUTER JOIN
--                 AdjustmentDetails LEFT OUTER JOIN
--                 InventoryItemAlias ON AdjustmentDetails.ItemNumber = InventoryItemAlias.AliasName ON AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId = AdjustmentDetails.EntryDataDetailsId
--WHERE (AdjustmentDetails.EffectiveDate BETWEEN @startdate AND @endDate) AND (AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId IS NOT NULL)
--GROUP BY CASE WHEN AliasName IS NOT NULL THEN inventoryitemalias.itemnumber ELSE AdjustmentDetails.ItemNumber END, AdjustmentDetails.ItemDescription, AdjustmentDetails.EntryDataId, 
--                 AdjustmentDetails.EffectiveDate
--HAVING (abs(SUM(ISNULL(AdjustmentDetails.ReceivedQty, 0) - ISNULL(AdjustmentDetails.InvoiceQty, 0))) <> sum(AsycudaItemBasicInfo.ItemQuantity))
--select '///////////////////////////////////////////////////'




drop table [#Unexecuted Adjustments]


SELECT AdjustmentDetails.EntryDataDetailsId, AdjustmentDetails.EntryDataId AS InvoiceNo, AdjustmentDetails.EffectiveDate, CASE WHEN AliasName IS NOT NULL 
                 THEN inventoryitemalias.itemnumber ELSE AdjustmentDetails.ItemNumber END AS ItemNumber, AdjustmentDetails.ItemDescription, SUM(ISNULL(AdjustmentDetails.ReceivedQty, 0) 
                 - ISNULL(AdjustmentDetails.InvoiceQty, 0)) AS Quantity, AVG(AdjustmentDetails.Cost) AS Cost, AdjustmentDetails.IsReconciled
into [#Unexecuted Adjustments]
FROM    AdjustmentDetails  with (nolock) LEFT OUTER JOIN
               InventoryItemAliasEx as  InventoryItemAlias  with (nolock) ON AdjustmentDetails.InventoryItemId = InventoryItemAlias.InventoryItemId LEFT OUTER JOIN
                 AsycudaDocumentItemEntryDataDetails with (nolock) ON AdjustmentDetails.EntryDataDetailsId = AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId
WHERE (AdjustmentDetails.ApplicationSettingsId = @ApplicationSettingsId) and (AdjustmentDetails.EffectiveDate BETWEEN @startdate AND @endDate) AND (AsycudaDocumentItemEntryDataDetails.EntryDataDetailsId IS NULL and isnull(AdjustmentDetails.IsReconciled, 0) <> 1)
GROUP BY CASE WHEN AliasName IS NOT NULL THEN inventoryitemalias.itemnumber ELSE AdjustmentDetails.ItemNumber END, AdjustmentDetails.ItemDescription, AdjustmentDetails.EntryDataId, 
                 AdjustmentDetails.EffectiveDate, AdjustmentDetails.EntryDataDetailsId, AdjustmentDetails.IsReconciled
select 'Unexecuted Adjustments'
select * from [#Unexecuted Adjustments]

--update EntryDataDetails
--set IsReconciled = 1
--from EntryDataDetails inner join [#Unexecuted Adjustments] on EntryDataDetails.EntryDataDetailsId = [#Unexecuted Adjustments].EntryDataDetailsId

--update EntryDataDetails
--set IsReconciled = null

--go
select 'Sales Errors -> to OPS - Overs'
select * from [#Sales Errors -> to OPS - Overs] --where ItemNumber = @ItemNumber
--select 'Sales Errors -> to OPS - Shorts'
--select * from [#Sales Errors -> to OPS - Shorts]
select 'Sales Errors -> to A2O - Over'
select * from [#Sales Errors -> to A2O - Overs]
--select 'Sales Errors -> to A2O - Shorts'
--select * from [#Sales Errors -> to A2O - Shorts]

--go
select '#P2O-Overs'
select * from [#P2O-Overs]
select 'P2O-Shorts'
select * from [#P2O-Shorts]
select 'A2O Overs'
select * from  [#A2O-Overs] 
select 'A2O Shorts'
select * from  [#A2O-Shorts]



select 'Unexecuted shorts'
SELECT distinct AdjustmentShortAllocations.AllocationId, AdjustmentShortAllocations.TotalValue, AdjustmentShortAllocations.AllocatedValue, AdjustmentShortAllocations.Status, AdjustmentShortAllocations.QtyAllocated, 
                 AdjustmentShortAllocations.xLineNumber, AdjustmentShortAllocations.PreviousItem_Id, AdjustmentShortAllocations.InvoiceDate, AdjustmentShortAllocations.SalesQuantity, 
                 AdjustmentShortAllocations.SalesQtyAllocated, AdjustmentShortAllocations.InvoiceNo, AdjustmentShortAllocations.ItemNumber, AdjustmentShortAllocations.ItemDescription, 
                 AdjustmentShortAllocations.EntryDataDetailsId, AdjustmentShortAllocations.xBond_Item_Id, AdjustmentShortAllocations.pCNumber, AdjustmentShortAllocations.pRegistrationDate, 
                 AdjustmentShortAllocations.pQuantity, AdjustmentShortAllocations.pQtyAllocated, AdjustmentShortAllocations.PiQuantity, AdjustmentShortAllocations.SalesFactor, AdjustmentShortAllocations.xCNumber, 
                 AdjustmentShortAllocations.xRegistrationDate, AdjustmentShortAllocations.xQuantity, AdjustmentShortAllocations.pReferenceNumber, AdjustmentShortAllocations.pLineNumber, 
                 AdjustmentShortAllocations.xASYCUDA_Id, AdjustmentShortAllocations.pASYCUDA_Id, AdjustmentShortAllocations.Cost, AdjustmentShortAllocations.Total_CIF_itm, AdjustmentShortAllocations.DutyLiability, 
                 AdjustmentShortAllocations.pIsAssessed, AdjustmentShortAllocations.DoNotAllocateSales, AdjustmentShortAllocations.DoNotAllocatePreviousEntry, AdjustmentShortAllocations.WarehouseError, 
                 AdjustmentShortAllocations.SANumber, AdjustmentShortAllocations.xReferenceNumber, AdjustmentShortAllocations.TariffCode, AdjustmentShortAllocations.Invalid, AdjustmentShortAllocations.pExpiryDate, 
                 AdjustmentShortAllocations.pTariffCode, AdjustmentShortAllocations.pItemNumber, AdjustmentShortAllocations.EffectiveDate, AdjustmentShortAllocations.Comment, 
                 AdjustmentShortAllocations.Asycudadocumentsetid, AsycudaItemPiQuantityData.Item_Id, AsycudaItemPiQuantityData.PiQuantity AS Expr1
FROM    AdjustmentShortAllocations with (nolock) LEFT OUTER JOIN
                 [AsycudaItemPiQuantityData-basic] as AsycudaItemPiQuantityData with (nolock) ON AdjustmentShortAllocations.PreviousItem_Id = AsycudaItemPiQuantityData.Item_Id
WHERE (AdjustmentShortAllocations.ApplicationSettingsId = @ApplicationSettingsId) and ((AsycudaItemPiQuantityData.Item_Id IS NULL)) --AdjustmentShortAllocations.ItemNumber = 'AWL/G8044QT' AND 

select 'Unexecuted Allocations'
SELECT distinct AdjustmentShortAllocations.AllocationId, AdjustmentShortAllocations.TotalValue, AdjustmentShortAllocations.AllocatedValue, AdjustmentShortAllocations.Status, AdjustmentShortAllocations.QtyAllocated, 
                 AdjustmentShortAllocations.xLineNumber, AdjustmentShortAllocations.PreviousItem_Id, AdjustmentShortAllocations.InvoiceDate, AdjustmentShortAllocations.SalesQuantity, 
                 AdjustmentShortAllocations.SalesQtyAllocated, AdjustmentShortAllocations.InvoiceNo, AdjustmentShortAllocations.ItemNumber, AdjustmentShortAllocations.ItemDescription, 
                 AdjustmentShortAllocations.EntryDataDetailsId, AdjustmentShortAllocations.xBond_Item_Id, AdjustmentShortAllocations.pCNumber, AdjustmentShortAllocations.pRegistrationDate, 
                 AdjustmentShortAllocations.pQuantity, AdjustmentShortAllocations.pQtyAllocated, AdjustmentShortAllocations.PiQuantity, AdjustmentShortAllocations.SalesFactor, AdjustmentShortAllocations.xCNumber, 
                 AdjustmentShortAllocations.xRegistrationDate, AdjustmentShortAllocations.xQuantity, AdjustmentShortAllocations.pReferenceNumber, AdjustmentShortAllocations.pLineNumber, 
                 AdjustmentShortAllocations.xASYCUDA_Id, AdjustmentShortAllocations.pASYCUDA_Id, AdjustmentShortAllocations.Cost, AdjustmentShortAllocations.Total_CIF_itm, AdjustmentShortAllocations.DutyLiability, 
                 AdjustmentShortAllocations.pIsAssessed, AdjustmentShortAllocations.DoNotAllocateSales, AdjustmentShortAllocations.DoNotAllocatePreviousEntry, AdjustmentShortAllocations.WarehouseError, 
                 AdjustmentShortAllocations.SANumber, AdjustmentShortAllocations.xReferenceNumber, AdjustmentShortAllocations.TariffCode, AdjustmentShortAllocations.Invalid, AdjustmentShortAllocations.pExpiryDate, 
                 AdjustmentShortAllocations.pTariffCode, AdjustmentShortAllocations.pItemNumber, 
                 AsycudaItemPiQuantityData.Item_Id, AsycudaItemPiQuantityData.PiQuantity AS Expr1
FROM   AsycudaSalesAllocationsEx as AdjustmentShortAllocations with (nolock) LEFT OUTER JOIN
                 [AsycudaItemPiQuantityData-basic] as AsycudaItemPiQuantityData  with (nolock) ON AdjustmentShortAllocations.PreviousItem_Id = AsycudaItemPiQuantityData.Item_Id
WHERE (AdjustmentShortAllocations.ApplicationSettingsId = @ApplicationSettingsId) and ((AsycudaItemPiQuantityData.Item_Id IS NULL))

go
--go
--TODO: check out AOR/151550F, TAY/71026
							--POR/385007 = 0  AAA/13576 = 0  MMM/05205 = 0   BLF/1169 +2,  AAA/10240 - 3
							----------------After Adjustments entered
							-- 7IT/PR-LBL-2X1 = 0 just 3 from adjustments no recon needed
						     --returns  short 7IT/PR-LBL-2X1 BC/3 Fuse, YZR/F987HGS, LUC/1001
							 -- LUC/1001 = 0 - the returnt cancels the adjustment = 4 with 1 pi
							-- just adjustment alone:FAA/SBHC716X2
							-- UND/MHS126 zeroed so no instruction needed
							----- AOR/102510F delay pi until assessmentdate
							--SNS/50091443,SEH/1231GH,LOF/CB3M1712
							--FAS/SWFL6 - current no sales no pi
							---'UND/ATLANTIS270' the adjustment came first and was already pi therefore will not im9 it
							---THI/G20 = 0 adjustment from manaually assessed entry
							---'MMM/09006' - 8 because everything allocated but used ex9bucket to create a partial execution 
							---AOR/151550F=422 - no asycuda entries found
							---SEH/1231GH +2 after recon cuz sales already take out 
							--PSP/085005000,HS/SPL038
							--LOF/CB3M1712 +0 has unexecuted adjustment
							--AB/205000100002 = 0
							-- 8BM/MK-BAG-REUSE510 +2
							-- HS/SAN2 -156 to 124
							-- WES/404-45 +10 - only sales completely closed out	
							--MAR/199119 -292 then -2 the reason is 2 from could not be ex9 because of pre existing sales allocations was different to pi
							--------------so i added the extra 2 to short file and reconciled the short that couldn't be executed because of sales 
							--LOF/BK33609 reconciled after recon
							--LTR/3BPS-E +9 this is a sales factor issue 10 in pos 1 in asycuda
							------------remaining amount vs total amount 
							--7IT/PR-LBL-2X1 -0 
							-- vs FAS/SWFL6 -4
							-------------Diff <> piquantity
							---  XAN/813-0400-01 +1
							---- vs SPY/0799320 0
declare @ItemNumber varchar(50) = 'XMA4FLW3'
select 'Unexecuted Adjustments'
select * from [#Unexecuted Adjustments] where itemnumber = @ItemNumber
select 'Adjustments-Shorts Data'
select * from  #adjmentsData where itemnumber = @ItemNumber
select 'Sales Data'
select * from  #salesData where itemnumber = @ItemNumber
select 'Returns'
select * from  #returns where itemnumber = @ItemNumber
select 'Sales'
select * from  #sales where itemnumber = @ItemNumber
select 'adjustments-Shorts'
select * from  #adjustments where itemnumber = @ItemNumber
select 'Mapping Issues'
		 select * from [#MappingIssues] where itemnumber = @ItemNumber
select 'AsycudaData'						 
select * from #AsycudaData where itemnumber = @ItemNumber order by AssessmentDate asc
select '#AsycudaData-ItemAlias'
select * from [#AsycudaData-ItemAlias] where itemnumber = @ItemNumber
select '[#AsycudaSummary] with itemalias'
select * from [#AsycudaSummary] where itemnumber = @ItemNumber
select '#OPSData'
select * from #OPSData where itemnumber = @ItemNumber
select 'OPS'
select * from #OPS where itemnumber = @ItemNumber

--select '#StockDifferences'
--select * from [#StockDifferences] where itemnumber = @ItemNumber
select 'OPS to Zero - Overs'
select * from [#OPS to Zero - Overs]  where ItemNumber = @ItemNumber
select 'P2O-Returns'
select * from [#P2O-Returns] where ItemNumber = @ItemNumber
select '#Results'
select * from #Results where ItemNumber = @ItemNumber
select '#Completions'
select * from [#Completions] where ItemNumber = @ItemNumber
select '#InComplete'
select * from [#InComplete] where ItemNumber = @ItemNumber
select '#P2O-Overs'
select * from [#P2O-Overs] where ItemNumber = @ItemNumber
select 'P2O-Shorts'
select * from [#P2O-Shorts] where ItemNumber = @ItemNumber
select 'A2O Overs'
select * from  [#A2O-Overs]  where ItemNumber = @ItemNumber
select 'A2O Shorts'
select * from  [#A2O-Shorts] where ItemNumber = @ItemNumber


