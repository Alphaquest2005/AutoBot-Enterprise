
using System.Threading.Tasks;
using EntryDataQS.Client.Entities;
using EntryDataQS.Client.Services;

namespace EntryDataQS.Client.Repositories 
{
   
    public partial class EntryDataDetailsExRepository
    {

        public async Task SaveEntryDataDetailsEX(EntryDataDetailsEx entryDataDetailsEx)
        {
            using (var ctx = new EntryDataDetailsExClient())
            {
                await ctx.SaveEntryDataDetailsEX(entryDataDetailsEx.DTO).ConfigureAwait(false);
            }
        }
                         
    }
}

