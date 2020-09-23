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
													 InventoryItems AS InventoryItems_1 ON CounterPointPODetails.ITEM_NO  COLLATE Database_Default = InventoryItems_1.ItemNumber  COLLATE Database_Default
									WHERE (CounterPointPODetails.PO_NO = @PONumber) AND (InventoryItems_1.ItemNumber IS NULL) AND (LEFT(CounterPointPODetails.ITEM_NO, 1) <> '*') AND (EntryData.ApplicationSettingsId = @applicationSettingsId) AND 
													 (EntryData.EntryDataDate = @Date)

                                    INSERT INTO EntryDataDetails
                                                      (EntryData_Id, EntryDataId, LineNumber, ItemNumber, Quantity, Units, ItemDescription, Cost, UnitWeight, QtyAllocated)
                                    SELECT EntryData.EntryData_Id, CounterPointPODetails.PO_NO, CounterPointPODetails.SEQ_NO, CounterPointPODetails.ITEM_NO, CounterPointPODetails.ORD_QTY, CounterPointPODetails.ORD_UNIT, 
													 CounterPointPODetails.ITEM_DESCR, CounterPointPODetails.ORD_COST, CounterPointPODetails.UNIT_WEIGHT, 0 AS Expr1
									FROM    CounterPointPODetails INNER JOIN
													 EntryData ON CounterPointPODetails.PO_NO = EntryData.EntryDataId COLLATE Latin1_General_CI_AS
									WHERE (CounterPointPODetails.PO_NO = @PONumber) AND (LEFT(CounterPointPODetails.ITEM_NO, 1) <> '*') AND (EntryData.ApplicationSettingsId = @applicationsettingsId) AND (EntryData.EntryDataDate = @Date)", 
                                                                                                               
                                                                                                               
                                   new SqlParameter("@AsycudaDocumentSetId", asycudaDocumentSetId),
                                   new SqlParameter("@PONumber", c.PurchaseOrderNo),
                                    new SqlParameter("@Date", c.Date),
                                    new SqlParameter("@ApplicationSettingsId", BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)).ConfigureAwait(false);
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