//------------------------------------------------------------------------------
// <auto-generated>
//    This code was generated from a template.
//
//    Manual changes to this file may cause unexpected behavior in your application.
//    Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace WaterNutDB
{
    #pragma warning disable 1573
    using System;
    using System.Collections.Generic;
    
    public partial class xcuda_Identification
    {
        public int ASYCUDA_Id { get; set; }
        public string Manifest_reference_number { get; set; }
    
        public virtual xcuda_Assessment xcuda_Assessment { get; set; }
        public virtual xcuda_ASYCUDA xcuda_ASYCUDA { get; set; }
        public virtual xcuda_Office_segment xcuda_Office_segment { get; set; }
        public virtual xcuda_receipt xcuda_receipt { get; set; }
        public virtual xcuda_Registration xcuda_Registration { get; set; }
        public virtual xcuda_Type xcuda_Type { get; set; }
    }
}
