

using System.Threading.Tasks;
using EntryDataDS.Business.Entities;
using Omu.ValueInjecter;
using SalesDataQS.Business.Entities;
using TrackableEntities;

namespace SalesDataQS.Business.Services
{
   
    public partial class SalesDataDetailService
    {
        public async Task SaveSalesDataDetail(SalesDataDetail salesDataDetail)
        {
            salesDataDetail.ModifiedProperties = null;
            using (var ctx = new EntryDataDS.Business.Services.EntryDataDetailsService() { StartTracking = true })
            {
                var ed =
                    await
                        ctx.GetEntryDataDetailsByKey(salesDataDetail.EntryDataDetailsId.ToString())
                            .ConfigureAwait(false) ?? new EntryDataDetails() { TrackingState = TrackingState.Added };

                ed.InjectFrom(salesDataDetail);
                
                ed.ModifiedProperties = null;
                await ctx.UpdateEntryDataDetails(ed).ConfigureAwait(false);
            }
        }
    }
}



