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
    
    public partial class AsycudaDocumentItemEntryDataDetails
    {
        public int EntryDataDetailsId { get; set; }
        public int Item_Id { get; set; }
        public string ItemNumber { get; set; }
        public string key { get; set; }
        public string DocumentType { get; set; }
        public Nullable<double> Quantity { get; set; }
        public bool ImportComplete { get; set; }
        public int EntryData_Id { get; set; }
        public string CustomsProcedure { get; set; }
        public int Asycuda_id { get; set; }
        public string EntryDataType { get; set; }
        public int ApplicationSettingsId { get; set; }
        public int AsycudaDocumentSetId { get; set; }
        public string CNumber { get; set; }
        public int LineNumber { get; set; }
        public string CustomsOperation { get; set; }
        public Nullable<long> Id { get; set; }
    
        public virtual AsycudaDocumentItem AsycudaDocumentItem { get; set; }
    }
}