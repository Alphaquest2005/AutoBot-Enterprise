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
    
    public partial class LicenceSummary
    {
        public string TariffCode { get; set; }
        public Nullable<double> Quantity { get; set; }
        public int Total { get; set; }
        public string TariffCodeDescription { get; set; }
        public int AsycudaDocumentSetId { get; set; }
        public long RowNumber { get; set; }
        public int ApplicationSettingsId { get; set; }
    
        public virtual AsycudaDocumentSetEx AsycudaDocumentSetEx { get; set; }
    }
}