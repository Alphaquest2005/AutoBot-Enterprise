//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace AdjustmentQS
{
    using System;
    using System.Collections.Generic;
    
    public partial class xcuda_Item
    {
        public xcuda_Item()
        {
            this.AsycudaSalesAllocations = new HashSet<AsycudaSalesAllocation>();
            this.AdjustmentOversAllocations = new HashSet<AdjustmentOversAllocation>();
        }
    
        public string Amount_deducted_from_licence { get; set; }
        public string Quantity_deducted_from_licence { get; set; }
        public int Item_Id { get; set; }
        public int ASYCUDA_Id { get; set; }
        public string Licence_number { get; set; }
        public string Free_text_1 { get; set; }
        public string Free_text_2 { get; set; }
        public Nullable<int> EntryDataDetailsId { get; set; }
        public int LineNumber { get; set; }
        public Nullable<bool> IsAssessed { get; set; }
        public double DPQtyAllocated { get; set; }
        public double DFQtyAllocated { get; set; }
        public Nullable<System.DateTime> EntryTimeStamp { get; set; }
        public Nullable<bool> AttributeOnlyAllocation { get; set; }
        public Nullable<bool> DoNotAllocate { get; set; }
        public Nullable<bool> DoNotEX { get; set; }
        public bool ImportComplete { get; set; }
        public string WarehouseError { get; set; }
        public double SalesFactor { get; set; }
        public string PreviousInvoiceNumber { get; set; }
        public string PreviousInvoiceLineNumber { get; set; }
        public string PreviousInvoiceItemNumber { get; set; }
        public string EntryDataType { get; set; }
        public Nullable<int> UpgradeKey { get; set; }
    
        public virtual ICollection<AsycudaSalesAllocation> AsycudaSalesAllocations { get; set; }
        public virtual ICollection<AdjustmentOversAllocation> AdjustmentOversAllocations { get; set; }
    }
}
