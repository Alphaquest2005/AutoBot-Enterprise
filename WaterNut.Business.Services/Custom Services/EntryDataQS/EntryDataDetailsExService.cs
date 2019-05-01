

using System.Threading.Tasks;
using EntryDataDS.Business.Entities;
using Omu.ValueInjecter;
using TrackableEntities;
using EntryDataDetailsEx = EntryDataQS.Business.Entities.EntryDataDetailsEx;

namespace EntryDataQS.Business.Services
{

    public partial class EntryDataDetailsExService
    {
        public async Task SaveEntryDataDetailsEX(EntryDataDetailsEx entryDataDetailsEx)
        {
            entryDataDetailsEx.ModifiedProperties = null;
            using (var ctx = new EntryDataDS.Business.Services.EntryDataDetailsService() {StartTracking = true})
            {
                var ed =
                    await
                        ctx.GetEntryDataDetailsByKey(entryDataDetailsEx.EntryDataDetailsId.ToString())
                            .ConfigureAwait(false) ?? new EntryDataDetails() {TrackingState = TrackingState.Added};

                ed.InjectFrom(entryDataDetailsEx);


                ed.ModifiedProperties = null;
                await ctx.UpdateEntryDataDetails(ed).ConfigureAwait(false);
            }
        }
    }
}



