

using System.ServiceModel;
using System.Threading.Tasks;
using EntryDataQS.Client.DTO;

namespace EntryDataQS.Client.Contracts
{
   
    public partial interface IContainerExService 
    {
        [OperationContract]
        Task SaveContainer(ContainerEx container);

       
    }
}

