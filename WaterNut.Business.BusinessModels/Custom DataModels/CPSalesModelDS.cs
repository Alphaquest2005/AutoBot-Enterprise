using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

using CounterPointQS.Business.Entities;
using Core.Common.UI;
using WaterNut.DataLayer;


namespace WaterNut.DataSpace
{
	public class CPSalesModel  
	{
       private static readonly CPSalesModel instance;
       static CPSalesModel()
        {
            instance = new CPSalesModel();
            
        }

       public static CPSalesModel Instance
        {
            get { return instance; }
        }

	    public async Task DownloadCPSales(CounterPointSales c, int docSetId)
	    {
	        //WaterNutDBEntities db = BaseDataModel.db;//new WaterNutDBEntities(Properties.Settings.Default.WaterNutDBEntitiesConnection);
	        if (docSetId != 0)
	        {
	            StatusModel.Timer("Downloading CP Sales...");
	            using (var ctx = new WaterNutDBEntities() {CommandTimeout = 0})
	            {
	                await ctx.ExecuteStoreCommandAsync(@"
                            
                                    Delete from EntryData
                                    Where EntryDataId = @INVNumber

                                    Delete from EntryData_Sales
                                    Where EntryDataId = @INVNumber

                                    INSERT INTO EntryData
                                                      (EntryDataId, EntryDataDate)
                                    SELECT INVNO, DATE
                                    FROM     CounterPointSales
                                    WHERE  (INVNO = @INVNumber)

                                    INSERT INTO AsycudaDocumentSetEntryData
                                                      (EntryDataId, AsycudaDocumentSetId)
                                    SELECT INVNO, @AsycudaDocumentSetId
                                    FROM     CounterPointSales
                                    WHERE  (INVNO = @INVNumber)

                                    INSERT INTO EntryData_Sales
                                                      (EntryDataId, INVNumber, TaxAmount,CustomerName)
                                    SELECT distinct INVNO, INVNO AS Expr1,TAX_AMT, [CUSTOMER NAME]
                                    FROM     CounterPointSales
                                    WHERE  (INVNO = @INVNumber)

                                    INSERT INTO InventoryItems
                                                      (ItemNumber, Description)
                                    SELECT CounterPointSalesDetails.ITEM_NO, CounterPointSalesDetails.ITEM_DESCR
                                    FROM     CounterPointSalesDetails LEFT OUTER JOIN
                                                      InventoryItems AS InventoryItems_1 ON CounterPointSalesDetails.ITEM_NO = InventoryItems_1.ItemNumber
                                    WHERE  (CounterPointSalesDetails.INVNO = @INVNumber) AND (InventoryItems_1.ItemNumber IS NULL) AND Left(CounterPointSalesDetails.ITEM_NO,1) <>'*'

                                    INSERT INTO EntryDataDetails
                                                      (EntryDataId, LineNumber, ItemNumber, Quantity, Units, ItemDescription, Cost, UnitWeight, QtyAllocated)
                                    SELECT INVNO, SEQ_NO, ITEM_NO, QUANTITY, QTY_UNIT, ITEM_DESCR, COST, isnull(UNIT_WEIGHT,0), 0
                                    FROM     CounterPointSalesDetails
                                    WHERE  (INVNO = @INVNumber) AND Left(CounterPointSalesDetails.ITEM_NO,1) <>'*'",

	                    new SqlParameter("@AsycudaDocumentSetId", docSetId),
	                    new SqlParameter("@INVNumber", c.InvoiceNo)).ConfigureAwait(false);
	            }
	            StatusModel.Timer("Refreshing Sales Data");


	            StatusModel.StopStatusUpdate();

	        }
	    }

	    public async Task DownloadCPSalesDateRange(DateTime startDate, DateTime endDate, int docSetId)
        {
           
              
                    StatusModel.Timer("Downloading CP Sales Data...");
                    using (var ctx = new WaterNutDBEntities() { CommandTimeout = 0 })
                    {
                        await ctx.ExecuteStoreCommandAsync(@"

                                    DELETE From EntryData
                                    Where EntryDataId in (SELECT distinct INVNO
                                    FROM     CounterPointSales
                                    WHERE  (DATE >= @StartDate and DATE <= @EndDate))
            
                                    INSERT INTO EntryData
                                                      (EntryDataId, EntryDataDate)
                                    SELECT distinct INVNO, DATE
                                    FROM     CounterPointSales
                                    WHERE  (DATE >= @StartDate and DATE <= @EndDate)

                                    INSERT INTO AsycudaDocumentSetEntryData
                                                      (EntryDataId, AsycudaDocumentSetId)
                                    SELECT distinct INVNO, @AsycudaDocumentSetId
                                    FROM     CounterPointSales
                                    WHERE  (DATE >= @StartDate and DATE <= @EndDate)

                                    DELETE From EntryData_Sales
                                    Where EntryDataId in (SELECT distinct INVNO
                                    FROM     CounterPointSales
                                    WHERE  (DATE >= @StartDate and DATE <= @EndDate))

                                    INSERT INTO EntryData_Sales
                                                      (EntryDataId, INVNumber, TaxAmount,CustomerName)
                                    SELECT distinct INVNO, INVNO AS Expr1,TAX_AMT, [CUSTOMER NAME]
                                    FROM     CounterPointSales
                                    WHERE  (DATE >= @StartDate and DATE <= @EndDate)

                                    INSERT INTO InventoryItems
                                                      (ItemNumber, Description)
                                    SELECT CounterPointSalesDetails.ITEM_NO, MAX(CounterPointSalesDetails.ITEM_DESCR) AS ITEM_DESCR
                                    FROM     CounterPointSalesDetails LEFT OUTER JOIN
                                                      InventoryItems AS InventoryItems_1 ON CounterPointSalesDetails.ITEM_NO = InventoryItems_1.ItemNumber
                                    WHERE  (CounterPointSalesDetails.DATE >= @StartDate) AND (CounterPointSalesDetails.DATE <= @EndDate) AND (InventoryItems_1.ItemNumber IS NULL)
                                    GROUP BY CounterPointSalesDetails.ITEM_NO
                                    HAVING (LEFT(CounterPointSalesDetails.ITEM_NO, 1) <> '*')   

                                    INSERT INTO EntryDataDetails
                                                      (EntryDataId, LineNumber, ItemNumber, Quantity, Units, ItemDescription, Cost, UnitWeight, QtyAllocated)
                                    SELECT INVNO, SEQ_NO, ITEM_NO, QUANTITY, QTY_UNIT, ITEM_DESCR, COST, isnull(UNIT_WEIGHT,0),0
                                    FROM     CounterPointSalesDetails
                                    WHERE   (DATE >= @StartDate and DATE <= @EndDate) AND Left(CounterPointSalesDetails.ITEM_NO,1) <>'*'",
                                                                                                                                         
                                   new SqlParameter("@AsycudaDocumentSetId", docSetId),
                                   new SqlParameter("@StartDate", startDate),
                                   new SqlParameter("@EndDate", endDate)).ConfigureAwait(false);
                    }
             
                    StatusModel.StopStatusUpdate();
              

           
        }


        
    }
}