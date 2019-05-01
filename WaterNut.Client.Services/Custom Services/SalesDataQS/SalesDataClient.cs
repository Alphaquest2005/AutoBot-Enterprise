
using System;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using CoreEntities.Client.DTO;
using SalesDataQS.Client.DTO;
using SalesDataQS.Client.Contracts;
using Core.Common.Client.Services;

using Core.Common.Contracts;
using System.ComponentModel.Composition;


namespace SalesDataQS.Client.Services
{

    public partial class SalesDataClient
    {
        public async Task DownloadQBData(DateTime startDate, DateTime endDate, bool importSales, bool importInventory,
            int docSetId)
        {
            await Channel.DownloadQBData(startDate, endDate, importSales, importInventory, docSetId).ConfigureAwait(false);
        }

        public async Task<IEnumerable<AsycudaDocument>> GetSalesDocuments(int docSetId)
        {
           return await Channel.GetSalesDocuments(docSetId).ConfigureAwait(false);
        }
    }
}

