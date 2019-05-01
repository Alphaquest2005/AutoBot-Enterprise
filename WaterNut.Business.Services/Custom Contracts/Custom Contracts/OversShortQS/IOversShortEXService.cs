using System.Collections.Generic;
using System.ServiceModel;
using System.Text;
using System.Threading.Tasks;
using Core.Common.Business.Services;

namespace AdjustmentQS.Business.Services
{

    public partial interface IOversShortEXService
    {

        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task Import(string fileName, string fileType, int docSetId, bool overWriteExisting);
        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task SaveReferenceNumber(IEnumerable<int> slst, string refTxt);
        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task SaveCNumber(IEnumerable<int> slst, string cntxt);
        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task MatchEntries(IEnumerable<int> olst);
        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task RemoveSelectedOverShorts(IEnumerable<int> lst);
        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task AutoMatch(IEnumerable<int> slst);
        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task CreateOversOps(IEnumerable<int> selOS, int docSet);
        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task CreateShortsEx9(IEnumerable<int> selos, int docSet,
            bool BreakOnMonthYear, bool ApplyEX9Bucket);
        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task<StringBuilder> BuildOSLst(List<int> lst);
    }
}

