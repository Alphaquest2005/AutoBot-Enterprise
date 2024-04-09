using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Core.Common.UI;
using WaterNut.DataSpace;

namespace WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.CreatingEx9
{
    public class CreateAllocationDataBlocks
    {
       

        static CreateAllocationDataBlocks()
        {
        }

      
        public async Task<IEnumerable<AllocationDataBlock>> Execute(string filterExpression,
            List<string> errors,
            (string currentFilter, string dateFilter, DateTime startDate, DateTime endDate) filter, GetEx9DataMem getEx9DataMem, bool perIM7)
        {
            try
            {
                StatusModel.Timer("Getting ExBond Data");
                var ex9Data = await getEx9DataMem.Execute(filterExpression, filter.dateFilter).ConfigureAwait(false);
                var slstSource = ex9Data.Where(x => !errors.Contains(x.ItemNumber)).ToList();
                StatusModel.StartStatusUpdate("Creating xBond Entries", slstSource.Count());
                IEnumerable<AllocationDataBlock> slst;
                slst = CreateWholeAllocationDataBlocks(slstSource, filter, perIM7);
                return slst;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private IEnumerable<AllocationDataBlock> CreateWholeAllocationDataBlocks(IEnumerable<EX9Allocations> slstSource,
            (string currentFilter, string dateFilter, DateTime startDate, DateTime endDate) filter, bool perIM7)
        {
            IEnumerable<AllocationDataBlock> slst;
            if (perIM7 == true)
            {
                slst = CreateWholeIM7AllocationDataBlocks(slstSource,filter);
            }
            else
            {
                slst = CreateWholeNonIM7AllocationDataBlocks(slstSource, filter);
            }
            return slst;
        }

        private IEnumerable<AllocationDataBlock> CreateWholeNonIM7AllocationDataBlocks(
            IEnumerable<EX9Allocations> slstSource,
            (string currentFilter, string dateFilter, DateTime startDate, DateTime endDate) filter)
        {
            try
            {

                IEnumerable<AllocationDataBlock> slst;
                var source = slstSource.OrderBy(x => x.pTariffCode).ToList();

                slst = from s in source
                    group s by new {s.DutyFreePaid, s.Type, MonthYear = "NoMTY", s.PreviousItem_Id}
                    into g
                    select new AllocationDataBlock
                    {
                        Type = g.Key.Type,
                        MonthYear = g.Key.MonthYear,
                        DutyFreePaid = g.Key.DutyFreePaid,
                        PreviousItem_Id = g.Key.PreviousItem_Id,
                        Allocations = g.ToList(),
                        Filter = filter
                    };
                return slst;
            }
            catch (Exception)
            {

                throw;
            }
        }

        private IEnumerable<AllocationDataBlock> CreateWholeIM7AllocationDataBlocks(
            IEnumerable<EX9Allocations> slstSource,
            (string currentFilter, string dateFilter, DateTime startDate, DateTime endDate) filter)
        {
            try
            {
                IEnumerable<AllocationDataBlock> slst;
                slst = from s in slstSource.OrderBy(x => x.pTariffCode)
                    group s by
                        new
                        {
                            s.DutyFreePaid,
                            s.Type,
                            MonthYear = "NoMTY",
                            CNumber = s.pCNumber
                        }
                    into g
                    select new AllocationDataBlock
                    {
                        Type = g.Key.Type,
                        MonthYear = g.Key.MonthYear,
                        DutyFreePaid = g.Key.DutyFreePaid,
                        Allocations = g.ToList(),
                        CNumber = g.Key.CNumber,
                        Filter = filter
                    };
                return slst;
            }
            catch (Exception)
            {

                throw;
            }
        }
    }
}