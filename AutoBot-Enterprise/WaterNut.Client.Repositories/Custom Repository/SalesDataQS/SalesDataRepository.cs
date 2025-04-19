
using System.Collections.Generic;
using System.Linq;
using CoreEntities.Client.Entities;


using System.Threading.Tasks;
using System;
using SalesDataQS.Client.Services;


namespace SalesDataQS.Client.Repositories 
{
   
    public partial class SalesDataRepository
    {
        public async Task DownloadQBData(DateTime startDate, DateTime endDate, bool importSales, bool importInventory,
            int docSetId)
        {
            using (var t = new SalesDataClient())
            {
                await t.DownloadQBData(startDate, endDate, importSales, importInventory, docSetId).ConfigureAwait(false);
            }
        }

        public async Task<IEnumerable<AsycudaDocument>> GetSalesDocuments(int docSetId)
        {
            using (var t = new SalesDataClient())
            {
                return (await t.GetSalesDocuments(docSetId).ConfigureAwait(false)).Select(x => new AsycudaDocument(x));
            }

        }
     
       
    }
}

