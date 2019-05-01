
//using System;
//using System.Linq;
//using System.Collections.Generic;
//using System.ServiceModel;
//using System.Text;
//using System.Threading.Tasks;
//using Core.Common.Contracts;
//using CoreEntities.Client.DTO;
//using AdjustmentQS.Client.DTO;

//namespace AdjustmentQS.Client.Contracts
//{
    
//    public partial interface IOversShortEXService
//    {
//        [OperationContract]
//        Task Import(string fileName, string fileType, int docSetId, bool overWriteExisting);

//        [OperationContract]
//        Task SaveReferenceNumber(IEnumerable<int> slst, string refTxt);
//        [OperationContract]
//        Task SaveCNumber(IEnumerable<int> slst, string cntxt);
//        [OperationContract]
//        Task MatchEntries(IEnumerable<int> olst);
//        [OperationContract]
//        Task RemoveSelectedOverShorts(IEnumerable<int> lst);
//        [OperationContract]
//        Task AutoMatch(IEnumerable<int> slst);
//        [OperationContract]
//        Task CreateOversOps(IEnumerable<int> selOS, int docSet);
//        [OperationContract]
//        Task CreateShortsEx9(IEnumerable<int> selos, int docSet,
//            bool applyCurrentChecks, bool process7100);
//        [OperationContract]
//        Task<StringBuilder> BuildOSLst(List<int> lst);
//    }
//}

