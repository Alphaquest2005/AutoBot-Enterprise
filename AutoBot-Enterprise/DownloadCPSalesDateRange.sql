 declare @StartDate datetime = '1/1/2021', @EndDate datetime = '1/1/2022',  @AsycudaDocumentSetId int = 4286,  @ApplicationSettingsId int = 5
 
									DELETE FROM EntryData
									FROM    EntryData INNER JOIN
														 (SELECT DISTINCT INVNO, DATE
														  FROM     CounterPointSales) AS c ON EntryData.EntryDataId COLLATE DATABASE_DEFAULT = c.INVNO COLLATE DATABASE_DEFAULT AND EntryData.EntryDataDate = c.DATE
									WHERE (c.DATE >= @StartDate) AND (c.DATE <= @EndDate)  and EntryData.ApplicationSettingsId = @ApplicationSettingsId
            
                                    INSERT INTO EntryData
                                                      (EntryDataId, EntryDataDate, ApplicationSettingsId)
                                    SELECT distinct INVNO, DATE, @ApplicationSettingsId
                                    FROM     CounterPointSales
                                    WHERE  (DATE >= @StartDate and DATE <= @EndDate)

                                    INSERT INTO AsycudaDocumentSetEntryData
                                                      (EntryData_Id, AsycudaDocumentSetId)
                                    SELECT DISTINCT EntryData.EntryData_Id, @AsycudaDocumentSetId AS Expr1
									FROM    CounterPointSales INNER JOIN
													 EntryData ON CounterPointSales.INVNO COLLATE DATABASE_DEFAULT = EntryData.EntryDataId COLLATE DATABASE_DEFAULT AND CounterPointSales.DATE = EntryData.EntryDataDate
									WHERE (CounterPointSales.DATE >= @StartDate) AND (CounterPointSales.DATE <= @EndDate)

                                    DELETE From EntryData_Sales
                                    Where EntryData_Id in (SELECT DISTINCT EntryData.EntryData_Id
														FROM    CounterPointSales INNER JOIN
																		 EntryData ON CounterPointSales.INVNO COLLATE DATABASE_DEFAULT = EntryData.EntryDataId COLLATE DATABASE_DEFAULT AND CounterPointSales.DATE = EntryData.EntryDataDate
														WHERE (CounterPointSales.DATE >= @StartDate) AND (CounterPointSales.DATE <= @EndDate))

                                    INSERT INTO EntryData_Sales
                                                      (EntryData_Id, INVNumber, Tax,CustomerName)
                                    SELECT DISTINCT EntryData.EntryData_Id, CounterPointSales.INVNO, CounterPointSales.TAX_AMT, CounterPointSales.[CUSTOMER NAME]
									FROM    CounterPointSales INNER JOIN
													 EntryData ON CounterPointSales.INVNO COLLATE DATABASE_DEFAULT = EntryData.EntryDataId COLLATE DATABASE_DEFAULT AND CounterPointSales.DATE = EntryData.EntryDataDate
									WHERE (CounterPointSales.DATE >= @StartDate) AND (CounterPointSales.DATE <= @EndDate)


                                    INSERT INTO InventoryItems
                                                      (ItemNumber, Description, ApplicationSettingsId)
                                    SELECT CounterPointSalesDetails.ITEM_NO, MAX(CounterPointSalesDetails.ITEM_DESCR) AS ITEM_DESCR, @ApplicationSettingsId
                                    FROM     CounterPointSalesDetails LEFT OUTER JOIN
                                                      InventoryItems AS InventoryItems_1 ON CounterPointSalesDetails.ITEM_NO COLLATE DATABASE_DEFAULT = InventoryItems_1.ItemNumber COLLATE DATABASE_DEFAULT
                                    WHERE  (CounterPointSalesDetails.DATE >= @StartDate) AND (CounterPointSalesDetails.DATE <= @EndDate) AND (InventoryItems_1.ItemNumber IS NULL)
                                    GROUP BY CounterPointSalesDetails.ITEM_NO
                                    HAVING (LEFT(CounterPointSalesDetails.ITEM_NO, 1) <> '*')   

                                    INSERT INTO EntryDataDetails
                                                      (EntryData_Id,EntryDataId, LineNumber, ItemNumber, Quantity, Units, ItemDescription, Cost, UnitWeight, QtyAllocated, InventoryItemId)
                                    SELECT EntryData.EntryData_Id, CounterPointSalesDetails.INVNO, CounterPointSalesDetails.SEQ_NO, CounterPointSalesDetails.ITEM_NO, CounterPointSalesDetails.QUANTITY, CounterPointSalesDetails.QTY_UNIT, 
													 CounterPointSalesDetails.ITEM_DESCR, CounterPointSalesDetails.COST, ISNULL(CounterPointSalesDetails.UNIT_WEIGHT, 0) AS Expr1, 0 AS Expr2, InventoryItems.Id
									FROM    CounterPointSalesDetails INNER JOIN
													 EntryData ON CounterPointSalesDetails.INVNO = EntryData.EntryDataId COLLATE DATABASE_DEFAULT AND CounterPointSalesDetails.DATE = EntryData.EntryDataDate INNER JOIN
													 InventoryItems ON EntryData.ApplicationSettingsId = InventoryItems.ApplicationSettingsId AND CounterPointSalesDetails.ITEM_NO = InventoryItems.ItemNumber COLLATE DATABASE_DEFAULT
									WHERE (CounterPointSalesDetails.DATE >= @StartDate) AND (CounterPointSalesDetails.DATE <= @EndDate) AND (LEFT(CounterPointSalesDetails.ITEM_NO, 1) <> '*')



		----------------------------------------------------
		----------------------------------------------------

		 declare @Date datetime = '1/1/2021', @INVNumber varchar(50)--  @AsycudaDocumentSetId int = 4286,  @ApplicationSettingsId int = 5

									declare @entryData_Id int
									set @entryData_Id = (select distinct entrydata_id from entrydata where entrydataid = @INVNumber and EntryDataDate = @Date and ApplicationSettingsId = @ApplicationSettingsId)

									Delete from EntryData
                                    Where EntryData_Id = @entryData_Id

                                    Delete from EntryData_Sales
                                    Where EntryData_Id = @entryData_Id

                                    INSERT INTO EntryData
                                                      (EntryDataId, EntryDataDate, ApplicationSettingsId)
                                    SELECT INVNO, DATE, @ApplicationSettingsId
                                    FROM     CounterPointSales
                                    WHERE  (INVNO = @INVNumber)

                                    INSERT INTO AsycudaDocumentSetEntryData
                                                      (EntryData_Id, AsycudaDocumentSetId)
                                    SELECT EntryData.EntryData_Id, @AsycudaDocumentSetId AS Expr1
									FROM    CounterPointSales INNER JOIN
													 EntryData ON CounterPointSales.INVNO COLLATE DATABASE_DEFAULT = EntryData.EntryDataId COLLATE DATABASE_DEFAULT AND CounterPointSales.DATE = EntryData.EntryDataDate
									WHERE (CounterPointSales.INVNO = @INVNumber)

                                    INSERT INTO EntryData_Sales
                                                      (EntryData_Id, INVNumber, Tax,CustomerName)
                                    SELECT DISTINCT EntryData.EntryData_Id, CounterPointSales.INVNO, CounterPointSales.TAX_AMT, CounterPointSales.[CUSTOMER NAME]
									FROM    CounterPointSales INNER JOIN
													 EntryData ON CounterPointSales.INVNO COLLATE DATABASE_DEFAULT = EntryData.EntryDataId COLLATE DATABASE_DEFAULT AND CounterPointSales.DATE = EntryData.EntryDataDate
									WHERE (CounterPointSales.INVNO = @INVNumber)

                                    INSERT INTO InventoryItems
                                                      (ItemNumber, Description, ApplicationSettingsId)
                                    SELECT CounterPointSalesDetails.ITEM_NO, CounterPointSalesDetails.ITEM_DESCR, EntryData.ApplicationSettingsId
									FROM    CounterPointSalesDetails INNER JOIN
													 EntryData ON CounterPointSalesDetails.INVNO  COLLATE DATABASE_DEFAULT = EntryData.EntryDataId  COLLATE DATABASE_DEFAULT AND CounterPointSalesDetails.DATE = EntryData.EntryDataDate LEFT OUTER JOIN
													 InventoryItems AS InventoryItems_1 ON CounterPointSalesDetails.ITEM_NO  COLLATE DATABASE_DEFAULT = InventoryItems_1.ItemNumber  COLLATE DATABASE_DEFAULT
									WHERE (CounterPointSalesDetails.INVNO = @INVNumber and CounterPointSalesDetails.Date = @Date) AND (InventoryItems_1.ItemNumber IS NULL) AND (LEFT(CounterPointSalesDetails.ITEM_NO, 1) <> '*') AND (EntryData.ApplicationSettingsId = @ApplicationSettingsId)

                                    INSERT INTO EntryDataDetails
                                                      (EntryData_Id,EntryDataId, LineNumber, ItemNumber, Quantity, Units, ItemDescription, Cost, UnitWeight, QtyAllocated, InventoryItemId)
                                    SELECT EntryData.EntryData_Id, CounterPointSalesDetails.INVNO, CounterPointSalesDetails.SEQ_NO, CounterPointSalesDetails.ITEM_NO, CounterPointSalesDetails.QUANTITY, CounterPointSalesDetails.QTY_UNIT, 
													 CounterPointSalesDetails.ITEM_DESCR, CounterPointSalesDetails.COST, ISNULL(CounterPointSalesDetails.UNIT_WEIGHT, 0) AS Expr1, 0 AS Expr2, InventoryItems.Id
									FROM    CounterPointSalesDetails INNER JOIN
													 EntryData ON CounterPointSalesDetails.INVNO COLLATE DATABASE_DEFAULT = EntryData.EntryDataId COLLATE DATABASE_DEFAULT AND CounterPointSalesDetails.DATE = EntryData.EntryDataDate INNER JOIN
													 InventoryItems ON CounterPointSalesDetails.INVNO  COLLATE DATABASE_DEFAULT = InventoryItems.ItemNumber  COLLATE DATABASE_DEFAULT AND EntryData.ApplicationSettingsId = InventoryItems.ApplicationSettingsId
									WHERE (CounterPointSalesDetails.INVNO = @INVNumber) AND (CounterPointSalesDetails.DATE = @Date) AND (LEFT(CounterPointSalesDetails.ITEM_NO, 1) <> '*') AND 
													 (EntryData.ApplicationSettingsId = @ApplicationSettingsId)


		-------------------------------------------------------------------
		-------------------------------------------------------------------
									 declare @PONumber varchar(50)

									 declare @entryData_Id int
									set @entryData_Id = (select distinct entrydata_id from entrydata where entrydataid = @PONumber and EntryDataDate = @Date and ApplicationSettingsId = @ApplicationSettingsId)


									Delete from EntryData
                                    Where EntryData_Id = @entryData_Id

                                    Delete from EntryData_PurchaseOrders
                                    Where EntryData_Id = @entryData_Id

                                    INSERT INTO EntryData
                                                      (EntryDataId, EntryDataDate, ApplicationSettingsId)
                                    SELECT PO_NO, DATE, @ApplicationSettingsId
                                    FROM     CounterPointPOs
                                    WHERE  (PO_NO = @PONumber and [Date] = @Date)

                                    INSERT INTO AsycudaDocumentSetEntryData
                                                      (EntryData_Id, AsycudaDocumentSetId)
                                   SELECT CounterPointPOs.PO_NO, @AsycudaDocumentSetId AS Expr1
									FROM    CounterPointPOs INNER JOIN
													 EntryData ON CounterPointPOs.PO_NO COLLATE Database_Default = EntryData.EntryDataId COLLATE Database_Default AND CounterPointPOs.DATE = EntryData.EntryDataDate
									WHERE (CounterPointPOs.PO_NO = @PONumber) AND (CounterPointPOs.DATE = @Date) and entrydata.ApplicationSettingsId = @ApplicationSettingsId

                                    INSERT INTO EntryData_PurchaseOrders
                                                      (EntryData_Id, PONumber)
                                    SELECT distinct EntryData_Id, PO_NO AS Expr1
                                    FROM    CounterPointPOs INNER JOIN
													 EntryData ON CounterPointPOs.PO_NO COLLATE Database_Default = EntryData.EntryDataId COLLATE Database_Default AND CounterPointPOs.DATE = EntryData.EntryDataDate
									WHERE (CounterPointPOs.PO_NO = @PONumber) AND (CounterPointPOs.DATE = @Date) and entrydata.ApplicationSettingsId = @ApplicationSettingsId

                                    INSERT INTO InventoryItems
                                                      (ItemNumber, Description, ApplicationSettingsId)
                                    SELECT DISTINCT CounterPointPODetails.ITEM_NO, CounterPointPODetails.ITEM_DESCR,  EntryData.ApplicationSettingsId
									FROM    CounterPointPODetails INNER JOIN
													 EntryData ON CounterPointPODetails.PO_NO COLLATE Database_Default  = EntryData.EntryDataId COLLATE Database_Default  LEFT OUTER JOIN
													 InventoryItems AS InventoryItems_1 ON CounterPointPODetails.ITEM_NO = InventoryItems_1.ItemNumber
									WHERE (CounterPointPODetails.PO_NO = @PONumber) AND (InventoryItems_1.ItemNumber IS NULL) AND (LEFT(CounterPointPODetails.ITEM_NO, 1) <> '*') AND (EntryData.ApplicationSettingsId = @applicationSettingsId) AND 
													 (EntryData.EntryDataDate = @Date)

                                    INSERT INTO EntryDataDetails
                                                      (EntryDataId, LineNumber, ItemNumber, Quantity, Units, ItemDescription, Cost, UnitWeight, QtyAllocated,InventoryItemId)
									SELECT EntryData.EntryData_Id, CounterPointPODetails.PO_NO, CounterPointPODetails.SEQ_NO, CounterPointPODetails.ITEM_NO, CounterPointPODetails.ORD_QTY, CounterPointPODetails.ORD_UNIT, 
													 CounterPointPODetails.ITEM_DESCR, CounterPointPODetails.ORD_COST, CounterPointPODetails.UNIT_WEIGHT, 0 AS Expr1, InventoryItems.Id
									FROM    CounterPointPODetails INNER JOIN
													 EntryData ON CounterPointPODetails.PO_NO COLLATE DATABASE_DEFAULT = EntryData.EntryDataId COLLATE DATABASE_DEFAULT INNER JOIN
													 InventoryItems ON CounterPointPODetails.PO_NO  COLLATE Database_Default = InventoryItems.ItemNumber  COLLATE Database_Default AND EntryData.ApplicationSettingsId = InventoryItems.ApplicationSettingsId
									WHERE (CounterPointPODetails.PO_NO = @PONumber) AND (LEFT(CounterPointPODetails.ITEM_NO, 1) <> '*') AND (EntryData.ApplicationSettingsId = @applicationsettingsId) AND (EntryData.EntryDataDate = @Date)

