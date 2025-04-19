
using System;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Core.Common.Business.Services;
using Core.Common.Contracts;
using CounterPointQS.Business.Entities;

namespace CounterPointQS.Business.Contracts
{
  
    public partial interface ICounterPointPOsService
    {
        [OperationContract]
        [FaultContract(typeof(ValidationFault))]
        Task DownloadCPO(CounterPointPOs c, int asycudaDocumentSetId);
    }
}

