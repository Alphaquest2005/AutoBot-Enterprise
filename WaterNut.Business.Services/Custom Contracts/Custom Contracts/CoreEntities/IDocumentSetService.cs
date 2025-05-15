

using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Core.Common.Business.Services;
using Core.Common.Contracts;
using CoreEntities.Business.Entities;

namespace CoreEntities.Business.Services
{
    using Serilog;

    [ServiceContract(Namespace = "http://www.insight-software.com/WaterNut")]
    public partial interface IDocumentSetService : IBusinessService
    {
        [OperationContract][FaultContract(typeof(ValidationFault))]
        Task DeleteDocuments(int docSetId);
        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task DeleteDocumentSet(int docSetId);

        [OperationContract][FaultContract(typeof(ValidationFault))]
        Task ImportDocuments(int asycudaDocumentSetId, List<string> fileNames, bool onlyRegisteredDocuments, bool importTariffCodes, bool noMessages,
            bool overwriteExisting, bool linkPi);

        [OperationContract][FaultContract(typeof(ValidationFault))]
        Task ExportDocument(string fileName, int docId);

        [OperationContract][FaultContract(typeof(ValidationFault))]
        Task ExportDocSet(int docSetId, string directoryName);

        [OperationContract][FaultContract(typeof(ValidationFault))]
        Task SaveAsycudaDocumentSetEx(AsycudaDocumentSetEx asycudaDocumentSetEx, ILogger log);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<AsycudaDocumentSetEx> NewDocumentSet(int applicationSettingsId);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task BaseDataModelInitialize();

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task CleanBond(int docSetId, bool perIM7);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task CleanEntries(int docSetId, IEnumerable<int> lst, bool perIM7);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task CleanLines(int docSetId, IEnumerable<int> lst, bool perIM7);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task AttachDocuments(int asycudaDocumentSetId, List<string> files);

    }
}

