
using System;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Core.Common.Contracts;
using CounterPointQS.Client.DTO;

namespace CounterPointQS.Client.Contracts
{
    
    public partial interface ICounterPointSalesService
    {
        [OperationContract]
        Task DownloadCPSales(CounterPointSales counterPointSales, int p);

        [OperationContract]
        Task DownloadCPSalesDateRange(DateTime startDate, DateTime endDate, int docSetId);

    }
}

