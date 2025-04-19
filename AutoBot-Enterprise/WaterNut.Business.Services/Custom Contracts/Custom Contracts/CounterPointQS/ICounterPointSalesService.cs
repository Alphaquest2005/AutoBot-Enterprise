
using System;
using System.ServiceModel;
using System.Threading.Tasks;
using Core.Common.Business.Services;
using CounterPointQS.Business.Entities;

namespace CounterPointQS.Business.Services
{
    
    public partial interface ICounterPointSalesService
    {
        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task DownloadCPSales(CounterPointSales counterPointSales, int p);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task DownloadCPSalesDateRange(DateTime startDate, DateTime endDate, int docSetId);

    }
}

