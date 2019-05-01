

using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Core.Common.Business.Services;
using CoreEntities.Business.Entities;

namespace CoreEntities.Business.Contracts
{
    
    public partial interface IAsycudaDocumentSetExService
    {
        [OperationContract][FaultContract(typeof(ValidationFault))]
        Task DeleteDocuments(int docSetId);

        [OperationContract][FaultContract(typeof(ValidationFault))]
        Task ImportDocuments(bool onlyRegisteredDocuments, bool importTariffCodes, bool noMessages,
            bool overwriteExisting, List<string> fileNames);

        [OperationContract][FaultContract(typeof(ValidationFault))]
        Task ExportDocuments(string fileName);

        [OperationContract][FaultContract(typeof(ValidationFault))]
        Task ExportDocSet(int docSetId, string directoryName);

        [OperationContract][FaultContract(typeof(ValidationFault))]
        Task SaveAsycudaDocumentSetEx(AsycudaDocumentSetEx asycudaDocumentSetEx);

    }
}

