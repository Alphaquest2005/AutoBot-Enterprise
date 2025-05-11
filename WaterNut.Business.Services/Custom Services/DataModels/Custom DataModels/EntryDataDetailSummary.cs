using System;

namespace WaterNut.DataSpace;

public class EntryDataDetailSummary
{
    public int EntryData_Id { get; set; }
    public string EntryDataId { get; set; }
    public int EntryDataDetailsId { get; set; }
    public DateTime EntryDataDate { get; set; }

    public double QtyAllocated { get; set; }

    public DateTime EffectiveDate { get; set; }

    public string Currency { get; set; }
    public int? LineNumber { get; set; }
    public string Comment { get; set; }
    public int AllocationId { get; set; }
    public int InventoryItemId { get; set; }
    public string ItemNumber { get; set; }
    public string ItemDescription { get; set; }
    public double Cost { get; set; }
    public double Quantity { get; set; }
    public string SourceFile { get; set; }
}