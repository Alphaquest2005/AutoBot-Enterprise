
using System.ServiceModel;
using System.Threading.Tasks;
using Core.Common.Business.Services;
using CoreEntities.Business.Entities;

namespace CoreEntities.Business.Services
{
   public partial interface IAsycudaDocumentService
   {
       [OperationContract][FaultContract(typeof(ValidationFault))]
       Task SaveDocument(AsycudaDocument entity);

       [OperationContract]
       [FaultContract(typeof (ValidationFault))]
       Task SaveDocumentCT(AsycudaDocument entity);

       [OperationContract]
       [FaultContract(typeof(ValidationFault))]
       Task DeleteDocument(int asycudaDocumentId);

       [OperationContract][FaultContract(typeof(ValidationFault))]
       Task ExportDocument(string fileName, int asycudaDocumentId);

       [OperationContract]
       [FaultContract(typeof(ValidationFault))]
       Task<AsycudaDocument> NewDocument(int docSetId);

       [OperationContract]
       [FaultContract(typeof (ValidationFault))]
       void IM72Ex9Document(string filename);

   }
}

