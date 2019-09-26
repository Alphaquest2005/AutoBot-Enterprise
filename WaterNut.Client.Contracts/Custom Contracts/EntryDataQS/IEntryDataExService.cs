
using System;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Core.Common.Contracts;
using CoreEntities.Client.DTO;
using EntryDataQS.Client.DTO;

namespace EntryDataQS.Client.Contracts
{
   
    public partial interface IEntryDataExService
    {
        [OperationContract]
        Task AddDocToEntry(IEnumerable<string> lst, int docSetId, bool perInvoice, bool combineEntryDataInSameFile);
        
        
        [OperationContract]
        Task SaveCSV(string droppedFilePath, string fileType, int docSetId,
            bool overWriteExisting);
        [OperationContract]
        Task SavePDF(string droppedFilePath, string fileType, int docSetId, bool overwrite);

        [OperationContract]
        Task SaveTXT(string droppedFilePath, string fileType, int docSetId, bool overwrite);
    }
}

