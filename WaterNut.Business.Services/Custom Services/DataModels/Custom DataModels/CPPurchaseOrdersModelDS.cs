using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using Core.Common.UI;
using CounterPointQS.Business.Entities;
using WaterNut.DataLayer;
using Serilog;

namespace WaterNut.DataSpace
{
    public class CPPurchaseOrdersModel
    {
        static CPPurchaseOrdersModel()
        {
            Instance = new CPPurchaseOrdersModel();
        }

        public static CPPurchaseOrdersModel Instance { get; }

        public async Task DownloadCPO(CounterPointPOs c, int asycudaDocumentSetId, ILogger logger)
        {
            if (c == null) return;
            if (asycudaDocumentSetId != 0)
            {

                var sysDocSet = await EntryDocSetUtils.GetAsycudaDocumentSet(logger, "Purchase Orders", true).ConfigureAwait(false);
                if (sysDocSet == null) throw new ApplicationException("No System Docset for 'Purchase Orders'");


                StatusModel.Timer("Downloading CP Data...");
                using (var ctx = new WaterNutDBEntities { CommandTimeout = 0 })
                {
                    await ctx.ExecuteStoreCommandAsync(@"

                                    Declare @entryData_Id int
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
                                   SELECT EntryData.EntryData_Id, @AsycudaDocumentSetId AS Expr1
									FROM    CounterPointPOs INNER JOIN
													 EntryData ON CounterPointPOs.PO_NO COLLATE Database_Default = EntryData.EntryDataId COLLATE Database_Default AND CounterPointPOs.DATE = EntryData.EntryDataDate
									WHERE (CounterPointPOs.PO_NO = @PONumber) AND (CounterPointPOs.DATE = @Date) and entrydata.ApplicationSettingsId = @ApplicationSettingsId


                                    INSERT INTO AsycudaDocumentSetEntryData
                                                      (EntryData_Id, AsycudaDocumentSetId)
                                   SELECT EntryData.EntryData_Id, @PoDocSetId AS Expr1
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
                                    SELECT DISTINCT CounterPointPODetails.ITEM_NO, CounterPointPODetails.ITEM_DESCR, EntryData.ApplicationSettingsId
                                    FROM    InventoryItemSource INNER JOIN
                                                     InventoryItems AS InventoryItems_1 ON InventoryItemSource.InventoryId = InventoryItems_1.Id INNER JOIN
                                                     InventorySources ON InventoryItemSource.InventorySourceId = InventorySources.Id RIGHT OUTER JOIN
                                                     CounterPointPODetails INNER JOIN
                                                     EntryData ON CounterPointPODetails.PO_NO COLLATE Database_Default = EntryData.EntryDataId COLLATE Database_Default ON InventoryItems_1.ApplicationSettingsId = EntryData.ApplicationSettingsId AND 
                                                     InventoryItems_1.ItemNumber = CounterPointPODetails.ITEM_NO
                                    WHERE (CounterPointPODetails.PO_NO = @PONumber) AND (InventoryItems_1.ItemNumber IS NULL) AND (LEFT(CounterPointPODetails.ITEM_NO, 1) <> '*') AND (EntryData.ApplicationSettingsId = @applicationSettingsId) AND 
                                                     (EntryData.EntryDataDate = @Date)

									insert into InventoryItemSource(InventoryId, InventorySourceId)
									SELECT        InventoryItems.Id AS InventoryItemId, InventorySources.Id AS InventorySourceId
									FROM            InventoryItems LEFT OUTER JOIN
															 InventoryItemSource ON InventoryItems.Id = InventoryItemSource.InventoryId CROSS JOIN
															 InventorySources
									WHERE        (InventorySources.Name = N'POS') AND (InventoryItemSource.Id IS NULL)


                                    INSERT INTO EntryDataDetails
                                                      (EntryData_Id, EntryDataId, LineNumber, ItemNumber, Quantity, Units, ItemDescription, Cost, UnitWeight, QtyAllocated,InventoryItemId)
										SELECT        EntryData.EntryData_Id, CounterPointPODetails.PO_NO, CounterPointPODetails.SEQ_NO, CounterPointPODetails.ITEM_NO, CounterPointPODetails.ORD_QTY, CounterPointPODetails.ORD_UNIT, 
                                                             CounterPointPODetails.ITEM_DESCR, CounterPointPODetails.ORD_COST, CounterPointPODetails.UNIT_WEIGHT, 0 AS Expr1, Min(InventoryItems.Id) AS Id
                                    FROM            CounterPointPODetails INNER JOIN
                                                             EntryData ON CounterPointPODetails.PO_NO COLLATE DATABASE_DEFAULT = EntryData.EntryDataId COLLATE DATABASE_DEFAULT INNER JOIN
                                                             InventoryItems ON CounterPointPODetails.ITEM_NO COLLATE Database_Default = InventoryItems.ItemNumber COLLATE Database_Default AND EntryData.ApplicationSettingsId = InventoryItems.ApplicationSettingsId INNER JOIN
                                                             InventoryItemSource ON InventoryItems.Id = InventoryItemSource.InventoryId INNER JOIN
                                                             InventorySources ON InventoryItemSource.InventorySourceId = InventorySources.Id
                                    WHERE        (CounterPointPODetails.PO_NO = @PONumber) AND (LEFT(CounterPointPODetails.ITEM_NO, 1) <> '*') AND (EntryData.ApplicationSettingsId = @applicationSettingsId) AND (EntryData.EntryDataDate = @Date)
                                                           -- AND   (InventorySources.Name = N'POS') --- took out because same item can be in multiple sources
                                    GROUP BY EntryData.EntryData_Id, CounterPointPODetails.PO_NO, CounterPointPODetails.SEQ_NO, CounterPointPODetails.ITEM_NO, CounterPointPODetails.ORD_QTY, CounterPointPODetails.ORD_UNIT, 
                                                             CounterPointPODetails.ITEM_DESCR, CounterPointPODetails.ORD_COST, CounterPointPODetails.UNIT_WEIGHT
",
                            new SqlParameter("@AsycudaDocumentSetId", asycudaDocumentSetId),
                            new SqlParameter("@PoDocSetId", sysDocSet.AsycudaDocumentSetId),
                            new SqlParameter("@PONumber", c.PurchaseOrderNo),
                            new SqlParameter("@Date", c.Date),
                            new SqlParameter("@ApplicationSettingsId",
                                BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId))
                        .ConfigureAwait(false);
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