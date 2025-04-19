
using System.ServiceModel;
using System.Threading.Tasks;
using Core.Common.Business.Services;
using CoreEntities.Business.Entities;

namespace CoreEntities.Business.Contracts
{
   public partial interface IAsycudaDocumentService
   {
       [OperationContract][FaultContract(typeof(ValidationFault))]
       Task SaveDocument(AsycudaDocument entity);

       [OperationContract][FaultContract(typeof(ValidationFault))]
       Task ExportDocument(int asycudaDocumentId);

   }
}

