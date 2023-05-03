
using System;
using System.ComponentModel.Composition;
using System.Linq;
using System.Collections.Generic;
using System.Data.Entity;
using System.ServiceModel;
using System.Threading.Tasks;
using AllocationDS.Business.Entities;
using Core.Common.Contracts;
using CoreEntities.Business.Entities;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.Asycuda.AllocatingSales;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.CreatingEx9;
using WaterNut.DataSpace;
using AsycudaDocument = AllocationDS.Business.Entities.AsycudaDocument;
using ConcurrencyMode = System.ServiceModel.ConcurrencyMode;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels;


namespace AllocationQS.Business.Services
{
    [Export(typeof(IAsycudaSalesAllocationsExService))]
    [Export(typeof(IBusinessService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerSession,
                     ConcurrencyMode = ConcurrencyMode.Multiple)]
    public partial class AllocationsService : IAllocationsService, IDisposable
   {
       
       

        public async Task CreateEx9(string filterExpression, bool perIM7, bool stressTest, bool process7100, bool applyCurrentChecks,
            int AsycudaDocumentSetId, string documentType, string ex9BucketType, bool isGrouped, bool checkQtyAllocatedGreaterThanPiQuantity, bool checkForMultipleMonths, bool applyEx9Bucket, bool applyHistoricChecks, bool perInvoice, bool autoAssess, bool overPIcheck, bool universalPIcheck, bool itemPIcheck)
        {
            var docset =
                await WaterNut.DataSpace.BaseDataModel.Instance.GetAsycudaDocumentSet(AsycudaDocumentSetId)
                    .ConfigureAwait(false);
           var docs =  await AllocationsModel.Instance.CreateEx9.Execute(filterExpression, perIM7, process7100, applyCurrentChecks, docset, documentType, ex9BucketType, isGrouped, checkQtyAllocatedGreaterThanPiQuantity, checkForMultipleMonths,
                    applyEx9Bucket, applyHistoricChecks, perInvoice, autoAssess, overPIcheck, universalPIcheck, itemPIcheck).ConfigureAwait(false);
            
           if (stressTest) await new BondStressTest().Execute(docs).ConfigureAwait(false);
        }

        public async Task CreateOPS(string filterExpression, int AsycudaDocumentSetId, bool perInvoice)
        {
            var docset =
               await WaterNut.DataSpace.BaseDataModel.Instance.GetAsycudaDocumentSet(AsycudaDocumentSetId)
                   .ConfigureAwait(false);

            //await WaterNut.DataSpace.CreateOPSClass.Instance.CreateOPS(filterExpression, docset).ConfigureAwait(false);
            await WaterNut.DataSpace.CreateErrOPS.Instance.CreateErrorOPS(filterExpression, docset, perInvoice).ConfigureAwait(false);

        }

        public async Task ManuallyAllocate(int AllocationId, int PreviousItem_Id)
        {
            xcuda_Item pitm;
            AsycudaSalesAllocations allo;
            using (var ctx = new AllocationDSContext())
            {
                pitm = ctx.xcuda_Item.FirstOrDefault(x => x.Item_Id == PreviousItem_Id);
                allo = ctx.AsycudaSalesAllocations.FirstOrDefault(x => x.AllocationId == AllocationId);
            }
            
            await WaterNut.DataSpace.AllocationsModel.Instance.ManuallyAllocate(allo, pitm).ConfigureAwait(false);
        }

        public async Task ClearAllocations(IEnumerable<int> alst)
        {
            var allst = new List<AsycudaSalesAllocations>();
            using (var ctx = new AllocationDSContext())
            {
                allst.AddRange(alst.Select(aid => ctx.AsycudaSalesAllocations
                                .Include(x => x.EntryDataDetails)
                                .Include(x => x.PreviousDocumentItem)
                                .FirstOrDefault(x => x.AllocationId == aid)));
            }
            await WaterNut.DataSpace.AllocationsModel.Instance.ClearAllocations(allst).ConfigureAwait(false);
        }

       public async Task ClearAllAllocations(int appSettings)
       {
           await WaterNut.DataSpace.AllocationsModel.Instance.ClearAllAllocations(appSettings).ConfigureAwait(false);

       }

       public async Task ClearAllocationsByFilter(string filterExpression)
        {
            await WaterNut.DataSpace.AllocationsModel.Instance.ClearAllocations(filterExpression).ConfigureAwait(false);
        }

        //public async Task CreateIncompOPS(string filterExpression, int AsycudaDocumentSetId)
        //{
        //    var docset =
        //       await WaterNut.DataSpace.BaseDataModel.Instance.GetAsycudaDocumentSet(AsycudaDocumentSetId, null)
        //           .ConfigureAwait(false);
        //    await
        //        WaterNut.DataSpace.CreateIncompOPSClass.Instance.CreateIncompOPS(filterExpression, docset)
        //            .ConfigureAwait(false);
        //}

        public async Task AllocateSales(ApplicationSettings applicationSettings, bool allocateToLastAdjustment)
        {
            await
               new AllocateSales().Execute(applicationSettings, allocateToLastAdjustment)
                    .ConfigureAwait(false);
        }

        public Task ReBuildSalesReports()
        {
           return Task.Run(() => WaterNut.DataSpace.BuildSalesReportClass.Instance.ReBuildSalesReports()) ;
        }

        public async Task ReBuildSalesReports(int asycuda_id)
        {
            AsycudaDocument doc = null;
            using (var ctx = new AllocationDSContext())
            {
                doc = ctx.AsycudaDocument.FirstOrDefault(x => x.ASYCUDA_Id == asycuda_id);
            }
            await WaterNut.DataSpace.BuildSalesReportClass.Instance.ReBuildSalesReports(doc).ConfigureAwait(false);
        }

        #region IDisposable Members

        public void Dispose()
        {
           // throw new NotImplementedException();
        }

        #endregion
   }
}

