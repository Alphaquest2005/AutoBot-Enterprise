


using System;
using System.Threading.Tasks;
using CounterPointQS.Client.DTO;

namespace CounterPointQS.Client.Services
{

    public partial class CounterPointSalesClient 
    {
        public async Task DownloadCPSales(CounterPointSales counterPointSales, int p)
        {
            await Channel.DownloadCPSales(counterPointSales, p).ConfigureAwait(false);
        }

        public async Task DownloadCPSalesDateRange(DateTime startDate, DateTime endDate, int docSetId)
        {
            await Channel.DownloadCPSalesDateRange(startDate, endDate, docSetId).ConfigureAwait(false);
        }
    }
}

