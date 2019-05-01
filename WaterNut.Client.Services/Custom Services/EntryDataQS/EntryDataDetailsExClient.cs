


using System.Threading.Tasks;
using EntryDataQS.Client.DTO;

namespace EntryDataQS.Client.Services
{
   
    public partial class EntryDataDetailsExClient 
    {

        public async Task SaveEntryDataDetailsEX(EntryDataDetailsEx entryDataDetailsEx)
        {
            await Channel.SaveEntryDataDetailsEX(entryDataDetailsEx).ConfigureAwait(false);
        }
         
    }
}

