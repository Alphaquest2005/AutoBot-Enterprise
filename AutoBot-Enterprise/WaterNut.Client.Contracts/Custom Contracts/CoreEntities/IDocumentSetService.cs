

using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Core.Common.Contracts;
using CoreEntities.Client.DTO;

namespace CoreEntities.Client.Contracts
{
    [ServiceContract(Namespace = "http://www.insight-software.com/WaterNut")]
    public partial interface IDocumentSetService : IClientService
    {
        [OperationContract]
        Task DeleteDocuments(int docSetId);

        [OperationContract]
        Task DeleteDocumentSet(int docSetId);

        [OperationContract]
        Task ImportDocuments(int asycudaDocumentSetId, List<string> fileNames, bool onlyRegisteredDocuments, bool importTariffCodes, bool noMessages,
            bool overwriteExisting, bool linkPi);

        [OperationContract]
        Task ExportDocument(string fileName, int docId);

        [OperationContract]
        Task ExportDocSet(int docSetId, string directoryName);

        [OperationContract]
        Task SaveAsycudaDocumentSetEx(AsycudaDocumentSetEx asycudaDocumentSetEx);

        [OperationContract]
        Task BaseDataModelInitialize();

        [OperationContract]
        Task<AsycudaDocumentSetEx> NewDocumentSet(int applicationSettingsId);

        [OperationContract]
        Task CleanBond(int docSetId, bool perIM7);

        [OperationContract]
        Task CleanEntries(int docSetId, IEnumerable<int> lst, bool perIM7);

        [OperationContract]
        Task CleanLines(int docSetId, IEnumerable<int> lst, bool perIM7);
        [OperationContract]
        Task AttachDocuments(int asycudaDocumentSetId, List<string> files);
    }
}

