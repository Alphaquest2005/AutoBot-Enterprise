
using System.Threading.Tasks;
using CounterPointQS.Business.Entities;
using Serilog;

namespace CounterPointQS.Business.Services
{

    public partial class CounterPointPOsService
    {
        
        public async Task DownloadCPO(CounterPointPOs c, int asycudaDocumentSetId)
        {
            var logger = Log.ForContext<CounterPointPOsService>();
            await
                WaterNut.DataSpace.CPPurchaseOrdersModel.Instance.DownloadCPO(c, asycudaDocumentSetId, logger)
                    .ConfigureAwait(false);
        }
    }
}



