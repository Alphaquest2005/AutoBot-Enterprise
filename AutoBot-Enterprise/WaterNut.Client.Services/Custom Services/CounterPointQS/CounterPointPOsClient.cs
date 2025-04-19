


using System.Threading.Tasks;
using CounterPointQS.Client.DTO;

namespace CounterPointQS.Client.Services
{
  
    public partial class CounterPointPOsClient
    {
        public async Task DownloadCPO(CounterPointPOs c, int asycudaDocumentSetId)
        {
            await Channel.DownloadCPO(c, asycudaDocumentSetId).ConfigureAwait(false);
        }
    }
}

