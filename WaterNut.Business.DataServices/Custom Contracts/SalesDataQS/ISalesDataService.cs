
using System;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Core.Common.Contracts;
using CoreEntities.Business.Entities;
using SalesDataQS.Business.Entities;

namespace SalesDataQS.Business.Contracts
{
    
    public partial interface ISalesDataService
    {
        [OperationContract]
        Task DownloadQBData(DateTime startDate, DateTime endDate, bool importSales, bool importInventory,
            AsycudaDocumentSetEx docSet);

        [OperationContract]
        Task<IEnumerable<AsycudaDocument>> GetSalesDocuments(int docSetId);
    }
}

