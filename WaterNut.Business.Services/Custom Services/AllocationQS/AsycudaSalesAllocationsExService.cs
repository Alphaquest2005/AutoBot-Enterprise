
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.ServiceModel;
using System.Threading.Tasks;
using AllocationQS.Business.Entities;

namespace AllocationQS.Business.Services
{
  
    public partial class AsycudaSalesAllocationsExService
    {
        //private readonly AllocationQSContext dbContext;


        public async Task<IEnumerable<AsycudaSalesAllocationsEx>> GetAsycudaSalesAllocationsExesByDates(DateTime startDate, DateTime endDate)
        {
            try
            {
                using (var _dbContext = new AllocationQSContext())
                {
                    IEnumerable<AsycudaSalesAllocationsEx> entities = await _dbContext.AsycudaSalesAllocationsExs
                                                                                .Where(x => x.InvoiceNo != null
                                                                              && x.InvoiceDate >= startDate
                                                                              && x.InvoiceDate <= endDate)

                                                                        .OrderByDescending(x => x.InvoiceDate)
                                                                        .ToListAsync().ConfigureAwait(false);
                    return entities;
                }
            }
            catch (Exception updateEx)
            {
                throw new FaultException(updateEx.Message);
            }
        }


    }
}



