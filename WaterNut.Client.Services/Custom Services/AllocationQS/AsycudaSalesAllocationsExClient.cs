
using System;
using System.Linq;
using System.Collections.Generic;
using System.ServiceModel;
using System.Threading.Tasks;
using AllocationQS.Client.DTO;
using AllocationQS.Client.Contracts;
using CoreEntities.Client.DTO;
using PreviousDocumentQS.Client.DTO;
using WaterNut.Client.Services;
using Core.Common.Contracts;
using System.ComponentModel.Composition;


namespace AllocationQS.Client.Services
{
  
     public partial class AsycudaSalesAllocationsExClient 
    {
        
        public Task<IEnumerable<AsycudaSalesAllocationsEx>> GetAsycudaSalesAllocationsExesByDates(DateTime startDate, DateTime endDate)
        {
            return Channel.GetAsycudaSalesAllocationsExesByDates(startDate,endDate);
        }

    }
}

