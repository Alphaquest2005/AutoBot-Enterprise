
using System;
using System.Threading.Tasks;
using CounterPointQS.Business.Entities;

namespace CounterPointQS.Business.Services
{

    public partial class CounterPointSalesService
    {
        public async Task DownloadCPSales(CounterPointSales counterPointSales, int p)
        {
            await WaterNut.DataSpace.CPSalesModel.Instance.DownloadCPSales(counterPointSales, p).ConfigureAwait(false);
        }

        public async Task DownloadCPSalesDateRange(DateTime startDate, DateTime endDate, int docSetId)
        {
            await
                WaterNut.DataSpace.CPSalesModel.Instance.DownloadCPSalesDateRange(startDate, endDate, docSetId)
                    .ConfigureAwait(false);
        }
    }
}



