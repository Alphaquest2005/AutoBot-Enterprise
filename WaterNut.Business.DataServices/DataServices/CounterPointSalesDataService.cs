using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CounterPointQS.Business.Contracts;

namespace WaterNut.Business.DataServices.DataServices
{
    public class CounterPointSalesDataService : ICounterPointSalesService, IDisposable
    {
        #region ICounterPointSalesService Members

        public async Task DownloadCPSales(CounterPointQS.Business.Entities.CounterPointSales counterPointSales, int docSetId)
        {
            await DataSpace.CPSalesModel.Instance.DownloadCPSales(counterPointSales, docSetId).ConfigureAwait(false);
        }

        public async Task DownloadCPSalesDateRange(DateTime startDate, DateTime endDate, int docSetId)
        {
            await
                DataSpace.CPSalesModel.Instance.DownloadCPSalesDateRange(startDate, endDate, docSetId)
                    .ConfigureAwait(false);
        }

        #endregion

        #region IDisposable Members

        public void Dispose()
        {
            //throw new NotImplementedException();
        }

        #endregion
    }
}
