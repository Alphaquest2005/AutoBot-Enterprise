



using System.Threading.Tasks;
using System;
using CounterPointQS.Client.Services;


namespace CounterPointQS.Client.Repositories 
{
   
    public partial class CounterPointSalesRepository 
    {
        public async Task DownloadCPSales(Entities.CounterPointSales counterPointSales, int p)
        {
            using (var t = new CounterPointSalesClient())
            {
                await t.DownloadCPSales(counterPointSales.DTO, p).ConfigureAwait(false);
            }
        }

        public async Task DownloadCPSalesDateRange(DateTime startDate, DateTime endDate, int docSetId)
        {
            using (var t = new CounterPointSalesClient())
            {
                await t.DownloadCPSalesDateRange(startDate, endDate, docSetId).ConfigureAwait(false);
            }
        }
       
    }
}

