
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using CoreEntities.Business.Entities;

namespace SalesDataQS.Business.Services
{
    
    public partial interface ISalesDataService
    {
        [OperationContract]
        Task DownloadQBData(DateTime startDate, DateTime endDate, bool importSales, bool importInventory,
            int docSetId);

        [OperationContract]
        Task<IEnumerable<AsycudaDocument>> GetSalesDocuments(int docSetId);
    }
}

