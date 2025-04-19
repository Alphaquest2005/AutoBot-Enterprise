using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using AllocationDS.Business.Entities;
using WaterNut.DataSpace;
using CustomsOperations = CoreEntities.Business.Enums.CustomsOperations;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.GettingXcudaItems
{
    public class GetXcudaItemsDbQuery : IGetXcudaItemsProcessor
    {
        public  List<xcuda_Item> Execute(int? asycudaDocumentSetId, List<int> xlst)
        {
            List<xcuda_Item> lst;
            using (var ctx = new AllocationDSContext { StartTracking = false })
            {
                ctx.Database.CommandTimeout = 0;
                ctx.Configuration.ValidateOnSaveEnabled = false;
                ctx.Configuration.AutoDetectChangesEnabled = false;

                var res = ctx.AsycudaSalesAllocations_XcudaItemsToAllocate.AsNoTracking()
                    .Join(xlst, a => a.Item_Id, i => i, (a, i) => a)
                    .Where(x => asycudaDocumentSetId == null ||
                                x.AsycudaDocumentSetId == asycudaDocumentSetId)
                    .Where(x => x.ApplicationSettingsId ==
                                WaterNut.DataSpace.BaseDataModel.Instance.CurrentApplicationSettings.ApplicationSettingsId)
                               .Where(x => x.AssessmentDate >=
                                           (WaterNut.DataSpace.BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate))
                    .Select(x => x.Item_Id)
                    .ToList();


                lst = ctx.xcuda_Item
                    .Join(res, x => x.Item_Id, i => i, (x, i) => x) //
                    /// ---late join causing navigation properties not to load
                    .Include("AsycudaDocument.Customs_Procedure")
                    .Include("xcuda_Tarification.xcuda_HScode")
                    .Include("EntryPreviousItems.xcuda_PreviousItem.xcuda_Item.AsycudaDocument")
                    .Include("xcuda_Tarification.xcuda_Supplementary_unit")
                    .Include("SubItems")
                    .Include("EntryPreviousItems.xcuda_PreviousItem")
                    //.Where(x => xlst.Contains(x.Item_Id))
                    //.Where(x => x.AsycudaDocument.ApplicationSettingsId == applicationSettingsId)
         //           .Where(x => asycudaDocumentSetId == null ||
         //                       x.AsycudaDocument.AsycudaDocumentSetId == asycudaDocumentSetId)
         //           .Where(x => (x.AsycudaDocument.CNumber != null || x.AsycudaDocument.IsManuallyAssessed == true)
         //                       && ( /*x.AsycudaDocument.CustomsOperationId == (int)CustomsOperations.Import
								 //||*/ x.AsycudaDocument.CustomsOperationId == (int)CustomsOperations.Warehouse)
         //                       && (x.AsycudaDocument.Customs_Procedure.Sales == true ||
         //                           x.AsycudaDocument.Customs_Procedure.Stock == true) &&
         //                       (x.AsycudaDocument.Cancelled == null || x.AsycudaDocument.Cancelled == false) &&
         //                       x.AsycudaDocument.DoNotAllocate != true)
         //           .Where(x => x.AsycudaDocument.AssessmentDate >=
         //                       (BaseDataModel.Instance.CurrentApplicationSettings.OpeningStockDate))
                    .OrderBy(x => x.LineNumber)
                    .ToList();
            }
            
            
            return lst;
        }
    }
}