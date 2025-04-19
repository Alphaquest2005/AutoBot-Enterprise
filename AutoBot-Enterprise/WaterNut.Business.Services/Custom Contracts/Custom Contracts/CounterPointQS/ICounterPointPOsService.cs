using System.ServiceModel;
using System.Threading.Tasks;
using Core.Common.Business.Services;
using CounterPointQS.Business.Entities;

namespace CounterPointQS.Business.Services
{
  
    public partial interface ICounterPointPOsService
    {
        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task DownloadCPO(CounterPointPOs c, int asycudaDocumentSetId);
    }
}

