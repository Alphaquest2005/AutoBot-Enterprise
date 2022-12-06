
using System.Threading.Tasks;
using AllocationQS.Client.Services;
using AllocationQS.Client.Entities;

using System.Linq;
using System.ComponentModel;
using System.Collections.Generic;
using System;
using CoreEntities.Client.Entities;

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

        public async Task CreateEx9(string filterExpression, bool perIM7, bool process7100, bool applyCurrentChecks, int asycudaDocumentSetId, string documentType, string ex9BucketType, bool isGrouped, bool checkQtyAllocatedGreaterThanPiQuantity, bool checkForMultipleMonths, bool applyEx9Bucket, bool applyHistoricChecks, bool perInvoice, bool autoAssess, bool overPIcheck, bool universalPIcheck, bool itemPIcheck)
        {
            using (var t = new AllocationsClient())
            {
                
                await t.CreateEx9(filterExpression, perIM7, process7100, applyCurrentChecks, asycudaDocumentSetId, documentType, ex9BucketType, isGrouped,
                    checkQtyAllocatedGreaterThanPiQuantity, checkForMultipleMonths, applyEx9Bucket,
                    applyHistoricChecks, perInvoice, autoAssess, overPIcheck, universalPIcheck, itemPIcheck).ConfigureAwait(false);
            }
        }



        public async Task CreateOPS(string filterExpression, int asycudaDocumentSetId, bool perInvoice)
        {
            using (var t = new AllocationsClient())
            {
                await t.CreateOPS(filterExpression, asycudaDocumentSetId, perInvoice).ConfigureAwait(false);
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

        public async Task AllocateSales(ApplicationSettings applicationSettings, bool allocateToLastAdjustment)
        {
            using (var t = new AllocationsClient())
            {
                await t.AllocateSales(applicationSettings.DTO, allocateToLastAdjustment).ConfigureAwait(false);
            }
        }

        public async Task ClearAllocations(int applicationSettingsId)
        {
            using (var t = new AllocationsClient())
            {
                await t.ClearAllAllocations(applicationSettingsId).ConfigureAwait(false);
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

