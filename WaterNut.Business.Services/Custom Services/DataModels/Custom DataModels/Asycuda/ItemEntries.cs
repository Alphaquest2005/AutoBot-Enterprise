using System.Collections.Generic;
using AllocationDS.Business.Entities;

namespace WaterNut.DataSpace
{
    public partial class AllocationsBaseModel
    {
        public class ItemEntries
        {
            public string Key { get; set; }
            public List<xcuda_Item> EntriesList { get; set; }
        }
    }
}