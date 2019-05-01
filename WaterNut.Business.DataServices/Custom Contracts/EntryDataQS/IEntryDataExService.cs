
using System;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Core.Common.Business.Services;
using Core.Common.Contracts;
using CoreEntities.Business.Entities;
using EntryDataQS.Business.Entities;

namespace EntryDataQS.Business.Contracts
{
   
    public partial interface IEntryDataExService
    {
        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task AddDocToEntry(IEnumerable<EntryDataEx> lst);
        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task RemoveSelectedEntryData(IEnumerable<EntryDataEx> selectedEntryDataEx);
        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task SaveCSV(string droppedFilePath, string fileType, AsycudaDocumentSetEx docSet,
            bool overWriteExisting);
    }
}

