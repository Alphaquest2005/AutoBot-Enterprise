using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CounterPointQS.Business.Contracts;

namespace WaterNut.Business.DataServices
{
    public class CounterPointPOsDataService : ICounterPointPOsService, IDisposable
    {

        #region ICounterPointPOsService Members

        public async Task DownloadCPO(CounterPointQS.Business.Entities.CounterPointPOs c, int asycudaDocumentSetId)
        {
            await DataSpace.CPPurchaseOrdersModel.Instance.DownloadCPO(c, asycudaDocumentSetId).ConfigureAwait(false);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            
        }

        #endregion
    }
}
