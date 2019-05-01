
using System;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using Core.Common.Business.Services;
using AllocationQS.Business.Entities;

namespace AllocationQS.Business.Services
{
    
    public partial interface IAsycudaSalesAllocationsExService
    {
        [OperationContract][FaultContract(typeof(ValidationFault))]
        Task<IEnumerable<AsycudaSalesAllocationsEx>> GetAsycudaSalesAllocationsExesByDates(DateTime startDate, DateTime endDate);
        
        
    }
}

