using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using CounterPointQS.Business.Entities;
using Core.Common.UI;
using WaterNut.DataLayer;


namespace WaterNut.DataSpace
{
	public class CPPurchaseOrdersModel 
	{
        private static readonly CPPurchaseOrdersModel instance;
        static CPPurchaseOrdersModel()
        {
            instance = new CPPurchaseOrdersModel();
            
        }

        public static CPPurchaseOrdersModel Instance
        {
            get { return instance; }
        }

        public async Task DownloadCPO(CounterPointPOs c, int asycudaDocumentSetId)
        {
           if (c == null) return;
           if (asycudaDocumentSetId != 0)
            {
         
                StatusModel.Timer("Downloading CP Data...");
                using (var ctx = new WaterNutDBEntities() { CommandTimeout = 0 })
                {
                   await ctx.ExecuteStoreCommandAsync(@"

                                    Delete from EntryData
                                    Where EntryDataId = @PONumber

                                    Delete from EntryData_PurchaseOrders
                                    Where EntryDataId = @PONumber

                                    INSERT INTO EntryData
                                                      (EntryDataId, EntryDataDate)
                                    SELECT PO_NO, DATE
                                    FROM     CounterPointPOs
                                    WHERE  (PO_NO = @PONumber)

                                    INSERT INTO AsycudaDocumentSetEntryData
                                                      (EntryDataId, AsycudaDocumentSetId)
                                    SELECT PO_NO, @AsycudaDocumentSetId
                                    FROM     CounterPointPOs
                                    WHERE  (PO_NO = @PONumber)

                                    INSERT INTO EntryData_PurchaseOrders
                                                      (EntryDataId, PONumber)
                                    SELECT distinct PO_NO, PO_NO AS Expr1
                                    FROM     CounterPointPOs
                                    WHERE  (PO_NO = @PONumber)

                                    INSERT INTO InventoryItems
                                                      (ItemNumber, Description)
                                    SELECT Distinct CounterPointPODetails.ITEM_NO, CounterPointPODetails.ITEM_DESCR
                                    FROM     CounterPointPODetails LEFT OUTER JOIN
                                                      InventoryItems AS InventoryItems_1 ON CounterPointPODetails.ITEM_NO = InventoryItems_1.ItemNumber
                                    WHERE  (CounterPointPODetails.PO_NO = @PONumber) AND (InventoryItems_1.ItemNumber IS NULL) AND Left(CounterPointPODetails.ITEM_NO,1) <>'*'

                                    INSERT INTO EntryDataDetails
                                                      (EntryDataId, LineNumber, ItemNumber, Quantity, Units, ItemDescription, Cost, UnitWeight, QtyAllocated)
                                    SELECT PO_NO, SEQ_NO, ITEM_NO, ORD_QTY, ORD_UNIT, ITEM_DESCR, ORD_COST, UNIT_WEIGHT, 0
                                    FROM     CounterPointPODetails
                                    WHERE  (PO_NO = @PONumber) AND Left(CounterPointPODetails.ITEM_NO,1) <>'*'", 
                                                                                                               
                                                                                                               
                                   new SqlParameter("@AsycudaDocumentSetId", asycudaDocumentSetId), new SqlParameter("@PONumber", c.PurchaseOrderNo)).ConfigureAwait(false);
                }
            StatusModel.Timer("Refreshing CP Data...");

           

           StatusModel.StopStatusUpdate();
            }
            else
            {
                throw new ApplicationException("Please Select a Asycuda Document Set before downloading PO");
            }
        }

      
    }
}