using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Core.Common.UI;
using CounterPointQS.Business.Entities;
using WaterNut.DataLayer;

namespace WaterNut.DataSpace
{
    public class CPSalesModel
    {
        static CPSalesModel()
        {
            Instance = new CPSalesModel();
        }

        public static CPSalesModel Instance { get; }

        public async Task DownloadCPSales(CounterPointSales c, int docSetId)
        {
            //WaterNutDBEntities db = BaseDataModel.db;//new WaterNutDBEntities(Properties.Settings.Default.WaterNutDBEntitiesConnection);
            if (docSetId != 0)
            {
                StatusModel.Timer("Downloading CP Sales...");
                using (var ctx = new WaterNutDBEntities {CommandTimeout = 0})
                {
                    ctx.ExecuteStoreCommand(@"
                            
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
                                    FROM    InventoryItemSource INNER JOIN
                                                        InventoryItems AS InventoryItems_1 ON InventoryItemSource.InventoryId = InventoryItems_1.Id INNER JOIN
                                                        InventorySources ON InventoryItemSource.InventorySourceId = InventorySources.Id RIGHT OUTER JOIN
                                                        CounterPointSalesDetails INNER JOIN
                                                        EntryData ON CounterPointSalesDetails.INVNO COLLATE DATABASE_DEFAULT = EntryData.EntryDataId COLLATE DATABASE_DEFAULT AND CounterPointSalesDetails.DATE = EntryData.EntryDataDate ON 
                                                        InventoryItems_1.ApplicationSettingsId = EntryData.ApplicationSettingsId AND 
                                                        InventoryItems_1.ItemNumber COLLATE DATABASE_DEFAULT = CounterPointSalesDetails.ITEM_NO COLLATE DATABASE_DEFAULT
                                    WHERE (CounterPointSalesDetails.INVNO = @INVNumber) AND (CounterPointSalesDetails.DATE = @Date) AND (InventoryItems_1.ItemNumber IS NULL) AND (LEFT(CounterPointSalesDetails.ITEM_NO, 1) <> '*') AND 
                                                        (EntryData.ApplicationSettingsId = @ApplicationSettingsId) 

				                    insert into InventoryItemSource(InventoryId, InventorySourceId)
									SELECT        InventoryItems.Id AS InventoryItemId, InventorySources.Id AS InventorySourceId
									FROM            InventoryItems LEFT OUTER JOIN
															 InventoryItemSource ON InventoryItems.Id = InventoryItemSource.InventoryId CROSS JOIN
															 InventorySources
									WHERE        (InventorySources.Name = N'POS') AND (InventoryItemSource.Id IS NULL)


                                    INSERT INTO EntryDataDetails
                                                      (EntryData_Id,EntryDataId, LineNumber, ItemNumber, Quantity, Units, ItemDescription, Cost, UnitWeight, QtyAllocated, InventoryItemId)
                                    SELECT EntryData.EntryData_Id, CounterPointSalesDetails.INVNO, CounterPointSalesDetails.SEQ_NO, CounterPointSalesDetails.ITEM_NO, CounterPointSalesDetails.QUANTITY, CounterPointSalesDetails.QTY_UNIT, 
                                                     CounterPointSalesDetails.ITEM_DESCR, CounterPointSalesDetails.COST, ISNULL(CounterPointSalesDetails.UNIT_WEIGHT, 0) AS Expr1, 0 AS Expr2, InventoryItems.Id
                                    FROM    CounterPointSalesDetails INNER JOIN
                                                     EntryData ON CounterPointSalesDetails.INVNO COLLATE DATABASE_DEFAULT = EntryData.EntryDataId COLLATE DATABASE_DEFAULT AND CounterPointSalesDetails.DATE = EntryData.EntryDataDate INNER JOIN
                                                     InventoryItems ON CounterPointSalesDetails.INVNO COLLATE DATABASE_DEFAULT = InventoryItems.ItemNumber COLLATE DATABASE_DEFAULT AND 
                                                     EntryData.ApplicationSettingsId = InventoryItems.ApplicationSettingsId INNER JOIN
                                                     InventoryItemSource ON InventoryItems.Id = InventoryItemSource.InventoryId INNER JOIN
                                                     InventorySources ON InventoryItemSource.InventorySourceId = InventorySources.Id
                                    WHERE (CounterPointSalesDetails.INVNO = @INVNumber) AND (CounterPointSalesDetails.DATE = @Date) AND (LEFT(CounterPointSalesDetails.ITEM_NO, 1) <> '*') AND 
                                                     (EntryData.ApplicationSettingsId = @ApplicationSettingsId) AND (InventorySources.Name = N'POS')",
                        new SqlParameter("@AsycudaDocumentSetId", docSetId),
                        new SqlParameter("@INVNumber", c.InvoiceNo),
                        new SqlParameter("@Date", c.Date),
                        new SqlParameter("@ApplicationSettingsId",
                            BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId));
                }

                StatusModel.Timer("Refreshing Sales Data");


                StatusModel.StopStatusUpdate();
            }
        }

        public async Task DownloadCPSalesDateRange(DateTime startDate, DateTime endDate, int docSetId)
        {
            StatusModel.Timer("Downloading CP Sales Data...");
            using (var ctx = new WaterNutDBEntities {CommandTimeout = 0})
            {
                ctx.ExecuteStoreCommand(@"

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
													 EntryData ON CounterPointSales.INVNO = EntryData.EntryDataId COLLATE Latin1_General_CI_AS AND CounterPointSales.DATE = EntryData.EntryDataDate
									WHERE (CounterPointSales.DATE >= @StartDate) AND (CounterPointSales.DATE <= @EndDate)


                                    INSERT INTO InventoryItems
                                                      (ItemNumber, Description, ApplicationSettingsId)
                                    SELECT CounterPointSalesDetails.ITEM_NO, MAX(CounterPointSalesDetails.ITEM_DESCR) AS ITEM_DESCR, @ApplicationSettingsId AS Expr1
                                    FROM    InventoryItemSource INNER JOIN
                                                     InventoryItems AS InventoryItems_1 ON InventoryItemSource.InventoryId = InventoryItems_1.Id INNER JOIN
                                                     InventorySources ON InventoryItemSource.InventorySourceId = InventorySources.Id RIGHT OUTER JOIN
                                                     CounterPointSalesDetails ON InventoryItems_1.ItemNumber COLLATE DATABASE_DEFAULT = CounterPointSalesDetails.ITEM_NO COLLATE DATABASE_DEFAULT
                                    WHERE (CounterPointSalesDetails.DATE >= @StartDate) AND (CounterPointSalesDetails.DATE <= @EndDate) AND (InventoryItems_1.ItemNumber IS NULL) AND (InventorySources.Name = N'POS')
                                    GROUP BY CounterPointSalesDetails.ITEM_NO, InventoryItems_1.ApplicationSettingsId, InventorySources.Name
                                    HAVING (LEFT(CounterPointSalesDetails.ITEM_NO, 1) <> '*') AND (InventoryItems_1.ApplicationSettingsId = 5)

                                    insert into InventoryItemSource(InventoryId, InventorySourceId)
									SELECT        InventoryItems.Id AS InventoryItemId, InventorySources.Id AS InventorySourceId
									FROM            InventoryItems LEFT OUTER JOIN
															 InventoryItemSource ON InventoryItems.Id = InventoryItemSource.InventoryId CROSS JOIN
															 InventorySources
									WHERE        (InventorySources.Name = N'POS') AND (InventoryItemSource.Id IS NULL)


                                    INSERT INTO EntryDataDetails
                                                      (EntryData_Id,EntryDataId, LineNumber, ItemNumber, Quantity, Units, ItemDescription, Cost, UnitWeight, QtyAllocated, InventoryItemId)
                                    SELECT EntryData.EntryData_Id, CounterPointSalesDetails.INVNO, CounterPointSalesDetails.SEQ_NO, CounterPointSalesDetails.ITEM_NO, CounterPointSalesDetails.QUANTITY, CounterPointSalesDetails.QTY_UNIT, 
													 CounterPointSalesDetails.ITEM_DESCR, CounterPointSalesDetails.COST, ISNULL(CounterPointSalesDetails.UNIT_WEIGHT, 0) AS Expr1, 0 AS Expr2, InventoryItems.Id
									FROM    CounterPointSalesDetails INNER JOIN
													 EntryData ON CounterPointSalesDetails.INVNO COLLATE DATABASE_DEFAULT = EntryData.EntryDataId COLLATE DATABASE_DEFAULT AND CounterPointSalesDetails.DATE = EntryData.EntryDataDate INNER JOIN
													 InventoryItems ON EntryData.ApplicationSettingsId = InventoryItems.ApplicationSettingsId AND 
													 CounterPointSalesDetails.ITEM_NO COLLATE DATABASE_DEFAULT = InventoryItems.ItemNumber COLLATE DATABASE_DEFAULT INNER JOIN
													 InventoryItemSource ON InventoryItems.Id = InventoryItemSource.InventoryId INNER JOIN
													 InventorySources ON InventoryItemSource.InventorySourceId = InventorySources.Id
									WHERE (CounterPointSalesDetails.DATE >= @StartDate) AND (CounterPointSalesDetails.DATE <= @EndDate) AND (LEFT(CounterPointSalesDetails.ITEM_NO, 1) <> '*') AND (CounterPointSalesDetails.ITEM_DESCR IS NOT NULL) 
													 AND (InventorySources.Name = N'POS')",
                    new SqlParameter("@AsycudaDocumentSetId", docSetId),
                    new SqlParameter("@StartDate", startDate),
                    new SqlParameter("@EndDate", endDate),
                    new SqlParameter("@ApplicationSettingsId",
                        BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId));
            }

            StatusModel.StopStatusUpdate();
        }
    }
}