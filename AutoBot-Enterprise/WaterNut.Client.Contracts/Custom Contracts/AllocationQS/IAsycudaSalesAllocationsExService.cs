
using System;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Core.Common.Contracts;
using AllocationQS.Client.DTO;
using CoreEntities.Client.DTO;
using PreviousDocumentQS.Client.DTO;

namespace AllocationQS.Client.Contracts
{
   
    public partial interface IAsycudaSalesAllocationsExService
    {
        [OperationContract]
        Task<IEnumerable<AsycudaSalesAllocationsEx>> GetAsycudaSalesAllocationsExesByDates(DateTime startDate, DateTime endDate);
      

    }
}

