using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Dynamic;
using System.Threading.Tasks;
using AllocationDS.Business.Entities;

namespace WaterNut.DataSpace
{
    public class GetEx9AsycudaSalesAllocations
    {
        public async Task<List<EX9AsycudaSalesAllocations>> Execute(string filterExpression)
        {
            try
            {


                using (var ctx = new AllocationDSContext(){StartTracking = false})
                {
                    ctx.Database.CommandTimeout = 0;

                    ctx.Configuration.AutoDetectChangesEnabled = false;
                    ctx.Configuration.ValidateOnSaveEnabled = false;
                    
                    //if (_ex9AsycudaSalesAllocations == null)
                    //{
                    //////////////////////Cant load all data in memory too much
                   return await ctx.EX9AsycudaSalesAllocations
                        .Where(filterExpression)
                        .Include(x => x.AsycudaSalesAllocationsPIData)
                        .Include("EntryDataDetails.AsycudaDocumentItemEntryDataDetails")
                        .Include("PreviousDocumentItem.EntryPreviousItems.xcuda_PreviousItem.xcuda_Item.AsycudaDocument.Customs_Procedure")
                        .Include("PreviousDocumentItem.AsycudaDocument.Customs_Procedure.CustomsOperations")
                        .Include("PreviousDocumentItem.xcuda_Tarification.xcuda_HScode.TariffCodes.TariffCategory.TariffCategoryCodeSuppUnit.TariffSupUnitLkps")
                        .AsNoTracking()
                        .ToListAsync().ConfigureAwait(false);
                    //}

                    
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
                throw;
            }
        }
    }
}