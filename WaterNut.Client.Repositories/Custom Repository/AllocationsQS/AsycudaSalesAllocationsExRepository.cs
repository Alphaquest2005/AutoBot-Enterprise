
using System.Threading.Tasks;
using AllocationQS.Client.Services;
using AllocationQS.Client.Entities;

using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System;

namespace AllocationQS.Client.Repositories 
{
   
    public partial class AsycudaSalesAllocationsExRepository
    {
        
        public IQueryable<AsycudaSalesAllocationsEx> AsycudaSalesAllocationsExByDates(DateTime startDate, DateTime endDate)
        {
            if (LicenseManager.UsageMode == LicenseUsageMode.Designtime) return new List<AsycudaSalesAllocationsEx>().AsQueryable();
            using (var t = new AsycudaSalesAllocationsExClient())
                {
                    var res = t.GetAsycudaSalesAllocationsExesByDates(startDate,endDate).Result;
                    if (res != null)
                    {
                        return res.Select(x => new AsycudaSalesAllocationsEx(x)).AsQueryable();
                    }
                    else
                    {
                        return null;
                    }                    
                }
        }

        public async Task CreateEx9(string filterExpression, bool perIM7, bool process7100, bool applyCurrentChecks, int asycudaDocumentSetId)
        {
            using (var t = new AllocationsClient())
            {
                
                await t.CreateEx9(filterExpression, perIM7, process7100, applyCurrentChecks, asycudaDocumentSetId).ConfigureAwait(false);
            }
        }



        public async Task CreateOPS(string filterExpression, int asycudaDocumentSetId)
        {
            using (var t = new AllocationsClient())
            {
                await t.CreateOPS(filterExpression, asycudaDocumentSetId).ConfigureAwait(false);
            }
        }

        public async Task ManuallyAllocate(int allocationId, int previousDocumentItemId)
        {
            using (var t = new AllocationsClient())
            {
                await t.ManuallyAllocate(allocationId, previousDocumentItemId).ConfigureAwait(false);
            }
        }

        public async Task ClearAllocations(IEnumerable<AsycudaSalesAllocationsEx> alst)
        {
            using (var t = new AllocationsClient())
            {
                await t.ClearAllocations(alst.Select(x => x.AllocationId)).ConfigureAwait(false);
            }
        }

        public async Task ClearAllocations(string filterExpression)
        {
            using (var t = new AllocationsClient())
            {
                await t.ClearAllocationsByFilter(filterExpression).ConfigureAwait(false);
            }
        }

        public async Task CreateIncompOPS(string filterExpression, int asycudaDocumentSetId)
        {
            using (var t = new AllocationsClient())
            {
                await t.CreateIncompOPS(filterExpression, asycudaDocumentSetId).ConfigureAwait(false);
            }
        }

        public async Task AllocateSales(bool itemDescriptionContainsAsycudaAttribute, bool allocateToLastAdjustment)
        {
            using (var t = new AllocationsClient())
            {
                await t.AllocateSales(itemDescriptionContainsAsycudaAttribute, allocateToLastAdjustment).ConfigureAwait(false);
            }
        }

        public async Task ClearAllocations()
        {
            using (var t = new AllocationsClient())
            {
                await t.ClearAllAllocations().ConfigureAwait(false);
            }
        }

        public async Task ReBuildSalesReports()
        {
            using (var t = new AllocationsClient())
            {
                await t.ReBuildSalesReports().ConfigureAwait(false);
            }
        }
    }
}

