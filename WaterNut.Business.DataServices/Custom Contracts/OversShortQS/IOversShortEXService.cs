
using System;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Core.Common.Contracts;
using CoreEntities.Business.Entities;
using OversShortQS.Business.Entities;

namespace OversShortQS.Business.Contracts
{
    
    public partial interface IOversShortEXService
    {
        [OperationContract]
        Task Import(string fileName, string fileType);
        [OperationContract]
        Task SaveReferenceNumber(IEnumerable<OversShortEX> slst, string refTxt);
        [OperationContract]
        Task SaveCNumber(IEnumerable<OversShortEX> slst, string cntxt);
        [OperationContract]
        Task MatchEntries(IEnumerable<OversShortEX> olst);
        [OperationContract]
        Task RemoveSelectedOverShorts(IEnumerable<OversShortEX> lst);
        [OperationContract]
        Task AutoMatch(IEnumerable<OversShortEX> slst);
        [OperationContract]
        Task CreateOversOps(IEnumerable<OversShortEX> selOS, AsycudaDocumentSetEx docSet);
        [OperationContract]
        Task CreateShortsEx9(IEnumerable<OversShortEX> selos, AsycudaDocumentSetEx docSet,
            bool BreakOnMonthYear, bool ApplyEX9Bucket);
    }
}

