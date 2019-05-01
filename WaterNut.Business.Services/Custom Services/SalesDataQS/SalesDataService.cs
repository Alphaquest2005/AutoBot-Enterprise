

using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using CoreEntities.Business.Entities;

namespace SalesDataQS.Business.Services
{

    public partial class SalesDataService 
    {
        public async Task DownloadQBData(DateTime startDate, DateTime endDate, bool importSales, bool importInventory, int docSetId)
        {
            var docSet = await WaterNut.DataSpace.BaseDataModel.Instance.GetAsycudaDocumentSet(docSetId, null).ConfigureAwait(false);
            //await
            //    QuickBooks.QBPOS.DownloadQbData(startDate, endDate,docSet, importSales, importInventory)
            //        .ConfigureAwait(false);
        }

        public async Task<IEnumerable<AsycudaDocument>> GetSalesDocuments(int docSetId)
        {
            return await WaterNut.DataSpace.SalesReportModel.Instance.GetSalesDocuments(docSetId).ConfigureAwait(false);
        }
    }
}



