//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace ValuationDS
{
    using System;
    using System.Collections.Generic;
    
    public partial class xC71_Seller_segment
    {
        public string Name { get; set; }
        public string Address { get; set; }
        public int Identification_segment_Id { get; set; }
        public string CountryCode { get; set; }
    
        public virtual xC71_Identification_segment xC71_Identification_segment { get; set; }
    }
}
