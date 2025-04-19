
using System;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Core.Common.Contracts;
using CoreEntities.Client.DTO;
using SalesDataQS.Client.DTO;

namespace SalesDataQS.Client.Contracts
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

