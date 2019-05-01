
using System.ServiceModel;
using System.Threading.Tasks;
using Core.Common.Business.Services;
using EntryDataQS.Business.Entities;

namespace EntryDataQS.Business.Services
{
    public partial interface IContainerExService 
    {
        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task SaveContainer(ContainerEx container);

   

    }
}

