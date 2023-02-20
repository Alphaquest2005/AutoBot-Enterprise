using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using AllocationDS.Business.Entities;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.CreatingEx9.GettingEx9SalesAllocations
{
    public class GetEx9AsycudaSalesAllocationsMem : IGetEx9AsycudaSalesAllocations
    {
        private ConcurrentDictionary<int , EX9AsycudaSalesAllocations> _eX9AsycudaSalesAllocations = null;

        static readonly object Identity = new object();
        public GetEx9AsycudaSalesAllocationsMem(string filterExp, string rdateFilter)
        {
            lock (Identity)
            {
                if (_eX9AsycudaSalesAllocations != null) return;
                try
                {


                    using (var ctx = new AllocationDSContext() { StartTracking = false })
                    {
                        ctx.Database.CommandTimeout = 0;

                        ctx.Configuration.AutoDetectChangesEnabled = false;
                        ctx.Configuration.ValidateOnSaveEnabled = false;

                        //if (_ex9AsycudaSalesAllocations == null)
                        //{
                        //////////////////////Cant load all data in memory too much
                        var lst = ctx.EX9AsycudaSalesAllocations
                            .Where(rdateFilter)
                            .Include(x => x.AsycudaSalesAllocationsPIData)
                            .Include("EntryDataDetails.AsycudaDocumentItemEntryDataDetails")
                            .Include("PreviousDocumentItem.EntryPreviousItems.xcuda_PreviousItem.xcuda_Item.AsycudaDocument.Customs_Procedure")
                            .Include("PreviousDocumentItem.AsycudaDocument.Customs_Procedure.CustomsOperations")
                            .Include("PreviousDocumentItem.xcuda_Tarification.xcuda_HScode.TariffCodes.TariffCategory.TariffCategoryCodeSuppUnit.TariffSupUnitLkps")
                            .AsNoTracking()
                            .ToList()
                            .AsQueryable()
                            .Where(filterExp).
                            ToList();
                        //}

                        _eX9AsycudaSalesAllocations = new ConcurrentDictionary<int, EX9AsycudaSalesAllocations>(lst.ToDictionary(x => x.AllocationId, x => x));
                    }
                }
                catch (Exception e)
                {
                    Console.WriteLine(e);
                    throw;
                }
            }
        }

        public GetEx9AsycudaSalesAllocationsMem()
        {
            
        }

        public async Task<List<EX9AsycudaSalesAllocations>> Execute(string filterExpression)
        {
            return await _eX9AsycudaSalesAllocations
                .Select(x => x.Value)
                .AsQueryable()
                .Where(filterExpression)
                .ToListAsync()
                .ConfigureAwait(false);
        }
    }
}