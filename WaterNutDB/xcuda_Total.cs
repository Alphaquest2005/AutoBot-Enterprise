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
    
    public partial class xcuda_Total
    {
        public float Total_invoice { get; set; }
        public float Total_weight { get; set; }
        public int Valuation_Id { get; set; }
    
        public virtual xcuda_Valuation xcuda_Valuation { get; set; }
    }
}
