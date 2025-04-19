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


        public IEnumerable<AllocationDataBlock> Execute(string filterExpression,
            List<string> errors,
            (string currentFilter, string dateFilter, DateTime startDate, DateTime endDate) filter, GetEx9DataMem getEx9DataMem, bool perIM7)
        {

            StatusModel.Timer("Getting ExBond Data");
            var ex9Data =  getEx9DataMem.Execute(filterExpression, filter.dateFilter);
            var slstSource = ex9Data.Where(x => !errors.Contains(x.ItemNumber));
            StatusModel.StartStatusUpdate("Creating xBond Entries", slstSource.Count());
            foreach (var block in CreateWholeAllocationDataBlocks(slstSource, filter, perIM7))
            {
                yield return block;
            }

        }


        private IEnumerable<AllocationDataBlock> CreateWholeAllocationDataBlocks(IEnumerable<EX9Allocations> slstSource,
            (string currentFilter, string dateFilter, DateTime startDate, DateTime endDate) filter, bool perIM7)
        {
            if (perIM7)
            {
                foreach (var block in CreateWholeIM7AllocationDataBlocks(slstSource, filter))
                {
                    yield return block;
                }
            }
            else
            {
                foreach (var block in CreateWholeNonIM7AllocationDataBlocks(slstSource, filter))
                {
                    yield return block;
                }
            }
        }


        private IEnumerable<AllocationDataBlock> CreateWholeNonIM7AllocationDataBlocks(
            IEnumerable<EX9Allocations> slstSource,
            (string currentFilter, string dateFilter, DateTime startDate, DateTime endDate) filter)
        {
            var source = slstSource.OrderBy(x => x.pTariffCode);

            foreach (var g in source
                         .GroupBy(s => new { s.DutyFreePaid, s.Type, MonthYear = "NoMTY", s.PreviousItem_Id }))
            {
                yield return new AllocationDataBlock
                {
                    Type = g.Key.Type,
                    MonthYear = g.Key.MonthYear,
                    DutyFreePaid = g.Key.DutyFreePaid,
                    PreviousItem_Id = g.Key.PreviousItem_Id,
                    Allocations = g.ToList(),
                    Filter = filter
                };
            }
        }


        private IEnumerable<AllocationDataBlock> CreateWholeIM7AllocationDataBlocks(
            IEnumerable<EX9Allocations> slstSource,
            (string currentFilter, string dateFilter, DateTime startDate, DateTime endDate) filter)
        {
            
                foreach (var g in slstSource.OrderBy(x => x.pTariffCode)
                             .GroupBy(s => new
                             {
                                 s.DutyFreePaid,
                                 s.Type,
                                 MonthYear = "NoMTY",
                                 CNumber = s.pCNumber
                             }))
                {
                    yield return new AllocationDataBlock
                    {
                        Type = g.Key.Type,
                        MonthYear = g.Key.MonthYear,
                        DutyFreePaid = g.Key.DutyFreePaid,
                        Allocations = g.ToList(),
                        CNumber = g.Key.CNumber,
                        Filter = filter
                    };
                }
           
        }
    }
}