
using System.Threading.Tasks;
using CounterPointQS.Business.Entities;

namespace CounterPointQS.Business.Services
{

    public partial class CounterPointPOsService
    {
        
        public async Task DownloadCPO(CounterPointPOs c, int asycudaDocumentSetId)
        {

            await
                WaterNut.DataSpace.CPPurchaseOrdersModel.Instance.DownloadCPO(c, asycudaDocumentSetId)
                    .ConfigureAwait(false);
        }
    }
}



