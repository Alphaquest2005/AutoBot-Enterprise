using System;
using System.Collections.Generic;
using WaterNut.Business.Services.Custom_Services.DataModels.Custom_DataModels.CreatingEx9;

namespace WaterNut.DataSpace
{
    public class AllocationDataBlock
    {
        public string MonthYear { get; set; }
        public string DutyFreePaid { get; set; }
        public List<EX9Allocations> Allocations { get; set; }
        public string CNumber { get; set; }
        public string Type { get; set; }
        public (string currentFilter, string dateFilter, DateTime startDate, DateTime endDate) Filter { get; set; }
    }
}