using System;
using System.Collections.Generic;
using System.ComponentModel.Composition;
using System.Linq;
using System.Runtime.InteropServices;
using System.ServiceModel;
using System.ServiceModel.Configuration;
using System.Text;
using System.Threading.Tasks;
using AllocationDS.Business.Entities;
using AllocationDS.Business.Services;
using AllocationQS.Business.Contracts;
using Core.Common.Contracts;
using DocumentDS.Business.Entities;

namespace WaterNut.Business.DataServices
{
    [Export(typeof(IAsycudaSalesAllocationsExService))]
    [Export(typeof(IBusinessService))]
    [PartCreationPolicy(CreationPolicy.NonShared)]
    [ServiceBehavior(InstanceContextMode = InstanceContextMode.PerCall,
                     ConcurrencyMode = ConcurrencyMode.Multiple)]
    public class AsycudaSalesAllocationsExDataService:  IAsycudaSalesAllocationsExService, IDisposable
    {
        #region IDisposable Members

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        #endregion

        #region IAsycudaSalesAllocationsExService Members

        public async Task CreateEx9(string filterExpression, bool perIM7, bool applyEx9Bucket, bool breakOnMonthYear, int AsycudaDocumentSetId)
        {
            var docset = await DataSpace.BaseDataModel.Instance.GetAsycudaDocumentSet(AsycudaDocumentSetId, null).ConfigureAwait(false);
            await DataSpace.CreateEx9Class.Instance.CreateEx9(filterExpression, perIM7, applyEx9Bucket, breakOnMonthYear,
                docset).ConfigureAwait(false);
        }

        public async Task CreateOPS(string filterExpression, int AsycudaDocumentSetId)
        {
            var docset = await DataSpace.BaseDataModel.Instance.GetAsycudaDocumentSet(AsycudaDocumentSetId, null).ConfigureAwait(false);
            await DataSpace.CreateOPSClass.Instance.CreateOPS(filterExpression, docset).ConfigureAwait(false);
        }

        public async Task ManuallyAllocate(int AllocationId, int PreviousItem_Id)
        {
            var allo =
                await DataSpace.AllocationDS.DataModels.BaseDataModel.Instance.SearchAsycudaSalesAllocations(
                    new List<string>()
                    {
                        string.Format("AllocationId == {0}", AllocationId)
                    }, null).ConfigureAwait(false);

            var pitm =
                await
                    DataSpace.AllocationDS.DataModels.BaseDataModel.Instance.Searchxcuda_Item(
                        new List<string>() { string.Format("Item_Id == {0}", PreviousItem_Id) }, null).ConfigureAwait(false);

            await DataSpace.AllocationsModel.Instance.ManuallyAllocate(allo.FirstOrDefault(), pitm.FirstOrDefault());
        }

       

        #endregion

        #region IAsycudaSalesAllocationsExService Members


        public async Task ClearAllocations(IEnumerable<int> alst)
        {
            var allos = new List<AsycudaSalesAllocations>();
            using (var ctx = new AsycudaSalesAllocationsService())
            {
                foreach (var aid in alst)
                {
                     allos.Add(await ctx.GetAsycudaSalesAllocationsByKey(aid.ToString(), new List<string>()
                     {
                         "EntryDataDetails",
                         "PreviousDocumentItem"
                     }).ConfigureAwait(false));
                }
               
            }

            await DataSpace.AllocationsModel.Instance.ClearAllocations(allos).ConfigureAwait(false);
        }

        public async Task ClearAllocations(string filterExpression)
        {
            await DataSpace.AllocationsModel.Instance.ClearAllocations(filterExpression).ConfigureAwait(false);
        }

        public async Task CreateIncompOPS(string filterExpression, int AsycudaDocumentSetId)
        {
            var docset = await DataSpace.BaseDataModel.Instance.GetAsycudaDocumentSet(AsycudaDocumentSetId, null).ConfigureAwait(false);
            await DataSpace.CreateIncompOPSClass.Instance.CreateIncompOPS(filterExpression, docset);
        }

        public async Task AllocateSales(bool itemDescriptionContainsAsycudaAttribute)
        {
            await
                DataSpace.AllocationsBaseModel.Instance.AllocateSales(itemDescriptionContainsAsycudaAttribute)
                    .ConfigureAwait(false);
        }

        #endregion
    }
}
