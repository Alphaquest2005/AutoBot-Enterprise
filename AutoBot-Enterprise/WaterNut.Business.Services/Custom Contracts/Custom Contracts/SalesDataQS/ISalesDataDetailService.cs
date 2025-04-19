

using System.ServiceModel;
using System.Threading.Tasks;
using Core.Common.Business.Services;
using SalesDataQS.Business.Entities;

namespace SalesDataQS.Business.Services
{
    
    public partial interface ISalesDataDetailService
    {
        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task SaveSalesDataDetail(SalesDataDetail salesDataDetail);

       

    }
}

