//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace CoreEntities
{
    using System;
    using System.Collections.Generic;
    
    public partial class AsycudaItemBasicInfo
    {
        public int ASYCUDA_Id { get; set; }
        public int Item_Id { get; set; }
        public string ItemNumber { get; set; }
        public Nullable<double> ItemQuantity { get; set; }
        public double DPQtyAllocated { get; set; }
        public double DFQtyAllocated { get; set; }
        public Nullable<bool> IsAssessed { get; set; }
        public int LineNumber { get; set; }
        public string CNumber { get; set; }
        public Nullable<System.DateTime> RegistrationDate { get; set; }
        public Nullable<int> AsycudaDocumentSetId { get; set; }
        public string Commercial_Description { get; set; }
        public string TariffCode { get; set; }
        public Nullable<int> ApplicationSettingsId { get; set; }
        public string EntryDataType { get; set; }
    }
}
