using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Core.Common.Business.Services;

namespace EntryDataQS.Business.Services
{
   
    public partial interface IEntryDataExService
    {
        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task AddDocToEntry(IEnumerable<string> lst, int docSetId, bool perInvoice, bool combineEntryDataInSameFile, bool checkPackages);

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task SaveCSV(string droppedFilePath, string fileType, int docSetId,
            bool overWriteExisting);

        //[OperationContract]
        //[FaultContract(typeof(ValidationFault))]
        //Task SavePDF(string droppedFilePath, string fileType, int docSetId, bool overwrite);

      
    }
}

