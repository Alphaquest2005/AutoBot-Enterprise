using System.Collections.Generic;

namespace WaterNut.DataSpace
{
    public class AllocationDataBlock
    {
        public string MonthYear { get; set; }
        public string DutyFreePaid { get; set; }
        public List<EX9Allocations> Allocations { get; set; }
        public string CNumber { get; set; }
        public string Type { get; set; }
    }
}